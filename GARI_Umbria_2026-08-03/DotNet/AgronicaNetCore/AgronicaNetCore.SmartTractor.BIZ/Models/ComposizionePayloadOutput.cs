namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Output model returned by the payload composition service.
/// Contains the composed payload, its unique request ID, size metadata, and
/// schema validation result.
/// Referenced in Design Specification: DS03-BL - Composizione Payload (OUTPUT section, PHASE 12)
/// </summary>
public class ComposizionePayloadOutput
{
    public int RicettaOperazioneCod {  get; set; }
    public int MacCod {  get; set; }
    /// <summary>
    /// Gets or sets the fully composed Smart Tractor payload ready for submission.
    /// Referenced in Design Specification: DS03-BL - Rule 13.
    /// </summary>
    public SmartTractorPayload Payload { get; set; } = null!;

    /// <summary>
    /// Gets or sets the serialized payload size in bytes.
    /// Referenced in Design Specification: DS03-BL - Rule 12.
    /// </summary>
    public long PayloadSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the result of the schema validation performed against the
    /// Smart Tractor API contract.
    /// Referenced in Design Specification: DS03-BL - Rule 11.
    /// </summary>
    public SchemaValidationResult SchemaValidation { get; set; } = new();
}

/// <summary>
/// Represents the schema validation outcome for a composed payload.
/// Referenced in Design Specification: DS03-BL - Rule 11.
/// </summary>
public class SchemaValidationResult
{
    /// <summary>Gets or sets a value indicating whether the payload passed schema validation.</summary>
    public bool IsValid { get; set; }

    /// <summary>Gets or sets the list of constraint violation messages, if any.</summary>
    public IReadOnlyList<string> Errors { get; set; } = Array.Empty<string>();
}
