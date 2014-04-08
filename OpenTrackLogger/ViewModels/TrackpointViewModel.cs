using OpenTrackLogger.Models;

namespace OpenTrackLogger.ViewModels
{
    public class TrackpointViewModel
    {
        private readonly Trackpoint _trackpoint;

        public TrackpointViewModel(Trackpoint trackpoint)
        {
            _trackpoint = trackpoint;
            Timestamp = trackpoint.Timestamp.ToString();
            Coordinates = string.Format("{0},{1}", trackpoint.Latitude, trackpoint.Longitude);
        }

        public string Timestamp { get; private set; }

        public string Coordinates { get; private set; }

        public double Latitide
        {
            get { return _trackpoint.Latitude; }
        }

        public double Longitude
        {
            get { return _trackpoint.Longitude; }
        }
    }
}
