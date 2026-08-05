namespace AgronicaNetCore.SmartTractor.BIZ.Exceptions;

/// <summary>
/// Exception thrown when an activity is not found.
/// </summary>
public class ActivityNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityNotFoundException"/> class.
    /// </summary>
    /// <param name="activityId">The activity ID that was not found.</param>
    public ActivityNotFoundException(string activityId)
        : base($"Activity with ID '{activityId}' was not found.")
    {
        ActivityId = activityId;
    }

    /// <summary>
    /// Gets the activity ID that was not found.
    /// </summary>
    public string ActivityId { get; }
}

/// <summary>
/// Exception thrown when a machine is not found.
/// </summary>
public class MachineNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MachineNotFoundException"/> class.
    /// </summary>
    /// <param name="machineId">The machine ID that was not found.</param>
    public MachineNotFoundException(string machineId)
        : base($"Machine with ID '{machineId}' was not found.")
    {
        MachineId = machineId;
    }

    /// <summary>
    /// Gets the machine ID that was not found.
    /// </summary>
    public string MachineId { get; }
}

/// <summary>
/// Exception thrown when provider requirements are not found.
/// </summary>
public class ProviderRequirementsNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderRequirementsNotFoundException"/> class.
    /// </summary>
    /// <param name="providerId">The provider ID for which requirements were not found.</param>
    public ProviderRequirementsNotFoundException(string providerId)
        : base($"Requirements for provider '{providerId}' were not found.")
    {
        ProviderId = providerId;
    }

    /// <summary>
    /// Gets the provider ID.
    /// </summary>
    public string ProviderId { get; }
}

/// <summary>
/// Exception thrown when geometry/polygon is invalid.
/// </summary>
public class InvalidGeometryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGeometryException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidGeometryException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when a mandatory field is missing or invalid.
/// </summary>
public class MissingMandatoryFieldException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingMandatoryFieldException"/> class.
    /// </summary>
    /// <param name="fieldName">The name of the missing field.</param>
    public MissingMandatoryFieldException(string fieldName)
        : base($"Mandatory field '{fieldName}' is missing or invalid.")
    {
        FieldName = fieldName;
    }

    /// <summary>
    /// Gets the name of the missing field.
    /// </summary>
    public string FieldName { get; }
}

/// <summary>
/// Exception thrown when a provider is not found.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public class ProviderNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderNotFoundException"/> class.
    /// </summary>
    /// <param name="providerId">The provider ID that was not found.</param>
    public ProviderNotFoundException(string providerId)
        : base($"Provider with ID '{providerId}' was not found.")
    {
        ProviderId = providerId;
    }

    /// <summary>
    /// Gets the provider ID that was not found.
    /// </summary>
    public string ProviderId { get; }
}

/// <summary>
/// Exception thrown when a required mapping is not found in the provider mapping tables.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public class MissingMappingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingMappingException"/> class.
    /// </summary>
    /// <param name="mappingType">The type of mapping (plant, machine, product, etc.).</param>
    /// <param name="entityId">The ID of the entity that is not mapped.</param>
    /// <param name="providerId">The provider ID for which mapping is missing.</param>
    public MissingMappingException(string mappingType, string entityId, string providerId)
        : base($"Mapping not found: {mappingType} '{entityId}' is not mapped with provider '{providerId}'.")
    {
        MappingType = mappingType;
        EntityId = entityId;
        ProviderId = providerId;
    }

    /// <summary>
    /// Gets the type of mapping (plant, machine, product, etc.).
    /// </summary>
    public string MappingType { get; }

    /// <summary>
    /// Gets the ID of the entity that is not mapped.
    /// </summary>
    public string EntityId { get; }

    /// <summary>
    /// Gets the provider ID for which mapping is missing.
    /// </summary>
    public string ProviderId { get; }
}

// ─── DS03-BLb exceptions ────────────────────────────────────────────────────

/// <summary>
/// Exception thrown when an Operazione entity is not found.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività
/// </summary>
public class OperazioneNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperazioneNotFoundException"/> class.
    /// </summary>
    /// <param name="operazioneId">The operazione integer ID that was not found.</param>
    public OperazioneNotFoundException(int operazioneId)
        : base($"Operazione correlata non trovata per id='{operazioneId}'.")
    {
        OperazioneId = operazioneId;
    }

    /// <summary>Gets the operazione ID that was not found.</summary>
    public int OperazioneId { get; }
}

