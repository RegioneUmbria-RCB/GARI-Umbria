namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Enumeration of validation error codes for activity validation.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public enum ValidationErrorCode
{
    /// <summary>
    /// Activity date is in the past.
    /// </summary>
    ErrActivityDateInPast,

    /// <summary>
    /// Facility/plant is not mapped with the provider.
    /// </summary>
    ErrFacilityNotMapped,

    /// <summary>
    /// Machine is not mapped with the provider.
    /// </summary>
    ErrMachineNotMapped,

    /// <summary>
    /// Product is not mapped with the provider.
    /// </summary>
    ErrProductNotMapped,

    /// <summary>
    /// Generic validation error.
    /// </summary>
    ErrGeneric
}
