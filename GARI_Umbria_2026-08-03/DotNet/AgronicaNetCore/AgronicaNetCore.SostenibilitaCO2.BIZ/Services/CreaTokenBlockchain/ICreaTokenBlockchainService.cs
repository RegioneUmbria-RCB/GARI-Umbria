using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.Models;
using InData.FoodMetaVerse;
using OutData.FoodMetaverse;
using System.Threading.Tasks;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.CreaTokenBlockchain
{
    /// <summary>
    /// Costruisce il payload M5 (<see cref="SostenibilitaCO2CarbonDataRequest"/>), lo invia
    /// al servizio Blockchain FMP e restituisce la risposta come
    /// <see cref="SostenibilitaCO2CreaTokenBlockchainResponse"/>.
    /// <para>
    /// Flusso: legge contesto da <c>Lookup_Sost_CO2_Aziendale_Chiavi</c>, usa il payload M4
    /// passato in input, interroga GIAS per metadati azienda e geometrie GIS,
    /// assembla il JSON M5 e lo invia in POST al servizio Blockchain.
    /// </para>
    /// Riferimento spec: DS10-BL CreaTokenBlockchain.
    /// </summary>
    public interface ICreaTokenBlockchainService
    {
        /// <summary>
        /// Costruisce il payload M5 e invoca il servizio Blockchain FMP.
        /// </summary>
        /// <param name="request">
        /// Request contenente PIVA azienda, id_invocazione e payload risposta M4.
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <param name="objParametriSuperServer">Parametri di connessione al database di configurazione (URL Blockchain).</param>
        /// <returns>Risposta ricevuta dal servizio Blockchain M5.</returns>
        /// <exception cref="Exceptions.DataNotFoundException">
        /// Azienda non trovata in tabella <c>Aziende</c> o invocazione non trovata in Lookup.
        /// </exception>
        /// <exception cref="Exceptions.PayloadStructureException">
        /// Response M4 manca campi obbligatori (<c>start_date</c>, <c>end_date</c>,
        /// <c>var_soc_soil_biogenic_carbon</c>) o azienda non presente nel payload.
        /// </exception>
        /// <exception cref="Exceptions.MissingGeometryException">
        /// Poligono appezzamento assente o geometria invalida. Errore bloccante, nessun fallback.
        /// </exception>
        Task<RispostaStandard> CreateTokenAsync(
            SostenibilitaCO2CreaTokenBlockchainRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
