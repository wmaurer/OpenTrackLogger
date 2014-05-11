namespace OpenTrackLogger.Views
{
    using System.Windows;

    using OpenTrackLogger.ViewModels;

    using ReactiveUI;

    public partial class TrackListView : IViewFor<TrackListViewModel>
    {
        public TrackListView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.TrackSummaries, x => x.TrackSummaries.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedTrackSummary, x => x.TrackSummaries.SelectedItem);
        }

        #region ViewModel

        public TrackListViewModel ViewModel
        {
            get { return (TrackListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TrackListViewModel), typeof(TrackListView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TrackListViewModel)value; }
        }

        #endregion
    }
}
