namespace OpenTrackLogger.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using Windows.Devices.Geolocation;

    using Microsoft.Phone.Data.Linq.Mapping;

    using ReactiveUI;

    [Table]
    [Index(Columns = "TrackId, Timestamp asc", IsUnique = true, Name = "IX_Trackpoint__TrackId_Timestamp")]
    public class Trackpoint : ReactiveObject
    {
        public Trackpoint() {}

        public Trackpoint(int trackId, Geocoordinate geoCoordinate)
        {
            TrackId = trackId;
            Timestamp = geoCoordinate.Timestamp.UtcDateTime;
            Latitude = geoCoordinate.Latitude;
            Longitude = geoCoordinate.Longitude;
            Elevation = geoCoordinate.Altitude;
            Speed = geoCoordinate.Speed;
            HorizontalDilutionOfPrecision = geoCoordinate.SatelliteData.HorizontalDilutionOfPrecision;
            PositionDilutionOfPrecision = geoCoordinate.SatelliteData.PositionDilutionOfPrecision;
            VerticalDilutionOfPrecision = geoCoordinate.SatelliteData.VerticalDilutionOfPrecision;
        }

        private long _id;
        [Column(IsPrimaryKey = true, Name = "TrackpointId", IsDbGenerated = true, DbType = "bigint not null Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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

        private DateTime _timestamp;
        [Column(DbType = "datetime", CanBeNull = false)]
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { this.RaiseAndSetIfChanged(ref _timestamp, value); }
        }

        private double _latitude;
        [Column(DbType = "numeric(16, 13)", CanBeNull = false)]
        public double Latitude
        {
            get { return _latitude; }
            set { this.RaiseAndSetIfChanged(ref _latitude, value); }
        }

        private double _longitude;
        [Column(DbType = "numeric(16, 13)", CanBeNull = false)]
        public double Longitude
        {
            get { return _longitude; }
            set { this.RaiseAndSetIfChanged(ref _longitude, value); }
        }

        private double? _elevation;
        [Column(CanBeNull = true)]
        public double? Elevation
        {
            get { return _elevation; }
            set { this.RaiseAndSetIfChanged(ref _elevation, value); }
        }

        private double? _speed;
        [Column(CanBeNull = true)]
        public double? Speed
        {
            get { return _speed; }
            set { this.RaiseAndSetIfChanged(ref _speed, value); }
        }

        private double? _horizontalDilutionOfPrecision;
        [Column(DbType = "float", CanBeNull = true)]
        public double? HorizontalDilutionOfPrecision
        {
            get { return _horizontalDilutionOfPrecision; }
            set { this.RaiseAndSetIfChanged(ref _horizontalDilutionOfPrecision, value); }
        }

        private double? _positionDilutionOfPrecision;
        [Column(DbType = "float", CanBeNull = true)]
        public double? PositionDilutionOfPrecision
        {
            get { return _positionDilutionOfPrecision; }
            set { this.RaiseAndSetIfChanged(ref _positionDilutionOfPrecision, value); }
        }

        private double? _verticalDilutionOfPrecision;
        [Column(DbType = "float", CanBeNull = true)]
        public double? VerticalDilutionOfPrecision
        {
            get { return _verticalDilutionOfPrecision; }
            set { this.RaiseAndSetIfChanged(ref _verticalDilutionOfPrecision, value); }
        }
    }
}
