Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.Web.Script.Services.ScriptService()> _
Public Class Operazione_Agenda_Utility
    Inherits System.Web.Services.WebService



    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function ControllaOperazione(ByVal objParametri As String) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParametri)

        Try

            'Inserire il codice QUI..

            Dim obj As Boolean

            r.RispostaOK = True
            r.RispostaStringa = obj

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function




End Class