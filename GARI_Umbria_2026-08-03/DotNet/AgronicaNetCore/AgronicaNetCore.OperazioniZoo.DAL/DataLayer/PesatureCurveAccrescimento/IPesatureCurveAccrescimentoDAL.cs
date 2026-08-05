using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaNetCore.Base.Models;
using OutData.Zoo;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesatureCurveAccrescimento
{
    /// <summary>
    /// Contratto DAL per il recupero dei dati di pesatura a supporto
    /// del calcolo della curva di accrescimento bovini.
    /// Vedere DS06-API: Endpoint Query Metriche Curva Accrescimento.
    /// </summary>
    public interface IPesatureCurveAccrescimentoDAL
    {
        /// <summary>
        /// Recupera le pesate degli animali con i dati necessari al calcolo delle metriche
        /// della curva di accrescimento (eta, peso teorico, scostamento).
        /// Supporta filtro per stalla, razza e range temporale con paginazione.
        /// Vedere DS06-API, sezione "Specifiche Tecniche".
        /// </summary>
        /// <param name="queryParams">Parametri di filtro e paginazione.</param>
        /// <param name="pivaJwt">Piva estratto dal JWT (usato per sicurezza e filtro dati).</param>
        /// <param name="usernameJwt">Username estratto dal JWT (usato per visibilità stalle).</param>
        /// <param name="visibilitaTotale">
        /// Se true l'utente ha visibilità totale e il filtro utenti_Visibilita_Appoggio viene saltato.
        /// </param>
        /// <param name="objParametriServer">Parametri server correnti (cultura, finestra temporale, ecc.).</param>
        /// <returns>Tupla con le righe della pagina corrente e il conteggio totale non paginato.</returns>
        Task<(IReadOnlyList<OutData.Zoo.PesaturaCurvaAccrescimentoRow> Rows, int TotalCount)> LeggiPesatureCurveAccrescimentoAsync(
            PesatureCurveAccrescimentoQueryParams queryParams,
            string pivaJwt,
            string usernameJwt,
            bool visibilitaTotale,
            AgronicaCoreParametriServer objParametriServer);
    }
}
