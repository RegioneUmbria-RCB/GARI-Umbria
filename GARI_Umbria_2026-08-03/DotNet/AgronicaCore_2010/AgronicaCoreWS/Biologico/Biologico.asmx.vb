Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreContabDAL
Imports AgronicaCoreBiologicoBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Biologico
    Inherits System.Web.Services.WebService

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function ReportBio(ByVal piva As String,
    '                    ByVal tipo_Report As Integer,
    '                    ByVal dataMovDal As String, ByVal dataMovAl As String,
    '                    ByVal categorieProdotti As String,
    '                    ByVal causaliMovimento As String,
    '                    ByVal objP_server As String) As RispostaStandard

    '    Dim r As New RispostaStandard

    '    If objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    End If

    '    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

    '    Try
    '        Dim reportBiz As New ReportBIO
    '        Return reportBiz.ReportBio(piva, tipo_Report, dataMovDal, dataMovAl, categorieProdotti, causaliMovimento, objParametri_Server)

    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
    '    End Try

    '    Return r
    'End Function


End Class