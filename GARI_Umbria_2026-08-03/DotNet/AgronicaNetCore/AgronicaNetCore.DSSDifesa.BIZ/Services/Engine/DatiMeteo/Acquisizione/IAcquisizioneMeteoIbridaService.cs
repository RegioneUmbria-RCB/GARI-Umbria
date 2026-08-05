using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Interface for the hybrid meteorological data acquisition business logic service.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "Acquisire i dati meteorologici orari (temperatura, umidità, pioggia, bagnatura fogliare) 
    /// per centralina meteo o coordinate geografiche, a seconda del metodo chiamante, 
    /// standardizzati, con normalizzazione, validazione e tracciabilità della fonte."
    /// </summary>
    public interface IAcquisizioneMeteoIbridaService
    {
        /// <summary>
        /// Acquires meteorological data using either station code or geographic coordinates.
        /// 
        /// Referenced in DS02-BL_:
        /// "Acquisire i dati meteorologici orari (temperatura, umidità, pioggia, bagnatura fogliare) 
        /// per centralina meteo o o coordinate geografiche, a seconda del metodo chiamante, 
        /// standardizzati, con normalizzazione, validazione e tracciabilità della fonte."
        /// </summary>
        /// <param name="request">The request containing station code or coordinates and date range</param>
        /// <returns>Response containing meteorological data and overall validation status</returns>
        /// <exception cref="Exceptions.MeteoMissingInputDataException">
        /// When neither station code nor valid coordinates are provided
        /// </exception>
        /// <exception cref="Exceptions.MeteoTimeoutException">
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
        Task<AcquisizioneMeteoIbridaResponse> AcquisisciDatiMeteorologiciAsync(
            AcquisizioneMeteoIbridaRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
