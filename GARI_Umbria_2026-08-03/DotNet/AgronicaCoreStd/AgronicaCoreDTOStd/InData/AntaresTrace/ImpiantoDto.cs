using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace InData.AntaresTrace
{

    /// <summary>
    /// DTO per la deserializzazione JSON di un impianto
    /// </summary>
    public class ImpiantoDto
    {
        /// <summary>
        /// Identificativo unico dell'impianto
        /// </summary>
        [Required(ErrorMessage = "La chiave impianto è obbligatoria")]
        [JsonPropertyName("chiaveImpianto")]
        public string ChiaveImpianto { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione dell'impianto.
        /// </summary>
        [JsonPropertyName("descrizioneImpianto")]
        public string DescrizioneImpianto { get; set; } = string.Empty;

        /// <summary>
        /// Superficie impianto
        /// </summary>
        [Range(0, float.MaxValue, ErrorMessage = "La superficie deve essere un valore positivo")]
        [JsonPropertyName("superficie")]
        public float Superficie { get; set; }

        /// <summary>
        /// Superficie trattata
        /// </summary>
        [Range(0, float.MaxValue, ErrorMessage = "La superficie trattata deve essere un valore positivo")]
        [JsonPropertyName("superficieTrattata")]
        public float SuperficieTrattata { get; set; }

        /// <summary>
        /// Specie vegetale dell'impianto
        /// </summary>
        [Required(ErrorMessage = "La specie vegetale è obbligatoria")]
        [JsonPropertyName("specieVegetale")]
        public int SpecieVegetale { get; set; }

        /// <summary>
        /// Finalità dell'impianto
        /// </summary>
        [Required(ErrorMessage = "La finalità è obbligatoria")]
        [JsonPropertyName("finalita")]
        public int Finalita { get; set; }

        /// <summary>
        /// Tipo di protezione dell'impianto (0 - non specificato, 1 - protetta, 2 - pieno campo)
        /// </summary>
        [JsonPropertyName("tipoProtezione")]
        public int TipoProtezione { get; set; } = 0;

        /// <summary>
        /// Fase Produttiva dell'impianto
        /// </summary>
        [Required(ErrorMessage = "La fase produttiva è obbligatoria")]
        [JsonPropertyName("faseProduttiva")]
        public int FaseProduttiva { get; set; }

        /// <summary>
        /// Copertura dell'impianto
        /// </summary>
        [JsonPropertyName("copertura")]
        public int Copertura { get; set; } = 0;

        /// <summary>
        /// Forma di allevamento dell'impianto
        /// </summary>
        [JsonPropertyName("formaAllevamento")]
        public int FormaAllevamento { get; set; } = 0;

        [JsonPropertyName("lunghezzaConfineBufferZone")]
        public float LunghezzaConfineBufferZone { get; set; } = 0;

        [JsonPropertyName("dataRaccoltaPrevista")]
        public DateTime? DataRaccoltaPrevista { get; set; }
    
        [JsonPropertyName("offsetUltimaPianta")]
        public float OffsetUltimaPianta { get; set; } = 0;

        [JsonPropertyName("percRiduzioneDeriva")]
        public float PercRiduzioneDeriva { get; set; } = 0;

        [JsonPropertyName("comuni")]
        public List<ComuneDto> Comuni { get; set; }
        
        [Required(ErrorMessage = "La validità dell'impianto è obbligatoria")]
        [JsonPropertyName("validita")]
        public IntervalloDto Validita { get; set; }

        [JsonPropertyName("massimaleN")]
        public decimal? MassimaleN { get; set; } = null;

        [JsonPropertyName("massimaleP")]
        public decimal? MassimaleP { get; set; } = null;

        [JsonPropertyName("massimaleK")]
        public decimal? MassimaleK { get; set; } = null;

        [JsonPropertyName("massimaleMg")]
        public decimal? MassimaleMg { get; set; } = null;
    }

    public class IntervalloDto
    {
        [Required(ErrorMessage = "La data di inizio dell'impianto è obbligatoria")]
        [JsonPropertyName("inizio")]
        public DateTime Inizio { get; set; } 

        [Required(ErrorMessage = "La data di fine dell'impianto è obbligatoria")]
        [JsonPropertyName("fine")]
        public DateTime Fine { get; set; } 
    }

    public class ComuneDto
    {
        [JsonPropertyName("provinciaISTAT")]
        public string ProvinciaISTAT { get; set; } = "000";

        [JsonPropertyName("comuneISTAT")]
        public string ComuneISTAT { get; set; } = "000";
    }

}