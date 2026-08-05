using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InData.AntaresTrace
{
    public class RaccoltaDto : OperazioneColturaleDto
    {
        /// <summary>
        /// Elenco dei prodotti fertilizzanti impiegati
        /// </summary>
        [JsonPropertyName("prodottiRaccolta")]
        public List<ProdottoSeminaDto> ProdottiRaccolti { get; set; }

        public RaccoltaDto(
            string idAttivita, string partitaIva, int codiceSedeAziendale, string descrizioneAttivita, string saNome, int codiceAttivita, int specieVegetale,
            DateTime dataAttivita
        ) : base(idAttivita, partitaIva, codiceSedeAziendale, descrizioneAttivita, saNome, codiceAttivita, specieVegetale, dataAttivita)
        { }
    }
};

