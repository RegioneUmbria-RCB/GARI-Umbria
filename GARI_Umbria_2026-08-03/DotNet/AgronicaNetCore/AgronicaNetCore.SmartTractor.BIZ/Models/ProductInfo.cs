namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents a product associated with an activity, sourced from RICETTE_DETTAGLI.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività,
/// DS03-BL - Composizione Payload (PHASE 6 — Prodotti Condizionali)
/// </summary>
public class ProductInfo
{
    /// <summary>Gets or sets the product identifier (Pro_Cod from RICETTE_DETTAGLI).</summary>
    public int Id { get; set; }

    public int ElemCod { get; set; }

    /// <summary>Gets or sets the product quantity (Qta from RICETTE_DETTAGLI).</summary>
    public float Quantity { get; set; }

    /// <summary>
    /// Gets or sets the provider-specific product code resolved from
    /// <c>smart_tractor_provider_mapping_product</c>.
    /// Populated during DS03-BL double-check mapping phase (PHASE 6).
    /// Referenced in Design Specification: DS03-BL — Rule 5.
    /// </summary>
    public string? ProviderCode { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure for the product quantity
    /// (e.g. "kg/ha", "l/ha") as expected by the Smart Tractor API.
    /// Referenced in Design Specification: DS03-BL — PHASE 6.
    /// </summary>
    public int? UnitOfMeasure { get; set; }
}
