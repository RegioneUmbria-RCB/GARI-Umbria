using AgronicaNetCore.MeteoSuite.DAL.DataLayer.DatiMeteo.Models;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Data Transfer Object representing the request for hybrid meteorological data acquisition.
    /// 
    /// Handles both station-based and coordinate-based meteorological data retrieval.
    /// Either station_cod or coordinates must be provided, but not both as the only input method.
    /// </summary>
    public class AcquisizioneDatiMeteoRequest
    {
        /// <summary>
        /// The meteorological station code (optional).
        /// If provided and not null, takes precedence over coordinates for data retrieval.
        /// Format: string identifier code for the meteorological station
        /// </summary>
        public string? StationCode { get; set; }

        /// <summary>
        /// The meteorological station id (optional).
        /// The ID is an alternative for obtaining station cod, if this is not provided
        /// Format: integer identifier for the meteorological station
        /// </summary>
        public int? StationId { get; set; }

        /// <summary>
        /// The station type.
        /// Format: integer identifier 
        /// </summary>
        public int? TipoStazione { get; set; }

        /// <summary>
        /// The meteorological station's owner' (optional).
        /// Used for retriving station code by id
        /// </summary>
        public string? Piva { get; set; }

        /// <summary>
        /// The geographic coordinates for data retrieval (optional).
        /// Used when StationCod is null or empty.
        /// Coordinates must be valid WGS84 format.
        /// </summary>
        public GeographicalCoordinates? Coordinates { get; set; }

        /// <summary>
        /// The meteo provider for data retrieval (optional).
        /// Used when StationCod is null or empty.
        /// If null or empty "HYPERMETEO" is used as default provider (public meteo data).
        /// </summary>
        public string? Provider { get; set; }

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
        /// Last days to consider for reading meteo data.
        /// </summary>
        public int? IntervalloG { get; set; }

        /// <summary>
        /// Last hours to consider for reading meteo data.
        /// </summary>
        public int? IntervalloH { get; set; }

        /// <summary>
        /// If set calculates termical sum (Somma Termica) derivated meteo value.
        /// </summary>
        public decimal? SogliaTermica { get; set; }

        /// <summary>
        /// If set calculates chilling requirement accumulation (Cumulo Fabbisogno Freddo) derivated meteo value, 
        /// as sum of the hours temperature temperature go below this value (only for daily readings)
        /// </summary>
        public decimal? SogliaFabbisognoFreddo { get; set; }

        /// <summary>
        /// Determines if the request has valid input for data acquisition.
        /// Either StationCod must be provided, or Coordinates must be valid, 
        /// as sum of the hours temperature temperature go below this value  (only for daily readings)
        /// </summary>
        /// <returns>True if the request can be processed, false if no valid data source is provided.</returns>
        public bool HasValidDataSource()
        {
            bool hasStation = !string.IsNullOrWhiteSpace(StationCode);
            bool hasCoordinates = Coordinates != null && Coordinates.IsValid();
            return hasStation || hasCoordinates;
        }
    }
}
