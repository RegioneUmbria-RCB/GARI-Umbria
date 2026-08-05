using AgronicaNetCore.Base.Models;
using InData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StagingSessioniApi;

/// <summary>
/// Accesso ai dati per la tabella STAGING_SESSIONI_API.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Persistenze Coinvolte, STAGING_SESSIONI_API.</para>
/// </summary>
public interface IStagingSessioniApiRepository
{
    /// <summary>
    /// Inserisce un nuovo record di metadati/conteggi per una sessione di acquisizione.
    /// </summary>
    Task<bool> InsertAsync(StagingSessioneApiRecord record, AgronicaCoreParametriServer objParametriServer);
}
