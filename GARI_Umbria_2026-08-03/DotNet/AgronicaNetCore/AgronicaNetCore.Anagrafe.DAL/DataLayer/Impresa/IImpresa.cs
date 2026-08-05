using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa
{
    public interface IImpresa
    {
        Task<DataTable?> LeggiAsync(string piva, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiPadriAsync(DataTable dtImpreseVisibili, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiImpreseAsync(string piva, DataTable dtImpreseVisibili, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiClausolaInAsync(List<string> elencoPiva, List<int> elencoVegCod, int varieta, AgronicaCoreParametriServer objParametriServer);

        Task<string> PivaFromCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer);

        Task<string> CuaaFromPivaAsync(string piva, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Conta il numero totale di aziende attive nel sistema (<c>inviato = 1</c>).
        /// Utilizzato da DS04-BL per il confronto real-time con il numero di aziende visibili per l'utente.
        /// </summary>
        Task<int> ContaTotaleImpreseAsync(AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera Ragione Sociale, Città, Regione e Stato ISO 3166-1 alpha-3 per l'azienda specificata.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="DataTable"/> con colonne <c>RagioneSociale</c>, <c>Citta</c>, <c>Regione</c>, <c>Stato_ISO3</c>.
        /// Zero righe se l'azienda non è trovata.
        /// </returns>
        Task<DataTable> LeggiMetadatiImpresaBlockchainAsync(string piva, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera latitudine (id_cod = <c>IMPRESE_CODICI.LATITUDINE</c>) e longitudine
        /// (id_cod = <c>IMPRESE_CODICI.LONGITUDINE</c>) dell'azienda da <c>Imprese_Codici</c>
        /// con un'unica query pivot ottimizzata.
        /// Restituisce <c>(0, 0)</c> se nessun record è presente.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>Tupla <c>(Latitudine, Longitudine)</c> in <see cref="decimal"/>.</returns>
        Task<(decimal Latitudine, decimal Longitudine)> GetCoordinateCentroideAsync(string piva, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera il codice nazione dell'azienda dall'indirizzo di sede operativa.
        /// Restituisce stringa vuota se nessun indirizzo valido e' presente oppure se il campo stato non e' valorizzato.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>Codice ISO 3166-1 alpha-2 della nazione oppure stringa vuota.</returns>
        Task<string> GetNazioneAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
