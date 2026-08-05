using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.DAL.DataLayer;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Data Access Layer for raster map provider mappings.
/// Manages CRUD operations on <c>smart_tractor_provider_mapping_raster_map</c>.
/// Reads raster map associations from <c>Alert_Entita</c> joined with <c>Allegati_Documenti</c>.
///
/// Method Conventions:
/// - SELECT methods use ExecuteReadAsync() with Dictionary&lt;string, object&gt; parameters
/// - INSERT/UPDATE/DELETE methods use Execute_WriteAsync() with ExpandoObject parameters
///
/// Referenced in Design Specification:
/// DS03-BLc - Lettura e invio mappa raster associata
/// </summary>
public class ProviderMappingRasterMap : BaseDALSmartTractor, IProviderMappingRasterMap
{
    private readonly IAgro_Sequence _sequenceDal;

    public ProviderMappingRasterMap(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
        _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
    }

    /// <inheritdoc/>
    public async Task<DataTable> GetRasterMapsByPrescriptionAsync(
        int ricettaOperazioneCod,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        // Join Alert_Entita (TipoEntita_Cod = 16 identifies recipes) with Allegati_Documenti
        // to retrieve the physical raster file path for the given prescription.
        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    ad.Allegati_Documenti_Cod,");
        stbQuery.AppendLine("    ad.Allegati_Documenti_Des,");
        stbQuery.AppendLine("    ad.Allegati_Documenti_NomeFile");
        stbQuery.AppendLine("FROM Alert_Entita ae");
        stbQuery.AppendLine("INNER JOIN Allegati_Documenti ad");
        stbQuery.AppendLine("    ON ad.Allegati_Documenti_Cod = ae.Allegati_Documenti_Cod");
        stbQuery.AppendLine("WHERE ae.TipoEntita_Cod = 16");
        stbQuery.AppendLine("  AND ae.Ricetta_Operazione_Cod = @ricetta_operazione_cod");
        stbQuery.AppendLine("  AND ad.Allegati_Documenti_NomeFile IS NOT NULL");

        parSql.Add("@ricetta_operazione_cod", ricettaOperazioneCod);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading raster maps for prescription {ricettaOperazioneCod}", serverParams, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DataTable> GetRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    id, allegati_documenti_cod, provider_id, provider_code,");
        stbQuery.AppendLine("    Data_Creazione, Data_Modifica");
        stbQuery.AppendLine("FROM smart_tractor_provider_mapping_raster_map");
        stbQuery.AppendLine("WHERE allegati_documenti_cod = @allegati_documenti_cod");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.Add("@allegati_documenti_cod", allegatiDocumentiCod);
        parSql.Add("@provider_id", providerId);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading raster map mapping for document {allegatiDocumentiCod} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> InsertRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        string providerCode,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("INSERT INTO smart_tractor_provider_mapping_raster_map");
        stbQuery.AppendLine("    (id, allegati_documenti_cod, provider_id, provider_code,");
        stbQuery.AppendLine("     Data_Creazione, Data_Modifica,");
        stbQuery.AppendLine("     Username_Creazione, Username_Modifica,");
        stbQuery.AppendLine("     Validita_Inizio)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@id, @allegati_documenti_cod, @provider_id, @provider_code,");
        stbQuery.AppendLine("     @data_creazione, @data_modifica,");
        stbQuery.AppendLine("     @username_creazione, @username_modifica,");
        stbQuery.AppendLine("     @validita_inizio)");

        var id = await _sequenceDal.NuovoId_TabellaAsync("smart_tractor_provider_mapping_raster_map", 0, 2000000000, serverParams);
        parSql.id = id;
        parSql.allegati_documenti_cod = allegatiDocumentiCod;
        parSql.provider_id = providerId;
        parSql.provider_code = providerCode;
        parSql.data_creazione = DateTime.UtcNow;
        parSql.data_modifica = DateTime.UtcNow;
        parSql.username_creazione = serverParams.UtenteUsername;
        parSql.username_modifica = serverParams.UtenteUsername;
        parSql.validita_inizio = DateTime.UtcNow;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error inserting raster map mapping for document {allegatiDocumentiCod} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        string providerCode,
        string currentUserId,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("UPDATE smart_tractor_provider_mapping_raster_map");
        stbQuery.AppendLine("SET");
        stbQuery.AppendLine("    provider_code      = @provider_code,");
        stbQuery.AppendLine("    Data_Modifica      = @data_modifica,");
        stbQuery.AppendLine("    Username_Modifica  = @username_modifica");
        stbQuery.AppendLine("WHERE allegati_documenti_cod = @allegati_documenti_cod");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.allegati_documenti_cod = allegatiDocumentiCod;
        parSql.provider_id = providerId;
        parSql.provider_code = providerCode;
        parSql.data_modifica = DateTime.UtcNow;
        parSql.username_modifica = currentUserId;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error updating raster map mapping for document {allegatiDocumentiCod} and provider {providerId}", serverParams, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteRasterMapMappingAsync(
        int allegatiDocumentiCod,
        string providerId,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("DELETE FROM smart_tractor_provider_mapping_raster_map");
        stbQuery.AppendLine("WHERE allegati_documenti_cod = @allegati_documenti_cod");
        stbQuery.AppendLine("  AND provider_id = @provider_id");

        parSql.allegati_documenti_cod = allegatiDocumentiCod;
        parSql.provider_id = providerId;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting raster map mapping for document {allegatiDocumentiCod} and provider {providerId}", serverParams, ex);
            throw;
        }
    }
}
