namespace TestClient
{
    using System;

    public class Waypoint
    {
        public long WaypointId { get; set; }

        public int TrackId { get; set; }

        public long TrackpointId { get; set; }

        public Int16 WaypointType { get; set; }

        public string Name  { get; set; }

        public string Link { get; set; }

        public DateTime CreatedAt { get; set; }

        public Trackpoint Trackpoint { get; set; }
    }
}
