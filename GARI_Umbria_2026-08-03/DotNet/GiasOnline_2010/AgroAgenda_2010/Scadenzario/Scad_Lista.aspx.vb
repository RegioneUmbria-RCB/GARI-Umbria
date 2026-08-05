Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtentiDAL
Imports System.Web.Services
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Scad_Lista
    Inherits System.Web.UI.Page

    Public SincroDatiApp As Boolean = False

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Private Sub MenuBS_Agenda_Nuovo_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return Me.Master.PATH_GIASBASE
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Expires = 0

        Dim bPermessiOk As Boolean = True
        Dim Attivita_Cod As Integer
        Dim Permesso_Cod As Integer

        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'Anna 23/05/22: Aggiunto componente kendoUpload, per caricamento di allegati multipli per singolo upload
        Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "UploadMultiploAllegati_Documentale", "", "", objParametri_Server)
        Dim Autorizzato As String = dt_Conf.Rows(0).Item("Valore")
        hf_UploadMultiploAllegatiAbilitato.Value = CBool(Autorizzato)

        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("type")) Then
            hdModalita.Value = Request.QueryString.Item("type")
        Else
            hdModalita.Value = ""
        End If


        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("p")) Then
            hdPiva.Value = Request.QueryString.Item("p")
        Else
            hdPiva.Value = ""
        End If

        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("area_provenienza")) Then
            hdArea_Provenienza.Value = Request.QueryString.Item("area_provenienza")
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            hdArea_Provenienza.Value = 0
        End If

        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("richiesta_cod")) Then
            hdRichiesta_Cod.Value = Request.QueryString.Item("richiesta_cod")
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            hdRichiesta_Cod.Value = 0
        End If

        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("id_agenda")) Then
            hdIdAgenda.Value = Request.QueryString.Item("id_agenda")
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            hdIdAgenda.Value = 0
        End If

        If Not IsNothing(Request.QueryString.Item("analisi_testata_cod")) Then
            hdAnalisi_Testata_Cod.Value = Request.QueryString.Item("analisi_testata_cod")
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            hdAnalisi_Testata_Cod.Value = 0
        End If

        'DCA20260105 filtro per Mac_Cod
        If Not IsNothing(Request.QueryString.Item("Mac_Cod")) Then
            hdMac_Cod.Value = Request.QueryString.Item("Mac_Cod")
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            hdMac_Cod.Value = 0
        End If

        If Not IsNothing(Request.QueryString.Item("tipologia_provenienza")) Then
            hdTipologia_Provenienza.Value = Request.QueryString.Item("tipologia_provenienza")
        Else
            hdTipologia_Provenienza.Value = 0
        End If

        If Not IsNothing(Request.QueryString.Item("Ricetta_Operazione_Cod")) Then
            hdRicetta.Value = Request.QueryString.Item("Ricetta_Operazione_Cod")
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            hdRicetta.Value = 0
        End If

        If Not IsNothing(Request.QueryString.Item("Cod_Contatto")) Then
            hdCod_Contatto.Value = Request.QueryString.Item("Cod_Contatto")
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            hdCod_Contatto.Value = ""
        End If

        If Not IsNothing(Request.QueryString.Item("indici")) Then
            hdIndici.Value = Request.QueryString.Item("indici")
        Else
            hdIndici.Value = ""
        End If



        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("sito_provenienza")) Then
            hdSito_Provenienza.Value = Request.QueryString.Item("sito_provenienza")
        Else
            hdSito_Provenienza.Value = ""
        End If

        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("cod_Documenti")) Then
            hdxFiltroDocumenti.Value = Request.QueryString.Item("cod_Documenti")
        Else
            hdxFiltroDocumenti.Value = ""
        End If

        'Controllo se i parametri mi sono stati passati in querystring
        If Not IsNothing(Request.QueryString.Item("du")) Then
            hdDataUpload.Value = Request.QueryString.Item("du")
        Else
            hdDataUpload.Value = ""
        End If

        '============================================================================================================================
        'Controllo Permessi
        '----------------------------------------------------------------------------------------------------------------------------            
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

        'Inizializzazione
        hdAllegato_Permesso_Richiesta_Modifica.Value = True
        hdAllegato_Permesso_Approvazione_Richiesta_Modifica.Value = True
        hdAllegato_Permesso_Rendcontazione_Modifica.Value = True
        hdAllegato_Permesso_Approvazione_Rendcontazione_Modifica.Value = True
        hdAllegato_Permesso_Storicizzazione.Value = True

        Dim stato_Richiesta = True
        If hdRichiesta_Cod.Value <> "0" Then
            stato_Richiesta = testate.Leggi_Stato_Modificabile(hdPiva.Value, hdRichiesta_Cod.Value, False, "", "", objParametri_Server)
        End If
        Dim stato_Approvazione = True
        If hdRichiesta_Cod.Value <> "0" Then
            stato_Approvazione = testate.Leggi_Stato_Modificabile(hdPiva.Value, hdRichiesta_Cod.Value, True, "", "", objParametri_Server)
        End If

        Select Case UCase(hdModalita.Value)

            Case "" 'Scadenza

                If Master.flag_MostraHeader Then
                    Master.SetTitoloPagina(54)
                End If

                Attivita_Cod = enum_Security_Attivita.Scadenzario_Lista
                Permesso_Cod = enum_Security_Operazione.Lettura

                'Controllo se l'utente ha i permessi per accedere
                bPermessiOk = objPermessi.Controlla_Permessi_Utente(
                                  Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                  Attivita_Cod,
                                  Permesso_Cod,
                                  Date.Now, "", objParametri_Utenti)


                'Controllo Permessi Cancellazione 
                hdAllegato_Permesso_Cancellazione.Value = objPermessi.Controlla_Permessi_Utente(
                                                      Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                                      enum_Security_Attivita.Scadenzario_Canc,
                                                      enum_Security_Operazione.Modifica,
                                                      Date.Now, "", objParametri_Utenti)



            Case "DOC" 'Documento

                If Master.flag_MostraHeader AndAlso (hdRichiesta_Cod.Value = 0 OrElse hdAnalisi_Testata_Cod.Value = 0 OrElse hdIdAgenda.Value = 0) Then
                    Master.SetTitoloPagina(55)
                End If


                Attivita_Cod = enum_Security_Attivita.Documentale_Lista
                Permesso_Cod = enum_Security_Operazione.Lettura

                'Controllo se l'utente ha i permessi per accedere
                bPermessiOk = objPermessi.Controlla_Permessi_Utente(
                                  Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                  Attivita_Cod,
                                  Permesso_Cod,
                                  Date.Now, "", objParametri_Utenti)

                'Controllo Permessi Cancellazione 
                hdAllegato_Permesso_Cancellazione.Value = objPermessi.Controlla_Permessi_Utente(
                                                      Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                                      enum_Security_Attivita.Documentale_Canc,
                                                      enum_Security_Operazione.Modifica,
                                                      Date.Now, "", objParametri_Utenti) AndAlso (stato_Richiesta OrElse stato_Approvazione)






                'Controllo Permesso Richiesta UMA
                hdAllegato_Permesso_Richiesta_Modifica.Value = objPermessi.Controlla_Permessi_Utente(
                                                                    Session("ASG_Utente_Username"),
                                                                    Session("ASG_IdServizio"),
                                                                    enum_Security_Attivita.Richiesta_UMA,
                                                                    enum_Security_Operazione.Modifica,
                                                                    Date.Now,
                                                                    "",
                                                                    objParametri_Utenti) AndAlso stato_Richiesta


                'Controllo Permesso Richiesta UMA
                hdAllegato_Permesso_Approvazione_Richiesta_Modifica.Value = objPermessi.Controlla_Permessi_Utente(
                                                                    Session("ASG_Utente_Username"),
                                                                    Session("ASG_IdServizio"),
                                                                    enum_Security_Attivita.Approvazione_Richiesta_UMA,
                                                                    enum_Security_Operazione.Modifica,
                                                                    Date.Now,
                                                                    "",
                                                                    objParametri_Utenti) AndAlso stato_Approvazione





                'Controllo Permesso Rendicontazione UMA
                hdAllegato_Permesso_Rendcontazione_Modifica.Value = objPermessi.Controlla_Permessi_Utente(
                                                                    Session("ASG_Utente_Username"),
                                                                    Session("ASG_IdServizio"),
                                                                    enum_Security_Attivita.Rendicontazione_UMA,
                                                                    enum_Security_Operazione.Modifica,
                                                                    Date.Now,
                                                                    "",
                                                                    objParametri_Utenti) AndAlso stato_Richiesta

                'Controllo Permesso Rendicontazione UMA
                hdAllegato_Permesso_Approvazione_Rendcontazione_Modifica.Value = objPermessi.Controlla_Permessi_Utente(
                                                                    Session("ASG_Utente_Username"),
                                                                    Session("ASG_IdServizio"),
                                                                    enum_Security_Attivita.Approvazione_Rendicontazione_UMA,
                                                                    enum_Security_Operazione.Modifica,
                                                                    Date.Now,
                                                                    "",
                                                                    objParametri_Utenti) AndAlso stato_Approvazione


            Case Else 'Non riconosciuto

                bPermessiOk = False

        End Select

        'Controllo Permessi Allegato
        hdAllegato_Permesso.Value = objPermessi.Controlla_Permessi_Utente(
                      Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                      enum_Security_Attivita.Documentale_Lista,
                      enum_Security_Operazione.Lettura,
                      Date.Now, "", objParametri_Utenti)


        'Controllo Permessi Storicizzazione
        hdAllegato_Permesso_Storicizzazione.Value = objPermessi.Controlla_Permessi_Utente(
                      Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                      enum_Security_Attivita.Documentale_Storicizzazione,
                      enum_Security_Operazione.Lettura,
                      Date.Now, "", objParametri_Utenti)




