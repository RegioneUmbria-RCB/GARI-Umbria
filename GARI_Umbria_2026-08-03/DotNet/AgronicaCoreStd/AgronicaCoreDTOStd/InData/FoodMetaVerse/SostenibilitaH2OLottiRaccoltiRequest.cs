using Newtonsoft.Json;
using System.Collections.Generic;

namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Request body for POST /sostenibilita_h2o/sintesi_per_lotti_raccolti.
    /// See DS11-API Endpoint POST Sintesi Lotti Raccolti Sostenibilita H2O, section "Formato Richiesta".
    /// </summary>
    public class SostenibilitaH2OLottiRaccoltiRequest
    {
        [JsonProperty("id_filiera")]
        public string IdFiliera { get; set; } = string.Empty;

        [JsonProperty("cod_prodotto")]
        public string CodProdottoFmp { get; set; } = string.Empty;

        [JsonProperty("lotti")]
        public List<SostenibilitaH2OLottoRichiestoDto> Lotti { get; set; } = new List<SostenibilitaH2OLottoRichiestoDto>();
    }

    /// <summary>
    /// Requested lot identity for the batch input.
    /// See DS11-API Endpoint POST Sintesi Lotti Raccolti Sostenibilita H2O, section "Formato Richiesta".
    /// </summary>
    public class SostenibilitaH2OLottoRichiestoDto
    {
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        [JsonProperty("cod_lotto")]
        public string CodLottoFmp { get; set; } = string.Empty;
    }
}
