Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Filtro_Contatti
    Inherits System.Web.UI.Page

#Region " Filtro Esportazione Contatti "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents LblTitolo As System.Web.UI.WebControls.Label
    Protected WithEvents ImgIcona As System.Web.UI.WebControls.Image
    Protected WithEvents Txt_Impresa As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL13 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Impresa As System.Web.UI.WebControls.DropDownList
    Protected WithEvents LABEL9 As System.Web.UI.WebControls.Label
    Protected WithEvents Lbl_Stampa As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Stampa As System.Web.UI.WebControls.Panel
    Protected WithEvents ImgBtn_Esci As System.Web.UI.WebControls.ImageButton
    Protected WithEvents ImgBtn_Stampa As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LblDeselezionaTutto As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtnDeselezionaTutto As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LblSelezionaTutto As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtnSelezionaTutto As System.Web.UI.WebControls.ImageButton
    Protected WithEvents ChkGruppo3 As System.Web.UI.WebControls.CheckBoxList
    Protected WithEvents ChkGruppo2 As System.Web.UI.WebControls.CheckBoxList
    Protected WithEvents ChkGruppo1 As System.Web.UI.WebControls.CheckBoxList
    Protected WithEvents LblSeleziona As System.Web.UI.WebControls.Label
    Protected WithEvents BtnValiditaInizio As System.Web.UI.HtmlControls.HtmlInputButton
    Protected WithEvents Rbl_Registri As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Label6 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL1 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL2 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL4 As System.Web.UI.WebControls.Label
    Protected WithEvents Rbl_Visibilita As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Cmb_RapportiContabili As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Rbl_Tipologia As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents ImgBtn_CaricaImprese As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Check As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_Filtri_PacchettoIgiene As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL3 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL5 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL7 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Filtri_AnagraficaContatti As System.Web.UI.WebControls.Panel
    Protected WithEvents Cbl_Rapporto_Contabile As System.Web.UI.WebControls.CheckBoxList
    Protected WithEvents Panel1 As System.Web.UI.WebControls.Panel
    Protected WithEvents Rbl_Visibilita_Export_Contatti As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Rbl_Tipologia_contatti As System.Web.UI.WebControls.RadioButtonList

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Dim Qs_Piva As String
    Dim Report As Integer

    Dim x_Piva As String
    Dim x_Data_Inizio As String
    Dim x_Data_Fine As String
    Dim x_Filtro_SaCod As String = ""
    Dim x_Filtro_CodRapporto As String = ""
    Dim x_ID_CF As Integer = 0
    Dim x_Cod_Rapporto As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '=========================================================================
    '=========================================================================

    'Questa pagina di filtro viene chiamata
    'dal registro Clienti e dal registro Fornitori che si trovano al percorso GestioneStampe/PacchettoIgiene/Registro_ClientiFornitori
    'e dall'esportazione excel dei contatti che si trova al percorso GestioneEsportazioni/Esportazione_AnagraficaContatti

    '=========================================================================
    '=========================================================================


    '##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Try

            ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            'Dim strDummy As String      'controllo accesso negato.....
            'Dim UtenteAbilitato As Boolean

            'UtenteAbilitato = Controlla_Permessi_Utente_2( _
            '                            Server, Session, Page, _
            '                            Session("ASG_Utente_Username"), _
            '                            Session("ASG_IdServizio"), _
            '                            TipiEnumerativi.enum_Security_Attivita.Anagrafica_Contatto, _
            '                            TipiEnumerativi.enum_Security_Operazione.Lettura, _
            '                            strDummy)

            ''----- Se l'utente non ha il permesso per visualizzare la pagina ...
            'If UtenteAbilitato = False Then

            '    Dim strClose As String = "<script language='javascript'> window.close() </script>"
            '    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

            '    Exit Sub

            'End If


            '##############################################################
            '###################### QUERYSTRING ###########################
            '##############################################################

            Report = Session("ReportSelezionato")

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

            'Qs_Rag_Soc = Stringa_Decodifica(Request.QueryString("rs").ToString, _
            '                      AgroKey_EncoderDecoder, _
            '                      Server)

            'Qs_Anno = Stringa_Decodifica(Request.QueryString("a").ToString, _
            '                         AgroKey_EncoderDecoder, _
            '                         Server)

            'Qs_Di = Stringa_Decodifica(Request.QueryString("di").ToString, _
            '                       AgroKey_EncoderDecoder, _
            '                        Server)

            'Qs_Df = Stringa_Decodifica(Request.QueryString("df").ToString, _
            '                       AgroKey_EncoderDecoder, _
            '                       Server)


            If Me.IsPostBack Then

                Exit Sub

            End If


            'Lista Piva 
            If Report = enum_CodificaStampe.Esportazione_AnagraficaContatti Then

                'Dim FlagAnagraficaContatti As Boolean = False
                Dim ElencoChiaviImpresa As String
                Dim StringaPiva As String

                StringaPiva = Session("strXmlVariabilistampe")

                'modifica del 25/07/2011 in seguito a quella del 30/05/2011: 
                'la stringa MAGA viene già rimossa dalla pagina gestione richieste
                ' StringaPiva = Right(StringaPiva, StringaPiva.Length - 4)

                Dim XmlDoc As New System.Xml.XmlDocument
                'Dim XML_FiltroStampa As System.Xml.XmlElement
                Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
                Dim XML_VariabiliStampe As System.Xml.XmlElement
                Dim num_imprese As Integer
                Dim i As Integer
                Dim Piva, msg As String
                'Carico la stringa xml in un nuovo documento xml

                If StringaPiva <> "" Then

                    XmlDoc.LoadXml(StringaPiva)

                    If XmlDoc.HasChildNodes Then

                        'XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
                        'XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

                        XMLs_VariabiliStampe = XmlDoc.SelectNodes("//VariabiliStampe")

                        num_imprese = XMLs_VariabiliStampe.Count

                        '------------------------------------------------
                        '------------ INIZIO CICLO IMPRESE -------------
                        '------------------------------------------------

                        '-----------------------------------------------
                        ' la i scorre le imprese
                        '-----------------------------------------------
                        For i = 0 To num_imprese - 1

                            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                            If (IsNothing(XML_VariabiliStampe.GetAttribute("p"))) Then
                                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("p")) = "") Then
                                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                            End If

                            If msg <> "" Then
                                AgroMsgBox("Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg, Page)
                                Exit Sub
                            End If

                            'Ricavo la piva
                            Piva = XML_VariabiliStampe.GetAttribute("p")

                            'la chiave impresa è la piva
                            ElencoChiaviImpresa += ",'" & Piva & "' "

                        Next

                        'elimino la prima virgola
                        ElencoChiaviImpresa = Right(ElencoChiaviImpresa, ElencoChiaviImpresa.Length - 1)
                        viewstate("FlagAnagraficaContatti") = True
                        viewstate("ElencoChiaviImpresa") = ElencoChiaviImpresa

                    Else
                        AgroMsgBox("Non sono arrivati dati dal Filtro!", Page)
                        Exit Sub
                    End If
                Else
                    AgroMsgBox("Non sono arrivati dati dal Filtro!", Page)
                    Exit Sub
                End If

            Else
                'pacchetto igiene
                viewstate("FlagAnagraficaContatti") = False
            End If


            '----------------------------------
            '------------ Imprese -------------
            '----------------------------------
            'carico le imprese solamente se provengo da igiene
            If viewstate("FlagAnagraficaContatti") = False Then
                Carica_Imprese("", Qs_Piva)
                Me.Pannello_Filtri_PacchettoIgiene.Visible = True
                Me.Pannello_Filtri_AnagraficaContatti.Visible = False
            Else
                Pannello_Filtri_PacchettoIgiene.Visible = False
                Me.Pannello_Filtri_AnagraficaContatti.Visible = True
                Me.Pannello_Filtri_AnagraficaContatti.Style.Item("Top") = "56px"
                Me.Pannello_Filtri_AnagraficaContatti.Style.Item("Left") = "8px"
            End If


            Configura_Pannelli()



        Catch ex As Exception
            AgroMsgBox("Problemi durante il caricamento della pagina: " + vbCrLf + ex.Message, Page)
        End Try



    End Sub


    '##################################################################################
    Private Sub ImgBtn_Esci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Esci.Click

        Dim strClose As String = "<script language='javascript'>window.close()</script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))


    End Sub

    '##############################################################
    Private Sub Configura_Pannelli()

        Dim FiltroQuery_RappCont As String = ""
        Dim Testo_PrimaRiga As String = ""
        Dim Cod_PrimaRiga As String = ""
        Dim Flag_PrimaRiga As Boolean = False

        'Non permetto all'utente di cambiare output
        Me.Rbl_Registri.Enabled = False


        Select Case Report

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori

                Me.Rbl_Registri.SelectedValue = -3

                Configura_Check_ReportClientiFornitori()

                Imposta_SelezioneCheck_ReportClientiFornitori()

                'FiltroQuery_RappCont = " AND (Fornitore = 1 ) "
                FiltroQuery_RappCont = " (Fornitore = 1 ) "

                Me.ImgBtnSelezionaTutto.Enabled = False
                Me.ImgBtnDeselezionaTutto.Enabled = False

                '------------------------------------------------------

            Case enum_CodificaStampe.PacchettoIgiene_RegistroClienti

                Me.Rbl_Registri.SelectedValue = -2

                Configura_Check_ReportClientiFornitori()

                Imposta_SelezioneCheck_ReportClientiFornitori()

                'FiltroQuery_RappCont = " AND (Cliente = 1) "
                FiltroQuery_RappCont = " (Cliente = 1) "

                Me.ImgBtnSelezionaTutto.Enabled = False
                Me.ImgBtnDeselezionaTutto.Enabled = False

                '------------------------------------------------------

            Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                Me.Rbl_Registri.SelectedValue = 0

                Configura_Check_EsportazioneContatti()

                Imposta_SelezioneCheck_EsportazioneContatti()

                Testo_PrimaRiga = "Tutti"
                Cod_PrimaRiga = "-999"
                Flag_PrimaRiga = True

                '------------------------------------------------------

            Case Else

                Me.ImgBtnSelezionaTutto.Enabled = True
                Me.ImgBtnDeselezionaTutto.Enabled = True

                Testo_PrimaRiga = "Tutti"
                Cod_PrimaRiga = "-999"
                Flag_PrimaRiga = True

                '------------------------------------------------------

        End Select


        '----------------------------------
        '---- Rapporti Contabili ----------
        '----------------------------------
        If viewstate("FlagAnagraficaContatti") = False Then
            'CaricaCombo_RapportiContabili(Server, Session, Page, Me.Cmb_RapportiContabili, 0, False, 0, False, False, False, False, False, FiltroQuery_RappCont, Testo_PrimaRiga, Cod_PrimaRiga, Flag_PrimaRiga)
            AgronicaCoreUtility.CaricaListControl.RapportiContabili(Me.Cmb_RapportiContabili, _
                                        Flag_PrimaRiga, Testo_PrimaRiga, Cod_PrimaRiga, _
                                    0, False, 0, False, False, False, False, False, False, False, FiltroQuery_RappCont, "", objParametri_Server)
        Else
            'CaricaCheckBoxList_RapportiContabili(Server, Session, Page, _
            '            Me.Cbl_Rapporto_Contabile, 0, False, 0, False, False, False, False, False, FiltroQuery_RappCont)

            AgronicaCoreUtility.CaricaListControl.RapportiContabili(Me.Cbl_Rapporto_Contabile, _
                            False, "", "", _
                        0, False, 0, False, False, False, False, False, False, False, FiltroQuery_RappCont, "", objParametri_Server)
        End If


    End Sub


    '###################################################################################
    Private Sub Imposta_SelezioneCheck_ReportClientiFornitori()


        'GRUPPO 1
        'piva dell'impresa
        Me.ChkGruppo1.Items.FindByValue(0).Selected = False
        'rag soc dell'impresa
        Me.ChkGruppo1.Items.FindByValue(1).Selected = False
        'cod_contatto
        Me.ChkGruppo1.Items.FindByValue(2).Selected = False
        'rag_soc contatto
        Me.ChkGruppo1.Items.FindByValue(3).Selected = True
        'sa_cod
        Me.ChkGruppo1.Items.FindByValue(4).Selected = False
        'id_cf
        Me.ChkGruppo1.Items.FindByValue(5).Selected = False
        'convenevoli
        Me.ChkGruppo1.Items.FindByValue(6).Selected = False
        'tipo indirizzo default
        Me.ChkGruppo1.Items.FindByValue(7).Selected = False
        'sconto default
        Me.ChkGruppo1.Items.FindByValue(8).Selected = False
        'cod risum
        Me.ChkGruppo1.Items.FindByValue(9).Selected = False
        'spesometro
        Me.ChkGruppo1.Items.FindByValue(10).Selected = False

        'GRUPPO 2
        'settore_des (progessivo)
        Me.ChkGruppo2.Items.FindByValue(10).Selected = False
        'attivita_des
        Me.ChkGruppo2.Items.FindByValue(11).Selected = False
        'validita inizio e fine risorsa umana
        Me.ChkGruppo2.Items.FindByValue(12).Selected = False
        'rapporto_des
        Me.ChkGruppo2.Items.FindByValue(13).Selected = False
        'patentino
        Me.ChkGruppo2.Items.FindByValue(14).Selected = False
        ''data inizio e fine patentino
        'Me.ChkGruppo2.Items.FindByValue(15).Selected = False
        ''corrispettivo
        'Me.ChkGruppo2.Items.FindByValue(16).Selected = False
        ''ore settimanali, giorni malattia, giorni ferie, giorni goduti
        'Me.ChkGruppo2.Items.FindByValue(17).Selected = False
        'indirizzo, farzione, cap, comune, provincia
        Me.ChkGruppo2.Items.FindByValue(18).Selected = True
        'telefono
        Me.ChkGruppo2.Items.FindByValue(19).Selected = True

        'GRUPPO 3
        'fax
        Me.ChkGruppo3.Items.FindByValue(20).Selected = True
        'email
        Me.ChkGruppo3.Items.FindByValue(21).Selected = True
        'cellulare
        Me.ChkGruppo3.Items.FindByValue(22).Selected = False
        'persona referente
        Me.ChkGruppo3.Items.FindByValue(23).Selected = True
        ''istituti di credito
        'Me.ChkGruppo3.Items.FindByValue(24).Selected = False
        ''coordinate iban
        'Me.ChkGruppo3.Items.FindByValue(25).Selected = False
        ''data apertura ed estinzione conto
        'Me.ChkGruppo3.Items.FindByValue(26).Selected = False
        'tipologia prodotto
        Me.ChkGruppo3.Items.FindByValue(27).Selected = True
        ''conti economici
        'Me.ChkGruppo3.Items.FindByValue(28).Selected = False
        ''codice gias pre importazione e impresa da cui si è importato il contatto
        'Me.ChkGruppo3.Items.FindByValue(29).Selected = False
        'listino vendita
        Me.ChkGruppo3.Items.FindByValue(30).Selected = False

    End Sub

    '###################################################################################
    Private Sub Configura_Check_ReportClientiFornitori()

        'GRUPPO 1
        'piva dell'impresa
        Me.ChkGruppo1.Items.FindByValue(0).Text = "<font color= ""gray""><i>" + "Partita IVA dell'Impresa che ha creato il contatto" + "</i></font>"
        'rag soc dell'impresa
        Me.ChkGruppo1.Items.FindByValue(1).Text = "<font color= ""gray""><i>" + "Ragione Sociale dell'Impresa che ha creato il contatto" + "</i></font>"
        'cod_contatto
        Me.ChkGruppo1.Items.FindByValue(2).Text = "<font color= ""gray""><i>" + "Partita IVA / Codice Fiscale" + "</i></font>"
        'rag_soc contatto
        Me.ChkGruppo1.Items.FindByValue(3).Text = "Ragione Sociale / Nome e Cognome"
        'sa_cod
        Me.ChkGruppo1.Items.FindByValue(4).Text = "<font color= ""gray""><i>" + "Visibilità" + "</i></font>"
        'id_cf
        Me.ChkGruppo1.Items.FindByValue(5).Text = "<font color= ""gray""><i>" + "Tipologia" + "</i></font>"
        'convenevoli
        Me.ChkGruppo1.Items.FindByValue(6).Text = "<font color= ""gray""><i>" + "Convenevoli" + "</i></font>"
        'tipo indirizzo default
        Me.ChkGruppo1.Items.FindByValue(7).Text = "<font color= ""gray""><i>" + "Tipo Indirizzo di Default per documenti contabili" + "</i></font>"
        'sconto default
        Me.ChkGruppo1.Items.FindByValue(8).Text = "<font color= ""gray""><i>" + "Sconto di Default per Documenti Contabili Emessi" + "</i></font>"
        'cod risum
        Me.ChkGruppo1.Items.FindByValue(9).Text = "<font color= ""gray""><i>" + "Codice GIAS" + "</i></font>"
        'spesometro
        Me.ChkGruppo1.Items.FindByValue(10).Text = "<font color= ""gray""><i>" + "Flag Spesometro" + "</i></font>"


        'GRUPPO 2
        'settore_des (progessivo)
        Me.ChkGruppo2.Items.FindByValue(10).Text = "<font color= ""gray""><i>" + "Progressivo" + "</i></font>"
        'attivita_des
        Me.ChkGruppo2.Items.FindByValue(11).Text = "<font color= ""gray""><i>" + "Attività" + "</i></font>"
        'validita inizio e fine risorsa umana
        Me.ChkGruppo2.Items.FindByValue(12).Text = "<font color= ""gray""><i>" + "Validita Inizio e Fine" + "</i></font>"
        'rapporto_des
        Me.ChkGruppo2.Items.FindByValue(13).Text = "<font color= ""gray""><i>" + "Rapporto Contabile" + "</i></font>"
        'patentino
        Me.ChkGruppo2.Items.FindByValue(14).Text = "<font color= ""gray""><i>" + "Dati Patentino" + "</i></font>"
        ''data inizio e fine patentino
        'Me.ChkGruppo2.Items.FindByValue(15).Text = "<font color= ""gray""><i>" + "Validita Inizio e Fine Patentino" + "</i></font>"
        ''corrispettivo
        'Me.ChkGruppo2.Items.FindByValue(16).Text = "<font color= ""gray""><i>" + "Corrispettivo" + "</i></font>"
        ''ore settimanali, giorni malattia, giorni ferie, giorni goduti
        'Me.ChkGruppo2.Items.FindByValue(17).Text = "<font color= ""gray""><i>" + "Ore Settimanali, Giorni Malattia, Giorni Ferie, Giorni Goduti" + "</i></font>"
        'indirizzo, farzione, cap, comune, provincia
        Me.ChkGruppo2.Items.FindByValue(18).Text = "Indirizzo, Frazione, CAP, Comune, Provincia"
        'telefono
        Me.ChkGruppo2.Items.FindByValue(19).Text = "Telefono"

        'GRUPPO 3
        'fax
        Me.ChkGruppo3.Items.FindByValue(20).Text = "Fax"
        'email
        Me.ChkGruppo3.Items.FindByValue(21).Text = "Email"
        'cellulare
        Me.ChkGruppo3.Items.FindByValue(22).Text = "<font color= ""gray""><i>" + "Cellulare" + "</i></font>"
        'persona referente
        Me.ChkGruppo3.Items.FindByValue(23).Text = "Persona Referente"
        ''istituti di credito
        'Me.ChkGruppo3.Items.FindByValue(24).Text = "<font color= ""gray""><i>" + "Istituto di Credito" + "</i></font>"
        ''coordinate iban
        'Me.ChkGruppo3.Items.FindByValue(25).Text = "<font color= ""gray""><i>" + "Coordinate IBAN" + "</i></font>"
        ''data apertura ed estinzione conto
        'Me.ChkGruppo3.Items.FindByValue(26).Text = "<font color= ""gray""><i>" + "Data Apertura e Estinzione Conto" + "</i></font>"
        'tipologia prodotto
        Me.ChkGruppo3.Items.FindByValue(27).Text = "<font color= ""gray""><i>" + "Tipologia Prodotto Acquistato / Venduto" + "</i></font>"
        ''conti economici
        'Me.ChkGruppo3.Items.FindByValue(28).Text = "<font color= ""gray""><i>" + "Conti Economici Direttamente Imputabili" + "</i></font>"
        ''codice gias pre importazione e impresa da cui si è importato il contatto
        'Me.ChkGruppo3.Items.FindByValue(29).Text = "<font color= ""gray""><i>" + "Codice GIAS pre importazione e Impresa da cui è stato importato il Contatto" + "</i></font>"


    End Sub


    '###################################################################################
    Private Sub Imposta_SelezioneCheck_EsportazioneContatti()


        'GRUPPO 1
        'piva dell'impresa
        Me.ChkGruppo1.Items.FindByValue(0).Selected = False
        'rag soc dell'impresa
        Me.ChkGruppo1.Items.FindByValue(1).Selected = True
        'cod_contatto
        Me.ChkGruppo1.Items.FindByValue(2).Selected = True
        'rag_soc contatto
        Me.ChkGruppo1.Items.FindByValue(3).Selected = True
        'sa_cod
        Me.ChkGruppo1.Items.FindByValue(4).Selected = False
        'id_cf
        Me.ChkGruppo1.Items.FindByValue(5).Selected = False
        'convenevoli
        Me.ChkGruppo1.Items.FindByValue(6).Selected = False
        'tipo indirizzo default
        Me.ChkGruppo1.Items.FindByValue(7).Selected = False
        'sconto default
        Me.ChkGruppo1.Items.FindByValue(8).Selected = False
        'cod risum
        Me.ChkGruppo1.Items.FindByValue(9).Selected = False
        'spesometro
        Me.ChkGruppo1.Items.FindByValue(10).Selected = False

        'GRUPPO 2
        'settore_des (progessivo)
        Me.ChkGruppo2.Items.FindByValue(10).Selected = False
        'attivita_des
        Me.ChkGruppo2.Items.FindByValue(11).Selected = False
        'validita inizio e fine risorsa umana
        Me.ChkGruppo2.Items.FindByValue(12).Selected = False
        'rapporto_des
        Me.ChkGruppo2.Items.FindByValue(13).Selected = False
        'patentino
        Me.ChkGruppo2.Items.FindByValue(14).Selected = False
        ''data inizio e fine patentino
        'Me.ChkGruppo2.Items.FindByValue(15).Selected = False
        ''corrispettivo
        'Me.ChkGruppo2.Items.FindByValue(16).Selected = False
        ''ore settimanali, giorni amlattia, giorni ferie, giorni goduti
        'Me.ChkGruppo2.Items.FindByValue(17).Selected = False
        'indirizzo, frazione, cap, comune, provincia
        Me.ChkGruppo2.Items.FindByValue(18).Selected = True
        'telefono
        Me.ChkGruppo2.Items.FindByValue(19).Selected = True

        'GRUPPO 3
        'fax
        Me.ChkGruppo3.Items.FindByValue(20).Selected = True
        'email
        Me.ChkGruppo3.Items.FindByValue(21).Selected = True
        'cellulare
        Me.ChkGruppo3.Items.FindByValue(22).Selected = True
        'persona referente
        Me.ChkGruppo3.Items.FindByValue(23).Selected = False
        ''istituti di credito
        'Me.ChkGruppo3.Items.FindByValue(24).Selected = False
        ''coordinate iban
        'Me.ChkGruppo3.Items.FindByValue(25).Selected = False
        ''data apertura ed estinzione conto
        'Me.ChkGruppo3.Items.FindByValue(26).Selected = False
        'tipologia prodotto
        Me.ChkGruppo3.Items.FindByValue(27).Selected = False
        ''conti economici
        'Me.ChkGruppo3.Items.FindByValue(28).Selected = False
        ''codice gias pre importazione e impresa da cui si è importato il contatto
        'Me.ChkGruppo3.Items.FindByValue(29).Selected = False
        'listino vendita
        Me.ChkGruppo3.Items.FindByValue(30).Selected = False
        'pec
        Me.ChkGruppo3.Items.FindByValue(31).Selected = True
        'sdi
        Me.ChkGruppo3.Items.FindByValue(32).Selected = True



    End Sub

    '###################################################################################
    Private Sub Configura_Check_EsportazioneContatti()

        'GRUPPO 1
        'piva dell'impresa
        Me.ChkGruppo1.Items.FindByValue(0).Text = "Partita IVA dell'Impresa che ha creato il contatto"
        'rag soc dell'impresa
        Me.ChkGruppo1.Items.FindByValue(1).Text = "Ragione Sociale dell'Impresa che ha creato il contatto"
        'cod_contatto
        Me.ChkGruppo1.Items.FindByValue(2).Text = "Partita IVA / Codice Fiscale"
        'rag_soc contatto
        Me.ChkGruppo1.Items.FindByValue(3).Text = "Ragione Sociale / Nome e Cognome"
        'sa_cod
        Me.ChkGruppo1.Items.FindByValue(4).Text = "Visibilità"
        'id_cf
        Me.ChkGruppo1.Items.FindByValue(5).Text = "Tipologia"
        'convenevoli
        Me.ChkGruppo1.Items.FindByValue(6).Text = "Convenevoli"
        'tipo indirizzo default
        Me.ChkGruppo1.Items.FindByValue(7).Text = "Tipo Indirizzo di Default per documenti contabili"
        'sconto default
        Me.ChkGruppo1.Items.FindByValue(8).Text = "Sconto di Default per Documenti Contabili Emessi"
        'cod risum
        Me.ChkGruppo1.Items.FindByValue(9).Text = "Codice GIAS"
        'spesometro
        Me.ChkGruppo1.Items.FindByValue(10).Text = "Flag Spesometro"

        'GRUPPO 2
        'settore_des (progessivo)
        Me.ChkGruppo2.Items.FindByValue(10).Text = "Progressivo"
        'attivita_des
        Me.ChkGruppo2.Items.FindByValue(11).Text = "Attività"
        'validita inizio e fine risorsa umana
        Me.ChkGruppo2.Items.FindByValue(12).Text = "Validita Inizio e Fine"
        'rapporto_des
        Me.ChkGruppo2.Items.FindByValue(13).Text = "Rapporto Contabile"
        'patentino
        Me.ChkGruppo2.Items.FindByValue(14).Text = "Dati Patentino"
        ''data inizio e fine patentino
        'Me.ChkGruppo2.Items.FindByValue(15).Text = "Validita Inizio e Fine Patentino"
        ''corrispettivo
        'Me.ChkGruppo2.Items.FindByValue(16).Text = "Corrispettivo"
        ''ore settimanali, giorni amlattia, giorni ferie, giorni goduti
        'Me.ChkGruppo2.Items.FindByValue(17).Text = "Ore Settimanali, Giorni Malattia, Giorni Ferie, Giorni Goduti"
        'indirizzo, farzione, cap, comune, provincia
        Me.ChkGruppo2.Items.FindByValue(18).Text = "Indirizzo, Frazione, CAP, Comune, Provincia, Stato"
        'telefono
        Me.ChkGruppo2.Items.FindByValue(19).Text = "Telefono"

        'GRUPPO 3
        'fax
        Me.ChkGruppo3.Items.FindByValue(20).Text = "Fax"
        'email
        Me.ChkGruppo3.Items.FindByValue(21).Text = "Email"
        'cellulare
        Me.ChkGruppo3.Items.FindByValue(22).Text = "Cellulare"
        'persona referente
        Me.ChkGruppo3.Items.FindByValue(23).Text = "<font color= ""gray""><i>" + "Persona Referente" + "</i></font>"
        'listino vendita
        Me.ChkGruppo3.Items.FindByValue(30).Text = "Listino Vendita associato"
        ''istituti di credito
        'Me.ChkGruppo3.Items.FindByValue(24).Text = "Istituto di Credito"
        ''coordinate iban
        'Me.ChkGruppo3.Items.FindByValue(25).Text = "Coordinate IBAN"
        ''data apertura ed estinzione conto
        'Me.ChkGruppo3.Items.FindByValue(26).Text = "Data Apertura e Estinzione Conto"
        'tipologia prodotto
        Me.ChkGruppo3.Items.FindByValue(27).Text = "<font color= ""gray""><i>" + "Tipologia Prodotto Acquistato / Venduto" + "</i></font>"
        ''conti economici
        'Me.ChkGruppo3.Items.FindByValue(28).Text = "Conti Economici Direttamente Imputabili"
        ''codice gias pre importazione e impresa da cui si è importato il contatto
        'Me.ChkGruppo3.Items.FindByValue(29).Text = "Codice GIAS pre importazione e Impresa da cui è stato importato il Contatto"


    End Sub



    '###################################################################################
    Private Sub Disattiva_SelezioneCheck_EsportazioneContatti()

        'GRUPPO 1
        ''piva dell'impresa
        'Me.ChkGruppo1.Items.FindByValue(0).Selected = False
        ''rag soc dell'impresa
        'Me.ChkGruppo1.Items.FindByValue(1).Selected = False
        ''cod_contatto
        'Me.ChkGruppo1.Items.FindByValue(2).Selected = True
        ''rag_soc contatto
        'Me.ChkGruppo1.Items.FindByValue(3).Selected = True
        ''sa_cod
        'Me.ChkGruppo1.Items.FindByValue(4).Selected = False
        ''id_cf
        'Me.ChkGruppo1.Items.FindByValue(5).Selected = False
        ''convenevoli
        'Me.ChkGruppo1.Items.FindByValue(6).Selected = False
        'tipo indirizzo default
        Me.ChkGruppo1.Items.FindByValue(7).Selected = False
        'sconto default
        Me.ChkGruppo1.Items.FindByValue(8).Selected = False
        ''cod risum
        'Me.ChkGruppo1.Items.FindByValue(9).Selected = False
        'spesometro
        Me.ChkGruppo1.Items.FindByValue(10).Selected = False

        'GRUPPO 2
        ''settore_des (progessivo)
        'Me.ChkGruppo2.Items.FindByValue(10).Selected = False
        ''attivita_des
        'Me.ChkGruppo2.Items.FindByValue(11).Selected = False
        ''validita inizio e fine risorsa umana
        'Me.ChkGruppo2.Items.FindByValue(12).Selected = False
        ''rapporto_des
        'Me.ChkGruppo2.Items.FindByValue(13).Selected = True
        'patentino
        Me.ChkGruppo2.Items.FindByValue(14).Selected = False
        ''data inizio e fine patentino
        'Me.ChkGruppo2.Items.FindByValue(15).Selected = False
        ''corrispettivo
        'Me.ChkGruppo2.Items.FindByValue(16).Selected = False
        ''ore settimanali, giorni amlattia, giorni ferie, giorni goduti
        'Me.ChkGruppo2.Items.FindByValue(17).Selected = False
        ''indirizzo, farzione, cap, comune, provincia
        'Me.ChkGruppo2.Items.FindByValue(18).Selected = True
        ''telefono
        'Me.ChkGruppo2.Items.FindByValue(19).Selected = True

        'GRUPPO 3
        ''fax
        'Me.ChkGruppo3.Items.FindByValue(20).Selected = True
        ''email
        'Me.ChkGruppo3.Items.FindByValue(21).Selected = True
        ''cellulare
        'Me.ChkGruppo3.Items.FindByValue(22).Selected = True
        'persona referente
        Me.ChkGruppo3.Items.FindByValue(23).Selected = False
        ''istituti di credito
        'Me.ChkGruppo3.Items.FindByValue(24).Selected = False
        ''coordinate iban
        'Me.ChkGruppo3.Items.FindByValue(25).Selected = False
        ''data apertura ed estinzione conto
        'Me.ChkGruppo3.Items.FindByValue(26).Selected = False
        'tipologia prodotto
        Me.ChkGruppo3.Items.FindByValue(27).Selected = False
        ''conti economici
        'Me.ChkGruppo3.Items.FindByValue(28).Selected = False
        'codice gias pre importazione e impresa da cui si è importato il contatto
        Me.ChkGruppo3.Items.FindByValue(29).Selected = False
        Me.ChkGruppo3.Items.FindByValue(30).Selected = False
        Me.ChkGruppo3.Items.FindByValue(31).Selected = False
        Me.ChkGruppo3.Items.FindByValue(32).Selected = False


    End Sub



    '###################################################################################
    Private Sub ChkGruppo1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkGruppo1.SelectedIndexChanged

        Select Case Report

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori, _
                    enum_CodificaStampe.PacchettoIgiene_RegistroClienti

                Imposta_SelezioneCheck_ReportClientiFornitori()

            Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                'Disattiva_SelezioneCheck_EsportazioneContatti()

            Case Else


        End Select

    End Sub



    '###################################################################################
    Private Sub ChkGruppo2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkGruppo2.SelectedIndexChanged

        Select Case Report

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori, _
                    enum_CodificaStampe.PacchettoIgiene_RegistroClienti

                Imposta_SelezioneCheck_ReportClientiFornitori()

            Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                'Disattiva_SelezioneCheck_EsportazioneContatti()

            Case Else


        End Select

    End Sub


    '###################################################################################
    Private Sub ChkGruppo3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkGruppo3.SelectedIndexChanged

        Select Case Report

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori, _
                    enum_CodificaStampe.PacchettoIgiene_RegistroClienti

                Imposta_SelezioneCheck_ReportClientiFornitori()

            Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                'Disattiva_SelezioneCheck_EsportazioneContatti()

            Case Else


        End Select

    End Sub

    '###################################################################################
    Private Sub ImgBtnSelezionaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnSelezionaTutto.Click

        'Tutti_Check_Seleziona()

        Select Case Report

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori, _
                    enum_CodificaStampe.PacchettoIgiene_RegistroClienti

                Imposta_SelezioneCheck_ReportClientiFornitori()

            Case enum_CodificaStampe.Esportazione_AnagraficaContatti
                'Dim i As Integer
                'For i = 0 To Me.ChkGruppo1.Items.Count - 1
                '    Me.ChkGruppo1.Items.FindByValue(i).Selected = True
                'Next
                'For i = 0 To Me.ChkGruppo2.Items.Count - 1
                '    Me.ChkGruppo2.Items.FindByValue(i).Selected = True
                'Next
                'For i = 0 To Me.ChkGruppo3.Items.Count - 1
                '    Me.ChkGruppo3.Items.FindByValue(i).Selected = True
                'Next
                'Imposta_SelezioneCheck_EsportazioneContatti()
                Tutti_Check_Seleziona()
        End Select



    End Sub


    '###################################################################################
    Private Sub ImgBtnDeselezionaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnDeselezionaTutto.Click

        Select Case Report

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori, _
                    enum_CodificaStampe.PacchettoIgiene_RegistroClienti

                Imposta_SelezioneCheck_ReportClientiFornitori()

            Case enum_CodificaStampe.Esportazione_AnagraficaContatti
                'Dim i As Integer
                'For i = 0 To Me.ChkGruppo1.Items.Count - 1
                '    Me.ChkGruppo1.Items.FindByValue(i).Selected = False
                'Next
                'For i = 0 To Me.ChkGruppo2.Items.Count - 1
                '    Me.ChkGruppo2.Items.FindByValue(i).Selected = False
                'Next
                'For i = 0 To Me.ChkGruppo3.Items.Count - 1
                '    Me.ChkGruppo3.Items.FindByValue(i).Selected = False
                'Next
                Tutti_Check_Deseleziona()


        End Select

    End Sub


    '###################################################################################
    Private Sub Tutti_Check_Seleziona()

        Dim i As Integer

        For i = 0 To 10
            Me.ChkGruppo1.Items.FindByValue(i).Selected = True
        Next

        For i = 10 To 19
            Select Case i
                Case 15, 16, 17
                    'disattivati
                Case Else
                    Me.ChkGruppo2.Items.FindByValue(i).Selected = True
            End Select
        Next

        For i = 20 To 32
            Select Case i
                Case 24, 25, 26, 28, 29, 23, 27
                Case Else
                    Me.ChkGruppo3.Items.FindByValue(i).Selected = True
            End Select
        Next

    End Sub


    '###################################################################################
    Private Sub Tutti_Check_Deseleziona()

        Dim i As Integer

        For i = 0 To 10
            Me.ChkGruppo1.Items.FindByValue(i).Selected = False
        Next

        For i = 10 To 19
            Select Case i
                Case 15, 16, 17
                    'disattivati
                Case Else
                    Me.ChkGruppo2.Items.FindByValue(i).Selected = False
            End Select
        Next

        For i = 20 To 32
            Select Case i
                Case 24, 25, 26, 28, 29
                Case Else
                    Me.ChkGruppo3.Items.FindByValue(i).Selected = False
            End Select
        Next

    End Sub


    ''###################################################################################
    ''Funziona solo per il primo listitem del gruppo... uff
    'Private Sub Abilita_Tutti_Check()

    '    Dim i As Integer

    '    For i = 0 To 9
    '        If Not IsNothing(Me.Pannello_Check.FindControl("ChkGruppo1").Controls.Item(i)) Then
    '            CType(Me.Pannello_Check.FindControl("ChkGruppo1").Controls.Item(i), CheckBox).Enabled = True
    '        End If
    '    Next

    '    For i = 10 To 19
    '        CType(FindControl(Me.ChkGruppo2.Items.FindByValue(i).Value), CheckBox).Enabled = True
    '    Next

    '    For i = 20 To 29
    '        CType(FindControl(Me.ChkGruppo3.Items.FindByValue(i).Value), CheckBox).Enabled = True
    '    Next

    '    CType(FindControl(Me.ChkGruppo3.Items.FindByValue(28).Value), CheckBox).Enabled = False
    '    CType(FindControl(Me.ChkGruppo3.Items.FindByValue(29).Value), CheckBox).Enabled = False

    'End Sub


    ''###################################################################################
    ''Funziona solo per il primo listitem del gruppo... uff
    'Private Sub Disabilita_Tutti_Check()

    '    Dim i As Integer

    '    For i = 0 To 9
    '        If Not IsNothing(Me.Pannello_Check.FindControl("ChkGruppo1").Controls.Item(i)) Then
    '            CType(Me.Pannello_Check.FindControl("ChkGruppo1").Controls.Item(i), CheckBox).Enabled = False
    '        End If
    '    Next

    '    'For i = 10 To 19
    '    '    If Not IsNothing(FindControl("ChkGruppo2_" + Me.ChkGruppo2.Items.FindByValue(i).Value)) Then
    '    '        CType(FindControl("ChkGruppo2_" + Me.ChkGruppo2.Items.FindByValue(i).Value), CheckBox).Enabled = False
    '    '    End If
    '    'Next

    '    'For i = 20 To 29
    '    '    If Not IsNothing(FindControl("ChkGruppo3_" + Me.ChkGruppo3.Items.FindByValue(i).Value)) Then
    '    '        CType(FindControl("ChkGruppo3_" + Me.ChkGruppo3.Items.FindByValue(i).Value), CheckBox).Enabled = False
    '    '    End If
    '    'Next

    'End Sub



    '########################################################################################
    Private Sub ImgBtn_CaricaImprese_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CaricaImprese.Click

        If Me.Txt_Impresa.Text = "" Then
            AgroMsgBox("Impostare un filtro sul nome dell'impresa!", Page)
        Else
            Carica_Imprese(Me.Txt_Impresa.Text, "")
        End If

    End Sub



    '########################################################################################
    Private Sub Carica_Imprese(ByVal TestoCercaRagSoc As String, _
                                ByVal Piva As String)

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0


        '----------------------------------------------------------------
        '--- Filtro personalizzato 
        '----------------------------------------------------------------

        If Piva <> "" Then
            strFiltro = " (Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "') "
        Else
            strFiltro = " (Imprese.Rag_Soc like '%" & Agro_SQL_SaveText(TestoCercaRagSoc) & "%') "
        End If

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



    End Sub




    '##############################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        If viewstate("FlagAnagraficaContatti") = False Then
            If IsNothing(Me.Cmb_Impresa.SelectedItem) Then
                AgroMsgBox("E' necessario selezionare un'impresa!", Page)
            Else
                Stampa()
            End If
        Else
            'x_Data_Fine = AgroDataFine
            'x_Data_Inizio = AgroDataInizio
            Stampa()
        End If

    End Sub


    '##############################################################
    Private Sub Stampa()

        Try

            '------------------------------
            '-------------- PIVA ----------
            '------------------------------

            x_Piva = Me.Cmb_Impresa.SelectedValue



            '------------------------------
            '----- RAPPORTO CONTABILI -----
            '------------------------------
            If viewstate("FlagAnagraficaContatti") = False Then

                If Me.Cmb_RapportiContabili.SelectedValue <> "-999" Then
                    'se non ho selezionato tutti i rapporti
                    x_Cod_Rapporto = Me.Cmb_RapportiContabili.SelectedValue
                End If
            Else
                Dim j As Integer
                For j = 0 To Me.Cbl_Rapporto_Contabile.Items.Count - 1
                    If Cbl_Rapporto_Contabile.Items(j).Selected = True Then
                        x_Cod_Rapporto = x_Cod_Rapporto & ", " & Cbl_Rapporto_Contabile.Items(j).Value
                    End If
                Next
                'rimuovo la ,
                If x_Cod_Rapporto.Length > 0 Then
                    x_Cod_Rapporto = Right(x_Cod_Rapporto, x_Cod_Rapporto.Length - 1)
                End If

            End If
            If x_Cod_Rapporto <> "" Then
                x_Filtro_CodRapporto = " AND Risorse_Umane.Cod_Rapporto in (" & x_Cod_Rapporto & ")"
            End If


            ''------------------------------
            ''------ PERIODO TEMP ----------
            ''------------------------------

            ''INTERVALLO

            'If Me.Txt_ValiditaInizio.Text = "" Then
            x_Data_Inizio = AGRODATAINIZIO.ToShortDateString
            'Else
            '    x_Data_Inizio = Me.Txt_ValiditaInizio.Text
            'End If

            'If Me.Txt_ValiditaFine.Text = "" Then
            x_Data_Fine = AGRODATAFINE.ToShortDateString
            'Else
            '    x_Data_Fine = Me.Txt_ValiditaFine.Text
            'End If


            '------------------------------
            '----- TIPOLOGIA CONTATTO -----
            '------------------------------
            If viewstate("FlagAnagraficaContatti") = False Then
                x_ID_CF = Me.Rbl_Tipologia.SelectedValue
            Else
                x_ID_CF = Rbl_Tipologia_contatti.SelectedValue
            End If

            If x_ID_CF <> 2 Then
                x_Filtro_SaCod = " And Contatti.Id_Cf = " & x_ID_CF
            End If


            '==========================================
            '  FILTRO VISIBILITA' CENTRI AZIENDALI
            '==========================================
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            Dim filtro As String
            If ViewState("FlagAnagraficaContatti") = False Then
                filtro = " Piva='" & x_Piva & "'"
            Else
                filtro = " Piva IN (" + CStr(ViewState("ElencoChiaviImpresa")) + ")"
            End If
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, filtro, "", objParametri_Server)
            If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                FiltroCentri &= "AND ( "
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= "( Contatti.Piva = '" & Agro_SQL_SaveText(DtCentriVisibili.Rows(i).Item("piva")) & "' AND Contatti.Sa_cod = " & Agro_SQL_SaveNum(DtCentriVisibili.Rows(i).Item("sa_cod")) & " ) "
                    If i <> DtCentriVisibili.Rows.Count - 1 Then
                        FiltroCentri &= " OR "
                    End If
                Next
                FiltroCentri &= " ) "
            End If

            '--------------------------------
            '----- VISIBILITA' CONTATTO -----
            '--------------------------------
            If ViewState("FlagAnagraficaContatti") = False Then

                '<asp:ListItem Value="1">Solo i Contatti dell'Impresa selezionata</asp:ListItem>
                '<asp:ListItem Value="2">Contatti dell'Impresa e Contatti Pubblici</asp:ListItem>
                '<asp:ListItem Value="3" Selected="True">Tutti i Contatti</asp:ListItem>

                If FiltroCentri = "" Then

                    Select Case Me.Rbl_Visibilita.SelectedValue
                        'Case 0 'SOLO CONTATTI PRIVATI DELL'IMPRESA -> è stato disattivato
                        '    x_Filtro_SaCod = x_Filtro_SaCod & " AND ( Contatti.Piva = '" + Agro_SQL_SaveText(x_Piva) + "' AND Contatti.Sa_Cod = 0)"
                        Case 1 'TUTTI I CONTATTI DELL'IMPRESA
                            x_Filtro_SaCod &= " AND ( Contatti.Piva = '" + Agro_SQL_SaveText(x_Piva) + "' )"
                        Case 2 'TUTTI I CONTATTI DELL'IMPRESA E I CONTATTI PUBBLICI
                            x_Filtro_SaCod &= " AND ( Contatti.Piva = '" + Agro_SQL_SaveText(x_Piva) + "' OR Contatti.Sa_Cod = -1)"
                        Case 3 'TUTTI I CONTATTI
                            x_Filtro_SaCod &= " "
                    End Select

                Else

                    'c'è filtro di visibilità sui centri

                    Select Case Me.Rbl_Visibilita.SelectedValue

                        'Case 0 'SOLO CONTATTI PRIVATI DELL'IMPRESA -> è stato disattivato
                        '    x_Filtro_SaCod = x_Filtro_SaCod & " AND ( Contatti.Piva = '" + Agro_SQL_SaveText(x_Piva) + "' AND Contatti.Sa_Cod = 0)"

                        Case 1 'TUTTI I CONTATTI DELL'IMPRESA
                            x_Filtro_SaCod &= " AND Contatti.Piva = '" + Agro_SQL_SaveText(x_Piva) + "'  " & FiltroCentri

                        Case 2 'TUTTI I CONTATTI DELL'IMPRESA E I CONTATTI PUBBLICI
                            x_Filtro_SaCod &= " AND (   ( Contatti.Piva = '" + Agro_SQL_SaveText(x_Piva) + "'  " & _
                                                            FiltroCentri & " ) OR Contatti.Sa_Cod = -1 ) "

                        Case 3 'TUTTI I CONTATTI
                            x_Filtro_SaCod &= FiltroCentri

                    End Select

                End If 'visibilità centri

            Else
                If FiltroCentri = "" Then
                    Select Case Me.Rbl_Visibilita_Export_Contatti.SelectedValue
                        Case 1 'TUTTI I CONTATTI DELL'IMPRESA
                            x_Filtro_SaCod &= " AND ( Contatti.Piva IN (" + CStr(ViewState("ElencoChiaviImpresa")) + " ) ) "
                        Case 2 'TUTTI I CONTATTI DELL'IMPRESA E I CONTATTI PUBBLICI
                            x_Filtro_SaCod &= " AND ( Contatti.Piva IN (" + CStr(ViewState("ElencoChiaviImpresa")) + ") OR Contatti.Sa_Cod = -1)"
                        Case 3 'TUTTI I CONTATTI
                    End Select
                Else
                    'c'è filtro di visibilità sui centri
                    Select Case Me.Rbl_Visibilita_Export_Contatti.SelectedValue
                        Case 1 'TUTTI I CONTATTI DELL'IMPRESA
                            x_Filtro_SaCod &= " AND  Contatti.Piva IN (" + CStr(ViewState("ElencoChiaviImpresa")) + " )  " & FiltroCentri
                        Case 2 'TUTTI I CONTATTI DELL'IMPRESA E I CONTATTI PUBBLICI
                            x_Filtro_SaCod &= " AND ( ( Contatti.Piva IN (" + CStr(ViewState("ElencoChiaviImpresa")) + ") " & _
                                                 FiltroCentri & " ) " & _
                                                        " OR Contatti.Sa_Cod = -1 ) "
                        Case 3 'TUTTI I CONTATTI
                            x_Filtro_SaCod &= FiltroCentri
                    End Select

                End If 'visibilità centri

            End If 'tipo stampa
            '============================================================

            Dim TargetURL, QueryString, Pagina_Titolo As String

            Select Case Report

                Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori, _
                        enum_CodificaStampe.PacchettoIgiene_RegistroClienti


                    Pagina_Titolo = "PacchettoIgiene_RegistroClientiFornitori"
                    TargetURL = "../PacchettoIgiene/Registro_ClientiFornitori/Registro_ClientiFornitori.aspx"

                    QueryString = "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                                    "&di=" & _
                                    Stringa_Codifica(x_Data_Inizio, AgroKey_EncoderDecoder, Server) & _
                                    "&df=" & _
                                    Stringa_Codifica(x_Data_Fine, AgroKey_EncoderDecoder, Server) & _
                                    "&idcf=" & _
                                     Stringa_Codifica(x_ID_CF, AgroKey_EncoderDecoder, Server) & _
                                    "&fil=" & _
                                     Stringa_Codifica(x_Filtro_SaCod, AgroKey_EncoderDecoder, Server)

                    '#########################################################################

                Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                    '==========================================
                    '         CONTROLLO SELEZIONE CHECK
                    '==========================================

                    Dim Flag_AlmenoUno As Boolean

                    Flag_AlmenoUno = Controllo_Selezione_Check()

                    If Flag_AlmenoUno = False Then
                        AgroMsgBox("Non è stato selezionato alcun campo da esportare.", Page)
                        Exit Sub
                    End If


                    '==========================================
                    '            RECUPERO DEI DATI
                    '==========================================

                    Dim DT_Contatti As DataTable
                    Dim i As Integer
                    Dim str_indirizzo, comune, sigla_prov As String

                    DT_Contatti = Esegui_Query_Esportazione_Contatti()

                    If Not IsNothing(DT_Contatti) AndAlso DT_Contatti.Rows.Count <> 0 Then

                        For i = 0 To DT_Contatti.Rows.Count - 1

                            If DT_Contatti.Columns.Contains("Tipo_Indirizzo_Default") = True Then
                                ''PF
                                'Domicilio = 2
                                'Residenza = 3
                                'ResidenzaEstiva = 4
                                'LuogoDiNascita = 5
                                ''PG
                                'SedeOperativa = 1
                                'SedeLegale = 101
                                'SedeAziendale = 102
                                'Stabilimento = 103
                                'StabileOrganizzazione = 201
                                Select Case DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default")
                                    Case 0
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = ""
                                    Case 2
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Domicilio"
                                    Case 3
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Residenza"
                                    Case 4
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Residenza Estiva"
                                    Case 5
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Luogo Di Nascita"
                                    Case 1
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Sede Operativa"
                                    Case 101
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Sede Legale"
                                    Case 102
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Sede Aziendale"
                                    Case 103
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Stabilimento"
                                    Case 201
                                        DT_Contatti.Rows(i).Item("Tipo_Indirizzo_Default") = "Stabile Organizzazione"
                                    Case Else
                                        'non faccio nulla
                                End Select

                            End If

                            If DT_Contatti.Columns.Contains("Indirizzo_1") = True Then
                                str_indirizzo = DT_Contatti.Rows(i).Item("Indirizzo_1")
                                If InStr(str_indirizzo, "|") > 0 Then
                                    DT_Contatti.Rows(i).Item("ind_des_1") = str_indirizzo.Split("|")(0)
                                    DT_Contatti.Rows(i).Item("frz_des_1") = str_indirizzo.Split("|")(1)
                                    DT_Contatti.Rows(i).Item("cap_1") = str_indirizzo.Split("|")(2)

                                    comune = str_indirizzo.Split("|")(3)
                                    sigla_prov = str_indirizzo.Split("|")(4)
                                    Sistema_Comune_Provincia(comune, sigla_prov)
                                    DT_Contatti.Rows(i).Item("comune_1") = comune
                                    DT_Contatti.Rows(i).Item("sigla_prov_1") = sigla_prov

                                    DT_Contatti.Rows(i).Item("stato_1") = str_indirizzo.Split("|")(5)
                                End If
                            End If

                            If DT_Contatti.Columns.Contains("Indirizzo_2") = True Then
                                str_indirizzo = DT_Contatti.Rows(i).Item("Indirizzo_2")
                                If InStr(str_indirizzo, "|") > 0 Then
                                    DT_Contatti.Rows(i).Item("ind_des_2") = str_indirizzo.Split("|")(0)
                                    DT_Contatti.Rows(i).Item("frz_des_2") = str_indirizzo.Split("|")(1)
                                    DT_Contatti.Rows(i).Item("cap_2") = str_indirizzo.Split("|")(2)

                                    comune = str_indirizzo.Split("|")(3)
                                    sigla_prov = str_indirizzo.Split("|")(4)
                                    Sistema_Comune_Provincia(comune, sigla_prov)
                                    DT_Contatti.Rows(i).Item("comune_2") = comune
                                    DT_Contatti.Rows(i).Item("sigla_prov_2") = sigla_prov

                                    DT_Contatti.Rows(i).Item("stato_2") = str_indirizzo.Split("|")(5)
                                End If
                            End If

                            If DT_Contatti.Columns.Contains("Indirizzo_3") = True Then
                                str_indirizzo = DT_Contatti.Rows(i).Item("Indirizzo_3")
                                If InStr(str_indirizzo, "|") > 0 Then
                                    DT_Contatti.Rows(i).Item("ind_des_3") = str_indirizzo.Split("|")(0)
                                    DT_Contatti.Rows(i).Item("frz_des_3") = str_indirizzo.Split("|")(1)
                                    DT_Contatti.Rows(i).Item("cap_3") = str_indirizzo.Split("|")(2)

                                    comune = str_indirizzo.Split("|")(3)
                                    sigla_prov = str_indirizzo.Split("|")(4)
                                    Sistema_Comune_Provincia(comune, sigla_prov)
                                    DT_Contatti.Rows(i).Item("comune_3") = comune
                                    DT_Contatti.Rows(i).Item("sigla_prov_3") = sigla_prov

                                    DT_Contatti.Rows(i).Item("stato_3") = str_indirizzo.Split("|")(5)
                                End If
                            End If

                            If DT_Contatti.Columns.Contains("Indirizzo_4") = True Then
                                str_indirizzo = DT_Contatti.Rows(i).Item("Indirizzo_4")
                                If InStr(str_indirizzo, "|") > 0 Then
                                    DT_Contatti.Rows(i).Item("ind_des_4") = str_indirizzo.Split("|")(0)
                                    DT_Contatti.Rows(i).Item("frz_des_4") = str_indirizzo.Split("|")(1)
                                    DT_Contatti.Rows(i).Item("cap_4") = str_indirizzo.Split("|")(2)

                                    comune = str_indirizzo.Split("|")(3)
                                    sigla_prov = str_indirizzo.Split("|")(4)
                                    Sistema_Comune_Provincia(comune, sigla_prov)
                                    DT_Contatti.Rows(i).Item("comune_4") = comune
                                    DT_Contatti.Rows(i).Item("sigla_prov_4") = sigla_prov

                                    DT_Contatti.Rows(i).Item("stato_4") = str_indirizzo.Split("|")(5)
                                End If
                            End If

                        Next

                        If Me.ChkGruppo1.Items.FindByValue(0).Selected = False Then
                            DT_Contatti.Columns.Remove("Piva")
                        End If
                        If Me.ChkGruppo2.Items.FindByValue(18).Selected = True Then
                            DT_Contatti.Columns.Remove("Indirizzo_1")
                            DT_Contatti.Columns.Remove("Indirizzo_2")
                            DT_Contatti.Columns.Remove("Indirizzo_3")
                            DT_Contatti.Columns.Remove("Indirizzo_4")
                        End If

                        Session("DT_Finale") = DT_Contatti

                        Pagina_Titolo = "Esportazione_Contatti"
                        TargetURL = "../../GestioneEsportazioni/Esportazione_AnagraficaContatti/AnagraficaContatti_XLS.aspx"
                        QueryString = ""

                    Else
                        AgroMsgBox("Non è presente alcun contatto da esportare.", Page)
                        Exit Sub
                    End If

                    '#########################################################################

                Case Else



            End Select


            Page_NewWindow(Page, TargetURL, QueryString, Pagina_Titolo, , , , , , , , )


            '##############################################################
            '##############################################################

        Catch ex As Exception
            AgroMsgBox("Stampa: " + vbCrLf + ex.Message, Page)
        End Try



    End Sub


    '###################################################################################
    Private Function Controllo_Selezione_Check() As Boolean

        '==========================================
        '         CONTROLLO SELEZIONE CHECK
        '==========================================

        Dim Flag_AlmenoUno As Boolean = False
        Dim i As Integer

        For i = 0 To 10
            If Me.ChkGruppo1.Items.FindByValue(i).Selected = True Then
                Flag_AlmenoUno = True
            End If
        Next

        If Flag_AlmenoUno = False Then
            For i = 10 To 19
                Select Case i
                    Case 15, 16, 17
                        'disattivati
                    Case Else
                        If Me.ChkGruppo2.Items.FindByValue(i).Selected = True Then
                            Flag_AlmenoUno = True
                        End If
                End Select
            Next

            If Flag_AlmenoUno = False Then
                For i = 20 To 32
                    Select Case i
                        Case 24, 25, 26, 28, 29, 23, 27
                        Case Else
                            If Me.ChkGruppo3.Items.FindByValue(i).Selected = True Then
                                Flag_AlmenoUno = True
                            End If
                    End Select
                Next
            End If

        End If

        Return Flag_AlmenoUno

    End Function




    '###############################################################################
    Public Function Esegui_Query_Esportazione_Contatti() As DataTable


        Dim objSQL As New Codex_Utility.Sql
        Dim Str_SQL As New System.Text.StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable


        '############################################################
        '################## PREPARAZIONE QUERY  #####################
        '############################################################

        Str_SQL = Prepara_StringaQuery_Contatti()


        '----------------------------------------------------
        '--- Recupero il datatable --------------------------
        '----------------------------------------------------
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        'Recupero il datatable
        DT = objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
                        CStr(Session("ASG_Connessione_Server")), _
                        Str_SQL.ToString, 1, MessaggioErrore)

        'Elimino l'oggetto
        objSQL.SqlDispose()

        'Verifico la presenza di errori
        If Not IsNothing(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
            Return Nothing
        Else
            Return DT
        End If


    End Function


    '###############################################################################
    Public Function Prepara_StringaQuery_Contatti() As System.Text.StringBuilder

        '==========================================
        '         PREPARAZIONE FILTRO QUERY
        '==========================================

        Dim stbQuery As New System.Text.StringBuilder
        stbQuery.Length = 0

        Try

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------

            stbQuery.Append(" SELECT  CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Contatti.Piva ELSE Imprese.partitaIvaReale END AS Piva   " + vbCrLf)

            'IMPRESA
            If Me.ChkGruppo1.Items.FindByValue(1).Selected = True Then
                stbQuery.Append(" , Imprese.Rag_Soc AS Impresa " + vbCrLf)
            End If

            'CODICE CONTATTO
            If Me.ChkGruppo1.Items.FindByValue(2).Selected = True Then
                stbQuery.Append(" , Contatti.Cod_Contatto , Contatti.Codice_Fiscale " + vbCrLf)
            End If

            'RAGIONE SOCIALE
            If Me.ChkGruppo1.Items.FindByValue(3).Selected = True Then
                'stbQuery.Append(" , Contatti.Rag_Soc " + vbCrLf)
                stbQuery.Append(" , CASE Contatti.Rag_Soc " + vbCrLf)
                stbQuery.Append(" WHEN ''      THEN Contatti.Cognome + ' ' + Contatti.Nome " + vbCrLf)
                stbQuery.Append(" Else Contatti.Rag_Soc" + vbCrLf)
                stbQuery.Append(" END as Rag_Soc " + vbCrLf)
            End If

            'VISIBILITA
            If Me.ChkGruppo1.Items.FindByValue(4).Selected = True Then
                stbQuery.Append(" , Contatti.Sa_Cod " + vbCrLf)
            End If

            'TIPOLOGIA
            If Me.ChkGruppo1.Items.FindByValue(5).Selected = True Then
                stbQuery.Append(" , Contatti.Id_CF " + vbCrLf)
            End If

            'CONVENEVOLI
            If Me.ChkGruppo1.Items.FindByValue(6).Selected = True Then
                stbQuery.Append(" , Contatti.Convenevoli " + vbCrLf)
            End If

            'TIPO INDIRIZZO DEFAULT
            If Me.ChkGruppo1.Items.FindByValue(7).Selected = True Then
                stbQuery.Append(" , CONVERT(varchar(150), Contatti.Tipo_Indirizzo_Default) as Tipo_Indirizzo_Default " + vbCrLf)
            End If

            'TIPO SCONTO DI DEFAULT
            If Me.ChkGruppo1.Items.FindByValue(8).Selected = True Then
                'stbQuery.Append(" , TipoSconto_Default " + vbCrLf)
            End If

            'CODICE GIAS
            If Me.ChkGruppo1.Items.FindByValue(9).Selected = True Then
                stbQuery.Append(" , Risorse_Umane.Cod_RisUm " + vbCrLf)
            End If

            'CODICE GIAS
            If Me.ChkGruppo1.Items.FindByValue(10).Selected = True Then
                stbQuery.Append(" , CASE WHEN ChkSpesometro = 1 THEN 'SI' ELSE 'NO' END AS chkspesometro " + vbCrLf)
            End If

            '-----

            'PROGRESSIVO
            If Me.ChkGruppo2.Items.FindByValue(10).Selected = True Then
                stbQuery.Append(" , Risorse_Umane.Settore_Des " + vbCrLf)
            End If

            'ATTIVITA
            If Me.ChkGruppo2.Items.FindByValue(11).Selected = True Then
                stbQuery.Append(" , Risorse_Umane.Attivita_des " + vbCrLf)
            End If

            'VALIDITA INIZIO E FINE
            If Me.ChkGruppo2.Items.FindByValue(12).Selected = True Then
                stbQuery.Append(" , Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine " + vbCrLf)
            End If

            'RAPPORTO CONTABILE
            If Me.ChkGruppo2.Items.FindByValue(13).Selected = True Then
                stbQuery.Append("  , Rapporti_Contabili.Rapporto_Des " & vbCrLf)
            End If

            'NUMERO PATENTINO
            If Me.ChkGruppo2.Items.FindByValue(14).Selected = True Then
                stbQuery.Append(" , ISNULL ( (  " & vbCrLf)
                stbQuery.Append("           SELECT TOP 1 'Numero: ' + allegati_documenti.Allegati_Documenti_Numero + ' - Data Rilascio: ' + CONVERT(varchar(20),Allegati_Documenti.validazione_data,103) + ' - Data Scadenza: ' + CONVERT(varchar(20),Alert_Elenco.Data_Scadenza,103)  " & vbCrLf)
                stbQuery.Append("           FROM alert_entita  " & vbCrLf)
                stbQuery.Append("           INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita  " & vbCrLf)
                stbQuery.Append("           INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod   " & vbCrLf)
                stbQuery.Append("           WHERE tipoentita_cod=8 " & vbCrLf)
                stbQuery.Append("           AND Allegati_Documenti.Allegati_documenti_CatCod=2  " & vbCrLf)
                stbQuery.Append("           AND alert_entita.Cod_Contatto=Contatti.Cod_Contatto and alert_entita.piva = Contatti.piva  " & vbCrLf)
                stbQuery.Append("           ORDER BY Alert_Elenco.Data_Scadenza DESC " & vbCrLf)
                stbQuery.Append("  ) , '') AS Patentino  " & vbCrLf)
                '--  AND Allegati_Documenti.Validazione_Data <= " & Agro_SQL_SaveDate(Data_Fine) 
                '--  AND Alert_Elenco.Data_Scadenza >= " & Agro_SQL_SaveDate(Data_Inizio) 
            End If

            ''VALIDITA INIZIO E FINE PATENTINO
            'If Me.ChkGruppo2.Items.FindByValue(15).Selected = True Then
            '    stbQuery.Append(" , Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino " + vbCrLf)
            'End If

            ''CORRISPETTIVO
            ''Mezzo per le Materie Prime
            ''Indefinito = -1      
            ''Ettolitro = 0
            ''Ettaro = 1
            ''Ora = 2
            ''Mensile = 3
            ''Complessivo = 4
            ''ci va una query interna
            'If Me.ChkGruppo2.Items.FindByValue(16).Selected = True Then
            '    'stbQuery.Append(" , ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo, ISNULL(Prodotti_Costi.Udm_Cod, 0) AS UdmCod_ProdCosti, ISNULL(UnitaMisura_ProdCosti.UDM_DES, '') AS UdmDes_ProdCosti, ISNULL(UnitaMisura_ProdCosti.UDM_SIM, '') AS UdmSim_ProdCosti, " & vbCrLf)
            '    'stbQuery.Append("   ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Validita_Inizio_ProdCosti, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Validita_Fine_ProdCosti  " & vbCrLf)
            '    'If Me.ChkGruppo2.Items.FindByValue(16).Selected = True Then
            '    '    stbQuery.Append("AND Prodotti_Costi.Elem_Cod = 0 ")
            '    'End If
            'End If

            ''ORE SETTIMANALI, GIORNI FERIE, FERIE GODUTE, GIORNI MALATTIA
            'If Me.ChkGruppo2.Items.FindByValue(17).Selected = True Then
            '    stbQuery.Append("  , Risorse_Umane.Ore_Settimanali, Risorse_Umane.Giorni_Ferie, Risorse_Umane.Ferie_Godute, Risorse_Umane.Giorni_Malattia " & vbCrLf)
            'End If

            'INDIRIZZI
            If Me.ChkGruppo2.Items.FindByValue(18).Selected = True Then

                stbQuery.Append(", ISNULL( (SELECT TOP 1 Indirizzi.ind_des + '|' + Indirizzi.frz_des + '|' + Indirizzi.CAP + '|' + ISNULL(ISTAT.LOCALITA, '') + '|'  + ISNULL(ISTAT.COMUNI_PROV, '') + '|'  + Indirizzi.Stato " + vbCrLf)
                stbQuery.Append("           FROM ContattiXIndirizzi   " & vbCrLf)
                stbQuery.Append("           INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " + vbCrLf)
                stbQuery.Append("           LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " + vbCrLf)
                stbQuery.Append("           WHERE (ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_RESIDENZA) + " " & vbCrLf)
                stbQuery.Append("           OR ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_SEDE_OPERATIVA) + ") " & vbCrLf)
                stbQuery.Append("           AND Contatti.Piva = ContattiXIndirizzi.Piva  " & vbCrLf)
                stbQuery.Append("           AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                stbQuery.Append("           ), '') AS Indirizzo_1 " & vbCrLf)

                stbQuery.Append("           ,'' AS ind_des_1, '' AS frz_des_1, '' AS cap_1, '' AS comune_1, '' AS sigla_prov_1, '' AS stato_1   " & vbCrLf)

                stbQuery.Append(", ISNULL( (SELECT TOP 1 Indirizzi.ind_des + '|' + Indirizzi.frz_des + '|' + Indirizzi.CAP + '|' + ISNULL(ISTAT.LOCALITA, '') + '|' + ISNULL(ISTAT.COMUNI_PROV, '') + '|'  + Indirizzi.Stato " + vbCrLf)
                stbQuery.Append("           FROM ContattiXIndirizzi   " & vbCrLf)
                stbQuery.Append("           INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " + vbCrLf)
                stbQuery.Append("           LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " + vbCrLf)
                stbQuery.Append("           WHERE (ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_LUOGO_NASCITA) + "" & vbCrLf)
                stbQuery.Append("           OR ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_SEDE_LEGALE) + ") " & vbCrLf)
                stbQuery.Append("           AND Contatti.Piva = ContattiXIndirizzi.Piva  " & vbCrLf)
                stbQuery.Append("           AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                stbQuery.Append("           ), '') AS Indirizzo_2 " & vbCrLf)

                stbQuery.Append("           ,'' AS ind_des_2, '' AS frz_des_2, '' AS cap_2, '' AS comune_2, '' AS sigla_prov_2, '' AS stato_2   " & vbCrLf)

                stbQuery.Append(", ISNULL( (SELECT TOP 1 Indirizzi.ind_des + '|' + Indirizzi.frz_des + '|' + Indirizzi.CAP + '|' + ISNULL(ISTAT.LOCALITA, '') + '|' + ISNULL(ISTAT.COMUNI_PROV, '') + '|'  + Indirizzi.Stato " + vbCrLf)
                stbQuery.Append("           FROM ContattiXIndirizzi   " & vbCrLf)
                stbQuery.Append("           INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " + vbCrLf)
                stbQuery.Append("           LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " + vbCrLf)
                stbQuery.Append("           WHERE (ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_DOMICILIO) + "" & vbCrLf)
                stbQuery.Append("           OR ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_SEDE_AZIENDALE) + ") " & vbCrLf)
                stbQuery.Append("           AND Contatti.Piva = ContattiXIndirizzi.Piva  " & vbCrLf)
                stbQuery.Append("           AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                stbQuery.Append("           ), '') AS Indirizzo_3 " & vbCrLf)

                stbQuery.Append("           ,'' AS ind_des_3, '' AS frz_des_3, '' AS cap_3, '' AS comune_3, '' AS sigla_prov_3, '' AS stato_3   " & vbCrLf)

                stbQuery.Append(", ISNULL( (SELECT TOP 1 Indirizzi.ind_des + '|' + Indirizzi.frz_des + '|' + Indirizzi.CAP + '|' + ISNULL(ISTAT.LOCALITA, '') + '|' +  ISNULL(ISTAT.COMUNI_PROV, '') + '|'  + Indirizzi.Stato " + vbCrLf)
                stbQuery.Append("           FROM ContattiXIndirizzi   " & vbCrLf)
                stbQuery.Append("           INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " + vbCrLf)
                stbQuery.Append("           LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " + vbCrLf)
                stbQuery.Append("           WHERE (ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_RESIDENZA_ESTIVA) + "" & vbCrLf)
                stbQuery.Append("           OR ContattiXIndirizzi.Tipo_Indirizzo =  " + CStr(INDIRIZZO_STABILIMENTO) + ") " & vbCrLf)
                stbQuery.Append("           AND Contatti.Piva = ContattiXIndirizzi.Piva  " & vbCrLf)
                stbQuery.Append("           AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                stbQuery.Append("           ), '') AS Indirizzo_4 " & vbCrLf)

                stbQuery.Append("           ,'' AS ind_des_4, '' AS frz_des_4, '' AS cap_4, '' AS comune_4, '' AS sigla_prov_4, '' AS stato_4   " & vbCrLf)

            End If

            'TELEFONO
            If Me.ChkGruppo2.Items.FindByValue(19).Selected = True Then
                stbQuery.Append(" , ISNULL( " + vbCrLf)
                stbQuery.Append("           (SELECT TOP 1 Rubrica.numero " + vbCrLf)
                stbQuery.Append(" 	        FROM ContattiXRubrica " + vbCrLf)
                stbQuery.Append("   	    INNER JOIN Rubrica ON ContattiXRubrica.cod_rubrica = Rubrica.cod_rubrica " + vbCrLf)
                stbQuery.Append("    	    WHERE	contatti.Piva = ContattiXRubrica.Piva AND contatti.Cod_Contatto = ContattiXRubrica.Cod_Contatto " + vbCrLf)
                stbQuery.Append("           AND UPPER(Rubrica.descr) LIKE '%TEL%' " + vbCrLf)
                stbQuery.Append("           ), ' ' ) as Telefono " + vbCrLf)
            End If

            '-----

            'FAX
            If Me.ChkGruppo3.Items.FindByValue(20).Selected = True Then
                stbQuery.Append(" , ISNULL( " + vbCrLf)
                stbQuery.Append("           (SELECT TOP 1 Rubrica.numero " + vbCrLf)
                stbQuery.Append(" 	        FROM ContattiXRubrica " + vbCrLf)
                stbQuery.Append("   	    INNER JOIN Rubrica ON ContattiXRubrica.cod_rubrica = Rubrica.cod_rubrica " + vbCrLf)
                stbQuery.Append("    	    WHERE	contatti.Piva = ContattiXRubrica.Piva AND contatti.Cod_Contatto = ContattiXRubrica.Cod_Contatto " + vbCrLf)
                stbQuery.Append("           AND (UPPER(Rubrica.descr) LIKE '%FAX%') " + vbCrLf)
                stbQuery.Append("           ), ' ' ) as Fax " + vbCrLf)
            End If

            'EMAIL
            If Me.ChkGruppo3.Items.FindByValue(21).Selected = True Then
                stbQuery.Append(" , ISNULL( " + vbCrLf)
                stbQuery.Append("           (SELECT TOP 1 Rubrica.numero " + vbCrLf)
                stbQuery.Append(" 	        FROM ContattiXRubrica " + vbCrLf)
                stbQuery.Append("   	    INNER JOIN Rubrica ON ContattiXRubrica.cod_rubrica = Rubrica.cod_rubrica " + vbCrLf)
                stbQuery.Append("    	    WHERE	contatti.Piva = ContattiXRubrica.Piva AND contatti.Cod_Contatto = ContattiXRubrica.Cod_Contatto " + vbCrLf)
                stbQuery.Append("           AND (UPPER(Rubrica.descr) LIKE '%MAIL%') " + vbCrLf)
                stbQuery.Append("           ), ' ' ) as Email " + vbCrLf)
            End If

            'CELLULARE
            If Me.ChkGruppo3.Items.FindByValue(22).Selected = True Then
                stbQuery.Append(" , ISNULL( " + vbCrLf)
                stbQuery.Append("           (SELECT TOP 1 Rubrica.numero " + vbCrLf)
                stbQuery.Append(" 	        FROM ContattiXRubrica " + vbCrLf)
                stbQuery.Append("   	    INNER JOIN Rubrica ON ContattiXRubrica.cod_rubrica = Rubrica.cod_rubrica " + vbCrLf)
                stbQuery.Append("    	    WHERE	contatti.Piva = ContattiXRubrica.Piva AND contatti.Cod_Contatto = ContattiXRubrica.Cod_Contatto " + vbCrLf)
                stbQuery.Append("           AND (UPPER(Rubrica.descr) LIKE '%CELL%') " + vbCrLf)
                stbQuery.Append("           ), ' ' ) as Cellulare " + vbCrLf)
            End If

            'LISTINO DI DEFAULT
            If Me.ChkGruppo3.Items.FindByValue(30).Selected = True Then
                stbQuery.Append(" , ISNULL(Listini_Vendita_Contatto_Def.Listino_Des, '') as ListinoVenditaDefault" + vbCrLf)
            End If

            'PEC
            If Me.ChkGruppo3.Items.FindByValue(31).Selected = True Then
                stbQuery.Append(" , ISNULL( " + vbCrLf)
                stbQuery.Append("           (SELECT TOP 1 val_cod " + vbCrLf)
                stbQuery.Append(" 	        FROM    Contatti_Codici " + vbCrLf)
                stbQuery.Append("    	    WHERE	contatti.Piva = Contatti_Codici.Piva AND contatti.Cod_Contatto = Contatti_Codici.Cod_Contatto " + vbCrLf)
                stbQuery.Append("           AND id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.PEC) & "" + vbCrLf)
                stbQuery.Append("           ), ' ' ) AS PEC " + vbCrLf)
            End If

            'SDI
            If Me.ChkGruppo3.Items.FindByValue(32).Selected = True Then
                stbQuery.Append(" , ISNULL( " + vbCrLf)
                stbQuery.Append("           (SELECT TOP 1 val_cod " + vbCrLf)
                stbQuery.Append(" 	        FROM    Contatti_Codici " + vbCrLf)
                stbQuery.Append("    	    WHERE	contatti.Piva = Contatti_Codici.Piva AND contatti.Cod_Contatto = Contatti_Codici.Cod_Contatto " + vbCrLf)
                stbQuery.Append("           AND id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.SDI) & "" + vbCrLf)
                stbQuery.Append("           ), ' ' ) AS SDI " + vbCrLf)
            End If

            ''PERSONA REFERENTE
            'If Me.ChkGruppo3.Items.FindByValue(23).Selected = True Then
            '    'stbQuery.Append("  , '' AS Referente " & vbCrLf)
            'End If

            ''ISTITUTI CREDITO
            'If Me.ChkGruppo3.Items.FindByValue(24).Selected = True Then
            '    'stbQuery.Append("  , '' AS Istituto_Credito " & vbCrLf)
            'End If

            ''COORDINATE IBAN
            'If Me.ChkGruppo3.Items.FindByValue(25).Selected = True Then
            '    'stbQuery.Append("  , '' AS Coordinate IBAN " & vbCrLf)
            'End If

            ''DATA APERTURA E CHIUSURA CONTO
            'If Me.ChkGruppo3.Items.FindByValue(26).Selected = True Then
            '    'stbQuery.Append("  , '' AS Apertura_Conto, '' AS Chiusura_Conto " & vbCrLf)
            'End If

            ''TIPOLOGIA PRODOTTO ACQUISTATO VENDUTO
            'If Me.ChkGruppo3.Items.FindByValue(27).Selected = True Then
            '    'stbQuery.Append("  , '' AS Tipo_Prodotto " & vbCrLf)
            'End If

            ''CONTI ECONOMICI
            'If Me.ChkGruppo3.Items.FindByValue(28).Selected = True Then
            '    'stbQuery.Append("  , '' AS Conti_Economici " & vbCrLf)
            'End If

            ''CODICE GIAS PRE IMPORTAZIONE E IMPRESA DA CUI E' IMPORTATO
            'If Me.ChkGruppo3.Items.FindByValue(29).Selected = True Then
            '    'stbQuery.Append("  , '' AS  " & vbCrLf)
            'End If


            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------

            stbQuery.Append(" FROM Contatti " + vbCrLf)


            '------------------------------------------------------ 
            '-------------------- JOIN ----------------------------
            '------------------------------------------------------

            'IMPRESA
            If Me.ChkGruppo1.Items.FindByValue(0).Selected = True Or Me.ChkGruppo1.Items.FindByValue(1).Selected = True Then
                stbQuery.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " + vbCrLf)
            End If

            'DATI RISORSE UMANE
            If Me.ChkGruppo1.Items.FindByValue(9).Selected = True Or _
                Me.ChkGruppo1.Items.FindByValue(10).Selected = True Or _
                Me.ChkGruppo2.Items.FindByValue(10).Selected = True Or _
                Me.ChkGruppo2.Items.FindByValue(11).Selected = True Or _
                Me.ChkGruppo2.Items.FindByValue(12).Selected = True Or _
                Me.ChkGruppo2.Items.FindByValue(13).Selected = True Or _
                x_Filtro_CodRapporto <> "" Then
                stbQuery.Append(" INNER JOIN Risorse_Umane ON Risorse_Umane.Piva = Contatti.Piva " + vbCrLf)
                stbQuery.Append(" AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto " + vbCrLf)
            End If

            stbQuery.Append(" INNER  JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA " + vbCrLf)

            'RAPPORTO CONTABILE
            If Me.ChkGruppo2.Items.FindByValue(13).Selected = True Then
                stbQuery.Append(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " + vbCrLf)
                stbQuery.Append(" AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] " + vbCrLf)
            End If

            ''CORRISPETTIVO
            'If Me.ChkGruppo2.Items.FindByValue(16).Selected = True Then
            '    stbQuery.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Piva = Prodotti_Costi.Piva AND Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod " & vbCrLf)
            '    stbQuery.Append(" LEFT OUTER JOIN UnitaMisura UnitaMisura_ProdCosti ON UnitaMisura_ProdCosti.UDM_COD = Prodotti_Costi.Udm_Cod  " & vbCrLf)
            'End If
            If Me.ChkGruppo3.Items.FindByValue(30).Selected = True Then
                stbQuery.Append(" LEFT JOIN " + vbCrLf)
                stbQuery.Append("           (SELECT Listini_Prezzi.Listino_Des, Contatti_Codici.Piva, Contatti_Codici.Cod_Contatto " + vbCrLf)
                stbQuery.Append(" 	        FROM    Contatti_Codici " + vbCrLf)
                stbQuery.Append("   	    INNER JOIN Listini_Prezzi ON Listini_Prezzi.Listino_Cod = Contatti_Codici.val_cod " + vbCrLf)
                stbQuery.Append("    	    WHERE Contatti_Codici.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.ListinoPrezziVenditaDefault) & "" + vbCrLf)
                stbQuery.Append("           ) AS Listini_Vendita_Contatto_Def " + vbCrLf)
                stbQuery.Append(" ON Contatti.Piva = Listini_Vendita_Contatto_Def.Piva AND Contatti.Cod_Contatto = Listini_Vendita_Contatto_Def.Cod_Contatto" + vbCrLf)
            End If


            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------

            stbQuery.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) & "'" + vbCrLf)

            stbQuery.Append(Agro_SQL_Save_xFiltroAggiuntivo(x_Filtro_SaCod, objParametri_Server) + vbCrLf)

            'cod rapp
            stbQuery.Append(Agro_SQL_Save_xFiltroAggiuntivo(x_Filtro_CodRapporto, objParametri_Server) + vbCrLf)

            'RISORSE UMANE
            If Me.ChkGruppo1.Items.FindByValue(9).Selected = True Or _
                Me.ChkGruppo2.Items.FindByValue(10).Selected = True Or _
                Me.ChkGruppo2.Items.FindByValue(11).Selected = True Or _
                Me.ChkGruppo2.Items.FindByValue(12).Selected = True Or _
                x_Filtro_CodRapporto <> "" Then
                stbQuery.Append(" AND   Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(x_Data_Fine) & " " + vbCrLf)
                stbQuery.Append(" AND   Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(x_Data_Inizio) & " " + vbCrLf)
            End If

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------

            stbQuery.Append(" ORDER BY Contatti.Rag_Soc Asc ")



        Catch ex As Exception
            Throw New Exception("Prepara_StringaQuery_Contatti: " & ex.Message)
        End Try

        Return stbQuery



    End Function









End Class
