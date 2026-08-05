using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.DAL.Resources;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Text;
using System.Data;
using System.Dynamic;
using System.Transactions;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Lotto
{
    /// <summary>
    /// Data access for Lookup_Sost_H20_Lotto.
    /// See Database schema, table lookup_sost_h2o_lotto.
    /// </summary>
    public class Lookup_Sost_H2O_Lotto : BaseDALSostenibilitaH2O, ILookup_Sost_H2O_Lotto
    {
        private readonly IAgro_Sequence _sequenceDal;
        public Lookup_Sost_H2O_Lotto(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)

            : base(provider, localizer, securityBypass)
        {
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        /// <inheritdoc/>
        public async Task<DataTable> ReadAsync(GetSostH2OLotto dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Lookup_Sost_H2O_Lotto")
                    .AppendLine("WHERE 1 = 1");

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

            if (!string.IsNullOrWhiteSpace(dto.CodProdottoFmp))
            {
                // Resolve FMP product code to GIAS veg_cod via CAC_Codifica_Veg_Cod.
                sqlParams.TryAdd("@codProdottoFmp", dto.CodProdottoFmp);
                stbQuery.AppendLine("  AND veg_cod IN (SELECT veg_cod_gias FROM CAC_Codifica_Veg_Cod WHERE veg_cod_coltiva = @codProdottoFmp)");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY data_calcolo DESC, cuaa_azienda ASC, anno DESC");

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
        public async Task<DataTable> ReadPerLottiBatchAsync(GetSostH2OLottiBatch dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Lookup_Sost_H2O_Lotto")
                    .AppendLine("WHERE 1 = 1");

            if (!string.IsNullOrWhiteSpace(dto.Filiera))
            {
                sqlParams.TryAdd("@cuaaFiliera", dto.Filiera);
                stbQuery.AppendLine("  AND cuaa_filiera = @cuaaFiliera");
            }

            if (!string.IsNullOrWhiteSpace(dto.CodProdottoFmp))
            {
                sqlParams.TryAdd("@codProdottoFmp", dto.CodProdottoFmp);
                stbQuery.AppendLine("  AND veg_cod IN (SELECT veg_cod_gias FROM CAC_Codifica_Veg_Cod WHERE veg_cod_coltiva = @codProdottoFmp)");
            }

            if (dto.Lotti != null && dto.Lotti.Count > 0)
            {
                var conditions = new List<string>();

                for (var i = 0; i < dto.Lotti.Count; i++)
                {
                    var lot = dto.Lotti[i];
                    var aziendaKey = $"@azienda_{i}";
                    var lottoKey = $"@lotto_{i}";

                    sqlParams.TryAdd(aziendaKey, lot.Azienda ?? string.Empty);
                    sqlParams.TryAdd(lottoKey, lot.CodLottoFmp ?? string.Empty);
                    conditions.Add($"(cuaa_azienda = {aziendaKey} AND lotto_raccolta = {lottoKey})");
                }

                if (conditions.Count > 0)
                    stbQuery.AppendLine($"  AND ({string.Join(" OR ", conditions)})");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY data_calcolo DESC, cuaa_azienda ASC, lotto_raccolta ASC, anno DESC");

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
        public async Task<bool> CreateAsync(WriteLookupSostH2OLotto dto, AgronicaCoreParametri objP)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.DataCalcolo == default)
                dto.DataCalcolo = DateTime.UtcNow;

            if (dto.Id <= 0)
                dto.Id = await _sequenceDal.NuovoId_TabellaAsync("lookup_sost_h2o_lotto", 0, 2000000000, objP);

            const string insertQuery =
                @"INSERT INTO Lookup_Sost_H2O_Lotto
                    (id, data_calcolo, cuaa_filiera, cuaa_azienda, anno, appezzamento, cul_cod, veg_cod, esercizio,
                     lotto_raccolta, nazione, regione, quantita_raccolta_kg, superficie_riferimento,
                     payload_json, json_firmato, Username_Creazione, Username_Modifica)
                VALUES
                    (@id, @dataCalcolo, @cuaaFiliera, @cuaaAzienda, @anno, @appezzamento, @culCod, @vegCod, @esercizio,
                     @lottoRaccolta, @nazione, @regione, @quantitaRaccoltaKg, @superficieRiferimento,
                     @payloadJson, @jsonFirmato, @usernameCreazione, @usernameModifica)";

            var expandoObj = BuildParams(dto);

            try
            {
                return await GetDataProvider(objP).Execute_WriteAsync(insertQuery, expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> InsertBatchAsync(IReadOnlyList<WriteLookupSostH2OLotto> righe, AgronicaCoreParametri objP)
        {
            ArgumentNullException.ThrowIfNull(righe);

            if (righe.Count == 0)
                return 0;

            int righeInserite = 0;

            using var ts = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TimeSpan(0, 10, 0),
                TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                foreach (var riga in righe)
                {
                    await CreateAsync(riga, objP);
                    righeInserite++;
                }

                ts.Complete();
                return righeInserite;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Calcola il valore per-ha a partire dal totale e dalla superficie.
        /// Riferimento spec: DS08-BL — le colonne *_per_ha sono i valori normalizzati per ettaro.
        /// </summary>
        private static ExpandoObject BuildParams(WriteLookupSostH2OLotto dto)
        {
            var exp = new ExpandoObject();
            exp.TryAdd("@id", dto.Id);
            exp.TryAdd("@dataCalcolo", dto.DataCalcolo);
            exp.TryAdd("@cuaaFiliera", dto.CuaaFiliera);
            exp.TryAdd("@cuaaAzienda", dto.CuaaAzienda);
            exp.TryAdd("@anno", dto.Anno);
            exp.TryAdd("@appezzamento", dto.Appezzamento);
            exp.TryAdd("@culCod", dto.CulCod);
            exp.TryAdd("@vegCod", (object?)dto.VegCod ?? DBNull.Value);
            exp.TryAdd("@esercizio", dto.Esercizio);
            exp.TryAdd("@lottoRaccolta", (object?)dto.LottoRaccolta ?? DBNull.Value);
            exp.TryAdd("@nazione", dto.Nazione);
            exp.TryAdd("@regione", (object?)dto.Regione ?? DBNull.Value);
            exp.TryAdd("@quantitaRaccoltaKg", dto.QuantitaRaccoltaKg);
            exp.TryAdd("@superficieRiferimento", dto.SuperficieRiferimento);
            exp.TryAdd("@payloadJson", (object?)dto.PayloadJson ?? DBNull.Value);
            exp.TryAdd("@jsonFirmato", (object?)dto.JsonFirmato ?? DBNull.Value);
            exp.TryAdd("@usernameCreazione", (object?)dto.Username_Creazione ?? DBNull.Value);
            exp.TryAdd("@usernameModifica", (object?)dto.Username_Modifica ?? DBNull.Value);
            return exp;
        }
    }
}