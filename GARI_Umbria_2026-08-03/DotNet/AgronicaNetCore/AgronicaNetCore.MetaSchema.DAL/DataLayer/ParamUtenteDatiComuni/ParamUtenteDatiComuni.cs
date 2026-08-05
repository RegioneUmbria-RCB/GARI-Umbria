using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ParamUtenteDatiComuni;

/// <summary>
/// Data-access implementation for <c>app_param_utente_daticomuni_web2app</c>.
/// Ref: DS07-BL – Pattern Framework: querylettura del TECH_FRAMEWORK.
/// Ref: DS10-BL – Pattern Framework: queryinsert e queryupdate del TECH_FRAMEWORK.
/// </summary>
public sealed class ParamUtenteDatiComuni : BaseDALMetaschema, IParamUtenteDatiComuni
{

    public ParamUtenteDatiComuni(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer) { }

    /// <inheritdoc/>
    public async Task<DataTable> LeggiParametriUtenteAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS07-BL – Implementazione recupero parametri memorizzati.
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT param_richiamo_api, param_permessi_utente, param_visibilita_utente");
        stbQuery.AppendLine("FROM   app_param_utente_daticomuni_web2app");
        stbQuery.AppendLine("WHERE  username = @username");

        parSql.Add("@username", objParametriServer.UtenteUsername);

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
    public async Task<DataTable> VerificaEsistenzaAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS10-BL – Pattern Framework: VerificaEsistenzaParametri.
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT COUNT(*) AS cnt");
        stbQuery.AppendLine("FROM   app_param_utente_daticomuni_web2app");
        stbQuery.AppendLine("WHERE  username = @username");

        parSql.Add("@username", username);

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
    public async Task InsertAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS10-BL – Pattern Framework: InserisciParametriUtente.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("INSERT INTO app_param_utente_daticomuni_web2app");
        stbQuery.AppendLine("    (username, param_richiamo_api, param_permessi_utente,");
        stbQuery.AppendLine("     param_visibilita_utente, timestamp_aggiornamento,");
        stbQuery.AppendLine("     Data_Creazione, Username_Creazione)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@username, @paramRichiamo, @paramPermessi,");
        stbQuery.AppendLine("     @paramVisibilita, @timestamp,");
        stbQuery.AppendLine("     GETDATE(), @usernameCreazione)");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@username",           username);
        expandoObj.TryAdd("@paramRichiamo",      paramRichiamoApi);
        expandoObj.TryAdd("@paramPermessi",      paramPermessiUtente);
        expandoObj.TryAdd("@paramVisibilita",    paramVisibilitaUtente);
        expandoObj.TryAdd("@timestamp",          timestampAggiornamento);
        expandoObj.TryAdd("@usernameCreazione",  objParametriServer.UtenteUsername);

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
    public async Task UpdateAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS10-BL – Pattern Framework: AggiornaParametriUtente.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("UPDATE app_param_utente_daticomuni_web2app");
        stbQuery.AppendLine("SET    param_richiamo_api       = @paramRichiamo,");
        stbQuery.AppendLine("       param_permessi_utente    = @paramPermessi,");
        stbQuery.AppendLine("       param_visibilita_utente  = @paramVisibilita,");
        stbQuery.AppendLine("       timestamp_aggiornamento  = @timestamp,");
        stbQuery.AppendLine("       Data_Modifica            = GETDATE(),");
        stbQuery.AppendLine("       Username_Modifica        = @usernameModifica");
        stbQuery.AppendLine("WHERE  username = @username");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@paramRichiamo",    paramRichiamoApi);
        expandoObj.TryAdd("@paramPermessi",    paramPermessiUtente);
        expandoObj.TryAdd("@paramVisibilita",  paramVisibilitaUtente);
        expandoObj.TryAdd("@timestamp",        timestampAggiornamento);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UtenteUsername);
        expandoObj.TryAdd("@username",         username);

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
