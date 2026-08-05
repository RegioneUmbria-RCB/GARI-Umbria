using AgronicaNetCore.SmartTractor.DAL.DataLayer;
using System.Text;
using System.Data;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Data Access Layer for provider machine mappings.
/// Manages queries for checking machine to provider mappings.
/// Implements IProviderMappingMachine interface for dependency injection and testing.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// 
/// Method Conventions:
/// - SELECT methods use ExecuteReadAsync() with Dictionary&lt;string, object&gt; parameters
/// - INSERT/UPDATE/DELETE methods use Execute_WriteAsync() with ExpandoObject parameters
/// </summary>
public class ProviderMappingMachine : BaseDALSmartTractor, IProviderMappingMachine
{
    public ProviderMappingMachine(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    /// <summary>
    /// Gets the machine mapping for a specific machine and provider.
    /// </summary>
    /// <param name="mac_cod">The machine ID (UUID).</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>A DataTable with the mapping if it exists, empty otherwise.</returns>
    public async Task<DataTable> GetMachineMappingAsync(
        int mac_cod,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    mac_cod, provider_id, provider_code, onboarding_completed, onboarding_data");
        stbQuery.AppendLine("FROM smart_tractor_machine");
        stbQuery.AppendLine("WHERE mac_cod = @mac_cod");

        parSql.Add("@mac_cod", mac_cod);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading machine mapping for machine {mac_cod}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Checks if a machine is mapped with a provider.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if mapping exists, false otherwise.</returns>
    public async Task<bool> IsMachineMappedAsync(
        int macCod,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var dt = await GetMachineMappingAsync(macCod, serverParams);
        return dt != null && dt.Rows.Count > 0;
    }

    /// <summary>
    /// Inserts a new machine to provider mapping.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The provider-specific code for this machine.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if insertion was successful, false otherwise.</returns>
    public async Task<bool> InsertMachineMappingAsync(
        int macCod,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("INSERT INTO smart_tractor_machine");
        stbQuery.AppendLine("    (mac_cod, provider_id, provider_code, Data_Creazione, Data_Modifica)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@mac_cod, @provider_id, @provider_code, @created_date, @modified_date)");

        parSql.mac_cod = macCod;
        parSql.provider_id = providerId;
        parSql.provider_code = providerCode;
        parSql.created_date = DateTime.UtcNow;
        parSql.modified_date = DateTime.UtcNow;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error inserting machine mapping for machine {macCod} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing machine to provider mapping.
    /// </summary>
    /// <param name="mac_cod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The updated provider-specific code.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if update was successful, false otherwise.</returns>
    public async Task<bool> UpdateMachineMappingAsync(
        int mac_cod,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("UPDATE smart_tractor_machine");
        stbQuery.AppendLine("SET");
        stbQuery.AppendLine("    provider_code = @provider_code,");
        stbQuery.AppendLine("    modified_date = @modified_date");
        stbQuery.AppendLine("WHERE mac_cod = @mac_cod");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.mac_cod = mac_cod;
        parSql.provider_id = providerId;
        parSql.provider_code = providerCode;
        parSql.modified_date = DateTime.UtcNow;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error updating machine mapping for machine {mac_cod} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Deletes a machine to provider mapping.
    /// </summary>
    /// <param name="macCod">The machine ID (UUID).</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public async Task<bool> DeleteMachineMappingAsync(
        int macCod,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("DELETE FROM smart_tractor_machine");
        stbQuery.AppendLine("WHERE mac_cod = @mac_cod");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.mac_cod = macCod;
        parSql.provider_id = providerId;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting machine mapping for machine {macCod} and provider {providerId}", serverParams, ex);
            throw;
        }
    }
}