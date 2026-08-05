using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Read-only DAL contract for DS02 unified log queries on company-data synchronisation tables.
/// Ref: DS02-BL – Persistenze Coinvolte.
/// </summary>
public interface ILogSincronizzazioneDatiAzienda
{
    /// <summary>
    /// Returns the logical table names that have at least one matching entry in
    /// <c>Agronica_Log_Anagrafe</c> after <paramref name="timestampUltimaSincronizzazione"/>.
    /// Ref: DS02-BL – Query unificata su log.
    /// </summary>
    Task<DataTable> LeggiTabelleModificateLogAnagrafeAsync(
        IReadOnlyCollection<VerificaTimestampLogQueryItem> items,
        DateTime timestampUltimaSincronizzazione,
        string username,
        bool applyCompanyVisibility,
        bool applyCenterVisibility,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Returns the logical table names that have at least one matching entry in
    /// <c>Agronica_Log_Contatti</c> after <paramref name="timestampUltimaSincronizzazione"/>.
    /// Ref: DS02-BL – Query unificata su log.
    /// </summary>
    Task<DataTable> LeggiTabelleModificateLogContattiAsync(
        IReadOnlyCollection<VerificaTimestampLogQueryItem> items,
        DateTime timestampUltimaSincronizzazione,
        string username,
        bool applyCompanyVisibility,
        bool applyCenterVisibility,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer);
}
