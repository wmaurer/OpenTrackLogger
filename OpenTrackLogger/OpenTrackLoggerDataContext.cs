namespace OpenTrackLogger
{
    using System.Data.Linq;

    using OpenTrackLogger.Models;

    public class OpenTrackLoggerDataContext : DataContext
    {
        public static string ConnectionString = "Data Source=isostore:/OpenTrackLogger.sdf";

        public OpenTrackLoggerDataContext(string connectionString)
            : base(connectionString)
        {
            var loadOptions = new DataLoadOptions();
            loadOptions.LoadWith<Photo>(x => x.Trackpoint);
            LoadOptions = loadOptions;
        }

        public Table<Track> Tracks
        {
            get { return GetTable<Track>(); }
        }

        public Table<Trackpoint> Trackpoints
        {
            get { return GetTable<Trackpoint>(); }
        }

        public Table<Photo> Photos
        {
            get { return GetTable<Photo>(); }
        }
    }
}
