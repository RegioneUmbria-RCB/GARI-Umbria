Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ

<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Gruppi_Utente
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaGruppoUtente(InData As CoreWS_Generic(Of GruppoUtente)) As RispostaStandard
        Dim res As New RispostaStandard
        Try
            Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objGruppiBiz = New AgronicaCoreUtentiBIZ.Gruppi_UtenteBiz(obj_Server, obj_Utenti)
            res.RispostaOK = objGruppiBiz.SalvaGruppoUtente(InData.InData)
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

End Class