namespace TestClient
{
    using System;

    public class Trackpoint
    {
        public long TrackpointId { get; set; }

        public int TrackId { get; set; }

        public DateTime Timestamp { get; set; }

        public Decimal Latitude { get; set; }

        public Decimal Longitude { get; set; }
    }
}
