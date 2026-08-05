Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Newtonsoft.Json
' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.Web.Script.Services.ScriptService()> _
Public Class Test
    Inherits System.Web.Services.WebService




    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Function rispostaOggetto_Test(ByVal pippo As String) As AgronicaCoreWS.OggettoRisposta
        Dim ris As New AgronicaCoreWS.OggettoRisposta

        ris.risposta = True
        ris.messaggio = pippo


        Return ris
    End Function

End Class