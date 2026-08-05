using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.DAL.Resources;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Dynamic;
using System.Data;
using System.Text;
using System.Transactions;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Chiavi
{
    /// <summary>
    /// Data access for Lookup_Sost_H20_Aziendale_Chiavi.
    /// See Database schema, table lookup_sost_h2o_aziendale_chiavi.
    /// </summary>
    public class Lookup_Sost_H2O_Aziendale_Chiavi : BaseDALSostenibilitaH2O, ILookup_Sost_H2O_Aziendale_Chiavi
    {
        private readonly IAgro_Sequence _sequenceDal;

        public Lookup_Sost_H2O_Aziendale_Chiavi(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)
            : base(provider, localizer, securityBypass)
        {
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        private static WriteLookupSostH2OAziendaleChiavi Valorizza(WriteLookupSostH2OAziendaleChiavi dto)
        {
            dto.Id_Invocazione ??= string.Empty;
            dto.CuaaFiliera ??= string.Empty;
            dto.CuaaAzienda ??= string.Empty;

            if (dto.Data_Calcolo == default)
                dto.Data_Calcolo = DateTime.UtcNow;

            return dto;
        }

        /// <inheritdoc/>
        public async Task<DataTable> ReadAsync(GetSostH2OAziendale dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Lookup_Sost_H2O_Aziendale_Chiavi")
                    .AppendLine("WHERE 1 = 1");

            if (!string.IsNullOrWhiteSpace(dto.IdInvocazione))
            {
                sqlParams.TryAdd("@idInvocazione", dto.IdInvocazione);
                stbQuery.AppendLine("  AND id_invocazione = @idInvocazione");
            }

            if (!string.IsNullOrWhiteSpace(dto.Filiera))
            {
                sqlParams.TryAdd("@cuaaFiliera", dto.Filiera);
                stbQuery.AppendLine("  AND cuaa_filiera = @cuaaFiliera");
            }

            if (!string.IsNullOrWhiteSpace(dto.Azienda))
            {
                sqlParams.TryAdd("@cuaaAzienda", dto.Azienda);
                stbQuery.AppendLine("  AND cuaa_azienda = @cuaaAzienda");
            }

            if (dto.Anno > 0)
            {
                sqlParams.TryAdd("@anno", dto.Anno);
                stbQuery.AppendLine("  AND anno = @anno");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY data_calcolo DESC");

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
        public async Task<bool> ExistAsync(int id, AgronicaCoreParametri objP)
        {
            if (id <= 0)
                throw new Exception("id non valorizzato.");

            var sqlParams = new Dictionary<string, object>
            {
                ["@id"] = id
            };

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) id FROM Lookup_Sost_H2O_Aziendale_Chiavi")
                    .AppendLine("WHERE id = @id");

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
        public async Task<bool> CreateAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametri objP)
        {
            dto = Valorizza(dto);

            if (dto.Id <= 0)
                throw new Exception("id non valorizzato.");

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Lookup_Sost_H2O_Aziendale_Chiavi")
                    .AppendLine("    (id, id_invocazione, data_calcolo, anno, cuaa_filiera, cuaa_azienda, Username_Creazione, Username_Modifica)")
                    .AppendLine("VALUES")
                    .AppendLine("    (@id, @idInvocazione, @dataCalcolo, @anno, @cuaaFiliera, @cuaaAzienda, @usernameCreazione, @usernameModifica)");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", dto.Id);
            expandoObj.TryAdd("@idInvocazione", dto.Id_Invocazione);
            expandoObj.TryAdd("@dataCalcolo", dto.Data_Calcolo);
            expandoObj.TryAdd("@anno", dto.Anno);
            expandoObj.TryAdd("@cuaaFiliera", dto.CuaaFiliera);
            expandoObj.TryAdd("@cuaaAzienda", dto.CuaaAzienda);
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
        public async Task<bool> UpdateAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(dto.Id_Invocazione))
                throw new Exception("id_invocazione non valorizzato.");

            dto = Valorizza(dto);

            const string updateQuery =
                @"UPDATE Lookup_Sost_H2O_Aziendale_Chiavi SET
                    data_calcolo = @dataCalcolo,
                    anno = @anno,
                    cuaa_filiera = @cuaaFiliera,
                    cuaa_azienda = @cuaaAzienda,
                    Username_Modifica = @usernameModifica
                WHERE id_invocazione = @idInvocazione";

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idInvocazione", dto.Id_Invocazione);
            expandoObj.TryAdd("@dataCalcolo", dto.Data_Calcolo);
            expandoObj.TryAdd("@anno", dto.Anno);
            expandoObj.TryAdd("@cuaaFiliera", dto.CuaaFiliera);
            expandoObj.TryAdd("@cuaaAzienda", dto.CuaaAzienda);
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
                "DELETE FROM Lookup_Sost_H2O_Aziendale_Chiavi WHERE id_invocazione = @idInvocazione";

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
        public async Task<string> ScriviModificaAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(dto.Id_Invocazione))
                throw new Exception("id_invocazione non valorizzato.");

            try
            {
                bool isNew = false;

                if (dto.Id <= 0)
                    dto.Id = await _sequenceDal.NuovoId_TabellaAsync("lookup_sost_H2O_aziendale_chiavi", 0, 2000000000, objP);

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

        /// <inheritdoc/>
        public async Task<bool> EliminaAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametriServer objP)
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
        public async Task<bool> EliminaAsync(List<WriteLookupSostH2OAziendaleChiavi> dtos, AgronicaCoreParametriServer objP)
        {
            using (TransactionScope ts = new(objP.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (WriteLookupSostH2OAziendaleChiavi dto in dtos)
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
