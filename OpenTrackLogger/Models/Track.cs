namespace OpenTrackLogger.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using ReactiveUI;

    [Table]
    public class Track : ReactiveObject
    {
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
    }
}
