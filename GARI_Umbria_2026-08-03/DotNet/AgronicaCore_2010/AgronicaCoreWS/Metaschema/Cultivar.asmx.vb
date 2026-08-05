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
Imports AgronicaCoreDTOStd.InData.Metaschema


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Cultivar
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCultivar_conFiltroUtente_NG(ByVal InData As CoreWS_Generic(Of CaricaComboCultivar_conFiltroUtente)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objCom As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim Dt As DataTable = objCom.GestioneFiltroUtente_Leggi(0,
                                                                    InData.InData.Veg_Cod,
                                                                         InData.InData.StringaCerca,
                                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                     InData.InData.FiltroAggiuntivo,
                                                                         InData.InData.Ordinamento,
                                                                        objParametri_Utenti)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("cul_cod", dr.Item("CUL_COD")), New JProperty("cul_des", dr.Item("CUL_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCultivar_conFiltroUtente(objP_utenti As String,
                                                        Veg_Cod As Integer,
                                                        LetteraIniziale As String, StringaCerca As String,
                                                        FiltroAggiuntivo As String, Ordinamento As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objCom As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim Dt As DataTable = objCom.GestioneFiltroUtente_Leggi(0,
                                                                    Veg_Cod,
                                                                        StringaCerca,
                                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                    FiltroAggiuntivo,
                                                                        Ordinamento,
                                                                        objParametri_Utenti)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("cul_cod", dr.Item("CUL_COD")), New JProperty("cul_des", dr.Item("CUL_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Function GetVarietaAGEA_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim Veg_Cod_Agea As String = InData.InData

        Try


            Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

            Dim dt As DataTable = objR.Leggi(Veg_Cod_Agea, "", 0, 0, 0, AGRODATAINIZIO, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)


            Dim lista As New List(Of String)

            For Each dr As DataRow In dt.Rows
                lista.Add("{""varieta"":""" & jSon.Escape(dr.Item("cul_des_agea")) & """, ""cul_cod_agea"":""" & dr.Item("cul_cod_agea") & """}")
            Next


            Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"


            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Function GetVarietaAGEA(ByVal Veg_Cod_Agea As String) As RispostaStandard

        Dim r As New RispostaStandard


        Try


            Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

            Dim dt As DataTable = objR.Leggi(Veg_Cod_Agea, "", 0, 0, 0, AGRODATAINIZIO, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)


            Dim lista As New List(Of String)

            For Each dr As DataRow In dt.Rows
                lista.Add("{""varieta"":""" & jSon.Escape(dr.Item("cul_des_agea")) & """, ""cul_cod_agea"":""" & dr.Item("cul_cod_agea") & """}")
            Next


            Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"


            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function LeggiFiltroUtente(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiCultivar)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta))

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = "CultivarModello_" & InData.InData.specie.codice & "_" & objParametri_Utenti.PivaSuperUser & "_" & objParametri_Utenti.UtenteUsername & "_" & objParametri_Utenti.Recupera_NomeDB
            Dim listCultivar As List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)

            If InData.InData.cache AndAlso cache IsNot Nothing AndAlso cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If

            ' filtro specie vegetali x app
            Dim xFiltroAggiuntivo As String = ""
            Dim listaSpecie = InData.InData.listaSpecie
            If listaSpecie IsNot Nothing AndAlso listaSpecie.Count > 0 Then
                xFiltroAggiuntivo &= " Cultivar.Veg_Cod IN (" & String.Join(",", listaSpecie) & ") "
            End If

            Dim objCom As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim Dt As DataTable = objCom.GestioneFiltroUtente_Leggi(0, InData.InData.specie.codice, "",
                                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                    xFiltroAggiuntivo, "", objParametri_Utenti)

            'Dim JArrayLista As New JArray()
            'For Each dr In Dt.Rows
            '    JArrayLista.Add(New JObject(New JProperty("cul_cod", dr.Item("CUL_COD")), New JProperty("cul_des", dr.Item("CUL_DES"))))
            'Next

            listCultivar = (From dr In Dt.Rows Select New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(dr("Cul_Cod")) With {
                .descrizione = dr("Cul_Des"),
                .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(dr("Veg_Cod")) With {
                                                   .descrizione = dr("Veg_Des")
                }
            }).ToList

            cache.Insert(key, listCultivar)

            r.RispostaStringa = listCultivar
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiCultivar)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta))

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = "CultivarModello_" & InData.InData.specie.codice & "_" & objParametri_Utenti.PivaSuperUser & "_" & objParametri_Utenti.UtenteUsername & "_" & objParametri_Utenti.Recupera_NomeDB
            Dim listCultivar As List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)

            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If


            Dim objCom As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim Dt As DataTable = objCom.GestioneFiltroUtente_Leggi(0, InData.InData.specie.codice, "",
                                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                    "", "", objParametri_Utenti)

            listCultivar = (From dr In Dt.Rows Select New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(dr("Cul_Cod")) With {
                .descrizione = dr("Cul_Des"),
                .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(dr("Veg_Cod")) With {
                                                   .descrizione = dr("Veg_Des")
                }
            }).ToList

            cache.Insert(key, listCultivar)

            r.RispostaStringa = listCultivar
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class