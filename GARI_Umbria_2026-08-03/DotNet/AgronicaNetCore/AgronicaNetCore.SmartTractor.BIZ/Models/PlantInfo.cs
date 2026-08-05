namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents a destination plant (impianto) from RICETTE_DESTINAZIONI,
/// identified by the composite key (Piva, SaCod, Appezza, IdReg).
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 5, FASE 6)
/// </summary>
public class PlantInfo
{
    /// <summary>Gets or sets the Partita IVA of the farm.</summary>
    public string Piva { get; set; } = string.Empty;

    /// <summary>Gets or sets the Sa_Cod value (from RICETTE_DESTINAZIONI).</summary>
    public int SaCod { get; set; }

    /// <summary>Gets or sets the Appezza value (from RICETTE_DESTINAZIONI).</summary>
    public int Appezza { get; set; }

    /// <summary>Gets or sets the Id_reg value (from RICETTE_DESTINAZIONI).</summary>
    public int IdReg { get; set; }

    /// <summary>Gets or sets the provider-specific code resolved from smart_tractor_provider_mapping_plant.</summary>
    public string? ProviderCode { get; set; }
}
