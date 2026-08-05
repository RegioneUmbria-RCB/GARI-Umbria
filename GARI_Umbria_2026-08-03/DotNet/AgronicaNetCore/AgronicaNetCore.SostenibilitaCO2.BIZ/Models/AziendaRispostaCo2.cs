using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Indicatori di sostenibilità CO2 a livello di azienda restituiti dal motore M4.
    /// Riferimento spec: DS07-API — Risposta 200, array <c>aziende[]</c>.
    /// </summary>
    public class AziendaRispostaCo2
    {
        /// <summary>Partita IVA dell'azienda.</summary>
        [JsonPropertyName("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        /// <summary>Anno campagna.</summary>
        [JsonPropertyName("campagna")]
        public int Campagna { get; set; }

        /// <summary>Centroide geografico dell'azienda in formato WKT.</summary>
        [JsonPropertyName("centroide")]
        public string? Centroide { get; set; }

        /// <summary>Nazione dell'azienda.</summary>
        [JsonPropertyName("nazione")]
        public string? Nazione { get; set; }

        /// <summary>Emissioni GHG totali aziendali (Kg CO2 equiv).</summary>
        [JsonPropertyName("ghg")]
        public string Ghg { get; set; }

        /// <summary>Rimozioni totali aziendali (Kg CO2 equiv).</summary>
        [JsonPropertyName("rimozioni")]
        public string Rimozioni { get; set; }

        // Campi mantenuti per compatibilità con versioni precedenti del servizio M4.
        /// <summary>Emissioni Scope 1 in Kg CO2 equiv.</summary>
        [JsonPropertyName("scope_1")]
        public string Scope1 { get; set; }

        /// <summary>Emissioni Scope 2 (location-based) in Kg CO2 equiv.</summary>
        [JsonPropertyName("scope_2_location_based")]
        public string Scope2LocationBased { get; set; }

        /// <summary>Emissioni Scope 3 in Kg CO2 equiv.</summary>
        [JsonPropertyName("scope_3")]
        public string Scope3 { get; set; }

        /// <summary>Carbonio biogenico totale in Kg CO2 equiv.</summary>
        [JsonPropertyName("biogenic_carbon")]
        public string BiogenicCarbon { get; set; }

        /// <summary>
        /// Variazione di carbonio biogenico nel suolo per SOC (Kg CO2 equiv).
        /// Può essere negativo.
        /// </summary>
        [JsonPropertyName("var_soc_soil_biogenic_carbon")]
        public string VarSocSoilBiogenicCarbon { get; set; }

        /// <summary>
        /// Variazione di carbonio biogenico nella biomassa (Kg CO2 equiv).
        /// Può essere negativo.
        /// </summary>
        [JsonPropertyName("var_biomass_biogenic_carbon")]
        public string VarBiomassBiogenicCarbon { get; set; }

        /// <summary>Totali CO2eq per scope a livello aziendale.</summary>
        [JsonPropertyName("totali")]
        public List<TotaliRispostaCo2> Totali { get; set; } = new();

        /// <summary>Dettaglio indicatori per appezzamento.</summary>
        [JsonPropertyName("appezzamenti")]
        public List<AppezzamentoRispostaCo2> Appezzamenti { get; set; } = new();

        /// <summary>Dettaglio indicatori per allevamento.</summary>
        [JsonPropertyName("allevamenti")]
        public List<AllevamentoRispostaCo2> Allevamenti { get; set; } = new();

        /// <summary>Emissioni da fonti non agricole (energia, carburante aziendale, ecc.).</summary>
        [JsonPropertyName("other_parts")]
        public List<OtherPartsRispostaCo2> OtherParts { get; set; } = new();

        /// <summary>Warning non bloccanti a livello di azienda.</summary>
        [JsonPropertyName("warnings")]
        public List<string> Warnings { get; set; } = new();
    }
}
