Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreUtentiDAL

Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreModelsSTD.exceptions

Public Class Modifica_Multipla_PianoColturale
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Public FiltroneImpostato As Boolean
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametriAgenda = New ParametriAgenda

        inizializzoObjParametri()
        inizializzoParametriPagina()


        If Request.QueryString("f") = "ng" Then
            'Quando vengo dal pulsantye in griglia Angular 'Modifica Multipla' non ho bisogno di controllare i permessi, basta quello di edit appezzamento
            hidden_ModalitaMonoAzienda.Value = "true"
        Else

            'Controllo se l'utente ha i permessi per accedere
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                Session("ASG_Utente_Username"),
                                                Session("ASG_IdServizio"),
                                                enum_Security_Attivita.Anagrafica_MultiModifica_PianoColturale,
                                                enum_Security_Operazione.Lettura,
                                                Date.Now,
                                                "",
                                                objParametri_Utenti)

            Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                               Session("ASG_Utente_Username"),
                                               Session("ASG_IdServizio"),
                                               enum_Security_Attivita.Anagrafica_MultiModifica_PianoColturale,
                                               enum_Security_Operazione.Modifica,
                                               Date.Now,
                                               "",
                                               objParametri_Utenti)

            If UtenteAbilitatoLettura = False Then
                Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            End If

            If UtenteAbilitatoScrittura = False Then

            End If
        End If


        'Impostazione SuperUser x ereditatore
        Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
        Dim modificaMultipla As String

        If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
            modificaMultipla = "2"
        Else
            modificaMultipla = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Ereditatore, objParametri_Utenti, 2)  'to do...
        End If

        Dim numeroAziende As Integer = (From ii In objParametriAgenda.Impianti Distinct Select ii.Piva).Distinct.Count
        Dim numeroImpianti As Integer = objParametriAgenda.Impianti.Count

        Dim usaFiltroRicercaNG As Boolean
        If Not Page.IsPostBack Then
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            usaFiltroRicercaNG = objFiltroRicerca.usaFiltroRicercaNG(objParametri_Utenti)
            hd_usaFiltroRicercaNG.Value = usaFiltroRicercaNG
            hd_Piva.Value = objParametriAgenda.Piva
        End If

        If usaFiltroRicercaNG Then
            FiltroneImpostato = numeroImpianti > 0
        Else
            FiltroneImpostato = (Not String.IsNullOrEmpty(Request.QueryString("f")))
        End If

        Dim IDTestataTemp As Integer = 0
        Dim Modifica_Multipla_PianoColturale As String = ""

        If FiltroneImpostato Then

            If numeroImpianti > 0 Then
                Dim chiavi = From i In objParametriAgenda.Impianti Distinct
                             Select i.Piva & "_" & i.Sa_Cod & "_" & i.Appezza & "_" & i.ID_Reg & "_" & i.Veg_Cod & "_" & i.Progetto_Cod

                Dim progetti As List(Of Integer) = (From i In objParametriAgenda.Impianti Select i.Progetto_Cod).Distinct.ToList

                Modifica_Multipla_PianoColturale = Popola_Griglia(progetti, objParametri_Server)

                hdChiavi.Value = JsonConvert.SerializeObject(chiavi.ToList())
            End If
        End If

        hidden_modificaMultipla.Value = modificaMultipla
        hdTestataTemp.Value = IDTestataTemp
        hdKendoModifica_Multipla_PianoColturale.Value = Modifica_Multipla_PianoColturale

    End Sub

    Private Sub inizializzoObjParametri()

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)

    End Sub

    Private Sub inizializzoParametriPagina()

    End Sub

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    Private Sub Modifica_Multipla_PianoColturale_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True

    End Sub

    Private Sub AnnullaTutto()
        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then
            Dim TargetRedirect As String = ""

            MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                          Enum_SiteRedirector.GiasNG,
                                                          enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti,
                                                          TargetRedirect,
                                                          objParametri_Server)

            Response.Redirect(TargetRedirect)
        Else
            Response.Redirect("../Menu/MenuBS_2017.aspx")
        End If
    End Sub

    Public Shared Function RedirectURLFiltrone() As String

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Impianti.Clear()

        Dim RedirectURL As String = "../Filtrone/Filtrone_nuovo.aspx?" &
                    "p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_Modifica_Multipla_PianoColturale, AgroKey_EncoderDecoder, Nothing) &
                    "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                    "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_Modifica_Multipla_PianoColturale, AgroKey_EncoderDecoder, Nothing) &
                    "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                    "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.Modifica_Multipla_PianoColturale, AgroKey_EncoderDecoder, Nothing) &
                    "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                    "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Nothing) &
                    "&cat=esercizio&nopiva=1"

        Return RedirectURL

    End Function

    'richiama filtrone per selezione esecizi
    <WebMethod(EnableSession:=True)>
    Public Shared Function FiltraEsercizi() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            r.RispostaOK = True
            r.RispostaStringa = RedirectURLFiltrone()

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Link_Pagina_FiltroRicercaNG(piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            Dim parametriFiltroRicercaNG As New ParametriFiltroRicercaNG With {
                .Piva = piva,
                .TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita,
                .SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                .PaginaProvenienza = enum_PagineAgenda_2010.Pagina_Modifica_Multipla_PianoColturale,
                .FiltriTemporali = objFiltroRicerca.Imposta_FiltroEntitaAttivaAllaData(Date.Now, Enum_Entita_FiltroRicerca.Impianto),
                .TipoMostraGestitiChiamante = New List(Of Enum_TipoMostra_FiltroRicerca) From {Enum_TipoMostra_FiltroRicerca.Esercizi}
            }

            r.RispostaStringa = objFiltroRicerca.Link_Pagina_FiltroRicercaNG(piva, parametriFiltroRicercaNG)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    '##########################################################################################################################################################
    Public Shared Function LeggiKendoModifica_Multipla_PianoColturale(ByVal dt As DataTable) As String 'TO DO....

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})

        l.Add(New ColonneNome("PIVA", AgronicaAgenda_2010.PartitaIVA, "string") With {._hidden = True, ._width = "145px"})
        l.Add(New ColonneNome("partitaIvaReale", AgronicaAgenda_2010.PartitaIVA, "string") With {._width = "145px"})
        l.Add(New ColonneNome("rag_soc", AgronicaAgenda_2010.RagioneSociale, "string") With {._width = "145px"})
        l.Add(New ColonneNome("sa_nome", AgronicaAgenda_2010.Centro, "string") With {._width = "145px"})
        l.Add(New ColonneNome("app_nome", AgronicaAgenda_2010.AppezzamentoAbbr, "string") With {._width = "145px"})
        l.Add(New ColonneNome("Sup_Imp", AgronicaAgenda_2010.Superficie, "string") With {._width = "145px"})
        l.Add(New ColonneNome("utilizzo", AgronicaAgenda_2010.Utilizzo, "string") With {._width = "145px"})

        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Appezza", "Appezza", "string") With {._hidden = True})
        l.Add(New ColonneNome("id_Reg", "id_Reg", "string") With {._hidden = True})
        l.Add(New ColonneNome("Progetto_Cod", "Progetto_Cod", "string") With {._hidden = True})

        l.Add(New ColonneNome("Blk_Flag", "Blk_Flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("AppBloccato", AgronicaAgenda_2010.Bloccato, "string") With {._Display = True, ._width = "145px"})

        '--- APPEZZAMENTO
        l.Add(New ColonneNome("campo_des", AgronicaAgenda_2010.Campo, "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("nrAppBio", AgronicaAgenda_2010.AppBioCod, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("MetodoProduzione_Des", AgronicaAgenda_2010.MetodoDiProduzione, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("data_fine_appezzamento", AgronicaAgenda_2010.DataFineAppezzamento, "date") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "number") With {._hidden = True})


        '--- IMPIANTO
        l.Add(New ColonneNome("tra_fila_m", AgronicaAgenda_2010.TraFila, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("su_fila_m", AgronicaAgenda_2010.SuFila, "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("cul_des", AgronicaAgenda_2010.Varietà, "string") With {._width = "145px"})
        l.Add(New ColonneNome("Grfi_Des", AgronicaAgenda_2010.Finalità, "string") With {._width = "145px"})
        l.Add(New ColonneNome("port_des", AgronicaAgenda_2010.Portinnesto, "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("Data_Inizio_Impianto", AgronicaAgenda_2010.DataInizioImpianto, "date") With {._width = "145px"})

        'Chiusura Impianto
        l.Add(New ColonneNome("Cop_Des", AgronicaAgenda_2010.Copertura, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("Imp_des", AgronicaAgenda_2010.ImpiantoIrrigazione, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("foral_des", AgronicaAgenda_2010.FormaAllevamento, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("grva_des", AgronicaAgenda_2010.GruppoVarietale, "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("data_fine_impianto", AgronicaAgenda_2010.DataFineImpianto, "date") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("Data_Inizio_Portinnesto", AgronicaAgenda_2010.DataInizioPortinnesto, "date") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("stato_impianto", AgronicaAgenda_2010.StatoImpianto, "string") With {._hidden = True}) 'With {._Display = False, ._width = "145px"}) 'to do, tabellare...

        l.Add(New ColonneNome("veg_cod", "veg_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("cul_cod", "cul_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("grfi_cod", "grfi_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Dpi_Cod", "Dpi_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Stato_Impianto_cod", "Stato_Impianto_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Regolamento_Concimazioni_Cod", "Regolamento_Concimazioni_Cod", "number") With {._hidden = True})



        '--- ESERCIZIO
        l.Add(New ColonneNome("capitolatoPrivato_des", AgronicaAgenda_2010.CapitolatoPrivato, "string") With {._Display = False, ._width = "145px"})
        'Disciplinare - Massimali NPK
        l.Add(New ColonneNome("Resa_Prevista", AgronicaAgenda_2010.ResaPerHa & " [kg/ha]", "number") With {._Display = False, ._formatNr = "n2", ._cssHeader = "text-right", ._css = "text-right", ._width = "145px"})
        l.Add(New ColonneNome("certificazione", AgronicaAgenda_2010.Certificazione, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("organismoReferente_des", AgronicaAgenda_2010.OrganismoReferente, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("magazzinoConferimento_des", AgronicaAgenda_2010.MagazzinoConferimento, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("regolamento_str", AgronicaAgenda_2010.Regolamento, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("disciplinare", AgronicaAgenda_2010.Disciplinare, "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("LimiteN", "N [kg/ha]", "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("LimiteP", "P205 [kg/ha]", "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("LimiteK", "K205 [kg/ha]", "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("Data_Semina_Prevista", AgronicaAgenda_2010.DataSeminaPrevista, "date") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("Data_Raccolta_Prevista", AgronicaAgenda_2010.DataRaccoltaPrevista, "date") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("Data_Fioritura_Prevista", AgronicaAgenda_2010.DataFiorituraPrevista, "date") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("organismoReferente_cod", "organismoReferente_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("magazzinoConferimento", "magazzinoConferimento", "string") With {._hidden = True})


        l.Add(New ColonneNome("validita_inizio", AgronicaAgenda_2010.DataInizioEsercizio, "date") With {._width = "145px"})
        l.Add(New ColonneNome("validita_fine", AgronicaAgenda_2010.DataFineEsercizio, "date") With {._width = "145px"})

        l.Add(New ColonneNome("FlagSecondoRaccolto", AgronicaAgenda_2010.SecondoRaccolto, "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("CertificazioneProdotto", AgronicaAgenda_2010.CertificazioneProdotto, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("Residuo", AgronicaAgenda_2010.Residuo, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("LicenzaColtivazione", AgronicaAgenda_2010.LicenzaColtivazione, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("RiferimentoTrasferimentoDati", AgronicaAgenda_2010.RiferimentoTrasferimentoDati, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("PianoSemina", AgronicaAgenda_2010.PianoSemina, "string") With {._Display = False, ._width = "145px"})
        l.Add(New ColonneNome("Prodotto", AgronicaAgenda_2010.Prodotto, "string") With {._Display = False, ._width = "145px"})

        l.Add(New ColonneNome("EsercizioChiusoCod", "EsercizioChiusoCod", "string") With {._hidden = True, ._width = "145px"})
        l.Add(New ColonneNome("EsercizioChiusoDes", AgronicaAgenda_2010.EsercizioChiuso, "string") With {._Display = False, ._width = "145px"})




        Dim js As New JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l)
        Return risp

    End Function



    '##########################################################################################################################################################

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDisciplinarePrivato() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim leggiPrivato As String = "false"
            Dim ret = objConfSiti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Server)

            If ret <> "" Then
                r.RispostaStringa = ret.ToLower
            Else
                r.RispostaStringa = leggiPrivato
            End If
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaMultipla(ByVal parametri As String, ByVal dati As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim objEreditatore As New AgronicaCoreAnagrafeBIZ.Ereditatore
            r = objEreditatore.ModificaMultipla_PianoColturale(parametri, dati, objParametri_Server)


        Catch ex As GiasException
            'Errore gestito
            r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & ex.Message
            r.RispostaOK = False
        Catch ex As Exception

            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function editResaPrevista(ByVal esercizi As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim startTime As Date = DateTime.UtcNow
        Try
            Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            Dim objParametriSuperServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

            Dim objEreditatore As New AgronicaCoreAnagrafeBIZ.Ereditatore
            r.RispostaOK = objEreditatore.editResaPrevista(esercizi, objParametriServer, objParametriUtenti, objParametriSuperServer)
            r.RispostaStringa = ""
        Catch ex As GiasException
            'Errore gestito
            r.RispostaStringa = ex.Message
            r.RispostaOK = False
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        System.Diagnostics.Debug.WriteLine("Totale tempo impiegato: " & (CLng(DateTime.UtcNow.Subtract(startTime).TotalMilliseconds)))

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GestioneEsercizi(ByVal azione As String, ByVal dataChiusura As String, ByVal esercizi As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objDistinta As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objImpreseProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
            Dim objEsercizi As JArray = JsonConvert.DeserializeObject(esercizi)
            Dim objAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim objReplica As New AgronicaCoreAnagrafeBIZ.Replica_GIAS

            Dim errore As String = ""
            Dim annualita = CInt(azione)

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(objEsercizi)

            For Each obj In objEsercizi

                Dim validita_fine As Date = If(dataChiusura = "", CDate(CStr(obj("validita_fine"))), CDate(dataChiusura))
                Dim dataUltimaChiusura As Date = If(annualita > 0, validita_fine.AddYears(annualita), validita_fine)

                Dim filtroDistinta As String = ""

                Dim strValiditaFineJarray As String = CStr(obj("validita_fine"))
                Dim dateValiditaFine As Date

                If String.IsNullOrEmpty(strValiditaFineJarray) Then
                    dateValiditaFine = AGRODATAFINE
                Else
                    dateValiditaFine = CDate(strValiditaFineJarray)
                End If


                'If azione = "-1" Then
                '    filtroDistinta = " Imprese_Progetti.Validita_Fine > " & Agro_SQL_SaveDate(dataUltimaChiusura)
                'Else
                filtroDistinta = " Imprese_Progetti.Progetto_Cod <> " & Agro_SQL_SaveNum(CInt(obj("Progetto_Cod"))) &
                        " AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(dataUltimaChiusura) &
                        " AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(dateValiditaFine)
                'End If

                ' verifico se ci sono sovrapposizioni con esercizi esistenti
                Dim dtDistinta = objDistinta.LeggiDistinta(
                    CStr(obj("PIVA")), CInt(obj("sa_cod")), CInt(obj("Appezza")), CInt(obj("id_Reg")), "",
                    enumSelezioneVariabile.Selezione_TabellaCompleta, filtroDistinta, "", objParametri_Server)

                If dtDistinta.Rows.Count > 0 Then
                    errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/GestioneEsercizi.aspx", "ImpossibileEseguireOperazioneSuEserciziConDateSovrapposteConAltri"), String)
                Else
                    Dim esistonoMovimenti As Boolean = False
                    Dim esistonoCdG As Boolean = False

                    ' verifico se ci sono movimenti di agenda successivi la data ultima chiusura
                    Dim dtAgenda = objAgenda.LeggiCronologiaMovimenti(CStr(obj("PIVA")), CInt(obj("sa_cod")), CInt(obj("Appezza")), CInt(obj("id_Reg")),
                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      " Movimenti.Data_Movimento > " & Agro_SQL_SaveDate(dataUltimaChiusura),
                                                                      "", objParametri_Server)

                    If dtAgenda.Rows.Count > 0 Then
                        esistonoMovimenti = True
                        errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/GestioneEsercizi.aspx",
                            "ImpossibileEseguireInQuantoPresentiOperazioniAgendaSuccessiveADataChiusura"), String)
                        Exit For
                    End If

                    'COSTI DI GESTIONE
                    Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                    Dim controllo = objControllo.controllo_CdG(Nothing, CStr(obj("PIVA")), CInt(obj("sa_cod")),
                                                               CInt(obj("Appezza")), CInt(obj("id_Reg")), CInt(obj("Progetto_Cod")),
                                                               AGRODATAINIZIO, validita_fine, objParametri_Server)
                    If controllo.errore = True Then
                        esistonoCdG = True
                        errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/GestioneEsercizi.aspx",
                            "ImpossibileEseguireInQuantoPresentiCdGSuccessiviADataChiusura"), String)
                        Exit For
                    End If

                    If esistonoCdG = False AndAlso esistonoMovimenti = False Then
                        Dim NoteLog As String = "Operazione effettuata da Modifica Multipla Piano Colturale"
                        objReplica.Replica_Esercizio(CStr(obj("PIVA")), CInt(obj("Progetto_Cod")), annualita, validita_fine, objParametri_Server, NoteLog)
                    End If
                End If
            Next

            If errore = "" Then
                r.RispostaStringa = "Salvataggio effettuato correttamente"
                r.RispostaOK = True
            Else
                r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & errore
                r.RispostaOK = False
            End If

        Catch ex As GiasException
            'Errori Gestiti
            r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & ex.Message
            r.RispostaOK = False
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            'r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function WS_Popola_Griglia_da_Chiavi(progetti As List(Of Integer), chiavi As List(Of String)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim kendoModificaMultiplaPianoColturale As String = ""

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            kendoModificaMultiplaPianoColturale = Popola_Griglia(progetti, objParametri_Server)

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.Impianti.Clear()

            For Each chiave As String In chiavi
                Dim objImpianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                Dim piva As String = chiave.Split("_")(0)
                Dim sa_cod As String = chiave.Split("_")(1)
                Dim appezza As String = chiave.Split("_")(2)
                Dim id_reg As String = chiave.Split("_")(3)
                Dim veg_cod As String = chiave.Split("_")(4)
                Dim progetto_cod As String = chiave.Split("_")(5)

                objImpianto.Piva = piva
                objImpianto.Sa_Cod = sa_cod
                objImpianto.Appezza = appezza
                objImpianto.ID_Reg = id_reg
                objImpianto.Veg_Cod = veg_cod
                objImpianto.Progetto_Cod = progetto_cod

                objParametriAgenda.Impianti.Add(objImpianto)
            Next

            r.RispostaStringa = kendoModificaMultiplaPianoColturale
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Private Shared Function Popola_Griglia(progetti As List(Of Integer), objParametri_Server As AgronicaCoreParametri) As String
        Dim modificaMultiplaPianoColturale As String

        Try
            Dim PianoColturale As New Impresa_Progetti_R 'to do....
            Dim dt = PianoColturale.Leggi_Report_Modifica_Multipla_PianoColturale(progetti, "", objParametri_Server)
            'dt = DistintaPredisponiDatiMancanti(dt, objParametri_Server)
            modificaMultiplaPianoColturale = LeggiKendoModifica_Multipla_PianoColturale(dt)
        Catch ex As Exception
            Throw New Exception("Errore nell'estrazione dati griglia")
        End Try

        Return modificaMultiplaPianoColturale

    End Function

End Class
