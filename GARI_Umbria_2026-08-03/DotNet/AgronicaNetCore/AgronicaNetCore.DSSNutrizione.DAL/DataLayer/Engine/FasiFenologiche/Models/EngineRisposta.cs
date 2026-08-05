using AgronicaCoreModelsSTD.Engine;
using System.Collections.Generic;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models
{
    /// <summary>
    /// Risposta complessiva della chiamata all'engine Fasi Fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: ChiamataEngineFasiFenologiche - Output.
    /// </remarks>
    public sealed class EngineRisposta
    {
        public bool EsitoChiamata { get; init; }
        public IReadOnlyList<FaseFenologicaEngine> FasiFenologiche { get; init; } = new List<FaseFenologicaEngine>();
        public EngineRispostaMetadata MetadataRisposta { get; init; } = new();
    }
}
