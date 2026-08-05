
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace InData.AntaresTrace
{
    public class SeminaDto : OperazioneColturaleDto
    {
        /// <summary>
        /// Elenco dei prodotti fertilizzanti impiegati
        /// </summary>
        [JsonPropertyName("prodottiSementi")]
        public List<ProdottoSeminaDto> ProdottiSeminaUtilizzati { get; set; }


        public SeminaDto(
            string idAttivita, string partitaIva, int codiceSedeAziendale, string descrizioneAttivita, string saNome, int codiceAttivita, int specieVegetale,
            DateTime dataAttivita
        ) : base(idAttivita, partitaIva, codiceSedeAziendale, descrizioneAttivita, saNome, codiceAttivita, specieVegetale, dataAttivita)
        { }
    }
}