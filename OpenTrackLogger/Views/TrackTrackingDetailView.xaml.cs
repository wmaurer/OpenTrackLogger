namespace OpenTrackLogger.Views
{
    using System.Windows;

    using OpenTrackLogger.ViewModels;

    using ReactiveUI;

    public partial class TrackTrackingDetailView : IViewFor<TrackTrackingViewModel>
    {
        public TrackTrackingDetailView()
        {
            InitializeComponent();

            this.BindCommand(ViewModel, x => x.StartTracking);
            this.BindCommand(ViewModel, x => x.StopTracking);
            this.BindCommand(ViewModel, x => x.CapturePhoto);
            this.BindCommand(ViewModel, x => x.ExportTrack);
            //this.BindCommand(ViewModel, x => x.UploadTrack);

            this.OneWayBind(ViewModel, x => x.Status, x => x.Status.Text);
            this.OneWayBind(ViewModel, x => x.CurrentLocation, x => x.CurrentLocation.Text);
            this.OneWayBind(ViewModel, x => x.Elevation, x => x.Elevation.Text);
            this.OneWayBind(ViewModel, x => x.TimeSinceLast, x => x.TimeSinceLast.Text);

            //this.BindCommand(ViewModel, x => x.RestartTracking);

            //this.OneWayBind(ViewModel, x => x.Trackpoints, x => x.CoordinatesListBox.ItemsSource);
        }

        #region ViewModel

        public TrackTrackingViewModel ViewModel
        {
            get { return (TrackTrackingViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TrackTrackingViewModel), typeof(TrackTrackingDetailView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TrackTrackingViewModel)value; }
        }

        #endregion
    }
}
