namespace OpenTrackLogger.ViewModels
{
    using System;
    using System.Reactive.Disposables;

    using Microsoft.Phone.Data.Linq;

    using OpenTrackLogger.Services;
    using OpenTrackLogger.Views;

    using ReactiveUI;
    using ReactiveUI.Mobile;

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

            // TODO: why is the lambda being called twice???
            resolver.Register(() => new TrackTrackingDetailView(), typeof(IViewFor<TrackTrackingViewModel>));

            resolver.RegisterConstant(this, typeof(IApplicationRootState));
            resolver.RegisterConstant(this, typeof(IScreen));
            resolver.RegisterConstant(new BackgroundHost(), typeof(BackgroundHost));
            resolver.RegisterConstant(new GeolocatorService(), typeof(GeolocatorService));

            var cameraService = new CameraService();
            var localDriveService = new LocalDriveService();

            Router.Navigate.Execute(new TrackTrackingViewModel(cameraService, localDriveService, this));
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
