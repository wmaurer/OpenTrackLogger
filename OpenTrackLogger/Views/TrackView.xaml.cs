namespace OpenTrackLogger.Views
{
    using System.Windows;

    using OpenTrackLogger.ViewModels;

    using ReactiveUI;

    public partial class TrackView : IViewFor<TrackViewViewModel>
    {
        public TrackView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.TrackSummary.CreatedAtString, x => x.CreatedAt.Text);
            this.OneWayBind(ViewModel, x => x.TrackSummary.Model.NumberOfTrackpoints, x => x.NumberOfTrackpoints.Text);
            this.OneWayBind(ViewModel, x => x.TrackSummary.Model.NumberOfPhotoWaypoints, x => x.NumberOfPhotoWaypoints.Text);
            this.OneWayBind(ViewModel, x => x.TrackSummary.Model.NumberOfVideoWaypoints, x => x.NumberOfVideoWaypoints.Text);
            this.OneWayBind(ViewModel, x => x.TrackSummary.Model.NumberOfAudioWaypoints, x => x.NumberOfAudioWaypoints.Text);
            this.OneWayBind(ViewModel, x => x.TrackSummary.Model.NumberOfTextWaypoints, x => x.NumberOfTextWaypoints.Text);
            this.OneWayBind(ViewModel, x => x.TrackSummary.Model.NumberOfOsmTagWaypoints, x => x.NumberOfOsmTagWaypoints.Text);
        }

        #region ViewModel

        public TrackViewViewModel ViewModel
        {
            get { return (TrackViewViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TrackViewViewModel), typeof(TrackView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TrackViewViewModel)value; }
        }

        #endregion
    }
}
