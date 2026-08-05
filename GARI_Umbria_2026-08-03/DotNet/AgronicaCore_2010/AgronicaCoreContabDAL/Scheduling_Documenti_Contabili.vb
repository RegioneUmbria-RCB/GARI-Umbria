Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class Scheduling_Documenti_Contabili_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID As Integer,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Scheduling_Documenti_Contabili_DAL.Scheduling_Documenti_Contabili_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Scheduling_Documenti_Contabili ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ID ASC ")
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

End Class



Public Class Scheduling_Documenti_Contabili_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(lav_Cod_Destinazione As Integer,
                           numero As Integer,
                           data As Date,
                           elenco_Id_Agenda As String,
                           elenco_Id_Mov_Det As String,
                           parametri_rottura As String,
                           data_Inizio_Elaborazione As DateTime?,
                           data_Fine_Elaborazione As DateTime?,
                           numero_Documenti_Creati As Integer?,
                           Validita_Inizio As Date,
                           Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional data_creazione As Date = #2/1/1900#,
                           Optional data_modifica As Date = #2/1/1900#,
                           Optional username_creazione As String = "",
                           Optional username_modifica As String = "") As Boolean

        Dim NomeRoutine As String = "Scheduling_Documenti_Contabili_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If String.IsNullOrEmpty(username_creazione) Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If String.IsNullOrEmpty(username_modifica) Then
                username_modifica = objParametri.UsernameOperazione
            End If

            If data_creazione = #2/1/1900# Then
                data_creazione = Date.Now
            End If

            If data_modifica = #2/1/1900# Then
                data_modifica = Date.Now
            End If

            StrSQL.AppendLine(" INSERT INTO Scheduling_Documenti_Contabili ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("  Lav_Cod_Destinazione, Numero, ")
            StrSQL.AppendLine("  Data, Elenco_Id_Agenda, ")
            StrSQL.AppendLine("  elenco_Id_Mov_Det, parametri_rottura,")
            StrSQL.AppendLine("  data_Inizio_Elaborazione, data_Fine_Elaborazione, ")
            StrSQL.AppendLine("  numero_Documenti_Creati, ")
            StrSQL.AppendLine("  Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine("  Username_Creazione, Username_Modifica, ")
            StrSQL.AppendLine("  Validita_Inizio, Validita_Fine ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" VALUES ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("  " & Agro_SQL_SaveNum(lav_Cod_Destinazione) & ", ")
            StrSQL.AppendLine("  " & Agro_SQL_SaveNum(numero) & ", ")
            StrSQL.AppendLine("  " & Agro_SQL_SaveDateTime(data) & ", ")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(elenco_Id_Agenda) & "', ")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(elenco_Id_Mov_Det) & "', ")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(parametri_rottura, False) & "', ")

            If IsNothing(data_Inizio_Elaborazione) OrElse Not data_Inizio_Elaborazione.HasValue Then
                StrSQL.AppendLine("  NULL, ")
            Else
                StrSQL.AppendLine("  " & Agro_SQL_SaveDateTime(data_Inizio_Elaborazione) & ", ")
            End If
            If IsNothing(data_Fine_Elaborazione) OrElse Not data_Fine_Elaborazione.HasValue Then
                StrSQL.AppendLine("  NULL, ")
            Else
                StrSQL.AppendLine("  " & Agro_SQL_SaveDateTime(data_Fine_Elaborazione) & ", ")
            End If
            If IsNothing(numero_Documenti_Creati) OrElse Not numero_Documenti_Creati.HasValue Then
                StrSQL.AppendLine("  NULL, ")
            Else
                StrSQL.AppendLine("  " & Agro_SQL_SaveNum(numero_Documenti_Creati) & ", ")
            End If
            StrSQL.AppendLine("  " & Agro_SQL_SaveDateTime(data_creazione) & ", ")
            StrSQL.AppendLine("  " & Agro_SQL_SaveDateTime(data_modifica) & ", ")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(username_creazione) & "',")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(username_modifica) & "', ")
            StrSQL.AppendLine("  " & Agro_SQL_SaveDateTime(Validita_Inizio) & ", ")
            StrSQL.AppendLine("  " & Agro_SQL_SaveDateTime(Validita_Fine))

            StrSQL.AppendLine(" ) ")

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

    'Public Function Scrivi(ByVal piva As String,
    '                       ByVal numeratore_Tipo As Integer,
    '                       ByVal doc_Numero_Sin As String,
    '                       ByVal doc_Numero_Des As String,
    '                       ByVal descrizione As String,
    '                       ByVal lunghezza_Centro As Integer,
    '                       ByVal carattereFormattazione As String,
    '                       ByVal data_Inizio As DateTime?,
    '                       ByVal data_Fine As DateTime?,
    '                       ByVal username_creazione As String,
    '                       ByVal username_modifica As String,
    '                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

    '    Dim NomeRoutine As String = "Scrivi()"
    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        If username_creazione = "" Then
    '            username_creazione = objParametri.UsernameOperazione
    '        End If

    '        If username_modifica = "" Then
    '            username_modifica = objParametri.UsernameOperazione
    '        End If

    '        Dim sequenza = New AgronicaCoreDataProvider.Agro_Sequenze
    '        Dim Id As Integer = sequenza.NuovoId_Tabella("Scheduling_Documenti_Contabili", 0, 2000000000, objParametri)

    '        StrSQL.Length = 0
    '        StrSQL.AppendLine(" INSERT INTO Scheduling_Documenti_Contabili " + vbCrLf)
    '        StrSQL.AppendLine(" ( Id, PivaSuperUser, Piva, Numeratore_Tipo, " + vbCrLf)
    '        StrSQL.AppendLine(" doc_Numero_Sin, doc_Numero_Des, Descrizione, " + vbCrLf)
    '        StrSQL.AppendLine(" lunghezza_Centro, carattereFormattazione, " + vbCrLf)
    '        StrSQL.AppendLine(" Inviato, datainvio, " + vbCrLf)
    '        StrSQL.AppendLine(" Data_Creazione, Data_Modifica, " + vbCrLf)
    '        StrSQL.AppendLine(" Username_Creazione, Username_Modifica, " + vbCrLf)
    '        StrSQL.AppendLine(" Validita_Inizio, Validita_Fine) " + vbCrLf)

    '        StrSQL.AppendLine(" VALUES " + vbCrLf)
    '        StrSQL.AppendLine(" ( " & Id & ", " + vbCrLf)
    '        StrSQL.AppendLine(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', " + vbCrLf)
    '        StrSQL.AppendLine(" '" & Agro_SQL_SaveText(piva) & "', " + vbCrLf)
    '        StrSQL.AppendLine(numeratore_Tipo & ", " + vbCrLf)
    '        StrSQL.AppendLine(" '" & Agro_SQL_SaveText(doc_Numero_Sin) & "', " + vbCrLf)
    '        StrSQL.AppendLine(" '" & Agro_SQL_SaveText(doc_Numero_Des) & "', " + vbCrLf)
    '        StrSQL.AppendLine(" '" & Agro_SQL_SaveText(descrizione) & "', " + vbCrLf)
    '        StrSQL.AppendLine(" " & Agro_SQL_SaveNum(lunghezza_Centro) & ", " + vbCrLf)
    '        StrSQL.AppendLine(" '" & Agro_SQL_SaveText(carattereFormattazione) & "', " + vbCrLf)
    '        StrSQL.AppendLine(" 0, null, " + vbCrLf)
    '        StrSQL.AppendLine(Agro_SQL_SaveDateTime(DateTime.Now) & ", " & Agro_SQL_SaveDateTime(DateTime.Now) & ", " + vbCrLf)
    '        StrSQL.AppendLine(" '" & Agro_SQL_SaveText(username_creazione) & "', '" & Agro_SQL_SaveText(username_modifica) & "', " + vbCrLf)
    '        StrSQL.AppendLine(Agro_SQL_SaveDateTime(data_Inizio) & ", " & Agro_SQL_SaveDateTime(data_Fine) & vbCrLf)
    '        StrSQL.AppendLine(" )")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function



    Public Function Modifica(ByVal ID As Integer,
                             ByVal Numero_Documenti_Creati As Integer,
                             ByVal Data_Inizio_Elaborazione As DateTime,
                             ByVal Data_Fine_Elaborazione As DateTime,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Scheduling_Documenti_Contabili.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Scheduling_Documenti_Contabili SET ")
            StrSQL.AppendLine("    Numero_Documenti_Creati    = isnull(Numero_Documenti_Creati, 0) + " & Agro_SQL_SaveNum(Numero_Documenti_Creati) & "  ")
            StrSQL.AppendLine("   ,Data_Inizio_Elaborazione   =  " & Agro_SQL_SaveDateTime(Data_Inizio_Elaborazione))
            StrSQL.AppendLine("   ,Data_Fine_Elaborazione     =  " & Agro_SQL_SaveDateTime(Data_Fine_Elaborazione))

            StrSQL.AppendLine(" WHERE ID    = " & Agro_SQL_SaveNum(ID))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal ID As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "Cancella()"

        '====================================================================================
        'Parametri opzionali :   

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM Scheduling_Documenti_Contabili " + vbCrLf)
            StrSQL.AppendLine(" WHERE Id = " & Agro_SQL_SaveNum(ID) & " " & vbCrLf)

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
            End If

            ' --------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            ' --------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function


End Class


