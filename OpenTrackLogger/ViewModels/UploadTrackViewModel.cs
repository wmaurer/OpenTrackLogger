namespace OpenTrackLogger.ViewModels
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;

    using OpenTrackLogger.Mixins;
    using OpenTrackLogger.Services;

    using ReactiveUI;

    public class UploadTrackViewModel : ReactiveObject, IRoutableViewModel
    {
        private IOneDriveClientService _oneDriveClientService;

        public UploadTrackViewModel(IScreen screen = null)
        {
            HostScreen = screen;

            var localDriveService = RxApp.DependencyResolver.GetService<ILocalDriveService>();
            var oneDriveService = RxApp.DependencyResolver.GetService<IOneDriveService>();
            var trackExportService = RxApp.DependencyResolver.GetService<ITrackExportService>();

            var getOneDriveClient = new ReactiveCommand();
            getOneDriveClient.ThrownExceptions.LogException("getOneDriveClient received exception");
            getOneDriveClient.ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(x => _oneDriveClientService.GetClient().ToObservable())
                .ObserveOn(RxApp.TaskpoolScheduler)
                .SelectMany(_ => localDriveService.ExportTrackToZip(_trackSummary.CreatedAtId, s => trackExportService.WriteTrackToStream(_trackSummary.Model, s)))
                .Select(x => oneDriveService.UploadToSkyDrive(_oneDriveClientService, x.Name))
                .Subscribe();

            var uploadTrack = new ReactiveCommand();
            uploadTrack.ThrownExceptions.LogException("uploadTrack received exception");
            uploadTrack.InvokeCommand(getOneDriveClient);

            var trackExportProgress = trackExportService.TrackExportProgress
                .Distinct(x => (int)x.PercentageComplete)
                .Where(x => (int)x.PercentageComplete == 100 || x.TotalNodes > 500 || x.NodesWritten % 5 == 0);

            _exportProgressText = trackExportProgress
                .Select(x => string.Format("{0} / {1}", x.NodesWritten, x.TotalNodes))
                .ToProperty(this, x => x.ExportProgressText);

            _exportProgress = trackExportProgress
                .Select(x => x.PercentageComplete)
                .ToProperty(this, x => x.ExportProgress);

            _zipProgress = localDriveService.ZipProgress
                .Select(x => string.Format("{0} / {1}", x.FilesWritten, x.TotalFiles))
                .ToProperty(this, x => x.ZipProgress);

            _uploadPercentage = oneDriveService.UploadProgress
                .Select(x => x.ProgressPercentage.ToString("N"))
                .ToProperty(this, x => x.UploadPercentage, "0.0");

            oneDriveService.UploadProgress
                .SkipWhile(x => x.ProgressPercentage < 100)
                .Take(1)
                .Delay(TimeSpan.FromSeconds(1), RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => HostScreen.Router.NavigateBack.Execute(null));

            this.WhenNavigatedTo(() => {
                _oneDriveClientService = RxApp.DependencyResolver.GetService<IOneDriveClientService>();
                uploadTrack.Execute(null);
                return Disposable.Empty;
            });

            var backgroundHost = RxApp.DependencyResolver.GetService<BackgroundHost>();
            var supressChangeDisposable = Disposable.Empty;
            var isRunningInBackGround = false;
            backgroundHost.IsRunningInBackground.Subscribe(_ => {
                if (!isRunningInBackGround)
                    supressChangeDisposable = new CompositeDisposable { SuppressChangeNotifications(), Disposable.Create(() => isRunningInBackGround = false) };
                isRunningInBackGround = true;
            });
            backgroundHost.IsResuming.Subscribe(_ => supressChangeDisposable.Dispose());
        }

        public void NavigateToWithParameters(TrackSummaryViewModel trackSummary)
        {
            _trackSummary = trackSummary;
            HostScreen.Router.Navigate.Execute(this);
        }

        private TrackSummaryViewModel _trackSummary;

        private readonly ObservableAsPropertyHelper<double> _exportProgress;
        public double ExportProgress { get { return _exportProgress.Value; } }

        private readonly ObservableAsPropertyHelper<string> _exportProgressText;
        public string ExportProgressText { get { return _exportProgressText.Value; } }

        private readonly ObservableAsPropertyHelper<string> _zipProgress;
        public string ZipProgress { get { return _zipProgress.Value; } }

        private readonly ObservableAsPropertyHelper<string> _uploadPercentage;
        public string UploadPercentage { get { return _uploadPercentage.Value; } }

        public string UrlPathSegment
        {
            get { return "UploadTrack"; }
        }

        public IScreen HostScreen { get; private set; }
    }
}
