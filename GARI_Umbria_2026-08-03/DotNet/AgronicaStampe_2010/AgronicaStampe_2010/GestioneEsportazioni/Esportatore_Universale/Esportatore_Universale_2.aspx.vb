Imports System.Xml
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaStampe_2010.XMLObject
Imports System.Xml.Serialization
Imports System.IO
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider
Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreUtility.Varie
Imports Newtonsoft.Json.Linq
'   IL PORTING NON E' COMPLETATO, BISOGNA:
'   - EVITARE DI USARE CODEX UTILITY
'   - UTILIZZARE LE QUERY DEL DATAPROVIDER
'   - ELIMINARE L'UTILIZZO DEL RECORDSET
'   - ELIMINARE L'UTILIZZO DELLE CHIAVI DEL WEB.CONFIG (CHE NON ESISTONO SU STAMPE_2010)

Public Class Esportatore_Universale_2
    Inherits System.Web.UI.Page

    'GRUPPO 1
    Const I_Piva As Integer = 0 ' "Partita IVA"
    Const I_Rag_Soc As Integer = 1 '"Ragione Sociale"
    Const I_Cuaa As Integer = 2 '"CUAA"
    Const I_IndImpresa As Integer = 3 '"Indirizzo Impresa"
    Const I_ImpresaPadre As Integer = 4 '"Partita IVA  e Rag_Soc Impresa Padre"
    Const I_Tecnico As Integer = 5
    Const I_NomeCentro As Integer = 6 '"Nome Centro"
    Const I_IndCentro As Integer = 7 '"Indirizzo Centro"
    Const I_CodiciIstat As Integer = 8 '"Codici ISTAT Comune e Provincia"
    Const I_NomeCampo As Integer = 9 '"Nome Campo"

    'GRUPPO 2
    Const I_NomeAppezza As Integer = 10 '"Nome Appezzamento"
    Const I_DatiAppezza As Integer = 11 '"Supp, data inizio e fine Appezzamento"
    Const I_GruppoVeg As Integer = 12 '"Gruppo Vegetale"
    Const I_Specie As Integer = 13 ' "Specie Vegetale"
    Const I_Varieta As Integer = 14 ' "Varietà"
    Const I_TipVar As Integer = 15 ' "Tipologia Varietale"
    Const I_Finalita As Integer = 16 ' "Finalità"
    Const I_DestUso As Integer = 17 ' "Destinazione d'uso"
    Const I_SumImp As Integer = 18 ' "Superficie Impianto"
    Const I_InizioFineImp As Integer = 19 ' "Data Inizio / Data Fine Impianto"


    'GRUPPO 3
    Const I_SemTrapData As Integer = 20 ' "Seminato/Trapiantato, Data Semina/Trapianto"
    Const I_Copertura As Integer = 21 '"Copertura"
    Const I_FA_DSP As Integer = 22 ' "Forma Allevamento e Dettaglio Specie Personalizzato 
    Const I_Port_Irr As Integer = 23    'Portinensto, Impianto Irrigazione
    Const I_SestoImpianto As Integer = 24  'Sesto d'Impianto"
    Const I_Piante_Resa As Integer = 25  'Num Piante/Ha, Num Piante Tot e Resa Prevista"
    Const I_Lotto_DateDistinta As Integer = 26 ' "Lotto, Data Inizio e Fine Esercizio"
    Const I_Reg_Dpi_Capitolato As Integer = 27 ' "Regolamento e Capitolato Privato"
    Const I_OrgRef_MagConf As Integer = 28  '"Organismo Referente e Magazzino Conferimento"
    Const I_Particelle As Integer = 29 ' "Particelle e Superficie Particelle e Sup. di intersezione Impianto e Particella"

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    'Per Query di Lettura
    Dim DataProvider As New AgronicaCoreDataProvider.DataProvider

    Dim obj__tmp_Agenda_W As New AgronicaCoreVarieDAL.__tmp_Agenda_W

    '####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '                            Server, Session, Page, _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            TipiEnumerativi.enum_Security_Attivita.Stampe_Esportatore_Universale, _
        '                            TipiEnumerativi.enum_Security_Operazione.Modifica, _
        '                            strDummy)

        '----- !!!!!!!!!!! -------------
        'Attivazione forzata provvisoria

        UtenteAbilitato = True
        '----- !!!!!!!!!!! -------------

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

        If UtenteAbilitato = False Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If

        Dim scheda As String

        scheda = Stringa_Decodifica(Request.QueryString("t").ToString,
                                       AgroKey_EncoderDecoder,
                                       Server)

        Dim objConfSiti = New AgronicaCoreVarieDAL.Configurazione_Siti_R
        txt_XML.Text = objConfSiti.Leggi_Valore(6, "GestioneEsportazioni_Repository", "", "", objParametri_Server)

        ' se è in postback
        If Page.IsPostBack Then

            'se uso l'autopostback dei check per deselezioanre, 
            'in caso l'utente lo voglia selezionare, un check che non è abilitato

            'Select Case scheda

            '    Case "impianti"
            '        Me.ChkGruppo3.Items.FindByValue(26).Selected = False

            '    Case "imprese"
            '        Me.ChkGruppo2.Items.FindByValue(17).Selected = False
            '        Me.ChkGruppo2.Items.FindByValue(18).Selected = False
            '        Me.ChkGruppo2.Items.FindByValue(19).Selected = False

            'End Select


            Exit Sub

        End If ' if postback

        ' se non è in postback fa tutto questo ---->

        Call Carica_Pannello(scheda)

        'If HttpContext.Current.Session("chiaviEsportazioneAgenda") IsNot Nothing Then
        '    Try
        '        Dim controlValues As ControlValues = HttpContext.Current.Session("controlValues")
        '        Me.RdBList_Esporta.SelectedValue = controlValues.RdBList_Esporta_SelectedValue
        '        Me.ChkGruppo1.Items.FindByValue(0).Selected = controlValues.ChkValue0
        '        Me.ChkGruppo1.Items.FindByValue(1).Selected = controlValues.ChkValue1
        '        Me.ChkGruppo1.Items.FindByValue(2).Selected = controlValues.ChkValue2
        '        Me.ChkGruppo1.Items.FindByValue(3).Selected = controlValues.ChkValue3
        '        Me.ChkGruppo1.Items.FindByValue(4).Selected = controlValues.ChkValue4
        '        Me.ChkGruppo1.Items.FindByValue(5).Selected = controlValues.ChkValue5
        '        Me.ChkGruppo1.Items.FindByValue(6).Selected = controlValues.ChkValue6
        '        Me.ChkGruppo1.Items.FindByValue(7).Selected = controlValues.ChkValue7
        '        Me.ChkGruppo1.Items.FindByValue(8).Selected = controlValues.ChkValue8
        '        Me.ChkGruppo1.Items.FindByValue(9).Selected = controlValues.ChkValue9
        '        Me.ChkGruppo2.Items.FindByValue(10).Selected = controlValues.ChkValue10
        '        Me.ChkGruppo2.Items.FindByValue(11).Selected = controlValues.ChkValue11
        '        Me.ChkGruppo2.Items.FindByValue(12).Selected = controlValues.ChkValue12
        '        Me.ChkGruppo2.Items.FindByValue(13).Selected = controlValues.ChkValue13
        '        Me.ChkGruppo2.Items.FindByValue(14).Selected = controlValues.ChkValue14
        '        Me.ChkGruppo2.Items.FindByValue(15).Selected = controlValues.ChkValue15
        '        Me.ChkGruppo2.Items.FindByValue(16).Selected = controlValues.ChkValue16
        '        Me.ChkGruppo2.Items.FindByValue(17).Selected = controlValues.ChkValue17
        '        Me.ChkGruppo2.Items.FindByValue(18).Selected = controlValues.ChkValue18
        '        Me.ChkGruppo2.Items.FindByValue(19).Selected = controlValues.ChkValue19
        '        Me.ChkGruppo3.Items.FindByValue(20).Selected = controlValues.ChkValue20
        '        Me.ChkGruppo3.Items.FindByValue(21).Selected = controlValues.ChkValue21
        '        Me.ChkGruppo3.Items.FindByValue(22).Selected = controlValues.ChkValue22
        '        Me.ChkGruppo3.Items.FindByValue(23).Selected = controlValues.ChkValue23
        '        Me.ChkGruppo3.Items.FindByValue(24).Selected = controlValues.ChkValue24
        '        Me.ChkGruppo3.Items.FindByValue(25).Selected = controlValues.ChkValue25
        '        Me.ChkGruppo3.Items.FindByValue(26).Selected = controlValues.ChkValue26
        '        Me.ChkGruppo3.Items.FindByValue(27).Selected = controlValues.ChkValue27
        '        Me.ChkGruppo3.Items.FindByValue(28).Selected = controlValues.ChkValue28
        '        Me.ChkGruppo3.Items.FindByValue(29).Selected = controlValues.ChkValue29
        '        Me.ChkGruppo3.Items.FindByValue(30).Selected = controlValues.ChkValue30
        '        Me.ChkGruppo3.Items.FindByValue(31).Selected = controlValues.ChkValue31
        '        Me.ChkGruppo4.Items.FindByValue(32).Selected = controlValues.ChkValue32
        '        Me.ChkGruppo5.Items.FindByValue(33).Selected = controlValues.ChkValue33
        '        Me.ChkGruppo5.Items.FindByValue(34).Selected = controlValues.ChkValue34

        '        Esportazione_START(controlValues.Chiavi)
        '    Catch ex As Exception
        '        Dim msg = "Esportazione fallita: " + ex.Message
        '        AgroMsgBox(msg)
        '    Finally
        '        HttpContext.Current.Session("chiaviEsportazioneAgenda") = Nothing
        '        HttpContext.Current.Session("controlValues") = Nothing
        '    End Try
        'Else
        '    BtnSelezionaTutto_ServerClick(Me, Nothing)
        'End If

        BtnSelezionaTutto_ServerClick(Me, Nothing)
    End Sub


    '##################################################################
    Private Sub Carica_Pannello(ByVal scheda As String)

        '************************
        '   A = IMPRESE
        '   B = CENTRI
        '   C = APPEZZAMENTI
        '   D = IMPIANTI
        '   E = AGENDA
        '   F = XML RINTRACCIO
        '************************

        'Dim PathWebConfig As String

        Me.lbl_ordinamento.Visible = False
        Me.Cmb_OrdinamentoImpianti.Visible = False

        'TOGLIERE QUANDO SERVIRA'
        Me.Txt_FileOutput.Enabled = False
        Me.Txt_FileOutput.BackColor = Drawing.Color.Gray

        Me.Panel_Btn_Impianti.BackColor = Drawing.Color.PaleTurquoise
        Me.Lbl_BTN_Impianti.BackColor = Drawing.Color.PaleTurquoise
        Me.Panel_Btn_Imprese.BackColor = Drawing.Color.PaleTurquoise
        Me.Lbl_BTN_Imprese.BackColor = Drawing.Color.PaleTurquoise


        Me.Txt_Data.Text = CStr(Date.Today)

        If Not objParametri_Server.LogDirectory.EndsWith("\") Then
            objParametri_Server.LogDirectory += "\"
        End If

        'If (Not IsNothing(ConfigurationSettings.AppSettings("Path_Esportazioni_Stampe")) And (ConfigurationSettings.AppSettings("Path_Esportazioni_Stampe") <> "")) Then
        '    'prelevo il percorso dal web config
        '    PathWebConfig = ConfigurationSettings.AppSettings("Path_Esportazioni_Stampe")
        'Else
        '    PathWebConfig = "C:\AgroTemporanea"
        'End If

        ViewState("scheda") = scheda

        Select Case scheda

            Case "impianti"

                Session("scheda") = "D"

                Carica_Cmb_OrdinamentoImpianti()
                Me.lbl_ordinamento.Visible = True
                Me.Cmb_OrdinamentoImpianti.Visible = True

                'CType(Master.FindControl("lbl_Titolo"), Label).Text = "Esportazione Impianti"
                CType(Page.Master, StampeBootstrap).Lbl_Titolo.Text = "Esportazione Impianti"

                'Me.Txt_PathEsportazione.Text = PathWebConfig + "\Esportatore_Universale"
                Me.Txt_PathEsportazione.Text = objParametri_Server.LogDirectory + "Esportatore_Universale"

                Me.Panel_Btn_Impianti.BackColor = Drawing.Color.Yellow
                Me.Lbl_BTN_Impianti.BackColor = Drawing.Color.Yellow

                'Me.RdBList_Esporta.Items(1).Text = "<font color=""gray""><i>XML Rintraccio</i></font>"
                Me.RdBList_Esporta.Items(1).Enabled = False
                Me.RdBList_Esporta.SelectedValue = "A"

                'GRUPPO 1
                'Me.ChkGruppo1.Items.FindByValue(I_Cod_Centro).Text = "Codice Centro"
                'Me.ChkGruppo1.Items.FindByValue(I_Cod_Campo).Text = "Codice Campo"
                'Me.ChkGruppo1.Items.FindByValue(I_Cod_Appezza).Text = "Codice Appezzamento"
                'Me.ChkGruppo1.Items.FindByValue(I_Cod_Imp).Text = "Codice Impianto"
                Me.ChkGruppo1.Items.FindByValue(I_Piva).Text = "Partita IVA"
                Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Text = "Ragione Sociale"
                Me.ChkGruppo1.Items.FindByValue(I_Cuaa).Text = "CUAA"
                'Me.ChkGruppo1.Items.FindByValue(I_Cod_Socio).Text = "Codice Socio"
                Me.ChkGruppo1.Items.FindByValue(I_IndImpresa).Text = "Indirizzo Impresa"
                Me.ChkGruppo1.Items.FindByValue(I_ImpresaPadre).Text = "P.IVA e Rag.Soc. Impresa Padre"
                Me.ChkGruppo1.Items.FindByValue(I_Tecnico).Text = "Tecnico di Riferimento"
                Me.ChkGruppo1.Items.FindByValue(I_NomeCentro).Text = "Centro Aziendale"
                Me.ChkGruppo1.Items.FindByValue(I_IndCentro).Text = "Indirizzo Centro"
                Me.ChkGruppo1.Items.FindByValue(I_CodiciIstat).Text = "Codici ISTAT Comune e Provincia"
                Me.ChkGruppo1.Items.FindByValue(I_NomeCampo).Text = "Campo"


                'GRUPPO 2
                Me.ChkGruppo2.Items.FindByValue(I_NomeAppezza).Text = "Appezzamento"
                Me.ChkGruppo2.Items.FindByValue(I_DatiAppezza).Text = "Superficie, DataInizio e Fine Appezzamento"
                Me.ChkGruppo2.Items.FindByValue(I_GruppoVeg).Text = "Gruppo Vegetale"
                'Me.ChkGruppo2.Items.FindByValue(I_Cod_Specie).Text = "Codice Specie Vegetale"
                Me.ChkGruppo2.Items.FindByValue(I_Specie).Text = "Specie Vegetale"
                Me.ChkGruppo2.Items.FindByValue(I_Varieta).Text = "Varietà"
                Me.ChkGruppo2.Items.FindByValue(I_TipVar).Text = "Tipologia Varietale"
                Me.ChkGruppo2.Items.FindByValue(I_Finalita).Text = "Finalità"
                Me.ChkGruppo2.Items.FindByValue(I_DestUso).Text = "Destinazione d'uso"
                Me.ChkGruppo2.Items.FindByValue(I_SumImp).Text = "Superficie Impianto"
                Me.ChkGruppo2.Items.FindByValue(I_InizioFineImp).Text = "Data Inizio / Data Fine Impianto"

                'GRUPPO 3
                'Me.ChkGruppo3.Items.FindByValue(I_SemTrap).Text = "Operazione Semina / Trapianto"
                'Me.ChkGruppo3.Items.FindByValue(I_DataSemTrap).Text = "Data Semina / Data Trapianto"
                'Me.ChkGruppo3.Items.FindByValue(I_SupImpPart).Text = "Sup. di intersezione Impianto e Particella"
                Me.ChkGruppo3.Items.FindByValue(I_SemTrapData).Text = "Seminato/Trapiantato, Data Semina/Trapianto"
                Me.ChkGruppo3.Items.FindByValue(I_Copertura).Text = "Copertura"
                Me.ChkGruppo3.Items.FindByValue(I_FA_DSP).Text = "Forma Allevamento e Dettaglio Specie Personalizzato"
                Me.ChkGruppo3.Items.FindByValue(I_Port_Irr).Text = "Portinnesto e Impianto Irrigazione"
                Me.ChkGruppo3.Items.FindByValue(I_SestoImpianto).Text = "Sesto d'Impianto"
                Me.ChkGruppo3.Items.FindByValue(I_Piante_Resa).Text = "Num Piante/Ha, Num Piante Tot e Resa Prevista"
                Me.ChkGruppo3.Items.FindByValue(I_Lotto_DateDistinta).Text = "Lotto e Data Inizio/Fine Distinta"
                Me.ChkGruppo3.Items.FindByValue(I_Reg_Dpi_Capitolato).Text = "Regolamento e Capitolato Privato"
                Me.ChkGruppo3.Items.FindByValue(I_OrgRef_MagConf).Text = "Organismo Referente e Magazzino Conferimento"
                Me.ChkGruppo3.Items.FindByValue(I_Particelle).Text = "Particelle, Sup Catastale e Sup di Intersezione con Impianto"

                Me.ChkGruppo1.Visible = True
                Me.ChkGruppo1.AutoPostBack = False
                Me.ChkGruppo2.Visible = True
                Me.ChkGruppo2.AutoPostBack = False
                Me.ChkGruppo3.Visible = True
                Me.ChkGruppo3.AutoPostBack = False

                Me.LblDeselezionaTutto.Visible = True
                Me.LblSelezionaTutto.Visible = True
                Me.BtnDeselezionaTutto.Visible = True
                Me.BtnSelezionaTutto.Visible = True
                Me.ImgBtnDeselezionaTutto.Visible = True
                Me.ImgBtnSelezionaTutto.Visible = True
                Me.LblSeleziona.Visible = True
                Me.Panel_Btn_Rintraccio.Visible = False

                For Each elem As ListItem In ChkGruppo3.Items
                    elem.Selected = True
                Next
                For Each elem As ListItem In ChkGruppo2.Items
                    elem.Selected = True
                Next
                For Each elem As ListItem In ChkGruppo1.Items
                    elem.Selected = True
                Next

                Me.LblDeselezionaTutto.Visible = True
                Me.LblSelezionaTutto.Visible = True
                Me.BtnDeselezionaTutto.Visible = True
                Me.BtnSelezionaTutto.Visible = True
                Me.ImgBtnDeselezionaTutto.Visible = True
                Me.ImgBtnSelezionaTutto.Visible = True



                '---------------------------------------------------

            Case "imprese"

                Session("scheda") = "A"

                'CType(Master.FindControl("lbl_Titolo"), Label).Text = "Esportazione Imprese"
                CType(Page.Master, StampeBootstrap).Lbl_Titolo.Text = "Esportazione Imprese"

                'Me.Txt_PathEsportazione.Text = PathWebConfig + "\Esportatore_Universale"
                Me.Txt_PathEsportazione.Text = objParametri_Server.LogDirectory + "Esportatore_Universale"
                Me.Panel_Btn_Imprese.BackColor = Drawing.Color.Yellow
                Me.Lbl_BTN_Imprese.BackColor = Drawing.Color.Yellow

                'Me.RdBList_Esporta.Items(1).Text = "<font color=""gray""><i>XML Rintraccio</i></font>"
                Me.RdBList_Esporta.Items(1).Enabled = False
                Me.RdBList_Esporta.SelectedValue = "A"

                'GRUPPO 1
                Me.ChkGruppo1.Items.FindByValue(0).Text = "Partita IVA"
                Me.ChkGruppo1.Items.FindByValue(1).Text = "Ragione Sociale"
                Me.ChkGruppo1.Items.FindByValue(2).Text = "Tipo Impresa"
                Me.ChkGruppo1.Items.FindByValue(3).Text = "Codice Socio"
                Me.ChkGruppo1.Items.FindByValue(4).Text = "CUAA"
                Me.ChkGruppo1.Items.FindByValue(5).Text = "Cooperativa Padre"
                Me.ChkGruppo1.Items.FindByValue(6).Text = "Partita IVA Coop. Padre"
                Me.ChkGruppo1.Items.FindByValue(7).Text = "Indirizzo"
                Me.ChkGruppo1.Items.FindByValue(8).Text = "Codici ISTAT Comune e Provincia"
                Me.ChkGruppo1.Items.FindByValue(9).Text = "Data Inizio / Data Fine Impresa"


                'GRUPPO2
                Me.ChkGruppo2.Items.FindByValue(10).Text = "Codice Fiscale Legale Rappresentante"
                Me.ChkGruppo2.Items.FindByValue(11).Text = "Nome e Cognome Legale Rappresentante"
                Me.ChkGruppo2.Items.FindByValue(12).Text = "Provincia e Comune Nascita Legale Rappr."
                Me.ChkGruppo2.Items.FindByValue(13).Text = "Particelle"
                Me.ChkGruppo2.Items.FindByValue(14).Text = "Titolo Possesso Particella"
                Me.ChkGruppo2.Items.FindByValue(15).Text = "Data Inizio / Data Fine Possesso"
                Me.ChkGruppo2.Items.FindByValue(16).Text = "Superficie Particella"
                Me.ChkGruppo2.Items.FindByValue(17).Text = "<font color=""gray""><i>non utilizzato</i></font>"
                Me.ChkGruppo2.Items.FindByValue(18).Text = "<font color=""gray""><i>non utilizzato</i></font>"
                Me.ChkGruppo2.Items.FindByValue(19).Text = "<font color=""gray""><i>non utilizzato</i></font>"

                Me.ChkGruppo1.Visible = True
                Me.ChkGruppo1.AutoPostBack = False
                Me.ChkGruppo2.Visible = True
                Me.ChkGruppo2.AutoPostBack = False
                Me.ChkGruppo3.Visible = False
                Me.ChkGruppo3.AutoPostBack = False

                Me.LblDeselezionaTutto.Visible = True
                Me.LblSelezionaTutto.Visible = True
                Me.ImgBtnDeselezionaTutto.Visible = True
                Me.ImgBtnSelezionaTutto.Visible = True
                Me.BtnSelezionaTutto.Visible = True
                Me.BtnDeselezionaTutto.Visible = True
                Me.LblSeleziona.Visible = True
                Me.Panel_Btn_Rintraccio.Visible = False

                '---------------------------------------------------

            Case "centri"

                Session("scheda") = "B"

                'CType(Master.FindControl("lbl_Titolo"), Label).Text = "Esportazione Centri Aziendali"
                CType(Page.Master, StampeBootstrap).Lbl_Titolo.Text = "Esportazione Centri Aziendali"
                'Me.Txt_PathEsportazione.Text = PathWebConfig + "\Esportatore_Universale"
                Me.Txt_PathEsportazione.Text = objParametri_Server.LogDirectory + "Esportatore_Universale"
                Me.Panel_Btn_Centri.BackColor = Drawing.Color.Yellow
                Me.Lbl_BTN_Centri.BackColor = Drawing.Color.Yellow

                'Me.RdBList_Esporta.Items(1).Text = "<font color=""gray""><i>XML Rintraccio</i></font>"
                Me.RdBList_Esporta.Items(1).Enabled = False
                Me.RdBList_Esporta.SelectedValue = "A"

                'GRUPPO 1
                Me.ChkGruppo1.Items.FindByValue(0).Text = "Partita IVA"
                Me.ChkGruppo1.Items.FindByValue(1).Text = "Ragione Sociale Impresa"
                Me.ChkGruppo1.Items.FindByValue(2).Text = "Codice Centro"
                Me.ChkGruppo1.Items.FindByValue(3).Text = "Nome Centro Aziendale"
                Me.ChkGruppo1.Items.FindByValue(4).Text = "Tipo Centro Aziendale"
                Me.ChkGruppo1.Items.FindByValue(5).Text = "Indirizzo Centro"
                Me.ChkGruppo1.Items.FindByValue(6).Text = "Data Inizio / Fine Attività Centro"
                Me.ChkGruppo1.Items.FindByValue(7).Text = "Titolo Possesso Centro"
                Me.ChkGruppo1.Items.FindByValue(8).Text = "Tipo Attività"
                Me.ChkGruppo1.Items.FindByValue(9).Text = "O.T.E."

                Me.ChkGruppo2.Items.FindByValue(10).Text = "Superficie Totale (somma particelle)"
                Me.ChkGruppo2.Items.FindByValue(11).Text = "SAU Totale (somma appezzamenti)"
                Me.ChkGruppo2.Items.FindByValue(12).Text = "Tara (Sup Totale - SAU Totale)"
                Me.ChkGruppo2.Items.FindByValue(13).Text = "SAU Convenzionale"
                Me.ChkGruppo2.Items.FindByValue(14).Text = "SAU Conversione"
                Me.ChkGruppo2.Items.FindByValue(15).Text = "SAU Biologico"
                Me.ChkGruppo2.Items.FindByValue(16).Text = "Superficie Bosco"
                Me.ChkGruppo2.Items.FindByValue(17).Text = "Superficie Prato"
                Me.ChkGruppo2.Items.FindByValue(18).Text = "Mappa"
                Me.ChkGruppo2.Items.FindByValue(19).Text = "Fabbricati"

                Me.ChkGruppo3.Items.FindByValue(20).Text = "Codice Operatore"
                Me.ChkGruppo3.Items.FindByValue(21).Text = "Codice Zooprofilattico"
                Me.ChkGruppo3.Items.FindByValue(22).Text = "Codice CERPL"
                Me.ChkGruppo3.Items.FindByValue(23).Text = "Codice AUA"
                Me.ChkGruppo3.Items.FindByValue(24).Text = "Codice AUSL"
                Me.ChkGruppo3.Items.FindByValue(25).Text = "Codice CNAL"
                Me.ChkGruppo3.Items.FindByValue(26).Text = "Organismi Controllo Biologico"
                Me.ChkGruppo3.Items.FindByValue(27).Text = "Particelle"
                Me.ChkGruppo3.Items.FindByValue(28).Text = "Superficie Particella"
                Me.ChkGruppo3.Items.FindByValue(29).Text = "Titolo Possesso Particella,<br/>Data Inizio/Fine Possesso Particella"

                Me.ChkGruppo1.Visible = True
                Me.ChkGruppo1.AutoPostBack = False
                Me.ChkGruppo2.Visible = True
                Me.ChkGruppo2.AutoPostBack = False
                Me.ChkGruppo3.Visible = True
                Me.ChkGruppo3.AutoPostBack = False

                Me.LblDeselezionaTutto.Visible = True
                Me.LblSelezionaTutto.Visible = True
                Me.ImgBtnDeselezionaTutto.Visible = True
                Me.ImgBtnSelezionaTutto.Visible = True
                Me.BtnSelezionaTutto.Visible = True
                Me.BtnDeselezionaTutto.Visible = True
                Me.LblSeleziona.Visible = True
                Me.Panel_Btn_Rintraccio.Visible = False

                '-----------------------------------------------------------

            Case "agenda"

                Session("scheda") = "E"

                'CType(Master.FindControl("lbl_Titolo"), Label).Text = "Esportazione Operazioni di Agenda"
                CType(Page.Master, StampeBootstrap).Lbl_Titolo.Text = "Esportazione Operazioni di Agenda"
                'Me.Txt_PathEsportazione.Text = PathWebConfig + "\Esportatore_Universale"
                Me.Txt_PathEsportazione.Text = objParametri_Server.LogDirectory + "Esportatore_Universale"
                Me.Panel_Btn_Agenda.BackColor = Drawing.Color.Yellow
                Me.Lbl_BTN_Agenda.BackColor = Drawing.Color.Yellow

                'Me.RdBList_Esporta.Items(1).Text = "<font color=""gray""><i>XML Rintraccio</i></font>"
                Me.RdBList_Esporta.Items(1).Enabled = False
                Me.RdBList_Esporta.SelectedValue = "A"

                'GRUPPO 1
                Me.ChkGruppo1.Items.FindByValue(0).Text = "Partita IVA Impresa"
                Me.ChkGruppo1.Items.FindByValue(1).Text = "Ragione Sociale Impresa"
                Me.ChkGruppo1.Items.FindByValue(2).Text = "Codice Centro Aziendale"
                Me.ChkGruppo1.Items.FindByValue(3).Text = "Nome Centro Aziendale"
                Me.ChkGruppo1.Items.FindByValue(4).Text = "Codice Campo"
                Me.ChkGruppo1.Items.FindByValue(5).Text = "Nome Campo"
                Me.ChkGruppo1.Items.FindByValue(6).Text = "Codice Appezzamento"
                Me.ChkGruppo1.Items.FindByValue(7).Text = "Nome Appezzamento"
                Me.ChkGruppo1.Items.FindByValue(8).Text = "Codice Impianto"
                Me.ChkGruppo1.Items.FindByValue(9).Text = "Superficie Impianto"

                'GRUPPO 2           
                'Me.ChkGruppo2.Items.FindByValue(10).Text = "Codice Specie Vegetale"
                'Me.ChkGruppo2.Items.FindByValue(11).Text = "Specie Vegetale"
                'Me.ChkGruppo2.Items.FindByValue(12).Text = "Codice Varietà"
                'Me.ChkGruppo2.Items.FindByValue(13).Text = "Varietà"
                'Me.ChkGruppo2.Items.FindByValue(14).Text = "Tipologia Varietale"
                'Me.ChkGruppo2.Items.FindByValue(15).Text = "Finalità"
                'Me.ChkGruppo2.Items.FindByValue(16).Text = "Data Inizio / Data Fine Impianto"
                'Me.ChkGruppo2.Items.FindByValue(17).Text = "Gruppo Operazione"
                'Me.ChkGruppo2.Items.FindByValue(18).Text = "Blocco Operazione"
                'Me.ChkGruppo2.Items.FindByValue(19).Text = "Codice Agenda"

                'GRUPPO 2           
                Me.ChkGruppo2.Items.FindByValue(10).Text = "Codice Specie Vegetale"
                Me.ChkGruppo2.Items.FindByValue(11).Text = "Specie Vegetale"
                Me.ChkGruppo2.Items.FindByValue(12).Text = "Codice Varietà"
                Me.ChkGruppo2.Items.FindByValue(13).Text = "Varietà"
                Me.ChkGruppo2.Items.FindByValue(14).Text = "Tipologia Varietale"
                Me.ChkGruppo2.Items.FindByValue(15).Text = "Finalità"
                Me.ChkGruppo2.Items.FindByValue(16).Text = "Data Inizio / Data Fine Impianto"
                Me.ChkGruppo2.Items.FindByValue(17).Text = "Regolamento"
                Me.ChkGruppo2.Items.FindByValue(18).Text = "Gruppo Operazione"
                Me.ChkGruppo2.Items.FindByValue(19).Text = "Blocco Operazione"

                ''GRUPPO 3
                'Me.ChkGruppo3.Items.FindByValue(20).Text = "Data Operazione"
                'Me.ChkGruppo3.Items.FindByValue(21).Text = "Descrizione Operazione"
                'Me.ChkGruppo3.Items.FindByValue(22).Text = "Categoria e Prodotto / Materia Prima utilizzata"
                'Me.ChkGruppo3.Items.FindByValue(23).Text = "Unità di misura"
                'Me.ChkGruppo3.Items.FindByValue(24).Text = "Quantità"
                'Me.ChkGruppo3.Items.FindByValue(25).Text = "Particelle"
                'Me.ChkGruppo3.Items.FindByValue(26).Text = "Superficie Particella"
                'Me.ChkGruppo3.Items.FindByValue(27).Text = "Titolo Possesso e Data Inizio/Fine Possesso Particella"
                'Me.ChkGruppo3.Items.FindByValue(28).Text = "Superficie di Intersezione con Particella"
                'Me.ChkGruppo3.Items.FindByValue(29).Text = "Note e Creatore Operazione"

                'GRUPPO 3
                Me.ChkGruppo3.Items.FindByValue(20).Text = "Data Operazione"
                Me.ChkGruppo3.Items.FindByValue(21).Text = "Descrizione Operazione"
                Me.ChkGruppo3.Items.FindByValue(22).Text = "Categoria e Prodotto / Materia Prima utilizzata / P.A."
                Me.ChkGruppo3.Items.FindByValue(23).Text = "Unità di misura"
                Me.ChkGruppo3.Items.FindByValue(24).Text = "Quantità"
                Me.ChkGruppo3.Items.FindByValue(25).Text = "Superficie Trattata"
                Me.ChkGruppo3.Items.FindByValue(26).Text = "Quantità Totale"
                Me.ChkGruppo3.Items.FindByValue(27).Text = "Particelle"
                Me.ChkGruppo3.Items.FindByValue(28).Text = "Superficie Particella"
                Me.ChkGruppo3.Items.FindByValue(29).Text = "Titolo Possesso e Data Inizio/Fine Possesso Particella"
                Me.ChkGruppo3.Items.FindByValue(30).Text = "Superficie di Intersezione con Particella"
                Me.ChkGruppo3.Items.FindByValue(31).Text = "Note e Creatore Operazione"


                Me.ChkGruppo1.Visible = True
                Me.ChkGruppo1.AutoPostBack = False
                Me.ChkGruppo2.Visible = True
                Me.ChkGruppo2.AutoPostBack = False
                Me.ChkGruppo3.Visible = True
                Me.ChkGruppo3.AutoPostBack = False
                Me.ChkGruppo4.Visible = True
                Me.ChkGruppo4.AutoPostBack = False
                Me.ChkGruppo5.Visible = True
                Me.ChkGruppo5.AutoPostBack = False

                Me.LblDeselezionaTutto.Visible = True
                Me.LblSelezionaTutto.Visible = True
                Me.ImgBtnDeselezionaTutto.Visible = True
                Me.ImgBtnSelezionaTutto.Visible = True
                Me.BtnSelezionaTutto.Visible = True
                Me.BtnDeselezionaTutto.Visible = True
                Me.LblSeleziona.Visible = True
                Me.Panel_Btn_Rintraccio.Visible = False

                Me.Panel_Esadecimale.Visible = False
                Me.Txt_Data.Visible = False
                Me.RdBList_Distinta.Visible = False
                Me.LblFiltroEsercizi.Visible = False

                Dim filtroRicerca = New AgronicaCoreFiltroneBIZ.FiltroRicerca
                If (filtroRicerca.usaFiltroRicercaNG(objParametri_Utenti)) Then
                    Me.PanelButtonEsporta.Visible = True
                    Me.Btn_Esporta.Visible = False
                End If

                '---------------------------------------------------

            Case "rintraccio"

                Session("scheda") = "F"

                Me.Panel_Esadecimale.Visible = False

                'Me.RdBList_Esporta.Items(0).Text = "<font color=""gray""><i>Excel (xls)</i></font>"
                Me.RdBList_Esporta.Items(0).Enabled = False
                Me.RdBList_Esporta.SelectedValue = "C"

                'CType(Master.FindControl("lbl_Titolo"), Label).Text = "Esportazione XML Rintraccio"
                CType(Page.Master, StampeBootstrap).Lbl_Titolo.Text = "Esportazione XML Rintraccio"
                'Me.Txt_PathEsportazione.Text = PathWebConfig + "\Esportazione_Rintraccio"
                Me.Txt_PathEsportazione.Text = objParametri_Server.LogDirectory + "Esportazione_Rintraccio"
                Me.Panel_Btn_Rintraccio.BackColor = Drawing.Color.Yellow
                Me.Lbl_BTN_Rintraccio.BackColor = Drawing.Color.Yellow

                Me.Txt_FileOutput.Enabled = False
                Me.Txt_FileOutput.BackColor = Drawing.Color.Gray
                Me.RdBList_Esporta.SelectedValue = "C"

                Me.LblDeselezionaTutto.Visible = False
                Me.LblSelezionaTutto.Visible = False
                Me.ImgBtnDeselezionaTutto.Visible = False
                Me.ImgBtnSelezionaTutto.Visible = False
                Me.BtnSelezionaTutto.Visible = False
                Me.BtnDeselezionaTutto.Visible = False
                Me.LblSeleziona.Visible = False
                Me.Panel_Btn_Rintraccio.Visible = True

                Me.Lbl_Punti.Visible = True
                Me.Cmb_Punti.Visible = True

                Me.Lbl_Validita_Inizio.Visible = True
                Me.Txt_ValiditaInizio.Visible = True
                'Me.BtnValiditaInizio.Style.Item("left") = 430
                'Me.BtnValiditaInizio.Style.Item("top") = 63
                Me.Txt_ValiditaInizio.Text = "01/01/" + CStr(Date.Today.Year)

                Me.Cmb_Punti.Items.Add(New ListItem("", -1))
                Me.Cmb_Punti.Items.Add(New ListItem("ARP Pisello/Fagiolo", 99))
                'Me.Cmb_Punti.Items.Add(New ListItem("COPADOR", 10099))
                Me.Cmb_Punti.Items.Add(New ListItem("ARP Pomodoro", 20099))
                'Me.Cmb_Punti.Items.Add(New ListItem("FAP", 30099))
                'Me.Cmb_Punti.Items.Add(New ListItem("COPAP", 40099))
                Me.Cmb_Punti.SelectedIndex = 2


        End Select


    End Sub

    '###################################################################################
    Private Sub ImgBtnSelezionaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnSelezionaTutto.Click

        Dim i As Integer

        For i = 0 To 9
            Me.ChkGruppo1.Items.FindByValue(i).Selected = True
        Next

        For i = 10 To 19
            Me.ChkGruppo2.Items.FindByValue(i).Selected = True
        Next

        For i = 20 To 31
            Me.ChkGruppo3.Items.FindByValue(i).Selected = True
        Next

        Me.ChkGruppo4.Items.FindByValue(32).Selected = True
        Me.ChkGruppo5.Items.FindByValue(33).Selected = True
        Me.ChkGruppo5.Items.FindByValue(34).Selected = True

        Select Case ViewState("scheda")

            'Case "impianti"
            'quando c'era il poligono
            '    Me.ChkGruppo3.Items.FindByValue(26).Selected = False

            Case "imprese"
                Me.ChkGruppo2.Items.FindByValue(17).Selected = False
                Me.ChkGruppo2.Items.FindByValue(18).Selected = False
                Me.ChkGruppo2.Items.FindByValue(19).Selected = False

        End Select

    End Sub


    '###################################################################################
    Private Sub ImgBtnDeselezionaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnDeselezionaTutto.Click

        Dim i As Integer

        For i = 0 To 9
            Me.ChkGruppo1.Items.FindByValue(i).Selected = False
        Next

        For i = 10 To 19
            Me.ChkGruppo2.Items.FindByValue(i).Selected = False
        Next

        For i = 20 To 31
            Me.ChkGruppo3.Items.FindByValue(i).Selected = False
        Next

        Me.ChkGruppo4.Items.FindByValue(32).Selected = False
        Me.ChkGruppo5.Items.FindByValue(33).Selected = False
        Me.ChkGruppo5.Items.FindByValue(34).Selected = False

    End Sub


    '###################################################################################
    Private Sub ImgBtn_Annulla_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) 'Handles ImgBtn_Annulla.Click

        Session("strXmlVariabilistampe") = Nothing
        Session("scheda") = Nothing

        '----- Chiudo la finestra
        Page.FindControl("Form1").Controls.Add(
            New LiteralControl(
                "<script language='javascript'>window.close();</script>"))

    End Sub

    '###################################################################################
    Private Sub Btn_Esporta_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Esporta.Click

        Try
            Esportazione_START()
        Catch ex As Exception
            Dim msg = "Esportazione fallita: " + ex.Message
            AgroMsgBox(msg)
        End Try
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Link_Pagina_FiltroRicercaNG(datiAggiuntiviFiltroRicerca As String) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim RispostaOK As Boolean
        Dim RispostaStringa As String

        Try
            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            Dim codificaStampe = enum_CodificaStampe.Esportatore_Universale_Agenda
            Dim tipoMostraFiltriRicerca = New List(Of Enum_TipoMostra_FiltroRicerca) From {
                Enum_TipoMostra_FiltroRicerca.Movimenti
            }

            Dim blocchiSelezionexStampa = objFiltroRicerca.GetBlocchiSelezionePerStampa(codificaStampe)
            Dim parametriFiltroRicercaNG As New AgronicaCoreGestioneRichieste.ParametriFiltroRicercaNG With {
                .Piva = objParametriAgenda.Piva,
                .CodificaStampe = codificaStampe,
                .TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita,
                .SitoOrigine = Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                .FiltriTemporali = objFiltroRicerca.Imposta_FiltroEntitaAttivaAllaData(Date.Now, Enum_Entita_FiltroRicerca.Impianto),
                .TipoMostraGestitiChiamante = tipoMostraFiltriRicerca,
                .BlocchiSelezionexStampa = blocchiSelezionexStampa
            }

            Dim objDatiAggiuntivi As JArray = JArray.Parse(datiAggiuntiviFiltroRicerca)
            For Each datiAggiuntivi As JObject In objDatiAggiuntivi
                Dim campo = CStr(datiAggiuntivi("Campo"))
                Dim checked = CBool(CStr(datiAggiuntivi("Checked")))

                If checked Then
                    Select Case campo
                        Case "ACA"
                            parametriFiltroRicercaNG.CaricaDatiAggiuntivi.ContributiACA = True
                        Case "ZVN"
                            parametriFiltroRicercaNG.CaricaDatiAggiuntivi.CatastoAppezzamento = True
                    End Select
                End If
            Next

            Dim link = objFiltroRicerca.Link_Pagina_FiltroRicercaNG(objParametriAgenda.Piva, parametriFiltroRicercaNG)
            If link = "" Then
                RispostaOK = False
                RispostaStringa = "Errore"
            Else
                RispostaOK = True
                RispostaStringa = link
            End If

            r.RispostaOK = RispostaOK
            r.RispostaStringa = JsonConvert.SerializeObject(New AgronicaCoreFiltroneBIZ.RespFiltroRicercaNG With {.RispostaStringa = RispostaStringa, .TipoMostra = Enum_TipoMostra_FiltroRicerca.Movimenti})

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    '<WebMethod(EnableSession:=True)>
    'Public Shared Function SalvaChiaviEsportazioneAgenda(controlValues As ControlValues) As AgronicaCoreVarieBIZ.RispostaStandard

    '    Dim chiavi As List(Of String) = New List(Of String)()
    '    Dim count = controlValues.Chiavi.Count

    '    For index = 0 To count - 1
    '        Dim c = controlValues.Chiavi(index)
    '        Dim ChiaviAgenda_Split = c.Split("_")

    '        Dim Piva = ChiaviAgenda_Split(0)
    '        Dim Sa_Cod = CInt(ChiaviAgenda_Split(1))
    '        Dim id_Agenda = CInt(ChiaviAgenda_Split(5))
    '        Dim chiave = $"{Piva}_{Sa_Cod}_{id_Agenda}"

    '        If (Not chiavi.Contains(chiave)) Then
    '            chiavi.Add(chiave)
    '        End If
    '    Next

    '    controlValues.Chiavi = chiavi

    '    HttpContext.Current.Session("chiaviEsportazioneAgenda") = controlValues.Chiavi
    '    HttpContext.Current.Session("controlValues") = controlValues

    '    Dim r As New AgronicaCoreVarieBIZ.RispostaStandard
    '    r.RispostaOK = True
    '    Return r

    'End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function EsportazioneAgenda_Excel(controlValues As ControlValues) As AgronicaCoreVarieBIZ.RispostaStandard
        Dim chiavi As List(Of String) = New List(Of String)()
        Dim count = controlValues.Chiavi.Count

        Dim r As AgronicaCoreVarieBIZ.RispostaStandard = New AgronicaCoreVarieBIZ.RispostaStandard()

        For index = 0 To count - 1
            Dim c = controlValues.Chiavi(index)
            Dim ChiaviAgenda_Split = c.Split("_")

            Dim Piva = ChiaviAgenda_Split(0)
            Dim Sa_Cod = CInt(ChiaviAgenda_Split(1))
            Dim id_Agenda = CInt(ChiaviAgenda_Split(5))
            Dim chiave = $"{Piva}_{Sa_Cod}_{id_Agenda}"

            If (Not chiavi.Contains(chiave)) Then
                chiavi.Add(chiave)
            End If
        Next

        controlValues.Chiavi = chiavi

        Dim DT As New DataTable
        Dim msg As String = ""
        Dim Messaggio As String = ""
        Dim AgroMsg As String = ""
        Dim Log As String = ""

        Try
            Esportazione_Agenda(DT, controlValues, msg, Messaggio, AgroMsg, Log, True)

            For i = 0 To DT.Rows.Count - 1

                For j = 0 To DT.Columns.Count - 1
                    Select Case DT.Columns.Item(j).ColumnName

                        Case "inizio_distinta", "inizio_impianto", "inizio_centro", "inizio_impresa", "inizio_appezza", "data_semina"
                            If DT.Rows(i)(j) = "01/01/1900" Then
                                DT.Rows(i)(j) = " ... "
                            End If

                        Case "fine_distinta", "fine_impianto", "fine_centro", "fine_impresa", "fine_appezza"
                            If DT.Rows(i)(j) = "31/12/2100" Then
                                DT.Rows(i)(j) = " ... "
                            End If

                    End Select
                Next

            Next

            If (DT.Columns.Contains("PIVA")) Then
                DT.Columns("PIVA").ColumnName = "Partita IVA Impresa"
            End If

            If (DT.Columns.Contains("SA_COD")) Then
                DT.Columns("SA_COD").ColumnName = "Codice Centro"
            End If

            If (DT.Columns.Contains("campo_cod")) Then
                DT.Columns("campo_cod").ColumnName = "Codice Campo"
            End If

            If (DT.Columns.Contains("appezza")) Then
                DT.Columns("appezza").ColumnName = "Codice Appezzamento"
            End If

            If (DT.Columns.Contains("id_reg")) Then
                DT.Columns("id_reg").ColumnName = "Codice Impianto"
            ElseIf (DT.Columns.Contains("id_dest")) Then
                DT.Columns("id_dest").ColumnName = "Codice Impianto"
            End If

            If (DT.Columns.Contains("rag_soc")) Then
                DT.Columns("rag_soc").ColumnName = "Ragione Sociale Impresa"
            End If

            If (DT.Columns.Contains("codice_socio")) Then
                DT.Columns("codice_socio").ColumnName = "Codice Socio"
            End If

            If (DT.Columns.Contains("coop_padre")) Then
                DT.Columns("coop_padre").ColumnName = "Impresa Padre"
            End If

            If (DT.Columns.Contains("piva_padre")) Then
                DT.Columns("piva_padre").ColumnName = "Partita IVA Impresa Padre"
            End If

            If (DT.Columns.Contains("imp_ind_des")) Then
                DT.Columns("imp_ind_des").ColumnName = "Indirizzo Impresa"
            End If

            If (DT.Columns.Contains("imp_frz_des")) Then
                DT.Columns("imp_frz_des").ColumnName = "Frazione Impresa"
            End If

            If (DT.Columns.Contains("imp_cap")) Then
                DT.Columns("imp_cap").ColumnName = "CAP Impresa"
            End If

            If (DT.Columns.Contains("imp_com_des")) Then
                DT.Columns("imp_com_des").ColumnName = "Comune Impresa"
            End If

            If (DT.Columns.Contains("imp_pro_cod")) Then
                DT.Columns("imp_pro_cod").ColumnName = "Prov Impresa"
            End If

            If (DT.Columns.Contains("cen_ind_des")) Then
                DT.Columns("cen_ind_des").ColumnName = "Indirizzo Centro"
            End If

            If (DT.Columns.Contains("cen_frz_des")) Then
                DT.Columns("cen_frz_des").ColumnName = "Frazione Centro"
            End If

            If (DT.Columns.Contains("cen_cap")) Then
                DT.Columns("cen_cap").ColumnName = "CAP Centro"
            End If

            If (DT.Columns.Contains("cen_com_des")) Then
                DT.Columns("cen_com_des").ColumnName = "Comune Centro"
            End If

            If (DT.Columns.Contains("cen_pro_cod")) Then
                DT.Columns("cen_pro_cod").ColumnName = "Prov Centro"
            End If

            If (DT.Columns.Contains("pro_cod_istat")) Then
                DT.Columns("pro_cod_istat").ColumnName = "Prov ISTAT Centro"
            End If

            If (DT.Columns.Contains("com_cod_istat")) Then
                DT.Columns("com_cod_istat").ColumnName = "Com ISTAT Centro"
            End If

            If (DT.Columns.Contains("sa_nome")) Then
                DT.Columns("sa_nome").ColumnName = "Nome Centro"
            End If

            If (DT.Columns.Contains("campo_des")) Then
                DT.Columns("campo_des").ColumnName = "Nome Campo"
            End If

            If (DT.Columns.Contains("app_nome")) Then
                DT.Columns("app_nome").ColumnName = "Nome Appezzamento"
            End If

            If (DT.Columns.Contains("App_RifNum")) Then
                DT.Columns("App_RifNum").ColumnName = "Rif. Appezzamento"
            End If

            If (DT.Columns.Contains("sup_app")) Then
                DT.Columns("sup_app").ColumnName = "Superficie Appezzamento"
            End If

            If (DT.Columns.Contains("sup_trattata")) Then
                DT.Columns("sup_trattata").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sup_trattata").ColumnName = "Superficie Trattata"
            End If

            If (DT.Columns.Contains("qta_totale")) Then
                DT.Columns("qta_totale").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("qta_totale").ColumnName = "Quantita' Totale [Kg/L]"
            End If

            If (DT.Columns.Contains("gru_cod")) Then
                DT.Columns("gru_cod").ColumnName = "Codice Gruppo Vegetale"
            End If

            If (DT.Columns.Contains("gru_des")) Then
                DT.Columns("gru_des").ColumnName = "Gruppo Vegetale"
            End If

            If (DT.Columns.Contains("veg_cod")) Then
                DT.Columns("veg_cod").ColumnName = "Codice Specie Vegetale"
            End If

            If (DT.Columns.Contains("veg_des")) Then
                DT.Columns("veg_des").ColumnName = "Specie Vegetale"
            End If

            If (DT.Columns.Contains("cul_cod")) Then
                DT.Columns("cul_cod").ColumnName = "Codice Varieta'"
            End If

            If (DT.Columns.Contains("cul_des")) Then
                DT.Columns("cul_des").ColumnName = "Varieta'"
            End If

            If (DT.Columns.Contains("grva_des")) Then
                DT.Columns("grva_des").ColumnName = "Tipologia Varietale"
            End If

            If (DT.Columns.Contains("grfi_des")) Then
                DT.Columns("grfi_des").ColumnName = "Finalita'"
            End If

            If (DT.Columns.Contains("cop_des")) Then
                DT.Columns("cop_des").ColumnName = "Tipo Copertura"
            End If

            If (DT.Columns.Contains("reg_des")) Then
                DT.Columns("reg_des").ColumnName = "Regolamento"
            End If

            If (DT.Columns.Contains("sup_imp")) Then
                DT.Columns("sup_imp").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sup_imp").ColumnName = "Superficie Impianto [Ha]"
            End If

            If (DT.Columns.Contains("inizio_impianto")) Then
                DT.Columns("inizio_impianto").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("inizio_impianto").ColumnName = "Data Inizio Impianto"
            End If

            If (DT.Columns.Contains("fine_impianto")) Then
                DT.Columns("fine_impianto").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("fine_impianto").ColumnName = "Data Fine Impianto"
            End If

            If (DT.Columns.Contains("inizio_appezza")) Then
                DT.Columns("inizio_appezza").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("inizio_appezza").ColumnName = "Data Inizio Appezzamento"
            End If

            If (DT.Columns.Contains("fine_appezza")) Then
                DT.Columns("fine_appezza").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("fine_appezza").ColumnName = "Data Fine Appezzamento"
            End If

            If (DT.Columns.Contains("lav_cod_imp")) Then
                DT.Columns("lav_cod_imp").ColumnName = "Operazione Semina / Trapianto"
            End If

            If (DT.Columns.Contains("lav_cod")) Then
                DT.Columns("lav_cod").ColumnName = "Codice Operazione"
            End If

            If (DT.Columns.Contains("cau_mov")) Then
                DT.Columns("cau_mov").ColumnName = "Causale Movimento"
            End If

            If (DT.Columns.Contains("id_agenda")) Then
                DT.Columns("id_agenda").ColumnName = "Codice Agenda"
            End If

            If (DT.Columns.Contains("des_lib")) Then
                DT.Columns("des_lib").ColumnName = "Descrizione Operazione"
            End If

            If (DT.Columns.Contains("gruppo_operazione")) Then
                DT.Columns("gruppo_operazione").ColumnName = "Gruppo Operazione"
            End If

            If (DT.Columns.Contains("data_operazione")) Then
                DT.Columns("data_operazione").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("data_operazione").ColumnName = "Data Operazione"
            End If

            If (DT.Columns.Contains("data_semina")) Then
                DT.Columns("data_semina").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("data_semina").ColumnName = "Data Semina / Trapianto"
            End If

            If (DT.Columns.Contains("part_regione")) Then
                DT.Columns("part_regione").ColumnName = "Regione"
            End If

            If (DT.Columns.Contains("part_pro_cod")) Then
                DT.Columns("part_pro_cod").ColumnName = "Cod Prov"
            End If

            If (DT.Columns.Contains("part_com_des")) Then
                DT.Columns("part_com_des").ColumnName = "Descr Comune"
            End If

            If (DT.Columns.Contains("prov")) Then
                DT.Columns("prov").ColumnName = "Provincia"
            End If

            If (DT.Columns.Contains("com")) Then
                DT.Columns("com").ColumnName = "Comune"
            End If

            If (DT.Columns.Contains("sezione")) Then
                DT.Columns("sezione").ColumnName = "Sezione"
            End If

            If (DT.Columns.Contains("foglio")) Then
                DT.Columns("foglio").ColumnName = "Foglio"
            End If

            If (DT.Columns.Contains("numero")) Then
                DT.Columns("numero").ColumnName = "Numero"
            End If

            If (DT.Columns.Contains("subalterno")) Then
                DT.Columns("subalterno").ColumnName = "Subalterno"
            End If

            If (DT.Columns.Contains("area")) Then
                DT.Columns("area").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("area").ColumnName = "Superficie d'Intersezione con Particella [Ha]"
            End If

            If (DT.Columns.Contains("rappr_legale")) Then
                DT.Columns("rappr_legale").ColumnName = "Rappresentante Legale"
            End If

            If (DT.Columns.Contains("cf_legale")) Then
                DT.Columns("cf_legale").ColumnName = "Codice Fiscale Rappresentante Legale"
            End If

            If (DT.Columns.Contains("com_legale")) Then
                DT.Columns("com_legale").ColumnName = "Comune Nascita Rappresentante Legale"
            End If

            If (DT.Columns.Contains("pro_legale")) Then
                DT.Columns("pro_legale").ColumnName = "Provincia Nascita Rappresentante Legale"
            End If

            If (DT.Columns.Contains("tipo_impresa")) Then
                DT.Columns("tipo_impresa").ColumnName = "Tipo Impresa"
            End If

            If (DT.Columns.Contains("tipo_centro")) Then
                DT.Columns("tipo_centro").ColumnName = "Tipo Centro"
            End If

            If (DT.Columns.Contains("inizio_impresa")) Then
                DT.Columns("inizio_impresa").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("inizio_impresa").ColumnName = "Data Inizio Impresa"
            End If

            If (DT.Columns.Contains("fine_impresa")) Then
                DT.Columns("fine_impresa").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("fine_impresa").ColumnName = "Data Fine Impresa"
            End If

            If (DT.Columns.Contains("inizio_centro")) Then
                DT.Columns("inizio_centro").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("inizio_centro").ColumnName = "Data Inizio Centro"
            End If

            If (DT.Columns.Contains("fine_centro")) Then
                DT.Columns("fine_centro").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("fine_centro").ColumnName = "Data Fine Centro"
            End If

            If (DT.Columns.Contains("titolopossesso")) Then
                DT.Columns("titolopossesso").ColumnName = "Titolo Possesso Particella"
            End If

            If (DT.Columns.Contains("possesso_centro")) Then
                DT.Columns("possesso_centro").ColumnName = "Titolo Possesso Centro"
            End If

            If (DT.Columns.Contains("tipo_attivita")) Then
                DT.Columns("tipo_attivita").ColumnName = "Tipo Attivita'"
            End If

            If (DT.Columns.Contains("dal")) Then
                DT.Columns("dal").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("dal").ColumnName = "Data Inizio Possesso Particella"
            End If

            If (DT.Columns.Contains("al")) Then
                DT.Columns("al").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("al").ColumnName = "Data Fine Possesso Particella"
            End If

            If (DT.Columns.Contains("ettari")) Then
                DT.Columns("ettari").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("ettari").ColumnName = "Superficie [HA] Particella"
            End If

            If (DT.Columns.Contains("are")) Then
                DT.Columns("are").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("are").ColumnName = "Superficie [AA] Particella"
            End If

            If (DT.Columns.Contains("centiare")) Then
                DT.Columns("centiare").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("centiare").ColumnName = "Superficie [CA] Particella"
            End If

            If (DT.Columns.Contains("cod_ote")) Then
                DT.Columns("cod_ote").ColumnName = "Orientamento Tenico Economico"
            End If

            If (DT.Columns.Contains("cod_operatore")) Then
                DT.Columns("cod_operatore").ColumnName = "Codice Operatore"
            End If

            If (DT.Columns.Contains("cod_zoo")) Then
                DT.Columns("cod_zoo").ColumnName = "Codice Zooprofilattico"
            End If

            If (DT.Columns.Contains("cod_cerpl")) Then
                DT.Columns("cod_cerpl").ColumnName = "Codice CERPL"
            End If

            If (DT.Columns.Contains("cod_aua")) Then
                DT.Columns("cod_aua").ColumnName = "Codice AUA"
            End If

            If (DT.Columns.Contains("cod_ausl")) Then
                DT.Columns("cod_ausl").ColumnName = "Codice AUSL"
            End If

            If (DT.Columns.Contains("cod_cnal")) Then
                DT.Columns("cod_cnal").ColumnName = "Codice CNAL"
            End If

            If (DT.Columns.Contains("fabbricato")) Then
                DT.Columns("fabbricato").ColumnName = "Nome Fabbricato"
            End If

            If (DT.Columns.Contains("tipo_fabbricato")) Then
                DT.Columns("tipo_fabbricato").ColumnName = "Tipo Fabbricato"
            End If

            If (DT.Columns.Contains("sup_totale")) Then
                DT.Columns("sup_totale").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sup_totale").ColumnName = "Superficie Totale [Ha] (somma particelle)"
            End If

            If (DT.Columns.Contains("sup_sau")) Then
                DT.Columns("sup_sau").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sup_sau").ColumnName = "SAU Totale [Ha] (somma appezzamenti)"
            End If

            If (DT.Columns.Contains("sup_tara")) Then
                DT.Columns("sup_tara").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sup_tara").ColumnName = "Tara [Ha] (Sup Totale - SAU Totale)"
            End If

            If (DT.Columns.Contains("sau_convenz")) Then
                DT.Columns("sau_convenz").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sau_convenz").ColumnName = "SAU Convenzionale [Ha]"
            End If

            If (DT.Columns.Contains("sau_convers")) Then
                DT.Columns("sau_convers").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sau_convers").ColumnName = "SAU in Conversione [Ha]"
            End If

            If (DT.Columns.Contains("sau_bio")) Then
                DT.Columns("sau_bio").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sau_bio").ColumnName = "SAU Biologico [Ha]"
            End If

            If (DT.Columns.Contains("sup_bosco")) Then
                DT.Columns("sup_bosco").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sup_bosco").ColumnName = "Superficie Bosco [Ha]"
            End If

            If (DT.Columns.Contains("sup_prato")) Then
                DT.Columns("sup_prato").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("sup_prato").ColumnName = "Superficie Prato [Ha]"
            End If

            If (DT.Columns.Contains("mappa")) Then
                DT.Columns("mappa").ColumnName = "Mappe associate al Centro Aziendale"
            End If

            If (DT.Columns.Contains("organismi")) Then
                DT.Columns("organismi").ColumnName = "Organismi di Controllo Biologico"
            End If

            If (DT.Columns.Contains("capitolato_privato")) Then
                DT.Columns("capitolato_privato").ColumnName = "Capitolato Privato"
            End If

            If (DT.Columns.Contains("dett_specie_pers")) Then
                DT.Columns("dett_specie_pers").ColumnName = "Dettaglio Specie Personalizzato"
            End If

            If (DT.Columns.Contains("foral_des")) Then
                DT.Columns("foral_des").ColumnName = "Forma Allevamento"
            End If

            If (DT.Columns.Contains("resa_prevista")) Then
                DT.Columns("resa_prevista").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("resa_prevista").ColumnName = "Resa Prevista [Kg]"
            End If

            If (DT.Columns.Contains("p_ha")) Then
                DT.Columns("p_ha").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("p_ha").ColumnName = "Num. Piante/Ha"
            End If

            If (DT.Columns.Contains("p_tot")) Then
                DT.Columns("p_tot").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("p_tot").ColumnName = "Num. Piante Tot"
            End If

            If (DT.Columns.Contains("tra_fila_maschio")) Then
                DT.Columns("tra_fila_maschio").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("tra_fila_maschio").ColumnName = "Distanza Tra Fila"
            End If

            If (DT.Columns.Contains("su_fila_maschio")) Then
                DT.Columns("su_fila_maschio").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("su_fila_maschio").ColumnName = "Distanza Su Fila"
            End If

            If (DT.Columns.Contains("coop_referente")) Then
                DT.Columns("coop_referente").ColumnName = "Organismo Referente"
            End If

            If (DT.Columns.Contains("lotto_distinta")) Then
                DT.Columns("lotto_distinta").ColumnName = "Lotto Distinta"
            End If

            If (DT.Columns.Contains("inizio_distinta")) Then
                DT.Columns("inizio_distinta").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("inizio_distinta").ColumnName = "Data Inizio Distinta"
            End If

            If (DT.Columns.Contains("fine_distinta")) Then
                DT.Columns("fine_distinta").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("fine_distinta").ColumnName = "Data Fine Distinta"
            End If

            If (DT.Columns.Contains("bloccato")) Then
                DT.Columns("bloccato").ColumnName = "Operazione Bloccata"
            End If

            If (DT.Columns.Contains("data_bloccato")) Then
                DT.Columns("data_bloccato").ExtendedProperties("ExportFormat") = "Date"
                DT.Columns("data_bloccato").ColumnName = "Data Blocco Operazione"
            End If

            If (DT.Columns.Contains("tecnico_blocco")) Then
                DT.Columns("tecnico_blocco").ColumnName = "Tecnico Blocco Operazione"
            End If

            If (DT.Columns.Contains("categoria")) Then
                DT.Columns("categoria").ColumnName = "Categoria Prodotto"
            End If

            If (DT.Columns.Contains("prodotto")) Then
                DT.Columns("prodotto").ColumnName = "Prodotto / Materia Prima"
            End If

            If (DT.Columns.Contains("udm_des")) Then
                DT.Columns("udm_des").ColumnName = "Unita' di Misura"
            End If

            If (DT.Columns.Contains("qta")) Then
                DT.Columns("qta").ExtendedProperties("ExportFormat") = "Number"
                DT.Columns("qta").ColumnName = "Quantita'"
            End If

            If (DT.Columns.Contains("tecnico")) Then
                DT.Columns("tecnico").ColumnName = "Tecnico di Riferimento"
            End If

            If (DT.Columns.Contains("dest_uso")) Then
                DT.Columns("dest_uso").ColumnName = "Destinazione d'uso"
            ElseIf DT.Columns.Contains("Destinazione_Uso") Then
                DT.Columns("Destinazione_Uso").ColumnName = "Destinazione d'uso"
            End If

            If (DT.Columns.Contains("port_des")) Then
                DT.Columns("port_des").ColumnName = "Portinnesto"
            End If

            If (DT.Columns.Contains("imp_des")) Then
                DT.Columns("imp_des").ColumnName = "Impianto Irrigazione"
            End If

            If (DT.Columns.Contains("magazzino_conf")) Then
                DT.Columns("magazzino_conf").ColumnName = "Magazzino Conferimento"
            End If

            If (DT.Columns.Contains("principiattivi")) Then
                DT.Columns("principiattivi").ColumnName = "Principi attivi"
            End If

            If (DT.Columns.Contains("org_referente")) Then
                DT.Columns("org_referente").ColumnName = "Organismo Referente"
            End If

            If (DT.Columns.Contains("ind_des")) Then
                DT.Columns("ind_des").ColumnName = "Indirizzo"
            End If

            If (DT.Columns.Contains("frz_des")) Then
                DT.Columns("frz_des").ColumnName = "Frazione"
            End If

            If (DT.Columns.Contains("com_des")) Then
                DT.Columns("com_des").ColumnName = "Comune"
            End If

            If (DT.Columns.Contains("pro_cod")) Then
                DT.Columns("pro_cod").ColumnName = "Provincia"
            End If

            If (DT.Columns.Contains("metodoproduzione_des")) Then
                DT.Columns("metodoproduzione_des").ColumnName = "Metodo di Produzione"
            End If

            Dim objAllegato As New objAllegato
            Dim completeFilePath = AgronicaCoreGestioneRichieste.Esporta.EsportaExcelPath(DT, "RisultatoEsportazione", objAllegato, True)
            completeFilePath = Stringa_Codifica(completeFilePath, AgroKey_EncoderDecoder)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(objAllegato, System.Xml.Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    Private Shared Function Esportazione_Agenda(ByRef DT As DataTable, ByVal controlValues As ControlValues, ByRef msg As String, ByRef Messaggio As String, ByRef AgroMsg As String, ByRef Log As String, ByVal esportazioneNew As Boolean) As String

        Dim NomeRoutine As String = "Esportatore_Universale_2.Esportazione_Agenda"
        Dim mezzo As String
        Dim num_righe_dt As Integer
        Dim Idtestata As Integer = 0
        Dim objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim stbQ As New System.Text.StringBuilder

        Dim flag_campo As Boolean = False
        Dim flag_CUAA As Boolean = False
        Dim flag_ZVN As Boolean = controlValues.ChkValue33
        Dim flag_ACA As Boolean = controlValues.ChkValue34

        Dim NomeDB_Utenti As String

        If (controlValues.ChkValue18 = True) Or
            (controlValues.ChkValue29 = True) Then

            Dim dataprov As New AgronicaCoreDataProvider.DataProvider


            NomeDB_Utenti = dataprov.NomeDataBase_FromStringaConnessione(objParametriUtenti.StringaConnessione)


        End If

        'Al posto del WHERE Faccio l'Insert in __tmp_Agenda per velocizzare la query
        Popola__tmp_Agenda(NomeRoutine, controlValues.Chiavi, Idtestata)

        '--------- LETTURA PRINCIPI ATTIVI
        Dim objPA As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R
        Dim DtPrincipiDes As DataTable
        If (controlValues.ChkValue22 = True) Or (controlValues.ChkValue23 = True) Or (controlValues.ChkValue24 = True) Then
            DtPrincipiDes = objPA.Leggi_Da_StrPa_Cod("", AGRODATAINIZIO, AGRODATAFINE,
                                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                     "", "pa_cod", objParametriServer)
        End If

        '---------------------------------------------
        ' VAI CON LA TERA-QUERY  AGENDA!!!!!!!!
        '---------------------------------------------
        Try
            stbQ.Length = 0
            stbQ.AppendLine("")

            stbQ.AppendLine(" ;WITH contributi_cte as (SELECT DISTINCT imp.Piva, imp.Sa_Cod, imp.Appezza, imp.Id_Reg, imp.Progetto_Cod, imp.Validita_Inizio, imp.Validita_Fine, c.ContributoDes")
            stbQ.AppendLine(" FROM Imprese_Progetti imp")
            stbQ.AppendLine(" INNER JOIN Imprese_ProgettiXContributi  ipc")
            stbQ.AppendLine(" on imp.Progetto_Cod = ipc.ProgettoCod")
            stbQ.AppendLine(" INNER JOIN Contributi c")
            stbQ.AppendLine(" on c.Tipo = ipc.ContributoTipo ")
            stbQ.AppendLine(" and c.Tipo = 1 ")
            stbQ.AppendLine(" and c.ContributoCod = ipc.ContributoCod)")

            stbQ.AppendLine(" Select DISTINCT CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Agenda.PIVA ELSE Imprese.partitaIvaReale END AS Piva, ")
            If (controlValues.ChkValue1 = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Imprese.rag_soc, ' ') AS rag_soc, ")
            End If
            stbQ.AppendLine(" Agenda.sa_cod, ")
            If (controlValues.ChkValue3 = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Centri_Aziendali.sa_nome, ' ') AS sa_nome, ")
            End If
            If (controlValues.ChkValue32 = True) Then
                flag_CUAA = True
                stbQ.AppendLine(" ISNULL(Imprese_Codici.Val_Cod, ' ') AS CUAA, ")
            End If
            If flag_ACA Then
                stbQ.AppendLine("  ISNULL(contributiTbl.ContributoDes, ' ') as ACA, ")
            End If
            If (controlValues.ChkValue4 = True) Then
                flag_campo = True
                If esportazioneNew Then
                    stbQ.AppendLine(" ISNULL(CONVERT(VARCHAR, Appezzamento.Campo_Cod), ' ') AS campo_cod, ")
                Else
                    stbQ.AppendLine(" ISNULL(Appezzamento.Campo_Cod, -1) AS campo_cod, ")
                End If
            End If
            If (controlValues.ChkValue5) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Campi.Campo_Des, ' ') AS campo_des, ")
            End If
            If (controlValues.ChkValue6) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Mov_Destinazioni.Appezza, -1) AS appezza, ")
            End If
            If (controlValues.ChkValue7) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome, ")
                stbQ.AppendLine(" ISNULL((select top 1 val_cod from Appezzamento_Codici where Reg_Impianti.PIVA = Appezzamento_Codici.PIVA AND Reg_Impianti.SA_COD = Appezzamento_Codici.Sa_Cod AND Appezzamento_Codici.Appezza = Reg_Impianti.Appezza  and id_cod=1104), ' ') AS App_RifNum   , ")
            End If
            If (controlValues.ChkValue8) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Mov_Destinazioni.Id_Destinazione, -1) AS id_dest, ")
            End If
            If (controlValues.ChkValue9) Or (controlValues.ChkValue11) Or (controlValues.ChkValue13) Then
                flag_campo = True
                If esportazioneNew Then
                    stbQ.AppendLine(" ISNULL(CONVERT(VARCHAR, Reg_Impianti.Sup_Imp), ' ') AS sup_imp, ")
                Else
                    stbQ.AppendLine(" ISNULL(Reg_Impianti.Sup_Imp, -1.0000) AS sup_imp, ")
                End If
                stbQ.AppendLine(" ISNULL((select top 1 val_cod from Reg_Impianti_Codici where Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.Sa_Cod AND Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg and id_cod=1061), ' ') AS tra_fila_maschio, ")
                stbQ.AppendLine(" ISNULL((select top 1 val_cod from Reg_Impianti_Codici where Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.Sa_Cod AND Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg and id_cod=1063), ' ') AS su_fila_maschio, ")
                stbQ.AppendLine(" ISNULL((SELECT TOP 1 ca.descrizione from Reg_Impianti_Codici ric INNER JOIN Codici_Anagrafe ca ON ric.Id_Cod = ca.codice WHERE ric.PIVA = Reg_Impianti.PIVA AND ric.Sa_Cod = Reg_Impianti.SA_COD AND ric.Appezza = Reg_Impianti.Appezza AND ric.Id_Reg = Reg_Impianti.Id_Reg AND ca.codice > 3000 AND ca.codice < 4000), ' ') as Destinazione_Uso, ")
            End If
            If (controlValues.ChkValue10) Then
                flag_campo = True
                If esportazioneNew Then
                    stbQ.AppendLine(" ISNULL(CONVERT(VARCHAR, Cultivar.Veg_Cod), ' ') AS veg_cod, ")
                Else
                    stbQ.AppendLine(" ISNULL(Cultivar.Veg_Cod, -1) AS veg_cod, ")
                End If
            End If
            If (controlValues.ChkValue9) Or (controlValues.ChkValue11) Or (controlValues.ChkValue13) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(SpecieVegetali.Veg_Des, ' ') AS veg_des, ")
            End If
            If (controlValues.ChkValue12) Then
                flag_campo = True
                If esportazioneNew Then
                    stbQ.AppendLine("  ISNULL(CONVERT(VARCHAR, Reg_Impianti.CUL_COD), ' ') AS cul_cod, ")
                Else
                    stbQ.AppendLine("  ISNULL(Reg_Impianti.CUL_COD, -1) AS cul_cod, ")
                End If
            End If
            If (controlValues.ChkValue9) Or (controlValues.ChkValue11) Or (controlValues.ChkValue13) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Cultivar.Cul_Des, ' ') AS cul_des, ")
            End If
            If (controlValues.ChkValue14) Then
                flag_campo = True
                'StrSQL += " ISNULL(GruppoVarietale.Grva_Des, ' ') AS grva_des, "
                stbQ.AppendLine(" ISNULL(GruppoVarietale.Grva_Des,ISNULL( (select grva_des + ' -- Ibrido ' from GruppoVarietale where GruppoVarietale.Grva_cod = (0- Reg_Impianti.GRVA_Cod_VEG)), ' ')) AS grva_des, ")
            End If
            If (controlValues.ChkValue15) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(GruppoFinalita.Grfi_Des, ' ') AS grfi_des, ")
            End If
            If (controlValues.ChkValue16) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Inizio, 103), ' ') AS inizio_impianto, ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), ' ') AS fine_impianto, ")
            End If
            If (controlValues.ChkValue17) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(( SELECT    TOP 1 Regolamenti.Reg_Des ")
                stbQ.AppendLine("             FROM    Imprese_Progetti ")
                stbQ.AppendLine("             INNER JOIN  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod ")
                stbQ.AppendLine("             WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Validita_Inizio <= Movimenti.data_movimento ) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Validita_Fine >= Movimenti.data_movimento ) ")
                stbQ.AppendLine(" ) , ' ') AS Reg_Des, ")
            End If

            If (controlValues.ChkValue18) Then
                flag_campo = True
                'ISNULL(Operazioni.GRU_OP, -1) AS gru_op, 
                stbQ.AppendLine(" ISNULL(GruppoOperazioni.GRU_DES, ' ') AS gruppo_operazione, ")
            End If
            'If (Me.ChkGruppo2.Items.FindByValue(18).Selected = True) Then
            '    flag_campo = True
            '    stbQ.AppendLine("  ")
            'End If
            'If (Me.ChkGruppo2.Items.FindByValue(19).Selected = True) Then
            '    flag_campo = True
            '   stbQ.AppendLine("  ")
            'End If
            stbQ.AppendLine(" Agenda.id_agenda, Agenda.lav_cod, ISNULL(Agenda.des_lib, ' ') AS des_lib, ISNULL(Blocco_Flag, 0) AS Blocco_Flag, ' ' AS Bloccato, ISNULL(Blocco_Data, ' ') AS Blocco_Data, ' ' AS Data_Bloccato, ISNULL(Blocco_Username, ' ') AS Blocco_Username, ")
            If (controlValues.ChkValue19) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(DettagliBlocco.Cognome + ' ' + DettagliBlocco.Nome, ' ') AS Tecnico_Blocco, ")
            End If
            'stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), Movimenti.Data_Movimento, 103), ' ') AS data_operazione") 'NO VIRGOLA!
            stbQ.AppendLine(" ISNULL(Movimenti.Data_Movimento, '01/01/1900') AS data_operazione") 'NO VIRGOLA!


            'Me.ChkGruppo3.Items.FindByValue(22).Text = "Categoria e Prodotto / Materia Prima utilizzata"
            'Me.ChkGruppo3.Items.FindByValue(23).Text = "Unità di misura"
            If (controlValues.ChkValue22) Or (controlValues.ChkValue23) Or (controlValues.ChkValue24) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, ' ' AS Categoria, ' ' AS Prodotto, ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, ")
                stbQ.AppendLine(" ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des, ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des, ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des, ")
                stbQ.AppendLine(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo, ")
                stbQ.AppendLine(" ISNULL(Movimenti_dettagli.Udm_Cod, - 1) AS udm_cod, ISNULL(UnitaMisura1.UDM_SIM, ' ') AS udm_sim_1, ISNULL(Movimenti_dettagli.Extra_Int, - 1 ) AS extra_int, ISNULL(UnitaMisura2.UDM_SIM, ' ') AS udm_sim_2, ' ' AS Udm_Des, ISNULL(Movimenti_dettagli.Qta, 0) AS Qta, ISNULL(Movimenti.Mezzo, -1) AS mezzo ")
                '29/01/2020: nel metaschema castrato dei clienti non ci sono queste informazioni!
                'stbQ.AppendLine(" ISNULL(SUBSTRING( ")
                'stbQ.AppendLine( "         (")
                'stbQ.AppendLine("             SELECT ', '+ PrincipiAttivi.pa_des AS [text()] ")
                'stbQ.AppendLine("             FROM FormulatixPrincipiAttivi ")
                'stbQ.AppendLine(" 			LEFT OUTER JOIN PrincipiAttivi on FormulatixPrincipiAttivi.pa_Cod=PrincipiAttivi.pa_Cod ")
                'stbQ.AppendLine("             WHERE FormulatixPrincipiAttivi.Fr_Cod=Formulati.Fr_Cod ")
                'stbQ.AppendLine("             ORDER BY PrincipiAttivi.pa_des ")
                'stbQ.AppendLine("             FOR XML PATH ('') ")
                'stbQ.AppendLine("         ), 3, 1000),'') PrincipiAttivi ")
                stbQ.AppendLine(" , ISNULL(Movimenti_dettagli.PrincipiAttivi, '' ) AS PrincipiAttivi  ")
            End If

            If (controlValues.ChkValue25) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(Mov_Destinazioni.qta2, 0) AS sup_trattata ")
            End If

            If (controlValues.ChkValue26) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(Mov_Destinazioni.qta, 0) AS qta_totale ")
            End If

            If (controlValues.ChkValue27) Or flag_ZVN Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.PROV, ' ') AS PROV, ")
                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.COM, ' ')AS COM, ")
                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.SEZIONE, ' ') AS SEZIONE, ")
                If esportazioneNew Then
                    stbQ.AppendLine(" ISNULL(CONVERT(VARCHAR, AppezzamentiXParticelle.FOGLIO), ' ') AS FOGLIO, ")
                    stbQ.AppendLine(" ISNULL(CONVERT(VARCHAR, AppezzamentiXParticelle.NUMERO), ' ') AS NUMERO, ")
                Else
                    stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.FOGLIO, -1) AS FOGLIO, ")
                    stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.NUMERO, -1) AS NUMERO, ")
                End If
                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.SUBALTERNO, ' ') AS SUBALTERNO")
            End If
            If (controlValues.ChkValue28) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                If esportazioneNew Then
                    stbQ.AppendLine(" ISNULL(CONVERT(VARCHAR, ParticelleCatastali.ETTARI), ' ') AS ETTARI, ISNULL(CONVERT(VARCHAR, ParticelleCatastali.[ARE]), ' ') AS ARE, ISNULL(CONVERT(VARCHAR, ParticelleCatastali.CENTIARE), ' ') AS CENTIARE")
                Else
                    stbQ.AppendLine(" ISNULL(ParticelleCatastali.ETTARI, -1) AS ETTARI, ISNULL(ParticelleCatastali.[ARE], -1) AS ARE, ISNULL(ParticelleCatastali.CENTIARE, -1) AS CENTIARE")
                End If
            End If
            If (controlValues.ChkValue29) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                If esportazioneNew Then
                    stbQ.AppendLine(" CASE ISNULL(ImpreseXParticelle.TitoloPossesso, -1) ")
                    stbQ.AppendLine("   WHEN -1 THEN ' '")
                    stbQ.AppendLine("   WHEN 0 THEN 'Altro'")
                    stbQ.AppendLine("   WHEN 1 THEN 'Proprieta'''")
                    stbQ.AppendLine("   WHEN 2 THEN 'Comodato d''uso'")
                    stbQ.AppendLine("   WHEN 3 THEN 'Affitto con contratto'")
                    stbQ.AppendLine("   WHEN 4 THEN 'Affitto senza contratto'")
                    stbQ.AppendLine("   WHEN 5 THEN 'In conto terzi'")
                    stbQ.AppendLine("   ELSE ' '")
                    stbQ.AppendLine(" END AS TitoloPossesso, ")
                Else
                    stbQ.AppendLine(" ISNULL(ImpreseXParticelle.TitoloPossesso, -1) AS TitoloPossesso, ")
                End If
                stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Inizio, 103), ' ') AS dal, ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Fine, 103), ' ') AS al")
            End If
            If (controlValues.ChkValue30) Then
                'superficie di intersezione
                flag_campo = True
                stbQ.AppendLine(", ")
                If esportazioneNew Then
                    stbQ.AppendLine(" ISNULL(CONVERT(VARCHAR, dbo.AppezzamentiXParticelle.AREA), ' ') AS AREA")
                Else
                    stbQ.AppendLine(" ISNULL(dbo.AppezzamentiXParticelle.AREA, - 1) AS AREA")
                End If
            End If
            If (flag_ZVN) Then
                stbQ.AppendLine(" , CASE")
                stbQ.AppendLine("   WHEN zp.Zona_Cod IS NULL THEN 'NO'")
                stbQ.AppendLine("   ELSE 'SI'")
                stbQ.AppendLine(" END as ZVN")
            End If

            If (controlValues.ChkValue31) Then
                flag_campo = True
                stbQ.AppendLine(" , ISNULL(Movimenti.Mov_Desc, ' ') AS Note, ISNULL(Agenda.Username_Creazione, ' ') AS Username_Creazione ")
                stbQ.AppendLine(" , Dettagli.Cognome + ' ' + Dettagli.Nome AS Tecnico ")
            End If
            stbQ.AppendLine(" ")

            'FROM
            'FROM
            stbQ.AppendLine(" FROM Agenda ")

            'appezza, id_destinazione, data_movimento, mov_desc
            stbQ.AppendLine(" INNER JOIN Movimenti ")
            stbQ.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            stbQ.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stbQ.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            stbQ.AppendLine(" INNER JOIN dbo.Mov_Destinazioni ")
            stbQ.AppendLine(" ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

            If (controlValues.ChkValue22) Or (controlValues.ChkValue23) Or (controlValues.ChkValue24) Then
                flag_campo = True
                stbQ.AppendLine(" LEFT OUTER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod AND Movimenti_dettagli.elem_cod=191")
                stbQ.AppendLine(" LEFT OUTER JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod AND Movimenti_dettagli.elem_cod=3 ")
                stbQ.AppendLine(" LEFT OUTER JOIN Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD AND Movimenti_dettagli.elem_cod=197 ")
                stbQ.AppendLine(" LEFT OUTER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND ")
                stbQ.AppendLine(" Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND (Movimenti_dettagli.elem_cod=10 OR Movimenti_dettagli.elem_cod=201 OR Movimenti_dettagli.elem_cod=210) ")
                stbQ.AppendLine(" LEFT OUTER JOIN UnitaMisura UnitaMisura1 ON Movimenti_dettagli.Udm_Cod  = UnitaMisura1.UDM_COD ")
                stbQ.AppendLine(" LEFT OUTER JOIN UnitaMisura UnitaMisura2 ON Movimenti_dettagli.Extra_Int = UnitaMisura2.UDM_COD ")
            End If

            'sup imp, cul cod, inizio impianto, fine impianto
            stbQ.AppendLine(" INNER JOIN Reg_Impianti ")
            stbQ.AppendLine(" ON Reg_Impianti.PIVA = Agenda.PIVA AND Reg_Impianti.SA_COD = Agenda.Sa_Cod AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza ")

            If flag_ACA Then
                stbQ.AppendLine("Left Join ( ")
                stbQ.AppendLine("   SELECT ")
                stbQ.AppendLine("        c1.Piva,")
                stbQ.AppendLine("        c1.Sa_Cod,")
                stbQ.AppendLine("        c1.Appezza,")
                stbQ.AppendLine("        c1.Id_Reg,")
                stbQ.AppendLine("        c1.Progetto_Cod,")
                stbQ.AppendLine("        c1.Validita_Inizio,")
                stbQ.AppendLine("        c1.Validita_Fine,")
                stbQ.AppendLine("        COALESCE( ")
                stbQ.AppendLine("                STUFF((")
                stbQ.AppendLine("                      SELECT ', ' + c2.ContributoDes")
                stbQ.AppendLine("                      FROM contributi_cte c2")
                stbQ.AppendLine("                      WHERE c2.Piva = c1.Piva")
                stbQ.AppendLine("                      AND c2.Sa_Cod = c1.Sa_Cod ")
                stbQ.AppendLine("                      AND c2.Appezza = c1.Appezza ")
                stbQ.AppendLine("                      AND c2.Id_Reg = c1.Id_Reg ")
                stbQ.AppendLine("                      AND c2.Progetto_Cod = c1.Progetto_Cod")
                stbQ.AppendLine("                      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ' '), ")
                stbQ.AppendLine("                      ' ') AS ContributoDes")
                stbQ.AppendLine("    From contributi_cte c1")
                stbQ.AppendLine("    GROUP BY c1.Piva, c1.Sa_Cod, c1.Appezza, c1.Id_Reg, c1.Progetto_Cod, c1.Validita_Inizio, c1.Validita_Fine")
                stbQ.AppendLine(") contributiTbl")
                stbQ.AppendLine("On Reg_Impianti.piva = contributiTbl.piva")
                stbQ.AppendLine("And Reg_Impianti.sa_cod = contributiTbl.sa_cod")
                stbQ.AppendLine("And Reg_Impianti.appezza = contributiTbl.appezza")
                stbQ.AppendLine("And Reg_Impianti.id_reg = contributiTbl.id_reg")
                stbQ.AppendLine("And Agenda.Validita_Inizio >= contributiTbl.Validita_Inizio")
                stbQ.AppendLine("And Agenda.Validita_Inizio <= contributiTbl.Validita_Fine")

            End If
            If (controlValues.ChkValue1) Then
                'rag soc
                stbQ.AppendLine(" INNER JOIN Imprese ON Imprese.PIVA = Agenda.PIVA ")
            End If
            If (controlValues.ChkValue3) Then
                'sa_nome
                stbQ.AppendLine(" INNER JOIN Centri_Aziendali ")
                stbQ.AppendLine(" On Agenda.Sa_Cod = Centri_Aziendali.sa_cod And Agenda.PIVA = Centri_Aziendali.PIVA ")
            End If
            If (controlValues.ChkValue18) Then
                'gruppo operazione
                stbQ.AppendLine(" LEFT OUTER JOIN Operazioni ")
                stbQ.AppendLine(" On Operazioni.LAV_COD = Agenda.Lav_Cod")
                stbQ.AppendLine(" LEFT OUTER JOIN GruppoOperazioni On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
            End If
            If (controlValues.ChkValue7) Or (controlValues.ChkValue4) Or (controlValues.ChkValue5) Then
                'campo cod, campo des, app nome
                stbQ.AppendLine(" INNER JOIN Appezzamento ")
                stbQ.AppendLine(" On Mov_Destinazioni.Appezza = Appezzamento.Appezza And Appezzamento.PIVA = Agenda.PIVA And Appezzamento.SA_COD = Agenda.Sa_Cod ")
                stbQ.AppendLine(" LEFT OUTER JOIN Campi On Appezzamento.Campo_Cod = Campi.Campo_Cod ")
                stbQ.AppendLine(" And Appezzamento.piva = Campi.piva ")
                stbQ.AppendLine(" And Appezzamento.sa_cod = Campi.sa_cod ")
            End If
            If (controlValues.ChkValue14) Then
                'gruppo varietale
                stbQ.AppendLine(" LEFT OUTER JOIN GruppoVarietale On Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod ")
            End If
            If (controlValues.ChkValue15) Then
                'gruppo finalità
                stbQ.AppendLine(" LEFT OUTER JOIN GruppoFinalita On Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod ")
            End If

            If (controlValues.ChkValue13) Or (controlValues.ChkValue10) Or (controlValues.ChkValue11) Or (controlValues.ChkValue9) Then
                'cul des, veg cod, veg des,  sup imp
                stbQ.AppendLine(" LEFT OUTER JOIN Cultivar On Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            End If
            If (controlValues.ChkValue11) Or (controlValues.ChkValue9) Or (controlValues.ChkValue13) Then
                'veg des, sup imp, cul des
                stbQ.AppendLine(" LEFT OUTER JOIN SpecieVegetali On SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            End If
            If (controlValues.ChkValue27) Or (controlValues.ChkValue28) Or (controlValues.ChkValue29) Or (controlValues.ChkValue31) Or flag_ZVN Then
                'particelle, area (sup intersezione), titolo possesso, inizio e fine possesso, ettari, are, centiare (sup particella)
                stbQ.AppendLine(" LEFT OUTER JOIN AppezzamentiXParticelle ")
                stbQ.AppendLine(" On Reg_Impianti.PIVA = AppezzamentiXParticelle.PIVA And Reg_Impianti.SA_COD = AppezzamentiXParticelle.SA_COD And Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza ")
            End If
            If flag_ZVN Then
                stbQ.AppendLine(" LEFT JOIN ZonexParticelle zp")
                stbQ.AppendLine($" On AppezzamentiXParticelle.PROV = zp.PROV And AppezzamentiXParticelle.COM = zp.COM And AppezzamentiXParticelle.SEZIONE = zp.SEZIONE  And AppezzamentiXParticelle.FOGLIO = zp.FOGLIO  And AppezzamentiXParticelle.NUMERO = zp.NUMERO  And AppezzamentiXParticelle.SUBALTERNO = zp.SUBALTERNO And zp.Zona_Cod = {CInt(enum_Zone.ZVN)} ")
            End If
            If (controlValues.ChkValue28) Or (controlValues.ChkValue29) Or (controlValues.ChkValue30) Then
                'titolo possesso, inizio e fine possesso, ettari, are, centiare (sup particella)
                stbQ.AppendLine(" LEFT OUTER JOIN ImpreseXParticelle ")
                stbQ.AppendLine(" On ImpreseXParticelle.PIVA = AppezzamentiXParticelle.PIVA ")
                stbQ.AppendLine(" And ImpreseXParticelle.sa_cod = AppezzamentiXParticelle.SA_COD ")
                stbQ.AppendLine(" And ImpreseXParticelle.PROV = AppezzamentiXParticelle.PROV And ImpreseXParticelle.COM = AppezzamentiXParticelle.COM ")
                stbQ.AppendLine(" And ImpreseXParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE And ImpreseXParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO ")
                stbQ.AppendLine(" And ImpreseXParticelle.NUMERO = AppezzamentiXParticelle.NUMERO And ImpreseXParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO ")
            End If
            If (controlValues.ChkValue27) Then
                'ettari, are, centiare (sup particella)
                stbQ.AppendLine(" LEFT OUTER JOIN  ParticelleCatastali ")
                stbQ.AppendLine(" On ImpreseXParticelle.PROV = ParticelleCatastali.PROV And ImpreseXParticelle.COM = ParticelleCatastali.COM ")
                stbQ.AppendLine(" And ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE And ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                stbQ.AppendLine(" And ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO And ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
            End If

            If (controlValues.ChkValue19) Then
                stbQ.AppendLine(" LEFT OUTER JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli DettagliBlocco ")
                stbQ.AppendLine(" On DettagliBlocco.Username = Agenda.Blocco_Username  ")
            End If

            If (flag_CUAA) Then
                stbQ.AppendLine(" LEFT JOIN Imprese_Codici")
                stbQ.AppendLine($" On Imprese_Codici.Piva = Imprese.Piva And Imprese_Codici.Id_Cod = {CInt(enum_CodiciAnagrafe.CodiceCUAA)} ")
            End If

            If (controlValues.ChkValue31) Then
                stbQ.AppendLine(" INNER JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli ")
                stbQ.AppendLine(" On Dettagli.CodFisc = Agenda.Username_Creazione ")
            End If


            'StrSQL &= vbCrLf

            'WHERE
            'stbQ.AppendLine(" WHERE               ((Agenda.PIVA  ")
            'stbQ.AppendLine("                     + '_' + CONVERT(varchar(10), Agenda.SA_COD)  ")
            'stbQ.AppendLine("                     + '_' + CONVERT(varchar(10), Agenda.Id_Agenda)  ")
            'stbQ.AppendLine("                     + '_' + CONVERT(varchar(10), Agenda.lav_cod))  ")
            'stbQ.AppendLine("                     IN (" & ElencoChiaviAgenda & "))  ")

            'Filtro Where sostuito con tabella temporanea
            'stbQ.AppendLine(" WHERE               Agenda.PIVA  ")
            'stbQ.AppendLine("                     + '_' + CONVERT(varchar(10), Agenda.SA_COD)  ")
            'stbQ.AppendLine("                     + '_' + CONVERT(varchar(10), Agenda.Id_Agenda)  ")
            'stbQ.AppendLine("                     IN (" & ElencoChiaviAgenda & ")  ")

            If Idtestata <> 0 Then

                stbQ.AppendLine(" inner Join __Tmp_Agenda a ")
                stbQ.AppendLine(" On  Agenda.PIVA = a.piva  ")
                stbQ.AppendLine(" And Agenda.Id_Agenda = a.Id_Agenda ")
                stbQ.AppendLine("       And a.idTestataTemp = " & Idtestata)

            End If

            'ORDINAMENTO
            If (controlValues.ChkValue1) Then
                stbQ.AppendLine(" ORDER BY rag_soc ")
                If (controlValues.ChkValue20) Then
                    stbQ.AppendLine(" , data_operazione ")
                End If
                stbQ.AppendLine(" ASC ")
            ElseIf (controlValues.ChkValue20) Then
                stbQ.AppendLine(" ORDER BY data_operazione ASC ")
            End If

            'StrSQL += vbCrLf

            'Recupero il datatable
            'Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
            '                      Session("ASG_Connessione_Server"),
            '                      StrSQL,
            '                      0,
            '                      Messaggio)

            Dim dataProvider As New AgronicaCoreDataProvider.DataProvider

            '--------------------------------------------------------------------------
            DT = dataProvider.EseguiQuery_Lettura(objParametriServer, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Dim tmp_Agenda_W As New AgronicaCoreVarieDAL.__tmp_Agenda_W

            tmp_Agenda_W.CancellaRecordDaIDTestataTemp(Idtestata, objParametriServer)

            'If (Not IsNothing(Rs)) AndAlso
            '    (Rs.State <> 0) AndAlso
            '        (Not Rs.EOF) Then

            If Not IsNothing(DT) AndAlso
               DT.Rows.Count <> 0 Then


                'DT = objSQL.CreateDT_from_RS(Rs, errore)

                ''basta clonarlo una sola volta
                'If i = 0 Then
                '    DT_Finale = DT.Clone
                'End If

                num_righe_dt = DT.Rows.Count

                ' la j scorre i record
                For j = 0 To num_righe_dt - 1

                    If (controlValues.ChkValue1) Then
                        If (CStr(DT.Rows(j).Item("rag_soc")) = "") Then
                            DT.Rows(j).Item("rag_soc") = " "
                        End If
                    End If

                    If (controlValues.ChkValue3) Then
                        If (CStr(DT.Rows(j).Item("sa_nome")) = "") Then
                            DT.Rows(j).Item("sa_nome") = " "
                        End If
                    End If

                    If (controlValues.ChkValue4) Then
                        If CStr(DT.Rows(j).Item("campo_cod")) = "" Then
                            DT.Rows(j).Item("campo_cod") = -1
                        End If
                    End If

                    If (controlValues.ChkValue5) Then
                        If (CStr(DT.Rows(j).Item("campo_des")) = "") Then
                            DT.Rows(j).Item("campo_des") = " "
                        End If
                    End If

                    If (controlValues.ChkValue6) Then
                        If CStr(DT.Rows(j).Item("appezza")) = "" Then
                            DT.Rows(j).Item("appezza") = -1
                        End If
                    End If

                    If (controlValues.ChkValue7) Then
                        If (CStr(DT.Rows(j).Item("app_nome")) = "") Then
                            DT.Rows(j).Item("app_nome") = " "
                        End If
                    End If

                    If (controlValues.ChkValue8) Then
                        If CStr(DT.Rows(j).Item("id_dest")) = "" Then
                            DT.Rows(j).Item("id_dest") = -1
                        End If
                    End If

                    'If (Me.ChkGruppo2.Items.FindByValue(10).Selected = True) Then
                    '    If CStr(DT.Rows(j).Item("gru_cod")) = "" Then
                    '        DT.Rows(j).Item("gru_cod") = -1
                    '    End If
                    'End If

                    'If (controlValues.ChkValue11) Then
                    '    If (CStr(DT.Rows(j).Item("gru_des")) = "") Then
                    '        DT.Rows(j).Item("gru_des") = " "
                    '    End If
                    'End If

                    If (controlValues.ChkValue10) Then
                        If (CStr(DT.Rows(j).Item("veg_cod")) = "") Then
                            DT.Rows(j).Item("veg_cod") = -1
                        End If
                    End If

                    If (controlValues.ChkValue9) Or
                            (controlValues.ChkValue11) Or
                                 (controlValues.ChkValue13) Then

                        If (CStr(DT.Rows(j).Item("veg_des")) = "") Then
                            DT.Rows(j).Item("veg_des") = " "
                        End If
                        If (CStr(DT.Rows(j).Item("cul_des")) = "") Then
                            DT.Rows(j).Item("cul_des") = " "
                        End If
                        If (CStr(DT.Rows(j).Item("sup_imp")) = "") Then
                            DT.Rows(j).Item("sup_imp") = -1.0
                        End If
                        If CStr(DT.Rows(j).Item("cul_des")) = " " Then
                            'se c'è la superficie, ma non la varietà, significa che è terreno nudo
                            If DT.Rows(j).Item("sup_imp") <> -1.0 Then
                                DT.Rows(j).Item("veg_des") = "Terreno Nudo"
                            End If
                        End If
                    End If

                    If (controlValues.ChkValue12) Then
                        If CStr(DT.Rows(j).Item("cul_cod")) = "" Then
                            DT.Rows(j).Item("cul_cod") = -1
                        End If
                    End If

                    If (controlValues.ChkValue14) Then
                        If (CStr(DT.Rows(j).Item("grva_des")) = "") Then
                            DT.Rows(j).Item("grva_des") = " "
                        End If
                    End If

                    If (controlValues.ChkValue15) Then
                        If (CStr(DT.Rows(j).Item("grfi_des")) = "") Then
                            DT.Rows(j).Item("grfi_des") = " "
                        End If
                    End If

                    If (controlValues.ChkValue17) Then
                        If (CStr(DT.Rows(j).Item("reg_des")) = "") Then
                            DT.Rows(j).Item("reg_des") = " "
                        End If
                    End If

                    If (controlValues.ChkValue19) Then
                        Select Case DT.Rows(j).Item("blocco_flag")
                            Case 0
                                DT.Rows(j).Item("bloccato") = "NO"
                            Case 1
                                DT.Rows(j).Item("bloccato") = "SI"
                            Case Else
                                DT.Rows(j).Item("bloccato") = " "
                        End Select
                        If CStr(DT.Rows(j).Item("blocco_data")) = "01/01/1900" Then
                            DT.Rows(j).Item("Data_Bloccato") = " "
                        Else
                            DT.Rows(j).Item("Data_Bloccato") = DT.Rows(j).Item("blocco_data")
                        End If
                    End If

                    If (controlValues.ChkValue21) Then
                        If (CStr(DT.Rows(j).Item("des_lib")) = "") Then
                            DT.Rows(j).Item("des_lib") = " "
                        End If
                    End If

                    If (controlValues.ChkValue22) Or
                            (controlValues.ChkValue23) Or
                            (controlValues.ChkValue24) Then

                        'If (Me.ChkGruppo3.Items.FindByValue(23).Selected = True) Or _
                        '    (Me.ChkGruppo3.Items.FindByValue(24).Selected = True) Then

                        Select Case DT.Rows(j).Item("mezzo")
                            Case enum_TipoMezzo.Indefinito  'Mezzo per le Materie Prime
                                mezzo = ""
                            Case enum_TipoMezzo.Ettolitro
                                mezzo = "/Hl"
                            Case enum_TipoMezzo.Ettaro
                                mezzo = "/Ha"
                            Case enum_TipoMezzo.Ora
                                mezzo = "/h"
                            Case enum_TipoMezzo.Mensile
                                mezzo = ""
                            Case enum_TipoMezzo.Complessivo
                                mezzo = "tot"
                        End Select

                        Select Case DT.Rows(j).Item("elem_cod")
                            Case 3
                                DT.Rows(j).Item("Categoria") = "Fertilizzanti"
                                DT.Rows(j).Item("Prodotto") = DT.Rows(j).Item("Fer_Des")
                                DT.Rows(j).Item("Udm_Des") = DT.Rows(j).Item("Udm_Sim_2") + mezzo
                            Case 10
                                DT.Rows(j).Item("Categoria") = "Sementi e materiale vivaisti"
                                DT.Rows(j).Item("Prodotto") = DT.Rows(j).Item("Mat_Des") + " (" + DT.Rows(j).Item("Cod_Articolo") + ")"
                                DT.Rows(j).Item("Udm_Des") = DT.Rows(j).Item("Udm_Sim_1")
                            Case 191
                                DT.Rows(j).Item("Categoria") = "Formulati"
                                DT.Rows(j).Item("Prodotto") = DT.Rows(j).Item("Fr_Des") + " (" + CStr(DT.Rows(j).Item("Pro_Cod")) + ")"
                                DT.Rows(j).Item("Udm_Des") = DT.Rows(j).Item("Udm_Sim_2") + mezzo
                            Case 197
                                DT.Rows(j).Item("Categoria") = "Trappole commerciali"
                                DT.Rows(j).Item("Prodotto") = DT.Rows(j).Item("Trap_Des")
                                DT.Rows(j).Item("Udm_Des") = DT.Rows(j).Item("Udm_Sim_1") + mezzo
                            Case 201
                                DT.Rows(j).Item("Categoria") = "Semilavorati Produzione Vegetale"
                                DT.Rows(j).Item("Prodotto") = DT.Rows(j).Item("Mat_Des") + " (" + DT.Rows(j).Item("Cod_Articolo") + ")"
                                DT.Rows(j).Item("Udm_Des") = DT.Rows(j).Item("Udm_Sim_1")
                        End Select

                        If DT.Rows(j).Item("PrincipiAttivi") <> "" Then
                            Dim strPriAtt As String = ""
                            Try
                                'esempio: 326§66.67|1024§4.44
                                '           223§27.9

                                Dim objHLP As New AgronicaCoreContabHLP.Contabilita
                                Dim strPa_Cod As String = DT.Rows(j).Item("PrincipiAttivi")
                                Dim p As Integer
                                Dim Principi() As String
                                Dim pa_cod As Integer
                                Dim pa_des As String
                                Dim titolo As String

                                Principi = Split(strPa_Cod, "|")
                                If Not Principi Is Nothing Then
                                    For p = 0 To Principi.Length - 1

                                        pa_cod = Split(Principi(p), "§")(0)
                                        titolo = Split(Principi(p), "§")(1)

                                        pa_des = objHLP.Des_from_Cod(DtPrincipiDes, "pa_cod", "pa_des", pa_cod)

                                        If IsNumeric(titolo) AndAlso CDec(titolo) <> 0 Then
                                            strPriAtt &= titolo.ToString & "% " & pa_des
                                        Else
                                            strPriAtt &= pa_des
                                        End If

                                        If p <> Principi.Length - 1 Then
                                            strPriAtt &= " - " & vbCrLf
                                        Else
                                            strPriAtt &= vbCrLf
                                        End If

                                    Next
                                End If

                            Catch ex As Exception

                            End Try
                            DT.Rows(j).Item("PrincipiAttivi") = strPriAtt

                        End If 'PrincipiAttivi

                    End If

                    If (controlValues.ChkValue18) Then
                        If (CStr(DT.Rows(j).Item("gruppo_operazione")) = "") Then
                            DT.Rows(j).Item("gruppo_operazione") = " "
                        End If
                    End If

                    If (controlValues.ChkValue27) Then
                        If (CStr(DT.Rows(j).Item("PROV")) = "") Then
                            DT.Rows(j).Item("PROV") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("COM")) = "") Then
                            DT.Rows(j).Item("COM") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("SEZIONE")) = "") Then
                            DT.Rows(j).Item("SEZIONE") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("FOGLIO")) = "") Then
                            DT.Rows(j).Item("FOGLIO") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("NUMERO")) = "") Then
                            DT.Rows(j).Item("NUMERO") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("SUBALTERNO")) = "") Then
                            DT.Rows(j).Item("SUBALTERNO") = " "
                        End If
                    End If

                    If (controlValues.ChkValue28) Then
                        If (CStr(DT.Rows(j).Item("ETTARI")) = "") Then
                            DT.Rows(j).Item("ETTARI") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("ARE")) = "") Then
                            DT.Rows(j).Item("ARE") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("CENTIARE")) = "") Then
                            DT.Rows(j).Item("CENTIARE") = -1
                        End If

                    End If

                    If (controlValues.ChkValue29) Then
                        If (CStr(DT.Rows(j).Item("TitoloPossesso")) = "") Then
                            DT.Rows(j).Item("TitoloPossesso") = -1
                        End If
                    End If

                    If (controlValues.ChkValue30) Then
                        If (CStr(DT.Rows(j).Item("AREA")) = "") Then
                            DT.Rows(j).Item("AREA") = -1.0
                        End If
                    End If

                    'DT_Finale.ImportRow(DT.Rows.Item(j))

                Next

                'CANCELLA LE INFORMAZIONI NON NECESSARIE

                'GRUPPO 1

                If controlValues.ChkValue0 Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("PIVA")
                End If

                If controlValues.ChkValue2 Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("sa_cod")
                End If

                '(18/07/2016 fede) eliminato su richiesta di fabrizio
                DT.Columns.Remove("id_agenda")
                'If Me.ChkGruppo2.Items.FindByValue(20).Selected = True Then
                '    flag_campo = True
                'Else
                '    DT.Columns.Remove("id_agenda")
                'End If

                'If Me.ChkGruppo3.Items.FindByValue(21).Selected = True Then
                '    flag_campo = True
                'Else
                '    DT.Columns.Remove("lav_cod")
                'End If

                If controlValues.ChkValue19 Then
                    flag_campo = True
                    DT.Columns.Remove("blocco_flag")
                    DT.Columns.Remove("blocco_data")
                    DT.Columns.Remove("blocco_username")
                Else
                    DT.Columns.Remove("blocco_flag")
                    DT.Columns.Remove("blocco_data")
                    DT.Columns.Remove("blocco_username")
                    DT.Columns.Remove("Bloccato")
                    DT.Columns.Remove("Data_Bloccato")
                End If

                DT.Columns.Remove("lav_cod")

                If controlValues.ChkValue21 Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("des_lib")
                End If

                If (controlValues.ChkValue22) Or (controlValues.ChkValue23) Or (controlValues.ChkValue24) Then
                    DT.Columns.Remove("elem_cod")
                    DT.Columns.Remove("mat_cod")
                    DT.Columns.Remove("pro_cod")
                    DT.Columns.Remove("fr_des")
                    DT.Columns.Remove("fer_des")
                    DT.Columns.Remove("trap_des")
                    DT.Columns.Remove("mat_des")
                    DT.Columns.Remove("cod_articolo")

                    DT.Columns.Remove("udm_cod")
                    DT.Columns.Remove("udm_sim_1")
                    DT.Columns.Remove("extra_int")
                    DT.Columns.Remove("udm_sim_2")
                    DT.Columns.Remove("mezzo")

                    If Not controlValues.ChkValue22 Then
                        DT.Columns.Remove("categoria")
                        DT.Columns.Remove("prodotto")
                    End If

                    If Not controlValues.ChkValue23 Then
                        DT.Columns.Remove("udm_des")
                    End If

                    If Not controlValues.ChkValue24 Then
                        DT.Columns.Remove("qta")
                    End If
                End If

                If controlValues.ChkValue20 Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("data_operazione")
                End If

                If controlValues.ChkValue31 Then
                    DT.Columns.Remove("username_creazione")
                End If

                If (controlValues.ChkValue9) Or (controlValues.ChkValue11) Or (controlValues.ChkValue13) Then

                    If Not controlValues.ChkValue11 Then
                        DT.Columns.Remove("veg_des")
                    End If

                    If Not controlValues.ChkValue13 Then
                        DT.Columns.Remove("cul_des")
                    End If

                    If Not controlValues.ChkValue9 Then
                        DT.Columns.Remove("sup_imp")
                    End If

                End If

                If flag_campo = False Then
                    AgroMsg = "Selezionare almeno un campo di cui si vuole fare l'esportazione!!"
                    Return AgroMsg
                    Exit Function
                End If

                'Rs.Close() 'non serve!

            Else 'nothing rs - state - eof

                'NON CI SONO MOVIMENTI!!!

                'Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative al centro: " + piva + " - " + CStr(sa_cod) + ", all'id_agenda: " + CStr(id_agenda) + " e al lav_cod: " + CStr(lav_cod) + ". " & vbCrLf & vbCrLf
                Log += CStr(Date.Now) + "   La TERA QUERY AGENDA non ha restituito alcun risultato per il seguente elenco di chiavi agenda: " + controlValues.Chiavi.ToString() & vbCrLf & vbCrLf

            End If 'nothing

        Catch ex As Exception
            Log += CStr(Date.Now) + "   Errore nella TERA QUERY AGENDA: " + ex.Message & vbCrLf & vbCrLf
        End Try



        'Next ' for num_elementi 
        'ORA LA QUERY VIENE ESEGUITA SOLO UNA VOLTA, NON PIU' DENTRO AL FOR

        '------------------------------------------------
        '-------------- FINE CICLO AGENDA ---------------
        '------------------------------------------------

        Return AgroMsg

    End Function

    '###################################################################################
    Private Sub Esportazione_START(Optional ByVal chiavi As List(Of String) = Nothing)


        '########################################################################
        '''''''''''''''''''''''''''''' CONTROLLI ''''''''''''''''''''''''''''''''
        '########################################################################

        Dim Messaggio As String = ""

        '************************
        '   A = EXCEL
        '   B = XML
        '   C = XML RINTRACCIO
        '   D = ACCESS
        '   E = TESTO CAMPO FISSO
        '   F = TESTO CSV
        '************************

        Select Case Me.RdBList_Esporta.SelectedValue

            Case "A"

                If ViewState("scheda") = "rintraccio" Then
                    Messaggio = "Selezionare l'esportazione XML Rintraccio!" & vbCrLf
                End If

            Case "C"

                If ViewState("scheda") <> "rintraccio" Then
                    Messaggio = "E' attivata solo l'esportazione Excel!"
                End If

            Case "B"

                If ViewState("scheda") = "rintraccio" Then
                    Messaggio = "Selezionare l'esportazione XML Rintraccio!" & vbCrLf
                End If

            Case "D", "E", "F"

                If ViewState("scheda") = "rintraccio" Then
                    Messaggio = "Selezionare l'esportazione XML Rintraccio!" & vbCrLf
                Else
                    Messaggio = "E' attivata solo l'esportazione Excel!"
                End If


            Case Else

                Messaggio = "Selezionare il tipo di formato in cui si vuole esportare l'output!!" & vbCrLf

        End Select

        '-----------------------------------------------------------------------------

        Dim validita_inizio As String

        If ViewState("scheda") = "rintraccio" Then

            If Me.Txt_ValiditaInizio.Text = "" Then
                Messaggio += "Inserire la validità inizio degli impianti!" & vbCrLf
            Else
                validita_inizio = CStr(Me.Txt_ValiditaInizio.Text)
            End If

            If Me.Cmb_Punti.SelectedValue = -1 Then
                Messaggio = "Selezionare il Punto del Sistema di Tracciabilità dal menu' a tendina!"
            End If

        End If 'rintraccio



        If Messaggio <> "" Then
            AgroMsgBox(Messaggio, Page)
            Exit Sub
        End If

        'AL MOMENTO DISATTIVO   -> SE SERVE E' DA ATTIVARE
        'Select Case Me.RdBList_Esporta.SelectedValue

        '    Case "A", "C"

        '    Case Else

        '        If Me.Txt_FileOutput.Text <> "" Then
        '            NomeFileOutput = Me.Txt_FileOutput.Text
        '        Else
        '            AgroMsgBox("Inserire il nome da attribuire al file di output!!!", Page)
        '            Exit Sub
        '        End If

        'End Select

        '-------------------------------------------------


        '########################################################################
        '''''''''''''''''''''''' PREPARAZIONE STRINGA XML '''''''''''''''''''''''
        '########################################################################

        Dim strXmlVariabili As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim DT As New DataTable
        Dim Log As String = ""


        strXmlVariabili = Session("strXmlVariabilistampe")

        'modifica del 30/05/2011: la stringa MAGA viene già rimossa dalla pagina gestione richieste

        ''Se il primo carattere è MAGA
        ''e devo fare il replace degl nome degli attributi
        'If Left(strXmlVariabili, 4) = "MAGA" Then

        '   strXmlVariabili = Mid(strXmlVariabili, 5)

        strXmlVariabili = strXmlVariabili.Replace("<VS", "<VariabiliStampe")


        Select Case ViewState("scheda")

            Case "impianti"

                strXmlVariabili = strXmlVariabili.Replace("s=", "sa_cod=")
                strXmlVariabili = strXmlVariabili.Replace("a=", "appezza=")
                strXmlVariabili = strXmlVariabili.Replace("r=", "id_reg=")
                strXmlVariabili = strXmlVariabili.Replace("p=", "piva=")

            Case "agenda"

                strXmlVariabili = strXmlVariabili.Replace("s=", "sa_cod=")
                strXmlVariabili = strXmlVariabili.Replace("i=", "id_agenda=")
                'strXmlVariabili = strXmlVariabili.Replace("l=", "lav_cod=")
                strXmlVariabili = strXmlVariabili.Replace("p=", "piva=")

            Case "centri"

                strXmlVariabili = strXmlVariabili.Replace("s=", "sa_cod=")
                strXmlVariabili = strXmlVariabili.Replace("p=", "piva=")

            Case "imprese", "rintraccio"

                strXmlVariabili = strXmlVariabili.Replace("p=", "piva=")

        End Select


        'End If


        '----------------------------------------------------------------


        If (Not IsNothing(strXmlVariabili)) And (strXmlVariabili <> "") Then

            'Carico la stringa xml in un nuovo documento xml
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXmlVariabili)

            If XmlDoc.HasChildNodes Then


                '########################################################################
                '''''''''''''''''''''''' RECUPERA I DATI DA ESPORTARE '''''''''''''''''''
                '########################################################################


                Select Case ViewState("scheda")


                    '---------------------------------------------
                    '-------------- IMPIANTI ---------------------
                    '---------------------------------------------
                    Case "impianti"

                        Messaggio = Esportazione_Impianti(XmlDoc, DT, Log)


                        '==========================================================================


                        '---------------------------------------
                        '---------- IMPRESE --------------------
                        '---------------------------------------
                    Case "imprese"


                        Messaggio = Esportazione_Imprese(XmlDoc, DT, Log)


                        '==========================================================================

                        '---------------------------------------------
                        '------------- CENTRI AZIENDALI --------------
                        '---------------------------------------------
                    Case "centri"

                        Messaggio = Esportazione_Centri(XmlDoc, DT, Log)


                        '==========================================================================


                        '---------------------------------------------
                        '-------------- AGENDA ---------------------
                        '---------------------------------------------
                    Case "agenda"

                        Messaggio = Esportazione_Agenda(XmlDoc, DT, Log, chiavi)


                        '==========================================================================


                        '---------------------------------------------
                        '-------------- RINTRACCIO -------------------
                        '---------------------------------------------
                    Case "rintraccio"

                        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

                        Dim UtenteAbilitato As Boolean
                        Dim strDummy As String      'controllo accesso negato.....

                        UtenteAbilitato = Controlla_Permessi_Utente_2(
                                                    Server, Session, Page,
                                                    Session("ASG_Utente_Username"),
                                                    Session("ASG_IdServizio"),
                                                    enum_Security_Attivita.Stampe_Esportazione_Rintraccio,
                                                    enum_Security_Operazione.Modifica,
                                                    strDummy)

                        If UtenteAbilitato = True Then

                            '////////////////////////////////////////////////////////////////
                            '///////////////// VERIFICA FILE E CARTELLE /////////////////////
                            '////////////////////////////////////////////////////////////////

                            Try

                                Dim PathCompletoRintraccio, StrErroreRintraccio, PathFinaleRintraccio As String
                                Dim NomeCartella As String = ""

                                If Me.Cmb_Punti.SelectedValue = 99 Then
                                    '/************ PISELLO/FAGIOLO **********************/
                                    NomeCartella = "Pisello_Fagiolo"
                                Else '/************ POMODORO **********************/
                                    NomeCartella = "Pomodoro"
                                End If

                                'GestioneFile_CreaCartellaNelPathWebConfig("Path_Esportazioni_Stampe", "Esportazione_Rintraccio_" + CStr(Date.Today.Year), PathCompletoRintraccio, StrErroreRintraccio)
                                AgronicaCoreDataProvider.GestioneFile.CreaCartellaNelPath(objParametri_Server.LogDirectory, "Esportazione_Rintraccio_" + CStr(Date.Today.Year), PathCompletoRintraccio, StrErroreRintraccio)

                                If StrErroreRintraccio = "" Then

                                    AgronicaCoreDataProvider.GestioneFile.CreaCartellaNelPath(PathCompletoRintraccio, NomeCartella, PathFinaleRintraccio, StrErroreRintraccio)

                                    If StrErroreRintraccio = "" Then

                                        FileSystem.ChDir(PathFinaleRintraccio)

                                        Dim Flag_Esiste As Boolean
                                        Dim PathCartella As String = ""

                                        'Flag_Esiste = GestioneFile_EsistePercorsoCartellaWebConfig("Path_Esportazioni_Stampe", "Esportazione_Rintraccio_" + CStr(Date.Today.Year), PathCartella)
                                        Flag_Esiste = AgronicaCoreDataProvider.GestioneFile.EsistePercorsoCartella2("Path_Esportazioni_Stampe", "Esportazione_Rintraccio_" + CStr(Date.Today.Year), PathCartella)

                                        'SE ESISTE LA ACRTELAL PRINCIPALE DELL'ESPORTAZIONE
                                        If Flag_Esiste = True Then

                                            Flag_Esiste = False

                                            Flag_Esiste = AgronicaCoreDataProvider.GestioneFile.EsistePercorsoCartella(PathCartella + "\" + NomeCartella)

                                            'SE ESISTE LA CARTELLA POMODORO O PISELLO_FAGIOLO, CHE CONTIENE I FILE
                                            If Flag_Esiste = True Then

                                                'CANCELLO IL CONTENUTO DELLA CARTELLA
                                                AgronicaCoreDataProvider.GestioneFile.CancellaFilesWithoutPattern(PathCartella + "\" + NomeCartella)

                                            End If

                                        End If

                                        '////////////////////////////////////////////////////////////////
                                        '////////////////// ESPORTAZIONE RINTRACCIO /////////////////////
                                        '////////////////////////////////////////////////////////////////

                                        Messaggio = Esportazione_Rintraccio(XmlDoc, DT, Log, validita_inizio, PathFinaleRintraccio)

                                    Else
                                        Messaggio = "Errore durante la creazione della cartella " + NomeCartella
                                    End If
                                Else
                                    Messaggio = "Errore durante la creazione della cartella dell'Esportazione Rintraccio"
                                End If


                            Catch ex As Exception

                                Log += CStr(Date.Now) + "   Errore durante l'esportazione Rintraccio: " + ex.Message & vbCrLf & vbCrLf

                            End Try
                        Else
                            Messaggio = "Non si hanno i permessi per l'Esportazione verso Rintraccio."
                        End If


                        '==========================================================================

                End Select 'SCHEDA

                If Messaggio <> "" Then
                    AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If


                '----------------------------------------------------------------


                '########################################################################
                '''''''''''''''''''''''''' CREA FILE DI LOG '''''''''''''''''''''''''''''
                '########################################################################

                Dim PathCompletoLog, PathfinaleLog, StrErroreLog As String
                Dim NomeFileOutput As String = ""


                If Log <> "" Then

                    '+ "_" + Format(Date.Today, "yyyy_MM_dd")

                    If ViewState("scheda") = "rintraccio" Then

                        '----------------------------------------------
                        '        ESPORTAZIONE RINTRACCIO
                        '----------------------------------------------

                        'GestioneFile_CreaCartellaNelPathWebConfig("Path_Esportazioni_Stampe", "Esportazione_Rintraccio_" + CStr(Date.Today.Year), PathCompletoLog, StrErroreLog)
                        AgronicaCoreDataProvider.GestioneFile.CreaCartellaNelPath(objParametri_Server.LogDirectory, "Esportazione_Rintraccio_" + CStr(Date.Today.Year), PathCompletoLog, StrErroreLog)


                        If StrErroreLog <> "" Then
                            AgroMsgBox(StrErroreLog, Page)
                            Exit Sub
                        End If

                        Dim data_ora As String
                        data_ora = Replace(Date.Today.Now.ToString, "/", "-")

                        AgronicaCoreDataProvider.GestioneFile.CreaCartellaNelPath(PathCompletoLog, "Log_Errori", PathfinaleLog, StrErroreLog)

                        If StrErroreLog <> "" Then
                            AgroMsgBox(StrErroreLog, Page)
                            Exit Sub
                        End If

                        If Me.Cmb_Punti.SelectedValue = 99 Then

                            AgronicaCoreDataProvider.GestioneFile.CreaScriviFileSovrascrivi(Log, PathfinaleLog, "Log_Errori_ARP_Pisello_Fagiolo " + data_ora, StrErroreLog)

                            If StrErroreLog <> "" Then
                                AgroMsgBox(StrErroreLog, Page)
                                Exit Sub
                            End If

                        Else

                            AgronicaCoreDataProvider.GestioneFile.CreaScriviFileSovrascrivi(Log, PathfinaleLog, "Log_Errori_ARP_Pomodoro " + data_ora, StrErroreLog)

                            If StrErroreLog <> "" Then
                                AgroMsgBox(StrErroreLog, Page)
                                Exit Sub
                            End If

                        End If


                    Else

                        '----------------------------------------------
                        '              ESPORTATORE UNIVERSALE
                        '----------------------------------------------

                        'GestioneFile_CreaCartellaNelPathWebConfig("Path_Esportazioni_Stampe", "Esportatore_Universale", PathCompletoLog, StrErroreLog)
                        AgronicaCoreDataProvider.GestioneFile.CreaCartellaNelPath(objParametri_Server.LogDirectory, "Esportatore_Universale", PathCompletoLog, StrErroreLog)


                        If StrErroreLog <> "" Then
                            AgroMsgBox(StrErroreLog, Page)
                            Exit Sub
                        End If

                        AgronicaCoreDataProvider.GestioneFile.CreaScriviFileSovrascrivi(Log, PathCompletoLog, "Log_Errori_" + CStr(ViewState("scheda")).ToUpper, StrErroreLog)

                        If StrErroreLog <> "" Then
                            AgroMsgBox(StrErroreLog, Page)
                            Exit Sub
                        End If

                    End If


                End If

                '----------------------------------------------------------------


                '########################################################################
                '''''''''''''''''''''''''''' CREA L'EXCEL '''''''''''''''''''''''''''''''
                '########################################################################

                'VERIFICO IL TIPO DI OUTPUT SCELTO
                'chiama la routine in base ai check scelti
22:
                Select Case Me.RdBList_Esporta.SelectedValue

                    'DT_Finale

                    Case "A"

                        Esporta_Excel(NomeFileOutput, DT)

                    Case "B"

                        Esporta_XML(NomeFileOutput, DT, ViewState("scheda"))

                        'Case "C"

                        '    Esporta_XML_Rintraccio(StringoneXML())

                    Case "D"

                        Esporta_Access(NomeFileOutput, DT)

                    Case "E"

                        Esporta_Testo_CSV(NomeFileOutput, DT)

                    Case "F"

                        Esporta_Testo_CampoFisso(NomeFileOutput, DT)


                End Select

                'elimino gli oggetti
                'DT_Finale = Nothing
                DT = Nothing
                XmlDoc = Nothing

                '----------------------------------------------------------------


                '########################################################################
                '''''''''''''''''''''''''' CREA SMART BUILD '''''''''''''''''''''''''''''
                '########################################################################

                If ViewState("scheda") <> "rintraccio" Then

                    Me.Txt_Esadecimale.Text = ""
                    Call SmartBuild_Crea()

                Else

                    'rintraccio
                    AgroMsgBox("Esportazione avvenuta con successo!", Page)

                    Session("strXmlVariabilistampe") = Nothing
                    Session("scheda") = Nothing

                    '----- Chiudo la finestra
                    Page.FindControl("Form1").Controls.Add(
                        New LiteralControl(
                            "<script language='javascript'>window.close();</script>"))


                End If

                '--------------------------------------------------------------------------

                FileSystem.ChDir("C:\")


            Else
                AgroMsgBox("Il file XML è vuoto!!!", Page)
                Exit Sub
            End If 'if haschildnodes

        Else
            AgroMsgBox("Non sono arrivati i dati dal filtrone!!!", Page)
            Exit Sub

        End If 'fine controllo strXmlVariabili nothing


    End Sub


    '############################################################################################################
    'ATTENZIONE! Per i clienti che hanno SQL Server con le impostazioni CASE SENSITIVE
    'bisogna fare attenzione ai campi (scritti in minuscolo/maiuscolo) che sono in join con la tabella temporanea!
    Private Function Esportazione_Impianti(ByRef XmlDoc As XmlDocument, ByRef DT As DataTable, ByRef Log As String) As String

        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim piva As String
        Dim sa_cod, appezza, id_reg As Integer

        Dim i As Integer ' la i scorre gli impianti e le imprese
        Dim j As Integer ' la j scorre le righe del DT

        Dim num_elementi As Integer
        Dim num_righe_dt As Integer

        Dim errore, msg As String
        Dim Messaggio As String
        Dim AgroMsg As String = ""
        Dim flag_campo As Boolean

        Dim stbQ As New System.Text.StringBuilder
        Dim Query1_TempTableCreazione As String = ""
        Dim Query2_TempTableIndice As String = ""
        Dim Query3_TempTableFill As String = ""
        Dim Query4_TempTableJoin As String = ""

        Dim Flag_FiltroDistinta As Boolean


        If (Me.ChkGruppo3.Items.FindByValue(I_Lotto_DateDistinta).Selected = True Or
            Me.ChkGruppo3.Items.FindByValue(I_Piante_Resa).Selected = True Or
            Me.ChkGruppo3.Items.FindByValue(I_Reg_Dpi_Capitolato).Selected = True Or
                Me.ChkGruppo3.Items.FindByValue(I_OrgRef_MagConf).Selected = True) _
            And (Me.Txt_Data.Text = "") Then
            AgroMsg = "Poichè è stata selezionata l'esportazione di alcuni dati legati alla distinta dell'impianto, è necessario inserire la data dell'esercizio richiesto!"
            Return AgroMsg
            Exit Function
        End If


        XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
        If XML_FiltroStampa Is Nothing Then
            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")
        End If

        XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

        num_elementi = XMLs_VariabiliStampe.Count

        '------------------------------------------------
        '------------ INIZIO CICLO IMPIANTI -------------
        '------------------------------------------------       

        Dim ElencoChiaviImpianto, ChiaveImpianto As String

        flag_campo = False
        ElencoChiaviImpianto = ""

        '-----------------------------------------------
        ' la i scorre gli impianti
        '-----------------------------------------------
        For i = 0 To num_elementi - 1

            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

            If (IsNothing(XML_VariabiliStampe.GetAttribute("piva"))) Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("piva")) = "") Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            End If
            If (IsNothing(XML_VariabiliStampe.GetAttribute("sa_cod"))) Then
                msg = "IL SA_COD E' NULLO!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("sa_cod")) = "") Then
                msg = "IL SA_COD E' NULLO!!!" & vbCrLf
            End If
            If (IsNothing(XML_VariabiliStampe.GetAttribute("appezza"))) Then
                msg = "APPEZZA E' NULLO!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("appezza")) = "") Then
                msg = "APPEZZA E' NULLO!!!" & vbCrLf
            End If
            If (IsNothing(XML_VariabiliStampe.GetAttribute("id_reg"))) Then
                msg = "ID_REG E' NULLO!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("id_reg")) = "") Then
                msg = "ID_REG E' NULLO!!!" & vbCrLf
            End If

            If msg <> "" Then
                AgroMsg = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                Return AgroMsg
                Exit Function
            End If

            'ricavo gli impianti
            piva = XML_VariabiliStampe.GetAttribute("piva")
            sa_cod = XML_VariabiliStampe.GetAttribute("sa_cod")
            appezza = XML_VariabiliStampe.GetAttribute("appezza")
            id_reg = XML_VariabiliStampe.GetAttribute("id_reg")

            'Genero la chiave impianto
            ChiaveImpianto = piva & "_" & sa_cod & "_" & appezza & "_" & id_reg

            ElencoChiaviImpianto += ",'" & ChiaveImpianto & "'"

            '-------------------------------------------------

            Query3_TempTableFill += " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf
            Query3_TempTableFill += " VALUES     (" + Agro_SQL_SaveText_NULL(piva) + "," + Agro_SQL_SaveNum(sa_cod) + "," + Agro_SQL_SaveNum(appezza) + "," + Agro_SQL_SaveNum(id_reg) + ")  " & vbCrLf


        Next

        'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
        ElencoChiaviImpianto = Mid(ElencoChiaviImpianto, 2)

        Dim TabelleTemp_Mode As Integer
        Dim Str_TabelleTemp_RegolaConfronto As String

        TabelleTemp_Mode = Recupera_TabelleTemp_Mode()

        '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT INSERT INTO 
        '2: TRAMITE CREATE TABLE
        Select Case TabelleTemp_Mode

            Case 1
                Query1_TempTableCreazione += " SELECT Piva, Sa_Cod, Appezza, Id_Reg "
                Query1_TempTableCreazione += "   INTO #tempimpianti   "
                Query1_TempTableCreazione += "       FROM Reg_Impianti "
                Query1_TempTableCreazione += "           WHERE 1 = 0   "
                Query1_TempTableCreazione += vbCrLf

            Case 2

                'Recupera regola di Confronto: collate sql_LATIN1_GENERAL_cp850_ci_as NOT NULL

                Str_TabelleTemp_RegolaConfronto = Recupera_Str_TabelleTemp_RegolaConfronto()

                Query1_TempTableCreazione += "   CREATE TABLE #tempimpianti (	"
                Query1_TempTableCreazione += " [Piva]       [nvarchar] (25) " + Str_TabelleTemp_RegolaConfronto + " NOT NULL ,"
                Query1_TempTableCreazione += " [Sa_Cod]     [int] 		    NOT NULL ,"
                Query1_TempTableCreazione += " [Appezza]    [int] 	        NOT NULL ,"
                Query1_TempTableCreazione += " [Id_Reg]     [int] 		    NOT NULL ,"
                Query1_TempTableCreazione += " ) ON [PRIMARY]"
                Query1_TempTableCreazione += vbCrLf
                'Non creo la chiave, perchè poi mi da dei problemi
                'creo l'indice dopo
                'Query1_TempTableCreazione += " ALTER TABLE #tempimpianti ADD "
                'Query1_TempTableCreazione += " CONSTRAINT [PK_Temp_Impianti] PRIMARY KEY  CLUSTERED "
                'Query1_TempTableCreazione += " ("
                'Query1_TempTableCreazione += " [Piva], "
                'Query1_TempTableCreazione += " [Sa_Cod], "
                'Query1_TempTableCreazione += " [Appezza], "
                'Query1_TempTableCreazione += " [Id_Reg] "
                'Query1_TempTableCreazione += " )  ON [PRIMARY] "
                'Query1_TempTableCreazione += vbCrLf

        End Select

        Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimpianti] ON [dbo].[#tempimpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) "


        Select Case Me.RdBList_Distinta.SelectedValue

            Case 0
                Flag_FiltroDistinta = False

            Case 1
                Flag_FiltroDistinta = True

            Case Else
                AgroMsgBox("E' necessario scegliere se filtrare gli esercizi dell'impianto oppure no.", Page)
                Exit Function
        End Select


        '-----------------------------------------------------------------------
        ' VAI CON LA POTENZA DELLA TERA-QUERY IMPIANTI !!!!!!!!
        '-----------------------------------------------------------------------
        ' NOTA: SE AL CENTRO AZIENDALE NON E' IMPOSTATO L'INDIRIZZO DI TIPO 1
        ' I RELATIVI IMPIANTI NON VENGONO TIRATI SU DALLA QUERY!
        '-----------------------------------------------------------------------
        stbQ.Length = 0
        stbQ.Append(" ")
        stbQ.Append(" SELECT DISTINCT CASE  ")
        stbQ.Append(" WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.Piva ")
        stbQ.Append(" ELSE Imprese.partitaIvaReale ")
        stbQ.Append(" End Piva " & vbCrLf)
        'If (Me.ChkGruppo1.Items.FindByValue(I_Cod_Campo).Selected = True) Then
        '    flag_campo = True
        '    stbQ.Append(" ISNULL(Appezzamento.Campo_Cod, -1) As campo_cod, " & vbCrLf)
        'End If
        'stbQ.Append("  Reg_Impianti.Sa_Cod, Reg_Impianti.Appezza, Reg_Impianti.Id_Reg " & vbCrLf)

        If (Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Imprese.rag_soc, ' ') AS rag_soc" & vbCrLf)
        End If
        'If (Me.ChkGruppo1.Items.FindByValue(I_Cod_Socio).Selected = True) Then
        '    flag_campo = True
        '    ' query interna per recuperare il Codice Socio
        '    stbQ.Append(", ")
        '    stbQ.Append(" ISNULL((SELECT TOP 1 val_cod " & vbCrLf)
        '    stbQ.Append("         FROM Imprese_Codici AS Imprese_Codici_Interno " & vbCrLf)
        '    stbQ.Append("         WHERE Imprese_Codici_Interno.piva = Reg_Impianti.piva " & vbCrLf)
        '    stbQ.Append("         AND Imprese_Codici_Interno.id_cod = 1033), ' ') AS Codice_Socio" & vbCrLf)
        '    'fine
        'End If
        If (Me.ChkGruppo1.Items.FindByValue(I_Cuaa).Selected = True) Then
            flag_campo = True
            ' query interna per recuperare il CUAA
            stbQ.Append(", ")
            stbQ.Append(" ISNULL((SELECT TOP 1 val_cod " & vbCrLf)
            stbQ.Append("         FROM Imprese_Codici AS Imprese_Codici_CUAA " & vbCrLf)
            stbQ.Append("         WHERE Imprese_Codici_CUAA.piva = Reg_Impianti.piva " & vbCrLf)
            stbQ.Append("         AND Imprese_Codici_CUAA.id_cod = 1010), ' ') AS CUAA" & vbCrLf)

            'Recupero anche il Codice Socio
            stbQ.Append(", ")
            stbQ.Append(" ISNULL((SELECT TOP 1 val_cod " & vbCrLf)
            stbQ.Append("         FROM Imprese_Codici AS Imprese_Codici_CodSocio " & vbCrLf)
            stbQ.Append("         WHERE Imprese_Codici_CodSocio.piva = Reg_Impianti.piva " & vbCrLf)
            stbQ.Append("         AND Imprese_Codici_CodSocio.id_cod = 1033), ' ') AS CodiceSocio" & vbCrLf)
            'fine
        End If
        'controllo selezione indirizzo impresa
        If (Me.ChkGruppo1.Items.FindByValue(I_IndImpresa).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(IndImp.ind_des, ' ') AS imp_ind_des, ISNULL(IndImp.frz_des, ' ') AS imp_frz_des, " & vbCrLf)
            stbQ.Append(" ISNULL(IndImp.CAP, '') AS imp_cap, ISNULL(IstatImp.LOCALITA, '') AS imp_com_des, ISNULL(IstatImp.COMUNI_PROV, '') AS imp_pro_cod " & vbCrLf)
        End If
        'controllo se sono state selezionate le cooperative padre
        If (Me.ChkGruppo1.Items.FindByValue(I_ImpresaPadre).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Imprese_1.Piva, ' ') AS PIVA_padre" & vbCrLf)
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Imprese_1.rag_soc, ' ') AS coop_padre" & vbCrLf)
        End If
        'fine coop padre

        If Me.ChkGruppo1.Items.FindByValue(I_Tecnico).Selected = True Then
            flag_campo = True
            stbQ.Append(", ISNULL(    ")
            stbQ.Append(" (SELECT  TOP 1 Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS tecnico  " & vbCrLf)
            stbQ.Append(" FROM    Imprese_Codici " & vbCrLf)
            stbQ.Append(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod   " & vbCrLf)
            stbQ.Append(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva " & vbCrLf)
            stbQ.Append(" WHERE  (Imprese_Codici.Piva = Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Codici.Id_Cod = " + CStr(enum_CodiciAnagrafe.Tecnico) + ") " & vbCrLf)
            stbQ.Append(" AND     (UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) & "') " & vbCrLf)
            stbQ.Append(" )   ")
            stbQ.Append(", '') AS tecnico " & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_IndCentro).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(IndCentro.ind_des, ' ') AS cen_ind_des, ISNULL(IndCentro.frz_des, ' ') AS cen_frz_des, " & vbCrLf)
            stbQ.Append(" ISNULL(IndCentro.CAP, '') AS cen_cap, ISNULL(ISTATCentro.LOCALITA, '') AS cen_com_des, ISNULL(ISTATCentro.COMUNI_PROV, '') AS cen_pro_cod " & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_CodiciIstat).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(IndCentro.pro_cod_istat, ' ') AS pro_cod_istat, ISNULL(IndCentro.com_cod_istat, ' ') AS com_cod_istat" & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_NomeCentro).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Centri_Aziendali.sa_cod, 0) AS sa_cod" & vbCrLf)
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Centri_Aziendali.sa_nome, ' ') AS sa_nome" & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_NomeCampo).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Campi.Campo_Des, ' ') AS campo_des" & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_NomeAppezza).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome" & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_DatiAppezza).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Appezzamento.Sup_App, 0) AS sup_app, ISNULL(Appezzamento.Validita_Inizio, '01/01/1900') AS Inizio_Appezza, ISNULL(Appezzamento.Validita_Fine, '31/12/2100') AS Fine_Appezza, " & vbCrLf)
            stbQ.Append(" (SELECT TOP 1 val_cod FROM [Appezzamento_Codici] WHERE Piva = Appezzamento.Piva AND Sa_Cod = Appezzamento.Sa_Cod AND Appezza = Appezzamento.Appezza AND appezzamento_codici.id_cod=1018) As MetodoProduzione_Cod ," & vbCrLf)
            stbQ.Append(" (SELECT TOP 1 case val_cod when 1 then 'Convenzionale' 
                                  when 2 then 'In Conversione' 
                                  when 3 then 'Biologico' 
                            end 
                            FROM Appezzamento_Codici WHERE Piva = Appezzamento.Piva AND Sa_Cod = Appezzamento.Sa_Cod AND Appezza = Appezzamento.Appezza 
                            AND appezzamento_codici.id_cod=1018) As MetodoProduzione_Des" & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_GruppoVeg).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(GruppoVegetale.Gru_Des, ' ') AS gru_des" & vbCrLf)
        End If
        'If (Me.ChkGruppo2.Items.FindByValue(I_Cod_Specie).Selected = True) Then
        '    flag_campo = True
        '    stbQ.Append(", ")
        '    stbQ.Append(" ISNULL(SpecieVegetali.Veg_Cod, -1) AS veg_cod" & vbCrLf)
        'End If
        If (Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True) Or
            (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Or
                (Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True) Then
            'serve per veg-des, cul-des, sup-imp
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(SpecieVegetali.Veg_Cod, ' ') AS Veg_Cod" & vbCrLf)
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(SpecieVegetali.Veg_Des, ' ') AS veg_des" & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True) Or
                (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Or
                    (Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True) Then
            'serve per veg-des, cul-des, sup-imp
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Cultivar.Cul_Cod, ' ') AS cul_cod" & vbCrLf)
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Cultivar.Cul_Des, ' ') AS cul_des" & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_TipVar).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            'stbQ.Append(" ISNULL(GruppoVarietale.Grva_Des, ' ') AS grva_des" & vbCrLf)
            stbQ.Append(" ISNULL(GruppoVarietale.Grva_Des,ISNULL( (select grva_des + ' -- Ibrido ' from GruppoVarietale where GruppoVarietale.Grva_cod = (0- Reg_Impianti.GRVA_Cod_VEG)), ' ')) AS grva_des " & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_Finalita).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(GruppoFinalita.Grfi_Des, ' ') AS grfi_des" & vbCrLf)
        End If

        If (Me.ChkGruppo2.Items.FindByValue(I_DestUso).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione " & vbCrLf)
            stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
            stbQ.Append(" INNER JOIN Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice " & vbCrLf)
            stbQ.Append(" WHERE  (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            stbQ.Append(" AND   (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
            stbQ.Append(" AND   (Codici_Anagrafe.gruppo = 'TERRENO') " & vbCrLf)
            stbQ.Append(" ) , '') AS dest_uso " & vbCrLf)
        End If

        If (Me.ChkGruppo3.Items.FindByValue(I_Copertura).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Copertura.Cop_Des, ' ') AS cop_des" & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True) Or
                (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Or
                    (Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True) Or
                        (Me.ChkGruppo3.Items.FindByValue(I_Piante_Resa).Selected = True) Then
            'serve per veg-des, cul-des, sup-imp, resa prevista, piante totali
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Reg_Impianti.Sup_Imp, -1.0000) AS sup_imp" & vbCrLf)
            stbQ.Append(", ")
            stbQ.Append(" CAST(Reg_Impianti.Appezza as varchar(10)) + '_' + CAST(Reg_Impianti.ID_Reg as varchar(10)) as id_Impianto" & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_InizioFineImp).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Inizio, 103), ' ') AS inizio_impianto, " & vbCrLf)
            stbQ.Append(" ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), ' ') AS fine_impianto " & vbCrLf)
        End If

        If (Me.ChkGruppo3.Items.FindByValue(I_SemTrapData).Selected = True) Then
            flag_campo = True

            ''query interna per recuperare il lav_cod
            'stbQ.Append(", ")
            'stbQ.Append(" ISNULL((SELECT TOP 1 Agenda.lav_cod " & vbCrLf)
            'stbQ.Append("         FROM Agenda " & vbCrLf)
            'stbQ.Append("         INNER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva " & vbCrLf)
            'stbQ.Append("         AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            'stbQ.Append("         AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
            'stbQ.Append("         INNER JOIN Movimenti_dettagli ON Movimenti.Piva = Movimenti_dettagli.Piva " & vbCrLf)
            'stbQ.Append("         AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod " & vbCrLf)
            'stbQ.Append("         AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda " & vbCrLf)
            'stbQ.Append("         AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            'stbQ.Append("         INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.Piva = Mov_Destinazioni.Piva " & vbCrLf)
            'stbQ.Append("         AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            'stbQ.Append("         AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf)
            'stbQ.Append("         AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
            'stbQ.Append("         AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            'stbQ.Append("         WHERE (Agenda.Lav_Cod = 2 OR Agenda.Lav_Cod = 71) " & vbCrLf)
            'stbQ.Append("         AND Movimenti.Cau_Mov = '2300' " & vbCrLf)
            'stbQ.Append("         AND Mov_Destinazioni.Piva = Reg_Impianti.Piva " & vbCrLf)
            'stbQ.Append("         AND Mov_Destinazioni.sa_cod = Reg_Impianti.sa_cod " & vbCrLf)
            'stbQ.Append("         AND Mov_Destinazioni.appezza = Reg_Impianti.Appezza " & vbCrLf)
            'stbQ.Append("         AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg), -1) AS lav_cod_imp" & vbCrLf)
            ''fine

            'End If
            'If (Me.ChkGruppo3.Items.FindByValue(I_SemTrapData).Selected = True) Then
            '    flag_campo = True

            ' query interna per recuperare la data di trapianto/semina
            stbQ.Append(", ")
            stbQ.Append(" ISNULL( (SELECT TOP 1 CONVERT(varchar(50), Agenda.lav_cod) + '|' + CONVERT(varchar(10), (Agenda.Validita_Inizio), 103)" & vbCrLf)
            stbQ.Append("         FROM Agenda " & vbCrLf)
            stbQ.Append("         INNER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva " & vbCrLf)
            stbQ.Append("         AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            stbQ.Append("         AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
            stbQ.Append("         INNER JOIN Movimenti_dettagli ON Movimenti.Piva = Movimenti_dettagli.Piva " & vbCrLf)
            stbQ.Append("         AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod " & vbCrLf)
            stbQ.Append("         AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda " & vbCrLf)
            stbQ.Append("         AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append("         INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.Piva = Mov_Destinazioni.Piva " & vbCrLf)
            stbQ.Append("         AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
            stbQ.Append("         AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf)
            stbQ.Append("         AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
            stbQ.Append("         AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            stbQ.Append("         WHERE (Agenda.Lav_Cod = 2 OR Agenda.Lav_Cod = 71) " & vbCrLf)
            stbQ.Append("         AND Movimenti.Cau_Mov = '2300' " & vbCrLf)
            stbQ.Append("         AND Mov_Destinazioni.Piva = Reg_Impianti.Piva " & vbCrLf)
            stbQ.Append("         AND Mov_Destinazioni.sa_cod = Reg_Impianti.sa_cod " & vbCrLf)
            stbQ.Append("         AND Mov_Destinazioni.appezza = Reg_Impianti.Appezza " & vbCrLf)
            stbQ.Append("         AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg " & vbCrLf)
            stbQ.Append("         ORDER BY Agenda.Validita_Inizio), '-1|01/01/1900') AS LavCod_Data_Semina, -1 AS lav_cod_imp, '01/01/1900' AS data_semina " & vbCrLf)
            'fine
        End If


        'controllo se sono stati selezionati forma allevamento e dettaglio specie personalizzato
        If (Me.ChkGruppo3.Items.FindByValue(I_FA_DSP).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(FormeAllevamento.Foral_Des, ' ') AS foral_des " & vbCrLf)

            stbQ.Append(", ISNULL(( SELECT     TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des " & vbCrLf)
            stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
            stbQ.Append(" INNER JOIN CAC_Codifica_InfoAggiuntive ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod" & vbCrLf)
            stbQ.Append(" WHERE  (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            stbQ.Append(" AND (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
            stbQ.Append(" AND (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) + ") " & vbCrLf)
            stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Piva_SuperUser = " & Agro_SQL_SaveText_NULL(CStr(Session("ASG_SuperUser_CodFiscale"))) & ") " & vbCrLf)
            stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Argomento_Cod = 2) " & vbCrLf)
            stbQ.Append(" ) , '') AS dett_specie_pers " & vbCrLf)
        End If

        'controllo se sono stati selezionati portinnesto e imp irrigazione
        If (Me.ChkGruppo3.Items.FindByValue(I_Port_Irr).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ISNULL(Portinnesti.Port_Des, ' ') AS port_des " & vbCrLf)
            stbQ.Append(", ISNULL(ImpiantiIrrigazioni.Imp_Des, ' ') AS imp_des " & vbCrLf)
        End If

        'se è stato selezionato il sesto d'impianto
        If (Me.ChkGruppo3.Items.FindByValue(I_SestoImpianto).Selected = True) Then
            flag_campo = True

            stbQ.Append(", ISNULL(( SELECT TOP 1  Reg_Impianti_Codici.val_cod " & vbCrLf)
            stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
            stbQ.Append(" WHERE   (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_TraFila_Maschio) + ") " & vbCrLf)
            stbQ.Append(" ) , 0) AS tra_fila_maschio " & vbCrLf)

            stbQ.Append(", ISNULL(( SELECT TOP 1  Reg_Impianti_Codici.val_cod " & vbCrLf)
            stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
            stbQ.Append(" WHERE   (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_SuFila_Maschio) + ") " & vbCrLf)
            stbQ.Append(" ) , 0) AS su_fila_maschio " & vbCrLf)
        End If

        'controllo se sono stati selezionati piante e resa
        If (Me.ChkGruppo3.Items.FindByValue(I_Piante_Resa).Selected = True) Then
            flag_campo = True
            'stbQ.Append(", ISNULL(Reg_Impianti.P_HA, 0) AS P_HA, 0 AS P_Tot " & vbCrLf)

            stbQ.Append(", 0 AS P_HA, 0 AS P_Tot, 0 AS resa_prevista " & vbCrLf)

            stbQ.Append(", ISNULL(( SELECT TOP 1 CONVERT(varchar(250), ISNULL(Imprese_Progetti.P_HA, 0) ) + '|' + CONVERT(varchar(250), ISNULL(Imprese_Progetti.Produzione_Prevista, 0)) " & vbCrLf)
            stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
            stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            If Flag_FiltroDistinta = True Then
                stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
                stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            End If
            stbQ.Append(" ) , '0|0') AS piante_resa " & vbCrLf)

        End If

        'controllo se sono stati selezionati lotto e date distinta
        If (Me.ChkGruppo3.Items.FindByValue(I_Lotto_DateDistinta).Selected = True) Then
            flag_campo = True

            stbQ.Append(", ISNULL(( SELECT    TOP 1 Imprese_Progetti.Progetto_Nome + '|' + CONVERT(varchar(10), Imprese_Progetti.Validita_Inizio, 120) + '|' + CONVERT(varchar(10), Imprese_Progetti.Validita_Fine, 120) " & vbCrLf)
            stbQ.Append("             FROM    Imprese_Progetti " & vbCrLf)
            stbQ.Append("             WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append("             AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append("             AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append("             AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            If Flag_FiltroDistinta = True Then
                stbQ.Append("             AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
                stbQ.Append("             AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            End If
            stbQ.Append(" ) , '|01/01/1900|31/12/2100') AS date_distinta, '' AS lotto_distinta, '' AS inizio_distinta, '' AS fine_distinta " & vbCrLf)
        End If


        'controllo se sono stati selezionati il capitolato privato e il regolamento
        If (Me.ChkGruppo3.Items.FindByValue(I_Reg_Dpi_Capitolato).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ISNULL(( SELECT     TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des " & vbCrLf)
            stbQ.Append(" FROM         Imprese_Progetti " & vbCrLf)
            stbQ.Append(" INNER JOIN Reg_Impianti_Codici ON Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod AND  " & vbCrLf)
            stbQ.Append(" Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg AND  " & vbCrLf)
            stbQ.Append(" Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN CAC_Codifica_InfoAggiuntive ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod" & vbCrLf)
            stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            stbQ.Append(" AND (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Capitolato_Privato) + ") " & vbCrLf)
            stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Piva_SuperUser = " & Agro_SQL_SaveText_NULL(CStr(Session("ASG_SuperUser_CodFiscale"))) & ") " & vbCrLf)
            stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Argomento_Cod = 1) " & vbCrLf)
            If Flag_FiltroDistinta = True Then
                stbQ.Append(" AND (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
                stbQ.Append(" AND (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            End If
            stbQ.Append(" ) , '') AS capitolato_privato " & vbCrLf)

            stbQ.Append(", ISNULL(( SELECT    TOP 1 Regolamenti.Reg_Des " & vbCrLf)
            stbQ.Append("             FROM    Imprese_Progetti " & vbCrLf)
            stbQ.Append("             INNER JOIN  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)
            stbQ.Append("             WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append("             AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append("             AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append("             AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            If Flag_FiltroDistinta = True Then
                stbQ.Append("             AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
                stbQ.Append("             AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            End If
            stbQ.Append(" ) , ' ') AS Regolamento " & vbCrLf)
        End If

        'controllo se sono stati selezionati organismo referente e magazzino conferimento
        If (Me.ChkGruppo3.Items.FindByValue(I_OrgRef_MagConf).Selected = True) Then
            flag_campo = True

            '16/11/2017: viste le modifiche sull'iorganismo referente che ora è sempre contatto
            'non serve più andare a leggere la gerarchia

            'stbQ.Append(", ISNULL(    " & vbCrLf)
            'stbQ.Append("( SELECT     TOP 1 Imprese.rag_soc AS coop_referente " & vbCrLf)
            'stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
            'stbQ.Append(" INNER JOIN    Reg_Impianti_Codici ON " & vbCrLf)
            'stbQ.Append(" Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod " & vbCrLf)
            'stbQ.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg   " & vbCrLf)
            'stbQ.Append(" AND  Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod    " & vbCrLf)
            'stbQ.Append(" INNER JOIN GerarchiaImprese ON GerarchiaImprese.Padre = Reg_Impianti_Codici.val_cod  " & vbCrLf)
            'stbQ.Append(" INNER JOIN  Imprese ON GerarchiaImprese.Padre = Imprese.Piva   " & vbCrLf)
            'stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            'stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            'stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            'stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            'stbQ.Append(" AND     (Reg_Impianti_Codici.Id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_Cooperativa) + ") " & vbCrLf)
            'If Flag_FiltroDistinta = True Then
            '    stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            '    stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            'End If
            'stbQ.Append(" )   ")
            'stbQ.Append(", '') AS coop_referente " & vbCrLf)

            'stbQ.Append(", ISNULL(    ")
            'stbQ.Append(" (SELECT  TOP 1 Contatti.rag_soc AS coop_referente  " & vbCrLf)
            'stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
            'stbQ.Append(" INNER JOIN    Reg_Impianti_Codici ON " & vbCrLf)
            'stbQ.Append(" Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod " & vbCrLf)
            'stbQ.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg   " & vbCrLf)
            'stbQ.Append(" AND  Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod    " & vbCrLf)
            'stbQ.Append(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Reg_Impianti_Codici.val_cod   " & vbCrLf)
            'stbQ.Append(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva " & vbCrLf)
            'stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            'stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            'stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            'stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            'stbQ.Append(" AND     (Reg_Impianti_Codici.Id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_Cooperativa) + ") " & vbCrLf)
            'stbQ.Append(" AND     (UtentiXImprese.[USER] = '" & CStr(Session("ASG_SuperUser_CodFiscale")) & "') " & vbCrLf)
            'If Flag_FiltroDistinta = True Then
            '    stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            '    stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            'End If
            'stbQ.Append(" )   ")
            'stbQ.Append(", '') AS coop_referente_bis " & vbCrLf)

            'organismo referente
            stbQ.Append(", ISNULL(    ")
            stbQ.Append(" (SELECT  TOP 1 Contatti.rag_soc AS coop_referente  " & vbCrLf)
            stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
            stbQ.Append(" INNER JOIN    Reg_Impianti_Codici ON " & vbCrLf)
            stbQ.Append(" Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod " & vbCrLf)
            stbQ.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg   " & vbCrLf)
            stbQ.Append(" AND  Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod    " & vbCrLf)
            stbQ.Append(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Reg_Impianti_Codici.val_cod   " & vbCrLf)
            stbQ.Append(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva " & vbCrLf)
            stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_Cooperativa) + ") " & vbCrLf)
            stbQ.Append(" AND     (UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) & "') " & vbCrLf)
            If Flag_FiltroDistinta = True Then
                stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
                stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            End If
            stbQ.Append(" )   ")
            stbQ.Append(", '') AS org_referente " & vbCrLf)

            'magazzino conferimento
            stbQ.Append(", ISNULL(    " & vbCrLf)
            stbQ.Append("( SELECT TOP 1 Fabbricati.Fabbricato_des " & vbCrLf)
            stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
            stbQ.Append(" INNER JOIN    Reg_Impianti_Codici ON " & vbCrLf)
            stbQ.Append(" Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod " & vbCrLf)
            stbQ.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg   " & vbCrLf)
            stbQ.Append(" AND  Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod    " & vbCrLf)
            stbQ.Append(" INNER JOIN Fabbricati ON CONVERT(varchar(50),Fabbricati.Fabbricato_Cod) +'|'+ CONVERT(varchar(50),FABBRICATI.sa_COD) +'|'+ Fabbricati.piva = Reg_Impianti_Codici.val_cod  " & vbCrLf)
            stbQ.Append("    " & vbCrLf)
            stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
            stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
            stbQ.Append(" AND     (Reg_Impianti_Codici.Id_cod = " + CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) + ") " & vbCrLf)
            'stbQ.Append(" AND     Fabbricati.Piva = " & vbCrLf)
            'stbQ.Append("                               ISNULL( (    " & vbCrLf)
            'stbQ.Append("                                   SELECT  TOP 1 val_cod AS piva_org " & vbCrLf)
            'stbQ.Append("                                   FROM    Reg_Impianti_Codici RIC " & vbCrLf)
            'stbQ.Append("                                   WHERE  (RIC.Piva = Imprese_Progetti.Piva) " & vbCrLf)
            'stbQ.Append("                                   AND     (RIC.Sa_Cod = Imprese_Progetti.sa_cod) " & vbCrLf)
            'stbQ.Append("                                   AND     (RIC.Appezza  = Imprese_Progetti.Appezza) " & vbCrLf)
            'stbQ.Append("                                   AND     (RIC.Id_Reg = Imprese_Progetti.Id_Reg) " & vbCrLf)
            'stbQ.Append("                                   AND     (RIC.Progetto_Cod = Imprese_Progetti.Progetto_Cod) " & vbCrLf)
            'stbQ.Append("                                   AND     (RIC.Id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_Cooperativa) + ") ), '')  " & vbCrLf)
            If Flag_FiltroDistinta = True Then
                stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
                stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
            End If
            stbQ.Append(" )   ")
            stbQ.Append(", ' ') AS magazzino_conf " & vbCrLf)
        End If
        'fine controllo

        'controllo se sono state selezionate le particelle
        If (Me.ChkGruppo3.Items.FindByValue(I_Particelle).Selected = True) Then
            flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(Lista_Regioni.Regione_Des, '') AS part_regione, ISNULL(ISTATParticelle.COMUNI_PROV, '') AS part_pro_cod, ISNULL(ISTATParticelle.LOCALITA, '') AS part_com_des, " & vbCrLf)
            stbQ.Append(" ISNULL(AppezzamentiXParticelle.PROV, ' ') AS PROV, " & vbCrLf)
            stbQ.Append(" ISNULL(AppezzamentiXParticelle.COM, ' ')AS COM, " & vbCrLf)
            stbQ.Append(" ISNULL(AppezzamentiXParticelle.SEZIONE, ' ') AS SEZIONE, " & vbCrLf)
            stbQ.Append(" ISNULL(AppezzamentiXParticelle.FOGLIO, -1) AS FOGLIO, " & vbCrLf)
            stbQ.Append(" ISNULL(AppezzamentiXParticelle.NUMERO, -1) AS NUMERO, " & vbCrLf)
            stbQ.Append(" ISNULL(AppezzamentiXParticelle.SUBALTERNO, ' ') AS SUBALTERNO, " & vbCrLf)
            stbQ.Append(" ISNULL(ParticelleCatastali.ETTARI, -1) AS ETTARI, " & vbCrLf)
            stbQ.Append(" ISNULL(ParticelleCatastali.[ARE], -1) AS ARE, " & vbCrLf)
            stbQ.Append(" ISNULL(ParticelleCatastali.CENTIARE, -1) AS CENTIARE" & vbCrLf)
            'End If
            'If (Me.ChkGruppo3.Items.FindByValue(I_SupImpPart).Selected = True) Then
            '    flag_campo = True
            stbQ.Append(", ")
            stbQ.Append(" ISNULL(AppezzamentiXParticelle.AREA, -1.0000)AS AREA" & vbCrLf)
        End If
        'fine controllo

        stbQ.Append(" ")
        ' stbQ.Append( " Reg_Impianti.CUL_COD, Reg_Impianti.GRFI_COD" 'non serve al momento

        'fine SELECT
        stbQ.Append(" FROM Reg_Impianti ")

        If (Me.ChkGruppo1.Items.FindByValue(I_NomeCampo).Selected = True) Or
               (Me.ChkGruppo2.Items.FindByValue(I_NomeAppezza).Selected = True) Or
                   (Me.ChkGruppo2.Items.FindByValue(I_DatiAppezza).Selected = True) Then
            'campo cod, app nome, campo nome, data semina?
            stbQ.Append(" INNER JOIN Appezzamento ON Reg_Impianti.Appezza = Appezzamento.Appezza " & vbCrLf)
            stbQ.Append(" AND Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod " & vbCrLf)
            stbQ.Append(" AND Reg_Impianti.Piva = Appezzamento.Piva " & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_NomeCampo).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN  Campi ON Appezzamento.Piva = Campi.Piva " & vbCrLf)
            stbQ.Append(" AND Appezzamento.Sa_Cod = Campi.Sa_Cod " & vbCrLf)
            stbQ.Append(" AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
        End If

        If (Me.ChkGruppo1.Items.FindByValue(I_IndCentro).Selected = True) Or
            (Me.ChkGruppo1.Items.FindByValue(I_NomeCentro).Selected = True) Or
                (Me.ChkGruppo1.Items.FindByValue(I_CodiciIstat).Selected = True) Then
            'indirizzo centro, nome centro, codici istat
            stbQ.Append(" INNER JOIN Centri_Aziendali ON Reg_Impianti.Sa_Cod = Centri_Aziendali.sa_cod " & vbCrLf)
            stbQ.Append(" AND Reg_Impianti.Piva = Centri_Aziendali.Piva " & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_IndCentro).Selected = True) Or
                (Me.ChkGruppo1.Items.FindByValue(I_CodiciIstat).Selected = True) Then
            'indirizzo centro, codici istat
            stbQ.Append(" INNER JOIN CentrixIndirizzi " & vbCrLf)
            stbQ.Append(" ON Centri_Aziendali.Piva = CentrixIndirizzi.Piva " & vbCrLf)
            stbQ.Append(" AND Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Indirizzi IndCentro ON CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo " & vbCrLf)
            stbQ.Append(" INNER JOIN ISTAT IstatCentro ON IndCentro.pro_cod_istat = IstatCentro.PROV AND IndCentro.com_cod_istat = IstatCentro.COM " & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_IndImpresa).Selected = True) Then
            'indirizzo impresa, codici istat
            stbQ.Append(" INNER JOIN ImpresexIndirizzi " & vbCrLf)
            stbQ.Append(" ON Reg_Impianti.Piva = ImpresexIndirizzi.Piva " & vbCrLf)
            stbQ.Append(" INNER JOIN Indirizzi IndImp ON ImpresexIndirizzi.cod_indirizzo = IndImp.cod_indirizzo " & vbCrLf)
            stbQ.Append(" INNER JOIN ISTAT IstatImp ON IndImp.pro_cod_istat = IstatImp.PROV AND IndImp.com_cod_istat = IstatImp.COM " & vbCrLf)
        End If
        If (Me.ChkGruppo3.Items.FindByValue(I_Particelle).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN AppezzamentiXParticelle " & vbCrLf)
            stbQ.Append(" ON Reg_Impianti.Piva = AppezzamentiXParticelle.Piva " & vbCrLf)
            stbQ.Append(" AND Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod " & vbCrLf)
            stbQ.Append(" AND Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza " & vbCrLf)
            'End If
            'If (Me.ChkGruppo3.Items.FindByValue(27).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN ParticelleCatastali " & vbCrLf)
            stbQ.Append(" ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV " & vbCrLf)
            stbQ.Append(" AND ParticelleCatastali.COM = AppezzamentiXParticelle.COM " & vbCrLf)
            stbQ.Append(" AND ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE " & vbCrLf)
            stbQ.Append(" AND ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO " & vbCrLf)
            stbQ.Append(" AND ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO " & vbCrLf)
            stbQ.Append(" AND ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)
            'Decodifica regione + istat particelle
            stbQ.Append(" LEFT OUTER JOIN ISTAT IstatParticelle ON AppezzamentiXParticelle.PROV = ISTATParticelle.PROV AND AppezzamentiXParticelle.COM = ISTATParticelle.COM " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN Lista_Province ON Lista_Province.SIGLA = ISTATParticelle.Comuni_Prov " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG " & vbCrLf)

        End If
        If (Me.ChkGruppo3.Items.FindByValue(I_Copertura).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN Copertura ON Reg_Impianti.COP_COD = Copertura.Cop_Cod " & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_TipVar).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod " & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_Finalita).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod " & vbCrLf)
        End If
        If (Me.ChkGruppo3.Items.FindByValue(I_Port_Irr).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN Portinnesti ON Reg_Impianti.Port_COD = Portinnesti.Port_Cod " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN ImpiantiIrrigazioni ON Reg_Impianti.Imp_COD = ImpiantiIrrigazioni.Imp_Cod " & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Or
                (Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True) Or
                    (Me.ChkGruppo2.Items.FindByValue(I_GruppoVeg).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True) Then
            'serve per veg-des, cul-des, sup-imp
            stbQ.Append(" LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Or
                  (Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True) Or
                      (Me.ChkGruppo2.Items.FindByValue(I_GruppoVeg).Selected = True) Or
                              (Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True) Then
            'veg cod, veg des, gru des,
            'serve per veg-des, cul-des, sup-imp
            stbQ.Append(" LEFT OUTER JOIN SpecieVegetali " & vbCrLf)
            stbQ.Append(" ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod " & vbCrLf)
        End If
        If (Me.ChkGruppo2.Items.FindByValue(I_GruppoVeg).Selected = True) Then
            'gru des
            stbQ.Append(" LEFT OUTER JOIN GruppoVegetale ON SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod " & vbCrLf)
        End If
        If (Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True) Or
            (Me.ChkGruppo1.Items.FindByValue(I_ImpresaPadre).Selected = True) Then
            'rag soc, coop padre, piva padre
            stbQ.Append(" INNER JOIN Imprese ON Reg_Impianti.Piva = Imprese.Piva " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN Imprese Imprese_1 " & vbCrLf)
            stbQ.Append(" INNER JOIN GerarchiaImprese ON Imprese_1.Piva = GerarchiaImprese.Padre " & vbCrLf)
            stbQ.Append(" ON Imprese.Piva = GerarchiaImprese.Figlio " & vbCrLf)
        End If
        If (Me.ChkGruppo3.Items.FindByValue(I_FA_DSP).Selected = True) Then
            stbQ.Append(" LEFT OUTER JOIN  FormeAllevamento ON Reg_Impianti.FORAL_COD = FormeAllevamento.Foral_Cod " & vbCrLf)
        End If


        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA

        '           --> AGGIUNGO QUESTA PARTE:

        stbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
        stbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
        '/**************************************************


        'WHERE

        ' stbQ.Append( " WHERE Reg_Impianti.Piva = '" & SQL_SaveText(piva) & "' " )
        ' stbQ.Append( " AND Reg_Impianti.Sa_Cod = " & SQL_SaveNum(sa_cod) & " " )
        ' stbQ.Append( " AND Reg_Impianti.Appezza = " & SQL_SaveNum(appezza) & " " )
        ' stbQ.Append( " AND Reg_Impianti.Id_Reg = " & SQL_SaveNum(id_reg) & " " )

        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA

        '           --> RIMUOVO QUESTA PARTE:

        'stbQ.Append( " AND               ((Reg_Impianti.Piva  " & vbCrLf)
        'stbQ.Append( "                     + '_' + CONVERT(varchar(10), Reg_Impianti.Sa_Cod)  " & vbCrLf)
        'stbQ.Append( "                     + '_' + CONVERT(varchar(10), Reg_Impianti.Appezza)  " & vbCrLf)
        'stbQ.Append( "                     + '_' + CONVERT(varchar(10), Reg_Impianti.Id_Reg))  " & vbCrLf)
        'stbQ.Append( "                     IN (" & ElencoChiaviImpianto & "))  " & vbCrLf)
        '/**************************************************


        If (Me.ChkGruppo1.Items.FindByValue(I_IndCentro).Selected = True) Or
            (Me.ChkGruppo1.Items.FindByValue(I_CodiciIstat).Selected = True) Then
            ' stbQ.Append( " AND CentrixIndirizzi.Tipo_Indirizzo = 1 " & vbCrLf)
            stbQ.Append(" WHERE CentrixIndirizzi.Tipo_Indirizzo = 1 " & vbCrLf)
        End If

        'ORDINAMENTO OLD
        'If (Me.ChkGruppo1.Items.FindByValue(I_ImpresaPadre).Selected = True) Then
        '    stbQ.Append(" ORDER BY coop_padre " & vbCrLf)
        '    If (Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True) Then
        '        stbQ.Append(" , rag_soc " & vbCrLf)
        '    End If
        '    stbQ.Append(" ASC ")
        'ElseIf (Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True) Then
        '    stbQ.Append(" ORDER BY rag_soc ASC " & vbCrLf)
        'End If

        'ORDINAMENTO
        'modificato il 24/02/2016 x GENAGRICOLA
        Select Case Me.Cmb_OrdinamentoImpianti.SelectedValue

            Case enum_OrdinamentoImpianto.OrdinamentoDefault
                If (Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True) Then
                    stbQ.Append(" ORDER BY rag_soc ASC " & vbCrLf)
                End If
            Case enum_OrdinamentoImpianto.CoopImpresa
                If (Me.ChkGruppo1.Items.FindByValue(I_ImpresaPadre).Selected = True) Then
                    stbQ.Append(" ORDER BY coop_padre " & vbCrLf)
                    If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then
                        stbQ.Append(" , rag_soc " & vbCrLf)
                    End If
                    stbQ.Append(" ASC ")
                End If

            Case enum_OrdinamentoImpianto.ImpresaCentroParticella

                Dim countorder As Integer = 0
                stbQ.Append(" ORDER BY " & vbCrLf)

                If (Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True) Then
                    countorder += 1
                    stbQ.Append(" rag_soc " & vbCrLf)
                End If

                If (Me.ChkGruppo1.Items.FindByValue(I_NomeCentro).Selected = True) Then
                    If countorder >= 1 Then
                        stbQ.Append(" , " & vbCrLf)
                    End If
                    stbQ.Append(" sa_nome " & vbCrLf)
                    countorder += 1
                End If

                If (Me.ChkGruppo3.Items.FindByValue(I_Particelle).Selected = True) Then
                    If countorder >= 1 Then
                        stbQ.Append(" , " & vbCrLf)
                    End If
                    stbQ.Append(" PROV,COM,SEZIONE, FOGLIO,NUMERO, SUBALTERNO " & vbCrLf)
                    countorder += 1
                End If

                If countorder > 0 Then
                    stbQ.Append(" ASC ")
                End If

        End Select

        stbQ.Append(vbCrLf)

        '/**************************************************
        Query4_TempTableJoin = stbQ.ToString

        stbQ = Nothing
        '/**************************************************



        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA  
        Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
        DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                Query2_TempTableIndice,
                                                                Query3_TempTableFill,
                                                                Query4_TempTableJoin,
                                                                 objParametri_Server.StringaConnessione,
                                                                 Messaggio)
        '/**************************************************

        If IsNothing(Messaggio) Then

            '    If (Not IsNothing(Rs)) AndAlso _
            '        (Rs.State <> 0) AndAlso _
            '            (Not Rs.EOF) Then

            If Not IsNothing(DT) Then

                If DT.Rows.Count <> 0 Then

                    'DT = objSQL.CreateDT_from_RS(Rs, errore)

                    ''basta clonarlo una sola volta
                    'If i = 0 Then
                    '    DT_Finale = DT.Clone
                    'End If

                    num_righe_dt = DT.Rows.Count

                    ' la j scorre i record (particelle intersecate dall'impianto)
                    For j = 0 To num_righe_dt - 1

                        'If (Me.ChkGruppo1.Items.FindByValue(I_Cod_Campo).Selected = True) Then
                        '    If CStr(DT.Rows(j).Item("campo_cod")) = "" Then
                        '        DT.Rows(j).Item("campo_cod") = -1
                        '    End If
                        'End If

                        If (Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("rag_soc")) = "") Then
                                DT.Rows(j).Item("rag_soc") = " "
                            End If
                        End If

                        'If (Me.ChkGruppo1.Items.FindByValue(I_Cod_Socio).Selected = True) Then
                        '    If (CStr(DT.Rows(j).Item("Codice_Socio")) = "") Then
                        '        DT.Rows(j).Item("Codice_Socio") = " "
                        '    End If
                        'End If

                        If (Me.ChkGruppo1.Items.FindByValue(I_Cuaa).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("CUAA")) = "") Then
                                DT.Rows(j).Item("CUAA") = " "
                            End If
                        End If


                        If (Me.ChkGruppo1.Items.FindByValue(I_IndImpresa).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("imp_ind_des")) = "") Then
                                DT.Rows(j).Item("imp_ind_des") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("imp_frz_des")) = "") Then
                                DT.Rows(j).Item("imp_frz_des") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("imp_CAP")) = "") Then
                                DT.Rows(j).Item("imp_CAP") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("imp_com_des")) = "") Then
                                DT.Rows(j).Item("imp_com_des") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("imp_pro_cod")) = "") Then
                                DT.Rows(j).Item("imp_pro_cod") = " "
                            End If
                        End If

                        If (Me.ChkGruppo1.Items.FindByValue(I_ImpresaPadre).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("PIVA_padre")) = "") Then
                                DT.Rows(j).Item("PIVA_padre") = " "
                            End If
                            If (CStr(DT.Rows(j).Item("coop_padre")) = "") Then
                                DT.Rows(j).Item("coop_padre") = " "
                            End If
                        End If

                        If (Me.ChkGruppo1.Items.FindByValue(I_IndCentro).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("cen_ind_des")) = "") Then
                                DT.Rows(j).Item("cen_ind_des") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("cen_frz_des")) = "") Then
                                DT.Rows(j).Item("cen_frz_des") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("cen_CAP")) = "") Then
                                DT.Rows(j).Item("cen_CAP") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("cen_com_des")) = "") Then
                                DT.Rows(j).Item("cen_com_des") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("cen_pro_cod")) = "") Then
                                DT.Rows(j).Item("cen_pro_cod") = " "
                            End If
                        End If

                        If (Me.ChkGruppo1.Items.FindByValue(I_CodiciIstat).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("pro_cod_istat")) = "") Then
                                DT.Rows(j).Item("pro_cod_istat") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("com_cod_istat")) = "") Then
                                DT.Rows(j).Item("com_cod_istat") = " "
                            End If
                        End If

                        If (Me.ChkGruppo1.Items.FindByValue(I_NomeCentro).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("sa_nome")) = "") Then
                                DT.Rows(j).Item("sa_nome") = " "
                            End If
                        End If

                        If (Me.ChkGruppo1.Items.FindByValue(I_NomeCampo).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("campo_des")) = "") Then
                                DT.Rows(j).Item("campo_des") = " "
                            End If
                        End If

                        If (Me.ChkGruppo2.Items.FindByValue(I_NomeAppezza).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("app_nome")) = "") Then
                                DT.Rows(j).Item("app_nome") = " "
                            End If
                        End If

                        If (Me.ChkGruppo2.Items.FindByValue(I_GruppoVeg).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("gru_des")) = "") Then
                                DT.Rows(j).Item("gru_des") = " "
                            End If
                        End If

                        'If (Me.ChkGruppo2.Items.FindByValue(I_Cod_Specie).Selected = True) Then
                        '    If (CStr(DT.Rows(j).Item("veg_cod")) = "") Then
                        '        DT.Rows(j).Item("veg_cod") = -1
                        '    End If
                        'End If

                        If (Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True) Or
                            (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Or
                                (Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("veg_des")) = "") Then
                                DT.Rows(j).Item("veg_des") = " "
                            End If
                            If (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Then
                                If (CStr(DT.Rows(j).Item("cul_des")) = "") Then
                                    DT.Rows(j).Item("cul_des") = " "
                                End If
                            End If
                            If (CStr(DT.Rows(j).Item("sup_imp")) = "") Then
                                DT.Rows(j).Item("sup_imp") = -1.0
                            End If
                            If CStr(DT.Rows(j).Item("cul_des")) = " " Then
                                'se c'è la superficie, ma non la varietà, significa che è terreno nudo
                                If DT.Rows(j).Item("sup_imp") <> -1.0 Then
                                    DT.Rows(j).Item("veg_des") = "Terreno Nudo"
                                End If
                            End If
                        End If

                        If (Me.ChkGruppo2.Items.FindByValue(I_TipVar).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("grva_des")) = "") Then
                                DT.Rows(j).Item("grva_des") = " "
                            End If
                        End If

                        If (Me.ChkGruppo2.Items.FindByValue(I_Finalita).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("grfi_des")) = "") Then
                                DT.Rows(j).Item("grfi_des") = " "
                            End If
                        End If

                        If (Me.ChkGruppo3.Items.FindByValue(I_Copertura).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("cop_des")) = "") Then
                                DT.Rows(j).Item("cop_des") = " "
                            End If
                        End If

                        If (Me.ChkGruppo3.Items.FindByValue(I_SemTrapData).Selected = True) Then

                            If Split(CStr(DT.Rows(j).Item("LavCod_Data_Semina")), "|")(0) <> "" Then
                                DT.Rows(j).Item("lav_cod_imp") = CInt(Split(CStr(DT.Rows(j).Item("LavCod_Data_Semina")), "|")(0))
                            Else
                                DT.Rows(j).Item("lav_cod_imp") = -1
                            End If

                            If Split(CStr(DT.Rows(j).Item("LavCod_Data_Semina")), "|")(1) <> "" Then
                                DT.Rows(j).Item("data_semina") = CDate(Split(CStr(DT.Rows(j).Item("LavCod_Data_Semina")), "|")(1)).ToShortDateString
                            Else
                                DT.Rows(j).Item("data_semina") = "01/01/1900"
                            End If

                            'If CStr(DT.Rows(j).Item("lav_cod_imp")) = "" Then
                            '    DT.Rows(j).Item("lav_cod_imp") = -1
                            'End If
                        End If

                        If (Me.ChkGruppo3.Items.FindByValue(I_Piante_Resa).Selected = True) Then

                            DT.Rows(j).Item("P_HA") = CDbl(Split(CStr(DT.Rows(j).Item("piante_resa")), "|")(0))

                            DT.Rows(j).Item("resa_prevista") = CDbl(Split(CStr(DT.Rows(j).Item("piante_resa")), "|")(1))

                            DT.Rows(j).Item("P_TOT") = Math.Round((CDbl(DT.Rows(j).Item("P_HA")) * CDbl(DT.Rows(j).Item("SUP_IMP"))), 0)


                        End If

                        '16/11/2017: non serve più
                        'If (Me.ChkGruppo3.Items.FindByValue(I_OrgRef_MagConf).Selected = True) Then
                        '    If CStr(DT.Rows(j).Item("coop_referente_bis")) <> "" Then
                        '        DT.Rows(j).Item("coop_referente") = DT.Rows(j).Item("coop_referente_bis")
                        '    End If
                        'End If

                        If (Me.ChkGruppo3.Items.FindByValue(I_Particelle).Selected = True) Then

                            If (CStr(DT.Rows(j).Item("part_com_des")) = "") Then
                                DT.Rows(j).Item("part_com_des") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("part_pro_cod")) = "") Then
                                DT.Rows(j).Item("part_pro_cod") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("PROV")) = "") Then
                                DT.Rows(j).Item("PROV") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("COM")) = "") Then
                                DT.Rows(j).Item("COM") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("SEZIONE")) = "") Then
                                DT.Rows(j).Item("SEZIONE") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("FOGLIO")) = "") Then
                                DT.Rows(j).Item("FOGLIO") = -1
                            End If

                            If (CStr(DT.Rows(j).Item("NUMERO")) = "") Then
                                DT.Rows(j).Item("NUMERO") = -1
                            End If

                            If (CStr(DT.Rows(j).Item("SUBALTERNO")) = "") Then
                                DT.Rows(j).Item("SUBALTERNO") = " "
                            End If

                            If (CStr(DT.Rows(j).Item("ETTARI")) = "") Then
                                DT.Rows(j).Item("ETTARI") = -1
                            End If

                            If (CStr(DT.Rows(j).Item("ARE")) = "") Then
                                DT.Rows(j).Item("ARE") = -1
                            End If

                            If (CStr(DT.Rows(j).Item("CENTIARE")) = "") Then
                                DT.Rows(j).Item("CENTIARE") = -1
                            End If

                        End If

                        If (Me.ChkGruppo3.Items.FindByValue(I_Particelle).Selected = True) Then
                            If (CStr(DT.Rows(j).Item("AREA")) = "") Then
                                DT.Rows(j).Item("AREA") = -1.0
                            End If
                        End If

                        'DT_Finale.ImportRow(DT.Rows.Item(j))

                        If Me.ChkGruppo3.Items.FindByValue(I_Lotto_DateDistinta).Selected = True Then
                            DT.Rows(j).Item("lotto_distinta") = CStr(Split(CStr(DT.Rows(j).Item("date_distinta")), "|")(0))
                            DT.Rows(j).Item("inizio_distinta") = CDate(Split(CStr(DT.Rows(j).Item("date_distinta")), "|")(1)).ToShortDateString
                            DT.Rows(j).Item("fine_distinta") = CDate(Split(CStr(DT.Rows(j).Item("date_distinta")), "|")(2)).ToShortDateString
                        End If


                    Next

                    'CANCELLA LE INFORMAZIONI NON NECESSARIE

                    'GRUPPO 1

                    If Me.ChkGruppo1.Items.FindByValue(I_Piva).Selected = True Then
                        flag_campo = True
                    Else
                        DT.Columns.Remove("PIVA")
                    End If

                    'If Me.ChkGruppo1.Items.FindByValue(I_Cod_Centro).Selected = True Then
                    '    flag_campo = True
                    'Else
                    '    DT.Columns.Remove("sa_cod")
                    'End If

                    'If Me.ChkGruppo1.Items.FindByValue(I_Cod_Appezza).Selected = True Then
                    '    flag_campo = True
                    'Else
                    '    DT.Columns.Remove("appezza")
                    'End If

                    'If Me.ChkGruppo1.Items.FindByValue(I_Cod_Imp).Selected = True Then
                    '    flag_campo = True
                    'Else
                    '    DT.Columns.Remove("id_reg")
                    'End If


                    'GRUPPO 2

                    If (Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True) Or
                            (Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True) Or
                                (Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True) Then

                        If Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = False Then
                            DT.Columns.Remove("veg_des")
                        End If

                        If Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = False Then
                            DT.Columns.Remove("cul_des")
                        End If

                        If Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = False Then
                            DT.Columns.Remove("sup_imp")
                        End If

                    End If

                    'GRUPPO 3

                    If Me.ChkGruppo3.Items.FindByValue(I_Lotto_DateDistinta).Selected = True Then
                        DT.Columns.Remove("date_distinta")
                    End If

                    If Me.ChkGruppo3.Items.FindByValue(I_SemTrapData).Selected = True Then
                        DT.Columns.Remove("LavCod_Data_Semina")
                    End If

                    If Me.ChkGruppo3.Items.FindByValue(I_Piante_Resa).Selected = True Then
                        DT.Columns.Remove("piante_resa")
                    End If

                    '16/11/2017: non serve più
                    'If (Me.ChkGruppo3.Items.FindByValue(I_OrgRef_MagConf).Selected = True) Then
                    ''    DT.Columns.Remove("coop_referente_bis")
                    'End If

                    'Escludo la colonna MetodoProduzione_Des se sono nell'export
                    'in XML
                    If Me.ChkGruppo2.Items.FindByValue(I_DatiAppezza).Selected = True Then
                        If Me.RdBList_Esporta.SelectedValue = "B" Then
                            DT.Columns.Remove("MetodoProduzione_Des")
                        Else
                            DT.Columns.Remove("MetodoProduzione_Cod")
                        End If
                    End If


                    If flag_campo = False Then
                        AgroMsg = "Selezionare almeno un campo di cui si vuole fare l'esportazione!!"
                        Return AgroMsg
                        Exit Function
                    End If

                    'DT_Finale = DT.Copy

                    'Rs.Close() 'non serve perchè è già chiuso dalla funzione che crea il DT!

                Else 'dt rows 0

                    'NON CI SONO IMPIANTI!!!

                    Log += CStr(Date.Now) + "   La QUERY IMPIANTI non ha restituito alcun risultato per il seguente elenco di chiavi impianto: " + ElencoChiaviImpianto & vbCrLf & vbCrLf

                End If 'nothing

            Else 'dt nothing
                'nothing rs - state - eof
                'NON CI SONO IMPIANTI!!!

                'Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative all'impianto " + piva + " - " + CStr(sa_cod) + " - " + CStr(appezza) + " - " + CStr(id_reg) + ". " & vbCrLf & vbCrLf

                Log += CStr(Date.Now) + "   La QUERY IMPIANTI non ha restituito alcun risultato per il seguente elenco di chiavi impianto: " + ElencoChiaviImpianto & vbCrLf & vbCrLf

            End If 'nothing

        Else 'rs - state - eof

            Log += CStr(Date.Now) + "   Errore nella QUERY IMPIANTI: " + Messaggio & vbCrLf & vbCrLf

        End If 'messaggio

        'Next ' for num_elementi impianti
        'ORA LA QUERY VIENE ESEGUITA SOLO UNA VOLTA, NON PIU' DENTRO AL FOR

        '------------------------------------------------
        '----------------- FINE  IMPIANTI ---------------
        '------------------------------------------------

        Return AgroMsg

    End Function




    '############################################################################################################
    Private Function Esportazione_Centri(ByRef XmlDoc As XmlDocument, ByRef DT As DataTable, ByRef Log As String) As String

        Dim NomeRoutine As String = "Esportatore_Universale_2.Esportazione_Centri"

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim piva As String
        Dim sa_cod, appezza, id_reg As Integer

        Dim i As Integer ' la i scorre gli impianti e le imprese
        Dim j As Integer ' la j scorre le righe del DT

        Dim num_elementi As Integer
        Dim num_righe_dt As Integer
        Dim num_sfondi As Integer
        Dim num_organismi As Integer

        Dim DT_Finale As New DataTable
        'Dim Rs As ADODB.Recordset
        'Dim RsSfondi, RsOrganismi As ADODB.Recordset
        'Dim objSQL As New Codex_Utility.Sql
        ''Dim objSQL As New Codex_Utility.Sql
        'Dim StrSQL As String
        Dim stbQ As New System.Text.StringBuilder
        Dim DTSfondi, DTOrganismi As DataTable

        Dim errore, msg As String
        Dim Messaggio As String = ""
        Dim AgroMsg As String = ""
        Dim flag_campo As Boolean

        Dim ChiaveCentro As String = ""

        Dim ElencoChiaviCentro As String = ""

        XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

        If XML_FiltroStampa Is Nothing Then
            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")
        End If

        XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

        num_elementi = XMLs_VariabiliStampe.Count

        '------------------------------------------------
        '--------- INIZIO CICLO CENTRI AZIENDALI --------
        '------------------------------------------------    

        flag_campo = False
        Dim mappa, organismo As String

        Dim Sup_Totale As Double = 0
        Dim Sup_Bosco As Double = 0
        Dim Sup_Prati As Double = 0
        Dim Sup_Tare As Double = 0
        Dim SAU_Totale As Double = 0
        Dim SAU_Biologico As Double = 0
        Dim SAU_Conversione As Double = 0
        Dim SAU_Convenzionale As Double = 0

        '-----------------------------------------------
        ' la i scorre i centri
        '-----------------------------------------------
        For i = 0 To num_elementi - 1

            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

            If (IsNothing(XML_VariabiliStampe.GetAttribute("piva"))) Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("piva")) = "") Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            End If

            If (IsNothing(XML_VariabiliStampe.GetAttribute("sa_cod"))) Then
                msg = "IL SA_COD E' NULLO!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("sa_cod")) = "") Then
                msg = "IL SA_COD E' NULLO!!!" & vbCrLf
            End If

            If msg <> "" Then
                AgroMsg = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                Return AgroMsg
                Exit Function
            End If

            'Ricavo I CENTRI
            piva = XML_VariabiliStampe.GetAttribute("piva")
            sa_cod = XML_VariabiliStampe.GetAttribute("sa_cod")

            'Genero la chiave impianto
            ChiaveCentro = piva & "_" & sa_cod

            ElencoChiaviCentro += ",'" & ChiaveCentro & "'"

        Next

        'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
        ElencoChiaviCentro = Mid(ElencoChiaviCentro, 2)

        '---------------------------------------------
        ' VAI CON LA TERA-QUERY CENTRI AZIENDALI!!!!!!
        '---------------------------------------------
        ' NOTA: SE AL CENTRO AZIENDALE NON E' IMPOSTATO L'INDIRIZZO DI TIPO 1
        ' TALE CENTRO NON VIENE TIRATO SU DALLA QUERY!
        '-----------------------------------------------------------------------
        Try

            stbQ.Length = 0
            stbQ.AppendLine("")
            stbQ.AppendLine(" SELECT DISTINCT ")
            stbQ.AppendLine(" CASE   WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Centri_Aziendali.PIVA  ELSE Imprese.partitaIvaReale  END PIVA,  ")
            If (Me.ChkGruppo1.Items.FindByValue(1).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Imprese.rag_soc, ' ') AS rag_soc, ")
            End If
            stbQ.AppendLine(" Centri_Aziendali.sa_cod, ISNULL(Centri_Aziendali.sa_nome, ' ') AS sa_nome, ")
            If (Me.ChkGruppo1.Items.FindByValue(4).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(( SELECT TOP 1    descrizione ")
                stbQ.AppendLine("          FROM            Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1, Codici_Anagrafe")
                stbQ.AppendLine("          WHERE           Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND             Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND             Codici_Anagrafe.codice = Centri_Aziendali_Codici_1.id_cod ")
                stbQ.AppendLine("          AND             Centri_Aziendali_Codici_1.id_cod IN (101, 102, 103)), ' ') AS tipo_centro, ")
            End If
            If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Indirizzi.ind_des, ' ') AS ind_des, ISNULL(Indirizzi.frz_des, ' ') AS frz_des, ")
                stbQ.AppendLine(" ISNULL(ISTAT.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ")
                stbQ.AppendLine(" ISNULL(Indirizzi.pro_cod_istat, ' ') AS pro_cod_istat, ")
                stbQ.AppendLine(" ISNULL(Indirizzi.com_cod_istat, ' ') AS com_cod_istat, ")
            End If
            'stbQ.AppendLine(" Centri_Aziendali.Validita_Inizio AS inizio_centro, ")
            'stbQ.AppendLine(" Centri_Aziendali.Validita_Fine AS fine_centro, ")
            stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), Centri_Aziendali.Validita_Inizio, 103), ' ') AS inizio_centro,  ")
            stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), Centri_Aziendali.Validita_Fine, 103), ' ') AS fine_centro,  ")
            'stbQ.AppendLine(" REPLACE(CAST(ISNULL(Centri_Aziendali.Validita_Inizio, '01/01/1800') AS varchar), 'gen  1 1800 12:00AM', ' ') AS inizio_centro, ")
            'stbQ.AppendLine(" REPLACE(CAST(ISNULL(Centri_Aziendali.Validita_Fine, '01/01/1800') AS varchar), 'gen  1 1800 12:00AM', ' ') AS fine_centro, ")
            stbQ.AppendLine(" ISNULL(Centri_Aziendali.TitoloPossesso, -1) AS possesso_centro, ")
            If (Me.ChkGruppo1.Items.FindByValue(8).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 val_cod ")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 1000), ' ') AS tipo_attivita, ")
            End If
            If (Me.ChkGruppo1.Items.FindByValue(9).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 OTE_des ")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1, OrientamentoTecnicoEconomico AS OTE")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 1009 ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.val_cod = OTE.OTE_cod), ' ') AS cod_ote, ")
            End If
            'stbQ.AppendLine(" ISNULL(Centri_Aziendali.Sup_Totale, -1) AS sup_totale, ISNULL(Centri_Aziendali.Sup_SAU, -1) AS sup_sau, ")
            'stbQ.AppendLine(" ISNULL(Centri_Aziendali.Sup_Tare, -1) AS sup_tara, ISNULL(Centri_Aziendali.Sup_SAU_Convenzionale, -1) AS sau_convenz, ")
            'stbQ.AppendLine(" ISNULL(Centri_Aziendali.Sup_SAU_Conversione, -1) AS sau_convers, ISNULL(Centri_Aziendali.Sup_SAU_Biologico, -1) AS sau_bio, ")
            'stbQ.AppendLine(" ISNULL(Centri_Aziendali.Sup_Bosco, -1) AS sup_bosco, ISNULL(Centri_Aziendali.Sup_Prati, -1) AS sup_prato)" 'NO VIRGOLA, POTREBBE ESSERE L'ULTIMO!!!
            stbQ.AppendLine(" -1.0000 AS sup_totale, -1.0000 AS sup_sau, ")
            stbQ.AppendLine(" -1.0000 AS sup_tara, -1.0000 AS sau_convenz, ")
            stbQ.AppendLine(" -1.0000 AS sau_convers, -1.0000 AS sau_bio, ")
            stbQ.AppendLine(" -1.0000 AS sup_bosco, -1.0000 AS sup_prato") 'NO VIRGOLA, POTREBBE ESSERE L'ULTIMO!!!
            If (Me.ChkGruppo2.Items.FindByValue(18).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ' ' AS mappa")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(19).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(Fabbricati.Fabbricato_Des, ' ') AS fabbricato, ")
                stbQ.AppendLine(" ISNULL(Fabbricati_Tipi.Tipo_Fabbricato_Des, ' ') AS tipo_fabbricato")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(20).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 val_cod ")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 1003), ' ') AS cod_operatore")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(21).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 val_cod")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 1), ' ') AS cod_zoo")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(22).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 val_cod ")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1 ")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 2), ' ') AS cod_cerpl")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(23).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 val_cod ")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1 ")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 3), ' ') AS cod_aua")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(24).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 val_cod ")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1 ")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 4), ' ') AS cod_ausl")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(25).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(( SELECT      TOP 1 val_cod ")
                stbQ.AppendLine("          FROM        Centri_Aziendali_Codici AS Centri_Aziendali_Codici_1 ")
                stbQ.AppendLine("          WHERE       Centri_Aziendali_Codici_1.piva = Centri_Aziendali.piva ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine("          AND         Centri_Aziendali_Codici_1.id_cod = 5), ' ') AS cod_cnal")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(26).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ' ' AS organismi")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(27).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.PROV, ' ') AS PROV, ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.COM, ' ') AS COM, ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.SEZIONE, ' ') AS SEZIONE, ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.FOGLIO, -1) AS FOGLIO, ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.NUMERO, -1) AS NUMERO, ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.SUBALTERNO, ' ') AS SUBALTERNO")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(28).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(ParticelleCatastali.ETTARI, -1) AS ETTARI, ")
                stbQ.AppendLine(" ISNULL(ParticelleCatastali.[ARE], -1) AS ARE, ")
                stbQ.AppendLine(" ISNULL(ParticelleCatastali.CENTIARE, -1) AS CENTIARE")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(29).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.TitoloPossesso, -1) AS TitoloPossesso, ")
                stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Inizio, 103), ' ') AS dal, ")
                stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Fine, 103), ' ') AS al ")
                'stbQ.AppendLine(" REPLACE(CAST(ISNULL(ImpreseXParticelle.Validita_Inizio, '01/01/1800') AS varchar), 'gen  1 1800 12:00AM', ' ') AS dal, ")
                'stbQ.AppendLine(" REPLACE(CAST(ISNULL(ImpreseXParticelle.Validita_Fine, '01/01/1800') AS varchar), 'gen  1 1800 12:00AM', ' ') AS al ")
                'stbQ.AppendLine(" ImpreseXParticelle.Validita_Inizio AS dal, ")
                'stbQ.AppendLine(" ImpreseXParticelle.Validita_Fine AS al ")
            End If
            stbQ.AppendLine(" ")
            '-----
            'FROM
            '-----
            stbQ.AppendLine(" FROM Centri_Aziendali ")
            If (Me.ChkGruppo3.Items.FindByValue(27).Selected = True) Or (Me.ChkGruppo3.Items.FindByValue(28).Selected = True) Or (Me.ChkGruppo3.Items.FindByValue(29).Selected = True) Then
                stbQ.AppendLine(" LEFT OUTER JOIN ImpreseXParticelle ")
                stbQ.AppendLine(" ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA ")
                stbQ.AppendLine(" AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
            End If
            If (Me.ChkGruppo3.Items.FindByValue(28).Selected = True) Then
                stbQ.AppendLine(" LEFT OUTER JOIN ParticelleCatastali ")
                stbQ.AppendLine(" ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV ")
                stbQ.AppendLine(" AND ImpreseXParticelle.COM = ParticelleCatastali.COM ")
                stbQ.AppendLine(" AND ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                stbQ.AppendLine(" AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                stbQ.AppendLine(" AND ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                stbQ.AppendLine(" AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
            End If
            If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then
                stbQ.AppendLine(" INNER JOIN CentrixIndirizzi ")
                stbQ.AppendLine(" ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA ")
                stbQ.AppendLine(" AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
                stbQ.AppendLine(" INNER JOIN Indirizzi ")
                stbQ.AppendLine(" ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ")
                stbQ.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            End If
            If (Me.ChkGruppo1.Items.FindByValue(1).Selected = True) Then
                stbQ.AppendLine(" INNER JOIN Imprese ")
                stbQ.AppendLine(" ON Centri_Aziendali.PIVA = Imprese.PIVA ")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(19).Selected = True) Then
                stbQ.AppendLine(" LEFT OUTER JOIN Fabbricati ")
                stbQ.AppendLine(" ON Centri_Aziendali.PIVA = Fabbricati.PIVA ")
                stbQ.AppendLine(" AND Centri_Aziendali.sa_cod = Fabbricati.SA_COD ")
                stbQ.AppendLine(" LEFT OUTER JOIN Fabbricati_Tipi ")
                stbQ.AppendLine(" ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
            End If
            'If (Me.ChkGruppo2.Items.FindByValue(18).Selected = True) Then
            '    stbQ.AppendLine(" LEFT OUTER JOIN CentriXSfondi ")
            '    stbQ.AppendLine(" ON Centri_Aziendali.PIVA = CentriXSfondi.PIVA ")
            '    stbQ.AppendLine(" AND Centri_Aziendali.sa_cod = CentriXSfondi.SA_COD ")
            'End If
            '-----
            'WHERE
            '-----

            'stbQ.AppendLine(" WHERE (Centri_Aziendali.PIVA = " & Agro_SQL_SaveText(piva) & ") ")
            'stbQ.AppendLine(" AND (Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & ") ")

            stbQ.AppendLine(" WHERE               ((Centri_Aziendali.PIVA  ")
            stbQ.AppendLine("                     + '_' + CONVERT(varchar(10), Centri_Aziendali.SA_COD))  ")
            stbQ.AppendLine("                     IN (" & Agro_SQL_Save_Clausola_IN(ElencoChiaviCentro, True) & "))  ")


            If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then
                stbQ.AppendLine(" AND (CentrixIndirizzi.Tipo_Indirizzo = 1) ")
            End If

            'ORDINAMENTO
            If (Me.ChkGruppo1.Items.FindByValue(1).Selected = True) Then
                stbQ.AppendLine(" ORDER BY rag_soc, sa_nome")

                If (Me.ChkGruppo3.Items.FindByValue(27).Selected = True) Then
                    stbQ.AppendLine(" , PROV, COM, SEZIONE, ")
                    stbQ.AppendLine(" FOGLIO, NUMERO, SUBALTERNO ")
                End If
            Else

                If (Me.ChkGruppo3.Items.FindByValue(27).Selected = True) Then
                    stbQ.AppendLine(" ORDER BY sa_nome, PROV, COM, SEZIONE, ")
                    stbQ.AppendLine(" FOGLIO, NUMERO, SUBALTERNO ")
                Else
                    stbQ.AppendLine(" ORDER BY sa_nome ")
                End If
            End If

            stbQ.AppendLine(vbCrLf)


            'Recupero il datatable
            'Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
            '                      Session("ASG_Connessione_Server"),
            '                      StrSQL,
            '                      0,
            '                      Messaggio)

            '--------------------------------------------------------------------------
            DT = DataProvider.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            'If (Not IsNothing(Rs)) AndAlso
            '    (Rs.State <> 0) AndAlso
            '        (Not Rs.EOF) Then

            If Not IsNothing(DT) AndAlso
               DT.Rows.Count <> 0 Then

                '############################################################
                '##################### SFONDI ###############################

                If (Me.ChkGruppo2.Items.FindByValue(18).Selected = True) Then

                    'flag_campo = True
                    stbQ.Length = 0
                    stbQ.AppendLine("")
                    stbQ.AppendLine(" SELECT ISNULL(CentriXSfondi.SfondoCod, -1) AS sfondixcentro ")
                    stbQ.AppendLine(" FROM Centri_Aziendali ")
                    stbQ.AppendLine(" LEFT OUTER JOIN CentriXSfondi ")
                    stbQ.AppendLine(" ON Centri_Aziendali.PIVA = CentriXSfondi.PIVA ")
                    stbQ.AppendLine(" AND Centri_Aziendali.sa_cod = CentriXSfondi.SA_COD ")
                    stbQ.AppendLine(" WHERE (Centri_Aziendali.PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
                    stbQ.AppendLine(" AND (Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & ") ")

                    'Recupero il datatable
                    'RsSfondi = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                    '                            Session("ASG_Connessione_Server"),
                    '                            StrSQL,
                    '                            0,
                    '                            Messaggio)

                    '--------------------------------------------------------------------------
                    DTSfondi = DataProvider.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    'If (Not IsNothing(RsSfondi)) AndAlso
                    '    (RsSfondi.State <> 0) AndAlso
                    '        (Not RsSfondi.EOF) Then

                    '    num_sfondi = RsSfondi.RecordCount

                    If Not IsNothing(DTSfondi) AndAlso
                                    DTSfondi.Rows.Count <> 0 Then

                        num_sfondi = DTSfondi.Rows.Count

                        mappa = ""

                        'For z = 0 To num_sfondi
                        'Do While Not RsSfondi.EOF

                        '    If CInt(RsSfondi.Fields("sfondixcentro").Value) <> -1 Then

                        '        If CInt(RsSfondi.Fields("sfondixcentro").Value) = 1 Then

                        '            mappa = mappa & "C.T.R. (Carta Tecnica Regionale)<br/>"

                        '        ElseIf CInt(RsSfondi.Fields("sfondixcentro").Value) = 2 Then

                        '            mappa = mappa & "Foto Aerea"

                        '        End If

                        '    End If

                        '    'Next
                        '    RsSfondi.MoveNext()

                        'Loop

                        'RsSfondi.Close()
                        'RsSfondi = Nothing

                        For Each dr As DataRow In DTSfondi.Rows
                            If CInt(dr.Item("sfondixcentro")) <> -1 Then

                                If CInt(dr.Item("sfondixcentro")) = 1 Then

                                    mappa = mappa & "C.T.R. (Carta Tecnica Regionale)<br/>"

                                ElseIf CInt(dr.Item("sfondixcentro")) = 2 Then

                                    mappa = mappa & "Foto Aerea"

                                End If

                            End If
                        Next

                    Else 'nothing rs - state - eof

                        mappa = " "

                    End If 'rs


                End If

                '###################### FINE SFONDI ###################################

                '########################################################################
                '###################### ORGANISMI CONTROLLO BIOLOGICO ###################

                If (Me.ChkGruppo3.Items.FindByValue(26).Selected = True) Then

                    'flag_campo = True
                    stbQ.Length = 0
                    stbQ.AppendLine("")
                    stbQ.AppendLine(" SELECT ISNULL(Codici_Anagrafe.descrizione, 'n.d.') AS organismo, ")
                    stbQ.AppendLine(" Centri_Aziendali_Codici.Validita_Inizio AS org_inizio, Centri_Aziendali_Codici.Validita_Fine AS org_fine ")
                    stbQ.AppendLine(" FROM Centri_Aziendali_Codici ")
                    stbQ.AppendLine(" INNER JOIN Codici_Anagrafe ")
                    stbQ.AppendLine(" ON Centri_Aziendali_Codici.id_cod = Codici_Anagrafe.codice ")
                    stbQ.AppendLine(" WHERE (Codici_Anagrafe.gruppo = 'ORGANISMO') ")
                    stbQ.AppendLine(" AND (Centri_Aziendali_Codici.PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
                    stbQ.AppendLine(" AND (Centri_Aziendali_Codici.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & ") ")

                    'Recupero il datatable
                    'RsOrganismi = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                    '                                Session("ASG_Connessione_Server"),
                    '                                StrSQL,
                    '                                0,
                    '                                Messaggio)

                    '--------------------------------------------------------------------------
                    DTOrganismi = DataProvider.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    'If (Not IsNothing(RsOrganismi)) AndAlso
                    '    (RsOrganismi.State <> 0) AndAlso
                    '        (Not RsOrganismi.EOF) Then

                    '    num_organismi = RsOrganismi.RecordCount

                    If Not IsNothing(DTOrganismi) AndAlso
                                    DTOrganismi.Rows.Count <> 0 Then

                        num_organismi = DTOrganismi.Rows.Count

                        organismo = ""

                        'For z = 0 To num_sfondi
                        'Do While Not RsOrganismi.EOF

                        '    If CStr(RsOrganismi.Fields("organismo").Value) <> "n.d." Then

                        '        organismo = organismo & CStr(RsOrganismi.Fields("organismo").Value) & " dal " & CStr(RsOrganismi.Fields("org_inizio").Value) & " al " & CStr(RsOrganismi.Fields("org_fine").Value) & "<br/>"

                        '    End If

                        '    'Next
                        '    RsOrganismi.MoveNext()

                        'Loop

                        'RsOrganismi.Close()
                        'RsOrganismi = Nothing

                        For Each dr As DataRow In DTOrganismi.Rows
                            If CStr(dr.Item("organismo")) <> "n.d." Then

                                organismo = organismo & CStr(dr.Item("organismo")) & " dal " & CStr(dr.Item("org_inizio")) & " al " & CStr(dr.Item("org_fine")) & "<br/>"

                            End If
                        Next

                    Else 'nothing rs - state - eof

                        organismo = " "

                    End If 'nothing

                End If

                '############################ FINE ORGANISMI #####################################

                '###################### SUPERFICI ####################################

                If (Me.ChkGruppo2.Items.FindByValue(10).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(11).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(12).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(13).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(14).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(15).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(16).Selected = True) Or
                        (Me.ChkGruppo2.Items.FindByValue(17).Selected = True) Then

                    Call objCentriAz.Recupera_Superfici_CentroAziendale(piva, sa_cod, Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, Date.Today, objParametri_Server)

                End If

                '#####################################################################


                'basta clonarlo una sola volta
                DT_Finale = DT.Clone

                num_righe_dt = DT.Rows.Count

                ' ******** FOR *************
                ' la j scorre i record
                For j = 0 To num_righe_dt - 1

                    If (Me.ChkGruppo1.Items.FindByValue(1).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("rag_soc")) = "") Then
                            DT.Rows(j).Item("rag_soc") = " "
                        End If
                    End If

                    If (CStr(DT.Rows(j).Item("sa_nome")) = "") Then
                        DT.Rows(j).Item("sa_nome") = " "
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(4).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("tipo_centro")) = "") Then
                            DT.Rows(j).Item("tipo_centro") = " "
                        End If
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then

                        If (CStr(DT.Rows(j).Item("ind_des")) = "") Then
                            DT.Rows(j).Item("ind_des") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("frz_des")) = "") Then
                            DT.Rows(j).Item("frz_des") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("CAP")) = "") Then
                            DT.Rows(j).Item("CAP") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("com_des")) = "") Then
                            DT.Rows(j).Item("com_des") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("pro_cod")) = "") Or CStr((DT.Rows(j).Item("pro_cod")) = "0") Then
                            DT.Rows(j).Item("pro_cod") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("pro_cod_istat")) = "") Then
                            DT.Rows(j).Item("pro_cod_istat") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("com_cod_istat")) = "") Then
                            DT.Rows(j).Item("com_cod_istat") = " "
                        End If

                    End If

                    If (CStr(DT.Rows(j).Item("possesso_centro")) = "") Then
                        DT.Rows(j).Item("possesso_centro") = -1
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(8).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("tipo_attivita")) = "") Then
                            DT.Rows(j).Item("tipo_attivita") = " "
                        End If
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(9).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("cod_ote")) = "") Then
                            DT.Rows(j).Item("cod_ote") = " "
                        End If
                    End If

                    If (CStr(DT.Rows(j).Item("sup_totale")) = "") Then
                        DT.Rows(j).Item("sup_totale") = -1.0
                    End If

                    If (CStr(DT.Rows(j).Item("sup_sau")) = "") Then
                        DT.Rows(j).Item("sup_sau") = -1.0
                    End If

                    If (CStr(DT.Rows(j).Item("sup_tara")) = "") Then
                        DT.Rows(j).Item("sup_tara") = -1.0
                    End If

                    If (CStr(DT.Rows(j).Item("sau_convenz")) = "") Then
                        DT.Rows(j).Item("sau_convenz") = -1.0
                    End If

                    If (CStr(DT.Rows(j).Item("sau_convers")) = "") Then
                        DT.Rows(j).Item("sau_convers") = -1.0
                    End If

                    If (CStr(DT.Rows(j).Item("sau_bio")) = "") Then
                        DT.Rows(j).Item("sau_bio") = -1.0
                    End If

                    If (CStr(DT.Rows(j).Item("sup_bosco")) = "") Then
                        DT.Rows(j).Item("sup_bosco") = -1.0
                    End If

                    If (CStr(DT.Rows(j).Item("sup_prato")) = "") Then
                        DT.Rows(j).Item("sup_prato") = -1.0
                    End If

                    If (Me.ChkGruppo2.Items.FindByValue(19).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("fabbricato")) = "") Then
                            DT.Rows(j).Item("fabbricato") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("tipo_fabbricato")) = "") Then
                            DT.Rows(j).Item("tipo_fabbricato") = " "
                        End If
                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(20).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("cod_operatore")) = "") Then
                            DT.Rows(j).Item("cod_operatore") = " "
                        End If
                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(21).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("cod_zoo")) = "") Then
                            DT.Rows(j).Item("cod_zoo") = " "
                        End If
                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(22).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("cod_cerpl")) = "") Then
                            DT.Rows(j).Item("cod_cerpl") = " "
                        End If
                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(23).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("cod_aua")) = "") Then
                            DT.Rows(j).Item("cod_aua") = " "
                        End If
                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(24).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("cod_ausl")) = "") Then
                            DT.Rows(j).Item("cod_ausl") = " "
                        End If
                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(25).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("cod_cnal")) = "") Then
                            DT.Rows(j).Item("cod_cnal") = " "
                        End If
                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(27).Selected = True) Then

                        If (CStr(DT.Rows(j).Item("PROV")) = "") Then
                            DT.Rows(j).Item("PROV") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("COM")) = "") Then
                            DT.Rows(j).Item("COM") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("SEZIONE")) = "") Then
                            DT.Rows(j).Item("SEZIONE") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("FOGLIO")) = "") Then
                            DT.Rows(j).Item("FOGLIO") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("NUMERO")) = "") Then
                            DT.Rows(j).Item("NUMERO") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("SUBALTERNO")) = "") Then
                            DT.Rows(j).Item("SUBALTERNO") = " "
                        End If

                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(28).Selected = True) Then

                        If (CStr(DT.Rows(j).Item("ETTARI")) = "") Then
                            DT.Rows(j).Item("ETTARI") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("ARE")) = "") Then
                            DT.Rows(j).Item("ARE") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("CENTIARE")) = "") Then
                            DT.Rows(j).Item("CENTIARE") = -1
                        End If

                    End If

                    If (Me.ChkGruppo3.Items.FindByValue(29).Selected = True) Then

                        If (CStr(DT.Rows(j).Item("TitoloPossesso")) = "") Then
                            DT.Rows(j).Item("TitoloPossesso") = -1
                        End If

                    End If

                    If Me.ChkGruppo2.Items.FindByValue(18).Selected = True Then
                        'nel dt finale vado a riempire il campo mappa
                        DT.Rows(j).Item("mappa") = mappa
                    End If

                    If Me.ChkGruppo3.Items.FindByValue(26).Selected = True Then
                        'nel dt finale vado a riempire il campo organismo
                        DT.Rows(j).Item("organismi") = organismo
                    End If

                    If (Me.ChkGruppo2.Items.FindByValue(10).Selected = True) Then
                        DT.Rows(j).Item("sup_totale") = Sup_Totale
                    End If
                    If (Me.ChkGruppo2.Items.FindByValue(11).Selected = True) Then
                        DT.Rows(j).Item("sup_sau") = SAU_Totale
                    End If
                    If (Me.ChkGruppo2.Items.FindByValue(12).Selected = True) Then
                        DT.Rows(j).Item("sup_tara") = Sup_Tare
                    End If
                    If (Me.ChkGruppo2.Items.FindByValue(13).Selected = True) Then
                        DT.Rows(j).Item("sau_convenz") = SAU_Convenzionale
                    End If
                    If (Me.ChkGruppo2.Items.FindByValue(14).Selected = True) Then
                        DT.Rows(j).Item("sau_convers") = SAU_Conversione
                    End If
                    If (Me.ChkGruppo2.Items.FindByValue(15).Selected = True) Then
                        DT.Rows(j).Item("sau_bio") = SAU_Biologico
                    End If
                    If (Me.ChkGruppo2.Items.FindByValue(16).Selected = True) Then
                        DT.Rows(j).Item("sup_bosco") = Sup_Bosco
                    End If
                    If (Me.ChkGruppo2.Items.FindByValue(17).Selected = True) Then
                        DT.Rows(j).Item("sup_prato") = Sup_Prati
                    End If


                    DT_Finale.ImportRow(DT.Rows.Item(j))

                Next

                'Rs.Close() 'non serve!

            Else 'nothing rs - state - eof

                'NON CI SONO CENTRI!!!

                Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative al centro " + piva + " - " + CStr(sa_cod) + ". " & vbCrLf & vbCrLf

            End If 'nothing rs - state - eof

        Catch ex As Exception
            Log += CStr(Date.Now) + "   Errore nella TERA QUERY CENTRI AZIENDALI: " + ex.Message & vbCrLf & vbCrLf
        End Try


        'Next ' for num_elementi centri

        '------------------------------------------------
        '---------- FINE CICLO CENTRI AZIENDALI ---------
        '------------------------------------------------

        'CANCELLO LE INFORMAZIONI NON RICHIESTE

        'GRUPPO 1

        If Me.ChkGruppo1.Items.FindByValue(0).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("PIVA")
        End If

        If Me.ChkGruppo1.Items.FindByValue(2).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sa_cod")
        End If

        If Me.ChkGruppo1.Items.FindByValue(3).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sa_nome")
        End If

        If Me.ChkGruppo1.Items.FindByValue(6).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("inizio_centro")
            DT_Finale.Columns.Remove("fine_centro")
        End If

        If Me.ChkGruppo1.Items.FindByValue(7).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("possesso_centro")
        End If

        ' GRUPPO 2

        If Me.ChkGruppo2.Items.FindByValue(10).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sup_totale")
        End If

        If Me.ChkGruppo2.Items.FindByValue(11).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sup_sau")
        End If

        If Me.ChkGruppo2.Items.FindByValue(12).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sup_tara")
        End If

        If Me.ChkGruppo2.Items.FindByValue(13).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sau_convenz")
        End If

        If Me.ChkGruppo2.Items.FindByValue(14).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sau_convers")
        End If

        If Me.ChkGruppo2.Items.FindByValue(15).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sau_bio")
        End If

        If Me.ChkGruppo2.Items.FindByValue(16).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sup_bosco")
        End If

        If Me.ChkGruppo2.Items.FindByValue(17).Selected = True Then
            flag_campo = True
        Else
            DT_Finale.Columns.Remove("sup_prato")
        End If



        DT = Nothing
        DT = DT_Finale.Copy
        DT_Finale = Nothing


        If flag_campo = False Then
            AgroMsg = "Selezionare almeno un campo di cui si vuole fare l'esportazione!!"
        End If

        Return AgroMsg


    End Function

    '############################################################################################################
    Private Function Esportazione_Imprese(ByRef XmlDoc As XmlDocument, ByRef DT As DataTable, ByRef Log As String) As String

        Dim NomeRoutine As String = "Esportatore_Universale_2.Esportazione_Imprese"

        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim piva As String
        Dim sa_cod, appezza, id_reg As Integer

        Dim i As Integer ' la i scorre gli impianti e le imprese
        Dim j As Integer ' la j scorre le righe del DT

        Dim num_elementi As Integer
        Dim num_righe_dt As Integer

        'Dim DT_Finale As New DataTable
        'Dim Rs As ADODB.Recordset
        'Dim objSQL As New Codex_Utility.Sql
        ''Dim objSQL As New Codex_Utility.Sql
        'Dim StrSQL As String
        Dim stbQ As New System.Text.StringBuilder

        Dim errore, msg As String
        Dim Messaggio As String = ""
        Dim AgroMsg As String = ""
        Dim flag_campo As Boolean


        'XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
        XML_FiltroStampa = XmlDoc.GetElementsByTagName("FiltroStampa")(0)


        XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

        num_elementi = XMLs_VariabiliStampe.Count

        '------------------------------------------------
        '------------ INIZIO CICLO IMPRESE -------------
        '------------------------------------------------

        Dim ElencoChiaviImpresa As String

        flag_campo = False
        Dim array(3) As String

        '-----------------------------------------------
        ' la i scorre le imprese
        '-----------------------------------------------
        For i = 0 To num_elementi - 1

            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

            If (IsNothing(XML_VariabiliStampe.GetAttribute("piva"))) Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("piva")) = "") Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            End If

            If msg <> "" Then
                AgroMsg = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                Return AgroMsg
                Exit Function
            End If

            'Ricavo la piva
            piva = XML_VariabiliStampe.GetAttribute("piva")

            'la chiave impresa è la piva

            ElencoChiaviImpresa += ",'" & piva & "'"

        Next


        'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
        ElencoChiaviImpresa = Mid(ElencoChiaviImpresa, 2)


        '---------------------------------------------
        ' VAI CON LA TERA-QUERY IMPRESE!!!!!!!!
        '---------------------------------------------
        Try
            stbQ.Length = 0
            stbQ.AppendLine(" ")
            stbQ.Append(" SELECT DISTINCT CASE  ")
            stbQ.Append(" WHEN ISNULL(Impresa.partitaIvaReale, '') = '' THEN Impresa.PIVA ")
            stbQ.Append(" ELSE Impresa.partitaIvaReale ")
            stbQ.Append(" END PIVA " & vbCrLf)
            stbQ.AppendLine(", ISNULL(Impresa.rag_soc, ' ') AS rag_soc, ")
            stbQ.AppendLine(" ISNULL(Impresa.TipoImpresaGerarchia, -1) AS tipo_impresa, ")
            If (Me.ChkGruppo1.Items.FindByValue(3).Selected = True) Then
                flag_campo = True
                ' query interna per recuperare il Codice Socio
                stbQ.AppendLine(" ISNULL((SELECT TOP 1 val_cod ")
                stbQ.AppendLine("         FROM Imprese_Codici AS Imprese_Codici_Interno ")
                stbQ.AppendLine("         WHERE Imprese_Codici_Interno.piva = Impresa.piva ")
                stbQ.AppendLine("         AND Imprese_Codici_Interno.id_cod = 1033), ' ') AS Codice_Socio, ")
                'fine
            End If
            If (Me.ChkGruppo1.Items.FindByValue(4).Selected = True) Then
                flag_campo = True
                ' query interna per recuperare il CUAA
                stbQ.AppendLine(" ISNULL((SELECT TOP 1 val_cod ")
                stbQ.AppendLine("         FROM Imprese_Codici AS Imprese_Codici_CUAA ")
                stbQ.AppendLine("         WHERE Imprese_Codici_CUAA.piva = Impresa.piva ")
                stbQ.AppendLine("         AND Imprese_Codici_CUAA.id_cod = 1010), ' ') AS CUAA, ")
                'fine
            End If
            'controllo se sono state selezionate le cooperative padre
            If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(coop.rag_soc, ' ') AS coop_padre, ")
            End If
            If (Me.ChkGruppo1.Items.FindByValue(6).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(coop.PIVA, ' ') AS PIVA_padre, ")
            End If
            'fine cooperativa
            If (Me.ChkGruppo1.Items.FindByValue(7).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Indirizzo_sede.ind_des, ' ') AS ind_des, ISNULL(Indirizzo_sede.frz_des, ' ') AS frz_des, ")
                stbQ.AppendLine(" ISNULL(ISTAT.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ")
            End If
            If (Me.ChkGruppo1.Items.FindByValue(8).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ISNULL(Indirizzo_sede.pro_cod_istat, ' ') AS pro_cod_istat, ISNULL(Indirizzo_sede.com_cod_istat, ' ') AS com_cod_istat, ")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(10).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(11).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(12).Selected = True) Then
                stbQ.AppendLine(" ISNULL((SELECT TOP 1 Contatti.Cod_Contatto + '|' + Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome + '|' + ISNULL(ISTAT.LOCALITA, '') + '|' + ISNULL(ISTAT.COMUNI_PROV,'') AS legale ")
                stbQ.AppendLine("         FROM Contatti")
                stbQ.AppendLine("         INNER JOIN ContattiXIndirizzi ")
                stbQ.AppendLine("         ON Contatti.Piva = ContattiXIndirizzi.Piva")
                stbQ.AppendLine("         AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto")
                stbQ.AppendLine("         INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
                stbQ.AppendLine("         LEFT OUTER JOIN Indirizzi Indirizzo_legale ")
                stbQ.AppendLine("         ON Indirizzo_legale.cod_indirizzo = ContattiXIndirizzi.Cod_Indirizzo")
                stbQ.AppendLine("         LEFT OUTER JOIN ISTAT ON Indirizzo_legale.pro_cod_istat = ISTAT.PROV ")
                stbQ.AppendLine("         AND Indirizzo_legale.com_cod_istat = ISTAT.COM ")
                stbQ.AppendLine("         WHERE (Contatti.PIVA = Impresa.PIVA) ")
                stbQ.AppendLine("         AND (ContattiXIndirizzi.Tipo_Indirizzo = 5) AND Risorse_Umane.Cod_Rapporto = -1 ), 'n.d.') AS legale, ")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(10).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ' ' AS CF_legale, ")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(11).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ' ' AS rappr_legale, ")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(12).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(" ' ' AS com_legale, ' ' AS pro_legale, ")
            End If
            stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), Impresa.Validita_Inizio, 103), ' ') AS inizio_impresa, ")
            stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), Impresa.Validita_Fine, 103), ' ') AS fine_impresa") '---------no virgola, no spazio----------- !!!!!!!
            'controllo se sono state selezionate le particelle
            If (Me.ChkGruppo2.Items.FindByValue(13).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.PROV, ' ') AS PROV, ISNULL(ImpreseXParticelle.COM, ' ') AS COM, ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.SEZIONE, ' ') AS SEZIONE, ISNULL(ImpreseXParticelle.FOGLIO, -1) AS FOGLIO, ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.NUMERO, -1) AS NUMERO, ISNULL(ImpreseXParticelle.SUBALTERNO, ' ') AS SUBALTERNO")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(14).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(ImpreseXParticelle.TitoloPossesso, -1) AS TitoloPossesso")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(15).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Inizio, 103), ' ') AS dal, ")
                stbQ.AppendLine(" ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Fine, 103), ' ') AS al")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(16).Selected = True) Then
                flag_campo = True
                stbQ.AppendLine(", ")
                stbQ.AppendLine(" ISNULL(ParticelleCatastali.ETTARI, -1) AS ETTARI, ISNULL(ParticelleCatastali.ARE, -1) AS ARE, ISNULL(ParticelleCatastali.CENTIARE, -1) AS CENTIARE")
            End If
            stbQ.AppendLine(" ")
            'FROM
            stbQ.AppendLine(" FROM Imprese Impresa ")
            If (Me.ChkGruppo1.Items.FindByValue(7).Selected = True) Or (Me.ChkGruppo1.Items.FindByValue(8).Selected = True) Then
                stbQ.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzo_sede ")
                stbQ.AppendLine(" INNER JOIN ImpresexIndirizzi ")
                stbQ.AppendLine(" ON Indirizzo_sede.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo ")
                stbQ.AppendLine(" ON Impresa.PIVA = ImpresexIndirizzi.PIVA ")
                stbQ.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzo_sede.pro_cod_istat = ISTAT.PROV AND Indirizzo_sede.com_cod_istat = ISTAT.COM ")
            End If
            'controllo se sono state selezionate le cooperative padre
            If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Or (Me.ChkGruppo1.Items.FindByValue(6).Selected = True) Then
                stbQ.AppendLine(" LEFT OUTER JOIN Imprese coop ")
                stbQ.AppendLine(" INNER JOIN GerarchiaImprese ")
                stbQ.AppendLine(" ON coop.PIVA = GerarchiaImprese.Padre ")
                stbQ.AppendLine(" ON Impresa.PIVA = GerarchiaImprese.Figlio")
            End If
            'controllo se sono state selezionate le particelle
            If (Me.ChkGruppo2.Items.FindByValue(13).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(14).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(15).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(16).Selected = True) Then
                stbQ.AppendLine(" LEFT OUTER JOIN ImpreseXParticelle ")
                stbQ.AppendLine(" ON Impresa.PIVA = ImpreseXParticelle.PIVA ")
            End If
            If (Me.ChkGruppo2.Items.FindByValue(16).Selected = True) Then
                stbQ.AppendLine(" LEFT OUTER JOIN ParticelleCatastali ")
                stbQ.AppendLine(" ON ParticelleCatastali.PROV = ImpreseXParticelle.PROV ")
                stbQ.AppendLine(" AND ParticelleCatastali.COM = ImpreseXParticelle.COM ")
                stbQ.AppendLine(" AND ParticelleCatastali.SEZIONE = ImpreseXParticelle.SEZIONE ")
                stbQ.AppendLine(" AND ParticelleCatastali.FOGLIO = ImpreseXParticelle.FOGLIO ")
                stbQ.AppendLine(" AND ParticelleCatastali.NUMERO = ImpreseXParticelle.NUMERO ")
                stbQ.AppendLine(" AND ParticelleCatastali.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ")
            End If

            'WHERE
            'stbQ.AppendLine(" WHERE Impresa.PIVA = '" & SQL_SaveText(piva) & "' ")

            stbQ.AppendLine(" WHERE (Impresa.PIVA IN (" & Agro_SQL_Save_Clausola_IN(ElencoChiaviImpresa, True) & ") ) ")


            If (Me.ChkGruppo1.Items.FindByValue(7).Selected = True) Or (Me.ChkGruppo1.Items.FindByValue(8).Selected = True) Then
                stbQ.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo = 1 ")
            End If
            'controllo se sono state selezionate le cooperative padre
            If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then
                stbQ.AppendLine(" ORDER BY coop_padre, rag_soc ASC ")
            ElseIf (Me.ChkGruppo1.Items.FindByValue(6).Selected = True) Then
                stbQ.AppendLine(" ORDER BY PIVA_padre, rag_soc ASC ")
            Else
                stbQ.AppendLine(" ORDER BY rag_soc ASC ")
            End If
            'fine coop padre
            stbQ.AppendLine(vbCrLf)

            'Recupero il datatable
            'Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
            '                      Session("ASG_Connessione_Server"),
            '                      StrSQL,
            '                      0,
            '                      Messaggio)

            '--------------------------------------------------------------------------
            DT = DataProvider.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'If (Not IsNothing(Rs)) AndAlso
            '    (Rs.State <> 0) AndAlso
            '        (Not Rs.EOF) Then

            '    DT = objSQL.CreateDT_from_RS(Rs, errore)

            If Not IsNothing(DT) AndAlso
                    DT.Rows.Count <> 0 Then


                'DT.Columns.Add("CF_legale")
                'DT.Columns.Add("rappr_legale")
                'DT.Columns.Add("com_legale")
                'DT.Columns.Add("pro_legale")

                ''basta clonarlo una sola volta
                'If i = 0 Then
                '    DT_Finale = DT.Clone
                'End If

                num_righe_dt = DT.Rows.Count

                ' la j scorre i record (particelle intersecate dall'impresa)
                For j = 0 To num_righe_dt - 1

                    If (CStr(DT.Rows(j).Item("rag_soc")) = "") Then
                        DT.Rows(j).Item("rag_soc") = " "
                    End If

                    If (CStr(DT.Rows(j).Item("tipo_impresa")) = "") Then
                        DT.Rows(j).Item("tipo_impresa") = -1
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(3).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("Codice_Socio")) = "") Then
                            DT.Rows(j).Item("Codice_Socio") = " "
                        End If
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(4).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("CUAA")) = "") Then
                            DT.Rows(j).Item("CUAA") = " "
                        End If
                    End If

                    'cooperativa/e padre
                    If (Me.ChkGruppo1.Items.FindByValue(5).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("coop_padre")) = "") Then
                            DT.Rows(j).Item("coop_padre") = " "
                        End If
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(6).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("PIVA_padre")) = "") Then
                            DT.Rows(j).Item("PIVA_padre") = " "
                        End If
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(7).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("ind_des")) = "") Then
                            DT.Rows(j).Item("ind_des") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("frz_des")) = "") Then
                            DT.Rows(j).Item("frz_des") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("CAP")) = "") Then
                            DT.Rows(j).Item("CAP") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("com_des")) = "") Then
                            DT.Rows(j).Item("com_des") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("pro_cod")) = "") Or CStr((DT.Rows(j).Item("pro_cod")) = "0") Then
                            DT.Rows(j).Item("pro_cod") = " "
                        End If
                    End If

                    If (Me.ChkGruppo1.Items.FindByValue(8).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("pro_cod_istat")) = "") Then
                            DT.Rows(j).Item("pro_cod_istat") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("com_cod_istat")) = "") Then
                            DT.Rows(j).Item("com_cod_istat") = " "
                        End If
                    End If

                    If (Me.ChkGruppo2.Items.FindByValue(10).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(11).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(12).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("legale")) = "") Then
                            DT.Rows(j).Item("legale") = " "
                        End If
                    End If

                    'controllo se sono state selezionate le particelle
                    If (Me.ChkGruppo2.Items.FindByValue(13).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("PROV")) = "") Then
                            DT.Rows(j).Item("PROV") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("COM")) = "") Then
                            DT.Rows(j).Item("COM") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("SEZIONE")) = "") Then
                            DT.Rows(j).Item("SEZIONE") = " "
                        End If

                        If (CStr(DT.Rows(j).Item("FOGLIO")) = "") Then
                            DT.Rows(j).Item("FOGLIO") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("NUMERO")) = "") Then
                            DT.Rows(j).Item("NUMERO") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("SUBALTERNO")) = "") Then
                            DT.Rows(j).Item("SUBALTERNO") = " "
                        End If
                    End If

                    If (Me.ChkGruppo2.Items.FindByValue(14).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("TitoloPossesso")) = "") Then
                            DT.Rows(j).Item("TitoloPossesso") = -1
                        End If
                    End If

                    If (Me.ChkGruppo2.Items.FindByValue(16).Selected = True) Then
                        If (CStr(DT.Rows(j).Item("ETTARI")) = "") Then
                            DT.Rows(j).Item("ETTARI") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("ARE")) = "") Then
                            DT.Rows(j).Item("ARE") = -1
                        End If

                        If (CStr(DT.Rows(j).Item("CENTIARE")) = "") Then
                            DT.Rows(j).Item("CENTIARE") = -1
                        End If

                    End If
                    'fine controllo particelle

                    'DT_Finale.ImportRow(DT.Rows.Item(j))

                Next

                'CANCELLA LE INFORMAZIONI NON NECESSARIE

                If Me.ChkGruppo1.Items.FindByValue(0).Selected = True Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("PIVA")
                End If

                If Me.ChkGruppo1.Items.FindByValue(1).Selected = True Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("rag_soc")
                End If

                If Me.ChkGruppo1.Items.FindByValue(2).Selected = True Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("tipo_impresa")
                End If

                If Me.ChkGruppo1.Items.FindByValue(9).Selected = True Then
                    flag_campo = True
                Else
                    DT.Columns.Remove("inizio_impresa")
                    DT.Columns.Remove("fine_impresa")
                End If


                'GRUPPO 2   

                If (Me.ChkGruppo2.Items.FindByValue(10).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(11).Selected = True) Or (Me.ChkGruppo2.Items.FindByValue(12).Selected = True) Then

                    For i = 0 To DT.Rows.Count - 1

                        If DT.Rows(i).Item("legale") <> "n.d." Then

                            array = Split(DT.Rows(i).Item("legale"), "|")

                            If Me.ChkGruppo2.Items.FindByValue(10).Selected = True Then
                                DT.Rows(i).Item("CF_legale") = array(0)
                            End If

                            If Me.ChkGruppo2.Items.FindByValue(11).Selected = True Then
                                DT.Rows(i).Item("rappr_legale") = array(1)
                            End If

                            If Me.ChkGruppo2.Items.FindByValue(12).Selected = True Then
                                DT.Rows(i).Item("com_legale") = array(2)
                                DT.Rows(i).Item("pro_legale") = array(3)
                            End If

                        Else
                            If Me.ChkGruppo2.Items.FindByValue(10).Selected = True Then
                                DT.Rows(i).Item("CF_legale") = " "
                            End If

                            If Me.ChkGruppo2.Items.FindByValue(11).Selected = True Then
                                DT.Rows(i).Item("rappr_legale") = " "
                            End If

                            If Me.ChkGruppo2.Items.FindByValue(12).Selected = True Then
                                DT.Rows(i).Item("com_legale") = " "
                                DT.Rows(i).Item("pro_legale") = " "
                            End If

                        End If

                    Next

                    DT.Columns.Remove("legale")

                End If

                If flag_campo = False Then
                    AgroMsg = "Selezionare almeno un campo di cui si vuole fare l'esportazione!!"
                    Return AgroMsg
                    Exit Function
                End If


                'Rs.Close() 'non serve

            Else 'nothing rs - state - eof

                'NON CI SONO IMPRESE!!!

                'Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni riguardanti tale impresa." & vbCrLf & vbCrLf
                Log += CStr(Date.Now) + "   La TERA QUERY IMPRESE non ha restituito alcun risultato per il seguente elenco di partite iva: " + ElencoChiaviImpresa & vbCrLf & vbCrLf

            End If 'rs

        Catch ex As Exception
            Log += CStr(Date.Now) + "   Errore nella TERA QUERY IMPRESE: " + ex.Message & vbCrLf & vbCrLf
        End Try


        'Next ' for num_elementi  imprese
        'ORA LA QUERY VIENE ESEGUITA SOLO UNA VOLTA, NON PIU' DENTRO AL FOR

        '------------------------------------------------
        '------------- FINE CICLO IMPRESE ---------------
        '------------------------------------------------

        Return AgroMsg


    End Function

    '############################################################################################################
    Private Function Esportazione_Agenda(ByRef XmlDoc As XmlDocument, ByRef DT As DataTable, ByRef Log As String, Optional ByVal chiavi As List(Of String) = Nothing) As String

        Dim NomeRoutine As String = "Esportatore_Universale_2.Esportazione_Agenda"

        ' nelle stampe 2003: 18 Luglio 2016 Esportatore_Universale_2 Agenda : introdotti campi regolamento, sup_trattata, qta totale.
        ' 29/01/2020 porting su stampe_2010

        Dim piva As String
        Dim sa_cod As Integer
        'Dim lav_cod As Integer
        Dim id_agenda As Integer

        Dim i As Integer ' la i scorre gli impianti e le imprese
        Dim j As Integer ' la j scorre le righe del DT

        Dim num_elementi As Integer

        'Dim DT_Finale As New DataTable
        'Dim Rs As ADODB.Recordset
        'Dim objSQL As New Codex_Utility.Sql
        'Dim objSQL As New Codex_Utility.Sql

        Dim msg As String
        Dim Messaggio As String = ""
        Dim AgroMsg As String = ""


        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

        If XML_FiltroStampa Is Nothing Then
            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")
        End If

        XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

        num_elementi = XMLs_VariabiliStampe.Count

        '------------------------------------------------
        '------------ INIZIO CICLO AGENDA -------------
        '------------------------------------------------       

        Dim ElencoChiaviAgenda, ChiaveAgenda As String

        Dim ListChiaviAgenda As New List(Of String)

        '-----------------------------------------------
        ' la i scorre le operazioni
        '-----------------------------------------------

        If chiavi Is Nothing Then
            For i = 0 To num_elementi - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                If (IsNothing(XML_VariabiliStampe.GetAttribute("piva"))) Then
                    msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("piva")) = "") Then
                    msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("sa_cod"))) Then
                    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("sa_cod")) = "") Then
                    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("id_agenda"))) Then
                    msg = "L'ID AGENDA E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("id_agenda")) = "") Then
                    msg = "L'ID AGENDA E' NULLO!!!" & vbCrLf
                End If
                'If (IsNothing(XML_VariabiliStampe.GetAttribute("lav_cod"))) Then
                '    msg = "IL LAV_COD E' NULLO!!!" & vbCrLf
                'ElseIf (CStr(XML_VariabiliStampe.GetAttribute("lav_cod")) = "") Then
                '    msg = "IL LAV_COD E' NULLO!!!" & vbCrLf
                'End If

                If msg <> "" Then
                    AgroMsg = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                    Return AgroMsg
                    Exit Function
                End If

                'ricavo le operazioni d'agenda
                piva = XML_VariabiliStampe.GetAttribute("piva")
                sa_cod = XML_VariabiliStampe.GetAttribute("sa_cod")
                id_agenda = XML_VariabiliStampe.GetAttribute("id_agenda")
                'lav_cod = XML_VariabiliStampe.GetAttribute("lav_cod")

                'Genero la chiave agenda
                ChiaveAgenda = piva & "_" & sa_cod & "_" & id_agenda '& "_" & lav_cod

                ElencoChiaviAgenda += ",'" & ChiaveAgenda & "'"

                'Controllo se mi arrivano dei duplicati
                If Not IsNothing(ListChiaviAgenda) AndAlso Not ListChiaviAgenda.Contains(ChiaveAgenda) Then

                    ListChiaviAgenda.Add(ChiaveAgenda)

                End If

            Next

            'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
            ElencoChiaviAgenda = Mid(ElencoChiaviAgenda, 2)
        Else
            ListChiaviAgenda = chiavi
        End If

        Dim controlValues As New ControlValues
        controlValues.Chiavi = ListChiaviAgenda
        controlValues.RdBList_Esporta_SelectedValue = Me.RdBList_Esporta.SelectedValue
        controlValues.ChkValue0 = Me.ChkGruppo1.Items.FindByValue(0).Selected
        controlValues.ChkValue1 = Me.ChkGruppo1.Items.FindByValue(1).Selected
        controlValues.ChkValue2 = Me.ChkGruppo1.Items.FindByValue(2).Selected
        controlValues.ChkValue3 = Me.ChkGruppo1.Items.FindByValue(3).Selected
        controlValues.ChkValue4 = Me.ChkGruppo1.Items.FindByValue(4).Selected
        controlValues.ChkValue5 = Me.ChkGruppo1.Items.FindByValue(5).Selected
        controlValues.ChkValue6 = Me.ChkGruppo1.Items.FindByValue(6).Selected
        controlValues.ChkValue7 = Me.ChkGruppo1.Items.FindByValue(7).Selected
        controlValues.ChkValue8 = Me.ChkGruppo1.Items.FindByValue(8).Selected
        controlValues.ChkValue9 = Me.ChkGruppo1.Items.FindByValue(9).Selected
        controlValues.ChkValue10 = Me.ChkGruppo2.Items.FindByValue(10).Selected
        controlValues.ChkValue11 = Me.ChkGruppo2.Items.FindByValue(11).Selected
        controlValues.ChkValue12 = Me.ChkGruppo2.Items.FindByValue(12).Selected
        controlValues.ChkValue13 = Me.ChkGruppo2.Items.FindByValue(13).Selected
        controlValues.ChkValue14 = Me.ChkGruppo2.Items.FindByValue(14).Selected
        controlValues.ChkValue15 = Me.ChkGruppo2.Items.FindByValue(15).Selected
        controlValues.ChkValue16 = Me.ChkGruppo2.Items.FindByValue(16).Selected
        controlValues.ChkValue17 = Me.ChkGruppo2.Items.FindByValue(17).Selected
        controlValues.ChkValue18 = Me.ChkGruppo2.Items.FindByValue(18).Selected
        controlValues.ChkValue19 = Me.ChkGruppo2.Items.FindByValue(19).Selected
        controlValues.ChkValue20 = Me.ChkGruppo3.Items.FindByValue(20).Selected
        controlValues.ChkValue21 = Me.ChkGruppo3.Items.FindByValue(21).Selected
        controlValues.ChkValue22 = Me.ChkGruppo3.Items.FindByValue(22).Selected
        controlValues.ChkValue23 = Me.ChkGruppo3.Items.FindByValue(23).Selected
        controlValues.ChkValue24 = Me.ChkGruppo3.Items.FindByValue(24).Selected
        controlValues.ChkValue25 = Me.ChkGruppo3.Items.FindByValue(25).Selected
        controlValues.ChkValue26 = Me.ChkGruppo3.Items.FindByValue(26).Selected
        controlValues.ChkValue27 = Me.ChkGruppo3.Items.FindByValue(27).Selected
        controlValues.ChkValue28 = Me.ChkGruppo3.Items.FindByValue(28).Selected
        controlValues.ChkValue29 = Me.ChkGruppo3.Items.FindByValue(29).Selected
        controlValues.ChkValue30 = Me.ChkGruppo3.Items.FindByValue(30).Selected
        controlValues.ChkValue31 = Me.ChkGruppo3.Items.FindByValue(31).Selected
        controlValues.ChkValue32 = Me.ChkGruppo4.Items.FindByValue(32).Selected
        controlValues.ChkValue33 = Me.ChkGruppo5.Items.FindByValue(33).Selected
        controlValues.ChkValue34 = Me.ChkGruppo5.Items.FindByValue(34).Selected

        Return Esportazione_Agenda(DT, controlValues, msg, Messaggio, AgroMsg, Log, False)

    End Function

    '############################################################################################################
    Private Function Esportazione_Rintraccio(ByRef XmlDoc As XmlDocument, ByRef DT As DataTable, ByRef Log As String,
                                                ByVal validita_inizio As String, ByVal PathFinaleRintraccio As String) As String

        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim piva As String
        Dim sa_cod, appezza, id_reg As Integer
        Dim lav_cod, id_agenda As Integer

        Dim i As Integer ' la i scorre gli impianti e le imprese
        Dim j As Integer ' la j scorre le righe del DT

        Dim num_elementi As Integer
        Dim num_righe_dt As Integer

        'Dim DT_Finale As New DataTable
        Dim Rs As ADODB.Recordset
        Dim objSQL As New Codex_Utility.Sql
        ' Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String

        Dim errore, msg As String
        Dim Messaggio As String = ""
        Dim AgroMsg As String = ""
        Dim flag_campo As Boolean


        Session("strXmlVariabilistampe") = Nothing

        'Me.ImgBtn_Annulla.Enabled = False

        XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

        XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

        num_elementi = XMLs_VariabiliStampe.Count

        '------------------------------------------------
        '----------- INIZIO CICLO RINTRACCIO ------------
        '------------------------------------------------  

        Dim num_record As Integer

        Dim AlmenoUno As Boolean = False

        Dim flag_semtrap As Boolean = False
        Dim flag_seme As Boolean = False
        Dim flag_prodotto As Boolean = False
        Dim flag_trattam As Boolean = False
        Dim flag_pioirr As Boolean = False
        Dim flag_opcolt As Boolean = False

        Dim XmlDoc_SemTrap As XmlDocument
        Dim XmlDoc_Trattam As XmlDocument
        Dim XmlDoc_PioIrr As XmlDocument
        Dim XmlDoc_OpColt As XmlDocument

        Dim XmlRoot_SemTrap As XmlElement
        Dim XmlRoot_Trattam As XmlElement
        Dim XmlRoot_PioIrr As XmlElement
        Dim XmlRoot_OpColt As XmlElement

        Dim StringoneXMLSemTrap As String
        Dim StringoneXMLTrattam As String
        Dim StringoneXMLPioIrr As String
        Dim StringoneXMLOpColt As String

        Dim memo_piva_semtrap As String = ""
        Dim memo_piva_trattam As String = ""
        Dim memo_piva_pioirr As String = ""
        Dim memo_piva_opcolt As String = ""

        '-----------------------------------------------
        ' la i scorre le imprese
        '-----------------------------------------------
        For i = 0 To num_elementi - 1

            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

            If (IsNothing(XML_VariabiliStampe.GetAttribute("piva"))) Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("piva")) = "") Then
                msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
            End If
            'If (IsNothing(XML_VariabiliStampe.GetAttribute("sa_cod"))) Then
            '    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
            'ElseIf (CStr(XML_VariabiliStampe.GetAttribute("sa_cod")) = "") Then
            '    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
            'End If
            'If (IsNothing(XML_VariabiliStampe.GetAttribute("id_agenda"))) Then
            '    msg = "L'ID AGENDA E' NULLO!!!" & vbCrLf
            'ElseIf (CStr(XML_VariabiliStampe.GetAttribute("id_agenda")) = "") Then
            '    msg = "L'ID AGENDA E' NULLO!!!" & vbCrLf
            'End If
            'If (IsNothing(XML_VariabiliStampe.GetAttribute("lav_cod"))) Then
            '    msg = "IL LAV_COD E' NULLO!!!" & vbCrLf
            'ElseIf (CStr(XML_VariabiliStampe.GetAttribute("lav_cod")) = "") Then
            '    msg = "IL LAV_COD E' NULLO!!!" & vbCrLf
            'End If

            If msg <> "" Then
                AgroMsg = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                Return AgroMsg
                Exit Function
            End If

            'ricavo le operazioni d'agenda
            piva = XML_VariabiliStampe.GetAttribute("piva")
            'sa_cod = XML_VariabiliStampe.GetAttribute("sa_cod")
            'id_agenda = XML_VariabiliStampe.GetAttribute("id_agenda")
            'lav_cod = XML_VariabiliStampe.GetAttribute("lav_cod")


            '@@@@@@@@@@ PRELEVARE LE OPERAZIONI DI AGENDA @@@@@@@@@@@@@@@@@

            'E' IMPORTANTE IL DISTINCT PERCHE' NON MI INTERESSANO GLI IMPIANTI!!!!
            'CI DEVE ESSERE UN RECORD PER OGNI OPERAZIONI DI AGENDA!

            'Questa query è sbagliata perchè tira su anche le operazioni di agenda fatte anche su impianti di altre specie vegetali
            '(un'impresa non coltiva una sola specie vegetale)

            'StrSQL = ""
            'StrSQL = "  SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, SpecieVegetali.Veg_Des, Agenda.Sa_Cod, Agenda.Id_Agenda, "
            'StrSQL += "                 Agenda.Lav_Cod, Operazioni.LAV_DES, GruppoOperazioni.GRU_COD, GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo "
            'StrSQL += " FROM            Reg_Impianti INNER JOIN "
            'StrSQL += "                 Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod INNER JOIN "
            'StrSQL += "                 Imprese ON Reg_Impianti.PIVA = Imprese.PIVA INNER JOIN "
            'StrSQL += "                 SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN "
            'StrSQL += "                 Agenda ON Imprese.PIVA = Agenda.PIVA INNER JOIN "
            'StrSQL += "                 Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD LEFT OUTER JOIN "
            'StrSQL += "                 GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD "
            'StrSQL += " WHERE           (Imprese.PIVA = '" & SQL_SaveText(piva) & "') "
            'StrSQL += " AND             (Reg_Impianti.Validita_Inizio >= " & SQL_SaveDate(validita_inizio) & " )"
            'StrSQL += " AND             (GruppoOperazioni.Tipo = 'C' OR GruppoOperazioni.Tipo IS NULL) "
            'If Me.Cmb_Punti.SelectedValue = 99 Then
            '    StrSQL += " AND         (Cultivar.Veg_Cod = 23 OR Cultivar.Veg_Cod = 51) "
            'Else
            '    If Me.Cmb_Punti.SelectedValue = 20099 Then
            '        StrSQL += " AND         (Cultivar.Veg_Cod = 52) "
            '    End If
            'End If
            'StrSQL += " ORDER BY        GruppoOperazioni.GRU_COD, Agenda.Lav_Cod "


            'ci vuole il distinct se no la stessa operazione compare tante volte quanti sono gli impianti su cui è effettuata
            StrSQL = ""
            StrSQL = " SELECT  DISTINCT     CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Impresa.PIVA ELSE Imprese.partitaIvaReale End PIVA, "
            StrSQL += "                     Imprese.rag_soc, SpecieVegetali.Veg_Des, Agenda.Sa_Cod, Agenda.Id_Agenda,  "
            StrSQL += "                     Agenda.Lav_Cod, Operazioni.LAV_DES, GruppoOperazioni.GRU_COD, GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo "
            StrSQL += " FROM                Reg_Impianti INNER JOIN "
            StrSQL += "                     Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod INNER JOIN "
            StrSQL += "                     SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN "
            StrSQL += "                     Mov_Destinazioni ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND  "
            StrSQL += "                     Reg_Impianti.Appezza = Mov_Destinazioni.Appezza AND Reg_Impianti.Id_Reg = Mov_Destinazioni.Id_Destinazione INNER JOIN "
            StrSQL += "                     Agenda INNER JOIN "
            StrSQL += "                     Imprese ON Agenda.PIVA = Imprese.PIVA INNER JOIN "
            StrSQL += "                     Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD INNER JOIN "
            StrSQL += "                     Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN "
            StrSQL += "                     Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  "
            StrSQL += "                     Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ON "
            StrSQL += "                     Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND "
            StrSQL += "                     Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND "
            StrSQL += "                     Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det LEFT OUTER JOIN "
            StrSQL += "                     GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD "
            StrSQL += " WHERE               (Imprese.PIVA = " & Agro_SQL_SaveText(piva) & ") "
            StrSQL += " AND                 (GruppoOperazioni.Tipo = 'C' OR GruppoOperazioni.Tipo IS NULL) "

            'e se l'impianto è di quella specie vegetale ed è maggiore o uguale a quella validità inizio
            StrSQL += " AND     ( "
            StrSQL += "             (       (Reg_Impianti.Validita_Inizio >= " & Agro_SQL_SaveDate(validita_inizio) & " )"
            If Me.Cmb_Punti.SelectedValue = 99 Then
                StrSQL += "         AND     (Cultivar.Veg_Cod = 23 OR Cultivar.Veg_Cod = 51)    )"
            Else
                If Me.Cmb_Punti.SelectedValue = 20099 Then
                    StrSQL += "     AND     (Cultivar.Veg_Cod = 52)                             )"
                End If
            End If
            ' o se si tratta del rilievo piogge (operazione sul centro aziendale)
            StrSQL += "         OR     (    (Agenda.Lav_Cod= 126)         "
            StrSQL += "                     AND  (Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(validita_inizio) & " )     )       )"

            StrSQL += " ORDER BY GruppoOperazioni.GRU_COD, Agenda.Lav_Cod "


            Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                                    Session("ASG_Connessione_Server"),
                                    StrSQL,
                                    0,
                                    Messaggio)

            If IsNothing(Messaggio) Then

                If (Not IsNothing(Rs)) AndAlso
                    (Rs.State <> 0) AndAlso
                        (Not Rs.EOF) Then

                    num_record = Rs.RecordCount

                    AlmenoUno = True

                    For j = 0 To num_record - 1

                        lav_cod = CInt(Rs.Fields("lav_cod").Value)
                        id_agenda = CInt(Rs.Fields("id_agenda").Value)
                        sa_cod = CInt(Rs.Fields("sa_cod").Value)

                        Select Case lav_cod


                            '############################################################
                            '------------- SEMINA ---------------------------------------
                            Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO
                                Try

                                    ' 2 = semina
                                    ' 71 = trapianto

                                    If memo_piva_semtrap = "" Then

                                        'primo giro
                                        memo_piva_semtrap = piva

                                        'SEMINA/TRAPIANTO
                                        Call Rintraccio_Semina_Trapianto(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_semtrap, XmlRoot_SemTrap, XmlDoc_SemTrap)

                                    Else

                                        If piva = memo_piva_semtrap Then

                                            'SEMINA/TRAPIANTO
                                            Call Rintraccio_Semina_Trapianto(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_semtrap, XmlRoot_SemTrap, XmlDoc_SemTrap)

                                        Else

                                            'è cambiato il socio ---> devo scrivere il file XML precedente

                                            'DEVO APPENDERE LA RADICE AL DOCUMENTO
                                            XmlDoc_SemTrap.AppendChild(XmlRoot_SemTrap)

                                            StringoneXMLSemTrap = XmlDoc_SemTrap.OuterXml

                                            'devo salvare il socio precedente, quindi passo la memo_piva!
                                            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLSemTrap, memo_piva_semtrap,
                                                                    "Semina", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_SemTrap)

                                            'il nuovo socio ora lo metto in memo_piva
                                            memo_piva_semtrap = piva

                                            'azzero tutto
                                            flag_semtrap = False
                                            StringoneXMLSemTrap = ""
                                            XmlRoot_SemTrap = Nothing
                                            XmlDoc_SemTrap = Nothing


                                            'SEMINA/TRAPIANTO
                                            Call Rintraccio_Semina_Trapianto(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_semtrap, XmlRoot_SemTrap, XmlDoc_SemTrap)

                                        End If

                                    End If

                                Catch ex As Exception

                                    Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                                    " -   Errore durante l'esportazione Semina Trapianto: " + ex.Message & vbCrLf & vbCrLf

                                End Try

                                '#######################################################################

                                ' ANALISI SEME ???

                                ' ANALISI PRODOTTO ?????

                                '#######################################################################

                                '----------------------- TRATTAMENTI --------------------------
                            Case LAVCOD_DISERBO, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_GEODISINFESTAZIONE

                                Try

                                    ' 18 = Diserbo
                                    ' 74 = Trattamenti Antiparassitari
                                    ' 103 = Trattamento Fitoregolatore o Cosmetico
                                    ' 155 = Geodisinfestazione

                                    'TRATTAMENTI
                                    If memo_piva_trattam = "" Then

                                        'primo giro
                                        memo_piva_trattam = piva

                                        Call Rintraccio_Trattamenti(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_trattam, XmlRoot_Trattam, XmlDoc_Trattam)

                                    Else

                                        If piva = memo_piva_trattam Then

                                            Call Rintraccio_Trattamenti(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_trattam, XmlRoot_Trattam, XmlDoc_Trattam)

                                        Else

                                            'è cambiato il socio ---> devo scrivere il file XML precedente

                                            'DEVO APPENDERE LA RADICE AL DOCUMENTO
                                            XmlDoc_Trattam.AppendChild(XmlRoot_Trattam)

                                            StringoneXMLTrattam = XmlDoc_Trattam.OuterXml

                                            'devo salvare il socio precedente, quindi passo la memo_piva!
                                            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLTrattam, memo_piva_trattam,
                                                                    "Trattamenti", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_Trattam)

                                            'il nuovo socio ora lo metto in memo_piva
                                            memo_piva_trattam = piva

                                            'azzero tutto
                                            flag_trattam = False
                                            StringoneXMLTrattam = ""
                                            XmlRoot_Trattam = Nothing
                                            XmlDoc_Trattam = Nothing

                                            Call Rintraccio_Trattamenti(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_trattam, XmlRoot_Trattam, XmlDoc_Trattam)

                                        End If

                                    End If

                                Catch ex As Exception

                                    Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                                    " -   Errore durante l'esportazione Trattamenti: " + ex.Message & vbCrLf & vbCrLf

                                End Try

                                '########################################################################
                                '-------------------- 'PIOGGE/IRRIGAZIONI
                            Case LAVCOD_IRRIGAZIONE, LAVCOD_RILIEVO_PIOGGE

                                Try

                                    ' 1 = Irrigazione
                                    ' 126 = Rilievo Piogge Giornaliere

                                    If memo_piva_pioirr = "" Then

                                        'primo giro
                                        memo_piva_pioirr = piva

                                        Call Rintraccio_Piogge_Irrigazioni(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_pioirr, XmlRoot_PioIrr, XmlDoc_PioIrr)

                                    Else

                                        If piva = memo_piva_pioirr Then

                                            Call Rintraccio_Piogge_Irrigazioni(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_pioirr, XmlRoot_PioIrr, XmlDoc_PioIrr)

                                        Else

                                            'è cambiato il socio ---> devo scrivere il file XML precedente

                                            'DEVO APPENDERE LA RADICE AL DOCUMENTO
                                            XmlDoc_PioIrr.AppendChild(XmlRoot_PioIrr)

                                            StringoneXMLPioIrr = XmlDoc_PioIrr.OuterXml

                                            'devo salvare il socio precedente, quindi passo la memo_piva!
                                            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLPioIrr, memo_piva_pioirr,
                                                                    "Piogge_Irrigazioni", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_PioIrr)

                                            'il nuovo socio ora lo metto in memo_piva
                                            memo_piva_pioirr = piva

                                            'azzero tutto
                                            flag_pioirr = False
                                            StringoneXMLPioIrr = ""
                                            XmlRoot_PioIrr = Nothing
                                            XmlDoc_PioIrr = Nothing


                                            Call Rintraccio_Piogge_Irrigazioni(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_pioirr, XmlRoot_PioIrr, XmlDoc_PioIrr)

                                        End If

                                    End If

                                Catch ex As Exception

                                    Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                                    " -   Errore durante l'esportazione Piogge Irrigazioni: " + ex.Message & vbCrLf & vbCrLf

                                End Try


                                '#######################################################################
                            Case Else

                                Try

                                    'ALTRE OPERAZIONI COLTURALI

                                    If memo_piva_opcolt = "" Then

                                        'primo giro
                                        memo_piva_opcolt = piva

                                        Call Rintraccio_Operazioni_Colturali(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_opcolt, XmlRoot_OpColt, XmlDoc_OpColt)

                                    Else

                                        If piva = memo_piva_opcolt Then

                                            Call Rintraccio_Operazioni_Colturali(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_opcolt, XmlRoot_OpColt, XmlDoc_OpColt)

                                        Else

                                            'è cambiato il socio ---> devo scrivere il file XML precedente

                                            'DEVO APPENDERE LA RADICE AL DOCUMENTO
                                            XmlDoc_OpColt.AppendChild(XmlRoot_OpColt)

                                            StringoneXMLOpColt = XmlDoc_OpColt.OuterXml

                                            'devo salvare il socio precedente, quindi passo la memo_piva!
                                            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLOpColt, memo_piva_opcolt,
                                                                    "Operazioni_Colturali", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_OpColt)

                                            'il nuovo socio ora lo metto in memo_piva
                                            memo_piva_opcolt = piva

                                            'azzero tutto
                                            flag_opcolt = False
                                            StringoneXMLOpColt = ""
                                            XmlRoot_OpColt = Nothing
                                            XmlDoc_OpColt = Nothing


                                            Call Rintraccio_Operazioni_Colturali(Log, Me.Cmb_Punti.SelectedValue, piva, sa_cod, id_agenda, lav_cod, flag_opcolt, XmlRoot_OpColt, XmlDoc_OpColt)

                                        End If

                                    End If

                                Catch ex As Exception

                                    Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                                    " -   Errore durante l'esportazione Operazioni Colturali: " + ex.Message & vbCrLf & vbCrLf

                                End Try


                        End Select


                        'vado all'altro record della stessa impresa
                        Rs.MoveNext()


                    Next

                    'Chiudo il recordset
                    Rs.Close()

                Else 'nothing rs - state - eof

                    'Tolgo questo logging se no vengono scritte tutte le imprese che non coltivano la specie vegetale selezionata.
                    'If Me.Cmb_Punti.SelectedValue = 99 Then
                    '    Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovati movimenti d'agenda per impianti con validità inizio >= " & CStr(validita_inizio) & " (ARP Pisello/Fagiolo)." & vbCrLf & vbCrLf
                    'Else
                    '    If Me.Cmb_Punti.SelectedValue = 20099 Then
                    '        Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovati movimenti d'agenda per impianti con validità inizio >= " & CStr(validita_inizio) & " (ARP Pomodoro)." & vbCrLf & vbCrLf
                    '    End If
                    'End If

                End If 'nothing

            Else 'messaggio

                Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Errore nella QUERY RINTRACCIO Imprese: " + Messaggio & vbCrLf & vbCrLf

            End If 'messaggio

            'Elimino il recordset
            Rs = Nothing


        Next 'x ogni piva   --- impresa

        'Elimino l'oggetto
        objSQL = Nothing


        'devo scrivere il file xml dell'ultimo socio per ogni tipo di documento!!!!!!!!!
        'tolgo il select case se no scriverebbe solo il documento riferito all'ultimo lav_cod

        'Select Case lav_cod

        'NOTA! X OGNI DOCUMENTO DEVO PASSARE LA SUA MEMO_PIVA PERCHE' NON E' DETTO 
        'CHE L'ULTIMA PIVA ANALIZZATA ABBIA IMPIANTI DELLA SPECIE VEGETALE SELEZIONATA


        '---------------------- SEMINA ---------------------------------------
        'Case 2, 71

        If Not IsNothing(XmlDoc_SemTrap) And Not IsNothing(XmlRoot_SemTrap) Then

            'DEVO APPENDERE LA RADICE AL DOCUMENTO
            XmlDoc_SemTrap.AppendChild(XmlRoot_SemTrap)

            StringoneXMLSemTrap = XmlDoc_SemTrap.OuterXml

            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLSemTrap, memo_piva_semtrap,
                                    "Semina", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_SemTrap)

        End If



        '----------------------- TRATTAMENTI --------------------------
        'Case 18, 74, 103, 155

        If Not IsNothing(XmlDoc_Trattam) And Not IsNothing(XmlRoot_Trattam) Then

            'DEVO APPENDERE LA RADICE AL DOCUMENTO
            XmlDoc_Trattam.AppendChild(XmlRoot_Trattam)

            StringoneXMLTrattam = XmlDoc_Trattam.OuterXml

            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLTrattam, memo_piva_trattam,
                                    "Trattamenti", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_Trattam)

        End If


        '------------------ PIOGGE/IRRIGAZIONI -----------------------
        'Case 1, 126
        ' 1 = Irrigazione
        ' 126 = Rilievo Piogge Giornaliere

        If Not IsNothing(XmlDoc_PioIrr) And Not IsNothing(XmlRoot_PioIrr) Then

            'DEVO APPENDERE LA RADICE AL DOCUMENTO
            XmlDoc_PioIrr.AppendChild(XmlRoot_PioIrr)

            StringoneXMLPioIrr = XmlDoc_PioIrr.OuterXml

            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLPioIrr, memo_piva_pioirr,
                                    "Piogge_Irrigazioni", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_PioIrr)

        End If



        '------------------ ALTRE OPERAZIONI COLTURALI --------------------
        'Case Else

        If Not IsNothing(XmlDoc_OpColt) And Not IsNothing(XmlRoot_OpColt) Then

            'DEVO APPENDERE LA RADICE AL DOCUMENTO
            XmlDoc_OpColt.AppendChild(XmlRoot_OpColt)

            StringoneXMLOpColt = XmlDoc_OpColt.OuterXml

            Esporta_XML_Rintraccio(Log, PathFinaleRintraccio, StringoneXMLOpColt, memo_piva_opcolt,
                                    "Operazioni_Colturali", CInt(Me.Cmb_Punti.SelectedValue), XmlDoc_OpColt)

        End If


        'End Select

        If AlmenoUno = False Then

            If Me.Cmb_Punti.SelectedValue = 99 Then
                Log += CStr(Date.Now) + "   Non sono state trovate imprese con impianti di Pisello o Fagiolo con validità inizio >= " & CStr(validita_inizio) & "." & vbCrLf & vbCrLf
            Else
                If Me.Cmb_Punti.SelectedValue = 20099 Then
                    Log += CStr(Date.Now) + "   Non sono state trovate imprese con impianti di Pomodoro con validità inizio >= " & CStr(validita_inizio) & "." & vbCrLf & vbCrLf
                End If
            End If

        End If


        Return AgroMsg

    End Function


    ''-----------> DA ATTIVARE QUANDO SERVE!
    ''############################################################################################################
    Private Sub RdBList_Esporta_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RdBList_Esporta.SelectedIndexChanged

        Select Case Me.RdBList_Esporta.SelectedValue

            Case "B"
                Row_XML.Visible = True
                ChkGruppo1.Items(0).Enabled = False
                ChkGruppo1.Items(0).Selected = True
            Case Else
                Row_XML.Visible = False
                ChkGruppo1.Items(0).Enabled = True
        End Select

        'Select Case Me.RdBList_Esporta.SelectedValue

        '    Case "A", "C"
        '        Me.Txt_FileOutput.Enabled = False
        '        Me.Txt_FileOutput.BackColor = Color.Gray

        '    Case Else
        '        Me.Txt_FileOutput.Enabled = True
        '        Me.Txt_FileOutput.BackColor = Color.White

        'End Select

    End Sub


    '#############################################################################################################
    Private Sub Rintraccio_Semina_Trapianto(ByRef Log As String, ByVal Punto As Integer, ByVal piva As String, ByVal sa_cod As Integer, ByVal id_agenda As Integer, ByVal lav_cod As Integer, ByRef flag_semtrap As Boolean, ByRef XmlRoot_send As XmlElement, ByRef XmlDocumento_send As XmlDocument)

        Dim XmlDocumento As XmlDocument
        Dim XmlRoot, XmlPadre As XmlElement
        Dim Node, NodeRiga As XmlElement 'As XmlNode
        Dim xPI As XmlProcessingInstruction

        Dim num_record As Integer

        Dim Rs, RsParticelle, RsBolle As ADODB.Recordset
        Dim objSQL As New Codex_Utility.Sql
        ' Dim objSQL As New Codex_Utility.Sql

        Dim Messaggio, StrSQL, anno, data_trapianto As String
        Dim mat_cod, elem_cod, pro_cod, udm_cod As Integer

        '------------------------------------------------

        If flag_semtrap = False Then

            XmlDocumento = New XmlDocument

            XmlRoot = XmlDocumento.CreateElement("dataroot")

            'xPI = XmlDocumento.CreateProcessingInstruction("xml", "version=""1.0"" encoding=""UTF-8""")
            'XmlDocumento.AppendChild(xPI)

            ' Create an XML declaration. 
            Dim xmldecl As XmlDeclaration
            xmldecl = XmlDocumento.CreateXmlDeclaration("1.0", Nothing, Nothing)
            xmldecl.Encoding = "UTF-8"
            'xmldecl.Standalone = "yes"
            XmlDocumento.AppendChild(xmldecl)


        Else

            XmlRoot = XmlRoot_send
            XmlDocumento = XmlDocumento_send

        End If


        '-------------------------------------------------------
        ' VAI CON LA QUERY RINTRACCIO Semina/Trapianto!!!!!!!!
        '-------------------------------------------------------

        StrSQL = " "
        StrSQL += " SELECT  Agenda.PIVA, ISNULL(Imprese.rag_soc, ' ') AS rag_soc, Agenda.Sa_Cod, ISNULL(Centri_Aziendali.sa_nome, ' ') AS sa_nome,  "
        StrSQL += "         ISNULL(Appezzamento.Campo_Cod, - 1) AS campo_cod, ISNULL(Campi.Campo_Des, ' ') AS campo_des, "
        StrSQL += "         ISNULL(Mov_Destinazioni.Appezza, - 1) AS appezza, ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome, ISNULL(Mov_Destinazioni.Id_Destinazione, - 1) AS id_dest,  "
        StrSQL += "         ISNULL(Reg_Impianti.Sup_Imp, 0.0000) AS sup_imp, ISNULL(GruppoVegetale.Gru_Cod, - 1) AS gru_cod, "
        StrSQL += "         ISNULL(GruppoVegetale.Gru_Des, ' ') AS gru_des, ISNULL(Cultivar.Veg_Cod, - 1) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, ' ') AS veg_des, "
        StrSQL += "         ISNULL(Reg_Impianti.CUL_COD, - 1) AS cul_cod, ISNULL(Cultivar.Cul_Des, ' ') AS cul_des, ISNULL(GruppoVarietale.Grva_Des, ' ') AS grva_des,"
        StrSQL += "         ISNULL(GruppoFinalita.Grfi_Des, ' ') AS grfi_des, ISNULL(Reg_Impianti.Resa_Prevista, 0) AS resa_prevista, ISNULL(Reg_Impianti.Resa_Effettiva, 0) AS resa_effettiva, "
        StrSQL += "         ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Inizio, 103), ' ') AS inizio_impianto, "
        StrSQL += "         ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), ' ') AS fine_impianto, ISNULL(Movimenti.Cau_Mov, ' ') AS cau_mov, "
        StrSQL += "         Agenda.Id_Agenda, Agenda.Lav_Cod, ISNULL(Agenda.des_lib, ' ') AS des_lib, ISNULL(GruppoOperazioni.GRU_DES, ' ') AS gruppo_operazione, "
        StrSQL += "         ISNULL(CONVERT(varchar(10), Agenda.Validita_Inizio, 103), ' ') AS validita_inizio_agenda, ISNULL(CONVERT(varchar(10), Movimenti.Data_Movimento, 103), ' ') AS data_movimento,  "
        StrSQL += "         ISNULL(Movimenti_dettagli.Udm_Cod, - 1) AS udm_cod, ISNULL(UnitaMisura.UDM_DES, ' ') AS udm_des, ISNULL(Movimenti_dettagli.Extra_Int, - 1 ) AS extra_int, ISNULL(Movimenti_dettagli.Qta, 0) AS qta_tot,  "
        StrSQL += "         ISNULL(Mov_Destinazioni.Qta, 0) AS qta_impianto, ISNULL(Movimenti_dettagli.Elem_Cod, -1) AS elem_cod, ISNULL(Movimenti_dettagli.Pro_Cod, -1) AS pro_cod, ISNULL(Movimenti_dettagli.Mat_Cod, -1) AS mat_cod,  "
        StrSQL += "         ISNULL(Movimenti.Mov_Desc, ' ') AS Mov_Desc, ISNULL(Indirizzi.ind_des, ' ') AS ind_des, ISNULL(Indirizzi.frz_des, ' ') AS frz_des,  "
        StrSQL += "         ISNULL(ISTAT.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS provincia, ISNULL(Indirizzi.stato, ' ') AS stato, "
        'StrSQL += "         ISNULL(Imprese_Progetti.Progetto_Cod, 0) AS progetto_cod, ISNULL(Imprese_Progetti.Progetto_Nome, ' ') AS lotto "
        StrSQL += "         ISNULL((SELECT     TOP 1 Progetto_Nome "
        StrSQL += "         FROM    Imprese_Progetti"
        StrSQL += "         WHERE   Imprese_Progetti.Piva = Reg_Impianti.PIVA "
        StrSQL += "         AND     Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD "
        StrSQL += "         AND     Imprese_Progetti.Appezza = Reg_Impianti.Appezza "
        StrSQL += "         AND     Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg), ' ') AS lotto "
        'FROM
        StrSQL += " FROM    Indirizzi "
        StrSQL += "         INNER JOIN CentrixIndirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo "
        StrSQL += "         RIGHT OUTER JOIN    Centri_Aziendali INNER JOIN    Appezzamento INNER JOIN Movimenti_dettagli INNER JOIN"
        StrSQL += "         Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND "
        StrSQL += "         Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN "
        StrSQL += "         Mov_Destinazioni ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND "
        StrSQL += "         Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND  "
        StrSQL += "         Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ON Appezzamento.Appezza = Mov_Destinazioni.Appezza INNER JOIN "
        StrSQL += "         Imprese INNER JOIN Agenda ON Imprese.PIVA = Agenda.PIVA "
        StrSQL += "         INNER JOIN Reg_Impianti ON Agenda.PIVA = Reg_Impianti.PIVA AND Agenda.Sa_Cod = Reg_Impianti.SA_COD ON  "
        StrSQL += "         Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza "
        'StrSQL += "         INNER JOIN Imprese_Progetti ON Imprese_Progetti.Piva = Reg_Impianti.PIVA AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD   "
        'StrSQL += "         AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg "
        StrSQL += "         AND Appezzamento.PIVA = Agenda.PIVA AND Appezzamento.SA_COD = Agenda.Sa_Cod AND Movimenti.PIVA = Agenda.PIVA AND "
        StrSQL += "         Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda ON Centri_Aziendali.sa_cod = Agenda.Sa_Cod AND "
        StrSQL += "         Centri_Aziendali.PIVA = Agenda.PIVA INNER JOIN "
        StrSQL += "         UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND  "
        StrSQL += "         CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod LEFT OUTER JOIN "
        StrSQL += "         GruppoOperazioni LEFT OUTER JOIN "
        StrSQL += "         Operazioni ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP ON Operazioni.LAV_COD = Agenda.Lav_Cod LEFT OUTER JOIN "
        StrSQL += "         Campi ON Appezzamento.Campo_Cod = Campi.Campo_Cod AND Appezzamento.PIVA = Campi.Piva AND  "
        StrSQL += "         Appezzamento.SA_COD = Campi.Sa_Cod LEFT OUTER JOIN "
        StrSQL += "         GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod LEFT OUTER JOIN "
        StrSQL += "         GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN "
        StrSQL += "         Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod LEFT OUTER JOIN "
        StrSQL += "         SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod LEFT OUTER JOIN "
        StrSQL += "         GruppoVegetale ON SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod "
        StrSQL += "         LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM "
        'WHERE
        StrSQL += " WHERE   Agenda.PIVA = " & Agro_SQL_SaveText(piva) & " "
        StrSQL += " AND     Agenda.SA_COD = " & Agro_SQL_SaveNum(sa_cod) & " "
        StrSQL += " AND     Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & " "
        StrSQL += " AND     Agenda.lav_cod = " & Agro_SQL_SaveNum(lav_cod) & " "
        StrSQL += vbCrLf

        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              Session("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)


        If IsNothing(Messaggio) Then

            If (Not IsNothing(Rs)) AndAlso
                (Rs.State <> 0) AndAlso
                    (Not Rs.EOF) Then

                num_record = Rs.RecordCount

                '''If num_record > 1 Then
                '''    Log += CStr(Date.Now) + "   L'operazione Semina/Trapianto con id_agenda: " + CStr(id_agenda) + " e lav_cod: " + CStr(lav_cod) + " è stata eseguita su  num. appezzamenti = " + CStr(num_record) + ". Centro: " + piva + " - " + CStr(sa_cod) + ". " & vbCrLf & vbCrLf
                '''End If

                While Not Rs.EOF

                    Try

                        'CREO IL P99M100000
                        XmlPadre = XmlDocumento.CreateElement("P" + CStr(Punto) + "M100000")

                        '---------------


                        'descrizione
                        Node = XmlDocumento.CreateElement("Descrizione")

                        If CStr(Rs.Fields("data_movimento").Value) <> " " Then
                            anno = Left(CStr(Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")), 4)
                        End If

                        If lav_cod = 2 Then
                            Node.InnerText = "Semina " + anno
                        Else
                            If lav_cod = 71 Then
                                Node.InnerText = "Trapianto " + anno
                            End If
                        End If

                        XmlPadre.AppendChild(Node)

                        '---------------

                        'data riferimento
                        Node = XmlDocumento.CreateElement("DataRiferimento")

                        If CStr(Rs.Fields("data_movimento").Value) <> " " Then
                            Node.InnerText = Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")
                        End If
                        XmlPadre.AppendChild(Node)

                        '----------

                        'partita iva
                        Node = XmlDocumento.CreateElement("PM100000")
                        Node.InnerText = piva
                        XmlPadre.AppendChild(Node)

                        '-------

                        'ragione sociale
                        Node = XmlDocumento.CreateElement("PM100001")
                        If CStr(Rs.Fields("rag_soc").Value) <> " " Then
                            Node.InnerText = CStr(Rs.Fields("rag_soc").Value)
                        Else
                            Node.InnerText = "n.d."
                        End If
                        XmlPadre.AppendChild(Node)

                        '--------

                        'codice appezzamento
                        Dim CodiceCampoAppezza As String = ""
                        Dim CodiceVarieta As String = ""

                        If CInt(Rs.Fields("cul_cod").Value) <> -1 Then
                            CodiceVarieta = CStr(Rs.Fields("cul_cod").Value)
                        Else
                            CodiceVarieta = CStr("n.d.")
                        End If

                        Node = XmlDocumento.CreateElement("PM100002")
                        If CStr(Rs.Fields("campo_des").Value) <> " " Then

                            CodiceCampoAppezza = CStr(CodiceCampoAppezza_from_Nome(CStr(Rs.Fields("campo_des").Value)))
                            Node.InnerText = CStr(CInt(CodiceCampoAppezza)) + "__" + CodiceVarieta

                        Else
                            If CStr(Rs.Fields("app_nome").Value) <> " " Then

                                CodiceCampoAppezza = CStr(CodiceCampoAppezza_from_Nome(CStr(Rs.Fields("app_nome").Value)))
                                Node.InnerText = CStr(CInt(CodiceCampoAppezza)) + "__" + CodiceVarieta

                            Else
                                Node.InnerText = "n.d.__n.d."
                            End If
                        End If
                        XmlPadre.AppendChild(Node)

                        ''codice appezzamento
                        'Node = XmlDocumento.CreateElement("PM100002")
                        'If CStr(Rs.Fields("app_nome").Value) <> " " Then
                        '    Node.InnerText = CodiceCampoAppezza_from_Nome(CStr(Rs.Fields("app_nome").Value))
                        'Else
                        '    Node.InnerText = "n.d."
                        'End If
                        'XmlPadre.AppendChild(Node)

                        '--------

                        'descr appezzamento
                        Node = XmlDocumento.CreateElement("PM100003")
                        If CStr(Rs.Fields("campo_des").Value) <> " " Then
                            Node.InnerText = CStr(Rs.Fields("campo_des").Value)
                        Else
                            If CStr(Rs.Fields("app_nome").Value) <> " " Then
                                Node.InnerText = CStr(Rs.Fields("app_nome").Value)
                            Else
                                Node.InnerText = "n.d."
                            End If
                        End If
                        XmlPadre.AppendChild(Node)

                        ''descr appezzamento
                        'Node = XmlDocumento.CreateElement("PM100003")
                        'If CStr(Rs.Fields("app_nome").Value) <> " " Then
                        '    Node.InnerText = CStr(Rs.Fields("app_nome").Value)
                        'Else
                        '    Node.InnerText = "n.d."
                        'End If
                        'XmlPadre.AppendChild(Node)

                        '-------

                        'indirizzo appezzamento
                        If CStr(Rs.Fields("ind_des").Value) <> " " Then
                            Node = XmlDocumento.CreateElement("PM100004")
                            Node.InnerText = CStr(Rs.Fields("ind_des").Value) + " - " + CStr(Rs.Fields("cap").Value) + " " + CStr(Rs.Fields("com_des").Value) + " (" + CStr(Rs.Fields("provincia").Value) + ")" + " - " + CStr(Rs.Fields("stato").Value)
                            XmlPadre.AppendChild(Node)
                        End If

                        '------------

                        'sup appezzamento
                        If CDbl(Rs.Fields("sup_imp").Value) <> 0 Then
                            Node = XmlDocumento.CreateElement("PM100005")
                            Node.InnerText = CDbl(Rs.Fields("sup_imp").Value)
                            XmlPadre.AppendChild(Node)
                        End If

                        '------------

                        'data operazione
                        Node = XmlDocumento.CreateElement("PM100006")
                        If CStr(Rs.Fields("data_movimento").Value) <> " " Then
                            Node.InnerText = Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")
                            data_trapianto = Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")
                        Else
                            Node.InnerText = Format(CDate(Date.Today), "yyyy-MM-dd")
                        End If
                        XmlPadre.AppendChild(Node)

                        '------------

                        'annata operazione
                        Node = XmlDocumento.CreateElement("PM100007")
                        If CStr(Rs.Fields("data_movimento").Value) <> " " Then
                            Node.InnerText = CInt(anno)
                        Else
                            Node.InnerText = CInt(Left(CStr(Format(CDate(Date.Today), "yyyy-MM-dd")), 4))
                        End If
                        XmlPadre.AppendChild(Node)

                        '==============================================================

                        ' /***********PARTICELLE******************/

                        StrSQL = ""
                        StrSQL = "  SELECT  AppezzamentiXParticelle.PIVA, AppezzamentiXParticelle.SA_COD, AppezzamentiXParticelle.Appezza, AppezzamentiXParticelle.PROV, "
                        StrSQL += "         AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO,  "
                        StrSQL += "         AppezzamentiXParticelle.SUBALTERNO, AppezzamentiXParticelle.AREA, ISTAT.LOCALITA, ISTAT.COMUNI_PROV "
                        StrSQL += " FROM    AppezzamentiXParticelle INNER JOIN "
                        StrSQL += "         ISTAT ON AppezzamentiXParticelle.PROV = ISTAT.PROV AND AppezzamentiXParticelle.COM = ISTAT.COM "
                        StrSQL += " WHERE   piva = " + Agro_SQL_SaveText(piva) + " "
                        StrSQL += " AND     sa_cod = " + Agro_SQL_SaveNum(sa_cod) + " "
                        StrSQL += " AND     appezza = " + Agro_SQL_SaveNum(CInt(Rs.Fields("appezza").Value)) + " "
                        StrSQL += " ORDER BY AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO, AppezzamentiXParticelle.SUBALTERNO "

                        RsParticelle = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                                                        Session("ASG_Connessione_Server"),
                                                        StrSQL,
                                                        0,
                                                        Messaggio)

                        If IsNothing(Messaggio) Then

                            If (Not IsNothing(RsParticelle)) AndAlso
                                (RsParticelle.State <> 0) AndAlso
                                    (Not RsParticelle.EOF) Then


                                Do While Not RsParticelle.EOF

                                    NodeRiga = XmlDocumento.CreateElement("Riga")

                                    Node = XmlDocumento.CreateElement("PM100008")
                                    Node.InnerText = CStr(RsParticelle.Fields("foglio").Value)
                                    NodeRiga.AppendChild(Node)

                                    Node = XmlDocumento.CreateElement("PM100009")
                                    Node.InnerText = CStr(RsParticelle.Fields("Numero").Value) + " - " + CStr(RsParticelle.Fields("Subalterno").Value)
                                    NodeRiga.AppendChild(Node)

                                    Node = XmlDocumento.CreateElement("PM100010")
                                    Node.InnerText = CStr(RsParticelle.Fields("prov").Value) + " - " + CStr(RsParticelle.Fields("com").Value)
                                    NodeRiga.AppendChild(Node)

                                    Node = XmlDocumento.CreateElement("PM100011")
                                    Node.InnerText = CStr(RsParticelle.Fields("comuni_prov").Value)
                                    NodeRiga.AppendChild(Node)

                                    Node = XmlDocumento.CreateElement("PM100012")
                                    Node.InnerText = CStr(RsParticelle.Fields("localita").Value)
                                    NodeRiga.AppendChild(Node)

                                    Node = XmlDocumento.CreateElement("PM100013")
                                    Node.InnerText = CDbl(RsParticelle.Fields("area").Value)
                                    NodeRiga.AppendChild(Node)

                                    XmlPadre.AppendChild(NodeRiga)

                                    RsParticelle.MoveNext()

                                Loop


                            Else

                                Log += CStr(Date.Now) + "   Impresa: " + piva + ".   L'impianto di " + CStr(Rs.Fields("veg_des").Value) + " - " + CStr(Rs.Fields("cul_des").Value) + " (Appezzamento: " + CStr(Rs.Fields("app_nome").Value) + ") non interseca alcuna particella." & vbCrLf & vbCrLf

                            End If


                        Else 'messaggio

                            Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Errore nella query PARTICELLE: " + Messaggio & vbCrLf & vbCrLf

                        End If 'messaggio


                        '==============================================================

                        'semina/trapianto
                        Node = XmlDocumento.CreateElement("PM100014")
                        If lav_cod = 2 Then
                            Node.InnerText = "Semina"
                        Else
                            If lav_cod = 71 Then
                                Node.InnerText = "Trapianto"
                            End If
                        End If
                        XmlPadre.AppendChild(Node)

                        '----------

                        'codice specie
                        Node = XmlDocumento.CreateElement("PM100015")
                        If CInt(Rs.Fields("veg_cod").Value) <> -1 Then
                            Node.InnerText = CStr(Rs.Fields("veg_cod").Value)
                        Else
                            'Node.InnerText = CStr(Date.Now.Millisecond)
                            Node.InnerText = CStr("n.d.")
                        End If
                        XmlPadre.AppendChild(Node)

                        '----------

                        ' specie
                        Node = XmlDocumento.CreateElement("PM100016")
                        If CStr(Rs.Fields("veg_des").Value) <> " " Then
                            Node.InnerText = CStr(Rs.Fields("veg_des").Value)
                        Else
                            Node.InnerText = "n.d."
                        End If
                        XmlPadre.AppendChild(Node)

                        '----------

                        'codice varietà
                        Node = XmlDocumento.CreateElement("PM100017")
                        If CInt(Rs.Fields("cul_cod").Value) <> -1 Then
                            Node.InnerText = CStr(Rs.Fields("cul_cod").Value)
                        Else
                            'Node.InnerText = CStr(Date.Now.Millisecond)
                            Node.InnerText = CStr("n.d.")
                        End If
                        XmlPadre.AppendChild(Node)

                        '----------

                        ' varietà
                        Node = XmlDocumento.CreateElement("PM100018")
                        If CStr(Rs.Fields("cul_des").Value) <> " " Then
                            Node.InnerText = CStr(Rs.Fields("cul_des").Value)
                        Else
                            Node.InnerText = "n.d."
                        End If
                        XmlPadre.AppendChild(Node)

                        '----------

                        'unità di misura
                        If CInt(Rs.Fields("udm_cod").Value) <> -1 Then
                            If CInt(Rs.Fields("udm_cod").Value) = 92 Then
                                Node = XmlDocumento.CreateElement("PM100019")
                                Node.InnerText = CStr("N.")
                                XmlPadre.AppendChild(Node)
                            Else
                                If CInt(Rs.Fields("udm_cod").Value) = 2 Then
                                    Node = XmlDocumento.CreateElement("PM100019")
                                    Node.InnerText = CStr("Kg")
                                    XmlPadre.AppendChild(Node)
                                End If

                            End If
                        End If

                        mat_cod = CInt(Rs.Fields("mat_cod").Value)
                        udm_cod = CInt(Rs.Fields("udm_cod").Value)
                        pro_cod = CInt(Rs.Fields("pro_cod").Value)

                        '----------

                        'resa
                        If CDbl(Rs.Fields("resa_effettiva").Value) <> 0 Then
                            Node = XmlDocumento.CreateElement("PM100020")
                            Node.InnerText = CDbl(Rs.Fields("resa_effettiva").Value)
                            XmlPadre.AppendChild(Node)
                        Else
                            If CDbl(Rs.Fields("resa_prevista").Value) <> 0 Then
                                Node = XmlDocumento.CreateElement("PM100020")
                                Node.InnerText = CDbl(Rs.Fields("resa_prevista").Value)
                                XmlPadre.AppendChild(Node)
                            End If
                        End If

                        '----------

                        'lotto seme
                        If CStr(Rs.Fields("Lotto").Value) <> " " Then

                            NodeRiga = XmlDocumento.CreateElement("Riga")

                            Node = XmlDocumento.CreateElement("PM100024")
                            Node.InnerText = CStr(Rs.Fields("Lotto").Value)
                            NodeRiga.AppendChild(Node)

                            XmlPadre.AppendChild(NodeRiga)

                        End If


                        '/***************** SEME PIANTINE *************/
                        'commento perchè ARP non ha inserito nessuna bolla e nessuna fattura

                        'StrSQL = ""
                        'StrSQL = "  SELECT  Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Data_Creazione, Agenda.Validita_Inizio AS Validita_Inizio_Agenda,   "
                        'StrSQL += "         Agenda.Validita_Fine AS Validita_Fine_Agenda, Movimenti.Id_Mov AS Id_Mov_Movimenti, Movimenti.Cau_Mov, Movimenti.Data_Movimento,  "
                        'StrSQL += "         Movimenti.Doc_Numero, Movimenti.Validita_Inizio AS Validita_Inizio_Movimenti, Movimenti.Validita_Fine AS Validita_Fine_Movimenti,  "
                        'StrSQL += "         Movimenti_dettagli.Id_Mov AS Id_Mov_Dettagli, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod,  "
                        'StrSQL += "         Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, Movimenti_dettagli.Validita_Inizio AS Validita_Inizio_Dettagli,  "
                        'StrSQL += "         Movimenti_dettagli.Validita_Fine AS Validita_Fine_Dettagli, Materie_Prime.Cod_Articolo, Materie_Prime.Mat_Des, Materie_Prime.Cul_Cod, "
                        'StrSQL += "         Movimenti.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc AS Fornitore "
                        'StrSQL += " FROM    Contatti INNER JOIN "
                        'StrSQL += "         Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto RIGHT OUTER JOIN "
                        'StrSQL += "         Agenda INNER JOIN "
                        'StrSQL += "         Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod INNER JOIN "
                        'StrSQL += "         Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda LEFT OUTER JOIN "
                        'StrSQL += "         Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ON  "
                        'StrSQL += "         Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm "
                        'StrSQL += " WHERE   Movimenti.Data_Movimento <= " & SQL_SaveDate(CDate(data_trapianto)) & "  "
                        'StrSQL += " AND     (Agenda.Lav_Cod = 1000 OR Agenda.Lav_Cod = 1025) "
                        'StrSQL += " AND     Agenda.PIVA = '" & SQL_SaveText(piva) & "'  "
                        'StrSQL += " AND     Agenda.sa_cod = '" & SQL_SaveText(sa_cod) & "'  "
                        'StrSQL += " AND     Movimenti_dettagli.Elem_Cod = 10  "
                        'StrSQL += " AND     Movimenti_dettagli.Mat_Cod = " & SQL_SaveNum(mat_cod) & " "
                        'StrSQL += " AND     Movimenti_dettagli.Udm_Cod = " & SQL_SaveNum(udm_cod) & "  "
                        'StrSQL += " AND     Movimenti_dettagli.Pro_Cod = " & SQL_SaveNum(pro_cod) & "  "
                        'StrSQL += " AND     Movimenti.Cau_Mov = '4000' "
                        'StrSQL += " ORDER BY Movimenti.Data_Movimento DESC "

                        'RsBolle = objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
                        '                        Session("ASG_Connessione_Server"), _
                        '                        StrSQL, _
                        '                        0, _
                        '                        Messaggio)

                        'If IsNothing(Messaggio) Then

                        '    If (Not IsNothing(RsBolle)) AndAlso _
                        '        (RsBolle.State <> 0) AndAlso _
                        '            (Not RsBolle.EOF) Then

                        '        Do While Not RsBolle.EOF

                        '            NodeRiga = XmlDocumento.CreateElement("Riga")

                        '            If Not IsDBNull(RsBolle.Fields("Cod_Contatto").Value) Then
                        '                Node = XmlDocumento.CreateElement("PM100021")
                        '                Node.InnerText = CStr(RsBolle.Fields("Cod_Contatto").Value)
                        '                NodeRiga.AppendChild(Node)
                        '            End If

                        '            If Not IsDBNull(CStr(RsBolle.Fields("Fornitore").Value)) Then
                        '                Node = XmlDocumento.CreateElement("PM100022")
                        '                Node.InnerText = CStr(RsBolle.Fields("Fornitore").Value)
                        '                NodeRiga.AppendChild(Node)
                        '            End If

                        '            If Not IsDBNull(CDate(RsBolle.Fields("Data_Movimento").Value)) Then
                        '                Node = XmlDocumento.CreateElement("PM100023")
                        '                Node.InnerText = Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")
                        '                NodeRiga.AppendChild(Node)
                        '            End If

                        '            If Not IsDBNull(CStr(RsBolle.Fields("cod_articolo").Value)) Then
                        '                Node = XmlDocumento.CreateElement("PM100024")
                        '                Node.InnerText = CStr(RsBolle.Fields("cod_articolo").Value)
                        '                NodeRiga.AppendChild(Node)
                        '            End If

                        '            If Not IsDBNull(CDbl(RsBolle.Fields("Qta").Value)) Then
                        '                Node = XmlDocumento.CreateElement("PM100026")
                        '                Node.InnerText = CDbl(RsBolle.Fields("Qta").Value)
                        '                NodeRiga.AppendChild(Node)
                        '            End If

                        '            XmlPadre.AppendChild(NodeRiga)

                        '            RsBolle.MoveNext()

                        '        Loop

                        '    Else

                        '        'Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono stati trovati fatture e ddt relativi alle piantine dell'impianto " + CStr(Rs.Fields("veg_des").Value) + " - " + CStr(Rs.Fields("cul_des").Value) + " (Appezzamento: " + CStr(Rs.Fields("app_nome").Value) + ")." & vbCrLf & vbCrLf

                        '    End If


                        'Else 'messaggio

                        '    Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Errore nella query FATTURE: " + Messaggio & vbCrLf & vbCrLf

                        'End If 'messaggio


                        '/***************** SEME PIANTINE *************/


                        '----------

                        'note
                        If CStr(Rs.Fields("mov_desc").Value) <> " " Then
                            Node = XmlDocumento.CreateElement("PM100027")
                            Node.InnerText = CStr(Rs.Fields("mov_desc").Value)
                            XmlPadre.AppendChild(Node)
                        End If

                        '--------------------------------------------------------------


                        'APPENDO ALLA RADICE
                        XmlRoot.AppendChild(XmlPadre)


                    Catch ex As Exception

                        Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                        " -   Errore durante la generazione del file xml Semina Trapianto: " + ex.Message & vbCrLf & vbCrLf

                    End Try

                    Rs.MoveNext()


                End While

                Rs.Close()

            Else 'nothing rs - state - eof

                Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative al centro: " + piva + " - " + CStr(sa_cod) + ", all'id_agenda: " + CStr(id_agenda) + " e al lav_cod: " + CStr(lav_cod) + ". " & vbCrLf & vbCrLf

            End If 'nothing

        Else 'rs - state - eof

            Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Errore nella QUERY RINTRACCIO Semina/Trapianto: " + Messaggio & vbCrLf & vbCrLf

        End If 'messaggio



        '------------------------------------------------
        '------------ FINE CICLO RINTRACCIO -------------
        '------------------------------------------------

        flag_semtrap = True

        XmlRoot_send = XmlRoot
        XmlDocumento_send = XmlDocumento


    End Sub


    '###########################################################################
    Private Function Crea_DT_Generale()

        Dim DT As New DataTable

        '----- Definisco la struttura del DataTable

        DT.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        DT.Columns.Add(New DataColumn("campo_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("campo_des", GetType(String)))
        DT.Columns.Add(New DataColumn("des_lib", GetType(String)))
        DT.Columns.Add(New DataColumn("data_movimento", GetType(String)))
        DT.Columns.Add(New DataColumn("qta_ril", GetType(Double)))
        DT.Columns.Add(New DataColumn("mov_desc", GetType(String)))

        Return DT

    End Function

    '###########################################################################
    Private Function Crea_DT_Impianti()

        Dim DT As New DataTable

        '----- Definisco la struttura del DataTable

        DT.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        DT.Columns.Add(New DataColumn("app_nome", GetType(String)))
        DT.Columns.Add(New DataColumn("id_reg", GetType(Integer)))
        DT.Columns.Add(New DataColumn("sup_imp", GetType(Double)))
        DT.Columns.Add(New DataColumn("qta_impianto", GetType(Double)))


        Return DT

    End Function

    '###########################################################################
    Private Function Crea_DT_Generale_Impianto()

        Dim DT As New DataTable

        '----- Definisco la struttura del DataTable

        DT.Columns.Add(New DataColumn("des_lib", GetType(String)))
        DT.Columns.Add(New DataColumn("data_movimento", GetType(String)))

        DT.Columns.Add(New DataColumn("rag_soc", GetType(String)))

        DT.Columns.Add(New DataColumn("campo_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("campo_des", GetType(String)))
        DT.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        DT.Columns.Add(New DataColumn("app_nome", GetType(String)))
        DT.Columns.Add(New DataColumn("id_reg", GetType(Integer)))
        DT.Columns.Add(New DataColumn("cul_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("sup_imp", GetType(Double)))
        DT.Columns.Add(New DataColumn("qta_impianto", GetType(Double)))

        DT.Columns.Add(New DataColumn("qta_ril", GetType(Double)))
        DT.Columns.Add(New DataColumn("mov_desc", GetType(String)))

        Return DT

    End Function


    '###########################################################################
    Private Function Crea_DT_Formulati_Old()

        Dim DT As New DataTable

        '----- Definisco la struttura del DataTable

        DT.Columns.Add(New DataColumn("fr_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("fr_des", GetType(String)))
        DT.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("extra_int", GetType(Integer)))
        DT.Columns.Add(New DataColumn("udm_des", GetType(String)))
        DT.Columns.Add(New DataColumn("qta", GetType(Double)))
        DT.Columns.Add(New DataColumn("pa_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("pa_des", GetType(String)))
        DT.Columns.Add(New DataColumn("titolo", GetType(Double)))

        Return DT

    End Function


    '###########################################################################
    Private Function Crea_DT_Formulati()

        Dim DT As New DataTable

        '----- Definisco la struttura del DataTable

        DT.Columns.Add(New DataColumn("fr_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("fr_des", GetType(String)))
        DT.Columns.Add(New DataColumn("extra_int", GetType(Integer)))
        DT.Columns.Add(New DataColumn("UdmDose", GetType(String)))
        DT.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("UdmImp", GetType(String)))
        DT.Columns.Add(New DataColumn("qta", GetType(Double)))
        DT.Columns.Add(New DataColumn("pa_cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("pa_des", GetType(String)))
        DT.Columns.Add(New DataColumn("titolo", GetType(Double)))

        Return DT

    End Function

    ''###########################################################################
    'Private Function Crea_DT_Avversita()

    '    Dim DT As New DataTable

    '    '----- Definisco la struttura del DataTable

    '    DT.Columns.Add(New DataColumn("av_cod", GetType(Integer)))
    '    DT.Columns.Add(New DataColumn("av_des_vol", GetType(String)))

    '    Return DT

    'End Function



    '#############################################################################################################
    Private Sub Rintraccio_Trattamenti(ByRef Log As String, ByVal Punto As Integer, ByVal piva As String, ByVal sa_cod As Integer, ByVal id_agenda As Integer, ByVal lav_cod As Integer, ByRef flag_trattam As Boolean, ByRef XmlRoot_send As XmlElement, ByRef XmlDocumento_send As XmlDocument)

        Dim XmlDocumento As XmlDocument
        Dim XmlRoot, XmlPadre As XmlElement
        Dim Node, NodeRiga As XmlElement 'As XmlNode
        Dim xPI As XmlProcessingInstruction

        Dim num_record, i, j As Integer

        Dim Rs As ADODB.Recordset
        Dim objSQL As New Codex_Utility.Sql
        'Dim objSQL As New Codex_Utility.Sql
        Dim DT_globale As DataTable

        'Dim DT_generale As DataTable
        'Dim DT_impianti As New DataTable
        Dim DT_formulati As DataTable
        'Dim DT_avversita As New DataTable
        Dim DT_Generale_Impianto As DataTable

        'Dim DR_generale As DataRow
        'Dim DR_impianti As DataRow
        Dim DR_formulati As DataRow
        'Dim DR_avversita As DataRow
        Dim DR_Generale_Impianto As DataRow

        'riempio il dataset....
        'Dim objSqlDis As New Codex_Utility_Sql_DistinctOnDT
        Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility

        Dim Messaggio, StrSQL, anno, data_trapianto As String
        Dim mat_cod, elem_cod, pro_cod, udm_cod As Integer

        Dim str_avcod() As String
        Dim str_avdes() As String
        Dim str_avgru() As String
        Dim str_avgrudes() As String

        '------------------------------------------------

        If flag_trattam = False Then

            XmlDocumento = New XmlDocument

            'xPI = XmlDocumento.CreateProcessingInstruction("xml", "version=""1.0"" encoding=""UTF-8""")
            'XmlDocumento.AppendChild(xPI)

            XmlRoot = XmlDocumento.CreateElement("dataroot")

            ' Create an XML declaration. 
            Dim xmldecl As XmlDeclaration
            xmldecl = XmlDocumento.CreateXmlDeclaration("1.0", Nothing, Nothing)
            xmldecl.Encoding = "UTF-8"
            'xmldecl.Standalone = "yes"
            XmlDocumento.AppendChild(xmldecl)

        Else

            XmlRoot = XmlRoot_send
            XmlDocumento = XmlDocumento_send

        End If


        '-------------------------------------------------------
        ' VAI CON LA QUERY RINTRACCIO Trattamenti!!!!!!!!
        '-------------------------------------------------------

        Select Case lav_cod

            Case LAVCOD_DISERBO, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO '74 LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, 18 LAVCOD_DISERBO

                StrSQL = ""
                StrSQL = " SELECT   Agenda.PIVA, ISNULL(Imprese.rag_soc, ' ') AS rag_soc, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, ISNULL(Agenda.des_lib, ' ') AS des_lib, Movimenti.Id_Mov, Movimenti.Cau_Mov,   "
                StrSQL += "         ISNULL(Movimenti.Mov_Desc, ' ') AS mov_desc, ISNULL(Movimenti.Data_Movimento, ' ') AS data_movimento, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, Movimenti_dettagli.Extra_Int,  "
                StrSQL += "         Campi.Campo_Cod, ISNULL(Campi.Campo_Des, ' ') AS campo_des, Reg_Impianti.APPEZZA, ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome, Reg_Impianti.Id_Reg, "
                StrSQL += "         ISNULL(Reg_Impianti.Sup_Imp, 0) AS sup_imp,  Mov_Destinazioni.Qta AS qta_impianto, Formulati.Fr_Cod, Formulati.Fr_Des, PrincipiAttivi.Pa_Cod,  "
                StrSQL += "         PrincipiAttivi.Pa_Des, FormulatixPrincipiAttivi.Titolo, ISNULL(Acqua.Qta_Ril, 0) AS qta_ril, UdmDose.UDM_DES AS UdmDose, ISNULL(Avversita.Av_Cod, 0) AS av_cod, Avversita.Av_Des_Vol, ISNULL(GruppoAvversita.Av_Gru, 0) AS av_gru, GruppoAvversita.Av_Gru_Des, ISNULL(Reg_Impianti.CUL_COD, - 1) AS cul_cod, UdmImp.UDM_DES AS UdmImp"
                'FROM
                StrSQL += " FROM    Agenda "
                StrSQL += "         INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda "
                StrSQL += "         INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  "
                StrSQL += "             Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov "
                StrSQL += "         INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND   "
                StrSQL += "             Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  "
                StrSQL += "             Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det "
                StrSQL += "         INNER JOIN Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  "
                StrSQL += "             Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg "
                StrSQL += "         INNER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod "
                StrSQL += "         INNER JOIN FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod "
                StrSQL += "         INNER JOIN  PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod "
                StrSQL += "         INNER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  "
                StrSQL += "             Reg_Impianti.APPEZZA = Appezzamento.APPEZZA "
                StrSQL += "         INNER JOIN Imprese ON Agenda.PIVA = Imprese.PIVA "
                StrSQL += "         INNER JOIN Mov_Dettaglio_Tecnico Acqua ON Movimenti.PIVA = Acqua.Piva AND Movimenti.Sa_Cod = Acqua.Sa_Cod AND "
                StrSQL += "             Movimenti.Id_Agenda = Acqua.Id_Agenda AND Movimenti.Id_Mov = Acqua.Id_Mov "
                StrSQL += "         INNER JOIN Mov_Dettaglio_Tecnico Mov_Avversita ON Movimenti.PIVA = Mov_Avversita.Piva AND Movimenti.Sa_Cod = Mov_Avversita.Sa_Cod AND  "
                StrSQL += "             Movimenti.Id_Agenda = Mov_Avversita.Id_Agenda AND Movimenti.Id_Mov = Mov_Avversita.Id_Mov "
                StrSQL += "         LEFT OUTER JOIN Avversita ON Mov_Avversita.Av_Cod = Avversita.Av_Cod "
                StrSQL += "         LEFT OUTER JOIN GruppoAvversita ON Mov_Avversita.Av_Gru = GruppoAvversita.Av_Gru "
                StrSQL += "         LEFT OUTER JOIN Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod "
                StrSQL += "         INNER JOIN UnitaMisura UdmDose ON Movimenti_dettagli.Extra_Int = UdmDose.UDM_COD "
                StrSQL += "         INNER JOIN UnitaMisura UdmImp ON Movimenti_dettagli.Udm_Cod = UdmImp.UDM_COD "
                'WHERE
                StrSQL += " WHERE   Agenda.PIVA = " & Agro_SQL_SaveText(piva) & " "
                StrSQL += " AND     Agenda.SA_COD = " & Agro_SQL_SaveNum(sa_cod) & " "
                StrSQL += " AND     Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & " "
                StrSQL += " AND     Agenda.lav_cod = " & Agro_SQL_SaveNum(lav_cod) & " "
                StrSQL += " AND     Movimenti.Cau_Mov = '2050' "
                StrSQL += " AND     Acqua.Av_Cod = 0 "
                StrSQL += " AND     Acqua.Av_Gru = 0  "
                StrSQL += " AND     Mov_Avversita.qta_ril = 0 "
                StrSQL += vbCrLf

            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_GEODISINFESTAZIONE '103 LAVCOD_TRATTAMENTO_FITOREGOLATORE, 155 LAVCOD_GEODISINFESTAZIONE


                StrSQL = ""
                StrSQL = " SELECT   Agenda.PIVA, ISNULL(Imprese.rag_soc, ' ') AS rag_soc, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, ISNULL(Agenda.des_lib, ' ') AS des_lib, Movimenti.Id_Mov, Movimenti.Cau_Mov,   "
                StrSQL += "         ISNULL(Movimenti.Mov_Desc, ' ') AS mov_desc, ISNULL(Movimenti.Data_Movimento, ' ') AS data_movimento, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, Movimenti_dettagli.Extra_Int,  "
                StrSQL += "         Campi.Campo_Cod, ISNULL(Campi.Campo_Des, ' ') AS campo_des, Reg_Impianti.APPEZZA, ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome, Reg_Impianti.Id_Reg, "
                StrSQL += "         ISNULL(Reg_Impianti.Sup_Imp, 0) AS sup_imp,  Mov_Destinazioni.Qta AS qta_impianto, Formulati.Fr_Cod, Formulati.Fr_Des, PrincipiAttivi.Pa_Cod,  "
                StrSQL += "         PrincipiAttivi.Pa_Des, FormulatixPrincipiAttivi.Titolo, ISNULL(Acqua.Qta_Ril, 0) AS qta_ril, UdmDose.UDM_DES AS UdmDose, 0 AS av_cod, '' AS Av_Des_Vol, 0 AS av_gru, '' AS Av_Gru_Des, ISNULL(Reg_Impianti.CUL_COD, - 1) AS cul_cod, UdmImp.UDM_DES AS UdmImp "
                'FROM
                StrSQL += " FROM    Agenda "
                StrSQL += "         INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda "
                StrSQL += "         INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  "
                StrSQL += "             Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov "
                StrSQL += "         INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND   "
                StrSQL += "             Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  "
                StrSQL += "             Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det "
                StrSQL += "         INNER JOIN Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  "
                StrSQL += "             Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg "
                StrSQL += "         INNER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod "
                StrSQL += "         INNER JOIN FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod "
                StrSQL += "         INNER JOIN  PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod "
                StrSQL += "         INNER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  "
                StrSQL += "             Reg_Impianti.APPEZZA = Appezzamento.APPEZZA "
                StrSQL += "         INNER JOIN Imprese ON Agenda.PIVA = Imprese.PIVA "
                StrSQL += "         INNER JOIN Mov_Dettaglio_Tecnico Acqua ON Movimenti.PIVA = Acqua.Piva AND Movimenti.Sa_Cod = Acqua.Sa_Cod AND "
                StrSQL += "             Movimenti.Id_Agenda = Acqua.Id_Agenda AND Movimenti.Id_Mov = Acqua.Id_Mov "
                StrSQL += "         LEFT OUTER JOIN Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod "
                StrSQL += "         INNER JOIN UnitaMisura UdmDose ON Movimenti_dettagli.Extra_Int = UdmDose.UDM_COD "
                StrSQL += "         INNER JOIN UnitaMisura UdmImp ON Movimenti_dettagli.Udm_Cod = UdmImp.UDM_COD "
                'WHERE
                StrSQL += " WHERE   Agenda.PIVA = " & Agro_SQL_SaveText(piva) & " "
                StrSQL += " AND     Agenda.SA_COD = " & Agro_SQL_SaveNum(sa_cod) & " "
                StrSQL += " AND     Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & " "
                StrSQL += " AND     Agenda.lav_cod = " & Agro_SQL_SaveNum(lav_cod) & " "
                StrSQL += " AND     Movimenti.Cau_Mov = '2050' "
                StrSQL += " AND     Acqua.Av_Cod = 0 "
                StrSQL += " AND     Acqua.Av_Gru = 0  "
                StrSQL += vbCrLf


        End Select

        DT_globale = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                                    Session("ASG_Connessione_Server"),
                                    StrSQL,
                                    1,
                                    Messaggio)



        If IsNothing(Messaggio) Then

            If (Not IsNothing(DT_globale)) Then

                num_record = DT_globale.Rows.Count

                If num_record <> 0 Then

                    Try

                        'Dim str_apezza() As String = objSqlDis.SelectDistinct(DT_globale, "appezza")
                        'Dim str_idreg() As String = objSqlDis.SelectDistinct(DT_globale, "id_reg")
                        'Dim str_frcod() As String = objSqlDis.SelectDistinct(DT_globale, "fr_cod")

                        If lav_cod = 74 Or lav_cod = 18 Then
                            str_avcod = objSqlDis.SelectDistinct(DT_globale, "av_cod")
                            str_avdes = objSqlDis.SelectDistinct(DT_globale, "av_des_vol")
                            str_avgru = objSqlDis.SelectDistinct(DT_globale, "av_gru")
                            str_avgrudes = objSqlDis.SelectDistinct(DT_globale, "av_gru_des")
                        End If


                        'DT_generale = Crea_DT_Generale()
                        'DT_impianti = Crea_DT_Impianti()
                        DT_formulati = Crea_DT_Formulati()
                        'DT_avversita = Crea_DT_Avversita()
                        DT_Generale_Impianto = Crea_DT_Generale_Impianto()

                        '''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                        '''Questo serviva se cercavamo la quantità distribuita sull'impianto
                        ''Dim Hash_Form_Imp As New Hashtable
                        ''Dim key_Form_Imp As String
                        '''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

                        For i = 0 To num_record - 1

                            'DR_generale = DT_generale.NewRow
                            'DR_impianti = DT_impianti.NewRow
                            DR_formulati = DT_formulati.NewRow
                            'DR_avversita = DT_avversita.NewRow
                            DR_Generale_Impianto = DT_Generale_Impianto.NewRow


                            DR_Generale_Impianto.Item("des_lib") = DT_globale.Rows(i).Item(5)
                            DR_Generale_Impianto.Item("data_movimento") = DT_globale.Rows(i).Item(9)

                            DR_Generale_Impianto.Item("rag_soc") = DT_globale.Rows(i).Item(1)

                            DR_Generale_Impianto.Item("campo_cod") = DT_globale.Rows(i).Item(13)
                            DR_Generale_Impianto.Item("campo_des") = DT_globale.Rows(i).Item(14)
                            DR_Generale_Impianto.Item("appezza") = DT_globale.Rows(i).Item(15)
                            DR_Generale_Impianto.Item("app_nome") = DT_globale.Rows(i).Item(16)
                            DR_Generale_Impianto.Item("id_reg") = DT_globale.Rows(i).Item(17)
                            DR_Generale_Impianto.Item("cul_cod") = DT_globale.Rows(i).Item(31)
                            DR_Generale_Impianto.Item("sup_imp") = DT_globale.Rows(i).Item(18)
                            DR_Generale_Impianto.Item("qta_impianto") = DT_globale.Rows(i).Item(19)

                            DR_Generale_Impianto.Item("qta_ril") = DT_globale.Rows(i).Item(25)
                            DR_Generale_Impianto.Item("mov_desc") = DT_globale.Rows(i).Item(8)


                            'DR_impianti.Item("campo_cod") = DT_globale.Rows(i).Item(13)
                            'DR_impianti.Item("appezza") = DT_globale.Rows(i).Item(15)
                            'DR_impianti.Item("app_nome") = DT_globale.Rows(i).Item(16)
                            'DR_impianti.Item("id_reg") = DT_globale.Rows(i).Item(17)
                            'DR_impianti.Item("sup_imp") = DT_globale.Rows(i).Item(18)
                            'DR_impianti.Item("qta_impianto") = DT_globale.Rows(i).Item(19)


                            DR_formulati.Item("fr_cod") = DT_globale.Rows(i).Item(20)
                            DR_formulati.Item("fr_des") = DT_globale.Rows(i).Item(21)
                            DR_formulati.Item("udm_cod") = DT_globale.Rows(i).Item(10)
                            DR_formulati.Item("extra_int") = DT_globale.Rows(i).Item(12)
                            DR_formulati.Item("UdmDose") = DT_globale.Rows(i).Item(26)
                            DR_formulati.Item("UdmImp") = DT_globale.Rows(i).Item(32)
                            DR_formulati.Item("qta") = DT_globale.Rows(i).Item(11)
                            DR_formulati.Item("pa_cod") = DT_globale.Rows(i).Item(22)
                            DR_formulati.Item("pa_des") = DT_globale.Rows(i).Item(23)
                            DR_formulati.Item("titolo") = DT_globale.Rows(i).Item(24)

                            'DR_avversita.Item("av_cod") = DT_globale.Rows(i).Item(27)
                            'DR_avversita.Item("av_des_vol") = DT_globale.Rows(i).Item(28)

                            'DT_generale.Rows.Add(DR_generale)
                            'DT_impianti.Rows.Add(DR_impianti)
                            DT_formulati.Rows.Add(DR_formulati)
                            'DT_avversita.Rows.Add(DR_avversita)
                            DT_Generale_Impianto.Rows.Add(DR_Generale_Impianto)


                            '''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                            '''Questo serviva se cercavamo la quantità distribuita sull'impianto
                            ''key_Form_Imp = CStr(DT_globale.Rows(i).Item(13)) + "|" + _
                            ''                CStr(DT_globale.Rows(i).Item(15)) + "|" + _
                            ''                CStr(DT_globale.Rows(i).Item(17)) + "|" + _
                            ''                CStr(DT_globale.Rows(i).Item(20))

                            ''If Not Hash_Form_Imp.ContainsKey(key_Form_Imp) Then

                            ''    Hash_Form_Imp.Add(key_Form_Imp, CDbl(DT_globale.Rows(i).Item(19)))

                            ''End If
                            '''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


                        Next


                        '==============================================================


                        '==============================================
                        Dim Hash_Impianti As Hashtable
                        Dim chiave_impianto() As String = {"campo_cod", "appezza", "id_reg"}
                        Dim sup_totale As Double = 0

                        Hash_Impianti = objSqlDis.SelectDistinctWithHash_Base(DT_Generale_Impianto, chiave_impianto)

                        'SUP totale - sommo le superfici di tutti gli impianti trattati
                        For i = 0 To DT_Generale_Impianto.Rows.Count - 1

                            sup_totale = sup_totale + CDbl(DT_Generale_Impianto.Rows(i).Item("sup_imp"))

                        Next

                        '==============================================

                        'x ogni impianto
                        For i = 0 To DT_Generale_Impianto.Rows.Count - 1


                            'CREO IL P99M100000
                            XmlPadre = XmlDocumento.CreateElement("P" + CStr(Punto) + "M100003")

                            '---------------


                            'descrizione
                            Node = XmlDocumento.CreateElement("Descrizione")

                            If CStr(DT_Generale_Impianto.Rows(i).Item("data_movimento")) <> " " Then
                                anno = Left(CStr(Format(CDate(DT_Generale_Impianto.Rows(i).Item("data_movimento")), "yyyy-MM-dd")), 4)
                            End If
                            Node.InnerText = "Trattamenti " + anno

                            XmlPadre.AppendChild(Node)

                            '--------------

                            'data riferimento
                            Node = XmlDocumento.CreateElement("DataRiferimento")

                            If CStr(DT_Generale_Impianto.Rows(i).Item("data_movimento")) <> " " Then
                                Node.InnerText = Format(CDate(DT_Generale_Impianto.Rows(i).Item("data_movimento")), "yyyy-MM-dd")
                            End If
                            XmlPadre.AppendChild(Node)

                            '----------

                            'partita iva
                            Node = XmlDocumento.CreateElement("PM100068")
                            Node.InnerText = piva
                            XmlPadre.AppendChild(Node)

                            '-------

                            'ragione sociale
                            Node = XmlDocumento.CreateElement("PM100069")
                            If CStr(DT_Generale_Impianto.Rows(i).Item("rag_soc")) <> " " Then
                                Node.InnerText = CStr(DT_Generale_Impianto.Rows(i).Item("rag_soc"))
                            Else
                                Node.InnerText = "n.d."
                            End If
                            XmlPadre.AppendChild(Node)

                            '--------

                            'codice appezzamento
                            Dim CodiceCampoAppezza As String = ""
                            Dim CodiceVarieta As String = ""

                            Node = XmlDocumento.CreateElement("PM100070")

                            If CInt(DT_Generale_Impianto.Rows(i).Item("cul_cod")) <> -1 Then
                                CodiceVarieta = CStr(DT_Generale_Impianto.Rows(i).Item("cul_cod"))
                            Else
                                CodiceVarieta = CStr("n.d.")
                            End If

                            If CStr(DT_Generale_Impianto.Rows(i).Item("campo_des")) <> " " Then

                                CodiceCampoAppezza = CStr(CodiceCampoAppezza_from_Nome(CStr(DT_Generale_Impianto.Rows(i).Item("campo_des"))))
                                Node.InnerText = CStr(CInt(CodiceCampoAppezza)) + "__" + CodiceVarieta

                            Else
                                If CStr(DT_Generale_Impianto.Rows(i).Item("app_nome")) <> " " Then

                                    CodiceCampoAppezza = CStr(CodiceCampoAppezza_from_Nome(CStr(DT_Generale_Impianto.Rows(i).Item("app_nome"))))
                                    Node.InnerText = CStr(CInt(CodiceCampoAppezza)) + "__" + CodiceVarieta

                                Else
                                    Node.InnerText = "n.d.__n.d."
                                End If
                            End If

                            XmlPadre.AppendChild(Node)

                            '--------

                            'descr appezzamento
                            Node = XmlDocumento.CreateElement("PM100071")
                            If CStr(DT_Generale_Impianto.Rows(i).Item("campo_des")) <> " " Then
                                Node.InnerText = CStr(DT_Generale_Impianto.Rows(i).Item("campo_des"))
                            Else
                                If CStr(DT_Generale_Impianto.Rows(i).Item("app_nome")) <> " " Then
                                    Node.InnerText = CStr(DT_Generale_Impianto.Rows(i).Item("app_nome"))
                                Else
                                    Node.InnerText = "n.d."
                                End If
                            End If
                            XmlPadre.AppendChild(Node)

                            '------------

                            'data operazione
                            Node = XmlDocumento.CreateElement("PM100072")
                            If CStr(DT_Generale_Impianto.Rows(i).Item("data_movimento")) <> " " Then
                                Node.InnerText = Format(CDate(DT_Generale_Impianto.Rows(i).Item("data_movimento")), "yyyy-MM-dd")
                                data_trapianto = Format(CDate(DT_Generale_Impianto.Rows(i).Item("data_movimento")), "yyyy-MM-dd")
                            Else
                                Node.InnerText = Format(CDate(Date.Today), "yyyy-MM-dd")
                            End If
                            XmlPadre.AppendChild(Node)

                            '------------

                            'annata operazione
                            Node = XmlDocumento.CreateElement("PM100073")
                            If CStr(DT_Generale_Impianto.Rows(i).Item("data_movimento")) <> " " Then
                                Node.InnerText = CInt(anno)
                            Else
                                Node.InnerText = CInt(Left(CStr(Format(CDate(Date.Today), "yyyy-MM-dd")), 4))
                            End If
                            XmlPadre.AppendChild(Node)

                            '------------------

                            'tipo operazione
                            Node = XmlDocumento.CreateElement("PM100074")
                            If CStr(DT_Generale_Impianto.Rows(i).Item("des_lib")) <> " " Then
                                Node.InnerText = CStr(DT_Generale_Impianto.Rows(i).Item("des_lib"))
                            Else
                                Node.InnerText = "n.d."
                            End If
                            XmlPadre.AppendChild(Node)

                            '==============================================================

                            'sup appezzamento
                            Dim sup_imp As Double
                            sup_imp = DT_Generale_Impianto.Rows(i).Item("sup_imp")

                            Node = XmlDocumento.CreateElement("PM100082")
                            Node.InnerText = sup_imp
                            XmlPadre.AppendChild(Node)


                            '==============================================================
                            'acqua

                            Node = XmlDocumento.CreateElement("PM100083")

                            'l'acqua è salvata in ettolitri ---> devo convertire in litri

                            If Left(CStr(DT_Generale_Impianto.Rows(i).Item("qta_ril")), 1) = "-" Then

                                'la quantità è negativa ---> allora è quantità x ettaro

                                Node.InnerText = DT_Generale_Impianto.Rows(i).Item("qta_ril") * 100 * -1

                            Else

                                'la quantità è positiva ---> allora è quantità totale e devo dividere per gli ettari

                                If sup_totale <> 0 Then

                                    Node.InnerText = Math.Round((DT_Generale_Impianto.Rows(i).Item("qta_ril") / sup_totale * 100), 4)

                                Else

                                    Node.InnerText = DT_Generale_Impianto.Rows(i).Item("qta_ril") * 100

                                End If


                            End If

                            XmlPadre.AppendChild(Node)

                            '==============================================================


                            'formulati

                            Dim Hash_Formulati As Hashtable
                            Dim chiave_formulato() As String = {"fr_cod", "pa_cod"}

                            Hash_Formulati = objSqlDis.SelectDistinctWithHash_Base(DT_formulati, chiave_formulato)

                            For j = 0 To DT_formulati.Rows.Count - 1

                                'For i = 0 To Lista.Length - 1

                                'Dim Righe() As DataRow = DT_formulati.Select("fr_cod=" & Lista(i).Value & "AND pa_cod=" & Lista(i).Text)

                                NodeRiga = XmlDocumento.CreateElement("Riga")

                                Node = XmlDocumento.CreateElement("PM100075")
                                'Node.InnerText = CInt(Righe(0).Item("pa_cod"))
                                Node.InnerText = CInt(DT_formulati.Rows(j).Item("pa_cod"))
                                NodeRiga.AppendChild(Node)

                                Node = XmlDocumento.CreateElement("PM100076")
                                'Node.InnerText = CStr(Righe(0).Item("pa_des"))
                                Node.InnerText = CStr(DT_formulati.Rows(j).Item("pa_des"))
                                NodeRiga.AppendChild(Node)

                                Node = XmlDocumento.CreateElement("PM100077")
                                'Node.InnerText = CInt(Righe(0).Item("fr_cod"))
                                Node.InnerText = CInt(DT_formulati.Rows(j).Item("fr_cod"))
                                NodeRiga.AppendChild(Node)

                                Node = XmlDocumento.CreateElement("PM100078")
                                'Node.InnerText = CStr(Righe(0).Item("fr_des"))
                                Node.InnerText = CStr(DT_formulati.Rows(j).Item("fr_des"))
                                NodeRiga.AppendChild(Node)

                                Node = XmlDocumento.CreateElement("PM100079")
                                '''Node.InnerText = CStr(Righe(0).Item("udm_des"))
                                'Node.InnerText = CStr(DT_formulati.Rows(j).Item("UdmImp"))
                                Node.InnerText = CStr(DT_formulati.Rows(j).Item("UdmDose"))
                                NodeRiga.AppendChild(Node)


                                Node = XmlDocumento.CreateElement("PM100080")
                                '''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                                '''Questo serviva se cercavamo la quantità distribuita sull'impianto
                                ''key_Form_Imp = CStr(DT_Generale_Impianto.Rows(i).Item(3)) + "|" + _
                                ''                CStr(DT_Generale_Impianto.Rows(i).Item(5)) + "|" + _
                                ''                CStr(DT_Generale_Impianto.Rows(i).Item(7)) + "|" + _
                                ''                CStr(DT_formulati.Rows(j).Item("fr_cod"))


                                ''Node.InnerText = Hash_Form_Imp(key_Form_Imp)
                                '''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
                                Node.InnerText = CStr(DT_formulati.Rows(j).Item("Qta"))
                                NodeRiga.AppendChild(Node)


                                Node = XmlDocumento.CreateElement("PM100081")
                                '''Node.InnerText = CDbl(Righe(0).Item("titolo"))
                                Node.InnerText = CDbl(DT_formulati.Rows(j).Item("titolo"))
                                NodeRiga.AppendChild(Node)

                                XmlPadre.AppendChild(NodeRiga)

                            Next


                            '==============================================================

                            'avversita

                            If lav_cod = 74 Or lav_cod = 18 Then

                                If str_avcod(0) <> 0 Then

                                    For j = 0 To str_avcod.Length - 1

                                        NodeRiga = XmlDocumento.CreateElement("Riga")

                                        If str_avcod(j) <> 0 Then
                                            Node = XmlDocumento.CreateElement("PM100084")
                                            Node.InnerText = str_avcod(j)
                                            NodeRiga.AppendChild(Node)

                                            Node = XmlDocumento.CreateElement("PM100085")
                                            Node.InnerText = str_avdes(j)
                                            NodeRiga.AppendChild(Node)

                                        End If

                                        XmlPadre.AppendChild(NodeRiga)

                                    Next

                                Else

                                    If str_avgru(0) <> 0 Then

                                        For j = 0 To str_avgru.Length - 1

                                            NodeRiga = XmlDocumento.CreateElement("Riga")

                                            If str_avgru(j) <> 0 Then
                                                Node = XmlDocumento.CreateElement("PM100084")
                                                Node.InnerText = str_avgru(j)
                                                NodeRiga.AppendChild(Node)

                                                Node = XmlDocumento.CreateElement("PM100085")
                                                Node.InnerText = str_avgrudes(j)
                                                NodeRiga.AppendChild(Node)
                                            End If

                                            XmlPadre.AppendChild(NodeRiga)

                                        Next

                                    End If

                                End If

                            End If

                            '==============================================================


                            'note
                            If CStr(DT_Generale_Impianto.Rows(i).Item("mov_desc")) <> " " Then
                                Node = XmlDocumento.CreateElement("PM100086")
                                Node.InnerText = CStr(DT_Generale_Impianto.Rows(i).Item("mov_desc"))
                                XmlPadre.AppendChild(Node)
                            End If

                            '--------------------------------------------------------------


                            'APPENDO ALLA RADICE
                            XmlRoot.AppendChild(XmlPadre)

                        Next


                    Catch ex As Exception

                        Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                        " -   Errore durante la generazione del file xml Trattamenti: " + ex.Message & vbCrLf & vbCrLf

                    End Try



                Else 'num_record

                    Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative al centro: " + piva + " - " + CStr(sa_cod) + ", all'id_agenda: " + CStr(id_agenda) + " e al lav_cod: " + CStr(lav_cod) + ". " & vbCrLf & vbCrLf

                End If

            Else 'nothing 

                Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative al centro: " + piva + " - " + CStr(sa_cod) + ", all'id_agenda: " + CStr(id_agenda) + " e al lav_cod: " + CStr(lav_cod) + ". " & vbCrLf & vbCrLf

            End If 'nothing

        Else 'messaggio

            Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Errore nella QUERY RINTRACCIO Trattamenti: " + Messaggio & vbCrLf & vbCrLf

        End If 'messaggio





        '------------------------------------------------
        '------------ FINE CICLO RINTRACCIO -------------
        '------------------------------------------------

        flag_trattam = True

        XmlRoot_send = XmlRoot
        XmlDocumento_send = XmlDocumento





    End Sub




    '#############################################################################################################
    Private Sub Rintraccio_Piogge_Irrigazioni(ByRef Log As String, ByVal Punto As Integer, ByVal piva As String, ByVal sa_cod As Integer, ByVal id_agenda As Integer, ByVal lav_cod As Integer, ByRef flag_pioirr As Boolean, ByRef XmlRoot_send As XmlElement, ByRef XmlDocumento_send As XmlDocument)

        Dim XmlDocumento As XmlDocument
        Dim XmlRoot, XmlPadre As XmlElement
        Dim Node, NodeRiga As XmlElement
        Dim xPI As XmlProcessingInstruction

        Dim num_record, i As Integer
        Dim udm_cod As Integer

        Dim sup_trattata As Double = 0
        Dim dose As Double = 0
        Dim CodiceVarieta As String = ""

        Dim Rs As ADODB.Recordset
        Dim objSQL As New Codex_Utility.Sql
        ' Dim objSQL As New Codex_Utility.Sql

        Dim Messaggio, StrSQL, anno, rag_soc, note, campo_des, campo_cod, udm_des As String
        Dim data_movimento As String

        '------------------------------------------------

        If flag_pioirr = False Then

            XmlDocumento = New XmlDocument

            'xPI = XmlDocumento.CreateProcessingInstruction("xml", "version=""1.0"" encoding=""UTF-8""")
            'XmlDocumento.AppendChild(xPI)

            ' Create an XML declaration. 
            Dim xmldecl As XmlDeclaration
            xmldecl = XmlDocumento.CreateXmlDeclaration("1.0", Nothing, Nothing)
            xmldecl.Encoding = "UTF-8"
            'xmldecl.Standalone = "yes"
            XmlDocumento.AppendChild(xmldecl)

            XmlRoot = XmlDocumento.CreateElement("dataroot")

        Else

            XmlRoot = XmlRoot_send
            XmlDocumento = XmlDocumento_send

        End If


        '----------------------------------------------------------
        ' VAI CON LA QUERY RINTRACCIO Piogge - Irrigazioni!!!!!!!!
        '----------------------------------------------------------

        StrSQL = ""
        StrSQL = "  SELECT  Agenda.PIVA, ISNULL(Imprese.rag_soc, ' ') AS rag_soc, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, ISNULL(Agenda.des_lib, ' ') AS des_lib, Movimenti.Id_Mov, Movimenti.Cau_Mov,  "
        StrSQL += "         Movimenti.Mov_Desc, ISNULL(Movimenti.Data_Movimento, ' ') AS data_movimento, Agenda.Validita_Inizio, ISNULL(Mov_Dettaglio_Tecnico.Qta_Ril, 0) AS qta_ril, ISNULL(Mov_Dettaglio_Tecnico.Dett_Cod, -1) AS udm_cod,  "
        StrSQL += "         ISNULL(UnitaMisura.UDM_DES, ' ') AS udm_des, Campi.Campo_Cod, ISNULL(Campi.Campo_Des, ' ') AS campo_des, ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome, Mov_Destinazioni.Appezza,  "
        StrSQL += "         Mov_Destinazioni.Id_Destinazione, ISNULL(Reg_Impianti.Sup_Imp, 0) AS sup_imp, ISNULL(Reg_Impianti.CUL_COD, - 1) AS cul_cod "
        StrSQL += " FROM    Agenda "
        StrSQL += "         INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda "
        StrSQL += "         INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  "
        StrSQL += "             Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov "
        StrSQL += "         INNER JOIN Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND "
        StrSQL += "             Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND  "
        StrSQL += "             Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det "
        StrSQL += "         LEFT OUTER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  "
        StrSQL += "             Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND "
        StrSQL += "             Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det "
        StrSQL += "         INNER JOIN Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  "
        StrSQL += "             Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg "
        StrSQL += "         INNER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  "
        StrSQL += "             Reg_Impianti.APPEZZA = Appezzamento.APPEZZA "
        StrSQL += "         LEFT OUTER JOIN Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND  "
        StrSQL += "             Appezzamento.Campo_Cod = Campi.Campo_Cod "
        StrSQL += "         INNER JOIN Imprese ON Agenda.PIVA = Imprese.PIVA "
        StrSQL += "         INNER JOIN UnitaMisura ON Mov_Dettaglio_Tecnico.Dett_Cod = UnitaMisura.UDM_COD "
        'WHERE
        StrSQL += " WHERE   Agenda.PIVA = " & Agro_SQL_SaveText(piva) & " "
        StrSQL += " AND     Agenda.SA_COD = " & Agro_SQL_SaveNum(sa_cod) & " "
        StrSQL += " AND     Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & " "
        StrSQL += " AND     Agenda.lav_cod = " & Agro_SQL_SaveNum(lav_cod) & " "
        StrSQL += vbCrLf


        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                             Session("ASG_Connessione_Server"),
                             StrSQL,
                             0,
                             Messaggio)


        If IsNothing(Messaggio) Then

            If (Not IsNothing(Rs)) AndAlso
                (Rs.State <> 0) AndAlso
                    (Not Rs.EOF) Then

                num_record = Rs.RecordCount

                For i = 0 To num_record - 1

                    Try

                        'While Not Rs.EOF

                        'MODIFICA DEL 17/10/2007
                        'LA NET-AGREE VUOLE NEL TAG DEL CODICE APPEZZAMENTO
                        'IL CODICE CAMPO-CODICE VARIETA'
                        'QUINDI SE UN'OPERAZIONE RIGUARDA PIU' IMPIANTI
                        'BISOGNA INSERIRE PIU' BLOCCHI XML

                        '''sup_trattata = sup_trattata + CDbl(Rs.Fields("sup_imp").Value)

                        sup_trattata = CDbl(Rs.Fields("sup_imp").Value)


                        ''''informazioni che mi occorrono una volta sola
                        '''If i = 0 Then

                        '''    If num_record > 1 Then
                        '''        Log += CStr(Date.Now) + "   Impresa: " + piva + ".   L'operazione " + piva + " - " + CStr(sa_cod) + " - " + CStr(id_agenda) + " - " + CStr(lav_cod) + " riguarda più impianti! " & vbCrLf & vbCrLf
                        '''    End If

                        If CStr(Rs.Fields("data_movimento").Value) <> " " Then
                            anno = Left(CStr(Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")), 4)
                            data_movimento = Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")
                        Else
                            anno = "2006"
                            data_movimento = Format(CDate(Date.Today), "yyyy-MM-dd")
                        End If

                        If CStr(Rs.Fields("rag_soc").Value) <> " " Then
                            rag_soc = CStr(Rs.Fields("rag_soc").Value)
                        Else
                            rag_soc = "n.d."
                        End If

                        If CStr(Rs.Fields("campo_des").Value) <> " " Then
                            campo_cod = CStr(CodiceCampoAppezza_from_Nome(CStr(Rs.Fields("campo_des").Value)))
                            campo_des = CStr(Rs.Fields("campo_des").Value)
                        Else
                            If CStr(Rs.Fields("app_nome").Value) <> " " Then
                                campo_cod = CStr(CodiceCampoAppezza_from_Nome(CStr(Rs.Fields("app_nome").Value)))
                                campo_des = CStr(Rs.Fields("app_nome").Value)
                            Else
                                campo_cod = "n.d."
                                campo_des = "n.d."
                            End If
                        End If

                        If CInt(Rs.Fields("cul_cod").Value) <> -1 Then
                            CodiceVarieta = CStr(Rs.Fields("cul_cod").Value)
                        Else
                            CodiceVarieta = CStr("n.d.")
                        End If

                        If CDbl(Rs.Fields("qta_ril").Value) <> 0 Then
                            dose = CDbl(Rs.Fields("qta_ril").Value)
                        End If

                        If CStr(Rs.Fields("udm_des").Value) <> " " Then
                            udm_des = CStr(Rs.Fields("udm_des").Value)
                        Else
                            udm_des = ""
                        End If

                        udm_cod = CInt(Rs.Fields("udm_cod").Value)

                        If CStr(Rs.Fields("des_lib").Value) <> " " Then
                            note = CStr(Rs.Fields("des_lib").Value)
                        Else
                            note = ""
                        End If

                        '''End If


                        'CREO IL P99M100004
                        XmlPadre = XmlDocumento.CreateElement("P" + CStr(Punto) + "M100004")

                        '---------------

                        'descrizione
                        Node = XmlDocumento.CreateElement("Descrizione")
                        Select Case lav_cod
                            Case LAVCOD_IRRIGAZIONE '1 LAVCOD_IRRIGAZIONE
                                Node.InnerText = "Irrigazione " + anno
                            Case Is = LAVCOD_RILIEVO_PIOGGE '126 LAVCOD_RILIEVO_PIOGGE
                                Node.InnerText = "Rilievo Piogge " + anno
                        End Select
                        XmlPadre.AppendChild(Node)

                        '---------------

                        'data riferimento
                        Node = XmlDocumento.CreateElement("DataRiferimento")
                        Node.InnerText = data_movimento
                        XmlPadre.AppendChild(Node)

                        '----------

                        'partita iva
                        Node = XmlDocumento.CreateElement("PM100087")
                        Node.InnerText = piva
                        XmlPadre.AppendChild(Node)

                        '-------

                        'ragione sociale
                        Node = XmlDocumento.CreateElement("PM100088")
                        Node.InnerText = rag_soc
                        XmlPadre.AppendChild(Node)

                        '--------

                        'codice campo
                        Node = XmlDocumento.CreateElement("PM100089")
                        Node.InnerText = CStr(CInt(campo_cod)) + "__" + CodiceVarieta
                        XmlPadre.AppendChild(Node)

                        '--------

                        'descr campo
                        Node = XmlDocumento.CreateElement("PM100090")
                        Node.InnerText = campo_des
                        XmlPadre.AppendChild(Node)

                        '------------------

                        'tipo operazione
                        Node = XmlDocumento.CreateElement("PM100091")
                        Select Case lav_cod
                            Case LAVCOD_IRRIGAZIONE '1  LAVCOD_IRRIGAZIONE
                                Node.InnerText = "Irrigazione"
                            Case Is = LAVCOD_RILIEVO_PIOGGE ' 126 LAVCOD_RILIEVO_PIOGGE
                                Node.InnerText = "Pioggia"
                        End Select
                        XmlPadre.AppendChild(Node)

                        '------------

                        'data operazione
                        Node = XmlDocumento.CreateElement("PM100092")
                        Node.InnerText = data_movimento
                        XmlPadre.AppendChild(Node)

                        '------------

                        'annata operazione
                        Node = XmlDocumento.CreateElement("PM100093")
                        Node.InnerText = CInt(anno)
                        XmlPadre.AppendChild(Node)


                        '==============================================================
                        'dose

                        If dose <> 0 Then

                            Node = XmlDocumento.CreateElement("PM100094")

                            Select Case udm_cod

                                Case 18 'millimetri

                                    Node.InnerText = dose

                                Case 90 'metricubi/ha

                                    'dose in mm / 1000 * 10000 = dose metricubi/ha
                                    'visto che ho i metricubi/ha ricavo la dose in mm

                                    Node.InnerText = dose / 10 ' ovvero: / 10000 * 1000

                            End Select

                            XmlPadre.AppendChild(Node)

                        End If

                        '==============================================================

                        If sup_trattata <> 0 Then
                            Node = XmlDocumento.CreateElement("PM100095")
                            Node.InnerText = sup_trattata
                            XmlPadre.AppendChild(Node)
                        End If


                        '=================================================================

                        'note

                        If note <> "" Then
                            Node = XmlDocumento.CreateElement("PM100096")
                            Node.InnerText = note
                            XmlPadre.AppendChild(Node)
                        End If

                        ''Select Case udm_cod

                        ''    Case 18 'millimetri

                        ''        If note <> "" Then
                        ''            Node = XmlDocumento.CreateElement("PM100096")
                        ''            Node.InnerText = note
                        ''            XmlPadre.AppendChild(Node)
                        ''        End If

                        ''    Case 19 'metricubi

                        ''        If note <> "" Then
                        ''            Node = XmlDocumento.CreateElement("PM100096")
                        ''            Node.InnerText = note + " (unità di misura dose acqua: metri cubi)"
                        ''            XmlPadre.AppendChild(Node)
                        ''        End If


                        ''End Select


                        '--------------------------------------------------------------


                        'APPENDO ALLA RADICE
                        XmlRoot.AppendChild(XmlPadre)

                    Catch ex As Exception

                        Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                        " -   Errore durante la generazione del file xml Piogge Irrigazioni: " + ex.Message & vbCrLf & vbCrLf

                    End Try


                    Rs.MoveNext()

                    'End While

                Next

                Rs.Close()


                'SPOSTATO DENTRO AL CICLO

                ''''CREO IL P99M100004
                '''XmlPadre = XmlDocumento.CreateElement("P" + CStr(Punto) + "M100004")

                ''''---------------

                ''''descrizione
                '''Node = XmlDocumento.CreateElement("Descrizione")
                '''Select Case lav_cod
                '''    Case 1
                '''        Node.InnerText = "Irrigazione " + anno
                '''    Case Is = 126
                '''        Node.InnerText = "Rilievo Piogge " + anno
                '''End Select
                '''XmlPadre.AppendChild(Node)

                ''''---------------

                ''''data riferimento
                '''Node = XmlDocumento.CreateElement("DataRiferimento")
                '''Node.InnerText = data_movimento
                '''XmlPadre.AppendChild(Node)

                ''''----------

                ''''partita iva
                '''Node = XmlDocumento.CreateElement("PM100087")
                '''Node.InnerText = piva
                '''XmlPadre.AppendChild(Node)

                ''''-------

                ''''ragione sociale
                '''Node = XmlDocumento.CreateElement("PM100088")
                '''Node.InnerText = rag_soc
                '''XmlPadre.AppendChild(Node)

                ''''--------

                ''''codice campo
                '''Node = XmlDocumento.CreateElement("PM100089")
                '''Node.InnerText = campo_cod + "-" + CodiceVarieta
                '''XmlPadre.AppendChild(Node)

                ''''--------

                ''''descr campo
                '''Node = XmlDocumento.CreateElement("PM100090")
                '''Node.InnerText = campo_des
                '''XmlPadre.AppendChild(Node)

                ''''------------------

                ''''tipo operazione
                '''Node = XmlDocumento.CreateElement("PM100091")
                '''Select Case lav_cod
                '''    Case 1
                '''        Node.InnerText = "Irrigazione"
                '''    Case Is = 126
                '''        Node.InnerText = "Pioggia"
                '''End Select
                '''XmlPadre.AppendChild(Node)

                ''''------------

                ''''data operazione
                '''Node = XmlDocumento.CreateElement("PM100092")
                '''Node.InnerText = data_movimento
                '''XmlPadre.AppendChild(Node)

                ''''------------

                ''''annata operazione
                '''Node = XmlDocumento.CreateElement("PM100093")
                '''Node.InnerText = CInt(anno)
                '''XmlPadre.AppendChild(Node)


                ''''==============================================================

                '''If dose <> 0 Then
                '''    Node = XmlDocumento.CreateElement("PM100094")
                '''    Node.InnerText = dose
                '''    XmlPadre.AppendChild(Node)
                '''End If

                '''If sup_trattata <> 0 Then
                '''    Node = XmlDocumento.CreateElement("PM100095")
                '''    Node.InnerText = sup_trattata
                '''    XmlPadre.AppendChild(Node)
                '''End If


                ''''=================================================================

                ''''note
                '''Select Case udm_cod

                '''    Case 18

                '''        If note <> "" Then
                '''            Node = XmlDocumento.CreateElement("PM100096")
                '''            Node.InnerText = note
                '''            XmlPadre.AppendChild(Node)
                '''        End If

                '''    Case 19

                '''        If note <> "" Then
                '''            Node = XmlDocumento.CreateElement("PM100096")
                '''            Node.InnerText = note + " (unità di misura dose acqua: metri cubi)"
                '''            XmlPadre.AppendChild(Node)
                '''        End If


                '''End Select


                ''''--------------------------------------------------------------


                ''''APPENDO ALLA RADICE
                '''XmlRoot.AppendChild(XmlPadre)



            Else 'nothing rs - state - eof

                Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative al centro: " + piva + " - " + CStr(sa_cod) + ", all'id_agenda: " + CStr(id_agenda) + " e al lav_cod: " + CStr(lav_cod) + ". " & vbCrLf & vbCrLf

            End If 'nothing

        Else 'rs - state - eof

            Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Errore nella QUERY RINTRACCIO Piogge/Irrigazioni: " + Messaggio & vbCrLf & vbCrLf

        End If 'messaggio



        '------------------------------------------------
        '------------ FINE CICLO RINTRACCIO -------------
        '------------------------------------------------

        flag_pioirr = True

        XmlRoot_send = XmlRoot
        XmlDocumento_send = XmlDocumento



    End Sub




    '#############################################################################################################
    Private Sub Rintraccio_Operazioni_Colturali(ByRef Log As String, ByVal Punto As Integer, ByVal piva As String, ByVal sa_cod As Integer, ByVal id_agenda As Integer, ByVal lav_cod As Integer, ByRef flag_opcolt As Boolean, ByRef XmlRoot_send As XmlElement, ByRef XmlDocumento_send As XmlDocument)

        Dim XmlDocumento As XmlDocument
        Dim XmlRoot, XmlPadre As XmlElement
        Dim Node, NodeRiga As XmlElement
        Dim xPI As XmlProcessingInstruction

        Dim num_record, i As Integer

        Dim sup_trattata As Double = 0
        Dim CodiceVarieta As String = ""

        Dim Rs As ADODB.Recordset
        Dim objSQL As New Codex_Utility.Sql
        ' Dim objSQL As New Codex_Utility.Sql

        Dim Messaggio, StrSQL, anno, rag_soc, note, campo_des, campo_cod, lav_des, gru_des As String
        Dim data_movimento As String

        '------------------------------------------------

        If flag_opcolt = False Then

            XmlDocumento = New XmlDocument

            'xPI = XmlDocumento.CreateProcessingInstruction("xml", "version=""1.0"" encoding=""UTF-8""")
            'XmlDocumento.AppendChild(xPI)

            ' Create an XML declaration. 
            Dim xmldecl As XmlDeclaration
            xmldecl = XmlDocumento.CreateXmlDeclaration("1.0", Nothing, Nothing)
            xmldecl.Encoding = "UTF-8"
            'xmldecl.Standalone = "yes"
            XmlDocumento.AppendChild(xmldecl)

            XmlRoot = XmlDocumento.CreateElement("dataroot")

        Else

            XmlRoot = XmlRoot_send
            XmlDocumento = XmlDocumento_send

        End If


        '----------------------------------------------------------
        ' VAI CON LA QUERY RINTRACCIO Operazioni Colturali!!!!!!!!
        '----------------------------------------------------------

        StrSQL = ""
        StrSQL = "  SELECT  Agenda.PIVA, ISNULL(Imprese.rag_soc, ' ') AS rag_soc, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, ISNULL(Operazioni.LAV_DES, ' ') AS lav_des, Operazioni.GRU_OP, ISNULL(GruppoOperazioni.GRU_DES, ' ') AS gru_des, "
        StrSQL += "         ISNULL(Agenda.des_lib, ' ') AS des_lib, Movimenti.Id_Mov, Movimenti.Cau_Mov,  "
        StrSQL += "         Movimenti.Mov_Desc, ISNULL(Movimenti.Data_Movimento, ' ') AS data_movimento, Agenda.Validita_Inizio, "
        StrSQL += "         Campi.Campo_Cod, ISNULL(Campi.Campo_Des, ' ') AS campo_des, ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome, Mov_Destinazioni.Appezza,  "
        StrSQL += "         Mov_Destinazioni.Id_Destinazione, ISNULL(Reg_Impianti.Sup_Imp, 0) AS sup_imp, ISNULL(Reg_Impianti.CUL_COD, - 1) AS cul_cod "
        StrSQL += " FROM    Agenda "
        StrSQL += "         INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda "
        StrSQL += "         INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  "
        StrSQL += "             Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov "
        StrSQL += "         LEFT OUTER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  "
        StrSQL += "             Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND "
        StrSQL += "             Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det "
        StrSQL += "         INNER JOIN Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND  "
        StrSQL += "             Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg "
        StrSQL += "         INNER JOIN  Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  "
        StrSQL += "             Reg_Impianti.APPEZZA = Appezzamento.APPEZZA "
        StrSQL += "         LEFT OUTER JOIN Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND  "
        StrSQL += "             Appezzamento.Campo_Cod = Campi.Campo_Cod "
        StrSQL += "         INNER JOIN Imprese ON Agenda.PIVA = Imprese.PIVA "
        StrSQL += "         INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD "
        StrSQL += "         LEFT OUTER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD "
        'WHERE
        StrSQL += " WHERE   Agenda.PIVA = " & Agro_SQL_SaveText(piva) & " "
        StrSQL += " AND     Agenda.SA_COD = " & Agro_SQL_SaveNum(sa_cod) & " "
        StrSQL += " AND     Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & " "
        StrSQL += " AND     Agenda.lav_cod = " & Agro_SQL_SaveNum(lav_cod) & " "
        StrSQL += vbCrLf


        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                             Session("ASG_Connessione_Server"),
                             StrSQL,
                             0,
                             Messaggio)


        If IsNothing(Messaggio) Then

            If (Not IsNothing(Rs)) AndAlso
                (Rs.State <> 0) AndAlso
                    (Not Rs.EOF) Then

                num_record = Rs.RecordCount

                For i = 0 To num_record - 1

                    Try

                        'MODIFICA DEL 17/10/2007
                        'NET-AGREE VUOLE NEL TAG DEL CODICE APPEZZAMENTO
                        'IL CODICE CAMPO-CODICE VARIETA'
                        'QUINDI SE UN'OPERAZIONE RIGUARDA PIU' IMPIANTI
                        'BISOGNA INSERIRE PIU' BLOCCHI XML

                        '''sup_trattata = sup_trattata + CDbl(Rs.Fields("sup_imp").Value)

                        sup_trattata = CDbl(Rs.Fields("sup_imp").Value)


                        ''''informazioni che mi occorrono una volta sola
                        '''If i = 0 Then

                        '''    If num_record > 1 Then
                        '''        Log += CStr(Date.Now) + "   Impresa: " + piva + ".   L'operazione " + piva + " - " + CStr(sa_cod) + " - " + CStr(id_agenda) + " - " + CStr(lav_cod) + " riguarda più impianti! " & vbCrLf & vbCrLf
                        '''    End If

                        If CStr(Rs.Fields("data_movimento").Value) <> " " Then
                            anno = Left(CStr(Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")), 4)
                            data_movimento = Format(CDate(Rs.Fields("data_movimento").Value), "yyyy-MM-dd")
                        Else
                            anno = "2006"
                            data_movimento = Format(CDate(Date.Today), "yyyy-MM-dd")
                        End If

                        If CStr(Rs.Fields("rag_soc").Value) <> " " Then
                            rag_soc = CStr(Rs.Fields("rag_soc").Value)
                        Else
                            rag_soc = "n.d."
                        End If

                        If CStr(Rs.Fields("campo_des").Value) <> " " Then
                            campo_cod = CStr(CodiceCampoAppezza_from_Nome(CStr(Rs.Fields("campo_des").Value)))
                            campo_des = CStr(Rs.Fields("campo_des").Value)
                        Else
                            If CStr(Rs.Fields("app_nome").Value) <> " " Then
                                campo_cod = CStr(CodiceCampoAppezza_from_Nome(CStr(Rs.Fields("app_nome").Value)))
                                campo_des = CStr(Rs.Fields("app_nome").Value)
                            Else
                                campo_cod = "n.d."
                                campo_des = "n.d."
                            End If
                        End If

                        If CInt(Rs.Fields("cul_cod").Value) <> -1 Then
                            CodiceVarieta = CStr(Rs.Fields("cul_cod").Value)
                        Else
                            CodiceVarieta = CStr("n.d.")
                        End If

                        If CStr(Rs.Fields("lav_des").Value) <> " " Then
                            lav_des = CStr(Rs.Fields("lav_des").Value)
                        Else
                            lav_des = ""
                        End If

                        If CStr(Rs.Fields("gru_des").Value) <> " " Then
                            gru_des = CStr(Rs.Fields("gru_des").Value)
                        Else
                            gru_des = ""
                        End If

                        If CStr(Rs.Fields("des_lib").Value) <> " " Then
                            note = CStr(Rs.Fields("des_lib").Value)
                        Else
                            note = ""
                        End If

                        '''End If



                        'CREO IL P99M100005
                        XmlPadre = XmlDocumento.CreateElement("P" + CStr(Punto) + "M100005")

                        '---------------

                        'descrizione
                        Node = XmlDocumento.CreateElement("Descrizione")
                        Node.InnerText = lav_des + " - " + anno
                        XmlPadre.AppendChild(Node)

                        '---------------

                        'data riferimento
                        Node = XmlDocumento.CreateElement("DataRiferimento")
                        Node.InnerText = data_movimento
                        XmlPadre.AppendChild(Node)

                        '----------

                        'partita iva
                        Node = XmlDocumento.CreateElement("PM100097")
                        Node.InnerText = piva
                        XmlPadre.AppendChild(Node)

                        '-------

                        'ragione sociale
                        Node = XmlDocumento.CreateElement("PM100098")
                        Node.InnerText = rag_soc
                        XmlPadre.AppendChild(Node)

                        '--------

                        'codice campo
                        Node = XmlDocumento.CreateElement("PM100099")
                        Node.InnerText = CStr(CInt(campo_cod)) + "__" + CodiceVarieta
                        XmlPadre.AppendChild(Node)

                        '--------

                        'descr campo
                        Node = XmlDocumento.CreateElement("PM100100")
                        Node.InnerText = campo_des
                        XmlPadre.AppendChild(Node)

                        '------------

                        'data operazione
                        Node = XmlDocumento.CreateElement("PM100101")
                        Node.InnerText = data_movimento
                        XmlPadre.AppendChild(Node)

                        '------------

                        'annata operazione
                        Node = XmlDocumento.CreateElement("PM100102")
                        Node.InnerText = CInt(anno)
                        XmlPadre.AppendChild(Node)

                        '------------------

                        'tipo operazione
                        Node = XmlDocumento.CreateElement("PM100103")
                        Node.InnerText = lav_des
                        XmlPadre.AppendChild(Node)


                        '==============================================================

                        If sup_trattata <> 0 Then
                            Node = XmlDocumento.CreateElement("PM100104")
                            Node.InnerText = sup_trattata
                            XmlPadre.AppendChild(Node)
                        End If


                        '=================================================================

                        'note
                        If note <> "" Then
                            Node = XmlDocumento.CreateElement("PM100105")
                            Node.InnerText = note + " - [" + gru_des + "]"
                            XmlPadre.AppendChild(Node)
                        End If


                        '--------------------------------------------------------------


                        'APPENDO ALLA RADICE
                        XmlRoot.AppendChild(XmlPadre)



                    Catch ex As Exception

                        Log += CStr(Date.Now) + "   Impresa: " + piva + " Centro: " + sa_cod + " Gruppo Operazione: " + lav_cod + " Operazione: " + id_agenda +
                        " -   Errore durante la generazione del file xml Operazioni Colturali: " + ex.Message & vbCrLf & vbCrLf

                    End Try


                    Rs.MoveNext()

                Next

                Rs.Close()

                'SPOSTATO DENTRO AL CICLO

                ''''CREO IL P99M100005
                '''XmlPadre = XmlDocumento.CreateElement("P" + CStr(Punto) + "M100005")

                ''''---------------

                ''''descrizione
                '''Node = XmlDocumento.CreateElement("Descrizione")
                '''Node.InnerText = lav_des + " - " + anno
                '''XmlPadre.AppendChild(Node)

                ''''---------------

                ''''data riferimento
                '''Node = XmlDocumento.CreateElement("DataRiferimento")
                '''Node.InnerText = data_movimento
                '''XmlPadre.AppendChild(Node)

                ''''----------

                ''''partita iva
                '''Node = XmlDocumento.CreateElement("PM100097")
                '''Node.InnerText = piva
                '''XmlPadre.AppendChild(Node)

                ''''-------

                ''''ragione sociale
                '''Node = XmlDocumento.CreateElement("PM100098")
                '''Node.InnerText = rag_soc
                '''XmlPadre.AppendChild(Node)

                ''''--------

                ''''codice campo
                '''Node = XmlDocumento.CreateElement("PM100099")
                '''Node.InnerText = campo_cod + "-" + CodiceVarieta
                '''XmlPadre.AppendChild(Node)

                ''''--------

                ''''descr campo
                '''Node = XmlDocumento.CreateElement("PM100100")
                '''Node.InnerText = campo_des
                '''XmlPadre.AppendChild(Node)

                ''''------------

                ''''data operazione
                '''Node = XmlDocumento.CreateElement("PM100101")
                '''Node.InnerText = data_movimento
                '''XmlPadre.AppendChild(Node)

                ''''------------

                ''''annata operazione
                '''Node = XmlDocumento.CreateElement("PM100102")
                '''Node.InnerText = CInt(anno)
                '''XmlPadre.AppendChild(Node)

                ''''------------------

                ''''tipo operazione
                '''Node = XmlDocumento.CreateElement("PM100103")
                '''Node.InnerText = lav_des
                '''XmlPadre.AppendChild(Node)


                ''''==============================================================

                '''If sup_trattata <> 0 Then
                '''    Node = XmlDocumento.CreateElement("PM100104")
                '''    Node.InnerText = sup_trattata
                '''    XmlPadre.AppendChild(Node)
                '''End If


                ''''=================================================================

                ''''note
                '''If note <> "" Then
                '''    Node = XmlDocumento.CreateElement("PM100105")
                '''    Node.InnerText = note + " - [" + gru_des + "]"
                '''    XmlPadre.AppendChild(Node)
                '''End If


                ''''--------------------------------------------------------------


                ''''APPENDO ALLA RADICE
                '''XmlRoot.AppendChild(XmlPadre)



            Else 'nothing rs - state - eof

                Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Non sono state trovate informazioni relative al centro: " + piva + " - " + CStr(sa_cod) + ", all'id_agenda: " + CStr(id_agenda) + " e al lav_cod: " + CStr(lav_cod) + ". " & vbCrLf & vbCrLf

            End If 'nothing

        Else 'rs - state - eof

            Log += CStr(Date.Now) + "   Impresa: " + piva + ".   Errore nella QUERY RINTRACCIO Operazioni Colturali: " + Messaggio & vbCrLf & vbCrLf

        End If 'messaggio



        '------------------------------------------------
        '------------ FINE CICLO RINTRACCIO -------------
        '------------------------------------------------

        flag_opcolt = True

        XmlRoot_send = XmlRoot
        XmlDocumento_send = XmlDocumento



    End Sub






    '###############################################################################
    Private Function CodiceCampoAppezza_from_Nome(ByVal nome As String) As String

        nome = nome.Replace("2000", "")
        nome = nome.Replace("2001", "")
        nome = nome.Replace("2002", "")
        nome = nome.Replace("2003", "")
        nome = nome.Replace("2004", "")
        nome = nome.Replace("2005", "")
        nome = nome.Replace("2006", "")
        nome = nome.Replace("2007", "")
        nome = nome.Replace("2008", "")
        nome = nome.Replace("2009", "")

        Dim i As Integer
        Dim Array() As Char
        Dim Numero As String

        Array = nome.ToCharArray()

        For i = 0 To Array.Length - 1
            If IsNumeric(Array(i)) Then
                Numero += Array(i)
            End If
        Next

        Return Numero


    End Function




    '#################################################################
    Private Sub SmartBuild_Crea()

        Dim sum_gruppo1 As Integer = 0
        Dim sum_gruppo2 As Integer = 0
        Dim sum_gruppo3 As Integer = 0
        Dim codice, fin As String
        Dim i As Integer

        For i = 0 To 9
            If Me.ChkGruppo1.Items.FindByValue(i).Selected = True Then
                sum_gruppo1 = sum_gruppo1 + (2 ^ i)
            End If
        Next

        For i = 10 To 19
            If Me.ChkGruppo2.Items.FindByValue(i).Selected = True Then
                sum_gruppo2 = sum_gruppo2 + (2 ^ (i - 10))
            End If
        Next

        For i = 20 To 29
            If Me.ChkGruppo3.Items.FindByValue(i).Selected = True Then
                sum_gruppo3 = sum_gruppo3 + (2 ^ (i - 20))
            End If
        Next

        Select Case ViewState("scheda")
            Case "imprese"
                codice = "A_"
            Case "centri"
                codice = "B_"
            Case "impianti"
                codice = "D_"
            Case "agenda"
                codice = "E_"
                'Case "rintraccio"
                '    codice = "F_"
        End Select


        Select Case Me.RdBList_Esporta.SelectedValue
            Case "A"
                fin = "A"
            Case "B"
                fin = "B"
            Case "C"
                fin = "C"
            Case "D"
                fin = "D"
            Case "E"
                fin = "E"
                'Case "F"
                '    fin = "F"
        End Select

        codice = codice & Hex(sum_gruppo1) & "." & Hex(sum_gruppo2) & "." & Hex(sum_gruppo3) & "_" & fin

        Me.Txt_Esadecimale.Text = codice

    End Sub



    '###############################################################################################
    Private Sub Btn_Imposta_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Imposta.Click

        SmartBuild_Imposta()

    End Sub


    '################################################################################################
    Private Sub SmartBuild_Imposta()

        If Me.Txt_Esadecimale.Text = "" Then
            AgroMsgBox("Inserire la Stringa SmartBuild da impostare!", Page)
            Exit Sub
        End If

        Dim codice As String
        codice = Me.Txt_Esadecimale.Text

        Dim vet(2) As String
        vet = Split(codice, "_")

        If vet(0) <> Session("scheda") Then
            AgroMsgBox("Hai cercato di caricare la stringa SmartBuild di un'altra scheda!!!" & vbCrLf & "Inserire la stringa corrispondente alla scheda scelta.", Page)
            Exit Sub
        End If

        Select Case vet(2)

            Case "A"
                Me.RdBList_Esporta.Items(0).Selected = True
                Me.Txt_FileOutput.Enabled = False
                Me.Txt_FileOutput.BackColor = Drawing.Color.Gray

            Case "B"
                Me.RdBList_Esporta.Items(1).Selected = True
                Me.Txt_FileOutput.Enabled = True
                Me.Txt_FileOutput.BackColor = Drawing.Color.White

            Case "C"
                Me.RdBList_Esporta.Items(2).Selected = True
                Me.Txt_FileOutput.Enabled = True
                Me.Txt_FileOutput.BackColor = Drawing.Color.White

            Case "D"
                Me.RdBList_Esporta.Items(3).Selected = True
                Me.Txt_FileOutput.Enabled = True
                Me.Txt_FileOutput.BackColor = Drawing.Color.White

            Case "E"
                Me.RdBList_Esporta.Items(4).Selected = True
                Me.Txt_FileOutput.Enabled = True
                Me.Txt_FileOutput.BackColor = Drawing.Color.White

        End Select

        Dim gruppi(2) As String
        gruppi = Split(vet(1), ".")

        Dim dec1, dec2, dec3, i As Integer
        Dim bin1, bin2, bin3 As String
        dec1 = Val("&H" & gruppi(0))
        dec2 = Val("&H" & gruppi(1))
        dec3 = Val("&H" & gruppi(2))

        bin1 = BinarioFromDecimale(dec1)
        bin2 = BinarioFromDecimale(dec2)
        bin3 = BinarioFromDecimale(dec3)

        If bin1.Length <> 10 Then
            bin1 = bin1.PadLeft(10, "0")
        End If

        If bin2.Length <> 10 Then
            bin2 = bin2.PadLeft(10, "0")
        End If

        If Session("scheda") <> "A" Then
            If bin3.Length <> 10 Then
                bin3 = bin3.PadLeft(10, "0")
            End If
        End If

        For i = 0 To 9

            If bin1.Chars(i) = "1" Then
                Me.ChkGruppo1.Items(9 - i).Selected = True
            End If

            If bin2.Chars(i) = "1" Then
                Me.ChkGruppo2.Items(9 - i).Selected = True
            End If

        Next

        If Session("scheda") <> "A" Then
            For i = 0 To 9
                If bin3.Chars(i) = "1" Then
                    Me.ChkGruppo3.Items(9 - i).Selected = True
                End If
            Next
        End If

        Me.Txt_Esadecimale.Text = ""

    End Sub



    '#########################################################################################
    Private Sub Esporta_Excel(ByVal NomeFileOutput As String, ByVal DT_Finale As DataTable)

        Session("DT_Finale") = DT_Finale

        Dim scheda As String
        scheda = Stringa_Codifica(ViewState("scheda"),
                            AgroKey_EncoderDecoder,
                             Server)

        NomeFileOutput = Stringa_Codifica(NomeFileOutput,
                            AgroKey_EncoderDecoder,
                             Server)

        Dim strOpen As String = "<script language='javascript'>" & vbNewLine &
                "window.open('RisultatoEsportazione_XLS.aspx?a=" & NomeFileOutput & "&t=" & scheda & "'," &
                "'Esportazioni','height=600,width=1000,menubar=yes,scrollbars=yes,top=0,left=0,resizable=yes');" & vbNewLine &
                "</script>"
        'apro la finestra...
        Page.RegisterClientScriptBlock("key", strOpen)
        'Me.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))

    End Sub



    '###########################################################################################
    Private Sub Esporta_XML(ByVal NomeFileOutput As String, ByVal DT_Finale As DataTable, scheda As String)


        Select Case scheda
            Case "imprese"
                Dim ser As XmlSerializer = New XmlSerializer(GetType(ImpreseList))
                Dim objXMLImprese As New ImpreseList
                Dim impreseList As New List(Of Impresa)
                Dim PIVA As String
                For Each rows In DT_Finale.Rows
                    PIVA = rows.item("piva")
                    Dim impresa As Impresa
                    If (impreseList.Where(Function(c) (c.PIVA = PIVA)).ToList.Count > 0) Then
                        impresa = impreseList.Where(Function(c) (c.PIVA = PIVA)).ToList.First
                    Else
                        impresa = New Impresa
                        impresa.PIVA = PIVA
                        If ChkGruppo1.Items.FindByText("Ragione Sociale").Selected Then
                            impresa.Ragione_Sociale = rows.item("rag_soc")
                        End If
                        If ChkGruppo1.Items.FindByText("Tipo Impresa").Selected Then
                            impresa.Tipo_Impresa = rows.item("tipo_impresa")
                        End If
                        If ChkGruppo1.Items.FindByText("Codice Socio").Selected Then
                            impresa.Codice_Socio = rows.item("Codice_Socio")
                        End If
                        If ChkGruppo1.Items.FindByText("CUAA").Selected Then
                            impresa.CUAA = rows.item("CUAA")
                        End If
                        If ChkGruppo1.Items.FindByText("Cooperativa Padre").Selected Then
                            impresa.Impresa_Padre = rows.item("Coop_Padre")
                        End If
                        If ChkGruppo1.Items.FindByText("Partita IVA Coop. Padre").Selected Then
                            impresa.PIVA_Padre = rows.item("piva_Padre")
                        End If
                        If ChkGruppo1.Items.FindByText("Indirizzo").Selected Then
                            Dim indi = New Indirizzo
                            indi.CAP = rows.item("CAP")
                            indi.Indirizzo_Des = rows.item("ind_Des")
                            indi.Comune_Des = rows.item("com_Des")
                            indi.Frazione_Des = rows.item("frz_des")
                            indi.Provincia_Cod = rows.item("pro_Cod")
                            impresa.Indirizzo = indi
                        End If
                        If ChkGruppo1.Items.FindByText("Codici ISTAT Comune e Provincia").Selected Then
                            impresa.Istat_Provincia = rows.item("pro_cod_istat")
                            impresa.Istat_Comune = rows.item("com_cod_istat")
                        End If
                        If ChkGruppo1.Items.FindByText("Data Inizio / Data Fine Impresa").Selected Then
                            If (CDate(rows.item("inizio_impresa")) <> AGRODATAINIZIO) Then
                                impresa.Data_Inizio = CDate(rows.item("inizio_impresa"))
                            End If
                            If (CDate(rows.item("fine_impresa")) <> AGRODATAFINE) Then
                                impresa.Data_Fine = CDate(rows.item("fine_impresa"))
                            End If
                        End If
                        Dim rapp_leg As New Rappresentante_Legale
                        If ChkGruppo2.Items.FindByText("Codice Fiscale Legale Rappresentante").Selected Then
                            rapp_leg.Codice_Fiscale = rows.item("CF_legale")
                        End If
                        If ChkGruppo2.Items.FindByText("Nome e Cognome Legale Rappresentante").Selected Then
                            rapp_leg.Cognome_Nome = rows.item("rappr_legale")
                        End If
                        If ChkGruppo2.Items.FindByText("Provincia e Comune Nascita Legale Rappr.").Selected Then
                            If (rows.item("com_legale") <> "Non Definita") Then
                                rapp_leg.Comune_Nascita = rows.item("com_legale")
                            End If
                            If (rows.item("pro_legale") <> "00") Then
                                rapp_leg.Provincia_Nascita = rows.item("pro_legale")
                            End If
                        End If
                        impresa.Rappresentante_Legale = rapp_leg
                        impresa.Particelle = New List(Of Particella)
                        impreseList.Add(impresa)
                    End If

                    If ChkGruppo2.Items.FindByText("Particelle").Selected Then
                        Dim particella As New Particella
                        If rows.item("prov").ToString.Trim <> "" Then
                            particella.Provincia = rows.item("prov")
                            particella.Comune = rows.item("com")
                            particella.Sezione = rows.item("sezione")
                            particella.Foglio = rows.item("foglio")
                            particella.Subalterno = rows.item("subalterno")
                            particella.Numero = rows.item("numero")
                            If ChkGruppo2.Items.FindByText("Titolo Possesso Particella").Selected Then
                                particella.Titolo_Possesso = rows.item("TitoloPossesso")
                            End If
                            If ChkGruppo2.Items.FindByText("Data Inizio / Data Fine Possesso").Selected Then
                                If (CDate(rows.item("dal")) <> AGRODATAINIZIO) Then
                                    particella.Data_Inizio_Possesso = CDate(rows.item("dal"))
                                End If
                                If (CDate(rows.item("al")) <> AGRODATAFINE) Then
                                    particella.Data_Fine_Possesso = CDate(rows.item("al"))
                                End If
                            End If
                            If ChkGruppo2.Items.FindByText("Superficie Particella").Selected Then
                                particella.Superficie_HA = rows.item("ETTARI")
                                particella.Superficie_AA = rows.item("ARE")
                                particella.Superficie_CA = rows.item("CENTIARE")
                                particella.Superficie = Ettari_from_EttariAreCentiare(particella.Superficie_HA,
                                                                                      particella.Superficie_AA,
                                                                                      particella.Superficie_CA)
                            End If
                            impresa.Particelle.Add(particella)
                        End If
                    End If

                Next
                objXMLImprese.imprese = impreseList
                Dim filePath As String
                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                filePath = objConfSiti.Leggi_Valore(6, "GestioneEsportazioni_Repository", "", "", objParametri_Server) + "/ExportXMLImprese_" +
                            DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + DateTime.Now.Hour.ToString("D2") +
                            DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2") + ".xml"

                Dim writer As TextWriter = New StreamWriter(filePath)
                ser.Serialize(writer, objXMLImprese)
                writer.Close()
            Case "impianti"
                Dim ser As XmlSerializer = New XmlSerializer(GetType(ImpiantiList))
                Dim objXMLImpianti As New ImpiantiList
                Dim impiantiList As New List(Of Impianto)
                Dim objOperazioni As New AgronicaCoreMetaSchemaDAL.Operazioni_R
                Dim dt_Op As DataTable
                Dim lav_Cod As Integer
                Dim operazione As String
                Dim PIVA As String
                Dim catasto As Boolean = Me.ChkGruppo3.Items.FindByValue(I_Particelle).Selected
                For Each rows In DT_Finale.Rows
                    Dim imp As New Impianto
                    imp.id_Impianto = rows.item("id_Impianto")
                    imp.PIVA = rows.item("Piva")
                    imp.Ragione_Sociale = rows.item("rag_soc")
                    imp.Codice_Socio = rows.item("CodiceSocio")
                    imp.CUAA = rows.item("CUAA")
                    imp.PIVA_Padre = rows.item("Piva_padre")
                    imp.Impresa_Padre = rows.item("coop_padre")
                    imp.CentroAziendale_id = rows.item("sa_cod")
                    imp.CentroAziendale_Nome = rows.item("sa_nome")
                    imp.DataInizioImpianto = rows.item("inizio_impianto")
                    imp.DataFineImpianto = rows.item("fine_impianto")
                    imp.Superficie = rows.item("sup_imp")
                    imp.DataSeminaTrapianto = rows.item("data_Semina")
                    imp.Veg_Cod = rows.item("veg_cod")
                    imp.Veg_Des = rows.item("veg_des")
                    imp.Cul_Cod = rows.item("cul_cod")
                    imp.Cul_Des = rows.item("cul_des")
                    If Not IsDBNull(rows.item("MetodoProduzione_Cod")) Then
                        imp.MetodoProduzione = rows.item("MetodoProduzione_Cod")
                    Else
                        imp.MetodoProduzione = 0
                    End If
                    If Not IsDBNull(rows.item("lav_cod_imp")) Then
                        lav_Cod = rows.item("lav_cod_imp")
                        dt_Op = objOperazioni.Leggi(lav_Cod, 0, 0, "", 0, "", "", True, True, True, True, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                        If dt_Op IsNot Nothing AndAlso dt_Op.Rows IsNot Nothing AndAlso dt_Op.Rows.Count > 0 Then
                            imp.Operazione = dt_Op.Rows(0).Item("lav_des")
                        End If
                    End If
                    imp.Portainnesto = rows.item("port_des")
                    imp.Tipo_Copertura = rows.item("cop_des")
                    imp.Nr_Piante = rows.item("p_tot")
                    imp.GuppoVarietale = rows.item("grva_des")

                    If catasto Then
                        Dim cat As New ImpiantoCatasto
                        cat.Com = rows.item("Com")
                        cat.Prov = rows.item("Prov")
                        cat.Sezione = rows.item("Sezione")
                        cat.Subalterno = rows.item("Subalterno")
                        cat.Foglio = rows.item("Foglio")
                        cat.Numero = rows.item("Numero")
                        imp.Catasto = cat
                    End If
                    impiantiList.Add(imp)
                Next
                objXMLImpianti.impianti = impiantiList
                Dim filePath As String
                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim nomeFile As String = ""
                If catasto Then
                    nomeFile = "ExportXMLImpiantiCatasto"
                Else
                    nomeFile = "ExportXMLImpianti"
                End If
                filePath = objConfSiti.Leggi_Valore(6, "GestioneEsportazioni_Repository", "", "", objParametri_Server) + "/" + nomeFile + "_" +
                            DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + DateTime.Now.Hour.ToString("D2") +
                            DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2") + ".xml"

                Dim writer As TextWriter = New StreamWriter(filePath)
                ser.Serialize(writer, objXMLImpianti)
                writer.Close()
            Case Else
                AgroMsgBox("Tipo di Esportazione non ancora implementata!", Page)
                Exit Sub
        End Select

    End Sub



    '########################################################################################################################
    Private Sub Esporta_XML_Rintraccio(ByRef Log As String,
                                        ByVal PathFinaleRintraccio As String,
                                        ByVal StringoneXML As String,
                                        ByVal piva As String,
                                        ByVal nome_documento As String,
                                        ByVal punto As Integer,
                                        ByVal XmlDoc As XmlDocument)

        Try

            '------------------------------------------------------------
            'questa parte è stata spostata in Esportazione_START

            'data_ora = Replace(Date.Today.Now.ToString, "/", "-")

            'GestioneFile_CreaCartellaNelPathWebConfig("Path_Esportazioni_Stampe", "Esportazione_Rintraccio_" + CStr(Date.Today.Year), PathCompleto, StrErrore)

            'If punto = 99 Then   '/************ PISELLO/FAGIOLO **********************/

            '    GestioneFile_CreaCartellaNelPath(PathCompleto, "Pisello_Fagiolo", PathFinale, StrErrore)

            'Else '/************ POMODORO **********************/

            '    GestioneFile_CreaCartellaNelPath(PathCompleto, "Pomodoro", PathFinale, StrErrore)

            'End If

            'FileSystem.ChDir(PathFinale)

            'NO, non si può cancellare qui, perchè se no cancella anche i file appena creati
            ''GestioneFile_CancellaFilesWithoutPattern(PathFinale)
            '------------------------------------------------------------


            '---------- 3° METODO ----------------- 
            XmlDoc.Save(PathFinaleRintraccio + "\FileXmlFiltro_" + piva + "_" + nome_documento + ".xml")
            '---------------------------------------


            ''---------- 2° METODO ----------------- 
            ''Da settembre 2009 questa funzione ha iniziato ad avere problemi:
            ''salva la stringa xml troncata
            ''Dove al posto di [NomeFileXml] ci metti il percorso completo del file da salvare su disco (con estensione .xml) 
            ''e al posto di [StringaXml] ci metti l'xml commutato in stringa.
            'Dim sw As New System.IO.StreamWriter("FileXmlFiltro_" + piva + "_" + nome_documento + ".xml", False, System.Text.Encoding.UTF8)
            'sw.Write(StringoneXML)
            '-------------------------------------------

            ''---------- 1° METODO ----------------- 
            ''a Michelangelo non gli andava bene
            ''perchè salvava il file con una codifica sbagliata
            'Dim FileObject = CreateCANCELLATOObject("Scripting.FileSystemObject")
            'Dim OutStream = FileObject.CreateTextFile("FileXmlFiltro_" + piva + "_" + nome_documento + ".xml", True, 0)

            'OutStream.WriteLine(StringoneXML)
            'OutStream.Close()

            'OutStream = Nothing
            'FileObject = Nothing
            ''-------------------------------------------

            'FileSystem.ChDir("C:\")

        Catch ex As Exception

            Log += CStr(Date.Now) + "   Impresa: " + piva + " Documento: " + nome_documento +
            " -   Errore durante l'esportazione XML Rintraccio: " + ex.Message & vbCrLf & vbCrLf

        End Try


    End Sub



    '#####################################################################################################
    Private Sub Esporta_Access(ByVal NomeFileOutput As String, ByVal DT_Finale As DataTable)

        AgroMsgBox("Tipo di Esportazione non ancora implementata!", Page)
        Exit Sub

    End Sub



    '#################################################################
    Private Sub Esporta_Testo_CSV(ByVal NomeFileOutput As String, ByVal DT_Finale As DataTable)

        AgroMsgBox("Tipo di Esportazione non ancora implementata!", Page)
        Exit Sub

    End Sub



    '#################################################################
    Private Sub Esporta_Testo_CampoFisso(ByVal NomeFileOutput As String, ByVal DT_Finale As DataTable)

        AgroMsgBox("Tipo di Esportazione non ancora implementata!", Page)
        Exit Sub

    End Sub


    '##################################################################
    Private Sub Btn_Salva_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Salva.Click

        SmartBuild_Salva()

    End Sub


    '################################################################################################
    Private Sub SmartBuild_Salva()

        If (Me.Txt_FileTxt.Text = "") Or (Me.Txt_FileTxt.Text = " ") Then
            AgroMsgBox("Inserire il nome del file da salvare!", Page)
            Exit Sub
        End If

        If (Me.Txt_Esadecimale.Text = "") Then
            AgroMsgBox("La Stringa SmartBuild non è stata ancora calcolata!" & vbCrLf & "Effettuare l'esportazione per calcolarla!", Page)
            Exit Sub
        End If

        Dim PathEsportatoreUniversale, PathCompleto, StrErrore As String
        Dim esiste As Boolean = False

        'GestioneFile_CreaCartellaNelPathWebConfig("Path_Esportazioni_Stampe", "Esportatore_Universale", PathCompleto, StrErrore)

        AgronicaCoreDataProvider.GestioneFile.CreaCartellaNelPath(objParametri_Server.LogDirectory, "Esportatore_Universale", PathCompleto, StrErrore)


        If StrErrore <> "" Then
            AgroMsgBox(StrErrore, Page)
            Exit Sub
        End If

        AgronicaCoreDataProvider.GestioneFile.CreaCartellaNelPath(PathCompleto, "SmartBuild_EsportatoreUniversale", PathEsportatoreUniversale, StrErrore)

        If StrErrore <> "" Then
            AgroMsgBox(StrErrore, Page)
            Exit Sub
        End If

        AgronicaCoreDataProvider.GestioneFile.CreaScriviFileSovrascrivi(Me.Txt_Esadecimale.Text, PathEsportatoreUniversale, Me.Txt_FileTxt.Text, StrErrore, "txt")

        If StrErrore <> "" Then
            AgroMsgBox(StrErrore, Page)
            Exit Sub
        Else
            AgroMsgBox("Salvataggio avvenuto con successo!", Page)
        End If

    End Sub



    '################################################################################################
    Private Sub Btn_Apri_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Apri.Click

        SmartBuild_Apri()

    End Sub

    '################################################################################################
    Private Sub SmartBuild_Apri()

        If Me.Txt_FileTxt.Text = "" Then
            AgroMsgBox("Inserire il nome del file da aprire!", Page)
            Exit Sub
        End If

        Dim PathCompleto As String
        Dim esiste_file As Boolean = False
        Dim esiste_dir As Boolean = False

        If objParametri_Server.LogDirectory <> "" Then

            If Not objParametri_Server.LogDirectory.EndsWith("\") Then
                objParametri_Server.LogDirectory += "\"
            End If

            'controllo se la cartella esiste già
            esiste_dir = System.IO.Directory.Exists(objParametri_Server.LogDirectory & "Esportatore_Universale\SmartBuild_EsportatoreUniversale")

            If esiste_dir = True Then

                'ESISTE

                PathCompleto = objParametri_Server.LogDirectory & "Esportatore_Universale\SmartBuild_EsportatoreUniversale"

                'ci vado dentro
                FileSystem.ChDir(PathCompleto)

                esiste_file = System.IO.File.Exists(PathCompleto & "\" & Me.Txt_FileTxt.Text & ".txt")

                If esiste_file Then

                    Dim FileObject = Server.CreateObject("Scripting.FileSystemObject")
                    Dim txtObject = FileObject.OpenTextFile(Me.Txt_FileTxt.Text & ".txt", 1, 0)

                    Me.Txt_Esadecimale.Text = txtObject.ReadLine()
                    Me.Txt_FileTxt.Text = ""

                    txtObject.Close()

                    txtObject = Nothing
                    FileObject = Nothing

                Else

                    AgroMsgBox("Impossibile trovare il file specificato!", Page)
                    Exit Sub

                End If

            Else

                'NON ESISTE

                AgroMsgBox("Impossibile trovare la cartella in cui è contenuto il file specificato!", Page)
                Exit Sub


            End If 'esiste cartella


        Else 'non c'è il percorso nel web config

            '----------------------------------------------------------
            'SE NEL WEB CONFIG NON C'E' IL PERCORSO, CERCO SU AGROTEMPORANEA
            '----------------------------------------------------------

            'controllo se la cartella esiste già
            esiste_dir = System.IO.Directory.Exists("c:\AgroTemporanea\Esportatore_Universale\SmartBuild_EsportatoreUniversale")

            If esiste_dir = True Then

                'ESISTE

                PathCompleto = "c:\AgroTemporanea\Esportatore_Universale\SmartBuild_EsportatoreUniversale"

                'ci vado dentro
                FileSystem.ChDir(PathCompleto)

                esiste_file = System.IO.File.Exists(PathCompleto & "\" & Me.Txt_FileTxt.Text & ".txt")

                If esiste_file Then

                    Dim FileObject = Server.CreateObject("Scripting.FileSystemObject")
                    Dim txtObject = FileObject.OpenTextFile(Me.Txt_FileTxt.Text & ".txt", 1, 0)

                    Me.Txt_Esadecimale.Text = txtObject.ReadLine()
                    Me.Txt_FileTxt.Text = ""

                    txtObject.Close()

                    txtObject = Nothing
                    FileObject = Nothing

                Else

                    AgroMsgBox("Impossibile trovare il file specificato!", Page)
                    Exit Sub

                End If

            Else

                'NON ESISTE

                AgroMsgBox("Impossibile trovare la cartella in cui è contenuto il file specificato!", Page)
                Exit Sub

            End If


        End If 'web config



    End Sub

    ''################################################################################################
    'Private Sub SmartBuild_Apri()

    '    If Me.Txt_FileTxt.Text = "" Then
    '        AgroMsgBox("Inserire il nome del file da aprire!", Page)
    '        Exit Sub
    '    End If

    '    Dim PathWebConfig, PathCompleto As String
    '    Dim esiste_file As Boolean = False
    '    Dim esiste_dir As Boolean = False

    '    If (Not IsNothing(ConfigurationSettings.AppSettings("Path_Esportazioni_Stampe")) And (ConfigurationSettings.AppSettings("Path_Esportazioni_Stampe") <> "")) Then

    '        'prelevo il percorso dal web config
    '        PathWebConfig = ConfigurationSettings.AppSettings("Path_Esportazioni_Stampe")

    '        'controllo se la cartella esiste già
    '        esiste_dir = System.IO.Directory.Exists(PathWebConfig & "\Esportatore_Universale\SmartBuild_EsportatoreUniversale")

    '        If esiste_dir = True Then

    '            'ESISTE

    '            PathCompleto = PathWebConfig & "\Esportatore_Universale\SmartBuild_EsportatoreUniversale"

    '            'ci vado dentro
    '            FileSystem.ChDir(PathCompleto)

    '            esiste_file = System.IO.File.Exists(PathCompleto & "\" & Me.Txt_FileTxt.Text & ".txt")

    '            If esiste_file Then

    '                Dim FileObject = Server.CreateObject("Scripting.FileSystemObject")
    '                Dim txtObject = FileObject.OpenTextFile(Me.Txt_FileTxt.Text & ".txt", 1, 0)

    '                Me.Txt_Esadecimale.Text = txtObject.ReadLine()
    '                Me.Txt_FileTxt.Text = ""

    '                txtObject.Close()

    '                txtObject = Nothing
    '                FileObject = Nothing

    '            Else

    '                AgroMsgBox("Impossibile trovare il file specificato!", Page)
    '                Exit Sub

    '            End If

    '        Else

    '            'NON ESISTE

    '            AgroMsgBox("Impossibile trovare la cartella in cui è contenuto il file specificato!", Page)
    '            Exit Sub


    '        End If 'esiste cartella


    '    Else 'non c'è il percorso nel web config

    '        '----------------------------------------------------------
    '        'SE NEL WEB CONFIG NON C'E' IL PERCORSO, CERCO SU AGROTEMPORANEA
    '        '----------------------------------------------------------

    '        'controllo se la cartella esiste già
    '        esiste_dir = System.IO.Directory.Exists("c:\AgroTemporanea\Esportatore_Universale\SmartBuild_EsportatoreUniversale")

    '        If esiste_dir = True Then

    '            'ESISTE

    '            PathCompleto = "c:\AgroTemporanea\Esportatore_Universale\SmartBuild_EsportatoreUniversale"

    '            'ci vado dentro
    '            FileSystem.ChDir(PathCompleto)

    '            esiste_file = System.IO.File.Exists(PathCompleto & "\" & Me.Txt_FileTxt.Text & ".txt")

    '            If esiste_file Then

    '                Dim FileObject = Server.CreateObject("Scripting.FileSystemObject")
    '                Dim txtObject = FileObject.OpenTextFile(Me.Txt_FileTxt.Text & ".txt", 1, 0)

    '                Me.Txt_Esadecimale.Text = txtObject.ReadLine()
    '                Me.Txt_FileTxt.Text = ""

    '                txtObject.Close()

    '                txtObject = Nothing
    '                FileObject = Nothing

    '            Else

    '                AgroMsgBox("Impossibile trovare il file specificato!", Page)
    '                Exit Sub

    '            End If

    '        Else

    '            'NON ESISTE

    '            AgroMsgBox("Impossibile trovare la cartella in cui è contenuto il file specificato!", Page)
    '            Exit Sub

    '        End If


    '    End If 'web config



    'End Sub

    Private Enum enum_OrdinamentoImpianto
        OrdinamentoDefault = 1
        ImpresaCentroParticella = 2
        CoopImpresa = 3
    End Enum

    Private Sub Carica_Cmb_OrdinamentoImpianti()

        Me.Cmb_OrdinamentoImpianti.Items.Clear()

        'me.Cmb_OrdinamentoImpianti.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))

        Me.Cmb_OrdinamentoImpianti.Items.Add(New ListItem("Ordinamento Default", enum_OrdinamentoImpianto.OrdinamentoDefault))
        Me.Cmb_OrdinamentoImpianti.Items.Add(New ListItem("Impresa, Centro, Particella", enum_OrdinamentoImpianto.ImpresaCentroParticella))
        Me.Cmb_OrdinamentoImpianti.Items.Add(New ListItem("Cooperativa padre, impresa", enum_OrdinamentoImpianto.CoopImpresa))

    End Sub


    ''###############################################################################################
    'Private Function Recupera_TabelleTemp_Mode() As Integer

    '    '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT - INSERT INTO
    '    '2: CREAZIONE TABELLA TEMPORANEA TRAMITE CREATE TABLE

    '    Dim TabelleTemp_Mode As Integer

    '    If Not IsNothing(ConfigurationSettings.AppSettings("TabelleTemp_Mode")) And _
    '            ConfigurationSettings.AppSettings("TabelleTemp_Mode") <> "" Then

    '        TabelleTemp_Mode = ConfigurationSettings.AppSettings("TabelleTemp_Mode")

    '        If TabelleTemp_Mode <> 1 And TabelleTemp_Mode <> 2 Then
    '            TabelleTemp_Mode = 1
    '        End If

    '    Else
    '        TabelleTemp_Mode = 1

    '    End If


    '    Return TabelleTemp_Mode


    'End Function


#Region "Funzioni Inserite"

    Private Sub AgroMsgBox(ByVal Testo, Optional ByVal Page = Nothing)
        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        Testo = Replace(Testo, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        Testo = Replace(Testo, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        Testo = Replace(Testo, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        Testo = Replace(Testo, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        Testo = Replace(Testo, Chr(13), "\r")

        '----- Faccio apparire un msgbox aggiungendo il controllo alla form
        ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('" + Testo + "');", True)
    End Sub

    Public Function BinarioFromDecimale(ByVal Decimale As Long) As String
        Dim Dec As Long
        Dim Bin As String
        Dec = Decimale
        Bin = ""
        Do While Dec > 1
            Bin = Trim$(Dec Mod 2) & Bin
            Dec = Int(Dec / 2)
        Loop
        If Decimale > 1 Then
            Bin = "1" & Bin
        End If
        BinarioFromDecimale = Bin

    End Function

    Public Function Controlla_Permessi_Utente_2(ByRef objServer As System.Web.HttpServerUtility,
                                                ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                ByRef objPage As System.Web.UI.Page,
                                                ByVal UserName As String,
                                                ByVal Servizio As Integer,
                                                ByVal Attivita As enum_Security_Attivita,
                                                ByVal Operazione As enum_Security_Operazione,
                                                ByRef MsgEsito As String) _
                                                As Boolean


        'NOTA
        '   Il parametro di uscita MsgEsito, assume i seguenti valori
        '   a seconda dell'esito della verifica e delle motivazioni
        '
        '       "0.Permesso concesso"
        '       "1.Permesso negato"
        '       "2.Permesso scaduto il 25/05/2002"
        '       "3.Permesso attivo a partire dal 25/05/2003"
        '       "4.Permesso attivo dalle 8.30 alle 12.30"
        '
        '   Prima della visualizzazione troncare i primi due caratteri
        '   i quali mi possono invece essere utili per una verifica da codice


        '----- Dimensiono le variabili

        Dim xAttivita As Integer
        Dim xOperazione As Integer

        Dim UtenteAbilitato As Boolean
        Dim Testo As String
        Dim FunzionalitaAttivata As Boolean

        '----- Recupero i valori

        xAttivita = Attivita
        xOperazione = Operazione


        '----- Verifico le eccezioni ai controlli

        'NOTA
        'Un giorno dovra' essere previsto il controllo sulla chiave ONLINE_KEY
        'Per ora lavoro con il web.config ...


        '### Gestione Analisi ###

        'If (Attivita = enum_Security_Attivita.Gest_Analisi_AccessoMenu) Or _
        '   (Attivita = enum_Security_Attivita.Gest_Analisi_Cartografia) Then

        '    If Not IsNothing(ConfigurationSettings.AppSettings("Flag_GestioneAnalisiAttivo")) Then
        '        Testo = ConfigurationSettings.AppSettings("Flag_GestioneAnalisiAttivo").ToString
        '        FunzionalitaAttivata = CBool(Testo)
        '    Else
        '        FunzionalitaAttivata = False
        '    End If

        '    'Se la funzione non e' attivata ...
        '    If FunzionalitaAttivata = False Then

        '        Return False
        '        Exit Function

        '    End If

        'End If

        FunzionalitaAttivata = True



        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                Session("ASG_Utente_Username"),
                                Session("ASG_IdServizio"),
                                xAttivita,
                                xOperazione,
                                Date.Now,
                                "",
                                objParametri_Utenti)

        'UtenteAbilitato = Verifica_Utenti_Permessi(objServer, objSession, objPage, _
        '                                            MsgEsito, _
        '                                            UserName, _
        '                                            xOperazione, _
        '                                            xAttivita, _
        '                                            Date.Now, _
        '                                            CType(Now.Hour, Short))

        'Restituisco il risultato
        Return UtenteAbilitato



    End Function

#End Region

    Private Sub BtnSelezionaTutto_ServerClick(sender As Object, e As System.EventArgs) Handles BtnSelezionaTutto.ServerClick
        Dim i As Integer

        For i = 0 To 9
            If Me.ChkGruppo1.Items.FindByValue(i).Enabled = True Then
                Me.ChkGruppo1.Items.FindByValue(i).Selected = True
            End If
        Next

        For i = 10 To 19
            If Me.ChkGruppo2.Items.FindByValue(i).Enabled = True Then
                Me.ChkGruppo2.Items.FindByValue(i).Selected = True
            End If
        Next

        For i = 20 To 31
            If Me.ChkGruppo3.Items.FindByValue(i).Enabled = True Then
                Me.ChkGruppo3.Items.FindByValue(i).Selected = True
            End If
        Next

        Me.ChkGruppo4.Items.FindByValue(32).Selected = True
        Me.ChkGruppo5.Items.FindByValue(33).Selected = True
        Me.ChkGruppo5.Items.FindByValue(34).Selected = True

        Select Case ViewState("scheda")

            'Case "impianti"
            'quando c'era il poligono
            '    Me.ChkGruppo3.Items.FindByValue(26).Selected = False

            Case "imprese"
                Me.ChkGruppo2.Items.FindByValue(17).Selected = False
                Me.ChkGruppo2.Items.FindByValue(18).Selected = False
                Me.ChkGruppo2.Items.FindByValue(19).Selected = False

        End Select
    End Sub

    Private Sub BtnDeselezionaTutto_ServerClick(sender As Object, e As System.EventArgs) Handles BtnDeselezionaTutto.ServerClick
        Dim i As Integer

        For i = 0 To 9
            If Me.ChkGruppo1.Items.FindByValue(i).Enabled Then
                Me.ChkGruppo1.Items.FindByValue(i).Selected = False
            End If
        Next

        For i = 10 To 19
            If Me.ChkGruppo2.Items.FindByValue(i).Enabled Then
                Me.ChkGruppo2.Items.FindByValue(i).Selected = False
            End If
        Next

        For i = 20 To 31
            If Me.ChkGruppo3.Items.FindByValue(i).Enabled Then
                Me.ChkGruppo3.Items.FindByValue(i).Selected = False
            End If
        Next

        Me.ChkGruppo4.Items.FindByValue(32).Selected = False
        Me.ChkGruppo5.Items.FindByValue(33).Selected = False
        Me.ChkGruppo5.Items.FindByValue(34).Selected = False

    End Sub

    Private Shared Sub Popola__tmp_Agenda(ByVal NomeRoutine As String,
                                    ByVal ListChiaviAgenda As List(Of String),
                                    ByRef Idtestata As Integer)

        Dim FlagTransazioneLocale_Server As Boolean = False
        Dim FlagConnessioneLocale_Server As Boolean = False

        Dim New_NomeRoutine As String = NomeRoutine & ".Popola__tmp_Agenda_Esportatore_Universale"

        Dim Piva As String

        Dim Sa_Cod As Integer

        Dim id_Agenda As Integer

        Dim objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim tmp_Agenda_W As New AgronicaCoreVarieDAL.__tmp_Agenda_W

        Try
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_Server,
                                                                                    FlagTransazioneLocale_Server,
                                                                                    objParametriServer)

            If Not IsNothing(ListChiaviAgenda) AndAlso ListChiaviAgenda.Count > 0 Then

                Dim tempAgendaModels As New List(Of __Tmp_Agenda_Model2)

                For Each c As String In ListChiaviAgenda

                    Dim ChiaviAgenda_Split = c.Split("_")

                    If Not IsNothing(ChiaviAgenda_Split) AndAlso ChiaviAgenda_Split.Length > 0 Then

                        Piva = ChiaviAgenda_Split(0)

                        Sa_Cod = CInt(ChiaviAgenda_Split(1))

                        id_Agenda = CInt(ChiaviAgenda_Split(2))

                        Dim model As New __Tmp_Agenda_Model2
                        model.piva = Piva
                        model.id_agenda = id_Agenda
                        model.lav_cod = 0

                        tempAgendaModels.Add(model)

                        'obj__tmp_Agenda_W.Scrivi(Idtestata, Piva, id_Agenda, 0, objParametri_Server)
                    End If
                Next

                tmp_Agenda_W.ScriviMassivo2(Idtestata, tempAgendaModels, objParametriServer)

            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_Server, objParametriServer)

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametriServer.objConnessione Is Nothing AndAlso
               Not objParametriServer.objTransazione Is Nothing AndAlso
               FlagTransazioneLocale_Server = True Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If
            'MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & New_NomeRoutine & "] : " & ex.Message)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_Server, objParametriServer)
        End Try

    End Sub


End Class