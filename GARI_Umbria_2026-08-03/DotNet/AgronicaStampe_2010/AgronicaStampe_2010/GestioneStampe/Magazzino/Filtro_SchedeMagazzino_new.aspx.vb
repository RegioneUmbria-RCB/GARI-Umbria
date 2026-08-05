Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ



Public Class Filtro_SchedeMagazzino_new
    Inherits System.Web.UI.Page

#Region " Filtro schede magazzino "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents LABEL18 As System.Web.UI.WebControls.Label

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Fabbricato_Cod As String
    Dim Qs_Pro_Cod As String
    Dim Qs_Mat_Cod As String
    Dim Qs_Elem_Cod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim QS_DataStampa As String

    'Dim Qs_Origine As String
    Dim Qs_Destinazione As String
    'Dim Qs_Chiave As String

    Const PaginaLinkStampaSchedaGiacenzeMagazzino As String = "Giacenze/SchedaGiacenzeMagazzino_2.aspx"
    Const PaginaLinkStampaSchedaMovimentiMagazzino As String = "Movimenti/SchedaMovimentiMagazzino.aspx"
    Const PaginaLinkStampaSchedaFertilizzantiMagazzino As String = "Fertilizzanti/SchedaFertilizzantiMagazzino.aspx"
    Const PaginaLinkStampaSchedaProdottiFitosanitariMagazzino As String = "ProdottiFitosanitari/SchedaProdottiFitosanitariMagazzino.aspx"
    Const PaginaLinkStampaSchedaRiepilogoProdottiUtilizzati As String = "RiepilogoProdotti/RiepilogoProdotti.aspx"

    Const PaginaLinkStampaSchedaMovimentiMagazzinoExcel As String = "Movimenti/SchedaMovimentiMagazzinoExcel.aspx"

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Permessi As AgronicaCoreUtentiDAL.PermessiUtente

