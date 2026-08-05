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
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.baseClass


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Categorie_Magazzino
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Categorie_Magazzino_NG(ByVal InData As CoreWS_Generic(Of Leggi_Categorie_Magazzino)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objCategorie_Magazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim Dt = objCategorie_Magazzino.Leggi(InData.InData.Elem_Cod, InData.InData.Cau_Mov, InData.InData.Flag_Cantina, "", "", objParametri_Server)

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

    '#########################################################################################
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Categorie_Magazzino(ByVal objP_server As String, ByVal objP_utenti As String, ByVal Elem_Cod As Integer,
                                              ByVal Cau_Mov As String, ByVal Flag_Cantina As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)


            Dim objCategorie_Magazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim Dt = objCategorie_Magazzino.Leggi(Elem_Cod, Cau_Mov, Flag_Cantina, "", "", objParametri_Server)

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

    '#########################################################################################
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CategorieMagazzino_NG(ByVal InData As CoreWS_Generic(Of CategorieMagazzino)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim ddl As New DropDownList

            'x fare in modo di scartare i coadiuvanti (vecchia categoria)
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Date.Today, Date.Today)
            'xOrderBy non viene utilizzato
            AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(
               ddl,
               False, "", "",
               InData.InData.Elem_Cod,
               InData.InData.Cau_Mov,
               InData.InData.Lav_Cod,
               InData.InData.Flag_NoSemilavorati,
               InData.InData.Flag_AltriBeni,
               InData.InData.xFiltroAggiuntivo,
               "",
               objParametri_Server
            )
            objParametri_Server.ResettaFinestra()

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                JArrayListaOp.Add(New JObject(New JProperty("elem_des", i.Text), New JProperty("elem_cod", i.Value)))
            Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CategorieMagazzino(ByVal objP_server As String,
                                     ByVal Elem_Cod As Integer,
                                        ByVal Cau_Mov As String,
                                        ByVal Lav_Cod As Integer,
                                        ByVal Flag_NoSemilavorati As Boolean,
                                        ByVal Flag_AltriBeni As Boolean,
                                        ByVal xFiltroAggiuntivo As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'x fare in modo di scartare i coadiuvanti (vecchia categoria)
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Date.Today, Date.Today)

            Dim ddl As New DropDownList

            'xOrderBy non viene utilizzato
            AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(ddl,
                                                                     False, "", "",
                                                                     Elem_Cod,
                                                                     Cau_Mov,
                                                                     Lav_Cod,
                                                                     Flag_NoSemilavorati,
                                                                     Flag_AltriBeni,
                                                                    xFiltroAggiuntivo,
                                                                    "",
                                                                    objParametri_Server)

            objParametri_Server.ResettaFinestra()

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                JArrayListaOp.Add(New JObject(New JProperty("elem_des", i.Text), New JProperty("elem_cod", i.Value)))
            Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
            Return r
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Carica le categorie magazzino da database con possibilità di filtrare solo quelle interessate.
    ''' <br></br>
    ''' Comportamento default: carica tutte le categorie.
    ''' </summary>
    ''' <param name="data">Un Object contenente una stringa con il criterio di filtraggio</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns>Una RispostaStandard con una lista delle categorie interessate in RispostaStringa.</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    <Obsolete("Function is deprecated. Use Leggi_Generico_Categorie_Magazzino_NG instead.")>
    Public Function Leggi_Generico_Categorie_Magazzino(data As String,
                                                       ByVal objParametri_Server As String,
                                                       ByVal objParametri_Utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Utenti)

        Dim objCatMagazzino As New AgronicaCoreMetaSchemaBIZ.Categorie_Magazzino
        Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

        Try
            Dim filtro As String = JsonConvert.DeserializeObject(data)
            Dim categorie = objCatMagazzino.Leggi_CategorieMagazzino(filtro, obj_Server, obj_Utenti)


            r.RispostaStringa = JsonConvert.SerializeObject(categorie)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
            Return r
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Carica le categorie magazzino da database con possibilità di filtrare solo quelle interessate.
    ''' <br></br>
    ''' Comportamento default: carica tutte le categorie.
    ''' </summary>
    ''' <param name="InData">Un CoreWS_Generic di LeggiGenericoCategorieMagazzino, classe contenente una stringa con il criterio di filtraggio e gli objParametri.</param>
    ''' <returns>Una RispostaStandard con una lista delle categorie interessate in RispostaStringa.</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Generico_Categorie_Magazzino_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiGenericoCategorieMagazzino)) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Dim objCatMagazzino As New AgronicaCoreMetaSchemaBIZ.Categorie_Magazzino
        Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

        Try
            Dim filtro As String = InData.InData.Filtro
            Dim categorie = objCatMagazzino.Leggi_CategorieMagazzino(filtro, obj_Server, obj_Utenti)
            If (InData.InData.Simple) Then
                categorie = categorie.Select(Function(c) New BaseCodeDescr(c.Elem_Cod, c.NomeComune))
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(categorie)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Categorie_Magazzino_Impostazioni_NG(ByVal InData As CoreWS_Generic(Of LeggiCategorieMagazzinoImpostazioni)) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Dim objCatMagazzino As New AgronicaCoreMetaSchemaBIZ.Categorie_Magazzino

        Try
            Dim elemCodFilter = InData.InData.filter
            Dim setting = InData.InData.Elem_Cod
            Dim storageCategories = objCatMagazzino.Leggi_CategorieMagazzino(elemCodFilter, obj_Server, obj_Utenti)

            Dim setValues = objCatMagazzino.LeggiImpostazioniCategorieMagazzinoImpreseSU(
                setting, InData.InData.Piva, InData.InData.Sa_Cod,
                obj_Server, obj_Utenti
            )

            Dim listitem As New List(Of Object)
            For Each catVal In setValues
                Dim cat = storageCategories.FirstOrDefault(Function(c) c.Elem_Cod = catVal.Elem_Cod)
                If cat IsNot Nothing Then
                    listitem.Add(New With {
                        .NomeComune = cat.NomeComune,
                        .Elem_Cod = catVal.Elem_Cod,
                        .radioValue = catVal.radioValue
                    })
                End if
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(listItem)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Categorie_Magazzino_Impostazioni(filter As String, Elem_Cod As Integer,
                                                   ByVal objParametri_Server As String,
                                                   ByVal objParametri_Utenti As String) As RispostaStandard
        Dim r As New RispostaStandard()

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Utenti)

        Dim objCatMagazzino As New AgronicaCoreMetaSchemaBIZ.Categorie_Magazzino

        Try
            Dim listItem As New List(Of Object)
            Dim Categorie = objCatMagazzino.Leggi_CategorieMagazzino(filter, obj_Server, obj_Utenti)
            Dim CategoriexValori = objCatMagazzino.Leggi_CategorieMagazzino_Impostazioni_CentroAziendaSuperuser(
                                                Elem_Cod, obj_Server, obj_Utenti
                                            )
            Dim cat As List(Of Object)
            For Each catVal In CategoriexValori.values
                cat = Categorie.Where(Function(c)
                                          Return If(c.Elem_Cod = catVal.Elem_Cod, True, False)
                                      End Function).ToList()
                If cat.Count <> 0 Then
                    listItem.Add(New With {
                        .NomeComune = cat.First().NomeComune,
                        .Elem_Cod = catVal.Elem_Cod,
                        .radioValue = catVal.radioValue
                    })
                End If

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(listItem)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
            Return r
        End Try

        Return r

    End Function

End Class
