namespace AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente;

/// <summary>
/// Aggregated visibility filters for a user, collected from four distinct sources:
/// vegetable groups, crop species, varieties, and operations.
/// Ref: DS07-BL – Recupero Filtri Visibilità Correnti.
/// </summary>
public sealed class ParamVisibilitaUtente
{
    /// <summary>Codes of visible vegetable groups. Ref: DS07-BL – Visibilità GruppoVegetale.</summary>
    public List<int> CodiciGruppiVegetali { get; set; } = new();

    /// <summary>Codes of visible crop species. Ref: DS07-BL – Visibilità Specie Vegetali.</summary>
    public List<int> CodiciSpecieVegetali { get; set; } = new();

    /// <summary>Codes of visible varieties. Ref: DS07-BL – Visibilità Varietà.</summary>
    public List<int> CodiciVarieta { get; set; } = new();

    /// <summary>Codes of visible operation groups (integer identifiers). Ref: DS07-BL – Visibilità Operazioni/Lavorazioni.</summary>
    public List<int> CodiciGruppoOperazioni { get; set; } = new();

    /// <summary>Codes of visible operations (integer identifiers). Ref: DS07-BL – Visibilità Operazioni/Lavorazioni.</summary>
    public List<int> CodiciOperazioni { get; set; } = new();
}
