Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.Utility
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class ImportContatti
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ImportLavoratoriQDCDemetra(InData As Object) As Api_Response

        Dim r As New Api_Response

        Try

            Dim objRequest As CoreWS_Generic(Of ImportDemetra) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ImportDemetra))(JsonConvert.SerializeObject(InData))
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)


            Dim errorMessage As String = String.Empty
            Dim objDemetraBIZ_util As New AgronicaCoreDemetraBIZ.Util
            If Not objDemetraBIZ_util.CheckRequest(objRequest, errorMessage) Then
                Throw New Exception(errorMessage)
            End If

            Dim cuaaRequest As String = objRequest.InData.CUAA

            Dim datiJson As String = AgroZip.DeCompressioneBase64(1, objRequest.InData.dati)
            Dim anagrafica As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto) =
                JsonConvert.DeserializeObject(Of AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto))(datiJson)

            errorMessage = String.Empty
            Dim importatore As New AgronicaCoreDemetraBIZ.ImportContatti(cuaaRequest, Nothing, objParametri_Server, objParametri_Utenti)
            errorMessage = importatore.Importa(cuaaRequest, anagrafica)

            If errorMessage <> "" Then
                Throw New Exception(errorMessage)
            End If

            r.message = Api_Response_Message_Type.Ok

        Catch ex As Exception

            r.errore = ex.Message
            r.message = ""

        End Try

        Return r

    End Function

End Class