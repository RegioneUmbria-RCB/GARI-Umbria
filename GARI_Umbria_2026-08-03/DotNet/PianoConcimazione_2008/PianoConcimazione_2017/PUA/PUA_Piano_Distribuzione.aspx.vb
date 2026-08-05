Imports System.Web.Services
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello

Public Class PUA_Piano_Distribuzione
    Inherits System.Web.UI.Page

    Private Const SeparatoreChiave As Char = "_"
    Private Const VulnerabileStr As String = " <b>(V)</b>"

    Private Enum SemaforoValutazione
        Verde = 1
        Arancione = 3
        Rosso = 2
    End Enum

    Private _objParametriUtenti As AgronicaCoreParametri
    Private _objParametriServer As AgronicaCoreParametri

    'Dim Qs_AnalisiTestataCod As String
    Private _qsOperazione As String
    Private _qsPiva As String
    Private _qsSaCod As Integer
    Private _qsDataInizio As Date
    Private _qsDataFine As Date
    Private _qsTipo As enum_PUA_Tipo
    Private _qsRicetta As Integer
    Private _qsModalita As enum_PUA_Modalita
    Private _qsPuaCod As Integer
    Private _qsRegolamentoCod As Integer
    Private _qsBloccoFlag As Integer


    Dim objPua As AgronicaCoreGestioneRichieste.ParametriPUA

    Public Shared MenuAgendaNG As Boolean = False

    Public Shared OperazioniAgendaNG As Boolean = False

    Private Sub PUA_Piano_Distribuzione_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Master, MasterConcimazione).ImgBtnAnnullaTutto.Click, AddressOf AnnullaTutto
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.flag_MostraBtnIndietro = True

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        _objParametriUtenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        _objParametriServer = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        objPua = New AgronicaCoreGestioneRichieste.ParametriPUA
        objPua.Leggi()


        If Not IsNothing(Request.QueryString("tipo")) Then
            _qsTipo = Stringa_Decodifica(Request.QueryString("tipo").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsTipo = objPua.Tipo_Pua
        End If

        _qsOperazione = Stringa_Decodifica(Request.QueryString("o").ToString, AgroKey_EncoderDecoder, Server)

        If Not IsNothing(Request.QueryString("p")) Then
            _qsPiva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsPiva = objPua.Piva
        End If

        If Not IsNothing(Request.QueryString("sa_cod")) Then
            _qsSaCod = Stringa_Decodifica(Request.QueryString("sa_cod").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsSaCod = objPua.Sa_Cod
        End If

        If Not IsNothing(Request.QueryString("q")) Then
            _qsPuaCod = Stringa_Decodifica(Request.QueryString("q").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsPuaCod = objPua.PUA_Testata_Cod
        End If

        If Not IsNothing(Request.QueryString("r")) Then
            _qsRegolamentoCod = Stringa_Decodifica(Request.QueryString("r").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsRegolamentoCod = objPua.Regolamento_Cod
        End If

        _qsDataInizio = AGRODATAINIZIO
        If Not IsNothing(Request.QueryString("data_da")) Then
            Dim dataStr As String = Stringa_Decodifica(Request.QueryString("data_da").ToString, AgroKey_EncoderDecoder, Server)
            If IsDate(dataStr) Then
                _qsDataInizio = CDate(dataStr)
            End If
        End If
        _qsDataFine = AGRODATAFINE
        If Not IsNothing(Request.QueryString("data_a")) Then
            Dim dataStr As String = Stringa_Decodifica(Request.QueryString("data_a").ToString, AgroKey_EncoderDecoder, Server)
            If IsDate(dataStr) Then
                _qsDataFine = CDate(dataStr)
            End If
        End If

        If Not IsNothing(Request.QueryString("ric")) Then
            _qsRicetta = Stringa_Decodifica(Request.QueryString("ric").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsRicetta = 0
        End If

        'Modalità pagina (Piano distribuzione / verifica)
        If Not IsNothing(Request.QueryString("m")) Then
            _qsModalita = Stringa_Decodifica(Request.QueryString("m").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsModalita = enum_PUA_Modalita.Modalita_PianoDistribuzione
        End If

        If Not IsNothing(Request.QueryString("blocco_flag")) Then
            _qsBloccoFlag = Stringa_Decodifica(Request.QueryString("blocco_flag").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            _qsBloccoFlag = 0
        End If



        Dim utenteAbilitato As Boolean = True
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        'Select Case _qsOperazione

        '    Case enum_TipoOperazioneDB.Lettura
        '        utenteAbilitato = objPermessi.Controlla_Permessi_Utente(
        '            Session("ASG_Utente_Username"),
        '            Session("ASG_IdServizio"),
        '            enum_Security_Attivita.Gest_PUA,
        '            enum_Security_Operazione.Lettura,
        '            Date.Now,
        '            "",
        '            _objParametriUtenti)

        '    Case Else
        '        utenteAbilitato = objPermessi.Controlla_Permessi_Utente(
        '            Session("ASG_Utente_Username"),
        '            Session("ASG_IdServizio"),
        '            enum_Security_Attivita.Gest_PUA,
        '            enum_Security_Operazione.Modifica,
        '            Date.Now,
        '            "",
        '            _objParametriUtenti)
        'End Select

        If utenteAbilitato = False Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If


        'verifiche permessi vari

        Dim utenteAbilitatoAnalisi As Boolean = True
        utenteAbilitatoAnalisi = objPermessi.Controlla_Permessi_Utente(
                    Session("ASG_Utente_Username"),
                    Session("ASG_IdServizio"),
                    enum_Security_Attivita.Gest_Analisi_AccessoMenu,
                    enum_Security_Operazione.Lettura,
                    Date.Now,
                    "",
                    _objParametriUtenti)

        hdVisualizzaAnalisi.Value = utenteAbilitatoAnalisi

        Dim objAnalisiModelloUtils = New AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility
        Dim analisiTerrenoNG = objAnalisiModelloUtils.usaAnalisiTerrenoNG(_objParametriUtenti)

        hdUsaAnalisiNG.Value = analisiTerrenoNG

        Dim utenteAbilitatoImportaDaQdc As Boolean = True
        utenteAbilitatoImportaDaQdc = objPermessi.Controlla_Permessi_Utente(
                    Session("ASG_Utente_Username"),
                    Session("ASG_IdServizio"),
                    enum_Security_Attivita.Gest_PUA_2_ImportaFertilizzazioniDaRegistro,
                    enum_Security_Operazione.Modifica,
                    Date.Now,
                    "",
                    _objParametriUtenti)


        Dim objUtility_Op As New AgronicaCoreModello.Utility_Operazioni

        MenuAgendaNG = objUtility_Op.Utente_Abilitato_MenuAgendaNG(enum_Security_Operazione.Modifica, _objParametriServer, _objParametriUtenti)

        OperazioniAgendaNG = objUtility_Op.Utente_Abilitato_OperazioniAgendaNG(enum_Security_Operazione.Modifica, _objParametriServer, _objParametriUtenti)


        hdImportaDaQdC.Value = utenteAbilitatoImportaDaQdc


        Txt_ValiditaFine.Enabled = False
        Txt_ValiditaInizio.Enabled = False
        ddlRegolamento.Enabled = False
        ddlCentriAziendali.Enabled = False
        ddlMetodo.Enabled = False

        hdPiva.Value = _qsPiva
        hdSaCod.Value = _qsSaCod
        hdPuaCod.Value = _qsPuaCod
        hdPuaTipo.Value = CInt(_qsTipo)
        hdRegCod.Value = _qsRegolamentoCod
        hdDataInizio.Value = _qsDataInizio
        hdDataFine.Value = _qsDataFine
        hdRicettaCod.Value = _qsRicetta
        hdModalita.Value = _qsModalita
        hdBloccoFlag.Value = _qsBloccoFlag

        If _qsModalita = enum_PUA_Modalita.Modalita_Verifica Then
            If _qsTipo = enum_PUA_Tipo.Completo Then
                rigabilancio.Style.Remove("display")
            End If
            DivMedieAziendali.Style.Remove("display")
            DivEffluenti.Style.Remove("display")
        Else
            DivMedieAziendali.Style.Add("display", "none")
            DivEffluenti.Style.Add("display", "none")
        End If

        If Not Page.IsPostBack Then

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_WS(ddlRegolamento, False, "", "", enum_PUARegolamenti_Tipo.PUA, _qsTipo, "", " Ordine desc ")
            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ddlCentriAziendali, True, "Tutti i Centri Aziendali", "0", _qsPiva, True, 2, "", "", _objParametriServer)

            ddlRegolamento.SelectedIndex = ddlRegolamento.Items.IndexOf(ddlRegolamento.Items.FindByValue(_qsRegolamentoCod))
            Txt_ValiditaInizio.Text = _qsDataInizio.ToShortDateString
            Txt_ValiditaFine.Text = _qsDataFine.ToShortDateString

            Select Case _qsOperazione

                Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura

                    Dim objTest As New AgronicaCorePUA_DAL.PUA_Testata_R
                    Dim dtTest As DataTable = objTest.Leggi(_qsRegolamentoCod, _qsPuaCod, _qsPiva, AGRODATAINIZIO, AGRODATAFINE, "", "", _objParametriServer)

                    If Not dtTest Is Nothing AndAlso dtTest.Rows.Count > 0 Then

                        ddlRegolamento.SelectedIndex = ddlRegolamento.Items.IndexOf(ddlRegolamento.Items.FindByValue(dtTest.Rows(0).Item("Regolamento_Cod")))
                        Txt_ValiditaInizio.Text = dtTest.Rows(0).Item("Validita_inizio")
                        Txt_ValiditaFine.Text = dtTest.Rows(0).Item("Validita_fine")

                        hdDataInizio.Value = dtTest.Rows(0).Item("Validita_inizio")
                        hdDataFine.Value = dtTest.Rows(0).Item("Validita_fine")


                        _qsTipo = dtTest.Rows(0).Item("pua_tipo")
                        hdPuaTipo.Value = CInt(_qsTipo)

                        If Not IsDBNull(dtTest.Rows(0).Item("pua_tipo")) AndAlso CInt(dtTest.Rows(0).Item("pua_tipo")) > 0 Then
                            Select Case CInt(dtTest.Rows(0).Item("pua_tipo"))
                                Case enum_PUA_Tipo.Completo
                                    ddlMetodo.Items.Add(New ListItem("Bilancio", enum_PUA_Tipo.Completo))
                                Case enum_PUA_Tipo.Semplificato
                                    ddlMetodo.Items.Add(New ListItem("Semplificato", enum_PUA_Tipo.Semplificato))
                            End Select
                        End If

                        _qsSaCod = 0
                        If Not IsDBNull(dtTest.Rows(0).Item("sa_cod")) AndAlso CInt(dtTest.Rows(0).Item("sa_cod")) > 0 Then
                            _qsSaCod = CInt(dtTest.Rows(0).Item("sa_cod"))
                            ddlCentriAziendali.SelectedIndex = ddlCentriAziendali.Items.IndexOf(ddlCentriAziendali.Items.FindByValue(dtTest.Rows(0).Item("sa_cod")))
                        End If
                        hdSaCod.Value = _qsSaCod

                    End If

            End Select


            Select Case _qsTipo
                Case enum_PUA_Tipo.Completo
                    Master.Lbl_Titolo.Text = "PUA (Metodo Bilancio)"
                Case Else
                    Master.Lbl_Titolo.Text = "PUA (Metodo MAS)"
            End Select

            Select Case _qsModalita
                Case enum_PUA_Modalita.Modalita_PianoDistribuzione
                    Title = "Piano Distribuzione"
                Case enum_PUA_Modalita.Modalita_Verifica
                    Title = "Verifica Indici Bilancio"
            End Select


            If _qsPiva <> "" Then
                Dim objRS As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Master.LblRag_Soc.Text = objRS.RagSoc_from_Piva(_qsPiva, _objParametriServer)
            End If

        End If

    End Sub

    Private Sub AnnullaTutto()

        Dim urlTarget As String

        urlTarget = "..\PianoConcimazione_MenuBS.aspx" &
                    "?n=" &
                    "&t=" &
                    "&p=" & Stringa_Codifica(_qsPiva, AgroKey_EncoderDecoder, Server) &
                    "&m=" & Stringa_Codifica(CStr(enum_PUARegolamenti_Tipo.PUA), AgroKey_EncoderDecoder, Server)

        Response.Redirect(urlTarget)

    End Sub

#Region "Genera link collegamento ad altre pagine"

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function apriLetamazioni(ByVal regolamento_cod As Integer,
                                           ByVal pua_cod As Integer,
                                           ByVal id As Integer,
                                           ByVal piva As String,
                                           ByVal sa_cod As Integer,
                                           ByVal appezza As Integer,
                                           ByVal id_reg As Integer,
                                           ByVal progetto_cod As Integer,
                                           ByVal data_inizio As String, ByVal data_fine As String
                                           ) As String

        Dim targetUrl As String = "PUA_Letamazioni_Precedenti.aspx" &
                                  "?regolamento_cod=" & Stringa_Codifica(regolamento_cod, AgroKey_EncoderDecoder, Nothing) &
                                  "&pua_cod=" & Stringa_Codifica(pua_cod, AgroKey_EncoderDecoder, Nothing) &
                                  "&id=" & Stringa_Codifica(id, AgroKey_EncoderDecoder, Nothing) &
                                  "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing) &
                                  "&sa_cod=" & Stringa_Codifica(sa_cod, AgroKey_EncoderDecoder, Nothing) &
                                  "&appezza=" & Stringa_Codifica(appezza, AgroKey_EncoderDecoder, Nothing) &
                                  "&id_reg=" & Stringa_Codifica(id_reg, AgroKey_EncoderDecoder, Nothing) &
                                  "&progetto_cod=" & Stringa_Codifica(progetto_cod, AgroKey_EncoderDecoder, Nothing) &
                                  "&data_inizio=" & Stringa_Codifica(data_inizio, AgroKey_EncoderDecoder, Nothing) &
                                  "&data_fine=" & Stringa_Codifica(data_fine, AgroKey_EncoderDecoder, Nothing)

        Return targetUrl

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function NuovaFertilizzazione(ByVal Lav_Cod As Integer, ByVal ChiaveSelezione As String, ByVal RicettaRaccoglitore_Cod As Integer, ByVal Regolamento_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            r.RispostaStringa = Gestione_Ricetta(Lav_Cod, ChiaveSelezione, RicettaRaccoglitore_Cod, Regolamento_Cod)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Gestione_Ricetta(ByVal Lav_Cod As Integer, ByVal ChiaveSelezione As String, ByVal RicettaRaccoglitore_Cod As Integer, ByVal Regolamento_Cod As Integer) As String

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim vegCod As Integer

        Dim objParametriAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        If Not HttpContext.Current.Session("objRicette_2010PerRitorno") Is Nothing Then
            objParametriAgenda = CType(HttpContext.Current.Session("objRicette_2010PerRitorno"), AgronicaCoreGestioneRichieste.ParametriAgenda_2010)
        End If

        Dim xRedir As String = ""

        Dim objImpianto As New PUA_Appezzamento

        Dim chiaveSplit As String() = ChiaveSelezione.Split(SeparatoreChiave)

        objImpianto.Piva = chiaveSplit(0)
        objImpianto.Sa_Cod = chiaveSplit(1)
        objImpianto.appezza = chiaveSplit(2)
        objImpianto.id_reg = chiaveSplit(3)
        objImpianto.Progetto_Cod = chiaveSplit(4)

        Dim leggiVegCodDatoImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        vegCod = leggiVegCodDatoImpianto.VegCod_from_PivaSaCodAppezzaIdimp(objImpianto.Piva,
                                                                           objImpianto.Sa_Cod,
                                                                           objImpianto.appezza,
                                                                           objImpianto.id_reg,
                                                                           "",
                                                                           "",
                                                                           objParametriServer)

        Dim ApriQdCNG As Boolean = OperazioniAgendaNG

        'La Direttiva nitrati PUA_ER_2018 non è gestita sul QdC Angular
        If ApriQdCNG AndAlso Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI AndAlso Regolamento_Cod = enum_PUARegolamenti.PUA_ER_2018 Then
            ApriQdCNG = False
        End If


        If Not ApriQdCNG Then

            objParametriAgenda.Operazione = enum_TipoOperazioneDB.Scrittura
            objParametriAgenda.Piva = objImpianto.Piva
            objParametriAgenda.Sa_Cod = objImpianto.Sa_Cod
            objParametriAgenda.Appezza = objImpianto.appezza
            objParametriAgenda.Id_Reg = objImpianto.id_reg
            objParametriAgenda.DataSelezionata = Now.ToShortDateString
            objParametriAgenda.Lavorazione = -Lav_Cod
            objParametriAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_Trattamenti_B
            objParametriAgenda.Veg_Cod = vegCod
            objParametriAgenda.Id_Agenda = RicettaRaccoglitore_Cod
            objParametriAgenda.Cul_Cod = Regolamento_Cod 'so che fa schifo .. per adesso lo imposto in cul_Cod

            xRedir = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriAgenda)
        Else

            Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

            Dim objParametriSuperServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

            Dim Parametri_Aggiuntivi As New JObject
            Parametri_Aggiuntivi.Item("TipoOperazioneDB") = enum_TipoOperazioneDB.Scrittura
            Parametri_Aggiuntivi.Item("Piva") = objImpianto.Piva
            Parametri_Aggiuntivi.Item("Sa_Cod") = objImpianto.Sa_Cod
            Parametri_Aggiuntivi.Item("Data") = DateTime.Now
            Parametri_Aggiuntivi.Item("Lav_Cod") = Lav_Cod
            Parametri_Aggiuntivi.Item("Veg_Cod") = vegCod
            Parametri_Aggiuntivi.Item("TipoOperazioneAgenda") = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta
            Parametri_Aggiuntivi.Item("TipoRicetta") = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Ricetta.PianoDistribuzionePua
            Parametri_Aggiuntivi.Item("Stato") = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire
            Parametri_Aggiuntivi.Item("Ricetta_Cod") = RicettaRaccoglitore_Cod

            'Imposto il Disciplinare solo per la Distribuzione Ammendanti (che sarà sempre una Direttiva Nitrati)
            If Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then
                Parametri_Aggiuntivi.Item("Regolamento_Cod") = Regolamento_Cod
                Parametri_Aggiuntivi.Item("Tipo_Regolamento") = enum_PUARegolamenti_Tipo.PUA
            End If

            Parametri_Aggiuntivi.Item("Pagina_Provenienza") = enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti
            Parametri_Aggiuntivi.Item("GenericObj_string") = ""


            Dim listImpianti As New List(Of AgronicaCoreGestioneRichieste.ImpiantiAgendaNG)

            listImpianti.Add(New AgronicaCoreGestioneRichieste.ImpiantiAgendaNG With {
                .Piva = objImpianto.Piva,
                .Sa_Cod = objImpianto.Sa_Cod,
                .Appezza = objImpianto.appezza,
                .Id_Reg = objImpianto.id_reg,
                .Progetto_Cod = objImpianto.Progetto_Cod,
                .Sup_Imp_help = 0
                })

            Dim ObjParametriAgenda_NG = JsonConvert.DeserializeObject(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)(Parametri_Aggiuntivi.ToString())

            ObjParametriAgenda_NG.Impianti = listImpianti

            Parametri_Aggiuntivi.Item("Impianti") = JsonConvert.SerializeObject(listImpianti)

            Dim attivita As AgronicaCoreModelsSTD.attivita.Attivita = AgronicaCoreMapper.Utility.GetAttivita_for_Redirect_To_NG(ObjParametriAgenda_NG, True, objParametriServer, objParametriUtenti, objParametriSuperServer)

            Parametri_Aggiuntivi.Item("GenericObj_string") = JsonConvert.SerializeObject(attivita)

            AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objImpianto.Piva,
                                                                  Enum_SiteRedirector.GiasNG,
                                                                  enum_PagineGiasNG.Pagina_Edit_Attivita,
                                                                  xRedir,
                                                                  objParametriServer,
                                                                  Parametri_Aggiuntivi,
                                                                  Enum_SiteRedirector.Sito_PianoConcimazione_2017)

        End If



        Return xRedir

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function MostraBrogliaccio(ByVal piva As String, ByVal RicettaRaccoglitore_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim xRedir As String = ""

            If Not MenuAgendaNG Then
                Dim objParametriAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010 With {
                    .Operazione = enum_TipoOperazioneDB.Scrittura,
                    .Piva = piva,
                    .PaginaRichiesta = enum_PagineAgenda_2010.Menu_BS,
                    .Id_Agenda = RicettaRaccoglitore_Cod
                }



                xRedir = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objParametriAgenda)

            Else


                Dim Parametri_Aggiuntivi As New JObject
                Parametri_Aggiuntivi.Item("TipoOperazioneDB") = enum_TipoOperazioneDB.Scrittura
                Parametri_Aggiuntivi.Item("Ricetta_Cod") = RicettaRaccoglitore_Cod
                Parametri_Aggiuntivi.Item("QSF") = "?d_t=1"
                Parametri_Aggiuntivi.Item("Pagina_Provenienza") = enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti

                AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(piva,
                                                                      Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Menu_Agenda,
                                                                      xRedir,
                                                                      objParametriServer,
                                                                      Parametri_Aggiuntivi,
                                                                      Enum_SiteRedirector.Sito_PianoConcimazione_2017)

            End If

            r.RispostaStringa = xRedir
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function MostraQdC(ByVal piva As String, ByVal RicettaRaccoglitore_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim xRedir As String = ""

            If Not MenuAgendaNG Then

                Dim objParametriAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010 With {
                    .Operazione = enum_TipoOperazioneDB.Scrittura,
                    .Piva = piva,
                    .PaginaRichiesta = enum_PagineAgenda_2010.Menu_BS,
                    .Id_Agenda = RicettaRaccoglitore_Cod,
                    .Mode = "AggiungiAlPua"
                }



                xRedir = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objParametriAgenda)

            Else

                Dim Parametri_Aggiuntivi As New JObject
                Parametri_Aggiuntivi.Item("TipoOperazioneDB") = enum_TipoOperazioneDB.Scrittura
                Parametri_Aggiuntivi.Item("Ricetta_Cod") = RicettaRaccoglitore_Cod
                Parametri_Aggiuntivi.Item("QSF") = "?d_t=0&mode=" & enum_Menu_Agenda_NG_Mode.PUA
                Parametri_Aggiuntivi.Item("Pagina_Provenienza") = enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti

                AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(piva,
                                                                      Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Menu_Agenda,
                                                                      xRedir,
                                                                      objParametriServer,
                                                                      Parametri_Aggiuntivi,
                                                                      Enum_SiteRedirector.Sito_PianoConcimazione_2017)

            End If

            r.RispostaStringa = xRedir
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function MostraAnalisi(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim permessi As New PermessiUtente()
            Dim utenteAbilitatoModifica As Boolean = permessi.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu).Scrittura

            If utenteAbilitatoModifica Then

                Dim url As String = ""
                Dim objAnalisiModelloUtils = New AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility
                Dim analisiTerrenoNG = objAnalisiModelloUtils.usaAnalisiTerrenoNG(objParametri_Utenti)
                If analisiTerrenoNG Then
                    Dim objAnalisiNG As New AgronicaCoreGestioneRichieste.ParametriAnalisiTerrenoNG With {
                        .Pagina_SitoOrigine = enum_PaginePianoConcimazione_2017.PUA_Piano_Distribuzione,
                        .SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                        .Piva = piva,
                        .Tipo_Analisi = enum_AnalisiTipo.Analisi_Terreno,
                        .Tipo_Operazione = enum_TipoOperazioneDB.Lettura,
                        .Pagina_Richiesta = enum_PagineGiasNG.Pagina_Analisi_Terreno
                    }

                    url = objAnalisiModelloUtils.Link_Pagina_AnalisiTerrenoNG(piva, objAnalisiNG)
                Else
                    Dim objAnalisi As New AgronicaCoreGestioneRichieste.ParametriAnalisi_2010 With {
                        .Pagina_SitoOrigine = enum_PaginePianoConcimazione_2017.PUA_Piano_Distribuzione,
                        .SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                        .Piva = piva,
                        .Sa_Cod = 0,
                        .Tipo_Analisi = enum_AnalisiTipo.Analisi_Terreno,
                        .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                        .Pagina_Richiesta = enum_PagineAnalisi_2010.Pagina_Analisi
                    }

                    url = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAnalisi_2010_PassandoDirettamente_ParametriAnalisi_2010(
                    Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                    objAnalisi)
                End If


                r.RispostaStringa = url
                r.RispostaOK = True
            Else
                r.RispostaStringa = "Non si dispone del permesso richiesto per gestire le analisi."
                r.RispostaOK = False
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Web Method Caricamento Griglia"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaPianoDistribuzione(ByVal piva As String, ByVal saCod As Integer,
                                                           ByVal regolamentoCod As Integer,
                                                           ByVal puaCod As Integer,
                                                           ByVal puaTipo As Integer,
                                                           ByVal dataInizio As Date, ByVal dataFine As Date,
                                                           ByVal modalita As Integer, ByVal num_blocco As Integer,
                                                           ByVal forzadefault As String
                                                           ) As rispostaStandard(Of Piano_Distribuzione)

        Dim r As New rispostaStandard(Of Piano_Distribuzione)
        r.RispostaStringa = New Piano_Distribuzione

        Dim objParametriSuperServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try



            'recupero effluenti e relativi fertilizzanti per l'analisi dell'azoto organico
            Dim HashFerCodOrganici As New Hashtable
            Dim HashFerCodDigestati As New Hashtable
            Dim HashFerCodLetami As New Hashtable
            Dim HashFerCodLiquami As New Hashtable

            Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
            Dim DtEffluenti As DataTable = objEff.Leggi(regolamentoCod, puaCod, 0, " azoto_qta >0 ", "", objParametriServer)

            Dim IncludiEffRif As Boolean = False
            If modalita = enum_PUA_Modalita.Modalita_Verifica Then
                IncludiEffRif = True
            End If
            Dim objEffluentiInput As New PUA_Effluenti_input With {
                    .Regolamento_Cod = regolamentoCod,
                    .Includi_Efficienza_Rif = IncludiEffRif
                }
            Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
            Dim objEffluentiOutput As New PUA_Effluenti_output
            objEffluentiOutput = objPC.Effluenti(objEffluentiInput, objParametriServer, objParametriSuperServer)

            Dim Perc_Zootecnico As Decimal = 100
            Dim str_FerCod_Org As String = ""

            If Not objEffluentiOutput Is Nothing Then
                For Each eff As PUA_Effluente In objEffluentiOutput.ListaEffluenti

                    Perc_Zootecnico = 100
                    If eff.MatricePrevalente = 1 AndAlso Not DtEffluenti Is Nothing AndAlso DtEffluenti.Select("eff_cod=" & eff.Eff_Cod).Length > 0 Then
                        Perc_Zootecnico = DtEffluenti.Select("eff_cod=" & eff.Eff_Cod)(0).Item("perc_zootecnico")
                    End If

                    If eff.SpecieAllevamento = 1 Then
                        HashFerCodOrganici.Add(eff.Fer_Cod, Perc_Zootecnico)
                        str_FerCod_Org &= eff.Fer_Cod & ","
                    End If
                    If eff.MatricePrevalente = 1 Then
                        HashFerCodDigestati.Add(eff.Fer_Cod, Perc_Zootecnico)
                    End If

                    Select Case eff.Id_tp_fer
                        Case enum_PUA_TipoFertilizzante.Ammendante
                            HashFerCodLetami.Add(eff.Fer_Cod, Perc_Zootecnico)
                        Case enum_PUA_TipoFertilizzante.Liquame
                            HashFerCodLiquami.Add(eff.Fer_Cod, Perc_Zootecnico)
                    End Select


                Next
            End If

            If str_FerCod_Org <> "" Then
                str_FerCod_Org = "(" & Left(str_FerCod_Org, str_FerCod_Org.Length - 1) & ")"
            End If

            '"(" & fer_cod_d & ")") * HashFerCodDigestati(fer_cod_d) / 100

            Dim objImpiantiR As New Reg_Impianti_Read
            Dim objImpreseProgettiW As New Impresa_Progetti_W
            Dim objSequenze As New Agro_Sequenze
            Dim objAnaVincoliW As New Anagrafe_VincoliAgronomici_W
            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

            Dim Filtro As String = "" ' s.veg_cod=38 "

            '(05/04/2023) Modifica Stato_Impianto (settare a 102 [enum_Stato_Impianto.Impianto_Produzione] dove = 0)
            objImpreseProgettiW.AggiornaStatoImpianto_Massiva(piva, dataInizio, dataFine, objParametriServer)

            Dim dt As DataTable = objImpiantiR.Leggi_Dati_Impianti_per_PUA_NEW(piva, saCod, puaCod, regolamentoCod,
                                                                                    dataInizio, dataFine,
                                                                                       1,
                                                                                       str_FerCod_Org, HashFerCodDigestati,
                                                                                       modalita, HashFerCodLetami, HashFerCodLiquami,
                                                                                       Filtro, "App_Nome", objParametriServer)

            Dim dtTN As DataTable = objImpiantiR.Leggi_Dati_Impianti_per_PUA_NEW(piva, saCod, puaCod, regolamentoCod,
                                                                                    dataInizio, dataFine,
                                                                                       2,
                                                                                       "", Nothing,
                                                                                       modalita, Nothing, Nothing,
                                                                                       "", "App_Nome", objParametriServer)

            Dim dictStatoImpianto As New Dictionary(Of Integer, List(Of Fase))

            Dim listaPrecessioni As List(Of Precessione) = GetListaPrecessioniDes(regolamentoCod, puaTipo)
            Dim listaUbicazioni As List(Of Ubicazione) = GetListaUbicazioniDes(regolamentoCod)
            Dim listaTipiAcqua As List(Of TipoAcqua) = GetListaTipiAcquaDes(regolamentoCod)
            Dim ListaPrecessionexSpecie As List(Of PrecessionexSpecie) = GetListaPrecessionixSpecieDes(regolamentoCod)

            'estraggo i singoli veg_cod
            Dim listaReseMas As List(Of PianoConcimazione_LimiteMAS_output)
            Dim listaFinalita As List(Of Finalita)
            Dim listaCoefficienteB As List(Of PUA_CoefficienteB)
            Dim HashVegCod As New Hashtable
            Dim veg_cod_elenco As String
            If Not dt Is Nothing Then
                For Each row In dt.Rows
                    If Not HashVegCod.ContainsKey(row.Item("veg_cod")) Then
                        HashVegCod.Add(row.Item("veg_cod"), "")
                        veg_cod_elenco &= row.Item("veg_cod") & ","
                    End If
                Next
                If veg_cod_elenco <> "" Then
                    listaReseMas = GetListaReseMas(regolamentoCod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
                    listaFinalita = GetListaFinalitaRer(regolamentoCod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
                    listaCoefficienteB = GetListaCoefficienteB(regolamentoCod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
                End If
            End If


            Dim dtZvnServer As DataTable = Nothing
            Dim dtZvnMetaschema As DataTable = Nothing
            LeggiZoneVulnerabili(regolamentoCod, dataInizio, dataFine, dtZvnServer, dtZvnMetaschema, objParametriServer)


            'TODO: verifico se nelle liste c'è un solo elemento possibile, quindi lo scriverò subito

            Dim preimpostaPrecessione As Integer? = Nothing
            If Not listaPrecessioni Is Nothing AndAlso listaPrecessioni.Count = 1 Then
                preimpostaPrecessione = listaPrecessioni(0).Codice
            End If

            Dim preimpostaUbicazione As Integer? = Nothing
            If Not listaUbicazioni Is Nothing AndAlso listaUbicazioni.Count = 1 Then
                preimpostaUbicazione = listaUbicazioni(0).Codice
            End If

            Dim listPiano As New List(Of PUA_Appezzamento)

            'Chiave = keyImpianto, Value = keyParticella
            Dim dict As New Dictionary(Of String, List(Of String))
            Dim dictAnalisi As New Dictionary(Of Integer, AnalisiDettaglio)

            Dim count As Integer = 1

            If Not dt Is Nothing Then

                For Each row In dt.Rows

                    Dim chiaveDistinta As String = row.Item("Piva") & SeparatoreChiave &
                                                   row.Item("Sa_Cod") & SeparatoreChiave &
                                                   row.Item("Appezza") & SeparatoreChiave &
                                                   row.Item("Id_Reg") & SeparatoreChiave &
                                                   row.Item("Progetto_Cod")

                    'Dim chiavePart As String = row.Item("PROV") & ":" & row.Item("COM") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    Dim chiavePart As String = ""
                    If row.Item("PROV") <> "" Then
                        chiavePart = row.Item("provincia") & ":_" & row.Item("comune") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    End If

                    Dim objPiano As PUA_Appezzamento

                    If Not dict.ContainsKey(chiaveDistinta) Then

                        Dim nFabbDatabaseStr As String = row.Item("N_Fabbisogno")
                        Dim nFabbDatabase As Decimal = -1
                        If IsNumeric(nFabbDatabaseStr) Then
                            nFabbDatabase = CDec(nFabbDatabaseStr)
                        End If

                        Dim nFabbDatabaseOrgStr As String = row.Item("N_Fabbisogno_Organico")
                        Dim nFabbDatabaseOrg As Decimal = -1
                        If IsNumeric(nFabbDatabaseOrgStr) Then
                            nFabbDatabaseOrg = CDec(nFabbDatabaseOrgStr)
                        End If

                        'è la prima volta dell'impianto, quindi devo creare tutto
                        objPiano = New PUA_Appezzamento With {
                            .Chiave = chiaveDistinta,
                            .Piva = row.Item("Piva"),
                            .Sa_Cod = row.Item("Sa_Cod"),
                            .Campo_Cod = row.Item("Campo_Cod"),
                            .appezza = row.Item("Appezza"),
                            .id_reg = row.Item("Id_Reg"),
                            .Progetto_Cod = row.Item("Progetto_Cod"),
                            .Veg_Cod = row.Item("Veg_Cod"),
                            .Grfi_Cod = row.Item("Grfi_Cod"),
                            .Grfi_Cod_Concimazione = row.Item("Grfi_Cod_Concimazione"),
                            .B_Perc = 0,
                            .Appezzamento = row.Item("App_Nome") & " - " & row.Item("veg_des"),
                            .Superficie = row.Item("Sup_Imp"),
                            .ValiditaInizio = row.Item("Validita_Inizio_Impianto"),
                            .ValiditaFine = row.Item("Validita_Fine_Impianto"),
                            .DurataColtura = row.Item("Validita_Inizio_Impianto") & "|" & row.Item("Validita_Fine_Impianto"),
                            .StatoImpiantoCod = row.Item("Stato_Impianto"),
                            .Ciclo = row.Item("Ciclo_Cod"),
                            .CicloDes = row.Item("Ciclo_Des"),
                            .Resa = row.Item("Resa"),
                            .Resa_Rif = 0,
                            .FattoreCorrettivo_N = 0,
                            .Id_AnagrafeVincoli = row.Item("Id_AnaVincoli"),
                            .Pua_Cod = row.Item("Pua_Cod"),
                            .Regolamento_Cod = row.Item("PUA_Regolamento_Cod"),
                            .Data_Pua = row.Item("DataPua"),
                            .AnalisiTestataCod = row.Item("Analisi_Testata_Cod"),
                            .AnalisiTestataDes = row.Item("Analisi_Testata_Des"),
                            .Sabbia = row.Item("sabbia"),
                            .Argilla = row.Item("argilla"),
                            .So = row.Item("So"),
                            .PrecessioneCod = row.Item("Veg_Cod_Prec"),
                            .UbicazioneCod = row.Item("Ubicazione_Cod"),
                            .TipoAcquaCod = row.Item("TipoAcqua_Cod"),
                            .N_FertilizzazioniPrecedenti = row.Item("N_FertilizzazioniPrecedenti"),
                            .N_Fabbisogno_Database = nFabbDatabase,
                            .N_Fabbisogno = nFabbDatabaseOrg,
                            .N_FabbisognoComplessivo = If(nFabbDatabase >= 0, nFabbDatabase, 0) * row.Item("Sup_Imp"),
                            .N_FabbisognoSoddisfatto = CDec(row.Item("N_FabbisognoSoddisfatto")),
                            .N_Zootecnico = CDec(row.Item("N_FabbisognoSoddisfattoOrganico")) + CDec(row.Item("N_SoddisfattoDigestato")),
                            .LimiteMas = 0,
                            .N_TotaleSoddisfatto = CDec(row.Item("N_TotaleSoddisfatto")),
                            .N_Zootecnico_Letame = CDec(row.Item("N_Zootecnico_Letame")),
                            .N_Zootecnico_Liquame = CDec(row.Item("N_Zootecnico_Liquame"))
                        }


                        'objPiano.N_Zootecnico = N_SoddisfattoOrg + N_SoddisfattoDigestato

                        '------ Scrivo (se già non l'avevo) Anagrafe_VincoliAgronomici
                        'TODO: Se Id_PartVincoli è 0, scrivo cmq un record fittizio, così poi non ho più il problema se modifico molteplici volte la stessa riga
                        If objPiano.Id_AnagrafeVincoli <= 0 Then

                            'TODO: in realtà prima di scrivere dovrei verificare se quella particella ce l'avrei già (a parità di tutti i valori)?!?
                            ' se mi dovesse capitare che la stessa particella (e macrouso) è presente su più righe finirei per scriverla due volte, mentre dovrebbe essere
                            ' scritta una volta sola e riportata la chiave
                            'Ma il problema così si potrebbe verificare che se la stessa chiave è su più righe e modifico qualcosa, allora dovrei ricaricare anche la seconda riga

                            Dim idAna As Integer = objSequenze.NuovoId_Tabella("Anagrafe_VincoliAgronomici",
                                                                               0, 2000000000, objParametriServer)

                            Dim xRisp As Boolean = False
                            xRisp = objAnaVincoliW.Scrivi(idAna,
                                                          objPiano.Piva, objPiano.Sa_Cod, objPiano.appezza, objPiano.id_reg, objPiano.Progetto_Cod,
                                                          puaCod, regolamentoCod,
                                                          objPiano.AnalisiTestataCod, objPiano.PrecessioneCod,
                                                          objPiano.UbicazioneCod, objPiano.TipoAcquaCod,
                                                          objPiano.N_FertilizzazioniPrecedenti,
                                                          dataInizio, dataFine,
                                                          objParametriServer)

                            If xRisp = True Then
                                objPiano.Id_AnagrafeVincoli = idAna
                                objPUALog.Scrivi(enum_TipoOperazioneDB.Scrittura, "Anagrafe_VincoliAgronomici", puaCod, regolamentoCod, idAna, objPiano.Piva, objPiano.Sa_Cod, objPiano.appezza, objPiano.id_reg, objPiano.Progetto_Cod, Nothing, Nothing, "", objParametriServer)
                            End If

                        End If

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            objPiano.ZVN = True
                        End If


                        objPiano.ParticelleVincoli.Add(objParticella)


                        '------ Catasto
                        objPiano.Catasto = chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")

                        '------ StatoImpiantoDes
                        objPiano.StatoImpiantoDes = GetStatoImpiantoDes(dictStatoImpianto,
                                                                        regolamentoCod, objPiano.Veg_Cod,
                                                                        objPiano.StatoImpiantoCod)

                        '------ PrecessioneDes
                        objPiano.PrecessioneDes = GetPrecessioneDes(listaPrecessioni, objPiano.PrecessioneCod)

                        '------ UbicazioneDes
                        objPiano.UbicazioneDes = GetUbicazioneDes(listaUbicazioni, objPiano.UbicazioneCod)

                        '------ TipoAcquaDes
                        objPiano.TipoAcquaDes = GetTipoAcquaDes(listaTipiAcqua, objPiano.TipoAcquaCod)

                        Dim ResaDB As Decimal = -1
                        Dim MasDB As Decimal = -1
                        Dim Grfi_Cod_ConcimazioneDB As Integer = -1
                        Dim FattoreCorrettivo_N_DB As Decimal = -1
                        GetResaMas(listaReseMas, listaFinalita, listaCoefficienteB,
                                   objPiano.Veg_Cod, objPiano.Grfi_Cod, objPiano.Grfi_Cod_Concimazione,
                                   objPiano.StatoImpiantoCod, ResaDB, MasDB, Grfi_Cod_ConcimazioneDB, FattoreCorrettivo_N_DB)

                        '------ Resa
                        If objPiano.Resa <= 0 And ResaDB > 0 Then
                            objPiano.Resa = ResaDB
                            ValorizzaResaDaMas(objPiano, objParametriServer)
                        End If

                        '----- Resa_Rif
                        If objPiano.Resa_Rif <= 0 And ResaDB > 0 Then
                            objPiano.Resa_Rif = ResaDB
                        End If

                        '----- FattoreCorrettivo_N
                        If objPiano.FattoreCorrettivo_N <= 0 And FattoreCorrettivo_N_DB > 0 Then
                            objPiano.FattoreCorrettivo_N = FattoreCorrettivo_N_DB
                        End If

                        '----- Mas
                        If objPiano.LimiteMas <= 0 And MasDB > 0 Then
                            objPiano.LimiteMas = MasDB
                        End If

                        '-- finalita piano conc
                        If objPiano.Grfi_Cod_Concimazione <= 0 And Grfi_Cod_ConcimazioneDB > 0 Then
                            objPiano.Grfi_Cod_Concimazione = Grfi_Cod_ConcimazioneDB
                            ValorizzaFinalitaRer(objPiano, objParametriServer)
                        End If

                        If objPiano.Grfi_Cod_Concimazione > 0 Then
                            objPiano.Grfi_Des_Concimazione = GetFinalitaRerDes(listaFinalita, objPiano.Grfi_Cod_Concimazione)
                            objPiano.B_Perc = GetCoefficienteB(listaCoefficienteB, objPiano.Veg_Cod, objPiano.Grfi_Cod_Concimazione)
                        End If

                        listPiano.Add(objPiano)

                        dict.Add(chiaveDistinta, New List(Of String) From {chiavePart})

                    Else

                        'devo solo prendere la parte della particella
                        Dim idx As Integer = listPiano.FindIndex(Function(x) x.Chiave = chiaveDistinta)

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            listPiano(idx).ZVN = True
                        End If

                        listPiano(idx).Catasto = listPiano(idx).Catasto & "|" & chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")
                        listPiano(idx).ParticelleVincoli.Add(objParticella)

                    End If

                    count += 1
                Next

            End If


            '------ Imposto dei valori di default, quando ce n'è uno solo possibile

            ' Così mi tocca fare un n-esimo ciclo, ma almeno posso andare sempre in modifica
            ' (perché nel giro precedente, se non c'era il record l'ho scritto, quindi ho sempre Id_AnagrafeVincoli)
            ' in aggiunta, visto che per capire se ho un'unica analisi possibile devo fare il controllo quando conosco tutte le particelle della riga,
            ' avrei cmq dovuto fare questo ciclo, anche se gli altri elementi li avrei potuti impostare prima

            If modalita = enum_PUA_Modalita.Modalita_PianoDistribuzione Then
                AssegnaValoriDefault(piva, puaCod, regolamentoCod, dataInizio, dataFine, forzadefault,
                                 listPiano,
                                 listaPrecessioni, ListaPrecessionexSpecie,
                                 listaUbicazioni, listaTipiAcqua, dictAnalisi,
                                 preimpostaPrecessione, preimpostaUbicazione,
                                 objParametriServer, objParametriUtenti)
            End If


            'ricavo N_Fabbisogno 

            '(09/10/2019 fede) spezzo le chiamate al ws per ridurre la dimensione della richiesta
            Dim listPianoTmp As New List(Of PUA_Appezzamento)
            Dim n As Integer = 1
            Dim listPianoTotale As New List(Of PUA_Appezzamento)

            'num_blocco = 100

            For l = 0 To listPiano.Count - 1

                listPianoTmp.Add(listPiano(l))

                If (listPiano.Count - 1) = l Then
                    Dim Fabb_InputTmp As New PUA_Fabbisogni_input With {
                        .Regolamento_Cod = regolamentoCod,
                        .PUA_Tipo = puaTipo,
                        .PUA_ListaAppezzamenti = listPianoTmp
                    }
                    Dim Fabb_outTmp As New PUA_Fabbisogni_output
                    Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                    'Fabb_outTmp = objPC_WS.PUAFabbisogni(Fabb_InputTmp)
                    Fabb_outTmp = objPC_WS.PUAFabbisogniCompresso(Fabb_InputTmp)

                    listPianoTotale.AddRange(Fabb_outTmp.PUA_ListaAppezzamenti)
                    n = 1
                    listPianoTmp.Clear()
                ElseIf n < num_blocco Then
                    n += 1
                Else
                    Dim Fabb_InputTmp As New PUA_Fabbisogni_input With {
                        .Regolamento_Cod = regolamentoCod,
                        .PUA_Tipo = puaTipo,
                        .PUA_ListaAppezzamenti = listPianoTmp
                    }
                    Dim Fabb_outTmp As New PUA_Fabbisogni_output
                    Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                    'Fabb_outTmp = objPC_WS.PUAFabbisogni(Fabb_InputTmp)
                    Fabb_outTmp = objPC_WS.PUAFabbisogniCompresso(Fabb_InputTmp)

                    listPianoTotale.AddRange(Fabb_outTmp.PUA_ListaAppezzamenti)
                    n = 1
                    listPianoTmp.Clear()
                End If

            Next

            listPiano = listPianoTotale

            'Fisso su DB il Fabbisogno di N
            Dim objRegImpianti As New Reg_Impianti_Codici_W

            For Each Item In listPiano
                'Reg_Impianti_Codici (in automatico prima cancella e poi riscrive)
                objRegImpianti.ModificaxProgetto2(Item.Piva, Item.Sa_Cod, Item.appezza, Item.id_reg, Item.Progetto_Cod,
                                                  enum_CodiciAnagrafe.Impianto_LimiteN_Organico,
                                                  Item.N_Fabbisogno,
                                                  AGRODATAINIZIO, AGRODATAFINE,
                                                  "",
                                                  objParametriServer)

            Next
            '----------------------------------------------------------------------------------------------------

            'aggiungo i terreni nudi
            If Not dtTN Is Nothing Then

                For Each row In dtTN.Rows

                    Dim chiaveDistinta As String = row.Item("Piva") & SeparatoreChiave &
                                                   row.Item("Sa_Cod") & SeparatoreChiave &
                                                   row.Item("Appezza") & SeparatoreChiave &
                                                   row.Item("Id_Reg") & SeparatoreChiave &
                                                   row.Item("Progetto_Cod")
                    'Dim chiavePart As String = row.Item("PROV") & ":" & row.Item("COM") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    Dim chiavePart As String = ""
                    If row.Item("PROV") <> "" Then
                        chiavePart = row.Item("provincia") & ":_" & row.Item("comune") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    End If
                    Dim objPiano As PUA_Appezzamento

                    If Not dict.ContainsKey(chiaveDistinta) Then

                        Dim nFabbDatabaseStr As String = row.Item("N_Fabbisogno")
                        Dim nFabbDatabase As Decimal = -1
                        If IsNumeric(nFabbDatabaseStr) Then
                            nFabbDatabase = CDec(nFabbDatabaseStr)
                        End If

                        Dim nFabbDatabaseOrgStr As String = row.Item("N_Fabbisogno_Organico")
                        Dim nFabbDatabaseOrg As Decimal = -1
                        If IsNumeric(nFabbDatabaseOrgStr) Then
                            nFabbDatabaseOrg = CDec(nFabbDatabaseOrgStr)
                        End If

                        ' Giulia: 9/1/2020: Quando passava da web service, N_fabbisogno, veniva impostato a 0, ora viene mostrato -1
                        'quindi se su db è "impostato" -1 (vuol dire che non l'avevo ancora associato all'impianto), mostro 0; se poi si sceglie di far ereditare all'impianto il valore, sarà salvato 0

                        'è la prima volta dell'impianto, quindi devo creare tutto
                        objPiano = New PUA_Appezzamento With {
                            .Chiave = chiaveDistinta,
                            .Piva = row.Item("Piva"),
                            .Sa_Cod = row.Item("Sa_Cod"),
                            .Campo_Cod = row.Item("Campo_Cod"),
                            .appezza = row.Item("Appezza"),
                            .id_reg = row.Item("Id_Reg"),
                            .Progetto_Cod = row.Item("Progetto_Cod"),
                            .Veg_Cod = row.Item("Veg_Cod"),
                            .Grfi_Cod = row.Item("Grfi_Cod"),
                            .Grfi_Cod_Concimazione = row.Item("Grfi_Cod_Concimazione"),
                            .B_Perc = 0,
                            .Appezzamento = row.Item("App_Nome") & " - " & row.Item("veg_des"),
                            .Superficie = row.Item("Sup_Imp"),
                            .ValiditaInizio = row.Item("Validita_Inizio_Impianto"),
                            .ValiditaFine = row.Item("Validita_Fine_Impianto"),
                            .DurataColtura = row.Item("Validita_Inizio_Impianto") & "|" & row.Item("Validita_Fine_Impianto"),
                            .StatoImpiantoCod = row.Item("Stato_Impianto"),
                            .Ciclo = row.Item("Ciclo_Cod"),
                            .CicloDes = row.Item("Ciclo_Des"),
                            .Resa = row.Item("Resa"),
                            .Id_AnagrafeVincoli = row.Item("Id_AnaVincoli"),
                            .Pua_Cod = row.Item("Pua_Cod"),
                            .Regolamento_Cod = row.Item("PUA_Regolamento_Cod"),
                            .Data_Pua = row.Item("DataPua"),
                            .AnalisiTestataCod = row.Item("Analisi_Testata_Cod"),
                            .AnalisiTestataDes = row.Item("Analisi_Testata_Des"),
                            .Sabbia = row.Item("sabbia"),
                            .Argilla = row.Item("argilla"),
                            .So = row.Item("So"),
                            .PrecessioneCod = row.Item("Veg_Cod_Prec"),
                            .UbicazioneCod = row.Item("Ubicazione_Cod"),
                            .TipoAcquaCod = row.Item("TipoAcqua_Cod"),
                            .N_FertilizzazioniPrecedenti = row.Item("N_FertilizzazioniPrecedenti"),
                            .N_Fabbisogno_Database = nFabbDatabase,
                            .N_Fabbisogno = If(nFabbDatabaseOrg >= 0, nFabbDatabaseOrg, 0),
                            .N_FabbisognoComplessivo = If(nFabbDatabase >= 0, nFabbDatabase, 0) * row.Item("Sup_Imp"),
                            .N_FabbisognoSoddisfatto = 0,
                            .N_Zootecnico = 0,
                            .LimiteMas = 0
                        }

                        '------ Scrivo (se già non l'avevo) Anagrafe_VincoliAgronomici
                        'TODO: Se Id_PartVincoli è 0, scrivo cmq un record fittizio, così poi non ho più il problema se modifico molteplici volte la stessa riga
                        If objPiano.Id_AnagrafeVincoli <= 0 Then

                            'TODO: in realtà prima di scrivere dovrei verificare se quella particella ce l'avrei già (a parità di tutti i valori)?!?
                            ' se mi dovesse capitare che la stessa particella (e macrouso) è presente su più righe finirei per scriverla due volte, mentre dovrebbe essere
                            ' scritta una volta sola e riportata la chiave
                            'Ma il problema così si potrebbe verificare che se la stessa chiave è su più righe e modifico qualcosa, allora dovrei ricaricare anche la seconda riga

                            Dim idAna As Integer = objSequenze.NuovoId_Tabella("Anagrafe_VincoliAgronomici",
                                                                               0, 2000000000, objParametriServer)

                            Dim xRisp As Boolean = False
                            xRisp = objAnaVincoliW.Scrivi(idAna,
                                                          objPiano.Piva, objPiano.Sa_Cod, objPiano.appezza, objPiano.id_reg, objPiano.Progetto_Cod,
                                                          puaCod, regolamentoCod,
                                                          objPiano.AnalisiTestataCod, objPiano.PrecessioneCod,
                                                          objPiano.UbicazioneCod, objPiano.TipoAcquaCod,
                                                          objPiano.N_FertilizzazioniPrecedenti,
                                                          dataInizio, dataFine,
                                                          objParametriServer)

                            If xRisp = True Then
                                objPiano.Id_AnagrafeVincoli = idAna
                                objPUALog.Scrivi(enum_TipoOperazioneDB.Scrittura, "Anagrafe_VincoliAgronomici", puaCod, regolamentoCod, idAna, objPiano.Piva, objPiano.Sa_Cod, objPiano.appezza, objPiano.id_reg, objPiano.Progetto_Cod, Nothing, Nothing, "", objParametriServer)
                            End If

                        End If

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            objPiano.ZVN = True
                        End If

                        objPiano.ParticelleVincoli.Add(objParticella)

                        '------ Catasto
                        objPiano.Catasto = chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")

                        '------ StatoImpiantoDes
                        objPiano.StatoImpiantoDes = GetStatoImpiantoDes(dictStatoImpianto,
                                                                        regolamentoCod, objPiano.Veg_Cod,
                                                                        objPiano.StatoImpiantoCod)

                        '------ PrecessioneDes
                        objPiano.PrecessioneDes = GetPrecessioneDes(listaPrecessioni, objPiano.PrecessioneCod)

                        '------ UbicazioneDes
                        objPiano.UbicazioneDes = GetUbicazioneDes(listaUbicazioni, objPiano.UbicazioneCod)

                        '------ TipoAcquaDes
                        objPiano.TipoAcquaDes = GetTipoAcquaDes(listaTipiAcqua, objPiano.TipoAcquaCod)

                        listPiano.Add(objPiano)

                        dict.Add(chiaveDistinta, New List(Of String) From {chiavePart})

                    Else

                        'devo solo prendere la parte della particella
                        Dim idx As Integer = listPiano.FindIndex(Function(x) x.Chiave = chiaveDistinta)

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            listPiano(idx).ZVN = True
                        End If

                        listPiano(idx).Catasto = listPiano(idx).Catasto & "|" & chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")
                        listPiano(idx).ParticelleVincoli.Add(objParticella)

                    End If

                    count += 1
                Next

            End If




            '----------------------------------------------------------------------------------------------------

            If modalita = enum_PUA_Modalita.Modalita_Verifica Then

                For Each item As PUA_Appezzamento In listPiano
                    AssegnaSemafori(item)
                Next

                If Not DtEffluenti Is Nothing Then

                    Dim DT_Effluenti As New DataTable
                    Dim DT_Effluenti_Dettagli As New DataTable

                    Crea_DT_Effluenti(DT_Effluenti)
                    Crea_DT_Effluenti_Dettagli(DT_Effluenti_Dettagli)

                    Dim HashEffCod As New Hashtable

                    Dim Eff_Cod As Integer
                    Dim Eff_Des As String
                    Dim Fer_Cod As Integer
                    Dim Fer_Des As String
                    Dim Tipo_Eff_Cod As Integer
                    Dim Tipo_Eff_Des As String
                    Dim Udm_Cod As Integer
                    Dim Udm_Sim As String
                    Dim Flag_Tipo_Allevamento As Integer
                    Dim Flag_Matrice_Prevalente As Integer
                    Dim Flag_ProvenienzaEsterna As Integer
                    Dim Flag_ProvenienzaEsterna_Des As String
                    Dim FertilizzanteUsato As Decimal
                    Dim EffPesataConNDistribuito As Decimal
                    Dim NDistribuito As Decimal
                    Dim EffRif As Decimal
                    Dim EffCons As Decimal
                    Dim CaricoTot As Decimal
                    Dim RiempimentoTot As Decimal
                    Dim AzotoQtaTot As Decimal


                    For i = 0 To DtEffluenti.Rows.Count - 1

                        Eff_Cod = DtEffluenti.Rows(i).Item("eff_cod")
                        Eff_Des = ""
                        Tipo_Eff_Cod = 0
                        Tipo_Eff_Des = ""
                        Udm_Cod = 0
                        Udm_Sim = ""
                        Flag_Tipo_Allevamento = 0
                        Flag_Matrice_Prevalente = 0
                        Flag_ProvenienzaEsterna = 0
                        Flag_ProvenienzaEsterna_Des = "No"
                        FertilizzanteUsato = 0
                        Fer_Cod = 0
                        Fer_Des = ""
                        EffRif = 0
                        EffCons = 0
                        EffPesataConNDistribuito = 0
                        NDistribuito = 0

                        If Not IsNothing(objEffluentiOutput) Then
                            Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluente
                            Effluente = objEffluentiOutput.ListaEffluenti.Where(Function(x) x.Eff_Cod = Eff_Cod)(0)
                            If Not Effluente Is Nothing Then
                                Eff_Des = Effluente.Eff_Des
                                Tipo_Eff_Cod = Effluente.Tipo_Eff_Cod
                                Tipo_Eff_Des = Effluente.Tipo_Eff_Des
                                Udm_Cod = Effluente.Udm_Cod
                                Udm_Sim = Effluente.Udm_Sim
                                Flag_Tipo_Allevamento = Effluente.SpecieAllevamento
                                Flag_Matrice_Prevalente = Effluente.MatricePrevalente
                                Fer_Cod = Effluente.Fer_Cod
                                Fer_Des = Effluente.Fer_Des
                                EffRif = Effluente.Efficienza_Rif
                            End If
                        End If

                        If IsDBNull(DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna")) Then
                            Flag_ProvenienzaEsterna = DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna")
                        End If

                        Flag_ProvenienzaEsterna_Des = If(DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna") = 1, "Sì", "No")


                        InserisciRiga_DT_Effluenti_Dettagli(DT_Effluenti_Dettagli,
                                                            DtEffluenti.Rows(i).Item("id"),
                                                            Eff_Cod, Eff_Des, Fer_Cod, Fer_Des, Udm_Cod, Udm_Sim,
                                                             DtEffluenti.Rows(i).Item("carico"), DtEffluenti.Rows(i).Item("riempimento"),
                                                              DtEffluenti.Rows(i).Item("azoto_qta"), DtEffluenti.Rows(i).Item("azoto_titoli"),
                                                              Flag_ProvenienzaEsterna, Flag_ProvenienzaEsterna_Des, EffRif)



                        If Not HashEffCod.ContainsKey(Eff_Cod) Then
                            HashEffCod.Add(Eff_Cod, Fer_Cod)
                        End If


                    Next

                    If Not HashEffCod Is Nothing AndAlso DT_Effluenti_Dettagli.Rows.Count > 0 Then

                        For Each Eff_Cod In HashEffCod.Keys

                            Dim DrEffCod() As DataRow = DT_Effluenti_Dettagli.Select("eff_cod=" & Eff_Cod)

                            Eff_Des = ""
                            Udm_Cod = 0
                            Udm_Sim = ""
                            Fer_Cod = 0
                            Fer_Des = ""
                            CaricoTot = 0
                            RiempimentoTot = 0
                            AzotoQtaTot = 0
                            EffRif = 0
                            NDistribuito = 0

                            If Not DrEffCod Is Nothing AndAlso DrEffCod.Length > 0 Then
                                Eff_Des = DrEffCod(0).Item("eff_des")
                                Udm_Cod = DrEffCod(0).Item("Udm_Cod")
                                Udm_Sim = DrEffCod(0).Item("Udm_Sim")
                                Fer_Cod = DrEffCod(0).Item("Fer_Cod")
                                Fer_Des = DrEffCod(0).Item("Fer_Des")
                                EffRif = DrEffCod(0).Item("efficienza_riferimento")
                                For Each dr In DrEffCod
                                    CaricoTot += CDec(dr.Item("carico"))
                                    RiempimentoTot += CDec(dr.Item("riempimento"))
                                    AzotoQtaTot += CDec(dr.Item("azoto_qta"))
                                Next
                            End If

                            EffRif = EffRif * 100

                            Dim objRecuperaDati As New AgronicaCorePUA_DAL.PUA_Apporti_DAL
                            FertilizzanteUsato = objRecuperaDati.FertilizzanteDistribuitoIntervallo(objParametriServer, piva, dataInizio, dataFine, Fer_Cod)

                            If FertilizzanteUsato > 0 Then

                                NDistribuito = objRecuperaDati.NDistribuitoIntervallo(objParametriServer, piva, dataInizio, dataFine, Fer_Cod)

                                EffPesataConNDistribuito = objRecuperaDati.EfficienzaPesataConNDistribuito(objParametriServer, piva, dataInizio, dataFine, Fer_Cod)
                                EffCons = EffPesataConNDistribuito / NDistribuito
                                'EffCons = EffCons * 100

                                'converto l'unita di misura (restitutita in kg io l)
                                Select Case Udm_Cod
                                    Case enum_UnitaMisura.Quintali
                                        FertilizzanteUsato = FertilizzanteUsato / 100
                                    Case enum_UnitaMisura.Metri_Cubi
                                        FertilizzanteUsato = FertilizzanteUsato / 1000
                                End Select
                            End If

                            InserisciRiga_DT_Effluenti(DT_Effluenti,
                                    Eff_Cod, Eff_Des, Fer_Cod, Fer_Des, Udm_Cod, Udm_Sim,
                                     CaricoTot, RiempimentoTot,
                                      FertilizzanteUsato, NDistribuito, 0, EffRif, EffCons)

                        Next

                    End If


                    r.RispostaStringa.KendoGridEffluenti = JSON_DataTableEffluenti_Tabella(DT_Effluenti)
                    r.RispostaStringa.KendoGridEffluentiDettagli = JSON_DataTableEffluentiDettagli_Tabella(DT_Effluenti_Dettagli)


                End If

                'loggo l'elaborazione 
                'elimino e riscrivo
                Dim objPuaElaborazione As New AgronicaCorePUA_DAL.Pua_Elaborazione_W
                objPuaElaborazione.Cancella(puaCod, regolamentoCod, piva, saCod, objParametriServer)

                Dim objImpresaC As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim cuaa As String = objImpresaC.Leggi_CUAA(piva, objParametriServer)

                For Each item As PUA_Appezzamento In listPiano

                    Dim intZvn As Integer = 0
                    Dim Conforme As Integer = 0

                    If item.ZVN = True Then
                        intZvn = 1
                    End If

                    'se almeno un valore non è conforme
                    If Not (item.Valutazione_NUtile = 1 Or item.Valutazione_NTotale = 1 Or item.Valutazione_Efficienza = 1) Then
                        Conforme = 1
                    End If

                    objPuaElaborazione.Scrivi(puaCod, regolamentoCod, puaTipo,
                                              item.Id_AnagrafeVincoli,
                                                cuaa, piva, item.Sa_Cod,
                                                item.Campo_Cod, item.appezza, item.id_reg, item.Progetto_Cod,
                                                IIf(item.Veg_Cod < 0, 0, item.Veg_Cod), 0, IIf(item.Veg_Cod < 0, Math.Abs(item.Veg_Cod), 0),
                                                item.Grfi_Cod, item.Grfi_Cod_Concimazione, item.StatoImpiantoCod, item.Ciclo,
                                                item.Resa, item.Resa_Rif, item.FattoreCorrettivo_N, item.Superficie, intZvn,
                                                item.N_Fabbisogno_Database, item.N_Fabbisogno, item.LimiteMas,
                                                item.N_FabbisognoSoddisfatto, item.N_TotaleSoddisfatto, item.N_Zootecnico, item.N_Zootecnico_Letame, item.N_Zootecnico_Liquame,
                                                item.N_BilancioAzotato_Utile, item.N_BilancioAzotato_Totale, item.Indice_Efficienza_Azotata,
                                                item.Valutazione_NUtile, item.Valutazione_NTotale, item.Valutazione_Efficienza,
                                                Conforme, dataInizio, dataFine, objParametriServer)
                Next

            End If
            '----------------------------------------------------------------------------------------------------

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa.KendoGrid = JsonConvert.SerializeObject(listPiano, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function JSON_DataTableEffluenti_Tabella(ByRef DT_Effluenti As DataTable,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("eff_cod", "eff_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("eff_des", "Effluente", "string"))
        l.Add(New ColonneNome("udm_cod", "udm_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim", "Unita Misura", "string") With {._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("carico", "Carico", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("riempimento", "Riempimento a Fine Divieto", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("distribuito", "Effluente Distribuito tal quale", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("azoto_qta", "Azoto Distribuito [kg]", "number") With {._formatNr = "n2", ._sum = True, ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("efficienza_riferimento", "Efficienza Riferimento [%]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("efficienza_conseguita", "Efficienza Conseguita [%]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT_Effluenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function

    Private Shared Function JSON_DataTableEffluentiDettagli_Tabella(ByRef DT_Effluenti As DataTable,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id", "id", "number") With {._hidden = True})
        l.Add(New ColonneNome("flag_provenienzaesterna", "flag_provenienzaesterna", "number") With {._hidden = True})
        l.Add(New ColonneNome("flag_provenienzaesterna_des", "Provenienza Esterna", "string"))
        l.Add(New ColonneNome("eff_cod", "eff_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("eff_des", "Effluente", "string") With {._hidden = True})
        l.Add(New ColonneNome("udm_cod", "udm_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim", "Unita Misura", "string") With {._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("carico", "Carico", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("riempimento", "Riempimento a Fine Divieto", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("azoto_qta", "Azoto [kg]", "number") With {._formatNr = "n2", ._sum = True, ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("azoto_titoli", "Titolo [kg/q]", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT_Effluenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function



    Private Shared Sub Crea_DT_Effluenti(ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("fer_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("fer_des", GetType(String)))
        DT.Columns.Add(New DataColumn("eff_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("eff_des", GetType(String)))
        DT.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("udm_sim", GetType(String)))
        DT.Columns.Add(New DataColumn("carico", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("riempimento", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("distribuito", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("azoto_qta", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("azoto_titoli", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("efficienza_riferimento", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("efficienza_conseguita", GetType(Decimal)))

    End Sub

    Private Shared Sub Crea_DT_Effluenti_Dettagli(ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("id", GetType(Integer)))
        DT.Columns.Add(New DataColumn("eff_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("eff_des", GetType(String)))
        DT.Columns.Add(New DataColumn("fer_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("fer_des", GetType(String)))
        DT.Columns.Add(New DataColumn("flag_provenienzaesterna", GetType(Integer)))
        DT.Columns.Add(New DataColumn("flag_provenienzaesterna_des", GetType(String)))
        DT.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("udm_sim", GetType(String)))
        DT.Columns.Add(New DataColumn("carico", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("riempimento", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("azoto_titoli", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("azoto_qta", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("efficienza_riferimento", GetType(Decimal)))

    End Sub

    Private Shared Sub InserisciRiga_DT_Effluenti(ByRef DT As DataTable,
                                                           ByVal eff_cod As Integer, ByVal eff_des As String, ByVal fer_cod As Integer, ByVal fer_des As String,
                                                               ByVal udm_cod As Integer, ByVal udm_sim As String,
                                                               ByVal carico As Decimal, ByVal riempimento As Decimal, ByVal distribuito As Decimal,
                                                               ByVal azoto_qta As Decimal, ByVal azoto_titoli As Decimal,
                                                                    ByVal efficienza_riferimento As Decimal, ByVal efficienza_conseguita As Decimal)



        Dim Dr As DataRow

        Dr = DT.NewRow

        Dr.Item("eff_cod") = eff_cod
        Dr.Item("eff_des") = eff_des
        Dr.Item("fer_cod") = fer_cod
        Dr.Item("fer_des") = fer_des
        Dr.Item("udm_cod") = udm_cod
        Dr.Item("udm_sim") = udm_sim
        Dr.Item("carico") = carico
        Dr.Item("riempimento") = riempimento
        Dr.Item("distribuito") = distribuito
        Dr.Item("azoto_qta") = azoto_qta
        Dr.Item("azoto_titoli") = azoto_titoli
        Dr.Item("efficienza_riferimento") = efficienza_riferimento
        Dr.Item("efficienza_conseguita") = efficienza_conseguita

        DT.Rows.Add(Dr)

    End Sub

    Private Shared Sub InserisciRiga_DT_Effluenti_Dettagli(ByRef DT As DataTable,
                                                            ByVal id As Integer,
                                                           ByVal eff_cod As Integer, ByVal eff_des As String, ByVal fer_cod As Integer, ByVal fer_des As String,
                                                                  ByVal udm_cod As Integer, ByVal udm_sim As String,
                                                               ByVal carico As Decimal, ByVal riempimento As Decimal,
                                                               ByVal azoto_qta As Decimal, ByVal azoto_titoli As Decimal,
                                                                 ByVal flag_provenienzaesterna As Integer, ByVal flag_provenienzaesterna_des As String,
                                                           ByVal efficienza_riferimento As Decimal)

        Dim Dr As DataRow

        Dr = DT.NewRow
        Dr.Item("id") = id
        Dr.Item("eff_cod") = eff_cod
        Dr.Item("eff_des") = eff_des
        Dr.Item("fer_cod") = fer_cod
        Dr.Item("fer_des") = fer_des
        Dr.Item("udm_cod") = udm_cod
        Dr.Item("udm_sim") = udm_sim
        Dr.Item("carico") = carico
        Dr.Item("riempimento") = riempimento
        Dr.Item("azoto_qta") = azoto_qta
        Dr.Item("azoto_titoli") = azoto_titoli
        Dr.Item("flag_provenienzaesterna") = flag_provenienzaesterna
        Dr.Item("flag_provenienzaesterna_des") = flag_provenienzaesterna_des
        Dr.Item("efficienza_riferimento") = efficienza_riferimento
        DT.Rows.Add(Dr)

    End Sub





#End Region

#Region "Web Method Salvataggio"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva(ByVal dati As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim msgError As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objPD As PUA_Appezzamento = JsonConvert.DeserializeObject(dati, GetType(PUA_Appezzamento))

            xRisp = objPD.Modifica(objParametriServer, msgError)



            If xRisp = True Then

                Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W
                objPUALog.Scrivi(enum_TipoOperazioneDB.Modifica, "Anagrafe_VincoliAgronomici", objPD.Pua_Cod, objPD.Regolamento_Cod, objPD.Id_AnagrafeVincoli, objPD.Piva, objPD.Sa_Cod, objPD.appezza, objPD.id_reg, objPD.Progetto_Cod, Nothing, Nothing, "", objParametriServer)

                'TODO: visto che non ricarica la griglia, devo capire se inizialmente non avevo Id_PartVincoli (e quindi dovevo scrivere la riga)
                ' perché ora la riga l'ho scritta, e quindi ho un Id_PartVincoli e lo devo impostare, altrimenti uno rifà una modifica e viene scritta un'altra riga,
                ' anziché modificare quella scritta nel giro precedente

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                r.RispostaStringa = JsonConvert.SerializeObject(objPD, Formatting.None, serializerSettings)

            Else
                r.RispostaStringa = "{}"
            End If

            r.RispostaOK = xRisp

        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = IIf(msgError <> "", msgError, "Errore Modifica")
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaMultiplo(ByVal dati As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim msgError As String = ""

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            Dim listPD As List(Of PUA_Appezzamento) = JsonConvert.DeserializeObject(dati, GetType(List(Of PUA_Appezzamento)))

            For i As Integer = 0 To listPD.Count - 1
                If i = 0 OrElse xRisp = True Then
                    xRisp = listPD(i).Modifica(objParametriServer, msgError)
                    If xRisp = True Then
                        Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W
                        objPUALog.Scrivi(enum_TipoOperazioneDB.Modifica, "Anagrafe_VincoliAgronomici", listPD(i).Pua_Cod, listPD(i).Regolamento_Cod, listPD(i).Id_AnagrafeVincoli, listPD(i).Piva, listPD(i).Sa_Cod, listPD(i).appezza, listPD(i).id_reg, listPD(i).Progetto_Cod, Nothing, Nothing, "", objParametriServer)
                    End If

                End If
            Next

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)
                r.RispostaStringa = "OK"
            End If

            r.RispostaOK = xRisp

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            r.RispostaOK = False
            r.RispostaStringa = IIf(msgError <> "", msgError, "Errore Modifica")
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaN(ByVal Pua_Tipo As Integer,
                                     ByVal Regolamento_Cod As Integer,
                                     ByVal N_Fabbisogno As Decimal,
                                     ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Appezza As Integer,
                                     ByVal Id_Reg As Integer,
                                     ByVal Progetto_Cod As Integer
                                     ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim objImpProgettiW As New Impresa_Progetti_W

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            Dim objRegImpianti As New Reg_Impianti_Codici_W
            xRisp = objRegImpianti.ModificaxProgetto2(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                                      enum_CodiciAnagrafe.Impianto_LimiteN,
                                                      N_Fabbisogno,
                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                      "",
                                                      objParametriServer)

            If xRisp = True Then

                Dim regCod As Integer = 0
                Select Case Pua_Tipo
                    Case enum_PUA_Tipo.Completo
                        regCod = -1 * Regolamento_Cod
                    Case enum_PUA_Tipo.Semplificato
                        regCod = Regolamento_Cod
                End Select

                xRisp = objImpProgettiW.Modifica_Parametrizzata(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                                                "Regolamento_Concimazioni_Cod",
                                                                regCod,
                                                                "",
                                                                objParametriServer)

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)
            End If


            If xRisp = True Then
                r.RispostaStringa = "OK"
            Else
                r.RispostaStringa = "Errore aggiornamento N Massimo"
            End If

            r.RispostaOK = xRisp

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            r.RispostaOK = False
            r.RispostaStringa = "Errore aggiornamento N Massimo"
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaNOrganico(ByVal N_Fabbisogno As Decimal,
                                             ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Appezza As Integer,
                                             ByVal Id_Reg As Integer,
                                             ByVal Progetto_Cod As Integer
                                             ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objRegImpianti As New Reg_Impianti_Codici_W
            xRisp = objRegImpianti.ModificaxProgetto2(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                                      enum_CodiciAnagrafe.Impianto_LimiteN_Organico,
                                                      N_Fabbisogno,
                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                      "",
                                                      objParametriServer)

            r.RispostaStringa = "OK"
            r.RispostaOK = xRisp

        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = "Errore salvataggio N Fabbisogno"
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Assegna valori a Pua Appezzamento"

    Private Shared Sub AssegnaValoriDefault(ByVal piva As String,
                                            ByVal puaCod As String, ByVal regolamentoCod As String,
                                            ByVal dataInizio As Date, ByVal dataFine As Date, ByVal forzadefault As String,
                                            ByRef listPiano As List(Of PUA_Appezzamento),
                                            ByRef listaPrecessioni As List(Of Precessione),
                                            ByRef ListaPrecessionexSpecie As List(Of PrecessionexSpecie),
                                            ByRef listaUbicazioni As List(Of Ubicazione),
                                            ByRef listaTipiAcqua As List(Of TipoAcqua),
                                            ByRef dictAnalisi As Dictionary(Of Integer, AnalisiDettaglio),
                                            ByVal preimpostaPrecessione As Integer?,
                                            ByVal preimpostaUbicazione As Integer?,
                                            ByRef objParametriServer As AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreParametri)

        Const nomeRoutine = "AssegnaValoriDefault"

        Dim objAnaVincoliW As New Anagrafe_VincoliAgronomici_W
        Dim objAnalisi As New Analisi_Testata_R
        Dim objAnalisiD As New Analisi_Dettagli_R
        Dim objPuaTestata As New AgronicaCorePUA_DAL.PUA_Testata_R

        Try

            'verifica se esistono pua precedenti
            '(se non esistono evito LetturaVincoliAgronomiciDaElementiCollegati)
            Dim DtPuaPrecedenti As DataTable
            DtPuaPrecedenti = objPuaTestata.Leggi(0, 0, piva, AGRODATAINIZIO, AGRODATAFINE, " pua_cod<>" & puaCod & " AND validita_fine <= " & Agro_SQL_SaveDate(dataInizio), " validita_fine desc", objParametriServer)

            Dim objEFOutput As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output
            Dim HashLetami As New Hashtable
            Dim str_FerCod_Letami As String = ""

            If Not DtPuaPrecedenti Is Nothing AndAlso DtPuaPrecedenti.Rows.Count > 0 Then

                Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
                Dim objEFInput As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_input
                objEFInput.Regolamento_Cod = regolamentoCod
                objEFOutput = objPC.EffluentiXFrequenza(objEFInput)

                For Each e As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza In objEFOutput.ListaEffluentiXFrequenza
                    If Not HashLetami.ContainsKey(e.Fer_Cod) Then
                        HashLetami.Add(e.Fer_Cod, "")
                        str_FerCod_Letami &= e.Fer_Cod & ","
                    End If
                Next

                If str_FerCod_Letami <> "" Then
                    str_FerCod_Letami = "(" & Left(str_FerCod_Letami, str_FerCod_Letami.Length - 1) & ")"
                End If

            End If

            Dim i As Integer = 0

            Dim Anno_Precedente_Inizio As Date = DateAdd(DateInterval.Year, -1, dataInizio)
            Dim Anno_Precedente_Fine As Date = DateAdd(DateInterval.Year, -1, dataFine)
            Dim Anno_2Precedente_Inizio As Date = DateAdd(DateInterval.Year, -2, dataInizio)
            Dim Anno_2Precedente_Fine As Date = DateAdd(DateInterval.Year, -2, dataFine)
            Dim Anno_3Precedente_Inizio As Date = DateAdd(DateInterval.Year, -3, dataInizio)
            Dim Anno_3Precedente_Fine As Date = DateAdd(DateInterval.Year, -3, dataFine)

            'se sto riassegnado i default cancello le letamazioni precedenti
            If forzadefault = "true" Then
                Dim objPUA_LetPrec_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W
                Dim bRet As Boolean = objPUA_LetPrec_W.Cancella(CInt(puaCod), regolamentoCod, "", 0, 0, 0, 0, "", objParametriServer)
            End If



            For Each item As PUA_Appezzamento In listPiano

                Dim necessarioUpdate As Boolean = False
                Dim valDef As New ValoriDefault

                'vincoli degli appezzamenti del pua precedente
                Dim Analisi_Testata_Cod_Precedente As Integer = 0
                Dim Ubicazione_Cod_Precedente As Integer = 0
                Dim TipoAcqua_Cod_Precedente As Integer = 0
                Dim N_FertilizzazioniPrecedenti_Precedente As Decimal = 0
                Dim Veg_Cod_Precedente As Integer = 0
                Dim N_Distribuito_Ha As Decimal = 0
                Dim N_Distribuito_HA_Tot As Decimal = 0

                'setto i default solo la prima volta (se i valori sono tutti = 0)
                If (item.PrecessioneCod = 0 And item.UbicazioneCod = 0 And item.TipoAcquaCod = 0 And item.N_FertilizzazioniPrecedenti = 0 And item.AnalisiTestataCod = 0) Or
                    forzadefault = "true" Then

                    If Not DtPuaPrecedenti Is Nothing AndAlso DtPuaPrecedenti.Rows.Count > 0 Then

                        Dim objAVPrec As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

                        'i default del precedente pua li carico solo in fase di creazione del pua
                        If (item.PrecessioneCod = 0 And item.UbicazioneCod = 0 And item.TipoAcquaCod = 0 And item.N_FertilizzazioniPrecedenti = 0 And item.AnalisiTestataCod = 0) Then
                            objAVPrec.LetturaVincoliAgronomiciDaElementiCollegati(item.Piva, item.Sa_Cod, item.appezza, puaCod, regolamentoCod, dataInizio, dataFine,
                                               Veg_Cod_Precedente, Analisi_Testata_Cod_Precedente, Ubicazione_Cod_Precedente, TipoAcqua_Cod_Precedente,
                                                objParametriServer)
                        End If

                        'i default delle letamazioni precedenti da qdc li posso aggiornare anche dopo la creazione del pua
                        If (item.PrecessioneCod = 0 And item.UbicazioneCod = 0 And item.TipoAcquaCod = 0 And item.N_FertilizzazioniPrecedenti = 0 And item.AnalisiTestataCod = 0) Or
                            forzadefault = "true" Then

                            If Not objEFOutput Is Nothing AndAlso objEFOutput.ListaEffluentiXFrequenza.Count > 0 Then

                                'priorità 1
                                'N residuo da fertilizzazioni salvate nel registro
                                objAVPrec.LetturaLetamazioniPrecedentiDaElementiCollegati(regolamentoCod, puaCod, item.Piva, item.Sa_Cod, item.appezza, item.id_reg, item.Progetto_Cod,
                                                                                     dataInizio, dataFine,
                                                                                      objEFOutput, str_FerCod_Letami, N_FertilizzazioniPrecedenti_Precedente,
                                                                                       objParametriServer)
                                'priorità 2
                                'N residuo da letamazioni salvate nel pua precedente
                                If N_FertilizzazioniPrecedenti_Precedente = 0 Then
                                    objAVPrec.LetturaPUALetamazioniPrecedentiDaElementiCollegati(item.Piva, item.Sa_Cod, item.appezza, item.id_reg, item.Progetto_Cod,
                                                                                             puaCod, regolamentoCod, dataInizio, dataFine,
                                                                                              objEFOutput, N_FertilizzazioniPrecedenti_Precedente,
                                                                                               objParametriServer)
                                End If

                            End If
                        End If


                    End If

                    'manca tabella mappatura pre_cod/veg_cod
                    If item.PrecessioneCod = 0 Then
                        If Veg_Cod_Precedente > 0 Then
                            valDef.PrecessioneCod = GetPrecessioneDaSpecie(ListaPrecessionexSpecie, Veg_Cod_Precedente)
                            If valDef.PrecessioneCod > 0 Then
                                necessarioUpdate = True
                            End If
                        Else
                            If Not preimpostaPrecessione Is Nothing Then
                                necessarioUpdate = True
                                valDef.PrecessioneCod = preimpostaPrecessione
                            End If
                        End If
                    End If

                    If item.UbicazioneCod = 0 Then
                        If Ubicazione_Cod_Precedente > 0 Then
                            necessarioUpdate = True
                            valDef.UbicazioneCod = Ubicazione_Cod_Precedente
                        Else
                            If Not preimpostaUbicazione Is Nothing Then
                                necessarioUpdate = True
                                valDef.UbicazioneCod = preimpostaUbicazione
                            End If
                        End If
                    End If

                    If item.TipoAcquaCod = 0 And TipoAcqua_Cod_Precedente > 0 Then
                        necessarioUpdate = True
                        valDef.TipoAcquaCod = TipoAcqua_Cod_Precedente
                    End If

                    If (item.N_FertilizzazioniPrecedenti = 0 And N_FertilizzazioniPrecedenti_Precedente > 0) Or forzadefault = "true" Then
                        necessarioUpdate = True
                        valDef.N_FertilizzazioniPrecedenti = N_FertilizzazioniPrecedenti_Precedente
                    End If

                    Dim preimpostaAnalisi As Integer? = Nothing

                    If item.AnalisiTestataCod = 0 Then

                        If Analisi_Testata_Cod_Precedente > 0 Then

                            necessarioUpdate = True
                            valDef.AnalisiCod = Analisi_Testata_Cod_Precedente

                        Else

                            Dim strAnalisiCatasto As String = ""
                            strAnalisiCatasto = PUA_ParticellaVincoloAgronomico.EstrapolaStringaFiltroAnalisiCatasto(piva, item.ParticelleVincoli)

                            '(22/11/2019) aggiunto default analisi più recente
                            Dim strAnalisi As String = ""
                            Dim numAnalisi As Integer = 0
                            Dim AnalisiCodPiuRecente As Integer = 0
                            strAnalisi = objAnalisi.OttieniFiltroAnalisi(piva, item.Sa_Cod, item.Campo_Cod,
                                                                 item.appezza, item.id_reg,
                                                                 strAnalisiCatasto,
                                                                 numAnalisi, AnalisiCodPiuRecente,
                                                                 objParametriServer)
                            If AnalisiCodPiuRecente <> 0 Then
                                preimpostaAnalisi = AnalisiCodPiuRecente
                            End If

                            If item.AnalisiTestataCod = 0 AndAlso Not preimpostaAnalisi Is Nothing Then
                                necessarioUpdate = True
                                valDef.AnalisiCod = preimpostaAnalisi
                            End If


                        End If
                    End If

                    If necessarioUpdate = True Then

                        Dim xRisp As Boolean = False
                        xRisp = objAnaVincoliW.ModificaPuntuale(item.Id_AnagrafeVincoli,
                                                            objParametriServer,
                                                            valDef.AnalisiCod,
                                                            valDef.PrecessioneCod,
                                                            valDef.UbicazioneCod,
                                                            valDef.TipoAcquaCod,
                                                            valDef.N_FertilizzazioniPrecedenti)


                        If xRisp = True Then

                            If Not valDef.PrecessioneCod Is Nothing Then
                                item.PrecessioneCod = valDef.PrecessioneCod
                                item.PrecessioneDes = GetPrecessioneDes(listaPrecessioni, item.PrecessioneCod)
                            End If

                            If Not valDef.AnalisiCod Is Nothing Then
                                item.AnalisiTestataCod = valDef.AnalisiCod

                                If dictAnalisi.ContainsKey(item.AnalisiTestataCod) Then
                                    item.AnalisiTestataDes = dictAnalisi(item.AnalisiTestataCod).Descrizione
                                    item.Argilla = dictAnalisi(item.AnalisiTestataCod).Argilla
                                    item.Sabbia = dictAnalisi(item.AnalisiTestataCod).Sabbia
                                    item.So = dictAnalisi(item.AnalisiTestataCod).So
                                Else

                                    item.AnalisiTestataDes = objAnalisi.Descrizione_from_TestataCod(item.AnalisiTestataCod, objParametriServer)

                                    objAnalisiD.ValorizzaDettagliAnalisiTerreno(item.AnalisiTestataCod,
                                                                            item.Sabbia, item.Argilla,
                                                                            item.So,
                                                                            objParametriServer)

                                    dictAnalisi.Add(item.AnalisiTestataCod,
                                                New AnalisiDettaglio With {
                                                   .Descrizione = item.AnalisiTestataDes,
                                                   .Argilla = item.Argilla,
                                                   .Sabbia = item.Sabbia,
                                                   .So = item.So
                                                   })
                                End If

                            End If

                            If Not valDef.UbicazioneCod Is Nothing Then
                                item.UbicazioneCod = valDef.UbicazioneCod
                                item.UbicazioneDes = GetUbicazioneDes(listaUbicazioni, item.UbicazioneCod)
                            End If

                            If Not valDef.TipoAcquaCod Is Nothing Then
                                item.TipoAcquaCod = valDef.TipoAcquaCod
                                item.TipoAcquaDes = GetTipoAcquaDes(listaTipiAcqua, item.TipoAcquaCod)
                            End If

                            If Not valDef.N_FertilizzazioniPrecedenti Is Nothing Then
                                item.N_FertilizzazioniPrecedenti = valDef.N_FertilizzazioniPrecedenti
                            End If

                        End If

                    End If

                End If

                '(19/08/2020 fede) commentato per non farlo fare tutte le volte

                'necessarioUpdate = False

                'If item.N_FertilizzazioniPrecedenti = 0 Then

                '    If Not DtPuaPrecedenti Is Nothing AndAlso DtPuaPrecedenti.Rows.Count > 0 Then

                '        Dim objAVPrec As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

                '        If Not objEFOutput Is Nothing AndAlso objEFOutput.ListaEffluentiXFrequenza.Count > 0 Then

                '            'priorità 1
                '            'N residuo da fertilizzazioni salvate nel registro
                '            objAVPrec.LetturaLetamazioniPrecedentiDaElementiCollegati(regolamentoCod, puaCod, item.Piva, item.Sa_Cod, item.appezza, item.id_reg, item.Progetto_Cod,
                '                                                                 dataInizio, dataFine,
                '                                                                  objEFOutput, str_FerCod_Letami, N_FertilizzazioniPrecedenti_Precedente,
                '                                                                   objParametriServer)
                '            'priorità 2
                '            'N residuo da letamazioni salvate nel pua precedente
                '            If N_FertilizzazioniPrecedenti_Precedente = 0 Then
                '                objAVPrec.LetturaPUALetamazioniPrecedentiDaElementiCollegati(item.Piva, item.Sa_Cod, item.appezza, item.id_reg, item.Progetto_Cod,
                '                                                                         puaCod, regolamentoCod, dataInizio, dataFine,
                '                                                                          objEFOutput, N_FertilizzazioniPrecedenti_Precedente,
                '                                                                           objParametriServer)
                '            End If

                '        End If

                '    End If

                '    If item.N_FertilizzazioniPrecedenti = 0 And N_FertilizzazioniPrecedenti_Precedente > 0 Then
                '        necessarioUpdate = True
                '        valDef.N_FertilizzazioniPrecedenti = N_FertilizzazioniPrecedenti_Precedente
                '    End If

                '    If necessarioUpdate = True Then

                '        Dim xRisp As Boolean = False
                '        xRisp = objAnaVincoliW.ModificaPuntuale(item.Id_AnagrafeVincoli,
                '                                            objParametriServer,
                '                                            Nothing,
                '                                            Nothing,
                '                                            Nothing,
                '                                            Nothing,
                '                                            valDef.N_FertilizzazioniPrecedenti)


                '        If xRisp = True Then

                '            If Not valDef.N_FertilizzazioniPrecedenti Is Nothing Then
                '                item.N_FertilizzazioniPrecedenti = valDef.N_FertilizzazioniPrecedenti
                '            End If

                '        End If

                '    End If

                'End If





                i += 1

            Next


        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub

    Private Shared Sub AssegnaSemafori(ByRef objPiano As PUA_Appezzamento)

        ' Bilancio N Utile 
        objPiano.N_BilancioAzotato_Utile = objPiano.N_FabbisognoSoddisfatto - objPiano.N_Fabbisogno

        Select Case objPiano.N_BilancioAzotato_Utile
            Case <= 0
                objPiano.Valutazione_NUtile = SemaforoValutazione.Verde
            Case 0 To 30
                objPiano.Valutazione_NUtile = SemaforoValutazione.Arancione
            Case Else
                objPiano.Valutazione_NUtile = SemaforoValutazione.Rosso
        End Select


        ' Bilancio N Totale
        objPiano.N_BilancioAzotato_Totale = objPiano.N_TotaleSoddisfatto - objPiano.N_Fabbisogno

        Select Case objPiano.N_BilancioAzotato_Totale
            Case <= 0
                objPiano.Valutazione_NTotale = SemaforoValutazione.Verde
            Case 0 To 50
                objPiano.Valutazione_NTotale = SemaforoValutazione.Arancione
            Case Else
                objPiano.Valutazione_NTotale = SemaforoValutazione.Rosso
        End Select

        'valutazione efficienza
        If objPiano.N_TotaleSoddisfatto <> 0 Then
            objPiano.Indice_Efficienza_Azotata = objPiano.Assorbimento / objPiano.N_TotaleSoddisfatto * 100
        End If

        Select Case objPiano.Indice_Efficienza_Azotata
            Case 0
                objPiano.Valutazione_Efficienza = SemaforoValutazione.Verde
            Case >= 50
                objPiano.Valutazione_Efficienza = SemaforoValutazione.Verde
            Case Else
                objPiano.Valutazione_Efficienza = SemaforoValutazione.Rosso
        End Select

    End Sub

#End Region

#Region "Scrivi Dati"

    Private Shared Sub ValorizzaResaDaMas(ByRef objPiano As PUA_Appezzamento,
                                          ByRef objParametriServer As AgronicaCoreParametri)

        Dim objImpreseProgettiW As New Impresa_Progetti_W

        If objPiano.Resa <> 0 Then
            'Viene salvata direttamente su DB la resa
            Dim xRisp As Boolean = False
            xRisp = objImpreseProgettiW.Modifica_Parametrizzata(objPiano.Piva, objPiano.Sa_Cod,
                                                                objPiano.appezza, objPiano.id_reg,
                                                                objPiano.Progetto_Cod,
                                                                "Produzione_Prevista", objPiano.Resa,
                                                                "", objParametriServer)

        End If

    End Sub

    Private Shared Sub ValorizzaFinalitaRer(ByRef objPiano As PUA_Appezzamento,
                                            ByRef objParametriServer As AgronicaCoreParametri)

        Dim objRegImpianti As New Reg_Impianti_Codici_W

        If objPiano.Grfi_Cod_Concimazione <> 0 Then

            Dim xRisp As Boolean = False

            xRisp = objRegImpianti.ModificaxProgetto2(objPiano.Piva, objPiano.Sa_Cod, objPiano.appezza,
                                                      objPiano.id_reg, objPiano.Progetto_Cod,
                                                      enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                      objPiano.Grfi_Cod_Concimazione,
                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                      "", objParametriServer)

        End If

    End Sub

#End Region

#Region "Zone Vulnerabili"

    Private Shared Function IsZonaVulnerabile(ByVal prov As String, ByVal com As String,
                                              ByVal sezione As String, ByVal foglio As Integer,
                                              ByVal numero As Integer, ByVal subalterno As String,
                                              ByRef dtZvnServer As DataTable, ByRef dtZvnMetaschema As DataTable
                                              ) As Boolean

        Dim flagVulnerabile As Boolean = False

        If Not dtZvnServer Is Nothing AndAlso dtZvnServer.Rows.Count > 0 Then

            flagVulnerabile = dtZvnServer.AsEnumerable().Any(Function(x) x.Item("PROV") = prov AndAlso
                                                                         x.Item("COM") = com AndAlso
                                                                         x.Item("SEZIONE") = sezione AndAlso
                                                                         x.Item("FOGLIO") = foglio AndAlso
                                                                         x.Item("NUMERO") = numero AndAlso
                                                                         x.Item("SUBALTERNO") = subalterno)

        End If

        If flagVulnerabile = False AndAlso
           Not dtZvnMetaschema Is Nothing AndAlso dtZvnMetaschema.Rows.Count > 0 Then

            'Se non l'ho trovato nel server lo cerco nel metaschema

            flagVulnerabile = dtZvnMetaschema.AsEnumerable().Any(Function(x) x.Item("PROV") = prov AndAlso
                                                                             x.Item("COM") = com AndAlso
                                                                             x.Item("SEZIONE") = sezione AndAlso
                                                                             x.Item("FOGLIO") = foglio AndAlso
                                                                             x.Item("NUMERO") = numero AndAlso
                                                                             x.Item("SUBALTERNO") = subalterno)

        End If

        Return flagVulnerabile

    End Function

    Private Shared Sub LeggiZoneVulnerabili(ByVal regCod As Integer, ByVal dataInizio As Date, ByVal dataFine As Date,
                                            ByRef dtZvnServer As DataTable,
                                            ByRef dtZvnMetaschema As DataTable,
                                            ByRef objParametriServer As AgronicaCoreParametri)

        Dim filtroDate As String = " (Validita_inizio <= " & Agro_SQL_SaveDate(dataFine) & ")  AND     (Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio) & ") "

        'lettura zone vulnerabili
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        dtZvnServer = objPV.Leggi(-17,
                                  "", "", "", 0, 0, "",
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  filtroDate,
                                  "", objParametriServer)


        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Try
            'TODO: introdurre lettura zone vulnerabili PUA tramite WebService e non in locale?!?
            Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
            dtZvnMetaschema = objPVF.Leggi("", "", "", 0, 0, "",
                                           regCod,
                                           " Fascia_Cod <>0 AND " & filtroDate,
                                           "", objParametriServer)
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Get Liste tabelle da Web Service"

    Private Shared Function GetListaStatiImpiantiDes(ByVal regolamentoCod As Integer, ByVal vegCod As Integer) As List(Of Fase)

        Dim objParametriIngresso As New PianoConcimazione_FasiCicloColturale_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod = vegCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.FasiCicloColturaleStatoImpianto(objParametriIngresso)

        Return objParametriUscita.ListaFasi

    End Function

    Private Shared Function GetListaPrecessioniDes(ByVal regolamentoCod As Integer, ByVal puaTipo As Integer) As List(Of Precessione)

        Dim objParametriIngresso As New PianoConcimazione_Precessione_input With {
            .Regolamento_Cod = regolamentoCod,
            .PUA_Tipo = puaTipo,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Precessione(objParametriIngresso)

        Return objParametriUscita.ListaPrecessione

    End Function

    Private Shared Function GetListaPrecessionixSpecieDes(ByVal regolamentoCod As Integer) As List(Of PrecessionexSpecie)

        Dim objParametriIngresso As New PianoConcimazione_PrecessionexSpecie_input With {
            .Regolamento_Cod = regolamentoCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_PrecessionexSpecie_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.PrecessionexSpecie(objParametriIngresso)

        Return objParametriUscita.ListaPrecessionexSpecie

    End Function





    Private Shared Function GetListaUbicazioniDes(ByVal regolamentoCod As Integer) As List(Of Ubicazione)

        Dim objParametriIngresso As New PianoConcimazione_Ubicazione_input With {
            .Regolamento_Cod = regolamentoCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Ubicazione(objParametriIngresso)

        Return objParametriUscita.ListaUbicazione

    End Function

    Private Shared Function GetListaTipiAcquaDes(ByVal regolamentoCod As Integer) As List(Of TipoAcqua)

        Dim objParametriIngresso As New PianoConcimazione_TipoAcqua_input With {
            .Regolamento_Cod = regolamentoCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.TipoAcqua(objParametriIngresso)

        Return objParametriUscita.ListaTipiAcqua

    End Function

    Private Shared Function GetListaFinalitaRer(ByVal regolamentoCod As Integer, ByVal vegcod_elenco As String) As List(Of Finalita)

        Dim objParametriIngresso As New PianoConcimazione_FinalitaRER_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod_Elenco = vegcod_elenco
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscita As PianoConcimazione_FinalitaRER_output = objPC_WS.FinalitaRER(objParametriIngresso)

        Return objParametriUscita.ListaFinalita

    End Function

    Private Shared Function GetListaCoefficienteB(ByVal regolamentoCod As Integer, ByVal vegcod_elenco As String) As List(Of PUA_CoefficienteB)

        Dim objParametriIngresso As New PUA_CoefficienteB_Coltura_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod_Elenco = vegcod_elenco
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscita As PUA_CoefficienteB_Coltura_output = objPC_WS.CoefficienteB_Coltura(objParametriIngresso)

        Return objParametriUscita.ListaCoefficienteB

    End Function

    Private Shared Function GetListaReseMas(ByVal regolamentoCod As Integer, ByVal vegcod_elenco As String) As List(Of PianoConcimazione_LimiteMAS_output)

        Dim objParametriIngressoMAS As New PianoConcimazione_LimiteMAS_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod_Elenco = vegcod_elenco
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscitaMASElenco As PianoConcimazione_LimiteMAS_Elenco_output = objPC_WS.LimiteMASElenco(objParametriIngressoMAS)

        Return objParametriUscitaMASElenco.ListaLimitiMas

    End Function

    'Private Shared Function GetParametriRegolamento(ByVal regolamentoCod As Integer, ByVal Anno_Inizio As Integer, ByVal Anno_fine As Integer, ByRef Data_Competenza_Inizio As Date, ByRef Data_Competenza_Fine As Date)

    '    Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
    '    objParametriIngresso.Regolamento_Cod = regolamentoCod

    '    Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
    '    Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
    '    objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

    '    Dim Competenza_Inizio_Giorno As String
    '    Dim Competenza_Fine_Giorno As String
    '    Dim Competenza_Inizio_Mese As String
    '    Dim Competenza_Fine_Mese As String

    '    If Not objParametriUscita Is Nothing AndAlso Not objParametriUscita.ListaRegolamenti Is Nothing AndAlso objParametriUscita.ListaRegolamenti.Count > 0 AndAlso Not objParametriUscita.ListaRegolamenti(0).ListaParametri Is Nothing Then

    '        Dim objIG = (From l In objParametriUscita.ListaRegolamenti(0).ListaParametri Where l.Codice = enum_PUAParametri.Competenza_Inizio_Giorno Select l).FirstOrDefault()
    '        If Not objIG Is Nothing Then
    '            Competenza_Inizio_Giorno = objIG.Valore
    '        End If
    '        Dim objFG = (From l In objParametriUscita.ListaRegolamenti(0).ListaParametri Where l.Codice = enum_PUAParametri.Competenza_Fine_Giorno Select l).FirstOrDefault()
    '        If Not objFG Is Nothing Then
    '            Competenza_Fine_Giorno = objFG.Valore
    '        End If
    '        Dim objIM = (From l In objParametriUscita.ListaRegolamenti(0).ListaParametri Where l.Codice = enum_PUAParametri.Competenza_Inizio_Mese Select l).FirstOrDefault()
    '        If Not objIM Is Nothing Then
    '            Competenza_Inizio_Mese = objIM.Valore
    '        End If
    '        Dim objFM = (From l In objParametriUscita.ListaRegolamenti(0).ListaParametri Where l.Codice = enum_PUAParametri.Competenza_Fine_Mese Select l).FirstOrDefault()
    '        If Not objFM Is Nothing Then
    '            Competenza_Fine_Mese = objFM.Valore
    '        End If

    '        If IsNumeric(Competenza_Inizio_Giorno) And IsNumeric(Competenza_Inizio_Mese) Then
    '            Data_Competenza_Inizio = CDate(Right("00" & CInt(Competenza_Inizio_Giorno), 2) & "/" & Right("00" & CInt(Competenza_Inizio_Mese), 2) & "/" & Anno_Inizio)
    '        End If
    '        If IsNumeric(Competenza_Fine_Giorno) And IsNumeric(Competenza_Fine_Mese) Then
    '            Data_Competenza_Fine = CDate(Right("00" & CInt(Competenza_Fine_Giorno), 2) & "/" & Right("00" & CInt(Competenza_Fine_Mese), 2) & "/" & Anno_fine)
    '        End If

    '    End If

    'End Function

#End Region

#Region "Ricava Descrizioni"

    Private Shared Function GetStatoImpiantoDes(ByRef dictStatoImpianto As Dictionary(Of Integer, List(Of Fase)),
                                                ByVal regolamentoCod As Integer,
                                                ByVal vegCod As Integer,
                                                ByVal statoImpiantoCod As Integer
                                                ) As String

        Dim descrizione As String = ""

        Dim listaStatiImpianti As List(Of Fase)
        If dictStatoImpianto.ContainsKey(vegCod) Then
            'avevo già letto gli stati, li recupero dal dizionario senza rileggere
            listaStatiImpianti = dictStatoImpianto(vegCod)
        Else
            'non avevo ancora letto gli stati per questo veg_cod, quindi li leggo
            listaStatiImpianti = GetListaStatiImpiantiDes(regolamentoCod, vegCod)
            dictStatoImpianto.Add(vegCod, listaStatiImpianti)
        End If

        If Not listaStatiImpianti Is Nothing AndAlso listaStatiImpianti.Count > 0 Then
            Dim obj = (From l In listaStatiImpianti Where l.Codice = statoImpiantoCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetPrecessioneDes(ByRef listaPrecessioni As List(Of Precessione), ByVal precessioneCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaPrecessioni Is Nothing AndAlso listaPrecessioni.Count > 0 Then
            Dim obj = (From l In listaPrecessioni Where l.Codice = precessioneCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetPrecessioneDaSpecie(ByRef listaPrecessionixSpecie As List(Of PrecessionexSpecie), ByVal vegCod As Integer) As Integer

        Dim precessioneCod As Integer = 0

        If Not listaPrecessionixSpecie Is Nothing AndAlso listaPrecessionixSpecie.Count > 0 Then
            Dim obj = (From l In listaPrecessionixSpecie Where l.Veg_Cod = vegCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                precessioneCod = obj.Codice
            End If
        End If

        Return precessioneCod

    End Function

    'Private Shared Function GetFrequenzaDes(ByRef listaFrequenze As List(Of Frequenza), ByVal frequenzaCod As Integer) As String

    '    Dim descrizione As String = ""

    '    If Not listaFrequenze Is Nothing AndAlso listaFrequenze.Count > 0 Then
    '        Dim obj = (From l In listaFrequenze Where l.Codice = frequenzaCod Select l).FirstOrDefault()

    '        If Not obj Is Nothing Then
    '            descrizione = obj.Descrizione
    '        End If
    '    End If

    '    Return descrizione

    'End Function

    'Private Shared Function GetFertOrganicoDes(ByRef listaFertOrganici As List(Of PUA_EffluentiXFrequenza), ByVal fertOrganicoCod As Integer) As String

    '    Dim descrizione As String = ""

    '    If Not listaFertOrganici Is Nothing AndAlso listaFertOrganici.Count > 0 Then
    '        Dim obj = (From l In listaFertOrganici Where l.Eff_Cod = fertOrganicoCod Select l).FirstOrDefault()

    '        If Not obj Is Nothing Then
    '            descrizione = obj.Eff_Des
    '        End If
    '    End If

    '    Return descrizione

    'End Function

    Private Shared Function GetUbicazioneDes(ByRef listaUbicazioni As List(Of Ubicazione), ByVal ubicazioneCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaUbicazioni Is Nothing AndAlso listaUbicazioni.Count > 0 Then
            Dim obj = (From l In listaUbicazioni Where l.Codice = ubicazioneCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetTipoAcquaDes(ByRef listaTipiAcqua As List(Of TipoAcqua), ByVal tipoAcquaCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaTipiAcqua Is Nothing AndAlso listaTipiAcqua.Count > 0 Then
            Dim obj = (From l In listaTipiAcqua Where l.Codice = tipoAcquaCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetFinalitaRerDes(ByVal listaFinalita As List(Of Finalita), ByVal Grfi_Cod_Rer As Integer) As String

        Dim descrizione As String = ""

        If Not listaFinalita Is Nothing AndAlso listaFinalita.Count > 0 Then
            Dim obj = (From l In listaFinalita Where l.Codice = Grfi_Cod_Rer Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

#End Region

#Region "Ricava altri dati"

    Private Shared Sub GetResaMas(ByVal listaReseMas As List(Of PianoConcimazione_LimiteMAS_output),
                                  ByVal listaFinalita As List(Of Finalita),
                                  ByVal listaCoefficienteB As List(Of PUA_CoefficienteB),
                                  ByVal Veg_Cod As Integer, ByVal Grfi_Cod_Gias As Integer,
                                  ByVal Grfi_Cod_Concimazione As Integer, ByVal Stato_Cod As Integer,
                                  ByRef ResaDB As Decimal, ByRef MasDB As Decimal, ByRef Grfi_Cod_DB As Integer, ByRef FattoreCorrettivo_N_DB As Decimal)


        If Not listaReseMas Is Nothing AndAlso listaReseMas.Count > 0 Then

            If Stato_Cod = 0 Then
                Stato_Cod = 102
            End If

            Dim obj As PianoConcimazione_LimiteMAS_output
            If Grfi_Cod_Concimazione > 0 Then
                obj = (From l In listaReseMas
                       Where l.Veg_Cod = Veg_Cod And
                             l.Grfi_Cod = Grfi_Cod_Concimazione And
                             l.Stato_Cod = Stato_Cod
                       Select l).FirstOrDefault()
            Else
                Grfi_Cod_DB = GetFinalitaRer(listaFinalita, listaCoefficienteB, Veg_Cod, Grfi_Cod_Gias)
                Dim Grfi_Cod_DB_Tmp As Integer = Grfi_Cod_DB
                obj = (From l In listaReseMas
                       Where l.Veg_Cod = Veg_Cod And
                             l.Grfi_Cod = Grfi_Cod_DB_Tmp And
                             l.Stato_Cod = Stato_Cod
                       Select l).FirstOrDefault()
            End If

            'Dim obj = (From l In listaReseMas Where l.Veg_Cod = Veg_Cod And l.Grfi_Cod = Grfi_Cod And l.Stato_Cod = Stato_Cod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                ResaDB = obj.Resa
                MasDB = obj.N
                FattoreCorrettivo_N_DB = obj.FattoreCorrettivo_N
            End If
        End If

    End Sub

    Private Shared Function GetFinalitaRer(ByVal listaFinalita As List(Of Finalita),
                                           ByVal listaCoefficienteB As List(Of PUA_CoefficienteB),
                                           ByVal Veg_Cod As Integer, ByVal Grfi_Cod_Gias As Integer
                                           ) As Integer

        Dim GrfiCodPua As Integer = 0

        If Not listaFinalita Is Nothing AndAlso listaFinalita.Count > 0 Then

            Dim objListaGrfiCodPua As List(Of Finalita)
            objListaGrfiCodPua = (From l In listaFinalita Where l.CodiceSpecie = Veg_Cod Select l).ToList()

            If Not objListaGrfiCodPua Is Nothing AndAlso objListaGrfiCodPua.Count > 0 Then

                For Each objGrfiCodPua As Finalita In objListaGrfiCodPua

                    Dim objGrfiCodgias = (From g In objGrfiCodPua.ListaFinalitaGias Where g.Codice = Grfi_Cod_Gias Select g).FirstOrDefault

                    If Not objGrfiCodgias Is Nothing Then

                        'verifico che esista il coeff_b
                        If Not listaCoefficienteB Is Nothing AndAlso listaCoefficienteB.Count > 0 Then
                            Dim obj = (From l In listaCoefficienteB Where l.Veg_Cod = Veg_Cod And l.Grfi_Cod_RER = objGrfiCodPua.Codice Select l).FirstOrDefault()
                            If Not obj Is Nothing Then
                                GrfiCodPua = objGrfiCodPua.Codice
                            End If
                        End If

                        Exit For

                    End If

                Next

            End If

        End If

        Return GrfiCodPua

    End Function

    Private Shared Function GetCoefficienteB(ByVal listaCoefficienteB As List(Of PUA_CoefficienteB),
                                             ByVal Veg_Cod As Integer, ByVal Grfi_Cod_RER As Integer
                                             ) As Decimal

        Dim B_Perc As Decimal = 0

        If Not listaCoefficienteB Is Nothing AndAlso listaCoefficienteB.Count > 0 Then
            Dim obj = (From l In listaCoefficienteB Where l.Veg_Cod = Veg_Cod And l.Grfi_Cod_RER = Grfi_Cod_RER Select l).FirstOrDefault()
            If Not obj Is Nothing Then
                B_Perc = obj.B_Perc
            End If
        End If

        Return B_Perc

    End Function


#End Region


    <WebMethod(EnableSession:=True)>
    Public Shared Function ApriVerificaIndiciBilancio(ByVal PC_Testata_Cod As Integer,
                                                      ByVal PC_Dettagli_PIVA As String,
                                                      ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                                      ByVal Regolamento_Cod As Integer,
                                                      ByVal Ricetta_Cod As Integer,
                                                        ByVal blocco_flag As Integer,
                                                        ByVal data_inizio As String,
                                                        ByVal data_fine As String,
                                                      ByVal modalita As enum_PUA_Modalita
                                                      ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            If data_inizio = "" Then
                data_inizio = AGRODATAINIZIO
            End If

            If data_fine = "" Then
                data_fine = AGRODATAFINE
            End If

            Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
            Dim dt As DataTable = objRicette.Leggi_xGriglia(Ricetta_Cod, PC_Dettagli_PIVA, 0, enum_TipoRicetta.PianoDistribuzionePua,
                                                                0, PC_Testata_Cod, data_inizio, data_fine,
                                                                "", "", objParametri_Server)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Dim urlTarget As String = GetUrlPuaPianoDistribuzione(enum_TipoOperazioneDB.Modifica,
                                                                          modalita,
                                                                          PC_Tipo, PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Cod,
                                                                          CDate(dt.Rows(0).Item("Validita_Inizio")), CDate(dt.Rows(0).Item("Validita_Fine")),
                                                                          Ricetta_Cod, blocco_flag,
                                                                          objParametri_Server)

                r.RispostaOK = True
                r.RispostaStringa = urlTarget
                r.ParametroDue_stringa = CStr(dt.Rows.Count)

            Else
                r.RispostaOK = False
                r.Errore = "Non è stato ancora creato un Piano Distribuzione"
            End If



        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function GetUrlPuaPianoDistribuzione(ByVal tipoOp As enum_TipoOperazioneDB,
                                                        ByVal modalita As enum_PUA_Modalita,
                                                        ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                                        ByVal PC_Dettagli_PIVA As String,
                                                        ByVal PC_Testata_Cod As Integer,
                                                        ByVal Regolamento_Cod As Integer,
                                                        ByVal Data_Inizio As Date, ByVal Data_Fine As Date,
                                                        ByVal Ricetta_Cod As Integer,
                                                        ByVal Blocco_Flag As Integer,
                                                        ByRef objParametri_Server As AgronicaCoreParametri
                                                        ) As String
        Dim urlTarget As String

        urlTarget = "PUA_Piano_Distribuzione.aspx?tipo=" + Stringa_Codifica(CStr(PC_Tipo), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&o=" & Stringa_Codifica(CStr(tipoOp), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&m=" & Stringa_Codifica(CStr(modalita), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&p=" & Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&q=" & Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&r=" & Stringa_Codifica(CStr(Regolamento_Cod), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_da=" & Stringa_Codifica(CStr(Data_Inizio), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_a=" & Stringa_Codifica(CStr(Data_Fine), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&blocco_flag=" & Stringa_Codifica(CStr(Blocco_Flag), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&ric=" & Stringa_Codifica(CStr(Ricetta_Cod), AgroKey_EncoderDecoder, objParametri_Server)

        Return urlTarget

    End Function


    Private Class ValoriDefault
        Public Property PrecessioneCod As Integer?
        Public Property FrequenzaCod As Integer?
        Public Property FertOrganicoCod As Integer?
        Public Property UbicazioneCod As Integer?
        Public Property AnalisiCod As Integer?
        Public Property TipoAcquaCod As Integer?
        Public Property N_FertilizzazioniPrecedenti As Decimal?

    End Class

    Private Class AnalisiDettaglio
        Public Property Descrizione As String
        Public Property Argilla As Decimal
        Public Property Sabbia As Decimal
        Public Property So As Decimal
    End Class

End Class


Public Class Piano_Distribuzione

    Public KendoGrid As String
    Public KendoGridFiglia As String

    Public KendoGridEffluenti As String
    Public KendoGridEffluentiDettagli As String

End Class