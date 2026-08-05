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
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDTOStd.InData.Metaschema


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class PUA_Regolamenti
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_PUA_Regolamenti_NG(InData As CoreWS_Generic(Of Leggi_PUA_Regolamenti)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)


            Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R

            Dim Dt = objReg.Leggi(InData.InData.Regolamento_Cod,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          InData.InData.xFiltroAggiuntivo, InData.InData.xOrderBy,
                          objParametri_Server)

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
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_PUA_Regolamenti(ByVal objP_server As String, ByVal objP_utenti As String,
                                          ByVal Regolamento_Cod As Long,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String
                                          ) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)


            Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R

            Dim Dt = objReg.Leggi(Regolamento_Cod,
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          xFiltroAggiuntivo, xOrderBy,
                          objParametri_Server)

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
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_PUA_RegolamentixEditImpianto_NG(InData As CoreWS_Generic(Of Leggi_PUA_Regolamenti)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            End If

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
            objParametriIngresso.strFiltro = InData.InData.xFiltroAggiuntivo
            objParametriIngresso.Tipo = TipiEnumerativi.enum_PUARegolamenti_Tipo.PianoComcimazione
            'objParametriIngresso.TipoMetodo = TipoMetodo
            objParametriIngresso.strOrdinamento = InData.InData.xOrderBy
            objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

            Dim JArrayLista As New JArray()
            For Each i As Regolamento In objParametriUscita.ListaRegolamenti
                JArrayLista.Add(New JObject(New JProperty("Reg_Cod", i.Codice), New JProperty("Reg_Des", i.Descrizione & " Dose Standard")))
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
    Public Function Leggi_PUA_RegolamentixEditImpianto(ByVal objP_super_server As String,
                                                       ByVal objP_server As String, ByVal objP_utenti As String,
                                          ByVal Regolamento_Cod As Long,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String
                                          ) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            End If

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
            objParametriIngresso.strFiltro = xFiltroAggiuntivo
            objParametriIngresso.Tipo = TipiEnumerativi.enum_PUARegolamenti_Tipo.PianoComcimazione
            'objParametriIngresso.TipoMetodo = TipoMetodo
            objParametriIngresso.strOrdinamento = xOrderBy
            objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

            Dim JArrayLista As New JArray()
            For Each i As Regolamento In objParametriUscita.ListaRegolamenti
                JArrayLista.Add(New JObject(New JProperty("Reg_Cod", i.Codice), New JProperty("Reg_Des", i.Descrizione & " Dose Standard")))
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
    Public Function Leggi_PUA_RegolamentixEditImpianto_Modello(InData As CoreWS_Generic(Of IntervalloTemporale)) As rispostaStandard(Of List(Of RegolamentoConcimazione))

        Dim r As New rispostaStandard(Of List(Of RegolamentoConcimazione))

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            End If

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input

            Dim filtro = ""
            If InData.InData.inizio <> AGRODATAINIZIO Or InData.InData.fine <> AGRODATAFINE Then
                filtro = " PUA_Regolamenti.Validita_inizio <= '" & InData.InData.fine.ToShortDateString & "' AND PUA_Regolamenti.Validita_Fine >= '" & InData.InData.inizio.ToShortDateString & "' "
            End If

            objParametriIngresso.strFiltro = filtro
            objParametriIngresso.Tipo = TipiEnumerativi.enum_PUARegolamenti_Tipo.PianoComcimazione
            'objParametriIngresso.TipoMetodo = TipoMetodo
            objParametriIngresso.strOrdinamento = "Regolamento_Des"
            objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

            Dim lista As New List(Of RegolamentoConcimazione)
            For Each i As Regolamento In objParametriUscita.ListaRegolamenti
                lista.Add(New RegolamentoConcimazione(i.Codice) With {.descrizione = i.Descrizione & " Dose Standard"})
            Next

            r.RispostaStringa = lista

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_PUA_RegolamentixImpostazioniNitrati(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_SuperServer As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim list = AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_WS(
                objParametri_Server, objParametri_SuperServer
            )
            r.RispostaStringa = JsonConvert.SerializeObject(list, Formatting.None)
            r.RispostaOK = True
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

End Class