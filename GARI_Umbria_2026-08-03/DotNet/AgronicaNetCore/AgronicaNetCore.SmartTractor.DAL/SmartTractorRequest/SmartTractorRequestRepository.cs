using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.DAL.DataLayer;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.DAL.SmartTractorRequest;

/// <summary>
/// Data Access Layer for the <c>smart_tractor_request</c> table.
/// Records every outgoing Smart Tractor request for status tracking and
/// idempotency checks.
///
/// Method Conventions:
/// - INSERT methods use <c>Execute_WriteAsync()</c> with ExpandoObject parameters.
///
/// Referenced in Design Specification: DS03-BL - Composizione Payload
/// (Tracciamento Persistente Post-Composizione)
/// </summary>
public class SmartTractorRequestRepository : BaseDALSmartTractor, ISmartTractorRequestRepository
{
    private readonly IAgro_Sequence _sequenceDal;

    public SmartTractorRequestRepository(
        IServiceProvider provider,
        IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
        _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
    }

    /// <inheritdoc/>
    public async Task<bool> InsertRequestAsync(
        string providerId,
        int ricettaOperazioneCod,
        int macCod,
        string smartTractorPrescriptionId,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("INSERT INTO smart_tractor_request_tracking");
        stbQuery.AppendLine("    (id, prescription_id, request_id_smarttractor, provider_id, machine_id, user_id,");
        stbQuery.AppendLine("     status, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica)");
        stbQuery.AppendLine("VALUES");
        stbQuery.AppendLine("    (@id, @agronica_activity_id, @request_id_smarttractor, @provider_id, @machine_id, @agronica_user_id,");
        stbQuery.AppendLine("     @status, @created_at, @updated_at, @created_by, @updated_by)");

        var id = await _sequenceDal.NuovoId_TabellaAsync("smart_tractor_request_tracking", 0, 2000000000, serverParams);
        parSql.id = id;
        parSql.agronica_activity_id = ricettaOperazioneCod;
        parSql.request_id_smarttractor = smartTractorPrescriptionId;
        parSql.provider_id = providerId;
        parSql.machine_id = macCod;
        parSql.agronica_user_id = serverParams.UsernameOperazione;
        parSql.status = "PENDING";
        parSql.created_at = DateTime.UtcNow;
        parSql.updated_at = DateTime.UtcNow;
        parSql.created_by = serverParams.UtenteUsername;
        parSql.updated_by = serverParams.UtenteUsername;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError(
                $"Error inserting smart_tractor_request_tracking for activity={ricettaOperazioneCod}",
                serverParams,
                ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateRequestStatusAsync(
        int ricettaOperazioneCod,
        string status,
        long? providerResponseId,
        string? errorDetail,
        string currentUserId,
        AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        dynamic parSql = new ExpandoObject();

        stbQuery.AppendLine("UPDATE smart_tractor_request_tracking");
        stbQuery.AppendLine("SET    status = @status,");
        stbQuery.AppendLine("       provider_response_id = @request_id_smarttractor,");
        stbQuery.AppendLine("       error_detail = @error_detail,");
        stbQuery.AppendLine("       updated_at = @updated_at,");
        stbQuery.AppendLine("       updated_by = @updated_by");
        stbQuery.AppendLine("WHERE  prescription_id = @ricetta_operazione_cod");

        parSql.ricetta_operazione_cod = ricettaOperazioneCod;
        parSql.status = status;
        parSql.provider_response_id = (object?)providerResponseId ?? DBNull.Value;
        parSql.error_detail = (object?)errorDetail ?? DBNull.Value;
        parSql.updated_at = DateTime.UtcNow;
        parSql.updated_by = currentUserId;

        try
        {
            return await GetDataProvider(serverParams).Execute_WriteAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError(
                $"Error updating smart_tractor_request_tracking status for id={ricettaOperazioneCod} to status={status}",
                serverParams,
                ex);
            throw;
        }
    }
}
