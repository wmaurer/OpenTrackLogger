namespace OpenTrackLogger.Mixins
{
    using System;
    using System.Reactive.Linq;

    using Windows.Devices.Geolocation;
    using Windows.Foundation;

    public static class GeolocatorMixin
    {
        public static GeolocatorEvents Events(this Geolocator geolocator)
        {
            return new GeolocatorEvents(geolocator);
        }
    }

    public class GeolocatorEvents
    {
        private readonly Geolocator _geolocator;

        public GeolocatorEvents(Geolocator geolocator)
        {
            _geolocator = geolocator;
        }

        public IObservable<StatusChangedEventArgs> StatusChanged
        {
            get { return Observable.FromEventPattern<TypedEventHandler<Geolocator, StatusChangedEventArgs>, StatusChangedEventArgs>(h => _geolocator.StatusChanged += h, h => _geolocator.StatusChanged -= h).Select(x => x.EventArgs); }
        }

        public IObservable<PositionChangedEventArgs> PositionChanged
        {
            get { return Observable.FromEventPattern<TypedEventHandler<Geolocator, PositionChangedEventArgs>, PositionChangedEventArgs>(h => _geolocator.PositionChanged += h, h => _geolocator.PositionChanged -= h).Select(x => x.EventArgs); }
        }
    }
}
