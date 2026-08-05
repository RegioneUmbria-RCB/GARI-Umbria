namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Acquisizione
{
    /// <summary>
    /// Input model for retrieving dynamic list of pest models for a crop.
    /// Referenced in DS03-BL_ Recupero Lista Infestanti Dinamica - Input section.
    /// </summary>
    public class AcquisizioneModelliDSSDifesaRequest
    {
        /// <summary>
        /// Crop code (e.g. "12", "20", "420").
        /// </summary>
        public string CropCode { get; set; } = string.Empty;

        /// <summary>
        /// Variety code, can be null.
        /// </summary>
        public string? VarCode { get; set; }

        /// <summary>
        /// List of model codes to retrieve. If null or empty, retrieve all models for the crop.
        /// </summary>
        public List<string>? ModelliCodici { get; set; }
    }
}