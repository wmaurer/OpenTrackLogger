namespace OpenTrackLogger.Services
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.Storage;

    using Microsoft.Phone.Reactive;

    using Observable = System.Reactive.Linq.Observable;

    public class ZipProgress
    {
        public int FilesWritten { get; private set; }
        public int TotalFiles { get; private set; }

        public double PercentageComplete { get; private set; }

        public ZipProgress(int filesWritten, int totalFiles)
        {
            FilesWritten = filesWritten;
            TotalFiles = totalFiles;
            PercentageComplete = FilesWritten / (double)TotalFiles * 100;
        }
    }

    public interface ILocalDriveService
    {
        IObservable<StorageFolder> GetTrackFolder(string trackTimeId);
        Task SavePhoto(string trackTimeId, string filename, Stream photoStream);
        IObservable<StorageFile> ExportTrackToZip(string trackTimeId, Func<Stream, Task> writerAction);
        IObservable<ZipProgress> ZipProgress { get; }
    }

    public class LocalDriveService : ILocalDriveService
    {
        private static StorageFolder _transfersFolder;

        public LocalDriveService()
        {
            _zipProgress = new Subject<ZipProgress>();
        }

        private static async Task<StorageFolder> GetTransfersFolder()
        {
            if (_transfersFolder != null) return _transfersFolder;
            var sharedFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shared", CreationCollisionOption.OpenIfExists);
            return _transfersFolder = await sharedFolder.CreateFolderAsync("Transfers", CreationCollisionOption.OpenIfExists);
        }

        public IObservable<StorageFolder> GetTrackFolder(string trackTimeId)
        {
            return Observable.StartAsync(async () => await GetTrackFolderInternal(trackTimeId));
        }

        private static async Task<StorageFolder> GetTrackFolderInternal(string trackTimeId)
        {
            return await (await GetTransfersFolder()).CreateFolderAsync(trackTimeId, CreationCollisionOption.OpenIfExists);
        }

        public async Task SavePhoto(string trackTimeId, string filename, Stream photoStream)
        {
            var folder = await GetTrackFolderInternal(trackTimeId);
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var writeStream = await file.OpenStreamForWriteAsync()) {
                await photoStream.CopyToAsync(writeStream);
            }
        }

        public IObservable<StorageFile> ExportTrackToZip(string trackTimeId, Func<Stream, Task> writerAction)
        {
            return Observable.StartAsync(async () => {
                var folder = await GetTrackFolderInternal(trackTimeId);
                var filename = string.Format("{0}.gpx", trackTimeId);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                using (var writeStream = await file.OpenStreamForWriteAsync()) {
                    await writerAction(writeStream);
                }

                var fileIndex = 0;
                var zipFile = await (await GetTransfersFolder()).CreateFileAsync(folder.Name + ".zip", CreationCollisionOption.ReplaceExisting);
                using (var archive = new ZipArchive(await zipFile.OpenStreamForWriteAsync(), ZipArchiveMode.Create)) {
                    var files = (await folder.GetFilesAsync()).Cast<StorageFile>().ToList();
                    foreach (var storageFile in files) {
                        var entry = archive.CreateEntry(storageFile.Name);
                        using (var writeStream = entry.Open()) {
                            using (var readStream = await storageFile.OpenStreamForReadAsync()) {
                                await readStream.CopyToAsync(writeStream);
                            }
                        }
                        _zipProgress.OnNext(new ZipProgress(++fileIndex, files.Count));
                    }
                }

                return zipFile;
            });
        }

        private readonly Subject<ZipProgress> _zipProgress;
        public IObservable<ZipProgress> ZipProgress { get { return _zipProgress; } }
    }
}
