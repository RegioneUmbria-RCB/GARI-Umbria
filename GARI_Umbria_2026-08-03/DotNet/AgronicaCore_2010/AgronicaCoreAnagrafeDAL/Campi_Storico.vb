Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Campi_Storico_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Campo_Cod As Int32, _
                                ByVal Appezza As Int32, _
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_Storico_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Append(" SELECT * FROM Campi_Storico ")
                    StrSQL.Append(" WHERE 1=1")
                    If Piva <> "" Then
                        StrSQL.Append(" AND Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


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



    '##############################################################################################
    Public Function Esiste_Record_Campi_Storico( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Campo_Cod As Int32, _
                                ByVal Appezza As Int32, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_Storico_R.Esiste_Record_Campi_Storico()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim objCamStor As New AgronicaCoreAnagrafeDAL.Campi_Storico_R
        DT = objCamStor.Leggi(Piva, Sa_Cod, Campo_Cod, Appezza, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
        If DT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function


End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Campi_Storico_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi( _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Int32, _
                        ByVal Campo_Cod As Int32, _
                        ByVal Appezza As Int32, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            , Optional ByVal Data_creazione As Date = #2/1/1900# _
            , Optional ByVal Data_modifica As Date = #2/1/1900# _
            , Optional ByVal username_creazione As String = "" _
            , Optional ByVal username_modifica As String = "" _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_Storico_R.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

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


        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Campi_Storico(Piva, Sa_Cod, Campo_Cod, Appezza, Data_Creazione, Data_Modifica, ")
            StrSQL.Append("                          Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")

            StrSQL.Append("   VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza))
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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


    Public Function Modifica( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Long, _
                            ByVal Campo_Cod As Int32, _
                            ByVal Appezza As Int32, _
                                ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campi_Storico_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""         
        '   Sa_Cod = 0        
        '   Campo_Cod = 0       
        '   Appezza = 0       
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Campi_Storico ")
            StrSQL.Append(" SET ")
            StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDate(Now.Today) & ", ")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
            StrSQL.Append(" Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" WHERE 1=1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If


            '----------------------------------------------------------------------
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
