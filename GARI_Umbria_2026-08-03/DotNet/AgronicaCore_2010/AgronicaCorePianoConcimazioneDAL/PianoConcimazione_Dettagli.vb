Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class PianoConcimazione_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '  Galassi, 24/02/2017 11.42.15: Copiato dai Core dell'anagrafe
    '##############################################################################################
    Public Function Leggi_default(ByVal PC_Testata_Cod As Integer, _
                            ByVal PC_Dettagli_Cod As Integer, _
                            ByVal PC_Dettagli_PIVA As String, _
                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & vbCrLf)
                    StrSQL.Append(" FROM    PianoConcimazione_Dettagli " & vbCrLf)
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND     PC_Dettagli_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)

                    If PC_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
                    End If

                    If PC_Dettagli_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Dettagli_Cod = " & Agro_SQL_SaveNum(PC_Dettagli_Cod) & " " & vbCrLf)
                    End If

                    If PC_Dettagli_PIVA <> "" Then
                        StrSQL.Append(" AND PC_Dettagli_PIVA = '" & Agro_SQL_SaveText(PC_Dettagli_PIVA) & "' " & vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function Leggi( _
            ByVal PC_Testata_Cod As Int32, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT PD.*, PC_Testata_Des, Regolamento_Cod, PC_Tipo, PT.validita_inizio AS validita_inizio_piano, PT.validita_fine AS validita_fine_piano ")
            StrSQL.Append(" , ISNULL(PT.Note, '') AS Note , ISNULL(Pt.Flag_NonUtilizzo_Fertilizzanti,0) AS Flag_NonUtilizzo_Fertilizzanti ")
            StrSQL.Append(" , A.Analisi_Testata_Data_Inizio,a.Analisi_Testata_Data_Fine  ")
            StrSQL.Append(" FROM  PianoConcimazione_Dettagli PD INNER JOIN PianoConcimazione_Testata PT  ")
            StrSQL.Append(" ON PD.PC_Dettagli_SuperUser = PT.PC_SuperUser AND PD.PC_Testata_Cod = PT.PC_Testata_Cod ")
            StrSQL.Append(" left outer join Analisi_Testata A on PD.PC_Dettagli_SuperUser = A.Analisi_SuperUser AND PD.PC_Dettagli_Analisi_Testata_Cod = A.Analisi_Testata_Cod ")

            StrSQL.Append(" WHERE   (PC_Dettagli_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")

            If PC_Testata_Cod <> 0 Then
                StrSQL.Append(" AND     PD.PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PD.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PD.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_xStatistiche_PianoConcimazione_Utenti(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                  Optional ByVal Applica_VisibilitaUtente As Boolean = False
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_xStatistiche_PianoConcimazione_Utenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Try

            strSql.Length = 0

            strSql.Append(" select  distinct " & vbCrLf)

            strSql.Append(" p.PC_Dettagli_PIVA as piva, p.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From PianoConcimazione_Dettagli p " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=p.Username_Creazione ")

            If Applica_VisibilitaUtente Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON p.PC_Dettagli_PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where p.validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   p.validita_fine >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where p.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   p.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("         AND   p.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   p.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY piva  ")
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

End Class
Public Class PianoConcimazione_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '  Galassi, 24/02/2017 11.40.33: Copiato dal vecchio Core di Anagrafe
    '##############################################################################################
    Public Function Cancella(ByVal PC_Testata_Cod As Integer, _
                             ByVal PC_Dettagli_Cod As Integer, _
                             ByVal PC_Dettagli_Piva As String, _
                               ByVal xFiltroAggiuntivo As String, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (PC_Testata_Cod obbligatorio)")
            End If

            'If PC_Dettagli_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (PC_Dettagli_Cod obbligatorio)")
            'End If

            If PC_Dettagli_Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (PC_Dettagli_Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE PianoConcimazione_Dettagli ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   PC_Dettagli_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Dettagli_Piva = '" & Agro_SQL_SaveText(PC_Dettagli_Piva) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    PianoConcimazione_Dettagli ")
                StrSQL.Append(" WHERE   PC_Dettagli_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Dettagli_Piva = '" & Agro_SQL_SaveText(PC_Dettagli_Piva) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")


            End If

            If PC_Dettagli_Cod <> 0 Then
                StrSQL.Append(" AND     PC_Dettagli_Cod = " & Agro_SQL_SaveNum(PC_Dettagli_Cod) & " ")
            End If


            '---------------------------------------------

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


    '##############################################################################################
    Public Function Scrivi(ByVal PC_Testata_Cod As Integer,
                           ByVal PC_Dettagli_Cod As Integer,
                           ByVal PC_Dettagli_PIVA As String,
                           ByVal PC_Dettagli_Appezzamenti_Des As String,
                           ByVal PC_Dettagli_Finalita_GRFI_COD As Integer,
                           ByVal PC_Dettagli_FaseCicloColturale_fase_des As String,
                           ByVal PC_Dettagli_FaseCicloColturale_id_fase As Integer,
                           ByVal PC_Dettagli_Ciclo_EP_COD As Integer,
                           ByVal PC_Dettagli_ListDurataAnni As Integer,
                           ByVal PC_Dettagli_Resa As Decimal,
                           ByVal PC_Dettagli_Precessione_Veg_Cod As Integer,
                           ByVal PC_Dettagli_FinalitaPrecessione_Grfi_Cod As Integer,
                           ByVal PC_Dettagli_Condizionidelterreno_Des As String,
                           ByVal PC_Dettagli_Condizionidelterreno_id_terreno As Integer,
                           ByVal PC_Dettagli_Sabbia As Decimal,
                           ByVal PC_Dettagli_Limo As Decimal,
                           ByVal PC_Dettagli_Argilla As Decimal,
                           ByVal PC_Dettagli_Ph As Decimal,
                           ByVal PC_Dettagli_Caco3 As Decimal,
                           ByVal PC_Dettagli_Caco3_Attivo As Decimal,
                           ByVal PC_Dettagli_CN As Decimal,
                           ByVal PC_Dettagli_So As Decimal,
                           ByVal PC_Dettagli_ntot As Decimal,
                           ByVal PC_Dettagli_p2o5 As Decimal,
                           ByVal PC_Dettagli_k2o As Decimal,
                           ByVal PC_Dettagli_Quantita As Decimal,
                           ByVal PC_Dettagli_Frequenza As Integer,
                           ByVal PC_Dettagli_fn As Decimal,
                           ByVal PC_Dettagli_fp2o5 As Decimal,
                           ByVal PC_Dettagli_fk2o As Decimal,
                           ByVal PC_Dettagli_fss As Decimal,
                           ByVal PC_Dettagli_Quantita1 As Decimal,
                           ByVal PC_Dettagli_f1n As Decimal,
                           ByVal PC_Dettagli_f1p2o5 As Decimal,
                           ByVal PC_Dettagli_f1k2o As Decimal,
                           ByVal PC_Dettagli_f1ss As Decimal,
                           ByVal PC_Dettagli_Fertilizzazione_id_tp_fer As Integer,
                           ByVal PC_Dettagli_Fertilizzazione1_id_tp_fer As Integer,
                           ByVal PC_Dettagli_Piovosita As Decimal,
                           ByVal PC_Dettagli_AreaVulnerabile As Integer,
                           ByVal PC_Dettagli_Anno As Integer,
                           ByVal PC_Dettagli_AreaOmogenea As String,
                           ByVal PC_Dettagli_Campioneterreno As String,
                           ByVal PC_Dettagli_Prova As String,
                           ByVal PC_Dettagli_Datadal As Date,
                           ByVal PC_Dettagli_DataAl As Date,
                           ByVal PC_Dettagli_Quadrante_Des As String,
                           ByVal PC_Dettagli_Quadrante_Classe As Integer,
                           ByVal PC_Dettagli_Quadrante_Cella As Integer,
                           ByVal PC_Dettagli_ColturaPrincipale_Veg_Cod As Integer,
                           ByVal PC_Dettagli_RegimeIrriguo As Integer,
                           ByVal PC_Dettagli_Anticipazioni As Integer,
                           ByVal PC_Dettagli_Anticipazioni_Anni As Integer,
                           ByVal PC_Ubicazione_Cod As Integer,
                           ByVal PC_PercFissazioneN As Decimal,
                           ByVal PC_Copertura As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal N_Ammesso As Decimal,
                           ByVal P_Ammesso As Decimal,
                           ByVal K_Ammesso As Decimal,
                           ByVal PC_Dettagli_Mg As Decimal,
                           ByVal PC_Dettagli_CSC As Decimal,
                           ByVal PC_Dettagli_Analisi_Testata_Cod As Integer,
                           ByVal PC_Dettagli_Piovosita_Febbraio As Decimal,
                           ByVal PC_Dettagli_SaCod As Integer,
                           ByVal PC_Dettagli_P As Decimal,
                           ByVal PC_Dettagli_Flag_P As Integer,
                           ByVal PC_Dettagli_K As Decimal,
                           ByVal PC_Dettagli_Flag_K As Integer,
                           ByVal Allegati_Documenti_Cod As Integer,
                           ByVal N_Mas As Decimal?,
                           ByVal N_Mas_Regolamento_Tipo As Integer?,
                           ByVal PC_Dettagli_Precessione_Veg_Cod_AnnoPrecedente As Integer,
                           ByVal PC_Dettagli_Precessione_Veg_Cod_2anni_prima As Integer,
                           ByVal PC_Dettagli_Precessione_Veg_Cod_3anni_prima As Integer,
                           ByVal PC_Dettagli_Piovosita_Primavera As Decimal,
                           ByVal PC_Dettagli_norg As Decimal,
                           ByVal PC_Dettagli_bio As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal PC_Dettagli_Resa_Storica As Decimal = 0,
                            Optional ByVal PC_Dettagli_Flag_Residui_Precessione_Asportati As Integer = 0,
                            Optional ByVal PC_Dettagli_Mese_Residui_Precessione_Interramento As Integer = 0,
                            Optional ByVal PC_Dettagli_Mese_Semina As Integer = 0,
                            Optional ByVal PC_Dettagli_Mese_Raccolta As Integer = 0,
                            Optional ByVal PC_Dettagli_Temperatura_Media_ColturaInCampo As Decimal = 0,
                            Optional ByVal PC_Dettagli_Temperatura_Media_MeseSemina_Febbraio As Decimal = 0,
                            Optional ByVal PC_Dettagli_Perc_Umidita_Coltura_Principale As Decimal = 0,
                            Optional ByVal PC_Dettagli_Perc_Umidita_Raccolta_Precessione As Decimal = 0
                           ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO PianoConcimazione_Dettagli " & vbCrLf)

            StrSQL.Append(" (PC_Dettagli_SuperUser, PC_Testata_Cod, PC_Dettagli_Cod, PC_Dettagli_PIVA,PC_Dettagli_SaCod, PC_Dettagli_Appezzamenti_Des, PC_Dettagli_Finalita_GRFI_COD, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_FaseCicloColturale_fase_des, PC_Dettagli_FaseCicloColturale_id_fase, PC_Dettagli_Ciclo_EP_COD, PC_Dettagli_ListDurataAnni, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Resa, PC_Dettagli_Precessione_Veg_Cod, PC_Dettagli_FinalitaPrecessione_Grfi_Cod, PC_Dettagli_Condizionidelterreno_Des, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Condizionidelterreno_id_terreno, PC_Dettagli_Sabbia, PC_Dettagli_Limo, PC_Dettagli_Argilla, PC_Dettagli_Ph, PC_Dettagli_Caco3, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Caco3_Attivo, PC_Dettagli_CN, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_So, PC_Dettagli_ntot, PC_Dettagli_p2o5, PC_Dettagli_k2o, PC_Dettagli_Quantita, PC_Dettagli_Frequenza, PC_Dettagli_fn, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_fp2o5, PC_Dettagli_fk2o, PC_Dettagli_fss, PC_Dettagli_Quantita1, PC_Dettagli_f1n, PC_Dettagli_f1p2o5, PC_Dettagli_f1k2o, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_f1ss, PC_Dettagli_Fertilizzazione_id_tp_fer, PC_Dettagli_Fertilizzazione1_id_tp_fer, PC_Dettagli_Piovosita, PC_Dettagli_AreaVulnerabile, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Anno, PC_Dettagli_AreaOmogenea, PC_Dettagli_Campioneterreno, PC_Dettagli_Prova, PC_Dettagli_Datadal, PC_Dettagli_DataAl, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Quadrante_Des, PC_Dettagli_Quadrante_Classe, PC_Dettagli_Quadrante_Cella, PC_Dettagli_ColturaPrincipale_Veg_Cod,  " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_RegimeIrriguo, PC_Dettagli_Anticipazioni, PC_Dettagli_Anticipazioni_Anni, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Ubicazione_Cod, PC_Dettagli_PercFissazioneN, PC_Dettagli_Copertura, " & vbCrLf)
            StrSQL.Append(" N_Ammesso, P_Ammesso, K_Ammesso, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Mg, PC_Dettagli_CSC, PC_Dettagli_Analisi_Testata_Cod, PC_Dettagli_Piovosita_Febbraio, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_P, PC_Dettagli_Flag_P, PC_Dettagli_K, PC_Dettagli_Flag_K, " & vbCrLf)
            StrSQL.Append(" Allegati_Documenti_Cod, " & vbCrLf)
            StrSQL.Append(" N_Mas, N_Mas_Regolamento_Tipo, " & vbCrLf)

            '21/10/21 Anna: Piano Nutrizionale 
            StrSQL.Append(" PC_Dettagli_Precessione_Veg_Cod_AnnoPrecedente, PC_Dettagli_Precessione_Veg_Cod_2anni_Prima, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Precessione_Veg_Cod_3anni_Prima, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Piovosita_Primavera, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_norg, PC_Dettagli_bio, " & vbCrLf)

            '08/23 Anna: Piano Nutrizionale IBF
            StrSQL.Append(" PC_Dettagli_Resa_Storica, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Flag_Residui_Precessione_Asportati, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Mese_Residui_Precessione_Interramento, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Mese_Semina, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Mese_Raccolta, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Temperatura_Media_ColturaInCampo, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Temperatura_Media_MeseSemina_Febbraio, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Perc_Umidita_Coltura_Principale, " & vbCrLf)
            StrSQL.Append(" PC_Dettagli_Perc_Umidita_Raccolta_Precessione, " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, " & vbCrLf)
            StrSQL.Append("              Data_Creazione,     Data_Modifica, " & vbCrLf)
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, " & vbCrLf)
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock " & vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (" & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "', " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Testata_Cod)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Cod)) & ", " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_PIVA)) & "', " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_SaCod)) & ", " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_Appezzamenti_Des)) & "', " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Finalita_GRFI_COD)) & ", " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_FaseCicloColturale_fase_des)) & "', " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_FaseCicloColturale_id_fase)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Ciclo_EP_COD)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_ListDurataAnni)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Resa)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Precessione_Veg_Cod)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_FinalitaPrecessione_Grfi_Cod)) & ", " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_Condizionidelterreno_Des)) & "', " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Condizionidelterreno_id_terreno)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Sabbia)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Limo)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Argilla)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Ph)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Caco3)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Caco3_Attivo)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_CN)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_So)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_ntot)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_p2o5)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_k2o)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Quantita)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Frequenza)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_fn)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_fp2o5)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_fk2o)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_fss)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Quantita1)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_f1n)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_f1p2o5)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_f1k2o)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_f1ss)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Fertilizzazione_id_tp_fer)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Fertilizzazione1_id_tp_fer)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Piovosita)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_AreaVulnerabile)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Anno)) & ", " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_AreaOmogenea)) & "', " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_Campioneterreno)) & "', " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_Prova)) & "', " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveDate(Trim(PC_Dettagli_Datadal)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveDate(Trim(PC_Dettagli_DataAl)) & ", " & vbCrLf)
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PC_Dettagli_Quadrante_Des)) & "', " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Quadrante_Classe)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Quadrante_Cella)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_ColturaPrincipale_Veg_Cod)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_RegimeIrriguo)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Anticipazioni)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Anticipazioni_Anni)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Ubicazione_Cod)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_PercFissazioneN)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Copertura)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(N_Ammesso)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(P_Ammesso)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(K_Ammesso)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Mg)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_CSC)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Analisi_Testata_Cod)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Piovosita_Febbraio)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_P)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Flag_P)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_K)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Flag_K)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(Allegati_Documenti_Cod)) & ", " & vbCrLf)

            StrSQL.Append("         " & If(IsNothing(N_Mas), "Null", Agro_SQL_SaveNum(Trim(N_Mas))) & ", " & vbCrLf)
            StrSQL.Append("         " & If(IsNothing(N_Mas_Regolamento_Tipo), "Null", Agro_SQL_SaveNum(Trim(N_Mas_Regolamento_Tipo))) & ", " & vbCrLf)
            'StrSQL.Append("         " + Agro_SQL_SaveNum(Trim(N_Mas_Regolamento_Tipo)) + " " & vbCrLf)

            '21/10/21 Anna: Piano Nutrizionale 
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Precessione_Veg_Cod_AnnoPrecedente)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Precessione_Veg_Cod_2anni_prima)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Precessione_Veg_Cod_3anni_prima)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Piovosita_Primavera)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_norg)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_bio)) & ", " & vbCrLf)

            '08/23 Anna: Piano Nutrizionale IBF
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Resa_Storica)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Flag_Residui_Precessione_Asportati)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Mese_Residui_Precessione_Interramento)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Mese_Semina)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Mese_Raccolta)) & ", " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Temperatura_Media_ColturaInCampo)) & ",  " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Temperatura_Media_MeseSemina_Febbraio)) & ",  " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Perc_Umidita_Coltura_Principale)) & ",  " & vbCrLf)
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Dettagli_Perc_Umidita_Raccolta_Precessione)) & "  " & vbCrLf)

            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & vbCrLf)
            StrSQL.Append("         , 0  ")
            StrSQL.Append(") ")

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

    '##############################################################################################
    Public Function UpdateCodAllegato(ByVal PC_Testata_Cod As Integer,
                            ByVal PC_Dettagli_Cod As Integer,
                            ByVal PC_Dettagli_PIVA As String,
                            ByVal Allegati_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali : 
        ' PC_Dettagli_Cod = 0 per non considerarlo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE PianoConcimazione_Dettagli " & vbCrLf)

            StrSQL.Append(" SET PianoConcimazione_Dettagli.Allegati_Documenti_Cod =  " & Agro_SQL_SaveNum(Allegati_Cod) & " " & vbCrLf)


            StrSQL.Append(" WHERE   PC_Dettagli_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     PC_Dettagli_Piva = '" & Agro_SQL_SaveText(PC_Dettagli_PIVA) & "' ")
            StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")



            If PC_Dettagli_Cod <> 0 Then
                StrSQL.Append(" AND     PC_Dettagli_Cod = " & Agro_SQL_SaveNum(PC_Dettagli_Cod) & " ")
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


End Class



