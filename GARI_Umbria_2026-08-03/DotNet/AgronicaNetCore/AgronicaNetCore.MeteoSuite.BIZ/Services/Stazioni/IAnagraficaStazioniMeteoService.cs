using AgronicaNetCore.Base.Models;
using InData.Engine.MeteoSuite;
using OutData.Engine.MeteoSuite;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni
{
    /// <summary>
    /// Interface for accessing (reading or editing) meteo stations, accessible by single user/enterprise, and public stations' alias.
    /// </summary>
    public interface IAnagraficaStazioniMeteoService
    {
        /// <summary>
        /// Read accessible meteo stations list by type and eventually (only for public stations) position, with distance if position is provided.
        /// </summary>
        Task<List<StazioneMeteoDto>> LeggiStazioniMeteoAutorizzateAsync(
            string Piva,
            int Tipo,
            decimal? Longitude, decimal? Latitude,
            int offset, int limit,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read single accessible meteo station by type and Id, with distance if position is provided.
        /// </summary>
        Task<StazioneMeteoDto?> LeggiStazioneMeteoPerIdAsync(
            string Piva,
            int Tipo,
            int IdStazione,
            decimal? Longitude, decimal? Latitude,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read single accessible meteo station by type and position.
        /// </summary>
        Task<StazioneMeteoDto?> LeggiStazioneMeteoPerPosizioneAsync(
            string Piva,
            int Tipo,
            decimal Longitude, decimal Latitude,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update meteo station (ownership permissions required) and its sensors descriptions, or create new virtual station (if id not provided).
        /// </summary>
        Task<StazioneMeteoDto?> AggiornaOCreaStazioneMeteoAsync(
            AggiornaStazioneMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deleted meteo station (ownership permissions required) and its sensors descriptions, or create new virtual station (if id not provided).
        /// </summary>
        Task<bool> EliminaStazioneMeteoAsync(
            string Piva,
            int IdStazione,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read all alias assigned by user to public stations.
        /// </summary>
        Task<List<StazioneMeteoAliasDto>> LeggiElencoAlias(
            string Piva,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read single public meteo station and associated by user alias by position.
        /// </summary>
        Task<StazioneMeteoAliasNewDto?> LeggiStazioneMeteoXAlias(
            string Piva,
            decimal Longitude, decimal Latitude,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update alias assigned by user to public stations.
        /// </summary>
        Task<StazioneMeteoAliasDto?> AggiornaAlias(
            string Piva,
            int IdStazione, 
            string NomeAlias,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete alias assigned by user to public stations.
        /// </summary>
        Task<bool> EliminaAlias(
            string Piva,
            int IdStazione,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
