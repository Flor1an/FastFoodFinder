using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using FastFoodFinder.Resources;
using System.IO.IsolatedStorage;


namespace FastFoodFinder
{
    public partial class SettingsPage : PhoneApplicationPage
    {

        private IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;
        ApplicationBar ab = new ApplicationBar();
        ApplicationBarIconButton restaurantsSortByName = new ApplicationBarIconButton();
        ApplicationBarIconButton restaurantsSortBySelection = new ApplicationBarIconButton();


        public SettingsPage()
        {
            InitializeComponent();

            if (Settings.Contains("isLocationAllowed"))
            {
                locationtTggle.IsChecked = (bool)Settings["isLocationAllowed"];
            }

            if (Settings.Contains("isTrackingAllowed"))
            {
                trackingTggle.IsChecked = (bool)Settings["isTrackingAllowed"];
            }

            if (Settings.Contains("ColorMode"))
            {
                if ((MapColorMode)Settings["ColorMode"] == MapColorMode.Dark)
                {
                    colorModeTggle.IsChecked = true;
                }
                else
                {
                    colorModeTggle.IsChecked = false;
                }
            }

            if (Settings.Contains("LandmarksEnabled"))
            {
                landmarkTggle.IsChecked = (bool)Settings["LandmarksEnabled"];
            }

            if (Settings.Contains("PedestrianFeatures"))
            {
                pedestrianFeaturesTggle.IsChecked = (bool)Settings["PedestrianFeatures"];
            }

            if (Settings.Contains("mapMode"))
            {
                changeMapMode((MapCartographicMode)Settings["mapMode"]);
            }

            if (Settings.Contains("ZoomControls"))
            {
                zoomControlsTggle.IsChecked = (bool)Settings["ZoomControls"];
            }





            //2nd page
            if (Settings.Contains(AppResources.SearchStringMcDonalds))
            {
                mcdonaldsToggle.IsChecked = (bool)Settings[AppResources.SearchStringMcDonalds];
            }
            if (Settings.Contains(AppResources.SearchStringKFC))
            {
                kfcToggle.IsChecked = (bool)Settings[AppResources.SearchStringKFC];
            }
            if (Settings.Contains(AppResources.SearchStringBurgerKing))
            {
                burgerKingToggle.IsChecked = (bool)Settings[AppResources.SearchStringBurgerKing];
            }
            if (Settings.Contains(AppResources.SearchStringVapiano))
            {
                vapianoToggle.IsChecked = (bool)Settings[AppResources.SearchStringVapiano];
            }
            if (Settings.Contains(AppResources.SearchStringStarbucks))
            {
                starbucksToggle.IsChecked = (bool)Settings[AppResources.SearchStringStarbucks];
            }
            if (Settings.Contains(AppResources.SearchStringSubway))
            {
                subwayToggle.IsChecked = (bool)Settings[AppResources.SearchStringSubway];
            }
            if (Settings.Contains(AppResources.SearchStringNordsee))
            {
                nordseeToggle.IsChecked = (bool)Settings[AppResources.SearchStringNordsee];
            }
            if (Settings.Contains(AppResources.SearchStringJimBlock))
            {
                jimblockToggle.IsChecked = (bool)Settings[AppResources.SearchStringJimBlock];
            }


            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.IsVisible = false;
            ApplicationBar.Opacity = 0.7;
            //ab.IsMenuEnabled = true;

            restaurantsSortByName = new ApplicationBarIconButton(new Uri("/Assets/sort1.png", UriKind.Relative));

            restaurantsSortByName.Text = "by name";
            restaurantsSortByName.Click += new EventHandler(restaurantsSortByName_Click);
            restaurantsSortByName.IsEnabled = true;
            ApplicationBar.Buttons.Add(restaurantsSortByName);

            restaurantsSortBySelection = new ApplicationBarIconButton(new Uri("/Assets/sort1.png", UriKind.Relative));
            restaurantsSortBySelection.Text = "by selection";
            restaurantsSortBySelection.Click += new EventHandler(restaurantsSortBySelection_Click);
            restaurantsSortBySelection.IsEnabled = false;

            ApplicationBar.Buttons.Add(restaurantsSortBySelection);

        }

        private void restaurantsSortBySelection_Click(object sender, EventArgs e)
        {
            var button = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            button.IconUri = new Uri("/Assets/sort2.png", UriKind.Relative);
        }

        private void restaurantsSortByName_Click(object sender, EventArgs e)
        {
            var button = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            button.IconUri = new Uri("/Assets/sort2.png", UriKind.Relative);


            foreach (Control item in restaurantsToggleList.Children)
            {
                //if (item.GetType() is 
            }

        }

