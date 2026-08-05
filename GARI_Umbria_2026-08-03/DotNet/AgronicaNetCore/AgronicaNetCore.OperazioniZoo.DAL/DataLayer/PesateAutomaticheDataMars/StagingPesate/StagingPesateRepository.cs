using AgronicaNetCore.Base.Models;
using InData.Zoo.DataMars;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StagingPesate;

/// <summary>
/// Implementazione del repository per STAGING_PESATE.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Step 3 deduplicazione
/// e Step 4 Persistenza atomica, STAGING_PESATE.</para>
/// </summary>
public sealed class StagingPesateRepository : BaseDALOperazioniZoo, IStagingPesateRepository
{
    public StagingPesateRepository(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    /// <inheritdoc/>
    public async Task<bool> SessioneEsistenteAsync(string idSessione, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT TOP 1 id");
        stbQuery.AppendLine("FROM STAGING_PESATE (NOLOCK)");
        stbQuery.AppendLine("WHERE id_sessione = @idSessione");
        stbQuery.AppendLine("    AND flag_validazione <> 'ERRORE_TOTALE'");
        parSql.Add("@idSessione", idSessione);

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            return dt.Rows.Count > 0;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> InsertAsync(StagingPesataRecord record, AgronicaCoreParametriServer objParametriServer)
    {
        var sb = new StringBuilder();
        var parameters = new ExpandoObject();

        sb.AppendLine("INSERT INTO STAGING_PESATE");
        sb.AppendLine("    (id_sessione, timestamp_inizio_sessione, payload_json, id_farm, flag_validazione, errore_descrizione, processato)");
        sb.AppendLine("VALUES");
        sb.AppendLine("    (@idSessione, @timestampInizioSessione, @payloadJson, @idFarm, @flagValidazione, @erroreDescrizione, 0)");

        parameters.TryAdd("@idSessione", record.IdSessione);
        parameters.TryAdd("@timestampInizioSessione", record.TimestampInizioSessione);
        parameters.TryAdd("@payloadJson", record.PayloadJson);
        parameters.TryAdd("@flagValidazione", record.FlagValidazione);
        parameters.TryAdd("@erroreDescrizione", (object?)record.ErroreDescrizione ?? DBNull.Value);
        parameters.TryAdd("@idFarm", record.IdFarm);

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

    /// <inheritdoc/>
    public async Task<DataTable> GetPesateNonProcessateAsync(int batchSize, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT TOP (@batchSize)");
        stbQuery.AppendLine("    id, id_sessione, payload_json, timestamp_inizio_sessione, id_farm");
        stbQuery.AppendLine("FROM STAGING_PESATE (NOLOCK)");
        stbQuery.AppendLine("WHERE processato = 0");
        stbQuery.AppendLine("    AND flag_validazione <> 'ERRORE_TOTALE'");
        stbQuery.AppendLine("ORDER BY timestamp_inizio_sessione ASC");
        parSql.Add("@batchSize", batchSize);

        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateProcessatoAsync(long id, int idAgenda, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var expandoObj = new ExpandoObject();

        stbQuery.AppendLine("UPDATE STAGING_PESATE");
        stbQuery.AppendLine("SET");
        stbQuery.AppendLine("    processato = 1,");
        stbQuery.AppendLine("    timestamp_processamento = GETDATE(),");
        stbQuery.AppendLine("    id_agenda = @idAgenda");
        stbQuery.AppendLine("WHERE id = @id");

        expandoObj.TryAdd("@id", id);
        expandoObj.TryAdd("@idAgenda", idAgenda);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateErroreAsync(long id, string erroreProcessamento, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var expandoObj = new ExpandoObject();

        stbQuery.AppendLine("UPDATE STAGING_PESATE");
        stbQuery.AppendLine("SET");
        stbQuery.AppendLine("    errore_processamento = @erroreProcessamento");
        stbQuery.AppendLine("WHERE id = @id");

        expandoObj.TryAdd("@id", id);
        expandoObj.TryAdd("@erroreProcessamento", erroreProcessamento);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateProcessatoConAccorpamentoAsync(long id, int idAgenda, int numPesateAccorpate, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var expandoObj = new ExpandoObject();

        stbQuery.AppendLine("UPDATE STAGING_PESATE");
        stbQuery.AppendLine("SET");
        stbQuery.AppendLine("    processato = 1,");
        stbQuery.AppendLine("    timestamp_processamento = GETDATE(),");
        stbQuery.AppendLine("    id_agenda = @idAgenda,");
        stbQuery.AppendLine("    num_pesate_accorpate = @numPesateAccorpate");
        stbQuery.AppendLine("WHERE id = @id");

        expandoObj.TryAdd("@id", id);
        expandoObj.TryAdd("@idAgenda", idAgenda);
        expandoObj.TryAdd("@numPesateAccorpate", numPesateAccorpate);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
