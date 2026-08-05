using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SemaforoDatiComuni;

/// <summary>
/// Data-access implementation for <c>app_semaforo_daticomuni_web2app</c>.
/// Ref: DS06-BL – Pattern Framework: queryinsert e queryupdate del TECH_FRAMEWORK.
/// </summary>
public sealed class SemaforoDatiComuni : BaseDALMetaschema, ISemaforoDatiComuni
{
    public SemaforoDatiComuni(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer) { }

    /// <inheritdoc/>
    public async Task<bool> VerificaSemaforoAttivoAsync(AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS06-BL – Regole di Business: verifica flag_aggiornamento_in_corso = 1
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("SELECT COUNT(*)");
        stbQuery.AppendLine("FROM   app_semaforo_daticomuni_web2app (NOLOCK)");
        stbQuery.AppendLine("WHERE  flag_aggiornamento_in_corso = 1");

        try
        {
            DataTable result = await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stbQuery.ToString(), new Dictionary<string, object>());
            return Convert.ToInt32(result.Rows[0][0]) > 0;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task InserisciSemaforoInizioAsync(DateTime timestampInizio, AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS06-BL – Regole di Business: INSERT con timestamp_inizio, flag_aggiornamento_in_corso = 1,
        //      Data_Creazione = GETDATE(), Username_Creazione = 'agronica_job'.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("INSERT INTO app_semaforo_daticomuni_web2app");
        stbQuery.AppendLine("    (timestamp_inizio, flag_aggiornamento_in_corso, Data_Creazione, Username_Creazione)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@timestampInizio, @flagInCorso, GETDATE(), @username)");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@timestampInizio", timestampInizio);
        expandoObj.TryAdd("@flagInCorso", 1);
        expandoObj.TryAdd("@username", objParametriServer.UtenteUsername);

        try
        {
            await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task AggiornaSemaforoFineAsync(
        DateTime timestampInizio,
        DateTime timestampFine,
        int durataSecondi,
        bool successo,
        string? messaggioStato,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS06-BL – Regole di Business: UPDATE con timestamp_fine, durata_secondi,
        //      flag_aggiornamento_in_corso = 0, flag_aggiornamento_andato_bene, messaggio_stato.
        //      Standard GIAS columns: Data_Modifica = GETDATE(), Username_Modifica = 'agronica_job'.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("UPDATE app_semaforo_daticomuni_web2app");
        stbQuery.AppendLine("SET    timestamp_fine                 = @timestampFine,");
        stbQuery.AppendLine("       durata_secondi                 = @durataSecondi,");
        stbQuery.AppendLine("       flag_aggiornamento_in_corso    = @flagInCorso,");
        stbQuery.AppendLine("       flag_aggiornamento_andato_bene = @flagSuccesso,");
        stbQuery.AppendLine("       messaggio_stato                = @messaggioStato,");
        stbQuery.AppendLine("       Data_Modifica                  = GETDATE(),");
        stbQuery.AppendLine("       Username_Modifica              = @username");
        stbQuery.AppendLine("WHERE  timestamp_inizio = @timestampInizio");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@timestampFine", timestampFine);
        expandoObj.TryAdd("@durataSecondi", durataSecondi);
        expandoObj.TryAdd("@flagInCorso", 0);
        expandoObj.TryAdd("@flagSuccesso", successo ? 1 : 0);
        expandoObj.TryAdd("@messaggioStato", (object?)messaggioStato ?? DBNull.Value);
        expandoObj.TryAdd("@username", objParametriServer.UtenteUsername);
        expandoObj.TryAdd("@timestampInizio", timestampInizio);

        try
        {
            await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DataTable> LeggiStatoSemaforoAsync(AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS11-BL – Descrizione: TOP 1 ORDER BY timestamp_inizio DESC
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT TOP 1");
        stbQuery.AppendLine("    flag_aggiornamento_in_corso,");
        stbQuery.AppendLine("    timestamp_inizio");
        stbQuery.AppendLine("FROM app_semaforo_daticomuni_web2app");
        stbQuery.AppendLine("ORDER BY timestamp_inizio DESC");

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
    public async Task<DataTable> CercaJobInTimeoutAsync(int timeoutMinuti, AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS12-BL – Regole di Business: flag=1 AND DATEDIFF(MINUTE, timestamp_inizio, GETUTCDATE()) > timeoutMinuti
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT TOP 1 id, timestamp_inizio");
        stbQuery.AppendLine("FROM app_semaforo_daticomuni_web2app");
        stbQuery.AppendLine("WHERE flag_aggiornamento_in_corso = @flagInCorso");
        stbQuery.AppendLine("  AND DATEDIFF(MINUTE, timestamp_inizio, GETUTCDATE()) > @timeoutMinuti");
        stbQuery.AppendLine("ORDER BY timestamp_inizio DESC");

        parSql.Add("@flagInCorso", 1);
        parSql.Add("@timeoutMinuti", timeoutMinuti);

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
    public async Task ResetSemaforoTimeoutAsync(int idRecord, DateTime timestampInizio, AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS12-BL – Regole di Business: UPDATE con flag=0, flag_bene=0, timestamp_fine, durata, messaggio, colonne GIAS
        var stbQuery = new StringBuilder();
        var timestampFine = DateTime.UtcNow;
        var durataSecondi = (int)(timestampFine - timestampInizio).TotalSeconds;

        stbQuery.AppendLine("UPDATE app_semaforo_daticomuni_web2app");
        stbQuery.AppendLine("SET    flag_aggiornamento_in_corso    = @flagInCorso,");
        stbQuery.AppendLine("       flag_aggiornamento_andato_bene = @flagSuccesso,");
        stbQuery.AppendLine("       timestamp_fine                 = @timestampFine,");
        stbQuery.AppendLine("       durata_secondi                 = @durataSecondi,");
        stbQuery.AppendLine("       messaggio_stato                = @messaggio,");
        stbQuery.AppendLine("       Data_Modifica                  = GETDATE(),");
        stbQuery.AppendLine("       Username_Modifica              = @username");
        stbQuery.AppendLine("WHERE  id = @id");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@flagInCorso",   0);
        expandoObj.TryAdd("@flagSuccesso",  0);
        expandoObj.TryAdd("@timestampFine", timestampFine);
        expandoObj.TryAdd("@durataSecondi", durataSecondi);
        expandoObj.TryAdd("@messaggio",     "Job timeout detected, semaforo reset automatico");
        expandoObj.TryAdd("@username",      "agronica_watchdog");
        expandoObj.TryAdd("@id",            idRecord);

        try
        {
            await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
