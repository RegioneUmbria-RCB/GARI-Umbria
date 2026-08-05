
Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Codifica_ImpiantiIrrigui_Clienti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggixCliente(ByVal Codice_Cliente As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Clienti_R.LeggixCli()"

        Dim DT As DataTable

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SELECT *")
            sql.AppendLine("FROM Codifica_ImpiantiIrrigui_Clienti")
            sql.AppendLine("WHERE Codice_Cliente = " & Agro_SQL_SaveNum(Codice_Cliente))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            sql.Append("ORDER BY Imp_Cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

End Class
