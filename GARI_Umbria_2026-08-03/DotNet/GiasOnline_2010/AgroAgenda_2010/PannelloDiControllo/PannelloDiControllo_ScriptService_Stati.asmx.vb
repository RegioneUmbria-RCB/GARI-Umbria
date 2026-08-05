Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Web.Script.Serialization
Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
Public Class PannelloDiControllo_ScriptService_Stati
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function Leggi_Stati() As rispostaStandard(Of List(Of ListItem))
        Dim r As New rispostaStandard(Of List(Of ListItem))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim StatiR As New Stati_R
        Dim tabStati As List(Of PnlCtrl_Stati) = StatiR.leggi_PnlCtrl_Stati(objParametri_Server)

        Dim lista As New List(Of ListItem)
        lista.Add(New ListItem("Nessuna Selezione", -1))
        For Each s As PnlCtrl_Stati In tabStati
            lista.Add(New ListItem(s.Nome, s.ID))
        Next

        r.RispostaOK = True
        r.RispostaStringa = lista

        Return r
    End Function

End Class