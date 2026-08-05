using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Payload JSON costruito da GIAS e inviato in POST al servizio Blockchain FMP (M5)
    /// per la creazione del Token CO2.
    /// Struttura gerarchica <c>farm → fields[]</c> conforme alla specifica M5 Input Schema.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Output, Regole 1-8.
    /// </summary>
    public class SostenibilitaCO2CarbonDataRequest
    {
        /// <summary>
        /// Codice univoco del token: <c>join('-', [filiera, azienda, anno, data_invocazione, id_invocazione])</c>.
        /// Riferimento spec: DS10-BL Regola 1.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Codice univoco del token: <c>join('-', [filiera, azienda, anno, data_invocazione, id_invocazione])</c>.
        /// Riferimento spec: DS10-BL Regola 1.
        /// </summary>
        [JsonPropertyName("farmAdminId")]
        public string FarmAdminId { get; set; } = string.Empty;

        /// <summary>Data inizio periodo (ISO 8601). Riferimento spec: DS10-BL Regola 2.</summary>
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = string.Empty;

        /// <summary>Data fine periodo (ISO 8601). Riferimento spec: DS10-BL Regola 2.</summary>
        [JsonPropertyName("end_date")]
        public string EndDate { get; set; } = string.Empty;

        /// <summary>
        /// Variazione SOC soil biogenic carbon a livello aziendale (Kg CO2 equiv).
        /// Riferimento spec: DS10-BL Regola 3.
        /// </summary>
        [JsonPropertyName("var_soc_soil_biogenic_carbon")]
        public decimal VarSocSoilBiogenicCarbon { get; set; }

        /// <summary>Dati dell'azienda agricola. Riferimento spec: DS10-BL Regole 4-8.</summary>
        [JsonPropertyName("farm")]
        public SostenibilitaCO2CarbonDataFarm Farm { get; set; } = new SostenibilitaCO2CarbonDataFarm();
    }

    /// <summary>
    /// Sezione <c>farm</c> del payload M5 inviato al servizio Blockchain.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Regole 4-7.
    /// </summary>
    public class SostenibilitaCO2CarbonDataFarm
    {
        /// <summary>Partita IVA (codice univoco azienda). Riferimento spec: DS10-BL Regola 4.</summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>Ragione Sociale dell'azienda. Riferimento spec: DS10-BL Regola 5.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Città sede dell'azienda. Riferimento spec: DS10-BL Regola 6.</summary>
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        /// <summary>Regione sede dell'azienda. Riferimento spec: DS10-BL Regola 6.</summary>
        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        /// <summary>Stato sede (ISO 3166-1 alpha-3, es. <c>ITA</c>). Riferimento spec: DS10-BL Regola 6.</summary>
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Superficie totale in ettari (somma <c>area_ha</c> degli appezzamenti).
        /// Riferimento spec: DS10-BL Regola 7.
        /// </summary>
        [JsonPropertyName("area_ha")]
        public decimal AreaHa { get; set; }

        /// <summary>Elenco appezzamenti. Riferimento spec: DS10-BL Regola 8.</summary>
        [JsonPropertyName("fields")]
        public List<SostenibilitaCO2CarbonDataField> Fields { get; set; } = new List<SostenibilitaCO2CarbonDataField>();
    }

    /// <summary>
    /// Sezione <c>fields[]</c> del payload M5: un singolo appezzamento con geometria e indicatori.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Regola 8.
    /// </summary>
    public class SostenibilitaCO2CarbonDataField
    {
        /// <summary>Identificativo appezzamento (<c>id_appezzamento</c> da risposta M4). Riferimento spec: DS10-BL Regola 8.code.</summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>Geometria poligono in formato WKT. Riferimento spec: DS10-BL Regola 8.WKT.</summary>
        [JsonPropertyName("WKT")]
        public string Wkt { get; set; } = string.Empty;

        /// <summary>
        /// Proiezione geografica (es. <c>EPSG:4326</c>). Default <c>EPSG:4326</c> se assente.
        /// Riferimento spec: DS10-BL Regola 8.EPSG.
        /// </summary>
        [JsonPropertyName("EPSG")]
        public string Epsg { get; set; } = string.Empty;

        /// <summary>Superficie in ettari. Riferimento spec: DS10-BL Regola 8.area_ha.</summary>
        [JsonPropertyName("area_ha")]
        public decimal AreaHa { get; set; }

        /// <summary>Variazione SOC soil biogenic carbon (Kg CO2 equiv). Riferimento spec: DS10-BL Regola 8.var_soc.</summary>
        [JsonPropertyName("var_soc_soil_biogenic_carbon")]
        public decimal VarSocSoilBiogenicCarbon { get; set; }
    }
}
