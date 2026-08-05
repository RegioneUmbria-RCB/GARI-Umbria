Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreContabDAL.AttivitaXOperazioni_R
Imports AgronicaCoreContabDAL.AttivitaXOperazioni_W
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class AttivitaXOperazioni
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(objP_server As String, Id_Attivita As Int32, Lav_Cod As Int32) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objAttivitaXOper As New AgronicaCoreContabDAL.AttivitaXOperazioni_R

            Dim Dt As DataTable = objAttivitaXOper.Leggi(Id_Attivita, Lav_Cod, "", "", objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each dr As DataRow In Dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("ID_Attivita", dr("ID_Attivita")), New JProperty("Lav_Cod", dr("Lav_Cod")), New JProperty("Descrizione", dr("Descrizione")), New JProperty("Sigla", dr("Sigla"))))
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
    Public Function Leggi_APP(ByVal piva As String,
                              ByVal objP_super_server As String,
                              ByVal objP_server As String,
                              ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objAttivitaXOper As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim xFiltroAggiuntivo = " A.Utilizzo_GiasAPP = 1 "
            Dim Dt As DataTable = objAttivitaXOper.Leggi(0, 0, xFiltroAggiuntivo, "", objParametri_Server, piva)

            Dim JArrayListaOp As New JArray()

            For Each dr As DataRow In Dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("ID_Attivita", dr("ID_Attivita")), New JProperty("Lav_Cod", dr("Lav_Cod")), New JProperty("Descrizione", dr("Descrizione"))))
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