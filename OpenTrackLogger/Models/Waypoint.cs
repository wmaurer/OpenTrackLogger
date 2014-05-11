namespace OpenTrackLogger.Models
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    using ReactiveUI;

    [Table]
    public class Waypoint : ReactiveObject
    {
        private long _id;
        [Column(IsPrimaryKey = true, Name = "WaypointId", IsDbGenerated = true, DbType = "bigint not null Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert, Storage = "_id")]
        public long Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        private int _trackId;
        [Column(DbType = "int", CanBeNull = false, Storage = "_trackId")]
        public int TrackId
        {
            get { return _trackId; }
            set { this.RaiseAndSetIfChanged(ref _trackId, value); }
        }

        private long _trackpointId;
        [Column(DbType = "bigint", CanBeNull = false, Storage = "_trackpointId")]
        public long TrackpointId
        {
            get { return _trackpointId; }
            set { this.RaiseAndSetIfChanged(ref _trackpointId, value); }
        }

        private int _waypointType;
        [Column(DbType = "smallint", CanBeNull = false, Storage = "_waypointType")]
        public int WaypointType
        {
            get { return _waypointType; }
            set { this.RaiseAndSetIfChanged(ref _waypointType, value); }
        }

        private string _name;
        [Column(DbType = "nvarchar(256)", CanBeNull = false)]
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _link;
        [Column(DbType = "nvarchar(256)", CanBeNull = false)]
        public string Link
        {
            get { return _link; }
            set { this.RaiseAndSetIfChanged(ref _link, value); }
        }

        private DateTime _createdAt;
        [Column(DbType = "datetime", CanBeNull = false)]
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { this.RaiseAndSetIfChanged(ref _createdAt, value); }
        }

        private EntityRef<Track> _track;
        [Association(Name = "FK_Waypoint_Track", Storage = "_track", ThisKey = "TrackId", OtherKey = "Id", IsForeignKey = true)]
        public Track Track
        {
            get { return _track.Entity; }
            set
            {
                this.RaisePropertyChanging();
                _track.Entity = value;
                _trackId = value.Id;
                this.RaisePropertyChanged();
            }
        }

        private EntityRef<Trackpoint> _trackpoint;
        [Association(Name = "FK_Waypoint_Trackpoint", Storage = "_trackpoint", ThisKey = "TrackpointId", OtherKey = "Id", IsForeignKey = true)]
        public Trackpoint Trackpoint
        {
            get { return _trackpoint.Entity; }
            set
            {
                if (_trackpoint.Entity != null)
                    throw new ApplicationException("Reassigning waypoint to another trackpoint is not supported.");
                if (value == null)
                    throw new ApplicationException("Unexpected assignation of null Trackpoint entity to Waypoint");

                this.RaisePropertyChanging();
                _trackpoint.Entity = value;
                _trackpointId = value.Id;
                this.RaisePropertyChanged();
            }
        }
    }
}
