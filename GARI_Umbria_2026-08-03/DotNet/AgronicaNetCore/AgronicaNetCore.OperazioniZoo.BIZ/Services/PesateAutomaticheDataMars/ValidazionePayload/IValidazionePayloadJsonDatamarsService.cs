using InData.Zoo.DataMars;
using OutData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.ValidazionePayload;

/// <summary>
/// Contratto per la validazione del payload JSON ricevuto dall'API Datamars.
/// <para>Riferimento spec: DS09-BL ValidazionePayloadJsonDatamars.</para>
/// </summary>
public interface IValidazionePayloadJsonDatamarsService
{
    /// <summary>
    /// Valida il dettaglio di una sessione Datamars applicando tutte le regole di business:
    /// LID obbligatorio, peso &gt; 0, timestamp ISO 8601 non nel futuro, array non vuoto,
    /// deduplicazione per LID/giorno (mantieni timestamp più recente).
    /// </summary>
    /// <param name="sessioneDettaglio">Risposta dell'endpoint GET /integrationSessions/{sessionId}.</param>
    /// <param name="idSessione">Identificativo della sessione, usato nei messaggi di errore.</param>
    ValidazionePayloadResult ValidaPayload(DatamarsSessioneDettaglio sessioneDettaglio, string idSessione);
}
