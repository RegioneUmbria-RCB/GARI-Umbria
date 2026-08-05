Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class IsAlive
	Inherits System.Web.Services.WebService

	<WebMethod(EnableSession:=True)>
	Public Function IsAlive(InData As Object) As RispostaStandard
		Dim r As New RispostaStandard

		r.RispostaStringa = My.Computer.FileSystem.ReadAllText(Server.MapPath("..") & "\GiasVersioneCorrente.txt")
		r.RispostaOK = True

		Return r

	End Function


End Class