namespace OpenTrackLogger.ViewModels
{
    using System;
    using System.Reactive.Disposables;

    using OpenTrackLogger.Services;
    using OpenTrackLogger.Views;

    using ReactiveUI;
    using ReactiveUI.Mobile;
    using ReactiveUI.NLog;

    public class AppBootstrapper : ReactiveObject, IApplicationRootState, IDisposable
    {
        private readonly IDisposable _cleanupDisposable;

        public IRoutingState Router { get; private set; }

        public AppBootstrapper()
        {
            // TODO: move this out of the constructor
            CreateDatabase();

            Router = new RoutingState();

            var resolver = RxApp.MutableResolver;

            resolver.RegisterConstant(new DebugLogger { Level = LogLevel.Info }, typeof(ILogger));

            var logManager = RxApp.MutableResolver.GetService<ILogManager>();
            RxApp.MutableResolver.RegisterConstant(logManager.GetLogger<NLogLogger>(), typeof(IFullLogger)); 
            
            resolver.RegisterConstant(this, typeof(IApplicationRootState));
            resolver.RegisterConstant(this, typeof(IScreen));

            resolver.RegisterConstant(new BackgroundHost(), typeof(BackgroundHost));
            resolver.RegisterConstant(new GeolocatorService(), typeof(GeolocatorService));
            resolver.RegisterConstant(new LogService(), typeof(LogService));

            resolver.RegisterConstant(new CameraService(), typeof(ICameraService));
            resolver.RegisterConstant(new LocalDriveService(), typeof(ILocalDriveService));

            resolver.RegisterConstant(new TrackExportService(), typeof(ITrackExportService));

            resolver.RegisterConstant(new OneDriveClientService(), typeof(IOneDriveClientService));
            resolver.RegisterConstant(new OneDriveService(), typeof(IOneDriveService));

            // TODO: why is the lambda being called twice???
            resolver.Register(() => new TrackTrackingDetailView(), typeof(IViewFor<TrackTrackingViewModel>));
            resolver.Register(() => new TrackListView(), typeof(IViewFor<TrackListViewModel>));
            resolver.Register(() => new TrackView(), typeof(IViewFor<TrackViewViewModel>));
            resolver.Register(() => new UploadTrackView(), typeof(IViewFor<UploadTrackViewModel>));

            //Router.Navigate.Execute(new TrackTrackingViewModel(cameraService, localDriveService, this));
            Router.Navigate.Execute(new TrackListViewModel(this));
            _cleanupDisposable = new CompositeDisposable();
        }

        private static void CreateDatabase()
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString))
            {
                if (!db.DatabaseExists()) db.CreateDatabase();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AppBootstrapper() 
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_cleanupDisposable != null)
                _cleanupDisposable.Dispose();
        }
    }
}
