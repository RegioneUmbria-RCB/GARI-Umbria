using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Data Access Layer interface for raster map provider mappings.
/// Manages CRUD operations on <c>smart_tractor_provider_mapping_raster_map</c>.
///
/// Reads raster map associations from the Agronica GIAS tables:
/// <c>Alert_Entita</c> (TipoEntita_Cod = 16) → <c>Allegati_Documenti</c>.
///
/// Referenced in Design Specification:
/// DS03-BLc - Lettura e invio mappa raster associata
/// </summary>
public interface IProviderMappingRasterMap
{
    /// <summary>
    /// Reads all raster maps associated with a prescription by joining
    /// <c>Alert_Entita</c> (TipoEntita_Cod = 16, Ricetta_Operazione_Cod)
    /// with <c>Allegati_Documenti</c> to retrieve the physical file path.
    /// </summary>
    /// <param name="ricettaOperazioneCod">The prescription code (Ricetta_Operazione_Cod).</param>
    /// <param name="serverParams">Database connection context.</param>
    /// <returns>
    /// A <see cref="DataTable"/> with columns:
    /// <c>Allegati_Documenti_Cod</c>, <c>Allegati_Documenti_Des</c>,
    /// <c>Allegati_Documenti_NomeFile</c>.
    /// Empty when no raster maps are associated.
    /// </returns>
    Task<DataTable> GetRasterMapsByPrescriptionAsync(
        int ricettaOperazioneCod,
        AgronicaCoreParametriServer serverParams);

    /// <summary>
    /// Gets the provider mapping record for a specific raster map and provider.
    /// </summary>
    /// <param name="allegatiDocumentiCod">The raster document code.</param>
    /// <param name="providerId">The integer provider ID.</param>
    /// <param name="serverParams">Database connection context.</param>
    /// <returns>
    /// A <see cref="DataTable"/> with columns:
    /// <c>id</c>, <c>allegati_documenti_cod</c>, <c>provider_id</c>, <c>provider_code</c>.
    /// Empty when no mapping exists.
    /// </returns>
    Task<DataTable> GetRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        AgronicaCoreParametriServer serverParams);

    /// <summary>
    /// Inserts a new raster map provider mapping record.
    /// </summary>
    /// <param name="allegatiDocumentiCod">The raster document code (FK to Allegati_Documenti).</param>
    /// <param name="providerId">The integer provider ID.</param>
    /// <param name="providerCode">The provider-assigned code returned by the engine.</param>
    /// <param name="currentUserId">Username for audit columns.</param>
    /// <param name="serverParams">Database connection context.</param>
    /// <returns><c>true</c> if the insert succeeded, <c>false</c> otherwise.</returns>
    Task<bool> InsertRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams);

    /// <summary>
    /// Updates the provider code of an existing raster map mapping record.
    /// </summary>
    /// <param name="allegatiDocumentiCod">The raster document code.</param>
    /// <param name="providerId">The integer provider ID.</param>
    /// <param name="providerCode">The updated provider-assigned code.</param>
    /// <param name="currentUserId">Username for audit columns.</param>
    /// <param name="serverParams">Database connection context.</param>
    /// <returns><c>true</c> if the update succeeded, <c>false</c> otherwise.</returns>
    Task<bool> UpdateRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        string providerCode,
        string currentUserId,
        AgronicaCoreParametriServer serverParams);

    /// <summary>
    /// Deletes the provider mapping record for a specific raster map and provider.
    /// </summary>
    /// <param name="allegatiDocumentiCod">The raster document code.</param>
    /// <param name="providerId">The integer provider ID.</param>
    /// <param name="serverParams">Database connection context.</param>
    /// <returns><c>true</c> if the delete succeeded, <c>false</c> otherwise.</returns>
    Task<bool> DeleteRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        AgronicaCoreParametriServer serverParams);
}
