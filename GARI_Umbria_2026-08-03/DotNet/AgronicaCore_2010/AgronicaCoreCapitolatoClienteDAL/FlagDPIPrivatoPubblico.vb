Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text

Public Class FlagDPIPrivatoPubblico_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiElencoFlag(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.FlagDPIPrivatoPubblico_R.LeggiElencoFlag()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim stb As New StringBuilder

        Try

            stb.Length = 0

            stb.AppendLine("    Select ")
            stb.AppendLine("        Flag_DPI_COD, Descrizione ")
            stb.AppendLine("    From ")
            stb.AppendLine("        Flag_Privato_Pubblico_DPI ")

            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function
End Class
