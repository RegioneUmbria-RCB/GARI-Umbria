using System;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni
{
    public class DatiComuniFromFlutterRequest
    {
        // anagrafiche metaschema
        [JsonProperty("anagMetaschema")]
        public bool AnagMetaschema { get; set; } = true;

        // impostazioni rilievi
        [JsonProperty("anagPersonalizzataRilievoFasiFenologiche")]
        public bool AnagPersonalizzataRilievoFasiFenologiche { get; set; }

        [JsonProperty("anagPersonalizzataRilievoIndiciMaturita")]
        public bool AnagPersonalizzataRilievoIndiciMaturita { get; set; }

        [JsonProperty("anagPersonalizzataRilievoAvversita")]
        public bool AnagPersonalizzataRilievoAvversita { get; set; }

        [JsonProperty("anagPersonalizzataRilievoDanniRaccolta")]
        public bool AnagPersonalizzataRilievoDanniRaccolta { get; set; }

        [JsonProperty("anagPersonalizzataRilievoErbeInfestanti")]
        public bool AnagPersonalizzataRilievoErbeInfestanti { get; set; }

        [JsonProperty("anagUtilizziTerreno")]
        public string AnagUtilizziTerreno { get; set; }

        // impostazioni nazioni estero
        [JsonProperty("anagNazioni")]
        public string AnagNazioni { get; set; }

        // impostazioni disciplinari
        [JsonProperty("anagDisciplinari")]
        public string AnagDisciplinari { get; set; }

        [JsonProperty("timestamp_ultima_sincro")]
        public string TimestampUltimaSincronizzazione { get; set; }
    }
}
