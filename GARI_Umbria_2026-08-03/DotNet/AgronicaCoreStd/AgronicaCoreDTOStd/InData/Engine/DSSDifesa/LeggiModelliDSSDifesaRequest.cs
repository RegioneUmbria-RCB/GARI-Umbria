using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace InData.Engine.DSSDifesa
{
    public class LeggiModelliDSSDifesaRequest
    {
        /// <summary>
        /// Crop code (e.g. "12", "20", "420").
        /// </summary>
        //[Required]
        //[JsonProperty("crop_code")]
        public string CropCode { get; set; } = string.Empty;

        /// <summary>
        /// Variety code, can be null.
        /// </summary>
        //[JsonProperty("var_code")]
        public string VarCode { get; set; } = string.Empty;

        /// <summary>
        /// List of model codes to retrieve. If null or empty, retrieve all models for the crop.
        /// </summary>
        //[JsonProperty("modelli_codici")]
        public List<string> ModelliCodici { get; set; } = new List<string>();
    }
}
