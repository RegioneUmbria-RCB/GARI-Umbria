using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda
{
    /// <summary>
    /// HTTP request body for <c>POST /SincroWeb2App/DatiAziendaFromFlutter</c>.
    /// Ref: DS07-API – Formato Richiesta.
    /// </summary>
    public class DatiAziendaFromFlutterRequest
    {
        /// <summary>
        /// Company VAT number (codice fiscale azienda) – exactly 11 Italian digits or up to 16 for
        /// foreign entities.
        /// Ref: DS07-API – Sicurezza: Input Validation piva.
        /// </summary>
        [Required]
        [JsonProperty("piva")]
        public string Piva { get; set; } = null;

        /// <summary>
        /// ISO 8601 UTC timestamp of the client's last successful synchronisation.
        /// Ref: DS07-API – Formato Richiesta: timestamp_ultima_sincro.
        /// </summary>
        [JsonProperty("timestamp_ultima_sincro")]
        public string TimestampUltimaSincro { get; set; }

        /// <summary>
        /// Include supplier contacts, human resources and weather-station contacts.
        /// Ref: DS07-API – parametri_api: anagFornitori.
        /// </summary>
        [JsonProperty("anagFornitori")]
        public bool AnagFornitori { get; set; }

        /// <summary>
        /// Include crop plan (piano colturale) data.
        /// Ref: DS07-API – parametri_api: anagPianoColturale.
        /// </summary>
        [JsonProperty("anagPianoColturale")]
        public bool AnagPianoColturale { get; set; }

        /// <summary>
        /// Estrae i non movimentati ( i movimentati vengono estratti sempre)
        /// </summary>
        [JsonProperty("anagSementiNonMovimentati")]
        public bool AnagSementiNonMovimentati { get; set; }
        /// <summary>
        /// Estrae i non movimentati ( i movimentati vengono estratti sempre)
        /// </summary>
        [JsonProperty("anagFormulatiNonMovimentati")]
        public bool AnagFormulatiNonMovimentati { get; set; }
        /// <summary>
        /// Estrae i non movimentati ( i movimentati vengono estratti sempre)
        /// </summary>
        [JsonProperty("anagFertilizzantiNonMovimentati")]
        public bool AnagFertilizzantiNonMovimentati { get; set; }

        /// <summary>
        /// Estrae i non movimentati ( i movimentati vengono estratti sempre)
        /// </summary>
        [JsonProperty("anagTrasformatiVegetaliNonMovimentati")]
        public bool AnagTrasformatiVegetaliNonMovimentati { get; set; }

        //Porting da DatiComuni per estrazione Misure Avversita/Stadi x Anagrafica
        [JsonProperty("anagMetaschema")]
        public bool AnagMetaschema { get; set; } = true;

        //Porting da DatiComuni
        [JsonProperty("anagCodificaProdotti")]
        public bool AnagCodificaProdotti { get; set; }

        [JsonProperty("specieProdotti")]
        public string SpecieProdotti { get; set; }

        //Porting da DatiComuni, usato per i formulati
        [JsonProperty("nazione")]
        public string Nazione { get; set; }
    }
}

