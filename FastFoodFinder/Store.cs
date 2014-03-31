
using FastFoodFinder.Resources;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodFinder
{
    class Store
    {
        WebClient osmClient = new WebClient();
        List<OSMObject> stores = new List<OSMObject>();
        String _name = "";
        MainPage _parent;
        double distanceFromPosition = 0.04;

        public Store(string name,MainPage parent)
        {
            try
            {
                _name = name;
                _parent = parent;

                //osmClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);

                ////GeoCoordinate MyCoordinate = ((MainPage)(((System.Windows.Controls.ContentControl)(App.RootFrame)).Content)).getMyLocation();
                //GeoCoordinate MyCoordinate = _parent.getMyLocation();

                //string l = (MyCoordinate.Longitude - distanceFromPosition).ToString().Replace(",", ".");
                //string t = (MyCoordinate.Latitude + distanceFromPosition).ToString().Replace(",", ".");
                //string r = (MyCoordinate.Longitude + distanceFromPosition).ToString().Replace(",", ".");
                //string b = (MyCoordinate.Latitude - distanceFromPosition).ToString().Replace(",", ".");

                //osmClient.DownloadStringAsync(new Uri(AppResources.SearchURL + "?q=" + name + "&format=json&polygon=0&addressdetails=0&bounded=1&viewbox=" + l + "," + t + "," + r + "," + b));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void searchForFastFood(GeoCoordinate MyLocation)
        {
            try
            {
                osmClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);

                string l = (MyLocation.Longitude - distanceFromPosition).ToString().Replace(",", ".");
                string t = (MyLocation.Latitude + distanceFromPosition).ToString().Replace(",", ".");
                string r = (MyLocation.Longitude + distanceFromPosition).ToString().Replace(",", ".");
                string b = (MyLocation.Latitude - distanceFromPosition).ToString().Replace(",", ".");

                osmClient.DownloadStringAsync(new Uri(AppResources.SearchURL + "?q=" + _name + "&format=json&polygon=0&addressdetails=1&bounded=1&viewbox=" + l + "," + t + "," + r + "," + b));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                //stores = JsonConvert.DeserializeObject<List<OSMObject>>(e.Result);

                ////((MainPage)(((System.Windows.Controls.ContentControl)(App.RootFrame)).Content)).dataRecived(_name, new HashSet<OSMObject>(stores));
                //_parent.dataRecived(_name, new HashSet<OSMObject>(stores));


       
                try
                {
                    if (!string.IsNullOrEmpty(e.Result))
                    {
                        stores = (List<OSMObject>)JsonConvert.DeserializeObject(e.Result, typeof(List<OSMObject>),
                        new JsonSerializerSettings
                        {
                            Error = delegate(object sender1, ErrorEventArgs args)
                            {
                                args.ErrorContext.Handled = true;
                            }
                        });

                        _parent.dataRecived(_name, new HashSet<OSMObject>(stores));

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }




    }
}
