using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Calcolo
{
    /// <summary>
    /// Interface for calculating risk scores in parallel using DSS Difesa risk models.
    /// Referenced in DS05-BL_ Lancio Modelli Paralleli Calcolo Rischio.
    /// </summary>
    public interface ICalcoloRischioPerModelliDifesaService
    {
        /// <summary>
        /// Executes the risk model calls in parallel for each pest model, with polling, per-model timeout and global timeout.
        /// </summary>
        /// <param name="request">Input payload containing pest model list, meteorological data and detail level.</param>
        /// <param name="objParametriServer">Server parameters used for security config retrieval.</param>
        /// <param name="objParametriSuperServer">Super server parameters used for security config retrieval.</param>
        /// <param name="cancellationToken">Token to cancel the entire processing operation.</param>
        /// <returns>Aggregated execution results and global timeout flag.</returns>
        Task<CalcoloRischioPerModelliDifesaResponse> CalcolaRischioPerModelliDifesaAsync(
            CalcoloRischioPerModelliDifesaRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
