Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Causali_Fattura
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCausaliFattura(ByVal piva As String,
                                        ByVal codice As Integer,
                                        ByVal categoria As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByVal objP_server As String
                                        ) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim dal As New AgronicaCoreContabDAL.Causali_Fattura_R
            Dim dt As DataTable = dal.Leggi(piva, codice, categoria, xFiltroAggiuntivo, xOrderBy, objParametriServer)

            Dim jArrayLista As New JArray()

            For Each dr As DataRow In dt.Rows
                Dim jObj As New JObject(New JProperty("Cau_Contab_Codice", CInt(dr("Codice"))),
                                        New JProperty("Cau_Contab_Descrizione", dr("Descrizione")),
                                        New JProperty("Cau_Contab_Categoria", dr("Categoria")),
                                        New JProperty("Piva", dr("Piva"))
                                        )
                jArrayLista.Add(jObj)
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

End Class