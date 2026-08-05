using AgronicaNetCore.Base.Models;
using InData.Engine.MeteoSuite;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public interface IMeteoSuiteOnboardingDevicesService
    {
        Task<MeteoSuiteOnboardingDevicesDto?> LeggiStazioniAutorizzateAsynch(
            string piva, 
            bool? reale,
            MeteoStationVisibility visibility,
            int offset, int limit,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        Task<MeteoSuiteOnboardingDeviceDto?> LeggiStazionePerIdAsynch(
            string piva, 
            int IdStazione,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        Task<MeteoSuiteOnboardingDeviceDto?> LeggiStazionePerProviderCodeAsynch(
            string piva, 
            string ProviderCode,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        Task<MeteoSuiteOnboardingDeviceDto?> AggiornaOCreaStazioneAsynch(
            AggiornaStazioneMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        Task<bool> EliminaStazioneAsynch(
            string piva, 
            int IdStazione,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
