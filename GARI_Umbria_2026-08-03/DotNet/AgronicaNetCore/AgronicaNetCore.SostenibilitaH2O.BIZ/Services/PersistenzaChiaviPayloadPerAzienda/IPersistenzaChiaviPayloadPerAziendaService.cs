using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.PersistenzaChiaviPayloadPerAzienda
{
    /// <summary>
    /// Contratto per la persistenza atomica di chiavi e payload di sostenibilità idrica
    /// nella modalità "Per Azienda".
    /// <para>
    /// Inserisce in modo transazionale una riga in <c>Lookup_Sost_H20_Aziendale_Chiavi</c>
    /// (metadati di aggregazione) e una riga correlata in <c>Lookup_Sost_H20_Aziendale_Payload</c>
    /// (payload JSON precalcolato), collegate tramite <c>id_invocazione</c>.
    /// </para>
    /// Riferimento spec: DS10-BL PersistenzaChiaviPayloadPerAzienda.
    /// </summary>
    public interface IPersistenzaChiaviPayloadPerAziendaService
    {
        /// <summary>
        /// Valida l'input, verifica che <c>payload_json</c> sia JSON valido, quindi esegue
        /// le due INSERT in un'unica transazione atomica. In caso di fallimento di una delle
        /// due INSERT viene eseguito il rollback dell'intera operazione.
        /// <para>
        /// Eccezioni attese:
        /// <list type="bullet">
        /// <item><description><see cref="Exceptions.InvalidJsonException"/>: payload_json non supera il parse test JSON.</description></item>
        /// <item><description><see cref="Exceptions.ForeignKeyViolationException"/>: violazione FK (SqlException error 547).</description></item>
        /// <item><description><see cref="Exceptions.TransactionFailureException"/>: qualsiasi altro errore di database — causa rollback.</description></item>
        /// </list>
        /// </para>
        /// Riferimento spec: DS10-BL — Regole di Business, Eccezioni.
        /// </summary>
        /// <param name="input">Metadati e payload da persistere.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="PersistenzaChiaviPayloadPerAziendaOutput"/> con gli identificativi delle righe inserite.
        /// </returns>
        Task<PersistenzaChiaviPayloadPerAziendaOutput> EseguiAsync(
            PersistenzaChiaviPayloadPerAziendaInput input,
            AgronicaCoreParametriServer objParametriServer);
    }
}
