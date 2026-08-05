Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq


Public Class Stalla_Raggruppamenti

    Public Function ScriviModifica(ByRef Obj_Raggruppamento_Stalla As JObject,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As RispostaStandard

        Dim r As New RispostaStandard


        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim scope As New TransactionScope()

        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim objRaggr As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_W
            If CInt(Obj_Raggruppamento_Stalla("Raggruppamento_Cod")) = 0 Then
                objRaggr.Scrivi(Obj_Raggruppamento_Stalla("PIVA"), Obj_Raggruppamento_Stalla("sa_cod"), Obj_Raggruppamento_Stalla("STA_NUM"), Obj_Raggruppamento_Stalla, GiasContext, objParametri)
            Else
                objRaggr.Modifica(Obj_Raggruppamento_Stalla("PIVA"), Obj_Raggruppamento_Stalla("sa_cod"), Obj_Raggruppamento_Stalla("STA_NUM"), Obj_Raggruppamento_Stalla("Raggruppamento_Cod"), Obj_Raggruppamento_Stalla, GiasContext, objParametri)
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message
        Finally

            GiasContext.Dispose()

        End Try
        scope.Complete()
        scope.Dispose()

        Return r

    End Function


End Class