#Region "Script.Services"

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Validazione_Excel(ByVal tipo_scheda As String) As RispostaStandard
        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Select Case tipo_scheda
            Case enum_CodificaStampe.SchedaMagazzinoMovimenti
            Case Else
                r.RispostaOK = False
                r.Errore = "Funzione non ancora disponibile!"
                Return r
        End Select

        r.RispostaOK = True

        Return r
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Sub Salva_tipo_scheda(ByVal value As Integer)

        '04/04/2018 - Giulia: devo sovrascrivere anche questo perché è quello che viene letto da "Tipo_Scheda"
        '   e se non lo cambio mi rimette la scelta di partenza con cui è stata aperta la pagina, perdendo eventuale cambio di tipo stampa
        HttpContext.Current.Session("ReportSelezionato") = value

    End Sub


    '#########################################################################################
    '<Script.Services.ScriptMethod()>
    '<WebMethod(EnableSession:=True)>
    'Public Shared Function Tipo_Scheda() As String
    '    Dim r As String

    '    r = HttpContext.Current.Session("ReportSelezionato")

    '    Return r
    'End Function

    '#########################################################################################
    '<Script.Services.ScriptMethod()> _
    '<WebMethod(EnableSession:=True)> _
    'Public Shared Function Carica_date() As String()
    '    Dim r(2) As String

    '    r(0) = HttpContext.Current.Session("QS_DataStampa")
    '    r(1) = HttpContext.Current.Session("QS_DataInizio")
    '    r(2) = HttpContext.Current.Session("QS_DataFine")

    '    Return r
    'End Function

    '#########################################################################################
    '<Script.Services.ScriptMethod()>
    '<WebMethod(EnableSession:=True)>
    'Public Shared Function Salva_Date(ByVal dataStampa As String, ByVal dataInizio As String, ByVal dataFine As String)

    '    HttpContext.Current.Session("QS_DataStampa") = dataStampa
    '    HttpContext.Current.Session("QS_DataInizio") = dataInizio
    '    HttpContext.Current.Session("QS_DataFine") = dataFine

    'End Function

    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_ProdottiWS(ByVal testo As String,
                                             ByVal gruppo_date As String,
                                             ByVal cat_prod As String,
                                             ByVal impresa As String,
                                             ByVal sa_cod As String,
                                             ByVal fabbricato_cod As String,
                                             ByVal tipo_scheda As String
                                             ) As String()

        Dim r(1) As String

        Dim centro_cod As Integer
        Dim fab_cod As Integer

        Dim Filtro_Desc As String = testo
        Dim d As String() = gruppo_date.Split(New Char() {","c})
        'gruppo_date = $('#<%=TxtDataDa.ClientID %>').val() + "," + $('#<%=TxtStampa.ClientID %>').val() + "," + $('#<%=TxtDataA.ClientID %>').val();

        Dim Data As Date

        Select Case tipo_scheda

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti,
             enum_CodificaStampe.RiepilogoProdottiUtilizzati

                If d(2) = "" Then
                    d(2) = CStr(Date.Today)
                End If
                Data = d(2)

                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                  enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari
                If d(0) = "" Then
                    d(0) = CStr(Date.Today)
                End If
                Data = d(0)

                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze
                If d(1) = "" Then
                    d(1) = CStr(Date.Today)
                End If
                Data = d(1)

                '-----------------------------------------------------------
        End Select

        If sa_cod = "" Or Not IsNumeric(sa_cod) Then
            centro_cod = 0
        Else
            centro_cod = CInt(sa_cod)
        End If


        If fabbricato_cod = "" Or Not IsNumeric(fabbricato_cod) Then
            fab_cod = 0
        Else
            fab_cod = CInt(fabbricato_cod)
        End If

        Dim cmb_prod As New DropDownList

        Select Case cat_prod

            Case MACCHINE, SEMENTI, ALTRE_MATERIE, MANGIMI, FARMACI,
                SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                            SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI

                'nel caso delle sementi non posso usare la ProdottiAnagrafica
                'perché altrimenti mi caricherebbe le tipologie sementi
                'uso cmq questa funzione per tutte le materie_prime, perché ho bisogno del codice negativo
                '(per distinguere il amt_cod da pro_cod)
                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.Materie_Prime(cmb_prod,
                                                                    True, "", "",
                                                                    CAU_SCARICO,
                                                                    impresa,
                                                                    centro_cod,
                                                                    fabbricato_cod,
                                                                    cat_prod,
                                                                    True,
                                                                    Filtro_Desc,
                                                                    "",
                                                                    "",
                                                                    "",
                                                                    0, 0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO,
                                                                    0, 0, 0, 0, 0, 0, 0,
                                                                    Data,
                                                                    "", "",
                                                                    HttpContext.Current.Session("ASG_objParametri_Server"),
                                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Case CARBURANTI, FERTILIZZANTI, FORMULATI, COADIUVANTI, INSETTI, TRAPPOLE, INNESCHI

                Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
                clc.Prodotti(cmb_prod,
                              True,
                              "",
                              "",
                           CAU_SCARICO,
                          impresa,
                          centro_cod,
                          fab_cod,
                          cat_prod,
                          True,
                          Filtro_Desc,
                          True,
                          False,
                          0, 0,
                          Data,
                          0,
                          0,
                          True,
                          False,
                          "", "", HttpContext.Current.Session("ASG_objParametri_Server"),
                          HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Case Else
                ' Categorie limite che non possono essere scaricate dal magazzino
                cmb_prod.Items.Add(New ListItem("", ""))
        End Select


        Dim rval As String = ""
        For Each itm As ListItem In cmb_prod.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval
        r(1) = cmb_prod.Items.Count - 1

        Return r

    End Function

    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_ImpreseWS(ByVal testo As String) As String()

        Dim r(1) As String

        Dim cmb_imprese As New DropDownList

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0


        'If TestoCercaRagSoc <> "" Then
        strFiltro = " (Imprese.Rag_Soc like '%" & testo & "%') "
        'End If

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_imprese, False, "", "",
                                                         strFiltro, " ORDER BY Imprese.Rag_Soc asc",
                                                           HttpContext.Current.Session("ASG_objParametri_Server"),
                                                           HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_imprese.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval
        r(1) = cmb_imprese.Items.Count

        Return r

    End Function

    '###########################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Centro_Aziendale(ByVal Piva As String,
                                                   ByVal sa_cod As String,
                                                   ByVal QS_Sa_Cod As String,
                                                   ByVal QS_Fabbricato_Cod As String
                                                   ) As String()

        Dim r(4) As String
        Dim Cmb_CentroAz As New DropDownList

        HttpContext.Current.Session("PartitaIVA") = Piva

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim clc = New AgronicaCoreUtility.CaricaListControl

        clc.Centri_Aziendali(Cmb_CentroAz,
                                                               True, "Tutti i Centri Aziendali", "0",
                                                               Piva,
                                                               False,
                                                               2,
                                                               "", "",
                                                               objParametri_Server)


        Dim objOmniLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
        Dim fabbricato_cod As Integer

        objOmniLog.Recupera_Magazzino_Confezionato(Piva,
                                                   "",
                                                   objParametri_Server,
                                                   CInt(sa_cod),
                                                   fabbricato_cod)

        If CInt(sa_cod) = 0 AndAlso fabbricato_cod = 0 Then
            
            If QS_Sa_Cod <> 0 AndAlso QS_Fabbricato_Cod <> 0 Then
                sa_cod = QS_Sa_Cod
                fabbricato_cod = QS_Fabbricato_Cod
            Else
                sa_cod = 0
                fabbricato_cod = 0
            End If
        Else
            QS_Sa_Cod = CInt(sa_cod)
            QS_Fabbricato_Cod = fabbricato_cod
        End If

        Dim rval As String = ""
        Dim i As Integer = 0
        If CInt(sa_cod) = 0 Then
            If Cmb_CentroAz.Items.Count = 2 Then
                'Ho un unico centro ==> lo seleziono direttamente (anche se nella lista ci sono 2 voci, perché c'è anche tutti)
                For Each itm As ListItem In Cmb_CentroAz.Items
                    If i = 1 Then
                        rval &= "<option value=""" & itm.Value & """ selected>" & itm.Text & "</option>"
                    Else
                        rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
                    End If

                    i += 1
                Next
            Else
                'Seleziono direttamente la priva voce (cioè tutti i centri) 
                For Each itm As ListItem In Cmb_CentroAz.Items
                    If i = 0 Then
                        rval &= "<option value=""" & itm.Value & """ selected>" & itm.Text & "</option>"
                    Else
                        rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
                    End If

                    i += 1
                Next
            End If

        Else
            'In input ho un centro preciso ==> viene selezionata la voce di quel centro
            For Each itm As ListItem In Cmb_CentroAz.Items
                If itm.Value = CStr(sa_cod) Then
                    rval &= "<option value=""" & itm.Value & """ selected>" & itm.Text & "</option>"
                Else
                    rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
                End If
            Next

        End If

        'In risposta = elenco option, num centri, fabbricato_cod, Qs_Sa_Cod, Qs_Fabbricato_Cod
        r(0) = rval
        r(1) = Cmb_CentroAz.Items.Count - 1
        r(2) = fabbricato_cod
        r(3) = QS_Sa_Cod
        r(4) = QS_Fabbricato_Cod

        Return r

    End Function

    '###########################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Magazzini(ByVal piva As String, ByVal sa_cod As Integer, ByVal fabbricato_cod As Integer) As String()

        Dim Cmb_Mag As New DropDownList

        Dim r(2) As String

        Dim clc = New AgronicaCoreUtility.CaricaListControl
        clc.Fabbricati(Cmb_Mag,
                                        True, "Tutti i Magazzini", "0",
                                        piva,
                                        sa_cod,
                                        0,
                                        FABBRICATI_NO_STALLE,
                                         False,
                                         "",
                                         " Fabbricati.Fabbricato_Des ",
                                        AGRODATAFINE,
                                         HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim rval As String = ""
        ' se ho solo un centro aziendale lo seleziono
        If CInt(Cmb_Mag.Items.Count) <> 2 Then
            For Each itm As ListItem In Cmb_Mag.Items
                If itm.Value = CStr(fabbricato_cod) Then
                    rval &= "<option value=""" & itm.Value & """ selected>" & itm.Text & "</option>"
                Else
                    rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
                End If
            Next
        Else
            Dim i = 0
            For Each itm As ListItem In Cmb_Mag.Items
                If i = 1 Then
                    rval &= "<option value=""" & itm.Value & """ selected>" & itm.Text & "</option>"
                Else
                    rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
                End If

                i += 1
            Next
        End If

        r(0) = rval
        r(1) = Cmb_Mag.Items.Count - 1

        Dim RegioneCod As String = ""
        Dim objF As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        objF.MagazzinoIndirizzo_Leggi_2(piva, sa_cod, fabbricato_cod,
                                        Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing,
                                        RegioneCod,
                                        HttpContext.Current.Session("ASG_objParametri_Server"))

        'Seleziono la regione di appartenenza dell'impresa
        r(2) = RegioneCod

        Return r

    End Function

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If
        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Permessi = New AgronicaCoreUtentiDAL.PermessiUtente() 'leggo i permessi che serviranno anche lato client


        '=====================================================
        '----- Recupero i valori dalla querystring
        '=====================================================

        QS_DataStampa = Stringa_Decodifica(Request.QueryString("ds").ToString, AgroKey_EncoderDecoder)

        QS_DataInizio = Stringa_Decodifica(Request.QueryString("di").ToString, AgroKey_EncoderDecoder)
        QS_DataFine = Stringa_Decodifica(Request.QueryString("df").ToString, AgroKey_EncoderDecoder)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, AgroKey_EncoderDecoder)

        'TODO: verificare la funzione in agronicacorexml\xml_stampe\XML_EstraiVariabiliStampe (non viene letto il sa_Cod)
        If Qs_Sa_Cod = "" Then
            Qs_Sa_Cod = "0"
        End If

        Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("f").ToString, AgroKey_EncoderDecoder)

        If Qs_Fabbricato_Cod = "" Then
            Qs_Fabbricato_Cod = "0"
        End If

        hdQs_Sa_Cod.Value = Qs_Sa_Cod
        hdQs_Fabbricato_Cod.Value = Qs_Fabbricato_Cod

        Qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("e").ToString, AgroKey_EncoderDecoder)

        Qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro").ToString, AgroKey_EncoderDecoder)

        Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat").ToString, AgroKey_EncoderDecoder)


        ''########################################################################
        ''#####  Verifica se la pagina e' stata caricata per la prima volta  #####
        ''########################################################################

        If Not Page.IsPostBack Then

            'Le date le metto in sessione solo se non è PostBack, perché le ho già salvate in sessione lato client prima dei tasti stampa/stampa excel
            hdQS_DataStampa.Value = QS_DataStampa
            hdQS_DataInizio.Value = QS_DataInizio
            hdQS_DataFine.Value = QS_DataFine

            'ReportSelezionato rimane gestito anche in sessione per non creare casini se serve nelle pagine successive
            If HttpContext.Current.Session("ReportSelezionato") = 0 Then
                hdTipo_Scheda.Value = 11
            Else
                hdTipo_Scheda.Value = HttpContext.Current.Session("ReportSelezionato")
            End If
            hdReportSelezionato.Value = hdTipo_Scheda.Value

            hds_piva.Value = ""
            hds_sa_cod.Value = ""
            hds_fabbricato_cod.Value = ""

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = False
            UtenteAbilitato = objUtenti.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                  Session("ASG_IdServizio"),
                                                                  enum_Security_Attivita.Stampa_MovimentiMagazziniExcel,
                                                                  enum_Security_Operazione.Lettura,
                                                                  Now,
                                                                  "",
                                                                  objParametri_Utenti)

            Dim UtenteAbilitatoBlocco As Boolean = False
            UtenteAbilitatoBlocco = objUtenti.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                        Session("ASG_IdServizio"),
                                                                        enum_Security_Attivita.Agenda_Operazioni_Blocco,
                                                                        enum_Security_Operazione.Modifica,
                                                                        Now,
                                                                        "",
                                                                        objParametri_Utenti)
        Else

            '///////////////////////////////////////
            '/// La pagina ha subito un PostBack ///
            '///////////////////////////////////////

            'Non faccio nulla
            Exit Sub

        End If


        '=====================================================
        '----- Inizializzo i controlli
        '=====================================================

        'DEFAULT SULLA STAMPA DEL LOTTO
        Dim Is_UtenteGiasLan As Boolean = False
        Dim objProfil As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim clc = New AgronicaCoreUtility.CaricaListControl
        Is_UtenteGiasLan = objProfil.Is_UtenteGiasLan("", objParametri_Utenti)
        If Is_UtenteGiasLan = True Then
            Me.Rbl_StampaLotto.SelectedValue = 1
            Me.Rbl_Raggruppamento.SelectedValue = 1
        Else
            Me.Rbl_StampaLotto.SelectedValue = 0
            Me.Rbl_Raggruppamento.SelectedValue = 0
        End If

        '-------------------------------------------------------------------
        '----- Logo Regionale

        ' Per default viene stampato il logo della regione
        Chk_LogoRegione.Checked = True
        AgronicaCoreUtility.CaricaListControl.Regioni(Me.Cmb_Regioni,
                                                        True, " ", "",
                                                        "",
                                                        "", "",
                                                        objParametri_Server)


        If Qs_Piva <> "" Then

            'chiamo CaricaCombo_Imprese perché devo caricare solo l'impresa che ha quella piva
            '(per caricare tutte le imprese uso CaricaCombo_ImpreseUtente_Optimize)
            AgronicaCoreUtility.CaricaListControl.Imprese(Me.Cmb_Impresa,
                                                False,
                                                "", "",
                                                Qs_Piva,
                                                "", "",
                                                objParametri_Server)


            Me.Lbl_NumImprese.Text = CStr(Me.Cmb_Impresa.Items.Count)

        End If

        If Qs_Sa_Cod <> "" Then

            clc.Centri_Aziendali(Cmb_CentroAziendale,
                                                                   True, "Tutti i Centri Aziendali", "0",
                                                                    Qs_Piva, False, 2, "", "", objParametri_Server)


            Me.Lbl_NumCentri.Text = CStr(Me.Cmb_CentroAziendale.Items.Count - 1)

            If Qs_Sa_Cod <> "0" Then
                Cmb_CentroAziendale.SelectedValue = Qs_Sa_Cod
            ElseIf Cmb_CentroAziendale.Items.Count = 2 Then
                Cmb_CentroAziendale.SelectedIndex = 1
            End If

            If Qs_Fabbricato_Cod <> "" Then
                clc.Fabbricati(Cmb_Magazzino,
                                  True, "Tutti i Magazzini", "0",
                                  Qs_Piva,
                                  CInt(Cmb_CentroAziendale.SelectedValue),
                                  0,
                                  FABBRICATI_NO_STALLE,
                                   False,
                                   "",
                                   " Fabbricati.Fabbricato_Des ",
                                  AGRODATAFINE,
                                   objParametri_Server)

                Me.Lbl_NumMagazzini.Text = CStr(Me.Cmb_Magazzino.Items.Count - 1)

                If Qs_Fabbricato_Cod <> "0" Then
                    Cmb_Magazzino.SelectedValue = Qs_Fabbricato_Cod
                ElseIf Cmb_Magazzino.Items.Count = 2 Then
                    Cmb_Magazzino.SelectedIndex = 1
                End If
            End If


        End If

        AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(Me.cmb_CatProdotto,
                                                        True, "", "",
                                                        0,
                                                        CAU_SCARICO,
                                                        0,
                                                        False,
                                                        False,
                                                            "",
                                                        "",
                                                        objParametri_Server)

        Me.cmb_CatProdotto.SelectedIndex = Me.cmb_CatProdotto.Items.IndexOf(Me.cmb_CatProdotto.Items.FindByValue(CStr(Qs_Elem_Cod)))

        Dim Filtro_Cat As String
        Filtro_Cat = " (Tabella IN ('Materie_Prime','TipologieSementi') ) " &
                        " AND  Elem_Cod NOT IN (" & CStr(FARMACI) & ", " & CStr(MANGIMI) & ")"

        AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(Me.ChkList_Categorie,
                                                                    False, "", "",
                                                                    0,
                                                                    CAU_SCARICO,
                                                                    0,
                                                                    False,
                                                                    False,
                                                                    Filtro_Cat, "",
                                                                    objParametri_Server)

        Dim i As Integer
        For i = 0 To Me.ChkList_Categorie.Items.Count - 1
            'seleziono tutto
            Me.ChkList_Categorie.Items(i).Selected = True
        Next

        ''--------------------------------------------------------------------

    End Sub

    ''###########################################################################
    Private Sub GestioneCheckComposizione()
    End Sub

    Private Sub Cambia_Impresa()

        Me.Cmb_CentroAziendale.Items.Clear()
        Me.Cmb_Magazzino.Items.Clear()
        Me.cmb_Prodotti.Items.Clear()

        If Me.Cmb_Impresa.SelectedValue <> "" Then

            Dim Piva As String

            Piva = Me.Cmb_Impresa.SelectedValue

            Session("PartitaIVA") = Piva
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Centri_Aziendali(Me.Cmb_CentroAziendale,
                                                        True, "", "",
                                                        Piva,
                                                         False,
                                                         2,
                                                         "", "",
                                                            objParametri_Server)

            'verifico omni, reparto confezionato

            Dim objOmniLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
            Dim sa_cod, fabbricato_cod As Integer

            objOmniLog.Recupera_Magazzino_Confezionato(Piva,
                                                        "",
                                                        objParametri_Server,
                                                        sa_cod,
                                                        fabbricato_cod)

            If sa_cod <> 0 And fabbricato_cod <> 0 Then
                'centro e magazzino passato
                Me.Cmb_CentroAziendale.SelectedIndex = Me.Cmb_CentroAziendale.Items.IndexOf(Me.Cmb_CentroAziendale.Items.FindByValue(CStr(sa_cod)))
                Cambia_CentroAziendale(fabbricato_cod)
            Else

                If Qs_Sa_Cod <> 0 And Qs_Fabbricato_Cod <> 0 Then
                    'centro e magazzino passato
                    Me.Cmb_CentroAziendale.SelectedIndex = Me.Cmb_CentroAziendale.Items.IndexOf(Me.Cmb_CentroAziendale.Items.FindByValue(CStr(Qs_Sa_Cod)))
                    Cambia_CentroAziendale(Qs_Fabbricato_Cod)
                Else
                    If Me.Cmb_CentroAziendale.SelectedIndex = 0 Then
                        If Me.Cmb_CentroAziendale.Items.Count > 1 Then
                            Me.Cmb_CentroAziendale.SelectedIndex = 1
                            Cambia_CentroAziendale(0)
                        Else
                            Me.Cmb_Magazzino.Items.Clear()
                        End If
                    End If
                End If
            End If

            Me.Lbl_NumCentri.Text = CStr(Me.Cmb_CentroAziendale.Items.Count - 1)

        End If


    End Sub

    '###########################################################################
    Private Sub Cambia_CentroAziendale(ByVal fabbricato_cod As Integer)

        Me.Cmb_Magazzino.Items.Clear()
        Me.cmb_Prodotti.Items.Clear()

        If Me.Cmb_CentroAziendale.SelectedValue <> "" Then

            Dim Piva As String
            Dim Sa_Cod As Integer

            Piva = Me.Cmb_Impresa.SelectedValue
            Sa_Cod = Me.Cmb_CentroAziendale.SelectedValue

            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Fabbricati(Me.Cmb_Magazzino,
                                            False, "", "",
                                            Piva,
                                            Sa_Cod,
                                            0,
                                            FABBRICATI_NO_STALLE,
                                             False,
                                             "",
                                             " Fabbricati.Fabbricato_Des ",
                                            AGRODATAFINE,
                                             objParametri_Server)


            If fabbricato_cod <> 0 Then
                Me.Cmb_Magazzino.SelectedIndex = Me.Cmb_Magazzino.Items.IndexOf(Me.Cmb_Magazzino.Items.FindByValue(CStr(fabbricato_cod)))
            Else
                If Me.Cmb_Magazzino.SelectedIndex = 0 Then
                    If Me.Cmb_Magazzino.Items.Count = 2 Then
                        Me.Cmb_Magazzino.SelectedIndex = 1
                    End If
                End If
            End If

            Cambia_Magazzino()

            Me.Lbl_NumMagazzini.Text = CStr(Me.Cmb_Magazzino.Items.Count)

        End If

    End Sub

    '###########################################################################
    Private Sub Cambia_Magazzino()

        Me.cmb_Prodotti.Items.Clear()

        If Me.Cmb_Magazzino.SelectedValue <> "" Then

            Dim RegioneCod As String = ""
            Dim Piva As String
            Dim Sa_Cod As Integer
            Piva = Me.Cmb_Impresa.SelectedValue
            Sa_Cod = Me.Cmb_CentroAziendale.SelectedValue
            Dim objF As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            objF.MagazzinoIndirizzo_Leggi_2(Piva, Sa_Cod, Cmb_Magazzino.SelectedValue,
                                                  Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing,
                                                  RegioneCod,
                                                  objParametri_Server)

            'Seleziono la regione di appartenenza dell'impresa
            Cmb_Regioni.SelectedIndex =
                Cmb_Regioni.Items.IndexOf(
                    Cmb_Regioni.Items.FindByValue(
                        RegioneCod))

            Cambia_CategoriaMagazzino()

        End If

    End Sub

    '###########################################################################
    Private Sub Cambia_CategoriaMagazzino()

        Me.cmb_Prodotti.Items.Clear()
        Me.Lbl_NumProdotti.Text = "0"

        GestioneCheckComposizione()

    End Sub

    '########################################################################################
    Private Sub ImgBtn_CercaImpresa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CercaImpresa.Click

        'Pulisco le combo 
        Me.Cmb_CentroAziendale.Items.Clear()
        Me.Cmb_Magazzino.Items.Clear()

        If Me.Txt_Impresa.Text = "" Then

            AgroMsgBox("Impostare un filtro!", Page)

        Else

            Carica_Imprese(Me.Txt_Impresa.Text)

        End If

    End Sub

    '########################################################################################
    Private Sub Carica_Imprese(ByVal TestoCercaRagSoc As String)

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0


        '----------------------------------------------------------------
        '--- Filtro personalizzato 
        '----------------------------------------------------------------
        strFiltro = " (Imprese.Rag_Soc like '%" & TestoCercaRagSoc & "%') "

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(Cmb_Impresa, False, "", "",
                                                         strFiltro, " ORDER BY Imprese.Rag_Soc asc",
                                                           objParametri_Server,
                                                           objParametri_Utenti)


        Me.Lbl_NumImprese.Text = CStr(Cmb_Impresa.Items.Count)

    End Sub


    '###########################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click


        Select Case hdTipo_Scheda.Value

            Case enum_CodificaStampe.RiepilogoProdottiUtilizzati
                If hds_sa_cod.Value = "" Then
                    hds_sa_cod.Value = "0"
                End If
                If hds_fabbricato_cod.Value = "" Then
                    hds_fabbricato_cod.Value = "0"
                End If
                If hds_piva.Value <> "" Then
                    Stampa()
                End If

            Case Else
                If hds_piva.Value <> "" AndAlso CInt(hds_sa_cod.Value) <> 0 AndAlso hds_fabbricato_cod.Value <> "" Then
                    Stampa()
                End If
        End Select

    End Sub


    '###########################################################################
    Private Sub Stampa()

        Dim TargetURL As String
        Dim Piva As String
        Dim Sa_Cod, Fabbricato_Cod As Integer
        Dim Data_Stampa, Data_Inizio, Data_Fine As Date
        Dim Elem_Cod As Integer = 0
        Dim Pro_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Tipo_Arrotondamento As Integer
        Dim Str_elem_cod As String = ""
        Dim AlmenoUno As Boolean = False
        Dim FlagVisualizzaComposizione As Integer = 0
        Dim CarichiScarichi As Integer = 0

        Piva = hds_piva.Value
        Sa_Cod = CInt(hds_sa_cod.Value)
        Fabbricato_Cod = CInt(hds_fabbricato_cod.Value)

        'Controllo che nella stampa dei movimenti, poiché le date sono editabili
        'la data inizio sia minore della data fine

        Select Case hdTipo_Scheda.Value

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze

                TargetURL = PaginaLinkStampaSchedaGiacenzeMagazzino

                Data_Stampa = Me.TxtStampa.Text

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                TargetURL = PaginaLinkStampaSchedaMovimentiMagazzino
                CarichiScarichi = caricoscarico.SelectedValue

                Data_Inizio = Me.TxtDataDa.Text
                Data_Fine = Me.TxtDataA.Text

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                If hdTipo_Scheda.Value = enum_CodificaStampe.SchedaMagazzinoFertilizzanti Then
                    TargetURL = PaginaLinkStampaSchedaFertilizzantiMagazzino
                Else
                    TargetURL = PaginaLinkStampaSchedaProdottiFitosanitariMagazzino
                End If

                Data_Inizio = Me.TxtDataDa.Text
                Data_Fine = Me.TxtDataA.Text

            Case enum_CodificaStampe.RiepilogoProdottiUtilizzati

                TargetURL = PaginaLinkStampaSchedaRiepilogoProdottiUtilizzati
                CarichiScarichi = rbl_caricoscarico2.SelectedValue
                Data_Inizio = Me.TxtDataDa.Text
                Data_Fine = Me.TxtDataA.Text

        End Select

        If hds_cat_prodotto.Value <> "" Then
            Elem_Cod = Me.cmb_CatProdotto.SelectedValue
        End If

        If hds_prodotto.Value <> "" Then
            If CInt(hds_prodotto.Value) > 0 Then
                Pro_Cod = CInt(hds_prodotto.Value)
                Mat_Cod = 0
            Else
                Pro_Cod = 0
                Mat_Cod = -CInt(hds_prodotto.Value)
            End If

        End If

        Tipo_Arrotondamento = Me.Rbl_Arrotondamento.SelectedValue

        Dim j As Integer
        For j = 0 To Me.ChkList_Categorie.Items.Count - 1
            If Me.ChkList_Categorie.Items(j).Selected = True Then
                Str_elem_cod &= CStr(Me.ChkList_Categorie.Items(j).Value) & ","
                AlmenoUno = True
            End If
        Next
        If AlmenoUno = False Then
        Else
            Str_elem_cod = Left(Str_elem_cod, Str_elem_cod.Length - 1)
            Str_elem_cod = "(" & Str_elem_cod & ")"
        End If

        If Me.Chk_Composizione.Visible = True Then
            If Me.Chk_Composizione.Checked = True Then
                FlagVisualizzaComposizione = 1
            Else
                FlagVisualizzaComposizione = 0
            End If
        Else
            FlagVisualizzaComposizione = 0
        End If

        ''================================================================

        Session("Regione_Selezionata") = ""
        If Me.Chk_LogoRegione.Checked = True AndAlso Me.Cmb_Regioni.SelectedIndex >= 0 Then
            Session("Regione_Selezionata") = Me.Cmb_Regioni.SelectedValue.ToString
        End If

        Dim queryString As String = "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder) &
                                    "&s=" & Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder) &
                                    "&f=" & Stringa_Codifica(Fabbricato_Cod, AgroKey_EncoderDecoder) &
                                    "&e=" & Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder) &
                                    "&pro=" & Stringa_Codifica(Pro_Cod, AgroKey_EncoderDecoder) &
                                    "&mat=" & Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder) &
                                    "&arr=" & Stringa_Codifica(Tipo_Arrotondamento, AgroKey_EncoderDecoder)

        Select Case hdTipo_Scheda.Value

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze

                queryString &= "&ds=" & Stringa_Codifica(Data_Stampa.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&fec=" & Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder) &
                               "&fvc=" & Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder) &
                               "&lot=" & Stringa_Codifica(Me.Txt_Lotto.Text, AgroKey_EncoderDecoder) &
                               "&sl=" & Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder) &
                               "&rag=" & Stringa_Codifica(Me.Rbl_Raggruppamento.SelectedValue, AgroKey_EncoderDecoder)

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                queryString &= "&di=" & Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&df=" & Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&ord=" & Stringa_Codifica(Me.Rbl_Ordinamento.SelectedValue, AgroKey_EncoderDecoder) &
                               "&fec=" & Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder) &
                               "&lot=" & Stringa_Codifica(Me.Txt_Lotto.Text, AgroKey_EncoderDecoder) &
                               "&sl=" & Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder) &
                               "&cs=" & Stringa_Codifica(CarichiScarichi, AgroKey_EncoderDecoder)

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                queryString &= "&di=" & Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&df=" & Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&fvc=" & Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder)

            Case enum_CodificaStampe.RiepilogoProdottiUtilizzati

                queryString &= "&di=" & Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&df=" & Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&fec=" & Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder) &
                               "&sl=" & Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder) &
                               "&cm=" & Stringa_Codifica(CarichiScarichi, AgroKey_EncoderDecoder) &
                               "&fvc=" & Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder)

        End Select

        Page_NewWindow_2010(Page,
                TargetURL, queryString, , , , , , , , , "MainContent", True)

    End Sub

    Private Sub ImgBtn_StampaExcel_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaExcel.Click
        StampaExcel()
    End Sub

    '###########################################################################
    Private Sub StampaExcel()

        Dim TargetURL As String
        Dim Piva As String
        Dim Sa_Cod As Integer = 0
        Dim Fabbricato_Cod As Integer = 0
        Dim Data_Stampa, Data_Inizio, Data_Fine As Date
        Dim Elem_Cod As Integer = 0
        Dim Pro_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Tipo_Arrotondamento As Integer
        Dim Str_elem_cod As String
        Dim AlmenoUno As Boolean = False

        TargetURL = PaginaLinkStampaSchedaMovimentiMagazzinoExcel

        Piva = hds_piva.Value

        Sa_Cod = hds_sa_cod.Value

        Fabbricato_Cod = hds_fabbricato_cod.Value

        Data_Inizio = CDate(TxtDataDa.Text)
        Data_Fine = CDate(TxtDataA.Text)

        If hds_cat_prodotto.Value <> "" Then
            Elem_Cod = hds_cat_prodotto.Value
        End If

        If hds_prodotto.Value <> "" Then
            If CInt(hds_prodotto.Value) > 0 Then
                Pro_Cod = CInt(hds_prodotto.Value)
                Mat_Cod = 0
            Else
                Pro_Cod = 0
                Mat_Cod = -CInt(hds_prodotto.Value)
            End If
        End If


        Tipo_Arrotondamento = Me.Rbl_Arrotondamento.SelectedValue

        Dim j As Integer
        For j = 0 To Me.ChkList_Categorie.Items.Count - 1
            If Me.ChkList_Categorie.Items(j).Selected = True Then
                Str_elem_cod &= CStr(Me.ChkList_Categorie.Items(j).Value) & ","
                AlmenoUno = True
            End If
        Next
        If AlmenoUno = False Then
            AgroMsgBox("E' necessario selezionare almeno una categoria di magazzino!", Page)
            Exit Sub
        Else
            Str_elem_cod = Left(Str_elem_cod, Str_elem_cod.Length - 1)
            Str_elem_cod = "(" & Str_elem_cod & ")"
        End If

        ''================================================================

        Session("Regione_Selezionata") = ""
        If Me.Chk_LogoRegione.Checked = True Then
            If Me.Cmb_Regioni.SelectedIndex >= 0 Then
                Session("Regione_Selezionata") = Me.Cmb_Regioni.SelectedValue.ToString
            End If
        End If

        Dim queryString As String = "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder) &
                                    "&s=" & Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder) &
                                    "&f=" & Stringa_Codifica(Fabbricato_Cod, AgroKey_EncoderDecoder) &
                                    "&e=" & Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder) &
                                    "&pro=" & Stringa_Codifica(Pro_Cod, AgroKey_EncoderDecoder) &
                                    "&mat=" & Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder) &
                                    "&arr=" & Stringa_Codifica(Tipo_Arrotondamento, AgroKey_EncoderDecoder)

        Select Case hdTipo_Scheda.Value

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                Session("BloccaOperazioni") = CB_BloccaOperazioni.Checked
                Dim caricaScarica As Integer = caricoscarico.SelectedValue

                queryString &= "&di=" & Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&df=" & Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder) &
                               "&ord=" & Stringa_Codifica(Me.Rbl_Ordinamento.SelectedValue, AgroKey_EncoderDecoder) &
                               "&fec=" & Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder) &
                               "&lot=" & Stringa_Codifica(Me.Txt_Lotto.Text, AgroKey_EncoderDecoder) &
                               "&sl=" & Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder) &
                               "&cs=" & Stringa_Codifica(caricaScarica, AgroKey_EncoderDecoder)

        End Select

        Dim StrWindowOpen As String

        StrWindowOpen = "<script language='javascript'>" &
                        vbNewLine &
                        "window.open('" &
                            TargetURL &
                            "" & queryString & "'," &
                            "'SchedeMagazzinoExcel'" &
                            ");" &
                        vbNewLine &
                        "</script>"

        Page.RegisterClientScriptBlock("key", StrWindowOpen)

    End Sub

End Class