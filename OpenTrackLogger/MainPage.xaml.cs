namespace OpenTrackLogger
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;

    using Microsoft.Phone.Shell;

    using OpenTrackLogger.ViewModels;

    using ReactiveUI;

    using CompositeDisposable = Microsoft.Phone.Reactive.CompositeDisposable;

    public partial class MainPage : IViewFor<AppBootstrapper>
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var menuItems = new Dictionary<AbMenuItemId, ApplicationBarMenuItem>
            {
                { AbMenuItemId.New, new ApplicationBarMenuItem("New") },
                { AbMenuItemId.Delete, new ApplicationBarMenuItem("Delete") },
                { AbMenuItemId.Upload, new ApplicationBarMenuItem("Upload") }
            };

            var menuItemsDisposable = Disposable.Empty;

            this.WhenAnyValue(x => x.Router.Router.CurrentViewModel)
                .SelectMany(x => x)
                .Where(x => x is IHasApplicationBar)
                .Cast<IHasApplicationBar>()
                .Subscribe(x => {
                    menuItemsDisposable.Dispose();
                    ApplicationBar.MenuItems.Clear();
                    var disposables = new List<IDisposable>();
                    if (x.MenuItems != null) {
                        foreach (var abMenuItem in x.MenuItems) {
                            ApplicationBar.MenuItems.Add(menuItems[abMenuItem.Id]);
                            disposables.Add(menuItems[abMenuItem.Id].Events().Click.Subscribe(y => abMenuItem.Command.Execute(y)));
                        }
                    }
                    menuItemsDisposable = new CompositeDisposable(disposables.ToArray());
                });

            this.OneWayBind(ViewModel, x => x.Router, x => x.Router.Router);
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