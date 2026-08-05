using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.ModelliDifesa.Models;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Acquisizione
{
    /// <summary>
    /// Output model for retrieving dynamic list of pest models for a crop.
    /// Referenced in DS03-BL_ Recupero Lista Infestanti Dinamica - Output section.
    /// </summary>
    public class AcquisizioneModelliDSSDifesaResponse
    {
        /// <summary>
        /// List of pest models.
        /// </summary>
        public List<ModelloDifesa> Modelli { get; set; } = new();

        /// <summary>
        /// Status of the operation ("OK" or "ERROR").
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of the fetch in ISO 8601 format.
        /// </summary>
        public string FetchTimestamp { get; set; } = string.Empty;
    }
}