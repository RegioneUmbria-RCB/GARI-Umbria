using AgronicaNetCore.Base.Models;
using InData.Engine.FasiFenologiche;
using OutData.Engine.FasiFenologiche;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.FasiFenologiche.Orchestration
{
    /// <summary>
    /// Contratto BIZ dell'orchestratore per l'acquisizione fasi fenologiche.
    /// Estende <see cref="DAL.DataLayer.Engine.Orchestration.IOrchestrationAcquisizioneFasiFenologicheService"/>
    /// così che i consumer cross-BIZ possano risolvere l'implementazione tramite l'interfaccia DAL.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS01-BL Acquisizione Fasi Fenologiche da Engine Nutrizione — Scopo.
    /// </remarks>
    public interface IOrchestrationAcquisizioneFasiFenologicheService
    {

        Task<AcquisizioneFasiFenologicheResponse> EseguiAsync(
            AcquisizioneFasiFenologicheRequest request,
            string usernameRichiedente,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
