using Newtonsoft.Json;

namespace InData.Zoo.DataMars
{
    /// <summary>
    /// Rappresenta un identificativo animale (LID o EID) nel formato oggetto ufficiale dell'API Datamars.
    /// <para>
    /// Struttura fissa secondo schema OpenAPI 3.1.0 Datamars Livestock Session API:
    /// <code>{ "id": "uuid", "value": "IT004992384739", "type": "LID" }</code>
    /// </para>
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI; DS09-BL ValidazionePayloadJsonDatamars.</para>
    /// </summary>
    public sealed class DatamarsIdentificativoTag
    {
        /// <summary>UUID interno Datamars dell'identificativo.</summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>Valore leggibile dell'identificativo (es. "IT004992384739" per LID, "380 004992384739" per EID).</summary>
        [JsonProperty("value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>Tipo di identificativo: "LID" oppure "EID".</summary>
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;
    }
}
