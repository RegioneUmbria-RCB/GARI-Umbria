Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
'Imports AgronicaCoreUmaDal.UMASetup_W

Public Class UMA_Lavorazioni_R
    Inherits DataProvider

    ''' <summary>
    ''' Restituisce le UF prodotte della coltura interessata
    ''' </summary>
    ''' <param name="lavorazione_Cod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>Una dataTable con 3 colonne: UF, UFL, UFC</returns>
    Public Function Leggi(ByVal lavorazione_Cod As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Lavorazioni_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT [Regione_Cod], ")
            strSql.AppendLine("[Lav_UMA_Cod], ")
            strSql.AppendLine("[Lav_UMA_Des], ")
            strSql.AppendLine("[inviato], ")
            strSql.AppendLine("[datainvio], ")
            strSql.AppendLine("[Data_Creazione], ")
            strSql.AppendLine("[Data_Modifica], ")
            strSql.AppendLine("[Username_Creazione], ")
            strSql.AppendLine("[Username_Modifica], ")
            strSql.AppendLine("[Validita_Inizio], ")
            strSql.AppendLine("[Validita_Fine], ")
            strSql.AppendLine("[Maggiorazione_Terreno_MedioTenace], ")
            strSql.AppendLine("[GestioneTerzista], ")
            strSql.AppendLine("[Utilizzata_Da_Consorzio_Bonifica] ")
            strSql.AppendLine("FROM UMA_Lavorazioni")
            strSql.AppendLine("WHERE 1=1 ")

            If lavorazione_Cod <> "" Then
                strSql.AppendLine("AND UMA_Lavorazioni.Lav_UMA_Cod = '" & Agro_SQL_SaveText(lavorazione_Cod) & "' ")
            End If
            'If destinazione_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Lavorazioni.Destinazione_Cod = '" & destinazione_Cod + "' ")
            'End If
            'If uso_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Lavorazioni.Uso_Cod = '" & uso_Cod + "' ")
            'End If
            'If qualita_Cod <> "" Then
            '    strSql.AppendLine("AND UMA_Lavorazioni.Qualita_Cod = '" & qualita_Cod + "' ")
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   UMA_Lavorazioni.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   UMA_Lavorazioni.Inviato = -1 ")
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

    Public Sub ComponiFiltroAggiuntivoValidita(ByRef filtroAggiuntivo As String, inizioValidita As String, fineValidita As String)
        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " ( (UMA_Lavorazioni.Validita_Inizio >= '" & inizioValidita & "' AND UMA_Lavorazioni.Validita_Fine <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_Lavorazioni.Validita_Fine >= '" & inizioValidita & "' AND UMA_Lavorazioni.Validita_Fine <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_Lavorazioni.Validita_Inizio >= '" & inizioValidita & "' AND UMA_Lavorazioni.Validita_Inizio <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_Lavorazioni.Validita_Inizio <= '" & inizioValidita & "' AND UMA_Lavorazioni.Validita_Fine >= '" & fineValidita & "') ) ")

    End Sub

End Class

