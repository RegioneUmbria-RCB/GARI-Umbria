using AgronicaNetCore.Base.Models;
using InData.Zoo.DataMars;
using OutData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.AcquisizionePesate;

/// <summary>
/// Contratto per il ciclo completo di acquisizione pesate bovine da API Datamars.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI.</para>
/// </summary>
public interface IAcquisizionePesateDatamarsService
{
    /// <summary>
    /// Esegue il ciclo completo di acquisizione: autenticazione OAuth2 → estrazione FarmID →
    /// GET sessioni → deduplicazione → GET dettaglio → validazione → persistenza atomica → conteggi.
    /// Il token OAuth2 viene ottenuto internamente tramite le credenziali fornite e rinnovato
    /// automaticamente 10 minuti prima della scadenza.
    /// </summary>
    /// <param name="credenziali">Credenziali OAuth2 Datamars (grant_type, client_id, client_secret).</param>
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Autenticazione DataMars API.</para>
    Task<RisultatoAcquisizionePesate> AcquisisciPesateAsync(
        DatamarsCredentials credenziali,
        AgronicaCoreParametriServer objParametriServer,
        AgronicaCoreParametriSuperServer objParametriSuperServer,
        CancellationToken cancellationToken = default);
}
