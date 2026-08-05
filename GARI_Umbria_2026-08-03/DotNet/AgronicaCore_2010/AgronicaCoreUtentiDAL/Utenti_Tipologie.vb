Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class Utenti_Tipologie_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Tipologia_Cod As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Tipologie_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Tipologia_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Tipologia_Cod, Tipologia_Des, Note ")
                    StrSQL.Append(" FROM    Utenti_Tipologie ")
                    StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

                    If Tipologia_Cod <> 0 Then
                        StrSQL.Append(" AND (Tipologia_Cod = " & Agro_SQL_SaveNum(Tipologia_Cod) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Tipologia_Des ")
                    End If


                Case Else 'enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM    Utenti_Tipologie ")
                    StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

                    If Tipologia_Cod <> 0 Then
                        StrSQL.Append(" AND (Tipologia_Cod = " & Agro_SQL_SaveNum(Tipologia_Cod) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Tipologia_Des ")
                    End If

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

    Public Function LeggiImpostazioniCollegate(ByVal tipologiaCod As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Tipologie_R.LeggiImpostazioniCollegate()"

        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Utenti_Impostazioni ")
            StrSQL.AppendLine(" WHERE UserName like " & Agro_SQL_SaveNum(tipologiaCod))

            If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" AND " & Agro_SQL_SaveText(xFiltroAggiuntivo))
            End If
            If Not String.IsNullOrWhiteSpace(xOrderBy) Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_SaveText(xOrderBy))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT
    End Function

End Class

Public Class Utenti_Tipologie_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Tipologia_Cod As Int32,
                           ByVal Tipologia_Des As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional noteTipologia As String = ""
    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Tipologie_W.Scrivi()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        Try
            '---------------------------------------------
            StrSQL.AppendLine("INSERT INTO Utenti_Tipologie       ")
            StrSQL.AppendLine("            ( Piva_SuperUser, Tipologia_Cod, Tipologia_Des,  ")
            StrSQL.AppendLine("                    Inviato,             DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,     Validita_Fine, ")
            StrSQL.AppendLine("                    Note ")
            StrSQL.AppendLine("                    ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipologia_Cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Tipologia_Des) & "' ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(noteTipologia) & "' ")
            StrSQL.AppendLine(")")
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp
    End Function

    '##############################################################################################
    Public Function Modifica(ByVal Tipologia_Cod As Int32,
                             ByVal Tipologia_Des As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional noteTipologia As String = ""
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Tipologie_W.Modifica()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        Try
            StrSQL.Append("UPDATE Utenti_Tipologie SET ")
            StrSQL.Append("       Tipologia_Des = '" & Agro_SQL_SaveText(Tipologia_Des) & "'")
            StrSQL.Append("       ,Note = '" & Agro_SQL_SaveText(noteTipologia) & "'")
            StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE Piva_SuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Tipologia_Cod = " & Agro_SQL_SaveNum(Tipologia_Cod) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp
    End Function


    '##############################################################################################
    Public Function Cancella(ByVal Tipologia_Cod As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Tipologie_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Utenti_Tipologie ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

                If Tipologia_Cod <> 0 Then
                    StrSQL.Append(" AND  Tipologia_Cod =  " & Agro_SQL_SaveNum(Tipologia_Cod) & " ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Utenti_Tipologie ")
                StrSQL.Append(" WHERE    Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                If Tipologia_Cod <> 0 Then
                    StrSQL.Append(" AND  Tipologia_Cod =  " & Agro_SQL_SaveNum(Tipologia_Cod) & " ")
                End If

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


End Class