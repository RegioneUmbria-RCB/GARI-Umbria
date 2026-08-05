namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Input model for the DS04-BL orchestrator service that coordinates reading
/// activity data, composing the Smart Tractor payload, and sending it to the provider.
///
/// Referenced in Design Specification:
/// DS04-BL - Gestore Invio e Transazione Smart Tractor (INPUT section)
/// </summary>
public class GestoreInvioRequest
{
    /// <summary>
    /// Gets or sets the Agronica activity integer primary key (<c>Ricetta_Cod</c>).
    /// Must be &gt; 0.
    /// </summary>
    public int AttivitaId { get; set; }

    /// <summary>
    /// Gets or sets the ricetta-operazione integer primary key (<c>Ricetta_Operazione_Cod</c>).
    /// Must be &gt; 0.
    /// </summary>
    public int RicettaOperazioneId { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the Smart Tractor provider.
    /// Must not be <see cref="Guid.Empty"/>.
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the authenticated user triggering the send.
    /// Used as <c>operatorCode</c> in the payload and for audit columns.
    /// </summary>
    public string CurrentUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the planned activity start date in ISO 8601 format (YYYY-MM-DD).
    /// Referenced in Design Specification: DS04-BL - Rule 10 (plannedPeriod).
    /// </summary>
    public string PlannedStartDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the planned activity end date in ISO 8601 format (YYYY-MM-DD).
    /// Referenced in Design Specification: DS04-BL - Rule 10 (plannedPeriod).
    /// </summary>
    public string PlannedEndDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether products should be included in the payload.
    /// Defaults to <c>true</c>.
    /// Referenced in Design Specification: DS04-BL - Rule 5 (include_products).
    /// </summary>
    public bool IncludeProducts { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether prescription maps should be included
    /// and pre-uploaded to the provider. Defaults to <c>true</c>.
    /// Referenced in Design Specification: DS04-BL - Rule 6 (include_prescription_maps).
    /// </summary>
    public bool IncludePrescriptionMaps { get; set; } = true;

    /// <summary>
    /// Gets or sets the optional subset of plant IDs to include.
    /// When empty, all plants for the activity are included.
    /// Referenced in Design Specification: DS04-BL - PHASE 2 (selected_plants_ids).
    /// </summary>
    public IReadOnlyList<string> SelectedPlantsIds { get; set; } = Array.Empty<string>();
}