Public Class UMA_Lavorazioni_W
    Inherits DataProvider

    Public Function AggiungiNuovi(listLav As List(Of UMA_Lavorazioni_Dto),
                                  ByRef objParametri As AgronicaCoreParametri,
                                  ByRef strErr As String) As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Lavorazioni_W.AggiungiNouvi()"
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

                    StrSQL.Append(" INSERT INTO [dbo].[UMA_Lavorazioni] ")
                    StrSQL.AppendLine("( ")
                    StrSQL.AppendLine("[Regione_Cod], ")
                    StrSQL.AppendLine("[Lav_UMA_Cod], ")
                    StrSQL.AppendLine("[Lav_UMA_Des], ")
                    StrSQL.AppendLine("[inviato], ")
                    StrSQL.AppendLine("[datainvio], ")
                    StrSQL.AppendLine("[Data_Creazione], ")
                    StrSQL.AppendLine("[Data_Modifica], ")
                    StrSQL.AppendLine("[Username_Creazione], ")
                    StrSQL.AppendLine("[Username_Modifica], ")
                    StrSQL.AppendLine("[Validita_Inizio], ")
                    StrSQL.AppendLine("[Validita_Fine], ")
                    StrSQL.AppendLine("[Maggiorazione_Terreno_MedioTenace], ")
                    StrSQL.AppendLine("[GestioneTerzista], ")
                    StrSQL.AppendLine("[Utilizzata_Da_Consorzio_Bonifica] ")
                    StrSQL.AppendLine(") ")
                    StrSQL.AppendLine("VALUES ")
                    StrSQL.AppendLine("( ")
                    StrSQL.AppendLine($"'010', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.Lav_UMA_Cod)}', ")
                    StrSQL.AppendLine($"'{Agro_SQL_SaveText(l.Lav_UMA_Des)}', ")
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
                    StrSQL.AppendLine($"{Agro_SQL_SaveBoolean(l.Maggiorazione_Terreno_MedioTenace)}, ")
                    StrSQL.AppendLine($"{Agro_SQL_SaveBoolean(l.GestioneTerzista)}, ")
                    StrSQL.AppendLine($"{Agro_SQL_SaveBoolean(l.Utilizzata_Da_Consorzio_Bonifica)}")

                    StrSQL.AppendLine($")")

                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                Else
                    strErr = String.Concat(strErr, vbCrLf & " Riga non inserita: Codice: " & l.Lav_UMA_Cod & ", Descrizione: " & l.Lav_UMA_Des &
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

    Public Function Rimuovi(lista As List(Of UMA_Lavorazioni_Dto), objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Lavorazioni_W.Rimuovi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            For Each l In lista

                StrSQL.AppendLine("DELETE FROM [UMA_Lavorazioni] ")
                StrSQL.AppendLine($"WHERE [Regione_Cod] = '{Agro_SQL_SaveText(l.Regione_Cod)}' ")
                StrSQL.AppendLine($"AND [Lav_UMA_Cod] = '{Agro_SQL_SaveText(l.Lav_UMA_Cod)}' ")
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

    Public Function Aggiorna(righeModificateArr As List(Of UMA_Lavorazioni_Dto),
                             ByRef objParametri As AgronicaCoreParametri,
                             ByRef StrErr As String) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Lavorazioni_W.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As StringBuilder = New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Try

            For Each l In righeModificateArr

                If CheckDuplicati(l, objParametri, True) Then

                    l.Username_Modifica = objParametri.UtenteUsername
                    l.Data_Modifica = Date.Now

                    StrSQL.AppendLine("UPDATE [UMA_Lavorazioni] ")
                    StrSQL.AppendLine($"SET [Lav_UMA_Des] = '{Agro_SQL_SaveText(l.Lav_UMA_Des)}', ")
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
                    StrSQL.AppendLine($"[Maggiorazione_Terreno_MedioTenace] = {Agro_SQL_SaveBoolean(l.Maggiorazione_Terreno_MedioTenace)}, ")
                    StrSQL.AppendLine($"[GestioneTerzista] = {Agro_SQL_SaveBoolean(l.GestioneTerzista)}, ")
                    StrSQL.AppendLine($"[Utilizzata_Da_Consorzio_Bonifica] = {Agro_SQL_SaveBoolean(l.Utilizzata_Da_Consorzio_Bonifica)} ")

                    StrSQL.AppendLine($"WHERE [Regione_Cod] = '{Agro_SQL_SaveText(l.Regione_Cod)}' ")
                    StrSQL.AppendLine($"AND [Lav_UMA_Cod] = '{Agro_SQL_SaveText(l.Lav_UMA_Cod)}' ")
                    StrSQL.AppendLine($"And [Validita_Inizio] = {Agro_SQL_SaveDate(l.Validita_Inizio)} ")

                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                Else
                    StrErr = String.Concat(StrErr, vbCrLf & " Riga non aggiornata: Codice: " & l.Lav_UMA_Cod & ", Descrizione: " & l.Lav_UMA_Des &
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

    Private Function CheckDuplicati(riga As UMA_Lavorazioni_Dto, objParametri As AgronicaCoreParametri, aggiornamento As Boolean) As Boolean

        Dim leggi = New UMA_Lavorazioni_R
        Dim filtroAggiorna = ""

        If aggiornamento Then
            filtroAggiorna = " AND UMA_Lavorazioni.Validita_Inizio <> '" & riga.Validita_Inizio.ToShortDateString & "' "
        End If

        Dim dt = leggi.Leggi(riga.Lav_UMA_Cod,
                             " UMA_Lavorazioni.Validita_Fine >= '" & riga.Validita_Inizio.ToShortDateString &
                             "' AND UMA_Lavorazioni.Validita_Inizio <= '" & riga.Validita_Fine.ToShortDateString & "' " &
                             filtroAggiorna,
                             "",
                             objParametri)

        Return dt.Rows.Count = 0
    End Function

    Private Function Agro_SQL_SaveBoolean(ByVal value As Boolean) As Byte
        Dim result As Byte = 0
        If (value) Then
            result = 1
        End If
        Return result
    End Function


End Class

Public Class UMA_Lavorazioni_Dto
    ' *************************** Colonne chiavi primarie ***************************
    Public Regione_Cod As String
    Public Lav_UMA_Cod As String
    Public Validita_Inizio As DateTime

    ' ************************* Colonne della tabella UMA_Lavorazioni *************************
    Public Lav_UMA_Des As String
    Public Inviato As Short
    Public DataInvio As Date?

    Public Validita_Fine As Date

    Public Data_Creazione As Date?
    Public Data_Modifica As Date?
    Public Username_Creazione As String
    Public Username_Modifica As String

    Public Maggiorazione_Terreno_MedioTenace As Boolean
    Public GestioneTerzista As Boolean
    Public Utilizzata_Da_Consorzio_Bonifica As Boolean

End Class