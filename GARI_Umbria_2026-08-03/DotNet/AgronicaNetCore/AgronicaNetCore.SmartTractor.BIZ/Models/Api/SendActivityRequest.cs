using System.ComponentModel.DataAnnotations;

namespace AgronicaNetCore.SmartTractor.BIZ.Models.Api;

/// <summary>
/// Allowed values for the <see cref="SendActivityRequest.SendMode"/> field.
/// Referenced in Design Specification: DS04-BLb - Schema Richiesta.
/// </summary>
public static class SendMode
{
    /// <summary>Quick send — minimal composition, no prescription-map upload.</summary>
    public const string Quick = "quick";

    /// <summary>Full send — complete payload with products and prescription maps.</summary>
    public const string Full = "full";
}

/// <summary>
/// HTTP request body for the <c>POST /api/smarttractor/send-activity</c> endpoint.
///
/// All UUID fields are validated with <see cref="System.Guid"/> parsing before the
/// orchestrator service is invoked.
/// Referenced in Design Specification: DS04-BLb - Schema Richiesta.
/// </summary>
public class SendActivityRequest
{
    /// <summary>
    /// Gets or sets the Agronica activity identifier (<c>Ricetta_Cod</c>).
    /// Must be a valid integer value greater than 0.
    /// </summary>
    [Required]
    public int ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the ricetta-operazione identifier (<c>Ricetta_Operazione_Cod</c>).
    /// Must be a valid integer value greater than 0.
    /// </summary>
    [Required]
    public int RicettaOperazioneId { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the Smart Tractor provider (e.g. CNH, CLAAS).
    /// Referenced in Design Specification: DS04-BLb - Validazioni Input.
    /// </summary>
    [Required]
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the planned activity start date in ISO 8601 format (YYYY-MM-DD).
    /// Referenced in Design Specification: DS04-BLb - Schema Richiesta.
    /// </summary>
    [Required]
    public string PlannedStartDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the planned activity end date in ISO 8601 format (YYYY-MM-DD).
    /// Referenced in Design Specification: DS04-BLb - Schema Richiesta.
    /// </summary>
    [Required]
    public string PlannedEndDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether products should be included in the payload.
    /// Defaults to <c>true</c>.
    /// Referenced in Design Specification: DS04-BLb - Validazioni Input.
    /// </summary>
    public bool IncludeProducts { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the prescription map should be included
    /// and pre-uploaded to the provider.  Defaults to <c>true</c>.
    /// Referenced in Design Specification: DS04-BLb - Validazioni Input.
    /// </summary>
    public bool IncludePrescriptionMap { get; set; } = true;

    /// <summary>
    /// Gets or sets the optional subset of plant IDs to include.
    /// When empty all plants for the activity are included.
    /// Referenced in Design Specification: DS04-BLb - Schema Richiesta.
    /// </summary>
    public IReadOnlyList<string> SelectedPlantsIds { get; set; } = Array.Empty<string>();
}
