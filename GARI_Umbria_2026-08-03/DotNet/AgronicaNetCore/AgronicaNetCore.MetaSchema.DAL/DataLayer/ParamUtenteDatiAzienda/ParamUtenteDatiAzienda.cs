using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ParamUtenteDatiAzienda;

/// <summary>
/// Data-access implementation for <c>app_param_utente_datiazienda_web2app</c>.
/// Ref: DS01-BL – Persistenze Coinvolte.
/// </summary>
public sealed class ParamUtenteDatiAzienda : BaseDALMetaschema, IParamUtenteDatiAzienda
{
    public ParamUtenteDatiAzienda(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer) { }

    /// <inheritdoc/>
    public async Task<DataTable> LeggiParametriUtenteDatiAziendaAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        // Ref: DS01-BL – Descrizione: lookup della tabella app_param_utente_datiazienda_web2app.
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine(
            "SELECT param_richiamo_api, param_permessi_utente, param_visibilita_utente, param_visibilita_azienda_utente"
        );
        stbQuery.AppendLine("FROM   app_param_utente_datiazienda_web2app");
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
    public async Task<DataTable> VerificaEsistenzaAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        // Ref: DS01-BL – Regole di Business: verifica esistenza record per username.
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT COUNT(*) AS cnt");
        stbQuery.AppendLine("FROM   app_param_utente_datiazienda_web2app");
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
        string paramVisibilitaAziende,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        // Ref: DS01-BL – Persistenze Coinvolte: insert del record utente dati azienda.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("INSERT INTO app_param_utente_datiazienda_web2app");
        stbQuery.AppendLine("    (username, param_richiamo_api, param_permessi_utente,");
        stbQuery.AppendLine("     param_visibilita_utente, param_visibilita_azienda_utente, timestamp_aggiornamento,");
        stbQuery.AppendLine("     Data_Creazione, Username_Creazione)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@username, @paramRichiamo, @paramPermessi,");
        stbQuery.AppendLine("     @paramVisibilita, @paramVisibilitaAziende, @timestamp,");
        stbQuery.AppendLine("     GETDATE(), @usernameCreazione)");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@username", username);
        expandoObj.TryAdd("@paramRichiamo", paramRichiamoApi);
        expandoObj.TryAdd("@paramPermessi", paramPermessiUtente);
        expandoObj.TryAdd("@paramVisibilita", paramVisibilitaUtente);
        expandoObj.TryAdd("@paramVisibilitaAziende", paramVisibilitaAziende);
        expandoObj.TryAdd("@timestamp", timestampAggiornamento);
        expandoObj.TryAdd("@usernameCreazione", objParametriServer.UtenteUsername);

        try
        {
            await GetDataProvider(objParametriServer)
                .Execute_WriteAsync(stbQuery.ToString(), expandoObj);
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
        string paramVisibilitaAziende,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        // Ref: DS01-BL – Persistenze Coinvolte: update del record utente dati azienda.
        var stbQuery = new StringBuilder();
        stbQuery.AppendLine("UPDATE app_param_utente_datiazienda_web2app");
        stbQuery.AppendLine("SET    param_richiamo_api       = @paramRichiamo,");
        stbQuery.AppendLine("       param_permessi_utente    = @paramPermessi,");
        stbQuery.AppendLine("       param_visibilita_utente  = @paramVisibilita,");
        stbQuery.AppendLine("       param_visibilita_azienda_utente = @paramVisibilitaAziende,");
        stbQuery.AppendLine("       timestamp_aggiornamento  = @timestamp,");
        stbQuery.AppendLine("       Data_Modifica            = GETDATE(),");
        stbQuery.AppendLine("       Username_Modifica        = @usernameModifica");
        stbQuery.AppendLine("WHERE  username = @username");

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@paramRichiamo", paramRichiamoApi);
        expandoObj.TryAdd("@paramPermessi", paramPermessiUtente);
        expandoObj.TryAdd("@paramVisibilita", paramVisibilitaUtente);
        expandoObj.TryAdd("@paramVisibilitaAziende", paramVisibilitaAziende);
        expandoObj.TryAdd("@timestamp", timestampAggiornamento);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UtenteUsername);
        expandoObj.TryAdd("@username", username);

        try
        {
            await GetDataProvider(objParametriServer)
                .Execute_WriteAsync(stbQuery.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
