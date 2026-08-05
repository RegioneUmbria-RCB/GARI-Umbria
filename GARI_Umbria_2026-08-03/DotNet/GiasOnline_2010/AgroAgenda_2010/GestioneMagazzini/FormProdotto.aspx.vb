Imports System.Drawing
Imports System.Web
Imports System.Xml
Imports AgroAgenda_2010.Resources
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL

Public Class FormProdotto
    Inherits System.Web.UI.Page



    Dim objParametriAgenda As ParametriAgenda_2010



#Region " FormProdotto "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    'Protected WithEvents Txt_RagioneSociale As System.Web.UI.WebControls.TextBox
    'Protected WithEvents Txt_CentroAziendale As System.Web.UI.WebControls.TextBox
    Protected WithEvents Lbl_SalvaEsci As System.Web.UI.WebControls.Label
    Protected WithEvents IMAGEBUTTON2 As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LABEL33 As System.Web.UI.WebControls.Label
    Protected WithEvents PANEL2 As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL32 As System.Web.UI.WebControls.Label
    Protected WithEvents IMAGEBUTTON1 As System.Web.UI.WebControls.ImageButton
    Protected WithEvents PANEL1 As System.Web.UI.WebControls.Panel
    Protected WithEvents lbl_InsDocumento As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtnInsDocumento As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_InsDocumento As System.Web.UI.WebControls.Panel
    Protected WithEvents Lbl_SalvaMagazzino As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtnSalvaTutto As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Salvataggio As System.Web.UI.WebControls.Panel
    Protected WithEvents ImgBtn_Inserisci_nel_DataGrid As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Lbl_Inserisci_Carico As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_InsGriglia As System.Web.UI.WebControls.Panel
    Protected WithEvents DataGrid_Prodotti As System.Web.UI.WebControls.DataGrid
    Protected WithEvents Lbl_PannelloCarichi As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Prodotti As System.Web.UI.WebControls.Panel
    Protected WithEvents ImgIcona As System.Web.UI.WebControls.Image
    'Protected WithEvents LblTitolo As System.Web.UI.WebControls.Label
    Protected WithEvents ImageLogo As System.Web.UI.WebControls.Image
    Protected WithEvents LblCancellazione As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtnCancella As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Cancellazione As System.Web.UI.WebControls.Panel
    Protected WithEvents ImgBtnAnnulla As System.Web.UI.WebControls.ImageButton
    Protected WithEvents ImgBtn_Help As System.Web.UI.WebControls.ImageButton
    Protected WithEvents lbl_dettaglio As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_prezzo As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_euro As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_PrezzoUnitario As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Quantita As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_PrezzoUnitario_Netto As System.Web.UI.WebControls.TextBox
    Protected WithEvents lbl_qta As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL6 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Imponibile As System.Web.UI.WebControls.TextBox
    Protected WithEvents Opt_Imponibile As System.Web.UI.WebControls.RadioButton
    Protected WithEvents Opt_Importo As System.Web.UI.WebControls.RadioButton
    Protected WithEvents LABEL5 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Importo As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Imponibile_Netto As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label7 As System.Web.UI.WebControls.Label
    Protected WithEvents Label9 As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_sconto As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_ScontoPercentuale As System.Web.UI.WebControls.TextBox
    Protected WithEvents lbl_magg As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_MaggiorazionePercentuale As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL12 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_IVA As System.Web.UI.WebControls.TextBox
    Protected WithEvents Cmb_IVA As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Label15 As System.Web.UI.WebControls.Label
    Protected WithEvents Label16 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL18 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_AnnoContabile As System.Web.UI.WebControls.DropDownList
    Protected WithEvents LABEL19 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Conti As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Chk_Conti As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Txt_Sconto_Calcolato As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Maggiorazione_Calcolato As System.Web.UI.WebControls.TextBox
    Protected WithEvents Cmb_ContiContatto As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Btn_CalcolaImporti As System.Web.UI.WebControls.Button
    Protected WithEvents Label23 As System.Web.UI.WebControls.Label
    Protected WithEvents Opt_Sconto As System.Web.UI.WebControls.RadioButton
    Protected WithEvents Opt_Maggiorazione As System.Web.UI.WebControls.RadioButton
    Protected WithEvents Label26 As System.Web.UI.WebControls.Label
    Protected WithEvents Label27 As System.Web.UI.WebControls.Label
    Protected WithEvents Label28 As System.Web.UI.WebControls.Label
    Protected WithEvents Label11 As System.Web.UI.WebControls.Label
    Protected WithEvents Chk_IVAmanuale As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Txt_IVAmanuale As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label17 As System.Web.UI.WebControls.Label
    Protected WithEvents Label24 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL29 As System.Web.UI.WebControls.Label
    Protected WithEvents Label31 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Economico As System.Web.UI.WebControls.Panel
    Protected WithEvents Txt_Qta_Extra As System.Web.UI.WebControls.TextBox
    Protected WithEvents Opt_PrezzoUnitario As System.Web.UI.WebControls.RadioButton
    Protected WithEvents lbl_PrezzoNetto As System.Web.UI.WebControls.Label
    Protected WithEvents Label30 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Dettaglio As System.Web.UI.WebControls.Panel
    Protected WithEvents cmb_Prodotti As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_descrizione As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_categoria As System.Web.UI.WebControls.Label
    Protected WithEvents cmb_Lotto As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_LottoInterno As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_calibri As System.Web.UI.WebControls.Label
    Protected WithEvents cmb_Calibro As System.Web.UI.WebControls.DropDownList
    Protected WithEvents cmb_Udm As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_udm As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_LottoAccettazione As System.Web.UI.WebControls.Label
    Protected WithEvents cmb_LottoAccettazione As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Txt_CercaProdotto As System.Web.UI.WebControls.TextBox
    Protected WithEvents ImgBtn_CercaProdotti As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Lbl_Cerca As System.Web.UI.WebControls.Label
    Protected WithEvents cmb_Categoria As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Lbl_Risorse As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_BeniStrumentali As System.Web.UI.WebControls.TextBox
    Protected WithEvents TxtProdotto As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_NumProdotti As System.Web.UI.WebControls.Label
    Protected WithEvents Lbl_Ordine As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Ordini As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Chk_Contatto As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Txt_CercaLotto As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_LottoAccettazione As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Prodotto As System.Web.UI.WebControls.Panel
    Protected WithEvents Cmb_Destinazione As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_MagProvenienza As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Provenienza As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_MagDestinazione As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Giacenza_Provenienza As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Giacenza_Destinazione As System.Web.UI.WebControls.TextBox
    Protected WithEvents Chk_MovimentoMag As System.Web.UI.WebControls.CheckBox
    Protected WithEvents lbl_giacenza_mag As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Magazzini As System.Web.UI.WebControls.Panel
    Protected WithEvents lbl_prodotto As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_magazzini As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL1 As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_causale As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_data As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DataMovimento As System.Web.UI.WebControls.TextBox
    Protected WithEvents cmb_Causale As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_ora As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Ora As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_PendenzaIniziale As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Movimento As System.Web.UI.WebControls.Panel
    Protected WithEvents Cmb_IndirizzoProduttivo As System.Web.UI.WebControls.DropDownList
    Protected WithEvents LABEL14 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL13 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Animali As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Lbl_AnagrafeAnimale As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_AnagrafeAnimale As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL21 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL22 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Matricola As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Cmb_Razza As System.Web.UI.WebControls.DropDownList
    Protected WithEvents DataGrid_ZooAnimali As System.Web.UI.WebControls.DataGrid
    Protected WithEvents Pannello_ZooAnimali As System.Web.UI.WebControls.Panel
    Protected WithEvents Label20 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Animale As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_Generale As System.Web.UI.WebControls.Panel
    Protected WithEvents Label2 As System.Web.UI.WebControls.Label
    Protected WithEvents Label3 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DaInviare As System.Web.UI.WebControls.TextBox
    Protected WithEvents SalvaEsci As System.Web.UI.HtmlControls.HtmlImage

    Protected WithEvents Risorse As System.Web.UI.HtmlControls.HtmlImage
    Protected WithEvents AnagrafeAnimale As System.Web.UI.HtmlControls.HtmlImage
    Protected WithEvents Aggiunto_Animale As System.Web.UI.HtmlControls.HtmlInputHidden
    Protected WithEvents IndirizzoProfitosan As System.Web.UI.HtmlControls.HtmlInputHidden
    Protected WithEvents Txt_DoseEtichetta As System.Web.UI.WebControls.TextBox
    Protected WithEvents Lbl_DoseEtichetta As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_CercaCodArticolo As System.Web.UI.WebControls.TextBox
    Protected WithEvents lbl_CercaProdotto As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_CercaLotto As System.Web.UI.WebControls.Label
    Protected WithEvents Lbl_CercaCodArticolo As System.Web.UI.WebControls.Label
    Protected WithEvents Modifica_Data As System.Web.UI.HtmlControls.HtmlInputHidden
    Protected WithEvents SI_NO As System.Web.UI.HtmlControls.HtmlInputHidden
    Protected WithEvents lbl_giacenza1_mag As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_giacenza2_mag As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Giacenza_Provenienza_Tot_old As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Giacenza_Destinazione_Tot_old As System.Web.UI.WebControls.TextBox
    Protected WithEvents lbl_giacenza_centro_old As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL4 As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_NumeroDenuncia As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumeroDenuncia As System.Web.UI.WebControls.TextBox
    Protected WithEvents lbl_DataDenuncia As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DataDenuncia As System.Web.UI.WebControls.TextBox
    Protected WithEvents denuncia As System.Web.UI.WebControls.Panel

    Public Master_Operazione As Agenda

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()


        Master_Operazione = CType(Page.Master, Agenda)
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto


    End Sub

#End Region

    Const COL_P_ELEM_COD As Integer = 0
    Const COL_P_PRO_COD As Integer = 1
    Const COL_P_MAT_COD As Integer = 2
    Const COL_P_CAL_COD As Integer = 3
    Const COL_P_COD_PROGETTO As Integer = 4
    Const COL_P_LOTTO As Integer = 5
    Const COL_P_CODICE As Integer = 6
    Const COL_P_PIVA_DEST As Integer = 7
    Const COL_P_SACOD_DEST As Integer = 8
    Const COL_P_ID_DEST As Integer = 9
    Const COL_P_DESTINAZIONE As Integer = 10
    Const COL_P_PIVA_PROV As Integer = 11
    Const COL_P_SACOD_PROV As Integer = 12
    Const COL_P_ID_PROV As Integer = 13
    Const COL_P_PROVENIENZA As Integer = 14
    Const COL_P_DESCR As Integer = 15
    Const COL_P_UDM_COD As Integer = 16
    Const COL_P_UDM_DES As Integer = 17
    Const COL_P_QTA As Integer = 18
    Const COL_P_PREZZO_UNIT As Integer = 19
    Const COL_P_CHIAVE As Integer = 20
    Const COL_P_FASE_COD As Integer = 21
    Const COL_P_PRODOTTO As Integer = 22
    Const COL_P_UDM_COD_EXTRA As Integer = 23
    Const COL_P_QTA_EXTRA As Integer = 24
    Const COL_P_PREZZO_UNIT_NETTO As Integer = 25
    Const COL_P_COD_VARIAZIONE As Integer = 26
    Const COL_P_VARIAZIONE_PERC As Integer = 27
    Const COL_P_VARIAZIONE As Integer = 28
    Const COL_P_CODICE_IVA As Integer = 29
    Const COL_P_ALIQUOTA As Integer = 30
    Const COL_P_IVA As Integer = 31
    Const COL_P_IMPONIBILE As Integer = 32
    Const COL_P_IMPONIBILE_NETTO As Integer = 33
    Const COL_P_PREZZO_EFFETTIVO As Integer = 34
    Const COL_P_ANNO As Integer = 35
    Const COL_P_RIC_COD As Integer = 36
    Const COL_P_COD_CONTO As Integer = 37
    Const COL_P_CONTO As Integer = 38
    Const COL_P_ELIMINA As Integer = 39

    Const str_COL_PIVA_DEST As String = "Piva_Destinazione"
    Const str_COL_SACOD_DEST As String = "SaCod_Destinazione"
    Const str_COL_ID_DEST As String = "Id_Destinazione"
    Const str_COL_DESTINAZIONE As String = "Destinazione"

    Const str_COL_PIVA_PROV As String = "Piva_Provenienza"
    Const str_COL_SACOD_PROV As String = "SaCod_Provenienza"
    Const str_COL_ID_PROV As String = "Id_Provenienza"
    Const str_COL_PROVENIENZA As String = "Provenienza"

    Const SEP_PuaReg = "/"

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- Gestione Querystring
    Dim Qs_Key As String
    Dim Qs_Operazione As String
    Dim Qs_CaricoScarico As String
    Dim Qs_Lav_Cod As Integer
    Dim Qs_ElemCod As Integer
    Dim Qs_IdAgenda As Integer
    Dim Qs_DataSelezionata As String
    Dim Qs_OraSelezionata As String
    Dim Qs_PagRitorno As String
    Dim Qs_Mode As String
    Dim Qs_Tipo As String
    Dim Qs_CodContatto As String
    Dim Qs_RagSocContatto As String
    Dim Qs_SaCod As Integer
    Dim Qs_Pua_Regolamento_Cod As String
    Dim Qs_Servizio_Cod As Integer

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""

    Dim xChiave As String

    Dim xCaricoScarico As Integer

    Dim xPiva As String
    Dim xSa_Cod As Integer
    Dim xFabbricato_Cod As Integer
    Dim xAgenda As String

    Dim xElem_Cod As Integer
    Dim xMat_Cod As Integer
    Dim xCod_Progetto As Integer
    Dim xLotto As String
    Dim xCod_Calibro As Integer
    Dim xUdm_Cod As Integer
    Dim xFase_Cod As Integer

    Dim xFlagDocLight As Boolean

    Dim Qta_Modifica_Giacenza As Decimal = 0

    Dim objParametri_Utenti As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri





    '####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
        'IMPOSTA IL NUMERO DI MINUTI DOPO I QUALI
        'LA PAGINA MEMORIZZATA NELLA CACHE SCADE

        Response.Expires = 0

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        'Se ci sono molte giacenze la pagina va in timeout ...
        'Allungo il timeout dai 180 secondi di default (3 minuti) a 900 secondi (15 minuti)
        Server.ScriptTimeout = 900

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        Try


            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            '----- Verifico che l'utente sia autenticato

            If Session("ASG_Utente_Username") = "" Then
                Response.Redirect("~/Custom500.aspx")
            End If


            '##############################################################
            '###################  objParametri_Server  ####################
            '##############################################################
            objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
            objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            '---

            inzializzadate()
            '##############################################################
            '###################  QUERY STRING  ###########################
            '##############################################################

            Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString, AgroKey_EncoderDecoder, Server)

            'Qs_Origine = Stringa_Decodifica(Request.QueryString("orig").ToString, _
            '                 AgroKey_EncoderDecoder, _
            '                 Server)

            'pagina chiamante --> per gestire il tipo di uscita dalla pagina:
            'fare un redirect o chiuderla (perché aperta in modal dialog)
            'usa enum_PagineGiasOnline
            If Not IsNothing(Request.QueryString("orig")) Then

                Qs_PagRitorno = Stringa_Decodifica(Request.QueryString("orig").ToString, AgroKey_EncoderDecoder, Server)

            Else
                Qs_PagRitorno = CStr(0)
            End If

            Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString, AgroKey_EncoderDecoder, Server)

            Qs_CaricoScarico = Stringa_Decodifica(Request.QueryString("c").ToString,
                                           AgroKey_EncoderDecoder, Server)

            Qs_Key = Stringa_Decodifica(Request.QueryString("k").ToString,
                                        AgroKey_EncoderDecoder, Server)

            Qs_Mode = Stringa_Decodifica(Request.QueryString("mode").ToString,
                                AgroKey_EncoderDecoder, Server)

            If InStr(Request.QueryString.ToString, "&tipo=") <> 0 Then
                Qs_Tipo = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                              AgroKey_EncoderDecoder, Server)
            Else
                Qs_Tipo = CAU_MAGAZZINO
            End If

            'leggo il lav_cod e l'elem_cod solo se sono presenti nella 
            'querystring, cioè se arrivo qui dalla semina o da altre operazioni di agenda
            If InStr(Request.QueryString.ToString, "&l=") <> 0 Then

                Qs_Lav_Cod = Stringa_Decodifica(Request.QueryString("l").ToString,
                                       AgroKey_EncoderDecoder, Server)
            Else
                Qs_Lav_Cod = 0
            End If

            If Not IsNothing(Request.QueryString("light")) Then
                xFlagDocLight = Stringa_Decodifica(Request.QueryString("light").ToString,
                                                    AgroKey_EncoderDecoder, Server)
            Else
                xFlagDocLight = False
            End If
            If xFlagDocLight Then
                If Not IsNothing(Request.QueryString("lcl")) Then
                    'devo sovrascrivere il lav_cod prendendo quello originale
                    'sopra, su Qs_Lav_Cod, c'è quello del carico di magazzino che gli è stato messo per chiamare la funzione che restituisce il targeturl della formprodotto
                    Qs_Lav_Cod = Stringa_Decodifica(Request.QueryString("lcl").ToString,
                                                        AgroKey_EncoderDecoder, Server)
                Else
                    'i18n
                    'non deve succedere
                    Throw New Exception("Non è arrivato il lavcod corretto per la modalità light del documento di carico.")
                End If
                If Not IsNothing(Request.QueryString("mol")) Then
                    'devo sovrascrivere il 'mode' prendendo quello originale
                    'sopra, su Qs_Mode, c'è quello del carico di magazzino che gli è stato messo per chiamare la funzione che restituisce il targeturl della formprodotto
                    Qs_Mode = Stringa_Decodifica(Request.QueryString("mol").ToString,
                                                        AgroKey_EncoderDecoder, Server)
                Else
                    'i18n
                    'non deve succedere
                    Throw New Exception("Non è arrivato il mode corretto per la modalità light del documento di carico.")
                End If
            End If

            If Not IsNothing(Request.QueryString("e")) Then
                Qs_ElemCod = Stringa_Decodifica(Request.QueryString("e").ToString,
                                                    AgroKey_EncoderDecoder, Server)
            Else
                Qs_ElemCod = 0
            End If

            If InStr(Request.QueryString.ToString, "&d=") <> 0 Then

                Qs_DataSelezionata = Stringa_Decodifica(Request.QueryString("d").ToString,
                                       AgroKey_EncoderDecoder, Server)

                '01/03/2019: a volte capita che il menù agenda mandi data e ora
                Qs_DataSelezionata = CDate(Qs_DataSelezionata).ToShortDateString
            Else
                Qs_DataSelezionata = CStr(Date.Today)
            End If

            'If InStr(Request.QueryString.ToString, "&ora=") <> 0 Then

            '    Qs_OraSelezionata = Format(CDate(Stringa_Decodifica(Request.QueryString("ora").ToString, _
            '                           AgroKey_EncoderDecoder, _
            '                           Server)), "HH" & gettimesep() & "mm")
            'Else
            '    Qs_OraSelezionata = "12" & gettimesep() & "00"
            'End If

            Qs_OraSelezionata = "12" & gettimesep() & "00"

            If InStr(Request.QueryString.ToString, "&a=") <> 0 Then

                Qs_IdAgenda = Stringa_Decodifica(Request.QueryString("a").ToString,
                                       AgroKey_EncoderDecoder, Server)
            Else
                Qs_IdAgenda = 0
            End If

            If InStr(Request.QueryString.ToString, "&s=") <> 0 Then

                Qs_SaCod = Stringa_Decodifica(Request.QueryString("s").ToString,
                                       AgroKey_EncoderDecoder, Server)
            Else
                Qs_SaCod = 0
            End If

            If InStr(Request.QueryString.ToString, "&codcont=") <> 0 Then

                Qs_CodContatto = Stringa_Decodifica(Request.QueryString("codcont").ToString,
                                       AgroKey_EncoderDecoder, Server)
            Else
                Qs_CodContatto = ""
            End If

            If InStr(Request.QueryString.ToString, "&ragcont=") <> 0 Then

                Qs_RagSocContatto = Stringa_Decodifica(Request.QueryString("ragcont").ToString,
                                       AgroKey_EncoderDecoder, Server)
            Else
                Qs_RagSocContatto = ""
            End If

            If InStr(Request.QueryString.ToString, "&pr=") <> 0 Then

                Qs_Pua_Regolamento_Cod = Stringa_Decodifica(Request.QueryString("pr").ToString,
                                       AgroKey_EncoderDecoder, Server)
            Else
                Qs_Pua_Regolamento_Cod = ""
            End If

            If InStr(Request.QueryString.ToString, "&sc=") <> 0 Then

                Qs_Servizio_Cod = Stringa_Decodifica(Request.QueryString("sc").ToString,
                                       AgroKey_EncoderDecoder, Server)
            Else
                Qs_Servizio_Cod = enum_Servizi.Quaderno_Campagna_Caa
            End If


            '##############################################################

            EseguitaOperazione = False
            PremutoAnnulla = False

            Dim AggiuntoAnimale As Boolean = False
            Dim xCampo_Cod As Integer
            Dim xAppezza As Integer
            Dim xID_Imp As Integer
            Dim xCodFiscale As String = ""
            Dim xTipoNodo As enum_TipoNodo

            'Recupero Chiave ed Operazione dalla querystring
            xChiave = Qs_Key
            xAgenda = Qs_IdAgenda
            Operazione = Qs_Operazione

            Call AgronicaCoreDataProvider.Albero.ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
                                xChiave,
                                xTipoNodo,
                                xPiva,
                                xSa_Cod,
                                xCampo_Cod,
                                xAppezza,
                                xID_Imp,
                                xCodFiscale,
                                xFabbricato_Cod)


            If Qs_SaCod <> 0 Then
                xSa_Cod = Qs_SaCod
            End If

            'assegno il valore alla variabile xCaricoScarico di tipo integer
            If Qs_CaricoScarico = "C" Then
                xCaricoScarico = enum_Agenda_Causali.CARICO
            ElseIf Qs_CaricoScarico = "S" Then
                xCaricoScarico = enum_Agenda_Causali.SCARICO
            ElseIf Qs_CaricoScarico = "T" Then
                xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO
            End If



            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################

            If Not Page.IsPostBack Then
                'output.Write("Page has just been loaded")

                ViewState("Piva") = xPiva
                ViewState("Sa_Cod") = xSa_Cod
                ViewState("Fabbricato_Cod") = xFabbricato_Cod
                ViewState("id_agenda") = xAgenda

            Else
                'output.Write("Postback has occured")

                xPiva = ViewState("Piva")
                xSa_Cod = ViewState("Sa_Cod")
                xFabbricato_Cod = ViewState("Fabbricato_Cod")
                xAgenda = ViewState("id_agenda")

                If Aggiunto_Animale.Value = "1" Then
                    Aggiungi_Matricola()
                    Aggiunto_Animale.Value = "0"
                End If

                If Modifica_Data.Value = "1" Then
                    Modifica_Data.Value = ""
                    'aggiorno
                    Carica_Dopo_Udm()
                End If



                '//////////////////////////////////////////////////////
                '            RITORNO DALLA EDIT CONTATTO
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                'ritorno dalla FormProdotto: devo inserire i prodotti nel dettaglio
                Select Case Me.InsFornitore.Value

                    Case "0", "", "undefined" 'NO

                        '23/05/2019: ad ogni postback la combo perde la classe
                        'gliela rimettiamo (si lo sappiamo che fa c....e)
                        Dim strJs As String = "$('.myCombo').combobox3();"
                        If upDati Is Nothing Then
                            Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(strJs))
                        Else
                            ScriptManager.RegisterClientScriptBlock(upDati, upDati.GetType(),
                                                         String.Format("jQuery_{0}", "openmodal"), strJs, True)
                        End If

                    Case Else 'SI

                        'ricarica il menù dei fornitori
                        AgronicaCoreUtility.CaricaListControl.Contatti(Me.Cmb_Fornitore,
                                    True,
                                    DirectCast(GetLocalResourceObject("SelezionaUnFornitoreTreCaratteri"), String),
                                    0,
                                    xPiva,
                                    "",
                                    0,
                                    0,
                                    True,
                                    False,
                                    0,
                                   0,
                                    False,
                                    0,
                                    ID_CF_NOFILTRO,
                                    " Rapporti_Contabili.Fornitore = 1 ",
                                    "",
                                    objParametri_Server,
                                    1)

                        'Me.InsFornitore.Value contiene il cod_contatto
                        Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                        Dim cod_risum As Integer
                        cod_risum = objRisUm.CodRisUm_by_PivaCodContattoCodRapporto2(xPiva,
                                                                                 Me.InsFornitore.Value,
                                                                                 COD_FORNITORE,
                                                                                 objParametri_Server)

                        Me.Cmb_Fornitore.SelectedIndex = Me.Cmb_Fornitore.Items.IndexOf(Me.Cmb_Fornitore.Items.FindByValue(cod_risum))

                        Dim strJs As String = "$('.myCombo').combobox3();"

                        If upDati Is Nothing Then
                            Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(strJs))
                        Else
                            ScriptManager.RegisterClientScriptBlock(upDati, upDati.GetType(),
                                                         String.Format("jQuery_{0}", "openmodal"), strJs, True)

                        End If

                End Select

                'If AggiungiInneschi_SI_NO.Value = "0" Then
                'Else
                'End If

                Exit Sub

            End If

            'Costruisco il link
            Me.TxtProdotto.Text = "e=" &
                      Stringa_Codifica(Qs_ElemCod, AgroKey_EncoderDecoder, Server) &
                      "&l=" &
                      Stringa_Codifica(Qs_Lav_Cod, AgroKey_EncoderDecoder, Server) &
                      "&k=" &
                      Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                      "&o=" &
                      Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                      "&d=" &
                      Stringa_Codifica(Qs_DataSelezionata, AgroKey_EncoderDecoder, Server) &
                      "&c=" &
                      Stringa_Codifica("C", AgroKey_EncoderDecoder, Server) &
                      "&a=" &
                      Stringa_Codifica(Qs_IdAgenda, AgroKey_EncoderDecoder, Server) &
                      "&r=" &
                      Stringa_Codifica("FormProdotto.aspx", AgroKey_EncoderDecoder, Server) &
                      "&orig=" &
                      Stringa_Codifica("FormProdotto.aspx", AgroKey_EncoderDecoder, Server) &
                      "&mode=" & Request.QueryString("mode").ToString

            'il link al contatto è lato server, non client (altrimenti non si riusciva a ricaricare la cmb dei fornitori
            'Me.TxtContatto.Text = "o=" &
            '          Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
            '            "&piva=" &
            '          Stringa_Codifica(xPiva, AgroKey_EncoderDecoder, Server) &
            '               "&tipo_rapporto=" &
            '          Stringa_Codifica(COD_FORNITORE, AgroKey_EncoderDecoder, Server) &
            '            "&lav_cod=" &
            '          Stringa_Codifica(Qs_Lav_Cod, AgroKey_EncoderDecoder, Server) &
            '            "&orig=" &
            '          Stringa_Codifica(enum_PagineAgenda_2010.FormProdotto, AgroKey_EncoderDecoder, Server) &
            '            "&codcont=" &
            '          Stringa_Codifica("", AgroKey_EncoderDecoder, Server)


            '##############################################################
            '#####################  PERMESSI  #############################
            '##############################################################

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            ' Dim strDummy As String
            Dim UtenteAbilitato As Boolean
            Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Select Case CInt(Qs_Operazione)

                Case enum_TipoOperazioneDB.Lettura

                    UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
                                          Session("ASG_Utente_Username"),
                                          Session("ASG_IdServizio"),
                                          enum_Security_Attivita.Gest_Magazzino,
                                          enum_Security_Operazione.Lettura,
                                          Now, "", objParametri_Utenti
                                          )



                Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura


                    UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
                                          Session("ASG_Utente_Username"),
                                          Session("ASG_IdServizio"),
                                          enum_Security_Attivita.Gest_Magazzino,
                                          enum_Security_Operazione.Modifica,
                                          Now, "", objParametri_Utenti
                                          )

                Case enum_TipoOperazioneDB.Cancellazione


                    UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
                                          Session("ASG_Utente_Username"),
                                          Session("ASG_IdServizio"),
                                          enum_Security_Attivita.Gest_Magazzino,
                                          enum_Security_Operazione.Cancellazione,
                                          Now, "", objParametri_Utenti
                                          )


            End Select


            If Not UtenteAbilitato Then

                PremutoAnnulla = True

                AAA_GestioneUscitaPagina()

                Exit Sub

            End If

            '##############################################################

            ' controllo hidden per il salvataggio dell'indirizzo del profitosan recuperato da web config
            IndirizzoProfitosan.Value = "http://www.agronicagroup.it"


            '##############################################################
            '###############  Inizializzo i pannelli  #####################
            '##############################################################

            Imposta_Pannelli(Operazione, Qs_Mode, xCaricoScarico)

            cmb_PUARegolamenti.Visible = False
            Lbl_PUARegolamenti.Visible = False

            '##############################################################
            '#####  Se sono in MODIFICA carico i dati  ####################
            '##############################################################

            'assegno il valore alla stringa pubblica che verrà poi inserito in un campo hidden sul client..

            Select Case Operazione


                Case enum_TipoOperazioneDB.Modifica

                    Select Case Qs_Lav_Cod

                        '26/02/2019
                        Case LAVCOD_BOLLA_EMESSA,
                                LAVCOD_BOLLA_RICEVUTA,
                                LAVCOD_FATTURA_RICEVUTA,
                                LAVCOD_FATTURA_EMESSA

                            If xFlagDocLight Then
                                Throw New Exception("TO DO")
                            Else
                                'Nuova gestione 2013
                                'modifica del dettaglio nelle operazioni di 
                                'LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA
                                '   ddt, devo permettere la modifica del dettaglio inserito che ora non si può fare
                                '   fattura, se allego il dettaglio di un ddt devo permettere la modifica per aggiungere altre informazioni senza modificare il dettaglio ddt

                                Ripristina_DETTAGLIO_nei_Controlli(Operazione)
                            End If

                        Case LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                               LAVCOD_CONFERIMENTO,
                               LAVCOD_CONFERIMENTO_DIVERSI,
                               LAVCOD_NOTA_ACCREDITO_EMESSA,
                               LAVCOD_NOTA_ACCREDITO_RICEVUTA

                            'Case LAVCOD_BOLLA_EMESSA,
                            'LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                            'LAVCOD_BOLLA_RICEVUTA,
                            'LAVCOD_BOLLA_EMESSA,
                            'LAVCOD_FATTURA_RICEVUTA,
                            'LAVCOD_FATTURA_EMESSA,
                            'LAVCOD_CONFERIMENTO,
                            'LAVCOD_CONFERIMENTO_DIVERSI,
                            'LAVCOD_NOTA_ACCREDITO_EMESSA,
                            'LAVCOD_NOTA_ACCREDITO_RICEVUTA

                            'Nuova gestione 2013
                            'modifica del dettaglio nelle operazioni di 
                            'LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA
                            '   ddt, devo permettere la modifica del dettaglio inserito che ora non si può fare
                            '   fattura, se allego il dettaglio di un ddt devo permettere la modifica per aggiungere altre informazioni senza modificare il dettaglio ddt

                            Ripristina_DETTAGLIO_nei_Controlli(Operazione)


                        Case Else

                            'Ripristina dati nei controlli
                            Me.Ripristina_DATI_nei_Controlli_2(Operazione)

                    End Select


                    'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                    '    Carica_Giacenze_Prezzo()
                    'End If
                    'in data 21/09/2009 ho commentato
                    'visualizzo la giacenza per ogni tipo di operazione
                    Carica_Giacenze_Prezzo()


                    '======================================================================

                Case enum_TipoOperazioneDB.Lettura

                    '26/02/2019
                    Select Case Qs_Lav_Cod

                        Case LAVCOD_BOLLA_EMESSA,
                                LAVCOD_BOLLA_RICEVUTA,
                                LAVCOD_FATTURA_RICEVUTA,
                                LAVCOD_FATTURA_EMESSA

                            If xFlagDocLight Then
                                Throw New Exception("TO DO")
                            Else
                                Throw New Exception("Non dovrebbe entrare qui")
                            End If

                        Case Else 'questo è quello che faceva prima

                            'Ripristina dati nei controlli
                            'Me.Ripristina_DATI_nei_Controlli(Operazione)
                            Me.Ripristina_DATI_nei_Controlli_2(Operazione)
                            'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                            '    Carica_Giacenze_Prezzo()
                            'End If
                            'in data 21/09/2009 ho commentato
                            'visualizzo la giacenza per ogni tipo di operazione
                            Carica_Giacenze_Prezzo()

                    End Select



                    '======================================================================

                Case enum_TipoOperazioneDB.Scrittura


                    CaricaCombo_Causale(Qs_Lav_Cod)

                    Dim Tipo_Fabbricato As Integer


                    If Qs_Tipo = CAU_ANIMALE Then

                        CaricaGriglia_Matricole()
                        Tipo_Fabbricato = STALLA

                    Else

                        CaricaGriglia_Prodotti()
                        Tipo_Fabbricato = FABBRICATI_NO_STALLE

                        Dim Num_Totale As Integer = 0


                        'se ho impostato un filtro sulle categorie ......
                        Dim Dt_Impost As DataTable
                        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                        Dim strElem_Cod As String = ""

                        Dt_Impost = objImpost.Leggi2(1,
                                                    Session("ASG_Utente_Username"),
                                                    0,
                                                    "", "",
                                                    objParametri_Utenti)

                        Dim DrFiltroElemCod As DataRow() = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO)
                        Dim DrFiltro As DataRow() = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT)

                        If DrFiltroElemCod IsNot Nothing AndAlso DrFiltroElemCod.Length > 0 Then
                            strElem_Cod = DrFiltroElemCod(0).Item("Impostazione_Valore_1")
                            strElem_Cod = Replace(strElem_Cod, "|", ",")
                        End If
                        If strElem_Cod <> "" Then
                            strElem_Cod = " Elem_Cod IN (" & strElem_Cod & ") "
                        Else
                            strElem_Cod = "Elem_Cod <> " & COADIUVANTI.ToString
                        End If

                        Call AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(
                                                                Me.cmb_Categoria,
                                                                True, "", "",
                                                                0,
                                                                 xCaricoScarico,
                                                                 0,
                                                                 False,
                                                                 True,
                                                                 strElem_Cod, "", objParametri_Server)

                        Num_Totale = cmb_Categoria.Items.Count - 1

                        'se ho impostato una categoria di default nelle impostazioni utente la imposto
                        Dim Impostazione_Valore_1 As String = ""
                        If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                            Impostazione_Valore_1 = DrFiltro(0).Item("Impostazione_Valore_1")
                            Select Case Impostazione_Valore_1
                                Case Is <> ""
                                    cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(
                                                                      cmb_Categoria.Items.FindByValue(
                                                                                    Impostazione_Valore_1))

                                    cmb_Categoria_SelectedIndexChanged(Nothing, Nothing)

                            End Select
                        End If


                        'se provengo da qualche operazione d'agenda...
                        If Qs_ElemCod <> 0 Then

                            'Imposto la posizione nella combo 
                            Me.cmb_Categoria.SelectedIndex =
                                Me.cmb_Categoria.Items.IndexOf(
                                    Me.cmb_Categoria.Items.FindByValue(
                                        Qs_ElemCod))

                            Me.cmb_Categoria_SelectedIndexChanged(Me, Nothing)

                            ' Me.Lbl_Cerca.ForeColor = AgroColor_Rosso

                        Else
                            '  Me.lbl_categoria.ForeColor = AgroColor_Rosso
                        End If

                    End If

                    Carica_Magazzini(Tipo_Fabbricato, Qs_CodContatto, xPiva, xSa_Cod, xFabbricato_Cod)

                    '======================================================================

            End Select

            If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline_2010 Then

                ' @Paolo:
                ' Gestione proveniente da Smart
                objParametriAgenda = New ParametriAgenda_2010
                objParametriAgenda.Leggi()

                ViewState("Piva") = objParametriAgenda.Piva
                ViewState("Sa_Cod") = objParametriAgenda.Sa_Cod
                ViewState("id_agenda") = objParametriAgenda.Id_Agenda

                Select Case objParametriAgenda.Lavorazione

                    Case LAVCOD_CARICO '1022
                        Imposta_Pannelli(objParametriAgenda.Operazione, "magazzino", enum_Agenda_Causali.CARICO)
                        'Ripristina_DATI_nei_Controlli_2(objParametriAgenda.Operazione)
                        'Carica_Giacenze_Prezzo()

                        Select Case objParametriAgenda.Operazione


                            Case enum_TipoOperazioneDB.Modifica

                                Select Case Qs_Lav_Cod

                                    Case LAVCOD_BOLLA_EMESSA,
                                    LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                                    LAVCOD_BOLLA_RICEVUTA,
                                    LAVCOD_BOLLA_EMESSA,
                                    LAVCOD_FATTURA_RICEVUTA,
                                    LAVCOD_FATTURA_EMESSA,
                                    LAVCOD_CONFERIMENTO,
                                    LAVCOD_CONFERIMENTO_DIVERSI,
                                    LAVCOD_NOTA_ACCREDITO_EMESSA,
                                    LAVCOD_NOTA_ACCREDITO_RICEVUTA


                                        'Nuova gestione 2013
                                        'modifica del dettaglio nelle operazioni di 
                                        'LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA
                                        '   ddt, devo permettere la modifica del dettaglio inserito che ora non si può fare
                                        '   fattura, se allego il dettaglio di un ddt devo permettere la modifica per aggiungere altre informazioni senza modificare il dettaglio ddt

                                        Ripristina_DETTAGLIO_nei_Controlli(objParametriAgenda.Operazione)


                                    Case Else

                                        'Ripristina dati nei controlli
                                        Me.Ripristina_DATI_nei_Controlli_2(objParametriAgenda.Operazione)

                                End Select


                                'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                                '    Carica_Giacenze_Prezzo()
                                'End If
                                'in data 21/09/2009 ho commentato
                                'visualizzo la giacenza per ogni tipo di operazione
                                Carica_Giacenze_Prezzo()


                                '======================================================================

                            Case enum_TipoOperazioneDB.Lettura

                                'Ripristina dati nei controlli
                                'Me.Ripristina_DATI_nei_Controlli(Operazione)
                                Me.Ripristina_DATI_nei_Controlli_2(objParametriAgenda.Operazione)
                                'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                                '    Carica_Giacenze_Prezzo()
                                'End If
                                'in data 21/09/2009 ho commentato
                                'visualizzo la giacenza per ogni tipo di operazione
                                Carica_Giacenze_Prezzo()

                                '======================================================================

                            Case enum_TipoOperazioneDB.Scrittura


                                CaricaCombo_Causale(Qs_Lav_Cod)

                                Dim Tipo_Fabbricato As Integer


                                If Qs_Tipo = CAU_ANIMALE Then

                                    CaricaGriglia_Matricole()
                                    Tipo_Fabbricato = STALLA

                                Else

                                    CaricaGriglia_Prodotti()
                                    Tipo_Fabbricato = FABBRICATI_NO_STALLE

                                    Dim Num_Totale As Integer = 0


                                    'se ho impostato un filtro sulle categorie ......
                                    Dim Dt_Impost As DataTable
                                    Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                    Dim strElem_Cod As String = ""

                                    Dt_Impost = objImpost.Leggi2(1,
                                                                Session("ASG_Utente_Username"),
                                                                0,
                                                                "", "",
                                                                objParametri_Utenti)

                                    Dim DrFiltroElemCod As DataRow() = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO)
                                    Dim DrFiltro As DataRow() = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT)

                                    If DrFiltroElemCod IsNot Nothing AndAlso DrFiltroElemCod.Length > 0 Then
                                        strElem_Cod = DrFiltroElemCod(0).Item("Impostazione_Valore_1")
                                        strElem_Cod = Replace(strElem_Cod, "|", ",")
                                    End If
                                    If strElem_Cod <> "" Then
                                        strElem_Cod = " Elem_Cod IN (" & strElem_Cod & ") "
                                    Else
                                        strElem_Cod = "Elem_Cod <> " & COADIUVANTI.ToString
                                    End If

                                    Call AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(
                                                                            Me.cmb_Categoria,
                                                                            True, "", "",
                                                                            0,
                                                                             xCaricoScarico,
                                                                             0,
                                                                             False,
                                                                             True,
                                                                             strElem_Cod, "", objParametri_Server)

                                    Num_Totale = cmb_Categoria.Items.Count - 1

                                    'se ho impostato una categoria di default nelle impostazioni utente la imposto
                                    Dim Impostazione_Valore_1 As String = ""
                                    If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                                        Impostazione_Valore_1 = DrFiltro(0).Item("Impostazione_Valore_1")
                                        Select Case Impostazione_Valore_1
                                            Case Is <> ""
                                                cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(
                                                                                  cmb_Categoria.Items.FindByValue(
                                                                                                Impostazione_Valore_1))

                                                cmb_Categoria_SelectedIndexChanged(Nothing, Nothing)

                                        End Select
                                    End If


                                    'se provengo da qualche operazione d'agenda...
                                    If Qs_ElemCod <> 0 Then

                                        'Imposto la posizione nella combo 
                                        Me.cmb_Categoria.SelectedIndex =
                                            Me.cmb_Categoria.Items.IndexOf(
                                                Me.cmb_Categoria.Items.FindByValue(
                                                    Qs_ElemCod))

                                        Me.cmb_Categoria_SelectedIndexChanged(Me, Nothing)

                                        '  Me.Lbl_Cerca.ForeColor = AgroColor_Rosso

                                    Else
                                        '  Me.lbl_categoria.ForeColor = AgroColor_Rosso
                                    End If

                                End If

                                Carica_Magazzini(Tipo_Fabbricato, Qs_CodContatto, xPiva, xSa_Cod, xFabbricato_Cod)

                                '======================================================================

                        End Select


                    Case LAVCOD_SCARICO '1023
                        Imposta_Pannelli(objParametriAgenda.Operazione, "magazzino", enum_Agenda_Causali.SCARICO)
                        'Ripristina_DATI_nei_Controlli_2(objParametriAgenda.Operazione)
                        'Carica_Giacenze_Prezzo()

                        Select Case objParametriAgenda.Operazione


                            Case enum_TipoOperazioneDB.Modifica

                                Select Case Qs_Lav_Cod

                                    Case LAVCOD_BOLLA_EMESSA,
                                    LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                                    LAVCOD_BOLLA_RICEVUTA,
                                    LAVCOD_BOLLA_EMESSA,
                                    LAVCOD_FATTURA_RICEVUTA,
                                    LAVCOD_FATTURA_EMESSA,
                                    LAVCOD_CONFERIMENTO,
                                    LAVCOD_CONFERIMENTO_DIVERSI,
                                    LAVCOD_NOTA_ACCREDITO_EMESSA,
                                    LAVCOD_NOTA_ACCREDITO_RICEVUTA


                                        'Nuova gestione 2013
                                        'modifica del dettaglio nelle operazioni di 
                                        'LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA
                                        '   ddt, devo permettere la modifica del dettaglio inserito che ora non si può fare
                                        '   fattura, se allego il dettaglio di un ddt devo permettere la modifica per aggiungere altre informazioni senza modificare il dettaglio ddt

                                        Ripristina_DETTAGLIO_nei_Controlli(objParametriAgenda.Operazione)


                                    Case Else

                                        'Ripristina dati nei controlli
                                        Me.Ripristina_DATI_nei_Controlli_2(objParametriAgenda.Operazione)

                                End Select


                                'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                                '    Carica_Giacenze_Prezzo()
                                'End If
                                'in data 21/09/2009 ho commentato
                                'visualizzo la giacenza per ogni tipo di operazione
                                Carica_Giacenze_Prezzo()


                                '======================================================================

                            Case enum_TipoOperazioneDB.Lettura

                                'Ripristina dati nei controlli
                                'Me.Ripristina_DATI_nei_Controlli(Operazione)
                                Me.Ripristina_DATI_nei_Controlli_2(objParametriAgenda.Operazione)
                                'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                                '    Carica_Giacenze_Prezzo()
                                'End If
                                'in data 21/09/2009 ho commentato
                                'visualizzo la giacenza per ogni tipo di operazione
                                Carica_Giacenze_Prezzo()

                                '======================================================================

                            Case enum_TipoOperazioneDB.Scrittura


                                CaricaCombo_Causale(Qs_Lav_Cod)

                                Dim Tipo_Fabbricato As Integer


                                If Qs_Tipo = CAU_ANIMALE Then

                                    CaricaGriglia_Matricole()
                                    Tipo_Fabbricato = STALLA

                                Else

                                    CaricaGriglia_Prodotti()
                                    Tipo_Fabbricato = FABBRICATI_NO_STALLE

                                    Dim Num_Totale As Integer = 0


                                    'se ho impostato un filtro sulle categorie ......
                                    Dim Dt_Impost As DataTable
                                    Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                    Dim strElem_Cod As String = ""

                                    Dt_Impost = objImpost.Leggi2(1,
                                                                Session("ASG_Utente_Username"),
                                                                0,
                                                                "", "",
                                                                objParametri_Utenti)

                                    Dim DrFiltroElemCod As DataRow() = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO)
                                    Dim DrFiltro As DataRow() = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT)

                                    If DrFiltroElemCod IsNot Nothing AndAlso DrFiltroElemCod.Length > 0 Then
                                        strElem_Cod = DrFiltroElemCod(0).Item("Impostazione_Valore_1")
                                        strElem_Cod = Replace(strElem_Cod, "|", ",")
                                    End If
                                    If strElem_Cod <> "" Then
                                        strElem_Cod = " Elem_Cod IN (" & strElem_Cod & ") "
                                    Else
                                        strElem_Cod = "Elem_Cod <> " & COADIUVANTI.ToString
                                    End If

                                    Call AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(
                                                                            Me.cmb_Categoria,
                                                                            True, "", "",
                                                                            0,
                                                                             xCaricoScarico,
                                                                             0,
                                                                             False,
                                                                             True,
                                                                             strElem_Cod, "", objParametri_Server)

                                    Num_Totale = cmb_Categoria.Items.Count - 1

                                    'se ho impostato una categoria di default nelle impostazioni utente la imposto
                                    Dim Impostazione_Valore_1 As String = ""
                                    If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                                        Impostazione_Valore_1 = DrFiltro(0).Item("Impostazione_Valore_1")
                                        Select Case Impostazione_Valore_1
                                            Case Is <> ""
                                                cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(
                                                                                  cmb_Categoria.Items.FindByValue(
                                                                                                Impostazione_Valore_1))

                                                cmb_Categoria_SelectedIndexChanged(Nothing, Nothing)

                                        End Select
                                    End If


                                    'se provengo da qualche operazione d'agenda...
                                    If Qs_ElemCod <> 0 Then

                                        'Imposto la posizione nella combo 
                                        Me.cmb_Categoria.SelectedIndex =
                                            Me.cmb_Categoria.Items.IndexOf(
                                                Me.cmb_Categoria.Items.FindByValue(
                                                    Qs_ElemCod))

                                        Me.cmb_Categoria_SelectedIndexChanged(Me, Nothing)

                                        ' Me.Lbl_Cerca.ForeColor = AgroColor_Rosso

                                    Else
                                        ' Me.lbl_categoria.ForeColor = AgroColor_Rosso
                                    End If

                                End If

                                Carica_Magazzini(Tipo_Fabbricato, Qs_CodContatto, xPiva, xSa_Cod, xFabbricato_Cod)

                                '======================================================================

                        End Select

                End Select

            End If

        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            Disattiva_Pulsanti()

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore &
                        Chr(13) &
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            'Call Messaggi.AgroMsgBox(Messaggio, Page, "aspnetForm", Me.upDati)
            Call Messaggi.AgroMsgBox(Messaggio, Page, "Form1")
            '------------------------------------------------

        End Try


    End Sub

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

    '########################################################################################
    'La pagina, non essendo più transazionale, non ha nè il Page Commit, nè il Page Abort
    'Utilizza AAA_GestioneUscitaPagina per la gestione di uscita dalla pagina 
    '(in caso di salvataggio o di exit)
    Private Sub AAA_GestioneUscitaPagina(Optional ByVal SalvaNuovo As Boolean = False)

        If Messaggio = "" Then

            If SalvaNuovo Then
                Response.Redirect("FormProdotto.aspx?" & Request.QueryString.ToString)
            Else

                If Not IsNothing(Request.QueryString("exit")) AndAlso Request.QueryString("exit") = True Then
                    'chiudo 
                    Dim str As String = " $(document).ready(function () { "
                    str &= "  window.close();"
                    str &= "});"
                    'ScriptManager.RegisterClientScriptBlock(Me.Master.FindControl("FORM1"), Me.Master.FindControl("FORM1").GetType(),
                    '                                 String.Format("jQuery_{0}", Txt_DataMovimento.ClientID), str, True)

                    Dim strClose As String = "<script language='javascript'>" & str & "</script>"
                    Me.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))


                    'ClientScript.RegisterClientScriptBlock(Me.GetType(), "Close", "window.close()", True)
                    Exit Sub
                End If

                'controllo se provengo dal gias smart

                Dim objParametriAgenda2 As New ParametriAgenda

                Select Case objParametriAgenda2.SitoOrigine
                    Case Enum_SiteRedirector.Sito_GiasOnline_2010
                        Dim link As String = ""
                        Try
                            Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
                            Dim paginaOnLineRitorno As Integer = CInt(Qs_PagRitorno)

                            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso
                               paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                   enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                   enum_PagineAgenda_2010.Menu, objParametriAgenda2.Piva, "", "", 0, "")

                                Response.Redirect(link)
                            Else
                                If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
                                    If paginaOnLineRitorno = enum_PagineGiasOnline_2010.concimazione_lite OrElse
                                       paginaOnLineRitorno = enum_PagineGiasOnline_2010.trattamenti_lite Then
                                        Dim str As String = " $(document).ready(function () { "
                                        str &= "  window.close();"
                                        str &= "});"
                                        Dim strClose As String = "<script language='javascript'>" & str & "</script>"
                                        Me.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

                                        Exit Sub
                                    End If
                                End If
                            End If

                        Catch ex As Exception

                        End Try

                    Case Enum_SiteRedirector.GiasNG
                        Dim link As String = ""
                        AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda2.Piva,
                                                                                Enum_SiteRedirector.GiasNG,
                                                                               objParametriAgenda2.PaginaSitoOrigine,
                                                                                link,
                                                                                objParametri_Server)

                        Response.Redirect(link)
                End Select

                If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Menu Then
                    Response.Redirect("../Menu/Menu.aspx")
                End If

                If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Menu_BS Then
                    Response.Redirect("../Menu/MenuBS_Agenda_Nuovo.aspx")
                End If

                If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Pagina_GestioneMagazzini Then
                    Response.Redirect("../GestioneMagazzini/GestioneMagazzini.aspx")
                End If

                If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS Then
                    Response.Redirect("../GestioneMagazzini/GestioneMagazziniBS.aspx")
                End If

                If CInt(Qs_PagRitorno) = enum_PagineGiasOnline.MenuContab Then
                    'controllo se provengo dal giasonline
                    If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline Then
                        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                        objGiasOnline.PaginaRichiesta = Qs_PagRitorno
                        objGiasOnline.Piva = ViewState("Piva")
                        objGiasOnline.Sa_Cod = ViewState("Sa_Cod")

                        Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                             objGiasOnline)
                        Response.Redirect(str)
                    End If
                End If

                'controllo se provengo dal giasonline
                If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline Then
                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                    objGiasOnline.PaginaRichiesta = Qs_PagRitorno
                    objGiasOnline.Piva = ViewState("Piva")
                    objGiasOnline.Sa_Cod = ViewState("Sa_Cod")

                    Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                     objGiasOnline)
                    Response.Redirect(str)
                End If





                Dim TargetURL As String

                'Verifico che l'uscita sia voluta ....
                If PremutoAnnulla OrElse EseguitaOperazione Then

                    Select Case (Qs_Lav_Cod)

                        Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_TRASFERIMENTO,
                         LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO,
                         LAVCOD_VENDITA, LAVCOD_ACQUISTO

                            Session("DT_Prodotti_nel_Doc") = Nothing
                            Session("matrix_XML_To_Documento") = Nothing
                            Session("vet_XML_To_FormProdotto") = Nothing




                            Dim Origine As String
                            Origine = PaginaAspx_from_TipoEnumPagina(Qs_PagRitorno, "../")

                            'Costruisco il link
                            TargetURL = Origine &
                                    "?k=" &
                                    Stringa_Codifica(Qs_Key, AgroKey_EncoderDecoder, Server) &
                                    "&p=" &
                                    Stringa_Codifica(ViewState("Piva"), AgroKey_EncoderDecoder, Server) &
                                    "&s=" &
                                    Stringa_Codifica(ViewState("Sa_Cod"), AgroKey_EncoderDecoder, Server) &
                                    "&d=" &
                                    Stringa_Codifica(Qs_DataSelezionata, AgroKey_EncoderDecoder, Server) &
                                    "&orig=" &
                                    Stringa_Codifica(Qs_PagRitorno, AgroKey_EncoderDecoder, Server)

                            'Vado alla pagina
                            Response.Redirect(TargetURL)



                        Case Else


                            'non faccio niente xchè ho chiuso la finestra al click sul bottone Salva Tutto



                    End Select

                End If

            End If 'salva e nuovo

        End If 'messaggio

    End Sub


    Private Sub Carica_Magazzini(ByVal Tipo_Fabbricato As Integer, ByVal m_Cod_Contatto As String, ByVal m_Piva As String, ByVal m_Sa_Cod As Integer, ByVal m_Fabbricato_Cod As Integer)

        'TRASFERIMENTO: -> CARICARE SIA PROVENIENZA CHE DESTINAZIONE

        'ALTRI CASI:

        'CARICO: ------> MAGAZZ. DESTINAZIONE
        '                SE IL CONTATTO E' IMPRESA GIAS --> MAGAZZ. PROVENIENZA

        'SCARICO: ------> MAGAZZ. PROVENIENZA
        '                SE IL CONTATTO E' IMPRESA GIAS --> MAGAZZ. DESTINAZIONE

        Dim Num_Totale As Integer = 0
        Dim clc = New AgronicaCoreUtility.CaricaListControl

        Select Case Qs_Mode.ToLower

            Case "trasferimento"

                clc.Fabbricati(Me.Cmb_Provenienza,
                                                True, "", "",
                                                m_Piva,
                                                m_Sa_Cod,
                                                0,
                                                Tipo_Fabbricato,
                                                True,
                                                "",
                                                " Fabbricati.Fabbricato_Des ",
                                                AGRODATAFINE,
                                                objParametri_Server)



                clc.Fabbricati(Me.Cmb_Destinazione,
                                      True, "", "",
                                      m_Piva,
                                      m_Sa_Cod,
                                      0,
                                      Tipo_Fabbricato,
                                      True,
                                      "",
                                      " Fabbricati.Fabbricato_Des ",
                                      AGRODATAFINE,
                                      objParametri_Server)

                'Me.Cmb_Destinazione.Items.Add(New ListItem(strContoConferimento, "-1|-1"))

            Case Else

                'Dim Tipo_Fabbricato As Integer

                Select Case xCaricoScarico

                    Case CAU_CARICO


                        clc.Fabbricati(Me.Cmb_Destinazione,
                                                                True, "", "",
                                                                m_Piva,
                                                                m_Sa_Cod,
                                                                0,
                                                                Tipo_Fabbricato,
                                                                True,
                                                                "",
                                                                " Fabbricati.Fabbricato_Des ",
                                                                AGRODATAFINE,
                                                                objParametri_Server)

                        If m_Fabbricato_Cod <> 0 AndAlso m_Sa_Cod <> 0 Then

                            'Imposto la posizione nella combo 
                            Cmb_Destinazione.SelectedIndex =
                                Cmb_Destinazione.Items.IndexOf(
                                    Cmb_Destinazione.Items.FindByValue(
                                        m_Fabbricato_Cod & "|" & m_Sa_Cod))
                        Else
                            If Me.Cmb_Destinazione.Items.Count = 2 Then
                                Me.Cmb_Destinazione.SelectedIndex = 1
                            End If
                            If Me.Cmb_Destinazione.Items.Count > 2 Then
                                Me.Cmb_Destinazione.SelectedIndex = 0
                            End If
                        End If
                        Cmb_Destinazione_SelectedIndexChanged(Me, Nothing)

                        If m_Cod_Contatto <> "" Then


                            If VerificaEsistenza_PivaGIAS(objParametri_Server, m_Cod_Contatto) = True Then

                                Cmb_Provenienza.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("NessunaGestioneDelMagazzino"), String), "|||"))


                                Me.Cmb_Provenienza.Enabled = True
                                Me.Cmb_Provenienza.Visible = True
                                Me.Chk_MovimentoMag.Visible = True
                                Me.lbl_MagProvenienza.Visible = True
                                Me.lbl_giacenza1_mag.Visible = True
                                '    ' End If

                            End If

                        End If


                    Case CAU_SCARICO

                        ''Carico nella combo provenienza tutti i magazzini del centro aziendale

                        clc.Fabbricati(Me.Cmb_Provenienza,
                                                        True, "", "",
                                                        m_Piva,
                                                        m_Sa_Cod,
                                                        0,
                                                        Tipo_Fabbricato,
                                                        True,
                                                        "",
                                                        " Fabbricati.Fabbricato_Des ",
                                                        AGRODATAFINE,
                                                        objParametri_Server)

                        If m_Fabbricato_Cod <> 0 AndAlso m_Sa_Cod <> 0 Then

                            'Imposto la posizione nella combo 
                            Cmb_Provenienza.SelectedIndex =
                                Cmb_Provenienza.Items.IndexOf(
                                    Cmb_Provenienza.Items.FindByValue(
                                        m_Fabbricato_Cod & "|" & m_Sa_Cod))

                        Else
                            If Me.Cmb_Provenienza.Items.Count = 2 Then
                                Me.Cmb_Provenienza.SelectedIndex = 1
                            End If
                            If Me.Cmb_Provenienza.Items.Count > 2 Then
                                Me.Cmb_Provenienza.SelectedIndex = 0
                            End If
                        End If
                        Cmb_Provenienza_SelectedIndexChanged(Me, Nothing)


                        If m_Cod_Contatto <> "" Then

                            If VerificaEsistenza_PivaGIAS(objParametri_Server, m_Cod_Contatto) = True Then

                                ''Carico nella combo provenienza tutti i magazzini del centro aziendale

                                Cmb_Destinazione.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("NessunaGestioneDelMagazzino"), String), "|||"))

                                Me.Cmb_Destinazione.Enabled = True
                                Me.Cmb_Destinazione.Visible = True
                                Me.lbl_MagDestinazione.Visible = True
                                Me.lbl_giacenza2_mag.Visible = True


                            End If

                        End If

                End Select



        End Select




    End Sub

    '####################################################################################
    Private Sub Cmb_Destinazione_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Cmb_Destinazione.SelectedIndexChanged

        'se sono in bolla ricevuta, il caumov è carico
        'e si può muovere la cmb destinazione

        'se sono in bolla emessa, il caumov è scarico
        'e non si può muovere la cmb destinazione
        'se si muove, significa che è il magazzino del contatto
        'e non si deve gestire l'evento

        If xCaricoScarico <> CAU_SCARICO Then

            Pulisci_Giacenze_Prezzo()

            If Me.Cmb_Destinazione.SelectedIndex > 0 Then

                Dim Gen_Cod As Integer
                Dim Spe_Cod As Integer
                Dim IPro_Cod As Integer

                Select Case Qs_Mode.ToLower

                    Case "trasferimento"

                        If (Me.cmb_Udm.SelectedIndex > 0) AndAlso (Me.cmb_Categoria.SelectedIndex > 0) AndAlso (Me.cmb_Prodotti.SelectedIndex > 0) Then
                            Carica_Giacenze_Prezzo()
                        End If


                    Case Else

                        If Qs_Tipo = CAU_ANIMALE Then

                            Dim ObjStalle As New AgronicaCoreAnagrafeDAL.Stalla_R
                            Dim DtStalle As DataTable

                            xFabbricato_Cod = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(0)
                            xSa_Cod = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(1)
                            ViewState("Sa_Cod") = xSa_Cod

                            Cmb_Animali.Items.Clear()
                            Cmb_IndirizzoProduttivo.Items.Clear()

                            CType(ViewState("vs_dtMatricole"), DataTable).Rows.Clear()
                            Me.DataGrid_ZooAnimali.DataSource = CType(ViewState("vs_dtMatricole"), DataTable)
                            Me.DataGrid_ZooAnimali.DataBind()

                            'Mi procuro il recordset richiesto
                            DtStalle = ObjStalle.Leggi(CStr(xPiva),
                                                       CInt(xSa_Cod),
                                                        CInt(xFabbricato_Cod),
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                        "", "", objParametri_Server)

                            If Not IsNothing(DtStalle) AndAlso DtStalle.Rows.Count > 0 Then

                                Gen_Cod = CInt(DtStalle.Rows(0).Item("Gen_Cod"))
                                Spe_Cod = CInt(DtStalle.Rows(0).Item("Spe_Cod"))
                                IPro_Cod = CInt(DtStalle.Rows(0).Item("IPro_Cod"))

                                'CaricaCombo_Lista_Specie_Animali(Server, Session, Page, Me.Cmb_Animali, 0)
                                AgronicaCoreUtility.CaricaListControl.Lista_Specie_Animali(
                                        Me.Cmb_Animali,
                                        True,
                                        "",
                                        "0",
                                        0,
                                        "",
                                        "",
                                        objParametri_Server
                                )

                                If Cmb_Animali.Items.Count > 1 Then

                                    Cmb_Animali.SelectedIndex =
                                        Cmb_Animali.Items.IndexOf(
                                            Cmb_Animali.Items.FindByValue(
                                                CStr(Gen_Cod) & "|" & CStr(Spe_Cod)))

                                    Me.Cmb_Animali_SelectedIndexChanged(Me, Nothing)

                                    If Me.Cmb_IndirizzoProduttivo.Items.Count > 0 Then

                                        Cmb_IndirizzoProduttivo.SelectedIndex =
                                            Cmb_IndirizzoProduttivo.Items.IndexOf(
                                                Cmb_IndirizzoProduttivo.Items.FindByValue(
                                                    Gen_Cod & "|" & Spe_Cod & "|" & IPro_Cod))


                                        'Costruisco il link
                                        Me.Txt_AnagrafeAnimale.Text = "o=" &
                                                Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                                                "&p=" &
                                                Stringa_Codifica(xPiva, AgroKey_EncoderDecoder, Server) &
                                                "&s=" &
                                                Stringa_Codifica(xSa_Cod, AgroKey_EncoderDecoder, Server) &
                                                "&prg=" &
                                                Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                                                "&gen_cod=" &
                                                Stringa_Codifica(Gen_Cod, AgroKey_EncoderDecoder, Server) &
                                                "&spe_cod=" &
                                                Stringa_Codifica(Spe_Cod, AgroKey_EncoderDecoder, Server) &
                                                "&ipro_cod=" &
                                                Stringa_Codifica(IPro_Cod, AgroKey_EncoderDecoder, Server) &
                                                "&raz_cod=" &
                                                Stringa_Codifica(-1, AgroKey_EncoderDecoder, Server) &
                                                "&r=" &
                                                Stringa_Codifica("../GestioneMagazzini/FormProdotto.aspx", AgroKey_EncoderDecoder, Server) &
                                                "&orig=" &
                                                Stringa_Codifica("../GestioneMagazzini/FormProdotto.aspx", AgroKey_EncoderDecoder, Server)


                                    End If

                                End If

                            End If

                        Else

                            xFabbricato_Cod = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(0)
                            xSa_Cod = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(1)
                            ViewState("Sa_Cod") = xSa_Cod

                            'Me.Txt_CentroAziendale.Text = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)

                            If (Me.cmb_Udm.SelectedIndex > 0) AndAlso
                            (Me.cmb_Categoria.SelectedIndex > 0) AndAlso
                            (Me.cmb_Prodotti.SelectedIndex > 0) Then
                                Carica_Giacenze_Prezzo()
                            End If

                        End If

                End Select


            Else

                Messaggi.AgroMsgBox(AgronicaAgenda_2010.SelezionareIlMagazzinoDiDestinazione, Page, , Me.upDati)

            End If

        Else

            'è il caso del contatto
            If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                Messaggi.AgroMsgBox(AgronicaAgenda_2010.SelezionareIlMagazzinoDiDestinazione, Page, , Me.upDati)
            End If


        End If


    End Sub


    '####################################################################################
    Private Sub Cmb_Provenienza_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Provenienza.SelectedIndexChanged

        'se sono in bolla emessa, il caumov è scarico
        'e si può muovere la cmb provenienza

        'se sono in bolla ricevuta, il caumov è carico
        'e non si può muovere la cmb provenienza
        'se si muove, significa che è il magazzino del contatto
        'e non si deve gestire l'evento

        If xCaricoScarico <> CAU_CARICO Then

            Pulisci_Giacenze_Prezzo()

            'non pulisco la chiave nel caso di ddt e fattura emessa e nota accredito ricevuta o mi resetta combo prodotto e elimina categoria selezionata
            If (Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura) AndAlso
                 (Qs_Lav_Cod = LAVCOD_BOLLA_EMESSA OrElse Qs_Lav_Cod = LAVCOD_FATTURA_EMESSA OrElse Qs_Lav_Cod = LAVCOD_NOTA_ACCREDITO_RICEVUTA) Then
                '' ''ATTENZIONE, LO REIMPOSTA A ZERO NEL CASO DI MODIFICA DDT EMESSO, 
                'anche la combo prodotto viene azzerata, conviene gestire il caso in
                '' ''lo fa in Carica_Magazzini()->Cmb_Provenienza_SelectedIndexChanged() perché controlla If xCaricoScarico <> CAU_CARICO  e chiama Pulisci_ChiaveProdotto() 
                'ho l'elem_cod -> imposto la combo delle categorie magazzino
            Else
                Pulisci_ChiaveProdotto()
            End If


            If Me.Cmb_Provenienza.SelectedIndex > 0 Then

                Dim mbProvenienza_Valida As Boolean

                Dim Gen_Cod As Integer
                Dim Spe_Cod As Integer
                Dim IPro_Cod As Integer

                Dim Spe_Des As String
                Dim Ipro_Des As String
                Dim Raz_Des As String
                Dim Cat_Des As String

                Dim Metodo_Produzione_Des As String

                Dim i As Integer

                Select Case Qs_Mode.ToLower


                    Case "trasferimento"

                        If (Me.cmb_Udm.SelectedIndex > 0) AndAlso (Me.cmb_Categoria.SelectedIndex > 0) AndAlso (Me.cmb_Prodotti.SelectedIndex > 0) Then
                            Carica_Giacenze_Prezzo()
                        End If

                    Case Else

                        If Qs_Tipo = CAU_ANIMALE Then


                            'ObjStalle = Server.CreateCANCELLATOObject("Agro_Anagrafe_AD.Stalla_R")
                            Dim ObjStalle As New AgronicaCoreAnagrafeDAL.Stalla_R
                            Dim DtStalle As DataTable

                            'Creo l'oggetto COM+
                            Dim ObjConsistenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                            Dim RsConsistenze As DataTable




                            Dim ObjAnimale_Anagrafe_R As New AgronicaCoreZooDAL.Zoo_Animali_R
                            Dim RsAnimale_Anagrafe_R As DataTable


                            xFabbricato_Cod = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(0)
                            xSa_Cod = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(1)
                            ViewState("Sa_Cod") = xSa_Cod

                            Cmb_Animali.Items.Clear()
                            Cmb_IndirizzoProduttivo.Items.Clear()

                            CType(ViewState("vs_dtMatricole"), DataTable).Rows.Clear()
                            Me.DataGrid_ZooAnimali.DataSource = CType(ViewState("vs_dtMatricole"), DataTable)
                            Me.DataGrid_ZooAnimali.DataBind()

                            'Mi procuro il recordset richiesto
                            DtStalle = ObjStalle.Leggi(CStr(xPiva),
                                                       CInt(xSa_Cod),
                                                        CInt(xFabbricato_Cod),
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                        "", "", objParametri_Server)

                            If Not IsNothing(DtStalle) AndAlso DtStalle.Rows.Count > 0 Then

                                '==============================================================================
                                'Controllo se la stalla ha un genere valido per gestire la variazione
                                'di consistenza
                                If Qs_Lav_Cod = "3002" AndAlso Qs_Operazione = enum_TipoOperazioneDB.Scrittura Then

                                    Select Case CInt(DtStalle.Rows(0).Item("Gen_Cod"))

                                        Case 1, 2, 3 'Bovini, Bufali, Ovini, Caprini, Suini, Cinghiali

                                            mbProvenienza_Valida = False

                                            Messaggi.AgroMsgBox(
                                                String.Format(
                                                    DirectCast(
                                                        GetLocalResourceObject("Cmb_Provenienza_SelectedIndexChanged_UtilizzareOperazioniPerLaStallaSelezionata"), String),
                                                    Chr(13)
                                                ), Page, , Me.upDati)
                                            Exit Sub

                                        Case Else

                                            mbProvenienza_Valida = True

                                    End Select

                                Else

                                    mbProvenienza_Valida = True

                                End If
                                '==============================================================================

                                If mbProvenienza_Valida Then

                                    Gen_Cod = CInt(DtStalle.Rows(0).Item("Gen_Cod"))
                                    Spe_Cod = CInt(DtStalle.Rows(0).Item("Spe_Cod"))
                                    IPro_Cod = CInt(DtStalle.Rows(0).Item("IPro_Cod"))

                                    'CaricaCombo_Lista_Specie_Animali(Server, Session, Page, Me.Cmb_Animali, 0)

                                    AgronicaCoreUtility.CaricaListControl.Lista_Specie_Animali(
                                            Me.Cmb_Animali,
                                            True,
                                            "",
                                            "0",
                                            0,
                                            "",
                                            "",
                                            objParametri_Server
                                    )


                                    If Cmb_Animali.Items.Count > 1 Then

                                        Cmb_Animali.SelectedIndex =
                                            Cmb_Animali.Items.IndexOf(
                                                Cmb_Animali.Items.FindByValue(
                                                    CStr(Gen_Cod) & "|" & CStr(Spe_Cod)))

                                        Me.Cmb_Animali_SelectedIndexChanged(Me, Nothing)

                                        If Me.Cmb_IndirizzoProduttivo.Items.Count > 0 Then

                                            Cmb_IndirizzoProduttivo.SelectedIndex =
                                                Cmb_IndirizzoProduttivo.Items.IndexOf(
                                                    Cmb_IndirizzoProduttivo.Items.FindByValue(
                                                        Gen_Cod & "|" & Spe_Cod & "|" & IPro_Cod))


                                            '------------------------------
                                            'CARICO GLI ANIMALI PRESENTI

                                            RsConsistenze = ObjConsistenze.LeggiConsistenze(CStr(xPiva),
                                                                                            CInt(xSa_Cod),
                                                                                            CInt(300),
                                                                                            CInt(0),
                                                                                            CInt(0),
                                                                                            CInt(38),
                                                                                            CInt(xFabbricato_Cod),
                                                                                            CInt(0),
                                                                                            CInt(0),
                                                                                            CInt(0),
                                                                                            CStr(""),
                                                                                            CInt(Gen_Cod),
                                                                                            CInt(Spe_Cod),
                                                                                            CInt(IPro_Cod),
                                                                                             0,
                                                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                             "",
                                                                                            "",
                                                                                            objParametri_Server)


                                            If RsConsistenze.Rows.Count <> 0 Then
                                                Dim j As Integer = 0
                                                Do While j < RsConsistenze.Rows.Count

                                                    'Inserisco le sole Consistenze <> 0
                                                    If CStr(RsConsistenze.Rows(j).Item("Qta")) <> "0" Then

                                                        RsAnimale_Anagrafe_R = ObjAnimale_Anagrafe_R.Leggi(CStr(xPiva),
                                                                                                        0,
                                                                                                           CInt(RsConsistenze.Rows(j).Item("Cod_Progetto")),
                                                                                                            "", 0, 0, 0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


                                                        If RsAnimale_Anagrafe_R.Rows.Count <> 0 Then

                                                            Dim k = 0

                                                            Spe_Des = StallaSpecieDes_from_StallaSpecieCod2(
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Gen_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Spe_Cod")),
                                                                                    objParametri_Server)

                                                            Ipro_Des = StallaIProDes_from_StallaIProCod2(
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Gen_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Spe_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Ipro_Cod")),
                                                                                    objParametri_Server)

                                                            Cat_Des = StallaCategoriaDes_from_StallaCategoriaCod2(
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Gen_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Spe_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Ipro_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Cat_Cod")),
                                                                                    objParametri_Server)

                                                            Raz_Des = StallaRazzaDes_from_StallaRazzaCod2(
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Gen_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Spe_Cod")),
                                                                                    CInt(RsAnimale_Anagrafe_R.Rows(k).Item("Raz_Cod")),
                                                                                    objParametri_Server)

                                                            Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale

                                                            Select Case RsAnimale_Anagrafe_R.Rows(k).Item("metodo_produzione")
                                                                Case 1
                                                                    Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale
                                                                Case 2
                                                                    Metodo_Produzione_Des = AgronicaAgenda_2010.InConversione
                                                                Case 3
                                                                    Metodo_Produzione_Des = AgronicaAgenda_2010.Biologico
                                                            End Select

                                                            Inserisci_Riga_Matricola(CInt(RsConsistenze.Rows(j).Item("Cod_Progetto")),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Gen_Cod"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Spe_Cod"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Ipro_Cod"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Raz_Cod"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Cat_Cod"),
                                                                                    Spe_Des,
                                                                                    Ipro_Des,
                                                                                    Raz_Des,
                                                                                    Cat_Des,
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Matricola"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Nome"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("Collare"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("nome_aia"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("matricola_aia"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("dat_nascita"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("prov_nascita"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("stato_nascita"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("aua_azi_nascita"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("ausl_azi_nascita"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("sesso"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("mat_padre"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("mat_madre"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("peso"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("data_pesa"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("metodo_produzione"),
                                                                                    Metodo_Produzione_Des,
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("conversione_inizio"),
                                                                                    RsAnimale_Anagrafe_R.Rows(k).Item("conversione_fine"),
                                                                                    RsConsistenze.Rows(j).Item("Qta"),
                                                                                    RsConsistenze.Rows(j).Item("Prezzo_Unitario"))

                                                        End If

                                                    End If

                                                    j = j + 1

                                                Loop

                                            End If

                                            For i = 0 To Me.DataGrid_ZooAnimali.Items.Count - 1

                                                CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text = DataGrid_ZooAnimali.Items(i).Cells(32).Text
                                                CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text = DataGrid_ZooAnimali.Items(i).Cells(33).Text

                                            Next


                                        End If

                                    End If

                                End If


                            End If


                            ObjConsistenze = Nothing
                            RsConsistenze = Nothing

                            ObjAnimale_Anagrafe_R = Nothing
                            RsAnimale_Anagrafe_R = Nothing

                        Else

                            xFabbricato_Cod = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(0)
                            xSa_Cod = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(1)
                            ViewState("Sa_Cod") = xSa_Cod

                            'Me.Txt_CentroAziendale.Text = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)

                            'non serve, perchè viene svuotato tutto
                            'If (Me.cmb_Udm.SelectedIndex > 0) And (Me.cmb_Categoria.SelectedIndex > 0) And (Me.cmb_Prodotti.SelectedIndex > 0) Then
                            '    Carica_Giacenze_Prezzo()
                            'End If

                        End If

                End Select


            Else

                'viene già notificato nell'evento della categoria
                ' Messaggi.AgroMsgBox("Selezionare il magazzino di provenienza!",Page, , Me.upDati)

            End If

        Else

            'è il caso del contatto
            If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                Messaggi.AgroMsgBox(AgronicaAgenda_2010.SelezionareIlMagazzinoDiProvenienza, Page, , Me.upDati)
            End If

        End If


    End Sub


    '####################################################################################
    Private Sub Imposta_MovimentoMagazzino(ByVal Operazione As Integer, ByVal CAU_MOV As Integer)

        Me.Pannello_Prodotto.Visible = True

        Me.Txt_PendenzaIniziale.Text = enum_Pendenza.MovPendente

        Me.cmb_Prodotti.SelectedIndex = 0
        Me.cmb_Lotto.SelectedIndex = 0
        Me.cmb_LottoAccettazione.SelectedIndex = 0
        Me.cmb_Calibro.SelectedIndex = 0
        Me.cmb_Udm.SelectedIndex = 0

        'Disabilito momentaneamente le combo del lotto e del calibro
        Me.cmb_Calibro.Enabled = False
        Me.cmb_Lotto.Enabled = False

        'PANNELLO DETTAGLIO
        Me.Txt_Quantita.Text = ""
        Me.Txt_PrezzoUnitario.Text = "0,00"


        Select Case Operazione

            '#########################################
            '################ LETTURA ################
            '#########################################

            Case enum_TipoOperazioneDB.Lettura

                'Me.Pannello_InsGriglia.Visible = False
                'Me.Pannello_Salvataggio.Visible = False


            Case enum_TipoOperazioneDB.Scrittura

                '#########################################
                '############### SCRITTURA ###############
                '#########################################

                'PANNELLO MOVIMENTI
                Me.Txt_DataMovimento.Text = Qs_DataSelezionata
                Me.Txt_Ora.Text = CDate(Qs_DataSelezionata).ToShortTimeString
                If Me.Txt_Ora.Text = "00.00" OrElse Me.Txt_Ora.Text = "0.00" Then
                    Me.Txt_Ora.Text = "12" & gettimesep() & "00"
                End If

                Me.Pannello_InsGriglia.Visible = True
                'Me.Pannello_Salvataggio.Visible = True


            Case enum_TipoOperazioneDB.Modifica

                '#########################################
                '############### MODIFICA ################
                '#########################################

                Me.Pannello_Salvataggio.Visible = True
                Me.ImgBtnSalvaNuovo.Visible = False
                Me.ImgBtnSalvaNuovo.Enabled = False

        End Select


        Select Case CAU_MOV

            Case CAU_CARICO

                Imposta_MovimentoMagazzino_Carico()

            Case CAU_SCARICO

                Imposta_MovimentoMagazzino_Scarico()

        End Select


    End Sub


    '####################################################################################
    Private Sub Imposta_MovimentoMagazzino_Carico()


        'PANNELLO MAGAZZINI MOVIMENTATI

        'CARICO ---> attivata la destinazione

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String)
        ' Me.Lbl_SalvaMagazzino.Text = "Salva il carico"

        'Me.Cmb_Provenienza.SelectedIndex = 0
        Me.Cmb_Provenienza.Enabled = False
        Me.Cmb_Provenienza.Visible = False
        Me.Chk_MovimentoMag.Visible = False
        Me.Txt_Giacenza_Provenienza.Visible = False
        Me.lbl_MagProvenienza.Visible = False
        Me.lbl_giacenza1_mag.Visible = False
        'Me.Txt_Giacenza_Provenienza_Tot.Visible = False

        Me.Cmb_Destinazione.Enabled = True
        Me.Cmb_Destinazione.Visible = True
        Me.lbl_MagDestinazione.Visible = True
        Me.lbl_giacenza2_mag.Visible = True
        Me.Txt_Giacenza_Destinazione.Visible = True
        'Me.Txt_Giacenza_Destinazione_Tot.Visible = True


    End Sub


    '####################################################################################
    Private Sub Imposta_MovimentoMagazzino_Scarico()


        'PANNELLO MAGAZZINI MOVIMENTATI

        'SCARICO ---> attivata la provenienza

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String)
        'Me.Lbl_SalvaMagazzino.Text = "Salva lo scarico"

        'Me.Cmb_Destinazione.SelectedIndex = 0
        Me.Cmb_Destinazione.Enabled = False
        Me.Cmb_Destinazione.Visible = False
        Me.lbl_MagDestinazione.Visible = False
        Me.lbl_giacenza2_mag.Visible = False
        Me.Txt_Giacenza_Destinazione.Visible = False
        '  Me.Txt_Giacenza_Destinazione_Tot.Visible = False

        Me.Cmb_Provenienza.Enabled = True
        Me.Cmb_Provenienza.Visible = True
        Me.Chk_MovimentoMag.Visible = True
        Me.lbl_MagProvenienza.Visible = True
        Me.lbl_giacenza1_mag.Visible = True
        Me.Txt_Giacenza_Provenienza.Visible = True
        '  Me.Txt_Giacenza_Provenienza_Tot.Visible = True


    End Sub


    '####################################################################################
    Private Sub Imposta_ZOO()

        Me.Txt_PendenzaIniziale.Text = enum_Pendenza.ZooConsistenzeIniziali

        Me.Pannello_Animale.Visible = True

        Me.lbl_prodotto.Text = "&nbsp;" & DirectCast(GetLocalResourceObject("ConsistenzaZootecnica"), String) & " :"
        Me.lbl_magazzini.Text = "&nbsp; " & DirectCast(GetLocalResourceObject("StalleMovimentate"), String) & " :"
        Me.lbl_MagDestinazione.Text = DirectCast(GetLocalResourceObject("StallaDestinazione"), String) & " :"
        Me.lbl_MagProvenienza.Text = DirectCast(GetLocalResourceObject("StallaProvenienza"), String) & " :"
        'Me.lbl_giacenza_mag.Text = "Giacenza Stalla"
        'Me.lbl_giacenza_mag.Visible = False
        'Me.lbl_giacenza_centro.Visible = False

        Me.cmb_Categoria.SelectedIndex = 0
        Me.cmb_Prodotti.SelectedIndex = 0
        Me.cmb_Lotto.SelectedIndex = 0
        Me.Cmb_Ordini.SelectedIndex = 0
        Me.cmb_LottoAccettazione.SelectedIndex = 0
        Me.cmb_Calibro.SelectedIndex = 0
        Me.cmb_Udm.SelectedIndex = 0

        Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoStallaAbbr"), String)
        Me.Chk_MovimentoMag.Checked = True


    End Sub



    '####################################################################################
    Private Sub Imposta_MovimentoConsistenza(ByVal Operazione As Integer, ByVal CAU_MOV As Integer)


        Imposta_ZOO()

        Me.lbl_dettaglio.Visible = False
        Me.Pannello_Dettaglio.Visible = False


        'PANNELLO DETTAGLIO
        Me.Txt_Quantita.Text = ""
        Me.Txt_PrezzoUnitario.Text = "0,00"


        Select Case Operazione

            '#########################################
            '################ LETTURA ################
            '#########################################

            Case enum_TipoOperazioneDB.Lettura



                '#########################################
                '############### SCRITTURA ###############
                '#########################################

            Case enum_TipoOperazioneDB.Scrittura


                'PANNELLO MOVIMENTI
                Me.Txt_DataMovimento.Text = Qs_DataSelezionata
                Me.Txt_Ora.Text = CDate(Qs_DataSelezionata).ToShortTimeString
                If Me.Txt_Ora.Text = "00.00" OrElse Me.Txt_Ora.Text = "0.00" Then
                    Me.Txt_Ora.Text = "12" & gettimesep() & "00"
                End If

                Me.Pannello_Salvataggio.Visible = True


                '#########################################
                '############### MODIFICA ################
                '#########################################

            Case enum_TipoOperazioneDB.Modifica

                Me.Pannello_Salvataggio.Visible = True
                Me.AnagrafeAnimale.Visible = False
                Me.Lbl_AnagrafeAnimale.Visible = False

        End Select


        Select Case CAU_MOV

            Case CAU_CARICO

                Imposta_MovimentoConsistenza_Carico()

            Case CAU_SCARICO

                Imposta_MovimentoConsistenza_Scarico()

        End Select


    End Sub


    '####################################################################################
    Private Sub Imposta_MovimentoConsistenza_Carico()

        'PANNELLO STALLE MOVIMENTATE

        'CARICO ---> attivata la destinazione

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("IncrementoDiConsistenze"), String)
        'Me.Lbl_SalvaMagazzino.Text = "Salva l'Incremento di Consistenze"

        Me.Cmb_Provenienza.Enabled = False
        Me.Cmb_Provenienza.Visible = False
        Me.Chk_MovimentoMag.Visible = False
        Me.Txt_Giacenza_Provenienza.Visible = False
        Me.lbl_MagProvenienza.Visible = False
        Me.lbl_giacenza1_mag.Visible = False
        '  Me.Txt_Giacenza_Provenienza_Tot.Visible = False

        Me.Cmb_Destinazione.Enabled = True
        Me.Cmb_Destinazione.Visible = True
        Me.lbl_MagDestinazione.Visible = True
        Me.lbl_giacenza2_mag.Visible = True
        Me.Txt_Giacenza_Destinazione.Visible = False
        '  Me.Txt_Giacenza_Destinazione_Tot.Visible = False



    End Sub


    '####################################################################################
    Private Sub Imposta_MovimentoConsistenza_Scarico()

        'PANNELLO STALLE MOVIMENTATE

        'SCARICO ---> attivata la provenienza

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("DecrementoDiConsistenze"), String)
        'Me.Lbl_SalvaMagazzino.Text = "Salva il Decremento di Consistenze"

        Me.Cmb_Destinazione.Enabled = False
        Me.Cmb_Destinazione.Visible = False
        Me.lbl_MagDestinazione.Visible = False
        Me.lbl_giacenza2_mag.Visible = False
        Me.Txt_Giacenza_Destinazione.Visible = False
        '  Me.Txt_Giacenza_Destinazione_Tot.Visible = False

        Me.Cmb_Provenienza.Enabled = True
        Me.Cmb_Provenienza.Visible = True
        Me.Chk_MovimentoMag.Visible = True
        Me.Txt_Giacenza_Provenienza.Visible = False
        Me.lbl_MagProvenienza.Visible = True
        Me.lbl_giacenza1_mag.Visible = True
        '  Me.Txt_Giacenza_Provenienza_Tot.Visible = False

        Me.AnagrafeAnimale.Visible = False
        Me.Lbl_AnagrafeAnimale.Visible = False


    End Sub


    '####################################################################################
    Private Sub Imposta_DocumentoTrasporto(ByVal Operazione As Integer, ByVal CAU_MOV As Integer)

        Select Case Qs_Tipo
            Case CAU_MAGAZZINO
                Me.Pannello_Prodotto.Visible = True
                Me.Txt_PendenzaIniziale.Text = enum_Pendenza.MovPendente
            Case CAU_ANIMALE
                Me.Pannello_Animale.Visible = True
                Imposta_ZOO()
        End Select


        Select Case Operazione

            '#########################################
            '################ LETTURA ################
            '#########################################

            Case enum_TipoOperazioneDB.Lettura



            Case enum_TipoOperazioneDB.Scrittura

                '#########################################
                '############### SCRITTURA ###############
                '#########################################

                Me.Txt_DataMovimento.Text = Qs_DataSelezionata
                Me.Txt_Ora.Text = Qs_OraSelezionata

                If Not xFlagDocLight Then
                    Select Case Qs_Tipo
                        Case CAU_MAGAZZINO
                            Me.Pannello_InsGriglia.Visible = True
                            Me.Pannello_InsDocumento.Visible = False
                        Case CAU_ANIMALE
                            Me.Pannello_InsDocumento.Visible = True
                            ' x il momento (un giorno non dovrà esserlo)
                            Me.Pannello_Dettaglio.Visible = False
                            Me.lbl_dettaglio.Visible = False
                    End Select

                Else
                    'modalità light
                    ConfigXDocumentoLight()

                    Select Case Qs_Tipo
                        Case CAU_MAGAZZINO
                            Me.Pannello_InsGriglia.Visible = True
                            'Me.Pannello_InsDocumento.Visible = False
                        Case CAU_ANIMALE
                            'Me.Pannello_InsDocumento.Visible = True
                            'qui andrà attivato il pulsante di salva TO DO

                            ' x il momento (un giorno non dovrà esserlo)
                            Me.Pannello_Dettaglio.Visible = False
                            Me.lbl_dettaglio.Visible = False
                    End Select


                End If

            Case enum_TipoOperazioneDB.Modifica

                '#########################################
                '############### MODIFICA ################
                '#########################################

                'è possibile solo per i prodotti?

                If Not xFlagDocLight Then
                    Me.Pannello_InsDocumento.Visible = True
                Else

                    'carica il menù dei fornitori
                    AgronicaCoreUtility.CaricaListControl.Contatti(Me.Cmb_Fornitore,
                                True,
                                DirectCast(GetLocalResourceObject("SelezionaUnFornitoreTreCaratteri"), String),
                                0,
                                xPiva,
                                "",
                                0,
                                0,
                                True,
                                False,
                                0,
                               0,
                                False,
                                0,
                                ID_CF_NOFILTRO,
                                " Rapporti_Contabili.Fornitore = 1 ",
                                "",
                                objParametri_Server,
                                1)
                    Me.Pannello_InsGriglia.Visible = True
                End If


        End Select


        Select Case CAU_MOV

            Case CAU_CARICO

                Imposta_DocumentoTrasporto_Ricevuto()

            Case CAU_SCARICO

                Imposta_DocumentoTrasporto_Emesso()

        End Select


    End Sub

    '####################################################################################
    Private Sub ConfigXDocumentoLight()

        Me.Pannello_DocLight.Visible = True
        Me.LblNote.Visible = False
        Me.TextBoxNote.Visible = False

        'carica il menù dei fornitori
        AgronicaCoreUtility.CaricaListControl.Contatti(Me.Cmb_Fornitore,
                    True,
                    DirectCast(GetLocalResourceObject("SelezionaUnFornitoreTreCaratteri"), String),
                    0,
                    xPiva,
                    "",
                    0,
                    0,
                    True,
                    False,
                    0,
                   0,
                    False,
                    0,
                    ID_CF_NOFILTRO,
                    " Rapporti_Contabili.Fornitore = 1 ",
                    "",
                    objParametri_Server,
                    1)



    End Sub




    '####################################################################################
    Private Sub Imposta_DocumentoTrasporto_Ricevuto()

        'PANNELLO MAGAZZINI MOVIMENTATI

        'CARICO ---> attivata la destinazione

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("InserimentoDettaglioNelDDTRicevuto"), String)

        'Me.Cmb_Provenienza.SelectedIndex = 0
        Me.Cmb_Provenienza.Enabled = False
        Me.Cmb_Provenienza.Visible = False
        Me.Chk_MovimentoMag.Visible = False
        Me.Txt_Giacenza_Provenienza.Visible = False
        Me.lbl_MagProvenienza.Visible = False
        Me.lbl_giacenza1_mag.Visible = False
        '  Me.Txt_Giacenza_Provenienza_Tot.Visible = False

        Me.Cmb_Destinazione.Enabled = True
        Me.Cmb_Destinazione.Visible = True
        Me.lbl_MagDestinazione.Visible = True
        Me.lbl_giacenza2_mag.Visible = True
        Me.Txt_Giacenza_Destinazione.Visible = True
        '  Me.Txt_Giacenza_Destinazione_Tot.Visible = True


    End Sub


    '####################################################################################
    Private Sub Imposta_DocumentoTrasporto_Emesso()


        'PANNELLO MAGAZZINI MOVIMENTATI

        'SCARICO ---> attivata la provenienza

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("InserimentoDettaglioNelDDTEmesso"), String)

        'Me.Cmb_Destinazione.SelectedIndex = 0
        Me.Cmb_Destinazione.Enabled = False
        Me.Cmb_Destinazione.Visible = False
        Me.lbl_MagDestinazione.Visible = False
        Me.lbl_giacenza2_mag.Visible = False
        Me.Txt_Giacenza_Destinazione.Visible = False
        '   Me.Txt_Giacenza_Destinazione_Tot.Visible = False

        Me.Cmb_Provenienza.Enabled = True
        Me.Cmb_Provenienza.Visible = True
        Me.Chk_MovimentoMag.Visible = True
        Me.Txt_Giacenza_Provenienza.Visible = True
        Me.lbl_MagProvenienza.Visible = True
        Me.lbl_giacenza1_mag.Visible = True
        '  Me.Txt_Giacenza_Provenienza_Tot.Visible = True


    End Sub


    '####################################################################################
    Private Sub Imposta_Fattura(ByVal Operazione As Integer, ByVal CAU_MOV As Integer)

        Me.Pannello_Prodotto.Visible = True
        Me.Pannello_Economico.Visible = True

        If Qs_RagSocContatto <> "" Then
            Me.Chk_Conti.Visible = True
            Me.Chk_Conti.Text = Me.Chk_Conti.Text & Qs_RagSocContatto
        Else
            Me.Chk_Conti.Visible = False
        End If

        Me.Txt_PendenzaIniziale.Text = enum_Pendenza.MovPendente

        Select Case Qs_Tipo
            Case CAU_MAGAZZINO
                Me.Pannello_Prodotto.Visible = True
            Case CAU_ANIMALE
                Me.Pannello_Animale.Visible = True
        End Select

        Carica_Pannello_Economico()

        Select Case Operazione

            '#########################################
            '################ LETTURA ################
            '#########################################

            Case enum_TipoOperazioneDB.Lettura

            Case enum_TipoOperazioneDB.Scrittura

                '#########################################
                '############### SCRITTURA ###############
                '#########################################

                Me.Txt_DataMovimento.Text = Qs_DataSelezionata
                Me.Txt_Ora.Text = Qs_OraSelezionata

                If Not xFlagDocLight Then

                    Me.Pannello_InsGriglia.Visible = True
                    Me.Pannello_InsDocumento.Visible = True
                Else
                    'modalità light
                    Me.Pannello_InsGriglia.Visible = True
                    Me.Pannello_InsDocumento.Visible = False

                    ConfigXDocumentoLight()
                End If

            Case enum_TipoOperazioneDB.Modifica

                '#########################################
                '############### MODIFICA ################
                '#########################################

                If Not xFlagDocLight Then
                    Me.Pannello_InsDocumento.Visible = True
                Else
                    'modalità light
                    Me.Pannello_InsDocumento.Visible = False

                    'carica il menù dei fornitori
                    AgronicaCoreUtility.CaricaListControl.Contatti(Me.Cmb_Fornitore,
                                True,
                                DirectCast(GetLocalResourceObject("SelezionaUnFornitoreTreCaratteri"), String),
                                0,
                                xPiva,
                                "",
                                0,
                                0,
                                True,
                                False,
                                0,
                               0,
                                False,
                                0,
                                ID_CF_NOFILTRO,
                                " Rapporti_Contabili.Fornitore = 1 ",
                                "",
                                objParametri_Server,
                                1)
                End If

        End Select


        Select Case CAU_MOV

            Case CAU_CARICO

                Imposta_Fattura_Ricevuta()

            Case CAU_SCARICO

                Imposta_Fattura_Emessa()

        End Select


    End Sub


    '####################################################################################
    Private Sub Imposta_Fattura_Emessa()

        'PANNELLO MAGAZZINI MOVIMENTATI

        'SCARICO ---> attivata la provenienza

        Select Case Qs_Lav_Cod
            Case LAVCOD_FATTURA_EMESSA
                CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("InserimentoDettaglioNellaFatturaEmessa"), String)
            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("InserimentoDettaglioNellaNotadiAccreditoRicevuta"), String)
        End Select

        ' Me.Lbl_SalvaMagazzino.Text = ""

        Me.Cmb_Destinazione.Enabled = False
        Me.Cmb_Destinazione.Visible = False
        Me.lbl_MagDestinazione.Visible = False
        Me.lbl_giacenza2_mag.Visible = False
        Me.Txt_Giacenza_Destinazione.Visible = False
        Me.lbl_MagDestinazione.Visible = False
        Me.lbl_giacenza2_mag.Visible = False

        Me.Cmb_Provenienza.Enabled = True
        Me.Cmb_Provenienza.Visible = True
        Me.Chk_MovimentoMag.Visible = True
        Me.lbl_MagProvenienza.Visible = True
        Me.lbl_giacenza1_mag.Visible = True
        Me.Txt_Giacenza_Provenienza.Visible = True


    End Sub


    '####################################################################################
    Private Sub Imposta_Fattura_Ricevuta()

        'PANNELLO MAGAZZINI MOVIMENTATI

        'CARICO ---> attivata la destinazione

        Select Case Qs_Lav_Cod
            Case LAVCOD_FATTURA_RICEVUTA
                CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("InserimentoDettaglioNellaFatturaRicevuta"), String)
            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("InserimentoDettaglioNellaNotaDiAccreditoEmessa"), String)
        End Select

        ' Me.Lbl_SalvaMagazzino.Text = ""

        Me.Cmb_Provenienza.Enabled = False
        Me.Cmb_Provenienza.Visible = False
        Me.Chk_MovimentoMag.Visible = False
        Me.lbl_MagProvenienza.Visible = False
        Me.lbl_giacenza1_mag.Visible = False
        Me.Txt_Giacenza_Provenienza.Visible = False

        Me.Cmb_Destinazione.Enabled = True
        Me.Cmb_Destinazione.Visible = True
        Me.lbl_MagDestinazione.Visible = True
        Me.lbl_giacenza2_mag.Visible = True
        Me.lbl_MagDestinazione.Visible = True
        Me.lbl_giacenza2_mag.Visible = True
        Me.Txt_Giacenza_Destinazione.Visible = True


    End Sub


    '####################################################################################
    Private Sub Imposta_Compravendita(ByVal Operazione As Integer, ByVal CAU_MOV As Integer)

        Me.Pannello_Prodotto.Visible = True
        Me.Pannello_Economico.Visible = True
        Me.Chk_Conti.Visible = False

        Me.Txt_PendenzaIniziale.Text = enum_Pendenza.MovPendente


        Carica_Pannello_Economico()


        Select Case Operazione

            '#########################################
            '################ LETTURA ################
            '#########################################

            Case enum_TipoOperazioneDB.Lettura


                '#########################################
                '############### SCRITTURA ###############
                '#########################################

            Case enum_TipoOperazioneDB.Scrittura

                Me.Txt_DataMovimento.Text = Qs_DataSelezionata
                Me.Txt_Ora.Text = CDate(Qs_DataSelezionata).ToShortTimeString
                If Me.Txt_Ora.Text = "00.00" OrElse Me.Txt_Ora.Text = "0.00" Then
                    Me.Txt_Ora.Text = "12" & gettimesep() & "00"
                End If

                Me.Pannello_InsGriglia.Visible = True
                'Me.Pannello_Salvataggio.Visible = True


                '#########################################
                '############### MODIFICA ################
                '#########################################

            Case enum_TipoOperazioneDB.Modifica

                Me.Pannello_Salvataggio.Visible = True
                Me.ImgBtnSalvaNuovo.Visible = False
                Me.ImgBtnSalvaNuovo.Enabled = False


        End Select


        Select Case CAU_MOV


            Case CAU_CARICO

                Imposta_Compravendita_Acquisto()


            Case CAU_SCARICO

                Imposta_Compravendita_Vendita()


        End Select



    End Sub

    '####################################################################################
    Private Sub Imposta_Compravendita_Vendita()

        'PANNELLO MAGAZZINI MOVIMENTATI

        'SCARICO ---> attivata la provenienza

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("VenditaBeni"), String)
        'Me.Lbl_SalvaMagazzino.Text = "Salva l'operazione di Vendita"

        'Me.Cmb_Destinazione.SelectedIndex = 0
        Me.Cmb_Destinazione.Enabled = False
        Me.Cmb_Destinazione.Visible = False
        Me.lbl_MagDestinazione.Visible = False
        Me.lbl_giacenza2_mag.Visible = False
        Me.lbl_MagDestinazione.Visible = False
        Me.lbl_giacenza2_mag.Visible = False
        Me.Txt_Giacenza_Destinazione.Visible = False
        '   Me.Txt_Giacenza_Destinazione_Tot.Visible = False

        Me.Cmb_Provenienza.Enabled = True
        Me.Cmb_Provenienza.Visible = True
        Me.Chk_MovimentoMag.Visible = True
        Me.lbl_MagProvenienza.Visible = True
        Me.lbl_giacenza1_mag.Visible = True
        Me.Txt_Giacenza_Provenienza.Visible = True
        '   Me.Txt_Giacenza_Provenienza_Tot.Visible = True


    End Sub


    '####################################################################################
    Private Sub Imposta_Compravendita_Acquisto()

        'PANNELLO MAGAZZINI MOVIMENTATI

        'CARICO ---> attivata la destinazione

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("AcquistoBeni"), String)
        'Me.Lbl_SalvaMagazzino.Text = "Salva l'operazione di Acquisto"

        'Me.Cmb_Provenienza.SelectedIndex = 0
        Me.Cmb_Provenienza.Enabled = False
        Me.Cmb_Provenienza.Visible = False
        Me.Chk_MovimentoMag.Visible = False
        Me.Txt_Giacenza_Provenienza.Visible = False
        Me.lbl_MagProvenienza.Visible = False
        Me.lbl_giacenza1_mag.Visible = False
        '  Me.Txt_Giacenza_Provenienza_Tot.Visible = False

        Me.Cmb_Destinazione.Enabled = True
        Me.Cmb_Destinazione.Visible = True
        Me.lbl_MagDestinazione.Visible = True
        Me.lbl_giacenza2_mag.Visible = True
        Me.Txt_Giacenza_Destinazione.Visible = True
        '  Me.Txt_Giacenza_Destinazione_Tot.Visible = True


    End Sub



    '####################################################################################
    Private Sub Imposta_Conferimento(ByVal Operazione As Integer)




        Select Case Operazione

            '#########################################
            '################ LETTURA ################
            '#########################################

            Case enum_TipoOperazioneDB.Lettura


                '#########################################
                '############### SCRITTURA ###############
                '#########################################

            Case enum_TipoOperazioneDB.Scrittura




                '#########################################
                '############### MODIFICA ################
                '#########################################

            Case enum_TipoOperazioneDB.Modifica






        End Select




    End Sub


    '####################################################################################
    Private Sub Imposta_Trasferimento(ByVal Operazione As Integer)

        Me.Pannello_Prodotto.Visible = True

        Me.cmb_Causale.Enabled = False

        cmb_Causale.SelectedIndex =
            cmb_Causale.Items.IndexOf(
                cmb_Causale.Items.FindByValue(enum_Pendenza.Trasferimento))

        If cmb_Causale.SelectedValue = CStr(enum_Pendenza.Furto) Then
            'se sono in causale furto allora abilito le textbox per scrivere la denuncia
            denuncia.Visible = True
        Else
            denuncia.Visible = False
        End If

        Me.Txt_PendenzaIniziale.Text = enum_Pendenza.Trasferimento

        Me.cmb_Prodotti.SelectedIndex = 0
        Me.cmb_Lotto.SelectedIndex = 0
        Me.cmb_LottoAccettazione.SelectedIndex = 0
        Me.cmb_Calibro.SelectedIndex = 0
        Me.cmb_Udm.SelectedIndex = 0
        'Disabilito momentaneamente le combo del lotto e del calibro
        Me.cmb_Calibro.Enabled = False
        Me.cmb_Lotto.Enabled = False


        'PANNELLO DETTAGLIO
        Me.Txt_Quantita.Text = ""
        Me.Txt_PrezzoUnitario.Text = "0,00"


        Select Case Operazione

            '#########################################
            '################ LETTURA ################
            '#########################################

            Case enum_TipoOperazioneDB.Lettura

                Me.Pannello_InsGriglia.Visible = False
                Me.Pannello_Salvataggio.Visible = False

                Me.cmb_Calibro.Enabled = True
                Me.cmb_Lotto.Enabled = True
                Me.cmb_LottoAccettazione.Enabled = True


                '#########################################
                '############### SCRITTURA ###############
                '#########################################

            Case enum_TipoOperazioneDB.Scrittura


                'PANNELLO MOVIMENTI
                Me.Txt_DataMovimento.Text = Qs_DataSelezionata
                Me.Txt_Ora.Text = CDate(Qs_DataSelezionata).ToShortTimeString
                If Me.Txt_Ora.Text = "00.00" OrElse Me.Txt_Ora.Text = "0.00" Then
                    Me.Txt_Ora.Text = "12" & gettimesep() & "00"
                End If

                Me.Pannello_InsGriglia.Visible = False
                Me.Pannello_Salvataggio.Visible = True



                '#########################################
                '############### MODIFICA ################
                '#########################################

            Case enum_TipoOperazioneDB.Modifica

                Me.Pannello_InsGriglia.Visible = False
                Me.Pannello_Salvataggio.Visible = True

                Me.ImgBtnSalvaNuovo.Visible = False
                Me.ImgBtnSalvaNuovo.Enabled = False
        End Select


        'PANNELLO MAGAZZINI MOVIMENTATI

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = DirectCast(GetLocalResourceObject("TrasferimentoMerci"), String)
        'Me.Lbl_SalvaMagazzino.Text = "Salva il trasferimento"

        'Me.Cmb_Destinazione.SelectedIndex = 0
        Me.Cmb_Destinazione.Enabled = True
        Me.Cmb_Destinazione.Visible = True
        Me.lbl_MagDestinazione.Visible = True
        Me.lbl_giacenza2_mag.Visible = True
        Me.Txt_Giacenza_Destinazione.Visible = True
        '  Me.Txt_Giacenza_Destinazione_Tot.Visible = True

        Me.Cmb_Provenienza.Enabled = True
        Me.Cmb_Provenienza.Visible = True
        Me.Chk_MovimentoMag.Visible = True
        Me.Txt_Giacenza_Provenienza.Visible = True
        Me.lbl_MagProvenienza.Visible = True
        Me.lbl_giacenza1_mag.Visible = True
        ' Me.Txt_Giacenza_Provenienza_Tot.Visible = True



    End Sub

    '####################################################################################
    Private Sub Disattiva_Pulsanti()

        Me.Pannello_Cancellazione.Visible = False
        Me.Pannello_InsDocumento.Visible = False
        Me.Pannello_InsGriglia.Visible = False
        Me.Pannello_Salvataggio.Visible = False

        Me.ImgBtn_Inserisci_nel_DataGrid.Enabled = False
        Me.ImgBtn_Inserisci_nel_DataGrid.Visible = False
        Me.ImgBtnCancella.Enabled = False
        Me.ImgBtnCancella.Visible = False
        Me.ImgBtnInsDocumento.Enabled = False
        Me.ImgBtnInsDocumento.Visible = False
        Me.ImgBtnSalvaTutto.Enabled = False
        Me.ImgBtnSalvaTutto.Visible = False
        Me.ImgBtnSalvaNuovo.Enabled = False
        Me.ImgBtnSalvaNuovo.Visible = False
        Me.ImgBtn_CercaProdotti.Enabled = False
        Me.ImgBtn_CercaProdotti.Visible = False

    End Sub

    '####################################################################################
    Private Sub Disattiva_InsDocumento()

        Me.ImgBtnInsDocumento.Visible = False
        Me.ImgBtnInsDocumento.Enabled = False
        Me.lbl_InsDocumento.Visible = False

    End Sub

    '####################################################################################
    Private Sub Attiva_InsDocumento()

        Me.ImgBtnInsDocumento.Visible = True
        Me.ImgBtnInsDocumento.Enabled = True
        Me.lbl_InsDocumento.Visible = True

    End Sub

    '####################################################################################
    Private Sub Pulisci_DettagliEconomici()

        Me.Chk_IVAmanuale.Checked = False
        Chk_IVAmanuale_CheckedChanged(Me, Nothing)

        Me.Opt_PrezzoUnitario.Checked = True
        Me.Opt_Imponibile.Checked = False
        Me.Opt_Importo.Checked = False

        'Me.Txt_Quantita.Text = "0"
        Me.Txt_Imponibile.Text = ""
        Me.Txt_Imponibile_Netto.Text = ""
        Me.Txt_Importo.Text = ""
        Me.Txt_IVA.Text = ""
        Me.Txt_IVAmanuale.Text = ""
        'Me.Txt_MaggiorazionePercentuale.Text = "0"
        Me.Txt_Maggiorazione_Calcolato.Text = ""
        Me.Txt_Sconto_Calcolato.Text = ""
        'Me.Txt_ScontoPercentuale.Text = "0"

    End Sub

    '####################################################################################
    Private Sub Imposta_Pannelli(ByVal Operazione As Integer, ByVal Mode As String, ByVal CaricoScarico As Integer)

        Me.Pannello_Generale.Visible = False
        Me.Pannello_Prodotto.Visible = False
        Me.Pannello_Animale.Visible = False
        Me.Pannello_Dettaglio.Visible = True
        Me.Pannello_Economico.Visible = False
        Me.Pannello_Prodotti.Visible = False
        Me.Pannello_InsDocumento.Visible = False
        Me.Pannello_InsGriglia.Visible = False
        Me.Pannello_Salvataggio.Visible = False
        Me.Pannello_Cancellazione.Visible = False

        'Me.BtnInfo.Visible = False
        Me.BtnInfo_Concime.Visible = False
        Me.BtnInfo_fito.Visible = False

        '--------------------------------------------------------

        Dettaglio_Fertilizzante_Visibilita(False)

        Select Case Operazione


            '#########################################
            '############ CANCELLAZIONE ##############
            '#########################################

            Case enum_TipoOperazioneDB.Cancellazione


                Me.Pannello_Cancellazione.Visible = True


            Case Else

                Me.Pannello_Generale.Visible = True

                '#########################################
                '########### ALTRE OPERAZIONI ############
                '#########################################

                Select Case Mode.ToLower

                    Case "magazzino"

                        Imposta_MovimentoMagazzino(Operazione, CaricoScarico)

                        Datagrid_Imposta_Visibilita(False)

                        '===================================================================================


                    Case "bolla"

                        Imposta_DocumentoTrasporto(Operazione, CaricoScarico)

                        Datagrid_Imposta_Visibilita(False)

                        '===================================================================================


                    Case "fattura"

                        Imposta_Fattura(Operazione, CaricoScarico)

                        Datagrid_Imposta_Visibilita(True)

                        '===================================================================================


                    Case "compravendita"

                        Imposta_Compravendita(Operazione, CaricoScarico)

                        Datagrid_Imposta_Visibilita(True)

                        '===================================================================================

                    Case "conferimento"

                        Imposta_Conferimento(Operazione)

                        Datagrid_Imposta_Visibilita(False)

                        '===================================================================================


                    Case "trasferimento"

                        Imposta_Trasferimento(Operazione)

                        Datagrid_Imposta_Visibilita(False)

                        '===================================================================================


                    Case "stalla"

                        Imposta_MovimentoConsistenza(Operazione, CaricoScarico)

                        Datagrid_Imposta_Visibilita(False)

                        '===================================================================================


                End Select


        End Select



    End Sub

    '########################################################################################
    Private Sub Datagrid_Imposta_Visibilita(ByVal Flag_Visibilita As Boolean)

        Select Case Flag_Visibilita

            Case False

                Me.DataGrid_Prodotti.Columns.Item(COL_P_VARIAZIONE_PERC).Visible = False
                Me.DataGrid_Prodotti.Columns.Item(COL_P_VARIAZIONE).Visible = False
                Me.DataGrid_Prodotti.Columns.Item(COL_P_ALIQUOTA).Visible = False
                Me.DataGrid_Prodotti.Columns.Item(COL_P_IVA).Visible = False
                Me.DataGrid_Prodotti.Columns.Item(COL_P_IMPONIBILE).Visible = False
                Me.DataGrid_Prodotti.Columns.Item(COL_P_IMPONIBILE_NETTO).Visible = False
                Me.DataGrid_Prodotti.Columns.Item(COL_P_ANNO).Visible = False
                Me.DataGrid_Prodotti.Columns.Item(COL_P_CONTO).Visible = False


            Case True

                Me.DataGrid_Prodotti.Columns.Item(COL_P_VARIAZIONE_PERC).Visible = True
                Me.DataGrid_Prodotti.Columns.Item(COL_P_VARIAZIONE).Visible = True
                Me.DataGrid_Prodotti.Columns.Item(COL_P_ALIQUOTA).Visible = True
                Me.DataGrid_Prodotti.Columns.Item(COL_P_IVA).Visible = True
                Me.DataGrid_Prodotti.Columns.Item(COL_P_IMPONIBILE).Visible = True
                Me.DataGrid_Prodotti.Columns.Item(COL_P_IMPONIBILE_NETTO).Visible = True
                Me.DataGrid_Prodotti.Columns.Item(COL_P_ANNO).Visible = True
                Me.DataGrid_Prodotti.Columns.Item(COL_P_CONTO).Visible = True

        End Select


    End Sub


    '########################################################################################
    Private Sub ImgBtnCancella_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnCancella.Click

        Cancella_Tutto()

    End Sub


    '########################################################################################
    Private Sub Cancella_Tutto()

        Dim StringaXmlCancellazione As String
        Dim strDummy, Messaggio As String

        Try

            Dim objAgenda_R As New AgronicaCoreContabBIZ.Agenda_R  'New Agro_Contab.Agenda_R
            Dim objAgenda_W As New AgronicaCoreContabBIZ.Agenda_W 'New Agro_Contab.Agenda_W

            'Recupero la stringa XML di cancellazione
            StringaXmlCancellazione = objAgenda_R.Agenda_Leggi(
                                                        CStr(xPiva),
                                                        CInt(xSa_Cod),
                                                        CInt(xAgenda),
                                                        0,
                                                        CBool(True),
                                                        objParametri_Server)

            '===============================================

            'Cancello i dati esistenti
            Dim OUTPUT_ID_Agenda As Int32
            strDummy = objAgenda_W.Agenda_Scrivi(
                    StringaXmlCancellazione,
                    OUTPUT_ID_Agenda,
                    0,
                    0, 0, "", objParametri_Server)

            '===============================================


            EseguitaOperazione = True

            AAA_GestioneUscitaPagina()

            Select Case Qs_Lav_Cod

                Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_TRASFERIMENTO, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO

                Case Else

                    Dim strClose As String = "<script language='javascript'> window.close() </script>"
                    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

            End Select


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            ''Faccio abortire la transazione
            'System.EnterpriseServices.ContextUtil.SetAbort()

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore &
                        Chr(13) &
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)


        End Try


    End Sub


    '###############################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        If Not IsNothing(Request.QueryString("dialog")) Then
            Dim strClose As String = "<script language='javascript'>$(document).ready(function() { parent.chiudidialog(); }); </script>"

            Me.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        Else

            PremutoAnnulla = True

            AAA_GestioneUscitaPagina()


        End If





        ' '' ''se provenivo da qualche operazione d'agenda chiudo la finestra..
        '' ''Select Case Qs_Lav_Cod


        '' ''    Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_TRASFERIMENTO, _
        '' ''    LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, _
        '' ''    LAVCOD_VENDITA, LAVCOD_ACQUISTO

        '' ''        If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Menu Then
        '' ''            Response.Redirect("../Menu/Menu.aspx")
        '' ''        End If

        '' ''        If CInt(Qs_PagRitorno) = enum_PagineGiasOnline.FiltroMovContabili Then

        '' ''            Dim strClose As String = "<script language='javascript'> window.close()</script>"

        '' ''            Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

        '' ''        Else
        '' ''            '''Simulo il blocco della transazione
        '' ''            'System.EnterpriseServices.ContextUtil.SetAbort()

        '' ''            ''Assegno il flag di uscita dalla pagina
        '' ''            PremutoAnnulla = True

        '' ''            AAA_GestioneUscitaPagina()

        '' ''        End If


        '' ''    Case Else

        '' ''        Dim strClose As String = "<script language='javascript'> window.close()</script>"

        '' ''        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

        '' ''End Select


    End Sub



    '###############################################################################
    Private Sub ImgBtnInsDocumento_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnInsDocumento.Click

        Select Case Qs_Tipo
            Case CAU_MAGAZZINO

                ''TEMP
                'Me.SalvaEsci.Visible = True
                'Me.Lbl_SalvaEsci.Visible = True

                Disattiva_InsDocumento()

                Inserisci_Prodotto_nel_Documento()

            Case CAU_ANIMALE
                Inserisci_Animale_nel_Documento()
        End Select


    End Sub


    '###############################################################################
    Private Sub Inserisci_Animale_nel_Documento()

        Dim AlmenoUno As Boolean = False
        Dim Qta As Decimal = 0
        Dim Prezzo_Unitario As Decimal = 0
        Dim Prezzo_Unitario_Netto As Decimal = 0


        '------------------------------------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        Operazione = CInt(Qs_Operazione)

        Try


            Select Case Operazione

                Case enum_TipoOperazioneDB.Scrittura

                    Dim Dt As DataTable
                    Dim i As Integer


                    Dt = ViewState("vs_dtMatricole")

                    Messaggio = ""

                    '----- Deve essere inserita la data del movimento
                    If Me.Txt_DataMovimento.Text = "" Then
                        Messaggio = DirectCast(GetLocalResourceObject("InserireLaDataDelMovimento"), String) & vbCrLf
                    End If


                    Select Case xCaricoScarico
                        Case CAU_CARICO
                            If Me.Cmb_Destinazione.SelectedItem.Text = "" Then
                                Messaggio &= DirectCast(GetLocalResourceObject("SelezionareLaDestinazione"), String) & vbCrLf
                            End If
                        Case CAU_SCARICO
                            If Me.Cmb_Provenienza.SelectedItem.Text = "" Then
                                Messaggio &= DirectCast(GetLocalResourceObject("SelezionareLaProvenienza"), String) & vbCrLf
                            End If
                    End Select

                    For i = 0 To Me.DataGrid_ZooAnimali.Items.Count - 1

                        'Se la riga e' selezionata ...
                        If CType(Me.DataGrid_ZooAnimali.Items(i).FindControl("ChkSeleziona"), CheckBox).Checked Then

                            AlmenoUno = True

                            Qta = 0
                            Prezzo_Unitario = 0

                            '-----
                            'QUANTITA'
                            If CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text = "" Then
                                Messaggio &= DirectCast(GetLocalResourceObject("InserireLaQuantità"), String) & vbCrLf
                            Else
                                If Not IsNumeric(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text) Then
                                    Messaggio &= DirectCast(GetLocalResourceObject("LaQuantitàDeveEssereUnNumero"), String) & vbCrLf
                                    'ripulisco la textbox
                                    CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text = ""
                                    Exit Sub
                                Else
                                    If InStr(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text, ".") <> 0 Then
                                        CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text = Replace(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text, ".", ",")
                                    End If

                                    Qta = CDbl(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text)

                                    Dt.Rows(i).Item("Qta_Reale") = Qta

                                End If


                            End If

                            '-----
                            'PREZZO
                            If CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text <> "" Then
                                If Not IsNumeric(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text) Then
                                    Messaggio &= DirectCast(GetLocalResourceObject("IlPrezzoDeveEssereUnNumero"), String) & vbCrLf
                                    'ripulisco la textbox
                                    CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text = ""
                                    Exit Sub
                                Else
                                    If InStr(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text, ".") <> 0 Then
                                        CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text = Replace(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text, ".", ",")
                                    End If
                                    Prezzo_Unitario = CDbl(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text)

                                    Dt.Rows(i).Item("Prezzo_Reale") = Prezzo_Unitario

                                End If
                            End If

                        End If

                    Next

                    '----- Deve essere riempito il datagrid
                    If Not AlmenoUno Then
                        Messaggio &= DirectCast(GetLocalResourceObject("ScegliereAlmenoUnAnimale"), String) & vbCrLf
                    End If


                    'If Messaggio <> "" Then
                    '    Call  Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                    '    Exit Sub
                    'End If

                    ''----- Deve essere riempito il datagrid
                    'If Dt.Rows.Count = 0 Then
                    '    Messaggio &= "E' necessario scegliere delle consistenze!" & vbCrLf
                    'End If


                    If Messaggio <> "" Then

                        'Dim StringaMessaggio(0) As String

                        'StringaMessaggio(0) = "ERRORE" & Messaggio

                        'Session("vet_XML_To_Documento") = StringaMessaggio
                        'Exit Sub

                        Dim matrix_errore(0, 1) As String

                        matrix_errore(0, 0) = "ERRORE" & Messaggio
                        matrix_errore(0, 1) = "ERRORE" & Messaggio

                        Session("matrix_XML_To_Documento") = matrix_errore

                        Exit Sub

                    End If



                    '------------------------------------------------
                    '------------------------------------------------
                    '------------------------------------------------

                    'recupero le chiavi dal viewstate
                    xPiva = ViewState("Piva")
                    xSa_Cod = ViewState("Sa_Cod")
                    xAgenda = ViewState("id_agenda")

                    '------------------------------------------------
                    '------------------------------------------------
                    '------------------------------------------------

                    'Dim Lunghezza_Vettore As Integer
                    'Lunghezza_Vettore = 2 + Dt.Rows.Count - 1

                    'Dim StringaXmlCreazione(Lunghezza_Vettore) As String

                    'StringaXmlCreazione(0) = "XXXXXXXXXXXXXXXXXX"
                    'StringaXmlCreazione(1) = "INSERIMENTO"



                    'MATRICE matrix_for_doc DA PASSARE AL DOCUMENTO CHIAMANTE

                    'COLONNE
                    '   BASE                RIF
                    '   0                    1

                    'RIGHE
                    '0  ERRORE              ERRORE
                    '1  INSERIMENTO         INSERIMENTO
                    '2  DETTAGLIO           PIVA|SACOD|IDDEST|DEST DEL RIF
                    '3     "                    "
                    '4     "                    "
                    '5     "                    "
                    '6     "                    "
                    '7      ECC                 ECC

                    Dim Righe_Matrice As Integer

                    Righe_Matrice = 2 + Dt.Rows.Count - 1

                    Dim matrix_for_doc(Righe_Matrice, 1) As String

                    matrix_for_doc(0, 0) = "XXXXXXXXXXXXXXXXXX"
                    matrix_for_doc(0, 1) = "XXXXXXXXXXXXXXXXXX"

                    matrix_for_doc(1, 0) = "INSERIMENTO"
                    matrix_for_doc(1, 1) = "INSERIMENTO"

                    Dim j As Integer = 2
                    Dim temp As String = ""

                    For i = 0 To Me.DataGrid_ZooAnimali.Items.Count - 1

                        'Se la riga è selezionata ...
                        If CType(Me.DataGrid_ZooAnimali.Items(i).FindControl("ChkSeleziona"), CheckBox).Checked Then

                            temp = XML_Genera_StringoneFinale(Operazione, Dt.Rows(i))
                            matrix_for_doc(j, 0) = temp

                            Select Case xCaricoScarico

                                Case CAU_CARICO

                                    'matrix_for_doc(j, 1) = CStr(Dt.Rows(i).Item(str_COL_PIVA_PROV)) & "|" & _
                                    '                        CStr(Dt.Rows(i).Item(str_COL_SACOD_PROV)) & "|" & _
                                    '                        CStr(Dt.Rows(i).Item(str_COL_ID_PROV)) & "|" & _
                                    '                        CStr(Dt.Rows(i).Item(str_COL_PROVENIENZA))

                                Case CAU_SCARICO

                                    'matrix_for_doc(j, 1) = CStr(Dt.Rows(i).Item(str_COL_PIVA_DEST)) & "|" & _
                                    '                       CStr(Dt.Rows(i).Item(str_COL_SACOD_DEST)) & "|" & _
                                    '                       CStr(Dt.Rows(i).Item(str_COL_ID_DEST)) & "|" & _
                                    '                       CStr(Dt.Rows(i).Item(str_COL_DESTINAZIONE))

                                Case Else
                                    matrix_for_doc(j, 1) = "|||"


                            End Select

                            j += 1

                        End If

                    Next


                    'Session("vet_XML_To_Documento") = StringaXmlCreazione
                    'Session("matrix_XML_To_Documento") = StringaXmlCreazione
                    Session("matrix_XML_To_Documento") = matrix_for_doc



                    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


                Case enum_TipoOperazioneDB.Modifica

                    Dim Str_MovDettaglio As String


                    Messaggio = ""


                    If Messaggio <> "" Then

                        'Dim StringaMessaggio(0) As String

                        'StringaMessaggio(0) = "ERRORE" & Messaggio

                        'Session("matrix_XML_To_Documento") = StringaMessaggio
                        'Exit Sub


                        Dim matrix_errore(0, 1) As String

                        matrix_errore(0, 0) = "ERRORE" & Messaggio
                        matrix_errore(0, 1) = "ERRORE" & Messaggio

                        Session("matrix_XML_To_Documento") = matrix_errore
                        Exit Sub

                    End If



                    '------------------------------------------------
                    '------------------------------------------------
                    '------------------------------------------------


                    Dim XmlDoc As New XmlDocument
                    Dim XML_MovimentoDettaglio As XmlElement


                    Str_MovDettaglio = Session("vet_XML_To_FormProdotto")(1)


                    'Carico la stringa nel documento XML
                    XmlDoc.LoadXml(Str_MovDettaglio)


                    '-----------------------------------------------------------------
                    '------------------------- DETTAGLIO ------------------------------
                    '-----------------------------------------------------------------

                    XML_MovimentoDettaglio = XmlDoc.SelectSingleNode("Movimento_Dettaglio")



                    '-----------------------------------------------------------------
                    '--------------------- QUANTITA' E PREZZO ------------------------
                    '-----------------------------------------------------------------

                    XML_MovimentoDettaglio.SetAttribute("qta", Qta)

                    XML_MovimentoDettaglio.SetAttribute("prezzo_unitario", Prezzo_Unitario)
                    XML_MovimentoDettaglio.SetAttribute("prezzo_unitario_netto", Prezzo_Unitario_Netto)



                    'Dim StringaXmlCreazione(2) As String

                    'StringaXmlCreazione(0) = "XXXXXXXXXXXXXXXXXX"

                    ''StringaXmlCreazione(1) = "MODIFICA" & Session("vet_XML_To_FormProdotto").split("|")(0)
                    'StringaXmlCreazione(1) = "MODIFICA" & Session("vet_XML_To_FormProdotto")(0)

                    'StringaXmlCreazione(2) = XML_MovimentoDettaglio.OuterXml



                    'MATRICE matrix_for_doc DA PASSARE AL DOCUMENTO CHIAMANTE

                    'COLONNE
                    '   BASE                RIF
                    '   0                    1

                    'RIGHE
                    '0  ERRORE              ERRORE
                    '1  MODIFICA+CHIAVE     MODIFICA+CHIAVE
                    '2  DETTAGLIO           PIVA|SACOD|IDDEST|DEST DEL RIF

                    Dim matrix_for_doc(2, 1) As String

                    matrix_for_doc(0, 0) = "XXXXXXXXXXXXXXXXXX"
                    matrix_for_doc(0, 1) = "XXXXXXXXXXXXXXXXXX"

                    matrix_for_doc(1, 0) = "MODIFICA" & Session("vet_XML_To_FormProdotto")(0)
                    matrix_for_doc(1, 1) = "MODIFICA" & Session("vet_XML_To_FormProdotto")(0)

                    matrix_for_doc(2, 0) = XML_MovimentoDettaglio.OuterXml

                    If CStr(Session("vet_XML_To_FormProdotto")(2)) <> "" AndAlso
                        CStr(Session("vet_XML_To_FormProdotto")(2)) <> "&nbsp;" Then

                        If Qs_CodContatto <> "" Then

                            Dim str_rif As String = ""

                            Select Case xCaricoScarico

                                Case CAU_CARICO

                                    'str_rif = Qs_CodContatto & "|" & _
                                    '                        Me.Cmb_Provenienza.SelectedValue.Split("|")(1) & "|" & _
                                    '                        Me.Cmb_Provenienza.SelectedValue.Split("|")(0) & "|" & _
                                    '                        Me.Cmb_Provenienza.SelectedItem.Text

                                Case CAU_SCARICO

                                    'str_rif = Qs_CodContatto & "|" & _
                                    '                        Me.Cmb_Destinazione.SelectedValue.Split("|")(1) & "|" & _
                                    '                        Me.Cmb_Destinazione.SelectedValue.Split("|")(0) & "|" & _
                                    '                        Me.Cmb_Destinazione.SelectedItem.Text

                                Case Else
                                    str_rif = "|||" 'errore

                            End Select

                            matrix_for_doc(2, 1) = str_rif 'da valorizzare

                        Else
                            matrix_for_doc(2, 1) = "|||" 'errore
                        End If

                    Else
                        If Qs_CodContatto <> "" Then
                            matrix_for_doc(2, 1) = "|||" 'errore
                        Else
                            matrix_for_doc(2, 1) = "" 'non è impostato il contatto
                        End If
                    End If

                    Session("matrix_XML_To_Documento") = matrix_for_doc



                    '===========================================================



            End Select

            Me.Txt_DaInviare.Text = "999"

            Dim strClose As String = "<SCRIPT language='javascript'> " &
                                    "window.returnValue = document.all('Txt_DaInviare').value; " &
                                    "window.close(); " &
                                    "</SCRIPT>"

            Me.Controls.Add(New LiteralControl(strClose))


            '===============================================


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore &
                        Chr(13) &
                        exc.Message.ToString()


            'preparo il messaggio di errore nella sessione
            'per la comunicazione alla bolla

            Dim matrix_errore(0, 1) As String

            matrix_errore(0, 0) = "ERRORE" & Messaggio
            matrix_errore(0, 1) = "ERRORE" & Messaggio

            Session("matrix_XML_To_Documento") = matrix_errore


            ''Faccio abortire la transazione
            'System.EnterpriseServices.ContextUtil.SetAbort()


        End Try



    End Sub




    '###############################################################################
    Private Sub Inserisci_Prodotto_nel_Documento()

        '------------------------------------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        Operazione = CInt(Qs_Operazione)


        Try

            Dim Unid As String = ""
            Dim NumeroRecordInteressati As Integer = 0

            Select Case Qs_Lav_Cod

                'TODO, vanni, ricomprese le bolle come fatture ...
                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA


                    Select Case Operazione

                        Case enum_TipoOperazioneDB.Scrittura

                            Dim Dt As DataTable
                            Dim i As Integer

                            Dt = ViewState("DT_Prodotti_NO_Contabili")

                            Messaggio = ""

                            '----- Deve essere inserita la data del movimento
                            If Me.Txt_DataMovimento.Text = "" Then
                                Messaggio = DirectCast(GetLocalResourceObject("InserireLaDataDelMovimento"), String) & vbCrLf
                            End If

                            '----- Deve essere riempito il datagrid
                            If Dt.Rows.Count = 0 Then
                                Messaggio &= DirectCast(GetLocalResourceObject("InserireProdottoNelRiepilogoPrimaDeiDettagli"), String) & vbCrLf
                            End If

                            If Messaggio = "" Then

                                ''recupero le chiavi dal viewstate
                                'xPiva = viewstate("Piva")
                                'xSa_Cod = viewstate("Sa_Cod")
                                'xAgenda = viewstate("id_agenda")

                                '------------------------------------------------
                                '------------------------------------------------
                                '------------------------------------------------
                                'MATRICE matrix_for_doc DA PASSARE AL DOCUMENTO CHIAMANTE

                                'COLONNE
                                '   BASE                RIF
                                '   0                    1

                                'RIGHE
                                '0  ERRORE              ERRORE
                                '1  INSERIMENTO         INSERIMENTO
                                '2  DETTAGLIO           PIVA|SACOD|IDDEST|DEST DEL RIF
                                '3     "                    "
                                '4     "                    "
                                '5     "                    "
                                '6     "                    "
                                '7      ECC                 ECC

                                Dim Stringa_Parametri_Impresa As String = ""
                                Dim Stringa_Parametri_Contatto As String = ""

                                Unid = System.Guid.NewGuid.ToString
                                NumeroRecordInteressati = 0

                                For i = 0 To Dt.Rows.Count - 1

                                    Stringa_Parametri_Impresa = XML_Genera_StringoneFinale(Operazione, Dt.Rows(i))

                                    'riferimento
                                    Select Case xCaricoScarico

                                        Case CAU_CARICO

                                            Stringa_Parametri_Contatto = CStr(Dt.Rows(i).Item(str_COL_PIVA_PROV)) & "|" &
                                                                    CStr(Dt.Rows(i).Item(str_COL_SACOD_PROV)) & "|" &
                                                                    CStr(Dt.Rows(i).Item(str_COL_ID_PROV)) & "|" &
                                                                    CStr(Dt.Rows(i).Item(str_COL_PROVENIENZA))

                                        Case CAU_SCARICO

                                            Stringa_Parametri_Contatto = CStr(Dt.Rows(i).Item(str_COL_PIVA_DEST)) & "|" &
                                                                   CStr(Dt.Rows(i).Item(str_COL_SACOD_DEST)) & "|" &
                                                                   CStr(Dt.Rows(i).Item(str_COL_ID_DEST)) & "|" &
                                                                   CStr(Dt.Rows(i).Item(str_COL_DESTINAZIONE))

                                        Case Else
                                            Stringa_Parametri_Contatto = "|||"


                                    End Select


                                    Try

                                        NewCom_Web_ComunicazionePagine_Scrivi(objParametri_Server, Session, Page, NumeroRecordInteressati, Unid, i, enum_TipoOperazioneDB.Scrittura, 0, "", 0, Stringa_Parametri_Impresa, Stringa_Parametri_Contatto)


                                    Catch ex As Exception

                                        Messaggi.AgroMsgBox(
                                            String.Format(DirectCast(GetLocalResourceObject("ErroreInserimentoEnnesimoDettagli"), String), CStr(i)) & ex.Message,
                                            Page, , Me.upDati
                                        )
                                        Exit For
                                        Exit Sub

                                    End Try


                                Next


                            Else

                                Attiva_InsDocumento()
                                Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                Exit Sub

                            End If

                            '------------------------------------------------
                            '------------------------------------------------
                            '------------------------------------------------


                            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


                        Case enum_TipoOperazioneDB.Modifica

                            Dim Qta, Prezzo_Unitario, Prezzo_Unitario_Netto As Decimal
                            Dim Str_MovDettaglio As String

                            Messaggio = ""

                            '-----
                            'QUANTITA'
                            VerificaValorizza_Quantita(Qta)


                            '-----
                            'PREZZO
                            VerificaValorizza_PrezzoUnitario(Prezzo_Unitario)


                            'PREZZO Unitario_Netto gestito dopo



                            'più controlli sugli altri dettagli contabili

                            'NO Prezzo_Unitario_Netto = Prezzo_Unitario

                            'Select Case Me.Txt_PrezzoUnitario_Netto.Text
                            '    Case "", "0", "0,0", "0,00"
                            '        Prezzo_Unitario_Netto = Prezzo_Unitario
                            '    Case Else
                            '        Prezzo_Unitario_Netto = CDbl(Txt_PrezzoUnitario_Netto.Text)
                            'End Select

                            '------------------------------------------------
                            '------------------------------------------------
                            '------------------------------------------------

                            Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita

                            If Messaggio = "" Then

                                Dim XmlDoc As New XmlDocument
                                Dim XML_MovimentoDettaglio As XmlElement


                                Str_MovDettaglio = Session("vet_XML_To_FormProdotto")(1)


                                'Carico la stringa nel documento XML
                                XmlDoc.LoadXml(Str_MovDettaglio)


                                '-----------------------------------------------------------------
                                '------------------------- DETTAGLIO ------------------------------
                                '-----------------------------------------------------------------

                                XML_MovimentoDettaglio = XmlDoc.SelectSingleNode("Movimento_Dettaglio")

                                Dim XML_Movimento_Destinazione As XmlElement = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Destinazione")

                                '-----------------------------------------------------------------
                                '--------------------- QUANTITA' E PREZZO ------------------------
                                '-----------------------------------------------------------------

                                XML_MovimentoDettaglio.SetAttribute("qta", Qta)

                                'devo modificare la qta anche nella destinazione perché è li che si legge per le giacenze
                                XML_Movimento_Destinazione.SetAttribute("qta", Qta)

                                'prezzi netti gestiti poi gestiti poi a seconda della visibilità del pannello dati economici
                                XML_MovimentoDettaglio.SetAttribute("prezzo_unitario", Prezzo_Unitario)


                                '===================================================
                                Dim Imponibile As Decimal = 0
                                Dim Imponibile_Netto As Decimal = 0

                                'DETTAGLI ECONOMICI
                                If Me.Pannello_Economico.Visible Then

                                    'per prima cosa aggiorno dettagli economici per aggiornare il prezzo etc
                                    Dim Str_Errore As String
                                    If Not UsaNuoviArrotondamenti(objParametri_Server) Then
                                        Str_Errore = AggiornaDettagliEconomici()
                                    Else
                                        Str_Errore = AggiornaDettagliEconomiciNuoviArrotondamenti()
                                    End If
                                    If Str_Errore <> "" Then
                                        Messaggi.AgroMsgBox(Str_Errore, Page, , Me.upDati)
                                        Exit Sub
                                    End If

                                    Dim Udm_Cod_Extra As Decimal = 0
                                    Dim Qta_Extra As Decimal = 0
                                    Dim Prezzo_Effettivo As Decimal = 0
                                    Dim Sconto_Magg As Decimal = 0
                                    Dim Variazione_Perc As String = ""
                                    Dim Variazione As Decimal = 0
                                    Dim Cod_Iva As Integer = 0
                                    Dim Iva As Decimal = 0
                                    Dim Anno As Integer = 0
                                    Dim Ric_Cod As Integer = 0
                                    Dim Cod_Conto As Integer = 0
                                    Dim Conto As String = ""

                                    Recupera_DettagliEconomici(Udm_Cod_Extra,
                                                                Qta_Extra,
                                                                Prezzo_Effettivo,
                                                                Sconto_Magg,
                                                                Variazione_Perc,
                                                                Variazione,
                                                                Imponibile,
                                                                Imponibile_Netto,
                                                                Cod_Iva,
                                                                Iva,
                                                                Anno,
                                                                Ric_Cod,
                                                                Cod_Conto,
                                                                Conto)

                                    XML_MovimentoDettaglio.SetAttribute(LCase("Udm_Cod_Extra"), Udm_Cod_Extra)
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Qta_Extra"), Qta_Extra)
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Prezzo_Effettivo"), Prezzo_Effettivo)
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Sconto"), Sconto_Magg)
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Imponibile"), objContabHLP.Leggi_Imponibile_PositivoNegativo(Qs_Lav_Cod, Imponibile))
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Imponibile_Netto"), objContabHLP.Leggi_Imponibile_PositivoNegativo(Qs_Lav_Cod, Imponibile_Netto))
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Cod_Iva"), Cod_Iva)
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Iva"), objContabHLP.Leggi_IVA_PositivaNegativa(Qs_Lav_Cod, Iva))
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Anno"), Anno)
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Ric_Cod"), Ric_Cod)
                                    XML_MovimentoDettaglio.SetAttribute(LCase("Cod_Conto"), Cod_Conto)

                                    'Prezzo_Unitario_Netto = Imponibile_Netto / Qta
                                    'se ho pannello economico dovrei averlo calcolato con calcola importi
                                    If Me.Txt_PrezzoUnitario_Netto.Text <> "" Then
                                        If Not IsNumeric(Me.Txt_PrezzoUnitario_Netto.Text) Then
                                            Messaggio &= DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerIlPrezzo"), String)
                                            'Call  Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                            'Exit Sub
                                        Else
                                            If Me.Txt_PrezzoUnitario_Netto.Text < 0 Then
                                                Messaggio &= DirectCast(GetLocalResourceObject("ImpossibileInserirePrezzoNegativo"), String)
                                                'Call  Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                                'Exit Sub
                                            Else
                                                If InStr(Me.Txt_PrezzoUnitario_Netto.Text, ".") <> 0 Then
                                                    Txt_PrezzoUnitario_Netto.Text = Replace(Txt_PrezzoUnitario_Netto.Text, ".", ",")
                                                End If
                                                Prezzo_Unitario_Netto = CDbl(Txt_PrezzoUnitario_Netto.Text)
                                            End If
                                        End If
                                    Else
                                        Prezzo_Unitario_Netto = CDbl(0)
                                    End If


                                    XML_MovimentoDettaglio.SetAttribute("prezzo_unitario_netto", Prezzo_Unitario_Netto)

                                Else

                                    Prezzo_Unitario_Netto = Prezzo_Unitario
                                    XML_MovimentoDettaglio.SetAttribute("prezzo_unitario_netto", Prezzo_Unitario_Netto)

                                    Imponibile = Qta * Prezzo_Unitario
                                    Imponibile_Netto = Qta * Prezzo_Unitario_Netto

                                    'Dim Segno As String

                                    'Select Case xCaricoScarico
                                    '    Case CAU_CARICO
                                    '        Segno = "-" 'acquisto - fattura ricevuta
                                    '    Case CAU_CONFERIMENTO
                                    '        Segno = "-" 'costo: ricevi da un socio
                                    '    Case CAU_CONFERIMENTO_DIVERSI
                                    '        Segno = "+" 'ricavo: vendi a esterni
                                    '    Case CAU_ACCETTAZIONE_BENI
                                    '        Segno = "-"             '?
                                    '    Case CAU_SCARICO
                                    '        Segno = "+" ' vendita - fattura emessa
                                    '    Case CAU_TRASFERIMENTO
                                    '        Segno = "+" 'Fittizio    ?
                                    '    Case Else
                                    '        Segno = "+" 'default
                                    'End Select
                                    'XML_MovimentoDettaglio.SetAttribute("imponibile", CStr(CDbl(Segno & 1) * CDbl(Imponibile)))
                                    'XML_MovimentoDettaglio.SetAttribute("imponibile_netto", CStr(CDbl(Segno & 1) * CDbl(Imponibile_Netto)))

                                    XML_MovimentoDettaglio.SetAttribute("imponibile", CStr(objContabHLP.Leggi_Imponibile_PositivoNegativo(Qs_Lav_Cod, Imponibile)))
                                    XML_MovimentoDettaglio.SetAttribute("imponibile_netto", CStr(objContabHLP.Leggi_Imponibile_PositivoNegativo(Qs_Lav_Cod, Imponibile_Netto)))


                                End If
                                '============================================================
                                '-------------------------------------------------------

                                'MATRICE matrix_for_doc DA PASSARE AL DOCUMENTO CHIAMANTE

                                'COLONNE
                                '   BASE                RIF
                                '   0                    1

                                'RIGHE
                                '0  ERRORE              ERRORE
                                '1  MODIFICA+CHIAVE     MODIFICA+CHIAVE
                                '2  DETTAGLIO           PIVA|SACOD|IDDEST|DEST DEL RIF

                                Dim Stringa_Parametri_Impresa As String = ""
                                Dim Stringa_Parametri_Contatto As String = ""


                                Stringa_Parametri_Impresa = XML_MovimentoDettaglio.OuterXml

                                If CStr(Session("vet_XML_To_FormProdotto")(2)) <> "" AndAlso
                                    CStr(Session("vet_XML_To_FormProdotto")(2)) <> "&nbsp;" Then

                                    If Qs_CodContatto <> "" Then

                                        Select Case xCaricoScarico

                                            Case CAU_CARICO

                                                Stringa_Parametri_Contatto = Qs_CodContatto & "|" &
                                                                        Me.Cmb_Provenienza.SelectedValue.Split("|")(1) & "|" &
                                                                        Me.Cmb_Provenienza.SelectedValue.Split("|")(0) & "|" &
                                                                        Me.Cmb_Provenienza.SelectedItem.Text

                                            Case CAU_SCARICO

                                                Stringa_Parametri_Contatto = Qs_CodContatto & "|" &
                                                                        Me.Cmb_Destinazione.SelectedValue.Split("|")(1) & "|" &
                                                                        Me.Cmb_Destinazione.SelectedValue.Split("|")(0) & "|" &
                                                                        Me.Cmb_Destinazione.SelectedItem.Text

                                            Case Else
                                                Stringa_Parametri_Contatto = "|||" 'errore

                                        End Select

                                    Else
                                        Stringa_Parametri_Contatto = "|||" 'errore
                                    End If

                                Else
                                    If Qs_CodContatto <> "" Then
                                        Stringa_Parametri_Contatto = "|||" 'errore
                                    Else
                                        Stringa_Parametri_Contatto = "" 'non è impostato il contatto
                                    End If
                                End If


                                Try

                                    Unid = System.Guid.NewGuid.ToString
                                    NumeroRecordInteressati = 0

                                    NewCom_Web_ComunicazionePagine_Scrivi(objParametri_Server, Session, Page, NumeroRecordInteressati, Unid, 0, enum_TipoOperazioneDB.Modifica, 0, "", Session("vet_XML_To_FormProdotto")(0), Stringa_Parametri_Impresa, Stringa_Parametri_Contatto)

                                Catch ex As Exception

                                    Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ErroreInModificaDelDettaglio"), String) & ex.Message, Page, , Me.upDati)
                                    Exit Sub

                                End Try


                            Else
                                Attiva_InsDocumento()
                                Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                Exit Sub

                            End If

                    End Select

                    '===========================================================

                    ''viewstate("Txt_DaInviare") = Unid

                    Me.Txt_DaInviare.Text = Unid

                    Dim strClose As String = "parent.settavalore(' " & Unid & "', 'prodotto');"

                    'Me.Controls.Add(New LiteralControl(strClose))

                    'update panel
                    ScriptManager.RegisterClientScriptBlock(upDati, upDati.GetType(),
                                         String.Format("jQuery_{0}", "chiudimi"), strClose, True)




                    '/////////////////////////////////////////////////
                    '/////////////////////////////////////////////////
                    '/////////////////////////////////////////////////


                Case Else 'DDT


                    Select Case Operazione

                        Case enum_TipoOperazioneDB.Scrittura

                            Dim Dt As DataTable
                            Dim i As Integer


                            Dt = ViewState("DT_Prodotti_NO_Contabili")

                            Messaggio = ""

                            '----- Deve essere inserita la data del movimento
                            If Me.Txt_DataMovimento.Text = "" Then
                                Messaggio = DirectCast(GetLocalResourceObject("InserireLaDataDelMovimento"), String) & vbCrLf
                                'Call  Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                'Exit Sub
                            End If


                            '----- Deve essere riempito il datagrid
                            If Dt.Rows.Count = 0 Then
                                Messaggio &= DirectCast(GetLocalResourceObject("InserireProdottoNelRiepilogoPrimaDeiDettagli"), String) & vbCrLf
                                'Call  Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                'Exit Sub
                            End If

                            If Messaggio <> "" Then

                                'Dim StringaMessaggio(0) As String

                                'StringaMessaggio(0) = "ERRORE" & Messaggio

                                'Session("vet_XML_To_Documento") = StringaMessaggio
                                'Exit Sub

                                Dim matrix_errore(0, 1) As String

                                matrix_errore(0, 0) = "ERRORE" & Messaggio
                                matrix_errore(0, 1) = "ERRORE" & Messaggio

                                Session("matrix_XML_To_Documento") = matrix_errore

                                Exit Sub

                            End If



                            '------------------------------------------------
                            '------------------------------------------------
                            '------------------------------------------------

                            'recupero le chiavi dal viewstate
                            xPiva = ViewState("Piva")
                            xSa_Cod = ViewState("Sa_Cod")
                            xAgenda = ViewState("id_agenda")

                            '------------------------------------------------
                            '------------------------------------------------
                            '------------------------------------------------

                            'Dim StringaXmlCreazione(Lunghezza_Vettore) As String
                            'StringaXmlCreazione(0) = "XXXXXXXXXXXXXXXXXX"
                            'StringaXmlCreazione(1) = "INSERIMENTO"


                            'MATRICE matrix_for_doc DA PASSARE AL DOCUMENTO CHIAMANTE

                            'COLONNE
                            '   BASE                RIF
                            '   0                    1

                            'RIGHE
                            '0  ERRORE              ERRORE
                            '1  INSERIMENTO         INSERIMENTO
                            '2  DETTAGLIO           PIVA|SACOD|IDDEST|DEST DEL RIF
                            '3     "                    "
                            '4     "                    "
                            '5     "                    "
                            '6     "                    "
                            '7      ECC                 ECC

                            Dim Righe_Matrice As Integer

                            Righe_Matrice = 2 + Dt.Rows.Count - 1

                            Dim matrix_for_doc(Righe_Matrice, 1) As String

                            matrix_for_doc(0, 0) = "XXXXXXXXXXXXXXXXXX"
                            matrix_for_doc(0, 1) = "XXXXXXXXXXXXXXXXXX"

                            matrix_for_doc(1, 0) = "INSERIMENTO"
                            matrix_for_doc(1, 1) = "INSERIMENTO"

                            Dim j As Integer = 2
                            Dim temp As String = ""
                            'Dim XmlDoc As XmlDocument
                            'Dim Movimento_Dettaglio As XmlElement

                            For i = 0 To Dt.Rows.Count - 1

                                'StringaXmlCreazione(j) = XML_Genera_StringoneFinale(Operazione, Dt.Rows(i))

                                temp = XML_Genera_StringoneFinale(Operazione, Dt.Rows(i))
                                matrix_for_doc(j, 0) = temp

                                'in posizione 1 si genera eventualmente il riferimento
                                'If temp <> "" Then
                                'XmlDoc.LoadXml(temp)
                                'Movimento_Dettaglio = XmlDoc.SelectSingleNode("Movimento_Dettaglio")

                                'End If

                                Select Case xCaricoScarico

                                    Case CAU_CARICO

                                        matrix_for_doc(j, 1) = CStr(Dt.Rows(i).Item(str_COL_PIVA_PROV)) & "|" &
                                                                CStr(Dt.Rows(i).Item(str_COL_SACOD_PROV)) & "|" &
                                                                CStr(Dt.Rows(i).Item(str_COL_ID_PROV)) & "|" &
                                                                CStr(Dt.Rows(i).Item(str_COL_PROVENIENZA))

                                    Case CAU_SCARICO

                                        matrix_for_doc(j, 1) = CStr(Dt.Rows(i).Item(str_COL_PIVA_DEST)) & "|" &
                                                               CStr(Dt.Rows(i).Item(str_COL_SACOD_DEST)) & "|" &
                                                               CStr(Dt.Rows(i).Item(str_COL_ID_DEST)) & "|" &
                                                               CStr(Dt.Rows(i).Item(str_COL_DESTINAZIONE))

                                    Case Else
                                        matrix_for_doc(j, 1) = "|||"


                                End Select

                                j += 1

                            Next

                            'Session("vet_XML_To_Documento") = StringaXmlCreazione
                            Session("matrix_XML_To_Documento") = matrix_for_doc



                            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


                        Case enum_TipoOperazioneDB.Modifica

                            Dim Qta, Prezzo_Unitario, Prezzo_Unitario_Netto As Decimal
                            Dim Str_MovDettaglio As String


                            Messaggio = ""

                            '-----
                            'QUANTITA'
                            VerificaValorizza_Quantita(Qta)


                            '-----
                            'PREZZO
                            VerificaValorizza_PrezzoUnitario(Prezzo_Unitario)


                            Prezzo_Unitario_Netto = Prezzo_Unitario

                            'Select Case Me.Txt_PrezzoUnitario_Netto.Text
                            '    Case "", "0", "0,0", "0,00"
                            '        Prezzo_Unitario_Netto = Prezzo_Unitario
                            '    Case Else
                            '        Prezzo_Unitario_Netto = CDbl(Txt_PrezzoUnitario_Netto.Text)
                            'End Select


                            'If Messaggio <> "" Then

                            '    Dim StringaMessaggio(0) As String

                            '    StringaMessaggio(0) = "ERRORE" & Messaggio

                            '    Session("vet_XML_To_Documento") = StringaMessaggio
                            '    Exit Sub

                            'End If

                            If Messaggio <> "" Then

                                Dim matrix_errore(0, 1) As String

                                matrix_errore(0, 0) = "ERRORE" & Messaggio
                                matrix_errore(0, 1) = "ERRORE" & Messaggio

                                Session("matrix_XML_To_Documento") = matrix_errore
                                Exit Sub

                            End If


                            '------------------------------------------------
                            '------------------------------------------------
                            '------------------------------------------------


                            Dim XmlDoc As New XmlDocument
                            Dim XML_MovimentoDettaglio As XmlElement


                            'Str_MovDettaglio = Session("vet_XML_To_FormProdotto").split("|")(1)
                            Str_MovDettaglio = Session("vet_XML_To_FormProdotto")(1)


                            'Carico la stringa nel documento XML
                            XmlDoc.LoadXml(Str_MovDettaglio)


                            '-----------------------------------------------------------------
                            '------------------------- DETTAGLIO ------------------------------
                            '-----------------------------------------------------------------

                            XML_MovimentoDettaglio = XmlDoc.SelectSingleNode("Movimento_Dettaglio")



                            '-----------------------------------------------------------------
                            '--------------------- QUANTITA' E PREZZO ------------------------
                            '-----------------------------------------------------------------

                            XML_MovimentoDettaglio.SetAttribute("qta", Qta)

                            XML_MovimentoDettaglio.SetAttribute("prezzo_unitario", Prezzo_Unitario)
                            XML_MovimentoDettaglio.SetAttribute("prezzo_unitario_netto", Prezzo_Unitario_Netto)

                            '===================================================
                            Dim Imponibile As Decimal = 0
                            Dim Imponibile_Netto As Decimal = 0

                            'DETTAGLI ECONOMICI
                            If Me.Pannello_Economico.Visible Then

                                Dim Udm_Cod_Extra As Decimal = 0
                                Dim Qta_Extra As Decimal = 0
                                Dim Prezzo_Effettivo As Decimal = 0
                                Dim Sconto_Magg As Decimal = 0
                                Dim Variazione_Perc As String = ""
                                Dim Variazione As Decimal = 0
                                Dim Cod_Iva As Integer = 0
                                Dim Iva As Decimal = 0
                                Dim Anno As Integer = 0
                                Dim Ric_Cod As Integer = 0
                                Dim Cod_Conto As Integer = 0
                                Dim Conto As String = ""

                                Recupera_DettagliEconomici(
                                                        Udm_Cod_Extra,
                                                        Qta_Extra,
                                                        Prezzo_Effettivo,
                                                        Sconto_Magg,
                                                        Variazione_Perc,
                                                        Variazione,
                                                        Imponibile,
                                                        Imponibile_Netto,
                                                        Cod_Iva,
                                                        Iva,
                                                        Anno,
                                                        Ric_Cod,
                                                        Cod_Conto,
                                                        Conto)

                                XML_MovimentoDettaglio.SetAttribute(LCase("Udm_Cod_Extra"), Udm_Cod_Extra)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Qta_Extra"), Qta_Extra)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Prezzo_Effettivo"), Prezzo_Effettivo)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Sconto"), Sconto_Magg)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Imponibile"), Imponibile)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Imponibile_Netto"), Imponibile_Netto)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Cod_Iva"), Cod_Iva)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Iva"), Iva)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Anno"), Anno)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Ric_Cod"), Ric_Cod)
                                XML_MovimentoDettaglio.SetAttribute(LCase("Cod_Conto"), Cod_Conto)

                            Else

                                Imponibile = Qta * Prezzo_Unitario
                                Imponibile_Netto = Qta * Prezzo_Unitario_Netto

                                Dim Segno As String

                                Select Case xCaricoScarico
                                    Case CAU_CARICO
                                        Segno = "-" 'acquisto - fattura ricevuta
                                    Case CAU_CONFERIMENTO
                                        Segno = "-" 'costo: ricevi da un socio
                                    Case CAU_CONFERIMENTO_DIVERSI
                                        Segno = "+" 'ricavo: vendi a esterni
                                    Case CAU_ACCETTAZIONE_BENI
                                        Segno = "-"             '?
                                    Case CAU_SCARICO
                                        Segno = "+" ' vendita - fattura emessa
                                    Case CAU_TRASFERIMENTO
                                        Segno = "+" 'Fittizio    ?
                                    Case Else
                                        Segno = "+" 'default
                                End Select

                                'XML_MovimentoDettaglio.SetAttribute("imponibile", CStr(Imponibile))
                                'XML_MovimentoDettaglio.SetAttribute("imponibile_netto", CStr(Imponibile_Netto))
                                XML_MovimentoDettaglio.SetAttribute("imponibile", CStr(CDbl(Segno & 1) * CDbl(Imponibile)))
                                XML_MovimentoDettaglio.SetAttribute("imponibile_netto", CStr(CDbl(Segno & 1) * CDbl(Imponibile_Netto)))

                            End If
                            '============================================================



                            'Dim StringaXmlCreazione(2) As String

                            'StringaXmlCreazione(0) = "XXXXXXXXXXXXXXXXXX"

                            ''StringaXmlCreazione(1) = "MODIFICA" & Session("vet_XML_To_FormProdotto").split("|")(0)
                            'StringaXmlCreazione(1) = "MODIFICA" & Session("vet_XML_To_FormProdotto")(0)

                            'StringaXmlCreazione(2) = XML_MovimentoDettaglio.OuterXml

                            'Session("vet_XML_To_Documento") = StringaXmlCreazione

                            '-------------------------------------------------------

                            'MATRICE matrix_for_doc DA PASSARE AL DOCUMENTO CHIAMANTE

                            'COLONNE
                            '   BASE                RIF
                            '   0                    1

                            'RIGHE
                            '0  ERRORE              ERRORE
                            '1  MODIFICA+CHIAVE     MODIFICA+CHIAVE
                            '2  DETTAGLIO           PIVA|SACOD|IDDEST|DEST DEL RIF

                            Dim matrix_for_doc(2, 1) As String

                            matrix_for_doc(0, 0) = "XXXXXXXXXXXXXXXXXX"
                            matrix_for_doc(0, 1) = "XXXXXXXXXXXXXXXXXX"

                            matrix_for_doc(1, 0) = "MODIFICA" & Session("vet_XML_To_FormProdotto")(0)
                            matrix_for_doc(1, 1) = "MODIFICA" & Session("vet_XML_To_FormProdotto")(0)

                            matrix_for_doc(2, 0) = XML_MovimentoDettaglio.OuterXml

                            If CStr(Session("vet_XML_To_FormProdotto")(2)) <> "" AndAlso
                                CStr(Session("vet_XML_To_FormProdotto")(2)) <> "&nbsp;" Then

                                If Qs_CodContatto <> "" Then

                                    Dim str_rif As String = ""

                                    Select Case xCaricoScarico

                                        Case CAU_CARICO

                                            str_rif = Qs_CodContatto & "|" &
                                                                    Me.Cmb_Provenienza.SelectedValue.Split("|")(1) & "|" &
                                                                    Me.Cmb_Provenienza.SelectedValue.Split("|")(0) & "|" &
                                                                    Me.Cmb_Provenienza.SelectedItem.Text

                                        Case CAU_SCARICO

                                            str_rif = Qs_CodContatto & "|" &
                                                                    Me.Cmb_Destinazione.SelectedValue.Split("|")(1) & "|" &
                                                                    Me.Cmb_Destinazione.SelectedValue.Split("|")(0) & "|" &
                                                                    Me.Cmb_Destinazione.SelectedItem.Text

                                        Case Else
                                            str_rif = "|||" 'errore

                                    End Select

                                    matrix_for_doc(2, 1) = str_rif 'da valorizzare

                                Else
                                    matrix_for_doc(2, 1) = "|||" 'errore
                                End If

                            Else
                                If Qs_CodContatto <> "" Then
                                    matrix_for_doc(2, 1) = "|||" 'errore
                                Else
                                    matrix_for_doc(2, 1) = "" 'non è impostato il contatto
                                End If
                            End If


                            Session("matrix_XML_To_Documento") = matrix_for_doc


                            '===========================================================


                    End Select

                    ''viewstate("Txt_DaInviare") = "999"


                    Me.Txt_DaInviare.Text = "999"

                    Dim strClose As String = "<SCRIPT language='javascript'> " &
                                            "window.returnValue = document.all('Txt_DaInviare').value; " &
                                            "window.close(); " &
                                            "</SCRIPT>"

                    Me.Controls.Add(New LiteralControl(strClose))


            End Select 'fattura o ddt



            'NON SERVE ---> E' NEL JAVASCRIPT DENTRO ALL'HTML!!!
            'Dim strClose As String = "<script language='javascript'> window.close() </script>"
            'Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))


            '===============================================


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore &
                        Chr(13) &
                        exc.Message.ToString()

            'preparo il messaggio di errore nella sessione
            'per la comunicazione alla bolla

            Dim matrix_errore(0, 1) As String

            matrix_errore(0, 0) = "ERRORE" & Messaggio
            matrix_errore(0, 1) = "ERRORE" & Messaggio

            Session("matrix_XML_To_Documento") = matrix_errore


            ''Faccio abortire la transazione
            'System.EnterpriseServices.ContextUtil.SetAbort()


        End Try




    End Sub

    Private Sub VerificaValorizza_Quantita(ByRef Qta As Decimal)

        If Me.Txt_Quantita.Text <> "" Then
            If Not IsNumeric(Me.Txt_Quantita.Text) Then
                Messaggio &= DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerLaQuantità"), String)
            Else
                If Me.Txt_Quantita.Text <= 0 Then
                    Messaggio &= DirectCast(GetLocalResourceObject("ImpossibileInserireNullaONegativa"), String)
                Else
                    If InStr(Me.Txt_Quantita.Text, ".") <> 0 Then
                        Me.Txt_Quantita.Text = Replace(Me.Txt_Quantita.Text, ".", ",")
                    End If
                    Qta = CDbl(Me.Txt_Quantita.Text)
                End If
            End If
        Else
            Messaggio &= DirectCast(GetLocalResourceObject("InserireUnaQuantità"), String)
        End If
    End Sub

    Private Sub VerificaValorizza_PrezzoUnitario(ByRef Prezzo_Unitario As Decimal)

        If Me.Txt_PrezzoUnitario.Text <> "" Then
            If Not IsNumeric(Me.Txt_PrezzoUnitario.Text) Then
                Messaggio &= DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerIlPrezzo"), String)
            Else
                If Me.Txt_PrezzoUnitario.Text < 0 Then
                    Messaggio &= DirectCast(GetLocalResourceObject("ImpossibileInserirePrezzoNegativo"), String)
                Else
                    If InStr(Me.Txt_PrezzoUnitario.Text, ".") <> 0 Then
                        Txt_PrezzoUnitario.Text = Replace(Txt_PrezzoUnitario.Text, ".", ",")
                    End If
                    Prezzo_Unitario = CDbl(Txt_PrezzoUnitario.Text)
                End If
            End If
        Else
            Prezzo_Unitario = CDbl(0)
        End If
    End Sub


    '###############################################################################
    Protected Sub ImgBtnSalvaNuovo_Click(sender As Object, e As ImageClickEventArgs) Handles ImgBtnSalvaNuovo.Click

        Salva_Tutto(True)

        If Messaggio = "" Then

            'qui non ci arriva neanche perché chiama la AAA_GestioneUscitaPagina
            Response.Redirect("FormProdotto.aspx?" & Request.QueryString.ToString)
        End If

    End Sub

    '###############################################################################
    Private Sub ImgBtnSalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnSalvaTutto.Click

        Salva_Tutto(False)

        If Messaggio = "" Then

            If Not IsNothing(Request.QueryString("exit")) AndAlso Request.QueryString("exit") = True Then
                'chiudo 


                Dim str As String = " $(document).ready(function () { "
                str &= "  window.close();"
                str &= "});"

                'Dim strClose As String = "<script language='javascript'> window.close() </script>"
                ScriptManager.RegisterStartupScript(update_salva, update_salva.GetType(),
                                           String.Format("jQuery_{0}", "chiudimi"), str, True)


            Else

                If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline_2010 Then

                    Dim paginaOnLineRitorno As Integer = CInt(Qs_PagRitorno)

                    If paginaOnLineRitorno = enum_PagineGiasOnline_2010.concimazione_lite OrElse paginaOnLineRitorno = enum_PagineGiasOnline_2010.trattamenti_lite Then
                        Dim str As String = " $(document).ready(function () { "
                        str &= "  window.close();"
                        str &= "});"
                        ScriptManager.RegisterStartupScript(update_salva, update_salva.GetType(),
                                          String.Format("jQuery_{0}", "chiudimi"), str, True)
                        Exit Sub
                    End If

                End If

                If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Menu Then
                    Response.Redirect("../Menu/Menu.aspx")
                End If

                If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Menu_BS Then
                    Response.Redirect("../Menu/MenuBS_Agenda_Nuovo.aspx")
                End If

            End If

        End If

    End Sub



    '###############################################################################
    ''' <summary>
    ''' Usa l'AgronicaCoreModello, tranne che per  il CAU_ANIMALE dove usa l'XML
    ''' </summary>
    ''' <param name="SalvaNuovo"></param>
    Private Sub Salva_Tutto(Optional ByVal SalvaNuovo As Boolean = False)

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim StringaXmlCreazione As String
        Dim Dt As DataTable = Nothing
        Dim i As Integer
        Dim AlmenoUno As Boolean = False

        Dim Qta As Decimal = 0
        Dim Prezzo As Decimal = 0
        Dim PrezzoNetto As Decimal = 0

        '------------------------------------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        Operazione = CInt(Qs_Operazione)

        '---------------------------------
        '-------- CONTROLLI --------------
        '---------------------------------

        Dim risControllo As Boolean = False
        risControllo = Controllo_PreSalvataggio(Dt, AlmenoUno, Qta, Prezzo, PrezzoNetto)

        If Not risControllo AndAlso Messaggio <> "" Then
            Call Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
            Exit Sub
        End If


        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS").ToString)

        '------------------------------------------------
        '------------------------------------------------
        '------------------------------------------------

        'recupero le chiavi dal ViewState
        xPiva = ViewState("Piva")
        xSa_Cod = ViewState("Sa_Cod")
        xAgenda = ViewState("id_agenda")

        '=======================
        '===  Aggiornamento  ===
        '=======================

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Try

            Dim Flag_Insert As Boolean = False
            Dim OUTPUT_ID_Agenda As Integer = 0

            Dim objAgendaScrivi As New Agenda_Operazione_Helper
            Dim Id_Agenda As Integer = 0

            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)
            '-----------------------------------------------------

            Select Case Operazione

                Case enum_TipoOperazioneDB.Scrittura

                    Select Case Qs_Mode.ToLower

                        '######################################################
                        '################## TRASFERIMENTO #####################
                        '######################################################

                        Case "trasferimento"

                            Dim Agenda As Operazione_Agenda = Genera_Agenda(Operazione)
                            Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                            '######################################################
                            '##################### STALLA #########################
                            '######################################################

                        Case Else

                            If Not xFlagDocLight Then

                                If Qs_Tipo = CAU_ANIMALE Then

                                    Dim objAgenda_W As New AgronicaCoreContabBIZ.Agenda_W

                                    For i = 0 To Me.DataGrid_ZooAnimali.Items.Count - 1

                                        'Se la riga è selezionata ...
                                        If CType(Me.DataGrid_ZooAnimali.Items(i).FindControl("ChkSeleziona"), CheckBox).Checked Then

                                            'Creo la stringa di inserimento
                                            StringaXmlCreazione = XML_Genera_StringoneFinale(Operazione, Dt.Rows(i))

                                            Flag_Insert = objAgenda_W.Agenda_Scrivi(CStr(StringaXmlCreazione),
                                                                                    OUTPUT_ID_Agenda,
                                                                                    0,
                                                                                    CInt(Session("ASG_IdServizio")),
                                                                                    0, "",
                                                                                    objParametri_Server)

                                        End If

                                    Next

                                    '######################################################
                                    '############# MAGAZZINO - COMPRAVENDITA ##############
                                    '######################################################

                                Else

                                    Dim Agenda As Operazione_Agenda

                                    For i = 0 To Dt.Rows.Count - 1

                                        'MAGAZZINO DI DESTINAZIONE
                                        If Dt.Rows(i).Item("Elem_Cod") <> ALTRI_BENI And Dt.Rows(i).Item("Elem_Cod") <> SERVIZI And Dt.Rows(i).Item("Elem_Cod") <> 1 Then

                                            'solo se sto facendo un carico deve essere selezionato il magazzino destinazione
                                            If xCaricoScarico = CAU_CARICO Then
                                                '----- Deve essere selezionato un magazzino
                                                If Dt.Rows(i).Item("Destinazione") = "" Then
                                                    Messaggio = DirectCast(GetLocalResourceObject("PerOgniProdottoSelezionareMagazzinoDestinazione"), String)
                                                    Call Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                                    Exit Sub
                                                End If
                                            End If

                                        End If

                                        'Inserisco
                                        Agenda = Genera_Agenda(Operazione, Dt.Rows(i))
                                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                                    Next

                                End If


                            Else
                                '////////////////////////////////////////////////////////////
                                'modalità LIGHT DOCUMENTO
                                Dim Num_Doc As Double
                                Dim Cod_RisUm As Integer

                                If Me.Txt_NumeroDoc.Text = "" Then
                                    Messaggio &= DirectCast(GetLocalResourceObject("ValorizzareIlNumeroDocumento"), String) & vbCrLf
                                Else
                                    If Not IsNumeric(Me.Txt_NumeroDoc.Text) Then
                                        Messaggio &= DirectCast(GetLocalResourceObject("IlNumeroDocumentoDeveContenereSoloNumeri"), String) & vbCrLf
                                    Else
                                        Num_Doc = Me.Txt_NumeroDoc.Text
                                    End If
                                End If

                                If Me.Cmb_Fornitore.SelectedValue = 0 Then
                                    Messaggio &= DirectCast(GetLocalResourceObject("SelezionareIlFornitore"), String) & vbCrLf
                                Else
                                    Cod_RisUm = Me.Cmb_Fornitore.SelectedValue
                                End If


                                'in teoria siamo sempre e solo in scrittura x il doc light
                                If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Scrittura Then

                                    Dim Str_docPresente As String = ""
                                    Dim flag_esiste As Boolean = VerificaNumDocumento2(objParametri_Server, Session, Page,
                                                                                     xPiva,
                                                                                     CStr(CDate(Me.Txt_DataMovimento.Text).Year),
                                                                                     Qs_Lav_Cod,
                                                                                     Cod_RisUm,
                                                                                     Num_Doc,
                                                                                     CAU_REGISTRAZIONI,
                                                                                        Str_docPresente)

                                    If flag_esiste Then
                                        Messaggio &= String.Format(
                                                DirectCast(GetLocalResourceObject("Salva_Tutto_inArchivioEPresenteDocumentoFornitore"), String),
                                                CStr(Str_docPresente)
                                            ) & vbCrLf
                                    End If
                                End If

                                If Messaggio <> "" Then
                                    Call Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                                    Exit Sub
                                End If

                                Dim Agenda As Operazione_Agenda = Genera_Agenda_DocumentoLight(Operazione, Num_Doc, Cod_RisUm, Me.Cmb_Fornitore.SelectedItem.Text, Dt)
                                Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                                '////////////////////////////////////////////////////////////
                            End If

                    End Select


                    '===============================================


                Case enum_TipoOperazioneDB.Modifica

                    Dim Agenda As Operazione_Agenda = Genera_Agenda(Operazione)

                    'Cancello i dati esistenti
                    Dim CancellataOperazione As Boolean = False
                    CancellataOperazione = objAgendaScrivi.Cancella(ViewState("Piva"),
                                                                    Qs_SaCod,
                                                                    Qs_IdAgenda, False,
                                                                    objParametri_Server, logCancellazione:=False)

                    'Inserisco
                    Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                    '===============================================

            End Select

            'chiudi connessione e commit transazione
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            '------------------------------------------------
            '----- Conferma di aggiornamento del database
            '------------------------------------------------

            EseguitaOperazione = True

        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore &
                        Chr(13) &
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        AAA_GestioneUscitaPagina(SalvaNuovo)

        'Select Case Qs_Lav_Cod

        '    Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_TRASFERIMENTO,
        '        LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO,
        '        LAVCOD_VENDITA, LAVCOD_ACQUISTO

        '    Case Else

        '        Dim strClose As String = "<script language='javascript'> window.close() </script>"
        '        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

        'End Select

        ''se provenivo da qualche operazione d'agenda chiudo la finestra..
        'If ((Qs_Lav_Cod < 1000) Or (Qs_Lav_Cod >= 3002) Or (Qs_Lav_Cod = 3000)) And (Qs_Lav_Cod <> -1) Then
        '    Dim strClose As String = "<script language='javascript'> window.close() </script>"
        '    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        'End If

        '------------------------------------------------

    End Sub

    Private Function Controllo_PreSalvataggio(ByRef Dt As DataTable,
                                              ByRef AlmenoUno As Boolean,
                                              ByRef Qta As Decimal,
                                              ByRef Prezzo As Decimal,
                                              ByRef PrezzoNetto As Decimal
                                              ) As Boolean

        Dim i As Integer

        Select Case Qs_Tipo


            '#########################################################
            '########## STALLA e DOCUMENTI CON ANIMALI ###############
            '#########################################################

            Case CAU_ANIMALE

                Dt = ViewState("vs_dtMatricole")

                '----- Deve essere inserita la data del movimento
                If Me.Txt_DataMovimento.Text = "" Then
                    Messaggio &= DirectCast(GetLocalResourceObject("InserireLaDataDelMovimento"), String) & vbCrLf
                End If

                Select Case xCaricoScarico
                    Case CAU_CARICO
                        If Me.Cmb_Destinazione.SelectedItem.Text = "" Then
                            Messaggio &= DirectCast(GetLocalResourceObject("SelezionareLaDestinazione"), String) & vbCrLf
                        End If
                    Case CAU_SCARICO
                        If Me.Cmb_Provenienza.SelectedItem.Text = "" Then
                            Messaggio &= DirectCast(GetLocalResourceObject("SelezionareLaProvenienza"), String) & vbCrLf
                        End If
                End Select

                For i = 0 To Me.DataGrid_ZooAnimali.Items.Count - 1

                    'Se la riga e' selezionata ...
                    If CType(Me.DataGrid_ZooAnimali.Items(i).FindControl("ChkSeleziona"), CheckBox).Checked Then

                        AlmenoUno = True

                        Qta = 0
                        Prezzo = 0

                        '-----
                        'QUANTITA'
                        If CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text = "" Then
                            Messaggio &= DirectCast(GetLocalResourceObject("InserireLaQuantità"), String) & vbCrLf
                        Else
                            If Not IsNumeric(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text) Then
                                Messaggio &= DirectCast(GetLocalResourceObject("LaQuantitàDeveEssereUnNumero"), String) & vbCrLf
                                'ripulisco la textbox
                                CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text = ""
                                Return False
                            Else
                                If InStr(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text, ".") <> 0 Then
                                    CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text = Replace(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text, ".", ",")
                                End If

                                Qta = CDbl(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtqta"), TextBox).Text)

                                Dt.Rows(i).Item("Qta_Reale") = Qta

                            End If


                        End If

                        '-----
                        'PREZZO
                        If CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text <> "" Then
                            If Not IsNumeric(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text) Then
                                Messaggio &= DirectCast(GetLocalResourceObject("IlPrezzoDeveEssereUnNumero"), String) & vbCrLf
                                'ripulisco la textbox
                                CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text = ""
                                Return False
                            Else
                                If InStr(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text, ".") <> 0 Then
                                    CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text = Replace(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text, ".", ",")
                                End If
                                Prezzo = CDbl(CType(DataGrid_ZooAnimali.Items(i).FindControl("txtprezzo"), TextBox).Text)

                                Dt.Rows(i).Item("Prezzo_Reale") = Prezzo

                            End If
                        End If

                    End If

                Next

                '----- Deve essere riempito il datagrid
                If Not AlmenoUno Then
                    Messaggio &= DirectCast(GetLocalResourceObject("ScegliereAlmenoUnAnimale"), String) & vbCrLf
                End If



                '######################################################
                '############# MAGAZZINO - TRASFERIMENTO ##############
                '######################################################

            Case Else


                '######################################################
                '############# SCRITTURA - TRASFERIMENTO ##############
                '######################################################
                If (Operazione = enum_TipoOperazioneDB.Scrittura) AndAlso
                   (Qs_Mode <> "trasferimento") Then

                    Dt = ViewState("DT_Prodotti_NO_Contabili")

                    '----- Deve essere inserita la data del movimento
                    If Me.Txt_DataMovimento.Text = "" Then
                        Messaggio &= DirectCast(GetLocalResourceObject("InserireLaDataDelMovimento"), String) & vbCrLf
                    End If

                    '----- Deve essere riempito il datagrid
                    If Dt.Rows.Count = 0 Then
                        Messaggio &= DirectCast(GetLocalResourceObject("InserireAlmenoUnProdottoNelRiepilogo"), String) & vbCrLf
                    End If


                    '######################################################
                    '################### MODIFICA #########################
                    '######################################################
                ElseIf (Operazione = enum_TipoOperazioneDB.Modifica) OrElse
                       (Qs_Mode = "trasferimento") Then


                    '----- Deve essere inserita la data del movimento
                    If Me.Txt_DataMovimento.Text = "" Then
                        Messaggio &= DirectCast(GetLocalResourceObject("InserireLaDataDelMovimento"), String) & vbCrLf
                    End If

                    '----- Deve essere selezionato un prodotto
                    If IsNothing(cmb_Prodotti.SelectedItem) OrElse cmb_Prodotti.SelectedItem.Value = "0" Then
                        Messaggio &= AgronicaAgenda_2010.SelezionareUnProdotto & vbCrLf
                    End If

                    If xCaricoScarico = CAU_TRASFERIMENTO Then

                        If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                            Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiProvenienzaNonSelezionato"), String) & vbCrLf
                        End If
                        If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                            Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiDestinazioneNonSelezionato"), String) & vbCrLf
                        End If
                        If Me.Cmb_Provenienza.SelectedIndex <> 0 AndAlso Me.Cmb_Destinazione.SelectedIndex <> 0 Then
                            If Me.Cmb_Provenienza.SelectedValue = Me.Cmb_Destinazione.SelectedValue Then
                                Messaggio &= AgronicaAgenda_2010.MagazziniDiProvenienzaEDestinazioneCoincidono & vbCrLf
                            End If
                        End If

                    End If

                    'MAGAZZINO DI DESTINAZIONE
                    If Me.cmb_Categoria.SelectedValue <> ALTRI_BENI AndAlso Me.cmb_Categoria.SelectedValue <> SERVIZI AndAlso Me.cmb_Categoria.SelectedValue <> 1 Then
                        'solo se sto facendo un carico deve essere selezionato il magazzino destinazione
                        If xCaricoScarico = CAU_CARICO Then
                            '----- Deve essere selezionato un magazzino
                            If Me.Cmb_Destinazione.SelectedIndex < 1 Then
                                Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiDestinazioneNonSelezionato"), String) & vbCrLf
                            End If
                        End If
                    End If


                    '----- Deve essere selezionata un'unità di misura
                    If Me.cmb_Udm.SelectedIndex < 1 Then
                        Messaggio &= DirectCast(GetLocalResourceObject("SelezionareUnaUnitàDiMisura"), String)
                    End If


                    '-----
                    'QUANTITA'
                    If Me.Txt_Quantita.Text <> "" Then
                        If Not IsNumeric(Me.Txt_Quantita.Text) Then
                            Messaggio &= DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerLaQuantità"), String)
                        Else
                            If Me.Txt_Quantita.Text <= 0 Then
                                Messaggio &= DirectCast(GetLocalResourceObject("ImpossibileInserireNullaONegativa"), String)
                            Else
                                If InStr(Me.Txt_Quantita.Text, ".") <> 0 Then
                                    Me.Txt_Quantita.Text = Replace(Me.Txt_Quantita.Text, ".", ",")
                                End If

                                If Qs_Mode = "trasferimento" Then
                                    'controllo di non aver trasferito più di quello che avevo
                                    If ViewState("Giacenza_Magazzino") < CDbl(Txt_Quantita.Text) Then
                                        Messaggio &= DirectCast(GetLocalResourceObject("ImpossibileTrasferireQuantitàMaggioreDellaDisponibile"), String)
                                    End If
                                End If

                                Qta = CDbl(Txt_Quantita.Text)
                            End If
                        End If
                    Else
                        Messaggio &= DirectCast(GetLocalResourceObject("InserireUnaQuantità"), String)
                    End If

                    '-----
                    'PREZZO
                    VerificaValorizza_PrezzoUnitario(Prezzo)


                    If Txt_PrezzoUnitario_Netto.Text = "" Then
                        PrezzoNetto = Prezzo
                    Else
                        Select Case CInt(Qs_Lav_Cod)
                            Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_RICEVUTA
                                'in questi casi non sono gestiti i dettagli economici
                                'perciò prezzo netto è sempre uguale a prezzo
                                PrezzoNetto = Prezzo
                            Case Else
                                PrezzoNetto = CDbl(Txt_PrezzoUnitario_Netto.Text)
                        End Select
                    End If

                End If

        End Select

        If Messaggio <> "" Then
            Return False
        Else
            Return True
        End If

    End Function

    '########################################################################################
    ''' <summary>
    ''' Usa l'XML, usata per il salvataggio con documento non light, per cui chi scrive effettivamente è la Form <see cref="DocumentoContabileGenerico"/>
    ''' </summary>
    ''' <param name="Operazione"></param>
    ''' <param name="Dr"></param>
    ''' <returns></returns>
    Private Function XML_Genera_StringoneFinale(ByVal Operazione As enum_TipoOperazioneDB,
                                                Optional ByVal Dr As DataRow = Nothing
                                                ) As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New XmlDocument

        Dim XML_DatiAgenda As XmlElement
        Dim XML_Agenda As XmlElement
        Dim XML_DatiMovimenti As XmlElement
        Dim XML_MovimentoMagazzino As XmlElement
        Dim XML_MovimentoContabile As XmlElement
        Dim XML_Movimento As XmlElement
        Dim XML_DatiMovimentiDettagli As XmlElement
        Dim XML_MovimentoDettaglio As XmlElement

        Dim str_MovimentoDettaglio As String
        Dim str_Movimenti As String
        Dim str_XML_ZooAnimale As String

        Dim XML_DatiZoo_Animale_Anagrafe As XmlElement

        Dim BaseCode As Integer
        Dim TopCode As Integer
        'Dim Descrizione As String
        Dim Des_Lib As String = ""
        Dim Mov_Desc As String
        Dim Mov_Desc_Contabile As String = ""
        Dim Mov_Det_Des As String = ""
        Dim Lav_Cod As Integer

        Dim array_temp As String()
        Dim Sa_Cod As Integer
        Dim Magazzino_Cod As Integer

        Dim Cau_Mov As String = ""
        Dim Elem_Cod As Integer
        Dim Pro_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Cod_Progetto As Integer
        Dim Fase_Cod As Integer
        Dim Lotto As String = ""
        Dim Cal_Cod As Integer
        Dim Udm_Cod As Integer
        Dim Extra_int As Integer

        Dim Gen_Cod As Integer
        Dim Spe_Cod As Integer
        Dim Ipro_Cod As Integer
        Dim Raz_Cod As Integer
        Dim Cat_Cod As Integer
        Dim Spe_Des As String = ""
        Dim Ipro_Des As String = ""
        Dim Raz_Des As String = ""

        Dim Qta As Decimal
        Dim PrezzoUnitario, PrezzoUnitario_Netto As Decimal

        Dim Udm_Cod_Extra As Decimal = 0
        Dim Qta_Extra As Decimal = 0
        Dim Prezzo_Effettivo As Decimal = 0
        Dim Sconto_Magg As Decimal = 0
        Dim Variazione_Perc As String = ""
        Dim Variazione As Decimal = 0
        Dim Imponibile As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Cod_Iva As Integer = 0
        Dim Iva As Decimal = 0
        Dim Anno As Integer = 0
        Dim Ric_Cod As Integer = 0
        Dim Cod_Conto As Integer = 0
        Dim Conto As String = ""

        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Cu As Decimal = 0
        Dim Regolamento_Cod_Ferti As Integer = 0

        Dim Matricola As String = ""
        Dim Nome As String = ""
        Dim Collare As String = ""
        Dim Nome_Aia As String = ""
        Dim Matricola_Aia As String = ""
        Dim Dat_Nascita As Date = AGRODATAINIZIO
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE
        Dim Stato_Nascita As String = "000"
        Dim Provincia_Nascita As String = "000"
        Dim AUA_Azi_Nascita As String = ""
        Dim AUSL_Azi_Nascita As String = ""
        Dim Sesso As String = "M"
        Dim Mat_Padre As String = ""
        Dim Mat_Madre As String = ""
        Dim CF_Proprietario As String = ""
        Dim CF_Detentore As String = ""
        Dim Presente As String = ""
        Dim Peso As Decimal = 0
        Dim Data_Pesa As Date = AGRODATAINIZIO
        Dim Metodo_Produzione As Integer
        Dim Conversione_Inizio As Date = AGRODATAINIZIO
        Dim Conversione_Fine As Date = AGRODATAINIZIO


        'modifica 12 06 2014, mostro le note della form prodotto
        Mov_Desc = TextBoxNote.Text

        Dim Rif_Esterno As String = ""
        Dim Rif_Esterno_2 As String = ""
        Recupera_DettagliNascosti(Rif_Esterno, Rif_Esterno_2)


        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS").ToString)



        Select Case Qs_Tipo


            '####################################
            '############# STALLA ###############
            '####################################

            Case CAU_ANIMALE

                Elem_Cod = ZOO_CONSISTENZA
                Pro_Cod = 0
                Mat_Cod = 0
                Lotto = ""
                Cal_Cod = 0
                Udm_Cod = 38

                'setto le variabili per differenziare il caso del carico e dello scarico.
                Select Case xCaricoScarico


                    Case enum_Agenda_Causali.CARICO

                        '-----------------------------------------------------------------
                        '------------------------- CARICO --------------------------------
                        '-----------------------------------------------------------------

                        array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                        Cau_Mov = CAU_CARICO
                        Lav_Cod = 3001

                        Des_Lib = DirectCast(GetLocalResourceObject("AumentoConsistenzeZootecniche"), String)
                        'Mov_Desc = "Aumento Consistenze Zootecniche"


                    Case enum_Agenda_Causali.SCARICO

                        '-----------------------------------------------------------------
                        '------------------------- SCARICO -------------------------------
                        '-----------------------------------------------------------------


                        array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                        Cau_Mov = CAU_SCARICO
                        Lav_Cod = 3002

                        Des_Lib = DirectCast(GetLocalResourceObject("DecrementoConsistenzeZootecniche"), String)
                        'Mov_Desc = "Decremento Consistenze Zootecniche"

                End Select


                If Dr IsNot Nothing Then

                    '-----------------------------------------------------------------
                    '------------------------- SCRITTURA -----------------------------
                    '-----------------------------------------------------------------

                    Cod_Progetto = Dr.Item("Cod_Progetto")
                    'Fase_Cod = Dr.Item("Fase_Cod")

                    Gen_Cod = Dr.Item("Gen_Cod")
                    Spe_Cod = Dr.Item("Spe_Cod")
                    Ipro_Cod = Dr.Item("Ipro_Cod")
                    Raz_Cod = Dr.Item("Raz_Cod")
                    Cat_Cod = Dr.Item("Cat_Cod")
                    Spe_Des = Dr.Item("Spe_Des")
                    Ipro_Des = Dr.Item("Ipro_Des")
                    Raz_Des = Dr.Item("Raz_Des")
                    Matricola = Dr.Item("Matricola")
                    Nome = Dr.Item("Nome")
                    Collare = Dr.Item("Collare")
                    Nome_Aia = Dr.Item("Nome_Aia")
                    Matricola_Aia = Dr.Item("Matricola_Aia")
                    Dat_Nascita = Dr.Item("Dat_Nascita")
                    Stato_Nascita = Dr.Item("Stato_Nascita")
                    Provincia_Nascita = Dr.Item("Prov_Nascita")
                    AUA_Azi_Nascita = Dr.Item("AUA_Azi_Nascita")
                    AUSL_Azi_Nascita = Dr.Item("AUSL_Azi_Nascita")

                    Mov_Det_Des = Ipro_Des & " - " & Raz_Des & " - " & Matricola & ": " & Nome

                    Sesso = Dr.Item("Sesso")
                    Mat_Padre = Dr.Item("Mat_Padre")
                    Mat_Madre = Dr.Item("Mat_Madre")
                    Peso = Dr.Item("Peso")
                    Data_Pesa = Dr.Item("Data_Pesa")

                    Metodo_Produzione = Dr.Item("Metodo_Produzione")
                    Conversione_Inizio = Dr.Item("Conversione_Inizio")
                    Conversione_Fine = Dr.Item("Conversione_Fine")

                    Qta = Dr.Item("Qta_Reale")
                    PrezzoUnitario = Dr.Item("Prezzo_Reale")
                    PrezzoUnitario_Netto = PrezzoUnitario


                Else

                    '-----------------------------------------------------------------
                    '------------------------- MODIFICA -----------------------------
                    '-----------------------------------------------------------------



                End If


                '######################################################
                '#################### MAGAZZINO #######################
                '######################################################

            Case Else


                If Dr IsNot Nothing Then

                    '-----------------------------------------------------------------
                    '------------------------- SCRITTURA -----------------------------
                    '-----------------------------------------------------------------

                    Elem_Cod = Dr.Item("Elem_Cod")
                    Pro_Cod = Dr.Item("Pro_Cod")
                    Mat_Cod = Dr.Item("Mat_Cod")
                    Cod_Progetto = Dr.Item("Cod_Progetto")
                    Fase_Cod = Dr.Item("Fase_Cod")
                    Lotto = Dr.Item("Lotto")
                    Cal_Cod = Dr.Item("Cal_Cod")

                    Udm_Cod = Dr.Item("Udm_Cod")
                    Qta = Dr.Item("Qta")
                    Extra_int = 0
                    ' nicoletta
                    If Dr.Item("Extra_Int") <> "0" Then

                        Extra_int = Udm_Cod

                        Select Case Udm_Cod
                            Case enum_UnitaMisura.Quintali
                                Qta = Qta * 100
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Tonnellate
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Metri_Cubi
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.Litri
                        End Select

                    End If


                    Mov_Det_Des = Dr.Item("Prodotto")
                    PrezzoUnitario = Dr.Item("Prezzo_Unitario")
                    PrezzoUnitario_Netto = Dr.Item("Prezzo_Unitario_Netto")
                    Udm_Cod_Extra = Dr.Item("Udm_Cod_Extra")
                    Qta_Extra = Dr.Item("Qta_Extra")

                    Sconto_Magg = Dr.Item("Cod_Variazione")
                    Cod_Iva = Dr.Item("Cod_Iva")
                    Iva = Dr.Item("Iva")
                    Imponibile = Dr.Item("Imponibile")
                    Imponibile_Netto = Dr.Item("Imponibile_Netto")
                    Prezzo_Effettivo = Dr.Item("Prezzo_Effettivo")
                    Anno = Dr.Item("Anno")
                    Ric_Cod = Dr.Item("Ric_Cod")
                    Cod_Conto = Dr.Item("Cod_Conto")
                    Conto = Dr.Item("Conto")

                    N = Dr.Item("N")
                    P2O5 = Dr.Item("P2O5")
                    K2O = Dr.Item("K2O")
                    Cu = Dr.Item("Cu")
                    Regolamento_Cod_Ferti = Dr.Item("Regolamento_Cod_Ferti")

                    Rif_Esterno = Dr.Item("Rif_Esterno")
                    Rif_Esterno_2 = Dr.Item("Rif_Esterno_2")


                    'setto le variabili per differenziare il caso del carico e dello scarico.
                    Select Case xCaricoScarico

                        Case enum_Agenda_Causali.CARICO

                            '-----------------------------------------------------------------
                            '------------------------- CARICO --------------------------------
                            '-----------------------------------------------------------------

                            Magazzino_Cod = Dr.Item("Id_Destinazione")
                            Sa_Cod = Dr.Item("SaCod_Destinazione")
                            Cau_Mov = CAU_CARICO
                            'modifica 12 06 2014, mostro ne note della form prodotto
                            'Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"

                            Select Case Qs_Lav_Cod

                                Case LAVCOD_ACQUISTO
                                    Lav_Cod = LAVCOD_ACQUISTO
                                    Des_Lib = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_FATTURA_RICEVUTA
                                    Lav_Cod = LAVCOD_FATTURA_RICEVUTA
                                    Des_Lib = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                                    Lav_Cod = LAVCOD_FATTURA_RICEVUTA
                                    Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case Else
                                    Lav_Cod = LAVCOD_CARICO
                                    Des_Lib = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"
                                    Mov_Desc_Contabile = ""

                            End Select


                        Case enum_Agenda_Causali.SCARICO

                            '-----------------------------------------------------------------
                            '------------------------- SCARICO -------------------------------
                            '-----------------------------------------------------------------


                            Magazzino_Cod = Dr.Item("Id_Provenienza")
                            Sa_Cod = Dr.Item("SaCod_Provenienza")
                            Cau_Mov = CAU_SCARICO
                            'modifica 12 06 2014, mostro ne note della form prodotto
                            'Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"

                            Select Case Qs_Lav_Cod

                                Case LAVCOD_VENDITA
                                    Lav_Cod = LAVCOD_VENDITA
                                    Des_Lib = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_FATTURA_EMESSA
                                    Lav_Cod = LAVCOD_FATTURA_EMESSA
                                    Des_Lib = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                    Lav_Cod = LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                    Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case Else
                                    Lav_Cod = LAVCOD_SCARICO
                                    Des_Lib = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"
                                    Mov_Desc_Contabile = ""
                            End Select


                    End Select


                Else

                    '-----------------------------------------------------------------
                    '------------------------- MODIFICA -----------------------------
                    '-----------------------------------------------------------------

                    Dim Progetto_Des As String = ""
                    Dim Cal_Des As String = ""
                    Dim Udm_des As String = ""
                    Dim Str_Errore As String = ""

                    Elem_Cod = Me.cmb_Categoria.SelectedItem.Value

                    Select Case Elem_Cod

                        Case ALTRI_BENI, SERVIZI

                            If Me.Txt_BeniStrumentali.Text = "" Then
                                Str_Errore &= DirectCast(GetLocalResourceObject("InserisciIlNomeDelProdotto"), String) & vbCrLf
                            Else
                                Mov_Det_Des = Me.Txt_BeniStrumentali.Text
                            End If

                            '-------------------------------------------------------------

                        Case Else

                            If IsNothing(cmb_Prodotti.SelectedItem) OrElse cmb_Prodotti.SelectedItem.Value = "0" Then
                                Str_Errore &= AgronicaAgenda_2010.SelezionareUnProdotto & vbCrLf
                            Else
                                If cmb_Prodotti.SelectedItem.Value.Split("|")(0) > 0 Then
                                    Pro_Cod = Me.cmb_Prodotti.SelectedItem.Value.Split("|")(0)
                                    Mov_Det_Des = Me.cmb_Prodotti.SelectedItem.Text
                                Else
                                    Mat_Cod = -Me.cmb_Prodotti.SelectedItem.Value
                                    Mov_Det_Des = Me.cmb_Prodotti.SelectedItem.Text
                                End If
                            End If

                    End Select

                    Recupera_DettagliNascosti(Rif_Esterno, Rif_Esterno_2)

                    Recupera_DettagliProdotto(Str_Errore,
                                             Cod_Progetto,
                                             Progetto_Des,
                                             Fase_Cod,
                                             Lotto,
                                             Cal_Cod,
                                             Cal_Des,
                                             Udm_Cod,
                                             Udm_des,
                                             Qta,
                                             PrezzoUnitario,
                                             PrezzoUnitario_Netto)

                    Extra_int = 0
                    If Elem_Cod = FERTILIZZANTI Then

                        Select Case Udm_Cod

                            Case enum_UnitaMisura.Quintali
                                Extra_int = Udm_Cod
                                Qta = Qta * 100
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Tonnellate
                                Extra_int = Udm_Cod
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Metri_Cubi
                                Extra_int = Udm_Cod
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.Litri
                        End Select

                        Recupera_DettagliFertilizzante(Str_Errore, N, P2O5, K2O, Cu, Regolamento_Cod_Ferti)

                    End If

                    '===================================================
                    'DETTAGLI ECONOMICI
                    If Me.Pannello_Economico.Visible Then

                        Recupera_DettagliEconomici(Udm_Cod_Extra,
                                                    Qta_Extra,
                                                    Prezzo_Effettivo,
                                                    Sconto_Magg,
                                                    Variazione_Perc,
                                                    Variazione,
                                                    Imponibile,
                                                    Imponibile_Netto,
                                                    Cod_Iva,
                                                    Iva,
                                                    Anno,
                                                    Ric_Cod,
                                                    Cod_Conto,
                                                    Conto)

                    Else
                        Imponibile = Qta * PrezzoUnitario
                        Imponibile_Netto = Qta * PrezzoUnitario_Netto
                    End If
                    '============================================================


                    Select Case Qs_Mode.ToLower


                        '######################################################
                        '#################### TRASFERIMENTO ###################
                        '######################################################

                        Case "trasferimento"

                            Des_Lib = DirectCast(GetLocalResourceObject("TrasferimentoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                            Lav_Cod = LAVCOD_TRASFERIMENTO
                            Sa_Cod = Me.Cmb_Provenienza.SelectedValue.Split("|")(1)

                            'modifica 12 06 2014, mostro ne note della form prodotto
                            Select Case xCaricoScarico
                                Case enum_Agenda_Causali.CARICO
                                    ' Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                Case enum_Agenda_Causali.SCARICO
                                    'Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                            End Select


                            '######################################################
                            '################# MAGAZZINO  #########################
                            '######################################################

                        Case Else

                            'setto le variabili per differenziare il caso del carico e dello scarico.
                            Select Case xCaricoScarico

                                Case enum_Agenda_Causali.CARICO

                                    '-----------------------------------------------------------------
                                    '------------------------- CARICO --------------------------------
                                    '-----------------------------------------------------------------

                                    Cau_Mov = CAU_CARICO
                                    'If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                    'Else
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                    'End If

                                    Select Case Qs_Lav_Cod

                                        Case LAVCOD_ACQUISTO
                                            Lav_Cod = LAVCOD_ACQUISTO
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case LAVCOD_FATTURA_RICEVUTA
                                            Lav_Cod = LAVCOD_FATTURA_RICEVUTA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case LAVCOD_NOTA_ACCREDITO_EMESSA
                                            Lav_Cod = LAVCOD_NOTA_ACCREDITO_EMESSA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case Else
                                            Lav_Cod = LAVCOD_CARICO
                                            Mov_Desc_Contabile = ""
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                            End If

                                    End Select


                                    Select Case Elem_Cod

                                        Case MACCHINE, ALTRI_BENI, SERVIZI
                                            Magazzino_Cod = 0
                                            Sa_Cod = 0

                                        Case Else
                                            array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                                            Magazzino_Cod = array_temp(0)
                                            Sa_Cod = array_temp(1)

                                    End Select


                                Case enum_Agenda_Causali.SCARICO

                                    '-----------------------------------------------------------------
                                    '------------------------- SCARICO -------------------------------
                                    '-----------------------------------------------------------------

                                    Cau_Mov = CAU_SCARICO
                                    'If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                    'Else
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                    'End If

                                    Select Case Qs_Lav_Cod

                                        Case LAVCOD_VENDITA
                                            Lav_Cod = LAVCOD_VENDITA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case LAVCOD_FATTURA_EMESSA
                                            Lav_Cod = LAVCOD_FATTURA_EMESSA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ")"
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ")"
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                            End If

                                        Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                            Lav_Cod = LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiRicevutaEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case Else
                                            Lav_Cod = LAVCOD_SCARICO
                                            Mov_Desc_Contabile = ""
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                            End If

                                    End Select

                                    Select Case Elem_Cod

                                        Case 1, ALTRI_BENI, SERVIZI
                                            Magazzino_Cod = 0
                                            Sa_Cod = 0

                                        Case Else
                                            array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                                            Magazzino_Cod = array_temp(0)
                                            Sa_Cod = array_temp(1)

                                    End Select


                            End Select


                    End Select


                End If

        End Select

        '-----------------------------------------------------------------------

        'Giulia 02/11/2020: tolta tutta la parte dell'AgronicaCoreModello in questa funzione, perché tanto non è usata,
        ' Qui fà solo l'XML, l'obj del modello è creato in Genera_Agenda

        'Dim Agenda As New Operazione_Agenda
        'Dim Movimento As Movimento
        'Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        'Dim Movimento_Dettaglio As Movimento_Dettaglio
        'Dim Movimento_Destinazione As Movimento_Destinazione


        '#######################################################
        '############## Genero la struttura XML ################
        '#######################################################




        '#######################################################
        '################## DATI  AGENDA   #####################
        '#######################################################

        '----- DatiAgenda

        XML_DatiAgenda = XmlDoc.CreateElement("DatiAgenda")

        XmlDoc.AppendChild(XML_DatiAgenda)



        '#######################################################
        '##################   AGENDA   #########################
        '#######################################################

        'Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        'Agenda.Id_Agenda = Qs_IdAgenda
        'Agenda.Data = Me.Txt_DataMovimento.Text
        'Agenda.Piva = ViewState("Piva")
        'Agenda.Sa_Cod = Sa_Cod
        'Agenda.Lav_Cod = Lav_Cod
        'Agenda.Des_Lib = Des_Lib

        'Agenda.BaseCode = BaseCode
        'Agenda.TopCode = TopCode

        '----- Agenda

        XML_Agenda = XML_2_Agenda_Agenda(enum_TipoOperazioneDB.Scrittura,
                                        ViewState("Piva"),
                                        Sa_Cod,
                                        0,
                                        Lav_Cod,
                                        Des_Lib,
                                        ,
                                        ,
                                        ,
                                        ,
                                        Me.Txt_DataMovimento.Text,
                                        AGRODATAFINE,
                                        BaseCode,
                                        TopCode,
                                        XmlDoc)

        XML_DatiAgenda.AppendChild(XML_Agenda)

        'Agenda.Movimenti = New List(Of Movimento)


        Select Case Qs_Mode.ToLower


            Case "trasferimento"


                '#######################################################
                '################  DATI  MOVIMENTI    ##################
                '#######################################################

                '----- DatiMovimenti

                XML_DatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

                XML_Agenda.AppendChild(XML_DatiMovimenti)



                '#######################################################
                '##################   MOVIMENTI    #####################
                '#######################################################

                str_Movimenti = ""

                For i As Integer = 0 To 1

                    'Movimento = New Movimento

                    If i = 0 Then

                        'PROVENIENZA
                        Cau_Mov = CAU_SCARICO
                        'Mov_Desc = "Scarico da Trasferimento di magazzino (" & Me.cmb_Prodotti.SelectedItem.Text & ")"

                        array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                    Else
                        'DESTINAZIONE
                        Cau_Mov = CAU_CARICO
                        'Mov_Desc = "Carico da Trasferimento di magazzino (" & Me.cmb_Prodotti.SelectedItem.Text & ")"

                        array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                    End If

                    'Movimento.Id_Agenda = Qs_IdAgenda
                    'Movimento.Piva = ViewState("Piva")
                    'Movimento.Sa_Cod = Sa_Cod
                    'Movimento.Data = Me.Txt_DataMovimento.Text
                    'Movimento.Lav_Cod = Lav_Cod
                    'Movimento.Cau_Mov = Cau_Mov
                    'Movimento.Mov_Desc = Mov_Desc
                    'Movimento.Ora = Me.Txt_Ora.Text
                    'Movimento.Data_Registrazione = Me.Txt_DataMovimento.Text
                    'Movimento.Scadenza = AGRODATAFINE

                    'Movimento.BaseCode = BaseCode
                    'Movimento.TopCode = TopCode

                    'Agenda.Movimenti.Add(Movimento)


                    '#######################################################
                    '###########   MOVIMENTO DI CARICO / SCARICO    ########
                    '#######################################################

                    XML_MovimentoMagazzino = XML_2_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura,
                                                                    ViewState("Piva"),
                                                                    Sa_Cod,
                                                                    0,
                                                                    0,
                                                                    Cau_Mov,
                                                                    Mov_Desc,
                                                                    Me.Txt_DataMovimento.Text,
                                                                    Me.Txt_Ora.Text,
                                                                    Me.Txt_DataMovimento.Text,
                                                                    AGRODATAFINE,
                                                                    ,
                                                                    "",
                                                                    0,
                                                                    "",
                                                                    ,
                                                                    ,
                                                                    ,
                                                                    ,
                                                                    ,
                                                                    0,
                                                                    0,
                                                                    ,
                                                                    ,
                                                                    ,
                                                                    ,
                                                                    ,
                                                                    ,
                                                                    "",
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    "",
                                                                    "",
                                                                    0,
                                                                    ,
                                                                    Me.Txt_DataMovimento.Text,
                                                                    AGRODATAFINE,
                                                                    BaseCode,
                                                                    TopCode,
                                                                    XmlDoc)

                    '#######################################################
                    '##############  DATI MOVIMENTI DETTAGLI    ############
                    '#######################################################

                    '----- DatiMovimenti_Dettagli

                    XML_DatiMovimentiDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

                    XML_MovimentoMagazzino.AppendChild(XML_DatiMovimentiDettagli)



                    '#######################################################
                    '################   MOVIMENTI DETTAGLIO    #############
                    '#######################################################


                    str_MovimentoDettaglio = XML_Genera_MovimentoDettaglio(enum_TipoOperazioneDB.Scrittura,
                                                                        ViewState("Piva"),
                                                                        Sa_Cod,
                                                                        , , ,
                                                                        Lav_Cod,
                                                                        Elem_Cod,
                                                                        Pro_Cod,
                                                                        Mat_Cod,
                                                                        Cod_Progetto,
                                                                        Cal_Cod,
                                                                        Fase_Cod,
                                                                        Lotto,
                                                                        Udm_Cod,
                                                                        Qta,
                                                                        PrezzoUnitario,
                                                                        PrezzoUnitario_Netto,
                                                                        , ,
                                                                        Extra_int,
                                                                        CAU_TRASFERIMENTO,
                                                                        Mov_Det_Des,
                                                                        Imponibile,
                                                                        Imponibile_Netto,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        , ,
                                                                        BaseCode,
                                                                        TopCode,
                                                                        Magazzino_Cod,
                                                                        ,
                                                                         i,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                           N, P2O5, K2O, Cu,
                                                                           Regolamento_Cod_Ferti,
                                                                           Rif_Esterno, Rif_Esterno_2)


                    XML_DatiMovimentiDettagli.InnerXml = str_MovimentoDettaglio


                    str_Movimenti &= XML_MovimentoMagazzino.OuterXml


                    XML_MovimentoMagazzino = Nothing
                    'XML_Temp.InnerXml = ""


                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    '#######################################################
                    '################   MOVIMENTI DETTAGLIO    #############
                    '#######################################################

                    'Movimento_Dettaglio = New Movimento_Dettaglio

                    'Movimento_Dettaglio.Id_Agenda = Qs_IdAgenda
                    'Movimento_Dettaglio.Piva = ViewState("Piva")
                    'Movimento_Dettaglio.Sa_Cod = Sa_Cod
                    'Movimento_Dettaglio.Data = Me.Txt_DataMovimento.Text
                    'Movimento_Dettaglio.Lav_Cod = Lav_Cod
                    'Movimento_Dettaglio.Cau_Mov = Cau_Mov
                    'Movimento_Dettaglio.Mov_Det_Des = Mov_Det_Des
                    'Movimento_Dettaglio.Elem_Cod = Elem_Cod
                    'Movimento_Dettaglio.Pro_Cod = Pro_Cod
                    'Movimento_Dettaglio.Mat_Cod = Mat_Cod
                    'Movimento_Dettaglio.Cod_Progetto = Cod_Progetto
                    'Movimento_Dettaglio.Fase_Cod = Fase_Cod
                    'Movimento_Dettaglio.Lotto = Lotto
                    'Movimento_Dettaglio.Cal_Cod = Cal_Cod
                    'Movimento_Dettaglio.Udm_Cod = Udm_Cod
                    'Movimento_Dettaglio.Udm_Cod_Extra = Udm_Cod_Extra
                    'Movimento_Dettaglio.Qta = Qta
                    'Movimento_Dettaglio.Qta_Extra = Qta_Extra

                    'Movimento_Dettaglio.Prezzo_Effettivo = Prezzo_Effettivo
                    'Movimento_Dettaglio.Prezzo_Unitario = PrezzoUnitario
                    'Movimento_Dettaglio.Prezzo_Unitario_Netto = PrezzoUnitario_Netto

                    'Movimento_Dettaglio.Sconto = Sconto_Magg
                    'Movimento_Dettaglio.Imponibile = Imponibile
                    'Movimento_Dettaglio.Imponibile_Netto = Imponibile_Netto

                    'Movimento_Dettaglio.Cod_Iva = Cod_Iva
                    'Movimento_Dettaglio.Iva = Iva

                    'Movimento_Dettaglio.Anno = Anno
                    'Movimento_Dettaglio.Ric_Cod = Ric_Cod
                    'Movimento_Dettaglio.Cod_Conto = Cod_Conto
                    'Movimento_Dettaglio.Extra_Int = Extra_int

                    'Movimento_Dettaglio.BaseCode = BaseCode
                    'Movimento_Dettaglio.TopCode = TopCode

                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)


                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                    'Movimento_Destinazione = New Movimento_Destinazione

                    'Movimento_Destinazione.Id_Agenda = Qs_IdAgenda
                    'Movimento_Destinazione.Piva = ViewState("Piva")
                    'Movimento_Destinazione.Sa_Cod = Sa_Cod
                    'Movimento_Destinazione.Appezza = 0
                    'Movimento_Destinazione.Id_Destinazione = Magazzino_Cod
                    'Movimento_Destinazione.Tipo = MAGAZZINO
                    'Movimento_Destinazione.Qta = Qta
                    'Movimento_Destinazione.Data = Me.Txt_DataMovimento.Text
                    'Movimento_Destinazione.BaseCode = BaseCode
                    'Movimento_Destinazione.TopCode = TopCode

                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni.Add(Movimento_Destinazione)


                Next


                XML_DatiMovimenti.InnerXml = str_Movimenti


            Case Else



                If Qs_Tipo = CAU_ANIMALE Then

                    '######################################################################################################
                    '----- DatiMovimenti

                    'Creo un tag volante di nome dati movimenti
                    XML_DatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

                    'Rendo il tag appena creato figlio del nodo agenda
                    XML_Agenda.AppendChild(XML_DatiMovimenti)

                    '######################################################################################################
                    '----- Movimento

                    'Imposto il tag Movimento creato nella routine XML_Agenda_Movimento come figlio del tag XML_DatiMovimenti
                    XML_DatiMovimenti.InnerXml = XML_Agenda_Movimento(
                                                    enum_TipoOperazioneDB.Scrittura,
                                                    ViewState("Piva"),
                                                    Sa_Cod,
                                                    0,
                                                    0, 0,
                                                    xCaricoScarico.ToString,
                                                    Mov_Desc,
                                                    Me.Txt_DataMovimento.Text,
                                                    AGRODATAFINE,
                                                    , ,
                                                    , Me.Txt_DataMovimento.Text,
                                                    AGRODATAFINE,
                                                    BaseCode, TopCode, Me.Txt_Ora.Text)


                    XML_Movimento = XML_DatiMovimenti.SelectSingleNode("Movimento")


                    '######################################################################################################
                    '----- DatiMovimenti_Dettagli

                    'Creo un tag volante di nome dati movimenti
                    XML_DatiMovimentiDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

                    'Imposto il tag volante come figlio di Movimento
                    XML_Movimento.AppendChild(XML_DatiMovimentiDettagli)

                    '######################################################################################################
                    '----- Movimento_Dettaglio

                    str_MovimentoDettaglio = XML_Genera_MovimentoDettaglio(enum_TipoOperazioneDB.Scrittura,
                                                                            ViewState("Piva"),
                                                                            Sa_Cod,
                                                                            0, 0, 0,
                                                                            Lav_Cod,
                                                                            Elem_Cod,
                                                                            Pro_Cod,
                                                                            Mat_Cod,
                                                                            Cod_Progetto,
                                                                            Cal_Cod,
                                                                            Fase_Cod,
                                                                            Lotto,
                                                                            Udm_Cod,
                                                                            Qta,
                                                                            PrezzoUnitario,
                                                                            PrezzoUnitario_Netto,
                                                                            , , ,
                                                                            Cau_Mov,
                                                                            Mov_Det_Des,
                                                                            Imponibile,
                                                                            Imponibile_Netto,
                                                                            Cod_Iva,
                                                                            Sconto_Magg,
                                                                            Ric_Cod,
                                                                            Cod_Conto,
                                                                            Anno,
                                                                            , ,
                                                                            BaseCode,
                                                                            TopCode,
                                                                            Magazzino_Cod,
                                                                            , ,
                                                                            Udm_Cod_Extra,
                                                                            Qta_Extra,
                                                                            Prezzo_Effettivo,
                                                                            Iva,
                                                                           N, P2O5, K2O, Cu,
                                                                           Regolamento_Cod_Ferti,
                                                                           Rif_Esterno, Rif_Esterno_2)

                    XML_DatiMovimentiDettagli.InnerXml = str_MovimentoDettaglio

                    XML_MovimentoDettaglio = XML_DatiMovimentiDettagli.SelectSingleNode("Movimento_Dettaglio")

                    '######################################################################################################
                    '----- DatiZoo_Animali_Anagrafe

                    XML_DatiZoo_Animale_Anagrafe = XmlDoc.CreateElement("DatiZoo_Animali_Anagrafe")

                    XML_MovimentoDettaglio.AppendChild(XML_DatiZoo_Animale_Anagrafe)

                    '######################################################################################################
                    '----- Zoo_Animale_Anagrafe

                    str_XML_ZooAnimale = XML_Zoo_Animale(enum_TipoOperazioneDB.Scrittura,
                                                        ViewState("Piva"),
                                                        Sa_Cod,
                                                        Cod_Progetto,
                                                        Matricola,
                                                        Gen_Cod,
                                                        Spe_Cod,
                                                        Ipro_Cod,
                                                        Raz_Cod,
                                                        Cat_Cod,
                                                        Spe_Des,
                                                        Ipro_Des,
                                                        Raz_Des,
                                                        Nome,
                                                        Collare,
                                                        Nome_Aia,
                                                        Matricola_Aia,
                                                        Dat_Nascita,
                                                        Dat_Nascita,
                                                        ,
                                                        Stato_Nascita,
                                                        Provincia_Nascita,
                                                        AUA_Azi_Nascita,
                                                        AUSL_Azi_Nascita,
                                                        Sesso,
                                                        Mat_Padre,
                                                        Mat_Madre,
                                                        , , ,
                                                        Peso,
                                                        Data_Pesa,
                                                        Metodo_Produzione,
                                                        ,
                                                        Conversione_Inizio,
                                                        Conversione_Fine,
                                                        BaseCode,
                                                        TopCode)


                    XML_DatiZoo_Animale_Anagrafe.InnerXml = str_XML_ZooAnimale

                Else

                    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

                    ' TUTTI I CASI TRANNE TRASFERIMENTO E CAU_ANIMALE 

                    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§




                    '#######################################################
                    '################  DATI  MOVIMENTI    ##################
                    '#######################################################

                    '----- DatiMovimenti

                    XML_DatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

                    XML_Agenda.AppendChild(XML_DatiMovimenti)


                    '#######################################################
                    '################   MOVIMENTO CONTABILE    #############
                    '#######################################################


                    Select Case Qs_Lav_Cod

                        Case LAVCOD_ACQUISTO, LAVCOD_VENDITA

                            XML_MovimentoContabile = XML_2_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura,
                                           ViewState("Piva"),
                                           Sa_Cod,
                                           0,
                                           0,
                                            CAU_REGISTRAZIONI,
                                           Mov_Desc_Contabile,
                                           Me.Txt_DataMovimento.Text,
                                           Me.Txt_Ora.Text,
                                           Me.Txt_DataMovimento.Text,
                                           AGRODATAFINE,
                                           ,
                                           "",
                                           0,
                                           "",
                                           ,
                                           ,
                                           ,
                                           ,
                                           0,
                                           0,
                                           ,
                                           ,
                                           ,
                                           ,
                                           ,
                                           ,
                                           ,
                                           "",
                                           0,
                                           0,
                                           0,
                                           0,
                                           "",
                                           "",
                                           0,
                                           ,
                                           Me.Txt_DataMovimento.Text,
                                           AGRODATAFINE,
                                           BaseCode,
                                           TopCode,
                                           XmlDoc)

                            XML_DatiMovimenti.AppendChild(XML_MovimentoContabile)


                            '#######################################################
                            '##################   PAGAMENTI   ######################
                            '#######################################################


                    End Select


                    '#######################################################
                    '###########   MOVIMENTO DI CARICO / SCARICO    ########
                    '#######################################################

                    'Movimento = New Movimento

                    'Movimento.Id_Agenda = Qs_IdAgenda
                    'Movimento.Piva = ViewState("Piva")
                    'Movimento.Sa_Cod = Sa_Cod
                    'Movimento.Data = Me.Txt_DataMovimento.Text
                    'Movimento.Lav_Cod = Lav_Cod
                    'Movimento.Cau_Mov = xCaricoScarico.ToString
                    'Movimento.Mov_Desc = Mov_Desc
                    'Movimento.Ora = Me.Txt_Ora.Text
                    'Movimento.Data_Registrazione = Me.Txt_DataMovimento.Text
                    'Movimento.Scadenza = AGRODATAFINE

                    'Movimento.BaseCode = BaseCode
                    'Movimento.TopCode = TopCode

                    'Agenda.Movimenti.Add(Movimento)


                    XML_MovimentoMagazzino = XML_2_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura,
                                                ViewState("Piva"),
                                                Sa_Cod,
                                                0,
                                                0,
                                                xCaricoScarico.ToString,
                                                Mov_Desc,
                                                   Me.Txt_DataMovimento.Text,
                                                Me.Txt_Ora.Text,
                                                Me.Txt_DataMovimento.Text,
                                                AGRODATAFINE,
                                                ,
                                                "",
                                                0,
                                                "",
                                                ,
                                                ,
                                                ,
                                                ,
                                                0,
                                                0,
                                                ,
                                                ,
                                                ,
                                                ,
                                                ,
                                                ,
                                                ,
                                                "",
                                                0,
                                                0,
                                                0,
                                                0,
                                                "",
                                                "",
                                                0,
                                                ,
                                                Me.Txt_DataMovimento.Text,
                                                AGRODATAFINE,
                                                BaseCode,
                                                TopCode,
                                                XmlDoc)

                    XML_DatiMovimenti.AppendChild(XML_MovimentoMagazzino)

                    '#######################################################
                    '##############  DATI MOVIMENTI DETTAGLI    ############
                    '#######################################################

                    XML_DatiMovimentiDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

                    XML_MovimentoMagazzino.AppendChild(XML_DatiMovimentiDettagli)

                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    '#######################################################
                    '################   MOVIMENTI DETTAGLIO    #############
                    '#######################################################

                    'Movimento_Dettaglio = New Movimento_Dettaglio

                    'Movimento_Dettaglio.Id_Agenda = Qs_IdAgenda
                    'Movimento_Dettaglio.Piva = ViewState("Piva")
                    'Movimento_Dettaglio.Sa_Cod = Sa_Cod
                    'Movimento_Dettaglio.Data = Me.Txt_DataMovimento.Text
                    'Movimento_Dettaglio.Lav_Cod = Lav_Cod
                    'Movimento_Dettaglio.Cau_Mov = xCaricoScarico.ToString
                    'Movimento_Dettaglio.Mov_Det_Des = Mov_Det_Des
                    'Movimento_Dettaglio.Elem_Cod = Elem_Cod
                    'Movimento_Dettaglio.Pro_Cod = Pro_Cod
                    'Movimento_Dettaglio.Mat_Cod = Mat_Cod
                    'Movimento_Dettaglio.Cod_Progetto = Cod_Progetto
                    'Movimento_Dettaglio.Fase_Cod = Fase_Cod
                    'Movimento_Dettaglio.Lotto = Lotto
                    'Movimento_Dettaglio.Cal_Cod = Cal_Cod
                    'Movimento_Dettaglio.Udm_Cod = Udm_Cod
                    'Movimento_Dettaglio.Udm_Cod_Extra = Udm_Cod_Extra
                    'Movimento_Dettaglio.Qta = Qta
                    'Movimento_Dettaglio.Qta_Extra = Qta_Extra

                    'Movimento_Dettaglio.Prezzo_Effettivo = Prezzo_Effettivo
                    'Movimento_Dettaglio.Prezzo_Unitario = PrezzoUnitario
                    'Movimento_Dettaglio.Prezzo_Unitario_Netto = PrezzoUnitario_Netto

                    'Movimento_Dettaglio.Sconto = Sconto_Magg
                    'Movimento_Dettaglio.Imponibile = Imponibile
                    'Movimento_Dettaglio.Imponibile_Netto = Imponibile_Netto

                    'Movimento_Dettaglio.Cod_Iva = Cod_Iva
                    'Movimento_Dettaglio.Iva = Iva

                    'Movimento_Dettaglio.Anno = Anno
                    'Movimento_Dettaglio.Ric_Cod = Ric_Cod
                    'Movimento_Dettaglio.Cod_Conto = Cod_Conto
                    'Movimento_Dettaglio.Extra_Int = Extra_int

                    'Movimento_Dettaglio.BaseCode = BaseCode
                    'Movimento_Dettaglio.TopCode = TopCode

                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)


                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                    'Movimento_Destinazione = New Movimento_Destinazione

                    'Movimento_Destinazione.Id_Agenda = Qs_IdAgenda
                    'Movimento_Destinazione.Piva = ViewState("Piva")
                    'Movimento_Destinazione.Sa_Cod = Sa_Cod
                    'Movimento_Destinazione.Appezza = 0
                    'Movimento_Destinazione.Id_Destinazione = Magazzino_Cod
                    'Movimento_Destinazione.Tipo = MAGAZZINO
                    'Movimento_Destinazione.Qta = Qta
                    'Movimento_Destinazione.Data = Me.Txt_DataMovimento.Text
                    'Movimento_Destinazione.BaseCode = BaseCode
                    'Movimento_Destinazione.TopCode = TopCode

                    'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni.Add(Movimento_Destinazione)


                    str_MovimentoDettaglio = XML_Genera_MovimentoDettaglio(enum_TipoOperazioneDB.Scrittura,
                                                                        ViewState("Piva"),
                                                                        Sa_Cod,
                                                                        0, 0, 0,
                                                                        Lav_Cod,
                                                                        Elem_Cod,
                                                                        Pro_Cod,
                                                                        Mat_Cod,
                                                                        Cod_Progetto,
                                                                        Cal_Cod,
                                                                        Fase_Cod,
                                                                        Lotto,
                                                                        Udm_Cod,
                                                                        Qta,
                                                                        PrezzoUnitario,
                                                                        PrezzoUnitario_Netto,
                                                                        , ,
                                                                        Extra_int,
                                                                        Cau_Mov,
                                                                        Mov_Det_Des,
                                                                        Imponibile,
                                                                        Imponibile_Netto,
                                                                        Cod_Iva,
                                                                        Sconto_Magg,
                                                                        Ric_Cod,
                                                                        Cod_Conto,
                                                                        Anno,
                                                                        , ,
                                                                        BaseCode,
                                                                        TopCode,
                                                                        Magazzino_Cod,
                                                                        , ,
                                                                        Udm_Cod_Extra,
                                                                        Qta_Extra,
                                                                        Prezzo_Effettivo,
                                                                        Iva,
                                                                           N, P2O5, K2O, Cu,
                                                                           Regolamento_Cod_Ferti,
                                                                           Rif_Esterno, Rif_Esterno_2)


                    XML_DatiMovimentiDettagli.InnerXml = str_MovimentoDettaglio

                End If

        End Select

        Return XmlDoc.OuterXml

    End Function


    '########################################################################################
    ''' <summary>
    ''' è usata solo dalla <see cref="Salva_Tutto"/>
    ''' </summary>
    Private Function Genera_Agenda(ByVal Operazione As enum_TipoOperazioneDB, Optional ByVal Dr As DataRow = Nothing) As Operazione_Agenda

        Dim BaseCode As Integer
        Dim TopCode As Integer
        Dim Des_Lib As String = ""
        Dim Mov_Desc As String
        Dim Mov_Desc_Contabile As String = ""
        Dim Mov_Det_Des As String = ""
        Dim Lav_Cod As Integer

        Dim array_temp As String()
        Dim Sa_Cod As Integer
        Dim Magazzino_Cod As Integer

        Dim Cau_Mov As String = ""
        Dim Elem_Cod As Integer
        Dim Pro_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Cod_Progetto As Integer
        Dim Fase_Cod As Integer
        Dim Lotto As String = ""
        Dim Cal_Cod As Integer
        Dim Udm_Cod As Integer
        Dim Extra_int As Integer

        Dim Gen_Cod As Integer
        Dim Spe_Cod As Integer
        Dim Ipro_Cod As Integer
        Dim Raz_Cod As Integer
        Dim Cat_Cod As Integer
        Dim Spe_Des As String
        Dim Ipro_Des As String
        Dim Raz_Des As String

        Dim Qta As Decimal
        Dim PrezzoUnitario, PrezzoUnitario_Netto As Decimal

        Dim Udm_Cod_Extra As Decimal = 0
        Dim Qta_Extra As Decimal = 0
        Dim Prezzo_Effettivo As Decimal = 0
        Dim Sconto_Magg As Decimal = 0
        Dim Variazione_Perc As String = ""
        Dim Variazione As Decimal = 0
        Dim Imponibile As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Cod_Iva As Integer = 0
        Dim Iva As Decimal = 0
        Dim Anno As Integer = 0
        Dim Ric_Cod As Integer = 0
        Dim Cod_Conto As Integer = 0
        Dim Conto As String = ""

        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Cu As Decimal = 0
        Dim Regolamento_Cod_Ferti As Integer = 0

        Dim Matricola As String = ""
        Dim Nome As String = ""
        Dim Collare As String = ""
        Dim Nome_Aia As String = ""
        Dim Matricola_Aia As String = ""
        Dim Dat_Nascita As Date = AGRODATAINIZIO
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE
        Dim Stato_Nascita As String = "000"
        Dim Provincia_Nascita As String = "000"
        Dim AUA_Azi_Nascita As String = ""
        Dim AUSL_Azi_Nascita As String = ""
        Dim Sesso As String = "M"
        Dim Mat_Padre As String = ""
        Dim Mat_Madre As String = ""
        Dim CF_Proprietario As String = ""
        Dim CF_Detentore As String = ""
        Dim Presente As String = ""
        Dim Peso As Decimal = 0
        Dim Data_Pesa As Date = AGRODATAINIZIO
        Dim Metodo_Produzione As Integer
        Dim Conversione_Inizio As Date = AGRODATAINIZIO
        Dim Conversione_Fine As Date = AGRODATAINIZIO


        'modifica 12/06/2014, mostro le note della form prodotto
        Mov_Desc = TextBoxNote.Text

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS").ToString)

        Dim Rif_Esterno As String = ""
        Dim Rif_Esterno_2 As String = ""
        Recupera_DettagliNascosti(Rif_Esterno, Rif_Esterno_2)

        Select Case Qs_Tipo


            '####################################
            '############# STALLA ###############
            '####################################

            Case CAU_ANIMALE

                Elem_Cod = ZOO_CONSISTENZA
                Pro_Cod = 0
                Mat_Cod = 0
                Lotto = ""
                Cal_Cod = 0
                Udm_Cod = enum_UnitaMisura.Numero

                'setto le variabili per differenziare il caso del carico e dello scarico.
                Select Case xCaricoScarico


                    Case enum_Agenda_Causali.CARICO

                        '-----------------------------------------------------------------
                        '------------------------- CARICO --------------------------------
                        '-----------------------------------------------------------------

                        array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                        Cau_Mov = CAU_CARICO
                        Lav_Cod = 3001

                        Des_Lib = DirectCast(GetLocalResourceObject("AumentoConsistenzeZootecniche"), String)
                        'Mov_Desc = "Aumento Consistenze Zootecniche"


                    Case enum_Agenda_Causali.SCARICO

                        '-----------------------------------------------------------------
                        '------------------------- SCARICO -------------------------------
                        '-----------------------------------------------------------------


                        array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                        Cau_Mov = CAU_SCARICO
                        Lav_Cod = 3002

                        Des_Lib = DirectCast(GetLocalResourceObject("DecrementoConsistenzeZootecniche"), String)
                        'Mov_Desc = "Decremento Consistenze Zootecniche"

                End Select


                If Dr IsNot Nothing Then

                    '-----------------------------------------------------------------
                    '------------------------- SCRITTURA -----------------------------
                    '-----------------------------------------------------------------

                    Cod_Progetto = Dr.Item("Cod_Progetto")
                    'Fase_Cod = Dr.Item("Fase_Cod")

                    Gen_Cod = Dr.Item("Gen_Cod")
                    Spe_Cod = Dr.Item("Spe_Cod")
                    Ipro_Cod = Dr.Item("Ipro_Cod")
                    Raz_Cod = Dr.Item("Raz_Cod")
                    Cat_Cod = Dr.Item("Cat_Cod")
                    Spe_Des = Dr.Item("Spe_Des")
                    Ipro_Des = Dr.Item("Ipro_Des")
                    Raz_Des = Dr.Item("Raz_Des")
                    Matricola = Dr.Item("Matricola")
                    Nome = Dr.Item("Nome")
                    Collare = Dr.Item("Collare")
                    Nome_Aia = Dr.Item("Nome_Aia")
                    Matricola_Aia = Dr.Item("Matricola_Aia")
                    Dat_Nascita = Dr.Item("Dat_Nascita")
                    Stato_Nascita = Dr.Item("Stato_Nascita")
                    Provincia_Nascita = Dr.Item("Prov_Nascita")
                    AUA_Azi_Nascita = Dr.Item("AUA_Azi_Nascita")
                    AUSL_Azi_Nascita = Dr.Item("AUSL_Azi_Nascita")

                    Mov_Det_Des = Ipro_Des & " - " & Raz_Des & " - " & Matricola & ": " & Nome

                    Sesso = Dr.Item("Sesso")
                    Mat_Padre = Dr.Item("Mat_Padre")
                    Mat_Madre = Dr.Item("Mat_Madre")
                    Peso = Dr.Item("Peso")
                    Data_Pesa = Dr.Item("Data_Pesa")

                    Metodo_Produzione = Dr.Item("Metodo_Produzione")
                    Conversione_Inizio = Dr.Item("Conversione_Inizio")
                    Conversione_Fine = Dr.Item("Conversione_Fine")

                    Qta = Dr.Item("Qta_Reale")
                    PrezzoUnitario = Dr.Item("Prezzo_Reale")
                    PrezzoUnitario_Netto = PrezzoUnitario


                Else

                    '-----------------------------------------------------------------
                    '------------------------- MODIFICA -----------------------------
                    '-----------------------------------------------------------------



                End If


                '######################################################
                '#################### MAGAZZINO #######################
                '######################################################

            Case Else

                If Dr IsNot Nothing Then

                    '-----------------------------------------------------------------
                    '------------------------- SCRITTURA -----------------------------
                    '-----------------------------------------------------------------

                    Elem_Cod = Dr.Item("Elem_Cod")
                    Pro_Cod = Dr.Item("Pro_Cod")
                    Mat_Cod = Dr.Item("Mat_Cod")
                    Cod_Progetto = Dr.Item("Cod_Progetto")
                    Fase_Cod = Dr.Item("Fase_Cod")
                    Lotto = Dr.Item("Lotto")
                    Cal_Cod = Dr.Item("Cal_Cod")

                    Udm_Cod = Dr.Item("Udm_Cod")
                    Qta = Dr.Item("Qta")
                    Extra_int = 0
                    ' nicoletta
                    If Dr.Item("Extra_Int") <> "0" Then

                        Extra_int = Udm_Cod

                        Select Case Udm_Cod
                            Case enum_UnitaMisura.Quintali
                                Qta = Qta * 100
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Tonnellate
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Metri_Cubi
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.Litri
                        End Select

                    End If


                    Mov_Det_Des = Dr.Item("Prodotto")
                    PrezzoUnitario = Dr.Item("Prezzo_Unitario")
                    PrezzoUnitario_Netto = Dr.Item("Prezzo_Unitario_Netto")
                    Udm_Cod_Extra = Dr.Item("Udm_Cod_Extra")
                    Qta_Extra = Dr.Item("Qta_Extra")

                    Sconto_Magg = Dr.Item("Cod_Variazione")
                    Cod_Iva = Dr.Item("Cod_Iva")
                    Iva = Dr.Item("Iva")
                    Imponibile = Dr.Item("Imponibile")
                    Imponibile_Netto = Dr.Item("Imponibile_Netto")
                    Prezzo_Effettivo = Dr.Item("Prezzo_Effettivo")
                    Anno = Dr.Item("Anno")
                    Ric_Cod = Dr.Item("Ric_Cod")
                    Cod_Conto = Dr.Item("Cod_Conto")
                    Conto = Dr.Item("Conto")

                    N = Dr.Item("N")
                    P2O5 = Dr.Item("P2O5")
                    K2O = Dr.Item("K2O")
                    Cu = Dr.Item("Cu")
                    Regolamento_Cod_Ferti = Dr.Item("Regolamento_Cod_Ferti")


                    'setto le variabili per differenziare il caso del carico e dello scarico.
                    Select Case xCaricoScarico

                        Case enum_Agenda_Causali.CARICO

                            '-----------------------------------------------------------------
                            '------------------------- CARICO --------------------------------
                            '-----------------------------------------------------------------

                            Magazzino_Cod = Dr.Item("Id_Destinazione")
                            Sa_Cod = Dr.Item("SaCod_Destinazione")
                            Cau_Mov = CAU_CARICO
                            'modifica 12 06 2014, mostro ne note della form prodotto
                            'Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"


                            Select Case Qs_Lav_Cod

                                Case LAVCOD_ACQUISTO
                                    Lav_Cod = LAVCOD_ACQUISTO
                                    Des_Lib = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_FATTURA_RICEVUTA
                                    Lav_Cod = LAVCOD_FATTURA_RICEVUTA
                                    Des_Lib = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                                    Lav_Cod = LAVCOD_NOTA_ACCREDITO_EMESSA
                                    Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case Else
                                    Lav_Cod = LAVCOD_CARICO
                                    Des_Lib = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"
                                    Mov_Desc_Contabile = ""

                            End Select


                        Case enum_Agenda_Causali.SCARICO

                            '-----------------------------------------------------------------
                            '------------------------- SCARICO -------------------------------
                            '-----------------------------------------------------------------


                            Magazzino_Cod = Dr.Item("Id_Provenienza")
                            Sa_Cod = Dr.Item("SaCod_Provenienza")
                            Cau_Mov = CAU_SCARICO
                            'modifica 12 06 2014, mostro ne note della form prodotto
                            'Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"

                            Select Case Qs_Lav_Cod

                                Case LAVCOD_VENDITA
                                    Lav_Cod = LAVCOD_VENDITA
                                    Des_Lib = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_FATTURA_EMESSA
                                    Lav_Cod = LAVCOD_FATTURA_EMESSA
                                    Des_Lib = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                    Lav_Cod = LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                    Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Dr.Item("Prodotto") & ""
                                    Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiRicevutaEmessaDi_"), String) & Dr.Item("Prodotto") & ""

                                Case Else
                                    Lav_Cod = LAVCOD_SCARICO
                                    Des_Lib = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"
                                    Mov_Desc_Contabile = ""

                            End Select


                    End Select


                Else

                    '-----------------------------------------------------------------
                    '------------------------- MODIFICA -----------------------------
                    '-----------------------------------------------------------------

                    Dim Progetto_Des As String = ""
                    Dim Cal_Des As String = ""
                    Dim Udm_des As String = ""
                    Dim Str_Errore As String = ""

                    Elem_Cod = Me.cmb_Categoria.SelectedItem.Value

                    Select Case Elem_Cod

                        Case ALTRI_BENI, SERVIZI

                            If Me.Txt_BeniStrumentali.Text = "" Then
                                Str_Errore &= DirectCast(GetLocalResourceObject("InserisciIlNomeDelProdotto"), String) & vbCrLf
                            Else
                                Mov_Det_Des = Me.Txt_BeniStrumentali.Text
                            End If

                            '-------------------------------------------------------------

                        Case Else

                            If IsNothing(cmb_Prodotti.SelectedItem) OrElse cmb_Prodotti.SelectedItem.Value = "0" Then
                                Str_Errore &= AgronicaAgenda_2010.SelezionareUnProdotto & vbCrLf
                            Else
                                If cmb_Prodotti.SelectedItem.Value.Split("|")(0) > 0 Then
                                    Pro_Cod = Me.cmb_Prodotti.SelectedItem.Value.Split("|")(0)
                                    Mov_Det_Des = Me.cmb_Prodotti.SelectedItem.Text
                                Else
                                    Mat_Cod = -Me.cmb_Prodotti.SelectedItem.Value
                                    Mov_Det_Des = Me.cmb_Prodotti.SelectedItem.Text
                                End If
                            End If

                    End Select


                    Recupera_DettagliProdotto(Str_Errore,
                                             Cod_Progetto,
                                             Progetto_Des,
                                             Fase_Cod,
                                             Lotto,
                                             Cal_Cod,
                                             Cal_Des,
                                             Udm_Cod,
                                             Udm_des,
                                             Qta,
                                             PrezzoUnitario,
                                             PrezzoUnitario_Netto)

                    Extra_int = 0
                    If Elem_Cod = FERTILIZZANTI Then

                        Select Case Udm_Cod

                            Case enum_UnitaMisura.Quintali
                                Extra_int = Udm_Cod
                                Qta = Qta * 100
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Tonnellate
                                Extra_int = Udm_Cod
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.KG
                            Case enum_UnitaMisura.Metri_Cubi
                                Extra_int = Udm_Cod
                                Qta = Qta * 1000
                                Udm_Cod = enum_UnitaMisura.Litri
                        End Select

                        Recupera_DettagliFertilizzante(Str_Errore, N, P2O5, K2O, Cu, Regolamento_Cod_Ferti)
                    End If

                    '===================================================
                    'DETTAGLI ECONOMICI
                    If Me.Pannello_Economico.Visible Then

                        Recupera_DettagliEconomici(Udm_Cod_Extra,
                                                    Qta_Extra,
                                                    Prezzo_Effettivo,
                                                    Sconto_Magg,
                                                    Variazione_Perc,
                                                    Variazione,
                                                    Imponibile,
                                                    Imponibile_Netto,
                                                    Cod_Iva,
                                                    Iva,
                                                    Anno,
                                                    Ric_Cod,
                                                    Cod_Conto,
                                                    Conto)

                    Else
                        Imponibile = Qta * PrezzoUnitario
                        Imponibile_Netto = Qta * PrezzoUnitario_Netto
                    End If
                    '============================================================


                    Select Case Qs_Mode.ToLower


                        '######################################################
                        '#################### TRASFERIMENTO ###################
                        '######################################################

                        Case "trasferimento"

                            Des_Lib = DirectCast(GetLocalResourceObject("TrasferimentoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                            Lav_Cod = LAVCOD_TRASFERIMENTO
                            Sa_Cod = Me.Cmb_Provenienza.SelectedValue.Split("|")(1)

                            'modifica 12 06 2014, mostro ne note della form prodotto
                            Select Case xCaricoScarico
                                Case enum_Agenda_Causali.CARICO
                                    ' Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                Case enum_Agenda_Causali.SCARICO
                                    'Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                            End Select


                            '######################################################
                            '################# MAGAZZINO  #########################
                            '######################################################

                        Case Else

                            'setto le variabili per differenziare il caso del carico e dello scarico.
                            Select Case xCaricoScarico

                                Case enum_Agenda_Causali.CARICO

                                    '-----------------------------------------------------------------
                                    '------------------------- CARICO --------------------------------
                                    '-----------------------------------------------------------------

                                    Cau_Mov = CAU_CARICO
                                    'If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                    'Else
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                    'End If

                                    Select Case Qs_Lav_Cod

                                        Case LAVCOD_ACQUISTO
                                            Lav_Cod = LAVCOD_ACQUISTO
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("AcquistoDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case LAVCOD_FATTURA_RICEVUTA
                                            Lav_Cod = LAVCOD_FATTURA_RICEVUTA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case LAVCOD_NOTA_ACCREDITO_EMESSA
                                            Lav_Cod = LAVCOD_NOTA_ACCREDITO_EMESSA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case Else
                                            Lav_Cod = LAVCOD_CARICO
                                            Mov_Desc_Contabile = ""
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("CaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                            End If

                                    End Select


                                    Select Case Elem_Cod

                                        Case MACCHINE, ALTRI_BENI, SERVIZI
                                            Magazzino_Cod = 0
                                            Sa_Cod = 0

                                        Case Else
                                            array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                                            Magazzino_Cod = array_temp(0)
                                            Sa_Cod = array_temp(1)

                                    End Select


                                Case enum_Agenda_Causali.SCARICO

                                    '-----------------------------------------------------------------
                                    '------------------------- SCARICO -------------------------------
                                    '-----------------------------------------------------------------

                                    Cau_Mov = CAU_SCARICO
                                    'If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                    'Else
                                    '    Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                    'End If

                                    Select Case Qs_Lav_Cod

                                        Case LAVCOD_VENDITA
                                            Lav_Cod = LAVCOD_VENDITA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("VenditaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case LAVCOD_FATTURA_EMESSA
                                            Lav_Cod = LAVCOD_FATTURA_EMESSA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ")"
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.Txt_BeniStrumentali.Text & ")"
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                            End If

                                        Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                            Lav_Cod = LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Me.Txt_BeniStrumentali.Text & ""
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                                Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String) & Me.cmb_Prodotti.SelectedItem.Text & ""
                                            End If

                                        Case Else
                                            Lav_Cod = LAVCOD_SCARICO
                                            Mov_Desc_Contabile = ""
                                            If Trim(Me.Txt_BeniStrumentali.Text) <> "" Then
                                                Des_Lib = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.Txt_BeniStrumentali.Text & ")"
                                            Else
                                                Des_Lib = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Me.cmb_Prodotti.SelectedItem.Text & ")"
                                            End If

                                    End Select

                                    Select Case Elem_Cod

                                        Case 1, ALTRI_BENI, SERVIZI
                                            Magazzino_Cod = 0
                                            Sa_Cod = 0

                                        Case Else
                                            array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                                            Magazzino_Cod = array_temp(0)
                                            Sa_Cod = array_temp(1)

                                    End Select


                            End Select


                    End Select


                End If

        End Select

        Dim Pendente As Integer
        Dim Descrizione As String = ""
        Dim ID_Destinazione As Integer = 0
        Pendente = GetPendente(Elem_Cod, Descrizione, Pro_Cod, Mat_Cod, Fase_Cod, Cal_Cod, Udm_Cod, ID_Destinazione)


        Dim Extra_Str As String = ""
        Dim Extra_Date As Date = AGRODATAINIZIO

        If cmb_Causale.SelectedValue = CStr(enum_Pendenza.Furto) Then
            'se sono in causale furto allora abilito le textbox per scrivere la denuncia
            Extra_Str = Txt_NumeroDenuncia.Text
            If IsDate(Txt_DataDenuncia.Text) Then
                Extra_Date = CDate(Txt_DataDenuncia.Text)
            End If
        End If




        '-----------------------------------------------------------------------

        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Destinazione As Movimento_Destinazione


        '#######################################################
        '##################   AGENDA   #########################
        '#######################################################

        Dim Agenda As New Operazione_Agenda With {
            .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
            .Id_Agenda = Qs_IdAgenda,
            .Data = Me.Txt_DataMovimento.Text,
            .Piva = ViewState("Piva"),
            .Sa_Cod = Sa_Cod,
            .Lav_Cod = Lav_Cod,
            .Des_Lib = Des_Lib,
            .BaseCode = BaseCode,
            .TopCode = TopCode
        }
        Agenda.Movimenti = New List(Of Movimento)


        Select Case Qs_Mode.ToLower


            Case "trasferimento"


                '#######################################################
                '##################   MOVIMENTI    #####################
                '#######################################################

                For i As Integer = 0 To 1

                    Movimento = New Movimento

                    If i = 0 Then

                        'PROVENIENZA
                        Cau_Mov = CAU_SCARICO
                        'Mov_Desc = "Scarico da Trasferimento di magazzino (" & Me.cmb_Prodotti.SelectedItem.Text & ")"

                        array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                    Else
                        'DESTINAZIONE
                        Cau_Mov = CAU_CARICO
                        'Mov_Desc = "Carico da Trasferimento di magazzino (" & Me.cmb_Prodotti.SelectedItem.Text & ")"

                        array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                        Magazzino_Cod = array_temp(0)
                        Sa_Cod = array_temp(1)

                    End If

                    Movimento.Id_Agenda = Qs_IdAgenda
                    Movimento.Piva = ViewState("Piva")
                    Movimento.Sa_Cod = Sa_Cod
                    Movimento.Data = Me.Txt_DataMovimento.Text
                    Movimento.Lav_Cod = Lav_Cod
                    Movimento.Cau_Mov = Cau_Mov
                    Movimento.Mov_Desc = Mov_Desc
                    Movimento.Ora = Me.Txt_Ora.Text
                    Movimento.Data_Registrazione = Me.Txt_DataMovimento.Text
                    Movimento.Scadenza = AGRODATAFINE

                    Movimento.BaseCode = BaseCode
                    Movimento.TopCode = TopCode

                    Agenda.Movimenti.Add(Movimento)


                    '#######################################################
                    '################   MOVIMENTI DETTAGLIO    #############
                    '#######################################################


                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    Movimento_Dettaglio = New Movimento_Dettaglio

                    Movimento_Dettaglio.Id_Agenda = Qs_IdAgenda
                    Movimento_Dettaglio.Piva = ViewState("Piva")
                    Movimento_Dettaglio.Sa_Cod = Sa_Cod
                    Movimento_Dettaglio.Data = Me.Txt_DataMovimento.Text
                    Movimento_Dettaglio.Lav_Cod = Lav_Cod
                    Movimento_Dettaglio.Cau_Mov = Cau_Mov
                    Movimento_Dettaglio.Mov_Det_Des = Mov_Det_Des
                    Movimento_Dettaglio.Elem_Cod = Elem_Cod
                    Movimento_Dettaglio.Pro_Cod = Pro_Cod
                    Movimento_Dettaglio.Mat_Cod = Mat_Cod
                    Movimento_Dettaglio.Cod_Progetto = Cod_Progetto
                    Movimento_Dettaglio.Fase_Cod = Fase_Cod
                    Movimento_Dettaglio.Lotto = Lotto
                    Movimento_Dettaglio.Cal_Cod = Cal_Cod
                    Movimento_Dettaglio.Udm_Cod = Udm_Cod
                    Movimento_Dettaglio.Udm_Cod_Extra = Udm_Cod_Extra
                    Movimento_Dettaglio.Qta = Qta
                    Movimento_Dettaglio.Qta_Extra = Qta_Extra
                    Movimento_Dettaglio.Pendente = Pendente

                    Movimento_Dettaglio.Prezzo_Effettivo = Prezzo_Effettivo
                    Movimento_Dettaglio.Prezzo_Unitario = PrezzoUnitario
                    Movimento_Dettaglio.Prezzo_Unitario_Netto = PrezzoUnitario_Netto

                    Movimento_Dettaglio.Sconto = Sconto_Magg
                    Movimento_Dettaglio.Imponibile = Imponibile
                    Movimento_Dettaglio.Imponibile_Netto = Imponibile_Netto

                    Movimento_Dettaglio.Cod_Iva = Cod_Iva
                    Movimento_Dettaglio.Iva = Iva

                    Movimento_Dettaglio.Anno = Anno
                    Movimento_Dettaglio.Ric_Cod = Ric_Cod
                    Movimento_Dettaglio.Cod_Conto = Cod_Conto
                    Movimento_Dettaglio.Extra_Int = Extra_int
                    Movimento_Dettaglio.Extra_Str = Extra_Str
                    Movimento_Dettaglio.Extra_Date = Extra_Date

                    Movimento_Dettaglio.Contabilizzato = NONCONTABILE

                    Movimento_Dettaglio.Rif_Esterno = Rif_Esterno
                    Movimento_Dettaglio.Rif_Esterno_2 = Rif_Esterno_2

                    Movimento_Dettaglio.BaseCode = BaseCode
                    Movimento_Dettaglio.TopCode = TopCode

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)


                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                    '#######################################################
                    '################   MOVIMENTI DESTINAZIONE    ##########
                    '#######################################################

                    Movimento_Destinazione = New Movimento_Destinazione With {
                        .Id_Agenda = Qs_IdAgenda,
                        .Piva = ViewState("Piva"),
                        .Sa_Cod = Sa_Cod,
                        .Appezza = 0,
                        .Id_Destinazione = Magazzino_Cod,
                        .Tipo = MAGAZZINO,
                        .Qta = Qta,
                        .Data = Me.Txt_DataMovimento.Text,
                        .BaseCode = BaseCode,
                        .TopCode = TopCode
                    }

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni.Add(Movimento_Destinazione)

                    '#######################################################
                    '################   MOVIMENTO TECNICO    ###############
                    '#######################################################

                    If Cau_Mov = CAU_CARICO AndAlso Elem_Cod = FERTILIZZANTI Then

                        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                            .Id_Agenda = Qs_IdAgenda,
                            .Piva = ViewState("Piva"),
                            .Sa_Cod = Sa_Cod,
                            .N = N,
                            .P = P2O5,
                            .K = K2O,
                            .Cu = Cu,
                            .Extra_Int = Regolamento_Cod_Ferti,
                            .Data = Me.Txt_DataMovimento.Text,
                            .BaseCode = BaseCode,
                            .TopCode = TopCode
                        }

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                    End If

                Next


            Case Else



                If Qs_Tipo = CAU_ANIMALE Then

                    ''######################################################################################################
                    ''----- DatiMovimenti

                    ''Creo un tag volante di nome dati movimenti
                    'XML_DatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

                    ''Rendo il tag appena creato figlio del nodo agenda
                    'XML_Agenda.AppendChild(XML_DatiMovimenti)

                    ''######################################################################################################
                    ''----- Movimento

                    ''Imposto il tag Movimento creato nella routine XML_Agenda_Movimento come figlio del tag XML_DatiMovimenti
                    'XML_DatiMovimenti.InnerXml = XML_Agenda_Movimento( _
                    '                                enum_TipoOperazioneDB.Scrittura, _
                    '                                ViewState("Piva"), _
                    '                                Sa_Cod, _
                    '                                0, _
                    '                                0, 0, _
                    '                                xCaricoScarico.ToString, _
                    '                                Mov_Desc, _
                    '                                Me.Txt_DataMovimento.Text, _
                    '                                #12/31/2100#, _
                    '                                , , _
                    '                                , Me.Txt_DataMovimento.Text, _
                    '                                #12/31/2100#, _
                    '                                BaseCode, TopCode, Me.Txt_Ora.Text)


                    'XML_Movimento = XML_DatiMovimenti.SelectSingleNode("Movimento")


                    ''######################################################################################################
                    ''----- DatiMovimenti_Dettagli

                    ''Creo un tag volante di nome dati movimenti
                    'XML_DatiMovimentiDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

                    ''Imposto il tag volante come figlio di Movimento
                    'XML_Movimento.AppendChild(XML_DatiMovimentiDettagli)

                    ''######################################################################################################
                    ''----- Movimento_Dettaglio

                    'str_MovimentoDettaglio = XML_Genera_MovimentoDettaglio(enum_TipoOperazioneDB.Scrittura, _
                    '                                                        ViewState("Piva"), _
                    '                                                        Sa_Cod, _
                    '                                                        0, 0, 0, _
                    '                                                        Lav_Cod, _
                    '                                                        Elem_Cod, _
                    '                                                        Pro_Cod, _
                    '                                                        Mat_Cod, _
                    '                                                        Cod_Progetto, _
                    '                                                        Cal_Cod, _
                    '                                                        Fase_Cod, _
                    '                                                        Lotto, _
                    '                                                        Udm_Cod, _
                    '                                                        Qta, _
                    '                                                        PrezzoUnitario, _
                    '                                                        PrezzoUnitario_Netto, _
                    '                                                        , , , _
                    '                                                        Cau_Mov, _
                    '                                                        Mov_Det_Des, _
                    '                                                        Imponibile, _
                    '                                                        Imponibile_Netto, _
                    '                                                        Cod_Iva, _
                    '                                                        Sconto_Magg, _
                    '                                                        Ric_Cod, _
                    '                                                        Cod_Conto, _
                    '                                                        Anno, _
                    '                                                        , , _
                    '                                                        BaseCode, _
                    '                                                        TopCode, _
                    '                                                        Magazzino_Cod, _
                    '                                                        , , _
                    '                                                        Udm_Cod_Extra, _
                    '                                                        Qta_Extra, _
                    '                                                        Prezzo_Effettivo, _
                    '                                                        Iva)

                    'XML_DatiMovimentiDettagli.InnerXml = str_MovimentoDettaglio

                    'XML_MovimentoDettaglio = XML_DatiMovimentiDettagli.SelectSingleNode("Movimento_Dettaglio")

                    ''######################################################################################################
                    ''----- DatiZoo_Animali_Anagrafe

                    'XML_DatiZoo_Animale_Anagrafe = XmlDoc.CreateElement("DatiZoo_Animali_Anagrafe")

                    'XML_MovimentoDettaglio.AppendChild(XML_DatiZoo_Animale_Anagrafe)

                    ''######################################################################################################
                    ''----- Zoo_Animale_Anagrafe

                    'str_XML_ZooAnimale = XML_Zoo_Animale(enum_TipoOperazioneDB.Scrittura, _
                    '                                    ViewState("Piva"), _
                    '                                    Sa_Cod, _
                    '                                    Cod_Progetto, _
                    '                                    Matricola, _
                    '                                    Gen_Cod, _
                    '                                    Spe_Cod, _
                    '                                    Ipro_Cod, _
                    '                                    Raz_Cod, _
                    '                                    Cat_Cod, _
                    '                                    Spe_Des, _
                    '                                    Ipro_Des, _
                    '                                    Raz_Des, _
                    '                                    Nome, _
                    '                                    Collare, _
                    '                                    Nome_Aia, _
                    '                                    Matricola_Aia, _
                    '                                    Dat_Nascita, _
                    '                                    Dat_Nascita, _
                    '                                    , _
                    '                                    Stato_Nascita, _
                    '                                    Provincia_Nascita, _
                    '                                    AUA_Azi_Nascita, _
                    '                                    AUSL_Azi_Nascita, _
                    '                                    Sesso, _
                    '                                    Mat_Padre, _
                    '                                    Mat_Madre, _
                    '                                    , , , _
                    '                                    Peso, _
                    '                                    Data_Pesa, _
                    '                                    Metodo_Produzione, _
                    '                                    , _
                    '                                    Conversione_Inizio, _
                    '                                    Conversione_Fine, _
                    '                                    BaseCode, _
                    '                                    TopCode)


                    'XML_DatiZoo_Animale_Anagrafe.InnerXml = str_XML_ZooAnimale




                Else

                    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

                    ' TUTTI I CASI TRANNE TRASFERIMENTO E CAU_ANIMALE 

                    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

                    '#######################################################
                    '################   MOVIMENTO CONTABILE    #############
                    '#######################################################


                    Select Case Qs_Lav_Cod


                        Case LAVCOD_ACQUISTO, LAVCOD_VENDITA

                            Movimento = New Movimento

                            Movimento.Id_Agenda = Qs_IdAgenda
                            Movimento.Piva = ViewState("Piva")
                            Movimento.Sa_Cod = Sa_Cod
                            Movimento.Data = Me.Txt_DataMovimento.Text
                            Movimento.Lav_Cod = Lav_Cod
                            Movimento.Cau_Mov = CAU_REGISTRAZIONI
                            Movimento.Mov_Desc = Mov_Desc_Contabile
                            Movimento.Ora = Me.Txt_Ora.Text
                            Movimento.Data_Registrazione = Me.Txt_DataMovimento.Text
                            Movimento.Scadenza = AGRODATAFINE

                            Movimento.BaseCode = BaseCode
                            Movimento.TopCode = TopCode

                            Agenda.Movimenti.Add(Movimento)

                            'XML_MovimentoContabile = XML_2_Agenda_Movimento(
                            '               enum_TipoOperazioneDB.Scrittura,
                            '               ViewState("Piva"),
                            '               Sa_Cod,
                            '               0,
                            '               0,
                            '                CAU_REGISTRAZIONI,
                            '               Mov_Desc_Contabile,
                            '               Me.Txt_DataMovimento.Text,
                            '               Me.Txt_Ora.Text,
                            '               Me.Txt_DataMovimento.Text,
                            '               #12/31/2100#,
                            '               ,
                            '               "",
                            '               0,
                            '               "",
                            '               ,
                            '               ,
                            '               ,
                            '               ,
                            '               0,
                            '               0,
                            '               ,
                            '               ,
                            '               ,
                            '               ,
                            '               ,
                            '               ,
                            '               ,
                            '               "",
                            '               0,
                            '               0,
                            '               0,
                            '               0,
                            '               "",
                            '               "",
                            '               0,
                            '               ,
                            '               Me.Txt_DataMovimento.Text,
                            '               #12/31/2100#,
                            '               BaseCode,
                            '               TopCode,
                            '               XmlDoc)

                            'XML_DatiMovimenti.AppendChild(XML_MovimentoContabile)


                            '#######################################################
                            '##################   PAGAMENTI   ######################
                            '#######################################################


                    End Select


                    '#######################################################
                    '###########   MOVIMENTO DI CARICO / SCARICO    ########
                    '#######################################################

                    Movimento = New Movimento

                    Movimento.Id_Agenda = Qs_IdAgenda
                    Movimento.Piva = ViewState("Piva")
                    Movimento.Sa_Cod = Sa_Cod
                    Movimento.Data = Me.Txt_DataMovimento.Text
                    Movimento.Lav_Cod = Lav_Cod
                    Movimento.Cau_Mov = xCaricoScarico.ToString
                    Movimento.Mov_Desc = Mov_Desc
                    Movimento.Ora = Me.Txt_Ora.Text
                    Movimento.Data_Registrazione = Me.Txt_DataMovimento.Text
                    Movimento.Scadenza = AGRODATAFINE

                    Movimento.BaseCode = BaseCode
                    Movimento.TopCode = TopCode

                    Agenda.Movimenti.Add(Movimento)


                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    '#######################################################
                    '################   MOVIMENTI DETTAGLIO    #############
                    '#######################################################

                    Movimento_Dettaglio = New Movimento_Dettaglio

                    Movimento_Dettaglio.Id_Agenda = Qs_IdAgenda
                    Movimento_Dettaglio.Piva = ViewState("Piva")
                    Movimento_Dettaglio.Sa_Cod = Sa_Cod
                    Movimento_Dettaglio.Data = Me.Txt_DataMovimento.Text
                    Movimento_Dettaglio.Lav_Cod = Lav_Cod
                    Movimento_Dettaglio.Cau_Mov = xCaricoScarico.ToString
                    Movimento_Dettaglio.Mov_Det_Des = Mov_Det_Des
                    Movimento_Dettaglio.Elem_Cod = Elem_Cod
                    Movimento_Dettaglio.Pro_Cod = Pro_Cod
                    Movimento_Dettaglio.Mat_Cod = Mat_Cod
                    Movimento_Dettaglio.Cod_Progetto = Cod_Progetto
                    Movimento_Dettaglio.Fase_Cod = Fase_Cod
                    Movimento_Dettaglio.Lotto = Lotto
                    Movimento_Dettaglio.Cal_Cod = Cal_Cod
                    Movimento_Dettaglio.Udm_Cod = Udm_Cod
                    Movimento_Dettaglio.Udm_Cod_Extra = Udm_Cod_Extra
                    Movimento_Dettaglio.Qta = Qta
                    Movimento_Dettaglio.Qta_Extra = Qta_Extra
                    Movimento_Dettaglio.Pendente = Pendente

                    Movimento_Dettaglio.Prezzo_Effettivo = Prezzo_Effettivo
                    Movimento_Dettaglio.Prezzo_Unitario = PrezzoUnitario
                    Movimento_Dettaglio.Prezzo_Unitario_Netto = PrezzoUnitario_Netto

                    Movimento_Dettaglio.Sconto = Sconto_Magg
                    Movimento_Dettaglio.Imponibile = Imponibile
                    Movimento_Dettaglio.Imponibile_Netto = Imponibile_Netto

                    Movimento_Dettaglio.Cod_Iva = Cod_Iva
                    Movimento_Dettaglio.Iva = Iva

                    Movimento_Dettaglio.Anno = Anno
                    Movimento_Dettaglio.Ric_Cod = Ric_Cod
                    Movimento_Dettaglio.Cod_Conto = Cod_Conto
                    Movimento_Dettaglio.Extra_Int = Extra_int
                    Movimento_Dettaglio.Extra_Str = Extra_Str
                    Movimento_Dettaglio.Extra_Date = Extra_Date

                    If Qs_Lav_Cod = LAVCOD_ACQUISTO OrElse Qs_Lav_Cod = LAVCOD_VENDITA Then
                        Movimento_Dettaglio.Contabilizzato = CONTABILE
                    Else
                        Movimento_Dettaglio.Contabilizzato = NONCONTABILE
                    End If

                    Movimento_Dettaglio.Rif_Esterno = Rif_Esterno
                    Movimento_Dettaglio.Rif_Esterno_2 = Rif_Esterno_2

                    Movimento_Dettaglio.BaseCode = BaseCode
                    Movimento_Dettaglio.TopCode = TopCode

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)


                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                    '#######################################################
                    '################   MOVIMENTI DESTINAZIONE    ##########
                    '#######################################################

                    Movimento_Destinazione = New Movimento_Destinazione

                    Movimento_Destinazione.Id_Agenda = Qs_IdAgenda
                    Movimento_Destinazione.Piva = ViewState("Piva")
                    Movimento_Destinazione.Sa_Cod = Sa_Cod
                    Movimento_Destinazione.Appezza = 0
                    Movimento_Destinazione.Id_Destinazione = Magazzino_Cod
                    Movimento_Destinazione.Tipo = MAGAZZINO
                    Movimento_Destinazione.Qta = Qta
                    Movimento_Destinazione.Data = Me.Txt_DataMovimento.Text
                    Movimento_Destinazione.BaseCode = BaseCode
                    Movimento_Destinazione.TopCode = TopCode

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni.Add(Movimento_Destinazione)

                    '#######################################################
                    '################   MOVIMENTO TECNICO    ###############
                    '#######################################################

                    If Cau_Mov = CAU_CARICO AndAlso Elem_Cod = FERTILIZZANTI Then

                        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                        Movimento_Dettaglio_Tecnico.Id_Agenda = Qs_IdAgenda
                        Movimento_Dettaglio_Tecnico.Piva = ViewState("Piva")
                        Movimento_Dettaglio_Tecnico.Sa_Cod = Sa_Cod
                        Movimento_Dettaglio_Tecnico.N = N
                        Movimento_Dettaglio_Tecnico.P = P2O5
                        Movimento_Dettaglio_Tecnico.K = K2O
                        Movimento_Dettaglio_Tecnico.Cu = Cu
                        Movimento_Dettaglio_Tecnico.Extra_Int = Regolamento_Cod_Ferti
                        Movimento_Dettaglio_Tecnico.Data = Me.Txt_DataMovimento.Text
                        Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
                        Movimento_Dettaglio_Tecnico.TopCode = TopCode

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                    End If

                End If

        End Select

        Return Agenda


    End Function

    '########################################################################################
    ' Corrisponde alla routine XML_2_Agenda_MovimentoDettaglio
    ' del modulo XML_Manager_2
    ' Non viene usata quella, perché nella FormProdotto 
    ' bisogna gestire vari casi nella creazione dell'XML
    Private Function XML_Genera_MovimentoDettaglio(ByVal Operazione As enum_TipoOperazioneDB,
                                                    ByVal Piva As String,
                                                    Optional ByVal Sa_Cod As Integer = 0,
                                                    Optional ByVal ID_Agenda As Integer = 0,
                                                    Optional ByVal ID_Mov As Integer = 0,
                                                    Optional ByVal ID_Mov_Det As Integer = 0,
                                                    Optional ByVal Lav_Cod As Integer = 0,
                                                    Optional ByVal Elem_Cod As Integer = 0,
                                                    Optional ByVal Pro_Cod As Integer = 0,
                                                    Optional ByVal Mat_Cod As Integer = 0,
                                                    Optional ByVal Cod_Progetto As Integer = 0,
                                                    Optional ByVal Cal_Cod As Integer = 0,
                                                    Optional ByVal Fase_Cod As Integer = 0,
                                                    Optional ByVal Lotto As String = "",
                                                    Optional ByVal Udm_Cod As Integer = 0,
                                                    Optional ByVal Qta As Decimal = 0,
                                                    Optional ByVal Prezzo_Unitario As Decimal = 0,
                                                    Optional ByVal Prezzo_Unitario_Netto As Decimal = 0,
                                                    Optional ByVal Contabilizzato As Integer = 1,
                                                    Optional ByVal Pendente As Integer = 1,
                                                    Optional ByVal Extra_Int As Integer = 0,
                                                    Optional ByVal Cau_Mov As String = "",
                                                    Optional ByVal Descrizione As String = "",
                                                    Optional ByVal Imponibile As Decimal = 0,
                                                    Optional ByVal Imponibile_Netto As Decimal = 0,
                                                    Optional ByVal Cod_IVA As Integer = 0,
                                                    Optional ByVal Sconto_Magg As Decimal = 0,
                                                    Optional ByVal Ric_Cod As Integer = 0,
                                                    Optional ByVal Cod_Conto As Integer = 0,
                                                    Optional ByVal Anno As Integer = 1900,
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                    Optional ByVal BaseCode As Integer = 0,
                                                    Optional ByVal TopCode As Integer = 200000000,
                                                    Optional ByVal ID_Destinazione As Integer = 0,
                                                    Optional ByVal Lav_Cod_Allegato As Integer = 0,
                                                    Optional ByVal Contatore_Trasf As Integer = -1,
                                                    Optional ByVal Udm_Cod_Extra As Decimal = 0,
                                                    Optional ByVal Qta_Extra As Decimal = 0,
                                                    Optional ByVal Prezzo_Effettivo As Decimal = 0,
                                                    Optional ByVal Iva As Decimal = 0,
                                                   Optional ByVal N As Decimal = 0,
                                                   Optional ByVal P2O5 As Decimal = 0,
                                                   Optional ByVal K2O As Decimal = 0,
                                                   Optional ByVal Cu As Decimal = 0,
                                                   Optional ByVal Regolamento_Cod_Ferti As Integer = 0,
                                                   Optional ByVal Rif_Esterno As String = "",
                                                   Optional ByVal Rif_Esterno_2 As String = ""
                                                   ) As String

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim XmlDoc As New XmlDocument
        Dim XmlMovimento_Dettaglio As XmlElement
        Dim XmlMov_Destinazione As XmlElement
        Dim XmlMov_Tecnico As XmlElement
        'Dim XmlMov_Det_Riferimento As XmlElement
        Dim Segno As String

        Select Case Cau_Mov
            Case CAU_CARICO
                Segno = "-" 'acquisto - fattura ricevuta
            Case CAU_CONFERIMENTO
                Segno = "-" 'costo: ricevi da un socio
            Case CAU_CONFERIMENTO_DIVERSI
                Segno = "+" 'ricavo: vendi a esterni
            Case CAU_ACCETTAZIONE_BENI
                Segno = "-"             '?
            Case CAU_SCARICO
                Segno = "+" ' vendita - fattura emessa
            Case CAU_TRASFERIMENTO
                Segno = "+" 'Fittizio    ?
            Case Else
                Segno = "+" 'default
        End Select

        Dim str_XML_ZooAnimale As String = ""

        XmlMovimento_Dettaglio = XmlDoc.CreateElement("Movimento_Dettaglio")

        XmlMovimento_Dettaglio.SetAttribute("TipoOperazioneDB", Operazione)

        XmlMovimento_Dettaglio.SetAttribute("piva", CStr(Piva))
        XmlMovimento_Dettaglio.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlMovimento_Dettaglio.SetAttribute("id_agenda", CStr(ID_Agenda))
        XmlMovimento_Dettaglio.SetAttribute("id_mov", CStr(ID_Mov))
        XmlMovimento_Dettaglio.SetAttribute("id_mov_det", CStr(ID_Mov_Det))


        '=========================

        Pendente = GetPendente(Elem_Cod, Descrizione, Pro_Cod, Mat_Cod, Fase_Cod, Cal_Cod, Udm_Cod, ID_Destinazione)
        '-------------------------------------------------------------------

        XmlMovimento_Dettaglio.SetAttribute("mov_det_des", Descrizione)

        XmlMovimento_Dettaglio.SetAttribute("elem_cod", CStr(Elem_Cod))
        XmlMovimento_Dettaglio.SetAttribute("pro_cod", CStr(Pro_Cod))
        XmlMovimento_Dettaglio.SetAttribute("mat_cod", CStr(Mat_Cod))
        XmlMovimento_Dettaglio.SetAttribute("cod_progetto", CStr(Cod_Progetto))
        XmlMovimento_Dettaglio.SetAttribute("fase_cod", CStr(Fase_Cod))
        XmlMovimento_Dettaglio.SetAttribute("lotto", Lotto)
        XmlMovimento_Dettaglio.SetAttribute("cal_cod", CStr(Cal_Cod))
        XmlMovimento_Dettaglio.SetAttribute("udm_cod", CStr(Udm_Cod))

        XmlMovimento_Dettaglio.SetAttribute("qta", CStr(Qta))

        XmlMovimento_Dettaglio.SetAttribute("udm_cod_extra", CStr(Udm_Cod_Extra))
        XmlMovimento_Dettaglio.SetAttribute("qta_extra", CStr(Qta_Extra))

        XmlMovimento_Dettaglio.SetAttribute("prezzo_unitario", CStr(Prezzo_Unitario))
        XmlMovimento_Dettaglio.SetAttribute("prezzo_unitario_netto", CStr(Prezzo_Unitario_Netto))

        'XmlMovimento_Dettaglio.SetAttribute("imponibile", CStr(Imponibile))
        'XmlMovimento_Dettaglio.SetAttribute("imponibile_netto", CStr(Imponibile_Netto))
        XmlMovimento_Dettaglio.SetAttribute("imponibile", CStr(CDbl(Segno & 1) * CDbl(Imponibile)))
        XmlMovimento_Dettaglio.SetAttribute("imponibile_netto", CStr(CDbl(Segno & 1) * CDbl(Imponibile_Netto)))

        XmlMovimento_Dettaglio.SetAttribute("cod_iva", CStr(Cod_IVA))
        'Calcolo Iva Credito/Debito
        'XmlMovimento_Dettaglio.SetAttribute("iva", Format(CDbl(mSegno & 1) * (CDbl(me.txt_imponibile_netto.text) - CDbl(me.txt_importo.text)), "##,###,###.00"))
        XmlMovimento_Dettaglio.SetAttribute("iva", CStr(objContabHLP.Leggi_IVA_PositivaNegativa(Lav_Cod, Iva)))

        XmlMovimento_Dettaglio.SetAttribute("sconto", CStr(Sconto_Magg))
        XmlMovimento_Dettaglio.SetAttribute("prezzo_effettivo", CStr(Prezzo_Effettivo))

        '=============================================================================================
        'Attributi per l'aggiornamento Piano dei Conti
        '---------------------------------------------------------------------------------------------
        XmlMovimento_Dettaglio.SetAttribute("anno", CStr(Anno))
        XmlMovimento_Dettaglio.SetAttribute("ric_cod", CStr(Ric_Cod))

        Select Case Lav_Cod

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO,
            LAVCOD_CARICO, LAVCOD_SCARICO,
            LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_TRASFERIMENTO,
            LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO
                XmlMovimento_Dettaglio.SetAttribute("cod_conto", CStr(0)) 'Conto Non Imputato

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA,
                 LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA

                XmlMovimento_Dettaglio.SetAttribute("cod_conto", CStr(Cod_Conto))

            Case Else
                XmlMovimento_Dettaglio.SetAttribute("cod_conto", CStr(0)) 'Conto Non Imputato

        End Select
        '---------------------------------------------------------------------------------------------

        'Impostazione del campo Jolly:Int = Movimentazione di Magazzino
        If Not Me.Chk_MovimentoMag.Checked Then
            'CASO DI DOCUMENTO ASSOCIATO A UN ALTRO (ES. BOLLE - FATTURE)
            'IL COMPONENTE NON DEVE GESTIRE LE GIACENZE
            XmlMovimento_Dettaglio.SetAttribute("jolly_int", CStr(enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato))
        Else
            'CASO BUONO DI CARICO E SCARICO
            'IL COMPONENTE GESTISCE LE GIACENZE
            XmlMovimento_Dettaglio.SetAttribute("jolly_int", CStr(enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato))
        End If

        Select Case Lav_Cod

            'CONTABILIZZATO POSITIVO -> IL COM GESTISCE LE GIACENZE
            'CONTABILIZZATO NEGATIVO -> E' UN'OPERAZIONE PIANIFICATA E NON VENGONO GESTITE LE GIACENZE
            '                           (ci pensa il COM, in base alla data, a impostare il valore negativo)

            Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_TRASFERIMENTO,
            LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO

                XmlMovimento_Dettaglio.SetAttribute("contabilizzato", CStr(NONCONTABILE))

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_DDT_CONTABILIZZATO_EMESSO,
            LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA,
            LAVCOD_VENDITA, LAVCOD_ACQUISTO,
            LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA

                XmlMovimento_Dettaglio.SetAttribute("contabilizzato", CStr(CONTABILE))

        End Select

        XmlMovimento_Dettaglio.SetAttribute("pendente", CStr(Pendente))



        If cmb_Causale.SelectedValue = CStr(enum_Pendenza.Furto) Then
            'se sono in causale furto allora abilito le textbox per scrivere la denuncia
            XmlMovimento_Dettaglio.SetAttribute("extra_str", Txt_NumeroDenuncia.Text)
        Else
            XmlMovimento_Dettaglio.SetAttribute("extra_str", "")
        End If


        XmlMovimento_Dettaglio.SetAttribute("extra_int", CStr(Extra_Int))

        If cmb_Causale.SelectedValue = CStr(enum_Pendenza.Furto) Then
            'se sono in causale furto allora abilito le textbox per scrivere la denuncia
            XmlMovimento_Dettaglio.SetAttribute("extra_date", CDate(Txt_DataDenuncia.Text))
        Else
            XmlMovimento_Dettaglio.SetAttribute("extra_date", "01/01/1900")
        End If

        XmlMovimento_Dettaglio.SetAttribute("rif_esterno", Rif_Esterno)
        XmlMovimento_Dettaglio.SetAttribute("rif_esterno_2", Rif_Esterno_2)

        XmlMovimento_Dettaglio.SetAttribute("validita_inizio", Me.Txt_DataMovimento.Text)
        XmlMovimento_Dettaglio.SetAttribute("validita_fine", "31/12/2100")
        XmlMovimento_Dettaglio.SetAttribute("basecode", CStr(BaseCode))
        XmlMovimento_Dettaglio.SetAttribute("topcode", CStr(TopCode))

        '=============================================================================================



        '==============================================================================================================
        'Inserimento nella stringa Xml dei campi non presenti in Db ma utili (necessari) per la gestione delle giacenze
        '--------------------------------------------------------------------------------------------------------------
        XmlMovimento_Dettaglio.SetAttribute("id_destinazione", CStr(ID_Destinazione))

        XmlMovimento_Dettaglio.SetAttribute("lav_cod", Lav_Cod_Allegato) 'Nota: Utile in FormFattura


        Select Case Cau_Mov

            Case CAU_TRASFERIMENTO

                If Contatore_Trasf = 0 Then

                    XmlMovimento_Dettaglio.SetAttribute("cau_mov", CAU_SCARICO)

                ElseIf Contatore_Trasf = 1 Then

                    XmlMovimento_Dettaglio.SetAttribute("cau_mov", CAU_CARICO)

                End If

            Case Else

                XmlMovimento_Dettaglio.SetAttribute("cau_mov", Cau_Mov)

        End Select
        '--------------------------------------------------------------------------------------------------------------




        '==============================================================================================================
        '==============================================================================================================
        '======================== MOV DESTINAZIONE =================================================
        '==============================================================================================================
        '==============================================================================================================

        Select Case Elem_Cod

            Case ALTRI_BENI, SERVIZI

                'Altri Beni Strumentali non Movimentabili

                '-------------------------------------------------------------------

            Case MACCHINE

                '#################################################################
                '############### MACCHINA / ATTREZZATURA  ########################
                '#################################################################

                'If mParcoMacchina Is Nothing Then
                '    'Non è stata inizializzata la formmacchine (non è stata ancora richiesta la visualizzazione della scheda tecnica)
                '    FormMacchine.FormInizializza(mPiva(Indice), mRag_Soc, 0, mClass_Code)
                '    mParcoMacchina = FormMacchine.FormPreparaDatiXml(mStato)
                'End If

                'XmlMovimento_Dettaglio.AppendChild(mParcoMacchina)

                '-------------------------------------------------------------------

            Case ZOO_CONSISTENZA

                XmlMov_Destinazione = XmlDoc.CreateElement("Movimento_Destinazione")

                XmlMov_Destinazione.SetAttribute("TipoOperazioneDB", Operazione)
                XmlMov_Destinazione.SetAttribute("piva", Piva)
                XmlMov_Destinazione.SetAttribute("sa_cod", Sa_Cod)
                XmlMov_Destinazione.SetAttribute("id_agenda", 0) 'mId_Agenda
                XmlMov_Destinazione.SetAttribute("id_mov", 0)
                XmlMov_Destinazione.SetAttribute("id_mov_det", 0)
                XmlMov_Destinazione.SetAttribute("appezza", 0) 'Carico/Scarico in Magazzino
                XmlMov_Destinazione.SetAttribute("id_destinazione", ID_Destinazione)
                XmlMov_Destinazione.SetAttribute("tipo_destinazione", STALLA)
                XmlMov_Destinazione.SetAttribute("qta", Qta)
                XmlMov_Destinazione.SetAttribute("qta2", 0)
                XmlMov_Destinazione.SetAttribute(LCase("Tipo_Scorta"), CStr(0))
                XmlMov_Destinazione.SetAttribute(LCase("Scorta_Min"), CStr(0))
                XmlMov_Destinazione.SetAttribute("validita_inizio", Me.Txt_DataMovimento.Text)
                XmlMov_Destinazione.SetAttribute("validita_fine", "31/12/2100")
                XmlMov_Destinazione.SetAttribute("basecode", BaseCode)
                XmlMov_Destinazione.SetAttribute("topcode", TopCode)

                XmlMovimento_Dettaglio.AppendChild(XmlMov_Destinazione)

                'If Elem_Cod = ZOO_CONSISTENZA Then

                '    'If StatoZoo <> dbleggi Then
                '    '    '===========================================================
                '    '    'Inizializzazione Form Anagrafe Animale
                '    '    '-----------------------------------------------------------
                '    '    FormZoo_Animale.FormInizializza(mPiva(BASE), mRag_Soc, mSa_Cod(BASE), mSa_Nome, mCod_Progetto, mDatiZoo, mGen_Cod, mSpe_Cod, mIPro_Cod, mRaz_Cod)
                '    '    mDatiZoo = FormZoo_Animale.FormPreparaDatiXml(StatoZoo)
                '    '    '===========================================================

                '    '    XmlMovimento_Dettaglio.AppendChild(mDatiZoo)

                '    'End If


                'End If

                '-------------------------------------------------------------------



            Case Else

                'Il movimento NON riguarda il parco macchine/attrezzature


                '#######################################################
                '############  MOVIMENTO RIFERIMENTI  ##################
                '#######################################################


                'If Not Set_Allegato Then

                '    'Inserisco i magazzini di destinazione

                '    '#######################################################
                '    '###########  MOVIMENTO DESTINAZIONE  ##################
                '    '#######################################################

                XmlMov_Destinazione = XmlDoc.CreateElement("Movimento_Destinazione")

                XmlMov_Destinazione.SetAttribute("TipoOperazioneDB", Operazione)
                XmlMov_Destinazione.SetAttribute("piva", Piva)
                XmlMov_Destinazione.SetAttribute("sa_cod", Sa_Cod)
                XmlMov_Destinazione.SetAttribute("id_agenda", 0) 'mId_Agenda
                XmlMov_Destinazione.SetAttribute("id_mov", 0)
                XmlMov_Destinazione.SetAttribute("id_mov_det", 0)
                XmlMov_Destinazione.SetAttribute("appezza", 0) 'Carico/Scarico in Magazzino
                XmlMov_Destinazione.SetAttribute("id_destinazione", ID_Destinazione)
                XmlMov_Destinazione.SetAttribute("tipo_destinazione", MAGAZZINO)
                XmlMov_Destinazione.SetAttribute("qta", Qta)
                XmlMov_Destinazione.SetAttribute("qta2", 0)
                XmlMov_Destinazione.SetAttribute(LCase("Tipo_Scorta"), CStr(0))
                XmlMov_Destinazione.SetAttribute(LCase("Scorta_Min"), CStr(0))
                XmlMov_Destinazione.SetAttribute("validita_inizio", Me.Txt_DataMovimento.Text)
                XmlMov_Destinazione.SetAttribute("validita_fine", "31/12/2100")
                XmlMov_Destinazione.SetAttribute("basecode", BaseCode)
                XmlMov_Destinazione.SetAttribute("topcode", TopCode)

                XmlMovimento_Dettaglio.AppendChild(XmlMov_Destinazione)


                'End If

                '-------------------------------------------------------------------


        End Select

        '==============================================================================================================
        '==============================================================================================================
        '======================== MOV DETTAGLIO TECNICO =================================================
        '==============================================================================================================
        '==============================================================================================================

        If Cau_Mov = CAU_CARICO AndAlso Elem_Cod = FERTILIZZANTI Then

            XmlMov_Tecnico = XmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_2")

            With XmlMov_Tecnico
                .SetAttribute("TipoOperazioneDB", Operazione)
                .SetAttribute("piva", Piva)
                .SetAttribute("sa_cod", Sa_Cod)
                .SetAttribute("id_agenda", 0)
                .SetAttribute("id_mov", 0)
                .SetAttribute("id_mov_det", 0)

                .SetAttribute("n", CStr(N))
                .SetAttribute("k", CStr(K2O))
                .SetAttribute("p", CStr(P2O5))
                .SetAttribute("cu", CStr(Cu))
                .SetAttribute("extra_int", CStr(Regolamento_Cod_Ferti))

                .SetAttribute("validita_inizio", Me.Txt_DataMovimento.Text)
                .SetAttribute("validita_fine", "31/12/2100")
                .SetAttribute("basecode", BaseCode)
                .SetAttribute("topcode", TopCode)
            End With

            XmlMovimento_Dettaglio.AppendChild(XmlMov_Tecnico)
        End If

        'If Set_Allegato Then

        '    'Se il documento è allegato i dati sulle destinazioni sono riferite all'allegato
        '    'Inserimento del riferimento

        '    XmlMov_Det_Riferimento = XmlDoc.CreateElement("Movimento_Riferimento2")
        '    XmlMov_Det_Riferimento.SetAttribute("TipoOperazioneDB", Operazione)
        '    XmlMov_Det_Riferimento.SetAttribute("piva", mPiva(Indice))
        '    XmlMov_Det_Riferimento.SetAttribute("sa_cod", 0)
        '    XmlMov_Det_Riferimento.SetAttribute("lav_cod", mLav_Cod)
        '    XmlMov_Det_Riferimento.SetAttribute("cau_mov", mCau_Mov)
        '    XmlMov_Det_Riferimento.SetAttribute("id_agenda", 0) 'Da Definire
        '    XmlMov_Det_Riferimento.SetAttribute("id_mov", 0) 'Da Definire
        '    XmlMov_Det_Riferimento.SetAttribute("id_mov_det", 0) 'Da Definire
        '    XmlMov_Det_Riferimento.SetAttribute("piva_rif", mPiva(Indice))
        '    XmlMov_Det_Riferimento.SetAttribute("sa_cod_rif", 0)
        '    XmlMov_Det_Riferimento.SetAttribute("lav_cod_rif", mLav_Cod_Allegato)
        '    XmlMov_Det_Riferimento.SetAttribute("cau_mov_rif", CAU_REGISTRAZIONI)
        '    XmlMov_Det_Riferimento.SetAttribute("id_agenda_rif", mId_Agenda_Allegato)
        '    XmlMov_Det_Riferimento.SetAttribute("id_mov_rif", mId_Mov_Allegato)
        '    XmlMov_Det_Riferimento.SetAttribute("id_mov_det_rif", mId_Mov_Det_Allegato)
        '    XmlMov_Det_Riferimento.SetAttribute("qta", me.txt_qta.text)
        '    XmlMov_Det_Riferimento.SetAttribute("validita_inizio", TxtData.Text)
        '    XmlMov_Det_Riferimento.SetAttribute("validita_fine", Format(AgroDataFine, "dd/mm/yyyy"))

        '    XmlMovimento_Dettaglio.AppendChild(XmlMov_Det_Riferimento)

        'End If


        Return XmlMovimento_Dettaglio.OuterXml


    End Function

    Private Function GetPendente(ByVal Elem_Cod As Integer, ByRef Descrizione As String, ByRef Pro_Cod As Integer, ByRef Mat_Cod As Integer, ByRef Fase_Cod As Integer, ByRef Cal_Cod As Integer, ByRef Udm_Cod As Integer, ByRef ID_Destinazione As Integer) As Integer

        Dim Pendente As Integer

        Select Case Elem_Cod

            Case SERVIZI

                If (Me.Txt_PendenzaIniziale.Text = enum_Pendenza.DocBolla) OrElse (Me.Txt_PendenzaIniziale.Text = enum_Pendenza.DocFattura) Then
                    Pendente = Me.Txt_PendenzaIniziale.Text
                Else
                    Pendente = enum_Pendenza.MovESENTE
                End If

                Fase_Cod = 0
                ID_Destinazione = 0

                '-------------------------------------------------------------------

            Case MACCHINE, ALTRI_BENI

                If (Me.Txt_PendenzaIniziale.Text = enum_Pendenza.DocBolla) OrElse (Me.Txt_PendenzaIniziale.Text = enum_Pendenza.DocFattura) Then
                    Pendente = Me.Txt_PendenzaIniziale.Text
                Else
                    Pendente = enum_Pendenza.MovESENTE
                End If

                Fase_Cod = 0
                ID_Destinazione = 0

                '-------------------------------------------------------------------

            Case ZOO_CONSISTENZA

                If IsNothing(Me.cmb_Causale.SelectedItem) Then
                    Pendente = Me.Txt_PendenzaIniziale.Text
                Else
                    Pendente = CStr(Me.cmb_Causale.SelectedValue)
                End If

                Fase_Cod = 0

                'siam sicuri che va bene?
                Descrizione = Me.Cmb_IndirizzoProduttivo.SelectedItem.Text

                Pro_Cod = 0
                Mat_Cod = 0
                Fase_Cod = 0
                Cal_Cod = 0
                Udm_Cod = enum_UnitaMisura.Numero

                '-------------------------------------------------------------------

            Case Else

                If IsNothing(Me.cmb_Causale.SelectedItem) Then
                    Pendente = Me.Txt_PendenzaIniziale.Text
                Else
                    Pendente = CStr(Me.cmb_Causale.SelectedValue)
                End If

                'If Me.Cmb_Ordini.SelectedIndex > 0 Then
                '    Fase_Cod = CInt(Me.Cmb_Ordini.SelectedValue)
                'Else
                '    Fase_Cod = 0
                'End If

        End Select

        Return Pendente

    End Function


    '###############################################################################
    Private Sub Recupera_ChiaveMagazzino()

        If xCaricoScarico = enum_Agenda_Causali.SCARICO OrElse
        xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then

            'scarico
            xSa_Cod = Me.Cmb_Provenienza.SelectedValue.Split("|")(1)
            xFabbricato_Cod = Me.Cmb_Provenienza.SelectedValue.Split("|")(0)

        Else
            'carico
            xSa_Cod = Me.Cmb_Destinazione.SelectedValue.Split("|")(1)
            xFabbricato_Cod = Me.Cmb_Destinazione.SelectedValue.Split("|")(0)

        End If


    End Sub

    '###############################################################################
    Private Sub Recupera_ChiaveProdotto()

        If Me.cmb_Categoria.SelectedIndex > 0 Then
            xElem_Cod = Me.cmb_Categoria.SelectedItem.Value
        Else
            xElem_Cod = 0
        End If

        If Me.cmb_Prodotti.SelectedIndex > 0 Then
            xMat_Cod = -Me.cmb_Prodotti.SelectedItem.Value
        Else
            xMat_Cod = 0
        End If

        If Chk_LottoImpianto.Checked Then
            xCod_Progetto = CODPROGETTO_NONDEFINITO
        Else
            If Me.cmb_Lotto.SelectedIndex > 0 Then
                xCod_Progetto = Me.cmb_Lotto.SelectedItem.Value
            Else
                xCod_Progetto = 0
            End If
        End If


        If Me.Cmb_Ordini.SelectedIndex > 0 Then
            xFase_Cod = Me.Cmb_Ordini.SelectedItem.Value
        Else
            xFase_Cod = 0
        End If

        'If Me.cmb_LottoAccettazione.SelectedIndex > 0 Then
        '    'lotto non definito
        '    If Me.cmb_LottoAccettazione.SelectedItem.Value = "-1" Or _
        '    IsNothing(Me.cmb_LottoAccettazione.SelectedItem.Value) Then
        '        xLotto = ""
        '    Else
        '        xLotto = Me.cmb_LottoAccettazione.SelectedItem.Value
        '    End If
        'Else
        '    xLotto = ""
        'End If

        'LOTTO DI ACCETTAZIONE
        If Me.cmb_LottoAccettazione.SelectedIndex < 1 Then
            If xCaricoScarico = CAU_CARICO Then
                xLotto = Me.Txt_LottoAccettazione.Text
            Else
                xLotto = ""
            End If
        Else
            If Me.cmb_LottoAccettazione.SelectedItem.Value = "-1" OrElse
           IsNothing(Me.cmb_LottoAccettazione.SelectedItem.Value) Then
                xLotto = ""
            Else
                xLotto = Me.cmb_LottoAccettazione.SelectedItem.Value
            End If
        End If


        If Chk_ParametroQualitativo.Checked Then
            xCod_Calibro = 0
        Else

            If cmb_Calibro.SelectedIndex > 0 Then
                xCod_Calibro = cmb_Calibro.SelectedItem.Value
            Else
                xCod_Calibro = 0
            End If
        End If

    End Sub

    '###############################################################################
    Private Sub Recupera_DoseEtichetta(ByVal Pro_Cod As Integer)

        'non valorizzata
        'If Session("Collegamento_Fito") = True Then

        Dim strErr As String = ""
        Dim Udm_Cod As Integer = 0
        Dim Udm_Des As String = ""
        Dim Udm_Sim As String = ""

        Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi
        objDPILeggi.Recupera_UdM_da_FrCod(strErr,
                                            Pro_Cod,
                                            Udm_Cod,
                                            Udm_Sim,
                                            Udm_Des,
                                            Session)

        If strErr = "" Then
            If Udm_Des <> "" Then
                cmb_Udm.SelectedIndex =
                    cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                cmb_Udm_SelectedIndexChanged(Me, Nothing)
            End If
        End If

        'End If


    End Sub

    '###############################################################################
    Private Sub Recupera_UdmFertilizzante(ByVal Fer_Cod As Integer, ByVal PUA_RegolamentoCod As Integer)

        Dim Udm_Cod As Integer = 0

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input With {
            .Codice = Fer_Cod,
            .Descrizione = "",
            .DataInizio = AGRODATAINIZIO,
            .DataFine = AGRODATAFINE,
            .Tipo = PUA_RegolamentoCod,
            .IncludiApporti = True,
            .IncludiTipologia = False,
            .Regolamento = PUA_RegolamentoCod,
            .strFiltro = ""
        }

        '(27/09/2018 fede)
        Dim objAgroWebConfig As AgroWebConfig = Nothing
        If HttpContext.Current.Session IsNot Nothing Then
            objAgroWebConfig = New AgroWebConfig
            'Else
            '    objAgroWebConfig = New AgroWebConfig(objParametri_Super_Server, objParametri_server, True)
        End If

        objParametriIngresso.Url = objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti & "/Fertilizzanti"

        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output
        Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
        objParametriUscita = objFert_WS.Fertilizzanti(objParametriIngresso)

        If objParametriUscita.ListaFertilizzanti IsNot Nothing AndAlso objParametriUscita.ListaFertilizzanti.Count = 1 Then
            Udm_Cod = objParametriUscita.ListaFertilizzanti(0).Udm_Cod
        End If

        If objParametriUscita.MessaggioErrore = "" AndAlso Udm_Cod <> 0 Then
            cmb_Udm.SelectedIndex = cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

            cmb_Udm_SelectedIndexChanged(Me, Nothing)

            ' Giulia: 17/6/2019: solo il regolamento 78 (Umbria 2019) per il momento, va implementato per tutti!!!!
            'TODO: rimuovere!!!!
            'If PUA_RegolamentoCod = 78 Then
            Select Case PUA_RegolamentoCod
                Case 78, 125
                    cmb_Udm.Enabled = False
            End Select
            'End If
        End If

    End Sub

    '###############################################################################
    Private Sub Visualizza_DoseEtichetta(ByVal Dose As String)

        Me.Txt_DoseEtichetta.Text = Dose
        Me.Txt_DoseEtichetta.Visible = True
        Me.Lbl_DoseEtichetta.Visible = True

    End Sub

    '###############################################################################
    Private Sub Nascondi_DoseEtichetta()

        Me.Txt_DoseEtichetta.Text = ""
        Me.Txt_DoseEtichetta.Visible = False
        Me.Lbl_DoseEtichetta.Visible = False

    End Sub

    '###############################################################################
    Private Sub Recupera_DettagliEconomici(ByRef Udm_Cod_Extra As Decimal,
                                            ByRef Qta_Extra As Decimal,
                                            ByRef Prezzo_Effettivo As Decimal,
                                            ByRef Sconto_Magg As Decimal,
                                            ByRef Variazione_Perc As String,
                                            ByRef Variazione As Decimal,
                                            ByRef Imponibile As Decimal,
                                            ByRef Imponibile_Netto As Decimal,
                                            ByRef Cod_Iva As Integer,
                                            ByRef Iva As Decimal,
                                            ByRef Anno As Integer,
                                            ByRef Ric_Cod As Integer,
                                            ByRef Cod_Conto As Integer,
                                            ByRef Conto As String)

        'ByRef Sconto_Netto As Decimal, _
        '                           ByRef Maggiorazione As Decimal, _
        '                           ByRef Maggiorazione_Netta As Decimal, _


        Dim Sconto As Decimal = 0
        Dim Maggiorazione As Decimal = 0

        Udm_Cod_Extra = 0

        If Me.Txt_Qta_Extra.Text = "" Then
            Qta_Extra = 0
        Else
            Qta_Extra = CDbl(Me.Txt_Qta_Extra.Text)
        End If

        Prezzo_Effettivo = 0

        If Me.Txt_ScontoPercentuale.Text = "" Then
            Sconto = 0
        Else
            Sconto = CDbl(Me.Txt_ScontoPercentuale.Text)
            'Sconto_Netto = CDbl(Me.Txt_Sconto_Netto.Text)
        End If

        If Me.Txt_MaggiorazionePercentuale.Text = "" Then
            Maggiorazione = 0
        Else
            Maggiorazione = CDbl(Me.Txt_MaggiorazionePercentuale.Text)
            'Maggiorazione_Netta = CDbl(Me.Txt_Maggiorazione_Netta.Text)
        End If

        If Sconto <> 0 Then
            Sconto_Magg = -Sconto
        ElseIf Maggiorazione <> 0 Then
            Sconto_Magg = Maggiorazione
        Else
            Sconto_Magg = 0
        End If

        Variazione_Perc = CStr(Sconto_Magg) & "%"

        If (Me.Txt_Sconto_Calcolato.Text = "" OrElse Me.Txt_Sconto_Calcolato.Text = "0") AndAlso
            (Me.Txt_Maggiorazione_Calcolato.Text = "" OrElse Me.Txt_Maggiorazione_Calcolato.Text = "0") Then
            Variazione = 0
        Else
            If (Me.Txt_Sconto_Calcolato.Text <> "" AndAlso CDbl(Me.Txt_Sconto_Calcolato.Text) > 0) Then
                Variazione = -1 * CDbl(Me.Txt_Sconto_Calcolato.Text)
            Else
                If (Me.Txt_Maggiorazione_Calcolato.Text <> "" AndAlso CDbl(Me.Txt_Maggiorazione_Calcolato.Text) > 0) Then
                    Variazione = CDbl(Me.Txt_Maggiorazione_Calcolato.Text)
                Else
                    Variazione = 0
                End If
            End If
        End If

        If Me.Txt_Imponibile.Text = "" Then
            Imponibile = 0
        Else
            Imponibile = CDbl(Me.Txt_Imponibile.Text)
        End If

        If Me.Txt_Imponibile_Netto.Text = "" Then
            Imponibile_Netto = 0
        Else
            Imponibile_Netto = CDbl(Me.Txt_Imponibile_Netto.Text)
        End If


        If Me.Cmb_IVA.SelectedIndex > 0 Then
            Cod_Iva = Split(Cmb_IVA.SelectedValue, "|")(0) 'CInt(Me.Cmb_IVA.SelectedValue)
        Else
            Cod_Iva = 0
        End If

        If Not Me.Chk_IVAmanuale.Checked Then
            If Me.Txt_IVA.Text = "" Then
                Iva = 0
            Else
                Iva = CDbl(Me.Txt_IVA.Text)
            End If
        Else
            Iva = CDbl(Me.Txt_IVAmanuale.Text)
        End If

        'importo = CDbl(Me.Txt_Importo.Text)

        If Not IsNothing(Me.Cmb_AnnoContabile.SelectedValue) Then
            If Me.Cmb_AnnoContabile.SelectedValue <> "" Then
                Anno = CInt(Me.Cmb_AnnoContabile.SelectedValue)
            Else
                Anno = 0
            End If
        Else
            Anno = 0
        End If

        Ric_Cod = BILANCIO_PERSONALIZZATO

        If Me.Cmb_Conti.SelectedIndex > 0 Then
            Cod_Conto = CInt(Me.Cmb_Conti.SelectedValue)
            Conto = Me.Cmb_Conti.SelectedItem.Text
        Else
            Cod_Conto = 0
            Conto = ""
        End If



    End Sub



    '###############################################################################
    Private Sub Recupera_DettagliProdotto(ByRef Str_Errore As String,
                                            ByRef Cod_Progetto As String,
                                            ByRef Progetto_Des As String,
                                            ByRef Fase_Cod As Integer,
                                            ByRef LottoAccettazione As String,
                                            ByRef Cal_Cod As Integer,
                                            ByRef Cal_Des As String,
                                            ByRef UdmCod As Integer,
                                            ByRef UdmDes As String,
                                            ByRef Qta As Decimal,
                                            ByRef Prezzo As Decimal,
                                            ByRef PrezzoNetto As Decimal)

        '-----
        'LOTTO INTERNO
        If Me.cmb_Lotto.SelectedIndex < 1 Then
            Cod_Progetto = 0
        Else
            Cod_Progetto = cmb_Lotto.SelectedItem.Value
            Progetto_Des = cmb_Lotto.SelectedItem.Text
        End If

        '-----
        'ORDINE
        If Me.Cmb_Ordini.SelectedIndex > 0 Then
            Fase_Cod = Me.Cmb_Ordini.SelectedItem.Value
        Else
            Fase_Cod = 0
        End If

        '-----
        'ORDINE
        xFase_Cod = 0 '?

        '-----
        'LOTTO DI ACCETTAZIONE
        If Me.cmb_LottoAccettazione.SelectedIndex < 1 Then
            If xCaricoScarico = CAU_CARICO OrElse xCaricoScarico = CAU_SCARICO OrElse xCaricoScarico = CAU_TRASFERIMENTO Then
                LottoAccettazione = Me.Txt_LottoAccettazione.Text
            Else
                LottoAccettazione = ""
            End If
        Else
            If Me.cmb_LottoAccettazione.SelectedItem.Value = "-1" OrElse
           IsNothing(Me.cmb_LottoAccettazione.SelectedItem.Value) Then
                LottoAccettazione = ""
            Else
                LottoAccettazione = Me.cmb_LottoAccettazione.SelectedItem.Value
            End If
        End If

        '-----
        'CALIBRO
        If Me.cmb_Calibro.SelectedIndex < 1 Then
            Cal_Cod = 0
        Else
            Cal_Cod = cmb_Calibro.SelectedItem.Value
            Cal_Des = cmb_Calibro.SelectedItem.Text
        End If

        '-----
        'UNITA' DI MISURA
        If Me.cmb_Udm.SelectedIndex < 1 Then
            Str_Errore &= DirectCast(GetLocalResourceObject("SelezionareUnaUnitàDiMisura"), String) & vbCrLf
        Else
            UdmCod = cmb_Udm.SelectedItem.Value
            UdmDes = cmb_Udm.SelectedItem.Text
        End If

        '-----
        'QUANTITA'
        If Me.Txt_Quantita.Text <> "" Then
            If Not IsNumeric(Me.Txt_Quantita.Text) Then
                Str_Errore &= DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerLaQuantità"), String) & vbCrLf
            Else
                If Me.Txt_Quantita.Text <= 0 Then
                    Str_Errore &= DirectCast(GetLocalResourceObject("ImpossibileInserireUnaQuantitàNullaONegativa"), String) & vbCrLf
                Else
                    If InStr(Me.Txt_Quantita.Text, ".") <> 0 Then
                        Me.Txt_Quantita.Text = Replace(Me.Txt_Quantita.Text, ".", ",")
                    End If
                    Qta = CDbl(Me.Txt_Quantita.Text)
                End If
            End If
        Else
            Str_Errore &= DirectCast(GetLocalResourceObject("InserireUnaQuantità"), String) & vbCrLf
        End If


        '-----
        'PREZZO UNITARIO
        If Me.Txt_PrezzoUnitario.Text <> "" Then
            If Not IsNumeric(Me.Txt_PrezzoUnitario.Text) Then
                Str_Errore &= DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerIlPrezzo"), String) & vbCrLf
            Else
                Prezzo = CDbl(Txt_PrezzoUnitario.Text)
            End If
        Else
            Prezzo = CDbl(0)
        End If

        '-----
        'PREZZO UNITARIO NETTO
        Select Case Me.Txt_PrezzoUnitario_Netto.Text
            Case "", "0", "0,0", "0,00"
                PrezzoNetto = Prezzo
            Case Else

                Select Case CInt(Qs_Lav_Cod)
                    Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_RICEVUTA
                        'in questi casi non sono gestiti i dettagli economici
                        'perciò prezzo netto è sempre uguale a prezzo
                        PrezzoNetto = Prezzo
                    Case Else
                        PrezzoNetto = CDbl(Txt_PrezzoUnitario_Netto.Text)
                End Select

        End Select

        '-----------------------------------------


    End Sub


    '###############################################################################
    Private Sub Ripristina_DettagliEconomici(ByVal Udm_Cod_Extra As Decimal,
                                            ByVal Qta_Extra As Decimal,
                                            ByVal Prezzo_Effettivo As Decimal,
                                           ByVal Sconto_Magg As Decimal,
                                           ByVal Imponibile As Decimal,
                                           ByVal Imponibile_Netto As Decimal,
                                           ByVal Cod_Iva As Integer,
                                           ByVal Iva As Decimal,
                                           ByVal Anno As Integer,
                                           ByVal Ric_Cod As Integer,
                                           ByVal Cod_Conto As Integer)

        Me.Txt_Qta_Extra.Text = Qta_Extra

        If Sconto_Magg >= 0 Then
            Me.Txt_MaggiorazionePercentuale.Text = Sconto_Magg
            Me.Opt_Maggiorazione.Checked = True
            Me.Opt_Sconto.Checked = False
        Else
            Me.Txt_ScontoPercentuale.Text = -Sconto_Magg
            Me.Opt_Sconto.Checked = True
            Me.Opt_Maggiorazione.Checked = False
        End If

        Me.Txt_Imponibile.Text = Imponibile

        Me.Txt_Imponibile_Netto.Text = Imponibile_Netto

        For i = 0 To Cmb_IVA.Items.Count - 1
            If CInt(Split(Cmb_IVA.Items(i).Value, "|")(0)) = Cod_Iva Then
                Cmb_IVA.SelectedIndex = Me.Cmb_IVA.Items.IndexOf(Cmb_IVA.Items.FindByValue(Cmb_IVA.Items(i).Value))
                Exit For
            End If
        Next
        'Cmb_IVA.SelectedIndex = Me.Cmb_IVA.Items.IndexOf(Cmb_IVA.Items.FindByValue(Cod_Iva))

        Me.Txt_IVA.Text = Iva

        Me.Cmb_AnnoContabile.SelectedIndex =
                Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_AnnoContabile.Items.FindByValue(Anno))

        Me.Cmb_Conti.SelectedIndex =
            Me.Cmb_Conti.Items.IndexOf(Me.Cmb_Conti.Items.FindByValue(Cod_Conto))


        Dim Str_Errore As String
        If Not UsaNuoviArrotondamenti(objParametri_Server) Then
            Str_Errore = AggiornaDettagliEconomici()
        Else
            Str_Errore = AggiornaDettagliEconomiciNuoviArrotondamenti()
        End If

    End Sub

    '###############################################################################
    Private Sub Recupera_DettagliFertilizzante(ByRef Str_Errore As String,
                                               ByRef N As Decimal,
                                               ByRef P2O5 As Decimal,
                                               ByRef K2O As Decimal,
                                               ByRef Cu As Decimal,
                                               ByRef Regolamento_Cod_Ferti As Integer)


        '-----
        'N
        If Me.Txt_N.Text <> "" Then
            If Not IsNumeric(Me.Txt_N.Text) Then
                Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerIndicarePlaceholder"), String), "N") & vbCrLf
            Else
                If Me.Txt_N.Text < 0 Then
                    Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("ImpossibileInserireUnValoreNegativoPlaceholder"), String), "N") & vbCrLf
                Else
                    If InStr(Me.Txt_N.Text, ".") <> 0 Then
                        Me.Txt_N.Text = Replace(Me.Txt_N.Text, ".", ",")
                    End If
                    N = CDec(Me.Txt_N.Text)
                End If
            End If
        Else
            N = CDec(0)
        End If

        '-----
        'P2O5
        If Me.Txt_P2O5.Text <> "" Then
            If Not IsNumeric(Me.Txt_P2O5.Text) Then
                Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerIndicarePlaceholder"), String), "P2O5") & vbCrLf
            Else
                If Me.Txt_P2O5.Text < 0 Then
                    Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("ImpossibileInserireUnValoreNegativoPlaceholder"), String), "P2O5") & vbCrLf
                Else
                    If InStr(Me.Txt_P2O5.Text, ".") <> 0 Then
                        Me.Txt_P2O5.Text = Replace(Me.Txt_P2O5.Text, ".", ",")
                    End If
                    P2O5 = CDec(Me.Txt_P2O5.Text)
                End If
            End If
        Else
            P2O5 = CDec(0)
        End If

        '-----
        'K2O
        If Me.Txt_K2O.Text <> "" Then
            If Not IsNumeric(Me.Txt_K2O.Text) Then
                Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerIndicarePlaceholder"), String), "K2O") & vbCrLf
            Else
                If Me.Txt_K2O.Text < 0 Then
                    Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("ImpossibileInserireUnValoreNegativoPlaceholder"), String), "K2O") & vbCrLf
                Else
                    If InStr(Me.Txt_K2O.Text, ".") <> 0 Then
                        Me.Txt_K2O.Text = Replace(Me.Txt_K2O.Text, ".", ",")
                    End If
                    K2O = CDec(Me.Txt_K2O.Text)
                End If
            End If
        Else
            K2O = CDec(0)
        End If

        '-----
        'Cu
        If Me.Txt_Cu.Text <> "" Then
            If Not IsNumeric(Me.Txt_Cu.Text) Then
                Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerIndicarePlaceholder"), String), "Cu") & vbCrLf
            Else
                If Me.Txt_Cu.Text < 0 Then
                    Str_Errore &= String.Format(DirectCast(GetLocalResourceObject("ImpossibileInserireUnValoreNegativoPlaceholder"), String), "Cu") & vbCrLf
                Else
                    If InStr(Me.Txt_Cu.Text, ".") <> 0 Then
                        Me.Txt_Cu.Text = Replace(Me.Txt_Cu.Text, ".", ",")
                    End If
                    Cu = CDec(Me.Txt_Cu.Text)
                End If
            End If
        Else
            Cu = CDec(0)
        End If

        If xCaricoScarico = CAU_CARICO AndAlso
           cmb_Categoria.SelectedValue = FERTILIZZANTI AndAlso
           cmb_PUARegolamenti.Visible AndAlso
           cmb_PUARegolamenti.Items.Count > 0 AndAlso
           cmb_PUARegolamenti.SelectedItem.Value.Split(SEP_PuaReg)(0) <> "0" Then
            Regolamento_Cod_Ferti = cmb_PUARegolamenti.SelectedItem.Value.Split(SEP_PuaReg)(0)
        Else
            Regolamento_Cod_Ferti = 0
        End If

    End Sub

    '###############################################################################
    Private Sub Ripristina_DettagliFertilizzante(ByVal N As Decimal,
                                                 ByVal P2O5 As Decimal,
                                                 ByVal K2O As Decimal,
                                                 ByVal Cu As Decimal,
                                                 ByVal Regolamento_Cod_Ferti As Integer)

        Me.Txt_N.Text = N
        Me.Txt_P2O5.Text = P2O5
        Me.Txt_K2O.Text = K2O
        Me.Txt_Cu.Text = Cu

        'Nella drop down in realtà c'è Cod_regolamento/Tipo_Regolamento ==>
        'devo prima trovare l'elemento che comincia con "Regolamento_Cod_Ferti/" ed
        'a quel punto ho il "codice" completo dell'elemento e lo posso selezionare

        If Regolamento_Cod_Ferti <> 0 Then

            ' carico la combo se non l'ho già caricata
            If cmb_PUARegolamenti.Items.Count = 0 Then
                CaricaCombo_Regolamenti()
            End If

            Dim elementoReg = cmb_PUARegolamenti.Items.Cast(Of ListItem).FirstOrDefault(Function(x) x.Value.StartsWith(Regolamento_Cod_Ferti & SEP_PuaReg))

            If Not IsNothing(elementoReg) Then
                cmb_PUARegolamenti.SelectedValue = elementoReg.Value
                cmb_PUARegolamenti.Visible = True
                Lbl_PUARegolamenti.Visible = True
            End If
        End If

    End Sub

    '###############################################################################
    Private Sub Recupera_DettagliNascosti(ByRef Rif_Esterno As String, ByRef Rif_Esterno_2 As String)
        Rif_Esterno = If(hf_RifEsterno.Value, "")
        Rif_Esterno_2 = If(hf_RifEsterno_2.Value, "")
    End Sub

    '###############################################################################
    Private Sub Ripristina_DettagliNascosti(ByVal Rif_Esterno As String, ByVal Rif_Esterno_2 As String)
        hf_RifEsterno.Value = Rif_Esterno
        hf_RifEsterno_2.Value = Rif_Esterno_2
    End Sub

    '###############################################################################
    Private Sub Btn_CalcolaImporti_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcolaImporti.Click


        Dim Str_Errore As String

        If Not UsaNuoviArrotondamenti(objParametri_Server) Then
            Str_Errore = AggiornaDettagliEconomici()
        Else
            Str_Errore = AggiornaDettagliEconomiciNuoviArrotondamenti()
        End If

        If Str_Errore <> "" Then
            Messaggi.AgroMsgBox(Str_Errore, Page, , Me.upDati)
        End If


    End Sub


    '###############################################################################
    Private Sub Chk_IVAmanuale_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_IVAmanuale.CheckedChanged

        Select Case Me.Chk_IVAmanuale.Checked

            Case True
                'IVA MODIFICATA A MANO

                Me.Txt_IVAmanuale.Enabled = True
                Me.Txt_IVAmanuale.BackColor = Color.White

                'LA TXT MANUALE VIENE IMPOSTATA UGUALE A QUELLA AUTO
                Me.Txt_IVAmanuale.Text = Me.Txt_IVA.Text

            Case False
                'IVA CALCOLATA AUTOMATICAMENTE

                Me.Txt_IVAmanuale.Enabled = False
                Me.Txt_IVAmanuale.BackColor = AgroColor_VerdeChiaro

                Me.Txt_IVAmanuale.Text = "0"

        End Select

    End Sub

    '###############################################################################
    Private Function AggiornaDettagliEconomici() As String

        Dim PercentualeVariazione As Decimal = 0
        Dim Variazione As Decimal = 0
        Dim Percentuale_Sconto As Decimal = 0
        Dim Percentuale_Magg As Decimal = 0
        Dim Sconto As Decimal = 0
        Dim Maggiorazione As Decimal = 0

        Dim Aliquota As Integer
        Dim IVA_manuale As Decimal = 0
        Dim IVA_auto As Decimal = 0

        Dim Quantita As Decimal = 0
        Dim Prezzo_Unitario As Decimal = 0
        Dim Prezzo_Unitario_Netto As Decimal = 0
        Dim Imponibile As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Importo As Decimal = 0

        Dim x_EditImporto As Integer = 0
        Dim x_Editvariazione As Integer = 0

        Dim Messaggio As String = ""

        '-------------------------------------

        Select Case Me.Chk_IVAmanuale.Checked
            Case True
                'IVA MODIFICATA A MANO
                If Me.Txt_IVAmanuale.Text = "" OrElse Not IsNumeric(Me.Txt_IVAmanuale.Text) Then
                    ' Messaggi.AgroMsgBox("E' stato scelto di impostare l'IVA manualmente, ma non ne è stato impostato il valore o il valore non è corretto!",Page, , Me.upDati)
                    Messaggio &= DirectCast(GetLocalResourceObject("IvaValoreMancanteONonCorretto"), String) & vbCrLf
                    'Exit Function
                Else
                    IVA_manuale = CDbl(Me.Txt_IVAmanuale.Text)
                End If
            Case False
                'IVA CALCOLATA AUTOMATICAMENTE
                IVA_manuale = 0
        End Select


        If Me.Opt_PrezzoUnitario.Checked Then
            x_EditImporto = enum_EditImporto.PrezzoUnitario
        ElseIf Me.Opt_Imponibile.Checked Then
            x_EditImporto = enum_EditImporto.Imponibile
        ElseIf Me.Opt_Importo.Checked Then
            x_EditImporto = enum_EditImporto.Importo
        End If

        If Me.Opt_Sconto.Checked Then

            If Me.Txt_ScontoPercentuale.Text = "" OrElse Not IsNumeric(Trim(Me.Txt_ScontoPercentuale.Text)) Then
                Percentuale_Sconto = 0
            Else
                Percentuale_Sconto = CDbl(Trim(Me.Txt_ScontoPercentuale.Text))
            End If

            Percentuale_Magg = 0
            Maggiorazione = 0

            If Percentuale_Sconto = 0 Then
                'Me.Txt_Maggiorazione.Visible = True
                'Me.lbl_magg.Visible = True
                'Me.Txt_Maggiorazione_Netta.Visible = True
            Else
                'Me.Txt_Maggiorazione.Visible = False
                'Me.lbl_magg.Visible = False
                'Me.Txt_Maggiorazione_Netta.Visible = False
                'Me.Txt_Maggiorazione.Text = Format(0, "0.00")
                PercentualeVariazione = -CDbl(Percentuale_Sconto)
            End If

        ElseIf Me.Opt_Maggiorazione.Checked Then

            If Me.Txt_MaggiorazionePercentuale.Text = "" OrElse Not IsNumeric(Trim(Me.Txt_MaggiorazionePercentuale.Text)) Then
                Percentuale_Magg = 0
            Else
                Percentuale_Magg = CDbl(Trim(Me.Txt_MaggiorazionePercentuale.Text))
            End If

            Percentuale_Sconto = 0
            Sconto = 0

            If Percentuale_Magg = 0 Then
                'Me.Txt_Sconto.Visible = True
                'Me.lbl_sconto.Visible = True
                'Me.Txt_Sconto_Netto.Visible = True
            Else
                'Me.Txt_Sconto.Visible = False
                'Me.lbl_sconto.Visible = False
                'Me.Txt_Sconto_Netto.Visible = False
                'Me.Txt_Sconto.Text = Format(0, "0.00")
                PercentualeVariazione = CDbl(Percentuale_Magg)
            End If

        End If


        If Me.Txt_Quantita.Text = "" OrElse Not IsNumeric(Trim(Me.Txt_Quantita.Text)) Then
            Quantita = 0
        Else
            Quantita = CDbl(Trim(Me.Txt_Quantita.Text))
        End If


        Select Case x_EditImporto


            '#########################################################################################
            '##################### MODALITA' INSERIMENTO = PREZZO UNITARIO  ##########################
            '#########################################################################################


            Case enum_EditImporto.PrezzoUnitario

                'Aggiorno il costo complessivo della registrazione
                If IsNumeric(Trim(Me.Txt_PrezzoUnitario.Text)) Then

                    Prezzo_Unitario = CDbl(Trim(Me.Txt_PrezzoUnitario.Text))
                    If Me.Txt_Quantita.Text <> "" Then
                        Quantita = CDbl(Me.Txt_Quantita.Text)
                    End If


                    Prezzo_Unitario_Netto = Prezzo_Unitario + (Prezzo_Unitario * PercentualeVariazione / 100)

                    'Sconto_Magg_Netto = Format(Math.Abs(CDbl(Me.Txt_PrezzoUnitario_Netto.Text - Me.Txt_PrezzoUnitario.Text)), "#,###,##0.00######")
                    Variazione = Math.Abs(Prezzo_Unitario_Netto - Prezzo_Unitario)

                    Imponibile = Quantita * Prezzo_Unitario

                    Imponibile_Netto = Quantita * Prezzo_Unitario_Netto

                    'Iva
                    If Me.Cmb_IVA.SelectedIndex > 0 Then

                        Aliquota = CInt(Split(Cmb_IVA.SelectedValue, "|")(1)) 'CInt(Me.Cmb_IVA.SelectedValue)

                        IVA_auto = (Imponibile_Netto * Aliquota) / 100

                        'Select Case Aliquota
                        '    Case IVA_4, IVA_10, IVA_12, IVA_20, IVA_21, IVA_22
                        '         IVA_auto = (Imponibile_Netto * Aliquota) / 100
                        '    Case Else
                        '         IVA_auto = 0 'FCI, NI
                        'End Select
                    Else
                        IVA_auto = 0
                    End If

                    If Not Me.Chk_IVAmanuale.Checked Then
                        Importo = Imponibile_Netto + IVA_auto
                    Else
                        If Math.Round(Math.Abs(IVA_auto - IVA_manuale), 2) > 0.02 Then
                            ' Messaggi.AgroMsgBox("L'IVA impostata può essere arrotondata, rispetto a quella proposta automaticamente, al massimo di 2 centesimi (in difetto o in eccesso)!" & vbCrLf & _
                            '"Impostare il valore corretto!",Page, , Me.upDati)
                            Messaggio &= DirectCast(GetLocalResourceObject("IvaArrotondabileAlMassimoDiDueCentesimiImpostareValoreCorretto"), String) & vbCrLf
                        End If
                        Importo = Imponibile_Netto + IVA_manuale
                    End If

                Else
                    Prezzo_Unitario_Netto = 0
                    Imponibile = 0
                    Imponibile_Netto = 0
                    Importo = 0
                    Sconto = 0
                    Maggiorazione = 0
                    IVA_auto = 0
                End If

                'imposto i valori nelle textbox
                Me.Txt_PrezzoUnitario_Netto.Text = Format(Prezzo_Unitario_Netto, "#,###,##0.00######")
                Me.Txt_Imponibile.Text = Format(Imponibile, "#,###,##0.00")
                Me.Txt_Imponibile_Netto.Text = Format(Imponibile_Netto, "#,###,##0.00")
                Me.Txt_IVA.Text = Format(IVA_auto, "#,###,##0.00")
                Me.Txt_Importo.Text = Format(Importo, "#,###,##0.00")



                '#########################################################################################
                '##################### MODALITA' INSERIMENTO = IMPORTO TOTALE  ###########################
                '#########################################################################################

            Case enum_EditImporto.Importo

                'Aggiorno il costo unitario della registrazione
                If Quantita > 0 AndAlso IsNumeric(Trim(Me.Txt_Importo.Text)) Then

                    Importo = CDbl(Trim(Me.Txt_Importo.Text))

                    'Iva
                    If Me.Cmb_IVA.SelectedIndex > 0 Then

                        'Aliquota = CInt(Me.Cmb_IVA.SelectedValue)
                        Aliquota = CInt(Split(Cmb_IVA.SelectedValue, "|")(1))

                        'Select Case Aliquota
                        '    Case IVA_4, IVA_10, IVA_12, IVA_20, IVA_21
                        '        '100 + aliquota : importo = aliquota : iva
                        '        IVA_auto = ((Importo * Aliquota) / (100 + Aliquota))
                        '    Case Else
                        '        IVA_auto = 0
                        'End Select

                        '100 + aliquota : importo = aliquota : iva
                        IVA_auto = ((Importo * Aliquota) / (100 + Aliquota))

                    Else
                        IVA_auto = 0
                    End If

                    'imponibile netto = importo - iva
                    If Not Me.Chk_IVAmanuale.Checked Then
                        Imponibile_Netto = Importo - IVA_auto
                    Else
                        If Math.Abs(IVA_auto - IVA_manuale) > 0.02 Then
                            ' Messaggi.AgroMsgBox("L'IVA impostata può essere arrotondata, rispetto a quella proposta automaticamente, al massimo di 2 centesimi (in difetto o in eccesso)!" & vbCrLf & _
                            '"Impostare il valore corretto!",Page, , Me.upDati)
                            Messaggio &= DirectCast(GetLocalResourceObject("IvaArrotondabileAlMassimoDiDueCentesimiImpostareValoreCorretto"), String) & vbCrLf
                        End If
                        Imponibile_Netto = Importo - IVA_manuale
                    End If

                    ' prezzo netto = imponib netto / qta
                    Prezzo_Unitario_Netto = Imponibile_Netto / Quantita

                    'prezzo = prezzo netto -  (-sconto o +magg)   100 +/- sconto : prezzo netto = %sconto : prezzo
                    Prezzo_Unitario = Prezzo_Unitario_Netto - ((Prezzo_Unitario_Netto * PercentualeVariazione) / (PercentualeVariazione + 100))

                    Imponibile = Quantita * Prezzo_Unitario

                    'Variazione = Format(Math.Abs(CDbl(Me.Txt_Imponibile.Text) - CDbl(Me.Txt_Imponibile_Netto.Text)), "#,###,##0.00######")
                    Variazione = Math.Abs(Imponibile - Imponibile_Netto)

                Else
                    Prezzo_Unitario_Netto = 0
                    Imponibile = 0
                    Imponibile_Netto = 0
                    Prezzo_Unitario = 0
                    Sconto = 0
                    Maggiorazione = 0
                    IVA_auto = 0
                End If

                Me.Txt_IVA.Text = Format(IVA_auto, "##,###,##0.00") 'FCI, NI
                Me.Txt_Imponibile_Netto.Text = Format(Imponibile_Netto, "#,###,##0.00")
                Me.Txt_PrezzoUnitario_Netto.Text = Format(Prezzo_Unitario_Netto, "#,###,##0.00######")
                Me.Txt_PrezzoUnitario.Text = Format(Prezzo_Unitario, "#,###,##0.00######")
                Me.Txt_Imponibile.Text = Format(Imponibile, "#,###,##0.00")



                '#########################################################################################
                '##################### MODALITA' INSERIMENTO = IMPONIBILE ################################
                '#########################################################################################

            Case enum_EditImporto.Imponibile

                'Aggiorno il costo unitario della registrazione
                If Quantita > 0 AndAlso IsNumeric(Trim(Me.Txt_Imponibile.Text)) Then

                    Imponibile = CDbl(Trim(Me.Txt_Imponibile.Text))

                    Prezzo_Unitario = (Imponibile / Quantita)

                    '100 +/- sconto : imponib netto = 100 : imponibile
                    Imponibile_Netto = Imponibile * (100 + PercentualeVariazione) / 100

                    Prezzo_Unitario_Netto = Imponibile_Netto / Quantita

                    Variazione = Math.Abs(Prezzo_Unitario_Netto - Prezzo_Unitario)

                    'Iva
                    If Me.Cmb_IVA.SelectedIndex > 0 Then

                        Aliquota = CInt(Split(Cmb_IVA.SelectedValue, "|")(1)) ' CInt(Me.Cmb_IVA.SelectedValue)

                        IVA_auto = (Imponibile_Netto * Aliquota) / 100

                        'Select Case Aliquota
                        '    Case IVA_4, IVA_10, IVA_12, IVA_20, IVA_21
                        '        IVA_auto = (Imponibile_Netto * Aliquota) / 100
                        '    Case Else
                        '        IVA_auto = 0
                        'End Select
                    Else
                        IVA_auto = 0
                    End If

                    If Not Me.Chk_IVAmanuale.Checked Then
                        Importo = Imponibile_Netto + IVA_auto
                    Else
                        If Math.Abs(IVA_auto - IVA_manuale) > 0.02 Then
                            ' Messaggi.AgroMsgBox("L'IVA impostata può essere arrotondata, rispetto a quella proposta automaticamente, al massimo di 2 centesimi (in difetto o in eccesso)!" & vbCrLf & _
                            '"Impostare il valore corretto!",Page, , Me.upDati)
                            Messaggio &= DirectCast(GetLocalResourceObject("IvaArrotondabileAlMassimoDiDueCentesimiImpostareValoreCorretto"), String) & vbCrLf
                        End If
                        Importo = Imponibile_Netto + IVA_manuale
                    End If

                Else
                    Prezzo_Unitario_Netto = 0
                    Importo = 0
                    Prezzo_Unitario = 0
                    Sconto = 0
                    Maggiorazione = 0
                    Imponibile_Netto = 0
                    IVA_auto = 0
                End If

                Me.Txt_PrezzoUnitario.Text = Format(Prezzo_Unitario, "##,###,##0.00")
                Me.Txt_Imponibile_Netto.Text = Format(Imponibile_Netto, "##,###,##0.00")
                Me.Txt_PrezzoUnitario_Netto.Text = Format(Prezzo_Unitario_Netto, "##,###,##0.00")
                Me.Txt_Importo.Text = Format(Importo, "##,###,##0.00")
                Me.Txt_IVA.Text = Format(IVA_auto, "#,###,##0.00")


        End Select

        '###########################################################


        If Me.Opt_Sconto.Checked Then
            Sconto = Variazione
        ElseIf Me.Opt_Maggiorazione.Checked Then
            Maggiorazione = Variazione
        End If

        Me.Txt_Sconto_Calcolato.Text = Format(Sconto, "##,###,##0.00")
        Me.Txt_Maggiorazione_Calcolato.Text = Format(Maggiorazione, "##,###,##0.00")

        Return Messaggio


    End Function

    '###############################################################################
    Private Function AggiornaDettagliEconomiciNuoviArrotondamenti() As String

        Dim PercentualeVariazione As Decimal = 0
        Dim Variazione As Decimal = 0
        Dim Percentuale_Sconto As Decimal = 0
        Dim Percentuale_Magg As Decimal = 0
        Dim Sconto As Decimal = 0
        Dim Maggiorazione As Decimal = 0

        Dim Aliquota As Integer
        Dim IVA_manuale As Decimal = 0
        Dim IVA_auto As Decimal = 0

        Dim Quantita As Decimal = 0
        Dim Prezzo_Unitario As Decimal = 0
        Dim Prezzo_Unitario_Netto As Decimal = 0
        Dim Imponibile As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Importo As Decimal = 0

        Dim x_EditImporto As Integer = 0
        Dim x_Editvariazione As Integer = 0

        Dim Messaggio As String = ""

        '-------------------------------------

        Select Case Me.Chk_IVAmanuale.Checked
            Case True
                'IVA MODIFICATA A MANO
                If Me.Txt_IVAmanuale.Text = "" OrElse Not IsNumeric(Me.Txt_IVAmanuale.Text) Then
                    ' Messaggi.AgroMsgBox("E' stato scelto di impostare l'IVA manualmente, ma non ne è stato impostato il valore o il valore non è corretto!",Page, , Me.upDati)
                    Messaggio &= DirectCast(GetLocalResourceObject("IvaValoreMancanteONonCorretto"), String) & vbCrLf
                    'Exit Function
                Else
                    IVA_manuale = CDbl(Me.Txt_IVAmanuale.Text)
                End If
            Case False
                'IVA CALCOLATA AUTOMATICAMENTE
                IVA_manuale = 0
        End Select


        If Me.Opt_PrezzoUnitario.Checked Then
            x_EditImporto = enum_EditImporto.PrezzoUnitario
        ElseIf Me.Opt_Imponibile.Checked Then
            x_EditImporto = enum_EditImporto.Imponibile
        ElseIf Me.Opt_Importo.Checked Then
            x_EditImporto = enum_EditImporto.Importo
        End If

        If Me.Opt_Sconto.Checked Then

            If Me.Txt_ScontoPercentuale.Text = "" OrElse Not IsNumeric(Trim(Me.Txt_ScontoPercentuale.Text)) Then
                Percentuale_Sconto = 0
            Else
                Percentuale_Sconto = CDec(Trim(Me.Txt_ScontoPercentuale.Text))
            End If

            Percentuale_Magg = 0
            Maggiorazione = 0

            If Percentuale_Sconto = 0 Then
                'Me.Txt_Maggiorazione.Visible = True
                'Me.lbl_magg.Visible = True
                'Me.Txt_Maggiorazione_Netta.Visible = True
            Else
                'Me.Txt_Maggiorazione.Visible = False
                'Me.lbl_magg.Visible = False
                'Me.Txt_Maggiorazione_Netta.Visible = False
                'Me.Txt_Maggiorazione.Text = Format(0, "0.00")
                PercentualeVariazione = -CDec(Percentuale_Sconto)
            End If

        ElseIf Me.Opt_Maggiorazione.Checked Then

            If Me.Txt_MaggiorazionePercentuale.Text = "" OrElse Not IsNumeric(Trim(Me.Txt_MaggiorazionePercentuale.Text)) Then
                Percentuale_Magg = 0
            Else
                Percentuale_Magg = CDec(Trim(Me.Txt_MaggiorazionePercentuale.Text))
            End If

            Percentuale_Sconto = 0
            Sconto = 0

            If Percentuale_Magg = 0 Then
                'Me.Txt_Sconto.Visible = True
                'Me.lbl_sconto.Visible = True
                'Me.Txt_Sconto_Netto.Visible = True
            Else
                'Me.Txt_Sconto.Visible = False
                'Me.lbl_sconto.Visible = False
                'Me.Txt_Sconto_Netto.Visible = False
                'Me.Txt_Sconto.Text = Format(0, "0.00")
                PercentualeVariazione = CDec(Percentuale_Magg)
            End If

        End If


        If Me.Txt_Quantita.Text = "" OrElse Not IsNumeric(Trim(Me.Txt_Quantita.Text)) Then
            Quantita = 0
        Else
            Quantita = CDec(Trim(Me.Txt_Quantita.Text))
        End If


        Select Case x_EditImporto


            '#########################################################################################
            '##################### MODALITA' INSERIMENTO = PREZZO UNITARIO  ##########################
            '#########################################################################################


            Case enum_EditImporto.PrezzoUnitario

                'Aggiorno il costo complessivo della registrazione
                If IsNumeric(Trim(Me.Txt_PrezzoUnitario.Text)) Then

                    Prezzo_Unitario = Agro_Math.ArrotondaVal_6(Decimal.Parse((Trim(Me.Txt_PrezzoUnitario.Text))))
                    If Me.Txt_Quantita.Text <> "" Then
                        Quantita = CDec(Me.Txt_Quantita.Text)
                    End If

                    Imponibile = Agro_Math.ArrotondaVal_2(Quantita * Prezzo_Unitario)

                    Imponibile_Netto = Agro_Math.ArrotondaVal_2(Imponibile + (Imponibile * PercentualeVariazione / 100))

                    If PercentualeVariazione = 0 Then
                        Prezzo_Unitario_Netto = Prezzo_Unitario
                    Else
                        Prezzo_Unitario_Netto = Agro_Math.ArrotondaVal_6(Imponibile_Netto / Quantita)
                    End If

                    'Sconto_Magg_Netto = Format(Math.Abs(CDec(Me.Txt_PrezzoUnitario_Netto.Text - Me.Txt_PrezzoUnitario.Text)), "#,###,##0.00######")
                    Variazione = Math.Abs(Prezzo_Unitario_Netto - Prezzo_Unitario)

                    'Iva
                    If Me.Cmb_IVA.SelectedIndex > 0 Then

                        Aliquota = CInt(Split(Cmb_IVA.SelectedValue, "|")(1)) 'CInt(Me.Cmb_IVA.SelectedValue)

                        IVA_auto = Agro_Math.ArrotondaVal_2(Imponibile_Netto * Aliquota / 100)

                        'Select Case Aliquota
                        '    Case IVA_4, IVA_10, IVA_12, IVA_20, IVA_21, IVA_22
                        '         IVA_auto = (Imponibile_Netto * Aliquota) / 100
                        '    Case Else
                        '         IVA_auto = 0 'FCI, NI
                        'End Select
                    Else
                        IVA_auto = 0
                    End If

                    If Not Me.Chk_IVAmanuale.Checked Then
                        Importo = Imponibile_Netto + IVA_auto
                    Else
                        If Math.Round(Math.Abs(IVA_auto - IVA_manuale), 2) > 0.02 Then
                            ' Messaggi.AgroMsgBox("L'IVA impostata può essere arrotondata, rispetto a quella proposta automaticamente, al massimo di 2 centesimi (in difetto o in eccesso)!" & vbCrLf & _
                            '"Impostare il valore corretto!",Page, , Me.upDati)
                            Messaggio &= DirectCast(GetLocalResourceObject("IvaArrotondabileAlMassimoDiDueCentesimiImpostareValoreCorretto"), String) & vbCrLf
                        End If
                        Importo = Imponibile_Netto + IVA_manuale
                    End If

                Else
                    Prezzo_Unitario_Netto = 0
                    Imponibile = 0
                    Imponibile_Netto = 0
                    Importo = 0
                    Sconto = 0
                    Maggiorazione = 0
                    IVA_auto = 0
                End If

                'imposto i valori nelle textbox
                Me.Txt_PrezzoUnitario.Text = Format(Prezzo_Unitario, "#,###,##0.00####")
                Me.Txt_PrezzoUnitario_Netto.Text = Format(Prezzo_Unitario_Netto, "#,###,##0.00####")
                Me.Txt_Imponibile.Text = Format(Imponibile, "#,###,##0.00")
                Me.Txt_Imponibile_Netto.Text = Format(Imponibile_Netto, "#,###,##0.00")
                Me.Txt_IVA.Text = Format(IVA_auto, "#,###,##0.00")
                Me.Txt_Importo.Text = Format(Importo, "#,###,##0.00")



                '#########################################################################################
                '##################### MODALITA' INSERIMENTO = IMPORTO TOTALE  ###########################
                '#########################################################################################

            Case enum_EditImporto.Importo

                'Aggiorno il costo unitario della registrazione
                If Quantita > 0 AndAlso IsNumeric(Trim(Me.Txt_Importo.Text)) Then

                    Importo = Agro_Math.ArrotondaVal_2(Decimal.Parse(Trim(Me.Txt_Importo.Text)))

                    'Iva
                    If Me.Cmb_IVA.SelectedIndex > 0 Then

                        'Aliquota = CInt(Me.Cmb_IVA.SelectedValue)
                        Aliquota = CInt(Split(Cmb_IVA.SelectedValue, "|")(1))

                        'Select Case Aliquota
                        '    Case IVA_4, IVA_10, IVA_12, IVA_20, IVA_21
                        '        '100 + aliquota : importo = aliquota : iva
                        '        IVA_auto = ((Importo * Aliquota) / (100 + Aliquota))
                        '    Case Else
                        '        IVA_auto = 0
                        'End Select

                        '100 + aliquota : importo = aliquota : iva
                        IVA_auto = Agro_Math.ArrotondaVal_2((Importo * Aliquota) / (100 + Aliquota))

                    Else
                        IVA_auto = 0
                    End If

                    'imponibile netto = importo - iva
                    If Not Me.Chk_IVAmanuale.Checked Then
                        Imponibile_Netto = Importo - IVA_auto
                    Else
                        If Math.Abs(IVA_auto - IVA_manuale) > 0.02 Then
                            ' Messaggi.AgroMsgBox("L'IVA impostata può essere arrotondata, rispetto a quella proposta automaticamente, al massimo di 2 centesimi (in difetto o in eccesso)!" & vbCrLf & _
                            '"Impostare il valore corretto!",Page, , Me.upDati)
                            Messaggio &= DirectCast(GetLocalResourceObject("IvaArrotondabileAlMassimoDiDueCentesimiImpostareValoreCorretto"), String) & vbCrLf
                        End If
                        Imponibile_Netto = Importo - IVA_manuale
                    End If

                    ' prezzo netto = imponib netto / qta
                    Prezzo_Unitario_Netto = Agro_Math.ArrotondaVal_6(Imponibile_Netto / Quantita)

                    Imponibile = Agro_Math.ArrotondaVal_2(Imponibile_Netto / (100 + PercentualeVariazione) * 100)

                    Prezzo_Unitario = Agro_Math.ArrotondaVal_6(Imponibile / Quantita)
                    'prezzo = prezzo netto -  (-sconto o +magg)   100 +/- sconto : prezzo netto = %sconto : prezzo
                    ''''Prezzo_Unitario = Prezzo_Unitario_Netto - Agro_Math.ArrotondaVal_6((Prezzo_Unitario_Netto * PercentualeVariazione) / (PercentualeVariazione + 100))

                    ''''Imponibile = Agro_Math.ArrotondaVal_2(Quantita * Prezzo_Unitario)

                    'Variazione = Format(Math.Abs(CDec(Me.Txt_Imponibile.Text) - CDec(Me.Txt_Imponibile_Netto.Text)), "#,###,##0.00######")
                    ''''''Variazione = Math.Abs(Imponibile - Imponibile_Netto)
                    Variazione = Math.Abs(Prezzo_Unitario_Netto - Prezzo_Unitario)

                Else
                    Prezzo_Unitario_Netto = 0
                    Imponibile = 0
                    Imponibile_Netto = 0
                    Prezzo_Unitario = 0
                    Sconto = 0
                    Maggiorazione = 0
                    IVA_auto = 0
                End If

                Me.Txt_IVA.Text = Format(IVA_auto, "##,###,##0.00") 'FCI, NI
                Me.Txt_Imponibile_Netto.Text = Format(Imponibile_Netto, "#,###,##0.00")
                Me.Txt_PrezzoUnitario_Netto.Text = Format(Prezzo_Unitario_Netto, "#,###,##0.00#####")
                Me.Txt_PrezzoUnitario.Text = Format(Prezzo_Unitario, "#,###,##0.00#####")
                Me.Txt_Imponibile.Text = Format(Imponibile, "#,###,##0.00")
                Me.Txt_Importo.Text = Format(Importo, "##,###,##0.00")

                '#########################################################################################
                '##################### MODALITA' INSERIMENTO = IMPONIBILE ################################
                '#########################################################################################

            Case enum_EditImporto.Imponibile

                'Aggiorno il costo unitario della registrazione
                If Quantita > 0 AndAlso IsNumeric(Trim(Me.Txt_Imponibile.Text)) Then

                    Imponibile = Agro_Math.ArrotondaVal_2(Decimal.Parse(Trim(Me.Txt_Imponibile.Text)))

                    Prezzo_Unitario = Agro_Math.ArrotondaVal_6(Imponibile / Quantita)

                    '100 +/- sconto : imponib netto = 100 : imponibile
                    Imponibile_Netto = Agro_Math.ArrotondaVal_2(Imponibile * (100 + PercentualeVariazione) / 100)

                    Prezzo_Unitario_Netto = Agro_Math.ArrotondaVal_6(Imponibile_Netto / Quantita)

                    Variazione = Math.Abs(Prezzo_Unitario_Netto - Prezzo_Unitario)

                    'Iva
                    If Me.Cmb_IVA.SelectedIndex > 0 Then

                        Aliquota = CInt(Split(Cmb_IVA.SelectedValue, "|")(1)) ' CInt(Me.Cmb_IVA.SelectedValue)

                        IVA_auto = Agro_Math.ArrotondaVal_2(Imponibile_Netto * Aliquota / 100)

                        'Select Case Aliquota
                        '    Case IVA_4, IVA_10, IVA_12, IVA_20, IVA_21
                        '        IVA_auto = (Imponibile_Netto * Aliquota) / 100
                        '    Case Else
                        '        IVA_auto = 0
                        'End Select
                    Else
                        IVA_auto = 0
                    End If

                    If Not Me.Chk_IVAmanuale.Checked Then
                        Importo = Imponibile_Netto + IVA_auto
                    Else
                        If Math.Abs(IVA_auto - IVA_manuale) > 0.02 Then
                            ' Messaggi.AgroMsgBox("L'IVA impostata può essere arrotondata, rispetto a quella proposta automaticamente, al massimo di 2 centesimi (in difetto o in eccesso)!" & vbCrLf & _
                            '"Impostare il valore corretto!",Page, , Me.upDati)
                            Messaggio &= DirectCast(GetLocalResourceObject("IvaArrotondabileAlMassimoDiDueCentesimiImpostareValoreCorretto"), String) & vbCrLf
                        End If
                        Importo = Imponibile_Netto + IVA_manuale
                    End If

                Else
                    Prezzo_Unitario_Netto = 0
                    Importo = 0
                    Prezzo_Unitario = 0
                    Sconto = 0
                    Maggiorazione = 0
                    Imponibile_Netto = 0
                    IVA_auto = 0
                End If

                Me.Txt_PrezzoUnitario.Text = Format(Prezzo_Unitario, "#,###,##0.00####")
                Me.Txt_PrezzoUnitario_Netto.Text = Format(Prezzo_Unitario_Netto, "##,###,##0.00####")
                Me.Txt_Imponibile.Text = Format(Imponibile, "#,###,##0.00")
                Me.Txt_Imponibile_Netto.Text = Format(Imponibile_Netto, "##,###,##0.00")
                Me.Txt_Importo.Text = Format(Importo, "##,###,##0.00")
                Me.Txt_IVA.Text = Format(IVA_auto, "#,###,##0.00")

        End Select

        '###########################################################


        If Me.Opt_Sconto.Checked Then
            Sconto = Variazione
        ElseIf Me.Opt_Maggiorazione.Checked Then
            Maggiorazione = Variazione
        End If

        Me.Txt_Sconto_Calcolato.Text = Format(Sconto, "##,###,##0.00###")
        Me.Txt_Maggiorazione_Calcolato.Text = Format(Maggiorazione, "##,###,##0.00###")

        Return Messaggio


    End Function

    '###############################################################################
    Private Function UsaNuoviArrotondamenti(ByRef objParametriServer As AgronicaCoreParametri) As Boolean
        Dim objConfigSitiR As New Configurazione_Siti_R
        Dim val As String = objConfigSitiR.Recupera_Valore_ByChiave(Enum_SiteRedirector.Sito_AgronicaStampe_2010, "Flag_Stampe_Nuovi_Arrotondamenti", objParametriServer)

        If val = "true" Then
            Return True
        Else
            Return False
        End If

    End Function

    '###############################################################################
    Private Sub Setta_Label_Nero()

        Me.lbl_categoria.ForeColor = AgroColor_Nero
        Me.Lbl_Cerca.ForeColor = AgroColor_Nero
        Me.lbl_descrizione.ForeColor = AgroColor_Nero
        Me.lbl_LottoInterno.ForeColor = AgroColor_Nero
        Me.lbl_LottoAccettazione.ForeColor = AgroColor_Nero
        Me.lbl_calibri.ForeColor = AgroColor_Nero
        Me.lbl_udm.ForeColor = AgroColor_Nero

    End Sub

    '###############################################################################
    Private Sub Gestisci_Visibilita_Cmb(ByVal elem_cod As Integer)

        lbl_LottoInterno.Visible = False
        cmb_Lotto.Visible = False
        Chk_LottoImpianto.Visible = False
        Lbl_Ordine.Visible = False
        Cmb_Ordini.Visible = False
        Chk_Contatto.Visible = False
        lbl_calibri.Visible = False
        cmb_Calibro.Visible = False
        Chk_ParametroQualitativo.Visible = False

        Select Case elem_cod

            Case SEMILAVORATI_VEGETALI, SEMILAVORATI_ANIMALI

                lbl_LottoInterno.Visible = True
                cmb_Lotto.Visible = True
                Chk_LottoImpianto.Visible = True
                lbl_calibri.Visible = True
                cmb_Calibro.Visible = True
                Chk_ParametroQualitativo.Visible = True

        End Select


    End Sub

    '###############################################################################
    Private Sub cmb_Categoria_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmb_Categoria.SelectedIndexChanged

        Cambio_Categoria()

    End Sub

    Private Sub Cambio_Categoria()

        'Me.Risorse.Visible = False
        Me.ImgBtn_NuovoProdotto.Visible = False

        If IsNothing(Me.cmb_Categoria) OrElse Me.cmb_Categoria.Items.Count = 0 Then
            Exit Sub
        End If

        Me.Txt_CercaProdotto.Text = ""

        Me.Txt_CercaLotto.Text = ""
        Me.Txt_CercaLotto.Visible = False
        Me.lbl_CercaLotto.Visible = False

        Me.Txt_CercaCodArticolo.Text = ""
        Me.Txt_CercaCodArticolo.Visible = False
        Me.Lbl_CercaCodArticolo.Visible = False

        Me.Txt_NumProdotti.Text = ""
        'Me.BtnInfo.Visible = False
        Me.BtnInfo_Concime.Visible = False
        Me.BtnInfo_fito.Visible = False

        Nascondi_DoseEtichetta()

        Me.cmb_Prodotti.Items.Clear()

        Me.cmb_Lotto.Items.Clear()
        Me.cmb_Lotto.Enabled = False

        Me.Cmb_Ordini.Items.Clear()
        Me.Cmb_Ordini.Enabled = False

        Me.cmb_Calibro.Items.Clear()
        Me.cmb_Calibro.Enabled = False

        Me.cmb_Udm.Items.Clear()
        Me.cmb_Udm.Enabled = False

        Setta_Label_Nero()

        Dim Num_Totale As Integer = 0

        Pulisci_Giacenze_Prezzo()
        Pulisci_Dettaglio_Fertilizzante()
        Dettaglio_Fertilizzante_Visibilita(False)
        Lotto_ElemCod_Visibilita()

        If Me.cmb_Categoria.SelectedValue <> "" Then

            Gestisci_Visibilita_Cmb(Me.cmb_Categoria.SelectedValue)

            ' Me.Lbl_Cerca.ForeColor = AgroColor_Rosso

            'Select Case Me.cmb_Categoria.SelectedValue
            '    Case SEMILAVORATI_VEGETALI
            '        Me.Txt_CercaLotto.ReadOnly = False
            '        Me.Txt_CercaProdotto.ReadOnly = False
            '        Me.Txt_CercaCodArticolo.ReadOnly = True
            '    Case SEMENTI
            '        If xCaricoScarico = CAU_CARICO Then
            '            Me.Txt_CercaLotto.ReadOnly = True
            '            Me.Txt_CercaProdotto.ReadOnly = False
            '            Me.Txt_CercaCodArticolo.ReadOnly = False
            '        Else
            '            Me.Txt_CercaLotto.ReadOnly = True
            '            Me.Txt_CercaProdotto.ReadOnly = False
            '            Me.Txt_CercaCodArticolo.ReadOnly = True
            '        End If
            '    Case ALTRI_BENI
            '        Me.Txt_CercaLotto.ReadOnly = True
            '        Me.Txt_CercaProdotto.ReadOnly = True
            '        Me.Txt_CercaCodArticolo.ReadOnly = True
            '    Case Else
            '        Me.Txt_CercaLotto.ReadOnly = True
            '        Me.Txt_CercaProdotto.ReadOnly = False
            '        Me.Txt_CercaCodArticolo.ReadOnly = True
            'End Select


            Select Case Me.cmb_Categoria.SelectedValue

                'Case ZOO_CONSISTENZA

                '    Me.Txt_BeniStrumentali.Visible = False
                '    Me.cmb_Prodotti.Visible = True

                '    Me.ImgBtn_CercaProdotti.Visible = True
                '    Me.ImgBtn_CercaProdotti.Enabled = True
                '    Me.Lbl_Cerca.Visible = True

                '    Me.Pannello_Magazzini.Visible = True
                '    Me.lbl_magazzini.Visible = True

                '    Me.Chk_MovimentoMag.Text = "Mov Stalla"
                '    Me.Chk_MovimentoMag.Checked = False

                Case MACCHINE, ALTRI_BENI

                    Me.Txt_BeniStrumentali.Visible = True
                    Me.cmb_Prodotti.Visible = False

                    Me.ImgBtn_CercaProdotti.Visible = False
                    Me.ImgBtn_CercaProdotti.Enabled = False
                    Me.Lbl_Cerca.Visible = False

                    Me.Pannello_Magazzini.Visible = False
                    Me.lbl_magazzini.Visible = False
                    Me.Cmb_Destinazione.SelectedIndex = -1
                    Me.Cmb_Provenienza.SelectedIndex = -1

                    Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                    Me.Chk_MovimentoMag.Checked = False

                    'CaricaCombo_Udm(Server, Session, Page, Me.cmb_Udm)

                    'il cau_mov è quello del carico, così va a leggere in CategorieXUnitaMisura
                    AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                                                Me.cmb_Udm,
                                                Num_Totale,
                                                "",
                                                0,
                                                0,
                                                CAU_CARICO,
                                                0,
                                                False,
                                                0,
                                                0,
                                                True, "", "",
                                                "", "", "",
                                                objParametri_Server, objParametri_Utenti)

                    RipristinaUdmSelezionataInPrecedenza()

                    Me.cmb_Udm.Enabled = True

                    Me.Txt_NumProdotti.Text = ""

                Case SERVIZI

                    Me.Txt_BeniStrumentali.Visible = False
                    Me.cmb_Prodotti.Visible = True

                    '----------
                    'COM: CaricaCombo_Servizi(Server, Session, Page, Me.cmb_Prodotti)
                    AgronicaCoreUtility.CaricaListControl.CaricaServizi(Me.cmb_Prodotti, False, "", "", "", "", "", objParametri_Server)
                    '----------

                    Me.ImgBtn_CercaProdotti.Visible = False
                    Me.ImgBtn_CercaProdotti.Enabled = False
                    Me.Lbl_Cerca.Visible = False

                    Me.Pannello_Magazzini.Visible = False
                    Me.lbl_magazzini.Visible = False
                    Me.Cmb_Destinazione.SelectedIndex = -1
                    Me.Cmb_Provenienza.SelectedIndex = -1

                    Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                    Me.Chk_MovimentoMag.Checked = False

                    'CaricaCombo_Udm(Server, Session, Page, Me.cmb_Udm)

                    'il cau_mov è quello del carico, così va a leggere in CategorieXUnitaMisura
                    AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                            Me.cmb_Udm,
                            Num_Totale,
                            "",
                            0,
                            0,
                            CAU_CARICO,
                            0,
                            False,
                            0,
                            0,
                            True, "", "",
                            "", "", "", objParametri_Server, objParametri_Utenti)
                    RipristinaUdmSelezionataInPrecedenza()

                    Me.cmb_Udm.Enabled = True

                Case Else

                    If xCaricoScarico = CAU_CARICO Then
                        If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("MagazzinoDiDestinazioneNonSelezionato"), String), Page, , Me.upDati)
                        End If

                    ElseIf xCaricoScarico = CAU_SCARICO Then
                        If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("MagazzinoDiProvenienzaNonSelezionato"), String), Page, , Me.upDati)
                        End If

                    ElseIf xCaricoScarico = CAU_TRASFERIMENTO Then

                        If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                            Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiProvenienzaNonSelezionato"), String) & vbCrLf
                        End If
                        If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                            Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiDestinazioneNonSelezionato"), String) & vbCrLf
                        End If
                        If Me.Cmb_Provenienza.SelectedIndex <> 0 AndAlso Me.Cmb_Destinazione.SelectedIndex <> 0 Then
                            If Me.Cmb_Provenienza.SelectedValue <> "" AndAlso Me.Cmb_Destinazione.SelectedValue <> "" Then
                                If Me.Cmb_Provenienza.SelectedValue = Me.Cmb_Destinazione.SelectedValue Then
                                    Messaggio &= AgronicaAgenda_2010.MagazziniDiProvenienzaEDestinazioneCoincidono & vbCrLf
                                End If
                            End If
                        End If

                        If Messaggio <> "" Then
                            Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                        End If

                    End If

                    Me.Txt_BeniStrumentali.Visible = False
                    Me.cmb_Prodotti.Visible = True

                    Me.ImgBtn_CercaProdotti.Visible = True
                    Me.ImgBtn_CercaProdotti.Enabled = True
                    Me.Lbl_Cerca.Visible = True

                    Me.Pannello_Magazzini.Visible = True
                    Me.lbl_magazzini.Visible = True

                    'ATTENZIONE!!!!!!!
                    'Il prodotto è materiale --> verifico se il dettaglio sia stato già movimentato da bolla precedente
                    Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                    Me.Chk_MovimentoMag.Checked = True

                    cmb_PUARegolamenti.Visible = False
                    Lbl_PUARegolamenti.Visible = False

                    Select Case Me.cmb_Categoria.SelectedValue

                        Case SEMENTI, ALTRE_MATERIE,
                                SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI

                            Me.Txt_CercaCodArticolo.Visible = True
                            Me.Lbl_CercaCodArticolo.Visible = True

                            If Me.cmb_Categoria.SelectedValue = SEMILAVORATI_VEGETALI Then
                                Me.Txt_CercaLotto.Visible = True
                                Me.lbl_CercaLotto.Visible = True
                            End If

                            'Me.Risorse.Visible = True
                            Me.ImgBtn_NuovoProdotto.Visible = True

                        Case FORMULATI
                            ' Me.BtnInfo.Visible = True
                            Me.BtnInfo_fito.Visible = True

                        Case FERTILIZZANTI

                            'Me.BtnInfo_Concime.Visible = True

                            ' aggiungo il filtro solo nello scarico
                            If xCaricoScarico = CAU_CARICO Then
                                cmb_PUARegolamenti.Visible = True
                                Lbl_PUARegolamenti.Visible = True

                                ' carico la combo se non l'ho già caricata
                                If cmb_PUARegolamenti.Items.Count = 0 Then
                                    CaricaCombo_Regolamenti()
                                End If

                                Dettaglio_Fertilizzante_Visibilita(True)
                            End If

                        Case CARBURANTI

                            'Me.Risorse.Visible = True
                            Me.ImgBtn_NuovoProdotto.Visible = True

                        Case Else

                    End Select


            End Select


            'Costruisco il link
            Me.TxtProdotto.Text = "e=" &
                      Stringa_Codifica(Me.cmb_Categoria.SelectedValue, AgroKey_EncoderDecoder, Server) &
                      "&l=" &
                      Stringa_Codifica(Qs_Lav_Cod, AgroKey_EncoderDecoder, Server) &
                      "&k=" &
                      Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                      "&o=" &
                      Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                      "&d=" &
                      Stringa_Codifica(Qs_DataSelezionata, AgroKey_EncoderDecoder, Server) &
                      "&c=" &
                      Stringa_Codifica("C", AgroKey_EncoderDecoder, Server) &
                      "&a=" &
                      Stringa_Codifica(Qs_IdAgenda, AgroKey_EncoderDecoder, Server) &
                      "&r=" &
                      Stringa_Codifica("FormProdotto.aspx", AgroKey_EncoderDecoder, Server) &
                      "&orig=" &
                      Stringa_Codifica("FormProdotto.aspx", AgroKey_EncoderDecoder, Server) &
                      "&mode=" & Request.QueryString("mode").ToString

        Else
            '  Me.lbl_categoria.ForeColor = AgroColor_Rosso
        End If

        If Not IsNothing(Me.cmb_Categoria) AndAlso Me.cmb_Categoria.Items.Count > 0 AndAlso IsNumeric(Me.cmb_Categoria.SelectedValue) Then
            If Me.cmb_Categoria.SelectedValue = SEMILAVORATI_VEGETALI AndAlso xCaricoScarico = CAU_SCARICO Then
                Chk_LottoImpianto.Checked = True
                Chk_LottoImpianto.Enabled = True
                Chk_ParametroQualitativo.Checked = True
                Chk_ParametroQualitativo.Enabled = True
                cmb_Lotto.Enabled = False
                cmb_Calibro.Enabled = False
            ElseIf Me.cmb_Categoria.SelectedValue = TRASFORMATI_VEGETALI AndAlso xCaricoScarico = CAU_SCARICO Then
                Chk_LottoImpianto.Checked = False
                Chk_LottoImpianto.Enabled = False
                Chk_ParametroQualitativo.Checked = True
                Chk_ParametroQualitativo.Enabled = True
                cmb_Lotto.Enabled = False
                cmb_Calibro.Enabled = False
            Else
                Chk_LottoImpianto.Checked = False
                Chk_LottoImpianto.Enabled = False
                Chk_ParametroQualitativo.Checked = False
                Chk_ParametroQualitativo.Enabled = False
                cmb_Lotto.Enabled = True
                cmb_Calibro.Enabled = True
            End If
        End If


    End Sub

    Private Sub Lotto_ElemCod_Visibilita()

        Try

            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim impGestLottoElemCod = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(xPiva, New List(Of Integer)({xSa_Cod}),
                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, If(cmb_Categoria.SelectedValue = "", 0, cmb_Categoria.SelectedValue), 1,
                objParametri_Utenti, objParametri_Server))

            Dim lottoAbilitato = If(impGestLottoElemCod <> 0, True, False) 'Facendo così perdo l'informazione sul fatto che il lotto per la categoria sia facoltativo/obbligatorio

            If xCaricoScarico = CAU_CARICO Then

                'lbl_LottoInterno.Visible = lottoAbilitato

                'Me.cmb_Lotto.Items.Clear()
                'Me.cmb_Lotto.Enabled = False
                'cmb_Lotto.Visible = False

                'Chk_LottoImpianto.Visible = False

                cmb_LottoAccettazione.Visible = False

                lbl_LottoAccettazione.Visible = lottoAbilitato
                Txt_LottoAccettazione.Text = ""
                Txt_LottoAccettazione.Visible = lottoAbilitato

                'Txt_CercaLotto.Text = ""
                'Txt_CercaLotto.Visible = False
                'lbl_CercaLotto.Visible = False

            Else
                'Scarico, Trasferimento → ergo Giacenza

                'lbl_LottoInterno.Visible = lottoAbilitato

                'Me.cmb_Lotto.Items.Clear()
                'Me.cmb_Lotto.Enabled = lottoAbilitato
                'cmb_Lotto.Visible = lottoAbilitato

                'Chk_LottoImpianto.Visible = lottoAbilitato

                Txt_LottoAccettazione.Visible = False

                lbl_LottoAccettazione.Visible = lottoAbilitato
                cmb_LottoAccettazione.Items.Clear()
                cmb_LottoAccettazione.Enabled = False
                cmb_LottoAccettazione.Visible = lottoAbilitato

                'Txt_CercaLotto.Text = ""
                'Txt_CercaLotto.Visible = lottoAbilitato
                'lbl_CercaLotto.Visible = lottoAbilitato

            End If

        Catch ex As Exception
            'TODO Gestione Errori
            'r.RispostaOK = False
            'r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

    End Sub

    Private Sub Lotto_ElemCod_Caricamento()

        Try

            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim impGestLottoElemCod = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(xPiva, New List(Of Integer)({xSa_Cod}),
                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, If(cmb_Categoria.SelectedValue = "", 0, cmb_Categoria.SelectedValue), 1,
                objParametri_Utenti, objParametri_Server))

            If impGestLottoElemCod <> 0 Then

                If xCaricoScarico = CAU_CARICO Then

                    Me.Txt_LottoAccettazione.Text = ""

                Else

                    Dim temp_ProCod, temp_MatCod As Integer

                    'Il valore della cmb_prodotti può essere:
                    '- un intero positivo per indicare un pro_cod
                    '- un intero negativo per indicare un mat_cod
                    '- una stringa "pro_cod|udm" per alcune categorie di banche dati
                    'Se la funzione split non trova il carattere separatore nel valore, crea un array con un solo elemento,
                    'di conseguenza l'if riesce a gestire sia il primo che il terzo caso
                    If cmb_Prodotti.SelectedItem.Value.Split("|")(0) > 0 Then
                        temp_ProCod = cmb_Prodotti.SelectedItem.Value.Split("|")(0)
                    Else
                        temp_MatCod = -cmb_Prodotti.SelectedItem.Value
                    End If

                    CaricaListControl.MateriePrime_LottoAccettazione(Me.cmb_LottoAccettazione,
                                                           0,
                                                           xPiva,
                                                           xSa_Cod,
                                                           xFabbricato_Cod,
                                                            xElem_Cod,
                                                            temp_ProCod,
                                                           temp_MatCod,
                                                           xCod_Progetto,
                                                           True, "", "-1",
                                                           Me.Txt_CercaLotto.Text,
                                                           CDate(Txt_DataMovimento.Text),
                                                           "", "", objParametri_Server, objParametri_Utenti)

                    Me.cmb_LottoAccettazione.Enabled = True

                    If Me.cmb_LottoAccettazione.Items.Count = 2 Then
                        'se uno lo preseleziono
                        Me.cmb_LottoAccettazione.SelectedIndex = 1
                        Cambio_LottoAccettazione()
                    End If

                End If

            End If

        Catch ex As Exception
            'TODO Gestione Errori
            'r.RispostaOK = False
            'r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try


    End Sub

    Private Sub Lotto_ElemCod_Azzera()

        Try

            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim impGestLottoElemCod = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(xPiva, New List(Of Integer)({xSa_Cod}),
                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, If(cmb_Categoria.SelectedValue = "", 0, cmb_Categoria.SelectedValue), 1,
                objParametri_Utenti, objParametri_Server))

            If impGestLottoElemCod <> 0 Then
                If xCaricoScarico = CAU_CARICO Then

                    Txt_LottoAccettazione.Text = ""

                Else

                    cmb_LottoAccettazione.Items.Clear()

                End If
            End If


        Catch ex As Exception

        End Try

    End Sub

    Private Sub Lotto_ElemCod_RipristinaDettaglio(ByVal elem_cod As Integer, ByVal lotto As String, ByVal temp_ProCod As Integer, ByVal temp_MatCod As Integer, ByVal solaLettura As Boolean)

        Try

            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim impGestLottoElemCod = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(xPiva, New List(Of Integer)({xSa_Cod}),
                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, elem_cod, 1,
                objParametri_Utenti, objParametri_Server))

            If impGestLottoElemCod <> 0 Then

                lbl_LottoAccettazione.Visible = True

                If xCaricoScarico = CAU_CARICO Then

                    cmb_LottoAccettazione.Visible = False

                    Txt_LottoAccettazione.Text = lotto
                    Txt_LottoAccettazione.ReadOnly = solaLettura
                    Txt_LottoAccettazione.Visible = True

                Else

                    Txt_LottoAccettazione.Visible = False

                    CaricaListControl.MateriePrime_LottoAccettazione(cmb_LottoAccettazione,
                                                           0,
                                                           xPiva,
                                                           xSa_Cod,
                                                           xFabbricato_Cod,
                                                            elem_cod,
                                                            temp_ProCod,
                                                           temp_MatCod,
                                                           xCod_Progetto,
                                                           True, "", "-1",
                                                           Me.Txt_CercaLotto.Text,
                                                           CDate(Txt_DataMovimento.Text),
                                                           "", "", objParametri_Server, objParametri_Utenti)

                    cmb_LottoAccettazione.SelectedIndex = cmb_LottoAccettazione.Items.IndexOf(cmb_LottoAccettazione.Items.FindByValue(lotto))
                    cmb_LottoAccettazione.Enabled = Not solaLettura
                    cmb_LottoAccettazione.Visible = True

                End If

            Else

                Txt_LottoAccettazione.Visible = False
                cmb_LottoAccettazione.Visible = False
                lbl_LottoAccettazione.Visible = False

            End If


        Catch ex As Exception

        End Try

    End Sub

    Private Sub Chk_LottoImpianto_CheckedChanged(sender As Object, e As System.EventArgs) Handles Chk_LottoImpianto.CheckedChanged

        Cambio_Prodotto()
        If Chk_LottoImpianto.Checked Then
            cmb_Lotto.Enabled = False
            lbl_LottoInterno.ForeColor = Color.Black
            ''lbl_LottoAccettazione.ForeColor = Color.Red
            Lbl_Inserisci_Carico.Text = DirectCast(GetLocalResourceObject("InserisciDettagliCaricoInBaseAiParametri"), String)

        Else
            cmb_Lotto.Enabled = True
            'lbl_LottoInterno.ForeColor = Color.Red
            lbl_LottoAccettazione.ForeColor = Color.Black
            Lbl_Inserisci_Carico.Text = DirectCast(GetLocalResourceObject("InserisciIlDettaglioNelRiepilogo"), String)
        End If

    End Sub


    Private Sub Chk_ParametroQualitativo_CheckedChanged(sender As Object, e As System.EventArgs) Handles Chk_ParametroQualitativo.CheckedChanged

        'Cambio_Prodotto()
        If Chk_ParametroQualitativo.Checked Then

            Me.cmb_Calibro.Items.Clear()
            Me.cmb_Calibro.Items.Add(New ListItem("", ""))
            Me.cmb_Calibro.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Indefinito"), String), 0))
            cmb_Calibro.SelectedIndex = 1
            cambio_Calibro()

            cmb_Calibro.Enabled = False
            lbl_calibri.ForeColor = Color.Black
            ''lbl_udm.ForeColor = Color.Red
            Lbl_Inserisci_Carico.Text = DirectCast(GetLocalResourceObject("InserisciDettagliCaricoInBaseAiParametri"), String)
        Else
            Cambio_LottoAccettazione()

            cmb_Calibro.Enabled = True
            'lbl_calibri.ForeColor = Color.Red
            lbl_udm.ForeColor = Color.Black
            Lbl_Inserisci_Carico.Text = DirectCast(GetLocalResourceObject("InserisciIlDettaglioNelRiepilogo"), String)

        End If

    End Sub

    '###############################################################################
    Private Sub cmb_Prodotti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmb_Prodotti.SelectedIndexChanged

        Cambio_Prodotto()


    End Sub

    Private Sub Cambio_Prodotto()

        If IsNothing(Me.cmb_Prodotti) OrElse Me.cmb_Prodotti.Items.Count = 0 Then
            Exit Sub
        End If

        Me.cmb_Lotto.Items.Clear()
        Me.cmb_Lotto.Enabled = False

        Me.Cmb_Ordini.Items.Clear()
        Me.Cmb_Ordini.Enabled = False

        Me.cmb_Calibro.Items.Clear()
        Me.cmb_Calibro.Enabled = False

        Me.cmb_Udm.Items.Clear()
        Me.cmb_Udm.Enabled = False

        'Me.BtnInfo.Visible = False
        Me.BtnInfo_Concime.Visible = False
        Me.BtnInfo_fito.Visible = False

        Nascondi_DoseEtichetta()

        Setta_Label_Nero()

        Pulisci_Giacenze_Prezzo()

        Pulisci_Dettaglio_Fertilizzante()
        Dettaglio_Fertilizzante_Visibilita(False)

        Dim dtProdotti As DataTable = Nothing
        If Session("dtCmbProdotti") IsNot Nothing Then
            dtProdotti = Session("dtCmbProdotti")
        End If


        If Me.cmb_Categoria.SelectedValue <> ALTRI_BENI AndAlso
                Me.cmb_Categoria.SelectedValue <> SERVIZI Then

            If xCaricoScarico = CAU_CARICO Then
                If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                    Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("MagazzinoNonSelezionato"), String), Page, , Me.upDati)
                End If

            ElseIf xCaricoScarico = CAU_SCARICO Then
                If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                    Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("MagazzinoNonSelezionato"), String), Page, , Me.upDati)
                End If

            ElseIf xCaricoScarico = CAU_TRASFERIMENTO Then

                If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                    Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiProvenienzaNonSelezionato"), String) & vbCrLf
                End If
                If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                    Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiDestinazioneNonSelezionato"), String) & vbCrLf
                End If
                If Me.Cmb_Provenienza.SelectedIndex <> 0 AndAlso Me.Cmb_Destinazione.SelectedIndex <> 0 Then
                    If Me.Cmb_Provenienza.SelectedValue = Me.Cmb_Destinazione.SelectedValue Then
                        Messaggio &= AgronicaAgenda_2010.MagazziniDiProvenienzaEDestinazioneCoincidono & vbCrLf
                    End If
                End If

                If Messaggio <> "" Then
                    Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                End If

            End If

        End If

        Recupera_ChiaveMagazzino()


        If Me.cmb_Categoria.SelectedItem.Text <> "" AndAlso
            Me.cmb_Prodotti.SelectedItem.Text <> "" Then

            Dim Cau_Mov As Integer
            Dim Num_Totale As Integer = 0

            If xCaricoScarico = enum_Agenda_Causali.SCARICO OrElse
                xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                'scarico o trasferimento
                Cau_Mov = enum_Agenda_Causali.SCARICO
            Else
                'carico
                Cau_Mov = enum_Agenda_Causali.CARICO
            End If


            '--------------------------------------
            Dim strElemCodUdmCod As String
            Dim ArrayElemCodUdmCod As String()
            Dim Elem_Cod, Udm_Cod As Integer

            Dim Dt_Impost As DataTable
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim strElem_Cod As String = ""

            Dt_Impost = objImpost.Leggi2(1,
                                        Session("ASG_Utente_Username"),
                                        0,
                                        "", "",
                                        objParametri_Utenti)

            Dim DrFiltroElemCodUdmCod As DataRow() = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT)

            If DrFiltroElemCodUdmCod IsNot Nothing AndAlso DrFiltroElemCodUdmCod.Length > 0 Then
                strElemCodUdmCod = DrFiltroElemCodUdmCod(0).Item("Impostazione_Valore_1")
                ArrayElemCodUdmCod = Split(strElemCodUdmCod, "|")
            End If

            '----------------------------------------------
            Dim RegolamentoCod As Integer = 0
            Dim TipoRegolamento As Integer = 0
            If cmb_PUARegolamenti.Visible AndAlso cmb_PUARegolamenti.Items.Count > 0 Then
                RegolamentoCod = cmb_PUARegolamenti.SelectedItem.Value.Split(SEP_PuaReg)(0)
                TipoRegolamento = cmb_PUARegolamenti.SelectedValue.Split(SEP_PuaReg)(1)
            End If
            '----------------------------------------------

            Select Case Me.cmb_Categoria.SelectedItem.Value

                Case SEMILAVORATI_VEGETALI

                    '  Me.lbl_LottoInterno.ForeColor = AgroColor_Rosso

                    Me.cmb_Lotto.Items.Clear()
                    Me.cmb_Lotto.Enabled = True

                    If Cau_Mov <> enum_Agenda_Causali.CARICO Then

                        '#######################################
                        '####### SCARICO O TRASFERIMENTO #######
                        '#######################################

                        If Chk_LottoImpianto.Checked Then
                            'caso nuovo senza lotto impianto, carico lotto magazzino
                            Me.cmb_Lotto.Enabled = False
                            Cambio_LottoInterno()

                        Else


                            'caso vecchio
                            AgronicaCoreUtility.CaricaListControl.Semilavorati_LottoInterno(Me.cmb_Lotto,
                                                Num_Totale,
                                                xPiva,
                                                xSa_Cod,
                                                xFabbricato_Cod,
                                                -Me.cmb_Prodotti.SelectedItem.Value,
                                               True, "", "",
                                               "",
                                               CDate(Me.Txt_DataMovimento.Text),
                                               CStr(Cau_Mov),
                                               "", "", objParametri_Server, objParametri_Utenti)

                        End If


                    Else

                        '#######################################
                        '############# CARICO ##################
                        '#######################################


                        '1) CASO DI LETTURA ---> DEVO DISTINGUERE IL CASO A DAL CASO B

                        '2) CASO DI SCRITTURA / MODIFICA ---> SEMILAVORATI CON COD_PROGETTO = 0


                        If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Lettura Then

                            'CASO DI LETTURA

                            'CASO A
                            'STO CONSULTANDO IN INFO UN CASO DI CARICO RIFERITO DI SEMILAVORATI
                            '( i semilavorati sono stati caricati con un'operazione di raccolta
                            'e hanno il cod_progetto valorizzato, ovvero l'impianto)
                            'AgronicaCoreUtility.CaricaListControl.Semilavorati_LottoInterno(Me.cmb_Lotto, _
                            '                    Num_Totale, _
                            '                    xPiva, _
                            '                    xSa_Cod, _
                            '                    xFabbricato_Cod, _
                            '                    -Me.cmb_Prodotti.SelectedItem.Value, _
                            '                   True, "", "", _
                            '                    Me.Txt_CercaLotto.Text, CDate(Me.Txt_DataMovimento.Text), "", "", objParametri_Server)
                            'CASO B
                            'i semilavorati provengono da terzi
                            'hanno il cod_progetto =0, ovvero nessun impianto
                            Me.cmb_Lotto.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("DaTerzi"), String), 0))


                        Else

                            'CASO DI   SCRITTURA   O MODIFICA

                            'i semilavorati provengono da terzi
                            'hanno il cod_progetto =0, ovvero nessun impianto

                            Me.cmb_Lotto.Items.Add(New ListItem("", ""))
                            Me.cmb_Lotto.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("DaTerzi"), String), 0))

                        End If


                    End If


                    '----------------------------------------------------------


                    '--------------------------------------------------------
                    '---------------MODIFICA SEMINA LOTTO--------------------
                Case SEMENTI, TRASFORMATI_VEGETALI, ALTRE_MATERIE, TRASFORMATI_ANIMALI

                    If Cau_Mov = enum_Agenda_Causali.CARICO Then

                        Me.cmb_Udm.Items.Clear()
                        Me.cmb_Udm.Enabled = True

                        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Txt_DataMovimento.Text, AGRODATAFINE)

                        AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(
                                                 Me.cmb_Udm,
                                                 Num_Totale,
                                                 xPiva,
                                                 xSa_Cod,
                                                 xFabbricato_Cod,
                                                 Cau_Mov,
                                                 Me.cmb_Categoria.SelectedItem.Value,
                                                 True,
                                                 Me.cmb_Prodotti.SelectedItem.Value.Split("|")(0),
                                                 0,
                                                 True, "", "", "",
                                                 0, "", "", objParametri_Server, objParametri_Utenti)

                        objParametri_Server.ResettaFinestra()

                        Dim Flag_UdmDefault As Boolean = False
                        If Cau_Mov = enum_Agenda_Causali.CARICO Then
                            If ArrayElemCodUdmCod IsNot Nothing Then
                                For i = 0 To ArrayElemCodUdmCod.Length - 1
                                    Elem_Cod = ArrayElemCodUdmCod(i).Split("_")(0)
                                    Udm_Cod = ArrayElemCodUdmCod(i).Split("_")(1)
                                    If Elem_Cod = cmb_Categoria.SelectedItem.Value Then
                                        Flag_UdmDefault = True
                                        cmb_Udm.SelectedIndex = cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))
                                        Exit For
                                    End If
                                Next
                            End If
                            If Not Flag_UdmDefault Then
                                'non è configurata impostazione utente sul default per categoria magazzino
                                '-> imposto l'udm default in base alla tipologia semente
                                Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                Udm_Cod = objMP.RicavaUdmDefaultxTipoSemente_from_MatCod(xPiva,
                                                                                         Me.cmb_Categoria.SelectedItem.Value,
                                                                                         -Me.cmb_Prodotti.SelectedValue,
                                                                                         Nothing,
                                                                                         objParametri_Server)

                                cmb_Udm.SelectedIndex = cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))
                            End If
                        End If 'carico

                        RipristinaUdmSelezionataInPrecedenza()

                    End If

                    '----------------------------------------------------------
                    '----------------------------------------------------------

                Case Else

                    ' Me.lbl_udm.ForeColor = AgroColor_Rosso

                    Me.cmb_Udm.Items.Clear()
                    Me.cmb_Udm.Enabled = True

                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Txt_DataMovimento.Text, AGRODATAFINE)

                    AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(
                                             Me.cmb_Udm,
                                             Num_Totale,
                                             xPiva,
                                             xSa_Cod,
                                             xFabbricato_Cod,
                                             Cau_Mov,
                                             Me.cmb_Categoria.SelectedItem.Value,
                                             True,
                                             Me.cmb_Prodotti.SelectedItem.Value.Split("|")(0),
                                             0,
                                             True, "", "", "",
                                             RegolamentoCod, "", "", objParametri_Server, objParametri_Utenti)

                    objParametri_Server.ResettaFinestra()

                    If Cau_Mov = enum_Agenda_Causali.CARICO Then
                        If ArrayElemCodUdmCod IsNot Nothing Then
                            For i = 0 To ArrayElemCodUdmCod.Length - 1
                                Elem_Cod = ArrayElemCodUdmCod(i).Split("_")(0)
                                Udm_Cod = ArrayElemCodUdmCod(i).Split("_")(1)
                                If Elem_Cod = cmb_Categoria.SelectedItem.Value Then
                                    cmb_Udm.SelectedIndex =
                                        cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                    RipristinaUdmSelezionataInPrecedenza()

            End Select


            Select Case Me.cmb_Categoria.SelectedItem.Value

                Case FERTILIZZANTI

                    If Me.cmb_Prodotti.SelectedValue.Split("|")(0) > 0 Then
                        Me.BtnInfo_Concime.Visible = True
                    End If

                    If xCaricoScarico = CAU_CARICO AndAlso TipoRegolamento = enum_PUARegolamenti_Tipo.PUA Then
                        Dim ferCod = Me.cmb_Prodotti.SelectedValue.Split("|")(0)
                        Recupera_UdmFertilizzante(ferCod, RegolamentoCod)

                        'If RegolamentoCod = 78 AndAlso
                        If ferCod <> "" AndAlso ferCod <> "0" AndAlso
                           dtProdotti IsNot Nothing AndAlso dtProdotti.Rows.Count > 0 Then
                            'Imposta di default i titoli

                            Dim riga As DataRow = (From dt In dtProdotti.AsEnumerable Where CStr(dt.Item("Fer_Cod")) = ferCod).FirstOrDefault()

                            If riga IsNot Nothing Then
                                Txt_N.Text = CStr(riga.Item("N"))
                                Txt_P2O5.Text = CStr(riga.Item("P2O5"))
                                Txt_K2O.Text = CStr(riga.Item("K2O"))
                                Txt_Cu.Text = CStr(riga.Item("Cu"))
                            End If

                        End If
                    End If

                Case FORMULATI

                    If Me.cmb_Prodotti.SelectedValue.Split("|")(0) > 0 Then
                        Me.BtnInfo_fito.Visible = True
                    End If


                    Dim msg As String = ""

                    If IsDate(Qs_DataSelezionata) AndAlso
                       Not VerificaPermessoClasseToxPatentino(Qs_DataSelezionata, CInt(cmb_Prodotti.SelectedValue.Split("|")(0)), xPiva, objParametri_Server, msg) Then

                        Messaggi.AgroMsgBox(String.Format(AgronicaAgenda_2010.Cambio_Prodotto_RichiestoContattoConPatentinoValido, msg), Page, , Me.upDati)
                        Me.cmb_Prodotti.Items.Clear()
                        Me.cmb_Prodotti.Items.Add(New ListItem("", ""))
                        Exit Sub
                    End If


                    '------
                    If InStr(cmb_Prodotti.SelectedItem.Value, "|") = 0 Then
                        'No, ci mette troppo!
                        Recupera_DoseEtichetta(Me.cmb_Prodotti.SelectedValue.Split("|")(0))
                    Else
                        Dim objCore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                        Udm_Cod = objCore.Converti_Kg_L_from_UdmCod(cmb_Prodotti.SelectedItem.Value.Split("|")(1), "", "")

                        'Udm_Cod = cmb_Prodotti.SelectedItem.Value.Split("|")(1)
                        cmb_Udm.SelectedIndex =
                            cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))
                        cmb_Udm_SelectedIndexChanged(Me, Nothing)
                    End If

            End Select

            Carica_Ordini(xSa_Cod,
                                    xFabbricato_Cod,
                                    Me.cmb_Categoria.SelectedItem.Value,
                                    0)

        Else

            ' Me.lbl_descrizione.ForeColor = AgroColor_Rosso

            If Me.cmb_Categoria.SelectedItem.Value = FORMULATI Then
                'per i formulati visualizzo cmq l'icona
                'anziché andare nella scheda del profitosan andrà nella homepage
                'Me.BtnInfo.Visible = True
                Me.BtnInfo_fito.Visible = True
            End If
            If Me.cmb_Categoria.SelectedItem.Value = FERTILIZZANTI Then
                Me.BtnInfo_Concime.Visible = True
            End If

        End If



        'recupero le info su finescorta ecc ecc
        info_sul_prodotto.Text = ""
        '(29/11/2019 FEDE)
        Dim Stato_Cod As String = ""
        If cmb_Categoria.SelectedValue = FORMULATI Then
            'controllo stato centro
            Dim objIndC As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            Stato_Cod = objIndC.Stato_from_PivaSaCod(xPiva, xSa_Cod, objParametri_Server)
            Select Case Stato_Cod
                Case "FR"
                    info_sul_prodotto.Text = "<br><span style='color:#FF0000'>" & DirectCast(GetLocalResourceObject("ProdottoCommercializzatoInFrancia"), String) & " </span>"
                Case "BR"
                    info_sul_prodotto.Text = "<br><span style='color:#FF0000'>" & DirectCast(GetLocalResourceObject("ProdottoCommercializzatoInBrasile"), String) & " </span>"
                Case Else
                    If cmb_Prodotti.SelectedValue <> "" Then
                        FineScorta_e_altreInfo(cmb_Prodotti.SelectedValue.Split("|")(0))
                    End If
            End Select
        End If


        If Me.cmb_Lotto.Items.Count = 2 Then

            'se uno lo preseleziono
            Me.cmb_Lotto.SelectedIndex = 1
            Cambio_LottoInterno()

        End If

        If xCaricoScarico = CAU_CARICO AndAlso Me.cmb_Categoria.SelectedItem.Value = FERTILIZZANTI Then

            If cmb_PUARegolamenti.Visible AndAlso cmb_PUARegolamenti.Items.Count > 0 Then
                Dim TipoPuaRegolamento As Integer = cmb_PUARegolamenti.SelectedValue.Split(SEP_PuaReg)(1)

                If TipoPuaRegolamento = enum_PUARegolamenti_Tipo.PUA Then
                    Dettaglio_Fertilizzante_Visibilita(True)
                Else
                    Dettaglio_Fertilizzante_Visibilita(False)
                End If
            Else
                'se non ho i regolamenti devo renderli visibili cmq perché potrei essere in modifica
                Dettaglio_Fertilizzante_Visibilita(True)
            End If

            Dettaglio_Fertilizzante_Attivo(True)
        End If

    End Sub



    Private Sub FineScorta_e_altreInfo(ByVal fr_cod As Integer)

        Dim XML_Credenziali As XmlElement
        Dim StrCredenziali As String = ""
        Dim StrParametri As String = ""
        Dim Parametri As String = ""
        Dim strErr As String = ""
        Dim LastFr_Des As String = ""
        Dim i As Integer


        Try

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim objAgroWebConfig As New AgroWebConfig
            Dim XmlDoc As New XmlDocument


            objWs.NewWS(ObjDownloadWs,
                            objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci,
                            objParametri_Utenti)
            XmlDoc = New XmlDocument

            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                            StrCredenziali,
                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                            HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                            HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                            HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            'Dim grfi_Cod As Integer = 0
            objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                   StrParametri,
                                                   fr_cod,
                                                   strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)
            Dim dt As DataTable
            dt = ObjDownloadWs.Leggi_Formulati_Info_DT(Parametri, strErr)


            ''verifico se è tutto ok

            info_sul_prodotto.Text = "<span >"

            For i = 0 To dt.Rows.Count - 1
                If Not IsDBNull(dt.Rows(i).Item("Datasospensioneda")) Then
                    info_sul_prodotto.Text = info_sul_prodotto.Text & "<br><span style='color:#2B7000'> " & AgronicaAgenda_2010.SospesoDal_ & CDate(dt.Rows(i).Item("Datasospensioneda")).ToShortDateString & " </span>"
                End If
                If Not IsDBNull(dt.Rows(i).Item("Datasospensionea")) Then
                    info_sul_prodotto.Text = info_sul_prodotto.Text & "<span style='color:#2B7000'> " & AgronicaAgenda_2010.SospesoAl_ & CDate(dt.Rows(i).Item("Datasospensionea")).ToShortDateString & " </span>"
                End If
            Next


            If dt.Rows(0).Item("Revocato") = 1 Then
                info_sul_prodotto.Text = info_sul_prodotto.Text & "<br><span style='color:#FF0000'> " & AgronicaAgenda_2010.RevocatoIl_ & CDate(dt.Rows(0).Item("data_revo")).ToShortDateString & " </span>"
            End If

            If Not IsDBNull(dt.Rows(0).Item("Data_fine_comm")) Then
                info_sul_prodotto.Text = info_sul_prodotto.Text & "<br><span style='color:#FF0000'> " & AgronicaAgenda_2010.FineCommercializzazione_ & CDate(dt.Rows(0).Item("Data_fine_comm")).ToShortDateString & " </span>"
            End If

            If Not IsDBNull(dt.Rows(0).Item("Data_fine_usoscorte")) Then
                info_sul_prodotto.Text = info_sul_prodotto.Text & "<br><span style='color:#2B7000'> " & AgronicaAgenda_2010.FineUsoScorte_ & CDate(dt.Rows(0).Item("Data_fine_usoscorte")).ToShortDateString & " </span>"
            End If



            info_sul_prodotto.Text = info_sul_prodotto.Text & "</span>"

            ObjDownloadWs.Dispose()

        Catch ex As Exception

        End Try


    End Sub


    '############################################################################################################
    Private Sub cmb_Lotto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmb_Lotto.SelectedIndexChanged

        Cambio_LottoInterno()

    End Sub

    Private Sub Cambio_LottoInterno()

        ''If IsNothing(Me.cmb_Lotto) Or Me.cmb_Lotto.Items.Count = 0 Then
        ''    Exit Sub
        ''End If

        Me.Cmb_Ordini.Items.Clear()
        Me.Cmb_Ordini.Enabled = False

        Me.cmb_LottoAccettazione.Items.Clear()
        Me.cmb_LottoAccettazione.Enabled = False

        Me.cmb_Calibro.Items.Clear()
        Me.cmb_Calibro.Enabled = False

        Me.cmb_Udm.Items.Clear()
        Me.cmb_Udm.Enabled = False

        Setta_Label_Nero()

        'If Me.cmb_Lotto.SelectedIndex > 0 Then
        '    Me.lbl_LottoAccettazione.ForeColor = AgroColor_Rosso
        'Else
        '    Me.lbl_LottoInterno.ForeColor = AgroColor_Rosso
        'End If


        Pulisci_Giacenze_Prezzo()

        '------------

        Recupera_ChiaveProdotto()

        '----------------

        Recupera_ChiaveMagazzino()

        '---------------

        Carica_Ordini(xSa_Cod, xFabbricato_Cod, xElem_Cod, xCod_Progetto)

        If Me.Cmb_Ordini.Items.Count = 2 Then

            'se uno lo preseleziono
            Me.Cmb_Ordini.SelectedIndex = 1
            Cambio_Ordine()

        End If

        If Me.cmb_Calibro.Items.Count = 2 Then

            'se uno lo preseleziono
            Me.cmb_Calibro.SelectedIndex = 1
            cambio_Calibro()

        End If

    End Sub


    '############################################################################################################
    Private Sub Carica_Ordini(ByVal xSa_Cod As Integer, ByVal xFabbricato_Cod As Integer, ByVal xElem_Cod As Integer, ByVal xCod_Progetto As Integer)

        If xCaricoScarico = CAU_SCARICO OrElse xCaricoScarico = CAU_TRASFERIMENTO Then

            Cmb_Ordini.Items.Add(New ListItem(AgronicaAgenda_2010.Nessuno, "0"))

            Select Case Me.Cmb_Ordini.Items.Count

                Case 1

                    xFase_Cod = 0
                    Me.Cmb_Ordini.Enabled = False

                    Lotto_ElemCod_Caricamento()

                Case Is > 1

                    Me.Cmb_Ordini.Enabled = True

                Case Else

            End Select


        Else

            'carico

            '1) CASO DI LETTURA ---> DEVO DISTINGUERE IL CASO A DAL CASO B

            '2) CASO DI SCRITTURA / MODIFICA ---> SEMILAVORATI CON COD_PROGETTO = 0

            xFase_Cod = 0
            Me.Cmb_Ordini.Enabled = False


            If xElem_Cod = SEMILAVORATI_VEGETALI Then

                'i semilavorati provengono da terzi
                'hanno il cod_progetto =0, ovvero nessun impianto

                Me.cmb_Calibro.Enabled = True
                Me.cmb_Calibro.Items.Clear()
                Me.cmb_Calibro.Items.Add(New ListItem("", ""))
                Me.cmb_Calibro.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Indefinito"), String), 0))

            End If


        End If


    End Sub



    Private Sub Chk_Contatto_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Chk_Contatto.CheckedChanged


        Me.Cmb_Ordini.Items.Clear()
        Me.Cmb_Ordini.Enabled = False

        Me.cmb_LottoAccettazione.Items.Clear()
        Me.cmb_LottoAccettazione.Enabled = False

        Me.cmb_Calibro.Items.Clear()
        Me.cmb_Calibro.Enabled = False

        Me.cmb_Udm.Items.Clear()
        Me.cmb_Udm.Enabled = False


        Pulisci_Giacenze_Prezzo()

        '------------

        Recupera_ChiaveProdotto()

        Recupera_ChiaveMagazzino()

        Cmb_Ordini.Items.Add(New ListItem(AgronicaAgenda_2010.Nessuno, "0"))

        '-------------------
        'CaricaCombo_OrdiniFasi(Server, Session, Page, _
        '                       Me.Cmb_Ordini, xPiva, , , 0, "", xElem_Cod, 0, xMat_Cod, xCod_Progetto, , Me.Chk_Contatto.Checked)
        'AgronicaCoreUtility.CaricaListControl.OrdiniFasi(Me.Cmb_Ordini, _
        '                                True, _
        '                                AgronicaAgenda_2010.Nessuno, _
        '                                "0", _
        '                                xPiva, _
        '                                0, _
        '                                0, _
        '                                0, _
        '                                "", _
        '                                xElem_Cod, _
        '                                0, _
        '                                xMat_Cod, _
        '                                xCod_Progetto, _
        '                                0, _
        '                                Me.Chk_Contatto.Checked, _
        '                                 "", _
        '                                 "", _
        '                                 objParametri_Server _
        '                                )
        ''----------------

    End Sub

    Private Sub Cmb_Ordini_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Ordini.SelectedIndexChanged

        Cambio_Ordine()

    End Sub

    Private Sub Cambio_Ordine()

        If IsNothing(Me.Cmb_Ordini) OrElse Me.Cmb_Ordini.Items.Count = 0 Then
            Exit Sub
        End If

        Me.cmb_Calibro.Items.Clear()
        Me.cmb_Calibro.Enabled = False

        Pulisci_Giacenze_Prezzo()

        '------------

        Recupera_ChiaveProdotto()


        Recupera_ChiaveMagazzino()

        Lotto_ElemCod_Caricamento()

    End Sub



    '############################################################################################################
    Private Sub cmb_LottoAccettazione_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmb_LottoAccettazione.SelectedIndexChanged

        Cambio_LottoAccettazione()

    End Sub

    Private Sub Cambio_LottoAccettazione()

        If IsNothing(Me.cmb_LottoAccettazione) OrElse Me.cmb_LottoAccettazione.Items.Count = 0 Then
            Exit Sub
        End If

        Me.cmb_Calibro.Items.Clear()
        Me.cmb_Calibro.Enabled = False

        Dim Num_Totale As Integer = 0

        Me.cmb_Udm.Items.Clear()
        Me.cmb_Udm.Enabled = False

        Setta_Label_Nero()

        Pulisci_Giacenze_Prezzo()

        '------------

        Recupera_ChiaveProdotto()

        Recupera_ChiaveMagazzino()

        If xElem_Cod = SEMILAVORATI_VEGETALI OrElse xElem_Cod = TRASFORMATI_VEGETALI Then

            'If Me.cmb_LottoAccettazione.SelectedIndex > 0 Then
            '    Me.lbl_calibri.ForeColor = AgroColor_Rosso
            'Else
            '    Me.lbl_LottoAccettazione.ForeColor = AgroColor_Rosso
            'End If

            Me.cmb_Calibro.Enabled = True
        End If

        Select Case xElem_Cod

            Case SEMILAVORATI_VEGETALI

                If Chk_ParametroQualitativo.Checked Then

                    Me.cmb_Calibro.Items.Clear()
                    Me.cmb_Calibro.Items.Add(New ListItem("", ""))
                    Me.cmb_Calibro.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Indefinito"), String), 0))
                    cmb_Calibro.SelectedIndex = 1
                    cambio_Calibro()

                Else

                    AgronicaCoreUtility.CaricaListControl.Semilavorati_ParametriQualitativi(
                                                           Me.cmb_Calibro,
                                                           Num_Totale,
                                                           xPiva,
                                                           xSa_Cod,
                                                           xFabbricato_Cod,
                                                           xMat_Cod,
                                                           xCod_Progetto,
                                                           xLotto,
                                                           True, "", "",
                                                           CDate(Txt_DataMovimento.Text),
                                                           "", "", objParametri_Server, objParametri_Utenti)

                    If Me.cmb_Calibro.Items.Count = 2 Then
                        'se uno lo preseleziono
                        Me.cmb_Calibro.SelectedIndex = 1
                        cambio_Calibro()

                    End If

                End If

            Case TRASFORMATI_VEGETALI

                If Chk_ParametroQualitativo.Checked Then

                    Me.cmb_Calibro.Items.Clear()
                    Me.cmb_Calibro.Items.Add(New ListItem("", ""))
                    Me.cmb_Calibro.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Indefinito"), String), 0))
                    cmb_Calibro.SelectedIndex = 1
                    cambio_Calibro()

                Else

                    AgronicaCoreUtility.CaricaListControl.Trasformati_ParametriQualitativi(
                                                           Me.cmb_Calibro,
                                                           Num_Totale,
                                                           xPiva,
                                                           xSa_Cod,
                                                           xFabbricato_Cod,
                                                           xMat_Cod,
                                                           xLotto,
                                                           True, "", "",
                                                           CDate(Txt_DataMovimento.Text),
                                                           "", "", objParametri_Server, objParametri_Utenti)

                    If Me.cmb_Calibro.Items.Count = 2 Then
                        'se uno lo preseleziono
                        Me.cmb_Calibro.SelectedIndex = 1
                        cambio_Calibro()
                    ElseIf Me.cmb_Calibro.Items.Count > 2 Then
                        Me.cmb_Calibro.Visible = True
                        Me.lbl_calibri.Visible = True
                    End If

                End If


            Case Else
                'le altre categorie di materie prime senza parametri qualitativi

                ' Me.lbl_udm.ForeColor = AgroColor_Rosso

                Me.cmb_Udm.Items.Clear()
                Me.cmb_Udm.Enabled = True

                AgronicaCoreUtility.CaricaListControl.Udm_Optimize2(
                                         Me.cmb_Udm,
                                         Num_Totale,
                                         xPiva,
                                         xSa_Cod,
                                         xFabbricato_Cod,
                                         CAU_SCARICO,
                                         Me.cmb_Categoria.SelectedItem.Value,
                                         True,
                                          Me.cmb_Prodotti.SelectedItem.Value,
                                        0,
                                         xLotto,
                                        True, "", "",
                                        "", "", "", objParametri_Server, objParametri_Utenti)


                RipristinaUdmSelezionataInPrecedenza()

                If Me.cmb_Udm.Items.Count = 2 Then

                    'se uno lo preseleziono
                    Me.cmb_Udm.SelectedIndex = 1
                    Cambio_Udm()

                End If

        End Select






    End Sub



    '###############################################################################
    Private Sub cmb_Calibro_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmb_Calibro.SelectedIndexChanged

        cambio_Calibro()

    End Sub

    Private Sub cambio_Calibro()

        If IsNothing(Me.cmb_Calibro) Then
            Exit Sub
        End If

        Dim Num_Totale As Integer = 0

        Dim Cau_Mov As Integer

        Me.cmb_Udm.Items.Clear()
        Me.cmb_Udm.Enabled = True

        Setta_Label_Nero()

        'If Chk_ParametroQualitativo.Checked Then
        '    Me.lbl_udm.ForeColor = AgroColor_Rosso
        '    Me.lbl_calibri.ForeColor = AgroColor_Nero
        'Else
        '    Me.lbl_udm.ForeColor = AgroColor_Nero
        '    Me.lbl_calibri.ForeColor = AgroColor_Nero

        '    If Me.cmb_Calibro.SelectedIndex > 0 Then
        '        Me.lbl_udm.ForeColor = AgroColor_Rosso
        '    Else
        '        Me.lbl_calibri.ForeColor = AgroColor_Rosso
        '    End If
        'End If



        Pulisci_Giacenze_Prezzo()

        '------------

        Recupera_ChiaveProdotto()


        Recupera_ChiaveMagazzino()


        If xCod_Progetto <> 0 Then

            AgronicaCoreUtility.CaricaListControl.Semilavorati_Udm(Me.cmb_Udm,
                                                                    Num_Totale,
                                                                    xPiva,
                                                                    xSa_Cod,
                                                                    xFabbricato_Cod,
                                                                    xMat_Cod,
                                                                    xCod_Progetto,
                                                                    xLotto,
                                                                    xCod_Calibro,
                                                                    True, True, "", "",
                                                                    CDate(Txt_DataMovimento.Text),
                                                                    "", "", objParametri_Server, objParametri_Utenti)
            RipristinaUdmSelezionataInPrecedenza()

        Else

            'caso di carico ---> i semilavorati provengono da terzi
            'hanno il cod_progetto =0, ovvero nessun impianto

            If xCaricoScarico = enum_Agenda_Causali.SCARICO OrElse
                xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                'scarico o trasferimento
                Cau_Mov = enum_Agenda_Causali.SCARICO
            Else
                'carico
                Cau_Mov = enum_Agenda_Causali.CARICO
            End If

            AgronicaCoreUtility.CaricaListControl.Udm_Optimize2(
                                            Me.cmb_Udm,
                                             Num_Totale,
                                             xPiva,
                                             xSa_Cod,
                                             xFabbricato_Cod,
                                             Cau_Mov,
                                             xElem_Cod,
                                             True,
                                             0,
                                             0,
                                            xLotto,
                                            True, "", "",
                                            "", "", "", objParametri_Server, objParametri_Utenti)
            RipristinaUdmSelezionataInPrecedenza()

        End If

        If Me.cmb_Udm.Items.Count = 2 Then

            'se uno lo preseleziono
            Me.cmb_Udm.SelectedIndex = 1
            Cambio_Udm()

        End If

    End Sub


    '########################################################################################
    Private Sub cmb_Udm_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmb_Udm.SelectedIndexChanged

        Cambio_Udm()

    End Sub

    Private Sub Cambio_Udm()

        If IsNothing(Me.cmb_Udm) OrElse Me.cmb_Udm.Items.Count = 0 Then
            Exit Sub
        End If

        Setta_Label_Nero()
        Carica_Dopo_Udm()
    End Sub


    Private Sub Carica_Dopo_Udm()
        If Me.cmb_Udm.SelectedIndex > 0 Then

            'solo se non si tratta di altri beni o servizi
            If Not Me.Txt_BeniStrumentali.Visible Then

                If xCaricoScarico = CAU_CARICO Then
                    If Me.Cmb_Destinazione.SelectedIndex <> 0 Then
                        Carica_Giacenze_Prezzo()
                    Else
                        Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("MagazzinoDiDestinazioneNonSelezionato"), String), Page, , Me.upDati)
                    End If

                ElseIf xCaricoScarico = CAU_SCARICO Then
                    If Me.Cmb_Provenienza.SelectedIndex <> 0 Then
                        Carica_Giacenze_Prezzo()
                    Else
                        Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("MagazzinoDiProvenienzaNonSelezionato"), String), Page, , Me.upDati)
                    End If

                ElseIf xCaricoScarico = CAU_TRASFERIMENTO Then

                    If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                        Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiProvenienzaNonSelezionato"), String) & vbCrLf
                    End If
                    If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                        Messaggio &= DirectCast(GetLocalResourceObject("MagazzinoDiDestinazioneNonSelezionato"), String) & vbCrLf
                    End If
                    If Me.Cmb_Provenienza.SelectedIndex <> 0 AndAlso Me.Cmb_Destinazione.SelectedIndex <> 0 Then
                        If Me.Cmb_Provenienza.SelectedValue = Me.Cmb_Destinazione.SelectedValue Then
                            Messaggio &= AgronicaAgenda_2010.MagazziniDiProvenienzaEDestinazioneCoincidono & vbCrLf
                        End If
                    End If

                    If Messaggio <> "" Then
                        Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
                    End If

                    If Me.cmb_Udm.SelectedIndex > 0 Then
                        Carica_Giacenze_Prezzo()
                    End If

                End If

                'caso di scarico e trasferimento
                If xCaricoScarico <> CAU_CARICO Then

                    'se la giacenza non è negativa
                    If CDbl(ViewState("Giacenza_Magazzino")) >= 0 Then
                        Me.Txt_Quantita.Text = CStr(ViewState("Giacenza_Magazzino"))
                    Else
                        Me.Txt_Quantita.Text = "0"
                    End If

                End If

            End If

        Else
            '  Me.lbl_udm.ForeColor = AgroColor_Rosso
        End If

    End Sub

    '###############################################################################
    Private Sub Carica_Pannello_Economico()

        Me.Cmb_ContiContatto.Visible = False
        Me.Cmb_Conti.Visible = True

        'CaricaCombo_CodiciIVA(Server, Session, Page, Me.Cmb_IVA)
        AgronicaCoreUtility.CaricaListControl.IVA_aliquote(Me.Cmb_IVA,
                                                            False, "", "",
                                                            True,
                                                            True,
                                                            objParametri_Server)


        AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.Cmb_AnnoContabile,
                                                                           True, "", "",
                                                                           ViewState("Piva"),
                                                                           0,
                                                                           0,
                                                                           "", "",
                                                                           objParametri_Server)

        Me.Cmb_AnnoContabile.SelectedIndex = Me.Cmb_AnnoContabile.Items.Count - 1

        Cmb_AnnoContabile_SelectedIndexChanged(Me, Nothing)


    End Sub


    '########################################################################################
    Private Sub Cmb_AnnoContabile_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_AnnoContabile.SelectedIndexChanged

        If Me.Cmb_AnnoContabile.SelectedValue <> "" Then
            CaricaConti(Me.Cmb_AnnoContabile.SelectedValue)
        End If

    End Sub


    '########################################################################################
    Private Sub CaricaConti(ByVal Anno As Integer)

        Dim Dare_Avere As String = ""

        If xCaricoScarico = enum_Agenda_Causali.SCARICO Then

            Dare_Avere = "A"
            'strFiltro = "Cod_Contatto = '" & mPiva(DESTINAZIONE) & "'"

        ElseIf xCaricoScarico = enum_Agenda_Causali.CARICO Then

            Dare_Avere = "D"
            'strFiltro = "Cod_Contatto = '" & mPiva(PROVENIENZA) & "'"

        End If

        If Qs_CodContatto <> "" Then

            '_CaricaCombo_Conti2(Server, Session, Page, Me.Cmb_ContiContatto, _
            '                    viewstate("Piva"), _
            '                    BILANCIO_PERSONALIZZATO, _
            '                    Anno, _
            '                    0, _
            '                    False, _
            '                    Dare_Avere, _
            '                    Qs_CodContatto)

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Conti(
                           Me.Cmb_Conti,
                           True, "", "-1",
                           ViewState("Piva"),
                           BILANCIO_PERSONALIZZATO,
                           Anno,
                           0,
                           0, 0,
                           Dare_Avere,
                           Qs_CodContatto,
                           False,
                           "", "",
                           objParametri_Server)

        Else

            '_CaricaCombo_Conti2(Server, Session, Page, Me.Cmb_Conti, _
            '             viewstate("Piva"), _
            '             BILANCIO_PERSONALIZZATO, _
            '             Anno, _
            '             0, _
            '             False, _
            '             Dare_Avere, _
            '             "")

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Conti(
                                Me.Cmb_Conti,
                                True, "", "-1",
                                ViewState("Piva"),
                                BILANCIO_PERSONALIZZATO,
                                Anno,
                                0,
                                0, 0,
                                Dare_Avere,
                                "",
                                False,
                                "", "",
                                objParametri_Server)

        End If

        If Me.Cmb_Conti.Items.Count > 1 Then
            Me.Cmb_Conti.SelectedIndex = 1
        End If



    End Sub


    '########################################################################################
    Private Sub Chk_Conti_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_Conti.CheckedChanged

        Select Case Me.Chk_Conti.Checked

            Case True

                Me.Cmb_ContiContatto.Visible = True
                Me.Cmb_Conti.Visible = False

                'If CmbContoDiretto.ListIndex <> -1 Then
                '    Call CmbContoDiretto_Click()
                'End If

            Case False

                Me.Cmb_ContiContatto.Visible = False
                Me.Cmb_Conti.Visible = True

                'If CmbConto.ListIndex <> -1 Then
                '    Call CmbConto_Click()
                'End If

        End Select

    End Sub



    '########################################################################################
    Private Sub ImgBtn_CercaProdotti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CercaProdotti.Click
        info_sul_prodotto.Text = ""
        Cerca_Prodotti()

    End Sub



    '########################################################################################
    Private Sub Cerca_Prodotti()

        Dim NumProdottiTrovati As Integer = 0
        Dim FiltroAggiuntivo As String = ""
        Dim TipoPuaRegolamento As Integer = 0

        'Mi devo salvare il dt prodotti, perché in alcuni casi per il fertilizzante devo ricavare i titoli
        Dim dtProdotti As New DataTable
        Session("dtCmbProdotti") = Nothing


        Me.cmb_Prodotti.Items.Clear()

        Setta_Label_Nero()

        Nascondi_DoseEtichetta()

        If Me.cmb_Categoria.SelectedItem.Value = "" Then

            '  Me.lbl_categoria.ForeColor = AgroColor_Rosso

            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ScegliereLaCategoriaProdotto"), String), Page, , Me.upDati)
            Exit Sub

        End If

        'ogni volta che cambio categoria di prodotto elimino tutte le selezioni fatte precedentemente
        Me.cmb_Prodotti.Items.Clear()
        Me.cmb_Udm.Items.Clear()
        Me.cmb_Lotto.Items.Clear()
        Me.cmb_LottoAccettazione.Items.Clear()
        Me.cmb_Calibro.Items.Clear()

        Me.cmb_Prodotti.Enabled = True
        Me.cmb_Udm.Enabled = False
        Me.cmb_Lotto.Enabled = False
        Me.cmb_LottoAccettazione.Enabled = False
        Me.cmb_Calibro.Enabled = False


        Pulisci_Giacenze_Prezzo()

        Pulisci_Dettaglio_Fertilizzante()
        Dettaglio_Fertilizzante_Visibilita(False)


        If xCaricoScarico = enum_Agenda_Causali.SCARICO OrElse xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then

            If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                Messaggi.AgroMsgBox(AgronicaAgenda_2010.SelezionareIlMagazzinoDiProvenienza, Page, , Me.upDati)
                Exit Sub
            Else
                'scarico
                xSa_Cod = Me.Cmb_Provenienza.SelectedValue.Split("|")(1)
                xFabbricato_Cod = Me.Cmb_Provenienza.SelectedValue.Split("|")(0)
            End If

        Else
            'CARICO

            If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                Messaggi.AgroMsgBox(AgronicaAgenda_2010.SelezionareIlMagazzinoDiDestinazione, Page, , Me.upDati)
                Exit Sub
            Else
                'carico
                xSa_Cod = Me.Cmb_Destinazione.SelectedValue.Split("|")(1)
                xFabbricato_Cod = Me.Cmb_Destinazione.SelectedValue.Split("|")(0)
            End If

            'aggiunto il 08/08/2018: altrimenti con la creazione automatica, da banca dati, di queste materie prime, la ricerca diventava lentissima
            '05/09/2018: esteso anche ai prodotti da B.D. per evitare errore webservice
            Select Case Me.cmb_Categoria.SelectedValue
                Case SEMENTI, SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI, FERTILIZZANTI, FORMULATI
                    If Not (cmb_Categoria.SelectedValue = FERTILIZZANTI AndAlso
                        cmb_PUARegolamenti.Visible AndAlso
                        cmb_PUARegolamenti.Items.Count > 0 AndAlso
                        cmb_PUARegolamenti.SelectedItem.Value.Split(SEP_PuaReg)(1) <> "0") Then
                        If Me.Txt_CercaCodArticolo.Text = "" AndAlso Me.Txt_CercaProdotto.Text = "" AndAlso Me.Txt_CercaLotto.Text = "" Then
                            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("SpecificareUnTestoPerRicercaProdotti"), String), Page,  , Me.upDati)
                            Exit Sub
                        End If
                    End If
            End Select

        End If


        Select Case Me.cmb_Categoria.SelectedItem.Value

            Case MACCHINE, SEMENTI, ALTRE_MATERIE, MANGIMI,
                SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI,
                CARBURANTI, CAT_MAG_SERVIZI_PROFESSIONALI

                Select Case Me.cmb_Categoria.SelectedValue

                    Case SEMENTI, ALTRE_MATERIE,
                            SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                            SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI

                        If Me.Txt_CercaCodArticolo.Text <> "" Then
                            FiltroAggiuntivo = "  (Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(Me.Txt_CercaCodArticolo.Text) & "%') "
                        End If

                End Select

                Select Case Qs_Mode.ToLower

                    Case "trasferimento"
                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                        clc.Materie_Prime(cmb_Prodotti,
                                                             True,
                                                              "",
                                                            "",
                                                            CAU_SCARICO,
                                                            xPiva,
                                                            xSa_Cod,
                                                            xFabbricato_Cod,
                                                            Me.cmb_Categoria.SelectedItem.Value,
                                                            True,
                                                            Me.Txt_CercaProdotto.Text,
                                                            Me.Txt_CercaLotto.Text,
                                                            Me.Txt_CercaCodArticolo.Text,
                                                            "", 0, 0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO, 0, 0, 0, 0, 0, 0, 0,
                                                            CDate(Txt_DataMovimento.Text).ToShortDateString,
                                                            FiltroAggiuntivo, "", objParametri_Server, objParametri_Utenti
                                                             )

                        NumProdottiTrovati = cmb_Prodotti.Items.Count - 1


                    Case Else

                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                        clc.Materie_Prime(cmb_Prodotti,
                                                             True,
                                                              "",
                                                            "",
                                                            xCaricoScarico,
                                                            xPiva,
                                                            xSa_Cod,
                                                            xFabbricato_Cod,
                                                            Me.cmb_Categoria.SelectedItem.Value,
                                                            True,
                                                            Me.Txt_CercaProdotto.Text,
                                                            Me.Txt_CercaLotto.Text,
                                                            Me.Txt_CercaCodArticolo.Text,
                                                            "", 0, 0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO, 0, 0, 0, 0, 0, 0, 0,
                                                            CDate(Txt_DataMovimento.Text).ToShortDateString,
                                                            FiltroAggiuntivo, "", objParametri_Server, objParametri_Utenti
                                                             )

                        NumProdottiTrovati = cmb_Prodotti.Items.Count - 1

                End Select

            Case FERTILIZZANTI, FARMACI, FORMULATI, COADIUVANTI, INSETTI, TRAPPOLE, INNESCHI 'CARBURANTI

                Dim Flag_VisualizzaProCod As Boolean = True

                If Me.cmb_Categoria.SelectedItem.Value = CARBURANTI Then
                    Flag_VisualizzaProCod = False
                End If

                Dim Flag_CaricaUdmCod As Boolean = False

                If xCaricoScarico = CAU_CARICO Then
                    If Me.cmb_Categoria.SelectedItem.Value = FORMULATI Then
                        Flag_CaricaUdmCod = True
                    End If
                End If

                Dim PuaRegolamento As Integer = 0
                Dim RegolamentoCod_Operazioni As Integer = 0
                Dim Flag_IncludiNPK_Desc As Boolean = False
                Dim Flag_IncludiClassificazione As Boolean = False

                If Me.cmb_Categoria.SelectedItem.Value = FERTILIZZANTI Then

                    Flag_VisualizzaProCod = False
                    Flag_IncludiNPK_Desc = True
                    Flag_IncludiClassificazione = True

                    If cmb_PUARegolamenti.Visible AndAlso cmb_PUARegolamenti.Items.Count > 0 Then
                        PuaRegolamento = cmb_PUARegolamenti.SelectedValue.Split(SEP_PuaReg)(0)
                        TipoPuaRegolamento = cmb_PUARegolamenti.SelectedValue.Split(SEP_PuaReg)(1)
                        'If PuaRegolamento = -2 Then
                        '    PuaRegolamento = 0
                        '    RegolamentoCod_Operazioni = -2
                        'End If
                    End If
                End If



                Select Case Qs_Mode.ToLower

                    Case "trasferimento"

                        'Carico i prodotti della categoria scelta nella cmb_CatProdotto e con la descrizione scelta
                        Dim clc As New AgronicaCoreVarieBIZ.CaricaListControl_2010
                        clc.Prodotti(Me.cmb_Prodotti,
                                                                             True,
                                                                             "",
                                                                             "",
                                                                             CAU_SCARICO,
                                                                             xPiva,
                                                                             xSa_Cod,
                                                                             xFabbricato_Cod,
                                                                             Me.cmb_Categoria.SelectedItem.Value,
                                                                             False,
                                                                             Me.Txt_CercaProdotto.Text,
                                                                             Flag_VisualizzaProCod,
                                                                             False,
                                                                             0, 0,
                                                                             Me.Txt_DataMovimento.Text,
                                                                             PuaRegolamento,
                                                                             0,
                                                                             True,
                                                                             False,
                                                                             "",
                                                                             "",
                                                                             objParametri_Server, objParametri_Utenti,
                                                                             dtProdotti)

                        NumProdottiTrovati = Me.cmb_Prodotti.Items.Count - 1
                        Session("dtCmbProdotti") = dtProdotti

                    Case Else

                        'Carico i prodotti della categoria scelta nella cmb_CatProdotto e con la descrizione scelta

                        Dim Flag_FiltraRevocati As Boolean = True
                        If xCaricoScarico = CAU_SCARICO Then
                            Flag_FiltraRevocati = False
                        End If

                        '(29/11/2019)
                        Dim Stato_Cod As String = ""
                        Select Case cmb_Categoria.SelectedItem.Value
                            Case FORMULATI, FERTILIZZANTI
                                'controllo stato centro
                                Dim objIndC As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                                Stato_Cod = objIndC.Stato_from_PivaSaCod(xPiva, xSa_Cod, objParametri_Server)
                                If Stato_Cod.Length <> 2 Then
                                    Stato_Cod = "IT"
                                End If
                        End Select

                        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
                        clc.Prodotti(Me.cmb_Prodotti,
                                                                             True,
                                                                             "",
                                                                             "",
                                                                             xCaricoScarico,
                                                                             xPiva,
                                                                             xSa_Cod,
                                                                             xFabbricato_Cod,
                                                                             Me.cmb_Categoria.SelectedItem.Value,
                                                                             False,
                                                                             Me.Txt_CercaProdotto.Text,
                                                                             Flag_VisualizzaProCod,
                                                                             Flag_CaricaUdmCod,
                                                                             0, 0,
                                                                             Me.Txt_DataMovimento.Text,
                                                                             PuaRegolamento,
                                                                             0,
                                                                             True,
                                                                             Flag_FiltraRevocati,
                                                                             "", "",
                                                                             objParametri_Server, objParametri_Utenti,
                                                                             dtProdotti,
                                                                             Nothing,
                                                                             RegolamentoCod_Operazioni,
                                                                             TipoPuaRegolamento,
                                                                             Flag_IncludiNPK_Desc,
                                                                             Flag_IncludiClassificazione, Stato_Cod:=Stato_Cod)

                        NumProdottiTrovati = Me.cmb_Prodotti.Items.Count - 1
                        Session("dtCmbProdotti") = dtProdotti
                End Select


            Case Else
                'Consistenza Zootecnica

        End Select

        If xCaricoScarico = enum_Agenda_Causali.CARICO Then

            Dim Num_Totale As Integer = 0


            AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                                              Me.cmb_Udm,
                                             Num_Totale,
                                             xPiva,
                                             xSa_Cod,
                                             xFabbricato_Cod,
                                             xCaricoScarico,
                                             Me.cmb_Categoria.SelectedItem.Value,
                                             True,
                                             0,
                                             0,
                                            True, "", "",
                                            "", "", "", objParametri_Server, objParametri_Utenti)
            RipristinaUdmSelezionataInPrecedenza()

        End If

        Txt_NumProdotti.ForeColor = System.Drawing.Color.Green
        Select Case NumProdottiTrovati
            Case 0
                Me.Txt_NumProdotti.Text = AgronicaAgenda_2010.NessunProdottoTrovato
                'Me.lbl_categoria.ForeColor = AgroColor_Rosso
               ' Txt_NumProdotti.ForeColor = AgroColor_Rosso
            Case 1
                Me.Txt_NumProdotti.Text = CStr(NumProdottiTrovati) & " " & AgronicaAgenda_2010.Trovato.ToLower()
                'Me.lbl_descrizione.ForeColor = AgroColor_Rosso
            Case Else
                Me.Txt_NumProdotti.Text = CStr(NumProdottiTrovati) & " " & AgronicaAgenda_2010.Trovati.ToLower()
                'Me.lbl_descrizione.ForeColor = AgroColor_Rosso
        End Select


        If NumProdottiTrovati = 1 Then

            'se uno lo preseleziono
            Me.cmb_Prodotti.SelectedIndex = 1
            Cambio_Prodotto()

        End If

        If xCaricoScarico = enum_Agenda_Causali.CARICO AndAlso
           Me.cmb_Categoria.SelectedItem.Value = FERTILIZZANTI AndAlso
           TipoPuaRegolamento = enum_PUARegolamenti_Tipo.PUA Then

            Dettaglio_Fertilizzante_Visibilita(True)

        End If

    End Sub

    '##########################################################################################################
    Private Sub ImgBtn_Help_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Help.Click

        'Dim strOpen As String
        Dim LinkPagina As String

        LinkPagina = "../Help/Help_GestioneMagazzini_MagazziniMovimenti_Edit.aspx"

        'AgroHelp(Server, Session, Page, LinkPagina)

    End Sub


    '################################################################################################################
    ''' <summary>
    ''' Usa l'AgronicaCoreModello, CHIAMATA IN CASO DI MODIFICA, NELLE OPERAZIONI DI CARICO/SCARICO, INCREMENTO/DECREMENTO/ 
    ''' TRASFERIMENTO, ACQUISTO/VENDITA
    ''' </summary>
    Private Sub Ripristina_DATI_nei_Controlli_2(ByVal Operazione As Integer)

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita

        Dim Sa_Cod As Integer
        Dim Lav_Cod As Integer
        Dim Elem_Cod As Integer
        Dim Pro_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Id_Magazzino As Integer
        Dim Cau_Mov As String
        Dim Qta As Decimal
        Dim Udm_Cod As Integer
        Dim Prezzo_Unitario As Decimal
        Dim Prezzo_Unitario_Netto As Decimal
        Dim Lotto As String
        Dim Cod_Progetto As Integer
        Dim Cal_Cod As Integer
        Dim Pendente As String
        Dim Extra_int As Integer


        Dim Udm_Cod_Extra As Decimal = 0
        Dim Qta_Extra As Decimal = 0
        Dim Prezzo_Effettivo As Decimal = 0
        Dim Sconto_Magg As Decimal = 0
        Dim Imponibile As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Cod_Iva As Integer = 0
        Dim Iva As Decimal = 0
        Dim Anno As Integer = 0
        Dim Ric_Cod As Integer = 0
        Dim Cod_Conto As Integer = 0

        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Cu As Decimal = 0
        Dim Regolamento_Cod_Ferti As Integer = 0

        Dim Spe_Des As String
        Dim Ipro_Des As String
        Dim Raz_Des As String
        Dim Cat_Des As String

        Dim Metodo_Produzione_Des As String

        Dim Rif_Esterno As String = ""
        Dim Rif_Esterno_2 As String = ""

        Dim Num_Totale As Integer = 0

        xPiva = ViewState("Piva")
        xSa_Cod = ViewState("Sa_Cod")
        xAgenda = ViewState("id_agenda")


        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As Operazione_Agenda = objAgenda.Leggi(xPiva, xSa_Cod, xAgenda, 0,
                                                          objParametri_Server)

        If Not IsNothing(Agenda) Then

            Lav_Cod = Agenda.Lav_Cod

            If Agenda.Movimenti IsNot Nothing AndAlso Agenda.Movimenti.Count > 0 Then

                '22/08/2018: introdotto controllo sul cau_mov per gestire i corrispettivi di vendita
                For z = 0 To Agenda.Movimenti.Count - 1
                    If Agenda.Movimenti(z).Cau_Mov <> CAU_REGISTRAZIONI Then

                        'nel caso del trasferimento deve fare un giro solo perché il doppio popolamento del magazzino è gestito a parte, in fondo
                        If Qs_Mode.ToLower <> "trasferimento" OrElse (Qs_Mode.ToLower = "trasferimento" AndAlso z = 0) Then
                            '-----------------------------------------------------------------
                            '--------------------- PANNELLO MOVIMENTO ------------------------
                            '-----------------------------------------------------------------
                            Me.Txt_DataMovimento.Text = Agenda.Movimenti(z).Data
                            Me.Txt_Ora.Text = Format(CDate(Agenda.Movimenti(z).Ora), "HH:mm")
                            If Me.Txt_Ora.Text = "00.00" Then
                                Me.Txt_Ora.Text = "12" & gettimesep() & "00"
                            End If
                            Me.TextBoxNote.Text = CStr(Agenda.Movimenti(z).Mov_Desc)

                            '-----------------------------------------------------------------
                            '------------------------- DETTAGLI ------------------------------
                            '-----------------------------------------------------------------
                            If Agenda.Movimenti(z).Movimenti_Dettagli IsNot Nothing AndAlso Agenda.Movimenti(z).Movimenti_Dettagli.Count > 0 Then

                                Rif_Esterno = Agenda.Movimenti(z).Movimenti_Dettagli(0).Rif_Esterno
                                Rif_Esterno_2 = Agenda.Movimenti(z).Movimenti_Dettagli(0).Rif_Esterno_2

                                Ripristina_DettagliNascosti(Rif_Esterno, Rif_Esterno_2)

                                CaricaCombo_Causale(Lav_Cod)

                                Pendente = Agenda.Movimenti(z).Movimenti_Dettagli(0).Pendente
                                Me.cmb_Causale.SelectedIndex = CInt(Me.cmb_Causale.Items.IndexOf(Me.cmb_Causale.Items.FindByValue(Pendente)))
                                If cmb_Causale.SelectedValue = CStr(enum_Pendenza.Furto) Then
                                    'se sono in causale furto allora abilito le textbox per scrivere la denuncia
                                    denuncia.Visible = True
                                    Dim temp As String = Agenda.Movimenti(z).Movimenti_Dettagli(0).Extra_Date
                                    If CDate(temp) <> AGRODATAINIZIO AndAlso CDate(temp) <> AGRODATAFINE Then
                                        Txt_DataDenuncia.Text = CDate(temp)
                                    End If
                                    temp = Agenda.Movimenti(z).Movimenti_Dettagli(0).Extra_Str
                                    Txt_NumeroDenuncia.Text = temp
                                Else
                                    denuncia.Visible = False
                                End If

                                Cau_Mov = Agenda.Movimenti(z).Movimenti_Dettagli(0).Cau_Mov

                                Elem_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Elem_Cod
                                Pro_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Pro_Cod
                                Mat_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Mat_Cod
                                Cod_Progetto = Agenda.Movimenti(z).Movimenti_Dettagli(0).Cod_Progetto
                                Cal_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Cal_Cod
                                Lotto = Agenda.Movimenti(z).Movimenti_Dettagli(0).Lotto
                                Udm_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Udm_Cod
                                Extra_int = Agenda.Movimenti(z).Movimenti_Dettagli(0).Extra_Int

                                Udm_Cod_Extra = Agenda.Movimenti(z).Movimenti_Dettagli(0).Udm_Cod_Extra
                                Qta_Extra = Agenda.Movimenti(z).Movimenti_Dettagli(0).Qta_Extra
                                Prezzo_Effettivo = Agenda.Movimenti(z).Movimenti_Dettagli(0).Prezzo_Effettivo
                                Sconto_Magg = Agenda.Movimenti(z).Movimenti_Dettagli(0).Sconto
                                Cod_Iva = Agenda.Movimenti(z).Movimenti_Dettagli(0).Cod_Iva

                                Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDbl(Agenda.Movimenti(z).Movimenti_Dettagli(0).Imponibile))
                                Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDbl(Agenda.Movimenti(z).Movimenti_Dettagli(0).Imponibile_Netto))
                                Iva = objContabHLP.Leggi_IVA_PositivaNegativa(Lav_Cod, CDbl(Agenda.Movimenti(z).Movimenti_Dettagli(0).Iva))

                                Anno = Agenda.Movimenti(z).Movimenti_Dettagli(0).Anno
                                Ric_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Ric_Cod
                                Cod_Conto = Agenda.Movimenti(z).Movimenti_Dettagli(0).Cod_Conto

                                '-----------------------------------------------------------------
                                '--------------------- QUANTITA' E PREZZO ------------------------
                                '-----------------------------------------------------------------

                                Qta = Agenda.Movimenti(z).Movimenti_Dettagli(0).Qta
                                Qta_Modifica_Giacenza = Qta

                                Prezzo_Unitario = Agenda.Movimenti(z).Movimenti_Dettagli(0).Prezzo_Unitario
                                Prezzo_Unitario_Netto = Agenda.Movimenti(z).Movimenti_Dettagli(0).Prezzo_Unitario_Netto

                                Me.Txt_Quantita.Text = Qta
                                Me.Txt_PrezzoUnitario.Text = Prezzo_Unitario
                                Me.Txt_PrezzoUnitario_Netto.Text = Prezzo_Unitario_Netto


                                '-----------------------------------------------------------------
                                '--------------------- CATEGORIA PRODOTTO ------------------------
                                '-----------------------------------------------------------------

                                Dim Flag_NoSemilavorati As Boolean = False

                                Call AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(
                                                                Me.cmb_Categoria,
                                                                True, "", "",
                                                                0,
                                                                 xCaricoScarico,
                                                                 0,
                                                                 Flag_NoSemilavorati,
                                                                 True,
                                                                 "", "", objParametri_Server)

                                Num_Totale = cmb_Categoria.Items.Count - 1
                                'ho l'elem_cod -> imposto la combo delle categorie magazzino
                                'Imposto la selezione della combobox
                                cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(Me.cmb_Categoria.Items.FindByValue(Elem_Cod))

                                Cambio_Categoria()

                                Lotto_ElemCod_RipristinaDettaglio(Elem_Cod, Lotto, Pro_Cod, Mat_Cod, Operazione = enum_TipoOperazioneDB.Lettura)

                                Select Case Elem_Cod

                                    Case SERVIZI

                                        'Bene Non Movimentabile           
                                        Id_Magazzino = 0

                                        Me.Pannello_Magazzini.Visible = False
                                        Me.lbl_magazzini.Visible = False

                                        Me.Txt_BeniStrumentali.Visible = False
                                        Me.cmb_Prodotti.Visible = True
                                        AgronicaCoreUtility.CaricaListControl.CaricaServizi(Me.cmb_Prodotti, False, "", "", "", "", "", objParametri_Server)


                                        Me.ImgBtn_CercaProdotti.Visible = False
                                        Me.ImgBtn_CercaProdotti.Enabled = False
                                        Me.Lbl_Cerca.Visible = False

                                        Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                                        Me.Chk_MovimentoMag.Checked = False

                                        AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                                                          Me.cmb_Udm,
                                                            Num_Totale,
                                                            "",
                                                            0,
                                                            0,
                                                            CAU_CARICO,
                                                            0,
                                                            False,
                                                            0,
                                                            0,
                                                True, "", "",
                                                "", "", "", objParametri_Server, objParametri_Utenti)

                                        'seleziono l'unita di misura specifica
                                        cmb_Udm.SelectedIndex = cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                                        Me.cmb_Udm.Enabled = True


                                    Case ALTRI_BENI, 1

                                        'Bene Non Movimentabile           
                                        Id_Magazzino = 0

                                        Me.Pannello_Magazzini.Visible = False
                                        Me.lbl_magazzini.Visible = False

                                        Me.Txt_BeniStrumentali.Text = Agenda.Movimenti(z).Movimenti_Dettagli(0).Mov_Det_Des
                                        Me.cmb_Prodotti.Visible = False
                                        Me.Txt_BeniStrumentali.Visible = True

                                        Me.ImgBtn_CercaProdotti.Visible = False
                                        Me.ImgBtn_CercaProdotti.Enabled = False
                                        Me.Lbl_Cerca.Visible = False

                                        Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                                        Me.Chk_MovimentoMag.Checked = False

                                        AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                                                  Me.cmb_Udm,
                                                            Num_Totale,
                                                            "",
                                                            0,
                                                            0,
                                                            CAU_CARICO,
                                                            0,
                                                            False,
                                                            0,
                                                            0,
                                                True, "", "",
                                                "", "", "", objParametri_Server, objParametri_Utenti)


                                        'seleziono l'unita di misura specifica
                                        cmb_Udm.SelectedIndex = cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))
                                        Me.cmb_Udm.Enabled = True


                                    Case ZOO_CONSISTENZA

                                        Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoStallaAbbr"), String)
                                        Me.Chk_MovimentoMag.Checked = True

                                        CaricaGriglia_Matricole()

                                        '-----------------------------------------------------------------
                                        '------------ PANNELLO MAGAZZINI --- MOV DESTINAZIONE ------------
                                        '-----------------------------------------------------------------

                                        If Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing AndAlso Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then

                                            Id_Magazzino = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Id_Destinazione

                                            If Cau_Mov.Trim = CAU_CARICO Then

                                                Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                clc.Fabbricati(Me.Cmb_Destinazione,
                                                                            True, "", "",
                                                                             xPiva,
                                                                           xSa_Cod,
                                                                            0,
                                                                            STALLA,
                                                                            True,
                                                                            "",
                                                                            " Fabbricati.Fabbricato_Des ",
                                                                            AGRODATAFINE,
                                                                            objParametri_Server)

                                                'CARICO
                                                Cmb_Destinazione.SelectedIndex =
                                            Cmb_Destinazione.Items.IndexOf(Cmb_Destinazione.Items.FindByValue(
                                                CStr(Id_Magazzino) & "|" & CStr(xSa_Cod)))

                                                Cmb_Destinazione_SelectedIndexChanged(Me, Nothing)

                                            Else

                                                Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                clc.Fabbricati(Me.Cmb_Provenienza,
                                                                          True, "", "",
                                                                           xPiva,
                                                                         xSa_Cod,
                                                                          0,
                                                                          STALLA,
                                                                          True,
                                                                          "",
                                                                          " Fabbricati.Fabbricato_Des ",
                                                                          AGRODATAFINE,
                                                                          objParametri_Server)

                                                'SCARICO
                                                Cmb_Provenienza.SelectedIndex =
                                            Cmb_Provenienza.Items.IndexOf(Cmb_Provenienza.Items.FindByValue(
                                                CStr(Id_Magazzino) & "|" & CStr(xSa_Cod)))

                                                Cmb_Provenienza_SelectedIndexChanged(Me, Nothing)

                                            End If

                                            '-------------- PANNELLO ANIMALI   -------------------------------
                                            '-----------------------------------------------------------------

                                            Dim ObjAnimale_Anagrafe_R As New AgronicaCoreZooDAL.Zoo_Animali_R
                                            Dim RsAnimale_Anagrafe_R As DataTable


                                            RsAnimale_Anagrafe_R = ObjAnimale_Anagrafe_R.Leggi(CStr(xPiva),
                                                                       0,
                                                                       CInt(Cod_Progetto),
                                                                       "", 0, 0, 0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


                                            If RsAnimale_Anagrafe_R.Rows.Count <> 0 Then

                                                Dim ll As Integer = 0

                                                Spe_Des = StallaSpecieDes_from_StallaSpecieCod2(
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Gen_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Spe_Cod")),
                                                                        objParametri_Server)

                                                Ipro_Des = StallaIProDes_from_StallaIProCod2(
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Gen_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Spe_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Ipro_Cod")),
                                                                        objParametri_Server)

                                                Cat_Des = StallaCategoriaDes_from_StallaCategoriaCod2(
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Gen_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Spe_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Ipro_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Cat_Cod")),
                                                                        objParametri_Server)

                                                Raz_Des = StallaRazzaDes_from_StallaRazzaCod2(
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Gen_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Spe_Cod")),
                                                                        CInt(RsAnimale_Anagrafe_R.Rows(ll).Item("Raz_Cod")),
                                                                        objParametri_Server)

                                                Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale

                                                Select Case RsAnimale_Anagrafe_R.Rows(ll).Item("metodo_produzione")
                                                    Case 1
                                                        Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale
                                                    Case 2
                                                        Metodo_Produzione_Des = AgronicaAgenda_2010.InConversione
                                                    Case 3
                                                        Metodo_Produzione_Des = AgronicaAgenda_2010.Biologico
                                                End Select

                                                Inserisci_Riga_Matricola(Cod_Progetto,
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Gen_Cod"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Spe_Cod"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Ipro_Cod"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Raz_Cod"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Cat_Cod"),
                                                                Spe_Des,
                                                                Ipro_Des,
                                                                Raz_Des,
                                                                Cat_Des,
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Matricola"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Nome"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("Collare"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("nome_aia"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("matricola_aia"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("dat_nascita"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("prov_nascita"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("stato_nascita"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("aua_azi_nascita"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("ausl_azi_nascita"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("sesso"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("mat_padre"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("mat_madre"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("peso"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("data_pesa"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("metodo_produzione"),
                                                                Metodo_Produzione_Des,
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("conversione_inizio"),
                                                                RsAnimale_Anagrafe_R.Rows(ll).Item("conversione_fine"),
                                                                Qta,
                                                                Prezzo_Unitario)

                                                CType(DataGrid_ZooAnimali.Items(0).FindControl("txtqta"), TextBox).Text = Qta

                                                CType(DataGrid_ZooAnimali.Items(0).FindControl("txtprezzo"), TextBox).Text = Prezzo_Unitario


                                            End If


                                        End If



                                    Case Else


                                        'ATTENZIONE!!!!!!!
                                        Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                                        Me.Chk_MovimentoMag.Checked = True

                                        Select Case Elem_Cod
                                            Case MACCHINE, SEMENTI, ALTRE_MATERIE, MANGIMI, FARMACI,
                                                SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                                                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI,
                                                CARBURANTI, CAT_MAG_SERVIZI_PROFESSIONALI

                                                '-----------------------------------------------------------------
                                                '----------------------- MATERIE PRIME ---------------------------
                                                '-----------------------------------------------------------------
                                                '21/08/2018: introdotto case cau_mov per filtrare in modalità CARICO il mat_cod 
                                                '(per caricare solo quello ed evitare di caricare la combo con tutti i prodotti di anagrafica, 
                                                'l 'utente eventualmente li cerca dopo con la lente)
                                                Select Case Cau_Mov

                                                    Case CAU_CARICO
                                                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                        clc.Materie_Prime(cmb_Prodotti,
                                                                                                            True,
                                                                                                            "",
                                                                                                            "",
                                                                                                            Cau_Mov,
                                                                                                            xPiva,
                                                                                                            xSa_Cod,
                                                                                                            Id_Magazzino,
                                                                                                            Elem_Cod,
                                                                                                            True,
                                                                                                            "",
                                                                                                            "",
                                                                                                            "",
                                                                                                            "",
                                                                                                            Mat_Cod,
                                                                                                            0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO, 0, 0, 0, 0, 0, 0, 0,
                                                                                                            CDate(Txt_DataMovimento.Text).ToShortDateString,
                                                                                                            "", "", objParametri_Server, objParametri_Utenti)

                                                    Case Else
                                                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                        clc.Materie_Prime(cmb_Prodotti,
                                                                                                            True,
                                                                                                            "",
                                                                                                            "",
                                                                                                            Cau_Mov,
                                                                                                            xPiva,
                                                                                                            xSa_Cod,
                                                                                                            Id_Magazzino,
                                                                                                            Elem_Cod,
                                                                                                            True,
                                                                                                            "",
                                                                                                            "",
                                                                                                            "",
                                                                                                            "",
                                                                                                            0, 0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO, 0, 0, 0, 0, 0, 0, 0,
                                                                                                            CDate(Txt_DataMovimento.Text).ToShortDateString,
                                                                                                            "", "", objParametri_Server, objParametri_Utenti)

                                                End Select

                                                Num_Totale = cmb_Prodotti.Items.Count - 1


                                                cmb_Prodotti.SelectedIndex =
                                                cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(-Mat_Cod))


                                                Select Case Elem_Cod

                                                    Case SEMILAVORATI_VEGETALI



                                                        If Cod_Progetto <> 0 Then

                                                            '-----------------------------------------------------------------
                                                            '---------------- SEMILAVORATI:  LOTTI ---------------------------
                                                            '-----------------------------------------------------------------


                                                            AgronicaCoreUtility.CaricaListControl.Semilavorati_LottoInterno(Me.cmb_Lotto,
                                                                            Num_Totale,
                                                                            xPiva,
                                                                            xSa_Cod,
                                                                            Id_Magazzino,
                                                                            Mat_Cod,
                                                                            True, "", "",
                                                                            "",
                                                                            CDate(Txt_DataMovimento.Text),
                                                                            Cau_Mov,
                                                                            "", "", objParametri_Server, objParametri_Utenti)

                                                            Me.cmb_Lotto.Enabled = True


                                                            cmb_Lotto.SelectedIndex = cmb_Lotto.Items.IndexOf(cmb_Lotto.Items.FindByValue(Cod_Progetto))

                                                        Else

                                                            '-----------------------------------------------------------------
                                                            '---------------- SEMILAVORATI:  LOTTI ---------------------------
                                                            '-----------------------------------------------------------------

                                                            Me.cmb_Lotto.Items.Add(New ListItem("", ""))
                                                            Me.cmb_Lotto.Enabled = True

                                                            cmb_Lotto.SelectedIndex =
                                                        cmb_Lotto.Items.IndexOf(cmb_Lotto.Items.FindByValue(
                                                            Cod_Progetto))

                                                        End If

                                                        AgronicaCoreUtility.CaricaListControl.Semilavorati_ParametriQualitativi(
                                                                           Me.cmb_Calibro,
                                                                                    Num_Totale,
                                                                                    xPiva,
                                                                                    xSa_Cod,
                                                                                    Id_Magazzino,
                                                                                    Mat_Cod,
                                                                                    Cod_Progetto,
                                                                                    Lotto,
                                                                               True, "", "",
                                                                               CDate(Txt_DataMovimento.Text),
                                                                               "", "", objParametri_Server, objParametri_Utenti)


                                                        Me.cmb_Calibro.Enabled = True

                                                        cmb_Calibro.SelectedIndex =
                                                    cmb_Calibro.Items.IndexOf(cmb_Calibro.Items.FindByValue(Cal_Cod))

                                                End Select

                                            Case FERTILIZZANTI, FORMULATI, COADIUVANTI, INSETTI, TRAPPOLE, INNESCHI 'CARBURANTI

                                                '-----------------------------------------------------------------
                                                '------------------------- PRODOTTI ------------------------------
                                                '-----------------------------------------------------------------

                                                Dim Flag_VisualizzaProCod As Boolean = True

                                                Select Case Me.cmb_Categoria.SelectedItem.Value
                                                    Case FORMULATI
                                                        'Me.BtnInfo.Visible = True
                                                        Me.BtnInfo_fito.Visible = True
                                                    Case FERTILIZZANTI
                                                        Me.BtnInfo_Concime.Visible = True
                                                    Case CARBURANTI
                                                        Flag_VisualizzaProCod = False
                                                End Select

                                                Num_Totale = 1

                                                ' Sono in modifica e carico solo il prodotto. Se si vuole modificare il prodotto
                                                ' l'utente deve eseguire di nuovo la ricerca

                                                Dim objCatMagazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                                                Dim objMatPrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                                Dim Pro_Des As String
                                                Select Case Me.cmb_Categoria.SelectedItem.Value

                                                    Case FERTILIZZANTI
                                                        If Mat_Cod <> 0 Then
                                                            Pro_Des = objMatPrime.MatDes_from_MatCod(xPiva, Elem_Cod, Mat_Cod, "", "", "", objParametri_Server)
                                                            cmb_Prodotti.Items.Add(New ListItem(Pro_Des, -Mat_Cod))
                                                            cmb_Prodotti.SelectedIndex =
                                                       cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(
                                                           -Mat_Cod))
                                                        Else
                                                            Pro_Des = objCatMagazzino.ProDes_from_ProCod(Elem_Cod, Pro_Cod, objParametri_Server)
                                                            cmb_Prodotti.Items.Add(New ListItem(Pro_Des, Pro_Cod))

                                                            cmb_Prodotti.SelectedIndex =
                                                       cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(Pro_Cod))
                                                        End If

                                                        '-----------------------------------------------------------------
                                                        '----------------------- DETTAGLI FERTILIZZANTE ------------------
                                                        '-----------------------------------------------------------------

                                                        If Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso
                                                           Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici.Count > 0 Then
                                                            N = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).N
                                                            P2O5 = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).P
                                                            K2O = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).K
                                                            Cu = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Cu
                                                            Regolamento_Cod_Ferti = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Extra_Int
                                                        End If

                                                        Ripristina_DettagliFertilizzante(N, P2O5, K2O, Cu, Regolamento_Cod_Ferti)



                                                    Case Else
                                                        Pro_Des = objCatMagazzino.ProDes_from_ProCod(Elem_Cod, Pro_Cod, objParametri_Server)
                                                        cmb_Prodotti.Items.Add(New ListItem(Pro_Des, Pro_Cod))

                                                        cmb_Prodotti.SelectedIndex = cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(Pro_Cod))

                                                End Select

                                            Case Else
                                                'Consistenza Zootecnica

                                        End Select


                                        If Me.Pannello_Economico.Visible Then


                                            '-----------------------------------------------------------------
                                            '----------------------- DETTAGLI ECONOMICI ----------------------
                                            '-----------------------------------------------------------------

                                            Ripristina_DettagliEconomici(Udm_Cod_Extra,
                                                                    Qta_Extra,
                                                                    Prezzo_Effettivo,
                                                                    Sconto_Magg,
                                                                    Imponibile,
                                                                    Imponibile_Netto,
                                                                    Cod_Iva,
                                                                    Iva,
                                                                    Anno,
                                                                    Ric_Cod,
                                                                    Cod_Conto)

                                        End If


                                        '-----------------------------------------------------------------
                                        '-------------- PANNELLO MAGAZZINI --- DESTINAZIONE --------------
                                        '-----------------------------------------------------------------

                                        Select Case Qs_Mode.ToLower

                                            Case "trasferimento"

                                                Select Case Agenda.Movimenti(z).Movimenti_Dettagli(0).Cau_Mov

                                                    Case CAU_SCARICO

                                                        If Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing AndAlso Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then

                                                            Sa_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Sa_Cod
                                                            Id_Magazzino = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Id_Destinazione

                                                            Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                            clc.Fabbricati(Me.Cmb_Provenienza,
                                                                     True, "", "",
                                                                      xPiva,
                                                                    0,
                                                                     0,
                                                                     FABBRICATI_NO_STALLE,
                                                                     True,
                                                                     "",
                                                                     " Fabbricati.Fabbricato_Des ",
                                                                     AGRODATAFINE,
                                                                     objParametri_Server)

                                                            'Imposto la posizione nella combo 
                                                            Cmb_Provenienza.SelectedIndex =
                                                        Cmb_Provenienza.Items.IndexOf(
                                                            Cmb_Provenienza.Items.FindByValue(
                                                                CStr(Id_Magazzino) & "|" & CStr(Sa_Cod)))


                                                            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Txt_DataMovimento.Text, AGRODATAFINE)

                                                            AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(
                                                                            Me.cmb_Udm,
                                                                            Num_Totale,
                                                                            xPiva,
                                                                            xSa_Cod,
                                                                            Id_Magazzino,
                                                                            CAU_SCARICO,
                                                                            Elem_Cod,
                                                                            False,
                                                                            Pro_Cod,
                                                                            Mat_Cod,
                                                                            True, "", "",
                                                                            "", 2, "", "",
                                                                            objParametri_Server, objParametri_Utenti)

                                                            objParametri_Server.ResettaFinestra()

                                                            'seleziono l'unita di misura specifica
                                                            cmb_Udm.SelectedIndex =
                                                        cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                                                            Me.cmb_Udm.Enabled = True

                                                        End If

                                                        If Agenda.Movimenti.Count > 1 Then

                                                            If Agenda.Movimenti(1).Movimenti_Dettagli IsNot Nothing AndAlso
                                                               Agenda.Movimenti(1).Movimenti_Dettagli.Count > 0 AndAlso
Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing AndAlso
                                                               Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then

                                                                Sa_Cod = Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Sa_Cod
                                                                Id_Magazzino = Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Id_Destinazione

                                                                Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                                clc.Fabbricati(Me.Cmb_Destinazione,
                                                                   True, "", "",
                                                                    xPiva,
                                                                  0,
                                                                   0,
                                                                   FABBRICATI_NO_STALLE,
                                                                   True,
                                                                   "",
                                                                   " Fabbricati.Fabbricato_Des ",
                                                                   AGRODATAFINE,
                                                                   objParametri_Server)


                                                                Cmb_Destinazione.SelectedIndex =
                                                             Cmb_Destinazione.Items.IndexOf(Cmb_Destinazione.Items.FindByValue(
                                                                 CStr(Id_Magazzino) & "|" & CStr(Sa_Cod)))

                                                            End If

                                                        End If


                                                    Case CAU_CARICO

                                                        If Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing AndAlso Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then

                                                            Sa_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Sa_Cod
                                                            Id_Magazzino = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Id_Destinazione

                                                            Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                            clc.Fabbricati(Me.Cmb_Destinazione,
                                                               True, "", "",
                                                                xPiva,
                                                              0,
                                                               0,
                                                               FABBRICATI_NO_STALLE,
                                                               True,
                                                               "",
                                                               " Fabbricati.Fabbricato_Des ",
                                                               AGRODATAFINE,
                                                               objParametri_Server)


                                                            Cmb_Destinazione.SelectedIndex =
                                                         Cmb_Destinazione.Items.IndexOf(Cmb_Destinazione.Items.FindByValue(
                                                             CStr(Id_Magazzino) & "|" & CStr(Sa_Cod)))

                                                        End If

                                                        If Agenda.Movimenti.Count > 1 Then

                                                            If Agenda.Movimenti(1).Movimenti_Dettagli IsNot Nothing AndAlso
                                                        Agenda.Movimenti(1).Movimenti_Dettagli.Count > 0 AndAlso
Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing AndAlso
                                                        Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then

                                                                Sa_Cod = Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Sa_Cod
                                                                Id_Magazzino = Agenda.Movimenti(1).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Id_Destinazione

                                                                Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                                clc.Fabbricati(Me.Cmb_Provenienza,
                                                                         True, "", "",
                                                                          xPiva,
                                                                        0,
                                                                         0,
                                                                         FABBRICATI_NO_STALLE,
                                                                         True,
                                                                         "",
                                                                         " Fabbricati.Fabbricato_Des ",
                                                                         AGRODATAFINE,
                                                                         objParametri_Server)

                                                                'Imposto la posizione nella combo 
                                                                Cmb_Provenienza.SelectedIndex =
                                                            Cmb_Provenienza.Items.IndexOf(
                                                                Cmb_Provenienza.Items.FindByValue(
                                                                    CStr(Id_Magazzino) & "|" & CStr(Sa_Cod)))


                                                                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Txt_DataMovimento.Text, AGRODATAFINE)

                                                                AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(
                                                                                Me.cmb_Udm,
                                                                                Num_Totale,
                                                                                xPiva,
                                                                                xSa_Cod,
                                                                                Id_Magazzino,
                                                                                CAU_SCARICO,
                                                                                Elem_Cod,
                                                                                False,
                                                                                Pro_Cod,
                                                                                Mat_Cod,
                                                                                True, "", "",
                                                                                "", 2, "", "",
                                                                                objParametri_Server, objParametri_Utenti)

                                                                objParametri_Server.ResettaFinestra()

                                                                'seleziono l'unita di misura specifica
                                                                cmb_Udm.SelectedIndex =
                                                            cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                                                                Me.cmb_Udm.Enabled = True

                                                            End If

                                                        End If

                                                End Select


                                                'FINE TRASFERIMENTO

                                            Case Else
                                                'TUTTE LE OPERAZIONI TRANNE IL TRASFERIMENTO

                                                If Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing AndAlso Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then

                                                    Sa_Cod = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Sa_Cod
                                                    Id_Magazzino = Agenda.Movimenti(z).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Id_Destinazione

                                                    If Cau_Mov.Trim = CAU_CARICO Then

                                                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                        clc.Fabbricati(Me.Cmb_Destinazione,
                                                                                    True, "", "",
                                                                                        xPiva,
                                                                                    Sa_Cod,
                                                                                    0,
                                                                                    FABBRICATI_NO_STALLE,
                                                                                    True,
                                                                                    "",
                                                                                    " Fabbricati.Fabbricato_Des ",
                                                                                    AGRODATAFINE,
                                                                                    objParametri_Server)

                                                        'CARICO
                                                        Cmb_Destinazione.SelectedIndex =
                                                    Cmb_Destinazione.Items.IndexOf(Cmb_Destinazione.Items.FindByValue(
                                                        CStr(Id_Magazzino) & "|" & CStr(Sa_Cod)))

                                                    Else

                                                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                                                        clc.Fabbricati(Me.Cmb_Provenienza,
                                                                                    True, "", "",
                                                                                    xPiva,
                                                                                    Sa_Cod,
                                                                                    0,
                                                                                    FABBRICATI_NO_STALLE,
                                                                                    True,
                                                                                    "",
                                                                                    " Fabbricati.Fabbricato_Des ",
                                                                                    AGRODATAFINE,
                                                                                    objParametri_Server)

                                                        'SCARICO
                                                        Cmb_Provenienza.SelectedIndex =
                                                    Cmb_Provenienza.Items.IndexOf(Cmb_Provenienza.Items.FindByValue(
                                                        CStr(Id_Magazzino) & "|" & CStr(Sa_Cod)))

                                                    End If


                                                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Txt_DataMovimento.Text, AGRODATAFINE)

                                                    AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(
                                                                    Me.cmb_Udm,
                                                                    Num_Totale,
                                                                    xPiva,
                                                                    xSa_Cod,
                                                                    Id_Magazzino,
                                                                    Cau_Mov,
                                                                    Elem_Cod,
                                                                    False,
                                                                    Pro_Cod,
                                                                    Mat_Cod,
                                                                    True, "", "",
                                                                    "", 2, "", "",
                                                                    objParametri_Server, objParametri_Utenti)

                                                    objParametri_Server.ResettaFinestra()

                                                    'seleziono l'unita di misura specifica
                                                    cmb_Udm.SelectedIndex = cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                                                    Me.cmb_Udm.Enabled = True

                                                    If Elem_Cod = FERTILIZZANTI Then

                                                        ' carico la combo se non l'ho già caricata
                                                        If cmb_PUARegolamenti.Items.Count = 0 Then
                                                            CaricaCombo_Regolamenti()
                                                        End If

                                                        If Extra_int <> 0 Then

                                                            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Txt_DataMovimento.Text, AGRODATAFINE)

                                                            AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(
                                                                            Me.cmb_Udm,
                                                                            Num_Totale,
                                                                            xPiva,
                                                                            xSa_Cod,
                                                                            Id_Magazzino,
                                                                            Cau_Mov,
                                                                            Elem_Cod,
                                                                            False,
                                                                            Pro_Cod,
                                                                            Mat_Cod,
                                                                            True, "", "",
                                                                            "", 2, "", "",
                                                                            objParametri_Server, objParametri_Utenti)


                                                            objParametri_Server.ResettaFinestra()

                                                            'seleziono l'unita di misura specifica
                                                            cmb_Udm.SelectedIndex =
                                                        cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Extra_int))

                                                            Select Case Extra_int
                                                                Case enum_UnitaMisura.Quintali
                                                                    Qta = Qta / 100
                                                                Case enum_UnitaMisura.Tonnellate
                                                                    Qta = Qta / 1000
                                                                Case enum_UnitaMisura.Metri_Cubi
                                                                    Qta = Qta / 1000
                                                            End Select

                                                            Txt_Quantita.Text = Qta

                                                        End If

                                                        Me.cmb_Udm.Enabled = True

                                                    End If

                                                End If

                                        End Select


                                End Select


                            End If

                        End If 'controllo sul trasferimento

                    End If 'cau_mov 4000

                Next 'movimenti

            End If

        End If


    End Sub


    '########################################################
    ''' <summary>
    ''' Usa l'XML, CHIAMATA IN CASO DI MODIFICA, QUANDO SI PROVIENE DA UN DOCUMENTO
    ''' </summary>
    Private Sub Ripristina_DETTAGLIO_nei_Controlli(ByVal operazione As enum_TipoOperazioneDB)

        Dim vet_XML_To_FormProdotto As String()

        Dim Str_MovDettaglio As String
        Dim Str_Contatto As String

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        vet_XML_To_FormProdotto = Session("vet_XML_To_FormProdotto")


        If Not IsNothing(vet_XML_To_FormProdotto) Then

            For i As Integer = 1 To vet_XML_To_FormProdotto.Length - 1


                Select Case i

                    Case 0 'chiave del DataGrid

                        '==============================================

                    Case 1  'TUTTO IL RIPRISTINA

                        Str_MovDettaglio = vet_XML_To_FormProdotto(i)

                        If Str_MovDettaglio <> "" AndAlso Str_MovDettaglio <> "&nbsp;" Then

                            Ripristina_Dettaglio_Movimento(Str_MovDettaglio, operazione)

                        Else
                            'i18n
                            Throw New Exception("Ripristina_DETTAGLIO_nei_Controlli: dettaglio prodotto non pervenuto.")

                        End If



                        '==============================================

                    Case 2 'GESTIRE IL MAGAZZINO DEL CONTATTO

                        Str_Contatto = vet_XML_To_FormProdotto(i)

                        If Str_Contatto <> "" AndAlso Str_Contatto <> "&nbsp;" Then

                            Ripristina_Dettaglio_Contatto(Str_Contatto)

                        Else
                            'If Qs_CodContatto <> "" Then
                            '    Throw New Exception("Ripristina_DETTAGLIO_nei_Controlli: dettagli del contatto non pervenuti.")
                            'End If
                        End If


                        '==============================================

                    Case Else

                        'IMPOSSIBLE

                End Select

            Next

        End If

    End Sub

    '########################################################
    ' IMPOSTANDO IL MAGAZZINO DEL CONTATTO
    Private Sub Ripristina_Dettaglio_Contatto(ByVal Str_Contatto As String)

        Dim XmlDoc As New XmlDocument

        Dim XML_MovimentoDettaglio As XmlElement
        Dim XML_MovimentoDestinazione As XmlElement

        Dim c_Piva As String
        Dim c_Id_Magazzino As Integer
        Dim c_Sa_Cod As Integer

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Str_Contatto)

        '-----------------------------------------------------------------
        '------------------------- DETTAGLI ------------------------------
        '-----------------------------------------------------------------

        XML_MovimentoDettaglio = XmlDoc.SelectSingleNode("Movimento_Dettaglio")

        XML_MovimentoDestinazione = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Destinazione")

        c_Id_Magazzino = XML_MovimentoDestinazione.GetAttribute("id_destinazione")
        c_Sa_Cod = XML_MovimentoDestinazione.GetAttribute("sa_cod")
        c_Piva = XML_MovimentoDestinazione.GetAttribute("piva")

        '-----------------------------------------------------------------
        '----------- PANNELLO MAGAZZINI --- MOV DESTINAZIONE --------------
        '-----------------------------------------------------------------

        Dim Num_Totale As Integer = 0

        Select Case xCaricoScarico

            Case CAU_SCARICO '---> destinazione del contatto

                ''Carico nella combo provenienza tutti i magazzini del centro aziendale
                'CaricaCombo_Magazzini2( _
                '                Server, Session, Page, _
                '                Me.Cmb_Destinazione, c_Piva, FABBRICATI_NO_STALLE, c_Sa_Cod)

                'Call CaricaCombo_Fabbricati(Server, Session, Page, _
                '                                Me.Cmb_Destinazione, _
                '                                Num_Totale, _
                '                                c_Piva, _
                '                                c_Sa_Cod, _
                '                                FABBRICATI_NO_STALLE, _
                '                                , , , , , , _
                '                                " ORDER BY Fabbricati.Fabbricato_Des ")

                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.Fabbricati(Me.Cmb_Destinazione,
                                   True, "", "",
                                    c_Piva,
                                  c_Sa_Cod,
                                   0,
                                   FABBRICATI_NO_STALLE,
                                   True,
                                   "",
                                   " Fabbricati.Fabbricato_Des ",
                                   AGRODATAFINE,
                                   objParametri_Server)

                If Me.Cmb_Destinazione.Items.Count < 2 Then
                    Throw New Exception(DirectCast(GetLocalResourceObject("Ripristina_Dettaglio_Contatto_NecessarioUnMagazzinoPerOperazioneDiScarico_"), String) & Qs_RagSocContatto)
                Else

                    'Imposto la posizione nella combo 
                    Me.Cmb_Destinazione.SelectedIndex =
                        Me.Cmb_Destinazione.Items.IndexOf(
                            Me.Cmb_Destinazione.Items.FindByValue(
                                c_Id_Magazzino & "|" & c_Sa_Cod))

                    Me.Cmb_Destinazione.Enabled = False
                    Me.Cmb_Destinazione.Visible = True
                    Me.lbl_MagDestinazione.Visible = True
                    Me.lbl_giacenza2_mag.Visible = True

                End If



            Case CAU_CARICO '---> provenienza del contatto

                ''Carico nella combo provenienza tutti i magazzini del centro aziendale
                'CaricaCombo_Magazzini2( _
                '                Server, Session, Page, _
                '                Me.Cmb_Provenienza, c_Piva, FABBRICATI_NO_STALLE, c_Sa_Cod)

                'Call CaricaCombo_Fabbricati(Server, Session, Page, _
                '                                Me.Cmb_Provenienza, _
                '                                Num_Totale, _
                '                                c_Piva, _
                '                                c_Sa_Cod, _
                '                                FABBRICATI_NO_STALLE, _
                '                                , , , , , , _
                '                                " ORDER BY Fabbricati.Fabbricato_Des ")

                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.Fabbricati(Me.Cmb_Provenienza,
                                   True, "", "",
                                    c_Piva,
                                  c_Sa_Cod,
                                   0,
                                   FABBRICATI_NO_STALLE,
                                   True,
                                   "",
                                   " Fabbricati.Fabbricato_Des ",
                                   AGRODATAFINE,
                                   objParametri_Server)

                If Me.Cmb_Provenienza.Items.Count < 2 Then
                    Throw New Exception(DirectCast(GetLocalResourceObject("Ripristina_Dettaglio_Contatto_NecessarioUnMagazzinoPerOperazioneDiScarico_"), String) & Qs_RagSocContatto)
                Else

                    'Imposto la posizione nella combo 
                    Me.Cmb_Provenienza.SelectedIndex =
                        Me.Cmb_Provenienza.Items.IndexOf(
                            Me.Cmb_Provenienza.Items.FindByValue(
                                c_Id_Magazzino & "|" & c_Sa_Cod))

                    Me.Cmb_Provenienza.Enabled = False
                    Me.Cmb_Provenienza.Visible = True
                    Me.Chk_MovimentoMag.Visible = True
                    Me.lbl_MagProvenienza.Visible = True
                    Me.lbl_giacenza1_mag.Visible = True

                End If

        End Select

    End Sub


    '########################################################
    ''' <summary>
    ''' Usa l'XML, CARICA LA FORM PRODOTTO CON I DATI DEL DETTAGLIO (PRODOTTO) E IMPOSTA IL MAGAZZINO DELL'IMPRESA
    ''' </summary>
    Private Sub Ripristina_Dettaglio_Movimento(ByVal Str_MovDettaglio As String, ByVal operazione As enum_TipoOperazioneDB)

        Dim XmlDoc As New XmlDocument

        Dim XML_MovimentoDettaglio As XmlElement
        Dim XML_MovimentoDestinazione As XmlElement
        Dim XML_MovimentoTecnico As XmlElement

        Dim Lav_Cod As Integer
        Dim Elem_Cod As Integer
        Dim Pro_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Id_Magazzino As Integer
        Dim Cau_Mov As String
        Dim Qta As Decimal
        Dim Udm_Cod As Integer
        Dim Prezzo_Unitario As Decimal
        Dim Prezzo_Unitario_Netto As Decimal
        Dim Lotto As String
        Dim Cod_Progetto As Integer
        Dim Cal_Cod As Integer
        Dim Pendente As String

        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Cu As Decimal = 0
        Dim Regolamento_Cod_Ferti As Integer = 0

        Dim Spe_Des As String
        Dim Ipro_Des As String
        Dim Raz_Des As String
        Dim Cat_Des As String

        Dim Metodo_Produzione_Des As String

        Dim visualizzaDettFert As Boolean = False

        Dim Rif_Esterno As String = ""
        Dim Rif_Esterno_2 As String = ""

        Dim Num_Totale As Integer = 0

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Str_MovDettaglio)

        '-----------------------------------------------------------------
        '------------------------- DETTAGLI ------------------------------
        '-----------------------------------------------------------------

        XML_MovimentoDettaglio = XmlDoc.SelectSingleNode("Movimento_Dettaglio")


        '-----------------------------------------------------------------
        '--------------------- PANNELLO MOVIMENTO ------------------------
        '-----------------------------------------------------------------

        Rif_Esterno = Agro_XML_GetString(XML_MovimentoDettaglio, "rif_esterno", "")
        Rif_Esterno_2 = Agro_XML_GetString(XML_MovimentoDettaglio, "rif_esterno_2", "")

        Ripristina_DettagliNascosti(Rif_Esterno, Rif_Esterno_2)

        Me.Txt_DataMovimento.Text = CStr(Qs_DataSelezionata)
        'Me.Txt_Ora.Text = Format(CDate(Qs_DataSelezionata), "HH:mm")
        'If Me.Txt_Ora.Text = "00.00" Then
        Me.Txt_Ora.Text = "12" & gettimesep() & "00"
        'End If

        CaricaCombo_Causale(Qs_Lav_Cod)
        Pendente = XML_MovimentoDettaglio.GetAttribute("pendente")
        Me.cmb_Causale.SelectedIndex = CInt(Me.cmb_Causale.Items.IndexOf(Me.cmb_Causale.Items.FindByValue(Pendente)))


        Cau_Mov = XML_MovimentoDettaglio.GetAttribute("cau_mov")

        Elem_Cod = XML_MovimentoDettaglio.GetAttribute("elem_cod")
        Pro_Cod = XML_MovimentoDettaglio.GetAttribute("pro_cod")
        Mat_Cod = XML_MovimentoDettaglio.GetAttribute("mat_cod")
        Cod_Progetto = XML_MovimentoDettaglio.GetAttribute("cod_progetto")
        Cal_Cod = XML_MovimentoDettaglio.GetAttribute("cal_cod")
        Lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
        Udm_Cod = XML_MovimentoDettaglio.GetAttribute("udm_cod")


        '-----------------------------------------------------------------
        '--------------------- QUANTITA' E PREZZO ------------------------
        '-----------------------------------------------------------------

        Qta = XML_MovimentoDettaglio.GetAttribute("qta")
        Qta_Modifica_Giacenza = Qta

        Prezzo_Unitario = XML_MovimentoDettaglio.GetAttribute("prezzo_unitario")
        Prezzo_Unitario_Netto = XML_MovimentoDettaglio.GetAttribute("prezzo_unitario_netto")



        '-----------------------------------------------------------------
        '--------------------- CATEGORIA PRODOTTO ------------------------
        '-----------------------------------------------------------------


        'Dim Flag_NoSemilavorati As Boolean = True
        Dim Flag_NoSemilavorati As Boolean = False

        'Select Case Operazione
        '    Case enum_TipoOperazioneDB.Lettura
        '        Flag_NoSemilavorati = False
        '    Case enum_TipoOperazioneDB.Modifica
        '        Flag_NoSemilavorati = True
        'End Select

        Call AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(
                                        Me.cmb_Categoria,
                                        True, "", "",
                                        0,
                                         xCaricoScarico,
                                         0,
                                         Flag_NoSemilavorati,
                                         True,
                                         "", "", objParametri_Server)
        Num_Totale = cmb_Categoria.Items.Count - 1


        '' ''ATTENZIONE, LO REIMPOSTA A ZERO NEL CASO DI MODIFICA DDT EMESSO, 
        'anche la combo prodotto viene azzerata, conviene gestire il caso in
        '' ''lo fa in Carica_Magazzini()->Cmb_Provenienza_SelectedIndexChanged() perché controlla If xCaricoScarico <> CAU_CARICO  e chiama Pulisci_ChiaveProdotto() 
        'ho l'elem_cod -> imposto la combo delle categorie magazzino
        'Imposto la selezione della combobox
        Me.cmb_Categoria.SelectedIndex =
            Me.cmb_Categoria.Items.IndexOf(Me.cmb_Categoria.Items.FindByValue(Elem_Cod))

        Lotto_ElemCod_RipristinaDettaglio(Elem_Cod, Lotto, Pro_Cod, Mat_Cod, True)

        Select Case Elem_Cod

            Case SERVIZI

                'Bene Non Movimentabile           
                Id_Magazzino = 0

                Me.Pannello_Magazzini.Visible = False
                Me.lbl_magazzini.Visible = False

                Me.Txt_BeniStrumentali.Visible = False
                Me.cmb_Prodotti.Visible = True
                'COM: CaricaCombo_Servizi(Server, Session, Page, Me.cmb_Prodotti)
                AgronicaCoreUtility.CaricaListControl.CaricaServizi(Me.cmb_Prodotti, False, "", "", "", "", "", objParametri_Server)

                Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                Me.Chk_MovimentoMag.Checked = False

                'CaricaCombo_Udm(Server, Session, Page, Me.cmb_Udm)

                'il cau_mov è quello del carico, così va a leggere in CategorieXUnitaMisura


                AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                                            Me.cmb_Udm,
                                            Num_Totale,
                                            "",
                                            0,
                                            0,
                                            CAU_CARICO,
                                            0,
                                            False,
                                            0,
                                            0,
                    True, "", "",
                    "", "", "", objParametri_Server, objParametri_Utenti)

                'seleziono l'unita di misura specifica
                cmb_Udm.SelectedIndex =
                    cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                Me.cmb_Udm.Enabled = True


            Case ALTRI_BENI, 1

                'Bene Non Movimentabile           
                Id_Magazzino = 0

                Me.Pannello_Magazzini.Visible = False
                Me.lbl_magazzini.Visible = False

                Me.Txt_BeniStrumentali.Text = XML_MovimentoDettaglio.GetAttribute("mov_det_des")
                Me.cmb_Prodotti.Visible = False
                Me.Txt_BeniStrumentali.Visible = True

                Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                Me.Chk_MovimentoMag.Checked = False

                'CaricaCombo_Udm(Server, Session, Page, Me.cmb_Udm)

                'il cau_mov è quello del carico, così va a leggere in CategorieXUnitaMisura

                AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                                          Me.cmb_Udm,
                                            Num_Totale,
                                            "",
                                            0,
                                            0,
                                            CAU_CARICO,
                                            0,
                                            False,
                                            0,
                                            0,
                                True, "", "",
                                "", "", "", objParametri_Server, objParametri_Utenti)

                'seleziono l'unita di misura specifica
                cmb_Udm.SelectedIndex =
                    cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                Me.cmb_Udm.Enabled = True


            Case ZOO_CONSISTENZA

                'DA PROVARE!!!
                'E' STATO COPIATO E BASTA

                Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoStallaAbbr"), String)
                Me.Chk_MovimentoMag.Checked = True

                CaricaGriglia_Matricole()

                '-----------------------------------------------------------------
                '-------------- PANNELLO MAGAZZINI --- DESTINAZIONE --------------
                '-----------------------------------------------------------------


                XML_MovimentoDestinazione = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Destinazione")

                Id_Magazzino = XML_MovimentoDestinazione.GetAttribute("id_destinazione")

                Carica_Magazzini(STALLA, "", xPiva, xSa_Cod, Id_Magazzino)


                '-----------------------------------------------------------------
                '-------------- PANNELLO ANIMALI   -------------------------------
                '-----------------------------------------------------------------

                Dim ObjAnimale_Anagrafe_R As New AgronicaCoreZooDAL.Zoo_Animali_R
                Dim RsAnimale_Anagrafe_R As DataTable


                RsAnimale_Anagrafe_R = ObjAnimale_Anagrafe_R.Leggi(CStr(xPiva),
                                                   0,
                                                   CInt(Cod_Progetto),
                                                   "", 0, 0, 0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)



                If RsAnimale_Anagrafe_R.Rows.Count <> 0 Then
                    Dim hh As Integer = 0

                    Spe_Des = StallaSpecieDes_from_StallaSpecieCod2(
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Gen_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Spe_Cod")),
                                                    objParametri_Server)

                    Ipro_Des = StallaIProDes_from_StallaIProCod2(
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Gen_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Spe_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Ipro_Cod")),
                                                    objParametri_Server)

                    Cat_Des = StallaCategoriaDes_from_StallaCategoriaCod2(
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Gen_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Spe_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Ipro_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Cat_Cod")),
                                                    objParametri_Server)

                    Raz_Des = StallaRazzaDes_from_StallaRazzaCod2(
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Gen_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Spe_Cod")),
                                                    CInt(RsAnimale_Anagrafe_R.Rows(hh).Item("Raz_Cod")),
                                                    objParametri_Server)

                    Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale

                    Select Case RsAnimale_Anagrafe_R.Rows(hh).Item("metodo_produzione")
                        Case 1
                            Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale
                        Case 2
                            Metodo_Produzione_Des = AgronicaAgenda_2010.InConversione
                        Case 3
                            Metodo_Produzione_Des = AgronicaAgenda_2010.Biologico
                    End Select

                    Inserisci_Riga_Matricola(Cod_Progetto,
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Gen_Cod"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Spe_Cod"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Ipro_Cod"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Raz_Cod"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Cat_Cod"),
                                            Spe_Des,
                                            Ipro_Des,
                                            Raz_Des,
                                            Cat_Des,
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Matricola"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Nome"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("Collare"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("nome_aia"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("matricola_aia"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("dat_nascita"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("prov_nascita"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("stato_nascita"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("aua_azi_nascita"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("ausl_azi_nascita"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("sesso"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("mat_padre"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("mat_madre"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("peso"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("data_pesa"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("metodo_produzione"),
                                            Metodo_Produzione_Des,
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("conversione_inizio"),
                                            RsAnimale_Anagrafe_R.Rows(hh).Item("conversione_fine"),
                                            Qta,
                                            Prezzo_Unitario)

                    CType(DataGrid_ZooAnimali.Items(0).FindControl("txtqta"), TextBox).Text = Qta

                    CType(DataGrid_ZooAnimali.Items(0).FindControl("txtprezzo"), TextBox).Text = Prezzo_Unitario

                End If

            Case Else

                XML_MovimentoDestinazione = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Destinazione")

                Id_Magazzino = XML_MovimentoDestinazione.GetAttribute("id_destinazione")

                xSa_Cod = XML_MovimentoDestinazione.GetAttribute("sa_cod")


                'ATTENZIONE!!!!!!!
                Me.Chk_MovimentoMag.Text = DirectCast(GetLocalResourceObject("MovimentoMagazzinoAbbr"), String)
                Me.Chk_MovimentoMag.Checked = True

                Select Case Elem_Cod

                    Case MACCHINE, SEMENTI, ALTRE_MATERIE, MANGIMI, FARMACI,
                        SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                        SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI,
                        CARBURANTI


                        '-----------------------------------------------------------------
                        '----------------------- MATERIE PRIME ---------------------------
                        '-----------------------------------------------------------------
                        '21/08/2018: introdotto filtro sul mat_cod 
                        '(per caricare solo quello ed evitare di caricare la combo con tutti i prodotti di anagrafica)
                        'non serve diversificare caso carico da scarico perché entra qui solo nel caso di ddt/fatture
                        'e la "chiave della giacenza" (quindi anche il mat_cod) non è modificabile
                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                        clc.Materie_Prime(cmb_Prodotti,
                                    True,
                                 "",
                               "",
                               Cau_Mov,
                               xPiva,
                               xSa_Cod,
                               Id_Magazzino,
                               Elem_Cod,
                               True,
                               Me.Txt_CercaProdotto.Text,
                               Me.Txt_CercaLotto.Text,
                               Me.Txt_CercaCodArticolo.Text,
                               "",
                               Mat_Cod,
                               0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO, 0, 0, 0, 0, 0, 0, 0,
                               CDate(Txt_DataMovimento.Text).ToShortDateString,
                               "", "", objParametri_Server, objParametri_Utenti
                                )

                        Num_Totale = cmb_Prodotti.Items.Count - 1

                        cmb_Prodotti.SelectedIndex =
                                cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(-Mat_Cod))


                        Select Case Elem_Cod

                            Case SEMILAVORATI_VEGETALI

                                If Cod_Progetto <> 0 Then

                                    '-----------------------------------------------------------------
                                    '---------------- SEMILAVORATI:  LOTTI ---------------------------
                                    '----------------------------------------------------------------

                                    AgronicaCoreUtility.CaricaListControl.Semilavorati_LottoInterno(Me.cmb_Lotto,
                                                        Num_Totale,
                                                        xPiva,
                                                        xSa_Cod,
                                                        Id_Magazzino,
                                                        Mat_Cod,
                                                        True, "", "",
                                                        "",
                                                        CDate(Txt_DataMovimento.Text),
                                                        Cau_Mov,
                                                        "", "", objParametri_Server, objParametri_Utenti)

                                    Me.cmb_Lotto.Enabled = True


                                    cmb_Lotto.SelectedIndex =
                                        cmb_Lotto.Items.IndexOf(cmb_Lotto.Items.FindByValue(Cod_Progetto))

                                Else

                                    '-----------------------------------------------------------------
                                    '---------------- SEMILAVORATI:  LOTTI ---------------------------
                                    '-----------------------------------------------------------------

                                    Me.cmb_Lotto.Items.Add(New ListItem("", ""))
                                    Me.cmb_Lotto.Enabled = True

                                    cmb_Lotto.SelectedIndex =
                                        cmb_Lotto.Items.IndexOf(cmb_Lotto.Items.FindByValue(Cod_Progetto))

                                End If


                                AgronicaCoreUtility.CaricaListControl.Semilavorati_ParametriQualitativi(
                                                    Me.cmb_Calibro,
                                                    Num_Totale,
                                                    xPiva,
                                                    xSa_Cod,
                                                    Id_Magazzino,
                                                    Mat_Cod,
                                                    Cod_Progetto,
                                                    Lotto,
                                                    True, "", "",
                                                    CDate(Txt_DataMovimento.Text),
                                                    "", "", objParametri_Server, objParametri_Utenti)




                                cmb_Calibro.SelectedIndex =
                                    cmb_Calibro.Items.IndexOf(cmb_Calibro.Items.FindByValue(Cal_Cod))

                        End Select

                    Case FERTILIZZANTI, FORMULATI, COADIUVANTI, INSETTI, TRAPPOLE, INNESCHI 'CARBURANTI


                        '-----------------------------------------------------------------
                        '------------------------- PRODOTTI ------------------------------
                        '-----------------------------------------------------------------

                        Dim Flag_VisualizzaProCod As Boolean = True

                        If Me.cmb_Categoria.SelectedItem.Value = CARBURANTI Then
                            Flag_VisualizzaProCod = False
                        End If

                        Dim Flag_CaricaUdmCod As Boolean = False

                        'If Me.cmb_Categoria.SelectedItem.Value = FORMULATI Then
                        '    Flag_CaricaUdmCod = True
                        'End If

                        Dim Flag_LeggiGiacenze As Boolean = False
                        Dim Flag_FiltraRevocati As Boolean = True
                        If Cau_Mov = CAU_SCARICO Then
                            Flag_LeggiGiacenze = True
                            Flag_FiltraRevocati = False
                        End If

                        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
                        clc.Prodotti(Me.cmb_Prodotti,
                                          True,
                                          "",
                                          "",
                                          Cau_Mov,
                                          xPiva,
                                          xSa_Cod,
                                          Id_Magazzino,
                                          Elem_Cod,
                                          False,
                                          "",
                                          Flag_VisualizzaProCod,
                                          Flag_CaricaUdmCod,
                                          Pro_Cod, 0,
                                          Me.Txt_DataMovimento.Text,
                                          0,
                                          0,
                                          Flag_LeggiGiacenze,
                                          Flag_FiltraRevocati,
                                          "", "", objParametri_Server, objParametri_Utenti)

                        Num_Totale = Me.cmb_Prodotti.Items.Count - 1

                        Select Case Me.cmb_Categoria.SelectedItem.Value

                            Case FERTILIZZANTI

                                If Mat_Cod <> 0 Then
                                    cmb_Prodotti.SelectedIndex =
                                       cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(
                                           -Mat_Cod))
                                Else
                                    cmb_Prodotti.SelectedIndex =
                                       cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(
                                           Pro_Cod))
                                End If

                                XML_MovimentoTecnico = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Dettaglio_Tecnico_2")

                                If XML_MovimentoTecnico IsNot Nothing AndAlso Cau_Mov = CAU_CARICO Then

                                    '-----------------------------------------------------------------
                                    '----------------------- DETTAGLI FERTILIZZANTE ------------------
                                    '-----------------------------------------------------------------

                                    visualizzaDettFert = True

                                    N = If(Not XML_MovimentoTecnico.HasAttribute("n"), 0, XML_MovimentoTecnico.GetAttribute("n"))
                                    P2O5 = If(Not XML_MovimentoTecnico.HasAttribute("p"), 0, XML_MovimentoTecnico.GetAttribute("p"))
                                    K2O = If(Not XML_MovimentoTecnico.HasAttribute("k"), 0, XML_MovimentoTecnico.GetAttribute("k"))
                                    Cu = If(Not XML_MovimentoTecnico.HasAttribute("cu"), 0, XML_MovimentoTecnico.GetAttribute("cu"))
                                    Regolamento_Cod_Ferti = Agro_XML_GetInteger(XML_MovimentoTecnico, "extra_int", 0)

                                    Ripristina_DettagliFertilizzante(N, P2O5, K2O, Cu, Regolamento_Cod_Ferti)

                                End If

                            Case Else

                                For i As Integer = 0 To cmb_Prodotti.Items.Count - 1
                                    If InStr(cmb_Prodotti.Items(i).Value, "|") <> 0 Then
                                        If InStr(cmb_Prodotti.Items(i).Value, Pro_Cod.ToString & "|") <> 0 Then
                                            cmb_Prodotti.SelectedIndex = i
                                            Exit For
                                        End If
                                    Else
                                        If cmb_Prodotti.Items(i).Value = Pro_Cod.ToString Then
                                            cmb_Prodotti.SelectedIndex = i
                                            Exit For
                                        End If
                                    End If

                                Next
                                'cmb_Prodotti.SelectedIndex = _
                                '   cmb_Prodotti.Items.IndexOf(cmb_Prodotti.Items.FindByValue(Pro_Cod))

                        End Select


                End Select



                '-----------------------------------------------------------------
                '-------------- PANNELLO MAGAZZINI --- DESTINAZIONE --------------
                '-----------------------------------------------------------------

                Carica_Magazzini(FABBRICATI_NO_STALLE, "", xPiva, xSa_Cod, Id_Magazzino)

                Me.Txt_Quantita.Text = Qta
                Me.Txt_PrezzoUnitario.Text = Prezzo_Unitario
                Me.Txt_PrezzoUnitario_Netto.Text = Prezzo_Unitario_Netto

                'AgronicaCoreUtility.CaricaListControl.Udm_Optimize( _
                '              Me.cmb_Udm, _
                '                    Num_Totale, _
                '                    xPiva, _
                '                    xSa_Cod, _
                '                    Id_Magazzino, _
                '                    Cau_Mov, _
                '                    Elem_Cod, _
                '                    False, _
                '                    Pro_Cod, _
                '                    Mat_Cod, _
                '                True, "", "", _
                '                "", "", "", objParametri_Server)

                Dim RegolamentoCod As Integer = 0
                If cmb_PUARegolamenti.Visible Then
                    RegolamentoCod = cmb_PUARegolamenti.SelectedItem.Value.Split(SEP_PuaReg)(0)
                End If

                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Txt_DataMovimento.Text, AGRODATAFINE)

                AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(
                                         Me.cmb_Udm,
                                         Num_Totale,
                                         xPiva,
                                         xSa_Cod,
                                         Id_Magazzino,
                                         Cau_Mov,
                                         Elem_Cod,
                                         True,
                                         Pro_Cod,
                                         Mat_Cod,
                                         True, "", "", "",
                                         RegolamentoCod, "", "",
                                         objParametri_Server, objParametri_Utenti)

                objParametri_Server.ResettaFinestra()

                'seleziono l'unita di misura specifica
                cmb_Udm.SelectedIndex = cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(Udm_Cod))

                Me.cmb_Udm.Enabled = True


                '===========================================================
                Try

                    'nel caso di fattura e ddt allegato blocco la combo della quantità e prezzo
                    If operazione = enum_TipoOperazioneDB.Modifica OrElse operazione = enum_TipoOperazioneDB.Lettura Then
                        If Qs_Lav_Cod = LAVCOD_FATTURA_RICEVUTA OrElse
                            Qs_Lav_Cod = LAVCOD_FATTURA_EMESSA OrElse
                            Qs_Lav_Cod = LAVCOD_NOTA_ACCREDITO_EMESSA OrElse
                            Qs_Lav_Cod = LAVCOD_NOTA_ACCREDITO_RICEVUTA Then

                            If Not IsNothing(XML_MovimentoDettaglio.SelectSingleNode("Movimento_Riferimento2")) Then
                                Me.Txt_Quantita.Enabled = False
                                'Me.Txt_PrezzoUnitario.Enabled = False il prezzo lo lascio modificabile
                            End If


                            If Me.Pannello_Economico.Visible Then


                                '-----------------------------------------------------------------
                                '----------------------- DETTAGLI ECONOMICI ----------------------
                                '-----------------------------------------------------------------
                                Dim Udm_Cod_Extra As Decimal = XML_MovimentoDettaglio.GetAttribute("udm_cod_extra")
                                Dim Qta_Extra As Decimal = XML_MovimentoDettaglio.GetAttribute("qta_extra")
                                Dim Prezzo_Effettivo As Decimal = XML_MovimentoDettaglio.GetAttribute("prezzo_effettivo")
                                Dim Sconto_Magg As Decimal = XML_MovimentoDettaglio.GetAttribute("sconto")
                                Dim Cod_Iva As Integer = XML_MovimentoDettaglio.GetAttribute("cod_iva")
                                Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
                                Dim Imponibile As Decimal = Math.Abs(objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDbl(XML_MovimentoDettaglio.GetAttribute("imponibile"))))
                                Dim Imponibile_Netto As Decimal = Math.Abs(objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDbl(XML_MovimentoDettaglio.GetAttribute("imponibile_netto"))))
                                Dim Iva As Decimal = Math.Abs(objContabHLP.Leggi_IVA_PositivaNegativa(Lav_Cod, CDbl(XML_MovimentoDettaglio.GetAttribute("iva"))))
                                Dim Anno As Integer = XML_MovimentoDettaglio.GetAttribute("anno")
                                Dim Ric_Cod As Integer = XML_MovimentoDettaglio.GetAttribute("ric_cod")
                                Dim Cod_Conto As Integer = XML_MovimentoDettaglio.GetAttribute("cod_conto")

                                Ripristina_DettagliEconomici(Udm_Cod_Extra,
                                                             Qta_Extra,
                                                             Prezzo_Effettivo,
                                                             Sconto_Magg,
                                                             Imponibile,
                                                             Imponibile_Netto,
                                                             Cod_Iva,
                                                             Iva,
                                                             Anno,
                                                             Ric_Cod,
                                                             Cod_Conto)

                            End If


                        End If
                    End If




                Catch ex As Exception

                End Try




        End Select


        '===========================================================

        'DISABILITO I CONTROLLI

        Me.Txt_DataMovimento.Enabled = False
        Me.Txt_Ora.Enabled = False
        Me.cmb_Causale.Enabled = False

        Me.Cmb_Provenienza.Enabled = False
        Me.Cmb_Destinazione.Enabled = False


        Me.cmb_Categoria.Enabled = False
        Me.cmb_Prodotti.Enabled = False
        Me.cmb_Lotto.Enabled = False
        Me.cmb_Calibro.Enabled = False
        Me.cmb_Udm.Enabled = False

        Me.ImgBtn_CercaProdotti.Enabled = False
        Me.ImgBtn_CercaProdotti.Visible = False
        Me.Lbl_Cerca.Visible = False
        Me.Txt_CercaProdotto.Visible = False
        Me.lbl_CercaProdotto.Visible = False

        'Me.BtnInfo.Visible = False
        Me.BtnInfo_Concime.Visible = False
        Me.BtnInfo_fito.Visible = False
        'Me.Risorse.Visible = False
        'Me.Lbl_Risorse.Visible = False
        Me.ImgBtn_NuovoProdotto.Visible = False

        cmb_PUARegolamenti.Enabled = False
        Dettaglio_Fertilizzante_Visibilita(visualizzaDettFert)
        Dettaglio_Fertilizzante_Attivo(False)

        '===========================================================

    End Sub


    '########################################################################################
    Private Sub CaricaGriglia_Prodotti()

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        'Dim Dr As DataRow
        'Dim Dt_Old As New DataTable
        'Dim i As Integer

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Pro_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lotto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Codice", GetType(String)))

        Dt.Columns.Add(New DataColumn("Piva_Destinazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("SaCod_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Destinazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Piva_Provenienza", GetType(String)))
        Dt.Columns.Add(New DataColumn("SaCod_Provenienza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Provenienza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Provenienza", GetType(String)))

        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Chiave", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Prodotto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Fase_Cod", GetType(String)))

        Dt.Columns.Add(New DataColumn("Udm_Cod_Extra", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta_Extra", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Prezzo_Unitario_Netto", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Cod_Variazione", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Variazione_Perc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Variazione", GetType(String)))

        Dt.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Aliquota", GetType(String)))
        Dt.Columns.Add(New DataColumn("Iva", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Imponibile", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Imponibile_Netto", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Prezzo_Effettivo", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Anno", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ric_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Conto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Conto", GetType(String)))

        Dt.Columns.Add(New DataColumn("Extra_Int", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("N", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P2O5", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K2O", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Cu", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Regolamento_Cod_Ferti", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Rif_Esterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("Rif_Esterno_2", GetType(String)))

        Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Chiave")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Associo il DataTable con la DataGrid

        Me.DataGrid_Prodotti.DataSource = Dt
        Me.DataGrid_Prodotti.DataBind()


        ''se avevo salvato dei dati nella sessione, li ripristino e svuoto la sessione
        'If Not Session("vs_dtCarichi") Is Nothing Then

        '    '????????????????????????

        '    'Session("vs_dtCarichi") non è inizializzata da nessuna parte

        '    Dt_Old = Session("vs_dtCarichi")

        '    For i = 0 To Dt_Old.Rows.Count - 1

        '        'Creo una nuova riga
        '        Dr = Dt.NewRow

        '        'Definisco i valori
        '        Dr.Item("Piva_Destinazione") = Dt_Old.Rows(i).Item("Piva_Destinazione")
        '        Dr.Item("SaCod_Destinazione") = Dt_Old.Rows(i).Item("SaCod_Destinazione")
        '        Dr.Item("Id_Destinazione") = Dt_Old.Rows(i).Item("Id_Destinazione")
        '        Dr.Item("Destinazione") = Dt_Old.Rows(i).Item("Destinazione")
        '        Dr.Item("Piva_Provenienza") = Dt_Old.Rows(i).Item("Piva_Provenienza")
        '        Dr.Item("SaCod_Provenienza") = Dt_Old.Rows(i).Item("SaCod_Provenienza")
        '        Dr.Item("Id_Provenienza") = Dt_Old.Rows(i).Item("Id_Provenienza")
        '        Dr.Item("Provenienza") = Dt_Old.Rows(i).Item("Provenienza")
        '        Dr.Item("Elem_Cod") = Dt_Old.Rows(i).Item("Elem_Cod")
        '        Dr.Item("Pro_Cod") = Dt_Old.Rows(i).Item("Pro_Cod")
        '        Dr.Item("Mat_Cod") = Dt_Old.Rows(i).Item("Mat_Cod")
        '        Dr.Item("Cal_Cod") = Dt_Old.Rows(i).Item("Cal_Cod")
        '        Dr.Item("Cod_Progetto") = Dt_Old.Rows(i).Item("Cod_Progetto")
        '        Dr.Item("Lotto") = Dt_Old.Rows(i).Item("Lotto")
        '        Dr.Item("Udm_Cod") = Dt_Old.Rows(i).Item("Udm_Cod")
        '        Dr.Item("Udm_Des") = Dt_Old.Rows(i).Item("Udm_Des")
        '        Dr.Item("Qta") = Dt_Old.Rows(i).Item("Qta")
        '        Dr.Item("Chiave") = Dt_Old.Rows(i).Item("Chiave")
        '        Dr.Item("Descrizione") = Dt_Old.Rows(i).Item("Descrizione")
        '        Dr.Item("Prezzo_Unitario") = Dt_Old.Rows(i).Item("Prezzo_Unitario")
        '        Dr.Item("Prezzo_Unitario_Netto") = Dt_Old.Rows(i).Item("Prezzo_Unitario_Netto")
        '        Dr.Item("Prodotto") = Dt_Old.Rows(i).Item("Prodotto")

        '        'Associo alla tabella la nuova riga creata
        '        Dt.Rows.Add(Dr)

        '    Next

        '    '----- Associo il DataTable con la DataGrid

        '    Me.DataGrid_Prodotti.DataSource = Dt
        '    Me.DataGrid_Prodotti.DataBind()

        '    'Me.Pannello_Prodotti.Visible = True
        '    'Me.Lbl_PannelloCarichi.Visible = True

        '    Session("vs_dtCarichi") = Nothing

        'End If

        '----- Salvo il DataTable dentro il viewstate

        ViewState("DT_Prodotti_NO_Contabili") = Dt

    End Sub


    '###################################################################################################################
    Private Sub ImgBtn_Inserisci_nel_DataGrid_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Inserisci_nel_DataGrid.Click

        If xCaricoScarico = enum_Agenda_Causali.SCARICO OrElse xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
            If Me.Cmb_Provenienza.SelectedIndex = 0 Then
                Messaggi.AgroMsgBox(AgronicaAgenda_2010.SelezionareIlMagazzinoDiProvenienza, Page, , Me.upDati)
                Exit Sub
            End If
        Else
            'CARICO
            If Me.Cmb_Destinazione.SelectedIndex = 0 Then
                Messaggi.AgroMsgBox(AgronicaAgenda_2010.SelezionareIlMagazzinoDiDestinazione, Page, , Me.upDati)
                Exit Sub
            End If
        End If


        If Me.Pannello_Economico.Visible Then

            'faccio l'aggiornamento cmq, perché può darsi che l'utente non l'abbia fatto da solo

            Dim Str_Errore As String

            If Not UsaNuoviArrotondamenti(objParametri_Server) Then
                Str_Errore = AggiornaDettagliEconomici()
            Else
                Str_Errore = AggiornaDettagliEconomiciNuoviArrotondamenti()
            End If

            If Str_Errore <> "" Then
                Messaggi.AgroMsgBox(Str_Errore, Page, , Me.upDati)
                Exit Sub
            End If

        End If


        Select Case Qs_Mode.ToLower

            Case "stalla"

                ''NUOVA GESTIONE
                'DA USARE QUANDO SI FARANNO LE MODIFICHE PER GESTIRE I DETTAGLI ECONOMICI
                'E QUANDO SI GESTIRANNO LA MODIFICA E LA CANCELLAZIONE

                Inserisci_Animale_in_Griglia()

            Case Else

                Inserisci_Prodotto_in_Griglia()

        End Select



    End Sub


    '###################################################################################################################
    Private Sub Inserisci_Animale_in_Griglia()

        'NUOVA GESTIONE
        'DA USARE QUANDO SI FARANNO LE MODIFICHE PER GESTIRE I DETTAGLI ECONOMICI
        'E QUANDO SI GESTIRANNO LA MODIFICA E LA CANCELLAZIONE

        Dim strZoo_Animale_Anagrafe As String
        Dim Metodo_Produzione_Des As String

        Dim XmlDoc As New XmlDocument
        Dim XML_Zoo_Animale_Anagrafe As XmlElement


        If Session("Zoo_Animale_Anagrafe") IsNot Nothing Then

            strZoo_Animale_Anagrafe = Session("Zoo_Animale_Anagrafe").ToString

            Session("Zoo_Animale_Anagrafe") = Nothing


            'Carico la stringa nel documento XML
            XmlDoc.LoadXml(strZoo_Animale_Anagrafe)

            '----- Tag Zoo_Animale_Anagrafe

            XML_Zoo_Animale_Anagrafe = XmlDoc.SelectSingleNode("Zoo_Animale_Anagrafe")

            Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale

            Select Case XML_Zoo_Animale_Anagrafe.GetAttribute("metodo_produzione")
                Case 1
                    Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale
                Case 2
                    Metodo_Produzione_Des = AgronicaAgenda_2010.InConversione
                Case 3
                    Metodo_Produzione_Des = AgronicaAgenda_2010.Biologico
            End Select


            Inserisci_Riga_Matricola(XML_Zoo_Animale_Anagrafe.GetAttribute("cod_progetto"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("gen_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("spe_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("ipro_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("raz_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("cat_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("spe_des"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("ipro_des"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("raz_des"),
                                    "",
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("matricola"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("nome"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("collare"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("nome_aia"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("matricola_aia"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("dat_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("prov_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("stato_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("aua_azi_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("ausl_azi_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("sesso"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("mat_padre"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("mat_madre"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("peso"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("data_pesa"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("metodo_produzione"),
                                    Metodo_Produzione_Des,
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("conversione_inizio"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("conversione_fine"),
                                    0,
                                    0)


        End If

    End Sub



    '###################################################################################################################
    Private Sub Inserisci_Prodotto_in_Griglia()

        Dim Elem_Cod As Integer
        Dim Elem_Des As String = ""
        Dim Pro_Cod As Integer
        Dim Mat_Cod As Integer
        Dim ProMat_Des As String = ""
        Dim Cod_Progetto As Integer
        Dim Fase_Cod As Integer
        Dim Progetto_Des As String = ""
        Dim LottoAccettazione As String = ""
        Dim Cal_Cod As Integer
        Dim Cal_Des As String = ""
        Dim Qta As Decimal
        Dim UdmCod As Integer
        Dim UdmDes As String = ""
        Dim Piva_Destinazione As String
        Dim SaCod_Destinazione As Integer
        Dim Id_Destinazione As Integer
        Dim Destinazione As String
        Dim Piva_Provenienza As String
        Dim SaCod_Provenienza As Integer
        Dim Id_Provenienza As Integer
        Dim Provenienza As String
        Dim Prezzo, PrezzoNetto As Decimal

        Dim Udm_Cod_Extra As Decimal = 0
        Dim Qta_Extra As Decimal = 0
        Dim Prezzo_Effettivo As Decimal = 0

        Dim Sconto_Magg As Decimal = 0
        Dim Variazione As Decimal = 0
        Dim Variazione_Perc As String = ""

        Dim Imponibile As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Cod_Iva As Integer = 0
        Dim Iva As Decimal = 0
        Dim Anno As Integer = 0
        Dim Ric_Cod As Integer = 0
        Dim Cod_Conto As Integer = 0
        Dim Conto As String = ""

        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Cu As Decimal = 0
        Dim Regolamento_Cod_Ferti As Integer = 0

        Dim array_temp As String()

        '===================================================

        Messaggio = ""

        ' CATEGORIA MAGAZZINO
        If Me.cmb_Categoria.SelectedIndex < 1 Then
            Messaggio &= DirectCast(GetLocalResourceObject("ScegliereLaCategoriaProdotto"), String) & vbCrLf
        Else
            Elem_Cod = Me.cmb_Categoria.SelectedItem.Value
            Elem_Des = Me.cmb_Categoria.SelectedItem.Text
        End If


        'MAGAZZINO DI DESTINAZIONE
        If Elem_Cod <> ALTRI_BENI AndAlso Elem_Cod <> SERVIZI AndAlso Elem_Cod <> 1 Then

            'se sono in bolla ricevuta, il caumov è carico
            'e si può muovere la cmb destinazione

            'se sono in bolla emessa, il caumov è scarico
            'e non si può muovere la cmb destinazione
            'se si muove, significa che è il magazzino del contatto
            'e non si deve gestire l'evento

            If xCaricoScarico <> CAU_SCARICO Then
                '//////////////////////////////////////////
                If Me.Cmb_Destinazione.SelectedIndex > 0 Then
                    Piva_Destinazione = ViewState("Piva")
                    Destinazione = Cmb_Destinazione.SelectedItem.Text
                    array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                    Id_Destinazione = array_temp(0)
                    SaCod_Destinazione = array_temp(1)
                Else
                    Piva_Destinazione = 0
                    SaCod_Destinazione = 0
                    Id_Destinazione = 0
                    Destinazione = AgronicaAgenda_2010.Nessuno
                End If
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            Else
                '//////////////////////////////////////////
                'E' IL CONTATTO!!!!!!!!!!!!!!!!!
                If Me.Cmb_Destinazione.SelectedIndex > 0 Then
                    Piva_Destinazione = Qs_CodContatto
                    array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                    Id_Destinazione = array_temp(0)
                    SaCod_Destinazione = array_temp(1)
                    Destinazione = Cmb_Destinazione.SelectedItem.Text
                Else
                    Piva_Destinazione = 0
                    SaCod_Destinazione = 0
                    Id_Destinazione = 0
                    Destinazione = AgronicaAgenda_2010.Nessuno
                End If
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            End If


        Else 'ALTRI_BENI - SERVIZI

            If xCaricoScarico <> CAU_SCARICO Then
                '//////////////////////////////////////////
                Piva_Destinazione = ViewState("Piva")
                SaCod_Destinazione = 0
                Id_Destinazione = 0
                Destinazione = AgronicaAgenda_2010.Nessuno
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            Else
                '//////////////////////////////////////////
                'E' IL CONTATTO!!!!!!!!!!!!!!!!!
                Piva_Destinazione = Qs_CodContatto
                SaCod_Destinazione = 0
                Id_Destinazione = 0
                Destinazione = AgronicaAgenda_2010.Nessuno
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            End If

        End If


        'MAGAZZINO DI PROVENIENZA
        If Elem_Cod <> ALTRI_BENI AndAlso Elem_Cod <> SERVIZI AndAlso Elem_Cod <> 1 Then

            'se sono in bolla emessa, il caumov è scarico
            'e si può muovere la cmb provenienza

            'se sono in bolla ricevuta, il caumov è carico
            'e non si può muovere la cmb provenienza
            'se si muove, significa che è il magazzino del contatto
            'e non si deve gestire l'evento

            If xCaricoScarico <> CAU_CARICO Then
                '//////////////////////////////////////////
                If Me.Cmb_Provenienza.SelectedIndex > 0 Then
                    Piva_Provenienza = ViewState("Piva")
                    Provenienza = Cmb_Provenienza.SelectedItem.Text
                    array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                    Id_Provenienza = array_temp(0)
                    SaCod_Provenienza = array_temp(1)
                Else
                    Piva_Provenienza = "0"
                    SaCod_Provenienza = 0
                    Id_Provenienza = 0
                    Provenienza = AgronicaAgenda_2010.Nessuno
                End If
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            Else
                '//////////////////////////////////////////
                'E' IL CONTATTO!!!!!!!!!!!!!!!!!
                If Me.Cmb_Provenienza.SelectedIndex > 0 Then
                    Piva_Provenienza = Qs_CodContatto
                    Provenienza = Cmb_Provenienza.SelectedItem.Text
                    array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                    Id_Provenienza = array_temp(0)
                    SaCod_Provenienza = array_temp(1)
                Else
                    Piva_Provenienza = "0"
                    SaCod_Provenienza = 0
                    Id_Provenienza = 0
                    Provenienza = AgronicaAgenda_2010.Nessuno
                End If
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            End If


        Else 'ALTRI_BENI - SERVIZI

            If xCaricoScarico <> CAU_CARICO Then
                '//////////////////////////////////////////
                Piva_Provenienza = ViewState("Piva")
                SaCod_Provenienza = 0
                Id_Provenienza = 0
                Provenienza = AgronicaAgenda_2010.Nessuno
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            Else
                '//////////////////////////////////////////
                'E' IL CONTATTO!!!!!!!!!!!!!!!!!
                Piva_Provenienza = Qs_CodContatto
                SaCod_Provenienza = 0
                Id_Provenienza = 0
                Provenienza = AgronicaAgenda_2010.Nessuno
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            End If

        End If

        '-------------------------------------------------------------------------------

        Select Case Elem_Cod

            Case ALTRI_BENI, SERVIZI

                If Me.Txt_BeniStrumentali.Text = "" Then
                    Messaggio &= DirectCast(GetLocalResourceObject("InserisciIlNomeDelProdotto"), String) & vbCrLf
                Else
                    ProMat_Des = Me.Txt_BeniStrumentali.Text
                End If

                '-------------------------------------------------------------

            Case ZOO_CONSISTENZA

                Exit Sub

                '--------------------------------------------

            Case Else

                If Me.cmb_Prodotti.SelectedIndex < 1 Then
                    Messaggio &= AgronicaAgenda_2010.SelezionareUnProdotto & vbCrLf
                Else
                    If cmb_Prodotti.SelectedItem.Value.Split("|")(0) > 0 Then
                        Pro_Cod = Me.cmb_Prodotti.SelectedItem.Value.Split("|")(0)
                        ProMat_Des = Me.cmb_Prodotti.SelectedItem.Text
                    Else
                        Mat_Cod = -Me.cmb_Prodotti.SelectedItem.Value
                        ProMat_Des = Me.cmb_Prodotti.SelectedItem.Text
                    End If
                End If

        End Select

        '===================================================

        'DETTAGLIO PRODOTTO


        Recupera_DettagliProdotto(Messaggio,
                                Cod_Progetto,
                                Progetto_Des,
                                Fase_Cod,
                                LottoAccettazione,
                                Cal_Cod,
                                Cal_Des,
                                UdmCod,
                                UdmDes,
                                Qta,
                                Prezzo,
                                PrezzoNetto)


        '===================================================

        'DETTAGLI ECONOMICI

        If Me.Pannello_Economico.Visible Then

            Recupera_DettagliEconomici(Udm_Cod_Extra,
                                        Qta_Extra,
                                        Prezzo_Effettivo,
                                        Sconto_Magg,
                                        Variazione_Perc,
                                        Variazione,
                                        Imponibile,
                                        Imponibile_Netto,
                                        Cod_Iva,
                                        Iva,
                                        Anno,
                                        Ric_Cod,
                                        Cod_Conto,
                                        Conto)

            If Imponibile = 0 AndAlso Imponibile_Netto = 0 Then
                ' Dim Messaggio2 As String = "Non sono stati valorizzati correttamente alcuni dettagli economici, verificare!" & vbCrLf
                Dim Messaggio2 As String = DirectCast(GetLocalResourceObject("ImponibileEImponibileNettoSonoPariAZero"), String) & vbCrLf
                Messaggi.AgroMsgBox(Messaggio2, Page, , Me.upDati)

            End If

        Else
            Imponibile = Qta * Prezzo
            Imponibile_Netto = Qta * PrezzoNetto
        End If

        '===================================================

        '===================================================

        'DETTAGLI FERTILIZZANTE

        'If Me.Pannello_Fertilizzante.Visible = True Then
        Recupera_DettagliFertilizzante(Messaggio, N, P2O5, K2O, Cu, Regolamento_Cod_Ferti)
        'End If
        '===================================================

        Dim Rif_Esterno As String = ""
        Dim Rif_Esterno_2 As String = ""
        Recupera_DettagliNascosti(Rif_Esterno, Rif_Esterno_2)


        Dim udms As String = ""
        ' Giulia: 18/6/2019: Visto che al salvataggio Quintali e Tonnellate diventeranno Kg e Metri cubi diventeranno Litri
        ' è inutile che confronto l'udm della drop down con quelle del db perché in questi casi saranno sempre diversi.
        ' Confronto invece lo storico sul db con quella che verrà effettivamente salvata
        Dim udmConvertita As Integer = UdmCod

        If Elem_Cod = FERTILIZZANTI Then
            Select Case UdmCod
                Case enum_UnitaMisura.Quintali, enum_UnitaMisura.Tonnellate
                    udmConvertita = enum_UnitaMisura.KG
                Case enum_UnitaMisura.Metri_Cubi
                    udmConvertita = enum_UnitaMisura.Litri
            End Select
        End If

        If Not VerificaUdmCarichiPrecedenti(Piva_Destinazione, SaCod_Destinazione, Id_Destinazione, Pro_Cod, Mat_Cod, Elem_Cod, udmConvertita, udms) Then

            'controllo se ho acconsentito precedentemente
            If UdmMovimenti_SI_NO.Value = "0" Then

                'se non ho acconsentito genere agrosino che mi rilancerà il salvataggio via jscript
                Dim messaggio_errore As String = String.Format(
                    DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_MovimentatoPrecedentementeConDiversaUnitàDiMisuraProcedereUgualmente"), String),
                    udms)
                'AgroSiNo

                'impedisco di procedere
                Messaggi.AgroSiNo(messaggio_errore, "UdmMovimenti", Page, , update_si_no)


                Exit Sub

            Else
                'se ho già cliccato  ok vado avanti
                UdmMovimenti_SI_NO.Value = "0"
            End If

        End If

        If Messaggio <> "" Then
            Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
            Exit Sub
        End If

        Dim MessaggioAlert As String = ""

        '===================================================
        '   CONTROLLO GIACENZA
        '===================================================

        Dim LottoXControllo As String

        'caso di scarico (e trasferimento, ma il trasferimento è per singolo prodotto quindi non passa di qui)
        If xCaricoScarico <> CAU_CARICO Then

            Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim qta_in_data As Decimal


            'poiché da un certa data i com+ salvano il lotto accettazione anche per prodotti che non sono semilavorati
            'può essere che nel record della giacenza ci sia salvato il lotto 'Indefinito'
            'quindi andando a filtrare per lotto ='' non viene ricavata la giusta giacenza (risulta giacenza 0)
            'metto il controllo che se il lotto non è specificato nella form (la combo è disabilitata)
            'non filtro per lotto, quindi imposto il valore predefinito per non utilizzarlo nel where
            If Not Me.cmb_LottoAccettazione.Enabled Then
                LottoXControllo = LOTTO_NONDEFINITO
            Else
                LottoXControllo = LottoAccettazione
            End If

            'controllo in data
            qta_in_data = objMovDet.Verifica_Giacenze(CStr(xPiva),
                                                    SaCod_Provenienza,
                                                    Id_Provenienza,
                                                    Elem_Cod,
                                                    Pro_Cod,
                                                    Mat_Cod,
                                                    Cod_Progetto,
                                                    Fase_Cod,
                                                    LottoXControllo,
                                                    Cal_Cod,
                                                    UdmCod,
                                                    AGRODATAINIZIO,
                                                    CDate(Txt_DataMovimento.Text),
                                                    Qs_IdAgenda,
                                                    objParametri_Server)

            If Qta > qta_in_data Then
                MessaggioAlert &= vbCrLf & vbCrLf &
                    String.Format(
                        DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_QuantitàSpecificataMaggioreDiGiacenza"), String),
                        Txt_DataMovimento.Text, qta_in_data, Me.cmb_Udm.SelectedItem.Text
                    )
            End If



            'controllo a oggi
            qta_in_data = objMovDet.Verifica_Giacenze(CStr(xPiva),
                                                    SaCod_Provenienza,
                                                    Id_Provenienza,
                                                    Elem_Cod,
                                                    Pro_Cod,
                                                    Mat_Cod,
                                                    Cod_Progetto,
                                                    Fase_Cod,
                                                    LottoXControllo,
                                                    Cal_Cod,
                                                    UdmCod,
                                                    AGRODATAINIZIO,
                                                    Date.Now,
                                                    Qs_IdAgenda,
                                                    objParametri_Server)

            If Qta > qta_in_data Then

                MessaggioAlert &= vbCrLf & vbCrLf & String.Format(
                    DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_GiacenzaSufficienteInDataSceltaNonInOdierna"), String),
                    qta_in_data, Me.cmb_Udm.SelectedItem.Text
                )

            End If

        End If


        If Chk_LottoImpianto.Checked OrElse Chk_ParametroQualitativo.Checked Then
            'Cal_Cod =0
            'Pro_Cod=CODPROGETTO_NONDEFINITO 

            ' devo inserire n righe di dettagli a prescindere dai Pro_Cod eo Cal_Cod

            If Chk_LottoImpianto.Checked OrElse Cod_Progetto = 0 Then
                Cod_Progetto = CODPROGETTO_NONDEFINITO
            End If

            'If Cod_Progetto = 0 Then
            '    Cod_Progetto = CODPROGETTO_NONDEFINITO
            'End If

            If Not Me.cmb_LottoAccettazione.Enabled Then
                LottoXControllo = LOTTO_NONDEFINITO
            Else
                LottoXControllo = LottoAccettazione
            End If


            'Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            ''mi serve Materie_Prime_Campionature
            'Dim Dt As DataTable = objMP.MateriePrime_Giacenze(CStr(xPiva), _
            '                                             SaCod_Provenienza, _
            '                                             Elem_Cod, _
            '                                             Pro_Cod, _
            '                                             Mat_Cod, _
            '                                             UdmCod, _
            '                                             Id_Destinazione, _
            '                                       Cal_Cod, _
            '                                       Cod_Progetto, _
            '                                            Fase_Cod, _
            '                                            LottoAccettazione, _
            '                                            "", _
            '                                            0, _
            '                                            0, _
            '                                            0, _
            '                                            0, _
            '                                            0, _
            '                                            0, _
            '                                            0, _
            '                                            "", _
            '                                            "", _
            '                                            "", 0, "", False, " Movimenti_dettagli.Cod_Progetto <> 0 ", "", objParametri_Server)


            Dim objG As New AgronicaCoreContabDAL.Giacenze_R
            Dim Dt_Giacenze As DataTable = objG.SchedaGiacenzeMagazzino(CDate(Txt_DataMovimento.Text),
                                                                        xPiva,
                                                                        SaCod_Provenienza,
                                                                        Id_Destinazione,
                                                                        Elem_Cod,
                                                                        Pro_Cod,
                                                                        Mat_Cod,
                                                                        Cal_Cod,
                                                                        Cod_Progetto,
                                                                        Fase_Cod,
                                                                        UdmCod,
                                                                        LottoXControllo,
                                                                        False,
                                                                        "",
                                                                        "", "", "", "", "", "", "", "", "", "",
                                                                        "",
                                                                        objParametri_Server, objParametri_Utenti)



            Dim Qta_Tot_da_inserire As Decimal = Qta
            Dim Qta_Tot_inserita As Decimal = 0
            Dim totale_giacenza_lotto As Decimal = 0
            Dim Inseriti As Integer = 0

            Dim MessaggioFinale As String = ""

            For j = 0 To Dt_Giacenze.Rows.Count - 1

                Dim cal_des2 As String = ""

                Select Case CInt(Dt_Giacenze.Rows(j).Item("Cal_Cod"))

                    Case Is > 0
                        Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R
                        cal_des2 = objCalibri.CalDes_from_CalCod(Dt_Giacenze.Rows(j).Item("Cal_Cod"), objParametri_Server)
                        'x_Des = CalDes_from_CalCod(objServer, objSession, objPage, x_Cod)
                        'cal_des2 = Dt_Giacenze.Rows(i).Item("Cal_Des")

                    Case Is < 0
                        'cal_des2 = Dt_Giacenze.Rows(i).Item("Descrizione")

                        Dim ObjCampionature As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R
                        Dim DtCampionature As DataTable
                        DtCampionature = ObjCampionature.Leggi(Dt_Giacenze.Rows(j).Item("Cal_Cod"), "", 0, 0, 0,
                                                               "", True,
                                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                               "", "", objParametri_Server)

                        If DtCampionature IsNot Nothing AndAlso DtCampionature.Rows.Count > 0 Then
                            cal_des2 = DtCampionature.Rows(0).Item("Descrizione")
                        End If

                    Case Is = 0
                        'caso di carico ---> i semilavorati provengono da terzi
                        'hanno il cod_progetto =0, ovvero nessun impianto
                        'e non hanno nessun calibro
                        cal_des2 = DirectCast(GetLocalResourceObject("Indefinito"), String)

                End Select

                Progetto_Des = ""
                If Dt_Giacenze.Rows(j).Item("Cod_Progetto") <> 0 Then
                    Progetto_Des = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R().Progetto_Des_From_Progetto_Cod(xPiva, Dt_Giacenze.Rows(j).Item("Cod_Progetto"), objParametri_Server)
                End If

                Dim Lotto As String = Dt_Giacenze.Rows(j).Item("Lotto")

                Dim Dest_Qta As Decimal = Dt_Giacenze.Rows(j).Item("Giacenza")
                'Dim Dett_Qta As Decimal = Dt.Rows(j).Item("Dett_Qta")
                'If Dest_Qta <> Dett_Qta Then
                '    Messaggi.AgroMsgBox("ATTENZIONE!!!" & vbCrLf & vbCrLf & _
                '       "Dest_Qta <> Dett_Qta." & vbCrLf & _
                '       "Non previsto", Page, , Me.upDati)
                '    Exit Sub
                'End If

                totale_giacenza_lotto += Dest_Qta

                'Qta_Tot_da inserire = Qta
                'Qta_Tot_inserita As Decimal = ...
                Dim Qta_inserire_Ora As Decimal = 0
                If Dest_Qta + Qta_Tot_inserita <= Qta_Tot_da_inserire Then
                    'se posso inserire tutta la giacenza la inserisco
                    Qta_inserire_Ora = Dest_Qta

                Else
                    'se non posso inserirla tutta
                    If Qta_Tot_inserita < Qta_Tot_da_inserire Then
                        'se la quantità finora inserita è minore di quella da inserire inserisco quanto manca
                        Qta_inserire_Ora = Qta_Tot_da_inserire - Qta_Tot_inserita
                    Else
                        'se è già completa (non dovrebbe mai essere maggiore) inserisco 0
                        Qta_inserire_Ora = 0
                    End If

                End If

                Qta_Tot_inserita += Qta_inserire_Ora

                'controllo se 0
                If Qta_inserire_Ora = 0 Then

                Else

                    Inserisci_Riga_Prodotto(Piva_Destinazione,
                                            SaCod_Destinazione,
                                            Id_Destinazione,
                                            Destinazione,
                                            Piva_Provenienza,
                                            SaCod_Provenienza,
                                            Id_Provenienza,
                                            Provenienza,
                                            Elem_Cod,
                                            Elem_Des,
                                            Pro_Cod,
                                            Mat_Cod,
                                            ProMat_Des,
                                            Dt_Giacenze.Rows(j).Item("Cal_Cod"),
                                            cal_des2,
                                            Dt_Giacenze.Rows(j).Item("Cod_Progetto"),
                                            Fase_Cod,
                                            Progetto_Des,
                                            Lotto,
                                            UdmCod,
                                            UdmDes,
                                            Qta_inserire_Ora,
                                            Prezzo,
                                            PrezzoNetto,
                                            Udm_Cod_Extra,
                                            Qta_Extra,
                                            Sconto_Magg,
                                            Variazione_Perc,
                                            Variazione,
                                            Cod_Iva,
                                            Iva,
                                            Imponibile,
                                            Imponibile_Netto,
                                            Prezzo_Effettivo,
                                            Anno,
                                            Ric_Cod,
                                            Cod_Conto,
                                            Conto,
                                            N,
                                            P2O5,
                                            K2O,
                                            Cu,
                                            Regolamento_Cod_Ferti,
                                            Rif_Esterno,
                                            Rif_Esterno_2)

                    Inseriti += 1

                End If

                'controlli giacenze nell'ultima riga
                If j = Dt_Giacenze.Rows.Count - 1 Then

                    If Inseriti <> (j + 1) Then

                        Dim attenzione As String = String.Format(
                            DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_NonSonoStatiInseritiTuttiIDettagli"), String),
                            Dt_Giacenze.Rows.Count, Inseriti
                        )

                        If Qta_Tot_inserita = Qta_Tot_da_inserire Then

                            'ok
                            If totale_giacenza_lotto = Qta_Tot_inserita Then
                                MessaggioFinale &= attenzione & vbCrLf & String.Format(
                                        DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_GiacenzeUtilizzateCompletamente"), String),
                                        totale_giacenza_lotto, Qta_Tot_da_inserire
                                    ) & vbCrLf

                            Else
                                MessaggioFinale &= attenzione & vbCrLf & String.Format(
                                        DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_GiacenzeNonCompletamenteUtilizzate"), String),
                                        totale_giacenza_lotto, Qta_Tot_inserita
                                    ) & vbCrLf
                            End If

                        End If


                        If Qta_Tot_inserita < Qta_Tot_da_inserire Then
                            'non tutta inserita
                            MessaggioFinale &= attenzione & vbCrLf & String.Format(
                                    DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_QuantitàInseritaEccedenteGiacenza"), String),
                                    Qta_Tot_da_inserire, Qta_Tot_inserita
                                )

                        End If

                        If Qta_Tot_inserita > Qta_Tot_da_inserire Then
                            'impossibile
                            MessaggioFinale &= attenzione & vbCrLf & String.Format(
                                    DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_QuantitàTotInseritaEccessiva"), String),
                                    Qta_Tot_inserita, Qta_Tot_da_inserire
                                )

                        End If

                    Else

                        If Qta_Tot_inserita = Qta_Tot_da_inserire Then

                            'ok
                            If totale_giacenza_lotto = Qta_Tot_inserita Then
                                MessaggioFinale &= String.Format(
                                        DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_OKDettagliInseritiGiacenzeUtilizzateCompletamente"), String),
                                        Dt_Giacenze.Rows.Count, totale_giacenza_lotto, Qta_Tot_da_inserire
                                    ) & vbCrLf

                            Else
                                MessaggioFinale &= String.Format(
                                        DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_NBDettagliInseritiGiacenzeNonCompletamenteUtilizzate"), String),
                                        Dt_Giacenze.Rows.Count, totale_giacenza_lotto, Qta_Tot_inserita
                                    ) & vbCrLf

                            End If

                        End If

                        If Qta_Tot_inserita < Qta_Tot_da_inserire Then
                            'non tutta inserita
                            MessaggioFinale &= String.Format(
                                DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_QuantitàInseritaEccedenteGiacenza"), String),
                                Qta_Tot_da_inserire, Qta_Tot_inserita
                            )
                            'i18n per poter riusare la risorsa inserita ho ignorato la stringa "ATTENZIONE!!!" & vbCrLf & vbCrLf
                        End If

                        If Qta_Tot_inserita > Qta_Tot_da_inserire Then
                            'impossibile
                            MessaggioFinale &= String.Format(
                                DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_QuantitàTotInseritaEccessiva"), String),
                                Qta_Tot_inserita, Qta_Tot_da_inserire
                            )
                            'i18n per poter riusare la risorsa inserita ho ignorato la stringa "ATTENZIONE!!!" & vbCrLf & vbCrLf
                        End If

                    End If


                End If

            Next

            Messaggi.AgroMsgBox(MessaggioFinale & vbCrLf & vbCrLf & MessaggioAlert, Page, , Me.upDati)

        Else

            If xCaricoScarico <> CAU_CARICO AndAlso Elem_Cod = TRASFORMATI_VEGETALI Then

                Dim objG As New AgronicaCoreContabDAL.Giacenze_R
                Dim Dt_Giacenze As DataTable = objG.SchedaGiacenzeMagazzino(CDate(Txt_DataMovimento.Text),
                                                                            xPiva,
                                                                            SaCod_Provenienza,
                                                                            Id_Provenienza,
                                                                            Elem_Cod,
                                                                            Pro_Cod,
                                                                            Mat_Cod,
                                                                            0,
                                                                            0,
                                                                            0,
                                                                            UdmCod,
                                                                            LOTTO_NONDEFINITO,
                                                                            False,
                                                                            "",
                                                                            "", "", "", "", "", "", "", "", "", "",
                                                                            "",
                                                                            objParametri_Server, objParametri_Utenti)

                If Dt_Giacenze.Rows.Count > 0 Then
                    Cal_Cod = Dt_Giacenze.Rows(0).Item("cal_cod")
                    LottoAccettazione = Dt_Giacenze.Rows(0).Item("lotto")
                End If



            End If

            Inserisci_Riga_Prodotto(Piva_Destinazione,
                                    SaCod_Destinazione,
                                    Id_Destinazione,
                                    Destinazione,
                                    Piva_Provenienza,
                                    SaCod_Provenienza,
                                    Id_Provenienza,
                                    Provenienza,
                                    Elem_Cod,
                                    Elem_Des,
                                    Pro_Cod,
                                    Mat_Cod,
                                    ProMat_Des,
                                    Cal_Cod,
                                    Cal_Des,
                                    Cod_Progetto,
                                    Fase_Cod,
                                    Progetto_Des,
                                    LottoAccettazione,
                                    UdmCod,
                                    UdmDes,
                                    Qta,
                                    Prezzo,
                                    PrezzoNetto,
                                    Udm_Cod_Extra,
                                    Qta_Extra,
                                    Sconto_Magg,
                                    Variazione_Perc,
                                    Variazione,
                                    Cod_Iva,
                                    Iva,
                                    Imponibile,
                                    Imponibile_Netto,
                                    Prezzo_Effettivo,
                                    Anno,
                                    Ric_Cod,
                                    Cod_Conto,
                                    Conto,
                                    N,
                                    P2O5,
                                    K2O,
                                    Cu,
                                    Regolamento_Cod_Ferti,
                                    Rif_Esterno,
                                    Rif_Esterno_2)


            If MessaggioAlert <> "" Then
                Messaggi.AgroMsgBox(MessaggioAlert, Page, , Me.upDati)
            End If

            Trap_Cod.Value = "0"

            'se ho caricato una trappola 
            'verifico se è stato caricato un innesco
            If Elem_Cod = TRAPPOLE AndAlso xCaricoScarico = CAU_CARICO Then

                Dim objTrap As New AgronicaCoreMetaSchemaDAL.Trappole_R
                '--------------------------------------
                'NOTA: USO vale: 
                ' 1 = Installazione Trappole
                ' 2 = Cattura di Massa
                ' 3 = Confusione Sessuale
                ' 4 = Disorientamento Sessuale
                '--------------------------------------
                Select Case objTrap.Uso_from_TrapCod(Pro_Cod, objParametri_Server)

                    Case 1, 2

                        Dim InnescoPresente As Boolean = False
                        Dim DtxTrappole = ViewState("DT_Prodotti_NO_Contabili")

                        'Ciclo nelle righe del DataTable
                        For IndiceRiga = 0 To DtxTrappole.Rows.Count - 1

                            If DtxTrappole.Rows(IndiceRiga).Item("Piva_Destinazione") = Piva_Destinazione AndAlso
                               DtxTrappole.Rows(IndiceRiga).Item("SaCod_Destinazione") = SaCod_Destinazione AndAlso
                               DtxTrappole.Rows(IndiceRiga).Item("Id_Destinazione") = Id_Destinazione AndAlso
                               DtxTrappole.Rows(IndiceRiga).Item("Piva_Provenienza") = Piva_Provenienza AndAlso
                               DtxTrappole.Rows(IndiceRiga).Item("SaCod_Provenienza") = SaCod_Provenienza AndAlso
                               DtxTrappole.Rows(IndiceRiga).Item("Id_Provenienza") = Id_Provenienza AndAlso
                               DtxTrappole.Rows(IndiceRiga).Item("Elem_Cod") = INNESCHI Then

                                InnescoPresente = True
                                Exit For
                            End If
                        Next

                        If Not InnescoPresente Then

                            Trap_Cod.Value = Pro_Cod

                            Dim messaggio_errore As String = DirectCast(GetLocalResourceObject("Inserisci_Prodotto_in_Griglia_CaricareInnescoPerTrappola"), String)
                            Messaggi.AgroSiNo(messaggio_errore, "CaricoInnesco", Page, , update_si_no)

                        End If

                End Select

            End If

        End If


        '-------------------------------------------------------------------------------

        Me.Pannello_Prodotti.Visible = True
        Me.DataGrid_Prodotti.Visible = True
        Me.Lbl_PannelloCarichi.Visible = True

        Select Case Qs_Mode.ToLower
            Case "magazzino", "compravendita"
                Me.Pannello_Salvataggio.Visible = True
                If Qs_Operazione <> enum_TipoOperazioneDB.Scrittura Then
                    Me.ImgBtnSalvaNuovo.Visible = False
                    Me.ImgBtnSalvaNuovo.Enabled = False
                End If
            Case "bolla", "fattura"
                If Not xFlagDocLight Then
                    Me.Pannello_InsDocumento.Visible = True
                Else
                    Me.Pannello_Salvataggio.Visible = True
                End If

        End Select


        'Pulisci_DettagliEconomici()

        Me.Txt_Quantita.Text = "0"

    End Sub


    '########################################################################################
    Private Sub Inserisci_Riga_Prodotto(ByVal Piva_Destinazione As String,
                                        ByVal SaCod_Destinazione As Integer,
                                        ByVal Id_Destinazione As Integer,
                                        ByVal Destinazione As String,
                                        ByVal Piva_Provenienza As String,
                                        ByVal SaCod_Provenienza As Integer,
                                        ByVal Id_Provenienza As Integer,
                                        ByVal Provenienza As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Categoria_Des As String,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal ProMat_Des As String,
                                        ByVal Cal_Cod As Integer,
                                        ByVal Cal_Des As String,
                                        ByVal Cod_Progetto As Integer,
                                        ByVal Fase_Cod As Integer,
                                        ByVal Progetto_Des As String,
                                        ByVal Lotto As String,
                                        ByVal Udm_Cod As Integer,
                                        ByVal Udm_Des As String,
                                        ByVal Qta As Decimal,
                                        ByVal Prezzo As Decimal,
                                        ByVal Prezzo_Netto As Decimal,
                                        ByVal Udm_Cod_Extra As Decimal,
                                        ByVal Qta_Extra As Decimal,
                                        ByVal Cod_Variazione As Decimal,
                                        ByVal Variazione_Perc As String,
                                        ByVal Variazione As Decimal,
                                        ByVal Cod_Iva As Integer,
                                        ByVal Iva As Decimal,
                                        ByVal Imponibile As Decimal,
                                        ByVal Imponibile_Netto As Decimal,
                                        ByVal Prezzo_Effettivo As Decimal,
                                        ByVal Anno As Integer,
                                        ByVal Ric_Cod As Integer,
                                        ByVal Cod_Conto As Integer,
                                        ByVal Conto As String,
                                        ByVal N As Decimal,
                                        ByVal P2O5 As Decimal,
                                        ByVal K2O As Decimal,
                                        ByVal Cu As Decimal,
                                        ByVal Regolamento_Cod_Ferti As Integer,
                                        ByVal Rif_Esterno As String,
                                        ByVal Rif_Esterno_2 As String)

        Dim objIVA As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R

        Dim Dt As DataTable
        Dim Dr As DataRow
        Dim ElementoPresente As Boolean
        Dim IndiceRiga As Integer

        '----- Verifico la correttezza dei dati inseriti

        If Qta = 0 Then
            Messaggio = DirectCast(GetLocalResourceObject("ImpossibileInserireUnaQuantitàNullaONegativa"), String)
            Call Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
            Exit Sub
        End If

        '----- Verifico che il formulato non sia gia' presente nel DataTable

        'Inizializzo
        ElementoPresente = False

        'Recupero il DataTable
        Dt = ViewState("DT_Prodotti_NO_Contabili")


        'Ciclo nelle righe del DataTable
        For IndiceRiga = 0 To Dt.Rows.Count - 1

            If Dt.Rows(IndiceRiga).Item("Piva_Destinazione") = Piva_Destinazione And
               Dt.Rows(IndiceRiga).Item("SaCod_Destinazione") = SaCod_Destinazione And
               Dt.Rows(IndiceRiga).Item("Id_Destinazione") = Id_Destinazione And
               Dt.Rows(IndiceRiga).Item("Piva_Provenienza") = Piva_Provenienza And
               Dt.Rows(IndiceRiga).Item("SaCod_Provenienza") = SaCod_Provenienza And
               Dt.Rows(IndiceRiga).Item("Id_Provenienza") = Id_Provenienza And
               Dt.Rows(IndiceRiga).Item("Elem_Cod") = Elem_Cod And
               Dt.Rows(IndiceRiga).Item("Pro_Cod") = Pro_Cod And
               Dt.Rows(IndiceRiga).Item("Mat_Cod") = Mat_Cod And
               Dt.Rows(IndiceRiga).Item("Cal_Cod") = Cal_Cod And
               Dt.Rows(IndiceRiga).Item("Cod_Progetto") = Cod_Progetto And
               Dt.Rows(IndiceRiga).Item("Lotto") = Lotto And
               Dt.Rows(IndiceRiga).Item("Udm_Cod") = Udm_Cod And
               Dt.Rows(IndiceRiga).Item("Fase_Cod") = Fase_Cod Then

                Select Case Dt.Rows(IndiceRiga).Item("Elem_Cod")

                    Case ALTRI_BENI, SERVIZI

                        If Dt.Rows(IndiceRiga).Item("Descrizione") = Categoria_Des & ": " & ProMat_Des Then
                            ElementoPresente = True
                            Exit For
                        Else
                            ElementoPresente = False
                        End If

                    Case Else
                        ElementoPresente = True
                        Exit For
                End Select

            End If

        Next

        '  Galassi, 23/03/2017 17.55.08: Modifica Richiesta da Fabrizio per Studio Tini che caricano n volte lo stesso prodotto e lo vogliono sommato
        'Se esiste gia' allora esco
        If ElementoPresente Then
            Messaggio = String.Format(
                DirectCast(GetLocalResourceObject("Inserisci_Riga_Prodotto_DettaglioGiàPresenteSommareQuantità"), String),
                Qta.ToString, Dt.Rows(IndiceRiga).Item("Qta").ToString
            )
            'Call Messaggi.AgroMsgBox(Messaggio, Page, , Me.upDati)
            ViewState("rigaProdottoInserito") = IndiceRiga
            ViewState("qtaProdottoDaAggiornare") = Qta
            Call Messaggi.AgroSiNo(Messaggio, "SommaProdotto", Page, , Me.upDati)
            Exit Sub
        End If

        '----- Inserisco il nuovo record

        Dim Max As Integer = 0

        For i As Integer = 0 To Dt.Rows.Count - 1
            Dr = Dt.Rows(i)
            If Max < Dr.Item("Chiave") Then
                Max = Dr.Item("Chiave")
            End If
        Next

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Piva_Destinazione") = Piva_Destinazione
        Dr.Item("SaCod_Destinazione") = SaCod_Destinazione
        Dr.Item("Id_Destinazione") = Id_Destinazione
        Dr.Item("Destinazione") = Destinazione
        Dr.Item("Piva_Provenienza") = Piva_Provenienza
        Dr.Item("SaCod_Provenienza") = SaCod_Provenienza
        Dr.Item("Id_Provenienza") = Id_Provenienza
        Dr.Item("Provenienza") = Provenienza

        Dr.Item("Elem_Cod") = Elem_Cod
        Dr.Item("Pro_Cod") = Pro_Cod
        Dr.Item("Mat_Cod") = Mat_Cod
        Dr.Item("Cal_Cod") = Cal_Cod
        Dr.Item("Cod_Progetto") = Cod_Progetto
        Dr.Item("Fase_Cod") = Fase_Cod
        Dr.Item("Lotto") = Lotto
        Dr.Item("Udm_Cod") = Udm_Cod
        Dr.Item("Udm_Des") = Udm_Des
        Dr.Item("Qta") = Qta
        Dr.Item("Chiave") = Max + 1

        ' se l'unità di misura può essere diversa da quella di salvataggio, salvo l'extra_int
        If Elem_Cod = FERTILIZZANTI AndAlso (cmb_PUARegolamenti.Visible AndAlso cmb_PUARegolamenti.SelectedItem.Value.Split(SEP_PuaReg)(0) <> "0") Then
            Dr.Item("Extra_Int") = Udm_Cod
        Else
            Dr.Item("Extra_Int") = 0
        End If

        Dr.Item("Descrizione") = Categoria_Des
        Dr.Item("Descrizione") &= ": " & ProMat_Des
        'i18n
        Select Case Elem_Cod

            Case ZOO_CONSISTENZA

                If Progetto_Des <> "" Then
                    Dr.Item("Descrizione") &= " (" & AgronicaAgenda_2010.MatricolaAbbr & " " & Progetto_Des & ")"
                End If

            Case Else

                If Pro_Cod <> 0 Then
                    Dr.Item("Codice") = "Gias" & Pro_Cod
                Else
                    Dr.Item("Codice") = "Azi" & Mat_Cod

                    If Progetto_Des <> "" Then
                        Dr.Item("Descrizione") &= ", " & AgronicaAgenda_2010.LottoImpianto & ": " & Progetto_Des
                    End If

                    If Lotto <> "" AndAlso Lotto <> "-1" Then
                        Dr.Item("Descrizione") &= ", " & AgronicaAgenda_2010.LottoMagazzino & ": " & Lotto
                    End If

                    If Cal_Des <> "" Then
                        Dr.Item("Descrizione") &= ", " & AgronicaAgenda_2010.Calibro & ": " & Cal_Des
                    End If
                End If

        End Select

        Dr.Item("Prodotto") = ProMat_Des

        Dr.Item("Prezzo_Unitario") = Prezzo 'Format(Prezzo, "0.00")
        Dr.Item("Prezzo_Unitario_Netto") = Prezzo_Netto 'Format(Prezzo_Netto, "0.00")

        Dr.Item("Udm_Cod_Extra") = Udm_Cod_Extra
        Dr.Item("Qta_Extra") = Qta_Extra

        Dr.Item("Cod_Variazione") = Cod_Variazione
        Dr.Item("Variazione_Perc") = Variazione_Perc
        Dr.Item("Variazione") = Variazione

        Dr.Item("Cod_Iva") = Cod_Iva
        Dr.Item("Aliquota") = objIVA.Aliquota_from_CodIVA(Cod_Iva, "", objParametri_Server) 'objContabHLP.Aliquota_from_CodIVA(Cod_Iva)
        Dr.Item("Iva") = Iva
        Dr.Item("Imponibile") = Imponibile
        Dr.Item("Imponibile_Netto") = Imponibile_Netto
        Dr.Item("Prezzo_Effettivo") = Prezzo_Effettivo
        Dr.Item("Anno") = Anno
        Dr.Item("Ric_Cod") = Ric_Cod
        Dr.Item("Cod_Conto") = Cod_Conto
        Dr.Item("Conto") = Conto

        Dr.Item("N") = N
        Dr.Item("P2O5") = P2O5
        Dr.Item("K2O") = K2O
        Dr.Item("Cu") = Cu
        Dr.Item("Regolamento_Cod_Ferti") = Regolamento_Cod_Ferti

        Dr.Item("Rif_Esterno") = Rif_Esterno
        Dr.Item("Rif_Esterno_2") = Rif_Esterno_2

        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)

        '----- Associo il DataTable con la DataGrid

        Me.DataGrid_Prodotti.DataSource = Dt
        Me.DataGrid_Prodotti.DataBind()

        'dopo il DataBind, se il DataGrid non ha righe nascondo il pannello che lo contiene
        If Me.DataGrid_Prodotti.Items.Count < 1 Then
            'Imposta_Pannelli("D")
            Me.Pannello_Prodotti.Visible = False
            Me.Lbl_PannelloCarichi.Visible = False
        Else
            Me.Pannello_Prodotti.Visible = True
            Me.Lbl_PannelloCarichi.Visible = True
        End If

        '----- Salvo il DataTable dentro il ViewState
        ViewState("DT_Prodotti_NO_Contabili") = Dt

    End Sub


    Private Function VerificaUdmCarichiPrecedenti(ByVal pivadest As String, ByVal sacoddest As Int32, ByVal iddest As Int32, ByVal ProCod As Int32, ByVal MatCod As Int32, ByVal Elemcod As Int32, ByVal udmselezionata As Int32, ByRef udms As String) As Boolean

        'Dim UdmSelezionata As String = cmb_Udm.SelectedValue
        'Dim Prodotto As String = cmb_Prodotti.SelectedValue

        Dim dt As DataTable
        dt = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().Leggi_UDM_Utilizzate(pivadest, sacoddest, iddest, 0, 0, 0, Elemcod, ProCod, MatCod, 0, "", 0, 0, 0, 0, "", 0, "  Movimenti_Dettagli.Udm_Cod <> " & udmselezionata & "   ", "", objParametri_Server)
        If dt.Rows.Count > 0 Then
            Dim i As Integer = 0
            For i = 0 To dt.Rows.Count - 1

                If i = 0 Then
                Else
                    udms = ", " & udms
                End If
                udms = udms & New AgronicaCoreMetaSchemaDAL.UnitaMisura_R().UdmDes_from_UdmCod(dt.Rows(i).Item("Udm_Cod"), "", objParametri_Server)
            Next
            Return False
        End If
        Return True

    End Function

    '####################################################################################################################
    Private Sub DataGrid_Prodotti_ItemCommand(ByVal source As System.Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles DataGrid_Prodotti.ItemCommand

        Cancella_Prodotti(e)

    End Sub


    '####################################################################################################################
    Private Sub Cancella_Prodotti(ByRef e As System.Web.UI.WebControls.DataGridCommandEventArgs)


        Dim IndiceRigaGriglia As Integer
        '  Dim Chiave(12) As Object
        Dim Chiave(0) As Object

        Dim Dt As DataTable
        Dim Dr As DataRow

        'Recupero l'indice di riga del datagrid
        IndiceRigaGriglia = e.Item.ItemIndex

        Select Case e.CommandName

            Case "Cancella"

                Chiave(0) = DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_P_CHIAVE).Text

                'Recupero il datatable
                Dt = ViewState("DT_Prodotti_NO_Contabili")

                'Trovo la riga da cancellare tramite la chiave
                Dr = Dt.Rows.Find(Chiave)

                'Elimino la riga
                Dr.Delete()

                'Associo il DataTable con la DataGrid
                Me.DataGrid_Prodotti.DataSource = Dt
                Me.DataGrid_Prodotti.DataBind()

                'dopo il databind, se il datagrid non ha righe nascondo il pannello che lo contiene
                If Me.DataGrid_Prodotti.Items.Count < 1 Then
                    'Imposta_Pannelli("D")
                    Me.Pannello_Prodotti.Visible = False
                    Me.Lbl_PannelloCarichi.Visible = False
                Else
                    'Imposta_Pannelli("DC")
                    Me.Pannello_Prodotti.Visible = True
                    Me.Lbl_PannelloCarichi.Visible = True
                End If

                'Salvo il DataTable dentro il viewstate
                ViewState("DT_Prodotti_NO_Contabili") = Dt


        End Select


    End Sub


    '####################################################################################################################
    Private Sub CaricaCombo_Causale(ByVal lav_cod As Integer)

        Me.cmb_Causale.Items.Clear()

        Select Case lav_cod

            Case LAVCOD_CARICO, LAVCOD_ACQUISTO_BENI 'Carico, Attrezzature
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.RilevamentoGiacenzeInizialiDiMagazzino, enum_Pendenza.GiacenzeIniziali))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.BeniAutoprodotti, enum_Pendenza.AutoProduzione))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato, enum_Pendenza.MovPendente))

            Case LAVCOD_SCARICO 'Scarico
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.Autoconsumo, enum_Pendenza.AutoConsumo))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.SmaltimentoPerditaDiLavorazione, enum_Pendenza.Smaltimento))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato, enum_Pendenza.MovPendente))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoASeguitoDiFurto, enum_Pendenza.Furto))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.UtilizzoProdottoFuoriRegione, enum_Pendenza.ScaricoFuoriRegione))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.ResoAFornitore, enum_Pendenza.ResoFornitore))

            Case LAVCOD_AUTOCONSUMO 'Autoconsumo
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.Autoconsumo, enum_Pendenza.AutoConsumo))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato, enum_Pendenza.MovPendente))

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO
                Select Case Qs_Tipo
                    Case CAU_MAGAZZINO
                        Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoDaDocumentoDiTrasporto, enum_Pendenza.DocBolla))
                        'Me.cmb_Causale.Items.Add(New ListItem("Movimento di Magazzino Non Giustificato/Documentato", enum_Pendenza.MovPendente))
                    Case CAU_ANIMALE
                        Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiStallaDaDocumentoDiTrasporto, enum_Pendenza.DocBolla))
                        'Me.cmb_Causale.Items.Add(New ListItem("Movimento di Stalla Non Giustificato/Documentato", enum_Pendenza.MovPendente))
                End Select

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA 'Fattura
                Select Case Qs_Tipo
                    Case CAU_MAGAZZINO
                        Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoDaFatturaCommerciale, enum_Pendenza.DocFattura))
                        Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoDaDocumentoDiTrasportoConFatturaCommerciale, enum_Pendenza.DocBolla))
                        'Me.cmb_Causale.Items.Add(New ListItem("Movimento di Magazzino Non Giustificato/Documentato", enum_Pendenza.MovPendente))
                    Case CAU_ANIMALE
                        Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiStallaDaFatturaCommerciale, enum_Pendenza.DocFattura))
                        Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiStallaDaDocumentoDiTrasportoConFatturaCommerciale, enum_Pendenza.DocBolla))
                        'Me.cmb_Causale.Items.Add(New ListItem("Movimento di Stalla Non Giustificato/Documentato", enum_Pendenza.MovPendente))
                End Select

            Case LAVCOD_VENDITA, LAVCOD_ACQUISTO 'Acquisto, Vendita
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.ProdottoEsenteDaUlterioreDocumentazione, enum_Pendenza.MovESENTE))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato, enum_Pendenza.MovPendente))

            Case LAVCOD_TRASFERIMENTO 'Trasferimento Merci
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.TrasferimentoMerciDiMagazzino, enum_Pendenza.Trasferimento))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato, enum_Pendenza.MovPendente))

            Case LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_ACCETTAZIONE 'Conferimento, Accettazione
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.ConferimentoBeni, enum_Pendenza.Conferimento))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato, enum_Pendenza.MovPendente))

            Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO 'Aumento Consistenze
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.RilevamentoConsistenzeInizialiDiStalla, enum_Pendenza.ZooConsistenzeIniziali))

            Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO 'Diminuzione Consistenze
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.VariazioneConsistenzeZootecniche, enum_Pendenza.MovForzato))

            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.ScaricoGiustificatoDaResiSuAcquisti, enum_Pendenza.Resi_Acquisti))

            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.CaricoGiustificatoDaResiSuVendite, enum_Pendenza.Resi_Vendite))

            Case Else
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato, enum_Pendenza.MovPendente))
                Me.cmb_Causale.Items.Add(New ListItem(AgronicaAgenda_2010.RilevamentoGiacenzeInizialiDiMagazzino, enum_Pendenza.GiacenzeIniziali))

        End Select



    End Sub


    '###############################################################################
    Private Sub Pulisci_Giacenze_Prezzo()

        'Visualizzo il prezzo unitario nella text box
        'Me.Txt_PrezzoUnitario.Text = "0,00"
        'Me.Txt_PrezzoUnitario_Netto.Text = "0,00"
        Me.Txt_Giacenza_Destinazione.Text = ""
        '  Me.Txt_Giacenza_Destinazione_Tot.Text = ""
        Me.Txt_Giacenza_Provenienza.Text = ""
        '   Me.Txt_Giacenza_Provenienza_Tot.Text = ""

    End Sub

    '###############################################################################
    Private Sub Pulisci_Dettaglio_Fertilizzante()

        Me.Txt_N.Text = ""
        Me.Txt_P2O5.Text = ""
        Me.Txt_K2O.Text = ""
        Me.Txt_Cu.Text = ""

    End Sub

    '###############################################################################
    Private Sub Dettaglio_Fertilizzante_Visibilita(ByVal visibile As Boolean)

        Me.lbl_N.Visible = visibile
        Me.lbl_P2O5.Visible = visibile
        Me.lbl_K2O.Visible = visibile
        Me.lbl_Cu.Visible = visibile

        Me.Txt_N.Visible = visibile
        Me.Txt_P2O5.Visible = visibile
        Me.Txt_K2O.Visible = visibile
        Me.Txt_Cu.Visible = visibile

    End Sub

    '###############################################################################
    Private Sub Dettaglio_Fertilizzante_Attivo(ByVal attivo As Boolean)

        Me.Txt_N.Enabled = attivo
        Me.Txt_P2O5.Enabled = attivo
        Me.Txt_K2O.Enabled = attivo
        Me.Txt_Cu.Enabled = attivo

    End Sub

    '###############################################################################
    Private Sub Pulisci_ChiaveProdotto()

        Me.cmb_Categoria.SelectedIndex = 0
        Me.cmb_Categoria_SelectedIndexChanged(Me, Nothing)

        'il resto si pulisce dentro l'evento della categoria


    End Sub



    '####################################################################################################################
    Private Sub Carica_Giacenze_Prezzo()

        Dim Flag_GiacenzaEsiste As Boolean
        Dim StrGiacenza As String = ""
        'Dim StrGiacenzaTot As String
        Dim Id_Magazzino, Sa_Cod, Mag_Index As Integer
        Dim Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod As Integer

        Dim Giacenza_Magazzino As Decimal = 0
        'Dim Giacenza_Centro As Decimal = 0

        ViewState("Giacenza_Magazzino") = 0

        Elem_Cod = CInt(Me.cmb_Categoria.SelectedValue)

        If cmb_Udm.SelectedIndex > 0 Then
            Udm_Cod = cmb_Udm.SelectedItem.Value
        Else
            Udm_Cod = 0
        End If

        Dim codiceProdotto As String = Me.cmb_Prodotti.SelectedItem.Value
        If codiceProdotto Is Nothing OrElse codiceProdotto = "" Then
            'i18n
            Throw New Exception("Errore nel recuperare il prodotto [codice prodotto stringa vuota]. Contattare l'amministratore.")
        End If

        If codiceProdotto.Split("|")(0) < 0 Then
            Pro_Cod = 0
            Mat_Cod = -CInt(codiceProdotto)
        Else
            Pro_Cod = codiceProdotto.Split("|")(0)
            Mat_Cod = 0
        End If

        If Chk_LottoImpianto.Checked Then
            xCod_Progetto = CODPROGETTO_NONDEFINITO
        Else
            If Me.cmb_Lotto.SelectedIndex > 0 Then
                xCod_Progetto = Me.cmb_Lotto.SelectedItem.Value
            Else
                xCod_Progetto = 0
            End If
        End If


        '-----
        'LOTTO DI ACCETTAZIONE
        If Me.cmb_LottoAccettazione.SelectedIndex < 1 Then
            If xCaricoScarico = CAU_CARICO Then
                xLotto = Me.Txt_LottoAccettazione.Text
            Else
                'xLotto = ""
                xLotto = LOTTO_NONDEFINITO
            End If
        Else
            '19/06/2018: nel caso di categorie che hanno il lotto non calcolava bene la giacenza
            'lotto non definito
            xLotto = cmb_LottoAccettazione.SelectedItem.Value
            If xLotto = "-1" OrElse IsNothing(xLotto) Then
                '  xLotto = ""
                xLotto = LOTTO_NONDEFINITO
            End If
            ''lotto non definito
            'If xLotto = "-1" OrElse IsNothing(xLotto) Then
            '    '  xLotto = ""
            '    xLotto = LOTTO_NONDEFINITO
            'Else
            '    xLotto = cmb_LottoAccettazione.SelectedItem.Value
            'End If
        End If

        'If cmb_LottoAccettazione.SelectedIndex > 0 Then
        '    'lotto non definito
        '    If xLotto = "-1" Or IsNothing(xLotto) Then
        '        xLotto = ""
        '    Else
        '        xLotto = cmb_LottoAccettazione.SelectedItem.Value
        '    End If
        'Else
        '    xLotto = ""
        'End If

        If Chk_ParametroQualitativo.Checked Then
            xCod_Calibro = 0
        Else

            If cmb_Calibro.SelectedIndex > 0 Then
                xCod_Calibro = cmb_Calibro.SelectedItem.Value
            Else
                xCod_Calibro = 0
            End If
        End If


        If xCaricoScarico = CAU_CARICO Then

            Id_Magazzino = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(0)
            Sa_Cod = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(1)
            Mag_Index = Me.Cmb_Destinazione.SelectedIndex

        ElseIf xCaricoScarico = CAU_SCARICO Then

            Id_Magazzino = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(0)
            Sa_Cod = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(1)
            Mag_Index = Me.Cmb_Provenienza.SelectedIndex


        ElseIf xCaricoScarico = CAU_TRASFERIMENTO Then

            Id_Magazzino = 9999

        End If

        Dim Data_Verifica As Date
        Data_Verifica = Me.Txt_DataMovimento.Text

        If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
            Qta_Modifica_Giacenza = Me.Txt_Quantita.Text
        End If

        '======================================================================
        'Lettura della giacenza del prodotto selezionato in magazzino
        '----------------------------------------------------------------------

        If (Pro_Cod = 0 AndAlso Mat_Cod = 0) OrElse Id_Magazzino = 0 Then

            Me.Txt_Giacenza_Destinazione.Text = ""
            '   Me.Txt_Giacenza_Destinazione_Tot.Text = ""
            Me.Txt_Giacenza_Provenienza.Text = ""
            '   Me.Txt_Giacenza_Provenienza_Tot.Text = ""

        Else

            Flag_GiacenzaEsiste = False

            Select Case Elem_Cod

                Case 1 'Macchine/Attrezzature

                    'Non Esistono Giacenze in Magazzino
                    'DO NOTHING

                    '########################################################

                Case ZOO_CONSISTENZA 'Macchine/Attrezzature


                    '########################################################

                Case Else


                    Select Case xCaricoScarico

                        Case CAU_TRASFERIMENTO

                            Dim i As Integer
                            Dim objG As New AgronicaCoreContabDAL.Giacenze_R

                            For i = 0 To 1

                                If i = 0 Then

                                    Id_Magazzino = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(0)
                                    Sa_Cod = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")(1)
                                    Mag_Index = Me.Cmb_Provenienza.SelectedIndex

                                ElseIf i = 1 Then
                                    Id_Magazzino = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(0)
                                    Sa_Cod = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")(1)
                                    Mag_Index = Me.Cmb_Destinazione.SelectedIndex
                                End If



                                'Nota: In caso di prodotto aziendale l'unita di misura non è specificata, ma
                                '      cmq è definita una sua giacenza in magazzino

                                If (Udm_Cod <> 0 OrElse (Elem_Cod = 200 AndAlso Mat_Cod <> 0)) AndAlso Id_Magazzino <> 0 Then

                                    'controllo se ci sono giacenze
                                    Dim Dt_Giacenze As DataTable
                                    'Dim Filtro_Aggiuntivo As String = ""
                                    'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                                    '    Filtro_Aggiuntivo = " AND Movimenti_dettagli.Extra_Date <= " & SQL_SaveDate(CDate(Txt_DataMovimento.Text))
                                    'End If

                                    'legge le giacenze 


                                    Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Verifica,
                                                                                xPiva,
                                                                                Sa_Cod,
                                                                                Id_Magazzino,
                                                                                Elem_Cod,
                                                                                Pro_Cod,
                                                                                Mat_Cod,
                                                                                xCod_Calibro,
                                                                                xCod_Progetto,
                                                                                0,
                                                                                Udm_Cod,
                                                                                xLotto,
                                                                                False,
                                                                                "",
                                                                                "", "", "", "", "", "", "", "", "", "",
                                                                                "",
                                                                                objParametri_Server, objParametri_Utenti)


                                    If Not IsNothing(Dt_Giacenze) Then

                                        If Dt_Giacenze.Rows.Count <> 0 Then

                                            Giacenza_Magazzino = Dt_Giacenze.Rows(0).Item("Giacenza")
                                            'Giacenza_Centro = Dt_Giacenze.Rows(0).Item("Dett_Qta")

                                            If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then

                                                If i = 0 Then
                                                    'provenienza

                                                    'se sono in modifica di un trasferimento 
                                                    'e sono nel caso del magazzino di provenienza
                                                    'aggiungo alla giacenza la qta specificata nell'operazione
                                                    'così ho la giacenza reale prima dell'operazione
                                                    Giacenza_Magazzino += Qta_Modifica_Giacenza
                                                    ' Giacenza_Centro += Qta_Modifica_Giacenza

                                                    ViewState("Giacenza_Magazzino") = Giacenza_Magazzino

                                                ElseIf i = 1 Then
                                                    'destinazione

                                                    'se sono in modifica di un trasferimento 
                                                    'e sono nel caso del magazzino di destinazione
                                                    'sottraggo alla giacenza la qta specificata nell'operazione
                                                    'così ho la giacenza reale prima dell'operazione
                                                    Giacenza_Magazzino = Giacenza_Magazzino - Qta_Modifica_Giacenza
                                                    ' Giacenza_Centro = Giacenza_Centro - Qta_Modifica_Giacenza

                                                End If

                                            Else
                                                'scrittura
                                                If i = 0 Then
                                                    'provenienza
                                                    ViewState("Giacenza_Magazzino") = Giacenza_Magazzino
                                                End If
                                            End If

                                            StrGiacenza = Me.cmb_Udm.SelectedItem.Text & " " & Format(Giacenza_Magazzino, "##,###,##0.0000")

                                            '   StrGiacenzaTot = Me.cmb_Udm.SelectedItem.Text & " " & CStr(Giacenza_Centro)

                                            Flag_GiacenzaEsiste = True

                                            'modifica del 27/10/2010: commentato questo pezzo, 
                                            'perché altrimenti nil prezzo dell'operazione
                                            'viene sovrascritto con il prezzo salvato nel record delle giacenze
                                            '(che non ha senso)
                                            ''====================================================================
                                            ''Aggiornamento del Prezzo Unitario Ponderato del Prodotto
                                            ''--------------------------------------------------------------------
                                            'Me.Txt_PrezzoUnitario.Text = CStr(Dt_Giacenze.Rows(0).Item("Prezzo_Unitario"))
                                            'Me.Txt_PrezzoUnitario_Netto.Text = CStr(Dt_Giacenze.Rows(0).Item("Prezzo_Unitario_Netto"))
                                            ''====================================================================

                                        Else

                                            Flag_GiacenzaEsiste = False
                                            StrGiacenza = ""
                                            ' StrGiacenzaTot = ""

                                            ''devo aggiungere la qta oggetto della modifica anche in questo caso
                                            ''perchp può darsi che alla data non ci sia la gicenza
                                            ''e quindi va in questo else
                                            'If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
                                            '    If i = 0 Then
                                            '        'provenienza

                                            '        'se sono in modifica di un trasferimento 
                                            '        'e sono nel caso del magazzino di provenienza
                                            '        'aggiungo alla giacenza la qta specificata nell'operazione
                                            '        'così ho la giacenza reale prima dell'operazione
                                            '        Giacenza_Magazzino = Giacenza_Magazzino + Qta_Modifica_Giacenza
                                            '        ' Giacenza_Centro = Giacenza_Centro + Qta_Modifica_Giacenza

                                            '        viewstate("Giacenza_Magazzino") = Giacenza_Magazzino

                                            '    ElseIf i = 1 Then
                                            '        'destinazione

                                            '        'se sono in modifica di un trasferimento 
                                            '        'e sono nel caso del magazzino di destinazione
                                            '        'sottraggo alla giacenza la qta specificata nell'operazione
                                            '        'così ho la giacenza reale prima dell'operazione
                                            '        Giacenza_Magazzino = Giacenza_Magazzino - Qta_Modifica_Giacenza
                                            '        ' Giacenza_Centro = Giacenza_Centro - Qta_Modifica_Giacenza
                                            '    End If

                                            '    StrGiacenza = Me.cmb_Udm.SelectedItem.Text & " " & Format(Giacenza_Magazzino, "##,###,##0.0000")
                                            '    Flag_GiacenzaEsiste = True
                                            'Else
                                            '    Flag_GiacenzaEsiste = False
                                            '    StrGiacenza = ""
                                            '    ' StrGiacenzaTot = ""
                                            'End If

                                        End If

                                    Else
                                        Flag_GiacenzaEsiste = False
                                        StrGiacenza = ""
                                        '   StrGiacenzaTot = ""
                                    End If

                                End If

                                If Not Flag_GiacenzaEsiste Then

                                    Select Case Mag_Index

                                        Case -1 'Nessun Magazzino Imputato

                                            StrGiacenza = AgronicaAgenda_2010.NonDisponibile

                                        Case Else

                                            StrGiacenza = AgronicaAgenda_2010.NessunCaricoPrecedente 'Inizializzazione

                                    End Select

                                End If


                                If i = 0 Then

                                    Me.Txt_Giacenza_Provenienza.Text = StrGiacenza
                                    ' Me.Txt_Giacenza_Provenienza_Tot.Text = StrGiacenzaTot

                                    'aggiungo il controllo sulle giacenze ed eventualmente do errore
                                    If Not Flag_GiacenzaEsiste Then
                                        If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
                                            Pannello_Salvataggio.Visible = False
                                            '       ImgBtnSalvaTutto.Visible = False
                                            'MESSAGGIO DI ERRORE
                                            Call Messaggi.AgroMsgBox(AgronicaAgenda_2010.ImpossibileSalvareTrasferimentoModificareDataDiRegistrazione_, Page, , Me.upDati)
                                        End If
                                    End If

                                ElseIf i = 1 Then

                                    Me.Txt_Giacenza_Destinazione.Text = StrGiacenza
                                    '  Me.Txt_Giacenza_Destinazione_Tot.Text = StrGiacenzaTot

                                End If


                                StrGiacenza = ""
                                'StrGiacenzaTot = ""
                                Flag_GiacenzaEsiste = False

                            Next

                            '///////////////////////////////////////////////////////////

                        Case Else

                            'Controllo che i parametri siano completi

                            'Nota: In caso di prodotto aziendale l'unita di misura non è specificata, ma
                            '      cmq è definita una sua giacenza in magazzino

                            If (Udm_Cod <> 0 OrElse (Elem_Cod = 200 AndAlso Mat_Cod <> 0)) AndAlso Id_Magazzino <> 0 Then

                                'controllo se ci sono giacenze
                                Dim Dt_Giacenze As DataTable
                                Dim objG As New AgronicaCoreContabDAL.Giacenze_R

                                ''legge le giacenze 

                                Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Verifica,
                                                                xPiva,
                                                                Sa_Cod,
                                                                Id_Magazzino,
                                                                Elem_Cod,
                                                                Pro_Cod,
                                                                Mat_Cod,
                                                                xCod_Calibro,
                                                                xCod_Progetto,
                                                                0,
                                                                Udm_Cod,
                                                                xLotto,
                                                                False,
                                                                "",
                                                                "", "", "", "", "", "", "", "", "", "",
                                                                "",
                                                                objParametri_Server, objParametri_Utenti)


                                If Not IsNothing(Dt_Giacenze) Then

                                    If Dt_Giacenze.Rows.Count <> 0 Then

                                        Giacenza_Magazzino = Dt_Giacenze.Rows(0).Item("Giacenza")
                                        ' Giacenza_Centro = Dt_Giacenze.Rows(0).Item("Dett_Qta")

                                        If Chk_LottoImpianto.Checked OrElse Chk_ParametroQualitativo.Checked Then
                                            For kk = 1 To Dt_Giacenze.Rows.Count - 1
                                                Giacenza_Magazzino += Dt_Giacenze.Rows(kk).Item("Giacenza")
                                            Next
                                        End If

                                        If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
                                            If xCaricoScarico = enum_Agenda_Causali.CARICO Then
                                                'se sono in modifica di un carico 
                                                'sottraggo alla giacenza la qta specificata nell'operazione
                                                'così ho la giacenza reale prima dell'operazione
                                                Giacenza_Magazzino = Giacenza_Magazzino - Qta_Modifica_Giacenza
                                                '  Giacenza_Centro = Giacenza_Centro - Qta_Modifica_Giacenza
                                            Else
                                                'se sono in modifica di un scarico 
                                                'aggiungo alla giacenza la qta specificata nell'operazione
                                                'così ho la giacenza reale prima dell'operazione
                                                Giacenza_Magazzino = Giacenza_Magazzino + Qta_Modifica_Giacenza
                                                '   Giacenza_Centro = Giacenza_Centro + Qta_Modifica_Giacenza
                                            End If
                                        End If

                                        ViewState("Giacenza_Magazzino") = Giacenza_Magazzino

                                        ' StrGiacenza = Me.cmb_Udm.SelectedItem.Text & " " & CStr(Giacenza_Magazzino)
                                        StrGiacenza = Me.cmb_Udm.SelectedItem.Text & " " & Format(Giacenza_Magazzino, "##,###,##0.####")

                                        '  StrGiacenzaTot = Me.cmb_Udm.SelectedItem.Text & " " & CStr(Giacenza_Centro)

                                        Flag_GiacenzaEsiste = True

                                        'modifica del 27/10/2010: commentato questo pezzo, 
                                        'perché altrimenti nil prezzo dell'operazione
                                        'viene sovrascritto con il prezzo salvato nel record delle giacenze
                                        '(che non ha senso)
                                        ''====================================================================
                                        ''Aggiornamento del Prezzo Unitario Ponderato del Prodotto
                                        ''--------------------------------------------------------------------
                                        'Me.Txt_PrezzoUnitario.Text = CStr(Dt_Giacenze.Rows(0).Item("Prezzo_Unitario"))
                                        'Me.Txt_PrezzoUnitario_Netto.Text = CStr(Dt_Giacenze.Rows(0).Item("Prezzo_Unitario_Netto"))
                                        ''====================================================================

                                    Else
                                        'If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
                                        '    If xCaricoScarico = enum_Agenda_Causali.CARICO Then
                                        '        'se sono in modifica di un carico 
                                        '        'sottraggo alla giacenza la qta specificata nell'operazione
                                        '        'così ho la giacenza reale prima dell'operazione
                                        '        Giacenza_Magazzino = Giacenza_Magazzino - Qta_Modifica_Giacenza
                                        '        '  Giacenza_Centro = Giacenza_Centro - Qta_Modifica_Giacenza
                                        '    Else
                                        '        'se sono in modifica di un scarico 
                                        '        'aggiungo alla giacenza la qta specificata nell'operazione
                                        '        'così ho la giacenza reale prima dell'operazione
                                        '        Giacenza_Magazzino = Giacenza_Magazzino + Qta_Modifica_Giacenza
                                        '        '   Giacenza_Centro = Giacenza_Centro + Qta_Modifica_Giacenza
                                        '    End If
                                        '    StrGiacenza = Me.cmb_Udm.SelectedItem.Text & " " & Format(Giacenza_Magazzino, "##,###,##0.####")
                                        '    Flag_GiacenzaEsiste = True
                                        '    viewstate("Giacenza_Magazzino") = Giacenza_Magazzino
                                        'End If
                                        'modifica del 27/10/2010: commentato questo pezzo, 
                                        'perché altrimenti nil prezzo dell'operazione
                                        'viene sovrascritto con il prezzo salvato nel record delle giacenze
                                        '(che non ha senso)
                                        'Me.Txt_PrezzoUnitario.Text = CStr(0)
                                        'Me.Txt_PrezzoUnitario_Netto.Text = CStr(0)
                                    End If

                                Else
                                    'modifica del 27/10/2010: commentato questo pezzo, 
                                    'perché altrimenti nil prezzo dell'operazione
                                    'viene sovrascritto con il prezzo salvato nel record delle giacenze
                                    '(che non ha senso)
                                    'Me.Txt_PrezzoUnitario.Text = CStr(0)
                                    'Me.Txt_PrezzoUnitario_Netto.Text = CStr(0)
                                End If

                            End If

                            If Not Flag_GiacenzaEsiste Then

                                Select Case Mag_Index

                                    Case -1 'Nessun Magazzino Imputato

                                        StrGiacenza = AgronicaAgenda_2010.NonDisponibile

                                    Case Else

                                        StrGiacenza = AgronicaAgenda_2010.NessunCaricoPrecedente 'Inizializzazione

                                End Select

                            End If

                            If xCaricoScarico = CAU_CARICO Then
                                Me.Txt_Giacenza_Destinazione.Text = StrGiacenza
                                '  Me.Txt_Giacenza_Destinazione_Tot.Text = StrGiacenzaTot
                            ElseIf xCaricoScarico = CAU_SCARICO Then
                                Me.Txt_Giacenza_Provenienza.Text = StrGiacenza
                                '   Me.Txt_Giacenza_Provenienza_Tot.Text = StrGiacenzaTot
                            End If

                            '///////////////////////////////////////////////////////////


                    End Select

                    '########################################################


            End Select

        End If


    End Sub



    '########################################################################################
    Private Sub Cmb_Animali_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Animali.SelectedIndexChanged

        Dim ArrayAnimali As String()
        Dim Gen_Cod As Integer
        Dim Spe_Cod As Integer

        Cmb_IndirizzoProduttivo.Items.Clear()

        If Me.Cmb_Animali.SelectedItem.Text <> "" Then
            ArrayAnimali = Split(Cmb_Animali.SelectedItem.Value, "|")
            Gen_Cod = ArrayAnimali(0)
            Spe_Cod = ArrayAnimali(1)
        End If

        'CaricaCombo_Lista_IndirizziProd_Animali( _
        '                                    Server, Session, Page, _
        '                                    Me.Cmb_IndirizzoProduttivo, _
        '                                    Gen_Cod, _
        '                                    Spe_Cod)


        AgronicaCoreUtility.CaricaListControl.Lista_IndirizziProd_Animali(Me.Cmb_IndirizzoProduttivo,
                                True,
                                "",
                                "",
                                Gen_Cod,
                                Spe_Cod,
                                "",
                                "",
                                objParametri_Server)


    End Sub

    '########################################################################################
    Private Sub CaricaGriglia_Matricole()

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Gen_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Spe_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ipro_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Raz_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cat_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Spe_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ipro_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Raz_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Matricola", GetType(String)))
        Dt.Columns.Add(New DataColumn("Nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("Collare", GetType(String)))
        Dt.Columns.Add(New DataColumn("Nome_Aia", GetType(String)))
        Dt.Columns.Add(New DataColumn("Matricola_Aia", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dat_Nascita", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Prov_Nascita", GetType(String)))
        Dt.Columns.Add(New DataColumn("Stato_Nascita", GetType(String)))
        Dt.Columns.Add(New DataColumn("AUA_AZI_NASCITA", GetType(String)))
        Dt.Columns.Add(New DataColumn("AUSL_AZI_NASCITA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sesso", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mat_Padre", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mat_Madre", GetType(String)))
        Dt.Columns.Add(New DataColumn("Peso", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Data_Pesa", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Metodo_Produzione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Metodo_Produzione_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Conversione_Inizio", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Conversione_Fine", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Qta_Reale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Prezzo_Reale", GetType(Decimal)))


        Dim DtKeys(0) As DataColumn

        ''Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Matricola")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Associo il DataTable con la DataGrid

        Me.DataGrid_Prodotti.DataSource = Dt
        Me.DataGrid_Prodotti.DataBind()

        '----- Salvo il DataTable dentro il viewstate

        ViewState("vs_dtMatricole") = Dt

    End Sub


    '########################################################################################
    Private Sub Aggiungi_Matricola_New()

        'NUOVA GESTIONE
        'DA USARE QUANDO SI FARANNO LE MODIFICHE PER GESTIRE I DETTAGLI ECONOMICI
        'E QUANDO SI GESTIRANNO LA MODIFICA E LA CANCELLAZIONE

        Dim strZoo_Animale_Anagrafe As String
        'Dim Metodo_Produzione_Des As String

        Dim XmlDoc As New XmlDocument
        Dim XML_Zoo_Animale_Anagrafe As XmlElement

        'Dim Dt As DataTable
        'Dim Dr As DataRow

        If Session("Zoo_Animale_Anagrafe") IsNot Nothing Then

            strZoo_Animale_Anagrafe = Session("Zoo_Animale_Anagrafe").ToString

            'Session("Zoo_Animale_Anagrafe") = Nothing


            'Carico la stringa nel documento XML
            XmlDoc.LoadXml(strZoo_Animale_Anagrafe)

            '----- Tag Zoo_Animale_Anagrafe

            XML_Zoo_Animale_Anagrafe = XmlDoc.SelectSingleNode("Zoo_Animale_Anagrafe")

            'Metodo_Produzione_Des = "Convenzionale"

            'Select Case XML_Zoo_Animale_Anagrafe.GetAttribute("metodo_produzione")
            '    Case 1
            '        Metodo_Produzione_Des = "Convenzionale"
            '    Case 2
            '        Metodo_Produzione_Des = "In Conversione"
            '    Case 3
            '        Metodo_Produzione_Des = "Biologico"
            'End Select


            'Aggiungo la Razza
            Me.Cmb_Razza.Items.Clear()
            Me.Cmb_Razza.Items.Add(New ListItem(XML_Zoo_Animale_Anagrafe.GetAttribute("raz_des"), XML_Zoo_Animale_Anagrafe.GetAttribute("raz_cod")))
            Me.Cmb_Razza.SelectedIndex = 1

            'Aggiungo la Matricola
            Me.Cmb_Matricola.Items.Clear()
            Me.Cmb_Matricola.Items.Add(New ListItem(XML_Zoo_Animale_Anagrafe.GetAttribute("matricola"), XML_Zoo_Animale_Anagrafe.GetAttribute("cod_progetto")))
            Me.Cmb_Matricola.SelectedIndex = 1

            'Inserisci_Riga_Matricola(XML_Zoo_Animale_Anagrafe.GetAttribute("cod_progetto"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("gen_cod"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("spe_cod"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("ipro_cod"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("raz_cod"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("cat_cod"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("spe_des"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("ipro_des"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("raz_des"), _
            '                        "", _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("matricola"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("nome"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("collare"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("nome_aia"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("matricola_aia"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("dat_nascita"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("prov_nascita"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("stato_nascita"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("aua_azi_nascita"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("ausl_azi_nascita"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("sesso"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("mat_padre"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("mat_madre"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("peso"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("data_pesa"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("metodo_produzione"), _
            '                        Metodo_Produzione_Des, _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("conversione_inizio"), _
            '                        XML_Zoo_Animale_Anagrafe.GetAttribute("conversione_fine"), _
            '                        0, _
            '                        0)


        End If


    End Sub





    '########################################################################################
    Private Sub Aggiungi_Matricola()

        Dim strZoo_Animale_Anagrafe As String

        Dim XmlDoc As New XmlDocument
        Dim XML_Zoo_Animale_Anagrafe As XmlElement

        Dim Metodo_Produzione_Des As String

        If Session("Zoo_Animale_Anagrafe") IsNot Nothing Then

            strZoo_Animale_Anagrafe = Session("Zoo_Animale_Anagrafe").ToString

            Session("Zoo_Animale_Anagrafe") = Nothing

            'Carico la stringa nel documento XML
            XmlDoc.LoadXml(strZoo_Animale_Anagrafe)

            '----- Tag Zoo_Animale_Anagrafe

            XML_Zoo_Animale_Anagrafe = XmlDoc.SelectSingleNode("Zoo_Animale_Anagrafe")

            Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale

            Select Case XML_Zoo_Animale_Anagrafe.GetAttribute("metodo_produzione")
                Case 1
                    Metodo_Produzione_Des = AgronicaAgenda_2010.Convenzionale
                Case 2
                    Metodo_Produzione_Des = AgronicaAgenda_2010.InConversione
                Case 3
                    Metodo_Produzione_Des = AgronicaAgenda_2010.Biologico
            End Select


            Inserisci_Riga_Matricola(XML_Zoo_Animale_Anagrafe.GetAttribute("cod_progetto"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("gen_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("spe_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("ipro_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("raz_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("cat_cod"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("spe_des"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("ipro_des"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("raz_des"),
                                    "",
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("matricola"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("nome"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("collare"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("nome_aia"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("matricola_aia"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("dat_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("prov_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("stato_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("aua_azi_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("ausl_azi_nascita"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("sesso"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("mat_padre"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("mat_madre"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("peso"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("data_pesa"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("metodo_produzione"),
                                    Metodo_Produzione_Des,
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("conversione_inizio"),
                                    XML_Zoo_Animale_Anagrafe.GetAttribute("conversione_fine"),
                                    0,
                                    0)

        End If

    End Sub



    '########################################################################################
    Private Sub Inserisci_Riga_Matricola(ByVal Cod_Progetto As Integer,
                                        ByVal Gen_Cod As Integer,
                                        ByVal Spe_Cod As Integer,
                                        ByVal Ipro_Cod As Integer,
                                        ByVal Raz_Cod As Integer,
                                        ByVal Cat_Cod As Integer,
                                        ByVal Spe_Des As String,
                                        ByVal Ipro_Des As String,
                                        ByVal Raz_Des As String,
                                        ByVal Cat_Des As String,
                                        ByVal Matricola As String,
                                        ByVal Nome As String,
                                        ByVal Collare As String,
                                        ByVal Nome_Aia As String,
                                        ByVal Matricola_Aia As String,
                                        ByVal Dat_Nascita As Date,
                                        ByVal Prov_Nascita As String,
                                        ByVal Stato_Nascita As String,
                                        ByVal AUA_AZI_NASCITA As String,
                                        ByVal AUSL_AZI_NASCITA As String,
                                        ByVal Sesso As String,
                                        ByVal Mat_Padre As String,
                                        ByVal Mat_Madre As String,
                                        ByVal Peso As Decimal,
                                        ByVal Data_Pesa As Date,
                                        ByVal Metodo_Produzione As Integer,
                                        ByVal Metodo_Produzione_Des As String,
                                        ByVal Conversione_Inizio As Date,
                                        ByVal Conversione_Fine As Date,
                                        ByVal Qta_Reale As Decimal,
                                        ByVal Prezzo_Reale As Decimal)



        Dim Dt As DataTable
        Dim Dr As DataRow

        'Recupero il datatable
        Dt = ViewState("vs_dtMatricole")

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Cod_Progetto") = Cod_Progetto
        Dr.Item("Gen_Cod") = Gen_Cod
        Dr.Item("Spe_Cod") = Spe_Cod
        Dr.Item("Ipro_Cod") = Ipro_Cod
        Dr.Item("Raz_Cod") = Raz_Cod
        Dr.Item("Cat_Cod") = Cat_Cod
        Dr.Item("Spe_Des") = Spe_Des
        Dr.Item("Ipro_Des") = Ipro_Des
        Dr.Item("Raz_Des") = Raz_Des
        Dr.Item("Cat_Des") = Cat_Des
        Dr.Item("Matricola") = Matricola
        Dr.Item("Nome") = Nome
        Dr.Item("Collare") = Collare
        Dr.Item("Nome_Aia") = Nome_Aia
        Dr.Item("Matricola_Aia") = Matricola_Aia
        Dr.Item("Dat_Nascita") = Dat_Nascita
        Dr.Item("Prov_Nascita") = Prov_Nascita
        Dr.Item("Stato_Nascita") = Stato_Nascita
        Dr.Item("AUA_AZI_NASCITA") = AUA_AZI_NASCITA
        Dr.Item("AUSL_AZI_NASCITA") = AUSL_AZI_NASCITA
        Dr.Item("Sesso") = Sesso
        Dr.Item("Mat_Padre") = Mat_Padre
        Dr.Item("Mat_Madre") = Mat_Madre
        Dr.Item("Peso") = Peso
        Dr.Item("Data_Pesa") = Data_Pesa
        Dr.Item("Metodo_Produzione") = Metodo_Produzione
        Dr.Item("Metodo_Produzione_Des") = Metodo_Produzione_Des
        Dr.Item("Conversione_Inizio") = Conversione_Inizio
        Dr.Item("Conversione_Fine") = Conversione_Fine
        Dr.Item("Qta_Reale") = Qta_Reale
        Dr.Item("Prezzo_Reale") = Prezzo_Reale

        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)

        '----- Associo il DataTable con la DataGrid

        Me.DataGrid_ZooAnimali.DataSource = Dt
        Me.DataGrid_ZooAnimali.DataBind()

        '----- Salvo il DataTable dentro il viewstate

        ViewState("vs_dtMatricole") = Dt


    End Sub


    Private Sub cmb_Causale_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmb_Causale.SelectedIndexChanged

        If cmb_Causale.SelectedValue = CStr(enum_Pendenza.Furto) Then

            'se sono in causale furto allora abilito le textbox per scrivere la denuncia
            denuncia.Visible = True
        Else
            denuncia.Visible = False
            Txt_NumeroDenuncia.Text = ""
            Txt_DataDenuncia.Text = CDate("01/01/1900")
        End If

    End Sub

    Private Sub inzializzadate()

        Dim str As String = " $(document).ready(function () { "
        str &= "$('#" & Txt_DataMovimento.ClientID & "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });"
        str &= "  $('#" & Txt_DataMovimento.ClientID & "').change(function () {  $('#" & BTN_ChangeData.ClientID & "').click(); });"
        str &= "});"
        ScriptManager.RegisterStartupScript(upDati, upDati.GetType(),
                                         String.Format("jQuery_{0}", Txt_DataMovimento.ClientID), str, True)

        str = " $(document).ready(function () { "
        str &= "$('#" & Txt_DataDenuncia.ClientID & "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });"
        str &= "  $('#" & Txt_DataDenuncia.ClientID & "').change(function () {  $('#" & BTN_ChangeDataDenuncia.ClientID & "').click(); });"
        str &= "});"
        ScriptManager.RegisterStartupScript(upDati, upDati.GetType(),
                                         String.Format("jQuery_{0}", Txt_DataDenuncia.ClientID), str, True)
    End Sub

    Protected Sub BTN_ChangeData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ChangeData.Click

        If IsDate(Txt_DataMovimento.Text) Then

            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean = True
            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
            objPratiche.Data_Sportello_Da_Servizio(xPiva,
                                                    enum_Servizi.Quaderno_Campagna_Caa,'Qs_Servizio_Cod,
                                                    DateTime.Now,
                                                    SportelloAperto,
                                                    dataMin,
                                                    dataMax,
                                                    objParametri_Server,
                                                    objParametri_Utenti)

            objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(xPiva,
                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                        DateTime.Now,
                                                                        True,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        SportelloAperto,
                                                                        dataMin,
                                                                        dataMax,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti)
            If Txt_DataMovimento.Text < dataMin Then
                Txt_DataMovimento.Text = dataMin.ToShortDateString
                Messaggi.AgroMsgBox("Non è possibile inserire un'operazione prima del " & dataMin.ToShortDateString & ". Sportello chiuso", Page, , Me.upDati)
            End If

            If Txt_DataMovimento.Text > dataMax Then
                Txt_DataMovimento.Text = dataMax.ToShortDateString
                Messaggi.AgroMsgBox("Non è possibile inserire un'operazione dopo il " & dataMax.ToShortDateString & ". Sportello chiuso", Page, , Me.upDati)
            End If

        End If

    End Sub

    Protected Sub BtnInfo_fito_Click(sender As Object, e As ImageClickEventArgs) Handles BtnInfo_fito.Click
        'Profitosan
        If cmb_Prodotti.SelectedValue <> "" Then

            Dim Link As String = profitosan.getLink2023(True, New AgronicaCoreGestioneRichieste.AgroWebConfig,
                                                        CInt(Me.cmb_Prodotti.SelectedValue.Split("|")(0)),
                                                        objParametri_Server, objParametri_Utenti)

            Dim Str As New StringBuilder
            Str.AppendLine("$(document).ready(function () {")
            Str.AppendLine("  window.open('" & Link & "' , ''); ")
            Str.AppendLine("});")

            ScriptManager.RegisterClientScriptBlock(upDati, upDati.GetType(),
                       String.Format("jQuery_{0}", upDati.ClientID), Str.ToString, True)
        End If

    End Sub

    Protected Sub BtnInfo_Concime_Click(sender As Object, e As ImageClickEventArgs) Handles BtnInfo_Concime.Click
        Dim fer_cod As Integer
        If CInt(Me.cmb_Prodotti.SelectedValue) > 1 Then
            fer_cod = Me.cmb_Prodotti.SelectedValue

            Dim rndClass As New Random
            Dim stringClass As String = "dialogPopUpInformativiFertilizzante_" & rndClass.Next

            dialogPupUpInformativiFertilizzante.CssClass = stringClass

            dialogPupUpInformativiFertilizzante.Controls.Add(New LiteralControl(
                                                            PopUpInformativi_Fertilizzante.getStringaInformazioniFertilizzanti(fer_cod)
                                                            ))

            Dim StrSelect As New StringBuilder
            StrSelect.AppendLine("$(document).ready(function () {  ")
            StrSelect.AppendLine("      $('." & stringClass & "').dialog({ ")
            StrSelect.AppendLine("              autoOpen: false,")
            StrSelect.AppendLine("              width: 500,")
            StrSelect.AppendLine("              modal: true,")
            StrSelect.AppendLine("              buttons: {")
            StrSelect.AppendLine("                  'Chiudi': function () {")
            StrSelect.AppendLine("                      $('." & stringClass & "').dialog('close');")
            StrSelect.AppendLine("                  }")
            StrSelect.AppendLine("              }")
            StrSelect.AppendLine("      });")
            StrSelect.AppendLine("      $('." & stringClass & "').dialog('open');")
            StrSelect.AppendLine("  });")

            ScriptManager.RegisterStartupScript(Me.upDati, upDati.GetType(),
                                        String.Format("jQuery_{0}", PopUpInformativi_Fertilizzante.ClientID),
                                        StrSelect.ToString(),
                                        True)



        End If
    End Sub

    'Public Sub Info_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles BtnInfo.Click

    '    'prodotti fitosanitari
    '    If Me.cmb_Categoria.SelectedValue = "191" Then
    '        'Profitosan
    '        Dim Link As String = profitosan.getLinkFRCOD(CInt(Me.cmb_Prodotti.SelectedValue.Split("|")(0)), True, Request, New AgronicaCoreGestioneRichieste.AgroWebConfig,
    '                                                           objParametri_Server, objParametri_Utenti)





    '        Dim Str As New StringBuilder
    '        Str.AppendLine("$(document).ready(function () {")
    '        Str.AppendLine("  window.open('" & Link & "' , ''); ")
    '        Str.AppendLine("});")

    '        ScriptManager.RegisterClientScriptBlock(upDati, upDati.GetType(),
    '                   String.Format("jQuery_{0}", upDati.ClientID), Str.ToString, True)


    '    End If


    '    'Fertilizzanti
    '    If Me.cmb_Categoria.SelectedValue = "3" Then

    '        Dim fer_cod As Integer
    '        If CInt(Me.cmb_Prodotti.SelectedValue) > 1 Then
    '            fer_cod = Me.cmb_Prodotti.SelectedValue

    '            Dim rndClass As New Random
    '            Dim stringClass As String = "dialogPopUpInformativiFertilizzante_" & rndClass.Next

    '            dialogPupUpInformativiFertilizzante.CssClass = stringClass

    '            dialogPupUpInformativiFertilizzante.Controls.Add(New LiteralControl(
    '                                                            PopUpInformativi_Fertilizzante.getStringaInformazioniFertilizzanti(fer_cod)
    '                                                            ))

    '            Dim StrSelect As New StringBuilder
    '            StrSelect.AppendLine("$(document).ready(function () {  ")
    '            StrSelect.AppendLine("      $('." & stringClass & "').dialog({ ")
    '            StrSelect.AppendLine("              autoOpen: false,")
    '            StrSelect.AppendLine("              width: 500,")
    '            StrSelect.AppendLine("              modal: true,")
    '            StrSelect.AppendLine("              buttons: {")
    '            StrSelect.AppendLine("                  'Chiudi': function () {")
    '            StrSelect.AppendLine("                      $('." & stringClass & "').dialog('close');")
    '            StrSelect.AppendLine("                  }")
    '            StrSelect.AppendLine("              }")
    '            StrSelect.AppendLine("      });")
    '            StrSelect.AppendLine("      $('." & stringClass & "').dialog('open');")
    '            StrSelect.AppendLine("  });")

    '            ScriptManager.RegisterStartupScript(Me.upDati, upDati.GetType(),
    '                                        String.Format("jQuery_{0}", PopUpInformativi_Fertilizzante.ClientID),
    '                                        StrSelect.ToString(),
    '                                        True)



    '        End If

    '    End If




    'End Sub

    Private Sub RipristinaUdmSelezionataInPrecedenza()
        If Not IsNothing(Session("UdmSelezionata")) AndAlso Session("UdmSelezionata") <> "" AndAlso Not IsNothing(cmb_Udm) Then
            'seleziono l'unita di misura specifica
            cmb_Udm.SelectedIndex =
                cmb_Udm.Items.IndexOf(cmb_Udm.Items.FindByValue(
                    Session("UdmSelezionata")))
        End If
    End Sub






    Private Function FormulatoRecuperaClassiTox(ByVal Fr_Cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByRef objSession As System.Web.SessionState.HttpSessionState
                                                          ) As Hashtable
        Dim ClassiTox As New Hashtable
        'Creo la stringa XML di richiesta

        Dim XmlDoc As New XmlDocument
        Dim XmlCredenziali As XmlElement
        Dim XmlParametri As XmlElement
        Dim XmlNodo As XmlElement
        Dim StringaXML As String

        Dim WsScheda As String
        Dim WsDoorKey As String
        Dim WsCodiceGias As String
        Dim WsUsernameSuperuser As String
        Dim WsPasswordSuperuser As String

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
        '   </CREDENZIALI>
        '===========================================================================================

        WsScheda = "1080"                                               'AWS_Fitofarmaci_Formulati_PrincipiAttivi
        WsDoorKey = "portone_grth45ksh4ghajnl32149"
        WsCodiceGias = objSession("ASG_ProgressivoGIAS")
        WsUsernameSuperuser = objSession("ASG_SuperUser_Username")         'Session("ASG_SuperUser_Username_Crypt")
        WsPasswordSuperuser = objSession("ASG_SuperUser_Password")         'Session("ASG_SuperUser_Password_Crypt")

        'Creo il nodo CREDENZIALI
        XmlCredenziali = XmlDoc.CreateElement("CREDENZIALI")

        'Imposto gli attributi
        XmlCredenziali.SetAttribute("scheda", CStr(WsScheda))
        XmlCredenziali.SetAttribute("doorkey", CStr(WsDoorKey))
        XmlCredenziali.SetAttribute("codicegias", CStr(WsCodiceGias))
        XmlCredenziali.SetAttribute("username", CStr(WsUsernameSuperuser))
        XmlCredenziali.SetAttribute("password", CStr(WsPasswordSuperuser))

        'Imposto XmlParametri come figlio del documento principale
        XmlDoc.AppendChild(XmlCredenziali)

        'Creo il nodo PARAMETRI
        XmlParametri = XmlDoc.CreateElement("PARAMETRI")

        'Imposto gli attributi
        XmlParametri.SetAttribute("fr_cod", Fr_Cod)

        'Imposto XmlParametri come figlio del documento principale
        XmlCredenziali.AppendChild(XmlParametri)

        'Restituisco in uscita la stringa creata
        StringaXML = XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlNodo = Nothing
        XmlParametri = Nothing
        'XmlDoc = Nothing

        '//////////////////////////////////////////////////////////
        '/////   Chiamo la funzione del webservice per recuperare la composizione
        '//////////////////////////////////////////////////////////

        Dim LinkWsFitofarmaci As String
        Dim WsRisposta As String = ""
        Dim objCoreWebService As New AgronicaCoreWebService.AgroWs
        StringaXML = objCoreWebService.AWS_Codifica_P(StringaXML)

        Try
            Dim WsFito As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objAgroWebConfig As New AgroWebConfig
            If objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci = "" Then
                LinkWsFitofarmaci = Server.MapPath("https://ws.netagronica.it/AgronicaWebService/AgroWS_Fitofarmaci.asmx")
            Else
                LinkWsFitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            WsFito.Url = LinkWsFitofarmaci
            WsFito.Timeout = 60000
            WsRisposta = WsFito.Formulati_PrincipiAttivi_2(StringaXML)

        Catch ex As Exception
            Return ClassiTox
            'Throw New Exception("Webservice fitofarmaci : " & ex.Message)
        End Try


        '//////////////////////////////////////////////////////////
        '/////   Decodifico il risultato ==> DataTable
        '//////////////////////////////////////////////////////////

        'Dim DTfor As New DataTable
        'DTfor = New DataTable
        'DTfor.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
        'DTfor.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
        'DTfor.Columns.Add(New DataColumn("Elenco_ClassiTossicologiche", GetType(String)))
        'DTfor.Columns.Add(New DataColumn("Elenco_PrincipiAttivi", GetType(String)))


        Dim XMLs_Formulati As XmlNodeList
        Dim XMLs_ClassiTossicologiche As XmlNodeList

        Dim XmlFormulato As XmlElement
        Dim XmlClasseTossicologica As XmlElement

        Dim ict As Integer = 0
        Dim Testo As String = ""

        XmlDoc.LoadXml(WsRisposta)

        'Recupero l'elenco dei Formulati
        XMLs_Formulati = XmlDoc.GetElementsByTagName("FORMULATO")

        If Not IsNothing(XMLs_Formulati) Then

            For i = 0 To XMLs_Formulati.Count - 1

                'Formulato i-esimo
                XmlFormulato = XMLs_Formulati.Item(i)


                '---------------------------------------------------
                'CLASSE TOSSICOLOGICA ==>  cod1§des1|cod2§des2
                'Recupero l'elenco delle Classi Tossicologiche

                XMLs_ClassiTossicologiche = XmlFormulato.GetElementsByTagName("CLASSE_TOSSICOLOGICA")
                Testo = ""
                For ict = 0 To XMLs_ClassiTossicologiche.Count - 1
                    XmlClasseTossicologica = XMLs_ClassiTossicologiche.Item(ict)
                    Dim cltoss_cod As String = CStr(XmlClasseTossicologica.GetAttribute("cltoss_cod"))
                    Dim cltoss_des As String = CStr(XmlClasseTossicologica.GetAttribute("cltoss_des"))
                    ClassiTox.Add(cltoss_cod, cltoss_des)
                Next

            Next

        End If 'XMLs_Formulati

        Return ClassiTox

    End Function


    Private Function VerificaPermessoClasseToxPatentino(ByVal Data_Validita As Date, ByVal Fr_Cod As Integer, ByVal PivaAzienda As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef msg As String) As Boolean
        'Per acquistare e impiegare prodotti fitosanitari classificati come 
        'MOLTO TOSSICI, TOSSICI e NOCIVI è necessaria un'apposita autorizzazione comunemente nota come "Patentino" (D.P.R. 290/01).


        'controllo impostazione utente
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt As DataTable
        Dt = objUtenti.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_ProdottiTossiciPatentinoMovimenti,
                 1,
                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                 "",
                 "",
                 objParametri_Utenti)


        Dim i As Integer = 0
        Dim str As String = ""

        'se esiste l'impostazione utilizzo quella altrimenti metto dei default (x i nuovi utenti per esempio)
        If Dt.Rows.Count > 0 Then
            str = Dt.Rows(0).Item("Impostazione_Valore_1")
        End If

        If str = "" OrElse str = "0" Then
            Return True
        End If



        'verifico se patentino permette
        Dt = New AgronicaCoreAnagrafeDAL.Contatti_R().Contatti_Con_Patentino(PivaAzienda, Data_Validita, objParametri_Server)
        If Dt.Rows.Count > 0 Then
            Return True
        End If

        Dim ClassiTox As Hashtable = FormulatoRecuperaClassiTox(Fr_Cod, objParametri_Server, Session)
        If ClassiTox.Count = 0 Then
            Return True
        End If

        'T	Tossico 
        'T+	Molto Tossico
        'Xn	Nocivo
        For Each classe As DictionaryEntry In ClassiTox

            Select Case classe.Key

                Case "T", "T+", "Xn"
                    msg = classe.Value


                    If str = "2" Then
                        'blocco
                        Return False
                    End If


                    If str = "1" Then
                        'avviso
                        'non funziona con agrosino!!!!
                        '' ''controllo se ho acconsentito precedentemente
                        ' ''If UdmPatentinoTox_SI_NO.Value = "0" Then

                        ' ''    'se non ho acconsentito genere agrosino che mi rilancerà il salvataggio via jscript
                        ' ''    Dim messaggio_errore As String = "Attenzione, . Procedere ugualmente?"
                        ' ''    'AgroSiNo

                        ' ''    'impedisco di procedere
                        ' ''    Messaggi.AgroSiNo(messaggio_errore, "UdmPatentinoTox", Page, , update_si_no)


                        ' ''    Return True

                        ' ''Else
                        ' ''    'se ho già cliccato  ok vado avanti
                        ' ''    UdmPatentinoTox_SI_NO.Value = "0"
                        ' ''    Return True
                        ' ''End If

                        ''''ASPX

                        '' ''                                function DoPostBack_ControlliSiNo(key) {
                        '' ''            alert(key);
                        '' ''                if (key == 'UdmMovimenti') {
                        '' ''                    $("#<%=UdmMovimenti_SI_NO.ClientID %>").val("OK");
                        '' ''                    $("#<%=ImgBtn_Inserisci_nel_DataGrid.ClientID %>").click();
                        '' ''                }

                        '' ''                if (key == 'UdmPatentinoTox') {
                        '' ''                    alert('b');
                        '' ''                     alert($("#<%=UdmPatentinoTox_SI_NO.ClientID %>").val());
                        '' ''//                       if ($("#<%=UdmPatentinoTox_SI_NO.ClientID %>").val()="OK"){
                        '' ''//                        alert('a');
                        '' ''//                       }

                        '' ''//                    $("#<%=UdmPatentinoTox_SI_NO.ClientID %>").val("OK");
                        '' ''//                    $("#<%=ImgBtn_Inserisci_nel_DataGrid.ClientID %>").click();
                        '' ''                }

                        '' ''            }      



                        '' ''  <ContentTemplate>
                        '' ''            <asp:HiddenField ID="UdmMovimenti_SI_NO" runat="server" Value="0" />
                        '' ''             <asp:HiddenField ID="UdmPatentinoTox_SI_NO" runat="server" Value="0" />
                        '' ''        </ContentTemplate>


                        Messaggi.AgroMsgBox(
                            String.Format(AgronicaAgenda_2010.RichiestoContattoConPatentinoValidoOperazioneNonBloccata, msg),
                            Page, , Me.upDati
                        )
                        Return True


                    End If

                    Return False

            End Select

        Next classe

        Return True

    End Function


    Protected Sub Btn_CaricaInneschi_Click(sender As Object, e As EventArgs) Handles Btn_CaricaInneschi.Click

        'se l'utente vuole caricare gli inneschi
        'carico quelli relativi alla trappola che ha inserito
        cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(cmb_Categoria.Items.FindByValue(INNESCHI))
        cmb_Categoria_SelectedIndexChanged(Nothing, Nothing)

        If Trap_Cod.Value IsNot Nothing AndAlso IsNumeric(Trap_Cod.Value) Then
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            'Carico tutte le avversità che sono combattute dalla trappola scelta per quella specie vegetale
            clc.AvversitaxTrappole(cmb_Prodotti,
                                                                    True, "", "0",
                                                                    0,
                                                                    CInt(Trap_Cod.Value),
                                                                    0,
                                                                    "", "", objParametri_Server)
        End If

    End Sub

    Private Sub Btn_SommaProdotto_Click(sender As Object, e As System.EventArgs) Handles Btn_SommaProdotto.Click
        Dim Qta As Double
        Dim indice As Integer

        If Not IsNothing(ViewState("rigaProdottoInserito")) AndAlso IsNumeric(ViewState("rigaProdottoInserito")) Then
            indice = CInt(ViewState("rigaProdottoInserito"))

            '-----
            'QUANTITA'
            If Not IsNothing(ViewState("qtaProdottoDaAggiornare")) Then
                If Not IsNumeric(ViewState("qtaProdottoDaAggiornare")) Then
                    Return
                    'Str_Errore &= DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerLaQuantità"), String) & vbCrLf
                Else
                    If ViewState("qtaProdottoDaAggiornare") <= 0 Then
                        Return
                        'Str_Errore &= "Non e' possibile inserire una quantità nulla o negativa!" & vbCrLf
                    Else
                        If InStr(ViewState("qtaProdottoDaAggiornare"), ".") <> 0 Then
                            ViewState("qtaProdottoDaAggiornare") = Replace(ViewState("qtaProdottoDaAggiornare"), ".", ",")
                        End If
                        Qta = CDbl(ViewState("qtaProdottoDaAggiornare"))

                        SommaQtaProdottoInGriglia(indice, Qta)
                    End If
                End If
            Else
                Return
                'Str_Errore &= DirectCast(GetLocalResourceObject("InserireUnaQuantità"), String) & vbCrLf
            End If

        End If
    End Sub

    Private Sub SommaQtaProdottoInGriglia(IndiceRiga As Integer, Qta As Double)

        Dim Dt As DataTable
        'Recupero il datatable
        Dt = ViewState("DT_Prodotti_NO_Contabili")
        Dt.Rows(IndiceRiga).Item("Qta") = Qta + CDbl(Dt.Rows(IndiceRiga).Item("Qta"))

        '----- Associo il DataTable con la DataGrid

        Me.DataGrid_Prodotti.DataSource = Dt
        Me.DataGrid_Prodotti.DataBind()

        ViewState("DT_Prodotti_NO_Contabili") = Dt
    End Sub

    Protected Sub ImgBtn_NuovoProdotto_Click(sender As Object, e As ImageClickEventArgs) Handles ImgBtn_NuovoProdotto.Click
        LinkNuovoProdotto()
    End Sub
    Private Sub LinkNuovoProdotto()

        Dim objParametriAgenda2 As New ParametriAgenda
        Dim TargetUrl As String

        'Apertura vecchia Anagrafica Prodotti
        'objParametriAgenda2.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        'objParametriAgenda2.PaginaSitoOrigine = enum_PagineAgenda_2010.FormProdotto
        'objParametriAgenda2.StrGenericaXlinkGiasOnline = Me.TxtProdotto.Text
        'TargetUrl = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.GestioneRisorse_Edit, objParametriAgenda2)

        'Dim Str As New StringBuilder
        'Str.AppendLine("$(document).ready(function () {")
        'Str.AppendLine("  window.open('" & TargetUrl & "' , ''); ")
        'Str.AppendLine("});")

        'ScriptManager.RegisterClientScriptBlock(upDati, upDati.GetType(),
        '           String.Format("jQuery_{0}", upDati.ClientID), Str.ToString, True)

        If Not IsNumeric(Me.cmb_Categoria.SelectedValue) OrElse Me.cmb_Categoria.SelectedValue = 0 Then
            Exit Sub
        End If

        Dim elem_cod As Integer = Me.cmb_Categoria.SelectedValue

        TargetUrl = MenuBS_Anagrafica.NuovoProdotto(elem_cod) & "&win=1"

        Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
                TargetUrl,
                "",
                "",
                1250, 800, 0, 0,
                NomeForm:="aspnetForm")


        ScriptManager.RegisterStartupScript(upDati, upDati.GetType(),
                                                String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, True)
    End Sub

    Protected Sub ImgBtn_CaricaFornitori_Click(sender As Object, e As ImageClickEventArgs) Handles ImgBtn_CaricaFornitori.Click
        'evento finto, serve per far scattare il postback della pagina, in cui è gestito il ricaricamento dei contatti e la selezione di quello appena inserito
        Dim debug As String = Me.InsFornitore.Value
    End Sub

    Protected Sub ImgBtn_NuovoFornitore_Click(sender As Object, e As ImageClickEventArgs) Handles ImgBtn_NuovoFornitore.Click
        LinkNuovoFornitore()
    End Sub

    Private Sub LinkNuovoFornitore()

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Piva = xPiva
        objParametriAgenda.RagSoc = ""
        objParametriAgenda.Lav_Cod = Qs_Lav_Cod


        Dim QueryString As String
        QueryString = "?o=" &
                      Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                        "&piva=" &
                      Stringa_Codifica(xPiva, AgroKey_EncoderDecoder, Server) &
                       "&tipo_rapporto=" &
                      Stringa_Codifica(COD_FORNITORE, AgroKey_EncoderDecoder, Server) &
                        "&lav_cod=" &
                      Stringa_Codifica(Qs_Lav_Cod, AgroKey_EncoderDecoder, Server) &
                        "&orig=" &
                      Stringa_Codifica(enum_PagineAgenda_2010.FormProdotto, AgroKey_EncoderDecoder, Server) &
                        "&codcont=" &
                      Stringa_Codifica("", AgroKey_EncoderDecoder, Server)


        Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
                "../Anagrafica/New_Contatto_Edit.aspx",
                QueryString,
                InsFornitore.ClientID,
                1250, 800, 0, 0,
                NomeForm:="aspnetForm")


        'Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
        '        "../GestioneContatti/Contatto.aspx",
        '        QueryString,
        '        InsFornitore.ClientID,
        '        1000, 800, 0, 0,
        '        NomeForm:="aspnetForm")

        StrWindowOpen &= "$('.myCombo').combobox3();"

        If upDati Is Nothing Then
            Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(StrWindowOpen))
        Else
            ScriptManager.RegisterStartupScript(upDati, upDati.GetType(),
                                                String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, True)

        End If

    End Sub

    '########################################################################################
    Private Function Genera_Agenda_DocumentoLight(ByVal Operazione As enum_TipoOperazioneDB,
                                                  ByVal Num_Doc As Double,
                                                  ByVal Cod_RisUm As Integer,
                                                  ByVal Contatto_Desc As String,
                                                  ByVal DT As DataTable) As Operazione_Agenda


        Dim Agenda As New Operazione_Agenda
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim objContIndirizzi As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita

        ' Dim i As Integer
        Dim BaseCode As Integer
        Dim TopCode As Integer
        Dim Des_Lib As String = ""
        Dim Mov_Desc_Magazzino As String = ""
        Dim Mov_Desc_Contabile As String = ""
        Dim Mov_Det_Des As String
        Dim Lav_Cod As Integer
        Dim Cod_IndirizzoRisUm As Integer

        'Dim array_temp As String()
        Dim Sa_Cod As Integer
        Dim Magazzino_Cod As Integer

        Dim Cau_Mov As String = ""
        Dim Elem_Cod As Integer
        Dim Pro_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Cod_Progetto As Integer
        Dim Fase_Cod As Integer
        Dim Lotto As String
        Dim Cal_Cod As Integer
        Dim Udm_Cod As Integer
        Dim Extra_int As Integer
        Dim Extra_int_contabile As Integer

        'Dim Gen_Cod As Integer
        'Dim Spe_Cod As Integer
        'Dim Ipro_Cod As Integer
        'Dim Raz_Cod As Integer
        'Dim Cat_Cod As Integer
        'Dim Spe_Des As String
        'Dim Ipro_Des As String
        'Dim Raz_Des As String

        Dim Qta As Decimal
        Dim PrezzoUnitario, PrezzoUnitario_Netto As Decimal

        Dim Udm_Cod_Extra As Decimal = 0
        Dim Qta_Extra As Decimal = 0
        Dim Prezzo_Effettivo As Decimal = 0
        Dim Sconto_Magg As Decimal = 0
        Dim Variazione_Perc As String = ""
        Dim Variazione As Decimal = 0
        Dim Imponibile As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Cod_Iva As Integer = 0
        Dim Iva As Decimal = 0
        Dim Anno As Integer = 0
        Dim Ric_Cod As Integer = 0
        Dim Cod_Conto As Integer = 0
        Dim Conto As String = ""

        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Cu As Decimal = 0
        Dim Regolamento_Cod_Ferti As Integer = 0

        Dim Matricola As String = ""
        Dim Nome As String = ""
        Dim Collare As String = ""
        Dim Nome_Aia As String = ""
        Dim Matricola_Aia As String = ""
        Dim Dat_Nascita As Date = AGRODATAINIZIO
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE
        Dim Stato_Nascita As String = "000"
        Dim Provincia_Nascita As String = "000"
        Dim AUA_Azi_Nascita As String = ""
        Dim AUSL_Azi_Nascita As String = ""
        Dim Sesso As String = "M"
        Dim Mat_Padre As String = ""
        Dim Mat_Madre As String = ""
        Dim CF_Proprietario As String = ""
        Dim CF_Detentore As String = ""
        Dim Presente As String = ""
        Dim Peso As Decimal = 0
        Dim Data_Pesa As Date = AGRODATAINIZIO
        'Dim Metodo_Produzione As Integer
        Dim Conversione_Inizio As Date = AGRODATAINIZIO
        Dim Conversione_Fine As Date = AGRODATAINIZIO


        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS").ToString)


        '#######################################################
        '##################   AGENDA   #########################
        '#######################################################

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = Qs_IdAgenda
        Agenda.Data = Me.Txt_DataMovimento.Text
        Agenda.Piva = ViewState("Piva")
        Agenda.Sa_Cod = Sa_Cod
        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        'setto le variabili per differenziare il caso del carico e dello scarico.
        Select Case xCaricoScarico

            Case enum_Agenda_Causali.CARICO

                '-----------------------------------------------------------------
                '------------------------- CARICO --------------------------------
                '-----------------------------------------------------------------

                Cau_Mov = CAU_CARICO

                Select Case Qs_Lav_Cod

                    Case LAVCOD_BOLLA_RICEVUTA
                        Lav_Cod = LAVCOD_BOLLA_RICEVUTA
                        Des_Lib = String.Format(DirectCast(GetLocalResourceObject("DDTRicevutoNumeroERiferimentoContatto"), String), CStr(Num_Doc), Contatto_Desc)
                        'Mov_Desc_Contabile = "DDT ricevuto rif. " & Contatto_Desc
                        Mov_Desc_Magazzino = DirectCast(GetLocalResourceObject("CaricoArticoliAMagazzinoRelativiAlDDTRicevutoNumero_"), String) & CStr(Num_Doc)
                        Extra_int_contabile = 0

                    Case LAVCOD_FATTURA_RICEVUTA
                        Lav_Cod = LAVCOD_FATTURA_RICEVUTA
                        Des_Lib = String.Format(DirectCast(GetLocalResourceObject("FatturaRicevutaNumeroERiferimentoContatto"), String), CStr(Num_Doc), Contatto_Desc)
                        'Mov_Desc_Contabile = "Fattura ricevuta rif. " & Contatto_Desc
                        Mov_Desc_Magazzino = DirectCast(GetLocalResourceObject("CaricoArticoliAMagazzinoRelativiAllaFatturaRicevutaNumero_"), String) & CStr(Num_Doc)
                        Extra_int_contabile = enum_FatturaTipo.Immediata

                End Select

            Case enum_Agenda_Causali.SCARICO

                ''-----------------------------------------------------------------
                ''------------------------- SCARICO -------------------------------
                ''-----------------------------------------------------------------

                'Cau_Mov = CAU_SCARICO
                ''modifica 12 06 2014, mostro ne note della form prodotto
                ''Mov_Desc = DirectCast(GetLocalResourceObject("ScaricoDiMagazzino"), String) & " (" & Dr.Item("Prodotto") & ")"

                'Select Case Qs_Lav_Cod

                '    Case LAVCOD_VENDITA
                '        Lav_Cod = LAVCOD_VENDITA
                '        Des_Lib = DirectCast(GetLocalResourceObject("VenditaDi_"), String)
                '        Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("VenditaDi_"), String)

                '    Case LAVCOD_FATTURA_EMESSA
                '        Lav_Cod = LAVCOD_FATTURA_EMESSA
                '        Des_Lib = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String)
                '        Mov_Desc_Contabile = DirectCast(GetLocalResourceObject("FatturaEmessaDi_"), String)

                '    Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                '        Lav_Cod = LAVCOD_NOTA_ACCREDITO_RICEVUTA
                '        Des_Lib = DirectCast(GetLocalResourceObject("NotaDiAccreditoRicevutaDi_"), String)
                '        Mov_Desc_Contabile = "Nota di ricevuta emessa di "

                '    Case Else
                '        Lav_Cod = LAVCOD_SCARICO
                '        Des_Lib = "Scarico di magazzino "
                '        Mov_Desc_Contabile = ""

                'End Select

        End Select

        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Des_Lib

        Agenda.Movimenti = New List(Of Movimento)

        '#######################################################
        '################   MOVIMENTO CONTABILE    #############
        '#######################################################
        'i18n
        Movimento = New Movimento

        Movimento.Id_Agenda = Qs_IdAgenda
        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode
        Movimento.Piva = ViewState("Piva")
        Movimento.Sa_Cod = Sa_Cod
        Movimento.Data = Me.Txt_DataMovimento.Text
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_REGISTRAZIONI
        Movimento.Cod_Risum = Cod_RisUm
        Movimento.Doc_Numero = Num_Doc
        Movimento.Mov_Desc = ""
        Movimento.Ora = Me.Txt_Ora.Text
        Movimento.Data_Registrazione = Me.Txt_DataMovimento.Text
        Movimento.Scadenza = Me.Txt_DataMovimento.Text
        Movimento.Colli = 1
        Movimento.Causale_Trasporto = "CONTO ACQUISTO"
        Movimento.Aspetto = "VISIBILE"
        Movimento.Extra_Int = Extra_int_contabile

        Cod_IndirizzoRisUm = objContIndirizzi.Ricava_CodIndirizzoValorizzato_from_CodRisUm(Cod_RisUm, objParametri_Server)
        Movimento.Cod_IndirizzoRisUm = Cod_IndirizzoRisUm

        Agenda.Movimenti.Add(Movimento)

        '#######################################################
        '###########   MOVIMENTO DI CARICO / SCARICO    ########
        '#######################################################

        Movimento = New Movimento

        Movimento.Id_Agenda = Qs_IdAgenda
        Movimento.Piva = ViewState("Piva")
        Movimento.Sa_Cod = Sa_Cod
        Movimento.Data = Me.Txt_DataMovimento.Text
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = Cau_Mov
        Movimento.Mov_Desc = Mov_Desc_Magazzino
        Movimento.Ora = Me.Txt_Ora.Text
        Movimento.Data_Registrazione = Me.Txt_DataMovimento.Text
        Movimento.Scadenza = AGRODATAFINE
        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)


        Select Case Qs_Tipo


            '####################################
            '############# STALLA ###############
            '####################################

            Case CAU_ANIMALE

                'Elem_Cod = ZOO_CONSISTENZA
                'Pro_Cod = 0
                'Mat_Cod = 0
                'Lotto = ""
                'Cal_Cod = 0
                'Udm_Cod = 38

                ''setto le variabili per differenziare il caso del carico e dello scarico.
                'Select Case xCaricoScarico


                '    Case enum_Agenda_Causali.CARICO

                '        '-----------------------------------------------------------------
                '        '------------------------- CARICO --------------------------------
                '        '-----------------------------------------------------------------

                '        array_temp = Me.Cmb_Destinazione.SelectedItem.Value.Split("|")
                '        Magazzino_Cod = array_temp(0)
                '        Sa_Cod = array_temp(1)

                '        Cau_Mov = CAU_CARICO
                '        Lav_Cod = 3001

                '        Des_Lib = "Aumento Consistenze Zootecniche"
                '        'Mov_Desc = "Aumento Consistenze Zootecniche"


                '    Case enum_Agenda_Causali.SCARICO

                '        '-----------------------------------------------------------------
                '        '------------------------- SCARICO -------------------------------
                '        '-----------------------------------------------------------------


                '        array_temp = Me.Cmb_Provenienza.SelectedItem.Value.Split("|")
                '        Magazzino_Cod = array_temp(0)
                '        Sa_Cod = array_temp(1)

                '        Cau_Mov = CAU_SCARICO
                '        Lav_Cod = 3002

                '        Des_Lib = "Decremento Consistenze Zootecniche"
                '        'Mov_Desc = "Decremento Consistenze Zootecniche"

                'End Select


                'If Not Dr Is Nothing Then

                '    '-----------------------------------------------------------------
                '    '------------------------- SCRITTURA -----------------------------
                '    '-----------------------------------------------------------------

                '    Cod_Progetto = Dr.Item("Cod_Progetto")
                '    'Fase_Cod = Dr.Item("Fase_Cod")

                '    Gen_Cod = Dr.Item("Gen_Cod")
                '    Spe_Cod = Dr.Item("Spe_Cod")
                '    Ipro_Cod = Dr.Item("Ipro_Cod")
                '    Raz_Cod = Dr.Item("Raz_Cod")
                '    Cat_Cod = Dr.Item("Cat_Cod")
                '    Spe_Des = Dr.Item("Spe_Des")
                '    Ipro_Des = Dr.Item("Ipro_Des")
                '    Raz_Des = Dr.Item("Raz_Des")
                '    Matricola = Dr.Item("Matricola")
                '    Nome = Dr.Item("Nome")
                '    Collare = Dr.Item("Collare")
                '    Nome_Aia = Dr.Item("Nome_Aia")
                '    Matricola_Aia = Dr.Item("Matricola_Aia")
                '    Dat_Nascita = Dr.Item("Dat_Nascita")
                '    Stato_Nascita = Dr.Item("Stato_Nascita")
                '    Provincia_Nascita = Dr.Item("Prov_Nascita")
                '    AUA_Azi_Nascita = Dr.Item("AUA_Azi_Nascita")
                '    AUSL_Azi_Nascita = Dr.Item("AUSL_Azi_Nascita")

                '    Mov_Det_Des = Ipro_Des & " - " & Raz_Des & " - " & Matricola & ": " & Nome

                '    Sesso = Dr.Item("Sesso")
                '    Mat_Padre = Dr.Item("Mat_Padre")
                '    Mat_Madre = Dr.Item("Mat_Madre")
                '    Peso = Dr.Item("Peso")
                '    Data_Pesa = Dr.Item("Data_Pesa")

                '    Metodo_Produzione = Dr.Item("Metodo_Produzione")
                '    Conversione_Inizio = Dr.Item("Conversione_Inizio")
                '    Conversione_Fine = Dr.Item("Conversione_Fine")

                '    Qta = Dr.Item("Qta_Reale")
                '    PrezzoUnitario = Dr.Item("Prezzo_Reale")
                '    PrezzoUnitario_Netto = PrezzoUnitario


                'Else

                '    '-----------------------------------------------------------------
                '    '------------------------- MODIFICA -----------------------------
                '    '-----------------------------------------------------------------



                'End If


                '######################################################
                '#################### MAGAZZINO #######################
                '######################################################

            Case Else

                Dim dr As DataRow
                For Each dr In DT.Rows

                    If dr IsNot Nothing Then

                        '-----------------------------------------------------------------
                        '------------------------- SCRITTURA -----------------------------
                        '-----------------------------------------------------------------

                        Elem_Cod = dr.Item("Elem_Cod")
                        Pro_Cod = dr.Item("Pro_Cod")
                        Mat_Cod = dr.Item("Mat_Cod")
                        Cod_Progetto = dr.Item("Cod_Progetto")
                        Fase_Cod = dr.Item("Fase_Cod")
                        Lotto = dr.Item("Lotto")
                        Cal_Cod = dr.Item("Cal_Cod")

                        Udm_Cod = dr.Item("Udm_Cod")
                        Qta = dr.Item("Qta")
                        Extra_int = 0

                        If dr.Item("Extra_Int") <> "0" Then

                            Extra_int = Udm_Cod

                            Select Case Udm_Cod
                                Case enum_UnitaMisura.Quintali
                                    Qta = Qta * 100
                                    Udm_Cod = enum_UnitaMisura.KG
                                Case enum_UnitaMisura.Tonnellate
                                    Qta = Qta * 1000
                                    Udm_Cod = enum_UnitaMisura.KG
                                Case enum_UnitaMisura.Metri_Cubi
                                    Qta = Qta * 1000
                                    Udm_Cod = enum_UnitaMisura.Litri
                            End Select

                        End If

                        Mov_Det_Des = dr.Item("Prodotto")
                        PrezzoUnitario = dr.Item("Prezzo_Unitario")
                        PrezzoUnitario_Netto = dr.Item("Prezzo_Unitario_Netto")
                        Udm_Cod_Extra = dr.Item("Udm_Cod_Extra")
                        Qta_Extra = dr.Item("Qta_Extra")

                        Sconto_Magg = dr.Item("Cod_Variazione")
                        Cod_Iva = dr.Item("Cod_Iva")
                        Iva = objContabHLP.Leggi_IVA_PositivaNegativa(Qs_Lav_Cod, dr.Item("Iva"))
                        Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, dr.Item("Imponibile"))
                        Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, dr.Item("Imponibile_Netto"))

                        Prezzo_Effettivo = dr.Item("Prezzo_Effettivo")
                        Anno = dr.Item("Anno")
                        Ric_Cod = dr.Item("Ric_Cod")
                        Cod_Conto = dr.Item("Cod_Conto")
                        Conto = dr.Item("Conto")

                        N = dr.Item("N")
                        P2O5 = dr.Item("P2O5")
                        K2O = dr.Item("K2O")
                        Cu = dr.Item("Cu")
                        Regolamento_Cod_Ferti = dr.Item("Regolamento_Cod_Ferti")

                        'setto le variabili per differenziare il caso del carico e dello scarico.
                        Select Case xCaricoScarico

                            Case enum_Agenda_Causali.CARICO

                                '-----------------------------------------------------------------
                                '------------------------- CARICO --------------------------------
                                '-----------------------------------------------------------------
                                Magazzino_Cod = dr.Item("Id_Destinazione")
                                Sa_Cod = dr.Item("SaCod_Destinazione")
                                Cau_Mov = CAU_CARICO

                            Case enum_Agenda_Causali.SCARICO

                                '-----------------------------------------------------------------
                                '------------------------- SCARICO -------------------------------
                                '-----------------------------------------------------------------
                                Magazzino_Cod = dr.Item("Id_Provenienza")
                                Sa_Cod = dr.Item("SaCod_Provenienza")
                                Cau_Mov = CAU_SCARICO

                        End Select

                        Dim Pendente As Integer
                        Dim Descrizione As String = ""
                        Dim ID_Destinazione As Integer = 0
                        Pendente = GetPendente(Elem_Cod, Descrizione, Pro_Cod, Mat_Cod, Fase_Cod, Cal_Cod, Udm_Cod, ID_Destinazione)


                        Dim Extra_Str As String = ""
                        Dim Extra_Date As Date = AGRODATAINIZIO

                        If cmb_Causale.SelectedValue = CStr(enum_Pendenza.Furto) Then
                            'se sono in causale furto allora abilito le textbox per scrivere la denuncia
                            Extra_Str = Txt_NumeroDenuncia.Text
                            If IsDate(Txt_DataDenuncia.Text) Then
                                Extra_Date = CDate(Txt_DataDenuncia.Text)
                            End If
                        End If

                        '#######################################################
                        '################   MOVIMENTI DETTAGLIO    #############
                        '#######################################################

                        Movimento_Dettaglio = New Movimento_Dettaglio

                        Movimento_Dettaglio.Id_Agenda = Qs_IdAgenda
                        Movimento_Dettaglio.Piva = ViewState("Piva")
                        Movimento_Dettaglio.Sa_Cod = Sa_Cod
                        Movimento_Dettaglio.Data = Me.Txt_DataMovimento.Text
                        Movimento_Dettaglio.Lav_Cod = Lav_Cod
                        Movimento_Dettaglio.Cau_Mov = xCaricoScarico.ToString
                        Movimento_Dettaglio.Mov_Det_Des = Mov_Det_Des
                        Movimento_Dettaglio.Elem_Cod = Elem_Cod
                        Movimento_Dettaglio.Pro_Cod = Pro_Cod
                        Movimento_Dettaglio.Mat_Cod = Mat_Cod
                        Movimento_Dettaglio.Cod_Progetto = Cod_Progetto
                        Movimento_Dettaglio.Fase_Cod = Fase_Cod
                        Movimento_Dettaglio.Lotto = Lotto
                        Movimento_Dettaglio.Cal_Cod = Cal_Cod
                        Movimento_Dettaglio.Udm_Cod = Udm_Cod
                        Movimento_Dettaglio.Udm_Cod_Extra = Udm_Cod_Extra
                        Movimento_Dettaglio.Qta = Qta
                        Movimento_Dettaglio.Qta_Extra = Qta_Extra
                        Movimento_Dettaglio.Pendente = Pendente

                        Movimento_Dettaglio.Prezzo_Effettivo = Prezzo_Effettivo
                        Movimento_Dettaglio.Prezzo_Unitario = PrezzoUnitario
                        Movimento_Dettaglio.Prezzo_Unitario_Netto = PrezzoUnitario_Netto

                        Movimento_Dettaglio.Sconto = Sconto_Magg
                        Movimento_Dettaglio.Imponibile = Imponibile
                        Movimento_Dettaglio.Imponibile_Netto = Imponibile_Netto

                        Movimento_Dettaglio.Cod_Iva = Cod_Iva
                        Movimento_Dettaglio.Iva = Iva

                        Movimento_Dettaglio.Anno = Anno
                        Movimento_Dettaglio.Ric_Cod = Ric_Cod
                        Movimento_Dettaglio.Cod_Conto = Cod_Conto
                        Movimento_Dettaglio.Extra_Int = Extra_int
                        Movimento_Dettaglio.Extra_Str = Extra_Str
                        Movimento_Dettaglio.Extra_Date = Extra_Date

                        If Qs_Lav_Cod = LAVCOD_ACQUISTO OrElse Qs_Lav_Cod = LAVCOD_VENDITA Then
                            Movimento_Dettaglio.Contabilizzato = CONTABILE
                        Else
                            Movimento_Dettaglio.Contabilizzato = NONCONTABILE
                        End If

                        Movimento_Dettaglio.BaseCode = BaseCode
                        Movimento_Dettaglio.TopCode = TopCode

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)


                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                        '#######################################################
                        '################   MOVIMENTI DESTINAZIONE    ##########
                        '#######################################################

                        Movimento_Destinazione = New Movimento_Destinazione

                        Movimento_Destinazione.Id_Agenda = Qs_IdAgenda
                        Movimento_Destinazione.Piva = ViewState("Piva")
                        Movimento_Destinazione.Sa_Cod = Sa_Cod
                        Movimento_Destinazione.Appezza = 0
                        Movimento_Destinazione.Id_Destinazione = Magazzino_Cod
                        Movimento_Destinazione.Tipo = MAGAZZINO
                        Movimento_Destinazione.Qta = Qta
                        Movimento_Destinazione.Data = Me.Txt_DataMovimento.Text
                        Movimento_Destinazione.BaseCode = BaseCode
                        Movimento_Destinazione.TopCode = TopCode

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni.Add(Movimento_Destinazione)

                        '#######################################################
                        '################   MOVIMENTO TECNICO    ###############
                        '#######################################################

                        If Cau_Mov = CAU_CARICO AndAlso Elem_Cod = FERTILIZZANTI Then

                            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                            Movimento_Dettaglio_Tecnico.Id_Agenda = Qs_IdAgenda
                            Movimento_Dettaglio_Tecnico.Piva = ViewState("Piva")
                            Movimento_Dettaglio_Tecnico.Sa_Cod = Sa_Cod
                            Movimento_Dettaglio_Tecnico.N = N
                            Movimento_Dettaglio_Tecnico.P = P2O5
                            Movimento_Dettaglio_Tecnico.K = K2O
                            Movimento_Dettaglio_Tecnico.Cu = Cu
                            Movimento_Dettaglio_Tecnico.Extra_Int = Regolamento_Cod_Ferti
                            Movimento_Dettaglio_Tecnico.Data = Me.Txt_DataMovimento.Text
                            Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
                            Movimento_Dettaglio_Tecnico.TopCode = TopCode

                            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                        End If

                    Else

                        '-----------------------------------------------------------------
                        '------------------------- MODIFICA -----------------------------
                        '-----------------------------------------------------------------

                        'Per il momento non esiste la modifica del documento in versione light

                    End If

                Next

        End Select

        Return Agenda

    End Function

    Private Sub CaricaCombo_Regolamenti()

        ' Giulia: 4/6/2019: Leggo da WS

        Try

            'Dim FiltroTipo As String = " ( Tipo=2 OR Tipo=3 )"
            Dim FiltroTipo As String = " ( Tipo=2 )"

            ListControl_PianoConcimazione_WS.PUA_Regolamento_WS_xAgenda(cmb_PUARegolamenti,
                                                                        True,
                                                                        AgronicaAgenda_2010.Nessuno,
                                                                        "0" & SEP_PuaReg & "0",
                                                                        False,
                                                                        0,
                                                                        CDate(Me.Txt_DataMovimento.Text),
                                                                        CDate(Me.Txt_DataMovimento.Text),
                                                                        FiltroTipo,
                                                                        " Ordine desc ")

        Catch ex As Exception
            'Aggiungo almeno la voce nulla se c'è stato qualche errore
            cmb_PUARegolamenti.Items.Add(New ListItem(AgronicaAgenda_2010.Nessuno, "0" & SEP_PuaReg & "0"))
        End Try

        If Qs_Pua_Regolamento_Cod <> "" AndAlso Not IsNothing(cmb_PUARegolamenti.Items.FindByValue(Qs_Pua_Regolamento_Cod)) Then
            cmb_PUARegolamenti.SelectedValue = Qs_Pua_Regolamento_Cod
        End If

        '28/06/2018: aggiunto tipo al cod regolamento
        'AgronicaCoreUtility.CaricaListControl.PUA_Regolamento(cmb_PUARegolamenti, True, AgronicaAgenda_2010.Nessuno, "0" & SEP_PuaReg & "0", "Tipo=2 OR Tipo=3", "", objParametri_Server, True)

        '28/06/2018: aggiunta regolamento bio
        'Lascio l'aggiunta da fuori, perché dentro non inserisce la tipologia
        'i18n
        cmb_PUARegolamenti.Items.Add(New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio & SEP_PuaReg & "0"))
    End Sub

    Private Sub Txt_DataMovimento_TextChanged(sender As Object, e As EventArgs) Handles Txt_DataMovimento.TextChanged

        'devo riazzerare la combo regolamenti e ricrearla
        '(solo se ho già scelto i fertilizzanti, sennò è inutile fare la chiamata per popolarla,
        'tanto verrà fatta al cambio della categoria)
        cmb_PUARegolamenti.Items.Clear()

        If IsNumeric(Me.cmb_Categoria.SelectedValue) AndAlso Me.cmb_Categoria.SelectedValue = FERTILIZZANTI Then
            CaricaCombo_Regolamenti()
        End If
    End Sub

End Class
