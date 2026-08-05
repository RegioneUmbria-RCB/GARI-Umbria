using InData.Zoo.DataMars;
using System.Collections.Generic;

namespace OutData.Zoo.DataMars
{
    /// <summary>
    /// DTO di output della validazione del payload JSON ricevuto dall'API Datamars.
    /// <para>Riferimento spec: DS09-BL ValidazionePayloadJsonDatamars â€” Output.</para>
    /// </summary>
    public sealed class ValidazionePayloadResult
    {
        /// <summary>Indica se la validazione ha prodotto almeno una pesata utilizzabile.</summary>
        public bool ValidazioneOK { get; set; }

        /// <summary>
        /// Stato complessivo della validazione.
        /// Valori: <c>VALIDA</c>, <c>ERRORI_PARZIALI</c>, <c>ERRORE_TOTALE</c>.
        /// </summary>
        public string StatusValidazione { get; set; } = string.Empty;

        /// <summary>Numero di pesate che hanno superato tutte le regole di validazione.</summary>
        public int NumPesateValidate { get; set; }

        /// <summary>Numero di pesate scartate per errori di validazione.</summary>
        public int NumPesateScartate { get; set; }

        /// <summary>
        /// Lista delle pesate valide (dopo deduplicazione per LID/giorno)
        /// da persistere in STAGING_PESATE.
        /// </summary>
        public List<DatamarsAnimalePesata> PesateValide { get; set; } = new List<DatamarsAnimalePesata>();

        /// <summary>Numero di warning non bloccanti rilevati durante la validazione.</summary>
        public int WarningCount { get; set; }

        /// <summary>Dettaglio dei warning per singola pesata.</summary>
        public List<ValidazioneWarning> WarningDettagli { get; set; } = new List<ValidazioneWarning>();

        /// <summary>Messaggio d'errore in caso di fallimento totale; null se validazione OK.</summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Warning non bloccante rilevato sulla singola pesata durante la fase di validazione.
    /// <para>Riferimento spec: DS09-BL ValidazionePayloadJsonDatamars â€” Output (warningDettagli).</para>
    /// </summary>
    public sealed class ValidazioneWarning
    {
        /// <summary>Indice (0-based) della pesata nell'array originale della sessione.</summary>
        public int PesataIndex { get; set; }

        /// <summary>Descrizione del warning (es. EID non valorizzato, animale non in anagrafica).</summary>
        public string Warning { get; set; } = string.Empty;
    }
}
