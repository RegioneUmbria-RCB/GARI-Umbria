Imports System.Web.Services
Imports System.Web.Services.Protocols
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
Public Class CategorieXUnitaMisura
    Inherits System.Web.Services.WebService


    ''' <summary>
    ''' Letture delle categorie per unità misura, se chiamata da Gias APP impostare elem_cod = -100
    ''' </summary>
    ''' <param name="objP_server"></param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Udm_Cod"></param>
    ''' <param name="Cau_Mov"></param>
    ''' <param name="Flag_Cantina"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_CategorieXUnitaMisura(ByVal objP_server As String, ByVal objP_utenti As String, ByVal Elem_Cod As Integer,
                            ByVal Udm_Cod As Integer, ByVal Cau_Mov As String, ByVal Flag_Cantina As Boolean, ByVal xFiltroAggiuntivo As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)


            Dim Dt As DataTable


            If Elem_Cod = -100 Then
                Dt = UdmXAPP(objParametri_Server)
            Else

                Dim objCategorieXUnitaMisura As New AgronicaCoreMetaSchemaDAL.CategorieXUnitaMisura_R
                Dt = objCategorieXUnitaMisura.Leggi(Elem_Cod, Udm_Cod, Cau_Mov, Flag_Cantina, enumSelezioneVariabile.Selezione_JoinDescrizioni, xFiltroAggiuntivo, "", objParametri_Server)

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Private Function UdmXAPP(objParametri As AgronicaCoreParametri) As DataTable

        Dim leggiudm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim dtUdm As DataTable =
            leggiudm.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        Dim dt As DataTable = UdmXAPP_GeneraDT()

        For Each cUdm In dtUdm.Rows
            Dim tipoControlloCod As Integer = 0
            If Not IsDBNull(cUdm("tipocontrollo_cod")) Then
                tipoControlloCod = cUdm("tipocontrollo_cod")
            End If
            UdmXAPP_AggiungiRighe(-1, cUdm("udm_cod"), "", cUdm("udm_des"), cUdm("udm_sim"), tipoControlloCod, dt)
        Next

        Return dt

    End Function


    Private Sub UdmXAPP_AggiungiRighe(Elem_cod As Integer, Udm_cod As Integer, NomeComune As String, udm_des As String, udm_sim As String, tipocontrollo_cod As Integer, dt As DataTable)

        Dim dr As DataRow = dt.NewRow()
        dr("Elem_cod") = Elem_cod
        dr("Udm_cod") = Udm_cod
        dr("NomeComune") = NomeComune
        dr("udm_des") = udm_des
        dr("udm_sim") = udm_sim
        dr("TipoControllo_Cod") = tipocontrollo_cod
        dt.Rows.Add(dr)

    End Sub


    Private Function UdmXAPP_GeneraDT() As DataTable
        Dim dt As New DataTable
        dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("NomeComune", GetType(String)))
        dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        dt.Columns.Add(New DataColumn("Udm_Sim", GetType(String)))
        dt.Columns.Add(New DataColumn("TipoControllo_Cod", GetType(Integer)))
        Return dt
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_UnitaMisura_Categoria_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiUnitaMisuraCategoria)) As rispostaStandard(Of Object)
        Dim res As New rispostaStandard(Of Object)

        Dim objCatxUDM As New AgronicaCoreMetaSchemaBIZ.CategoriexUnitaMisura
        Dim listItem As New List(Of Object)

        Try
            Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim Elem_Cod = InData.InData.Elem_Cod

            Dim UDMList = objCatxUDM.Leggi_CategoriaxUnitaMisura(Elem_Cod, obj_Server, obj_Utenti)

            res.RispostaStringa = UDMList
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

End Class