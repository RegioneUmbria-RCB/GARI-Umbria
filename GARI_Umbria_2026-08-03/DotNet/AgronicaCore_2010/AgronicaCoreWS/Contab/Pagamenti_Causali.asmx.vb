Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
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
Public Class Pagamenti_Causali
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(ByVal piva As String,
                          ByVal cau_pagamento As String,
                          ByVal x_filtroAggiuntivo As String,
                          ByVal x_OrderBy As String,
                          ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim dal As New AgronicaCoreContabDAL.Pagamenti_Causali_R

            Dim Dt As DataTable = dal.Leggi(piva, cau_pagamento, x_filtroAggiuntivo, x_OrderBy, objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each dr As DataRow In Dt.Rows
                JArrayListaOp.Add(
                    New JObject(
                        New JProperty("Cau_Pagamento", dr("Cau_Pagamento")),
                        New JProperty("Cau_Pagamento_Sigla", dr("Cau_Pagamento_Sigla")),
                        New JProperty("Cau_Pagamento_Des", dr("Cau_Pagamento_Des"))))
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