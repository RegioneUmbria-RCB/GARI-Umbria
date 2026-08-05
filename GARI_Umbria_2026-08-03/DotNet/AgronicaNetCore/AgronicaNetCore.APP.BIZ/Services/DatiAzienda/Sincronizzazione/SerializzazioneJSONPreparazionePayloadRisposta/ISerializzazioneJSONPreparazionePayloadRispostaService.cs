using AgronicaNetCore.APP.BIZ.Exceptions;
using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.SerializzazioneJSONPreparazionePayloadRisposta;

/// <summary>
/// Serializes the final Dati Azienda payload into the HTTP-ready response contract.
/// Ref: DS05-BL – Nome: SerializzazioneJSONPreparazionePayloadRisposta.
/// </summary>
public interface ISerializzazioneJSONPreparazionePayloadRispostaService
{
    /// <summary>
    /// Produces the final response payload by validating the DS01 timestamp, serializing the
    /// ordered company-data structure, and preparing the response headers.
    /// Ref: DS05-BL – Descrizione; Regole di Business; Output.
    /// </summary>
    /// <exception cref="InvalidTimestampException">
    /// Thrown when <see cref="SerializzazioneJSONPreparazionePayloadRispostaInput.TimestampSincronizzazione"/>
    /// is null, empty, or not a valid ISO 8601 timestamp.
    /// Ref: DS05-BL – Eccezioni: InvalidTimestampException.
    /// </exception>
    /// <exception cref="JsonSerializationException">
    /// Thrown when the payload cannot be serialized to JSON.
    /// Ref: DS05-BL – Eccezioni: JsonSerializationException.
    /// </exception>
    SerializzazioneJSONPreparazionePayloadRispostaResult Prepara(
        SerializzazioneJSONPreparazionePayloadRispostaInput input);
}