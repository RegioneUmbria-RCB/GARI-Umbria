Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDTOStd.InData.Shared.GridDto
Imports AgronicaCoreVarieBIZ

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class VisteGriglia
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function CaricaViste(InData As CoreWS_Generic(Of ChiaveVista)) As rispostaStandard(Of List(Of Vista))
        Dim r As New rispostaStandard(Of List(Of Vista))

        Dim contextUtenti = InData.objP.objP_utenti
        If contextUtenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim visteService As New VisteGrigliaBizService()
        Dim risp = visteService.CaricaViste(InData.InData, contextUtenti)

        Return risp
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function ScriviViste(InData As CoreWS_Generic(Of SalvaVisteWrapper)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim contextUtenti = InData.objP.objP_utenti
        If contextUtenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim visteService As New AgronicaCoreAnagrafeBIZ.VisteGrigliaBizService()
        Dim json = visteService.ScriviViste(InData.InData, contextUtenti)

        Return json
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function CancellaVista(InData As CoreWS_Generic(Of ChiaveVista)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim contextUtenti = InData.objP.objP_utenti
        If contextUtenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim visteService As New VisteGrigliaBizService()
        Dim risp = visteService.CancellaVista(InData.InData, contextUtenti)

        Return risp
    End Function
End Class