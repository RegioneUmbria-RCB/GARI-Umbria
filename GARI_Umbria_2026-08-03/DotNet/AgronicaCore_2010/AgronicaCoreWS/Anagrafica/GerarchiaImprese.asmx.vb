Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility
Imports AgronicaCoreDTOStd.InData.Anagrafica

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class GerarchiaImprese
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PadriGerarchia2(ByVal objP_server As String, ByVal PivaFiglio As String,
                                    ByVal Foglia As Integer, ByVal Livello As Integer,
                                    ByVal xFiltroAggiuntivo As String) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

            Dim objGer As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim dt As DataTable = objGer.LeggiPadriGerarchia("",
                                           PivaFiglio,
                                           Foglia,
                                           Livello,
                                           objParametriServer.FinestraTemporaleInizio,
                                           objParametriServer.FinestraTemporaleFine,
                                           xFiltroAggiuntivo,
                                           " RagSoc_Padre ",
                                            objParametriServer)

            Dim jArrayListaOp As New JArray()

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows
                    Dim ragSoc As String = CStr(dr.Item("RagSoc_Padre"))

                    jArrayListaOp.Add(New JObject(New JProperty("piva", dr.Item("padre")),
                                              New JProperty("ragsoc_padre", ragSoc.Replace("""", "'"))))
                Next

            Else
                Dim ragsoc_superuser As String = ""
                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                ragsoc_superuser = objImp.RagSoc_from_Piva(objParametriServer.PivaSuperUser, objParametriServer)

                jArrayListaOp.Add(New JObject(New JProperty("piva", objParametriServer.PivaSuperUser),
                                            New JProperty("ragsoc_padre", ragsoc_superuser)))
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "PadriGerarchia, Errore: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PadriGerarchia2_NG(ByVal InData As CoreWS_Generic(Of PadriGerarchia2)) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objGer As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim dt As DataTable = objGer.LeggiPadriGerarchia("",
                                           InData.InData.PivaFiglio,
                                            InData.InData.Foglia,
                                            InData.InData.Livello,
                                           objParametriServer.FinestraTemporaleInizio,
                                           objParametriServer.FinestraTemporaleFine,
                                            InData.InData.xFiltroAggiuntivo,
                                           " RagSoc_Padre ",
                                            objParametriServer)

            Dim jArrayListaOp As New JArray()

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows
                    Dim ragSoc As String = CStr(dr.Item("RagSoc_Padre"))

                    jArrayListaOp.Add(New JObject(New JProperty("piva", dr.Item("padre")),
                                              New JProperty("ragsoc_padre", ragSoc.Replace("""", "'"))))
                Next

            Else
                Dim ragsoc_superuser As String = ""
                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                ragsoc_superuser = objImp.RagSoc_from_Piva(objParametriServer.PivaSuperUser, objParametriServer)

                jArrayListaOp.Add(New JObject(New JProperty("piva", objParametriServer.PivaSuperUser),
                                            New JProperty("ragsoc_padre", ragsoc_superuser)))
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "PadriGerarchia, Errore: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PadriGerarchia(ByVal objP_server As String) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim ClassJoin As New JoinFiltrone
            Dim classFiltrone As New AgronicaCoreUtility.Filtrone

            ClassJoin.bGerarchiaImprese = True

            'End If

            classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente("", ClassJoin)

            Dim Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametriServer,
                                                    " TipoImpresaGerarchia<>1 ",
                                                    enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                    "",
                                                    ClassJoin)

            Dim jArrayListaOp As New JArray()

            If Not IsNothing(Dt_Imprese) AndAlso Dt_Imprese.Rows.Count > 0 Then

                For Each dr As DataRow In Dt_Imprese.Rows
                    Dim ragSoc As String = CStr(dr.Item("rag_soc"))

                    jArrayListaOp.Add(New JObject(New JProperty("piva", dr.Item("piva")),
                                              New JProperty("ragsoc_padre", ragSoc.Replace("""", "'"))))
                Next

            Else
                Dim ragsoc_superuser As String = ""
                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                ragsoc_superuser = objImp.RagSoc_from_Piva(objParametriServer.PivaSuperUser, objParametriServer)

                jArrayListaOp.Add(New JObject(New JProperty("padre", objParametriServer.PivaSuperUser),
                                            New JProperty("ragsoc_padre", ragsoc_superuser)))
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "PadriGerarchia, Errore: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function






End Class