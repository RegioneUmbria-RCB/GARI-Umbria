using AgronicaNetCore.Base.Models;
using OutData.FoodMetaverse;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.IndicatoriSostenibilita;

/// <summary>
/// Service for retrieving sustainability indicators, aggregated by year.
/// See DS08-API: GET /v1/api/sostenibilita/indicatori-azienda-annuale (Design Specification 01KHXAFCB1651AWBC2Y884DNTR).
/// See DS08-API: GET /v1/api/sostenibilita/indicatori-prodotto-filiera (Design Specification 01KHXAFDCVCGSE6Y6NJFJ2C63V).
/// Depends on DS08-BL: PersistenzaRisultatiM4LookupTable (01KHXAFA2T3Y2SVH2TYF1TA671).
/// </summary>
public interface IIndicatoriSostenibilitaService
{
    /// <summary>
    /// Retrieves CO2 sustainability indicators for the given company (PIVA),
    /// aggregated by year. Applies best-effort logic: years with corrupt or missing M4 JSON are omitted.
    /// </summary>
    /// <param name="idAzienda">Company PIVA (VAT number).</param>
    /// <param name="objP">Database access parameters.</param>
    /// <returns>
    /// Response object with available year indicators.
    /// Returns <c>null</c> if no lookup records exist for the company (caller should return HTTP 404).
    /// Returns an object with an empty <c>Anni</c> list when records exist but all years failed extraction (HTTP 200 best-effort).
    /// </returns>
    Task<IndicatoriAziendaAnnualeResponse?> GetIndicatoriAziendaAnnualeAsync(
        string idAzienda,
        AgronicaCoreParametriServer objP);

    /// <summary>
    /// Retrieves CO2 sustainability indicators per product of supply chain,
    /// with company-level detail, historicised by year. Applies best-effort logic: years/companies
    /// with corrupt or missing M4 JSON data are omitted.
    /// See DS08-API: GET /v1/api/sostenibilita/indicatori-prodotto-filiera (Design Specification 01KHXAFDCVCGSE6Y6NJFJ2C63V).
    /// </summary>
    /// <param name="idFiliera">Supply chain identifier.</param>
    /// <param name="codProdotto">FMP product code (maps to <c>mat_cod</c> in lookup table).</param>
    /// <param name="objP">Database access parameters.</param>
    /// <returns>
    /// Response with per-company, per-year normalised GHG indicators (kg CO2 equiv/ha).
    /// Returns <c>null</c> when no records exist for the given filiera/product (caller returns HTTP 404).
    /// Returns a response with empty <c>Aziende</c> when records exist but all yield no valid indicators (HTTP 200 best-effort).
    /// </returns>
    Task<IndicatoriProdottoFilieraResponse?> GetIndicatoriProdottoFilieraAsync(
        string idFiliera,
        string codProdotto,
        AgronicaCoreParametriServer objP);

    /// <summary>
    /// Retrieves CO2 sustainability indicators for a single harvested lot
    /// (lotto raccolto), normalised per hectare and per kg of harvested product.
    /// Queries <c>Lookup_Sost_CO2_Colture_Chiavi</c> and <c>Lookup_Sost_CO2_Colture_Payload</c>
    /// in "Per Colture" mode, targeting the most recent invocation for the given lot.
    /// See DS08-API: FS2.08.4 Sost. CO2| API| Indicatori di Sintesi per Lotti Raccolti
    /// (Design Specification 01KHXAFE70RN423MWA4DYTNC6Q).
    /// Depends on DS08-BL: PersistenzaRisultatiM4LookupTable (01KHXAFA2T3Y2SVH2TYF1TA671).
    /// </summary>
    /// <param name="idFiliera">Supply chain identifier.</param>
    /// <param name="idAzienda">Company PIVA (VAT number).</param>
    /// <param name="codProdottoFmp">FMP product code.</param>
    /// <param name="codLottoFmp">FMP lot code.</param>
    /// <param name="objP">Database access parameters.</param>
    /// <returns>
    /// Response object with per-ha and per-kg normalised indicators.
    /// Returns <c>null</c> when no lookup record exists for the specified lot (caller returns HTTP 404).
    /// </returns>
    /// <exception cref="DivideByZeroException">
    /// Thrown when <c>tot_raccolto_kg = 0</c> (caller returns HTTP 500 with specific error message).
    /// </exception>
    Task<IndicatoriLottoRaccoltaResponse?> GetIndicatoriLottoRaccoltaAsync(
        string idFiliera,
        string idAzienda,
        string codProdottoFmp,
        string codLottoFmp,
        AgronicaCoreParametriServer objP);
}
