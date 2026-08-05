using AgronicaNetCore.Base.Models;
using InData.FoodMetaVerse;
using OutData.FoodMetaverse;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.IndicatoriSostenibilitaH2O
{
    /// <summary>
    /// Service that reads annual H2O sustainability indicators by company.
    /// See DS09-API Endpoint GET /sostenibilita_h2o/per_azienda_annuale.
    /// </summary>
    public interface ISostenibilitaH2OService
    {
        /// <summary>
        /// Retrieves H2O sustainability indicators for a filiera and optional azienda.
        /// Reads data from lookup_sost_h2o_aziendale_chiavi and lookup_sost_h2o_aziendale_payload.
        /// </summary>
        Task<SostenibilitaH2OAziendaAnnualeResponse?> GetPerAziendaAnnualeAsync(
            string idFiliera,
            string? idAzienda,
            AgronicaCoreParametriServer objP);

        /// <summary>
        /// Retrieves H2O sustainability indicators for a filiera product with optional azienda filter.
        /// Reads data from lookup_sost_h2o_lotto.
        /// See DS10-API Endpoint GET /sostenibilita_h2o/per_prodotto_di_filiera.
        /// </summary>
        Task<SostenibilitaH2OProdottoFilieraResponse?> GetPerProdottoDiFilieraAsync(
            string idFiliera,
            string codProdottoFmp,
            string? idAzienda,
            AgronicaCoreParametriServer objP);

        /// <summary>
        /// Retrieves H2O sustainability summary aggregated by harvested lots.
        /// Reads data from lookup_sost_h2o_lotto.
        /// See DS11-API Endpoint POST /sostenibilita_h2o/sintesi_per_lotti_raccolti.
        /// </summary>
        Task<SostenibilitaH2OLottiRaccoltiResponse> GetSintesiPerLottiRaccoltiAsync(
            SostenibilitaH2OLottiRaccoltiRequest request,
            AgronicaCoreParametriServer objP);
    }
}
