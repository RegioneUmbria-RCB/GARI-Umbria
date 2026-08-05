using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Data Transfer Object representing the request for hybrid meteorological data acquisition.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida - Input section:
    /// Handles both station-based and coordinate-based meteorological data retrieval.
    /// Either station_cod or coordinates must be provided, but not both as the only input method.
    /// </summary>
    public class AcquisizioneMeteoIbridaRequest
    {
        /// <summary>
        /// The meteorological station code (optional).
        /// If provided and not null, takes precedence over coordinates for data retrieval.
        /// Format: string identifier for the meteorological station
        /// </summary>
        public string? StationCod { get; set; }

        /// <summary>
        /// The geographic coordinates for data retrieval (optional).
        /// Used when StationCod is null or empty.
        /// Coordinates must be valid WGS84 format.
        /// </summary>
        public GeographicalCoordinates? Coordinates { get; set; }

        /// <summary>
        /// Start date for the meteorological data retrieval period.
        /// ISO 8601 format (e.g., "2026-03-27T00:00:00Z")
        /// </summary>
        public string DataInizio { get; set; } = string.Empty;

        /// <summary>
        /// End date for the meteorological data retrieval period.
        /// ISO 8601 format (e.g., "2026-03-28T23:00:00Z")
        /// </summary>
        public string DataFine { get; set; } = string.Empty;

        /// <summary>
        /// Determines if the request has valid input for data acquisition.
        /// Either StationCod must be provided, or Coordinates must be valid.
        /// </summary>
        /// <returns>True if the request can be processed, false if no valid data source is provided.</returns>
        public bool HasValidDataSource()
        {
            bool hasStation = !string.IsNullOrWhiteSpace(StationCod);
            bool hasCoordinates = Coordinates != null && Coordinates.IsValid();
            return hasStation || hasCoordinates;
        }
    }
}
