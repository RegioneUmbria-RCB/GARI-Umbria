Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
<ToolboxItem(False)>
Public Class Notifiche
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NotificaCUAA(ByVal InData As Object) As RispostaStandard

        Dim xRisp As New RispostaStandard

        Try

            Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Notifiche.NotificaCUAA) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Notifiche.NotificaCUAA))(JsonConvert.SerializeObject(InData))

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim PIVA_Superuser As String = objParametri_Server.PivaSuperUser

            Dim reg_not_biz As New AgronicaCoreNotifichePushBIZ.SistemiEsterni_RicezioneNotifiche_W
            Dim anag_sysext As New AgronicaCoreMetaSchemaDAL.Sistemi_Esterni_R

            Dim dtid_extsys = anag_sysext.Leggi(0, "", AGRODATAINIZIO, AGRODATAFINE, "Sistema_Des like '%" & iData.InData.ID_ExternalSystem & "%'", "", objParametri_Server)
            If dtid_extsys.Rows.Count <= 0 Then
                Throw New Exception("Rif external system for (" & iData.InData.ID_ExternalSystem & ") not found")
            End If

            xRisp = reg_not_biz.RegistraNotificheCUAA(dtid_extsys.Rows(0)("Sistema_Cod"), iData.InData.elencoCUAA, objParametri_Server)

            xRisp.RispostaStringa = "Notifiche correttamente registrate"

        Catch ex As Exception

            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)

        End Try

        Return xRisp
    End Function

End Class