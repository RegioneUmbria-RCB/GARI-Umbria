using AgronicaNetCore.Base.Models;
using InData.Engine.DSSDifesa;
using OutData.Engine.DSSDifesa;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Acquisizione
{
    /// <summary>
    /// Interface for the service that retrieves the dynamic list of pest models for a crop from DSS Engine.
    /// Referenced in DS03-BL_ Recupero Lista Infestanti Dinamica.
    /// </summary>
    public interface IAcquisizioneModelliDifesaService
    {
        /// <summary>
        /// Retrieves the dynamic list of pest models for the specified crop.
        /// </summary>
        /// <param name="request">The request containing crop code, variety code and model codes.</param>
        /// <param name="objParametriServer">Server parameters.</param>
        /// <param name="objParametriSuperServer">Super server parameters.</param>
        /// <returns>The response containing the list of pest models, status, and fetch timestamp.</returns>
        Task<AcquisizioneModelliDSSDifesaResponse> RecuperaListaModelliDifesaDinamicaAsync(
            AcquisizioneModelliDSSDifesaRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}