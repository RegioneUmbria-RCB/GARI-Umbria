Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Numeratore_PrefissoSuffisso_R
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
                         ByVal numeratore_Tipo As Integer,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                          Optional ByVal Validita_Fine As Date = AGRODATAFINE
                           ) As DataTable

        Dim NomeRoutine As String = "Numeratore_PrefissoSuffisso_DAL.Numeratore_PrefissoSuffisso_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT nps.*, nt.Descrizione as DescrizioneTipo ")
            StrSQL.AppendLine(" FROM  Numeratore_PrefissoSuffisso nps ")
            StrSQL.AppendLine(" INNER JOIN Numeratore_Tipo nt ")
            StrSQL.AppendLine(" on nps.Numeratore_Tipo =  nt.Tipo ")
            StrSQL.AppendLine(" and nps.Piva =  nt.Piva and nps.PivaSuperUser = nt.PivaSuperUser ")
            StrSQL.AppendLine(" WHERE nps.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            '12/06/2020: nella gestione dei numeratori non serve fare il filtro della finestra temporale
            'StrSQL.Appendline(" AND nps.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            'StrSQL.Appendline(" AND nps.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND nps.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" AND nps.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")

            If numeratore_Tipo <> 0 Then
                StrSQL.AppendLine(" AND nps.Numeratore_tipo = " & Agro_SQL_SaveNum(numeratore_Tipo) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY nps.Numeratore_Tipo, nps.Descrizione ASC ")
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



Public Class Numeratore_PrefissoSuffisso_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub New()

    End Sub

    Public Function Scrivi(ByVal piva As String,
                           ByVal numeratore_Tipo As Integer,
                           ByVal doc_Numero_Sin As String,
                           ByVal doc_Numero_Des As String,
                           ByVal descrizione As String,
                           ByVal lunghezza_Centro As Integer,
                           ByVal carattereFormattazione As String,
                           ByVal data_Inizio As DateTime?,
                           ByVal data_Fine As DateTime?,
                           ByVal username_creazione As String,
                           ByVal username_modifica As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            Dim sequenza = New AgronicaCoreDataProvider.Agro_Sequenze
            Dim Id As Integer = sequenza.NuovoId_Tabella("Numeratore_PrefissoSuffisso", 0, 2000000000, objParametri)

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO Numeratore_PrefissoSuffisso " + vbCrLf)
            StrSQL.AppendLine(" ( Id, PivaSuperUser, Piva, Numeratore_Tipo, " + vbCrLf)
            StrSQL.AppendLine(" doc_Numero_Sin, doc_Numero_Des, Descrizione, " + vbCrLf)
            StrSQL.AppendLine(" lunghezza_Centro, carattereFormattazione, " + vbCrLf)
            StrSQL.AppendLine(" Inviato, datainvio, " + vbCrLf)
            StrSQL.AppendLine(" Data_Creazione, Data_Modifica, " + vbCrLf)
            StrSQL.AppendLine(" Username_Creazione, Username_Modifica, " + vbCrLf)
            StrSQL.AppendLine(" Validita_Inizio, Validita_Fine) " + vbCrLf)

            StrSQL.AppendLine(" VALUES " + vbCrLf)
            StrSQL.AppendLine(" ( " & Id & ", " + vbCrLf)
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', " + vbCrLf)
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(piva) & "', " + vbCrLf)
            StrSQL.AppendLine(numeratore_Tipo & ", " + vbCrLf)
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(doc_Numero_Sin) & "', " + vbCrLf)
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(doc_Numero_Des) & "', " + vbCrLf)
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(descrizione) & "', " + vbCrLf)
            StrSQL.AppendLine(" " & Agro_SQL_SaveNum(lunghezza_Centro) & ", " + vbCrLf)
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(carattereFormattazione) & "', " + vbCrLf)
            StrSQL.AppendLine(" 0, null, " + vbCrLf)
            StrSQL.AppendLine(Agro_SQL_SaveDateTime(DateTime.Now) & ", " & Agro_SQL_SaveDateTime(DateTime.Now) & ", " + vbCrLf)
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(username_creazione) & "', '" & Agro_SQL_SaveText(username_modifica) & "', " + vbCrLf)
            StrSQL.AppendLine(Agro_SQL_SaveDateTime(data_Inizio) & ", " & Agro_SQL_SaveDateTime(data_Fine) & vbCrLf)
            StrSQL.AppendLine(" )")

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

    Public Function Aggiorna(ByVal Id As Integer,
                            ByVal piva As String,
                            ByVal numeratore_Tipo As Integer,
                            ByVal doc_Numero_Sin As String,
                            ByVal doc_Numero_Des As String,
                            ByVal descrizione As String,
                            ByVal lunghezza_Centro As Integer,
                            ByVal carattereFormattazione As String,
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
            StrSQL.AppendLine(" UPDATE Numeratore_PrefissoSuffisso " + vbCrLf)
            StrSQL.AppendLine(" Set Numeratore_Tipo = " & Agro_SQL_SaveNum(numeratore_Tipo) & ", " + vbCrLf)
            StrSQL.AppendLine(" Descrizione = '" & Agro_SQL_SaveText(descrizione) & "', " + vbCrLf)
            StrSQL.AppendLine(" Doc_Numero_Sin = '" & Agro_SQL_SaveText(doc_Numero_Sin) & "', " + vbCrLf)
            StrSQL.AppendLine(" Doc_Numero_Des = '" & Agro_SQL_SaveText(doc_Numero_Des) & "', " + vbCrLf)
            StrSQL.AppendLine(" Lunghezza_Centro = " & Agro_SQL_SaveNum(lunghezza_Centro) & ", " + vbCrLf)
            StrSQL.AppendLine(" CarattereFormattazione = '" & Agro_SQL_SaveText(carattereFormattazione) & "', " + vbCrLf)
            StrSQL.AppendLine(" Validita_Inizio = " & Agro_SQL_SaveDateTime(data_Inizio) & ", " & vbCrLf)
            StrSQL.AppendLine(" Validita_Fine = " & Agro_SQL_SaveDateTime(data_Fine) & ", " & vbCrLf)
            StrSQL.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " + vbCrLf)
            StrSQL.AppendLine(" WHERE " + vbCrLf)
            StrSQL.AppendLine(" PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " + vbCrLf)
            StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' " + vbCrLf)
            StrSQL.AppendLine(" AND Id = " & Agro_SQL_SaveNum(Id) & " " + vbCrLf)

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
    Public Function Cancella(ByVal Id As Integer,
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
            StrSQL.AppendLine(" DELETE FROM Numeratore_PrefissoSuffisso " + vbCrLf)
            StrSQL.AppendLine(" WHERE Id = " & Agro_SQL_SaveNum(Id) & " " & vbCrLf)

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
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


