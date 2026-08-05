Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Planning
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function



    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Function ScriviPianificazione(ByVal s As String, ByVal objP_server As String) As rispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..
            Dim xScrivi As New AgronicaCoreAnagrafeBIZ.Programmazione_W

            Dim rval As String = ""
            'xScrivi.Programmazione_EF_Scrivi(s, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function




End Class