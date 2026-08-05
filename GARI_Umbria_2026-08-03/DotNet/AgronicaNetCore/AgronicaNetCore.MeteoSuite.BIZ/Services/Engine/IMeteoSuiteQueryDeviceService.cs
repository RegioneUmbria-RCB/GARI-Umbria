using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public interface IMeteoSuiteQueryDeviceService
    {
        Task<List<MeteoSuiteQueryWeatherDataPointDto>> LeggiDatiMeteoPerPosizioneAsync(
            string provider, decimal lon, decimal lat,
            string dataInizio, string dataFine,
            string[] sensori,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        Task<List<MeteoSuiteQueryWeatherDataPointDto>> LeggiDatiMeteoPerCodiceAsync(
            string stationCode, 
            string dataInizio, string dataFine,
            string? Piva,
            string[] sensori,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        Task<MeteoSuiteQueryWeatherStationDto?> LeggiStazionePerPosizioneAsync(
            string provider, decimal lon, decimal lat,
            string? Piva,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        Task<MeteoSuiteQueryWeatherStationDto?> LeggiStazionePerCodiceAsync(
            string stationCode,
            string? Piva,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
