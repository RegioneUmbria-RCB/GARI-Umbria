namespace AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente;

/// <summary>
/// Aggregated visibility filters for a user, collected from two distinct sources:
/// companies and company centers.
/// Ref: DS07-BL – Recupero Filtri Visibilità Correnti.
/// </summary>
public sealed class ParamVisibilitaAziendeUtente
{
    /// <summary>Codes of visible companies. Ref: DS07-BL – Visibilità Aziende.</summary>
    public List<string> Aziende { get; set; } = new();

    /// <summary>Codes of visible company centers. Ref: DS07-BL – Visibilità Centri Aziendali.</summary>
    public List<(string, int)> CentriAziendali { get; set; } = new();

}
