namespace OpenTrackLogger.Services
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;

    using Windows.Devices.Geolocation;

    using OpenTrackLogger.Mixins;

    public class GeolocatorService
    {
        public IDisposable Start()
        {
            var geolocator = new Geolocator { DesiredAccuracy = PositionAccuracy.High, ReportInterval = 1000 };

            return new CompositeDisposable {
                geolocator.Events().StatusChanged.Subscribe(_statusChanged),
                geolocator.Events().PositionChanged.Subscribe(_positionChanged),
                Disposable.Create(() => geolocator = null)
            };
        }

        private readonly Subject<StatusChangedEventArgs> _statusChanged = new Subject<StatusChangedEventArgs>();
        public IObservable<StatusChangedEventArgs> StatusChanged { get { return _statusChanged; } }

        private readonly Subject<PositionChangedEventArgs> _positionChanged = new Subject<PositionChangedEventArgs>();
        public IObservable<PositionChangedEventArgs> PositionChanged { get { return _positionChanged; } }
    }
}
