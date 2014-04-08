namespace OpenTrackLogger.Models
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    using ReactiveUI;

    [Table]
    public class Photo : ReactiveObject
    {
        private long _id;
        [Column(IsPrimaryKey = true, Name = "PhotoId", IsDbGenerated = true, DbType = "bigint not null Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert, Storage = "_id")]
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

        private string _filename;
        [Column(DbType = "nvarchar(256)", CanBeNull = false)]
        public string Filename
        {
            get { return _filename; }
            set { this.RaiseAndSetIfChanged(ref _filename, value); }
        }

        private DateTime _createdAt;
        [Column(DbType = "datetime", CanBeNull = false)]
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { this.RaiseAndSetIfChanged(ref _createdAt, value); }
        }

        private EntityRef<Track> _track;
        [Association(Name = "FK_Photo_Track", Storage = "_track", ThisKey = "TrackId", OtherKey = "Id", IsForeignKey = true)]
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
        [Association(Name = "FK_Photo_Trackpoint", Storage = "_trackpoint", ThisKey = "TrackpointId", OtherKey = "Id", IsForeignKey = true)]
        public Trackpoint Trackpoint
        {
            get { return _trackpoint.Entity; }
            set
            {
                if (_trackpoint.Entity != null)
                    throw new ApplicationException("Reassigning photo to another trackpoint is not supported.");
                if (value == null)
                    throw new ApplicationException("Unexpected assignation of null Trackpoint entity to Photo");

                this.RaisePropertyChanging();
                _trackpoint.Entity = value;
                _trackpointId = value.Id;
                this.RaisePropertyChanged();
            }
        }
    }
}
