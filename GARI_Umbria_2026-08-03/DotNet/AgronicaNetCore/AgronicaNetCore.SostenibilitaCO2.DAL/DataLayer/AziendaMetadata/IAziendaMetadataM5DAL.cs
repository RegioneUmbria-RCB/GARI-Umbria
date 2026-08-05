using AgronicaNetCore.Base.Models;
using System.Data;
using System.Threading.Tasks;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.AziendaMetadata
{
    /// <summary>
    /// Recupera i metadati anagrafici di un'azienda dalla tabella <c>Aziende</c>
    /// necessari per la costruzione del payload M5 Blockchain.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Regole 5, 6.
    /// </summary>
    public interface IAziendaMetadataM5DAL
    {
        /// <summary>
        /// Recupera Ragione Sociale, Città, Regione e Stato ISO 3166-1 alpha-3 per l'azienda specificata.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="DataTable"/> con colonne <c>RagioneSociale</c>, <c>Citta</c>, <c>Regione</c>, <c>Stato_ISO3</c>.
        /// Zero righe se l'azienda non è trovata.
        /// </returns>
        Task<DataTable> LeggiMetadatiAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
