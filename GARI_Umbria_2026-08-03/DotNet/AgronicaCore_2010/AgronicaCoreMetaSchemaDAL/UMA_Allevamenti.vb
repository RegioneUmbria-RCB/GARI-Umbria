Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Allevamenti_R
    Inherits DataProvider


    Public Function Leggi(ByVal regioneCod As String,
                          ByVal umaAllCod As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                          Optional ByVal validitaFine As Date = AGRODATAFINE
                          ) As List(Of UMA_Allevamenti)

        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.UMA_Allevamenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim umaAllevamenti As List(Of UMA_Allevamenti)

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim query = (From um In GiasContext.UMA_Allevamenti)

                If regioneCod <> "" Then
                    query = query.Where(Function(el) el.Regione_Cod.Equals(regioneCod))
                End If

                If umaAllCod <> "" Then
                    query = query.Where(Function(el) el.UMA_All_Cod = umaAllCod)
                End If

                If (validitaInizio <> AGRODATAINIZIO OrElse validitaFine <> AGRODATAFINE) Then
                    query = query.Where(Function(el) el.Validita_Inizio <= validitaFine AndAlso el.Validita_Fine >= validitaInizio)
                End If

                query.Select(Function(el) el)

                umaAllevamenti = query.ToList

            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            umaAllevamenti = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return umaAllevamenti

    End Function
End Class
