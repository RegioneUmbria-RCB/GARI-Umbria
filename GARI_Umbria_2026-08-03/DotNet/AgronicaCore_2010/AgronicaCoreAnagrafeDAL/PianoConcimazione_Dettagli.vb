Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

' ---------------------------------------------- 
'  
'  Galassi, 24/02/2017 11.43.52: IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
' ---------------------------------------------- 


''' <summary>
''' IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
''' </summary>
''' <remarks></remarks>
Public Class PianoConcimazione_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal PC_Testata_Cod As Integer, _
                            ByVal PC_Dettagli_Cod As Integer, _
                            ByVal PC_Dettagli_PIVA As String, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & vbCrLf)
                    StrSQL.Append(" FROM    PianoConcimazione_Dettagli " & vbCrLf)
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND     PC_Dettagli_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " " & vbCrLf)

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
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


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




End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


' ---------------------------------------------- 
'  
'  Galassi, 24/02/2017 11.43.52: IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
' ---------------------------------------------- 


''' <summary>
''' IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
''' </summary>
''' <remarks></remarks>
Public Class PianoConcimazione_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Cancella(ByVal PC_Testata_Cod As Integer,
                             ByVal PC_Dettagli_Cod As Integer,
                             ByVal PC_Dettagli_Piva As String,
                               ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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
    Public Function Scrivi(ByVal PC_Testata_Cod As Integer, _
                            ByVal PC_Dettagli_Cod As Integer, _
                            ByVal PC_Dettagli_PIVA As String, _
                            ByVal PC_Dettagli_Appezzamenti_Des As String, _
                            ByVal PC_Dettagli_Finalita_GRFI_COD As Integer, _
                            ByVal PC_Dettagli_FaseCicloColturale_fase_des As String, _
                            ByVal PC_Dettagli_FaseCicloColturale_id_fase As Integer, _
                            ByVal PC_Dettagli_Ciclo_EP_COD As Integer, _
                            ByVal PC_Dettagli_ListDurataAnni As Integer, _
                            ByVal PC_Dettagli_Resa As Decimal, _
                            ByVal PC_Dettagli_Precessione_Veg_Cod As Integer, _
                            ByVal PC_Dettagli_FinalitaPrecessione_Grfi_Cod As Integer, _
                            ByVal PC_Dettagli_Condizionidelterreno_Des As String, _
                            ByVal PC_Dettagli_Condizionidelterreno_id_terreno As Integer, _
                            ByVal PC_Dettagli_Sabbia As Decimal, _
                            ByVal PC_Dettagli_Limo As Decimal, _
                            ByVal PC_Dettagli_Argilla As Decimal, _
                            ByVal PC_Dettagli_Ph As Decimal, _
                            ByVal PC_Dettagli_Caco3 As Decimal, _
                            ByVal PC_Dettagli_Caco3_Attivo As Decimal, _
                            ByVal PC_Dettagli_CN As Decimal, _
                            ByVal PC_Dettagli_So As Decimal, _
                            ByVal PC_Dettagli_ntot As Decimal, _
                            ByVal PC_Dettagli_p2o5 As Decimal, _
                            ByVal PC_Dettagli_k2o As Decimal, _
                            ByVal PC_Dettagli_Quantita As Decimal, _
                            ByVal PC_Dettagli_Frequenza As Integer, _
                            ByVal PC_Dettagli_fn As Decimal, _
                            ByVal PC_Dettagli_fp2o5 As Decimal, _
                            ByVal PC_Dettagli_fk2o As Decimal, _
                            ByVal PC_Dettagli_fss As Decimal, _
                            ByVal PC_Dettagli_Quantita1 As Decimal, _
                            ByVal PC_Dettagli_f1n As Decimal, _
                            ByVal PC_Dettagli_f1p2o5 As Decimal, _
                            ByVal PC_Dettagli_f1k2o As Decimal, _
                            ByVal PC_Dettagli_f1ss As Decimal, _
                            ByVal PC_Dettagli_Fertilizzazione_id_tp_fer As Integer, _
                            ByVal PC_Dettagli_Fertilizzazione1_id_tp_fer As Integer, _
                            ByVal PC_Dettagli_Piovosita As Decimal, _
                            ByVal PC_Dettagli_AreaVulnerabile As Integer, _
                            ByVal PC_Dettagli_Anno As Integer, _
                            ByVal PC_Dettagli_AreaOmogenea As String, _
                            ByVal PC_Dettagli_Campioneterreno As String, _
                            ByVal PC_Dettagli_Prova As String, _
                            ByVal PC_Dettagli_Datadal As Date, _
                            ByVal PC_Dettagli_DataAl As Date, _
                            ByVal PC_Dettagli_Quadrante_Des As String, _
                            ByVal PC_Dettagli_Quadrante_Classe As Integer, _
                            ByVal PC_Dettagli_Quadrante_Cella As Integer, _
                            ByVal PC_Dettagli_ColturaPrincipale_Veg_Cod As Integer, _
                            ByVal PC_Dettagli_RegimeIrriguo As Integer, _
                            ByVal PC_Dettagli_Anticipazioni As Integer, _
                            ByVal PC_Dettagli_Anticipazioni_Anni As Integer, _
                            ByVal PC_Ubicazione_Cod As Integer, _
                            ByVal PC_PercFissazioneN As Decimal, _
                            ByVal PC_Copertura As Integer, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO PianoConcimazione_Dettagli " & vbCrLf)

            StrSQL.Append(" (PC_Dettagli_SuperUser, PC_Testata_Cod, PC_Dettagli_Cod, PC_Dettagli_PIVA, PC_Dettagli_Appezzamenti_Des, PC_Dettagli_Finalita_GRFI_COD, " & vbCrLf)
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
            StrSQL.Append("         " & Agro_SQL_SaveNum(Trim(PC_Copertura)) & " " & vbCrLf)
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


End Class



