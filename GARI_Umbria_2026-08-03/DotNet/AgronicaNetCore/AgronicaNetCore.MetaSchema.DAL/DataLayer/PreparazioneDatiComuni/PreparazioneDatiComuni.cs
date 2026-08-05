using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;

/// <summary>
/// Data-access implementation for <c>app_preparazione_daticomuni_web2app</c>.
/// Executes within the SERIALIZABLE transaction already opened by the BIZ layer.
/// Ref: DS05-BL – Pattern Framework: queryupdate con StringBuilder + ExpandoObject.
/// Ref: DS03-BL – Persistenze Coinvolte: lettura json precedente per confronto.
/// </summary>
public sealed class PreparazioneDatiComuni : BaseDALMetaschema, IAggiornamentoAtomicoPreparazione, IPreparazioneDatiComuniRead, ITimestampAggiornamentoDatiComuni, IAggregazioneJSONDatiComuni
{
    public PreparazioneDatiComuni(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer) { }

    /// <inheritdoc/>
    public async Task<(bool Exists, string? JsonContent)> LeggiJsonContentAsync(
        string nomeTabella,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS03-BL – Regole di Business: Fase 1 – SELECT json_content WHERE nome_tabella = @nomeTabella.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("SELECT json_content");
        stbQuery.AppendLine("FROM   app_preparazione_daticomuni_web2app (NOLOCK)");
        stbQuery.AppendLine("WHERE  nome_tabella = @nomeTabella");

        var sqlParams = new Dictionary<string, object> { { "@nomeTabella", nomeTabella } };

        try
        {
            DataTable result = await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stbQuery.ToString(), sqlParams);

            if (result.Rows.Count == 0)
                return (false, null);

            string? jsonContent = result.Rows[0]["json_content"] == DBNull.Value
                ? null
                : result.Rows[0]["json_content"]?.ToString();
            return (true, jsonContent);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task InsertPreparazioneAsync(
        string nomeTabella,
        string jsonNuovo,
        DateTime timestampGenerazione,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS03-BL – Nota Tecnica; DS05-BL – INSERT when exists = false (first execution).
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("INSERT INTO app_preparazione_daticomuni_web2app");
        stbQuery.AppendLine("    (nome_tabella, json_content, timestamp_aggiornamento,");
        stbQuery.AppendLine("     flag_aggiornamento_in_corso, Data_Creazione, Username_Creazione)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@nomeTabella, @json, @timestamp, @flag, GETDATE(), @username)");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@nomeTabella", nomeTabella);
        expandoObj.TryAdd("@json",        jsonNuovo);
        expandoObj.TryAdd("@timestamp",   timestampGenerazione);
        expandoObj.TryAdd("@flag",        0);
        expandoObj.TryAdd("@username",    "agronica_job");

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
    public async Task UpdatePreparazioneAsync(
        string nomeTabella,
        string jsonNuovo,
        DateTime timestampGenerazione,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS05-BL – Regole di Business: aggiorna colonne json_content, timestamp_aggiornamento,
        //      flag_aggiornamento_in_corso = 0, messaggio_ultima_estrazione = NULL,
        //      Data_Modifica = GETDATE(), Username_Modifica = 'agronica_job'.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("UPDATE app_preparazione_daticomuni_web2app");
        stbQuery.AppendLine("SET    json_content                  = @json,");
        stbQuery.AppendLine("       timestamp_aggiornamento       = @timestamp,");
        stbQuery.AppendLine("       flag_aggiornamento_in_corso   = @flag,");
        stbQuery.AppendLine("       messaggio_ultima_estrazione   = NULL,");
        stbQuery.AppendLine("       Data_Modifica                 = GETDATE(),");
        stbQuery.AppendLine("       Username_Modifica             = @username");
        stbQuery.AppendLine("WHERE  nome_tabella = @nomeTabella");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@json",        jsonNuovo);
        expandoObj.TryAdd("@timestamp",   timestampGenerazione);
        expandoObj.TryAdd("@flag",        0);
        expandoObj.TryAdd("@username",    "agronica_job");
        expandoObj.TryAdd("@nomeTabella", nomeTabella);

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
    /// <summary>
    /// Reads all rows from <c>app_preparazione_daticomuni_web2app</c> and returns a
    /// dictionary keyed by <c>nome_tabella</c>.
    /// Ref: DS03-BL – Persistenze Coinvolte: lettura json_content da app_preparazione_daticomuni_web2app.
    /// </summary>
    public async Task<Dictionary<string, string?>> LeggiJsonPrecedentiAsync(AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("SELECT nome_tabella,");
        stbQuery.AppendLine("       json_content");
        stbQuery.AppendLine("FROM   app_preparazione_daticomuni_web2app");

        var parSql = new Dictionary<string, object>();
        var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

        try
        {
            DataTable dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);

            var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in dt.Rows)
            {
                string? nomeTabella = row["nome_tabella"]?.ToString();
                string? jsonContent = row["json_content"] == DBNull.Value ? null : row["json_content"]?.ToString();
                if (nomeTabella is not null)
                    result[nomeTabella] = jsonContent;
            }

            return result;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DataTable> LeggiTimestampAggiornamentoAsync(
        List<string> tabelleRichieste,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS08-BL – Pattern Framework: LeggiTimestampAggiornamento.
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT MAX(timestamp_aggiornamento) AS max_timestamp");
        stbQuery.AppendLine("FROM   app_preparazione_daticomuni_web2app");
        stbQuery.AppendLine("WHERE  nome_tabella IN (@tabelleList)");

        parSqlIn.Add("@tabelleList", FormatClauseIn(tabelleRichieste));

        try
        {
            return await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DataTable> LeggiJSONTabelleomuniAsync(
        List<string> tabelleRichieste,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS09-BL – Pattern Framework: LeggiJSONTabelleComuni.
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT nome_tabella, json_content, timestamp_aggiornamento");
        stbQuery.AppendLine("FROM   app_preparazione_daticomuni_web2app");
        stbQuery.AppendLine("WHERE  nome_tabella IN (@tabelleList)");

        parSqlIn.Add("@tabelleList", FormatClauseIn(tabelleRichieste));

        try
        {
            return await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
