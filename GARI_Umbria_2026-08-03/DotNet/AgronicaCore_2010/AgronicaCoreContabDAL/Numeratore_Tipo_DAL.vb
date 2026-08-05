Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider

Public Class Numeratore_Tipo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiEF(ByVal piva As String,
                          ByVal tipo As Integer,
                          ByVal sigla As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As List(Of Numeratore_Tipo)

        Dim NomeRoutine As String = "Numeratore_Tipo_DAL.Numeratore_Tipo_R.LeggiEF()"
        Dim returnValue = New List(Of Numeratore_Tipo)

        '----- Descrizione

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            returnValue = From nt In GiasContext.Numeratore_Tipo
                          Where nt.Piva.Equals(piva) _
                          AndAlso (tipo <> 0 OrElse nt.Tipo.Equals(tipo)) _
                          AndAlso (sigla <> "" OrElse nt.Sigla.Equals(sigla))

        End Using

        Return returnValue.ToList()

    End Function

    Public Function Leggi(ByVal piva As String,
                         ByVal tipo As Integer,
                         ByVal sigla As String,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        Dim NomeRoutine As String = "Numeratore_Tipo_DAL.Numeratore_Tipo_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Numeratore_Tipo ")
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")


            If tipo <> 0 Then
                StrSQL.Append(" AND tipo = " & Agro_SQL_SaveNum(tipo) & " ")
            End If

            If Not String.IsNullOrEmpty(sigla) Then
                StrSQL.Append(" AND Sigla = '" & Agro_SQL_SaveText(sigla) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Sigla ASC ")
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



Public Class Numeratore_Tipo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub New()

    End Sub

    Public Function Scrivi(ByVal piva As String,
                           ByVal sigla As String,
                           ByVal descrizione As String,
                           ByVal data_Inizio As DateTime?,
                           ByVal data_Fine As DateTime?,
                           ByVal username_creazione As String,
                           ByVal username_modifica As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine As String = "ContabDAL.Numeratore_Tipo_W.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            Dim sequenza As New Agro_Sequenze
            Dim Tipo As Integer = sequenza.NuovoId_Tabella("Numeratore_Tipo", 0, 2000000000, objParametri)

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO Numeratore_Tipo ")
            StrSQL.AppendLine(" ( PivaSuperUser, Piva, Tipo, ")
            StrSQL.AppendLine(" Sigla, Descrizione, ")
            StrSQL.AppendLine(" Inviato, datainvio, ")
            StrSQL.AppendLine(" Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine(" Username_Creazione, Username_Modifica, ")
            StrSQL.AppendLine(" Validita_Inizio, Validita_Fine) ")

            StrSQL.AppendLine(" VALUES ")
            StrSQL.AppendLine(" ( '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(piva) & "', ")
            StrSQL.AppendLine(Tipo & ", ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(sigla) & "', ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(descrizione) & "', ")
            StrSQL.AppendLine(" 0, NULL, ")
            StrSQL.AppendLine(Agro_SQL_SaveDateTime(Now) & ", " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(username_creazione) & "', '" & Agro_SQL_SaveText(username_modifica) & "', ")
            StrSQL.AppendLine(Agro_SQL_SaveDateTime(data_Inizio) & ", " & Agro_SQL_SaveDateTime(data_Fine))
            StrSQL.AppendLine(" )")

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

    Public Function Aggiorna(ByVal piva As String,
                           ByVal tipo As Integer,
                           ByVal sigla As String,
                           ByVal descrizione As String,
                           ByVal data_Inizio As DateTime?,
                           ByVal data_Fine As DateTime?,
                           ByVal username_modifica As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Numeratore_Tipo " + vbCrLf)
            StrSQL.Append(" Set Sigla = '" & Agro_SQL_SaveText(sigla) & "', " + vbCrLf)
            StrSQL.Append(" Descrizione = '" & Agro_SQL_SaveText(descrizione) & "', " + vbCrLf)
            StrSQL.Append(" Validita_Inizio = " & Agro_SQL_SaveDateTime(data_Inizio) & ", " & vbCrLf)
            StrSQL.Append(" Validita_Fine = " & Agro_SQL_SaveDateTime(data_Inizio) & ", " & vbCrLf)
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " + vbCrLf)
            StrSQL.Append(" WHERE " + vbCrLf)
            StrSQL.Append(" PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " + vbCrLf)
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' " + vbCrLf)
            StrSQL.Append(" AND Tipo = " & Agro_SQL_SaveNum(tipo) & " " + vbCrLf)

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
    Public Function Cancella(ByVal Piva As String,
                            ByVal Tipo As Integer,
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
            StrSQL.Append(" DELETE FROM numeratore_tipo " + vbCrLf)
            StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StrSQL.Append(" And Tipo = " & Agro_SQL_SaveNum(Tipo) & " " & vbCrLf)

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
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


