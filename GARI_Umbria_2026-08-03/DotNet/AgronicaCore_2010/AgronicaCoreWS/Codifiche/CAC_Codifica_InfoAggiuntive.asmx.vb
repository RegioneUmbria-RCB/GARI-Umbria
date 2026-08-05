Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.anagrafiche

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class CAC_Codifica_InfoAggiuntive
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCAC_Codifica_InfoAggiuntive(objP_server As String,
                                        Argomento_Cod As Integer,
                                        InfoAgg_Cod As String,
                                        Tipo_Codifica As Integer,
                                        StringaCerca As String,
                                        FiltroAggiuntivo As String, Ordinamento As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim Dt As DataTable
            'Recupero il datatable
            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

            Dt = objCAC.Leggi(Argomento_Cod, InfoAgg_Cod, Tipo_Codifica, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, FiltroAggiuntivo, Ordinamento, objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("InfoAgg_Cod", dr.Item("InfoAgg_Cod")),
                                            New JProperty("InfoAgg_Des", dr.Item("InfoAgg_Des")),
                                            New JProperty("CodiceAux_1", dr.Item("CodiceAux_1")),
                                            New JProperty("CodiceAux_2", dr.Item("CodiceAux_2")),
                                            New JProperty("CodiceAux_3", dr.Item("CodiceAux_3"))))
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
    Public Function LeggiDettaglioVarietaPersonalizzato(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato))

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

            Dim key = "LeggiDettaglioVarietaPersonalizzato" + "_" + objParametri_Server.PivaSuperUser
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If


            Dim Dt As DataTable
            'Recupero il datatable
            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

            Dt = objCAC.Leggi(2, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato(dr.Item("InfoAgg_Cod")) With {
                                .descrizione = dr.Item("InfoAgg_Des")
                             }).ToList

            'cache.Insert(key, listItems)

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCapitolatoPrivato(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

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

            Dim key = "LeggiCapitolatoPrivato_" + objParametri_Server.PivaSuperUser
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If


            Dim Dt As DataTable
            'Recupero il datatable
            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

            Dt = objCAC.Leggi(1, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(dr.Item("InfoAgg_Cod"), dr.Item("InfoAgg_Des"))).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiResiduiDisponibili(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

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

            Dim key = "LeggiResiduiDisponibili_" + objParametri_Server.PivaSuperUser
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If


            Dim Dt As DataTable
            Dim val_cod As String
            'Recupero il datatable
            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

            Dt = objCAC.Leggi(7, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(dr.Item("InfoAgg_Cod"), dr.Item("InfoAgg_Des"))).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function LeggiCertCommDisponibili(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))
    '
    'Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))
    '
    'If InData.objP.objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    'Return r
    'End If
    '
    '
    'Try
    '
    'Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
    '
    'Dim cache As Cache
    'If HttpContext.Current.Cache IsNot Nothing Then
    '           cache = HttpContext.Current.Cache
    'End If
    '
    'Dim key = "LeggiCertCommDisponibili_" + objParametri_Server.PivaSuperUser
    'If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
    '           r.RispostaStringa = cache.Item(key)
    '            r.RispostaOK = True
    'Return r
    'End If
    '
    '
    'Dim Dt As DataTable
    'Dim val_cod As String
    ''Recupero il datatable
    'Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
    '
    '        Dt = objCAC.Leggi(8, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
    '
    'Dim listItems = (From dr In Dt.Rows
    'Select Case New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(dr.Item("InfoAgg_Cod"), dr.Item("InfoAgg_Des"))).ToList
    '
    '        r.RispostaStringa = listItems
    '        r.RispostaOK = True
    '
    'Catch ex As Exception
    '       'uso questa funzione per ottenere il Messaggio..:
    '       r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
    'End Try
    '
    'Return r
    '
    'End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCertProdDisponibili(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

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

            Dim key = "LeggiCertProdDisponibili_" + objParametri_Server.PivaSuperUser
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If


            Dim Dt As DataTable
            Dim val_cod As String
            'Recupero il datatable
            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

            Dt = objCAC.Leggi(8, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(dr.Item("InfoAgg_Cod"), dr.Item("InfoAgg_Des"))).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiPianiSemina(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

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

            Dim key = "LeggiCapitolatoPrivato_" + objParametri_Server.PivaSuperUser
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If


            Dim Dt As DataTable
            'Recupero il datatable
            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

            Dt = objCAC.Leggi(5, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(dr.Item("InfoAgg_Cod"), dr.Item("InfoAgg_Des"))).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#Region "COMBO MODIFICA MULTIPLA"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_CertificazioniProdotto(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
            Dim DT As DataTable = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Certificazione_Prodotto, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim jArray As New JArray()
            For Each dr As DataRow In DT.Rows
                jArray.Add(New JObject(New JProperty("text", dr.Item("InfoAgg_Des")),
                                                   New JProperty("value", dr.Item("InfoAgg_Cod"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArray, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Residui(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
            Dim DT As DataTable = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Residuo, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim jArray As New JArray()
            For Each dr As DataRow In DT.Rows
                jArray.Add(New JObject(New JProperty("text", dr.Item("InfoAgg_Des")),
                                                   New JProperty("value", dr.Item("InfoAgg_Cod"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArray, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_PianiSemina(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
            Dim DT As DataTable = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Piano_Semina, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim jArray As New JArray()
            For Each dr As DataRow In DT.Rows
                jArray.Add(New JObject(New JProperty("text", dr.Item("InfoAgg_Des")),
                                                   New JProperty("value", dr.Item("InfoAgg_Cod"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArray, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCAC_Codifica_InfoAggiuntive_NG(ByVal InData As CoreWS_Generic(Of LeggiCAC_Codifica_InfoAggiuntive)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim Dt As DataTable
            'Recupero il datatable
            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

            Dt = objCAC.Leggi(InData.InData.Argomento_Cod, InData.InData.InfoAgg_Cod, InData.InData.Tipo_Codifica, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(dr.Item("InfoAgg_Cod"), dr.Item("InfoAgg_Des"))).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region

End Class