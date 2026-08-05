namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents the request for activity data validation.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public class ActivityValidationRequest
{
    /// <summary>
    /// Gets or sets the activity ID.
    /// </summary>
    public int ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the activity date.
    /// </summary>
    public DateTime ActivityDate { get; set; }

    /// <summary>
    /// Gets or sets the list of plant IDs associated with the activity.
    /// </summary>
    public int[]? Plants { get; set; }

    /// <summary>
    /// Gets or sets the polygon SRID (if provided instead of plants).
    /// </summary>
    public string? PolygonSrid { get; set; }

    /// <summary>
    /// Gets or sets the list of product IDs associated with the activity.
    /// </summary>
    public KeyValuePair<int, int>[]? Products { get; set; }

    /// <summary>
    /// Gets or sets the machine ID (UUID).
    /// </summary>
    public int? MachineId { get; set; }

    /// <summary>
    /// Gets or sets the provider ID (UUID).
    /// </summary>
    public string? ProviderId { get; set; }
}
