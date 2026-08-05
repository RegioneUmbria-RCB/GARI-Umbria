Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Nazioni
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim dtNazioni As DataTable = Nothing

            Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            dtNazioni = objNazioni.Leggi("", "", "", objParametriServer)

            Dim lista = (From d In dtNazioni.AsEnumerable() Select New With
                                                                     {
                                                                        .Codice = d.Item("Codice").ToString(),
                                                                        .Descrizione = d.Item("Descrizione").ToString(),
                                                                        .EntrateUnico = d.Field(Of Integer)("Entrate_Unico_Elenco_paesi_territori_esteri_COD")
                                                                     }).ToList()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(lista, Formatting.None)


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


End Class