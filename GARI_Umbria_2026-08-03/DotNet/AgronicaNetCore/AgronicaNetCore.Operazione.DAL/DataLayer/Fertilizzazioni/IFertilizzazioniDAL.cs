using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Fertilizzazioni
{
    /// <summary>
    /// Contratto DAL per la lettura delle fertilizzazioni minerali precedenti di un appezzamento.
    /// Porta la logica equivalente a <c>Leggi_Macroelementi_Distribuiti_List_Id_Agenda_Esclusi()</c>
    /// (AgronicaCoreContabDAL) arricchita con la fase fenologica BBCH attiva al momento
    /// dell'applicazione da <c>Impianto_Fasi_Fenologiche_Engine</c>.
    /// Riferimento: DS04-BL — Regole di Business (Fertilizzazioni considerate).
    /// </summary>
    public interface IFertilizzazioniDAL
    {
        /// <summary>
        /// Restituisce le fertilizzazioni minerali (N, P, K) applicate all'appezzamento
        /// nell'intervallo <paramref name="dataAnalisi"/> (escluso) — <paramref name="dataConsiglio"/> (incluso),
        /// con il codice BBCH della fase fenologica attiva alla data di ciascuna applicazione.
        /// Ogni elemento non nullo genera una riga distinta nel DataTable risultante.
        /// </summary>
        /// <param name="lavCodList">Lista dei codici operazione (Lav_Cod) da considerare come fertilizzazioni.</param>
        /// <param name="piva">Partita IVA dell'impresa.</param>
        /// <param name="saCod">Codice centro aziendale.</param>
        /// <param name="appezza">Codice appezzamento.</param>
        /// <param name="idReg">Identificativo registro impianto.</param>
        /// <param name="dataAnalisi">Data inizio dell'ultima analisi terreno: limite inferiore esclusivo.</param>
        /// <param name="dataConsiglio">Data del consiglio nutrizionale: limite superiore inclusivo.</param>
        /// <param name="objParametriServer">Parametri server GIAS.</param>
        Task<DataTable> LeggiMacroelementiDistribuitiAsync(
            IReadOnlyList<int> lavCodList,
            string piva,
            int saCod,
            int appezza,
            int idReg,
            DateTime dataAnalisi,
            DateTime dataConsiglio,
            AgronicaCoreParametriServer objParametriServer);
    }
}
