Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Lavorazioni_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Lav_UMA_Cod As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of UMA_Lavorazioni)


        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.UMA_Lavorazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim umaLav As List(Of UMA_Lavorazioni)

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim query = (From um In GiasContext.UMA_Lavorazioni)

                If Lav_UMA_Cod <> "" Then
                    query.Where(Function(el) el.Lav_UMA_Cod = Lav_UMA_Cod)
                End If

                query.Select(Function(el) el)

                umaLav = query.ToList

            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            umaLav = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return umaLav

    End Function
End Class
