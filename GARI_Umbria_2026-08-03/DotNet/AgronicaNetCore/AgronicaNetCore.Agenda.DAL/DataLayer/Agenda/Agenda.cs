using AgronicaCoreDTOStd.Identity;
using AgronicaCoreDTOStd.InData.Agea;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.DataExchange.AntaresTrace;
using AgronicaNetCore.Agenda.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using InData.Anagrafica;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Linq;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Agenda.DAL.DataLayer.Agenda
{
    public class Agenda : BaseDALAgenda, IAgenda
    {
        public Agenda(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }
        
        public async Task<DataTable> LeggiAgendaAsync(string? piva, List<int>? idAgendas,
             int? sacod, List<int>? lavCods, List<string>? cauMovs, IntervalloTemporale? dateInterval, AgronicaCoreParametriServer objParametriServer,
             List<FiltroRicercaSemine>? filtroSemineImpianti)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            const int allValues = 0;
            const string allValuesStr = "";

            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            // 1. GESTIONE TABELLA TEMPORANEA FILTRI
            if (filtroSemineImpianti != null && filtroSemineImpianti.Count > 0)
            {
                stbQuery.AppendLine("IF OBJECT_ID('tempdb..#FiltroSemine') IS NOT NULL DROP TABLE #FiltroSemine;");
                stbQuery.AppendLine("CREATE TABLE #FiltroSemine (Piva varchar(11) COLLATE DATABASE_DEFAULT, Sa_Cod int, Appezza int, Id_Reg int, ValiditaInizio datetime, ValiditaFine datetime);");

                foreach (var f in filtroSemineImpianti)
                {
                    string vInizio = f.ValiditaInizio.ToString("yyyyMMdd");
                    string vFine = f.ValiditaFine.ToString("yyyyMMdd");
                    stbQuery.AppendLine($"INSERT INTO #FiltroSemine VALUES ('{f.Piva}', {f.Sa_Cod}, {f.Appezza}, {f.Id_Reg}, '{vInizio}', '{vFine}');");
                }
            }

            stbQuery.AppendLine("SELECT DISTINCT ");
            stbQuery.AppendLine("  Agenda.id_agenda, Agenda.piva, Agenda.Lav_Cod, ");
            stbQuery.AppendLine("  movimenti.Data_Movimento, Agenda.des_lib, Operazioni.Lav_Des, Imprese_Codici.val_cod As CUAA ");
            stbQuery.AppendLine("FROM Agenda ");
            stbQuery.AppendLine("INNER JOIN Movimenti ON ");
            stbQuery.AppendLine("    Movimenti.Id_Agenda = Agenda.Id_Agenda ");
            stbQuery.AppendLine("  AND  Movimenti.PIVA = Agenda.PIVA "); 
            stbQuery.AppendLine("INNER JOIN Operazioni ON Operazioni.Lav_Cod = Agenda.Lav_Cod ");
            stbQuery.AppendLine("INNER JOIN Imprese_Codici ON  ");
            stbQuery.AppendLine("    Imprese_Codici.Piva = Agenda.Piva ");

            // 3. JOIN FILTRO SEMINE
            if (filtroSemineImpianti != null && filtroSemineImpianti.Count > 0)
            {
                stbQuery.AppendLine(" INNER JOIN Mov_Destinazioni md ON Agenda.Id_Agenda = md.Id_Agenda AND Agenda.Piva = md.Piva ");
                stbQuery.AppendLine(" INNER JOIN #FiltroSemine fs ON md.Piva = fs.Piva AND md.Sa_Cod = fs.Sa_Cod AND md.Appezza = fs.Appezza AND md.Id_Destinazione = fs.Id_Reg ");
                stbQuery.AppendLine(" AND Movimenti.Data_Movimento >= fs.ValiditaInizio AND Movimenti.Data_Movimento <= fs.ValiditaFine ");
            }

            stbQuery.AppendLine("WHERE ");
            stbQuery.AppendLine(" Imprese_Codici.Id_Cod = " + (int) Enum_CodiciAnagrafe.CodiceCUAA + " ");
            if (!string.IsNullOrEmpty(piva))
            {
                stbQuery.AppendLine(" AND Agenda.Piva = @piva ");
                parSql.Add("@piva", piva);
            }
            if (sacod != null && sacod != 0)
            {
                stbQuery.AppendLine("  AND Agenda.Sa_Cod in (@sacod) ");
                parSql.Add("@sacod", sacod);
            }
            if (idAgendas != null && idAgendas.Except(new[] { allValues }).Any())
            {
                if (idAgendas.Count == 1)
                {
                    stbQuery.AppendLine("  AND Agenda.Id_Agenda = @id_agenda ");
                    parSql.Add("@id_agenda", idAgendas.First());
                }
                else
                {
                    stbQuery.AppendLine("  AND Agenda.Id_Agenda IN (@id_agendas) ");
                    parSqlIn.Add("@id_agendas", FormatClauseIn(idAgendas.ToList()));
                }
            }

            if (lavCods != null && lavCods.Except(new[] { allValues }).Any())
            {
                if (lavCods.Count == 1)
                {
                    stbQuery.AppendLine("  AND Agenda.lav_cod = @lav_cod ");
                    parSql.Add("@lav_cod", lavCods.First());
                } else
                {
                    stbQuery.AppendLine("  AND Agenda.lav_cod IN (@lav_cods) ");
                    parSqlIn.Add("@lav_cods", FormatClauseIn(lavCods.ToList()));
                }     
            }

            if (dateInterval != null)
            {
                if (dateInterval.inizio != new DateTime(1900, 1, 1))
                {
                    stbQuery.AppendLine("  AND Movimenti.Data_Movimento <= (@fine) ");
                    parSql.Add("@inizio", dateInterval.inizio.ToShortDateString());
                }
                if (dateInterval.inizio != new DateTime(2100, 12, 31))
                {
                    stbQuery.AppendLine("  AND Movimenti.Data_Movimento >= (@inizio) ");
                    parSql.Add("@fine", dateInterval.fine.ToShortDateString());
                }
            }

            if (cauMovs != null && cauMovs.Except(new[] { allValuesStr }).Any())
            {
                if (cauMovs.Count == 1)
                {
                    stbQuery.AppendLine("  AND Movimenti.cau_mov = @cau_mov ");
                    parSql.Add("@cau_mov", cauMovs.First());
                }
                else
                {
                    stbQuery.AppendLine("  AND Movimenti.cau_mov IN (@cau_movs) ");
                    parSqlIn.Add("@cau_movs", FormatClauseIn(cauMovs.ToList()));
                }
            }

            stbQuery.AppendLine("  AND Movimenti.Inviato >= 0 ");

            if (filtroSemineImpianti != null && filtroSemineImpianti.Count > 0)
            {
                stbQuery.AppendLine("DROP TABLE #FiltroSemine;");
            }

            //stbQuery.AppendLine("ORDER BY  movimenti.Data_Movimento desc");
            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiImpiantiAsync(List<int> Id_Agenda,
                                                        AgronicaCoreParametriServer objParametriServer, 
                                                        bool createTempEsercizi = false, 
                                                        bool leggiFlagEsercizioChiuso = false,
                                                        // NUOVI PARAMETRI OPZIONALI 
                                                        bool leggiIndirizzoAppezzamento = false,
                                                        bool leggiGIS = false,
                                                        bool leggiDettagli_Tecnici = false,
                                                        List<int>? Id_Esercizi = null,
                                                        List<int>? lavCods = null
                                                        )
                                                                              
        {
            const int allValues = 0;

            if (Id_Agenda is null && Id_Esercizi is null)
                throw new ArgumentNullException("Id_Agenda");

            if (Id_Agenda != null && !Id_Agenda.Any() && Id_Esercizi != null && !Id_Esercizi.Any())
                throw new ArgumentException("La lista di id agenda non contiene nessun elemento");

            if ((Id_Agenda is null ||  !Id_Agenda.Any()) &&
                (Id_Esercizi is null || !Id_Esercizi.Any() || lavCods is null || !lavCods.Any()))
                throw new ArgumentException("La lista di id esercizi o di lav cod non contiene nessun elemento");

            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            DataTable dt;
            try
            {
                var strSql = new StringBuilder() { Length = 0 };
                AddQueryForDroppingAllTemporaryTables(strSql);

                if (createTempEsercizi)
                {
                    AddQueryForCreatingTempEsercizi(strSql);
                }

                await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql, parSqlIn);

                strSql = new StringBuilder() { Length = 0 };

                //TEMP TABLES
                AddQueryForCreatingTemporaryTables(strSql);

                if (createTempEsercizi)
                {
                    strSql.AppendLine(" INSERT INTO ");
                    strSql.AppendLine("        #ImpiantiDaAgenda (Piva, Sa_Cod, Appezza, Id_Reg)");
                    strSql.AppendLine(" SELECT DISTINCT ");
                    strSql.AppendLine("        md.Piva, md.Sa_Cod, md.Appezza, md.Id_Destinazione");
                    strSql.AppendLine(" FROM ");
                    strSql.AppendLine("        Mov_Destinazioni md");
                    strSql.AppendLine("    WHERE md.Id_Agenda IN ( @Id_Agendas )");
                    strSql.AppendLine($"          AND md.Tipo_Destinazione = {(int)TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_IMPIANTO};");
                }

                //Query
                strSql.AppendLine("SELECT DISTINCT");
                strSql.AppendLine("  Mov_Destinazioni.Piva,");
                strSql.AppendLine("  Mov_Destinazioni.Sa_Cod,");
                strSql.AppendLine("  Mov_Destinazioni.Appezza,");
                strSql.AppendLine("  Mov_Destinazioni.Id_Destinazione,");
                strSql.AppendLine("  Mov_Destinazioni.Tipo_Destinazione AS Id_Reg,");
                strSql.AppendLine("  Mov_Destinazioni.Id_Agenda,");
                strSql.AppendLine("  Mov_Destinazioni.Qta2 AS sup_trattata,"); //sup trattata
                strSql.AppendLine("  Mov_Destinazioni.Qta,");
                strSql.AppendLine("  Mov_Destinazioni.Id_Mov,");
                strSql.AppendLine("  Mov_Destinazioni.Validita_Inizio AS Validita_Inizio_Mov_Destinazione,");
                strSql.AppendLine("  Movimenti.Cau_Mov,");


                strSql.AppendLine("  ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,");
                strSql.AppendLine("  ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod,");
                strSql.AppendLine("  ISNULL(Cultivar.Cul_Des,'') AS Cul_Des,");
                strSql.AppendLine("  ISNULL(Cultivar.Cul_Cod,0) AS Cul_Cod,");
                strSql.AppendLine("  ISNULL(Reg_Impianti.foral_cod,0) AS forma_allevamento,");
                strSql.AppendLine("  ISNULL(Reg_Impianti.Cop_Cod,0) AS copertura,");

                strSql.AppendLine("  Appezzamento.APP_NOME,");
                strSql.AppendLine("  Appezzamento.SUP_APP,");

                //  QUESTE COLONNE MANCAVANO (Indirizzi) 
                // Prendo l'indirizzo e i dati geografici dalla tabella Indirizzi
                if (leggiIndirizzoAppezzamento)
                {
                    strSql.AppendLine("  ISNULL(Indirizzi.ind_des, '') AS Indirizzo,");
                    strSql.AppendLine("  ISNULL(Indirizzi.com_des, '') AS Comune,");
                    strSql.AppendLine("  ISNULL(Indirizzi.pro_cod, '') AS Provincia,");
                    // Concateno Prov+Comune per Istat completo (es 001 + 272)
                    strSql.AppendLine("  ISNULL(Indirizzi.pro_cod_istat, '') + ISNULL(Indirizzi.com_cod_istat, '') AS Codice_Istat,");
                }
                //  QUESTA COLONNA MANCAVA (WKT Poligono) ---
                if (leggiGIS)
                {
                    strSql.AppendLine("  GIS_ElementiGrafici.Poligono_GeoEntity_WKT AS WKT,");
                }

                strSql.AppendLine("  Reg_Impianti.sup_imp,");
                strSql.AppendLine("  Reg_Impianti.DATA_RACCOLTA,");
                strSql.AppendLine("  Reg_Impianti.Validita_Inizio AS validita_inizio_impianto,");
                strSql.AppendLine("  ISNULL(Reg_Impianti.grfi_cod,0) AS grfi_cod,");
                strSql.AppendLine("  GruppoFinalita.Grfi_Des,");
                strSql.AppendLine("  ISNULL(GruppoFinalita_1.Grfi_Des,'') AS Stato,");
                strSql.AppendLine("  Imprese_Progetti.progetto_cod,");
                strSql.AppendLine("  Imprese_Progetti.Progetto_Nome,");
                strSql.AppendLine("  Imprese_Progetti.Stato_Impianto,");
                strSql.AppendLine("  ISNULL(Imprese_Progetti.Regolamento_Cod,1) AS Regolamento_Cod,");
                strSql.AppendLine("  ISNULL(Imprese_Progetti.Disciplinare_Cod,0) AS Disciplinare_Cod,");
                strSql.AppendLine("  ISNULL(Imprese_Progetti.Disciplinare_PubblicoPrivato,0) AS Disciplinare_PubblicoPrivato,");
                strSql.AppendLine("  ISNULL(Imprese_Progetti.Regolamento_Concimazioni_Cod,0) AS Regolamento_Concimazioni_Cod,");
                strSql.AppendLine("  Imprese_Progetti.validita_inizio AS validita_inizio_esercizio,");
                strSql.AppendLine("  Imprese_Progetti.validita_fine AS validita_fine_esercizio,");
                strSql.AppendLine("  Imprese_Progetti.data_fioritura_prevista,");
                strSql.AppendLine("  Imprese_Progetti.data_fine_prevista AS data_raccolta_prevista,");
                strSql.AppendLine("  ISNULL(sup_riduzione_bufferzone,0) as sup_riduzione_bufferzone,");
                strSql.AppendLine("  ISNULL(perc_riduzione_deriva,0) as perc_riduzione_deriva,");
                strSql.AppendLine("  ISNULL(SupBZ_Riduzione,0) as SupBZ_Riduzione,");
                strSql.AppendLine("  ISNULL(DistBZ_CorpiIdrici,0) as DistBZ_CorpiIdrici,");
                strSql.AppendLine("  ISNULL(DistBZ_AreeResPub,0) as DistBZ_AreeResPub,");
                strSql.AppendLine("  ISNULL(DistBZ_Allevamenti,0) as DistBZ_Allevamenti,");
                strSql.AppendLine("  ISNULL(DistBZ_VegNatNonColt,0) as DistBZ_VegNatNonColt,");

                strSql.AppendLine("  ISNULL(N_Massimo.val_cod,NULL) as N_Massimo, ISNULL(P_Massimo.val_cod,NULL) as P_Massimo, ISNULL(K_Massimo.val_cod,NULL) as K_Massimo, ISNULL(Mg_Massimo.val_cod,NULL) as Mg_Massimo,");
                strSql.AppendLine("  ISNULL(Imprese_Progetti.Produzione_Prevista, 0) as Produzione_Prevista,");
                strSql.AppendLine("  ISNULL(Elem_Cod, 0) AS Elem_Cod,");
                strSql.AppendLine("  ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod,");
                strSql.AppendLine("  Lav_Cod, Data_Movimento");

                if (leggiFlagEsercizioChiuso)
                {
                    strSql.AppendLine(" , ISNULL(ric_flag_esercizio_chiuso.val_cod, '0') as Flag_Esercizio_Chiuso");
                }

                if (leggiDettagli_Tecnici)
                {
                    strSql.AppendLine(", Mov_Dettaglio_Tecnico.FF_Classe");
                }

                strSql.AppendLine("FROM Mov_Destinazioni");

                strSql.AppendLine("INNER JOIN Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND");
                strSql.AppendLine("    Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG");

                strSql.AppendLine("LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod");

                strSql.AppendLine("LEFT OUTER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod");

                strSql.AppendLine("INNER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND");
                strSql.AppendLine("    Reg_Impianti.APPEZZA = Appezzamento.APPEZZA");

                //  QUESTE JOIN PER GLI INDIRIZZI 
                if (leggiIndirizzoAppezzamento)
                {
                    strSql.AppendLine("LEFT OUTER JOIN AppezzamentixIndirizzi ON Reg_Impianti.PIVA = AppezzamentixIndirizzi.PIVA AND Reg_Impianti.SA_COD = AppezzamentixIndirizzi.sa_cod AND Reg_Impianti.APPEZZA = AppezzamentixIndirizzi.appezza");
                    strSql.AppendLine("LEFT OUTER JOIN Indirizzi ON AppezzamentixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo");
                }

                //  QUESTE JOIN (Collegamento GIS) 
                if (leggiGIS)
                {
                    // Collega l'Impianto (ID_REG) alla tabella GIS_Entita (Id_Imp)
                    strSql.AppendLine("LEFT OUTER JOIN GIS_Entita ON Reg_Impianti.PIVA = GIS_Entita.Piva AND Reg_Impianti.SA_COD = GIS_Entita.Sa_Cod AND Reg_Impianti.APPEZZA = GIS_Entita.Appezza");
                    strSql.AppendLine("     AND Reg_Impianti.ID_REG = GIS_Entita.Id_Imp");
                    // Collega GIS_Entita alla tabella che contiene il WKT
                    strSql.AppendLine("LEFT OUTER JOIN GIS_ElementiGrafici ON GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod");
                }
                // ---------------------------------------------------

                strSql.AppendLine("INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND");
                strSql.AppendLine("    Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg");

                if (leggiFlagEsercizioChiuso)
                {
                    strSql.AppendLine("LEFT JOIN Reg_Impianti_Codici ric_flag_esercizio_chiuso ON ric_flag_esercizio_chiuso.PIVA = Imprese_Progetti.Piva AND ric_flag_esercizio_chiuso.SA_COD = Imprese_Progetti.Sa_Cod AND");
                    strSql.AppendLine("    ric_flag_esercizio_chiuso.APPEZZA = Imprese_Progetti.Appezza AND ric_flag_esercizio_chiuso.ID_REG = Imprese_Progetti.Id_Reg AND ric_flag_esercizio_chiuso.Progetto_Cod = Imprese_Progetti.Progetto_Cod");
                    strSql.AppendLine(" AND ric_flag_esercizio_chiuso.Id_Cod = @flag_esercizio_chiuso");
                    parametriSql.Add("@flag_esercizio_chiuso", (int)Enum_CodiciAnagrafe.Distinta_Chiusa);
                }

                strSql.AppendLine(" INNER JOIN Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ");
                strSql.AppendLine("     Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Destinazioni.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det");

                strSql.AppendLine("INNER JOIN Movimenti ON Mov_Destinazioni.PIVA = Movimenti.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti.Sa_Cod AND");
                strSql.AppendLine("    Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda And Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov");

                strSql.AppendLine("INNER JOIN Agenda ON Movimenti.Id_Agenda = Agenda.Id_Agenda AND Movimenti.PIVA = Agenda.PIVA");

                strSql.AppendLine("LEFT OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod");

                strSql.AppendLine("LEFT OUTER JOIN GruppoFinalita AS GruppoFinalita_1 ON Imprese_Progetti.Stato_Impianto = GruppoFinalita_1.Grfi_Cod");

                strSql.AppendLine("LEFT OUTER JOIN #N_Massimo N_Massimo ON N_Massimo.Piva = Reg_Impianti.PIVA AND N_Massimo.Sa_Cod = Reg_Impianti.SA_COD AND N_Massimo.Appezza = Reg_Impianti.APPEZZA AND N_Massimo.Id_Reg = Reg_Impianti.ID_REG");
                strSql.AppendLine("LEFT OUTER JOIN #P_Massimo P_Massimo ON P_Massimo.Piva = Reg_Impianti.PIVA AND P_Massimo.Sa_Cod = Reg_Impianti.SA_COD AND P_Massimo.Appezza = Reg_Impianti.APPEZZA AND P_Massimo.Id_Reg = Reg_Impianti.ID_REG");
                strSql.AppendLine("LEFT OUTER JOIN #K_Massimo K_Massimo ON K_Massimo.Piva = Reg_Impianti.PIVA AND K_Massimo.Sa_Cod = Reg_Impianti.SA_COD AND K_Massimo.Appezza = Reg_Impianti.APPEZZA AND K_Massimo.Id_Reg = Reg_Impianti.ID_REG");
                strSql.AppendLine("LEFT OUTER JOIN #Mg_Massimo Mg_Massimo ON Mg_Massimo.Piva = Reg_Impianti.PIVA AND Mg_Massimo.Sa_Cod = Reg_Impianti.SA_COD AND Mg_Massimo.Appezza = Reg_Impianti.APPEZZA AND Mg_Massimo.Id_Reg = Reg_Impianti.ID_REG");

                if (leggiDettagli_Tecnici)
                {
                    strSql.AppendLine("LEFT JOIN Mov_Dettaglio_Tecnico ON Mov_Destinazioni.PIVA = Mov_Dettaglio_Tecnico.PIVA AND Mov_Destinazioni.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND");
                    strSql.AppendLine("Mov_Destinazioni.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda And Mov_Destinazioni.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov And Mov_Destinazioni.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det");
                }

                strSql.AppendLine("WHERE 1=1");

                if (Id_Agenda is not null && Id_Agenda.Any())
                {
                    strSql.AppendLine(" AND Mov_Destinazioni.id_agenda IN (@Id_Agendas)");
                    parSqlIn.Add("@Id_Agendas", FormatClauseIn(Id_Agenda));
                }

                strSql.AppendLine("    AND Imprese_Progetti.Validita_Inizio <= Movimenti.Data_Movimento");
                strSql.AppendLine("    AND Imprese_Progetti.Validita_fine >= Movimenti.Data_Movimento");

                if (Id_Esercizi is not null && Id_Esercizi.Any())
                {
                    strSql.AppendLine(" AND Imprese_Progetti.Progetto_Cod IN (@Id_Esercizi)");
                    parSqlIn.Add("@Id_Esercizi", FormatClauseIn(Id_Esercizi));
                }

                if (lavCods != null && lavCods.Except(new[] { allValues }).Any())
                {
                    if (lavCods.Count == 1)
                    {
                        strSql.AppendLine("  AND Agenda.lav_cod = @lav_cod ");
                        parametriSql.Add("@lav_cod", lavCods.First());
                    }
                    else
                    {
                        strSql.AppendLine("  AND Agenda.lav_cod IN (@lav_cods) ");
                        parSqlIn.Add("@lav_cods", FormatClauseIn(lavCods.ToList()));
                    }
                }

                strSql.AppendLine("ORDER BY Mov_Destinazioni.Id_Agenda;");

                //insert into tabella temp 
                //strSql.AppendLine("SELECT DISTINCT");
                //strSql.AppendLine("  Mov_Destinazioni.Piva,");
                //strSql.AppendLine("  Mov_Destinazioni.Sa_Cod,");
                //strSql.AppendLine("  Mov_Destinazioni.Appezza,");
                //strSql.AppendLine("  Mov_Destinazioni.Id_Destinazione,");
                //strSql.AppendLine("  Imprese_Progetti.Progetto_cod,");
                // INTO TempImpianto
                // JOIN Imprese_Progetti


                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }

        public async Task<DataTable> LeggiProdottiAsync(List<int> idAgendas, bool leggiMateriePrime, bool leggiEsercizi, AgronicaCoreParametriServer objParametriServer, bool estraiSpecieECultivar = true,
                                                        bool leggiFlagEsercizioChiuso = false, List<int>? Id_Esercizi = null, List<int>? lavCods = null)
        {
            var parametriSql = new Dictionary<string, object>();
            const int allValues = 0;
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            DataTable dt;
            try
            {
                var strSql = new StringBuilder();
                strSql.Length = 0;

                strSql.AppendLine("SELECT DISTINCT ");
                strSql.AppendLine("    Mov_Destinazioni.Id_Agenda,");
                strSql.AppendLine("    Movimenti_dettagli.Qta AS Qta_Dett,-- qta ettaro");

                strSql.AppendLine("    Movimenti_dettagli.Elem_Cod,");
                strSql.AppendLine("    Movimenti_dettagli.Pro_Cod,");
                strSql.AppendLine("    Movimenti_dettagli.Mat_Cod,");
                strSql.AppendLine("    COALESCE(Fertilizzanti.Fer_Des, '') AS Fer_Des, ");
                strSql.AppendLine("    COALESCE(Formulati.Fr_Des, '') AS Fr_Des, ");
                strSql.AppendLine("    COALESCE(InsettiUtili.Ins_Des, '') AS Ins_Des, ");
                // The following is an alternative approach to the function STRING_AGG
                strSql.AppendLine("    ISNULL(STUFF(( ");
                strSql.AppendLine("        SELECT ', ' + Pa_Des FROM PrincipiAttivi ");
                strSql.AppendLine("        LEFT JOIN FormulatixPrincipiAttivi ON FormulatixPrincipiAttivi.pa_cod = PrincipiAttivi.pa_cod ");
                strSql.AppendLine("        WHERE FormulatixPrincipiAttivi.Fr_Cod = Formulati.Fr_Cod ");
                strSql.AppendLine("        FOR XML PATH('') ");
                strSql.AppendLine("    ), 1, 2, ''), '') AS Pa_Des, ");

                strSql.AppendLine("    Movimenti_dettagli.Udm_Cod,");
                strSql.AppendLine("    Movimenti_dettagli.extra_int,");
                strSql.AppendLine("    Movimenti_dettagli.Lotto, --?");
                strSql.AppendLine("    Movimenti_dettagli.doseetichetta_value,");
                strSql.AppendLine("    Movimenti_dettagli.extra_str,");
                strSql.AppendLine("    COALESCE(Movimenti_dettagli.PrincipiAttiviPercAbb, '') AS PrincipiAttiviPercAbb,");
                strSql.AppendLine("    COALESCE(Movimenti_dettagli.PrincipiAttivi, '') AS PrincipiAttivi,");
                strSql.AppendLine("    COALESCE(Movimenti_dettagli.Polverulento, 0) AS Polverulento, ");

                strSql.AppendLine("    CASE ");
                strSql.AppendLine("      WHEN Mov_Dettaglio_Tecnico.av_cod > 0 THEN COALESCE(Avversita.Av_Des_Vol, '') ");
                strSql.AppendLine("      WHEN Mov_Dettaglio_Tecnico.av_gru > 0 THEN COALESCE(GruppoAvversita.Av_Gru_Des COLLATE Latin1_General_CI_AS, '') ");
                strSql.AppendLine("      ELSE '' ");
                strSql.AppendLine("    END AS av_des_vol, ");
                strSql.AppendLine("    ISNULL(Mov_Dettaglio_Tecnico.av_cod, 0) AS av_cod,");
                strSql.AppendLine("    ISNULL(Mov_Dettaglio_Tecnico.av_gru, 0) AS av_gru,");
                strSql.AppendLine("    ISNULL(Mov_Dettaglio_Tecnico.soglia_cod, 0) AS soglia_cod,");

                if (estraiSpecieECultivar)
                {
                    strSql.AppendLine("    COALESCE(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ");
                    strSql.AppendLine("    COALESCE(SpecieVegetali.Veg_Des, '') AS Veg_Des, ");
                    strSql.AppendLine("    COALESCE(Cultivar.Cul_Des, '') AS Cul_Des, ");
                }

                //Fertilizzazioni
                strSql.AppendLine(" isnull(Mov_Dettaglio_Tecnico.n,0) as n");
                strSql.AppendLine(", isnull(Mov_Dettaglio_Tecnico.p,0) as p");
                strSql.AppendLine(", isnull(Mov_Dettaglio_Tecnico.k,0) as k");
                strSql.AppendLine(", isnull(Mov_Dettaglio_Tecnico.mg,0) as mg");
                strSql.AppendLine(",  isnull(Mov_Dettaglio_Tecnico.cu,0) as cu");
                strSql.AppendLine(", isnull(Mov_Dettaglio_Tecnico.efficienza,0) as efficienza");

                if (leggiEsercizi)
                {
                    strSql.AppendLine(" ,ISNULL(Imprese_Progetti.Progetto_Nome, '') as Progetto_Nome");
                    strSql.AppendLine(" ,Imprese_Progetti.Progetto_Cod ");
                    strSql.AppendLine(" ,Mov_Destinazioni.Qta2 AS ha_trattati");
                    strSql.AppendLine(" ,ISNULL(Appezzamento.APP_NOME, '') AS APP_NOME");
                    strSql.AppendLine(" ,ISNULL(Reg_Impianti.sup_imp, 0) AS sup_imp");
                    strSql.AppendLine(" ,Mov_Destinazioni.Sa_Cod");
                    strSql.AppendLine(" ,Mov_Destinazioni.Appezza");
                }

                if (leggiMateriePrime)
                {
                    strSql.AppendLine(" ,ISNULL(Materie_Prime.Mat_Des, '') as Mat_Des");
                    strSql.AppendLine(" ,ISNULL(Materie_prime.Veg_Cod, 0) as Veg_Cod");
                }
                else
                {
                    strSql.AppendLine(", '' as Mat_Des");
                    strSql.AppendLine(", 0 as Veg_Cod");
                }

                if (leggiFlagEsercizioChiuso)
                {
                    strSql.AppendLine(" , ISNULL(ric_flag_esercizio_chiuso.val_cod, '0') as Flag_Esercizio_Chiuso");
                }

                strSql.AppendLine("FROM Mov_Destinazioni ");

                strSql.AppendLine("INNER JOIN Movimenti_dettagli ");
                strSql.AppendLine("    ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA ");
                strSql.AppendLine("    AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ");
                strSql.AppendLine("    AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov ");
                strSql.AppendLine("    AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ");

                strSql.AppendLine("INNER JOIN Reg_Impianti ");
                strSql.AppendLine("    ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA ");
                strSql.AppendLine("    AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD ");
                strSql.AppendLine("    AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA ");
                strSql.AppendLine("    AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG ");

                if (estraiSpecieECultivar)
                {
                    strSql.AppendLine("LEFT OUTER JOIN Cultivar ");
                    strSql.AppendLine("    ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ");

                    strSql.AppendLine("LEFT OUTER JOIN SpecieVegetali ");
                    strSql.AppendLine("    ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ");
                }

                strSql.AppendLine("INNER JOIN Appezzamento ");
                strSql.AppendLine("    ON Reg_Impianti.PIVA = Appezzamento.PIVA ");
                strSql.AppendLine("    AND Reg_Impianti.SA_COD = Appezzamento.SA_COD ");
                strSql.AppendLine("    AND Reg_Impianti.APPEZZA = Appezzamento.APPEZZA ");

                if (leggiEsercizi)
                {

                    strSql.AppendLine("INNER JOIN Imprese_Progetti ");
                    strSql.AppendLine("    ON Reg_Impianti.PIVA = Imprese_Progetti.Piva ");
                    strSql.AppendLine("    AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod ");
                    strSql.AppendLine("    AND Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza ");
                    strSql.AppendLine("    AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg ");
                }

                if (leggiFlagEsercizioChiuso)
                {
                    strSql.AppendLine("LEFT JOIN Reg_Impianti_Codici ric_flag_esercizio_chiuso ON ric_flag_esercizio_chiuso.PIVA = Imprese_Progetti.Piva AND ric_flag_esercizio_chiuso.SA_COD = Imprese_Progetti.Sa_Cod AND");
                    strSql.AppendLine("    ric_flag_esercizio_chiuso.APPEZZA = Imprese_Progetti.Appezza AND ric_flag_esercizio_chiuso.ID_REG = Imprese_Progetti.Id_Reg AND ric_flag_esercizio_chiuso.Progetto_Cod = Imprese_Progetti.Progetto_Cod");
                    strSql.AppendLine(" AND ric_flag_esercizio_chiuso.Id_Cod = @flag_esercizio_chiuso");
                    parametriSql.Add("@flag_esercizio_chiuso", (int)Enum_CodiciAnagrafe.Distinta_Chiusa);
                }

                strSql.AppendLine("INNER JOIN Movimenti ");
                strSql.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti.PIVA ");
                strSql.AppendLine("    AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ");

                strSql.AppendLine("LEFT OUTER JOIN Mov_Dettaglio_Tecnico ");
                strSql.AppendLine("    ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva ");
                strSql.AppendLine("    AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ");

                if (leggiMateriePrime)
                {
                    strSql.AppendLine("INNER JOIN Materie_prime ");
                    strSql.AppendLine("    ON Movimenti_dettagli.Elem_cod = Materie_prime.Elem_cod ");
                    strSql.AppendLine("    AND Movimenti_dettagli.Mat_Cod = Materie_prime.Mat_Cod ");
                }

                strSql.AppendLine("LEFT JOIN Fertilizzanti ");
                strSql.AppendLine("    ON Fertilizzanti.Fer_Cod = Movimenti_Dettagli.Pro_Cod");
                strSql.AppendLine("LEFT JOIN Formulati ");
                strSql.AppendLine("    ON Formulati.Fr_Cod = Movimenti_Dettagli.Pro_Cod");
                strSql.AppendLine("LEFT JOIN Avversita ");
                strSql.AppendLine("    ON Avversita.Av_Cod = Mov_Dettaglio_Tecnico.Av_Cod");
                strSql.AppendLine("LEFT JOIN GruppoAvversita ");
                strSql.AppendLine("    ON GruppoAvversita.Av_Gru = Mov_Dettaglio_Tecnico.Av_Gru");
                strSql.AppendLine("LEFT OUTER JOIN InsettiUtili");
                strSql.AppendLine("    ON InsettiUtili.Ins_Cod = Movimenti_Dettagli.Pro_Cod");

                strSql.AppendLine("INNER JOIN Agenda ");
                strSql.AppendLine("    ON Movimenti.PIVA = Agenda.PIVA");
                strSql.AppendLine("    AND Movimenti.Id_Agenda = Agenda.Id_Agenda ");
 
                strSql.AppendLine("WHERE 1 = 1");
                strSql.AppendLine(" AND Movimenti_dettagli.Elem_Cod <> 200"); //acqua per le fertirrigazioni

                if (idAgendas is not null && idAgendas.Except(new[] { allValues }).Any())
                {
                    if (idAgendas.Count == 1)
                    {
                        strSql.AppendLine("AND Mov_Destinazioni.id_agenda = @id_Agenda");
                        parametriSql.Add("@id_agenda", idAgendas.First());
                    }
                    else
                    {
                        strSql.AppendLine("AND Mov_Destinazioni.id_agenda IN (@Id_Agendas)");
                        parSqlIn.Add("@Id_Agendas", FormatClauseIn(idAgendas));
                    }
                }

                if (leggiEsercizi)
                {
                    strSql.AppendLine("AND Imprese_Progetti.Validita_Inizio <= Movimenti.Data_Movimento");
                    strSql.AppendLine("AND Imprese_Progetti.Validita_fine >= Movimenti.Data_Movimento");
                }

                if (leggiMateriePrime)
                {
                    strSql.AppendLine("AND Movimenti_Dettagli.Mat_Cod != 0");
                }

                if (Id_Esercizi is not null && Id_Esercizi.Any())
                {
                    strSql.AppendLine(" AND Imprese_Progetti.Progetto_Cod IN (@Id_Esercizi)");
                    parSqlIn.Add("@Id_Esercizi", FormatClauseIn(Id_Esercizi));
                }

                if (lavCods != null && lavCods.Except(new[] { allValues }).Any())
                {
                    if (lavCods.Count == 1)
                    {
                        strSql.AppendLine("  AND Agenda.lav_cod = @lav_cod ");
                        parametriSql.Add("@lav_cod", lavCods.First());
                    }
                    else
                    {
                        strSql.AppendLine("  AND Agenda.lav_cod IN (@lav_cods) ");
                        parSqlIn.Add("@lav_cods", FormatClauseIn(lavCods.ToList()));
                    }
                }

                strSql.AppendLine("ORDER BY Mov_Destinazioni.Id_Agenda;");

                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        /// <summary>Metodo nato per la lettura dei prodotti applicati nelle operazioni di concia del seme e trattamento post raccolta</summary>
        /// <param name="idAgendas">Lista di id agenda da considerare</param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        public async Task<DataTable> LeggiProdottiOperazioniSenzaImpiantoAsync(List<int> idAgendas, AgronicaCoreParametriServer objParametriServer)
        {
            if (idAgendas is null)
                throw new ArgumentNullException("idAgendas");

            if (!idAgendas.Any())
                throw new ArgumentException("La lista di id agenda non contiene nessun elemento");

            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            var parSql = new Dictionary<string, object>();

            DataTable dt;
            var sb = new StringBuilder();

            List<string> cauMovs = new() {
                CAU_MOV.CAU_TRATTAMENTO, CAU_MOV.CAU_RILIEVO_CAMPO,
                CAU_MOV.CAU_RILIEVO_RACCOLTA, CAU_MOV.CAU_LAVORAZIONE
            };

            List<int> lavCods = new()
            {
                LAV_COD.LAVCOD_CONCIA_SEME,
                LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA
            };

            sb.AppendLine(" SELECT DISTINCT ");
            sb.AppendLine("     a.Id_Agenda, mdet.Qta as Qta_Dett, --qta ettaro ");

            sb.AppendLine("     mdet.Elem_Cod, ");
            sb.AppendLine("     mdet.Pro_Cod, ");
            sb.AppendLine("     mdet.Mat_Cod, ");
            sb.AppendLine("     COALESCE(fer.Fer_Des, '') AS Fer_Des, ");
            sb.AppendLine("     COALESCE(form.Fr_Des, '') AS Fr_Des, ");
            sb.AppendLine("     COALESCE(iu.Ins_Des, '') AS Ins_Des, ");
            // The following is an alternative approach to the function STRING_AGG
            sb.AppendLine("     STUFF(( ");
            sb.AppendLine("         SELECT ");
            sb.AppendLine("             ', ' + Pa_Des ");
            sb.AppendLine("         FROM ");
            sb.AppendLine("             PrincipiAttivi ");
            sb.AppendLine("         LEFT JOIN ");
            sb.AppendLine("             FormulatixPrincipiAttivi ");
            sb.AppendLine("             ON FormulatixPrincipiAttivi.pa_cod = PrincipiAttivi.pa_cod ");
            sb.AppendLine("         WHERE ");
            sb.AppendLine("             FormulatixPrincipiAttivi.Fr_Cod = form.Fr_Cod ");
            sb.AppendLine("         FOR XML PATH('') ");
            sb.AppendLine("     ), 1, 2, '') AS Pa_Des, ");

            sb.AppendLine("     mdet.Udm_Cod, ");
            sb.AppendLine("     mdet.extra_int, ");
            sb.AppendLine("     mdet.Lotto,");
            sb.AppendLine("     mdet.doseetichetta_value, ");
            sb.AppendLine("     mdet.extra_str, ");
            sb.AppendLine("     COALESCE(mdet.PrincipiAttiviPercAbb, '') AS PrincipiAttiviPercAbb, ");
            sb.AppendLine("     COALESCE(mdet.Polverulento, 0) AS Polverulento, ");
            
            sb.AppendLine("     COALESCE(av.Av_Des_Vol, '') AS av_des_vol, ");
            sb.AppendLine("     ISNULL(mdetT.av_cod, 0) AS av_cod, ");
            sb.AppendLine("     ISNULL(mdetT.av_gru, 0) AS av_gru, ");
            sb.AppendLine("     ISNULL(mdetT.soglia_cod, 0) AS soglia_cod, ");

            sb.AppendLine("     ISNULL(mdetT.n, 0) as n, ");
            sb.AppendLine("     ISNULL(mdetT.p, 0) as p, ");
            sb.AppendLine("     ISNULL(mdetT.k, 0) as k, ");
            sb.AppendLine("     ISNULL(mdetT.mg, 0) as mg, ");
            sb.AppendLine("     ISNULL(mdetT.cu, 0) as cu, ");
            sb.AppendLine("     ISNULL(mdetT.efficienza, 0) as efficienza, ");
            sb.AppendLine("     '' as Mat_Des ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Agenda a ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("     Movimenti m ");
            sb.AppendLine("     ON m.Id_Agenda = a.Id_Agenda ");
            sb.AppendLine("     AND m.PIVA = a.PIVA ");
            sb.AppendLine("     AND m.Sa_Cod = a.Sa_Cod ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("     Movimenti_dettagli mdet ");
            sb.AppendLine("     ON mdet.Id_Agenda = m.Id_Agenda ");
            sb.AppendLine("     AND mdet.Id_Mov = m.Id_Mov ");
            sb.AppendLine("     AND mdet.PIVA = m.PIVA ");
            sb.AppendLine("     AND mdet.Sa_Cod = m.Sa_Cod ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("     Mov_Dettaglio_Tecnico mdetT ");
            sb.AppendLine("     ON mdetT.Id_Agenda = mdet.Id_Agenda ");
            sb.AppendLine("     AND mdetT.Id_Mov = mdet.Id_Mov ");
            sb.AppendLine("     AND mdetT.Id_Mov_Det = mdet.Id_Mov_Det ");
            sb.AppendLine("     AND mdetT.Piva = mdet.PIVA ");
            sb.AppendLine("     AND mdet.Sa_Cod = mdet.Sa_Cod ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Fertilizzanti fer");
            sb.AppendLine("     ON fer.Fer_Cod = mdet.Pro_Cod ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Formulati form ");
            sb.AppendLine("     ON form.Fr_Cod = mdet.Pro_Cod ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Avversita av ");
            sb.AppendLine("     ON av.Av_Cod = mdetT.Av_Cod ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     InsettiUtili iu ");
            sb.AppendLine("     ON iu.Ins_Cod = mdet.Pro_Cod ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     m.Cau_Mov IN (@cauMov) ");
            sb.AppendLine("     AND a.Id_Agenda IN (@idAgenda) ");
            sb.AppendLine("     AND a.Lav_Cod IN (@lavCod) ");
            sb.AppendLine("     AND mdet.Pro_Cod > 0 "); //per trovare effettivamente la riga del prodotto, non sono interessato al semente che è stato trattato

            parSqlIn.Add("@cauMov", FormatClauseIn(cauMovs));
            parSqlIn.Add("@idAgenda", FormatClauseIn(idAgendas));
            parSqlIn.Add("@lavCod", FormatClauseIn(lavCods));

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(sb.ToString(), parSql, parSqlIn);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiProdottiAgendaNoQdCAAsync(List<int>? idAgendas, List<int>? lavCods, List<string>? cauMovs, List<int>? elemCods, bool leggiMateriePrime, AgronicaCoreParametriServer objParametriServer)
        {
            var parametriSql = new Dictionary<string, object>();
            const int allValues = 0;
            const string allValuesStr = ""; 
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            DataTable dt;
            try
            {
                var strSql = new StringBuilder();
                strSql.Length = 0;

                strSql.AppendLine("SELECT DISTINCT ");
                strSql.AppendLine("    Movimenti_dettagli.Id_Agenda,");
                strSql.AppendLine("    Movimenti_dettagli.Qta Qta_Dett, ");
                strSql.AppendLine("    Movimenti_dettagli.Elem_Cod,");
                strSql.AppendLine("    Movimenti_dettagli.Pro_Cod,");
                strSql.AppendLine("    Movimenti_dettagli.Mat_Cod,");
                strSql.AppendLine("    Movimenti_dettagli.Udm_Cod,");
                strSql.AppendLine("    Movimenti_dettagli.extra_int,");
                strSql.AppendLine("    Movimenti_dettagli.Lotto, ");
                strSql.AppendLine("    Movimenti_dettagli.extra_str ");
                if (leggiMateriePrime)
                {
                    strSql.AppendLine(" ,Materie_Prime.Mat_Des");
                }
                strSql.AppendLine("FROM Movimenti_dettagli ");
                 
                strSql.AppendLine("INNER JOIN Movimenti ");
                strSql.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti.PIVA ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ");
                strSql.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ");

                if(leggiMateriePrime)
                {
                        strSql.AppendLine("LEFT JOIN Materie_prime ");
                        strSql.AppendLine("    ON Movimenti_dettagli.Elem_cod = Materie_prime.Elem_cod ");
                        strSql.AppendLine("    AND Movimenti_dettagli.Mat_Cod = Materie_prime.Mat_Cod ");
                }

                if (idAgendas != null && idAgendas.Except(new[] { allValues }).Any())
                {
                    if (idAgendas.Count == 1)
                    {
                        strSql.AppendLine("WHERE Movimenti_dettagli.id_agenda = @id_Agenda");
                        parametriSql.Add("@id_agenda", idAgendas.First());
                    }
                    else
                    {
                        strSql.AppendLine("WHERE Movimenti_dettagli.id_agenda IN (@Id_Agendas)");
                        parSqlIn.Add("@id_agendas", FormatClauseIn(idAgendas.ToList()));
                    }
                }

                if (lavCods != null && lavCods.Except(new[] { allValues }).Any())
                {
                    if (lavCods.Count == 1)
                    {
                        strSql.AppendLine("  AND Agenda.lav_cod = @lav_cod ");
                        parametriSql.Add("@lav_cod", lavCods.First());
                    }
                    else
                    {
                        strSql.AppendLine("  AND Agenda.lav_cod IN (@lav_cods) ");
                        parSqlIn.Add("@lav_cods", FormatClauseIn(lavCods.ToList()));
                    }
                }

                if (cauMovs != null && cauMovs.Except(new[] { allValuesStr }).Any())
                {
                    if (cauMovs.Count == 1)
                    {
                        strSql.AppendLine("  AND Movimenti.cau_mov = @cau_mov ");
                        parametriSql.Add("@cau_mov", cauMovs.First());
                    }
                    else
                    {
                        strSql.AppendLine("  AND Movimenti.cau_mov IN (@cau_movs) ");
                        parSqlIn.Add("@cau_movs", FormatClauseIn(cauMovs.ToList()));
                    }
                }

                if (elemCods != null && elemCods.Except(new[] { allValues }).Any())
                {
                    if (elemCods.Count == 1)
                    {
                        strSql.AppendLine("  AND Movimenti_Dettagli.elem_cod = @elem_cod ");
                        parametriSql.Add("@elem_cod", elemCods.First());
                    }
                    else
                    {
                        strSql.AppendLine("  AND Movimenti_Dettagli.elem_cod IN (@elem_cods) ");
                        parSqlIn.Add("@elem_cods", FormatClauseIn(elemCods.ToList()));
                    }
                }

                strSql.AppendLine(" AND Movimenti_dettagli.jolly_int = @jolly_int");
                parametriSql.Add("@jolly_int", (int)enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato);

                // strSql.AppendLine("ORDER BY Movimenti_dettagli.Id_Agenda;");

                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        public async Task<DataTable> LeggiAcquaAsync(List<int> Id_Agenda,
                                                                   AgronicaCoreParametriServer objParametriServer)
        {
            if (!Id_Agenda.Any())
            {
                return new DataTable();
            }
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            stbQuery.AppendLine(" SELECT Id_Agenda, ISNULL(Mov_Dettaglio_Tecnico.qta_ril, 0) AS acqua ");
            stbQuery.AppendLine(" FROM Mov_Dettaglio_Tecnico  ");
            stbQuery.AppendLine(" WHERE Mov_Dettaglio_Tecnico.Id_Agenda IN (@Id_Agenda) ");
            stbQuery.AppendLine(" AND Mov_Dettaglio_Tecnico.qta_ril <> 0 ");
            stbQuery.AppendLine(" AND Mov_Dettaglio_Tecnico.id_mov_Det = 0 ");
            parSqlIn.Add("@Id_Agenda", FormatClauseIn(Id_Agenda));

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiRilieviAsync(List<int> Id_Agenda,
                                                       AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            if (!Id_Agenda.Any()) return new();

            stbQuery.AppendLine(" SELECT  ");
            stbQuery.AppendLine("       md.Id_Agenda, md.piva, md.sa_Cod, md.appezza, md.id_Destinazione, md.qta, mt.Qta_Ril,  ");
            stbQuery.AppendLine("       md.Data_Creazione AS Data, mt.Extra_Str, dett_cod, mt.Av_Cod, av_Gru, Avversita.av_des_vol, ff_classe ");
            stbQuery.AppendLine(" FROM  ");
            stbQuery.AppendLine("       Mov_Destinazioni md ");
            stbQuery.AppendLine(" INNER JOIN ");
            stbQuery.AppendLine("       Mov_Dettaglio_Tecnico mt");
            stbQuery.AppendLine("       ON mt.Id_Agenda = md.id_agenda ");
            stbQuery.AppendLine("     AND mt.Id_Mov = md.Id_Mov ");
            stbQuery.AppendLine("     AND mt.Id_Mov_Det = md.Id_Mov_Det ");
            stbQuery.AppendLine(" INNER JOIN ");
            stbQuery.AppendLine("     Avversita ");
            stbQuery.AppendLine("     on Avversita.Av_Cod = mt.Av_Cod ");
            stbQuery.AppendLine(" WHERE ");
            stbQuery.AppendLine("     md.Id_Agenda IN (@Id_Agenda) ");

            parSqlIn.Add("@Id_Agenda", FormatClauseIn(Id_Agenda));

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiOperazioniAsync(
            string piva, int sacod, int vegcod, IntervalloTemporale dateInterval, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            List<int> lavCods = new List<int>
            {
                LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                LAV_COD.LAVCOD_CONCIA_SEME,
                LAV_COD.LAVCOD_GEODISINFESTAZIONE,
                LAV_COD.LAVCOD_DISERBO,
                LAV_COD.LAVCOD_DISSECCAMENTO,
                LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE,
                LAV_COD.LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                LAV_COD.LAVCOD_DISTRIBUZIONE_CONCIME,
                LAV_COD.LAVCOD_FERTIRRIGAZIONE,
                LAV_COD.LAVCOD_SARCHIATURA_CONCIMAZIONE,
                LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                LAV_COD.LAVCOD_DISORIENTAMENTO_SESSUALE,
                LAV_COD.LAVCOD_CONFUSIONE_SESSUALE,
                LAV_COD.LAVCOD_RACCOLTA,
                LAV_COD.LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
                LAV_COD.LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                LAV_COD.LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
            };
            List<string> cauMovs = new() {
                CAU_MOV.CAU_TRATTAMENTO, CAU_MOV.CAU_RILIEVO_CAMPO,
                CAU_MOV.CAU_RILIEVO_RACCOLTA, CAU_MOV.CAU_LAVORAZIONE
            };

            parSql.Add("@piva", piva);
            parSql.Add("@sacod", sacod);
            parSql.Add("@vegcod", vegcod);
            parSql.Add("@inizio", dateInterval.inizio.ToShortDateString());
            parSql.Add("@fine", dateInterval.fine.ToShortDateString());

            stbQuery.AppendLine("SELECT DISTINCT ");
            stbQuery.AppendLine("  Agenda.id_agenda, Agenda.piva, Agenda.sa_cod, Centri_Aziendali.sa_nome, ");
            stbQuery.AppendLine("  movimenti.Data_Movimento, Agenda.Lav_Cod, Agenda.des_lib, Cultivar.Veg_Cod, ");
            stbQuery.AppendLine("  SpecieVegetali.Gru_Cod, movimenti.extra_int, movimenti.num_protocollo, ");
            stbQuery.AppendLine("  movimenti.doc_numero, movimenti.Disciplinare_PubblicoPrivato, ");
            stbQuery.AppendLine("  movimenti.Ora ");
            stbQuery.AppendLine("FROM Reg_Impianti ");
            stbQuery.AppendLine("INNER JOIN Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ");
            stbQuery.AppendLine("  AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ");
            stbQuery.AppendLine("  AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ");
            stbQuery.AppendLine("  AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ");
            stbQuery.AppendLine("INNER JOIN movimenti ON Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ");
            stbQuery.AppendLine("  AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ");
            stbQuery.AppendLine("  AND Movimenti.PIVA = Mov_Destinazioni.PIVA ");
            stbQuery.AppendLine("INNER JOIN Agenda ON Movimenti.Id_Agenda = agenda.Id_Agenda AND Movimenti.PIVA = agenda.PIVA");
            stbQuery.AppendLine("INNER JOIN Cultivar ON Reg_Impianti.cul_cod = cultivar.cul_cod ");
            stbQuery.AppendLine("INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = cultivar.veg_cod ");
            stbQuery.AppendLine("INNER JOIN Centri_Aziendali on Reg_Impianti.SA_COD = Centri_Aziendali.sa_cod ");
            stbQuery.AppendLine("  AND Reg_Impianti.PIVA = Centri_Aziendali.PIVA ");

            stbQuery.AppendLine("WHERE ");
            stbQuery.AppendLine("  Agenda.Piva = @piva ");
            if (sacod != 0) stbQuery.AppendLine("  AND Agenda.Sa_Cod in (@sacod) ");
            stbQuery.AppendLine("  AND Cultivar.Veg_Cod in (@vegcod) ");
            stbQuery.AppendLine("  AND Movimenti.Data_Movimento <= (@fine) ");
            stbQuery.AppendLine("  AND Movimenti.Data_Movimento >= (@inizio) ");
            stbQuery.AppendLine("  AND Movimenti.Inviato >= 0 ");
            stbQuery.AppendLine("  AND Movimenti.Cau_Mov in (@cau_mov) ");
            stbQuery.AppendLine("  AND agenda.lav_cod IN (@lav_cod) ");
            parSqlIn.Add("@lav_cod", FormatClauseIn(lavCods));
            parSqlIn.Add("@cau_mov", FormatClauseIn(cauMovs));

            stbQuery.AppendLine("ORDER BY  movimenti.Data_Movimento desc");
            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiOperazionixVerificaConformitaAsync(
           AgronicaCoreParametriServer objParametriServer, bool isOggettoVerifica, bool isOggettoDiVerificaMagazzino,
           string? piva = null, IEnumerable<int>? sacod = null, IEnumerable<int>? vegcod = null,
           IEnumerable<int>? typeOperation = null, IntervalloTemporale? dateInterval = null
        )
        {
            if (isOggettoVerifica && (piva == null || sacod == null || vegcod == null || dateInterval == null))
                throw new ArgumentNullException();

            const int allValues = 0;
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            List<int> lavCods = GetOperationsList(typeOperation, isOggettoVerifica, isOggettoDiVerificaMagazzino);
            List<string> cauMovs = new() {
                CAU_MOV.CAU_TRATTAMENTO, CAU_MOV.CAU_RILIEVO_CAMPO,
                CAU_MOV.CAU_RILIEVO_RACCOLTA, CAU_MOV.CAU_LAVORAZIONE
            };

            stbQuery.AppendLine("SELECT DISTINCT ");
            stbQuery.AppendLine("  Agenda.id_agenda, Agenda.piva, Agenda.sa_cod, Centri_Aziendali.sa_nome, ");
            stbQuery.AppendLine("  movimenti.Data_Movimento, Agenda.Lav_Cod, Agenda.des_lib, ");
            stbQuery.AppendLine("  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, ");
            stbQuery.AppendLine("  SpecieVegetali.Gru_Cod, movimenti.extra_int, movimenti.num_protocollo, ");
            stbQuery.AppendLine("  movimenti.doc_numero, movimenti.Disciplinare_PubblicoPrivato, ");
            stbQuery.AppendLine("  movimenti.Ora ");
            stbQuery.AppendLine("FROM Reg_Impianti ");
            stbQuery.AppendLine("INNER JOIN Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva ");
            stbQuery.AppendLine("  AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod ");
            stbQuery.AppendLine("  AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza ");
            stbQuery.AppendLine("  AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ");
            stbQuery.AppendLine("INNER JOIN movimenti ON Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ");
            stbQuery.AppendLine("  AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ");
            stbQuery.AppendLine("  AND Movimenti.PIVA = Mov_Destinazioni.PIVA ");
            stbQuery.AppendLine("  AND Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod ");
            stbQuery.AppendLine("INNER JOIN Agenda ON Movimenti.Id_Agenda = agenda.Id_Agenda AND Movimenti.PIVA = agenda.PIVA ");
            stbQuery.AppendLine("INNER JOIN Cultivar ON Reg_Impianti.cul_cod = cultivar.cul_cod ");
            stbQuery.AppendLine("INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = cultivar.veg_cod ");
            stbQuery.AppendLine("INNER JOIN Centri_Aziendali on Reg_Impianti.SA_COD = Centri_Aziendali.sa_cod ");
            stbQuery.AppendLine("  AND Reg_Impianti.PIVA = Centri_Aziendali.PIVA ");

            if (isOggettoVerifica)
            {
                stbQuery.AppendLine("WHERE ");
                stbQuery.AppendLine("  Agenda.Piva = @piva ");
                stbQuery.AppendLine("  AND Movimenti.Data_Movimento <= (@fine) ");
                stbQuery.AppendLine("  AND Movimenti.Data_Movimento >= (@inizio) ");
                if (sacod.Except(new[] { allValues }).Any())
                {
                    stbQuery.AppendLine("  AND Agenda.Sa_Cod in (@sacod) ");
                    parSqlIn.Add("@sacod", FormatClauseIn(sacod.ToList()));
                }
                if (vegcod.Except(new[] { allValues }).Any())
                {
                    stbQuery.AppendLine("  AND Cultivar.Veg_Cod  in (@vegcod) ");
                    parSqlIn.Add("@vegcod", FormatClauseIn(vegcod.ToList()));
                }
                parSql.Add("@piva", piva);
                parSql.Add("@inizio", dateInterval.inizio);
                parSql.Add("@fine", dateInterval.fine);
            }
            else
            {
                stbQuery.AppendLine("INNER JOIN #ImpiantiDaAgenda allExes ON allExes.PIVA = Mov_Destinazioni.Piva");
                stbQuery.AppendLine("  AND allExes.SA_COD = Mov_Destinazioni.Sa_Cod");
                stbQuery.AppendLine("  AND allExes.APPEZZA = Mov_Destinazioni.Appezza");
                stbQuery.AppendLine("  AND allExes.ID_REG = Mov_Destinazioni.Id_Destinazione");
            }
            stbQuery.AppendLine("  AND Movimenti.Inviato >= 0");
            stbQuery.AppendLine("  AND Movimenti.Cau_Mov in (@cau_mov) ");
            stbQuery.AppendLine("  AND agenda.lav_cod IN (@lav_cod) ");
            parSqlIn.Add("@lav_cod", FormatClauseIn(lavCods));
            parSqlIn.Add("@cau_mov", FormatClauseIn(cauMovs));

            if (isOggettoVerifica && isOggettoDiVerificaMagazzino && (lavCods.Contains(LAV_COD.LAVCOD_CONCIA_SEME) || lavCods.Contains(LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA))) //questa operazione non viene letta dalla query precedente perché è applicata sul prodotto e non sull'impianto
            {
                //ad oggi (gennaio 2026) la concia del seme serve solo per la verifica del magazzino, e la leggiamo solo la prima volta (ciclo con oggettoVerifica = true)
                stbQuery.AppendLine(" UNION ");
                stbQuery.AppendLine(" SELECT DISTINCT ");
                stbQuery.AppendLine("        a.id_agenda, a.piva, a.sa_cod, ca.sa_nome, ");
                stbQuery.AppendLine("        m.Data_Movimento, a.Lav_Cod, a.des_lib, mp.Veg_Cod, ");
                stbQuery.AppendLine("        sv.Veg_Des, sv.Gru_Cod, m.extra_int, m.num_protocollo, ");
                stbQuery.AppendLine("        m.doc_numero, m.Disciplinare_PubblicoPrivato, m.Ora ");
                stbQuery.AppendLine(" FROM ");
                stbQuery.AppendLine("        Agenda a ");
                stbQuery.AppendLine(" JOIN ");
                stbQuery.AppendLine("        Movimenti m ");
                stbQuery.AppendLine("        ON m.Id_Agenda = a.Id_Agenda ");
                stbQuery.AppendLine("        AND m.PIVA = a.PIVA ");
                stbQuery.AppendLine(" JOIN ");
                stbQuery.AppendLine("        Movimenti_dettagli md ");
                stbQuery.AppendLine("        ON md.PIVA = m.PIVA ");
                stbQuery.AppendLine("        AND md.Sa_Cod = m.Sa_Cod ");
                stbQuery.AppendLine("        AND md.Id_Agenda = m.Id_Agenda ");
                stbQuery.AppendLine("        AND md.Id_Mov = m.Id_Mov ");
                stbQuery.AppendLine("        AND md.Mat_Cod > 0 ");
                stbQuery.AppendLine(" JOIN");
                stbQuery.AppendLine("        Materie_Prime mp");
                stbQuery.AppendLine("        ON mp.Mat_Cod = md.Mat_Cod");
                stbQuery.AppendLine(" JOIN");
                stbQuery.AppendLine("        Cultivar c");
                stbQuery.AppendLine("        ON c.cul_cod = mp.cul_cod");
                stbQuery.AppendLine(" JOIN");
                stbQuery.AppendLine("        SpecieVegetali sv");
                stbQuery.AppendLine("        ON sv.veg_cod = c.veg_cod");
                stbQuery.AppendLine(" JOIN ");
                stbQuery.AppendLine("        Centri_Aziendali ca ");
                stbQuery.AppendLine("        ON ca.SA_COD = a.sa_cod ");
                stbQuery.AppendLine("        AND ca.PIVA = a.PIVA ");
                stbQuery.AppendLine(" WHERE ");
                stbQuery.AppendLine("        a.Piva = @piva ");
                stbQuery.AppendLine("        AND m.Data_Movimento <= (@fine) ");
                stbQuery.AppendLine("        AND m.Data_Movimento >= (@inizio) ");
                if (sacod.Except(new[] { allValues }).Any())
                {
                    stbQuery.AppendLine("        AND a.Sa_Cod in (@sacod) ");
                }
                stbQuery.AppendLine("        AND m.Inviato >= 0");
                stbQuery.AppendLine("        AND m.Cau_Mov in (@cau_mov) ");
                stbQuery.AppendLine($"        AND a.lav_cod IN ({LAV_COD.LAVCOD_CONCIA_SEME}, {LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA})");
            }

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiMovimentiProdottiPerOperazioniAsync(AgronicaCoreParametriServer objParametriServer, List<string> idAgendas, string piva)
        {
            if (idAgendas is null)
                throw new ArgumentNullException("idAgendas");

            if (!idAgendas.Any())
                throw new ArgumentException("La lista di id agenda non contiene nessun elemento");

            var sb = new StringBuilder();

            sb.AppendLine(" SELECT DISTINCT");
            sb.AppendLine("        m.Id_Agenda, mdet.Id_Mov_Det, m.Data_Movimento, mdest.Piva, mdest.Sa_Cod, mdest.Id_Destinazione, ");
            sb.AppendLine("        mdet.Qta, mdet.Udm_Cod,");
            sb.AppendLine("        mdet.Elem_Cod, mdet.Pro_Cod, mdet.Mat_Cod, mdet.Mov_Det_Des,");
            sb.AppendLine("        mdet.Lotto, f.fabbricato_des, u.udm_sim, m.Cau_Mov");
            sb.AppendLine(" FROM ");
            sb.AppendLine("        Mov_Destinazioni mdest ");
            sb.AppendLine(" INNER JOIN ");
            sb.AppendLine("        Movimenti_dettagli mdet");
            sb.AppendLine("        ON mdet.Id_Agenda = mdest.Id_Agenda AND mdest.Piva = mdet.PIVA ");
            sb.AppendLine("        AND mdest.Sa_Cod = mdet.Sa_Cod AND mdest.Id_Mov = mdet.Id_Mov ");
            sb.AppendLine("        AND mdest.Id_Mov_Det = mdet.Id_Mov_Det ");
            sb.AppendLine(" INNER JOIN ");
            sb.AppendLine("        fabbricati f");
            sb.AppendLine("        ON mdest.Piva = f.PIVA AND mdest.Sa_Cod = f.SA_COD ");
            sb.AppendLine("        AND mdest.Id_Destinazione = f.Fabbricato_Cod ");
            sb.AppendLine(" INNER JOIN ");
            sb.AppendLine("        UnitaMisura u");
            sb.AppendLine("        ON mdet.udm_cod = u.Udm_Cod");
            sb.AppendLine(" INNER JOIN ");
            sb.AppendLine("        Movimenti m");
            sb.AppendLine("        ON mdet.PIVA = m.PIVA AND mdet.Id_Agenda = m.Id_Agenda And mdet.Id_Mov = m.Id_Mov ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("        m.id_agenda IN (" + string.Join(",", idAgendas) + ")");
            sb.AppendLine("        AND mdest.PIVA = @piva");
            sb.AppendLine("        AND tipo_destinazione = " + TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO);
            sb.AppendLine("        AND Cau_Mov = '" + CAU_MOV.CAU_SCARICO + "'");
            sb.AppendLine("        AND mdest.Inviato >= 0");

            Dictionary<string, object> parameters = new();
            parameters.Add("@piva", piva);

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(sb.ToString(), parameters);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiMovimentiCorrelatiAsync(List<MovimentoFilter> movimenti, DateTime dataFine, AgronicaCoreParametriServer objParametriServer)
        {
            if (movimenti is null)
                throw new ArgumentNullException(nameof(movimenti));

            if (!movimenti.Any())
                throw new ArgumentException("Ci deve essere almeno un movimento");

            var sb = new StringBuilder();
            Dictionary<string, object> parameters = new();

            var filtroCauMovScaricoECarico = $"('{CAU_MOV.CAU_SCARICO}', '{CAU_MOV.CAU_CARICO}', '{CAU_MOV.CAU_CONFERIMENTO}', '{CAU_MOV.CAU_CONFERIMENTO_DIVERSI}')";

            sb.AppendLine(" SELECT");
            sb.AppendLine("        m.Id_Agenda, mdet.Id_Mov_Det, m.Data_Movimento, ISNULL(ROUND(mdest.Qta, +5), 0) as Qta,  ");
            sb.AppendLine("        mdest.Piva, mdest.Sa_Cod, mdest.Id_Destinazione, mdest.Id_Agenda, mdet.Udm_Cod,");
            sb.AppendLine("        mdet.Elem_Cod, mdet.Pro_Cod, mdet.Mat_Cod, mdet.Lotto, m.Cau_Mov");
            sb.AppendLine(" FROM ");
            sb.AppendLine("        Agenda a");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("        Movimenti m");
            sb.AppendLine("        ON a.PIVA = m.PIVA AND a.Id_Agenda = m.Id_Agenda ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("        Movimenti_Dettagli mdet ");
            sb.AppendLine("        ON m.PIVA = mdet.PIVA AND m.Id_Agenda = mdet.Id_Agenda AND m.Id_Mov = mdet.Id_Mov ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("        Mov_Destinazioni mdest");
            sb.AppendLine("        ON mdet.PIVA = mdest.Piva AND mdet.Id_Agenda = mdest.Id_Agenda AND mdet.Id_Mov = mdest.Id_Mov AND mdet.Id_Mov_Det = mdest.Id_Mov_Det ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("        mdet.Jolly_Int = 0 ");
            sb.AppendLine("        AND mdet.Contabilizzato >= 0 ");
            sb.AppendLine("        AND tipo_destinazione = " + TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO);
            sb.AppendLine("        AND Cau_Mov IN " + filtroCauMovScaricoECarico);
            sb.AppendLine("        AND m.Data_Movimento >= @dataInizio");
            sb.AppendLine("        AND m.Data_Movimento <= @dataFine");

            sb.AppendLine("        AND (");

            var lastMovimento = movimenti.Last();
            foreach (var movimento in movimenti)
            {
                sb.AppendLine("        (");

                var chiaviMagazzino = movimento.ChiaveMagazzino.Split("_");
                var piva = chiaviMagazzino[0];
                var saCod = int.Parse(chiaviMagazzino[1]);
                var idDestinazione = int.Parse(chiaviMagazzino[2]);

                sb.AppendLine($"        mdest.Piva = '{piva}' ");
                sb.AppendLine($"        AND mdest.Sa_Cod = {saCod} ");
                sb.AppendLine($"        AND mdest.Id_Destinazione = {idDestinazione} ");
                sb.AppendLine($"        AND mdet.Elem_Cod = {movimento.CategoriaProdotto} ");

                if (movimento.Pro_Cod != 0)
                {
                    sb.AppendLine($"        AND mdet.Pro_Cod = {movimento.Pro_Cod} ");
                }
                if (movimento.Mat_Cod != 0)
                {
                    sb.AppendLine($"        AND mdet.Mat_Cod = {movimento.Mat_Cod} ");
                }

                sb.AppendLine($"        AND mdet.Lotto = '{movimento.Lotto}' ");
                sb.AppendLine($"        AND mdet.Udm_Cod = {movimento.UnitaDiMisura} ");

                sb.AppendLine("        )");

                if (movimento != lastMovimento)
                {
                    sb.AppendLine("        OR ");
                }
            }
            sb.AppendLine("        )");

            parameters.Add("@dataInizio", CostantiPersonalizzate.AGRODATAINIZIO);
            parameters.Add("@dataFine", dataFine);

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(sb.ToString(), parameters);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private List<int> GetOperationsList(IEnumerable<int>? typeOperation, bool isOggettoDiVerifica, bool isOggettoDiVerificaMagazzino)
        {
            if (typeOperation == null || !typeOperation.Any()) typeOperation = new List<int> { 0 };

            List<int> lavCods = new List<int>();
            if (typeOperation.Contains(0) || typeOperation.Contains(1)) // TRATTAMENTO
            {
                lavCods.AddRange(Common.OperazioniTrattamento);
                if (isOggettoDiVerificaMagazzino)
                {
                    lavCods.AddRange(Common.OperazioniTrappoleInstallate);
                }
            }
            if (typeOperation.Contains(0) || typeOperation.Contains(2)) // FERTILIZZAZIONE
            {
                lavCods.AddRange(Common.OperazioniFertilizzazione);
            }
            if (typeOperation.Contains(0) || typeOperation.Contains(3)) // RACCOLTA
            {
                lavCods.AddRange(Common.OperazioniRaccolta);
            }
            if (!isOggettoDiVerifica)
            {
                lavCods.AddRange(Common.OperazioniRilievo);
            }
            if (isOggettoDiVerificaMagazzino || !isOggettoDiVerifica) //vengono lette le semine come operazioni correlate oppure per fare la verifica di conformità del magazzino
            {
                lavCods.AddRange(Common.OperazioniSeminaTrapianto);
            }
            return lavCods;
        }

        public async Task<bool> BulkInsertEserciziAsync(DataTable dtEsercizi, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtEsercizi is null || dtEsercizi.Rows.Count == 0)
            {
                throw new ArgumentException("Datatable contente gli esercizi non è valido");
            }

            var result = false;
            var strSql = new StringBuilder() { Length = 0 };

            try
            {
                AddQueryForCreatingTempEsercizi(strSql);

                result = await GetDataProvider(objParametriServer).Execute_WriteAsync(strSql.ToString());

                result = result && await GetDataProvider(objParametriServer).ExecuteBulkInsertAsync(dtEsercizi, "#ImpiantiDaAgenda", batchSize: dtEsercizi.Rows.Count);

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiDatiBaseOperazioniCorrelate(IEnumerable<int> idAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            List<string> cauMovs = new()  {
                CAU_MOV.CAU_TRATTAMENTO, CAU_MOV.CAU_RILIEVO_CAMPO,
                CAU_MOV.CAU_RILIEVO_RACCOLTA, CAU_MOV.CAU_LAVORAZIONE
            };

            stbQuery.AppendLine("SELECT DISTINCT ");
            stbQuery.AppendLine("  Agenda.Id_Agenda, Agenda.des_lib, Movimenti.Data_Movimento, Appezzamento.APP_NOME ");
            stbQuery.AppendLine("FROM Agenda");
            stbQuery.AppendLine("LEFT JOIN Movimenti ON Movimenti.Id_Agenda = Agenda.Id_Agenda");
            stbQuery.AppendLine("LEFT JOIN Mov_Destinazioni ON Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov");
            stbQuery.AppendLine("LEFT JOIN Appezzamento ON Appezzamento.Piva = Mov_Destinazioni.Piva");
            stbQuery.AppendLine("  AND Appezzamento.SA_COD = Mov_Destinazioni.Sa_Cod");
            stbQuery.AppendLine("  AND Appezzamento.APPEZZA = Mov_Destinazioni.Appezza");
            stbQuery.AppendLine("WHERE Movimenti.Cau_Mov IN (@caumov) ");
            parSqlIn.Add("@caumov", FormatClauseIn(cauMovs));
            if (idAgenda.Any())
            {
                stbQuery.AppendLine("  AND Agenda.Id_Agenda in (@ids) ");
                parSqlIn.Add("@ids", FormatClauseIn(idAgenda.ToList()));
            }

            try
            {
                DataTable dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
                return dt.AsEnumerable()
                    .GroupBy(row => row.Field<int>("Id_Agenda"))
                    .Select(groupedRows =>
                    {
                        DataRow first = groupedRows.First();
                        first["APP_NOME"] = string.Join(", ", groupedRows.Select(x => x.Field<string>("APP_NOME")));
                        return first;
                    }).CopyToDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiDettCodAsync(
            IReadOnlyList<int> idAgendas,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(idAgendas);

            if (idAgendas.Count == 0)
                return new DataTable();

            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();
            parSqlIn.Add("@idAgendas", FormatClauseIn(idAgendas.ToList()));

            // MIN(Dett_Cod): quando ci sono più righe prendo la prima in senso deterministico.
            const string query =
                @"SELECT Piva, Sa_Cod, Id_Agenda, Id_Mov, MIN(Dett_Cod) AS Dett_Cod
                  FROM Mov_Dettaglio_Tecnico
                  WHERE Id_Agenda IN (@idAgendas)
                  GROUP BY Piva, Sa_Cod, Id_Agenda, Id_Mov";

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(query, new Dictionary<string, object>(), parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private void AddQueryForDroppingAllTemporaryTables(StringBuilder strSql)
        {
            AddQueryForDroppingTemporaryTablesRelatedToActivePrinciples(strSql);
            strSql.AppendLine(" IF OBJECT_ID('tempdb..#ImpiantiDaAgenda') IS NOT NULL");
            strSql.AppendLine("    DROP TABLE #ImpiantiDaAgenda");
        }

        private void AddQueryForDroppingTemporaryTablesRelatedToActivePrinciples(StringBuilder strSql)
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

        private void AddQueryForCreatingTempEsercizi(StringBuilder strSql)
        {
            strSql.AppendLine(" CREATE TABLE #ImpiantiDaAgenda");
            strSql.AppendLine(" (");
            strSql.AppendLine("     Piva NVARCHAR(25) COLLATE DATABASE_DEFAULT NOT NULL,");
            strSql.AppendLine("     Sa_Cod INT,");
            strSql.AppendLine("     Appezza INT,");
            strSql.AppendLine("     Id_Reg INT");
            strSql.AppendLine(" );");
        }

        private void AddQueryForCreatingTemporaryTables(StringBuilder strSql)
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
    }
}
