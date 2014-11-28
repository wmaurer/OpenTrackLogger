namespace TestClient
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class OpenTrackLoggerDbContext : DbContext
    {
        public OpenTrackLoggerDbContext(string connectionString)
            : base(connectionString)
        {
        }

        //public DbSet<Track> Tracks { get; set; }

        //public DbSet<Waypoint> Waypoints { get; set; }

        //public DbSet<Trackpoint> Trackpoints { get; set; } 
    }
}
