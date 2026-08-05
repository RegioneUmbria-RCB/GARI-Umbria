Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
<System.Web.Script.Services.ScriptService()>
Public Class AnagraficaSharedFns
    Inherits System.Web.Services.WebService
    Public Sub objectConversion(ByVal anObject As Object)
        Dim agenda As ParametriAgenda
        agenda = CType(anObject, ParametriAgenda)
    End Sub
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function DatiRelativiPercorsoBreadcrumbs_NG(InData As CoreWS_Generic(Of DatiRelativiPercorsoBreadcrumbs)) As RispostaStandard
        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim biz As New Breadcrumbs(objP_Server, objP_Utenti)
            Dim tipoPagina As Integer = InData.InData.tipoPagina
            Dim agenda As New BreadcrumbParams
            agenda.Piva = InData.InData.agenda.Piva
            agenda.Sa_Cod = InData.InData.agenda.Sa_Cod
            agenda.Campo_Cod = InData.InData.agenda.Campo_Cod
            Return ProvideRispostaStandardFrom(biz.GetBreadcrumbs(tipoPagina, agenda))
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function DatiRelativiPercorsoBreadcrumbs(agenda As ParametriAgenda, tipoPagina As Integer, objParamServer As String,
                                                    objParamUtenti As String) As RispostaStandard
        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParamServer)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParamUtenti)

            Dim biz As New Breadcrumbs(objP_Server, objP_Utenti)
            Return ProvideRispostaStandardFrom(biz.GetBreadcrumbs(tipoPagina, ParseAgenda(agenda)))
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    Private Function ParseAgenda(agenda As ParametriAgenda)
        Return New BreadcrumbParams With {
            .Piva = agenda.Piva,
            .Sa_Cod = agenda.Sa_Cod,
            .Campo_Cod = agenda.Campo_Cod
        }
    End Function

End Class