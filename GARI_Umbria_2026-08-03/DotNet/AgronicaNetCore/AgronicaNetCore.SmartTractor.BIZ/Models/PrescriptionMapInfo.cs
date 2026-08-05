namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents a prescription map associated with an activity.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività
/// </summary>
public class PrescriptionMapInfo
{
    /// <summary>Gets or sets the prescription map id.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the map type (e.g. "raster", "vector").</summary>
    public string? MapType { get; set; }

    /// <summary>Gets or sets the file path or storage reference for the map.</summary>
    public string? FilePath { get; set; }
}
