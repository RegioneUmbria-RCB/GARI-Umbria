using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Interface for Data Access Layer operations on plant to provider mappings.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public interface IProviderMappingPlant
{
    /// <summary>
    /// Gets the plant mapping for a specific plant and provider.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>A DataTable with the mapping if it exists, empty otherwise.</returns>
    Task<DataTable> GetPlantMappingAsync(
        int plantId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Checks if a plant is mapped with a provider.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if mapping exists, false otherwise.</returns>
    Task<bool> IsPlantMappedAsync(
        int plantId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Inserts a new plant to provider mapping.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The provider-specific code for this plant.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if insertion was successful, false otherwise.</returns>
    Task<bool> InsertPlantMappingAsync(
        int plantId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Updates an existing plant to provider mapping.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The updated provider-specific code.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if update was successful, false otherwise.</returns>
    Task<bool> UpdatePlantMappingAsync(
        int plantId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Deletes a plant to provider mapping.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeletePlantMappingAsync(
        int plantId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Gets the plant mapping using the composite key (piva, sa_cod, appezza, id_reg) from RICETTE_DESTINAZIONI.
    /// Used in DS03-BLb FASE 6 to resolve provider_code for each destination plant.
    /// </summary>
    /// <param name="providerId">The integer provider ID.</param>
    /// <param name="piva">Partita IVA of the farm.</param>
    /// <param name="saCod">Sa_Cod value from RICETTE_DESTINAZIONI.</param>
    /// <param name="appezza">Appezza value from RICETTE_DESTINAZIONI.</param>
    /// <param name="idReg">Id_reg value from RICETTE_DESTINAZIONI.</param>
    /// <param name="serverParams">Database connection parameters.</param>
    /// <returns>A DataTable with columns id, provider_code when the mapping exists; empty otherwise.</returns>
    Task<DataTable> GetPlantMappingByCompositeKeyAsync(
        string providerId,
        string piva,
        int saCod,
        int appezza,
        int idReg,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Checks whether a plant identified by its composite key is mapped to the given provider.
    /// </summary>
    /// <param name="providerId">The integer provider ID.</param>
    /// <param name="piva">Partita IVA of the farm.</param>
    /// <param name="saCod">Sa_Cod value from RICETTE_DESTINAZIONI.</param>
    /// <param name="appezza">Appezza value from RICETTE_DESTINAZIONI.</param>
    /// <param name="idReg">Id_reg value from RICETTE_DESTINAZIONI.</param>
    /// <param name="serverParams">Database connection parameters.</param>
    /// <returns>True if the mapping exists, false otherwise.</returns>
    Task<bool> IsPlantMappedByCompositeKeyAsync(
        string providerId,
        string piva,
        int saCod,
        int appezza,
        int idReg,
        AgronicaCoreParametriServer serverParams
    );
}
