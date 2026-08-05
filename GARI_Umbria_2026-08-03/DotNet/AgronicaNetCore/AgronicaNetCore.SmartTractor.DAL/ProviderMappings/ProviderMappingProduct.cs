using AgronicaNetCore.SmartTractor.DAL.DataLayer;
using System.Text;
using System.Data;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Data Access Layer for provider product mappings.
/// Manages queries for checking product to provider mappings.
/// Implements IProviderMappingProduct interface for dependency injection and testing.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// 
/// Method Conventions:
/// - SELECT methods use ExecuteReadAsync() with Dictionary&lt;string, object&gt; parameters
/// - INSERT/UPDATE/DELETE methods use Execute_WriteAsync() with ExpandoObject parameters
/// </summary>
public class ProviderMappingProduct : BaseDALSmartTractor, IProviderMappingProduct
{
    private readonly IAgro_Sequence _sequenceDal;

    public ProviderMappingProduct(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
    }

    /// <summary>
    /// Gets the product mapping for a specific product and provider.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>A DataTable with the mapping if it exists, empty otherwise.</returns>
    public async Task<DataTable> GetProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    id, elem_cod, product_id, provider_id, provider_code");
        stbQuery.AppendLine("FROM smart_tractor_provider_mapping_product");
        stbQuery.AppendLine("WHERE elem_cod = @elem_cod");
        stbQuery.AppendLine("  AND product_id = @product_id");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.Add("@elem_cod", elemCod);
        parSql.Add("@product_id", productId);
        parSql.Add("@provider_id", providerId);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading product mapping for product {productId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Checks if a product is mapped with a provider.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if mapping exists, false otherwise.</returns>
    public async Task<bool> IsProductMappedAsync(
        int elemCod,
        int productId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var dt = await GetProductMappingAsync(elemCod, productId, providerId, serverParams);
        return dt != null && dt.Rows.Count > 0;
    }

    /// <summary>
    /// Inserts a new product to provider mapping.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The provider-specific code for this product.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if insertion was successful, false otherwise.</returns>
    public async Task<bool> InsertProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("INSERT INTO smart_tractor_provider_mapping_product");
        stbQuery.AppendLine("    (id, elem_cod, product_id, provider_id, provider_code, created_date, modified_date)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@id, @elem_cod, @product_id, @provider_id, @provider_code, @created_date, @modified_date)");

        var id = await _sequenceDal.NuovoId_TabellaAsync("smart_tractor_provider_mapping_product", 0, 2000000000, serverParams);
        parSql.id = id;
        parSql.elem_cod = elemCod;
        parSql.product_id = productId;
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
            LogError($"Error inserting product mapping for product {productId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing product to provider mapping.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The updated provider-specific code.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if update was successful, false otherwise.</returns>
    public async Task<bool> UpdateProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("UPDATE smart_tractor_provider_mapping_product");
        stbQuery.AppendLine("SET");
        stbQuery.AppendLine("    provider_code = @provider_code,");
        stbQuery.AppendLine("    modified_date = @modified_date");
        stbQuery.AppendLine("WHERE elem_cod = @elem_cod");
        stbQuery.AppendLine("  AND product_id = @product_id");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.elem_cod = elemCod;
        parSql.product_id = productId;
        parSql.provider_id = providerId;
        parSql.provider_code = providerCode;
        parSql.modified_date = DateTime.UtcNow;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error updating product mapping for product {productId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <summary>
    /// Deletes a product to provider mapping.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public async Task<bool> DeleteProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    )
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("DELETE FROM smart_tractor_provider_mapping_product");
        stbQuery.AppendLine("WHERE elem_cod = @elem_cod");
        stbQuery.AppendLine("  AND product_id = @product_id");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.elem_cod = elemCod;
        parSql.product_id = productId;
        parSql.provider_id = providerId;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting product mapping for product {productId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }
}