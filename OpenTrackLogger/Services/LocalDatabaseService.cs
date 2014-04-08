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

        public void InsertPhoto(Photo photo)
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                db.Photos.InsertOnSubmit(photo);
                db.SubmitChanges();
            }
        }

        public IQueryable<Photo> GetPhotos(int trackId)
        {
            using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                return db.Photos.Where(x => x.TrackId == trackId).OrderBy(x => x.CreatedAt);
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
