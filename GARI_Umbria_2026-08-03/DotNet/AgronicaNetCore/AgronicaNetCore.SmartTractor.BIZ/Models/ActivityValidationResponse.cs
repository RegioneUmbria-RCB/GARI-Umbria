namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents the response from activity data validation.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public class ActivityValidationResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the list of validation errors encountered.
    /// </summary>
    public List<ValidationError> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Gets or sets the recommended actions to resolve validation errors.
    /// </summary>
    public List<string> RecommendedActions { get; set; } = new();
}
