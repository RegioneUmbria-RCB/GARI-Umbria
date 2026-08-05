Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework

Public Class Mov_Destinazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Get_DataRaccolta_EfoPre_successiva_data_operazione(
                                 ByVal PIVA As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Appezza As Integer,
                                 ByVal Id_Reg As Integer,
                                 ByVal Data_Operazione As Date,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As Date

        'identifico la validita di inizio e di fine della distinta
        Dim dtImp As DataTable
        Dim objImprProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        dtImp = objImprProg.LeggiDistinta_Attiva_inData(PIVA, Sa_Cod, Appezza, Id_Reg, Data_Operazione,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", objParametri)

        Dim Data_VI_distinta, Data_VF_Distinta As Date
        Data_VI_distinta = dtImp.Rows(0).Item("Validita_Inizio")
        Data_VF_Distinta = dtImp.Rows(0).Item("Validita_Fine")


        Dim dtRaccolte As DataTable
        'la data di raccolta deve essere posteriore uguale alla data del movimento
        dtRaccolte = Leggi_Raccolte(CStr(PIVA),
                                    CInt(Sa_Cod),
                                    0, 0, 0,
                                    CInt(Appezza),
                                    CInt(Id_Reg),
                                    0, Data_VI_distinta, Data_VF_Distinta,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "Data_Movimento Asc", objParametri)

        'se ho una raccolta
        Dim dataRaccoltaXVerifica As Date = AGRODATAFINE
        If Not IsNothing(dtRaccolte) AndAlso dtRaccolte.Rows.Count > 0 Then
            Dim drR As DataRow()
            ' drR = dtRaccolte.Select("Data_Movimento >= '" & Data_Operazione & "'")

            '(26/04/2016 fede) modificato controllo x escludere lo stesso giorno
            drR = dtRaccolte.Select(String.Format("Data_Movimento > '{0}'", Data_Operazione.ToString("dd MM yyyy")))
            'drR = dtRaccolte.Select(String.Format("Data_Movimento >= '{0}'", Data_Operazione.ToString("dd MM yyyy")))

            If drR.Length > 0 Then
                'prendo la prima data di raccolta dopo la data del trattamento fatto
                dataRaccoltaXVerifica = drR(0).Item("Data_Movimento").ToShortDateString
            End If
        Else
            'verifico se non ho date di raccolte provo a controllare la data prevista
            '(26/04/2016 fede) modificato controllo x escludere lo stesso giorno
            If Not IsDBNull(dtImp.Rows(0).Item("data_fine_prevista")) AndAlso
                        IsDate(dtImp.Rows(0).Item("data_fine_prevista")) AndAlso
                        dtImp.Rows(0).Item("data_fine_prevista") > AGRODATAINIZIO AndAlso
                        dtImp.Rows(0).Item("data_fine_prevista") > Data_Operazione Then
                dataRaccoltaXVerifica = dtImp.Rows(0).Item("data_fine_prevista")
            End If
            'If Not IsDBNull(dt_imp.Rows(0).Item("data_fine_prevista")) AndAlso
            '   IsDate(dt_imp.Rows(0).Item("data_fine_prevista")) AndAlso
            '   dt_imp.Rows(0).Item("data_fine_prevista") > AGRODATAINIZIO AndAlso
            '   dt_imp.Rows(0).Item("data_fine_prevista") >= Data_Operazione Then
            '    dataRaccoltaXVerifica = dt_imp.Rows(0).Item("data_fine_prevista")
            'End If
        End If

        Return dataRaccoltaXVerifica

    End Function


    '##############################################################################################
    Public Function Leggi_Produzione(ByVal PIVA As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Appezza As Integer,
                                     ByVal Id_Destinazione As Integer,
                                     ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Produzione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim groupBy As String

        groupBy = " Agenda.Id_Agenda, Agenda.Des_Lib, Agenda.Lav_Cod, " &
             "                 Movimenti.Id_Mov, Movimenti.Data_Movimento, Movimenti.Cau_Mov, Movimenti.Mov_Desc," &
             "                 Movimenti_Dettagli.Id_Mov_Det, Movimenti_Dettagli.Elem_Cod, Movimenti_Dettagli.Pro_cod, " &
             "                 Movimenti_Dettagli.Mat_Cod, Movimenti_Dettagli.Udm_Cod,  Movimenti_Dettagli.Cal_Cod, " &
             "                 Movimenti_Dettagli.Cod_Progetto, Movimenti_Dettagli.Fase_Cod, Movimenti_Dettagli.Cod_Conto, " &
             "                 Movimenti_Dettagli.Lotto, Movimenti_Dettagli.Qta"


        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT distinct " & groupBy & "  ")
                    strSql.AppendLine(" , Sum(Reg_Impianti.Sup_Imp) as Sup_Totale ")
                    strSql.AppendLine(" FROM Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni, Reg_Impianti")
                    strSql.AppendLine(" WHERE Mov_Destinazioni.Appezza = Reg_Impianti.Appezza")
                    strSql.AppendLine(" and Reg_Impianti.Piva = Agenda.Piva")
                    strSql.AppendLine(" and Reg_Impianti.Sa_Cod = Agenda.Sa_Cod")
                    strSql.AppendLine(" and Reg_Impianti.Appezza = Mov_Destinazioni.Appezza ")
                    strSql.AppendLine(" and Reg_Impianti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")
                    strSql.AppendLine(" and Agenda.Id_Agenda = mov_Destinazioni.Id_Agenda")
                    strSql.AppendLine(" and Agenda.Id_Agenda = Movimenti.Id_Agenda")
                    strSql.AppendLine(" and Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov")
                    strSql.AppendLine(" and Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" and Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    strSql.AppendLine(" AND (Cau_Mov <> '2050' AND Cau_Mov <> '2100' AND Cau_Mov <> '2200' AND Cau_Mov <> '2300') ")
                    strSql.AppendLine(" and EXISTS ( Select Agenda.Id_Agenda from Agenda Agenda2, Mov_Destinazioni Mov_Destinazioni2")
                    strSql.AppendLine("     where Agenda2.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    strSql.AppendLine("     and   Agenda2.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                    strSql.AppendLine("     and   Agenda.Id_Agenda = Agenda2.Id_Agenda ")


                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "    ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "    ")
                    End If


                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "    ")
                    End If

                    strSql.AppendLine(" ) ")

                    strSql.AppendLine(" GROUP BY  " & groupBy)


                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Movimenti.Cau_Mov ASC") 'Importante!
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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


    '##############################################################################################
    'funzione utilizzata dall'analisi dei costi
    'l'ordinamento è importantissimo e viene passato dalla pagina web
    Public Function Leggi_Operazioni_Impianti(ByVal Query1_TempTableCreazione As String,
                                              ByVal Query2_TempTableIndice As String,
                                              ByVal Query3_TempTableFill As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Operazioni_Impianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim groupBy As String
        Dim query4TempTableJoin As String

        groupBy = " Operazioni.GRU_OP, Agenda.Id_Agenda, Agenda.Des_Lib, Agenda.Lav_Cod, " & vbCrLf &
             "                 Movimenti.Id_Mov, Movimenti.Data_Movimento, Movimenti.Cau_Mov, Movimenti.Mov_Desc," & vbCrLf &
             "                 Movimenti_Dettagli.Id_Mov_Det, Movimenti_Dettagli.Elem_Cod, Movimenti_Dettagli.Pro_cod, " & vbCrLf &
             "                 Movimenti_Dettagli.Mat_Cod, Movimenti_Dettagli.Udm_Cod,  Movimenti_Dettagli.Cal_Cod, " & vbCrLf &
             "                 Movimenti_Dettagli.Cod_Progetto, Movimenti_Dettagli.Fase_Cod, Movimenti_Dettagli.Cod_Conto, " & vbCrLf &
             "                 Movimenti_Dettagli.Lotto, Movimenti_Dettagli.Qta, Movimenti_Dettagli.Prezzo_Unitario AS Prezzo_Unitario_MovDettagli, " & vbCrLf &
             "                 Movimenti_Dettagli.Id_Attivita, Movimenti_Dettagli.Turno_Cod, " & vbCrLf &
             "                 Mov_Destinazioni.Qta as Qta_Destinazione, ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM,  Mov_Destinazioni.Piva, " & vbCrLf &
             "                 Mov_Destinazioni.sa_cod,Mov_Destinazioni.appezza,Mov_Destinazioni.id_destinazione, reg_impianti.sup_imp, " & vbCrLf &
             "                 ISNULL((SELECT TOP 1 Mov_Dettaglio_Tecnico.Sigla_AV FROM Mov_Dettaglio_Tecnico " & vbCrLf &
             "                 WHERE Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf &
             "                 And Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda  AND   Mov_Dettaglio_Tecnico.PIVA = Movimenti_dettagli.PIVA " & vbCrLf &
             "                 AND Mov_Dettaglio_Tecnico.id_mov_det = Movimenti_dettagli.Id_Mov_det),'') AS Sigla_AV, " & vbCrLf &
             "                 ISNULL((SELECT TOP 1 Mov_Dettaglio_Tecnico.Av_Cod FROM Mov_Dettaglio_Tecnico " & vbCrLf &
             "                 WHERE Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf &
             "                 And Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda  AND   Mov_Dettaglio_Tecnico.PIVA = Movimenti_dettagli.PIVA " & vbCrLf &
             "                 AND Mov_Dettaglio_Tecnico.id_mov_det = Movimenti_dettagli.Id_Mov_det),0) AS Av_Cod, " & vbCrLf &
             "                 ISNULL((SELECT SUM(Mov_Dettaglio_Tecnico.Dose) FROM Mov_Dettaglio_Tecnico " & vbCrLf &
             "                 WHERE Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf &
             "                 And Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda  AND   Mov_Dettaglio_Tecnico.PIVA = Movimenti_dettagli.PIVA " & vbCrLf &
             "                 AND Mov_Dettaglio_Tecnico.id_mov_det = Movimenti_dettagli.Id_Mov_det ),0) AS Dose " & vbCrLf

        '            "                 ,ISNULL(Mov_Dettaglio_Tecnico.Sigla_AV, '') AS Sigla_AV, ISNULL(Mov_Dettaglio_Tecnico.Av_Cod,0) AS Av_Cod, ISNULL(Mov_Dettaglio_Tecnico.Dose,0) AS Dose "

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT distinct " & groupBy & "  " & vbCrLf)

            strSql.AppendLine(" FROM  Reg_Impianti " & vbCrLf)

            'modifica del 29/12/14:
            'aggiunto join diretto di #tempimpianti con Reg_Impianti
            ' al posto dell'exists sul where
            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA
            '           --> AGGIUNGO QUESTA PARTE:
            strSql.AppendLine(" INNER JOIN  #tempimpianti " & vbCrLf)
            strSql.AppendLine(" ON Reg_Impianti.Piva = #tempimpianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            '/**************************************************

            strSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva  AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
            strSql.AppendLine(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione  " & vbCrLf)

            strSql.AppendLine(" INNER JOIN Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA " & vbCrLf)
            'commentato in data 29/12/14: non venivano letti dei costi accessori di una concimazione di bacchilega del 24/03/14
            'StrSQL.AppendLine(" AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod     " & vbCrLf)
            strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda  " & vbCrLf)
            'c'è un filtro dedicato sul where
            'StrSQL.AppendLine(" Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  "& vbCrLf )

            strSql.AppendLine(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_dettagli.PIVA  " & vbCrLf)
            'non è stato messo join su sa_cod
            strSql.AppendLine(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda  " & vbCrLf)
            strSql.AppendLine(" AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)

            strSql.AppendLine(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            strSql.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD " & vbCrLf)

            strSql.AppendLine(" LEFT OUTER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD " & vbCrLf)

            strSql.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico ON Mov_Dettaglio_Tecnico.PIVA = Movimenti_dettagli.PIVA  " & vbCrLf)
            'non è stato messo join su sa_cod
            strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda " & vbCrLf)
            strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.id_mov_det = Movimenti_dettagli.Id_Mov_det " & vbCrLf)

            'FinestraTemporaleFine in realtà non è la finestra temporale
            'contiene le date della validità dell'analisi dei costi
            strSql.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            strSql.AppendLine(" AND Cau_Mov NOT IN ( '" & CAU_SCARICO & "', '" & CAU_CARICO & "' ) " & vbCrLf)

            ' Nicoletta 16/01/2013
            strSql.AppendLine(" AND  ( " & vbCrLf)
            strSql.AppendLine("     (Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det )" & vbCrLf)
            strSql.AppendLine("     OR NOT EXISTS ( " & vbCrLf)
            strSql.AppendLine("                 SELECT 1 FROM Mov_Destinazioni MV3 " & vbCrLf)
            strSql.AppendLine("                 WHERE   MV3.Piva = Movimenti_dettagli.PIVA " & vbCrLf)
            strSql.AppendLine("                 AND     MV3.Sa_Cod = Movimenti_dettagli.Sa_Cod" & vbCrLf)
            strSql.AppendLine("                 AND     MV3.Id_Agenda = Movimenti_dettagli.Id_Agenda " & vbCrLf)
            strSql.AppendLine("                 AND     MV3.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            strSql.AppendLine("                 AND     MV3.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det" & vbCrLf)
            strSql.AppendLine("                 ) " & vbCrLf)
            strSql.AppendLine("     )" & vbCrLf)

            'StrSQL.AppendLine(" " & vbCrLf)
            'StrSQL.AppendLine(" " & vbCrLf)


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 " & vbCrLf)
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 " & vbCrLf)
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
            End Select

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If

            'commentato in data 29/12/14 messo diretto join su reg_impianti (vedi sopra)
            'StrSQL.AppendLine(" AND EXISTS ( SELECT 1 " & vbCrLf)
            'StrSQL.AppendLine("              FROM Agenda Agenda2 " & vbCrLf)
            'StrSQL.AppendLine("              INNER JOIN  Mov_Destinazioni Mov_Destinazioni2 ON Agenda.Id_Agenda = Agenda2.Id_Agenda " & vbCrLf)
            ''/**************************************************
            ''         NUOVO METODO CON TABELLA TEMPORANEA
            ''           --> AGGIUNGO QUESTA PARTE:
            'StrSQL.AppendLine(" INNER JOIN  #tempimpianti " & vbCrLf)
            'StrSQL.AppendLine(" ON #tempimpianti.Piva = Mov_Destinazioni.Piva AND Mov_Destinazioni.Sa_Cod = #tempimpianti.Sa_Cod AND  Mov_Destinazioni.Appezza = #tempimpianti.Appezza AND  Mov_Destinazioni.Id_destinazione = #tempimpianti.Id_Reg " & vbCrLf)
            ''/**************************************************
            'StrSQL.AppendLine(" ) " & vbCrLf)

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                'funzione utilizzata dall'analisi dei costi
                'l'ordinamento è importantissimo e viene passato dalla pagina web
                'occorre che sia:
                'ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Movimenti.Cau_Mov , 
                'movimenti_dettagli.elem_cod,movimenti_dettagli.mat_cod,movimenti_dettagli.Pro_Cod, 
                'Mov_Destinazioni.Piva, Mov_Destinazioni.sa_cod,Mov_Destinazioni.appezza,Mov_Destinazioni.id_destinazione
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Movimenti.Cau_Mov ASC " & vbCrLf) 'Importante!
            End If

            query4TempTableJoin = strSql.ToString

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA  
            Dim objMultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
            objMultiQuery.SettaParametriPrecedenti(Me.DammiParametriCollezionati)
            dt = objMultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                        Query2_TempTableIndice,
                                                                        Query3_TempTableFill,
                                                                        query4TempTableJoin,
                                                                        objParametri.StringaConnessione,
                                                                        messaggioErrore)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '################################################################################
    Public Function SupTot_from_IdAgenda(ByVal strIdAgenda As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.SupTot_from_IdAgenda()"

        Dim messaggioErrore As String
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Distinct Id_Agenda, SUM(Reg_Impianti.Sup_Imp*10000)/10000 as Sup_Operazione  ")
            strSql.AppendLine(" FROM  Mov_Destinazioni INNER JOIN  ")
            strSql.AppendLine(" Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND ")
            strSql.AppendLine(" Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG ")
            strSql.AppendLine(" and tipo_destinazione=0 ")
            strSql.AppendLine(" and id_agenda IN ( " & Agro_SQL_Save_Clausola_IN(strIdAgenda, False) & ") ")
            strSql.AppendLine(" group by Id_Agenda, Id_Mov, Id_Mov_Det ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Id_Agenda ASC ")
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

    ' a differenza di SupTot_from_IdAgenda, fa la somma delle superfici
    '################################################################################
    Public Function SupTot_from_IdAgenda2(ByVal strIdAgenda As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Decimal

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.SupTot_from_IdAgenda2()"
        Dim messaggioErrore As String
        Dim dt As DataTable

        Dim supTot As Decimal = 0

        Try

            dt = SupTot_from_IdAgenda(strIdAgenda, xFiltroAggiuntivo, "", objParametri)

            Dim i As Integer

            For i = 0 To dt.Rows.Count - 1
                supTot += dt.Rows(i).Item("Sup_Operazione")
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        dt.Dispose()
        dt = Nothing
        Return supTot

    End Function


    '################################################################################
    Public Function SupTotTrattata_from_IdAgenda(ByVal strIdAgenda As String,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.SupTotTrattata_from_IdAgenda()"

        Dim messaggioErrore As String
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            'StrSQL.AppendLine(" SELECT Distinct Id_Agenda, SUM(Mov_Destinazioni.Qta2) AS Sup_Operazione  ")
            strSql.AppendLine(" SELECT Distinct Id_Agenda, SUM(Mov_Destinazioni.Qta2*10000)/10000 AS Sup_Operazione  ")
            strSql.AppendLine(" FROM  Mov_Destinazioni ")
            strSql.AppendLine(" WHERE tipo_destinazione=0 ")
            strSql.AppendLine(" AND id_agenda IN ( " & Agro_SQL_Save_Clausola_IN(strIdAgenda, False) & ") ")
            strSql.AppendLine(" group by Id_Agenda, Id_Mov, Id_Mov_Det ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Id_Agenda ASC ")
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

    ' a differenza di SupTot_from_IdAgenda, fa la somma delle superfici
    '################################################################################
    Public Function SupTotTrattata_from_IdAgenda2(ByVal strIdAgenda As String,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Decimal

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.SupTotTrattata_from_IdAgenda2()"
        Dim messaggioErrore As String
        Dim dt As DataTable

        Dim supTot As Decimal = 0

        Try

            dt = SupTotTrattata_from_IdAgenda(strIdAgenda, xFiltroAggiuntivo, "", objParametri)

            Dim i As Integer

            For i = 0 To dt.Rows.Count - 1
                supTot += dt.Rows(i).Item("Sup_Operazione")
            Next

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        dt.Dispose()
        dt = Nothing
        Return supTot

    End Function

    '##############################################################################################
    Public Function Leggi_Operazioni_VegCod(ByVal PIVA As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Veg_Cod As Integer,
                                            ByVal Data As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Operazioni_VegCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Agenda.*  ")
            strSql.AppendLine(" FROM  Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND   ")
            strSql.AppendLine(" Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione AND Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND  ")
            strSql.AppendLine(" Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod INNER JOIN  ")
            strSql.AppendLine(" Agenda INNER JOIN  ")
            strSql.AppendLine(" Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD INNER JOIN  ")
            strSql.AppendLine(" Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod INNER JOIN  ")
            strSql.AppendLine(" Movimenti_dettagli ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND   ")
            strSql.AppendLine(" Movimenti.PIVA = Movimenti_dettagli.PIVA ON Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND   ")
            strSql.AppendLine(" Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA And Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            strSql.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.cul_cod = cultivar.cul_cod ")
            strSql.AppendLine(" WHERE  ")
            strSql.AppendLine(" Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data) & "  ")
            strSql.AppendLine(" AND (Cau_Mov <> '7350' AND Cau_Mov <> '7300' ) ")

            If PIVA <> "" Then
                strSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(PIVA) & "'    ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "    ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "    ")
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                ''Else
                ''    StrSQL.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Movimenti.Cau_Mov ASC") 'Importante!
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

    Public Function Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(
                                                             ByVal PIVA As String,
                                                             ByVal Sa_Cod As String,
                                                             ByVal Veg_Cod As Integer,
                                                             ByVal Data_Da As Date,
                                                             ByVal Data_A As Date,
                                                             ByVal Tipo_Operazione As enum_TipoOperazione_xAnalisiConformita,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreParametri
                                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable


        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Agenda.id_agenda, Agenda.piva, agenda.sa_cod, movimenti.Data_Movimento, agenda.Lav_Cod,agenda.des_lib  ")
            strSql.AppendLine(" , Cultivar.Veg_Cod,SpecieVegetali.Gru_Cod  ")
            strSql.AppendLine(" , movimenti.extra_int,   movimenti.num_protocollo, movimenti.mezzo ")

            Select Case Tipo_Operazione

                Case enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo

                    strSql.AppendLine(" , movimenti.doc_numero, movimenti.Disciplinare_PubblicoPrivato ")

                    strSql.AppendLine(" , ISNULL((SELECT TOP 1 Mov_Dettaglio_Tecnico.qta_ril FROM Mov_Dettaglio_Tecnico  ")
                    strSql.AppendLine("             WHERE Mov_Dettaglio_Tecnico.Id_Mov = movimenti.Id_Mov And Mov_Dettaglio_Tecnico.Id_Agenda = movimenti.Id_Agenda  And   Mov_Dettaglio_Tecnico.PIVA = movimenti.PIVA ")
                    strSql.AppendLine("             And qta_ril<>0),0) AS acqua ")

                Case enum_TipoOperazione_xAnalisiConformita.Fertilizzazioni

            End Select


            strSql.AppendLine(" FROM  Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli on Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND    Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA and Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov and Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" movimenti ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND    Movimenti.PIVA = Movimenti_dettagli.PIVA  INNER JOIN  ")
            strSql.AppendLine(" agenda on Movimenti.Id_Agenda = agenda.Id_Agenda AND    Movimenti.PIVA = agenda.PIVA  INNER JOIN  ")
            strSql.AppendLine(" Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD  INNER JOIN  ")
            strSql.AppendLine(" Cultivar ON Reg_Impianti.cul_cod = cultivar.cul_cod  INNER JOIN  ")
            strSql.AppendLine(" SpecieVegetali ON SpecieVegetali.veg_cod = cultivar.veg_cod  ")

            strSql.AppendLine(" WHERE  Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_A) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Da) & "  ")

            Select Case Tipo_Operazione

                Case enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo

                    strSql.AppendLine(" AND Cau_Mov = '2050'  ")
                    strSql.AppendLine(" AND agenda.lav_cod IN (" & LAVCOD_TRATTAMENTO_ANTIPARASSITARIO & "," & LAVCOD_CONCIA_SEME & "," & LAVCOD_GEODISINFESTAZIONE & "," & LAVCOD_DISERBO & "," & LAVCOD_DISSECCAMENTO & "," & LAVCOD_TRATTAMENTO_FITOREGOLATORE & "," & LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE & "," & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA & "," & LAVCOD_REINNESCO_TRAPPOLE & ") ")

                Case enum_TipoOperazione_xAnalisiConformita.Fertilizzazioni

                    strSql.AppendLine(" And Cau_Mov = '2300'  ")
                    strSql.AppendLine(" AND agenda.lav_cod IN (" & LAVCOD_TRATTAMENTO_ANTIBUTTERATURA & "," & LAVCOD_CONCIMAZIONE_FOGLIARE & "," & LAVCOD_DISTRIBUZIONE_AMMENDANTI & "," & LAVCOD_DISTRIBUZIONE_CONCIME & "," & LAVCOD_FERTIRRIGAZIONE & "," & LAVCOD_SARCHIATURA_CONCIMAZIONE & ") ")

                Case enum_TipoOperazione_xAnalisiConformita.Raccolta

                    strSql.AppendLine(" AND Cau_Mov = '2200'  ")
                    strSql.AppendLine(" AND agenda.lav_cod = " & LAVCOD_RACCOLTA & " ")

                Case enum_TipoOperazione_xAnalisiConformita.Magazzino

                    strSql.AppendLine(" AND Cau_Mov IN ( '2050' , '2300' , '2100' )  ")
                    strSql.AppendLine(" AND agenda.lav_cod IN (" & LAVCOD_TRATTAMENTO_ANTIPARASSITARIO & "," & LAVCOD_CONCIA_SEME & "," & LAVCOD_GEODISINFESTAZIONE & "," & LAVCOD_DISERBO & "," & LAVCOD_DISSECCAMENTO & "," & LAVCOD_TRATTAMENTO_FITOREGOLATORE & "," &
                                                            LAVCOD_TRATTAMENTO_ANTIBUTTERATURA & "," & LAVCOD_CONCIMAZIONE_FOGLIARE & "," & LAVCOD_DISTRIBUZIONE_AMMENDANTI & "," & LAVCOD_DISTRIBUZIONE_CONCIME & "," & LAVCOD_FERTIRRIGAZIONE & "," & LAVCOD_SARCHIATURA_CONCIMAZIONE & "," &
                                                            LAVCOD_CONFUSIONE_SESSUALE & "," & LAVCOD_DISORIENTAMENTO_SESSUALE & "," & LAVCOD_DISTRIBUZIONE_INSETTI & "," &
                                                            LAVCOD_CATTURE_MASSA & "," & LAVCOD_INSTALLAZIONE_TRAPPOLE & "," & LAVCOD_REINNESCO_TRAPPOLE & "," & LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE & "," & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA & ") ")

            End Select


            If PIVA <> "" Then
                strSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(PIVA) & "'    ")
            End If

            If Sa_Cod <> "" AndAlso Sa_Cod <> "0" Then
                strSql.AppendLine(" AND Agenda.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(Sa_Cod, False) & ") ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "    ")
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                ''Else
                ''    StrSQL.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Movimenti.Cau_Mov ASC") 'Importante!
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

    Public Function Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(
                                 ByVal PIVA As String,
                                 ByVal Sa_Cod As String,
                                 ByVal Veg_Cod As Integer,
                                 ByVal Data_Da As Date,
                                 ByVal Data_A As Date,
                                 ByVal Elem_Cod As Integer,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Movimenti_dettagli.pro_cod ")
            strSql.AppendLine(" FROM  Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli on Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND    Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA and Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov and Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" movimenti ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND    Movimenti.PIVA = Movimenti_dettagli.PIVA  INNER JOIN  ")
            strSql.AppendLine(" agenda on Movimenti.Id_Agenda = agenda.Id_Agenda AND    Movimenti.PIVA = agenda.PIVA  INNER JOIN  ")
            strSql.AppendLine(" Cultivar ON Reg_Impianti.cul_cod = cultivar.cul_cod  ")

            strSql.AppendLine(" WHERE  ")
            strSql.AppendLine(" Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_A) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Da) & "  ")

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "    ")
            End If

            If PIVA <> "" Then
                strSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(PIVA) & "'    ")
            End If

            If Sa_Cod <> "" AndAlso Sa_Cod <> "0" Then
                strSql.AppendLine(" AND Agenda.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(Sa_Cod, False) & ") ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "    ")
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If



            '----------------------------------------------------------------------------
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

    Public Function Leggi_ProdottiQtaSup_Utilizzati_Su_VegCod_IntervalloTemporale(
        ByVal idTestatatemp As Integer,
        ByVal PIVA As String,
        ByVal Sa_Cod As String,
        ByVal Veg_Cod As Integer,
        ByVal Data_Da As Date,
        ByVal Data_A As Date,
        ByVal Elem_Cod As Integer,
        ByVal OperazioneCorrente_FiltroImpianti As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_ProdottiQtaSup_Utilizzati_Su_VegCod_IntervalloTemporale()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim applicaFiltroImpianti As Boolean =
            (Not String.IsNullOrEmpty(OperazioneCorrente_FiltroImpianti) AndAlso idTestatatemp >= 0)

        If applicaFiltroImpianti Then
            Dim xScrivi As New AgronicaCoreContabDAL.VerificaSostenibilita_R
            xScrivi.leggiPopolaTabellaFiltroImpianti(OperazioneCorrente_FiltroImpianti, objParametri)
            PIVA = "-"
        Else
            'se non ho un filtro impianti non posso estrarre tutto il database, quindi faccio fallire la query
            If PIVA = "" Then
                PIVA = "---"
            End If

        End If

        Try

            stb.Length = 0

            stb.AppendLine(" Select quantita.*, numero.conteggio as ConteggioOperazioni ")
            stb.AppendLine(" from ( ")


            stb.AppendLine(" SELECT ii.piva, ii.rag_soc, ii.piva + ' - ' + cast(md.pro_cod as varchar(50)) as Codice, md.pro_Cod as CodiceProdotto ")
            Select Case Elem_Cod
                Case FORMULATI
                    stb.AppendLine(" ,f.fr_des as Descrizione ")
                Case FERTILIZZANTI
                    stb.AppendLine(" ,f.fer_des as Descrizione ")
            End Select
            stb.AppendLine(" ,sum(mde.qta) as sum_qta_dest, sum(mde.Qta2) as sup_tratt ")

            stb.AppendLine(" FROM movimenti_dettagli md inner join movimenti m on m.piva=md.piva and m.Id_Agenda=md.Id_Agenda and m.id_mov=md.id_mov ")
            stb.AppendLine(" inner join Mov_Destinazioni mde on mde.piva=md.piva and mde.Id_Agenda=md.Id_Agenda and mde.id_mov=md.id_mov and mde.id_mov_det=md.id_mov_det ")
            stb.AppendLine(" inner join reg_impianti r on mde.piva=r.piva and mde.sa_cod=r.sa_cod and mde.appezza=r.APPEZZA and mde.Id_Destinazione=r.id_reg ")

            stb.AppendLine(" inner join imprese ii on ii.piva = mde.piva")

            If applicaFiltroImpianti Then
                TmpFiltroImpianti(idTestatatemp, stb)
            End If

            stb.AppendLine(" inner join cultivar c on c.cul_cod=r.CUL_COD ")
            If Elem_Cod = FORMULATI Then
                stb.AppendLine(" inner join formulati f on f.Fr_Cod=md.pro_cod  ")
            Else
                stb.AppendLine(" inner join fertilizzanti f on f.Fer_Cod=md.pro_cod  ")
            End If

            Leggi_ProdottiQtaSup_Utilizzati_Su_VegCod_IntervalloTemporaleImpostFiltri(PIVA, Sa_Cod, Veg_Cod, Data_Da, Data_A, Elem_Cod, xFiltroAggiuntivo, objParametri, stb)

            stb.AppendLine(" group by ii.piva, ii.rag_soc, pro_cod ")
            Select Case Elem_Cod
                Case FORMULATI
                    stb.AppendLine(" ,fr_des ")
                Case FERTILIZZANTI
                    stb.AppendLine(" ,fer_des ")
            End Select


            stb.AppendLine(" ) as quantita")
            stb.AppendLine(" inner join (  ")

            stb.AppendLine("  Select ii.piva + ' - ' + cast(md.pro_cod as varchar(50)) as Codice, md.pro_Cod as CodiceProdotto  ")
            Select Case Elem_Cod
                Case FORMULATI
                    stb.AppendLine(" ,f.fr_des as Descrizione ")
                Case FERTILIZZANTI
                    stb.AppendLine(" ,f.fer_des as Descrizione ")
            End Select
            stb.AppendLine("  ,count(*) as conteggio ")
            stb.AppendLine(" From movimenti_dettagli md  ")
            stb.AppendLine(" inner Join( ")
            stb.AppendLine("  select distinct mde.piva, mde.Id_Agenda, Veg_Cod ")
            stb.AppendLine("     From Mov_Destinazioni mde ")
            If applicaFiltroImpianti Then
                TmpFiltroImpianti(idTestatatemp, stb)
            End If
            stb.AppendLine("  inner Join reg_impianti r on mde.piva=r.piva And mde.sa_cod=r.sa_cod And mde.appezza=r.APPEZZA And mde.Id_Destinazione=r.id_reg ")
            stb.AppendLine("     inner Join cultivar c on c.cul_cod=r.CUL_COD ")
            stb.AppendLine("   where mde.Tipo_Destinazione = 0")
            stb.AppendLine(" ) c ")
            stb.AppendLine("  On c.id_agenda = md.id_agenda")
            stb.AppendLine("  and c.piva = md.piva ")

            stb.AppendLine(" inner Join movimenti m on m.piva=md.piva And m.Id_Agenda=md.Id_Agenda And m.id_mov=md.id_mov  ")

            stb.AppendLine(" inner join imprese ii on ii.piva = m.piva")

            If Elem_Cod = FORMULATI Then
                stb.AppendLine(" inner join formulati f on f.Fr_Cod=md.pro_cod  ")
            Else
                stb.AppendLine(" inner join fertilizzanti f on f.Fer_Cod=md.pro_cod  ")
            End If


            Leggi_ProdottiQtaSup_Utilizzati_Su_VegCod_IntervalloTemporaleImpostFiltri(PIVA, Sa_Cod, Veg_Cod, Data_Da, Data_A, Elem_Cod, xFiltroAggiuntivo, objParametri, stb)

            stb.AppendLine(" group by ii.piva, ii.rag_soc, pro_cod ")
            Select Case Elem_Cod
                Case FORMULATI
                    stb.AppendLine(" ,fr_des ")
                Case FERTILIZZANTI
                    stb.AppendLine(" ,fer_des ")
            End Select

            stb.AppendLine(") as numero  ")
            stb.AppendLine("  On numero.codice=quantita.codice")


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else

                stb.AppendLine(" ORDER by rag_soc, quantita.Descrizione  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Shared Sub TmpFiltroImpianti(idTestataTemp As Integer, stb As StringBuilder)
        stb.AppendLine("  inner Join __tmp_FiltroImpianti ff ")
        stb.AppendLine("         On mde.Piva = ff.piva ")
        stb.AppendLine("      And mde.Sa_Cod = ff.sa_cod ")
        stb.AppendLine("      And mde.appezza = ff.appezza ")
        stb.AppendLine("      And mde.id_Destinazione = ff.id_reg ")
        stb.AppendLine("      And ff.idTestataTemp = " & idTestataTemp)
    End Sub

    Private Sub Leggi_ProdottiQtaSup_Utilizzati_Su_VegCod_IntervalloTemporaleImpostFiltri(PIVA As String, Sa_Cod As String, Veg_Cod As Integer, Data_Da As Date, Data_A As Date, Elem_Cod As Integer, xFiltroAggiuntivo As String, objParametri As AgronicaCoreParametri, stb As StringBuilder)
        stb.AppendLine(" WHERE  ")
        stb.AppendLine(" m.Data_Movimento <= " & Agro_SQL_SaveDate(Data_A) & " ")
        stb.AppendLine(" AND m.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Da) & "  ")

        If Elem_Cod <> 0 Then
            stb.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            Select Case Elem_Cod
                Case FORMULATI
                    stb.AppendLine(" AND Cau_Mov = '" & Agro_SQL_SaveText(CAU_TRATTAMENTO) & "' ")
                Case FERTILIZZANTI
                    stb.AppendLine(" AND Cau_Mov = '" & Agro_SQL_SaveText(CAU_LAVORAZIONE) & "' ")
            End Select
        End If

        If PIVA <> "-" Then
            stb.AppendLine(" AND m.Piva = '" & Agro_SQL_SaveText(PIVA) & "'    ")
        End If

        If Sa_Cod <> "" AndAlso Sa_Cod <> "0" Then
            stb.AppendLine(" AND m.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(Sa_Cod, False) & ") ")
        End If

        If Veg_Cod > 0 Then
            stb.AppendLine(" AND c.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "    ")
        End If


        '--------------------------------------------------------------------------
        Select Case objParametri.FlagVisibilita
            Case enumVisibilita.Visibilita_SoloNonCancellati
                stb.AppendLine(" AND   m.Inviato >=0 ")
            Case enumVisibilita.Visibilita_SoloCancellati
                stb.AppendLine(" AND   m.Inviato =-1 ")
            Case enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If
    End Sub

    Public Function VegCod_From_Agenda(ByVal Piva As String,
                                       ByVal Id_Agenda As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.VegCod_From_Agenda()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Agenda.id_agenda, SpecieVegetali.veg_cod, SpecieVegetali.veg_des  ")
            strSql.AppendLine(" FROM  Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND   ")
            strSql.AppendLine(" Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione AND Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND  ")
            strSql.AppendLine(" Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod INNER JOIN  ")
            strSql.AppendLine(" Agenda INNER JOIN  ")
            strSql.AppendLine(" Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod INNER JOIN  ")
            strSql.AppendLine(" Movimenti_dettagli ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND   ")
            strSql.AppendLine(" Movimenti.PIVA = Movimenti_dettagli.PIVA ON Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND   ")
            strSql.AppendLine(" Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA And Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            strSql.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.cul_cod = cultivar.cul_cod ")
            strSql.AppendLine(" INNER JOIN SpecieVegetali ON cultivar.veg_cod = SpecieVegetali.veg_cod ")
            strSql.AppendLine(" WHERE  Agenda.id_agenda IN (" & Agro_SQL_Save_Clausola_IN(Id_Agenda, False) & ")    ")

            '(23/11/2018 fede) aggiunto filtro piva per id_agenda doppi
            strSql.AppendLine(" AND Agenda.Piva ='" & Agro_SQL_SaveText(Piva) & "'    ")
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                ''Else
                ''    StrSQL.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Movimenti.Cau_Mov ASC") 'Importante!
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

    Public Function Leggi_Dettagli_Impianti(ByVal Piva As String,
                                            ByVal Id_Agenda As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dettagli_Impianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Mov_Destinazioni.Id_Mov, Mov_Destinazioni.Id_Mov_Det, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2,   ")
            strSql.AppendLine(" Movimenti_dettagli.Qta  AS Qta_Dett,  Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Progetto, ")
            strSql.AppendLine(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, ")
            strSql.AppendLine(" Movimenti_dettagli.Lotto,  SpecieVegetali.Veg_Des, Appezzamento.APP_NOME, Reg_Impianti.Validita_Inizio, Imprese_Progetti.Progetto_Nome, Imprese_Progetti.Stato_Impianto, GruppoFinalita.Grfi_Des, ISNULL(GruppoFinalita_1.Grfi_Des,'') AS Stato ")
            strSql.AppendLine(" , Imprese_Progetti.regolamento_cod, Imprese_Progetti.Disciplinare_Cod ")

            strSql.AppendLine(" FROM            Mov_Destinazioni  INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli  ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")

            strSql.AppendLine(" Movimenti  ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti.Piva = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  INNER JOIN ")

            strSql.AppendLine(" Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG LEFT JOIN ")
            strSql.AppendLine(" Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod LEFT JOIN ")
            strSql.AppendLine(" SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            strSql.AppendLine(" Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  ")
            strSql.AppendLine(" Reg_Impianti.APPEZZA = Appezzamento.APPEZZA INNER JOIN ")
            strSql.AppendLine(" Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND  ")
            strSql.AppendLine(" Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg LEFT JOIN ")
            strSql.AppendLine(" GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            strSql.AppendLine(" GruppoFinalita AS GruppoFinalita_1 ON Imprese_Progetti.Stato_Impianto = GruppoFinalita_1.Grfi_Cod ")

            strSql.AppendLine(" WHERE  Mov_Destinazioni.id_agenda IN (" & Agro_SQL_Save_Clausola_IN(Id_Agenda, False) & ")    ")

            strSql.AppendLine(" and Movimenti.data_movimento >= imprese_progetti.validita_inizio    ")
            strSql.AppendLine(" and Movimenti.data_movimento <= imprese_progetti.validita_fine    ")
            strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0 ")

            '(23/11/2018 fede) aggiunto filtro piva per id_agenda doppi
            strSql.AppendLine(" AND Mov_Destinazioni.Piva ='" & Agro_SQL_SaveText(Piva) & "'    ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Mov_Destinazioni.Id_Agenda")
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

    'a differenza della precedente specie, varieta e finalita sono in left join x caricare anche le destinazioni d'uso
    Public Function Leggi_Dettagli_Impianti_2(ByVal Piva As String,
                                              ByVal Id_Agenda As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dettagli_Impianti_2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Mov_Destinazioni.Id_Mov, Mov_Destinazioni.Id_Mov_Det, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Imprese_Progetti.progetto_cod, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2,   ")
            strSql.AppendLine(" Movimenti_dettagli.Qta  AS Qta_Dett,  Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Progetto, ")
            strSql.AppendLine(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, ")
            strSql.AppendLine(" Movimenti_dettagli.Lotto,  SpecieVegetali.Veg_Des, Appezzamento.APP_NOME, Reg_Impianti.Validita_Inizio, Imprese_Progetti.Progetto_Nome, Imprese_Progetti.Stato_Impianto, GruppoFinalita.Grfi_Des, ISNULL(GruppoFinalita_1.Grfi_Des,'') AS Stato ")

            strSql.AppendLine(" FROM            Mov_Destinazioni  INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli  ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG left outer JOIN ")
            strSql.AppendLine(" Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod left outer JOIN ")
            strSql.AppendLine(" SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            strSql.AppendLine(" Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  ")
            strSql.AppendLine(" Reg_Impianti.APPEZZA = Appezzamento.APPEZZA INNER JOIN ")
            strSql.AppendLine(" Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND  ")
            strSql.AppendLine(" Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg INNER JOIN ")

            strSql.AppendLine(" Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda And Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov left outer JOIN ")

            strSql.AppendLine(" GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            strSql.AppendLine(" GruppoFinalita AS GruppoFinalita_1 ON Imprese_Progetti.Stato_Impianto = GruppoFinalita_1.Grfi_Cod ")

            strSql.AppendLine(" WHERE  Movimenti.id_agenda IN (" & Agro_SQL_Save_Clausola_IN(Id_Agenda, False) & ")    ")

            '(20/10/2015 fede) modificata x tirare su solo Imprese_Progetti validi alla data dell'operazione 
            strSql.AppendLine(" AND Imprese_Progetti.Validita_Inizio<=Movimenti.Data_Movimento ")
            strSql.AppendLine(" AND Imprese_Progetti.Validita_fine>=Movimenti.Data_Movimento ")

            strSql.AppendLine(" AND Mov_Destinazioni.Piva ='" & Agro_SQL_SaveText(Piva) & "'    ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Mov_Destinazioni.Id_Agenda")
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

    'a differenza della precedente specie, varieta e finalita sono in left join x caricare anche le destinazioni d'uso + aggiunti campi ed eliminati altri
    Public Function Leggi_Dettagli_Impianti_x_Analisi_Conformita(ByVal Piva As String,
                                                                 ByVal Id_Agenda As String,
                                                                 ByVal Tipo_Operazione As enum_TipoOperazione_xAnalisiConformita,
                                                                 ByVal xFiltroAggiuntivo As String,
                                                                 ByVal xOrderBy As String,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dettagli_Impianti_x_Conformita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT distinct Movimenti.Data_Movimento, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione,  Mov_Destinazioni.Tipo_Destinazione,  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Agenda, Mov_Destinazioni.Id_Mov, Mov_Destinazioni.Id_Mov_Det, Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2,   ")
            strSql.AppendLine(" Movimenti_dettagli.Qta  AS Qta_Dett,  Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Progetto, ")
            strSql.AppendLine(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.extra_int, Movimenti_dettagli.Lotto, Movimenti_dettagli.doseetichetta_value, Movimenti_dettagli.extra_str,")
            strSql.AppendLine(" coalesce(Movimenti_dettagli.PrincipiAttiviPercAbb, '') as PrincipiAttiviPercAbb, ")
            strSql.AppendLine(" coalesce(Movimenti_dettagli.Polverulento, 0) as Polverulento, ")
            strSql.AppendLine(" ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod,  ")
            strSql.AppendLine(" ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ISNULL(Cultivar.Cul_Cod,0) AS Cul_Cod,  ")
            strSql.AppendLine(" ISNULL(Reg_Impianti.foral_cod,0) AS foral_cod, ISNULL(Reg_Impianti.Cop_Cod,0) AS Cop_Cod,  ")
            strSql.AppendLine(" Appezzamento.APP_NOME, Reg_Impianti.Validita_Inizio, Reg_Impianti.sup_imp, ISNULL(Reg_Impianti.grfi_cod,0) AS grfi_cod, GruppoFinalita.Grfi_Des, ISNULL(GruppoFinalita_1.Grfi_Des,'') AS Stato, ")
            strSql.AppendLine(" Imprese_Progetti.progetto_cod, Imprese_Progetti.Progetto_Nome, Imprese_Progetti.Stato_Impianto,  ")
            strSql.AppendLine(" ISNULL(Imprese_Progetti.Regolamento_Cod,1) AS Regolamento_Cod, ISNULL(Imprese_Progetti.Disciplinare_Cod,0) AS Disciplinare_Cod, ISNULL(Imprese_Progetti.Disciplinare_PubblicoPrivato,0) AS Disciplinare_PubblicoPrivato, ISNULL(Imprese_Progetti.Regolamento_Concimazioni_Cod,0) AS Regolamento_Concimazioni_Cod, ")
            strSql.AppendLine(" Imprese_Progetti.validita_inizio as Validita_inizio_esercizio, Imprese_Progetti.Validita_fine as Validita_fine_esercizio,  ")
            strSql.AppendLine(" Imprese_Progetti.data_fioritura_prevista , Imprese_Progetti.data_fine_prevista  ")

            '(29/10/2021 fede) aggiunto dettaglio per bufferzone
            strSql.AppendLine(" , ISNULL(sup_riduzione_bufferzone,0) as sup_riduzione_bufferzone,  ISNULL(perc_riduzione_deriva,0) as perc_riduzione_deriva  ")
            strSql.AppendLine(" , ISNULL(SupBZ_Riduzione,0) as SupBZ_Riduzione, ISNULL(DistBZ_CorpiIdrici,0) as DistBZ_CorpiIdrici, ISNULL(DistBZ_AreeResPub,0) as DistBZ_AreeResPub, ISNULL(DistBZ_Allevamenti,0) as DistBZ_Allevamenti, ISNULL(DistBZ_VegNatNonColt,0) as DistBZ_VegNatNonColt ")

            Select Case Tipo_Operazione

                Case enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo

                    strSql.AppendLine(" , isnull(Mov_Dettaglio_Tecnico.av_cod,0) as av_cod, isnull(Mov_Dettaglio_Tecnico.av_gru,0) as av_gru, isnull(Mov_Dettaglio_Tecnico.soglia_cod,0) as soglia_cod   ")

                    strSql.AppendLine(" , ISNULL((SELECT TOP 1 Mov_Dettaglio_Tecnico.qta_ril FROM Mov_Dettaglio_Tecnico  ")
                    strSql.AppendLine("             WHERE Mov_Dettaglio_Tecnico.Id_Mov = movimenti.Id_Mov And Mov_Dettaglio_Tecnico.Id_Agenda = movimenti.Id_Agenda  And   Mov_Dettaglio_Tecnico.PIVA = movimenti.PIVA ")
                    strSql.AppendLine("             And qta_ril<>0),0) AS acqua ")

                Case enum_TipoOperazione_xAnalisiConformita.Fertilizzazioni

                    strSql.AppendLine(" , isnull(Mov_Dettaglio_Tecnico.n,0) as n, isnull(Mov_Dettaglio_Tecnico.p,0) as p, isnull(Mov_Dettaglio_Tecnico.k,0) as k, isnull(Mov_Dettaglio_Tecnico.mg,0) as mg,  isnull(Mov_Dettaglio_Tecnico.cu,0) as cu , isnull(Mov_Dettaglio_Tecnico.efficienza,0) as efficienza  ")

                    '--- LIMITI APPORTI ---
                    strSql.AppendLine("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod ")
                    strSql.AppendLine("         FROM   Reg_Impianti_Codici ")
                    strSql.AppendLine("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
                    strSql.AppendLine("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
                    strSql.AppendLine("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
                    strSql.AppendLine("         AND Reg_Impianti_Codici.id_cod=1050 ")
                    strSql.AppendLine("         ), '') AS N_Massimo ")
                    strSql.AppendLine("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod ")
                    strSql.AppendLine("         FROM   Reg_Impianti_Codici ")
                    strSql.AppendLine("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
                    strSql.AppendLine("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
                    strSql.AppendLine("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
                    strSql.AppendLine("         AND Reg_Impianti_Codici.id_cod=1051 ")
                    strSql.AppendLine("         ), '') AS P_Massimo ")
                    strSql.AppendLine("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod ")
                    strSql.AppendLine("         FROM   Reg_Impianti_Codici ")
                    strSql.AppendLine("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
                    strSql.AppendLine("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
                    strSql.AppendLine("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
                    strSql.AppendLine("         AND Reg_Impianti_Codici.id_cod=1052 ")
                    strSql.AppendLine("         ), '') AS K_Massimo ")
                    strSql.AppendLine("   ,  ISNULL ((Select Reg_Impianti_Codici.val_cod ")
                    strSql.AppendLine("         FROM   Reg_Impianti_Codici ")
                    strSql.AppendLine("         WHERE  Piva = Imprese_Progetti.PIVA AND sa_cod = Imprese_Progetti.sa_cod ")
                    strSql.AppendLine("         AND appezza = Imprese_Progetti.appezza AND Id_Reg = Imprese_Progetti.id_reg ")
                    strSql.AppendLine("         AND Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
                    strSql.AppendLine("         AND Reg_Impianti_Codici.id_cod=1053 ")
                    strSql.AppendLine("         ), '') AS Mg_Massimo ")

            End Select

            strSql.AppendLine(" FROM  Mov_Destinazioni  INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli  ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND  Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")

            strSql.AppendLine(" Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG left outer JOIN ")
            strSql.AppendLine(" Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod left outer JOIN ")
            strSql.AppendLine(" SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            strSql.AppendLine(" Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  ")
            strSql.AppendLine(" Reg_Impianti.APPEZZA = Appezzamento.APPEZZA INNER JOIN ")
            strSql.AppendLine(" Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND  ")
            strSql.AppendLine(" Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg INNER JOIN ")

            strSql.AppendLine(" Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda And Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov left outer JOIN ")

            strSql.AppendLine(" Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND  ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det left outer JOIN ")

            strSql.AppendLine(" GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            strSql.AppendLine(" GruppoFinalita AS GruppoFinalita_1 ON Imprese_Progetti.Stato_Impianto = GruppoFinalita_1.Grfi_Cod ")

            strSql.AppendLine(" WHERE  Mov_Destinazioni.id_agenda IN (" & Agro_SQL_Save_Clausola_IN(Id_Agenda, False) & ")    ")

            '(20/10/2015 fede) modificata x tirare su solo Imprese_Progetti validi alla data dell'operazione 
            strSql.AppendLine(" AND Imprese_Progetti.Validita_Inizio<=Movimenti.Data_Movimento ")
            strSql.AppendLine(" AND Imprese_Progetti.Validita_fine>=Movimenti.Data_Movimento ")

            '(23/11/2018 fede) aggiunto filtro piva per id_agenda doppi
            strSql.AppendLine(" AND Mov_Destinazioni.Piva ='" & Agro_SQL_SaveText(Piva) & "'    ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Mov_Destinazioni.Id_Agenda")
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

    Public Function Leggi_Dettagli_Magazzino_x_Analisi_Conformita(ByVal Piva As String,
                                                                  ByVal Id_Agenda As String,
                                                                  ByVal xFiltroAggiuntivo As String,
                                                                  ByVal xOrderBy As String,
                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dettagli_Magazzino_x_Analisi_Conformita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT distinct Movimenti.Data_Movimento, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod,Mov_Destinazioni.Id_Destinazione,  Mov_Destinazioni.Tipo_Destinazione,  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Agenda, Movimenti_dettagli.Qta, Movimenti_dettagli.Udm_Cod,  ")
            strSql.AppendLine(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Mov_Det_Des,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.cal_cod,")
            strSql.AppendLine(" fabbricati.fabbricato_des, fabbricati.fabbricato_Cod, UnitaMisura.udm_sim ")

            strSql.AppendLine(" FROM  Mov_Destinazioni  INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli  ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" fabbricati ON Mov_Destinazioni.Piva = fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = fabbricati.SA_COD AND Mov_Destinazioni.Id_Destinazione = fabbricati.Fabbricato_Cod INNER JOIN ")
            strSql.AppendLine(" UnitaMisura ON Movimenti_dettagli.udm_cod=UnitaMisura.Udm_Cod  INNER JOIN ")

            'DRUDI
            'strSql.AppendLine(" Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND ")

            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda And Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  ")


            strSql.AppendLine(" WHERE  Movimenti.id_agenda IN (" & Agro_SQL_Save_Clausola_IN(Id_Agenda, False) & ")    ")

            '(23/11/2018 fede) aggiunto filtro piva per id_agenda doppi
            strSql.AppendLine(" AND Mov_Destinazioni.Piva ='" & Agro_SQL_SaveText(Piva) & "'    ")

            strSql.AppendLine(" AND tipo_destinazione = 20 ")
            strSql.AppendLine(" AND CAU_MOV ='7350' ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Mov_Destinazioni.Id_Agenda")
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

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Id_Mov_Det As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Destinazione As Integer,
                          ByVal Tipo_Destinazione As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal IDTestataTemp As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Appezza = 0
        '   Id_Destinazione = 0 
        '   Tipo_Destinazione = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Appezza, Id_Destinazione, Tipo_Destinazione, Qta, Qta2, mov_destinazioni_graphickey ")
                    stb.AppendLine(" FROM  Mov_Destinazioni WITH(NOLOCK)")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f WITH(NOLOCK)")
                        stb.AppendLine("On f.piva = Mov_Destinazioni.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Mov_Destinazioni.SA_COD ")
                        stb.AppendLine(" And f.appezza = Mov_Destinazioni.APPEZZA ")
                        stb.AppendLine(" And f.id_reg = Mov_Destinazioni.Id_Destinazione ")
                        stb.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione = 0")
                        stb.AppendLine(" And f.idTestataTemp =  " & IDTestataTemp)
                    End If

                    stb.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Appezza <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Tipo_Destinazione <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY Mov_Destinazioni.Piva Asc ")
                    End If

                    '===========================================================================================

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT Mov_Destinazioni.* , Agenda.Lav_Cod , Agenda.Des_Lib, Agenda.Validita_Inizio as xValidita_Inizio, ISNULL(Imprese_Progetti.Progetto_Cod,0) as Progetto_Cod ")
                    stb.AppendLine(" FROM  Mov_Destinazioni WITH(NOLOCK)")
                    stb.AppendLine(" inner Join  Agenda WITH(NOLOCK)")
                    stb.AppendLine(" On   Agenda.Piva = Mov_Destinazioni.Piva And   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                    stb.AppendLine(" LEFT JOIN Imprese_Progetti WITH(NOLOCK)")
                    stb.AppendLine(" ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA And Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD And Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA And Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")
                    stb.AppendLine(" And Imprese_Progetti.Validita_Inizio <= Agenda.Validita_Inizio ")
                    stb.AppendLine(" And Imprese_Progetti.Validita_Fine >= Agenda.Validita_Inizio ")
                    stb.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione = 0 ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f WITH(NOLOCK)")
                        stb.AppendLine(" On f.piva = Mov_Destinazioni.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Mov_Destinazioni.SA_COD ")
                        stb.AppendLine(" And f.appezza = Mov_Destinazioni.APPEZZA ")
                        stb.AppendLine(" And f.id_reg = Mov_Destinazioni.Id_Destinazione ")
                        stb.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione = 0")
                        stb.AppendLine(" And f.idTestataTemp =  " & IDTestataTemp)
                    End If

                    stb.AppendLine(" WHERE Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" And   Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    'Join sulla Piva
                    stb.AppendLine(" ")

                    'Join sul Id_Agenda
                    stb.AppendLine(" ")

                    If Piva <> "" Then
                        stb.AppendLine(" And Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Appezza <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Tipo_Destinazione <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Agenda.Inviato >=0 ")
                            stb.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Agenda.Inviato =-1 ")
                            stb.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY Mov_Destinazioni.Piva Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    stb.Length = 0
                    stb.AppendLine(" SELECT Mov_Destinazioni.* , Agenda.Lav_Cod , Agenda.Des_Lib, Agenda.Validita_Inizio as xValidita_Inizio, Operazioni.Lav_Des, ISNULL(Imprese_Progetti.Progetto_Cod,0) as Progetto_Cod ")
                    stb.AppendLine(" FROM  Mov_Destinazioni WITH(NOLOCK)")
                    stb.AppendLine(" INNER JOIN Agenda WITH(NOLOCK)")
                    stb.AppendLine(" On Mov_Destinazioni.Piva = Agenda.PIVA And Mov_Destinazioni.Id_Agenda = Agenda.Id_Agenda")
                    stb.AppendLine(" INNER Join Operazioni WITH(NOLOCK)")
                    stb.AppendLine(" On Agenda.Lav_Cod = Operazioni.LAV_COD ")

                    stb.AppendLine(" LEFT JOIN Imprese_Progetti WITH(NOLOCK)")
                    stb.AppendLine(" ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA And Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD And Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA And Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")
                    stb.AppendLine(" And Imprese_Progetti.Validita_Inizio <= Agenda.Validita_Inizio ")
                    stb.AppendLine(" And Imprese_Progetti.Validita_Fine >= Agenda.Validita_Inizio ")
                    stb.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione = 0 ")

                    If IDTestataTemp <> 0 Then
                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f WITH(NOLOCK)")
                        stb.AppendLine("On f.piva = Mov_Destinazioni.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Mov_Destinazioni.SA_COD ")
                        stb.AppendLine(" And f.appezza = Mov_Destinazioni.APPEZZA ")
                        stb.AppendLine(" And f.id_reg = Mov_Destinazioni.Id_Destinazione ")
                        stb.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione = 0")
                        stb.AppendLine(" And f.idTestataTemp =  " & IDTestataTemp)
                    End If

                    stb.AppendLine(" WHERE Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" And   Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.AppendLine(" And Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Appezza <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If Tipo_Destinazione <> 0 Then
                        stb.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Agenda.Inviato >=0 ")
                            stb.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Agenda.Inviato =-1 ")
                            stb.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY Mov_Destinazioni.Piva Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function Leggi_conImpianti(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Id_Mov As Integer,
                                      ByVal Id_Mov_Det As Integer,
                                      ByVal Appezza As Integer,
                                      ByVal Id_Destinazione As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_conImpianti()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Appezza = 0
        '   Id_Destinazione = 0 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Mov_Destinazioni.Id_Mov, Mov_Destinazioni.Id_Mov_Det, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta, Mov_Destinazioni.Qta2, Reg_Impianti.sup_imp ")
            strSql.AppendLine(" FROM  Mov_Destinazioni INNER JOIN Reg_Impianti ON Mov_Destinazioni.piva=Reg_Impianti.piva AND Mov_Destinazioni.sa_cod=Reg_Impianti.sa_cod AND Mov_Destinazioni.appezza=Reg_Impianti.appezza AND Mov_Destinazioni.Id_Destinazione=Reg_Impianti.id_reg ")
            strSql.AppendLine(" WHERE Mov_Destinazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Mov_Destinazioni.Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Mov_Destinazioni.Piva Asc ")
            End If

            '===========================================================================================

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

    ''' <summary>
    ''' Lettura delle destinazioni di agenda con info accessorie
    ''' </summary>
    ''' <param name="veg_cod"></param>
    ''' <param name="id_Cod"></param>
    ''' <param name="validita_inizio"></param>
    ''' <param name="validita_fine"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Leggi_DestinazioneConInformazioniRiepilogative(
        ByVal RaggruppaPerCampo As Boolean,
        ByVal piva As String,
        ByVal sa_Cod As Integer,
        ByVal veg_cod As Integer,
        ByVal id_Cod As Integer,
        ByVal validita_inizio As Date,
        ByVal validita_fine As Date,
        ByVal FiltroImpianti As String,
        ByVal NomeDB_Utenti As String,
        ByVal stbAgea2015 As StringBuilder,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Dim dt As DataTable

        Dim nomeRoutine As String = ""
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder

        Try

            If RaggruppaPerCampo Then

                stb.AppendLine(" --per raggruppamento su campo ")
                stb.AppendLine("  ")
                stb.AppendLine(" Select  ")
                stb.AppendLine("  ")
                stb.AppendLine("       piva ")
                stb.AppendLine("     , sa_Cod ")
                stb.AppendLine("     , campo_cod ")
                stb.AppendLine("     , Id_Agenda  ")
                stb.AppendLine("     , id_mov ")
                stb.AppendLine("     , id_mov_det ")
                stb.AppendLine("  ")
                stb.AppendLine("     , max(Impresa) as Impresa ")
                stb.AppendLine("     , max(CentroAziendale) as CentroAziendale ")
                stb.AppendLine("     , max(data_movimento) as data_movimento ")
                stb.AppendLine("     , max(Operazione) as Operazione ")
                stb.AppendLine("     ,'' as DescrizioneCommessa ")
                stb.AppendLine("     ,'' as CodiceCommessa ")
                stb.AppendLine("     , Campo_Des ")
                stb.AppendLine("     , max(specie) as specie ")
                stb.AppendLine("     , max(Varieta) as Varieta ")
                stb.AppendLine("     , max(DestinazioneUso) as DestinazioneUso ")
                stb.AppendLine("     , 0.0 as Ore --ToDo ")
                stb.AppendLine("     , sum(SupTrattata) as SupTrattata ")
                stb.AppendLine("     , max(Prodotto) as Prodotto ")
                stb.AppendLine("     , '' as LottoProdotto --TODO ")
                stb.AppendLine("     , max(FattoreProporzione) as FattoreProporzione ")
                stb.AppendLine("     , max(DoseHA) as DoseHA ")
                stb.AppendLine("     , max(UNITA_DI_MISURA) as UNITA_DI_MISURA ")
                stb.AppendLine("     , max(h2o) as h2o ")
                stb.AppendLine("     , max(avversita) as avversita ")
                stb.AppendLine("     , max(Operatore) as Operatore ")
                stb.AppendLine("     , max(Macchina_Trattore_codice) as Macchina_Trattore_codice ")
                stb.AppendLine("     , max(Macchina_Botte_codice) as Macchina_Botte_codice ")
                stb.AppendLine("     , max(numeroPatentino) as numeroPatentino ")
                stb.AppendLine("     , max(FaseFenologica) as FaseFenologica ")
                stb.AppendLine("     , max(Fase_Cod_Corrente) as Fase_Cod_Corrente ")
                stb.AppendLine("     , max(Utente_Modifica) as Utente_Modifica  ")
                If stbAgea2015 IsNot Nothing Then
                    stb.AppendLine("     , max(Macrouso_DES) as Macrouso_DES  ")
                End If

                stb.AppendLine("  ")
                stb.AppendLine(" from (")

            End If

            stb.AppendLine(" ")
            stb.AppendLine("  Select  ")
            stb.AppendLine("  ")
            stb.AppendLine("  --dati di chiave ")
            stb.AppendLine("  ")
            stb.AppendLine("   d.piva ")
            stb.AppendLine(" , d.sa_cod  ")
            stb.AppendLine(" , d.appezza  ")
            stb.AppendLine(" , d.Id_Destinazione as id_reg ")
            stb.AppendLine(" , isnull(cp.campo_Cod, -d.appezza)  as campo_cod ")
            stb.AppendLine(" , d.Id_Agenda  ")
            stb.AppendLine(" , d.Id_Mov  ")
            stb.AppendLine(" , d.Id_Mov_Det  ")
            stb.AppendLine(" , Qta2 / imp.Sup_Imp as FattoreProporzione --per riproporzionare le superfici in base alla sup trattata  ")
            stb.AppendLine("  ")
            stb.AppendLine(" , d.Qta / d.Qta2 as DoseHA ")
            stb.AppendLine(" , d.Qta2 as SupTrattata ")

            stb.AppendLine(" --dati descrittivi ")
            stb.AppendLine(" , veg.veg_Cod ")
            stb.AppendLine(" , veg.veg_des as Specie ")
            stb.AppendLine(" , c.cul_des as Varieta ")
            stb.AppendLine(" , anag.descrizione as DestinazioneUso ")
            stb.AppendLine(" , a.data_modifica ")
            stb.AppendLine(" , mTes.data_movimento ")
            stb.AppendLine(" , o.lav_des as Operazione ")
            stb.AppendLine(" , i.rag_soc as Impresa ")
            stb.AppendLine(" , sa.sa_nome as CentroAziendale ")
            stb.AppendLine(" , isnull(cp.campo_des, app.app_nome) as campo_Des ")
            stb.AppendLine(" , case when udm.Udm_Cod = 29 then 'L' else 'KG' end As UNITA_DI_MISURA ")
            stb.AppendLine(" , udm_sim ")
            stb.AppendLine(" , udm.udm_cod ")
            stb.AppendLine(" , isnull(isnull(ff.fr_des, fer.fer_des), '') as Prodotto ")
            stb.AppendLine("  ")
            stb.AppendLine(" , '' as LottoProdotto --todo ")
            stb.AppendLine(" , case when tec.qta_ril is null or tec.qta_ril = 0  then 0 else  case when tec.Qta_Ril < 0  then abs(tec.Qta_Ril) else  ( tec.Qta_Ril / sDetTotale.SuperficieDettaglio ) end end as [h2o]  ")
            stb.AppendLine(" , '' as CodiceCommessa --todo ")
            stb.AppendLine(" , '' as DescrizioneCommessa --todo ")
            stb.AppendLine(" , 0.0 as Ore --todo ")
            stb.AppendLine(" , '' as FaseFenologica --todo ")
            stb.AppendLine(" , ISNULL  ( ")
            stb.AppendLine("     (SELECT TOP 1 tecff.ff_classe    ")
            stb.AppendLine("         From Movimenti_dettagli dff  ")
            stb.AppendLine("             INNER Join  Movimenti mff    ")
            stb.AppendLine("                 On dff.PIVA = mff.PIVA  ")
            stb.AppendLine("                 And dff.Sa_Cod = mff.Sa_Cod  ")
            stb.AppendLine("                 And dff.Id_Agenda = mff.Id_Agenda  ")
            stb.AppendLine("                 And dff.Id_Mov = mff.Id_Mov  ")
            stb.AppendLine("             INNER Join  Agenda aff  ")
            stb.AppendLine("                 On mff.PIVA = aff.PIVA  ")
            stb.AppendLine("                 And mff.Sa_Cod = aff.Sa_Cod  ")
            stb.AppendLine("                 And mff.Id_Agenda = aff.Id_Agenda  ")
            stb.AppendLine("             INNER Join Mov_Destinazioni destff ")
            stb.AppendLine("                On dff.PIVA = destff.Piva  ")
            stb.AppendLine("                And dff.Sa_Cod = destff.Sa_Cod  ")
            stb.AppendLine("                And dff.Id_Agenda = destff.Id_Agenda  ")
            stb.AppendLine("                And dff.Id_Mov = destff.Id_Mov  ")
            stb.AppendLine("                And dff.Id_Mov_Det = destff.Id_Mov_Det   ")
            stb.AppendLine("              INNER Join Mov_Dettaglio_Tecnico tecff  ")
            stb.AppendLine("                 On dff.PIVA = tecff.Piva  ")
            stb.AppendLine("                 And dff.Id_Mov_Det = tecff.Id_Mov_Det  ")
            stb.AppendLine("                 And dff.Id_Mov = tecff.Id_Mov  ")
            stb.AppendLine("                 And dff.Id_Agenda = tecff.Id_Agenda  ")
            stb.AppendLine("                 And dff.Sa_Cod = tecff.Sa_Cod   ")
            stb.AppendLine("  ")
            stb.AppendLine("         WHERE(aff.Lav_Cod = 79) And (mff.Cau_Mov = '2100') AND (dff.PIVA = imp.PIVA)  ")
            stb.AppendLine("             And destff.Sa_Cod = imp.sa_cod  ")
            stb.AppendLine("             And destff.APPEZZA = imp.APPEZZA  ")
            stb.AppendLine("             And destff.ID_destinazione = imp.ID_REG    ")
            stb.AppendLine("             And mff.Data_Movimento >= ipp.Validita_Inizio   ")
            stb.AppendLine("             And mff.Data_Movimento <= ipp.Validita_Fine   ")
            stb.AppendLine("             And destff.validita_inizio <=  mTes.data_movimento ")
            stb.AppendLine("         ORDER BY mff.validita_inizio desc) ")
            stb.AppendLine(" , 0) AS Fase_Cod_Corrente")
            stb.AppendLine("  ")
            stb.AppendLine(" , '' as Carenza --todo ")
            stb.AppendLine(" , '' as TempoRientro --todo ")
            stb.AppendLine("  ")
            stb.AppendLine(" , ISNULL(ISNULL(av.av_des_vol, avGru.av_gru_des), '') as Avversita ")
            stb.AppendLine(" , '' as Operatore ")
            stb.AppendLine("  ")
            stb.AppendLine(" , '' as Macchina_Trattore_codice --todo ")
            stb.AppendLine(" , '' as Macchina_Botte_codice --todo --altre macchine in altre operazioni ?  ")
            stb.AppendLine("  ")
            stb.AppendLine(" , '' as numeroPatentino --todo (leggere da nuova tabella ricky) ")
            stb.AppendLine("  ")
            stb.AppendLine(" ,dd.nome + ' ' + dd.cognome + ' ' + dd.rag_soc as Utente_Modifica -- utente GIAS che ha fatto l'operazione ")

            If stbAgea2015 IsNot Nothing Then
                stb.AppendLine(" ,A2015.Macrouso_Des ")
            End If

            If Not RaggruppaPerCampo Then
                stb.AppendLine("  , (d.Qta / d.Qta2) * (isnull(tecDet.N, 100) / 100)  as DoseHA_N")
            End If

            stb.AppendLine(" -- si parte dalle distribuzioni di prodotto su impianti ")
            stb.AppendLine(" From Mov_Destinazioni d  ")
            stb.AppendLine("  ")

            stb.AppendLine("          inner Join ( ")
            stb.AppendLine("  ")
            stb.AppendLine("         Select  ")
            stb.AppendLine("               Id_Agenda ")
            stb.AppendLine("             , id_mov ")
            stb.AppendLine("             , id_mov_det ")
            stb.AppendLine("             , sum(qta2) as SuperficieDettaglio ")
            stb.AppendLine("         From Mov_Destinazioni d ")
            stb.AppendLine("         where d.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine("         and d.tipo_destinazione = 0  ")
            stb.AppendLine(FiltroImpianti)
            stb.AppendLine("         Group By ")
            stb.AppendLine("               Id_Agenda  ")
            stb.AppendLine("             , id_mov ")
            stb.AppendLine("             , id_mov_det ")
            stb.AppendLine("     ) sDetTotale ")
            stb.AppendLine("         On d.id_agenda = sDetTotale.id_agenda  ")
            stb.AppendLine("         And d.id_mov = sDetTotale.id_mov ")
            stb.AppendLine("         And d.id_mov_det = sDetTotale.id_mov_det")


            stb.AppendLine("     inner Join Reg_Impianti imp  ")
            stb.AppendLine("         On d.PIVA = imp.PIVA   ")
            stb.AppendLine("         And d.SA_COD = imp.SA_COD   ")
            stb.AppendLine("         And d.APPEZZA = imp.APPEZZA  ")
            stb.AppendLine("         And d.Id_Destinazione = imp.id_reg  ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner Join appezzamento app ")
            stb.AppendLine("         On app.PIVA = imp.PIVA   ")
            stb.AppendLine("         And app.SA_COD = imp.SA_COD   ")
            stb.AppendLine("         And app.APPEZZA = imp.APPEZZA  ")
            stb.AppendLine("          ")
            stb.AppendLine("    INNER Join Movimenti_dettagli Det   ")
            stb.AppendLine("         On d.PIVA = det.PIVA       ")
            stb.AppendLine("         And d.Sa_Cod = det.Sa_Cod       ")
            stb.AppendLine("         And d.Id_Agenda = det.Id_Agenda      ")
            stb.AppendLine("         And d.Id_Mov = det.Id_Mov    ")
            stb.AppendLine("         And d.Id_Mov_Det = det.Id_Mov_Det   ")
            stb.AppendLine("      ")
            stb.AppendLine("     inner Join movimenti mTes ")
            stb.AppendLine("         On mTes.id_agenda = d.id_agenda         ")
            stb.AppendLine("         And mTes.id_mov = det.id_mov ")
            stb.AppendLine("  ")

            stb.AppendLine("     inner Join imprese_progetti ipp ")
            stb.AppendLine("         On  ipp.PIVA = imp.PIVA   ")
            stb.AppendLine("         And ipp.SA_COD = imp.SA_COD   ")
            stb.AppendLine("         And ipp.APPEZZA = imp.APPEZZA  ")
            stb.AppendLine("         And ipp.id_reg = imp.id_reg  ")
            stb.AppendLine("         And ipp.validita_inizio <= mTes.data_movimento ")
            stb.AppendLine("         And ipp.validita_fine >= mTes.data_movimento")

            stb.AppendLine("     inner Join agenda a  ")
            stb.AppendLine("         On a.id_agenda = d.id_agenda ")
            stb.AppendLine("      ")
            stb.AppendLine("     inner Join [" & NomeDB_Utenti & "].dbo.utenti_dettagli dd ")
            stb.AppendLine("         On dd.codFisc = a.username_modifica ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner Join operazioni o ")
            stb.AppendLine("         On o.lav_cod = a.lav_cod ")
            stb.AppendLine("    ")
            stb.AppendLine("     inner Join centri_aziendali sa ")
            stb.AppendLine("         On sa.piva = d.piva  ")
            stb.AppendLine("         And sa.sa_cod = d.sa_Cod ")
            stb.AppendLine("      ")
            stb.AppendLine("     inner Join imprese i ")
            stb.AppendLine("         On i.piva = a.piva ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner Join UnitaMisura udm ")
            stb.AppendLine("         On udm.udm_cod = det.udm_cod ")
            stb.AppendLine("  ")
            stb.AppendLine("     --lista con ileft join ")

            If stbAgea2015 IsNot Nothing Then
                stb.AppendLine("   --lettura codifiche agea per impianto ")
                stb.AppendLine("   left Join ( ")

                stb.Append(stbAgea2015)

                stb.AppendLine(" ) A2015 ")


                stb.AppendLine("  On  imp.PIVA = A2015.PIVA      ")
                stb.AppendLine("  And imp.Sa_Cod = A2015.Sa_Cod      ")
                stb.AppendLine("  And imp.Appezza = A2015.APPEZZA ")
                stb.AppendLine("  And imp.id_reg = A2015.ID_REG      ")
                stb.AppendLine("  ")
                stb.AppendLine("   --fine lettura codifiche agea per impianto ")
                stb.AppendLine("  ")
            End If

            stb.AppendLine("    Left Join mov_dettaglio_tecnico tec ")
            stb.AppendLine("        On tec.id_agenda = d.id_agenda ")
            stb.AppendLine("        And tec.id_mov = d.id_mov ")
            stb.AppendLine("        And tec.id_mov_det = 0")

            stb.AppendLine("    Left Join mov_dettaglio_tecnico tecDet ")
            stb.AppendLine("         On tecDet.id_agenda = d.id_agenda ")
            stb.AppendLine("         And tecDet.id_mov = d.id_mov ")
            stb.AppendLine("         And tecDet.id_mov_det = d.id_mov_det  ")


            stb.AppendLine("     Left Join cultivar c ")
            stb.AppendLine("         On c.cul_Cod = imp.cul_Cod ")
            stb.AppendLine("  ")
            stb.AppendLine("     Left Join specieVegetali veg ")
            stb.AppendLine("         On veg.veg_Cod = c.veg_cod ")
            stb.AppendLine("  ")
            stb.AppendLine("     Left Join reg_impianti_codici cc ")
            stb.AppendLine("         On cc.PIVA = imp.PIVA   ")
            stb.AppendLine("         And cc.SA_COD = imp.SA_COD   ")
            stb.AppendLine("         And cc.APPEZZA = imp.APPEZZA  ")
            stb.AppendLine("         And cc.id_reg = imp.id_reg  ")
            stb.AppendLine("         And cc.progetto_Cod = 0  ")
            stb.AppendLine("         And cc.id_cod >= 3000 And cc.id_cod < 4000 ")
            stb.AppendLine("  ")
            stb.AppendLine("     Left Join codici_Anagrafe anag ")
            stb.AppendLine("         On anag.codice = cc.id_cod ")
            stb.AppendLine("  ")
            stb.AppendLine("     Left Join campi cp ")
            stb.AppendLine("         On cp.piva = app.piva ")
            stb.AppendLine("         And cp.sa_cod = app.sa_cod ")
            stb.AppendLine("         And cp.campo_cod = app.campo_cod ")
            stb.AppendLine("  ")
            stb.AppendLine("     Left Join formulati ff ")
            stb.AppendLine("        On ff.fr_cod = det.pro_cod ")
            stb.AppendLine("        And det.elem_cod = 191 ")
            stb.AppendLine("  ")
            stb.AppendLine("     Left Join fertilizzanti fer ")
            stb.AppendLine("        On fer.fer_cod = det.pro_cod ")
            stb.AppendLine("        And det.elem_cod = 3 ")

            stb.AppendLine("     Left Join avversita av ")
            stb.AppendLine("         On tecDet.av_cod = av.av_cod ")
            stb.AppendLine("         And det.elem_cod = 191 ")
            stb.AppendLine("         And tecDet.av_Cod <> 0 ")
            stb.AppendLine("  ")
            stb.AppendLine("     Left Join GruppoAvversita avGru ")
            stb.AppendLine("         On tecDet.av_gru = avGru.av_gru ")
            stb.AppendLine("         And det.elem_cod = 191 ")
            stb.AppendLine("         And tecDet.av_gru <> 0")

            stb.AppendLine("  ")
            stb.AppendLine(" where d.Tipo_Destinazione = 0 ")
            stb.AppendLine(FiltroImpianti)
            stb.AppendLine(" And o.gru_op in (3, 4, 5) ")
            stb.AppendLine("  ")
            stb.AppendLine(" And mTes.data_movimento >= " & Agro_SQL_SaveDate(validita_inizio) & " ")
            stb.AppendLine(" And mTes.data_movimento <= " & Agro_SQL_SaveDate(validita_fine) & " ")
            stb.AppendLine("  ")

            If veg_cod <> -1 Then
                stb.AppendLine(" And veg.veg_cod = " & veg_cod)
            End If

            stb.AppendLine(" And d.piva = '" & piva & "'")

            If sa_Cod <> 0 Then
                stb.AppendLine(" And d.sa_cod = " & sa_Cod)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If RaggruppaPerCampo Then

                stb.AppendLine("  ) cp1 ")
                stb.AppendLine("  ")
                stb.AppendLine(" group by ")
                stb.AppendLine("       piva ")
                stb.AppendLine("     , sa_Cod ")
                stb.AppendLine("     , campo_Cod ")
                stb.AppendLine("     , Campo_Des ")
                stb.AppendLine("     , Id_Agenda  ")
                stb.AppendLine("     , id_mov ")
                stb.AppendLine("     , id_mov_det")

            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                If RaggruppaPerCampo Then
                    stb.AppendLine(" ORDER BY Data_Movimento Desc ")
                Else
                    stb.AppendLine(" ORDER BY mTes.Data_Movimento Desc ")
                End If
            End If

            '===========================================================================================

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function Leggi_conDettagliAgenda_fromImpianti(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Id_Agenda As Integer,
                                                         ByVal Lav_Cod As Integer,
                                                         ByVal Id_Mov As Integer,
                                                         ByVal Id_Mov_Det As Integer,
                                                         ByVal Appezza As Integer,
                                                         ByVal Id_Destinazione As Integer,
                                                         ByVal validita_inizio As Date,
                                                         ByVal validita_fine As Date,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_conDettagliAgenda_fromImpianti()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Appezza = 0
        '   Id_Destinazione = 0 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim inizio As Date = objParametri.FinestraTemporaleInizio
        Dim fine As Date = objParametri.FinestraTemporaleFine

        If validita_inizio > inizio Then
            inizio = validita_inizio
        End If

        If validita_fine < fine Then
            fine = validita_fine
        End If

        Try
            ' Giulia: 14/3/2018:La validità inizio viene presa e confrontata quella dell'agenda, perché è sicuramente corretta,
            '       non più quella della destinazione perché a volte quella della destinazione è 01/01/1900

            '------------------------------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" SELECT  Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Mov_Destinazioni.Id_Mov, ")
            strSql.AppendLine("         Mov_Destinazioni.Id_Mov_Det, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, ")
            strSql.AppendLine("         Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta, Mov_Destinazioni.Qta2, ")
            strSql.AppendLine("         Reg_Impianti.sup_imp, Agenda.Validita_Inizio")
            strSql.AppendLine(" FROM  Mov_Destinazioni ")
            strSql.AppendLine(" INNER JOIN Reg_Impianti ON Mov_Destinazioni.piva=Reg_Impianti.piva AND Mov_Destinazioni.sa_cod=Reg_Impianti.sa_cod AND Mov_Destinazioni.appezza=Reg_Impianti.appezza AND Mov_Destinazioni.Id_Destinazione=Reg_Impianti.id_reg ")

            strSql.AppendLine("  inner join agenda on agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda and agenda.piva = Mov_Destinazioni.piva and agenda.sa_cod = Mov_Destinazioni.sa_cod")

            strSql.AppendLine(" WHERE Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(fine) & " ")
            strSql.AppendLine(" AND   Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(inizio) & " ")


            strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Lav_Cod <> 0 Then
                strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Mov_Destinazioni.Piva Asc ")
            End If

            '===========================================================================================

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


    '##############################################################################################
    Public Function LeggiOperazioni_dataDA_dataA(ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Appezza As Integer,
                                                 ByVal Id_Destinazione As Integer,
                                                 ByVal Data_DA As Date,
                                                 ByVal Data_A As Date,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.LeggiOperazioni_dataDA_dataA()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  si leggono tutti i Mov_Destinazioni
        '   Sa_Cod = 0           =>  si leggono tutti i Mov_Destinazioni dell'impresa
        '   Appezza = 0          =>  si leggono tutti i Mov_Destinazioni del centro aziendale
        '   Id_Destinazione = 0  =>  si leggono tutti i Mov_Destinazioni dell'appezzamento
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Mov_Destinazioni.Piva , Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Agenda.Lav_Cod , Agenda.Des_Lib , Movimenti.Data_Movimento, Movimenti.Mov_Desc ")
            strSql.AppendLine(" FROM  Agenda, Movimenti, Mov_Destinazioni  ")
            strSql.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_A) & " ")
            strSql.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_DA) & " ")
            strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 0 ")

            'Join sulla Piva
            strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
            strSql.AppendLine(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")

            'Join sul Sa_Cod
            strSql.AppendLine(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            strSql.AppendLine(" AND   Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

            'Join sul Id_Agenda
            strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine(" AND   Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

            'Join sul Id_Mov
            strSql.AppendLine(" AND   Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(" ORDER BY Movimenti.Data_Movimento Asc ")

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

    Public Function LeggiCronologiaMovimenti(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Appezza As Integer,
                                             ByVal Id_Destinazione As Integer,
                                             ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri,
                                                Optional joinDescrizioneImpianto As Boolean = False
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.LeggiCronologiaMovimenti()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""            =>  si leggono tutti i Mov_Destinazioni
        '   Sa_Cod = 0           =>  si leggono tutti i Mov_Destinazioni dell'impresa
        '   Appezza = 0          =>  si leggono tutti i Mov_Destinazioni del centro aziendale
        '   Id_Destinazione = 0  =>  si leggono tutti i Mov_Destinazioni dell'appezzamento
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                     enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT DISTINCT Mov_Destinazioni.Piva , Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Agenda.Lav_Cod , Agenda.Des_Lib , Movimenti.Data_Movimento, Movimenti.Mov_Desc ")

                    If joinDescrizioneImpianto Then
                        strSql.AppendLine(" , Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto ")
                        strSql.AppendLine(" , Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto ")
                        strSql.AppendLine(" , Reg_Impianti.Cul_Cod ")
                        strSql.AppendLine(" , ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des ")
                        strSql.AppendLine(" , ISNULL(Cultivar.Cul_Des,'') AS Cul_Des ")
                        strSql.AppendLine(" , Appezzamento.APP_NOME ")
                        strSql.AppendLine(" , ISNULL(DestinazioneUsoDes.descrizione, '') AS destinazioneUso ")
                    End If

                    strSql.AppendLine(" FROM  Agenda, Movimenti, Mov_Destinazioni  ")

                    If joinDescrizioneImpianto Then
                        strSql.AppendLine(" INNER JOIN Reg_Impianti ON ")
                        strSql.AppendLine("     Reg_Impianti.PIVA = Mov_Destinazioni.PIVA ")
                        strSql.AppendLine(" AND Reg_Impianti.SA_COD = Mov_Destinazioni.SA_COD ")
                        strSql.AppendLine(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.APPEZZA ")
                        strSql.AppendLine(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.ID_Destinazione ")
                        strSql.AppendLine(" INNER JOIN Appezzamento ON ")
                        strSql.AppendLine("     Appezzamento.PIVA = Reg_Impianti.PIVA ")
                        strSql.AppendLine(" AND Appezzamento.SA_COD = Reg_Impianti.SA_COD ")
                        strSql.AppendLine(" AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")
                        strSql.AppendLine(" LEFT JOIN Cultivar ON ")
                        strSql.AppendLine("     Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
                        strSql.AppendLine(" LEFT JOIN SpecieVegetali ON ")
                        strSql.AppendLine("     SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")

                        strSql.AppendLine(" LEFT JOIN Reg_Impianti_Codici DestinazioneUsoCod ON ")
                        strSql.AppendLine("     DestinazioneUsoCod.PIVA = Reg_Impianti.PIVA ")
                        strSql.AppendLine(" AND DestinazioneUsoCod.sa_cod = Reg_Impianti.sa_cod ")
                        strSql.AppendLine(" AND DestinazioneUsoCod.appezza = Reg_Impianti.appezza ")
                        strSql.AppendLine(" AND DestinazioneUsoCod.ID_REG = Reg_Impianti.Id_Reg ")
                        strSql.AppendLine(" AND DestinazioneUsoCod.id_cod >=3000 and DestinazioneUsoCod.id_cod < 4000 ")
                        strSql.AppendLine(" LEFT JOIN Codici_Anagrafe DestinazioneUsoDes ON DestinazioneUsoDes.codice = DestinazioneUsoCod.id_cod ")
                    End If

                    strSql.AppendLine($" WHERE Movimenti.Data_Movimento <= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)} ")
                    strSql.AppendLine($" AND   Movimenti.Data_Movimento >= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio)} ")
                    strSql.AppendLine(" And   Mov_Destinazioni.Tipo_Destinazione = 0 ")

                    'Join sulla Piva
                    strSql.AppendLine(" And   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" And   Movimenti.Piva = Mov_Destinazioni.Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" And   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
                    strSql.AppendLine(" And   Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

                    'Join sul Id_Agenda
                    strSql.AppendLine(" And   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" And   Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

                    'Join sul Id_Mov
                    strSql.AppendLine(" And   Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")

                    If Piva <> "" Then
                        strSql.AppendLine(" And Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                            strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                            strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        strSql.AppendLine(" ORDER BY Movimenti.Data_Movimento Asc ")
                    End If

            End Select

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

    '##############################################################################################
    Public Function LeggiAvversita(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Id_Agenda As Integer,
                                   ByVal Id_Mov As Integer,
                                   ByVal Id_Mov_Det As Integer,
                                   ByVal Appezza As Integer,
                                   ByVal Id_Destinazione As Integer,
                                   ByVal Tipo_Destinazione As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.LeggiAvversita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" (SELECT Distinct Av_Des_Vol, '' As Av_Gru_Des , Agenda.Id_Agenda ")
            strSql.AppendLine(" FROM  Avversita , GruppoAvversita, Agenda, Mov_Dettaglio_Tecnico, Mov_Destinazioni ")
            strSql.AppendLine(" WHERE Mov_Destinazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Mov_Destinazioni.Validita_inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Av_Cod <> 0 AND Mov_Dettaglio_Tecnico.Av_Gru = 0" & "  ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Av_Cod = Avversita.Av_Cod" & "  ")
            'Join sul Sa_Cod
            strSql.AppendLine(" AND   Agenda.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

            'Join sul Id_Agenda
            strSql.AppendLine(" AND   Agenda.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0 ) " & "   ")

            strSql.AppendLine(" UNION  (SELECT Distinct '' As Av_Des_Vol , Av_Gru_Des , Agenda.Id_Agenda ")
            strSql.AppendLine(" FROM  Avversita , GruppoAvversita, Agenda, Mov_Dettaglio_Tecnico, Mov_Destinazioni ")
            strSql.AppendLine(" WHERE Mov_Destinazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Mov_Destinazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Av_Cod = 0 AND Mov_Dettaglio_Tecnico.Av_Gru <> 0" & "  ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Av_Gru = GruppoAvversita.Av_Gru" & "  ")
            'Join sulla Piva
            strSql.AppendLine(" AND   Agenda.Piva = Mov_Dettaglio_Tecnico.Piva ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Piva = Mov_Destinazioni.Piva ")

            'Join sul Sa_Cod
            strSql.AppendLine(" AND   Agenda.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

            'Join sul Id_Agenda
            strSql.AppendLine(" AND   Agenda.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")
            strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0) " & "   ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'Nota: Questo ordinamento è importante per la gestione del campo.
                'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                strSql.AppendLine(" ORDER BY Avversita.Av_Des_Vol Asc ")
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


    Public Function Leggi_Raccolte(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Id_Agenda As Integer,
                                   ByVal Id_Mov As Integer,
                                   ByVal Id_Mov_Det As Integer,
                                   ByVal Appezza As Integer,
                                   ByVal Id_Destinazione As Integer,
                                   ByVal Tipo_Destinazione As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni.Leggi_Raccolte()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT DISTINCT Movimenti.Data_Movimento, Agenda.Lav_Cod, Agenda.Des_Lib, Agenda.Id_Agenda ")
                    strSql.AppendLine(" FROM  Mov_Destinazioni, Movimenti, Agenda ")

                    strSql.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    strSql.AppendLine(" AND   Agenda.Lav_Cod = 125  ")
                    strSql.AppendLine(" AND   Movimenti.Cau_Mov = '2200'")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 0 ")
                    strSql.AppendLine(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
                    strSql.AppendLine(" AND   Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Agenda.sa_Cod = Mov_Destinazioni.sa_Cod ")
                    strSql.AppendLine(" AND   Agenda.sa_Cod = Movimenti.sa_Cod ")
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Data_Movimento Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT DISTINCT Movimenti.Data_Movimento , Agenda.Lav_Cod , Agenda.Des_Lib , Agenda.Id_Agenda, Mov_Destinazioni.* " &
                                    " FROM  Mov_Destinazioni , Movimenti, Agenda " &
                                    " WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                    " AND   Agenda.Lav_Cod = 125 " & " " &
                                    " AND   Movimenti.Cau_Mov = '2200'" &
                                    " AND   Mov_Destinazioni.Tipo_Destinazione = 0 " &
                                    " AND   Agenda.Piva = Mov_Destinazioni.Piva " &
                                    " AND   Agenda.Piva = Movimenti.Piva " &
                                    " AND   Agenda.sa_Cod = Mov_Destinazioni.sa_Cod " &
                                    " AND   Agenda.sa_Cod = Movimenti.sa_Cod " &
                                    " AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                    " AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " &
                                    " AND   Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Data_Movimento Asc ")
                    End If

            End Select

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


    Public Function Leggi_Semine_Trapianti(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Id_Agenda As Integer,
                                           ByVal Id_Mov As Integer,
                                           ByVal Id_Mov_Det As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal Id_Destinazione As Integer,
                                           ByVal Tipo_Destinazione As Integer,
                                           ByVal Validita_Inizio As Date,
                                           ByVal Validita_Fine As Date,
                                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni.Leggi_Semine_Trapianti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT DISTINCT Movimenti.Data_Movimento , Agenda.Lav_Cod , Agenda.Des_Lib , Agenda.Id_Agenda, Mov_Destinazioni.* " &
                                    " FROM  Mov_Destinazioni , Movimenti, Agenda " &
                                    " WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                    " AND   Agenda.Lav_Cod In (2, 71) " & " " &
                                    " AND   Movimenti.Cau_Mov = '2300' " &
                                    " AND   Mov_Destinazioni.Tipo_Destinazione = 0 " &
                                    " AND   Agenda.Piva = Mov_Destinazioni.Piva " &
                                    " AND   Agenda.Piva = Movimenti.Piva " &
                                    " AND   Agenda.sa_Cod = Mov_Destinazioni.sa_Cod " &
                                    " AND   Agenda.sa_Cod = Movimenti.sa_Cod " &
                                    " AND   Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda " &
                                    " AND   Agenda.Id_Agenda = Movimenti.Id_Agenda " &
                                    " AND   Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Mov_Det <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Destinazione <> 0 Then
                        strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Data_Movimento Asc ")
                    End If

            End Select

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


    Public Function Leggi_Import_Agribologna(ByVal Piva As String,
                                             ByVal Cau_Mov As String,
                                             ByVal FinestraTemp_Inizio As Date,
                                             ByVal FinestraTemp_Fine As Date,
                                             ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni.Leggi_Import_Agribologna()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine("  SELECT Distinct *, [Movimenti].Sa_Cod ")
                    strSql.AppendLine("  FROM  [Movimenti] , [Agenda]  ")

                    strSql.AppendLine(" WHERE [Movimenti].Data_Movimento <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
                    strSql.AppendLine(" AND   [Movimenti].Data_Movimento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

                    strSql.AppendLine(" AND [Agenda].Piva = [Movimenti].Piva ")
                    strSql.AppendLine(" AND [Agenda].Id_Agenda = [Movimenti].Id_Agenda ")
                    strSql.AppendLine(" AND [Movimenti].Cau_Mov Not In ('6800', '8100', '6850')")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND [Movimenti].Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND  [Movimenti].Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Data_Movimento Asc ")
                    End If

            End Select

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


    Public Function Leggi_Semine_Trapianti_Import_Agribologna(ByVal Piva As String,
                                                              ByVal Sa_Cod As Integer,
                                                              ByVal Id_Agenda As Integer,
                                                              ByVal Elem_Cod As Integer,
                                                              ByVal Cau_Mov As String,
                                                              ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni.Leggi_Semine_Trapianti_Import_Agribologna()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Distinct [Agenda].* , [Movimenti].* , [Movimenti_Dettagli].* , [Mov_Destinazioni].* , [Mov_Destinazioni].Qta as Dest_Qta , [Movimenti_Dettagli].Qta as Dett_Qta  ")
                    strSql.AppendLine(" FROM  [Agenda] , [Movimenti] , [Movimenti_Dettagli] , [Mov_Destinazioni] ")

                    'Join sulla Piva
                    strSql.AppendLine(" WHERE [Agenda].Piva = [Movimenti].Piva ")
                    strSql.AppendLine(" AND   [Movimenti].Piva = [Movimenti_Dettagli].Piva ")
                    strSql.AppendLine(" AND   [Movimenti_Dettagli].Piva = [Mov_Destinazioni].Piva ")

                    'Join sul Sa_Cod
                    strSql.AppendLine(" AND [Agenda].Sa_Cod = [Movimenti].Sa_Cod ")
                    strSql.AppendLine(" AND [Movimenti_Dettagli].Sa_Cod = [Mov_Destinazioni].Sa_Cod ")

                    'Join sul Id_Agenda
                    strSql.AppendLine(" AND [Agenda].Id_Agenda = [Movimenti].Id_Agenda ")
                    strSql.AppendLine(" AND [Movimenti].Id_Agenda = [Movimenti_Dettagli].Id_Agenda ")
                    strSql.AppendLine(" AND [Movimenti_Dettagli].Id_Agenda = [Mov_Destinazioni].Id_Agenda ")

                    'Join su Id_Mov
                    strSql.AppendLine(" AND [Movimenti].Id_Mov = [Movimenti_Dettagli].Id_Mov ")
                    strSql.AppendLine(" AND [Movimenti_Dettagli].Id_Mov = [Mov_Destinazioni].Id_Mov ")

                    'Join su Id_Mov_Det
                    strSql.AppendLine(" AND [Movimenti_Dettagli].Id_Mov_Det = [Mov_Destinazioni].Id_Mov_Det ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND [Movimenti_Dettagli].Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND [Movimenti_Dettagli].Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND [Movimenti_Dettagli].Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND [Movimenti_Dettagli].Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND  [Movimenti].Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Data_Movimento Asc ")
                    End If

            End Select

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


    Public Function Leggi_Tracciabilita_Agribologna(ByVal Elem_Cod As Integer,
                                                    ByVal Cau_Mov As String,
                                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni.Leggi_Tracciabilita_Agribologna()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta ")
                    strSql.AppendLine(" FROM  Agenda , Movimenti , Movimenti_Dettagli , Mov_Destinazioni ")

                    strSql.AppendLine(" WHERE Agenda.Piva = Movimenti.Piva ")
                    strSql.AppendLine(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod")
                    strSql.AppendLine(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")

                    strSql.AppendLine(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND [Movimenti_Dettagli].Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        strSql.AppendLine(" AND  [Movimenti].Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Data_Movimento Asc ")
                    End If

            End Select

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


    Public Function Leggi_Destinazioni_Import_Agribologna(ByVal Piva As String,
                                                          ByVal Sa_Cod As Long,
                                                          ByVal Id_Agenda As Long,
                                                          ByVal Id_Mov As Long,
                                                          ByVal FinestraTemp_Inizio As Date,
                                                          ByVal FinestraTemp_Fine As Date,
                                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                          ByVal xFiltroAggiuntivo As String,
                                                          ByVal xOrderBy As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni.Leggi_Destinazioni_Import_Agribologna()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Distinct [Mov_Destinazioni].* , [Agenda].Lav_Cod , [Agenda].Des_Lib, [Agenda].Validita_Inizio as xValidita_Inizio  ")
                    strSql.AppendLine(" FROM  [Mov_Destinazioni], [Agenda] ")

                    strSql.AppendLine(" WHERE [Agenda].Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
                    strSql.AppendLine(" AND   [Agenda].Validita_Inizio >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
                    strSql.AppendLine(" AND   [Agenda].Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

                    'Join sulla Piva
                    strSql.AppendLine(" AND   [Agenda].Piva = [Mov_Destinazioni].Piva ")

                    'Join sul Id_Agenda
                    strSql.AppendLine(" AND [Agenda].Id_Agenda = [Mov_Destinazioni].Id_Agenda ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND [Agenda].Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND [Mov_Destinazioni].Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND [Mov_Destinazioni].Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND [Mov_Destinazioni].Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY xValidita_Inizio Asc ")
                    End If

            End Select

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


    Public Function Leggi_PrincipiAttiviXImpianto(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal Id_Reg As Integer,
                                                  ByVal Data_LimiteInferiorexConfronto As Date,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_PrincipiAttiviXImpianto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT    Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod,  ")
            strSql.AppendLine(" Mov_Destinazioni.Appezza,Mov_Destinazioni.Id_Destinazione, ")
            strSql.AppendLine(" FormulatixPrincipiAttivi.Pa_Cod, max(Movimenti.Data_Movimento) as dataPA ")
            strSql.AppendLine(" FROM         Mov_Destinazioni INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND  ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND  ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            strSql.AppendLine(" FormulatixPrincipiAttivi ON Movimenti_dettagli.Pro_Cod = FormulatixPrincipiAttivi.Fr_Cod ")

            strSql.AppendLine(" WHERE Mov_Destinazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Mov_Destinazioni.Validita_inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            strSql.AppendLine(" AND   Movimenti.Data_Movimento >=  " & Agro_SQL_SaveDate(Data_LimiteInferiorexConfronto) & " ")

            strSql.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.AppendLine(" AND (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.AppendLine(" AND (Movimenti.Cau_Mov = '2050') ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" GROUP BY Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod,  ")
            strSql.AppendLine(" Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, FormulatixPrincipiAttivi.Pa_Cod ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY FormulatixPrincipiAttivi.Pa_Cod Asc ")
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


    Public Function Leggi_PrincipiAttiviXImpianto2(ByVal Piva As String,
                                                   ByVal Sa_Cod As Integer,
                                                   ByVal Appezza As Integer,
                                                   ByVal Id_Reg As Integer,
                                                   ByVal Validita_Inizio As Date,
                                                   ByVal Validita_Fine As Date,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_PrincipiAttiviXImpianto2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Mov_Destinazioni.Id_Agenda, Movimenti.Data_Movimento,   ")
            strSql.AppendLine(" Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Reg_Impianti.Sup_Imp, Mov_Destinazioni.Qta2 AS Sup_Trattata, ")
            strSql.AppendLine(" Mov_Destinazioni.Qta, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Pro_Cod,  Formulati.Fr_Des, PrincipiAttivi.Pa_Des, FormulatixPrincipiAttivi.Pa_Cod, FormulatixPrincipiAttivi.Titolo ")

            strSql.AppendLine(" FROM      Movimenti INNER JOIN ")
            strSql.AppendLine("           Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine("           Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND  ")
            strSql.AppendLine("           Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            strSql.AppendLine("           Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine("           Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  ")
            strSql.AppendLine("           Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine("           Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod INNER JOIN ")
            strSql.AppendLine("           FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN ")
            strSql.AppendLine("           PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod ")

            strSql.AppendLine(" WHERE Movimenti.Data_Movimento >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine(" AND   Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            strSql.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.AppendLine(" AND (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.AppendLine(" AND (Movimenti.Cau_Mov = '2050') ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_dettagli.Pro_Cod, FormulatixPrincipiAttivi.Pa_Cod Asc ")
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

    Public Function Leggi_TrattamentiImpianto(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal Id_Reg As Integer,
                                              ByVal Validita_Inizio As Date,
                                              ByVal Validita_Fine As Date,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_TrattamentiImpianto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Mov_Destinazioni.Id_Agenda, Movimenti.Data_Movimento,   ")
            strSql.AppendLine("        Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Reg_Impianti.Sup_Imp, Mov_Destinazioni.Qta2 AS Sup_Trattata, ")
            strSql.AppendLine("        Mov_Destinazioni.Qta, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Pro_Cod,  Formulati.Fr_Des ")
            strSql.AppendLine("        , Movimenti_dettagli.PrincipiAttivi, Movimenti_dettagli.PrincipiAttiviPesi ")

            strSql.AppendLine(" FROM Movimenti INNER JOIN ")
            strSql.AppendLine("      Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine("      Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            strSql.AppendLine("      Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  ")
            strSql.AppendLine("      Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND  ")
            strSql.AppendLine("      Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            strSql.AppendLine("      Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine("      Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  ")
            strSql.AppendLine("      Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine("      Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")
            strSql.AppendLine(" INNER JOIN Agenda ON Movimenti.Id_Agenda = Agenda.Id_Agenda")

            strSql.AppendLine(" WHERE Movimenti.Data_Movimento >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            strSql.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.AppendLine(" AND (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.AppendLine(" AND (Movimenti.Cau_Mov = '2050') ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_dettagli.Pro_Cod Asc ")
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

    Public Function Leggi_TrattamentiImpianto_su_Avversita(ByVal Piva As String,
                                                           ByVal Sa_Cod As Integer,
                                                           ByVal Appezza As Integer,
                                                           ByVal Id_Reg As Integer,
                                                           ByVal strAvversita As String,
                                                           ByVal strGruppiAvversita As String,
                                                           ByVal Validita_Inizio As Date,
                                                           ByVal Validita_Fine As Date,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_TrattamentiImpianto_su_Avversita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Mov_Destinazioni.Id_Agenda, Movimenti.Data_Movimento,   ")
            strSql.AppendLine(" Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Reg_Impianti.Sup_Imp, Mov_Destinazioni.Qta2 AS Sup_Trattata, ")
            strSql.AppendLine(" Mov_Destinazioni.Qta, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Pro_Cod,  Formulati.Fr_Des ")
            strSql.AppendLine(" ,Movimenti_dettagli.PrincipiAttivi, Movimenti_dettagli.PrincipiAttiviPesi ")

            strSql.AppendLine(" FROM      Movimenti INNER JOIN ")
            strSql.AppendLine("           Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine("           Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND  ")
            strSql.AppendLine("           Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            strSql.AppendLine("           Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine("           Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  ")
            strSql.AppendLine("           Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine("           Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod INNER JOIN ")
            strSql.AppendLine("           Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")

            strSql.AppendLine(" WHERE Movimenti.Data_Movimento >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine(" AND   Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            strSql.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.AppendLine(" AND (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.AppendLine(" AND (Movimenti.Cau_Mov = '2050') ")

            If strAvversita <> "" Then
                strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Av_Cod IN " & Agro_SQL_Save_Clausola_IN(strAvversita, False) & "  ")
            End If
            If strGruppiAvversita <> "" Then
                strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Av_Gru IN " & Agro_SQL_Save_Clausola_IN(strGruppiAvversita, False) & "  ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_dettagli.Pro_Cod Asc ")
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


    Public Function Leggi_TrattamentiImpiantoEsteso(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Id_Reg As Integer,
                                                    ByVal strAvversita As String,
                                                    ByVal strGruppiAvversita As String,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_TrattamentiImpiantoEsteso()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Mov_Destinazioni.Id_Agenda, Movimenti.Data_Movimento,   ")
            strSql.AppendLine(" Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Reg_Impianti.Sup_Imp, Mov_Destinazioni.Qta2 AS Sup_Trattata, ")
            strSql.AppendLine(" Mov_Destinazioni.Qta, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Pro_Cod,  Formulati.Fr_Des ")
            strSql.AppendLine(" ,Movimenti_dettagli.Extra_Int, Movimenti_dettagli.Mezzo_Det, Movimenti_dettagli.Qta as Qta_Dose, Agenda.Des_Lib ")

            strSql.AppendLine(" FROM      Agenda INNER JOIN ")
            strSql.AppendLine("           Movimenti ON Agenda.PIVA = Movimenti.Piva AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")
            strSql.AppendLine("           INNER JOIN ")
            strSql.AppendLine("           Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine("           Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND  ")
            strSql.AppendLine("           Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            strSql.AppendLine("           Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine("           Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  ")
            strSql.AppendLine("           Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine("           Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod INNER JOIN ")
            strSql.AppendLine("           Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")

            strSql.AppendLine(" WHERE Movimenti.Data_Movimento >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine(" AND   Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            strSql.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.AppendLine(" AND (Movimenti_dettagli.Elem_Cod = 191) ")
            strSql.AppendLine(" AND (Movimenti.Cau_Mov = '2050') ")

            If strAvversita <> "" Then
                strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Av_Cod IN " & Agro_SQL_Save_Clausola_IN(strAvversita, False) & "  ")
            End If
            If strGruppiAvversita <> "" Then
                strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Av_Gru IN " & Agro_SQL_Save_Clausola_IN(strGruppiAvversita, False) & "  ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_dettagli.Pro_Cod Asc ")
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


    Public Function Leggi_DistribuzioneInsettiImpianto(ByVal Piva As String,
                                                       ByVal Sa_Cod As Integer,
                                                       ByVal Appezza As Integer,
                                                       ByVal Id_Reg As Integer,
                                                       ByVal strIns_Cod As String,
                                                       ByVal strAvversita As String,
                                                       ByVal Validita_Inizio As Date,
                                                       ByVal Validita_Fine As Date,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_DistribuzioneInsettiImpianto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Mov_Destinazioni.Id_Agenda, Movimenti.Data_Movimento,   ")
            strSql.AppendLine(" Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Reg_Impianti.Sup_Imp, Mov_Destinazioni.Qta2 AS Sup_Trattata, ")
            strSql.AppendLine(" Mov_Destinazioni.Qta, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Pro_Cod,  Formulati.Fr_Des ")
            strSql.AppendLine(" ,Movimenti_dettagli.PrincipiAttivi ")

            strSql.AppendLine(" FROM      Movimenti INNER JOIN ")
            strSql.AppendLine("           Movimenti_dettagli INNER JOIN ")
            strSql.AppendLine("           Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  ")
            strSql.AppendLine("           Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND  ")
            strSql.AppendLine("           Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            strSql.AppendLine("           Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine("           Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  ")
            strSql.AppendLine("           Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine("           Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod INNER JOIN ")
            'l'avversità (Mov_Dettaglio_Tecnico) è figlia di Movimenti (non di Movimenti_dettagli = prodotto)
            strSql.AppendLine("           Mov_Dettaglio_Tecnico ON Movimenti.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND  ")
            strSql.AppendLine("           Movimenti.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND Movimenti.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov   ")

            strSql.AppendLine(" WHERE Movimenti.Data_Movimento >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine(" AND   Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            strSql.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0) ")
            strSql.AppendLine(" AND (Movimenti_dettagli.Elem_Cod = 196) ")
            strSql.AppendLine(" AND (Movimenti.Cau_Mov = '2050') ")

            If strAvversita <> "" Then
                strSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Av_Cod IN " & Agro_SQL_Save_Clausola_IN(strAvversita, False) & "  ")
            End If

            If strIns_Cod <> "" Then
                strSql.AppendLine(" AND Movimenti_dettagli.Pro_Cod IN " & Agro_SQL_Save_Clausola_IN(strIns_Cod, False) & "  ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            'filtro aggiuntivo
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti_dettagli.Pro_Cod Asc ")
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

    '#########################################################
    '====================================================================================
    'Parametri opzionali :
    '   Lav_Cod = 0
    '   Piva = ""            
    '   Sa_Cod = 0           
    '   Id_Mov = 0           
    '   Cod_RisUm = 0       
    '   Cau_Mov = ""   
    ' Anno =0
    ' Data_Movimento = agrodatainizio
    ' Scadenza= agrodatafine
    ' Doc_Numero_Sin = XYZ (perché stringa vuota è significativa)
    ' Doc_Numero = 0
    ' Doc_Numero_Des = XYZ (perché stringa vuota è significativa)
    ' Progr_Protocollo = 0
    ' Progr_Registrazione =0
    ' Data_Registrazione = agrodatainizio
    '====================================================================================
    Public Function MovimentiContabili(ByVal Lav_Cod As Integer,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod_Dest As Integer,
                                       ByVal Id_Destinazione As Integer,
                                       ByVal Id_Agenda As Integer,
                                       ByVal Id_Mov As Integer,
                                       ByVal Cod_RisUm As Integer,
                                       ByVal Cau_Mov As String,
                                       ByVal Anno As Integer,
                                       ByVal Data_Movimento As Date,
                                       ByVal Scadenza As Date,
                                       ByVal Doc_Numero_Sin As String,
                                       ByVal Doc_Numero As Integer,
                                       ByVal Doc_Numero_Des As String,
                                       ByVal Progr_Protocollo As Integer,
                                       ByVal Progr_Registrazione As Integer,
                                       ByVal Data_Registrazione As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.MovimentiContabili()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")

            strSql.AppendLine(" FROM    Agenda  ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

            strSql.AppendLine(" INNER JOIN Movimenti_Dettagli ON Movimenti_Dettagli.PIVA = Movimenti.PIVA AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov  ")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Movimenti_Dettagli.PIVA = Mov_Destinazioni.PIVA AND Movimenti_Dettagli.sa_cod = Mov_Destinazioni.sa_cod AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            strSql.AppendLine(" WHERE Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Lav_Cod <> 0 Then
                strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
            End If

            If Anno <> 0 Then
                strSql.AppendLine(" AND Year(Movimenti.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & "  ")
            End If

            If Data_Movimento <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
            End If

            If Scadenza <> AGRODATAFINE AndAlso Scadenza <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Scadenza = " & UCase(Agro_SQL_SaveDate(Scadenza)) & "   ")
            End If

            If Doc_Numero_Sin.ToUpper <> "XYZ" Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero_Sin = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Sin)) & "'   ")
            End If

            If Doc_Numero <> 0 Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
            End If

            If Doc_Numero_Des.ToUpper <> "XYZ" Then
                strSql.AppendLine(" AND Movimenti.Doc_Numero_Des = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Des)) & "'   ")
            End If

            If Progr_Protocollo <> 0 Then
                strSql.AppendLine(" AND Movimenti.Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & "   ")
            End If

            If Progr_Registrazione <> 0 Then
                strSql.AppendLine(" AND Movimenti.Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & "   ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Registrazione = " & UCase(Agro_SQL_SaveDate(Data_Registrazione)) & "   ")
            End If

            If Sa_Cod_Dest <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_Dest) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Piva, Data_Movimento, Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des  Asc ")
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

    '#########################################################
    '====================================================================================
    'Parametri opzionali :
    '   Lav_Cod = 0
    '   Piva = ""            
    '   Sa_Cod = 0           
    '   Id_Mov = 0         
    '   Cau_Mov = ""   
    ' Data_Movimento = agrodatainizio
    '====================================================================================
    Public Function CarichiScarichi(ByVal Lav_Cod As Integer,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Destinazione As Integer,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Id_Mov As Integer,
                                    ByVal Cau_Mov As String,
                                    ByVal Data_Movimento As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_R.CarichiScarichi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")

            strSql.AppendLine(" FROM    Agenda  ")
            'AND Agenda.Sa_Cod = Movimenti.Sa_Cod: NOOOOOOOOOOOOOOOO
            'altrimenti non vengono su i trasferimenti!!!!!
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA  AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

            strSql.AppendLine(" INNER JOIN Movimenti_Dettagli ON Movimenti_Dettagli.PIVA = Movimenti.PIVA AND Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov  ")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Movimenti_Dettagli.PIVA = Mov_Destinazioni.PIVA AND Movimenti_Dettagli.sa_cod = Mov_Destinazioni.sa_cod AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            strSql.AppendLine(" WHERE   Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND     Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND     Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "' ) ")

            If Lav_Cod <> 0 Then
                strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Movimenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.AppendLine(" AND Movimenti.Cau_Mov = '" & UCase(Agro_SQL_SaveText(Cau_Mov)) & "'   ")
            End If

            If Data_Movimento <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Movimenti.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Movimenti.Piva, Data_Movimento, Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des  Asc ")
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


    '#################################################################################################################################
    Public Function Leggi_Prodotti_Raccolta(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                            ByVal Id_Agenda As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Prodotti_Raccolta()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT Mov_Destinazioni.Qta, Movimenti_dettagli.Udm_Cod, UnitaMisura.UDM_SIM ")
            strSql.AppendLine(" FROM Movimenti_dettagli INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND ")
            strSql.AppendLine(" Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine(" UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD ")
            strSql.AppendLine(" WHERE Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND Reg_Impianti.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine(" AND Reg_Impianti.id_reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Reg_Impianti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Reg_Impianti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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


    '#################################################################################################################################
    Public Function Leggi_Dati_Irrigazione(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal Id_Reg As Integer,
                                           ByVal Id_Agenda As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dati_Irrigazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT Mov_Destinazioni.Qta, Mov_Dettaglio_Tecnico.Qta_Ril, Mov_Dettaglio_Tecnico.Dett_Cod, UnitaMisura.UDM_SIM,  ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Dose, Mov_Dettaglio_Tecnico.Parziale, Mov_Dettaglio_Tecnico.Freatimetro, ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Inn1_Data, Mov_Dettaglio_Tecnico.Inn2_Data ")

            strSql.AppendLine(" FROM Agenda INNER JOIN ")
            strSql.AppendLine(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND ")
            strSql.AppendLine(" Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND ")
            strSql.AppendLine(" Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico ON Mov_Destinazioni.Piva = Mov_Dettaglio_Tecnico.Piva AND ")
            strSql.AppendLine(" Mov_Destinazioni.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND ")
            strSql.AppendLine(" Mov_Destinazioni.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" UnitaMisura ON Mov_Dettaglio_Tecnico.Dett_Cod = UnitaMisura.UDM_COD ")

            strSql.AppendLine(" WHERE Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            strSql.AppendLine(" AND Agenda.Lav_Cod = 1 ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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

    '#################################################################################################################################
    Public Function Leggi_Dati_OsservazioneFasi(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Appezza As Integer,
                                                ByVal Id_Reg As Integer,
                                                ByVal Id_Agenda As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dati_OsservazioneFasi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT FasiFenologiche.FF_DES, dbo.Mov_Destinazioni.validita_inizio AS data_ril ")
            strSql.AppendLine(" FROM         dbo.Mov_Dettaglio_Tecnico INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti INNER JOIN ")
            strSql.AppendLine(" dbo.Agenda ON dbo.Movimenti.PIVA = dbo.Agenda.PIVA AND dbo.Movimenti.Sa_Cod = dbo.Agenda.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti.Id_Agenda = dbo.Agenda.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti_dettagli ON dbo.Movimenti.PIVA = dbo.Movimenti_dettagli.PIVA AND dbo.Movimenti.Sa_Cod = dbo.Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti.Id_Agenda = dbo.Movimenti_dettagli.Id_Agenda AND dbo.Movimenti.Id_Mov = dbo.Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine(" dbo.Appezzamento INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Destinazioni ON dbo.Appezzamento.PIVA = dbo.Mov_Destinazioni.Piva AND ")
            strSql.AppendLine(" dbo.Appezzamento.SA_COD = dbo.Mov_Destinazioni.Sa_Cod AND dbo.Appezzamento.APPEZZA = dbo.Mov_Destinazioni.Appezza ON ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.PIVA = dbo.Mov_Destinazioni.Piva AND dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Destinazioni.Id_Agenda AND dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det ON dbo.Mov_Dettaglio_Tecnico.Piva = dbo.Movimenti_dettagli.PIVA AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Sa_Cod = dbo.Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Id_Agenda = dbo.Movimenti_dettagli.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Id_Mov = dbo.Movimenti_dettagli.Id_Mov AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Id_Mov_Det = dbo.Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.Cultivar Cultivar INNER JOIN ")
            strSql.AppendLine(" dbo.Reg_Impianti ON Cultivar.Cul_Cod = dbo.Reg_Impianti.CUL_COD INNER JOIN ")
            strSql.AppendLine(" dbo.SpecieVegetali SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON dbo.Mov_Destinazioni.Piva = dbo.Reg_Impianti.PIVA AND ")
            strSql.AppendLine(" dbo.Mov_Destinazioni.Sa_Cod = dbo.Reg_Impianti.SA_COD AND dbo.Mov_Destinazioni.Appezza = dbo.Reg_Impianti.APPEZZA AND ")
            strSql.AppendLine(" dbo.Mov_Destinazioni.Id_Destinazione = dbo.Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine(" dbo.FasiFenologiche FasiFenologiche ON dbo.Mov_Dettaglio_Tecnico.FF_Classe = FasiFenologiche.FF_COD ")

            strSql.AppendLine(" WHERE Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Reg_Impianti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Reg_Impianti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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

    '#################################################################################################################################
    Public Function Leggi_Dati_OsservazioneFasi_ByStrIdAgenda(ByVal Piva As String,
                                                              ByVal Str_Id_Agenda As String,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dati_OsservazioneFasi_ByStrIdAgenda()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT FasiFenologiche.FF_DES, dbo.Mov_Destinazioni.validita_inizio AS data_ril, ")
            strSql.AppendLine(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.appezza, Mov_Destinazioni.Id_Destinazione ")
            strSql.AppendLine(" FROM         dbo.Mov_Dettaglio_Tecnico INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti INNER JOIN ")
            strSql.AppendLine(" dbo.Agenda ON dbo.Movimenti.PIVA = dbo.Agenda.PIVA AND dbo.Movimenti.Sa_Cod = dbo.Agenda.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti.Id_Agenda = dbo.Agenda.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti_dettagli ON dbo.Movimenti.PIVA = dbo.Movimenti_dettagli.PIVA AND dbo.Movimenti.Sa_Cod = dbo.Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti.Id_Agenda = dbo.Movimenti_dettagli.Id_Agenda AND dbo.Movimenti.Id_Mov = dbo.Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine(" dbo.Appezzamento INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Destinazioni ON dbo.Appezzamento.PIVA = dbo.Mov_Destinazioni.Piva AND ")
            strSql.AppendLine(" dbo.Appezzamento.SA_COD = dbo.Mov_Destinazioni.Sa_Cod AND dbo.Appezzamento.APPEZZA = dbo.Mov_Destinazioni.Appezza ON ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.PIVA = dbo.Mov_Destinazioni.Piva AND dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Destinazioni.Id_Agenda AND dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det ON dbo.Mov_Dettaglio_Tecnico.Piva = dbo.Movimenti_dettagli.PIVA AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Sa_Cod = dbo.Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Id_Agenda = dbo.Movimenti_dettagli.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Id_Mov = dbo.Movimenti_dettagli.Id_Mov AND ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Id_Mov_Det = dbo.Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.Cultivar Cultivar INNER JOIN ")
            strSql.AppendLine(" dbo.Reg_Impianti ON Cultivar.Cul_Cod = dbo.Reg_Impianti.CUL_COD INNER JOIN ")
            strSql.AppendLine(" dbo.SpecieVegetali SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON dbo.Mov_Destinazioni.Piva = dbo.Reg_Impianti.PIVA AND ")
            strSql.AppendLine(" dbo.Mov_Destinazioni.Sa_Cod = dbo.Reg_Impianti.SA_COD AND dbo.Mov_Destinazioni.Appezza = dbo.Reg_Impianti.APPEZZA AND ")
            strSql.AppendLine(" dbo.Mov_Destinazioni.Id_Destinazione = dbo.Reg_Impianti.ID_REG INNER JOIN ")
            strSql.AppendLine(" dbo.FasiFenologiche FasiFenologiche ON dbo.Mov_Dettaglio_Tecnico.FF_Classe = FasiFenologiche.FF_COD ")

            strSql.AppendLine(" WHERE Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Mov_Destinazioni.id_agenda IN (" & Agro_SQL_Save_Clausola_IN(Str_Id_Agenda, False) & ") ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Reg_Impianti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Appezzamento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Reg_Impianti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Appezzamento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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


    Public Function Leggi_UltimaFaseFenologicaImpianti_InData(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Appezza As Integer,
                                                ByVal Id_Reg As Integer,
                                                ByVal Data As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_FaseFenologicaImpianti_InData()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Mov_Dettaglio_Tecnico.FF_Classe, Mov_Destinazioni.validita_inizio AS data_ril ")
            strSql.AppendLine(" FROM Mov_Dettaglio_Tecnico INNER JOIN ")
            strSql.AppendLine(" Movimenti INNER JOIN ")
            strSql.AppendLine(" Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti.Id_Agenda = Agenda.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine(" Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine(" Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ON Mov_Dettaglio_Tecnico.Piva = Movimenti_dettagli.PIVA AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov AND ")
            strSql.AppendLine(" Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  ")
            strSql.AppendLine(" WHERE agenda.lav_cod = " & Agro_SQL_SaveNum(LAVCOD_FASI_FENOLOGICHE) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.validita_inizio <= " & Agro_SQL_SaveDate(Data))

            If Piva <> "" Then
                strSql.AppendLine(" AND Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            'strSql.AppendLine(" WHERE Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            'strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            'strSql.AppendLine(" AND Mov_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            'strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            'strSql.AppendLine(" AND Mov_Destinazioni.id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Mov_Destinazioni.validita_inizio desc")
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


    '#################################################################################################################################
    Public Function Leggi_Dati_RilievoAvversitaTrappole(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Appezza As Integer,
                                                        ByVal Id_Reg As Integer,
                                                        ByVal Id_Agenda As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dati_RilievoAvversitaTrappole()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT dbo.Movimenti_dettagli.Pro_Cod, dbo.Trappole.TRAP_DES, dbo.Mov_Dettaglio_Tecnico.Qta_Ril, ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Av_Cod, dbo.Mov_Dettaglio_Tecnico.Av_Gru, ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Sigla_AV, dbo.Avversita.Av_Des_Vol, dbo.GruppoAvversita.Av_Gru_Des ")
            strSql.AppendLine(" FROM         dbo.Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti ON dbo.Agenda.PIVA = dbo.Movimenti.PIVA AND dbo.Agenda.Sa_Cod = dbo.Movimenti.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Agenda.Id_Agenda = dbo.Movimenti.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti_dettagli ON dbo.Movimenti.PIVA = dbo.Movimenti_dettagli.PIVA AND dbo.Movimenti.Sa_Cod = dbo.Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti.Id_Agenda = dbo.Movimenti_dettagli.Id_Agenda AND dbo.Movimenti.Id_Mov = dbo.Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Destinazioni ON dbo.Movimenti_dettagli.PIVA = dbo.Mov_Destinazioni.Piva AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Destinazioni.Sa_Cod AND dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Destinazioni.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico ON dbo.Movimenti_dettagli.PIVA = dbo.Mov_Dettaglio_Tecnico.Piva AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Dettaglio_Tecnico.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Dettaglio_Tecnico.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Dettaglio_Tecnico.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Dettaglio_Tecnico.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.Trappole ON dbo.Movimenti_dettagli.Pro_Cod = dbo.Trappole.TRAP_COD LEFT OUTER JOIN ")
            strSql.AppendLine(" dbo.GruppoAvversita ON dbo.Mov_Dettaglio_Tecnico.Av_Gru = dbo.GruppoAvversita.Av_Gru LEFT OUTER JOIN ")
            strSql.AppendLine(" dbo.Avversita ON dbo.Mov_Dettaglio_Tecnico.Av_Cod = dbo.Avversita.Av_Cod ")

            strSql.AppendLine(" WHERE Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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


    '#################################################################################################################################
    Public Function Leggi_Dati_RilievoAvversitaCampo(ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Appezza As Integer,
                                                     ByVal Id_Reg As Integer,
                                                     ByVal Id_Agenda As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dati_RilievoAvversitaCampo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Pro_Cod, dbo.Mov_Dettaglio_Tecnico.Qta_Ril, dbo.Mov_Dettaglio_Tecnico.Av_Cod, dbo.Mov_Dettaglio_Tecnico.Av_Gru, ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.Sigla_AV, dbo.Avversita.Av_Des_Vol, dbo.GruppoAvversita.Av_Gru_Des, dbo.Mov_Destinazioni.Qta, ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Udm_Cod, dbo.UnitaMisura.UDM_SIM, dbo.UnitaMisura.UDM_DES ")
            strSql.AppendLine(" FROM dbo.Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti ON dbo.Agenda.PIVA = dbo.Movimenti.PIVA AND dbo.Agenda.Sa_Cod = dbo.Movimenti.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Agenda.Id_Agenda = dbo.Movimenti.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti_dettagli ON dbo.Movimenti.PIVA = dbo.Movimenti_dettagli.PIVA AND dbo.Movimenti.Sa_Cod = dbo.Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti.Id_Agenda = dbo.Movimenti_dettagli.Id_Agenda AND dbo.Movimenti.Id_Mov = dbo.Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Destinazioni ON dbo.Movimenti_dettagli.PIVA = dbo.Mov_Destinazioni.Piva AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Destinazioni.Sa_Cod AND dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Destinazioni.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico ON dbo.Movimenti_dettagli.PIVA = dbo.Mov_Dettaglio_Tecnico.Piva AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Dettaglio_Tecnico.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Dettaglio_Tecnico.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Dettaglio_Tecnico.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Dettaglio_Tecnico.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.UnitaMisura ON dbo.Movimenti_dettagli.Udm_Cod = dbo.UnitaMisura.UDM_COD LEFT OUTER JOIN ")
            strSql.AppendLine(" dbo.GruppoAvversita ON dbo.Mov_Dettaglio_Tecnico.Av_Gru = dbo.GruppoAvversita.Av_Gru LEFT OUTER JOIN ")
            strSql.AppendLine(" dbo.Avversita ON dbo.Mov_Dettaglio_Tecnico.Av_Cod = dbo.Avversita.Av_Cod ")

            strSql.AppendLine(" WHERE Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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


    '#################################################################################################################################
    Public Function Leggi_Dati_RilievoIndiciMaturita(ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Appezza As Integer,
                                                     ByVal Id_Reg As Integer,
                                                     ByVal Id_Agenda As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_Dati_RilievoIndiciMaturita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT DISTINCT dbo.Mov_Dettaglio_Tecnico.Dett_Cod AS UDM_COD, dbo.UnitaMisura.UDM_SIM, dbo.Mov_Destinazioni.Qta, ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico.FF_Classe AS Ind_Mat_Cod, dbo.IndiciMaturita.IND_MAT_DES ")
            strSql.AppendLine(" FROM         dbo.Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti ON dbo.Agenda.PIVA = dbo.Movimenti.PIVA AND dbo.Agenda.Sa_Cod = dbo.Movimenti.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Agenda.Id_Agenda = dbo.Movimenti.Id_Agenda INNER JOIN ")
            strSql.AppendLine(" dbo.Movimenti_dettagli ON dbo.Movimenti.PIVA = dbo.Movimenti_dettagli.PIVA AND dbo.Movimenti.Sa_Cod = dbo.Movimenti_dettagli.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti.Id_Agenda = dbo.Movimenti_dettagli.Id_Agenda AND dbo.Movimenti.Id_Mov = dbo.Movimenti_dettagli.Id_Mov INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Destinazioni ON dbo.Movimenti_dettagli.PIVA = dbo.Mov_Destinazioni.Piva AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Destinazioni.Sa_Cod AND dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Destinazioni.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Destinazioni.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.Mov_Dettaglio_Tecnico ON dbo.Movimenti_dettagli.PIVA = dbo.Mov_Dettaglio_Tecnico.Piva AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Sa_Cod = dbo.Mov_Dettaglio_Tecnico.Sa_Cod AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Agenda = dbo.Mov_Dettaglio_Tecnico.Id_Agenda AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov = dbo.Mov_Dettaglio_Tecnico.Id_Mov AND ")
            strSql.AppendLine(" dbo.Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Dettaglio_Tecnico.Id_Mov_Det INNER JOIN ")
            strSql.AppendLine(" dbo.UnitaMisura ON dbo.Mov_Dettaglio_Tecnico.Dett_Cod = dbo.UnitaMisura.UDM_COD INNER JOIN ")
            strSql.AppendLine(" dbo.IndiciMaturita ON dbo.Mov_Dettaglio_Tecnico.FF_Classe = dbo.IndiciMaturita.IND_MAT_COD ")

            strSql.AppendLine(" WHERE Mov_Destinazioni.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato >=0 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato >=0 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato  >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Destinazioni.Inviato =-1 ")
                    strSql.AppendLine(" AND   Movimenti_Dettagli.Inviato =-1 ")
                    strSql.AppendLine(" AND   Mov_Dettaglio_Tecnico.Inviato  =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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


    '#############################################################################################
    Public Function NewCom_Mov_Destinazioni_Impianti_Leggi(ByRef objParametri As AgronicaCoreParametri,
                                                           Optional ByVal Piva As String = "",
                                                           Optional ByVal Sa_Cod As Integer = 0,
                                                           Optional ByVal Id_Agenda As Integer = 0,
                                                           Optional ByVal Id_Mov As Integer = 0,
                                                           Optional ByVal Id_Mov_Det As Integer = 0,
                                                           Optional ByVal Appezza As Integer = 0,
                                                           Optional ByVal Id_Destinazione As Integer = 0,
                                                           Optional ByVal Tipo_Destinazione As Integer = 0,
                                                           Optional ByVal FinestraTemp_Inizio As String = "01/01/1900",
                                                           Optional ByVal FinestraTemp_Fine As String = "31/12/2100",
                                                           Optional ByVal FiltroAggiuntivo As String = "",
                                                           Optional ByVal Ordinamento As String = ""
                                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.NewCom_Mov_Destinazioni_Impianti_Leggi()"
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Try

            Dim stbQuery As New StringBuilder

            stbQuery.AppendLine(" SELECT Imprese.Rag_Soc AS Rag_Soc, ")
            stbQuery.AppendLine(" Agenda.*, Mov_Destinazioni.* ")

            'JOIN AGENDA - IMPRESE
            stbQuery.AppendLine(" FROM    Agenda  ")
            stbQuery.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - MOVIMENTI DESTINAZIONI
            stbQuery.AppendLine(" INNER JOIN Mov_Destinazioni ")
            stbQuery.AppendLine(" ON Agenda.PIVA = Mov_Destinazioni.Piva ")
            stbQuery.AppendLine(" AND Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda  ")

            'CONDIZIONI
            stbQuery.AppendLine(" WHERE Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            stbQuery.AppendLine(" AND   Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            stbQuery.AppendLine(" AND   Agenda.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            If Piva <> "" Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Appezza <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            If Tipo_Destinazione <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
            End If

            If FiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri) & " ")
            End If

            If Ordinamento <> "" Then
                stbQuery.AppendLine(Ordinamento & " ")
            Else
                stbQuery.AppendLine(" ORDER BY Rag_Soc, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#############################################################################################
    Public Function Leggi_DistinctID_Agenda_Impianti(ByRef objParametri As AgronicaCoreParametri,
                                                     ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Appezza As Integer,
                                                     ByVal Id_Reg As Integer,
                                                     ByVal FinestraTemp_Inizio As Date,
                                                     ByVal FinestraTemp_Fine As Date,
                                                     ByVal FiltroAggiuntivo As String,
                                                     ByVal Ordinamento As String
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_DistinctID_Agenda_Impianti()"
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Try

            Dim stbQuery As New StringBuilder
            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            stbQuery.AppendLine(" SELECT DISTINCT Mov_Destinazioni.Id_Agenda ")

            stbQuery.AppendLine(" FROM Mov_Destinazioni ")
            stbQuery.AppendLine(" JOIN Movimenti ON Mov_Destinazioni.Piva = Movimenti.PIVA  ")
            stbQuery.AppendLine("       AND Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda ")
            stbQuery.AppendLine("       AND Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov ")

            stbQuery.AppendLine(" WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            stbQuery.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            stbQuery.AppendLine(" AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            stbQuery.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione >= " & Agro_SQL_SaveNum(TIPO_DESTINAZIONE_IMPIANTO) & " ")

            If Piva <> "" Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If FiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(FiltroAggiuntivo & " ")
            End If

            If Ordinamento <> "" Then
                stbQuery.AppendLine(Ordinamento & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#############################################################################################
    Public Function Controlla_Validita_Appezzamenti(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Controlla_Validita_Appezzamenti()"
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Try

            Dim stbQuery As New StringBuilder

            stbQuery.AppendLine("SELECT Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,")
            stbQuery.AppendLine("MIN(Agenda.Validita_Inizio) AS Data_Operazione_Max_Validita_inizio, MAX(Agenda.Validita_Inizio) AS Data_Operazione_Min_Validita_fine")

            stbQuery.AppendLine(" FROM Mov_Destinazioni ")
            stbQuery.AppendLine(" JOIN Agenda  ")
            stbQuery.AppendLine(" ON Mov_Destinazioni.Id_Agenda = Agenda.Id_Agenda")

            stbQuery.AppendLine(" WHERE Mov_Destinazioni.Tipo_Destinazione = 0 ")

            If Piva <> "" Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                stbQuery.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stbQuery.AppendLine(" GROUP BY Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza")

            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Legge semine e trapianti effettuati negli esercizi passati come parametro oppure in tutta l'azienda se non forniti esercizi
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <param name="piva"></param>
    ''' <param name="listaImpianti"></param>
    ''' <returns></returns>
    Public Function LeggiSemineStampaXLS(ByRef objParametri As AgronicaCoreParametri,
                                         ByVal piva As String,
                                         ByVal listaImpianti As List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti)
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.LeggiSemineStampaXLS()"
        Dim dt As New DataTable

        Try

            Dim stbQuery As New StringBuilder

            If listaImpianti.Count > 0 Then
                stbQuery.AppendLine("declare @listaImpianti table (")
                stbQuery.AppendLine("    piva varchar(25),")
                stbQuery.AppendLine("    sa_cod int,")
                stbQuery.AppendLine("    appezza int,")
                stbQuery.AppendLine("    id_reg int")
                stbQuery.AppendLine(")")
                stbQuery.AppendLine("")
                stbQuery.AppendLine("insert @listaImpianti values")

                Dim insertImpianti As New List(Of String)

                For Each imp In listaImpianti
                    insertImpianti.Add(String.Format("('{0}', {1}, {2}, {3})", imp.PIVA, imp.SA_COD, imp.APPEZZA, imp.ID_REG))
                Next

                stbQuery.AppendLine(String.Join("," & vbCrLf, insertImpianti))
                stbQuery.AppendLine("")
                stbQuery.AppendLine("")
            End If


            stbQuery.AppendLine("SELECT Agenda.Piva ")
            stbQuery.AppendLine(" ,CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Agenda.Piva ELSE Imprese.partitaIvaReale END AS PivaReale")
            stbQuery.AppendLine(" ,Imprese.rag_soc")
            stbQuery.AppendLine(" ,Agenda.Id_Agenda")
            stbQuery.AppendLine(" ,Movimenti.Id_Mov")
            stbQuery.AppendLine(" ,Movimenti_dettagli.Id_Mov_Det")
            stbQuery.AppendLine(" ,Mov_Destinazioni.Appezza")
            stbQuery.AppendLine(" ,Mov_Destinazioni.Id_Destinazione")
            stbQuery.AppendLine(" ,Mov_Destinazioni.Tipo_Destinazione")
            stbQuery.AppendLine(" ,Agenda.Lav_Cod")
            stbQuery.AppendLine(" ,Operazioni.Lav_Des")
            stbQuery.AppendLine(" ,Movimenti.Data_Movimento")
            stbQuery.AppendLine(" ,Reg_Impianti.Id_Reg")
            stbQuery.AppendLine(" ,Cultivar.Cul_Cod")
            stbQuery.AppendLine(" ,Cultivar.Cul_Des")
            stbQuery.AppendLine(" ,SpecieVegetali.Veg_Cod")
            stbQuery.AppendLine(" ,SpecieVegetali.Veg_Des")
            stbQuery.AppendLine(" ,Materie_Prime.Mat_Cod")
            stbQuery.AppendLine(" ,Materie_Prime.Mat_Des")
            stbQuery.AppendLine(" ,Materie_Prime.Cod_Articolo")
            stbQuery.AppendLine(" ,Movimenti_dettagli.Lotto as lotto")
            stbQuery.AppendLine(" ,'Cod.' + Materie_Prime.Cod_Articolo +")
            stbQuery.AppendLine("    CASE WHEN Movimenti_dettagli.Lotto <> '' AND Movimenti_dettagli.Lotto <> 'indefinito' THEN")
            stbQuery.AppendLine("    ' - Lotto: ' + Movimenti_dettagli.Lotto ELSE '' END")
            stbQuery.AppendLine("    AS dett_mat_prima")
            stbQuery.AppendLine(" ,Movimenti_dettagli.Udm_Cod")
            stbQuery.AppendLine(" ,UnitaMisura.UDM_SIM")
            stbQuery.AppendLine(" ,UnitaMisura.UDM_DES")
            stbQuery.AppendLine(" ,Mov_Destinazioni.Qta")
            stbQuery.AppendLine(" ,Mov_Destinazioni.Qta2 as sup_trattata")
            stbQuery.AppendLine(" ,Centri_Aziendali.sa_cod")
            stbQuery.AppendLine(" ,Centri_Aziendali.sa_nome")
            stbQuery.AppendLine(" ,Appezzamento.APP_NOME")
            'stbQuery.AppendLine(" ,Reg_Impianti.Validita_Inizio")
            'stbQuery.AppendLine(" ,Reg_Impianti.Validita_Fine")
            'stbQuery.AppendLine(" ,Reg_Impianti.Sup_Imp")
            stbQuery.AppendLine(" ,Reg_Impianti_PianoSemina.Val_Cod as piano_semina")
            stbQuery.AppendLine(" ,Impresa_Cooperativa.rag_soc as coop_referente")
            stbQuery.AppendLine(" ,Imprese_Progetti.Progetto_Nome")
            stbQuery.AppendLine(" ,Imprese_Progetti.Regolamento_Cod")
            stbQuery.AppendLine(" ,Imprese_Progetti.Validita_Inizio")
            stbQuery.AppendLine(" ,Imprese_Progetti.Validita_Fine")
            stbQuery.AppendLine(" FROM Agenda")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN Imprese")
            stbQuery.AppendLine(" ON Imprese.PIVA = Agenda.Piva")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN Operazioni")
            stbQuery.AppendLine(" ON Operazioni.Lav_Cod = Agenda.Lav_Cod")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN Movimenti")
            stbQuery.AppendLine(" ON Agenda.Piva = Movimenti.Piva")
            stbQuery.AppendLine(" AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN Movimenti_dettagli")
            stbQuery.AppendLine(" ON Movimenti.Piva = Movimenti_dettagli.Piva")
            stbQuery.AppendLine(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda")
            stbQuery.AppendLine(" AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" LEFT JOIN Materie_Prime")
            stbQuery.AppendLine(" ON Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN UnitaMisura")
            stbQuery.AppendLine(" ON UnitaMisura.UDM_COD = Movimenti_dettagli.Udm_Cod")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN Mov_Destinazioni")
            stbQuery.AppendLine(" ON Movimenti_dettagli.Piva = Mov_Destinazioni.Piva  ")
            stbQuery.AppendLine(" AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod  ")
            stbQuery.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda  ")
            stbQuery.AppendLine(" AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  ")
            stbQuery.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  ")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN Centri_Aziendali")
            stbQuery.AppendLine(" ON Centri_Aziendali.PIVA = Mov_Destinazioni.Piva")
            stbQuery.AppendLine(" AND Centri_Aziendali.sa_cod = Mov_Destinazioni.Sa_Cod")
            stbQuery.AppendLine("")

            If listaImpianti.Count > 0 Then
                stbQuery.AppendLine(" INNER JOIN @listaImpianti as impianti_filtrati")
                stbQuery.AppendLine(" ON impianti_filtrati.PIVA = Mov_Destinazioni.Piva  ")
                stbQuery.AppendLine(" AND impianti_filtrati.SA_COD = Mov_Destinazioni.Sa_Cod  ")
                stbQuery.AppendLine(" AND impianti_filtrati.APPEZZA = Mov_Destinazioni.Appezza  ")
                stbQuery.AppendLine(" AND impianti_filtrati.ID_REG = Mov_Destinazioni.Id_Destinazione ")
                stbQuery.AppendLine("")
            End If

            stbQuery.AppendLine(" INNER JOIN Reg_Impianti")
            stbQuery.AppendLine(" ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva  ")
            stbQuery.AppendLine(" AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod  ")
            stbQuery.AppendLine(" AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza  ")
            stbQuery.AppendLine(" AND Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" LEFT JOIN Cultivar")
            stbQuery.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" LEFT JOIN SpecieVegetali")
            stbQuery.AppendLine(" ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" INNER JOIN Appezzamento")
            stbQuery.AppendLine(" ON Appezzamento.PIVA = Reg_Impianti.PIVA  ")
            stbQuery.AppendLine(" AND Appezzamento.SA_COD = Reg_Impianti.SA_COD  ")
            stbQuery.AppendLine(" AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA  ")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" LEFT JOIN Imprese_Progetti")
            stbQuery.AppendLine(" ON Imprese_Progetti.Piva = Reg_Impianti.PIVA ")
            stbQuery.AppendLine(" AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD ")
            stbQuery.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.APPEZZA ")
            stbQuery.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG ")
            stbQuery.AppendLine(" AND Movimenti.Data_Movimento BETWEEN Imprese_Progetti.Validita_Inizio AND Imprese_Progetti.Validita_Fine ")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" LEFT JOIN Reg_Impianti_Codici AS Reg_Impianti_PianoSemina")
            stbQuery.AppendLine(" ON Reg_Impianti_PianoSemina.PIVA = Imprese_Progetti.PIVA  ")
            stbQuery.AppendLine(" AND Reg_Impianti_PianoSemina.SA_COD = Imprese_Progetti.SA_COD  ")
            stbQuery.AppendLine(" AND Reg_Impianti_PianoSemina.APPEZZA = Imprese_Progetti.APPEZZA  ")
            stbQuery.AppendLine(" AND Reg_Impianti_PianoSemina.ID_REG = Imprese_Progetti.ID_REG ")
            stbQuery.AppendLine(" AND Reg_Impianti_PianoSemina.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
            stbQuery.AppendLine(" AND Reg_Impianti_PianoSemina.Id_Cod IN(" & enum_CodiciAnagrafe.Impianto_PianoSemina & ")")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" LEFT JOIN Reg_Impianti_Codici AS Reg_Impianti_Cooperativa")
            stbQuery.AppendLine(" ON Reg_Impianti_Cooperativa.PIVA = Imprese_Progetti.PIVA  ")
            stbQuery.AppendLine(" AND Reg_Impianti_Cooperativa.SA_COD = Imprese_Progetti.SA_COD  ")
            stbQuery.AppendLine(" AND Reg_Impianti_Cooperativa.APPEZZA = Imprese_Progetti.APPEZZA  ")
            stbQuery.AppendLine(" AND Reg_Impianti_Cooperativa.ID_REG = Imprese_Progetti.ID_REG ")
            stbQuery.AppendLine(" AND Reg_Impianti_Cooperativa.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
            stbQuery.AppendLine(" AND Reg_Impianti_Cooperativa.Id_Cod IN(" & enum_CodiciAnagrafe.Impianto_Cooperativa & ")")
            stbQuery.AppendLine("")
            stbQuery.AppendLine(" LEFT JOIN Imprese AS Impresa_Cooperativa")
            stbQuery.AppendLine(" ON Impresa_Cooperativa.PIVA = Reg_Impianti_Cooperativa.val_cod")
            stbQuery.AppendLine("")

            Dim arrLavCod As Integer() = {LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING}

            stbQuery.AppendLine("WHERE Agenda.Lav_Cod IN(" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCod)) & ")")
            stbQuery.AppendLine("AND Movimenti.Cau_Mov = '" & CAU_LAVORAZIONE & "'")
            stbQuery.AppendLine("")
            'stbQuery.AppendLine("ORDER BY coop_referente, rag_soc, sa_nome, app_nome, id_agenda")
            stbQuery.AppendLine("ORDER BY rag_soc, Movimenti.Data_Movimento, SpecieVegetali.Veg_Des, Cultivar.Cul_Des")

            If listaImpianti.Count = 0 AndAlso piva <> "" Then
                stbQuery.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'   ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_PresenzaImpianti_Da_IdAgenda(Id_Agenda As Integer,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.Leggi_PresenzaImpianti_Da_IdAgenda()"

        Dim messaggioErrore As String
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Dim PresenzaImpianti As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ISNULL(COUNT(0), 0) AS CountImpianti ")
            StrSQL.AppendLine(" FROM Mov_Destinazioni ")
            StrSQL.AppendLine(" WHERE Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            StrSQL.AppendLine(" AND Tipo_Destinazione = " & TIPO_DESTINAZIONE_IMPIANTO & " ")

            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                PresenzaImpianti = dt.Rows(0)("CountImpianti") > 0
            End If

        Catch ex As Exception
            Return True
        End Try

        Return PresenzaImpianti
    End Function

    ''' <summary>
    ''' Legge la cronologia dei movimenti da un lista di chiavi (app/impianti)
    ''' </summary>
    ''' <param name="listChiavi">Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod</param>
    ''' <param name="profonditaJoin"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiCronologiaMovimenti_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer)),
                                                     profonditaJoin As Enum_EntitaModificaMultiplaPianoColturale,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_R.LeggiCronologiaMovimenti_Massivo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim flagConnessione, flagTransazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroImpianti(listChiavi, nomeRoutine, objParametri)

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT ")
            strSql.AppendLine("     Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione ")
            strSql.AppendLine(" FROM Mov_Destinazioni (NOLOCK) ")
            strSql.AppendLine(" JOIN Movimenti ON Movimenti.Piva = Mov_Destinazioni.Piva ")
            strSql.AppendLine(" AND Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            strSql.AppendLine(" AND Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")

            If listChiavi IsNot Nothing AndAlso listChiavi.Count > 0 Then
                strSql.AppendLine("	JOIN #TempImpianto temp (NOLOCK) ON Mov_Destinazioni.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
                strSql.AppendLine("	AND Mov_Destinazioni.Sa_Cod = temp.Sa_Cod ")
                If profonditaJoin >= Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti Then
                    strSql.AppendLine("	AND Mov_Destinazioni.Appezza = temp.Appezza ")
                End If
                If profonditaJoin >= Enum_EntitaModificaMultiplaPianoColturale.Impianti Then
                    strSql.AppendLine("	AND Mov_Destinazioni.Id_Destinazione = temp.Id_Reg ")
                End If
            End If

            strSql.Append(" WHERE 1 = 1 ")

            strSql.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = 0 ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroImpianti(nomeRoutine, objParametri)

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing

            'rollback transazione
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return dt

    End Function

    Private Function CreaTabellaTemp_FiltroChiavi() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempChiavi') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempChiavi ( ")
        stb.AppendLine("        Piva VARCHAR(50) NULL")
        stb.AppendLine("      , Sa_Cod INT NULL")
        stb.AppendLine("      , Appezza INT NULL")
        stb.AppendLine("      , Id_Reg INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Function EliminaTabellaTemp_FiltroChiavi() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempChiavi') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempChiavi ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Mov_Destinazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Destinazione As Integer,
                           ByVal Tipo_Destinazione As Integer,
                           ByVal Qta As Decimal,
                           ByVal Qta2 As Decimal,
                           ByVal Tipo_Scorta As Integer,
                           ByVal Scorta_Min As Decimal,
                           ByVal mov_destinazioni_graphickey As String,
                           ByVal QuotaDistribuzione As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Qta_Dest1 As Decimal = 0,
                           Optional ByVal Qta_Dest2 As Decimal = 0,
                           Optional ByVal Sup_Riduzione_BufferZone As Decimal = 0,
                           Optional ByVal Perc_Riduzione_Deriva As Decimal = 0,
                           Optional ByVal Sa_Cod_Riferimento As Integer = 0,
                           Optional ByVal Id_Destinazione_Riferimento As Integer = 0,
                           Optional ByVal Tipo_Destinazione_Riferimento As Integer = 0,
                           Optional ByVal Extra_Str As String = ""
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Mov_Destinazioni ")
            strSql.AppendLine("         ( Piva,      Sa_Cod,          Id_Agenda,         Id_Mov,    Id_Mov_Det, ")
            strSql.AppendLine("         Appezza,     Id_Destinazione, Tipo_Destinazione, Qta,       Qta2,       ")
            strSql.AppendLine("         Tipo_Scorta, Scorta_Min,      mov_destinazioni_graphickey, ")
            strSql.AppendLine("         Qta_Dest1,   Qta_Dest2,       QuotaDistribuzione,")
            strSql.AppendLine("         Sup_Riduzione_BufferZone,     Perc_Riduzione_Deriva, ")
            strSql.AppendLine("         Sa_Cod_Riferimento,   Id_Destinazione_Riferimento, Tipo_Destinazione_Riferimento,")
            strSql.AppendLine("         Extra_Str,")

            strSql.AppendLine("         Inviato,            DataInvio, ")
            strSql.AppendLine("         Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("         UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("         Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("          ) ")

            strSql.AppendLine(" VALUES  ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Destinazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta2) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Scorta) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Scorta_Min) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(mov_destinazioni_graphickey) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Dest1) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Dest2) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(QuotaDistribuzione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Riduzione_BufferZone) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Perc_Riduzione_Deriva) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod_Riferimento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Destinazione_Riferimento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Destinazione_Riferimento) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Extra_Str) & "'  ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine("         ) ")

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


    '########################################################################################
    'Nota Importante: la routine non accetta la modifica dei campi Tipo_Scorta e Scorta_Min.
    'Questo perché questa routine viene chiamata solo dal Giacenze_W e serve a modificare il quantitativo
    'in giacenza e la classe non conosce i valori dei 2 campi. Per farlo si dovrebbe provvedere
    'ad una lettura preventiva dentro Agro_Contab.Giacenze_W

    'La modifica mirata dei campi 'Tipo_Scorta' e 'Scorta_Min' avviene nella routine ModificaScorta
    <Obsolete("Usare la funzione ModificaPuntuale")>
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Destinazione As Integer,
                             ByVal Tipo_Destinazione As Integer,
                             ByVal Qta As Decimal,
                             ByVal Qta2 As Decimal,
                             ByVal mov_destinazioni_graphickey As String,
                             ByVal QuotaDistribuzione As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            If Id_Mov = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            End If

            If Id_Mov_Det = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Mov_Det obbligatorio)")
            End If

            'No, perché le operazioni di magazzino hanno appezza=0
            'If Appezza = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            'End If

            If Id_Destinazione = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Destinazione obbligatorio)")
            End If

            If Tipo_Destinazione = 0 Then
                Throw New Exception("Parametro non corretto nella query (Tipo_Destinazione obbligatorio)")
            End If


            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Mov_Destinazioni SET ")
            strSql.AppendLine("    Tipo_Destinazione    = " & Agro_SQL_SaveText(Tipo_Destinazione) & "  ")
            strSql.AppendLine("   ,Qta                  = " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.AppendLine("   ,Qta2                 = " & Agro_SQL_SaveNum(Qta2) & "  ")
            strSql.AppendLine("   ,mov_destinazioni_graphickey = '" & Agro_SQL_SaveText(mov_destinazioni_graphickey) & "'  ")
            strSql.AppendLine("   ,QuotaDistribuzione = " & Agro_SQL_SaveNum(QuotaDistribuzione) & "  ")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.AppendLine(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine(" AND   Id_Mov            =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine(" AND   Id_Mov_Det        =  " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.AppendLine(" AND   Appezza           =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine(" AND   Id_Destinazione   =  " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")
            strSql.AppendLine(" AND   Tipo_Destinazione =  " & Agro_SQL_SaveNum(Tipo_Destinazione) & "  ")

            If Qta2 <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Qta2 = " & Agro_SQL_SaveNum(Qta2) & "   ")
            End If


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByVal Id_Mov_Det As Integer,
                                     ByVal Appezza As Integer,
                                     ByVal Id_Destinazione As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Tipo_Destinazione As Integer? = Nothing,
                                     Optional ByVal Qta As Decimal? = Nothing,
                                     Optional ByVal Qta2 As Decimal? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Tipo_Scorta As Integer? = Nothing,
                                     Optional ByVal Scorta_Min As Decimal? = Nothing,
                                     Optional ByVal Mov_Destinazioni_Graphickey As String = Nothing,
                                     Optional ByVal Qta_Dest1 As Decimal? = Nothing,
                                     Optional ByVal Qta_Dest2 As Decimal? = Nothing,
                                     Optional ByVal QuotaDistribuzione As Decimal? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = "",
                                     Optional ByVal Sup_Riduzione_BufferZone As Decimal? = Nothing,
                                     Optional ByVal Perc_Riduzione_Deriva As Decimal? = Nothing,
                                     Optional ByVal Sa_Cod_Riferimento As Integer? = Nothing,
                                     Optional ByVal Id_Destinazione_Riferimento As Integer? = Nothing,
                                     Optional ByVal Tipo_Destinazione_Riferimento As Integer? = Nothing,
                                     Optional ByVal New_Sa_Cod As Integer? = Nothing,
                                     Optional ByVal New_Id_Destinazione As Integer? = Nothing,
                                     Optional ByVal Extra_Str As String = ""
                                     ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Mov_Destinazioni_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            'If Id_Mov = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            'End If

            'If Id_Mov_Det = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Mov_Det obbligatorio)")
            'End If

            'No, perché le operazioni di magazzino hanno appezza=0
            'If Appezza = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            'End If

            'No perché ci sono dei casi che è 0
            'If Id_Destinazione = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Destinazione obbligatorio)")
            'End If

            'No perché se è 0 ci sono i dati dell'impianto
            'If Tipo_Destinazione = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Tipo_Destinazione obbligatorio)")
            'End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Mov_Destinazioni ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")


            If Not IsNothing(Tipo_Destinazione) Then
                strSql.AppendLine("   , Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione) & " ")
            End If

            If Not IsNothing(Qta) Then
                strSql.AppendLine("   , Qta = " & Agro_SQL_SaveNum(Qta) & " ")
            End If

            If Not IsNothing(Qta2) Then
                strSql.AppendLine("   , Qta2 = " & Agro_SQL_SaveNum(Qta2) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Not IsNothing(Tipo_Scorta) Then
                strSql.AppendLine("   , Tipo_Scorta = " & Agro_SQL_SaveNum(Tipo_Scorta) & " ")
            End If

            If Not IsNothing(Scorta_Min) Then
                strSql.AppendLine("   , Scorta_Min = " & Agro_SQL_SaveNum(Scorta_Min) & " ")
            End If

            If Not IsNothing(Mov_Destinazioni_Graphickey) Then
                strSql.AppendLine("   , Mov_Destinazioni_Graphickey = '" & Agro_SQL_SaveText(Mov_Destinazioni_Graphickey) & "' ")
            End If

            If Not IsNothing(Qta_Dest1) Then
                strSql.AppendLine("   , Qta_Dest1 = " & Agro_SQL_SaveNum(Qta_Dest1) & " ")
            End If

            If Not IsNothing(Qta_Dest2) Then
                strSql.AppendLine("   , Qta_Dest2 = " & Agro_SQL_SaveNum(Qta_Dest2) & " ")
            End If

            If Not IsNothing(QuotaDistribuzione) Then
                strSql.AppendLine("   , QuotaDistribuzione = " & Agro_SQL_SaveNum(QuotaDistribuzione) & " ")
            End If

            If Not IsNothing(Sup_Riduzione_BufferZone) Then
                strSql.AppendLine("   , Sup_Riduzione_BufferZone = " & Agro_SQL_SaveNum(Sup_Riduzione_BufferZone) & " ")
            End If

            If Not IsNothing(Perc_Riduzione_Deriva) Then
                strSql.AppendLine("   , Perc_Riduzione_Deriva = " & Agro_SQL_SaveNum(Perc_Riduzione_Deriva) & " ")
            End If

            If Not IsNothing(Sa_Cod_Riferimento) Then
                strSql.AppendLine("   , Sa_Cod_Riferimento = " & Agro_SQL_SaveNum(Sa_Cod_Riferimento) & " ")
            End If

            If Not IsNothing(Id_Destinazione_Riferimento) Then
                strSql.AppendLine("   , Id_Destinazione_Riferimento = " & Agro_SQL_SaveNum(Id_Destinazione_Riferimento) & " ")
            End If

            If Not IsNothing(Tipo_Destinazione_Riferimento) Then
                strSql.AppendLine("   , Tipo_Destinazione_Riferimento = " & Agro_SQL_SaveNum(Tipo_Destinazione_Riferimento) & " ")
            End If

            If Not IsNothing(New_Sa_Cod) Then
                strSql.AppendLine("   , Sa_Cod = " & Agro_SQL_SaveNum(New_Sa_Cod) & " ")
            End If

            If Not IsNothing(New_Id_Destinazione) Then
                strSql.AppendLine("   , Id_Destinazione = " & Agro_SQL_SaveNum(New_Id_Destinazione) & " ")
            End If

            If Extra_Str <> "" Then
                strSql.AppendLine("   , Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "' ")
            End If

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & " ")
            End If

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

    Public Function Modifica_Quantita(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Id_Mov As Integer,
                                      ByVal Id_Mov_Det As Integer,
                                      ByVal Appezza As Integer,
                                      ByVal Id_Destinazione As Integer,
                                      ByVal Qta As Decimal,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Modifica_Quantita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Mov_Destinazioni SET ")
            strSql.AppendLine("    Qta              = " & Agro_SQL_SaveNum(Qta) & "  ")

            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.AppendLine(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine(" AND   Id_Mov            =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine(" AND   Id_Mov_Det        =  " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.AppendLine(" AND   Appezza           =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine(" AND   Id_Destinazione   =  " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    Public Function Modifica_SupTrattata(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Id_Mov As Integer,
                                         ByVal Id_Mov_Det As Integer,
                                         ByVal Appezza As Integer,
                                         ByVal Id_Destinazione As Integer,
                                         ByVal Qta2 As Decimal,
                                         ByVal QuotaDistribuzione As Decimal,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Modifica_SupTrattata()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Mov_Destinazioni SET ")
            strSql.AppendLine("    Qta2              = " & Agro_SQL_SaveNum(Qta2) & "  ")
            strSql.AppendLine("    ,QuotaDistribuzione  = " & Agro_SQL_SaveNum(QuotaDistribuzione) & "  ")

            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.AppendLine(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine(" AND   Id_Mov            =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine(" AND   Id_Mov_Det        =  " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.AppendLine(" AND   Appezza           =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine(" AND   Id_Destinazione   =  " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    Public Function Modifica_Quantita_e_SupTrattata(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal Id_Mov As Integer,
                                                    ByVal Id_Mov_Det As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Id_Destinazione As Integer,
                                                    ByVal Qta As Decimal,
                                                    ByVal Qta2 As Decimal,
                                                    ByVal QuotaDistribuzione As Decimal,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Modifica_Quantita_e_SupTrattata()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Mov_Destinazioni SET ")
            strSql.AppendLine("    Qta              = " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.AppendLine("    ,Qta2              = " & Agro_SQL_SaveNum(Qta2) & "  ")
            strSql.AppendLine("    ,QuotaDistribuzione  = " & Agro_SQL_SaveNum(QuotaDistribuzione) & "  ")

            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.AppendLine(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine(" AND   Id_Mov            =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.AppendLine(" AND   Id_Mov_Det        =  " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.AppendLine(" AND   Appezza           =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine(" AND   Id_Destinazione   =  " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    Public Function Modifica_Quantita_e_SupTrattata_DaPercentuale(ByVal Piva As String,
                                                                  ByVal Id_Agenda As Integer,
                                                                  ByVal Id_Mov As Integer,
                                                                  ByVal Id_Mov_Det As Integer,
                                                                  ByVal Appezza As Integer,
                                                                  ByVal Id_Destinazione As Integer,
                                                                  ByVal Percentuale As Double,
                                                                  ByVal xFiltroAggiuntivo As String,
                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Modifica_Quantita_e_SupTrattata_DaPercentuale()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Mov_Destinazioni SET ")
            strSql.AppendLine("    Qta              = Qta * " & Agro_SQL_SaveNum(Percentuale) & "/100  ")
            strSql.AppendLine("    ,Qta2              = Qta2 * " & Agro_SQL_SaveNum(Percentuale) & "/100  ")

            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If
            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If
            If Appezza <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If
            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    Public Function Modifica_x_Trasferimento(ByVal sa_cod As Integer,
                                             ByVal id_destinazione As Integer,
                                             ByVal Str_id_agenda_id_mov_id_mov_det As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Modifica_x_Trasferimento()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Str_id_agenda_id_mov_id_mov_det = "" Then
                Throw New Exception("Parametro non corretto nella query (Str_id_agenda_id_mov_id_mov_det obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Mov_Destinazioni SET ")
            strSql.AppendLine("    sa_cod    = " & Agro_SQL_SaveNum(sa_cod) & "  ")
            strSql.AppendLine("   ,id_destinazione                  = " & Agro_SQL_SaveNum(id_destinazione) & "  ")

            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE    (" & Str_id_agenda_id_mov_id_mov_det & ")")

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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Destinazione As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Destinazioni_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Appezza = 0
        '   Id_Destinazione = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Mov_Destinazioni ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Mov_Destinazioni ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If


            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")


            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.AppendLine(" AND Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    Public Sub Scrivi(ByRef Mov_Destinazioni As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Destinazioni_W.Scrivi()"
        Dim messaggioErrore As String = ""

        Try
            Valorizza(Mov_Destinazioni, objParametriServer)

            Mov_Destinazioni.data_creazione = DateTime.Now
            Mov_Destinazioni.data_modifica = DateTime.Now
            Mov_Destinazioni.username_creazione = objParametriServer.UsernameOperazione
            Mov_Destinazioni.username_modifica = objParametriServer.UsernameOperazione

            GiasContext.Mov_Destinazioni.Add(Mov_Destinazioni)
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef Mov_Destinazioni As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Destinazioni_W.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Mov_Destinazioni, objParametriServer)

            Mov_Destinazioni.data_modifica = DateTime.Now
            Mov_Destinazioni.username_modifica = objParametriServer.UsernameOperazione

            GiasContext.Entry(Mov_Destinazioni).State = EntityState.Modified
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Valorizza(ByRef Mov_Destinazioni As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni,
                         ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Destinazioni_W.Valorizza()"
        Dim messaggioErrore As String = ""

        Try

            If Mov_Destinazioni.inviato Is Nothing Then
                Mov_Destinazioni.inviato = 0
            End If

            If Mov_Destinazioni.data_creazione Is Nothing Then
                Mov_Destinazioni.data_creazione = DateTime.Now
            End If

            If Mov_Destinazioni.username_creazione Is Nothing Then
                Mov_Destinazioni.username_creazione = objParametriServer.UsernameOperazione
            End If

            If Mov_Destinazioni.mov_destinazioni_graphickey Is Nothing Then
                Mov_Destinazioni.mov_destinazioni_graphickey = ""
            End If

            If Mov_Destinazioni.Qta_Dest1 Is Nothing Then
                Mov_Destinazioni.Qta_Dest1 = 0
            End If

            If Mov_Destinazioni.Qta_Dest2 Is Nothing Then
                Mov_Destinazioni.Qta_Dest2 = 0
            End If

            If Mov_Destinazioni.QuotaDistribuzione Is Nothing Then
                Mov_Destinazioni.QuotaDistribuzione = 0
            End If

            If Mov_Destinazioni.validita_inizio < AGRODATAINIZIO Then
                Mov_Destinazioni.validita_inizio = AGRODATAINIZIO
            End If

            If Mov_Destinazioni.validita_fine < AGRODATAINIZIO Then
                Mov_Destinazioni.validita_fine = AGRODATAFINE
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Elimina(ByRef Mov_Destinazioni As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Destinazioni_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            GiasContext.Mov_Destinazioni.Remove(Mov_Destinazioni)
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class
