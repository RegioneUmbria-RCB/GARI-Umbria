using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OutData.FoodMetaverse;

namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Request DTO per l'endpoint POST <c>/v1/sostenibilita-co2/crea-token-blockchain</c>.
    /// Contiene i dati di selezione utente (azienda, id_invocazione) e il payload
    /// M4 già deserializzato che verrà usato per costruire il payload M5 Blockchain.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Input.
    /// </summary>
    public class SostenibilitaCO2CreaTokenBlockchainRequest
    {
        /// <summary>Partita IVA dell'azienda selezionata dall'utente (output DS09-BL).</summary>
        [Required]
        public string Azienda { get; set; } = string.Empty;

        [Required]
        public string Filiera { get; set; } = string.Empty;

        /// <summary>
        /// Identificativo univoco (UUID) dell'invocazione M4 selezionata dall'utente (output DS09-BL).
        /// </summary>
        [Required]
        public string IdInvocazione { get; set; } = string.Empty;

        /// <summary>
        /// Risposta M4 deserializzata da <c>Lookup_Sost_CO2_Aziendale_Payload.json_risposta</c>.
        /// Fornisce <c>start_date</c>, <c>end_date</c>, variazioni SOC e dati appezzamenti.
        /// </summary>
        [Required]
        public string PayloadLookupSostenibilitaCO2{ get; set; } = string.Empty;
    }
}
