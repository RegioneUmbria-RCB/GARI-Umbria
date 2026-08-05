Imports System.IO
Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.exceptions


<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class ImportazioneUtenti
    Inherits WebService

    Private Function GetObjParams(Of T)(inData As CoreWS_Generic(Of T)) As ObjParams
        Return New ObjParams With {
            .ObjParametri_SuperServer = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_super_server),
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_server),
            .ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_utenti)
        }
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ImportaUtentiDaExcel(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.Utility.FileWrapperAlt)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim importer = New AgronicaCoreUtentiBIZ.ImportazioneUtenti
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            }
            Dim response = importer.ImportUsersFromExcel(InData.InData, params)
            res.RispostaStringa = JsonConvert.SerializeObject(response.UsersOk)
            res.Errore = JsonConvert.SerializeObject(response.UsersErrors)
            res.RispostaOK = True
        Catch ex As GiasException
            res.RispostaOK = False
            res.Errore = ex.Message
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

End Class
