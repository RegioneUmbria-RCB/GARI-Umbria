using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    /// <summary>
    /// DTO di risposta per il cono di visibilità organizzativo di un Utente.
    /// Contiene l'elenco delle Filiere accesibili e, per ciascuna, le Aziende visibili.
    /// </summary>
    /// <remarks>
    /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Output.
    /// Se l'Utente esiste ma non ha alcuna visibilità assegnata, <c>Filiere</c> è una lista vuota
    /// (non null) e la risposta HTTP è 200 OK — non 404.
    /// </remarks>
    public class ConoVisibilitaResponseDto
    {
        [JsonProperty("filiere")]
        public List<FilieraDto> Filiere { get; set; } = new List<FilieraDto>();
    }
}
