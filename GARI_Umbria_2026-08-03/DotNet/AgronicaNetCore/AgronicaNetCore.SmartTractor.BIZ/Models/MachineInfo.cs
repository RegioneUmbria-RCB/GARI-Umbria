namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents a machine (macchina) looked up from smart_tractor_macchine joined with its
/// provider mapping. The integer ID corresponds to Mat_Cod in RICETTE_DETTAGLI.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 4),
/// DS03-BL - Composizione Payload (PHASE 8 — Campi Root da Oggetto Macchina)
/// </summary>
public class MachineInfo
{
    /// <summary>Gets or sets the machine integer ID (Mat_Cod from RICETTE_DETTAGLI).</summary>
    public int MacCod { get; set; }

    /// <summary>Gets or sets the machine model.</summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the provider brand code (e.g. "CNH", "CLAAS") used as
    /// <c>providerCode</c> in the Smart Tractor payload root.
    /// Referenced in Design Specification: DS03-BL — PHASE 8, Rule 9.
    /// </summary>
    public string? ProviderCode { get; set; }
}
