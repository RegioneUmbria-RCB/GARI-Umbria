using System;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models
{
    /// <summary>
    /// Metadati della risposta restituita dall'engine Fasi Fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: ChiamataEngineFasiFenologiche - Output.
    /// </remarks>
    public sealed class EngineRispostaMetadata
    {
        public DateTimeOffset TimestampChiamata { get; init; }
        public long DurataMs { get; init; }
        public int HttpStatusCode { get; init; }
    }
}
