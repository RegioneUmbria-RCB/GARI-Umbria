using AgronicaCoreModelsSTD.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class GoogleGeolocationResponse
    {
        public List<GoogleGeolocationResult> results { get; set; }
        public string status { get; set; }
    }

    public class GoogleGeolocationResult
    {
        public List<GoogleGeolocationItem> address_components { get; set; }
        public string formatted_address { get; set; }
        public GoogleGeolocationGeometry geometry { get; set; }
        public string place_id { get; set; }
        public List<String> types { get; set; }

        public string country()
        {
            return this.address_components.Find((i) => i.types.Exists((t) => t == "country"))?.short_name;
        }

        public LatLng location()
        {
            return this.geometry.location;
        }

        public string city()
        {
            return this.address_components.Find((i) => i.types.Exists((t) => t == "locality"))?.short_name;
        }
    }

    public class GoogleGeolocationItem
    { 
    public String long_name { get; set; }
    public String short_name { get; set;  }
    public List<string> types { get; set; }
    }

    public class GoogleGeolocationGeometry
    { 
    public Bound bounds { get; set; }
    public LatLng location {get; set; }
    public String location_type { get; set; }
    public Bound viewport { get; set; }
    }

    public class Bound
    { 
    public LatLng northeast { get; set; }
    public LatLng southwest { get; set; } 
    }

    public class LatLng
    { 
        public decimal lat { get; set; }
        public decimal lng { get; set; }

        public LatLng(decimal _lat, decimal _lng)
        {
            this.lat = _lat;
            this.lng = _lng;
        }

        public LatLng()
        {
            this.lat = 0;
            this.lng = 0;
        }
    }
}
