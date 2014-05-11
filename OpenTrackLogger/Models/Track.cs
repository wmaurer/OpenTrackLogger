namespace OpenTrackLogger.Models
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    using ReactiveUI;

    [Table]
    public class Track : ReactiveObject
    {
        public Track()
        {
            _trackpoints = new EntitySet<Trackpoint>();
            _waypoints = new EntitySet<Waypoint>();
        }

        private int _id;
        [Column(IsPrimaryKey = true, Name = "TrackId", IsDbGenerated = true, DbType = "int not null Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        private DateTime _createdAt;
        [Column(DbType = "datetime", CanBeNull = false)]
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { this.RaiseAndSetIfChanged(ref _createdAt, value); }
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private EntitySet<Trackpoint> _trackpoints;
        [Association(Name = "FK_Trackpoint_Track", Storage = "_trackpoints", ThisKey = "Id", OtherKey = "TrackId")]
        public EntitySet<Trackpoint> Trackpoints
        {
            get { return _trackpoints; }
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private EntitySet<Waypoint> _waypoints;
        [Association(Name = "FK_Waypoint_Track", Storage = "_waypoints", ThisKey = "Id", OtherKey = "TrackId")]
        public EntitySet<Waypoint> Waypoints
        {
            get { return _waypoints; }
        }
    }
}