/// <summary>
/// Exception thrown when a RicettaOperazione entity is not found.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività
/// </summary>
public class RicettaOperazioneNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RicettaOperazioneNotFoundException"/> class.
    /// </summary>
    /// <param name="ricettaOperazioneId">The Ricetta_Operazione_Cod (int PK) that was not found.</param>
    public RicettaOperazioneNotFoundException(int ricettaOperazioneId)
        : base($"Ricetta operazione non trovata per id='{ricettaOperazioneId}'.")
    {
        RicettaOperazioneId = ricettaOperazioneId;
    }

    /// <summary>Gets the ricetta-operazione integer ID that was not found.</summary>
    public int RicettaOperazioneId { get; }
}

/// <summary>
/// Exception thrown when a Ricetta entity is not found.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività
/// </summary>
public class RicettaNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RicettaNotFoundException"/> class.
    /// </summary>
    /// <param name="ricettaId">The Ricetta_Cod (int PK) that was not found.</param>
    public RicettaNotFoundException(int ricettaId)
        : base($"Ricetta non trovata per id='{ricettaId}'.")
    {
        RicettaId = ricettaId;
    }

    /// <summary>Gets the ricetta integer ID that was not found.</summary>
    public int RicettaId { get; }
}

/// <summary>
/// Exception thrown when a plant or product is not mapped for a given provider.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 7, FASE 8)
/// </summary>
public class MissingProviderMappingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingProviderMappingException"/> class.
    /// </summary>
    /// <param name="entityType">The entity type ("Plant" or "Product").</param>
    /// <param name="entityId">The entity identifier string that lacks a mapping.</param>
    /// <param name="providerId">The provider ID for which the mapping is absent.</param>
    public MissingProviderMappingException(string entityType, string entityId, string providerId)
        : base($"{entityType} '{entityId}' not mapped for provider {providerId}.")
    {
        EntityType = entityType;
        EntityId = entityId;
        ProviderId = providerId;
    }

    /// <summary>Gets the entity type (Plant / Product).</summary>
    public string EntityType { get; }

    /// <summary>Gets the entity identifier that lacks a mapping.</summary>
    public string EntityId { get; }

    /// <summary>Gets the provider ID for which the mapping is absent.</summary>
    public string ProviderId { get; }
}

/// <summary>
/// Exception thrown when assembled DTO data is internally inconsistent.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 9)
/// </summary>
public class DataConsistencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataConsistencyException"/> class.
    /// </summary>
    /// <param name="message">Description of the inconsistency.</param>
    public DataConsistencyException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when <c>DataFine</c> precedes <c>DataInizio</c> in an activity.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 9)
/// </summary>
public class InvalidDateRangeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDateRangeException"/> class.
    /// </summary>
    /// <param name="dataInizio">The activity start date.</param>
    /// <param name="dataFine">The activity end date.</param>
    public InvalidDateRangeException(DateTime dataInizio, DateTime dataFine)
        : base($"Data fine precedente data inizio (inizio={dataInizio:O}, fine={dataFine:O}).")
    {
        DataInizio = dataInizio;
        DataFine = dataFine;
    }

    /// <summary>Gets the start date.</summary>
    public DateTime DataInizio { get; }

    /// <summary>Gets the end date.</summary>
    public DateTime DataFine { get; }
}

/// <summary>
/// Exception thrown when input parameters for activity lookup fail validation.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 1)
/// </summary>
public class AttivitaValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttivitaValidationException"/> class.
    /// </summary>
    /// <param name="message">Description of the validation failure.</param>
    public AttivitaValidationException(string message) : base(message) { }
}

// ─── DS03-BL exceptions ─────────────────────────────────────────────────────

/// <summary>
/// Exception thrown when a required field is absent from the composition input DTO
/// or the machine object.
/// Referenced in Design Specification: DS03-BL - Composizione Payload (PHASE 1, Rule 9, Rule 10)
/// </summary>
public class MissingRequiredFieldException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredFieldException"/> class.
    /// </summary>
    /// <param name="fieldName">The name of the missing or null field.</param>
    public MissingRequiredFieldException(string fieldName)
        : base($"Required field missing: {fieldName}.")
    {
        FieldName = fieldName;
    }

    /// <summary>Gets the name of the missing field.</summary>
    public string FieldName { get; }
}

