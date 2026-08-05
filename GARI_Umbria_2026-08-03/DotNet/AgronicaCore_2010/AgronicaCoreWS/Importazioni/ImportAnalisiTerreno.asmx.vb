Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreModelsSTD.Utility

Public Class objAnalisiDemetra
    Public ListaAnalisiTerreno As List(Of AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno)
End Class

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class ImportAnalisiTerreno
    Inherits WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportAnalisiTerrenoDemetra(InData As Object) As Api_Response

        Dim r As New Api_Response

        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ImportDemetra))(JsonConvert.SerializeObject(InData))
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

            Dim datiJson As String = AgroZip.DeCompressioneBase64(1, objRequest.InData.dati)
            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim datiRequest As objAnalisiDemetra = JsonConvert.DeserializeObject(Of objAnalisiDemetra)(datiJson, tzh)

            errorMessage = ""
            Dim objDemetraBIZ_import As New AgronicaCoreDemetraBIZ.ImportAnalisiTerreno
            objDemetraBIZ_import.ImportAnalisiTerreno(cuaaRequest, datiRequest.ListaAnalisiTerreno, errorMessage, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, objRequest.objP.user_Agent)

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