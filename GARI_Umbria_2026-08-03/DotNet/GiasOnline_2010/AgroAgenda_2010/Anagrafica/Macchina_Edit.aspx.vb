Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports AgroAgenda_2010.Resources

Partial Class Macchina_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Dim Qs_Piva As String
    'Dim Qs_PivaPadre As String
    Dim Qs_PivaNuova As String
    Dim Qs_Visibilita As Integer = 0

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean
    Dim PremutoFiltro As Boolean

    'Public jsCosti As String
    Public objparametri_server_string As String
    Public objparametri_utenti_string As String
    Public xPiva As String
    Public xSa_Cod As String
    Public xMac_Cod As String

    Public Operazione As Integer
    Dim Operazione_Contatti As Integer
    Public ASG_ProgressivoGIAS As String
    Public IdServizio As String

    Public back As String

    Public permessi As PermessiUtente

    Public jsRevisioni As String
    Public jsCosti As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaCosto(ByVal row As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("Dt_Costi")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (i = row) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next

        HttpContext.Current.Session("Dt_Costi") = Dt
        Dim str_Risposta = DT_to_Json_Costi(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Costo(ByVal unita_cod As String, ByVal unita_des As String, ByVal prezzo As String, ByVal data_in As String, ByVal data_out As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        'Dim flag As Boolean = True



        If (IsNothing(HttpContext.Current.Session("Dt_Costi"))) Then
            'Dt.Columns.Add(New DataColumn("ID", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
            Dt.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("Dt_Costi")
        End If


        ''Controllo se esiste già la voce che si vuole inserire
        'For i = 0 To Dt.Rows.Count - 1

        '    If (Dt.Rows(i).Item("ID") = codice_id) Then
        '        flag = False
        '    End If
        'Next


        'If (flag) Then

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori

        'Dr.Item("Contatore") = Contatore

        Dr.Item("Udm_Cod") = unita_cod
        Dr.Item("Udm_Des") = unita_des
        Dr.Item("Validita_Inizio") = data_in
        Dr.Item("Validita_Fine") = data_out
        Dr.Item("Prezzo_Unitario") = prezzo

        'Dr.Item("Validita_Inizio") = xValiditaInizio

        'Dr.Item("Validita_Fine") = xValiditaFine

        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)
        HttpContext.Current.Session("Dt_Costi") = Dt
        Dim str_Risposta = DT_to_Json_Costi(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        'Else
        '    r.RispostaOK = False
        '    r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        'End If

        Return r

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Revisioni(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Agenda", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaRevisione(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaRevisione(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn) With {
            .cssColonna = "colmodmovimenti"
        }
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        Dim cn As New ColonneNome("Lav_Cod", "Codice", "number") With {
            ._hidden = True
        }
        l.Add(cn)

        cn = New ColonneNome("Data", AgronicaAgenda_2010.Data, "date")
        l.Add(cn)

        cn = New ColonneNome("Des_Lib", AgronicaAgenda_2010.Descrizione, "string")
        l.Add(cn)

        cn = New ColonneNome("Costo", "Costo", "number") With {
            ._hidden = True
        }
        l.Add(cn)

        cn = New ColonneNome("Doc_Numero", "Doc_Numero", "string") With {
            ._hidden = True
        }
        l.Add(cn)

        'cn = New ColonneNome("Validita_Inizio", "Dal", "string")
        'l.Add(cn)

        'cn = New ColonneNome("Validita_Fine", "Al", "string")
        'l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Costi(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Udm_Des", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-info info_elem", "InfoCosti(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCosti(this);"))

        End If

        Dim tool As New ToolStandard(listaBtn) With {
            .cssColonna = "colmodcosti"
        }
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        Dim cn As New ColonneNome("Udm_Cod", "Unità di Misura Codice", "number") With {
            ._hidden = True
        }

        l.Add(cn)

        cn = New ColonneNome("Udm_Des", AgronicaAgenda_2010.RisorsaUDM, "string")
        l.Add(cn)

        cn = New ColonneNome("Prezzo_Unitario", AgronicaAgenda_2010.Prezzo, "string")
        l.Add(cn)

        'cn = New ColonneNome(dt.Columns("Validita_Inizio"), "V_I")
        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.DataInizio, "date")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.DataFine, "date")
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function



    Private Sub Macchina_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        Dim xModalBS As String = Request.QueryString("modalBS")
        If Not String.IsNullOrEmpty(xModalBS) AndAlso xModalBS = "1" Then
            Me.Master.flag_pag_BootstrapModal = True
            Me.Master.flag_MostraHeader = False
            Me.Master.flag_MostraFooter = False
        Else

            ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
            'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
            AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
            Master.flag_MostraBtnIndietro = True
        End If

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        objparametri_utenti_string = Utility.convertOBJparametritoString(objParametri_Utenti)

        Master.flag_pag_Anagrafica = True

        Dim Qs_Operazione As String

        permessi = New PermessiUtente()

        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'Se Qs_Visibilita = 0 è stato aperto dal Menu Anagrafe generale e quindi utilizzo la versione standard della grafica
        If Qs_Visibilita = 0 Then
            Master.Master_versione = VERSIONE_MASTER_DEFAULT
            Master.Header_versione = VERSIONE_HEADER_DEFAULT
        End If

        objParametriAgenda = New ParametriAgenda
        'Dim xChiave As String
        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod


        If Request.QueryString("m") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString("m").ToString) Then
            xMac_Cod = Stringa_Decodifica(Request.QueryString("m").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

        Else
            xMac_Cod = objParametriAgenda.Mac_Cod

        End If

        hd_Piva.Value = xPiva
        hd_Mat_Cod.Value = xMac_Cod

        IdServizio = 5

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim dt As DataTable = objUtenti.Leggi("", "", objParametri_Utenti)
        ASG_ProgressivoGIAS = dt.Rows(0).Item("ProgressivoGIAS")

        back = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)

        'Qs_Key = Stringa_Decodifica(Request.QueryString("k").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        If Not IsNothing(Request.QueryString("o")) Then
            Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        Else
            Qs_Operazione = objParametriAgenda.Tipo_Operazione
        End If

        If objParametriAgenda.PaginaSitoOrigine <> enum_PagineAgenda_2010.Pagina_Anagrafica_Menu Then
            Master.flag_MostraHeader = False
            hd_PopUpUMA.Value = True

            tipo_salva.Value = objParametriAgenda.PaginaSitoOrigine
        End If

        'Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_PaginaRitorno = Stringa_Decodifica(Request.QueryString("r").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Recupero Chiave ed Operazione dalla querystring
        'xChiave = Qs_Key
        'Operazione = Qs_Operazione
        'xPiva = Qs_Piva

        Operazione = CInt(Qs_Operazione)
        HttpContext.Current.Session("operazione") = Operazione

        Select Case Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("CreazioneNuovaMacchina"), String)
                objParametriAgenda.Sa_Cod = 0
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaMacchina"), String)
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaMacchina"), String)
        End Select

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then

            '==========================================
            '===== Pagina caricata per la prima volta
            '==========================================

            'Aggiungo funzione per il salvataggio della griglia kendo Costi
            ImgBtn_SalvaTutto.Attributes.Add("onclick", "Salva_GrigliaCostoUnitario()")

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina
            Dim UtenteAbilitato_Lettura As Boolean = False
            Dim UtenteAbilitato_Modifica As Boolean = False

            UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine).Lettura
            UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine).Scrittura

            'Valorizzo gli hiddenfield che mi serviranno per la griglia kendo dei costi 
            hd_UtenteAbilitatoLettura.Value = UtenteAbilitato_Lettura
            hd_UtenteAbilitatoScrittura.Value = UtenteAbilitato_Modifica
            hd_TipoOperazione.Value = Operazione

            '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio ad AlberoImprese.
            If Not UtenteAbilitato_Modifica Then
                Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
                Exit Sub
            End If



        Else

            Dim Dt4 As New DataTable
            jsRevisioni = DT_to_Json_Revisioni(Dt4)
            HttpContext.Current.Session("Dt_Revisioni") = Dt4

            Dim Dt5 As New DataTable
            Dt5.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
            Dt5.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
            Dt5.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt5.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
            Dt5.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
            jsCosti = DT_to_Json_Costi(Dt5)
            HttpContext.Current.Session("Dt_Costi") = Dt5

            'Reimposto le griglie dopo l'errore 

            'carico la griglia dei costi

            ''carico la griglia dei movimenti
            ''  Carica_Movimenti(Qs_Mac_Cod)

            Carica_Manutenzioni(xMac_Cod)

            Exit Sub
        End If



        Txt_DataInizioUtilizzo.Text = "01/" & Right("0" & CStr(Date.Today.Month), 2) & "/" & CStr(Date.Today.Year)
        'Txt_DataFine_Movimenti.Text = Date.Today

        EseguitaOperazione = False
        PremutoAnnulla = False
        PremutoFiltro = False

        ''visualizzo i dati tecnici
        'Imposta_Pannelli("D")

        'variabile per il codice delle righe del datagrid dei costi
        ViewState("nPrezzi") = -1

        'varibile per sapere se ho modificato i prezzi
        ViewState("ModificatoPrezzi") = False

        '##############################################################
        '#####  Inizializzo i controlli  ##############################
        '##############################################################

        '----- Pulisco le varie Textbox e ComboBox

        'PANNELLO SPECIFICHE TECNICHE
        Me.Cmb_Tipo.SelectedIndex = 0
        Me.Cmb_Dettaglio1.SelectedIndex = 0
        Me.Cmb_Dettaglio1.Enabled = False
        Me.Cmb_Dettaglio2.SelectedIndex = 0
        Me.Cmb_Dettaglio2.Enabled = False
        Me.Txt_Targa.Text = ""
        Me.txt_telaio.Text = ""
        Me.Txt_Modello.Text = ""
        Me.Txt_Potenza.Text = ""
        Me.Txt_Descrizione.Text = ""
        Me.Txt_Codice.Text = ""


        'PANNELLO SPECIFICHE UTILIZZO
        Me.Txt_DataInizioUtilizzo.Text = Date.Today
        'Me.Txt_DataInizioUtilizzo_1.Text = Date.Today
        Me.Txt_DataUltimaManutenzione.Text = ""
        Me.Txt_DataImmatricolazione.Text = ""
        Me.Txt_DataUltimaRevisione.Text = ""
        Me.Txt_CostoAcquisto.Text = "0"
        Me.Txt_Ammortamento.Text = "0"

        Me.Txt_NumImmatricolazione.Text = ""
        Me.Txt_NumImmatricolazioneRimorchio.Text = ""
        Me.Txt_NumAutorizzazione.Text = ""
        Me.Txt_DataRilascioAutorizzazione.Text = ""
        'Me.Txt_Peso.Text = "0"


        'Disabilito momentaneamente le combo del lotto e del calibro
        Me.Txt_StatoUtilizzo.Text = ""
        Me.Txt_DataDismissione.Text = ""



        ''Imposto le informazioni dell'intestazione della pagina web
        ''Impresa
        'Me.Txt_Impresa.Text = RagSoc_from_Piva(Server, Session, Page, xPiva)


        'Anno = Today.Year
        'Mese = Right("00" & CStr(Today.Month), 2)

        'Me.Txt_DataInizio_Movimenti.Text = "01/" & Mese & "/" & Anno


        AgronicaCoreUtility.CaricaListControl.TitoloPossesso(CType(ddlTitoloPossesso, ListControl),
                                                                    False, "", "",
                                                                    "", "", objParametri_Server)
        ''Carico la combo delle Ditte
        ''CaricaCombo_Ditte(Server, Session, Page, Me.Cmb_DittaProvenienza)
        ''Carico la combo del Tipo di Attrezzatura 
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(Cmb_Tipo, 1, "", "", objParametri_Server)

        'carico la combo con le marche e via dicendo...
        AgronicaCoreUtility.CaricaListControl.MarcheMacchine(ddlMarca, objParametri_Server)
        'CaricaCombo_Macchine_Tipo_Targa(ddlTipoTarga)

        ddlTipoTarga.Items.Clear()
        ddlTipoTarga.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("NonDefinito"), String), enum_Macchine_TipoTarga.NonDefinito))
        ddlTipoTarga.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("SenzaTarga"), String), enum_Macchine_TipoTarga.SenzaTarga))
        ddlTipoTarga.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Stradale"), String), enum_Macchine_TipoTarga.Stradale))
        ddlTipoTarga.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Rimorchio"), String), enum_Macchine_TipoTarga.Rimorchio))
        ddlTipoTarga.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Triangolare"), String), enum_Macchine_TipoTarga.Triangolare))

        'i18n
        'CaricaCombo_Macchine_PotenzaUdm(ddlPotenzaUdm)
        ddlPotenzaUdm.Items.Clear()
        ddlPotenzaUdm.Items.Add(New ListItem("", "0"))
        ddlPotenzaUdm.Items.Add(New ListItem("CV", "5001028"))
        ddlPotenzaUdm.Items.Add(New ListItem("KW", "5001027"))

        'CaricaCombo_Macchine_Alimentazione(ddlAlimentazione)
        ddlAlimentazione.Items.Clear()
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("NonDefinita"), String), enum_Macchine_TipoAlimentazione.NonDefinita))
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Benzina"), String), enum_Macchine_TipoAlimentazione.Benzina))
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Gasolio"), String), enum_Macchine_TipoAlimentazione.Gasolio))
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Metano"), String), enum_Macchine_TipoAlimentazione.Metano))
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Gpl"), String), enum_Macchine_TipoAlimentazione.Gpl))
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Elettricita"), String), enum_Macchine_TipoAlimentazione.Elettricita))
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("OlioCombustibile"), String), enum_Macchine_TipoAlimentazione.Olio_Combustibile))
        ddlAlimentazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Petrolio"), String), enum_Macchine_TipoAlimentazione.Petrolio))


        Dim Dt2 As New DataTable
        jsRevisioni = DT_to_Json_Revisioni(Dt2)
        HttpContext.Current.Session("Dt_Revisioni") = Dt2

        Dim Dt3 As New DataTable
        Dt3.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt3.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt3.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt3.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Dt3.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        jsCosti = DT_to_Json_Costi(Dt3)
        HttpContext.Current.Session("Dt_Costi") = Dt3

        Cmb_Finalita.Items.Clear()
        Cmb_Finalita.Items.Add(New ListItem(AgronicaAgenda_2010.AgricolaZootecnica, 0))
        Cmb_Finalita.Items.Add(New ListItem(AgronicaAgenda_2010.Industriale, 1))
        Cmb_Finalita.Items.Add(New ListItem(AgronicaAgenda_2010.Commerciale, 2))


        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            Me.Opt_Attivo.Checked = True
            Me.Opt_Dismesso.Checked = False

            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, AgronicaAgenda_2010.MacchinaAziendale, "0", xPiva, False, 2, "", "", objParametri_Server)

            Dim obj As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            If obj.Controlla_Permessi_Utente(
                         objParametri_Utenti.UtenteUsername,
                         enum_Id_Servizio.GiasOnline,
                         enum_Security_Attivita.Macchine_Assegnazione_Pubblica,
                         enum_Security_Operazione.Modifica,
                         Date.Now, "", objParametri_Utenti) Then

                Cmb_CentriAziendali.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("MacchinaAttrezzaturaMovimentabileDaTutteLeImprese"), String), "-1"))
            End If


            Me.Cmb_CentriAziendali.SelectedIndex =
                Cmb_CentriAziendali.Items.IndexOf(
                    Cmb_CentriAziendali.Items.FindByValue("0"))


            ViewState("Mac_Cod") = 0

            'Me.Tab_Movimenti.Visible = False

            ViewState("Mac_Cod_Origine") = 0
            ViewState("Piva_superUser_Origine") = ""

            Carica_Contatto(xPiva, "")



        ElseIf Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then


            'vettore per il class_cod
            Dim Vettore() As String

            ''----- Creo gli oggetti COM+ per il recupero dei dati
            'Creo gli oggetti COM+
            'Dim objCOM As Object    'New Agro_Contab_AD.Parco_Macchine_R
            'Dim Rs As ADODB.Recordset
            '   *   CreateCANCELLATOObject("Agro_Contab_AD.Parco_Macchine_R")

            ''Leggo le informazioni sul macchinario selezionato			
            'Rs = objCOM.LeggiParcoMacchinexSuperUser( _
            '                    Session("ASG_SuperUser_CodFiscale").ToString, _
            '                    CStr(xPiva), CInt(Qs_Mac_Cod), , , _
            '                    CDate(Session("ASG_FinestraTemporale_Inizio").ToString), _
            '                    CDate(Session("ASG_FinestraTemporale_Fine").ToString), _
            '                    CStr(Session(ASG_.con..._server)))

            ''Elimino l'oggetto COM+
            'objCOM = Nothing

            '--- Genero la Tabella

            Dim core As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim dtPM As DataTable = core.LeggiParcoMacchinexSuperUser(xPiva, xMac_Cod, 0, "",
                False, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "",
                objParametri_Server)

            ViewState("Mac_Cod_Origine") = 0
            ViewState("Piva_superUser_Origine") = ""

            'macchine
            If dtPM.Rows.Count = 1 Then

                Carica_Contatto(xPiva, dtPM.Rows(0)("Cod_Contatto"))

                ViewState("Mac_Cod_Origine") = dtPM.Rows(0)("Mac_Cod_Origine")
                ViewState("Piva_superUser_Origine") = dtPM.Rows(0)("Piva_superUser_Origine")

                'imposto la selezione nella combo
                ddlMarca.SelectedValue = dtPM.Rows(0)("ditta_cod")

                'CaricaCombo_Macchine_Tipo_Targa(ddlTipoTarga)
                'ddlTipoTarga.SelectedValue = ImpostaSelezioneCombo(ddlTipoTarga, dtPM.Rows(0)("tipo_targa_cod"))
                ddlTipoTarga.SelectedValue = dtPM.Rows(0)("tipo_targa_cod")

                'CaricaCombo_Macchine_PotenzaUdm(ddlPotenzaUdm)
                'ddlPotenzaUdm.SelectedValue = ImpostaSelezioneCombo(ddlPotenzaUdm, dtPM.Rows(0)("potenza_udm_cod"))
                ddlPotenzaUdm.SelectedValue = dtPM.Rows(0)("potenza_udm_cod")

                'CaricaCombo_Macchine_Alimentazione(ddlAlimentazione)
                'ddlAlimentazione.SelectedValue = ImpostaSelezioneCombo(ddlAlimentazione, dtPM.Rows(0)("alimentazione_cod"))
                ddlAlimentazione.SelectedValue = dtPM.Rows(0)("alimentazione_cod")

                AgronicaCoreUtility.CaricaListControl.TitoloPossesso(CType(ddlTitoloPossesso, ListControl),
                                                                     False, "", "",
                                                                     "", "", objParametri_Server)

                'ddlTitoloPossesso.SelectedValue = ImpostaSelezioneCombo(ddlTitoloPossesso, dtPM.Rows(0)("TitoloPossesso"))
                ddlTitoloPossesso.SelectedValue = dtPM.Rows(0)("TitoloPossesso")


                'imposto la taratura...
                If IsDBNull(dtPM.Rows(0)("taratura_ugello")) Then
                    txtTaraUgello.Text = "0"
                Else
                    txtTaraUgello.Text = Format(dtPM.Rows(0)("taratura_ugello"), "0.00")
                End If
                Vettore = Split(dtPM.Rows(0)("class_code").ToString(), ".")


                'Carico la combo del tipo delle macchine
                'CaricaCombo_TipoMacchine(objParametri_Server, Me.Cmb_Tipo, 1, "", "")
                AgronicaCoreUtility.CaricaListControl.TipoMacchine(Cmb_Tipo, 1, "", "", objParametri_Server)

                'Imposto la posizione nella combo del tipo
                Cmb_Tipo.SelectedIndex =
                    Cmb_Tipo.Items.IndexOf(
                        Cmb_Tipo.Items.FindByValue(
                            Vettore(0)))
                If UBound(Vettore) > 0 Then

                    'Ripulisco la combo prima di caricarla
                    Me.Cmb_Dettaglio1.Items.Clear()

                    'Inserisco una riga vuota
                    Cmb_Dettaglio1.Items.Add(New ListItem("", ""))
                    'CaricaCombo_TipoMacchine(objParametri_Server, Me.Cmb_Dettaglio1, 2, Me.Cmb_Tipo.SelectedItem.Value, "")
                    AgronicaCoreUtility.CaricaListControl.TipoMacchine(Cmb_Dettaglio1, 2, Me.Cmb_Tipo.SelectedItem.Value, "", objParametri_Server)
                    Me.Cmb_Dettaglio1.SelectedIndex =
                        Cmb_Dettaglio1.Items.IndexOf(
                            Cmb_Dettaglio1.Items.FindByValue(
                                Vettore(1)))
                    If UBound(Vettore) > 1 Then

                        'Ripulisco la combo prima di caricarla
                        Me.Cmb_Dettaglio2.Items.Clear()

                        'Inserisco una riga vuota
                        Cmb_Dettaglio2.Items.Add(New ListItem("", ""))
                        'CaricaCombo_TipoMacchine(objParametri_Server, Me.Cmb_Dettaglio2, 3, Me.Cmb_Tipo.SelectedItem.Value, Me.Cmb_Dettaglio1.SelectedItem.Value)
                        AgronicaCoreUtility.CaricaListControl.TipoMacchine(Cmb_Dettaglio2, 3, Me.Cmb_Tipo.SelectedItem.Value, Me.Cmb_Dettaglio1.SelectedItem.Value, objParametri_Server)
                        Cmb_Dettaglio2.SelectedIndex =
                            Cmb_Dettaglio2.Items.IndexOf(
                                 Cmb_Dettaglio2.Items.FindByValue(
                                        Vettore(2)))
                    End If
                End If

                'Imposto il tipo
                If dtPM.Rows(0)("Tipo") <> 0 Then

                    Me.Cmb_Finalita.SelectedIndex =
                        Cmb_Finalita.Items.IndexOf(
                            Cmb_Finalita.Items.FindByValue(dtPM.Rows(0)("Tipo").ToString()))

                    Me.Cmb_Tipo.Enabled = False

                End If

                Me.Txt_Codice.Text = dtPM.Rows(0)("Codice").ToString()
                Me.Txt_Targa.Text = dtPM.Rows(0)("Targa").ToString()
                Me.txt_telaio.Text = dtPM.Rows(0)("Telaio").ToString()
                Me.Txt_Modello.Text = dtPM.Rows(0)("Modello").ToString()
                Me.Txt_Potenza.Text = dtPM.Rows(0)("Potenza").ToString()

                Me.txtCUAAProp.Text = dtPM.Rows(0)("CUAA_Proprietario").ToString()
                Me.txtProprietario.Text = dtPM.Rows(0)("Denominazione_Proprietario").ToString()

                'Imposto la ditta delle combo se è stata inserita una ditta
                'If Rs.Fields("Ditta_Cod").Value <> 0 Then
                '    CaricaCombo_Ditte(Server, Session, Page, Me.Cmb_DittaProvenienza)
                '    Me.Cmb_DittaProvenienza.SelectedIndex = _
                '        Cmb_DittaProvenienza.Items.IndexOf( _
                '            Cmb_DittaProvenienza.Items.FindByValue( _
                '                Rs.Fields("Ditta_Cod").Value))
                'End If

                txtDtCarico.Text = IIf(dtPM.Rows(0)("Data_Carico") = Date.Parse("01/01/1900"), "",
                                                                    CDate(dtPM.Rows(0)("Data_Carico")).ToShortDateString())

                txtDtScarico.Text = IIf(dtPM.Rows(0)("Data_Scarico") = Date.Parse("01/01/1900"), "",
                                                                    CDate(dtPM.Rows(0)("Data_Scarico")).ToShortDateString())


                Me.Txt_Descrizione.Text = dtPM.Rows(0)("Mac_Des").ToString()
                Me.Txt_Codice.Text = dtPM.Rows(0)("Codice").ToString()

                Me.Txt_DataInizioUtilizzo.Text = IIf(dtPM.Rows(0)("Validita_inizio") = Date.Parse("01/01/1900"), "",
                                                    CDate(dtPM.Rows(0)("Validita_inizio")).ToShortDateString())
                'Me.Txt_DataInizioUtilizzo_1.Text = IIf(dtPM.Rows(0)("Validita_inizio") = Date.Parse("01/01/1900"), "", _
                '                                    CDate(dtPM.Rows(0)("Validita_inizio")).ToShortDateString)
                Me.Txt_DataImmatricolazione.Text = IIf(dtPM.Rows(0)("Data_Immatricolazione") = Date.Parse("01/01/1900"), "",
                                                        CDate(dtPM.Rows(0)("Data_Immatricolazione")).ToShortDateString)
                Me.Txt_DataUltimaManutenzione.Text = IIf(dtPM.Rows(0)("Ultima_manutenzione") = Date.Parse("01/01/1900"), "",
                                                        CDate(dtPM.Rows(0)("Ultima_manutenzione")).ToShortDateString)
                Me.Txt_DataUltimaRevisione.Text = IIf(dtPM.Rows(0)("Ultima_Revisione") = Date.Parse("01/01/1900"), "",
                                                    CDate(dtPM.Rows(0)("Ultima_Revisione")).ToShortDateString)
                Me.Txt_CostoAcquisto.Text = Format(CDbl(dtPM.Rows(0)("Costo_Acquisto")), "##,###,##0.00")

                Me.Txt_Ammortamento.Text = Format(dtPM.Rows(0)("Ammortamento"), "##,###,##0.00")
                If IsNumeric(dtPM.Rows(0)("Ammortizzato")) Then
                    Me.Txt_Ammortizzato.Text = Format(dtPM.Rows(0)("Ammortizzato"), "##0.00")
                Else
                    Me.Txt_Ammortizzato.Text = Format(0, "##0.00")
                End If


                'Me.Txt_Peso.Text = dtPM.Rows(0)("Peso").ToString

                Me.Txt_NumImmatricolazione.Text = dtPM.Rows(0)("N_Immatricolazione").ToString
                Me.Txt_NumImmatricolazioneRimorchio.Text = dtPM.Rows(0)("N_Immatricolazione_Rimorchio").ToString

                Me.Txt_NumAutorizzazione.Text = dtPM.Rows(0)("N_Autorizzazione_Trasporto").ToString
                Me.Txt_DataRilascioAutorizzazione.Text = IIf(dtPM.Rows(0)("Data_Rilascio_Autorizzazione") = Date.Parse("01/01/1900"), "",
                                                            dtPM.Rows(0)("Data_Rilascio_Autorizzazione").ToString)

                '-----
                If (Not IsDBNull(dtPM.Rows(0)("Visibile_ctrl_gestione"))) Then
                    Me.chk_visibile_ctrl_gestione.Checked = CBool(dtPM.Rows(0)("Visibile_ctrl_gestione"))
                Else
                    Me.chk_visibile_ctrl_gestione.Checked = False
                End If

                If dtPM.Rows(0)("Validita_Fine") = CDate("31/12/2100") Then
                    Me.Opt_Attivo.Checked = True
                    Me.Txt_StatoUtilizzo.Enabled = True
                    Me.Txt_StatoUtilizzo.Text = dtPM.Rows(0)("stato_utilizzo").ToString
                    Me.Opt_Dismesso.Checked = False
                    Me.Txt_DataDismissione.Text = ""
                    Me.Txt_DataDismissione.Enabled = False
                    ' Me.BtnDataDismissione.Disabled = True
                Else
                    Me.Opt_Attivo.Checked = False
                    Me.Txt_StatoUtilizzo.Enabled = False
                    Me.Txt_StatoUtilizzo.Text = ""
                    Me.Txt_StatoUtilizzo.Enabled = False
                    Me.Opt_Dismesso.Checked = True
                    Me.Txt_DataDismissione.Text = CDate(dtPM.Rows(0)("Validita_Fine")).ToShortDateString
                    Me.Txt_DataDismissione.Enabled = True
                    ' Me.BtnDataDismissione.Disabled = False
                End If

                Dim sacodstring As String = dtPM.Rows(0)("sa_cod").ToString

                'If sacodstring = "0" Then
                '    Me.Chk_Movimentabile.Checked = False
                'ElseIf sacodstring = "-1" Then
                '    Me.Chk_Movimentabile.Checked = True
                'Else
                '    Me.Chk_Movimentabile.Checked = False
                'End If

                AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, AgronicaAgenda_2010.MacchinaAziendale, "0", xPiva, False, 2, "", "", objParametri_Server)

                If Operazione = enum_TipoOperazioneDB.Modifica Then

                    Dim obj As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    If obj.Controlla_Permessi_Utente(
                             objParametri_Utenti.UtenteUsername,
                             enum_Id_Servizio.GiasOnline,
                             enum_Security_Attivita.Macchine_Assegnazione_Pubblica,
                             enum_Security_Operazione.Modifica,
                             Date.Now, "", objParametri_Utenti) Then

                        Cmb_CentriAziendali.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("MacchinaAttrezzaturaMovimentabileDaTutteLeImprese"), String), "-1"))
                    End If
                End If

                Me.Cmb_CentriAziendali.SelectedIndex =
                        Cmb_CentriAziendali.Items.IndexOf(
                            Cmb_CentriAziendali.Items.FindByValue(sacodstring))

                If Not IsDBNull(dtPM.Rows(0)("note")) Then
                    Txt_Note.Text = dtPM.Rows(0)("note").ToString
                End If

                '-----
                If Not IsDBNull(dtPM.Rows(0)("Validita_Taratura_Inizio")) AndAlso dtPM.Rows(0)("Validita_Taratura_Inizio") <> AGRODATAINIZIO Then
                    txtDataTaraturaInizio.Text = CDate(dtPM.Rows(0)("Validita_Taratura_Inizio")).ToShortDateString
                Else
                    txtDataTaraturaInizio.Text = ""
                End If
                If Not IsDBNull(dtPM.Rows(0)("Validita_Taratura_Fine")) AndAlso dtPM.Rows(0)("Validita_Taratura_Fine") <> AGRODATAFINE Then
                    txtDataTaraturaFine.Text = CDate(dtPM.Rows(0)("Validita_Taratura_Fine")).ToShortDateString
                Else
                    txtDataTaraturaFine.Text = ""
                End If


            End If


            ' @Paolo: DA FARE CON WATABLE

            '''''

            'carico la griglia dei costi

            ''carico la griglia dei movimenti
            ''  Carica_Movimenti(Qs_Mac_Cod)

            Carica_Manutenzioni(xMac_Cod)

            If Operazione = enum_TipoOperazioneDB.Lettura Then
                ImpostaPaginaSoloLettura()
                'Me.Cmb_Tipo.Enabled = False
                'Me.Cmb_Dettaglio1.Enabled = False
                'Me.Cmb_Dettaglio2.Enabled = False
                'Me.Txt_Targa.Enabled = False
                'Me.txt_telaio.Enabled = False
                'Me.Txt_Modello.Enabled = False
                'Me.Txt_Potenza.Enabled = False
                'Me.Txt_Descrizione.Enabled = False
                'Me.Txt_Codice.Enabled = False

                'Me.Txt_DataInizioUtilizzo.Enabled = False
                'Me.Txt_DataUltimaManutenzione.Enabled = False
                'Me.Txt_DataImmatricolazione.Enabled = False
                'Me.Txt_DataUltimaRevisione.Enabled = False
                'Me.Txt_CostoAcquisto.Enabled = False
                'Me.Txt_Ammortamento.Enabled = False

                'Me.Txt_NumImmatricolazione.Enabled = False
                'Me.Txt_NumImmatricolazioneRimorchio.Enabled = False
                'Me.Txt_NumAutorizzazione.Enabled = False
                'Me.Txt_DataRilascioAutorizzazione.Enabled = False

                'Cmb_CentriAziendali.Enabled = False
                'ddlMarca.Enabled = False
                'Cmb_Finalita.Enabled = False
                'txtDtCarico.Enabled = False
                'txtDtScarico.Enabled = False
                'ddlTipoTarga.Enabled = False
                'txtProprietario.Enabled = False
                'ddlAlimentazione.Enabled = False
                'txtTaraUgello.Enabled = False
                'ddlTitoloPossesso.Enabled = False
                'txtCUAAProp.Enabled = False
                'ddlPotenzaUdm.Enabled = False
                'Txt_StatoUtilizzo.Enabled = False
                'Txt_Note.Enabled = False
                'Txt_Manutenzioni.Enabled = False
                'txtDataTaraturaInizio.Enabled = False
                'txtDataTaraturaFine.Enabled = False

            End If


        End If



    End Sub

    Private Sub ImpostaPaginaSoloLettura()
        Me.Cmb_Tipo.Enabled = False
        Me.Cmb_Dettaglio1.Enabled = False
        Me.Cmb_Dettaglio2.Enabled = False
        Me.Txt_Targa.Enabled = False
        Me.txt_telaio.Enabled = False
        Me.Txt_Modello.Enabled = False
        Me.Txt_Potenza.Enabled = False
        Me.Txt_Descrizione.Enabled = False
        Me.Txt_Codice.Enabled = False

        Me.Txt_DataInizioUtilizzo.Enabled = False
        Me.Txt_DataUltimaManutenzione.Enabled = False
        Me.Txt_DataImmatricolazione.Enabled = False
        Me.Txt_DataUltimaRevisione.Enabled = False
        Me.Txt_CostoAcquisto.Enabled = False
        Me.Txt_Ammortamento.Enabled = False

        Me.Txt_NumImmatricolazione.Enabled = False
        Me.Txt_NumImmatricolazioneRimorchio.Enabled = False
        Me.Txt_NumAutorizzazione.Enabled = False
        Me.Txt_DataRilascioAutorizzazione.Enabled = False

        Cmb_CentriAziendali.Enabled = False
        ddlMarca.Enabled = False
        Cmb_Finalita.Enabled = False
        txtDtCarico.Enabled = False
        txtDtScarico.Enabled = False
        ddlTipoTarga.Enabled = False
        txtProprietario.Enabled = False
        ddlAlimentazione.Enabled = False
        txtTaraUgello.Enabled = False
        ddlTitoloPossesso.Enabled = False
        txtCUAAProp.Enabled = False
        ddlPotenzaUdm.Enabled = False
        Txt_StatoUtilizzo.Enabled = False
        Txt_Note.Enabled = False
        Txt_Manutenzioni.Enabled = False
        txtDataTaraturaInizio.Enabled = False
        txtDataTaraturaFine.Enabled = False
    End Sub
    Private Sub Carica_Contatto(ByVal Piva As String, ByVal Cod_Contatto As String)

        hd_Contatto_Assegnato.Value = False
        If Not String.IsNullOrEmpty(Cod_Contatto) Then

            Dim dal = New Contatti_R
            Dim dt = dal.LeggiContattoSpecifico(Piva, Cod_Contatto, -99, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server, 0)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim rag_soc As String = If(dt.Rows(0).Item("Rag_Soc") Is DBNull.Value, "", dt.Rows(0).Item("Rag_Soc"))
                Dim cognome As String = If(dt.Rows(0).Item("cognome") Is DBNull.Value, "", dt.Rows(0).Item("cognome"))
                Dim nome As String = If(dt.Rows(0).Item("nome") Is DBNull.Value, "", dt.Rows(0).Item("nome"))

                Dim descrizione = If(Not String.IsNullOrEmpty(rag_soc), rag_soc, String.Format("{0} {1}", cognome, nome))
                ddlContatto.Items.Add(New ListItem(descrizione, Cod_Contatto))
                hd_Contatto_Assegnato.Value = True
            Else
                ddlContatto.Items.Add(New ListItem("", ""))
            End If

        Else
            ddlContatto.Items.Add(New ListItem("", ""))
        End If
        ddlContatto.SelectedIndex = 0
        ddlContatto.Enabled = False

    End Sub

    '#############################################################################################################
    'in data 12/03/2012: commentata la parte dei costi, non ha senso!
    'e impiega molto a caricare la pagina x niente
    Private Sub Carica_Manutenzioni(ByVal Mac_Cod As Integer)

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim ObjMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R 'Agro_Contab_AD.Movimenti_Dettagli_R
        '   *   CreateCANCELLATOObject("Agro_Contab_AD.Movimenti_Dettagli_R")

        'Dim ObjPrezzo_Unitario As New AgronicaCoreContabDAL.Movimenti_Dettagli_R 'Agro_Contab_AD.Movimenti_Dettagli_R
        '   *   CreateCANCELLATOObject("Agro_Contab_AD.Movimenti_Dettagli_R")

        'Dim CostoUnitario As String
        Dim CostoManutenzione As Double = 0

        'Dim bCheckListino As Boolean 'Booleano per la consultazione del prezzo nel listino generale (tabella Prodotti_Costi)

        'Dim i As Integer
        'Dim Manutenzione As Double = 0

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Des_Lib", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Doc_Numero", GetType(String)))

        'Vettore di DataColumn
        Dim DtKeys(0) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Id_Agenda")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        'Ricavo il recordset delle giacenze Aziendali
        'RsPrezzo_Unitario = ObjPrezzo_Unitario.LeggiGiacenze(CStr(viewstate("Piva")), , , , , , , , , , , , , , , CStr(Session(ASG_.con..._server)))

        'Dim dtRsPrezzo_Unitario As DataTable = ObjMovDet.LeggiGiacenze(CStr(viewstate("Piva")), _
        '                                                                0, _
        '                                                                0, _
        '                                                                0, _
        '                                                                0, _
        '                                                                0, 0, 0, 0, 0, _
        '                                                                "", "", "", objParametri_Server)


        '===========================================================================
        'Lettura delle Manutenzioni/Revisioni Associate alla Macchina Attrezzatura
        '---------------------------------------------------------------------------
        Dim dtManut As DataTable = ObjMovDet.Leggi("", 0, 0, 0, 0,
                                                    MACCHINE, 0,
                                                    Mac_Cod,
                                                    CAU_MANUTENZIONE_PARCOMACCHINE,
                                                    0, 0, 0, 0, 0, 0,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    "", "Data_Movimento DESC", objParametri_Server)

        'RsManuntezioni = ObjManutenzioni.Leggi(1, _
        '                         2, _
        '                         3, _
        '                         4, _
        '                         5, _
        '                         CInt(1), _
        '                         , _
        '                         CInt(Mac_Cod), _
        '                         CStr(CAU_MANUTENZIONE_PARCOMACCHINE), _
        '                         , _
        '                         , _
        '                         , _
        '                         , _
        '                         , _
        '                         , _
        '                         , _
        '                         , _
        '                         , _
        '                         , _
        '                         CStr(Session(ASG_.con..._server)))

        If Not IsNothing(dtManut) AndAlso dtManut.Rows.Count > 0 Then

            For Each drManut As DataRow In dtManut.Rows

                If Not ElementoPresente(CInt(drManut("Id_Agenda")), Dt) Then

                    Dr = Dt.NewRow

                    Dr.Item("Id_Agenda") = drManut("Id_Agenda")
                    Dr.Item("Lav_Cod") = drManut("Lav_Cod")
                    Dr.Item("Data") = CDate(drManut("Data_Movimento")).ToShortDateString
                    Dr.Item("Des_Lib") = drManut("Des_Lib")
                    Dr.Item("Doc_Numero") = drManut("Doc_Numero")


                    'Aggiornamento Costo Manutenzione
                    Dr.Item("Costo") = Format(CostoManutenzione, "##,###,##0.00")

                    '--------------------------------

                    Dt.Rows.Add(Dr)

                End If

            Next

        End If

        HttpContext.Current.Session("Dt_Revisioni") = Dt
        jsRevisioni = DT_to_Json_Revisioni(Dt)

        'Me.DataGrid_Manutenzioni.DataSource = Dt
        'Me.DataGrid_Manutenzioni.DataBind()

        'For i = 0 To Me.DataGrid_Manutenzioni.Items.Count - 1
        '    If IsNumeric(DataGrid_Manutenzioni.Items(i).Cells(4).Text) Then
        '        Manutenzione = Manutenzione + CDbl(DataGrid_Manutenzioni.Items(i).Cells(4).Text)
        '    End If
        'Next

        'Me.Txt_Manutenzioni.Text = Format(Manutenzione, "##,###,##0.00")


        'AggiornaAmmortamento()

        'Me.ImgBtn_NuovaRevisione.Visible = True
        'Me.ImgBtn_NuovaManutenzione.Visible = True

        ''verifico le date ultima manutenzione e ultimo utilizzo
        'Dim DataUltimaManutenzione As Date = #1/1/1900#
        'Dim DataUltimaRevisione As Date = #1/1/1900#

        'For i = 0 To Me.DataGrid_Manutenzioni.Items.Count - 1
        '    If DataUltimaManutenzione = #1/1/1900# Then
        '        If Me.DataGrid_Manutenzioni.Items(i).Cells(1).Text = LAVCOD_MANUTENZIONE_MACCHINE Then
        '            DataUltimaManutenzione = CDate(Me.DataGrid_Manutenzioni.Items(i).Cells(2).Text)
        '        End If
        '    End If
        '    If DataUltimaRevisione = #1/1/1900# Then
        '        If Me.DataGrid_Manutenzioni.Items(i).Cells(1).Text = LAVCOD_REVISIONE_MACCHINE Then
        '            DataUltimaRevisione = CDate(Me.DataGrid_Manutenzioni.Items(i).Cells(2).Text)
        '        End If
        '    End If
        'Next

        'If DataUltimaManutenzione <> #1/1/1900# Then
        '    Me.Txt_DataUltimaManutenzione.Text = DataUltimaManutenzione
        'End If
        'If DataUltimaRevisione <> #1/1/1900# Then
        '    Me.Txt_DataUltimaRevisione.Text = DataUltimaRevisione
        'End If

    End Sub


    '#############################################################################################################
    Private Function ElementoPresente(ByVal Id_Agenda As Integer,
                                      ByVal Dt As DataTable) As Boolean

        Dim i As Integer
        Dim Presente As Boolean = False

        For i = 0 To Dt.Rows.Count - 1

            If Dt.Rows(i).Item("Id_Agenda") = Id_Agenda Then

                Presente = True

                Exit For

            End If

        Next

        Return Presente

    End Function


    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)

        Response.Redirect(TargetUrl)

    End Sub



    '########################################################################################
    Private Sub ImgBtnSalvaTutto_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto_NEW()

    End Sub

    '########################################################################################

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaCostoUnitario(ByVal piva As String, ByVal mat_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        If HttpContext.Current.Session("righeInseriteGrigliaCosto") IsNot Nothing AndAlso
            HttpContext.Current.Session("righeModificateGrigliaCosto") IsNot Nothing AndAlso
            HttpContext.Current.Session("righeEliminateGrigliaCosto") IsNot Nothing AndAlso
            HttpContext.Current.Session("tutteleRigheGrigliaCosto") IsNot Nothing Then

            Dim tutteleRigheGrigliaCosto As List(Of MacchineCostiModel) = HttpContext.Current.Session("tutteleRigheGrigliaCosto")
            r.RispostaStringa = JsonConvert.SerializeObject(tutteleRigheGrigliaCosto, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True

        Else
            Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
                r.Sessione = False
                Return r
            End If


            Try
                Dim dtPC As DataTable
                Dim corePC As New AgronicaCoreContabDAL.Prodotti_Costi_R

                If mat_cod <> 0 Then
                    dtPC = corePC.Leggi_Macchine(piva,
                                mat_cod,
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                "", "",
                                objParametriServer)

                    r.RispostaStringa = JsonConvert.SerializeObject(dtPC, Newtonsoft.Json.Formatting.None)
                Else
                    r.RispostaStringa = JsonConvert.SerializeObject(New List(Of Object), Newtonsoft.Json.Formatting.None)
                End If

                r.RispostaOK = True

            Catch ex As Exception
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            End Try
        End If

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Sub SalvaGrigliaCosto(ByVal righeInserite As String,
                                        ByVal righeModificate As String,
                                        ByVal righeEliminate As String,
                                        ByVal tutteleRighe As String)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try
            If HttpContext.Current.Session("righeInseriteGrigliaCosto") IsNot Nothing AndAlso
               HttpContext.Current.Session("righeModificateGrigliaCosto") IsNot Nothing AndAlso
               HttpContext.Current.Session("righeEliminateGrigliaCosto") IsNot Nothing AndAlso
               HttpContext.Current.Session("tutteleRigheGrigliaCosto") IsNot Nothing Then



                Dim righeInseriteGrigliaCostoSessione As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeInseriteGrigliaCosto")
                Dim righeModificateGrigliaCostoSessione As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeModificateGrigliaCosto")
                Dim righeEliminateGrigliaCostoSessione As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeEliminateGrigliaCosto")
                Dim tutteleRigheGrigliaCostoSessione As List(Of MacchineCostiModel) = HttpContext.Current.Session("tutteleRigheGrigliaCosto")
                Dim NuovaRigheModificateSessione As List(Of MacchineCostiModel) = tutteleRigheGrigliaCostoSessione
                NuovaRigheModificateSessione.Clear()

                If righeInserite IsNot Nothing Then
                    Dim righeInseriteGrigliaCosto As List(Of MacchineCostiModel) = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeInserite, settingLoc)
                    righeInseriteGrigliaCostoSessione.Clear()
                    For Each ins As MacchineCostiModel In righeInseriteGrigliaCosto
                        righeInseriteGrigliaCostoSessione.Add(ins)
                    Next
                End If
                If righeModificate IsNot Nothing Then
                    Dim righeModificateGrigliaCosto As List(Of MacchineCostiModel) = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeModificate, settingLoc)
                    For Each modi As MacchineCostiModel In righeModificateGrigliaCosto
                        For Each modis As MacchineCostiModel In righeModificateGrigliaCostoSessione
                            If modis.ID = modi.ID Then
                                NuovaRigheModificateSessione.Add(modi)
                            Else
                                NuovaRigheModificateSessione.Add(modis)
                            End If
                        Next
                    Next
                End If
                If righeEliminate IsNot Nothing Then
                    Dim righeEliminateGrigliaCosto As List(Of MacchineCostiModel) = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeEliminate, settingLoc)
                    righeEliminateGrigliaCostoSessione.Clear()
                    For Each elim As MacchineCostiModel In righeEliminateGrigliaCosto
                        righeEliminateGrigliaCostoSessione.Add(elim)
                    Next
                End If
                If tutteleRighe IsNot Nothing Then
                    Dim tutteleRigheGrigliaCosto As List(Of MacchineCostiModel) = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(tutteleRighe, settingLoc)
                    tutteleRigheGrigliaCostoSessione.Clear()
                    For Each tutte As MacchineCostiModel In tutteleRigheGrigliaCosto
                        tutteleRigheGrigliaCostoSessione.Add(tutte)
                    Next
                End If

                HttpContext.Current.Session("righeInseriteGrigliaCosto") = righeInseriteGrigliaCostoSessione
                HttpContext.Current.Session("righeModificateGrigliaCosto") = NuovaRigheModificateSessione
                HttpContext.Current.Session("righeEliminateGrigliaCosto") = righeEliminateGrigliaCostoSessione
                HttpContext.Current.Session("tutteleRigheGrigliaCosto") = tutteleRigheGrigliaCostoSessione

            Else
                Dim righeInseriteGrigliaCosto As New List(Of MacchineCostiModel)
                Dim righeModificateGrigliaCosto As New List(Of MacchineCostiModel)
                Dim righeEliminateGrigliaCosto As New List(Of MacchineCostiModel)
                Dim tutteleRigheGrigliaCosto As New List(Of MacchineCostiModel)

                If righeInserite IsNot Nothing Then
                    righeInseriteGrigliaCosto = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeInserite, settingLoc)
                End If
                If righeModificate IsNot Nothing Then
                    righeModificateGrigliaCosto = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeModificate, settingLoc)
                End If
                If righeEliminate IsNot Nothing Then
                    righeEliminateGrigliaCosto = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeEliminate, settingLoc)
                End If
                If tutteleRighe IsNot Nothing Then
                    tutteleRigheGrigliaCosto = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(tutteleRighe, settingLoc)
                End If

                HttpContext.Current.Session("righeInseriteGrigliaCosto") = righeInseriteGrigliaCosto
                HttpContext.Current.Session("righeModificateGrigliaCosto") = righeModificateGrigliaCosto
                HttpContext.Current.Session("righeEliminateGrigliaCosto") = righeEliminateGrigliaCosto
                HttpContext.Current.Session("tutteleRigheGrigliaCosto") = tutteleRigheGrigliaCosto

            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Sub SalvaModificheGrigliaCosto(ByVal righeInserite As String,
                                                ByVal righeModificate As String,
                                                ByVal righeEliminate As String,
                                                ByVal tutteleRighe As String)
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try
            If HttpContext.Current.Session("righeInseriteGrigliaCosto") Is Nothing AndAlso
               HttpContext.Current.Session("righeModificateGrigliaCosto") Is Nothing AndAlso
               HttpContext.Current.Session("righeEliminateGrigliaCosto") Is Nothing AndAlso
               HttpContext.Current.Session("tutteleRigheGrigliaCosto") Is Nothing Then

                If righeInserite IsNot Nothing Then
                    HttpContext.Current.Session("righeInseriteGrigliaCosto") = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeInserite, settingLoc)
                End If
                If righeModificate IsNot Nothing Then
                    HttpContext.Current.Session("righeModificateGrigliaCosto") = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeModificate, settingLoc)
                End If
                If righeEliminate IsNot Nothing Then
                    HttpContext.Current.Session("righeEliminateGrigliaCosto") = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeEliminate, settingLoc)
                End If
                If tutteleRighe IsNot Nothing Then
                    HttpContext.Current.Session("tutteleRigheGrigliaCosto") = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(tutteleRighe, settingLoc)
                End If

            Else
                Dim righeInseriteGrigliaCosto As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeInseriteGrigliaCosto")
                Dim righeModificateGrigliaCostoSessione As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeModificateGrigliaCosto")
                Dim righeEliminateGrigliaCosto As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeEliminateGrigliaCosto")
                Dim righemodificateGriglia As List(Of MacchineCostiModel) = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeModificate, settingLoc)

                righeInseriteGrigliaCosto.AddRange(JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeInserite, settingLoc))
                righeEliminateGrigliaCosto.AddRange(JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(righeEliminate, settingLoc))

                Dim r As List(Of MacchineCostiModel) = righeModificateGrigliaCostoSessione

                If righemodificateGriglia.Count > 0 Then
                    r.Clear()
                    For Each modi As MacchineCostiModel In righemodificateGriglia
                        If righeModificateGrigliaCostoSessione.Count > 0 Then
                            For Each modis As MacchineCostiModel In righeModificateGrigliaCostoSessione
                                If modis.ID = modi.ID Then
                                    r.Add(modi)
                                Else
                                    r.Add(modis)
                                End If
                            Next
                        Else
                            r.Add(modi)
                        End If
                    Next
                End If


                HttpContext.Current.Session("righeInseriteGrigliaCosto") = righeInseriteGrigliaCosto
                HttpContext.Current.Session("righeModificateGrigliaCosto") = r
                HttpContext.Current.Session("righeEliminateGrigliaCosto") = righeEliminateGrigliaCosto
                HttpContext.Current.Session("tutteleRigheGrigliaCosto") = JsonConvert.DeserializeObject(Of List(Of MacchineCostiModel))(tutteleRighe, settingLoc)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub Salva_Tutto_NEW()

        'Prendo dalla variabile di sessione le righe della griglia kendo da salvare
        Dim righeInseriteGrigliaCosto As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeInseriteGrigliaCosto")
        Dim righeModificateGrigliaCosto As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeModificateGrigliaCosto")
        Dim righeEliminateGrigliaCosto As List(Of MacchineCostiModel) = HttpContext.Current.Session("righeEliminateGrigliaCosto")
        Dim tutteleRigheGrigliaCosto As List(Of MacchineCostiModel) = HttpContext.Current.Session("tutteleRigheGrigliaCosto")

        If Operazione = enum_TipoOperazioneDB.Cancellazione Then
            Exit Sub
        End If

        Dim Errore As Boolean = False

        Dim ErroreGrigliaCosto As Boolean = False

        'Controlli griglia kendo costi sulle date
        If righeInseriteGrigliaCosto IsNot Nothing Then
            'Controllo che non ci siano righe inserite con validita sovrapposte che hanno diversi prezzi ma stesse unita di misura
            Dim MessaggioErrore As String = String.Empty
            For Each ins As MacchineCostiModel In righeInseriteGrigliaCosto
                Dim unitadimisura_ins = ins.Mezzo
                Dim unitadimisurades_ins = ins.Udm_Des
                Dim validitaInizio = ins.Validita_Inizio
                Dim validitaFine = ins.Validita_Fine
                Dim prezzo = ins.Prezzo_Unitario

                If CDec(prezzo) > 0 Then
                    If validitaInizio < validitaFine Then
                        Dim esistenti = tutteleRigheGrigliaCosto.Where(Function(f) f.Mezzo = unitadimisura_ins AndAlso
                                                           ((f.Validita_Fine >= validitaInizio) AndAlso
                                                            (f.Validita_Inizio <= validitaFine)))

                        If esistenti.Count > 1 Then
                            MessaggioErrore = "Ci sono delle righe inserite con  Date sovrapposte."
                            Exit For
                        End If
                    Else
                        MessaggioErrore = "Ci sono delle righe inserite con  validità di inizio (" & validitaInizio & ") maggiore di quella finale (" & validitaFine & ")"
                        Exit For
                    End If
                Else
                    MessaggioErrore = "Inserire un prezzo corretto."
                End If
            Next

            If MessaggioErrore <> String.Empty Then
                Messaggi.AgroMsgBox("Impossibile inserire il costo della macchina: " & MessaggioErrore, Page, , UpdatePanel_script, , True)
                ErroreGrigliaCosto = True
                Errore = True
                Exit Sub
            End If
        End If


        If righeModificateGrigliaCosto IsNot Nothing Then
            'Controllo che non ci siano righe modificate con validita sovrapposte che hanno diversi prezzi ma stesse unita di misura
            Dim MessaggioErrore As String = String.Empty
            For Each modi As MacchineCostiModel In righeModificateGrigliaCosto
                Dim ID = modi.ID
                Dim unitadimisura_modi = modi.Mezzo
                Dim unitadimisurades_modi = modi.Udm_Des
                Dim validitaInizio = modi.Validita_Inizio
                Dim validitaFine = modi.Validita_Fine
                Dim prezzo = modi.Prezzo_Unitario

                If CDec(prezzo) > 0 Then
                    If validitaInizio < validitaFine Then
                        Dim esistenti = tutteleRigheGrigliaCosto.Where(Function(f) f.Mezzo = unitadimisura_modi AndAlso
                                                           ((f.Validita_Fine >= validitaInizio) AndAlso
                                                            (f.Validita_Inizio <= validitaFine)))

                        If esistenti.Count > 1 Then
                            MessaggioErrore = "Ci sono delle righe modificate con  Date sovrapposte."
                            Exit For
                        End If
                    Else
                        MessaggioErrore = "Ci sono delle righe modificate con  validità di inizio (" & validitaInizio & ") maggiore di quella finale (" & validitaFine & ")"
                        Exit For
                    End If
                Else
                    MessaggioErrore = "Inserire un prezzo corretto."
                End If
            Next

            If MessaggioErrore <> String.Empty Then
                Messaggi.AgroMsgBox("Impossibile modificare il costo della macchina: " & MessaggioErrore, Page, , UpdatePanel_script, , True)
                ErroreGrigliaCosto = True
                Errore = True
                Exit Sub
            End If
        End If


        'INIZIALIZZO LA TRANSAZIONE......
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

        Try

            '------------------------------------------------
            '----- Effettuo le modifiche al database
            '------------------------------------------------

            Dim esito As Boolean

            Dim MacCod As Integer = xMac_Cod
            Dim Piva As String = xPiva

            Dim DataImmatricolazione As Date = If(IsDate(Txt_DataImmatricolazione.Text), CDate(Txt_DataImmatricolazione.Text), #1/1/1900#)
            Dim DataRilascioAutorizzazione As Date = If(IsDate(Txt_DataRilascioAutorizzazione.Text), CDate(Txt_DataRilascioAutorizzazione.Text), #1/1/1900#)
            Dim DataUltimaManutenzione As Date = If(IsDate(Txt_DataUltimaManutenzione.Text), CDate(Txt_DataUltimaManutenzione.Text), #1/1/1900#)
            Dim DataUltimaRevisione As Date = If(IsDate(Txt_DataUltimaRevisione.Text), CDate(Txt_DataUltimaRevisione.Text), #1/1/1900#)
            Dim DataInizioUtilizzo As Date = If(IsDate(Txt_DataInizioUtilizzo.Text), CDate(Txt_DataInizioUtilizzo.Text), #1/1/1900#)
            Dim DataDismissione As Date = If(IsDate(Txt_DataDismissione.Text), CDate(Txt_DataDismissione.Text).ToShortDateString(), #12/31/2100#)
            Dim DataCarico As Date = If(IsDate(txtDtCarico.Text), Date.Parse(txtDtCarico.Text), Date.Parse("01/01/1900"))
            Dim DataScarico As Date = If(IsDate(txtDtScarico.Text), Date.Parse(txtDtScarico.Text), Date.Parse("01/01/1900"))

            Dim taratura_ugelli As Double = If(IsNumeric(txtTaraUgello.Text) AndAlso CDbl(txtTaraUgello.Text) > 0, CDbl(txtTaraUgello.Text.Replace(".", ",")), 0)

            Dim taratura_Inizio As Date = If(IsDate(txtDataTaraturaInizio.Text), Date.Parse(txtDataTaraturaInizio.Text), Date.Parse("01/01/1900"))
            Dim taratura_Fine As Date = If(IsDate(txtDataTaraturaFine.Text), Date.Parse(txtDataTaraturaFine.Text), Date.Parse("31/12/2100"))

            Dim visibile_ctrl_gestione = 0
            If Me.chk_visibile_ctrl_gestione.Checked Then
                visibile_ctrl_gestione = 1
            Else
                visibile_ctrl_gestione = 0
            End If

            'Calcolo il Class_Code
            Dim ClassCode As String = Cmb_Tipo.SelectedValue
            If Cmb_Dettaglio1.SelectedIndex > 0 Then
                ClassCode &= "." & Cmb_Dettaglio1.SelectedValue
            End If
            If Cmb_Dettaglio2.SelectedIndex > 0 Then
                ClassCode &= "." & Cmb_Dettaglio2.SelectedValue
            End If

            'Codice Contatto
            Dim cod_contatto As String = ddlContatto.SelectedValue


            '------------------------------------------------
            '----- SALVO PARCO MACCHINE
            '------------------------------------------------

            Dim pm_W As New AgronicaCoreContabDAL.Parco_Macchine_W()

            Select Case Operazione

                Case enum_TipoOperazioneDB.Scrittura

                    Dim seq As New Agro_Sequenze()
                    Dim MacCod_New As Integer = seq.NuovoId_Tabella("Parco_Macchine", 0, 2000000000, objParametri_Server)

                    'i18n
                    esito = pm_W.Scrivi(
                                    Piva, CInt(Cmb_CentriAziendali.SelectedValue), MacCod_New, "",
                                    ClassCode, CInt(Cmb_Finalita.SelectedValue), Txt_Descrizione.Text,
                                    Decimal.Parse(Txt_CostoAcquisto.Text), Txt_Targa.Text, txt_telaio.Text,
                                    CInt(ddlMarca.SelectedValue), Txt_Modello.Text, Txt_Potenza.Text,
                                    Decimal.Parse(Txt_Ammortamento.Text), DataImmatricolazione, DataUltimaManutenzione,
                                    DataUltimaRevisione, IIf(Opt_Attivo.Checked, Txt_StatoUtilizzo.Text, "Dismesso"),
                                    Txt_Note.Text, Txt_NumImmatricolazione.Text, Txt_NumImmatricolazioneRimorchio.Text,
                                    Txt_NumAutorizzazione.Text, DataRilascioAutorizzazione, 0, 0, 0,
                                    CInt(ddlAlimentazione.SelectedValue), CInt(ddlPotenzaUdm.SelectedValue),
                                    ViewState("Mac_Cod_Origine"), ViewState("Piva_superUser_Origine"),
                                    txtCUAAProp.Text, txtProprietario.Text, CInt(ddlTipoTarga.SelectedValue),
                                    0, "", 0, "", "", Date.Parse("01/01/1900"), DataCarico, DataScarico,
                                    CInt(ddlTitoloPossesso.SelectedValue), "M", DataInizioUtilizzo, DataDismissione,
                                    objParametri_Server, taratura_ugelli, taratura_Inizio, taratura_Fine, visibile_ctrl_gestione, Txt_Codice.Text)

                    If Not esito Then
                        ConnessioniTransazioni.RollBackTransazione_ChiudiConnessione(objParametri_Server)
                        Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ImpossibileCreareLaNuovaMacchina"), String), Page, , UpdatePanel_script, , True)
                        Exit Try
                    End If

                    MacCod = MacCod_New

                Case enum_TipoOperazioneDB.Modifica

                    esito = pm_W.Modifica(
                                    Piva, CInt(Cmb_CentriAziendali.SelectedValue), MacCod, cod_contatto,
                                    ClassCode, CInt(Cmb_Finalita.SelectedValue), Txt_Descrizione.Text,
                                    Decimal.Parse(Txt_CostoAcquisto.Text), Txt_Targa.Text, txt_telaio.Text,
                                    CInt(ddlMarca.SelectedValue), Txt_Modello.Text, Txt_Potenza.Text,
                                    Decimal.Parse(Txt_Ammortamento.Text), DataImmatricolazione, DataUltimaManutenzione,
                                    DataUltimaRevisione, IIf(Opt_Attivo.Checked, Txt_StatoUtilizzo.Text, "Dismesso"),
                                    Txt_Note.Text, Txt_NumImmatricolazione.Text, Txt_NumImmatricolazioneRimorchio.Text,
                                    Txt_NumAutorizzazione.Text, DataRilascioAutorizzazione, 0, 0, 0,
                                    CInt(ddlAlimentazione.SelectedValue), CInt(ddlPotenzaUdm.SelectedValue),
                                    txtCUAAProp.Text, txtProprietario.Text, CInt(ddlTipoTarga.SelectedValue),
                                    0, "", 0, "", "", Date.Parse("01/01/1900"), DataCarico, DataScarico,
                                    CInt(ddlTitoloPossesso.SelectedValue), "M", DataInizioUtilizzo, DataDismissione,
                                    "", objParametri_Server, taratura_ugelli, taratura_Inizio, taratura_Fine, visibile_ctrl_gestione, Txt_Codice.Text)

                    If Not esito Then
                        ConnessioniTransazioni.RollBackTransazione_ChiudiConnessione(objParametri_Server)
                        Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ImpossibileModificareLaMacchina"), String), Page, , UpdatePanel_script, , True)
                        Exit Try
                    End If

            End Select


            If Not ErroreGrigliaCosto Then


                Dim obj As New AgronicaCoreContabDAL.Prodotti_Costi_W


                If righeInseriteGrigliaCosto IsNot Nothing AndAlso righeInseriteGrigliaCosto.Any Then
                    For Each m As MacchineCostiModel In righeInseriteGrigliaCosto
                        esito = obj.Scrivi(Piva,
                                           "",
                                           MACCHINE,
                                           0,
                                           MacCod,
                                           0,
                                           CInt(m.Mezzo),
                                           CDec(m.Prezzo_Unitario),
                                           0,
                                           0,
                                           CDate(m.Validita_Inizio),
                                           CDate(m.Validita_Fine),
                                           objParametri_Server)


                        If Not esito Then
                            ConnessioniTransazioni.RollBackTransazione_ChiudiConnessione(objParametri_Server)
                            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ImpossibileCancellareDaProdottiCosti"), String), Page, , UpdatePanel_script, , True)
                            Exit Try
                        End If

                    Next
                End If


                If righeModificateGrigliaCosto IsNot Nothing AndAlso righeModificateGrigliaCosto.Any Then
                    For Each m As MacchineCostiModel In righeModificateGrigliaCosto
                        esito = obj.Scrivi_Completa(CInt(m.ID),
                                                    Piva,
                                                    "",
                                                    MACCHINE,
                                                    m.Pro_Cod,
                                                    MacCod,
                                                    0,
                                                    CInt(m.Mezzo),
                                                    CDec(m.Prezzo_Unitario),
                                                    0,
                                                    0,
                                                    CDate(m.Validita_Inizio),
                                                    CDate(m.Validita_Fine),
                                                    enum_TipoOperazioneDB.Modifica,
                                                    objParametri_Server)


                        If Not esito Then
                            ConnessioniTransazioni.RollBackTransazione_ChiudiConnessione(objParametri_Server)
                            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ImpossibileCancellareDaProdottiCosti"), String), Page, , UpdatePanel_script, , True)
                            Exit Try
                        End If
                    Next
                End If


                If righeEliminateGrigliaCosto IsNot Nothing AndAlso righeEliminateGrigliaCosto.Any Then
                    For Each m As MacchineCostiModel In righeEliminateGrigliaCosto
                        esito = obj.Cancella_da_ID(CInt(m.ID),
                                                   "",
                                                   objParametri_Server)

                        If Not esito Then
                            ConnessioniTransazioni.RollBackTransazione_ChiudiConnessione(objParametri_Server)
                            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ImpossibileCancellareDaProdottiCosti"), String), Page, , UpdatePanel_script, , True)
                            Exit Try
                        End If
                    Next
                End If

            End If
            ''------------------------------------------------
            ''----- Conferma di aggiornamento del database
            ''------------------------------------------------

            'COMMIT TRANSAZIONE
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

            EseguitaOperazione = True
            '------------------------------------------------

            'Assegno Nothing alle variabili delle righe kendo
            righeInseriteGrigliaCosto = Nothing
            righeModificateGrigliaCosto = Nothing
            righeEliminateGrigliaCosto = Nothing
            tutteleRigheGrigliaCosto = Nothing

            HttpContext.Current.Session("righeInseriteGrigliaCosto") = Nothing
            HttpContext.Current.Session("righeModificateGrigliaCosto") = Nothing
            HttpContext.Current.Session("righeEliminateGrigliaCosto") = Nothing
            HttpContext.Current.Session("tutteleRigheGrigliaCosto") = Nothing

        Catch exc As Exception

            Errore = True

            'Assegno Nothing alle variabili delle righe kendo
            righeInseriteGrigliaCosto = Nothing
            righeModificateGrigliaCosto = Nothing
            righeEliminateGrigliaCosto = Nothing
            tutteleRigheGrigliaCosto = Nothing

            HttpContext.Current.Session("righeInseriteGrigliaCosto") = Nothing
            HttpContext.Current.Session("righeModificateGrigliaCosto") = Nothing
            HttpContext.Current.Session("righeEliminateGrigliaCosto") = Nothing
            HttpContext.Current.Session("tutteleRigheGrigliaCosto") = Nothing

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!! e faccio il rollback!
            '------------------------------------------------
            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione_ChiudiConnessione(objParametri_Server)

            'Messaggio di errore
            Dim Messaggio As String = AgronicaAgenda_2010.SiEVerificatoUnErrore & exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================
        If Not Errore Then
            Dim Messaggio2 As String = DirectCast(GetLocalResourceObject("MACCHINASalvataConSuccesso"), String) & vbCrLf & vbCrLf

            Dim script As New StringBuilder
            script.AppendLine("GestioneUscita_ModalBS() ")

            Select Case tipo_salva.Value

                Case 1
                    Messaggio2 += "Verrà ricaricata la pagina per un ulteriore inserimento" & vbCrLf
                    Messaggi.AgroMsgBox(Messaggio2, Page, , UpdatePanel_script, , True)

                    Page_Load(Nothing, EventArgs.Empty)
                    Clear_form()
                Case enum_PagineAgenda_2010.Pagina_UMA_Richieste 'PopUp Aggiunta Macchine da Richieste UMA
                    'ScriptManager.RegisterStartupScript(UpdatePanel_script, UpdatePanel_script.GetType, "chiudiFinestra", "$(document).ready(window.parent.postMessage('chiudiFinestra'));", True)


                    'Dim javaScript As String = "window.parent.postMessage('chiudiFinestra');"
                    'ClientScript.RegisterStartupScript(Page.GetType(), "UniqueKeyForThisScript", javaScript, True)

                    'script.AppendLine("(window.parent.postMessage('chiudiFinestra'));")
                    'ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(),
                    '                             String.Format("jQuery_{0}", Page.ClientID), script.ToString, True)
                    'Page.ClientScript.RegisterStartupScript(Me.GetType(),
                    '                                            "AgendaBloccata",
                    '                                           script.ToString,
                    '                                            True)
                    Dim javaScript As String = "window.onload = function() { kendo.alert('Macchina salvata con successo!') };"
                    ClientScript.RegisterStartupScript(Me.GetType(), "alert", javaScript, True)

                    ImpostaPaginaSoloLettura()

                Case Else
                    Messaggi.AgroMsgBox(Messaggio2, Page, , UpdatePanel_script, script.ToString, True)

                    Dim xModalBS As String = Request.QueryString("modalBS")
                    If Not String.IsNullOrEmpty(xModalBS) AndAlso xModalBS = "1" Then

                    Else
                        Dim TargetUrl As String = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)
                        Response.Redirect(TargetUrl)
                    End If

            End Select

        End If
    End Sub

    Private Sub Clear_form()

        Me.Cmb_Tipo.ClearSelection()
        Me.Cmb_Dettaglio1.ClearSelection()
        Me.Cmb_Dettaglio2.ClearSelection()
        Me.Txt_Targa.Text = ""
        Me.txt_telaio.Text = ""
        Me.Txt_Modello.Text = ""
        Me.Txt_Potenza.Text = ""
        Me.Txt_Descrizione.Text = ""


        Me.Txt_DataInizioUtilizzo.Text = ""
        Me.Txt_DataUltimaManutenzione.Text = ""
        Me.Txt_DataImmatricolazione.Text = ""
        Me.Txt_DataUltimaRevisione.Text = ""
        Me.Txt_CostoAcquisto.Text = ""
        Me.Txt_Ammortamento.Text = ""

        Me.Txt_NumImmatricolazione.Text = ""
        Me.Txt_NumImmatricolazioneRimorchio.Text = ""
        Me.Txt_NumAutorizzazione.Text = ""
        Me.Txt_DataRilascioAutorizzazione.Text = ""

        Cmb_CentriAziendali.ClearSelection()
        ddlMarca.ClearSelection()
        Cmb_Finalita.ClearSelection()
        txtDtCarico.Text = ""
        txtDtScarico.Text = ""
        ddlTipoTarga.ClearSelection()
        txtProprietario.Text = ""
        ddlAlimentazione.ClearSelection()
        txtTaraUgello.Text = ""
        ddlTitoloPossesso.ClearSelection()
        txtCUAAProp.Text = ""
        ddlPotenzaUdm.ClearSelection()
        Txt_StatoUtilizzo.Text = ""
        Txt_Note.Text = ""
        Txt_Manutenzioni.Text = ""
    End Sub


    '########################################################################################
    Private Function XML_GeneraStringoneFinale() As String
        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        'uso i core per l'xml...
        Dim coreXml As New AgronicaCoreXML.XML_Contab

        Dim XmlDoc As New System.Xml.XmlDocument
        'Dim XmlDoc2 As New System.Xml.XmlDocument

        Dim XML_DatiAgenda As System.Xml.XmlElement
        Dim XML_Agenda As System.Xml.XmlElement
        Dim XML_DatiMovimenti As System.Xml.XmlElement
        Dim XML_Movimento As System.Xml.XmlElement
        Dim XML_DatiMovimentiDettagli As System.Xml.XmlElement
        Dim XML_MovimentoDettaglio As System.Xml.XmlElement
        Dim XML_DatiParcoMacchine As System.Xml.XmlElement
        Dim XML_ParcoMacchina As System.Xml.XmlElement

        Dim XML_DatiProdottiCosti As System.Xml.XmlElement
        Dim XML_ProdottoCosto() As System.Xml.XmlElement = Nothing


        'Dim str_DatiMovimentiDettagli As String
        'Dim str_MovimentoDettaglio As String
        'Dim str_MovimentoDestinazione As String
        'Dim strPrezzi As String


        ' Dim i As Integer
        Dim Descrizione As String
        'Dim Lav_Cod As Integer
        'Dim Magazzino_Cod As Integer
        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim DataImmatricolazione As Date
        Dim DataRilascioAutorizzazione As Date
        Dim DataUltimaManutenzione As Date
        Dim DataUltimaRevisione As Date
        Dim DataInizioUtilizzo As Date
        Dim DataDismissione As Date
        'Dim DittaCod As Integer = 0

        'Filo scriveva su PivasuperUser_Origine la PivasuperUser, sbagliato!
        'questo dati sono valorizzati dal gias2gias vecchia versione
        Dim Mac_Cod_Origine As Integer = ViewState("Mac_Cod_Origine")
        Dim PivasuperUser_Origine As String = ViewState("Piva_superUser_Origine")

        If Txt_DataImmatricolazione.Text <> "" Then
            If IsDate(Txt_DataImmatricolazione.Text) Then
                DataImmatricolazione = CDate(Txt_DataImmatricolazione.Text)
            Else
                DataImmatricolazione = #1/1/1900#
            End If
        Else
            DataImmatricolazione = #1/1/1900#
        End If

        If Me.Txt_DataRilascioAutorizzazione.Text <> "" Then
            If IsDate(Txt_DataRilascioAutorizzazione.Text) Then
                DataRilascioAutorizzazione = CDate(Txt_DataRilascioAutorizzazione.Text)
            Else
                DataRilascioAutorizzazione = #1/1/1900#
            End If
        Else
            DataRilascioAutorizzazione = #1/1/1900#
        End If

        If Txt_DataUltimaManutenzione.Text <> "" Then
            If IsDate(Txt_DataUltimaManutenzione.Text) Then
                DataUltimaManutenzione = CDate(Txt_DataUltimaManutenzione.Text)
            Else
                DataUltimaManutenzione = #1/1/1900#
            End If
        Else
            DataUltimaManutenzione = #1/1/1900#
        End If

        If Txt_DataUltimaRevisione.Text <> "" Then
            If IsDate(Txt_DataUltimaRevisione.Text) Then
                DataUltimaRevisione = CDate(Txt_DataUltimaRevisione.Text)
            Else
                DataUltimaRevisione = #1/1/1900#
            End If
        Else
            DataUltimaRevisione = #1/1/1900#
        End If

        If Txt_DataInizioUtilizzo.Text <> "" Then
            If IsDate(Txt_DataInizioUtilizzo.Text) Then
                DataInizioUtilizzo = CDate(Txt_DataInizioUtilizzo.Text)
            Else
                DataInizioUtilizzo = #1/1/1900#
            End If
        Else
            DataInizioUtilizzo = #1/1/1900#
        End If

        If Txt_DataDismissione.Text <> "" Then
            If IsDate(Txt_DataDismissione.Text) Then
                DataDismissione = CDate(Txt_DataDismissione.Text).ToShortDateString()
            Else
                DataDismissione = #12/31/2100#
            End If
        Else
            DataDismissione = #12/31/2100#
        End If




        Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                 Session("ASG_ProgressivoGIAS").ToString)

        '''setto le variabili per differenziare il caso del carico e dello scarico.
        ''If Me.Chk_Giacenza.Checked = True Then
        ''    Descrizione = "Giacenza " & IIf(Me.Cmb_Tipo.SelectedIndex > 0, Me.Cmb_Tipo.SelectedItem.Text, "") _
        ''                               & IIf(Me.Cmb_Dettagio1.SelectedIndex > 0, ", " & Me.Cmb_Dettagio1.SelectedItem.Text, "") _
        ''                               & IIf(Me.Cmb_Dettagio2.SelectedIndex > 0, ", " & Me.Cmb_Dettagio2.SelectedItem.Text, "")
        ''Else
        ''    Descrizione = "Acquisto " & IIf(Me.Cmb_Tipo.SelectedIndex > 0, Me.Cmb_Tipo.SelectedItem.Text, "") _
        ''                               & IIf(Me.Cmb_Dettagio1.SelectedIndex > 0, ", " & Me.Cmb_Dettagio1.SelectedItem.Text, "") _
        ''                               & IIf(Me.Cmb_Dettagio2.SelectedIndex > 0, ", " & Me.Cmb_Dettagio2.SelectedItem.Text, "")

        ''End If



        '''Calcolo il Class_Code
        ''Dim ClassCode As String
        ''ClassCode = ""
        ''ClassCode += Me.Cmb_Tipo.SelectedItem.Value
        ''ClassCode += IIf(Me.Cmb_Dettagio1.SelectedIndex > 0, "." & Me.Cmb_Dettagio1.SelectedItem.Value, "")
        ''ClassCode += IIf(Me.Cmb_Dettagio2.SelectedIndex > 0, "." & Me.Cmb_Dettagio2.SelectedItem.Value, "")

        ''setto le variabili per differenziare il caso del carico e dello scarico.
        'If Me.Chk_Giacenza.Checked = True Then
        '    Descrizione = "Giacenza "
        'Else
        Descrizione = AgronicaAgenda_2010.Acquisto & " "
        'End If

        Descrizione &= If(Me.Cmb_Tipo.SelectedIndex <> -1, Me.Cmb_Tipo.SelectedItem.Text, "")

        If Me.Cmb_Dettaglio1.SelectedIndex <> -1 Then
            Descrizione &= " - " & Me.Cmb_Dettaglio1.SelectedItem.Text
        End If
        If Me.Cmb_Dettaglio2.SelectedIndex <> -1 Then
            Descrizione &= " - " & Me.Cmb_Dettaglio2.SelectedItem.Text
        End If



        'Calcolo il Class_Code
        Dim ClassCode As String
        ClassCode = ""
        ClassCode &= Me.Cmb_Tipo.SelectedItem.Value
        If Me.Cmb_Dettaglio1.SelectedIndex > 0 Then
            ClassCode &= "." & Me.Cmb_Dettaglio1.SelectedItem.Value
        End If

        If Me.Cmb_Dettaglio2.SelectedIndex > 0 Then
            ClassCode &= "." & Me.Cmb_Dettaglio2.SelectedItem.Value
        End If


        'taratura ugelli
        Dim taratura_ugelli As Double = 0
        If (IsNumeric(txtTaraUgello.Text)) Then
            If (CDbl(txtTaraUgello.Text) > 0) Then
                taratura_ugelli = (CDbl(txtTaraUgello.Text))
            End If
        End If

        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        '######################################################################################################
        '----- DatiAgenda

        'XML_DatiAgenda = XmlDoc.CreateElement("DatiAgenda")


        '######################################################################################################
        '----- Agenda


        'Imposto il tag Agenda creato nella routine XML_Agenda_Agenda come figlio del tag XML_DatiAgenda
        'XML_DatiAgenda.InnerXml = XML_Agenda_Agenda( _
        '                                 enum_TipoOperazioneDB.Scrittura, _
        '                                viewstate("Piva"), _
        '                                0, _
        '                                0, _
        '                                Descrizione, _
        '                                1008, _
        '                                , _ 
        '                                , _
        '                                BaseCode, TopCode)

        ''Seleziono il nodo con il nome Agenda
        'XML_Agenda = XML_DatiAgenda.SelectSingleNode("Agenda")

        Dim errori As String = ""

        XML_Agenda = coreXml.MicroXML_Agenda(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, XmlDoc, xPiva,
                            0, 0, Descrizione, 1008, Date.Parse("01/01/1900"), Date.Parse("31/12/2100"), BaseCode, TopCode, errori)

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If


        XML_Movimento = coreXml.MicroXML_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura, XmlDoc, xPiva, 0, 0, 0, 0, CAU_CARICO,
                                               Descrizione, Date.Today, Date.Parse("31/12/2100"), 0, 0, 0, Date.Parse("01/01/1900"), Date.Parse("31/12/2100"),
                                                BaseCode, TopCode, DateTime.Parse("00" & System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator & "00"), 0, errori)


        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If



        XML_MovimentoDettaglio = coreXml.MicroXML_Agenda_Movimento_Dettaglio(enum_TipoOperazioneDB.Scrittura,
                                    XmlDoc, xPiva, 0, 0, 0, 0, 1, 0, 0, "", 38, 0, 1, 0, 0,
                                    Double.Parse(Me.Txt_CostoAcquisto.Text), 0, 0, 0,
                                     NONCONTABILE, enum_Pendenza.GiacenzeIniziali, Date.Parse("01/01/1900"), Date.Parse("31/12/2100"),
                                    BaseCode, TopCode, 0, CAU_CARICO, 0, "", Date.Now.Year, 0, 0, errori)

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If

        Dim MacCod As Integer = xMac_Cod
        Dim Piva As String = xPiva
        Dim dtCarico As Date = Date.Parse("01/01/1900")
        Dim dtScarico As Date = Date.Parse("01/01/1900")

        If (IsDate(txtDtCarico.Text)) Then
            dtCarico = Date.Parse(txtDtCarico.Text)
        End If

        If (IsDate(txtDtScarico.Text)) Then
            dtScarico = Date.Parse(txtDtScarico.Text)
        End If


        Dim sacod As Integer = 0
        'If Me.Chk_Movimentabile.Checked Then
        '    sacod = -1
        'Else
        '    sacod = 0
        'End If
        'i18n
        sacod = CInt(Cmb_CentriAziendali.SelectedValue)
        XML_ParcoMacchina = coreXml.MicroXML_ParcoMacchine(CInt(Operazione), XmlDoc, Piva,
                                                            sacod, MacCod, ClassCode,
                                                            Me.Txt_Descrizione.Text, Double.Parse(Me.Txt_CostoAcquisto.Text),
                                                            Me.Txt_Targa.Text, Me.txt_telaio.Text, CInt(ddlMarca.SelectedValue),
                                                            Me.Txt_Modello.Text,
                                                            Me.Txt_Potenza.Text, Double.Parse(Me.Txt_Ammortamento.Text), 0,
                                                            DataImmatricolazione, DataUltimaManutenzione, DataUltimaRevisione,
                                                            IIf(Me.Opt_Attivo.Checked, Me.Txt_StatoUtilizzo.Text, "Dismesso"),
                                                            DataInizioUtilizzo, DataDismissione, BaseCode, TopCode, Txt_Note.Text,
                                                            CInt(Me.Cmb_Finalita.SelectedValue), Me.Txt_NumImmatricolazione.Text,
                                                            Me.Txt_NumImmatricolazioneRimorchio.Text, Me.Txt_NumAutorizzazione.Text,
                                                            DataRilascioAutorizzazione, 0,
                                                            objParametri_Server.PivaSuperUser, CInt(ddlTipoTarga.SelectedValue),
                                                            CInt(ddlAlimentazione.SelectedValue), CInt(ddlPotenzaUdm.SelectedValue),
                                                            taratura_ugelli, CInt(ddlTitoloPossesso.SelectedValue),
                                                            txtCUAAProp.Text, txtProprietario.Text, dtCarico, dtScarico,
                                                            Mac_Cod_Origine,
                                                            PivasuperUser_Origine,
                                                            errori)

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If


        Dim DtCosti As DataTable = HttpContext.Current.Session("Dt_Costi")

        'XML_DatiProdottiCosti = XmlDoc.CreateElement("DatiProdotti_Costi")

        If Not IsNothing(DtCosti) Then

            If DtCosti.Rows.Count > 0 Then
                'creo un nodo per ogni prodotto
                Dim i As Integer
                For i = 0 To DtCosti.Rows.Count - 1

                    ReDim Preserve XML_ProdottoCosto(i)
                    XML_ProdottoCosto(i) = coreXml.MicroXML_Prodotti_Costi(enum_TipoOperazioneDB.Scrittura, XmlDoc, Piva, "", 1, 0, MacCod,
                                                            0, CInt(DtCosti.Rows(i).Item("Udm_Cod")),
                                                            CDbl(DtCosti.Rows(i).Item("Prezzo_Unitario")), 0, 0,
                                                             CDate(DtCosti.Rows(i).Item("Validita_Inizio")),
                                                             CDate(DtCosti.Rows(i).Item("Validita_Fine")), errori)


                Next

            Else

                'mi basta un nodo per cancellare tutti i record...
                ReDim Preserve XML_ProdottoCosto(0)
                XML_ProdottoCosto(0) = coreXml.MicroXML_Prodotti_Costi(enum_TipoOperazioneDB.Cancellazione, XmlDoc, "", "", 1, 0, MacCod,
                                                        0, 0, 0, 0, 0, CDate("01/01/1900"), CDate("31/12/2100"), errori)


            End If

        End If

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If



        'costruisco il documento....
        XML_DatiAgenda = XmlDoc.CreateElement("DatiAgenda")
        XML_DatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")
        XML_DatiMovimentiDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")
        XML_DatiParcoMacchine = XmlDoc.CreateElement("DatiParcoMacchine")
        XML_DatiProdottiCosti = XmlDoc.CreateElement("DatiProdotti_Costi")

        If (Not IsNothing(XML_ProdottoCosto)) Then
            'appendo tutti i nodi relativi ai costi..
            For Each xmlPC As System.Xml.XmlElement In XML_ProdottoCosto
                XML_DatiProdottiCosti.AppendChild(xmlPC)
            Next
            XML_ParcoMacchina.AppendChild(XML_DatiProdottiCosti)
        End If

        XML_DatiParcoMacchine.AppendChild(XML_ParcoMacchina)
        XML_MovimentoDettaglio.AppendChild(XML_DatiParcoMacchine)
        XML_DatiMovimentiDettagli.AppendChild(XML_MovimentoDettaglio)
        XML_Movimento.AppendChild(XML_DatiMovimentiDettagli)
        XML_DatiMovimenti.AppendChild(XML_Movimento)
        XML_Agenda.AppendChild(XML_DatiMovimenti)
        XML_DatiAgenda.AppendChild(XML_Agenda)
        XmlDoc.AppendChild(XML_DatiAgenda)


        Return XmlDoc.OuterXml

        'Dim xmlRoot As System.Xml.XmlElement = XML_DatiAgenda.AppendChild(XML_Agenda)



        ''creo la struttura...
        'XmlDoc.AppendChild(XML_DatiAgenda.AppendChild(XML_Agenda.AppendChild(XML_DatiMovimenti.AppendChild(XML_Movimento.AppendChild( _
        '                    XML_DatiMovimentiDettagli.AppendChild(XML_MovimentoDettaglio.AppendChild(XML_DatiParcoMacchine.AppendChild( _
        '                    XML_ParcoMacchina))))))))




        'XmlDoc.GetElementById("DatiAgenda").AppendChild(XML_Agenda).crea()


        ''appendo il nodo creato
        'XML_DatiAgenda.AppendChild(XML_Agenda)


        ''######################################################################################################
        ''----- DatiMovimenti

        ''Creo un tag volante di nome dati movimenti
        'XML_DatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

        ''Rendo il tag appena creato figlio del nodo agenda
        'XML_Agenda.AppendChild(XML_DatiMovimenti)

        ''######################################################################################################
        ''----- Movimento

        '''Imposto il tag Movimento creato nella routine XML_Agenda_Movimento come figlio del tag XML_DatiMovimenti
        ''XML_DatiMovimenti.InnerXml = XML_Agenda_Movimento( _
        ''                                 enum_TipoOperazioneDB.Scrittura, _
        ''                                viewstate("Piva"), _
        ''                                0, _
        ''                                0, _
        ''                                0, _
        ''                                0, _
        ''                                CAU_CARICO, _
        ''                                Descrizione, _
        ''                                 CDate(Date.Today), _
        ''                                 , _
        ''                                0, _
        ''                                0, _
        ''                                , _
        ''                                , _
        ''                                , BaseCode, TopCode)

        ''XML_Movimento = XML_DatiMovimenti.SelectSingleNode("Movimento") 'selezione




        ''######################################################################################################
        ''----- DatiMovimenti_Dettagli

        ''Creo un tag volante di nome dati movimenti
        'XML_DatiMovimentiDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

        ''Imposto il tag volante come figlio di Movimento
        'XML_Movimento.AppendChild(XML_DatiMovimentiDettagli)

        ''Inizializzo
        'str_DatiMovimentiDettagli = ""

        ''######################################################################################################
        ''----- Movimento_Dettaglio

        'str_MovimentoDettaglio = XML_Agenda_MovimentoDettaglio( _
        '                             enum_TipoOperazioneDB.Scrittura, _
        '                            viewstate("Piva"), _
        '                            , _
        '                            , _
        '                            , _
        '                            , _
        '                            1, _
        '                            , _
        '                            , _
        '                            , _
        '                            38, _
        '                            , _
        '                             1, _
        '                             , _
        '                            , _
        '                            Me.Txt_CostoAcquisto.Text, , , , _
        '                             NONCONTABILE, _
        '                             enum_Pendenza.GiacenzeIniziali, , _
        '                            , _
        '                             BaseCode, TopCode, , _
        '                             CAU_CARICO, , , , , _
        '                             Date.Today.Year)

        ''Carica il documento XML dalla stringa specificata
        'XmlDoc2.LoadXml(str_MovimentoDettaglio)

        'XML_MovimentoDettaglio = XmlDoc2.SelectSingleNode("Movimento_Dettaglio")



        ''Inizializzo
        'str_DatiMovimentiDettagli = ""

        ''Dim MacCod As Integer
        ''MacCod = viewstate("Mac_Cod")
        ''Dim Piva As String
        ''Piva = viewstate("Piva")


        ''Creo il figlio Movimento_Destinazione com figlio di Movimento_Dettaglio
        'XML_MovimentoDettaglio.InnerXml = XML_Agenda_ParcoMacchine(Qs_Operazione, _
        '                                                           viewstate("Piva"), _
        '                                                           IIf(Me.Chk_Movimentabile.Checked = True, -1, 0), _
        '                                                           MacCod, _
        '                                                           ClassCode, _
        '                                                           Me.Txt_Descrizione.Text, _
        '                                                           Me.Txt_CostoAcquisto.Text, _
        '                                                           Me.Txt_Targa.Text, _
        '                                                           Me.Txt_Telaio.Text, _
        '                                                           DittaCod, _
        '                                                           Me.Txt_Modello.Text, _
        '                                                           Me.Txt_Potenza.Text, _
        '                                                           Me.Txt_Ammortamento.Text, _
        '                                                           , _
        '                                                           DataImmatricolazione, _
        '                                                           DataUltimaManutenzione, _
        '                                                           DataUltimaRevisione, _
        '                                                           IIf(Me.Opt_Attivo.Checked, Me.Txt_StatoUtilizzo.Text, "Dismesso"), _
        '                                                           DataInizioUtilizzo, _
        '                                                           DataDismissione, _
        '                                                           BaseCode, _
        '                                                           TopCode, _
        '                                                           Txt_Note.Text, _
        '                                                           CInt(Me.Cmb_Finalita.SelectedValue), _
        '                                                           Me.Txt_NumImmatricolazione.Text, _
        '                                                           Me.Txt_NumImmatricolazioneRimorchio.Text, _
        '                                                           Me.Txt_NumAutorizzazione.Text, _
        '                                                           DataRilascioAutorizzazione, _
        '                                                           Me.Txt_Peso.Text)

        'XML_DatiParcoMacchine = XML_MovimentoDettaglio.SelectSingleNode("DatiParcoMacchine")

        'XML_ParcoMacchina = XML_DatiParcoMacchine.SelectSingleNode("ParcoMacchina")

        'strPrezzi = XML_GeneraStringonePrezzi()

        'XML_ParcoMacchina.InnerXml = strPrezzi


        'str_DatiMovimentiDettagli = str_DatiMovimentiDettagli + XmlDoc2.OuterXml
        ''IIf(IsDate(Me.Txt_DataImmatricolazione.Text), Me.Txt_DataImmatricolazione.Text, #1/1/1900#)


        'XML_DatiMovimentiDettagli.InnerXml = str_DatiMovimentiDettagli

        ''----- Assemblo la struttura

        'XmlDoc.AppendChild(XML_DatiAgenda)

        ''----- Restituisco il risultato

        'Return XmlDoc.OuterXml


    End Function

    Private Sub Cmb_Tipo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Tipo.SelectedIndexChanged
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(Cmb_Dettaglio1, 2, Me.Cmb_Tipo.SelectedItem.Value, "", objParametri_Server)
        If Cmb_Dettaglio1.Items.Count > 0 Then
            Cmb_Dettaglio1.Enabled = True
        End If
    End Sub

    Private Sub Cmb_Dettaglio1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Dettaglio1.SelectedIndexChanged
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(Cmb_Dettaglio2, 3, Me.Cmb_Tipo.SelectedItem.Value, Me.Cmb_Dettaglio1.SelectedItem.Value, objParametri_Server)
        If Cmb_Dettaglio2.Items.Count > 0 Then
            Cmb_Dettaglio2.Enabled = True
        End If
    End Sub

    Private Sub Salva_Tutto()

        Dim Messaggio As String

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim StringaXmlCreazione As String
        'Dim StringaXmlCancellazione As String

        'Dim strDummy As String


        '------------------------------------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------------------------------------


        'Se voglio cancellare un'operazione non mi servono i controlli
        If Operazione <> enum_TipoOperazioneDB.Cancellazione Then

            '---------------------------------------------------------
            '----- Verifico la correttezza delle informazioni inserite
            '---------------------------------------------------------

            ''controllo che sia stata almeno impostata la prima compo del tipo di macchina
            'If Me.Cmb_Finalita.SelectedValue = "0" Then
            '    If Me.Cmb_Tipo.SelectedIndex <= 0 Then
            '        Messaggio = "Scegliere almeno il Tipo di macchinario"
            '        Call AgroMsgBox(Messaggio, Page)
            '        Exit Sub
            '    End If
            'End If

            ''Imposto il format
            'Format(Me.Txt_CostoAcquisto.Text, "##,###,##0.00")
            ''Verifico la quantità se e' nulla o negativa
            'If CDbl(Me.Txt_CostoAcquisto.Text) < 0 Or (Not IsNumeric(Me.Txt_CostoAcquisto.Text)) Then
            '    Messaggio = "Il Costo di Acquisto non può essere negativo"
            '    Call AgroMsgBox(Messaggio, Page)
            '    Exit Sub
            'End If

            'Format(Me.Txt_Ammortamento.Text, "##,###,##0.00")
            ''Verifico la quantità se e' nulla o negativa
            'If CDbl(Me.Txt_Ammortamento.Text) < 0 Or (Not IsNumeric(Me.Txt_Ammortamento.Text)) Then
            '    Messaggio = "L'ammortamento non può essere negativo"
            '    Call AgroMsgBox(Messaggio, Page)
            '    Exit Sub
            'End If


            'Format(Me.Txt_Peso.Text, "##,###,##0.00")
            ''Verifico la quantità se e' nulla o negativa
            'If CDbl(Me.Txt_Peso.Text) < 0 Or (Not IsNumeric(Me.Txt_Peso.Text)) Then
            '    Messaggio = "Il Peso non può essere negativo"
            '    Call AgroMsgBox(Messaggio, Page)
            '    Exit Sub
            'End If


            '------------------------------------------------
            '----- Calcolo i valori di BaseCode e TopCode
            '------------------------------------------------

            Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                     Session("ASG_ProgressivoGIAS").ToString)

            '------------------------------------------------
            '------------------------------------------------
            '------------------------------------------------


        End If

        Dim Errore As Boolean = False

        '=======================
        '===  Aggiornamento  ===
        '=======================

        'INIZIALIZZO LA TRANSAZIONE......
        ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

        Try

            '------------------------------------------------
            '----- Effettuo le modifiche al database
            '------------------------------------------------

            Select Case Operazione

                Case enum_TipoOperazioneDB.Scrittura

                    'Creo la stringa di inserimento
                    StringaXmlCreazione = XML_GeneraStringoneFinale()

                    Dim objAgendaInserisciWriteNew As New AgronicaCoreContabBIZ.Agenda_W

                    'I PARAM flagmirror e rimappacodici non più utilizzati 
                    Dim resOp As Boolean = objAgendaInserisciWriteNew.Agenda_Scrivi(StringaXmlCreazione, 0, 0,
                                                                                    CInt(Session("ASG_IdServizio")),
                                                                                    0, "",
                                                                                    objParametri_Server)


                    'AGGIORNAMENTO MOVIMENTI D'AGENDA
                    '      AggiornaAgenda()

                Case enum_TipoOperazioneDB.Modifica

                    Dim Id_Agenda_vecchio As Integer
                    Dim Lav_ForDelete As String
                    'Dim xLav_Cod As Integer
                    Dim xFiltroAggiuntivo As String = " Agenda.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ACQUISTO_BENI)


                    'leggo il vecchio id_agenda
                    Dim objAgendaVecchia As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                    'CStr(viewstate("Piva"))
                    'in realtà si potrebbe filtrare per viewstate("Piva")
                    'ma in alcuni archivi, non si sa perché,
                    'la piva dell'operazione di agenda è diversa dalla piva di parco_macchine!
                    'cmq c'è una sola operazione per macchina, quindi basta il filtro sul mac_cod 
                    '(aggiungo anche il filtro sul lav_cod=1008)
                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
                    Dim dtRes As DataTable = objAgendaVecchia.Leggi("",
                                                                    0,
                                                                    0, 0, 0,
                                                                    MACCHINE,
                                                                    0,
                                                                    CInt(xMac_Cod),
                                                                    CStr(CAU_CARICO),
                                                                    0, 0, 0, 0, 0, 0,
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                    xFiltroAggiuntivo, "",
                                                                    objParametri_Server)
                    objParametri_Server.ResettaFinestra()

                    Select Case dtRes.Rows.Count
                        Case Is = 1
                            Id_Agenda_vecchio = CInt(dtRes.Rows(0)("id_agenda"))
                        Case Is = 0
                            Throw New Exception(DirectCast(GetLocalResourceObject("NessunaOperazioneDiCaricoDellaMacchinaImpossibileSalvare"), String))
                        Case Else
                            Throw New Exception(DirectCast(GetLocalResourceObject("MoltepliciOperazioniDiCaricoDellaMacchinaImpossibileSalvare"), String))
                    End Select

                    'ricreo l'oggetto appena eliminato
                    Dim objAgendaW As New AgronicaCoreContabBIZ.Agenda_W
                    Dim objAgendaRead As New AgronicaCoreContabBIZ.Agenda_R

                    'errore!!!! prima usava xSa_Cod
                    Lav_ForDelete = objAgendaRead.Agenda_Leggi("",
                                                                0,
                                                                Id_Agenda_vecchio,
                                                                LAVCOD_ACQUISTO_BENI,
                                                                True,
                                                                objParametri_Server)

                    If Lav_ForDelete = "" Then
                        Throw New Exception(DirectCast(GetLocalResourceObject("OperazioneDaModificareNonTrovata"), String))
                    End If


                    objAgendaW.Agenda_Scrivi(Lav_ForDelete,
                                            Id_Agenda_vecchio,
                                            0,
                                            CInt(Session("ASG_IdServizio")),
                                            0,
                                            "", objParametri_Server)

                    'Creo la stringa di modifica
                    'che inserisce una nuova operazione di agenda e modifica il parco macchine
                    StringaXmlCreazione = XML_GeneraStringoneFinale()

                    Dim Id_Agenda_new As Integer = 0
                    Dim resOp As Boolean = objAgendaW.Agenda_Scrivi(StringaXmlCreazione,
                                                                    Id_Agenda_new,
                                                                    0,
                                                                    CInt(Session("ASG_IdServizio")),
                                                                    0, "",
                                                                    objParametri_Server)


                    'AGGIORNAMENTO MOVIMENTI D'AGENDA
                    '    AggiornaAgenda()

            End Select


            ''------------------------------------------------
            ''----- Conferma di aggiornamento del database
            ''------------------------------------------------

            'COMMIT TRANSAZIONE
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

            'Throw New ApplicationException


            EseguitaOperazione = True
            'Page_Ok()
            '------------------------------------------------


        Catch exc As Exception

            Errore = True

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!! e faccio il rollback!
            '------------------------------------------------
            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione_ChiudiConnessione(objParametri_Server)

            ''Faccio abortire la transazione
            ''System.EnterpriseServices.ContextUtil.SetAbort()
            'Esci_Annulla()

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore &
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

            'Dim script As New StringBuilder
            'script.AppendLine("$(document).ready(function () { MessaggioErrore_Bootstrap(""" & Messaggio & """) }); ")
            'ScriptManager.RegisterClientScriptBlock(page, page.GetType(),
            '                             String.Format("jQuery_{0}", page.ClientID), script.ToString, True)

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================
        If Not Errore Then
            'Dim Piva As String

            'Ritorno alla pagina AlberoImprese
            'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)
            Dim Messaggio2 As String
            Messaggio2 = DirectCast(GetLocalResourceObject("MACCHINASalvataConSuccesso"), String) & vbCrLf & vbCrLf

            Dim script As New StringBuilder
            script.AppendLine("GestioneUscita_ModalBS() ")

            Dim TargetUrl As String
            Select Case tipo_salva.Value

                Case 1
                    Messaggio2 += "Verrà ricaricata la pagina per un ulteriore inserimento" & vbCrLf
                    Messaggi.AgroMsgBox(Messaggio2, Page, , UpdatePanel_script, , True)

                    Page_Load(Nothing, EventArgs.Empty)
                    Clear_form()
                Case Else
                    Messaggi.AgroMsgBox(Messaggio2, Page, , UpdatePanel_script, script.ToString, True)

                    Dim xModalBS As String = Request.QueryString("modalBS")
                    If Not String.IsNullOrEmpty(xModalBS) AndAlso xModalBS = "1" Then

                    Else
                        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                        Response.Redirect(TargetUrl)

                    End If


            End Select

        End If


    End Sub

    Private Sub Opt_Dismesso_CheckedChanged(sender As Object, e As EventArgs) Handles Opt_Dismesso.CheckedChanged
        Txt_DataDismissione.Enabled = True
    End Sub

    Private Sub Opt_Attivo_CheckedChanged(sender As Object, e As EventArgs) Handles Opt_Attivo.CheckedChanged
        Txt_DataDismissione.Text = ""
        Txt_DataDismissione.Enabled = False
    End Sub

    Private Class MacchineCostiModel
        Public ID As Integer? = Nothing
        Public Udm_Cod As Integer? = Nothing
        Public Udm_Des As String = ""
        Public Prezzo_Unitario As Decimal? = Nothing
        Public Validita_Inizio As Date = "1900/01/01"
        Public Validita_Fine As Date = "2100/12/31"
        Public Pro_Cod As Integer? = Nothing
        Public Mezzo As Integer? = Nothing
    End Class

End Class