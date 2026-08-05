using Newtonsoft.Json;
using System.Collections.Generic;

namespace InData.Zoo.DataMars
{
    /// <summary>
    /// Rappresenta la risposta di <c>GET /integrationSessions/{sessionId}</c>.
    /// Contiene l'array di tutte le pesate della sessione.
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI â€” Step 3 Lettura pesate per sessione;
    /// Architectural diagram (sezione risposta DataMars API GET /integrationSessions/{id}).</para>
    /// </summary>
    public sealed class DatamarsSessioneDettaglio
    {
        /// <summary>Identificativo univoco della sessione.</summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>Array di pesate animali acquisite in questa sessione.</summary>
        [JsonProperty("sessionAnimals")]
        public List<DatamarsAnimalePesata> SessionAnimals { get; set; } = new List<DatamarsAnimalePesata>();
    }
}
