using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InData.AntaresTrace
{
    public class OperazioneColturaleDto : IOperazioneColturaleDto
    {
        [JsonPropertyName("idAttivita")]
        public string IdAttivita { get; set; }
        [JsonPropertyName("descrizioneAttivita")]
        public string DescrizioneAttivita { get; set; } = "";
        [JsonPropertyName("partitaIva")]
        public string PartitaIva { get; set; } = "";
        [JsonPropertyName("codiceSedeAziendale")]
        public int CodiceSedeAziendale { get; set; }
        [JsonPropertyName("saNome")]
        public string SaNome { get; set; } = "";
        [JsonPropertyName("dataAttivita")]
        public DateTime DataAttivita { get; set; }
        [JsonPropertyName("codiceAttivita")]
        public int CodiceAttivita { get; set; }
        [JsonPropertyName("specieVegetale")]
        public int SpecieVegetale { get; set; }
        [JsonPropertyName("impianti")]
        public List<ImpiantoDto> Impianti { get; set; }
        [JsonPropertyName("superficieTrattataTotale")]
        public float SuperficieTrattataTotale { get; set; } = 0;

        public OperazioneColturaleDto(
            string idAttivita, string partitaIva, int codiceSedeAziendale, string descrizioneAttivita, string saNome, int codiceAttivita, int specieVegetale,
            DateTime dataAttivita
        )
        {
            IdAttivita = idAttivita;
            PartitaIva = partitaIva;
            CodiceSedeAziendale = codiceSedeAziendale;
            DescrizioneAttivita = descrizioneAttivita;
            SaNome = saNome;
            DataAttivita = dataAttivita;
            CodiceAttivita = codiceAttivita;
            SpecieVegetale = specieVegetale;
        }
    }
}