namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.SalvataggioConsiglio.Models
{
    /// <summary>
    /// Risposta prodotta da DS07-BL Salvataggio Consiglio Nutrizione.
    /// Riferimento: DS07-BL — Output; DS16-API POST /v1/dss/nutrizione/consigli/salva — Risposte.
    /// </summary>
    public sealed class SalvataggioConsiglioNutrizioneResponse
    {
        /// <summary>
        /// Identificativo del primo record inserito in Consigli_Nutrizione_Engine.
        /// Valorizzato anche nel caso DUPLICATE con il valore 0.
        /// </summary>
        public int SalvataggioId { get; init; }

        /// <summary>
        /// Esito dell'operazione: SUCCESS | DUPLICATE | ERROR.
        /// DUPLICATE indica che esistono già record per la stessa chiave
        /// (PIVA, SA_COD, APPEZZA, ID_REG, Data_Consiglio).
        /// </summary>
        public string Esito { get; init; } = string.Empty;

        /// <summary>Messaggio descrittivo dell'esito localizzato.</summary>
        public string Messaggio { get; init; } = string.Empty;

        /// <summary>Data e ora di salvataggio in formato ISO 8601.</summary>
        public string DataSalvataggio { get; init; } = string.Empty;
    }
}
