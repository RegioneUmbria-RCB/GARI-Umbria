using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using OutData.Zoo;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.CostantiPersonalizzate;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesatureCurveAccrescimento
{
    /// <summary>
    /// Implementazione DAL per il recupero delle pesature a supporto della curva di accrescimento.
    /// Vedere DS06-API: Endpoint Query Metriche Curva Accrescimento, sezione "Dipendenze Business Logic".
    /// </summary>
    public class PesatureCurveAccrescimentoDAL : BaseDALOperazioniZoo, IPesatureCurveAccrescimentoDAL
    {
        // Lav_Cod dei movimenti che rappresentano un carico/spostamento in stalla
        private static readonly List<int> LavCodsCaricoSpostamenti = new List<int>
        {
            LAV_COD.LAVCOD_NASCITA_ANIMALI,
            LAV_COD.LAVCOD_INCREMENTO_CONSISTENZE_ZOO,
            LAV_COD.LAVCOD_ACQUISTO_ANIMALI,
            LAV_COD.LAVCOD_SPOSTAMENTI_ZOO
        };

        public PesatureCurveAccrescimentoDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<(IReadOnlyList<OutData.Zoo.PesaturaCurvaAccrescimentoRow> Rows, int TotalCount)>
            LeggiPesatureCurveAccrescimentoAsync(
                PesatureCurveAccrescimentoQueryParams queryParams,
                string pivaJwt,
                string usernameJwt,
                bool visibilitaTotale,
                AgronicaCoreParametriServer objParametriServer)
        {
            var stb = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            var sqlParamsIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            // ── Parametri scalari comuni ──
            sqlParams.Add("@piva", pivaJwt);
            sqlParams.Add("@date_to", queryParams.DateTo.Date.AddDays(1).AddSeconds(-1)); // fine giornata
            sqlParams.Add("@date_from", queryParams.DateFrom.Date);
            sqlParams.Add("@kg_per_day", queryParams.KgPerDay);
            sqlParams.Add("@cau_pesatura", CAU_MOV.CAU_PESATURA_ANIMALI);
            sqlParams.Add("@cau_carico", CAU_MOV.CAU_CARICO_CAPO);
            sqlParams.Add("@zoo_consistenza", ELEM_COD.ZOO_CONSISTENZA);
            sqlParams.Add("@tipo_dest_stalla", TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA);

            if (!visibilitaTotale)
                sqlParams.Add("@username", usernameJwt);

            sqlParamsIn.Add("@lav_cods_carico", FormatClauseIn(LavCodsCaricoSpostamenti));

            // ── Filtri opzionali stalla e razza: costruiti dinamicamente ──
            var stallaWhereFragments = new List<string>();
            var razzaWhereFragments = new List<string>();

            ParseStallaKeys(queryParams.StallaPKeys, pivaJwt, sqlParams, stallaWhereFragments);
            ParseRazzaKeys(queryParams.RazzaKeys, sqlParams, razzaWhereFragments);

            // ── Filtro visibilità utente (sicurezza) ──
            string visibilitaJoin = visibilitaTotale
                ? string.Empty
                : "JOIN utenti_Visibilita_Appoggio uva (NOLOCK) ON uva.piva = @piva AND uva.Sa_Cod = us.sa_cod AND uva.Entita_Cod = 2 AND uva.Username = @username";

            // ── Paginazione ──
            int offset = (queryParams.Page - 1) * queryParams.Limit;
            sqlParams.Add("@offset", offset);
            sqlParams.Add("@limit", queryParams.Limit);

            string orderByColumn = queryParams.SortBy.ToLowerInvariant() switch
            {
                "data_pesata"     => "m.Data_Movimento",
                "scostamento_pct" => "scostamento_pct",
                _                 => "za.Matricola"
            };
            string orderByDir = queryParams.SortOrder.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";

            // ── Temp table: ultima posizione in stalla per ogni animale (al momento di @date_to) ──
            stb.AppendLine("CREATE TABLE #ultima_stalla");
            stb.AppendLine("(");
            stb.AppendLine("    Cod_Progetto   INT           NOT NULL,");
            stb.AppendLine("    sa_cod         INT           NOT NULL,");
            stb.AppendLine("    STA_NUM        INT           NOT NULL,");
            stb.AppendLine("    STA_DES        NVARCHAR(500) COLLATE DATABASE_DEFAULT NULL,");
            stb.AppendLine("    stalla_key     NVARCHAR(100) COLLATE DATABASE_DEFAULT NULL,");
            stb.AppendLine("    Data_Posizione DATETIME      NOT NULL");
            stb.AppendLine(");");

            stb.AppendLine();
            stb.AppendLine("WITH ranked_pos AS (");
            stb.AppendLine("    SELECT");
            stb.AppendLine("        md.Cod_Progetto,");
            stb.AppendLine("        mdes.sa_cod,");
            stb.AppendLine("        sta.STA_NUM,");
            stb.AppendLine("        sta.STA_DES,");
            stb.AppendLine("        @piva + '_' + CAST(mdes.sa_cod AS NVARCHAR(10)) + '_' + CAST(sta.STA_NUM AS NVARCHAR(10)) AS stalla_key,");
            stb.AppendLine("        m.Data_Movimento AS Data_Posizione,");
            stb.AppendLine("        ROW_NUMBER() OVER (PARTITION BY md.Cod_Progetto ORDER BY m.Data_Movimento DESC) AS rn");
            stb.AppendLine("    FROM Agenda ag (NOLOCK)");
            stb.AppendLine("    JOIN Movimenti m (NOLOCK)       ON m.PIVA = ag.PIVA AND m.Id_Agenda = ag.Id_Agenda");
            stb.AppendLine("    JOIN Movimenti_dettagli md (NOLOCK)");
            stb.AppendLine("        ON md.Id_Agenda = m.Id_Agenda AND md.Id_Mov = m.Id_Mov");
            stb.AppendLine("    JOIN Mov_Destinazioni mdes (NOLOCK)");
            stb.AppendLine("        ON mdes.Id_Agenda = md.Id_Agenda AND mdes.Id_Mov = md.Id_Mov AND mdes.Id_Mov_Det = md.Id_Mov_Det");
            stb.AppendLine("    JOIN Stalla_Raggruppamenti stra (NOLOCK)");
            stb.AppendLine("        ON stra.Piva = mdes.Piva AND stra.sa_cod = mdes.sa_cod AND stra.Raggruppamento_Cod = mdes.Id_Destinazione");
            stb.AppendLine("    JOIN Stalla sta (NOLOCK)");
            stb.AppendLine("        ON sta.PIVA = stra.PIVA AND sta.sa_cod = stra.sa_cod AND sta.STA_NUM = stra.STA_NUM");
            stb.AppendLine("    WHERE ag.Lav_Cod IN (@lav_cods_carico)");
            stb.AppendLine("      AND m.Cau_Mov        = @cau_carico");
            stb.AppendLine("      AND m.PIVA           = @piva");
            stb.AppendLine("      AND m.Data_Movimento <= @date_to");
            stb.AppendLine("      AND md.Elem_Cod      = @zoo_consistenza");
            stb.AppendLine("      AND md.Jolly_Int     = 0");
            stb.AppendLine("      AND mdes.Tipo_Destinazione = @tipo_dest_stalla");
            stb.AppendLine(")");
            stb.AppendLine("INSERT INTO #ultima_stalla (Cod_Progetto, sa_cod, STA_NUM, STA_DES, stalla_key, Data_Posizione)");
            stb.AppendLine("SELECT Cod_Progetto, sa_cod, STA_NUM, STA_DES, stalla_key, Data_Posizione");
            stb.AppendLine("FROM   ranked_pos");
            stb.AppendLine("WHERE  rn = 1;");

            stb.AppendLine();

            // ── SELECT principale: COUNT(*) OVER () fornisce il totale pre-paginazione ──
            stb.AppendLine("SELECT");
            stb.AppendLine("    COUNT(*) OVER ()                                                AS total_count,");
            stb.AppendLine("    za.Matricola                                                    AS lid,");
            stb.AppendLine("    m.Data_Movimento                                                AS data_pesata,");
            stb.AppendLine("    CAST(COALESCE(NULLIF(md.Qta, 0), md.Qta_Dettaglio1) AS FLOAT)           AS peso_kg,");
            stb.AppendLine("    DATEDIFF(day, za.Dat_Nascita, m.Data_Movimento)                 AS eta_giorni,");
            stb.AppendLine("    ROUND(");
            stb.AppendLine("        COALESCE(NULLIF(CAST(za.Peso AS FLOAT), 0), CAST(COALESCE(NULLIF(md.Qta, 0), md.Qta_Dettaglio1) AS FLOAT))");
            stb.AppendLine("        + DATEDIFF(day, za.Dat_Nascita, m.Data_Movimento) * @kg_per_day,");
            stb.AppendLine("        2)                                                          AS peso_teorico,");
            stb.AppendLine("    ROUND(");
            stb.AppendLine("        ((CAST(COALESCE(NULLIF(md.Qta, 0), md.Qta_Dettaglio1) AS FLOAT)");
            stb.AppendLine("            - ROUND(COALESCE(NULLIF(CAST(za.Peso AS FLOAT), 0), CAST(COALESCE(NULLIF(md.Qta, 0), md.Qta_Dettaglio1) AS FLOAT))");
            stb.AppendLine("                    + DATEDIFF(day, za.Dat_Nascita, m.Data_Movimento) * @kg_per_day, 2))");
            stb.AppendLine("         / NULLIF(ROUND(COALESCE(NULLIF(CAST(za.Peso AS FLOAT), 0), CAST(COALESCE(NULLIF(md.Qta, 0), md.Qta_Dettaglio1) AS FLOAT))");
            stb.AppendLine("                         + DATEDIFF(day, za.Dat_Nascita, m.Data_Movimento) * @kg_per_day, 2), 0))");
            stb.AppendLine("        * 100.0, 2)                                                 AS scostamento_pct,");
            stb.AppendLine("    CAST(za.GEN_COD AS NVARCHAR(10)) + '_'");
            stb.AppendLine("        + CAST(za.SPE_COD AS NVARCHAR(10)) + '_'");
            stb.AppendLine("        + CAST(za.RAZ_COD AS NVARCHAR(10))                         AS razza_key,");
            stb.AppendLine("    lra.RAZ_DES                                                     AS razza_des,");
            stb.AppendLine("    us.stalla_key,");
            stb.AppendLine("    COALESCE(us.STA_DES, '')                                        AS sta_des");
            stb.AppendLine("FROM  Movimenti m (NOLOCK)");
            stb.AppendLine("JOIN  Movimenti_dettagli md (NOLOCK)");
            stb.AppendLine("          ON md.Id_Agenda = m.Id_Agenda AND md.Id_Mov = m.Id_Mov");
            stb.AppendLine("JOIN  Zoo_Animali za (NOLOCK)");
            stb.AppendLine("          ON za.Cod_Progetto = md.Cod_Progetto AND za.Piva = @piva AND za.Sa_Cod = 0");
            stb.AppendLine("JOIN  #ultima_stalla us ON us.Cod_Progetto = za.Cod_Progetto");
            if (!string.IsNullOrEmpty(visibilitaJoin))
                stb.AppendLine(visibilitaJoin);
            stb.AppendLine("JOIN  Lista_Razze_Animali lra (NOLOCK)");
            stb.AppendLine("          ON lra.GEN_COD = za.GEN_COD AND lra.SPE_COD = za.SPE_COD AND lra.RAZ_COD = za.RAZ_COD");
            stb.AppendLine("WHERE m.Cau_Mov        = @cau_pesatura");
            stb.AppendLine("  AND m.PIVA           = @piva");
            stb.AppendLine("  AND m.Data_Movimento >= @date_from");
            stb.AppendLine("  AND m.Data_Movimento <= @date_to");

            foreach (var fragment in stallaWhereFragments)
                stb.AppendLine($"  AND {fragment}");
            foreach (var fragment in razzaWhereFragments)
                stb.AppendLine($"  AND {fragment}");

            stb.AppendLine($"ORDER BY {orderByColumn} {orderByDir}");
            stb.AppendLine("OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;");

            stb.AppendLine();
            stb.AppendLine("DROP TABLE #ultima_stalla;");

            try
            {
                DataTable dt = await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stb.ToString(), sqlParams, sqlParamsIn);

                int totalCount = 0;
                var rows = new List<OutData.Zoo.PesaturaCurvaAccrescimentoRow>(dt.Rows.Count);
                double alertThreshold = queryParams.AlertThresholdPct;

                foreach (DataRow dr in dt.Rows)
                {
                    if (totalCount == 0)
                        totalCount = Convert.ToInt32(dr["total_count"]);

                    double pesoKg = Convert.ToDouble(dr["peso_kg"]);
                    double pesoTeorico = Convert.ToDouble(dr["peso_teorico"]);
                    double scostamentoPct = pesoTeorico == 0d ? 0d : Convert.ToDouble(dr["scostamento_pct"]);

                    rows.Add(new OutData.Zoo.PesaturaCurvaAccrescimentoRow
                    {
                        Lid            = dr["lid"]?.ToString() ?? string.Empty,
                        DataPesata     = Convert.ToDateTime(dr["data_pesata"]),
                        PesoKg         = pesoKg,
                        EtaGiorni      = Convert.ToInt32(dr["eta_giorni"]),
                        PesoTeorico    = pesoTeorico,
                        ScostamentoPct = scostamentoPct,
                        AlertFlag      = Math.Abs(scostamentoPct) > alertThreshold,
                        RazzaKey       = dr["razza_key"]?.ToString()  ?? string.Empty,
                        RazzaDes       = dr["razza_des"]?.ToString()  ?? string.Empty,
                        StallaKey      = dr["stalla_key"]?.ToString() ?? string.Empty,
                        StaDes         = dr["sta_des"]?.ToString()    ?? string.Empty,
                    });
                }

                return (rows, totalCount);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Decompone i valori di <paramref name="stallaPKeys"/> (es. "01234567890_1_5,01234567890_1_6")
        /// in frammenti WHERE per sa_cod / STA_NUM.
        /// Il segmento piva è già validato dal chiamante (controller) contro il JWT.
        /// Vedere DS06-API, sezione "Sicurezza / stalla_key".
        /// </summary>
        private static void ParseStallaKeys(
            string? stallaPKeys,
            string pivaJwt,
            Dictionary<string, object> sqlParams,
            List<string> whereFragments)
        {
            if (string.IsNullOrWhiteSpace(stallaPKeys))
                return;

            var tuples = new List<(int SaCod, int StaNum)>();
            int idx = 0;
            foreach (var key in stallaPKeys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var segments = key.Split('_');
                // Formato: {piva}_{sa_cod}_{sta_num} — piva può contenere caratteri, sa_cod e sta_num sono interi.
                // I segmenti trailing sono sa_cod e sta_num; tutto il resto è il piva.
                if (segments.Length < 3) continue;
                if (!int.TryParse(segments[^1], out int staNum)) continue;
                if (!int.TryParse(segments[^2], out int saCod)) continue;

                string pivaSegment = string.Join("_", segments[..^2]);
                if (!pivaSegment.Equals(pivaJwt, StringComparison.OrdinalIgnoreCase)) continue;

                tuples.Add((saCod, staNum));
                sqlParams[$"@stalla_sa_{idx}"] = saCod;
                sqlParams[$"@stalla_sta_{idx}"] = staNum;
                idx++;
            }

            if (tuples.Count == 0)
                return;

            var conditions = tuples.Select((_, i) => $"(us.sa_cod = @stalla_sa_{i} AND us.STA_NUM = @stalla_sta_{i})");
            whereFragments.Add($"({string.Join(" OR ", conditions)})");
        }

        /// <summary>
        /// Decompone i valori di <paramref name="razzaKeys"/> (es. "1_1_3,1_1_5")
        /// in frammenti WHERE per GEN_COD / SPE_COD / RAZ_COD.
        /// Vedere DS06-API, sezione "Sicurezza / razza_key".
        /// </summary>
        private static void ParseRazzaKeys(
            string? razzaKeys,
            Dictionary<string, object> sqlParams,
            List<string> whereFragments)
        {
            if (string.IsNullOrWhiteSpace(razzaKeys))
                return;

            int idx = 0;
            var conditions = new List<string>();
            foreach (var key in razzaKeys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var parts = key.Split('_');
                if (parts.Length != 3) continue;
                if (!int.TryParse(parts[0], out int genCod)) continue;
                if (!int.TryParse(parts[1], out int speCod)) continue;
                if (!int.TryParse(parts[2], out int razCod)) continue;

                sqlParams[$"@razza_gen_{idx}"] = genCod;
                sqlParams[$"@razza_spe_{idx}"] = speCod;
                sqlParams[$"@razza_raz_{idx}"] = razCod;
                conditions.Add($"(za.GEN_COD = @razza_gen_{idx} AND za.SPE_COD = @razza_spe_{idx} AND za.RAZ_COD = @razza_raz_{idx})");
                idx++;
            }

            if (conditions.Count > 0)
                whereFragments.Add($"({string.Join(" OR ", conditions)})");
        }
    }
}
