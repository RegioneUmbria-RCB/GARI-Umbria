using System;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models
{
    /// <summary>
    /// Metadati di acquisizione da associare alla persistenza delle fasi fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: PersistenzaFasiFenologiche - Input.
    /// </remarks>
    public sealed class MetadataAcquisizione
    {
        public string UsernameCreazione { get; init; } = string.Empty;
        public DateTimeOffset TimestampAcquisizione { get; init; }
    }
}
