using System.Text;
using System.Data;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.SmartTractor.DAL.DataLayer;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Data Access Layer for provider plant mappings.
/// Manages queries for checking plant to provider mappings.
/// Implements IProviderMappingPlant interface for dependency injection and testing.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// 
/// Method Conventions:
/// - SELECT methods use ExecuteReadAsync() with Dictionary&lt;string, object&gt; parameters
/// - INSERT/UPDATE/DELETE methods use Execute_WriteAsync() with ExpandoObject parameters
/// </summary>
public class ProviderMappingPlant : BaseDALSmartTractor, IProviderMappingPlant
{
    private readonly IAgro_Sequence _sequenceDal;

    public ProviderMappingPlant(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();

    }

    /// <summary>
    /// Gets the plant mapping for a specific plant and provider.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>A DataTable with the mapping if it exists, empty otherwise.</returns>
    public async Task<DataTable> GetPlantMappingAsync(
        int plantId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    id, plant_id, provider_id, provider_code, created_date, modified_date");
        stbQuery.AppendLine("FROM smart_tractor_provider_mapping_plant");
        stbQuery.AppendLine("WHERE plant_id = @plant_id");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.Add("@plant_id", plantId);
        parSql.Add("@provider_id", providerId);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading plant mapping for plant {plantId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Checks if a plant is mapped with a provider.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if mapping exists, false otherwise.</returns>
    public async Task<bool> IsPlantMappedAsync(
        int plantId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var dt = await GetPlantMappingAsync(plantId, providerId, serverParams);
        return dt != null && dt.Rows.Count > 0;
    }

    /// <summary>
    /// Inserts a new plant to provider mapping.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The provider-specific code for this plant.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if insertion was successful, false otherwise.</returns>
    public async Task<bool> InsertPlantMappingAsync(
        int plantId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("INSERT INTO smart_tractor_provider_mapping_plant");
        stbQuery.AppendLine("    (id, plant_id, provider_id, provider_code, created_date, modified_date)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@plant_id, @provider_id, @provider_code, @created_date, @modified_date)");

        var id = await _sequenceDal.NuovoId_TabellaAsync("smart_tractor_provider_mapping_plant", 0, 2000000000, serverParams);
        parSql.id = id;
        parSql.plant_id = plantId;
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
            LogError($"Error inserting plant mapping for plant {plantId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing plant to provider mapping.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The updated provider-specific code.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if update was successful, false otherwise.</returns>
    public async Task<bool> UpdatePlantMappingAsync(
        int plantId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("UPDATE smart_tractor_provider_mapping_plant");
        stbQuery.AppendLine("SET");
        stbQuery.AppendLine("    provider_code = @provider_code,");
        stbQuery.AppendLine("    modified_date = @modified_date");
        stbQuery.AppendLine("WHERE plant_id = @plant_id");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.plant_id = plantId;
        parSql.provider_id = providerId;
        parSql.provider_code = providerCode;
        parSql.modified_date = DateTime.UtcNow;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error updating plant mapping for plant {plantId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Deletes a plant to provider mapping.
    /// </summary>
    /// <param name="plantId">The plant ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public async Task<bool> DeletePlantMappingAsync(
        int plantId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("DELETE FROM smart_tractor_provider_mapping_plant");
        stbQuery.AppendLine("WHERE plant_id = @plant_id");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.plant_id = plantId;
        parSql.provider_id = providerId;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting plant mapping for plant {plantId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the plant mapping using the composite key (piva, sa_cod, appezza, id_reg).
    /// Columns returned: id, piva, sa_cod, appezza, id_reg, provider_code.
    /// </summary>
    public async Task<DataTable> GetPlantMappingByCompositeKeyAsync(
        string providerId,
        string piva,
        int saCod,
        int appezza,
        int idReg,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    id, piva, sa_cod, appezza, id_reg, provider_code");
        stbQuery.AppendLine("FROM smart_tractor_provider_mapping_plant");
        stbQuery.AppendLine("WHERE provider_id = @provider_id");
        stbQuery.AppendLine("  AND piva        = @piva");
        stbQuery.AppendLine("  AND sa_cod      = @sa_cod");
        stbQuery.AppendLine("  AND appezza     = @appezza");
        stbQuery.AppendLine("  AND id_reg      = @id_reg");

        parSql.Add("@provider_id", providerId);
        parSql.Add("@piva",        piva);
        parSql.Add("@sa_cod",      saCod);
        parSql.Add("@appezza",     appezza);
        parSql.Add("@id_reg",      idReg);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading plant mapping by composite key (piva={piva}, sa_cod={saCod}, appezza={appezza}, id_reg={idReg}) for provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Checks whether a plant identified by its composite key is mapped to the given provider.
    /// </summary>
    public async Task<bool> IsPlantMappedByCompositeKeyAsync(
        string providerId,
        string piva,
        int saCod,
        int appezza,
        int idReg,
        AgronicaCoreParametriServer serverParams
    )
    {
        var dt = await GetPlantMappingByCompositeKeyAsync(providerId, piva, saCod, appezza, idReg, serverParams);
        return dt != null && dt.Rows.Count > 0;
    }
}