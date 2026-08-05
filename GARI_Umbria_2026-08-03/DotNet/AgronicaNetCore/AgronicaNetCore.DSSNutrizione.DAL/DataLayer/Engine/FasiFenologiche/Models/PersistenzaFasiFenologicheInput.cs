using System.Collections.Generic;
using AgronicaCoreModelsSTD.Engine;
using InData.Engine.FasiFenologiche;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models
{
    /// <summary>
    /// Input per il servizio di persistenza delle fasi fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: PersistenzaFasiFenologiche - Input.
    /// </remarks>
    public sealed class PersistenzaFasiFenologicheInput
    {
        public ImpiantoInput Impianto { get; init; } = new();
        public IReadOnlyList<FaseFenologicaEngine> FasiFenologiche { get; init; } = new List<FaseFenologicaEngine>();
        public MetadataAcquisizione MetadataAcquisizione { get; init; } = new();
    }
}
