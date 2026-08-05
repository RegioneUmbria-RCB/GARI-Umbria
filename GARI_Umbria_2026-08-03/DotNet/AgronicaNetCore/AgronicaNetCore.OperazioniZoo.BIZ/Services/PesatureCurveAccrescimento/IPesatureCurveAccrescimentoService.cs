using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaNetCore.Base.Models;
using OutData.Zoo;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesatureCurveAccrescimento
{
    /// <summary>
    /// Contratto BIZ per il calcolo e il recupero delle metriche della curva di accrescimento bovini.
    /// Vedere DS06-API: Endpoint Query Metriche Curva Accrescimento.
    /// </summary>
    public interface IPesatureCurveAccrescimentoService
    {
        /// <summary>
        /// Recupera e calcola le metriche della curva di accrescimento per gli animali
        /// del contesto utente corrente, con paginazione e filtri dinamici.
        /// Vedere DS06-API, sezione "Specifiche Tecniche".
        /// </summary>
        /// <param name="queryParams">Parametri di filtro, calcolo e paginazione.</param>
        /// <param name="objParametriServer">Parametri server estratti dal JWT (include piva e username).</param>
        /// <param name="objParametriUtenti">Parametri utente estratti dal JWT (profilo, visibilità).</param>
        /// <returns>Dataset paginato di pesate con metriche e metadati.</returns>
        Task<OutData.Zoo.PesatureCurveAccrescimentoResult> LeggiPesatureCurveAccrescimentoAsync(
            PesatureCurveAccrescimentoQueryParams queryParams,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Recupera le pesature e le aggrega in memoria per gruppo (animale, razza o stalla).
        /// Usato dal widget dashboard (view_mode=aggregated).
        /// Vedere DS06-API, sezione "Widget Pesate e Accrescimento (Fase 2)".
        /// </summary>
        Task<OutData.Zoo.PesatureCurveAccrescimentoAggregatedResult> LeggiPesatureCurveAccrescimentoAggregatedAsync(
            PesatureCurveAccrescimentoQueryParams queryParams,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
