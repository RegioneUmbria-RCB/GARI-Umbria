Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Campi_Codici_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Campo_Cod As Int32,
                          ByVal Id_Cod As Int32,
                          ByVal Val_Cod As String,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Campi_Codici_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.Append(" SELECT *,  Campi_Codici.Validita_Inizio as xValidita_Inizio, Campi_Codici.Validita_Fine as xValidita_Fine " &
                                    " FROM  Campi_Codici, Codici_Anagrafe " &
                                    " WHERE Campi_Codici.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Campi_Codici.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                    " AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                    " AND   Campi_Codici.Id_Cod = Codici_Anagrafe.Codice " &
                                    " AND   Campi_Codici.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'" &
                                    " AND   Campi_Codici.Sa_Cod =  " & Sa_Cod & " " &
                                    " AND   Campi_Codici.Campo_Cod =  " & Campo_Cod & " ")

                    If Id_Cod <> 0 Then
                        strSql.Append(" AND Campi_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
    Public Function Leggi2(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Campo_Cod As Int32,
                           ByVal Id_Cod As Int32,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Campi_Codici_Read.Leggi2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.Append(" SELECT *,  Campi_Codici.Validita_Inizio as xValidita_Inizio, Campi_Codici.Validita_Fine as xValidita_Fine " & _
                            " FROM  Campi_Codici, Codici_Anagrafe " & _
                            " WHERE Campi_Codici.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                            " AND   Campi_Codici.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                            " AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                            " AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                            " AND   Campi_Codici.Id_Cod = Codici_Anagrafe.Codice ")

                    If Piva <> "" Then
                        strSql.Append(" AND   Campi_Codici.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.Append(" AND   Campi_Codici.Sa_Cod =  " & Sa_Cod & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        strSql.Append(" AND   Campi_Codici.Campo_Cod =  " & Campo_Cod & " ")
                    End If

                    If Id_Cod <> 0 Then
                        strSql.Append(" AND  Campi_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.Append(" AND Campi_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '#######################################
    Public Function Esiste_Campo(ByVal Piva As String,
                                 ByVal Id_Cod As Int32,
                                 ByVal Val_Cod As String,
                                 ByRef Sa_Cod As Int32,
                                 ByRef Campo_Cod As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Esiste_Campo()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            dt = Leggi2(Piva, 0, 0, Id_Cod, Val_Cod,
                        AGRODATAINIZIO, AGRODATAFINE,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "", "", objParametri)

            If dt.Rows.Count = 0 Then

                Sa_Cod = 0
                Campo_Cod = 0
                Return False

            ElseIf dt.Rows.Count = 1 Then

                Sa_Cod = dt.Rows(0).Item("sa_cod")
                Campo_Cod = dt.Rows(0).Item("Campo_Cod")
                Return True

            Else
                Throw New Exception("Sono presenti più centri con quel codice, verificare sul db i record su campi_codici ")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Campi_Codici_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal PIVA As String,
                           ByVal Sa_Cod As Long,
                           ByVal Campo_Cod As Long,
                           ByVal id_cod As Long,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Campi_Codici_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            strSql.Length = 0

            strSql.Append("INSERT INTO Campi_Codici( ")
            strSql.Append("                    Piva,        ")
            strSql.Append("                    Sa_Cod,      ")
            strSql.Append("                    Campo_Cod,   ")
            strSql.Append("                    Id_Cod,      ")
            strSql.Append("                    Val_Cod,     ")
            strSql.Append("                    Inviato, DataInvio, ")
            strSql.Append("                    Data_Creazione,     Data_Modifica, ")
            strSql.Append("                    UserName_Creazione, UserName_Modifica, ")
            strSql.Append("                    Validita_Inizio,    Validita_Fine ")
            strSql.Append("                    ) ")
            strSql.Append("VALUES (")
            strSql.Append("          '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(id_cod) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Now.Date) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Now.Date) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(validita_fine) & " )")

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

    '##############################################################################################
    Public Function Modifica(ByVal PIVA As String,
                             ByVal Sa_Cod As Long,
                             ByVal Campo_Cod As Long,
                             ByVal id_cod As Long,
                             ByVal Val_Cod As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Campi_Codici_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            strSql.Length = 0

            strSql.Append("UPDATE Campi_Codici SET ")
            strSql.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Now.Date))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(validita_inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(validita_fine))
            strSql.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'")
            strSql.Append(" AND   Sa_Cod    = " & Sa_Cod & " ")
            strSql.Append(" AND   Campo_Cod = " & Campo_Cod & " ")
            strSql.Append(" AND   Id_Cod    = " & id_cod & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    '##############################################################################################
    Public Function Cancella(ByVal PIVA As String,
                             ByVal Sa_Cod As Long,
                             ByVal Campo_Cod As Long,
                             ByVal id_cod As Long,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Campi_Codici_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0

                strSql.Append(" UPDATE  Campi_Codici ")
                strSql.Append(" SET ")
                strSql.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("         ,Inviato = -1 ")
                strSql.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'")
                strSql.Append("   AND    Inviato >= 0 ")

            Else

                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM  Campi_Codici ")
                strSql.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'")
                strSql.Append("   AND    Inviato = 0 ")

            End If
            '---------------------------------------------

            If Sa_Cod <> 0 Then
                strSql.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                strSql.Append(" AND  Campo_Cod = " & Campo_Cod & " ")
            End If

            If id_cod <> 0 Then
                strSql.Append(" AND   Id_Cod = " & id_cod & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

End Class