/// <summary>
/// Exception thrown when the upload of a raster prescription map to the provider API fails.
/// Referenced in Design Specification: DS03-BL - Composizione Payload (PHASE 7, Rule 6)
/// </summary>
public class RasterUploadException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RasterUploadException"/> class.
    /// </summary>
    /// <param name="mapId">The Agronica map UUID whose upload failed.</param>
    /// <param name="errorDetail">Technical detail from the provider API response.</param>
    public RasterUploadException(int mapId, string errorDetail)
        : base($"Raster map {mapId} upload failed: {errorDetail}.")
    {
        MapId = mapId;
        ErrorDetail = errorDetail;
    }

    /// <summary>Gets the map UUID that failed to upload.</summary>
    public int MapId { get; }

    /// <summary>Gets the technical error detail from the provider response.</summary>
    public string ErrorDetail { get; }
}

/// <summary>
/// Exception thrown when the composed payload does not conform to the Smart Tractor API schema.
/// Referenced in Design Specification: DS03-BL - Composizione Payload (PHASE 10, Rule 11)
/// </summary>
public class SchemaValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaValidationException"/> class.
    /// </summary>
    /// <param name="errors">Collection of validation error messages from schema check.</param>
    public SchemaValidationException(IReadOnlyList<string> errors)
        : base($"Payload validation failed: {string.Join("; ", errors)}.")
    {
        Errors = errors;
    }

    /// <summary>Gets the list of schema validation errors.</summary>
    public IReadOnlyList<string> Errors { get; }
}

/// <summary>
/// Exception thrown when the serialized payload size exceeds the 100 MB hard limit.
/// Referenced in Design Specification: DS03-BL - Composizione Payload (PHASE 11, Rule 12)
/// </summary>
public class PayloadSizeExceededException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PayloadSizeExceededException"/> class.
    /// </summary>
    /// <param name="actualSizeBytes">The actual payload size that exceeded the limit.</param>
    public PayloadSizeExceededException(long actualSizeBytes)
        : base($"Payload exceeds maximum size (100MB): {actualSizeBytes} bytes.")
    {
        ActualSizeBytes = actualSizeBytes;
    }

    /// <summary>Gets the actual payload size in bytes.</summary>
    public long ActualSizeBytes { get; }
}

// ─── DS04-BL exceptions ─────────────────────────────────────────────────────

/// <summary>
/// Exception thrown when a database INSERT or UPDATE fails during the invio transaction,
/// leaving the request in an inconsistent state.
/// Referenced in Design Specification: DS04-BL - Gestore Invio e Transazione Smart Tractor
/// </summary>
public class DatabaseTransactionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseTransactionException"/> class.
    /// </summary>
    /// <param name="message">Human-readable description of the failure.</param>
    public DatabaseTransactionException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseTransactionException"/> class
    /// with an inner exception.
    /// </summary>
    /// <param name="message">Human-readable description of the failure.</param>
    /// <param name="innerException">The original database exception.</param>
    public DatabaseTransactionException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when the HTTP call to the Smart Tractor API endpoint for
/// sending an activity fails (non-2xx response or network error).
/// Referenced in Design Specification: DS04-BL - Gestore Invio e Transazione Smart Tractor
/// </summary>
public class SmartTractorSendException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SmartTractorSendException"/> class.
    /// </summary>
    /// <param name="ricettaOperazioneCod">The Agronica UUID for the failed request.</param>
    /// <param name="errorDetail">Technical detail from the provider API response or network layer.</param>
    /// <param name="innerException">The original exception, if any.</param>
    public SmartTractorSendException(int ricettaOperazioneCod, string errorDetail, Exception? innerException = null)
        : base($"Smart Tractor send failed for request {ricettaOperazioneCod}: {errorDetail}.", innerException)
    {
        RicettaOperazioneCod = ricettaOperazioneCod;
        ErrorDetail = errorDetail;
    }

    /// <summary>Gets the Agronica request UUID that failed to send.</summary>
    public int RicettaOperazioneCod { get; }

    /// <summary>Gets the technical error detail.</summary>
    public string ErrorDetail { get; }
}
