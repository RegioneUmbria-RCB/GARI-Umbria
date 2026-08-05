using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Indicatori di sostenibilità CO2 per un allevamento restituiti dal motore M4.
    /// Riferimento spec: DS07-API — Risposta 200, array <c>aziende[].allevamenti[]</c>.
    /// Struttura da completare quando il servizio M4 fornirà dati di allevamento.
    /// </summary>
    public class AllevamentoRispostaCo2
    {
        [JsonPropertyName("id_allevamento")]
        public string IdAllevamento { get; set; } = string.Empty;
    }
}
