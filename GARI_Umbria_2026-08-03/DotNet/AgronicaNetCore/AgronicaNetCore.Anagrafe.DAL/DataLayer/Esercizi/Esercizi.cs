using AgronicaCoreDTOStd.InData;
using AgronicaCoreModelsSTD.Widgets;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Esercizi
{
    public class Esercizi : BaseDALAnagrafe, IEsercizi
    {
        public Esercizi(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable?> LeggiAsync(
            AgronicaCoreParametriServer objParametriServer,
            string piva = "", int saCod = 0, int appezza = 0, int idReg = 0, int progCod = -1,
            DateTime? inizio = null, DateTime? fine = null
        )
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            try
            {
                stbQuery.AppendLine("SELECT * FROM Imprese_Progetti");
                stbQuery.AppendLine("WHERE 1 = 1");
                if (inizio != null)
                {
                    stbQuery.AppendLine(" AND Imprese_Progetti.Validita_Fine >= @inizio ");
                    sqlParams.TryAdd("@inizio", inizio);
                }
                if (fine != null)
                {
                    stbQuery.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= @fine ");
                    sqlParams.TryAdd("@fine", fine);
                }
                if (piva != "")
                {
                    stbQuery.AppendLine(" AND Imprese_Progetti.Piva = @piva ");
                    sqlParams.TryAdd("@piva", piva);
                }
                if (saCod != 0)
                {
                    stbQuery.AppendLine(" AND Imprese_Progetti.Sa_Cod = @saCod ");
                    sqlParams.TryAdd("@saCod", saCod);
                }
                if (appezza != 0)
                {
                    stbQuery.AppendLine(" AND Imprese_Progetti.Appezza = @appezza ");
                    sqlParams.TryAdd("@appezza", appezza);
                }
                if (idReg != 0)
                {
                    stbQuery.AppendLine(" AND Imprese_Progetti.Id_Reg = @idReg ");
                    sqlParams.TryAdd("@idReg", idReg);
                }
                if (progCod != -1)
                {
                    stbQuery.AppendLine(" AND Imprese_Progetti.Progetto_Cod = @progCod ");
                    sqlParams.TryAdd("@progCod", progCod);
                }
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }
        }

        /// <summary>
        /// Reads the active exercise for a plot at the specified date.
        /// </summary>
        /// <param name="piva">The VAT number (Partita IVA) of the company.</param>
        /// <param name="sacod">The company code (SA_COD).</param>
        /// <param name="appezza">The plot/field code (APPEZZA).</param>
        /// <param name="idReg">The register ID.</param>
        /// <param name="dataAttivita">The date of the activity to filter valid projects.</param>
        /// <param name="objParametriServer">Server parameters object containing configuration for the data provider.</param>
        /// <returns>A <see cref="DataTable"/> containing detailed information about the plant, including project codes,
        /// species, cultivars, coverage, and maximum nutrient values (N, P, K, Mg).</returns>
        public async Task<DataTable> LeggiEsercizioXIntegrazioneAttivita(string piva, int sacod, int appezza, int idReg, DateTime dataAttivita, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var parametriSql = new Dictionary<string, object>();
                var strSql = new StringBuilder() { Length = 0 };
                DropActivePrinciplesTempTables(strSql);
                await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString());

                strSql = new StringBuilder() { Length = 0 };
                CreateActivePrinciplesTempTables(strSql);

                //creazione query
                strSql.AppendLine(" SELECT DISTINCT");
                strSql.AppendLine("        Imprese_Progetti.Progetto_Cod, ");
                strSql.AppendLine("        ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod,");
                strSql.AppendLine("        ISNULL(Cultivar.Cul_Cod,0) AS Cul_Cod,");
                strSql.AppendLine("        ISNULL(Reg_Impianti.foral_cod,0) AS forma_allevamento,");
                strSql.AppendLine("        ISNULL(Reg_Impianti.Cop_Cod,0) AS copertura,");
                strSql.AppendLine("        Reg_Impianti.sup_imp,");
                strSql.AppendLine("        ISNULL(Reg_Impianti.grfi_cod,0) AS grfi_cod,");
                strSql.AppendLine("        Imprese_Progetti.Stato_Impianto,");
                strSql.AppendLine("        ISNULL(Imprese_Progetti.Regolamento_Cod,1) AS Regolamento_Cod,");
                strSql.AppendLine("        ISNULL(Imprese_Progetti.Regolamento_Concimazioni_Cod,0) AS Regolamento_Concimazioni_Cod,");
                strSql.AppendLine("        ISNULL(Imprese_Progetti.Disciplinare_Cod,0) AS Disciplinare_Cod,");
                strSql.AppendLine("        ISNULL(Imprese_Progetti.Disciplinare_PubblicoPrivato,0) AS Disciplinare_PubblicoPrivato,");
                strSql.AppendLine("        Imprese_Progetti.validita_inizio AS validita_inizio_esercizio,");
                strSql.AppendLine("        Imprese_Progetti.validita_fine AS validita_fine_esercizio,");
                strSql.AppendLine("        Imprese_Progetti.data_fine_prevista AS data_raccolta_prevista,");
                strSql.AppendLine("        ISNULL(Appezzamento.SupBZ_Riduzione,0) as SupBZ_Riduzione,");
                strSql.AppendLine("        ISNULL(Appezzamento.DistBZ_CorpiIdrici,0) as DistBZ_CorpiIdrici,");
                strSql.AppendLine("        ISNULL(Appezzamento.DistBZ_AreeResPub,0) as DistBZ_AreeResPub,");
                strSql.AppendLine("        ISNULL(Appezzamento.DistBZ_Allevamenti,0) as DistBZ_Allevamenti,");
                strSql.AppendLine("        ISNULL(Appezzamento.DistBZ_VegNatNonColt,0) as DistBZ_VegNatNonColt,");
                strSql.AppendLine("        ISNULL(N_Massimo.val_cod,NULL) as N_Massimo, ");
                strSql.AppendLine("        ISNULL(P_Massimo.val_cod,NULL) as P_Massimo, ");
                strSql.AppendLine("        ISNULL(K_Massimo.val_cod,NULL) as K_Massimo, ");
                strSql.AppendLine("        ISNULL(Mg_Massimo.val_cod,NULL) as Mg_Massimo ");

                strSql.AppendLine(" FROM ");
                strSql.AppendLine("        Reg_Impianti ");

                strSql.AppendLine(" JOIN ");
                strSql.AppendLine("        Imprese_Progetti ");
                strSql.AppendLine("        ON Reg_Impianti.PIVA = Imprese_Progetti.Piva ");
                strSql.AppendLine("        AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod ");
                strSql.AppendLine("        AND Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza ");
                strSql.AppendLine("        AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg ");

                strSql.AppendLine(" INNER JOIN");
                strSql.AppendLine("        Appezzamento");
                strSql.AppendLine("        ON Reg_Impianti.PIVA = Appezzamento.PIVA");
                strSql.AppendLine("        AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND");
                strSql.AppendLine("        Reg_Impianti.APPEZZA = Appezzamento.APPEZZA");

                strSql.AppendLine(" LEFT JOIN");
                strSql.AppendLine("        #N_Massimo N_Massimo");
                strSql.AppendLine("        ON N_Massimo.Piva = Reg_Impianti.PIVA");
                strSql.AppendLine("        AND N_Massimo.Sa_Cod = Reg_Impianti.SA_COD");
                strSql.AppendLine("        AND N_Massimo.Appezza = Reg_Impianti.APPEZZA");
                strSql.AppendLine("        AND N_Massimo.Id_Reg = Reg_Impianti.ID_REG");

                strSql.AppendLine(" LEFT JOIN");
                strSql.AppendLine("        #P_Massimo P_Massimo");
                strSql.AppendLine("        ON P_Massimo.Piva = Reg_Impianti.PIVA");
                strSql.AppendLine("        AND P_Massimo.Sa_Cod = Reg_Impianti.SA_COD");
                strSql.AppendLine("        AND P_Massimo.Appezza = Reg_Impianti.APPEZZA");
                strSql.AppendLine("        AND P_Massimo.Id_Reg = Reg_Impianti.ID_REG");

                strSql.AppendLine(" LEFT JOIN");
                strSql.AppendLine("        #K_Massimo K_Massimo");
                strSql.AppendLine("        ON K_Massimo.Piva = Reg_Impianti.PIVA");
                strSql.AppendLine("        AND K_Massimo.Sa_Cod = Reg_Impianti.SA_COD");
                strSql.AppendLine("        AND K_Massimo.Appezza = Reg_Impianti.APPEZZA");
                strSql.AppendLine("        AND K_Massimo.Id_Reg = Reg_Impianti.ID_REG");

                strSql.AppendLine(" LEFT JOIN");
                strSql.AppendLine("        #Mg_Massimo Mg_Massimo");
                strSql.AppendLine("        ON Mg_Massimo.Piva = Reg_Impianti.PIVA");
                strSql.AppendLine("        AND Mg_Massimo.Sa_Cod = Reg_Impianti.SA_COD");
                strSql.AppendLine("        AND Mg_Massimo.Appezza = Reg_Impianti.APPEZZA");
                strSql.AppendLine("        AND Mg_Massimo.Id_Reg = Reg_Impianti.ID_REG");

                strSql.AppendLine("LEFT JOIN  ");
                strSql.AppendLine("        Cultivar");
                strSql.AppendLine("        ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod");

                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("        SpecieVegetali");
                strSql.AppendLine("        ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod");

                strSql.AppendLine(" WHERE");
                strSql.AppendLine("        Imprese_Progetti.Validita_Inizio <= @dataAttivita");
                strSql.AppendLine("        AND Imprese_Progetti.Validita_fine >= @dataAttivita");
                strSql.AppendLine("        AND Reg_Impianti.PIVA = @piva ");
                strSql.AppendLine("        AND Reg_Impianti.SA_COD = @saCod ");
                strSql.AppendLine("        AND Reg_Impianti.APPEZZA = @appezza ");
                strSql.AppendLine("        AND Reg_Impianti.ID_REG = @idReg ");

                parametriSql.Add("@dataAttivita", dataAttivita);
                parametriSql.Add("@piva", piva);
                parametriSql.Add("@saCod", sacod);
                parametriSql.Add("@appezza", appezza);
                parametriSql.Add("@idReg", idReg);

                return await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <summary>
        /// Drops temporary tables used for storing active principles data if they exist.
        /// </summary>
        /// <param name="strSql">The StringBuilder object to which the SQL commands will be appended.</param>
        /// <remarks>
        /// This method generates SQL commands to drop the following temporary tables:
        /// <list type="bullet">
        /// <item><description>#N_Massimo</description></item>
        /// <item><description>#P_Massimo</description></item>
        /// <item><description>#K_Massimo</description></item>
        /// <item><description>#Mg_Massimo</description></item>
        /// </list>
        /// </remarks>
        private void DropActivePrinciplesTempTables(StringBuilder strSql)
        {
            strSql.AppendLine(" IF OBJECT_ID('tempdb..#N_Massimo') IS NOT NULL");
            strSql.AppendLine("    DROP TABLE #N_Massimo");
            strSql.AppendLine(" IF OBJECT_ID('tempdb..#P_Massimo') IS NOT NULL");
            strSql.AppendLine("    DROP TABLE #P_Massimo");
            strSql.AppendLine(" IF OBJECT_ID('tempdb..#K_Massimo') IS NOT NULL");
            strSql.AppendLine("    DROP TABLE #K_Massimo");
            strSql.AppendLine(" IF OBJECT_ID('tempdb..#Mg_Massimo') IS NOT NULL");
            strSql.AppendLine("    DROP TABLE #Mg_Massimo");
        }

        /// <summary>
        /// Creates temporary tables for storing active principles data by selecting from Reg_Impianti_Codici and Imprese_Progetti tables.
        /// </summary>
        /// <param name="strSql">The StringBuilder object to which the SQL commands will be appended.</param>
        /// <remarks>
        /// This method generates SQL commands to create and populate four temporary tables:
        /// <list type="bullet">
        /// <item><description>#N_Massimo - Contains data filtered by Impianto_LimiteN</description></item>
        /// <item><description>#P_Massimo - Contains data filtered by Impianto_LimiteP</description></item>
        /// <item><description>#K_Massimo - Contains data filtered by Impianto_LimiteK</description></item>
        /// <item><description>#Mg_Massimo - Contains data filtered by Impianto_LimiteMg</description></item>
        /// </list>
        /// Each temporary table is created by selecting distinct records from Reg_Impianti_Codici joined with Imprese_Progetti,
        /// filtered by specific conditions including project codes and register IDs.
        /// The selection is further filtered by different id_cod values from Enum_CodiciAnagrafe.
        /// </remarks>
        private void CreateActivePrinciplesTempTables(StringBuilder strSql)
        {
            strSql.AppendLine("    SELECT DISTINCT ric.val_cod, ric.PIVA, ric.sa_cod, ric.appezza, ric.Id_Reg, ric.Progetto_Cod ");
            strSql.AppendLine("    INTO #N_Massimo");
            strSql.AppendLine("    FROM Reg_Impianti_Codici ric");
            strSql.AppendLine("    INNER JOIN Imprese_Progetti ON Imprese_Progetti.Piva = ric.PIVA");
            strSql.AppendLine("        AND Imprese_Progetti.Sa_Cod = ric.sa_cod");
            strSql.AppendLine("        AND Imprese_Progetti.Appezza = ric.appezza");
            strSql.AppendLine("        AND Imprese_Progetti.Id_Reg = ric.Id_Reg");
            strSql.AppendLine("    WHERE ric.Piva = Imprese_Progetti.PIVA");
            strSql.AppendLine("        AND ric.sa_cod = Imprese_Progetti.sa_cod");
            strSql.AppendLine("        AND ric.appezza = Imprese_Progetti.appezza");
            strSql.AppendLine("        AND ric.Id_Reg = Imprese_Progetti.id_reg");
            strSql.AppendLine("        AND ric.Progetto_Cod = Imprese_Progetti.Progetto_Cod");
            strSql.AppendLine($"        AND ric.id_cod={(int)Enum_CodiciAnagrafe.Impianto_LimiteN}");

            strSql.AppendLine("    SELECT DISTINCT ric.val_cod, ric.PIVA, ric.sa_cod, ric.appezza, ric.Id_Reg, ric.Progetto_Cod");
            strSql.AppendLine("    INTO #P_Massimo");
            strSql.AppendLine("    FROM Reg_Impianti_Codici ric");
            strSql.AppendLine("    INNER JOIN Imprese_Progetti ON Imprese_Progetti.Piva = ric.PIVA");
            strSql.AppendLine("        AND Imprese_Progetti.Sa_Cod = ric.sa_cod");
            strSql.AppendLine("        AND Imprese_Progetti.Appezza = ric.appezza");
            strSql.AppendLine("        AND Imprese_Progetti.Id_Reg = ric.Id_Reg");
            strSql.AppendLine("    WHERE ric.Piva = Imprese_Progetti.PIVA");
            strSql.AppendLine("        AND ric.sa_cod = Imprese_Progetti.sa_cod");
            strSql.AppendLine("        AND ric.appezza = Imprese_Progetti.appezza");
            strSql.AppendLine("        AND ric.Id_Reg = Imprese_Progetti.id_reg");
            strSql.AppendLine("        AND ric.Progetto_Cod = Imprese_Progetti.Progetto_Cod");
            strSql.AppendLine($"        AND ric.id_cod={(int)Enum_CodiciAnagrafe.Impianto_LimiteP}");

            strSql.AppendLine("    SELECT DISTINCT ric.val_cod, ric.PIVA, ric.sa_cod, ric.appezza, ric.Id_Reg, ric.Progetto_Cod");
            strSql.AppendLine("    INTO #K_Massimo");
            strSql.AppendLine("    FROM Reg_Impianti_Codici ric");
            strSql.AppendLine("    INNER JOIN Imprese_Progetti ON Imprese_Progetti.Piva = ric.PIVA");
            strSql.AppendLine("        AND Imprese_Progetti.Sa_Cod = ric.sa_cod");
            strSql.AppendLine("        AND Imprese_Progetti.Appezza = ric.appezza");
            strSql.AppendLine("        AND Imprese_Progetti.Id_Reg = ric.Id_Reg");
            strSql.AppendLine("    WHERE ric.Piva = Imprese_Progetti.PIVA");
            strSql.AppendLine("        AND ric.sa_cod = Imprese_Progetti.sa_cod");
            strSql.AppendLine("        AND ric.appezza = Imprese_Progetti.appezza");
            strSql.AppendLine("        AND ric.Id_Reg = Imprese_Progetti.id_reg");
            strSql.AppendLine("        AND ric.Progetto_Cod = Imprese_Progetti.Progetto_Cod");
            strSql.AppendLine($"        AND ric.id_cod={(int)Enum_CodiciAnagrafe.Impianto_LimiteK}");

            strSql.AppendLine("    SELECT DISTINCT ric.val_cod, ric.PIVA, ric.sa_cod, ric.appezza, ric.Id_Reg, ric.Progetto_Cod");
            strSql.AppendLine("    INTO #Mg_Massimo");
            strSql.AppendLine("    FROM Reg_Impianti_Codici ric");
            strSql.AppendLine("    INNER JOIN Imprese_Progetti ON Imprese_Progetti.Piva = ric.PIVA");
            strSql.AppendLine("        AND Imprese_Progetti.Sa_Cod = ric.sa_cod");
            strSql.AppendLine("        AND Imprese_Progetti.Appezza = ric.appezza");
            strSql.AppendLine("        AND Imprese_Progetti.Id_Reg = ric.Id_Reg");
            strSql.AppendLine("    WHERE ric.Piva = Imprese_Progetti.PIVA");
            strSql.AppendLine("        AND ric.sa_cod = Imprese_Progetti.sa_cod");
            strSql.AppendLine("        AND ric.appezza = Imprese_Progetti.appezza");
            strSql.AppendLine("        AND ric.Id_Reg = Imprese_Progetti.id_reg");
            strSql.AppendLine("        AND ric.Progetto_Cod = Imprese_Progetti.Progetto_Cod");
            strSql.AppendLine($"        AND ric.id_cod={(int)Enum_CodiciAnagrafe.Impianto_LimiteMg}");
        }

        /// <summary>
        /// Recupera in una singola query denormalizzata tutte le combinazioni
        /// Azienda + Appezzamento + Impianto + Esercizio con esercizio in stato Aperto
        /// per le PIVA fornite.
        /// Riferimento spec: DS01-BL CaricamentoPerimetroFiltrato — STEP 2/3.
        /// </summary>
        public async Task<DataTable> LeggiEserciziApertixPivasAsync(List<string> pivas, AgronicaCoreParametriServer objParametriServer, bool dataAttuale = false)
        {
            if (pivas is null || pivas.Count == 0)
                throw new ArgumentException("La lista di PIVA non può essere vuota.", nameof(pivas));

            var parametriSql = new Dictionary<string, object>
            {
                // Id_Cod = 1301 (Enum_CodiciAnagrafe.Distinta_Chiusa) — identifica il flag "esercizio chiuso"
                // nella tabella Reg_Impianti_Codici. Se il record non esiste per questo esercizio,
                // il LEFT JOIN produce NULL, che viene interpretato come "aperto".
                { "@flag_esercizio_chiuso", (int)Enum_CodiciAnagrafe.Distinta_Chiusa }
            };

            if (dataAttuale)
                parametriSql["@dataAttuale"] = DateTime.Today;

            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();
            parSqlIn.Add("@pivas", FormatClauseIn(pivas));

            var strSql = new StringBuilder();
            strSql.AppendLine("SELECT");
            strSql.AppendLine("    imp.PIVA                                AS piva_azienda,");
            strSql.AppendLine("    ISNULL(imp.rag_soc, '')                AS nome_azienda,");
            strSql.AppendLine("    ri.APPEZZA                            AS id_appezzamento,");
            strSql.AppendLine("    ISNULL(app.APP_NOME, '')               AS nome_appezzamento,");
            strSql.AppendLine("    prj.progetto_cod                       AS id_esercizio,");
            strSql.AppendLine("    ISNULL(prj.progetto_nome, '')          AS nome_esercizio,");
            strSql.AppendLine("    ri.ID_REG                              AS id_impianto,");
            // sa_cod and appezza are exposed as separate columns to build AppezzamentoGeoKey in the BIZ layer
            strSql.AppendLine("    ri.SA_COD                             AS sa_cod,");
            strSql.AppendLine("    ri.APPEZZA                            AS appezza,");
            strSql.AppendLine("    ISNULL(ri.sup_imp, 0)                 AS superficie_ha,");
            strSql.AppendLine("    sv.Veg_Cod                            AS cod_specie,");
            strSql.AppendLine("    ISNULL(sv.Veg_Des, '')                AS nome_specie,");
            strSql.AppendLine("    cv.Cul_Cod                            AS cod_varieta,");
            strSql.AppendLine("    ISNULL(cv.Cul_Des, '')                AS nome_varieta,");
            strSql.AppendLine("    prj.validita_inizio                   AS data_inizio_esercizio,");
            strSql.AppendLine("    prj.validita_fine                     AS data_fine_esercizio");
            strSql.AppendLine("FROM Imprese imp");
            strSql.AppendLine("INNER JOIN Reg_Impianti ri");
            strSql.AppendLine("    ON  ri.PIVA    = imp.PIVA");
            strSql.AppendLine("INNER JOIN Appezzamento app");
            strSql.AppendLine("    ON  app.PIVA   = ri.PIVA");
            strSql.AppendLine("    AND app.SA_COD  = ri.SA_COD");
            strSql.AppendLine("    AND app.APPEZZA = ri.APPEZZA");
            strSql.AppendLine("INNER JOIN Imprese_Progetti prj");
            strSql.AppendLine("    ON  prj.Piva    = ri.PIVA");
            strSql.AppendLine("    AND prj.Sa_Cod  = ri.SA_COD");
            strSql.AppendLine("    AND prj.Appezza = ri.APPEZZA");
            strSql.AppendLine("    AND prj.Id_Reg  = ri.ID_REG");
            // LEFT JOIN to read the esercizio-chiuso flag; NULL means no record → open
            strSql.AppendLine("LEFT JOIN Reg_Impianti_Codici ric_esercizio_chiuso ");
            strSql.AppendLine("    ON  ric_esercizio_chiuso.PIVA         = ri.PIVA");
            strSql.AppendLine("    AND ric_esercizio_chiuso.SA_COD        = ri.SA_COD");
            strSql.AppendLine("    AND ric_esercizio_chiuso.APPEZZA       = ri.APPEZZA");
            strSql.AppendLine("    AND ric_esercizio_chiuso.ID_REG        = ri.ID_REG");
            strSql.AppendLine("    AND ric_esercizio_chiuso.Progetto_Cod  = prj.progetto_cod");
            strSql.AppendLine("    AND ric_esercizio_chiuso.Id_Cod        = @flag_esercizio_chiuso");
            // Species / variety
            strSql.AppendLine("LEFT JOIN Cultivar cv");
            strSql.AppendLine("    ON  cv.Cul_Cod = ri.CUL_COD");
            strSql.AppendLine("LEFT JOIN SpecieVegetali sv");
            strSql.AppendLine("    ON  sv.Veg_Cod = cv.Veg_Cod");
            strSql.AppendLine("WHERE imp.PIVA IN (@pivas)");
            // Keep only open esercizi: no chiuso-flag record (NULL) OR flag explicitly '0'
            strSql.AppendLine("  AND (ric_esercizio_chiuso.val_cod IS NULL OR ric_esercizio_chiuso.val_cod = '0')");
            // filtro data attuale: mantieni solo gli esercizi la cui finestra di validità comprende oggi
            if (dataAttuale)
            {
                strSql.AppendLine("  AND prj.validita_inizio <= @dataAttuale AND prj.validita_fine >= @dataAttuale");
            }

            strSql.AppendLine("ORDER BY imp.rag_soc, app.APP_NOME, prj.progetto_nome;");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }


        public async Task<DataTable> LeggiEserciziDSSNutrizioneAsync(string piva,
                                                                    int? saCod,
                                                                    int annoSolare,
                                                                    AgronicaCoreParametriServer objParametriServer,
                                                                    bool filtroVisibilitaUtente = false)
        {
            var parSql = new Dictionary<string, object>();
            DataTable result;

            try
            {
                var annoInizio = new DateTime(annoSolare, 1, 1);
                var annoFine = new DateTime(annoSolare, 12, 31);

                parSql.Add("@piva", piva);
                parSql.Add("@annoInizio", annoInizio);
                parSql.Add("@annoFine", annoFine);
                parSql.Add("@utenteUsername", objParametriServer.UtenteUsername);

                var stb = new StringBuilder();

                // ---------------------------------------------------------------------------
                // CTE: Appezzamenti attivi nel periodo dell'anno solare.
                // Filtro validità: Reg_Impianti attivi a oggi; Imprese_Progetti sovrapposti
                // all'anno solare (validita_inizio <= 31/12 E validita_fine >= 01/01).
                // DS05-BL: Persistenze Coinvolte - Reg_Impianti, Appezzamento, Imprese_Progetti.
                // ---------------------------------------------------------------------------
                stb.AppendLine("WITH AppezzamentiValidi AS (");
                stb.AppendLine("    SELECT");
                stb.AppendLine("        ri.PIVA,");
                stb.AppendLine("        ri.SA_COD,");
                stb.AppendLine("        ri.APPEZZA,");
                stb.AppendLine("        ri.ID_REG,");
                stb.AppendLine("        ip.PROGETTO_COD,");
                stb.AppendLine("        centri.Sa_Nome,");
                stb.AppendLine("        app.APP_NOME              AS Nome_Appezzamento,");
                stb.AppendLine("        sv.VEG_COD                AS Specie_Cod,");
                stb.AppendLine("        sv.Veg_Des                AS Specie_Vegetale,");
                stb.AppendLine("        ri.CUL_COD                AS Varieta_Cod,");
                stb.AppendLine("        cu.Cul_Des                AS Varieta,");
                stb.AppendLine("        por.PORT_DES              AS Portinnesto,");
                stb.AppendLine("        ISNULL(ip.P_HA, 0) *  ri.Sup_Imp            AS Num_Piante,");
                stb.AppendLine("    CASE WHEN ip.Stato_Impianto = 102 THEN 'In Produzione' ELSE COALESCE(FasiCicloColturale_Anagrafiche.Fase_Des, '') END as Stato_Impianto, ");
                stb.AppendLine("        ri.Sup_Imp               AS Superficie_Ha,");
                stb.AppendLine("        ip.Data_Inizio_Prevista               AS Data_Semina_Prevista,");
                stb.AppendLine("        app.x,");
                stb.AppendLine("        app.y,");
                stb.AppendLine("        centri.lat AS Centro_Lat,");
                stb.AppendLine("        centri.long AS Centro_Long,");
                stb.AppendLine("        ip.Validita_Inizio,");
                stb.AppendLine("        ip.Validita_Fine");
                stb.AppendLine("    FROM Reg_Impianti ri");
                stb.AppendLine("    INNER JOIN Appezzamento app");
                stb.AppendLine("        ON  app.PIVA    = ri.PIVA");
                stb.AppendLine("        AND app.SA_COD  = ri.SA_COD");
                stb.AppendLine("        AND app.APPEZZA = ri.APPEZZA");
                stb.AppendLine("    INNER JOIN Imprese_Progetti ip");
                stb.AppendLine("        ON  ip.Piva    = ri.PIVA");
                stb.AppendLine("        AND ip.Sa_Cod  = ri.SA_COD");
                stb.AppendLine("        AND ip.Appezza = ri.APPEZZA");
                stb.AppendLine("        AND ip.Id_Reg  = ri.ID_REG");
                stb.AppendLine("    INNER JOIN Centri_Aziendali centri");
                stb.AppendLine("        ON  centri.Piva    = ri.PIVA");
                stb.AppendLine("        AND centri.Sa_Cod  = ri.SA_COD");
                stb.AppendLine("    LEFT JOIN Cultivar cu");
                stb.AppendLine("        ON  cu.Cul_Cod = ri.CUL_COD");
                stb.AppendLine("    LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = cu.VEG_COD");
                stb.AppendLine("    LEFT JOIN Portinnesti por ON por.PORT_COD = ri.PORT_COD");
                stb.AppendLine("    LEFT JOIN FasiCicloColturale_Anagrafiche ON ip.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod ");

                // Filtro visibilità centri autorizzati per l'utente (DS05-BL: Utenti_Visibilita_Appoggio).
                if (filtroVisibilitaUtente)
                {
                    stb.AppendLine("    INNER JOIN Utenti_Visibilita_Appoggio uva (NOLOCK)");
                    stb.AppendLine("        ON  uva.Piva     = ri.PIVA");
                    stb.AppendLine("        AND uva.Sa_Cod   = ri.SA_COD");
                    stb.AppendLine("        AND uva.Username = @utenteUsername");
                }

                stb.AppendLine("    WHERE ri.PIVA = @piva");

                if (saCod > 0)
                {
                    parSql.Add("@saCod", saCod);
                    stb.AppendLine("      AND ri.SA_COD = @saCod");
                }

                // Filtro anno solare su esercizio (Imprese_Progetti deve sovrapporsi all'anno)
                stb.AppendLine("      AND ip.Validita_Inizio <= @annoFine");
                stb.AppendLine("      AND ip.Validita_Fine   >= @annoInizio");
                // Filtro attività corrente su Reg_Impianti
                stb.AppendLine("      AND ri.Validita_Inizio <= GETDATE()");
                stb.AppendLine("      AND ri.Validita_Fine   >= GETDATE()");
                stb.AppendLine("),");

                // Conteggio totale per la paginazione lato API
                stb.AppendLine("TotaleCount AS (");
                stb.AppendLine("    SELECT COUNT(*) AS Totale FROM AppezzamentiValidi");
                stb.AppendLine("),");

                // ---------------------------------------------------------------------------
                // CTE: Coordinate geografiche dal centroide del poligono GIS.
                // DS05-BL: Gis_ElementiGrafici, Gis_Entita (coordinate geografiche centroide).
                // ---------------------------------------------------------------------------
                stb.AppendLine("GisCoordinate AS (");
                stb.AppendLine("    SELECT Piva, Sa_Cod, Appezza, Lat = pt.Lat, Lng = pt.Long ");
                stb.AppendLine("    FROM (");
                // Calcolo centroide dal poligono geografico; STY = latitudine, STX = longitudine
                stb.AppendLine("         Select ge.piva, ge.sa_cod, ge.appezza, pt = geg.Poligono_GeoEntity.EnvelopeCenter(), pos = ROW_NUMBER() OVER (PARTITION BY ge.PIVA, ge.SA_COD, ge.Appezza ORDER BY ge.PIVA, ge.SA_COD, ge.APPEZZA, LayerElementiGrafici_Cod DESC) ");
                stb.AppendLine("         FROM Gis_ElementiGrafici geg ");
                stb.AppendLine("         INNER JOIN Gis_Entita ge On ge.Entita_Cod = geg.Entita_Cod ");
                stb.AppendLine("         WHERE ge.Piva = @PIVA AND LayerElementiGrafici_Cod In (1, 19) --1 -> Appezzamenti, 19 -> Impianti ");
                stb.AppendLine("        ) T WHERE pos = 1 ");
                stb.AppendLine("),");

                // ---------------------------------------------------------------------------
                // CTE: Ultima analisi terreno per appezzamento (la più recente <= GETDATE()).
                // DS05-BL: Analisi_EntitaxTestata, Analisi_Testata, Analisi_Dettagli.
                // NOTA: I codici parametro per granulometria (sabbia/limo/argilla) sono hardcoded.
                // ---------------------------------------------------------------------------
                stb.AppendLine("UltimaAnalisiTerreno AS (");
                stb.AppendLine("    SELECT");
                stb.AppendLine("        aext.PIVA,");
                stb.AppendLine("        aext.SA_COD,");
                stb.AppendLine("        aext.APPEZZA,");
                stb.AppendLine("        aext.Id_Imp AS ID_REG,");
                stb.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Sabbia}  THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS Sabbia_Percentuale,");
                stb.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Limo}  THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS Limo_Percentuale,");
                stb.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Argilla}  THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS Argilla_Percentuale,");
                stb.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Ntot}  THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS N_Totale,");
                stb.AppendLine("        MAX(aext.Analisi_SuperUser)    AS Analisi_SuperUser,");
                stb.AppendLine("        MAX(aext.Analisi_Testata_Cod)  AS Analisi_Testata_Cod,");
                stb.AppendLine("        atesta.Analisi_Testata_Data_Inizio AS Data_Analisi,");
                stb.AppendLine("         ROW_NUMBER() OVER (");
                stb.AppendLine("            PARTITION BY aext.PIVA, aext.SA_COD, aext.APPEZZA, aext.Id_Imp");
                stb.AppendLine("            ORDER BY aext.PIVA, aext.SA_COD, aext.APPEZZA, aext.Id_Imp, atesta.Analisi_Testata_Data_Inizio DESC");
                stb.AppendLine("        ) AS rn");
                stb.AppendLine("    FROM Analisi_EntitaxTestata aext");
                stb.AppendLine("    INNER JOIN Analisi_Testata atesta");
                stb.AppendLine("        ON  atesta.Analisi_SuperUser   = aext.Analisi_SuperUser");
                stb.AppendLine("        AND atesta.Analisi_Testata_Cod = aext.Analisi_Testata_Cod");
                stb.AppendLine("    LEFT JOIN Analisi_Dettagli ad");
                stb.AppendLine("        ON  ad.Analisi_SuperUser   = aext.Analisi_SuperUser");
                stb.AppendLine("        AND ad.Analisi_Testata_Cod = aext.Analisi_Testata_Cod");
                stb.AppendLine("    WHERE atesta.Analisi_Testata_Data_Inizio <= GETDATE() AND GETDATE() <= atesta.Analisi_Testata_Data_Fine");
                stb.AppendLine("    GROUP BY aext.PIVA, aext.SA_COD, aext.APPEZZA, aext.Id_Imp, atesta.Analisi_Testata_Data_Inizio");
                stb.AppendLine("),");

                // ---------------------------------------------------------------------------
                // CTE: Ultima fase fenologica per appezzamento dall'operazione QDCA.
                // DS05-BL: fase fenologica corrente = più recente BBCH da operazione QDCA
                // (Agenda.Lav_Cod = 79, Movimenti.Cau_Mov = '2100').
                // Riferimento: Query Fase Fenologica allegata alla DS05-BL.
                // ---------------------------------------------------------------------------
                stb.AppendLine("UltimaFaseFenologica AS (");
                stb.AppendLine("    SELECT");
                stb.AppendLine("        av.PIVA,");
                stb.AppendLine("        av.SA_COD,");
                stb.AppendLine("        av.APPEZZA,");
                stb.AppendLine("        av.ID_REG,");
                stb.AppendLine("        CONCAT(scb.Stadio_Principale, scb.Seconda_Cifra, scb.Terza_Cifra) AS BBCH_Cod,");
                stb.AppendLine("        ISNULL(svsc.Descrizione, '')           AS BBCH_Descrizione,");
                stb.AppendLine("        mddest.validita_inizio                 AS Data_Fase,");
                stb.AppendLine("        mddest.Id_Agenda                       AS Id_Agenda,");
                stb.AppendLine("        mddest.Id_Mov                          AS Id_Mov,");
                stb.AppendLine("        mddest.Id_Mov_Det                      AS Id_Mov_Det,");
                stb.AppendLine("        ROW_NUMBER() OVER (");
                stb.AppendLine("            PARTITION BY av.PIVA, av.SA_COD, av.APPEZZA, av.ID_REG");
                stb.AppendLine("            ORDER BY mddest.validita_inizio DESC,  CONCAT(scb.Stadio_Principale, scb.Seconda_Cifra, scb.Terza_Cifra) DESC");
                stb.AppendLine("        ) AS rn");
                stb.AppendLine("    FROM AppezzamentiValidi av");
                stb.AppendLine("    INNER JOIN Mov_Destinazioni mddest");
                stb.AppendLine("        ON  mddest.PIVA            = av.PIVA");
                stb.AppendLine("        AND mddest.Sa_Cod          = av.SA_COD");
                stb.AppendLine("        AND mddest.APPEZZA         = av.APPEZZA");
                stb.AppendLine("        AND mddest.ID_destinazione = av.ID_REG");
                stb.AppendLine("        AND mddest.validita_inizio <= GETDATE()");
                stb.AppendLine("    INNER JOIN Movimenti_dettagli mdtg");
                stb.AppendLine("        ON  mdtg.PIVA       = mddest.Piva");
                stb.AppendLine("        AND mdtg.Sa_Cod     = mddest.Sa_Cod");
                stb.AppendLine("        AND mdtg.Id_Agenda  = mddest.Id_Agenda");
                stb.AppendLine("        AND mdtg.Id_Mov     = mddest.Id_Mov");
                stb.AppendLine("        AND mdtg.Id_Mov_Det = mddest.Id_Mov_Det");
                stb.AppendLine("    INNER JOIN Movimenti mov");
                stb.AppendLine("        ON  mov.PIVA           = mdtg.PIVA");
                stb.AppendLine("        AND mov.Sa_Cod         = mdtg.Sa_Cod");
                stb.AppendLine("        AND mov.Id_Agenda      = mdtg.Id_Agenda");
                stb.AppendLine("        AND mov.Id_Mov         = mdtg.Id_Mov");
                stb.AppendLine("        AND mov.Cau_Mov        = '2100'");
                stb.AppendLine("        AND mov.Data_Movimento >= av.Validita_Inizio");
                stb.AppendLine("        AND mov.Data_Movimento <= av.Validita_Fine");
                stb.AppendLine("    INNER JOIN Agenda ag");
                stb.AppendLine("        ON  ag.PIVA      = mov.PIVA");
                stb.AppendLine("        AND ag.Sa_Cod    = mov.Sa_Cod");
                stb.AppendLine("        AND ag.Id_Agenda = mov.Id_Agenda");
                stb.AppendLine("        AND ag.Lav_Cod   = 79");
                stb.AppendLine("    INNER JOIN Mov_Dettaglio_Tecnico mdt");
                stb.AppendLine("        ON  mdt.Piva       = mdtg.PIVA");
                stb.AppendLine("        AND mdt.Sa_Cod     = mdtg.Sa_Cod");
                stb.AppendLine("        AND mdt.Id_Agenda  = mdtg.Id_Agenda");
                stb.AppendLine("        AND mdt.Id_Mov     = mdtg.Id_Mov");
                stb.AppendLine("        AND mdt.Id_Mov_Det = mdtg.Id_Mov_Det");
                stb.AppendLine("    LEFT JOIN SpecieVegetaliXStadiCrescita svsc");
                stb.AppendLine("        ON  svsc.Cod_SS    = mdt.ff_classe");
                stb.AppendLine("    LEFT JOIN Stadi_Crescita_BBCH scb");
                stb.AppendLine("        ON  scb.ID_BBCH    = svsc.ID_BBCH");
                stb.AppendLine("),");

                // ---------------------------------------------------------------------------
                // CTE: Ultimo consiglio nutrizionale per appezzamento (il più recente per data).
                // DS05-BL: Consigli_Nutrizione_Engine (consigli nutrizionali).
                // ---------------------------------------------------------------------------
                stb.AppendLine("UltimoConsiglioPerAppezzamento AS (");
                stb.AppendLine("    SELECT");
                stb.AppendLine("        PIVA, SA_COD, APPEZZA, ID_REG,");
                stb.AppendLine("        ID             AS Consiglio_ID,");
                stb.AppendLine("        Data_Consiglio,");
                stb.AppendLine("        ROW_NUMBER() OVER (");
                stb.AppendLine("            PARTITION BY PIVA, SA_COD, APPEZZA, ID_REG");
                stb.AppendLine("            ORDER BY Data_Consiglio DESC");
                stb.AppendLine("        ) AS rn");
                stb.AppendLine("    FROM Consigli_Nutrizione_Engine");
                stb.AppendLine("    WHERE Validita_Inizio <= GETDATE() AND Validita_Fine >= GETDATE()");
                stb.AppendLine(")");

                // ---------------------------------------------------------------------------
                // Query principale: JOIN degli appezzamenti paginati con tutte le CTE laterali.
                // Restituisce una riga per ogni elemento del consiglio nutrizionale;
                // se il consiglio è assente, una sola riga con campi elemento NULL.
                // Il campo Totale_Appezzamenti è uguale per tutte le righe del risultato.
                // ---------------------------------------------------------------------------
                stb.AppendLine("SELECT");
                stb.AppendLine("    ap.PIVA,");
                stb.AppendLine("    ap.SA_COD,");
                stb.AppendLine("    ap.APPEZZA,");
                stb.AppendLine("    ap.ID_REG,");
                stb.AppendLine("    ap.PROGETTO_COD,");
                stb.AppendLine("    ap.Sa_Nome,");
                stb.AppendLine("    ap.Nome_Appezzamento,");
                stb.AppendLine("    ap.Specie_Vegetale,");
                stb.AppendLine("    ap.Specie_Cod,");
                stb.AppendLine("    ap.Varieta,");
                stb.AppendLine("    ap.Varieta_Cod,");
                stb.AppendLine("    ap.Portinnesto,");
                stb.AppendLine("    ap.Num_Piante,");
                stb.AppendLine("    ap.Stato_Impianto,");
                stb.AppendLine("    ap.Superficie_Ha,");
                stb.AppendLine("    ap.Data_Semina_Prevista,");
                stb.AppendLine("    COALESCE(gis.Lat, IIF(COALESCE(ap.x, 0) = 0, NULL, ap.x), IIF(COALESCE(ap.Centro_Lat, 0) = 0, NULL, ap.Centro_Lat)) AS Lat,");
                stb.AppendLine("    COALESCE(gis.Lng, IIF(COALESCE(ap.y, 0) = 0, NULL, ap.y), IIF(COALESCE(ap.Centro_Long, 0) = 0, NULL, ap.Centro_Long)) AS Lng,");
                stb.AppendLine("    uat.Sabbia_Percentuale,");
                stb.AppendLine("    uat.Limo_Percentuale,");
                stb.AppendLine("    uat.Argilla_Percentuale,");
                stb.AppendLine("    uat.N_Totale,");
                stb.AppendLine("    uat.Analisi_SuperUser,");
                stb.AppendLine("    uat.Analisi_Testata_Cod,");
                stb.AppendLine("    uat.Data_Analisi,");
                stb.AppendLine("    uff.BBCH_Cod,");
                stb.AppendLine("    uff.BBCH_Descrizione,");
                stb.AppendLine("    uff.Data_Fase,");
                stb.AppendLine("    uff.Id_Agenda,");
                stb.AppendLine("    uff.Id_Mov,");
                stb.AppendLine("    uff.Id_Mov_Det,");
                stb.AppendLine("    ucn.Consiglio_ID,");
                stb.AppendLine("    ucn.Data_Consiglio,");
                stb.AppendLine("    cne.Elemento,");
                stb.AppendLine("    cne.Fabbisogno_Minimo,");
                stb.AppendLine("    cne.Fabbisogno_Massimo,");
                stb.AppendLine("    cne.Dose_Consigliata_Minima,");
                stb.AppendLine("    cne.Dose_Consigliata_Massima,");
                stb.AppendLine("    cne.Quantitativo_Presente,");
                stb.AppendLine("    cne.Quantitativo_Minimo_Residuo,");
                stb.AppendLine("    cne.Quantitativo_Massimo_Residuo,");
                stb.AppendLine("    (SELECT Totale FROM TotaleCount) AS Totale_Appezzamenti");
                stb.AppendLine("FROM AppezzamentiValidi ap");
                stb.AppendLine("LEFT JOIN GisCoordinate gis");
                stb.AppendLine("    ON  gis.Piva    = ap.PIVA");
                stb.AppendLine("    AND gis.Sa_Cod  = ap.SA_COD");
                stb.AppendLine("    AND gis.Appezza = ap.APPEZZA");
                stb.AppendLine("LEFT JOIN (SELECT * FROM UltimaAnalisiTerreno  WHERE rn = 1) uat");
                stb.AppendLine("    ON  uat.PIVA    = ap.PIVA");
                stb.AppendLine("    AND uat.SA_COD  = ap.SA_COD");
                stb.AppendLine("    AND uat.APPEZZA = ap.APPEZZA");
                stb.AppendLine("    AND uat.ID_REG  = ap.ID_REG");
                stb.AppendLine("LEFT JOIN (SELECT * FROM UltimaFaseFenologica  WHERE rn = 1) uff");
                stb.AppendLine("    ON  uff.PIVA    = ap.PIVA");
                stb.AppendLine("    AND uff.SA_COD  = ap.SA_COD");
                stb.AppendLine("    AND uff.APPEZZA = ap.APPEZZA");
                stb.AppendLine("    AND uff.ID_REG  = ap.ID_REG");
                stb.AppendLine("LEFT JOIN (SELECT * FROM UltimoConsiglioPerAppezzamento WHERE rn = 1) ucn");
                stb.AppendLine("    ON  ucn.PIVA    = ap.PIVA");
                stb.AppendLine("    AND ucn.SA_COD  = ap.SA_COD");
                stb.AppendLine("    AND ucn.APPEZZA = ap.APPEZZA");
                stb.AppendLine("    AND ucn.ID_REG  = ap.ID_REG");
                // JOIN con gli elementi del consiglio (1 riga per elemento; NULL se consiglio assente)
                stb.AppendLine("LEFT JOIN Consigli_Nutrizione_Engine cne");
                stb.AppendLine("    ON  cne.PIVA           = ucn.PIVA");
                stb.AppendLine("    AND cne.SA_COD         = ucn.SA_COD");
                stb.AppendLine("    AND cne.APPEZZA        = ucn.APPEZZA");
                stb.AppendLine("    AND cne.ID_REG         = ucn.ID_REG");
                stb.AppendLine("    AND cne.Data_Consiglio = ucn.Data_Consiglio");
                stb.AppendLine("ORDER BY ap.Nome_Appezzamento, cne.Elemento;");

                result = await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stb.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }
    }


}
