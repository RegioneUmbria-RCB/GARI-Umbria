namespace OutData.FoodMetaverse;

/// <summary>
/// Response DTO for <c>GET /v1/api/sostenibilita/indicatori-prodotto-filiera</c>.
/// See DS08-API: FS2.08.2 Sost. CO2| API| Indicatori per Prodotto di Filiera con Dettaglio Aziendale
/// (Design Specification 01KHXAFDCVCGSE6Y6NJFJ2C63V).
/// </summary>
public class IndicatoriProdottoFilieraResponse
{
    /// <summary>Identificativo Filiera.</summary>
    public string IdFiliera { get; set; } = string.Empty;

    /// <summary>Codice Prodotto FMP.</summary>
    public string CodProdotto { get; set; } = string.Empty;

    /// <summary>
    /// Indicatori per azienda (PIVA), ognuna con la propria storicizzazione annuale.
    /// Vuoto se nessuna azienda dispone di dati per il prodotto/filiera specificati.
    /// </summary>
    public List<IndicatoriAziendaFilieraDto> Aziende { get; set; } = new List<IndicatoriAziendaFilieraDto>();
}

/// <summary>
/// Indicatori CO2 per una singola azienda nell'ambito del prodotto di filiera.
/// See DS08-API spec section "Risposte" (01KHXAFDCVCGSE6Y6NJFJ2C63V).
/// </summary>
public class IndicatoriAziendaFilieraDto
{
    /// <summary>PIVA dell'azienda.</summary>
    public string IdAzienda { get; set; } = string.Empty;

    /// <summary>Indicatori per anno, ordinati per anno decrescente. Solo anni con dati validi.</summary>
    public List<IndicatoriAnnoColtureDto> Anni { get; set; } = new List<IndicatoriAnnoColtureDto>();
}

/// <summary>
/// Indicatori di sostenibilità CO2 normalizzati per ettaro per un singolo anno di campagna colturale.
/// Calcolati come media ponderata sulle colture in scope (DS08-API spec "Elaborazione" step 6).
/// </summary>
public class IndicatoriAnnoColtureDto
{
    /// <summary>Anno di riferimento della campagna (YYYY).</summary>
    public int Anno { get; set; }

    /// <summary>
    /// Emissioni GHG totali (Scope 1 + Scope 2 location-based + Scope 3) per ettaro (kg CO2 equiv/ha).
    /// Calcolato come: sum(scope_1 + scope_2_location_based + scope_3) / sum(area_ha) degli impianti in scope.
    /// </summary>
    public decimal GhgKgCo2EquivPerHa { get; set; }

    /// <summary>
    /// Variazione di biomassa biogenica per ettaro (kg CO2 equiv/ha).
    /// Calcolato come: sum(var_biomass_biogenic_carbon) / sum(area_ha) degli impianti in scope.
    /// </summary>
    public decimal VarBiomassBiogenicCarbonKgCo2EquivPerHa { get; set; }
}
