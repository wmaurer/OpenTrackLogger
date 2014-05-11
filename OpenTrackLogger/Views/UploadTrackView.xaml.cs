namespace OpenTrackLogger.Views
{
    using System.Windows;

    using OpenTrackLogger.ViewModels;

    using ReactiveUI;

    public partial class UploadTrackView : IViewFor<UploadTrackViewModel>
    {
        public UploadTrackView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.ExportProgress, x => x.ExportProgressBar.Value);
            this.OneWayBind(ViewModel, x => x.ExportProgressText, x => x.ExportProgressText.Text);
            this.OneWayBind(ViewModel, x => x.ZipProgress, x => x.ZipProgress.Text);
            this.OneWayBind(ViewModel, x => x.UploadPercentage, x => x.UploadPercentage.Text);
        }

        #region ViewModel

        public UploadTrackViewModel ViewModel
        {
            get { return (UploadTrackViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(UploadTrackViewModel), typeof(UploadTrackView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (UploadTrackViewModel)value; }
        }

        #endregion
    }
}