FinePermessi:

        If Not bPermessiOk Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        'Controllo Visibilità Validazione
        hdAllegato_Validazione_Visibilita.Value = objPermessi.Controlla_Permessi_Utente(
                                                        objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
                                                        enum_Security_Attivita.Documentale_Valid, enum_Security_Operazione.Lettura,
                                                        Date.Now, "", objParametri_Utenti)

        'Controllo Modifica Validazione
        hdAllegato_Validazione.Value = objPermessi.Controlla_Permessi_Utente(
                                                        objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
                                                        enum_Security_Attivita.Documentale_Valid, enum_Security_Operazione.Modifica,
                                                        Date.Now, "", objParametri_Utenti)

        'Scadenza/ Lettura controllato nella master
        Dim UtenteAbilitatoModificaScadenza As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           Attivita_Cod,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoModificaScadenza AndAlso (hdRichiesta_Cod.Value = "0" OrElse
        hdAllegato_Permesso_Approvazione_Rendcontazione_Modifica.Value = True OrElse
                                            hdAllegato_Permesso_Approvazione_Richiesta_Modifica.Value = True OrElse hdAllegato_Permesso_Rendcontazione_Modifica.Value = True OrElse
                                            hdAllegato_Permesso_Richiesta_Modifica.Value = True)


        If hdRichiesta_Cod.Value <> 0 Then
            'Richiesta Uma Carburanti --> ritorno alla pagina di richiesta Uma
            hdPaginaRedirect.Value = "../CarburantiUMA/RichiestaCarburanti.aspx" & "?p=" & Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder, Server) & "&rc=" & Stringa_Codifica(hdRichiesta_Cod.Value, AgroKey_EncoderDecoder, Server)

        End If

        ' abilitazione pulsante carica dati app
        Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
        Dim impostazione = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_DOCUMENTI, objParametri_Utenti, 2)
        Dim permesso = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline, enum_Security_Attivita.GiasAPP_DOCUMENTI, enum_Security_Operazione.Lettura, Date.Now, "", objParametri_Utenti)
        hf_AbilitazioneDocumentiAPP.Value = permesso OrElse (Not String.IsNullOrEmpty(impostazione) AndAlso impostazione <> "0")

        ' nuova importazione dati app
        Dim objAppHelper As New AgronicaCoreModello.AppHelper
        Dim CaricaDatiApp As Boolean = objAppHelper.Leggi_CaricaDati_APP(SincroDatiApp, objParametri_Server)
        If Not CaricaDatiApp Then
            hf_AbilitazioneDocumentiAPP.Value = False
        End If

    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda()

        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineGiasOnline_2010.Menu,
                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            ElseIf sitoorigine = Enum_SiteRedirector.Sito_GiasOnline AndAlso paginaOnLineRitorno = enum_PagineGiasOnline.MenuCartellaAziendale Then

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Scadenzario_Lista
                link = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.MenuCartellaAziendale, objParametriAgenda)

            Else
                link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try

        Response.Redirect(link)
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    'legge parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiFiltri(ByVal versione As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOCUMENTALE, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If dtImpostazioni.Rows.Count > 0 Then
                Dim impostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))

                Dim versioneSalvata = impostazioni("_versione")
                If versioneSalvata Is Nothing OrElse versioneSalvata.ToString <> versione Then
                    scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOCUMENTALE, "", objParametri_Utenti)
                    r.RispostaStringa = ""
                Else
                    r.RispostaStringa = JsonConvert.SerializeObject(impostazioni, Formatting.None)
                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaFiltri(ByVal parametri As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim modificati As New JArray()
            Dim nuovi As New JArray()
            Dim tutti As JArray = Nothing
            Dim jsonDaSalvare = JsonConvert.DeserializeObject(parametri)
            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOCUMENTALE, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If dtImpostazioni.Rows.Count > 0 Then
                Dim salvato = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                Dim versioneSalvata = salvato("_versione")
                Dim versioneCorrente = jsonDaSalvare("_versione")

                If versioneSalvata Is Nothing OrElse versioneSalvata.ToString() <> versioneCorrente.ToString Then
                    scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOCUMENTALE, "", objParametri_Utenti)
                End If

                Dim griglieSalvate As JArray = salvato("_griglia")
                Dim griglieDaSalvare As JArray = jsonDaSalvare("_griglia")

                For Each daSalvare In griglieDaSalvare
                    If griglieSalvate.Any(Function(s) s("IdControllo").ToString() = daSalvare("IdControllo").ToString()) Then
                        modificati.Add(daSalvare)
                    Else
                        nuovi.Add(daSalvare)
                    End If
                Next
                tutti = New JArray(modificati.Union(nuovi))

                For Each salvato In griglieSalvate
                    If Not tutti.Any(Function(s) s("IdControllo").ToString() = salvato("IdControllo").ToString()) Then
                        tutti.Add(salvato)
                    End If
                Next
            Else
                tutti = jsonDaSalvare("_griglia")
            End If

            jsonDaSalvare("_griglia") = tutti
            Dim impostazioniReport = JsonConvert.SerializeObject(jsonDaSalvare, Formatting.None)
            r.RispostaOK = scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOCUMENTALE, "", objParametri_Utenti)
            r.RispostaOK = scriviImpostazioni.Scrivi(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOCUMENTALE, impostazioniReport, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "true"
            Else
                r.RispostaStringa = "Problema nel salvataggio del report"
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GestionePassaggioDiStato(lista_allegati As String) As RispostaStandard
        Dim r As New RispostaStandard
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim arrAllegati = JArray.Parse(lista_allegati)
        Dim objAllegati As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Dim listAllegati As New List(Of Integer)
        Dim listaPratiche As String = ""

        For Each j In arrAllegati.ToList
            listAllegati.Add(CInt(j))
        Next

        Dim dt = objAllegati.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                   " Allegati_Documenti_Cod " & QueryBuilderUtility.GeneraClausolaINDaList(listAllegati) & " ",
                                   "", objParametri_Server)
        Dim pratichePresenti As Boolean = True

        For Each row In dt.Rows
            If IsDBNull(row.Item("Servizio_Cod")) Then
                pratichePresenti = False
            End If
        Next

        If pratichePresenti Then

            Dim servizio_Cod As Integer = dt.Rows(0).Item("Servizio_Cod")
            Dim stato_Cod As Integer = dt.Rows(0).Item("Stato_Cod")
            Dim valido As Boolean = True

            For Each row In dt.Rows

                listaPratiche &= IIf(listaPratiche.Length > 0, "_", "") & (CStr(row.Item("Pratica_Cod")))

                If servizio_Cod <> row.Item("Servizio_Cod") OrElse stato_Cod <> row.Item("Stato_Cod") Then
                    valido = False
                    Exit For
                End If

            Next

            If valido Then
                Try

                    Dim url As String = ""
                    url = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkPassaggioDiStato(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, listaPratiche, 0)

                    r.RispostaOK = True
                    r.RispostaStringa = url

                Catch ex As Exception

                    r.RispostaOK = False
                    r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False)

                End Try
            Else
                r.RispostaOK = False
                r.RispostaStringa = ""
                r.Errore = "I documenti selezionati non sono tutti nello stesso stato o non appartengono alla stessa categoria"
            End If

        Else

            r.RispostaOK = False
            r.RispostaStringa = ""
            r.Errore = "Per uno o più documenti manca la pratica associata"

        End If

        Return r
    End Function


End Class