using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.PersistenzaRisultatiCo2
{
    /// <summary>
    /// Contratto per la business logic di persistenza dei risultati M4 nelle tabelle di lookup
    /// (DS08-BL PersistenzaRisultatiM4LookupTable).
    /// <para>
    /// Separa la persistenza per modalità "Per Colture" (<c>Lookup_Sost_CO2_Colture_Chiavi</c> +
    /// <c>Lookup_Sost_CO2_Colture_Payload</c>) e "Aziendale" (<c>Lookup_Sost_CO2_Aziendale_Chiavi</c> +
    /// <c>Lookup_Sost_CO2_Aziendale_Payload</c>). Ogni esecuzione è atomica tramite
    /// <see cref="System.Transactions.TransactionScope"/>.
    /// </para>
    /// Riferimento spec: DS08-BL PersistenzaRisultatiM4LookupTable.
    /// </summary>
    public interface IPersistenzaRisultatiCo2Service
    {
        /// <summary>
        /// Persiste il payload di richiesta e la risposta M4 nelle tabelle di lookup appropriate
        /// in base alla modalità indicata nell'input.
        /// </summary>
        /// <param name="input">
        /// Payload M4 validato (request + response), modalità e contesto di invocazione.
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="PersistenzaRisultatiCo2Output"/> con esito, identificativo e tabelle populate.
        /// </returns>
        /// <exception cref="Exceptions.SostenibilitaCO2.SerializationException">
        /// Se il payload non è serializzabile a JSON (DS08-BL — SerializationException).
        /// </exception>
        /// <exception cref="System.Data.Common.DbException">
        /// Se la scrittura sul database fallisce (DS08-BL — DatabaseWriteException).
        /// </exception>
        Task<PersistenzaRisultatiCo2Output> EseguiAsync(
            PersistenzaRisultatiCo2Input input,
            AgronicaCoreParametriServer objParametriServer);
    }
}
