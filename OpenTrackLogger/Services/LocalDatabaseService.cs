namespace OpenTrackLogger.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OpenTrackLogger.Models;

    public class LocalDatabaseService
    {
        public Track CreateTrack()
        {
            var track = new Track { CreatedAt = DateTime.Now };
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString))
            {
                db.Tracks.InsertOnSubmit(track);
                db.SubmitChanges();
            }
            return track;
        }

        public void InsertTrackpoint(Trackpoint trackpoint)
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString))
            {
                db.Trackpoints.InsertOnSubmit(trackpoint);
                db.SubmitChanges();
            }
        }

        public void InsertPhoto(Waypoint waypoint)
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                db.Waypoints.InsertOnSubmit(waypoint);
                db.SubmitChanges();
            }
        }

        private class WaypointTypeCount
        {
            public WaypointTypeCount(WaypointType type, int count)
            {
                Type = type;
                Count = count;
            }

            public WaypointType Type { get; private set; }
            public int Count { get; private set; }
        }

        private static int GetWaypointTypeCount(IEnumerable<WaypointTypeCount> list, WaypointType type)
        {
            var item = list.FirstOrDefault(x => x.Type == type);
            return item == null ? 0 : item.Count;
        }

        public List<TrackSummary> GetAllTrackSummaries()
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                return db.Tracks
                    .Select(x => new { Track = x, WaypointTypes = x.Waypoints.GroupBy(y => y.WaypointType).Select(y => new WaypointTypeCount((WaypointType)y.Key, y.Count())).ToList() })
                    .Select(x => new TrackSummary
                    {
                        Track = x.Track,
                        NumberOfTrackpoints = x.Track.Trackpoints.Count,
                        NumberOfWaypoints = x.Track.Waypoints.Count,
                        NumberOfPhotoWaypoints = GetWaypointTypeCount(x.WaypointTypes, WaypointType.Photo),
                        NumberOfVideoWaypoints = GetWaypointTypeCount(x.WaypointTypes, WaypointType.Video),
                        NumberOfAudioWaypoints = GetWaypointTypeCount(x.WaypointTypes, WaypointType.Audio),
                        NumberOfTextWaypoints = GetWaypointTypeCount(x.WaypointTypes, WaypointType.Text),
                        NumberOfOsmTagWaypoints = GetWaypointTypeCount(x.WaypointTypes, WaypointType.OsmTag)
                    }).ToList();
            }
        }

        public IQueryable<Waypoint> GetPhotos(int trackId)
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                return db.Waypoints.Where(x => x.TrackId == trackId).OrderBy(x => x.CreatedAt);
            }
        }

        public Track GetTrack(int trackId)
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                return db.Tracks.First(x => x.Id == trackId);
            }
        }
    }
}
