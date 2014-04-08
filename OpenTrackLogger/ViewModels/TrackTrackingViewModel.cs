namespace OpenTrackLogger.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Windows.Devices.Geolocation;

    using OpenTrackLogger.Common;
    using OpenTrackLogger.Models;
    using OpenTrackLogger.Services;

    using ReactiveUI;

    public class TrackTrackingViewModel : ReactiveObject, IRoutableViewModel
    {
        public TrackTrackingViewModel(ICameraService cameraService, ILocalDriveService localDriveService, IScreen screen = null)
        {
            HostScreen = screen;

            var geolocatorService = RxApp.DependencyResolver.GetService<GeolocatorService>();
            var localDatabaseService = new LocalDatabaseService();
            Track currentTrack = null;
            Trackpoint currentTrackpoint = null;
            var currenttTrackTimeId = string.Empty;

            var isTracking = this.WhenAnyValue(x => x.IsTracking).StartWith(false);
            var isNotTracking = isTracking.Select(x => !x);
            StartTracking = new ReactiveCommand(isNotTracking);
            StopTracking = new ReactiveCommand(isTracking);

            geolocatorService.StatusChanged
                .Subscribe(x => { var status = x.Status; }, () => { var xxx = 1; });

            var trackpoints = geolocatorService.PositionChanged
                .Select(x => new TrackCoordinate(currentTrack, x.Position.Coordinate))
                .DistinctUntilChanged(TrackCoordinate.EqualityComparer)
                .Select(x => x.Coordinate)
                .Select(x => new Trackpoint(currentTrack.Id, x));

            trackpoints
                .Do(x => Debug.WriteLine("{0}: {1}, {2}", DateTime.Now.TimeOfDay, x.Latitude, x.Longitude))
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
                .Subscribe(x => {
                    IsTracking = x;
                    if (IsTracking) {
                        if (currentTrack == null) {
                            currenttTrackTimeId = DateTime.Now.ToString("yyyyMMddHHmmss");
                            currentTrack = localDatabaseService.CreateTrack();
                        }
                        disp = geolocatorService.Start();
                    }
                    else {
                        disp.Dispose();
                    }   
                });

            CapturePhoto = new ReactiveCommand(isTracking);
            CapturePhoto.Subscribe(x => cameraService.CapturePhoto());

            cameraService.PhotoCaptureCompleted.Subscribe(async x => {
                var filename = Path.GetFileName(x.OriginalFileName);
                if (string.IsNullOrEmpty(filename)) return;
                await localDriveService.SavePhoto(currenttTrackTimeId, filename, x.ChosenPhoto);
                localDatabaseService.InsertPhoto(new Photo { TrackId = currentTrack.Id, TrackpointId = currentTrackpoint.Id, CreatedAt = DateTime.Now, Filename = filename });
            });

            ExportTrack = new ReactiveCommand(isNotTracking);
            ExportTrack.Subscribe(_ => {
                var track = localDatabaseService.GetTrack(currentTrack.Id);
                localDriveService.ExportTrack(currenttTrackTimeId, s => currentTrack.WriteToStream(s));
            });

            var sds = new OneDriveService();
            UploadTrack = new ReactiveCommand(isNotTracking);
            UploadTrack.Subscribe(async _ => await sds.UploadToSkyDrive(await localDriveService.GetTrackFolder(currenttTrackTimeId)));

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
                    Debug.WriteLine("--");
                    Debug.WriteLine("TrackIds:  {0} {1}", left.Track.Id, right.Track.Id);
                    Debug.WriteLine("Latitude:  {0} {1} Δ: {2}", left.Coordinate.Latitude, right.Coordinate.Latitude, Math.Abs(left.Coordinate.Latitude - right.Coordinate.Latitude));
                    Debug.WriteLine("Longitude: {0} {1} Δ: {2}", left.Coordinate.Longitude, right.Coordinate.Longitude, Math.Abs(left.Coordinate.Longitude - right.Coordinate.Longitude));
                    Debug.WriteLine("--");
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

        public IReactiveCommand UploadTrack { get; private set; }

        private bool _isTracking;
        public bool IsTracking
        {
            get { return _isTracking; }
            private set { this.RaiseAndSetIfChanged(ref _isTracking, value); }
        }

        private readonly ObservableAsPropertyHelper<string> _currentLocation;
        public string CurrentLocation
        {
            get { return _currentLocation.Value; }
        }

        public string UrlPathSegment
        {
            get { return "HomePage"; }
        }

        public IScreen HostScreen { get; private set; }
    }
}
