Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Imprese_Impostazioni_R
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_Impostazioni_APP(ByVal piva As String,
                                         ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim dtImpostazioni As New DataTable

        Try

            Dim listaFiltroAggiuntivo As New List(Of String)

            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVA_RICETTA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_INTERVENTI_DA_FARE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_SCARICO_ORE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_MAX_AZIENDE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_ENTRATAUSCITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_LAMIAPOSIZIONE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_VISITE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_FREQUENZARILIEVO_MINUTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_FREQUENZASINCRO_MINUTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_DOCUMENTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_RILIEVI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_GIS)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_InCab)

            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_ERBE_INFESTANTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.Raccolta_Con_Carico_Magazzino)

            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE)

            Dim stbFiltro As New StringBuilder

            stbFiltro.Append(" Impostazione_Cod IN (")
            stbFiltro.Append(String.Join(",", listaFiltroAggiuntivo))
            stbFiltro.Append(")")

            Dim xFiltroAggiuntivo = stbFiltro.ToString

            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
            Dim dtImpreseImpostazioni As DataTable = objImpreseImpostazioni.Leggi(piva,
                                                                 CostantiPersonalizzate.SACOD_NOFILTRO,
                                                                 0,
                                                                 xFiltroAggiuntivo,
                                                                 "",
                                                                 objParametri_Server)

            If dtImpreseImpostazioni IsNot Nothing AndAlso dtImpreseImpostazioni.Rows.Count > 0 Then

                Dim view As New DataView(dtImpreseImpostazioni)
                dtImpostazioni = view.ToTable(False, "Piva", "Sa_Cod", "Impostazione_Cod", "Impostazione_Valore")

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtImpostazioni, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiSezioniImpostazioni_AziendeCentri(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_R
            Dim settingsList = objSettings.CaricaSezioni_Impostazioni_Aziende_Centri(objParametri_Utenti)
            res.RispostaStringa = JsonConvert.SerializeObject(settingsList, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiValoriImpostazioni_AziendeCentri_NG(InData As CoreWS_Generic(Of LeggiValoriImpostazioni_AziendeCentri)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim selezionati = InData.InData

            ''' Lista di oggetti {.Impostazione_Cod as Integer, .data as profilazione.ImpostazioneBase}
            Dim listItems = objSettings.CaricaImpostazioni_Aziende_Centri(selezionati.Impostazioni, objParametri_Utenti)

            If selezionati.Imprese.Count = 1 Then
                Dim azienda = selezionati.Imprese(0)
                listItems = objSettings.CaricaDatiImpostazioni_Aziende_Centri_NG(azienda, listItems,
                                                                     objParametri_Utenti, objParametri_Server)
            End If

            res.RispostaStringa = JsonConvert.SerializeObject(listItems, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpostazioneImpresaCentro(InData As CoreWS_Generic(Of LeggiValoriImpostazioni_AziendeCentri)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_R

            Dim listItems = objSettings.LeggiSoloImpostate(InData.InData.Imprese, InData.InData.Impostazioni, objParametri_Server)

            res.RispostaStringa = JsonConvert.SerializeObject(listItems, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

End Class