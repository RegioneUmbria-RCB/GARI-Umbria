namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Input descriptor for a table participating in DS02 timestamp verification.
/// Ref: DS02-BL – Input: lista_tabelle.
/// </summary>
public sealed class VerificaTimestampTabellaInput
{
    /// <summary>Logical payload table name. Ref: DS02-BL – Input: nome_tabella.</summary>
    public string NomeTabella { get; }

    /// <summary>Log source to query, if available. Ref: DS02-BL – Input: log_source.</summary>
    public string? LogSource { get; }

    /// <summary>Optional log type filter. Ref: DS02-BL – Input: log_tipo_filter.</summary>
    public string? LogTipoFilter { get; }

    /// <summary>
    /// True when company visibility may need to be applied.
    /// Ref: DS02-BL – Input: visibility_company.
    /// </summary>
    public bool VisibilityCompany { get; }

    /// <summary>
    /// True when company-center visibility may need to be applied.
    /// Ref: DS02-BL – Input: visibility_company_center.
    /// </summary>
    public bool VisibilityCompanyCenter { get; }

    public VerificaTimestampTabellaInput(string nomeTabella)
        : this(nomeTabella, null, null, false, false) { }

    public VerificaTimestampTabellaInput(
        string nomeTabella,
        string? logSource,
        string? logTipoFilter,
        bool visibilityCompany,
        bool visibilityCompanyCenter)
    {
        NomeTabella = nomeTabella;
        LogSource = logSource;
        LogTipoFilter = logTipoFilter;
        VisibilityCompany = visibilityCompany;
        VisibilityCompanyCenter = visibilityCompanyCenter;
    }
}