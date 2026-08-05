Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp
' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class FormeGiuridiche
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim FG_cod As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objFormeGiuridiche As New AgronicaCoreMetaSchemaDAL.FormeGiuridiche_R
            Dim dtFormeGiuridiche = objFormeGiuridiche.Leggi(FG_cod, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(dtFormeGiuridiche, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(FG_cod As Integer, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objFormeGiuridiche As New AgronicaCoreMetaSchemaDAL.FormeGiuridiche_R
            Dim dtFormeGiuridiche = objFormeGiuridiche.Leggi(FG_cod, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(dtFormeGiuridiche, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r
    End Function
    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function Leggi_Modello(FG_cod As Integer, ByVal objP_server As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.FormeGiuridiche))
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Modello_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiModello)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.FormeGiuridiche))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.FormeGiuridiche))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = "FormeGiuridicheModello_" & InData.InData.FG_cod
            Dim els As List(Of AgronicaCoreModelsSTD.metaschema.FormeGiuridiche)

            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If
            Dim objFormeGiuridiche As New AgronicaCoreMetaSchemaDAL.FormeGiuridiche_R
            Dim dtFormeGiuridiche As DataTable = objFormeGiuridiche.Leggi(InData.InData.FG_cod, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            els = (From row In dtFormeGiuridiche.Rows Select New AgronicaCoreModelsSTD.metaschema.FormeGiuridiche(row("FG_Cod")) With {.descrizione = row("FG_Des")}).ToList

            cache.Insert(key, els)

            r.RispostaStringa = els
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Modello(FG_cod As Integer, ByVal objP_server As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.FormeGiuridiche))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.FormeGiuridiche))

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = "FormeGiuridicheModello_" & FG_cod
            Dim els As List(Of AgronicaCoreModelsSTD.metaschema.FormeGiuridiche)

            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If
            Dim objFormeGiuridiche As New AgronicaCoreMetaSchemaDAL.FormeGiuridiche_R
            Dim dtFormeGiuridiche As DataTable = objFormeGiuridiche.Leggi(FG_cod, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            els = (From row In dtFormeGiuridiche.Rows Select New AgronicaCoreModelsSTD.metaschema.FormeGiuridiche(row("FG_Cod")) With {.descrizione = row("FG_Des")}).ToList

            cache.Insert(key, els)

            r.RispostaStringa = els
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r
    End Function

End Class