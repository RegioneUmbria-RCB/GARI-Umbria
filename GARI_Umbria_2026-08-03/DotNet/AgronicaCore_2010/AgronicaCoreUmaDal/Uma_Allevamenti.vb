Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.IO


Public Class UMA_Allevamenti_R
    Inherits DataProvider


    Public Function Leggi(ByVal regioneCod As String, ByVal umaAllCod As String,
                         ByRef objParametri As AgronicaCoreParametri) As List(Of UMA_Allevamenti)


        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.UMA_Allevamenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim umaAllevamenti As List(Of UMA_Allevamenti)

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim query = (From um In GiasContext.UMA_Allevamenti)

                If regioneCod <> "" Then
                    query.Where(Function(el) el.Regione_Cod = regioneCod)
                End If

                If umaAllCod <> "" Then
                    query.Where(Function(el) el.UMA_All_Cod = umaAllCod)
                End If

                query.Select(Function(el) el)

                umaAllevamenti = query.ToList

            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            umaAllevamenti = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return umaAllevamenti

    End Function


    ''' <summary>
    ''' Restituisce le UF prodotte della coltura interessata
    ''' </summary>
    ''' <param name="allevamenti_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>Una dataTable con 3 colonne: UF, UFL, UFC</returns>
    Public Function Leggi(ByVal allevamenti_Cod As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Allevamenti_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT [Regione_Cod], ")
            strSql.AppendLine("[UMA_All_Cod], ")
            strSql.AppendLine("[UMA_All_Des], ")
            strSql.AppendLine("[inviato], ")
            strSql.AppendLine("[datainvio], ")
            strSql.AppendLine("[Data_Creazione], ")
            strSql.AppendLine("[Data_Modifica], ")
            strSql.AppendLine("[Username_Creazione], ")
            strSql.AppendLine("[Username_Modifica], ")
            strSql.AppendLine("[Validita_Inizio], ")
            strSql.AppendLine("[Validita_Fine], ")
            strSql.AppendLine("[UMA_AllGru_Cod], ")
            strSql.AppendLine("[UMA_AllGru_Des] ")
            strSql.AppendLine("FROM UMA_Allevamenti")
            strSql.AppendLine("WHERE 1=1 ")

            If allevamenti_Cod <> "" Then
                strSql.AppendLine("AND UMA_Allevamenti.UMA_All_Cod = '" & Agro_SQL_SaveText(allevamenti_Cod) + "' ")
            End If
            'If destinazione_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Allevamenti.Destinazione_Cod = '" & destinazione_Cod + "' ")
            'End If
            'If uso_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Allevamenti.Uso_Cod = '" & uso_Cod + "' ")
            'End If
            'If qualita_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Allevamenti.Qualita_Cod = '" & qualita_Cod + "' ")
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   UMA_Allevamenti.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   UMA_Allevamenti.Inviato = -1 ")
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

    Public Function Gruppi_Allevamento_Leggi(ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim stb As New StringBuilder

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Allevamenti_R.Gruppi_Allevamento_Leggi()"

        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("SELECT [UMA_AllGru_Cod], [UMA_AllGru_Des] ")
            stb.AppendLine("FROM [UMA_Gruppi_Allevamento] ")
            stb.AppendLine("GROUP BY [UMA_AllGru_Cod], [UMA_AllGru_Des] ")
            stb.AppendLine("ORDER BY [UMA_AllGru_Cod], [UMA_AllGru_Des]")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] :    " & ex.Message)
        End Try

        Return dt

    End Function

    Public Sub ComponiFiltroAggiuntivoValidita(ByRef filtroAggiuntivo As String, inizioValidita As String, fineValidita As String)
        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " ( (UMA_Allevamenti.Validita_Inizio >= '" & inizioValidita & "' AND UMA_Allevamenti.Validita_Fine <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_Allevamenti.Validita_Fine >= '" & inizioValidita & "' AND UMA_Allevamenti.Validita_Fine <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_Allevamenti.Validita_Inizio >= '" & inizioValidita & "' AND UMA_Allevamenti.Validita_Inizio <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_Allevamenti.Validita_Inizio <= '" & inizioValidita & "' AND UMA_Allevamenti.Validita_Fine >= '" & fineValidita & "') ) ")

    End Sub

End Class

