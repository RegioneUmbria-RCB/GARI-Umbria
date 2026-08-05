namespace AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.ModelliDifesa.Models
{
    /// <summary>
    /// Represents a pest model.
    /// </summary>
    public class ModelloDifesa
    {
        //public class CropRef
        //{
        //    /// <summary>
        //    /// Code of the colture.
        //    /// </summary>
        //    public string CropCode { get; set; } = string.Empty;

        //    /// <summary>
        //    /// Variety code of the colture, can be null.
        //    /// </summary>
        //    public string? VarietyCode { get; set; }
        //}

        /// <summary>
        /// Unique identifier of the model.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Code of the model.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Type of the model (DEFENSE | NUTRITION).
        /// </summary>
        public string ModelType { get; set; } = string.Empty;

        /// <summary>
        /// Description of the model, can be null.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Coltures using this model.
        /// </summary>
        //public CropRef[]? ModelCrops { get; set; }
    }
}
