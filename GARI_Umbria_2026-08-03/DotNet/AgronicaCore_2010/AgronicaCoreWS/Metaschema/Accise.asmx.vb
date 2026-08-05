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
Public Class Accise
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUnitaTrasporto(ByVal objP_server As String, ByVal codice As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim obj As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T009_TabellaCodiciUnitaDiTrasporto_R
            Dim dt As DataTable = obj.Leggi(codice, "", "", objParametriServer)

            Dim jArrayLista As New JArray()
            For Each dr In dt.Rows
                jArrayLista.Add(New JObject(New JProperty("UnitaTrasporto_Cod", dr.Item("Codice")),
                                            New JProperty("UnitaTrasporto_Des", dr.Item("Descrizione"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiModalitaTrasporto_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim codice As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim obj As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T008_TabellaCodiciModalitaDiTrasporto_R
            Dim dt As DataTable = obj.Leggi(codice, "", "", objParametriServer)

            Dim jArrayLista As New JArray()
            For Each dr In dt.Rows
                jArrayLista.Add(New JObject(New JProperty("ModalitaTrasporto_Cod", dr.Item("Codice")),
                                            New JProperty("ModalitaTrasporto_Des", dr.Item("Descrizione"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiModalitaTrasporto(ByVal objP_server As String, ByVal codice As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim obj As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T008_TabellaCodiciModalitaDiTrasporto_R
            Dim dt As DataTable = obj.Leggi(codice, "", "", objParametriServer)

            Dim jArrayLista As New JArray()
            For Each dr In dt.Rows
                jArrayLista.Add(New JObject(New JProperty("ModalitaTrasporto_Cod", dr.Item("Codice")),
                                            New JProperty("ModalitaTrasporto_Des", dr.Item("Descrizione"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
End Class