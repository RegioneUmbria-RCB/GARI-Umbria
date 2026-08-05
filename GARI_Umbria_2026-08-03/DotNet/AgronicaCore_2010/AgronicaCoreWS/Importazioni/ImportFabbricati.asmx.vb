Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreInterscambioBIZ
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
Public Class ImportFabbricati
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportFabbricatiDemetra(InData As Object) As Api_Response

        Dim r As New Api_Response

        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ImportDemetra))(JsonConvert.SerializeObject(InData))
        Dim datiStr As String = AgroZip.DeCompressioneBase64(1, objRequest.InData.dati)
        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

        Try

            Dim errorMessage As String = ""
            Dim objDemetraBIZ_util As New AgronicaCoreDemetraBIZ.Util
            If Not objDemetraBIZ_util.CheckRequest(objRequest, errorMessage) Then
                Throw New Exception(errorMessage)
            End If

            Dim cuaaRequest As String = objRequest.InData.CUAA
            'Dim datiRequest As AnagraficaWrapper(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato) = objRequest.InData.dati
            Dim datiRequest As AnagraficaWrapper(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato) = JsonConvert.DeserializeObject(Of AnagraficaWrapper(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato))(datiStr)

            errorMessage = ""

            Dim objDemetraFabbricatiBIZ_import As New AgronicaCoreDemetraBIZ.ImportFabbricati

            objDemetraFabbricatiBIZ_import.ImportFabbricato(cuaaRequest, datiRequest, errorMessage, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)


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