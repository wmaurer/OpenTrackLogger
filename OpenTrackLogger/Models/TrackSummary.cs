namespace OpenTrackLogger.Models
{
    public class TrackSummary
    {
        public Track Track { get; set; }

        public int NumberOfTrackpoints { get; set; }

        public int NumberOfWaypoints { get; set; }

        public int NumberOfPhotoWaypoints { get; set; }

        public int NumberOfVideoWaypoints { get; set; }

        public int NumberOfAudioWaypoints { get; set; }

        public int NumberOfOsmTagWaypoints { get; set; }

        public int NumberOfTextWaypoints { get; set; }
    }
}
