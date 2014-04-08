namespace OpenTrackLogger.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.Storage;

    using Microsoft.Live;

    using OpenTrackLogger.Secrets;

    public class OneDriveService
    {
        LiveConnectClient _client;

        private async Task<LiveConnectClient> GetClient()
        {
            if (_client != null) return _client;

            var loginResult = await new LiveAuthClient(Secrets.Data.ClientId).LoginAsync(new[] { "wl.signin", "wl.skydrive_update" });
            if (loginResult.Status != LiveConnectSessionStatus.Connected) {
                throw new Exception("Failed to Connect");
            }
            _client = new LiveConnectClient(loginResult.Session);

            return _client;
        }

        public async Task UploadToSkyDrive(StorageFolder folder)
        {
            var client = await GetClient();

            dynamic result = (await client.PostAsync("me/skydrive", new Dictionary<string, object> { { "name", folder.Name } })).Result;
            var folderId = result.id;
            var files = (await folder.GetFilesAsync());
            foreach (var storageFile in files.Cast<StorageFile>()) {
                using (var readStream = await storageFile.OpenStreamForReadAsync()) {
                    await client.UploadAsync(folderId, storageFile.Name, readStream, OverwriteOption.Overwrite);
                }
            }
        }


        //private IObservable<LiveConnectSession> GetSession(Action<Exception> errorHandler)
        //{
        //    return Observable.Defer(async () => {
        //        if (_session == null) {
        //            try {
        //                var loginResult = await new LiveAuthClient("0000000048110A43").LoginAsync(new[] { "wl.skydrive", "wl.skydrive_update" });
        //                if (loginResult.Status != LiveConnectSessionStatus.Connected) {
        //                    errorHandler(new Exception("FailedToConnect"));
        //                    return Observable.Empty<LiveConnectSession>();
        //                }
        //                _session = loginResult.Session;
        //            }
        //            catch (Exception ex) {
        //                errorHandler(ex);
        //                return Observable.Empty<LiveConnectSession>();
        //            }
        //        }
        //        return Observable.Return(_session);
        //    });
        //}

        //public IObservable<LiveConnectSession> UploadToSkyDrive(XDocument xDocument, Action<Exception> errorHandler)
        //{
        //    return GetSession(errorHandler)
        //        .Select(x => new LiveConnectClient(x))
        //        .Select(async x => {
        //            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".gpx";
        //            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(@"Shared\Transfers");
        //            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
        //            using (var writeStream = await file.OpenStreamForWriteAsync()) {
        //                xDocument.Save(writeStream);
        //            }
        //            //var result = await x.GetAsync("me");
        //            //isfStream.Write(output, 0, output.Length);
        //            //isfStream.Close();
                    
        //            using (var readStream = await file.OpenStreamForReadAsync()) {
        //                var res = await x.UploadAsync("me/skydrive", filename, readStream, OverwriteOption.Overwrite);
        //            }
        //            //var fff = filename;
        //            //var gg = fff.First();
        //            return true;
        //        })
        //        .SelectMany(x => Observable.Empty<LiveConnectSession>());
        //}
    }
}
