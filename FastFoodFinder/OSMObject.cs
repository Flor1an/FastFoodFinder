using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodFinder
{


    public class OSMObject
    {
        public string place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public string osm_id { get; set; }
        public List<object> boundingbox { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public string @class { get; set; }
        public string type { get; set; }
        public double importance { get; set; }
        public string icon { get; set; }
        public OSMAddress address { get; set; }


        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            OSMObject p = obj as OSMObject;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (display_name ==p.display_name);
        }

        public bool Equals(OSMObject other)
        {
            // If parameter is null return false:
            if ((object)other == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (display_name == other.display_name);
        }

        public override int GetHashCode()
        {
            return display_name.GetHashCode();
        }
    }

    public class OSMAddress
    {
        public string fast_food { get; set; }
        public string footway { get; set; }
        public string suburb { get; set; }
        public string city_district { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public string continent { get; set; }
        public string road { get; set; }
        public string postcode { get; set; }
        public string house_number { get; set; }
        public string pedestrian { get; set; }


    }

}
