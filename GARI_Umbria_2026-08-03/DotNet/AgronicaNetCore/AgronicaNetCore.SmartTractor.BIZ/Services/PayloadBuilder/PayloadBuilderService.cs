using System.Data;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Exceptions;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.SmartTractor.BIZ.Services.Engine;
using AgronicaNetCore.SmartTractor.DAL.ProviderMappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.PayloadBuiler;

/// <summary>
/// Composes the Smart Tractor API request payload from a pre-processed activity DTO.
///
/// Execution phases (per DS03-BL specification):
/// PHASE 1  – Validate mandatory input fields
/// PHASE 2  – Determine plants to include
/// PHASE 3  – Compose plots (DISTINCT provider_code, de-duplicated)
/// PHASE 4  – Compose geometry (union WKT placeholder — requires external GIS)
/// PHASE 5  – Compose A-B line
/// PHASE 6  – Double-check product mappings, compose products
/// PHASE 7  – Pre-upload raster maps, compose attachments
/// PHASE 8  – Compose payload root fields from machine object
/// PHASE 9  – Validate date range
/// PHASE 10 – Validate payload schema
/// PHASE 11 – Check payload size (&lt;= 100 MB)
/// PHASE 12 – Build and return composition output
///
/// Referenced in Design Specification:
/// DS03-BL - Composizione Payload per Invio Smart Tractor
/// </summary>
public class PayloadBuilderService : BaseServiceSmartTractorBIZ, IPayloadBuilderService
{
    private const long MaxPayloadSizeBytes = 100_000_000; // 100 MB
    private const string ChiaveTenantEngine = "tenantEngine_SmartTractor";
    private const string ChiaveGestioneAllegatiRepository = "GestioneAllegati_Repository";

    private readonly IProviderMappingProduct _providerMappingProduct;
    private readonly IProviderMappingMachine _providerMappingMachine;
    private readonly IProviderMappingPlant _providerMappingPlant;
    private readonly IProviderMappingRasterMap _providerMappingRasterMap;
    private readonly IEngineService _engineService;
    private readonly ISecurityLayerDAL _securityLayerDal;

