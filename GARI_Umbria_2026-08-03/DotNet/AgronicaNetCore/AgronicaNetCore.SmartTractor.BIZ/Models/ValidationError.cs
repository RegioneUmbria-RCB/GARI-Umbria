namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents a single validation error from activity data validation.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the validation error code.
    /// </summary>
    public ValidationErrorCode ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the field name that caused the error.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a user-friendly, localized error message (max 80 characters).
    /// </summary>
    public string MessageSynthetic { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity level of this error.
    /// </summary>
    public ValidationErrorSeverity Severity { get; set; }
}
