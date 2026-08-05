using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace InData.Engine.FasiFenologiche
{
    /// <summary>
    /// DTO di input per l'endpoint di acquisizione fasi fenologiche.
    /// Mappa il corpo JSON della richiesta definito nelle specifiche API.
    /// </summary>
    /// <remarks>
    /// DS07-API: POST /Engine/AcquisizioneFasiFenologiche — Formato Richiesta.
    /// </remarks>
    public class AcquisizioneFasiFenologicheRequest
    {
        /// <summary>
        /// Identificatori dell'impianto/appezzamento oggetto della richiesta.
        /// DS07: impianto (obbligatorio) — piva, sa_cod, appezza, id_reg.
        /// </summary>
        [Required]
        [JsonProperty("impianto")]
        public ImpiantoInput Impianto { get; set; } = null;

        /// <summary>
        /// Tipo di richiesta fenologica (es. "lungo_termine"). Obbligatorio.
        /// DS07: tipo_richiesta (obbligatorio, default: 'lungo_termine').
        /// </summary>
        [Required]
        [JsonProperty("tipo_richiesta")]
        public string TipoRichiesta { get; set; } = "lungo_termine";

        /// <summary>
        /// Standard di codifica coltura: 1=Profitosan/GIAS, 2=AGEA, 3=EPPO. Default: 1.
        /// DS07: tipo_codice_coltura (opzionale, valori: 1|2|3, default: 1).
        /// </summary>
        [JsonProperty("tipo_codice_coltura")]
        public int? TipoCodiceColtura { get; set; } = 1;

        /// <summary>
        /// Identificativo della coltura secondo lo standard selezionato. Obbligatorio.
        /// DS07: colturaId (obbligatorio).
        /// </summary>
        [Required]
        [JsonProperty("colturaId")]
        public string ColturaId { get; set; }

        /// <summary>
        /// Identificativo della varietà secondo lo standard selezionato. Obbligatorio.
        /// DS07: varietaId (obbligatorio).
        /// </summary>
        [Required]
        [JsonProperty("varietaId")]
        public string VarietaId { get; set; }

        /// <summary>
        /// Latitudine del centroide dell'appezzamento. Obbligatorio. Range: -90.0 a 90.0.
        /// DS07: latitudine (obbligatorio, range: -90.0 to 90.0).
        /// </summary>
        [Required]
        [Range(-90.0, 90.0, ErrorMessage = "Latitudine fuori range valido (-90.0 to 90.0).")]
        [JsonProperty("latitudine")]
        public decimal Latitudine { get; set; }

        /// <summary>
        /// Longitudine del centroide dell'appezzamento. Obbligatorio. Range: -180.0 a 180.0.
        /// DS07: longitudine (obbligatorio, range: -180.0 to 180.0).
        /// </summary>
        [Required]
        [Range(-180.0, 180.0, ErrorMessage = "Longitudine fuori range valido (-180.0 to 180.0).")]
        [JsonProperty("longitudine")]
        public decimal Longitudine { get; set; }

        /// <summary>
        /// Data di semina in formato ISO 8601. Opzionale; richiesta per colture annuali.
        /// DS07: data_semina (opzionale, richiesta per colture annuali).
        /// </summary>
        [JsonProperty("data_semina")]
        public string DataSemina { get; set; }

        /// <summary>
        /// Data della richiesta in formato ISO 8601. Obbligatoria.
        /// DS07: data_richiesta (obbligatorio).
        /// </summary>
        [Required]
        [JsonProperty("data_richiesta")]
        public string DataRichiesta { get; set; }

        /// <summary>
        /// Codice lingua ISO 639-1 (es. "it", "en", "de"). Obbligatorio.
        /// DS07: codice_lingua (obbligatorio).
        /// </summary>
        [Required]
        [JsonProperty("codice_lingua")]
        public string CodiceLingua { get; set; }
    }

    /// <summary>
    /// Chiave identificativa dell'impianto/appezzamento.
    /// DS: impianto - parametri obbligatori per identificare univocamente la registrazione.
    /// </summary>
    public sealed class ImpiantoInput
    {

        [Required]
        [MaxLength(25)]
        [JsonProperty("piva")]
        public string Piva { get; set; } = string.Empty;

        [Required]
        [JsonProperty("sa_cod")]
        public int SaCod { get; set; }

        [Required]
        [JsonProperty("appezza")]
        public int Appezza { get; set; }

        [Required]
        [JsonProperty("id_reg")]
        public int IdReg { get; set; }

        [Required]
        [JsonProperty("progetto_cod")]
        public int ProgettoCod { get; set; }

        [Required]
        [JsonProperty("superficie_impianto")]
        public decimal SuperficieImpianto { get; set; }
    }
}
