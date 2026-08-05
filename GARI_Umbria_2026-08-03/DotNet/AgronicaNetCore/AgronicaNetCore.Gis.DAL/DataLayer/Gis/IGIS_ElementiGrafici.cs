using AgronicaNetCore.Base.Models;
using System.Data;
using System.Threading.Tasks;

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    /// <summary>
    /// Recupera la geometria WKT e la proiezione EPSG di un appezzamento da
    /// <c>GIS_ElementiGrafici</c> + <c>GIS_Entita</c>.
    /// Necessario per la costruzione del payload M5 Blockchain.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Regola 8 (WKT, EPSG).
    /// </summary>
    public interface IGIS_ElementiGrafici
    {
        /// <summary>
        /// Restituisce la geometria WKT e la proiezione EPSG per l'appezzamento specificato.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda titolare dell'appezzamento.</param>
        /// <param name="appezzaCode">Codice numerico dell'appezzamento.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIS.</param>
        /// <returns>
        /// Tupla <c>(Wkt, Epsg)</c>. <c>Wkt</c> è <c>null</c> se la geometria è assente;
        /// <c>Epsg</c> è <c>null</c> se il sistema di riferimento non è disponibile (il chiamante
        /// deve applicare il default <c>EPSG:4326</c>).
        /// </returns>
        Task<(string? Wkt, string? Epsg)> GetPoligonoConEpsgAsync(
            string piva,
            int appezzaCode,
            AgronicaCoreParametriServer objParametriServer);
    }
}
