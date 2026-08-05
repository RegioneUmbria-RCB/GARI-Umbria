Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ContrattiXImpreseXParticelle_R : Inherits DataProvider

    Public Function Leggi(ByVal PIVA As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov_Det As Integer,
                          ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContrattiXImpreseXParticelle_R.Leggi()"

        If IsNothing(Particella) Then
            Particella = New ContrattiXImpreseXParticelle_Particella
        End If

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT ContrattiXImpreseXParticelle.ID ")
                    strSql.AppendLine(" FROM ContrattiXImpreseXParticelle ")

                    'Where ContrattiXImpreseXParticelle

                    ApplicaWhereContrattiXImpreseXParticelle(strSql,
                                                             True,
                                                             PIVA,
                                                             Sa_Cod,
                                                             Id_Agenda,
                                                             Id_Mov_Det,
                                                             Particella,
                                                             xFiltroAggiuntivo,
                                                             objParametri)

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT ContrattiXImpreseXParticelle.* ")
                    strSql.AppendLine(" FROM ContrattiXImpreseXParticelle ")

                    'Where ContrattiXImpreseXParticelle

                    ApplicaWhereContrattiXImpreseXParticelle(strSql,
                                                             True,
                                                             PIVA,
                                                             Sa_Cod,
                                                             Id_Agenda,
                                                             Id_Mov_Det,
                                                             Particella,
                                                             xFiltroAggiuntivo,
                                                             objParametri)

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT ContrattiXImpreseXParticelle.* ")
                    strSql.AppendLine(" , ISTAT.LOCALITA")
                    strSql.AppendLine(" , ISTAT.COMUNI_PROV ")
                    strSql.AppendLine(" , ParticelleCatastali.ETTARI ")
                    strSql.AppendLine(" , ParticelleCatastali.ARE ")
                    strSql.AppendLine(" , ParticelleCatastali.CENTIARE ")
                    strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_DOMINICALE,0) as REDDITO_DOMINICALE_CLASS ")
                    strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_AGRARIO,0) as REDDITO_AGRARIO_CLASS ")
                    strSql.AppendLine(" FROM ContrattiXImpreseXParticelle ")

                    'Join ISTAT

                    strSql.AppendLine(" LEFT JOIN ISTAT ON ")
                    strSql.AppendLine("     ISTAT.PROV = ContrattiXImpreseXParticelle.PROV ")
                    strSql.AppendLine(" AND ISTAT.COM = ContrattiXImpreseXParticelle.COM ")

                    'Join ParticelleCatastali

                    strSql.AppendLine(" LEFT JOIN ParticelleCatastali ON ")
                    strSql.AppendLine("     ParticelleCatastali.PROV = ContrattiXImpreseXParticelle.PROV ")
                    strSql.AppendLine(" AND ParticelleCatastali.COM = ContrattiXImpreseXParticelle.COM ")
                    strSql.AppendLine(" AND ParticelleCatastali.SEZIONE = ContrattiXImpreseXParticelle.SEZIONE ")
                    strSql.AppendLine(" AND ParticelleCatastali.FOGLIO = ContrattiXImpreseXParticelle.FOGLIO ")
                    strSql.AppendLine(" AND ParticelleCatastali.NUMERO = ContrattiXImpreseXParticelle.NUMERO ")
                    strSql.AppendLine(" AND ParticelleCatastali.SUBALTERNO = ContrattiXImpreseXParticelle.SUBALTERNO ")

                    ' Join ParticelleCatastaliClassamento

                    strSql.AppendLine(" LEFT JOIN ParticelleCatastaliClassamento ON ")
                    strSql.AppendLine("     ParticelleCatastaliClassamento.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND ParticelleCatastaliClassamento.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND ParticelleCatastaliClassamento.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND ParticelleCatastaliClassamento.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND ParticelleCatastaliClassamento.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND ParticelleCatastaliClassamento.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    strSql.AppendLine(" AND ParticelleCatastaliClassamento.QUALITA_COD = ParticelleCatastali.QUALITA_COD ")

                    'Where ContrattiXImpreseXParticelle

                    ApplicaWhereContrattiXImpreseXParticelle(strSql,
                                                             True,
                                                             PIVA,
                                                             Sa_Cod,
                                                             Id_Agenda,
                                                             Id_Mov_Det,
                                                             Particella,
                                                             xFiltroAggiuntivo,
                                                             objParametri)

            End Select

            'Order by

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY ")
                strSql.AppendLine("   ContrattiXImpreseXParticelle.Piva ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.Sa_Cod ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.Id_Agenda ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.Id_Mov_Det ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.PROV ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.COM ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.SEZIONE ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.FOGLIO ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.NUMERO ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.SUBALTERNO ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function Estrazione_CatastoAffitti_All(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String, ByVal centriAziendali As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Estrazione_CatastoAffitti_All()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim codParticella As Integer = enum_CodiciAnagrafe.CodiceParticella
        Dim codAppezzaBio As Integer = enum_CodiciAnagrafe.Codice_Appezza_Biologico


        Try

            strSql.Length = 0

            strSql.AppendLine("WITH   contratti_CTE (")
            strSql.AppendLine("     Id_Agenda,")
            strSql.AppendLine("     PROV,")
            strSql.AppendLine("     COM,")
            strSql.AppendLine("     SEZIONE,")
            strSql.AppendLine("     FOGLIO,")
            strSql.AppendLine("     NUMERO,")
            strSql.AppendLine("     SUBALTERNO,")
            strSql.AppendLine("     Validita_Inizio,")
            strSql.AppendLine("     Validita_Fine,")
            strSql.AppendLine("     BioVincolo,")
            strSql.AppendLine("     Superficie)")
            strSql.AppendLine("AS")

            strSql.AppendLine("(SELECT DISTINCT ")
            strSql.AppendLine("		Id_Agenda,")
            strSql.AppendLine("		PROV,")
            strSql.AppendLine("		COM,")
            strSql.AppendLine("		SEZIONE,")
            strSql.AppendLine("		FOGLIO,")
            strSql.AppendLine("		NUMERO,")
            strSql.AppendLine("		SUBALTERNO,")
            strSql.AppendLine("		Validita_Inizio,")
            strSql.AppendLine("		Validita_Fine,")
            strSql.AppendLine("		BioVincolo,")
            strSql.AppendLine("		Superficie")
            strSql.AppendLine("FROM   ContrattiXImpreseXParticelle ")
            strSql.AppendLine("WHERE  piva = '" + Agro_SQL_SaveText(Trim(piva)) + "' ")

            'applica i filtri in base ai valori selezionati dalla multisel centriAziendali
            AppendFiltroCentriAziendali(strSql, centriAziendali, "ContrattiXImpreseXParticelle")

            strSql.AppendLine("AND    Validita_Inizio <= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)))
            strSql.AppendLine("AND    Validita_Fine >= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)) + " ),")

            strSql.AppendLine("reg_impianti_CTE (piva, sa_cod, appezza)")
            strSql.AppendLine("          AS (SELECT DISTINCT Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza")
            strSql.AppendLine("FROM Reg_Impianti ")

            strSql.AppendLine("INNER JOIN AppezzamentiXParticelle on")
            strSql.AppendLine("          AppezzamentiXParticelle.piva = Reg_Impianti.piva AND")
            strSql.AppendLine("          AppezzamentiXParticelle.sa_cod = Reg_Impianti.sa_cod AND")
            strSql.AppendLine("          AppezzamentiXParticelle.appezza = Reg_Impianti.appezza AND")
            strSql.AppendLine("          AppezzamentiXParticelle.Validita_Inizio <= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)) + " AND ")
            strSql.AppendLine("		     AppezzamentiXParticelle.Validita_Fine >= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)))
            strSql.AppendLine("WHERE Reg_Impianti.piva = '" + Agro_SQL_SaveText(Trim(piva)) + "' ")
            strSql.AppendLine("          AND Reg_Impianti.Validita_Inizio <= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)))
            strSql.AppendLine("          AND Reg_Impianti.Validita_Fine >= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)))
            strSql.AppendLine("          AND Reg_Impianti.cul_cod > 0)")

            strSql.AppendLine("SELECT ImpreseXParticelle.ID,")
            strSql.AppendLine("    ImpreseXParticelle_Codici.Val_Cod Cod_Particella,")
            strSql.AppendLine("	   ISNULL(ImpreseXParticelle.Validita_Inizio, '') InizioParticella,")
            strSql.AppendLine("	   ISNULL(ImpreseXParticelle.Validita_Fine, '') FineParticella,")
            strSql.AppendLine("    appezzamento_codici.val_cod Appezzamento,")
            strSql.AppendLine("    campi.Campo_Des Zona,")
            strSql.AppendLine("    movimenti.Doc_Numero_Visualizzato Numero_Reg_Contro,")
            strSql.AppendLine("	   ISNULL(movimenti.Data_Movimento, '') Data_Reg_Contr,")
            strSql.AppendLine("	   ParticelleCatastaliClassamento.REDDITO_DOMINICALE,")
            strSql.AppendLine("	   ParticelleCatastaliClassamento.REDDITO_AGRARIO,")
            strSql.AppendLine("	   ImpreseXParticelle.Sup_Condotta Sup_Cond,")
            strSql.AppendLine("	   ISNULL(ParticelleCatastali_MetodoProduzione.Validita_Inizio, '') InizioMetodo,")
            strSql.AppendLine("	   ISNULL(ParticelleCatastali_MetodoProduzione.Validita_Fine, '') FineMetodo,")

            'case per poter passare direttamente la stringa sul campo MetodoProd
            strSql.AppendLine("	   CASE")
            strSql.AppendLine("        WHEN ParticelleCatastali_MetodoProduzione.MetodoProduzione_Cod = 1 THEN '' ")
            strSql.AppendLine("        WHEN ParticelleCatastali_MetodoProduzione.MetodoProduzione_Cod = 2 THEN 'CONV'")
            strSql.AppendLine("        WHEN ParticelleCatastali_MetodoProduzione.MetodoProduzione_Cod = 3 THEN 'BIO'")
            strSql.AppendLine("    END MetodoProd,")

            strSql.AppendLine("    ISNULL(MetodoConversione.Validita_Inizio, '') InizioConversione,")
            strSql.AppendLine("    ISNULL(MetodoConversione.Validita_Fine, '') FineConversione,")
            strSql.AppendLine("	   ISNULL(contratti_cte.Validita_Inizio, '') InizioContratto,")
            strSql.AppendLine("	   ISNULL(contratti_CTE.Validita_Fine, '') FineContratto,")
            strSql.AppendLine("	   contratti_cte.superficie Sup_Contratto,")
            strSql.AppendLine("	   ParticelleCatastali.ETTARI + ")
            strSql.AppendLine("	   CAST(ParticelleCatastali.ARE as decimal) / 100 + ")
            strSql.AppendLine("	   CAST(ParticelleCatastali.CENTIARE as decimal) / 10000 Sup_Catast,")
            strSql.AppendLine("	   ISTAT_Comuni.cod_belfiore Cod_Comune,")
            strSql.AppendLine("    ImpreseXParticelle.PROV Provincia,")
            strSql.AppendLine("    ImpreseXParticelle.COM Comune,")
            strSql.AppendLine("	   ISTAT_Comuni.Descrizione Descr_Comune,")
            strSql.AppendLine("	   ImpreseXParticelle.FOGLIO Foglio,")
            strSql.AppendLine("	   ImpreseXParticelle.NUMERO Num_Particella,")
            strSql.AppendLine("    Contatti.Rag_Soc Concedente,")
            strSql.AppendLine("	   SUM(AppezzamentiXParticelle.area) sup_Arboree_Orticole_Erbacee,")
            strSql.AppendLine("	   SUM(AppezzamentiXParticelle.area) SauNoBoschi,")
            strSql.AppendLine("	   (CAST(ParticelleCatastali.ARE as decimal) / 100 + CAST(ParticelleCatastali.CENTIARE as decimal) / 10000) - ")
            strSql.Append("	       SUM(AppezzamentiXParticelle.area) TaraSatSau,")
            strSql.AppendLine("	   Movimenti.Mov_Desc LocatarioExtra,")
            strSql.AppendLine("	   Movimenti.Extra_Str RifOrdini,")
            strSql.AppendLine("	   Centri_Aziendali.sa_nome CentroAz,")
            strSql.AppendLine("	   COUNT(*) numero_righe")
            strSql.AppendLine("FROM ImpreseXParticelle")

            'JOIN ImpreseXParticelle_Codici
            strSql.AppendLine("LEFT JOIN ImpreseXParticelle_Codici ON ")
            strSql.AppendLine("            ImpreseXParticelle_Codici.ID = ImpreseXParticelle.ID AND")
            strSql.AppendLine("			   ImpreseXParticelle_Codici.Id_Cod = " + codParticella.ToString)

            'JOIN AppezzamentiXParticelle
            strSql.AppendLine("INNER JOIN AppezzamentiXParticelle on")
            strSql.AppendLine("            AppezzamentiXParticelle.PROV = ImpreseXParticelle.PROV AND")
            strSql.AppendLine("            AppezzamentiXParticelle.COM = ImpreseXParticelle.COM AND")
            strSql.AppendLine("            AppezzamentiXParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND")
            strSql.AppendLine("            AppezzamentiXParticelle.FOGLIO = ImpreseXParticelle.FOGLIO AND")
            strSql.AppendLine("            AppezzamentiXParticelle.NUMERO = ImpreseXParticelle.NUMERO AND")
            strSql.AppendLine("            AppezzamentiXParticelle.SUBALTERNO = ImpreseXParticelle.SUBALTERNO AND")
            strSql.AppendLine("		       AppezzamentiXParticelle.Validita_Inizio <= ImpreseXParticelle.Validita_Fine AND")
            strSql.AppendLine("		       AppezzamentiXParticelle.Validita_Fine >= ImpreseXParticelle.Validita_Inizio AND")
            strSql.AppendLine("            AppezzamentiXParticelle.Validita_Inizio <= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)) + " AND")
            strSql.AppendLine("		       AppezzamentiXParticelle.Validita_Fine >= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)))

            'JOIN Appezzamento
            strSql.AppendLine("INNER JOIN Appezzamento ON")
            strSql.AppendLine("            Appezzamento.PIVA = AppezzamentiXParticelle.piva AND")
            strSql.AppendLine("		       Appezzamento.SA_COD = AppezzamentiXParticelle.SA_COD AND")
            strSql.AppendLine("		       Appezzamento.APPEZZA = AppezzamentiXParticelle.APPEZZA")

            'JOIN reg_impianti_CTE
            strSql.AppendLine("INNER JOIN reg_impianti_CTE ON ")
            strSql.AppendLine("          reg_impianti_CTE.piva = AppezzamentiXParticelle.piva AND")
            strSql.AppendLine("          reg_impianti_CTE.sa_cod = AppezzamentiXParticelle.sa_cod AND")
            strSql.AppendLine("          reg_impianti_CTE.appezza = AppezzamentiXParticelle.appezza ")

            'JOIN Appezzamento_Codici
            strSql.AppendLine("LEFT JOIN Appezzamento_Codici ON")
            strSql.AppendLine("            Appezzamento_Codici.PIVA = Appezzamento.piva AND")
            strSql.AppendLine("		       Appezzamento_Codici.SA_COD = Appezzamento.SA_COD AND")
            strSql.AppendLine("		       Appezzamento_Codici.APPEZZA = Appezzamento.APPEZZA AND")
            strSql.AppendLine("            Appezzamento_Codici.id_cod = " + codAppezzaBio.ToString)

            'JOIN Campi
            strSql.AppendLine("LEFT JOIN Campi ON")
            strSql.AppendLine("            Campi.piva = Appezzamento.piva AND")
            strSql.AppendLine("		       Campi.Sa_Cod = Appezzamento.SA_COD AND")
            strSql.AppendLine("		       Campi.Campo_Cod = Appezzamento.Campo_Cod")

            'JOIN ISTAT_Comuni
            strSql.AppendLine("LEFT JOIN ISTAT_Comuni ON ")
            strSql.AppendLine("            ISTAT_Comuni.Pro_Cod_Istat = ImpreseXParticelle.PROV AND")
            strSql.AppendLine("			   ISTAT_Comuni.Com_Cod_Istat = ImpreseXParticelle.COM")

            'JOIN ParticelleCatastali
            strSql.AppendLine("LEFT JOIN ParticelleCatastali ON ")
            strSql.AppendLine("            ParticelleCatastali.PROV = ImpreseXParticelle.PROV AND")
            strSql.AppendLine("            ParticelleCatastali.COM = ImpreseXParticelle.COM AND")
            strSql.AppendLine("            ParticelleCatastali.SEZIONE = ImpreseXParticelle.SEZIONE AND")
            strSql.AppendLine("            ParticelleCatastali.FOGLIO = ImpreseXParticelle.FOGLIO AND")
            strSql.AppendLine("            ParticelleCatastali.NUMERO = ImpreseXParticelle.NUMERO AND")
            strSql.AppendLine("            ParticelleCatastali.SUBALTERNO = ImpreseXParticelle.SUBALTERNO")

            'JOIN ParticelleCatastaliClassamento
            strSql.AppendLine("LEFT JOIN ParticelleCatastaliClassamento ON")
            strSql.AppendLine("            ParticelleCatastaliClassamento.PROV = ParticelleCatastali.PROV AND")
            strSql.AppendLine("		       ParticelleCatastaliClassamento.COM = ParticelleCatastali.COM AND")
            strSql.AppendLine("		       ParticelleCatastaliClassamento.SEZIONE = ParticelleCatastali.SEZIONE AND")
            strSql.AppendLine("		       ParticelleCatastaliClassamento.FOGLIO = ParticelleCatastali.FOGLIO AND")
            strSql.AppendLine("		       ParticelleCatastaliClassamento.NUMERO = ParticelleCatastali.NUMERO AND ")
            strSql.AppendLine("		       ParticelleCatastaliClassamento.SUBALTERNO = ParticelleCatastali.SUBALTERNO AND")
            strSql.AppendLine("		       ParticelleCatastaliClassamento.QUALITA_COD = ParticelleCatastali.QUALITA_COD ")

            'JOIN ParticelleCatastali_MetodoProduzione
            strSql.AppendLine("LEFT JOIN ParticelleCatastali_MetodoProduzione ON")
            strSql.AppendLine("            ParticelleCatastali_MetodoProduzione.PROV = ImpreseXParticelle.PROV AND")
            strSql.AppendLine("            ParticelleCatastali_MetodoProduzione.COM = ImpreseXParticelle.COM AND")
            strSql.AppendLine("            ParticelleCatastali_MetodoProduzione.SEZIONE = ImpreseXParticelle.SEZIONE AND")
            strSql.AppendLine("            ParticelleCatastali_MetodoProduzione.FOGLIO = ImpreseXParticelle.FOGLIO AND")
            strSql.AppendLine("            ParticelleCatastali_MetodoProduzione.NUMERO = ImpreseXParticelle.NUMERO AND")
            strSql.AppendLine("            ParticelleCatastali_MetodoProduzione.SUBALTERNO = ImpreseXParticelle.SUBALTERNO AND")
            strSql.AppendLine("            ParticelleCatastali_MetodoProduzione.Validita_Inizio <= ImpreseXParticelle.Validita_Fine AND")
            strSql.AppendLine("			   ParticelleCatastali_MetodoProduzione.Validita_Fine >= ImpreseXParticelle.Validita_Inizio AND")
            strSql.AppendLine("			   ParticelleCatastali_MetodoProduzione.Validita_Inizio <= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)) + " AND")
            strSql.AppendLine("			   ParticelleCatastali_MetodoProduzione.Validita_Fine >= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)))

            'ParticelleCatastali_MetodoProduzione (AS MetodoConversione)
            strSql.AppendLine("LEFT JOIN ParticelleCatastali_MetodoProduzione MetodoConversione ON")
            strSql.AppendLine("            MetodoConversione.PROV = ImpreseXParticelle.PROV AND")
            strSql.AppendLine("            MetodoConversione.COM = ImpreseXParticelle.COM AND")
            strSql.AppendLine("            MetodoConversione.SEZIONE = ImpreseXParticelle.SEZIONE AND")
            strSql.AppendLine("            MetodoConversione.FOGLIO = ImpreseXParticelle.FOGLIO AND")
            strSql.AppendLine("            MetodoConversione.NUMERO = ImpreseXParticelle.NUMERO AND")
            strSql.AppendLine("            MetodoConversione.SUBALTERNO = ImpreseXParticelle.SUBALTERNO AND ")
            strSql.AppendLine("			   MetodoConversione.MetodoProduzione_Cod = 2")

            'JOIN contratti_cte
            strSql.AppendLine("LEFT JOIN contratti_cte ON")
            strSql.AppendLine("            contratti_cte.PROV = ImpreseXParticelle.PROV AND")
            strSql.AppendLine("            contratti_cte.COM = ImpreseXParticelle.COM AND")
            strSql.AppendLine("            contratti_cte.SEZIONE = ImpreseXParticelle.SEZIONE AND")
            strSql.AppendLine("            contratti_cte.FOGLIO = ImpreseXParticelle.FOGLIO AND")
            strSql.AppendLine("            contratti_cte.NUMERO = ImpreseXParticelle.NUMERO AND")
            strSql.AppendLine("            contratti_cte.SUBALTERNO = ImpreseXParticelle.SUBALTERNO AND")
            strSql.AppendLine("			   contratti_cte.Validita_Inizio <= ImpreseXParticelle.Validita_Fine AND")
            strSql.AppendLine("			   contratti_cte.Validita_Fine >= ImpreseXParticelle.Validita_Inizio AND")
            strSql.AppendLine("			   contratti_cte.Validita_Inizio <= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)) + " AND")
            strSql.AppendLine("			   contratti_cte.Validita_Fine >= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)))

            'JOIN Agenda
            strSql.AppendLine("LEFT JOIN Agenda ON")
            strSql.AppendLine("            Agenda.id_Agenda = contratti_cte.id_agenda AND")
            strSql.AppendLine("            Agenda.piva = '" + Agro_SQL_SaveText(Trim(piva)) + "' ")

            'JOIN Movimenti
            strSql.AppendLine("LEFT JOIN Movimenti ON")
            strSql.AppendLine("            Movimenti.Id_Agenda = Agenda.Id_Agenda AND")
            strSql.AppendLine("            Movimenti.piva = '" + Agro_SQL_SaveText(Trim(piva)) + "' AND")
            strSql.AppendLine("            Movimenti.cau_mov = '" + CAU_REGISTRAZIONI.ToString + "' ")

            'JOIN Movimenti_Extra
            strSql.AppendLine("LEFT JOIN Movimenti Movimenti_Extra ON")
            strSql.AppendLine("            Movimenti_Extra.Id_Agenda = Agenda.Id_Agenda AND")
            strSql.AppendLine("            Movimenti_Extra.piva = '" + Agro_SQL_SaveText(Trim(piva)) + "' AND")
            strSql.AppendLine("            Movimenti_Extra.cau_mov = '" + CAU_REGISTRAZIONE_SECONDARIA.ToString + "' ")

            'JOIN Risorse_Umane
            strSql.AppendLine("LEFT JOIN Risorse_Umane ON")
            strSql.AppendLine("            (Risorse_Umane.piva = Movimenti.piva OR Risorse_Umane.Sa_Cod = -1) AND")
            strSql.AppendLine("            Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm")

            'JOIN Contatti
            strSql.AppendLine("LEFT JOIN Contatti ON")
            strSql.AppendLine("            (Contatti.piva = Risorse_Umane.piva OR Contatti.Sa_Cod = -1) AND")
            strSql.AppendLine("            Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")

            'JOIN Centri_Aziendali
            strSql.AppendLine("LEFT JOIN Centri_Aziendali ON")
            strSql.AppendLine("     	   Centri_Aziendali.sa_cod = ImpreseXParticelle.sa_cod")

            'inizio WHERE
            strSql.AppendLine("WHERE ImpreseXParticelle.piva = '" + Agro_SQL_SaveText(Trim(piva)) + "' ")

            'applica i filtri in base ai valori selezionati dalla multisel centriAziendali
            AppendFiltroCentriAziendali(strSql, centriAziendali, "ImpreseXParticelle")

            strSql.AppendLine("AND ImpreseXParticelle.Validita_Inizio <= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)))
            strSql.AppendLine("AND ImpreseXParticelle.Validita_Fine >= " + Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)))

            'GROUP BY
            strSql.AppendLine("GROUP BY")
            strSql.AppendLine("    ImpreseXParticelle.ID,")
            strSql.AppendLine("    ImpreseXParticelle_Codici.Val_Cod,")
            strSql.AppendLine("	   ISNULL(ImpreseXParticelle.Validita_Inizio, ''),")
            strSql.AppendLine("	   ISNULL(ImpreseXParticelle.Validita_Fine, ''),")
            strSql.AppendLine("	   appezzamento_codici.val_cod,")
            strSql.AppendLine("	   campi.Campo_Des,")
            strSql.AppendLine("	   movimenti.Doc_Numero_Visualizzato,")
            strSql.AppendLine("	   ISNULL(movimenti.Data_Movimento, ''),")
            strSql.AppendLine("	   ParticelleCatastaliClassamento.REDDITO_DOMINICALE,")
            strSql.AppendLine("	   ParticelleCatastaliClassamento.REDDITO_AGRARIO,")
            strSql.AppendLine("	   ImpreseXParticelle.Sup_Condotta,")
            strSql.AppendLine("	   ISNULL(ParticelleCatastali_MetodoProduzione.Validita_Inizio, ''),")
            strSql.AppendLine("	   ISNULL(ParticelleCatastali_MetodoProduzione.Validita_Fine, ''),")
            strSql.AppendLine("	   ParticelleCatastali_MetodoProduzione.MetodoProduzione_Cod, ")
            strSql.AppendLine("	   ISNULL(contratti_cte.Validita_Inizio, ''),")
            strSql.AppendLine("	   ISNULL(contratti_CTE.Validita_Fine, ''),")
            strSql.AppendLine("	   contratti_cte.biovincolo,")
            strSql.AppendLine("	   contratti_cte.superficie,")
            strSql.AppendLine("	   ParticelleCatastali.ETTARI + ")
            strSql.AppendLine("	   CAST(ParticelleCatastali.ARE as decimal) / 100 + ")
            strSql.AppendLine("	   CAST(ParticelleCatastali.CENTIARE as decimal) / 10000,")
            strSql.AppendLine("	   ISTAT_Comuni.cod_belfiore,")
            strSql.AppendLine("    ImpreseXParticelle.PROV,")
            strSql.AppendLine("    ImpreseXParticelle.COM,")
            strSql.AppendLine("	   ISTAT_Comuni.Descrizione,")
            strSql.AppendLine("	   ImpreseXParticelle.FOGLIO,")
            strSql.AppendLine("	   ImpreseXParticelle.NUMERO,")
            strSql.AppendLine("    Contatti.Rag_Soc,")
            strSql.AppendLine("    ISNULL(MetodoConversione.Validita_Inizio, ''),")
            strSql.AppendLine("    ISNULL(MetodoConversione.Validita_Fine, ''),")
            strSql.AppendLine("    Movimenti.Mov_Desc,")
            strSql.AppendLine("    Movimenti.Extra_Str,")
            strSql.AppendLine("    Centri_Aziendali.sa_nome,")
            strSql.AppendLine("    (CAST(ParticelleCatastali.ARE as decimal) / 100 + CAST(ParticelleCatastali.CENTIARE as decimal) / 10000)")

            strSql.AppendLine("ORDER BY Cod_Particella, InizioParticella, FineParticella")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Private Function AppendFiltroCentriAziendali(ByRef strSql As StringBuilder, ByVal centriAziendali As String, tabSql As String)

        'aggiunge i filtri solo se la stringa non è nulla
        If (centriAziendali <> "") Then

            Dim listaCentriAz = String.Join(",", centriAziendali.Split("|"))

            strSql.AppendLine("AND " + tabSql + ".sa_cod IN (" + Agro_SQL_Save_Clausola_IN(listaCentriAz) + " )")

        End If

    End Function

    Public Function Leggi_conClassamentoCodice(ByVal PIVA As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Id_Agenda As Integer,
                                               ByVal Id_Mov_Det As Integer,
                                               ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByVal TitoloPossesso As Integer? = Nothing
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContrattiXImpreseXParticelle_R.Leggi()"

        If IsNothing(Particella) Then
            Particella = New ContrattiXImpreseXParticelle_Particella
        End If

        Dim messaggioErrore As String = ""
        Dim strColonneSql As New StringBuilder
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            'Elenco colonne estratte, usato anche in group by

            strColonneSql.Length = 0
            strColonneSql.AppendLine("   ContrattiXImpreseXParticelle.PROV ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.COM ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.SEZIONE ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.FOGLIO ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.NUMERO ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.SUBALTERNO ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Superficie ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Validita_Inizio ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Validita_Fine ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.BioVincolo ")
            strColonneSql.AppendLine(" , ParticelleCatastali.ETTARI ")
            strColonneSql.AppendLine(" , ParticelleCatastali.ARE ")
            strColonneSql.AppendLine(" , ParticelleCatastali.CENTIARE ")
            strColonneSql.AppendLine(" , ISTAT.LOCALITA ")
            strColonneSql.AppendLine(" , ISTAT.COMUNI_PROV ")

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.Append(strColonneSql.ToString)
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_DOMINICALE,0) as REDDITO_DOMINICALE_CLASS ")
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_AGRARIO,0) as REDDITO_AGRARIO_CLASS ")
            '---NB: MIN/MAX ignorano i valori NULLI
            strSql.AppendLine(" , MIN(ISNULL(ImpresexParticelle_Codici.Val_Cod,'')) Cod_Particella  ")
            strSql.AppendLine(" , MAX(ImpresexParticelle_Codici.Val_Cod) Cod_Particella_Max ")
            strSql.AppendLine(" , MIN(CONCAT(ImpresexParticelle_Codici.Val_Cod,'|',ImpresexParticelle.ID)) Cod_Particella_Id ")
            strSql.AppendLine(" , MIN(ContrattiXImpreseXParticelle.Id) Id")
            '---
            strSql.AppendLine(" FROM ContrattiXImpreseXParticelle ")

            'Join ISTAT

            strSql.AppendLine(" LEFT JOIN ISTAT ON ")
            strSql.AppendLine("     ISTAT.PROV = ContrattiXImpreseXParticelle.PROV ")
            strSql.AppendLine(" AND ISTAT.COM = ContrattiXImpreseXParticelle.COM ")

            'Join ParticelleCatastali

            strSql.AppendLine(" LEFT JOIN ParticelleCatastali ON ")
            strSql.AppendLine("     ParticelleCatastali.PROV = ContrattiXImpreseXParticelle.PROV ")
            strSql.AppendLine(" AND ParticelleCatastali.COM = ContrattiXImpreseXParticelle.COM ")
            strSql.AppendLine(" AND ParticelleCatastali.SEZIONE = ContrattiXImpreseXParticelle.SEZIONE ")
            strSql.AppendLine(" AND ParticelleCatastali.FOGLIO = ContrattiXImpreseXParticelle.FOGLIO ")
            strSql.AppendLine(" AND ParticelleCatastali.NUMERO = ContrattiXImpreseXParticelle.NUMERO ")
            strSql.AppendLine(" AND ParticelleCatastali.SUBALTERNO = ContrattiXImpreseXParticelle.SUBALTERNO ")

            ' Join ParticelleCatastaliClassamento

            strSql.AppendLine(" LEFT JOIN ParticelleCatastaliClassamento ON ")
            strSql.AppendLine("     ParticelleCatastaliClassamento.PROV = ParticelleCatastali.PROV ")
            strSql.AppendLine(" AND ParticelleCatastaliClassamento.COM = ParticelleCatastali.COM ")
            strSql.AppendLine(" AND ParticelleCatastaliClassamento.SEZIONE = ParticelleCatastali.SEZIONE ")
            strSql.AppendLine(" AND ParticelleCatastaliClassamento.FOGLIO = ParticelleCatastali.FOGLIO ")
            strSql.AppendLine(" AND ParticelleCatastaliClassamento.NUMERO = ParticelleCatastali.NUMERO ")
            strSql.AppendLine(" AND ParticelleCatastaliClassamento.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
            strSql.AppendLine(" AND ParticelleCatastaliClassamento.QUALITA_COD = ParticelleCatastali.QUALITA_COD ")

            ' Join ImpresexParticelle

            strSql.AppendLine(" LEFT JOIN ImpresexParticelle ON ")
            strSql.AppendLine("     ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
            strSql.AppendLine(" AND ImpresexParticelle.COM = ParticelleCatastali.COM ")
            strSql.AppendLine(" AND ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
            strSql.AppendLine(" AND ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
            strSql.AppendLine(" AND ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
            strSql.AppendLine(" AND ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
            strSql.AppendLine(" AND ImpresexParticelle.Validita_Inizio <= ContrattiXImpreseXParticelle.Validita_Fine ")
            strSql.AppendLine(" AND ImpresexParticelle.Validita_Fine >= ContrattiXImpreseXParticelle.Validita_Inizio ")

            If Not IsNothing(TitoloPossesso) Then
                strSql.AppendLine(" AND ImpresexParticelle.TitoloPossesso = " & Agro_SQL_SaveNum(TitoloPossesso))
            End If

            ' Join ImpresexParticelle_Codici

            strSql.AppendLine(" LEFT JOIN ImpresexParticelle_Codici ON ")
            strSql.AppendLine("     ImpresexParticelle_Codici.ID = ImpresexParticelle.ID ")
            strSql.AppendLine(" AND ImpresexParticelle_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.CodiceParticella.ToString("D")))

            'Where ContrattiXImpreseXParticelle

            ApplicaWhereContrattiXImpreseXParticelle(strSql,
                                                     True,
                                                     PIVA,
                                                     Sa_Cod,
                                                     Id_Agenda,
                                                     Id_Mov_Det,
                                                     Particella,
                                                     xFiltroAggiuntivo,
                                                     objParametri)

            'Group by

            strSql.AppendLine(" GROUP BY ")
            strSql.Append(strColonneSql.ToString)
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_DOMINICALE,0) ")
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_AGRARIO,0) ")

            'Order by

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function LeggiPeriodiValidita(ByVal PIVA As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Id_Mov_Det As Integer,
                                         ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                         ByVal ListaPeriodi As List(Of ContrattiXImpreseXParticelle_Periodo),
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContrattiXImpreseXParticelle_R.LeggiPeriodiValidita()"

        If IsNothing(Particella) Then
            Particella = New ContrattiXImpreseXParticelle_Particella
        End If

        Dim messaggioErrore As String = ""
        Dim strColonneSql As New StringBuilder
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            'Elenco colonne estratte, usato anche in group by

            strColonneSql.Length = 0
            strColonneSql.AppendLine("   ContrattiXImpreseXParticelle.Piva ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Sa_Cod ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Id_Agenda ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.PROV ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.COM ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.SEZIONE ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.FOGLIO ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.NUMERO ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.SUBALTERNO ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Superficie ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Validita_Inizio ")
            strColonneSql.AppendLine(" , ContrattiXImpreseXParticelle.Validita_Fine ")

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.Append(strColonneSql.ToString)
            strSql.AppendLine(" FROM ContrattiXImpreseXParticelle ")

            'Where ContrattiXImpreseXParticelle

            ApplicaWhereContrattiXImpreseXParticelle(strSql,
                                                     False,
                                                     PIVA,
                                                     Sa_Cod,
                                                     Id_Agenda,
                                                     Id_Mov_Det,
                                                     Particella,
                                                     xFiltroAggiuntivo,
                                                     objParametri)

            If ListaPeriodi.Count > 0 Then

                strSql.AppendLine(" AND ( ")

                Dim i As Integer = 0

                For Each periodo In ListaPeriodi

                    i += 1

                    If Not IsNothing(periodo.Validita_Inizio) AndAlso Not IsNothing(periodo.Validita_Fine) Then

                        If i > 1 Then
                            strSql.AppendLine(" OR ")
                        End If

                        strSql.AppendLine(" ( ContrattiXImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(periodo.Validita_Fine) & " AND ")
                        strSql.AppendLine("   ContrattiXImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(periodo.Validita_Inizio) & " ) ")

                    End If

                Next

                strSql.AppendLine(" ) ")

            End If

            'Group by

            strSql.AppendLine(" GROUP BY ")
            strSql.Append(strColonneSql.ToString)

            'Order by

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY ")
                strSql.AppendLine("   ContrattiXImpreseXParticelle.Piva ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.Sa_Cod ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.Id_Agenda ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.PROV ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.COM ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.SEZIONE ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.FOGLIO ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.NUMERO ")
                strSql.AppendLine(" , ContrattiXImpreseXParticelle.SUBALTERNO ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Private Sub ApplicaWhereContrattiXImpreseXParticelle(ByRef strSql As StringBuilder,
                                                         ByVal ApplicaFinestraTemporale As Boolean,
                                                         ByVal PIVA As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Id_Agenda As Integer,
                                                         ByVal Id_Mov_Det As Integer,
                                                         ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByRef objParametri As AgronicaCoreParametri)

        If ApplicaFinestraTemporale Then
            strSql.AppendLine(" WHERE ContrattiXImpreseXParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
        Else
            strSql.AppendLine(" WHERE 1 = 1 ")
        End If

        If PIVA <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
        End If

        If Sa_Cod <> 0 Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If

        If Id_Agenda <> 0 Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
        End If

        If Id_Mov_Det <> 0 Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
        End If

        ApplicaWhereParticellaLettura(strSql, Particella)

        If xFiltroAggiuntivo <> "" Then
            strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If

        '--------------------------------------------------------------------------
        ' Visibilità
        '--------------------------------------------------------------------------

        Select Case objParametri.FlagVisibilita
            Case enumVisibilita.Visibilita_SoloNonCancellati
                strSql.AppendLine(" AND ContrattiXImpreseXParticelle.inviato >= 0 ")
            Case enumVisibilita.Visibilita_SoloCancellati
                strSql.AppendLine(" AND ContrattiXImpreseXParticelle.inviato = -1 ")
            Case enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select

        '--------------------------------------------------------------------------

    End Sub

    Public Sub ApplicaWhereParticellaLettura(ByRef strSql As StringBuilder,
                                             ByVal particella As ContrattiXImpreseXParticelle_Particella)

        If particella.PROV <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.PROV = '" & Agro_SQL_SaveText(Trim(particella.PROV)) & "' ")
        End If

        If particella.COM <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.COM = '" & Agro_SQL_SaveText(Trim(particella.COM)) & "' ")
        End If

        If particella.SEZIONE <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(particella.SEZIONE)) & "' ")
        End If

        If particella.FOGLIO <> 0 Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.FOGLIO =  " & Agro_SQL_SaveNum(particella.FOGLIO) & " ")
        End If

        If particella.NUMERO <> 0 Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.NUMERO =  " & Agro_SQL_SaveNum(particella.NUMERO) & " ")
        End If

        If particella.SUBALTERNO <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(particella.SUBALTERNO))) & "' ")
        End If

    End Sub

End Class

Public Class ContrattiXImpreseXParticelle_W : Inherits DataProvider

    Public Function Scrivi(ByVal PIVA As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                           ByVal Superficie As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal BioVincolo As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_Creazione As DateTime = AgroDataInizializzata,
                           Optional ByVal Data_Modifica As DateTime = AgroDataInizializzata,
                           Optional ByVal Username_Creazione As String = "",
                           Optional ByVal Username_Modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ContrattiXImpreseXParticelle_W.Scrivi()"

        If IsNothing(Particella) Then
            Particella = New ContrattiXImpreseXParticelle_Particella
        End If

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_Creazione = AgroDataInizializzata Then
            Data_Creazione = Now
        End If

        If Data_Modifica = AgroDataInizializzata Then
            Data_Modifica = Now
        End If

        If Username_Creazione = "" Then
            Username_Creazione = objParametri.UsernameOperazione
        End If

        If Username_Modifica = "" Then
            Username_Modifica = objParametri.UsernameOperazione
        End If

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO ContrattiXImpreseXParticelle ")
            strSql.AppendLine("          ( PIVA ")
            strSql.AppendLine("          , Sa_Cod ")
            strSql.AppendLine("          , Id_Agenda ")
            strSql.AppendLine("          , Id_Mov_Det ")
            strSql.AppendLine("          , PROV ")
            strSql.AppendLine("          , COM ")
            strSql.AppendLine("          , SEZIONE ")
            strSql.AppendLine("          , FOGLIO ")
            strSql.AppendLine("          , NUMERO ")
            strSql.AppendLine("          , SUBALTERNO ")
            strSql.AppendLine("          , Superficie ")
            strSql.AppendLine("          , Validita_Inizio ")
            strSql.AppendLine("          , Validita_Fine ")
            strSql.AppendLine("          , BioVincolo ")
            strSql.AppendLine("          , Inviato ")
            strSql.AppendLine("          , DataInvio ")
            strSql.AppendLine("          , Data_Creazione ")
            strSql.AppendLine("          , Data_Modifica ")
            strSql.AppendLine("          , UserName_Creazione ")
            strSql.AppendLine("          , UserName_Modifica ")
            strSql.AppendLine("          ) ")
            strSql.AppendLine("VALUES    ( ")
            strSql.AppendLine("            '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            strSql.AppendLine("          , '" & Agro_SQL_SaveText(Particella.PROV) & "' ")
            strSql.AppendLine("          , '" & Agro_SQL_SaveText(Particella.COM) & "' ")
            strSql.AppendLine("          , '" & Agro_SQL_SaveText(LCase(Particella.SEZIONE)) & "' ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveNum(Particella.FOGLIO) & " ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveNum(Particella.NUMERO) & " ")
            strSql.AppendLine("          , '" & Agro_SQL_SaveText(Trim(LCase(Particella.SUBALTERNO))) & "' ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveNum(Superficie) & " ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveNum(BioVincolo) & " ")
            strSql.AppendLine("          ,  0 " + "")
            strSql.AppendLine("          ,  Null " + "")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveDateTime(Data_Creazione) & " ")
            strSql.AppendLine("          ,  " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("          , '" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            strSql.AppendLine("          , '" & Agro_SQL_SaveText(Username_Modifica) & "' ")
            strSql.AppendLine("          )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Cancella(ByVal Id As Integer,
                             ByVal PIVA As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                             ByVal Validita_Inizio As Date?,
                             ByVal Validita_Fine As Date?,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContrattiXImpreseXParticelle_W.Cancella()"

        If IsNothing(Particella) Then
            Particella = New ContrattiXImpreseXParticelle_Particella
        End If

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM  ContrattiXImpreseXParticelle ")
            StrSQL.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If Id <> 0 Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Id = " & Agro_SQL_SaveNum(Id) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            End If

            ApplicaWhereParticellaScrittura(StrSQL, Particella)

            If Not IsNothing(Validita_Inizio) Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try


        Return xRisp

    End Function

    Public Function Modifica(ByVal Id As Integer,
                             ByVal PIVA As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                             ByVal Superficie As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal BioVincolo As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContrattiXImpreseXParticelle_W.Modifica()"

        If IsNothing(Particella) Then
            Particella = New ContrattiXImpreseXParticelle_Particella
        End If

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE ContrattiXImpreseXParticelle SET ")
            StrSQL.AppendLine("   Superficie = " & Agro_SQL_SaveNum(Superficie) & " ")
            StrSQL.AppendLine(" , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine(" , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" , BioVincolo = " & Agro_SQL_SaveNum(BioVincolo) & " ")
            StrSQL.AppendLine(" , Data_Modifica =" & Agro_SQL_SaveDate(Date.Today) & " ")
            StrSQL.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "' ")

            StrSQL.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If Id <> 0 Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Id = " & Agro_SQL_SaveNum(Id) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.AppendLine(" AND ContrattiXImpreseXParticelle.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            End If

            ApplicaWhereParticellaScrittura(StrSQL, Particella)

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Sub ApplicaWhereParticellaScrittura(ByRef strSql As StringBuilder,
                                               ByVal particella As ContrattiXImpreseXParticelle_Particella)

        If particella.PROV <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.PROV = '" & Agro_SQL_SaveText(Trim(particella.PROV)) & "' ")
        End If

        If particella.COM <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.COM = '" & Agro_SQL_SaveText(Trim(particella.COM)) & "' ")
        End If

        If particella.SEZIONE <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(particella.SEZIONE)) & "' ")
        End If

        If particella.FOGLIO <> 0 Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.FOGLIO =  " & Agro_SQL_SaveNum(particella.FOGLIO) & " ")
        End If

        If particella.NUMERO <> 0 Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.NUMERO =  " & Agro_SQL_SaveNum(particella.NUMERO) & " ")
        End If

        If particella.SUBALTERNO <> "" Then
            strSql.AppendLine(" AND ContrattiXImpreseXParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(particella.SUBALTERNO))) & "' ")
        End If

    End Sub

End Class

Public Class ContrattiXImpreseXParticelle_Particella

    Property PROV As String = ""

    Property COM As String = ""

    Property SEZIONE As String = ""

    Property FOGLIO As Integer = 0

    Property NUMERO As Integer = 0

    Property SUBALTERNO As String = ""

End Class

Public Class ContrattiXImpreseXParticelle_Periodo

    Public Validita_Inizio As Date? = Nothing
    Public Validita_Fine As Date? = Nothing

End Class