namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents the root JSON payload sent to the Smart Tractor API endpoint.
/// All fields map to the Smart Tractor API contract documented in the framework reference.
/// Referenced in Design Specification: DS03-BL - Composizione Payload (OUTPUT section)
/// </summary>
public class SmartTractorPayload
{
    /// <summary>Gets or sets the SuperUser/tenant identifier.</summary>
    public string tenant { get; set; } = string.Empty;

    /// <summary>Gets or sets the FMIS organization ID (Partita IVA).</summary>
    public string fmisOrgId { get; set; } = string.Empty;

    public string providerCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the prescription key returned by the provider after raster upload.
    /// Null when <c>include_prescription_maps=false</c> or no maps are present.
    /// Referenced in Design Specification: DS03-BL - Rule 6.
    /// </summary>
    public string? prescriptionKey { get; set; }

    /// <summary>Gets or sets the activity type code (e.g. "aratura", "semina").</summary>
    public string activityType { get; set; } = string.Empty;

    /// <summary>Gets or sets the device code assigned by the provider for the machine.</summary>
    public string deviceCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the operator code (current user identifier).</summary>
    public string operatorCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the planned execution period.</summary>
    public PlannedPeriod plannedPeriod { get; set; } = new();

    /// <summary>Gets or sets the destination plots (impianti).</summary>
    public IReadOnlyList<PlotItem> plots { get; set; } = Array.Empty<PlotItem>();

    /// <summary>
    /// Gets or sets the union geometry of the destination plots in WKT.
    /// Null when no geometry is available.
    /// </summary>
    public GeometryItem? geometry { get; set; }

    /// <summary>
    /// Gets or sets the A-B guidance line. Null when not provided.
    /// Referenced in Design Specification: DS03-BL - Rule 8.
    /// </summary>
    public AbLineItem? abLine { get; set; }

    /// <summary>Gets or sets the products to apply. Empty when <c>include_products=false</c>.</summary>
    public IReadOnlyList<ProductItem> products { get; set; } = Array.Empty<ProductItem>();

    /// <summary>Gets or sets the attachment references for uploaded raster maps.</summary>
    public IReadOnlyList<AttachmentItem> attachments { get; set; } = Array.Empty<AttachmentItem>();
}

/// <summary>
/// Represents the planned activity period with ISO 8601 date strings.
/// Referenced in Design Specification: DS03-BL - Rule 10.
/// </summary>
public class PlannedPeriod
{
    /// <summary>Gets or sets the planned start date in YYYY-MM-DD format.</summary>
    public string startDate { get; set; } = string.Empty;

    /// <summary>Gets or sets the planned end date in YYYY-MM-DD format.</summary>
    public string endDate { get; set; } = string.Empty;
}

/// <summary>
/// Represents a spatial geometry (WKT) with its spatial reference system.
/// Referenced in Design Specification: DS03-BL - Rule 7 (PHASE 4).
/// </summary>
public class GeometryItem
{
    /// <summary>Gets or sets the spatial reference system identifier (e.g. "EPSG:4326").</summary>
    public string srs { get; set; } = "EPSG:4326";

    /// <summary>Gets or sets the WKT geometry string.</summary>
    public string wkt { get; set; } = string.Empty;
}

/// <summary>
/// Represents an A-B guidance line with its spatial reference system.
/// Referenced in Design Specification: DS03-BL - Rule 8 (PHASE 5).
/// </summary>
public class AbLineItem
{
    /// <summary>Gets or sets the spatial reference system identifier.</summary>
    public string srs { get; set; } = "EPSG:4326";

    /// <summary>Gets or sets the WKT line string.</summary>
    public string wkt { get; set; } = string.Empty;
}

/// <summary>
/// Represents a raster map attachment reference in the Smart Tractor payload.
/// The attachment ID is the identifier returned by the provider after pre-upload.
/// Referenced in Design Specification: DS03-BL - Rule 6 (PHASE 7).
/// </summary>
public class AttachmentItem
{
    /// <summary>Gets or sets the attachment identifier assigned by the provider.</summary>
    public string id { get; set; } = string.Empty;
    public string type { get; set; } = "raster";
    public string productUOM { get; set; } = string.Empty;
    public string areaUOM { get; set; } = string.Empty;
}

public class PlotItem
{
    public string plotCode { get; set; } = string.Empty;
}

public class ProductItem
{
    public string type { get; set; } = string.Empty;
    public string code { get; set; } = string.Empty;
    public float qty { get; set; } = 0;
    public string unitOfMeasure { get; set; } = string.Empty;
}