Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ

Public Class FiltroZootecnia
    Inherits System.Web.UI.Page

#Region "filtro"

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

    'Dim Qs_Origine As String
    Dim Qs_Destinazione As String
    'Dim Qs_Chiave As String

    Const PaginaLinkStampaSintesiPartite As String = "SintesiPartite.aspx"
    Const PaginaLinkStampaDettaglioPartite As String = "DettaglioPartite.aspx"

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

    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_LottiWS(ByVal gruppo_date As String,
                                          ByVal piva As String,
                                          ByVal centro As String,
                                          ByVal stalla As String,
                                          ByVal Dettaglio As Boolean) As String()

        Dim r(1) As String

        Dim d As String() = gruppo_date.Split(New Char() {","c})

        Dim cmb_lot As New DropDownList

        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
        clc.Lotti_Zootecnia(cmb_lot,
                            CDate(If(d(0) <> "", d(0), AGRODATAINIZIO)),
                            CDate(If(d(1) <> "", d(1), AGRODATAFINE)),
                            piva,
                            centro,
                            stalla,
                            Dettaglio,
                            "",
                            "",
                            HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_lot.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_ListaBoviniWS() As String()

        Dim r(1) As String

        Dim cmb_raz As New DropDownList

        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
        clc.Lista_Razze_Bovini_Zootecnia(cmb_raz,
                            "",
                            "",
                            HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        rval &= "<option value="""">TUTTE</option>"
        For Each itm As ListItem In cmb_raz.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Sesso() As String()

        Dim r(1) As String

        Dim cmb_ses As New DropDownList

        Dim rval As String = ""
        rval &= "<option value="""">TUTTO</option>"
        rval &= "<option value=""M"">MASCHI</option>"
        rval &= "<option value=""F"">FEMMINE</option>"

        r(0) = rval

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Stato_Partite() As String()

        Dim r(1) As String

        Dim cmb_fsp As New DropDownList

        Dim rval As String = ""
        rval &= "<option value="""">SOLO CHIUSE</option>"
        rval &= "<option value=""0"">SOLO APERTE</option>"
        rval &= "<option value=""-1"">TUTTE</option>"

        r(0) = rval

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
        If testo <> "" Then
            strFiltro = " (Imprese.Rag_Soc like '%" & testo & "%') "
        End If
        'End If

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_imprese, False, "", "",
                                                         strFiltro, " ORDER BY Imprese.Rag_Soc asc",
                                                           HttpContext.Current.Session("ASG_objParametri_Server"),
                                                           HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim rval As String = ""
        rval &= "<option value="""""">SELEZIONA</option>"
        For Each itm As ListItem In cmb_imprese.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval
        r(1) = cmb_imprese.Items.Count

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Fornitore_FatturazioneWS(ByVal testo As String) As String()

        Dim r(1) As String

        Dim cmb_fornitori As New DropDownList

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0

        If testo <> "" Then
            strFiltro = " (Rag_soc like '%" & testo & "%' OR (cognome + ' ' + nome) like '%" & testo & "%') "
        End If

        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010

        clc.Fornitore_Zootecnia(cmb_fornitori, strFiltro, "",
                                HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        rval &= "<option value="""""">TUTTI</option>"
        For Each itm As ListItem In cmb_fornitori.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval
        r(1) = cmb_fornitori.Items.Count

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Fornitore_ProvenienzaWS(ByVal testo As String) As String()

        Dim r(1) As String

        Dim cmb_fornitori As New DropDownList

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0

        If testo <> "" Then
            strFiltro = " (Rag_soc like '%" & testo & "%' OR (cognome + ' ' + nome) like '%" & testo & "%') "
        End If

        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010

        clc.Fornitore_Zootecnia(cmb_fornitori, strFiltro, "",
                                HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        rval &= "<option value="""""">TUTTI</option>"
        For Each itm As ListItem In cmb_fornitori.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval
        r(1) = cmb_fornitori.Items.Count

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Centri(ByVal Piva As String) As String()

        Dim r(1) As String

        Dim cmb_centri As New DropDownList

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0

        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010

        clc.Centri_Zoo_Per_Azienda(cmb_centri, Piva, "", "",
                                HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        rval &= "<option value="""""">TUTTI</option>"
        For Each itm As ListItem In cmb_centri.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval
        r(1) = cmb_centri.Items.Count

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Stalle(ByVal Piva As String, ByVal Sa_Cod As String) As String()

        Dim r(1) As String

        Dim cmb_stalle As New DropDownList

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0
        Dim Sa_Cod_Num As Integer = If(Sa_Cod <> "", CInt(Sa_Cod), 0)

        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010

        clc.Stalle_Per_AziendaCentro(cmb_stalle, Piva, Sa_Cod_Num, "", "",
                                HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        rval &= "<option value="""""">TUTTE</option>"
        For Each itm As ListItem In cmb_stalle.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        r(0) = rval
        r(1) = cmb_stalle.Items.Count

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
        '======
        ''########################################################################
        ''#####  Verifica se la pagina e' stata caricata per la prima volta  #####
        ''########################################################################

        If Not Page.IsPostBack Then

            'Le date le metto in sessione solo se non è PostBack, perché le ho già salvate in sessione lato client prima dei tasti stampa/stampa excel

            'ReportSelezionato rimane gestito anche in sessione per non creare casini se serve nelle pagine successive
            If HttpContext.Current.Session("ReportSelezionato") = 0 Then
                hdTipo_Scheda.Value = 11
            Else
                hdTipo_Scheda.Value = HttpContext.Current.Session("ReportSelezionato")
            End If
            hdReportSelezionato.Value = hdTipo_Scheda.Value

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean
            UtenteAbilitato = objUtenti.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                  Session("ASG_IdServizio"),
                                                                  enum_Security_Attivita.Report_Zootecnia,
                                                                  enum_Security_Operazione.Lettura,
                                                                  Now,
                                                                  "",
                                                                  objParametri_Utenti)

            If Not UtenteAbilitato Then

                Response.Redirect("~/Menu/MenuBS_2017.aspx")

            End If

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

        '-------------------------------------------------------------------
        '----- Logo Regionale

        ' Per default viene stampato il logo della regione



        If Qs_Piva <> "" Then

            'chiamo CaricaCombo_Imprese perché devo caricare solo l'impresa che ha quella piva
            '(per caricare tutte le imprese uso CaricaCombo_ImpreseUtente_Optimize)

        End If

        Dim Filtro_Cat As String
        Filtro_Cat = " (Tabella IN ('Materie_Prime','TipologieSementi') ) " &
                        " AND  Elem_Cod NOT IN (" & CStr(FARMACI) & ", " & CStr(MANGIMI) & ")"

        Dim i As Integer

        ''--------------------------------------------------------------------

    End Sub


    '###########################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        Stampa()

    End Sub


    '###########################################################################
    Private Sub Stampa()

        Dim TargetURL As String
        Dim Data_Inizio, Data_Fine As Date
        Dim impresa As String
        Dim stalla As String
        Dim Centro As String
        Dim lotto As String
        Dim lotto2 As String
        Dim Sesso As String
        Dim stato_partite As String
        Dim Razza As String
        Dim FornitoreFatturazione As String
        Dim FornitoreProvenienza As String

        Data_Inizio = If(hdDataDa.Value = "", AGRODATAINIZIO, hdDataDa.Value)
        Data_Fine = If(hdDataA.Value = "", AGRODATAFINE, hdDataA.Value)

        impresa = hdImpresaSelezionata.Value
        Centro = hdCentroSelezionato.Value
        stalla = hdStallaSelezionata.Value
        lotto = hdLottoSelezionato.Value
        lotto2 = hdLotto2Selezionato.Value
        Sesso = hdSessoSelezionato.Value
        Razza = hdRazzaSelezionato.Value
        FornitoreFatturazione = hdFornitoreFatturazioneSelezionato.Value
        FornitoreProvenienza = hdFornitoreProvenienzaSelezionato.Value
        stato_partite = hdStatoPartite.Value

        TargetURL = If(hdReportSelezionato.Value = 260, PaginaLinkStampaDettaglioPartite, PaginaLinkStampaSintesiPartite)

        Dim queryString As String = "?di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder) &
                                    "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder) &
                                    "&p=" & Stringa_Codifica(impresa, AgroKey_EncoderDecoder) &
                                    "&cen=" & Stringa_Codifica(Centro, AgroKey_EncoderDecoder) &
                                    "&sta=" & Stringa_Codifica(stalla, AgroKey_EncoderDecoder) &
                                    "&ses=" & Stringa_Codifica(Sesso, AgroKey_EncoderDecoder) &
                                    "&raz=" & Stringa_Codifica(Razza, AgroKey_EncoderDecoder) &
                                    "&ff=" & Stringa_Codifica(FornitoreFatturazione, AgroKey_EncoderDecoder) &
                                    "&fp=" & Stringa_Codifica(FornitoreProvenienza, AgroKey_EncoderDecoder)

        If hdReportSelezionato.Value = 260 Then
            queryString &= "&lot=" & Stringa_Codifica(lotto, AgroKey_EncoderDecoder) &
                           "&lot2=" & Stringa_Codifica(lotto2, AgroKey_EncoderDecoder)
        Else
            queryString &= "&sp=" & Stringa_Codifica(stato_partite, AgroKey_EncoderDecoder)
        End If

        'hdImpresaSelezionata.Value = ""
        'hdCentroSelezionato.Value = ""
        'hdStallaSelezionata.Value = ""
        'hdLottoSelezionato.Value = ""
        'hdLotto2Selezionato.Value = ""
        'hdSessoSelezionato.Value = ""
        'hdRazzaSelezionato.Value = ""
        'hdFornitoreFatturazioneSelezionato.Value = ""
        'hdFornitoreProvenienzaSelezionato.Value = ""

        Page_NewWindow_2010(Page,
                TargetURL, queryString, , , , , , , , , "MainContent", True)

    End Sub

    Private Sub ImgBtn_StampaExcel_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaExcel.Click
        StampaExcel()
    End Sub

    '###########################################################################
    Private Sub StampaExcel()

    End Sub

End Class