Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.NewAgri
Imports AgronicaCoreModelsSTD.Utility
Imports AgronicaCoreUtentiBIZ
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports OutData.NewAgri

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class NewAgri
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function N_Distribuito(InData As Object) As Api_Response
        Dim r As New Api_Response

        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.NewAgri.RequestNDistribuito))(JsonConvert.SerializeObject(InData))
        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

        Try

            Dim errorMessage As String = ""
            Dim objPUA_Util As New AgronicaCorePUA_BIZ.Util
            If Not objPUA_Util.CheckRequest(objRequest.InData, errorMessage) Then
                Throw New Exception(errorMessage)
            End If

            errorMessage = ""
            Dim objPUA As New AgronicaCorePUA_BIZ.NewAgri
            Dim datiResponse = objPUA.N_Distribuito(objRequest.InData, errorMessage, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            If errorMessage <> "" Then
                Throw New Exception(errorMessage)
            End If

            r.dati = datiResponse
            r.message = Api_Response_Message_Type.Ok

        Catch ex As Exception

            r.errore = ex.Message
            r.message = ""

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Aggiorna_Impianti_N_Pua(InData As Object) As Api_Response
        Dim r As New Api_Response
        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of RequestNDistribuito))(JsonConvert.SerializeObject(InData))
        Dim params As New ObjParams With {
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server),
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
        }

        Try
            Dim errorMessage As String = ""
            Dim objPUA_Util As New AgronicaCorePUA_BIZ.Util
            If Not objPUA_Util.CheckRequest(objRequest.InData, errorMessage) Then
                r.errore = errorMessage
                Return r
            End If

            Dim objPUA As New AgronicaCorePUA_BIZ.NewAgri
            Dim datiResponse = objPUA.Aggiorna_Impianti_N_Pua(objRequest.InData, params)

            r.dati = datiResponse

            If datiResponse.ElencoAppezzamenti.All(Function(a) a.message = Api_Response_Message_Type.Ok) Then
                r.message = Api_Response_Message_Type.Ok
            Else
                r.errore = "Uno o più impianti hanno riscontrato un problema durante l'operazione."
            End If
        Catch ex As Exception
            r.errore = ex.Message
            r.message = ""
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ImportUtenteNewAgri(InData As Object) As Api_Response
        Dim r As New Api_Response
        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of RequestUtente))(JsonConvert.SerializeObject(InData))
        Dim params As New ObjParams With {
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server),
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
        }

        Try
            Dim errorMessage As String = ""
            Dim objPUA_Util As New AgronicaCorePUA_BIZ.Util
            If Not objPUA_Util.CheckRequest(objRequest.InData, errorMessage) Then
                r.errore = errorMessage
                Return r
            End If

            Dim objImportazioneUtenti As New AgronicaCoreUtentiBIZ.ImportazioneUtenti
            Dim datiResponse = objImportazioneUtenti.ImportUserNewAgri(objRequest.InData, params)

            'r.dati = datiResponse

            If datiResponse.result = True Then
                r.message = Api_Response_Message_Type.Ok
            Else
                r.errore = datiResponse.error
            End If
        Catch ex As Exception
            r.errore = ex.Message
            r.message = ""
        End Try

        Return r
    End Function
    
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ImportVisibilitaUtenteNewAgri(InData As Object) As Api_Response
        Dim r As New Api_Response
        Dim request = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of RequestImportVisibilita))(JsonConvert.SerializeObject(InData))
        Dim params As New ObjParams With {
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(request.objP.objP_super_server),
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(request.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(request.objP.objP_utenti)
        }

        Try
            Dim errorMessage As String = ""
            Dim objPUA_Util As New AgronicaCorePUA_BIZ.Util
            Dim usersBiz As New AgronicaCoreUtentiBIZ.Utenti
            If Not objPUA_Util.CheckRequest(request.InData, errorMessage) Then
                r.errore = errorMessage
                Return r
            End If
            If Not usersBiz.usernameUsato(request.InData.username, params.ObjParametri_Utenti) Then
                r.errore = "L'username indicato non appartiene ad alcun utente"
                r.message = "USERNAME_NOT_FOUND"
                Return r
            End If

            Dim visibilityBiz As New AgronicaCoreUtentiBIZ.ImportazioneVisibilita
            Dim companyBiz As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim companies = request.InData.aziendeConMandato.ToList
            companies.AddRange(request.InData.aziendeSenzaMandato)
            companies = companies.Where(Function(cuaa) not String.IsNullOrWhiteSpace(cuaa)).ToList

            For i As Integer = 0 To companies.Count - 1
                If companies(i).Length = 9 Then
                    companies(i) = "UZ" & companies(i)
                End If
            Next

            If companies.any then
                companyBiz.VerificaEsistenzaImpresaByCuaaElseCreate(companies, params)
            End If
            visibilityBiz.ImportVisibilityNewAgri(request.InData.username, companies, params)

            Dim groupWriter As New Gruppi_UtenteBiz(params.ObjParametri_Server, params.ObjParametri_Utenti)
            dim found As AgronicaCoreModelsSTD.profilazione.GruppoUtente = Nothing
            If request.InData.gruppiUtente.Any Then
                If IsNumeric(request.InData.gruppiUtente(0)) Then
                    found = groupWriter.FindGruppoUtente(request.InData.gruppiUtente(0))
                End If
            End If
            If IsNothing(found) Then
                'groupWriter.SetUserGroup(request.InData.username, groupWriter.DEFAULT_GROUP, true)
                r.dati = "Il gruppo indicato non è stato trovato."
            Else
                groupWriter.SetUserGroup(request.InData.username, found.codice, True)
            End If

            r.message = "OK"
        Catch ex As Exception
            r.errore = ex.Message
            r.message = ""
        End Try

        Return r
    End Function

End Class