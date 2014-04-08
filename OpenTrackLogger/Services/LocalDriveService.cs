namespace OpenTrackLogger.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Windows.Storage;

    public interface ILocalDriveService
    {
        Task<StorageFolder> GetTrackFolder(string trackTimeId);
        Task SavePhoto(string trackTimeId, string filename, Stream photoStream);
        Task ExportTrack(string trackTimeId, Action<Stream> writerAction);
    }

    public class LocalDriveService : ILocalDriveService
    {
        private static StorageFolder _transfersFolder;

        private static async Task<StorageFolder> GetTransfersFolder()
        {
            if (_transfersFolder != null) return _transfersFolder;
            var sharedFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shared", CreationCollisionOption.OpenIfExists);
            return _transfersFolder = await sharedFolder.CreateFolderAsync("Transfers", CreationCollisionOption.OpenIfExists);
        }

        public async Task<StorageFolder> GetTrackFolder(string trackTimeId)
        {
            return await (await GetTransfersFolder()).CreateFolderAsync(trackTimeId, CreationCollisionOption.OpenIfExists);
        }

        public async Task SavePhoto(string trackTimeId, string filename, Stream photoStream)
        {
            var folder = await GetTrackFolder(trackTimeId);
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var writeStream = await file.OpenStreamForWriteAsync()) {
                await photoStream.CopyToAsync(writeStream);
            }
        }

        public async Task ExportTrack(string trackTimeId, Action<Stream> writerAction)
        {
            var folder = await GetTrackFolder(trackTimeId);
            var filename = string.Format("{0}.gpx", trackTimeId);
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var writeStream = await file.OpenStreamForWriteAsync()) {
                writerAction(writeStream);
            }
        }
    }
}