Public Class UMA_Allevamenti_W
    Inherits DataProvider

    Public Function AggiungiNuovi(listLav As List(Of UMA_Allevamenti_Dto),
                                  ByRef objParametri As AgronicaCoreParametri,
                                  ByRef strErr As String) As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Elenco_Allevamenti_W.AggiungiNouvi()"
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

                    StrSQL.AppendLine("INSERT INTO [UMA_Allevamenti] ")
                    StrSQL.AppendLine("(")
                    StrSQL.AppendLine("[Regione_Cod], ")
                    StrSQL.AppendLine("[UMA_All_Cod], ")
                    StrSQL.AppendLine("[UMA_All_Des], ")
                    StrSQL.AppendLine("[inviato], ")
                    StrSQL.AppendLine("[datainvio], ")
                    StrSQL.AppendLine("[Data_Creazione], ")
                    StrSQL.AppendLine("[Data_Modifica], ")
                    StrSQL.AppendLine("[Username_Creazione], ")
                    StrSQL.AppendLine("[Username_Modifica], ")
                    StrSQL.AppendLine("[Validita_Inizio], ")
                    StrSQL.AppendLine("[Validita_Fine], ")
                    StrSQL.AppendLine("[UMA_AllGru_Cod], ")
                    StrSQL.AppendLine("[UMA_AllGru_Des] ")
                    StrSQL.AppendLine(") ")
                    StrSQL.AppendLine("VALUES")
                    StrSQL.AppendLine("( ")
                    StrSQL.AppendLine($"'010', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.UMA_All_Cod)}', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.UMA_All_Des)}', ")
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
                    StrSQL.AppendLine($"{Agro_SQL_SaveDate(l.Validita_Fine)}, ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.UMA_AllGru_Cod)}', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.UMA_AllGru_Des)}' ")
                    StrSQL.AppendLine($")")

                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                Else
                    strErr = String.Concat(strErr, vbCrLf & " Riga non inserita: Codice: " & l.UMA_All_Cod & ", Descrizione: " & l.UMA_All_Des &
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

    Public Function Rimuovi(lista As List(Of UMA_Allevamenti_Dto), objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Allevamenti_W.Rimuovi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            For Each l In lista

                StrSQL.AppendLine("DELETE FROM [UMA_Allevamenti] ")
                StrSQL.AppendLine($"WHERE [Regione_Cod] = '{Agro_SQL_SaveText(l.Regione_Cod)}' ")
                StrSQL.AppendLine($"AND [UMA_All_Cod] = '{Agro_SQL_SaveText(l.UMA_All_Cod)}' ")
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

    Public Function Aggiorna(righeModificateArr As List(Of UMA_Allevamenti_Dto),
                             ByRef objParametri As AgronicaCoreParametri,
                             ByRef StrErr As String) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Allevamenti_W.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As StringBuilder = New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Try

            For Each l In righeModificateArr

                If CheckDuplicati(l, objParametri, True) Then

                    l.Username_Modifica = objParametri.UtenteUsername
                    l.Data_Modifica = Date.Now

                    StrSQL.AppendLine("UPDATE [UMA_Allevamenti] ")
                    StrSQL.AppendLine($"SET [UMA_All_Des] = '{Agro_SQL_SaveText(l.UMA_All_Des)}', ")
                    StrSQL.AppendLine($"[inviato] = {Agro_SQL_SaveNum(l.Inviato)}, ")
                    StrSQL.Append($"[datainvio] = ")
                    If IsNothing(l.DataInvio) Then
                        StrSQL.AppendLine("NULL, ")
                    Else
                        StrSQL.AppendLine(" " & Agro_SQL_SaveDate(l.DataInvio) & ", ")
                    End If
                    StrSQL.AppendLine($"[Data_Modifica] = {Agro_SQL_SaveDate(l.Data_Modifica)}, ")
                    StrSQL.AppendLine($"[Username_Modifica]= '{Agro_SQL_SaveText(l.Username_Modifica)}', ")
                    StrSQL.AppendLine($"[Validita_Fine] =  {Agro_SQL_SaveDate(l.Validita_Fine)}, ")
                    StrSQL.AppendLine($"[UMA_AllGru_Cod] = '{Agro_SQL_SaveText(l.UMA_AllGru_Cod)}', ")
                    StrSQL.AppendLine($"[UMA_AllGru_Des] = '{Agro_SQL_SaveText(l.UMA_AllGru_Des)}' ")

                    StrSQL.AppendLine($"WHERE [Regione_Cod] = '{Agro_SQL_SaveText(l.Regione_Cod)}' ")
                    StrSQL.AppendLine($"AND [UMA_All_Cod] = '{Agro_SQL_SaveText(l.UMA_All_Cod)}' ")
                    StrSQL.AppendLine($"AND [Validita_Inizio] = {Agro_SQL_SaveDate(l.Validita_Inizio)} ")


                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                Else
                    StrErr = String.Concat(StrErr, vbCrLf & " Riga non aggiornata: Codice: " & l.UMA_All_Cod & ", Descrizione: " & l.UMA_All_Des &
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

    Private Function CheckDuplicati(riga As UMA_Allevamenti_Dto, objParametri As AgronicaCoreParametri, aggiornamento As Boolean) As Boolean

        Dim leggi = New UMA_Allevamenti_R
        Dim filtroAggiorna = ""

        If aggiornamento Then
            filtroAggiorna = " AND UMA_Allevamenti.Validita_Inizio <> '" & riga.Validita_Inizio.ToShortDateString & "' "
        End If

        Dim dt = leggi.Leggi(riga.UMA_All_Cod,
                             " UMA_Allevamenti.Validita_Fine >= '" & riga.Validita_Inizio.ToShortDateString &
                             "' AND UMA_Allevamenti.Validita_Inizio <= '" & riga.Validita_Fine.ToShortDateString & "' " &
                             filtroAggiorna,
                             "",
                             objParametri)

        Return dt.Rows.Count = 0
    End Function

End Class

Public Class UMA_Allevamenti_Dto
    ' *************************** Colonne chiavi primarie ***************************
    Public Regione_Cod As String
    Public UMA_All_Cod As String
    Public Validita_Inizio As DateTime

    ' ************************* Colonne della tabella UMA_Allevamenti *************************
    Public UMA_All_Des As String
    Public Inviato As Short
    Public DataInvio As Date?

    Public Validita_Fine As Date

    Public Data_Creazione As Date?
    Public Data_Modifica As Date?
    Public Username_Creazione As String
    Public Username_Modifica As String

    Public UMA_AllGru_Cod As String
    Public UMA_AllGru_Des As String

End Class