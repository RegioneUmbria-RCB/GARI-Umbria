namespace OutData.FoodMetaverse;

/// <summary>
/// Response DTO for <c>GET /v1/api/sostenibilita/indicatori-lotto-raccolto</c>.
/// Returns CO2 sustainability indicators for a single harvested lot (lotto raccolto),
/// normalised per hectare and per kg of harvested product.
/// See DS08-API: FS2.08.4 Sost. CO2| API| Indicatori di Sintesi per Lotti Raccolti
/// (Design Specification 01KHXAFE70RN423MWA4DYTNC6Q).
/// </summary>
public class IndicatoriLottoRaccoltaResponse
{
    /// <summary>Identificativo Filiera.</summary>
    public string IdFiliera { get; set; } = string.Empty;

    /// <summary>PIVA dell'azienda.</summary>
    public string IdAzienda { get; set; } = string.Empty;

    /// <summary>Codice Prodotto FMP.</summary>
    public string CodProdotto { get; set; } = string.Empty;

    /// <summary>Codice Lotto FMP.</summary>
    public string CodLotto { get; set; } = string.Empty;

    /// <summary>
    /// GHG Scope 1 emissions per hectare (kg CO2 equiv/ha).
    /// Calcolato come: resp_esercizio.scope_1 / resp_esercizio.area_ha.
    /// </summary>
    public decimal GhgScope1KgCo2EquivPerHa { get; set; }

    /// <summary>
    /// GHG Scope 3 emissions per hectare (kg CO2 equiv/ha).
    /// Calcolato come: resp_esercizio.scope_3 / resp_esercizio.area_ha.
    /// </summary>
    public decimal GhgScope3KgCo2EquivPerHa { get; set; }

    /// <summary>
    /// Variazione di biomassa biogenica per ettaro (kg CO2 equiv/ha).
    /// Calcolato come: resp_esercizio.var_biomass_biogenic_carbon / resp_esercizio.area_ha.
    /// </summary>
    public decimal VarBiomassBiogenicCarbonKgCo2EquivPerHa { get; set; }

}
