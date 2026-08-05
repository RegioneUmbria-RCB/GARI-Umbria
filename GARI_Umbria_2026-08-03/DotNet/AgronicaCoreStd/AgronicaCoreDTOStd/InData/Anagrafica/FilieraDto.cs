using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    /// <summary>
    /// Rappresenta una Filiera con l'elenco delle Aziende visibili all'Utente.
    /// </summary>
    /// <remarks>
    /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Output.
    /// Corrisponde al nodo <c>filiere[i]</c> nella struttura JSON di risposta dell'API
    /// GET /api/v1/anagrafica/utenti/cono-visibilita.
    /// </remarks>
    public class FilieraDto
    {
        [JsonProperty("id_filiera")]
        public string IdFiliera { get; set; } = string.Empty;

        [JsonProperty("id_aziende")]
        public List<string> IdAziende { get; set; } = new List<string>();
    }
}
