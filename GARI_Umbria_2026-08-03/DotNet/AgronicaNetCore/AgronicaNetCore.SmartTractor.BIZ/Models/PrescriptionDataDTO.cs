namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Data Transfer Object carrying raw prescription data read from the legacy RICETTE* tables,
/// keyed by the GUID primary key <c>Ricetta_Operazione_Cod</c>.
/// Built by <c>PrescriptionService.ReadPrescriptionDataAsync()</c>.
///
/// Sources:
///   RICETTE_OPERAZIONI   → LavCod (activity type, Lav_Cod column)
///   RICETTE_DETTAGLI     → MachineryIds (Elem_Cod=1, Mat_Cod column)
///                          Prodotti     (Elem_Cod IN {0,3,10,191,210}, Pro_Cod ?? Mat_Cod column)
///   RICETTE_DESTINAZIONI → Destinazioni (destination plots)
/// </summary>
public class PrescriptionDataDTO
{
    /// <summary>Gets or sets the ricetta-operazione integer primary key (Ricetta_Operazione_Cod).</summary>
    public int RicettaOperazioneCod { get; set; }

    /// <summary>Gets or sets the lavoro code (Lav_Cod from RICETTE_OPERAZIONI).</summary>
    public int LavCod { get; set; }

    /// <summary>Gets or sets the machine integer ID (Mat_Cod from RICETTE_DETTAGLI).</summary>
    public IReadOnlyList<int> MachineryIds { get; set; } = Array.Empty<int>();

    /// <summary>Gets or sets the total area/quantity summed across all destinations.</summary>
    public float QuantitaDestinazioniTotale { get; set; }

    /// <summary>Gets or sets the destination plants (impianti) from RICETTE_DESTINAZIONI.</summary>
    public IReadOnlyList<PlantId> Destinazioni { get; set; } = Array.Empty<PlantId>();

    /// <summary>Gets or sets the products (prodotti) from RICETTE_DETTAGLI where Pro_Cod is not null.</summary>
    public IReadOnlyList<ProductDetails> Prodotti { get; set; } = Array.Empty<ProductDetails>();

    /// <summary>Gets or sets the UTC timestamp at which this DTO was built.</summary>
    public DateTime Timestamp { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the optional A-B guidance line in WKT format.
    /// When not null, it is included in the <c>abLine</c> section of the
    /// Smart Tractor payload.
    /// Referenced in Design Specification: DS03-BL — Rule 8.
    /// </summary>
    public string? AbLineWkt { get; set; }

    /// <summary>
    /// Gets or sets the prescription maps associated with this activity.
    /// When present and <c>include_prescription_maps=true</c>, each map is
    /// pre-uploaded to the provider before payload composition.
    /// Referenced in Design Specification: DS03-BL — Rule 6.
    /// </summary>
    public IReadOnlyList<PrescriptionMapInfo> PrescriptionMaps { get; set; } = Array.Empty<PrescriptionMapInfo>();
}

public class PlantId
{
    public string Piva { get; set; }
    public int SaCod { get; set; }
    public int Appezza { get; set; }
    public int IdReg { get; set; }
}

public class ProductDetails
{
    public int ElemCod { get; set; }
    public int Id { get; set; }
    public float Qty { get; set; }
    public int UoM { get; set; }
}
