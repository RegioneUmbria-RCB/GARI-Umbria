using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.RilievoFasiFenologiche
{
    /// <summary>
    /// Contratto DAL per l'estrazione cronologica delle fasi fenologiche registrate su un impianto/appezzamento.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS20-BL GetPhenologicalPhasesChronological — Estrazione Fasi Fenologiche Cronologiche per Appezzamento.
    /// </remarks>
    public interface IRilievoFasiFenologiche
    {
        /// <summary>
        /// Estrae l'elenco completo delle fasi fenologiche registrate sull'impianto nell'intervallo di date indicato,
        /// ordinato cronologicamente in modo ascendente, con deduplicazione per (CodBbch, Data_Movimento).
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda (obbligatoria).</param>
        /// <param name="saCod">Codice centro aziendale.</param>
        /// <param name="appezza">Codice appezzamento.</param>
        /// <param name="idReg">Codice registro impianto (ID_REG).</param>
        /// <param name="dataInizio">Data inizio intervallo per il filtro su Data_Movimento.</param>
        /// <param name="dataFine">Data fine intervallo per il filtro su Data_Movimento.</param>
        /// <param name="objParametriServer">Parametri di connessione al server.</param>
        /// <returns>
        /// <see cref="DataTable"/> con le colonne: <c>CodBbch</c> (int), <c>DescrizioneBbch</c> (string), <c>Data_Movimento</c> (DateTime).
        /// </returns>
        /// <remarks>
        /// Design Specification: DS20-BL — Query Movimenti Fenologici; Ordinamento e Deduplica.
        /// Filtri applicati: Agenda.Lav_Cod = 79 (LAVCOD_FASI_FENOLOGICHE), Movimenti.Cau_Mov = '2100' (CAU_RILIEVO_CAMPO).
        /// Deduplica: a parità di (ff_classe, Data_Movimento), viene mantenuta la riga con Data_Creazione più recente.
        /// </remarks>
        Task<DataTable> LeggiAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            DateTime dataInizio,
            DateTime dataFine,
            AgronicaCoreParametriServer objParametriServer);
    }
}
