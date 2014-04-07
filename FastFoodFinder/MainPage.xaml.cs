using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FastFoodFinder.Resources;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;
using System.IO.IsolatedStorage;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FastFoodFinder
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinate MyLocation = new GeoCoordinate();
        public GeoCoordinate getMyLocation()
        {
            return MyLocation;
        }
        Dictionary<String, HashSet<OSMObject>> Stores = new Dictionary<string, HashSet<OSMObject>>();
        public MainPage()
        {
            InitializeComponent();
            Settings = IsolatedStorageSettings.ApplicationSettings;

        }
        /// <summary>
        /// Fired while loding and redisplayed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Settings["isLocationAllowed"] = false; //DEBUG only
            base.OnNavigatedTo(e);

            if (_isNewInstance)
            {
                _isNewInstance = false;

                if (Settings.Contains("isLocationAllowed"))
                {
                    if ((bool)Settings["isLocationAllowed"]) //normaler user
                    {
                        QuestionLocationPanel.Visibility = Visibility.Collapsed;
                        BuildApplicationBar();
                        LoadSettings();
                        TrackLocation();
                    }
                }
            }
            else
            {
                LoadSettings();

                if (Settings.Contains("isLocationAllowed") && Settings.Contains("isTrackingAllowed"))
                {
                    if (!(bool)Settings["isLocationAllowed"] && !(bool)Settings["isTrackingAllowed"])
                    {
                        QuestionLocationPanel.Visibility = Visibility.Visible;
                        ApplicationBar.IsVisible = false;
                    }
                }

                TrackLocation();
            }
        }


        private void LoadSettings()
        {
            if (Settings.Contains("PedestrianFeatures"))
            {
                MyMap.PedestrianFeaturesEnabled = (bool)Settings["PedestrianFeatures"];
            }
            if (Settings.Contains("LandmarksEnabled"))
            {
                MyMap.LandmarksEnabled = (bool)Settings["LandmarksEnabled"];
            }
            if (Settings.Contains("isTrackingAllowed"))
            {
                tracking = (bool)Settings["isTrackingAllowed"];
            }
            if (Settings.Contains("mapMode"))
            {
                MyMap.CartographicMode = (MapCartographicMode)Settings["mapMode"];
            }
            if (Settings.Contains("ColorMode"))
            {
                MyMap.ColorMode = (MapColorMode)Settings["ColorMode"];
            }
            if (Settings.Contains("ZoomControls"))
            {
                if ((bool)Settings["ZoomControls"])
                {
                    PitchSlider.Visibility = Visibility.Visible;
                    HeadingSlider.Visibility = Visibility.Visible;
                    ZoomSlider.Visibility = Visibility.Visible;
                }
                else
                {
                    PitchSlider.Visibility = Visibility.Collapsed;
                    HeadingSlider.Visibility = Visibility.Collapsed;
                    ZoomSlider.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void TrackLocation()
        {
            try
            {

                if (tracking)
                {
                    ShowSearchingFullscreen("getting location...");
                    geolocator = new Geolocator();
                    geolocator.DesiredAccuracy = PositionAccuracy.High;
                    geolocator.MovementThreshold = 100; // The units are meters.

                    //geolocator.StatusChanged += geolocator_StatusChanged;
                    geolocator.PositionChanged += geolocator_TrackedPositionChanged;

                    tracking = true;

                }
                else
                {
                    try
                    {
                        geolocator.PositionChanged -= geolocator_TrackedPositionChanged;
                        //geolocator.StatusChanged -= geolocator_StatusChanged;
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        geolocator = null;
                        tracking = false;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("TrackLocation()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
            finally
            {

            }
        }

        private void geolocator_TrackedPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MyLocation.Latitude = args.Position.Coordinate.Latitude;
                    MyLocation.Longitude = args.Position.Coordinate.Longitude;

                    PositionChanged();
                    HideSearchingFullscreen();
                });
            }
            catch (Exception ex)
            {

                MessageBox.Show("geolocator_TrackedPositionChanged()\n\n" + ex.Message, "ERROR with tracking :/", MessageBoxButton.OK);
            }
        }

        private void PositionChanged()
        {
            //ShowProgressIndicator("refreshing ...");
            try
            {
                searchAndDrawStoresArround();
                CenterMapToLocation();
            }
            catch (Exception ex)
            {
                MessageBox.Show("PositionChanged()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
            finally
            {
                //HideProgressIndicator();
            }
            //DrawAllMarkers();
        }


        private void DrawAllMarkers()
        {
            try
            {
                if (MyMap != null)
                {
                    if (MyMap.Layers.Count > 0)
                    {
                        MyMap.Layers.Clear();
                    }
                    MapLayer mapLayerStores = new MapLayer();

                    DrawMyLocation(mapLayerStores);
                    DrawStoreMarkers(mapLayerStores);

                    MyMap.Layers.Add(mapLayerStores);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("DrawAllMarkers()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }



        /// <summary>
        /// Event handler for location usage permission at startup.
        /// </summary>
        private void LocationUsageQuestionPopup_Click(object sender, EventArgs e)
        {
            try
            {
                QuestionLocationPanel.Visibility = Visibility.Collapsed;
                BuildApplicationBar();
                if (sender == AllowButton)
                {
                    if (Settings.Contains("isLocationAllowed"))
                    {
                        Settings["isLocationAllowed"] = true;
                    }
                    else
                    {
                        Settings.Add("isLocationAllowed", true);
                    }


                    if (Settings.Contains("isTrackingAllowed"))
                    {
                        Settings["isTrackingAllowed"] = true;
                    }
                    else
                    {
                        Settings.Add("isTrackingAllowed", true);
                    }
                    tracking = true;

                    Settings.Save();

                    TrackLocation();
                    ApplicationBar.IsVisible = true;
                }
                else
                {
                    MessageBox.Show("Currently not yet Supported!\n\nFor the time beeing the application NEED to detect your location. Searching for an location is not jet implemented.\n\nApplication will not terminate...", "SORRY", MessageBoxButton.OK);
                    Application.Current.Terminate();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("LocationUsageQuestionPopup_Click()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// We must satisfy Maps API's Terms and Conditions by specifying
        /// the required Application ID and Authentication Token.
        /// See http://msdn.microsoft.com/en-US/library/windowsphone/develop/jj207033(v=vs.105).aspx#BKMK_appidandtoken
        /// </summary>
        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "56558da8-ec80-40a4-a0ee-4e79c52a3b6a";
                Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "vKIAQIIMW8EYwM5hua7NUg";
            }
            catch (Exception ex)
            {

                MessageBox.Show("MyMap_Loaded()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }




        #region "Navigation/Menu"

        private void LocateMe_Click(object sender, EventArgs e)
        {
            int dzl = 10;
            int.TryParse(AppResources.DefaultZoomLevel, out dzl);
            MyMap.SetView(MyLocation, dzl, MapAnimationKind.Parabolic);
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
        private void About_Click(object sender, EventArgs e)
        {
            // Clear map layers to avoid map markers briefly shown on top of about page 
            MyMap.Layers.Clear();
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
        /// <summary>
        /// Event handler for pitch slider value change.
        /// </summary>
        private void PitchValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (PitchSlider != null)
                {
                    MyMap.Pitch = PitchSlider.Value;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("PitchValueChanged()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Event handler for heading slider value change.
        /// </summary>
        private void HeadingValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (HeadingSlider != null)
                {
                    double value = HeadingSlider.Value;
                    if (value > 360) value -= 360;
                    MyMap.Heading = value;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("HeadingValueChanged()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private async void ZoomLevelChanged(object sender, EventArgs e)
        {
            try
            {
                if (ZoomSlider != null)
                {

                    if (sender == ZoomSlider)
                    {
                        MyMap.ZoomLevel = ZoomSlider.Value;
                    }
                    else
                    {
                        ZoomSlider.Value = MyMap.ZoomLevel;
                    }
                }


                DrawAllMarkers();
            }
            catch (Exception ex)
            {

                MessageBox.Show("ZoomLevelChanged()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }
        #endregion





        #region "## STORES ##"


        private async void searchAndDrawStoresArround()
        {
            try
            {
                Store mc = new Store(AppResources.SearchStringMcDonalds, this);

                if (Settings.Contains(AppResources.SearchStringMcDonalds))
                {
                    if ((bool)Settings[AppResources.SearchStringMcDonalds] == true)
                    {
                        mc.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringMcDonalds);
                    }
                }
                else
                {
                    mc.searchForFastFood(MyLocation);
                }

                Store bk = new Store(AppResources.SearchStringBurgerKing, this);

                if (Settings.Contains(AppResources.SearchStringBurgerKing))
                {
                    if ((bool)Settings[AppResources.SearchStringBurgerKing] == true)
                    {
                        bk.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringBurgerKing);
                    }
                }
                else
                {
                    bk.searchForFastFood(MyLocation);
                }

                Store kfc = new Store(AppResources.SearchStringKFC, this);

                if (Settings.Contains(AppResources.SearchStringKFC))
                {
                    if ((bool)Settings[AppResources.SearchStringKFC] == true)
                    {
                        kfc.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringKFC);
                    }
                }
                else
                {
                    kfc.searchForFastFood(MyLocation);
                }

                //--
                Store vapiano = new Store(AppResources.SearchStringVapiano, this);

                if (Settings.Contains(AppResources.SearchStringVapiano))
                {
                    if ((bool)Settings[AppResources.SearchStringVapiano] == true)
                    {
                        vapiano.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringVapiano);
                    }
                }
                else
                {
                    vapiano.searchForFastFood(MyLocation);
                }
                //--
                Store starbucks = new Store(AppResources.SearchStringStarbucks, this);

                if (Settings.Contains(AppResources.SearchStringStarbucks))
                {
                    if ((bool)Settings[AppResources.SearchStringStarbucks] == true)
                    {
                        starbucks.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringStarbucks);
                    }
                }
                else
                {
                    starbucks.searchForFastFood(MyLocation);
                }

                //--
                Store subway = new Store(AppResources.SearchStringSubway, this);

                if (Settings.Contains(AppResources.SearchStringSubway))
                {
                    if ((bool)Settings[AppResources.SearchStringSubway] == true)
                    {
                        subway.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringSubway);
                    }
                }
                else
                {
                    subway.searchForFastFood(MyLocation);
                }
                //--
                Store nordsee = new Store(AppResources.SearchStringNordsee, this);

                if (Settings.Contains(AppResources.SearchStringNordsee))
                {
                    if ((bool)Settings[AppResources.SearchStringNordsee] == true)
                    {
                        nordsee.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringNordsee);
                    }
                }
                else
                {
                    nordsee.searchForFastFood(MyLocation);
                }
                //--
                Store jimblock = new Store(AppResources.SearchStringJimBlock, this);

                if (Settings.Contains(AppResources.SearchStringJimBlock))
                {
                    if ((bool)Settings[AppResources.SearchStringJimBlock] == true)
                    {
                        jimblock.searchForFastFood(MyLocation);
                    }
                    else
                    {
                        Stores.Remove(AppResources.SearchStringJimBlock);
                    }
                }
                else
                {
                    jimblock.searchForFastFood(MyLocation);
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("searchAndDrawStoresArround()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }


        public void dataRecived(string companyName, HashSet<OSMObject> CompanyStores)
        {
            try
            {
                ShowProgressIndicator("finding stores...");
                if (Stores.ContainsKey(companyName))
                {
                    HashSet<OSMObject> currentOfThisCompany;
                    Stores.TryGetValue(companyName, out currentOfThisCompany);
                    currentOfThisCompany.UnionWith(CompanyStores);
                }
                else
                {
                    Stores.Add(companyName, CompanyStores);
                }

                DrawAllMarkers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR reciving data", MessageBoxButton.OK);
            }
            finally
            {
                HideProgressIndicator();
            }
        }


        private void DrawStoreMarkers(MapLayer mapLayerStores)
        {
            try
            {


                foreach (KeyValuePair<string, HashSet<OSMObject>> item in Stores)
                {
                    foreach (OSMObject store in item.Value)
                    {
                        if (store.type.Equals("fast_food") || store.type.Equals("restaurant"))
                        {
                            DrawStoreMarker(store, item.Key, mapLayerStores);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("DrawStoreMarkers()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);

            }
        }
        private void DrawStoreMarker(OSMObject osmObject, String company, MapLayer mapLayer)
        {
            try
            {


                BitmapImage bi = new BitmapImage();

                bi.UriSource = new Uri("/Assets/Pins/default.png", UriKind.Relative);

                if (company.Equals(AppResources.SearchStringBurgerKing))
                {
                    bi.UriSource = new Uri("/Assets/Pins/burgerking.png", UriKind.Relative);
                }
                else if (company.Equals(AppResources.SearchStringKFC))
                {
                    bi.UriSource = new Uri("/Assets/Pins/kfc.png", UriKind.Relative);
                }
                else if (company.Equals(AppResources.SearchStringMcDonalds))
                {
                    bi.UriSource = new Uri("/Assets/Pins/mcdonalds.png", UriKind.Relative);
                }
                else if (company.Equals(AppResources.SearchStringVapiano))
                {
                    bi.UriSource = new Uri("/Assets/Pins/vapiano.png", UriKind.Relative);
                }
                else if (company.Equals(AppResources.SearchStringStarbucks))
                {
                    bi.UriSource = new Uri("/Assets/Pins/starbucks.png", UriKind.Relative);
                }
                else if (company.Equals(AppResources.SearchStringSubway))
                {
                    bi.UriSource = new Uri("/Assets/Pins/subway.png", UriKind.Relative);
                }
                else if (company.Equals(AppResources.SearchStringNordsee))
                {
                    bi.UriSource = new Uri("/Assets/Pins/norsee.png", UriKind.Relative);
                }
                else if (company.Equals(AppResources.SearchStringJimBlock))
                {
                    bi.UriSource = new Uri("/Assets/Pins/jimblock.png", UriKind.Relative);
                }

                var image = new Image();
                image.Width = 28;
                image.Height = 50;
                image.Opacity = 100;
                image.Source = bi;

                image.Tag = osmObject;
                image.MouseLeftButtonUp += new MouseButtonEventHandler(Store_Click);

                // Create a MapOverlay and add marker.
                MapOverlay overlay = new MapOverlay();
                //overlay.Content = polygon;
                overlay.Content = image;
                overlay.GeoCoordinate = new GeoCoordinate(double.Parse(osmObject.lat, System.Globalization.CultureInfo.InvariantCulture), double.Parse(osmObject.lon, System.Globalization.CultureInfo.InvariantCulture));
                overlay.PositionOrigin = new Point(0.0, 1.0);
                mapLayer.Add(overlay);
            }
            catch (Exception ex)
            {

                MessageBox.Show("DrawStoreMarker()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void Store_Click(object sender, EventArgs e)
        {
            try
            {


                Image p = (Image)sender;
                OSMObject store = (OSMObject)p.Tag;
                //MessageBox.Show(store.address.road + " " + store.address.house_number+"\n"+store.address.city,store.address.fast_food,MessageBoxButton.OK);

                GeoCoordinate storeLocation = new GeoCoordinate(double.Parse(store.lat, System.Globalization.CultureInfo.InvariantCulture), double.Parse(store.lon, System.Globalization.CultureInfo.InvariantCulture));

                double distance = Math.Round(MyLocation.GetDistanceTo(storeLocation), 0);

                CustomMessageBox cmb = new CustomMessageBox()
                {
                    Caption = store.address.fast_food,
                    Message = store.address.road + " " + store.address.house_number + "\n" + store.address.city + "\n\nDistance: " + distance + " m",
                    LeftButtonContent = "Navigate to",
                    
                    RightButtonContent = "close",
                };

                cmb.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            List<GeoCoordinate> route = new List<GeoCoordinate>();
                            route.Add(storeLocation);
                            route.Add(MyLocation);
                            CalculateRoute(route);
                            break;
                        case CustomMessageBoxResult.RightButton:
                            // Do something.
                            break;
                        case CustomMessageBoxResult.None:
                            // Do something.
                            break;
                        default:
                            break;
                    }
                };

                cmb.Show();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Store_Click()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }


  

        private RouteQuery MyRouteQuery = null;
        private bool _isRouteSearch = false; // True when route is being searched, otherwise false

        private TravelMode _travelMode = TravelMode.Walking; // Travel mode used when calculating route

        //...

        private void GeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            //...
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    //if (_isRouteSearch) // Query is made to locate the destination of a route
                    //{
                        // Only store the destination for drawing the map markers
                        MyCoordinates.Add(e.Result[0].GeoCoordinate);

                        // Route from current location to first search result
                        List<GeoCoordinate> routeCoordinates = new List<GeoCoordinate>();
                        routeCoordinates.Add(MyLocation);
                        routeCoordinates.Add(e.Result[0].GeoCoordinate);
                        CalculateRoute(routeCoordinates);
                    //}
                    //else // Query is made to search the map for a keyword
                    //{
                    //    //...
                    //}
                }
                //...
            }
            //...
        }

        private void CalculateRoute(List<GeoCoordinate> route)
        {
            //...
            MyRouteQuery = new RouteQuery();
            MyRouteQuery.TravelMode = _travelMode;
            MyRouteQuery.Waypoints = route;
            MyRouteQuery.QueryCompleted += RouteQuery_QueryCompleted;
            MyRouteQuery.QueryAsync();
        }


        private Route MyRoute = null;
        private MapRoute MyMapRoute = null;
        private MapRoute MyMapLastRoute = null;

        
        private void RouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (MyMapLastRoute != null) { 
            MyMap.RemoveRoute(MyMapLastRoute);}
            //...
            if (e.Error == null)
            {
                MyRoute = e.Result;
                MyMapRoute = new MapRoute(MyRoute);
                MyMap.AddRoute(MyMapRoute);
                MyMapLastRoute = MyMapRoute;

                // Update route information and directions
                //...
            }
            //...
        }

    


        #endregion

        #region "## CURRENT POSITION ##"

        /// <summary>
        /// Method to draw markers on top of the map. Old markers are removed.
        /// </summary>
        private void DrawMyLocation(MapLayer mapLayerCurrentPosition)
        {
            try
            {


                // Draw marker for current position
                if (MyLocation != null)
                {
                    DrawMyLocationAccuracyRadius(mapLayerCurrentPosition);
                    DrawMyLocationPictogram(MyLocation, Colors.Red, mapLayerCurrentPosition);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("DrawMyLocation()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void DrawMyLocationAccuracyRadius(MapLayer mapLayer)
        {
            try
            {


                // The ground resolution (in meters per pixel) varies depending on the level of detail 
                // and the latitude at which it’s measured. It can be calculated as follows:
                double metersPerPixels = (Math.Cos(MyLocation.Latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2, MyMap.ZoomLevel));
                double radius = _accuracy / metersPerPixels;

                Ellipse ellipse = new Ellipse();
                ellipse.Width = radius * 2;
                ellipse.Height = radius * 2;
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(75, 200, 0, 0));

                MapOverlay overlay = new MapOverlay();
                overlay.Content = ellipse;
                overlay.GeoCoordinate = new GeoCoordinate(MyLocation.Latitude, MyLocation.Longitude);
                overlay.PositionOrigin = new Point(0.5, 0.5);
                mapLayer.Add(overlay);
            }
            catch (Exception ex)
            {

                MessageBox.Show("DrawMyLocationAccuracyRadius()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void DrawMyLocationPictogram(GeoCoordinate coordinate, Color color, MapLayer mapLayer)
        {
            try
            {
                BitmapImage bi = new BitmapImage();

                bi.UriSource = new Uri("/Assets/Pins/me.png", UriKind.Relative);

                var image = new Image();
                image.Width = 26;
                image.Height = 38;
                image.Opacity = 100;
                image.Source = bi;

                image.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
                image.MouseLeftButtonUp += new MouseButtonEventHandler(MyLocationMarkerPictogram_Click);

                // Create a MapOverlay and add marker.
                MapOverlay overlay = new MapOverlay();
                overlay.Content = image;
                overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
                overlay.PositionOrigin = new Point(0.0, 1.0);
                mapLayer.Add(overlay);
            }
            catch (Exception ex)
            {

                MessageBox.Show("DrawMyLocationPictogram()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }


        private void MyLocationMarkerPictogram_Click(object sender, EventArgs e)
        {
            try
            {


                Image p = (Image)sender;
                GeoCoordinate geoCoordinate = (GeoCoordinate)p.Tag;
                if (MyReverseGeocodeQuery == null || !MyReverseGeocodeQuery.IsBusy)
                {
                    MyReverseGeocodeQuery = new ReverseGeocodeQuery();
                    MyReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(geoCoordinate.Latitude, geoCoordinate.Longitude);
                    MyReverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
                    MyReverseGeocodeQuery.QueryAsync();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("MyLocationMarkerPictogram_Click()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void CenterMapToLocation()
        {
            try
            {


                if (MyLocation != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        int dzl = 10;
                        int.TryParse(AppResources.DefaultZoomLevel, out dzl);
                        MyMap.SetView(MyLocation, dzl, MapAnimationKind.Parabolic);
                    });

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("CenterMapToLocation()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        #endregion



        /// <summary>
        /// Event handler for reverse geocode query.
        /// </summary>
        /// <param name="e">Results of the reverse geocode query - list of locations</param>
        private void ReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            try
            {


                if (e.Error == null)
                {
                    if (e.Result.Count > 0)
                    {
                        MapAddress address = e.Result[0].Information.Address;
                        String msgBoxText = "";
                        if (address.Street.Length > 0)
                        {
                            msgBoxText += "\n" + address.Street;
                            if (address.HouseNumber.Length > 0) msgBoxText += " " + address.HouseNumber;
                        }
                        if (address.PostalCode.Length > 0) msgBoxText += "\n" + address.PostalCode;
                        if (address.City.Length > 0) msgBoxText += "\n" + address.City;
                        if (address.Country.Length > 0) msgBoxText += "\n" + address.Country;
                        MessageBox.Show(msgBoxText, AppResources.ApplicationTitle, MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show(AppResources.NoInfoMessageBoxText, AppResources.ApplicationTitle, MessageBoxButton.OK);
                    }
                    MyReverseGeocodeQuery.Dispose();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("ReverseGeocodeQuery_QueryCompleted()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }



        /// <summary>
        /// Helper method to build a localized ApplicationBar
        /// </summary>
        private void BuildApplicationBar()
        {
            try
            {
                // Set the page's ApplicationBar to a new instance of ApplicationBar.    
                ApplicationBar = new ApplicationBar();

                ApplicationBar.Mode = ApplicationBarMode.Default;
                ApplicationBar.IsVisible = true;
                ApplicationBar.Opacity = 0.7;
                ApplicationBar.IsMenuEnabled = true;


                // Create new menu items with the localized strings from AppResources.
                ApplicationBarMenuItem AppBarSettingsMenuItem = null;
                AppBarSettingsMenuItem = new ApplicationBarMenuItem("settings...");
                AppBarSettingsMenuItem.Click += new EventHandler(Settings_Click);
                ApplicationBar.MenuItems.Add(AppBarSettingsMenuItem);


                AppBarAboutMenuItem = new ApplicationBarMenuItem("about...");
                AppBarAboutMenuItem.Click += new EventHandler(About_Click);
                ApplicationBar.MenuItems.Add(AppBarAboutMenuItem);
            }
            catch (Exception ex)
            {

                MessageBox.Show("BuildApplicationBar()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }
        Geolocator geolocator = null;
        bool tracking = false;
        int amountOfSearchedStores = 0;
        /// <summary>
        /// Helper method to show progress indicator in system tray
        /// </summary>
        /// <param name="msg">Text shown in progress indicator</param>
        private void ShowProgressIndicator(String msg)
        {
            try
            {
                amountOfSearchedStores++;
                if (amountOfSearchedStores <= 1)
                {
                    if (ProgressIndicator == null)
                    {
                        ProgressIndicator = new ProgressIndicator();
                        ProgressIndicator.IsIndeterminate = true;
                    }
                    ProgressIndicator.Text = msg;
                    ProgressIndicator.IsVisible = true;
                    SystemTray.SetProgressIndicator(this, ProgressIndicator);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("ShowProgressIndicator()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
            finally
            {

            }
        }

        private void ShowSearchingFullscreen(String msg)
        {
            try
            {

                LoadingPanel.Visibility = Visibility.Visible;
                if (ProgressIndicator == null)
                {
                    ProgressIndicator = new ProgressIndicator();
                    ProgressIndicator.IsIndeterminate = true;
                }
                ProgressIndicator.Text = msg;
                ProgressIndicator.IsVisible = true;
                SystemTray.SetProgressIndicator(this, ProgressIndicator);

            }
            catch (Exception ex)
            {

                MessageBox.Show("ShowSearchingFullscreen()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
            finally
            {

            }
        }

        /// <summary>
        /// Helper method to hide progress indicator in system tray
        /// </summary>
        private void HideProgressIndicator()
        {
            try
            {
                amountOfSearchedStores--;
                if (amountOfSearchedStores == 0)
                {
                    ProgressIndicator.IsVisible = false;
                    SystemTray.SetProgressIndicator(this, ProgressIndicator);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("HideProgressIndicator()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void HideSearchingFullscreen()
        {
            try
            {
                LoadingPanel.Visibility = Visibility.Collapsed;

                if (amountOfSearchedStores <= 0)
                {
                    ProgressIndicator.IsVisible = false;
                    SystemTray.SetProgressIndicator(this, ProgressIndicator);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("HideSearchingFullscreen()\n\n" + ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }


        // Application bar menu items
        private ApplicationBarMenuItem AppBarAboutMenuItem = null;

        // Progress indicator shown in system tray
        private ProgressIndicator ProgressIndicator = null;


        // List of coordinates representing search hits / destination of route
        private List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();

        // Reverse geocode query
        private ReverseGeocodeQuery MyReverseGeocodeQuery = null;


        /// <summary>
        /// True when this object instance has been just created, otherwise false
        /// </summary>
        private bool _isNewInstance = true;

        /// <summary>
        /// True when color mode has been temporarily set to light, otherwise false
        /// </summary>
        public bool _isTemporarilyLight = false;

        /// <summary>
        /// Accuracy of my current location in meters;
        /// </summary>
        private double _accuracy = 0.0;

        /// <summary>
        /// Used for saving location usage permission
        /// </summary>
        private IsolatedStorageSettings Settings;

    }
}