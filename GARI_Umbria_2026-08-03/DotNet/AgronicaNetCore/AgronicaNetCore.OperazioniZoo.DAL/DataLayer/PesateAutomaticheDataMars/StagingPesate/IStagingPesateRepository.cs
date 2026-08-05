using AgronicaNetCore.Base.Models;
using InData.Zoo.DataMars;
using System.Data;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StagingPesate;

/// <summary>
/// Accesso ai dati per la tabella STAGING_PESATE.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Persistenze Coinvolte, STAGING_PESATE;
/// DS02-BL TrasformazionePesateStagingAgenda — Persistenze Coinvolte, STAGING_PESATE (UPDATE).</para>
/// </summary>
public interface IStagingPesateRepository
{
    /// <summary>
    /// Verifica se una sessione è già presente in STAGING_PESATE con flag_validazione diverso da 'ERRORE_TOTALE'
    /// (idempotenza: evita la re-importazione di sessioni già acquisite correttamente).
    /// </summary>
    Task<bool> SessioneEsistenteAsync(string idSessione, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Inserisce un nuovo record di staging per una sessionIntegration Datamars.
    /// </summary>
    Task<bool> InsertAsync(StagingPesataRecord record, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Legge un batch di record di staging non ancora processati (processato = 0),
    /// ordinati per timestamp_inizio_sessione ASC.
    /// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda — Integrazione DataProvider,
    /// Query lettura STAGING_PESATE.</para>
    /// </summary>
    Task<DataTable> GetPesateNonProcessateAsync(int batchSize, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Aggiorna STAGING_PESATE impostando processato=1, timestamp_processamento=NOW() e id_agenda
    /// per il record identificato dall'id primario.
    /// <para>Riferimento spec: DS02-BL — Step 5 Cross-Reference; Persistenze Coinvolte, STAGING_PESATE (UPDATE).</para>
    /// </summary>
    Task<bool> UpdateProcessatoAsync(long id, int idAgenda, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Aggiorna STAGING_PESATE impostando processato=1, timestamp_processamento=NOW(), id_agenda
    /// e num_pesate_accorpate per il record identificato dall'id primario.
    /// Usato per marcare le sessioni in cui le pesate sono state accorpate per data.
    /// <para>Riferimento spec: DS02-BL — Step 2d Update STAGING_PESATE con num_pesate_accorpate.</para>
    /// </summary>
    Task<bool> UpdateProcessatoConAccorpamentoAsync(long id, int idAgenda, int numPesateAccorpate, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Aggiorna STAGING_PESATE impostando errore_processamento per il record identificato dall'id primario.
    /// Il flag processato rimane false per consentire il retry se l'errore è transitorio.
    /// <para>Riferimento spec: DS02-BL — Step 7 Gestione Errori; Eccezioni (TransactionRollbackException).</para>
    /// </summary>
    Task<bool> UpdateErroreAsync(long id, string erroreProcessamento, AgronicaCoreParametriServer objParametriServer);
}
