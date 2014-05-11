namespace OpenTrackLogger.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Windows.Devices.Geolocation;

    using Microsoft.Phone.Tasks;

    using OpenTrackLogger.Common;
    using OpenTrackLogger.Mixins;
    using OpenTrackLogger.Models;
    using OpenTrackLogger.Services;

    using ReactiveUI;

    public class TrackTrackingViewModel : ReactiveObject, IRoutableViewModel
    {
        public TrackTrackingViewModel(IScreen screen = null)
        {
            var cameraService = RxApp.DependencyResolver.GetService<ICameraService>();
            var localDriveService = RxApp.DependencyResolver.GetService<ILocalDriveService>();

            HostScreen = screen;

            var logger = RxApp.DependencyResolver.GetService<LogService>();

            var geolocatorService = RxApp.DependencyResolver.GetService<GeolocatorService>();
            var localDatabaseService = new LocalDatabaseService();
            Track currentTrack = null;
            Trackpoint currentTrackpoint = null;
            var currenttTrackTimeId = string.Empty;

            var isTracking = this.WhenAnyValue(x => x.IsTracking).StartWith(false);
            var isNotTracking = isTracking.Select(x => !x);
            StartTracking = new ReactiveCommand(isNotTracking);
            StopTracking = new ReactiveCommand(isTracking);

            _status = geolocatorService.StatusChanged
                .Select(x => x.Status.ToString() + " " + DateTime.UtcNow.ToString("yyyyMMddHHmmss"))
                .ToProperty(this, x => x.Status);

            this.WhenAnyValue(x => x.Status)
                .Subscribe(async x => {
                    if (currentTrackpoint != null) {
                        await logger.Log(string.Format("WhenAnyValue(Status): {0} {1} {2},{3}", currentTrackpoint.Id, currentTrackpoint.Timestamp, currentTrackpoint.Latitude, currentTrackpoint.Longitude));
                    }
                    await logger.Log("WhenAnyValue(Status): " + x);                    
                });

            _currentLocation = geolocatorService.PositionChanged
                .Select(x => string.Format("{0:F13},{1:F13}", x.Position.Coordinate.Latitude, x.Position.Coordinate.Longitude))
                .ToProperty(this, x => x.CurrentLocation);

            _elevation = geolocatorService.PositionChanged
                .Select(x => !x.Position.Coordinate.Altitude.HasValue ? string.Empty : x.Position.Coordinate.Altitude.Value.ToString())
                .ToProperty(this, x => x.Elevation);

            var trackpoints = geolocatorService.PositionChanged
                .Select(x => new TrackCoordinate(currentTrack, x.Position.Coordinate))
                .DistinctUntilChanged(TrackCoordinate.EqualityComparer)
                .Select(x => x.Coordinate)
                .Select(x => new Trackpoint(currentTrack.Id, x));

            DateTime? lastReceivedPositionChanged = null;
            _timeSinceLast = geolocatorService.PositionChanged
                .Select(x => {
                    var timeSinceLast = string.Empty;
                    var now = DateTime.Now;
                    if (lastReceivedPositionChanged.HasValue) {
                        timeSinceLast = (now - lastReceivedPositionChanged.Value).TotalMilliseconds.ToString("N0");
                    }
                    lastReceivedPositionChanged = now;
                    return timeSinceLast;
                }).ToProperty(this, x => x.TimeSinceLast);


            trackpoints
                //.Do(x => Debug.WriteLine("{0}: {1}, {2}", DateTime.Now.TimeOfDay, x.Latitude, x.Longitude))
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(x => {
                    localDatabaseService.InsertTrackpoint(x);
                    currentTrackpoint = x;
                });

            var disp = Disposable.Empty;
            // ReSharper disable once InvokeAsExtensionMethod
            Observable.Merge(
                StartTracking.Select(_ => true),
                StopTracking.Select(_ => false))
                .Subscribe(async x => {
                    IsTracking = x;
                    if (IsTracking) {
                        if (currentTrack == null) {
                            currenttTrackTimeId = DateTime.Now.ToString("yyyyMMddHHmmss");
                            currentTrack = localDatabaseService.CreateTrack();
                            await logger.Log(string.Format("Creating new track. Id: {0}. TrackTimeId: {1}", currentTrack.Id, currenttTrackTimeId));
                        }
                        else {
                            await logger.Log(string.Format("Continuing track. Id: {0}. TrackTimeId: {1}", currentTrack.Id, currenttTrackTimeId));
                        }
                        disp = geolocatorService.Start();
                    }
                    else {
                        await logger.Log(string.Format("Stopping track. Id: {0}", currentTrack.Id));
                        disp.Dispose();
                    }   
                });

            var insertPhoto = new ReactiveCommand();
            insertPhoto.ThrownExceptions.LogException("insertPhoto received exception");
            insertPhoto.RegisterAsyncTask(async x => {
                var photoResult = (PhotoResult)x;
                var filename = Path.GetFileName(photoResult.OriginalFileName);
                if (string.IsNullOrEmpty(filename)) return;
                await localDriveService.SavePhoto(currenttTrackTimeId, filename, photoResult.ChosenPhoto);
                var photoWaypoint = new Waypoint { TrackId = currentTrack.Id, WaypointType = (int)WaypointType.Photo, TrackpointId = currentTrackpoint.Id, CreatedAt = DateTime.Now, Link = filename };
                localDatabaseService.InsertPhoto(photoWaypoint);
                await logger.Log(string.Format("insertPhoto: {0} {1} {2},{3}", currentTrackpoint.Id, currentTrackpoint.Timestamp, currentTrackpoint.Latitude, currentTrackpoint.Longitude));
                await logger.Log("insertPhoto: " + photoWaypoint.Id + ", " + photoWaypoint.Link);
            });

            CapturePhoto = new ReactiveCommand(isTracking.CombineLatest(insertPhoto.IsExecuting, (tracking, executing) => tracking && !executing).DistinctUntilChanged());
            CapturePhoto.Subscribe(x => cameraService.CapturePhoto());

            cameraService.PhotoCaptureCompleted
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(insertPhoto.Execute);

            //var sds = new OneDriveService();
            //UploadTrack = new ReactiveCommand(isNotTracking);
            //UploadTrack.ThrownExceptions.LogException("UploadTrack received exception");
            //UploadTrack.RegisterAsyncTask(async o => await sds.UploadToSkyDrive(await localDriveService.GetTrackFolder(currenttTrackTimeId)));

            var backgroundHost = RxApp.DependencyResolver.GetService<BackgroundHost>();
            var supressChangeDisposable = Disposable.Empty;
            var isRunningInBackGround = false;
            backgroundHost.IsRunningInBackground.Subscribe(async _ => {
                await logger.Log("backgroundHost.IsRunningInBackground");
                if (!isRunningInBackGround)
                    supressChangeDisposable = new CompositeDisposable { SuppressChangeNotifications(), Disposable.Create(() => isRunningInBackGround = false) };
                isRunningInBackGround = true;
            });
            backgroundHost.IsResuming.Subscribe(async _ => {
                await logger.Log("backgroundHost.IsResuming");
                supressChangeDisposable.Dispose();                
            });
        }

        private class TrackCoordinate
        {
            private const double _tolerance = 0.0000000000001;

            public TrackCoordinate(Track track, Geocoordinate coordinate)
            {
                Track = track;
                Coordinate = coordinate;
            }

            private Track Track { get; set; }

            public Geocoordinate Coordinate { get; private set; }

            private static bool CompareTrackCoordinates(TrackCoordinate left, TrackCoordinate right)
            {
                var isTheSame = left.Track.Id == right.Track.Id
                    && Math.Abs(left.Coordinate.Latitude - right.Coordinate.Latitude) < _tolerance
                    && Math.Abs(left.Coordinate.Longitude - right.Coordinate.Longitude) < _tolerance;

#if DEBUG
                if (!isTheSame) {
                    //Debug.WriteLine("--");
                    //Debug.WriteLine("TrackIds:  {0} {1}", left.Track.Id, right.Track.Id);
                    //Debug.WriteLine("Latitude:  {0} {1} Δ: {2}", left.Coordinate.Latitude, right.Coordinate.Latitude, Math.Abs(left.Coordinate.Latitude - right.Coordinate.Latitude));
                    //Debug.WriteLine("Longitude: {0} {1} Δ: {2}", left.Coordinate.Longitude, right.Coordinate.Longitude, Math.Abs(left.Coordinate.Longitude - right.Coordinate.Longitude));
                    //Debug.WriteLine("--");
                }
#endif

                return isTheSame;
            }

            private static readonly IEqualityComparer<TrackCoordinate> _equalityComparer = new LambdaComparer<TrackCoordinate>(CompareTrackCoordinates);

            public static IEqualityComparer<TrackCoordinate> EqualityComparer
            {
                get { return _equalityComparer; }
            }
        }

        public IReactiveCommand StartTracking { get; private set; }

        public IReactiveCommand StopTracking { get; private set; }

        public IReactiveCommand CapturePhoto { get; private set; }

        public IReactiveCommand ExportTrack { get; private set; }

        //public IReactiveCommand UploadTrack { get; private set; }

        //public IReactiveCommand RestartTracking { get; private set; }

        private bool _isTracking;
        public bool IsTracking
        {
            get { return _isTracking; }
            private set { this.RaiseAndSetIfChanged(ref _isTracking, value); }
        }

        private readonly ObservableAsPropertyHelper<string> _status;
        public string Status
        {
            get { return _status.Value; }
        }

        private readonly ObservableAsPropertyHelper<string> _currentLocation;
        public string CurrentLocation
        {
            get { return _currentLocation.Value; }
        }

        private readonly ObservableAsPropertyHelper<string> _elevation;
        public string Elevation
        {
            get { return _elevation.Value; }
        }

        private readonly ObservableAsPropertyHelper<string> _timeSinceLast;
        public string TimeSinceLast
        {
            get { return _timeSinceLast.Value; }
        }

        public string UrlPathSegment
        {
            get { return "TrackTracking"; }
        }

        public IScreen HostScreen { get; private set; }
    }
}
