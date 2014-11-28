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
            this.OneWayBind(ViewModel, x => x.ZipProgress, x => x.ZipProgressBar.Value);
            this.OneWayBind(ViewModel, x => x.ZipProgressText, x => x.ZipProgressText.Text);
            this.OneWayBind(ViewModel, x => x.UploadProgress, x => x.UploadProgressBar.Value);
            this.OneWayBind(ViewModel, x => x.UploadProgressText, x => x.UploadProgressText.Text);
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
