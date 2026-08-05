Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CAC_Codifica_InfoAggiuntive_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    Public Function Esiste_Codifica_CAA_Agrea(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                             ) As Boolean

        Dim Flag_Esiste As Boolean = False
        Dim Dt_InfoAgg As DataTable

        Dt_InfoAgg = Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CAA_Agrea, _
                                 "", _
                                 0, _
                                1, _
                                 enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                 "", _
                                 "", _
                                 objParametri)

        If Not IsNothing(Dt_InfoAgg) AndAlso Dt_InfoAgg.Rows.Count > 0 Then
            Flag_Esiste = True
        End If

        Return Flag_Esiste

    End Function

    '###############################################################################
    Public Function Esiste_Codifica_CAA_Anagrafe(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                             ) As Boolean

        Dim Flag_Esiste As Boolean = False
        Dim Dt_InfoAgg As DataTable

        Dt_InfoAgg = Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CAA_Anagrafe, _
                                 "", _
                                 0, _
                                1, _
                                 enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                 "", _
                                 "", _
                                 objParametri)

        If Not IsNothing(Dt_InfoAgg) AndAlso Dt_InfoAgg.Rows.Count > 0 Then
            Flag_Esiste = True
        End If

        Return Flag_Esiste

    End Function


    '##############################################################################################
    'argomento_cod è enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod
    Public Function Leggi( ByVal Argomento_Cod As Int32, _
                            ByVal InfoAgg_Cod As String, _
                            ByVal Tipo_Codifica As Integer, _
                            ByVal OrderBy_Cod1_Des2 As Int32, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    CAC_Codifica_InfoAggiuntive ")

                    StrSQL.Append(" WHERE CAC_Codifica_InfoAggiuntive.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   CAC_Codifica_InfoAggiuntive.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If Argomento_Cod <> 0 Then
                        StrSQL.Append(" AND Argomento_Cod = " & Agro_SQL_SaveNum(Argomento_Cod) & "   ")
                    End If

                    If InfoAgg_Cod <> "" Then
                        StrSQL.Append(" AND InfoAgg_Cod = '" & Agro_SQL_SaveText(InfoAgg_Cod) & "'   ")
                    End If

                    If Tipo_Codifica <> 0 Then
                        StrSQL.Append(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case OrderBy_Cod1_Des2
                        Case 1 '--- COD
                            StrSQL.Append(" ORDER BY InfoAgg_Cod ASC ")

                        Case 2 '--- DES
                            StrSQL.Append(" ORDER BY InfoAgg_Des ASC ")

                        Case Else
                            If xOrderBy <> "" Then
                                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                            Else
                                StrSQL.Append(" ORDER BY InfoAgg_Des ASC ")
                            End If
                    End Select

            End Select
            '------------------------------------------------------------------


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






    '###############################################################################
    'argomento_cod è enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod
    Public Function InfoAgg_Des_from_InfoAgg_Cod(ByVal InfoAgg_Cod As String, _
                                                    ByVal Argomento_Cod As Integer, _
                                                    ByVal Tipo_Codifica As Integer, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String


        Dim InfoAgg_Des As String = ""
        Dim Dt_InfoAgg As DataTable


        Dt_InfoAgg = Leggi(Argomento_Cod, _
                            InfoAgg_Cod, _
                            Tipo_Codifica, _
                            1, _
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                "", _
                                "", _
                                objParametri)

        If Not IsNothing(Dt_InfoAgg) AndAlso Dt_InfoAgg.Rows.Count > 0 Then
            InfoAgg_Des = CStr(Dt_InfoAgg.Rows(0).Item("InfoAgg_Des"))
        End If

        Dt_InfoAgg = Nothing

        Return InfoAgg_Des

    End Function

    '###############################################################################
    'argomento_cod è enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod
    Public Function Esiste_InfoAgg_Cod(ByVal InfoAgg_Cod As String, _
                                        ByVal Argomento_Cod As Integer, _
                                        ByVal Tipo_Codifica As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim Flag_Esiste As Boolean = False
        Dim Dt_InfoAgg As DataTable

        Dt_InfoAgg = Leggi(Argomento_Cod, _
                                InfoAgg_Cod, _
                                Tipo_Codifica, _
                                1, _
                                 enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                 "", _
                                 "", _
                                 objParametri)

        If Not IsNothing(Dt_InfoAgg) AndAlso Dt_InfoAgg.Rows.Count > 0 Then
            Flag_Esiste = True
        End If

        Return Flag_Esiste

    End Function








End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class CAC_Codifica_InfoAggiuntive_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '######################################################################
    'argomento_cod è enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod
    Public Function Scrivi( _
                           ByVal InfoAgg_Cod As String, _
                           ByVal InfoAgg_Des As String, _
                           ByVal Argomento_Cod As Integer, _
                           ByVal Argomento_Des As String, _
                            ByVal Tipo_Codifica As Integer, _
                            ByVal CodiceAux_1 As Integer, _
                            ByVal CodiceAux_2 As Integer, _
                            ByVal CodiceAux_3 As Integer, _
                            ByVal TestoAux_1 As String, _
                            ByVal TestoAux_2 As String, _
                            ByVal TestoAux_3 As String, _
                               ByVal Validita_Inizio As Date, _
                               ByVal Validita_Fine As Date, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = "" _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CAC_Codifica_InfoAggiuntive ")
            StrSQL.Append("             (Piva_SuperUser,    InfoAgg_Cod, ")
            StrSQL.Append("              InfoAgg_Des, Argomento_Cod, Argomento_Des, Tipo_Codifica, ")
            StrSQL.Append("               CodiceAux_1, CodiceAux_2, CodiceAux_3,  ")
            StrSQL.Append("               TestoAux_1, TestoAux_2, TestoAux_3,  ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(InfoAgg_Cod) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(InfoAgg_Des) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Argomento_Cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Argomento_Des) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Codifica) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(CodiceAux_1) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(CodiceAux_2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(CodiceAux_3) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(TestoAux_1) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(TestoAux_2) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(TestoAux_3) & "'  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

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

    Public Function Cancella( _
                                ByVal InfoAgg_Cod As String, _
                                ByVal Argomento_Cod As Integer, _
                                ByVal Tipo_Codifica As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If InfoAgg_Cod = "" Then
                Throw New Exception("Parametro non corretto nella query (InfoAgg_Cod obbligatorio)")
            End If

            If Argomento_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Argomento_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM    CAC_Codifica_InfoAggiuntive ")
            StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     InfoAgg_Cod = '" & Agro_SQL_SaveText(InfoAgg_Cod) & "' ")
            StrSQL.Append(" AND     Argomento_Cod = " & Agro_SQL_SaveNum(Argomento_Cod) & " ")
            StrSQL.Append(" AND     Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
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




End Class