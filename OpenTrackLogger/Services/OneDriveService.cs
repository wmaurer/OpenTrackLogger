namespace OpenTrackLogger.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Storage;

    using Microsoft.Live;
    using Microsoft.Phone.BackgroundTransfer;

    using OpenTrackLogger.Mixins;

    public interface IOneDriveService
    {
        IObservable<Unit> UploadToSkyDrive(IOneDriveClientService oneDriveClientService, string filename);
        IObservable<LiveOperationProgress> UploadProgress { get; }
    }

    public class OneDriveService : IOneDriveService
    {
        private readonly Progress<LiveOperationProgress> _uploadProgress;

        public OneDriveService()
        {
            _uploadProgress = new Progress<LiveOperationProgress>();
            UploadProgress = _uploadProgress.Events().ProgressChanged;
        }

        public IObservable<Unit> UploadToSkyDrive(IOneDriveClientService oneDriveClientService, string filename)
        {
            return Observable.StartAsync(async () => {
                var client = await oneDriveClientService.GetClient();

                var result = (await client.GetAsync("me/skydrive/files")).Result;
                var data = (List<object>)result["data"];
                var folderId = string.Empty;
                foreach (var album in data.Cast<IDictionary<string, object>>().Where(album => (string)album["name"] == "OpenTrackLogger")) {
                    if ((string)album["type"] != "folder")
                        throw new Exception(string.Format("onedrive item OpenTrackLogger is not a folder. type is {0}", album["type"]));
                    folderId = (string)album["id"];
                }

                if (string.IsNullOrEmpty(folderId)) {
                    dynamic createFolderResult = (await client.PostAsync("me/skydrive", new Dictionary<string, object> { { "name", "OpenTrackLogger" } })).Result;
                    folderId = createFolderResult.Id;
                }

                var thisZipFileUri = new Uri("\\shared\\transfers\\" + filename, UriKind.Relative);
                // TODO: ask user if they want to cancel pending request
                var reqList = BackgroundTransferService.Requests.Where(x => x.UploadLocation.Equals(thisZipFileUri)).ToList();
                foreach (var backgroundTransferRequest in reqList) {
                    BackgroundTransferService.Remove(backgroundTransferRequest);
                }
                // TODO: check if file exists on onedrive before attempting to upload
                await client.BackgroundUploadAsync(folderId, thisZipFileUri, OverwriteOption.Overwrite, new CancellationToken(), _uploadProgress);
            });
        }

        public IObservable<LiveOperationProgress> UploadProgress { get; private set; } 
    }
}
