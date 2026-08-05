Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreScadenziario
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Tipo_Entita_Chiavi
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Tipo_Entita_Chiavi(ByVal objP_server As String, ByVal id_tipologia As Integer) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objTipo As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
            Dim dt As DataTable = objTipo.Leggi(id_tipologia, objParametri_Server)

            Dim JArrayListaOp As New JArray()
            For Each dr In dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("nome_chiave", dr.Item("nome_chiave")), New JProperty("obbligatorio", dr.Item("obbligatorio"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Tipo_Entita_Chiavi_Secondario(ByVal objP_server As String, ByVal id_tipologia As Integer) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objTipo As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
            Dim dt As DataTable = objTipo.LeggiTipoEntitaCodSecondario(id_tipologia, objParametri_Server)

            Dim JArrayListaOp As New JArray()
            For Each dr In dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("nome_chiave", dr.Item("nome_chiave")), New JProperty("obbligatorio", dr.Item("obbligatorio"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


End Class