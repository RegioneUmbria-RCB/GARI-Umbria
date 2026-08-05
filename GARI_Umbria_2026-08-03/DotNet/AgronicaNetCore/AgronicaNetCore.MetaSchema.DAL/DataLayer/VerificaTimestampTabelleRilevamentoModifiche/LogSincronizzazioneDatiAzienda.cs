using System.Data;
using System.Text;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Executes DS02 unified EXISTS queries against the anagrafe and contatti log tables.
/// Ref: DS02-BL – Descrizione; Persistenze Coinvolte.
/// </summary>
public sealed class LogSincronizzazioneDatiAzienda : BaseDALMetaschema, ILogSincronizzazioneDatiAzienda
{
    /// <summary>
    /// Tipo filters for which a UNION ALL on agronica_log_invio_chiamate must also be emitted.
    /// Ref: DS02-BL – Persistenze Coinvolte: invio_chiamate.
    /// </summary>
    private static readonly HashSet<string> _invioChiamateTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        TipoAgronicaLogAnagrafe.Imprese,
        TipoAgronicaLogAnagrafe.Progetti,
        TipoAgronicaLogAnagrafe.ParcoMacchine,
        TipoAgronicaLogAnagrafe.Campi,
    };

    public LogSincronizzazioneDatiAzienda(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer) { }

    /// <inheritdoc/>
    public Task<DataTable> LeggiTabelleModificateLogAnagrafeAsync(
        IReadOnlyCollection<VerificaTimestampLogQueryItem> items,
        DateTime timestampUltimaSincronizzazione,
        string username,
        bool applyCompanyVisibility,
        bool applyCenterVisibility,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer)
    {
        return ExecuteUnifiedQueryAsync(
            items,
            timestampUltimaSincronizzazione,
            username,
            applyCompanyVisibility,
            applyCenterVisibility,
            TabellaLog.AgronicaLogAnagrafe,
            includeTipoFilter: true,
            modalitaDemetra,
            piva,
            objParametriServer);
    }

    /// <inheritdoc/>
    public Task<DataTable> LeggiTabelleModificateLogContattiAsync(
        IReadOnlyCollection<VerificaTimestampLogQueryItem> items,
        DateTime timestampUltimaSincronizzazione,
        string username,
        bool applyCompanyVisibility,
        bool applyCenterVisibility,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer)
    {
        return ExecuteUnifiedQueryAsync(
            items,
            timestampUltimaSincronizzazione,
            username,
            applyCompanyVisibility,
            applyCenterVisibility,
            TabellaLog.AgronicaLogContatti,
            includeTipoFilter: false,
            modalitaDemetra,
            piva,
            objParametriServer);
    }

    private async Task<DataTable> ExecuteUnifiedQueryAsync(
        IReadOnlyCollection<VerificaTimestampLogQueryItem> items,
        DateTime timestampUltimaSincronizzazione,
        string username,
        bool applyCompanyVisibility,
        bool applyCenterVisibility,
        string logTableName,
        bool includeTipoFilter,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer)
    {
        if (items.Count == 0)
        {
            return new DataTable();
        }

        var queryBuilder = new StringBuilder();
        var parameters = new Dictionary<string, object>
        {
            ["@timestampUltima"] = timestampUltimaSincronizzazione,
            ["@username"] = username,
            ["@pivaSuperUser"] = objParametriServer.PivaSuperUser,
            ["@piva"] = piva,
            ["@entitaImpresa"] = (int)Enum_TipoEntita.Impresa,
            ["@entitaCentro"] = (int)Enum_TipoEntita.Centro,
        };

        string logTemporaryTableName = "#LOG_TT_" + Guid.NewGuid().ToString().Replace("-", "_");
        string? utentiVisibilitaImpresaTemporaryTableName = applyCompanyVisibility
            ? "#UVA_IMP_" + Guid.NewGuid().ToString().Replace("-", "_")
            : null;
        string? utentiVisibilitaCentroTemporaryTableName = applyCenterVisibility
            ? "#UVA_CEN_" + Guid.NewGuid().ToString().Replace("-", "_")
            : null;
        string? invioChiamateTemporaryTableName = modalitaDemetra
            ? "#ALIC_TT_" + Guid.NewGuid().ToString().Replace("-", "_")
            : null;
        string? fornitoriPubbliciMovimentatiTemporaryTableName = !includeTipoFilter
            ? "#ALAUO_TT_" + Guid.NewGuid().ToString().Replace("-", "_")
            : null;

        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
        if (utentiVisibilitaImpresaTemporaryTableName != null)
        {
            queryBuilder.AppendLine("SELECT Piva, Sa_Cod");
            queryBuilder.AppendLine($"INTO {utentiVisibilitaImpresaTemporaryTableName}");
            queryBuilder.AppendLine("FROM Utenti_Visibilita_Appoggio uva");
            queryBuilder.AppendLine("WHERE uva.Entita_Cod = @entitaImpresa");
            queryBuilder.AppendLine("  AND uva.Username = @username");
            queryBuilder.AppendLine("  AND uva.PivaSuperUser = @pivaSuperUser");
            queryBuilder.AppendLine($"CREATE NONCLUSTERED INDEX IX_UVA_IMP_Piva_SaCod_{Guid.NewGuid().ToString().Replace("-", "_")} ON {utentiVisibilitaImpresaTemporaryTableName} (Piva, Sa_Cod)");
        }

        if (utentiVisibilitaCentroTemporaryTableName != null)
        {
            queryBuilder.AppendLine("SELECT Piva, Sa_Cod");
            queryBuilder.AppendLine($"INTO {utentiVisibilitaCentroTemporaryTableName}");
            queryBuilder.AppendLine("FROM Utenti_Visibilita_Appoggio uva");
            queryBuilder.AppendLine("WHERE uva.Entita_Cod = @entitaCentro");
            queryBuilder.AppendLine("  AND uva.Username = @username");
            queryBuilder.AppendLine("  AND uva.PivaSuperUser = @pivaSuperUser");
            queryBuilder.AppendLine($"CREATE NONCLUSTERED INDEX IX_UVA_CEN_Piva_SaCod_{Guid.NewGuid().ToString().Replace("-", "_")} ON {utentiVisibilitaCentroTemporaryTableName} (Piva, Sa_Cod)");
        }

        if (logTableName == TabellaLog.AgronicaLogAnagrafe)
            queryBuilder.AppendLine($"SELECT Param1, Param2, Param3, Tipo");
        else
            queryBuilder.AppendLine($"SELECT piva, Sa_Cod, 'Contatti' as tipo");
        queryBuilder.AppendLine($"INTO {logTemporaryTableName}");
        if (logTableName == TabellaLog.AgronicaLogAnagrafe)
            queryBuilder.AppendLine($"FROM Agronica_Log_Anagrafe log");
        else
            queryBuilder.AppendLine($"FROM Agronica_Log_Contatti log");
        queryBuilder.AppendLine($"WHERE log.Data_Ora_RegistrazioneLog >= @timestampUltima");
        if (logTableName == TabellaLog.AgronicaLogAnagrafe)
            queryBuilder.AppendLine("  AND log.Param1 = @piva");
        else
            queryBuilder.AppendLine("  AND (log.Piva = @piva OR log.sa_cod = -1)");
            
        if (logTableName == TabellaLog.AgronicaLogAnagrafe)
            queryBuilder.AppendLine($"CREATE NONCLUSTERED INDEX IX_LOG_TT_Param1_Tipo_{Guid.NewGuid().ToString().Replace("-", "_")} ON {logTemporaryTableName} (Param1, Tipo) INCLUDE (Param2, Param3)");
        else
            queryBuilder.AppendLine($"CREATE NONCLUSTERED INDEX IX_LOG_TT_Piva_{Guid.NewGuid().ToString().Replace("-", "_")} ON {logTemporaryTableName} (Piva) INCLUDE (Sa_Cod, Tipo)");

        if (invioChiamateTemporaryTableName != null)
        {
            queryBuilder.AppendLine("SELECT dettaglio2 as Tipo");
            queryBuilder.AppendLine($"INTO {invioChiamateTemporaryTableName}");
            queryBuilder.AppendLine("FROM agronica_log_invio_chiamate alic");
            queryBuilder.AppendLine("WHERE alic.Esito = 'OK'");
            queryBuilder.AppendLine("  AND alic.tipo_esportazione = 36");
            queryBuilder.AppendLine("  AND alic.dettaglio1 = @piva");
            queryBuilder.AppendLine("  AND alic.Data_Modifica >= @timestampUltima");
            queryBuilder.AppendLine($"CREATE NONCLUSTERED INDEX IX_ALIC_TT_Tipo_{Guid.NewGuid().ToString().Replace("-", "_")} ON {invioChiamateTemporaryTableName} (Tipo)");
        }

        if (fornitoriPubbliciMovimentatiTemporaryTableName != null)
        {
            queryBuilder.AppendLine("SELECT TOP 1 1 AS HasData");
            queryBuilder.AppendLine($"INTO {fornitoriPubbliciMovimentatiTemporaryTableName}");
            queryBuilder.AppendLine("FROM Agronica_Log_Agenda_UltimaOperazione a");
            queryBuilder.AppendLine("INNER JOIN Movimenti m ON a.PIVA = m.PIVA AND a.Id_Agenda = m.Id_Agenda");
            queryBuilder.AppendLine("INNER JOIN Risorse_Umane ru ON m.Cod_RisUm = ru.Cod_RisUm");
            queryBuilder.AppendLine("INNER JOIN Rapporti_Contabili rc ON ru.Cod_Rapporto = rc.Cod_Rapporto");
            queryBuilder.AppendLine($"WHERE a.Lav_Cod IN ({LAV_COD.LAVCOD_BOLLA_RICEVUTA}, {LAV_COD.LAVCOD_BOLLA_EMESSA}, {LAV_COD.LAVCOD_FATTURA_EMESSA}, {LAV_COD.LAVCOD_FATTURA_RICEVUTA})");
            queryBuilder.AppendLine("  AND a.Data_Ora_RegistrazioneLog >= @timestampUltima");
            queryBuilder.AppendLine($"  AND m.Cau_Mov = '{CAU_MOV.CAU_REGISTRAZIONI}'");
            queryBuilder.AppendLine("  AND rc.Fornitore = 1");
            queryBuilder.AppendLine("  AND a.Piva = @piva");
            queryBuilder.AppendLine("  AND ru.Sa_Cod = -1");
        }

        int index = 0;
        foreach (VerificaTimestampLogQueryItem item in items)
        {
            if (index > 0)
            {
                queryBuilder.AppendLine("UNION ALL");
            }

            string nomeParametro = $"@nomeTabella_{index}";
            parameters[nomeParametro] = item.NomeTabella;
            string? threeCasePivaColumn = null;
            string? threeCaseSaCodExpression = null;

            queryBuilder.AppendLine($"SELECT {nomeParametro} AS nome_tabella");
            queryBuilder.AppendLine("WHERE EXISTS (");
            queryBuilder.AppendLine("    SELECT 1");
            queryBuilder.AppendLine($"    FROM {logTemporaryTableName} log");

            if (applyCenterVisibility)
            {
                if (includeTipoFilter)
                {
                    if (item.LogTipoFilter == TipoAgronicaLogAnagrafe.ParcoMacchine)
                    {
                        threeCasePivaColumn = "log.Param1";
                        threeCaseSaCodExpression = "TRY_CONVERT(int, log.Param2)";
                    }
                    else
                    {
                        queryBuilder.AppendLine($"    INNER JOIN {utentiVisibilitaCentroTemporaryTableName} uva ON uva.Piva = log.Param1");
                        // SA_COD source depends on entity type (see param mapping table)
                        switch (item.LogTipoFilter)
                        {
                            case TipoAgronicaLogAnagrafe.Imprese:
                            case TipoAgronicaLogAnagrafe.Squadre:
                                // No SA_COD for these entities
                                break;
                            case TipoAgronicaLogAnagrafe.Progetti:
                                queryBuilder.AppendLine("        AND uva.Sa_Cod = TRY_CONVERT(int, log.Param3)");
                                break;
                            default:
                                queryBuilder.AppendLine("        AND uva.Sa_Cod = TRY_CONVERT(int, log.Param2)");
                                break;
                        }
                    }
                }
                else
                {
                    threeCasePivaColumn = "log.Piva";
                    threeCaseSaCodExpression = "log.Sa_Cod";
                }
            }
            else if (applyCompanyVisibility)
            {
                if (includeTipoFilter)
                {
                    queryBuilder.AppendLine($"    INNER JOIN {utentiVisibilitaImpresaTemporaryTableName} uva ON uva.Piva = log.Param1");
                }
                else
                {
                    queryBuilder.AppendLine($"    INNER JOIN {utentiVisibilitaImpresaTemporaryTableName} uva ON uva.Piva = log.Piva");
                }
            }

            queryBuilder.AppendLine($"    WHERE {(includeTipoFilter ? "log.Param1" : "log.Piva")} = @piva");

            if (threeCasePivaColumn != null)
            {
                AppendThreeCaseVisibilityCondition(
                    queryBuilder,
                    threeCasePivaColumn,
                    threeCaseSaCodExpression!,
                    utentiVisibilitaCentroTemporaryTableName!);
            }

            if (includeTipoFilter)
            {
                string tipoParametro = $"@tipo_{index}";
                parameters[tipoParametro] = item.LogTipoFilter ?? string.Empty;
                queryBuilder.AppendLine($"      AND log.Tipo = {tipoParametro}");
            }

            queryBuilder.AppendLine(")");

            // Emit an additional UNION ALL on agronica_log_invio_chiamate for applicable types.
            // Applies to: Imprese, Imprese_Progetti, Parco_Macchine, Campi (anagrafe path)
            // and all items on the contatti path (includeTipoFilter=false).
            // Only in Demetra mode (modalitaDemetra=true).
            bool needsInvioChiamateUnion = modalitaDemetra
                && (!includeTipoFilter
                    || (item.LogTipoFilter != null && _invioChiamateTypes.Contains(item.LogTipoFilter)));

            if (needsInvioChiamateUnion)
            {
                string tipoInvioParametro = $"@tipoInvio_{index}";
                parameters[tipoInvioParametro] = item.LogTipoFilter ?? item.NomeTabella;
                AppendInvioChiamateExistsBlock(
                    queryBuilder,
                    nomeParametro,
                    tipoInvioParametro,
                    applyCompanyVisibility,
                    applyCenterVisibility,
                    invioChiamateTemporaryTableName!,
                    utentiVisibilitaImpresaTemporaryTableName,
                    utentiVisibilitaCentroTemporaryTableName);
            }

            if (fornitoriPubbliciMovimentatiTemporaryTableName != null)
            {
                queryBuilder.AppendLine("UNION ALL");
                queryBuilder.AppendLine($"SELECT {nomeParametro} AS nome_tabella");
                queryBuilder.AppendLine("WHERE EXISTS (");
                queryBuilder.AppendLine($"    SELECT 1 FROM {fornitoriPubbliciMovimentatiTemporaryTableName}");
                queryBuilder.AppendLine(")");
            }

            index++;
        }

        if (utentiVisibilitaImpresaTemporaryTableName != null)
        {
            queryBuilder.AppendLine($"DROP TABLE {utentiVisibilitaImpresaTemporaryTableName}");
        }

        if (utentiVisibilitaCentroTemporaryTableName != null)
        {
            queryBuilder.AppendLine($"DROP TABLE {utentiVisibilitaCentroTemporaryTableName}");
        }

        if (invioChiamateTemporaryTableName != null)
        {
            queryBuilder.AppendLine($"DROP TABLE {invioChiamateTemporaryTableName}");
        }

        if (fornitoriPubbliciMovimentatiTemporaryTableName != null)
        {
            queryBuilder.AppendLine($"DROP TABLE {fornitoriPubbliciMovimentatiTemporaryTableName}");
        }

        queryBuilder.AppendLine($"DROP TABLE {logTemporaryTableName}");

        try
        {
            return await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(queryBuilder.ToString(), parameters, parSqlIn);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    /// <summary>
    /// Appends a UNION ALL block that checks agronica_log_invio_chiamate for successful export records
    /// matching the item's tipo filter (dettaglio2 LIKE) and the user's PIVA visibility (dettaglio1).
    /// Ref: DS02-BL – Persistenze Coinvolte: invio_chiamate.
    /// </summary>
    private static void AppendInvioChiamateExistsBlock(
        StringBuilder queryBuilder,
        string nomeParametro,
        string tipoParametro,
        bool applyCompanyVisibility,
        bool applyCenterVisibility,
        string invioChiamateTemporaryTableName,
        string? utentiVisibilitaImpresaTemporaryTableName,
        string? utentiVisibilitaCentroTemporaryTableName)
    {
        queryBuilder.AppendLine("UNION ALL");
        queryBuilder.AppendLine($"SELECT {nomeParametro} AS nome_tabella");
        queryBuilder.AppendLine("WHERE EXISTS (");
        queryBuilder.AppendLine("    SELECT 1");
        queryBuilder.AppendLine($"    FROM {invioChiamateTemporaryTableName} alic");

        if (applyCenterVisibility)
        {
            queryBuilder.AppendLine($"    WHERE EXISTS (SELECT 1 FROM {utentiVisibilitaCentroTemporaryTableName} uva WHERE uva.Piva = @piva)");
            queryBuilder.AppendLine("      AND (");
        }
        else if (applyCompanyVisibility)
        {
            queryBuilder.AppendLine($"    WHERE EXISTS (SELECT 1 FROM {utentiVisibilitaImpresaTemporaryTableName} uva WHERE uva.Piva = @piva)");
            queryBuilder.AppendLine("      AND (");
        }
        else
        {
            queryBuilder.AppendLine("    WHERE (");
        }

        queryBuilder.AppendLine($"        alic.Tipo = {tipoParametro}");
        queryBuilder.AppendLine($"        OR alic.Tipo LIKE {tipoParametro} + '|%'");
        queryBuilder.AppendLine($"        OR alic.Tipo LIKE '%|' + {tipoParametro} + '|%'");
        queryBuilder.AppendLine($"        OR alic.Tipo LIKE '%|' + {tipoParametro}");
        queryBuilder.AppendLine("      )");
        queryBuilder.AppendLine(")");
    }

    /// <summary>
    /// Appends a three-case center-visibility WHERE condition using a nested EXISTS on Utenti_Visibilita_Appoggio.
    /// Moving visibility to WHERE (instead of JOIN ON) avoids cross-join risk and allows index seeks on UVA.
    /// Cases: 1. exact center (sa_cod match), 2. all centers of company (sa_cod = 0), 3. public (sa_cod = -1).
    /// </summary>
    /// <param name="pivaColumn">SQL expression for the PIVA column in the log table (e.g. "log.Piva" or "log.Param1").</param>
    /// <param name="saCodExpression">SQL expression for the SA_COD value (e.g. "log.Sa_Cod" or "TRY_CONVERT(int, log.Param2)").</param>
    private static void AppendThreeCaseVisibilityCondition(
        StringBuilder queryBuilder,
        string pivaColumn,
        string saCodExpression,
        string utentiVisibilitaCentroTemporaryTableName)
    {
        queryBuilder.AppendLine($"      AND ({saCodExpression} = -1");
        queryBuilder.AppendLine($"            OR EXISTS (");
        queryBuilder.AppendLine($"                SELECT 1 FROM {utentiVisibilitaCentroTemporaryTableName} uva");
        queryBuilder.AppendLine($"                WHERE uva.Piva = {pivaColumn}");
        queryBuilder.AppendLine($"                  AND (uva.Sa_Cod = {saCodExpression} OR {saCodExpression} = 0)");
        queryBuilder.AppendLine($"            )");
        queryBuilder.AppendLine($"           )");
    }
}
