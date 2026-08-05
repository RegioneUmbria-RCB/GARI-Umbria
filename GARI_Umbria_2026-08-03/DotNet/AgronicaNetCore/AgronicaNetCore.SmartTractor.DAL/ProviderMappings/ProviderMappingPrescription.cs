using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.DAL.DataLayer;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Data Access Layer for the Operazione entity.
/// Reads from the <c>smart_tractor_operazione</c> table.
///
/// Method Conventions:
/// - SELECT methods use ExecuteReadAsync() with Dictionary&lt;string, object&gt; parameters.
///
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 3)
/// </summary>
public class ProviderMappingPrescription : BaseDALSmartTractor, IProviderMappingPrescription
{
    public ProviderMappingPrescription(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    /// <summary>
    /// Gets the prescription mapping for a specific prescription and provider.
    /// Columns returned: lav_cod, provider_id, provider_code.
    /// </summary>
    public async Task<DataTable> GetPrescriptionMappingAsync(
        int prescriptionTypeId,
        int providerId,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT prescription_type_id, provider_id, provider_code");
        stbQuery.AppendLine("FROM smart_tractor_provider_mapping_prescription_type");
        stbQuery.AppendLine("WHERE prescription_type_id = @id");
        stbQuery.AppendLine("    AND provider_id = @providerId");

        parSql.Add("@id", prescriptionTypeId);
        parSql.Add("@providerId", providerId);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading prescription mapping for prescription {prescriptionTypeId} and provider {providerId}", serverParams, ex);
            throw;
        }
    }
}
