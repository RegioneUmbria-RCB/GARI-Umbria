using System.Text.Json.Serialization;

namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Response model for a raster map upload to the Smart Tractor provider API.
/// Referenced in Design Specification: DS03-BLc - Lettura e invio mappa raster associata (PHASE 3).
/// </summary>
public class UploadRasterMapEngineResponse
{
    /// <summary>Gets or sets the provider-assigned attachment identifier.</summary>
    public string id { get; set; } = string.Empty;

    public UploadRasterMapEngineResponse() { }
    public UploadRasterMapEngineResponse(string id) { this.id = id; }
}

