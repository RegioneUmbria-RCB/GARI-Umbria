Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class DBUtilityComuni
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function getTypeCode(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                codiceRitorno As String) As String
        Dim tipo As String = ""

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getTypeCode()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT Tipo_Esito " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_CodiciRitorno " & vbCrLf)
            Stb.Append(" WHERE Codice_Esito='" & Agro_SQL_SaveText(codiceRitorno) & "'" & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count <> 1 Then
                If DT.Rows.Count < 1 Then
                    tipo = "Codice Errore non presente in anagrafica"
                Else
                    tipo = "Più definizioni per lo stesso codice errore"
                End If
            Else
                tipo = DT.Rows(0).Item("Tipo_Esito")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return tipo


    End Function

    Sub AggiornaControllataLogInvio(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, id As Integer, val As Boolean)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaControllataLogInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_LogInvio " & vbCrLf)
            Stb.Append(" SET Controllata=" & Agro_SQL_SaveBoolStrToInt(CStr(val)) & " " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_LogInvio_Cod=" & CStr(id) & " " & vbCrLf)
            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Sub

    Public Function NuovoID_ws_RegVino_LogInvio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.NuovoID_ws_RegVino_LogInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim StbCount As New System.Text.StringBuilder
        Dim DTCount As DataTable

        Dim StbId As New System.Text.StringBuilder
        Dim DTId As DataTable

        Try

            StbCount.Length = 0
            StbCount.Append(" Select count(*) as totaleLog from ws_RegVino_LogInvio " & vbCrLf)
            DTCount = EseguiQuery_Lettura(objParametri, StbCount.ToString, NomeRoutine)

            Dim totaleLog As Integer = CInt(DTCount.Rows(0).Item("totaleLog"))

            If totaleLog > 0 Then
                StbId.Length = 0
                StbId.Append(" Select Max(ws_RegVino_LogInvio_Cod) as maxID from ws_RegVino_LogInvio " & vbCrLf)
                DTId = EseguiQuery_Lettura(objParametri, StbId.ToString, NomeRoutine)
                Dim id As Integer = CInt(DTId.Rows(0).Item("maxID")) + 1
                Return id
            Else
                Return 1
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DTCount = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Public Function inserisciErrore(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      logInvio_Cod As Integer,
                                      codice_errore As String,
                                      descrizione_Errore As String,
                                      ws_regVino_Operazione_Cod As Integer,
                                      ws_RegVino_Soggetto_Cod As String,
                                      ws_RegVino_CodVaso As String,
                                      ws_RegVino_CodiceIcqrf As String,
                                      CodOper As String) As Boolean

        Dim NomeRoutine As String = "inserisciErrore()"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim response As Boolean

        Try

            Dim progressivo As Integer = getNewDettaglioErroriProgressivo(objParametri, logInvio_Cod)

            Stb.Length = 0
            Stb.Append("INSERT INTO ws_RegVino_LogInvio_Dettaglio_Errori " & vbCrLf)
            Stb.Append("(ws_RegVino_LogInvio_Cod, CodiceErrore, DescrizioneErrore, ProgressivoErrore, ws_RegVino_Operazione_Cod, ws_RegVino_Soggetto_Cod, ws_RegVino_CodVaso, ws_RegVino_CodiceIcqrf, CodOper ) " & vbCrLf)
            Stb.Append(" VALUES( " & vbCrLf)
            Stb.Append(" " & CStr(logInvio_Cod) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveText_NULL(codice_errore) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveText_NULL(descrizione_Errore) & " , " & vbCrLf)
            Stb.Append(" " & CStr(progressivo) & " , " & vbCrLf)
            Stb.Append(" " & CStr(ws_regVino_Operazione_Cod) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveText_NULL(ws_RegVino_Soggetto_Cod) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveText_NULL(ws_RegVino_CodVaso) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveText_NULL(ws_RegVino_CodiceIcqrf) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveText_NULL(CodOper) & "  " & vbCrLf)
            Stb.Append(" ) " & vbCrLf)

            response = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            response = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return response
    End Function

    Private Function getNewDettaglioErroriProgressivo(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, logInvio_Cod As Integer) As Integer
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.getNewDettaglioErroriProgressivo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim StbCount As New System.Text.StringBuilder
        Dim DTCount As DataTable
        Dim progressivo As Integer
        Try

            StbCount.Length = 0
            StbCount.Append(" Select count(*) as totaleErrori from ws_RegVino_LogInvio_Dettaglio_Errori " & vbCrLf)
            StbCount.Append(" WHERE ws_RegVino_LogInvio_Cod=" & CStr(logInvio_Cod) & " " & vbCrLf)
            DTCount = EseguiQuery_Lettura(objParametri, StbCount.ToString, NomeRoutine)
            progressivo = CInt(DTCount.Rows(0).Item("totaleErrori")) + 1


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DTCount = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return progressivo
    End Function

End Class
