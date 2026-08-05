namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Single logical table to evaluate inside a DS02 unified log query.
/// Ref: DS02-BL – Input: lista_tabelle.
/// </summary>
public sealed class VerificaTimestampLogQueryItem
{
    /// <summary>Logical payload table name. Ref: DS02-BL – Input: nome_tabella.</summary>
    public string NomeTabella { get; }

    /// <summary>Optional log type filter. Ref: DS02-BL – Input: log_tipo_filter.</summary>
    public string? LogTipoFilter { get; }

    public VerificaTimestampLogQueryItem(string nomeTabella, string? logTipoFilter)
    {
        NomeTabella = nomeTabella;
        LogTipoFilter = logTipoFilter;
    }
}
