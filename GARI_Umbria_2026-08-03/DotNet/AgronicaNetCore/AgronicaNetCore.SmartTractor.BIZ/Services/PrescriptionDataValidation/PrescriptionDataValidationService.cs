using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.DAL.ProviderMappings;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.SmartTractor.BIZ.Services.PrescriptionDataValidation;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.ValidazioneDatiAttivita;

/// <summary>
/// Service for validating activity data before submission to Smart Tractor provider.
/// Implements all validation rules specified in DS02-BL.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public class PrescriptionDataValidationService : BaseServiceSmartTractorBIZ, IPrescriptionDataValidation
{
    private readonly IProviderMappingPlant _plantMappingProvider;
    private readonly IProviderMappingMachine _machineMappingProvider;
    private readonly IProviderMappingProduct _productMappingProvider;

    /// <summary>
    /// Initializes a new instance of the ValidazioneDatiAttivitaService class.
    /// </summary>
    /// <param name="provider">Service provider for dependency injection.</param>
    /// <param name="localizer">Localizer for error messages.</param>
    /// <param name="plantMappingProvider">Provider for plant mapping access.</param>
    /// <param name="machineMappingProvider">Provider for machine mapping access.</param>
    /// <param name="productMappingProvider">Provider for product mapping access.</param>
    public PrescriptionDataValidationService(
        IServiceProvider provider,
        IStringLocalizer<Messages> localizer,
        IProviderMappingPlant plantMappingProvider,
        IProviderMappingMachine machineMappingProvider,
        IProviderMappingProduct productMappingProvider
    ) : base(provider, localizer)
    {
        _plantMappingProvider = plantMappingProvider ?? throw new ArgumentNullException(nameof(plantMappingProvider));
        _machineMappingProvider = machineMappingProvider ?? throw new ArgumentNullException(nameof(machineMappingProvider));
        _productMappingProvider = productMappingProvider ?? throw new ArgumentNullException(nameof(productMappingProvider));
    }

    /// <summary>
    /// Validates activity data for submission to a specific provider.
    /// 
    /// Validation rules implemented:
    /// 1. Activity date must not be in the past (>= TODAY())
    /// 2. Facility/plant must be mapped with provider (if not using SRID polygon)
    /// 3. Machine must be mapped with provider
    /// 4. All products must be mapped with provider (if any products are present)
    /// </summary>
    /// <param name="request">The activity validation request containing activity data and provider information.</param>
    /// <param name="serverParams">Server parameters for database access.</param>
    /// <returns>Validation response with errors and recommended actions if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if request or serverParams is null.</exception>
    public async Task<ActivityValidationResponse> ValidateActivityAsync(
        ActivityValidationRequest request,
        AgronicaCoreParametriServer serverParams
    )
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (serverParams == null)
            throw new ArgumentNullException(nameof(serverParams));

        var response = new ActivityValidationResponse();

        try
        {
            // Validate activity date is not in the past
            await ValidateActivityDateAsync(request, response);

            // Parse provider ID for mapping checks
            //if (!string.TryParse(request.ProviderId, out string providerId))
            //{
            //    response.ValidationErrors.Add(new ValidationError
            //    {
            //        ErrorCode = ValidationErrorCode.ErrGeneric,
            //        Field = "provider_id",
            //        MessageSynthetic = "Invalid provider ID format",
            //        Severity = ValidationErrorSeverity.Blocking
            //    });
            //    response.IsValid = false;
            //    return response;
            //}

            // Validate facility/plant mapping (unless SRID polygon is provided)
            if (string.IsNullOrWhiteSpace(request.PolygonSrid) && request.Plants != null && request.Plants.Length > 0)
            {
                await ValidatePlantMappingsAsync(request, request.ProviderId, serverParams, response);
            }

            // Validate machine mapping
            await ValidateMachineMappingAsync(request, request.ProviderId, serverParams, response);

            // Validate product mappings (if products present)
            if (request.Products != null && request.Products.Length > 0)
            {
                await ValidateProductMappingsAsync(request, request.ProviderId, serverParams, response);
            }

            // Determine overall validity based on blocking errors
            response.IsValid = !response.ValidationErrors.Any(e => e.Severity == ValidationErrorSeverity.Blocking);

            // Add recommended actions for errors
            GenerateRecommendedActions(response, request);
        }
        catch (Exception ex)
        {
            // Log the error but return structured validation response
            LogError($"Error validating activity {request.ActivityId}", serverParams, ex);
            response.ValidationErrors.Add(new ValidationError
            {
                ErrorCode = ValidationErrorCode.ErrGeneric,
                Field = "activity",
                MessageSynthetic = "Validation error occurred",
                Severity = ValidationErrorSeverity.Blocking
            });
            response.IsValid = false;
        }

        return response;
    }

    /// <summary>
    /// Validates that the activity date is not in the past.
    /// Rule: activity_date >= TODAY()
    /// </summary>
    private Task ValidateActivityDateAsync(ActivityValidationRequest request, ActivityValidationResponse response)
    {
        if (request.ActivityDate < DateTime.Today)
        {
            response.ValidationErrors.Add(new ValidationError
            {
                ErrorCode = ValidationErrorCode.ErrActivityDateInPast,
                Field = "activity_date",
                MessageSynthetic = "Activity date cannot be in the past",
                Severity = ValidationErrorSeverity.Blocking
            });
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Validates that all plants associated with the activity are mapped with the provider.
    /// Rule: For each plant, verify mapping exists in provider_mapping table
    /// </summary>
    private async Task ValidatePlantMappingsAsync(
        ActivityValidationRequest request,
        string providerId,
        AgronicaCoreParametriServer serverParams,
        ActivityValidationResponse response
    )
    {
        if (request.Plants == null || request.Plants.Length == 0)
            return;

        foreach (int plantId in request.Plants)
        {
            bool isMapped = await _plantMappingProvider.IsPlantMappedAsync(plantId, providerId, serverParams);

            if (!isMapped)
            {
                response.ValidationErrors.Add(new ValidationError
                {
                    ErrorCode = ValidationErrorCode.ErrFacilityNotMapped,
                    Field = "facility",
                    MessageSynthetic = $"Facility not mapped - complete mapping",
                    Severity = ValidationErrorSeverity.Blocking
                });

                // Only add one error for unmapped facility
                break;
            }
        }
    }

    /// <summary>
    /// Validates that the machine used in the activity is mapped with the provider.
    /// Rule: Verify machine mapping exists in provider_mapping table
    /// </summary>
    private async Task ValidateMachineMappingAsync(
        ActivityValidationRequest request,
        string providerId,
        AgronicaCoreParametriServer serverParams,
        ActivityValidationResponse response
    )
    {
        if (request.MachineId == null)
        {
            response.ValidationErrors.Add(new ValidationError
            {
                ErrorCode = ValidationErrorCode.ErrGeneric,
                Field = "machine_id",
                MessageSynthetic = "Machine ID is required",
                Severity = ValidationErrorSeverity.Blocking
            });
            return;
        }

        bool isMapped = await _machineMappingProvider.IsMachineMappedAsync(request.MachineId.Value, providerId, serverParams);

        if (!isMapped)
        {
            response.ValidationErrors.Add(new ValidationError
            {
                ErrorCode = ValidationErrorCode.ErrMachineNotMapped,
                Field = "machine",
                MessageSynthetic = "Machine not mapped - complete onboarding",
                Severity = ValidationErrorSeverity.Blocking
            });
        }
    }

    /// <summary>
    /// Validates that all products associated with the activity are mapped with the provider.
    /// Rule: For each product, verify mapping exists in provider_mapping table
    /// </summary>
    private async Task ValidateProductMappingsAsync(
        ActivityValidationRequest request,
        string providerId,
        AgronicaCoreParametriServer serverParams,
        ActivityValidationResponse response
    )
    {
        if (request.Products == null || request.Products.Length == 0)
            return;

        var unmappedProducts = new List<string>();

        foreach (KeyValuePair<int, int> product in request.Products)
        {
            bool isMapped = await _productMappingProvider.IsProductMappedAsync(product.Key, product.Value, providerId, serverParams);

            if (!isMapped)
            {
                unmappedProducts.Add(product.ToString());
            }
        }

        if (unmappedProducts.Count > 0)
        {
            response.ValidationErrors.Add(new ValidationError
            {
                ErrorCode = ValidationErrorCode.ErrProductNotMapped,
                Field = "products",
                MessageSynthetic = $"Product(s) not mapped with provider",
                Severity = ValidationErrorSeverity.Blocking
            });
        }
    }

    /// <summary>
    /// Generates recommended actions based on validation errors encountered.
    /// </summary>
    private void GenerateRecommendedActions(ActivityValidationResponse response, ActivityValidationRequest request)
    {
        foreach (var error in response.ValidationErrors)
        {
            switch (error.ErrorCode)
            {
                case ValidationErrorCode.ErrActivityDateInPast:
                    response.RecommendedActions.Add("Update activity date to a future date");
                    break;

                case ValidationErrorCode.ErrFacilityNotMapped:
                    response.RecommendedActions.Add("Map the facility with the provider in Complete Management");
                    break;

                case ValidationErrorCode.ErrMachineNotMapped:
                    response.RecommendedActions.Add("Complete machine onboarding for the provider");
                    break;

                case ValidationErrorCode.ErrProductNotMapped:
                    response.RecommendedActions.Add("Map all products with the provider");
                    break;

                default:
                    response.RecommendedActions.Add("Review activity data and try again");
                    break;
            }
        }
    }
}
