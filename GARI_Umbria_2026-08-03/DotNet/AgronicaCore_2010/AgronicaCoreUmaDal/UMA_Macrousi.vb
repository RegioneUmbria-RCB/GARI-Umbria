Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
'Imports AgronicaCoreUmaDal.UMASetup_W

Public Class UMA_Macrousi_R
    Inherits DataProvider

    ''' <summary>
    ''' Restituisce le UF prodotte della coltura interessata
    ''' </summary>
    ''' <param name="macrouso_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>Una dataTable con 3 colonne: UF, UFL, UFC</returns>
    Public Function Leggi(ByVal macrouso_Cod As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Macrousi_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT [Regione_Cod], ")
            strSql.AppendLine("[Macrouso_UMA_Cod], ")
            strSql.AppendLine("[Macrouso_UMA_Des], ")
            strSql.AppendLine("[inviato], ")
            strSql.AppendLine("[datainvio], ")
            strSql.AppendLine("[Data_Creazione], ")
            strSql.AppendLine("[Data_Modifica], ")
            strSql.AppendLine("[Username_Creazione], ")
            strSql.AppendLine("[Username_Modifica], ")
            strSql.AppendLine("[Validita_Inizio], ")
            strSql.AppendLine("[Validita_Fine] ")
            strSql.AppendLine("FROM UMA_Macrousi")
            strSql.AppendLine("WHERE 1=1 ")

            If macrouso_Cod <> "" Then
                strSql.AppendLine("AND UMA_Macrousi.Macrouso_UMA_Cod = '" & Agro_SQL_SaveText(macrouso_Cod) & "' ")
            End If
            'If destinazione_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Macrousi.Destinazione_Cod = '" & destinazione_Cod + "' ")
            'End If
            'If uso_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Macrousi.Uso_Cod = '" & uso_Cod + "' ")
            'End If
            'If qualita_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Macrousi.Qualita_Cod = '" & qualita_Cod + "' ")
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   UMA_Macrousi.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   UMA_Macrousi.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function Elenco(ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Macrousi_R.Elenco()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT ")
            strSql.AppendLine("[Macrouso_UMA_Cod], ")
            strSql.AppendLine("[Macrouso_UMA_Des] ")
            strSql.AppendLine("FROM UMA_Macrousi")
            strSql.AppendLine("WHERE Validita_Inizio < GETDATE() ")
            strSql.AppendLine("AND Validita_Fine > GETDATE() ")
            strSql.Append("GROUP BY [Macrouso_UMA_Cod], [Macrouso_UMA_Des] ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function Leggi_AssociazioniMacrousi(ByVal Cul_Cod_Agea As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Macrousi_R.Leggi_AssociazioniMacrousi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT [Cul_Cod_Agea], ")
            strSql.AppendLine("[Cul_Des_Agea], ")
            strSql.AppendLine("[Uso_Cod], ")
            strSql.AppendLine("[Uso_Des], ")
            strSql.AppendLine("[Macrouso_Cod], ")
            strSql.AppendLine("[Macrouso_Des], ")
            strSql.AppendLine("[Occupazione_Cod], ")
            strSql.AppendLine("[Occupazione_Des], ")
            strSql.AppendLine("[Destinazione_Cod], ")
            strSql.AppendLine("[Destinazione_Des], ")
            strSql.AppendLine("[Qualita_Cod], ")
            strSql.AppendLine("[Qualita_Des], ")
            strSql.AppendLine("[Macrouso_UMA_Cod], ")
            strSql.AppendLine("[Macrouso_UMA_Des], ")
            strSql.AppendLine("[inviato], ")
            strSql.AppendLine("[datainvio], ")
            strSql.AppendLine("[Data_Creazione], ")
            strSql.AppendLine("[Data_Modifica], ")
            strSql.AppendLine("[Username_Creazione], ")
            strSql.AppendLine("[Username_Modifica], ")
            strSql.AppendLine("[Validita_Inizio], ")
            strSql.AppendLine("[Validita_Fine] ")
            strSql.AppendLine("FROM Codifica_SpecieVegetali_Agea_2015_2020")
            strSql.AppendLine("WHERE 1=1 ")

            If Cul_Cod_Agea <> "" Then
                strSql.AppendLine("AND Codifica_SpecieVegetali_Agea_2015_2020.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            End If
            'If destinazione_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Macrousi.Destinazione_Cod = '" & destinazione_Cod + "' ")
            'End If
            'If uso_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Macrousi.Uso_Cod = '" & uso_Cod + "' ")
            'End If
            'If qualita_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Macrousi.Qualita_Cod = '" & qualita_Cod + "' ")
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   Codifica_SpecieVegetali_Agea_2015_2020.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   Codifica_SpecieVegetali_Agea_2015_2020.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Sub ComponiFiltroAggiuntivoValidita_AssociazioniMacrousi(ByRef filtroAggiuntivo As String, inizioValidita As String, fineValidita As String)
        ComponiFiltroAggiuntivoValidita("Codifica_SpecieVegetali_Agea_2015_2020", filtroAggiuntivo, inizioValidita, fineValidita)
    End Sub

    Public Sub ComponiFiltroAggiuntivoValidita(ByRef filtroAggiuntivo As String, inizioValidita As String, fineValidita As String)
        ComponiFiltroAggiuntivoValidita("UMA_Macrousi", filtroAggiuntivo, inizioValidita, fineValidita)
    End Sub

    Private Sub ComponiFiltroAggiuntivoValidita(ByVal tabella As String, ByRef filtroAggiuntivo As String, inizioValidita As String, fineValidita As String)
        filtroAggiuntivo = $"{filtroAggiuntivo} {vbCrLf}"
        filtroAggiuntivo = $"{filtroAggiuntivo} ( ({tabella}.Validita_Inizio >= '{inizioValidita}' AND {tabella}.Validita_Fine <= '{fineValidita}') OR {vbCrLf}"
        filtroAggiuntivo = $"{filtroAggiuntivo} ({tabella}.Validita_Fine >= '{inizioValidita}' AND {tabella}.Validita_Fine <= '{fineValidita}') OR {vbCrLf}"
        filtroAggiuntivo = $"{filtroAggiuntivo} ({tabella}.Validita_Inizio >= '{inizioValidita}' AND {tabella}.Validita_Inizio <= '{fineValidita}')OR {vbCrLf}"
        filtroAggiuntivo = $"{filtroAggiuntivo} ({tabella}.Validita_Inizio <= '{inizioValidita}' AND {tabella}.Validita_Fine >= '{fineValidita}') ) "
    End Sub



End Class

Public Class UMA_Macrousi_W
    Inherits DataProvider

    Public Function AggiungiNuovi(listLav As List(Of UMA_Macrousi_Dto),
                                  ByRef objParametri As AgronicaCoreParametri,
                                  ByRef strErr As String) As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Macrousi_W.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            For Each l In listLav

                If CheckDuplicati(l, objParametri, False) Then

                    l.Username_Modifica = objParametri.UtenteUsername
                    l.Data_Modifica = Date.Now
                    l.Username_Creazione = objParametri.UtenteUsername
                    l.Data_Creazione = Date.Now

                    StrSQL.AppendLine("INSERT INTO [UMA_Macrousi] ")
                    StrSQL.AppendLine("(")
                    StrSQL.AppendLine("[Regione_Cod], ")
                    StrSQL.AppendLine("[Macrouso_UMA_Cod], ")
                    StrSQL.AppendLine("[Macrouso_UMA_Des], ")
                    StrSQL.AppendLine("[inviato], ")
                    StrSQL.AppendLine("[datainvio], ")
                    StrSQL.AppendLine("[Data_Creazione], ")
                    StrSQL.AppendLine("[Data_Modifica], ")
                    StrSQL.AppendLine("[Username_Creazione], ")
                    StrSQL.AppendLine("[Username_Modifica], ")
                    StrSQL.AppendLine("[Validita_Inizio], ")
                    StrSQL.AppendLine("[Validita_Fine] ")
                    StrSQL.AppendLine(") ")
                    StrSQL.AppendLine("VALUES")
                    StrSQL.AppendLine("( ")
                    StrSQL.AppendLine($"'010', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.Macrouso_UMA_Cod)}', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.Macrouso_UMA_Des)}', ")
                    StrSQL.AppendLine($"{Agro_SQL_SaveNum(l.Inviato)}, ")

                    If IsNothing(l.DataInvio) Then
                        StrSQL.AppendLine("NULL, ")
                    Else
                        StrSQL.AppendLine($"{Agro_SQL_SaveDate(l.DataInvio)}, ")
                    End If

                    StrSQL.AppendLine($"{Agro_SQL_SaveDate(l.Data_Creazione)}, ")
                    StrSQL.AppendLine($"{Agro_SQL_SaveDate(l.Data_Modifica)}, ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.Username_Creazione)}', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.Username_Modifica)}', ")
                    StrSQL.AppendLine($"{Agro_SQL_SaveDate(l.Validita_Inizio)}, ")
                    StrSQL.AppendLine($"{Agro_SQL_SaveDate(l.Validita_Fine)}")
                    StrSQL.AppendLine($")")

                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                Else
                    strErr = String.Concat(strErr, vbCrLf & " Riga non inserita: Codice: " & l.Macrouso_UMA_Cod & ", Descrizione: " & l.Macrouso_UMA_Des &
                                           ", Validita Inizio: " & CStr(l.Validita_Inizio) & ". Rilevata sovrapposizione di periodi di validita.")
                End If
            Next
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return strErr
    End Function

    Public Function Rimuovi(lista As List(Of UMA_Macrousi_Dto), objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Macrousi_W.Rimuovi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            For Each l In lista

                StrSQL.AppendLine("DELETE FROM [UMA_Macrousi] ")
                StrSQL.AppendLine($"WHERE [Regione_Cod] = '{Agro_SQL_SaveText(l.Regione_Cod)}' ")
                StrSQL.AppendLine($"AND [Macrouso_UMA_Cod] = '{Agro_SQL_SaveText(l.Macrouso_UMA_Cod)}' ")
                StrSQL.AppendLine($"AND [Validita_Inizio] = {Agro_SQL_SaveDate(l.Validita_Inizio)} ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of UMA_Macrousi_Dto),
                             ByRef objParametri As AgronicaCoreParametri,
                             ByRef StrErr As String) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Macrousi_W.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As StringBuilder = New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Try

            For Each l In righeModificateArr

                If CheckDuplicati(l, objParametri, True) Then

                    l.Username_Modifica = objParametri.UtenteUsername
                    l.Data_Modifica = Date.Now

                    StrSQL.AppendLine("UPDATE [UMA_Macrousi] ")
                    StrSQL.AppendLine($"SET [Macrouso_UMA_Des] = '{Agro_SQL_SaveText(l.Macrouso_UMA_Des)}', ")
                    StrSQL.AppendLine($"[inviato] = {Agro_SQL_SaveNum(l.Inviato)}, ")
                    StrSQL.Append($"[datainvio] = ")
                    If IsNothing(l.DataInvio) Then
                        StrSQL.AppendLine("NULL, ")
                    Else
                        StrSQL.AppendLine(" " & Agro_SQL_SaveDate(l.DataInvio) & ", ")
                    End If
                    StrSQL.AppendLine($"[Data_Modifica] = {Agro_SQL_SaveDate(l.Data_Modifica)}, ")
                    StrSQL.AppendLine($"[Username_Modifica]= '{Agro_SQL_SaveText(l.Username_Modifica)}', ")
                    StrSQL.AppendLine($"[Validita_Fine] =  {Agro_SQL_SaveDate(l.Validita_Fine)}")

                    StrSQL.AppendLine($"WHERE [Regione_Cod] = '{Agro_SQL_SaveText(l.Regione_Cod)}' ")
                    StrSQL.AppendLine($"AND [Macrouso_UMA_Cod] = '{Agro_SQL_SaveText(l.Macrouso_UMA_Cod)}' ")
                    StrSQL.AppendLine($"AND [Validita_Inizio] = {Agro_SQL_SaveDate(l.Validita_Inizio)} ")

                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                Else
                    StrErr = String.Concat(StrErr, vbCrLf & " Riga non aggiornata: Codice: " & l.Macrouso_UMA_Cod & ", Descrizione: " & l.Macrouso_UMA_Des &
                                           ", Validita Inizio: " & CStr(l.Validita_Inizio) & ". Rilevata sovrapposizione di periodi di validita.")
                End If

            Next
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return StrErr
    End Function

    Private Function CheckDuplicati(riga As UMA_Macrousi_Dto, objParametri As AgronicaCoreParametri, aggiornamento As Boolean) As Boolean

        Dim leggi = New UMA_Macrousi_R
        Dim filtroAggiorna = ""

        If aggiornamento Then
            filtroAggiorna = " AND UMA_Macrousi.Validita_Inizio <> '" & riga.Validita_Inizio.ToShortDateString & "' "
        End If

        Dim dt = leggi.Leggi(riga.Macrouso_UMA_Cod,
                             " UMA_Macrousi.Validita_Fine >= '" & riga.Validita_Inizio.ToShortDateString &
                             "' AND UMA_Macrousi.Validita_Inizio <= '" & riga.Validita_Fine.ToShortDateString & "' " &
                             filtroAggiorna,
                             "",
                             objParametri)

        Return dt.Rows.Count = 0
    End Function

End Class

Public Class UMA_Macrousi_Dto
    ' *************************** Colonne chiavi primarie ***************************
    Public Regione_Cod As String
    Public Macrouso_UMA_Cod As String
    Public Validita_Inizio As DateTime

    ' ************************* Colonne della tabella UMA_Macrousi *************************
    Public Macrouso_UMA_Des As String
    Public Inviato As Short
    Public DataInvio As Date?

    Public Validita_Fine As Date

    Public Data_Creazione As Date?
    Public Data_Modifica As Date?
    Public Username_Creazione As String
    Public Username_Modifica As String

End Class