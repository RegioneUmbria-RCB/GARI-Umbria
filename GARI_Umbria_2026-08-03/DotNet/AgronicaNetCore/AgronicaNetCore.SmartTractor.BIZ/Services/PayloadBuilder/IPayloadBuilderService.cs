using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.PayloadBuiler;

/// <summary>
/// Service interface for composing the Smart Tractor submission payload from a
/// pre-processed activity DTO.
///
/// Responsibilities (per specification):
/// <list type="number">
///   <item><description>PHASE 1  – Validate mandatory input fields.</description></item>
///   <item><description>PHASE 2  – Determine plants to include (filter by <c>selected_plants_ids</c>).</description></item>
///   <item><description>PHASE 3  – Compose plots (DISTINCT provider codes from validated DTO).</description></item>
///   <item><description>PHASE 4  – Compose geometry (UNION WKT of plant polygons, if present).</description></item>
///   <item><description>PHASE 5  – Compose A-B line.</description></item>
///   <item><description>PHASE 6  – Double-check product mappings and compose products.</description></item>
///   <item><description>PHASE 7  – Pre-upload raster maps and compose attachments.</description></item>
///   <item><description>PHASE 8  – Compose payload root fields from machine object.</description></item>
///   <item><description>PHASE 9  – Validate date range.</description></item>
///   <item><description>PHASE 10 – Validate payload schema.</description></item>
///   <item><description>PHASE 11 – Check payload size (&lt;= 100 MB).</description></item>
///   <item><description>PHASE 12 – Build and return composition output.</description></item>
/// </list>
///
/// This service does NOT send the payload to Smart Tractor; that is handled by FS003.
/// Referenced in Design Specification: DS03-BL - Composizione Payload per Invio Smart Tractor
/// </summary>
public interface IPayloadBuilderService
{
    /// <summary>
    /// Composes the Smart Tractor API payload from the provided activity DTO and
    /// composition parameters.
    /// </summary>
    /// <param name="request">
    /// Composition request carrying the pre-processed <see cref="PrescriptionPayloadDTO"/>
    /// and caller-supplied parameters (dates, flags, plant filter).
    /// </param>
    /// <param name="serverParams">Database connection context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A <see cref="ComposizionePayloadOutput"/> containing the composed payload,
    /// the generated request UUID, payload size, and schema validation outcome.
    /// </returns>
    /// <exception cref="Exceptions.MissingRequiredFieldException">
    /// When a mandatory root field (machine tenant/org/provider/device) or date is absent.
    /// </exception>
    /// <exception cref="Exceptions.MissingProviderMappingException">
    /// When a product has no mapping for the provider (double-check, PHASE 6).
    /// </exception>
    /// <exception cref="Exceptions.RasterUploadException">
    /// When a prescription map upload to the provider API fails (PHASE 7).
    /// </exception>
    /// <exception cref="Exceptions.InvalidDateRangeException">
    /// When <c>plannedEndDate</c> precedes <c>plannedStartDate</c> (PHASE 9).
    /// </exception>
    /// <exception cref="Exceptions.SchemaValidationException">
    /// When the composed payload does not conform to the Smart Tractor API schema (PHASE 10).
    /// </exception>
    /// <exception cref="Exceptions.PayloadSizeExceededException">
    /// When the serialized payload exceeds 100 MB (PHASE 11).
    /// </exception>
    Task<IReadOnlyList<ComposizionePayloadOutput>> BuildPayloadAsync(
        PrescriptionDataDTO request,
        AgronicaCoreParametriServer serverParams,
        AgronicaCoreParametriSuperServer superServerParams,
        CancellationToken cancellationToken = default);
}
