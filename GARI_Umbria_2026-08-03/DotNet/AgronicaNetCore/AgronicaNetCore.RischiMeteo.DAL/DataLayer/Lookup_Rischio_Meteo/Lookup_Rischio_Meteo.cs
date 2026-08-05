using InData.FoodMetaVerse;
using System.Data;
using System.Text;
using System.Dynamic;
using System.Transactions;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.RischiMeteo.DAL.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.RischiMeteo.DAL.DataLayer.Lookup_Rischio_Meteo
{
    public class Lookup_Rischio_Meteo : BaseDALRischiMeteo, ILookup_Rischio_Meteo
    {
        private readonly IAgro_Sequence _sequenceDal;

        public Lookup_Rischio_Meteo(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)
            : base(provider, localizer, securityBypass)
        {
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        public async Task<(int CodSpecie, int CodVarieta)?> ResolveCodSpecieVarietaDaProdottoAsync(int elemCod, int matCod, AgronicaCoreParametri objP)
        {
            ArgumentNullException.ThrowIfNull(objP);

            if (elemCod <= 0)
                throw new ArgumentOutOfRangeException(nameof(elemCod), "elemCod deve essere > 0.");

            if (matCod <= 0)
                throw new ArgumentOutOfRangeException(nameof(matCod), "matCod deve essere > 0.");

            var sqlParams = new Dictionary<string, object>
            {
                ["@elemCod"] = elemCod,
                ["@matCod"] = matCod
            };

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) Cul_Cod, Veg_Cod")
                .AppendLine("FROM Materie_Prime")
                .AppendLine("WHERE Elem_Cod = @elemCod")
                .AppendLine("  AND Mat_Cod = @matCod");

            try
            {
                DataTable dt = await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                if (dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];
                if (row["Cul_Cod"] == DBNull.Value || row["Veg_Cod"] == DBNull.Value)
                    return null;

                int codSpecie = Convert.ToInt32(row["Cul_Cod"]);
                int codVarieta = Convert.ToInt32(row["Veg_Cod"]);

                return (codSpecie, codVarieta);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        private static WriteLookupRischioMeteo Valorizza(WriteLookupRischioMeteo dto)
        {
            dto.Cuaa_Filiera ??= "";
            dto.Cuaa_Azienda ??= "";
            dto.Nazione ??= "";
            dto.Regione ??= "";
            dto.Centroide_Wkt ??= "";
            dto.Poligono_Wkt ??= "";
            dto.Epsg ??= "";
            dto.Json_Richiesta ??= "";
            dto.Json_Risposta ??= "";
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

        public async Task<DataTable> ReadAsync(GetLookupRischioMeteo dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Lookup_Rischio_Meteo ")
                .AppendLine("WHERE 1=1 ");

            if (!string.IsNullOrWhiteSpace(dto.Cuaa_Filiera))
            {
                sqlParams.TryAdd("@cuaaFiliera", dto.Cuaa_Filiera);
                stbQuery.AppendLine("    AND cuaa_filiera = @cuaaFiliera ");
            }
            if (!string.IsNullOrWhiteSpace(dto.Cuaa_Azienda))
            {
                sqlParams.TryAdd("@cuaaAzienda", dto.Cuaa_Azienda);
                stbQuery.AppendLine("    AND cuaa_azienda = @cuaaAzienda ");
            }
            if (dto.Id_Appezzamento.HasValue)
            {
                sqlParams.TryAdd("@idAppezzamento", dto.Id_Appezzamento.Value);
                stbQuery.AppendLine("    AND id_appezzamento = @idAppezzamento ");
            }
            if (dto.Id_Esercizio.HasValue)
            {
                sqlParams.TryAdd("@idEsercizio", dto.Id_Esercizio.Value);
                stbQuery.AppendLine("    AND id_esercizio = @idEsercizio ");
            }
            if (dto.Anno_Esercizio.HasValue)
            {
                sqlParams.TryAdd("@annoEsercizio", dto.Anno_Esercizio.Value);
                stbQuery.AppendLine("    AND anno_esercizio = @annoEsercizio ");
            }
            if (dto.Cod_Specie.HasValue)
            {
                sqlParams.TryAdd("@codSpecie", dto.Cod_Specie.Value);
                stbQuery.AppendLine("    AND cod_specie = @codSpecie ");
            }
            if (dto.Cod_Varieta.HasValue)
            {
                sqlParams.TryAdd("@codVarieta", dto.Cod_Varieta.Value);
                stbQuery.AppendLine("    AND cod_varieta = @codVarieta ");
            }
            if (!string.IsNullOrWhiteSpace(dto.Nazione))
            {
                sqlParams.TryAdd("@nazione", dto.Nazione);
                stbQuery.AppendLine("    AND nazione = @nazione ");
            }
            if (!string.IsNullOrWhiteSpace(dto.Regione))
            {
                sqlParams.TryAdd("@regione", dto.Regione);
                stbQuery.AppendLine("    AND regione = @regione ");
            }
            if (dto.Inviato.HasValue)
            {
                sqlParams.TryAdd("@inviato", dto.Inviato.Value);
                stbQuery.AppendLine("    AND Inviato = @inviato ");
            }
            if (dto.Data_Invocazione_Da.HasValue)
            {
                sqlParams.TryAdd("@dataInvocazioneDa", dto.Data_Invocazione_Da.Value);
                stbQuery.AppendLine("    AND data_invocazione >= @dataInvocazioneDa ");
            }
            if (dto.Data_Invocazione_A.HasValue)
            {
                sqlParams.TryAdd("@dataInvocazioneA", dto.Data_Invocazione_A.Value);
                stbQuery.AppendLine("    AND data_invocazione <= @dataInvocazioneA ");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY cuaa_azienda, id_appezzamento ");

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

        public async Task<bool> ExistAsync(string cuaaFiliera, string cuaaAzienda, int idAppezzamento, int idEsercizio, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new Exception("cuaa_filiera non valorizzato.");

            if (string.IsNullOrWhiteSpace(cuaaAzienda))
                throw new Exception("cuaa_azienda non valorizzato.");

            if (idAppezzamento <= 0)
                throw new Exception("id_appezzamento non valorizzato.");

            if (idEsercizio <= 0)
                throw new Exception("id_esercizio non valorizzato.");

            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@cuaaFiliera", cuaaFiliera);
            sqlParams.TryAdd("@cuaaAzienda", cuaaAzienda);
            sqlParams.TryAdd("@idAppezzamento", idAppezzamento);
            sqlParams.TryAdd("@idEsercizio", idEsercizio);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) id FROM Lookup_Rischio_Meteo ")
                .AppendLine("WHERE cuaa_filiera = @cuaaFiliera ")
                .AppendLine("  AND cuaa_azienda = @cuaaAzienda ")
                .AppendLine("  AND id_appezzamento = @idAppezzamento ")
                .AppendLine("  AND id_esercizio = @idEsercizio ");

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

        public async Task<bool> CreateAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametri objP)
        {
            dto = Valorizza(dto);

            if (dto.Id <= 0)
                throw new Exception("id non valorizzato.");

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Lookup_Rischio_Meteo ")
                .AppendLine("    (id, cuaa_filiera, cuaa_azienda, id_appezzamento, id_esercizio, anno_esercizio,")
                .AppendLine("     cod_specie, cod_varieta,")
                .AppendLine("     nazione, regione, centroide_wkt, poligono_wkt, epsg,")
                .AppendLine("     superficie_ha, json_richiesta, json_risposta,")
                .AppendLine("     rischio_gelo, rischio_siccita, rischio_allagamento,")
                .AppendLine("     Inviato, DataInvio, Data_Creazione, Data_Modifica,")
                .AppendLine("     Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine,")
                .AppendLine("     data_invocazione) ")
                .AppendLine("VALUES ")
                .AppendLine("    (@id, @cuaaFiliera, @cuaaAzienda, @idAppezzamento, @idEsercizio, @annoEsercizio,")
                .AppendLine("    @codSpecie, @codVarieta,")
                .AppendLine("    @nazione, @regione, @centroideWkt, @poligonoWkt, @epsg,")
                .AppendLine("    @superficieHa, @jsonRichiesta, @jsonRisposta,")
                .AppendLine("    @rischioGelo, @rischioSiccita, @rischioAllagamento,")
                .AppendLine("    @inviato, @dataInvio, @dataCreazione, @dataModifica,")
                .AppendLine("    @usernameCreazione, @usernameModifica, @validitaInizio, @validitaFine,")
                .AppendLine("    @dataInvocazione);");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", dto.Id);
            expandoObj.TryAdd("@cuaaFiliera", dto.Cuaa_Filiera);
            expandoObj.TryAdd("@cuaaAzienda", dto.Cuaa_Azienda);
            expandoObj.TryAdd("@idAppezzamento", dto.Id_Appezzamento);
            expandoObj.TryAdd("@idEsercizio", dto.Id_Esercizio);
            expandoObj.TryAdd("@annoEsercizio", dto.Anno_Esercizio);
            expandoObj.TryAdd("@codSpecie", dto.Cod_Specie);
            expandoObj.TryAdd("@codVarieta", (object?)dto.Cod_Varieta ?? DBNull.Value);
            expandoObj.TryAdd("@nazione", dto.Nazione);
            expandoObj.TryAdd("@regione", (object?)dto.Regione ?? DBNull.Value);
            expandoObj.TryAdd("@centroideWkt", dto.Centroide_Wkt);
            expandoObj.TryAdd("@poligonoWkt", (object?)dto.Poligono_Wkt ?? DBNull.Value);
            expandoObj.TryAdd("@epsg", dto.Epsg);
            expandoObj.TryAdd("@superficieHa", dto.Superficie_Ha);
            expandoObj.TryAdd("@jsonRichiesta", dto.Json_Richiesta);
            expandoObj.TryAdd("@jsonRisposta", dto.Json_Risposta);
            expandoObj.TryAdd("@rischioGelo", (object?)dto.Rischio_Gelo ?? DBNull.Value);
            expandoObj.TryAdd("@rischioSiccita", (object?)dto.Rischio_Siccita ?? DBNull.Value);
            expandoObj.TryAdd("@rischioAllagamento", (object?)dto.Rischio_Allagamento ?? DBNull.Value);
            expandoObj.TryAdd("@inviato", (object?)dto.Inviato ?? DBNull.Value);
            expandoObj.TryAdd("@dataInvio", (object?)dto.DataInvio ?? DBNull.Value);
            expandoObj.TryAdd("@dataCreazione", dto.Data_Creazione);
            expandoObj.TryAdd("@dataModifica", dto.Data_Modifica);
            expandoObj.TryAdd("@usernameCreazione", dto.Username_Creazione);
            expandoObj.TryAdd("@usernameModifica", dto.Username_Modifica);
            expandoObj.TryAdd("@validitaInizio", dto.Validita_Inizio);
            expandoObj.TryAdd("@validitaFine", dto.Validita_Fine);
            expandoObj.TryAdd("@dataInvocazione", dto.Data_Invocazione);

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

        public async Task<bool> UpdateAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametri objP)
        {
            if (dto.Id <= 0)
                throw new Exception("id non valorizzato.");

            if (string.IsNullOrWhiteSpace(dto.Cuaa_Filiera))
                throw new Exception("cuaa_filiera non valorizzato.");

            if (string.IsNullOrWhiteSpace(dto.Cuaa_Azienda))
                throw new Exception("cuaa_azienda non valorizzato.");

            if (dto.Id_Appezzamento <= 0)
                throw new Exception("id_appezzamento non valorizzato.");

            if (dto.Id_Esercizio <= 0)
                throw new Exception("id_esercizio non valorizzato.");

            dto = Valorizza(dto);

            const string updateQuery =
                @"UPDATE Lookup_Rischio_Meteo SET
                    anno_esercizio      = @annoEsercizio,
                    cod_specie          = @codSpecie,
                    cod_varieta         = @codVarieta,
                    nazione             = @nazione,
                    regione             = @regione,
                    centroide_wkt       = @centroideWkt,
                    poligono_wkt        = @poligonoWkt,
                    epsg                = @epsg,
                    superficie_ha       = @superficieHa,
                    json_richiesta      = @jsonRichiesta,
                    json_risposta       = @jsonRisposta,
                    rischio_gelo        = @rischioGelo,
                    rischio_siccita     = @rischioSiccita,
                    rischio_allagamento = @rischioAllagamento,
                    data_invocazione    = @dataInvocazione,
                    Inviato             = @inviato,
                    DataInvio           = @dataInvio,
                    Data_Modifica       = @dataModifica,
                    Username_Modifica   = @usernameModifica,
                    Validita_Inizio     = @validitaInizio,
                    Validita_Fine       = @validitaFine
                WHERE id = @id
                  AND cuaa_filiera = @cuaaFiliera
                  AND cuaa_azienda = @cuaaAzienda
                  AND id_appezzamento = @idAppezzamento
                  AND id_esercizio = @idEsercizio";

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", dto.Id);
            expandoObj.TryAdd("@cuaaFiliera", dto.Cuaa_Filiera);
            expandoObj.TryAdd("@cuaaAzienda", dto.Cuaa_Azienda);
            expandoObj.TryAdd("@idAppezzamento", dto.Id_Appezzamento);
            expandoObj.TryAdd("@idEsercizio", dto.Id_Esercizio);
            expandoObj.TryAdd("@annoEsercizio", dto.Anno_Esercizio);
            expandoObj.TryAdd("@codSpecie", dto.Cod_Specie);
            expandoObj.TryAdd("@codVarieta", (object?)dto.Cod_Varieta ?? DBNull.Value);
            expandoObj.TryAdd("@nazione", dto.Nazione);
            expandoObj.TryAdd("@regione", (object?)dto.Regione ?? DBNull.Value);
            expandoObj.TryAdd("@centroideWkt", dto.Centroide_Wkt);
            expandoObj.TryAdd("@poligonoWkt", (object?)dto.Poligono_Wkt ?? DBNull.Value);
            expandoObj.TryAdd("@epsg", dto.Epsg);
            expandoObj.TryAdd("@superficieHa", dto.Superficie_Ha);
            expandoObj.TryAdd("@jsonRichiesta", dto.Json_Richiesta);
            expandoObj.TryAdd("@jsonRisposta", dto.Json_Risposta);
            expandoObj.TryAdd("@rischioGelo", (object?)dto.Rischio_Gelo ?? DBNull.Value);
            expandoObj.TryAdd("@rischioSiccita", (object?)dto.Rischio_Siccita ?? DBNull.Value);
            expandoObj.TryAdd("@rischioAllagamento", (object?)dto.Rischio_Allagamento ?? DBNull.Value);
            expandoObj.TryAdd("@dataInvocazione", dto.Data_Invocazione);
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

        public async Task<bool> DeleteAsync(int id, string cuaaFiliera, string cuaaAzienda, int idAppezzamento, int idEsercizio, AgronicaCoreParametri objP)
        {
            if (id <= 0)
                throw new Exception("id non valorizzato.");

            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new Exception("cuaa_filiera non valorizzato.");

            if (string.IsNullOrWhiteSpace(cuaaAzienda))
                throw new Exception("cuaa_azienda non valorizzato.");

            if (idAppezzamento <= 0)
                throw new Exception("id_appezzamento non valorizzato.");

            if (idEsercizio <= 0)
                throw new Exception("id_esercizio non valorizzato.");

            var expandoObj = new ExpandoObject();
                        expandoObj.TryAdd("@id", id);
            expandoObj.TryAdd("@cuaaFiliera", cuaaFiliera);
            expandoObj.TryAdd("@cuaaAzienda", cuaaAzienda);
            expandoObj.TryAdd("@idAppezzamento", idAppezzamento);
            expandoObj.TryAdd("@idEsercizio", idEsercizio);

            const string deleteQuery =
                                @"DELETE FROM Lookup_Rischio_Meteo
                                    WHERE id = @id
                                        AND cuaa_filiera = @cuaaFiliera
                    AND cuaa_azienda = @cuaaAzienda
                    AND id_appezzamento = @idAppezzamento
                    AND id_esercizio = @idEsercizio";

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

        public async Task<int> RetrieveIdAsync(string cuaaFiliera, string cuaaAzienda, int idAppezzamento, int idEsercizio, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new Exception("cuaa_filiera non valorizzato.");

            if (string.IsNullOrWhiteSpace(cuaaAzienda))
                throw new Exception("cuaa_azienda non valorizzato.");

            if (idAppezzamento <= 0)
                throw new Exception("id_appezzamento non valorizzato.");

            if (idEsercizio <= 0)
                throw new Exception("id_esercizio non valorizzato.");

            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@cuaaFiliera", cuaaFiliera);
            sqlParams.TryAdd("@cuaaAzienda", cuaaAzienda);
            sqlParams.TryAdd("@idAppezzamento", idAppezzamento);
            sqlParams.TryAdd("@idEsercizio", idEsercizio);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) id FROM Lookup_Rischio_Meteo ")
                .AppendLine("WHERE cuaa_filiera = @cuaaFiliera ")
                .AppendLine("  AND cuaa_azienda = @cuaaAzienda ")
                .AppendLine("  AND id_appezzamento = @idAppezzamento ")
                .AppendLine("  AND id_esercizio = @idEsercizio ");

            try
            {
                var dt = await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                if (dt.Rows.Count == 1)
                    return Convert.ToInt32(dt.Rows[0]["id"]);
                else
                    return 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<int> ScriviModificaAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametri objP)
        {
            if (string.IsNullOrWhiteSpace(dto.Cuaa_Filiera))
                throw new Exception("cuaa_filiera non valorizzato.");

            if (string.IsNullOrWhiteSpace(dto.Cuaa_Azienda))
                throw new Exception("cuaa_azienda non valorizzato.");

            if (dto.Id_Appezzamento <= 0)
                throw new Exception("id_appezzamento non valorizzato.");

            if (dto.Id_Esercizio <= 0)
                throw new Exception("id_esercizio non valorizzato.");

            try
            {
                bool isNew = false;

                if (dto.Id <= 0)
                {
                    dto.Id = await _sequenceDal.NuovoId_TabellaAsync("lookup_rischio_meteo", 0, 2000000000, objP);
                    isNew = true;
                }
                else
                    isNew = !await ExistAsync(dto.Cuaa_Filiera, dto.Cuaa_Azienda, dto.Id_Appezzamento, dto.Id_Esercizio, objP);

                if (isNew)
                    await CreateAsync(dto, objP);
                else
                    await UpdateAsync(dto, objP);

                return dto.Id;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametriServer objP)
        {
            try
            {
                return await DeleteAsync(dto.Id, dto.Cuaa_Filiera, dto.Cuaa_Azienda, dto.Id_Appezzamento, dto.Id_Esercizio, objP);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteLookupRischioMeteo> dtos, AgronicaCoreParametriServer objP)
        {
            using (TransactionScope ts = new(objP.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in dtos)
                        await DeleteAsync(dto.Id, dto.Cuaa_Filiera, dto.Cuaa_Azienda, dto.Id_Appezzamento, dto.Id_Esercizio, objP);

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