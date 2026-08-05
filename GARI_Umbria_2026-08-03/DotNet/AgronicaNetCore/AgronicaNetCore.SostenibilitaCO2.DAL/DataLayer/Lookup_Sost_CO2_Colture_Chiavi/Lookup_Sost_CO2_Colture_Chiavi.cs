using InData.FoodMetaVerse;
using System.Data;
using System.Text;
using System.Dynamic;
using System.Transactions;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Chiavi
{
    public class Lookup_Sost_CO2_Colture_Chiavi : BaseDALSostenibilitaCO2, ILookup_Sost_CO2_Colture_Chiavi
    {
        private readonly IAgro_Sequence _sequenceDal;

        public Lookup_Sost_CO2_Colture_Chiavi(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)
            : base(provider, localizer, securityBypass)
        {
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        private static WriteLookupSostCO2ColtureChiavi Valorizza(WriteLookupSostCO2ColtureChiavi dto)
        {
            dto.Piva_Filiera ??= "";
            dto.Piva_Azienda ??= "";
            dto.Nazione ??= "";
            dto.Username_Creazione ??= "agronica";
            dto.Username_Modifica ??= "agronica";

            if (dto.Data_Invocazione == default)
                dto.Data_Invocazione = DateTime.Now;

            if (dto.Data_Creazione == default)
                dto.Data_Creazione = DateTime.Now;

            if (dto.Data_Modifica == default)
                dto.Data_Modifica = DateTime.Now;

            if (dto.Validita_Inizio == default)
                dto.Validita_Inizio = new DateTime(1900, 1, 1);

            if (dto.Validita_Fine == default)
                dto.Validita_Fine = new DateTime(2100, 12, 31);

            return dto;
        }

        public async Task<DataTable> ReadAsync(GetSostCO2Colture dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Lookup_Sost_CO2_Colture_Chiavi ")
                    .AppendLine("WHERE 1=1 ");

            if (dto.Id.HasValue)
            {
                sqlParams.TryAdd("@id", dto.Id.Value);
                stbQuery.AppendLine("    AND id = @id ");
            }
            if (!string.IsNullOrWhiteSpace(dto.IdInvocazione))
            {
                sqlParams.TryAdd("@idInv", dto.IdInvocazione);
                stbQuery.AppendLine("    AND id_invocazione = @idInv ");
            }
            if (!string.IsNullOrEmpty(dto.PivaAzienda))
            {
                sqlParams.TryAdd("@pivaAzienda", dto.PivaAzienda);
                stbQuery.AppendLine("    AND cuaa_azienda = @pivaAzienda ");
            }
            if (!string.IsNullOrEmpty(dto.PivaFiliera))
            {
                sqlParams.TryAdd("@pivaFiliera", dto.PivaFiliera);
                stbQuery.AppendLine("    AND cuaa_filiera = @pivaFiliera ");
            }
            if (dto.Anno != 0)
            {
                sqlParams.TryAdd("@anno", dto.Anno);
                stbQuery.AppendLine("    AND anno = @anno ");
            }
            if (dto.Appezzamento.HasValue)
            {
                sqlParams.TryAdd("@appezzamento", dto.Appezzamento.Value);
                stbQuery.AppendLine("    AND appezzamento = @appezzamento ");
            }
            if (dto.MatCod.HasValue)
            {
                sqlParams.TryAdd("@matCod", dto.MatCod.Value);
                stbQuery.AppendLine("    AND mat_cod = @matCod ");
            }
            if (!string.IsNullOrEmpty(dto.Lotto))
            {
                sqlParams.TryAdd("@lotto", dto.Lotto);
                stbQuery.AppendLine("    AND lotto = @lotto ");
            }
            if (dto.Inviato.HasValue)
            {
                sqlParams.TryAdd("@inviato", dto.Inviato.Value);
                stbQuery.AppendLine("    AND Inviato = @inviato ");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY data_invocazione DESC ");

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

        public async Task<bool> ExistAsync(int id, AgronicaCoreParametri objP)
        {
            if (id <= 0)
                throw new Exception("id non valorizzato.");

            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@id", id);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) id FROM Lookup_Sost_CO2_Colture_Chiavi ")
                    .AppendLine("WHERE id = @id ");

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

        public async Task<bool> CreateAsync(WriteLookupSostCO2ColtureChiavi dto, AgronicaCoreParametri objP)
        {
            dto = Valorizza(dto);

            if (dto.Id <= 0)
                throw new Exception("id non valorizzato.");

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Lookup_Sost_CO2_Colture_Chiavi ")
                    .AppendLine("    (id, id_invocazione, data_invocazione, anno, cuaa_filiera, cuaa_azienda,")
                    .AppendLine("     appezzamento, nazione, regione, progetto_cod, veg_cod, cul_cod, elem_cod,")
                    .AppendLine("     mat_cod, lotto,")
                    .AppendLine("     Inviato, DataInvio, Data_Creazione, Data_Modifica,")
                    .AppendLine("     Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
                    .AppendLine("VALUES ")
                    .AppendLine("    (@id, @idInv, @dataInv, @anno, @pivaFiliera, @pivaAzienda,")
                    .AppendLine("     @appezzamento, @nazione, @regione, @progettoCod, @vegCod, @culCod, @elemCod,")
                    .AppendLine("     @matCod, @lotto,")
                    .AppendLine("     @inviato, @dataInvio, @dataCreazione, @dataModifica,")
                    .AppendLine("     @usernameCreazione, @usernameModifica, @validitaInizio, @validitaFine) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", dto.Id);
            expandoObj.TryAdd("@idInv", dto.Id_Invocazione);
            expandoObj.TryAdd("@dataInv", dto.Data_Invocazione);
            expandoObj.TryAdd("@anno", dto.Anno);
            expandoObj.TryAdd("@pivaFiliera", dto.Piva_Filiera);
            expandoObj.TryAdd("@pivaAzienda", dto.Piva_Azienda);
            expandoObj.TryAdd("@appezzamento", dto.Appezzamento);
            expandoObj.TryAdd("@nazione", dto.Nazione);
            expandoObj.TryAdd("@regione", (object?)dto.Regione ?? DBNull.Value);
            expandoObj.TryAdd("@progettoCod", (object?)dto.Progetto_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@vegCod", dto.Veg_Cod);
            expandoObj.TryAdd("@culCod", (object?)dto.Cul_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@elemCod", (object?)dto.Elem_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@matCod", (object?)dto.Mat_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@lotto", (object?)dto.Lotto ?? DBNull.Value);
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

        public async Task<bool> UpdateAsync(WriteLookupSostCO2ColtureChiavi dto, AgronicaCoreParametri objP)
        {
            if (dto.Id <= 0)
                throw new Exception("id non valorizzato.");

            if (string.IsNullOrWhiteSpace(dto.Id_Invocazione))
                throw new Exception("id_invocazione non valorizzato.");

            dto = Valorizza(dto);

            const string updateQuery =
                @"UPDATE Lookup_Sost_CO2_Colture_Chiavi SET
                    data_invocazione      = @dataInv,
                    anno                  = @anno,
                    cuaa_filiera          = @pivaFiliera,
                    cuaa_azienda          = @pivaAzienda,
                    appezzamento          = @appezzamento,
                    nazione               = @nazione,
                    regione               = @regione,
                    progetto_cod          = @progettoCod,
                    veg_cod               = @vegCod,
                    cul_cod               = @culCod,
                    elem_cod              = @elemCod,
                    mat_cod               = @matCod,
                    lotto                 = @lotto,
                    Inviato               = @inviato,
                    DataInvio             = @dataInvio,
                    Data_Modifica         = @dataModifica,
                    Username_Modifica     = @usernameModifica,
                    Validita_Inizio       = @validitaInizio,
                    Validita_Fine         = @validitaFine
                WHERE id = @id AND id_invocazione = @idInv";

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", dto.Id);
            expandoObj.TryAdd("@idInv", dto.Id_Invocazione);
            expandoObj.TryAdd("@dataInv", dto.Data_Invocazione);
            expandoObj.TryAdd("@anno", dto.Anno);
            expandoObj.TryAdd("@pivaFiliera", dto.Piva_Filiera);
            expandoObj.TryAdd("@pivaAzienda", dto.Piva_Azienda);
            expandoObj.TryAdd("@appezzamento", dto.Appezzamento);
            expandoObj.TryAdd("@nazione", dto.Nazione);
            expandoObj.TryAdd("@regione", (object?)dto.Regione ?? DBNull.Value);
            expandoObj.TryAdd("@progettoCod", (object?)dto.Progetto_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@vegCod", dto.Veg_Cod);
            expandoObj.TryAdd("@culCod", (object?)dto.Cul_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@elemCod", (object?)dto.Elem_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@matCod", (object?)dto.Mat_Cod ?? DBNull.Value);
            expandoObj.TryAdd("@lotto", (object?)dto.Lotto ?? DBNull.Value);
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

        public async Task<bool> DeleteAsync(int id, string idInvocazione, AgronicaCoreParametri objP)
        {
            if (id <= 0)
                throw new Exception("id non valorizzato.");

            if (string.IsNullOrWhiteSpace(idInvocazione))
                throw new Exception("id_invocazione non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", id);
            expandoObj.TryAdd("@idInv", idInvocazione);

            const string deleteQuery =
                "DELETE FROM Lookup_Sost_CO2_Colture_Chiavi WHERE id = @id AND id_invocazione = @idInv";

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

        public async Task<string> ScriviModificaAsync(WriteLookupSostCO2ColtureChiavi dto, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(dto.Id_Invocazione))
                throw new Exception("id_invocazione non valorizzato.");

            try
            {
                bool isNew = false;

                if (dto.Id <= 0)
                {
                    isNew = true;
                    dto.Id = await _sequenceDal.NuovoId_TabellaAsync("lookup_sost_co2_colture_chiavi", 0, 2000000000, objP);
                }
                else
                    isNew = !await ExistAsync(dto.Id, objP);

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

        public async Task<bool> EliminaAsync(WriteLookupSostCO2ColtureChiavi dto, AgronicaCoreParametriServer objP)
        {
            try
            {
                return await DeleteAsync(dto.Id, dto.Id_Invocazione, objP);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteLookupSostCO2ColtureChiavi> dtos, AgronicaCoreParametriServer objP)
        {
            using (TransactionScope ts = new(objP.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in dtos)
                        await DeleteAsync(dto.Id, dto.Id_Invocazione, objP);

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

