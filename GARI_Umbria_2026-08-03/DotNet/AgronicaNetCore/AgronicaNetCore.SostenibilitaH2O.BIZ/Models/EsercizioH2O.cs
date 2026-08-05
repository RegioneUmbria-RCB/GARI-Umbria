using Newtonsoft.Json;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Esercizio (unità territoriale) incluso nel calcolo del bilancio idrico.
    /// Riferimento spec: DS-02.2-BL Chiamata WebApi Calcolo Bilancio Idrico — Input Esercizio.
    /// </summary>
    public class EsercizioH2O
    {
        /// <summary>Identificativo numerico dell'esercizio (Progetto_Cod in GIAS).</summary>
        [JsonProperty("id_esercizio")]
        public int IdEsercizio { get; set; }

        /// <summary>Codice ISO Alpha-3 della nazione (es. <c>ITA</c>). Usato da DS06 per il lookup benchmark Water Footprint.</summary>
        [JsonProperty("nazione")]
        public string Nazione { get; set; } = string.Empty;

        /// <summary>Codice ISTAT della regione. Usato da DS06 come regionCode opzionale.</summary>
        [JsonProperty("istat_reg")]
        public string IstatRegione { get; set; } = string.Empty;

        // ── Campi aggiuntivi per la pipeline completa DS07→DS10 ──────────────────

        /// <summary>PIVA dell'azienda cui appartiene l'esercizio. Usata per raggruppamento aziendale (DS09, DS10).</summary>
        public string Azienda { get; set; } = string.Empty;

        /// <summary>Identificativo dell'appezzamento. Usato dalla persistenza DS08.</summary>
        public int Appezzamento { get; set; }

        /// <summary>Codice varietà/coltura leggibile. Usato da DS08, DS09, DS10.</summary>
        public string Varieta { get; set; } = string.Empty;

        /// <summary>
        /// Superficie dell'appezzamento in ettari (ha).
        /// Usata dalla formula DS07 e dalla persistenza DS08/DS09/DS10. Deve essere &gt; 0.
        /// </summary>
        public decimal SuperficieHa { get; set; }

        /// <summary>Codice del lotto di raccolta (opzionale). Usato da DS08.</summary>
        public string? LottoRaccolta { get; set; }

        /// <summary>Specie colturale (opzionale, es. <c>Vitis vinifera</c>). Usata da DS10.</summary>
        public string? Specie { get; set; }
    }
}
