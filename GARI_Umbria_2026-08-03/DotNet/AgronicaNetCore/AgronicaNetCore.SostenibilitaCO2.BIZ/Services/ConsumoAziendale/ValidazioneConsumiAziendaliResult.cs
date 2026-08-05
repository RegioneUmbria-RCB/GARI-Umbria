using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ConsumoAziendale
{
    /// <summary>
    /// Risultato della business logic <c>ValidazioneDatiConsumoAziendale</c> (DS02-BL).
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Output.
    /// </summary>
    public class ValidazioneConsumiAziendaliResult
    {
        /// <summary><c>true</c> se tutti i dati superano la validazione; <c>false</c> in presenza di errori.</summary>
        public bool ValidazioneEsito { get; set; }

        /// <summary>
        /// Elenco degli errori rilevati. Vuoto se <see cref="ValidazioneEsito"/> è <c>true</c>.
        /// </summary>
        public List<ErroreValidazione> Errori { get; set; } = new();

        /// <summary>
        /// Dati validati pronti per DS03-BL. Valorizzato solo se <see cref="ValidazioneEsito"/> è <c>true</c>;
        /// <c>null</c> in presenza di errori.
        /// </summary>
        public ConsumiValidati? ConsumiValidati { get; set; }
    }

    /// <summary>
    /// Dettaglio di un singolo errore di validazione.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Output <c>errori[]</c>.
    /// </summary>
    public class ErroreValidazione
    {
        /// <summary>
        /// Tipo di errore.
        /// Valori ammessi: <c>FIELD_REQUIRED</c> | <c>OUT_OF_RANGE</c> | <c>INVALID_FORMAT</c> |
        /// <c>AZIENDA_NOT_IN_PERIMETRO</c> | <c>DATE_OVERLAP</c>.
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Tabella di provenienza: <c>carburanti</c> | <c>energia</c>.</summary>
        public string Tabella { get; set; } = string.Empty;

        /// <summary>Indice 0-based della riga nella tabella.</summary>
        public int Riga { get; set; }

        /// <summary>Nome del campo che non ha superato la validazione.</summary>
        public string Campo { get; set; } = string.Empty;

        /// <summary>Messaggio leggibile dell'errore.</summary>
        public string Messaggio { get; set; } = string.Empty;

        /// <summary>Valore immesso dall'utente convertito in stringa; <c>null</c> se assente.</summary>
        public string? ValoreAttuale { get; set; }
    }
}
