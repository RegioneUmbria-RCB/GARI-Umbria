namespace AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models
{
    /// <summary>
    /// Data Transfer Object representing geographic coordinates in WGS84 format.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida - Input section:
    /// Geographic coordinates used as alternative to station code for meteorological data retrieval.
    /// </summary>
    public class GeographicalCoordinates
    {
        /// <summary>
        /// The latitude coordinate in WGS84 format (decimal degrees).
        /// Valid range: -90 to +90
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// The longitude coordinate in WGS84 format (decimal degrees).
        /// Valid range: -180 to +180
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Determines if the coordinates are valid.
        /// </summary>
        /// <returns>True if both latitude and longitude are set to non-zero values, false otherwise.</returns>
        public bool IsValid()
        {
            return Latitude != 0 && Latitude > -90 && Latitude < 90 && 
                Longitude != 0 && Longitude > -180 && Longitude < 180;
        }
    }
}
