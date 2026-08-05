Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions

Public Class UMA_Causali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Causali_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Causali t ")
            stb.AppendLine(" where Causale_Tipo = 1 ")
            stb.AppendLine($" and Piva_SuperUser = '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}' ")
            stb.AppendLine(" ORDER BY Causale_Des ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt


    End Function



End Class

