Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class Budget_Campi_codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    Public Function Leggi(ByVal Id_Budget As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal Id_Cod As Int32,
                            ByVal Val_Cod As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                               ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Campi_Codici_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT *,  Budget_Campi_Codici.Validita_Inizio as xValidita_Inizio, Budget_Campi_Codici.Validita_Fine as xValidita_Fine " &
                                    " FROM  Budget_Campi_Codici, Codici_Anagrafe " &
                                    " WHERE Budget_Campi_Codici.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Budget_Campi_Codici.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                    " AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                    " AND   Budget_Campi_Codici.Id_Cod = Codici_Anagrafe.Codice " &
                                    " AND   Budget_Campi_Codici.Id_Budget =  " & Id_Budget & " ")

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   Budget_Campi_Codici.Sa_Cod =  " & Sa_Cod & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND   Budget_Campi_Codici.Campo_Cod =  " & Campo_Cod & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND   Budget_Campi_Codici.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Campi_Codici.Id_Cod = " & Id_Cod & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


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
    Public Function Leggi2(ByVal Id_Budget As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal Id_Cod As Int32,
                            ByVal Val_Cod As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                               ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Campi_Codici_R.Leggi2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT *,  Budget_Campi_Codici.Validita_Inizio as xValidita_Inizio, Budget_Campi_Codici.Validita_Fine as xValidita_Fine " &
                            " FROM  Budget_Campi_Codici, Codici_Anagrafe " &
                            " WHERE Budget_Campi_Codici.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                            " AND   Budget_Campi_Codici.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                            " AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                            " AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                            " AND   Budget_Campi_Codici.Id_Cod = Codici_Anagrafe.Codice ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND   Budget_Campi_Codici.Id_Budget =  " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND   Budget_Campi_Codici.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   Budget_Campi_Codici.Sa_Cod =  " & Sa_Cod & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND   Budget_Campi_Codici.Campo_Cod =  " & Campo_Cod & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND  Budget_Campi_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Budget_Campi_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


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
End Class

Public Class Budget_Campi_codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal idBudget As Integer,
                                  ByVal PIVA As String,
                                  ByVal Sa_Cod As Long,
                                  ByVal Campo_Cod As Long,
                                  ByVal id_cod As Long,
                                  ByVal Val_Cod As String,
                                  ByVal Validita_Inizio As Date,
                                  ByVal Validita_Fine As Date,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Campi_codici_W.Scrivi_Budget()"

        '====================================================================================
        'Parametri opzionali :
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

            StrSQL.Append("INSERT INTO Budget_Campi_Codici( ")
            StrSQL.Append("                    Id_Budget,   ")
            StrSQL.Append("                    Piva,        ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Campo_Cod,   ")
            StrSQL.Append("                    Id_Cod,      ")
            StrSQL.Append("                    Val_Cod,     ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(idBudget) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(id_cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Now.Date) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Now.Date) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & " )")

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


    Public Function Cancella(ByVal Id_Budget As Integer,
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Long,
                            ByVal Campo_Cod As Long,
                            ByVal id_cod As Long,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Campi_Codici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0


                StrSQL.Append(" UPDATE  Budget_Campi_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'")
                StrSQL.Append("   AND    Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM  Budget_Campi_Codici ")
                StrSQL.Append(" WHERE Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "")
                StrSQL.Append("   AND    Inviato = 0 ")


            End If
            '---------------------------------------------

            If PIVA <> "" Then
                StrSQL.Append(" AND   Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND  Campo_Cod = " & Campo_Cod & " ")
            End If

            If id_cod <> 0 Then
                StrSQL.Append(" AND   Id_Cod = " & id_cod & " ")
            End If

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
