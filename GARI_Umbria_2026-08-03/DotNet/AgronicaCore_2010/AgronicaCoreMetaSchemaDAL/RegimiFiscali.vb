Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework_POCO
Imports OutData.Metaschema


Public Class RegimiFiscali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiRegimiFiscali(ByRef objParametriServer As AgronicaCoreParametri) As List(Of ListaRegimiFiscali)
        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.RegimiFiscali_R.LeggiRegimiFiscali()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Return (
                From rf In dal.RegimiFiscali
                Let ordine = (If(rf.Codice = 0, 0, 1))
                Order By ordine Ascending, rf.Descrizione Ascending
            ) _
            .Select(Function(r) New ListaRegimiFiscali() With
            {
                .RegimeFiscale_Cod = r.rf.Codice,
                .Descrizione = r.rf.Descrizione
            }) _
            .ToList()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Function

End Class
