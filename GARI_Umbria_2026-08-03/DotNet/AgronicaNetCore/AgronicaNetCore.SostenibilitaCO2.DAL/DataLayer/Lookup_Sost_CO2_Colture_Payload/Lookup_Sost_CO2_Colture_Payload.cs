using InData.FoodMetaVerse;
using System.Data;
using System.Text;
using System.Dynamic;
using System.Transactions;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Payload
{
    public class Lookup_Sost_CO2_Colture_Payload : BaseDALSostenibilitaCO2, ILookup_Sost_CO2_Colture_Payload
    {
        public Lookup_Sost_CO2_Colture_Payload(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)
            : base(provider, localizer, securityBypass)
        {
        }

        private static WriteLookupSostCO2ColturePayload Valorizza(WriteLookupSostCO2ColturePayload dto)
        {
            dto.Json_Richiesta ??= "";
            dto.Json_Risposta ??= "";
            dto.Username_Creazione ??= "agronica";
            dto.Username_Modifica ??= "agronica";

            if (dto.Data_Creazione == default)
                dto.Data_Creazione = DateTime.Now;

            if (dto.Data_Modifica == default)
                dto.Data_Modifica = DateTime.Now ;

            if (dto.Validita_Inizio == default)
                dto.Validita_Inizio = new DateTime(1900, 1, 1);

            if (dto.Validita_Fine == default)
                dto.Validita_Fine = new DateTime(2100, 12, 31);

            return dto;
        }

        public async Task<DataTable> ReadAsync(string? idInvocazione, short? inviato, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Lookup_Sost_CO2_Colture_Payload ")
                    .AppendLine("WHERE 1=1 ");

            if (!string.IsNullOrWhiteSpace(idInvocazione))
            {
                sqlParams.TryAdd("@idInv", idInvocazione);
                stbQuery.AppendLine("    AND id_invocazione = @idInv ");
            }
            if (inviato.HasValue)
            {
                sqlParams.TryAdd("@inviato", inviato.Value);
                stbQuery.AppendLine("    AND Inviato = @inviato ");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY Data_Creazione DESC ");

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

        public async Task<bool> ExistAsync(string idInvocazione, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(idInvocazione))
                throw new Exception("id_invocazione non valorizzato.");

            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@idInv", idInvocazione);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) id_invocazione FROM Lookup_Sost_CO2_Colture_Payload ")
                    .AppendLine("WHERE id_invocazione = @idInv ");

            try
            {
                var dt = await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<bool> CreateAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametri objP)
        {
            dto = Valorizza(dto);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Lookup_Sost_CO2_Colture_Payload ")
                    .AppendLine("    (id_invocazione, json_richiesta, json_risposta,")
                    .AppendLine("     Inviato, DataInvio, Data_Creazione, Data_Modifica,")
                    .AppendLine("     Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
                    .AppendLine("VALUES ")
                    .AppendLine("    (@idInv, @jsonRichiesta, @jsonRisposta,")
                    .AppendLine("     @inviato, @dataInvio, @dataCreazione, @dataModifica,")
                    .AppendLine("     @usernameCreazione, @usernameModifica, @validitaInizio, @validitaFine) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idInv", dto.Id_Invocazione);
            expandoObj.TryAdd("@jsonRichiesta", dto.Json_Richiesta);
            expandoObj.TryAdd("@jsonRisposta", dto.Json_Risposta);
            expandoObj.TryAdd("@inviato", (object?)dto.Inviato ?? DBNull.Value);
            expandoObj.TryAdd("@dataInvio", (object?)dto.DataInvio ?? DBNull.Value);
            expandoObj.TryAdd("@dataCreazione", dto.Data_Creazione);
            expandoObj.TryAdd("@dataModifica", dto.Data_Modifica);
            expandoObj.TryAdd("@usernameCreazione", dto.Username_Creazione);
            expandoObj.TryAdd("@usernameModifica", dto.Username_Modifica);
            expandoObj.TryAdd("@validitaInizio", dto.Validita_Inizio);
            expandoObj.TryAdd("@validitaFine", dto.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(dto.Id_Invocazione))
                throw new Exception("id_invocazione non valorizzato.");

            dto = Valorizza(dto);

            // Preserve existing request payload when caller is only updating response fields.
            if (string.IsNullOrWhiteSpace(dto.Json_Richiesta))
            {
                dto.Json_Richiesta = await LeggiJsonRichiestaCorrenteAsync(dto.Id_Invocazione, objP);
            }

            const string updateQuery =
                @"UPDATE Lookup_Sost_CO2_Colture_Payload SET
                    json_risposta     = @jsonRisposta,
                    Inviato           = @inviato,
                    DataInvio         = @dataInvio,
                    Data_Modifica     = @dataModifica,
                    Username_Modifica = @usernameModifica,
                    Validita_Inizio   = @validitaInizio,
                    Validita_Fine     = @validitaFine
                WHERE id_invocazione = @idInv";

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idInv", dto.Id_Invocazione);
            expandoObj.TryAdd("@jsonRisposta", dto.Json_Risposta);
            expandoObj.TryAdd("@inviato", (object?)dto.Inviato ?? DBNull.Value);
            expandoObj.TryAdd("@dataInvio", (object?)dto.DataInvio ?? DBNull.Value);
            expandoObj.TryAdd("@dataModifica", dto.Data_Modifica);
            expandoObj.TryAdd("@usernameModifica", dto.Username_Modifica);
            expandoObj.TryAdd("@validitaInizio", dto.Validita_Inizio);
            expandoObj.TryAdd("@validitaFine", dto.Validita_Fine);

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

        private async Task<string> LeggiJsonRichiestaCorrenteAsync(string idInvocazione, AgronicaCoreParametri objP)
        {
            var current = await ReadAsync(idInvocazione, null, objP);
            if (current.Rows.Count == 0)
                throw new Exception($"Record non trovato per id_invocazione={idInvocazione}.");

            return current.Rows[0].Field<string>("json_richiesta") ?? string.Empty;
        }

        public async Task<bool> DeleteAsync(string idInvocazione, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(idInvocazione))
                throw new Exception("id_invocazione non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idInv", idInvocazione);

            const string deleteQuery =
                "DELETE FROM Lookup_Sost_CO2_Colture_Payload WHERE id_invocazione = @idInv";

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

        public async Task<string> ScriviModificaAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametri objP)
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

        public async Task<bool> EliminaAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametriServer objP)
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

        public async Task<bool> EliminaAsync(List<WriteLookupSostCO2ColturePayload> dtos, AgronicaCoreParametriServer objP)
        {
            using (TransactionScope ts = new(objP.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in dtos)
                        await DeleteAsync(dto.Id_Invocazione, objP);

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objP, ex);
                    throw;
                }
                finally
                {
                    if (objP.objTransazione == null)
                        ts.Dispose();
                }
            }

            return true;
        }
    }
}

