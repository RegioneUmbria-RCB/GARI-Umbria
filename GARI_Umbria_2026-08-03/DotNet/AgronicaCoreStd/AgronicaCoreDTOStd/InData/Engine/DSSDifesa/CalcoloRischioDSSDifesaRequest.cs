using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace InData.Engine.DSSDifesa
{
    public class CalcoloRischioDSSDifesaRequest
    {
        /// <summary>
        /// Geographical coordinates model.
        /// </summary>
        public class GeoCoordinates
        {
            /// <summary>
            /// Latitude in WGS84 decimal degrees (-90 to 90).
            /// </summary>
            //[Required]
            //[Range(-90, 90)]
            public double Latitude { get; set; }

            /// <summary>
            /// Longitude in WGS84 decimal degrees (-180 to 180).
            /// </summary>
            //[Required]
            //[Range(-180, 180)]
            public double Longitude { get; set; }
        }

        /// <summary>
        /// Station code for weather data acquisition. If null, coordinates must be provided.
        /// </summary>
        public string StationCod { get; set; } = string.Empty;

        /// <summary>
        /// Geographical coordinates for weather data acquisition (WGS84). Required if station_cod is null.
        /// </summary>
        public GeoCoordinates Coordinates { get; set; } = null;

        /// <summary>
        /// Start date for risk calculation (ISO 8601 format).
        /// </summary>
        //[Required]
        public string DataInizio { get; set; } = string.Empty;

        /// <summary>
        /// End date for risk calculation (ISO 8601 format).
        /// </summary>
        //[Required]
        public string DataFine { get; set; } = string.Empty;

        /// <summary>
        /// Crop code for which to calculate phytosanitary risk.
        /// </summary>
        //[Required]
        public string CropCode { get; set; } = string.Empty;

        /// <summary>
        /// Crop variety code (optional).
        /// </summary>
        public string VarCode { get; set; }

        /// <summary>
        /// List of specific adversity model codes to consider. If empty, all available models for the crop will be used.
        /// </summary>
        public List<string> ModelliCodici { get; set; } = new List<string>();

        /// <summary>
        /// Detail level for the calculation results.
        /// </summary>
        public int LivelloDettaglio { get; set; }
    }
}
