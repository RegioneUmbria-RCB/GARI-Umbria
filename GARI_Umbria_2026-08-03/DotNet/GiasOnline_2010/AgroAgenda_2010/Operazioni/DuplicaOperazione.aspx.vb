Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtility

Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp

Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL

Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgroAgenda_2010.Resources

Public Class DuplicaOperazione
    Inherits System.Web.UI.Page



    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub



    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda
    Dim nrTotaleImpiantiOperazione As Integer

    Dim PaginaRitorno As String = "../Menu/Menu.aspx"

    Dim e_SortDirection As Integer
    Dim m_strSortExp As String


    Dim frm_Piva As String
    Dim frm_SaCod As Integer
    Dim frm_Appezza() As Integer
    Dim frm_IdReg() As Integer
    Dim frm_ValiditaInizio() As Date
    Dim frm_ValiditaFine() As Date

    Dim frm_IdAgenda As Integer
    Dim frm_DescrizioneOperazione As String
    Dim frm_DataOperazione As Date
    Dim frm_NoteIntervento As String

    Dim frm_BaseCode As Integer
    Dim frm_TopCode As Integer




    Private Sub DuplicaOperazione_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.Master.flag_pag_Operazione = True
        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub



    ''' <summary>
    ''' Bottone di annullamento
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then
            Dim TargetRedirect As String = ""
            MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                          Enum_SiteRedirector.GiasNG,
                                                          enum_PagineGiasNG.Pagina_Menu_Agenda,
                                                          TargetRedirect,
                                                          objParametri_Server)
            Response.Redirect(TargetRedirect)
        End If

        objParametriAgenda.Svuota_DatiOperazione()

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
            PaginaRitorno = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        Response.Redirect(PaginaRitorno)

    End Sub


    Public ids As String = ""
    Public raccoglitori_cod As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
        objParametriAgenda = New ParametriAgenda
        nrTotaleImpiantiOperazione = objParametriAgenda.Impianti.Count

        hd_isFertirrigazione.Value = False
        If objParametriAgenda.Lav_Cod = CStr(LAVCOD_FERTIRRIGAZIONE) Then
            hd_isFertirrigazione.Value = True
        Else
            Dim agendaIdsAsString = Request.QueryString("id")
            Dim idAgendaList As List(Of Integer) = agendaIdsAsString.Split(","c).Select(Function(s) Integer.Parse(s.Trim())).Where(Function(s) s <> -1).ToList()

            If idAgendaList.Count > 1 Then
                Dim agendaR As AgronicaCoreContabDAL.Agenda_R = New AgronicaCoreContabDAL.Agenda_R
                Dim lavCods = agendaR.LeggiDistinctLavCod(idAgendaList, objParametri_Server)
                If (lavCods.Contains(LAVCOD_FERTIRRIGAZIONE)) Then
                    hd_isFertirrigazione.Value = True
                End If
            End If
        End If

        InizializzaScript()

        ' VAnni: 18/3/2017: questo perchè le pagine di filtro impianto eseguono un redirect e quindi occorre re-inizializzare la pagina.
        '       la querystring è disponibile solo al primo passaggio... ri-allineo la situazione solo se non mi trovo in un postback...

        If Not Page.IsPostBack Then
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            hd_usaFiltroRicercaNG.Value = objFiltroRicerca.usaFiltroRicercaNG(objParametri_Utenti)
            hd_CurrentPiva.Value = objParametriAgenda.Piva
        End If

        ids = Request.QueryString("id")
        hd_ids.Value = ids
        raccoglitori_cod = Request.QueryString("raccoglitore")
        If ids <> "" Then
            Session("xFiltroAggiuntivo_colturali") = Request.QueryString("id")
            Session("KendoOperazioniSelezionate") = hdOperazioniSelezionate.Value
        Else
            ids = Session("xFiltroAggiuntivo_colturali")
            If hdOperazioniSelezionate.Value = "" Then
                hdOperazioniSelezionate.Value = Session("KendoOperazioniSelezionate")
            End If
        End If

        If raccoglitori_cod <> "" Then
            Session("xFiltroAggiuntivo_colturali_raccoglitori") = Request.QueryString("raccoglitore")
            Session("KendoOperazioniSelezionate") = hdOperazioniSelezionate.Value
        Else
            raccoglitori_cod = Session("xFiltroAggiuntivo_colturali_raccoglitori")
        End If





        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        Dim OperazioniMultiAziendali As Boolean = False

        Dim inserPrimaOper As Boolean = False

        If Not Page.IsPostBack Then

            If VerificaPermessi(enum_Security_Attivita.Agenda_AccessoMenu) = False Then
                objParametriAgenda.Svuota_DatiOperazione()
                TrovaRedirectCorretto(objParametriAgenda)
                'torno alla pagina di menu
                Exit Sub
            End If

            If objParametriAgenda.Impianti.Count = 0 AndAlso ids <> "" Then
                RecuperaImpiantiDaIDAgenda()
            End If

            nrTotaleImpiantiOperazione = objParametriAgenda.Impianti.Count

            OperazioniMultiAziendali = VerificaPermessi(enum_Security_Attivita.Agenda_OperazioniMultiAziendali)
            If OperazioniMultiAziendali Then

                Btn_FiltraImpianti.Visible = False
                Btn_FiltraAziende.Visible = False

                If Not hd_isFertirrigazione.Value = True Then
                    Btn_FiltroneNuovo.Visible = True
                End If

                'se ci sono impianti li ho filtrati nel filtrone
                If objParametriAgenda.Impianti.Count > 0 Then
                    Dim DataViewImpianti As DataTable = Nothing
                    Dim strImpiantiKendo As String = ""
                    Carica_Impianti(objParametriAgenda, ViewState("vs_dtInterventi"), strImpiantiKendo, DataViewImpianti)
                    'Riga_Impianti.Visible = True
                    inserPrimaOper = True

                    Session("DataViewImpianti") = DataViewImpianti
                    hdKendo_Impianti.Value = strImpiantiKendo
                End If

            End If

            Session("DtOperazioniNonSalvate") = Nothing
            HiddenVarie.Value = ""

        Else
            Exit Sub
        End If

        RecuperaIntervallo()
        CaricaGriglia_Interventi()

        If inserPrimaOper Then
            ImgBtn_InterventoInserisci_Click(Me, Nothing)
        End If

    End Sub


    Private Sub RecuperaIntervallo()

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_DatiAgenda As System.Xml.XmlElement
        Dim XML_Agenda As System.Xml.XmlElement
        Dim XML_DatiMovimenti As System.Xml.XmlElement
        Dim XML_Movimento As System.Xml.XmlElement
        Dim XMLs_Movimento As System.Xml.XmlNodeList
        Dim XML_DatiMovimentiDettagli As System.Xml.XmlElement
        Dim XML_MovimentoDettaglio As System.Xml.XmlElement
        Dim XMLs_MovimentoDettaglio As System.Xml.XmlNodeList
        Dim XML_MovimentoDestinazione As System.Xml.XmlElement
        Dim XMLs_MovimentoDestinazione As System.Xml.XmlNodeList

        Dim StringaXML As String

        Dim i As Integer
        Dim j As Integer
        Dim z As Integer
        Dim y As Integer

        Dim Appezza As Integer
        Dim Id_Imp As Integer

        Dim Trovato As Boolean


        '---------------------------------------------------
        '----- Recupero tutte le destinazioni per le date
        '---------------------------------------------------

        Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_R

        'Leggo la stringa relativa all'operazione
        StringaXML = objAgenda.Agenda_Leggi(
                                         CStr(objParametriAgenda.Piva),
                                         CInt(objParametriAgenda.Sa_Cod),
                                         CInt(objParametriAgenda.Id_Agenda),
                                         0,
                                         False,
                                        objParametri_Server)


        objAgenda = Nothing

        If StringaXML = "" Then
            AgroMsgBox(Resources.AgronicaAgenda_2010.ErroreDuranteLaLetturaDellOperazione, Page, EsisteMaster:=True)
        Else

            'salvo la stringa nel viewstate
            ViewState("StringaXML") = StringaXML

            '------------------------------------------
            '----- Analizzo la stringa XML
            '------------------------------------------

            'Carico la stringa nel documento XML
            XmlDoc.LoadXml(StringaXML)

            '----- Tag DatiAgenda

            XML_DatiAgenda = XmlDoc.SelectSingleNode("DatiAgenda")

            '----- Tag Agenda

            XML_Agenda = XML_DatiAgenda.SelectSingleNode("Agenda")

            frm_Piva = CStr(XML_Agenda.GetAttribute("piva"))
            frm_SaCod = CInt(XML_Agenda.GetAttribute("sa_cod"))
            frm_IdAgenda = CInt(XML_Agenda.GetAttribute("id_agenda"))
            frm_DescrizioneOperazione = CStr(XML_Agenda.GetAttribute("des_lib"))
            frm_DataOperazione = CDate(XML_Agenda.GetAttribute("validita_inizio"))

            '----- Tag DatiMovimenti

            XML_DatiMovimenti = XML_Agenda.SelectSingleNode("DatiMovimenti")

            '----- Tag Movimento (multiplo)


            XMLs_Movimento = XML_DatiMovimenti.GetElementsByTagName("Movimento")

            For i = 0 To XMLs_Movimento.Count - 1

                XML_Movimento = XMLs_Movimento.Item(i)

                If CInt(XML_Movimento.GetAttribute("cau_mov")) >= 2050 AndAlso CInt(XML_Movimento.GetAttribute("cau_mov")) <= 2300 Then

                    frm_NoteIntervento = CStr(XML_Movimento.GetAttribute("mov_desc"))

                    '----- Tag DatiMovimenti_Dettagli

                    XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                    '----- Tag Movimento_Dettaglio  (multiplo)

                    'Recupero la collezione dei nodi
                    XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

                    'Ciclo su tutti i nodi
                    For j = 0 To XMLs_MovimentoDettaglio.Count - 1

                        'Prendo l'i-esimo nodo della collezione
                        XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

                        'Recupero la collezione dei nodi
                        XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                        'Ciclo su tutti i nodi
                        For z = 0 To XMLs_MovimentoDestinazione.Count - 1

                            'Prendo il j-esimo nodo della collezione
                            XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(z)

                            'Recupero i valori
                            Appezza = CInt(XML_MovimentoDestinazione.GetAttribute("appezza"))
                            Id_Imp = CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione"))

                            'se io vettore è vuoto inserisco l'impianto
                            If IsNothing(frm_Appezza) Then

                                ReDim Preserve frm_Appezza(0)
                                ReDim Preserve frm_IdReg(0)

                                'Recupero i valori
                                frm_Appezza(0) = Appezza
                                frm_IdReg(0) = Id_Imp

                            Else

                                'prima di inserire un impianto nel vettore, controllo se c'è già
                                Trovato = False

                                For y = 0 To UBound(frm_Appezza)

                                    If frm_Appezza(y) = Appezza AndAlso frm_IdReg(y) = Id_Imp Then

                                        Trovato = True

                                        Exit For

                                    End If

                                Next

                                If Not Trovato Then

                                    ReDim Preserve frm_Appezza(UBound(frm_Appezza) + 1)
                                    ReDim Preserve frm_IdReg(UBound(frm_IdReg) + 1)

                                    'Recupero i valori
                                    frm_Appezza(UBound(frm_Appezza)) = Appezza
                                    frm_IdReg(UBound(frm_IdReg)) = Id_Imp

                                End If

                            End If

                        Next

                    Next

                End If

            Next

            'dimensione i vettori
            ReDim frm_ValiditaInizio(UBound(frm_Appezza))
            ReDim frm_ValiditaFine(UBound(frm_Appezza))

            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

            'recupero per ogni impianto la ValiditàInizio e la ValiditàFine e le salvo in due vettori
            For i = 0 To UBound(frm_Appezza)

                objImprese.Recupera_DateValidita_ElementiGerarchia(enum_GerarchiaImpresa_Elementi.Gerarchia_Impianto,
                                                                    frm_ValiditaInizio(i),
                                                                    frm_ValiditaFine(i),
                                                                    Nothing,
                                                                    frm_Piva,
                                                                    frm_SaCod,
                                                                    0,
                                                                    frm_Appezza(i),
                                                                    frm_IdReg(i),
                                                                    objParametri_Server)


            Next

            'ordino i due vettori
            Array.Sort(frm_ValiditaInizio)
            Array.Sort(frm_ValiditaFine)

            'recupero l'intervallo di tempo valido
            If frm_ValiditaInizio(0) = #1/1/1900# Then
                Me.Txt_ValiditaInizio.Text = ""
            Else
                Me.Txt_ValiditaInizio.Text = frm_ValiditaInizio(0)
            End If

            If frm_ValiditaFine(UBound(frm_ValiditaFine)) = #12/31/2100# Then
                Me.Txt_ValiditaFine.Text = Resources.AgronicaAgenda_2010.Attivo
            Else
                Me.Txt_ValiditaFine.Text = frm_ValiditaFine(UBound(frm_ValiditaFine))
            End If

            ' VAnni: 20/3/2017: ora in tabella....
            'visualizzo la descrizione dell'operazione
            'Me.Txt_Descrizione.Text = frm_DescrizioneOperazione

            'Dim objAgendaH As New Agenda_Operazione_Helper
            'Dim AgendaTemp As New Operazione_Agenda

            'AgendaTemp = objAgendaH.Leggi(frm_Piva, _
            '                             frm_SaCod, _
            '                             frm_IdAgenda, _
            '                             0, _
            '                             objParametri_Server)

            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim Dt_Imp As New DataTable
            'Dt_Imp = objImp.Leggi(AgendaTemp.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Piva, _
            '             AgendaTemp.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Sa_Cod, _
            '             AgendaTemp.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Appezza, _
            '             AgendaTemp.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Id_Destinazione, _
            '             enumSelezioneVariabile.Selezione_JoinDescrizioni, _
            '             "", "", objParametri_Server)

            Dt_Imp = objImp.Leggi(frm_Piva, frm_SaCod, frm_Appezza(0), frm_IdReg(0), enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
            objImp = Nothing

            If Dt_Imp.Rows.Count > 0 Then
                '  Galassi, 21/07/2016 11:51:43: Il veg_cod viene poi utlizzato per il filtrino nella selezione degli impianti
                ViewState("Veg_Cod") = CInt(Dt_Imp.Rows(0).Item("veg_cod"))
            End If


            Me.Txt_DataIntervento.Text = frm_DataOperazione



            'salvo nel viewstate le informazioni che dopo devo sostituire
            ViewState("Data") = frm_DataOperazione
            ViewState("Note") = frm_NoteIntervento

        End If

    End Sub

    Public Function TrovaRedirectCorretto(ByVal objParametriAgenda As ParametriAgenda) As String

        Dim strMenu = "../Menu/Menu.aspx"

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
            strMenu = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        Return strMenu

        'identifico il sito che mi ha chiamato 
        Select Case HttpContext.Current.Session("Sito_Origine")
            Case Enum_SiteRedirector.Sito_GiasOnline

                Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                objGiasOnline.Cul_Cod = objParametriAgenda.Cul_Cod
                objGiasOnline.DataSelezionata = objParametriAgenda.Data
                objGiasOnline.Id_Agenda = objParametriAgenda.Id_Agenda
                objGiasOnline.Lavorazione = objParametriAgenda.Lav_Cod
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.MenuAgenda
                objGiasOnline.Piva = objParametriAgenda.Piva
                objGiasOnline.Sa_Cod = objParametriAgenda.Sa_Cod
                Dim specie As Integer = 0
                If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
                    specie = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
                End If
                objGiasOnline.Veg_Cod = specie

                Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                          Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                          objGiasOnline)

                'Apro nella stessa pagina
                Return str
        End Select
    End Function

    Private Function VerificaPermessi(ByVal Id_Attivita As enum_Security_Attivita)

        Dim UtenteAbilitato_Modifica As Boolean = False

        Dim UtenteAbilitato_Lettura As Boolean = False

        Dim objUtility As New AgronicaCoreModello.Utility_Operazioni

        If Id_Attivita = enum_Security_Attivita.Agenda_AccessoMenu Then
            objUtility.Verifica_Permessi_OperazioniAgenda_X_PagineAgronicaAgenda(objParametri_Server, objParametri_Utenti, UtenteAbilitato_Lettura, UtenteAbilitato_Modifica)
        Else
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R


            UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                       Session("ASG_IdServizio"),
                                       Id_Attivita,
                                       enum_Security_Operazione.Modifica,
                                       Date.Now,
                                       "",
                                       objParametri_Utenti)
        End If

        Return UtenteAbilitato_Modifica

    End Function

    Private Sub InizializzaScript()

        Dim Str As New StringBuilder

        Str.AppendLine("$(document).ready(function () {")

        Str.AppendLine(" $('.bottone').button(); ")

        Str.AppendLine(" $('#" & Txt_DataIntervento.ClientID & "').kendoDatePicker(); ")
        Str.AppendLine(" console.log(""kendoDatePicker""); ")

        Str.AppendLine(" $('#chkSelezionaTuttiImpianti').click(function () {  ")
        Str.AppendLine("         SelezionaDeselezionaTutti(); ")
        Str.AppendLine("     }); ")



        Str.AppendLine("});")

        ScriptManager.RegisterStartupScript(UpdatePanelScript, UpdatePanelScript.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelScript.ClientID), Str.ToString, True)
    End Sub

    Private Sub CaricaGriglia_Interventi()

        Dim Dt As New DataTable

        Dt.Columns.Add(New DataColumn("ID", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))

        Dim DtKeys(0) As DataColumn
        DtKeys(0) = Dt.Columns("ID")
        Dt.PrimaryKey = DtKeys

        DataGridInterventi.DataSource = Dt
        DataGridInterventi.DataBind()

        ViewState("vs_dtInterventi") = Dt
        hd_dtInterventi.Value = JsonConvert.SerializeObject(Dt)

    End Sub

    Private Sub AAA_GestioneUscitaPagina()

        Session("DtOperazioniNonSalvate") = Nothing
        HiddenVarie.Value = ""
        Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, , UpdatePanelScript)

    End Sub

    Protected Sub ImgBtn_InterventoInserisci_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_InterventoInserisci.Click

        Dim Messaggio As String
        Dim Dt As New DataTable
        Dim Dr As DataRow
        '-----

        If Txt_DataIntervento.Text = "" Then
            Messaggio = String.Format(Resources.AgronicaAgenda_2010.InserireLaDataDellOperazioneCompresaNelPer, Txt_ValiditaInizio.Text, Txt_ValiditaFine.Text)
            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanelScript)
            Exit Sub
        End If


        If Txt_ValiditaInizio.Text <> "" AndAlso CDate(Txt_DataIntervento.Text) < CDate(Txt_ValiditaInizio.Text) Then
            Messaggio = String.Format(Resources.AgronicaAgenda_2010.LaDataDellOperazioneDeveRientrareNelPeriod, Txt_ValiditaInizio.Text, Txt_ValiditaFine.Text)
            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanelScript)
            Exit Sub
        End If

        If Txt_ValiditaFine.Text <> Resources.AgronicaAgenda_2010.Attivo Then

            If CDate(Txt_DataIntervento.Text) > CDate(Txt_ValiditaFine.Text) Then
                Messaggio = String.Format(Resources.AgronicaAgenda_2010.LaDataDellOperazioneDeveRientrareNelPeriod, Txt_ValiditaInizio.Text, Txt_ValiditaFine.Text)
                Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanelScript)
                Exit Sub
            End If

        End If

        'Recupero il datatable
        Dt = ViewState("vs_dtInterventi")

        Dim MaxID As Integer = 0

        'seleziono il max id
        For IndiceRiga = 0 To Dt.Rows.Count - 1
            ID = Dt.Rows(IndiceRiga).Item("ID")
            If ID > MaxID Then
                MaxID = ID
            End If
        Next

        MaxID += 1

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori+
        Dr.Item("ID") = MaxID
        Dr.Item("Data") = CDate(Txt_DataIntervento.Text).ToShortDateString
        Dr.Item("Note") = Txt_Note.Text

        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)

        '----- Associo il DataTable con la DataGrid
        DataGridInterventi.DataSource = Dt
        DataGridInterventi.DataBind()

        '----- Salvo il DataTable dentro il viewstate
        ViewState("vs_dtInterventi") = Dt
        hd_dtInterventi.Value = JsonConvert.SerializeObject(Dt)

        'pulisco i controlli
        Me.Txt_DataIntervento.Text = ""
        Me.Txt_Note.Text = ""

        RecuperaImpiantiOperazioneSelezionata()

    End Sub

    Private Function XML_GeneraStringoneFinale(ByVal Stringa As String,
                                               ByVal OldData As Date,
                                               ByVal NewData As Date,
                                               ByVal OldNote As String,
                                               ByVal NewNote As String,
                                               ByVal BaseCode As Integer,
                                               ByVal TopCode As Integer) As String

        Dim StringaXML As String = Stringa

        'Data
        StringaXML = Replace(StringaXML, OldData, NewData)

        'Note
        'StringaXML = Replace(StringaXML, OldNote, NewNote)
        StringaXML = Replace(StringaXML, "mov_desc=""" & OldNote & """", "mov_desc=""" & NewNote & """")

        'Reimpostazione del Nuovo TipoOperazioneDB/ BaseCode / TopCode
        StringaXML = Replace(StringaXML, "TipoOperazioneDB=""0""", "TipoOperazioneDB =""1"" basecode = """ & BaseCode & """ topcode = """ & TopCode & """")

        'Nota: Il trucco è di rendere negativi i codici in modi tale che il componente ne crei dei nuovi

        'Id_Agenda, Id_Mov, Id_Mov_Det, Id_Reg_Dettaglio
        StringaXML = Replace(StringaXML, "id_agenda=""", "id_agenda=""-")
        StringaXML = Replace(StringaXML, "id_mov=""", "id_mov=""-")
        StringaXML = Replace(StringaXML, "id_mov_det=""", "id_mov_det=""-")
        StringaXML = Replace(StringaXML, "id_reg_dettaglio=""", "id_reg_dettaglio=""-")

        Return StringaXML

    End Function

    Protected Sub Btn_FiltraImpianti_Click(sender As Object, e As EventArgs) Handles Btn_FiltraImpianti.Click

        objParametriAgenda.Impianti.Clear()

        Dim TargetRedirect As String

        TargetRedirect = "../Filtrone/Filtrone.aspx?p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Server) &
                        "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
                        "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Server) &
                        "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
                        "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.OperazioniMultiAziendali, AgroKey_EncoderDecoder, Server) &
                        "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                        "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server)

        Response.Redirect(TargetRedirect)

    End Sub

    Protected Sub Btn_FiltraAziende_Click(sender As Object, e As EventArgs) Handles Btn_FiltraAziende.Click

        objParametriAgenda.Impianti.Clear()

        Dim TargetRedirect As String

        TargetRedirect = "../Filtrino/FiltrinoImprese.aspx?d=" & Stringa_Codifica("../Operazioni/DuplicaOperazione.aspx", AgroKey_EncoderDecoder, Server) &
                        "&sito=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
                        "&pagina=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Server) &
                        "&o=" & Stringa_Codifica("../Operazioni/DuplicaOperazione.aspx", AgroKey_EncoderDecoder, Server) &
                        "&veg=" & Stringa_Codifica(IIf(IsNothing(ViewState("Veg_Cod")), 0, ViewState("Veg_Cod")), AgroKey_EncoderDecoder, Server)

        Response.Redirect(TargetRedirect)
    End Sub


    Protected Sub Btn_FiltroneNuovo_Click(sender As Object, e As EventArgs) Handles Btn_FiltroneNuovo.Click

        Dim TargetRedirect As String
        objParametriAgenda.Impianti.Clear()

        'TargetRedirect = "../Filtrone/Filtrone_nuovo.aspx?d=" & Stringa_Codifica("../Operazioni/DuplicaOperazione.aspx", AgroKey_EncoderDecoder, Server) &
        '                "&sito=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
        '                "&pagina=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Server) &
        '                "&o=" & Stringa_Codifica("../Operazioni/DuplicaOperazione.aspx", AgroKey_EncoderDecoder, Server) &
        '                "&veg=" & Stringa_Codifica(IIf(IsNothing(ViewState("Veg_Cod")), 0, ViewState("Veg_Cod")), AgroKey_EncoderDecoder, Server)

        TargetRedirect = "../Filtrone/Filtrone_nuovo.aspx?" &
                "p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Nothing) &
                "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Nothing) &
                "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.DuplicaOperazione, AgroKey_EncoderDecoder, Nothing) &
                "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                "&veg=" & Stringa_Codifica(IIf(IsNothing(ViewState("Veg_Cod")), 0, ViewState("Veg_Cod")), AgroKey_EncoderDecoder, Server)

        Response.Redirect(TargetRedirect)

    End Sub

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
                .TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita,
                .SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                .PaginaProvenienza = enum_PagineAgenda_2010.Pagina_DuplicaOperazione,
                .Piva = piva,
                .FiltriTemporali = objFiltroRicerca.Imposta_FiltroEntitaAttivaAllaData(Date.Now, Enum_Entita_FiltroRicerca.Impianto),
                .TipoMostraGestitiChiamante = New List(Of Enum_TipoMostra_FiltroRicerca) From {Enum_TipoMostra_FiltroRicerca.Impianti}
            }

            '.VegCod = IIf(IsNothing(ViewState("Veg_Cod")), 0, ViewState("Veg_Cod")),

            r.RispostaStringa = objFiltroRicerca.Link_Pagina_FiltroRicercaNG(piva, parametriFiltroRicercaNG)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaImpiantiDaChiavi(chiavi As List(Of String), strDtInterventi As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim strImpianti As String = ""


        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.Impianti.Clear()

            Dim dtInterventi As DataTable = JsonConvert.DeserializeObject(Of DataTable)(strDtInterventi)

            Dim filtroProgetti As New List(Of Integer)
            For Each chiave In chiavi
                filtroProgetti.Add(chiave.Split("_")(5))
            Next

            Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim dtimpianti = objRegImpianti.Leggi_x_ParametriAgenda(filtroProgetti, objParametri_Server, True)

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

                Dim dr() As DataRow = dtimpianti.Select("progetto_cod = " & progetto_cod)

                If dr.Length > 0 Then
                    objImpianto.Rag_Soc = CStr(dr(0).Item("rag_soc"))
                    objImpianto.Sa_Nome = CStr(dr(0).Item("sa_nome"))
                    objImpianto.App_Nome = CStr(dr(0).Item("app_nome"))

                    If IsDBNull(dr(0).Item("veg_cod")) Then
                        objImpianto.Veg_Cod = 0
                    Else
                        objImpianto.Veg_Cod = CStr(dr(0).Item("veg_cod"))
                    End If
                    If IsDBNull(dr(0).Item("veg_des")) Then
                        objImpianto.Veg_Des = ""
                    Else
                        objImpianto.Veg_Des = CStr(dr(0).Item("veg_des"))
                    End If
                    If IsDBNull(dr(0).Item("cul_cod")) Then
                        objImpianto.Cul_Cod = 0
                    Else
                        objImpianto.Cul_Cod = CStr(dr(0).Item("cul_cod"))
                    End If
                    If IsDBNull(dr(0).Item("cul_des")) Then
                        objImpianto.Cul_Des = ""
                    Else
                        objImpianto.Cul_Des = CStr(dr(0).Item("cul_des"))
                    End If
                    If IsDBNull(dr(0).Item("Validita_Inizio")) Then
                        objImpianto.Validita_Inizio = AGRODATAINIZIO
                    Else
                        objImpianto.Validita_Inizio = CStr(dr(0).Item("Validita_Inizio"))
                    End If
                    If IsDBNull(dr(0).Item("Validita_Fine")) Then
                        objImpianto.Validita_Fine = AGRODATAFINE
                    Else
                        objImpianto.Validita_Fine = CStr(dr(0).Item("Validita_Fine"))
                    End If
                    'If IsDBNull(dr(0).Item("Validita_Inizio_Distinta")) Then
                    '    objImpianto.Validita_Inizio_Distinta = AGRODATAINIZIO
                    'Else
                    '    objImpianto.Validita_Inizio_Distinta = CStr(dr(0).Item("Validita_Inizio_Distinta"))
                    'End If
                    'If IsDBNull(dr(0).Item("Validita_Fine_Distinta")) Then
                    '    objImpianto.Validita_Fine_Distinta = AGRODATAFINE
                    'Else
                    '    objImpianto.Validita_Fine_Distinta = CStr(dr(0).Item("Validita_Fine_Distinta"))
                    'End If
                    If IsDBNull(dr(0).Item("Campo_Cod")) Then
                        objImpianto.Campo_Cod = 0
                    Else
                        objImpianto.Campo_Cod = CStr(dr(0).Item("Campo_Cod"))
                    End If
                    If IsDBNull(dr(0).Item("Campo_Des")) Then
                        objImpianto.Campo_Des = ""
                    Else
                        objImpianto.Campo_Des = CStr(dr(0).Item("Campo_Des"))
                    End If

                    objImpianto.Sup_Imp = CStr(dr(0).Item("sup_imp"))
                End If

                objParametriAgenda.Impianti.Add(objImpianto)
            Next

            Carica_Impianti(objParametriAgenda, dtInterventi, strImpianti, Nothing)

            r.RispostaOK = True
            r.RispostaStringa = strImpianti

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Shared Sub Carica_Impianti(objParametriAgenda As ParametriAgenda, dtInterventi As DataTable, ByRef strImpiantiKendo As String, ByRef DataViewImpianti As DataTable)

        Dim Dr As DataRow
        Dim i As Integer

        '----- Definisco la struttura del DataTable
        Dim DTImpianti As New DataTable
        DTImpianti = GeneraStrutturaDTImpianti()


        If objParametriAgenda.Impianti IsNot Nothing AndAlso objParametriAgenda.Impianti.Count > 0 Then

            Dim listaChiaviImpianti As New List(Of String)

            'Ricavo le date delle operazioni d'agenda. i miei impianti devono essere validi in quelle date
            Dim operazione_DataMin As Date = AGRODATAFINE
            Dim operazione_DataMax As Date = AGRODATAINIZIO

            If Not IsNothing(dtInterventi) Then
                Dim DtOperazioni As DataTable = dtInterventi
                For Each dr_op As DataRow In DtOperazioni.Rows
                    If IsDate(dr_op.Item("Data")) Then
                        If dr_op.Item("Data") < operazione_DataMin Then
                            operazione_DataMin = CDate(dr_op.Item("Data"))
                        End If
                        If dr_op.Item("Data") > operazione_DataMax Then
                            operazione_DataMax = CDate(dr_op.Item("Data"))
                        End If
                    End If
                Next
            End If

            'Nel caso standard il DT viene letto da DB ed è da formattare
            For i = 0 To objParametriAgenda.Impianti.Count - 1

                'Mostro gli impianti solo se attivi durante l'arco temporale
                Dim validitaInizio_Impianto As Date = CDate(objParametriAgenda.Impianti(i).Validita_Inizio)
                Dim validitaFine_Impianto As Date = CDate(objParametriAgenda.Impianti(i).Validita_Fine)

                Dim validitaInizio_Distinta As Date = CDate(objParametriAgenda.Impianti(i).Validita_Inizio_Distinta)
                Dim validitaFine_Distinta As Date = CDate(objParametriAgenda.Impianti(i).Validita_Fine_Distinta)

                If (validitaInizio_Impianto <= operazione_DataMin AndAlso validitaFine_Impianto >= operazione_DataMax) Then

                    Dr = DTImpianti.NewRow

                    Dr.Item("kendoKey") =
                    objParametriAgenda.Impianti(i).Piva & "-" &
                    objParametriAgenda.Impianti(i).Sa_Cod & "-" &
                    objParametriAgenda.Impianti(i).Appezza & "-" &
                    objParametriAgenda.Impianti(i).ID_Reg

                    Dr.Item("piva") = objParametriAgenda.Impianti(i).Piva
                    Dr.Item("sa_cod") = objParametriAgenda.Impianti(i).Sa_Cod
                    Dr.Item("campo_cod") = objParametriAgenda.Impianti(i).Campo_Cod
                    Dr.Item("campo_des") = objParametriAgenda.Impianti(i).Campo_Des
                    Dr.Item("appezza") = objParametriAgenda.Impianti(i).Appezza
                    Dr.Item("id_reg") = objParametriAgenda.Impianti(i).ID_Reg
                    Dr.Item("veg_cod") = objParametriAgenda.Impianti(i).Veg_Cod
                    Dr.Item("cul_cod") = objParametriAgenda.Impianti(i).Cul_Cod
                    Dr.Item("grfi_cod") = 0
                    Dr.Item("regolamento") = 1
                    Dr.Item("finanziamento") = 0
                    Dr.Item("rag_soc") = objParametriAgenda.Impianti(i).Rag_Soc
                    Dr.Item("sa_nome") = objParametriAgenda.Impianti(i).Sa_Nome
                    Dr.Item("app_nome") = objParametriAgenda.Impianti(i).App_Nome
                    Dr.Item("Progetto_Cod") = objParametriAgenda.Impianti(i).Progetto_Cod
                    Dr.Item("descrizione") = objParametriAgenda.Impianti(i).Veg_Des & " - " & objParametriAgenda.Impianti(i).Cul_Des
                    Dr.Item("sup_imp") = objParametriAgenda.Impianti(i).Sup_Imp
                    Dr.Item("validita_inizio") = IIf(objParametriAgenda.Impianti(i).Validita_Inizio.ToShortDateString <> "01/01/1900", objParametriAgenda.Impianti(i).Validita_Inizio.ToShortDateString, "...")
                    Dr.Item("validita_fine") = IIf(objParametriAgenda.Impianti(i).Validita_Fine.ToShortDateString <> "31/12/2100", objParametriAgenda.Impianti(i).Validita_Fine.ToShortDateString, "...")

                    Dim validitaInizioDistinta = objParametriAgenda.Impianti(i).Validita_Inizio_Distinta.ToShortDateString
                    Dim validitaFineDistinta = objParametriAgenda.Impianti(i).Validita_Fine_Distinta.ToShortDateString
                    Dr.Item("validita_inizio_distinta") = IIf(validitaInizioDistinta <> "01/01/1900" AndAlso validitaInizioDistinta <> "01/01/0001", objParametriAgenda.Impianti(i).Validita_Inizio_Distinta.ToShortDateString, "...")
                    Dr.Item("validita_fine_distinta") = IIf(validitaFineDistinta <> "31/12/2100" AndAlso validitaFineDistinta <> "01/01/0001", objParametriAgenda.Impianti(i).Validita_Fine_Distinta.ToShortDateString, "...")

                    If Not listaChiaviImpianti.Contains(Dr.Item("kendoKey")) Then
                        listaChiaviImpianti.Add(Dr.Item("kendoKey"))
                        DTImpianti.Rows.Add(Dr)
                    End If

                End If
            Next

        End If

        'uso il dataview per RIordinare 
        Dim Dv As New DataView

        DTImpianti.TableName = "Impianti"
        Dv.Table = DTImpianti
        Dv.Sort = "Rag_Soc DESC,Sa_Nome DESC"

        Dim dtBack As DataTable = Dv.ToTable
        strImpiantiKendo = JSON_Datatable_Impianti(dtBack)
        DataViewImpianti = Dv.ToTable

    End Sub

    Private Shared Function JSON_Datatable_Impianti(ByVal dt As DataTable) As String

        Dim strImpiantiKendo As String = ""

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("kendoKey", "kendoKey", "string")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("rag_soc", Gias.RagioneSociale, "String")
        l.Add(c)

        c = New ColonneNome("sa_nome", Gias.CentroAziendale, "String")
        l.Add(c)

        c = New ColonneNome("campo_des", Gias.Campo, "String")
        l.Add(c)

        c = New ColonneNome("app_nome", Gias.AppezzamentoAbbr, "String")
        l.Add(c)

        c = New ColonneNome("descrizione", Gias.Descrizione, "String")
        l.Add(c)

        c = New ColonneNome("sup_imp", Gias.SuperficieAbbr & " [ha]", "number")
        l.Add(c)

        c = New ColonneNome("validita_inizio", Gias.DataInizioImpianto, "String")
        l.Add(c)

        c = New ColonneNome("validita_fine", Gias.DataFineImpianto, "String")
        l.Add(c)


        c = New ColonneNome("validita_inizio_distinta", Gias.DataInizioEsercizio, "String")
        l.Add(c)

        c = New ColonneNome("validita_fine_distinta", Gias.DataFineEsercizio, "String")
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        strImpiantiKendo = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

        'hdKendo_Impianti.Value = risp
        Return strImpiantiKendo

    End Function

    Private Shared Function GeneraStrutturaDTImpianti() As DataTable

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("kendoKey", GetType(String)))
        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("campo_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("campo_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("id_reg", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("veg_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("cul_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("grfi_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("regolamento", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("finanziamento", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("app_nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("Progetto_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("sup_imp", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("validita_fine", GetType(String)))
        Dt.Columns.Add(New DataColumn("validita_inizio_distinta", GetType(String)))
        Dt.Columns.Add(New DataColumn("validita_fine_distinta", GetType(String)))

        Return Dt

    End Function

    Private Function SommatoriaSupImp(ByVal dt As DataTable) As Decimal

        Dim xSomma As Decimal = 0
        For Each dr As DataRow In dt.Rows
            xSomma += CDec(dr("sup_imp"))
        Next

        Return xSomma
    End Function

    Private Function Genera_Agenda_MultiAziendale(ByVal AgendaDaCopiare As Operazione_Agenda,
                                                  ByVal Data As Date,
                                                  ByVal Note As String,
                                                  ByVal Dt As DataTable,
                                                  ByRef strErr As String
                                                  ) As Operazione_Agenda


        Dim Piva As String
        Dim SaCod As Integer

        Piva = Dt.Rows(0).Item("piva")
        SaCod = Dt.Rows(0).Item("sa_cod")

        Dim SupTrattata_Tot As Decimal = 0
        Dim SuperficieTotaleCentro As Decimal = 0

        For c = 0 To Dt.Rows.Count - 1
            SuperficieTotaleCentro = SuperficieTotaleCentro + Dt.Rows(c).Item("sup_imp")
        Next


        Dim Acqua As Decimal = 0
        Dim AcquaTot As Decimal = 0

        Dim HashTotNew As New Hashtable

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione
        Dim FlagNuovoControlloRiduzioneDiserbo As Boolean = False
        Dim PercAbbMin As Decimal = 100

        FlagNuovoControlloRiduzioneDiserbo = Leggi_FlagNuovoControlloRiduzioneDiserbo()

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = Data
        Agenda.Piva = Piva
        Agenda.Sa_Cod = SaCod
        Agenda.Lav_Cod = AgendaDaCopiare.Lav_Cod
        Agenda.Des_Lib = AgendaDaCopiare.Des_Lib 'ComboOperazione.Testo_Combo & " (" & Cmb_Specie.SelectedItem.Text & ")"

        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        For i = 0 To AgendaDaCopiare.Note.Count - 1
            Nota = New Nota
            Nota.Nota_Cod = AgendaDaCopiare.Note(i).Nota_Cod
            Agenda.Note.Add(Nota)
        Next

        Dim UdmCodTrasformato As Integer = 0
        Dim DoseTrasformata As Decimal = 0
        Dim DoseTotaleTrasformata As Decimal = 0

        Dim Dose As Decimal = 0
        Dim Qta_Dest_New As Decimal = 0
        Dim Dose_Tot_Old As Decimal = 0
        Dim Dose_Tot_New As Decimal = 0
        Dim Sup_Tot_Old As Decimal = 0

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        For i_movimento = 0 To AgendaDaCopiare.Movimenti.Count - 1

            Select Case AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov

                Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA

                    Movimento = New Movimento

                    Movimento.Piva = Agenda.Piva
                    Movimento.Sa_Cod = Agenda.Sa_Cod
                    Movimento.Data = Data
                    Movimento.Lav_Cod = Agenda.Lav_Cod
                    Movimento.Cau_Mov = AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov
                    Movimento.Mov_Desc = Note
                    '0=HL 1=HA
                    Movimento.Mezzo = AgendaDaCopiare.Movimenti(i_movimento).Mezzo
                    '11=dose (10=qta tot non contemplata)
                    Movimento.Modalita = AgendaDaCopiare.Movimenti(i_movimento).Modalita

                    Movimento.Num_Protocollo = AgendaDaCopiare.Movimenti(i_movimento).Num_Protocollo
                    Movimento.Doc_Numero = AgendaDaCopiare.Movimenti(i_movimento).Doc_Numero
                    Movimento.Disciplinare_PubblicoPrivato = AgendaDaCopiare.Movimenti(i_movimento).Disciplinare_PubblicoPrivato
                    Movimento.Extra_Int = AgendaDaCopiare.Movimenti(i_movimento).Extra_Int

                    Movimento.BaseCode = frm_BaseCode
                    Movimento.TopCode = frm_TopCode

                    Movimento.Modalita_Applicazione = AgendaDaCopiare.Movimenti(i_movimento).Modalita_Applicazione

                    Agenda.Movimenti.Add(Movimento)

                    '------------------------------------------------
                    '----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
                    '------------------------------------------------

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

                    'vanni, 28/06/2017, ricalcolo il dato della superficie totale per il caso di dosaggio totale (da dividere)
                    SupTrattata_Tot = SommatoriaSupImp(Dt)

                    For j = 0 To AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici.Count - 1

                        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                        '---------------------------------------------------
                        '----- Acqua  
                        '---------------------------------------------------

                        Select Case AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                            Case Is > 0 'dosaggio totale --> da dividere
                                Acqua = -(AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(j).Qta_Ril / SupTrattata_Tot)
                                AcquaTot = (AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(j).Qta_Ril / SupTrattata_Tot) * SuperficieTotaleCentro
                            Case Is < 0 'dosaggio/ha --> già ok
                                Acqua = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                                AcquaTot = Math.Abs(AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(j).Qta_Ril) * SuperficieTotaleCentro
                        End Select

                        ' per distribuzione insetti
                        Movimento_Dettaglio_Tecnico.Av_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(j).Av_Cod
                        Movimento_Dettaglio_Tecnico.Av_Gru = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(j).Av_Gru


                        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                        Movimento_Dettaglio_Tecnico.Data = Data
                        Movimento_Dettaglio_Tecnico.Qta_Ril = CDec(Acqua)
                        Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
                        Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                    Next


                    '------------------------------------------------
                    '----- MOVIMENTI DETTAGLI
                    '------------------------------------------------

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    'Per ciascuna dose impostata ...
                    For i_movimento_dettaglio = 0 To AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli.Count - 1

                        Movimento_Dettaglio = New Movimento_Dettaglio

                        Movimento_Dettaglio.Piva = Agenda.Piva
                        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
                        Movimento_Dettaglio.Data = Data
                        Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                        Movimento_Dettaglio.Cau_Mov = AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov

                        Movimento_Dettaglio.Elem_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Elem_Cod
                        Movimento_Dettaglio.Pro_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Pro_Cod
                        Movimento_Dettaglio.Mat_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Mat_Cod
                        Movimento_Dettaglio.Lotto = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Lotto

                        Movimento_Dettaglio.PrincipiAttivi = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).PrincipiAttivi
                        Movimento_Dettaglio.PrincipiAttiviPesi = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).PrincipiAttiviPesi
                        Movimento_Dettaglio.PrincipiAttiviPercAbb = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).PrincipiAttiviPercAbb
                        Movimento_Dettaglio.TempoCarenza = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).TempoCarenza
                        Movimento_Dettaglio.Polverulento = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Polverulento
                        Movimento_Dettaglio.DoseEtichetta = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).DoseEtichetta
                        Movimento_Dettaglio.DoseEtichetta_Value = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).DoseEtichetta_Value

                        Movimento_Dettaglio.Extra_Int = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Extra_Int
                        Movimento_Dettaglio.Udm_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Udm_Cod

                        Movimento_Dettaglio.Buffer = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Buffer
                        Movimento_Dettaglio.Extra_Str = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Extra_Str

                        If i_movimento_dettaglio = 0 Then
                            If Not IsNothing(AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni) Then
                                For x = 0 To AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni.Count - 1
                                    Sup_Tot_Old += AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(x).Qta2
                                Next
                            End If
                        End If


                        DoseTrasformata = 0
                        UdmCodTrasformato = 0
                        DoseTotaleTrasformata = 0


                        'dose Ha o dose tot (a seconda della lavorazione)
                        Dose = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Qta

                        Select Case Agenda.Lav_Cod

                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA
                                Dose_Tot_New = Math.Round(Dose / Sup_Tot_Old * SupTrattata_Tot, 0)
                                Dose_Tot_Old = Dose

                                Movimento_Dettaglio.Qta = Dose_Tot_New

                            Case Else

                                Dose_Tot_New = Dose * SupTrattata_Tot
                                Dose_Tot_Old = Dose * Sup_Tot_Old

                                'dose ha
                                Movimento_Dettaglio.Qta = Dose
                                'dose hl
                                If AcquaTot <> 0 Then
                                    Movimento_Dettaglio.Qta_Extra = Dose * SupTrattata_Tot / AcquaTot
                                End If
                                'qta tot
                                Movimento_Dettaglio.Qta_Extra_Totale = Dose * SupTrattata_Tot

                        End Select

                        Select Case Movimento_Dettaglio.Extra_Int
                            Case enum_UnitaMisura.Grammi  'g
                                UdmCodTrasformato = 2
                                DoseTrasformata = Dose / 1000
                                DoseTotaleTrasformata = Dose_Tot_New / 1000
                            Case enum_UnitaMisura.Milligrammi
                                UdmCodTrasformato = 2
                                DoseTrasformata = Dose / 1000000
                                DoseTotaleTrasformata = Dose_Tot_New / 1000000
                            Case enum_UnitaMisura.Quintali
                                UdmCodTrasformato = 2
                                DoseTrasformata = Dose * 100
                                DoseTotaleTrasformata = Dose_Tot_New * 100
                            Case enum_UnitaMisura.Tonnellate
                                UdmCodTrasformato = 2
                                DoseTrasformata = Dose * 1000
                                DoseTotaleTrasformata = Dose_Tot_New * 1000
                            Case enum_UnitaMisura.Millilitri
                                UdmCodTrasformato = 29
                                DoseTrasformata = Dose / 1000
                                DoseTotaleTrasformata = Dose_Tot_New / 1000
                            Case enum_UnitaMisura.CentimetriCubi
                                UdmCodTrasformato = 29
                                DoseTrasformata = Dose / 100
                                DoseTotaleTrasformata = Dose_Tot_New / 100
                            Case Else 'kg, l, n trappole etc
                                UdmCodTrasformato = Movimento_Dettaglio.Extra_Int
                                DoseTrasformata = Dose
                                DoseTotaleTrasformata = Dose_Tot_New
                        End Select


                        If Movimento_Dettaglio.Pro_Cod <> 0 Then
                            If Not HashTotNew.ContainsKey(Movimento_Dettaglio.Pro_Cod) Then
                                HashTotNew.Add(Movimento_Dettaglio.Pro_Cod, DoseTotaleTrasformata)
                            End If
                        ElseIf Movimento_Dettaglio.Mat_Cod <> 0 Then
                            If Not HashTotNew.ContainsKey(Movimento_Dettaglio.Mat_Cod & "|" & Movimento_Dettaglio.Udm_Cod & "|" & Movimento_Dettaglio.Lotto) Then
                                HashTotNew.Add(Movimento_Dettaglio.Mat_Cod & "|" & Movimento_Dettaglio.Udm_Cod & "|" & Movimento_Dettaglio.Lotto, DoseTotaleTrasformata)
                            End If
                        End If



                        'Ha_Hl
                        Movimento_Dettaglio.Mezzo_Det = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Mezzo_Det
                        'Dose_QtaTot
                        Movimento_Dettaglio.Udm_Cod_Extra = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Udm_Cod_Extra


                        Movimento_Dettaglio.Contabilizzato = NONCONTABILE

                        Movimento_Dettaglio.BaseCode = frm_BaseCode
                        Movimento_Dettaglio.TopCode = frm_TopCode

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                        '------------------------------------------------
                        '----- MOVIMENTO DETTAGLIO TECNICO
                        '------------------------------------------------

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

                        For i_movimento_dettaglio_tecnico = 0 To AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici.Count - 1

                            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                            Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                            Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                            Movimento_Dettaglio_Tecnico.Data = Data

                            'trattamenti
                            Movimento_Dettaglio_Tecnico.Av_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Av_Cod
                            Movimento_Dettaglio_Tecnico.Av_Gru = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Av_Gru

                            Movimento_Dettaglio_Tecnico.Soglia_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Soglia_Cod
                            If Movimento_Dettaglio_Tecnico.Soglia_Cod <> 0 Then
                                Movimento_Dettaglio_Tecnico.Soglia_Quantita = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Soglia_Quantita
                                Movimento_Dettaglio_Tecnico.Soglia_Des = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Soglia_Des
                            End If

                            'concimazioni
                            Movimento_Dettaglio_Tecnico.Efficienza = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Efficienza
                            Movimento_Dettaglio_Tecnico.N = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).N
                            Movimento_Dettaglio_Tecnico.P = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).P
                            Movimento_Dettaglio_Tecnico.K = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).K
                            Movimento_Dettaglio_Tecnico.M = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).M
                            Movimento_Dettaglio_Tecnico.Cu = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Cu

                            'conf/disor
                            Movimento_Dettaglio_Tecnico.Ditta_cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Ditta_cod
                            Movimento_Dettaglio_Tecnico.Sigla_av = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Sigla_av
                            Movimento_Dettaglio_Tecnico.Inn1_data = Data

                            'irrigazione
                            Movimento_Dettaglio_Tecnico.Qta_Ril = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Qta_Ril
                            Movimento_Dettaglio_Tecnico.dett_cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).dett_cod
                            Movimento_Dettaglio_Tecnico.Nitrati = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Nitrati
                            Movimento_Dettaglio_Tecnico.Inn1_data = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Inn1_data
                            Movimento_Dettaglio_Tecnico.Inn2_data = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Inn2_data
                            Movimento_Dettaglio_Tecnico.Freatimetro = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Freatimetro

                            'fertirrigazione
                            If Agenda.Lav_Cod = LAVCOD_FERTIRRIGAZIONE Then
                                Movimento_Dettaglio_Tecnico.Dose = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Dose
                                Movimento_Dettaglio_Tecnico.Parziale = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Parziale
                                Movimento_Dettaglio_Tecnico.ExtraStr = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).ExtraStr
                                Movimento_Dettaglio_Tecnico.Extra_Int = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Extra_Int
                                Movimento_Dettaglio_Tecnico.Extra_Date = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_dettaglio_tecnico).Extra_Date
                            End If

                            Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
                            Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

                            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                        Next

                        '------------------------------------------------
                        '----- MOVIMENTI DESTINAZIONI
                        '------------------------------------------------
                        Dim Sup_Imp As Decimal = 0
                        Dim Frazione As Decimal = 0
                        Dim QuantitaTotale As Decimal = 0

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)
                        'Uso il sup_imp perché sotto il qta2 viene impostato con il sup_imp
                        Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                            From ST In Dt.Rows
                            Select CType(ST.Item("sup_imp"), Decimal)
                        ).Sum

                        If Agenda.Lav_Cod = LAVCOD_FERTIRRIGAZIONE Then 'fertirrigazioni: copia di tutti i dati essendo la copia bloccata solo agli impianti originali
                            For i_mov_dest = 0 To AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni.Count - 1
                                Dim movDestDaCopiare = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(i_mov_dest)
                                movDestDaCopiare.Id_Agenda = 0
                                movDestDaCopiare.Id_Mov = 0
                                movDestDaCopiare.Id_Mov_Det = 0
                                movDestDaCopiare.Data = Data
                                movDestDaCopiare.Data_Creazione = #2/1/1900#
                                movDestDaCopiare.Data_Modifica = #2/1/1900#
                                movDestDaCopiare.BaseCode = frm_BaseCode
                                movDestDaCopiare.TopCode = frm_TopCode

                                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni.Add(movDestDaCopiare)

                            Next
                        Else
                            For j = 0 To Dt.Rows.Count - 1

                                Movimento_Destinazione = New Movimento_Destinazione

                                Movimento_Destinazione.Data = Data
                                Movimento_Destinazione.Piva = Dt.Rows(j).Item("piva")
                                Movimento_Destinazione.Sa_Cod = Dt.Rows(j).Item("sa_cod")
                                Movimento_Destinazione.Appezza = Dt.Rows(j).Item("appezza")
                                Movimento_Destinazione.Id_Destinazione = Dt.Rows(j).Item("id_reg")
                                Movimento_Destinazione.Tipo = 0

                                Sup_Imp = Dt.Rows(j).Item("sup_imp")
                                Movimento_Destinazione.Qta2 = Dt.Rows(j).Item("sup_imp")

                                Select Case Agenda.Lav_Cod

                                    Case LAVCOD_DISTRIBUZIONE_INSETTI

                                        Qta_Dest_New = Math.Round(Dose * Dt.Rows(j).Item("sup_imp"), 0)

                                    Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                                                 LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                                 LAVCOD_RACCOLTA
                                        'dose salvata sempre qta totale
                                        Qta_Dest_New = Math.Round((Dose_Tot_New / SupTrattata_Tot) * Dt.Rows(j).Item("sup_imp"), 0)
                                        Dose_Tot_New = Math.Round(Dose_Tot_New, 0)

                                        ' Movimento_Destinazione.Qta = Qta_Dest_New

                                    Case Else
                                        Qta_Dest_New = DoseTrasformata * Dt.Rows(j).Item("sup_imp")


                                End Select

                                Movimento_Destinazione.Qta = Qta_Dest_New

                                If xCalcolo_QD_SuperficieTotale <> 0 Then
                                    Movimento_Destinazione.QuotaDistribuzione = Movimento_Destinazione.Qta2 / xCalcolo_QD_SuperficieTotale
                                End If

                                ''0=HL 1=HA
                                'If Movimento.Mezzo = 0 Then
                                '    QuantitaTotale = DoseTrasformata * AcquaTot
                                'Else
                                '    QuantitaTotale = DoseTrasformata * SuperficieTotaleCentro
                                'End If

                                'Frazione = Sup_Imp / SuperficieTotaleCentro

                                'Movimento_Destinazione.Qta = QuantitaTotale * Frazione

                                Movimento_Destinazione.BaseCode = frm_BaseCode
                                Movimento_Destinazione.TopCode = frm_TopCode

                                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni.Add(Movimento_Destinazione)

                            Next
                        End If

                    Next

                    ' Controllo se devo ridurre la superficie degli impianti per trattamenti diserbo con prodotti contenenti PercAbb
                    If FlagNuovoControlloRiduzioneDiserbo Then

                        Agenda.Movimenti.ForEach(Sub(m)
                                                     m.Movimenti_Dettagli.ForEach(Sub(md)
                                                                                      If Not String.IsNullOrEmpty(md.PrincipiAttiviPercAbb) Then
                                                                                          Dim percAbbMinMovimento = OttieniPercAbbMinorePerPrincipioAttivo(md.PrincipiAttiviPercAbb)
                                                                                          If percAbbMinMovimento < PercAbbMin Then
                                                                                              PercAbbMin = percAbbMinMovimento
                                                                                          End If
                                                                                      End If
                                                                                  End Sub)

                                                 End Sub)

                        If PercAbbMin > 0 AndAlso PercAbbMin < 100 Then
                            Agenda.Movimenti.ForEach(Sub(m)
                                                         m.Movimenti_Dettagli.ForEach(Sub(md)
                                                                                          md.Qta_Extra_Totale = md.Qta_Extra_Totale * PercAbbMin / 100
                                                                                          md.Movimenti_Destinazioni.ForEach(Sub(des)
                                                                                                                                des.Qta2 = Agro_Math.ArrotondaVal_4(des.Qta2 * PercAbbMin / 100)
                                                                                                                                des.Qta = des.Qta * PercAbbMin / 100
                                                                                                                            End Sub)
                                                                                      End Sub)
                                                     End Sub)
                        End If

                    End If



                Case CAU_IMPUTAZIONE_PARCOMACCHINE, CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TERZISTI, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                    Movimento = New Movimento

                    Movimento.Piva = Agenda.Piva
                    Movimento.Sa_Cod = 0
                    Movimento.Data = Data
                    Movimento.Lav_Cod = Agenda.Lav_Cod
                    Movimento.Cau_Mov = AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov
                    Movimento.Mov_Desc = AgendaDaCopiare.Movimenti(i_movimento).Mov_Desc

                    Movimento.BaseCode = frm_BaseCode
                    Movimento.TopCode = frm_TopCode

                    Agenda.Movimenti.Add(Movimento)

                    Movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    For i_movimento_dettaglio = 0 To AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli.Count - 1

                        Movimento_Dettaglio = New Movimento_Dettaglio

                        Movimento_Dettaglio.Piva = Agenda.Piva
                        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
                        Movimento_Dettaglio.Data = Data
                        Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                        Movimento_Dettaglio.Cau_Mov = AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov

                        Movimento_Dettaglio.Elem_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Elem_Cod
                        Movimento_Dettaglio.Pro_Cod = 0
                        Movimento_Dettaglio.Mat_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Mat_Cod


                        Movimento_Dettaglio.Extra_Int = 0
                        Movimento_Dettaglio.Udm_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Udm_Cod
                        Movimento_Dettaglio.Qta = 0

                        Movimento_Dettaglio.BaseCode = frm_BaseCode
                        Movimento_Dettaglio.TopCode = frm_TopCode

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                    Next

            End Select

        Next


        For i_movimento = 0 To AgendaDaCopiare.Movimenti.Count - 1

            'Se copiamo l'operazione su un'azienda diversa, NON creo il movimento di scarico
            If Agenda.Piva <> AgendaDaCopiare.Piva Then
                Exit For
            End If

            Select Case AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov

                Case CAU_SCARICO

                    Movimento = New Movimento

                    Movimento.Piva = Agenda.Piva
                    Movimento.Sa_Cod = AgendaDaCopiare.Movimenti(i_movimento).Sa_Cod
                    Movimento.Data = Data
                    Movimento.Lav_Cod = Agenda.Lav_Cod
                    Movimento.Cau_Mov = AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov
                    Movimento.Mov_Desc = AgendaDaCopiare.Movimenti(i_movimento).Mov_Desc
                    Movimento.Mezzo = AgendaDaCopiare.Movimenti(i_movimento).Mezzo
                    Movimento.Modalita = AgendaDaCopiare.Movimenti(i_movimento).Modalita

                    Movimento.Num_Protocollo = AgendaDaCopiare.Movimenti(i_movimento).Num_Protocollo
                    Movimento.Doc_Numero = AgendaDaCopiare.Movimenti(i_movimento).Doc_Numero
                    Movimento.Disciplinare_PubblicoPrivato = AgendaDaCopiare.Movimenti(i_movimento).Disciplinare_PubblicoPrivato
                    Movimento.Extra_Int = AgendaDaCopiare.Movimenti(i_movimento).Extra_Int

                    Movimento.BaseCode = frm_BaseCode
                    Movimento.TopCode = frm_TopCode

                    Agenda.Movimenti.Add(Movimento)

                    Movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    For i_movimento_dettaglio = 0 To AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli.Count - 1

                        Movimento_Dettaglio = New Movimento_Dettaglio

                        Movimento_Dettaglio.Piva = Agenda.Piva
                        Movimento_Dettaglio.Sa_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Sa_Cod
                        Movimento_Dettaglio.Data = Data
                        Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                        Movimento_Dettaglio.Cau_Mov = AgendaDaCopiare.Movimenti(i_movimento).Cau_Mov

                        Movimento_Dettaglio.Elem_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Elem_Cod
                        Movimento_Dettaglio.Pro_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Pro_Cod
                        Movimento_Dettaglio.Mat_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Mat_Cod
                        Movimento_Dettaglio.Lotto = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Lotto

                        Movimento_Dettaglio.Extra_Int = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Extra_Int
                        Movimento_Dettaglio.Udm_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Udm_Cod

                        DoseTotaleTrasformata = 0

                        If Movimento_Dettaglio.Pro_Cod <> 0 Then
                            If HashTotNew.ContainsKey(Movimento_Dettaglio.Pro_Cod) Then
                                DoseTotaleTrasformata = HashTotNew(Movimento_Dettaglio.Pro_Cod)
                            End If
                        Else
                            If HashTotNew.ContainsKey(Movimento_Dettaglio.Mat_Cod & "|" & Movimento_Dettaglio.Udm_Cod & "|" & Movimento_Dettaglio.Lotto) Then
                                DoseTotaleTrasformata = HashTotNew(Movimento_Dettaglio.Mat_Cod & "|" & Movimento_Dettaglio.Udm_Cod & "|" & Movimento_Dettaglio.Lotto)
                            End If
                        End If

                        Movimento_Dettaglio.Qta = DoseTotaleTrasformata


                        Movimento_Dettaglio.BaseCode = frm_BaseCode
                        Movimento_Dettaglio.TopCode = frm_TopCode


                        Movimento_Dettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)


                        '-----------------------------------
                        Movimento_Destinazione = New Movimento_Destinazione

                        Movimento_Destinazione.Piva = Agenda.Piva
                        Movimento_Destinazione.Sa_Cod = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(0).Sa_Cod
                        Movimento_Destinazione.Appezza = 0
                        Movimento_Destinazione.Id_Destinazione = AgendaDaCopiare.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(0).Id_Destinazione
                        Movimento_Destinazione.Tipo = MAGAZZINO
                        Movimento_Destinazione.Data = Data

                        Movimento_Destinazione.Qta = DoseTotaleTrasformata

                        Movimento_Destinazione.BaseCode = frm_BaseCode
                        Movimento_Destinazione.TopCode = frm_TopCode

                        Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                    Next

            End Select

        Next

        Return Agenda

    End Function

    Private Function Genera_Agenda_SingolaAzienda(ByVal AgendaDaCopiare As Operazione_Agenda,
                                           ByVal Data As Date,
                                           ByVal Note As String,
                                                   ByRef strErr As String) _
                                                   As Operazione_Agenda



        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda

        Agenda = New Operazione_Agenda

        Agenda = AgendaDaCopiare

        'Data
        Agenda.Data = Data
        Agenda.Id_Agenda = 0
        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        For i = 0 To Agenda.Note.Count - 1
            Agenda.Note(i).Id_Agenda = 0
        Next

        For i_movimento = 0 To Agenda.Movimenti.Count - 1

            Agenda.Movimenti(i_movimento).Id_Agenda = 0
            Agenda.Movimenti(i_movimento).Id_Mov = 0

            Agenda.Movimenti(i_movimento).Data = Data

            'NOTA NUOVA
            Select Case Agenda.Movimenti(i_movimento).Cau_Mov
                Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA
                    Agenda.Movimenti(i_movimento).Mov_Desc = Note
                    'Case CAU_SCARICO
                    '    Agenda.Movimenti.RemoveAt(i_movimento)
                    '    Continue For
            End Select

            Agenda.Movimenti(i_movimento).BaseCode = frm_BaseCode
            Agenda.Movimenti(i_movimento).TopCode = frm_TopCode

            For i_movimento_tecnico = 0 To Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici.Count - 1

                Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(i_movimento_tecnico).Id_Agenda = 0
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(i_movimento_tecnico).Id_Mov = 0
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(i_movimento_tecnico).Id_Mov_Det = 0
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(i_movimento_tecnico).Id_Reg_Dettaglio = 0

                Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(i_movimento_tecnico).Data = Data
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(i_movimento_tecnico).BaseCode = frm_BaseCode
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli_Tecnici(i_movimento_tecnico).TopCode = frm_TopCode

            Next

            For i_movimento_dettaglio = 0 To Agenda.Movimenti(i_movimento).Movimenti_Dettagli.Count - 1

                Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Id_Agenda = 0
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Id_Mov = 0
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Id_Mov_Det = 0

                Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Data = Data
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).BaseCode = frm_BaseCode
                Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).TopCode = frm_TopCode

                Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Contabilizzato = NONCONTABILE

                For i_movimento_tecnico_2 = 0 To Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici.Count - 1

                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_tecnico_2).Id_Agenda = 0
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_tecnico_2).Id_Mov = 0
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_tecnico_2).Id_Mov_Det = 0
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_tecnico_2).Id_Reg_Dettaglio = 0

                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_tecnico_2).Data = Data
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_tecnico_2).BaseCode = frm_BaseCode
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Dettagli_Tecnici(i_movimento_tecnico_2).TopCode = frm_TopCode

                Next

                For i_movimento_destinazione = 0 To Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni.Count - 1

                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(i_movimento_destinazione).Id_Agenda = 0
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(i_movimento_destinazione).Id_Mov = 0
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(i_movimento_destinazione).Id_Mov_Det = 0

                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(i_movimento_destinazione).Data = Data
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(i_movimento_destinazione).BaseCode = frm_BaseCode
                    Agenda.Movimenti(i_movimento).Movimenti_Dettagli(i_movimento_dettaglio).Movimenti_Destinazioni(i_movimento_destinazione).TopCode = frm_TopCode

                Next

            Next

        Next

        Return Agenda

    End Function

    Private Sub RecuperaImpiantiOperazioneSelezionata()

        If objParametriAgenda.Impianti.Count = 0 Then

            ImpiantiAziendeInParametriAgenda()

            Dim DataViewImpianti As DataTable = Nothing
            Dim strImpiantiKendo As String = ""
            Carica_Impianti(objParametriAgenda, ViewState("vs_dtInterventi"), strImpiantiKendo, DataViewImpianti)

            Session("DataViewImpianti") = DataViewImpianti
            hdKendo_Impianti.Value = strImpiantiKendo

            ImpiantiAziendeImpostaSelezionati()

            Dim stb As New StringBuilder
            stb.Append("$(document).ready(function () {")
            stb.Append("    KendoImpianti_inizializza('divKendoImpianti');")
            stb.Append("    Aggiorna_hdKendo_ImpiantiSelezionaDaHidden();")
            stb.Append("});")

            ScriptManager.RegisterClientScriptBlock(UpdatePanelScript, UpdatePanelScript.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelScript.ClientID), stb.ToString, True)

        Else
            Dim DataViewImpianti As DataTable = Nothing
            Dim strImpiantiKendo As String = ""
            Carica_Impianti(objParametriAgenda, ViewState("vs_dtInterventi"), strImpiantiKendo, DataViewImpianti)

            Session("DataViewImpianti") = DataViewImpianti
            hdKendo_Impianti.Value = strImpiantiKendo

            ImpiantiAziendeImpostaSelezionati()

            Dim stb As New StringBuilder
            stb.Append("$(document).ready(function () {")
            stb.Append("    KendoImpianti_inizializza('divKendoImpianti');")
            stb.Append("    Aggiorna_hdKendo_ImpiantiSelezionaDaHidden();")
            stb.Append("});")

            ScriptManager.RegisterClientScriptBlock(UpdatePanelScript, UpdatePanelScript.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelScript.ClientID), stb.ToString, True)
        End If

    End Sub


    Private Sub ImpiantiAziendeInParametriAgenda()


        Dim objElencoImpianti = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objElencoImpiantiXProgetti = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        objParametriAgenda.Impianti.Clear()


        Dim vegCod As String
        vegCod = If(IsNothing(ViewState("Veg_Cod")), 0, ViewState("Veg_Cod"))

        'Dim DtElencoImpianti As DataTable = objElencoImpianti.Leggi_ImpiantiConSpecieSelezionata(selected.Item("piva"), 0, AGRODATAINIZIO, Qs_VegCod, "", "", objParametri_Server)
        '  Galassi, 25/07/2016 10:37:48: Con la seguente query tornano tutti gli impianti con la piva ordinati per sa_cod,appezza,id_reg,validità_fine in modo da poter utlizzare solo quelli con la distinta corrente.
        Dim DtElencoImpianti As DataTable = objElencoImpiantiXProgetti.LeggiDistinta3(objParametriAgenda.Piva, 0, 0, 0, "SpecieVegetali.veg_cod =" & vegCod, "Imprese_Progetti.Piva,Imprese_Progetti.Sa_Cod,Imprese_Progetti.Appezza,Imprese_Progetti.Id_Reg,Imprese_Progetti.Validita_fine DESC", objParametri_Server)
        If DtElencoImpianti IsNot Nothing AndAlso DtElencoImpianti.Rows.Count > 0 Then
            '  Galassi, 25/07/2016 11:39:47: Uso delle var. temp per vedere se ho già inserito la quaterna (vedi ordinamento)
            Dim sa_cod As Integer = 0
            Dim appezza As Integer = 0
            Dim id_reg As Integer = 0
            For Each tempImp As DataRow In DtElencoImpianti.Rows


                sa_cod = tempImp.Item("SA_COD")
                appezza = tempImp.Item("APPEZZA")
                id_reg = tempImp.Item("ID_REG")
                Dim objImpianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                objImpianto.Piva = tempImp.Item("PIVA")
                objImpianto.Sa_Cod = tempImp.Item("SA_COD")
                objImpianto.Appezza = tempImp.Item("APPEZZA")
                objImpianto.ID_Reg = tempImp.Item("ID_REG")
                objImpianto.Veg_Cod = tempImp.Item("Veg_Cod")
                objImpianto.Cul_Cod = tempImp.Item("CUL_COD")
                objImpianto.Sup_Imp = tempImp.Item("Sup_Imp")
                objImpianto.Validita_Inizio = tempImp.Item("Inizio_Impianto")
                objImpianto.Validita_Fine = tempImp.Item("Fine_Impianto")
                objImpianto.Validita_Inizio_Distinta = tempImp.Item("Validita_Inizio")
                objImpianto.Validita_Fine_Distinta = tempImp.Item("Validita_Fine")
                objImpianto.Progetto_Cod = tempImp.Item("Progetto_Cod")
                objImpianto.App_Nome = tempImp.Item("APP_NOME")
                objImpianto.Veg_Des = tempImp.Item("Veg_Des")
                objImpianto.Cul_Des = tempImp.Item("Cul_Des")
                'Cerco i valori necessari mancanti tramite le descrizioni..
                Dim AttributiImp = objElencoImpianti.Leggi_DescrizioniImpianti(objImpianto.Piva, objImpianto.Sa_Cod, objImpianto.Appezza, objImpianto.ID_Reg, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                'Dim AttributiImp = objElencoImpianti.Leggi(objImpianto.Piva, objImpianto.Sa_Cod, objImpianto.Appezza, objImpianto.ID_Reg, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "Imprese_Progetti.Regolamento_Cod =" & tempImp.Item("Progetto_Cod"), "", objParametri_Server)
                If AttributiImp IsNot Nothing AndAlso AttributiImp.Rows.Count > 0 Then
                    If IsDBNull(AttributiImp(0).Item("rag_soc")) Then
                        objImpianto.Rag_Soc = ""
                    Else
                        objImpianto.Rag_Soc = CStr(AttributiImp(0).Item("rag_soc"))
                    End If
                    If IsDBNull(AttributiImp(0).Item("sa_nome")) Then
                        objImpianto.Sa_Nome = ""
                    Else
                        objImpianto.Sa_Nome = CStr(AttributiImp(0).Item("sa_nome"))
                    End If
                    If IsDBNull(AttributiImp(0).Item("Campo_Cod")) Then
                        objImpianto.Campo_Cod = 0
                        objImpianto.Campo_Des = ""
                    Else
                        objImpianto.Campo_Cod = CStr(AttributiImp(0).Item("Campo_Cod"))
                        objImpianto.Campo_Des = New AgronicaCoreAnagrafeDAL.Campi_R().CampoDes_from_CampoCod(objImpianto.Piva, objImpianto.Sa_Cod, objImpianto.Campo_Cod, objParametri_Server)
                    End If
                End If
                objParametriAgenda.Impianti.Add(objImpianto)

            Next
            nrTotaleImpiantiOperazione = objParametriAgenda.Impianti.Count
        End If


    End Sub

    Private Sub ImpiantiAziendeImpostaSelezionati()
        Dim objAgenda As New Agenda_Operazione_Helper
        Dim AgendaDaCopiare As New Operazione_Agenda

        Dim dtImpiantiVisibili As DataTable = Session("DataViewImpianti")
        Dim listaDestinazioni As New List(Of String)

        'Gli impianti selezionati sono quelli salvati in variabile.
        'Se la variabile non è impostata allora li leggo dall'agenda delle operazioni selezionate
        Dim strSelezionati As String = hdImpiantiSelezionati.Value

        If strSelezionati <> "" Then

            Dim listaSelezionati As JArray = JsonConvert.DeserializeObject(strSelezionati)
            For Each elemSelezionato As JObject In listaSelezionati
                For Each impVisibili As DataRow In dtImpiantiVisibili.Rows

                    If impVisibili.Item("piva").ToString() = elemSelezionato.Item("piva").ToString() AndAlso impVisibili.Item("sa_cod").ToString() = elemSelezionato.Item("sa_cod").ToString() AndAlso
                        impVisibili.Item("appezza").ToString() = elemSelezionato.Item("appezza").ToString() AndAlso impVisibili.Item("id_reg").ToString() = elemSelezionato.Item("id_reg").ToString() Then

                        listaDestinazioni.Add(JsonConvert.SerializeObject(elemSelezionato))
                        Continue For
                    End If

                Next
            Next

            hdImpiantiSelezionati.Value = "[" & String.Join(",", listaDestinazioni.ToArray) & "]"

        Else

            Dim ListaOperazioniIn As List(Of AgronicaCoreModello.DuplicaOperazione_parametriInput) = getListaIn()

            If Not IsNothing(ListaOperazioniIn) Then

                AgendaDaCopiare = objAgenda.Leggi("", 0, ListaOperazioniIn.First.Id_agenda, 0, objParametri_Server)

                For Each mm In AgendaDaCopiare.Movimenti
                    For Each dd In mm.Movimenti_Dettagli
                        For Each iSel In dd.Movimenti_Destinazioni
                            If iSel.Tipo = 0 Then

                                'Seleziono l'impianto solo se è contenuto nell'elenco degli impianti visibili (e filtrati in base alle date delle operazioni d'agenda)
                                For Each impVisibili As DataRow In dtImpiantiVisibili.Rows

                                    If impVisibili.Item("piva") = iSel.Piva AndAlso impVisibili.Item("sa_cod") = iSel.Sa_Cod AndAlso impVisibili.Item("appezza") = iSel.Appezza AndAlso impVisibili.Item("id_reg") = iSel.Id_Destinazione Then
                                        'Dim sImp As String = "{""piva"":""" & iSel.Piva & """,""sa_cod"":""" & iSel.Sa_Cod & """,""appezza"":""" & iSel.Appezza & """,""id_reg"":""" & iSel.Id_Destinazione & """,""rag_soc"":"""",""sa_nome"":"""",""sup_imp"":0,""descrizione"":""""}"
                                        Dim sImp As String = "{""piva"":""" & iSel.Piva & """,""sa_cod"":""" & iSel.Sa_Cod & """,""appezza"":""" & iSel.Appezza & """,""id_reg"":""" & iSel.Id_Destinazione & """,""rag_soc"":"""",""sa_nome"":"""",""sup_imp"":0,""descrizione"":"""",""validita_inizio_distinta"":""" & impVisibili.Item("validita_inizio_distinta") & """,""validita_fine_distinta"":""" & impVisibili.Item("validita_fine_distinta") & """}"
                                        listaDestinazioni.Add(sImp)
                                        Continue For
                                    End If

                                Next

                            End If

                        Next
                    Next
                Next

                hdImpiantiSelezionati.Value = "[" & String.Join(",", listaDestinazioni.ToArray) & "]"
            End If

        End If

    End Sub

    Private Sub RecuperaImpiantiDaIDAgenda()


        Dim objElencoImpianti = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objElencoImpiantiXProgetti = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        objParametriAgenda.Impianti.Clear()

        'Dim DtElencoImpianti As DataTable = objElencoImpianti.Leggi_ImpiantiConSpecieSelezionata(selected.Item("piva"), 0, AGRODATAINIZIO, Qs_VegCod, "", "", objParametri_Server)
        '  Galassi, 25/07/2016 10:37:48: Con la seguente query tornano tutti gli impianti con la piva ordinati per sa_cod,appezza,id_reg,validità_fine in modo da poter utlizzare solo quelli con la distinta corrente.
        Dim DtElencoImpianti As DataTable = objElencoImpiantiXProgetti.LeggiDistinta3(objParametriAgenda.Piva, 0, 0, 0, "", "Imprese_Progetti.Piva,Imprese_Progetti.Sa_Cod,Imprese_Progetti.Appezza,Imprese_Progetti.Id_Reg,Imprese_Progetti.Validita_fine DESC", objParametri_Server, joinMovDestinazioni:=True, lista_Id_Agenda:=ids)
        If DtElencoImpianti IsNot Nothing AndAlso DtElencoImpianti.Rows.Count > 0 Then
            '  Galassi, 25/07/2016 11:39:47: Uso delle var. temp per vedere se ho già inserito la quaterna (vedi ordinamento)
            Dim sa_cod As Integer = 0
            Dim appezza As Integer = 0
            Dim id_reg As Integer = 0
            For Each tempImp As DataRow In DtElencoImpianti.Rows


                sa_cod = tempImp.Item("SA_COD")
                appezza = tempImp.Item("APPEZZA")
                id_reg = tempImp.Item("ID_REG")
                Dim objImpianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                objImpianto.Piva = tempImp.Item("PIVA")
                objImpianto.Sa_Cod = tempImp.Item("SA_COD")
                objImpianto.Appezza = tempImp.Item("APPEZZA")
                objImpianto.ID_Reg = tempImp.Item("ID_REG")
                objImpianto.Veg_Cod = tempImp.Item("Veg_Cod")
                objImpianto.Cul_Cod = tempImp.Item("CUL_COD")
                objImpianto.Sup_Imp = tempImp.Item("Sup_Imp")
                objImpianto.Validita_Inizio = tempImp.Item("Inizio_Impianto")
                objImpianto.Validita_Fine = tempImp.Item("Fine_Impianto")
                objImpianto.Validita_Inizio_Distinta = tempImp.Item("Validita_Inizio")
                objImpianto.Validita_Fine_Distinta = tempImp.Item("Validita_Fine")
                objImpianto.Progetto_Cod = tempImp.Item("Progetto_Cod")
                objImpianto.App_Nome = tempImp.Item("APP_NOME")
                objImpianto.Veg_Des = tempImp.Item("Veg_Des")
                objImpianto.Cul_Des = tempImp.Item("Cul_Des")
                'Cerco i valori necessari mancanti tramite le descrizioni..
                Dim AttributiImp = objElencoImpianti.Leggi_DescrizioniImpianti(objImpianto.Piva, objImpianto.Sa_Cod, objImpianto.Appezza, objImpianto.ID_Reg, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                'Dim AttributiImp = objElencoImpianti.Leggi(objImpianto.Piva, objImpianto.Sa_Cod, objImpianto.Appezza, objImpianto.ID_Reg, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "Imprese_Progetti.Regolamento_Cod =" & tempImp.Item("Progetto_Cod"), "", objParametri_Server)
                If AttributiImp IsNot Nothing AndAlso AttributiImp.Rows.Count > 0 Then
                    If IsDBNull(AttributiImp(0).Item("rag_soc")) Then
                        objImpianto.Rag_Soc = ""
                    Else
                        objImpianto.Rag_Soc = CStr(AttributiImp(0).Item("rag_soc"))
                    End If
                    If IsDBNull(AttributiImp(0).Item("sa_nome")) Then
                        objImpianto.Sa_Nome = ""
                    Else
                        objImpianto.Sa_Nome = CStr(AttributiImp(0).Item("sa_nome"))
                    End If
                    If IsDBNull(AttributiImp(0).Item("Campo_Cod")) Then
                        objImpianto.Campo_Cod = 0
                        objImpianto.Campo_Des = ""
                    Else
                        objImpianto.Campo_Cod = CStr(AttributiImp(0).Item("Campo_Cod"))
                        objImpianto.Campo_Des = New AgronicaCoreAnagrafeDAL.Campi_R().CampoDes_from_CampoCod(objImpianto.Piva, objImpianto.Sa_Cod, objImpianto.Campo_Cod, objParametri_Server)
                    End If
                End If
                objParametriAgenda.Impianti.Add(objImpianto)

            Next
        End If


    End Sub

    Protected Sub Btn_SalvaTutto_Click(sender As Object, e As EventArgs) Handles Btn_SalvaTutto.Click

        Dim Stringa As String
        Dim i As Integer
        Dim Log_Errori As String = ""
        Dim Alert_Errori As String = ""
        Dim Flag_Insert As Boolean = False

        Dim global_Messaggio As String = ""
        Dim global_Flag_Insert As Boolean = True
        Dim global_Log_errori As String = ""
        Dim global_Alert_Errori As String = ""

        Dim global_Raccoglitore_Alert_Errori As String = ""

        Dim dic_Raccoglitori As New Dictionary(Of Integer, Integer) '--old raccoglitore_cod, new raccoglitore_cod

        Dim isFromNG As Boolean = If(objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG, True, False)

        Dim gruppoRaccoglitore_selected As Boolean = False

        Stringa = ViewState("StringaXML")
        frm_DataOperazione = ViewState("Data")
        frm_NoteIntervento = ViewState("Note")

        If DataGridInterventi.Rows.Count = 0 Then
            Messaggi.AgroMsgBox("Selezionare almeno un intervento!", Page, , UpdatePanelScript)
            Exit Sub
        End If

        If hd_isFertirrigazione.Value = True Then
            Dim nrImpiantiSelezionati = getListaImpiantiSelezionati().Count
            If (nrImpiantiSelezionati < nrTotaleImpiantiOperazione) Then
                Messaggi.AgroMsgBox("Per copiare le fertirrigazioni è necessario scegliere tutti gli impianti!", Page, , UpdatePanelScript)
                Exit Sub
            End If
        End If

        '---------------------------------------------------
        '----- BaseCode  +  TopCode
        '---------------------------------------------------
        Dim frm_BaseCode, frm_TopCode As Integer
        Calcola_BaseCode_TopCode(frm_BaseCode, frm_TopCode, Session("ASG_ProgressivoGIAS"))

        Dim Messaggio As String = ""


        '---------------------------------------------------
        '----- IMPOSTAZIONI UTENTE
        '---------------------------------------------------
        Session("ControllaBlocco") = "0"
        Session("UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS") = False
        Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = False

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni As DataTable

        Dt_Impostazioni = ObjUtenti.Leggi(0,
                                          1,
                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri_Utenti)

        If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then
            For i = 0 To Dt_Impostazioni.Rows.Count - 1

                Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                    'Blocco DPI
                    Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME
                        Session("ControllaBlocco") = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")

                        'Blocco concimazioni
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS
                        If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                            Session("UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS") = True
                        End If

                        'blocco se supera giacenze
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE
                        If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                            Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True
                        End If

                End Select
            Next
        End If


        '---------------------------------------------------
        '----- DISTINCT OPERAZIONI SELEZIONATE
        '---------------------------------------------------
        Dim ListaOperazioniIn As List(Of AgronicaCoreModello.DuplicaOperazione_parametriInput) = getListaIn()
        'Per le multicentro seleziono solo la prima operazione per ogni coppia raccoglitore_cod-lav_cod
        filtraOperazioniMultiCentro(ListaOperazioniIn)

        Dim ListaOperazioniIn_Count = ListaOperazioniIn.Count


        '---------------------------------------------------
        '----- ORDINAMENTO OPERAZIONI SELEZIONATE
        '---------------------------------------------------
        'Ordino le operazioni per Raccoglitore_Cod, così da poter identificare l'ultimo elemento di ogni gruppo
        ListaOperazioniIn = ListaOperazioniIn.OrderBy(Function(x) x.Raccoglitore_Cod).ToList()

        'Ciclo le operazioni ordinate per Raccoglitore_Cod, per identificare di ogni gruppo l'ultimo elemento presente in lista
        Dim Raccoglitore_Cod_Precedente As Integer = -1
        Dim Raccoglitore_Cod_Corrente As Integer

        For i = 0 To ListaOperazioniIn.Count - 1
            Raccoglitore_Cod_Corrente = ListaOperazioniIn(i).Raccoglitore_Cod

            'Inizio i controlli solo dopo la prima iterazione (per aver popolato il precedente)
            If i > 0 Then
                'Se il raccoglitore cod dell'operazione corrente è diverso dall'ultimo salvato, significato che l'elemento precedente era l'ultimo del suo gruppo
                If Raccoglitore_Cod_Corrente <> Raccoglitore_Cod_Precedente Then
                    ListaOperazioniIn(i - 1).isLastxMultiAttivita = True
                End If
            End If

            'Se sono all'ultima iterazione, imposto l'elemento come ultimo
            If i = ListaOperazioniIn.Count - 1 Then
                ListaOperazioniIn(i).isLastxMultiAttivita = True
            End If

            Raccoglitore_Cod_Precedente = Raccoglitore_Cod_Corrente
            'Se ho appena passato una singola operazione, imposto il precedente a -1 così da non raggruupare le operazioni singole 
            If Raccoglitore_Cod_Precedente = 0 Then
                Raccoglitore_Cod_Precedente = -1
            End If
        Next

        'Se seleziono un'operazione singola oppure un multi è possibile scegliere la data intervento.
        'Il controllo all'interno del Copia si basa sul numero di operazioni selezionate,
        'per il multi attività controllo il numero di raccoglitori presenti:
        'se il distinct risponde con una sola riga, allora stiamo salvando un multi e possiamo applicare la data selezionata dall'utente
        'se ci sono più righe significa che l'utente ha selezionato N operazioni diverse
        Dim dummyDistinctRaccoglitori = ListaOperazioniIn.Select(Function(x) x.Raccoglitore_Cod).Distinct().ToList()
        If dummyDistinctRaccoglitori.Count = 1 Then
            'Se ho una sola riga controllo che sia <> "0" --> potrei aver selezionato 10 operazioni diverse senza raccoglitore, quindi non è possibile scegliere una data
            If dummyDistinctRaccoglitori(0) <> "0" Then
                gruppoRaccoglitore_selected = True
            End If
        End If

        '---------------------------------------------------
        '----- DT OPERAZIONI NON SALVATE
        '---------------------------------------------------
        'dt appoggio per capire quelle non salvate per controlli
        Dim DtOperazioniNonSalvate As New DataTable
        Dim FlagSalvaNonConformi As Boolean = False

        If Session("DtOperazioniNonSalvate") IsNot Nothing AndAlso HiddenVarie.Value <> "" Then
            DtOperazioniNonSalvate = Session("DtOperazioniNonSalvate")
            FlagSalvaNonConformi = True
        Else
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("piva", GetType(String)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("rag_soc", GetType(String)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("sa_nome", GetType(String)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("data", GetType(DateTime)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("strErrore", GetType(String)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("strAlert", GetType(String)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("des_lib", GetType(String)))

            DtOperazioniNonSalvate.Columns.Add(New DataColumn("id_agenda_originale", GetType(Integer)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("raccoglitore_cod_originale", GetType(Integer)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("piva_originale", GetType(String)))
            DtOperazioniNonSalvate.Columns.Add(New DataColumn("lav_cod", GetType(Integer)))

        End If

        'Creo un dataTable identico per contenere le operazioni con hanno un raccoglitore_cod <> 0
        'Mi servirà al salvataggio dopo la conferma degli alert per scrivere le operazioni con lo stesso raccoglitore_cod insieme
        Dim DtOperazioniRaccoglitore As New DataTable
        DtOperazioniRaccoglitore = DtOperazioniNonSalvate.Clone()

        '---------------------------------------------------
        '----- COPIA OPERAZIONI
        '---------------------------------------------------
        Dim N_Op As Integer = 0
        Dim N_Op_Salvate As Integer = 0
        Dim N_Op_Non_Salvate As Integer = 0

        For Each OperazioneIn As AgronicaCoreModello.DuplicaOperazione_parametriInput In ListaOperazioniIn

            If Not CheckValiditaDistinte(OperazioneIn, ListaOperazioniIn_Count, gruppoRaccoglitore_selected) Then
                Exit Sub
            End If

            Raccoglitore_Cod_Corrente = OperazioneIn.Raccoglitore_Cod
            N_Op += 1

            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            '-----------------------------------------------------
            'Se siamo alla prima iterazione, oppure ad un gruppo diverso dal precedente:
            'apro una nuova tranasazione, così da poter tenere separati i gruppi
            If N_Op = 1 OrElse Raccoglitore_Cod_Corrente <> Raccoglitore_Cod_Precedente Then
                'Resetto il messaggi globale ad ogni nuovo gruppo
                global_Raccoglitore_Alert_Errori = ""
                ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            End If

            ' Vanni: 18/3/2017: qui si deve innestare il ciclo di copia.
            CopiaUnOperazione(OperazioneIn.Id_agenda, OperazioneIn.Data, OperazioneIn.Note, ListaOperazioniIn_Count,
                              Messaggio, Flag_Insert, Log_Errori, Alert_Errori,
                              FlagSalvaNonConformi, DtOperazioniNonSalvate, dic_Raccoglitori,
                              DtOperazioniRaccoglitore:=DtOperazioniRaccoglitore,
                              isFromNG:=isFromNG, gruppoRaccoglitore_selected:=gruppoRaccoglitore_selected)

            global_Messaggio &= Messaggio
            global_Alert_Errori &= Alert_Errori
            global_Log_errori &= Log_Errori
            global_Flag_Insert = (global_Flag_Insert AndAlso Flag_Insert)

            global_Raccoglitore_Alert_Errori &= Alert_Errori

            'Se questa operazione è l'ultima del suo gruppo, confermo/annullo la transazione
            If OperazioneIn.isLastxMultiAttivita Then
                If Not Flag_Insert OrElse Log_Errori <> "" OrElse Alert_Errori <> "" OrElse global_Raccoglitore_Alert_Errori <> "" Then
                    'inserimento fallito
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
                    N_Op_Non_Salvate += 1
                Else
                    'chiudi connessione e commit transazione
                    ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                    ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
                    N_Op_Salvate += 1
                End If
            End If

            'RESETTO VARIABLI
            Messaggio = ""
            Alert_Errori = ""
            Log_Errori = ""


            Raccoglitore_Cod_Precedente = Raccoglitore_Cod_Corrente
            'Se ho appena passato una singola operazione, imposto il precedente a -1 così da non raggruupare le operazioni singole alla prossima iterazione
            If Raccoglitore_Cod_Precedente = 0 Then
                Raccoglitore_Cod_Precedente = -1
            End If

        Next

        '---------------------------------------------------
        '----- SALVATI TUTTI GLI ELEMENTI
        '---------------------------------------------------
        If DtOperazioniNonSalvate.Rows.Count = 0 Then

            AAA_GestioneUscitaPagina()

        Else

            '---------------------------------------------------
            '----- ELEMENTI NON SALVATI
            '---------------------------------------------------
            If Not FlagSalvaNonConformi Then
                Dim listOperazioniRaccoglitore As New List(Of String)

                'operazioni NON salvate a seguito di controlli
                Dim strOperazioniNONConformiErrore As String = ""
                Dim strOperazioniNONConformiAlert As String = ""
                Dim str As String = ""
                For o = 0 To DtOperazioniNonSalvate.Rows.Count - 1
                    '---------------------------------------------------
                    '----- PREP MESSAGGIO BASE
                    '---------------------------------------------------
                    str = GetMsgBase(DtOperazioniNonSalvate.Rows(o).Item("des_lib"), DtOperazioniNonSalvate.Rows(o).Item("rag_soc"),
                                        DtOperazioniNonSalvate.Rows(o).Item("sa_nome"), DtOperazioniNonSalvate.Rows(o).Item("data"))

                    '---------------------------------------------------
                    '----- AGGIUNTA ERRORE/AVVISO VERO E PROPRIO
                    '---------------------------------------------------
                    If DtOperazioniNonSalvate.Rows(o).Item("strErrore") <> "" Then
                        strOperazioniNONConformiErrore &= str & NEWLINE & "--> " & DtOperazioniNonSalvate.Rows(o).Item("strErrore") & NEWLINE
                    End If
                    If DtOperazioniNonSalvate.Rows(o).Item("strAlert") <> "" Then
                        strOperazioniNONConformiAlert &= str & NEWLINE & "--> " & DtOperazioniNonSalvate.Rows(o).Item("strAlert") & NEWLINE
                    End If

                    '---------------------------------------------------
                    '----- ESTRAZIONE OPERAZIONI RACCOGLITORE
                    '---------------------------------------------------
                    estraiAltreOperazioniRaccoglitore(DtOperazioniNonSalvate.Rows(o), listOperazioniRaccoglitore, objParametri_Server)
                Next

                '---------------------------------------------------
                '----- AGGIUNTA OPERAZIONI RACCOGLITORE (non in errore)
                '---------------------------------------------------
                Dim dummy As DataTable = DtOperazioniNonSalvate.Clone()
                'Ciclo le operazioni non salvate, se hanno il raccoglitore_cod valorizzato cerco le loro sorelle così da poterle salvare insieme dopo una conferma
                For Each operazione In DtOperazioniNonSalvate.Rows
                    For Each raccoglitore In DtOperazioniRaccoglitore.Rows
                        If operazione.item("raccoglitore_cod_originale") = raccoglitore.item("raccoglitore_cod_originale") AndAlso
                           operazione.item("id_agenda_originale") <> raccoglitore.item("id_agenda_originale") Then
                            'Se entro qui vuol dire che ho le stesso raccoglitore_cod, ma sto guardando un'operazione diversa
                            Dim drDummy = dummy.NewRow()

                            drDummy.Item("piva") = raccoglitore.Item("piva")
                            drDummy.Item("sa_cod") = raccoglitore.Item("sa_cod")
                            drDummy.Item("rag_soc") = raccoglitore.Item("rag_soc")
                            drDummy.Item("sa_nome") = raccoglitore.Item("sa_nome")
                            drDummy.Item("id_agenda") = raccoglitore.Item("id_agenda")
                            drDummy.Item("data") = raccoglitore.Item("data")
                            drDummy.Item("strErrore") = raccoglitore.Item("strErrore")
                            drDummy.Item("strAlert") = raccoglitore.Item("strAlert")
                            drDummy.Item("des_lib") = raccoglitore.Item("des_lib")

                            drDummy.Item("id_agenda_originale") = raccoglitore.Item("id_agenda_originale")
                            drDummy.Item("raccoglitore_cod_originale") = raccoglitore.Item("raccoglitore_cod_originale")
                            drDummy.Item("piva_originale") = raccoglitore.Item("piva_originale")
                            drDummy.Item("lav_cod") = raccoglitore.Item("lav_cod")

                            dummy.Rows.Add(drDummy)

                        End If
                    Next
                Next
                If dummy.Rows.Count > 0 Then
                    DtOperazioniNonSalvate.Merge(dummy)
                End If


                '---------------------------------------------------
                '----- GESTIONE ERRORE / AVVISI
                '---------------------------------------------------
                'se ci sono errori blocco e avviso
                If strOperazioniNONConformiErrore <> "" Then
                    Dim Msg As String = ""
                    Msg &= "<b> " & Gias.ATTENZIONESeguentiOperazioniNONSalvateNONCONFORMI & ": </b>" & NEWLINE & NEWLINE & strOperazioniNONConformiErrore

                    If strOperazioniNONConformiAlert <> "" Then
                        Msg &= NEWLINE & NEWLINE & strOperazioniNONConformiAlert
                    End If

                    If listOperazioniRaccoglitore.Count > 0 Then
                        Msg &= "<b> " & Gias.Attenzione.ToUpper() & " </b> " & Gias.OperazioniRegistrateInsiemeSeguentiNessunaCopiata & ": </b> " & NEWLINE & String.Join(NEWLINE, listOperazioniRaccoglitore)
                    End If

                    'Visualizzo il messaggio di errore
                    Messaggi.AgroMsgBox(Msg, Page, , UpdatePanelScript)

                Else

                    Session("DtOperazioniNonSalvate") = DtOperazioniNonSalvate

                    Dim Msg As String = ""
                    'se ci sono solo avvisi faccio scegliere all'utente
                    If strOperazioniNONConformiAlert <> "" Then
                        Msg &= "<b> " & Gias.ATTENZIONESeguentiOperazioniNONSalvateNONCONFORMI & ": </b>" & NEWLINE & NEWLINE &
                        vbCrLf & vbCrLf &
                        strOperazioniNONConformiAlert

                        If listOperazioniRaccoglitore.Count > 0 Then
                            Msg &= "<b> " & Gias.Attenzione.ToUpper() & " </b> " & Gias.OperazioniRegistrateInsiemeSeguentiNessunaCopiataSeNonSalvataggio & ": " & NEWLINE & String.Join(NEWLINE, listOperazioniRaccoglitore) & NEWLINE & NEWLINE
                        End If

                        ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Salva"
                        Messaggi.AgroSiNo(Msg & vbCrLf & vbCrLf & Gias.ProcedereUgualmenteAlSalvataggio & "?" & vbCrLf, "Salva", Page, , UpdatePanelScript)
                    End If

                End If

            Else

                AAA_GestioneUscitaPagina()

            End If

        End If

        '03/01 Riassegnato l'objParametri_Server e objParametri_Utenti alla sessione per far sì che venga resettato l'objConnessione
        HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server

        HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

    End Sub

    Private Shared Sub estraiAltreOperazioniRaccoglitore(OperazioneNonSalvata As DataRow,
                                                         ByRef listOperazioniRaccoglitore As List(Of String),
                                                         objParametri_Server As AgronicaCoreParametri
                                                         )

        Dim id_agenda_originale As String = OperazioneNonSalvata.Item("id_agenda_originale")
        Dim raccoglitore_cod_originale As String = OperazioneNonSalvata.Item("raccoglitore_cod_originale")
        Dim piva_originale As String = OperazioneNonSalvata.Item("piva_originale")
        Dim lav_cod As Integer = OperazioneNonSalvata.Item("lav_cod")

        If raccoglitore_cod_originale <> 0 Then
            'Estraggo le altre operazioni dello stesso stesso raccoglitore
            'Escludo anche il lav_cod corrente in caso di multicentro
            Dim obj_Agenda_R As New AgronicaCoreContabDAL.Agenda_R
            Dim DT_Agenda_Raccoglitori = obj_Agenda_R.Leggi(piva_originale, 0, 0, 0,
                                                            enumSelezioneVariabile.Selezione_TabellaCompleta, " (Raccoglitore_Cod = " & raccoglitore_cod_originale & " AND Id_Agenda <> " & id_agenda_originale & ") AND lav_cod <> " & lav_cod, "", objParametri_Server)

            If DT_Agenda_Raccoglitori.Rows.Count > 0 Then
                For Each operazione In DT_Agenda_Raccoglitori.Rows
                    listOperazioniRaccoglitore.Add("- <b>" & operazione.Item("des_lib") & "</b> " & Gias.del & " " & operazione.Item("validita_inizio"))
                Next
            End If

            'Faccio il distinct delle operazioni in caso di multicentro
            listOperazioniRaccoglitore = listOperazioniRaccoglitore.Distinct().ToList()
        End If
    End Sub

    Private Function getListaIn() As List(Of AgronicaCoreModello.DuplicaOperazione_parametriInput)


        Dim sss As String = hdOperazioniSelezionate.Value

        Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModello.DuplicaOperazione_parametriInput))(sss)

    End Function


    Private Sub CopiaUnOperazione(ByVal id_agenda As Integer,
                                  ByVal data As Date,
                                  ByVal nota As String,
                                  ByVal ListaOperazioniIn_Count As Integer,
                                  ByRef Messaggio As String,
                                  ByRef Flag_insert As Boolean,
                                  ByRef Log_errori As String,
                                  ByRef Alert_Errori As String,
                                  ByVal FlagSalvaNonConformi As Boolean,
                                  ByRef DtOperazioniNonSalvate As DataTable,
                                    Optional ByRef dic_Raccoglitori As Dictionary(Of Integer, Integer) = Nothing,
                                    Optional DtOperazioniRaccoglitore As DataTable = Nothing,
                                    Optional isFromNG As Boolean = False,
                                    Optional gruppoRaccoglitore_selected As Boolean = False)

        Dim Agenda As Operazione_Agenda
        Dim objAgenda As New Agenda_Operazione_Helper
        Dim AgendaDaCopiare As New Operazione_Agenda

        Dim Rag_Soc As String = ""
        Dim Sa_Nome As String = ""

        Dim raccoglitore_cod As Integer

        Dim DrOperazioniNonSalvate As DataRow
        Dim DrOperazioniRaccoglitore As DataRow


        Dim ListaImpiantiSelezionati As List(Of AgronicaCoreModello.DuplicaOperazione_Impianto) =
            getListaImpiantiSelezionati()

        Try

            AgendaDaCopiare = objAgenda.Leggi("", 0,
                                              id_agenda, 0,
                                              objParametri_Server)

            If AgendaDaCopiare.Raccoglitore_Cod <> 0 Then
                If dic_Raccoglitori.ContainsKey(AgendaDaCopiare.Raccoglitore_Cod) Then
                    raccoglitore_cod = dic_Raccoglitori(AgendaDaCopiare.Raccoglitore_Cod)
                Else
                    Dim objSequenze As New Agro_Sequenze
                    Dim InfoOperazione = New InfoOperazione
                    raccoglitore_cod = objSequenze.NuovoId_Tabella("raccoglitore", InfoOperazione.BaseCode, InfoOperazione.TopCode, objParametri_Server)

                    dic_Raccoglitori.Add(AgendaDaCopiare.Raccoglitore_Cod, raccoglitore_cod)
                End If
            End If


            If Not chk_ImpostaCopiaCosti.Checked Then
                'SE NON E' CHECKATO ALLORA ESCLUDO DAI MOVIMENTI I SEGUENTI CAU_MOV...
                Dim NewMM As New List(Of Movimento)
                For Each mm In AgendaDaCopiare.Movimenti

                    If Not {CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE, CAU_IMPUTAZIONE_TERZISTI, CAU_IMPUTAZIONE_COSTISTANDARD, CAU_IMPUTAZIONE_PARCOMACCHINE}.Contains(mm.Cau_Mov) Then
                        NewMM.Add(mm)
                    End If

                Next

                AgendaDaCopiare.Movimenti = NewMM

            End If

            For i = 0 To Me.DataGridInterventi.Rows.Count - 1

                SetData(Me.DataGridInterventi.Rows(i), ListaOperazioniIn_Count, gruppoRaccoglitore_selected, data)

                SetNota(Me.DataGridInterventi.Rows(i), ListaOperazioniIn_Count, gruppoRaccoglitore_selected, nota)

                ' VAnni: 18/3/2017: verificare la condizione su riga_impianti.
                'If Riga_Impianti.Visible = True And objParametriAgenda.Impianti.Count > 0 Then
                If objParametriAgenda.Impianti.Count > 0 Then

                    Dim DtImpiantiSelezionati As New DataTable
                    Dim Dr As DataRow

                    Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

                    Dim strPiva() As String
                    Dim strCentri() As String

                    Dim DrImprese() As DataRow
                    Dim DrCentri() As DataRow


                    '----- Definisco la struttura del DataTable
                    DtImpiantiSelezionati.Columns.Add(New DataColumn("piva", GetType(String)))
                    DtImpiantiSelezionati.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
                    DtImpiantiSelezionati.Columns.Add(New DataColumn("appezza", GetType(Integer)))
                    DtImpiantiSelezionati.Columns.Add(New DataColumn("id_reg", GetType(Integer)))
                    DtImpiantiSelezionati.Columns.Add(New DataColumn("descrizione", GetType(String)))
                    DtImpiantiSelezionati.Columns.Add(New DataColumn("sup_imp", GetType(Decimal)))

                    DtImpiantiSelezionati.Columns.Add(New DataColumn("rag_soc", GetType(String)))
                    DtImpiantiSelezionati.Columns.Add(New DataColumn("sa_nome", GetType(String)))

                    For Each ccListaImpiantiSelezionati In ListaImpiantiSelezionati

                        '(17/09/2018 fede) escludo le distinte non attive
                        If CheckValiditaDistinta(ccListaImpiantiSelezionati.validita_inizio_distinta, ccListaImpiantiSelezionati.validita_fine_distinta, data) Then
                            'Creo una nuova riga
                            Dr = DtImpiantiSelezionati.NewRow

                            'Definisco i valori
                            Dr.Item("piva") = ccListaImpiantiSelezionati.piva
                            Dr.Item("sa_cod") = ccListaImpiantiSelezionati.sa_cod
                            Dr.Item("appezza") = ccListaImpiantiSelezionati.appezza
                            Dr.Item("id_reg") = ccListaImpiantiSelezionati.id_reg
                            Dr.Item("descrizione") = ccListaImpiantiSelezionati.descrizione
                            Dr.Item("sup_imp") = ccListaImpiantiSelezionati.sup_imp
                            Dr.Item("rag_soc") = ccListaImpiantiSelezionati.rag_soc
                            Dr.Item("sa_nome") = ccListaImpiantiSelezionati.sa_nome

                            DtImpiantiSelezionati.Rows.Add(Dr)

                        End If
                    Next

                    Dim DtImpresa As New DataTable
                    DtImpresa = DtImpiantiSelezionati.Clone

                    Dim DtCentro As New DataTable
                    DtCentro = DtImpiantiSelezionati.Clone

                    'seleziono le imprese distinte
                    strPiva = objSqlDis.SelectDistinct(DtImpiantiSelezionati, "piva")

                    If strPiva IsNot Nothing AndAlso strPiva.Length > 0 Then

                        For impresa = 0 To strPiva.Length - 1

                            Rag_Soc = ""

                            DtImpresa.Rows.Clear()
                            DrImprese = DtImpiantiSelezionati.Select("piva='" & strPiva(impresa) & "'")

                            For j = 0 To DrImprese.Length - 1
                                If j = 0 Then
                                    Rag_Soc = DrImprese(j).Item("rag_soc")
                                End If
                                DtImpresa.ImportRow(DrImprese(j))
                            Next

                            'seleziono i centri per ogni impresa
                            strCentri = objSqlDis.SelectDistinct(DtImpresa, "sa_cod")

                            If strCentri IsNot Nothing AndAlso strCentri.Length > 0 Then

                                For x = 0 To strCentri.Length - 1

                                    Sa_Nome = ""

                                    DtCentro.Rows.Clear()
                                    DrCentri = DtImpresa.Select("piva='" & strPiva(impresa) & "' AND sa_cod='" & strCentri(x) & "'")

                                    For j = 0 To DrCentri.Length - 1
                                        If j = 0 Then
                                            Sa_Nome = DrCentri(j).Item("sa_nome")
                                        End If
                                        DtCentro.ImportRow(DrCentri(j))
                                    Next

                                    'salvo un'operazione per tutti gli impianti appartenenti ad uno stesso centro
                                    Dim strErr As String = ""
                                    Agenda = Genera_Agenda_MultiAziendale(AgendaDaCopiare, data, nota, DtCentro, strErr)

                                    If IsNothing(Agenda) Then
                                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                                    End If

                                    'Aggiungo raccoglitore_cod se esiste 
                                    If raccoglitore_cod <> 0 Then
                                        Agenda.Raccoglitore_Cod = raccoglitore_cod
                                    End If

                                    'verifico se l'operazione era stata bloccata
                                    If FlagSalvaNonConformi Then

                                        Dim DrOpDaSalvare As DataRow() = DtOperazioniNonSalvate.Select("piva='" & Agenda.Piva & "' AND sa_cod=" & Agenda.Sa_Cod & " AND id_agenda=" & id_agenda)
                                        If DrOpDaSalvare IsNot Nothing AndAlso DrOpDaSalvare.Length > 0 Then

                                            Dim objAgendaScrivi As New Agenda_Operazione_Helper
                                            Dim Id_Agenda_Nuovo As Integer = 0

                                            Id_Agenda_Nuovo = objAgendaScrivi.Scrivi(Agenda, objParametri_Server, IdAgendaCopiata_xLOG:=" (Id_Agenda copiata: " & AgendaDaCopiare.Id_Agenda & ")")

                                            Flag_insert = True

                                            Messaggio &= data & " - " & Agenda.Des_Lib & " - " & Rag_Soc & " - " & Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso &
                                                         Chr(13)

                                        End If
                                    Else


                                        '-------------------------------------------------------
                                        '-------------------------------------------------------
                                        '------------ CONTROLLI --------------------------------
                                        '-------------------------------------------------------
                                        '-------------------------------------------------------

                                        ' CONTROLLO CONFORMITA'

                                        Dim CorrettoConformita As Boolean = True
                                        Dim Flag_ControllaBlocco As Integer = 0

                                        Dim MessaggioControlloConformita As String = ""
                                        Dim MessaggioControlloMagazzino As String = ""

                                        Dim strErrore As String = ""
                                        Dim strAlert As String = ""

                                        Select Case Agenda.Lav_Cod

                                            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                                 LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                                 LAVCOD_CONCIA_SEME,
                                                 LAVCOD_GEODISINFESTAZIONE,
                                                 LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                                                 LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE

                                                '(31/05/2016) introdotta impostazione x dare alert
                                                Select Case Session("ControllaBlocco")

                                                    Case "1", "2" '1=blocco 2=alert

                                                        Flag_ControllaBlocco = CInt(Session("ControllaBlocco"))

                                                        CorrettoConformita = VerificaDPI(Agenda, MessaggioControlloConformita)

                                                        If Session("ControllaBlocco") = "1" Then
                                                            If Not CorrettoConformita Then
                                                                strErrore = MessaggioControlloConformita
                                                                If Log_errori = "" Then
                                                                    Log_errori &= Gias.NonConformitaRilevateControlloBloccante & ": <br>" & vbCrLf
                                                                End If
                                                                Log_errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Impresa & " " & Rag_Soc & " - " & Gias.CentroAziendale & " " & Sa_Nome & " - " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControlloConformita & vbCrLf
                                                                'Throw New Exception(Resources.AgronicaAgenda_2010.BLOperazioneNonÈConformeBBr & Messaggio)
                                                            End If
                                                        Else
                                                            If Not CorrettoConformita Then
                                                                If HiddenVarie.Value = "" Then
                                                                    strAlert = MessaggioControlloConformita
                                                                    If Alert_Errori = "" Then
                                                                        Alert_Errori &= Gias.NonConformitaRilevateControlloNonBloccante & ": <br>" & vbCrLf
                                                                    End If
                                                                    Alert_Errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Impresa & " " & Rag_Soc & " - " & Gias.CentroAziendale & " " & Sa_Nome & " - " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControlloConformita & vbCrLf
                                                                Else
                                                                    'forzata scrittura
                                                                    CorrettoConformita = True
                                                                    Alert_Errori = ""
                                                                End If

                                                            End If

                                                        End If

                                                End Select


                                            Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                                 LAVCOD_CONCIMAZIONE_FOGLIARE,
                                                 LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                                 LAVCOD_DISTRIBUZIONE_CONCIME,
                                                 LAVCOD_FERTIRRIGAZIONE,
                                                 LAVCOD_SARCHIATURA_CONCIMAZIONE

                                                If Session("UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS") = True Then

                                                    Flag_ControllaBlocco = 1

                                                    CorrettoConformita = ControllaMassimali(Agenda, MessaggioControlloConformita)

                                                    If Not CorrettoConformita Then
                                                        If strErrore = "" Then
                                                            strErrore = MessaggioControlloConformita
                                                        Else
                                                            strErrore &= "<br>" & MessaggioControlloConformita
                                                        End If

                                                        If Log_errori = "" Then
                                                            Log_errori &= Gias.NonConformitaRilevateControlloBloccante & " <br>:" & vbCrLf
                                                        End If
                                                        Log_errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Impresa & " " & Rag_Soc & " - " & Gias.CentroAziendale & " " & Sa_Nome & " - " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControlloConformita & vbCrLf
                                                    End If
                                                End If
                                        End Select

                                        ' CONTROLLO SCARICO
                                        Dim CorrettoMagazzino As Boolean = True
                                        Dim isBloccante As Boolean = True

                                        If (Not isFromNG AndAlso Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True) OrElse
                                            (isFromNG AndAlso getControlliMagazzino(Agenda.Piva, Agenda.Lav_Cod, isBloccante, objParametri_Server, objParametri_Utenti)) Then

                                            Flag_ControllaBlocco = 1

                                            CorrettoMagazzino = ControllaGiacenzeMagazzino(Agenda, MessaggioControlloMagazzino)

                                            If Not CorrettoMagazzino Then
                                                If isBloccante Then

                                                    If strErrore = "" Then
                                                        strErrore = MessaggioControlloMagazzino
                                                    Else
                                                        strErrore &= "<br>" & MessaggioControlloMagazzino
                                                    End If

                                                    If Log_errori = "" Then
                                                        Log_errori &= Gias.NonConformitaRilevateControlloBloccante & ": <br>" & vbCrLf
                                                    End If
                                                    Log_errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Impresa & " " & Rag_Soc & " - " & Gias.CentroAziendale & Sa_Nome & " - " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControlloMagazzino & vbCrLf

                                                Else

                                                    If strAlert = "" Then
                                                        strAlert = MessaggioControlloMagazzino
                                                    Else
                                                        strAlert &= "<br>" & MessaggioControlloMagazzino
                                                    End If

                                                    If Alert_Errori = "" Then
                                                        Alert_Errori &= Gias.NonConformitaRilevateControlloBloccante & ": <br>" & vbCrLf
                                                    End If
                                                    Alert_Errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Impresa & " " & Rag_Soc & " - " & Gias.CentroAziendale & Sa_Nome & " - " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControlloMagazzino & vbCrLf

                                                End If
                                            End If
                                        End If


                                        '-------------------------------------------------------
                                        '-------------------------------------------------------
                                        '------------ SALVATAGGIO ------------------------------
                                        '-------------------------------------------------------
                                        '-------------------------------------------------------


                                        If CorrettoMagazzino AndAlso CorrettoConformita Then

                                            Dim objAgendaScrivi As New Agenda_Operazione_Helper
                                            Dim Id_Agenda_Nuovo As Integer = 0

                                            Id_Agenda_Nuovo = objAgendaScrivi.Scrivi(Agenda, objParametri_Server, IdAgendaCopiata_xLOG:=" (Id_Agenda copiata: " & AgendaDaCopiare.Id_Agenda & ")")

                                            Flag_insert = True

                                            Messaggio &= data & " - " & Agenda.Des_Lib & " - " & Rag_Soc & " - " & Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso &
                                                     Chr(13)
                                        Else

                                            Flag_insert = False

                                            DrOperazioniNonSalvate = DtOperazioniNonSalvate.NewRow()

                                            DrOperazioniNonSalvate.Item("piva") = Agenda.Piva
                                            DrOperazioniNonSalvate.Item("sa_cod") = Agenda.Sa_Cod
                                            DrOperazioniNonSalvate.Item("rag_soc") = Rag_Soc
                                            DrOperazioniNonSalvate.Item("sa_nome") = Sa_Nome
                                            DrOperazioniNonSalvate.Item("id_agenda") = id_agenda
                                            DrOperazioniNonSalvate.Item("data") = data
                                            DrOperazioniNonSalvate.Item("strErrore") = strErrore
                                            DrOperazioniNonSalvate.Item("strAlert") = strAlert
                                            DrOperazioniNonSalvate.Item("des_lib") = Agenda.Des_Lib

                                            DrOperazioniNonSalvate.Item("id_agenda_originale") = AgendaDaCopiare.Id_Agenda
                                            DrOperazioniNonSalvate.Item("raccoglitore_cod_originale") = AgendaDaCopiare.Raccoglitore_Cod
                                            DrOperazioniNonSalvate.Item("piva_originale") = AgendaDaCopiare.Piva
                                            DrOperazioniNonSalvate.Item("lav_cod") = AgendaDaCopiare.Lav_Cod

                                            DtOperazioniNonSalvate.Rows.Add(DrOperazioniNonSalvate)

                                        End If

                                        'Se l'agenda corrente ha un raccoglitore_cod, lo aggiungo al DT
                                        'Mi servirà al salvataggio dopo la ocnferma degli alert per scrivere le operazioni con lo stesso raccoglitore_cod insieme
                                        If Agenda.Raccoglitore_Cod <> 0 Then
                                            DrOperazioniRaccoglitore = DtOperazioniRaccoglitore.NewRow()

                                            DrOperazioniRaccoglitore.Item("piva") = Agenda.Piva
                                            DrOperazioniRaccoglitore.Item("sa_cod") = Agenda.Sa_Cod
                                            DrOperazioniRaccoglitore.Item("rag_soc") = Rag_Soc
                                            DrOperazioniRaccoglitore.Item("sa_nome") = Sa_Nome
                                            DrOperazioniRaccoglitore.Item("id_agenda") = id_agenda
                                            DrOperazioniRaccoglitore.Item("data") = data
                                            DrOperazioniRaccoglitore.Item("strErrore") = strErrore
                                            DrOperazioniRaccoglitore.Item("strAlert") = strAlert
                                            DrOperazioniRaccoglitore.Item("des_lib") = Agenda.Des_Lib

                                            DrOperazioniRaccoglitore.Item("id_agenda_originale") = AgendaDaCopiare.Id_Agenda
                                            DrOperazioniRaccoglitore.Item("raccoglitore_cod_originale") = AgendaDaCopiare.Raccoglitore_Cod
                                            DrOperazioniRaccoglitore.Item("piva_originale") = AgendaDaCopiare.Piva
                                            DrOperazioniRaccoglitore.Item("lav_cod") = AgendaDaCopiare.Lav_Cod

                                            DtOperazioniRaccoglitore.Rows.Add(DrOperazioniRaccoglitore)
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                Else

                    'COPIA E INCOLLA AZIENDALE
                    Dim strErr As String = ""
                    Agenda = Genera_Agenda_SingolaAzienda(AgendaDaCopiare, data, nota, strErr)

                    'If Messaggio <> "" Then
                    '    Throw New Exception(Messaggio)
                    'End If

                    '---------------------------------------------------------------
                    '---------------------------------------------------------------
                    'creo l'operazione
                    '---------------------------------------------------------------
                    '---------------------------------------------------------------
                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                    End If

                    'Aggiungo raccoglitore_cod se esiste 
                    If raccoglitore_cod <> 0 Then
                        Agenda.Raccoglitore_Cod = raccoglitore_cod
                    End If

                    Select Case Agenda.Lav_Cod

                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                             LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                             LAVCOD_CONCIA_SEME,
                             LAVCOD_GEODISINFESTAZIONE,
                             LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                             LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE


                            '(31/05/2016) introdotta impostazione x dare alert
                            Select Case Session("ControllaBlocco")

                                Case "1", "2" '1=blocco 2=alert

                                    Dim Corretto As Boolean
                                    Dim MessaggioControllo As String = ""
                                    Corretto = VerificaDPI(Agenda, MessaggioControllo)

                                    If Session("ControllaBlocco") = "1" Then
                                        If Not Corretto Then
                                            If Log_errori = "" Then
                                                Log_errori &= Gias.NonConformitaRilevateControlloBloccante & ": <br>" & vbCrLf
                                            End If
                                            Log_errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControllo & vbCrLf
                                            'Throw New Exception(Resources.AgronicaAgenda_2010.BLOperazioneNonÈConformeBBr & Log_Errori)
                                        End If
                                    Else
                                        If Not Corretto Then
                                            If HiddenVarie.Value = "" Then
                                                If Alert_Errori = "" Then
                                                    Alert_Errori &= Gias.NonConformitaRilevateControlloNonBloccante & ": <br>" & vbCrLf
                                                End If
                                                Alert_Errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControllo & vbCrLf
                                            Else
                                                Alert_Errori = ""
                                            End If
                                        End If
                                    End If
                            End Select


                        Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                             LAVCOD_CONCIMAZIONE_FOGLIARE,
                             LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                             LAVCOD_DISTRIBUZIONE_CONCIME,
                             LAVCOD_FERTIRRIGAZIONE,
                             LAVCOD_SARCHIATURA_CONCIMAZIONE

                            If Session("UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS") = True Then

                                Dim Corretto As Boolean
                                Dim MessaggioControllo As String = ""
                                Corretto = ControllaMassimali(Agenda, MessaggioControllo)

                                If Not Corretto Then
                                    If Log_errori = "" Then
                                        Log_errori &= Gias.NonConformitaRilevate & ": <br>"
                                    End If
                                    Log_errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Impresa & " " & Rag_Soc & " - " & Gias.CentroAziendale & " " & Sa_Nome & " - " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControllo & vbCrLf
                                End If
                            End If


                    End Select

                    ' CONTROLLO SCARICO

                    If Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True Then

                        Dim Corretto As Boolean
                        Dim MessaggioControllo As String = ""
                        Corretto = ControllaGiacenzeMagazzino(Agenda, MessaggioControllo)

                        If Not Corretto Then
                            If Log_errori = "" Then
                                Log_errori &= Gias.NonConformitaRilevate & ": <br>"
                            End If
                            Log_errori &= "<b>" & Agenda.Des_Lib & "</b>: " & Gias.Impresa & " " & Rag_Soc & " - " & Gias.CentroAziendale & " " & Sa_Nome & " - " & Gias.Data & " " & data.ToShortDateString & "<br>-->" & MessaggioControllo & vbCrLf
                        End If
                    End If

                    Dim objAgendaScrivi As New Agenda_Operazione_Helper
                    Dim Id_Agenda_Nuovo As Integer = 0

                    Id_Agenda_Nuovo = objAgendaScrivi.Scrivi(Agenda, objParametri_Server, IdAgendaCopiata_xLOG:=" (Id_Agenda copiata: " & AgendaDaCopiare.Id_Agenda & ")")

                    Flag_insert = True

                    Messaggio &= Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso & Chr(13)

                End If
            Next


        Catch ex As Exception

            Flag_insert = False
            Log_errori &= String.Format(Resources.AgronicaAgenda_2010.OperazioneX0ScritturaFallitaPerIlSeguenteE, "1") & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)

            'uso questa funzione per ottenere il Messaggio..:


            'ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            If Log_errori <> "" Then
                'Messaggio di errore
                Messaggio &= Resources.AgronicaAgenda_2010.SiEVerificatoUnErrore & Chr(13) & Log_errori
                Exit Sub
            Else
                'alert
                If Alert_Errori <> "" Then
                    Exit Sub
                End If
            End If
        End Try
    End Sub

    Private Function getListaImpiantiSelezionati() As List(Of DuplicaOperazione_Impianto)

        Dim sss As String = hdImpiantiSelezionati.Value

        Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModello.DuplicaOperazione_Impianto))(sss)

    End Function


#Region "ORDINAMENTO GRIDVIEW"

    'Protected Sub OnSort_GridView_Impianti(ByVal sender As Object, ByVal e As GridViewSortEventArgs)

    '    e_SortDirection = ViewState("_Direction_")
    '    m_strSortExp = ViewState("_SortExp_")

    '    If Not IsNothing(m_strSortExp) AndAlso m_strSortExp = e.SortExpression Then
    '        If Not IsNothing(e_SortDirection) AndAlso e_SortDirection = e.SortDirection Then
    '            If e_SortDirection = SortDirection.Ascending Then
    '                e_SortDirection = SortDirection.Descending
    '            Else
    '                e_SortDirection = SortDirection.Ascending
    '            End If
    '        Else
    '            e_SortDirection = e.SortDirection
    '        End If
    '    Else
    '        e_SortDirection = e.SortDirection
    '    End If

    '    m_strSortExp = e.SortExpression
    '    ViewState("_Direction_") = e_SortDirection
    '    ViewState("_SortExp_") = m_strSortExp

    '    Dim dv = New DataView(Session("DataViewImpianti"))
    '    '        dv.Sort = m_strSortExp + IIf(m_SortDirection = SortDirection.Ascending, "ASC", "DESC")
    '    dv.Sort = String.Format("{0} {1}", e.SortExpression, ConvertSort(e_SortDirection)) '

    '    Session("DataViewImpianti") = dv.ToTable

    '    GridView_Impianti.DataSource = dv
    '    GridView_Impianti.DataBind()

    'End Sub

    'Private Function ConvertSort(ByVal sortDirection As SortDirection) As String
    '    Dim m_SortDirection As String = ""
    '    Select Case sortDirection
    '        Case SortDirection.Ascending
    '            m_SortDirection = "ASC"
    '        Case SortDirection.Descending
    '            m_SortDirection = "DESC"
    '    End Select
    '    Return m_SortDirection
    'End Function
#End Region


    Private Sub DataGridInterventi_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles DataGridInterventi.RowCommand

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim IndiceRigaGriglia As Integer = 0
        Dim ID As Integer = 0

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Dt = ViewState("vs_dtInterventi")

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            ID = Dt.Rows(IndiceRigaGriglia).Item("ID")

            Select Case e.CommandName

                Case "EliminaIntervento"

                    '---------------------------------------------
                    ' ELIMINO LA RIGA DAL DT
                    'Trovo la riga da cancellare    (chiave = ID)
                    Dr = Dt.Rows.Find(ID)

                    Dr.Delete()

                    ViewState("vs_dtInterventi") = Dt
                    hd_dtInterventi.Value = JsonConvert.SerializeObject(Dt)

                    DataGridInterventi.DataSource = Dt
                    DataGridInterventi.DataBind()

                    'Grilli 19/06/2017: ripulisco gli impianti e li ricarico da capo (devo reintrodurre gli impianti cancellati a causa delle date)
                    'objParametriAgenda.Impianti.Clear()
                    RecuperaImpiantiOperazioneSelezionata()

            End Select

        End If

    End Sub

    Private Function VerificaDPI(ByVal Agenda As Operazione_Agenda, ByRef Errore As String) As Boolean

        'una volta creato l'oggetto agenda posso infocare il suo metodo che mi genera l'xml
        Dim Dpi_Cod As Integer = 0
        Dim Dpi_PubblicoPrivato As Integer = 0

        Dim Helper As New Agenda_Operazione_Helper
        Dim strXML As String = Helper.GeneraXML_CAU_TRATTAMENTO(Agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        strXML = Replace(strXML, "TipoOperazioneDB=""1""", "TipoOperazioneDB=""" & Agenda.Tipo_Operazione & """")

        Dim Conforme As Boolean

        Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
        Dim rval As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento)
        rval = objDpiVerifica.Verifica_Conformita_Intervento_New(objParametri_Server, objParametri_Utenti,
                                                                            Agenda.Piva,
                                                                            strXML, 0,
                                                                            Agenda.Id_Agenda,
                                                                            True,
                                                                                enum_Disciplinare_Operazione.QuelloDellOperazione, 0,
                                                                                    "", 0, 0, 0)

        If rval.RispostaOK Then
            Errore = rval.RispostaStringa.strNonConformita
            Conforme = rval.RispostaStringa.Conforme 'VerificaStringaErrori(rval.RispostaStringa.Risultato, Errore)
        End If

        Return Conforme


    End Function

    Private Function ControllaMassimali(ByVal Agenda As Operazione_Agenda, ByRef Errore As String) As String

        Dim Helper As New Agenda_Operazione_Helper
        Dim Dpi_Cod As Integer = 0
        Dim Dpi_PubblicoPrivato As Integer = 0
        Dim strXML As String = Helper.GeneraXML_CAU_LAVORAZIONI(Agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        strXML = Replace(strXML, "TipoOperazioneDB=""1""", "TipoOperazioneDB=""" & Agenda.Tipo_Operazione & """")

        Dim Conforme As Boolean

        Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
        Dim rval As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento)
        rval = objDpiVerifica.Verifica_Conformita_Intervento_New(objParametri_Server, objParametri_Utenti,
                                                                 Agenda.Piva,
                                                                 strXML, 0,
                                                                 Agenda.Id_Agenda,
                                                                 True,
                                                                 enum_Disciplinare_Operazione.QuelloDellOperazione, 0,
                                                                 "", 0, 0, 0)

        If rval.RispostaOK Then
            Errore = rval.RispostaStringa.strNonConformita
            Conforme = rval.RispostaStringa.Conforme 'VerificaStringaErrori(rval.RispostaStringa.Risultato, Errore)
        End If

        Return Conforme


    End Function

    Private Function ControllaGiacenzeMagazzino(ByVal Agenda As Operazione_Agenda, ByRef Errore As String) As String

        Dim ProdottiSuff As Boolean = True
        Dim MessaggioControllo As String = ""

        For s = 0 To Agenda.Movimenti.Count - 1

            Select Case Agenda.Movimenti(s).Cau_Mov

                Case CAU_SCARICO

                    Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

                    'verifico ogni prodotto se suff alla data dell'intervento
                    If Agenda.Movimenti(s).Movimenti_Dettagli.Count > 0 Then

                        For p = 0 To Agenda.Movimenti(s).Movimenti_Dettagli.Count - 1

                            If Agenda.Movimenti(s).Movimenti_Dettagli(p).Movimenti_Destinazioni.Count > 0 Then

                                'guardo se la quantità è conforme per la data di intervento
                                Dim qta_in_data As Decimal = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Agenda.Movimenti(s).Movimenti_Dettagli(p).Movimenti_Destinazioni(0).Piva,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Movimenti_Destinazioni(0).Sa_Cod,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Movimenti_Destinazioni(0).Id_Destinazione,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Elem_Cod,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Pro_Cod,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Mat_Cod,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Cod_Progetto,
                                                                                                               0,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Lotto,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Cal_Cod,
                                                                                                               Agenda.Movimenti(s).Movimenti_Dettagli(p).Udm_Cod,
                                                                                                               AGRODATAINIZIO,
                                                                                                               Agenda.Data,
                                                                                                               Agenda.Piva, Agenda.Sa_Cod,
                                                                                                               0,
                                                                                                               objParametri_Server)

                                Dim Qta_scaricata As Decimal = Agenda.Movimenti(s).Movimenti_Dettagli(p).Qta

                                If (qta_in_data <> 0 AndAlso Not (qta_in_data < QTA_GiancenzeVisualizzate AndAlso qta_in_data > -QTA_GiancenzeVisualizzate)) Then

                                    qta_in_data = Math.Round(qta_in_data, 3)

                                    Qta_scaricata = Math.Round(Qta_scaricata, 3)

                                    If Qta_scaricata > qta_in_data Then

                                        Dim Descrizione As String = ""
                                        Select Case Agenda.Movimenti(s).Movimenti_Dettagli(p).Mat_Cod
                                            Case 0
                                                Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                                                Descrizione = objCatMag.ProDes_from_ProCod(Agenda.Movimenti(s).Movimenti_Dettagli(p).Elem_Cod, Agenda.Movimenti(s).Movimenti_Dettagli(p).Pro_Cod, objParametri_Server)
                                            Case Else
                                                Dim objMat As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                                Descrizione = objMat.MatDes_from_MatCod(Agenda.Piva, Agenda.Movimenti(s).Movimenti_Dettagli(p).Elem_Cod, Agenda.Movimenti(s).Movimenti_Dettagli(p).Mat_Cod, "", "", "", objParametri_Server)
                                        End Select

                                        Select Case Agenda.Movimenti(s).Movimenti_Dettagli(p).Udm_Cod
                                            'In caso di unità arrodondo senza decimali
                                            Case enum_UnitaMisura.Numero, enum_UnitaMisura.UNITA, enum_UnitaMisura.UNITA__HA, enum_UnitaMisura.Numero_Diffusori, enum_UnitaMisura.Numero_Diffusori_HA
                                                qta_in_data = Math.Round(qta_in_data, 0)
                                                Qta_scaricata = Math.Round(Qta_scaricata, 0)
                                        End Select

                                        Dim objUdM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                        Dim Udm_Sim As String = ""
                                        Dim Udm_Des As String = ""
                                        objUdM.Converti_Kg_L_from_UdmCod(Agenda.Movimenti(s).Movimenti_Dettagli(p).Udm_Cod, Udm_Sim, Udm_Des)

                                        ProdottiSuff = False
                                        MessaggioControllo &= "La Quantita di " & Descrizione & " presente in magazzino (" & qta_in_data & " " & Udm_Sim & ") non è sufficiente per lo scarico (" & Qta_scaricata & " " & Udm_Sim & ")<br>" & vbCrLf

                                    End If

                                End If


                            End If


                        Next

                    End If

            End Select

        Next

        Errore = MessaggioControllo

        Return ProdottiSuff


    End Function

    Private Function VerificaStringaErrori(ByVal RisultatoVerifica As String, ByRef strErrore As String) As Boolean

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim Conforme As Boolean = True

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiRisultati As System.Xml.XmlElement
        Dim XML_DatiGenerali As System.Xml.XmlElement
        Dim XML_DatiNonConformi As System.Xml.XmlElement
        Dim XML_DatoNonConforme As System.Xml.XmlElement
        Dim XMLs_DatoNonConforme As System.Xml.XmlNodeList

        Dim x As Integer = 0

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(RisultatoVerifica)

        '----- Elemento <DatiRisultati>
        XML_DatiRisultati = XmlDoc.SelectSingleNode("DatiRisultati")

        '----- Elemento <DatiGenerali>
        XML_DatiGenerali = XML_DatiRisultati.SelectSingleNode("DatiGenerali")

        '----- Elemento <DatiNonConformi>
        XML_DatiNonConformi = XML_DatiGenerali.SelectSingleNode("DatiNonConformi")

        XMLs_DatoNonConforme = XML_DatiNonConformi.GetElementsByTagName("DatoNonConforme")

        If Not XML_DatiNonConformi.HasChildNodes Then
            Conforme = True
        Else
            Conforme = False
        End If

        '------------------------------------------
        '----- Data Grid Dettagli
        '------------------------------------------
        For x = 0 To XMLs_DatoNonConforme.Count - 1
            XML_DatoNonConforme = XMLs_DatoNonConforme.Item(x)
            strErrore += CStr(XML_DatoNonConforme.GetAttribute("err_des")) & "<br>"
        Next

        Return Conforme

    End Function

    Private Function Leggi_FlagNuovoControlloRiduzioneDiserbo() As Boolean

        Dim flagNuovoControlloRiduzioneDiserbo As Boolean = False
        Dim csr As New Configurazione_Siti_R
        Dim dt As DataTable = csr.Leggi(0, "Flag_Nuovo_Controllo_Riduzione_Diserbo", "", "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            flagNuovoControlloRiduzioneDiserbo = CBool(dt.Rows(0)("Valore"))
        End If
        Return flagNuovoControlloRiduzioneDiserbo

    End Function

    Private Function OttieniPercAbbMinorePerPrincipioAttivo(ByVal strPrincipiAttiviPercAbb As String) As Decimal

        Dim arrayPercAbb = strPrincipiAttiviPercAbb.Split("|").ToList
        Dim percAbbMin As Decimal = 100

        For Each pa In arrayPercAbb
            Dim valori = pa.Split("§")
            If valori.Count > 1 Then
                If IsNumeric(valori(1)) Then
                    If CDec(valori(1) < percAbbMin) Then
                        percAbbMin = valori(1)
                    End If
                End If
            End If
        Next

        Return percAbbMin

    End Function

    Private Shared Sub filtraOperazioniMultiCentro(ByRef ListaOperazioniIn As List(Of AgronicaCoreModello.DuplicaOperazione_parametriInput))

        Dim dictionary_RaccoglitorexLavCod As New Dictionary(Of (Integer, Integer), String)
        Dim ListaOperazioniToRemove As New List(Of AgronicaCoreModello.DuplicaOperazione_parametriInput)

        For Each operazione In ListaOperazioniIn
            If operazione.Raccoglitore_Cod <> 0 Then
                If dictionary_RaccoglitorexLavCod.ContainsKey((operazione.Raccoglitore_Cod, operazione.Lav_Cod)) Then
                    ListaOperazioniToRemove.Add(operazione)
                Else
                    dictionary_RaccoglitorexLavCod.Add((operazione.Raccoglitore_Cod, operazione.Lav_Cod), "")
                End If
            End If
        Next

        For Each toDelete In ListaOperazioniToRemove
            ListaOperazioniIn.Remove(toDelete)
        Next

    End Sub

    Private Shared Function getControlliMagazzino(piva_agenda As String,
                                                  lav_Cod As Integer,
                                                  ByRef isBloccante As Boolean,
                                                  objParametri_Server As AgronicaCoreParametri,
                                                  objParametri_Utenti As AgronicaCoreParametri
                                                  ) As Boolean

        Dim InfoOperazione As InfoOperazione = Utility_Agenda.GetInfoOperazione(lav_Cod, AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna)

        'DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
        'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA
        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim SoloPresenti As Integer =
                CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva_agenda, Nothing,
                                                                                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE,
                                                                                            InfoOperazione.Elem_Cod, enum_Gestione_Giacenze.TuttiProdotti,
                                                                                            objParametri_Utenti, objParametri_Server))

        isBloccante = If(SoloPresenti = 1, True, False)

        Return True

    End Function

    Private Function CheckValiditaDistinta(ByVal validita_inizio_distinta As String, ByVal validita_fine_distinta As String, ByVal data As Date) As Boolean

        Dim validita As Boolean = False

        If ((Not IsDate(validita_inizio_distinta)) OrElse
            (IsDate(validita_inizio_distinta) AndAlso CDate(validita_inizio_distinta) <= data)) AndAlso
            ((Not IsDate(validita_fine_distinta)) OrElse
            (IsDate(validita_fine_distinta) AndAlso CDate(validita_fine_distinta) >= data)) Then

            validita = True

        End If

        Return validita

    End Function

    ''' <summary>
    ''' Controllo che la data scelta sia valida per gli esercizi selezionati
    ''' </summary>
    ''' <param name="OperazioneIn"></param>
    ''' <param name="ListaOperazioniIn_Count"></param>
    ''' <param name="gruppoRaccoglitore_selected"></param>
    ''' <returns></returns>
    Private Function CheckValiditaDistinte(ByVal OperazioneIn As AgronicaCoreModello.DuplicaOperazione_parametriInput, ByVal ListaOperazioniIn_Count As Integer,
                                           ByVal gruppoRaccoglitore_selected As Boolean) As Boolean

        Dim DictDistinteNonValide As New Dictionary(Of String, String)

        Dim check As Boolean = True

        Dim ListaImpiantiSelezionati As List(Of AgronicaCoreModello.DuplicaOperazione_Impianto) = getListaImpiantiSelezionati()

        If Not IsNothing(ListaImpiantiSelezionati) AndAlso ListaImpiantiSelezionati.Count > 0 Then

            Dim dataToCheck As Date = OperazioneIn.Data

            If Not IsNothing(Me.DataGridInterventi) AndAlso Not IsNothing(Me.DataGridInterventi.Rows) Then

                For i = 0 To Me.DataGridInterventi.Rows.Count - 1

                    SetData(Me.DataGridInterventi.Rows(i), ListaOperazioniIn_Count,
                            gruppoRaccoglitore_selected, dataToCheck)

                    For Each ImpiantoSelezionato In ListaImpiantiSelezionati

                        Dim keyS As String = String.Format("-", ImpiantoSelezionato.piva, ImpiantoSelezionato.sa_cod, ImpiantoSelezionato.appezza, ImpiantoSelezionato.id_reg)

                        If Not IsNothing(DictDistinteNonValide) AndAlso Not DictDistinteNonValide.ContainsKey(keyS) AndAlso
                            Not CheckValiditaDistinta(ImpiantoSelezionato.validita_inizio_distinta, ImpiantoSelezionato.validita_fine_distinta, dataToCheck) Then

                            Dim info As String = ImpiantoSelezionato.app_nome & " (" & ImpiantoSelezionato.validita_inizio_distinta & " - " & ImpiantoSelezionato.validita_fine_distinta & ")"

                            DictDistinteNonValide.Add(keyS, info)
                        End If

                    Next

                    If Not IsNothing(DictDistinteNonValide) AndAlso DictDistinteNonValide.Count > 0 Then

                        Dim dateStr = dataToCheck.ToShortDateString()

                        Dim DictValues As List(Of String) = TryCast(DictDistinteNonValide.Values.ToList(), List(Of String))

                        Messaggi.AgroMsgBox(String.Format(Gias.ImpiantiSenzaEserciziInData, dateStr, String.Join("<br> - ", DictValues)), Page, , UpdatePanelScript)

                        check = False

                        Exit For
                    End If

                Next
            End If

        End If

        Return check
    End Function

    Private Function GetMsgBase(ByVal des_lib As String, ByVal rag_soc As String, ByVal sa_nome As String, ByVal data As String) As String
        Return "<b>" & des_lib & ":</b>" &
        " " & Gias.Impresa & " " & rag_soc &
        " - " & Gias.CentroAziendale & " " & sa_nome & " - " & Gias.Data & " " &
        data
    End Function

    ''' <summary>
    ''' VAnni: 18/3/2017: nota e data vengono ora letti dall'operazione di origine se ci sono due o più operazioni, altrimenti riassegno il valore.
    ''' </summary>
    ''' <param name="Row"></param>
    ''' <param name="ListaOperazioniIn_Count"></param>
    ''' <param name="gruppoRaccoglitore_selected"></param>
    ''' <param name="data"></param>
    Private Sub SetData(ByVal Row As GridViewRow, ByVal ListaOperazioniIn_Count As Integer,
                        ByVal gruppoRaccoglitore_selected As Boolean, ByRef data As Date)

        If ListaOperazioniIn_Count = 1 OrElse gruppoRaccoglitore_selected Then

            data = CDate(Row.Cells(2).Text)

        End If
    End Sub

    ''' <summary>
    '''  VAnni: 18/3/2017: nota e data vengono ora letti dall'operazione di origine se ci sono due o più operazioni, altrimenti riassegno il valore.
    ''' </summary>
    ''' <param name="Row"></param>
    ''' <param name="ListaOperazioniIn_Count"></param>
    ''' <param name="gruppoRaccoglitore_selected"></param>
    ''' <param name="nota"></param>
    Private Sub SetNota(ByVal Row As GridViewRow, ByVal ListaOperazioniIn_Count As Integer,
                        ByVal gruppoRaccoglitore_selected As Boolean, ByRef nota As String)


        If ListaOperazioniIn_Count = 1 OrElse gruppoRaccoglitore_selected Then

            If Row.Cells(3).Text.Trim <> "&nbsp;" Then
                nota = Row.Cells(3).Text.Trim
            Else
                nota = ""
            End If

        End If
    End Sub
End Class