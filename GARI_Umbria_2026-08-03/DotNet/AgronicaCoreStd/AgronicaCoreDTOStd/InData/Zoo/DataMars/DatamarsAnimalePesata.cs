using Newtonsoft.Json;

namespace InData.Zoo.DataMars
{
    /// <summary>
    /// Rappresenta una singola pesata animale nel payload Datamars.
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI â€” Step 3 Lettura pesate per sessione;
    /// DS09-BL ValidazionePayloadJsonDatamars â€” Validazione Array Pesate.</para>
    /// </summary>
    public sealed class DatamarsAnimalePesata
    {
        /// <summary>Peso in kg dell'animale. Deve essere > 0 per superare la validazione.</summary>
        [JsonProperty("weight")]
        public decimal Weight { get; set; }

        /// <summary>
        /// Timestamp UTC della pesata in formato ISO 8601.
        /// Deve essere valido e non nel futuro.
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        /// <summary>Dati identificativi dell'animale.</summary>
        [JsonProperty("animal")]
        public DatamarsAnimaleIdentificativo Animal { get; set; }
    }

    /// <summary>
    /// Dati identificativi di un animale bovino nel payload Datamars.
    /// </summary>
    public sealed class DatamarsAnimaleIdentificativo
    {
        /// <summary>
        /// Lifetime Identifier (LID) dell'animale. Campo obbligatorio per la validazione.
        /// Struttura fissa: <c>{ "id": "uuid", "value": "IT004992384739", "type": "LID" }</c>.
        /// </summary>
        [JsonProperty("lifetimeIdentifierTag")]
        public DatamarsIdentificativoTag LifetimeIdentifierTag { get; set; }

        /// <summary>
        /// Electronic Identifier (EID) dell'animale. Campo opzionale.
        /// Struttura fissa: <c>{ "id": "uuid", "value": "380 004992384739", "type": "EID" }</c>.
        /// </summary>
        [JsonProperty("electronicIdentifierTag")]
        public DatamarsIdentificativoTag ElectronicIdentifierTag { get; set; }
    }
}
