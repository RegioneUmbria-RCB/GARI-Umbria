using System;

namespace OutData.Zoo.DataMars
{
    /// <summary>
    /// Risultato della validazione anagrafica animale e mapping stalla.
    /// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla â€” Output.</para>
    /// </summary>
    public sealed class ValidazioneAnagraficaResult
    {
        /// <summary>True se la validazione ha avuto esito positivo.</summary>
        public bool ValidazioneOK { get; set; }

        /// <summary>Cod_Progetto da ZOO_ANIMALI; null se la validazione Ã¨ fallita.</summary>
        public int? CodProgetto { get; set; }

        /// <summary>Sa_Cod della stalla GIAS mappata dal FarmID; null se mappatura fallita.</summary>
        public int? SaCod { get; set; }

        /// <summary>Sta_Num della stalla GIAS mappata dal FarmID; null se mappatura fallita.</summary>
        public int? StaNum { get; set; }

        /// <summary>PIVA dell'azienda GIAS ricavata dalla stalla mappata; null se mappatura fallita.</summary>
        public string Piva { get; set; }

        /// <summary>
        /// Codice stato di validazione.
        /// Valori attesi: OK, ANIMAL_NOT_FOUND, ANIMAL_INACTIVE, STALLA_MAPPING_FAILED, INVALID_COD_PROGETTO.
        /// </summary>
        public string StatusValidazione { get; set; } = string.Empty;

        /// <summary>Messaggio descrittivo dell'esito.</summary>
        public string Messaggio { get; set; } = string.Empty;

        /// <summary>Nome del tipo di eccezione in caso di errore; null se OK.</summary>
        public string ErrorType { get; set; }

        /// <summary>Messaggio tecnico dell'eccezione; null se OK.</summary>
        public string ErrorMessage { get; set; }

        // â”€â”€ Campi per cache temporale (usati internamente dal service di validazione) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        /// <summary>Data inizio validitÃ  anagrafica animale; null se validazione fallita.</summary>
        public DateTime? ValiditaInizio { get; set; }

        /// <summary>Data fine validitÃ  anagrafica animale; null se validazione fallita.</summary>
        public DateTime? ValiditaFine { get; set; }
    }
}
