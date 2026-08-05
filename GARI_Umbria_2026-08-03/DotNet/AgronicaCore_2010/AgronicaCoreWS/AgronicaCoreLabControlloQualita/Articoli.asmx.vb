Imports System.Web.Services
Imports AgronicaCoreLabControlloQualitaBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class LABCQ_Articoli
    Inherits System.Web.Services.WebService

    '<WebMethod()> <Script.Services.ScriptMethod()>
    'Public Function Leggi(objP_server As String, ByVal objP_utenti As String) As RispostaStandard

    '    Dim r As New RispostaStandard

    '    If objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    End If

    '    Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
    '    Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

    '    'Se non ho mai letto questi elementi li leggo da db
    '    Dim articoliFruttagel_R As New Articoli_R
    '    Dim lista As List(Of LCQ_Articoli) = articoliFruttagel_R.leggi_NC_Articoli(objParametri_Server, objParametri_Utenti)

    '    Dim listaJSON As New JArray()
    '    For Each obj As LCQ_Articoli In lista
    '        listaJSON.Add(New JObject(New JProperty("descr", obj.Codice + " - " + obj.Descrizione1), New JProperty("codice", obj.Codice)))
    '    Next

    '    r.RispostaStringa = JsonConvert.SerializeObject(listaJSON, Formatting.None)
    '    r.RispostaOK = True

    '    Return r
    'End Function

End Class