        private void changeMapMode(MapCartographicMode mm)
        {
            RoadButton.IsEnabled = true;
            AerialButton.IsEnabled = true;
            HybridButton.IsEnabled = true;
            TerrainButton.IsEnabled = true;

            switch (mm)
            {
                case MapCartographicMode.Road:
                    colorModeTggle.IsEnabled = true;
                    RoadButton.IsEnabled = false;
                    Settings["mapMode"] = MapCartographicMode.Road;
                    break;

                case MapCartographicMode.Aerial:
                    AerialButton.IsEnabled = false;
                    Settings["mapMode"] = MapCartographicMode.Aerial;

                    break;

                case MapCartographicMode.Hybrid:
                    HybridButton.IsEnabled = false;
                    Settings["mapMode"] = MapCartographicMode.Hybrid;

                    break;

                case MapCartographicMode.Terrain:
                    TerrainButton.IsEnabled = false;
                    Settings["mapMode"] = MapCartographicMode.Terrain;

                    break;

                default:
                    break;
            }
            Settings.Save();
        }

        private void colorModeTggle_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)colorModeTggle.IsChecked)
            {
                Settings["ColorMode"] = MapColorMode.Dark;
            }
            else
            {
                Settings["ColorMode"] = MapColorMode.Light;
            }

            Settings.Save();
        }

        private void landmarkTggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains("LandmarksEnabled"))
            {
                Settings["LandmarksEnabled"] = landmarkTggle.IsChecked;
            }
            else
            {
                Settings.Add("LandmarksEnabled", landmarkTggle.IsChecked);
            }

            Settings.Save();
        }

        private void pedestrianFeaturesTggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains("PedestrianFeatures"))
            {
                Settings["PedestrianFeatures"] = pedestrianFeaturesTggle.IsChecked;
            }
            else
            {
                Settings.Add("PedestrianFeatures", pedestrianFeaturesTggle.IsChecked);
            }

            Settings.Save();
        }

        private void locationtTggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains("isLocationAllowed"))
            {
                Settings["isLocationAllowed"] = locationtTggle.IsChecked;
            }
            else
            {
                Settings.Add("isLocationAllowed", locationtTggle.IsChecked);
            }
            Settings.Save();
        }

        private void trackingTggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains("isTrackingAllowed"))
            {
                Settings["isTrackingAllowed"] = trackingTggle.IsChecked;
            }
            else
            {
                Settings.Add("isTrackingAllowed", trackingTggle.IsChecked);
            }
            Settings.Save();
        }


        private void CartographicModeButton_Click(object sender, EventArgs e)
        {
            if (sender == RoadButton)
            {
                changeMapMode(MapCartographicMode.Road);
            }
            else if (sender == AerialButton)
            {
                changeMapMode(MapCartographicMode.Aerial);
            }
            else if (sender == HybridButton)
            {
                changeMapMode(MapCartographicMode.Hybrid);
            }
            else if (sender == TerrainButton)
            {
                changeMapMode(MapCartographicMode.Terrain);
            }
        }

        private void zoomControlsTggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains("ZoomControls"))
            {
                Settings["ZoomControls"] = zoomControlsTggle.IsChecked;
            }
            else
            {
                Settings.Add("ZoomControls", zoomControlsTggle.IsChecked);
            }
            Settings.Save();
        }


        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void mcdonaldsToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringMcDonalds))
            {
                Settings[AppResources.SearchStringMcDonalds] = mcdonaldsToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringMcDonalds, mcdonaldsToggle.IsChecked);
            }

            Settings.Save();
        }

        private void kfcToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringKFC))
            {
                Settings[AppResources.SearchStringKFC] = kfcToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringKFC, kfcToggle.IsChecked);
            }

            Settings.Save();
        }

        private void burgerKingToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringBurgerKing))
            {
                Settings[AppResources.SearchStringBurgerKing] = burgerKingToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringBurgerKing, burgerKingToggle.IsChecked);
            }

            Settings.Save();
        }

        private void panorma_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentPanormaItem = ((Panorama)sender).SelectedItem;
            if (currentPanormaItem.Equals(globalPanorma))
            {
                ApplicationBar.IsVisible = false;
            }
            else if (currentPanormaItem.Equals(restaurantsPanorma))
            {
                ApplicationBar.IsVisible = false; //must be true later

            }
        }

        private void vapianoToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringVapiano))
            {
                Settings[AppResources.SearchStringVapiano] = vapianoToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringVapiano, vapianoToggle.IsChecked);
            }

            Settings.Save();
        }

        private void starbucksToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringStarbucks))
            {
                Settings[AppResources.SearchStringStarbucks] = starbucksToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringStarbucks, starbucksToggle.IsChecked);
            }

            Settings.Save();
        }

        private void subwayToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringSubway))
            {
                Settings[AppResources.SearchStringSubway] = subwayToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringSubway, subwayToggle.IsChecked);
            }

            Settings.Save();
        }

        private void nordseeToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringNordsee))
            {
                Settings[AppResources.SearchStringNordsee] = nordseeToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringNordsee, nordseeToggle.IsChecked);
            }

            Settings.Save();
        }

        private void jimblockToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Contains(AppResources.SearchStringJimBlock))
            {
                Settings[AppResources.SearchStringJimBlock] = jimblockToggle.IsChecked;
            }
            else
            {
                Settings.Add(AppResources.SearchStringJimBlock, jimblockToggle.IsChecked);
            }

            Settings.Save();
        }












    }
}