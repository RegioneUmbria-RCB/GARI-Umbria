
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class PDC_Varie_r
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggixCancellazionePiva(ByVal Piva As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_Dettagli_R.Leggi()"
         

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.Append(" SELECT *  ")

            StrSQL.Append(" FROM PDC_Dettagli ")

            StrSQL.Append(" WHERE PDC_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

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

 