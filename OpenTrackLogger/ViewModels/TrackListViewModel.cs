namespace OpenTrackLogger.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Phone.Reactive;

    using OpenTrackLogger.Mixins;
    using OpenTrackLogger.Services;

    using ReactiveUI;

    public enum AbMenuItemId
    {
        New,
        Delete,
        Upload
    }

    public class AbMenuItem
    {
        public AbMenuItem(AbMenuItemId id, IReactiveCommand command)
        {
            Id = id;
            Command = command;
        }

        public AbMenuItemId Id { get; private set; }

        public IReactiveCommand Command { get; private set; }
    }

    public interface IHasApplicationBar
    {
        List<AbMenuItem> MenuItems { get;  } 
    }

    public class TrackListViewModel : ReactiveObject, IRoutableViewModel, IHasApplicationBar
    {
        public TrackListViewModel(IScreen screen = null)
        {
            HostScreen = screen;

            this.WhenNavigatedTo(() => {
                var localDatabaseService = new LocalDatabaseService();
                _trackSummaries = Observable.Start(() => localDatabaseService.GetAllTrackSummaries().Select(x => new TrackSummaryViewModel(x)).ToList())
                    .ToProperty(this, x => x.TrackSummaries);
                return Disposable.Empty;
            });

            this.WhenAnyNotNullValue(x => x.SelectedTrackSummary)
                .Subscribe(x => new TrackViewViewModel(screen).NavigateToWithParameters(x));

            NewTrack = new ReactiveCommand();
            NewTrack.Subscribe(x => HostScreen.Router.Navigate.Execute(new TrackTrackingViewModel(screen)));

            MenuItems = new List<AbMenuItem> {
                new AbMenuItem(AbMenuItemId.New, NewTrack)
            };
        }

        public IReactiveCommand NewTrack { get; private set; }

        private TrackSummaryViewModel _selectedTrackSummary;
        public TrackSummaryViewModel SelectedTrackSummary
        {
            get { return _selectedTrackSummary; }
            set { this.RaiseAndSetIfChanged(ref _selectedTrackSummary, value); }
        }

        private ObservableAsPropertyHelper<List<TrackSummaryViewModel>> _trackSummaries;
        public List<TrackSummaryViewModel> TrackSummaries { get { return _trackSummaries.Value; } }


        public List<AbMenuItem> MenuItems { get; private set; }


        public string UrlPathSegment { get { return "TrackList"; } }

        public IScreen HostScreen { get; private set; }

    }
}
