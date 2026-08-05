using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Operazione.DAL.Resources;
using InData.QuadernoDiCampagna;
using Microsoft.Extensions.Localization;
using OutData.Kendo;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.ReportImpiegoProdottiFitosanitari
{
    public class ReportImpiegoProdottiFitosanitari : BaseDALOperazione, IReportImpiegoProdottiFitosanitari
    {
        public ReportImpiegoProdottiFitosanitari(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<ResultAndKendoColumns> GetReportImpiegoFitosanitariAsync(ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, bool visibilitaLimitataImprese, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable dataTable;

            var kendoColumns = CreateKendoColumns(reportImpiegoProdottiFitosanitariIn);

            var inClauseForPiva = string.Empty;
            if (reportImpiegoProdottiFitosanitariIn.FiltraPerImprese)
            {
                inClauseForPiva = UtilityAgronica.GenerateParameterizedStringForInClause("piva", reportImpiegoProdottiFitosanitariIn.Imprese, parametriSql);
            }

            CreateTemporaryTable_particelleSumOfAreas(stbQuery, reportImpiegoProdottiFitosanitariIn, inClauseForPiva);
            CreateTemporaryTable_particellePercentuali(stbQuery, reportImpiegoProdottiFitosanitariIn, parametriSql);
            CreateTemporaryTable_joinedDataTemp(stbQuery, reportImpiegoProdottiFitosanitariIn, parametriSql, visibilitaLimitataImprese, inClauseForPiva, objParametriServer);

            var includiFertilizzanti = reportImpiegoProdottiFitosanitariIn.IncludiFertilizzanti && !reportImpiegoProdottiFitosanitariIn.FiltraPerSostanzaAttiva;
            if (includiFertilizzanti)
            {
                CreateTemporaryTable_joinedDataTempFert(stbQuery, reportImpiegoProdottiFitosanitariIn, parametriSql, visibilitaLimitataImprese, inClauseForPiva, objParametriServer);
            }

            CreateTemporaryTable_joinedDataWithPrincipiAttiviTemp(stbQuery, reportImpiegoProdottiFitosanitariIn, parametriSql);
            
            if (includiFertilizzanti)
            {
                CreateTemporaryTable_joinedDataTempFertFilteredByZVN(stbQuery, reportImpiegoProdottiFitosanitariIn);
            }
            
            CreateSelectStatement(stbQuery, reportImpiegoProdottiFitosanitariIn, includiFertilizzanti);

            parametriSql.Add("@dataInizio", reportImpiegoProdottiFitosanitariIn.DataInizio);
            parametriSql.Add("@dataFine", reportImpiegoProdottiFitosanitariIn.DataFine);
            if (reportImpiegoProdottiFitosanitariIn.FiltraPerSostanzaAttiva)
            {
                parametriSql.Add("@pa_cod", reportImpiegoProdottiFitosanitariIn.Pa_Cod);
            }

            if (visibilitaLimitataImprese)
            {
                parametriSql.Add("@entitaCod", (int)Enum_TipoEntita.Impresa);
                parametriSql.Add("@username", objParametriServer.UtenteUsername);
                parametriSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser);
            }

            try
            {
                dataTable = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return new ResultAndKendoColumns() { result = dataTable, kendoColumns = kendoColumns.ToArray() };
        }

        private List<KendoColumn> CreateKendoColumns(ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn)
        {
            var kendoColumns = new List<KendoColumn>()
            {
                new KendoColumn() { Field = "Provincia", Title = "Provincia", DataType = "string" },
                new KendoColumn() { Field = "Comune", Title = "Comune", DataType = "string" },
                new KendoColumn() { Field = "Foglio", Title = "Foglio", DataType = "number" },
                new KendoColumn() { Field = "Particella", Title = "Particella", DataType = "number" },
                new KendoColumn() { Field = "ZVN", Title = "ZVN", DataType = "string" },
            };


            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
            {
                kendoColumns.Add(new KendoColumn() { Field = "Codice", Title = "Codice", DataType = "number" });
                kendoColumns.Add(new KendoColumn() { Field = "Prodotto", Title = "Prodotto", DataType = "string" });
                kendoColumns.Add(new KendoColumn() { Field = "Qta", Title = "Quantita", DataType = "number" });
                kendoColumns.Add(new KendoColumn() { Field = "UnitaDiMisura", Title = "Unita_Misura", DataType = "string" });
                kendoColumns.Add(new KendoColumn() { Field = "SostanzaAttiva", Title = "SostanzaAttiva", DataType = "string" });
                kendoColumns.Add(new KendoColumn() { Field = "Titolo", Title = "TitoloInPercentuale", DataType = "number" });
            }
            else
            {
                kendoColumns.Add(new KendoColumn() { Field = "SostanzaAttiva", Title = "SostanzaAttiva", DataType = "string" });
                kendoColumns.Add(new KendoColumn() { Field = "Qta", Title = "Quantita", DataType = "number" });
                kendoColumns.Add(new KendoColumn() { Field = "UnitaDiMisura", Title = "Unita_Misura", DataType = "string" });
            }

            return kendoColumns;
        }

        private void CreateTemporaryTable_particelleSumOfAreas(StringBuilder stbQuery, ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, string inClauseForPiva)
        {
            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("              piva, sa_cod, appezza, sum(area) as areaTotale");
            stbQuery.AppendLine("INTO");
            stbQuery.AppendLine("              #particelleSumOfAreas");
            stbQuery.AppendLine("FROM");
            stbQuery.AppendLine("              AppezzamentiXParticelle");
            stbQuery.AppendLine("WHERE ");
            stbQuery.AppendLine("              1=1");
            if (reportImpiegoProdottiFitosanitariIn.FiltraPerImprese)
            {
                stbQuery.AppendLine("              AND piva IN (");
                stbQuery.AppendLine($"              {inClauseForPiva}");
                stbQuery.AppendLine("              )");
            }
            stbQuery.AppendLine("GROUP BY");
            stbQuery.AppendLine("              piva, sa_cod, appezza");
            stbQuery.AppendLine(";");
            stbQuery.AppendLine("");
        }

        private void CreateTemporaryTable_particellePercentuali(StringBuilder stbQuery, ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, Dictionary<string, object> parametriSql)
        {
            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("              p.piva, p.sa_cod, p.appezza, ap.area / NULLIF(p.areaTotale, 0) as areaPerc, ");
            stbQuery.AppendLine("              ap.PROV, ap.COM, ap.SEZIONE, ap.FOGLIO, ap.NUMERO, ap.SUBALTERNO");
            stbQuery.AppendLine(" INTO");
            stbQuery.AppendLine("              #particellePercentuali");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine("              AppezzamentiXParticelle ap");
            stbQuery.AppendLine(" JOIN");
            stbQuery.AppendLine("              #particelleSumOfAreas p");
            stbQuery.AppendLine("              ON p.PIVA = ap.PIVA and p.SA_COD = ap.SA_COD and p.APPEZZA = ap.APPEZZA");
            stbQuery.AppendLine(" WHERE        1 = 1");

            if (reportImpiegoProdottiFitosanitariIn.FiltraPerComuni)
            {
                stbQuery.AppendLine(" AND        (");
                for (int i = 0; i < reportImpiegoProdottiFitosanitariIn.Comuni.Count; i++)
                {
                    if (i != 0)
                        stbQuery.AppendLine("        OR");

                    stbQuery.AppendLine($"        (ap.PROV = @prov{i} AND ap.COM = @com{i})");
                    
                    parametriSql.Add($"@prov{i}", reportImpiegoProdottiFitosanitariIn.Comuni[i].PROV);
                    parametriSql.Add($"@com{i}", reportImpiegoProdottiFitosanitariIn.Comuni[i].COM);
                }
                stbQuery.AppendLine("            )");
            }
            else if (reportImpiegoProdottiFitosanitariIn.FiltraPerProvince)
            {
                stbQuery.AppendLine("              AND ap.prov IN (");
                stbQuery.AppendLine(UtilityAgronica.GenerateParameterizedStringForInClause("prov", reportImpiegoProdottiFitosanitariIn.Province, parametriSql));
                stbQuery.Append(")");
            }


            stbQuery.AppendLine(";");
            stbQuery.AppendLine("");
        }

        private void CreateTemporaryTable_joinedDataTemp(StringBuilder stbQuery, ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, Dictionary<string, object> parametriSql, bool visibilitaLimitataImprese, string inClauseForPiva, AgronicaCoreParametriServer objParametriServer)
        {
            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("             lp.PROV, lp.PROVINCIA, i.COM, i.LOCALITA, pp.FOGLIO, pp.NUMERO,");
            stbQuery.AppendLine("             mdet.Pro_Cod, mdet.Udm_Cod, um.Udm_Sim,");
            
            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                stbQuery.AppendLine("          f.Fr_Cod, f.Fr_Des, ");

            stbQuery.AppendLine("           CASE");
            stbQuery.AppendLine("             WHEN zp.Zona_Cod IS NULL THEN 'NO'");
            stbQuery.AppendLine("             ELSE 'SI'");
            stbQuery.AppendLine("           END as ZVN,");
            stbQuery.AppendLine("             CASE");
            stbQuery.AppendLine("             WHEN CHARINDEX('§', Principi.strName) > 0 THEN LEFT(Principi.strName, CHARINDEX('§', Principi.strName) -1)");
            stbQuery.AppendLine("               ELSE NULL");
            stbQuery.AppendLine("           END As Pa_Cod,");
            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
            {
                stbQuery.AppendLine("             CASE");
                stbQuery.AppendLine("             WHEN CHARINDEX('§', Principi.strName) > 0 THEN SUBSTRING(Principi.strName, CHARINDEX('§', Principi.strName) +1, LEN(Principi.strName))");
                stbQuery.AppendLine("               ELSE NULL");
                stbQuery.AppendLine("           END As Pa_Perc,");
                stbQuery.AppendLine("           mdes.Qta * pp.areaPerc as QtaTot");
            }
            else
            {
                stbQuery.AppendLine("             CASE");
                stbQuery.AppendLine("              WHEN CHARINDEX('§', Principi.strName) > 0");
                stbQuery.AppendLine("                THEN mdes.Qta * pp.areaPerc * (CAST(SUBSTRING(Principi.strName, CHARINDEX('§', Principi.strName) + 1, LEN(Principi.strName)) AS FLOAT) / 100)");
                stbQuery.AppendLine("               ELSE NULL");
                stbQuery.AppendLine("           END AS Pa_Qta");
            }

            stbQuery.AppendLine(" INTO");
            stbQuery.AppendLine("           #joinedDataTemp");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine("           agenda a");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           movimenti m");
            stbQuery.AppendLine("           on a.Id_Agenda = m.Id_Agenda");
            stbQuery.AppendLine("           and a.Sa_Cod = m.Sa_Cod");
            stbQuery.AppendLine("           and a.Piva = m.Piva");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           movimenti_dettagli mdet");
            stbQuery.AppendLine("           on m.piva = mdet.piva");
            stbQuery.AppendLine("           and m.Sa_Cod = mdet.Sa_Cod");
            stbQuery.AppendLine("           and m.Id_Agenda = mdet.Id_Agenda");
            stbQuery.AppendLine("           and m.id_mov = mdet.id_mov");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           Mov_Destinazioni mdes");
            stbQuery.AppendLine("           on mdes.piva = mdet.piva");
            stbQuery.AppendLine("           and mdes.sa_cod = mdet.Sa_Cod");
            stbQuery.AppendLine("           and mdes.Id_Agenda = mdet.Id_Agenda");
            stbQuery.AppendLine("           and mdes.id_mov = mdet.id_mov");
            stbQuery.AppendLine("           and mdes.id_mov_det = mdet.id_mov_det");

            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
            {
                stbQuery.AppendLine(" join");
                stbQuery.AppendLine("           Formulati f");
                stbQuery.AppendLine("           on f.Fr_Cod = mdet.Pro_Cod");
            }

            if (visibilitaLimitataImprese)
            {
                stbQuery.AppendLine(" join");
                stbQuery.AppendLine("           Utenti_Visibilita_Appoggio uvap");
                stbQuery.AppendLine("           ON a.Piva = uvap.Piva");
            }

            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           #particellePercentuali pp");
            stbQuery.AppendLine("           on pp.Piva = mdes.piva");
            stbQuery.AppendLine("             and pp.Sa_Cod = mdes.sa_cod");
            stbQuery.AppendLine("             and pp.APPEZZA = mdes.Appezza");
            stbQuery.AppendLine(" left join");
            stbQuery.AppendLine("           ZonexParticelle zp");
            stbQuery.AppendLine("           on pp.PROV = zp.PROV");
            stbQuery.AppendLine("           and pp.COM = zp.COM");
            stbQuery.AppendLine("           and pp.SEZIONE = zp.SEZIONE");
            stbQuery.AppendLine("           and pp.FOGLIO = zp.FOGLIO");
            stbQuery.AppendLine("           and pp.NUMERO = zp.NUMERO");
            stbQuery.AppendLine("           and pp.SUBALTERNO = zp.SUBALTERNO");
            stbQuery.AppendLine($"           and zp.Zona_Cod = {(int)Enum_Zone.ZVN}");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           Lista_Province lp");
            stbQuery.AppendLine("           on lp.PROV = pp.PROV");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           ISTAT i");
            stbQuery.AppendLine("           on i.COM = pp.COM");
            stbQuery.AppendLine("           and i.PROV = pp.PROV");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           UnitaMisura um");
            stbQuery.AppendLine("           on mdet.Udm_Cod = um.Udm_Cod");
            stbQuery.AppendLine(" CROSS APPLY");
            stbQuery.AppendLine("           dbo.fSplit(PrincipiAttivi, '|') As Principi");
            if (reportImpiegoProdottiFitosanitariIn.InitialLoading)
                stbQuery.AppendLine(" WHERE        1 = 2");
            else
                stbQuery.AppendLine(" WHERE        1 = 1");
            stbQuery.AppendLine($"           AND m.Cau_Mov = '{CAU_MOV.CAU_TRATTAMENTO}'");
            stbQuery.AppendLine("           AND m.Inviato >= 0 ");
            stbQuery.AppendLine($"           AND a.Lav_Cod IN ({LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO}, {LAV_COD.LAVCOD_DISERBO}, {LAV_COD.LAVCOD_DISSECCAMENTO}, {LAV_COD.LAVCOD_GEODISINFESTAZIONE}, ");
            stbQuery.AppendLine($"           {LAV_COD.LAVCOD_CONCIA_SEME}, {LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE}, {LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE}, {LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA})");
            if (reportImpiegoProdottiFitosanitariIn.FiltraPerImprese)
            {
                stbQuery.AppendLine("              AND a.PIVA IN (");
                stbQuery.AppendLine(inClauseForPiva);
                stbQuery.Append(")");
            }
            stbQuery.AppendLine("           AND m.Validita_Inizio >= @dataInizio");
            stbQuery.AppendLine("           AND m.Validita_Inizio <= @dataFine");
            stbQuery.AppendLine("           AND mdes.Tipo_Destinazione = 0 ");
            stbQuery.AppendLine($"           AND mdet.Elem_Cod = {ELEM_COD.FORMULATI}");
            stbQuery.AppendLine("           AND CHARINDEX('§', Principi.strName) > 0 ");
            stbQuery.AppendLine("           AND CHARINDEX('§', mdet.PrincipiAttivi) > 0");
            
            if (visibilitaLimitataImprese)
            {
                stbQuery.AppendLine("           and uvap.Entita_Cod = @entitaCod");
                stbQuery.AppendLine("           and uvap.Username = @username");
                stbQuery.AppendLine("           and uvap.PivaSuperUser = @pivaSuperUser");
            }
            stbQuery.AppendLine(";");
            stbQuery.AppendLine("");
        }

        private void CreateTemporaryTable_joinedDataTempFert(StringBuilder stbQuery, ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, Dictionary<string, object> parametriSql, bool visibilitaLimitataImprese, string inClauseForPiva, AgronicaCoreParametriServer objParametriServer)
        {
            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("             lp.PROV, lp.PROVINCIA, i.COM, i.LOCALITA, pp.FOGLIO, pp.NUMERO,");
            stbQuery.AppendLine("             mdet.Pro_Cod, mdet.Udm_Cod, um.Udm_Sim, ");

            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                stbQuery.AppendLine("             fer.Fer_Cod, fer.Fer_Des,");

            stbQuery.AppendLine("           CASE");
            stbQuery.AppendLine("             WHEN zp.Zona_Cod IS NULL THEN 'NO'");
            stbQuery.AppendLine("             ELSE 'SI'");
            stbQuery.AppendLine("           END as ZVN,");
            stbQuery.AppendLine("           mdet.substance as Pa_Des,");

            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
            {
                stbQuery.AppendLine("           mdes.Qta * pp.areaPerc as QtaTot,");
                stbQuery.AppendLine("           CASE ");
                stbQuery.AppendLine("             WHEN mdet.substance = 'N' THEN mdet.Titolo * mdet.ef");
                stbQuery.AppendLine("             ELSE mdet.Titolo");
                stbQuery.AppendLine("           END as Pa_Perc");
            }
            else
            {
                stbQuery.AppendLine("           CASE");
                stbQuery.AppendLine("             WHEN mdet.substance = 'N' THEN mdes.Qta * pp.areaPerc * mdet.Titolo * mdet.ef / 100");
                stbQuery.AppendLine("             ELSE mdes.Qta * pp.areaPerc * mdet.Titolo / 100");
                stbQuery.AppendLine("           END as Pa_Qta");
            }

            //inserire altri elementi

            stbQuery.AppendLine(" INTO");
            stbQuery.AppendLine("           #joinedDataTempFert");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine("           agenda a");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           movimenti m");
            stbQuery.AppendLine("           on a.Id_Agenda = m.Id_Agenda");
            stbQuery.AppendLine("           and a.Sa_Cod = m.Sa_Cod");
            stbQuery.AppendLine("           and a.Piva = m.Piva");
            stbQuery.AppendLine(" join");
            

            stbQuery.AppendLine("           (");
            stbQuery.AppendLine("           SELECT ");
            stbQuery.AppendLine("                      md.piva, md.Sa_Cod, md.id_agenda, md.Id_Mov, md.Id_Mov_Det, md.Pro_Cod, md.Elem_Cod, md.Udm_Cod, 'N' as substance, mt.N as titolo, mt.Efficienza as ef");
            stbQuery.AppendLine("           FROM ");
            stbQuery.AppendLine("                      movimenti_dettagli md");
            stbQuery.AppendLine("           JOIN");
            stbQuery.AppendLine("                      Mov_Dettaglio_Tecnico mt");
            stbQuery.AppendLine("                      on md.piva = mt.piva");
            stbQuery.AppendLine("                      and md.Sa_Cod = mt.Sa_Cod");
            stbQuery.AppendLine("                      and md.Id_Agenda = mt.Id_Agenda");
            stbQuery.AppendLine("                      and md.id_mov = mt.id_mov");
            stbQuery.AppendLine("                      and md.Id_Mov_Det = mt.Id_Mov_Det");
            stbQuery.AppendLine("                      and mt.N <> 0");
            stbQuery.AppendLine("           UNION ALL");
            stbQuery.AppendLine("           SELECT ");
            stbQuery.AppendLine("                      md.piva, md.Sa_Cod, md.id_agenda, md.Id_Mov, md.Id_Mov_Det, md.Pro_Cod, md.Elem_Cod, md.Udm_Cod, 'P' as substance, mt.P as titolo, mt.Efficienza as ef");
            stbQuery.AppendLine("           FROM ");
            stbQuery.AppendLine("                      movimenti_dettagli md");
            stbQuery.AppendLine("           JOIN");
            stbQuery.AppendLine("                      Mov_Dettaglio_Tecnico mt");
            stbQuery.AppendLine("                      on md.piva = mt.piva");
            stbQuery.AppendLine("                      and md.Sa_Cod = mt.Sa_Cod");
            stbQuery.AppendLine("                      and md.Id_Agenda = mt.Id_Agenda");
            stbQuery.AppendLine("                      and md.id_mov = mt.id_mov");
            stbQuery.AppendLine("                      and md.Id_Mov_Det = mt.Id_Mov_Det");
            stbQuery.AppendLine("                      and mt.P <> 0");
            stbQuery.AppendLine("           UNION ALL");
            stbQuery.AppendLine("           SELECT ");
            stbQuery.AppendLine("                      md.piva, md.Sa_Cod, md.id_agenda, md.Id_Mov, md.Id_Mov_Det, md.Pro_Cod, md.Elem_Cod, md.Udm_Cod, 'K' as substance, mt.K as titolo, mt.Efficienza as ef");
            stbQuery.AppendLine("           FROM ");
            stbQuery.AppendLine("                      movimenti_dettagli md");
            stbQuery.AppendLine("           JOIN");
            stbQuery.AppendLine("                      Mov_Dettaglio_Tecnico mt");
            stbQuery.AppendLine("                      on md.piva = mt.piva");
            stbQuery.AppendLine("                      and md.Sa_Cod = mt.Sa_Cod");
            stbQuery.AppendLine("                      and md.Id_Agenda = mt.Id_Agenda");
            stbQuery.AppendLine("                      and md.id_mov = mt.id_mov");
            stbQuery.AppendLine("                      and md.Id_Mov_Det = mt.Id_Mov_Det");
            stbQuery.AppendLine("                      and mt.K <> 0");
            stbQuery.AppendLine("           ) mdet");

            stbQuery.AppendLine("           on m.piva = mdet.piva");
            stbQuery.AppendLine("           and m.Sa_Cod = mdet.Sa_Cod");
            stbQuery.AppendLine("           and m.Id_Agenda = mdet.Id_Agenda");
            stbQuery.AppendLine("           and m.id_mov = mdet.id_mov");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           Mov_Destinazioni mdes");
            stbQuery.AppendLine("           on mdes.piva = mdet.piva");
            stbQuery.AppendLine("           and mdes.sa_cod = mdet.Sa_Cod");
            stbQuery.AppendLine("           and mdes.Id_Agenda = mdet.Id_Agenda");
            stbQuery.AppendLine("           and mdes.id_mov = mdet.id_mov");
            stbQuery.AppendLine("           and mdes.id_mov_det = mdet.id_mov_det");

            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
            {
                stbQuery.AppendLine(" join");
                stbQuery.AppendLine("           Fertilizzanti fer");
                stbQuery.AppendLine("           on fer.Fer_Cod = mdet.Pro_Cod");
            }
            if (visibilitaLimitataImprese)
            {
                stbQuery.AppendLine(" join");
                stbQuery.AppendLine("           Utenti_Visibilita_Appoggio uvap");
                stbQuery.AppendLine("           ON a.Piva = uvap.Piva");
            }
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           #particellePercentuali pp");
            stbQuery.AppendLine("           on pp.Piva = mdes.piva");
            stbQuery.AppendLine("             and pp.Sa_Cod = mdes.sa_cod");
            stbQuery.AppendLine("             and pp.APPEZZA = mdes.Appezza");
            stbQuery.AppendLine(" left join");
            stbQuery.AppendLine("           ZonexParticelle zp");
            stbQuery.AppendLine("           on pp.PROV = zp.PROV");
            stbQuery.AppendLine("           and pp.COM = zp.COM");
            stbQuery.AppendLine("           and pp.SEZIONE = zp.SEZIONE");
            stbQuery.AppendLine("           and pp.FOGLIO = zp.FOGLIO");
            stbQuery.AppendLine("           and pp.NUMERO = zp.NUMERO");
            stbQuery.AppendLine("           and pp.SUBALTERNO = zp.SUBALTERNO");
            stbQuery.AppendLine($"           and zp.Zona_Cod = {(int)Enum_Zone.ZVN}");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           Lista_Province lp");
            stbQuery.AppendLine("           on lp.PROV = pp.PROV");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           ISTAT i");
            stbQuery.AppendLine("           on i.COM = pp.COM");
            stbQuery.AppendLine("           and i.PROV = pp.PROV");
            stbQuery.AppendLine(" join");
            stbQuery.AppendLine("           UnitaMisura um");
            stbQuery.AppendLine("           on mdet.Udm_Cod = um.Udm_Cod");
            if (reportImpiegoProdottiFitosanitariIn.InitialLoading)
                stbQuery.AppendLine(" WHERE        1 = 2");
            else
                stbQuery.AppendLine(" WHERE        1 = 1");
            stbQuery.AppendLine($"           AND m.Cau_Mov = '{CAU_MOV.CAU_LAVORAZIONE}'");
            stbQuery.AppendLine("           AND m.Inviato >= 0 ");
            stbQuery.AppendLine($"           AND a.Lav_Cod IN ({LAV_COD.LAVCOD_DISTRIBUZIONE_CONCIME}, {LAV_COD.LAVCOD_SARCHIATURA_CONCIMAZIONE}, {LAV_COD.LAVCOD_DISTRIBUZIONE_AMMENDANTI}, {LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE}, ");
            stbQuery.AppendLine($"           {LAV_COD.LAVCOD_FERTIRRIGAZIONE}, {LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA})");
            if (reportImpiegoProdottiFitosanitariIn.FiltraPerImprese)
            {
                stbQuery.AppendLine("              AND a.PIVA IN (");
                stbQuery.AppendLine(inClauseForPiva);
                stbQuery.Append(")");
            }
            if (visibilitaLimitataImprese)
            {
                stbQuery.AppendLine("           and uvap.Entita_Cod = @entitaCod");
                stbQuery.AppendLine("           and uvap.Username = @username");
                stbQuery.AppendLine("           and uvap.PivaSuperUser = @pivaSuperUser");
            }
            stbQuery.AppendLine("           AND m.Validita_Inizio >= @dataInizio");
            stbQuery.AppendLine("           AND m.Validita_Inizio <= @dataFine");
            stbQuery.AppendLine("           AND mdes.Tipo_Destinazione = 0 ");
            stbQuery.AppendLine($"           AND mdet.Elem_Cod = {ELEM_COD.FERTILIZZANTI}");
            stbQuery.AppendLine(";");
            stbQuery.AppendLine("");

        }

        private void CreateTemporaryTable_joinedDataWithPrincipiAttiviTemp(StringBuilder stbQuery, ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, Dictionary<string, object> parametriSql)
        {
            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("           j.PROV, j.PROVINCIA,j.COM, j.LOCALITA, j.FOGLIO, j.NUMERO, j.ZVN, pa.Pa_Cod, pa.Pa_Des, j.Udm_Cod, j.Udm_Sim");
            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                stbQuery.AppendLine("           ,j.Fr_Cod, j.Fr_Des, j.Pa_Perc, j.QtaTot");
            else
                stbQuery.AppendLine("           , j.Pa_Qta");

            stbQuery.AppendLine(" INTO");
            stbQuery.AppendLine("             #joinedDataWithPrincipiAttiviTemp");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine("           #joinedDataTemp j");
            stbQuery.AppendLine(" JOIN");
            stbQuery.AppendLine("           PrincipiAttivi pa");
            stbQuery.AppendLine("           ON j.Pa_Cod = pa.Pa_Cod");
            stbQuery.AppendLine(" WHERE ");
            if (reportImpiegoProdottiFitosanitariIn.InitialLoading)
                stbQuery.AppendLine("           1 = 2");
            else
                stbQuery.AppendLine("           1 = 1");

            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
            {
                stbQuery.AppendLine("          AND CAST(j.QtaTot as float) > 0 ");
                stbQuery.AppendLine("          AND CAST(j.Pa_Perc as float) > 0 ");
            }
            else
                stbQuery.AppendLine("          AND CAST(j.Pa_Qta as float) > 0");

            if (reportImpiegoProdottiFitosanitariIn.FiltraPerSostanzaAttiva)
            {
                stbQuery.AppendLine("           AND j.Pa_Cod = @pa_cod");
            }

            if (reportImpiegoProdottiFitosanitariIn.ZVN)
            {
                stbQuery.AppendLine("           AND j.ZVN = 'SI'");
            }

            stbQuery.AppendLine(";");
            stbQuery.AppendLine("");
        }

        private void CreateTemporaryTable_joinedDataTempFertFilteredByZVN(StringBuilder stbQuery, ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn)
        {
            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("           j.PROV, j.PROVINCIA,j.COM, j.LOCALITA, j.FOGLIO, j.NUMERO, j.ZVN, j.Pa_Des, j.Udm_Cod, j.Udm_Sim");
            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                stbQuery.AppendLine("           ,j.Fer_Cod, j.Fer_Des, j.Pa_Perc, j.QtaTot");
            else
                stbQuery.AppendLine("           , j.Pa_Qta");

            stbQuery.AppendLine(" INTO");
            stbQuery.AppendLine("           #joinedDataTempFertFilteredByZVN");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine("           #joinedDataTempFert j");
            stbQuery.AppendLine(" WHERE ");
            if (reportImpiegoProdottiFitosanitariIn.InitialLoading)
                stbQuery.AppendLine("           1 = 2");
            else
                stbQuery.AppendLine("           1 = 1");
            if (reportImpiegoProdottiFitosanitariIn.ZVN)
            {
                stbQuery.AppendLine("           AND j.ZVN = 'SI'");
            }

            stbQuery.AppendLine(";");
            stbQuery.AppendLine("");
        }

        private void CreateSelectStatement(StringBuilder stbQuery, ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, bool includiFertilizzanti)
        {
            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("           Provincia, LOCALITA as Comune, Foglio, NUMERO as Particella, ZVN,");

            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                stbQuery.AppendLine("           Fr_Cod as Codice, Fr_Des as Prodotto,");
            
            stbQuery.AppendLine("           ");
            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
            {
                stbQuery.AppendLine("           CAST(ROUND(SUM(QtaTot), 3) as float) as Qta, Udm_Sim as UnitaDiMisura, Pa_Des as SostanzaAttiva, Pa_Perc as Titolo");
            }
            else
            {
                stbQuery.AppendLine("           Pa_Des as SostanzaAttiva, CAST(ROUND(SUM(Pa_Qta), 6) as float) as Qta, Udm_Sim as UnitaDiMisura");
            }
            stbQuery.AppendLine(" INTO");
            stbQuery.AppendLine("           #tempSommeFormulati");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine("           #joinedDataWithPrincipiAttiviTemp");
            stbQuery.AppendLine(" GROUP BY");
            stbQuery.AppendLine("           PROV, PROVINCIA, COM, LOCALITA, FOGLIO, NUMERO, ZVN, ");

            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                stbQuery.AppendLine("           Fr_Cod, Fr_Des, Pa_Perc,");

            stbQuery.AppendLine("           Pa_Cod, Pa_Des, Udm_Cod, Udm_Sim");

            if (includiFertilizzanti)
            {
                stbQuery.AppendLine(" SELECT");
                stbQuery.AppendLine("           Provincia, LOCALITA as Comune, Foglio, NUMERO as Particella, ZVN,");

                if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                    stbQuery.AppendLine("           Fer_Cod as Codice, Fer_Des as Prodotto,");

                stbQuery.AppendLine("           ");
                if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                {
                    stbQuery.AppendLine("           CAST(ROUND(SUM(QtaTot), 3) as float) as Qta, Udm_Sim as UnitaDiMisura, Pa_Des as SostanzaAttiva, Pa_Perc as Titolo");
                }
                else
                {
                    stbQuery.AppendLine("           Pa_Des as SostanzaAttiva, CAST(ROUND(SUM(Pa_Qta), 6) as float) as Qta, Udm_Sim as UnitaDiMisura");
                }
                stbQuery.AppendLine(" INTO");
                stbQuery.AppendLine("           #tempSommeFertilizzanti");
                stbQuery.AppendLine(" FROM");
                stbQuery.AppendLine("           #joinedDataTempFertFilteredByZVN");
                stbQuery.AppendLine(" GROUP BY");
                stbQuery.AppendLine("           PROV, PROVINCIA, COM, LOCALITA, FOGLIO, NUMERO, ZVN, ");

                if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                    stbQuery.AppendLine("           Fer_Cod, Fer_Des, Pa_Perc,");

                stbQuery.AppendLine("           Pa_Des, Udm_Cod, Udm_Sim");
            }

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("               *");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine("               #tempSommeFormulati");
            stbQuery.AppendLine(" WHERE ");
            
            if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                stbQuery.AppendLine("               Qta >= 0.001");
            else
                stbQuery.AppendLine("               Qta >= 0.000001");

            if (includiFertilizzanti)
            {
                stbQuery.AppendLine(" UNION ALL ");
                stbQuery.AppendLine(" SELECT ");
                stbQuery.AppendLine("               *");
                stbQuery.AppendLine(" FROM");
                stbQuery.AppendLine("               #tempSommeFertilizzanti");
                stbQuery.AppendLine(" WHERE ");
                if (reportImpiegoProdottiFitosanitariIn.VisualizzaProdotti)
                    stbQuery.AppendLine("               Qta >= 0.001");
                else
                    stbQuery.AppendLine("               Qta >= 0.000001");
            }


            stbQuery.AppendLine(" DROP TABLE #particelleSumOfAreas");
            stbQuery.AppendLine(" DROP TABLE #particellePercentuali");
            stbQuery.AppendLine(" DROP TABLE #joinedDataTemp");
            stbQuery.AppendLine(" DROP TABLE #joinedDataWithPrincipiAttiviTemp");
            stbQuery.AppendLine(" DROP TABLE #tempSommeFormulati");

            if (includiFertilizzanti)
            {
                stbQuery.AppendLine(" DROP TABLE #joinedDataTempFert");
                stbQuery.AppendLine(" DROP TABLE #joinedDataTempFertFilteredByZVN");
                stbQuery.AppendLine(" DROP TABLE #tempSommeFertilizzanti");
            }
        }
    }
}
