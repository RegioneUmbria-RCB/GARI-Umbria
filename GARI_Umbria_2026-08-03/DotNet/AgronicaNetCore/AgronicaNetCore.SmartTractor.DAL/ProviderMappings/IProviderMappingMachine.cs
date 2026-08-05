using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Interface for Data Access Layer operations on machine to provider mappings.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public interface IProviderMappingMachine
{
    /// <summary>
    /// Gets the machine mapping for a specific machine and provider.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>A DataTable with the mapping if it exists, empty otherwise.</returns>
    Task<DataTable> GetMachineMappingAsync(
        int macCod,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Checks if a machine is mapped with a provider.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if mapping exists, false otherwise.</returns>
    Task<bool> IsMachineMappedAsync(
        int macCod,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Inserts a new machine to provider mapping.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The provider-specific code for this machine.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if insertion was successful, false otherwise.</returns>
    Task<bool> InsertMachineMappingAsync(
        int macCod,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Updates an existing machine to provider mapping.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The updated provider-specific code.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if update was successful, false otherwise.</returns>
    Task<bool> UpdateMachineMappingAsync(
        int macCod,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Deletes a machine to provider mapping.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeleteMachineMappingAsync(
        int macCod,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );
}
