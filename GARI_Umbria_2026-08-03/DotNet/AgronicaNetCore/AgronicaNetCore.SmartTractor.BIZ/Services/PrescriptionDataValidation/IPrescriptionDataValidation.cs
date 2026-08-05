using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.PrescriptionDataValidation;

/// <summary>
/// Interface for activity data validation service.
/// Validates activity data before submission to Smart Tractor provider.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public interface IPrescriptionDataValidation
{
    /// <summary>
    /// Validates activity data for submission to a specific provider.
    /// 
    /// Validation rules:
    /// - Activity date must not be in the past (>= TODAY())
    /// - Facility/plant must be mapped with provider (if not using SRID polygon)
    /// - Machine must be mapped with provider
    /// - All products must be mapped with provider (if any products are present)
    /// 
    /// </summary>
    /// <param name="request">The activity validation request containing activity data and provider information.</param>
    /// <param name="serverParams">Server parameters for database access.</param>
    /// <returns>Validation response with errors and recommended actions if validation fails.</returns>
    Task<ActivityValidationResponse> ValidateActivityAsync(
        ActivityValidationRequest request,
        AgronicaCoreParametriServer serverParams
    );
}