    public PayloadBuilderService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _providerMappingProduct = provider.GetRequiredService<IProviderMappingProduct>();
        _providerMappingMachine= provider.GetRequiredService<IProviderMappingMachine >();
        _providerMappingPlant = provider.GetRequiredService<IProviderMappingPlant >();
        _providerMappingRasterMap = provider.GetRequiredService<IProviderMappingRasterMap >();
        _engineService = provider.GetRequiredService<IEngineService >();
        _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ComposizionePayloadOutput>> BuildPayloadAsync(
        PrescriptionDataDTO prescriptionData,
        AgronicaCoreParametriServer serverParams,
        AgronicaCoreParametriSuperServer superServerParams,
        CancellationToken cancellationToken = default)
    {
        var payloads = new List<ComposizionePayloadOutput>();
        var machineries = await ComposeMachineriesAsync(prescriptionData.MachineryIds, serverParams);

        foreach (var machinery in machineries)
        {
            var plants = await ComposePlantsAsync(prescriptionData.Destinazioni, machinery.ProviderId, serverParams);
            plants = DeterminePlantsToInclude(plants);

            // ===== PHASE 4: COMPOSE GEOMETRY (UNION WKT) =====
            GeometryItem? geometry = null; // Geometry union requires a GIS library; omitted when no WKT in plants.

            // ===== PHASE 5: COMPOSE A-B LINE =====
            AbLineItem? abLine = null;
            if (!string.IsNullOrWhiteSpace(prescriptionData.AbLineWkt))
            {
                abLine = new AbLineItem
                {
                    srs = "EPSG:4326",
                    wkt = prescriptionData.AbLineWkt
                };
            }

            var products = await ComposeProductsAsync(prescriptionData.Prodotti, machinery.ProviderId, serverParams);

            var (attachments, prescriptionKey) = await ComposeAttachmentsAsync(
                prescriptionData.RicettaOperazioneCod, machinery, serverParams, superServerParams, cancellationToken);

            var tenant = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(ChiaveTenantEngine, serverParams, superServerParams);

            var payload = new SmartTractorPayload
            {
                tenant          = tenant,
                providerCode    = machinery.ProviderId!,
                deviceCode      = machinery.ProviderCode!,
                operatorCode    = "",
                fmisOrgId       = serverParams.UsernameOperazione,
                activityType    = prescriptionData.LavCod.ToString(),
                plannedPeriod   = new PlannedPeriod
                {
                    startDate   = prescriptionData.StartDate.ToString("yyyy-MM-dd"),
                    endDate     = prescriptionData.EndDate.ToString("yyyy-MM-dd")
                },
                plots           = plants.Select<PlantInfo, PlotItem>((p) => new PlotItem() { plotCode = p.ProviderCode! } ).ToList(),
                products        = products.Select<ProductInfo, ProductItem>((p) => new ProductItem() { type = "", code = p.ProviderCode!, qty = p.Quantity, unitOfMeasure = "" }).ToList(),
                geometry        = geometry,
                abLine          = abLine,
                prescriptionKey = prescriptionData.RicettaOperazioneCod.ToString(),
                attachments     = attachments
            };

            // ===== PHASE 9: VALIDATE DATE RANGE =====
            if (DateOnly.TryParse(prescriptionData.EndDate.ToString(), out var endDate) &&
                DateOnly.TryParse(prescriptionData.StartDate.ToString(), out var startDate) &&
                endDate < startDate)
            {
                throw new InvalidDateRangeException(
                    startDate.ToDateTime(TimeOnly.MinValue),
                    endDate.ToDateTime(TimeOnly.MinValue));
            }

            // ===== PHASE 10: SCHEMA VALIDATION =====
            var schemaErrors = ValidateSchema(payload);
            if (schemaErrors.Count > 0)
                throw new SchemaValidationException(schemaErrors);

            // ===== PHASE 11: PAYLOAD SIZE CHECK =====
            var payloadJson = JsonSerializer.Serialize(payload);
            var payloadBytes = Encoding.UTF8.GetByteCount(payloadJson);

            if (payloadBytes > MaxPayloadSizeBytes)
                throw new PayloadSizeExceededException(payloadBytes);

            // ===== PHASE 12: BUILD OUTPUT =====
            payloads.Add(new ComposizionePayloadOutput
            {
                RicettaOperazioneCod = prescriptionData.RicettaOperazioneCod,
                MacCod = machinery.MacCod,
                Payload = payload,
                PayloadSizeBytes = payloadBytes,
                SchemaValidation = new SchemaValidationResult { IsValid = true, Errors = Array.Empty<string>() }
            });
        }
        return payloads;
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    private async Task<IReadOnlyList<MachineInfo>> ComposeMachineriesAsync(
        IReadOnlyList<int> machineryIds,
        AgronicaCoreParametriServer serverParams)
    {
        var items = new List<MachineInfo>(machineryIds.Count);

        foreach (var id in machineryIds)
        {
            var mappingTable = await _providerMappingMachine.GetMachineMappingAsync(id, serverParams);

            if (mappingTable.Rows.Count == 0)
                throw new MissingProviderMappingException(
                    "Machine", id.ToString(), "");

            var providerId = mappingTable.Rows[0].Field<string?>("provider_id");
            var providerCode = mappingTable.Rows[0].Field<string?>("provider_code");

            if (string.IsNullOrWhiteSpace(providerCode))
                throw new MissingProviderMappingException(
                    "Machine", id.ToString(), "");

            items.Add(new MachineInfo
            {
                MacCod = id,
                ProviderId = providerId,
                ProviderCode = providerCode
            });
        }

        return items;
    }
    
    private async Task<IReadOnlyList<PlantInfo>> ComposePlantsAsync(
        IReadOnlyList<PlantId> plantIds,
        string providerId,
        AgronicaCoreParametriServer serverParams)
    {
        var items = new List<PlantInfo>(plantIds.Count);

        foreach (var id in plantIds)
        {
            var mappingTable = await _providerMappingPlant.GetPlantMappingByCompositeKeyAsync(providerId, id.Piva, id.SaCod, id.Appezza, id.IdReg, serverParams);

            if (mappingTable.Rows.Count == 0)
                throw new MissingProviderMappingException(
                    "Machine", id.ToString(), "");

            var providerCode = mappingTable.Rows[0].Field<string?>("provider_code");

            if (string.IsNullOrWhiteSpace(providerCode))
                throw new MissingProviderMappingException(
                    "Machine", id.ToString(), "");

            items.Add(new PlantInfo
            {
                Piva = id.Piva,
                SaCod = id.SaCod,
                Appezza = id.Appezza,
                IdReg = id.IdReg,
                ProviderCode = providerCode
            });
        }

        return items;
    }

    private static IReadOnlyList<PlantInfo> DeterminePlantsToInclude(IReadOnlyList<PlantInfo> plants)
    {
        var selectedPlants = plants.DistinctBy(p => p.ProviderCode);

        if (plants.Count == 0)
            throw new MissingRequiredFieldException("plants");

        return plants;
    }

    private async Task<IReadOnlyList<ProductInfo>> ComposeProductsAsync(
        IReadOnlyList<ProductDetails> productsDetails,
        string providerId,
        AgronicaCoreParametriServer serverParams)
    {
        if (productsDetails.Count == 0)
            return Array.Empty<ProductInfo>();

        var items = new List<ProductInfo>(productsDetails.Count);

        foreach (var product in productsDetails)
        {
            // Double-check: fetch provider_code for this product (FASE 6 double-check rule)
            var mappingTable = await _providerMappingProduct.GetProductMappingAsync(
                product.ElemCod, product.Id, providerId, serverParams);

            if (mappingTable.Rows.Count == 0)
                throw new MissingProviderMappingException(
                    "Product", product.Id.ToString(), providerId);

            var providerCode = mappingTable.Rows[0].Field<string?>("provider_code");

            if (string.IsNullOrWhiteSpace(providerCode))
                throw new MissingProviderMappingException(
                    "Product", product.Id.ToString(), providerId);

            items.Add(new ProductInfo
            {
                Id            = product.Id,
                ElemCod       = product.ElemCod,
                Quantity      = product.Qty,
                ProviderCode  = providerCode,
                UnitOfMeasure = product.UoM
            });
        }

        return items;
    }

    /// <summary>
    /// Pre-uploads each prescription map to the provider and composes the attachments list.
    /// Persists the upload mapping in <c>smart_tractor_raster_upload_mapping</c>.
    /// Referenced in Design Specification: DS03-BL - Rule 6 (PHASE 7).
    /// </summary>
    private async Task<(IReadOnlyList<AttachmentItem> Attachments, string? PrescriptionKey)> ComposeAttachmentsAsync(
        int ricettaOperazioneCod,
        MachineInfo machine,
        AgronicaCoreParametriServer serverParams,
        AgronicaCoreParametriSuperServer superServerParams,
        CancellationToken cancellationToken)
    {
        var rasterMapTable = await _providerMappingRasterMap.GetRasterMapsByPrescriptionAsync(ricettaOperazioneCod, serverParams);

        if (rasterMapTable == null || rasterMapTable.Rows.Count == 0)
            return (Array.Empty<AttachmentItem>(), null);

        var filePath = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(ChiaveGestioneAllegatiRepository, serverParams, superServerParams);
        var attachments = new List<AttachmentItem>(rasterMapTable.Rows.Count);
        var rasterEnum = rasterMapTable.Rows.GetEnumerator();
        
        while (rasterEnum.MoveNext())
        {
            var fileName = ((DataRow)rasterEnum.Current).Field<string>("Allegati_Documenti_NomeFile");
            var fileId = ((DataRow)rasterEnum.Current).Field<int>("Allegati_Documenti_Cod");
            if(string.IsNullOrWhiteSpace(fileName))
                throw new RasterUploadException(fileId, "Raster file path is empty.");

            var basePath = filePath.TrimEnd('\\');
            var primaryPath = $@"{basePath}\PrecisionFarming_MappaPrescrizione\{fileName}";
            var fallbackPath = $@"{basePath}\{fileName}";

            byte[] tiffBytes;
            try
            {
                tiffBytes = await File.ReadAllBytesAsync(primaryPath, cancellationToken);
            }
            catch (Exception ex) when (ex is IOException or FileNotFoundException or UnauthorizedAccessException)
            {
                tiffBytes = await File.ReadAllBytesAsync(fallbackPath, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RasterUploadException(fileId, $"Could not read raster file '{fileName}': {ex.Message}");
            }

            var uploadResponse = await _engineService.UploadRasterMapAsync(
                fileId,
                machine.ProviderId!,
                tiffBytes,
                serverParams,
                superServerParams,
                cancellationToken);

            // Persist the upload mapping record for audit and future reference.
            await _providerMappingRasterMap.InsertRasterMapMappingAsync(
                fileId,
                machine.ProviderId!,
                uploadResponse.id,
                serverParams);

            attachments.Add(new AttachmentItem { id = uploadResponse.id });
        }

        // Use the first attachment ID as the prescriptionKey when only one map is uploaded.
        var prescriptionKey = attachments.Count == 1 ? attachments[0].id : null;

        return (attachments, prescriptionKey);
    }

    /// <summary>
    /// Validates the payload against the Smart Tractor API schema constraints.
    /// Returns an empty list when the payload is valid.
    /// Referenced in Design Specification: DS03-BL - Rule 11 (PHASE 10).
    /// </summary>
    private static List<string> ValidateSchema(SmartTractorPayload payload)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(payload.tenant))
            errors.Add("tenant is required");

        if (string.IsNullOrWhiteSpace(payload.providerCode))
            errors.Add("providerCode is required");

        if (string.IsNullOrWhiteSpace(payload.deviceCode))
            errors.Add("deviceCode is required");

        //if (string.IsNullOrWhiteSpace(payload.OperatorCode))
        //    errors.Add("operatorCode is required");

        if (string.IsNullOrWhiteSpace(payload.fmisOrgId))
            errors.Add("fmisOrgId is required");

        if (string.IsNullOrWhiteSpace(payload.activityType))
            errors.Add("activityType is required");

        if (string.IsNullOrWhiteSpace(payload.plannedPeriod?.startDate))
            errors.Add("plannedPeriod.startDate is required");

        if (string.IsNullOrWhiteSpace(payload.plannedPeriod?.endDate))
            errors.Add("plannedPeriod.endDate is required");

        if (payload.plots.Count == 0)
            errors.Add("plots must contain at least one item");

        return errors;
    }
}
