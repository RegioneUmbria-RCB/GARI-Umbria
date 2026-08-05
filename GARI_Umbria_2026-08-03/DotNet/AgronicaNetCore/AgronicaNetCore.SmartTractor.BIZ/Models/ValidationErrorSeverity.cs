namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Enumeration of error severity levels.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public enum ValidationErrorSeverity
{
    /// <summary>
    /// Error that blocks the activity submission.
    /// </summary>
    Blocking,

    /// <summary>
    /// Warning that does not block but indicates a potential issue.
    /// </summary>
    Warning
}
