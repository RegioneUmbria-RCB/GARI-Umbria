using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Engine
{
    /// <summary>
    /// Service interface for outbound HTTP communication with the Smart Tractor engine API.
    /// </summary>
    public interface IEngineService
    {
        /// <summary>
        /// Sends the composed prescription payload to the Smart Tractor provider API.
        ///
        /// Reads the engine URL and API key from the DB via <c>SecurityLayerDAL</c>,
        /// then POSTs to:
        /// <c>{urlEngine}/{tenant}/public/v1/organizations/{fmisOrgId}/providers/{providerCode}/prescriptions</c>
        /// </summary>
        /// <param name="payload">The fully composed Smart Tractor payload.</param>
        /// <param name="serverParams">Database connection context.</param>
        /// <param name="superServerParams">Super-server context required for engine config lookup.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A <see cref="SendPrescriptionResponse"/> containing the provider-assigned activity ID.
        /// </returns>
        /// <exception cref="Exceptions.SmartTractorSendException">
        /// When the HTTP call fails (timeout, non-2xx response, or network error).
        /// </exception>
        Task<SendPrescriptionResponse> SendPrescriptionAsync(
            SmartTractorPayload payload,
            AgronicaCoreParametriServer serverParams,
            AgronicaCoreParametriSuperServer superServerParams,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads a raster prescription map (.tiff bytes) to the Smart Tractor provider API.
        ///
        /// Reads the engine URL and API key from the DB, then POSTs the binary content to:
        /// <c>{urlEngine}/{tenant}/public/v1/organizations/{fmisOrgId}/providers/{providerCode}/prescriptions/attachment</c>
        /// </summary>
        /// <param name="mapId">The Agronica prescription map int (used to construct the exception on failure).</param>
        /// <param name="tenant">Provider tenant identifier.</param>
        /// <param name="fmisOrgId">FMIS organisation identifier.</param>
        /// <param name="providerId">Provider code (e.g. CNH).</param>
        /// <param name="tiffBytes">Raw bytes of the .tiff file to upload.</param>
        /// <param name="serverParams">Database connection context.</param>
        /// <param name="superServerParams">Super-server context required for engine config lookup.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// An <see cref="UploadRasterMapEngineResponse"/> containing the provider-assigned attachment ID.
        /// </returns>
        /// <exception cref="Exceptions.RasterUploadException">
        /// When the HTTP call fails (timeout, non-2xx response, or network error).
        /// </exception>
        Task<UploadRasterMapEngineResponse> UploadRasterMapAsync(
            int mapId,
            string providerId,
            byte[] tiffBytes,
            AgronicaCoreParametriServer serverParams,
            AgronicaCoreParametriSuperServer superServerParams,
            CancellationToken cancellationToken = default);
    }
}
