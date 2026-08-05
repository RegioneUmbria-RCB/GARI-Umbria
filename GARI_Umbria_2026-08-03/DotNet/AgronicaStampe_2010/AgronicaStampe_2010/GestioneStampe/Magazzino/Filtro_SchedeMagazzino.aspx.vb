Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010

Partial Class Filtro_SchedeMagazzino
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
    'Dim Qs_Destinazione As String
    'Dim Qs_Chiave As String

    Const PaginaLinkStampaSchedaGiacenzeMagazzino As String = "Giacenze/SchedaGiacenzeMagazzino_2.aspx"
    Const PaginaLinkStampaSchedaMovimentiMagazzino As String = "Movimenti/SchedaMovimentiMagazzino.aspx"
    Const PaginaLinkStampaSchedaFertilizzantiMagazzino As String = "Fertilizzanti/SchedaFertilizzantiMagazzino.aspx"
    Const PaginaLinkStampaSchedaProdottiFitosanitariMagazzino As String = "ProdottiFitosanitari/SchedaProdottiFitosanitariMagazzino.aspx"

    Const PaginaLinkStampaSchedaMovimentiMagazzinoExcel As String = "Movimenti/SchedaMovimentiMagazzinoExcel.aspx"

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If
        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '=====================================================
        '----- Recupero i valori dalla querystring
        '=====================================================

        QS_DataStampa = Stringa_Decodifica(Request.QueryString("ds").ToString, _
                              AgroKey_EncoderDecoder, _
                              Server)

        QS_DataInizio = Stringa_Decodifica(Request.QueryString("di").ToString, _
                                AgroKey_EncoderDecoder, _
                                Server)

        QS_DataFine = Stringa_Decodifica(Request.QueryString("df").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        'Qs_Origine = Stringa_Decodifica(Request.QueryString("o").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Destinazione = Stringa_Decodifica(Request.QueryString("d").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Chiave = Stringa_Decodifica(Request.QueryString("k").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                            AgroKey_EncoderDecoder, _
                            Server)

        'TODO: verificare la funzione in agronicacorexml\xml_stampe\XML_EstraiVariabiliStampe (non viene letto il sa_Cod)
        If Qs_Sa_Cod = "" Then
            Qs_Sa_Cod = "0"
        End If

        Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("f").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)

        If Qs_Fabbricato_Cod = "" Then
            Qs_Fabbricato_Cod = "0"
        End If

        Qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("e").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)

        Qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)

        Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)



        '########################################################################
        '#####  Verifica se la pagina e' stata caricata per la prima volta  #####
        '########################################################################

        If Not Page.IsPostBack Then


            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            Dim UtenteAbilitato As Boolean
            Dim strDummy As String      'controllo accesso negato.....

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato = objUtenti.Controlla_Permessi_Utente( _
                Session("ASG_Utente_Username"), _
                Session("ASG_IdServizio"), _
                enum_Security_Attivita.Stampa_MovimentiMagazziniExcel, _
                enum_Security_Operazione.Lettura, _
                 Now(), _
                 "", _
                 objParametri_Utenti _
            )


            '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

            If UtenteAbilitato = False Then
                ImgBtn_StampaExcel.Visible = False
            Else
                ImgBtn_StampaExcel.Visible = True
            End If


            Dim UtenteAbilitatoBlocco As Boolean
            Dim strDummyBlocco As String      'controllo accesso negato.....

            UtenteAbilitatoBlocco = objUtenti.Controlla_Permessi_Utente( _
                Session("ASG_Utente_Username"), _
                Session("ASG_IdServizio"), _
                enum_Security_Attivita.Agenda_Operazioni_Blocco, _
                enum_Security_Operazione.Modifica, _
                 Now(), _
                 "", _
                 objParametri_Utenti _
            )


            '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

            If UtenteAbilitatoBlocco = False Then
                CB_BloccaOperazioni.Visible = False
            Else
                CB_BloccaOperazioni.Visible = True
            End If

            ''//////////////////////////////////////////////////////
            ''/// La pagina e' stata caricata per la prima volta ///
            ''//////////////////////////////////////////////////////

            ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            'Dim UtenteAbilitato As Boolean
            'Dim strDummy As String      'controllo accesso negato.....


            'UtenteAbilitato = Controlla_Permessi_Utente_2( _
            '            Server, Session, Page, Session("ASG_Utente_Username"), _
            '            Session("ASG_IdServizio"), _
            '             enum_Security_Attivita.Gest_Stampe, _
            '             enum_Security_Operazione.Lettura, _
            '            strDummy)


            ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

            'If UtenteAbilitato = False Then
            'Dim strClose As String = "<script language='javascript'>window.close()</script>"
            'Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
            'End If

        Else

            '///////////////////////////////////////
            '/// La pagina ha subito un Postback ///
            '///////////////////////////////////////

            'Non faccio nulla
            Exit Sub

        End If


        '=====================================================
        '----- Inizializzo i controlli
        '=====================================================


        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti, _
                  enum_CodificaStampe.SchedaMagazzinoFertilizzanti, _
                  enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                Me.TxtDataInizio.Text = QS_DataInizio
                Me.TxtDataFine.Text = QS_DataFine

                Me.TxtDataDa.Text = QS_DataInizio
                Me.TxtDataA.Text = QS_DataFine

            Case Else

                'Me.TxtStampa.Text = Now.Today
                Me.TxtStampa.Text = QS_DataStampa

        End Select

        '--------------------------------------------------------------------

        Me.Rbl_SchedaMagazzino.SelectedValue = Session("ReportSelezionato")

        '-------------------------------------------------------------------
        '----- Logo Regionale

        ' Per default viene stampato il logo della regione
        Chk_LogoRegione.Checked = True
        AgronicaCoreUtility.CaricaListControl.Regioni(Me.Cmb_Regioni, _
                                                        True, " ", "", _
                                                        "", _
                                                        "", "", _
                                                        objParametri_Server)

        If Qs_Piva <> "" Then

            'chiamo CaricaCombo_Imprese perchè devo caricare solo l'impresa che ha quella piva
            '(per caricare tutte le imprese uso CaricaCombo_ImpreseUtente_Optimize)
            AgronicaCoreUtility.CaricaListControl.Imprese(Me.Cmb_Impresa, _
                                                False, _
                                                "", "", _
                                                Qs_Piva, _
                                                "", "", _
                                                objParametri_Server)


            Me.Lbl_NumImprese.Text = CStr(Me.Cmb_Impresa.Items.Count)

            Cambia_Impresa()

        End If

        AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(Me.cmb_CatProdotto, _
                                                        True, "", "", _
                                                        0, _
                                                        CAU_SCARICO, _
                                                        0, _
                                                        False, _
                                                        False, _
                                                            "", _
                                                        "", _
                                                        objParametri_Server)

        Me.cmb_CatProdotto.SelectedIndex = Me.cmb_CatProdotto.Items.IndexOf(Me.cmb_CatProdotto.Items.FindByValue(CStr(Qs_Elem_Cod)))

        Dim Filtro_Cat As String
        Filtro_Cat = " (Tabella IN ('Materie_Prime','TipologieSementi') ) " + _
                        " AND  Elem_Cod NOT IN (" + CStr(FARMACI) + ", " + CStr(MANGIMI) + ")"

        AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(Me.ChkList_Categorie, _
                                                                    False, "", "", _
                                                                    0, _
                                                                    CAU_SCARICO, _
                                                                    0, _
                                                                    False, _
                                                                    False, _
                                                                    Filtro_Cat, "", _
                                                                    objParametri_Server)

        Dim i As Integer
        For i = 0 To Me.ChkList_Categorie.Items.Count - 1
            'seleziono tutto
            Me.ChkList_Categorie.Items(i).Selected = True
        Next


        '--------------------------------------------------------------------

        Cambia_Scheda()



    End Sub

    '###########################################################################
    Private Sub ImgBtnEsci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEsci.Click

        Dim strClose As String = "<script language='javascript'>window.close()</script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub


    '###########################################################################
    Private Sub ImgBtn_AnnataPrecedente_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_AnnataPrecedente.Click

        Me.TxtDataInizio.Text = DateAdd(DateInterval.Year, -1, CDate(Me.TxtDataInizio.Text))
        Me.TxtDataFine.Text = DateAdd(DateInterval.Year, -1, CDate(Me.TxtDataFine.Text))

    End Sub


    '###########################################################################
    Private Sub ImgBtn_AnnataSuccessiva_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_AnnataSuccessiva.Click

        Me.TxtDataInizio.Text = DateAdd(DateInterval.Year, 1, CDate(Me.TxtDataInizio.Text))
        Me.TxtDataFine.Text = DateAdd(DateInterval.Year, 1, CDate(Me.TxtDataFine.Text))

    End Sub


    '###########################################################################
    Private Sub GestioneCheckComposizione()

        'Verifico il tipo di stampa ...
        Select Case Me.Rbl_SchedaMagazzino.SelectedValue


            Case enum_CodificaStampe.SchedaMagazzinoGiacenze
                '---------- GIACENZE MAGAZZINO

                'Verifico la categoria prodotto ...
                If (cmb_CatProdotto.SelectedValue = "191") Then
                    'Per le categorie di prodotto valide ...
                    Me.Chk_Composizione.Visible = True
                    Me.Chk_Composizione.Checked = False
                Else
                    'Per le categorie di prodotto NON giuste ...
                    Me.Chk_Composizione.Visible = False
                    Me.Chk_Composizione.Checked = False
                End If


            Case enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari
                '---------- PRODOTTI FITOSANITARI

                Me.Chk_Composizione.Visible = True
                Me.Chk_Composizione.Checked = False


            Case Else
                '---------- ALTRIMENTI ...

                'Se il tipo di stampa NON e' giusta ...
                Me.Chk_Composizione.Visible = False
                Me.Chk_Composizione.Checked = False


        End Select







        ''Verifico il tipo di stampa ...
        'If (Me.Rbl_SchedaMagazzino.SelectedValue = enum_CodificaStampe.SchedaMagazzinoGiacenze) Then
        '    'Verifico la categoria prodotto ...
        '    If (cmb_CatProdotto.SelectedValue = "191") Then
        '        'Per le categorie di prodotto valide ...
        '        Me.Chk_Composizione.Visible = True
        '        'Me.Chk_Composizione.Checked = False
        '    Else
        '        'Per le categorie di prodotto NON giuste ...
        '        Me.Chk_Composizione.Visible = False
        '        'Me.Chk_Composizione.Checked = False
        '    End If
        'Else
        '    'Se il tipo di stampa NON e' giusta ...
        '    Me.Chk_Composizione.Visible = False
        '    'Me.Chk_Composizione.Checked = False
        'End If







    End Sub




    '########################################################################################
    Private Sub Rbl_SchedaMagazzino_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_SchedaMagazzino.SelectedIndexChanged
        Cambia_Scheda()
    End Sub

    '########################################################################################
    Private Sub Cambia_Scheda()

        Me.Lbl_Ordinamento.Visible = False
        Me.Rbl_Ordinamento.Visible = False
        Me.Lbl_StampaLotto.Visible = False
        Me.Rbl_StampaLotto.Visible = False

        Me.Pannello_CodiciArticolo.Visible = False
        Me.lbl_lotto.Visible = False
        Me.Txt_Lotto.Visible = False

        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                Me.Pannello_CodiciArticolo.Visible = True

                Me.Lbl_Ordinamento.Visible = True
                Me.Rbl_Ordinamento.Visible = True
                Me.Lbl_StampaLotto.Visible = True
                Me.Rbl_StampaLotto.Visible = True

                Me.Pannello_IntervalloMovimento.Visible = True
                Me.Pannello_Intervallo.Visible = False
                Me.Pannello_Giorno.Visible = False

                Me.lbl_categorie.Visible = True
                Me.cmb_CatProdotto.Visible = True

                Me.cmb_Prodotti.Items.Clear()
                Me.Pannello_Prodotti.Visible = True

                GestioneCheckComposizione()

                Cambia_CategoriaMagazzino()

                Me.lbl_lotto.Visible = True
                Me.Txt_Lotto.Visible = True

                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti, _
                  enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                Me.Pannello_IntervalloMovimento.Visible = False
                Me.Pannello_Intervallo.Visible = True
                Me.Pannello_Giorno.Visible = False

                Me.lbl_categorie.Visible = False
                Me.cmb_CatProdotto.Visible = False

                Me.cmb_Prodotti.Items.Clear()
                Me.Pannello_Prodotti.Visible = False

                GestioneCheckComposizione()

                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze

                Me.Lbl_StampaLotto.Visible = True
                Me.Rbl_StampaLotto.Visible = True

                Me.Pannello_CodiciArticolo.Visible = True

                If Me.TxtStampa.Text = "" Then
                    Me.TxtStampa.Text = CStr(Date.Today)
                End If

                Me.Pannello_IntervalloMovimento.Visible = False
                Me.Pannello_Intervallo.Visible = False
                Me.Pannello_Giorno.Visible = True

                Me.lbl_categorie.Visible = True
                Me.cmb_CatProdotto.Visible = True

                Me.cmb_Prodotti.Items.Clear()
                Me.Pannello_Prodotti.Visible = False

                GestioneCheckComposizione()

                Me.cmb_Prodotti.Items.Clear()
                Me.Pannello_Prodotti.Visible = True

                Me.lbl_lotto.Visible = True
                Me.Txt_Lotto.Visible = True


        End Select



    End Sub


    '###########################################################################
    Private Sub Cmb_Impresa_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Impresa.SelectedIndexChanged

        Cambia_Impresa()

    End Sub

    '###########################################################################
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

            objOmniLog.Recupera_Magazzino_Confezionato(Piva, _
                                                        "", _
                                                        objParametri_Server, _
                                                        sa_cod, _
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
    Private Sub Cmb_CentroAziendale_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_CentroAziendale.SelectedIndexChanged

        Cambia_CentroAziendale(Qs_Fabbricato_Cod)

    End Sub

    '###########################################################################
    Private Sub Cambia_CentroAziendale(ByVal fabbricato_cod As Integer)

        Me.Cmb_Magazzino.Items.Clear()
        Me.cmb_Prodotti.Items.Clear()

        If Me.Cmb_CentroAziendale.SelectedValue <> "" Then

            'Dim Num_Totale As Integer = 0
            Dim Piva As String
            Dim Sa_Cod As Integer

            Piva = Me.Cmb_Impresa.SelectedValue
            'Sa_Cod = CInt(Right(Me.Cmb_CentroAziendale.SelectedValue, Me.Cmb_CentroAziendale.SelectedValue.Length - 12))
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
                    If Me.Cmb_Magazzino.Items.Count > 1 Then
                        Me.Cmb_Magazzino.SelectedIndex = 1
                    End If
                End If
            End If

            Cambia_Magazzino()

            Me.Lbl_NumMagazzini.Text = CStr(Me.Cmb_Magazzino.Items.Count)

        End If

    End Sub

    '########################################################################################
    Private Sub Cmb_Magazzino_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Magazzino.SelectedIndexChanged

        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                Cambia_Magazzino()

            Case Else

                'niente

        End Select

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
            objF.MagazzinoIndirizzo_Leggi_2(Piva, Sa_Cod, Cmb_Magazzino.SelectedValue, _
                                                  Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                                  RegioneCod, _
                                                  objParametri_Server)

            'Seleziono la regione di appartenenza dell'impresa
            Cmb_Regioni.SelectedIndex = _
                Cmb_Regioni.Items.IndexOf( _
                    Cmb_Regioni.Items.FindByValue( _
                        RegioneCod))

            Cambia_CategoriaMagazzino()

        End If

    End Sub


    '########################################################################################
    Private Sub cmb_CatProdotto_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmb_CatProdotto.SelectedIndexChanged

        Cambia_CategoriaMagazzino()

    End Sub


    '###########################################################################
    Private Sub Cambia_CategoriaMagazzino()

        Me.cmb_Prodotti.Items.Clear()
        Me.Lbl_NumProdotti.Text = "0"

        GestioneCheckComposizione()

    End Sub


    '########################################################################################
    Private Sub ImgBtn_ProdottiCerca_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_ProdottiCerca.Click

        Carica_Prodotti()

    End Sub

    '########################################################################################
    Private Sub Carica_Prodotti()

        Me.cmb_Prodotti.Items.Clear()
        Me.Lbl_NumProdotti.Text = "0"


        Dim Filtro_Desc As String = Agro_SQL_SaveText(Me.Txt_ProdottoCerca.Text)

        Dim Data As Date
        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti
                If Me.TxtDataA.Text = "" Then
                    Me.TxtDataA.Text = CStr(Date.Today)
                End If
                Data = Me.TxtDataA.Text

                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti, _
                  enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari
                If Me.TxtDataInizio.Text = "" Then
                    Me.TxtDataInizio.Text = CStr(Date.Today)
                End If
                Data = Me.TxtDataInizio.Text

                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze
                If Me.TxtStampa.Text = "" Then
                    Me.TxtStampa.Text = CStr(Date.Today)
                End If
                Data = Me.TxtStampa.Text

        End Select

        If Me.cmb_CatProdotto.SelectedItem.Text <> "" Then

            Select Case Me.cmb_CatProdotto.SelectedValue

                Case MACCHINE, SEMENTI, ALTRE_MATERIE, MANGIMI, FARMACI, _
                    SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI, _
                                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI

                    'nel caso delle sementi non posso usare la ProdottiAnagrafica
                    'perchè altrimenti mi caricherebbe le tipologie sementi
                    'uso cmq questa funzione per tutte le materie_prime, perchè ho bisogno del codice negativo
                    '(per distinguere il amt_cod da pro_cod)
                    Dim clc = New AgronicaCoreUtility.CaricaListControl
                    clc.Materie_Prime(Me.cmb_Prodotti,
                                                                        True, "", "",
                                                                        CAU_CARICO,
                                                                        "",
                                                                        0,
                                                                        0,
                                                                        Me.cmb_CatProdotto.SelectedValue,
                                                                        True,
                                                                       Filtro_Desc,
                                                                        "",
                                                                         "",
                                                                        "",
                                                                        0, 0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO,
                                                                        0, 0, 0, 0, 0, 0, 0,
                                                                        Date.Today,
                                                                          "", "",
                                                                        objParametri_Server,
                                                                        objParametri_Utenti)

                Case Else

                    If Me.cmb_CatProdotto.SelectedValue = FERTILIZZANTI Then

                        'uso il caricamento specifico, perchè deve caricare anche i fertilzizanti aziendali
                        AgronicaCoreUtility.CaricaListControl.Fertilizzanti_2(Me.cmb_Prodotti, _
                                                                                True, "", "", _
                                                                                Filtro_Desc, _
                                                                                0, _
                                                                                True, _
                                                                                Me.Cmb_Impresa.SelectedValue, _
                                                                                True, _
                                                                                False, _
                                                                                "", "", _
                                                                                objParametri_Server)

                    Else

                        AgronicaCoreUtility.CaricaListControl.ProdottiAnagrafica(Me.cmb_Prodotti, _
                                                                                True, "", "", _
                                                                                Data, _
                                                                                Me.cmb_CatProdotto.SelectedItem.Value, _
                                                                                 Filtro_Desc, _
                                                                                False, _
                                                                                False, _
                                                                                "", _
                                                                                objParametri_Server)
                    End If

            End Select

            Me.Lbl_NumProdotti.Text = CStr(Me.cmb_Prodotti.Items.Count - 1)

            If Me.cmb_Prodotti.SelectedIndex = 0 Then
                If Me.cmb_Prodotti.Items.Count > 1 Then
                    Me.cmb_Prodotti.SelectedIndex = 1
                End If
            End If

        Else
            AgroMsgBox("Per caricare i prodotti è necessario selezionare la categoria di magazzino.", Page)
        End If

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

        'If TestoCercaRagSoc <> "" Then
        strFiltro = " (Imprese.Rag_Soc like '%" & TestoCercaRagSoc & "%') "
        'End If

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(Cmb_Impresa, False, "", "", _
                                                         strFiltro, " ORDER BY Imprese.Rag_Soc asc", _
                                                           objParametri_Server, _
                                                           objParametri_Utenti)


        'CaricaCombo_ImpreseUtente_Optimize(Server, Session, Page, _
        '                                    Me.Cmb_Impresa, _
        '                                    Num_Totale, _
        '                                    strFiltro, _
        '                                    "ORDER BY Imprese.rag_soc", _
        '                                    False, , )

        Me.Lbl_NumImprese.Text = CStr(Cmb_Impresa.Items.Count)

        Cambia_Impresa()



    End Sub


    '###########################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        Stampa()

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
        Dim Str_elem_cod As String
        Dim AlmenoUno As Boolean = False
        Dim FlagVisualizzaComposizione As Integer = 0


        'Controllo che siano selezionati un'impresa, un centro ed un magazzino

        If Not IsNothing(Me.Cmb_Impresa.SelectedItem) Then
            If Me.Cmb_Impresa.SelectedItem.Text = "" Then
                AgroMsgBox("Selezionare un'Impresa!", Page)
                Exit Sub
            Else
                Piva = Me.Cmb_Impresa.SelectedValue
            End If
        Else
            AgroMsgBox("Non ci sono Imprese! Impossibile stampare.", Page)
            Exit Sub
        End If

        If Not IsNothing(Me.Cmb_CentroAziendale.SelectedItem) Then
            If Me.Cmb_CentroAziendale.SelectedItem.Text = "" Then
                AgroMsgBox("Selezionare un Centro Aziendale!", Page)
                Exit Sub
            Else
                Sa_Cod = Me.Cmb_CentroAziendale.SelectedValue
            End If
        Else
            AgroMsgBox("Non ci sono Centri Aziendali! Impossibile stampare.", Page)
            Exit Sub
        End If

        If Not IsNothing(Me.Cmb_Magazzino.SelectedItem) Then
            If Me.Cmb_Magazzino.SelectedValue = "" Then
                AgroMsgBox("Selezionare un Magazzino!", Page)
                Exit Sub
            Else
                Fabbricato_Cod = Me.Cmb_Magazzino.SelectedValue
            End If
        Else
            AgroMsgBox("Non ci sono Magazzini! Impossibile stampare.", Page)
            Exit Sub
        End If


        'Controllo che nella stampa dei movimenti, poichè le date sono editabili
        'la data inizio sia minore della data fine

        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze

                TargetURL = PaginaLinkStampaSchedaGiacenzeMagazzino

                If Me.TxtStampa.Text = "" Then
                    AgroMsgBox("Selezionare la data in cui verificare le Giacenze di Magazzino!", Page)
                    Exit Sub
                Else
                    Data_Stampa = Me.TxtStampa.Text
                End If

                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                TargetURL = PaginaLinkStampaSchedaMovimentiMagazzino

                If Me.TxtDataDa.Text = "" Or Me.TxtDataA.Text = "" Then
                    AgroMsgBox("E' necessario specificare l'intervallo temporale in cui stampare i Movimenti di Magazzino!", Page)
                    Exit Sub
                End If

                If CDate(Me.TxtDataDa.Text) > CDate(Me.TxtDataA.Text) Then
                    AgroMsgBox("La data di inizio dell'intervallo non può essere superiore alla data di fine!", Page)
                    Exit Sub
                End If

                Data_Inizio = Me.TxtDataDa.Text
                Data_Fine = Me.TxtDataA.Text
                '-----------------------------------------------------------

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti, _
                    enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                If Me.Rbl_SchedaMagazzino.SelectedValue = enum_CodificaStampe.SchedaMagazzinoFertilizzanti Then
                    TargetURL = PaginaLinkStampaSchedaFertilizzantiMagazzino
                Else
                    TargetURL = PaginaLinkStampaSchedaProdottiFitosanitariMagazzino
                End If

                If Me.TxtDataInizio.Text = "" Or Me.TxtDataFine.Text = "" Then
                    AgroMsgBox("E' necessario specificare l'intervallo temporale in cui stampare i Movimenti di Magazzino!", Page)
                    Exit Sub
                End If

                If CDate(Me.TxtDataInizio.Text) > CDate(Me.TxtDataFine.Text) Then
                    AgroMsgBox("La data di inizio dell'intervallo non può essere superiore alla data di fine!", Page)
                    Exit Sub
                End If

                Data_Inizio = Me.TxtDataInizio.Text
                Data_Fine = Me.TxtDataFine.Text

                '-----------------------------------------------------------

        End Select

        If Me.cmb_CatProdotto.SelectedValue <> "" Then
            Elem_Cod = Me.cmb_CatProdotto.SelectedValue
        End If

        If Me.cmb_Prodotti.SelectedValue <> "" Then

            If CInt(Me.cmb_Prodotti.SelectedValue) > 0 Then
                Pro_Cod = CInt(Me.cmb_Prodotti.SelectedValue)
                Mat_Cod = 0
            Else
                Pro_Cod = 0
                Mat_Cod = -CInt(Me.cmb_Prodotti.SelectedValue)
            End If

        End If

        Tipo_Arrotondamento = Me.Rbl_Arrotondamento.SelectedValue

        If Me.Pannello_CodiciArticolo.Visible = True Then
            Dim j As Integer
            For j = 0 To Me.ChkList_Categorie.Items.Count - 1
                If Me.ChkList_Categorie.Items(j).Selected = True Then
                    Str_elem_cod += CStr(Me.ChkList_Categorie.Items(j).Value) + ","
                    AlmenoUno = True
                End If
            Next
            If AlmenoUno = False Then
                AgroMsgBox("E' necessario selezionare almeno una categoria di magazzino!", Page)
                Exit Sub
            Else
                Str_elem_cod = Left(Str_elem_cod, Str_elem_cod.Length - 1)
                Str_elem_cod = "(" + Str_elem_cod + ")"
            End If
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
        If Me.Chk_LogoRegione.Checked = True Then
            If Me.Cmb_Regioni.SelectedIndex >= 0 Then
                Session("Regione_Selezionata") = Me.Cmb_Regioni.SelectedValue.ToString
            End If
        End If

        Dim Querystring As String

        Querystring = "?p=" + Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) + _
                        "&s=" + Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&f=" + Stringa_Codifica(Fabbricato_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&e=" + Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&pro=" + Stringa_Codifica(Pro_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&mat=" + Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&arr=" + Stringa_Codifica(Tipo_Arrotondamento, AgroKey_EncoderDecoder, Server)

        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze

                Querystring += "&ds=" + Stringa_Codifica(Data_Stampa.ToShortDateString, AgroKey_EncoderDecoder, Server) + _
                                "&fec=" + Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Server) + _
                                "&fvc=" + Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder, Server) & _
                                  "&lot=" + Stringa_Codifica(Me.Txt_Lotto.Text, AgroKey_EncoderDecoder, Server) & _
                                  "&sl=" + Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                Querystring += "&di=" + Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder, Server) + _
                                "&df=" + Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder, Server) + _
                                "&ord=" + Stringa_Codifica(Me.Rbl_Ordinamento.SelectedValue, AgroKey_EncoderDecoder, Server) + _
                                "&fec=" + Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Server) & _
                                  "&lot=" + Stringa_Codifica(Me.Txt_Lotto.Text, AgroKey_EncoderDecoder, Server) & _
                                  "&sl=" + Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti, _
                        enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                Querystring += "&di=" + Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder, Server) + _
                                "&df=" + Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder, Server) + _
                                "&ord=" + Stringa_Codifica(Me.Rbl_Ordinamento.SelectedValue, AgroKey_EncoderDecoder, Server) + _
                                "&fvc=" + Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder, Server)

        End Select


        Page_NewWindow_2010(Page, _
                        TargetURL, Querystring, "SchedeMagazzino", , , , , , , , )

        'Page_NewWindow_2010(Page, _
        '                TargetURL, Querystring, , , , , , , , , "MainContent", True)

        ''================================================================


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

        'Controllo che siano selezionati un'impresa, un centro ed un magazzino

        If Not IsNothing(Me.Cmb_Impresa.SelectedItem) Then
            If Me.Cmb_Impresa.SelectedItem.Text = "" Then
                AgroMsgBox("Selezionare un'Impresa!", Page)
                Exit Sub
            Else
                Piva = Me.Cmb_Impresa.SelectedValue
            End If
        Else
            AgroMsgBox("Non ci sono Imprese! Impossibile stampare.", Page)
            Exit Sub
        End If

        If Not IsNothing(Me.Cmb_CentroAziendale.SelectedItem) Then
            If Me.Cmb_CentroAziendale.SelectedItem.Text = "" Then
                'AgroMsgBox("Selezionare un Centro Aziendale!", Page)
                'Exit Sub
            Else
                Sa_Cod = Me.Cmb_CentroAziendale.SelectedValue
            End If
        Else
            'AgroMsgBox("Non ci sono Centri Aziendali! Impossibile stampare.", Page)
            'Exit Sub
        End If

        If Not IsNothing(Me.Cmb_Magazzino.SelectedItem) Then
            If Me.Cmb_Magazzino.SelectedValue = "" Then
                'AgroMsgBox("Selezionare un Magazzino!", Page)
                'Exit Sub
            Else
                Fabbricato_Cod = Me.Cmb_Magazzino.SelectedValue
            End If
        Else
            'AgroMsgBox("Non ci sono Magazzini! Impossibile stampare.", Page)
            'Exit Sub
        End If

        'Controllo che nella stampa dei movimenti, poichè le date sono editabili
        'la data inizio sia minore della data fine

        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                TargetURL = PaginaLinkStampaSchedaMovimentiMagazzinoExcel

                If Me.TxtDataDa.Text = "" Or Me.TxtDataA.Text = "" Then
                    AgroMsgBox("E' necessario specificare l'intervallo temporale in cui stampare i Movimenti di Magazzino!", Page)
                    Exit Sub
                End If

                If CDate(Me.TxtDataDa.Text) > CDate(Me.TxtDataA.Text) Then
                    AgroMsgBox("La data di inizio dell'intervallo non può essere superiore alla data di fine!", Page)
                    Exit Sub
                End If

                Data_Inizio = Me.TxtDataDa.Text
                Data_Fine = Me.TxtDataA.Text
              
        End Select

        If Me.cmb_CatProdotto.SelectedValue <> "" Then
            Elem_Cod = Me.cmb_CatProdotto.SelectedValue
        End If

        If Me.cmb_Prodotti.SelectedValue <> "" Then
            If CInt(Me.cmb_Prodotti.SelectedValue) > 0 Then
                Pro_Cod = CInt(Me.cmb_Prodotti.SelectedValue)
                Mat_Cod = 0
            Else
                Pro_Cod = 0
                Mat_Cod = -CInt(Me.cmb_Prodotti.SelectedValue)
            End If
        End If

        Tipo_Arrotondamento = Me.Rbl_Arrotondamento.SelectedValue

        If Me.Pannello_CodiciArticolo.Visible = True Then
            Dim j As Integer
            For j = 0 To Me.ChkList_Categorie.Items.Count - 1
                If Me.ChkList_Categorie.Items(j).Selected = True Then
                    Str_elem_cod += CStr(Me.ChkList_Categorie.Items(j).Value) + ","
                    AlmenoUno = True
                End If
            Next
            If AlmenoUno = False Then
                AgroMsgBox("E' necessario selezionare almeno una categoria di magazzino!", Page)
                Exit Sub
            Else
                Str_elem_cod = Left(Str_elem_cod, Str_elem_cod.Length - 1)
                Str_elem_cod = "(" + Str_elem_cod + ")"
            End If
        End If

        ''================================================================

        Session("Regione_Selezionata") = ""
        If Me.Chk_LogoRegione.Checked = True Then
            If Me.Cmb_Regioni.SelectedIndex >= 0 Then
                Session("Regione_Selezionata") = Me.Cmb_Regioni.SelectedValue.ToString
            End If
        End If

        Dim Querystring As String

        Querystring = "?p=" + Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) + _
                        "&s=" + Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&f=" + Stringa_Codifica(Fabbricato_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&e=" + Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&pro=" + Stringa_Codifica(Pro_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&mat=" + Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) + _
                        "&arr=" + Stringa_Codifica(Tipo_Arrotondamento, AgroKey_EncoderDecoder, Server)

        Select Case Me.Rbl_SchedaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                Session("BloccaOperazioni") = CB_BloccaOperazioni.Checked

                Querystring += "&di=" + Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder, Server) + _
                                "&df=" + Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder, Server) + _
                                "&ord=" + Stringa_Codifica(Me.Rbl_Ordinamento.SelectedValue, AgroKey_EncoderDecoder, Server) + _
                                "&fec=" + Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Server) & _
                                  "&lot=" + Stringa_Codifica(Me.Txt_Lotto.Text, AgroKey_EncoderDecoder, Server) & _
                                  "&sl=" + Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder, Server)

        End Select


        Page_NewWindow_2010(Page, _
                        TargetURL, Querystring, "SchedeMagazzinoExcel", , , , , , , , )

        ''================================================================


    End Sub





    Private Sub ImgBtn_StampaExcel_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaExcel.Click

        Select Case Me.Rbl_SchedaMagazzino.SelectedValue
            Case enum_CodificaStampe.SchedaMagazzinoMovimenti
            Case Else
                AgroMsgBox("Funzione non ancora disponibile!", Page)
                Exit Sub
        End Select


        ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        'Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '            Server, Session, Page, Session("ASG_Utente_Username"), _
        '            Session("ASG_IdServizio"), _
        '             enum_Security_Attivita.Stampa_MovimentiMagazziniExcel, _
        '             enum_Security_Operazione.Lettura, _
        '            strDummy)


        ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

        'If UtenteAbilitato = False Then
        '    AgroMsgBox("L'utente non è abilitato a stampare i Movimenti di Magazzino in formato Excel!", Page)
        '    Exit Sub
        'End If

        StampaExcel()




    End Sub




End Class
