using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS08-BL §Persistenze / §Query base: Concrete implementation of <see cref="IAppezzamentoOrogelDAL"/>.
    /// Executes a paginated SELECT against <c>Impresa_Progetti INNER JOIN Reg_Impianti</c>
    /// with four lookup LEFT JOINs (<c>Regolamenti</c>, <c>SpecieVegetali</c>,
    /// <c>Cultivar</c>, <c>GruppoFinalita</c>).
    /// Each correlated aggregate (sup_abbattuta, perc_piante_morte, impianto_attivo, rilievo, danni, tecnico)
    /// is evaluated once per row via OUTER APPLY, following the _NEW pattern from legacy Statistiche.vb.
    /// This eliminates the repeated inline scalar subquery execution that caused performance degradation.
    /// </summary>
    public class AppezzamentoOrogelDAL : BaseDALAnagrafe, IAppezzamentoOrogelDAL
    {
        /// <summary>
        /// DS08-BL: Initializes <see cref="AppezzamentoOrogelDAL"/>.
        /// </summary>
        public AppezzamentoOrogelDAL(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiAppezzamentiPaginatiAsync(
            int pageSize,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();
            var stbQuery = new StringBuilder();

            // DS08-BL §Query base: SELECT with INNER JOIN + lookup LEFT JOINs (WITH NOLOCK)
            stbQuery.AppendLine("SELECT TOP(@pageSize)");

            // DS08-BL §Output: Primary keys (esercizio level 5)
            stbQuery.AppendLine("    ese.Piva AS codice_azienda,");
            stbQuery.AppendLine("    ese.Sa_Cod AS codice_centro,");
            stbQuery.AppendLine("    ese.Appezza AS codice_appezzamento,");
            stbQuery.AppendLine("    ese.Id_Reg AS codice_impianto,");
            stbQuery.AppendLine("    ese.Progetto_Cod AS codice_esercizio,");

            // DS08-BL §Output: Esercizio fields
            stbQuery.AppendLine("    ese.Validita_Inizio AS data_inizio_esercizio,");
            stbQuery.AppendLine("    ese.Validita_Fine AS data_fine_esercizio,");
            stbQuery.AppendLine("    ese.Regolamento_Cod AS vincolo_impianto_codice,");
            stbQuery.AppendLine("    ISNULL(reg.Reg_Des, '') AS vincolo_impianto_descrizione,");

            // DS08-BL §Calcolo stato esercizio: GIAS codes map to BI state '2', else '1'
            stbQuery.AppendLine(
                "    ese.Stato_Impianto AS stato_impianto_codice,"
            );
            stbQuery.AppendLine(
                "    CASE WHEN ese.Stato_Impianto = 102 THEN 'In Produzione' ELSE COALESCE(fasi.Fase_Des, '') END AS stato_impianto_descrizione,"
            );

            // DS08-BL §Calcolo produzione totale: native null propagation if either operand is null
            stbQuery.AppendLine("    ese.produzione_prevista AS produzione_prevista_kg_ha,");
            stbQuery.AppendLine(
                "    ese.produzione_prevista * imp.sup_imp AS produzione_prevista_kg_tot,"
            );
            stbQuery.AppendLine("    ese.FlagSecondoRaccolto AS flagSecondoRaccolto,");
            stbQuery.AppendLine("    ese.data_fine_prevista AS data_raccolta_prevista,");

            // DS08-BL §Output: Impianto padre (level 4) — spec-aligned field names
            stbQuery.AppendLine("    cul.Veg_Cod AS specie_vegetale_codice,");
            stbQuery.AppendLine("    ISNULL(veg.Veg_Des, '') AS specie_vegetale_descrizione,");
            stbQuery.AppendLine("    imp.CUL_COD AS varieta_codice,");
            stbQuery.AppendLine("    ISNULL(cul.Cul_Des, '') AS varieta_descrizione,");
            stbQuery.AppendLine(
                "    ISNULL(CAST(veg.Gru_Cod AS VARCHAR(20)), '') AS codice_gruppo_vegetale,"
            );
            stbQuery.AppendLine("    imp.GrVa_Cod_Veg AS raggruppamento_varietale_codice,");
            stbQuery.AppendLine(
                "    ISNULL(gv.GRVA_DES, '') AS raggruppamento_varietale_descrizione,"
            );
            stbQuery.AppendLine(
                "    ISNULL(LEFT(wms.Cultivar_Coltiva, CHARINDEX('/', wms.Cultivar_Coltiva + '/') - 1), '') AS specie_wms_codice,"
            );
            stbQuery.AppendLine(
                "    ISNULL(SUBSTRING(wms.Cultivar_Coltiva, CHARINDEX('/', wms.Cultivar_Coltiva + '/') + 1, LEN(wms.Cultivar_Coltiva)), '') AS varieta_wms_codice,"
            );
            stbQuery.AppendLine("    imp.Grfi_cod AS finalita_produttiva_codice,");
            stbQuery.AppendLine(
                "    ISNULL(gf_imp.Grfi_des, '') AS finalita_produttiva_descrizione,"
            );
            stbQuery.AppendLine("    imp.foral_cod AS forma_allevamento_codice,");
            stbQuery.AppendLine("    imp.port_cod AS portinnesto_codice,");
            stbQuery.AppendLine("    imp.Validita_Inizio AS anno_impianto,");
            stbQuery.AppendLine("    imp.Validita_Fine AS data_abbattimento,");
            stbQuery.AppendLine("    imp.sup_imp AS superficie_impianto_ha,");
            stbQuery.AppendLine("    ese.P_HA AS numero_piante_ha,");
            stbQuery.AppendLine(
                "    CASE WHEN imp.Imp_Cod > 0 THEN 1 ELSE 0 END AS flag_irrigazione,"
            );
            stbQuery.AppendLine("    CASE WHEN imp.Cop_Cod > 0 THEN 1 ELSE 0 END AS flag_serra,");

            // DS08-BL §Output: Dati di Relazione e Gestionali (spec pag. 9-10)
            // Each computed aggregate is read from a named OUTER APPLY alias (evaluated once per row).
            // See SqlOuterApply* methods below for the actual SQL fragments appended to the FROM clause.
            stbQuery.AppendLine("    ISNULL(app.APP_NOME, '') AS numero_appezzamento,");

            stbQuery.AppendLine(
                "    ISNULL(cte_tecnico.tecnico_nome, '') AS tecnico_responsabile,"
            );
            stbQuery.AppendLine("    cte_tecnico.tecnico_cf AS tecnico_cf,");

            stbQuery.AppendLine(
                "    CASE WHEN imp.flagContributo = 'S' THEN 1 ELSE 0 END AS flag_fruizione_contributi,"
            );
            stbQuery.AppendLine("    CAST(0 AS DECIMAL(18,4)) AS kg_conferiti,");

            // DS08-BL §Sup_Abbattuta: read from cte_sup_abb (computed once via OUTER APPLY, lav_cod=170 cau_mov='2300')
            stbQuery.AppendLine("    cte_sup_abb.sup_abbattuta AS superficieAbbattuta,");

            // DS08-BL §Perc_Piante_Morte: read from cte_perc_morte (computed once via OUTER APPLY, lav_cod=108 cau_mov='2200')
            stbQuery.AppendLine("    cte_perc_morte.perc_piante_morte AS danni_rilevati_numero,");

            // DS08-BL §DataUltimoRilievo / §ResaUltimoRilievo: both from cte_rilievo (single OUTER APPLY, lav_cod=169)
            stbQuery.AppendLine(
                "    cte_rilievo.data_ultimo_rilievo_produzione AS data_ultimo_rilievo_produzione_prevista,"
            );
            stbQuery.AppendLine(
                "    cte_rilievo.resa_ultimo_rilievo AS resa_ultimo_rilievo_produzione_prevista_kg_ha,"
            );

            // DS08-BL §Stime_Produzione CASE formula — reads OUTER APPLY aliases (each computed once per row).
            // Case 3A: partial knockdown + moria  → ((sup_eff) - (sup_eff × %moria/100)) × resa
            // Case 3B: moria only               → (sup_imp - sup_imp × %moria/100) × resa
            // Case 3C: partial knockdown only    → (sup_imp - supAbb) × resa
            // Case 3D: (unreachable via inner ELSE) — outer gate ensures at least one damage type present
            // Outer gate: sup_abbattuta < sup_imp (partial/no knockdown) OR perc_morte > 0 (moria present)
            stbQuery.AppendLine("    CASE WHEN ISNULL(cte_sup_abb.sup_abbattuta, 0) < imp.sup_imp");
            stbQuery.AppendLine("              OR ISNULL(cte_perc_morte.perc_piante_morte, 0) > 0");
            stbQuery.AppendLine("         THEN CASE WHEN imp.Data_Inizio_Produzione IS NOT NULL");
            stbQuery.AppendLine(
                "                       AND YEAR(imp.Data_Inizio_Produzione) <= YEAR(GETDATE())"
            );
            stbQuery.AppendLine(
                "                       AND YEAR(imp.Data_Inizio_Produzione) > 1900"
            );
            stbQuery.AppendLine("                  THEN CASE");
            stbQuery.AppendLine(
                "                           WHEN cte_sup_abb.sup_abbattuta < imp.sup_imp AND cte_perc_morte.perc_piante_morte > 0"
            );
            stbQuery.AppendLine(
                "                               THEN ((imp.sup_imp - cte_sup_abb.sup_abbattuta)"
            );
            stbQuery.AppendLine(
                "                                    - ((imp.sup_imp - cte_sup_abb.sup_abbattuta) * cte_perc_morte.perc_piante_morte / 100))"
            );
            stbQuery.AppendLine(
                "                                    * ISNULL(ese.produzione_prevista, 0)"
            );
            stbQuery.AppendLine(
                "                           WHEN cte_perc_morte.perc_piante_morte > 0"
            );
            stbQuery.AppendLine(
                "                               THEN (imp.sup_imp - imp.sup_imp * cte_perc_morte.perc_piante_morte / 100)"
            );
            stbQuery.AppendLine(
                "                                    * ISNULL(ese.produzione_prevista, 0)"
            );
            stbQuery.AppendLine(
                "                           WHEN cte_sup_abb.sup_abbattuta < imp.sup_imp"
            );
            stbQuery.AppendLine(
                "                               THEN (imp.sup_imp - cte_sup_abb.sup_abbattuta)"
            );
            stbQuery.AppendLine(
                "                                    * ISNULL(ese.produzione_prevista, 0)"
            );
            stbQuery.AppendLine(
                "                           ELSE ISNULL(ese.produzione_prevista, 0) * ISNULL(imp.sup_imp, 0)"
            );
            stbQuery.AppendLine("                       END");
            stbQuery.AppendLine("                  ELSE 0");
            stbQuery.AppendLine("             END");
            stbQuery.AppendLine("         ELSE 0");
            stbQuery.AppendLine("    END AS stima_produzione_kg_tot,");

            // DS08-BL §DanniDescrizione: read from cte_danni (computed once via OUTER APPLY)
            stbQuery.AppendLine(
                "    ISNULL(cte_danni.danni_descrizione, '') AS danni_rilevati_descrizione,"
            );

            // data_rilievo_danni and resa_kg_tot reuse cte_rilievo — no re-execution
            stbQuery.AppendLine("    cte_perc_morte.data_ultimo_rilievo_danni AS data_rilievo_danni,");
            stbQuery.AppendLine(
                "    cte_rilievo.resa_ultimo_rilievo * (imp.sup_imp - ISNULL(cte_sup_abb.sup_abbattuta, 0)) AS resa_ultimo_rilievo_produzione_prevista_kg_tot"
            );

            stbQuery.AppendLine("FROM Imprese_Progetti ese");

            // DS08-BL §Persistenze: INNER JOIN Reg_Impianti on all 4 key columns
            stbQuery.AppendLine("INNER JOIN Reg_Impianti imp");
            stbQuery.AppendLine("    ON ese.Piva = imp.PIVA");
            stbQuery.AppendLine("    AND ese.Sa_Cod = imp.SA_COD");
            stbQuery.AppendLine("    AND ese.Appezza = imp.APPEZZA");
            stbQuery.AppendLine("    AND ese.Id_Reg = imp.ID_REG");

            // DS08-BL §Persistenze: Lookup LEFT JOINs (null-safe via ISNULL in SELECT)
            stbQuery.AppendLine("LEFT JOIN Regolamenti reg");
            stbQuery.AppendLine("    ON ese.Regolamento_Cod = reg.Reg_Cod");
            stbQuery.AppendLine("LEFT JOIN Cultivar cul");
            stbQuery.AppendLine("    ON imp.CUL_COD = cul.Cul_Cod");
            stbQuery.AppendLine("LEFT JOIN SpecieVegetali veg");
            stbQuery.AppendLine("    ON cul.Veg_Cod = veg.Veg_Cod");
            stbQuery.AppendLine("LEFT JOIN GruppoFinalita gf_imp");
            stbQuery.AppendLine("    ON imp.Grfi_cod = gf_imp.Grfi_Cod");
            stbQuery.AppendLine("LEFT JOIN GruppoVarietale gv");
            stbQuery.AppendLine("    ON ABS(imp.GrVa_Cod_Veg) = gv.GRVA_COD");
            // DS08-BL §WMS: destination code per centro (FRESCO=12, SURGELATO=13) drives Tipo_Codifica in CAC_Codifica_Cultivar
            stbQuery.AppendLine("LEFT JOIN Centri_Aziendali_Codici destCod");
            stbQuery.AppendLine("    ON destCod.piva = ese.Piva");
            stbQuery.AppendLine("    AND destCod.sa_cod = ese.Sa_Cod");
            stbQuery.AppendLine(
                $"    AND destCod.id_cod = {(int)Enum_CodiciAnagrafe.Codice_Destinazione}"
            );
            // DS08-BL §WMS: OUTER APPLY TOP 1 — only active when destCod.val_cod IN ('FRESCO', 'SURGELATO').
            // Joins on Cultivar_Gias, Veg_Cod_GIAS, Grfi_Cod and Grva_Cod are applied only when the
            // corresponding imp/cul value != 0 (matching legacy VB "If X <> 0 Then" conditional filter pattern).
            // cul.Veg_Cod is taken directly from the outer LEFT JOIN SpecieVegetali.
            stbQuery.AppendLine("OUTER APPLY (");
            stbQuery.AppendLine("    SELECT TOP 1 cac.Cultivar_Coltiva");
            stbQuery.AppendLine("    FROM CAC_Codifica_Cultivar cac");
            stbQuery.AppendLine(
                "    WHERE UPPER(ISNULL(destCod.val_cod, '')) IN ('FRESCO', 'SURGELATO')"
            );
            stbQuery.AppendLine("      AND cac.Cultivar_Gias = imp.CUL_COD");
            stbQuery.AppendLine("      AND cac.Veg_Cod_GIAS = cul.Veg_Cod");
            stbQuery.AppendLine("      AND (imp.Grfi_cod = 0 OR cac.Grfi_Cod = imp.Grfi_cod)");
            stbQuery.AppendLine(
                "      AND (ABS(ISNULL(imp.GrVa_Cod_Veg, 0)) = 0 OR cac.Grva_Cod = ABS(imp.GrVa_Cod_Veg))"
            );
            stbQuery.AppendLine(
                $"      AND cac.Tipo_Codifica = CASE WHEN UPPER(destCod.val_cod) = 'FRESCO' THEN {(int)Enum_Tipo_CAC_Codifica_Varieta.Fresco} ELSE {(int)Enum_Tipo_CAC_Codifica_Varieta.Surgelato} END"
            );
            stbQuery.AppendLine(") wms");

            stbQuery.AppendLine("LEFT JOIN Appezzamento app");
            stbQuery.AppendLine("    ON ese.Piva = app.PIVA");
            stbQuery.AppendLine("    AND ese.Sa_Cod = app.SA_COD");
            stbQuery.AppendLine("    AND ese.Appezza = app.APPEZZA");
            stbQuery.AppendLine("LEFT JOIN Regolamenti disc");
            stbQuery.AppendLine("    ON disc.Reg_Cod = ese.Disciplinare_Cod");
            stbQuery.AppendLine("LEFT JOIN FasiCicloColturale_Anagrafiche fasi");
            stbQuery.AppendLine("    ON ese.Stato_Impianto = fasi.Fase_Cod");

            // DS08-BL: OUTER APPLY aggregates — each evaluated once per row (legacy _NEW pattern).
            // cte_sup_abb: cumulative knocked-down surface (lav_cod=170, cau_mov='2300')
            stbQuery.AppendLine(SqlOuterApplySupAbbattuta());
            // cte_perc_morte: cumulative damage % (lav_cod=108, cau_mov='2200')
            stbQuery.AppendLine(SqlOuterApplyPercPianteMorte());
            // cte_rilievo: most-recent yield survey date + resa (lav_cod=169, ff_classe=34) — both columns from one OUTER APPLY
            stbQuery.AppendLine(SqlOuterApplyRilievo());
            // cte_danni: STRING_AGG of distinct DanniRaccolta descriptions (lav_cod=108, cau_mov='2200')
            stbQuery.AppendLine(SqlOuterApplyDanni());
            // cte_tecnico: technician name + CF (Reg_Impianti_Codici id_cod=1088) — both columns from one OUTER APPLY
            stbQuery.AppendLine(SqlOuterApplyTecnico());
            // DS08-BL §Filtri Obbligatori: Inviato >= 0
            stbQuery.AppendLine("WHERE ese.Inviato >= 0");

            parSql.Add("@pageSize", pageSize);

            // DS08-BL §Filtro Opzionale Azienda
            if (codiciAzienda.Count > 0)
            {
                stbQuery.AppendLine("    AND ese.Piva IN (@filtroPiva)");
                parSqlIn.Add("@filtroPiva", FormatClauseIn(codiciAzienda.ToList()));
            }

            // DS08-BL §Filtro paginazione singola chiave: hardcoded with ese. alias
            if (parSqlPaginazione.Count > 0)
            {
                stbQuery.AppendLine("    AND ese.Progetto_Cod > @LastProgettoCod");
                foreach (KeyValuePair<string, object> kv in parSqlPaginazione)
                    parSql[kv.Key] = kv.Value;
            }

            // DS08-BL §Ordine Statico Obbligatorio (5-column composite)
            stbQuery.AppendLine(
                "ORDER BY ese.Piva ASC, ese.Sa_Cod ASC, ese.Appezza ASC, ese.Id_Reg ASC, ese.Progetto_Cod ASC"
            );

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <summary>
        /// DS08-BL §Sup_Abbattuta: OUTER APPLY computing <c>ISNULL(SUM(ab_dest.qta2), 0) AS sup_abbattuta</c>
        /// for espianti/abbattimenti (<c>lav_cod=170</c>, <c>cau_mov='2300'</c>).
        /// Evaluated once per row; result alias <c>cte_sup_abb.sup_abbattuta</c> is referenced
        /// twice in the SELECT (as <c>superficieAbbattuta</c> and inside <c>stima_produzione_kg_tot</c>).
        /// </summary>
        private static string SqlOuterApplySupAbbattuta()
        {
            var sql = new StringBuilder();
            sql.AppendLine("OUTER APPLY (");
            sql.AppendLine("    SELECT ISNULL(SUM(ab_dest.qta2), 0) AS sup_abbattuta");
            sql.AppendLine("    FROM movimenti_dettagli ab_det");
            sql.AppendLine("    INNER JOIN movimenti ab_mov");
            sql.AppendLine("        INNER JOIN agenda ab_ag");
            sql.AppendLine("            ON ab_mov.piva = ab_ag.piva");
            sql.AppendLine("           AND ab_mov.sa_cod = ab_ag.sa_cod");
            sql.AppendLine("           AND ab_mov.id_agenda = ab_ag.id_agenda");
            sql.AppendLine("        ON ab_det.piva = ab_mov.piva");
            sql.AppendLine("       AND ab_det.sa_cod = ab_mov.sa_cod");
            sql.AppendLine("       AND ab_det.id_agenda = ab_mov.id_agenda");
            sql.AppendLine("       AND ab_det.id_mov = ab_mov.id_mov");
            sql.AppendLine("    INNER JOIN mov_destinazioni ab_dest");
            sql.AppendLine("        ON ab_det.piva = ab_dest.piva");
            sql.AppendLine("       AND ab_det.sa_cod = ab_dest.sa_cod");
            sql.AppendLine("       AND ab_det.id_agenda = ab_dest.id_agenda");
            sql.AppendLine("       AND ab_det.id_mov = ab_dest.id_mov");
            sql.AppendLine("       AND ab_det.id_mov_det = ab_dest.id_mov_det");
            sql.AppendLine(
                $"    WHERE ab_ag.lav_cod = {(int)LAV_COD.LAVCOD_ABBATTIMENTO_IMPIANTI}"
            );
            sql.AppendLine($"      AND ab_mov.cau_mov = '{CAU_MOV.CAU_LAVORAZIONE}'");
            sql.AppendLine("      AND ab_dest.piva = imp.piva");
            sql.AppendLine("      AND ab_dest.sa_cod = imp.sa_cod");
            sql.AppendLine("      AND ab_dest.appezza = imp.appezza");
            sql.AppendLine("      AND ab_dest.id_destinazione = imp.id_reg");
            sql.AppendLine("      AND ab_mov.data_movimento >= ese.validita_inizio");
            sql.AppendLine("      AND ab_mov.data_movimento <= CAST(GETDATE() AS DATE)");
            sql.Append(") cte_sup_abb");
            return sql.ToString();
        }

        /// <summary>
        /// DS08-BL §Perc_Piante_Morte: OUTER APPLY computing <c>SUM(mo_dest.qta) AS perc_piante_morte</c>
        /// for technical damage surveys (<c>lav_cod=108</c>, <c>cau_mov='2200'</c>).
        /// Returns NULL (not 0) for rows with no damage record — matches legacy null output.
        /// Evaluated once per row; result alias <c>cte_perc_morte.perc_piante_morte</c> is referenced
        /// twice in the SELECT (as <c>danni_rilevati_numero</c> and inside <c>stima_produzione_kg_tot</c>).
        /// </summary>
        private static string SqlOuterApplyPercPianteMorte()
        {
            var sql = new StringBuilder();
            sql.AppendLine("OUTER APPLY (");
            sql.AppendLine("    SELECT MAX(mo_mov.Data_Movimento) AS data_ultimo_rilievo_danni, SUM(mo_dest.qta) AS perc_piante_morte");
            sql.AppendLine("    FROM movimenti_dettagli mo_det");
            sql.AppendLine("    INNER JOIN movimenti mo_mov");
            sql.AppendLine("        INNER JOIN agenda mo_ag");
            sql.AppendLine("            ON mo_mov.piva = mo_ag.piva");
            sql.AppendLine("           AND mo_mov.sa_cod = mo_ag.sa_cod");
            sql.AppendLine("           AND mo_mov.id_agenda = mo_ag.id_agenda");
            sql.AppendLine("        ON mo_det.piva = mo_mov.piva");
            sql.AppendLine("       AND mo_det.sa_cod = mo_mov.sa_cod");
            sql.AppendLine("       AND mo_det.id_agenda = mo_mov.id_agenda");
            sql.AppendLine("       AND mo_det.id_mov = mo_mov.id_mov");
            sql.AppendLine("    INNER JOIN mov_destinazioni mo_dest");
            sql.AppendLine("        ON mo_det.piva = mo_dest.piva");
            sql.AppendLine("       AND mo_det.sa_cod = mo_dest.sa_cod");
            sql.AppendLine("       AND mo_det.id_agenda = mo_dest.id_agenda");
            sql.AppendLine("       AND mo_det.id_mov = mo_dest.id_mov");
            sql.AppendLine("       AND mo_det.id_mov_det = mo_dest.id_mov_det");
            sql.AppendLine($"    WHERE mo_ag.lav_cod = {(int)LAV_COD.LAVCOD_DANNI_RACCOLTA}");
            sql.AppendLine($"      AND mo_mov.cau_mov = '{CAU_MOV.CAU_RILIEVO_RACCOLTA}'");
            sql.AppendLine("      AND mo_dest.piva = imp.piva");
            sql.AppendLine("      AND mo_dest.sa_cod = imp.sa_cod");
            sql.AppendLine("      AND mo_dest.appezza = imp.appezza");
            sql.AppendLine("      AND mo_dest.id_destinazione = imp.id_reg");
            sql.AppendLine("      AND mo_mov.data_movimento >= ese.validita_inizio");
            sql.AppendLine("      AND mo_mov.data_movimento <= CAST(GETDATE() AS DATE)");
            sql.Append(") cte_perc_morte");
            return sql.ToString();
        }

        /// <summary>
        /// DS08-BL §DataUltimoRilievo / §ResaUltimoRilievo: single OUTER APPLY returning both
        /// <c>data_ultimo_rilievo</c> (Data_Movimento) and <c>resa_ultimo_rilievo</c> (qta × 100)
        /// from the most-recent yield survey (<c>lav_cod=169</c>, <c>cau_mov='2200'</c>,
        /// <c>ff_classe=34</c>), ordered by <c>ri_dest.validita_inizio DESC</c>.
        /// Combining both into one OUTER APPLY halves the number of executions vs. two separate subqueries.
        /// Evaluated once per row; <c>cte_rilievo.data_ultimo_rilievo</c> is used twice in SELECT
        /// (<c>data_ultimo_rilievo_produzione_prevista</c> and <c>data_rilievo_danni</c>);
        /// <c>cte_rilievo.resa_ultimo_rilievo</c> is used twice (<c>resa_kg_ha</c> and <c>resa_kg_tot</c>).
        /// </summary>
        private static string SqlOuterApplyRilievo()
        {
            var sql = new StringBuilder();
            sql.AppendLine("OUTER APPLY (");
            sql.AppendLine("    SELECT TOP 1");
            sql.AppendLine("        ri_mov.Data_Movimento AS data_ultimo_rilievo_produzione,");
            sql.AppendLine("        ri_dest.qta * 100 AS resa_ultimo_rilievo");
            sql.AppendLine("    FROM movimenti_dettagli ri_det");
            sql.AppendLine("    INNER JOIN movimenti ri_mov");
            sql.AppendLine("        INNER JOIN agenda ri_ag");
            sql.AppendLine("            ON ri_mov.piva = ri_ag.piva");
            sql.AppendLine("           AND ri_mov.sa_cod = ri_ag.sa_cod");
            sql.AppendLine("           AND ri_mov.id_agenda = ri_ag.id_agenda");
            sql.AppendLine("        ON ri_det.piva = ri_mov.piva");
            sql.AppendLine("       AND ri_det.sa_cod = ri_mov.sa_cod");
            sql.AppendLine("       AND ri_det.id_agenda = ri_mov.id_agenda");
            sql.AppendLine("       AND ri_det.id_mov = ri_mov.id_mov");
            sql.AppendLine("    INNER JOIN mov_destinazioni ri_dest");
            sql.AppendLine("        ON ri_det.piva = ri_dest.piva");
            sql.AppendLine("       AND ri_det.sa_cod = ri_dest.sa_cod");
            sql.AppendLine("       AND ri_det.id_agenda = ri_dest.id_agenda");
            sql.AppendLine("       AND ri_det.id_mov = ri_dest.id_mov");
            sql.AppendLine("       AND ri_det.id_mov_det = ri_dest.id_mov_det");
            sql.AppendLine("    INNER JOIN mov_dettaglio_tecnico ri_mdt");
            sql.AppendLine("        ON ri_det.piva = ri_mdt.piva");
            sql.AppendLine("       AND ri_det.sa_cod = ri_mdt.sa_cod");
            sql.AppendLine("       AND ri_det.id_agenda = ri_mdt.id_agenda");
            sql.AppendLine("       AND ri_det.id_mov = ri_mdt.id_mov");
            sql.AppendLine("       AND ri_det.id_mov_det = ri_mdt.id_mov_det");
            sql.AppendLine(
                $"    WHERE ri_ag.lav_cod = {(int)LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA}"
            );
            sql.AppendLine($"      AND ri_mov.cau_mov = '{CAU_MOV.CAU_RILIEVO_RACCOLTA}'");
            sql.AppendLine("      AND ri_mdt.ff_classe = 34");
            sql.AppendLine("      AND ri_dest.piva = imp.piva");
            sql.AppendLine("      AND ri_dest.sa_cod = imp.sa_cod");
            sql.AppendLine("      AND ri_dest.appezza = imp.appezza");
            sql.AppendLine("      AND ri_dest.id_destinazione = imp.id_reg");
            sql.AppendLine("      AND ri_mov.data_movimento >= ese.validita_inizio");
            sql.AppendLine("      AND ri_mov.data_movimento <= CAST(GETDATE() AS DATE)");
            sql.AppendLine("    ORDER BY ri_dest.validita_inizio DESC");
            sql.Append(") cte_rilievo");
            return sql.ToString();
        }

        /// <summary>
        /// DS08-BL §DanniDescrizione: OUTER APPLY returning
        /// <c>STRING_AGG(DISTINCT DR_DES, ', ') AS danni_descrizione</c>
        /// from DanniRaccolta joined through mov_dettaglio_tecnico, scoped to
        /// <c>lav_cod=108, cau_mov='2200'</c> and the exercise date range.
        /// Evaluated once per row; referenced as <c>danni_rilevati_descrizione</c> in SELECT.
        /// </summary>
        private static string SqlOuterApplyDanni()
        {
            var sql = new StringBuilder();
            sql.AppendLine("OUTER APPLY (");
            sql.AppendLine("    SELECT STRING_AGG(DR_DES, ', ') AS danni_descrizione");
            sql.AppendLine("    FROM (");
            sql.AppendLine("        SELECT DISTINCT dr.DR_DES");
            sql.AppendLine("        FROM movimenti_dettagli dd_det");
            sql.AppendLine("        INNER JOIN movimenti dd_mov");
            sql.AppendLine("            INNER JOIN agenda dd_ag");
            sql.AppendLine("                ON dd_mov.piva = dd_ag.piva");
            sql.AppendLine("               AND dd_mov.sa_cod = dd_ag.sa_cod");
            sql.AppendLine("               AND dd_mov.id_agenda = dd_ag.id_agenda");
            sql.AppendLine("            ON dd_det.piva = dd_mov.piva");
            sql.AppendLine("           AND dd_det.sa_cod = dd_mov.sa_cod");
            sql.AppendLine("           AND dd_det.id_agenda = dd_mov.id_agenda");
            sql.AppendLine("           AND dd_det.id_mov = dd_mov.id_mov");
            sql.AppendLine("        INNER JOIN mov_destinazioni dd_dest");
            sql.AppendLine("            ON dd_det.piva = dd_dest.piva");
            sql.AppendLine("           AND dd_det.sa_cod = dd_dest.sa_cod");
            sql.AppendLine("           AND dd_det.id_agenda = dd_dest.id_agenda");
            sql.AppendLine("           AND dd_det.id_mov = dd_dest.id_mov");
            sql.AppendLine("           AND dd_det.id_mov_det = dd_dest.id_mov_det");
            sql.AppendLine("        INNER JOIN mov_dettaglio_tecnico dd_mdt");
            sql.AppendLine("            ON dd_det.piva = dd_mdt.piva");
            sql.AppendLine("           AND dd_det.sa_cod = dd_mdt.sa_cod");
            sql.AppendLine("           AND dd_det.id_agenda = dd_mdt.id_agenda");
            sql.AppendLine("           AND dd_det.id_mov = dd_mdt.id_mov");
            sql.AppendLine("           AND dd_det.id_mov_det = dd_mdt.id_mov_det");
            sql.AppendLine("        INNER JOIN DanniRaccolta dr");
            sql.AppendLine("            ON dd_mdt.ff_classe = dr.DR_COD");
            sql.AppendLine($"        WHERE dd_ag.lav_cod = {(int)LAV_COD.LAVCOD_DANNI_RACCOLTA}");
            sql.AppendLine($"         AND dd_mov.cau_mov = '{CAU_MOV.CAU_RILIEVO_RACCOLTA}'");
            sql.AppendLine("          AND dd_dest.piva = imp.piva");
            sql.AppendLine("          AND dd_dest.sa_cod = imp.sa_cod");
            sql.AppendLine("          AND dd_dest.appezza = imp.appezza");
            sql.AppendLine("          AND dd_dest.id_destinazione = imp.id_reg");
            sql.AppendLine("          AND dd_mov.data_movimento >= ese.validita_inizio");
            sql.AppendLine("          AND dd_mov.data_movimento <= CAST(GETDATE() AS DATE)");
            sql.AppendLine("    ) distinct_danni");
            sql.Append(") cte_danni");
            return sql.ToString();
        }

        /// <summary>
        /// DS08-BL §Tecnico_Responsabile / §Tecnico_CF: single OUTER APPLY returning both
        /// <c>tecnico_nome</c> (Cognome + ' ' + Nome) and <c>tecnico_cf</c> (Cod_Contatto)
        /// for the technician linked via <c>Imprese_Codici.id_cod = 1088</c>.
        /// Combining both into one OUTER APPLY halves the number of executions vs. two separate subqueries.
        /// Evaluated once per row; referenced as <c>tecnico_responsabile</c> and <c>tecnico_cf</c> in SELECT.
        /// </summary>
        // DS08-BL §tecnico_cf: pipe-separated CF codes stored in Reg_Impianti_Codici id_cod=1088 (impianto level).
        // tecnico_nome is not stored separately; legacy always returned empty string for descrizioneTecnico.
        private static string SqlOuterApplyTecnico()
        {
            var sql = new StringBuilder();
            sql.AppendLine("OUTER APPLY (");
            sql.AppendLine("    SELECT");
            sql.AppendLine(
                "        STRING_AGG(Contatti.Cognome + ' ' + Contatti.Nome, '|') AS tecnico_nome,"
            );
            sql.AppendLine("        STRING_AGG(Contatti.Cod_Contatto, '|') AS tecnico_cf");
            sql.AppendLine("    FROM Reg_Impianti_Codici");
            sql.AppendLine("    CROSS APPLY dbo.fSplit(Reg_Impianti_Codici.val_cod, '|') split_cf");
            sql.AppendLine("    INNER JOIN Contatti ON split_cf.strName = Contatti.Cod_Contatto AND (Contatti.Piva = ese.piva OR Contatti.Sa_Cod = -1)");
            sql.AppendLine(
                $"    WHERE Reg_Impianti_Codici.id_cod = {(int)TipiEnumerativi.Enum_CodiciAnagrafe.Tecnico}"
            );
            sql.AppendLine("      AND Reg_Impianti_Codici.piva = ese.Piva");
            sql.AppendLine("      AND Reg_Impianti_Codici.sa_cod = ese.sa_cod");
            sql.AppendLine("      AND Reg_Impianti_Codici.appezza = ese.appezza");
            sql.AppendLine("      AND Reg_Impianti_Codici.id_reg = ese.id_reg");
            sql.AppendLine("      AND Reg_Impianti_Codici.progetto_cod = ese.progetto_cod");
            sql.Append(") cte_tecnico");
            return sql.ToString();
        }
    }
}
