Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreContabObject
Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.Web.Script.Services.ScriptService()> _
Public Class Intestazione
    Inherits System.Web.Services.WebService

    <WebMethod()> _
     <Script.Services.ScriptMethod()> _
    Public Function getHeaderPagina(ByVal Piva As String, ByVal Lav_Cod As Integer, _
                                ByVal objParametri As String) As rispostaHeader
        Dim risp As New rispostaHeader
        Dim objA As New AgronicaCoreAnagrafeDAL.Imprese_Read
        risp.Rag_Soc = objA.RagSoc_from_Piva(Piva, Utility.convertStringtoOBJparametri(objParametri))
        risp.Nome = Utility.convertStringtoOBJparametri(objParametri).UtenteUsername
        Return risp
    End Function

End Class


Public Class rispostaHeader
    Public Rag_Soc As String
    Public Nome As String

End Class