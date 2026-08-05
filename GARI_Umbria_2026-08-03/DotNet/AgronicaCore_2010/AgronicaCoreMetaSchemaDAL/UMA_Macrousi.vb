Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Macrousi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Macrouso_UMA_Cod As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional validitaInizio As Date = AGRODATAINIZIO,
                          Optional validitaFine As Date = AGRODATAFINE) As List(Of UMA_Macrousi)

        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.UMA_Macrousi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim umaMac As List(Of UMA_Macrousi)

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim query = (From um In GiasContext.UMA_Macrousi)

                If Macrouso_UMA_Cod <> "" Then
                    query = query.Where(Function(el) el.Macrouso_UMA_Cod = Macrouso_UMA_Cod)
                End If

                If (validitaInizio <> AGRODATAINIZIO OrElse validitaFine <> AGRODATAFINE) Then
                    query = query.Where(Function(el) el.Validita_Inizio <= validitaFine AndAlso el.Validita_Fine >= validitaInizio)
                End If

                query.Select(Function(el) el)

                umaMac = query.ToList

            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            umaMac = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return umaMac

    End Function

End Class
