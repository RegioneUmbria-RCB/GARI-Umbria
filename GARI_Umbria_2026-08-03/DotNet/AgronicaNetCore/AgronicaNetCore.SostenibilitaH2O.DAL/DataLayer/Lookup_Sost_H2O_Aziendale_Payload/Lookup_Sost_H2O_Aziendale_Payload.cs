using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.DAL.Resources;
using InData.FoodMetaVerse;
using Microsoft.Extensions.Localization;
using System.Dynamic;
using System.Data;
using System.Text;
using System.Transactions;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Payload
{
    /// <summary>
    /// Data access for Lookup_Sost_H20_Aziendale_Payload.
    /// See Database schema, table lookup_sost_h2o_aziendale_payload.
    /// </summary>
    public class Lookup_Sost_H2O_Aziendale_Payload : BaseDALSostenibilitaH2O, ILookup_Sost_H2O_Aziendale_Payload
    {
        public Lookup_Sost_H2O_Aziendale_Payload(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)
            : base(provider, localizer, securityBypass)
        {
        }

        private static WriteLookupSostH2OAziendalePayload Valorizza(WriteLookupSostH2OAziendalePayload dto)
        {
            dto.Id_Invocazione ??= string.Empty;
            dto.Payload_Json ??= string.Empty;
            dto.Json_Firmato ??= Array.Empty<byte>();

            return dto;
        }

        /// <inheritdoc/>
        public async Task<DataTable> ReadAsync(string idInvocazione, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            if (string.IsNullOrWhiteSpace(idInvocazione))
                throw new ArgumentException("Parametro 'idInvocazione' obbligatorio.", nameof(idInvocazione));

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>
            {
                ["@idInvocazione"] = idInvocazione
            };

            stbQuery.AppendLine("SELECT * FROM Lookup_Sost_H2O_Aziendale_Payload")
                    .AppendLine("WHERE id_invocazione = @idInvocazione");

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY id_invocazione");

            try
            {
                return await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistAsync(string idInvocazione, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(idInvocazione))
                throw new Exception("id_invocazione non valorizzato.");

            var sqlParams = new Dictionary<string, object>
            {
                ["@idInvocazione"] = idInvocazione
            };

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) id_invocazione FROM Lookup_Sost_H2O_Aziendale_Payload")
                    .AppendLine("WHERE id_invocazione = @idInvocazione");

            try
            {
                DataTable dt = await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CreateAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametri objP)
        {
            dto = Valorizza(dto);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Lookup_Sost_H2O_Aziendale_Payload")
                    .AppendLine("    (id_invocazione, payload_json, json_firmato, Username_Creazione, Username_Modifica)")
                    .AppendLine("VALUES")
                    .AppendLine("    (@idInvocazione, @payloadJson, @jsonFirmato, @usernameCreazione, @usernameModifica)");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idInvocazione", dto.Id_Invocazione);
            expandoObj.TryAdd("@payloadJson", dto.Payload_Json);
            expandoObj.TryAdd("@jsonFirmato", dto.Json_Firmato.Length == 0 ? DBNull.Value : dto.Json_Firmato);
            expandoObj.TryAdd("@usernameCreazione", dto.Username_Creazione);
            expandoObj.TryAdd("@usernameModifica", dto.Username_Modifica);

            try
            {
                return await GetDataProvider(objP).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(dto.Id_Invocazione))
                throw new Exception("id_invocazione non valorizzato.");

            dto = Valorizza(dto);

            const string updateQuery =
                @"UPDATE Lookup_Sost_H2O_Aziendale_Payload SET
                    payload_json = @payloadJson,
                    json_firmato = @jsonFirmato,
                    Username_Modifica = @usernameModifica
                WHERE id_invocazione = @idInvocazione";

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idInvocazione", dto.Id_Invocazione);
            expandoObj.TryAdd("@payloadJson", dto.Payload_Json);
            expandoObj.TryAdd("@jsonFirmato", dto.Json_Firmato.Length == 0 ? DBNull.Value : dto.Json_Firmato);
            expandoObj.TryAdd("@usernameModifica", dto.Username_Modifica);

            try
            {
                return await GetDataProvider(objP).Execute_WriteAsync(updateQuery, expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(string idInvocazione, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(idInvocazione))
                throw new Exception("id_invocazione non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idInvocazione", idInvocazione);

            const string deleteQuery =
                "DELETE FROM Lookup_Sost_H2O_Aziendale_Payload WHERE id_invocazione = @idInvocazione";

            try
            {
                return await GetDataProvider(objP).Execute_WriteAsync(deleteQuery, expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<string> ScriviModificaAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(dto.Id_Invocazione))
                throw new Exception("id_invocazione non valorizzato.");

            try
            {
                bool isNew = !await ExistAsync(dto.Id_Invocazione, objP);

                if (isNew)
                    await CreateAsync(dto, objP);
                else
                    await UpdateAsync(dto, objP);

                return dto.Id_Invocazione;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EliminaAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametriServer objP)
        {
            try
            {
                return await DeleteAsync(dto.Id_Invocazione, objP);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EliminaAsync(List<WriteLookupSostH2OAziendalePayload> dtos, AgronicaCoreParametriServer objP)
        {
            using (TransactionScope ts = new(objP.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (WriteLookupSostH2OAziendalePayload dto in dtos)
                        await DeleteAsync(dto.Id_Invocazione, objP);

                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objP, ex);
                    throw;
                }
            }
        }
    }
}
