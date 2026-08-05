using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Interface for Data Access Layer operations on product to provider mappings.
/// Referenced in Design Specification: DS02-BL - Validazione Dati Attività e Regole di Composizione
/// </summary>
public interface IProviderMappingProduct
{
    /// <summary>
    /// Gets the product mapping for a specific product and provider.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>A DataTable with the mapping if it exists, empty otherwise.</returns>
    Task<DataTable> GetProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Checks if a product is mapped with a provider.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if mapping exists, false otherwise.</returns>
    Task<bool> IsProductMappedAsync(
        int elemCod,
        int productId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Inserts a new product to provider mapping.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The provider-specific code for this product.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if insertion was successful, false otherwise.</returns>
    Task<bool> InsertProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Updates an existing product to provider mapping.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="providerCode">The updated provider-specific code.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if update was successful, false otherwise.</returns>
    Task<bool> UpdateProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams
    );

    /// <summary>
    /// Deletes a product to provider mapping.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="providerId">The provider ID.</param>
    /// <param name="serverParams">The server parameters.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeleteProductMappingAsync(
        int elemCod,
        int productId,
        string providerId,
        AgronicaCoreParametriServer serverParams
    );
}
