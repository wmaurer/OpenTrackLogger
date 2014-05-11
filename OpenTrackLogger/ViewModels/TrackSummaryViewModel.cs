namespace OpenTrackLogger.ViewModels
{
    using OpenTrackLogger.Models;

    using ReactiveUI;

    public class TrackSummaryViewModel : ReactiveObject
    {
        public TrackSummaryViewModel(TrackSummary trackSummary)
        {
            Model = trackSummary;
            CreatedAtString = trackSummary.Track.CreatedAt.ToString("R");
            CreatedAtId = trackSummary.Track.CreatedAt.ToString("yyyyMMddHHmmss");
        }

        public TrackSummary Model { get; private set; }

        public string CreatedAtString { get; private set; }

        public string CreatedAtId { get; private set; }
    }
}
