Imports AgronicaCoreDataProvider

Public Class SemaforoEsportazioneBIOrogel_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiSemaforo(cuaa As String, objParametriInterscambio As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.SemaforoEsportazioneBIOrogel_R.LeggiSemaforo()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT [EseguitoRefreshDaGIAS],[Cuaa],[Data_Ora] ")
            StrSQL.AppendLine("FROM Estrazione_Gias ")
            StrSQL.AppendLine("WHERE Cuaa = '" & Agro_SQL_SaveText(cuaa) & "'")

            '--------------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objParametriInterscambio, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiSemaforo(anno_chiave As Integer, objParametriInterscambio As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.SemaforoEsportazioneBIOrogel_R.LeggiSemaforo()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT TOP 1 * ")
            StrSQL.AppendLine("FROM Semaforo ")
            StrSQL.AppendLine($" WHERE ANNO_CHIAVE = {anno_chiave}")

            '--------------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objParametriInterscambio, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function
End Class


Public Class SemaforoEsportazioneBIOrogel_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ImpostaRefreshCompletato(cuaa As String, objParametriInterscambio As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.SemaforoEsportazioneBIOrogel_W.ImpostaRefreshCompletato()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Estrazione_Gias ")
            StrSQL.AppendLine("SET EseguitoRefreshDaGIAS = 1 ")
            StrSQL.AppendLine(",[Data_Ora] = GetDate() ")
            StrSQL.AppendLine("WHERE Cuaa = '" & Agro_SQL_SaveText(cuaa) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function ImpostaRefreshInLavorazione(cuaa As String, objParametriInterscambio As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.SemaforoEsportazioneBIOrogel_W.ImpostaRefreshCompletato()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Estrazione_Gias ")
            StrSQL.AppendLine("SET EseguitoRefreshDaGIAS = 0 ")
            StrSQL.AppendLine(",[Data_Ora] = GetDate() ")
            StrSQL.AppendLine("WHERE Cuaa = '" & Agro_SQL_SaveText(cuaa) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function ImpostaRefreshInLavorazioneGiasBi(anno_chiave As Integer, objParametriInterscambio As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.SemaforoEsportazioneBIOrogel_W.ImpostaRefreshCompletato()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Semaforo ")
            StrSQL.AppendLine($"SET STATO_GIAS = 'IN CORSO',")
            StrSQL.AppendLine("[TimeStamp_Mod_Record] = GETDATE()")
            StrSQL.AppendLine($" WHERE ANNO_CHIAVE = {anno_chiave}")
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function ImpostaRefreshCompletatoGiasBi(risultatoRefresh As String, anno_chiave As Integer, objParametriInterscambio As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.SemaforoEsportazioneBIOrogel_W.ImpostaRefreshCompletato()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Semaforo ")
            StrSQL.AppendLine($"SET STATO_GIAS = '{risultatoRefresh}',")

            'Quando GIAS imposta lo STATO_GIAS = 'TERMINATO' allora aggiorna il Timestamp_Ultimo_Agg_GIAS_Completato
            If risultatoRefresh = "TERMINATO" Then
                StrSQL.AppendLine("[Timestamp_Ultimo_Agg_GIAS_Completato] = GetDate(),")
            End If
            'Quando GIAS o BI modificano il valore dello STATO_GIAS o STATO_BI, aggiornato il TimeStamp_Mod_Record
            StrSQL.AppendLine("[TimeStamp_Mod_Record] = GetDate()")

            StrSQL.AppendLine($" WHERE ANNO_CHIAVE = {anno_chiave}")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function
End Class
