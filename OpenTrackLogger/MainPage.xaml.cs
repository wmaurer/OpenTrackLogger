namespace OpenTrackLogger
{
    using System.Windows;

    using OpenTrackLogger.ViewModels;

    using ReactiveUI;

    public partial class MainPage : IViewFor<AppBootstrapper>
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.Router, x => x.Router.Router);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
        //    {
        //        return;
        //    }

        //    var result = MessageBox.Show("This app accesses your phone's location. Is that ok?", "Location", MessageBoxButton.OKCancel);
        //    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = result == MessageBoxResult.OK;
        //    IsolatedStorageSettings.ApplicationSettings.Save();
        //}

        //private async void OneShotLocation_Click(object sender, RoutedEventArgs e)
        //{

        //    if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
        //    {
        //        // The user has opted out of Location.
        //        return;
        //    }

        //    Geolocator geolocator = new Geolocator();
        //    geolocator.DesiredAccuracyInMeters = 50;

        //    try
        //    {
        //        Geoposition geoposition = await geolocator.GetGeopositionAsync(
        //            maximumAge: TimeSpan.FromMinutes(5),
        //            timeout: TimeSpan.FromSeconds(10)
        //            );

        //        LatitudeTextBlock.Text = geoposition.Coordinate.Latitude.ToString("0.00");
        //        LongitudeTextBlock.Text = geoposition.Coordinate.Longitude.ToString("0.00");
        //    }
        //    catch (Exception ex)
        //    {
        //        if ((uint)ex.HResult == 0x80004004)
        //        {
        //            // the application does not have the right capability or the location master switch is off
        //            StatusTextBlock.Text = "location  is disabled in phone settings.";
        //        }
        //        //else
        //        {
        //            // something else happened acquring the location
        //        }
        //    }
        //}

        #region ViewModel

        public AppBootstrapper ViewModel
        {
            get { return (AppBootstrapper)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AppBootstrapper), typeof(MainPage), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (AppBootstrapper)value; }
        }

        #endregion
    }
}