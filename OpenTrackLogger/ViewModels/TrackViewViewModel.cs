namespace OpenTrackLogger.ViewModels
{
    using System;
    using System.Collections.Generic;

    using OpenTrackLogger.Mixins;

    using ReactiveUI;

    public class TrackViewViewModel : ReactiveObject, IRoutableViewModel, IHasApplicationBar
    {
        public TrackViewViewModel(IScreen screen = null)
        {
            HostScreen = screen;

            var deleteTrack = new ReactiveCommand();
            deleteTrack.Subscribe(x => {
                var xx = x;
            });

            var uploadTrack = new ReactiveCommand();
            uploadTrack.Subscribe(x => new UploadTrackViewModel(screen).NavigateToWithParameters(TrackSummary));

            MenuItems = new List<AbMenuItem> {
                new AbMenuItem(AbMenuItemId.Delete, deleteTrack),
                new AbMenuItem(AbMenuItemId.Upload, uploadTrack)
            };
        }

        public void NavigateToWithParameters(TrackSummaryViewModel trackSummary)
        {
            TrackSummary = trackSummary;
            HostScreen.Router.Navigate.Execute(this); ;
        }

        private TrackSummaryViewModel _trackSummary;
        public TrackSummaryViewModel TrackSummary
        {
            get { return _trackSummary; }
            private set { this.RaiseAndSetIfChanged(ref _trackSummary, value); }
        }

        public string UrlPathSegment
        {
            get { return "TrackView"; }
        }

        public IScreen HostScreen { get; private set; }

        public List<AbMenuItem> MenuItems { get; private set; }
    }
}
