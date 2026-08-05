using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Data;
using System.Reflection;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Magazzino.DAL.DataLayer
{
    public class Magazzino : DAL_Base, IMagazzino
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IUtilityDB _utilityDB;
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IUtentiProfili _utentiProfili;
        private readonly IUtilityDB _utilityDBDAL;
        private readonly TempChiaviMassivo _tempChiaviMassivo;

        public Magazzino(IServiceProvider provider) : base(provider)
        {
            _serviceProvider = provider;
            _utilityDB = _serviceProvider.GetRequiredService<IUtilityDB>();
            _utentiImpostazioni = _serviceProvider.GetRequiredService<IUtentiImpostazioni>();
            _utentiProfili = _serviceProvider.GetRequiredService<IUtentiProfili>();
            _utilityDBDAL = provider.GetRequiredService<IUtilityDB>();
            _tempChiaviMassivo = _serviceProvider.GetRequiredService<TempChiaviMassivo>();

        }


        public async Task<DataTable> SchedaGiacenzeMagazzino(DateTime Data_Giacenza,
            string Piva,
            int Sa_Cod,
            int Id_Destinazione,
            int Elem_Cod,
            int Pro_Cod,
            int Mat_Cod,
            int Cal_Cod,
            int Cod_Progetto,
            int Fase_Cod,
            int Udm_Cod,
            string Lotto,
            bool Flag_QtaNoZero,
            FiltroAggiuntivo? xFiltroAggiuntivo,
            FiltroAggiuntivo? xFiltroAggiuntivo_1,
            FiltroAggiuntivo? xFiltroAggiuntivo_2,
            FiltroAggiuntivo? xFiltroAggiuntivo_3,
            FiltroAggiuntivo? xFiltroAggiuntivo_4,
            FiltroAggiuntivo? xFiltroAggiuntivo_5,
            FiltroAggiuntivo? xFiltroAggiuntivo_6,
            FiltroAggiuntivo? xFiltroAggiuntivo_7,
            FiltroAggiuntivo? xFiltroAggiuntivo_8,
            FiltroAggiuntivo? xFiltroAggiuntivo_9,
            FiltroAggiuntivo? xFiltroAggiuntivo_10,
            FiltroAggiuntivo? xFiltroAggiuntivo_12,
            string xOrderBy,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti,
            FiltroAggiuntivo? xFiltroAggiuntivo_13 = null,
            string StrQuery_Output = "",
            bool isFreshAndFood = false,
            bool cercaLottoPerLike = false,
            FiltroAggiuntivo? xFiltroAggiuntivo_14 = null,
            bool Flag_QtaMaggioreZero = false,
            bool flagRecuperaCodArticolo = false,
            string codArticolo = "",
            bool cercaCodArticoloPerLike = false,
            FiltroAggiuntivo? xFiltroAggiuntivo_15 = null,
            bool eseguiQuery = true,
            bool inibisciVisibilitaGruppiMerce = false,
            bool creaParametriSql = true,
            FiltroAggiuntivo? xFiltroAggiuntivo_16 = null,
            bool leggiLinea = false,
            List<string> filtroMagazziniEsterni = null,
            List<int> calCodEsclusi = null,
            List<(string, int, int)> joinTempMagazzini = null,
            List<int> joinTempSpecie = null)
        {
            string nomeRoutine = "AgronicaNetCore.Operazione.DALSchedaGiacenzeMagazzino()";

            Dictionary<string, object> parSql = new();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            bool visibilita_totale = await _utentiProfili.VisibilitaTotaleGiasOnline(objParametriUtenti, objParametriServer);
            if (!visibilita_totale)
                parSql.Add("@username", objParametriUtenti.UtenteUsername);

            /// TODO 1
            /// Gestire ImpostazioneDefault Gruppi Merce
            /// Aggiungere Parametro opzionale: List<ImpostazioneDefault_GruppiMerce> gruppiMerceDefaultPerCategoria = null
            /// 

            if (filtroMagazziniEsterni != null && filtroMagazziniEsterni.Count > 0)
                Piva = "";


            string messaggioErrore = "";
            StringBuilder SQL_Generale = new StringBuilder();
            DataTable dt;
            int i, i_tot;


            DataTable dtParamQual = new DataTable();
            if (isFreshAndFood)
            {
                throw new Exception("Parametro isFreshAndFood non gestito");
                /// TODO
                //AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R objConfigDettagli = new AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R();
                //dtParamQual = objConfigDettagli.Leggi(Piva, 0, false, "Tipo = 1", "", objParametriServer);
            }

            try
            {
                if (joinTempMagazzini != null && joinTempMagazzini.Count > 0)
                    await _tempChiaviMassivo.CreaTabellaTemp_FiltroFabbricati(joinTempMagazzini, objParametriServer);
                if (joinTempSpecie != null && joinTempSpecie.Count > 0)
                    await _tempChiaviMassivo.CreaTabellaTemp_FiltroChiaveInt(joinTempSpecie, objParametriServer);

                switch (Elem_Cod)
                {
                    case 0 // tutte le categorie
                   :
                        {
                            i_tot = 16;
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_VEGETALI:
                        {
                            i_tot = 2; // 2 cicli per gestire la union tra CAL_COD = 0 (bottiglie e trasformati acquistati) e CAL_COD <> 0 trasf raccolti
                            break;
                        }

                    case ELEM_COD.SEMILAVORATI_VEGETALI:
                        {
                            i_tot = 3;
                            break;
                        }

                    case ELEM_COD.FERTILIZZANTI:
                        {
                            i_tot = 2; // 2 cicli per gestire la union tra fertilizzanti banca dati e fertilizzanti azi
                            break;
                        }

                    case ELEM_COD.CARBURANTI:
                        {
                            i_tot = 2; // 2 cicli per gestire la union tra vecchia gestione (banca dati e nuova gestione (materie prime)
                            break;
                        }

                    case ELEM_COD.FARMACI:
                        {
                            i_tot = 2; // 2 cicli per gestire la union tra materie_prime e tabella farmaci
                            break;
                        }

                    default:
                        {
                            i_tot = 1;
                            break;
                        }
                }


                // 15/01/2019: spostato l'add fuori dal ciclo, altrimenti ad ogni giro veniva sommato un giorno alla data fine
                Data_Giacenza = Data_Giacenza.AddDays(1);

                // Giulia: 29/3/2019: leggo l'opzione per sapere se devo fare join su CAC per Piva o Piva_SuperUser
                bool flagJoinSuperUserCac = await GetFlagJoinCac(objParametriUtenti, objParametriServer);

                /// TODO 3
                /// Riportare questo:
                /// 

                //Gruppi_Merce_R objGruppiMerce = new Gruppi_Merce_R();
                //if (!Information.IsNothing(gruppiMerceDefaultPerCategoria) && gruppiMerceDefaultPerCategoria.Count > 0)
                //{
                //    var strSql = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(new List<string>() { Piva }, objParametriServer, gruppiMerceDefaultPerCategoria);

                //    SQL_Generale.Append(strSql.ToString());
                //    SQL_Generale.AppendLine("");
                //}

                /// FINE TODO 3

                // per ogni sezione della union
                for (i = 1; i <= i_tot; i++)
                {
                    if (i != 1)
                    {
                        SQL_Generale.AppendLine();
                        SQL_Generale.AppendLine(Constants.vbCrLf + " UNION ALL ");
                        SQL_Generale.AppendLine();
                    }

                    // ------------------------------------------------------ 
                    // ------------------- SELECT ---------------------------
                    // ------------------------------------------------------

                    SQL_Generale.AppendLine(" ( ");

                    SQL_Generale.AppendLine(" SELECT Imprese.Piva, Imprese.Rag_Soc AS Impresa, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, ");
                    SQL_Generale.AppendLine(" CategorieMagazzino.Tabella_Cod, CategorieMagazzino.Tabella_Des, CategorieMagazzino.Tabella_Tipo, ");
                    SQL_Generale.AppendLine(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ");
                    SQL_Generale.AppendLine(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ");
                    SQL_Generale.AppendLine(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, ");
                    SQL_Generale.AppendLine(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ");
                    SQL_Generale.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des, ");
                    SQL_Generale.AppendLine(" Centri_Aziendali.Sa_Nome ");

                    SQL_Generale.AppendLine(" , Isnull(Imprese_Progetti.Progetto_Nome, '') as Lotto_Interno ");

                    if (isFreshAndFood)
                    {
                        SQL_Generale.AppendLine(" , Cantina_Vasche.Identificativo, Cantina_Vasche.Insieme_Cod,  ");
                        SQL_Generale.AppendLine(" Cantina_Insiemi.insieme_des ");

                        SQL_Generale.AppendLine(" , MIN(Movimenti.Data_Movimento) As Data_Movimento  ");


                        /// TODO

                        //foreach (DataRow paramQual in dtParamQual.Rows)
                        //{

                        //    if (paramQual("Tipo") == 3 || paramQual("Tipo") == 4 || paramQual("Tipo") == 5)
                        //        SQL_Generale.AppendLine(" , COALESCE(MAX(Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Val_Cod), '') AS FF_" + paramQual("Tabella_Key") + "_Val_Cod  ");
                        //    if (paramQual("Tipo") == 1)
                        //    {
                        //        SQL_Generale.AppendLine(" , COALESCE(MAX(Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Tipo_Cod), 0) AS FF_" + paramQual("Tabella_Key") + "_Tipo_Cod  ");

                        //        if (!(new[] { "cliente", "fornitore" }).Contains(paramQual("Tabella_Key")))
                        //        {
                        //            SQL_Generale.AppendLine(" , COALESCE(MAX(Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Tara_Campionatura), 0) AS FF_" + paramQual("Tabella_Key") + "_Tara_Campionatura  ");
                        //            SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Sigla), '') AS FF_" + paramQual("Tabella_Key") + "_Sigla  ");
                        //            SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Descrizione), '') AS FF_" + paramQual("Tabella_Key") + "_Descrizione ");
                        //            SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Codice_Generazione_Link), '') AS FF_" + paramQual("Tabella_Key") + "_Codice_Generazione_Link ");
                        //            SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Mat_Cod_Generazione_Link), '') AS FF_" + paramQual("Tabella_Key") + "_Mat_Cod_Generazione_Link ");
                        //        }
                        //    }
                        //}

                        /// FINE TODO

                        SQL_Generale.AppendLine("  , SUM(  CASE WHEN Movimenti.CAU_MOV In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') ");
                        SQL_Generale.AppendLine("           THEN -(Movimenti_dettagli.Tara)");
                        SQL_Generale.AppendLine("           ELSE Movimenti_dettagli.Tara");
                        SQL_Generale.AppendLine("           END  ) AS TaraTotale ");

                        SQL_Generale.AppendLine(" , MIN( Movimenti_dettagli.Qta_Extra) AS Qta_Extra ");
                    }


                    SQL_Generale.AppendLine(" , UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ");


                    /// TODO 4

                    //if (!Information.IsNothing(gruppiMerceDefaultPerCategoria) && gruppiMerceDefaultPerCategoria.Count > 0)
                    //{
                    //    SQL_Generale.AppendLine(" ,COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) AS Id_Gruppo_Merce ");
                    //    SQL_Generale.AppendLine(" ,CASE ");
                    //    SQL_Generale.AppendLine(" WHEN grpMerce.Codice IS NOT NULL THEN grpMerce.Codice + ' ' + grpMerce.Descrizione ");
                    //    SQL_Generale.AppendLine(" WHEN #DefaultGruppiMerce.Elem_Cod IS NOT NULL THEN #DefaultGruppiMerce.Codice + ' ' + #DefaultGruppiMerce.Descrizione COLLATE DATABASE_DEFAULT ");
                    //    SQL_Generale.AppendLine(" ELSE '' END AS Des_Gruppo_Merce ");
                    //}

                    /// FINE TODO 4

                    SchedeMagazzinoGestioneSelect(ref SQL_Generale, Elem_Cod, i, isFreshAndFood, flagRecuperaCodArticolo, leggiLinea);

                    // Il calcolo della giacenza è diverso se è isFreshAndFood ma solo in alcune categorie
                    SchedeMagazzinoGestioneSelectGiacenza(ref SQL_Generale, Elem_Cod, i, isFreshAndFood);

                    // ------------------------------------------------------ 
                    // -------------------- FROM ----------------------------
                    // ------------------------------------------------------

                    // JOIN AGENDA - MOVIMENTI
                    SQL_Generale.AppendLine(" FROM    Agenda ");

                    // ------------------------------------------------------ 
                    // -------------------- JOIN ----------------------------
                    // ------------------------------------------------------

                    // JOIN AGENDA - MOVIMENTI
                    SQL_Generale.AppendLine(" INNER JOIN Movimenti ");
                    SQL_Generale.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda");

                    // JOIN IMPRESE - AGENDA
                    SQL_Generale.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ");

                    // JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                    SQL_Generale.AppendLine(" INNER JOIN Movimenti_dettagli ");
                    SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ");

                    // JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                    SQL_Generale.AppendLine(" INNER JOIN Mov_Destinazioni ");
                    SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ");
                    SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ");
                    SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ");

                    // JOIN MOVIMENTI DESTINAZIONI - CENTRI AZIENDALI 
                    SQL_Generale.AppendLine(" INNER JOIN Centri_Aziendali ");
                    SQL_Generale.AppendLine(" ON Mov_Destinazioni.PIVA = Centri_Aziendali.Piva AND Mov_Destinazioni.Sa_Cod = Centri_Aziendali.Sa_Cod ");

                    if (!visibilita_totale)
                    {
                        SQL_Generale.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio uva ");
                        SQL_Generale.AppendLine(" ON Mov_Destinazioni.PIVA = uva.Piva AND Mov_Destinazioni.Sa_Cod = uva.Sa_Cod AND uva.Entita_Cod = 2 AND uva.Username = @username ");
                    }

                    if (!isFreshAndFood)
                    {
                        // JOIN FABBRICATO
                        SQL_Generale.AppendLine(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ");
                        SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ");

                        // JOIN TIPO FABBRICATO
                        SQL_Generale.AppendLine(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ");
                    }
                    else
                    {
                        // Id_Destinazione potrebbe essere un magazzino o una cella

                        // JOIN FABBRICATO
                        SQL_Generale.AppendLine(" LEFT JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ");
                        SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ");

                        // JOIN TIPO FABBRICATO
                        SQL_Generale.AppendLine(" LEFT JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ");

                        // JOIN Cantina Insiemi per trovare decodifica cella
                        SQL_Generale.AppendLine(" LEFT JOIN Cantina_Vasche ON Mov_Destinazioni.Piva = Cantina_Vasche.PIVA AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.SA_COD AND ");
                        SQL_Generale.AppendLine(" Mov_Destinazioni.tipo_destinazione = Cantina_Vasche.tipo_destinazione AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.vas_cod ");

                        SQL_Generale.AppendLine(" LEFT JOIN Cantina_Insiemi ON Cantina_Vasche.Insieme_Cod = Cantina_Insiemi.Insieme_Cod ");
                    }

                    // JOIN CATEGORIE MAGAZZINO
                    SQL_Generale.AppendLine(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod ");

                    // JOIN UNITA DI MISURA
                    SQL_Generale.AppendLine(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod ");

                    // JOIN IMPRESE_PROGETTI
                    SQL_Generale.AppendLine(" LEFT OUTER JOIN Imprese_Progetti  ON Imprese_Progetti.Piva = Movimenti_Dettagli.Piva And Imprese_Progetti.Progetto_Cod = Movimenti_dettagli.Cod_Progetto ");


                    if (isFreshAndFood)
                    {

                        /// TODO

                        //// JOIN MATERIE_PRIME_CAMPIONATURE
                        //foreach (var paramQual in dtParamQual.Rows)
                        //{
                        //    SQL_Generale.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" + paramQual("Tabella_Key") + " ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Progressivo " + " AND Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Tipo = 'o" + paramQual("Tabella_Key") + "'");
                        //    if (paramQual("Tipo") == 1)
                        //    {
                        //        if (!(new[] { "cliente", "fornitore" }).Contains(paramQual("Tabella_Key")))
                        //            SQL_Generale.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" + paramQual("Tabella_Key") + " ON Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Tipo_Cod =  OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Tabella_Par_Cod " + " AND OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Tabella_Cod = '" + paramQual("Tabella_ID") + "'");
                        //    }
                        //}

                        /// FINE TODO

                    }

                    /// TODO 5

                    //if (!Information.IsNothing(gruppiMerceDefaultPerCategoria) && gruppiMerceDefaultPerCategoria.Count > 0)
                    //{
                    //    SQL_Generale.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock) ");
                    //    SQL_Generale.AppendLine("              ON prodExtraPriv.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametriServer.PivaSuperUser) + "'");
                    //    SQL_Generale.AppendLine("       AND prodExtraPriv.Elem_Cod = Movimenti_dettagli.Elem_Cod ");
                    //    SQL_Generale.AppendLine("       AND prodExtraPriv.Mat_Cod = Movimenti_dettagli.Mat_Cod");
                    //    SQL_Generale.AppendLine("       AND prodExtraPriv.Pro_Cod = Movimenti_dettagli.Pro_Cod");
                    //    SQL_Generale.AppendLine("             AND (prodExtraPriv.Piva = Movimenti_dettagli.Piva OR Movimenti_dettagli.Pro_Cod = 0)");
                    //    SQL_Generale.AppendLine(" LEFT JOIN Gruppi_Merce as grpMerce WITH (nolock)");
                    //    SQL_Generale.AppendLine("              ON grpMerce.Id_Gruppo_Merce = prodExtraPriv.Id_Gruppo_Merce");
                    //    SQL_Generale.AppendLine(" LEFT JOIN #DefaultGruppiMerce");
                    //    SQL_Generale.AppendLine("              ON #DefaultGruppiMerce.Elem_Cod = Movimenti_dettagli.Elem_Cod");
                    //}

                    /// FINE TODO 5

                    SchedeMagazzinoGestioneJoin(ref SQL_Generale, parSql, parSqlIn, Elem_Cod, i, isFreshAndFood, flagRecuperaCodArticolo, flagJoinSuperUserCac, objParametriServer.PivaSuperUser, joinTempSpecie is not null && joinTempSpecie.Count > 0);


                    if (joinTempMagazzini != null && joinTempMagazzini.Count > 0)
                        SQL_Generale.AppendLine(" INNER JOIN #TempFabbricato temp  ON temp.Piva = Mov_Destinazioni.Piva And temp.Sa_Cod = Mov_Destinazioni.Sa_COD AND temp.Fabbricato_Cod = Mov_Destinazioni.Id_Destinazione ");

                    // ------------------------------------------------------ 
                    // -------------------- WHERE ---------------------------
                    // ------------------------------------------------------

                    // Marco G. 19/01/2018 Tolto il <= e messo il giorno successivo così da gestire le operazioni con orario che altrimenti verrebbero escluse
                    // Spostato l'add day fuori dal ciclo sennò ogni volta aggiungeva 1
                    SQL_Generale.AppendLine(" WHERE     1 = 1 ");

                    string parDataGiacenzaName = "@Data_Giacenza" + i;
                    parSql.Add(parDataGiacenzaName, Data_Giacenza);
                    SQL_Generale.AppendLine(" And       Movimenti.Data_Movimento < " + parDataGiacenzaName + " ");

                    string parAgrodataInizioName = "@agrodatainizio" + i;
                    parSql.Add(parAgrodataInizioName, COSTANTI_GENERALI.AGRODATAINIZIO);
                    SQL_Generale.AppendLine(" And       Movimenti.Data_Movimento >= " + parAgrodataInizioName + " ");

                    SQL_Generale.AppendLine(" And       Movimenti_Dettagli.Elem_Cod <> " + CATEGORIE_MAGAZZINO.CALI_LAVORAZIONE + " "); // escludere i cali di lavorazione
                    SQL_Generale.AppendLine(" And       Movimenti_Dettagli.Elem_Cod <> " + CATEGORIE_MAGAZZINO.CORPI_ESTRANEI + " "); // escludere i corpi estranei

                    SQL_Generale.AppendLine(" And Movimenti.Cau_Mov In ('" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "', '" + CAU_MOV.CAU_ACCETTAZIONE_BENI_DA_DIVERSI + "', '" + CAU_MOV.CAU_CARICO + "', '" + CAU_MOV.CAU_SCARICO + "', '" + CAU_MOV.CAU_CONFERIMENTO + "', '" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "')   ");

                    // --------------
                    // Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                    // e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                    SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Jolly_Int = " + COSTANTI_GENERALI.MAGAZZINO_MOVIMENTATO + "   ");
                    SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Contabilizzato >= 0  ");
                    // --------------

                    if (isFreshAndFood)
                    {
                        SQL_Generale.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione IN (" + TIPI_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO + "," + TIPI_DESTINAZIONE.CELLA_FRIGORIFERA + ")");
                        if (Elem_Cod == CATEGORIE_MAGAZZINO.TRASFORMATI_VEGETALI && calCodEsclusi != null && calCodEsclusi.Count > 0)
                        {
                            string parCalCodEsclusi = "@CalCodEsclusi" + i;
                            parSqlIn.Add(parCalCodEsclusi, FormatClauseIn(calCodEsclusi));
                            SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cal_Cod NOT IN (" + parCalCodEsclusi + " ) ");
                        }
                    }
                    else
                        SQL_Generale.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " + TIPI_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO + " ");

                    if (Piva != "")
                    {
                        string parPiva = "@Piva" + i;
                        parSql.Add(parPiva, Piva);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Piva = " + parPiva + "   ");
                    }
                    else if (filtroMagazziniEsterni != null && filtroMagazziniEsterni.Count > 0)
                    {
                        string parPivaMagazziniEsterni = "@PivaMagazziniEsterni" + i;
                        parSqlIn.Add(parPivaMagazziniEsterni, FormatClauseIn(filtroMagazziniEsterni));
                        SQL_Generale.AppendLine(" AND (Movimenti_Dettagli.Piva IN (" + parPivaMagazziniEsterni + ") ) ");
                    }

                    if (Sa_Cod != 0)
                    {
                        string parSa_Cod = "@Sa_Cod" + i;
                        parSql.Add(parSa_Cod, Sa_Cod);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " + parSa_Cod + "  ");
                    }
                    //else if (filtroCentri != "")
                    //    SQL_Generale.AppendLine(" AND (Movimenti_Dettagli.sa_cod IN (" + Agro_SQL_Save_Clausola_IN(Strings.Left(filtroCentri, filtroCentri.Length - 1)) + ") ) ");

                    if (Id_Destinazione != 0)
                    {
                        string parId_Destrinazione = "@Id_Destinazione" + i;
                        parSql.Add(parId_Destrinazione, Id_Destinazione);
                        SQL_Generale.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " + parId_Destrinazione + " ");
                    }

                    // non togliere questa condizione!
                    if (Elem_Cod != 0)
                    {
                        string parElem_Cod = "@Elem_Cod" + i;
                        parSql.Add(parElem_Cod, Elem_Cod);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " + parElem_Cod + " ");
                    }


                    if (Pro_Cod != 0)
                    {
                        string parPro_Cod = "@Pro_Cod" + i;
                        parSql.Add(parPro_Cod, Pro_Cod);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " + parPro_Cod + "   ");
                    }


                    if (Mat_Cod != 0)
                    {
                        string parMat_Cod = "@Mat_Cod" + i;
                        parSql.Add(parMat_Cod, Mat_Cod);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " + parMat_Cod + " ");
                    }


                    if (Cal_Cod != 0)
                    {
                        string parCal_Cod = "@Cal_Cod" + i;
                        parSql.Add(parCal_Cod, Cal_Cod);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " + parCal_Cod + " ");
                    }


                    if (Cod_Progetto != 0 && Cod_Progetto != COSTANTI_GENERALI.CODPROGETTO_NONDEFINITO)
                    {
                        string parCod_Progetto = "@Cod_Progetto" + i;
                        parSql.Add(parCod_Progetto, Cod_Progetto);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " + parCod_Progetto + "  ");
                    }


                    if (Fase_Cod != 0)
                    {
                        string parFase_Cod = "@Fase_Cod" + i;
                        parSql.Add(parFase_Cod, Fase_Cod);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " + parFase_Cod + "  ");
                    }


                    if (Udm_Cod != 0)
                    {
                        string parUdm_Cod = "@Udm_Cod" + i;
                        parSql.Add(parUdm_Cod, Udm_Cod);
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " + parUdm_Cod + "  ");
                    }


                    if (Lotto != COSTANTI_GENERALI.LOTTO_NONDEFINITO)
                    {
                        if (isFreshAndFood && cercaLottoPerLike)
                        {
                            string parLotto = "@Lotto" + i;
                            parSql.Add(parLotto, $"%{Lotto}%");
                            SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Lotto like " + parLotto + " ");
                        }
                        else
                        {
                            string parLotto = "@Lotto" + i;
                            parSql.Add(parLotto, Lotto);
                            SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Lotto = " + parLotto + "   ");
                        }

                    }

                    /// TODO 6

                    //if (!Information.IsNothing(gruppiMerceDefaultPerCategoria) && gruppiMerceDefaultPerCategoria.Count > 0 && objParametriServer.UtenteUsername != objParametriServer.SuperUserUsername && inibisciVisibilitaGruppiMerce == false)
                    //{

                    //    // Calcolo gestione visibilità gruppi merce
                    //    Gruppi_UtenteXGruppi_Merce_R objGruppiUtenteMerce = new Gruppi_UtenteXGruppi_Merce_R(objParametriServer, objParametriUtenti);
                    //    DataTable dtGruppiUtenteMerce = objGruppiUtenteMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva IN('" + Piva + "')", "");

                    //    if (dtGruppiUtenteMerce.Rows.Count > 0)
                    //    {
                    //        SQL_Generale.AppendLine(" AND COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) IN (");
                    //        SQL_Generale.AppendLine(objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("", Piva));
                    //        SQL_Generale.AppendLine(" )");
                    //    }
                    //}

                    /// FINE TODO 6

                    if (xFiltroAggiuntivo != null)
                    {
                        SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref parSql, "xFiltro1." + i + "_"));
                    }


                    SchedeMagazzinoGestioneWhere(ref SQL_Generale, parSql, parSqlIn, Elem_Cod, i, xFiltroAggiuntivo_1, xFiltroAggiuntivo_2, xFiltroAggiuntivo_3, xFiltroAggiuntivo_4, xFiltroAggiuntivo_5, xFiltroAggiuntivo_6, xFiltroAggiuntivo_7, xFiltroAggiuntivo_8, xFiltroAggiuntivo_9, xFiltroAggiuntivo_10, xFiltroAggiuntivo_12, xFiltroAggiuntivo_13, xFiltroAggiuntivo_14, flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, xFiltroAggiuntivo_15, xFiltroAggiuntivo_16);

                    // ------------------------------------------------------ 
                    // ----------------- GROUP BY ---------------------------
                    // -----------------------------------------------------

                    SQL_Generale.AppendLine(" GROUP BY Imprese.Piva, Imprese.Rag_Soc, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, ");
                    SQL_Generale.AppendLine(" CategorieMagazzino.Tabella_Cod, CategorieMagazzino.Tabella_Des, CategorieMagazzino.Tabella_Tipo, ");
                    SQL_Generale.AppendLine(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, Imprese_Progetti.Progetto_Nome, ");
                    SQL_Generale.AppendLine(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ");
                    SQL_Generale.AppendLine(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, ");
                    SQL_Generale.AppendLine(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ");
                    SQL_Generale.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des, Centri_Aziendali.Sa_Nome,");
                    if (isFreshAndFood)
                    {
                        SQL_Generale.AppendLine(" Cantina_Vasche.Identificativo, Cantina_Vasche.Insieme_Cod,  ");
                        SQL_Generale.AppendLine(" Cantina_Insiemi.insieme_des, ");
                    }
                    SQL_Generale.AppendLine(" UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ");

                    /// TODO 7

                    //if (!Information.IsNothing(gruppiMerceDefaultPerCategoria) && gruppiMerceDefaultPerCategoria.Count > 0)
                    //    SQL_Generale.AppendLine(", grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, grpMerce.Codice, grpMerce.Descrizione, #DefaultGruppiMerce.Elem_Cod, #DefaultGruppiMerce.Codice, #DefaultGruppiMerce.Descrizione ");

                    /// FINE TODO 7

                    SchedeMagazzinoGestioneGroupBy(ref SQL_Generale, Elem_Cod, i, isFreshAndFood, flagRecuperaCodArticolo, leggiLinea);

                    // ------------------------------------------------------ 
                    // -------------------- HAVING --------------------------
                    // ------------------------------------------------------

                    // L'if gestisce
                    // a. Flag_QtaMaggioreZero = true, Flag_QtaNoZero = true/false filtro > 0
                    // b. Flag_QtaMaggioreZero = false, Flag_QtaNoZero = true filtro <> 0
                    // c. Flag_QtaMaggioreZero = false, Flag_QtaNoZero = false filtro non impostato
                    // Queste condizioni, si possono ricondurre ai valori dell'impostazione 180 (enum_Gestione_Giacenze) in questo modo:
                    // a. [1 SoloPresenti] (giacenza positiva)
                    // b. [non corrisponde ad un valore specifico, è una opzione aggiuntiva] (giacenza positiva o negativa)
                    // c. [0 SoloMovimentati] (giacenza positiva, negativa o neutra)
                    // [2 TuttiProdotti] (uso la query di anagrafica (cau_carico), oppure se voglio vedere la giacenza passo i filtri come nel caso c)
                    if (Flag_QtaMaggioreZero == true)
                        SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) > 0 ");
                    else if (Flag_QtaNoZero == true)
                        SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) <> 0 ");


                    SQL_Generale.AppendLine(" ) ");
                }

                // ------------------------------------------------------
                // ------------------- ORDINAMENTO ----------------------
                // ------------------------------------------------------

                if (xOrderBy != "")
                {
                    if (xOrderBy.ToUpper() != "NO")
                    {
                        //SQL_Generale.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy, objParametriServer));
                        throw new Exception("xOrderBy non supportato");

                    }

                }
                else
                {
                    if (SQL_Generale.ToString().ToLower().Contains("Descrizione_Prodotto".ToLower()))
                    {
                        SQL_Generale.AppendLine(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto ");
                    }
                    else
                    {
                        SQL_Generale.AppendLine(" ORDER BY Impresa, NomeComune ");
                    }
                }

                /// TODO

                //if (!Information.IsNothing(gruppiMerceDefaultPerCategoria) && gruppiMerceDefaultPerCategoria.Count > 0)
                //{
                //    var strSql = objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce();

                //    SQL_Generale.AppendLine("");
                //    SQL_Generale.Append(strSql.ToString());
                //}

                /// FINE TODO

                var compatibilityLevel = await _utilityDBDAL.Read_SQL_Compatibility_LevelAsync(objParametriServer);

                if (compatibilityLevel >= 150 && eseguiQuery)
                {
                    SQL_Generale.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ");
                }

                StrQuery_Output = SQL_Generale.ToString();

                if (eseguiQuery)
                {
                    dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(SQL_Generale.ToString(), parSql, parSqlIn);
                }
                else
                {
                    dt = new DataTable();
                }

            }
            // --------------------------------------------------------------------------

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                dt = null;
                throw;
            }
            finally
            {
                if (joinTempMagazzini != null && joinTempMagazzini.Count > 0)
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroFabbricati(objParametriServer);
                if (joinTempSpecie != null && joinTempSpecie.Count > 0)
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroChiaveInt(objParametriServer);
            }

            return dt;
        }

        private async Task<bool> GetFlagJoinCac(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            string nomeRoutine = "GetFlagJoinCac";
            bool flagJoinSuperUserCac = false;

            try
            {
                string joinSuperUser = await _utentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(Enum_Impostazioni_Utenti.SUPERUSER_JoinCacPivaSuperUser, 2, objParametriUtenti, objParametriServer);

                if (joinSuperUser == "1")
                    flagJoinSuperUserCac = true;
            }
            catch (Exception ex)
            {
                throw new Exception("[" + nomeRoutine + "] : " + ex.Message);
            }

            return flagJoinSuperUserCac;
        }


        #region Gestione Select

        public void SchedeMagazzinoGestioneSelect(ref StringBuilder SQL_Generale, int Elem_Cod, int i, bool isFreshAndFood = false, bool flagRecuperaCodArticolo = false, bool leggiLinea = false)
        {
            if (Elem_Cod == 0)
            {
                switch (i)
                {
                    case 1 // coadiuvanti
                   :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_1(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 2 // carburanti (vecchia gestione)
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_2(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 3 // fertilizzanti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_3(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 4 // formulati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_4(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 5 // inneschi trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_5(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 6 // insetti utili
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_6(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 7 // materie prime (incluso fert azi) tranne semilav e trasf vegetali e trasf animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_7() + Constants.vbCrLf);
                            break;
                        }

                    case 8 // trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_8(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 9 // semilavorati vegetali raccolti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_9(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 10 // trasformati vegetali CAL_COD <>0
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_10(isFreshAndFood, leggiLinea) + Constants.vbCrLf);
                            break;
                        }

                    case 11 // trasformati vegetali CAL-COD =0
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_11(leggiLinea) + Constants.vbCrLf);
                            break;
                        }

                    case 12 // confezioni prodotti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_12() + Constants.vbCrLf);
                            break;
                        }

                    case 13 // semilavorati vegetali acquistati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_13() + Constants.vbCrLf);
                            break;
                        }

                    case 14 // semilavorati vegetali importati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_14() + Constants.vbCrLf);
                            break;
                        }

                    case 15 // trasformati animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_15() + Constants.vbCrLf);
                            break;
                        }

                    case 16 // farmaci
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_16(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }
                }
            }
            else
                switch (Elem_Cod)
                {
                    case ELEM_COD.COADIUVANTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_1(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.CARBURANTI:
                        {
                            // devo leggere la tabella carburanti e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Select_2(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Select_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FERTILIZZANTI:
                        {
                            // devo leggere la tabella fertilizzanti e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Select_3(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Select_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FARMACI:
                        {
                            // devo leggere la tabella farmaci e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Select_16(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Select_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FORMULATI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_4(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INNESCHI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_5(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INSETTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_6(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMENTI:
                    case ELEM_COD.ALTRE_MATERIE:
                    case ELEM_COD.MATERIE_VEGETALI:
                    case ELEM_COD.BENI_CONFEZ_VEGETALE:
                    case ELEM_COD.SEMILAVORATI_ANIMALI:
                    case ELEM_COD.MATERIE_ANIMALI:
                    case ELEM_COD.BENI_CONFEZ_ANIMALE:
                    case ELEM_COD.MANGIMI:
                    case ELEM_COD.RICAMBI:
                    case ELEM_COD.CAT_MAG_SERVIZI_PROFESSIONALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRAPPOLE:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_8(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMILAVORATI_VEGETALI:
                        {
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Select_9(isFreshAndFood) + Constants.vbCrLf);
                            else if (i == 2)
                                SQL_Generale.Append(SchedeMagazzino_Select_13() + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Select_14() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_VEGETALI:
                        {
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Select_10(isFreshAndFood, leggiLinea) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Select_11(leggiLinea) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.CONFEZIONI_PRODOTTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_12() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_ANIMALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_15() + Constants.vbCrLf);
                            break;
                        }
                }
        }

        // ###############################################################################
        /// <summary>
        /// Select Coadiuvanti (195)
        /// </summary>
        private string SchedeMagazzino_Select_1(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------
            sqlProdottiSelect.Append(", Coadiuvante.Coad_Des AS Descrizione_Prodotto");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ");
            else
                sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ");

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Carburanti (2)
        /// </summary>
        private string SchedeMagazzino_Select_2(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------
            sqlProdottiSelect.Append(" , Carburanti.Car_Des AS Descrizione_Prodotto ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ");
            else
                sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ");

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }


        // ###############################################################################
        /// <summary>
        /// Select Fertilizzanti (3)
        /// </summary>
        private string SchedeMagazzino_Select_3(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------
            sqlProdottiSelect.AppendLine(" , Fertilizzanti.Fer_Des AS Descrizione_Prodotto");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ");
            else
                sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ");

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // '###############################################################################
        // 'Fertilizzanti aziendali
        // Private Function SchedaGiacenzeMagazzino_Select_3B() As String

        // Dim SQL_Prodotti_Select_3 As New StringBuilder

        // SQL_Prodotti_Select_3.Length = 0

        // '------------------------------------------------------ 
        // '------------------- SELECT ---------------------------
        // '------------------------------------------------------
        // SQL_Prodotti_Select_3.Append(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto, Movimenti_Dettagli.Mat_Cod AS Codice_Prodotto ")


        // Return SQL_Prodotti_Select_3.ToString

        // End Function

        // ###############################################################################
        /// <summary>
        /// Select Formulati (191)
        /// </summary>
        private string SchedeMagazzino_Select_4(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------
            sqlProdottiSelect.Append(" , Formulati.Fr_Des AS Descrizione_Prodotto ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ");
            else
                sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ");

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Inneschi Trappole (198)
        /// </summary>
        private string SchedeMagazzino_Select_5(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.Append(" , Avversita.Av_des_Vol AS Descrizione_Prodotto ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ");
            else
                sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ");

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Insetti utili (196)
        /// </summary>
        private string SchedeMagazzino_Select_6(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.Append(" , InsettiUtili.Ins_Des AS Descrizione_Prodotto ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ");
            else
                sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ");

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Materie Prime
        /// </summary>
        private string SchedeMagazzino_Select_7()
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_TecnologiaSementi, Materie_Prime.Germinabilita ");
            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Trappole (197)
        /// </summary>
        private string SchedeMagazzino_Select_8(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.Append(" , Trappole.Trap_Des AS Descrizione_Prodotto ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ");
            else
                sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ");

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Semilavorati vegetali raccolti (201)
        /// </summary>
        private string SchedeMagazzino_Select_9(bool isFreshAndFood)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            if (isFreshAndFood)
                sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome AS Descrizione_Prodotto ");
            else
                sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Semilavorati Vegetali acquistati (201)
        /// </summary>
        private string SchedeMagazzino_Select_13()
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Semilavorati Vegetali importati (201)
        /// </summary>
        private string SchedeMagazzino_Select_14()
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Trasformati Vegetali raccolti (210)
        /// </summary>
        private string SchedeMagazzino_Select_10(bool isFreshAndFood, bool leggiLinea
    )
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            if (isFreshAndFood)
                sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto");
            else
                sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            if (leggiLinea)
                sqlProdottiSelect.AppendLine(" , Materie_Prime.Linea_Cod AS Linea_Cod ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Trasformati Vegetali: bottiglie (210)
        /// </summary>
        private string SchedeMagazzino_Select_11(bool leggiLinea)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            if (leggiLinea)
                sqlProdottiSelect.AppendLine(" , Materie_Prime.Linea_Cod AS Linea_Cod ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");

            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Confezioni prodotti (400)
        /// </summary>
        private string SchedeMagazzino_Select_12()
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Trasformati animali (310)
        /// </summary>
        private string SchedeMagazzino_Select_15()
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------

            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ");
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI());
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Select Farmaci (16)
        /// </summary>
        private string SchedeMagazzino_Select_16(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiSelect = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- SELECT ---------------------------
            // ------------------------------------------------------
            sqlProdottiSelect.AppendLine(" , (Farmaci.Denominazione + ' - ' + Farmaci.Confezione) AS Descrizione_Prodotto");
            sqlProdottiSelect.AppendLine(" , Farmaci.AIC AS Cod_Articolo");

            // If flagRecuperaCodArticolo = True Then
            // sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
            // Else
            // sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
            // End If

            sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ");
            sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ");
            sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ");
            sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ");
            sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI");
            sqlProdottiSelect.AppendLine(" , '' As Extra_Str");

            return sqlProdottiSelect.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// Aggiunge select per passare prodotto OMNI

        /// </summary>
        private string SchedeMagazzino_Select_Mat_Cod_OMNI()
        {
            StringBuilder sqlMatCod_Referenza = new StringBuilder();
            sqlMatCod_Referenza.AppendLine(" ,CASE WHEN Movimenti_dettagli.Elem_Cod != " + ELEM_COD.TRASFORMATI_VEGETALI + " THEN 0 ");
            sqlMatCod_Referenza.AppendLine("  ELSE CASE WHEN Materie_Prime.Mat_Cod_Referenza != 0 THEN Materie_Prime.Mat_Cod_Referenza ");
            sqlMatCod_Referenza.AppendLine("       ELSE Movimenti_dettagli.Mat_Cod ");
            sqlMatCod_Referenza.AppendLine("       END ");
            sqlMatCod_Referenza.AppendLine("  END AS Mat_Cod_OMNI");

            return sqlMatCod_Referenza.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// Aggiunge select per passare giacenza quando non ho FF oppure ho categoria precisa (No FF)

        /// </summary>
        private string SchedeMagazzino_Select_Giacenza_CategorieNoFF(bool isFreshAndFood)
        {
            StringBuilder sqlGiacenza = new StringBuilder();
            sqlGiacenza.AppendLine(" , CONVERT(decimal(38,5), SUM( ");
            sqlGiacenza.AppendLine("           CASE WHEN Movimenti.CAU_MOV In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') ");
            sqlGiacenza.AppendLine("                THEN -(Mov_Destinazioni.qta)");
            sqlGiacenza.AppendLine("           ELSE Mov_Destinazioni.qta");
            sqlGiacenza.AppendLine("           END) ");
            sqlGiacenza.AppendLine("   ) AS Giacenza ");

            if (isFreshAndFood)
            {
                // Mi è arrivato questo parametro, quindi anche se sono non in categoria FF,
                // per evitare casini inizializzo cmq le colonne altrimenti potrebbe andare in errore
                sqlGiacenza.AppendLine(" , 0 AS NrConfezioni ");
                sqlGiacenza.AppendLine(" , 0 AS NrContenitori ");
                sqlGiacenza.AppendLine(" , 0 AS NrImballaggi ");
            }

            return sqlGiacenza.ToString();
        }

        public void SchedeMagazzinoGestioneSelectGiacenza(ref StringBuilder SQL_Generale, int Elem_Cod, int i, bool isFreshAndFood = false)
        {
            if (Elem_Cod == 0)
            {
                switch (i)
                {
                    case 1 // coadiuvanti
                   :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 2 // carburanti (vecchia gestione)
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 3 // fertilizzanti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 4 // formulati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 5 // inneschi trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 6 // insetti utili
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 7 // materie prime (incluso fert azi) tranne semilav e trasf vegetali e trasf animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 8 // trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 9 // semilavorati vegetali raccolti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 10 // trasformati vegetali CAL_COD <>0
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 11 // trasformati vegetali CAL-COD =0
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 12 // confezioni prodotti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 13 // semilavorati vegetali acquistati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 14 // semilavorati vegetali importati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 15 // trasformati animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 16 // farmaci
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }
                }
            }
            else
                switch (Elem_Cod)
                {
                    case ELEM_COD.TRASFORMATI_ANIMALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMILAVORATI_VEGETALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_VEGETALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    default:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }
                }
        }

        private string SchedeMagazzino_Select_Giacenza_CategorieSiFF(bool isFreshAndFood)
        {
            StringBuilder sqlGiacenza = new StringBuilder();

            if (isFreshAndFood)
            {
                sqlGiacenza.AppendLine(" , CONVERT(decimal(38,5), SUM( ");
                sqlGiacenza.AppendLine("           CASE WHEN (Movimenti_Dettagli.Udm_Cod = 38 AND Movimenti_Dettagli.Qta_Extra <> 0) THEN ");
                sqlGiacenza.AppendLine("                CASE WHEN Movimenti.CAU_MOV In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') ");
                sqlGiacenza.AppendLine("                     THEN -(Mov_Destinazioni.qta * Movimenti_Dettagli.Qta_Extra)");
                sqlGiacenza.AppendLine("                ELSE Mov_Destinazioni.qta * Movimenti_Dettagli.Qta_Extra");
                sqlGiacenza.AppendLine("                END ");
                sqlGiacenza.AppendLine("           ELSE ");
                sqlGiacenza.AppendLine("                CASE WHEN Movimenti.CAU_MOV In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') ");
                sqlGiacenza.AppendLine("                     THEN -(Mov_Destinazioni.qta)");
                sqlGiacenza.AppendLine("                ELSE Mov_Destinazioni.qta");
                sqlGiacenza.AppendLine("                END ");
                sqlGiacenza.AppendLine("           END) ");
                sqlGiacenza.AppendLine("   ) AS Giacenza ");

                sqlGiacenza.AppendLine(" , SUM( ");
                sqlGiacenza.AppendLine("        CASE WHEN Movimenti_Dettagli.Udm_Cod = 38 THEN  ");
                sqlGiacenza.AppendLine("            CASE WHEN Movimenti.CAU_MOV In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') ");
                sqlGiacenza.AppendLine("                 THEN -(Mov_Destinazioni.qta)");
                sqlGiacenza.AppendLine("            ELSE Mov_Destinazioni.qta");
                sqlGiacenza.AppendLine("            END ");
                sqlGiacenza.AppendLine("        ELSE 0");
                sqlGiacenza.AppendLine("        END ");
                sqlGiacenza.AppendLine("   ) AS NrConfezioni ");

                sqlGiacenza.AppendLine(" , SUM( CASE WHEN Movimenti.CAU_MOV In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') ");
                sqlGiacenza.AppendLine("           THEN -(Mov_Destinazioni.Qta_Dest1)");
                sqlGiacenza.AppendLine("           ELSE Mov_Destinazioni.Qta_Dest1");
                sqlGiacenza.AppendLine("           END) AS NrContenitori ");

                sqlGiacenza.AppendLine(" , SUM( CASE WHEN Movimenti.CAU_MOV In ('" + CAU_MOV.CAU_SCARICO + "','" + CAU_MOV.CAU_CONFERIMENTO_DIVERSI + "','" + CAU_MOV.CAU_ACCETTAZIONE_BENI + "') ");
                sqlGiacenza.AppendLine("           THEN -(Mov_Destinazioni.Qta_Dest2)");
                sqlGiacenza.AppendLine("           ELSE Mov_Destinazioni.Qta_Dest2");
                sqlGiacenza.AppendLine("           END) AS NrImballaggi ");
            }
            else
                // È categoria possibile di FF, ma non c'è il flag, quindi giro normale
                sqlGiacenza.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) + Constants.vbCrLf);

            return sqlGiacenza.ToString();
        }

        #endregion


        #region Gestione Join
        public void SchedeMagazzinoGestioneJoin(ref StringBuilder SQL_Generale, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, int Elem_Cod, int i, bool isFreshAndFood = false, bool flagRecuperaCodArticolo = false, bool flagJoinSuperUserCac = false, string pivaSuperUser = "", bool joinTempSpecie = false)
        {
            if (Elem_Cod == 0)
            {
                switch (i)
                {
                    case 1 // coadiuvanti
                   :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_1(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case 2 // carburanti (vecchia gestione)
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_2(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case 3 // fertilizzanti                    
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_3(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case 4 // formulati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_4(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case 5 // inneschi trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_5(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case 6 // insetti utili
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_6(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case 7 // materie prime
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_7() + Constants.vbCrLf);
                            break;
                        }

                    case 8 // trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_8(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case 9 // semilavorati vegetali raccolti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_9(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 10 // trasformati vegetali raccolto (CAL_COD <> 0)
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_10(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 11 // trasformati vegetali bottiglie (CAL_COD = 0)
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_11() + Constants.vbCrLf);
                            break;
                        }

                    case 12 // confezioni prodotti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_12() + Constants.vbCrLf);
                            break;
                        }

                    case 13 // semilavorati vegetali acquistati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_13() + Constants.vbCrLf);
                            break;
                        }

                    case 14 // semilavorati vegetali importati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_14() + Constants.vbCrLf);
                            break;
                        }

                    case 15 // trasformati animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_15() + Constants.vbCrLf);
                            break;
                        }

                    case 16 // farmaci
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_16(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) + Constants.vbCrLf);
                            break;
                        }
                }
            }
            else
                switch (Elem_Cod)
                {
                    case ELEM_COD.COADIUVANTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_1(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.CARBURANTI:
                        {
                            // devo leggere la tabella carburanti e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Join_2(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Join_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FERTILIZZANTI:
                        {
                            // devo leggere la tabella fertilizzanti e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Join_3(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Join_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FARMACI:
                        {
                            // devo leggere la tabella farmaci e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Join_16(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Join_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FORMULATI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_4(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INNESCHI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_5(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INSETTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_6(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMENTI:
                    case ELEM_COD.ALTRE_MATERIE:
                    case ELEM_COD.MATERIE_VEGETALI:
                    case ELEM_COD.BENI_CONFEZ_VEGETALE:
                    case ELEM_COD.SEMILAVORATI_ANIMALI:
                    case ELEM_COD.MATERIE_ANIMALI:
                    case ELEM_COD.BENI_CONFEZ_ANIMALE:
                    case ELEM_COD.MANGIMI:
                    case ELEM_COD.RICAMBI:
                    case ELEM_COD.CAT_MAG_SERVIZI_PROFESSIONALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_7(joinTempSpecie ) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRAPPOLE:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_8(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMILAVORATI_VEGETALI:
                        {
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Join_9(isFreshAndFood) + Constants.vbCrLf);
                            else if (i == 2)
                                SQL_Generale.Append(SchedeMagazzino_Join_13() + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Join_14() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_VEGETALI:
                        {
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_Join_10(isFreshAndFood, joinTempSpecie) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_Join_11(joinTempSpecie) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.CONFEZIONI_PRODOTTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_12() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_ANIMALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Join_15() + Constants.vbCrLf);
                            break;
                        }
                }
        }


        // ###############################################################################
        /// <summary>
        /// Join Coadiuvanti (195)
        /// </summary>
        private string SchedeMagazzino_Join_1(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - COADIUVANTI
            sqlProdottiJoin.Append(" INNER JOIN Coadiuvante ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Coadiuvante.Coad_Cod " + Constants.vbCrLf);

            if (flagRecuperaCodArticolo == true)
            {
                List<int> filtroElemCod = new List<int>() { ELEM_COD.COADIUVANTI };
                sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(filtroElemCod, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn, "1"));
            }

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Carburanti (2)
        /// </summary>
        private string SchedeMagazzino_Join_2(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - CARBURANTI
            sqlProdottiJoin.Append(" INNER JOIN Carburanti ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Carburanti.Car_Cod " + Constants.vbCrLf);

            if (flagRecuperaCodArticolo == true)
            {
                List<int> filtroElemCod = new List<int>() { ELEM_COD.CARBURANTI };
                sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(filtroElemCod, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn, "2"));
            }


            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join fertilizzanti (3)
        /// </summary>
        private string SchedeMagazzino_Join_3(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - FERTILIZZANTI
            sqlProdottiJoin.Append(" INNER JOIN Fertilizzanti ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod " + Constants.vbCrLf);

            if (flagRecuperaCodArticolo == true)
            {
                List<int> filtroElemCod = new List<int>() { ELEM_COD.FERTILIZZANTI };
                sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(filtroElemCod, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn, "3"));
            }


            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Formulati (191)
        /// </summary>
        private string SchedeMagazzino_Join_4(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - FORMULATI
            sqlProdottiJoin.Append(" INNER JOIN Formulati ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod " + Constants.vbCrLf);

            if (flagRecuperaCodArticolo == true)
            {
                List<int> filtroElemCod = new List<int>() { ELEM_COD.FORMULATI };
                sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(filtroElemCod, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn, "4"));
            }

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Inneschi trappole (198)
        /// </summary>
        private string SchedeMagazzino_Join_5(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - INNESCHI TRAPPOLE
            sqlProdottiJoin.Append(" INNER JOIN Avversita ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Avversita.Av_Cod " + Constants.vbCrLf);

            if (flagRecuperaCodArticolo == true)
            {
                List<int> filtroElemCod = new List<int>() { ELEM_COD.INNESCHI };
                sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(filtroElemCod, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn, "5"));
            }


            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Insetti utili (196)
        /// </summary>
        private string SchedeMagazzino_Join_6(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - INSETTI UTILI
            sqlProdottiJoin.Append(" INNER JOIN InsettiUtili ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = InsettiUtili.Ins_Cod " + Constants.vbCrLf);

            if (flagRecuperaCodArticolo == true)
            {
                List<int> filtroElemCod = new List<int>() { ELEM_COD.INSETTI };
                sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(filtroElemCod, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn, "6"));
            }


            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Materie Prime
        /// </summary>
        private string SchedeMagazzino_Join_7(bool joinTempSpecie = false)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            if (joinTempSpecie)
                sqlProdottiJoin.Append(" INNER JOIN #TempChiaveInt AS tempSpecie ON tempSpecie.Chiave = Materie_Prime.Veg_Cod ");

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Trappole (197)
        /// </summary>
        private string SchedeMagazzino_Join_8(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - TRAPPOLE
            sqlProdottiJoin.Append(" INNER JOIN Trappole ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Trappole.Trap_Cod " + Constants.vbCrLf);

            if (flagRecuperaCodArticolo == true)
            {
                List<int> filtroElemCod = new List<int>() { ELEM_COD.TRAPPOLE };
                sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(filtroElemCod, flagJoinSuperUserCac, pivaSuperUser, parSql, parSqlIn, "8"));
            }

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Semilavorati raccolti (201)
        /// </summary>
        private string SchedeMagazzino_Join_9(bool isFreshAndFood)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            // sqlProdottiJoin.AppendLine(" INNER JOIN Imprese_Progetti ON Movimenti_dettagli.PIVA = Imprese_Progetti.Piva AND Movimenti_dettagli.Cod_Progetto = Imprese_Progetti.Progetto_Cod ")
            if (!isFreshAndFood)
            {
                sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ");
                sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_Calibri.Cal_Cod = Materie_Prime_Campionature.Tipo_Cod ");
            }

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Semilavorati Acquistati (201)
        /// </summary>
        private string SchedeMagazzino_Join_13()
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Semilavorati importati (201)
        /// </summary>
        private string SchedeMagazzino_Join_14()
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            // sqlProdottiJoin.AppendLine(" INNER JOIN Imprese_Progetti ON Movimenti_dettagli.PIVA = Imprese_Progetti.Piva AND Movimenti_dettagli.Cod_Progetto = Imprese_Progetti.Progetto_Cod ")

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Materie Prime
        /// </summary>
        private string SchedeMagazzino_Join_10(bool isFreshAndFood, bool joinTempSpecie = false)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            if (!isFreshAndFood)
            {
                sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ");
                // sqlProdottiJoin.AppendLine(" LEFT OUTER JOIN Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ")
                sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_Calibri.Cal_Cod = Materie_Prime_Campionature.Tipo_Cod AND Materie_Prime_Campionature.Tipo = 'calibro' ");
            }

            if (joinTempSpecie)
                sqlProdottiJoin.Append(" INNER JOIN #TempChiaveInt AS tempSpecie ON tempSpecie.Chiave = Materie_Prime.Veg_Cod ");

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Trasformati Vegetali: bottiglie (210)
        /// </summary>
        private string SchedeMagazzino_Join_11(bool joinTempSpecie = false)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            if (joinTempSpecie)
                sqlProdottiJoin.Append(" INNER JOIN #TempChiaveInt AS tempSpecie ON tempSpecie.Chiave = Materie_Prime.Veg_Cod ");

            return sqlProdottiJoin.ToString();
        }


        // ###############################################################################
        /// <summary>
        /// Join Confezioni Prodotti (400)
        /// </summary>
        private string SchedeMagazzino_Join_12()
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join Trasformati Animali (310)
        /// </summary>
        private string SchedeMagazzino_Join_15()
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " + Constants.vbCrLf);

            return sqlProdottiJoin.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Join farmaci (16)
        /// </summary>
        private string SchedeMagazzino_Join_16(bool flagRecuperaCodArticolo, bool flagJoinSuperUserCac, string pivaSuperUser)
        {
            StringBuilder sqlProdottiJoin = new StringBuilder();

            // JOIN MOVIMENTI DETTAGLI - FARMACI
            sqlProdottiJoin.Append(" INNER JOIN Farmaci ");
            sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Farmaci.Farm_Cod " + Constants.vbCrLf);

            // If flagRecuperaCodArticolo = True Then
            // sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(FARMACI, flagJoinSuperUserCac, pivaSuperUser))
            // End If

            return sqlProdottiJoin.ToString();
        }


        // ###############################################################################
        /// <summary>
        /// Join su CAC_Codifica_ProdottiAziendali da aggiungere sulle altre Join per ottenere il codice articolo
        /// </summary>
        private string SchedeMagazzino_Join_CacProdotti(List<int> elemCod,
                bool flagJoinSuperUserCac,
                string pivaSuperUser,
                Dictionary<string, object> parSql,
                Dictionary<string, Dictionary<Type,
                List<object>>> parSqlIn,
                string prefix)
        {
            StringBuilder sqlCacJoin = new StringBuilder();
            string parName = "@" + prefix + "_pivaSuperUser_SchedeMagazzino_Join_CacProdotti";
            string inParName = "@" + prefix + "_SchedeMagazzino_Join_CacProdotti";
            parSqlIn.Add(inParName, FormatClauseIn(elemCod));
            // filtro ulteriormente per elem cod, perché così se non c'è alcuna codifica per quella categoria faccio meno lavoro
            sqlCacJoin.AppendLine(" OUTER APPLY ( ");
            sqlCacJoin.AppendLine("     SELECT ");
            sqlCacJoin.AppendLine("         CASE  ");
            sqlCacJoin.AppendLine("             WHEN NOT EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali WHERE Elem_Cod IN (" + inParName + "))  ");
            sqlCacJoin.AppendLine("             THEN '' ");
            sqlCacJoin.AppendLine("         ELSE   ");
            sqlCacJoin.AppendLine("             STUFF((SELECT ', ' + Cod_Prodotto_Cliente ");
            sqlCacJoin.AppendLine("                  FROM CAC_Codifica_ProdottiAziendali ");
            sqlCacJoin.AppendLine("             WHERE Elem_Cod = Movimenti_Dettagli.Elem_Cod ");
            sqlCacJoin.AppendLine("             AND Codice_GIAS = Movimenti_Dettagli.Pro_Cod ");

            if (flagJoinSuperUserCac == true && pivaSuperUser != "")
            {
                parSql.Add(parName, pivaSuperUser);
                sqlCacJoin.AppendLine("             AND Piva_SuperUser = " + parName + " ");
            }
            else
            {
                sqlCacJoin.AppendLine("             AND Piva = Movimenti_Dettagli.Piva ");
            }


            sqlCacJoin.AppendLine("             FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, '') ");
            sqlCacJoin.AppendLine("         END ");
            sqlCacJoin.AppendLine("     AS Cod_Articolo ");
            sqlCacJoin.AppendLine(" ) AS CAC ");

            return sqlCacJoin.ToString();
        }

        #endregion


        #region Gestione Where

        public void SchedeMagazzinoGestioneWhere(ref StringBuilder SQL_Generale,
            Dictionary<string, object> parSql,
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn,
            int Elem_Cod,
            int i,
            FiltroAggiuntivo? xFiltroAggiuntivo_195,
            FiltroAggiuntivo? xFiltroAggiuntivo_2,
            FiltroAggiuntivo? xFiltroAggiuntivo_3,
            FiltroAggiuntivo? xFiltroAggiuntivo_191,
            FiltroAggiuntivo? xFiltroAggiuntivo_198,
            FiltroAggiuntivo? xFiltroAggiuntivo_196,
            FiltroAggiuntivo? xFiltroAggiuntivo_MP,
            FiltroAggiuntivo? xFiltroAggiuntivo_197,
            FiltroAggiuntivo? xFiltroAggiuntivo_201a,
            FiltroAggiuntivo? xFiltroAggiuntivo_210,
            FiltroAggiuntivo? xFiltroAggiuntivo_400,
            FiltroAggiuntivo? xFiltroAggiuntivo_201b = null,
            FiltroAggiuntivo? xFiltroAggiuntivo_201c = null,
            bool flagRecuperaCodArticolo = false,
            string codArticolo = "",
            bool cercaCodArticoloPerLike = false,
            FiltroAggiuntivo? xFiltroAggiuntivo_310 = null,
            FiltroAggiuntivo? xFiltroAggiuntivo_307 = null)
        {
            if (Elem_Cod == 0)
            {
                switch (i)
                {
                    case 1 // coadiuvanti
                   :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_1(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_195) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_195, ref parSql, "xFiltroAggiuntivo_195") + Constants.vbCrLf);
                            break;
                        }

                    case 2 // carburanti (vecchia gestione)
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_2_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_2) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_2, ref parSql, "xFiltroAggiuntivo_2") + Constants.vbCrLf);
                            break;
                        }

                    case 3 // fertilizzanti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_3_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_3) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_3, ref parSql, "xFiltroAggiuntivo_3") + Constants.vbCrLf);
                            break;
                        }

                    case 4 // formulati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_4(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_191) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_191, ref parSql, "xFiltroAggiuntivo_191") + Constants.vbCrLf);
                            break;
                        }

                    case 5 // inneschi trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_5(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_198) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_198, ref parSql, "xFiltroAggiuntivo_198") + Constants.vbCrLf);
                            break;
                        }

                    case 6 // insetti utili
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_6(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_196) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_196, ref parSql, "xFiltroAggiuntivo_196") + Constants.vbCrLf);
                            break;
                        }

                    case 7 // materie prime
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_7(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_MP, ref parSql, "xFiltroAggiuntivo_MP") + Constants.vbCrLf);
                            break;
                        }

                    case 8 // trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_8(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_197) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_197, ref parSql, "xFiltroAggiuntivo_197") + Constants.vbCrLf);
                            break;
                        }

                    case 9 // semilavorati veg raccolti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_9(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201a) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_201a, ref parSql, "xFiltroAggiuntivo_201a") + Constants.vbCrLf);
                            break;
                        }

                    case 10 // trasformati veg raccolto
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_10(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_210, ref parSql, "xFiltroAggiuntivo_210") + Constants.vbCrLf);
                            break;
                        }

                    case 11 // trasformati veg bottiglie
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_11(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_210, ref parSql, "xFiltroAggiuntivo_210.1") + Constants.vbCrLf);
                            break;
                        }

                    case 12 // confezioni prodotti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_12(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_400) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_400, ref parSql, "xFiltroAggiuntivo_400") + Constants.vbCrLf);
                            break;
                        }

                    case 13 // semilavorati veg acquistati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_13(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201b) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_201b, ref parSql, "xFiltroAggiuntivo_201b") + Constants.vbCrLf);
                            break;
                        }

                    case 14 // semilavorati veg importati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_14(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201c) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_201c, ref parSql, "xFiltroAggiuntivo_201c") + Constants.vbCrLf);
                            break;
                        }

                    case 15 // trasformati animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_15(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_310) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_310, ref parSql, "xFiltroAggiuntivo_310") + Constants.vbCrLf);
                            break;
                        }

                    case 16 // farmaci
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_16_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_307) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_307, ref parSql, "xFiltroAggiuntivo_307") + Constants.vbCrLf);
                            break;
                        }
                }
            }
            else
                switch (Elem_Cod)
                {
                    case ELEM_COD.COADIUVANTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_1(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_195) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_195, ref parSql, "xFiltroAggiuntivo_195") + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.CARBURANTI:
                        {
                            // devo leggere la tabella carburanti e materie prime
                            if (i == 1)
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_2_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_2) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_2, ref parSql, "xFiltroAggiuntivo_2") + Constants.vbCrLf);
                            }
                            else
                            {
                                // il where deve essere sempre il 2 perché deve cercare solo l'elem_cod dei carburanti
                                SQL_Generale.Append(SchedeMagazzino_Where_2_A(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_MP, ref parSql, "xFiltroAggiuntivo_MP") + Constants.vbCrLf);
                            }

                            break;
                        }

                    case ELEM_COD.FERTILIZZANTI:
                        {
                            // devo leggere la tabella fertilizzanti e materie prime
                            if (i == 1)
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_3_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_3) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_3, ref parSql, "xFiltroAggiuntivo_3") + Constants.vbCrLf);
                            }
                            else
                            {
                                // il where utilizza materie prime
                                SQL_Generale.Append(SchedeMagazzino_Where_3_A(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_MP, ref parSql, "xFiltroAggiuntivo_MP") + Constants.vbCrLf);
                            }

                            break;
                        }

                    case ELEM_COD.FARMACI:
                        {
                            // devo leggere la tabella farmaci e materie prime
                            if (i == 1)
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_16_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_307) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_307, ref parSql, "xFiltroAggiuntivo_307") + Constants.vbCrLf);
                            }
                            else
                            {
                                // il where utilizza materie prime
                                SQL_Generale.Append(SchedeMagazzino_Where_16_A(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_MP, ref parSql, "xFiltroAggiuntivo_MP") + Constants.vbCrLf);
                            }

                            break;
                        }

                    case ELEM_COD.FORMULATI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_4(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_191) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_191, ref parSql, "xFiltroAggiuntivo_191") + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INNESCHI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_5(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_198) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_198, ref parSql, "xFiltroAggiuntivo_198") + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INSETTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_6(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_196) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_196, ref parSql, "xFiltroAggiuntivo_196") + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMENTI:
                    case ELEM_COD.ALTRE_MATERIE:
                    case ELEM_COD.MATERIE_VEGETALI:
                    case ELEM_COD.BENI_CONFEZ_VEGETALE:
                    case ELEM_COD.SEMILAVORATI_ANIMALI:
                    case ELEM_COD.MATERIE_ANIMALI:
                    case ELEM_COD.BENI_CONFEZ_ANIMALE:
                    case ELEM_COD.MANGIMI:
                    case ELEM_COD.RICAMBI:
                    case ELEM_COD.CAT_MAG_SERVIZI_PROFESSIONALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_7(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_MP, ref parSql, "xFiltroAggiuntivo_MP") + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRAPPOLE:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_8(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_197) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_197, ref parSql, "xFiltroAggiuntivo_197") + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMILAVORATI_VEGETALI:
                        {
                            if (i == 1)
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_9(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201a) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_201a, ref parSql, "xFiltroAggiuntivo_201a") + Constants.vbCrLf);
                            }
                            else if (i == 2)
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_13(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201b) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_201b, ref parSql, "xFiltroAggiuntivo_201b") + Constants.vbCrLf);
                            }
                            else
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_14(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201c) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_201c, ref parSql, "xFiltroAggiuntivo_201c") + Constants.vbCrLf);
                            }

                            break;
                        }

                    case ELEM_COD.TRASFORMATI_VEGETALI:
                        {
                            if (i == 1)
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_10(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_210, ref parSql, "xFiltroAggiuntivo_210") + Constants.vbCrLf);
                            }
                            else
                            {
                                SQL_Generale.Append(SchedeMagazzino_Where_11(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                                //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) + Constants.vbCrLf);
                                SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_210, ref parSql, "xFiltroAggiuntivo_210") + Constants.vbCrLf);
                            }

                            break;
                        }

                    case ELEM_COD.CONFEZIONI_PRODOTTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_12(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_400) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_400, ref parSql, "xFiltroAggiuntivo_400") + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_ANIMALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_Where_15(codArticolo, cercaCodArticoloPerLike, parSql, parSqlIn) + Constants.vbCrLf);
                            //SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_310) + Constants.vbCrLf);
                            SQL_Generale.Append(FormatFiltroAggiuntivo(xFiltroAggiuntivo_310, ref parSql, "xFiltroAggiuntivo_310") + Constants.vbCrLf);
                            break;
                        }
                }
        }


        // ###############################################################################
        /// <summary>
        /// Where Coadiuvanti (195)
        /// </summary>
        private string SchedeMagazzino_Where_1(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.COADIUVANTI + " ");

            if (codArticolo != "")
            {

                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Carburanti (2) Aziendali [nuova gestione]
        /// </summary>
        private string SchedeMagazzino_Where_2_A(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.CARBURANTI + " ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Carburanti (2) Banche Dati [non sono più usati, ma serve per retro-compatibilità]
        /// </summary>
        private string SchedeMagazzino_Where_2_B(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.CARBURANTI + " ");

            if (codArticolo != "")
            {

                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Fertilizzanti (3) Aziendali [non sono più usati, ma serve per retro-compatibilità]
        /// </summary>
        private string SchedeMagazzino_Where_3_A(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.FERTILIZZANTI + " ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Fertilizzanti (3) Banche Dati [nuova gestione]
        /// </summary>
        private string SchedeMagazzino_Where_3_B(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.FERTILIZZANTI + " ");

            if (codArticolo != "")
            {

                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Formulati (191)
        /// </summary>
        private string SchedeMagazzino_Where_4(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.FORMULATI + " ");

            if (codArticolo != "")
            {

                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE  " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo =  " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Inneschi trappole (198)
        /// </summary>
        private string SchedeMagazzino_Where_5(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.INNESCHI + " ");

            if (codArticolo != "")
            {

                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Insetti utili (196)
        /// </summary>
        private string SchedeMagazzino_Where_6(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.INSETTI + " ");

            if (codArticolo != "")
            {

                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";

                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Materie Prime
        /// </summary>
        private string SchedeMagazzino_Where_7(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod IN  (" + ELEM_COD.FERTILIZZANTI + ", " + ELEM_COD.SEMENTI + ", " + ELEM_COD.ALTRE_MATERIE + ", " + ELEM_COD.MATERIE_VEGETALI + ", " + ELEM_COD.BENI_CONFEZ_VEGETALE + ", " + ELEM_COD.SEMILAVORATI_ANIMALI + ", " + ELEM_COD.MATERIE_ANIMALI + ", " + ELEM_COD.BENI_CONFEZ_ANIMALE + ", " + ELEM_COD.MANGIMI + ", " + ELEM_COD.FARMACI + ", " + ELEM_COD.CARBURANTI + ", " + ELEM_COD.RICAMBI + ", " + ELEM_COD.CAT_MAG_SERVIZI_PROFESSIONALI + " )  ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Semilavorati raccolti (201) [CAL_COD <> 0 e COD_PROGETTO <> 0]
        /// </summary>
        private string SchedeMagazzino_Where_9(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.SEMILAVORATI_VEGETALI + " ");
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod <> 0 ");
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cod_Progetto <> 0 ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Semilavorati acquistati (201) [CAL_COD = 0 e COD_PROGETTO = 0]
        /// </summary>
        private string SchedeMagazzino_Where_13(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.SEMILAVORATI_VEGETALI + " ");
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod = 0 ");
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cod_Progetto = 0 ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Semilavorati importati (201) [CAL_COD = 0 e COD_PROGETTO <> 0]
        /// </summary>
        private string SchedeMagazzino_Where_14(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.SEMILAVORATI_VEGETALI + " ");
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod = 0 ");
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cod_Progetto <> 0 ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Trasformati vegetali: Cal_Cod <> 0 (210)
        /// </summary>
        private string SchedeMagazzino_Where_10(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.TRASFORMATI_VEGETALI + " ");
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod <> 0 ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Trasformati vegetali: Cal_Cod = 0 (210)
        /// </summary>
        private string SchedeMagazzino_Where_11(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.TRASFORMATI_VEGETALI + " ");

            // sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod NOT IN ( SELECT Progressivo FROM Materie_Prime_Campionature ) ")
            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod = 0 ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Confezioni Prodotti (400)
        /// </summary>
        private string SchedeMagazzino_Where_12(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.CONFEZIONI_PRODOTTI + " ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }

                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Trasformati Animali (310)
        /// </summary>
        private string SchedeMagazzino_Where_15(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.TRASFORMATI_ANIMALI + " ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Farmaci (16) Aziendali [non sono più usati, ma serve per retro-compatibilità]
        /// </summary>
        private string SchedeMagazzino_Where_16_A(string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.FARMACI + " ");

            if (codArticolo != "")
            {
                string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                if (cercaCodArticoloPerLike == true)
                {
                    parSql.Add(parName, $"%{codArticolo}%");
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE " + parName + " ");
                }
                else
                {
                    parSql.Add(parName, codArticolo);
                    sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = " + parName + " ");
                }

            }

            return sqlProdottiWhere.ToString();
        }

        // ###############################################################################
        /// <summary>
        /// Where Farmaci (16) Banche Dati [nuova gestione]
        /// </summary>
        private string SchedeMagazzino_Where_16_B(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn)
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.FARMACI + " ");

            if (codArticolo != "")
            {
                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        // sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                        sqlProdottiWhere.AppendLine(" AND Farmaci.AIC LIKE " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        // sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                        sqlProdottiWhere.AppendLine(" AND Farmaci.AIC = " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        // '###############################################################################
        // 'ore conto terzi
        // Private Function SchedeMagazzino_Where_() As String

        // Dim SQL_Prodotti_Where_13 As New StringBuilder

        // SQL_Prodotti_Where_13.Append(" AND  (Movimenti_dettagli.Elem_Cod = " & CAT_MAG_SERVIZI_PROFESSIONALI & ") ")

        // Return SQL_Prodotti_Where_13.ToString

        // End Function

        // ###############################################################################
        /// <summary>
        /// Where Trappole (197)
        /// </summary>
        private string SchedeMagazzino_Where_8(bool flagRecuperaCodArticolo, string codArticolo, bool cercaCodArticoloPerLike, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
    )
        {
            StringBuilder sqlProdottiWhere = new StringBuilder();

            sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.TRAPPOLE + " ");

            if (codArticolo != "")
            {

                // Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
                if (flagRecuperaCodArticolo == true)
                {
                    string parName = "@" + MethodBase.GetCurrentMethod().Name + "_codArticolo";
                    if (cercaCodArticoloPerLike == true)
                    {
                        parSql.Add(parName, $"%{codArticolo}%");
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE " + parName + " ");
                    }
                    else
                    {
                        parSql.Add(parName, codArticolo);
                        sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = " + parName + " ");
                    }

                }
            }

            return sqlProdottiWhere.ToString();
        }

        #endregion


        #region Gestione GroupBy

        /// -----------------------------------------------------------------------------

        /// <summary>

        /// utilizzata per il group by delle schede di magazzino

        /// </summary>

        /// -----------------------------------------------------------------------------
        public void SchedeMagazzinoGestioneGroupBy(ref StringBuilder SQL_Generale, int Elem_Cod, int i, bool isFreshAndFood = false, bool flagRecuperaCodArticolo = false, bool leggiLinea = false)
        {
            if (Elem_Cod == 0)
            {
                switch (i)
                {
                    case 1 // coadiuvanti
                   :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_1(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 2 // carburanti (vecchia gestione)
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_2(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 3 // fertilizzanti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_3(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 4 // formulati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_4(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 5 // inneschi trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_5(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 6 // insetti utili
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_6(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 7 // materie prime (incluso fert azi) tranne semilav e trasf vegetali e trasf animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_7() + Constants.vbCrLf);
                            break;
                        }

                    case 8 // trappole
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_8(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case 9 // semilavorati vegetali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_9(isFreshAndFood) + Constants.vbCrLf);
                            break;
                        }

                    case 10 // trasformati vegetali raccolto
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_10(isFreshAndFood, leggiLinea) + Constants.vbCrLf);
                            break;
                        }

                    case 11 // trasformati vegetali bottiglie
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_11(leggiLinea) + Constants.vbCrLf);
                            break;
                        }

                    case 12 // confezioni prodotti
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_12() + Constants.vbCrLf);
                            break;
                        }

                    case 13 // semilavorati vegetali acquistati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_13() + Constants.vbCrLf);
                            break;
                        }

                    case 14 // semilavorati vegetali importati
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_14() + Constants.vbCrLf);
                            break;
                        }

                    case 15 // trasformati animali
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_15() + Constants.vbCrLf);
                            break;
                        }

                    case 16 // farmaci
             :
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_16(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }
                }
            }
            else
                switch (Elem_Cod)
                {
                    case ELEM_COD.COADIUVANTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_1(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.CARBURANTI:
                        {
                            // devo leggere la tabella carburanti e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_2(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FERTILIZZANTI:
                        {
                            // devo leggere la tabella fertilizzanti e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_3(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FARMACI:
                        {
                            // devo leggere la tabella farmaci e materie prime
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_16(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.FORMULATI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_4(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INNESCHI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_5(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.INSETTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_6(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMENTI:
                    case ELEM_COD.ALTRE_MATERIE:
                    case ELEM_COD.MATERIE_VEGETALI:
                    case ELEM_COD.BENI_CONFEZ_VEGETALE:
                    case ELEM_COD.SEMILAVORATI_ANIMALI:
                    case ELEM_COD.MATERIE_ANIMALI:
                    case ELEM_COD.BENI_CONFEZ_ANIMALE:
                    case ELEM_COD.MANGIMI:
                    case ELEM_COD.RICAMBI:
                    case ELEM_COD.CAT_MAG_SERVIZI_PROFESSIONALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_7() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRAPPOLE:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_8(flagRecuperaCodArticolo) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.SEMILAVORATI_VEGETALI:
                        {
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_9(isFreshAndFood) + Constants.vbCrLf);
                            else if (i == 2)
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_13() + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_14() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_VEGETALI:
                        {
                            if (i == 1)
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_10(isFreshAndFood, leggiLinea) + Constants.vbCrLf);
                            else
                                SQL_Generale.Append(SchedeMagazzino_GroupBy_11(leggiLinea) + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.CONFEZIONI_PRODOTTI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_12() + Constants.vbCrLf);
                            break;
                        }

                    case ELEM_COD.TRASFORMATI_ANIMALI:
                        {
                            SQL_Generale.Append(SchedeMagazzino_GroupBy_15() + Constants.vbCrLf);
                            break;
                        }
                }
        }


        // ###############################################################################
        /// <summary>

        /// GroupBy Coadiuvanti (195)

        /// </summary>
        private string SchedeMagazzino_GroupBy_1(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------
            sqlProdottiGroupBy.Append(", Coadiuvante.Coad_Des ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Carburanti (2)

        /// </summary>
        private string SchedeMagazzino_GroupBy_2(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------
            sqlProdottiGroupBy.Append(" , Carburanti.Car_Des ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Fertilizzanti (3)

        /// </summary>
        private string SchedeMagazzino_GroupBy_3(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------
            sqlProdottiGroupBy.AppendLine(" , Fertilizzanti.Fer_Des ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Formulati (191)

        /// </summary>
        private string SchedeMagazzino_GroupBy_4(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------
            sqlProdottiGroupBy.Append(" , Formulati.Fr_Des ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Inneschi (198)

        /// </summary>
        private string SchedeMagazzino_GroupBy_5(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.Append(" , Avversita.Av_des_Vol ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Insetti utili (196)

        /// </summary>
        private string SchedeMagazzino_GroupBy_6(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.Append(" , InsettiUtili.Ins_Des ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Materie Prime

        /// </summary>
        private string SchedeMagazzino_GroupBy_7()
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_TecnologiaSementi, Materie_Prime.Germinabilita ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Trappole (197)

        /// </summary>
        private string SchedeMagazzino_GroupBy_8(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.Append(" , Trappole.Trap_Des ");

            if (flagRecuperaCodArticolo == true)
                sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Semilavorati vegetali raccolti (201)

        /// </summary>
        private string SchedeMagazzino_GroupBy_9(bool isFreshAndFood)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            if (isFreshAndFood)
                sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome ");
            else
                sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Semilavorati vegetali acquistati (201)

        /// </summary>
        private string SchedeMagazzino_GroupBy_13()
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Semilavorati vegetali importati (201)

        /// </summary>
        private string SchedeMagazzino_GroupBy_14()
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Trasformati vegetali raccolti (210)

        /// </summary>
        private string SchedeMagazzino_GroupBy_10(bool isFreshAndFood, bool leggiLinea
        )
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            if (isFreshAndFood)
                sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des ");
            else
                sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            if (leggiLinea)
                sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Linea_Cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Trasformati vegetali: bottiglie (210)

        /// </summary>
        private string SchedeMagazzino_GroupBy_11(bool leggiLinea)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            if (leggiLinea)
                sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Linea_Cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }


        // ###############################################################################
        /// <summary>

        /// GroupBy Confezioni prodotti (400)

        /// </summary>
        private string SchedeMagazzino_GroupBy_12()
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Trasformati Animali (310)

        /// </summary>
        private string SchedeMagazzino_GroupBy_15()
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------

            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ");
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ");

            return sqlProdottiGroupBy.ToString();
        }

        // ###############################################################################
        /// <summary>

        /// GroupBy Farmaci (16)

        /// </summary>
        private string SchedeMagazzino_GroupBy_16(bool flagRecuperaCodArticolo)
        {
            StringBuilder sqlProdottiGroupBy = new StringBuilder();

            // ------------------------------------------------------ 
            // ------------------- GROUP BY ---------------------------
            // ------------------------------------------------------
            sqlProdottiGroupBy.AppendLine(" , Farmaci.Denominazione ");
            sqlProdottiGroupBy.AppendLine(" , Farmaci.Confezione ");
            sqlProdottiGroupBy.AppendLine(" , Farmaci.AIC ");

            if (flagRecuperaCodArticolo == true)
            {
            }

            return sqlProdottiGroupBy.ToString();
        }

        #endregion
    }
}
