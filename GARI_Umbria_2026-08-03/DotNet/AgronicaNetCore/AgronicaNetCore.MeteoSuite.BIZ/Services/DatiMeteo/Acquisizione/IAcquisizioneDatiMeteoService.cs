using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MeteoSuite.BIZ.Services.Engine.DatiMeteo.Acquisizione;
using InData.Engine.MeteoSuite;
using OutData.Engine.MeteoSuite;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Interface for the meteorological data acquisition business logic service.
    /// 
    /// Referenced in DS07-BL Aggregazione Dati Meteo Orari and DS08-BL  Aggregazione Dati Meteo Giornaliero:
    /// "Acquisire i dati meteorologici orari e giornalieri
    /// per centralina meteo o coordinate geografiche, a seconda del metodo chiamante, 
    /// standardizzati, con normalizzazione, validazione e tracciabilità della fonte."
    /// </summary>
    public interface IAcquisizioneDatiMeteoService
    {
        /// <summary>
        /// Acquires hourly meteorological data (all sensors' data for timestamp) using either station id, station code or geographic coordinates.
        /// </summary>
        /// <param name="request">The request containing station code or coordinates and date range</param>
        /// <returns>Response containing meteorological data and overall validation status</returns>
        /// <exception cref="Exceptions.MeteoMissingInputDataException">
        /// When neither station code nor valid coordinates are provided
        /// </exception>
        /// <exception cref="Exceptions.MeteoDevicesTimeoutException">
        /// When API request exceeds 2 second timeout
        /// </exception>
        /// <exception cref="Exceptions.MeteoUnauthorizedException">
        /// When API returns 401 Unauthorized (invalid API key)
        /// </exception>
        /// <exception cref="Exceptions.MeteoStationNotFoundException">
        /// When API returns 404 Not Found (station not found)
        /// </exception>
        /// <exception cref="Exceptions.MeteoInsuccessResponseException">
        /// When API returns other error status codes
        /// </exception>
        /// <exception cref="Exceptions.MeteoInvalidResponseException">
        /// When API returns malformed response data
        /// </exception>
        Task<AcquisizioneDatiMeteoResponse> AcquisisciDatiMeteoOrariAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Acquires daily meteorological data (all sensors' data for timestamp) using either station id, station code or geographic coordinates.
        /// </summary>
        /// <param name="request">The request containing station code or coordinates and date range</param>
        /// <returns>Response containing meteorological data and overall validation status</returns>
        /// <exception cref="Exceptions.MeteoMissingInputDataException">
        /// When neither station code nor valid coordinates are provided
        /// </exception>
        /// <exception cref="Exceptions.MeteoDevicesTimeoutException">
        /// When API request exceeds 2 second timeout
        /// </exception>
        /// <exception cref="Exceptions.MeteoUnauthorizedException">
        /// When API returns 401 Unauthorized (invalid API key)
        /// </exception>
        /// <exception cref="Exceptions.MeteoStationNotFoundException">
        /// When API returns 404 Not Found (station not found)
        /// </exception>
        /// <exception cref="Exceptions.MeteoInsuccessResponseException">
        /// When API returns other error status codes
        /// </exception>
        /// <exception cref="Exceptions.MeteoInvalidResponseException">
        /// When API returns malformed response data
        /// </exception>
        Task<AcquisizioneDatiMeteoResponse> AcquisisciDatiMeteoGiornalieriAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Acquires hourly meteorological data (only for rain, temperature and humidity) using either station id, station code or geographic coordinates.
        /// </summary>
        /// <param name="request">The request containing station code or coordinates and date range</param>
        /// <returns>Response containing meteorological data and overall validation status</returns>
        Task<AcquisizioneDatiMeteoPioggeOrariResponse> AcquisisciDatiMeteoPioggeOrariAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Acquires daily meteorological data (only for rain, temperature and humidity) using either station id, station code or geographic coordinates.
        /// </summary>
        /// <param name="request">The request containing station code or coordinates and date range</param>
        /// <returns>Response containing meteorological data and overall validation status</returns>
        Task<AcquisizioneDatiMeteoPioggeGiornalieriResponse> AcquisisciDatiMeteoPioggeGiornalieriAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            bool leggiOrePiogge = false,
            CancellationToken cancellationToken = default);

        Task<AcquisizioneDatiMeteoPioggeResponse> AcquisisciDatiMeteorologiciPioggeAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
