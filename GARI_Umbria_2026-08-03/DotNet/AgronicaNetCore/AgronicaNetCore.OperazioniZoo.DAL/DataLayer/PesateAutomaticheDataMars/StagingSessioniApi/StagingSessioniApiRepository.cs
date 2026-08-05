using AgronicaNetCore.Base.Models;
using InData.Zoo.DataMars;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StagingSessioniApi;

/// <summary>
/// Implementazione del repository per STAGING_SESSIONI_API.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Step 4 Persistenza atomica,
/// STAGING_SESSIONI_API (metadati di riepilogo di ogni sessione).</para>
/// </summary>
public sealed class StagingSessioniApiRepository : BaseDALOperazioniZoo, IStagingSessioniApiRepository
{
    public StagingSessioniApiRepository(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    /// <inheritdoc/>
    public async Task<bool> InsertAsync(StagingSessioneApiRecord record, AgronicaCoreParametriServer objParametriServer)
    {
        var sb = new StringBuilder();
        var parameters = new ExpandoObject();

        sb.AppendLine("INSERT INTO STAGING_SESSIONI_API");
        sb.AppendLine("    (id_sessione, timestamp_inizio, timestamp_fine, record_ricevuti, record_validati,");
        sb.AppendLine("     record_errori, record_persistiti, status, errore_descrizione, numero_tentativi)");
        sb.AppendLine("VALUES");
        sb.AppendLine("    (@idSessione, @timestampInizio, @timestampFine, @recordRicevuti, @recordValidati,");
        sb.AppendLine("     @recordErrori, @recordPersistiti, @status, @erroreDescrizione, @numeroTentativi)");

        parameters.TryAdd("@idSessione", record.IdSessione);
        parameters.TryAdd("@timestampInizio", record.TimestampInizio);
        parameters.TryAdd("@timestampFine", record.TimestampFine);
        parameters.TryAdd("@recordRicevuti", record.RecordRicevuti);
        parameters.TryAdd("@recordValidati", record.RecordValidati);
        parameters.TryAdd("@recordErrori", record.RecordErrori);
        parameters.TryAdd("@recordPersistiti", record.RecordPersistiti);
        parameters.TryAdd("@status", record.Status);
        parameters.TryAdd("@erroreDescrizione", (object?)record.ErroreDescrizione ?? DBNull.Value);
        parameters.TryAdd("@numeroTentativi", record.NumeroTentativi);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(sb.ToString(), parameters);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
