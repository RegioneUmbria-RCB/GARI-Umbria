
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaControlli_2010
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Xml
Imports System.Web.Services

Public Class Ricette_Edit
    Inherits System.Web.UI.Page


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

#Region "Metodi per ws"

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaComboMisuraXAvversitaInputData(ByVal MxAV_Cod As String, ByVal av_cod As String, ByVal udm_cod As String, ByVal veg_cod As String) As String
        Return AgronicaControlli_2010.ComboMisuraXAvversitaInputData.CaricaComboMisuraXAvversitaInputData(MxAV_Cod, av_cod, udm_cod, veg_cod)
    End Function

#End Region

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As Integer
    Dim Qs_Ricetta_Cod As String
    Dim Qs_Operazione As String
    Dim Qs_Veg_Cod As String
    Dim Qs_Cul_Cod As String
    Dim Qs_Data_Inizio As String
    Dim Qs_Data_Fine As String
    Dim Qs_Destinazione As String
    Dim Qs_Tipo_Ricetta As String
    Dim Qs_Unid_Ricetta As String
    Dim Qs_Unid_Ricetta_Operazione As String
    Dim Qs_Unid_Operazione As String
    Dim Qs_Ricetta_Des As String
    Dim Qs_Origine As String
    Dim Qs_SitoOrigine_Cod As Integer
    Dim Qs_Ricetta_Des_Long As String
    Dim Qs_Programmazione_Cod As Integer


    Dim frm_BaseCode As Integer
    Dim frm_TopCode As Integer

    Dim TabellaTrappole As Table
    Private Const SeparaID As String = "*"

    '################################################################################################################
    Private Sub Ricette_Manager_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub

    '########################################################################################
    Private Enum enum_TipoPannello

        Nessun_Pannello = 0

        Pannello_Impianti = 1

        PannelloDifesa = 2
        PannelloConcimazione = 3
        PannelloIrrigazione = 4
        PannelloTrappole = 5

        Pannello_Testata = 6

        Pannello_Costi = 7

        PannelloLavorazioni = 8

        Pannello_RilievoAvvAus = 9

    End Enum

    '################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        If Not IsNothing(Qs_SitoOrigine_Cod) Then
            Select Case Qs_SitoOrigine_Cod
                Case Enum_SiteRedirector.Sito_PianoConcimazione_2017

                    Dim objPC_2017 As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                    objPC_2017.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.MenuBS
                    objPC_2017.Pagina_SitoOrigine = enum_PagineAgenda_2010.Pagina_Ricette_Edit
                    objPC_2017.Piva = Qs_Piva
                    objPC_2017.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                    Dim link As String = RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                                                                                                Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objPC_2017)
                    Response.Redirect(link)
                Case Else

            End Select
        End If

        If Qs_Origine = "" Then
            Qs_Origine = "../menu/menu.aspx"
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                Qs_Origine = "../Menu/MenuBS_Agenda_Nuovo.aspx"
                Response.Redirect(Qs_Origine)
            End If
        End If

        Dim StrRedirect As String
        If Qs_Destinazione <> "" Then
            StrRedirect = Qs_Destinazione
        Else
            'provengo al momento da ricette lista
            StrRedirect = "Ricette_Manager.aspx"
            ''provengo direttamente dal menu agenda vecchio
            ''----- Chiudo la finestra
            'Page.Master.FindControl("Form1").Controls.Add( _
            '    New LiteralControl( _
            '        "<script language='javascript'>window.close();</script>"))
        End If
        StrRedirect = Qs_Destinazione & "?origine=" & Stringa_Codifica(Qs_Origine, AgroKey_EncoderDecoder, Server) _
                                            & "&p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)
        Response.Redirect(StrRedirect)


    End Sub

    Private Sub InizializzaTabellaTrappole()
        TabellaTrappole = New Table
        TabellaTrappole.ID = "TabellaTrappole"
        TabellaTrappole.CssClass = "ui-widget-content"
        TabellaTrappole.CellPadding = 5
        TabellaTrappole.CellSpacing = 0
        TabellaTrappole.ClientIDMode = UI.ClientIDMode.Static
        TabellaTrappole.BorderWidth = 1
        TabellaTrappole.Style.Add("width", "100%")
    End Sub

    Private Sub InizializzaVarie()

        ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                         String.Format("jQuery_{0}", ComboOperazione.ClientID), ComboOperazione.GetJS(), True)

        ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                                 String.Format("jQuery_{0}", ComboPianificazioni.ClientID), ComboPianificazioni.GetJS(), True)

        ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                    String.Format("jQuery_{0}", ComboEpoche.ClientID), ComboEpoche.GetJS(), True)

        'ScriptManager.RegisterStartupScript(Page, Page.GetType(),
        '                        String.Format("jQuery_{0}", ComboFormulati.ClientID), ComboFormulati.GetJS(), True)

        Dim script As New StringBuilder

        script.Length = 0

        script.AppendLine("$(document).ready(function () { ")

        script.AppendLine(" $('.bottone').button(); ")

        script.AppendLine(" $('#tabs-l').tabs(); ")
        script.AppendLine(" $('#tabs-d').tabs(); ")
        script.AppendLine(" $('#tabs-c').tabs(); ")
        script.AppendLine(" $('#tabs-i').tabs(); ")
        script.AppendLine(" $('#tabs-t').tabs(); ")

        'script.AppendLine("    $('#" & ComboFertilizzanti.ClientID & "').combobox();")
        script.AppendLine("    $('#" & Cmb_FormulatoClassificazioni.ClientID & "').combobox();")


        script.AppendLine("    $('#" & Cmb_Specie.ClientID & "').combobox();")
        script.AppendLine("    $('#" & Cmb_Disciplinare.ClientID & "').combobox();")

        'script.AppendLine("    $('#" & BTN_ComboFormulati.ClientID & "').click(); ")
        'script.AppendLine("    $('#" & BTN_ComboFertilizzanti.ClientID & "').click(); ")

        script.AppendLine("     $('#btn_costi_accessori').click(function () { ")
        script.AppendLine("         $('#" & AggiornaCostiAccessori.ClientID & "').click();")
        script.AppendLine("         $('#dialogCostiAccessori').dialog('open');")
        script.AppendLine("         $('#dialogCostiAccessori').parent().appendTo($('form:first')); ")
        script.AppendLine("     });")

        script.AppendLine("     $('#" & Txt_DataInizio.ClientID & "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        script.AppendLine("     $('#" & Txt_DataFine.ClientID & "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        script.AppendLine("     $('#" & Txt_DataInizio_Irrigazione.ClientID & "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        script.AppendLine("     $('#" & Txt_DataFine_Irrigazione.ClientID & "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        script.AppendLine("     $('#" & Txt_Data_Interventi.ClientID & "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")

        script.AppendLine("     $.datepicker.regional['it']; ")

        script.AppendLine("     Click_su_Giorno_Intervallo(); ")
        script.AppendLine("     Click_su_RBL_Cultivar(); ")
        script.AppendLine("     Click_su_Singole_Gruppo(); ")

        script.AppendLine("      $('#" & RBL_Data.ClientID & "').click(function () {")
        script.AppendLine("          Click_su_Giorno_Intervallo();")
        script.AppendLine("          Calcola_Str_Descrizione();")
        script.AppendLine("      }); ")

        script.AppendLine(" $('#" & RBL_Cultivar.ClientID & "').click(function () { ")
        script.AppendLine("     Click_su_RBL_Cultivar(); ")
        script.AppendLine(" }); ")

        script.AppendLine(" $('#" & Cmb_Specie.ClientID & "').change( ")
        script.AppendLine("     function () { ")
        script.AppendLine("         Calcola_Str_Descrizione(); ")
        script.AppendLine("     }); ")

        script.AppendLine(" $('#" & Txt_DataFine.ClientID & "').change( ")
        script.AppendLine("     function () { ")
        script.AppendLine("         Calcola_Str_Descrizione(); ")
        script.AppendLine("     }); ")

        script.AppendLine(" $('#" & Txt_DataInizio.ClientID & "').change( ")
        script.AppendLine("     function () { ")
        'script.AppendLine("         $('#" & Txt_DataFine.ClientID & "').val($('#" & Txt_DataInizio.ClientID & "').val()); ")
        script.AppendLine("         Calcola_Str_Descrizione(); ")
        script.AppendLine("     }); ")



        script.AppendLine("     $('#dialogOperazione').dialog({ ")
        script.AppendLine("             autoOpen: false,")
        script.AppendLine("             width: 'auto',")
        script.AppendLine("             height: 'auto',")
        script.AppendLine("             modal: true")
        'script.AppendLine("             modal: true,")
        'script.AppendLine("             buttons: {")
        'script.AppendLine("                 'Aggiungi Dettaglio': function () {")
        'script.AppendLine("                 $(this).dialog('close');")
        'script.AppendLine("                 $('#WaitFrame').hide();")
        'script.AppendLine("                 SalvaLavorazioni();")
        'script.AppendLine("             },")
        'script.AppendLine("             'Annulla': function () {")
        'script.AppendLine("                 $(this).dialog('close');")
        'script.AppendLine("                 $('#WaitFrame').hide();")
        'script.AppendLine("                 AnnullaLavorazioni();")
        'script.AppendLine("             }")
        'script.AppendLine("         }")
        script.AppendLine("     });")

        script.AppendLine(" $('#" & RBL_Avversita.ClientID & "').click(function () {")
        script.AppendLine("     Click_su_Singole_Gruppo();")
        script.AppendLine(" });")

        script.AppendLine("}); ")

        ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType(),
                                        String.Format("jQuery_{0}", Script_Panel.ClientID), script.ToString, True)

    End Sub

    Private Sub GestisciComboPianificazioni(ByVal RegistraScript As Boolean)

        If Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni_Planning Then

            Dim veg_cod_Selezionato As Integer = 0
            If Cmb_Specie.SelectedValue <> "" Then
                veg_cod_Selezionato = Cmb_Specie.SelectedValue
            End If

            Dim Data_Da As Date
            Dim Data_A As Date

            Data_Da = Txt_DataInizio.Text
            Data_A = Txt_DataFine.Text

            If Data_A < Data_Da Then
                Data_A = Data_Da
            End If

            ComboPianificazioni.CaricaComboPianificazioni(Qs_Piva, veg_cod_Selezionato, Data_Da, Data_A)
            If Not String.IsNullOrEmpty(Qs_Programmazione_Cod) Then
                ComboPianificazioni.Valore_Combo = Qs_Programmazione_Cod
            End If

            If RegistraScript Then

                ScriptManager.RegisterClientScriptBlock(upTabs_2, upTabs_2.GetType(),
                                                 String.Format("jQuery_{0}", upTabs_2.ClientID), ComboPianificazioni.GetJS(), True)

            End If


        End If


    End Sub

    '################################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        InizializzaVarie()

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
        objParametriAgenda = New ParametriAgenda


        Dim vDal As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Sostituzioni As String = vDal.Leggi_Valore(6, "Nomenclatura_Ricette", "", "", objParametri_Server)

        AgronicaControlli_2010.UI_ControlsHelper.RinominaControlli(Sostituzioni, Me, Nothing)

        '##############################################################
        '#####  Recupero la chiave che identifica l'oggetto  ##########
        '##############################################################

        Qs_Ricetta_Cod = "0"
        Qs_Operazione = "0"
        Qs_Tipo_Ricetta = "0"



        If Not IsNothing(Request.QueryString("p")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Piva = ""
        End If



        If Not IsNothing(Request.QueryString("s")) Then
            Qs_Sa_Cod = CInt(Stringa_Decodifica(Request.QueryString("s").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server))
        Else
            Qs_Sa_Cod = 0
        End If

        If Not IsNothing(Request.QueryString("r")) Then

            Qs_Ricetta_Cod = Stringa_Decodifica(Request.QueryString("r").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        ElseIf Not IsNothing(Session("Ricetta_Cod")) Then
            Qs_Ricetta_Cod = Session("Ricetta_Cod")
        End If


        If Not IsNothing(Request.QueryString("o")) Then

            Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        End If

        If Not IsNothing(Request.QueryString("veg_cod")) Then
            Qs_Veg_Cod = Stringa_Decodifica(Request.QueryString("veg_cod").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Veg_Cod = "0"
        End If

        If Not IsNothing(Request.QueryString("cul_cod")) Then
            Qs_Cul_Cod = Stringa_Decodifica(Request.QueryString("cul_cod").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Cul_Cod = ""
        End If

        If Not IsNothing(Request.QueryString("data_inizio")) Then
            Qs_Data_Inizio = Stringa_Decodifica(Request.QueryString("data_inizio").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Data_Inizio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO

        End If

        If Not IsNothing(Request.QueryString("data_fine")) Then
            Qs_Data_Fine = Stringa_Decodifica(Request.QueryString("data_fine").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Data_Fine = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
        End If


        If Not IsNothing(Request.QueryString("destinazione")) Then
            Qs_Destinazione = Stringa_Decodifica(Request.QueryString("destinazione").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Destinazione = ""
        End If

        If Not IsNothing(Request.QueryString("sito_or")) Then
            Qs_SitoOrigine_Cod = Stringa_Decodifica(Request.QueryString("sito_or").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_SitoOrigine_Cod = 0
        End If


        If Not IsNothing(Request.QueryString("origine")) Then
            Qs_Origine = Stringa_Decodifica(Request.QueryString("origine").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Origine = ""
        End If

        If Not IsNothing(Request.QueryString("tipo_ricetta")) Then
            Qs_Tipo_Ricetta = Stringa_Decodifica(Request.QueryString("tipo_ricetta").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Tipo_Ricetta = "0"
        End If


        'Modifica Simone Galassi per impostare l'intervallo personalizzato solo se entrambe le costanti non sono impostate
        If (Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi AndAlso
            Qs_Data_Inizio = AGRODATAINIZIO AndAlso
            Qs_Data_Fine = AGRODATAFINE
           ) Then
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DataInizio, DataFine As Date
            objImpost.AnnataAgraria(Date.Today, DataInizio, DataFine, objParametri_Utenti)

            Qs_Data_Inizio = DataInizio
            Qs_Data_Fine = DataFine

        End If

        If Not IsNothing(Request.QueryString("unid_ricetta")) Then
            Qs_Unid_Ricetta = Stringa_Decodifica(Request.QueryString("unid_ricetta").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Unid_Ricetta = ""
        End If
        If Not IsNothing(Request.QueryString("unid_ricetta_operazione")) Then
            Qs_Unid_Ricetta_Operazione = Stringa_Decodifica(Request.QueryString("unid_ricetta_operazione").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Unid_Ricetta_Operazione = ""
        End If
        If Not IsNothing(Request.QueryString("unid_operazione")) Then
            Qs_Unid_Operazione = Stringa_Decodifica(Request.QueryString("unid_operazione").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Unid_Operazione = ""
        End If

        If Not IsNothing(Request.QueryString("ricetta_des")) AndAlso Request.QueryString("ricetta_des") <> "" Then
            Qs_Ricetta_Des = Stringa_Decodifica(Request.QueryString("ricetta_des").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Ricetta_Des = ""
        End If

        If Not IsNothing(Request.QueryString("ricetta_des_long")) AndAlso Request.QueryString("ricetta_des_long") <> "" Then
            Qs_Ricetta_Des_Long = Stringa_Decodifica(Request.QueryString("ricetta_des_long").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Ricetta_Des_Long = ""
        End If

        If Not IsNothing(Request.QueryString("programmazione_cod")) AndAlso Request.QueryString("programmazione_cod") <> "0" Then
            Qs_Programmazione_Cod = Stringa_Decodifica(Request.QueryString("programmazione_cod").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Programmazione_Cod = 0
        End If

        'se la ricetta è un piano distribuzione verifico se è BLOCCATO
        Select Case Qs_Tipo_Ricetta
            Case enum_TipoRicetta.PianoDistribuzioneConcimi
                Dim objPC As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R
                Dim DtPC As DataTable = objPC.Leggi(Qs_Programmazione_Cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If DtPC IsNot Nothing AndAlso DtPC.Rows.Count > 0 Then
                    If Not IsDBNull(DtPC.Rows(0).Item("blocco_flag")) AndAlso IsNumeric(DtPC.Rows(0).Item("blocco_flag")) AndAlso CInt(DtPC.Rows(0).Item("blocco_flag")) > 0 Then
                        Btn_Salva.Visible = False
                    End If
                End If
        End Select


        '------------------------------------------------
        Calcola_BaseCode_TopCode(frm_BaseCode, frm_TopCode, Session("ASG_ProgressivoGIAS"))
        '------------------------------------------------

        Lbl_TipoRicetta.Text = Qs_Tipo_Ricetta

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then
            'output.Write("Page has just been loaded")

            ViewState("Piva") = Qs_Piva
            ViewState("Sa_Cod") = Qs_Sa_Cod
            ViewState("Programmazione_Cod") = Qs_Programmazione_Cod

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Gest_Ricette,
                                    enum_Security_Operazione.Modifica,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

            '----- Se l'utente non ha il permesso per visualizzare la pagina ... 

            If Not UtenteAbilitato Then

                'Esco dalla routine di Page_Load

                If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                    If objPermessi.Controlla_Permessi_Utente(
                                                Session("ASG_Utente_Username"),
                                                Session("ASG_IdServizio"),
                                                enum_Security_Attivita.SupportoDecisioni_PianoConcimazione,
                                                enum_Security_Operazione.Modifica,
                                                Date.Now,
                                                "",
                                                objParametri_Utenti) Then
                    Else
                        Exit Sub
                    End If
                Else
                    Exit Sub
                End If
            End If

            Dim ricettausata As String = ""
            If Not IsNothing(Request.QueryString("usata")) Then

                'non la codifico
                'ricettausata = Stringa_Decodifica(Request.QueryString("usata").ToString, _
                '                                    AgroKey_EncoderDecoder, _
                '                                    Server)
                ricettausata = Request.QueryString("usata").ToString
            Else
                ricettausata = ""
            End If
            If ricettausata = "1" Then
                Call Messaggi.AgroMsgBox("ATTENZIONE, si sta modificando una ricetta collegata a delle operazioni colturali.", Page, , Script_Panel)
            End If

            Select Case Qs_Tipo_Ricetta
                Case enum_TipoRicetta.Standard_Destinazioni
                    CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = "Ricetta Aziendale"
                Case enum_TipoRicetta.Standard_Destinazioni_Planning
                    CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = "Pianifica Attività"
                Case enum_TipoRicetta.PianoDistribuzioneConcimi
                    CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = "Piano Distribuzione Concimi"
                Case Else
                    CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = "Linea Tecnica"
            End Select


        Else
            'output.Write("Postback has occured")
            Abilita_Disabilita_Data()

            If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                Carica_Impianti("", ViewState("dt_Impianti"))
            End If

            Exit Sub
        End If

        'Gestione intervallo di tempo
        If Qs_Data_Inizio = CStr(AGRODATAINIZIO) OrElse
           Qs_Data_Fine = CStr(AGRODATAFINE) OrElse
           Qs_Data_Inizio = Qs_Data_Fine Then
            RBL_Data.SelectedIndex = 0
        Else
            RBL_Data.SelectedIndex = 1
        End If


        '  Marco Grilli, 12/04/2016 09:29:30: Colonne del GridView Da mostrare in base al tipo di ricetta
        Select Case Qs_Tipo_Ricetta
            Case enum_TipoRicetta.PianoDistribuzioneConcimi

                '  Marco Grilli, 12/04/2016 09:39:19: Mettere il nome o se si aggiunge un campo, è la fine.....
                GridView_Impianti.Columns.Item(20).Visible = True 'N_Max
                GridView_Impianti.Columns.Item(21).Visible = True 'P_Max
                GridView_Impianti.Columns.Item(22).Visible = True 'K_Max
                GridView_Impianti.Columns.Item(23).Visible = True 'N_Distribuito
                GridView_Impianti.Columns.Item(24).Visible = True 'P_Distribuito
                GridView_Impianti.Columns.Item(25).Visible = True 'K_Distribuito
                GridView_Impianti.Columns.Item(26).Visible = True 'N_Residuo
                GridView_Impianti.Columns.Item(27).Visible = True 'P_Residuo
                GridView_Impianti.Columns.Item(28).Visible = True 'K_Residuo

                'nascondo x ora perchè da valorizzare
                GridView_Impianti.Columns.Item(15).Visible = False 'progetto
                GridView_Impianti.Columns.Item(18).Visible = False 'inizio impianto
                GridView_Impianti.Columns.Item(19).Visible = False 'fine impianto

        End Select


        '#######################################################
        '#####  Carico le informazioni  ########################
        '#######################################################
        Dim Filtro As String = ""

        Select Case Qs_Tipo_Ricetta
            Case enum_TipoRicetta.Standard_Destinazioni, enum_TipoRicetta.Standard_Destinazioni_Planning

                'mancano rispetto sotto irrigazione e trappole
                Filtro = " AND operazioni.lav_cod IN (" &
                          LAVCOD_ALTRE_OPERAZIONI & "," &
                          LAVCOD_DISERBO & "," & LAVCOD_TRATTAMENTO_ANTIPARASSITARIO & "," & LAVCOD_TRATTAMENTO_FITOREGOLATORE & "," &
                          LAVCOD_GEODISINFESTAZIONE & "," & LAVCOD_CONCIA_SEME & "," & LAVCOD_DISSECCAMENTO & "," &
                          LAVCOD_DISTRIBUZIONE_CONCIME & "," & LAVCOD_FERTIRRIGAZIONE & "," & LAVCOD_TRATTAMENTO_ANTIBUTTERATURA & "," &
                          LAVCOD_CONCIMAZIONE_FOGLIARE & "," & LAVCOD_DISTRIBUZIONE_AMMENDANTI & "," & LAVCOD_SARCHIATURA_CONCIMAZIONE & "," &
                          LAVCOD_ARATURA & "," & LAVCOD_DEFOGLIAZIONE & "," & LAVCOD_ANDANAMENTO & "," &
                          LAVCOD_ASPORTAZIONE_ORGANI_INFETTI & "," & LAVCOD_ASSOLCATURA & "," & LAVCOD_CARICO_MANUALE_FRUTTA & "," &
                          LAVCOD_CIMATURA & "," & LAVCOD_DIRADAMENTO_MANUALE & "," & LAVCOD_DISSODAMENTO & "," &
                          LAVCOD_ERPICATURA & "," & LAVCOD_ERPICATURA_ROTANTE & "," & LAVCOD_ESPIANTO & "," &
                          LAVCOD_ESTIRPATURA & "," & LAVCOD_FALCIACONDIZIONATURA & "," & LAVCOD_FALCIATURA_ERBAI & "," &
                          LAVCOD_FORMAZIONE_ARGINELLI & "," & LAVCOD_FRANGIZOLLATURA & "," &
                          LAVCOD_FRESATURA & "," & LAVCOD_GEBIATURA & "," & LAVCOD_IMBALLO_FIENO_ROTOLI & "," & LAVCOD_INTERRAMENTO_PAGLIE & "," &
                          LAVCOD_INTERVENTO_ANTIBRINA & "," & LAVCOD_LAVORAZIONE_CONBINATA & "," &
                          LAVCOD_LAVORAZIONE_TRA_FILA & "," & LAVCOD_LAVORAZIONE_SU_FILA & "," & LAVCOD_LEGATURA & "," & LAVCOD_LIVELLAMENTO & "," &
                          LAVCOD_MANUTENZIONE_ARGINI & "," & LAVCOD_MESSA_DIMORA_PIANTE & "," & LAVCOD_MIETITREBBIATURA & "," & LAVCOD_MINIMUM_TILLAGE & "," &
                          LAVCOD_PACCIAMATURA & "," & LAVCOD_POTATURA_SECCA & "," & LAVCOD_POTATURA_VERDE & "," & LAVCOD_PRESSATURA & "," & LAVCOD_RACCOLTA_LEGNA_POTATURA & "," &
                          LAVCOD_RANGHINATURA & "," & LAVCOD_RINCALZATURA & "," & LAVCOD_RIPPATURA & "," & LAVCOD_RIPUNTATURA & "," & LAVCOD_RIVOLTAMENTO_FORAGGIO & "," &
                          LAVCOD_ROMPICROSTA & "," & LAVCOD_RULLATURA & "," & LAVCOD_SARCHIATURA & "," & LAVCOD_SCARIFICATURA & "," & LAVCOD_SCASSO & "," &
                          LAVCOD_SOD_SEDDING & "," & LAVCOD_TRINCIATURA & "," & LAVCOD_VANGATURA & "," & LAVCOD_ZAPPATURA & "," &
                          LAVCOD_SEMINA & "," & LAVCOD_SOVESCIO & "," & LAVCOD_TRAPIANTO & "," & LAVCOD_RILIEVO_AVVERSITA_CAMPO & ")"

            Case enum_TipoRicetta.PianoDistribuzioneConcimi
                Filtro = " AND operazioni.lav_cod IN (" &
                          LAVCOD_DISTRIBUZIONE_CONCIME & "," & LAVCOD_FERTIRRIGAZIONE & "," & LAVCOD_TRATTAMENTO_ANTIBUTTERATURA & "," &
                          LAVCOD_CONCIMAZIONE_FOGLIARE & "," & LAVCOD_DISTRIBUZIONE_AMMENDANTI & "," & LAVCOD_SARCHIATURA_CONCIMAZIONE & ")"
            Case Else

                Filtro = " AND operazioni.lav_cod IN (" & LAVCOD_DISERBO & "," & LAVCOD_TRATTAMENTO_ANTIPARASSITARIO & "," & LAVCOD_TRATTAMENTO_FITOREGOLATORE & "," &
                              LAVCOD_GEODISINFESTAZIONE & "," & LAVCOD_CONCIA_SEME & "," & LAVCOD_DISSECCAMENTO & "," &
                              LAVCOD_CONFUSIONE_SESSUALE & "," & LAVCOD_DISORIENTAMENTO_SESSUALE & "," &
                              LAVCOD_INSTALLAZIONE_TRAPPOLE & "," & LAVCOD_CATTURE_MASSA & "," &
                              LAVCOD_DISTRIBUZIONE_CONCIME & "," & LAVCOD_FERTIRRIGAZIONE & "," & LAVCOD_TRATTAMENTO_ANTIBUTTERATURA & "," &
                              LAVCOD_CONCIMAZIONE_FOGLIARE & "," & LAVCOD_DISTRIBUZIONE_AMMENDANTI & "," & LAVCOD_SARCHIATURA_CONCIMAZIONE & "," &
                              LAVCOD_IRRIGAZIONE & "," &
                              LAVCOD_ARATURA & "," & LAVCOD_DEFOGLIAZIONE & "," & LAVCOD_ANDANAMENTO & "," &
                              LAVCOD_ASPORTAZIONE_ORGANI_INFETTI & "," & LAVCOD_ASSOLCATURA & "," & LAVCOD_CARICO_MANUALE_FRUTTA & "," &
                              LAVCOD_CIMATURA & "," & LAVCOD_DIRADAMENTO_MANUALE & "," & LAVCOD_DISSODAMENTO & "," &
                              LAVCOD_ERPICATURA & "," & LAVCOD_ERPICATURA_ROTANTE & "," & LAVCOD_ESPIANTO & "," &
                              LAVCOD_ESTIRPATURA & "," & LAVCOD_FALCIACONDIZIONATURA & "," & LAVCOD_FALCIATURA_ERBAI & "," &
                              LAVCOD_FORMAZIONE_ARGINELLI & "," & LAVCOD_FRANGIZOLLATURA & "," &
                              LAVCOD_FRESATURA & "," & LAVCOD_GEBIATURA & "," & LAVCOD_IMBALLO_FIENO_ROTOLI & "," & LAVCOD_INTERRAMENTO_PAGLIE & "," &
                              LAVCOD_INTERVENTO_ANTIBRINA & "," & LAVCOD_LAVORAZIONE_CONBINATA & "," &
                              LAVCOD_LAVORAZIONE_TRA_FILA & "," & LAVCOD_LAVORAZIONE_SU_FILA & "," & LAVCOD_LEGATURA & "," & LAVCOD_LIVELLAMENTO & "," &
                              LAVCOD_MANUTENZIONE_ARGINI & "," & LAVCOD_MESSA_DIMORA_PIANTE & "," & LAVCOD_MIETITREBBIATURA & "," & LAVCOD_MINIMUM_TILLAGE & "," &
                              LAVCOD_PACCIAMATURA & "," & LAVCOD_POTATURA_SECCA & "," & LAVCOD_POTATURA_VERDE & "," & LAVCOD_PRESSATURA & "," & LAVCOD_RACCOLTA_LEGNA_POTATURA & "," &
                              LAVCOD_RANGHINATURA & "," & LAVCOD_RINCALZATURA & "," & LAVCOD_RIPPATURA & "," & LAVCOD_RIPUNTATURA & "," & LAVCOD_RIVOLTAMENTO_FORAGGIO & "," &
                              LAVCOD_ROMPICROSTA & "," & LAVCOD_RULLATURA & "," & LAVCOD_SARCHIATURA & "," & LAVCOD_SCARIFICATURA & "," & LAVCOD_SCASSO & "," &
                              LAVCOD_SOD_SEDDING & "," & LAVCOD_TRINCIATURA & "," & LAVCOD_VANGATURA & "," & LAVCOD_ZAPPATURA & "," &
                              LAVCOD_SEMINA & "," & LAVCOD_SOVESCIO & "," & LAVCOD_TRAPIANTO & "," & LAVCOD_RILIEVO_AVVERSITA_CAMPO & ")"
        End Select

        ComboOperazione.CaricaComboLavorazioni(True, Filtro)

        Crea_Griglia_Operazioni()

        Dim ArrayPannelli(0) As enum_TipoPannello
        ArrayPannelli(0) = enum_TipoPannello.Nessun_Pannello
        Imposta_Pannelli(ArrayPannelli)
        '---------------------------------------

        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Note,
                                                        False, "", "",
                                                         0, -1,
                                                         "", "",
                                                         objParametri_Server)

        Me.Btn_FiltraImpianti.Visible = False
        Me.Btn_FiltraInterventi.Visible = False

        cella_operazione.Visible = False
        Me.GridView_Operazioni.Columns(0).Visible = False

        Btn_Stampa_Cert.Visible = False
        Btn_Stampa_RicAz.Visible = False
        Btn_Salva_Stampa_RicAz.Visible = False

        Select Case Qs_Tipo_Ricetta
            Case enum_TipoRicetta.Standard_Destinazioni, enum_TipoRicetta.Standard_Destinazioni_Planning
                Chk_RicettaPubblica.Enabled = False
            Case Else
                Chk_RicettaPubblica.Enabled = True
        End Select

        Cmb_Specie.Enabled = False


        '---------------------------------------
        Select Case CInt(Qs_Operazione)

            Case enum_TipoOperazioneDB.Scrittura

                cella_operazione.Visible = True

                If Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni OrElse Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni_Planning Then
                    Btn_Salva_Stampa_RicAz.Visible = True
                End If

                'Inizializzo il codice della ricetta
                Select Case Qs_Tipo_Ricetta
                    Case enum_TipoRicetta.PianoDistribuzioneConcimi
                        Dim dt_elenco_conProgCod As DataTable = New AgronicaCoreContabDAL.Ricette_R().Leggi(0, Qs_Piva, 0, enum_TipoRicetta.PianoDistribuzioneConcimi, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "Programmazione_Cod =" & CStr(ViewState("Programmazione_Cod")) & "", "", objParametri_Server)
                        If Not IsNothing(dt_elenco_conProgCod) AndAlso dt_elenco_conProgCod.Rows.Count > 0 Then
                            Txt_Ricetta_Numero.Text = CStr(ViewState("Programmazione_Cod")) & "_" & CStr(dt_elenco_conProgCod.Rows.Count + 1)
                        Else
                            Txt_Ricetta_Numero.Text = CStr(ViewState("Programmazione_Cod")) & "_1"
                        End If
                    Case enum_TipoRicetta.Standard_Destinazioni
                        'Grilli 10/05/2018 Su indicazione di Fabrizio propongo di default l'anno + un progressivo
                        Dim dt_elenco As DataTable = New AgronicaCoreContabDAL.Ricette_R().Leggi(0, Qs_Piva, 0, enum_TipoRicetta.Standard_Destinazioni, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 1, 1)) & " AND Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 12, 31)), "", objParametri_Server)
                        If Not IsNothing(dt_elenco) AndAlso dt_elenco.Rows.Count > 0 Then
                            Txt_Ricetta_Numero.Text = Date.Now.Year & "_" & CStr(dt_elenco.Rows.Count + 1)
                        Else
                            Txt_Ricetta_Numero.Text = Date.Now.Year & "_1"
                        End If
                End Select

            Case enum_TipoOperazioneDB.Lettura

                Me.Btn_Salva.Visible = False

                Me.Btn_Nuovo_Consiglio.Visible = False

                Me.GridView_Operazioni.Columns(7).Visible = False
                Me.GridView_Operazioni.Columns(8).Visible = False

                Btn_Stampa_RicAz.Visible = True
                Btn_Stampa_Cert.Visible = True


            Case enum_TipoOperazioneDB.Modifica

                cella_operazione.Visible = True

                Me.Btn_Salva.Text = "MODIFICA"

                Btn_Salva_Stampa_RicAz.Visible = True
                Btn_Stampa_RicAz.Visible = True
                Btn_Stampa_Cert.Visible = True


                '--------------------------------------------------------------------
                '------------------MODIFICA PER COPIA RICETTA------------------------
                '--------------------------------------------------------------------
            Case enum_TipoOperazioneDB.Copia
                '--------------------------------------------------------------------
                '-------------FINE MODIFICA PER COPIA RICETTA------------------------
                '--------------------------------------------------------------------

            Case enum_TipoOperazioneDB.Cancellazione

                Me.Btn_Salva.Text = "ELIMINA"

                Me.Btn_Nuovo_Consiglio.Visible = False

                Me.GridView_Operazioni.Columns(6).Visible = False
                Me.GridView_Operazioni.Columns(7).Visible = False
                Me.GridView_Operazioni.Columns(8).Visible = False

            Case enum_TipoOperazioneDB.Trasferimento

                SelezionaTab(Script_Panel, 1)

                Me.GridView_Operazioni.Columns(0).Visible = True
                Me.GridView_Operazioni.Columns(6).Visible = False
                Me.GridView_Operazioni.Columns(7).Visible = False
                Me.GridView_Operazioni.Columns(8).Visible = False

                Me.Btn_Salva.Visible = False

                Me.Btn_Nuovo_Consiglio.Visible = False

                Me.Btn_FiltraImpianti.Visible = True

                'Dim strFiltroImpianti As String

                If objParametriAgenda.Impianti.Count > 0 Then
                    Carica_Impianti()
                    ArrayPannelli(0) = enum_TipoPannello.Nessun_Pannello
                    If GridView_Impianti.Rows.Count > 0 Then
                        ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                        ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
                    End If
                    Imposta_Pannelli(ArrayPannelli)
                End If

                'If Not Session("VariabiliFiltro") Is Nothing Then

                '    strFiltroImpianti = Session("VariabiliFiltro").ToString
                '    Session("VariabiliFiltro") = Nothing

                '    Dim ArrayPannelli1(0) As enum_TipoPannello
                '    ArrayPannelli1(0) = enum_TipoPannello.Pannello_Impianti
                '    Imposta_Pannelli(ArrayPannelli1)

                'End If

                'Carica_Impianti(strFiltroImpianti, Nothing)

        End Select 'operazione

        '-------------------------------------
        Caricaddl_tipo_irrigazione()
        Lbl_Id_Agenda.Text = "0"

        Select Case CInt(Qs_Ricetta_Cod)

            Case 0

                'SImone Galassi Se sono i piani di distribuzione prendo la data salvata
                If (Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                    Me.Txt_DataInizio.Text = Qs_Data_Inizio
                    Me.Txt_DataFine.Text = Qs_Data_Fine
                Else
                    Me.Txt_DataInizio.Text = Now.ToShortDateString
                    Me.Txt_DataFine.Text = Now.ToShortDateString
                End If

                CaricaListControl.SpecieVegetale_Optimize(Cmb_Specie, True, "", "", 0, "", True, 0, 0, 0, "", "", objParametri_Server, objParametri_Utenti)

                If Qs_Veg_Cod IsNot Nothing AndAlso Qs_Veg_Cod <> "0" Then

                    Me.Cmb_Specie.SelectedIndex =
                            Cmb_Specie.Items.IndexOf(Cmb_Specie.Items.FindByValue(
                                Qs_Veg_Cod))

                    Cmb_Specie.Enabled = True
                    If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                        '  Marco Grilli, 13/04/2016 10:41:59: Il titolo della ricetta se è passato per parametro, lo metto
                        Me.Txt_Ricetta_Des.Text = If(Qs_Ricetta_Des <> "", Qs_Ricetta_Des, "Piano Distribuzione " & Me.Cmb_Specie.SelectedItem.Text & " (" & Me.Txt_DataInizio.Text & ")")
                    Else
                        '  Marco Grilli, 13/04/2016 10:41:59: Il titolo della ricetta se è passato per parametro, lo metto
                        Me.Txt_Ricetta_Des.Text = If(Qs_Ricetta_Des <> "", Qs_Ricetta_Des, Me.Cmb_Specie.SelectedItem.Text & " (" & Me.Txt_DataInizio.Text & ")")
                    End If


                    If Qs_Cul_Cod IsNot Nothing AndAlso Qs_Cul_Cod <> "" Then

                        Me.RBL_Cultivar.SelectedValue = "1"
                        Me.btn_Specie_Click(Me, Nothing)

                        Dim ArrayCulCod() As String
                        ArrayCulCod = Split(Qs_Cul_Cod, ",")

                        If ArrayCulCod IsNot Nothing Then
                            For i = 0 To ArrayCulCod.Length - 1
                                For j = 0 To Me.CBL_Cultivar.Items.Count - 1
                                    If Me.CBL_Cultivar.Items(j).Value = ArrayCulCod(i) Then
                                        Me.CBL_Cultivar.Items(j).Selected = True
                                        Exit For
                                    End If
                                Next
                            Next
                        End If

                    End If

                End If

                CaricaGriglia_Dosi_Concimazione()

                CaricaGriglia_Dosi_Difesa()

                '-------------------------------------------------------------------------
                'CREAZIONE RICETTA DA OPERAZIONI già registrate
                '-------------------------------------------------------------------------
                If Session("FiltroAgenda") IsNot Nothing Then

                    '--------------------------------------------------------
                    'nascondo alcuni controlli
                    Me.Btn_Nuovo_Consiglio.Visible = False

                    Me.Btn_Salva.Text = "CONFERMA"

                    Me.GridView_Operazioni.Columns(6).Visible = False
                    Me.GridView_Operazioni.Columns(7).Visible = False
                    Me.GridView_Operazioni.Columns(8).Visible = False

                    '--------------------------------------------------------
                    'imposto i default precedenti di data, specie, varieta

                    If Qs_Data_Inizio IsNot Nothing AndAlso Qs_Data_Inizio <> AGRODATAINIZIO.ToString Then
                        Me.Txt_DataInizio.Text = Qs_Data_Inizio
                    End If

                    If Qs_Data_Fine IsNot Nothing AndAlso Qs_Data_Fine <> AGRODATAFINE.ToString Then
                        Me.Txt_DataFine.Text = Qs_Data_Fine
                    End If

                    If Qs_Data_Inizio = Qs_Data_Fine Then
                        Me.RBL_Data.SelectedValue = "0"
                    Else
                        Me.RBL_Data.SelectedValue = "1"
                    End If

                    '--------------------------------------------------------
                    'imposto i dettagli delle operazioni
                    Dim strXMLInterventi As String = ""

                    strXMLInterventi = Session("FiltroAgenda")

                    Crea_Ricetta_da_Interventi(strXMLInterventi)
                    Select Case CInt(Qs_Operazione)
                        Case enum_TipoOperazioneDB.Scrittura
                            'SelezionaNoteDiDefault()
                    End Select


                    Session("FiltroAgenda") = Nothing

                Else

                    '-------------------------------------------------------------------------
                    'CARICAMENTO IMPIANTI 
                    '-------------------------------------------------------------------------
                    Select Case CInt(Qs_Operazione)

                        Case enum_TipoOperazioneDB.Scrittura

                            Dim strFiltroImpianti As String

                            If Session("VariabiliFiltro") IsNot Nothing Then

                                SelezionaTab(Script_Panel, 1)

                                strFiltroImpianti = Session("VariabiliFiltro").ToString
                                Session("VariabiliFiltro") = Nothing

                                Dim ArrayPannelli1(0) As enum_TipoPannello
                                ArrayPannelli1(0) = enum_TipoPannello.Pannello_Impianti
                                Imposta_Pannelli(ArrayPannelli1)

                            End If

                            Carica_Impianti(strFiltroImpianti, Nothing)



                    End Select

                End If


            Case Else

                'se provengo dalle operazioni d'agenda non leggo la ricetta da DB
                'ma la leggo dalla stringa passata in Web_ComunicazionePagine
                If Qs_Unid_Ricetta = "" Then

                    Dim StringaXmlRicetta As String

                    '--------------------------------------------------
                    '----- RICETTA

                    'Creo gli oggetti COM+
                    Dim objCOM As New AgronicaCoreContabBIZ.Ricette_R

                    'Recupero le richieste salvate dall'utente		
                    StringaXmlRicetta = objCOM.Ricetta_Leggi(CInt(Qs_Ricetta_Cod),
                                                             "",
                                                             0,
                                                             0,
                                                             0,
                                                             False,
                                                             objParametri_Server)


                    objCOM = Nothing

                    Carica_Ricetta(StringaXmlRicetta)


                    If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                        'Estraggo il programmazione_cod=PC_Testata_cod
                        Dim XmlDoc As New XmlDocument
                        XmlDoc.LoadXml(StringaXmlRicetta)
                        Dim XML_Ricetta As XmlElement = XmlDoc.SelectSingleNode("DatiRicetta/Ricetta")
                        Dim prg_cod As Integer = XML_Ricetta.GetAttribute("programmazione_cod")

                        'Carico gli impianti
                        If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                            Dim Dt_Impianti As DataTable = ViewState("dt_Impianti")
                            If IsNothing(Dt_Impianti) Then
                                Dt_Impianti = getDtImpiantiFromPianoConimazione(prg_cod)
                                ViewState("dt_Impianti") = Dt_Impianti
                            End If
                            Carica_Impianti("", Dt_Impianti)
                        End If

                        Dim ArrayPannelli1(0) As enum_TipoPannello
                        ArrayPannelli1(0) = enum_TipoPannello.Pannello_Impianti
                        Imposta_Pannelli(ArrayPannelli1)

                    End If


                End If

        End Select


        'se provengo dall'agenda devo ricaricare i controlli
        Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
        Dim objWebC As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_R
        Dim DtWebC As DataTable

        Dim objRicetteOpR As New AgronicaCoreContabBIZ.Ricette_Operazioni_R

        'dati ricetta
        If Qs_Unid_Ricetta <> "" Then

            Dim StringaXmlRicetta As String

            DtWebC = objWebC.Leggi(Qs_Unid_Ricetta, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            objWebW.Cancella(Qs_Unid_Ricetta, 0, "", objParametri_Server)

            If DtWebC.Rows.Count > 0 Then

                SelezionaTab(Script_Panel, 1)

                StringaXmlRicetta = DtWebC.Rows(0).Item("Stringa_Parametri_Base")
                If StringaXmlRicetta <> "" Then
                    Carica_Ricetta(StringaXmlRicetta)
                End If

            End If

        End If

        'Dati OPERAZIONE
        If Qs_Unid_Operazione <> "" Then

            Dim StringaXmlOperazioni As String
            Dim StringaXmlOperazione As String
            Dim XmlConsiglio As String
            Dim Lav_Cod As Integer
            Dim Des_Lib As String
            Dim Data As Date
            Dim Note As String
            Dim ArrayAgende() As String

            DtWebC = objWebC.Leggi(Qs_Unid_Operazione, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            objWebW.Cancella(Qs_Unid_Operazione, 0, "", objParametri_Server)

            If DtWebC.Rows.Count > 0 Then

                SelezionaTab(Script_Panel, 1)

                StringaXmlOperazioni = DtWebC.Rows(0).Item("Stringa_Parametri_Base")
                If StringaXmlOperazioni <> "" Then
                    ArrayAgende = Split(StringaXmlOperazioni, "~")
                    If ArrayAgende IsNot Nothing AndAlso ArrayAgende.Length > 0 Then
                        For i = 0 To ArrayAgende.Length - 1
                            StringaXmlOperazione = ArrayAgende(i)
                            If StringaXmlOperazione <> "" Then
                                XmlConsiglio = objRicetteOpR.XML_GeneraStringa_Ricetta_Operazione(Qs_Ricetta_Cod, Qs_Tipo_Ricetta, objParametri_Server.PivaSuperUser, Session("ASG_ProgressivoGIAS"), StringaXmlOperazione, Lav_Cod, Des_Lib, Data, Note, objParametri_Server)
                                Inserisci_Operazione(0, Des_Lib, Data.ToShortDateString, Lav_Cod, XmlConsiglio, "0")
                            End If
                        Next
                    End If
                End If
            End If

        End If


        Abilita_Disabilita_Data()

        '--------------------------------------------------------------------
        '------------------MODIFICA PER COPIA RICETTA------------------------
        '--------------------------------------------------------------------
        If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Copia Then
            'sblocco le date
            Me.Txt_DataInizio.Enabled = True
            Me.Txt_DataFine.Enabled = True
            Me.RBL_Data.Enabled = True
            Dim messaggio As String = "Attenzione! Si sta procedendo con la copia di una ricetta, nel caso venga cambiata la data occorre ricontrollare le varie operazioni dato che i controlli sui disciplinari e sui prodotti si riferiscono alla data precedente."
            Messaggi.AgroMsgBox(messaggio, Page, , Script_Panel)
        End If
        '--------------------------------------------------------------------
        '-------------FINE MODIFICA PER COPIA RICETTA------------------------
        '--------------------------------------------------------------------

        If Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni_Planning Then
            GestisciComboPianificazioni(False)
        Else
            cella_planning.Visible = False
        End If

        'Grilli 10/05/2018: su richiesta di Fabrizio nascondo o preimposto controlli
        If Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni Then
            Chk_RicettaPubblica.Style.Add("display", "none")
            Lbl_Specie.Style.Add("display", "none")
            Pnl_Specie.Style.Add("display", "none")
            Lbl_Ricetta_Des.Style.Add("display", "none")
            Txt_Ricetta_Des.Style.Add("display", "none")
            RBL_Cultivar.Style.Add("display", "none")
            Pnl_CBL_Note.Style.Add("display", "none")
        End If

    End Sub

    Private Sub Carica_Ricetta(ByVal StringaXmlRicetta As String)

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_DatiRicetta As System.Xml.XmlElement
        Dim XML_Ricetta As System.Xml.XmlElement

        Dim XML_DatiRicettaxCultivar As System.Xml.XmlElement
        Dim XML_Cultivar As System.Xml.XmlElement
        Dim XMLs_Cultivar As System.Xml.XmlNodeList

        Dim XML_DatiRicettaxNote As System.Xml.XmlElement
        Dim XML_Note As System.Xml.XmlElement
        Dim XMLs_Note As System.Xml.XmlNodeList

        Dim XML_DatiOperazioni As System.Xml.XmlElement
        Dim XML_Operazione As System.Xml.XmlElement
        Dim XMLs_Operazione As System.Xml.XmlNodeList

        Dim Ricetta_Operazione_Cod As Integer
        Dim Ricetta_Operazione_Des As String
        Dim Ricetta_Operazione_Data As String
        Dim Lav_Cod As Integer

        Dim Piva As String
        Dim Veg_Cod, Cul_Cod, Nota_Cod As Integer

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(StringaXmlRicetta)

        '----- Tag DatiRicetta

        XML_DatiRicetta = XmlDoc.SelectSingleNode("DatiRicetta")

        '----- Tag Ricetta

        XML_Ricetta = XML_DatiRicetta.SelectSingleNode("Ricetta")

        Txt_Ricetta_Numero.Text = CStr(XML_Ricetta.GetAttribute("ricetta_numero"))
        Me.Lbl_Ricetta_Cod.Text = CStr(XML_Ricetta.GetAttribute("ricetta_cod"))
        Me.Lbl_Ricetta_Cod.Visible = True

        Me.Txt_Ricetta_Des.Text = CStr(XML_Ricetta.GetAttribute("ricetta_des"))
        Me.Txt_Note.Text = CStr(XML_Ricetta.GetAttribute("note"))

        If CDate(XML_Ricetta.GetAttribute("validita_inizio")).ToShortDateString <> "01/01/1900" Then
            Me.Txt_DataInizio.Text = CDate(XML_Ricetta.GetAttribute("validita_inizio")).ToShortDateString
        End If

        If CDate(XML_Ricetta.GetAttribute("validita_fine")).ToShortDateString <> "31/12/2100" Then
            Me.Txt_DataFine.Text = CDate(XML_Ricetta.GetAttribute("validita_fine")).ToShortDateString
        End If

        ViewState("Programmazione_Cod") = XML_Ricetta.GetAttribute("programmazione_cod")

        Veg_Cod = CInt(XML_Ricetta.GetAttribute("veg_cod"))
        Piva = XML_Ricetta.GetAttribute("piva")
        ViewState("Piva") = Piva

        If Piva = "" Then
            Me.Chk_RicettaPubblica.Checked = True
        Else
            Me.Chk_RicettaPubblica.Checked = False
        End If

        CaricaListControl.SpecieVegetale_Optimize(Cmb_Specie, True, "", "", 0, "", True, Veg_Cod, 0, 0, "", "", objParametri_Server, objParametri_Utenti)


        Me.Cmb_Specie.SelectedIndex =
            Cmb_Specie.Items.IndexOf(Cmb_Specie.Items.FindByValue(
                Veg_Cod))


        '--------------------------------------------------
        '----- VERIETA'

        '----- Tag DatiRicettaxCultivar

        XML_DatiRicettaxCultivar = XML_Ricetta.SelectSingleNode("DatiRicettaxCultivar")

        '----- Tag Ricetta_Operazione (multiplo)

        If XML_DatiRicettaxCultivar IsNot Nothing AndAlso XML_DatiRicettaxCultivar.HasChildNodes Then

            XMLs_Cultivar = XML_DatiRicettaxCultivar.GetElementsByTagName("RicettaxCultivar")

            For i = 0 To XMLs_Cultivar.Count - 1

                XML_Cultivar = XMLs_Cultivar.Item(i)

                Cul_Cod = CInt(XML_Cultivar.GetAttribute("cul_cod"))

                'Cul_Cod = -1 ---> TUTTE LE VARIETA'
                If Cul_Cod = -1 Then

                    Me.RBL_Cultivar.SelectedValue = "0"
                    ' Me.CBL_Cultivar.Visible = False

                Else

                    Me.RBL_Cultivar.SelectedValue = "1"
                    'Me.CBL_Cultivar.Visible = True

                    'carico le varietà la prima volta...
                    If i = 0 Then
                        CaricaListControl.Cultivar(CBL_Cultivar, False, "", "", Veg_Cod, 0, "", True, 0, 0, "", "", objParametri_Server, objParametri_Utenti)
                    End If

                    For j = 0 To CBL_Cultivar.Items.Count - 1
                        If Cul_Cod = CInt(CBL_Cultivar.Items(j).Value) Then
                            CBL_Cultivar.Items(j).Selected = True
                            Exit For
                        End If
                    Next

                End If

            Next

        End If

        '--------------------------------------------------
        '----- NOTE

        '----- Tag DatiRicettaxNote

        XML_DatiRicettaxNote = XML_Ricetta.SelectSingleNode("DatiRicettaxNote")

        '----- Tag RicettaxNote (multiplo)

        If XML_DatiRicettaxNote IsNot Nothing AndAlso XML_DatiRicettaxNote.HasChildNodes Then

            XMLs_Note = XML_DatiRicettaxNote.GetElementsByTagName("RicettaxNote")

            For i = 0 To XMLs_Note.Count - 1

                XML_Note = XMLs_Note.Item(i)

                Nota_Cod = CInt(XML_Note.GetAttribute("nota_cod"))

                For j = 0 To CBL_Note.Items.Count - 1
                    If Nota_Cod = CInt(CBL_Note.Items(j).Value) Then
                        CBL_Note.Items(j).Selected = True
                        Exit For
                    End If
                Next

            Next

        End If

        '--------------------------------------------------
        '----- OPERAZIONI


        '----- Tag DatiRicetta_Operazioni

        XML_DatiOperazioni = XML_Ricetta.SelectSingleNode("DatiRicetta_Operazioni")

        If XML_DatiOperazioni IsNot Nothing Then

            '----- Tag Ricetta_Operazione (multiplo)

            XMLs_Operazione = XML_DatiOperazioni.GetElementsByTagName("Ricetta_Operazione")

            For i = 0 To XMLs_Operazione.Count - 1

                XML_Operazione = XMLs_Operazione.Item(i)

                Ricetta_Operazione_Cod = CInt(XML_Operazione.GetAttribute("ricetta_operazione_cod"))
                Ricetta_Operazione_Des = CStr(XML_Operazione.GetAttribute("ricetta_operazione_des"))
                Ricetta_Operazione_Data = CStr(XML_Operazione.GetAttribute("validita_inizio"))
                Lav_Cod = CInt(XML_Operazione.GetAttribute("lav_cod"))

                Inserisci_Operazione(Ricetta_Operazione_Cod,
                                     Ricetta_Operazione_Des,
                                     Ricetta_Operazione_Data,
                                     Lav_Cod,
                                     XML_Operazione.OuterXml,
                                     0)

            Next

        End If

    End Sub


    Private Sub Abilita_Disabilita_Data()
        Dim abilita As Boolean = False
        If GridView_Operazioni.Rows.Count > 0 Then
            abilita = False
        Else
            abilita = True
        End If
        Me.Txt_DataInizio.Enabled = abilita
        Me.Txt_DataFine.Enabled = abilita
        Me.RBL_Data.Enabled = abilita
    End Sub

    Private Sub Caricaddl_tipo_irrigazione()
        AgronicaCoreUtility.CaricaListControl.CaricaCombo_ImpIrrigazione(Cmb_ModifTipoIrrig, True,
                                                                         "-Quella Dell'impianto-", "-1", -1,
                                                                         "", "", "",
                                                                         HttpContext.Current.Session("ASG_objParametri_Server"))
    End Sub

    '########################################################################################
    Private Sub Inserisci_Operazione(ByVal Ricetta_Operazione_Cod As Integer,
                                     ByVal Ricetta_Operazione_Des As String,
                                     ByVal Ricetta_Operazione_data As String,
                                     ByVal Lav_Cod As Integer,
                                     ByVal Xml_Operazione As String,
                                     ByVal Id_Agenda As Integer)

        '----- Dimensiono le variabili
        Dim Dt As DataTable
        Dim Dr As DataRow

        'Recupero il datatable
        Dt = ViewState("dt_Operazioni")

        '----- Inserisco il nuovo record

        'Creo una nuova riga
        Dr = Dt.NewRow

        Dr.Item("Ricetta_Operazione_Cod") = Ricetta_Operazione_Cod
        Dr.Item("Ricetta_Operazione_Des") = Ricetta_Operazione_Des
        Dr.Item("Ricetta_Operazione_Data") = Ricetta_Operazione_data
        Dr.Item("Lav_Cod") = Lav_Cod
        Dr.Item("Xml_Operazione") = Xml_Operazione
        Dr.Item("Id_Agenda") = Id_Agenda

        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)

        '----- Associo il DataTable con la DataGrid
        GridView_Operazioni.DataSource = Dt
        GridView_Operazioni.DataBind()

        '----- Salvo il DataTable dentro il viewstate
        ViewState("dt_Operazioni") = Dt

        Me.Lbl_Id_Agenda.Text = "0"

        Abilita_Disabilita_Data()

        If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
            If Not IsNothing(ViewState("dt_Impianti")) Then
                Carica_Impianti("", ViewState("dt_Impianti"))
            End If

        End If

    End Sub

    Private Function GeneraStrutturaDTImpianti() As DataTable
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("campo_cod", GetType(Integer)))
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
        Dt.Columns.Add(New DataColumn("lotto", GetType(String)))
        Dt.Columns.Add(New DataColumn("descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("sup_imp", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("validita_fine", GetType(String)))

        '  Marco Grilli, 12/04/2016 09:40:28: Aggiunti per Piano Distribuzione
        Dt.Columns.Add(New DataColumn("N_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("N_Distribuito", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Distribuito", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Distribuito", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("N_Residuo", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Residuo", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Residuo", GetType(Decimal)))

        Return Dt

    End Function

    '###########################################################################
    Private Sub Carica_Impianti(Optional ByVal strFiltroImpianti As String = "", Optional ByVal DtImpianti As DataTable = Nothing)


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_FiltroRisultati As System.Xml.XmlElement
        Dim XMLs_Risultato As System.Xml.XmlNodeList
        Dim Xml_Risultato As System.Xml.XmlElement

        Dim Dr As DataRow
        Dim i As Integer

        '----- Definisco la struttura del DataTable
        Dim Dt As DataTable = GeneraStrutturaDTImpianti()

        If objParametriAgenda.Impianti IsNot Nothing AndAlso objParametriAgenda.Impianti.Count > 0 Then

            'Nel caso standard il DT viene letto da DB ed è da formattare
            For i = 0 To objParametriAgenda.Impianti.Count - 1

                Dr = Dt.NewRow

                Dr.Item("piva") = objParametriAgenda.Impianti(i).Piva
                Dr.Item("sa_cod") = objParametriAgenda.Impianti(i).Sa_Cod
                Dr.Item("campo_cod") = objParametriAgenda.Impianti(i).Campo_Cod
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
                'Dr.Item("Progetto_Cod") = objParametriAgenda.Impianti(i).Progetto_Cod
                Dr.Item("descrizione") = objParametriAgenda.Impianti(i).Veg_Des & " - " & objParametriAgenda.Impianti(i).Cul_Des
                Dr.Item("sup_imp") = objParametriAgenda.Impianti(i).Sup_Imp
                Dr.Item("validita_inizio") = IIf(objParametriAgenda.Impianti(i).Validita_Inizio.ToShortDateString <> "01/01/1900", objParametriAgenda.Impianti(i).Validita_Inizio.ToShortDateString, "...")
                Dr.Item("validita_fine") = IIf(objParametriAgenda.Impianti(i).Validita_Fine.ToShortDateString <> "31/12/2100", objParametriAgenda.Impianti(i).Validita_Fine.ToShortDateString, "...")

                Dr.Item("lotto") = "" 'Xml_Risultato.GetAttribute("lotto")

                Dr.Item("N_Max") = 0
                Dr.Item("P_Max") = 0
                Dr.Item("K_Max") = 0
                Dr.Item("N_Distribuito") = 0
                Dr.Item("P_Distribuito") = 0
                Dr.Item("K_Distribuito") = 0
                Dr.Item("N_Residuo") = 0
                Dr.Item("P_Residuo") = 0
                Dr.Item("K_Residuo") = 0

                Dt.Rows.Add(Dr)
            Next

        Else

            If strFiltroImpianti <> "" Then

                'Carico la stringa nel documento XML
                XmlDoc.LoadXml(strFiltroImpianti)
                Xml_FiltroRisultati = XmlDoc.SelectSingleNode("FiltroRisultati")
                XMLs_Risultato = Xml_FiltroRisultati.GetElementsByTagName("Risultato")

                For i = 0 To XMLs_Risultato.Count - 1

                    Xml_Risultato = XMLs_Risultato.Item(i)

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    'Definisco i valori
                    Dr.Item("piva") = Xml_Risultato.GetAttribute("piva")
                    Dr.Item("sa_cod") = Xml_Risultato.GetAttribute("sa_cod")
                    Dr.Item("campo_cod") = Xml_Risultato.GetAttribute("campo_cod")
                    Dr.Item("appezza") = Xml_Risultato.GetAttribute("appezza")
                    Dr.Item("id_reg") = Xml_Risultato.GetAttribute("id_reg")
                    Dr.Item("veg_cod") = Xml_Risultato.GetAttribute("veg_cod")
                    Dr.Item("cul_cod") = Xml_Risultato.GetAttribute("cul_cod")
                    Dr.Item("grfi_cod") = Xml_Risultato.GetAttribute("grfi_cod")
                    Dr.Item("regolamento") = Xml_Risultato.GetAttribute("regolamento")
                    Dr.Item("finanziamento") = Xml_Risultato.GetAttribute("finanziamento")
                    Dr.Item("rag_soc") = Xml_Risultato.GetAttribute("rag_soc")
                    Dr.Item("sa_nome") = Xml_Risultato.GetAttribute("sa_nome")
                    Dr.Item("app_nome") = Xml_Risultato.GetAttribute("app_nome")
                    Dr.Item("lotto") = Xml_Risultato.GetAttribute("lotto")
                    Dr.Item("descrizione") = Xml_Risultato.GetAttribute("descrizione")
                    Dr.Item("sup_imp") = Xml_Risultato.GetAttribute("sup_imp")
                    Dr.Item("validita_inizio") = IIf(CDate(Xml_Risultato.GetAttribute("validita_inizio")).ToShortDateString <> "01/01/1900", CDate(Xml_Risultato.GetAttribute("validita_inizio")).ToShortDateString, "...")
                    Dr.Item("validita_fine") = IIf(CDate(Xml_Risultato.GetAttribute("validita_fine")).ToShortDateString <> "31/12/2100", CDate(Xml_Risultato.GetAttribute("validita_fine")).ToShortDateString, "...")

                    Dr.Item("N_Max") = IIf(String.IsNullOrEmpty(Xml_Risultato.GetAttribute("max_n")), 0, Xml_Risultato.GetAttribute("max_n"))
                    Dr.Item("P_Max") = IIf(String.IsNullOrEmpty(Xml_Risultato.GetAttribute("max_p")), 0, Xml_Risultato.GetAttribute("max_p"))
                    Dr.Item("K_Max") = IIf(String.IsNullOrEmpty(Xml_Risultato.GetAttribute("max_k")), 0, Xml_Risultato.GetAttribute("max_k"))
                    Dr.Item("N_Distribuito") = 0
                    Dr.Item("P_Distribuito") = 0
                    Dr.Item("K_Distribuito") = 0
                    Dr.Item("N_Residuo") = Dr.Item("N_Max")
                    Dr.Item("P_Residuo") = Dr.Item("P_Max")
                    Dr.Item("K_Residuo") = Dr.Item("K_Max")

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                Next

            Else



                If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then


                    'Se non ci sono le colonne per i piani concimazione, le aggiungo io
                    For Each nomeCol As String In {"N_Max", "P_Max", "K_Max", "N_Distribuito", "P_Distribuito", "K_Distribuito", "N_Residuo", "P_Residuo", "K_Residuo"}
                        If Not DtImpianti.Columns.Contains(nomeCol) Then
                            Dim col As New DataColumn(nomeCol, GetType(System.Decimal))
                            col.AllowDBNull = True
                            col.DefaultValue = System.DBNull.Value
                            DtImpianti.Columns.Add(col)
                        End If
                    Next


                    If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                        'Se sono in distribuzione concime il DT è già ben formato e quindi lo assegno
                        Dt = ViewState("dt_Impianti")

                        'Sono arrivato in questa sezione perché è stata aggiunta un'operazione. Devo quindi aggiornare i valori di NPK distribuito e residuo

                        'Recupero il datatable delle operazioni
                        Dim dtOperazioni As DataTable = ViewState("dt_Operazioni")

                        'Inizializzo le variabili di conteggio
                        Dim N_Distribuito_Ha As Decimal = 0
                        Dim P_Distribuito_Ha As Decimal = 0
                        Dim K_Distribuito_Ha As Decimal = 0
                        'Dim N_Residuo_Ha As Decimal = 0
                        'Dim P_Residuo_Ha As Decimal = 0
                        'Dim K_Residuo_Ha As Decimal = 0

                        'Imposto le variabili di conteggio in base alle operazioni
                        calcolaNPK(dtOperazioni, N_Distribuito_Ha, P_Distribuito_Ha, K_Distribuito_Ha)

                        'Modifico i valori dei residui
                        'N_Residuo_Ha = IIf(IsNothing(Xml_Risultato.GetAttribute("max_n")), 0, Xml_Risultato.GetAttribute("max_n")) - N_Distribuito_Ha
                        'P_Residuo_Ha = IIf(IsNothing(Xml_Risultato.GetAttribute("max_p")), 0, Xml_Risultato.GetAttribute("max_p")) - P_Distribuito_Ha
                        'K_Residuo_Ha = IIf(IsNothing(Xml_Risultato.GetAttribute("max_k")), 0, Xml_Risultato.GetAttribute("max_k")) - K_Distribuito_Ha

                        'Imposto gli NPK distribuiti e residui
                        For Each imp As DataRow In Dt.Rows
                            imp.Item("N_Distribuito") = N_Distribuito_Ha
                            imp.Item("P_Distribuito") = P_Distribuito_Ha
                            imp.Item("K_Distribuito") = K_Distribuito_Ha
                            imp.Item("N_Residuo") = CDec(imp.Item("N_Max")) - N_Distribuito_Ha 'N_Residuo_Ha
                            imp.Item("P_Residuo") = CDec(imp.Item("P_Max")) - P_Distribuito_Ha
                            imp.Item("K_Residuo") = CDec(imp.Item("K_Max")) - K_Distribuito_Ha
                        Next

                    Else
                        'Nel caso standard il DT viene letto da DB ed è da formattare
                        For i = 0 To DtImpianti.Rows.Count - 1

                            Dr = Dt.NewRow

                            Dr.Item("piva") = DtImpianti.Rows(i).Item("piva")
                            Dr.Item("sa_cod") = DtImpianti.Rows(i).Item("sa_cod")
                            Dr.Item("campo_cod") = DtImpianti.Rows(i).Item("campo_cod")
                            Dr.Item("appezza") = DtImpianti.Rows(i).Item("appezza")
                            Dr.Item("id_reg") = DtImpianti.Rows(i).Item("id_reg")
                            Dr.Item("veg_cod") = DtImpianti.Rows(i).Item("veg_cod")
                            Dr.Item("cul_cod") = DtImpianti.Rows(i).Item("cul_cod")
                            Dr.Item("grfi_cod") = DtImpianti.Rows(i).Item("grfi_cod")
                            Dr.Item("regolamento") = DtImpianti.Rows(i).Item("Regolamento")
                            Dr.Item("finanziamento") = DtImpianti.Rows(i).Item("finanziamento")
                            Dr.Item("rag_soc") = DtImpianti.Rows(i).Item("rag_soc")
                            Dr.Item("sa_nome") = DtImpianti.Rows(i).Item("sa_nome")
                            Dr.Item("app_nome") = DtImpianti.Rows(i).Item("app_nome")
                            Dr.Item("lotto") = DtImpianti.Rows(i).Item("progetto")
                            Dr.Item("descrizione") = DtImpianti.Rows(i).Item("veg_des") & " - " & DtImpianti.Rows(i).Item("cul_des")
                            Dr.Item("sup_imp") = DtImpianti.Rows(i).Item("sup_imp")
                            Dr.Item("validita_inizio") = IIf(CDate(DtImpianti.Rows(i).Item("validita_inizio")).ToShortDateString <> "01/01/1900", CDate(DtImpianti.Rows(i).Item("validita_inizio")).ToShortDateString, "...")
                            Dr.Item("validita_fine") = IIf(CDate(DtImpianti.Rows(i).Item("validita_fine")).ToShortDateString <> "31/12/2100", CDate(DtImpianti.Rows(i).Item("validita_fine")).ToShortDateString, "...")

                            Dt.Rows.Add(Dr)
                        Next
                    End If

                End If

            End If

        End If

        If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
            '  Marco Grilli, 12/04/2016 15:56:11: Salvo il DataTable nel GridView
            'Sono costretto perché la stringa iniziale viene cancellata e la lettura da DB non contempla i miei valori di NPK
            ViewState("dt_Impianti") = Dt
        End If


        '----- Associo il DataTable con la DataGrid
        GridView_Impianti.DataSource = Dt
        GridView_Impianti.DataBind()

        'di default seleziono tutti gli impianti..
        For i = 0 To Me.GridView_Impianti.Rows.Count - 1
            Dim lck As CheckBox = CType(GridView_Impianti.Rows(i).FindControl("ChkSelezionaImpianto"), CheckBox)
            lck.Checked = True

            'vanni, 16/05/2013 aggiunto enabled false
            If Not (Qs_Operazione = enum_TipoOperazioneDB.Trasferimento OrElse Qs_Operazione = enum_TipoOperazioneDB.Scrittura) Then
                lck.Enabled = False
            End If
        Next


    End Sub

    '########################################################################################
    Private Sub CaricaGriglia_Dosi_Difesa()

        '----- Definizione delle variabili
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Av_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Av_Gru", GetType(String)))
        Dt.Columns.Add(New DataColumn("Av_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Soglia_Value", GetType(String)))
        Dt.Columns.Add(New DataColumn("Soglia_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Carenza", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Etichetta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Etichetta_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Fittizia", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mezzo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta_Tot", GetType(String)))
        Dt.Columns.Add(New DataColumn("strPA_COD", GetType(String)))
        Dt.Columns.Add(New DataColumn("strCLTOSS_COD", GetType(String)))

        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(2) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Av_Cod")
        DtKeys(1) = Dt.Columns("Av_Gru")
        DtKeys(2) = Dt.Columns("Fr_Cod")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Associo il DataTable con la DataGrid

        GridView_Dosi_Difesa.DataSource = Dt
        GridView_Dosi_Difesa.DataBind()

        '----- Salvo il DataTable dentro il viewstate

        ViewState("dtDosi_Difesa") = Dt

    End Sub

    '########################################################################################
    Private Sub Imposta_Pannelli(ByVal Pannello() As enum_TipoPannello)

        Dim i As Integer
        Dim i_Pan_Imp As Integer


        Riga_Impianti.Visible = False

        Riga_Concimazione.Visible = False
        Riga_Irrigazione.Visible = False
        Riga_Trappole.Visible = False
        Riga_Lavorazioni.Visible = False
        Riga_Trattamento.Visible = False
        Riga_RilievoAvvAus.Visible = False


        'Riga_Costi.Visible = False

        'i_Pan_Imp = Pannello.IndexOf(Pannello, enum_TipoPannello.Pannello_Testata)
        'If i_Pan_Imp <> -1 Then
        '    Riga_Interventi.Visible = True
        'End If

        i_Pan_Imp = Pannello.IndexOf(Pannello, enum_TipoPannello.Pannello_Impianti)
        If i_Pan_Imp <> -1 Then
            Riga_Impianti.Visible = True
            Select Case Qs_Operazione
                Case enum_TipoOperazioneDB.Trasferimento
                    GridView_Impianti.Columns(0).Visible = True
                    cella_salva_operazione.Visible = True
                    'GridView_Operazioni.Columns(0).Visible = True
                Case enum_TipoOperazioneDB.Scrittura
                    GridView_Impianti.Columns(0).Visible = True
                    cella_salva_operazione.Visible = False
                    'GridView_Operazioni.Columns(0).Visible = False
                Case Else
                    'vanni, 16/05/2013 displaynone al posto di visible
                    'GridView_Impianti.Columns(0).visible = false
                    cella_salva_operazione.Visible = False
                    'GridView_Operazioni.Columns(0).Visible = False
            End Select
        End If


        i = Pannello.IndexOf(Pannello, enum_TipoPannello.PannelloConcimazione)
        If i <> -1 Then
            Riga_Concimazione.Visible = True
        End If

        i = Pannello.IndexOf(Pannello, enum_TipoPannello.PannelloIrrigazione)
        If i <> -1 Then
            Riga_Irrigazione.Visible = True
        End If

        i = Pannello.IndexOf(Pannello, enum_TipoPannello.PannelloTrappole)
        If i <> -1 Then
            Riga_Trappole.Visible = True
        End If

        i = Pannello.IndexOf(Pannello, enum_TipoPannello.PannelloLavorazioni)
        If i <> -1 Then
            Riga_Lavorazioni.Visible = True
        End If

        i = Pannello.IndexOf(Pannello, enum_TipoPannello.PannelloDifesa)
        If i <> -1 Then
            Riga_Trattamento.Visible = True
        End If

        i = Pannello.IndexOf(Pannello, enum_TipoPannello.Pannello_Costi)
        If i <> -1 Then
            Riga_Costi.Visible = True
        End If

        i = Pannello.IndexOf(Pannello, enum_TipoPannello.Pannello_RilievoAvvAus)
        If i <> -1 Then
            Riga_RilievoAvvAus.Visible = True
        End If

    End Sub


    '################################################################################################################
    Private Sub Crea_Griglia_Operazioni()

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Ricetta_Operazione_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Operazione_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ricetta_Operazione_Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Xml_Operazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))


        'Vettore di DataColumn
        Dim DtKeys(2) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Ricetta_Operazione_Cod")
        DtKeys(1) = Dt.Columns("Lav_Cod")
        DtKeys(2) = Dt.Columns("Xml_Operazione")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        ViewState("dt_Operazioni") = Dt

    End Sub

    '################################################################################################################
    Protected Sub btn_Specie_Click(sender As Object, e As EventArgs) Handles btn_Specie.Click


        Dim Veg_Cod As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim DataInizio As String
        Dim DataFine As String

        'azzero i controlli

        Dim ArrayPannelli(0) As enum_TipoPannello
        ArrayPannelli(0) = enum_TipoPannello.Nessun_Pannello
        Imposta_Pannelli(ArrayPannelli)

        Me.Txt_Ricetta_Des.Text = ""

        Me.RBL_Cultivar.SelectedValue = "0"
        'Me.CBL_Cultivar.Visible = False

        ComboOperazione.Valore_Combo = ""
        Cmb_Disciplinare.Items.Clear()

        Validita_Inizio = #1/1/1900#
        Validita_Fine = #12/31/2100#
        DataInizio = "..."
        DataFine = "..."

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            DataInizio = CDate(Txt_DataInizio.Text)
        End If
        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
            DataFine = CDate(Txt_DataFine.Text)
        End If

        If Me.Cmb_Specie.SelectedItem.Text <> "" Then

            Veg_Cod = CInt(Cmb_Specie.SelectedItem.Value)

            CaricaListControl.Cultivar(CBL_Cultivar, False, "", "", Veg_Cod, 0, "", True, 0, 0, "", "", objParametri_Server, objParametri_Utenti)

        End If

        If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
            Select Case Me.RBL_Data.SelectedValue
                Case "0"
                    Me.Txt_Ricetta_Des.Text = "Piano Distribuzione " & Me.Cmb_Specie.SelectedItem.Text & " (" & DataInizio & ")"
                Case Else
                    Me.Txt_Ricetta_Des.Text = "Piano Distribuzione " & Me.Cmb_Specie.SelectedItem.Text & " (" & DataInizio & "-" & DataFine & ")"
            End Select
        Else
            Select Case Me.RBL_Data.SelectedValue
                Case "0"
                    Me.Txt_Ricetta_Des.Text = "" & Me.Cmb_Specie.SelectedItem.Text & " (" & DataInizio & ")"
                Case Else
                    Me.Txt_Ricetta_Des.Text = "" & Me.Cmb_Specie.SelectedItem.Text & " (" & DataInizio & "-" & DataFine & ")"
            End Select
        End If

        'If Chk_RicettaPubblica.Checked = False Then
        '    Carica_Impianti()
        '    Dim ArrayPannelli1(1) As enum_TipoPannello
        '    ArrayPannelli1(0) = enum_TipoPannello.Nessun_Pannello
        '    ArrayPannelli1(0) = enum_TipoPannello.Pannello_Impianti
        '    Imposta_Pannelli(ArrayPannelli1)
        'End If


    End Sub


    Private Function ComboPianificazione_Get_COD() As Integer

        If ComboPianificazioni.Valore_Combo <> "" Then
            Return CInt(ComboPianificazioni.Valore_Combo)
        Else
            Return 0
        End If

    End Function

    '#####################################################################################################
    Protected Sub Btn_Nuovo_Consiglio_Click(sender As Object, e As EventArgs) Handles Btn_Nuovo_Consiglio.Click


        If Not (Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni OrElse Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni_Planning) Then

            Dim Validita_Inizio As Date
            Dim Validita_Fine As Date
            Dim DataInizio As String
            Dim DataFine As String
            Dim TipoTestata As Integer

            'azzero i controlli

            Dim ArrayPannelli(0) As enum_TipoPannello
            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
            If GridView_Impianti.Rows.Count > 0 Then
                ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
            End If
            Imposta_Pannelli(ArrayPannelli)

            Me.Cmb_Disciplinare.Items.Clear()
            cella_disciplinare.Visible = False

            Me.ComboEpoche.ddl_ComboEpocheFertilizzazione.Items.Clear()
            Me.Cmb_Epoca.Items.Clear()
            cella_epoca.Visible = False

            Validita_Inizio = #1/1/1900#
            Validita_Fine = #12/31/2100#
            DataInizio = "..."
            DataFine = "..."

            If Me.Txt_DataInizio.Text <> "" Then
                Validita_Inizio = CDate(Txt_DataInizio.Text)
                DataInizio = CDate(Txt_DataInizio.Text)
            End If
            If Me.Txt_DataFine.Text <> "" Then
                Validita_Fine = CDate(Txt_DataFine.Text)
                DataFine = CDate(Txt_DataFine.Text)
            End If

            If Cmb_Specie.SelectedItem.Text = "" Then
                Messaggi.AgroMsgBox("Selezionare la Specie Vegetale!", Page, , Script_Panel)
                Exit Sub
            End If

            Select Case CInt(ComboOperazione.Valore_Combo)

                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                    Select Case CInt(ComboOperazione.Valore_Combo)
                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                            TipoTestata = 0
                            Me.Lbl_Epoca.InnerText = "Modulo :"
                        Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                            TipoTestata = 1
                            Me.Lbl_Epoca.InnerText = "Epoca :"
                    End Select

                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(Validita_Inizio), CDate(Validita_Fine))
                    Dim objCaricaCombo As New AgronicaCoreDpiBIZ.CaricaListControl

                    objCaricaCombo.Disciplinari_ElencoxTestata(Me.Cmb_Disciplinare,
                                                                False, "", "",
                                                                Session,
                                                                objParametri_Server,
                                                                objParametri_Utenti,
                                                                CInt(0),
                                                                CInt(Cmb_Specie.SelectedValue),
                                                                CInt(0),
                                                                CInt(0),
                                                                Validita_Inizio,
                                                                Validita_Fine,
                                                                True, "",
                                                                False, "",
                                                                False,
                                                                TipoTestata)
                    objParametri_Server.ResettaFinestra()


                    Me.btn_Disciplinari_Click(Me, Nothing)

                    cella_disciplinare.Visible = True


                Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                    ComboEpoche.Veg_Cod = CInt(Cmb_Specie.SelectedValue)

                    ComboEpoche.PrimaRiga_Flag = True
                    ComboEpoche.PrimaRiga_Text = ""
                    ComboEpoche.PrimaRiga_Value = "0"

                    ComboEpoche.CaricaComboEpocheFertilizzazione()

                    ComboEpoche.Visible = True
                    cella_epoca.Visible = True
                    Cmb_Epoca.Visible = False

                Case Else

                    Cmb_Disciplinare.Items.Clear()
                    Cmb_Disciplinare.Items.Add(New ListItem("Nessun Disciplinare", "0"))

            End Select

        End If







        'controllo che sia stata selezionata una data
        Dim dataOK As Boolean = False
        If IsDate(Txt_DataInizio.Text) Then
            dataOK = True
        End If
        If Txt_DataFine.Visible Then
            If IsDate(Txt_DataFine.Text) Then
                dataOK = True
            Else
                dataOK = False
            End If
        End If




        If Not dataOK Then
            Messaggi.AgroMsgBox("Specificare le date", Page, , Script_Panel)
            Exit Sub
        End If


        If Qs_Tipo_Ricetta <> enum_TipoRicetta.Standard_Destinazioni AndAlso Cmb_Specie.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox("Selezionare la specie!", Page, , Script_Panel)
            Exit Sub
        End If

        If Not IsNumeric(ComboOperazione.Valore_Combo) Then
            Messaggi.AgroMsgBox("Selezionare l'operazione!", Page, , Script_Panel)
            Exit Sub
        End If

        Cmb_Specie.Enabled = False


        Select Case Qs_Tipo_Ricetta

            Case TipiEnumerativi.enum_TipoRicetta.Standard_Destinazioni, enum_TipoRicetta.Standard_Destinazioni_Planning

                Dim TargetUrl As String

                objParametriAgenda.Id_Agenda = 0
                objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

                objParametriAgenda.Data = Txt_DataInizio.Text
                objParametriAgenda.Lav_Cod = ComboOperazione.Valore_Combo
                objParametriAgenda.Veg_Cod = If(Cmb_Specie.SelectedValue = "", 0, Cmb_Specie.SelectedValue)

                objParametriAgenda.TipoOperazioneAgenda = TipiEnumerativi.enum_Tipo_Operazione_Agenda.Ricetta

                objParametriAgenda.TipoRicetta = Qs_Tipo_Ricetta

                If ComboPianificazione_Get_COD() <> 0 Then
                    objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Planning
                Else
                    objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
                End If

                objParametriAgenda.Programmazione_Cod = ComboPianificazione_Get_COD()

                Dim strErrore As String = ""
                Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni 'Utility_NS.Utility_Operazioni
                TargetUrl = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda, enum_PagineAgenda_2010.Pagina_Ricette_Edit, LeggiFlagConfigurazioneSiti:=False, strErrore:=strErrore, fromBootstrapToBootstrap:=True)

                If strErrore <> "" Then
                    Messaggi.AgroMsgBox(strErrore, Page, , Script_Panel)
                    Exit Sub
                End If

                Dim Unid_Ricetta As String = System.Guid.NewGuid.ToString

                Dim StringaXmlCreazione As String = XML_GeneraStringa_Ricetta()

                Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
                Dim res As Boolean = False
                res = objWebW.Scrivi(Unid_Ricetta, 0, enum_TipoOperazioneDB.Scrittura, 0, "", "", StringaXmlCreazione, "", objParametri_Server)
                If res Then
                    Response.Redirect(TargetUrl & "?unid_ricetta=" & Stringa_Codifica(Unid_Ricetta, AgroKey_EncoderDecoder) &
                                                  "&operazione_ricetta=" & Stringa_Codifica(Qs_Operazione, AgroKey_EncoderDecoder) &
                                                  "&r=" & Stringa_Codifica(Qs_Ricetta_Cod, AgroKey_EncoderDecoder)
                                                  )
                End If

            Case Else

                Costruisci_DT_Scarico()
                Btn_Costi.Visible = True


                Select Case CInt(ComboOperazione.Valore_Combo)

                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                        ImpostaCmbFormulatoClassificazioni_By_LavCod()

                        Select Case CInt(ComboOperazione.Valore_Combo)
                            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                                RBL_Avversita.Items(0).Text = "Avversita'"
                                RBL_Avversita.Items(1).Text = "Gruppi Avversita'"
                                CaricaGriglia_Avversita()
                            Case LAVCOD_DISERBO
                                RBL_Avversita.Items(0).Text = "Infestanti"
                                RBL_Avversita.Items(1).Text = "Gruppi Infestanti"
                                CaricaGriglia_Avversita()
                            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_DISSECCAMENTO
                                CellaAvversita.Visible = False
                        End Select

                        CaricaGriglia_Dosi_Difesa()

                        DefaultAziendali(CBL_Consigli_Difesa, CInt(ComboOperazione.Valore_Combo))

                        Dim ArrayPannelli() As enum_TipoPannello

                        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
                            ReDim ArrayPannelli(2)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloDifesa
                            ArrayPannelli(2) = enum_TipoPannello.Pannello_Costi
                        Else
                            ReDim ArrayPannelli(1)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloDifesa
                        End If
                        If GridView_Impianti.Rows.Count > 0 Then
                            ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                            ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
                        End If

                        Imposta_Pannelli(ArrayPannelli)

                    Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                        LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                        Rbl_Dosi_Fertilizzanti.Items.Clear()

                        CaricaGriglia_Dosi_Concimazione()

                        DefaultAziendali(CBL_Consigli_Concimazione, CInt(ComboOperazione.Valore_Combo))

                        Dim ArrayPannelli() As enum_TipoPannello

                        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
                            ReDim ArrayPannelli(2)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloConcimazione
                            ArrayPannelli(2) = enum_TipoPannello.Pannello_Costi
                        Else
                            ReDim ArrayPannelli(1)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloConcimazione
                        End If

                        If GridView_Impianti.Rows.Count > 0 Then
                            ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                            ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
                        End If

                        Imposta_Pannelli(ArrayPannelli)

                        Select Case CInt(ComboOperazione.Valore_Combo)
                            Case LAVCOD_DISTRIBUZIONE_AMMENDANTI
                                Txt_N.Enabled = True
                                Txt_P2O5.Enabled = True
                                Txt_K2O.Enabled = True
                                Txt_MgO.Enabled = True
                                Txt_Efficienza.Enabled = True
                            Case Else
                                Txt_N.Enabled = False
                                Txt_P2O5.Enabled = False
                                Txt_K2O.Enabled = False
                                Txt_MgO.Enabled = False
                                Txt_Efficienza.Enabled = False
                        End Select

                        Select Case CInt(ComboOperazione.Valore_Combo)
                            Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI
                                Cella_Acqua.Visible = False
                                Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Dose/Ha", "1"))
                                'Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Qta Totale", "0"))
                            Case Else
                                Cella_Acqua.Visible = True
                                Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Dose/Ha", "1"))
                                Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Dose/Hl", "0"))
                        End Select
                        Rbl_Dosi_Fertilizzanti.SelectedValue = "1"


                        'LAVORAZIONi
                    Case LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ANDANAMENTO, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
                         LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE,
                         LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ERPICATURA_ROTANTE, LAVCOD_ESPIANTO, LAVCOD_ESTIRPATURA,
                         LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA,
                         LAVCOD_FRESATURA, LAVCOD_GEBIATURA, LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE,
                         LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_LAVORAZIONE_CONBINATA, LAVCOD_LAVORAZIONE_TRA_FILA,
                         LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI,
                         LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA,
                         LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA,
                         LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO,
                         LAVCOD_ROMPICROSTA, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO,
                         LAVCOD_SOD_SEDDING, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA, LAVCOD_SEMINA, LAVCOD_SOVESCIO,
                         LAVCOD_TRAPIANTO

                        'per semina/trapianto verifico la specie (eccetto per la barbabietola vedi agenda)
                        If CInt(Cmb_Specie.SelectedValue) <> 6 Then
                            Select Case CInt(ComboOperazione.Valore_Combo)
                                Case LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                                    Dim objGruppo As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                                    Dim Gru_Cod As String = objGruppo.GruCod_and_VegDes_from_VegCod("", CInt(Cmb_Specie.SelectedValue), objParametri_Server)
                                    If Gru_Cod = "1" Then
                                        Messaggi.AgroMsgBox("Non è possibile registrare una semina sulla specie selezionata!", Page, , Script_Panel)
                                        Exit Sub
                                    End If
                                Case LAVCOD_TRAPIANTO
                                    Dim objGruppo As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                                    Dim Gru_Cod As String = objGruppo.GruCod_and_VegDes_from_VegCod("", CInt(Cmb_Specie.SelectedValue), objParametri_Server)
                                    If Gru_Cod = "2" Then
                                        Messaggi.AgroMsgBox("Non è possibile registrare un trapianto sulla specie selezionata!", Page, , Script_Panel)
                                        Exit Sub
                                    End If
                            End Select
                        End If

                        'Me.ImgBtn_SalvaConsiglio_Lavorazioni.Visible = True

                        DefaultAziendali(CBL_Consigli_Lavorazioni, CInt(ComboOperazione.Valore_Combo))

                        Dim ArrayPannelli() As enum_TipoPannello

                        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
                            ReDim ArrayPannelli(2)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloLavorazioni
                            ArrayPannelli(2) = enum_TipoPannello.Pannello_Costi
                        Else
                            ReDim ArrayPannelli(1)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloLavorazioni
                        End If

                        If GridView_Impianti.Rows.Count > 0 Then
                            ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                            ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
                        End If

                        Imposta_Pannelli(ArrayPannelli)


                    Case LAVCOD_IRRIGAZIONE


                        DefaultAziendali(CBL_Consigli_Irrigazione, CInt(ComboOperazione.Valore_Combo))

                        Dim ArrayPannelli() As enum_TipoPannello

                        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
                            ReDim ArrayPannelli(2)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloIrrigazione
                            ArrayPannelli(2) = enum_TipoPannello.Pannello_Costi
                        Else
                            ReDim ArrayPannelli(1)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloIrrigazione
                        End If

                        If GridView_Impianti.Rows.Count > 0 Then
                            ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                            ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
                        End If

                        Imposta_Pannelli(ArrayPannelli)

                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA, LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                        Dim TrappolaUso As Integer

                        Select Case CInt(ComboOperazione.Valore_Combo)
                            Case LAVCOD_INSTALLAZIONE_TRAPPOLE
                                TrappolaUso = enum_TrappoleUso.Monitor
                            Case LAVCOD_CATTURE_MASSA
                                TrappolaUso = enum_TrappoleUso.CattureDiMassa
                            Case LAVCOD_CONFUSIONE_SESSUALE
                                TrappolaUso = enum_TrappoleUso.ConfusioneSessuale
                            Case LAVCOD_DISORIENTAMENTO_SESSUALE
                                TrappolaUso = enum_TrappoleUso.Disorientamento
                        End Select

                        AgronicaCoreUtility.CaricaListControl.TrappolexSpecieVegetalixAvversita(CType(Me.cmb_Trappola, ListControl),
                                                                                    True, "", "0",
                                                                                    CInt(Me.Cmb_Specie.SelectedItem.Value),
                                                                                    0,
                                                                                    TrappolaUso,
                                                                                    "", "", objParametri_Server)

                        DefaultAziendali(CBL_Consigli_Trappole, CInt(ComboOperazione.Valore_Combo))

                        Dim ArrayPannelli() As enum_TipoPannello

                        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
                            ReDim ArrayPannelli(2)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloTrappole
                            ArrayPannelli(2) = enum_TipoPannello.Pannello_Costi
                        Else
                            ReDim ArrayPannelli(1)
                            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                            ArrayPannelli(1) = enum_TipoPannello.PannelloTrappole
                        End If

                        If GridView_Impianti.Rows.Count > 0 Then
                            ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                            ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
                        End If

                        Imposta_Pannelli(ArrayPannelli)


                    Case Else

                        Dim ArrayPannelli(0) As enum_TipoPannello
                        ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
                        Imposta_Pannelli(ArrayPannelli)

                        Messaggi.AgroMsgBox("Operazione non gestita!", Page, , Script_Panel)
                        Exit Sub

                End Select

                Btn_Salva_Operazione.Visible = False
                Btn_Aggiungi_Dettaglio.Visible = True
                Btn_Annulla_Dettaglio.Visible = True

                Dim script As New StringBuilder

                script.AppendLine("$(document).ready(function () { ")
                script.AppendLine("             $('#dialogOperazione').dialog('open');")
                script.AppendLine("                 $('#WaitFrame').hide();")
                script.AppendLine("             $('#dialogOperazione').parent().appendTo($('form:first')); ")
                script.AppendLine("     });")
                ScriptManager.RegisterClientScriptBlock(UpdatePanelOperazione, UpdatePanelOperazione.GetType(),
                            String.Format("jQuery_{0}", UpdatePanelOperazione.ClientID), script.ToString, True)


        End Select


    End Sub


    '########################################################################################
    ' CaricaGriglia_Avversita utilizzata con e senza DPI
    '########################################################################################
    Private Sub CaricaGriglia_Avversita()

        '----- Definizione delle variabili

        Dim Dt_Avv_Tot As DataTable
        Dim DrAvv() As DataRow
        Dim DrAvvGru() As DataRow

        Dim DtAvv As New DataTable
        Dim DtAvvGru As New DataTable
        Dim Dr As DataRow
        Dim DtAvvDPI As New DataTable
        Dim DtAvvGruDPI As New DataTable

        Dim filtroDPI As String = ""

        Dim Av_Cod As Integer
        Dim Av_Des_Vol As String
        Dim Av_Gru As Integer
        Dim Av_Gru_Des As String
        Dim Modulo, Epoca As Integer

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String
        Dim i As Integer

        Dim Array() As String
        Dim Dpi_Cod As Integer = 0
        Dim IdRcdpi As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim Flag_Protetto As Integer = 0
        Dim Flag_PubblicoPrivato As Integer = 0

        Dim AvvCodPresente As Boolean
        Dim AvvGruPresente As Boolean


        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Filtro_Tot As String = ""

        '----- Definisco la struttura dei DataTable

        DtAvv.Columns.Add(New DataColumn("Av_Des", GetType(String)))
        DtAvv.Columns.Add(New DataColumn("Av_Cod", GetType(Integer)))
        DtAvv.Columns.Add(New DataColumn("Soglia", GetType(String)))


        DtAvvGru.Columns.Add(New DataColumn("Av_Gru_Des", GetType(String)))
        DtAvvGru.Columns.Add(New DataColumn("Av_Gru", GetType(Integer)))
        DtAvvGru.Columns.Add(New DataColumn("Str_Av_Cod", GetType(String)))
        DtAvvGru.Columns.Add(New DataColumn("Soglia", GetType(String)))


        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(1) As DataColumn
        Dim DtKeys2(1) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = DtAvv.Columns("Av_Cod")
        DtKeys2(0) = DtAvvGru.Columns("Av_Gru")

        'Assegno il vettore delle chiavi al DataTable
        DtAvv.PrimaryKey = DtKeys
        DtAvvGru.PrimaryKey = DtKeys2

        '---------------------------------------------------------------
        If Cmb_Specie.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox("Selezionare la Specie Vegetale!", Page, , Script_Panel)
            Exit Sub
        End If

        Dim TipoTestata As Integer

        'controllo se devo caricare i fitofarmaci o i diserbanti
        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                TipoTestata = 0
            Case LAVCOD_DISERBO ', LAVCOD_DISSECCAMENTO
                TipoTestata = 1
            Case Else
                Exit Sub
        End Select

        'Select Case Session("Collegamento_Fito")

        '    '----------------------------------
        '    '--------------------------------
        '    'LETTURA DATI REMOTO
        '    '--------------------------------
        '    '----------------------------------
        '    Case True

        '----------------------------------
        ' DISCIPLINARE
        '----------------------------------               

        'If Session("permessoDPI") = True And Session("Collegamento_DPI") = True Then


        Select Case Cmb_Disciplinare.SelectedValue

            Case Is <> "0" 'DPI

                Array = Split(Cmb_Disciplinare.SelectedValue, "/")
                Dpi_Cod = Array(0)
                IdRcdpi = Array(1)
                Grfi_Cod = Array(2)
                Flag_Protetto = Array(3)
                Flag_PubblicoPrivato = Array(4)

                If IdRcdpi > 0 Then

                    Modulo = 0
                    Epoca = 0

                    If Cmb_Epoca.Visible Then
                        If TipoTestata = 0 Then
                            Modulo = CInt(Cmb_Epoca.SelectedValue)
                        Else
                            Epoca = CInt(Cmb_Epoca.SelectedValue)
                        End If
                    End If

                    ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                    ObjDownloadWs.Url = objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari 'System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")

                    If Flag_PubblicoPrivato = 2 Then
                        Dpi_Cod = -Dpi_Cod
                    End If

                    Dati = ObjDownloadWs.Leggi_Infestanti_conDescrizioniAvversita(CInt(IdRcdpi),
                                                          CInt(Dpi_Cod),
                                                          TipoTestata,
                                                          CInt(0),
                                                          CInt(0),
                                                          CInt(0),
                                                          CInt(0),
                                                          CInt(0),
                                                          CInt(0),
                                                          CStr(""),
                                                          CInt(Modulo),
                                                          CInt(Epoca),
                                                          CStr(Session("ASG_Utente_Username_Crypt")),
                                                          CStr(Session("ASG_Utente_Password_Crypt")),
                                                          Filtro_Tot,
                                                          "")

                    ObjDownloadWs = Nothing

                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                        XmlDocumento.LoadXml(Dati)

                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                        If XmlNodo IsNot Nothing Then

                            DtAvvDPI = DtAvv.Clone
                            DtAvvGruDPI = DtAvvGru.Clone

                            For Each XmlElemento In XmlNodo

                                AvvCodPresente = False
                                AvvGruPresente = False

                                If XmlElemento.GetAttribute("av_cod") <> "" AndAlso XmlElemento.GetAttribute("av_cod") <> "0" Then

                                    Av_Cod = CInt(XmlElemento.GetAttribute("av_cod"))
                                    Av_Des_Vol = XmlElemento.GetAttribute("av_des_vol")

                                    If InStr(Av_Des_Vol, "non usare") = 0 Then

                                        For i = 0 To DtAvv.Rows.Count - 1
                                            If DtAvv.Rows(i).Item("Av_Cod") = Av_Cod Then
                                                AvvCodPresente = True
                                                Exit For
                                            End If
                                        Next

                                        If Not AvvCodPresente Then

                                            Dr = DtAvv.NewRow
                                            Dr.Item("Av_Des") = XmlElemento.GetAttribute("av_des_vol") & " (<i>" & XmlElemento.GetAttribute("av_des_lat") & "</i>)"
                                            Dr.Item("Av_Cod") = Av_Cod
                                            Dr.Item("Soglia") = " --- "
                                            DtAvv.Rows.Add(Dr)

                                        End If

                                    End If

                                End If


                                If XmlElemento.GetAttribute("av_gru") <> "" AndAlso XmlElemento.GetAttribute("av_gru") <> "0" Then

                                    Av_Gru = CInt(XmlElemento.GetAttribute("av_gru"))
                                    Av_Gru_Des = XmlElemento.GetAttribute("av_gru_des")

                                    If InStr(Av_Gru_Des, "non usare") = 0 Then

                                        For i = 0 To DtAvvGru.Rows.Count - 1
                                            If DtAvvGru.Rows(i).Item("av_gru") = Av_Gru Then
                                                AvvGruPresente = True
                                                Exit For
                                            End If
                                        Next

                                        If Not AvvGruPresente Then

                                            Dr = DtAvvGru.NewRow
                                            Dr.Item("av_gru_des") = XmlElemento.GetAttribute("av_gru_des") & " (<i>" & XmlElemento.GetAttribute("av_gru_des_lat") & "</i>)"
                                            Dr.Item("Av_Gru") = Av_Gru
                                            Dr.Item("str_Av_Cod") = 0
                                            DtAvvGru.Rows.Add(Dr)

                                        End If

                                    End If

                                End If

                            Next

                        End If

                    End If

                End If

            Case Else

                '----------------------------------
                ' NESSUN DISCIPLINARE
                '----------------------------------
                Dim Data As Date = AGRODATAFINE
                If Me.Txt_DataFine.Text <> "" Then
                    Data = CDate(Txt_DataFine.Text)
                End If

                Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi
                If TipoTestata = 0 Then
                    Dt_Avv_Tot = objDPILeggi.Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_4(0,
                                                                        CInt(Cmb_Specie.SelectedValue),
                                                                        0,
                                                                        0,
                                                                        1,
                                                                        "",
                                                                        Data,
                                                                        Session)
                Else
                    Dt_Avv_Tot = objDPILeggi.Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_4(0,
                                                                    CInt(Cmb_Specie.SelectedValue),
                                                                    0,
                                                                    0,
                                                                    1,
                                                                    "",
                                                                    Data,
                                                                    Session)
                End If

                '---------
                'SINGOLE
                DrAvv = Dt_Avv_Tot.Select("Av_Gru=0", "Av_Des_Vol")

                If DrAvv IsNot Nothing AndAlso DrAvv.Length > 0 Then
                    For i = 0 To DrAvv.Length - 1
                        If DrAvv(i).Item("Av_Cod") <> 0 Then
                            If InStr(DrAvv(i).Item("Av_Des_Vol"), "non usare") = 0 Then
                                Dr = DtAvv.NewRow
                                Dr.Item("Av_Des") = DrAvv(i).Item("Av_Des_Vol") & " (<i>" & DrAvv(i).Item("Av_Des_Lat") & "</i>)"
                                Dr.Item("Av_Cod") = DrAvv(i).Item("Av_Cod")
                                Dr.Item("Soglia") = " --- "
                                DtAvv.Rows.Add(Dr)
                            End If
                        End If
                    Next
                End If
                '----------------
                'GRUPPI
                DrAvvGru = Dt_Avv_Tot.Select("Av_Cod=0", "Av_Gru_Des")
                If DrAvvGru IsNot Nothing AndAlso DrAvvGru.Length > 0 Then
                    For i = 0 To DrAvvGru.Length - 1
                        If DrAvvGru(i).Item("Av_Gru") <> 0 Then
                            If InStr(DrAvvGru(i).Item("Av_Gru_Des"), "non usare") = 0 Then
                                Dr = DtAvvGru.NewRow
                                Dr.Item("Av_Gru_Des") = DrAvvGru(i).Item("Av_Gru_Des") & " (<i>" & DrAvvGru(i).Item("Av_Gru_Des_Lat") & "</i>)"
                                Dr.Item("Av_Gru") = DrAvvGru(i).Item("Av_Gru")
                                Dr.Item("Soglia") = " --- "
                                Dr.Item("Str_Av_Cod") = ""
                                DtAvvGru.Rows.Add(Dr)
                            End If
                        End If
                    Next
                End If

        End Select



        '    Case Else

        '        '----------------------------------
        '        '--------------------------------
        '        'LETTURA DATI IN LOCALE
        '        '--------------------------------
        '        '----------------------------------

        '        '-----------------------
        '        'SINGOLE

        '        Dim filtroAvv As String
        '        Dim DtAvvTmp As DataTable
        '        Dim DtAvvGruTmp As DataTable

        '        If TipoTestata = 0 Then
        '            'TRATTAMENTO


        '            'filtro infestanti/gruppi infestanti
        '            filtroAvv &= "  NOT EXISTS (SELECT *	FROM InfestantiAttive " & _
        '                           " WHERE InfestantiAttive.Av_Cod = Avversita.Av_Cod ) "

        '            'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        '            filtroAvv &= " AND Avversita.Av_Des_Vol NOT LIKE '%non usare%' " & _
        '                         " AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' "

        '            Dim objAvv As New AgronicaCoreMetaSchemaDAL.Avversita_R
        '            DtAvvTmp = objAvv.Leggi(0, "", _
        '                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
        '                                filtroAvv, _
        '                                "", _
        '                                objParametri_Server)
        '            objAvv = Nothing
        '            If Not IsNothing(DtAvvTmp) Then
        '                For i = 0 To DtAvvTmp.Rows.Count - 1
        '                    Dr = DtAvv.NewRow
        '                    Dr.Item("Av_Des") = DtAvvTmp.Rows(i).Item("Av_Des_Vol") & " (<i>" & DtAvvTmp.Rows(i).Item("Av_Des_Lat") & "</i>)"
        '                    Dr.Item("Av_Cod") = DtAvvTmp.Rows(i).Item("Av_Cod")
        '                    Dr.Item("Soglia") = " --- "
        '                    DtAvv.Rows.Add(Dr)
        '                Next
        '            End If

        '            '-----------------------
        '            'GRUPPI
        '            Dim filtroAvvGru As String
        '            'filtro infestanti/gruppi infestanti
        '            filtroAvvGru = "  NOT EXISTS (SELECT *	FROM GruppoAvversitaAttive " & _
        '                         " WHERE GruppoAvversitaAttive.Av_Gru = GruppoAvversita.Av_Gru ) "

        '            'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        '            filtroAvvGru &= " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' " & _
        '                            " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' "

        '            'End If
        '            Dim objAvvGru As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
        '            DtAvvGruTmp = objAvvGru.Leggi(0, 0, _
        '                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
        '                                filtroAvvGru, _
        '                                "", _
        '                                objParametri_Server)
        '            objAvvGru = Nothing
        '            If Not IsNothing(DtAvvGruTmp) Then
        '                For i = 0 To DtAvvGruTmp.Rows.Count - 1
        '                    Dr = DtAvvGru.NewRow
        '                    Dr.Item("Av_Gru_Des") = DtAvvGruTmp.Rows(i).Item("Av_Gru_Des") & " (<i>" & DtAvvGruTmp.Rows(i).Item("Av_Gru_Des_Lat") & "</i>)"
        '                    Dr.Item("Av_Gru") = DtAvvGruTmp.Rows(i).Item("Av_Gru")
        '                    Dr.Item("Soglia") = " --- "
        '                    Dr.Item("Str_Av_Cod") = ""
        '                    DtAvvGru.Rows.Add(Dr)
        '                Next
        '            End If


        '        Else
        '            'DISERBO


        '            'filtro infestanti/gruppi infestanti
        '            'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        '            filtroAvv &= " Avversita.Av_Des_Vol NOT LIKE '%non usare%' " & _
        '                         " AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' "

        '            Dim objAvv As New AgronicaCoreMetaSchemaDAL.InfestantiAttive_R
        '            DtAvvTmp = objAvv.Leggi(0, 0, _
        '                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
        '                                filtroAvv, _
        '                                "", _
        '                                objParametri_Server)
        '            objAvv = Nothing
        '            If Not IsNothing(DtAvvTmp) Then
        '                For i = 0 To DtAvvTmp.Rows.Count - 1
        '                    Dr = DtAvv.NewRow
        '                    Dr.Item("Av_Des") = DtAvvTmp.Rows(i).Item("Av_Des_Vol") & " (<i>" & DtAvvTmp.Rows(i).Item("Av_Des_Lat") & "</i>)"
        '                    Dr.Item("Av_Cod") = DtAvvTmp.Rows(i).Item("Av_Cod")
        '                    Dr.Item("Av_Gru") = 0 ' DtAvvTmp.Rows(i).Item("Av_Gru")
        '                    DtAvv.Rows.Add(Dr)
        '                Next
        '            End If



        '            '-----------------------
        '            'GRUPPI
        '            Dim filtroAvvGru As String
        '            'filtro infestanti/gruppi infestanti
        '            'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        '            filtroAvvGru = " GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' " & _
        '                            " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' "

        '            Dim objAvvGru As New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R
        '            DtAvvGruTmp = objAvvGru.Leggi(0, 0, _
        '                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
        '                                filtroAvvGru, _
        '                                "", _
        '                                objParametri_Server)

        '            If Not IsNothing(DtAvvGruTmp) Then
        '                For i = 0 To DtAvvGruTmp.Rows.Count - 1
        '                    Dr = DtAvvGru.NewRow
        '                    Dr.Item("Av_Des") = DtAvvGruTmp.Rows(i).Item("Av_Gru_Des") & " (<i>" & DtAvvGruTmp.Rows(i).Item("Av_Gru_Des_Lat") & "</i>)"
        '                    Dr.Item("Av_Cod") = 0
        '                    Dr.Item("Av_Gru") = DtAvvGruTmp.Rows(i).Item("Av_Gru")
        '                    DtAvvGru.Rows.Add(Dr)
        '                Next
        '            End If


        '        End If


        'End Select


        '----------------------------------------------------------------------
        '----- Associo il DataTable con la DataGrid

        'uso il dataview per ordinare
        Dim DvAvv As New DataView
        Dim DvAvvGru As New DataView

        DtAvv.TableName = "Av_Des"
        DvAvv.Table = DtAvv
        DvAvv.Sort = "Av_Des ASC"

        DtAvvGru.TableName = "Av_Gru_Des"
        DvAvvGru.Table = DtAvvGru
        DvAvvGru.Sort = "Av_Gru_Des ASC"

        GridViewAvversita.DataSource = DvAvv
        GridViewAvversita.DataBind()

        GridViewGruppiAvversita.DataSource = DvAvvGru
        GridViewGruppiAvversita.DataBind()


        '----------------------------------------------------------------------

        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                RBL_Avversita.Items(0).Text = "Avversita'"
                RBL_Avversita.Items(1).Text = "Gruppi Avversita'"
                If Cmb_Disciplinare.SelectedValue <> "0" Then
                    RBL_Avversita.SelectedIndex = 0
                    RBL_Avversita.Enabled = False
                Else
                    RBL_Avversita.SelectedIndex = 1
                    RBL_Avversita.Enabled = True
                End If
            Case LAVCOD_DISERBO
                RBL_Avversita.Items(0).Text = "Infestanti"
                RBL_Avversita.Items(1).Text = "Gruppi Infestanti"
                RBL_Avversita.SelectedValue = "1"
                If Cmb_Disciplinare.SelectedValue <> "0" Then
                    RBL_Avversita.Enabled = False
                Else
                    RBL_Avversita.Enabled = True
                End If
        End Select




        If Me.RBL_Avversita.SelectedValue = "0" Then

            If DtAvv.Rows.Count > 0 Then
                'Me.GridViewAvversita.Visible = True
                'Me.GridViewGruppiAvversita.Visible = False
            Else
                'se nn ci sono singole ma gruppi sì
                'visualizzo i gruppi
                If DtAvvGru.Rows.Count > 0 Then
                    'Me.GridViewAvversita.Visible = False
                    'Me.GridViewGruppiAvversita.Visible = True
                    RBL_Avversita.SelectedValue = "1"
                End If
            End If

        Else

            If DtAvvGru.Rows.Count > 0 Then
                'Me.GridViewAvversita.Visible = False
                'Me.GridViewGruppiAvversita.Visible = True
            Else
                'se nn ci sono singole ma gruppi sì
                'visualizzo i gruppi
                If DtAvv.Rows.Count > 0 Then
                    'Me.GridViewAvversita.Visible = True
                    'Me.GridViewGruppiAvversita.Visible = False
                    RBL_Avversita.SelectedValue = "0"
                End If
            End If

        End If


        '==============================================================================
        'Abilito la griglia solo se esistono degli elementi selezionabili
        '------------------------------------------------------------------------------
        GridViewAvversita.Columns(3).Visible = False
        GridView_Dosi_Difesa.Columns(6).Visible = False

        If TipoTestata = 0 Then
            If GridViewAvversita.Rows.Count > 0 Then
                Dim objAgroWebConfig As New AgroWebConfig
                If Session("Collegamento_DPI") = True AndAlso
                    objAgroWebConfig.Flag_SoglieAttive = True AndAlso
                    Cmb_Disciplinare.SelectedValue <> "0" AndAlso
                    CInt(IdRcdpi) <> 0 Then

                    If Carica_SoglieIntervento_Avversita() = True Then
                        Me.GridViewAvversita.Columns(3).Visible = True
                        GridView_Dosi_Difesa.Columns(6).Visible = True
                    Else
                        'Me.GridViewAvversita.Columns(3).Visible = False
                        'If GridView_Dosi_Difesa.Rows.Count = 0 Then
                        '    GridView_Dosi_Difesa.Columns(6).Visible = False
                        'End If
                    End If

                Else

                    'Me.GridViewAvversita.Columns(3).Visible = False
                    'If GridView_Dosi_Difesa.Rows.Count = 0 Then
                    '    GridView_Dosi_Difesa.Columns(6).Visible = False
                    'End If


                End If

            End If
        Else
            'GridViewAvversita.Columns(3).Visible = False
            'GridView_Dosi_Difesa.Columns(6).Visible = False
        End If




    End Sub

    '########################################################################################
    Private Function Carica_SoglieIntervento_Avversita() As Boolean

        Dim risposta As Boolean = False

        Dim objAgroWebConfig As New AgroWebConfig
        Dim strAvversita As String
        Dim strGruppiAvversita As String
        Dim i As Integer
        'Dim SogliaDes As String
        Dim Av_Cod As Integer
        'Dim strAv_Cod As String

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim DtSoglieA As New DataTable
        Dim DrA As DataRow
        Dim DrSogliaA() As DataRow
        'Dim DtSoglieG As New DataTable
        'Dim DrG As DataRow
        'Dim DrSogliaG() As DataRow

        'Dim RsSoglie As ADODB.Recordset

        DtSoglieA.Columns.Add(New DataColumn("Av_Cod", GetType(Integer)))
        DtSoglieA.Columns.Add(New DataColumn("Soglia", GetType(String)))
        DtSoglieA.Columns.Add(New DataColumn("Soglia_Cod", GetType(Integer)))
        DtSoglieA.Columns.Add(New DataColumn("Quantita", GetType(String)))
        'DtSoglieG.Columns.Add(New DataColumn("Av_Cod", GetType(Integer)))
        ' DtSoglieG.Columns.Add(New DataColumn("Soglia", GetType(String)))

        strAvversita = ""
        strGruppiAvversita = ""

        Dim Array() As String
        Dim Dpi_Cod As Integer = 0
        Dim IdRcdpi As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim Flag_Protetto As Integer = 0
        Dim Flag_PubblicoPrivato As Integer = 0

        Array = Split(Cmb_Disciplinare.SelectedItem.Value, "/")
        Dpi_Cod = Array(0)
        IdRcdpi = Array(1)
        Grfi_Cod = Array(2)
        Flag_Protetto = Array(3)
        Flag_PubblicoPrivato = Array(4)


        '------------------------------------------------------------------------------
        'AVVERSITA' SINGOLE
        '------------------------------------------------------------------------------

        'Ricavo la stringa di tutte le avversità
        For i = 0 To Me.GridViewAvversita.Rows.Count - 1

            strAvversita = strAvversita & " Av_Cod=" & Me.GridViewAvversita.Rows(i).Cells(1).Text & " OR "

        Next

        If Trim(strAvversita) <> "" Then

            strAvversita = "(" & Left(strAvversita, Len(strAvversita) - 3) & ")"

            Try

                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                objWs.NewWS(ObjDownloadWs,
                                objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari,
                                objParametri_Utenti)

                If Flag_PubblicoPrivato = 2 Then
                    Dpi_Cod = -Dpi_Cod
                End If

                Dati = ObjDownloadWs.Leggi_SoglieIntervento(CInt(IdRcdpi),
                                                            CInt(Dpi_Cod),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CStr(strAvversita),
                                                            CInt(0),
                                                            CStr(Session("ASG_Utente_Username_Crypt")),
                                                            CStr(Session("ASG_Utente_Password_Crypt")))

            Catch ex As Exception

                'AgroMsgBox("Si sono verificati errori in fase di chiamata al WebService Dicliplinari!", Page)

            End Try

            If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                XmlDocumento.LoadXml(Dati)

                XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                If XmlNodo IsNot Nothing Then

                    For Each XmlElemento In XmlNodo

                        DrA = DtSoglieA.NewRow

                        DrA.Item("Av_Cod") = CInt(XmlElemento.GetAttribute("av_cod"))
                        DrA.Item("Soglia") = CStr(XmlElemento.GetAttribute("dichiarazione_des").ToString.ToLower)
                        DrA.Item("Soglia_Cod") = CStr(XmlElemento.GetAttribute("si_cod"))
                        DrA.Item("Quantita") = CStr(XmlElemento.GetAttribute("quantita"))

                        DtSoglieA.Rows.Add(DrA)

                    Next

                End If

            End If

        End If


        For i = 0 To Me.GridViewAvversita.Rows.Count - 1

            Av_Cod = Me.GridViewAvversita.Rows(i).Cells(1).Text()

            DrSogliaA = DtSoglieA.Select("Av_Cod=" & Av_Cod)

            If DrSogliaA IsNot Nothing AndAlso DrSogliaA.Length > 0 Then

                Dim iSoglie As Integer
                'controllo quale era selezionato 

                Dim valoreSelezionato As String = If(CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                                                        RadioButtonList).SelectedValue <> "",
                                                                CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                                                                    RadioButtonList).SelectedValue,
                                                                -1)
                'pulisco e rigenero
                CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                                                        RadioButtonList).Items.Clear()

                For iSoglie = 0 To DrSogliaA.Length - 1

                    risposta = True


                    'aggiungo il valore
                    Dim str As String
                    Dim value As String

                    If DrSogliaA(iSoglie).Item("Quantita") <> "" Then
                        str = " > del " & DrSogliaA(iSoglie).Item("Quantita") &
                            DrSogliaA(iSoglie).Item("Soglia").ToString
                        value = DrSogliaA(iSoglie).Item("Av_Cod") & "_" &
                                DrSogliaA(iSoglie).Item("Quantita") & "_" &
                                DrSogliaA(iSoglie).Item("Soglia_Cod")
                    Else
                        str = DrSogliaA(iSoglie).Item("Soglia").ToString
                        'value = DrSogliaA(iSoglie).Item("Av_Cod") & "_" & _
                        '        "_" & DrSogliaA(iSoglie).Item("Soglia_Cod")
                        value = DrSogliaA(iSoglie).Item("Av_Cod") & "_0_" & DrSogliaA(iSoglie).Item("Soglia_Cod")
                    End If

                    CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                        RadioButtonList).Items.Add(New ListItem(str, value))


                    If valoreSelezionato = value Then
                        CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                        RadioButtonList).Items(CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                        RadioButtonList).Items.Count - 1).Selected = True
                    Else
                        CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                        RadioButtonList).Items(CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"),
                        RadioButtonList).Items.Count - 1).Selected = False
                    End If
                Next
                'radio.Items(0).Selected = True
                'Me.GridViewAvversita.Rows(i).Cells(3).Controls.Add(radio)


            Else
                Me.GridViewAvversita.Rows(i).Cells(3).Text() = ""
            End If

        Next

        Return risposta

    End Function


    Protected Sub ImgBtn_Cerca_Formulati_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cerca_Formulati.Click


        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Validita_Inizio = #1/1/1900#
        Validita_Fine = #12/31/2100#



        '--------------------------------------------------------
        'controllo se ho selezionato la Specie
        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox("Selezionare la Specie Vegetale!", Page, , Script_Panel)
            Exit Sub
        End If

        '--------------------------------------------------------
        'controllo se ho selezionato l'operazione
        If ComboOperazione.Testo_Combo = "" Then
            Messaggi.AgroMsgBox("Selezionare l'Intervento!", Page, , Script_Panel)
            Exit Sub
        End If

        ComboFormulati.Veg_Cod = CInt(Cmb_Specie.SelectedValue)

        ComboFormulati.Disciplinare_Cod = Cmb_Disciplinare.SelectedValue

        '--------------modifica 18/07/2013 '-------------
        ComboFormulati.TipoRichiesto = CInt(Cmb_FormulatoClassificazioni.SelectedValue)

        If ComboFormulati.TipoRichiesto = enum_TipoFormulato.Coadiuvanti OrElse ComboFormulati.TipoRichiesto = enum_TipoFormulato.Corroboranti_Fisiofarmaci Then
            ComboFormulati.Veg_Cod = 0
        End If
        '------------- '------------- '-------------
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, Bagnanti, Antischiuma
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        'Select Case CInt(ComboOperazione.Valore_Combo)
        '    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO
        '        ComboFormulati.TipoRichiesto = 1
        '    Case LAVCOD_DISERBO
        '        ComboFormulati.TipoRichiesto = 2
        '    Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
        '        ComboFormulati.TipoRichiesto = 3
        '    Case LAVCOD_CONCIA_SEME
        '        ComboFormulati.TipoRichiesto = 5
        '    Case LAVCOD_DISSECCAMENTO
        '        ComboFormulati.TipoRichiesto = 6
        '    Case LAVCOD_GEODISINFESTAZIONE
        '        ComboFormulati.TipoRichiesto = 7
        'End Select

        If ComboFormulati.TipoRichiesto = 4 Then
            ComboFormulati.Veg_Cod = 0
        End If

        ComboFormulati.TestoRicerca = Txt_Formulati.Text
        ComboFormulati.Flag_PrincipiAttivi = True

        ComboFormulati.Piva = objParametriAgenda.Piva
        'ComboFormulati.Fabbricato_Cod = objParametriAgenda.Fabbricato
        'ComboFormulati.Cau_Mov = CAU_SCARICO

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
        End If
        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If

        ComboFormulati.Validita_Fine = Validita_Fine
        ComboFormulati.Validita_Inizio = Validita_Inizio


        If IsNumeric(Cmb_Epoca.SelectedValue) Then
            Select Case CInt(ComboOperazione.Valore_Combo)
                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                    ComboFormulati.Modulo = Cmb_Epoca.SelectedValue
                    ComboFormulati.Epoca_Cod = 0
                Case LAVCOD_DISERBO
                    ComboFormulati.Epoca_Cod = Cmb_Epoca.SelectedValue
                    ComboFormulati.Modulo = 0
            End Select
        End If

        ComboFormulati.WS_Disciplinari_AgroWS_Disciplinari = objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari
        ComboFormulati.WS_Fitofarmaci_AgroWS_Fitofarmaci = objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci

        If CellaAvversita.Visible Then

            Select Case CInt(ComboOperazione.Valore_Combo)
                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                    ComboFormulati.Opt_Avversita_Infestanti = 0
                    ComboFormulati.Tipo_Testata = 0
                Case LAVCOD_DISERBO
                    ComboFormulati.Opt_Avversita_Infestanti = 1
                    ComboFormulati.Tipo_Testata = 1
            End Select

            'Dim Valore_Ricerca As String
            'Valore_Ricerca = ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue
            'Select Case Valore_Ricerca
            '    Case "0" 'nessuno
            '        ComboFormulati.FiltroRicerca = 0
            '    Case "1" 'Prodotti --> Avversità
            '        ComboFormulati.Opt_Singola_Gruppo = -1
            '        ComboFormulati.FiltroRicerca = 1
            '    Case "2" 'Avversità --> Prodotti
            ComboFormulati.FiltroRicerca = 2
            ComboFormulati.Opt_Singola_Gruppo = CInt(RBL_Avversita.SelectedValue)

            Dim Array_AvCod(0) As Integer
            Dim Array_AvGru(0) As Integer

            Dim strAvversita As String = ""
            Dim NumAvv As Integer = 0
            Dim NumAvvGru As Integer = 0

            Select Case RBL_Avversita.SelectedValue

                Case "0" 'singole

                    For i = 0 To GridViewAvversita.Rows.Count - 1
                        If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                            ReDim Preserve Array_AvCod(NumAvv)
                            Array_AvCod(NumAvv) = GridViewAvversita.Rows(i).Cells(1).Text
                            ReDim Preserve Array_AvGru(NumAvv)
                            Array_AvGru(NumAvv) = "0"
                            NumAvv += 1
                        End If
                    Next

                Case Else 'gruppi

                    For i = 0 To GridViewGruppiAvversita.Rows.Count - 1
                        If CType(GridViewGruppiAvversita.Rows(i).FindControl("ChkSelezionaGruppoAvversita"), CheckBox).Checked = True Then
                            ReDim Preserve Array_AvCod(NumAvvGru)
                            Array_AvCod(NumAvvGru) = "0"
                            ReDim Preserve Array_AvGru(NumAvvGru)
                            Array_AvGru(NumAvvGru) = GridViewGruppiAvversita.Rows(i).Cells(1).Text
                            NumAvvGru += 1
                        End If
                    Next
            End Select

            If NumAvv <> 0 OrElse NumAvvGru <> 0 Then
                If strAvversita <> "" Then
                    strAvversita = Left(strAvversita, strAvversita.Length - 4)
                    ComboFormulati.StrAvversita = strAvversita
                End If
            Else
                'If Cmb_FormulatoClassificazioni.SelectedValue <> 4 Then
                '    Messaggi.AgroMsgBox("Se si seleziona il filtro Avversita' --> Prodotti occorre scegliere almeno un'Avversità o un Gruppo Avversita' prima di avviare la ricerca!", _
                '                        Page, , Script_Panel)
                '    Exit Sub
                'End If
            End If

            ComboFormulati.Av_Cod = Array_AvCod
            ComboFormulati.Av_Gru = Array_AvGru

            'End Select

        End If

        ComboFormulati.Flag_ClasseTossicologica = True
        ComboFormulati.CaricaComboFormulati()

        Me.Lbl_Num_Formulati.Text = "Trovati " & ComboFormulati.N_Formulati.ToString & " Formulati"


    End Sub


    ''' <summary>
    ''' FORMULATI
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BTN_ComboFormulati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTN_ComboFormulati.Click



        Dim FrCod As Integer
        Dim i As Integer
        Dim Udm_Sim As String

        Dim DoseMin, DoseMax As Decimal
        Dim Udm_Cod As Integer
        Dim AcquaMin, AcquaMax As Decimal
        Dim AcquaUdm_Cod As Integer
        Dim AcquaUdm_Sim As String
        Dim A_Epoca, Da_Epoca As Integer
        Dim Limite As Integer
        Dim LimiteUdm_Cod As Integer
        Dim LimiteUdm_Sim As String

        Dim DoseEtichetta As String = ""
        Dim DoseEtichettaTemp As String = ""



        'Resettasession_Etichetta()

        'CaricaComboUnitadiMisura()

        Dim ArrayTmp() As String

        If ComboFormulati.Valore_Combo <> "" Then

            ArrayTmp = Split(ComboFormulati.Valore_Combo, "£")

            If ArrayTmp IsNot Nothing Then
                FrCod = ArrayTmp(0)
            End If

            '----------------------------------------------------------

            If ArrayTmp IsNot Nothing Then

                DoseEtichetta = ""
                Dim strEpoca As String = ""
                Dim Flag_Fioritura As Integer

                If ArrayTmp.Length > 1 Then

                    For i = 6 To ArrayTmp.Length - 1

                        Dim ArrayDose() As String
                        ArrayDose = Split(ArrayTmp(i), "$")

                        If ArrayDose IsNot Nothing AndAlso ArrayDose.Length > 0 Then

                            DoseMin = ArrayDose(1)
                            DoseMax = ArrayDose(2)
                            Udm_Cod = ArrayDose(3)
                            Udm_Sim = ArrayDose(4)

                            AcquaMin = ArrayDose(5)
                            AcquaMax = ArrayDose(6)
                            AcquaUdm_Cod = ArrayDose(7)
                            AcquaUdm_Sim = ArrayDose(8)

                            Da_Epoca = ArrayDose(9)
                            A_Epoca = ArrayDose(10)

                            Limite = ArrayDose(11)
                            LimiteUdm_Cod = ArrayDose(12)
                            LimiteUdm_Sim = ArrayDose(13)

                            'Flag_Fioritura = ArrayDose(14)

                            Select Case ArrayDose.Length
                                Case 17
                                    Flag_Fioritura = ArrayDose(14)

                                Case Else
                                    Flag_Fioritura = ArrayDose(15)
                            End Select


                            DoseEtichettaTemp = CreaStringa_DoseEtichetta(DoseMin, DoseMax, Udm_Sim, Udm_Cod,
                                                          AcquaMin, AcquaMax, AcquaUdm_Sim, AcquaUdm_Cod,
                                                          Limite, LimiteUdm_Sim, LimiteUdm_Cod,
                                                          Da_Epoca, A_Epoca, Flag_Fioritura)
                            If InStr(DoseEtichetta, DoseEtichettaTemp) = 0 Then

                                DoseEtichetta &= DoseEtichettaTemp

                                'verifico se è dose Ettaro o dose HL
                                Dim Udm_Radice, per_ha_hl As Integer
                                ScomponiUdm(Udm_Radice, per_ha_hl, Udm_Cod)

                                If RBL_Dose_Formulati.Enabled OrElse
                                    (per_ha_hl = 2121 AndAlso RBL_Dose_Formulati.SelectedValue = "0") OrElse
                                    (per_ha_hl = 2123 AndAlso RBL_Dose_Formulati.SelectedValue = "1") Then


                                    If Udm_Radice <> -1 Then
                                        Select Case per_ha_hl
                                            Case 2121 'hl
                                                RBL_Dose_Formulati.SelectedValue = "0"
                                            Case 2123 'ha
                                                RBL_Dose_Formulati.SelectedValue = "1"
                                        End Select
                                    End If

                                    Txt_Dose_Formulati.Text = DoseMax


                                    Cmb_Udm_Formulati.SelectedIndex =
                                            Cmb_Udm_Formulati.Items.IndexOf(Cmb_Udm_Formulati.Items.FindByValue(
                                                    Udm_Radice))

                                Else
                                    'se è disabilitata non devo cambiarla

                                End If




                            End If

                        End If

                    Next

                    If DoseEtichetta <> "" Then
                        DoseEtichetta = Left(DoseEtichetta, DoseEtichetta.Length - 4)
                    End If

                End If


            End If

            '----
            'Acqua
            Imposta_VolumiAcqua(AcquaMin, AcquaMax, AcquaUdm_Cod, RBL_Acqua_Formulati, Txt_Acqua_Difesa)
            ''----------------
            ''DOSE

            'Imposta_DosiEtichetta(DoseEtichetta)
            Lbl_Dose_Etichetta.Text = DoseEtichetta
            Lbl_Dose_Etichetta.Visible = True

        End If

        'aggiungo blocco e sblocco

        'Post_CaricamentoEtichetta_AggiornaCaselleTesto()

    End Sub

    Private Sub Imposta_VolumiAcqua(ByVal AcquaMin As Decimal,
                                ByVal AcquaMax As Decimal,
                                ByVal AcquaUdm_Cod As Integer,
                                ByRef Rbl_Acqua As RadioButtonList,
                                ByRef Txt_Acqua As TextBox)

        If AcquaMax <> 0 OrElse AcquaMin <> 0 Then
            Dim AcquaUdm_Radice, Acquaper_ha_hl As Integer
            Dim AcquaMaxHL As Decimal
            Dim AcquaMinHL As Decimal
            AcquaUdm_Radice = -1
            ScomponiUdm(AcquaUdm_Radice, Acquaper_ha_hl, AcquaUdm_Cod)
            If AcquaUdm_Radice <> -1 Then
                If Acquaper_ha_hl = 2123 Then
                    Me.RBL_Acqua_Formulati.SelectedValue = "1"
                    If AcquaMax <> 0 Then
                        AcquaMaxHL = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(AcquaUdm_Cod,
                                                    AcquaMax, TipiEnumerativi.enum_UnitaMisura.Ettolitro)
                    End If

                    If AcquaMin <> 0 Then
                        AcquaMinHL = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(AcquaUdm_Cod,
                                                    AcquaMin, TipiEnumerativi.enum_UnitaMisura.Ettolitro)
                    End If

                    Txt_Acqua.Text = AcquaMaxHL.ToString
                End If
            End If
        End If
    End Sub

    Private Function CreaStringa_DoseEtichetta(ByVal DoseMin As String,
                                      ByVal DoseMax As String,
                                      ByVal Udm_Sim As String,
                                      ByVal Udm_Cod As Integer,
                                      ByVal AcquaMin As String,
                                      ByVal AcquaMax As String,
                                      ByVal AcquaUdm_Sim As String,
                                      ByVal AcquaUdm_Cod As Integer,
                                      ByVal Limite As String,
                                      ByVal Limite_Sim As String,
                                      ByVal Limite_Udm_Cod As Integer,
                                      ByVal Da_Epoca As String,
                                      ByVal A_Epoca As String,
                                      ByVal Flag_Fioritura As Integer) As String

        Dim DoseEtichetta As String = "<FONT color=blue>"
        DoseEtichetta &= DoseMin.ToString & "-" & DoseMax.ToString & " " & Udm_Sim.ToString
        DoseEtichetta &= "<font style='font-size:9px; color:#555;'>"

        If AcquaMin <> 0 OrElse AcquaMax <> 0 Then
            DoseEtichetta &= " (Vol.Acqua " & AcquaMin.ToString & "-" & AcquaMax.ToString & " " & AcquaUdm_Sim & ")"
        End If

        If Limite <> 0 Then
            DoseEtichetta &= " (Max " & Limite.ToString & " interventi " & Limite_Sim.ToString & ")"
        End If

        If Flag_Fioritura <> 0 Then
            DoseEtichetta &= " Sospendere i trattamenti a fine fioritura "
        End If

        Dim strEpoca As String = ""
        If Da_Epoca <> 0 AndAlso A_Epoca <> 0 Then
            If Da_Epoca <> A_Epoca Then
                If Da_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, objParametri_Server)
                End If
                If A_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, objParametri_Server)
                End If
            Else
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " in " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, objParametri_Server)
            End If
        Else
            If Da_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, objParametri_Server)
            End If
            If A_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, objParametri_Server)
            End If
        End If

        If strEpoca <> "" Then
            DoseEtichetta &= strEpoca
        End If
        DoseEtichetta &= "</font></font>"
        DoseEtichetta &= "<br>"

        'modifico le variabili di sessione

        'verifico se è dose Ettato o dose HL
        Dim Udm_Radice, per_ha_hl As Integer
        ScomponiUdm(Udm_Radice, per_ha_hl, Udm_Cod)
        If Udm_Radice <> -1 Then
            'converto l'acqua in Hl
            Dim AcquaMaxHL As Decimal = 0
            Dim AcquaMinHL As Decimal = 0

            If AcquaMax <> 0 Then
                Dim AcquaUdm_Radice, Acquaper_ha_hl As Integer

                AcquaUdm_Radice = -1
                ScomponiUdm(AcquaUdm_Radice, Acquaper_ha_hl, AcquaUdm_Cod)
                If AcquaUdm_Radice <> -1 Then
                    If Acquaper_ha_hl = 2123 Then
                        Select Case AcquaUdm_Cod
                            Case 2 'kg
                                AcquaMaxHL = AcquaMax / 100
                            Case 4 'q
                                AcquaMaxHL = AcquaMax
                            Case 304 't
                                AcquaMaxHL = AcquaMax * 10
                            Case 3 'g
                                AcquaMaxHL = AcquaMax / 10000
                            Case 2032 'mg
                                AcquaMaxHL = AcquaMax / 100000
                            Case 29 'l
                                AcquaMaxHL = AcquaMax / 100
                            Case 101 'ml
                                AcquaMaxHL = AcquaMax / 100000
                            Case 104 'cc
                                AcquaMaxHL = AcquaMax / 100000
                        End Select
                    End If
                End If

            End If

            If AcquaMin <> 0 Then
                Dim AcquaUdm_Radice, Acquaper_ha_hl As Integer

                AcquaUdm_Radice = -1
                ScomponiUdm(AcquaUdm_Radice, Acquaper_ha_hl, AcquaUdm_Cod)
                If AcquaUdm_Radice <> -1 Then
                    If Acquaper_ha_hl = 2123 Then
                        Select Case AcquaUdm_Cod
                            Case 2 'kg
                                AcquaMinHL = AcquaMin / 100
                            Case 4 'q
                                AcquaMinHL = AcquaMin
                            Case 304 't
                                AcquaMinHL = AcquaMin * 10
                            Case 3 'g
                                AcquaMinHL = AcquaMin / 10000
                            Case 2032 'mg
                                AcquaMinHL = AcquaMin / 100000
                            Case 29 'l
                                AcquaMinHL = AcquaMin / 100
                            Case 101 'ml
                                AcquaMinHL = AcquaMin / 100000
                            Case 104 'cc
                                AcquaMinHL = AcquaMin / 100000
                        End Select
                    End If
                End If
            End If


            If per_ha_hl = 2121 Then
                'HL
                Impostasession_Etichetta(AcquaMinHL, AcquaMaxHL, 0, DoseMax, 0, DoseMin, 0, 0, Udm_Radice, Udm_Cod, Limite, Limite_Sim, Limite_Udm_Cod, Flag_Fioritura)
            Else
                'HA
                Impostasession_Etichetta(AcquaMinHL, AcquaMaxHL, DoseMax, 0, DoseMin, 0, Udm_Radice, Udm_Cod, 0, 0, Limite, Limite_Sim, Limite_Udm_Cod, Flag_Fioritura)
            End If
        End If

        Return DoseEtichetta

    End Function

    Private Sub Impostasession_Etichetta(ByVal Acqua_Min As Decimal,
                                     ByVal Acqua_Max As Decimal,
                                     ByVal D_HA_Max As Decimal,
                                     ByVal D_HL_Max As Decimal,
                                     ByVal D_HA_Min As Decimal,
                                     ByVal D_HL_Min As Decimal,
                                     ByVal Udm_Radice_HA As Integer,
                                     ByVal Udm_Cod_HA As Integer,
                                     ByVal Udm_Radice_HL As Integer,
                                     ByVal Udm_Cod_HL As String,
                                     ByVal N_Trattamenti_Max As Integer,
                                     ByVal N_Trattamenti_UDM_Sim As String,
                                     ByVal N_Trattamenti_Umd_Cod As Integer,
                                     ByVal Flag_Fioritura As Integer)


        If N_Trattamenti_Max <> 0 Then
            If IsNothing(Session("N_Trattamenti_Max")) Then
                Session("N_Trattamenti_Max") = N_Trattamenti_Max
                Session("N_Trattamenti_Umd_Cod") = N_Trattamenti_Umd_Cod
                Session("N_Trattamenti_UDM_Sim") = N_Trattamenti_UDM_Sim
            Else
                If N_Trattamenti_Max > Session("N_Trattamenti_Max") Then
                    Session("N_Trattamenti_Max") = N_Trattamenti_Max
                    Session("N_Trattamenti_Umd_Cod") = N_Trattamenti_Umd_Cod
                    Session("N_Trattamenti_UDM_Sim") = N_Trattamenti_UDM_Sim
                End If
            End If
        End If


        If Flag_Fioritura <> 0 Then
            Session("Flag_Fioritura") = Flag_Fioritura
        End If


        If Acqua_Min <> 0 Then
            If Acqua_Min < Session("Acqua_Min") Then
                Session("Acqua_Min") = Acqua_Min
            Else
                If Session("Acqua_Min") = 0 Then
                    Session("Acqua_Min") = Acqua_Min
                End If
            End If
        End If

        If Acqua_Max <> 0 AndAlso Acqua_Max > Session("Acqua_Max") Then
            Session("Acqua_Max") = Acqua_Max
        End If

        If D_HA_Max <> 0 AndAlso D_HA_Max > Session("D_HA_Max") Then
            Session("D_HA_Max") = D_HA_Max
        End If

        If D_HL_Max <> 0 AndAlso D_HL_Max > Session("D_HL_Max") Then
            Session("D_HL_Max") = D_HL_Max
        End If


        If D_HL_Min <> 0 AndAlso D_HL_Min < IIf(IsNothing(Session("D_HL_Min")),
                                                10000000, Session("D_HL_Min")) Then
            Session("D_HL_Min") = D_HL_Min
        End If
        If D_HA_Min <> 0 AndAlso D_HA_Min < IIf(IsNothing(Session("D_HA_Min")),
                                                10000000, Session("D_HA_Min")) Then
            Session("D_HA_Min") = D_HA_Min
        End If



        If Udm_Radice_HA <> 0 Then
            Session("Udm_Radice_HA") = Udm_Radice_HA
        End If
        If Udm_Radice_HL <> 0 Then
            Session("Udm_Radice_HL") = Udm_Radice_HL
        End If

        If Udm_Cod_HA <> 0 Then
            Session("Udm_Cod_HA") = Udm_Cod_HA
        End If

        If Udm_Cod_HL <> 0 Then
            Session("Udm_Cod_HL") = Udm_Cod_HL
        End If

    End Sub

    Private Sub ScomponiUdm(ByRef UDM_radice As Integer,
                              ByRef perHa_hl As Integer,
                              ByRef UnitaMisura As Integer)
        Select Case UnitaMisura
            ' a HL
            Case 21
                'cc/hl
                UDM_radice = 104
                perHa_hl = 2121
            Case 23
                'g/hl
                UDM_radice = 3
                perHa_hl = 2121
            Case 126
                'mg/hl
                UDM_radice = 2032
                perHa_hl = 2121
            Case 164
                'ml/hl
                UDM_radice = 101
                perHa_hl = 2121
            Case 173
                'l/hl
                UDM_radice = 29
                perHa_hl = 2121
            Case 175
                'kg/hl
                UDM_radice = 2
                perHa_hl = 2121
                'a HA

            Case 20
                'g/ha
                UDM_radice = 3
                perHa_hl = 2123
            Case 22
                'l/ha
                UDM_radice = 29
                perHa_hl = 2123
            Case 88
                'kg/ha
                UDM_radice = 2
                perHa_hl = 2123

            Case 89
                'unita/ha
                UDM_radice = -1
                perHa_hl = 2123

            Case 90
                'm3/ha
                UDM_radice = -1
                perHa_hl = 2123

            Case 163
                'ml/ha
                UDM_radice = 101
                perHa_hl = 2123
            Case 176
                'n° u/ha
                perHa_hl = 2123

            Case 2098
                't/ha
                UDM_radice = 304
                perHa_hl = 2123

            Case 2112
                't/ha spighe
                UDM_radice = 304
                perHa_hl = 2123

            Case 2120
                'q/ha
                UDM_radice = 4
                perHa_hl = 2123

            Case 2
                'kg
                UDM_radice = 2
                perHa_hl = 2123

            Case 4
                'q
                UDM_radice = 4
                perHa_hl = 2123

            Case 29
                'l
                UDM_radice = 29
                perHa_hl = 2123

        End Select

    End Sub


    Protected Sub BTN_ComboOperazione_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboOperazione.Click

        'If Qs_Tipo_Ricetta <> enum_TipoRicetta.Standard_Destinazioni Then

        '    Dim Validita_Inizio As Date
        '    Dim Validita_Fine As Date
        '    Dim DataInizio As String
        '    Dim DataFine As String
        '    Dim TipoTestata As Integer

        '    'azzero i controlli

        '    Dim ArrayPannelli(0) As enum_TipoPannello
        '    ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
        '    If GridView_Impianti.Rows.Count > 0 Then
        '        ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
        '        ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
        '    End If
        '    Imposta_Pannelli(ArrayPannelli)

        '    Me.Cmb_Disciplinare.Items.Clear()
        '    cella_disciplinare.Visible = False

        '    Me.ComboEpoche.ddl_ComboEpocheFertilizzazione.Items.Clear()
        '    Me.Cmb_Epoca.Items.Clear()
        '    cella_epoca.Visible = False

        '    Validita_Inizio = #1/1/1900#
        '    Validita_Fine = #12/31/2100#
        '    DataInizio = "..."
        '    DataFine = "..."

        '    If Me.Txt_DataInizio.Text <> "" Then
        '        Validita_Inizio = CDate(Txt_DataInizio.Text)
        '        DataInizio = CDate(Txt_DataInizio.Text)
        '    End If
        '    If Me.Txt_DataFine.Text <> "" Then
        '        Validita_Fine = CDate(Txt_DataFine.Text)
        '        DataFine = CDate(Txt_DataFine.Text)
        '    End If

        '    If Cmb_Specie.SelectedItem.Text = "" Then
        '        Messaggi.AgroMsgBox("Selezionare la Specie Vegetale!", Page, , Script_Panel)
        '        Exit Sub
        '    End If

        '    Select Case CInt(ComboOperazione.Valore_Combo)

        '        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

        '            If Session("permessoDPI") = True Then

        '                Select Case CInt(ComboOperazione.Valore_Combo)
        '                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
        '                        TipoTestata = 0
        '                        Me.Lbl_Epoca.InnerText = "Modulo :"
        '                    Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
        '                        TipoTestata = 1
        '                        Me.Lbl_Epoca.InnerText = "Epoca :"
        '                End Select

        '                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(Validita_Inizio), CDate(Validita_Fine))
        '                Dim objCaricaCombo As New AgronicaCoreDpiBIZ.CaricaListControl

        '                objCaricaCombo.Disciplinari_ElencoxTestata(Me.Cmb_Disciplinare, _
        '                                                            False, "", "", _
        '                                                            Session, _
        '                                                            objParametri_Server, _
        '                                                            objParametri_Utenti, _
        '                                                            CInt(0), _
        '                                                            CInt(Cmb_Specie.SelectedValue), _
        '                                                            CInt(0), _
        '                                                            CInt(0), _
        '                                                            Validita_Inizio, _
        '                                                            Validita_Fine, _
        '                                                            True, _
        '                                                            False, _
        '                                                            False, _
        '                                                            TipoTestata)
        '                objParametri_Server.ResettaFinestra()

        '            Else

        '                Cmb_Disciplinare.Items.Clear()
        '                Cmb_Disciplinare.Items.Add(New ListItem("Nessun Disciplinare", "0"))

        '            End If

        '            Me.btn_Disciplinari_Click(Me, Nothing)

        '            cella_disciplinare.Visible = True


        '        Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, _
        '        LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

        '            ComboEpoche.Veg_Cod = CInt(Cmb_Specie.SelectedValue)

        '            ComboEpoche.PrimaRiga_Flag = True
        '            ComboEpoche.PrimaRiga_Text = ""
        '            ComboEpoche.PrimaRiga_Value = "0"

        '            ComboEpoche.CaricaComboEpocheFertilizzazione()

        '            ComboEpoche.Visible = True
        '            cella_epoca.Visible = True
        '            Cmb_Epoca.Visible = False

        '        Case Else

        '            Cmb_Disciplinare.Items.Clear()
        '            Cmb_Disciplinare.Items.Add(New ListItem("Nessun Disciplinare", "0"))

        '    End Select

        'End If




    End Sub


    Protected Sub ImgBtn_DoseInserisci_Formulati_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_DoseInserisci_Formulati.Click

        Dim Messaggio As String

        Dim Av_Cod As String
        Dim Av_Gru As String
        Dim Av_Des As String
        Dim Soglia_Value As String
        Dim Soglia_Des As String

        Dim Mezzo As String

        Dim FrCod As Integer
        Dim FrDes As String
        Dim Carenza As Integer

        Dim strPA_COD As String = ""
        Dim strCLTOSS_COD As String = ""

        Dim DoseMin, DoseMax As Decimal
        Dim DoseEtichetta As String = ""
        Dim DoseEtichettaTemp As String = ""
        Dim Udm_Cod As Integer
        Dim Udm_Sim As String
        Dim AcquaMin, AcquaMax As Decimal
        Dim AcquaUdm_Cod As Integer
        Dim AcquaUdm_Sim As String
        Dim A_Epoca, Da_Epoca As Integer
        Dim Limite As Integer
        Dim LimiteUdm_Cod As Integer
        Dim LimiteUdm_Sim As String

        Dim Udm_Radice, per_ha_hl As Integer

        '------------
        If Me.ComboFormulati.Testo_Combo = "" Then
            Messaggi.AgroMsgBox("Selezionare un formulato.", Page, , Script_Panel)
            Exit Sub
        End If

        '------------

        Av_Cod = "0"
        Av_Gru = "0"
        Av_Des = ""
        Soglia_Value = ""
        Soglia_Des = ""


        If ControllaSelezioneAvversita(Av_Cod, Av_Gru, Av_Des, Soglia_Value, Soglia_Des) = False Then
            Messaggi.AgroMsgBox("Selezionare un'avversità/infestante.", Page, , Script_Panel)
            Exit Sub
        End If


        If Not IsNumeric(Txt_Dose_Formulati.Text) Then
            Messaggio = "Inserire una dose."
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Sub
        Else
            If InStr(Txt_Dose_Formulati.Text, ".") <> 0 Then
                Txt_Dose_Formulati.Text = Replace(Txt_Dose_Formulati.Text, ".", ",")
            End If
            If CDbl(Me.Txt_Dose_Formulati.Text) <= 0 Then
                Messaggio = "Non e' possibile inserire una dose negativa o nulla."
                Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                Exit Sub
            End If
        End If

        Mezzo = RBL_Dose_Formulati.SelectedValue

        Dim ArrayTmp() As String

        If ComboFormulati.Valore_Combo <> "" Then

            Dim ArrayTmpFrDes() As String
            ArrayTmpFrDes = Split(ComboFormulati.Testo_Combo, "---")

            ArrayTmp = Split(ComboFormulati.Valore_Combo, "£")
            If ArrayTmpFrDes IsNot Nothing Then
                FrDes = ArrayTmpFrDes(0)
            End If

            If ArrayTmp IsNot Nothing Then
                FrCod = ArrayTmp(0)
            End If

            '----------------------------------------------------------

            If ArrayTmp IsNot Nothing Then

                DoseEtichetta = ""
                Dim strEpoca As String = ""
                Dim Flag_Fioritura As Integer

                If ArrayTmp.Length > 1 Then

                    If ArrayTmp(3) <> "" Then
                        Carenza = ArrayTmp(3)
                    Else
                        Carenza = 0
                    End If

                    If ArrayTmp.Length > 4 Then
                        If ArrayTmp(4) <> "" Then
                            strPA_COD = ArrayTmp(4)
                        Else
                            strPA_COD = ""
                        End If
                    End If

                    If ArrayTmp.Length > 1 Then
                        If ArrayTmp(5) <> "" Then
                            strCLTOSS_COD = ArrayTmp(5)
                        Else
                            strCLTOSS_COD = 0
                        End If
                    End If

                    For i = 6 To ArrayTmp.Length - 1

                        Dim ArrayDose() As String
                        ArrayDose = Split(ArrayTmp(i), "$")

                        If ArrayDose IsNot Nothing AndAlso ArrayDose.Length > 0 Then

                            DoseMin = ArrayDose(1)
                            DoseMax = ArrayDose(2)
                            Udm_Cod = ArrayDose(3)
                            Udm_Sim = ArrayDose(4)

                            AcquaMin = ArrayDose(5)
                            AcquaMax = ArrayDose(6)
                            AcquaUdm_Cod = ArrayDose(7)
                            AcquaUdm_Sim = ArrayDose(8)

                            Da_Epoca = ArrayDose(9)
                            A_Epoca = ArrayDose(10)

                            Limite = ArrayDose(11)
                            LimiteUdm_Cod = ArrayDose(12)
                            LimiteUdm_Sim = ArrayDose(13)

                            'Flag_Fioritura = ArrayDose(14)

                            Select Case ArrayDose.Length
                                Case 17
                                    Flag_Fioritura = ArrayDose(14)

                                Case Else
                                    Flag_Fioritura = ArrayDose(15)
                            End Select

                            DoseEtichettaTemp = CreaStringa_DoseEtichetta(DoseMin, DoseMax, Udm_Sim, Udm_Cod,
                                                          AcquaMin, AcquaMax, AcquaUdm_Sim, AcquaUdm_Cod,
                                                          Limite, LimiteUdm_Sim, LimiteUdm_Cod,
                                                          Da_Epoca, A_Epoca, Flag_Fioritura)
                            If InStr(DoseEtichetta, DoseEtichettaTemp) = 0 Then

                                DoseEtichetta &= DoseEtichettaTemp

                                'verifico se è dose Ettaro o dose HL

                                ScomponiUdm(Udm_Radice, per_ha_hl, Udm_Cod)
                                If RBL_Dose_Formulati.Enabled OrElse
                                   (per_ha_hl = 2121 AndAlso RBL_Dose_Formulati.SelectedValue = "0") OrElse
                                   (per_ha_hl = 2123 AndAlso RBL_Dose_Formulati.SelectedValue = "1") Then

                                    If Udm_Radice <> -1 Then
                                        Select Case per_ha_hl
                                            Case 2121 'hl
                                                RBL_Dose_Formulati.SelectedValue = "0"
                                            Case 2123 'ha
                                                RBL_Dose_Formulati.SelectedValue = "1"
                                        End Select
                                        'Udm_Cod = Udm_Radice
                                    End If

                                Else


                                End If


                            End If

                        End If

                    Next

                    If DoseEtichetta <> "" Then
                        DoseEtichetta = Left(DoseEtichetta, DoseEtichetta.Length - 4)
                    End If

                End If


            End If

            '----
            'Acqua
            'Imposta_VolumiAcqua(AcquaMin, AcquaMax, AcquaUdm_Cod, RBL_Acqua_Formulati, Txt_Acqua_Difesa)
            ''----------------
            ''DOSE

            'Imposta_DosiEtichetta(DoseEtichetta)

            'Lbl_Dose_Etichetta.Visible = True

        End If

        'Disabilito i RadioButton
        Me.RBL_Dose_Formulati.Enabled = False

        'Inserisco il formulato nella griglia delle dosi
        Call Dosi_Inserisci_Difesa(Av_Cod, Av_Gru, Av_Des,
                                   FrCod, FrDes,
                                   Carenza.ToString,
                                   DoseEtichetta, DoseMax.ToString,
                                   Txt_Dose_Formulati.Text,
                                   ,
                                   Cmb_Udm_Formulati.SelectedValue,
                                   Cmb_Udm_Formulati.SelectedItem.Text,
                                   Mezzo, 0,
                                   Soglia_Value, Soglia_Des,
                                   strPA_COD, strCLTOSS_COD)


    End Sub

    'Private Function ControllaSelezioneAvversita(ByRef Av_Cod As String, ByRef Av_Gru As String, ByRef Av_Des As String, _
    '                                          ByRef Soglia_Value As String, ByRef Soglia_Des As String) As Boolean



    '    If Me.RBL_Avversita.SelectedValue = "0" Then
    '        For i = 0 To GridViewAvversita.Rows.Count - 1
    '            If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
    '                Av_Cod &= GridViewAvversita.Rows(i).Cells(1).Text() & ","
    '                Av_Des &= GridViewAvversita.Rows(i).Cells(2).Text() & ", "
    '                Av_Gru &= "0,"
    '                If GridViewAvversita.Rows(i).Cells(3).Visible = True Then
    '                    If CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).Items.Count > 0 Then
    '                        If Not CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).SelectedItem Is Nothing Then
    '                            Dim valore As String = CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).SelectedValue
    '                            Dim des As String = CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).SelectedItem.Text & _
    '                                            " di " & GridViewAvversita.Rows(i).Cells(2).Text()
    '                            Soglia_Value &= valore & ", "
    '                            Soglia_Des &= des & ", "
    '                        End If
    '                    End If
    '                End If
    '            End If
    '        Next
    '    Else
    '        For i = 0 To GridViewGruppiAvversita.Rows.Count - 1
    '            If CType(GridViewGruppiAvversita.Rows(i).FindControl("ChkSelezionaGruppoAvversita"), CheckBox).Checked = True Then
    '                Av_Cod &= "0,"
    '                Av_Gru &= GridViewGruppiAvversita.Rows(i).Cells(1).Text() & ","
    '                Av_Des &= GridViewGruppiAvversita.Rows(i).Cells(2).Text() & ", "
    '            End If
    '        Next
    '    End If

    '    If Av_Des = "" Then
    '        Messaggi.AgroMsgBox("E' necessario selezionare un'avversità!", Page, , Script_Panel)
    '        Return False
    '    End If

    '    If Av_Cod <> "0" Then
    '        Av_Cod = Left(Av_Cod, Av_Cod.Length - 1)
    '    End If
    '    If Av_Gru <> "0" Then
    '        Av_Gru = Left(Av_Gru, Av_Gru.Length - 1)
    '    End If
    '    If Av_Des <> "" Then
    '        Av_Des = Left(Av_Des, Av_Des.Length - 2)
    '    End If

    '    If Soglia_Value <> "" Then
    '        Soglia_Value = Left(Soglia_Value, Soglia_Value.Length - 2)
    '    End If
    '    If Soglia_Des <> "" Then
    '        Soglia_Des = Left(Soglia_Des, Soglia_Des.Length - 2)
    '    End If

    '    Return True

    'End Function

    Private Function ControllaSelezioneAvversita(ByRef Av_Cod As String, ByRef Av_Gru As String, ByRef Av_Des As String,
                                             ByRef Soglia_Value As String, ByRef Soglia_Des As String) As Boolean


        If Not IsNothing(Cmb_FormulatoClassificazioni.SelectedItem) AndAlso IsNumeric(Cmb_FormulatoClassificazioni.SelectedValue) Then


            Select Case CInt(Cmb_FormulatoClassificazioni.SelectedValue)

                Case enum_TipoFormulato.Coadiuvanti
                    Av_Cod = "-1"
                    Av_Gru = "-1"
                    Av_Des = ""
                    Soglia_Value = ""
                    Soglia_Des = ""

                Case enum_TipoFormulato.Corroboranti_Fisiofarmaci
                    Av_Cod = "-2"
                    Av_Gru = "-2"
                    Av_Des = ""
                    Soglia_Value = ""
                    Soglia_Des = ""

                Case enum_TipoFormulato.Fitoregolatori, enum_TipoFormulato.Disseccanti
                    Av_Cod = "-3"
                    Av_Gru = "-3"
                    Av_Des = ""
                    Soglia_Value = ""
                    Soglia_Des = ""

                Case Else

                    'Select Case ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue
                    '    Case "0"
                    '        Av_Cod = "0"
                    '        Av_Gru = "0"
                    '        Av_Des = ""
                    '        Soglia_Value = ""
                    '        Soglia_Des = ""
                    '    Case Else

                    If CellaAvversita.Visible Then

                        If Me.RBL_Avversita.SelectedValue = "0" Then
                            For i = 0 To GridViewAvversita.Rows.Count - 1
                                If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                                    Av_Cod &= GridViewAvversita.Rows(i).Cells(1).Text() & ","
                                    Av_Des &= GridViewAvversita.Rows(i).Cells(2).Text() & ", "
                                    Av_Gru &= "0,"
                                    If GridViewAvversita.Rows(i).Cells(3).Visible Then
                                        If CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).Items.Count > 0 Then
                                            If CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).SelectedItem IsNot Nothing Then
                                                Dim valore As String = CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).SelectedValue
                                                Dim des As String = CType(GridViewAvversita.Rows(i).Cells(3).FindControl("rbl_soglie_intervento"), RadioButtonList).SelectedItem.Text &
                                                                " di " & GridViewAvversita.Rows(i).Cells(2).Text()

                                                Soglia_Value &= valore & "| "
                                                Soglia_Des &= des & "| "
                                            End If
                                        End If

                                    End If
                                End If
                            Next
                        Else
                            For i = 0 To GridViewGruppiAvversita.Rows.Count - 1
                                If CType(GridViewGruppiAvversita.Rows(i).FindControl("ChkSelezionaGruppoAvversita"), CheckBox).Checked = True Then
                                    Av_Cod &= "0,"
                                    Av_Gru &= GridViewGruppiAvversita.Rows(i).Cells(1).Text() & ","
                                    Av_Des &= GridViewGruppiAvversita.Rows(i).Cells(2).Text() & ", "
                                End If
                            Next
                        End If

                        If Av_Des = "" Then
                            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ENecessarioSelezionareUnAvversità, Page, , Script_Panel)
                            Return False
                        End If

                        If Av_Cod <> "0" Then
                            Av_Cod = Left(Av_Cod, Av_Cod.Length - 1)
                        End If
                        If Av_Gru <> "0" Then
                            Av_Gru = Left(Av_Gru, Av_Gru.Length - 1)
                        End If
                        If Av_Des <> "" Then
                            Av_Des = Left(Av_Des, Av_Des.Length - 2)
                        End If

                        If Soglia_Value <> "" Then
                            Soglia_Value = Left(Soglia_Value, Soglia_Value.Length - 2)
                        End If
                        If Soglia_Des <> "" Then
                            Soglia_Des = Left(Soglia_Des, Soglia_Des.Length - 2)
                        End If
                        'End Select

                    End If



            End Select
            Return True
        End If
        Return True
    End Function

    '########################################################################################
    Private Sub Dosi_Inserisci_Difesa(ByVal Av_Cod As String,
                                      ByVal Av_Gru As String,
                                      ByVal Av_Des As String,
                                      ByVal Fr_Cod As Integer,
                                      ByVal Fr_Des As String,
                                      ByVal Carenza As String,
                                      ByVal Dose_Etichetta As String,
                                      ByVal Dose_Etichetta_Max As String,
                                      Optional ByVal Dose As String = "",
                                      Optional ByVal Dose_Fittizia As String = "",
                                      Optional ByVal Udm_Cod As Integer = 0,
                                      Optional ByVal Udm_Des As String = "",
                                      Optional ByVal Mezzo As String = "",
                                      Optional ByVal Qta_Tot As String = "",
                                      Optional ByVal Soglia_Value As String = "",
                                      Optional ByVal Soglia_Des As String = "",
                                      Optional ByVal strPA_COD As String = "",
                                      Optional ByVal strCLTOSS_COD As String = "")

        Dim i As Integer

        Dim Dt As DataTable
        Dim Dr As DataRow

        '----- Recupero i dati 

        'Recupero il datatable
        Dt = ViewState("dtDosi_Difesa")

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Fr_Cod") = Fr_Cod Then
                Messaggi.AgroMsgBox("Formulato già presente!", Page, , Script_Panel)
                Exit Sub
            End If
        Next

        'Creo una nuova riga
        Dr = Dt.NewRow

        Dr.Item("Av_Cod") = Av_Cod
        Dr.Item("Av_Gru") = Av_Gru
        Dr.Item("Av_Des") = Av_Des

        Dr.Item("Fr_Cod") = Fr_Cod

        If Fr_Des = "" Then
            Dim Formulati_r As New AgronicaCoreMetaSchemaDAL.Formulati_R
            Dr.Item("Fr_Des") = Formulati_r.FrDes_from_FrCod(Fr_Cod, objParametri_Server)
        Else
            Dr.Item("Fr_Des") = Fr_Des
        End If


        Dr.Item("Soglia_Value") = Soglia_Value
        Dr.Item("Soglia_Des") = Soglia_Des

        Dr.Item("Carenza") = Carenza

        Dr.Item("Dose_Etichetta") = Dose_Etichetta
        Dr.Item("Dose_Etichetta_Max") = Dose_Etichetta_Max

        Dr.Item("Udm_Des") = Udm_Des
        Dr.Item("Udm_Cod") = Udm_Cod
        Dr.Item("Dose") = Dose
        Dr.Item("Dose_Fittizia") = Dose_Fittizia
        Dr.Item("Mezzo") = Mezzo
        Dr.Item("Qta_Tot") = Qta_Tot

        Dr.Item("strPA_COD") = strPA_COD
        Dr.Item("strCLTOSS_COD") = strCLTOSS_COD

        'If DoseMax <> 0 Then

        '    Dr.Item("Dose_Etichetta" & i) = DoseEtichetta
        '    Dr.Item("Dose_Etichetta_Max" & i) = DoseMax.ToString

        '    Dr.Item("Dose" & i) = DoseMax.ToString
        '    Dr.Item("Dose_Fittizia" & i) = Math.Round(DoseMax, 3)

        'Else

        '    Dr.Item("Dose" & i) = 0
        '    Dr.Item("Dose_Fittizia" & i) = 0

        'End If


        'For i = 1 To N_For


        '    'imposto la combo delle unità di misura
        '    Select Case Udm_Cod

        '        Case 2, 14, 88, 169, 175, 318     'kg

        '            Dr.Item("Udm_Cod" & i) = 2

        '        Case 3, 20, 23, 37, 171, 174, 300, 301   'g

        '            Dr.Item("Udm_Cod" & i) = 3

        '        Case 22, 29, 173, 302, 303   'l

        '            Dr.Item("Udm_Cod" & i) = 29

        '        Case 21, 104   'cc

        '            Dr.Item("Udm_Cod" & i) = 104

        '        Case 101, 163, 164, 165, 170, 172   'ml

        '            Dr.Item("Udm_Cod" & i) = 101

        '        Case Else

        '            Dr.Item("Udm_Cod" & i) = -1

        '    End Select



        '    If Not Dose Is Nothing Then

        '        Dr.Item("Udm_Cod" & i) = UdmCod(i - 1)

        '        Select Case Me.RBL_Dose_Formulati.SelectedValue

        '            Case "1" 'dose/ha

        '                Dr.Item("Dose" & i) = Dose(i - 1)
        '                Dr.Item("Dose_Fittizia" & i) = Dose(i - 1)
        '                Dr.Item("Mezzo" & i) = 1
        '                Dr.Item("Qta_Tot" & i) = "---"

        '            Case "0" 'dose/hl

        '                Dr.Item("Dose" & i) = Dose(i - 1)
        '                Dr.Item("Dose_Fittizia" & i) = Dose(i - 1)
        '                Dr.Item("Mezzo" & i) = 0
        '                Dr.Item("Qta_Tot" & i) = Math.Round(CDbl(Dose(i - 1)) * CDbl(Me.Txt_Acqua_Difesa.Text), 3)

        '            Case "10" 'qta tot in acqua

        '                Dr.Item("Dose" & i) = Math.Round(CDbl(Dose(i - 1)) / CDbl(Me.Txt_Acqua_Difesa.Text), 3)
        '                Dr.Item("Dose_Fittizia" & i) = Dose(i - 1)
        '                Dr.Item("Mezzo" & i) = 0
        '                Dr.Item("Qta_Tot" & i) = Dose(i - 1)

        '        End Select

        '    End If

        'Next


        'FINE CICLO SU i




        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)


        '----- Associo il DataTable con la DataGrid

        GridView_Dosi_Difesa.DataSource = Dt
        GridView_Dosi_Difesa.DataBind()

        If Av_Cod = "0" AndAlso Av_Gru = "0" Then
            GridView_Dosi_Difesa.Columns(4).HeaderStyle.CssClass = "displaynone"
            GridView_Dosi_Difesa.Columns(6).HeaderStyle.CssClass = "displaynone"
            GridView_Dosi_Difesa.Columns(4).ItemStyle.CssClass = "displaynone"
            GridView_Dosi_Difesa.Columns(6).ItemStyle.CssClass = "displaynone"
        Else
            GridView_Dosi_Difesa.Columns(4).HeaderStyle.CssClass = ""
            GridView_Dosi_Difesa.Columns(6).HeaderStyle.CssClass = ""
            GridView_Dosi_Difesa.Columns(4).ItemStyle.CssClass = ""
            GridView_Dosi_Difesa.Columns(6).ItemStyle.CssClass = ""
        End If

        '----- Salvo il DataTable dentro il viewstate

        ViewState("dtDosi_Difesa") = Dt

        '----- Azzero i controlli di provenienza dei dati

        Svuota_Controlli_Difesa()

    End Sub

    '########################################################################################
    Private Sub Dosi_Inserisci_Concimazione(ByVal Fer_Cod As Integer,
                                      ByVal Fer_Des As String,
                                      ByVal Efficienza As String,
                                      ByVal N As String,
                                      ByVal N_Utile As String,
                                      ByVal P As String,
                                      ByVal K As String,
                                      ByVal Mg As String,
                                      Optional ByVal Dose As String = "",
                                      Optional ByVal Dose_Fittizia As String = "",
                                      Optional ByVal Udm_Cod As Integer = 0,
                                      Optional ByVal Udm_Des As String = "",
                                      Optional ByVal Mezzo As String = "",
                                      Optional ByVal Qta_Tot As String = "")

        Dim i As Integer

        Dim Dt As DataTable
        Dim Dr As DataRow

        Dt = ViewState("dtDosi_Concimazione")

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Fer_Cod") = Fer_Cod Then
                Messaggi.AgroMsgBox("Fertilizzante già presente!", Page, , Script_Panel)
                Exit Sub
            End If
        Next

        Dr = Dt.NewRow

        Dr.Item("Fer_Cod") = Fer_Cod

        If Fer_Des = "" Then
            Dim Fertilizzanti_R As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
            Dr.Item("Fer_Des") = Fertilizzanti_R.FerDes_from_FerCod(Fer_Cod, objParametri_Server)
        Else
            Dr.Item("Fer_Des") = Fer_Des
        End If

        Dr.Item("Efficienza") = Efficienza

        Dr.Item("N") = N
        Dr.Item("N_Utile") = N_Utile
        Dr.Item("P") = P
        Dr.Item("K") = K
        Dr.Item("Mg") = Mg

        Dr.Item("Udm_Cod") = Udm_Cod
        Dr.Item("Udm_Des") = Udm_Des
        Dr.Item("Dose") = Dose
        Dr.Item("Dose_Fittizia") = Dose_Fittizia
        Dr.Item("Mezzo") = Mezzo
        Dr.Item("Qta_Tot") = Qta_Tot

        Dt.Rows.Add(Dr)

        GridView_Dosi_Concimazione.DataSource = Dt
        GridView_Dosi_Concimazione.DataBind()


        '----- Salvo il DataTable dentro il viewstate

        ViewState("dtDosi_Concimazione") = Dt

        Svuota_Controlli_Concimazione()

    End Sub







    '########################################################################################
    Private Sub Ripristina_Dati_nei_Controlli(ByVal Lav_Cod As Integer,
                                              ByVal Xml_Operazione As String,
                                              ByVal Operazione_Richiesta As Integer)


        Dim XmlDoc As New System.Xml.XmlDocument

        Dim ArrayPannelli() As enum_TipoPannello
        Dim N_Pannelli As Integer = 0
        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Operazione)

        ReDim ArrayPannelli(N_Pannelli)
        ArrayPannelli(N_Pannelli) = enum_TipoPannello.Pannello_Testata
        N_Pannelli += 1

        'Imposto la selezione della combobox
        ComboOperazione.Valore_Combo = Lav_Cod.ToString

        Btn_Salva_Operazione.Visible = False
        Btn_Aggiungi_Dettaglio.Visible = False
        Btn_Annulla_Dettaglio.Visible = True

        Select Case Operazione_Richiesta
            Case enum_TipoOperazioneDB.Lettura
            Case enum_TipoOperazioneDB.Modifica
                Btn_Aggiungi_Dettaglio.Visible = True
            Case enum_TipoOperazioneDB.Cancellazione
            Case enum_TipoOperazioneDB.Trasferimento
                Btn_Salva_Operazione.Visible = True
        End Select



        Dim script As New StringBuilder

        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("             $('#dialogOperazione').dialog('open');")
        script.AppendLine("                 $('#WaitFrame').hide();")
        script.AppendLine("             $('#dialogOperazione').parent().appendTo($('form:first')); ")
        script.AppendLine("     });")
        ScriptManager.RegisterClientScriptBlock(UpdatePanelOperazione, UpdatePanelOperazione.GetType(),
                    String.Format("jQuery_{0}", UpdatePanelOperazione.ClientID), script.ToString, True)

        Select Case Lav_Cod

            '------------------------------------------------------------------------------
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                ImpostaCmbFormulatoClassificazioni_By_LavCod()
                Ripristina_Dati_nei_Controlli_Trattamento(Xml_Operazione)

                Select Case Operazione_Richiesta

                    Case enum_TipoOperazioneDB.Lettura
                        'Me.ImgBtn_SalvaConsiglio_Difesa.Visible = False
                        'Me.ImgBtn_Esci_Difesa.Visible = True
                        GridView_Dosi_Difesa.Columns(0).Visible = False
                        Me.GridView_Dosi_Difesa.Columns(1).Visible = False
                    Case enum_TipoOperazioneDB.Modifica
                        'Me.ImgBtn_SalvaConsiglio_Difesa.Visible = True
                        'Me.ImgBtn_Esci_Difesa.Visible = False
                        Me.GridView_Dosi_Difesa.Columns(0).Visible = True
                        Me.GridView_Dosi_Difesa.Columns(1).Visible = True
                    Case enum_TipoOperazioneDB.Cancellazione
                    Case enum_TipoOperazioneDB.Trasferimento
                        'Me.ImgBtn_SalvaConsiglio_Difesa.Visible = False
                        'Me.ImgBtn_Esci_Difesa.Visible = False
                        Me.GridView_Dosi_Difesa.Columns(0).Visible = True
                        Me.GridView_Dosi_Difesa.Columns(1).Visible = True
                End Select

                ReDim Preserve ArrayPannelli(N_Pannelli)
                ArrayPannelli(N_Pannelli) = enum_TipoPannello.PannelloDifesa
                N_Pannelli += 1

                '------------------------------------------------------------------------------
            Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                Ripristina_Dati_nei_Controlli_Concimazione(Xml_Operazione)

                Select Case Operazione_Richiesta
                    Case enum_TipoOperazioneDB.Lettura
                        'Me.ImgBtn_SalvaConsiglio_Concimazione.Visible = False
                        'Me.ImgBtn_Esci_Concimazione.Visible = True
                        Me.GridView_Dosi_Difesa.Columns(0).Visible = False
                        Me.GridView_Dosi_Difesa.Columns(1).Visible = False
                    Case enum_TipoOperazioneDB.Modifica
                        'Me.ImgBtn_SalvaConsiglio_Concimazione.Visible = True
                        'Me.ImgBtn_Esci_Concimazione.Visible = False
                        Me.GridView_Dosi_Difesa.Columns(0).Visible = True
                        Me.GridView_Dosi_Difesa.Columns(1).Visible = True
                    Case enum_TipoOperazioneDB.Cancellazione
                    Case enum_TipoOperazioneDB.Trasferimento
                        'Me.ImgBtn_SalvaConsiglio_Concimazione.Visible = False
                        'Me.ImgBtn_Esci_Concimazione.Visible = False
                        Me.GridView_Dosi_Difesa.Columns(0).Visible = True
                        Me.GridView_Dosi_Difesa.Columns(1).Visible = True
                End Select

                ReDim Preserve ArrayPannelli(N_Pannelli)
                ArrayPannelli(N_Pannelli) = enum_TipoPannello.PannelloConcimazione
                N_Pannelli += 1

                'LAVORAZIONI
            Case LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ANDANAMENTO, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
                 LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE,
                 LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ERPICATURA_ROTANTE, LAVCOD_ESPIANTO,
                 LAVCOD_ESTIRPATURA, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI,
                 LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_GEBIATURA, LAVCOD_IMBALLO_FIENO_ROTOLI,
                 LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_LAVORAZIONE_CONBINATA,
                 LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO,
                 LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE,
                 LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA,
                 LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA,
                 LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_ROMPICROSTA, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA,
                 LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_SOD_SEDDING, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA,
                 LAVCOD_ZAPPATURA, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_TRAPIANTO

                Ripristina_Dati_nei_Controlli_Lavorazione(Xml_Operazione)

                'Select Case Operazione_Richiesta
                '    Case enum_TipoOperazioneDB.Lettura
                '        Me.ImgBtn_SalvaConsiglio_Lavorazioni.Visible = False
                '        Me.ImgBtn_Esci_Lavorazioni.Visible = True
                '    Case enum_TipoOperazioneDB.Modifica
                '        Me.ImgBtn_SalvaConsiglio_Lavorazioni.Visible = True
                '        Me.ImgBtn_Esci_Lavorazioni.Visible = False
                '    Case enum_TipoOperazioneDB.Cancellazione
                '    Case enum_TipoOperazioneDB.Trasferimento
                '        Me.ImgBtn_SalvaConsiglio_Lavorazioni.Visible = False
                '        Me.ImgBtn_Esci_Lavorazioni.Visible = False
                'End Select

                ReDim Preserve ArrayPannelli(N_Pannelli)
                ArrayPannelli(N_Pannelli) = enum_TipoPannello.PannelloLavorazioni
                N_Pannelli += 1

            Case LAVCOD_IRRIGAZIONE

                Ripristina_Dati_nei_Controlli_Irrigazione(Xml_Operazione)

                'Select Case Operazione_Richiesta
                '    Case enum_TipoOperazioneDB.Lettura
                '        'Me.ImgBtn_SalvaConsiglio_Irrigazione.Visible = False
                '        'Me.ImgBtn_Esci_Irrigazione.Visible = True
                '    Case enum_TipoOperazioneDB.Modifica
                '        'Me.ImgBtn_SalvaConsiglio_Irrigazione.Visible = True
                '        'Me.ImgBtn_Esci_Irrigazione.Visible = False
                '    Case enum_TipoOperazioneDB.Cancellazione
                '    Case enum_TipoOperazioneDB.Trasferimento
                '        'Me.ImgBtn_SalvaConsiglio_Irrigazione.Visible = False
                '        'Me.ImgBtn_Esci_Irrigazione.Visible = False
                'End Select

                ReDim Preserve ArrayPannelli(N_Pannelli)
                ArrayPannelli(N_Pannelli) = enum_TipoPannello.PannelloIrrigazione
                N_Pannelli += 1

            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA, LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                Ripristina_Dati_nei_Controlli_Trappole(Xml_Operazione)

                'Select Case Operazione_Richiesta
                '    Case enum_TipoOperazioneDB.Lettura
                '        '    Me.ImgBtn_SalvaConsiglio_Trappole.Visible = False
                '        '    Me.ImgBtn_Esci_Trappole.Visible = True
                '    Case enum_TipoOperazioneDB.Modifica
                '        'Me.ImgBtn_SalvaConsiglio_Trappole.Visible = True
                '        'Me.ImgBtn_Esci_Trappole.Visible = False
                '    Case enum_TipoOperazioneDB.Cancellazione
                '    Case enum_TipoOperazioneDB.Trasferimento
                '        'Me.ImgBtn_SalvaConsiglio_Trappole.Visible = False
                '        'Me.ImgBtn_Esci_Trappole.Visible = False
                'End Select

                ReDim Preserve ArrayPannelli(N_Pannelli)
                ArrayPannelli(N_Pannelli) = enum_TipoPannello.PannelloTrappole
                N_Pannelli += 1
                '    '------------------------------------------------------------------------------



            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                Btn_Salva_Operazione.Visible = False
                Btn_Aggiungi_Dettaglio.Visible = True
                Btn_Annulla_Dettaglio.Visible = False


                CaricaFasiFenologiche()
                Ripristina_Dati_nei_Controlli_RilieviAvvAus(Xml_Operazione)

                ReDim Preserve ArrayPannelli(N_Pannelli)
                ArrayPannelli(N_Pannelli) = enum_TipoPannello.Pannello_RilievoAvvAus
                N_Pannelli += 1

            Case Else

                'ArrayPannelli(0) = enum_TipoPannello.Nessun_Pannello
                Exit Sub

        End Select

        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
            ReDim Preserve ArrayPannelli(N_Pannelli)
            ArrayPannelli(N_Pannelli) = enum_TipoPannello.Pannello_Costi
            N_Pannelli += 1
        End If

        'Select Case Operazione_Richiesta
        '    Case enum_TipoOperazioneDB.Trasferimento
        '        ReDim Preserve ArrayPannelli(N_Pannelli)
        '        ArrayPannelli(N_Pannelli) = enum_TipoPannello.Pannello_Impianti
        '        N_Pannelli += 1
        'End Select

        If GridView_Impianti.Rows.Count > 0 Then
            ReDim Preserve ArrayPannelli(N_Pannelli)
            ArrayPannelli(N_Pannelli) = enum_TipoPannello.Pannello_Impianti
            N_Pannelli += 1
        End If

        Imposta_Pannelli(ArrayPannelli)


    End Sub

    '########################################################################################
    Private Sub Ripristina_Dati_nei_Controlli_Trattamento(ByVal Xml_Operazione As String)

        'per capire se è una ricetta vecchia, senza i det tecnici dei dettagli
        Dim RicettaVecchiaSenzaMovDetTEcn2 As Boolean = False

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement


        Dim xDatiRicettaxNote As XmlElement
        Dim xListaRicettaxNote As XmlNodeList
        Dim xRicettaxNote As XmlElement

        Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_RicettaDestinazione As System.Xml.XmlElement
        Dim XMLs_RicettaDestinazione As System.Xml.XmlNodeList

        Dim frm_Piva() As String
        Dim frm_SaCod() As Integer
        Dim frm_Appezza() As Integer
        Dim frm_IdReg() As Integer

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim i As Integer
        Dim j As Integer

        Dim frm_LavCod As Integer
        Dim frm_NoteIntervento As String
        Dim frm_CodDisciplinare As Integer
        Dim frm_DisciplinarePP As Integer
        Dim frm_Modulo As Integer
        Dim frm_IdRcdpi As Integer = 0
        Dim frm_Mezzo As Integer

        Dim frm_AvCod() As String
        Dim frm_AvGru() As String
        Dim frm_AvDes() As String
        Dim frm_AvGruDes() As String
        Dim frm_SogliaValue() As String
        Dim frm_SogliaDes() As String

        Dim frm_Acqua As Decimal = 0

        Dim frm_MiscelaCod() As Integer
        Dim frm_FrCod() As Integer
        Dim frm_FrDes() As String
        Dim frm_UdmCod() As Integer
        Dim frm_UdmSim() As String
        Dim frm_UdmCodTrasformato() As Integer
        Dim frm_Dose() As Decimal
        Dim frm_TempoCarenza() As Integer
        Dim frm_DoseEtichetta() As String
        Dim frm_PrincipiAttivi() As String
        Dim frm_ClassiTossicologiche() As String

        Dim TipoTestata As Integer

        Dim Cau_Mov As String

        Dim Nota_Cod As Integer

        Dim Udm_Cod_Costi As Integer = -1
        Dim Udm_Des_Costi As String = "Indefinito"
        Dim Costo_Unitario As String = "0"
        Dim Costo As String = "0"
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""

        Dim Centro As String
        Dim Tipo_Centro As String
        Dim Centro_Cod As Integer

        Dim Piva As String = ""
        If ViewState("Piva") IsNot Nothing Then
            Piva = ViewState("Piva")
        End If

        Costruisci_DT_Scarico()

        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

        CaricaListControl.Note_Intervento(CBL_Consigli_Difesa,
                                          False, "", "",
                                          0, -1,
                                          "", "",
                                          objParametri_Server)

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Operazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_LavCod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        'Disciplinari
        frm_CodDisciplinare = CInt(XML_Ricetta_Operazione.GetAttribute("num_protocollo"))


        'Rcdpi - Modulo
        If XML_Ricetta_Operazione.GetAttribute("num_protocollo") <> "0" Then
            If XML_Ricetta_Operazione.GetAttribute("id_rcdpi") <> "0" Then
                frm_IdRcdpi = CInt(XML_Ricetta_Operazione.GetAttribute("id_rcdpi"))
            End If
            If XML_Ricetta_Operazione.GetAttribute("extra_int") <> "0" Then
                frm_Modulo = CInt(XML_Ricetta_Operazione.GetAttribute("extra_int"))
            End If
            If XML_Ricetta_Operazione.GetAttribute("disciplinare_pubblicoprivato") <> "0" Then
                frm_DisciplinarePP = CInt(XML_Ricetta_Operazione.GetAttribute("disciplinare_pubblicoprivato"))
            End If
        End If

        frm_Mezzo = CInt(XML_Ricetta_Operazione.GetAttribute("mezzo"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            ' ----- NOTE

            xDatiRicettaxNote = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")

            If xDatiRicettaxNote IsNot Nothing Then

                xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")

                If xListaRicettaxNote IsNot Nothing Then

                    For i = 0 To xListaRicettaxNote.Count - 1

                        xRicettaxNote = xListaRicettaxNote.Item(i)

                        Nota_Cod = CInt(xRicettaxNote.GetAttribute("nota_cod"))

                        For j = 0 To CBL_Consigli_Difesa.Items.Count - 1
                            If Nota_Cod = CInt(CBL_Consigli_Difesa.Items(j).Value) Then
                                CBL_Consigli_Difesa.Items(j).Selected = True
                                Exit For
                            End If
                        Next

                    Next

                End If

            End If



            '----- Tag DatiRicetta_Dettagli_Tecnici

            XML_DatiRicetta_Dettagli_Tecnici = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli_Tecnici")

            If XML_DatiRicetta_Dettagli_Tecnici IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio_Tecnico  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio_Tecnico = XML_DatiRicetta_Dettagli_Tecnici.GetElementsByTagName("Ricetta_Dettaglio_Tecnico")

                Dim xAvCod As Integer
                Dim xAvGru As Integer
                Dim xAvDes As String
                Dim xAvGruDes As String
                Dim xQtaAcqua As Decimal

                'Ciclo su tutti i nodi
                For i = 0 To XMLs_Ricetta_Dettaglio_Tecnico.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio_Tecnico = XMLs_Ricetta_Dettaglio_Tecnico.Item(i)

                    'Recupero i valori
                    xAvCod = CInt(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_cod"))
                    xAvGru = CInt(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_gru"))
                    xAvDes = CStr(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_des_vol"))
                    xAvGruDes = CStr(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_gru_des"))
                    xQtaAcqua = CDbl(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("qta_ril"))

                    If (xAvCod = 0) AndAlso (xAvGru = 0) AndAlso (xQtaAcqua <> 0) Then
                        'Acqua
                        frm_Acqua = xQtaAcqua
                    ElseIf (xAvCod <> 0 OrElse xAvGru <> 0) AndAlso (xQtaAcqua = 0) Then
                        If frm_CodDisciplinare = 0 Then
                            'Avversita'
                            If frm_AvCod Is Nothing Then
                                ReDim Preserve frm_AvCod(0)
                                ReDim Preserve frm_AvGru(0)
                                ReDim Preserve frm_AvDes(0)
                                ReDim Preserve frm_AvGruDes(0)
                            Else
                                ReDim Preserve frm_AvCod(UBound(frm_AvCod) + 1)
                                ReDim Preserve frm_AvGru(UBound(frm_AvGru) + 1)
                                ReDim Preserve frm_AvDes(UBound(frm_AvDes) + 1)
                                ReDim Preserve frm_AvGruDes(UBound(frm_AvGruDes) + 1)
                            End If

                            frm_AvCod(UBound(frm_AvCod)) = xAvCod
                            frm_AvGru(UBound(frm_AvGru)) = xAvGru
                            frm_AvDes(UBound(frm_AvDes)) = xAvDes
                            frm_AvGruDes(UBound(frm_AvGruDes)) = xAvGruDes

                        End If
                    End If
                Next
            End If

            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If XML_DatiRicetta_Dettagli IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case Cau_Mov

                        Case "", CAU_TRATTAMENTO

                            If i = 0 Then

                                XMLs_RicettaDestinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                                If XMLs_RicettaDestinazione IsNot Nothing AndAlso XMLs_RicettaDestinazione.Count > 0 Then

                                    For j = 0 To XMLs_RicettaDestinazione.Count - 1

                                        XML_RicettaDestinazione = XMLs_RicettaDestinazione.Item(j)

                                        If frm_Piva Is Nothing Then
                                            ReDim Preserve frm_Piva(0)
                                            ReDim Preserve frm_SaCod(0)
                                            ReDim Preserve frm_Appezza(0)
                                            ReDim Preserve frm_IdReg(0)
                                        Else
                                            ReDim Preserve frm_Piva(UBound(frm_Piva) + 1)
                                            ReDim Preserve frm_SaCod(UBound(frm_SaCod) + 1)
                                            ReDim Preserve frm_Appezza(UBound(frm_Appezza) + 1)
                                            ReDim Preserve frm_IdReg(UBound(frm_IdReg) + 1)
                                        End If

                                        frm_Piva(UBound(frm_Piva)) = CStr(XML_RicettaDestinazione.GetAttribute("piva"))
                                        frm_SaCod(UBound(frm_SaCod)) = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                                        frm_Appezza(UBound(frm_Appezza)) = CInt(XML_RicettaDestinazione.GetAttribute("appezza"))
                                        frm_IdReg(UBound(frm_IdReg)) = CInt(XML_RicettaDestinazione.GetAttribute("id_reg"))

                                    Next

                                End If

                            End If

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If XMLs_Ricetta_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then

                                Select Case frm_CodDisciplinare

                                    Case Is <> 0 'DPI 2005
                                        If i = 0 Then
                                            'If frm_AvCod Is Nothing Then
                                            ReDim Preserve frm_AvCod(0)
                                            ReDim Preserve frm_AvGru(0)
                                            ReDim Preserve frm_AvDes(0)
                                            ReDim Preserve frm_AvGruDes(0)
                                            ReDim Preserve frm_SogliaValue(0)
                                            ReDim Preserve frm_SogliaDes(0)
                                        Else
                                            ReDim Preserve frm_AvCod(UBound(frm_AvCod) + 1)
                                            ReDim Preserve frm_AvGru(UBound(frm_AvGru) + 1)
                                            ReDim Preserve frm_AvDes(UBound(frm_AvDes) + 1)
                                            ReDim Preserve frm_AvGruDes(UBound(frm_AvGruDes) + 1)
                                            ReDim Preserve frm_SogliaValue(UBound(frm_SogliaValue) + 1)
                                            ReDim Preserve frm_SogliaDes(UBound(frm_SogliaDes) + 1)
                                        End If

                                End Select

                                For j = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(j)



                                    frm_AvCod(UBound(frm_AvCod)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod")) & ","
                                    frm_AvGru(UBound(frm_AvGru)) &= CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_gru")) & ","

                                    frm_AvDes(UBound(frm_AvDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_des_vol")) & ","
                                    frm_AvGruDes(UBound(frm_AvGruDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_gru_des")) & ","

                                    frm_SogliaValue(UBound(frm_SogliaValue)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod")) & "_" & CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("soglia_quantita")) & "_" & CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("soglia_cod")) & ","
                                    frm_SogliaDes(UBound(frm_SogliaDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("soglia_des")) & ","

                                Next

                                If frm_AvCod(UBound(frm_AvCod)) <> "" Then
                                    frm_AvCod(UBound(frm_AvCod)) = Left(frm_AvCod(UBound(frm_AvCod)), frm_AvCod(UBound(frm_AvCod)).Length - 1)
                                End If
                                If frm_AvGru(UBound(frm_AvGru)) <> "" Then
                                    frm_AvGru(UBound(frm_AvGru)) = Left(frm_AvGru(UBound(frm_AvGru)), frm_AvGru(UBound(frm_AvGru)).Length - 1)
                                End If
                                If frm_AvDes(UBound(frm_AvDes)) <> "" Then
                                    frm_AvDes(UBound(frm_AvDes)) = Left(frm_AvDes(UBound(frm_AvDes)), frm_AvDes(UBound(frm_AvDes)).Length - 1)
                                End If
                                If frm_AvGruDes(UBound(frm_AvGruDes)) <> "" Then
                                    frm_AvGruDes(UBound(frm_AvGruDes)) = Left(frm_AvGruDes(UBound(frm_AvGruDes)), frm_AvGruDes(UBound(frm_AvGruDes)).Length - 1)
                                End If
                                If frm_SogliaValue(UBound(frm_SogliaValue)) <> "" Then
                                    frm_SogliaValue(UBound(frm_SogliaValue)) = Left(frm_SogliaValue(UBound(frm_SogliaValue)), frm_SogliaValue(UBound(frm_SogliaValue)).Length - 1)
                                End If
                                If frm_SogliaDes(UBound(frm_SogliaDes)) <> "" Then
                                    frm_SogliaDes(UBound(frm_SogliaDes)) = Left(frm_SogliaDes(UBound(frm_SogliaDes)), frm_SogliaDes(UBound(frm_SogliaDes)).Length - 1)
                                End If


                            Else
                                'non ho il det tecnico2, significa che sono nel caso di una ricetta vecchia
                                'che aveva entrambi i dettagli tecnici collegati all'operazione
                                RicettaVecchiaSenzaMovDetTEcn2 = True

                            End If

                            'Inizializzo le variabili

                            If frm_FrCod Is Nothing Then

                                ReDim Preserve frm_FrCod(0)
                                ReDim Preserve frm_FrDes(0)
                                ReDim Preserve frm_MiscelaCod(0)
                                ReDim Preserve frm_UdmCod(0)
                                ReDim Preserve frm_UdmSim(0)
                                ReDim Preserve frm_Dose(0)
                                ReDim Preserve frm_UdmCodTrasformato(0)
                                ReDim Preserve frm_TempoCarenza(0)
                                ReDim Preserve frm_DoseEtichetta(0)
                                ReDim Preserve frm_PrincipiAttivi(0)
                                ReDim Preserve frm_ClassiTossicologiche(0)

                            Else

                                ReDim Preserve frm_FrCod(UBound(frm_FrCod) + 1)
                                ReDim Preserve frm_FrDes(UBound(frm_FrDes) + 1)
                                ReDim Preserve frm_MiscelaCod(UBound(frm_MiscelaCod) + 1)
                                ReDim Preserve frm_UdmCod(UBound(frm_UdmCod) + 1)
                                ReDim Preserve frm_UdmSim(UBound(frm_UdmSim) + 1)
                                ReDim Preserve frm_Dose(UBound(frm_Dose) + 1)
                                ReDim Preserve frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato) + 1)
                                ReDim Preserve frm_TempoCarenza(UBound(frm_TempoCarenza) + 1)
                                ReDim Preserve frm_DoseEtichetta(UBound(frm_DoseEtichetta) + 1)
                                ReDim Preserve frm_PrincipiAttivi(UBound(frm_PrincipiAttivi) + 1)
                                ReDim Preserve frm_ClassiTossicologiche(UBound(frm_ClassiTossicologiche) + 1)
                            End If

                            'Recupero i valori
                            frm_MiscelaCod(UBound(frm_MiscelaCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("miscela_cod"))
                            frm_FrCod(UBound(frm_FrCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))
                            frm_FrDes(UBound(frm_FrDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("fr_des"))
                            frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("udm_cod"))
                            frm_UdmCod(UBound(frm_UdmCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("extra_int"))

                            'sbagliato, in "udm_sim" dell'xml non c'è la desciz di extra_int ma di udm_cod, che è kg, l sempre
                            'frm_UdmSim(UBound(frm_UdmSim)) = XML_Ricetta_Dettaglio.GetAttribute("udm_sim")
                            Dim udmsim As String = ""
                            Dim udmdes As String = New AgronicaCoreMetaSchemaDAL.UnitaMisura_R().UdmDes_from_UdmCod(frm_UdmCod(UBound(frm_UdmCod)), udmsim, objParametri_Server)
                            frm_UdmSim(UBound(frm_UdmSim)) = udmsim

                            frm_Dose(UBound(frm_Dose)) = CDbl(XML_Ricetta_Dettaglio.GetAttribute("qta"))

                            If IsNumeric(XML_Ricetta_Dettaglio.GetAttribute("tempocarenza")) Then
                                frm_TempoCarenza(UBound(frm_TempoCarenza)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("tempocarenza"))
                            Else
                                frm_TempoCarenza(UBound(frm_TempoCarenza)) = 0
                            End If
                            frm_DoseEtichetta(UBound(frm_DoseEtichetta)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("doseetichetta"))

                            frm_PrincipiAttivi(UBound(frm_PrincipiAttivi)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("principiattivi"))
                            frm_ClassiTossicologiche(UBound(frm_ClassiTossicologiche)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("classitossicologiche"))




                        Case CAU_IMPUTAZIONE_PARCOMACCHINE

                            Categoria_Des = ""
                            Risorsa_Des = ""
                            Centro = "Parco Macchine"
                            Tipo_Centro = "PM"
                            Centro_Cod = -1

                            Dim DTMacchina As DataTable

                            Dim MacCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                            DTMacchina = objContab.MacchinaDes(Piva,
                                                       objParametriAgenda.Data,
                                                       MacCod,
                                                        "", "",
                                                       objParametri_Server)
                            objContab = Nothing


                            If DTMacchina.Rows.Count > 0 Then

                                Categoria_Des = DTMacchina.Rows(0).Item("Class_Desc")
                                If DTMacchina.Rows(0).Item("Mac_Des") <> "" Then
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Mac_Des")
                                Else
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Ditta_Des") & " " & DTMacchina.Rows(0).Item("Modello")
                                End If

                                InserisciRiga_dtScarico(Centro,
                                                        Centro_Cod,
                                                        Categoria_Des,
                                                        Risorsa_Des,
                                                        Udm_Des_Costi,
                                                        0,
                                                        Udm_Cod_Costi,
                                                        MACCHINE,
                                                        0,
                                                        Tipo_Centro,
                                                        0,
                                                        0,
                                                        MacCod,
                                                        Costo_Unitario,
                                                        Costo,
                                                        dtScarico)
                            End If


                        Case CAU_IMPUTAZIONE_MANODOPERA,
                             CAU_IMPUTAZIONE_TERZISTI,
                             CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            Dim Dt_Manodopera As DataTable
                            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                            Dim MatCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim Str As String
                            Str = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 ) "
                            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(Piva,
                                                                                                     MatCod,
                                                                                                     False,
                                                                                                     False,
                                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                      Str, "", objParametri_Server)




                            If Dt_Manodopera.Rows.Count = 0 Then
                                Exit For
                            End If

                            Dim Cod_Rapporto As String = ""
                            Dim Elem_Cod As Integer
                            Tipo_Centro = ""
                            Centro = ""
                            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

                            Select Case Cod_Rapporto
                                Case -1, -4, -6
                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                Case -5
                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                Case -12
                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                            End Select

                            If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
                                Centro_Cod = -3
                                Centro = "C/Terzisti"
                                Tipo_Centro = "CT"
                            End If

                            If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"
                                Centro_Cod = -2
                            End If

                            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                            Elem_Cod = 0

                            Dim Mat_Cod, Pro_Cod As Integer
                            Pro_Cod = 0
                            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")


                            InserisciRiga_dtScarico(Centro,
                                                            Centro_Cod,
                                                            Categoria_Des,
                                                            Risorsa_Des,
                                                            Udm_Des_Costi,
                                                            0,
                                                            Udm_Cod_Costi,
                                                            Elem_Cod,
                                                            0,
                                                            Tipo_Centro,
                                                            Pro_Cod,
                                                            0,
                                                            Mat_Cod,
                                                            Costo_Unitario,
                                                            Costo,
                                                            dtScarico)


                    End Select

                Next

            End If

        End If

        Session("dtScarico") = dtScarico
        AggiornaGridViewCostiAccessoriVisibili()

        'FINE SPACCHETTAMENTO

        '------------------------------------------
        '----- Combobox     DISCIPLINARE
        '------------------------------------------

        Select Case frm_LavCod
            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_DISSECCAMENTO
                CellaAvversita.Visible = False
            Case Else

                Cmb_Disciplinare.Enabled = True

                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(Me.Txt_DataInizio.Text), CDate(Me.Txt_DataFine.Text))
                Dim objCaricaCombo As New AgronicaCoreDpiBIZ.CaricaListControl

                objCaricaCombo.Disciplinari_ElencoxTestata(Me.Cmb_Disciplinare,
                                                            False, "", "",
                                                            Session,
                                                            objParametri_Server,
                                                            objParametri_Utenti,
                                                            CInt(0),
                                                            CInt(Cmb_Specie.SelectedValue),
                                                            CInt(0),
                                                            CInt(0),
                                                            CDate(Me.Txt_DataInizio.Text),
                                                            CDate(Me.Txt_DataFine.Text),
                                                            True, "",
                                                            False, "",
                                                            False,
                                                            TipoTestata)

                objParametri_Server.ResettaFinestra()

                'If frm_CodDisciplinare = 0 Then
                '    Cmb_Disciplinare.SelectedIndex = _
                '        Cmb_Disciplinare.Items.IndexOf(Cmb_Disciplinare.Items.FindByValue( _
                '            frm_CodDisciplinare))
                'ElseIf frm_CodDisciplinare = -1 Then
                '    Cmb_Disciplinare.SelectedIndex = _
                '        Cmb_Disciplinare.Items.IndexOf(Cmb_Disciplinare.Items.FindByValue( _
                '            frm_CodDisciplinare))
                'Else
                If True Then
                    Dim c As Integer
                    Dim Array() As String
                    Dim Dpi_Cod As Integer = 0
                    Dim IdRcdpi As Integer = 0
                    Dim Grfi_Cod As Integer = 0
                    Dim Flag_Protetto As Integer = 0
                    Dim Flag_PubblicoPrivato As Integer = 0
                    Dim Disciplinare_Valore As String
                    For c = 1 To Cmb_Disciplinare.Items.Count - 1
                        Array = Split(Cmb_Disciplinare.Items(c).Value, "/")
                        Dpi_Cod = Array(0)
                        'verifico se il dpi dell'operazione è lo stesso..
                        If Dpi_Cod = frm_CodDisciplinare Then
                            IdRcdpi = Array(1)
                            Flag_PubblicoPrivato = Array(4)
                            'verifico se la finalità è = 0 (= tutte x il DPI)
                            If (IdRcdpi = frm_IdRcdpi OrElse frm_IdRcdpi = 0) AndAlso Flag_PubblicoPrivato = frm_DisciplinarePP Then
                                Disciplinare_Valore = Cmb_Disciplinare.Items(c).Value
                                Exit For
                            End If
                        End If
                    Next

                    Cmb_Disciplinare.SelectedIndex =
                        Cmb_Disciplinare.Items.IndexOf(Cmb_Disciplinare.Items.FindByValue(
                            Disciplinare_Valore))

                    cella_disciplinare.Visible = True


                End If


                Select Case frm_CodDisciplinare

                    Case 0, -1

                    Case Else 'disciplinare 

                        Me.btn_Disciplinari_Click(Me, Nothing)

                        If Me.Cmb_Epoca.Items.Count > 0 Then

                            'Imposto la selezione della combobox
                            Cmb_Epoca.SelectedIndex =
                                Cmb_Epoca.Items.IndexOf(Cmb_Epoca.Items.FindByValue(
                                    frm_Modulo))

                        End If

                End Select


                '------------------------------------------
                '----- DataGrid     AVVERSITA
                '------------------------------------------

                CaricaGriglia_Avversita()

        End Select


        '------------------------------------------
        '----- DataGrid     DOSI
        '------------------------------------------

        Dim Dt As DataTable
        Dim strAvversita As String
        Dim ArrayAvCodTmp As String()

        'Creo la struttura della griglia
        CaricaGriglia_Dosi_Difesa()

        'Recupero il datatable
        Dt = ViewState("dtDosi_Difesa")

        For i = 0 To UBound(frm_FrCod)

            strAvversita = ""


            Select Case frm_LavCod

                Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_DISSECCAMENTO
                    Dim strAvCod As String = "0"
                    If frm_AvCod.Length > UBound(frm_FrCod) AndAlso IsNumeric(frm_AvCod(i)) Then
                        strAvCod = CStr(frm_AvCod(i))
                    End If

                    Dim strAvGru As String = "0"
                    If frm_AvGru.Length > UBound(frm_FrCod) AndAlso IsNumeric(frm_AvGru(i)) Then
                        strAvGru = CStr(frm_AvGru(i))
                    End If

                    Dosi_Inserisci_Difesa(strAvCod,
                                          strAvGru,
                                          "",
                                          frm_FrCod(i),
                                          frm_FrDes(i),
                                          frm_TempoCarenza(i),
                                          frm_DoseEtichetta(i), "",
                                          frm_Dose(i),
                                          Math.Round(frm_Dose(i)).ToString,
                                          frm_UdmCod(i),
                                          frm_UdmSim(i),
                                          frm_Mezzo,
                                          "0",
                                          "",
                                          "",
                                          frm_PrincipiAttivi(i),
                                          frm_ClassiTossicologiche(i))
                Case Else



                    If RicettaVecchiaSenzaMovDetTEcn2 Then

                        'Caso ricette vecchie

                        'se apro una ricetta vecchia questa potrebbe non avere il dettaglio tecnico 2, cioè il dettaglio tecnico del dettaglio
                        'quindi frm_SogliaValue e frm_SogliaDes non sono stati inizializzati, 
                        'quindi faccio un controllo e li imposto

                        Dim sogliavalue As String = "0"
                        Dim sogliaDes As String = ""
                        'If Not IsNothing(frm_SogliaValue) AndAlso frm_SogliaValue.Count > i Then
                        '    sogliavalue = frm_SogliaValue(i)
                        'End If
                        'If Not IsNothing(frm_SogliaDes) AndAlso frm_SogliaDes.Count > i Then
                        '    sogliaDes = frm_SogliaDes(i)
                        'End If


                        'Inoltre dato che le avversità sono nel det tecnico dell'operazione
                        'e non sono in ciascun dettaglio (formulato), allora
                        'per ciascun formulato attacco l'elenco delel avversità, che sono state lette dal dettaglio tecnico e non dal det tecnico2
                        Dim strAvCod As String = ""
                        Dim strAvGru As String = ""
                        Dim strAvDes As String = ""
                        Dim strAvGruDes As String = ""
                        For kk = 0 To frm_AvDes.Count - 1
                            If kk = frm_AvDes.Count - 1 Then
                                strAvCod &= CStr(frm_AvCod(kk))
                                strAvGru &= CStr(frm_AvGru(kk))
                                strAvDes &= CStr(frm_AvDes(kk))
                                strAvGruDes &= CStr(frm_AvGruDes(kk))
                            Else
                                strAvCod &= CStr(frm_AvCod(kk)) & ","
                                strAvGru &= CStr(frm_AvGru(kk)) & ","
                                strAvDes &= CStr(frm_AvDes(kk)) & ","
                                strAvGruDes &= CStr(frm_AvGruDes(kk)) & ","
                            End If

                        Next



                        ArrayAvCodTmp = Split(strAvCod, ",")
                        If ArrayAvCodTmp IsNot Nothing AndAlso ArrayAvCodTmp.Length > 0 Then
                            If ArrayAvCodTmp(0) = "0" Then
                                strAvversita = strAvGruDes
                            Else
                                strAvversita = strAvDes
                            End If
                        End If


                        Dosi_Inserisci_Difesa(strAvCod,
                                              strAvGru,
                                              strAvversita,
                                              frm_FrCod(i),
                                              frm_FrDes(i),
                                              frm_TempoCarenza(i),
                                              frm_DoseEtichetta(i), "",
                                              frm_Dose(i),
                                              Math.Round(frm_Dose(i)).ToString,
                                              frm_UdmCod(i),
                                              frm_UdmSim(i),
                                              frm_Mezzo,
                                              "0",
                                              sogliavalue,
                                              sogliaDes,
                                          frm_PrincipiAttivi(i),
                                          frm_ClassiTossicologiche(i))


                    Else

                        'Caso Standard, ricette nuove

                        ArrayAvCodTmp = Split(frm_AvCod(i), ",")
                        If ArrayAvCodTmp IsNot Nothing AndAlso ArrayAvCodTmp.Length > 0 Then
                            If ArrayAvCodTmp(0) = "0" Then
                                strAvversita = frm_AvGruDes(i)
                            Else
                                strAvversita = frm_AvDes(i)
                            End If
                        End If


                        Dosi_Inserisci_Difesa(frm_AvCod(i),
                                              frm_AvGru(i),
                                              strAvversita,
                                              frm_FrCod(i),
                                              frm_FrDes(i),
                                              frm_TempoCarenza(i),
                                              frm_DoseEtichetta(i), "",
                                              frm_Dose(i),
                                              Math.Round(frm_Dose(i)).ToString,
                                              frm_UdmCod(i),
                                              frm_UdmSim(i),
                                              frm_Mezzo,
                                              "0",
                                              frm_SogliaValue(i),
                                              frm_SogliaDes(i),
                                              frm_PrincipiAttivi(i),
                                              frm_ClassiTossicologiche(i))


                    End If



            End Select


        Next


        Select Case frm_Mezzo
            Case "1"
                Me.RBL_Dose_Formulati.SelectedValue = "1"
            Case "0"
                Me.RBL_Dose_Formulati.SelectedValue = "0"
        End Select

        'se l'acqua è stata salvata negativa significa che è stata salvata/ha, altrimenti totale
        If frm_Acqua < 0 Then
            Me.RBL_Acqua_Formulati.SelectedValue = "1"
        Else
            Me.RBL_Acqua_Formulati.SelectedValue = "0"
        End If

        Me.Txt_Acqua_Difesa.Text = Math.Abs(frm_Acqua)

        'Disabilito i RadioButton
        Me.RBL_Dose_Formulati.Enabled = False

        ''creo il dt dei formulati dal vettore miscele
        'Scombinatore_Difesa(Miscele, Dt)

        '------------------------------------------
        '----- Textbox     Note
        '------------------------------------------

        Me.Txt_Note_Difesa.Text = frm_NoteIntervento

        '------------------------------------------
        '----- IMPIANTI
        '------------------------------------------

        If frm_Piva IsNot Nothing AndAlso frm_Piva.Length > 0 Then
            Dim strFiltroImpianti As New System.Text.StringBuilder
            Dim Filtro As String = ""
            'Dim Piva As String = ""
            strFiltroImpianti.Length = 0
            For i = 0 To frm_Piva.Length - 1
                If i = 0 Then
                    Piva = frm_Piva(i)
                End If
                strFiltroImpianti.Append(" ( Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(frm_Piva(i))) & "' ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(frm_SaCod(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(frm_Appezza(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(frm_IdReg(i)) & " )  ")
                strFiltroImpianti.Append(" OR ")
            Next
            If strFiltroImpianti.Length > 0 Then
                strFiltroImpianti.Remove(strFiltroImpianti.Length - 4, 4)
                Filtro = " ( " & strFiltroImpianti.ToString & " ) "
            End If
            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtImpianti As DataTable
            Dim leggiAncheBloccati As Boolean = True
            DtImpianti = objImp.Leggi_Impianti_xAgenda2(False, Piva, 0, "", 0, "", 0, 0, Filtro, "", objParametri_Server, leggiAncheBloccati)
            If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                Carica_Impianti("", DtImpianti)
            End If
        End If

    End Sub

    '########################################################################################
    Private Sub Ripristina_Dati_nei_Controlli_Concimazione(ByVal Xml_Operazione As String)

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement


        Dim xDatiRicettaxNote As XmlElement
        Dim xListaRicettaxNote As XmlNodeList
        Dim xRicettaxNote As XmlElement

        Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_RicettaDestinazione As System.Xml.XmlElement
        Dim XMLs_RicettaDestinazione As System.Xml.XmlNodeList

        Dim frm_Piva() As String
        Dim frm_SaCod() As Integer
        Dim frm_Appezza() As Integer
        Dim frm_IdReg() As Integer

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim i As Integer
        Dim j As Integer

        Dim frm_LavCod As Integer
        Dim frm_NoteIntervento As String
        Dim frm_Mezzo As Integer
        Dim frm_Epoca As Integer = 0

        Dim frm_Acqua As Decimal = 0

        Dim frm_FerCod() As Integer
        Dim frm_FerDes() As String
        Dim frm_UdmCod() As Integer
        Dim frm_UdmSim() As String
        Dim frm_UdmCodTrasformato() As Integer
        Dim frm_Dose() As Decimal

        Dim N() As String
        Dim P() As String
        Dim K() As String
        Dim Mg() As String
        Dim Efficienza() As String

        Dim Cau_Mov As String

        Dim Nota_Cod As Integer

        Dim Udm_Cod_Costi As Integer = -1
        Dim Udm_Des_Costi As String = "Indefinito"
        Dim Costo_Unitario As String = "0"
        Dim Costo As String = "0"
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""

        Dim Centro As String
        Dim Tipo_Centro As String
        Dim Centro_Cod As Integer

        Dim Piva As String = ""
        If ViewState("Piva") IsNot Nothing Then
            Piva = ViewState("Piva")
        End If

        Costruisci_DT_Scarico()

        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

        CaricaListControl.Note_Intervento(CBL_Consigli_Concimazione,
                                                        False, "", "",
                                                         0, -1,
                                                         "", "",
                                                         objParametri_Server)

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Operazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_LavCod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.GetAttribute("extra_int") <> "0" Then
            frm_Epoca = CInt(XML_Ricetta_Operazione.GetAttribute("extra_int"))
        End If




        frm_Mezzo = CInt(XML_Ricetta_Operazione.GetAttribute("mezzo"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            ' ----- NOTE

            xDatiRicettaxNote = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")

            If xDatiRicettaxNote IsNot Nothing Then
                xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")
                If xListaRicettaxNote IsNot Nothing Then
                    For i = 0 To xListaRicettaxNote.Count - 1
                        xRicettaxNote = xListaRicettaxNote.Item(i)
                        Nota_Cod = CInt(xRicettaxNote.GetAttribute("nota_cod"))
                        For j = 0 To CBL_Consigli_Concimazione.Items.Count - 1
                            If Nota_Cod = CInt(CBL_Consigli_Concimazione.Items(j).Value) Then
                                CBL_Consigli_Concimazione.Items(j).Selected = True
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If



            '----- Tag DatiRicetta_Dettagli_Tecnici

            XML_DatiRicetta_Dettagli_Tecnici = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli_Tecnici")

            If XML_DatiRicetta_Dettagli_Tecnici IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio_Tecnico  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio_Tecnico = XML_DatiRicetta_Dettagli_Tecnici.GetElementsByTagName("Ricetta_Dettaglio_Tecnico")

                If XMLs_Ricetta_Dettaglio_Tecnico IsNot Nothing Then

                    'Ciclo su tutti i nodi
                    For i = 0 To XMLs_Ricetta_Dettaglio_Tecnico.Count - 1

                        'Prendo l'i-esimo nodo della collezione
                        XML_Ricetta_Dettaglio_Tecnico = XMLs_Ricetta_Dettaglio_Tecnico.Item(i)

                        'prendo il dettaglio tecnico dell'acqua (escludo quelli degli apporti)
                        If CDbl(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("qta_ril")) <> 0 Then
                            frm_Acqua = CDbl(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("qta_ril"))
                        End If
                    Next
                End If

            End If

            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If XML_DatiRicetta_Dettagli IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case Cau_Mov

                        Case "", CAU_LAVORAZIONE

                            If i = 0 Then

                                XMLs_RicettaDestinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                                If XMLs_RicettaDestinazione IsNot Nothing AndAlso XMLs_RicettaDestinazione.Count > 0 Then

                                    For j = 0 To XMLs_RicettaDestinazione.Count - 1

                                        XML_RicettaDestinazione = XMLs_RicettaDestinazione.Item(j)

                                        If frm_Piva Is Nothing Then
                                            ReDim Preserve frm_Piva(0)
                                            ReDim Preserve frm_SaCod(0)
                                            ReDim Preserve frm_Appezza(0)
                                            ReDim Preserve frm_IdReg(0)
                                        Else
                                            ReDim Preserve frm_Piva(UBound(frm_Piva) + 1)
                                            ReDim Preserve frm_SaCod(UBound(frm_SaCod) + 1)
                                            ReDim Preserve frm_Appezza(UBound(frm_Appezza) + 1)
                                            ReDim Preserve frm_IdReg(UBound(frm_IdReg) + 1)
                                        End If

                                        frm_Piva(UBound(frm_Piva)) = CStr(XML_RicettaDestinazione.GetAttribute("piva"))
                                        frm_SaCod(UBound(frm_SaCod)) = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                                        frm_Appezza(UBound(frm_Appezza)) = CInt(XML_RicettaDestinazione.GetAttribute("appezza"))
                                        frm_IdReg(UBound(frm_IdReg)) = CInt(XML_RicettaDestinazione.GetAttribute("id_reg"))

                                    Next

                                End If

                            End If

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If XMLs_Ricetta_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(j)

                                    If N Is Nothing Then
                                        ReDim Preserve N(0)
                                        ReDim Preserve P(0)
                                        ReDim Preserve K(0)
                                        ReDim Preserve Mg(0)
                                        ReDim Preserve Efficienza(0)
                                    Else
                                        ReDim Preserve N(UBound(N) + 1)
                                        ReDim Preserve P(UBound(P) + 1)
                                        ReDim Preserve K(UBound(K) + 1)
                                        ReDim Preserve Mg(UBound(Mg) + 1)
                                        ReDim Preserve Efficienza(UBound(Efficienza) + 1)
                                    End If

                                    N(UBound(N)) = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("n"))
                                    P(UBound(P)) = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("p"))
                                    K(UBound(K)) = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("k"))
                                    Mg(UBound(Mg)) = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("mg"))
                                    Efficienza(UBound(Efficienza)) = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("efficienza"))

                                Next

                            End If

                            'Inizializzo le variabili
                            If frm_FerCod Is Nothing Then

                                ReDim Preserve frm_FerCod(0)
                                ReDim Preserve frm_FerDes(0)
                                ReDim Preserve frm_UdmCod(0)
                                ReDim Preserve frm_UdmSim(0)
                                ReDim Preserve frm_Dose(0)
                                ReDim Preserve frm_UdmCodTrasformato(0)

                            Else

                                ReDim Preserve frm_FerCod(UBound(frm_FerCod) + 1)
                                ReDim Preserve frm_FerDes(UBound(frm_FerDes) + 1)
                                ReDim Preserve frm_UdmCod(UBound(frm_UdmCod) + 1)
                                ReDim Preserve frm_UdmSim(UBound(frm_UdmSim) + 1)
                                ReDim Preserve frm_Dose(UBound(frm_Dose) + 1)
                                ReDim Preserve frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato) + 1)

                            End If

                            'Recupero i valori
                            frm_FerCod(UBound(frm_FerCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))
                            frm_FerDes(UBound(frm_FerDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("fer_des"))
                            frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("udm_cod"))
                            frm_UdmCod(UBound(frm_UdmCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("extra_int"))

                            'sbagliato, in "udm_sim" dell'xml non c'è la desciz di extra_int ma di udm_cod, che è kg, l sempre
                            'frm_UdmSim(UBound(frm_UdmSim)) = XML_Ricetta_Dettaglio.GetAttribute("udm_sim")
                            Dim udmsim As String = ""
                            Dim udmdes As String = New AgronicaCoreMetaSchemaDAL.UnitaMisura_R().UdmDes_from_UdmCod(frm_UdmCod(UBound(frm_UdmCod)), udmsim, objParametri_Server)
                            frm_UdmSim(UBound(frm_UdmSim)) = udmsim

                            frm_Dose(UBound(frm_Dose)) = CDbl(XML_Ricetta_Dettaglio.GetAttribute("qta"))

                        Case CAU_IMPUTAZIONE_PARCOMACCHINE

                            Categoria_Des = ""
                            Risorsa_Des = ""
                            Centro = "Parco Macchine"
                            Tipo_Centro = "PM"
                            Centro_Cod = -1

                            Dim DTMacchina As DataTable

                            Dim MacCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                            DTMacchina = objContab.MacchinaDes(Piva,
                                                       objParametriAgenda.Data,
                                                       MacCod,
                                                        "", "",
                                                       objParametri_Server)
                            objContab = Nothing


                            If DTMacchina.Rows.Count > 0 Then

                                Categoria_Des = DTMacchina.Rows(0).Item("Class_Desc")
                                If DTMacchina.Rows(0).Item("Mac_Des") <> "" Then
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Mac_Des")
                                Else
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Ditta_Des") & " " & DTMacchina.Rows(0).Item("Modello")
                                End If

                                InserisciRiga_dtScarico(Centro,
                                                        Centro_Cod,
                                                        Categoria_Des,
                                                        Risorsa_Des,
                                                        Udm_Des_Costi,
                                                        0,
                                                        Udm_Cod_Costi,
                                                        MACCHINE,
                                                        0,
                                                        Tipo_Centro,
                                                        0,
                                                        0,
                                                        MacCod,
                                                        Costo_Unitario,
                                                        Costo,
                                                        dtScarico)
                            End If


                        Case CAU_IMPUTAZIONE_MANODOPERA,
                             CAU_IMPUTAZIONE_TERZISTI,
                             CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            Dim Dt_Manodopera As DataTable
                            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                            Dim MatCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim Str As String
                            Str = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 )  "
                            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(Piva,
                                                                                                     MatCod,
                                                                                                     False,
                                                                                                     False,
                                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                      Str, "", objParametri_Server)




                            If Dt_Manodopera.Rows.Count = 0 Then
                                Exit For
                            End If

                            Dim Cod_Rapporto As String = ""
                            Dim Elem_Cod As Integer
                            Tipo_Centro = ""
                            Centro = ""
                            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

                            Select Case Cod_Rapporto
                                Case -1, -4, -6
                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                Case -5
                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                Case -12
                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                            End Select

                            If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
                                Centro_Cod = -3
                                Centro = "C/Terzisti"
                                Tipo_Centro = "CT"
                            End If

                            If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"
                                Centro_Cod = -2
                            End If

                            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                            Elem_Cod = 0

                            Dim Mat_Cod, Pro_Cod As Integer
                            Pro_Cod = 0
                            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")


                            InserisciRiga_dtScarico(Centro,
                                                            Centro_Cod,
                                                            Categoria_Des,
                                                            Risorsa_Des,
                                                            Udm_Des_Costi,
                                                            0,
                                                            Udm_Cod_Costi,
                                                            Elem_Cod,
                                                            0,
                                                            Tipo_Centro,
                                                            Pro_Cod,
                                                            0,
                                                            Mat_Cod,
                                                            Costo_Unitario,
                                                            Costo,
                                                            dtScarico)


                    End Select

                Next

            End If

        End If

        Session("dtScarico") = dtScarico
        AggiornaGridViewCostiAccessoriVisibili()

        '------------------------------------------
        '----- EPOCHE
        '------------------------------------------

        ComboEpoche.Veg_Cod = CInt(Cmb_Specie.SelectedValue)

        ComboEpoche.PrimaRiga_Flag = True
        ComboEpoche.PrimaRiga_Text = ""
        ComboEpoche.PrimaRiga_Value = "0"

        ComboEpoche.CaricaComboEpocheFertilizzazione()

        ComboEpoche.Visible = True
        cella_epoca.Visible = True
        Cmb_Epoca.Visible = False

        ComboEpoche.Valore_Combo = frm_Epoca

        '------------------------------------------
        '----- DataGrid     DOSI
        '------------------------------------------

        Dim Dt As DataTable


        'Creo la struttura della griglia
        CaricaGriglia_Dosi_Concimazione()

        'Recupero il datatable
        Dt = ViewState("dtDosi_Concimazione")

        For i = 0 To UBound(frm_FerCod)

            Dosi_Inserisci_Concimazione(frm_FerCod(i),
                                  frm_FerDes(i),
                                  Efficienza(i),
                                  N(i),
                                  N(i),
                                  P(i),
                                  K(i),
                                  Mg(i),
                                  frm_Dose(i),
                                  Math.Round(frm_Dose(i)).ToString,
                                  frm_UdmCod(i),
                                  frm_UdmSim(i),
                                  frm_Mezzo,
                                  "0")
        Next


        Select Case frm_LavCod
            Case LAVCOD_DISTRIBUZIONE_AMMENDANTI
                Txt_N.Enabled = True
                Txt_P2O5.Enabled = True
                Txt_K2O.Enabled = True
                Txt_MgO.Enabled = True
                Txt_Efficienza.Enabled = True
            Case Else
                Txt_N.Enabled = False
                Txt_P2O5.Enabled = False
                Txt_K2O.Enabled = False
                Txt_MgO.Enabled = False
                Txt_Efficienza.Enabled = False
        End Select

        Rbl_Dosi_Fertilizzanti.Items.Clear()
        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI
                Cella_Acqua.Visible = False
                Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Dose/Ha", "1"))
                'Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Qta Totale", "0"))
            Case Else
                Cella_Acqua.Visible = True
                If frm_Acqua < 0 Then
                    Me.RBL_Acqua_Concimazione.SelectedValue = "1"
                Else
                    Me.RBL_Acqua_Concimazione.SelectedValue = "0"
                End If
                Me.Txt_Acqua_Concimazione.Text = Math.Abs(frm_Acqua)
                Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Dose/Ha", "1"))
                Rbl_Dosi_Fertilizzanti.Items.Add(New ListItem("Dose/Hl", "0"))
        End Select

        Me.Rbl_Dosi_Fertilizzanti.SelectedValue = frm_Mezzo
        Me.Rbl_Dosi_Fertilizzanti.Enabled = False

        '------------------------------------------
        '----- Textbox     Note
        '------------------------------------------

        Me.Txt_Note_Concimazione.Text = frm_NoteIntervento

        '------------------------------------------
        '----- IMPIANTI
        '------------------------------------------

        If frm_Piva IsNot Nothing AndAlso frm_Piva.Length > 0 Then
            Dim strFiltroImpianti As New System.Text.StringBuilder
            Dim Filtro As String = ""
            'Dim Piva As String = ""
            strFiltroImpianti.Length = 0
            For i = 0 To frm_Piva.Length - 1
                If i = 0 Then
                    Piva = frm_Piva(i)
                End If
                strFiltroImpianti.Append(" ( Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(frm_Piva(i))) & "' ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(frm_SaCod(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(frm_Appezza(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(frm_IdReg(i)) & " )  ")
                strFiltroImpianti.Append(" OR ")
            Next
            If strFiltroImpianti.Length > 0 Then
                strFiltroImpianti.Remove(strFiltroImpianti.Length - 4, 4)
                Filtro = " ( " & strFiltroImpianti.ToString & " ) "
            End If
            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtImpianti As DataTable
            Dim leggiAncheBloccati As Boolean = True
            DtImpianti = objImp.Leggi_Impianti_xAgenda2(False, Piva, 0, "", 0, "", 0, 0, Filtro, "", objParametri_Server, leggiAncheBloccati)
            If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                Carica_Impianti("", DtImpianti)
            End If
        End If

    End Sub

    '########################################################################################
    Private Sub Ripristina_Dati_nei_Controlli_Lavorazione(ByVal Xml_Operazione As String)

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement

        Dim xDatiRicettaxNote As XmlElement
        Dim xListaRicettaxNote As XmlNodeList
        Dim xRicettaxNote As XmlElement

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_RicettaDestinazione As System.Xml.XmlElement
        Dim XMLs_RicettaDestinazione As System.Xml.XmlNodeList

        Dim frm_Piva() As String
        Dim frm_SaCod() As Integer
        Dim frm_Appezza() As Integer
        Dim frm_IdReg() As Integer

        Dim i As Integer
        Dim j As Integer

        Dim frm_LavCod As Integer
        Dim frm_NoteIntervento As String

        Dim Cau_Mov As String

        Dim Nota_Cod As Integer

        Dim Udm_Cod_Costi As Integer = -1
        Dim Udm_Des_Costi As String = "Indefinito"
        Dim Costo_Unitario As String = "0"
        Dim Costo As String = "0"
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""

        Dim Centro As String
        Dim Tipo_Centro As String
        Dim Centro_Cod As Integer

        Dim Piva As String = ""
        If ViewState("Piva") IsNot Nothing Then
            Piva = ViewState("Piva")
        End If

        Costruisci_DT_Scarico()

        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

        CaricaListControl.Note_Intervento(CBL_Consigli_Lavorazioni,
                                          False, "", "",
                                          0, -1,
                                          "", "",
                                          objParametri_Server)

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Operazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_LavCod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            ' ----- NOTE

            xDatiRicettaxNote = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")

            If xDatiRicettaxNote IsNot Nothing Then
                xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")
                If xListaRicettaxNote IsNot Nothing Then
                    For i = 0 To xListaRicettaxNote.Count - 1
                        xRicettaxNote = xListaRicettaxNote.Item(i)
                        Nota_Cod = CInt(xRicettaxNote.GetAttribute("nota_cod"))
                        For j = 0 To CBL_Consigli_Lavorazioni.Items.Count - 1
                            If Nota_Cod = CInt(CBL_Consigli_Lavorazioni.Items(j).Value) Then
                                CBL_Consigli_Lavorazioni.Items(j).Selected = True
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If

            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If  XML_DatiRicetta_Dettagli IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case Cau_Mov

                        Case "", CAU_LAVORAZIONE

                            XMLs_RicettaDestinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If XMLs_RicettaDestinazione IsNot Nothing AndAlso XMLs_RicettaDestinazione.Count > 0 Then

                                For j = 0 To XMLs_RicettaDestinazione.Count - 1

                                    XML_RicettaDestinazione = XMLs_RicettaDestinazione.Item(j)

                                    If frm_Piva Is Nothing Then
                                        ReDim Preserve frm_Piva(0)
                                        ReDim Preserve frm_SaCod(0)
                                        ReDim Preserve frm_Appezza(0)
                                        ReDim Preserve frm_IdReg(0)
                                    Else
                                        ReDim Preserve frm_Piva(UBound(frm_Piva) + 1)
                                        ReDim Preserve frm_SaCod(UBound(frm_SaCod) + 1)
                                        ReDim Preserve frm_Appezza(UBound(frm_Appezza) + 1)
                                        ReDim Preserve frm_IdReg(UBound(frm_IdReg) + 1)
                                    End If

                                    frm_Piva(UBound(frm_Piva)) = CStr(XML_RicettaDestinazione.GetAttribute("piva"))
                                    frm_SaCod(UBound(frm_SaCod)) = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                                    frm_Appezza(UBound(frm_Appezza)) = CInt(XML_RicettaDestinazione.GetAttribute("appezza"))
                                    frm_IdReg(UBound(frm_IdReg)) = CInt(XML_RicettaDestinazione.GetAttribute("id_reg"))

                                Next

                            End If


                        Case CAU_IMPUTAZIONE_PARCOMACCHINE

                            Categoria_Des = ""
                            Risorsa_Des = ""
                            Centro = "Parco Macchine"
                            Tipo_Centro = "PM"
                            Centro_Cod = -1

                            Dim DTMacchina As DataTable

                            Dim MacCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                            DTMacchina = objContab.MacchinaDes(Piva,
                                                       objParametriAgenda.Data,
                                                       MacCod,
                                                        "", "",
                                                       objParametri_Server)
                            objContab = Nothing


                            If DTMacchina.Rows.Count > 0 Then

                                Categoria_Des = DTMacchina.Rows(0).Item("Class_Desc")
                                If DTMacchina.Rows(0).Item("Mac_Des") <> "" Then
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Mac_Des")
                                Else
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Ditta_Des") & " " & DTMacchina.Rows(0).Item("Modello")
                                End If

                                InserisciRiga_dtScarico(Centro,
                                                        Centro_Cod,
                                                        Categoria_Des,
                                                        Risorsa_Des,
                                                        Udm_Des_Costi,
                                                        0,
                                                        Udm_Cod_Costi,
                                                        MACCHINE,
                                                        0,
                                                        Tipo_Centro,
                                                        0,
                                                        0,
                                                        MacCod,
                                                        Costo_Unitario,
                                                        Costo,
                                                        dtScarico)
                            End If


                        Case CAU_IMPUTAZIONE_MANODOPERA,
                             CAU_IMPUTAZIONE_TERZISTI,
                             CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            Dim Dt_Manodopera As DataTable
                            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                            Dim MatCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim Str As String
                            Str = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 ) "
                            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(Piva,
                                                                                                     MatCod,
                                                                                                     False,
                                                                                                     False,
                                                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                      Str, "", objParametri_Server)




                            If Dt_Manodopera.Rows.Count = 0 Then
                                Exit For
                            End If

                            Dim Cod_Rapporto As String = ""
                            Dim Elem_Cod As Integer
                            Tipo_Centro = ""
                            Centro = ""
                            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

                            Select Case Cod_Rapporto
                                Case -1, -4, -6
                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                Case -5
                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                Case -12
                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                            End Select

                            If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
                                Centro_Cod = -3
                                Centro = "C/Terzisti"
                                Tipo_Centro = "CT"
                            End If

                            If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"
                                Centro_Cod = -2
                            End If

                            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                            Elem_Cod = 0

                            Dim Mat_Cod, Pro_Cod As Integer
                            Pro_Cod = 0
                            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")


                            InserisciRiga_dtScarico(Centro,
                                                            Centro_Cod,
                                                            Categoria_Des,
                                                            Risorsa_Des,
                                                            Udm_Des_Costi,
                                                            0,
                                                            Udm_Cod_Costi,
                                                            Elem_Cod,
                                                            0,
                                                            Tipo_Centro,
                                                            Pro_Cod,
                                                            0,
                                                            Mat_Cod,
                                                            Costo_Unitario,
                                                            Costo,
                                                            dtScarico)


                    End Select

                Next

            End If

        End If

        Session("dtScarico") = dtScarico
        AggiornaGridViewCostiAccessoriVisibili()

        '------------------------------------------
        '----- Textbox     Note
        '------------------------------------------

        Me.Txt_Note_Lavorazioni.Text = frm_NoteIntervento

        '------------------------------------------
        '----- IMPIANTI
        '------------------------------------------

        If frm_Piva IsNot Nothing AndAlso frm_Piva.Length > 0 Then
            Dim strFiltroImpianti As New System.Text.StringBuilder
            Dim Filtro As String = ""
            'Dim Piva As String = ""
            strFiltroImpianti.Length = 0
            For i = 0 To frm_Piva.Length - 1
                If i = 0 Then
                    Piva = frm_Piva(i)
                End If
                strFiltroImpianti.Append(" ( Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(frm_Piva(i))) & "' ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(frm_SaCod(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(frm_Appezza(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(frm_IdReg(i)) & " )  ")
                strFiltroImpianti.Append(" OR ")
            Next
            If strFiltroImpianti.Length > 0 Then
                strFiltroImpianti.Remove(strFiltroImpianti.Length - 4, 4)
                Filtro = " ( " & strFiltroImpianti.ToString & " ) "
            End If
            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtImpianti As DataTable
            Dim leggiAncheBloccati As Boolean = True
            DtImpianti = objImp.Leggi_Impianti_xAgenda2(False, Piva, 0, "", 0, "", 0, 0, Filtro, "", objParametri_Server, leggiAncheBloccati)
            If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                Carica_Impianti("", DtImpianti)
            End If
        End If




    End Sub

    '########################################################################################
    Private Sub Ripristina_Dati_nei_Controlli_Irrigazione(ByVal Xml_Operazione As String)

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement


        Dim xDatiRicettaxNote As XmlElement
        Dim xListaRicettaxNote As XmlNodeList
        Dim xRicettaxNote As XmlElement


        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_RicettaDestinazione As System.Xml.XmlElement
        Dim XMLs_RicettaDestinazione As System.Xml.XmlNodeList

        Dim frm_Piva() As String
        Dim frm_SaCod() As Integer
        Dim frm_Appezza() As Integer
        Dim frm_IdReg() As Integer

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim i As Integer
        Dim j As Integer

        Dim frm_LavCod As Integer
        Dim frm_NoteIntervento As String

        Dim frm_UdmCod As Integer
        Dim frm_Dose As Decimal
        Dim frm_Freatimetro As Integer
        'Dim frm_Ore As Decimal
        'Dim frm_Portata As Decimal
        Dim frm_DataInizio As Date
        Dim frm_DataFine As Date
        Dim frm_Frequenza As Integer

        Dim Cau_Mov As String

        Dim Nota_Cod As Integer

        Dim Udm_Cod_Costi As Integer = -1
        Dim Udm_Des_Costi As String = "Indefinito"
        Dim Costo_Unitario As String = "0"
        Dim Costo As String = "0"
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""

        Dim Centro As String
        Dim Tipo_Centro As String
        Dim Centro_Cod As Integer

        Dim Piva As String = ""
        If ViewState("Piva") IsNot Nothing Then
            Piva = ViewState("Piva")
        End If

        Costruisci_DT_Scarico()

        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

        CaricaListControl.Note_Intervento(CBL_Consigli_Irrigazione,
                                                        False, "", "",
                                                         0, -1,
                                                         "", "",
                                                         objParametri_Server)

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Operazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_LavCod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            ' ----- NOTE

            xDatiRicettaxNote = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")

            If xDatiRicettaxNote IsNot Nothing Then
                xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")
                If xListaRicettaxNote IsNot Nothing Then
                    For i = 0 To xListaRicettaxNote.Count - 1
                        xRicettaxNote = xListaRicettaxNote.Item(i)
                        Nota_Cod = CInt(xRicettaxNote.GetAttribute("nota_cod"))
                        For j = 0 To CBL_Consigli_Irrigazione.Items.Count - 1
                            If Nota_Cod = CInt(CBL_Consigli_Irrigazione.Items(j).Value) Then
                                CBL_Consigli_Irrigazione.Items(j).Selected = True
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If


            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If XML_DatiRicetta_Dettagli IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case Cau_Mov

                        Case "", CAU_LAVORAZIONE

                            XMLs_RicettaDestinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If XMLs_RicettaDestinazione IsNot Nothing AndAlso XMLs_RicettaDestinazione.Count > 0 Then

                                For j = 0 To XMLs_RicettaDestinazione.Count - 1

                                    XML_RicettaDestinazione = XMLs_RicettaDestinazione.Item(j)

                                    If frm_Piva Is Nothing Then
                                        ReDim Preserve frm_Piva(0)
                                        ReDim Preserve frm_SaCod(0)
                                        ReDim Preserve frm_Appezza(0)
                                        ReDim Preserve frm_IdReg(0)
                                    Else
                                        ReDim Preserve frm_Piva(UBound(frm_Piva) + 1)
                                        ReDim Preserve frm_SaCod(UBound(frm_SaCod) + 1)
                                        ReDim Preserve frm_Appezza(UBound(frm_Appezza) + 1)
                                        ReDim Preserve frm_IdReg(UBound(frm_IdReg) + 1)
                                    End If

                                    frm_Piva(UBound(frm_Piva)) = CStr(XML_RicettaDestinazione.GetAttribute("piva"))
                                    frm_SaCod(UBound(frm_SaCod)) = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                                    frm_Appezza(UBound(frm_Appezza)) = CInt(XML_RicettaDestinazione.GetAttribute("appezza"))
                                    frm_IdReg(UBound(frm_IdReg)) = CInt(XML_RicettaDestinazione.GetAttribute("id_reg"))

                                Next

                            End If

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If XMLs_Ricetta_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(j)

                                    'Recupero i valori
                                    frm_Dose = CDbl(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("qta_ril"))
                                    'frm_Ore = CDbl(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("dose"))
                                    frm_Freatimetro = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("freatimetro"))
                                    'frm_Portata = CDbl(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("parziale"))
                                    frm_DataInizio = CDate(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("inn1_data"))
                                    frm_DataFine = CDate(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("inn2_data"))
                                    frm_Frequenza = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("nitrati"))
                                    frm_UdmCod = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("dett_cod"))

                                Next

                            End If


                        Case CAU_IMPUTAZIONE_PARCOMACCHINE

                            Categoria_Des = ""
                            Risorsa_Des = ""
                            Centro = "Parco Macchine"
                            Tipo_Centro = "PM"
                            Centro_Cod = -1

                            Dim DTMacchina As DataTable

                            Dim MacCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                            DTMacchina = objContab.MacchinaDes(Piva,
                                                       objParametriAgenda.Data,
                                                       MacCod,
                                                        "", "",
                                                       objParametri_Server)
                            objContab = Nothing


                            If DTMacchina.Rows.Count > 0 Then

                                Categoria_Des = DTMacchina.Rows(0).Item("Class_Desc")
                                If DTMacchina.Rows(0).Item("Mac_Des") <> "" Then
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Mac_Des")
                                Else
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Ditta_Des") & " " & DTMacchina.Rows(0).Item("Modello")
                                End If

                                InserisciRiga_dtScarico(Centro,
                                                        Centro_Cod,
                                                        Categoria_Des,
                                                        Risorsa_Des,
                                                        Udm_Des_Costi,
                                                        0,
                                                        Udm_Cod_Costi,
                                                        MACCHINE,
                                                        0,
                                                        Tipo_Centro,
                                                        0,
                                                        0,
                                                        MacCod,
                                                        Costo_Unitario,
                                                        Costo,
                                                        dtScarico)
                            End If


                        Case CAU_IMPUTAZIONE_MANODOPERA,
                             CAU_IMPUTAZIONE_TERZISTI,
                             CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            Dim Dt_Manodopera As DataTable
                            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                            Dim MatCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim Str As String
                            Str = "  ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 ) "
                            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(Piva,
                                                                                                     MatCod,
                                                                                                     False,
                                                                                                     False,
                                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                      Str, "", objParametri_Server)




                            If Dt_Manodopera.Rows.Count = 0 Then
                                Exit For
                            End If

                            Dim Cod_Rapporto As String = ""
                            Dim Elem_Cod As Integer
                            Tipo_Centro = ""
                            Centro = ""
                            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

                            Select Case Cod_Rapporto
                                Case -1, -4, -6
                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                Case -5
                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                Case -12
                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                            End Select

                            If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
                                Centro_Cod = -3
                                Centro = "C/Terzisti"
                                Tipo_Centro = "CT"
                            End If

                            If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"
                                Centro_Cod = -2
                            End If

                            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                            Elem_Cod = 0

                            Dim Mat_Cod, Pro_Cod As Integer
                            Pro_Cod = 0
                            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")


                            InserisciRiga_dtScarico(Centro,
                                                            Centro_Cod,
                                                            Categoria_Des,
                                                            Risorsa_Des,
                                                            Udm_Des_Costi,
                                                            0,
                                                            Udm_Cod_Costi,
                                                            Elem_Cod,
                                                            0,
                                                            Tipo_Centro,
                                                            Pro_Cod,
                                                            0,
                                                            Mat_Cod,
                                                            Costo_Unitario,
                                                            Costo,
                                                            dtScarico)


                    End Select

                Next

            End If

        End If

        Session("dtScarico") = dtScarico
        AggiornaGridViewCostiAccessoriVisibili()

        Cmb_Udm_Irrigazione.SelectedIndex =
           Cmb_Udm_Irrigazione.Items.IndexOf(Cmb_Udm_Irrigazione.Items.FindByValue(
               frm_UdmCod))

        'If frm_Ore <> 0 Then
        '    Me.Txt_Ore_Irrigazione.Text = frm_Ore.ToString
        'End If
        'If frm_Portata <> 0 Then
        '    Me.Txt_Portata_Irrigazione.Text = frm_Portata.ToString
        'End If

        If frm_Freatimetro <> -1 Then
            Me.Cmb_ModifTipoIrrig.SelectedIndex =
                        Cmb_ModifTipoIrrig.Items.IndexOf(
                            Cmb_ModifTipoIrrig.Items.FindByValue(frm_Freatimetro))
        End If


        If frm_Dose <> 0 Then
            Me.Txt_Dose_Irrigazione.Text = frm_Dose.ToString
        End If
        If frm_DataInizio <> AGRODATAINIZIO Then
            Me.Txt_DataInizio_Irrigazione.Text = frm_DataInizio.ToShortDateString
        End If
        If frm_DataFine <> AGRODATAFINE Then
            Me.Txt_DataFine_Irrigazione.Text = frm_DataFine.ToShortDateString
        End If
        If frm_Frequenza <> 0 Then
            Me.Txt_Frequenza_Irrigazione.Text = frm_Frequenza.ToString
        End If

        Me.Txt_Note_Irrigazione.Text = frm_NoteIntervento


        '------------------------------------------
        '----- IMPIANTI
        '------------------------------------------

        If frm_Piva IsNot Nothing AndAlso frm_Piva.Length > 0 Then
            Dim strFiltroImpianti As New System.Text.StringBuilder
            Dim Filtro As String = ""
            'Dim Piva As String = ""
            strFiltroImpianti.Length = 0
            For i = 0 To frm_Piva.Length - 1
                If i = 0 Then
                    Piva = frm_Piva(i)
                End If
                strFiltroImpianti.Append(" ( Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(frm_Piva(i))) & "' ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(frm_SaCod(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(frm_Appezza(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(frm_IdReg(i)) & " )  ")
                strFiltroImpianti.Append(" OR ")
            Next
            If strFiltroImpianti.Length > 0 Then
                strFiltroImpianti.Remove(strFiltroImpianti.Length - 4, 4)
                Filtro = " ( " & strFiltroImpianti.ToString & " ) "
            End If
            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtImpianti As DataTable
            Dim leggiAncheBloccati As Boolean = True
            DtImpianti = objImp.Leggi_Impianti_xAgenda2(False, Piva, 0, "", 0, "", 0, 0, Filtro, "", objParametri_Server, leggiAncheBloccati)
            If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                Carica_Impianti("", DtImpianti)
            End If
        End If


    End Sub

    Private Sub CaricaFasiFenologiche()
        ddlFF1.CaricaComboFasiFenologiche(objParametriAgenda.Veg_Cod)
        ddlFF2.CaricaComboFasiFenologiche(objParametriAgenda.Veg_Cod)
    End Sub

    Private Sub Ripristina_Dati_nei_Controlli_RilieviAvvAus(ByVal Xml_Operazione As String)

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement


        Dim xDatiRicettaxNote As XmlElement
        Dim xListaRicettaxNote As XmlNodeList
        Dim xRicettaxNote As XmlElement


        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_RicettaDestinazione As System.Xml.XmlElement
        Dim XMLs_RicettaDestinazione As System.Xml.XmlNodeList

        Dim frm_Piva() As String
        Dim frm_SaCod() As Integer
        Dim frm_Appezza() As Integer
        Dim frm_IdReg() As Integer

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim i As Integer
        Dim j As Integer

        Dim frm_LavCod As Integer
        Dim frm_NoteIntervento As String

        Dim frm_TrapCod As Integer
        Dim frm_NumTrappole As Integer
        Dim frm_AvCod As Integer
        Dim frm_AvSigla As String
        Dim frm_DittaTrappola As Integer
        Dim frm_UsoTrappola As Integer

        Dim Cau_Mov As String

        Dim Nota_Cod As Integer

        Dim Udm_Cod_Costi As Integer = -1
        Dim Udm_Des_Costi As String = "Indefinito"
        Dim Costo_Unitario As String = "0"
        Dim Costo As String = "0"
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""

        Dim Centro As String
        Dim Tipo_Centro As String
        Dim Centro_Cod As Integer

        Dim Piva As String = ""
        If ViewState("Piva") IsNot Nothing Then
            Piva = ViewState("Piva")
        End If

        Costruisci_DT_Scarico()

        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

        CaricaListControl.Note_Intervento(CBL_Consigli_Trappole,
                                                        False, "", "",
                                                         0, -1,
                                                         "", "",
                                                         objParametri_Server)




        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Operazione)


        objParametriAgenda.Impianti.Add(
            New AgronicaCoreModello.ParametriAgenda_Temp.Impianto With {
                    .App_Nome = "-"
                })

        Dim rilievoAvvList As New List(Of rilievoAvv)

        Dim sFF_cod1 As String = XmlDoc.SelectSingleNode("Ricetta_Operazione").SelectSingleNode("DatiRicetta_Dettagli").SelectSingleNode("Ricetta_Dettaglio").SelectSingleNode("Ricetta_Dettaglio_Tecnico_2").Attributes("ff_classe").Value
        Dim lFF_cod1 As Integer = 0

        If sFF_cod1 <> "" Then
            lFF_cod1 = CInt(sFF_cod1)
        End If


        Dim sFF_cod2 As String = XmlDoc.SelectSingleNode("Ricetta_Operazione").SelectSingleNode("DatiRicetta_Dettagli").SelectSingleNode("Ricetta_Dettaglio").SelectSingleNode("Ricetta_Dettaglio_Tecnico_2").Attributes("piezo2").Value
        Dim lFF_cod2 As Integer = 0

        If sFF_cod2 <> "" Then
            lFF_cod2 = sFF_cod2
        End If


        If lFF_cod1 <> 0 Then
            ddlFF1.selectedValue = lFF_cod1
        End If

        If lFF_cod2 <> 0 Then
            ddlFF2.selectedValue = lFF_cod2
        End If

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_LavCod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            ' ----- NOTE

            xDatiRicettaxNote = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")

            If xDatiRicettaxNote IsNot Nothing Then
                xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")
                If xListaRicettaxNote IsNot Nothing Then
                    For i = 0 To xListaRicettaxNote.Count - 1
                        xRicettaxNote = xListaRicettaxNote.Item(i)
                        Nota_Cod = CInt(xRicettaxNote.GetAttribute("nota_cod"))
                        For j = 0 To CBL_Consigli_Trappole.Items.Count - 1
                            If Nota_Cod = CInt(CBL_Consigli_Trappole.Items(j).Value) Then
                                CBL_Consigli_Trappole.Items(j).Selected = True
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If


            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If XML_DatiRicetta_Dettagli IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case Cau_Mov

                        Case "", CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO

                            XMLs_RicettaDestinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If XMLs_RicettaDestinazione IsNot Nothing AndAlso XMLs_RicettaDestinazione.Count > 0 Then

                                For j = 0 To XMLs_RicettaDestinazione.Count - 1

                                    XML_RicettaDestinazione = XMLs_RicettaDestinazione.Item(j)

                                    If frm_Piva Is Nothing Then
                                        ReDim Preserve frm_Piva(0)
                                        ReDim Preserve frm_SaCod(0)
                                        ReDim Preserve frm_Appezza(0)
                                        ReDim Preserve frm_IdReg(0)
                                    Else
                                        ReDim Preserve frm_Piva(UBound(frm_Piva) + 1)
                                        ReDim Preserve frm_SaCod(UBound(frm_SaCod) + 1)
                                        ReDim Preserve frm_Appezza(UBound(frm_Appezza) + 1)
                                        ReDim Preserve frm_IdReg(UBound(frm_IdReg) + 1)
                                    End If

                                    frm_Piva(UBound(frm_Piva)) = CStr(XML_RicettaDestinazione.GetAttribute("piva"))
                                    frm_SaCod(UBound(frm_SaCod)) = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                                    frm_Appezza(UBound(frm_Appezza)) = CInt(XML_RicettaDestinazione.GetAttribute("appezza"))
                                    frm_IdReg(UBound(frm_IdReg)) = CInt(XML_RicettaDestinazione.GetAttribute("id_reg"))

                                Next

                            End If

                            'codice del tipo di trappola
                            frm_TrapCod = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))

                            'numero delle trappole installate
                            frm_NumTrappole = CInt(XML_Ricetta_Dettaglio.GetAttribute("qta"))

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If XMLs_Ricetta_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(j)

                                    Dim lVal As String = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("qta_ril"))
                                    Dim dVal As Decimal = 0

                                    If lVal <> "" Then
                                        dVal = CDbl(lVal)
                                    End If

                                    rilievoAvvList.Add(
                                        New rilievoAvv With {
                                            .Av_cod = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod")),
                                            .mov_destinazioni_graphickey = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("Ricette_Dettaglio_Tecnico_graphickey")),
                                            .Valore = dVal,
                                            .Impianto = objParametriAgenda.Impianti.First,
                                            .Udm_cod = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("dett_cod"))
                                        })


                                Next

                            End If


                        Case CAU_IMPUTAZIONE_PARCOMACCHINE

                            Categoria_Des = ""
                            Risorsa_Des = ""
                            Centro = "Parco Macchine"
                            Tipo_Centro = "PM"
                            Centro_Cod = -1

                            Dim DTMacchina As DataTable

                            Dim MacCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                            DTMacchina = objContab.MacchinaDes(Piva,
                                                       objParametriAgenda.Data,
                                                       MacCod,
                                                        "", "",
                                                       objParametri_Server)
                            objContab = Nothing


                            If DTMacchina.Rows.Count > 0 Then

                                Categoria_Des = DTMacchina.Rows(0).Item("Class_Desc")
                                If DTMacchina.Rows(0).Item("Mac_Des") <> "" Then
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Mac_Des")
                                Else
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Ditta_Des") & " " & DTMacchina.Rows(0).Item("Modello")
                                End If

                                InserisciRiga_dtScarico(Centro,
                                                        Centro_Cod,
                                                        Categoria_Des,
                                                        Risorsa_Des,
                                                        Udm_Des_Costi,
                                                        0,
                                                        Udm_Cod_Costi,
                                                        MACCHINE,
                                                        0,
                                                        Tipo_Centro,
                                                        0,
                                                        0,
                                                        MacCod,
                                                        Costo_Unitario,
                                                        Costo,
                                                        dtScarico)
                            End If


                        Case CAU_IMPUTAZIONE_MANODOPERA,
                             CAU_IMPUTAZIONE_TERZISTI,
                             CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            Dim Dt_Manodopera As DataTable
                            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                            Dim MatCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim Str As String
                            Str = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 ) "
                            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(Piva,
                                                                                                     MatCod,
                                                                                                     False,
                                                                                                     False,
                                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                      Str, "", objParametri_Server)




                            If Dt_Manodopera.Rows.Count = 0 Then
                                Exit For
                            End If

                            Dim Cod_Rapporto As String = ""
                            Dim Elem_Cod As Integer
                            Tipo_Centro = ""
                            Centro = ""
                            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

                            Select Case Cod_Rapporto
                                Case -1, -4, -6
                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                Case -5
                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                Case -12
                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                            End Select

                            If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
                                Centro_Cod = -3
                                Centro = "C/Terzisti"
                                Tipo_Centro = "CT"
                            End If

                            If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"
                                Centro_Cod = -2
                            End If

                            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                            Elem_Cod = 0

                            Dim Mat_Cod, Pro_Cod As Integer
                            Pro_Cod = 0
                            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")


                            InserisciRiga_dtScarico(Centro,
                                                            Centro_Cod,
                                                            Categoria_Des,
                                                            Risorsa_Des,
                                                            Udm_Des_Costi,
                                                            0,
                                                            Udm_Cod_Costi,
                                                            Elem_Cod,
                                                            0,
                                                            Tipo_Centro,
                                                            Pro_Cod,
                                                            0,
                                                            Mat_Cod,
                                                            Costo_Unitario,
                                                            Costo,
                                                            dtScarico)


                    End Select

                Next

            End If

        End If

        objParametriAgenda.Rilievi = rilievoAvvList
        objParametriAgenda.Lav_Cod = LAVCOD_RILIEVO_AVVERSITA_CAMPO
        objParametriAgenda.Veg_Cod = Cmb_Specie.SelectedValue



        GeneraTabellaHtmlTrappole(False)

        Session("dtScarico") = dtScarico
        AggiornaGridViewCostiAccessoriVisibili()


        '------------------------------------------
        '----- Imposto i Controlli
        '------------------------------------------

        Select Case frm_LavCod
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE
                frm_UsoTrappola = enum_TrappoleUso.Monitor
            Case LAVCOD_CATTURE_MASSA
                frm_UsoTrappola = enum_TrappoleUso.CattureDiMassa
            Case LAVCOD_CONFUSIONE_SESSUALE
                frm_UsoTrappola = enum_TrappoleUso.ConfusioneSessuale
            Case LAVCOD_DISORIENTAMENTO_SESSUALE
                frm_UsoTrappola = enum_TrappoleUso.Disorientamento
        End Select

        Me.Txt_Note_Trappole.Text = frm_NoteIntervento

        Me.Txt_NumeroTrappole.Text = frm_NumTrappole

        Dim clc = New AgronicaCoreUtility.CaricaListControl

        'Carico tutte le trappole per combattere tutte le avversità della specie vegetale scelta
        CaricaListControl.TrappolexSpecieVegetalixAvversita(CType(Me.cmb_Trappola, ListControl),
                                                            True, "", "0",
                                                            CInt(Me.Cmb_Specie.SelectedItem.Value),
                                                            0,
                                                            frm_UsoTrappola,
                                                            "", "", objParametri_Server)

        'Carico tutte le avversità che sono combattute dalla trappola scelta per quella specie vegetale
        clc.AvversitaxTrappole(CType(Me.cmb_Avversita, ListControl),
                                                                True, "", "0",
                                                                CInt(Cmb_Specie.SelectedValue),
                                                                frm_TrapCod,
                                                                frm_UsoTrappola,
                                                                "", "", objParametri_Server)

        'Carico le ditte della trappola scelta
        clc.DittexTrappole(CType(Me.cmb_Ditte, ListControl),
                                                            True, "", "0",
                                                            frm_TrapCod,
                                                            "", "", objParametri_Server)


        'Imposto l'indice della combo delle trappole
        cmb_Trappola.SelectedIndex =
                            cmb_Trappola.Items.IndexOf(cmb_Trappola.Items.FindByValue(
                                    frm_TrapCod))
        'Imposto l'indice della combo delle ditte
        cmb_Ditte.SelectedIndex =
                                  cmb_Ditte.Items.IndexOf(cmb_Ditte.Items.FindByValue(
                                               frm_DittaTrappola))

        'Imposto l'indice della combo delle avversità
        cmb_Avversita.SelectedIndex =
                                       cmb_Avversita.Items.IndexOf(cmb_Avversita.Items.FindByValue(
                                              frm_AvCod))

        'Carico la sigla dell'avversità
        Me.Txt_CodAvversita.Text = frm_AvSigla


        'Carico i dati relativi alla durata della trappola
        Dim Giorni As Integer
        Dim objTrap As New AgronicaCoreMetaSchemaDAL.Trappole_R
        Giorni = objTrap.TrapDur_from_TrapCod(frm_TrapCod, objParametri_Server)
        Me.Txt_GiorniFeromone.Text = Giorni

        '------------------------------------------
        '----- IMPIANTI
        '------------------------------------------

        If frm_Piva IsNot Nothing AndAlso frm_Piva.Length > 0 Then
            Dim strFiltroImpianti As New System.Text.StringBuilder
            Dim Filtro As String = ""
            'Dim Piva As String = ""
            strFiltroImpianti.Length = 0
            For i = 0 To frm_Piva.Length - 1
                If i = 0 Then
                    Piva = frm_Piva(i)
                End If
                strFiltroImpianti.Append(" ( Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(frm_Piva(i))) & "' ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(frm_SaCod(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(frm_Appezza(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(frm_IdReg(i)) & " )  ")
                strFiltroImpianti.Append(" OR ")
            Next
            If strFiltroImpianti.Length > 0 Then
                strFiltroImpianti.Remove(strFiltroImpianti.Length - 4, 4)
                Filtro = " ( " & strFiltroImpianti.ToString & " ) "
            End If
            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtImpianti As DataTable
            Dim leggiAncheBloccati As Boolean = True
            DtImpianti = objImp.Leggi_Impianti_xAgenda2(False, Piva, 0, "", 0, "", 0, 0, Filtro, "", objParametri_Server, leggiAncheBloccati)
            If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                Carica_Impianti("", DtImpianti)
            End If
        End If



    End Sub

    '########################################################################################
    Private Sub Ripristina_Dati_nei_Controlli_Trappole(ByVal Xml_Operazione As String)

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement


        Dim xDatiRicettaxNote As XmlElement
        Dim xListaRicettaxNote As XmlNodeList
        Dim xRicettaxNote As XmlElement


        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_RicettaDestinazione As System.Xml.XmlElement
        Dim XMLs_RicettaDestinazione As System.Xml.XmlNodeList

        Dim frm_Piva() As String
        Dim frm_SaCod() As Integer
        Dim frm_Appezza() As Integer
        Dim frm_IdReg() As Integer

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim i As Integer
        Dim j As Integer

        Dim frm_LavCod As Integer
        Dim frm_NoteIntervento As String

        Dim frm_TrapCod As Integer
        Dim frm_NumTrappole As Integer
        Dim frm_AvCod As Integer
        Dim frm_AvSigla As String
        Dim frm_DittaTrappola As Integer
        Dim frm_UsoTrappola As Integer

        Dim Cau_Mov As String

        Dim Nota_Cod As Integer

        Dim Udm_Cod_Costi As Integer = -1
        Dim Udm_Des_Costi As String = "Indefinito"
        Dim Costo_Unitario As String = "0"
        Dim Costo As String = "0"
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""

        Dim Centro As String
        Dim Tipo_Centro As String
        Dim Centro_Cod As Integer

        Dim Piva As String = ""
        If ViewState("Piva") IsNot Nothing Then
            Piva = ViewState("Piva")
        End If

        Costruisci_DT_Scarico()

        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

        CaricaListControl.Note_Intervento(CBL_Consigli_Trappole,
                                                        False, "", "",
                                                         0, -1,
                                                         "", "",
                                                         objParametri_Server)

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Operazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_LavCod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            ' ----- NOTE

            xDatiRicettaxNote = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")

            If xDatiRicettaxNote IsNot Nothing Then
                xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")
                If xListaRicettaxNote IsNot Nothing Then
                    For i = 0 To xListaRicettaxNote.Count - 1
                        xRicettaxNote = xListaRicettaxNote.Item(i)
                        Nota_Cod = CInt(xRicettaxNote.GetAttribute("nota_cod"))
                        For j = 0 To CBL_Consigli_Trappole.Items.Count - 1
                            If Nota_Cod = CInt(CBL_Consigli_Trappole.Items(j).Value) Then
                                CBL_Consigli_Trappole.Items(j).Selected = True
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If


            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If XML_DatiRicetta_Dettagli IsNot Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case Cau_Mov

                        Case "", CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO

                            XMLs_RicettaDestinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If XMLs_RicettaDestinazione IsNot Nothing AndAlso XMLs_RicettaDestinazione.Count > 0 Then

                                For j = 0 To XMLs_RicettaDestinazione.Count - 1

                                    XML_RicettaDestinazione = XMLs_RicettaDestinazione.Item(j)

                                    If frm_Piva Is Nothing Then
                                        ReDim Preserve frm_Piva(0)
                                        ReDim Preserve frm_SaCod(0)
                                        ReDim Preserve frm_Appezza(0)
                                        ReDim Preserve frm_IdReg(0)
                                    Else
                                        ReDim Preserve frm_Piva(UBound(frm_Piva) + 1)
                                        ReDim Preserve frm_SaCod(UBound(frm_SaCod) + 1)
                                        ReDim Preserve frm_Appezza(UBound(frm_Appezza) + 1)
                                        ReDim Preserve frm_IdReg(UBound(frm_IdReg) + 1)
                                    End If

                                    frm_Piva(UBound(frm_Piva)) = CStr(XML_RicettaDestinazione.GetAttribute("piva"))
                                    frm_SaCod(UBound(frm_SaCod)) = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                                    frm_Appezza(UBound(frm_Appezza)) = CInt(XML_RicettaDestinazione.GetAttribute("appezza"))
                                    frm_IdReg(UBound(frm_IdReg)) = CInt(XML_RicettaDestinazione.GetAttribute("id_reg"))

                                Next

                            End If

                            'codice del tipo di trappola
                            frm_TrapCod = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))

                            'numero delle trappole installate
                            frm_NumTrappole = CInt(XML_Ricetta_Dettaglio.GetAttribute("qta"))

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If XMLs_Ricetta_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(j)

                                    frm_AvCod = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod"))
                                    frm_DittaTrappola = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("ditta_cod"))
                                    frm_AvSigla = CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("sigla_av"))

                                Next

                            End If


                        Case CAU_IMPUTAZIONE_PARCOMACCHINE

                            Categoria_Des = ""
                            Risorsa_Des = ""
                            Centro = "Parco Macchine"
                            Tipo_Centro = "PM"
                            Centro_Cod = -1

                            Dim DTMacchina As DataTable

                            Dim MacCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                            DTMacchina = objContab.MacchinaDes(Piva,
                                                       objParametriAgenda.Data,
                                                       MacCod,
                                                        "", "",
                                                       objParametri_Server)
                            objContab = Nothing


                            If DTMacchina.Rows.Count > 0 Then

                                Categoria_Des = DTMacchina.Rows(0).Item("Class_Desc")
                                If DTMacchina.Rows(0).Item("Mac_Des") <> "" Then
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Mac_Des")
                                Else
                                    Risorsa_Des = DTMacchina.Rows(0).Item("Ditta_Des") & " " & DTMacchina.Rows(0).Item("Modello")
                                End If

                                InserisciRiga_dtScarico(Centro,
                                                        Centro_Cod,
                                                        Categoria_Des,
                                                        Risorsa_Des,
                                                        Udm_Des_Costi,
                                                        0,
                                                        Udm_Cod_Costi,
                                                        MACCHINE,
                                                        0,
                                                        Tipo_Centro,
                                                        0,
                                                        0,
                                                        MacCod,
                                                        Costo_Unitario,
                                                        Costo,
                                                        dtScarico)
                            End If


                        Case CAU_IMPUTAZIONE_MANODOPERA,
                             CAU_IMPUTAZIONE_TERZISTI,
                             CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            Dim Dt_Manodopera As DataTable
                            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                            Dim MatCod As Integer = CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod"))

                            Dim Str As String
                            Str = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 ) "
                            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(Piva,
                                                                                                     MatCod,
                                                                                                     False,
                                                                                                     False,
                                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                      Str, "", objParametri_Server)




                            If Dt_Manodopera.Rows.Count = 0 Then
                                Exit For
                            End If

                            Dim Cod_Rapporto As String = ""
                            Dim Elem_Cod As Integer
                            Tipo_Centro = ""
                            Centro = ""
                            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

                            Select Case Cod_Rapporto
                                Case -1, -4, -6
                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                Case -5
                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                Case -12
                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                            End Select

                            If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
                                Centro_Cod = -3
                                Centro = "C/Terzisti"
                                Tipo_Centro = "CT"
                            End If

                            If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"
                                Centro_Cod = -2
                            End If

                            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                            Elem_Cod = 0

                            Dim Mat_Cod, Pro_Cod As Integer
                            Pro_Cod = 0
                            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")


                            InserisciRiga_dtScarico(Centro,
                                                            Centro_Cod,
                                                            Categoria_Des,
                                                            Risorsa_Des,
                                                            Udm_Des_Costi,
                                                            0,
                                                            Udm_Cod_Costi,
                                                            Elem_Cod,
                                                            0,
                                                            Tipo_Centro,
                                                            Pro_Cod,
                                                            0,
                                                            Mat_Cod,
                                                            Costo_Unitario,
                                                            Costo,
                                                            dtScarico)


                    End Select

                Next

            End If

        End If

        Session("dtScarico") = dtScarico
        AggiornaGridViewCostiAccessoriVisibili()


        '------------------------------------------
        '----- Imposto i Controlli
        '------------------------------------------

        Select Case frm_LavCod
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE
                frm_UsoTrappola = enum_TrappoleUso.Monitor
            Case LAVCOD_CATTURE_MASSA
                frm_UsoTrappola = enum_TrappoleUso.CattureDiMassa
            Case LAVCOD_CONFUSIONE_SESSUALE
                frm_UsoTrappola = enum_TrappoleUso.ConfusioneSessuale
            Case LAVCOD_DISORIENTAMENTO_SESSUALE
                frm_UsoTrappola = enum_TrappoleUso.Disorientamento
        End Select

        Me.Txt_Note_Trappole.Text = frm_NoteIntervento

        Me.Txt_NumeroTrappole.Text = frm_NumTrappole

        Dim clc = New AgronicaCoreUtility.CaricaListControl
        'Carico tutte le trappole per combattere tutte le avversità della specie vegetale scelta
        AgronicaCoreUtility.CaricaListControl.TrappolexSpecieVegetalixAvversita(CType(Me.cmb_Trappola, ListControl),
                                                            True, "", "0",
                                                            CInt(Me.Cmb_Specie.SelectedItem.Value),
                                                            0,
                                                            frm_UsoTrappola,
                                                            "", "", objParametri_Server)

        'Carico tutte le avversità che sono combattute dalla trappola scelta per quella specie vegetale
        clc.AvversitaxTrappole(CType(Me.cmb_Avversita, ListControl),
                                                                True, "", "0",
                                                                CInt(Cmb_Specie.SelectedValue),
                                                                frm_TrapCod,
                                                                frm_UsoTrappola,
                                                                "", "", objParametri_Server)

        'Carico le ditte della trappola scelta
        clc.DittexTrappole(CType(Me.cmb_Ditte, ListControl),
                                                            True, "", "0",
                                                            frm_TrapCod,
                                                            "", "", objParametri_Server)


        'Imposto l'indice della combo delle trappole
        cmb_Trappola.SelectedIndex =
                            cmb_Trappola.Items.IndexOf(cmb_Trappola.Items.FindByValue(
                                    frm_TrapCod))
        'Imposto l'indice della combo delle ditte
        cmb_Ditte.SelectedIndex =
                                  cmb_Ditte.Items.IndexOf(cmb_Ditte.Items.FindByValue(
                                               frm_DittaTrappola))

        'Imposto l'indice della combo delle avversità
        cmb_Avversita.SelectedIndex =
                                       cmb_Avversita.Items.IndexOf(cmb_Avversita.Items.FindByValue(
                                              frm_AvCod))

        'Carico la sigla dell'avversità
        Me.Txt_CodAvversita.Text = frm_AvSigla


        'Carico i dati relativi alla durata della trappola
        Dim Giorni As Integer
        Dim objTrap As New AgronicaCoreMetaSchemaDAL.Trappole_R
        Giorni = objTrap.TrapDur_from_TrapCod(frm_TrapCod, objParametri_Server)
        Me.Txt_GiorniFeromone.Text = Giorni

        '------------------------------------------
        '----- IMPIANTI
        '------------------------------------------

        If frm_Piva IsNot Nothing AndAlso frm_Piva.Length > 0 Then
            Dim strFiltroImpianti As New System.Text.StringBuilder
            Dim Filtro As String = ""
            'Dim Piva As String = ""
            strFiltroImpianti.Length = 0
            For i = 0 To frm_Piva.Length - 1
                If i = 0 Then
                    Piva = frm_Piva(i)
                End If
                strFiltroImpianti.Append(" ( Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(frm_Piva(i))) & "' ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(frm_SaCod(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(frm_Appezza(i)) & "  ")
                strFiltroImpianti.Append(" AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(frm_IdReg(i)) & " )  ")
                strFiltroImpianti.Append(" OR ")
            Next
            If strFiltroImpianti.Length > 0 Then
                strFiltroImpianti.Remove(strFiltroImpianti.Length - 4, 4)
                Filtro = " ( " & strFiltroImpianti.ToString & " ) "
            End If
            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtImpianti As DataTable
            Dim leggiAncheBloccati As Boolean = True
            DtImpianti = objImp.Leggi_Impianti_xAgenda2(False, Piva, 0, "", 0, "", 0, 0, Filtro, "", objParametri_Server, leggiAncheBloccati)
            If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                Carica_Impianti("", DtImpianti)
            End If
        End If


    End Sub

    '########################################################################################
    Private Sub GridView_Dosi_Difesa_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Dosi_Difesa.RowCommand

        'Dim Coadiuvante As Boolean = False
        Dim TipoRichiesto As Integer = 0

        Cmb_FormulatoClassificazioni.SelectedIndex = 0

        Dim IndiceRigaGriglia As Integer
        Dim Av_Cod, Av_Gru, Fr_Cod As String
        Dim Soglia_Value As String = ""
        Dim Dose As String
        Dim UdmCod As Integer
        Dim Dt As DataTable
        Dim Dr As DataRow
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE

        Dim Array_AvCod_Temp() As String
        Dim Array_AvGru_Temp() As String


        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Av_Cod = GridView_Dosi_Difesa.Rows(IndiceRigaGriglia).Cells(2).Text()
        Av_Gru = GridView_Dosi_Difesa.Rows(IndiceRigaGriglia).Cells(3).Text()
        Fr_Cod = GridView_Dosi_Difesa.Rows(IndiceRigaGriglia).Cells(7).Text()

        If GridView_Dosi_Difesa.Rows(IndiceRigaGriglia).Cells(5).Text <> "&nbsp;" Then
            Soglia_Value = GridView_Dosi_Difesa.Rows(IndiceRigaGriglia).Cells(5).Text
        End If

        Dose = GridView_Dosi_Difesa.Rows(IndiceRigaGriglia).Cells(14).Text()
        UdmCod = GridView_Dosi_Difesa.Rows(IndiceRigaGriglia).Cells(13).Text()

        'Coadiuvante
        If Av_Gru = "-1" AndAlso Av_Cod = "-1" Then
            TipoRichiesto = enum_TipoFormulato.Coadiuvanti
            Cmb_FormulatoClassificazioni.SelectedIndex =
                Cmb_FormulatoClassificazioni.Items.IndexOf(Cmb_FormulatoClassificazioni.Items.FindByValue(TipoRichiesto.ToString))
        End If
        'Corroborante/Fisiofarmaco
        If Av_Gru = "-2" AndAlso Av_Cod = "-2" Then
            TipoRichiesto = enum_TipoFormulato.Corroboranti_Fisiofarmaci
            Cmb_FormulatoClassificazioni.SelectedIndex =
                Cmb_FormulatoClassificazioni.Items.IndexOf(Cmb_FormulatoClassificazioni.Items.FindByValue(TipoRichiesto.ToString))
        End If

        Select Case e.CommandName

            Case "Modifica"

                '--------------------------------------------------------
                'controllo se ho selezionato almeno una Avversità o un Gruppo

                Array_AvCod_Temp = Split(Av_Cod, ",")
                Array_AvGru_Temp = Split(Av_Gru, ",")
                Dim Array_Soglia_Value(0) As String
                Dim i_Soglie As Integer
                If Soglia_Value <> "" Then
                    Dim Array_Soglie_Value_Temp() As String
                    Array_Soglie_Value_Temp = Split(Soglia_Value, ",")
                    For i = 0 To UBound(Array_Soglie_Value_Temp)
                        ReDim Preserve Array_Soglia_Value(i)
                        Array_Soglia_Value(i) = Array_Soglie_Value_Temp(i)
                    Next
                End If

                For i = 0 To UBound(Array_AvCod_Temp)
                    For j = 0 To GridViewAvversita.Rows.Count - 1
                        If Array_AvCod_Temp(i) = GridViewAvversita.Rows(j).Cells(1).Text Then
                            CType(GridViewAvversita.Rows(j).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True
                            If Soglia_Value <> "" Then
                                'identifico se ci sono delle giustificazioni
                                If Array_Soglia_Value.Length > 0 Then
                                    If Array_Soglia_Value(i_Soglie).Split("_")(0) = Array_AvCod_Temp(i) Then
                                        'sono nella avversita corretta
                                        Dim jjj As Integer
                                        For jjj = 0 To CType(GridViewAvversita.Rows(j).FindControl("rbl_soglie_intervento"), RadioButtonList).Items.Count - 1
                                            If CType(GridViewAvversita.Rows(j).FindControl("rbl_soglie_intervento"), RadioButtonList).Items(jjj).Value = Array_Soglia_Value(i_Soglie) Then
                                                CType(GridViewAvversita.Rows(j).FindControl("rbl_soglie_intervento"), RadioButtonList).Items(jjj).Selected = True
                                                i_Soglie = i_Soglie + 1
                                                If Array_Soglia_Value.Length = i_Soglie Then
                                                    Exit For
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                            Exit For
                        End If
                    Next
                Next

                For i = 0 To UBound(Array_AvGru_Temp)
                    For j = 0 To GridViewGruppiAvversita.Rows.Count - 1
                        If Array_AvGru_Temp(i) = GridViewGruppiAvversita.Rows(j).Cells(1).Text Then
                            CType(GridViewGruppiAvversita.Rows(j).FindControl("ChkSelezionaGruppoAvversita"), CheckBox).Checked = True
                            Exit For
                        End If
                    Next
                Next

                '--------------------------------------------------------------------------

                'Imposto la selezione della checkboxlist

                ComboFormulati.Veg_Cod = CInt(Cmb_Specie.SelectedValue)
                ComboFormulati.Disciplinare_Cod = Cmb_Disciplinare.SelectedValue

                'Select Case CInt(ComboOperazione.Valore_Combo)
                '    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO
                '        ComboFormulati.TipoRichiesto = 1
                '    Case LAVCOD_DISERBO
                '        ComboFormulati.TipoRichiesto = 2
                '    Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                '        ComboFormulati.TipoRichiesto = 3
                '    Case LAVCOD_CONCIA_SEME
                '        ComboFormulati.TipoRichiesto = 5
                '    Case LAVCOD_DISSECCAMENTO
                '        ComboFormulati.TipoRichiesto = 6
                '    Case LAVCOD_GEODISINFESTAZIONE
                '        ComboFormulati.TipoRichiesto = 7
                'End Select

                'If ComboFormulati.TipoRichiesto = 4 Then
                '    ComboFormulati.Veg_Cod = 0
                'End If

                ComboFormulati.TipoRichiesto = CInt(Cmb_FormulatoClassificazioni.SelectedValue)

                If ComboFormulati.TipoRichiesto = enum_TipoFormulato.Coadiuvanti OrElse ComboFormulati.TipoRichiesto = enum_TipoFormulato.Corroboranti_Fisiofarmaci Then
                    ComboFormulati.Veg_Cod = 0
                End If
                '------------- '------------- '-------------

                ComboFormulati.FrCod = Fr_Cod
                ComboFormulati.FiltroAggiuntivo = " AND Formulati.Fr_Cod=" & Fr_Cod.ToString


                ComboFormulati.Piva = objParametriAgenda.Piva

                If Me.Txt_DataInizio.Text <> "" Then
                    Validita_Inizio = CDate(Txt_DataInizio.Text)
                End If
                If Me.Txt_DataFine.Text <> "" Then
                    Validita_Fine = CDate(Txt_DataFine.Text)
                End If

                ComboFormulati.Validita_Fine = Validita_Fine
                ComboFormulati.Validita_Inizio = Validita_Inizio

                If IsNumeric(Cmb_Epoca.SelectedValue) Then
                    Select Case CInt(ComboOperazione.Valore_Combo)
                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                            ComboFormulati.Modulo = Cmb_Epoca.SelectedValue
                            ComboFormulati.Epoca_Cod = 0
                        Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                            ComboFormulati.Epoca_Cod = Cmb_Epoca.SelectedValue
                            ComboFormulati.Modulo = 0
                    End Select
                End If

                ComboFormulati.WS_Disciplinari_AgroWS_Disciplinari = objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari
                ComboFormulati.WS_Fitofarmaci_AgroWS_Fitofarmaci = objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci

                If CellaAvversita.Visible Then

                    Select Case CInt(ComboOperazione.Valore_Combo)
                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                            ComboFormulati.Opt_Avversita_Infestanti = 0
                            ComboFormulati.Tipo_Testata = 0
                        Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                            ComboFormulati.Opt_Avversita_Infestanti = 1
                            ComboFormulati.Tipo_Testata = 1
                    End Select

                    ComboFormulati.FiltroRicerca = 2
                    ComboFormulati.Opt_Singola_Gruppo = CInt(RBL_Avversita.SelectedValue)

                    Dim Array_AvCod(0) As Integer
                    Dim Array_AvGru(0) As Integer

                    Dim strAvversita As String = ""
                    Dim NumAvv As Integer = 0
                    Dim NumAvvGru As Integer = 0

                    Select Case RBL_Avversita.SelectedValue

                        Case "0" 'singole

                            For i = 0 To GridViewAvversita.Rows.Count - 1
                                If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                                    ReDim Preserve Array_AvCod(NumAvv)
                                    Array_AvCod(NumAvv) = GridViewAvversita.Rows(i).Cells(1).Text
                                    ReDim Preserve Array_AvGru(NumAvv)
                                    Array_AvGru(NumAvv) = "0"
                                    NumAvv += 1
                                End If
                            Next

                        Case Else 'gruppi

                            For i = 0 To GridViewGruppiAvversita.Rows.Count - 1
                                If CType(GridViewGruppiAvversita.Rows(i).FindControl("ChkSelezionaGruppoAvversita"), CheckBox).Checked = True Then
                                    ReDim Preserve Array_AvGru(NumAvvGru)
                                    Array_AvGru(NumAvvGru) = GridViewGruppiAvversita.Rows(i).Cells(1).Text
                                    NumAvvGru += 1
                                End If
                            Next
                    End Select

                    If NumAvv <> 0 OrElse NumAvvGru <> 0 Then
                        If strAvversita <> "" Then
                            strAvversita = Left(strAvversita, strAvversita.Length - 4)
                            ComboFormulati.StrAvversita = strAvversita
                        End If
                    End If

                    ComboFormulati.Av_Cod = Array_AvCod
                    ComboFormulati.Av_Gru = Array_AvGru


                End If

                ComboFormulati.Flag_PrincipiAttivi = True
                ComboFormulati.Flag_ClasseTossicologica = True
                ComboFormulati.PrimaRiga_Flag = False

                ComboFormulati.CaricaComboFormulati()

                Txt_Dose_Formulati.Text = Dose
                Cmb_Udm_Formulati.SelectedIndex =
                        Cmb_Udm_Formulati.Items.IndexOf(Cmb_Udm_Formulati.Items.FindByValue(
                                UdmCod))

                'Recupero il datatable
                Dt = ViewState("dtDosi_Difesa")

                Dim Keys() As Object = {CStr(Av_Cod), CStr(Av_Gru), CStr(Fr_Cod)}

                'Trovo la riga da cancellare    (chiave = Av_Cod - Av_Gru - Fr_Cod)
                Dr = Dt.Rows.Find(Keys)

                'Elimino la riga
                Dr.Delete()

                'Associo il DataTable con la DataGrid
                GridView_Dosi_Difesa.DataSource = Dt
                GridView_Dosi_Difesa.DataBind()

                'Salvo il DataTable dentro il viewstate
                ViewState("dtDosi_Difesa") = Dt

                If Dt.Rows.Count < 1 Then
                    Me.RBL_Dose_Formulati.Enabled = True
                End If

                'Salvo il DataTable dentro il viewstate
                ViewState("dtDosi_Difesa") = Dt

            Case "Elimina"

                'Recupero il datatable
                Dt = ViewState("dtDosi_Difesa")

                Dim Keys() As Object = {CStr(Av_Cod), CStr(Av_Gru), CStr(Fr_Cod)}

                'Trovo la riga da cancellare    (chiave = Av_Cod - Av_Gru - Fr_Cod)
                Dr = Dt.Rows.Find(Keys)

                'Elimino la riga
                Dr.Delete()

                'Associo il DataTable con la DataGrid
                GridView_Dosi_Difesa.DataSource = Dt
                GridView_Dosi_Difesa.DataBind()


                If Dt.Rows.Count < 1 Then
                    Me.RBL_Dose_Formulati.Enabled = True
                End If

                'Salvo il DataTable dentro il viewstate
                ViewState("dtDosi_Difesa") = Dt

        End Select

    End Sub

    '########################################################################################
    Private Sub GridView_Dosi_Concimazione_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Dosi_Concimazione.RowCommand

        Dim IndiceRigaGriglia As Integer
        Dim Fer_Cod As String
        Dim N, P, K, Mg As String
        Dim UdmCod As Integer
        Dim Dose As String
        Dim Dt As DataTable
        Dim Dr As DataRow
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE


        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Fer_Cod = GridView_Dosi_Concimazione.Rows(IndiceRigaGriglia).Cells(2).Text()
        N = GridView_Dosi_Concimazione.Rows(IndiceRigaGriglia).Cells(5).Text()
        P = GridView_Dosi_Concimazione.Rows(IndiceRigaGriglia).Cells(7).Text()
        K = GridView_Dosi_Concimazione.Rows(IndiceRigaGriglia).Cells(8).Text()
        Mg = GridView_Dosi_Concimazione.Rows(IndiceRigaGriglia).Cells(9).Text()

        UdmCod = GridView_Dosi_Concimazione.Rows(IndiceRigaGriglia).Cells(11).Text()
        Dose = GridView_Dosi_Concimazione.Rows(IndiceRigaGriglia).Cells(12).Text()

        Select Case e.CommandName

            Case "Modifica"

                Dim TipoRichiesto As Integer = 0

                Select Case CInt(ComboOperazione.Valore_Combo)

                    Case LAVCOD_CONCIMAZIONE_FOGLIARE
                        TipoRichiesto = 2

                    Case LAVCOD_FERTIRRIGAZIONE
                        TipoRichiesto = 3

                    Case LAVCOD_DISTRIBUZIONE_CONCIME,
                        LAVCOD_SARCHIATURA_CONCIMAZIONE
                        TipoRichiesto = 5

                    Case LAVCOD_DISTRIBUZIONE_AMMENDANTI
                        TipoRichiesto = 4

                    Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                        TipoRichiesto = 1

                End Select

                ComboFertilizzanti.FerCod = Fer_Cod
                'ComboFormulati.FiltroAggiuntivo = " AND Formulati.Fr_Cod=" & Fr_Cod.ToString
                ComboFertilizzanti.TipoRichiesto = TipoRichiesto
                ComboFertilizzanti.TestoRicerca = Txt_Fertilizzanti.Text
                ComboFertilizzanti.IncludiAziendali = False
                'ComboFertilizzanti.Piva = Qs_Piva
                'ComboFertilizzanti.Fabbricato_Cod = objParametriAgenda.Fabbricato
                'ComboFertilizzanti.Cau_Mov = CAU_SCARICO

                If Me.Txt_DataInizio.Text <> "" Then
                    Validita_Inizio = CDate(Txt_DataInizio.Text)
                End If
                If Me.Txt_DataFine.Text <> "" Then
                    Validita_Fine = CDate(Txt_DataFine.Text)
                End If

                ComboFertilizzanti.Validita_Fine = Validita_Fine
                ComboFertilizzanti.Validita_Inizio = Validita_Inizio
                ComboFertilizzanti.PrimaRiga_Flag = False

                ComboFertilizzanti.CaricaComboFertilizzanti()

                Txt_Dose_Fertilizzanti.Text = Dose
                Cmb_UdM_Fertilizzanti.SelectedIndex =
                        Cmb_UdM_Fertilizzanti.Items.IndexOf(Cmb_UdM_Fertilizzanti.Items.FindByValue(
                                UdmCod))

                'Me.Lbl_Num_Fertilizzanti.Text = "Trovati " & ComboFertilizzanti.N_Fertilizzanti.ToString & " Fertilizzanti"

                '--------------------------------------------------------------------------

                'Recupero il datatable
                Dt = ViewState("dtDosi_Concimazione")

                Dim Keys() As Object = {CStr(Fer_Cod)}

                'Trovo la riga da cancellare    (chiave = Fer_Cod)
                Dr = Dt.Rows.Find(Keys)

                'Elimino la riga
                Dr.Delete()

                'Associo il DataTable con la DataGrid
                GridView_Dosi_Concimazione.DataSource = Dt
                GridView_Dosi_Concimazione.DataBind()

                'Salvo il DataTable dentro il viewstate
                ViewState("dtDosi_Concimazione") = Dt

                If Dt.Rows.Count < 1 Then
                    Me.Rbl_Dosi_Fertilizzanti.Enabled = True
                    'Me.Txt_Acqua_Concimazione.Enabled = True
                    'Me.RBL_Acqua_Concimazione.Enabled = True
                End If

                'Salvo il DataTable dentro il viewstate
                ViewState("dtDosi_Concimazione") = Dt

                Txt_N.Text = N
                Txt_P2O5.Text = P
                Txt_K2O.Text = K
                Txt_MgO.Text = Mg

            Case "Elimina"

                'Recupero il datatable
                Dt = ViewState("dtDosi_Concimazione")

                Dim Keys() As Object = {CStr(Fer_Cod)}

                'Trovo la riga da cancellare    (chiave = Fer_Cod)
                Dr = Dt.Rows.Find(Keys)

                'Elimino la riga
                Dr.Delete()

                'Associo il DataTable con la DataGrid
                GridView_Dosi_Concimazione.DataSource = Dt
                GridView_Dosi_Concimazione.DataBind()

                If Dt.Rows.Count < 1 Then
                    Me.Rbl_Dosi_Fertilizzanti.Enabled = True
                    'Me.Txt_Acqua_Concimazione.Enabled = True
                    'Me.RBL_Acqua_Concimazione.Enabled = True
                End If

                'Salvo il DataTable dentro il viewstate
                ViewState("dtDosi_Concimazione") = Dt

        End Select

    End Sub

    '######################################################################################## 
    Private Sub GridView_Operazioni_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Operazioni.RowCommand


        Dim IndiceRigaGriglia As Integer

        Dim Ricetta_Operazione_Cod As Integer
        Dim Lav_Cod As Integer
        Dim Xml_Operazione As String
        Dim Operazione_Richiesta As Integer
        Dim Id_Agenda As Integer
        Dim Ricetta_Operazione_Data As Date

        Dim Dt As DataTable
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Ricetta_Operazione_Cod = GridView_Operazioni.Rows(IndiceRigaGriglia).Cells(1).Text()
        Lav_Cod = CInt(GridView_Operazioni.Rows(IndiceRigaGriglia).Cells(2).Text())
        Xml_Operazione = GridView_Operazioni.Rows(IndiceRigaGriglia).Cells(3).Text()
        Ricetta_Operazione_Data = GridView_Operazioni.Rows(IndiceRigaGriglia).Cells(5).Text()
        Id_Agenda = GridView_Operazioni.Rows(IndiceRigaGriglia).Cells(6).Text()


        Select Case e.CommandName

            Case "Info"

                Select Case Qs_Tipo_Ricetta

                    Case enum_TipoRicetta.Standard_Destinazioni, enum_TipoRicetta.Standard_Destinazioni_Planning

                        Dim TargetUrl As String

                        objParametriAgenda.Id_Agenda = 0
                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura

                        Dim xPlEnt As XDocument
                        xPlEnt = XDocument.Parse(Xml_Operazione)
                        Dim Programmazione_Entita_cod As Integer
                        Programmazione_Entita_cod = (
                            From pE In xPlEnt.Elements("Ricetta_Operazione").Elements("DatiRicetta_Dettagli").Elements("Ricetta_Dettaglio").Elements("Ricetta_Destinazione")
                            Select pE.Attribute("programmazione_entita_cod")
                        ).First

                        If Programmazione_Entita_cod <> 0 Then
                            objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Planning

                            Dim xLeggiP As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                            Dim xDTLeggiP As DataTable =
                                xLeggiP.Leggi(0, Programmazione_Entita_cod, "", "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", "", objParametri_Server)

                            If xDTLeggiP.Rows.Count = 1 Then
                                objParametriAgenda.Programmazione_Cod = xDTLeggiP(0)("Programmazione_Cod")
                            End If
                        End If

                        objParametriAgenda.Data = Ricetta_Operazione_Data
                        objParametriAgenda.Lav_Cod = Lav_Cod
                        objParametriAgenda.Veg_Cod = Cmb_Specie.SelectedValue
                        objParametriAgenda.TipoRicetta = Qs_Tipo_Ricetta

                        'Grilli 22-05-2018 in seguito a trasformazione delle ricette aziendali in multispecie
                        If objParametriAgenda.Veg_Cod = "" Then
                            'estraggo la specie dall'operazione
                            Dim x As New AgronicaCoreContabDAL.Ricette_Operazioni_R
                            Dim veg_cod As Integer = x.VegCod_From_RicetteOperazioni(Ricetta_Operazione_Cod, "", "", objParametri_Server)
                            objParametriAgenda.Veg_Cod = veg_cod
                        End If

                        objParametriAgenda.TipoOperazioneAgenda = TipiEnumerativi.enum_Tipo_Operazione_Agenda.Ricetta

                        Dim strErrore As String = ""
                        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni 'Utility_NS.Utility_Operazioni
                        TargetUrl = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda, enum_PagineAgenda_2010.Pagina_Ricette_Edit, LeggiFlagConfigurazioneSiti:=False, strErrore:=strErrore, fromBootstrapToBootstrap:=True)

                        If strErrore <> "" Then
                            Messaggi.AgroMsgBox(strErrore, Page, , Script_Panel)
                            Exit Sub
                        End If

                        Dim Unid_Ricetta As String = System.Guid.NewGuid.ToString
                        Dim Unid_Ricetta_Operazione As String = System.Guid.NewGuid.ToString

                        Dim StringaXmlCreazione As String = XML_GeneraStringa_Ricetta()

                        Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
                        Dim res As Boolean = False
                        res = objWebW.Scrivi(Unid_Ricetta, 0, enum_TipoOperazioneDB.Scrittura, 0, "", "", StringaXmlCreazione, "", objParametri_Server)
                        res = objWebW.Scrivi(Unid_Ricetta_Operazione, 0, enum_TipoOperazioneDB.Scrittura, 0, "", "", Xml_Operazione, "", objParametri_Server)
                        If res Then
                            Response.Redirect(TargetUrl & "?unid_ricetta=" & Stringa_Codifica(Unid_Ricetta, AgroKey_EncoderDecoder) &
                                                           "&unid_ricetta_operazione=" & Stringa_Codifica(Unid_Ricetta_Operazione, AgroKey_EncoderDecoder) &
                                                           "&operazione_ricetta=" & Stringa_Codifica(Qs_Operazione, AgroKey_EncoderDecoder) &
                                                           "&r=" & Stringa_Codifica(Qs_Ricetta_Cod, AgroKey_EncoderDecoder))


                        End If


                    Case Else
                        Operazione_Richiesta = enum_TipoOperazioneDB.Lettura

                        Ripristina_Dati_nei_Controlli(Lav_Cod, Xml_Operazione, Operazione_Richiesta)

                        GridView_Operazioni.DataSource = ViewState("dt_Operazioni")
                        GridView_Operazioni.DataBind()

                        GridView_Operazioni.Rows(IndiceRigaGriglia).BackColor = System.Drawing.Color.Orange

                End Select


            Case "Modifica"

                Select Case Qs_Tipo_Ricetta

                    Case enum_TipoRicetta.Standard_Destinazioni, enum_TipoRicetta.Standard_Destinazioni_Planning

                        'Elimino la riga operazione
                        Dt = ViewState("dt_Operazioni")

                        Dim Keys() As Object = {CInt(Ricetta_Operazione_Cod), CInt(Lav_Cod), CStr(Xml_Operazione)}

                        'Trovo la riga da cancellare    (chiave = Ricetta_Operazione_Cod - Lav_Cod - Xml_Operazione)
                        Dr = Dt.Rows.Find(Keys)

                        'Elimino la riga
                        Dr.Delete()

                        ''Associo il DataTable con la DataGrid
                        'GridView_Operazioni.DataSource = Dt
                        'GridView_Operazioni.DataBind()

                        'Salvo il DataTable dentro il viewstate
                        ViewState("dt_Operazioni") = Dt

                        objParametriAgenda.Id_Agenda = 0
                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

                        Dim xPlEnt As XDocument = XDocument.Parse(Xml_Operazione)
                        Dim Programmazione_Entita_cod As Integer = (
                            From pE In xPlEnt.Elements("Ricetta_Operazione").Elements("DatiRicetta_Dettagli").Elements("Ricetta_Dettaglio").Elements("Ricetta_Destinazione")
                            Select pE.Attribute("programmazione_entita_cod")
                        ).First

                        objParametriAgenda.Data = Ricetta_Operazione_Data
                        objParametriAgenda.Lav_Cod = Lav_Cod
                        objParametriAgenda.Veg_Cod = Cmb_Specie.SelectedValue

                        'Grilli 22-05-2018 in seguito a trasformazione delle ricette aziendali in multispecie
                        If objParametriAgenda.Veg_Cod = "" Then
                            'estraggo la specie dall'operazione
                            Dim x As New AgronicaCoreContabDAL.Ricette_Operazioni_R
                            Dim veg_cod As Integer = x.VegCod_From_RicetteOperazioni(Ricetta_Operazione_Cod, "", "", objParametri_Server)
                            objParametriAgenda.Veg_Cod = veg_cod
                        End If

                        objParametriAgenda.TipoOperazioneAgenda = TipiEnumerativi.enum_Tipo_Operazione_Agenda.Ricetta
                        objParametriAgenda.TipoRicetta = Qs_Tipo_Ricetta

                        If Programmazione_Entita_cod <> 0 Then
                            objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Planning

                            Dim xLeggiP As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                            Dim xDTLeggiP As DataTable = xLeggiP.Leggi(0, Programmazione_Entita_cod, "", "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", "", objParametri_Server)

                            If xDTLeggiP.Rows.Count = 1 Then
                                objParametriAgenda.Programmazione_Cod = xDTLeggiP(0)("Programmazione_Cod")
                            End If
                        End If

                        Dim strErrore As String = ""
                        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni 'Utility_NS.Utility_Operazioni
                        Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda, enum_PagineAgenda_2010.Pagina_Ricette_Edit, LeggiFlagConfigurazioneSiti:=False, strErrore:=strErrore, fromBootstrapToBootstrap:=True)

                        If strErrore <> "" Then
                            Messaggi.AgroMsgBox(strErrore, Page, , Script_Panel)
                            Exit Sub
                        End If

                        Dim Unid_Ricetta As String = System.Guid.NewGuid.ToString
                        Dim Unid_Ricetta_Operazione As String = System.Guid.NewGuid.ToString

                        Dim StringaXmlCreazione As String = XML_GeneraStringa_Ricetta()

                        Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
                        Dim res As Boolean = False
                        res = objWebW.Scrivi(Unid_Ricetta, 0, enum_TipoOperazioneDB.Scrittura, 0, "", "", StringaXmlCreazione, "", objParametri_Server)
                        res = objWebW.Scrivi(Unid_Ricetta_Operazione, 0, enum_TipoOperazioneDB.Scrittura, 0, "", "", Xml_Operazione, "", objParametri_Server)
                        If res Then
                            Response.Redirect(TargetUrl & "?unid_ricetta=" & Stringa_Codifica(Unid_Ricetta, AgroKey_EncoderDecoder) &
                                                           "&unid_ricetta_operazione=" & Stringa_Codifica(Unid_Ricetta_Operazione, AgroKey_EncoderDecoder) &
                                                           "&operazione_ricetta=" & Stringa_Codifica(Qs_Operazione, AgroKey_EncoderDecoder) &
                                                           "&r=" & Stringa_Codifica(Qs_Ricetta_Cod, AgroKey_EncoderDecoder))

                        End If


                    Case Else

                        Operazione_Richiesta = enum_TipoOperazioneDB.Modifica

                        Ripristina_Dati_nei_Controlli(Lav_Cod, Xml_Operazione, Operazione_Richiesta)

                        ComboOperazione.Valore_Combo = Lav_Cod

                        'Recupero il datatable
                        Dt = ViewState("dt_Operazioni")

                        Dim Keys() As Object = {CInt(Ricetta_Operazione_Cod), CInt(Lav_Cod), CStr(Xml_Operazione)}

                        'Trovo la riga da cancellare    (chiave = Ricetta_Operazione_Cod - Lav_Cod - Xml_Operazione)
                        Dr = Dt.Rows.Find(Keys)

                        'Elimino la riga
                        Dr.Delete()

                        'Associo il DataTable con la DataGrid
                        GridView_Operazioni.DataSource = Dt
                        GridView_Operazioni.DataBind()

                        'Salvo il DataTable dentro il viewstate
                        ViewState("dt_Operazioni") = Dt

                        Abilita_Disabilita_Data()

                        Btn_Costi.Visible = True

                End Select



            Case "Elimina"

                'Recupero il datatable
                Dt = ViewState("dt_Operazioni")

                Dim Keys() As Object = {CInt(Ricetta_Operazione_Cod), CInt(Lav_Cod), CStr(Xml_Operazione)}

                'Trovo la riga da cancellare    (chiave = Ricetta_Operazione_Cod - Lav_Cod - Xml_Operazione)
                Dr = Dt.Rows.Find(Keys)

                'Elimino la riga
                Dr.Delete()

                'Associo il DataTable con la DataGrid
                GridView_Operazioni.DataSource = Dt
                GridView_Operazioni.DataBind()

                'Salvo il DataTable dentro il viewstate
                ViewState("dt_Operazioni") = Dt

                Abilita_Disabilita_Data()

        End Select




    End Sub

    Protected Sub ImgBtn_Cerca_Fertilizzanti_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cerca_Fertilizzanti.Click


        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Validita_Inizio = #1/1/1900#
        Validita_Fine = #12/31/2100#

        '--------------------------------------------------------
        'controllo se ho selezionato la Specie
        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox("Selezionare la Specie Vegetale!", Page, , Script_Panel)
            Exit Sub
        End If

        '--------------------------------------------------------
        'controllo se ho selezionato l'operazione
        If ComboOperazione.Testo_Combo = "" Then
            Messaggi.AgroMsgBox("Selezionare l'Intervento!", Page, , Script_Panel)
            Exit Sub
        End If

        Dim TipoRichiesto As Integer = 0

        Select Case CInt(ComboOperazione.Valore_Combo)

            Case LAVCOD_CONCIMAZIONE_FOGLIARE
                TipoRichiesto = 2

            Case LAVCOD_FERTIRRIGAZIONE
                TipoRichiesto = 3

            Case LAVCOD_DISTRIBUZIONE_CONCIME,
                LAVCOD_SARCHIATURA_CONCIMAZIONE
                TipoRichiesto = 5

            Case LAVCOD_DISTRIBUZIONE_AMMENDANTI
                TipoRichiesto = 4

            Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                TipoRichiesto = 1

        End Select

        ComboFertilizzanti.TipoRichiesto = TipoRichiesto
        ComboFertilizzanti.TestoRicerca = Txt_Fertilizzanti.Text
        ComboFertilizzanti.IncludiAziendali = False
        'ComboFertilizzanti.Piva = Qs_Piva
        'ComboFertilizzanti.Fabbricato_Cod = objParametriAgenda.Fabbricato
        'ComboFertilizzanti.Cau_Mov = CAU_SCARICO

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
        End If
        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If

        ComboFertilizzanti.Validita_Fine = Validita_Fine
        ComboFertilizzanti.Validita_Inizio = Validita_Inizio
        ComboFertilizzanti.CaricaComboFertilizzanti()

        Me.Lbl_Num_Fertilizzanti.Text = "Trovati " & ComboFertilizzanti.N_Fertilizzanti.ToString & " Fertilizzanti"

    End Sub

    Protected Sub BTN_ComboFertilizzanti_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboFertilizzanti.Click

        Dim FerCod As Integer
        Dim MatCod As Integer
        Dim N, N_Utile, P2O5, K2O, MgO As String

        Dim DatiFertilizzante As String
        'Dim TipoFertilizzante As Integer
        'Dim Epoca As Integer
        Dim Efficienza As Decimal = 1

        'Txt_Efficienza.Text = "1"

        If ComboFertilizzanti.Valore_Combo <> "" Then

            DatiFertilizzante = Split(ComboFertilizzanti.Valore_Combo, "$")(0)

            FerCod = Split(DatiFertilizzante, "/")(0)
            MatCod = Split(DatiFertilizzante, "/")(1)


            '-----------
            'EFFICIENZA
            '-----------
            'If Split(ComboFertilizzanti.Valore_Combo, "$").Length > 1 Then
            '    TipoFertilizzante = CInt(Split(ComboFertilizzanti.Valore_Combo, "$")(1))
            '    If Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione) AndAlso Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue) Then
            '        Epoca = CInt(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue)
            '    End If
            'End If

            'If CInt(ComboOperazione.Valore_Combo) = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then
            '    If Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione) AndAlso Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue) Then
            '        Epoca = CInt(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue)
            '    End If
            '    Dim objTipoFer As New AgronicaCoreMetaSchemaDAL.FertilizzantixTipoOrganici_R
            '    Dim DtFer As DataTable
            '    DtFer = objTipoFer.Leggi(FerCod, 0, "", "", objParametri_Server)
            '    If DtFer.Rows.Count > 0 Then
            '        TipoFertilizzante = DtFer.Rows(0).Item("id_tp_fer")
            '    End If
            '    If TipoFertilizzante <> 0 And Epoca <> 0 Then
            '        Dim objEff As New AgronicaCoreMetaSchemaDAL.EfficienzaxTipoFertilizzante_R
            '        Dim Regolamento_Cod As Integer = 1
            '        'Select Case Cmb_Filtro.SelectedValue
            '        '    Case "2"
            '        '        Regolamento_Cod = 1
            '        '    Case "3"
            '        '        Regolamento_Cod = 2
            '        'End Select
            '        Efficienza = objEff.Efficienza_From_Id_Tp_Fer_Em_Cod(TipoFertilizzante, Epoca, objParametri_Server)
            '        Txt_Efficienza.Text = Efficienza.ToString
            '    End If
            'End If
            Txt_Efficienza.Text = Efficienza.ToString
            '-----------
            'TITOLI
            '-----------
            Dim strNPKM As String

            strNPKM = Split(DatiFertilizzante, "/")(2)
            N = Split(strNPKM, "-")(0)
            P2O5 = Split(strNPKM, "-")(1)
            K2O = Split(strNPKM, "-")(2)
            MgO = Split(strNPKM, "-")(3)

            Txt_N.Text = N
            Txt_P2O5.Text = P2O5
            Txt_K2O.Text = K2O
            Txt_MgO.Text = MgO

            N_Utile = N
            If IsNumeric(N) Then
                N_Utile = CDbl(N) * Efficienza
            End If
            Txt_Efficienza.Text = Efficienza.ToString
            Txt_N_Utile.Text = N_Utile


        End If


    End Sub

    Protected Sub ImgBtn_DoseInserisci_Ferrtilizzanti_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_DoseInserisci_Fertilizzanti.Click

        Dim Mezzo As String
        Dim FerCod As Integer
        Dim MatCod As Integer
        Dim FerDes As String
        Dim N, N_Utile, P, K, Mg As String

        Dim DatiFertilizzante As String
        Dim Efficienza As Decimal = 1

        Dim Messaggio As String = ""

        '------------
        If Me.ComboFertilizzanti.Testo_Combo = "" Then
            Messaggi.AgroMsgBox("Selezionare un fertilizzante.", Page, , Script_Panel)
            Exit Sub
        End If

        '------------

        If Not IsNumeric(Txt_Dose_Fertilizzanti.Text) Then
            Messaggio = "Inserire una dose."
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Sub
        Else
            If InStr(Txt_Dose_Fertilizzanti.Text, ".") <> 0 Then
                Txt_Dose_Fertilizzanti.Text = Replace(Txt_Dose_Fertilizzanti.Text, ".", ",")
            End If
            If CDbl(Me.Txt_Dose_Fertilizzanti.Text) <= 0 Then
                Messaggio = "Non e' possibile inserire una dose negativa o nulla."
                Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                Exit Sub
            End If
        End If

        '------------

        Mezzo = Rbl_Dosi_Fertilizzanti.SelectedValue

        If ComboFertilizzanti.Testo_Combo <> "" Then

            FerDes = ComboFertilizzanti.Testo_Combo

            DatiFertilizzante = Split(ComboFertilizzanti.Valore_Combo, "$")(0)

            FerCod = Split(DatiFertilizzante, "/")(0)
            MatCod = Split(DatiFertilizzante, "/")(1)

            '-----------
            'TITOLI
            '-----------
            'Dim strNPKM As String

            'strNPKM = Split(DatiFertilizzante, "/")(2)
            'N = Split(strNPKM, "-")(0)
            'P2O5 = Split(strNPKM, "-")(1)
            'K2O = Split(strNPKM, "-")(2)
            'MgO = Split(strNPKM, "-")(3)

            '---------------
            'N - P - K - M
            '---------------
            If Txt_N.Text <> "" Then
                If InStr(Txt_N.Text, ".") <> 0 Then
                    Txt_N.Text = Replace(Txt_N.Text, ".", ",")
                End If
                If Not IsNumeric(Me.Txt_N.Text) Then
                    Messaggio = "Inserire un valore numerico per indicare N."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                End If
                If Me.Txt_N.Text < 0 Then
                    Messaggio = "Non e' possibile inserire una quantita' di N negativa."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                Else
                    N = Txt_N.Text
                End If
            Else
                N = "0"
            End If

            If Txt_N_Utile.Text <> "" Then
                If InStr(Txt_N_Utile.Text, ".") <> 0 Then
                    Txt_N_Utile.Text = Replace(Txt_N_Utile.Text, ".", ",")
                End If
                If Not IsNumeric(Me.Txt_N_Utile.Text) Then
                    Messaggio = "Inserire un valore numerico per indicare N Utile."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                End If
                If Me.Txt_N_Utile.Text < 0 Then
                    Messaggio = "Non e' possibile inserire una quantita' di N Utile negativa."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                Else
                    N_Utile = Txt_N_Utile.Text
                End If
            Else
                N_Utile = "0"
            End If


            '-----

            If Txt_P2O5.Text <> "" Then
                If InStr(Txt_P2O5.Text, ".") <> 0 Then
                    Txt_P2O5.Text = Replace(Txt_P2O5.Text, ".", ",")
                End If
                If Not IsNumeric(Me.Txt_P2O5.Text) Then
                    Messaggio = "Inserire un valore numerico per indicare P2O5."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                End If
                If Me.Txt_P2O5.Text < 0 Then
                    Messaggio = "Non e' possibile inserire una quantita' di P2O5 negativa."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                Else
                    P = Txt_P2O5.Text
                End If
            Else
                P = "0"
            End If

            '------

            If Txt_K2O.Text <> "" Then
                If InStr(Txt_K2O.Text, ".") <> 0 Then
                    Txt_K2O.Text = Replace(Txt_K2O.Text, ".", ",")
                End If
                If Not IsNumeric(Me.Txt_K2O.Text) Then
                    Messaggio = "Inserire un valore numerico per indicare K2O."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                End If
                If Me.Txt_K2O.Text < 0 Then
                    Messaggio = "Non e' possibile inserire una quantita' di K2O negativa."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                Else
                    K = Txt_K2O.Text
                End If
            Else
                K = "0"
            End If

            '------

            If Txt_MgO.Text <> "" Then
                If InStr(Txt_MgO.Text, ".") <> 0 Then
                    Txt_MgO.Text = Replace(Txt_MgO.Text, ".", ",")
                End If
                If Not IsNumeric(Me.Txt_MgO.Text) Then
                    Messaggio = "Inserire un valore numerico per indicare MgO."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                End If
                If CDbl(Me.Txt_MgO.Text) < 0 Then
                    Messaggio = "Non e' possibile inserire una quantita' di MgO negativa."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                Else
                    Mg = Txt_MgO.Text
                End If
            Else
                Mg = "0"
            End If

            If Txt_Efficienza.Text <> "" Then
                If InStr(Txt_Efficienza.Text, ".") <> 0 Then
                    Txt_Efficienza.Text = Replace(Txt_Efficienza.Text, ".", ",")
                End If
                If Not IsNumeric(Me.Txt_Efficienza.Text) Then
                    Messaggio = "Inserire un valore numerico per indicare l'efficienza."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                End If
                If CDbl(Me.Txt_Efficienza.Text) < 0 Then
                    Messaggio = "Non e' possibile inserire un'efficienza negativa."
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Sub
                Else
                    Efficienza = Txt_Efficienza.Text
                End If
            Else
                Efficienza = "1"
            End If

        End If

        'Disabilito i RadioButton
        Me.Rbl_Dosi_Fertilizzanti.Enabled = False


        'Inserisco il formulato nella griglia delle dosi
        Call Dosi_Inserisci_Concimazione(FerCod,
                                         FerDes,
                                         Efficienza.ToString,
                                         N, N_Utile, P, K, Mg,
                                         Txt_Dose_Fertilizzanti.Text,
                                         ,
                                         Cmb_UdM_Fertilizzanti.SelectedValue,
                                         Cmb_UdM_Fertilizzanti.SelectedItem.Text,
                                         Mezzo, )


    End Sub

    '########################################################################################
    Private Sub CaricaGriglia_Dosi_Concimazione()

        '----- Definizione delle variabili
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Fer_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fer_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Efficienza", GetType(String)))
        Dt.Columns.Add(New DataColumn("N", GetType(String)))
        Dt.Columns.Add(New DataColumn("N_Utile", GetType(String)))
        Dt.Columns.Add(New DataColumn("P", GetType(String)))
        Dt.Columns.Add(New DataColumn("K", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mg", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Fittizia", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mezzo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta_Tot", GetType(String)))


        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        Dim DtKeys(0) As DataColumn

        DtKeys(0) = Dt.Columns("Fer_Cod")

        Dt.PrimaryKey = DtKeys

        GridView_Dosi_Concimazione.DataSource = Dt
        GridView_Dosi_Concimazione.DataBind()

        ViewState("dtDosi_Concimazione") = Dt

    End Sub



    Private Sub Svuota_Controlli_Difesa()

        For i = 0 To Me.GridViewAvversita.Rows.Count - 1
            CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = False
        Next
        For i = 0 To GridViewGruppiAvversita.Rows.Count - 1
            CType(GridViewGruppiAvversita.Rows(i).FindControl("ChkSelezionaGruppoAvversita"), CheckBox).Checked = False
        Next

        Txt_Formulati.Text = ""
        Lbl_Num_Formulati.Text = ""

        ComboFormulati.ddl_Formulati.Items.Clear()

        Lbl_Dose_Etichetta.Text = ""
        Txt_Dose_Formulati.Text = ""

    End Sub

    Private Sub Svuota_Controlli_Concimazione()

        Lbl_Num_Fertilizzanti.Text = ""

        Txt_Fertilizzanti.Text = ""

        Txt_N.Text = ""
        Txt_P2O5.Text = ""
        Txt_K2O.Text = ""
        Txt_MgO.Text = ""
        Txt_Efficienza.Text = ""
        Txt_N_Utile.Text = ""

        ComboFertilizzanti.ddl_Fertilizzanti.Items.Clear()

        Txt_Dose_Fertilizzanti.Text = ""

    End Sub

    Private Sub Svuota_Controlli_Irrigazione()

        Txt_Dose_Irrigazione.Text = ""
        'Txt_Ore_Irrigazione.Text = ""
        'Txt_Portata_Irrigazione.Text = ""
        Cmb_ModifTipoIrrig.SelectedIndex = 0
        Txt_Frequenza_Irrigazione.Text = ""

    End Sub

    Private Sub Svuota_Controlli_Trappole()

        cmb_Trappola.Items.Clear()
        cmb_Ditte.Items.Clear()
        cmb_Avversita.Items.Clear()

        Txt_CodAvversita.Text = ""
        Txt_GiorniFeromone.Text = ""

    End Sub






    Protected Sub cmb_Trappola_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmb_Trappola.SelectedIndexChanged

        If cmb_Trappola.SelectedIndex > 0 Then 'Se seleziono un elemento valido dalla combo delle trappole..

            Dim TrappolaUso As Integer

            Select Case CInt(ComboOperazione.Valore_Combo)
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE
                    TrappolaUso = enum_TrappoleUso.Monitor
                Case LAVCOD_CATTURE_MASSA
                    TrappolaUso = enum_TrappoleUso.CattureDiMassa
                Case LAVCOD_CONFUSIONE_SESSUALE
                    TrappolaUso = enum_TrappoleUso.ConfusioneSessuale
                Case LAVCOD_DISORIENTAMENTO_SESSUALE
                    TrappolaUso = enum_TrappoleUso.Disorientamento
            End Select

            Dim clc = New AgronicaCoreUtility.CaricaListControl
            'Carico tutte le avversità che sono combattute dalla trappola scelta per quella specie vegetale
            clc.AvversitaxTrappole(CType(Me.cmb_Avversita, ListControl),
                                                                    True, "", "0",
                                                                    CInt(Cmb_Specie.SelectedValue),
                                                                    Me.cmb_Trappola.SelectedItem.Value,
                                                                    TrappolaUso,
                                                                    "", "", objParametri_Server)

            'Carico le ditte della trappola scelta
            clc.DittexTrappole(CType(Me.cmb_Ditte, ListControl),
                                                                True, "", "0",
                                                                CInt(Me.cmb_Trappola.SelectedItem.Value),
                                                                "", "", objParametri_Server)

            'Carico i dati relativi alla durata della trappola
            Dim Giorni As Integer
            Dim objTrap As New AgronicaCoreMetaSchemaDAL.Trappole_R
            Giorni = objTrap.TrapDur_from_TrapCod(CInt(Me.cmb_Trappola.SelectedItem.Value), objParametri_Server)
            Me.Txt_GiorniFeromone.Text = Giorni

        Else 'Se seleziono la riga vuota nella combo delle trappole svuoto le combo delle ditte e delle avversità

            Me.cmb_Ditte.Items.Clear()
            Me.cmb_Avversita.Items.Clear()

        End If

    End Sub

    Protected Sub cmb_Avversita_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmb_Avversita.SelectedIndexChanged

        If Me.cmb_Avversita.SelectedItem.Value = "" OrElse Me.cmb_Avversita.SelectedItem.Value = "0" OrElse Me.cmb_Avversita.SelectedItem.Value = "-1" Then
            Me.Txt_CodAvversita.Text = ""
            Exit Sub
        End If

        Dim ObjAv As New AgronicaCoreMetaSchemaDAL.Avversita_R
        Me.Txt_CodAvversita.Text = ObjAv.Abbreviazione_from_AvCod(CInt(Me.cmb_Avversita.SelectedItem.Value), objParametri_Server)

    End Sub



    Private Function objXml() As Object
        Throw New NotImplementedException
    End Function

#Region "Salvataggio Ricetta"

    Protected Sub Btn_Salva_Click(sender As Object, e As EventArgs) Handles Btn_Salva.Click

        Dim ricetta_cod As Integer = Salva_Ricetta()

        If ricetta_cod = 0 Then
            Return
        End If

        If Not IsNothing(Qs_SitoOrigine_Cod) Then
            Select Case Qs_SitoOrigine_Cod
                Case Enum_SiteRedirector.Sito_PianoConcimazione_2017
                    Dim objPC_2017 As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017

                    objPC_2017.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.MenuBS
                    objPC_2017.Pagina_SitoOrigine = enum_PagineAgenda_2010.Pagina_Ricette_Edit
                    objPC_2017.Piva = Qs_Piva
                    objPC_2017.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                    Dim link As String = RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objPC_2017)

                    Response.Redirect(link)
                Case Else

            End Select
        End If

        If Qs_Origine = "" Then
            Qs_Origine = "../menu/menu.aspx"
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                Qs_Origine = "../Menu/MenuBS_Agenda_Nuovo.aspx"
                Response.Redirect(Qs_Origine)
            End If
        End If

        Dim StrRedirect As String
        If Qs_Destinazione <> "" Then
            StrRedirect = Qs_Destinazione
        Else
            'provengo al momento da ricette lista
            StrRedirect = "Ricette_Manager.aspx"

            ''provengo direttamente dal menu agenda vecchio
            ''----- Chiudo la finestra
            'Page.Master.FindControl("Form1").Controls.Add( _
            '    New LiteralControl( _
            '        "<script language='javascript'>window.close();</script>"))
        End If
        StrRedirect = StrRedirect & "?origine=" & Stringa_Codifica(Qs_Origine, AgroKey_EncoderDecoder, Server) _
                                        & "&p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)

        Response.Redirect(StrRedirect)

    End Sub

    Private Function Salva_Ricetta() As Integer

        Dim objRicetta_Read As AgronicaCoreContabBIZ.Ricette_R  'Object
        Dim objRicetta_Write As AgronicaCoreContabBIZ.Ricette_W 'Object

        Dim StringaXmlCreazione As String
        Dim StringaXmlCancellazione As String

        Dim Ricetta_Cod As Integer = 0
        Dim strDummy As String
        Dim Messaggio As String

        Dim i As Integer
        Dim AlmenoUna As Boolean = False

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        'Se voglio cancellare un'operazione non mi servono i controlli
        If CInt(Qs_Operazione) <> enum_TipoOperazioneDB.Cancellazione Then

            '------------------------------------------------------------
            '----- Verifico la correttezza delle informazioni inserite
            '------------------------------------------------------------

            If Qs_Tipo_Ricetta <> enum_TipoRicetta.Standard_Destinazioni AndAlso Me.Cmb_Specie.SelectedItem.Text = "" Then
                Messaggio = "E' necessario selezionare una Specie Vegetale!"
                Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                Return 0
            End If

            If Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni AndAlso Me.Txt_Ricetta_Numero.Text = "" Then
                Messaggio = "E' necessario indicare il codice della Ricetta!"
                Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                Return 0
            End If

            'Verifico di aver inserito una data di inzio e di fine
            If Me.Txt_DataInizio.Text = "" Then
                Messaggio = "E' necessario selezionare una Data di Inizio!"
                Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                Return 0
            End If
            If Me.Txt_DataFine.Text = "" Then
                Messaggio = "E' necessario selezionare una Data di Fine!"
                Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                Return 0
            End If

            If Me.Txt_Ricetta_Des.Text = "" Then
                If Qs_Tipo_Ricetta = enum_TipoRicetta.Standard_Destinazioni Then
                    Me.Txt_Ricetta_Des.Text = Txt_Ricetta_Numero.Text & " (" & Me.Txt_DataInizio.Text & ")"
                Else
                    Messaggio = "E' necessario indicare la Descrizione della Ricetta!"
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Return 0
                End If
            End If

            'se ho selezionato varietà specifiche..
            'controllo di averne selezionata almeno una!
            If Me.RBL_Cultivar.SelectedValue <> "0" Then

                For i = 0 To Me.CBL_Cultivar.Items.Count - 1
                    If CBL_Cultivar.Items(i).Selected Then
                        AlmenoUna = True
                        Exit For
                    End If
                Next

                If Not AlmenoUna Then
                    Messaggio = "Se si sceglie di associare la ricetta a Varietà specifiche, occorre indicarle!"
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Return 0
                End If

            End If

        End If



        '=======================
        '===  Aggiornamento  ===
        '=======================

        Try

            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Select Case CInt(Qs_Operazione)

                Case enum_TipoOperazioneDB.Scrittura,
                     enum_TipoOperazioneDB.Modifica,
                     enum_TipoOperazioneDB.Copia

                    StringaXmlCreazione = XML_GeneraStringa_Ricetta()

                    objRicetta_Write = New AgronicaCoreContabBIZ.Ricette_W

                    Dim oldRicettaOperazioneCod As Integer = 0
                    'In caso di Modifica prima CANCELLO la vecchia RICETTA
                    If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Modifica Then

                        objRicetta_Read = New AgronicaCoreContabBIZ.Ricette_R

                        StringaXmlCancellazione = objRicetta_Read.Ricetta_Leggi(CStr(Qs_Ricetta_Cod),
                                                                                "",
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                CBool(True),
                                                                                objParametri_Server)

                        objRicetta_Read = Nothing



                        '  Vanni, 17/05/2013 15:17:26:  aggiunta gestione per dati Precision Farming

                        Dim xLeggiTmp As New AgronicaCoreContabDAL.Ricette_Operazioni_R
                        Dim dtLeggiTmp As DataTable =
                            xLeggiTmp.Leggi(Qs_Ricetta_Cod, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                        If dtLeggiTmp.Rows.Count > 0 Then
                            oldRicettaOperazioneCod = dtLeggiTmp.Rows(0)("Ricetta_Operazione_Cod")
                        End If

                        strDummy = objRicetta_Write.Ricetta_Scrivi(
                                                            CStr(StringaXmlCancellazione),
                                                            Ricetta_Cod,
                                                            objParametri_Server)

                        '--------------------------------------------------------------

                        StringaXmlCreazione = Replace(StringaXmlCreazione, "TipoOperazioneDB=""0""", "TipoOperazioneDB =""1""")

                        'azzero eventuali chiavi
                        'Nota: Il trucco è di rendere negativi i codici 
                        'in modo tale che il componente ne crei dei nuovi
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_cod=""", "ricetta_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_operazione_cod=""", "ricetta_operazione_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_dettaglio_cod=""", "ricetta_dettaglio_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_tecnico_cod=""", "ricetta_tecnico_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_destinazione_cod=""", "ricetta_destinazione_cod =""-")


                        '--------------------------------------------------------------

                    End If

                    '--------------------------------------------------------------------
                    '------------------MODIFICA PER COPIA RICETTA------------------------
                    '--------------------------------------------------------------------
                    'In caso di Copie non CANCELLO la vecchia RICETTA ma rimpiazzo i codici
                    'Devo inoltre cambiare la data di validità inizio e fine di
                    'Ricette_Operazioni e Ricette_Dettagli copiando quella di Ricette
                    If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Copia Then
                        '--------------------------------------------------------------

                        StringaXmlCreazione = Replace(StringaXmlCreazione, "TipoOperazioneDB=""0""", "TipoOperazioneDB =""1""")

                        'azzero eventuali chiavi
                        'Nota: Il trucco è di rendere negativi i codici 
                        'in modo tale che il componente ne crei dei nuovi
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_cod=""", "ricetta_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_operazione_cod=""", "ricetta_operazione_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_dettaglio_cod=""", "ricetta_dettaglio_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_tecnico_cod=""", "ricetta_tecnico_cod =""-")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "ricetta_destinazione_cod=""", "ricetta_destinazione_cod =""-")


                        'date Ricette_Operazioni e Ricette_Dettagli
                        'Dim pattern As String = "\s+"
                        'Dim replacement As String = " "
                        'Dim rgx As New Regex(pattern)
                        'Dim result As String = rgx.Replace(StringaXmlCreazione, replacement)
                        Dim Validita_Inizio As Date
                        Dim Validita_Fine As Date
                        Validita_Inizio = #1/1/1900#
                        Validita_Fine = #12/31/2100#
                        If Me.Txt_DataInizio.Text <> "" Then
                            Validita_Inizio = CDate(Txt_DataInizio.Text)
                        End If
                        Select Case Me.RBL_Data.SelectedValue
                            Case "0"
                                Validita_Fine = Validita_Inizio
                            Case Else
                                If Me.Txt_DataFine.Text <> "" Then
                                    Validita_Fine = CDate(Txt_DataFine.Text)
                                End If
                        End Select
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "validita_inizio=""" & Qs_Data_Inizio & "", "validita_inizio=""" & Validita_Inizio & "")
                        StringaXmlCreazione = Replace(StringaXmlCreazione, "validita_fine=""" & Qs_Data_Fine & "", "validita_fine=""" & Validita_Fine & "")

                        'importante, devo inoltre eliminare 
                        'il nodo dei ricettaxagenda che mi posso essere portato dietro nella copia
                        'lo faccio in XML_GeneraStringa_Ricetta

                        '--------------------------------------------------------------
                    End If
                    '--------------------------------------------------------------------
                    '-------------FINE MODIFICA PER COPIA RICETTA------------------------
                    '--------------------------------------------------------------------




                    'Inserisco poi la NUOVA RICETTA
                    strDummy = objRicetta_Write.Ricetta_Scrivi(
                                                        CStr(StringaXmlCreazione),
                                                        Ricetta_Cod,
                                                        objParametri_Server)


                    '  Vanni, 17/05/2013 15:01:50: aggiunta gestione per dati Precision Farming
                    If oldRicettaOperazioneCod <> 0 Then
                        Dim AggiornaOperazioniGrafica As New AgronicaCoreGisDAL.GIS_Entita_W
                        AggiornaOperazioniGrafica.AggiornaCodiceRicetta_operazione(objParametri_Server.PivaSuperUser, oldRicettaOperazioneCod, Ricetta_Cod, "", objParametri_Server)
                    End If



                    'Distruggo gli oggetti COM+
                    objRicetta_Write = Nothing

                    '===============================================


                Case enum_TipoOperazioneDB.Cancellazione

                    'Creo gli oggetti COM+
                    objRicetta_Write = New AgronicaCoreContabBIZ.Ricette_W
                    objRicetta_Read = New AgronicaCoreContabBIZ.Ricette_R

                    'Recupero la stringa XML di cancellazione
                    StringaXmlCancellazione = objRicetta_Read.Ricetta_Leggi(CStr(Qs_Ricetta_Cod),
                                                        "",
                                                        0,
                                                        0,
                                                        0,
                                                        CBool(True),
                                                        objParametri_Server)

                    'Distruggo gli oggetti COM+
                    objRicetta_Read = Nothing

                    'Cancello i dati esistenti
                    strDummy = objRicetta_Write.Ricetta_Scrivi(
                                                        CStr(StringaXmlCancellazione),
                                                        Ricetta_Cod,
                                                        objParametri_Server)


                    'Distruggo gli oggetti COM+
                    objRicetta_Write = Nothing

                    '===============================================


            End Select

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception
            Ricetta_Cod = 0

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try


        Return Ricetta_Cod

    End Function
    '########################################################################################
    Private Function XML_GeneraStringa_Ricetta() As String

        Dim i As Integer
        Dim Dt_Operazioni As New DataTable

        Dim Piva As String = ""
        Dim Sa_Cod As Integer = 0

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XMLDatiRicetta As System.Xml.XmlElement
        Dim XMLRicetta As System.Xml.XmlElement
        Dim XMLDatiRicetta_Operazioni As System.Xml.XmlElement
        Dim XMLDatiRicettaxCultivar As System.Xml.XmlElement
        Dim XMLDatiRicettaxNote As System.Xml.XmlElement

        Dim str_Ricetta_Operazione As String
        Dim str_Ricetta_Cultivar As String
        Dim str_Ricetta_Note As String

        Dim Array_CulCod() As String
        Dim N_CulCod As Integer = 0
        Dim Cul_Cod As Integer

        Dim Array_Note() As String
        Dim N_Note As Integer = 0

        Dim Validita_Inizio As Date = #1/1/1900#
        Dim Validita_Fine As Date = #12/31/2100#

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            Validita_Fine = CDate(Txt_DataInizio.Text)
        End If

        Select Case Me.RBL_Data.SelectedValue
            Case "0"
                'Validita_Fine = Validita_Inizio
            Case Else
                If Me.Txt_DataFine.Text <> "" Then
                    Validita_Fine = CDate(Txt_DataFine.Text)
                End If
        End Select


        If Dt_Operazioni IsNot Nothing Then

            Dt_Operazioni = ViewState("dt_Operazioni")

        End If

        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab

        '----- DatiRicetta
        XMLDatiRicetta = XmlDoc.CreateElement("DatiRicetta")

        If Chk_RicettaPubblica.Checked Then
            Piva = ""
            Sa_Cod = 0
        Else
            Piva = objParametriAgenda.Piva
            Sa_Cod = objParametriAgenda.Sa_Cod
        End If

        '----- Programmazione_Cod
        Dim Programmazione_Cod As Integer
        If Not IsNothing(ViewState("Programmazione_Cod")) AndAlso IsNumeric(ViewState("Programmazione_Cod")) Then
            Programmazione_Cod = ViewState("Programmazione_Cod")
        Else
            Programmazione_Cod = 0
        End If

        '----- Ricetta
        XMLDatiRicetta.InnerXml = objXml.XML_Ricetta(
                                        CInt(enum_TipoOperazioneDB.Scrittura),
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        frm_BaseCode,
                                        frm_TopCode,
                                        0,
                                        CStr(Me.Txt_Ricetta_Des.Text),
                                        CStr(Me.Txt_Ricetta_Des.Text),
                                        If(Me.Cmb_Specie.SelectedValue = "", -1, CInt(Me.Cmb_Specie.SelectedValue)),
                                        CStr(Me.Txt_Note.Text),
                                        CDate(Validita_Inizio),
                                        CDate(Validita_Fine),
                                        Piva,
                                        Sa_Cod,
                                        CInt(Qs_Tipo_Ricetta),
                                        CStr(Txt_Ricetta_Numero.Text),
                                        Programmazione_Cod)

        XMLRicetta = XMLDatiRicetta.SelectSingleNode("Ricetta")

        '----- DatiRicettaxCultivar

        XMLDatiRicettaxCultivar = XmlDoc.CreateElement("DatiRicettaxCultivar")

        XMLRicetta.AppendChild(XMLDatiRicettaxCultivar)

        'Cul_Cod = -1 ---> TUTTE LE VARIETA'
        If Me.RBL_Cultivar.SelectedValue = "0" Then
            Cul_Cod = -1
        Else
            For i = 0 To Me.CBL_Cultivar.Items.Count - 1
                If CBL_Cultivar.Items(i).Selected Then
                    ReDim Preserve Array_CulCod(N_CulCod)
                    Array_CulCod(N_CulCod) = CBL_Cultivar.Items(i).Value
                    N_CulCod += 1
                End If
            Next
        End If


        If Cul_Cod = -1 Then

            str_Ricetta_Cultivar = objXml.XML_RicettaxCultivar(enum_TipoOperazioneDB.Scrittura,
                                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                                        0,
                                                        If(Me.Cmb_Specie.SelectedValue = "", -1, CInt(Me.Cmb_Specie.SelectedValue)),
                                                        CInt(Cul_Cod),
                                                        CDate(Validita_Inizio),
                                                        CDate(Validita_Fine))

            XMLDatiRicettaxCultivar.InnerXml = XMLDatiRicettaxCultivar.InnerXml & str_Ricetta_Cultivar

        Else

            For i = 0 To UBound(Array_CulCod)

                str_Ricetta_Cultivar = objXml.XML_RicettaxCultivar(enum_TipoOperazioneDB.Scrittura,
                                                            CStr(Session("ASG_SuperUser_CodFiscale")),
                                                            0,
                                                            If(Me.Cmb_Specie.SelectedValue = "", -1, CInt(Me.Cmb_Specie.SelectedValue)),
                                                            CInt(Array_CulCod(i)),
                                                            CDate(Validita_Inizio),
                                                            CDate(Validita_Fine))

                XMLDatiRicettaxCultivar.InnerXml = XMLDatiRicettaxCultivar.InnerXml & str_Ricetta_Cultivar

            Next

        End If

        '----- DatiRicettaxNote

        XMLDatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote")

        XMLRicetta.AppendChild(XMLDatiRicettaxNote)

        For i = 0 To Me.CBL_Note.Items.Count - 1
            If CBL_Note.Items(i).Selected Then
                ReDim Preserve Array_Note(N_Note)
                Array_Note(N_Note) = CBL_Note.Items(i).Value
                N_Note += 1
            End If
        Next

        If Array_Note IsNot Nothing Then

            For i = 0 To UBound(Array_Note)

                str_Ricetta_Note = objXml.XML_RicettaxNote(enum_TipoOperazioneDB.Scrittura,
                                                            CStr(Session("ASG_SuperUser_CodFiscale")),
                                                            0,
                                                            0,
                                                            CInt(Array_Note(i)),
                                                            CDate(Validita_Inizio),
                                                            CDate(Validita_Fine))

                XMLDatiRicettaxNote.InnerXml = XMLDatiRicettaxNote.InnerXml & str_Ricetta_Note

            Next

        End If


        '----- DatiRicetta_Operazioni

        XMLDatiRicetta_Operazioni = XmlDoc.CreateElement("DatiRicetta_Operazioni")

        XMLRicetta.AppendChild(XMLDatiRicetta_Operazioni)

        For i = 0 To Dt_Operazioni.Rows.Count - 1

            str_Ricetta_Operazione = Dt_Operazioni.Rows(i).Item("Xml_Operazione").ToString

            XMLDatiRicetta_Operazioni.InnerXml = XMLDatiRicetta_Operazioni.InnerXml & str_Ricetta_Operazione

        Next



        '--------------------------------------------------------------------
        '------------------MODIFICA PER COPIA RICETTA------------------------
        '--------------------------------------------------------------------
        'se sto copiando una ricetta devo eliminare i riferimenti a ricettexagenda, che si riferiscono
        'all'operazione copiata
        If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Copia Then
            Dim XmlNodeList1 As XmlNodeList = XMLDatiRicetta.SelectNodes("Ricetta")
            For Each node1 As XmlNode In XmlNodeList1
                Dim XmlNodeList2 As XmlNodeList = node1.SelectNodes("DatiRicetta_Operazioni")
                For Each node2 As XmlNode In XmlNodeList2
                    Dim XmlNodeList4 As XmlNodeList = node2.SelectNodes("Ricetta_Operazione")
                    For Each node4 As XmlNode In XmlNodeList4
                        Dim XmlNodeList5 As XmlNodeList = node4.SelectNodes("DatiRicettaxAgenda_2")
                        For Each node5 As XmlNode In XmlNodeList5
                            node4.RemoveChild(node5)
                        Next
                    Next
                Next
            Next
        End If
        '--------------------------------------------------------------------
        '-------------FINE MODIFICA PER COPIA RICETTA------------------------
        '--------------------------------------------------------------------



        '----- Assemblo la struttura

        XmlDoc.AppendChild(XMLDatiRicetta)

        objXml = Nothing

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml

    End Function

    '########################################################################################
    Private Function XML_GeneraStringa_Ricetta_Concimazione() As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument

        Dim XML_Operazione As XmlElement

        Dim XML_DatiRicettaxNote As XmlElement
        Dim XML_DatiRicettaDettagliTecnici As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement
        Dim XML_RicettaDettaglio As XmlElement

        Dim str_DatiRicettaDettagli As String
        Dim str_RicettaDettaglio As String
        Dim str_DatiRicettaDettagliTecnico_2 As String
        Dim str_DatiRicettaDettagliCosti As String
        Dim str_RicettaDestinazioni As String
        Dim str_RicettaDestinazione As String

        Dim i, j As Integer

        Dim strOperazione As String
        Dim strNote As String

        Dim frm_VegCod As Integer

        Dim frm_Epoca As Integer = 0
        Dim Acqua As Decimal = 0
        Dim AcquaTot As Decimal = 0
        Dim frm_NoteIntervento As String

        Dim Mezzo As Integer


        Dim Messaggio As String

        Dim Validita_Inizio As Date = #1/1/1900#
        Dim Validita_Fine As Date = #12/31/2100#

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            Validita_Fine = CDate(Txt_DataInizio.Text)
        End If

        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If


        '------------------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------------------

        '------------------------------------------------------------
        '----- Deve essere selezionata la Specie Vegetale

        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggio = "E' necessario selezionare una specie vegetale"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        Else
            frm_VegCod = Me.Cmb_Specie.SelectedItem.Value
        End If

        Dim Dt As DataTable = ViewState("dtDosi_Concimazione")

        If Dt.Rows.Count = 0 Then
            Messaggio = "E' necessario selezionare almeno un Fertilizzante!"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        End If


        Dim strRiepilogo As String = ComboOperazione.Testo_Combo


        Dim SuperficieTotaleCentro As Decimal = 0

        For j = 0 To GridView_Impianti.Rows.Count - 1
            If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then
                SuperficieTotaleCentro = SuperficieTotaleCentro + CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
            End If
        Next

        '---------------------------------------------------
        '----- Acqua  +  NoteIntervento
        '---------------------------------------------------

        'se è stata scelta la dose d'acqua/ha salvo il dato negativo

        Select Case CInt(ComboOperazione.Valore_Combo)

            Case LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                If Me.Txt_Acqua_Concimazione.Text <> "" Then

                    If Not IsNumeric(Me.Txt_Acqua_Concimazione.Text) Then
                        Messaggio = " E' necessario indicare " & Chr(13) &
                                    " gli ettolitri di acqua " & Chr(13) &
                                    " con un valore numerico "
                        Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                        Exit Function
                    Else
                        If CDbl(Me.Txt_Acqua_Concimazione.Text) <= 0 Then
                            Messaggio = " E' necessario indicare " & Chr(13) &
                                        " gli ettolitri di acqua " & Chr(13) &
                                        " con un valore numerico positivo "
                            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                            Exit Function
                        Else
                            If InStr(Txt_Acqua_Concimazione.Text, ".") <> 0 Then
                                Txt_Acqua_Concimazione.Text = Replace(Txt_Acqua_Concimazione.Text, ".", ",")
                            End If
                        End If

                    End If
                    Select Case RBL_Acqua_Formulati.SelectedValue
                        Case "0"
                            Acqua = CDbl(Me.Txt_Acqua_Concimazione.Text)
                            AcquaTot = Acqua
                        Case "1"
                            Acqua = -CDbl(Me.Txt_Acqua_Concimazione.Text)
                            AcquaTot = Math.Abs(Acqua) * SuperficieTotaleCentro
                    End Select
                Else

                    Messaggio = " E' necessario indicare " & Chr(13) &
                                       " gli ettolitri di acqua "
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Function

                End If

        End Select

        frm_NoteIntervento = Me.Txt_Note_Concimazione.Text


        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab

        Mezzo = Rbl_Dosi_Fertilizzanti.SelectedValue

        str_DatiRicettaDettagli = ""
        str_RicettaDestinazioni = ""

        Dim Dose As Decimal
        Dim DoseTrasformata As Decimal
        Dim Udm_Cod, Udm_Cod_Trasf As Integer
        Dim strFert As String
        Dim N, P, K, Mg As String
        Dim Sup_Imp As Decimal = 0
        Dim Frazione As Decimal = 0
        Dim QuantitaTotale As Decimal = 0
        Dim QuantitaImpianto As Decimal = 0


        For i = 0 To GridView_Dosi_Concimazione.Rows.Count - 1

            strFert = GridView_Dosi_Concimazione.Rows(i).Cells(3).Text

            N = GridView_Dosi_Concimazione.Rows(i).Cells(5).Text
            P = GridView_Dosi_Concimazione.Rows(i).Cells(7).Text
            K = GridView_Dosi_Concimazione.Rows(i).Cells(8).Text
            Mg = GridView_Dosi_Concimazione.Rows(i).Cells(9).Text

            Dose = CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(12).Text)
            Udm_Cod = CInt(GridView_Dosi_Concimazione.Rows(i).Cells(11).Text)

            Select Case Udm_Cod
                Case 3  'g
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose / 1000  'caso in cui ho i grammi
                Case 2032 'mg
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose / 1000000  'caso in cui ho i grammi
                Case 4  'q
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose * 100  'caso in cui ho i quintali
                Case 304 't
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose * 1000   'caso in cui ho le tonnelate
                Case 101 'ml
                    Udm_Cod_Trasf = 29
                    DoseTrasformata = Dose / 1000  'caso in cui ho i ml
                Case 104 'cc
                    Udm_Cod_Trasf = 29
                    DoseTrasformata = Dose / 100  'caso in cui ho i cc
                Case 2, 29 'kg,l
                    Udm_Cod_Trasf = Udm_Cod
                    DoseTrasformata = Dose
            End Select


            str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                        CInt(enum_TipoOperazioneDB.Scrittura),
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        CInt(Qs_Ricetta_Cod),
                                        0,
                                        0,
                                        0,
                                        FERTILIZZANTI,
                                        CInt(GridView_Dosi_Concimazione.Rows(i).Cells(2).Text),
                                        0,
                                        Udm_Cod_Trasf,
                                        Udm_Cod,
                                        Dose,
                                        Validita_Inizio,
                                        Validita_Fine,
                                        ,
                                        CAU_LAVORAZIONE)

            XmlDoc2.LoadXml(str_RicettaDettaglio)

            XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

            str_DatiRicettaDettagliTecnico_2 = objXml.XML_Ricetta_DettaglioTecnico_2(
                                        enum_TipoOperazioneDB.Scrittura,
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        , , , , , , , , , , , , , , , , , , , , , , , ,
                                        CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(9).Text),
                                        CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(5).Text),
                                        CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(7).Text),
                                        CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(8).Text),
                                           , , ,
                                        CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(4).Text))

            For j = 0 To GridView_Impianti.Rows.Count - 1

                If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then

                    Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)

                    '0=HL 1=HA
                    If Mezzo = 0 Then
                        QuantitaTotale = DoseTrasformata * AcquaTot
                    Else
                        QuantitaTotale = DoseTrasformata * SuperficieTotaleCentro
                    End If

                    Frazione = Sup_Imp / SuperficieTotaleCentro

                    QuantitaImpianto = QuantitaTotale * Frazione

                    str_RicettaDestinazione = objXml.XML_Ricetta_Destinazione(
                                        enum_TipoOperazioneDB.Scrittura,
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        , , , ,
                                        GridView_Impianti.Rows(j).Cells(1).Text,
                                        GridView_Impianti.Rows(j).Cells(2).Text,
                                        GridView_Impianti.Rows(j).Cells(4).Text,
                                        GridView_Impianti.Rows(j).Cells(5).Text,
                                        ,
                                        QuantitaImpianto,
                                        , )

                    str_RicettaDestinazioni &= str_RicettaDestinazione

                End If

            Next

            XML_RicettaDettaglio.InnerXml = str_DatiRicettaDettagliTecnico_2 & str_RicettaDestinazioni

            str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml

            strRiepilogo &= " --- " & strFert

        Next

        Me.Lbl_Riepilogo.Text = strRiepilogo


        '-------------------
        ' Costi

        str_DatiRicettaDettagliCosti = ""

        SalvaCostiAccessori_Ricetta(str_DatiRicettaDettagliCosti)

        If Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione) AndAlso
            Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue) AndAlso
            ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue <> "" Then
            frm_Epoca = CInt(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue)
        End If

        '------------------------------------------------
        strOperazione = objXml.XML_Ricetta_Operazione(
                                    CInt(enum_TipoOperazioneDB.Scrittura),
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    frm_BaseCode,
                                    frm_TopCode,
                                    (Qs_Ricetta_Cod),
                                    0,
                                    CInt(ComboOperazione.Valore_Combo),
                                    CStr(strRiepilogo),
                                    CStr(Me.Txt_Note_Concimazione.Text),
                                    ,
                                    ,
                                    CInt(frm_Epoca),
                                    CInt(Mezzo),
                                    Validita_Inizio,
                                    Validita_Fine,
                                    0, 0, 0, )


        XmlDoc.LoadXml(strOperazione)

        XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")


        '----- DatiRicettaxNote

        XML_DatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

        XML_Operazione.AppendChild(XML_DatiRicettaxNote)

        For i = 0 To Me.CBL_Consigli_Concimazione.Items.Count - 1
            If CBL_Consigli_Concimazione.Items(i).Selected Then
                strNote = objXml.XML_RicettaxNote_2(enum_TipoOperazioneDB.Scrittura,
                                                           CStr(Session("ASG_SuperUser_CodFiscale")),
                                                           0,
                                                           0,
                                                           CInt(CBL_Consigli_Concimazione.Items(i).Value),
                                                            Validita_Inizio,
                                                            Validita_Fine
                                                           )

                XML_DatiRicettaxNote.InnerXml = XML_DatiRicettaxNote.InnerXml & strNote
            End If
        Next


        '----- DatiRicetta_Dettagli_Tecnici
        XML_DatiRicettaDettagliTecnici = XmlDoc.CreateElement("DatiRicetta_Dettagli_Tecnici")

        If Acqua <> 0 Then
            XML_DatiRicettaDettagliTecnici.InnerXml = XML_GeneraBlocco_RicettaDettaglioTecnico_DPI(Acqua)
        End If

        XML_Operazione.AppendChild(XML_DatiRicettaDettagliTecnici)

        '----- DatiRicetta_Dettagli

        XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

        XML_DatiRicettaDettagli.InnerXml = str_DatiRicettaDettagli & str_DatiRicettaDettagliCosti

        XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml


    End Function

    '########################################################################################
    Private Function XML_GeneraStringa_Ricetta_Irrigazione() As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument

        Dim XML_Operazione As XmlElement

        Dim XML_DatiRicettaxNote As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement
        Dim XML_RicettaDettaglio As XmlElement

        Dim str_DatiRicettaDettagli As String
        Dim str_DatiRicettaDettagliCosti As String

        Dim str_RicettaDettaglio As String

        Dim i As Integer

        Dim strOperazione As String
        Dim strNote As String

        Dim frm_VegCod As Integer

        Dim Udm_Cod As Integer
        Dim Dose As Decimal
        Dim Ore As Decimal = 0
        Dim Portata As Integer = 0
        Dim Inizio_Micro As Date = AGRODATAINIZIO
        Dim Fine_Micro As Date = AGRODATAFINE
        Dim Frequenza As Integer = 0
        Dim frm_NoteIntervento As String

        Dim Messaggio As String

        Dim Freatimetro As Integer

        Dim Validita_Inizio As Date = #1/1/1900#
        Dim Validita_Fine As Date = #12/31/2100#

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            Validita_Fine = CDate(Txt_DataInizio.Text)
        End If

        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If
        '------------------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------------------

        '------------------------------------------------------------
        '----- Deve essere selezionata la Specie Vegetale

        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggio = "E' necessario selezionare una specie vegetale"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        Else
            frm_VegCod = Me.Cmb_Specie.SelectedItem.Value
        End If

        Dim strRiepilogo As String = ComboOperazione.Testo_Combo


        '---------------------------------------------------
        'controllo INPUT

        Udm_Cod = Cmb_Udm_Irrigazione.SelectedItem.Value

        If Me.Txt_Dose_Irrigazione.Text = "" Then
            Messaggi.AgroMsgBox("Inserire la Dose!", Page, , Script_Panel)
            Exit Function
        Else
            If Not IsNumeric(Me.Txt_Dose_Irrigazione.Text) Then
                Messaggi.AgroMsgBox("La Dose deve essere un numero!", Page, , Script_Panel)
                Exit Function
            Else
                If CDbl(Me.Txt_Dose_Irrigazione.Text) <= 0 Then
                    Messaggi.AgroMsgBox("La Dose deve essere un numero positivo!", Page, , Script_Panel)
                    Exit Function
                Else
                    If InStr(Me.Txt_Dose_Irrigazione.Text, ".") <> 0 Then
                        Txt_Dose_Irrigazione.Text = Replace(Txt_Dose_Irrigazione.Text, ".", ",")
                    End If
                    Dose = CDbl(Me.Txt_Dose_Irrigazione.Text)
                End If
            End If
        End If

        Freatimetro = Cmb_ModifTipoIrrig.SelectedValue

        If Me.Txt_Frequenza_Irrigazione.Text <> "" Then
            If Not IsNumeric(Txt_Frequenza_Irrigazione.Text) Then
                Messaggi.AgroMsgBox("La Frequenza deve essere un numero!", Page, , Script_Panel)
                Exit Function
            Else
                If CDbl(Txt_Frequenza_Irrigazione.Text) <= 0 Then
                    Messaggi.AgroMsgBox("La Frequenza deve essere un numero positivo!", Page, , Script_Panel)
                    Exit Function
                Else
                    If InStr(Txt_Frequenza_Irrigazione.Text, ".") <> 0 Then
                        Txt_Frequenza_Irrigazione.Text = Replace(Txt_Frequenza_Irrigazione.Text, ".", ",")
                    End If
                    Frequenza = CInt(Me.Txt_Frequenza_Irrigazione.Text)
                End If
            End If
        End If

        If Me.Txt_DataInizio_Irrigazione.Text <> "" Then
            Inizio_Micro = CDate(Me.Txt_DataInizio_Irrigazione.Text)
        End If

        If Me.Txt_DataFine_Irrigazione.Text <> "" Then
            Fine_Micro = CDate(Me.Txt_DataFine_Irrigazione.Text)
        End If


        frm_NoteIntervento = Me.Txt_Note_Irrigazione.Text


        strRiepilogo &= " --- " & Dose.ToString & " " & Cmb_Udm_Irrigazione.SelectedItem.Text

        Me.Lbl_Riepilogo.Text = strRiepilogo


        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab

        '----- Operazione

        strOperazione = objXml.XML_Ricetta_Operazione(
                                CInt(enum_TipoOperazioneDB.Scrittura),
                                CStr(Session("ASG_SuperUser_CodFiscale")),
                                frm_BaseCode,
                                frm_TopCode,
                                CInt(Qs_Ricetta_Cod),
                                0,
                                CInt(ComboOperazione.Valore_Combo),
                                CStr(strRiepilogo),
                                CStr(Me.Txt_Note_Irrigazione.Text),
                                , , , ,
                                        Validita_Inizio,
                                        Validita_Fine,
)


        XmlDoc.LoadXml(strOperazione)

        XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")


        '----- DatiRicettaxNote

        XML_DatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

        XML_Operazione.AppendChild(XML_DatiRicettaxNote)

        For i = 0 To Me.CBL_Consigli_Irrigazione.Items.Count - 1
            If CBL_Consigli_Irrigazione.Items(i).Selected Then
                strNote = objXml.XML_RicettaxNote_2(enum_TipoOperazioneDB.Scrittura,
                                                           CStr(Session("ASG_SuperUser_CodFiscale")),
                                                           0,
                                                           0,
                                                           CInt(CBL_Consigli_Irrigazione.Items(i).Value),
                                        Validita_Inizio,
                                        Validita_Fine
                                                           )

                XML_DatiRicettaxNote.InnerXml = XML_DatiRicettaxNote.InnerXml & strNote
            End If
        Next


        '----- DatiRicetta_Dettagli

        XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

        XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

        'Inizializzo
        str_DatiRicettaDettagli = ""

        str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                    CInt(enum_TipoOperazioneDB.Scrittura),
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    CInt(Qs_Ricetta_Cod),
                                    0,
                                    0,
                                    ,
                                    ,
                                    ,
                                    ,
                                    ,
                                    ,
                                    ,
                                        Validita_Inizio,
                                        Validita_Fine,
                                    ,
                                    AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_LAVORAZIONE)


        XmlDoc2.LoadXml(str_RicettaDettaglio)

        XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

        XML_RicettaDettaglio.InnerXml = objXml.XML_Ricetta_DettaglioTecnico_2(
                                        enum_TipoOperazioneDB.Scrittura,
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        CInt(Qs_Ricetta_Cod),
                                        , , , ,
                                        Dose,
                                        Udm_Cod,
                                        , ,
                                        Ore,
                                        Portata,
                                        Frequenza,
                                        Freatimetro,
                                        Inizio_Micro,
                                        Fine_Micro,
                                         ,
                                         ,
                                        , , , , ,
                                        Validita_Inizio,
                                        Validita_Fine,
)



        str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml

        '-------------------
        ' Costi

        SalvaCostiAccessori_Ricetta(str_DatiRicettaDettagliCosti)


        XML_DatiRicettaDettagli.InnerXml = str_DatiRicettaDettagli & str_DatiRicettaDettagliCosti

        objXml = Nothing

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml


    End Function

    '########################################################################################
    Private Function XML_GeneraStringa_Ricetta_Lavorazioni() As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument
        Dim XML_Operazione As XmlElement

        Dim XML_DatiRicettaxNote As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement
        Dim XML_RicettaDettaglio As XmlElement

        Dim str_DatiRicettaDettagli As String
        Dim str_DatiRicettaDettagliCosti As String
        Dim str_RicettaDettaglio As String
        Dim str_RicettaDestinazioni As String
        Dim str_RicettaDestinazione As String

        Dim i, j As Integer

        Dim strOperazione As String
        Dim strNote As String

        Dim frm_VegCod As Integer
        Dim frm_NoteIntervento As String


        Dim Messaggio As String

        Dim Validita_Inizio As Date = #1/1/1900#
        Dim Validita_Fine As Date = #12/31/2100#

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            Validita_Fine = CDate(Txt_DataInizio.Text)
        End If

        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If

        '------------------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------------------

        '------------------------------------------------------------
        '----- Deve essere selezionata la Specie Vegetale

        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggio = "E' necessario selezionare una specie vegetale"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        Else
            frm_VegCod = Me.Cmb_Specie.SelectedItem.Value
        End If


        Dim strRiepilogo As String = ComboOperazione.Testo_Combo


        '---------------------------------------------------
        '-----   NoteIntervento
        '---------------------------------------------------

        frm_NoteIntervento = Me.Txt_Note_Lavorazioni.Text


        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab

        '----- Operazione


        'Inizializzo
        str_DatiRicettaDettagli = ""
        str_DatiRicettaDettagliCosti = ""

        Dim Elem_Cod As Integer = 0
        Dim Pro_Cod As Integer = 0
        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_TRAPIANTO, LAVCOD_SOD_SEDDING
                Elem_Cod = SEMENTI
                Dim objTipologia As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R
                Dim DtTipologia As DataTable
                DtTipologia = objTipologia.Leggi(CInt(Cmb_Specie.SelectedValue), 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)
                If DtTipologia IsNot Nothing AndAlso DtTipologia.Rows.Count > 0 Then
                    Pro_Cod = DtTipologia.Rows(0).Item("sem_cod")
                End If
        End Select

        '----- Ricetta_Dettaglio

        str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                    CInt(enum_TipoOperazioneDB.Scrittura),
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    CInt(Qs_Ricetta_Cod),
                                    0,
                                    0,
                                    0,
                                    Elem_Cod,
                                    Pro_Cod,
                                    0,
                                    0,
                                    0,
                                    0,
                                        Validita_Inizio,
                                        Validita_Fine,
                                    ,
                                    CAU_LAVORAZIONE)

        XmlDoc2.LoadXml(str_RicettaDettaglio)

        XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

        For j = 0 To GridView_Impianti.Rows.Count - 1

            If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then

                str_RicettaDestinazione = objXml.XML_Ricetta_Destinazione(
                                    enum_TipoOperazioneDB.Scrittura,
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    , , , ,
                                    GridView_Impianti.Rows(j).Cells(1).Text,
                                    GridView_Impianti.Rows(j).Cells(2).Text,
                                    GridView_Impianti.Rows(j).Cells(4).Text,
                                    GridView_Impianti.Rows(j).Cells(5).Text,
                                    ,
                                    ,
                                    , )

                str_RicettaDestinazioni &= str_RicettaDestinazione

            End If

        Next

        XML_RicettaDettaglio.InnerXml = str_RicettaDestinazioni

        str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml

        '  str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml

        'str_DatiRicettaDettagli = str_RicettaDettaglio

        Me.Lbl_Riepilogo.Text = strRiepilogo

        '-------------------
        ' Costi

        SalvaCostiAccessori_Ricetta(str_DatiRicettaDettagliCosti)

        '------------------------------------------------
        strOperazione = objXml.XML_Ricetta_Operazione(
                                    CInt(enum_TipoOperazioneDB.Scrittura),
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    frm_BaseCode,
                                    frm_TopCode,
                                    (Qs_Ricetta_Cod),
                                    0,
                                    CInt(ComboOperazione.Valore_Combo),
                                    CStr(strRiepilogo),
                                    CStr(Me.Txt_Note_Lavorazioni.Text),
                                    ,
                                    ,
                                    ,
                                    ,
                                        Validita_Inizio,
                                        Validita_Fine,
                                    0, 0, 0, )

        XmlDoc.LoadXml(strOperazione)

        XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        '----- DatiRicettaxNote

        XML_DatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

        XML_Operazione.AppendChild(XML_DatiRicettaxNote)

        For i = 0 To Me.CBL_Consigli_Lavorazioni.Items.Count - 1
            If CBL_Consigli_Lavorazioni.Items(i).Selected Then
                strNote = objXml.XML_RicettaxNote_2(enum_TipoOperazioneDB.Scrittura,
                                                           CStr(Session("ASG_SuperUser_CodFiscale")),
                                                           0,
                                                           0,
                                                           CInt(CBL_Consigli_Lavorazioni.Items(i).Value),
                                        Validita_Inizio,
                                        Validita_Fine
                                                           )

                XML_DatiRicettaxNote.InnerXml = XML_DatiRicettaxNote.InnerXml & strNote
            End If
        Next


        '----- DatiRicetta_Dettagli

        XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

        XML_DatiRicettaDettagli.InnerXml = str_DatiRicettaDettagli & str_DatiRicettaDettagliCosti

        XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml

    End Function



    Private Sub RigeneraTabellaPerSalvataggio()

        TabellaTrappole = Session("Tabella")

        'reimposto i valori inseriti
        For i = 1 To TabellaTrappole.Rows.Count - 1

            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                Case LAVCOD_FASI_FENOLOGICHE
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                Case Else
                    Throw New NotImplementedException
            End Select


            For j = 2 To TabellaTrappole.Rows(0).Cells.Count - 1
                CType(TabellaTrappole.Rows(i).Cells(j).Controls(0), TextBox).Text =
                    Request.Form(CType(TabellaTrappole.Rows(i).Cells(j).Controls(0), TextBox).ClientID.Replace("_", "$"))

            Next

        Next

        PlaceTabella.Controls.Add(TabellaTrappole)

    End Sub

    '########################################################################################
    Private Function XML_GeneraStringa_Ricetta_RilievoAvvCampo() As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument

        Dim XML_Operazione As XmlElement

        Dim XML_DatiRicettaxNote As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement
        Dim XML_RicettaDettaglio As XmlElement

        Dim str_DatiRicettaDettagli As String
        Dim str_DatiRicettaDettagliTecnico_2 As String
        Dim str_RicettaDettaglioTecnico_2 As String
        Dim str_DatiRicettaDettagliCosti As String
        Dim str_RicettaDettaglio As String

        Dim i As Integer

        Dim strOperazione As String
        Dim strNote As String

        Dim frm_VegCod As Integer

        Dim CauMov As String

        Dim Messaggio As String

        Dim Validita_Inizio As Date = #1/1/1900#
        Dim Validita_Fine As Date = #12/31/2100#

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            Validita_Fine = CDate(Txt_DataInizio.Text)
        End If

        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If

        '------------------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------------------

        '------------------------------------------------------------
        '----- Deve essere selezionata la Specie Vegetale

        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggio = "E' necessario selezionare una specie vegetale"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        Else
            frm_VegCod = Me.Cmb_Specie.SelectedItem.Value
        End If

        Dim strRiepilogo As String = ComboOperazione.Testo_Combo


        '---------------------------------------------------
        'controllo INPUT



        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab
        Dim Mezzo As Integer = 1    'distribuzione HA

        '----- Operazione

        strOperazione = objXml.XML_Ricetta_Operazione(
                                CInt(enum_TipoOperazioneDB.Scrittura),
                                CStr(Session("ASG_SuperUser_CodFiscale")),
                                frm_BaseCode,
                                frm_TopCode,
                                CInt(Qs_Ricetta_Cod),
                                0,
                                CInt(ComboOperazione.Valore_Combo),
                                CStr(strRiepilogo),
                                CStr(Me.Txt_Note_Trappole.Text),
                                , , ,
                                Mezzo,
                                        Validita_Inizio,
                                        Validita_Fine,
)


        XmlDoc.LoadXml(strOperazione)

        XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")


        '----- DatiRicettaxNote

        XML_DatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

        XML_Operazione.AppendChild(XML_DatiRicettaxNote)

        For i = 0 To Me.CBL_Consigli_Trappole.Items.Count - 1
            If CBL_Consigli_Trappole.Items(i).Selected Then
                strNote = objXml.XML_RicettaxNote_2(enum_TipoOperazioneDB.Scrittura,
                                                           CStr(Session("ASG_SuperUser_CodFiscale")),
                                                           0,
                                                           0,
                                                           CInt(CBL_Consigli_Trappole.Items(i).Value),
                                        Validita_Inizio,
                                        Validita_Fine
                                                           )

                XML_DatiRicettaxNote.InnerXml = XML_DatiRicettaxNote.InnerXml & strNote
            End If
        Next


        '----- DatiRicetta_Dettagli

        XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

        XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

        'Inizializzo
        str_DatiRicettaDettagli = ""

        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE
                CauMov = CAU_RILIEVO_CAMPO
            Case Else
                CauMov = CAU_TRATTAMENTO
        End Select

        Dim QuantitaHa As Decimal = CDbl(Txt_NumeroTrappole.Text)


        Dim N_Trappole As Integer = 0
        Dim N_Trappole_Tot As Integer = 0

        RigeneraTabellaPerSalvataggio()

        Dim Ricette_Dettaglio_Tecnico_graphickey As String
        Ricette_Dettaglio_Tecnico_graphickey = objParametriAgenda.Rilievi.FirstOrDefault.mov_destinazioni_graphickey


        For iRiga = 1 To TabellaTrappole.Rows.Count - 1 'scansiono le righe (avversità)
            'Creo il movimento dettaglio e l'unico movimento dettaglio tecnico che sarà presente 


            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO
            '------------------------------------------------

            Const col_udm_cod As Integer = 1
            Const col_qta_ril As Integer = 3
            Const col_av_cod As Integer = 0
            Const col_dett_cod As Integer = 1



            Dim valoreRilevato As String = CType(TabellaTrappole.Rows(iRiga).Cells(col_qta_ril).Controls(0), TextBox).Text
            valoreRilevato = valoreRilevato.Trim() 'rimuovo spazi
            CType(TabellaTrappole.Rows(iRiga).Cells(col_qta_ril).Controls(0), TextBox).Text = valoreRilevato


            If valoreRilevato <> "" Then



                str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                            CInt(enum_TipoOperazioneDB.Scrittura),
                            CStr(Session("ASG_SuperUser_CodFiscale")),
                            Ricetta_Cod:=CInt(Qs_Ricetta_Cod),
                            Ricetta_Operazione_Cod:=0,
                            Ricetta_Dettaglio_Cod:=0,
                            Miscela_Cod:=0,
                            Elem_Cod:=0,
                            Pro_Cod:=0,
                            Mat_Cod:=0,
                            Udm_Cod:=TabellaTrappole.Rows(iRiga).Cells(col_udm_cod).Attributes.Item("Udm_Cod"),
                            Extra_Int:=0,
                            Qta:=1,
                            Validita_Inizio:=Validita_Inizio,
                            Validita_Fine:=Validita_Fine,
                            Cau_Mov:=CauMov
                    )


                str_RicettaDettaglioTecnico_2 = objXml.XML_Ricetta_DettaglioTecnico_2(
                        enum_TipoOperazioneDB.Scrittura,
                        CStr(Session("ASG_SuperUser_CodFiscale")),
                        CInt(Qs_Ricetta_Cod),
                        Ricetta_Operazione_Cod:=0,
                        Ricetta_Dettaglio_Cod:=0,
                        Ricetta_Tecnico_Cod:=0,
                        Miscela_Cod:=0,
                        Qta_Ril:=valoreRilevato,
                        Dett_Cod:=TabellaTrappole.Rows(iRiga).Cells(col_dett_cod).Attributes.Item("Udm_Cod"),
                        Av_Cod:=TabellaTrappole.Rows(iRiga).Cells(col_av_cod).Attributes.Item("Av_Cod"),
                        Freatimetro:=TabellaTrappole.Rows(iRiga).Cells(col_dett_cod).Attributes.Item("Udm_Cod"),
                        FF_Classe:=ddlFF1.selectedValue,
                        Ricette_Dettaglio_Tecnico_graphickey:=Ricette_Dettaglio_Tecnico_graphickey,
                        Piezo1:=ddlFF2.selectedValue,
                        Validita_Inizio:=Validita_Inizio,
                        Validita_Fine:=Validita_Fine
                    )


                str_DatiRicettaDettagliTecnico_2 &= str_RicettaDettaglioTecnico_2

                XmlDoc2.LoadXml(str_RicettaDettaglio)
                XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

                XML_RicettaDettaglio.InnerXml = str_DatiRicettaDettagliTecnico_2

                str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml
            End If
        Next


        '-------------------
        ' Costi

        SalvaCostiAccessori_Ricetta(str_DatiRicettaDettagliCosti)

        XML_DatiRicettaDettagli.InnerXml = str_DatiRicettaDettagli & str_DatiRicettaDettagliCosti

        objXml = Nothing

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml


    End Function

    '########################################################################################
    Private Function XML_GeneraStringa_Ricetta_Trappole() As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument

        Dim XML_Operazione As XmlElement

        Dim XML_DatiRicettaxNote As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement
        Dim XML_RicettaDettaglio As XmlElement

        Dim str_DatiRicettaDettagli As String
        Dim str_DatiRicettaDettagliTecnico_2 As String
        Dim str_RicettaDettaglioTecnico_2 As String
        Dim str_DatiRicettaDettagliCosti As String
        Dim str_RicettaDettaglio As String
        Dim str_RicettaDestinazioni As String
        Dim str_RicettaDestinazione As String

        Dim i, j As Integer

        Dim strOperazione As String
        Dim strNote As String

        Dim frm_VegCod As Integer

        Dim CauMov As String

        Dim frm_Dose As Decimal

        Dim frm_NoteIntervento As String

        Dim Messaggio As String

        Dim Validita_Inizio As Date = #1/1/1900#
        Dim Validita_Fine As Date = #12/31/2100#

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            Validita_Fine = CDate(Txt_DataInizio.Text)
        End If

        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If

        '------------------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------------------

        '------------------------------------------------------------
        '----- Deve essere selezionata la Specie Vegetale

        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggio = "E' necessario selezionare una specie vegetale"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        Else
            frm_VegCod = Me.Cmb_Specie.SelectedItem.Value
        End If

        Dim strRiepilogo As String = ComboOperazione.Testo_Combo

        Dim SuperficieTotaleCentro As Decimal = 0

        For j = 0 To GridView_Impianti.Rows.Count - 1
            If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then
                SuperficieTotaleCentro = SuperficieTotaleCentro + CDbl(GridView_Impianti.Rows(j).Cells(1).Text)
            End If
        Next

        '---------------------------------------------------
        'controllo INPUT

        If Me.cmb_Trappola.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox("Selezionare la Trappola!", Page, , Script_Panel)
            Exit Function
        End If

        If Me.cmb_Avversita.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox("Indicare l'Avversita'!", Page, , Script_Panel)
            Exit Function
        End If

        If Me.Txt_NumeroTrappole.Text = "" Then
            Messaggi.AgroMsgBox("Indicare il Numero di Trappole!", Page, , Script_Panel)
            Exit Function
        Else
            If Not IsNumeric(Me.Txt_NumeroTrappole.Text) Then
                Messaggi.AgroMsgBox("Il Numero di Trappole deve essere un numero!", Page, , Script_Panel)
                Exit Function
            Else
                If CDbl(Me.Txt_NumeroTrappole.Text) <= 0 Then
                    Messaggi.AgroMsgBox("Il Numero di Trappole deve essere un numero positivo!", Page, , Script_Panel)
                    Exit Function
                Else
                    If InStr(Me.Txt_NumeroTrappole.Text, ".") <> 0 Then
                        Txt_NumeroTrappole.Text = Replace(Txt_NumeroTrappole.Text, ".", ",")
                    End If
                    frm_Dose = Me.Txt_NumeroTrappole.Text
                End If
            End If
        End If


        If cmb_Trappola.SelectedItem.Text <> "" Then
            Me.Lbl_Riepilogo.Text = strRiepilogo & " --- " & Me.cmb_Trappola.SelectedItem.Text & " (" & Me.cmb_Avversita.SelectedItem.Text & ")"
        Else
            Me.Lbl_Riepilogo.Text = strRiepilogo
        End If

        frm_NoteIntervento = Me.Txt_Note_Trappole.Text

        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab
        Dim Mezzo As Integer = 1    'distribuzione HA
        Dim Sup_Imp As Decimal
        Dim QuantitaTotale As Decimal = 0
        Dim QuantitaImpianto As Decimal = 0
        Dim QuantitaTotaleImpianti As Decimal = 0

        '----- Operazione

        strOperazione = objXml.XML_Ricetta_Operazione(
                                CInt(enum_TipoOperazioneDB.Scrittura),
                                CStr(Session("ASG_SuperUser_CodFiscale")),
                                frm_BaseCode,
                                frm_TopCode,
                                CInt(Qs_Ricetta_Cod),
                                0,
                                CInt(ComboOperazione.Valore_Combo),
                                CStr(strRiepilogo),
                                CStr(Me.Txt_Note_Trappole.Text),
                                , , ,
                                Mezzo,
                                        Validita_Inizio,
                                        Validita_Fine,
)


        XmlDoc.LoadXml(strOperazione)

        XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")


        '----- DatiRicettaxNote

        XML_DatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

        XML_Operazione.AppendChild(XML_DatiRicettaxNote)

        For i = 0 To Me.CBL_Consigli_Trappole.Items.Count - 1
            If CBL_Consigli_Trappole.Items(i).Selected Then
                strNote = objXml.XML_RicettaxNote_2(enum_TipoOperazioneDB.Scrittura,
                                                           CStr(Session("ASG_SuperUser_CodFiscale")),
                                                           0,
                                                           0,
                                                           CInt(CBL_Consigli_Trappole.Items(i).Value),
                                        Validita_Inizio,
                                        Validita_Fine
                                                           )

                XML_DatiRicettaxNote.InnerXml = XML_DatiRicettaxNote.InnerXml & strNote
            End If
        Next


        '----- DatiRicetta_Dettagli

        XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

        XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

        'Inizializzo
        str_DatiRicettaDettagli = ""

        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE
                CauMov = CAU_RILIEVO_CAMPO
            Case Else
                CauMov = CAU_TRATTAMENTO
        End Select

        Dim QuantitaHa As Decimal = CDbl(Txt_NumeroTrappole.Text)

        QuantitaTotale = Math.Round(QuantitaHa * SuperficieTotaleCentro, 0)

        Dim N_Trappole As Integer = 0
        Dim N_Trappole_Tot As Integer = 0

        Select Case CInt(ComboOperazione.Valore_Combo)

            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                If GridView_Impianti.Rows.Count > 0 Then

                    'N dettagli (1 dettaglio x ciascun impianto) che contengono ciascuno 
                    '1 destinazione 1 + N dettagli tecnici (quante sono realmente le trappole che finiscono su quell'impianto)

                    For j = 0 To GridView_Impianti.Rows.Count - 1

                        If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then

                            '------------------------------------------------
                            '----- MOVIMENTO DETTAGLIO
                            '------------------------------------------------

                            str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                      CInt(enum_TipoOperazioneDB.Scrittura),
                                      CStr(Session("ASG_SuperUser_CodFiscale")),
                                      CInt(Qs_Ricetta_Cod),
                                      0,
                                      0,
                                      ,
                                      TRAPPOLE,
                                      CInt(Me.cmb_Trappola.SelectedItem.Value),
                                      ,
                                      enum_UnitaMisura.Numero_Trappole,
                                      ,
                                      QuantitaTotale,
                                        Validita_Inizio,
                                        Validita_Fine,
                                      ,
                                      CauMov)

                            '------------------------------------------------
                            '----- MOVIMENTO DESTINAZIONE
                            '------------------------------------------------

                            Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
                            QuantitaImpianto = Math.Round(QuantitaHa * Sup_Imp, 0)

                            'N.B. Per l'ultimo appezzamento verifico quante trappole ho rimasto da distribuire
                            If j = GridView_Impianti.Rows.Count - 1 Then
                                QuantitaImpianto = QuantitaTotale - QuantitaTotaleImpianti
                            End If

                            QuantitaTotaleImpianti += QuantitaImpianto

                            str_RicettaDestinazione = objXml.XML_Ricetta_Destinazione(
                                                   enum_TipoOperazioneDB.Scrittura,
                                                   CStr(Session("ASG_SuperUser_CodFiscale")),
                                                   , , , ,
                                                   GridView_Impianti.Rows(j).Cells(1).Text,
                                                   GridView_Impianti.Rows(j).Cells(2).Text,
                                                   GridView_Impianti.Rows(j).Cells(4).Text,
                                                   GridView_Impianti.Rows(j).Cells(5).Text,
                                                   ,
                                                   QuantitaImpianto,
                                                   , )

                            '------------------------------------------------
                            '----- MOVIMENTO DETTAGLIO TECNICO
                            '------------------------------------------------
                            str_DatiRicettaDettagliTecnico_2 = ""

                            For N_Trappole = 1 To QuantitaImpianto

                                str_RicettaDettaglioTecnico_2 = objXml.XML_Ricetta_DettaglioTecnico_2(
                                            enum_TipoOperazioneDB.Scrittura,
                                            CStr(Session("ASG_SuperUser_CodFiscale")),
                                            CInt(Qs_Ricetta_Cod),
                                            , , , ,
                                            ,
                                            ,
                                            CInt(Me.cmb_Avversita.SelectedItem.Value),
                                            ,
                                            1,
                                            ,
                                            ,
                                            ,
                                            CDate(Me.Txt_DataInizio.Text),
                                             , , ,
                                            CInt(Me.cmb_Ditte.SelectedItem.Value),
                                            CStr(Me.Txt_CodAvversita.Text),
                                            N_Trappole_Tot + 1,
                                            , ,
                                        Validita_Inizio,
                                        Validita_Fine,
)

                                N_Trappole_Tot += 1

                                str_DatiRicettaDettagliTecnico_2 &= str_RicettaDettaglioTecnico_2

                            Next

                            XmlDoc2.LoadXml(str_RicettaDettaglio)

                            XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

                            XML_RicettaDettaglio.InnerXml = str_RicettaDestinazioni & str_DatiRicettaDettagliTecnico_2

                            str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml

                        End If

                    Next
                Else

                End If


            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                '1 dettaglio che contiene 1 dettaglio tecnico + N destinazioni

                str_DatiRicettaDettagliTecnico_2 = objXml.XML_Ricetta_DettaglioTecnico_2(
                                        enum_TipoOperazioneDB.Scrittura,
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        CInt(Qs_Ricetta_Cod),
                                        , , , ,
                                        ,
                                        ,
                                        CInt(Me.cmb_Avversita.SelectedItem.Value),
                                        ,
                                        ,
                                        ,
                                        ,
                                        ,
                                        , , , ,
                                        CInt(Me.cmb_Ditte.SelectedItem.Value),
                                        CStr(Me.Txt_CodAvversita.Text),
                                        , , ,
                                        Validita_Inizio,
                                        Validita_Fine,
)

                For j = 0 To GridView_Impianti.Rows.Count - 1

                    If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then

                        Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)

                        QuantitaImpianto = Math.Round(QuantitaHa * Sup_Imp, 0)

                        QuantitaTotale += QuantitaImpianto

                        str_RicettaDestinazione = objXml.XML_Ricetta_Destinazione(
                                            enum_TipoOperazioneDB.Scrittura,
                                            CStr(Session("ASG_SuperUser_CodFiscale")),
                                            , , , ,
                                            GridView_Impianti.Rows(j).Cells(1).Text,
                                            GridView_Impianti.Rows(j).Cells(2).Text,
                                            GridView_Impianti.Rows(j).Cells(4).Text,
                                            GridView_Impianti.Rows(j).Cells(5).Text,
                                            ,
                                            QuantitaImpianto,
                                            , )

                        str_RicettaDestinazioni &= str_RicettaDestinazione

                    End If

                Next

                str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                 CInt(enum_TipoOperazioneDB.Scrittura),
                                 CStr(Session("ASG_SuperUser_CodFiscale")),
                                 CInt(Qs_Ricetta_Cod),
                                 0,
                                 0,
                                 ,
                                 TRAPPOLE,
                                 CInt(Me.cmb_Trappola.SelectedItem.Value),
                                 ,
                                 11,
                                 ,
                                 QuantitaTotale,
                                        Validita_Inizio,
                                        Validita_Fine,
                                 ,
                                 CauMov)

                XmlDoc2.LoadXml(str_RicettaDettaglio)

                XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

                XML_RicettaDettaglio.InnerXml = str_DatiRicettaDettagliTecnico_2 & str_RicettaDestinazioni

                str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml

        End Select



        '-------------------
        ' Costi

        SalvaCostiAccessori_Ricetta(str_DatiRicettaDettagliCosti)

        XML_DatiRicettaDettagli.InnerXml = str_DatiRicettaDettagli & str_DatiRicettaDettagliCosti

        objXml = Nothing

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml


    End Function

    '########################################################################################
    Private Function XML_GeneraStringa_Ricetta_Trattamento() As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlDoc2 As New XmlDocument

        Dim XML_Operazione As XmlElement

        Dim XML_DatiRicettaxNote As XmlElement
        Dim XML_DatiRicettaDettagliTecnici As XmlElement
        Dim XML_DatiRicettaDettagli As XmlElement
        Dim XML_RicettaDettaglio As XmlElement

        Dim str_DatiRicettaDettagli As String
        Dim str_DatiRicettaDettagliTecnico_2 As String
        Dim str_DatiRicettaDettagliCosti As String
        Dim str_RicettaDettaglio As String
        Dim str_RicettaDestinazioni As String
        Dim str_RicettaDestinazione As String

        Dim i, j As Integer

        Dim strOperazione As String
        Dim strNote As String
        Dim strForm As String = ""
        Dim strAvversita As String

        Dim Array() As String

        Dim frm_VegCod As Integer
        Dim frm_CodDisciplinare As Integer = 0
        Dim frm_DisciplinarePP As Integer = 0
        Dim frm_IdRcdpi As Integer = 0
        Dim frm_Modulo As Integer = 0
        Dim frm_Epoca As Integer = 0
        Dim frm_NoteIntervento As String

        Dim Acqua As Decimal = 0
        Dim AcquaTot As Decimal = 0

        Dim Mezzo As Integer

        Dim Messaggio As String

        Dim Validita_Inizio As Date = #1/1/1900#
        Dim Validita_Fine As Date = #12/31/2100#

        If Me.Txt_DataInizio.Text <> "" Then
            Validita_Inizio = CDate(Txt_DataInizio.Text)
            Validita_Fine = CDate(Txt_DataInizio.Text)
        End If

        If Me.Txt_DataFine.Text <> "" Then
            Validita_Fine = CDate(Txt_DataFine.Text)
        End If
        '------------------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------------------

        '------------------------------------------------------------
        '----- Deve essere selezionata la Specie Vegetale

        If Me.Cmb_Specie.SelectedItem.Text = "" Then
            Messaggio = "E' necessario selezionare una specie vegetale"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        Else
            frm_VegCod = Me.Cmb_Specie.SelectedItem.Value
        End If

        Dim Dt As DataTable = ViewState("dtDosi_Difesa")

        If Dt.Rows.Count = 0 Then
            Messaggio = "E' necessario selezionare almeno un Formulato!"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Function
        End If


        Dim strRiepilogo As String = ComboOperazione.Testo_Combo

        Dim SuperficieTotaleCentro As Decimal = 0

        For j = 0 To GridView_Impianti.Rows.Count - 1
            If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then
                SuperficieTotaleCentro = SuperficieTotaleCentro + CDbl(GridView_Impianti.Rows(j).Cells(1).Text)
            End If
        Next

        '---------------------------------------------------
        '----- DPICod + modulo + epoca
        '---------------------------------------------------
        frm_Modulo = 0
        frm_Epoca = 0

        If Cmb_Disciplinare.SelectedValue <> "" Then
            Array = Split(Cmb_Disciplinare.SelectedValue, "/")
            frm_CodDisciplinare = CInt(Array(0))
            If frm_CodDisciplinare <> 0 Then
                frm_IdRcdpi = CInt(Array(1))
                frm_DisciplinarePP = CInt(Array(4))
            Else
                frm_CodDisciplinare = -1
            End If

            If Me.Cmb_Disciplinare.SelectedValue <> "0" AndAlso Me.Cmb_Epoca.Items.Count > 0 Then
                frm_Modulo = CInt(Me.Cmb_Epoca.SelectedValue)
            End If
        End If

        '---------------------------------------------------
        '----- Acqua  +  NoteIntervento
        '---------------------------------------------------

        'se è stata scelta la dose d'acqua/ha salvo il dato negativo

        Acqua = 0
        AcquaTot = 0

        If CInt(ComboOperazione.Valore_Combo) <> LAVCOD_GEODISINFESTAZIONE Then
            If Me.Txt_Acqua_Difesa.Text <> "" Then

                If Not IsNumeric(Me.Txt_Acqua_Difesa.Text) Then
                    Messaggio = " E' necessario indicare " & Chr(13) &
                                " gli ettolitri di acqua " & Chr(13) &
                                " con un valore numerico "
                    Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                    Exit Function
                Else
                    If CDbl(Me.Txt_Acqua_Difesa.Text) <= 0 Then
                        Messaggio = " E' necessario indicare " & Chr(13) &
                                    " gli ettolitri di acqua " & Chr(13) &
                                    " con un valore numerico positivo "
                        Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                        Exit Function
                    Else
                        If InStr(Txt_Acqua_Difesa.Text, ".") <> 0 Then
                            Txt_Acqua_Difesa.Text = Replace(Txt_Acqua_Difesa.Text, ".", ",")
                        End If
                    End If

                End If
                Select Case RBL_Acqua_Formulati.SelectedValue
                    Case "0"
                        Acqua = CDbl(Me.Txt_Acqua_Difesa.Text)
                        AcquaTot = Acqua
                    Case "1"
                        Acqua = -CDbl(Me.Txt_Acqua_Difesa.Text)
                        AcquaTot = Math.Abs(Acqua) * SuperficieTotaleCentro
                End Select

            Else

                Messaggio = " E' necessario indicare " & Chr(13) &
                                   " gli ettolitri di acqua "
                Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
                Exit Function
                'Me.Txt_Acqua_Difesa.Text = "0"

            End If

        End If

        frm_NoteIntervento = Me.Txt_Note_Difesa.Text

        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab


        '----- Operazione

        Select Case Me.RBL_Dose_Formulati.SelectedValue
            Case "1"
                Mezzo = 1
            Case "0"
                Mezzo = 0
            Case "10"
                Mezzo = 0
        End Select

        'Inizializzo
        str_DatiRicettaDettagli = ""
        str_DatiRicettaDettagliCosti = ""
        str_DatiRicettaDettagli = ""
        str_RicettaDestinazioni = ""

        Dim Dose As Decimal
        Dim DoseTrasformata As Decimal
        Dim Udm_Cod, Udm_Cod_Trasf As Integer
        Dim strAv_Cod As String
        Dim strAv_Gru As String
        Dim strSoglia_Value As String = ""
        Dim strSoglia_Des As String = ""
        Dim TempoCarenza As Integer = 0
        Dim DoseEtichetta As String = ""
        Dim PrincipiAttivi As String = ""
        Dim ClassiTossicologiche As String = ""
        Dim Sup_Imp As Decimal = 0
        Dim Frazione As Decimal = 0
        Dim QuantitaTotale As Decimal = 0
        Dim QuantitaImpianto As Decimal = 0

        'Per ciascuna dose impostata ...
        For i = 0 To GridView_Dosi_Difesa.Rows.Count - 1

            str_DatiRicettaDettagliTecnico_2 = ""
            str_RicettaDestinazioni = ""

            strAvversita = GridView_Dosi_Difesa.Rows(i).Cells(4).Text
            strForm = GridView_Dosi_Difesa.Rows(i).Cells(8).Text

            strAv_Cod = GridView_Dosi_Difesa.Rows(i).Cells(2).Text
            strAv_Gru = GridView_Dosi_Difesa.Rows(i).Cells(3).Text

            strSoglia_Value = ""
            strSoglia_Des = ""

            If GridView_Dosi_Difesa.Rows(i).Cells(5).Text <> "&nbsp;" Then
                strSoglia_Value = GridView_Dosi_Difesa.Rows(i).Cells(5).Text
                strSoglia_Des = GridView_Dosi_Difesa.Rows(i).Cells(6).Text
            End If

            Dose = CDbl(GridView_Dosi_Difesa.Rows(i).Cells(14).Text)
            Udm_Cod = CInt(GridView_Dosi_Difesa.Rows(i).Cells(13).Text)

            Select Case Udm_Cod
                Case 3  'g
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose / 1000  'caso in cui ho i grammi
                Case 2032 'mg
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose / 1000000  'caso in cui ho i grammi
                Case 4  'q
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose * 100  'caso in cui ho i quintali
                Case 304 't
                    Udm_Cod_Trasf = 2
                    DoseTrasformata = Dose * 1000   'caso in cui ho le tonnelate
                Case 101 'ml
                    Udm_Cod_Trasf = 29
                    DoseTrasformata = Dose / 1000  'caso in cui ho i ml
                Case 104 'cc
                    Udm_Cod_Trasf = 29
                    DoseTrasformata = Dose / 100  'caso in cui ho i cc
                Case 2, 29 'kg,l
                    Udm_Cod_Trasf = Udm_Cod
                    DoseTrasformata = Dose
            End Select

            TempoCarenza = CInt(GridView_Dosi_Difesa.Rows(i).Cells(9).Text)
            DoseEtichetta = CStr(GridView_Dosi_Difesa.Rows(i).Cells(10).Text)
            PrincipiAttivi = CStr(GridView_Dosi_Difesa.Rows(i).Cells(17).Text)
            ClassiTossicologiche = CStr(GridView_Dosi_Difesa.Rows(i).Cells(18).Text)

            str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                        CInt(enum_TipoOperazioneDB.Scrittura),
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        CInt(Qs_Ricetta_Cod),
                                        0,
                                        0,
                                        0,
                                        191,
                                        CInt(GridView_Dosi_Difesa.Rows(i).Cells(7).Text),
                                        0,
                                        Udm_Cod_Trasf,
                                        Udm_Cod,
                                        Dose,
                                        Validita_Inizio,
                                        Validita_Fine,
                                        ,
                                        CAU_TRATTAMENTO,
                                        TempoCarenza,
                                        DoseEtichetta,
                                        PrincipiAttivi,
                                        ClassiTossicologiche)

            XmlDoc2.LoadXml(str_RicettaDettaglio)

            XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

            If Not (strAv_Cod = "0" AndAlso strAv_Gru = "0") Then
                str_DatiRicettaDettagliTecnico_2 = XML_GeneraBlocco_RicettaDettaglioTecnico_2_DPI(strAv_Cod, strAv_Gru, strSoglia_Value, strSoglia_Des)
            End If

            For j = 0 To GridView_Impianti.Rows.Count - 1

                If CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then

                    Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)

                    '0=HL 1=HA
                    If Mezzo = 0 Then
                        QuantitaTotale = DoseTrasformata * AcquaTot
                    Else
                        QuantitaTotale = DoseTrasformata * SuperficieTotaleCentro
                    End If

                    Frazione = Sup_Imp / SuperficieTotaleCentro

                    QuantitaImpianto = QuantitaTotale * Frazione

                    str_RicettaDestinazione = objXml.XML_Ricetta_Destinazione(
                                        enum_TipoOperazioneDB.Scrittura,
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        , , , ,
                                        GridView_Impianti.Rows(j).Cells(1).Text,
                                        GridView_Impianti.Rows(j).Cells(2).Text,
                                        GridView_Impianti.Rows(j).Cells(4).Text,
                                        GridView_Impianti.Rows(j).Cells(5).Text,
                                        ,
                                        QuantitaImpianto,
                                        , )

                    str_RicettaDestinazioni &= str_RicettaDestinazione

                End If

            Next

            XML_RicettaDettaglio.InnerXml = str_DatiRicettaDettagliTecnico_2 & str_RicettaDestinazioni

            str_DatiRicettaDettagli = str_DatiRicettaDettagli & XmlDoc2.OuterXml

            strRiepilogo &= " --- " & strAvversita & " --- " & strForm

        Next

        Me.Lbl_Riepilogo.Text = strRiepilogo


        '-------------------
        ' Costi

        SalvaCostiAccessori_Ricetta(str_DatiRicettaDettagliCosti)

        '------------------------------------------------
        strOperazione = objXml.XML_Ricetta_Operazione(
                                    CInt(enum_TipoOperazioneDB.Scrittura),
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    frm_BaseCode,
                                    frm_TopCode,
                                    (Qs_Ricetta_Cod),
                                    0,
                                    CInt(ComboOperazione.Valore_Combo),
                                    CStr(strRiepilogo),
                                    CStr(Me.Txt_Note_Difesa.Text),
                                    CInt(frm_CodDisciplinare),
                                    CInt(frm_IdRcdpi),
                                    CInt(frm_Modulo),
                                    CInt(Mezzo),
                                        Validita_Inizio,
                                        Validita_Fine,
                                    0, 0, 0,
                                    frm_DisciplinarePP)


        XmlDoc.LoadXml(strOperazione)

        XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")


        '----- DatiRicettaxNote

        XML_DatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote_2")

        XML_Operazione.AppendChild(XML_DatiRicettaxNote)

        For i = 0 To Me.CBL_Consigli_Difesa.Items.Count - 1
            If CBL_Consigli_Difesa.Items(i).Selected Then
                strNote = objXml.XML_RicettaxNote_2(enum_TipoOperazioneDB.Scrittura,
                                                           CStr(Session("ASG_SuperUser_CodFiscale")),
                                                           0,
                                                           0,
                                                           CInt(CBL_Consigli_Difesa.Items(i).Value),
                                        Validita_Inizio,
                                        Validita_Fine
                                                           )

                XML_DatiRicettaxNote.InnerXml = XML_DatiRicettaxNote.InnerXml & strNote
            End If
        Next


        '----- DatiRicetta_Dettagli_Tecnici
        XML_DatiRicettaDettagliTecnici = XmlDoc.CreateElement("DatiRicetta_Dettagli_Tecnici")

        XML_DatiRicettaDettagliTecnici.InnerXml = XML_GeneraBlocco_RicettaDettaglioTecnico_DPI(Acqua)

        XML_Operazione.AppendChild(XML_DatiRicettaDettagliTecnici)

        '----- DatiRicetta_Dettagli

        XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

        XML_DatiRicettaDettagli.InnerXml = str_DatiRicettaDettagli & str_DatiRicettaDettagliCosti

        XML_Operazione.AppendChild(XML_DatiRicettaDettagli)



        '----- Restituisco il risultato

        Return XmlDoc.OuterXml


    End Function

    Private Sub SalvaCostiAccessori_Ricetta(ByRef str_DatiRicettaDettagli As String)

        Dim DT As DataTable

        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then

            Dim i, j As Integer
            Dim str_RicettaDettaglio As String

            Dim objXml As New AgronicaCoreXML.XML_Contab

            DT = CType(Session("dtScarico"), DataTable)

            'devo fare un movimento solo per ogni causale (al max 4)
            For i = -1 To -5 Step -1
                'filtro solo i movimenti di tipo ...
                Dim DR() As DataRow
                If i = -5 Then
                    DR = DT.Select("Centro_cod not in (-1,-2,-3,-4)")
                Else
                    DR = DT.Select("Centro_cod = " & i)
                End If
                If DR.Length > 0 Then
                    'ho qualche movimento da inserire 
                    Dim Cau_Mov As Integer
                    Select Case i
                        Case -1
                            Cau_Mov = CAU_IMPUTAZIONE_PARCOMACCHINE
                        Case -2
                            Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                        Case -3
                            Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
                        Case -4
                            Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                        Case Else
                            Cau_Mov = CAU_SCARICO
                    End Select

                    For j = 0 To DR.Length - 1

                        'creo un nuovo dettaglio 
                        str_RicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
                                CInt(enum_TipoOperazioneDB.Scrittura),
                                CStr(Session("ASG_SuperUser_CodFiscale")),
                                CInt(Qs_Ricetta_Cod),
                                0,
                                0,
                                0,
                                DR(j).Item("Elem_Cod"),
                                0,
                                DR(j).Item("Mat_cod"),
                                0,
                                DR(j).Item("Udm_Cod"),
                                0,
                                CDate(Me.Txt_DataInizio.Text),
                                CDate(Me.Txt_DataFine.Text),
                                ,
                                Cau_Mov)

                        str_DatiRicettaDettagli = str_DatiRicettaDettagli & str_RicettaDettaglio

                    Next

                End If
            Next

        End If

        Session("dtScarico") = Nothing
        AggiornaDgrScarico()
        AggiornaGridViewCostiAccessoriVisibili()

    End Sub

    '########################################################################################
    Private Function XML_GeneraBlocco_RicettaDettaglioTecnico_DPI(ByVal frm_Acqua As Decimal) As String

        'NOTA : Esiste SOLO il nodo per l'acqua

        Dim StringoneXML As String = ""
        Dim StringaXML As String

        Dim objXml As New AgronicaCoreXML.XML_Contab

        StringaXML = objXml.XML_Ricetta_DettaglioTecnico(
                                enum_TipoOperazioneDB.Scrittura,
                                CStr(Session("ASG_SuperUser_CodFiscale")),
                                , , , , ,
                                frm_Acqua, ,
                                0,
                                0,
                                , , , , , , , , , , , , , , )

        StringoneXML = StringoneXML & StringaXML

        objXml = Nothing

        'Restituisco il risultato
        Return StringoneXML

    End Function

    '########################################################################################
    'Crea Nodo Disciplinari MovimentoDettaglioTecnico_2 (utilizzato nel Discilpinare)
    '########################################################################################
    Private Function XML_GeneraBlocco_RicettaDettaglioTecnico_2_DPI(ByVal str_AvCod As String,
                                                              ByVal str_AvGru As String,
                                                              ByVal strSoglia_Value As String,
                                                              ByVal strSoglia_Des As String) _
                                                              As String

        Dim StringoneXML As String = ""
        Dim StringaXML As String
        Dim Array_AvCod() As String
        Dim Array_AvGru() As String
        Dim Array_SogliaDes() As String
        Dim Array_SogliaValue() As String
        Dim i As Integer
        Dim Soglia_Cod As Integer = 0
        Dim Soglia_Qta As Decimal = 0
        Dim Soglia_Des As String = ""

        Dim objXml As New AgronicaCoreXML.XML_Contab

        Array_AvCod = Split(str_AvCod, ",")
        Array_AvGru = Split(str_AvGru, ",")
        Array_SogliaDes = Split(strSoglia_Des, ",")
        Array_SogliaValue = Split(strSoglia_Value, ",")

        For i = 0 To UBound(Array_AvCod)

            If strSoglia_Des <> "" Then
                Soglia_Des = Array_SogliaDes(i)
                Soglia_Cod = Split(Array_SogliaValue(i), "_")(2)
                If IsNumeric(Split(Array_SogliaValue(i), "_")(1)) Then
                    Soglia_Qta = Split(Array_SogliaValue(i), "_")(1)
                Else
                    Soglia_Qta = 0
                End If
            Else
                Soglia_Des = ""
                Soglia_Cod = 0
                Soglia_Qta = 0
            End If

            StringaXML = objXml.XML_Ricetta_DettaglioTecnico_2(
                                        enum_TipoOperazioneDB.Scrittura,
                                        CStr(Session("ASG_SuperUser_CodFiscale")),
                                        , , , , , , ,
                                        CInt(Array_AvCod(i)),
                                        CInt(Array_AvGru(i)),
                                        , , , , , , , , , , , , , , , , , , ,
                                        Soglia_Cod,
                                        Soglia_Des,
                                        Soglia_Qta,
)

            StringoneXML = StringoneXML & StringaXML

        Next

        objXml = Nothing

        'Restituisco il risultato
        Return StringoneXML

    End Function

    '#####################################################################################################
    Private Sub Crea_Ricetta_da_Interventi(ByVal strFiltroAgenda As String)

        Dim i As Integer
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Id_Agenda As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_FiltroAgenda As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim Xml_VariabiliStampe As System.Xml.XmlElement

        Dim strAgenda As String = ""
        Dim XmlConsiglio As String
        Dim Lav_Cod As Integer
        Dim Des_Lib As String
        Dim Data As Date
        Dim Note As String

        Dim objAgenda_R As New AgronicaCoreContabBIZ.Agenda_R
        Dim objRicetteOpR As New AgronicaCoreContabBIZ.Ricette_Operazioni_R

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(strFiltroAgenda)

        Xml_FiltroAgenda = XmlDoc.SelectSingleNode("FiltroAgenda")

        XMLs_VariabiliStampe = Xml_FiltroAgenda.GetElementsByTagName("VariabiliStampe")

        For i = 0 To XMLs_VariabiliStampe.Count - 1

            Xml_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

            If Xml_VariabiliStampe.HasAttribute("id_agenda") AndAlso IsNumeric(Xml_VariabiliStampe.GetAttribute("id_agenda")) Then

                Piva = Xml_VariabiliStampe.GetAttribute("piva")
                Sa_Cod = CInt(Xml_VariabiliStampe.GetAttribute("sa_cod"))
                Id_Agenda = CInt(Xml_VariabiliStampe.GetAttribute("id_agenda"))

                ViewState("Piva") = Piva
                ViewState("Sa_Cod") = Sa_Cod


                'If Me.Txt_RagioneSociale.Text = "" Then
                '    Dim ObjImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
                '    Me.Txt_RagioneSociale.Text = ObjImpresa.RagSoc_from_Piva(Piva, objParametri_Server)
                '    ObjImpresa = Nothing
                'End If

                strAgenda = objAgenda_R.Agenda_Leggi(Piva,
                                                    Sa_Cod,
                                                    Id_Agenda,
                                                    0,
                                                    False,
                                                    objParametri_Server)

                If strAgenda <> "" Then


                    XmlConsiglio = objRicetteOpR.XML_GeneraStringa_Ricetta_Operazione(Qs_Ricetta_Cod, Qs_Tipo_Ricetta,
                                                                                      objParametri_Server.PivaSuperUser, Session("ASG_ProgressivoGIAS"),
                                                                        strAgenda,
                                                                        Lav_Cod, Des_Lib, Data, Note, objParametri_Server)

                    Inserisci_Operazione(0,
                                         Des_Lib,
                                         Data.ToShortDateString,
                                         Lav_Cod,
                                         XmlConsiglio,
                                         Id_Agenda)

                End If

            End If

        Next

        objAgenda_R = Nothing

    End Sub

    '########################################################################################
    'Private Function XML_GeneraStringa_Ricetta_Operazione(ByVal strAgenda As String, _
    '                                                        ByRef Lav_Cod As Integer, _
    '                                                        ByRef Des_Lib As String, _
    '                                                        ByRef Data As Date, _
    '                                                        ByRef Note As String) As String

    '    '------------------------------------------------
    '    '----- Definizione delle Variabili
    '    '------------------------------------------------

    '    Dim XmlDocAgenda As New System.Xml.XmlDocument
    '    Dim XML_DatiAgenda As System.Xml.XmlElement
    '    Dim XML_Agenda As System.Xml.XmlElement
    '    Dim XML_DatiMovimenti As System.Xml.XmlElement
    '    Dim XML_Movimento As System.Xml.XmlElement
    '    Dim XMLs_Movimento As System.Xml.XmlNodeList
    '    Dim XML_DatiMovDettagliTecnici As System.Xml.XmlElement
    '    Dim XML_MovimentoDettaglioTecnico As System.Xml.XmlElement
    '    Dim XMLs_MovimentoDettaglioTecnico As System.Xml.XmlNodeList
    '    Dim XML_MovimentoDettaglioTecnico_2 As System.Xml.XmlElement
    '    Dim XMLs_Movimento_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList
    '    Dim XML_DatiMovimentiDettagli As System.Xml.XmlElement
    '    Dim XML_MovimentoDettaglio As System.Xml.XmlElement
    '    Dim XMLs_MovimentoDettaglio As System.Xml.XmlNodeList
    '    Dim XML_MovimentoDestinazione As System.Xml.XmlElement
    '    Dim XMLs_MovimentoDestinazione As System.Xml.XmlNodeList

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XmlDoc2 As New System.Xml.XmlDocument
    '    Dim XmlDoc3 As New System.Xml.XmlDocument

    '    Dim XML_Operazione As System.Xml.XmlElement
    '    Dim XML_DatiRicettaxAgenda As System.Xml.XmlElement
    '    Dim XML_RicettaxAgenda As System.Xml.XmlElement
    '    Dim XML_DatiRicettaDettagliTecnici As System.Xml.XmlElement
    '    Dim XML_RicettaDettaglioTecnico As System.Xml.XmlElement
    '    Dim XML_DatiRicettaDettagli As System.Xml.XmlElement
    '    Dim XML_RicettaDettaglio As System.Xml.XmlElement
    '    Dim XML_RicettaDestinazione As System.Xml.XmlElement

    '    Dim strOperazione As String = ""
    '    Dim strRicettaxAgenda As String = ""
    '    Dim strRicettaDettagli As String = ""
    '    Dim strRicettaDettaglio As String = ""
    '    Dim strRicettaDettagliTecnici As String = ""
    '    Dim strRicettaDettaglioTecnico As String = ""
    '    Dim strRicettaDestinazioni As String = ""
    '    Dim strRicettaDestinazione As String = ""
    '    Dim strRicettaDettagliCostiAccessori As String = ""
    '    Dim strRicettaDettaglioCostiAccessori As String = ""
    '    Dim strRicettaDestinazioniCostiAccessori As String = ""
    '    Dim strRicettaDestinazioneCostiAccessori As String = ""

    '    Dim ArrayPiva(0) As String
    '    Dim ArraySaCod(0) As Integer
    '    Dim ArrayAppezza(0) As Integer
    '    Dim ArrayIdReg(0) As Integer
    '    Dim N_Array As Integer = 0
    '    Dim N_Cul_Cod As Integer = 0

    '    Dim i, j, n, c As Integer

    '    Dim Id_Agenda As Integer
    '    Dim Cau_Mov As String
    '    Dim CodDisciplinare As Integer
    '    Dim Extra_Int As Integer
    '    Dim Mezzo As Integer
    '    Dim DisciplinarePP As Integer = 0
    '    Dim Validita_Fine As Date = #12/31/2100#

    '    Dim TempoCarenza As Integer
    '    Dim DoseEtichetta As String
    '    Dim DoseEtichetta_Value As String
    '    Dim PrincipiAttivi As String
    '    Dim ClassiTossicologiche As String

    '    Dim Messaggio As String

    '    Dim objXml As New AgronicaCoreXML.XML_Contab
    '    'Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

    '    Dim Dt_Impianti As New DataTable

    '    'If Not Cul_Cod Is Nothing Then
    '    '    N_Cul_Cod = UBound(Cul_Cod)
    '    'End If

    '    '------------------------------------------------
    '    '----- Recupero i dati dalla stringa
    '    '------------------------------------------------

    '    XmlDocAgenda.LoadXml(strAgenda)

    '    '----- Tag DatiAgenda

    '    XML_DatiAgenda = XmlDocAgenda.SelectSingleNode("DatiAgenda")

    '    '----- Tag Agenda

    '    XML_Agenda = XML_DatiAgenda.SelectSingleNode("Agenda")

    '    Lav_Cod = CStr(XML_Agenda.GetAttribute("lav_cod"))
    '    Des_Lib = CStr(XML_Agenda.GetAttribute("des_lib"))
    '    Id_Agenda = CInt(XML_Agenda.GetAttribute("id_agenda"))

    '    '----- Tag DatiMovimenti

    '    XML_DatiMovimenti = XML_Agenda.SelectSingleNode("DatiMovimenti")

    '    '----- Tag Movimento (multiplo)

    '    XMLs_Movimento = XML_DatiMovimenti.GetElementsByTagName("Movimento")

    '    For i = 0 To XMLs_Movimento.Count - 1

    '        XML_Movimento = XMLs_Movimento.Item(i)

    '        Cau_Mov = XML_Movimento.GetAttribute("cau_mov")

    '        Select Case Cau_Mov

    '            Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_LAVORAZIONE, _
    '                AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_RILIEVO_CAMPO, _
    '                AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_RILIEVO_RACCOLTA, _
    '                AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_TRATTAMENTO

    '                Note = CStr(XML_Movimento.GetAttribute("mov_desc"))
    '                CodDisciplinare = CInt(XML_Movimento.GetAttribute("num_protocollo"))
    '                Mezzo = CInt(XML_Movimento.GetAttribute("mezzo"))
    '                Extra_Int = CInt(XML_Movimento.GetAttribute("extra_int"))
    '                DisciplinarePP = CInt(XML_Movimento.GetAttribute("disciplinare_pubblicoprivato"))

    '                Data = CDate(XML_Movimento.GetAttribute("data_movimento")).ToShortDateString

    '                '--------------------------------
    '                '----- XML Ricetta_Operazione
    '                '--------------------------------

    '                strOperazione = objXml.XML_Ricetta_Operazione( _
    '                                                CInt(enum_TipoOperazioneDB.Scrittura), _
    '                                                CStr(Session("ASG_SuperUser_CodFiscale")), _
    '                                                frm_BaseCode, _
    '                                                frm_TopCode, _
    '                                                CInt(Qs_Ricetta_Cod), _
    '                                                0, _
    '                                                CInt(Lav_Cod), _
    '                                                CStr(Des_Lib), _
    '                                                CStr(Note), _
    '                                                CInt(CodDisciplinare), _
    '                                                , _
    '                                                Extra_Int, _
    '                                                Mezzo, _
    '                                                Data, _
    '                                                Validita_Fine, _
    '                                                , , , _
    '                                                DisciplinarePP)

    '                XmlDoc.LoadXml(strOperazione)

    '                XML_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

    '                'se sto creando la ricetta a partire dall'operazione già registrata le lego
    '                If Id_Agenda <> 0 Then

    '                    XML_DatiRicettaxAgenda = XmlDoc.CreateElement("DatiRicettaxAgenda_2")

    '                    XML_Operazione.AppendChild(XML_DatiRicettaxAgenda)

    '                    strRicettaxAgenda = objXml.XML_RicettaxAgenda_2( _
    '                                                                    CInt(enum_TipoOperazioneDB.Scrittura), _
    '                                                                    CStr(Session("ASG_SuperUser_CodFiscale")), _
    '                                                                    CInt(Qs_Ricetta_Cod), _
    '                                                                    0, _
    '                                                                    CInt(Id_Agenda), _
    '                                                                    Data, _
    '                                                                    Validita_Fine)

    '                    XML_DatiRicettaxAgenda.InnerXml = strRicettaxAgenda

    '                End If

    '                If XML_Movimento.HasChildNodes Then

    '                    '-----------------------------------------
    '                    '-----------------------------------------
    '                    '----- Tag DatiMov_Dettagli_Tecnici
    '                    '-----------------------------------------
    '                    '-----------------------------------------

    '                    strRicettaDettagliTecnici = ""

    '                    XML_DatiMovDettagliTecnici = XML_Movimento.SelectSingleNode("DatiMov_Dettagli_Tecnici")

    '                    If Not XML_DatiMovDettagliTecnici Is Nothing Then

    '                        '----- Tag Movimento_Dettaglio_Tecnico  (multiplo)

    '                        XMLs_MovimentoDettaglioTecnico = XML_DatiMovDettagliTecnici.GetElementsByTagName("Movimento_Dettaglio_Tecnico")

    '                        If Not XMLs_MovimentoDettaglioTecnico Is Nothing AndAlso XMLs_MovimentoDettaglioTecnico.Count > 0 Then

    '                            XML_DatiRicettaDettagliTecnici = XmlDoc.CreateElement("DatiRicetta_Dettagli_Tecnici")

    '                            XML_Operazione.AppendChild(XML_DatiRicettaDettagliTecnici)

    '                            For j = 0 To XMLs_MovimentoDettaglioTecnico.Count - 1

    '                                XML_MovimentoDettaglioTecnico = XMLs_MovimentoDettaglioTecnico.Item(j)

    '                                strRicettaDettaglioTecnico = objXml.XML_Ricetta_DettaglioTecnico(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
    '                                                                                                CStr(Session("ASG_SuperUser_CodFiscale")), _
    '                                                                                                CInt(Qs_Ricetta_Cod), _
    '                                                                                                , , , , _
    '                                                                                                CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("qta_ril")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("dett_cod")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_cod")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_gru")), _
    '                                                                                                CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("dose")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("parziale")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("nitrati")), _
    '                                                                                                CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("freatimetro")), _
    '                                                                                                , _
    '                                                                                                , _
    '                                                                                                , _
    '                                                                                                , _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ditta_cod")), _
    '                                                                                                CStr(XML_MovimentoDettaglioTecnico.GetAttribute("sigla_av")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("trap_num")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("id_insetto")), _
    '                                                                                                CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ff_classe")), _
    '                                                                                                CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_inizio")), _
    '                                                                                                CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_fine")))

    '                                strRicettaDettagliTecnici &= strRicettaDettaglioTecnico

    '                            Next

    '                            XML_DatiRicettaDettagliTecnici.InnerXml = strRicettaDettagliTecnici

    '                        End If

    '                    End If

    '                    '-----------------------------------------
    '                    '-----------------------------------------
    '                    '----- Tag DatiMovimenti_Dettagli
    '                    '-----------------------------------------
    '                    '-----------------------------------------

    '                    strRicettaDettagli = ""
    '                    strRicettaDettagliTecnici = ""
    '                    strRicettaDestinazioni = ""

    '                    XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

    '                    If Not XML_DatiMovimentiDettagli Is Nothing Then

    '                        '----- Tag Movimento_Dettaglio  (multiplo)

    '                        'Recupero la collezione dei nodi
    '                        XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

    '                        If Not XMLs_MovimentoDettaglio Is Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

    '                            XML_DatiRicettaDettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

    '                            XML_Operazione.AppendChild(XML_DatiRicettaDettagli)

    '                            For j = 0 To XMLs_MovimentoDettaglio.Count - 1

    '                                strRicettaDettagliTecnici = ""
    '                                strRicettaDestinazioni = ""

    '                                XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

    '                                TempoCarenza = 0
    '                                DoseEtichetta = ""
    '                                DoseEtichetta_Value = ""
    '                                PrincipiAttivi = ""
    '                                ClassiTossicologiche = ""
    '                                If XML_MovimentoDettaglio.HasAttribute("tempocarenza") = True Then
    '                                    TempoCarenza = CInt(XML_MovimentoDettaglio.GetAttribute("tempocarenza"))
    '                                End If
    '                                If XML_MovimentoDettaglio.HasAttribute("doseetichetta") = True Then
    '                                    DoseEtichetta = CStr(XML_MovimentoDettaglio.GetAttribute("doseetichetta"))
    '                                End If
    '                                If XML_MovimentoDettaglio.HasAttribute("doseetichetta_value") = True Then
    '                                    DoseEtichetta_Value = CStr(XML_MovimentoDettaglio.GetAttribute("doseetichetta_value"))
    '                                End If
    '                                If XML_MovimentoDettaglio.HasAttribute("principiattivi") = True Then
    '                                    PrincipiAttivi = CStr(XML_MovimentoDettaglio.GetAttribute("principiattivi"))
    '                                End If
    '                                If XML_MovimentoDettaglio.HasAttribute("classitossicologiche") = True Then
    '                                    ClassiTossicologiche = CStr(XML_MovimentoDettaglio.GetAttribute("classitossicologiche"))
    '                                End If

    '                                Dim Qta_Extra As Decimal = 0
    '                                Dim Qta_Extra_Totale As Decimal = 0
    '                                Dim Udm_Cod_Extra As Integer = 0
    '                                Dim Mezzo_Det As Integer = 0

    '                                If XML_MovimentoDettaglio.HasAttribute("qta_extra") = True AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("qta_extra")) Then
    '                                    Qta_Extra = CStr(XML_MovimentoDettaglio.GetAttribute("qta_extra"))
    '                                End If
    '                                If XML_MovimentoDettaglio.HasAttribute("qta_extra_totale") = True AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("qta_extra_totale")) Then
    '                                    Qta_Extra_Totale = CStr(XML_MovimentoDettaglio.GetAttribute("qta_extra_totale"))
    '                                End If
    '                                If XML_MovimentoDettaglio.HasAttribute("udm_cod_extra") = True AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("udm_cod_extra")) Then
    '                                    Udm_Cod_Extra = CStr(XML_MovimentoDettaglio.GetAttribute("udm_cod_extra"))
    '                                End If
    '                                If XML_MovimentoDettaglio.HasAttribute("mezzo_det") = True AndAlso IsNumeric(XML_MovimentoDettaglio.GetAttribute("mezzo_det")) Then
    '                                    Mezzo_Det = CStr(XML_MovimentoDettaglio.GetAttribute("mezzo_det"))
    '                                End If

    '                                strRicettaDettaglio = objXml.XML_Ricetta_Dettaglio(
    '                                                            CInt(enum_TipoOperazioneDB.Scrittura),
    '                                                            CStr(Session("ASG_SuperUser_CodFiscale")),
    '                                                            CInt(Qs_Ricetta_Cod),
    '                                                             0,
    '                                                             0,
    '                                                             1,
    '                                                            CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")),
    '                                                            CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")),
    '                                                            CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")),
    '                                                            CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")),
    '                                                            CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")),
    '                                                            CDbl(XML_MovimentoDettaglio.GetAttribute("qta")),
    '                                                            CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")),
    '                                                            CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")),
    '                                                            0,
    '                                                            Cau_Mov,
    '                                                            TempoCarenza,
    '                                                            DoseEtichetta,
    '                                                            PrincipiAttivi,
    '                                                            ClassiTossicologiche,
    '                                                            DoseEtichetta_Value,
    '                                                            ,,,,
    '                                                            Qta_Extra, Qta_Extra_Totale, Udm_Cod_Extra, Mezzo_Det)


    '                                XmlDoc2.LoadXml(strRicettaDettaglio)

    '                                XML_RicettaDettaglio = XmlDoc2.SelectSingleNode("Ricetta_Dettaglio")

    '                                '----- Tag Movimento_Dettaglio_Tecnico_2  (multiplo)

    '                                XMLs_Movimento_Dettaglio_Tecnico_2 = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Dettaglio_Tecnico_2")

    '                                If Not XMLs_Movimento_Dettaglio_Tecnico_2 Is Nothing AndAlso XMLs_Movimento_Dettaglio_Tecnico_2.Count > 0 Then

    '                                    For n = 0 To XMLs_Movimento_Dettaglio_Tecnico_2.Count - 1

    '                                        XML_MovimentoDettaglioTecnico = XMLs_Movimento_Dettaglio_Tecnico_2.Item(n)

    '                                        Dim Inn1 As Date = AGRODATAINIZIO
    '                                        Dim Inn2 As Date = AGRODATAFINE
    '                                        If IsDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn1_data")) Then
    '                                            Inn1 = CDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn1_data"))
    '                                        End If
    '                                        If IsDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn2_data")) Then
    '                                            Inn2 = CDate(XML_MovimentoDettaglioTecnico.GetAttribute("inn2_data"))
    '                                        End If

    '                                        Dim Mg As Decimal = 0
    '                                        Dim Azoto As Decimal = 0
    '                                        Dim P As Decimal = 0
    '                                        Dim K As Decimal = 0
    '                                        Dim Efficienza As Decimal = 0
    '                                        If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("mg")) Then
    '                                            Mg = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("mg"))
    '                                        End If
    '                                        If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("n")) Then
    '                                            Azoto = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("n"))
    '                                        End If
    '                                        If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("p")) Then
    '                                            P = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("p"))
    '                                        End If
    '                                        If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("k")) Then
    '                                            K = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("k"))
    '                                        End If
    '                                        If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("efficienza")) Then
    '                                            Efficienza = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("efficienza"))
    '                                        End If

    '                                        Dim Soglia_Cod As Integer = 0
    '                                        Dim Soglia_Des As String = ""
    '                                        Dim Soglia_Qta As Decimal = 0
    '                                        If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_cod")) Then
    '                                            Soglia_Cod = CInt(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_cod"))
    '                                        End If
    '                                        Soglia_Des = XML_MovimentoDettaglioTecnico.GetAttribute("soglia_des")
    '                                        If IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_quantita")) Then
    '                                            Soglia_Qta = CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("soglia_quantita"))
    '                                        End If

    '                                        Dim Cu As Decimal = 0
    '                                        If XML_MovimentoDettaglioTecnico.HasAttribute("cu") = True AndAlso IsNumeric(XML_MovimentoDettaglioTecnico.GetAttribute("cu")) Then
    '                                            Cu = CInt(XML_MovimentoDettaglioTecnico.GetAttribute("cu"))
    '                                        End If

    '                                        strRicettaDettaglioTecnico = objXml.XML_Ricetta_DettaglioTecnico_2(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
    '                                                                                                        CStr(Session("ASG_SuperUser_CodFiscale")),
    '                                                                                                        CInt(Qs_Ricetta_Cod),
    '                                                                                                        , , ,
    '                                                                                                        1,
    '                                                                                                        CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("qta_ril")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("dett_cod")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_cod")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("av_gru")),
    '                                                                                                        CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("dose")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("parziale")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("nitrati")),
    '                                                                                                        CDbl(XML_MovimentoDettaglioTecnico.GetAttribute("freatimetro")),
    '                                                                                                        Inn1, Inn2,
    '                                                                                                        ,
    '                                                                                                        ,
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ditta_cod")),
    '                                                                                                        CStr(XML_MovimentoDettaglioTecnico.GetAttribute("sigla_av")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("trap_num")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("id_insetto")),
    '                                                                                                        CInt(XML_MovimentoDettaglioTecnico.GetAttribute("ff_classe")),
    '                                                                                                        CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_inizio")),
    '                                                                                                        CDate(XML_MovimentoDettaglioTecnico.GetAttribute("validita_fine")),
    '                                                                                                        Mg,
    '                                                                                                        Azoto,
    '                                                                                                        P,
    '                                                                                                        K,
    '                                                                                                        Soglia_Cod,
    '                                                                                                        Soglia_Des,
    '                                                                                                        Soglia_Qta,
    '                                                                                                        Efficienza,,
    '                                                                                                           ,,,,
    '                                                                                                           Cu)

    '                                        strRicettaDettagliTecnici &= strRicettaDettaglioTecnico

    '                                    Next


    '                                End If


    '                                '----- Tag Movimento_Destinazione  (multiplo)

    '                                XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

    '                                If Not XMLs_MovimentoDestinazione Is Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

    '                                    For n = 0 To XMLs_MovimentoDestinazione.Count - 1

    '                                        XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)

    '                                        Dim Qta2 As Decimal = 0D
    '                                        If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
    '                                            Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
    '                                        End If

    '                                        Dim Tipo_Destinazione As Integer
    '                                        If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
    '                                            Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
    '                                        End If

    '                                        strRicettaDestinazione = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
    '                                                                                                 CStr(Session("ASG_SuperUser_CodFiscale")), _
    '                                                                                                 CInt(Qs_Ricetta_Cod), _
    '                                                                                                 , , , _
    '                                                                                                 CStr(XML_MovimentoDestinazione.GetAttribute("piva")), _
    '                                                                                                 CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod")), _
    '                                                                                                 CInt(XML_MovimentoDestinazione.GetAttribute("appezza")), _
    '                                                                                                 CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione")), _
    '                                                                                                 CInt(IIf(IsNumeric(XML_MovimentoDestinazione.GetAttribute("programmazione_entita_cod")), XML_MovimentoDestinazione.GetAttribute("programmazione_entita_cod"), 0)), _
    '                                                                                                 CDbl(XML_MovimentoDestinazione.GetAttribute("qta")), _
    '                                                                                                 CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")), _
    '                                                                                                 CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")), _
    '                                                                                                 CDbl(IIf(IsNumeric(XML_MovimentoDestinazione.GetAttribute("quotadistribuzione")), XML_MovimentoDestinazione.GetAttribute("quotadistribuzione"), 0)), _
    '                                                                                                 Qta2, _
    '                                                                                                 Tipo_Destinazione) ' errore stringa programmazione_entità_cod

    '                                        strRicettaDestinazioni &= strRicettaDestinazione

    '                                    Next

    '                                End If

    '                                XML_RicettaDettaglio.InnerXml = strRicettaDettagliTecnici & strRicettaDestinazioni

    '                                Dim pro As String = XML_RicettaDettaglio.OuterXml

    '                                XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & XML_RicettaDettaglio.OuterXml

    '                            Next

    '                        End If

    '                    End If

    '                End If

    '            Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_PARCOMACCHINE, _
    '                 AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE, _
    '                 AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_MANODOPERA, _
    '                 AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_TERZISTI, _
    '                 AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_SCARICO

    '                '-----------------------------------------
    '                '-----------------------------------------
    '                '----- Tag DatiMovimenti_Dettagli
    '                '-----------------------------------------
    '                '-----------------------------------------

    '                strRicettaDestinazioni = ""

    '                XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

    '                If Not XML_DatiMovimentiDettagli Is Nothing Then

    '                    '----- Tag Movimento_Dettaglio  (multiplo)

    '                    'Recupero la collezione dei nodi
    '                    XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")

    '                    If Not XMLs_MovimentoDettaglio Is Nothing AndAlso XMLs_MovimentoDettaglio.Count > 0 Then

    '                        For j = 0 To XMLs_MovimentoDettaglio.Count - 1

    '                            strRicettaDestinazioniCostiAccessori = ""

    '                            XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(j)

    '                            Dim Qualifica_Cod As Integer = 0
    '                            Dim Tariffa_Cod As Integer = 0
    '                            Dim prezzo_unitario As Decimal = 0
    '                            Dim id_attivita As Integer = 0
    '                            Dim lotto As String = ""

    '                            If XML_MovimentoDettaglio.HasAttribute("qualifica_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("qualifica_cod")) Then
    '                                Qualifica_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("qualifica_cod"))
    '                            End If

    '                            If XML_MovimentoDettaglio.HasAttribute("tariffa_cod") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("tariffa_cod")) Then
    '                                Tariffa_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("tariffa_cod"))
    '                            End If

    '                            If XML_MovimentoDettaglio.HasAttribute("prezzo_unitario") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario")) Then
    '                                prezzo_unitario = CDec(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario"))
    '                            End If

    '                            If XML_MovimentoDettaglio.HasAttribute("id_attivita") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("id_attivita")) Then
    '                                id_attivita = CInt(XML_MovimentoDettaglio.GetAttribute("id_attivita"))
    '                            End If

    '                            If XML_MovimentoDettaglio.HasAttribute("lotto") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDettaglio.GetAttribute("lotto")) Then
    '                                lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
    '                            End If

    '                            strRicettaDettaglioCostiAccessori = objXml.XML_Ricetta_Dettaglio( _
    '                                                                        CInt(enum_TipoOperazioneDB.Scrittura), _
    '                                                                        CStr(Session("ASG_SuperUser_CodFiscale")), _
    '                                                                        CInt(Qs_Ricetta_Cod), _
    '                                                                        0, _
    '                                                                        0, _
    '                                                                        0, _
    '                                                                        CInt(XML_MovimentoDettaglio.GetAttribute("elem_cod")), _
    '                                                                        CInt(XML_MovimentoDettaglio.GetAttribute("pro_cod")), _
    '                                                                        CInt(XML_MovimentoDettaglio.GetAttribute("mat_cod")), _
    '                                                                        CInt(XML_MovimentoDettaglio.GetAttribute("udm_cod")), _
    '                                                                        CInt(XML_MovimentoDettaglio.GetAttribute("extra_int")), _
    '                                                                        CDbl(XML_MovimentoDettaglio.GetAttribute("qta")), _
    '                                                                        CDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")), _
    '                                                                        CDate(XML_MovimentoDettaglio.GetAttribute("validita_fine")), _
    '                                                                        prezzo_unitario, _
    '                                                                        Cau_Mov, _
    '                                                                        Qualifica_cod:=Qualifica_Cod, _
    '                                                                        Tariffa_cod:=Tariffa_Cod, _
    '                                                                        id_attivita:=id_attivita, _
    '                                                                        lotto:=lotto)


    '                            XmlDoc3.LoadXml(strRicettaDettaglioCostiAccessori)

    '                            XML_RicettaDettaglio = XmlDoc3.SelectSingleNode("Ricetta_Dettaglio")

    '                            '----- Tag Movimento_Destinazione  (multiplo)

    '                            XMLs_MovimentoDestinazione = XML_MovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

    '                            If Not XMLs_MovimentoDestinazione Is Nothing AndAlso XMLs_MovimentoDestinazione.Count > 0 Then

    '                                For n = 0 To XMLs_MovimentoDestinazione.Count - 1

    '                                    XML_MovimentoDestinazione = XMLs_MovimentoDestinazione.Item(n)


    '                                    Dim Qta2 As Decimal = 0D
    '                                    If XML_MovimentoDestinazione.HasAttribute("qta2") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("qta2")) Then
    '                                        Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
    '                                    End If

    '                                    Dim Tipo_Destinazione As Integer
    '                                    If XML_MovimentoDestinazione.HasAttribute("tipo_destinazione") AndAlso Not String.IsNullOrEmpty(XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")) Then
    '                                        Tipo_Destinazione = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
    '                                    End If

    '                                    strRicettaDestinazioneCostiAccessori = objXml.XML_Ricetta_Destinazione(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
    '                                                                                             CStr(Session("ASG_SuperUser_CodFiscale")), _
    '                                                                                             CInt(Qs_Ricetta_Cod), _
    '                                                                                             , , , _
    '                                                                                             CStr(XML_MovimentoDestinazione.GetAttribute("piva")), _
    '                                                                                             CInt(XML_MovimentoDestinazione.GetAttribute("sa_cod")), _
    '                                                                                             CInt(XML_MovimentoDestinazione.GetAttribute("appezza")), _
    '                                                                                             CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione")), _
    '                                                                                             0, _
    '                                                                                             CDbl(XML_MovimentoDestinazione.GetAttribute("qta")), _
    '                                                                                             CDate(XML_MovimentoDestinazione.GetAttribute("validita_inizio")), _
    '                                                                                             CDate(XML_MovimentoDestinazione.GetAttribute("validita_fine")), _
    '                                                                                             0, _
    '                                                                                             Qta2, _
    '                                                                                             Tipo_Destinazione)

    '                                    strRicettaDestinazioniCostiAccessori &= strRicettaDestinazioneCostiAccessori

    '                                Next

    '                            End If

    '                            XML_RicettaDettaglio.InnerXml = strRicettaDestinazioniCostiAccessori

    '                            Dim pro As String = XML_RicettaDettaglio.OuterXml

    '                            strRicettaDettagliCostiAccessori &= XML_RicettaDettaglio.OuterXml

    '                            'XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & XML_RicettaDettaglio.OuterXml

    '                        Next


    '                    End If

    '                End If

    '        End Select

    '    Next

    '    XML_DatiRicettaDettagli.InnerXml = XML_DatiRicettaDettagli.InnerXml & strRicettaDettagliCostiAccessori

    '    Dim pippo As String
    '    pippo = XmlDoc.OuterXml

    '    objXml = Nothing

    '    '----- Restituisco il risultato

    '    Return XmlDoc.OuterXml

    'End Function

#End Region

#Region "Ribaltamento Operazioni su Agenda"


    Protected Sub Btn_FiltraImpianti_Click(sender As Object, e As EventArgs) Handles Btn_FiltraImpianti.Click


        Dim ParametriRicette_2010 As New ParametriRicette_2010
        ParametriRicette_2010.Ricetta_Cod = Qs_Ricetta_Cod
        ParametriRicette_2010.Piva = Qs_Piva
        ParametriRicette_2010.Sa_Cod = Qs_Sa_Cod
        ParametriRicette_2010.Veg_Cod = Qs_Veg_Cod
        ParametriRicette_2010.Ricetta_Cod = Qs_Ricetta_Cod
        ParametriRicette_2010.Tipo_Operazione = enum_TipoOperazioneDB.Trasferimento
        ParametriRicette_2010.Ricetta_Des = Txt_Ricetta_Des.Text
        ParametriRicette_2010.Tipo_Ricetta = Qs_Tipo_Ricetta
        ParametriRicette_2010.Programmazione_Cod = Qs_Programmazione_Cod
        If IsDate(Txt_DataInizio.Text) Then
            ParametriRicette_2010.data_inizio = CDate(Txt_DataInizio.Text)
        End If
        If IsDate(Txt_DataFine.Text) Then
            ParametriRicette_2010.data_fine = CDate(Txt_DataFine.Text)
        End If

        ParametriRicette_2010.Salva()

        objParametriAgenda.Impianti.Clear()

        Dim TargetRedirect As String

        TargetRedirect = "../Filtrone/Filtrone.aspx?p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_Ricette_Edit, AgroKey_EncoderDecoder, Server) &
                        "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
                        "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_Ricette_Edit, AgroKey_EncoderDecoder, Server) &
                        "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
                        "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.Associa_Ricetta_Impianti, AgroKey_EncoderDecoder, Server) &
                        "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                        "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server)

        Response.Redirect(TargetRedirect)

        'Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
        'objGiasOnline.Operazione = enum_TipoOperazioneDB.Trasferimento
        'objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.FiltroImpresa_new4_Ricetta
        'objGiasOnline.Piva = Qs_Piva
        'objGiasOnline.Sa_Cod = Qs_Sa_Cod
        'objGiasOnline.Veg_Cod = Qs_Veg_Cod
        'objGiasOnline.xChiave = Qs_Ricetta_Cod


        'Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline( _
        '                          Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
        '                          objGiasOnline)
        'Response.Redirect(str)

    End Sub

    '#####################################################################################################
    Public Sub chkSelected_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)


        Dim NumSelezionati As Integer
        Dim i As Integer
        Dim Indice As Integer = 0
        Dim ArrayPannelli(0) As enum_TipoPannello

        Dim Lav_Cod As Integer
        Dim XML_Operazione As String
        Dim Operazione_Richiesta As Integer

        NumSelezionati = 0
        For i = 0 To GridView_Operazioni.Rows.Count - 1
            If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then
                Indice = i
                NumSelezionati += 1
            End If
        Next

        Select Case NumSelezionati

            Case 0

                Dim N_Pannelli As Integer = 0
                ArrayPannelli(N_Pannelli) = enum_TipoPannello.Nessun_Pannello
                N_Pannelli += 1

                'Select Case Operazione_Richiesta
                '    Case enum_TipoOperazioneDB.Trasferimento
                '        ReDim Preserve ArrayPannelli(N_Pannelli)
                '        ArrayPannelli(N_Pannelli) = enum_TipoPannello.Pannello_Impianti
                '        N_Pannelli += 1
                'End Select

                If GridView_Impianti.Rows.Count > 0 Then
                    ReDim Preserve ArrayPannelli(N_Pannelli)
                    ArrayPannelli(N_Pannelli) = enum_TipoPannello.Pannello_Impianti
                    N_Pannelli += 1
                End If

                Imposta_Pannelli(ArrayPannelli)

            Case 1

                Lav_Cod = CInt(GridView_Operazioni.Rows(Indice).Cells(2).Text())

                'verifica operazioni gestite
                Select Case Lav_Cod
                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                            LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                            LAVCOD_CONCIA_SEME,
                            LAVCOD_GEODISINFESTAZIONE,
                            LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                        'ok

                    Case LAVCOD_IRRIGAZIONE, LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                        LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_SARCHIATURA_CONCIMAZIONE,
                         LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
                         LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE,
                         LAVCOD_ANDANAMENTO, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA,
                         LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ERPICATURA_ROTANTE,
                         LAVCOD_ESPIANTO, LAVCOD_ESTIRPATURA, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI,
                         LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_GEBIATURA, LAVCOD_IMBALLO_FIENO_ROTOLI,
                         LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_LAVORAZIONE_CONBINATA, LAVCOD_LAVORAZIONE_TRA_FILA,
                         LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE,
                         LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE,
                         LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA,
                         LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_ROMPICROSTA, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA,
                         LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_SOD_SEDDING, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA,
                         LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_TRAPIANTO

                    Case Else
                        Messaggi.AgroMsgBox("Operazione non gestita!", Page, , Script_Panel)
                        Exit Sub
                End Select

                XML_Operazione = GridView_Operazioni.Rows(Indice).Cells(3).Text()
                Session("ricetta_operazione_cod") = GridView_Operazioni.Rows(Indice).Cells(1).Text()
                Operazione_Richiesta = enum_TipoOperazioneDB.Trasferimento

                Ripristina_Dati_nei_Controlli(Lav_Cod, XML_Operazione, Operazione_Richiesta)

            Case Else
                Call Messaggi.AgroMsgBox("Deve essere selezionato un intervento alla volta!", Page, , Script_Panel)
                Exit Sub

        End Select

    End Sub

    Public Sub Aggiungi_Dettaglio()

        Dim Xml_Operazione As String

        Select Case CInt(ComboOperazione.Valore_Combo)

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                Xml_Operazione = XML_GeneraStringa_Ricetta_Trattamento()

            Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                Xml_Operazione = XML_GeneraStringa_Ricetta_Concimazione()

                'LAVORAZIONI
            Case LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ANDANAMENTO, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
                 LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE,
                 LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ERPICATURA_ROTANTE, LAVCOD_ESPIANTO,
                 LAVCOD_ESTIRPATURA, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI,
                 LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_GEBIATURA, LAVCOD_IMBALLO_FIENO_ROTOLI,
                 LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_LAVORAZIONE_CONBINATA,
                 LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO,
                 LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE,
                 LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA,
                 LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA,
                 LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_ROMPICROSTA, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA,
                 LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_SOD_SEDDING, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA,
                 LAVCOD_ZAPPATURA, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_TRAPIANTO

                Xml_Operazione = XML_GeneraStringa_Ricetta_Lavorazioni()

            Case LAVCOD_IRRIGAZIONE

                Xml_Operazione = XML_GeneraStringa_Ricetta_Irrigazione()

            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA, LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                Xml_Operazione = XML_GeneraStringa_Ricetta_Trappole()

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                Xml_Operazione = XML_GeneraStringa_Ricetta_RilievoAvvCampo()

            Case Else




        End Select



        If Xml_Operazione IsNot Nothing Then

            'TODO: Vanni, 23/09/2016 09:45:35: Data operazione, verificare...
            Inserisci_Operazione(0,
                                 Me.Lbl_Riepilogo.Text,
                                 Txt_DataInizio.Text,
                                 CInt(ComboOperazione.Valore_Combo),
                                 Xml_Operazione,
                                 CInt(Lbl_Id_Agenda.Text))

            '------------------------------
            'Azzero i controlli

            Dim ArrayPannelli(0) As enum_TipoPannello
            ArrayPannelli(0) = enum_TipoPannello.Nessun_Pannello
            If GridView_Impianti.Rows.Count > 0 Then
                ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
                ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
            End If
            Imposta_Pannelli(ArrayPannelli)

            Svuota_Controlli_Operazione()

        End If


        Dim script As New StringBuilder

        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("             $('#dialogOperazione').dialog('close');")
        script.AppendLine("     });")
        ScriptManager.RegisterClientScriptBlock(UpdatePanelOperazione, UpdatePanelOperazione.GetType(),
                    String.Format("jQuery_{0}", UpdatePanelOperazione.ClientID), script.ToString, True)


        'ricarico gli impianti



    End Sub

    Public Sub Svuota_Controlli_Operazione()

        ComboOperazione.Valore_Combo = ""
        Me.Lbl_Riepilogo.Text = ""

        '------------------------------

        Me.Txt_Note_Lavorazioni.Text = ""

        '------------------------------
        Svuota_Controlli_Difesa()

        Me.Cmb_Disciplinare.SelectedIndex = -1
        cella_disciplinare.Visible = False


        Me.Cmb_Epoca.SelectedIndex = -1
        Me.Cmb_Epoca.Visible = False
        cella_epoca.Visible = False

        Me.Txt_Note_Difesa.Text = ""
        Me.Txt_Acqua_Difesa.Text = ""

        Me.RBL_Dose_Formulati.Enabled = True

        '------------------------------

        Svuota_Controlli_Concimazione()

        Me.ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedIndex = -1
        Me.ComboEpoche.Visible = False
        cella_epoca.Visible = False

        Me.Txt_Note_Concimazione.Text = ""
        Me.Txt_Acqua_Concimazione.Text = ""

        Rbl_Dosi_Fertilizzanti.Items.Clear()

        '------------------------------

        Svuota_Controlli_Irrigazione()

        Me.Txt_Note_Irrigazione.Text = ""

        '------------------------------

        Svuota_Controlli_Trappole()

        Me.Txt_Note_Irrigazione.Text = ""

        '------------------------------

        Btn_Costi.Visible = False

    End Sub

    Public Sub Annulla_Dettaglio()

        '------------------------------
        'Azzero i controlli

        Dim ArrayPannelli(0) As enum_TipoPannello
        ArrayPannelli(0) = enum_TipoPannello.Nessun_Pannello
        If GridView_Impianti.Rows.Count > 0 Then
            ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
            ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
        End If
        Imposta_Pannelli(ArrayPannelli)

        Svuota_Controlli_Operazione()

        GridView_Operazioni.DataSource = ViewState("dt_Operazioni")
        GridView_Operazioni.DataBind()

        Session("dtScarico") = Nothing


        Dim script As New StringBuilder

        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("             $('#dialogOperazione').dialog('close');")
        script.AppendLine("     });")
        ScriptManager.RegisterClientScriptBlock(UpdatePanelOperazione, UpdatePanelOperazione.GetType(),
                    String.Format("jQuery_{0}", UpdatePanelOperazione.ClientID), script.ToString, True)

        ''  Marco Grilli, 12/04/2016 16:06:53: Una volta aggiunta l'operazione devo aggiornare i campi di NPK distribuiti
        'If Qs_Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
        '    Carica_Impianti("", ViewState(""))
        'End If

    End Sub

    Public Sub Salva_Operazione()

        Dim AlmenoUna As Boolean
        Dim StessaSpecie As Boolean

        Dim DataInizio As Date
        Dim DataFine As Date
        Dim DataInterventi As Date
        Dim DataInizioImpianto As Date
        Dim DataFineImpianto As Date

        Dim i, j, n, x As Integer

        Dim Indice As Integer
        Dim Lav_Cod As Integer
        Dim strXml As String

        Dim Agenda As Operazione_Agenda

        Dim Messaggio As String

        '--------------------------------------------------------------
        'CONTROLLO DATI

        'verifico di aver selezionato almeno 1OPERAZIONE
        For i = 0 To GridView_Operazioni.Rows.Count - 1
            If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then
                AlmenoUna = True
                Exit For
            End If
        Next
        If Not AlmenoUna Then
            Messaggi.AgroMsgBox("Selezionare almeno una Operazione!", Page, , Script_Panel)
            Exit Sub
        End If

        AlmenoUna = False

        'verifico di aver selezionato almeno 1 IMPIANTO
        For i = 0 To GridView_Impianti.Rows.Count - 1
            If CType(GridView_Impianti.Rows(i).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then
                AlmenoUna = True
                Exit For
            End If
        Next
        If Not AlmenoUna Then
            Messaggi.AgroMsgBox("Selezionare almeno un Impianto Colturale!", Page, , Script_Panel)
            Exit Sub
        End If

        'verifico di aver selezionato la STESSA SPECIE della RICETTA
        StessaSpecie = True
        For i = 0 To GridView_Impianti.Rows.Count - 1
            If GridView_Impianti.Rows(i).Cells(6).Text <> Me.Cmb_Specie.SelectedItem.Value Then
                StessaSpecie = False
                Exit For
            End If
        Next
        If Not StessaSpecie Then
            Messaggi.AgroMsgBox("Si possono selezionare solo Impianti di " & Me.Cmb_Specie.SelectedItem.Text & "!", Page, , Script_Panel)
            Exit Sub
        End If

        'verifica la COERENZA delle DATE
        'che la data scelta sia appartenete all'intervallo di validita della RICETTA e degli IMPIANTI
        If Me.Txt_Data_Interventi.Text = "" Then
            Messaggi.AgroMsgBox("E' necessario scegliere la data in cui si desiderano creare le Operazioni!", Page, , Script_Panel)
            Exit Sub
        Else

            DataInterventi = CDate(Me.Txt_Data_Interventi.Text)
            DataInizio = CDate(Me.Txt_DataInizio.Text)
            DataFine = CDate(Me.Txt_DataFine.Text)

            If DataInterventi < DataInizio OrElse DataInterventi > DataFine Then
                Messaggi.AgroMsgBox("La data di registrazione delle Operazioni deve appartenere all'intervallo di validità della Ricetta (" & Me.Txt_DataInizio.Text & " - " & Me.Txt_DataFine.Text & ")!", Page, , Script_Panel)
                Exit Sub
            End If

            For i = 0 To GridView_Impianti.Rows.Count - 1
                If CType(GridView_Impianti.Rows(i).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then

                    If GridView_Impianti.Rows(i).Cells(18).Text = "..." Then
                        DataInizioImpianto = AGRODATAINIZIO
                    Else
                        DataInizioImpianto = CDate(GridView_Impianti.Rows(i).Cells(18).Text)
                    End If
                    If GridView_Impianti.Rows(i).Cells(19).Text = "..." Then
                        DataFineImpianto = AGRODATAFINE
                    Else
                        DataFineImpianto = CDate(GridView_Impianti.Rows(i).Cells(19).Text)
                    End If
                    If DataInizioImpianto > DataInterventi OrElse
                        DataFineImpianto < DataInterventi Then
                        Messaggi.AgroMsgBox("L'Impianto " & GridView_Impianti.Rows(i).Cells(16).Text & " non è Attivo alla Data scelta per la registrazione delle Operazioni!", Page, , Script_Panel)
                        Exit Sub
                    End If
                End If
            Next

        End If


        Try


            For n = 0 To GridView_Operazioni.Rows.Count - 1

                If CType(GridView_Operazioni.Rows(n).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                    Lav_Cod = CInt(GridView_Operazioni.Rows(n).Cells(2).Text)
                    strXml = GridView_Operazioni.Rows(n).Cells(3).Text

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


                    'Recupero dalla griglia gli impianti selezionati
                    For Indice = 0 To GridView_Impianti.Rows.Count - 1

                        If CType(GridView_Impianti.Rows(Indice).FindControl("chkSelezionaImpianto"), CheckBox).Checked = True Then

                            'Creo una nuova riga
                            Dr = DtImpiantiSelezionati.NewRow

                            'Definisco i valori
                            Dr.Item("piva") = GridView_Impianti.Rows(Indice).Cells(1).Text
                            Dr.Item("sa_cod") = CInt(GridView_Impianti.Rows(Indice).Cells(2).Text)
                            Dr.Item("appezza") = CInt(GridView_Impianti.Rows(Indice).Cells(4).Text)
                            Dr.Item("id_reg") = CInt(GridView_Impianti.Rows(Indice).Cells(5).Text)
                            Dr.Item("descrizione") = GridView_Impianti.Rows(Indice).Cells(16).Text
                            Dr.Item("sup_imp") = CDbl(GridView_Impianti.Rows(Indice).Cells(17).Text)

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

                        For i = 0 To strPiva.Length - 1

                            DtImpresa.Rows.Clear()

                            DrImprese = DtImpiantiSelezionati.Select("piva='" & strPiva(i) & "'")

                            For j = 0 To DrImprese.Length - 1
                                DtImpresa.ImportRow(DrImprese(j))
                            Next

                            'seleziono i centri per ogni impresa
                            strCentri = objSqlDis.SelectDistinct(DtImpresa, "sa_cod")

                            If strCentri IsNot Nothing AndAlso strCentri.Length > 0 Then

                                For x = 0 To strCentri.Length - 1

                                    DtCentro.Rows.Clear()

                                    DrCentri = DtImpresa.Select("piva=" & strPiva(i) & " AND sa_cod=" & strCentri(x))

                                    For j = 0 To DrCentri.Length - 1
                                        DtCentro.ImportRow(DrCentri(j))
                                    Next

                                    'salvo un'operazione per tutti gli impianti appartenenti ad uno stesso centro

                                    Select Case Lav_Cod

                                        Case LAVCOD_IRRIGAZIONE

                                            Agenda = Genera_Agenda_Irrigazione(Lav_Cod, DtCentro, Messaggio)

                                        Case LAVCOD_DISTRIBUZIONE_CONCIME,
                                            LAVCOD_FERTIRRIGAZIONE,
                                            LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                            LAVCOD_CONCIMAZIONE_FOGLIARE,
                                            LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                            LAVCOD_SARCHIATURA_CONCIMAZIONE

                                            Agenda = Genera_Agenda_Concimazione(Lav_Cod, DtCentro, Messaggio)

                                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                                    LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                                    LAVCOD_CONCIA_SEME,
                                                    LAVCOD_GEODISINFESTAZIONE,
                                                    LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO


                                            Agenda = Genera_Agenda_Trattamento(Lav_Cod, DtCentro, Messaggio)

                                        Case LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_CONFUSIONE_SESSUALE

                                            Agenda = Genera_Agenda_ConfusioneDisorientamento(Lav_Cod, DtCentro, Messaggio)

                                        Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                            Agenda = Genera_Agenda_InstallazioneTrappole(Lav_Cod, DtCentro, Messaggio)

                                        Case Else

                                            Agenda = Genera_Agenda_Lavorazione(Lav_Cod, DtCentro, Messaggio)

                                    End Select

                                    If Messaggio <> "" Then
                                        Throw New Exception(Messaggio)
                                    End If

                                    'Aggiungo i costi accessori
                                    SalvaCostiAccessori_Agenda(Agenda)

                                    '---------------------------------------------------------------
                                    '---------------------------------------------------------------
                                    'creo l'operazione
                                    '---------------------------------------------------------------
                                    '---------------------------------------------------------------
                                    If IsNothing(Agenda) AndAlso Not IsNothing(Session("ricetta_operazione_cod")) <> 0 Then
                                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                                    End If

                                    Dim objAgendaScrivi As New Agenda_Operazione_Helper
                                    Dim Id_Agenda As Integer = 0

                                    Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                                    'devo scrivere i riferimenti in ricetteXAgenda
                                    Dim ricetta_cod As String = Qs_Ricetta_Cod
                                    Dim Ricetta_Op_Cod As String = Session("ricetta_operazione_cod")

                                    Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                    Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni,
                                                                            HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                                                            1)
                                    If Impostazione_RicetteXagenda <> "0" Then
                                        Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                        If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Op_Cod, Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                            Throw New Exception(" Non È Riuscito L'Aggancio Della Ricetta")
                                        End If

                                    End If


                                Next

                            End If

                        Next

                    End If

                End If

            Next

            '------------------------------------------------
            '----- Conferma di aggiornamento del database
            '------------------------------------------------

            Messaggi.AgroMsgBuonFine("", Page)

        Catch ex As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Messaggio di errore
            Messaggio = "Si e' verificato un'errore : " &
                        Chr(13) &
                        ex.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)

            '------------------------------------------------

        End Try

        Svuota_Controlli_Operazione()

        Dim script As New StringBuilder

        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("             $('#dialogOperazione').dialog('close');")
        script.AppendLine("     });")
        ScriptManager.RegisterClientScriptBlock(UpdatePanelOperazione, UpdatePanelOperazione.GetType(),
                    String.Format("jQuery_{0}", UpdatePanelOperazione.ClientID), script.ToString, True)


    End Sub

    Private Function Genera_Agenda_Trattamento(ByVal Lav_Cod As Integer,
                                               ByVal Dt As DataTable,
                                               ByRef strErr As String
                                               ) As Operazione_Agenda


        Dim Piva As String = Dt.Rows(0).Item("piva")
        Dim SaCod As Integer = Dt.Rows(0).Item("sa_cod")


        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = CDate(Me.Txt_Data_Interventi.Text)
        Agenda.Piva = Piva
        Agenda.Sa_Cod = SaCod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = ComboOperazione.Testo_Combo & " (" & Cmb_Specie.SelectedItem.Text & ")"

        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        For i = 0 To CBL_Consigli_Difesa.Items.Count - 1
            If CBL_Consigli_Difesa.Items(i).Selected Then
                Nota = New Nota
                Nota.Nota_Cod = CBL_Consigli_Difesa.Items(i).Value
                Agenda.Note.Add(Nota)
            End If
        Next

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------
        Dim Modulo As Integer = 0

        Movimento = New Movimento

        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = CDate(Me.Txt_Data_Interventi.Text)
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_TRATTAMENTO
        Movimento.Mov_Desc = Txt_Note_Difesa.Text
        '0=HL 1=HA
        Movimento.Mezzo = CInt(RBL_Dose_Formulati.SelectedValue)
        '11=dose (10=qta tot non contemplata)
        Movimento.Modalita = 11


        Dim Disciplinare As String = ""
        If Not IsNothing(Me.Cmb_Disciplinare) AndAlso Not IsNothing(Me.Cmb_Disciplinare.SelectedItem) Then
            Disciplinare = Me.Cmb_Disciplinare.SelectedItem.Value
        End If

        Dim Dpi_Cod As Integer = 0
        Dim Disciplinare_PubblicoPrivato As Integer = 0
        If Disciplinare = "0" OrElse Disciplinare = "" Then
            Dpi_Cod = "-1"
        Else
            Dim Array As String()
            Array = Split(Disciplinare, "/")
            Dpi_Cod = Array(0)
            If Array(4) IsNot Nothing Then
                Disciplinare_PubblicoPrivato = Array(4)
            End If
            If Cmb_Disciplinare.SelectedValue <> "0" AndAlso Not IsNothing(Cmb_Epoca) AndAlso Cmb_Epoca.Items.Count > 0 Then
                Modulo = CInt(Me.Cmb_Epoca.SelectedValue)
            End If
        End If

        Movimento.Num_Protocollo = Dpi_Cod
        Movimento.Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato
        Movimento.Extra_Int = Modulo

        Movimento.BaseCode = frm_BaseCode
        Movimento.TopCode = frm_TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

        '---------------------------------------------------
        '----- Acqua  
        '---------------------------------------------------

        Dim SuperficieTotaleCentro As Decimal = 0

        For i = 0 To Dt.Rows.Count - 1
            SuperficieTotaleCentro = SuperficieTotaleCentro + Dt.Rows(i).Item("sup_imp")
        Next

        Dim Acqua As Decimal = 0
        Dim AcquaTot As Decimal = 0
        If Me.Txt_Acqua_Difesa.Text <> "" Then
            'se è stata scelta la dose d'acqua/ha salvo il dato negativo
            Select Case Me.RBL_Acqua_Formulati.SelectedValue
                Case "0"
                    Acqua = CDbl(Me.Txt_Acqua_Difesa.Text)
                    AcquaTot = Acqua
                Case "1"
                    Acqua = -CDbl(Me.Txt_Acqua_Difesa.Text)
                    AcquaTot = Math.Abs(Acqua) * SuperficieTotaleCentro
            End Select
        End If

        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio_Tecnico.Data = CDate(Me.Txt_Data_Interventi.Text)
        Movimento_Dettaglio_Tecnico.Qta_Ril = CDbl(Acqua)
        Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
        Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Dim FrCod As Integer
        Dim UdmCod As Integer = 0
        Dim UdmCodTrasformato As Integer = 0
        Dim Dose As Decimal = 0
        Dim DoseTrasformata As Decimal = 0
        Dim Carenza As Integer = 0
        Dim Dose_Etichetta As String = ""

        'Per ciascuna dose impostata ...
        For i = 0 To GridView_Dosi_Difesa.Rows.Count - 1

            FrCod = CInt(GridView_Dosi_Difesa.Rows(i).Cells(7).Text)
            Dose = CDbl(GridView_Dosi_Difesa.Rows(i).Cells(14).Text)
            UdmCod = CInt(GridView_Dosi_Difesa.Rows(i).Cells(13).Text)
            'dati non gestiti nelle ricette (solo visualizzate in insert)
            Carenza = CInt(GridView_Dosi_Difesa.Rows(i).Cells(9).Text)
            Dose_Etichetta = GridView_Dosi_Difesa.Rows(i).Cells(10).Text

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = Agenda.Data
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_TRATTAMENTO

            Movimento_Dettaglio.Elem_Cod = FORMULATI
            Movimento_Dettaglio.Pro_Cod = FrCod
            Movimento_Dettaglio.Mat_Cod = 0

            Movimento_Dettaglio.TempoCarenza = Carenza
            Movimento_Dettaglio.DoseEtichetta = Dose_Etichetta

            Select Case UdmCod
                Case 3  'g
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose / 1000  'caso in cui ho i grammi
                Case 2032 'mg
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose / 1000000  'caso in cui ho i grammi
                Case 4  'q
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose * 100  'caso in cui ho i quintali
                Case 304 't
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose * 1000   'caso in cui ho le tonnelate
                Case 101 'ml
                    UdmCodTrasformato = 29
                    DoseTrasformata = Dose / 1000  'caso in cui ho i ml
                Case 104 'cc
                    UdmCodTrasformato = 29
                    DoseTrasformata = Dose / 100  'caso in cui ho i cc
                Case 2, 29 'kg,l
                    UdmCodTrasformato = UdmCod
                    DoseTrasformata = Dose
            End Select

            Movimento_Dettaglio.Extra_Int = UdmCod
            Movimento_Dettaglio.Udm_Cod = UdmCodTrasformato
            Movimento_Dettaglio.Qta = Dose

            Movimento_Dettaglio.BaseCode = frm_BaseCode
            Movimento_Dettaglio.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO
            '------------------------------------------------

            Dim Array_AvCod() As String
            Dim Array_AvGru() As String

            Dim Array_Soglia_Value() As String
            Dim Array_Soglia_Des() As String

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

            Array_AvCod = Split(GridView_Dosi_Difesa.Rows(i).Cells(2).Text, ",")
            Array_AvGru = Split(GridView_Dosi_Difesa.Rows(i).Cells(3).Text, ",")

            Array_Soglia_Value = Split(GridView_Dosi_Difesa.Rows(i).Cells(5).Text, ",")
            Array_Soglia_Des = Split(GridView_Dosi_Difesa.Rows(i).Cells(6).Text, ",")

            If Not IsNothing(Array_AvCod) Then

                For j = 0 To UBound(Array_AvCod)

                    Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                    Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                    Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                    Movimento_Dettaglio_Tecnico.Data = Agenda.Data

                    Movimento_Dettaglio_Tecnico.Av_Cod = Array_AvCod(j)
                    Movimento_Dettaglio_Tecnico.Av_Gru = Array_AvGru(j)

                    If Array_Soglia_Value(j) <> "" Then
                        Dim Soglia() As String = Split(Array_Soglia_Value(j), "_")
                        If Soglia IsNot Nothing AndAlso Soglia.Length = 3 Then
                            Movimento_Dettaglio_Tecnico.Soglia_Quantita = Soglia(1)
                            Movimento_Dettaglio_Tecnico.Soglia_Cod = Soglia(2)
                        End If
                    End If
                    If Movimento_Dettaglio_Tecnico.Soglia_Cod <> 0 Then
                        Movimento_Dettaglio_Tecnico.Soglia_Des = Array_Soglia_Des(j)
                    End If


                    Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
                    Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                Next

            End If

            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI
            '------------------------------------------------
            Dim Sup_Imp As Decimal = 0
            Dim Frazione As Decimal = 0
            Dim QuantitaTotale As Decimal = 0

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            For j = 0 To GridView_Impianti.Rows.Count - 1

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Piva = GridView_Impianti.Rows(j).Cells(1).Text
                Movimento_Destinazione.Sa_Cod = CInt(GridView_Impianti.Rows(j).Cells(2).Text)
                Movimento_Destinazione.Appezza = CInt(GridView_Impianti.Rows(j).Cells(4).Text)
                Movimento_Destinazione.Id_Destinazione = CInt(GridView_Impianti.Rows(j).Cells(5).Text)
                Movimento_Destinazione.Tipo = 0

                Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
                Movimento_Destinazione.Qta2 = Sup_Imp

                '0=HL 1=HA
                If Movimento.Mezzo = 0 Then
                    QuantitaTotale = DoseTrasformata * AcquaTot
                Else
                    QuantitaTotale = DoseTrasformata * SuperficieTotaleCentro
                End If

                Frazione = Sup_Imp / SuperficieTotaleCentro

                Movimento_Destinazione.Qta = QuantitaTotale * Frazione

                Movimento_Destinazione.BaseCode = frm_BaseCode
                Movimento_Destinazione.TopCode = frm_TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            Next


        Next

        Return Agenda

    End Function


    '#####################################################################################################
    'utilizzata x il salvataggio di operazioni su impianti di imprese diverse..
    'in questo caso infatti salvo 1operazione x ogni centro, naturalmente sistemando le dosi
    '(x ora nn gestisco il magazzino)
    Private Function Genera_Agenda_Concimazione(ByVal Lav_Cod As Integer,
                                                          ByVal Dt As DataTable,
                                                          ByRef strErr As String) _
                                                          As Operazione_Agenda


        Dim Piva As String
        Dim SaCod As Integer

        Piva = Dt.Rows(0).Item("piva")
        SaCod = Dt.Rows(0).Item("sa_cod")


        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = CDate(Me.Txt_Data_Interventi.Text)
        Agenda.Piva = Piva
        Agenda.Sa_Cod = SaCod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = ComboOperazione.Testo_Combo & " (" & Cmb_Specie.SelectedItem.Text & ")"

        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        For i = 0 To CBL_Consigli_Concimazione.Items.Count - 1
            If CBL_Consigli_Concimazione.Items(i).Selected Then
                Nota = New Nota
                Nota.Nota_Cod = CBL_Consigli_Concimazione.Items(i).Value
                Agenda.Note.Add(Nota)
            End If
        Next

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------

        Movimento = New Movimento

        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = CDate(Me.Txt_Data_Interventi.Text)
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_LAVORAZIONE
        Movimento.Mov_Desc = Txt_Note_Concimazione.Text
        '0=HL 1=HA
        Movimento.Mezzo = CInt(Rbl_Dosi_Fertilizzanti.SelectedValue)
        '11=dose (10=qta tot non contemplata)
        Movimento.Modalita = 11

        'epoca
        If Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione) AndAlso
            Not IsNothing(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue) AndAlso
            ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue <> "" Then
            Movimento.Extra_Int = CInt(ComboEpoche.ddl_ComboEpocheFertilizzazione.SelectedValue)
        End If

        Movimento.BaseCode = frm_BaseCode
        Movimento.TopCode = frm_TopCode

        Agenda.Movimenti.Add(Movimento)

        Dim SuperficieTotaleCentro As Decimal = 0

        For i = 0 To Dt.Rows.Count - 1
            SuperficieTotaleCentro = SuperficieTotaleCentro + Dt.Rows(i).Item("sup_imp")
        Next

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
        '------------------------------------------------

        Dim Acqua As Decimal = 0
        Dim AcquaTot As Decimal = 0

        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                If Me.Txt_Acqua_Concimazione.Text <> "" Then
                    'se è stata scelta la dose d'acqua/ha salvo il dato negativo
                    Select Case Me.RBL_Acqua_Concimazione.SelectedValue
                        Case "0"
                            Acqua = CDbl(Me.Txt_Acqua_Concimazione.Text)
                            AcquaTot = Acqua
                        Case "1"
                            Acqua = -CDbl(Me.Txt_Acqua_Concimazione.Text)
                            AcquaTot = Math.Abs(Acqua) * SuperficieTotaleCentro
                    End Select
                End If

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

                Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio_Tecnico.Data = CDate(Me.Txt_Data_Interventi.Text)
                Movimento_Dettaglio_Tecnico.Qta_Ril = CDbl(Acqua)
                Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
                Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

        End Select

        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Dim FerCod As Integer
        Dim UdmCod As Integer = 0
        Dim UdmCodTrasformato As Integer = 0
        Dim Dose As Decimal = 0
        Dim DoseTrasformata As Decimal = 0

        'Per ciascuna dose impostata ...
        For i = 0 To GridView_Dosi_Concimazione.Rows.Count - 1

            FerCod = CInt(GridView_Dosi_Concimazione.Rows(i).Cells(2).Text)
            Dose = CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(12).Text)
            UdmCod = CInt(GridView_Dosi_Concimazione.Rows(i).Cells(11).Text)

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = Agenda.Data
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_LAVORAZIONE

            Movimento_Dettaglio.Elem_Cod = FERTILIZZANTI
            Movimento_Dettaglio.Pro_Cod = FerCod
            Movimento_Dettaglio.Mat_Cod = 0

            Select Case UdmCod
                Case 3  'g
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose / 1000  'caso in cui ho i grammi
                Case 2032 'mg
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose / 1000000  'caso in cui ho i grammi
                Case 4  'q
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose * 100  'caso in cui ho i quintali
                Case 304 't
                    UdmCodTrasformato = 2
                    DoseTrasformata = Dose * 1000   'caso in cui ho le tonnelate
                Case 101 'ml
                    UdmCodTrasformato = 29
                    DoseTrasformata = Dose / 1000  'caso in cui ho i ml
                Case 104 'cc
                    UdmCodTrasformato = 29
                    DoseTrasformata = Dose / 100  'caso in cui ho i cc
                Case 2, 29 'kg,l
                    UdmCodTrasformato = UdmCod
                    DoseTrasformata = Dose
            End Select

            Movimento_Dettaglio.Extra_Int = UdmCod
            Movimento_Dettaglio.Udm_Cod = UdmCodTrasformato
            Movimento_Dettaglio.Qta = Dose

            Movimento_Dettaglio.BaseCode = frm_BaseCode
            Movimento_Dettaglio.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO NPK
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

            Dim Efficienza As Decimal = 0
            Dim N As Decimal = 0
            Dim P As Decimal = 0
            Dim K As Decimal = 0
            Dim M As Decimal = 0

            Efficienza = CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(4).Text)
            N = CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(5).Text)
            P = CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(7).Text)
            K = CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(8).Text)
            M = CDbl(GridView_Dosi_Concimazione.Rows(i).Cells(9).Text)

            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

            Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
            Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio_Tecnico.Data = Agenda.Data

            Movimento_Dettaglio_Tecnico.Efficienza = Efficienza
            Movimento_Dettaglio_Tecnico.N = N
            Movimento_Dettaglio_Tecnico.P = P
            Movimento_Dettaglio_Tecnico.K = K
            Movimento_Dettaglio_Tecnico.M = M

            Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
            Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)


            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI
            '------------------------------------------------
            Dim Sup_Imp As Decimal = 0
            Dim Frazione As Decimal = 0
            Dim QuantitaTotale As Decimal = 0

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            For j = 0 To GridView_Impianti.Rows.Count - 1

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Piva = GridView_Impianti.Rows(j).Cells(1).Text
                Movimento_Destinazione.Sa_Cod = CInt(GridView_Impianti.Rows(j).Cells(2).Text)
                Movimento_Destinazione.Appezza = CInt(GridView_Impianti.Rows(j).Cells(4).Text)
                Movimento_Destinazione.Id_Destinazione = CInt(GridView_Impianti.Rows(j).Cells(5).Text)
                Movimento_Destinazione.Tipo = 0

                Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
                Movimento_Destinazione.Qta2 = Sup_Imp

                '0=HL 1=HA
                If Movimento.Mezzo = 0 Then
                    QuantitaTotale = DoseTrasformata * AcquaTot
                Else
                    QuantitaTotale = DoseTrasformata * SuperficieTotaleCentro
                End If

                Frazione = Sup_Imp / SuperficieTotaleCentro

                Movimento_Destinazione.Qta = QuantitaTotale * Frazione

                Movimento_Destinazione.BaseCode = frm_BaseCode
                Movimento_Destinazione.TopCode = frm_TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            Next


        Next

        Return Agenda

    End Function

    '#####################################################################################################
    'utilizzata x il salvataggio di operazioni su impianti di imprese diverse..
    'in questo caso infatti salvo 1operazione x ogni centro, naturalmente sistemando le dosi
    '(x ora nn gestisco il magazzino)
    Private Function Genera_Agenda_Irrigazione(ByVal Lav_Cod As Integer,
                                                          ByVal Dt As DataTable,
                                                          ByRef strErr As String) _
                                                          As Operazione_Agenda


        Dim Piva As String
        Dim SaCod As Integer

        Piva = Dt.Rows(0).Item("piva")
        SaCod = Dt.Rows(0).Item("sa_cod")

        Dim Dose As Decimal = 0
        Dim Udm_Cod As Integer = 0
        Dim Frequenza As Integer = 0
        Dim InizioMicro As Date = AGRODATAINIZIO
        Dim FineMicro As Date = AGRODATAFINE
        Dim Sup_Imp As Decimal = 0
        Dim TipoImpianto As Integer = 0

        If Not IsNumeric(Txt_Dose_Irrigazione.Text) Then
            Messaggi.AgroMsgBox("Inserire la Dose!", Page, , Script_Panel)
            Exit Function
        Else
            Dose = CDbl(Txt_Dose_Irrigazione.Text)
        End If
        Udm_Cod = Cmb_Udm_Irrigazione.SelectedItem.Value
        TipoImpianto = Cmb_ModifTipoIrrig.SelectedItem.Value
        If IsNumeric(Txt_Frequenza_Irrigazione.Text) Then
            Frequenza = Math.Round(CDbl(Txt_Frequenza_Irrigazione.Text), 0)
        End If
        If IsDate(Txt_DataInizio_Irrigazione.Text) Then
            InizioMicro = CDate(Txt_DataInizio_Irrigazione.Text)
        End If
        If IsDate(Txt_DataFine_Irrigazione.Text) Then
            FineMicro = CDate(Txt_DataFine_Irrigazione.Text)
        End If
        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = CDate(Me.Txt_Data_Interventi.Text)
        Agenda.Piva = Piva
        Agenda.Sa_Cod = SaCod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = ComboOperazione.Testo_Combo & " (" & Cmb_Specie.SelectedItem.Text & ")"

        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        For i = 0 To CBL_Consigli_Irrigazione.Items.Count - 1
            If CBL_Consigli_Irrigazione.Items(i).Selected Then
                Nota = New Nota
                Nota.Nota_Cod = CBL_Consigli_Irrigazione.Items(i).Value
                Agenda.Note.Add(Nota)
            End If
        Next

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------

        Movimento = New Movimento

        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = CDate(Me.Txt_Data_Interventi.Text)
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_LAVORAZIONE
        Movimento.Mov_Desc = Txt_Note_Irrigazione.Text

        Movimento.BaseCode = frm_BaseCode
        Movimento.TopCode = frm_TopCode

        Agenda.Movimenti.Add(Movimento)

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        For j = 0 To GridView_Impianti.Rows.Count - 1

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO
            '------------------------------------------------

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = Agenda.Data
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_LAVORAZIONE

            Movimento_Dettaglio.BaseCode = frm_BaseCode
            Movimento_Dettaglio.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

            '------------------------------------------------
            '----- MOVIMENTO DESTINAZIONE
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            Movimento_Destinazione = New Movimento_Destinazione

            Movimento_Destinazione.Data = Agenda.Data
            Movimento_Destinazione.Piva = GridView_Impianti.Rows(j).Cells(1).Text
            Movimento_Destinazione.Sa_Cod = CInt(GridView_Impianti.Rows(j).Cells(2).Text)
            Movimento_Destinazione.Appezza = CInt(GridView_Impianti.Rows(j).Cells(4).Text)
            Movimento_Destinazione.Id_Destinazione = CInt(GridView_Impianti.Rows(j).Cells(5).Text)
            Movimento_Destinazione.Tipo = 0
            Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
            Movimento_Destinazione.Qta2 = Sup_Imp
            Movimento_Destinazione.Qta = Dose

            Movimento_Destinazione.BaseCode = frm_BaseCode
            Movimento_Destinazione.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

            Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
            Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio_Tecnico.Data = Agenda.Data

            Movimento_Dettaglio_Tecnico.Qta_Ril = Dose
            Movimento_Dettaglio_Tecnico.dett_cod = Udm_Cod
            Movimento_Dettaglio_Tecnico.Nitrati = Frequenza
            Movimento_Dettaglio_Tecnico.Inn1_data = InizioMicro
            Movimento_Dettaglio_Tecnico.Inn2_data = FineMicro
            Movimento_Dettaglio_Tecnico.Freatimetro = TipoImpianto

            Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
            Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

        Next

        Return Agenda

    End Function

    '#####################################################################################################
    'utilizzata x il salvataggio di operazioni su impianti di imprese diverse..
    'in questo caso infatti salvo 1operazione x ogni centro, naturalmente sistemando le dosi
    '(x ora nn gestisco il magazzino)
    Private Function Genera_Agenda_InstallazioneTrappole(ByVal Lav_Cod As Integer,
                                                          ByVal Dt As DataTable,
                                                          ByRef strErr As String) _
                                                          As Operazione_Agenda


        Dim Piva As String
        Dim SaCod As Integer

        Piva = Dt.Rows(0).Item("piva")
        SaCod = Dt.Rows(0).Item("sa_cod")

        Dim Sup_Imp As Decimal = 0
        Dim QuantitaHa As Decimal = 0
        Dim QuantitaImp As Decimal = 0
        Dim QuantitaTot As Decimal = 0
        Dim QuantitaTotImp As Decimal = 0

        If Not IsNumeric(Txt_NumeroTrappole.Text) Then
            Messaggi.AgroMsgBox("Inserire il numero!", Page, , Script_Panel)
            Exit Function
        Else
            QuantitaHa = CDbl(Txt_NumeroTrappole.Text)
        End If

        Dim SuperficieTotaleCentro As Decimal = 0

        For i = 0 To Dt.Rows.Count - 1
            SuperficieTotaleCentro = SuperficieTotaleCentro + Dt.Rows(i).Item("sup_imp")
        Next
        QuantitaTot = Math.Round(QuantitaHa * SuperficieTotaleCentro, 0)

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = CDate(Me.Txt_Data_Interventi.Text)
        Agenda.Piva = Piva
        Agenda.Sa_Cod = SaCod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = ComboOperazione.Testo_Combo & " (" & Cmb_Specie.SelectedItem.Text & ")"

        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        For i = 0 To CBL_Consigli_Trappole.Items.Count - 1
            If CBL_Consigli_Trappole.Items(i).Selected Then
                Nota = New Nota
                Nota.Nota_Cod = CBL_Consigli_Trappole.Items(i).Value
                Agenda.Note.Add(Nota)
            End If
        Next

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------

        Movimento = New Movimento

        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = CDate(Me.Txt_Data_Interventi.Text)
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_RILIEVO_CAMPO
        Movimento.Mov_Desc = Txt_Note_Trappole.Text

        Movimento.BaseCode = frm_BaseCode
        Movimento.TopCode = frm_TopCode

        Agenda.Movimenti.Add(Movimento)

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Dim N_Trappole As Integer = 0
        Dim N_Trappole_Tot As Integer = 0

        For j = 0 To GridView_Impianti.Rows.Count - 1

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO
            '------------------------------------------------

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = Agenda.Data
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_RILIEVO_CAMPO

            Movimento_Dettaglio.Elem_Cod = TRAPPOLE
            Movimento_Dettaglio.Pro_Cod = CInt(cmb_Trappola.SelectedValue)
            Movimento_Dettaglio.Mat_Cod = 0
            Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.Numero_Trappole
            Movimento_Dettaglio.Qta = QuantitaTot
            Movimento_Dettaglio.Pendente = enum_Pendenza.MovESENTE

            Movimento_Dettaglio.BaseCode = frm_BaseCode
            Movimento_Dettaglio.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

            '------------------------------------------------
            '----- MOVIMENTO DESTINAZIONE
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            Movimento_Destinazione = New Movimento_Destinazione

            Movimento_Destinazione.Data = Agenda.Data
            Movimento_Destinazione.Piva = GridView_Impianti.Rows(j).Cells(1).Text
            Movimento_Destinazione.Sa_Cod = CInt(GridView_Impianti.Rows(j).Cells(2).Text)
            Movimento_Destinazione.Appezza = CInt(GridView_Impianti.Rows(j).Cells(4).Text)
            Movimento_Destinazione.Id_Destinazione = CInt(GridView_Impianti.Rows(j).Cells(5).Text)
            Movimento_Destinazione.Tipo = 0
            Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
            Movimento_Destinazione.Qta2 = Sup_Imp
            QuantitaImp = Math.Round(QuantitaHa * Sup_Imp, 0)

            'N.B. Per l'ultimo appezzamento verifico quante trappole ho rimasto da distribuire
            '(problema legato agli arrotondamenti)
            If j = GridView_Impianti.Rows.Count - 1 Then
                QuantitaImp = QuantitaTot - QuantitaTotImp
            End If

            Movimento_Destinazione.Qta = QuantitaImp
            QuantitaTotImp += QuantitaImp

            Movimento_Destinazione.BaseCode = frm_BaseCode
            Movimento_Destinazione.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

            For N_Trappole = 1 To QuantitaImp

                Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio_Tecnico.Data = Agenda.Data

                Movimento_Dettaglio_Tecnico.Ditta_cod = CInt(cmb_Ditte.SelectedValue)
                Movimento_Dettaglio_Tecnico.Sigla_av = Txt_CodAvversita.Text
                Movimento_Dettaglio_Tecnico.Inn1_data = Agenda.Data
                Movimento_Dettaglio_Tecnico.Dose = 1

                Movimento_Dettaglio_Tecnico.Av_Cod = CInt(cmb_Avversita.SelectedValue)

                Movimento_Dettaglio_Tecnico.Trap_num = N_Trappole_Tot + 1
                N_Trappole_Tot += 1

                Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
                Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

            Next

        Next

        Return Agenda

    End Function

    '#####################################################################################################
    'utilizzata x il salvataggio di operazioni su impianti di imprese diverse..
    'in questo caso infatti salvo 1operazione x ogni centro, naturalmente sistemando le dosi
    '(x ora nn gestisco il magazzino)
    Private Function Genera_Agenda_ConfusioneDisorientamento(ByVal Lav_Cod As Integer,
                                                          ByVal Dt As DataTable,
                                                          ByRef strErr As String) _
                                                          As Operazione_Agenda


        Dim Piva As String
        Dim SaCod As Integer

        Piva = Dt.Rows(0).Item("piva")
        SaCod = Dt.Rows(0).Item("sa_cod")

        Dim QuantitaHa As Decimal = 0
        Dim QuantitaImp As Decimal = 0
        Dim QuantitaTot As Decimal = 0

        If Not IsNumeric(Txt_NumeroTrappole.Text) Then
            Messaggi.AgroMsgBox("Inserire il numero!", Page, , Script_Panel)
            Exit Function
        Else
            QuantitaHa = CDbl(Txt_NumeroTrappole.Text)
        End If

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = CDate(Me.Txt_Data_Interventi.Text)
        Agenda.Piva = Piva
        Agenda.Sa_Cod = SaCod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = ComboOperazione.Testo_Combo & " (" & Cmb_Specie.SelectedItem.Text & ")"

        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        For i = 0 To CBL_Consigli_Trappole.Items.Count - 1
            If CBL_Consigli_Trappole.Items(i).Selected Then
                Nota = New Nota
                Nota.Nota_Cod = CBL_Consigli_Trappole.Items(i).Value
                Agenda.Note.Add(Nota)
            End If
        Next

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------

        Movimento = New Movimento

        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = CDate(Me.Txt_Data_Interventi.Text)
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_TRATTAMENTO
        Movimento.Mov_Desc = Txt_Note_Trappole.Text

        Movimento.BaseCode = frm_BaseCode
        Movimento.TopCode = frm_TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Movimento_Dettaglio = New Movimento_Dettaglio

        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio.Data = Agenda.Data
        Movimento_Dettaglio.Lav_Cod = Lav_Cod
        Movimento_Dettaglio.Cau_Mov = CAU_TRATTAMENTO

        Movimento_Dettaglio.Elem_Cod = TRAPPOLE
        Movimento_Dettaglio.Pro_Cod = CInt(cmb_Trappola.SelectedValue)
        Movimento_Dettaglio.Mat_Cod = 0

        Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.Numero_Trappole
        Movimento_Dettaglio.Qta = QuantitaHa
        Movimento_Dettaglio.Pendente = enum_Pendenza.MovESENTE

        Movimento_Dettaglio.BaseCode = frm_BaseCode
        Movimento_Dettaglio.TopCode = frm_TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio_Tecnico.Data = Agenda.Data

        Movimento_Dettaglio_Tecnico.Ditta_cod = CInt(cmb_Ditte.SelectedValue)
        Movimento_Dettaglio_Tecnico.Sigla_av = Txt_CodAvversita.Text
        Movimento_Dettaglio_Tecnico.Inn1_data = Agenda.Data

        Movimento_Dettaglio_Tecnico.Av_Cod = CInt(cmb_Avversita.SelectedValue)

        Movimento_Dettaglio_Tecnico.BaseCode = frm_BaseCode
        Movimento_Dettaglio_Tecnico.TopCode = frm_TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

        '------------------------------------------------
        '----- MOVIMENTI DESTINAZIONI
        '------------------------------------------------
        Dim Sup_Imp As Decimal = 0

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

        For j = 0 To GridView_Impianti.Rows.Count - 1

            Movimento_Destinazione = New Movimento_Destinazione

            Movimento_Destinazione.Piva = GridView_Impianti.Rows(j).Cells(1).Text
            Movimento_Destinazione.Sa_Cod = CInt(GridView_Impianti.Rows(j).Cells(2).Text)
            Movimento_Destinazione.Appezza = CInt(GridView_Impianti.Rows(j).Cells(4).Text)
            Movimento_Destinazione.Id_Destinazione = CInt(GridView_Impianti.Rows(j).Cells(5).Text)
            Movimento_Destinazione.Tipo = 0

            Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
            Movimento_Destinazione.Qta2 = Sup_Imp
            QuantitaImp = Math.Round(QuantitaHa * Sup_Imp, 0)
            Movimento_Destinazione.Qta = QuantitaImp
            QuantitaTot += QuantitaImp

            Movimento_Destinazione.BaseCode = frm_BaseCode
            Movimento_Destinazione.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni.Add(Movimento_Destinazione)

        Next

        Movimento_Dettaglio.Qta = QuantitaTot

        Return Agenda

    End Function

    '#####################################################################################################
    'utilizzata x il salvataggio di operazioni su impianti di imprese diverse..
    'in questo caso infatti salvo 1operazione x ogni centro, naturalmente sistemando le dosi
    '(x ora nn gestisco il magazzino)
    Private Function Genera_Agenda_Lavorazione(ByVal Lav_Cod As Integer,
                                                ByVal Dt As DataTable,
                                                ByRef strErr As String) _
                                                As Operazione_Agenda


        Dim Piva As String
        Dim SaCod As Integer

        Piva = Dt.Rows(0).Item("piva")
        SaCod = Dt.Rows(0).Item("sa_cod")

        Dim Sup_Imp As Decimal = 0

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = CDate(Me.Txt_Data_Interventi.Text)
        Agenda.Piva = Piva
        Agenda.Sa_Cod = SaCod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = ComboOperazione.Testo_Combo & " (" & Cmb_Specie.SelectedItem.Text & ")"

        Agenda.BaseCode = frm_BaseCode
        Agenda.TopCode = frm_TopCode

        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        For i = 0 To CBL_Consigli_Lavorazioni.Items.Count - 1
            If CBL_Consigli_Lavorazioni.Items(i).Selected Then
                Nota = New Nota
                Nota.Nota_Cod = CBL_Consigli_Lavorazioni.Items(i).Value
                Agenda.Note.Add(Nota)
            End If
        Next

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------

        Movimento = New Movimento

        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = CDate(Me.Txt_Data_Interventi.Text)
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_LAVORAZIONE
        Movimento.Mov_Desc = Txt_Note_Lavorazioni.Text

        Movimento.BaseCode = frm_BaseCode
        Movimento.TopCode = frm_TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Movimento_Dettaglio = New Movimento_Dettaglio

        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio.Data = Agenda.Data
        Movimento_Dettaglio.Lav_Cod = Lav_Cod
        Movimento_Dettaglio.Cau_Mov = CAU_LAVORAZIONE

        Dim Elem_Cod As Integer = 0
        Dim Pro_Cod As Integer = 0
        Select Case Lav_Cod
            Case LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_TRAPIANTO, LAVCOD_SOD_SEDDING
                Elem_Cod = SEMENTI
                Dim objTipologia As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R
                Dim DtTipologia As DataTable
                DtTipologia = objTipologia.Leggi(CInt(Cmb_Specie.SelectedValue), 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)
                If DtTipologia IsNot Nothing AndAlso DtTipologia.Rows.Count > 0 Then
                    Pro_Cod = DtTipologia.Rows(0).Item("sem_cod")
                End If
        End Select
        Movimento_Dettaglio.Elem_Cod = Elem_Cod
        Movimento_Dettaglio.Pro_Cod = Pro_Cod

        Movimento_Dettaglio.BaseCode = frm_BaseCode
        Movimento_Dettaglio.TopCode = frm_TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

        '------------------------------------------------
        '----- MOVIMENTI DESTINAZIONE
        '------------------------------------------------
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

        For j = 0 To GridView_Impianti.Rows.Count - 1

            Movimento_Destinazione = New Movimento_Destinazione

            Movimento_Destinazione.Data = Agenda.Data
            Movimento_Destinazione.Piva = GridView_Impianti.Rows(j).Cells(1).Text
            Movimento_Destinazione.Sa_Cod = CInt(GridView_Impianti.Rows(j).Cells(2).Text)
            Movimento_Destinazione.Appezza = CInt(GridView_Impianti.Rows(j).Cells(4).Text)
            Movimento_Destinazione.Id_Destinazione = CInt(GridView_Impianti.Rows(j).Cells(5).Text)
            Movimento_Destinazione.Tipo = 0
            Sup_Imp = CDbl(GridView_Impianti.Rows(j).Cells(17).Text)
            Movimento_Destinazione.Qta2 = Sup_Imp

            Movimento_Destinazione.BaseCode = frm_BaseCode
            Movimento_Destinazione.TopCode = frm_TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni.Add(Movimento_Destinazione)

        Next

        Return Agenda

    End Function

    Private Sub SalvaCostiAccessori_Agenda(ByRef Agenda As Operazione_Agenda)

        Dim DT As DataTable

        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then

            Dim i As Integer

            DT = CType(Session("dtScarico"), DataTable)

            'devo fare un movimento solo per ogni causale (al max 4)
            For i = -1 To -5 Step -1
                'filtro solo i movimenti di tipo ...
                Dim DR() As DataRow
                If i = -5 Then
                    DR = DT.Select("Centro_cod not in (-1,-2,-3,-4)")
                Else
                    DR = DT.Select("Centro_cod = " & i)
                End If
                If DR.Length > 0 Then
                    'ho qualche movimento da inserire 
                    Dim Cau_Mov As Integer
                    Select Case i
                        Case -1
                            Cau_Mov = CAU_IMPUTAZIONE_PARCOMACCHINE
                        Case -2
                            Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                        Case -3
                            Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
                        Case -4
                            Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                        Case Else
                            Cau_Mov = CAU_SCARICO
                    End Select

                    'creo un nuovo movimento 
                    Dim Movimento As New Movimento

                    Movimento.Piva = Agenda.Piva
                    Movimento.Sa_Cod = 0
                    Movimento.Data = Agenda.Data
                    Movimento.Lav_Cod = Agenda.Lav_Cod
                    Movimento.Cau_Mov = Cau_Mov
                    Movimento.Mov_Desc = Txt_Note_Concimazione.Text

                    Movimento.BaseCode = frm_BaseCode
                    Movimento.TopCode = frm_TopCode

                    Agenda.Movimenti.Add(Movimento)

                    Movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

                    'creo tanti dettagli quanti sono gli item di dr()

                    For j = 0 To DR.Length - 1

                        'creo un nuovo dettaglio
                        Dim Movimento_Dettaglio As New Movimento_Dettaglio

                        Movimento_Dettaglio.Piva = Agenda.Piva
                        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
                        Movimento_Dettaglio.Data = Agenda.Data
                        Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                        Movimento_Dettaglio.Cau_Mov = Cau_Mov

                        Movimento_Dettaglio.Elem_Cod = DR(j).Item("Elem_Cod")
                        Movimento_Dettaglio.Pro_Cod = 0
                        Movimento_Dettaglio.Mat_Cod = DR(j).Item("Mat_cod")


                        Movimento_Dettaglio.Extra_Int = 0
                        Movimento_Dettaglio.Udm_Cod = DR(j).Item("Udm_Cod")
                        Movimento_Dettaglio.Qta = 0

                        Movimento_Dettaglio.BaseCode = frm_BaseCode
                        Movimento_Dettaglio.TopCode = frm_TopCode

                        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                    Next

                End If
            Next

        End If

    End Sub
#End Region




#Region "Costi Accessori"

    '##########################################################################################

    'Public Sub CaricaCostiAccessori()
    '    'il DT viene salvato nel Session
    '    Session("dtScarico") = Nothing
    '    CaricaObjParametri()
    '    Costruisci_DT_Scarico()
    '    Datatable_from_MovimentiCostiAccessori()
    '    AggiornaGridViewCostiAccessoriVisibili()
    'End Sub

    Private Sub AggiornaGridViewCostiAccessoriVisibili()
        If Not IsNothing(Session("dtScarico")) Then
            GridViewCostiAccessoriVisibili.DataSource = Session("dtScarico")
            GridViewCostiAccessoriVisibili.DataBind()
            ControllaDDLCostiAccessori(GridViewCostiAccessoriVisibili)
        End If
    End Sub

    Private Sub AggiornaDgrScarico()
        If Not IsNothing(Session("dtScarico")) Then
            dgrScarico.DataSource = Session("dtScarico")
            dgrScarico.DataBind()
            'dopo aver fatto il bind verifico come impostare la dropdown
            ControllaDDLCostiAccessori(dgrScarico)
        End If
    End Sub

    Private Sub ControllaDDLCostiAccessori(ByVal Griglia As GridView)
        Dim i As Integer
        Dim dt As DataTable = Session("dtScarico")
        For i = 0 To Griglia.Rows.Count - 1
            CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Clear()
            If dt.Rows(i).Item("Centro_cod") < 0 Then
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("Ora", "2"))
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("Ha", "1"))
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("Indefinito", "-1"))
            Else
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("Kg", "2"))
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("Litri", "29"))
            End If

            'controllo se l'utente ha cambiato valore
            If Not IsDBNull(dt.Rows(i).Item("Udm_Selezionata")) AndAlso dt.Rows(i).Item("Udm_Selezionata") <> "" Then
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue = dt.Rows(i).Item("Udm_Selezionata")
            Else
                Dim j As Integer
                For j = 0 To CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Count - 1
                    If CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items(j).Text = dt.Rows(i).Item("Udm_Des") Then
                        CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedIndex = j
                        Exit For
                    End If
                Next
            End If

            'If CInt(dt.Rows(i).Item("Costo_Unitario")) <> 0 Then
            '    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Enabled = False
            'End If

            'txt selezionata
            If Not IsDBNull(dt.Rows(i).Item("Qta_Ril")) AndAlso dt.Rows(i).Item("Qta_Ril") <> "0.0" Then
                CType(Griglia.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text = dt.Rows(i).Item("Qta_Ril")
            End If
        Next

    End Sub

    'click sul bottone dei costi accessori
    Protected Sub AggiornaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AggiornaCostiAccessori.Click

        tabellaCostiAccessori.Visible = True

        Dim dummylist As DataTable
        If Not IsNothing(Session("dtScarico")) Then
            dummylist = Session("dtScarico")
            AggiornaDgrScarico()

            For i As Integer = 0 To dgrScarico.Rows.Count - 1
                dummylist.Rows(i).Item("Udm_Selezionata") = CType(dgrScarico.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue
                dummylist.Rows(i).Item("Valore") = CType(dgrScarico.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text
            Next

            Session("dtScarico") = dummylist
        End If

        Crea_Griglia_CentriCosto()
        'Datatable_from_MovimentiCostiAccessori()
        AggiornaDgrScarico()
        Session("dtScarico_old") = Session("dtScarico")

    End Sub

    ''' <summary>
    ''' salvataggio dei costi accessori
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub SalvaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SalvaCostiAccessori.Click

        AggiornaGridViewCostiAccessoriVisibili()
        'SalvaCostiAccessori_SuAgendaMovimenti(True)

    End Sub

    Protected Sub AnnullaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AnnullaCostiAccessori.Click
        Session("dtScarico") = Session("dtScarico_old")
        AggiornaGridViewCostiAccessoriVisibili()
    End Sub


    Private Sub Crea_Griglia_CentriCosto()

        'Aggiungo al datagrid dei centri di costo i magazzini
        Dim objDTableCentriCosto As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        objDTableCentriCosto = objFabbricati.LeggixCostiAccessori(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod,
                                                                  objParametriAgenda.Data,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                   "", "", objParametri_Server)
        objFabbricati = Nothing

        Dim Dr As DataRow

        'Aggiungo al datagrid la riga del parco macchine
        Dr = objDTableCentriCosto.NewRow

        Dr.Item(0) = "Parco Macchine"
        Dr.Item(1) = -1 'Fabbricato_Cod = -1 corrisponde al Parco Macchine

        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la riga della manodopera
        Dr = objDTableCentriCosto.NewRow

        Dr.Item(0) = "Manodopera"
        Dr.Item(1) = -2 'Fabbricato_Cod = -2 corrisponde alla Manodopera

        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la riga del parco macchine
        Dr = objDTableCentriCosto.NewRow

        Dr.Item(0) = "C/Terzisti"
        Dr.Item(1) = -3 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti


        Dr = objDTableCentriCosto.NewRow

        Dr.Item(0) = "Tecnico Responsabile"
        Dr.Item(1) = -4 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti

        objDTableCentriCosto.Rows.Add(Dr)

        'metto il DT all'interno del Session
        Session("DT_CentriCosto") = objDTableCentriCosto

        Me.dgrCentriCosto.DataSource = objDTableCentriCosto
        Me.dgrCentriCosto.DataBind()


    End Sub

    Private Sub dgrCentriCosto_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dgrCentriCosto.SelectedIndexChanged

        AggiornaCostiAccessori_Click(Me, Nothing)


        Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
        Dim Dt As DataTable

        Select Case CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))

            Case Is > 0

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMateriali.Columns(1).HeaderText = "Categoria Prodotto"
                Me.dgrMateriali.Columns(2).HeaderText = "Prodotto"
                Me.dgrMateriali.Columns(3).HeaderText = "Giacenza"

                If Not IsNothing(Session("Dt_Prodotti")) Then
                    Dt = Session("Dt_Prodotti")
                Else
                    Popola_ProdottiMagazzino(CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod")),
                                             objParametriAgenda.Elem_Cod)
                    Dt = Session("Dt_Prodotti")
                End If

                Me.dgrMateriali.DataSource = Dt
                Me.dgrMateriali.DataBind()

            Case -1

                'Cambio l'intestazione delle colonne nel datagrid
                dgrMateriali.Columns(1).HeaderText = "Macchina/Attrezzatura"
                dgrMateriali.Columns(2).HeaderText = "Descrizione"
                dgrMateriali.Columns(3).HeaderText = "Unità di Misura"

                If Not IsNothing(Session("Dt_Macchine")) Then
                    Dt = Session("Dt_Macchine")
                Else
                    Popola_ParcoMacchine()
                    Dt = Session("Dt_Macchine")
                End If

                Me.dgrMateriali.DataSource = Dt
                Me.dgrMateriali.DataBind()

            Case -2

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMateriali.Columns(1).HeaderText = "Rapporto Contabile"
                Me.dgrMateriali.Columns(2).HeaderText = "Manodopera"
                Me.dgrMateriali.Columns(3).HeaderText = "Unità di Misura"

                If Not IsNothing(Session("Dt_Manodopera")) Then
                    Dt = Session("Dt_Manodopera")
                Else
                    Popola_Manodopera()
                    Dt = Session("Dt_Manodopera")
                End If

                Me.dgrMateriali.DataSource = Dt
                Me.dgrMateriali.DataBind()

            Case -3

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMateriali.Columns(1).HeaderText = "Rapporto Contabile"
                Me.dgrMateriali.Columns(2).HeaderText = "Terzista"
                Me.dgrMateriali.Columns(3).HeaderText = "Unità di Misura"

                If Not IsNothing(Session("Dt_Terzisti")) Then
                    Dt = Session("Dt_Terzisti")
                Else
                    Popola_Terzisti()
                    Dt = Session("Dt_Terzisti")
                End If

                Me.dgrMateriali.DataSource = Dt
                Me.dgrMateriali.DataBind()


            Case -4

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMateriali.Columns(1).HeaderText = "Rapporto Contabile"
                Me.dgrMateriali.Columns(2).HeaderText = "Tecnico Responsabile"
                Me.dgrMateriali.Columns(3).HeaderText = "Unità di Misura"

                If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then
                    Dt = Session("Dt_TecnicoResponsabile")
                Else
                    Popola_TecnicoResponsabile()
                    Dt = Session("Dt_TecnicoResponsabile")
                End If

                Me.dgrMateriali.DataSource = Dt
                Me.dgrMateriali.DataBind()

        End Select

        ControllaDDLCostiAccessori(dgrScarico)
    End Sub

    Public Sub Popola_ProdottiMagazzino(ByVal Destinazione As Integer,
                                    ByVal Elem_Cod As Integer)


        Dim NomeTabella As String = ""
        Dim NomeCodice As String = ""
        Dim NomeDescrizione As String = ""
        Dim CategoriaProdotto As String = ""

        Dim objDt As DataTable
        Dim DT As DataTable
        Dim DTGiacenze As DataTable
        Dim DTProdotti As DataTable
        Dim DR As DataRow

        Dim strUdm_Des As String = ""

        Try

            objDt = Costruisci_Dt_Materiali()

            Dim filtro As String = ""
            If Elem_Cod = 197 Then
                filtro = " elem_cod <> " & CStr(Elem_Cod) & " AND elem_cod <> 198"
            Else
                filtro = " elem_cod <> " & CStr(Elem_Cod) & " "
            End If

            Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            'DTGiacenze = objGiacenze.LeggiGiacenze(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, _
            '                                       0, 0, 0, Destinazione, 0, 0, 0, "", filtro, "", _
            '                                       objParametri_Server)


            DTGiacenze = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino(
                                     AGRODATAFINE,
                                    objParametriAgenda.Piva, objParametriAgenda.Sa_Cod,
                                     Destinazione,
                                      0,
                                      0,
                                      0,
                                       0, 0, 0, 0,
                                      LOTTO_NONDEFINITO, False,
                                      filtro, "", "", "", "", "", "", "", "", "", "", "",
                                     objParametri_Server, objParametri_Utenti)




            objGiacenze = Nothing

            If DTGiacenze.Rows.Count > 0 Then

                'Faccio un filtro per evitare di inserire l'elem_cod che ho già usato nella lavorazione
                Dim i As Integer = 0
                Dim j As Integer = 0

                For i = 0 To DTGiacenze.Rows.Count - 1

                    DR = objDt.NewRow

                    'Leggo i prodotti associate al profilo selezionato			
                    Dim objCategorieMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                    DT = objCategorieMag.Leggi(DTGiacenze.Rows(i).Item("Elem_Cod"), 0, False,
                                        "",
                                        "",
                                         objParametri_Server)
                    objCategorieMag = Nothing

                    If DT.Rows.Count > 0 Then
                        NomeTabella = DT.Rows(0).Item("Tabella")
                        NomeCodice = DT.Rows(0).Item("Tabella_Cod")
                        NomeDescrizione = DT.Rows(0).Item("Tabella_Des")
                        CategoriaProdotto = DT.Rows(0).Item("NomeComune")
                    End If

                    Select Case CLng(DTGiacenze.Rows(i).Item("pro_cod"))

                        Case Is <> 0

                            Dim objProdotti As New AgronicaCoreContabDAL.Prodotti_R
                            DTProdotti = objProdotti.Leggi(CStr(NomeTabella),
                                                            CStr(NomeCodice),
                                                            CInt(DTGiacenze.Rows(i).Item("Pro_Cod")),
                                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                             "",
                                                             "",
                                                             objParametri_Server)
                            objProdotti = Nothing

                            For j = 0 To DTProdotti.Rows.Count - 1
                                DR("Col_0") = CategoriaProdotto
                                DR("Col_1") = DTProdotti.Rows(j).Item(NomeDescrizione)
                                Dim objConvert As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                DR("Col_2") = objConvert.UdmDes_from_UdmCod(DTGiacenze.Rows(i).Item("Udm_Cod"),
                                                                            strUdm_Des, objParametri_Server) & " " &
                                                                        Format(DTGiacenze.Rows(i).Item("Qta"), "0.00")
                                objConvert = Nothing
                                DR("Col_3") = DTGiacenze.Rows(i).Item("Pro_Cod")
                                DR("Col_4") = 0
                                DR("Col_5") = DTGiacenze.Rows(i).Item("Udm_Cod")
                                DR("Col_6") = DTGiacenze.Rows(i).Item("Elem_Cod")
                                DR("Col_7") = 0 'Ditta_Cod
                                DR("Col_8") = 0 'Mat_Cod
                                DR("Col_9") = Format(CDbl(DTGiacenze.Rows(i).Item("Prezzo_Unitario")), "0.00")
                            Next


                        Case 0

                            'Leggo le materie prime
                            Dim objProdottiAziendali As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                            DTProdotti = objProdottiAziendali.LeggiMateriePrimexSuperUser(
                                                 CStr(objParametri_Server.PivaSuperUser),
                                                 objParametriAgenda.Piva,
                                                 DTGiacenze.Rows(i).Item("Elem_Cod"),
                                                 DTGiacenze.Rows(i).Item("Mat_Cod"),
                                                 0,
                                                 "",
                                                 False,
                                                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                 "", "",
                                                 objParametri_Server)
                            objProdottiAziendali = Nothing

                            For j = 0 To DTProdotti.Rows.Count - 1
                                DR("Col_0") = CategoriaProdotto
                                DR("Col_1") = DTProdotti.Rows(j).Item("Mat_Des")
                                Dim objConvert As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                DR("Col_2") = objConvert.UdmDes_from_UdmCod(DTGiacenze.Rows(i).Item("Udm_Cod"),
                                                                            strUdm_Des, objParametri_Server) & " " &
                                                                        Format(DTGiacenze.Rows(i).Item("Qta"), "0.00")
                                objConvert = Nothing
                                DR("Col_3") = 0 'Pro_Cod
                                DR("Col_4") = 0
                                DR("Col_5") = DTGiacenze.Rows(i).Item("Udm_Cod")
                                DR("Col_6") = DTGiacenze.Rows(i).Item("Elem_Cod")
                                DR("Col_7") = 0 'Ditta_Cod
                                DR("Col_8") = DTProdotti.Rows(j).Item("Mat_Cod")
                                DR("Col_9") = Format(CDbl(DTGiacenze.Rows(i).Item("Prezzo_Unitario")), "0.00")

                            Next

                    End Select

                    objDt.Rows.Add(DR)

                Next

            End If

            Session("Dt_Prodotti") = objDt

        Catch ex As Exception
            Session("Dt_Prodotti") = Nothing
        End Try

    End Sub

    Private Sub Popola_ParcoMacchine()

        Dim i As Integer = 0
        Dim Mezzo As Integer = 0
        Dim Prezzo_Unitario As Decimal
        Dim Udm_Cod As Integer = 0
        Dim objDTableParcoMacchine As DataTable
        Dim Dr As DataRow

        Try

            objDTableParcoMacchine = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare le macchine del centro aziendale
            'Sa_Cod=-1 : Macchine di un altro centro a disposizione di tutti

            Dim objPM As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim Dt_Macchine As DataTable
            Dt_Macchine = objPM.LeggiMacchine_xCostiAccessori(objParametriAgenda.Piva,
                                                                           objParametriAgenda.Data,
                                                                           "", "", objParametri_Server)
            objPM = Nothing



            For i = 0 To Dt_Macchine.Rows.Count - 1

                Dr = objDTableParcoMacchine.NewRow()

                Dr.Item("Col_0") = Dt_Macchine.Rows(i).Item("Col_0")
                Dr.Item("Col_1") = Dt_Macchine.Rows(i).Item("Col_1")
                Dr.Item("Col_8") = Dt_Macchine.Rows(i).Item("Col_8")

                Dim objProdottiPrezzi As New AgronicaCoreContabDAL.Prodotti_Costi_R
                objProdottiPrezzi.Prezzo_from_Prodotto(Session("ASG_SuperUser_CodFiscale"),
                                     MACCHINE,
                                     0,
                                     Dr.Item("Col_8"),
                                     objParametriAgenda.Data,
                                     Mezzo,
                                     Udm_Cod,
                                     Prezzo_Unitario,
                                     "", objParametri_Server)

                objProdottiPrezzi = Nothing


                If IsDBNull(Mezzo) Then
                    Dr.Item("Col_2") = "indefinito"
                    Dr.Item("Col_5") = -1
                Else
                    Select Case Mezzo
                        Case TipiEnumerativi.enum_TipoMezzo.Ettaro
                            Dr.Item("Col_2") = "ha"
                            Dr.Item("Col_5") = "1"
                        Case TipiEnumerativi.enum_TipoMezzo.Ora
                            Dr.Item("Col_2") = "ora"
                            Dr.Item("Col_5") = "2"
                        Case TipiEnumerativi.enum_TipoMezzo.Indefinito
                            Dr.Item("Col_2") = "indefinito"
                            Dr.Item("Col_5") = "-1"
                    End Select
                End If




                Dr.Item("Col_9") = Prezzo_Unitario

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = MACCHINE
                Dr.Item("Col_7") = 0

                objDTableParcoMacchine.Rows.Add(Dr)

            Next

            Dt_Macchine = Nothing

            'Me.dgrMateriali.DataSource = objDTableParcoMacchine
            'Me.dgrMateriali.DataBind()

            Session("Dt_Macchine") = objDTableParcoMacchine



        Catch ex As Exception

            Session("Dt_Macchine") = Nothing
            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub

    Private Sub Popola_Manodopera()
        'Popola la griglia della "Manodpera"


        Dim i As Integer = 0
        Dim objDTableManodopera As DataTable
        Dim Dr As DataRow

        Try

            objDTableManodopera = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare tutti i dipendenti(manodopera)
            'Sa_Cod=-1 : manodopera di un altro centro a disposizione di tutti
            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim Dt_Manodopera As DataTable

            Dim Str As String = ""
            'Str = " ( (Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6))  or Rapporti_Contabili.Dipendente=1 ) "
            Str = " ( Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6) ) "

            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, "", objParametri_Server)
            objRapp_Contabili = Nothing

            'objDTableManodopera.Columns.Add("Col_0", System.Type.GetType("System.String"))  'rapporto contabile
            'objDTableManodopera.Columns.Add("Col_2", System.Type.GetType("System.String"))  'Udm_Des
            'objDTableManodopera.Columns.Add("Col_3", System.Type.GetType("System.Int32")) 'Pro_Cod
            'objDTableManodopera.Columns.Add("Col_4", System.Type.GetType("System.Decimal"))  'Qta_Ril 
            'objDTableManodopera.Columns.Add("Col_6", System.Type.GetType("System.Int32")) 'Elem_Cod
            'objDTableManodopera.Columns.Add("Col_7", System.Type.GetType("System.Int32")) 'Ditta_Cod



            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des")) &
                                  "  (Progressivo: " & CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) & ")"

                Dr.Item("Col_1") = Dt_Manodopera.Rows(i).Item("Col_1")
                Dr.Item("Col_8") = Dt_Manodopera.Rows(i).Item("Col_8")
                Dr.Item("Col_9") = Dt_Manodopera.Rows(i).Item("Col_9")
                Dr.Item("Col_5") = Dt_Manodopera.Rows(i).Item("Col_5")

                If Dr.Item("Col_5") = 2 Then
                    Dr.Item("Col_2") = "ora"
                Else
                    Dr.Item("Col_2") = "ha"
                End If

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = 0
                Dr.Item("Col_7") = 0

                objDTableManodopera.Rows.Add(Dr)

            Next

            Dt_Manodopera = Nothing

            'Me.dgrMateriali.DataSource = objDTableManodopera
            'Me.dgrMateriali.DataBind()
            'Format(Me.dgrMateriali.Columns.Item(9), "0.00")

            Session("Dt_Manodopera") = objDTableManodopera



        Catch ex As Exception

            Session("Dt_Manodopera") = Nothing
            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub

    Private Sub Popola_Terzisti()


        Dim i As Integer = 0
        Dim objDTableManodopera As DataTable
        Dim Dr As DataRow

        Try

            objDTableManodopera = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare tutti i dipendenti(manodopera)
            'Sa_Cod=-1 : manodopera di un altro centro a disposizione di tutti

            'Risorse_Umane.Settore_Des as Col_2

            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim Dt_Manodopera As DataTable
            Dim Str As String = ""
            Str = " ( (Rapporti_Contabili.Cod_Rapporto = -5)  or Rapporti_Contabili.Terzista=1 ) "
            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, "", objParametri_Server)
            objRapp_Contabili = Nothing



            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des")) &
                                  "  (Progressivo: " & CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) & ")"

                Dr.Item("Col_1") = Dt_Manodopera.Rows(i).Item("Col_1")
                Dr.Item("Col_8") = Dt_Manodopera.Rows(i).Item("Col_8")
                Dr.Item("Col_9") = Dt_Manodopera.Rows(i).Item("Col_9")
                Dr.Item("Col_5") = Dt_Manodopera.Rows(i).Item("Col_5")

                If Dr.Item("Col_5") = 2 Then
                    Dr.Item("Col_2") = "ora"
                Else
                    Dr.Item("Col_2") = "ha"
                End If

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = 0
                Dr.Item("Col_7") = 0

                objDTableManodopera.Rows.Add(Dr)

            Next

            Dt_Manodopera = Nothing

            'Me.dgrMateriali.DataSource = objDTableManodopera
            'Me.dgrMateriali.DataBind()

            Session("Dt_Terzisti") = objDTableManodopera


        Catch ex As Exception

            Session("Dt_Terzisti") = Nothing

            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub

    Private Sub Popola_TecnicoResponsabile()


        Dim i As Integer = 0
        Dim objDTableManodopera As DataTable
        Dim Dr As DataRow

        Try

            objDTableManodopera = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare tutti i dipendenti(manodopera)
            'Sa_Cod=-1 : manodopera di un altro centro a disposizione di tutti

            'Risorse_Umane.Settore_Des as Col_2

            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim Dt_Manodopera As DataTable
            Dim Str As String = ""
            Str = " (Rapporti_Contabili.Cod_Rapporto = -12) "
            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, "", objParametri_Server)
            objRapp_Contabili = Nothing



            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des")) &
                                  "  (Progressivo: " & CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) & ")"

                Dr.Item("Col_1") = Dt_Manodopera.Rows(i).Item("Col_1")
                Dr.Item("Col_8") = Dt_Manodopera.Rows(i).Item("Col_8")
                Dr.Item("Col_9") = Dt_Manodopera.Rows(i).Item("Col_9")
                Dr.Item("Col_5") = Dt_Manodopera.Rows(i).Item("Col_5")

                If Dr.Item("Col_5") = 2 Then
                    Dr.Item("Col_2") = "ora"
                Else
                    Dr.Item("Col_2") = "ha"
                End If

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = 0
                Dr.Item("Col_7") = 0

                objDTableManodopera.Rows.Add(Dr)

            Next

            Dt_Manodopera = Nothing

            'Me.dgrMateriali.DataSource = objDTableManodopera
            'Me.dgrMateriali.DataBind()

            Session("Dt_TecnicoResponsabile") = objDTableManodopera


        Catch ex As Exception

            Session("Dt_TecnicoResponsabile") = Nothing

            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub


    Private Sub Costruisci_DT_Scarico()
        Dim dt As DataTable
        dt = New DataTable("dtScarico")
        dt.Columns.Add("Centro", System.Type.GetType("System.String"))
        dt.Columns.Add("Centro_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Categoria_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Risorsa_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Udm_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Qta_Ril", System.Type.GetType("System.Decimal"))
        dt.Columns.Add("Udm_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Elem_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Riga", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Tipo_Centro", System.Type.GetType("System.String"))
        dt.Columns.Add("Pro_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Ditta_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Mat_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Costo_Unitario", System.Type.GetType("System.String"))
        dt.Columns.Add("Costo", System.Type.GetType("System.String"))

        'per gli input degli utenti
        dt.Columns.Add("Udm_Selezionata", System.Type.GetType("System.String"))
        dt.Columns.Add("Valore", System.Type.GetType("System.String"))

        Session("dtScarico") = dt
    End Sub

    Private Sub InserisciRiga_dtScarico(ByVal Centro As String,
                                ByVal Centro_Cod As Int32,
                                ByVal Categoria_Des As String,
                                ByVal Risorsa_Des As String,
                                ByVal Udm_Des As String,
                                ByVal Qta_Ril As Decimal,
                                ByVal Udm_Cod As Int32,
                                ByVal Elem_Cod As Int32,
                                ByVal Riga As Int32,
                                ByVal Tipo_Centro As String,
                                ByVal Pro_Cod As Int32,
                                ByVal Ditta_Cod As Int32,
                                ByVal Mat_Cod As Int32,
                                ByVal Costo_Unitario As String,
                                ByVal Costo As String,
                                ByVal DT As DataTable)



        Dim Dr As DataRow
        Dim ElementoPresente As Boolean
        Dim Messaggio As String

        '----- Verifico che il formulato non sia gia' presente nel datatable

        'Inizializzo
        ElementoPresente = False

        'Recupero il datatable


        'Ciclo nelle righe del datatable
        For IndiceRiga = 0 To DT.Rows.Count - 1

            If DT.Rows(IndiceRiga).Item("Centro_Cod") = Centro_Cod And
               DT.Rows(IndiceRiga).Item("Elem_Cod") = Elem_Cod And
               DT.Rows(IndiceRiga).Item("Pro_Cod") = Pro_Cod And
               DT.Rows(IndiceRiga).Item("Mat_Cod") = Mat_Cod Then
                ElementoPresente = True
            End If

        Next

        'Se esiste gia' allora esco
        If ElementoPresente Then
            Messaggio = "Non e' consentito inserire un elemento gia' presente"
            Messaggi.AgroMsgBox(Messaggio, Page, , Script_Panel)
            Exit Sub
        End If

        '----- Inserisco il nuovo record

        'Creo una nuova riga
        Dr = DT.NewRow

        'Definisco i valori

        Dr.Item("Centro") = Centro
        Dr.Item("Centro_Cod") = Centro_Cod
        Dr.Item("Categoria_Des") = Categoria_Des
        Dr.Item("Risorsa_Des") = Risorsa_Des
        Dr.Item("Udm_Des") = Udm_Des
        Dr.Item("Qta_Ril") = Qta_Ril
        Dr.Item("Udm_Cod") = Udm_Cod
        Dr.Item("Elem_Cod") = Elem_Cod
        Dr.Item("Riga") = Riga
        Dr.Item("Tipo_Centro") = Tipo_Centro
        Dr.Item("Pro_Cod") = Pro_Cod
        Dr.Item("Ditta_Cod") = Ditta_Cod
        Dr.Item("Mat_Cod") = Mat_Cod
        Dr.Item("Costo_Unitario") = Costo_Unitario
        Dr.Item("Costo") = Costo

        'Associo alla tabella la nuova riga creata
        DT.Rows.Add(Dr)


    End Sub

    Private Sub dgrScarico_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrScarico.RowCommand

        AggiornaCostiAccessori_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName

            Case "EliminaCosto"

                If Session("dtScarico") IsNot Nothing Then

                    Dim dtScarico As DataTable = Session("dtScarico")

                    If dtScarico.Rows.Count >= IndiceRigaGriglia Then

                        'Trovo la riga da cancellare    (chiave = FrCod)
                        Dr = dtScarico.Rows(IndiceRigaGriglia)

                        'Elimino la riga
                        Dr.Delete()
                        'Salvo il DataTable dentro il Session
                        Session("dtScarico") = dtScarico

                    End If

                End If

        End Select


        AggiornaDgrScarico()
        AggiornaGridViewCostiAccessoriVisibili()

        'se sono nella griglia pupup dei costidrg scarico (come in questo caso) non è necessario aggiornare anche la lista dei costi
        'prersente nei movimenti dell'agenda, dato che viene ricreata quando si seleziona salva_costi_accessori,
        'mentre se si seleziona annulla allora viene ricaricata in sessione la tabella old quindi 
        'i movimenti non devono essere toccati
        'se invece sono nella griglia a fondo pagina gridviewcostiaccessorivisibili allora se seleziono cancella 
        'oltre che a modificare la tabella dei costi in sessione Session("dtScarico") che non verrà mai ripristinata
        'dalla versione precedente, dato che l'operazione non è annullabile o confermabile, devo 
        'agire anche sulla lista dei movimenti eliminando il movimento 
        'corrispondente alla riga selezionata. Se non lo faccio non ho più corrispondenza tra la tabella in sessione e 
        'la lista movimenti e dato che la prima corrisponde di solito a ciò che vedo, mentre la seconda 
        'corrisponde a ciò che viene salvato mi trovo a salvare cose diverse da quello che vedo
    End Sub



    Private Function Costruisci_Dt_Materiali() As DataTable

        Dim Dt As New DataTable

        Dt.Columns.Add("Col_0", System.Type.GetType("System.String")) 'Categoria
        Dt.Columns.Add("Col_1", System.Type.GetType("System.String")) 'Descrizione
        Dt.Columns.Add("Col_2", System.Type.GetType("System.String")) 'Giacenza
        Dt.Columns.Add("Col_3", System.Type.GetType("System.Int32"))  'Pro_cod
        Dt.Columns.Add("Col_4", System.Type.GetType("System.Decimal")) 'Qta
        Dt.Columns.Add("Col_5", System.Type.GetType("System.Int32"))  'Udm_Cod
        Dt.Columns.Add("Col_6", System.Type.GetType("System.Int32"))  'Elem_Cod
        Dt.Columns.Add("Col_7", System.Type.GetType("System.Int32"))  'Ditta_Cod
        Dt.Columns.Add("Col_8", System.Type.GetType("System.Int32"))  'Mat_cod
        Dt.Columns.Add("Col_9", System.Type.GetType("System.String")) 'Prezzo_Unitario

        Return Dt

    End Function

    Private Sub dgrMateriali_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrMateriali.RowCommand


        AggiornaCostiAccessori_Click(Me, Nothing)



        Dim IndiceRigaGriglia As Integer = 0

        Dim Centro As String = ""
        Dim Centro_Cod As Int32
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""
        Dim Udm_Des As String = ""
        Dim Qta_Ril As Decimal
        Dim Udm_Cod As Int32
        Dim Elem_Cod As Int32
        Dim Riga As Int32
        Dim Tipo_Centro As String = ""
        Dim Pro_Cod As Int32
        Dim Ditta_Cod As Int32
        Dim Mat_Cod As Int32
        Dim Costo_Unitario As String = ""
        Dim Costo As String = ""

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Dim dtScarico As DataTable

        dtScarico = Session("dtScarico")

        Select Case e.CommandName

            Case "AggiungiCosto"

                If Session("DT_CentriCosto") IsNot Nothing Then

                    Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")

                    Select Case CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))

                        Case Is > 0

                            If Not IsNothing(Session("Dt_Prodotti")) Then

                                Dim Dt_Prodotti As DataTable = Session("Dt_Prodotti")

                                If Dt_Prodotti.Rows.Count >= IndiceRigaGriglia Then

                                    Centro = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(
                                                objParametriAgenda.Piva,
                                                objParametriAgenda.Sa_Cod,
                                                CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod")),
                                                objParametri_Server) 'Magazzino

                                    Tipo_Centro = "M"
                                    Centro_Cod = CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))

                                    Elem_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_6")
                                    Pro_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Categoria_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Udm_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -1

                            If Not IsNothing(Session("Dt_Macchine")) Then

                                Dim Dt_Macchine As DataTable = Session("Dt_Macchine")

                                If Dt_Macchine.Rows.Count >= IndiceRigaGriglia Then

                                    Centro = "Parco Macchine"
                                    Tipo_Centro = "PM"
                                    Centro_Cod = -1

                                    Categoria_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = MACCHINE
                                    Pro_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -2

                            If Not IsNothing(Session("Dt_Manodopera")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_Manodopera")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -3 'TERZISTI


                            If Not IsNothing(Session("Dt_Terzisti")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_Terzisti")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If



                        Case -4  'Tecnico Responsabile

                            If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_TecnicoResponsabile")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                    End Select

                    InserisciRiga_dtScarico(Centro,
                                            Centro_Cod,
                                            Categoria_Des,
                                            Risorsa_Des,
                                            Udm_Des,
                                            Qta_Ril,
                                            Udm_Cod,
                                            Elem_Cod,
                                            Riga,
                                            Tipo_Centro,
                                            Pro_Cod,
                                            Ditta_Cod,
                                            Mat_Cod,
                                            Costo_Unitario,
                                            Costo,
                                            dtScarico)


                End If

                'InserisciCosto(CType(dgrMateriali.Rows(IndiceRigaGriglia).FindControl("Col_4"), TextBox), Nothing)

        End Select

        Session("dtScarico") = dtScarico
        AggiornaDgrScarico()
        AggiornaGridViewCostiAccessoriVisibili()

    End Sub




    Private Sub GridViewCostiAccessoriVisibili_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewCostiAccessoriVisibili.RowCommand

        AggiornaCostiAccessori_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName

            Case "EliminaCosto"

                If Session("dtScarico") IsNot Nothing Then

                    Dim dtScarico As DataTable = Session("dtScarico")

                    If dtScarico.Rows.Count >= IndiceRigaGriglia Then


                        'Trovo la riga da cancellare    (chiave = FrCod)
                        Dr = dtScarico.Rows(IndiceRigaGriglia)

                        'Elimino la riga
                        Dr.Delete()
                        'Salvo il DataTable dentro il Session
                        Session("dtScarico") = dtScarico

                    End If

                End If

        End Select


        AggiornaDgrScarico()
        AggiornaGridViewCostiAccessoriVisibili()


        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        'Elimino dai movimenti
        'SalvaCostiAccessori_SuAgendaMovimenti(False)


    End Sub



    Private Sub DefaultAziendali(ByRef CBL_Consigli As System.Web.UI.WebControls.CheckBoxList,
                                 ByVal Lav_Cod As Integer,
                                 Optional ByVal Piva As String = "")

        'If CInt(Qs_Operazione) <> TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
        '    Exit Sub
        'End If

        Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R


        ' ''''''''''''''''''''''''''''''''''''''''
        ' ''''''''''' CARICO LE NOTE '''''''''''''
        ' ''''''''''''''''''''''''''''''''''''''''
        Dim DT_note As DataTable

        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
                                                False, "", "",
                                                 0, -1,
                                                 "", "",
                                                 objParametri_Server)

        Select Case Chk_RicettaPubblica.Checked
            Case True
                Piva = ""
            Case Else
                Piva = Qs_Piva
        End Select

        DT_note = objProfilazioneR.LeggiProfilazioneNote_In_Cascata(Piva,
                                         "-1",
                                         Lav_Cod,
                                         CInt(Cmb_Specie.SelectedValue),
                                         objParametri_Server)

        Dim i, j As Integer
        For i = 0 To DT_note.Rows.Count - 1
            For j = 0 To CBL_Consigli.Items.Count - 1

                If CBL_Consigli.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                    CBL_Consigli.Items(j).Selected = True
                    Exit For
                End If
            Next
        Next

        ''''''''''''''''''''''''''''''''''''''''
        ''''' CARICO MACCHINE E CONTATTI '''''''
        ''''''''''''''''''''''''''''''''''''''''
        Dim DS_Macchine As DataSet
        DS_Macchine = objProfilazioneR.LeggiProfilazioneMacchine_In_Cascata(Piva,
                                                 Lav_Cod, CInt(Cmb_Specie.SelectedValue),
                                                 objParametri_Server)


        'Creo le liste
        Dim listaMacCod As New List(Of String)
        Dim listaCodContatto As New List(Of String)

        For Each dtDati As DataTable In DS_Macchine.Tables
            For Each drDati In dtDati.Rows
                If (dtDati.TableName.Equals("macXlav")) Then
                    'cerco il check e lo seleziono
                    listaMacCod.Add(drDati("mac_cod"))
                ElseIf (dtDati.TableName.Equals("contXlav")) Then
                    listaCodContatto.Add(drDati("cod_risum"))
                End If

            Next
        Next

        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

        ' Dim objPrezzofromprodotti As New AgronicaCoreContabDAL.Prodotti_Costi_R

        Dim Udm_Cod As Integer = -1
        Dim Udm_Des As String = "Indefinito"
        Dim Costo_Unitario As String = "0"
        Dim Costo As String = "0"

        'PARCO MACCHINE
        For i = 0 To listaMacCod.Count - 1

            Dim Categoria_Des As String = ""
            Dim Risorsa_Des As String = ""

            'Dim Mezzo As Integer
            'Dim Prezzo As Decimal
            Dim Centro As String = "Parco Macchine"
            Dim Tipo_Centro As String = "PM"
            Dim Centro_Cod As Integer = -1


            'objPrezzofromprodotti.Prezzo_from_Prodotto(objParametri_Server.PivaSuperUser, _
            '                     1, _
            '                     0, _
            '                     listaMacCod(i), _
            '                     objParametriAgenda.Data, _
            '                     Mezzo, _
            '                     Udm_Cod, _
            '                     Prezzo, _
            '                     "", objParametri_Server)

            'If IsDBNull(Mezzo) Then
            '    Udm_Des = "Indefinito"
            '    Udm_Cod = "-1"
            'Else
            '    Select Case Mezzo
            '        Case TipiEnumerativi.enum_TipoMezzo.Ettaro
            '            Udm_Des = "Ha"
            '            Udm_Cod = "1"
            '        Case TipiEnumerativi.enum_TipoMezzo.Ora
            '            Udm_Des = "Ora"
            '            Udm_Cod = "2"
            '        Case TipiEnumerativi.enum_TipoMezzo.Indefinito
            '            Udm_Des = "Indefinito"
            '            Udm_Cod = "-1"
            '    End Select
            'End If


            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim DT = objContab.MacchinaDes(Piva,
                                       objParametriAgenda.Data,
                                       listaMacCod(i),
                                        "", "",
                                       objParametri_Server)
            objContab = Nothing


            If DT.Rows.Count > 0 Then

                Categoria_Des = DT.Rows(0).Item("Class_Desc")
                If DT.Rows(0).Item("Mac_Des") <> "" Then
                    Risorsa_Des = DT.Rows(0).Item("Mac_Des")
                Else
                    Risorsa_Des = DT.Rows(0).Item("Ditta_Des") & " " & DT.Rows(0).Item("Modello")
                End If

                'Costo_Unitario = Format(Prezzo * 1, "0.00")
                'Costo = Format(Prezzo * 0, "0.00")


                'verifico se ho l'acqua impostata 
                'If QtaAcqua.Value = "" Then
                '    If Not IsDBNull(DT.Rows(0).Item("Taratura_Ugello")) Then
                '        If IsNumeric(DT.Rows(0).Item("Taratura_Ugello")) Then
                '            Dim Acqua As Decimal
                '            Acqua = DT.Rows(0).Item("Taratura_Ugello")
                '            QtaAcqua.Value = Acqua
                '        End If
                '    End If
                'End If

                InserisciRiga_dtScarico(Centro,
                                                Centro_Cod,
                                                Categoria_Des,
                                                Risorsa_Des,
                                                Udm_Des,
                                                0,
                                                Udm_Cod,
                                                MACCHINE,
                                                0,
                                                Tipo_Centro,
                                                0,
                                                0,
                                                listaMacCod(i),
                                                Costo_Unitario,
                                                Costo,
                                                dtScarico)
            End If
        Next


        'RAPPORTI CONTABILI
        For i = 0 To listaCodContatto.Count - 1

            Dim Dt_Manodopera As DataTable
            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R


            Dim Str As String
            Str = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 ) "
            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(Piva,
                                                                                     listaCodContatto(i),
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, "", objParametri_Server)




            If Dt_Manodopera.Rows.Count = 0 Then
                Exit For
            End If

            Dim Cod_Rapporto As String = ""
            Dim Centro_Cod, Riga, Elem_Cod As Integer
            Dim Tipo_Centro As String = ""
            Dim Centro As String = ""
            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

            Select Case Cod_Rapporto
                Case -1, -4, -6
                    Centro = "Manodopera"
                    Tipo_Centro = "MD"
                    Centro_Cod = -2
                Case -5
                    Centro_Cod = -3
                    Centro = "C/Terzisti"
                    Tipo_Centro = "CT"
                Case -12
                    Centro_Cod = -4
                    Centro = "Tecnico Responsabile"
                    Tipo_Centro = "TR"
            End Select

            'If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
            '    Centro_Cod = -3
            '    Centro = "C/Terzisti"
            '    Tipo_Centro = "CT"
            'End If

            'If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
            '    Centro = "Manodopera"
            '    Tipo_Centro = "MD"
            '    Centro_Cod = -2
            'End If

            Dim Categoria_Des, Risorsa_Des As String

            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
            Elem_Cod = 0

            Dim Mat_Cod, Pro_Cod As Integer
            Pro_Cod = 0
            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")

            'If IsDBNull(Dt_Manodopera.Rows(0).Item("Mezzo")) Then
            '    Udm_Des = "Ha"
            '    Udm_Cod = "1"
            'Else
            '    If Dt_Manodopera.Rows(0).Item("Mezzo") = TipiEnumerativi.enum_TipoMezzo.Ettaro Then
            '        Udm_Des = "Ha"
            '        Udm_Cod = "1"
            '    Else
            '        Udm_Des = "Ora"
            '        Udm_Cod = "2"
            '    End If
            'End If


            'If IsDBNull(Dt_Manodopera.Rows(0).Item("Mezzo")) Then
            '    Udm_Des = "Indefinito"
            '    Udm_Cod = -1
            'Else
            '    Select Case Dt_Manodopera.Rows(0).Item("Mezzo")
            '        Case TipiEnumerativi.enum_TipoMezzo.Ettaro
            '            Udm_Des = "Ha"
            '            Udm_Cod = "1"
            '        Case TipiEnumerativi.enum_TipoMezzo.Ora
            '            Udm_Des = "Ora"
            '            Udm_Cod = "2"
            '        Case TipiEnumerativi.enum_TipoMezzo.Indefinito
            '            Udm_Des = "Indefinito"
            '            Udm_Cod = "-1"
            '    End Select
            'End If

            'Qta_Ril = Dt_Manodopera.Rows(0).Item("Col_4")


            'If IsDBNull(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario")) Then
            '    Costo_Unitario = "0.00"
            '    Costo = "0.00"
            'Else
            '    Costo_Unitario = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 1, "0.00")
            '    Costo = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 0, "0.00")
            'End If


            Dim Ditta_Cod As String
            Ditta_Cod = 0


            ''    Case -3 'TERZISTI

            ''Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
            ''Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

            ''Elem_Cod = 0
            ''Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
            ''Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

            ''Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
            ''Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

            ''Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
            ''Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
            ''Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

            ''Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")

            InserisciRiga_dtScarico(Centro,
                                            Centro_Cod,
                                            Categoria_Des,
                                            Risorsa_Des,
                                            Udm_Des,
                                            0,
                                            Udm_Cod,
                                            Elem_Cod,
                                            Riga,
                                            Tipo_Centro,
                                            Pro_Cod,
                                            Ditta_Cod,
                                            Mat_Cod,
                                            Costo_Unitario,
                                            Costo,
                                            dtScarico)
        Next


        'aggiorno i costi
        Session("dtScarico") = dtScarico

        AggiornaGridViewCostiAccessoriVisibili()

        'SalvaCostiAccessori_Default()


    End Sub










#End Region


    Private Sub ImpostaCmbFormulatoClassificazioni_By_LavCod()

        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, Bagnanti, Antischiuma
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   ecc...

        Cmb_FormulatoClassificazioni.Items.Clear()

        Select Case ComboOperazione.Valore_Combo

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Formulati, enum_TipoFormulato.Antiparassitari))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Coadiuvanti, enum_TipoFormulato.Coadiuvanti))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.CorroborantiFisiofarmaci, enum_TipoFormulato.Corroboranti_Fisiofarmaci))


            Case LAVCOD_CONCIA_SEME
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Concianti, enum_TipoFormulato.Concianti))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Coadiuvanti, enum_TipoFormulato.Coadiuvanti))

            Case LAVCOD_DISSECCAMENTO
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Disseccanti, enum_TipoFormulato.Disseccanti))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Coadiuvanti, enum_TipoFormulato.Coadiuvanti))


            Case LAVCOD_GEODISINFESTAZIONE
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Geodisinfestanti, enum_TipoFormulato.Geodisinfestanti))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Coadiuvanti, enum_TipoFormulato.Coadiuvanti))

            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Fitoregolatori, enum_TipoFormulato.Fitoregolatori))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Coadiuvanti, enum_TipoFormulato.Coadiuvanti))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.CorroborantiFisiofarmaci, enum_TipoFormulato.Corroboranti_Fisiofarmaci))

            Case LAVCOD_DISERBO
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Diserbante, enum_TipoFormulato.Diserbanti))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Coadiuvanti, enum_TipoFormulato.Coadiuvanti))

            Case LAVCOD_DISTRIBUZIONE_INSETTI
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Formulati, enum_TipoFormulato.Antiparassitari))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Coadiuvanti, enum_TipoFormulato.Coadiuvanti))
                Cmb_FormulatoClassificazioni.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.CorroborantiFisiofarmaci, enum_TipoFormulato.Corroboranti_Fisiofarmaci))


        End Select
    End Sub



    Public Sub SelezionaTab(ByVal upPanel As UpdatePanel, ByVal progressivoABaseZero As Integer)

        Dim stb As New StringBuilder

        stb.AppendLine("$(function() { ")
        'stb.AppendLine("    $('#tabs').tabs({ ")
        'stb.AppendLine("        active: " & progressivoABaseZero)
        'stb.AppendLine("    }); ")
        stb.AppendLine("    $(document).scrollTop( $('#divDettagli').offset().top );")
        stb.AppendLine("}); ")



        ScriptManager.RegisterClientScriptBlock(upPanel, upPanel.GetType(),
                                         String.Format("jQuery_{0}", "spostati"), stb.ToString, True)


    End Sub

    Protected Sub btn_Disciplinari_Click(sender As Object, e As EventArgs) Handles btn_Disciplinari.Click

        Dim pulisci As Boolean = True
        If GridView_Dosi_Difesa.Rows.Count > 0 Then

            'controllo se ho acconsentito precedenrtemente al salvataggio senza giacenze


            If Disciplinari_si_no.Value = "0" Then
                'Se  Disciplinari_si_no.Value = "0" allora non ho ancora risposto
                'quindi esco dal metodo, lancio si_no che rispinge il pulsante

                'se non ho acconsentito genere agrosino che mi rilancera il salvataggio via jscript
                Dim Messaggio As String = "Attenzione, è già stato inserito un prodotto, se si varia il disciplinare devi riverificare tutti i dettagli perché i prodotti inseriti in precedenza potrebbero non essere più validi per il nuovo disciplinare selezionato. AProseguire mantenendo i prodotti inseriti precedentemente?"
                Messaggi.AgroSiNo_Bis(Messaggio, "Disciplinari", Page, , )

                Exit Sub

            ElseIf Disciplinari_si_no.Value = "OK" Then
                'se Disciplinari_si_no.Value = "OK" allora ho già risposto SI, quindi 
                'proseguo senza cancellare la tabella
                pulisci = False
            ElseIf Disciplinari_si_no.Value = "NO" Then
                'se Disciplinari_si_no.Value = "NO" allora ho già risposto NO, quindi 
                'proseguo cancellando la tabella
                pulisci = True
            End If


        End If

        Dim TipoTestata As Integer

        Dim ArrayPannelli(0) As enum_TipoPannello

        If pulisci Then
            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
            Imposta_Pannelli(ArrayPannelli)
        End If


        Me.Cmb_Epoca.Items.Clear()
        cella_epoca.Visible = False
        ComboEpoche.Visible = False


        Select Case CInt(ComboOperazione.Valore_Combo)
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                TipoTestata = 0
            Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                TipoTestata = 1
        End Select

        Select Case Cmb_Disciplinare.SelectedItem.Value

            Case "0"

            Case Else

                Dim Array() As String
                Dim Dpi_Cod As Integer = 0
                Dim IdRcdpi As Integer = 0
                Dim Grfi_Cod As Integer = 0
                Dim Flag_Protetto As Integer = 0
                Dim Flag_PubblicoPrivato As Integer = 0

                Array = Split(Cmb_Disciplinare.SelectedValue, "/")
                Dpi_Cod = Array(0)
                IdRcdpi = Array(1)
                Grfi_Cod = Array(2)
                Flag_Protetto = Array(3)
                Flag_PubblicoPrivato = Array(4)

                If Flag_PubblicoPrivato = 2 Then
                    Dpi_Cod = -Dpi_Cod
                End If

                Dim objCaricaCombo As New AgronicaCoreDpiBIZ.CaricaListControl
                objCaricaCombo.Epoche_DPI(
                                Me.Cmb_Epoca,
                                False, "", "", Session,
                                objParametri_Server,
                                objParametri_Utenti,
                                TipoTestata,
                                CInt(IdRcdpi),
                                Dpi_Cod,
                                0,
                                False)

                If Me.Cmb_Epoca.Items.Count > 0 Then
                    cella_epoca.Visible = True
                Else
                    cella_epoca.Visible = False
                End If

        End Select


        CaricaGriglia_Avversita()



        If Session("dtScarico") IsNot Nothing AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
            ReDim ArrayPannelli(2)
            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
            ArrayPannelli(1) = enum_TipoPannello.PannelloDifesa
            ArrayPannelli(2) = enum_TipoPannello.Pannello_Costi
        Else
            ReDim ArrayPannelli(1)
            ArrayPannelli(0) = enum_TipoPannello.Pannello_Testata
            ArrayPannelli(1) = enum_TipoPannello.PannelloDifesa
        End If
        If GridView_Impianti.Rows.Count > 0 Then
            ReDim Preserve ArrayPannelli(ArrayPannelli.Length)
            ArrayPannelli(ArrayPannelli.Length - 1) = enum_TipoPannello.Pannello_Impianti
        End If

        Imposta_Pannelli(ArrayPannelli)


    End Sub

    Protected Sub Btn_Salva_Stampa_Click(sender As Object, e As EventArgs) Handles Btn_Salva_Stampa_RicAz.Click

        Dim Ricetta_Cod As Integer = 0
        Ricetta_Cod = Salva_Ricetta()

        If Ricetta_Cod <> 0 Then

            Dim strOpen As String
            Dim QueryStringParametri As String

            QueryStringParametri = "?ricetta_cod=" & Stringa_Codifica(CStr(Ricetta_Cod), AgroKey_EncoderDecoder, Server) & "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(2), AgroKey_EncoderDecoder, Server)

            strOpen = "<script language='javascript'>" & vbNewLine &
                    "window.open('Stampa/Ricetta_Stampa.aspx" & QueryStringParametri &
                    "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes')" &
                    "</script>"

            'apro la finestra...
            Page.Master.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))

        End If


    End Sub

    Protected Sub Btn_Stampa_RicAz_Click(sender As Object, e As EventArgs) Handles Btn_Stampa_RicAz.Click

        If Qs_Ricetta_Cod <> 0 Then

            Dim strOpen As String
            Dim QueryStringParametri As String

            QueryStringParametri = "?ricetta_cod=" & Stringa_Codifica(CStr(Qs_Ricetta_Cod), AgroKey_EncoderDecoder, Server) & "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(2), AgroKey_EncoderDecoder, Server)

            strOpen = "<script language='javascript'>" & vbNewLine &
                    "window.open('Stampa/Ricetta_Stampa.aspx" & QueryStringParametri &
                    "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes')" &
                    "</script>"

            'apro la finestra...
            Page.Master.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))

        End If

    End Sub

    Protected Sub Btn_Stampa_Cert_Click(sender As Object, e As EventArgs) Handles Btn_Stampa_Cert.Click

        If Qs_Ricetta_Cod <> 0 Then

            Dim strOpen As String
            Dim QueryStringParametri As String

            QueryStringParametri = "?ricetta_cod=" & Stringa_Codifica(CStr(Qs_Ricetta_Cod), AgroKey_EncoderDecoder, Server) & "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server)

            strOpen = "<script language='javascript'>" & vbNewLine &
                    "window.open('Stampa/Ricetta_Stampa.aspx" & QueryStringParametri &
                    "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes')" &
                    "</script>"

            'apro la finestra...
            Page.Master.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))

        End If

    End Sub


#Region "RilievoAvversitàAusiliari"

    'Per la generazione della tabella con impianti e trappole
    Private Function GeneraTabellaHtmlTrappole(ByVal genera_DaMaster As Boolean) As Boolean
        InizializzaTabellaTrappole()

        '-------------------------Genero le tabelle avversità per la lavorazione-------------------------------------
        Dim DT_Infestanti As DataTable = Nothing
        Dim DT_Fasi As DataTable = Nothing
        Dim DT_Avversita As DataTable = Nothing
        Dim DT_Indici As DataTable = Nothing
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)
                DT_Infestanti = New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R().Leggi(0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                'Session("DT_Infestanti") = DT_Infestanti
            Case LAVCOD_FASI_FENOLOGICHE
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)
                DT_Fasi = New AgronicaCoreMetaSchemaDAL.FasiFenologichexSpecie_R().Leggi(0, objParametriAgenda.Veg_Cod.Split("/")(0), enumSelezioneVariabile.Selezione_JoinCompleta, "", "Progressivo ASC", objParametri_Server)
                'Session("DT_Fasi") = DT_Fasi
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)
                'nel caso si voglia usare la combo avversità (nascosta) per scremarle mettere av_cod invece di 0
                DT_Avversita = New AgronicaCoreMetaSchemaDAL.Avversita_R().Leggi_Con_Misura(objParametriAgenda.Veg_Cod.Split("/")(0), 0, "", "", "", objParametri_Server)
                'Session("DT_Avversita") = DT_Avversita
            Case LAVCOD_RILIEVO_INDICI_MATURITA
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_RACCOLTA)
                DT_Indici = New AgronicaCoreMetaSchemaDAL.IndiciMaturitaxSpecie_R().Leggi_X_UDM(0,
                                 objParametriAgenda.Veg_Cod.Split("/")(0),
                                "", "IND_MAT_DES ASC", objParametri_Server)
                'Session("DT_Indici") = DT_Indici
        End Select
        '-----------------------------------------------------------------------------------------



        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        If genera_DaMaster Then
            'genero tabella da impianti master
            ListaImpianti = CType(Master, Operazione).GetImpianti()
        Else
            'genero tabella da oggettto parametri agenda, quando sono in modifica e devo generare
            'la tabella prima che venga chiamata la load della master
            ListaImpianti = objParametriAgenda.Impianti
        End If


        Dim N_Colonne As Integer = ListaImpianti.Count
        If N_Colonne = 0 Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale, Page, , UpdatePanelOperazione)
            Return False
        End If


        '___________________________________________________________
        '                    FIORITURA

        'ATTENZIONE!
        'Quando si assegna la fioritura ad una specie vegetale, 
        'si seleziona l'inizio fase e la fine fase.
        'La Fioritura che deve essere visualizzata in rosso (nel GiasOnline e 
        'nell' AgronicaManutenzione) è quella di inizio fase, 
        'quindi quella che nella tabella FasiFenologichexFioriture 
        'viene salvata nel campo DA_FF_COD.
        'Non si sa per quale motivo, ma la funzione Fioritura_from_VegCod restituisce 
        'il campo FF_COD (che è la fine) e non il campo DA_FF_COD (che è l'inizio).
        'Morale della favola: in banca dati bisogna inserire fase fine = fase inizio 

        'Evidenzio la fase fenologica associata alla fioritura
        Dim objFioritura As New AgronicaCoreMetaSchemaDAL.FasiFenologichexFioriture_R

        Dim FF_Cod_fioritura As Integer = 0
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

            Case LAVCOD_FASI_FENOLOGICHE
                FF_Cod_fioritura = objFioritura.Fioritura_from_VegCod(CInt(objParametriAgenda.Veg_Cod),
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                   "", "", objParametri_Server)
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
            Case LAVCOD_RILIEVO_INDICI_MATURITA
            Case Else
                Throw New NotImplementedException
        End Select
        '___________________________________________________________


        '-------------------------RIGA INTESTAZIONE TABELLA-------------------------------------

        Dim trow As New TableRow

        'intestazione'colonne
        trow.CssClass = "ui-widget-header"

        '---------------CELLA-----------------------------Testata Colonna 1 (Avvesita)--------------------------
        Dim tcell As New TableCell
        'impostazioni in base alla lavorazione   
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                tcell.Controls.Add(New LiteralControl("Infestante"))
            Case LAVCOD_FASI_FENOLOGICHE
                tcell.Controls.Add(New LiteralControl("Fase Fenologica"))
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                tcell.Controls.Add(New LiteralControl("Avversità"))
            Case LAVCOD_RILIEVO_INDICI_MATURITA
                tcell.Controls.Add(New LiteralControl("Indice di Maturità"))
            Case Else
                Throw New NotImplementedException
        End Select
        'tcell.CssClass = "ui-widget-header"
        tcell.Attributes.Add("TipoGruppo", "TestataColonna")
        tcell.Attributes.Add("TipoCella", "CodiceRilievo")
        trow.Cells.Add(tcell)

        '---------------CELLA----------------------------Testata Colonna 2 (UDM)---------------------------
        tcell = New TableCell
        tcell.Controls.Add(New LiteralControl("Unità di Misura"))
        ' tcell.CssClass = "ui-widget-header"
        tcell.Attributes.Add("TipoGruppo", "TestataColonna")
        tcell.Attributes.Add("TipoCella", "Misura")
        trow.Cells.Add(tcell)

        '---------------CELLA----------------------------Testata Colonna 3 (valore rilevato per tutti)---------------------------
        tcell = New TableCell
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                tcell.Controls.Add(New LiteralControl("Piante per metro quadro"))
            Case LAVCOD_FASI_FENOLOGICHE
                tcell.Controls.Add(New LiteralControl("Data Rilievi Default"))
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                tcell.Controls.Add(New LiteralControl("Quantità Default"))
            Case LAVCOD_RILIEVO_INDICI_MATURITA
                tcell.Controls.Add(New LiteralControl("Valore Default"))
            Case Else
                Throw New NotImplementedException
        End Select
        'tcell.CssClass = "ui-widget-header"
        tcell.Attributes.Add("TipoGruppo", "TestataColonna")
        tcell.Attributes.Add("TipoCella", "ValoreRilevato")
        trow.Cells.Add(tcell)

        Dim i As Integer
        For i = 0 To N_Colonne - 1

            '---------------CELLA----------------------Testata Colonne Impianti 4-> (Valore rilevato per impianto)---------------------------------
            tcell = New TableCell
            Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim nomeApp As String = objAnagrafe.AppezzamentoNome_from_Appezza(ListaImpianti(i).Piva,
                                                                             ListaImpianti(i).Sa_Cod,
                                                                             ListaImpianti(i).Appezza,
                                                                             objParametri_Server)
            Dim objAnagrafe2 As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim nomeCentro As String = objAnagrafe2.SaNome_from_SaCod(ListaImpianti(i).Piva, ListaImpianti(i).Sa_Cod, objParametri_Server)

            nomeApp = ""
            nomeCentro = ""

            tcell.Controls.Add(New LiteralControl("" & nomeCentro & " </br> " & nomeApp))
            'tcell.CssClass = "ui-widget-header"
            tcell.Attributes.Add("TipoGruppo", "TestataColonna")
            tcell.Attributes.Add("TipoCella", "TestataAppezzamento")

            tcell.Attributes.Add("IdAzienda", CStr(ListaImpianti(i).Piva))
            tcell.Attributes.Add("IdCentro", CStr(ListaImpianti(i).Sa_Cod))
            tcell.Attributes.Add("IdAppezzamento", CStr(ListaImpianti(i).Appezza))
            tcell.Attributes.Add("IdReg", CStr(ListaImpianti(i).ID_Reg))
            trow.Cells.Add(tcell)
        Next
        TabellaTrappole.Rows.Add(trow)


        'ricavo il numero di righe dalle tabelle corrispondenti all'operazione
        Dim N_Righe As Integer = 0
        'impostazioni in base alla lavorazione
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                N_Righe = DT_Infestanti.Rows.Count
            Case LAVCOD_FASI_FENOLOGICHE
                N_Righe = DT_Fasi.Rows.Count
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                N_Righe = DT_Avversita.Rows.Count
            Case LAVCOD_RILIEVO_INDICI_MATURITA
                N_Righe = DT_Indici.Rows.Count
            Case Else
                Throw New NotImplementedException
        End Select

        'variabile per cambio colore riga
        Dim CambioColoreRiga As Boolean = False

        '-------------------------RIGHE SUCCESSIVE TABELLA-------------------------------------
        For j = 0 To N_Righe - 1
            'creo la riga
            Dim trow2 As New TableRow

            'inizializzo i valori
            Dim Av_Cod As Integer = 0
            Dim Av_Des_Vol As String = ""
            Dim av_des_label As String = ""

            '---------------CELLA--------------------------Dato Righe - Colonna 1 (Avvesita)-----------------------------
            tcell = New TableCell
            'impostazioni in base alla lavorazione
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    Av_Cod = DT_Infestanti.Rows(j).Item("Av_Gru")
                    Av_Des_Vol = DT_Infestanti.Rows(j).Item("Av_Gru_Des")
                    av_des_label = Av_Des_Vol
                    CambioColoreRiga = Not CambioColoreRiga
                Case LAVCOD_FASI_FENOLOGICHE
                    Av_Cod = DT_Fasi.Rows(j).Item("FF_Cod")
                    Av_Des_Vol = DT_Fasi.Rows(j).Item("FF_Des")
                    av_des_label = Av_Des_Vol
                    CambioColoreRiga = Not CambioColoreRiga
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    Av_Cod = DT_Avversita.Rows(j).Item("Av_Cod")
                    Av_Des_Vol = DT_Avversita.Rows(j).Item("Av_Des_Vol")
                    av_des_label = Av_Des_Vol
                    If j > 0 AndAlso Av_Cod = DT_Avversita.Rows(j - 1).Item("Av_Cod") Then
                        'se si ripete lo stesso codice av non ripeto per ciascuna riga l'avversità
                        av_des_label = ""
                    Else
                        CambioColoreRiga = Not CambioColoreRiga
                    End If
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    Av_Cod = DT_Indici.Rows(j).Item("IND_MAT_COD")
                    Av_Des_Vol = DT_Indici.Rows(j).Item("IND_MAT_DES")
                    av_des_label = Av_Des_Vol
                    If j > 0 AndAlso Av_Cod = DT_Indici.Rows(j - 1).Item("IND_MAT_COD") Then
                        'se si ripete lo stesso codice av non ripeto per ciascuna riga l'avversità
                        av_des_label = ""
                    Else
                        CambioColoreRiga = Not CambioColoreRiga
                    End If
                Case Else
                    Throw New NotImplementedException
            End Select

            'cambio il colore della riga (per avversita raggruppa per av_cod)
            If CambioColoreRiga Then
                trow2.BackColor = Drawing.Color.Beige
            End If

            'per fasi fenologiche evidenzio fioritura qdc
            If CInt(objParametriAgenda.Lav_Cod) = LAVCOD_FASI_FENOLOGICHE AndAlso Av_Cod = FF_Cod_fioritura Then
                trow2.BackColor = System.Drawing.Color.Pink
            End If

            tcell.Controls.Add(New LiteralControl(av_des_label))
            tcell.Attributes.Add("TipoGruppo", "Dato")
            tcell.Attributes.Add("TipoCella", "CodiceRilievo")
            tcell.Attributes.Add("Av_Cod", Av_Cod)
            tcell.Attributes.Add("Av_Des", Av_Des_Vol)
            trow2.Cells.Add(tcell)

            Dim Udm_Cod As Integer = 0
            Dim Udm_Des As String = ""

            '---------------CELLA--------------------------Dato Righe - Colonna 2 (UDM)-----------------------------
            tcell = New TableCell
            'impostazioni in base alla lavorazione
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    tcell.Controls.Add(New LiteralControl(Udm_Des))
                Case LAVCOD_FASI_FENOLOGICHE
                    tcell.Controls.Add(New LiteralControl(Udm_Des))
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    Udm_Cod = DT_Avversita.Rows(j).Item("Udm_Cod")
                    Udm_Des = DT_Avversita.Rows(j).Item("Udm_Des")
                    tcell.Controls.Add(New LiteralControl(Udm_Des))
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    Udm_Cod = DT_Indici.Rows(j).Item("Udm_Cod")
                    Udm_Des = DT_Indici.Rows(j).Item("Udm_Des")
                    tcell.Controls.Add(New LiteralControl(Udm_Des))
                Case Else
                    Throw New NotImplementedException
            End Select

            tcell.Attributes.Add("TipoGruppo", "Dato")
            tcell.Attributes.Add("TipoCella", "Misura")

            tcell.Attributes.Add("Udm_Cod", Udm_Cod)
            tcell.Attributes.Add("Udm_Des", Udm_Des)

            trow2.Cells.Add(tcell)

            '---------------CELLA-------------------------Dato Righe - Colonna 3 (valore rilevato per tutti)------------------------------
            tcell = New TableCell
            Dim txt As New TextBox
            txt.ID = "TXTCodicePersonalizzato" & SeparaID & j + 1 & SeparaID & 0
            txt.CssClass = "txtUI valoreTutti"
            txt.Style.Add("width", "100px")
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    txt.MaxLength = 9
                Case LAVCOD_FASI_FENOLOGICHE
                    txt.MaxLength = 11
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    txt.MaxLength = 9
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    txt.MaxLength = 9
                Case Else
                    Throw New NotImplementedException
            End Select
            tcell.Controls.Add(txt)
            tcell.Attributes.Add("TipoGruppo", "Dato")
            tcell.Attributes.Add("TipoCella", "ValoreRilevato")

            tcell.Attributes.Add("IdAzienda", "")
            tcell.Attributes.Add("IdCentro", "0")
            tcell.Attributes.Add("IdAppezzamento", "0")
            tcell.Attributes.Add("IdReg", "0")
            trow2.Cells.Add(tcell)

            For i = 0 To N_Colonne - 1

                '---------------CELLA---------------------Dato Righe - Colonne impianti (Valore rilevato per impianto)----------------------------------
                tcell = New TableCell
                txt = New TextBox
                txt.ID = "TXTCodicePersonalizzato" & SeparaID & j + 1 & SeparaID & i + 1
                txt.CssClass = "txtUI valoreImpianto"
                txt.Style.Add("width", "100px")
                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                        txt.MaxLength = 9
                    Case LAVCOD_FASI_FENOLOGICHE
                        txt.MaxLength = 11

                    Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                        txt.MaxLength = 9
                        '  Vanni, 29/05/2014 15:19:31: per aprire la dialog..
                        ImpostaEventoFocusTextBox(Av_Cod, Udm_Cod, txt)

                    Case LAVCOD_RILIEVO_INDICI_MATURITA
                        txt.MaxLength = 9
                    Case Else
                        Throw New NotImplementedException
                End Select



                If genera_DaMaster Then

                Else

                    '-------------------------------------------------------------------
                    '-------------------------INSERIMENTO DATI--------------------------
                    '-------------quando tabella generata dai parametri agenda----------
                    '-------------------------------------------------------------------
                    For Each riliev As rilievoAvv In objParametriAgenda.Rilievi
                        If ListaImpianti(i).Piva = riliev.Impianto.Piva AndAlso
                            ListaImpianti(i).Sa_Cod = riliev.Impianto.Sa_Cod AndAlso
                            ListaImpianti(i).Appezza = riliev.Impianto.Appezza AndAlso
                            ListaImpianti(i).ID_Reg = riliev.Impianto.ID_Reg AndAlso
                            Av_Cod = riliev.Av_cod Then



                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                                    txt.Text = riliev.Valore

                                Case LAVCOD_FASI_FENOLOGICHE

                                    txt.Text = riliev.Valore

                                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                    If Udm_Cod = riliev.Udm_cod Then
                                        txt.Text = riliev.Valore

                                    End If
                                Case LAVCOD_RILIEVO_INDICI_MATURITA
                                    If Udm_Cod = riliev.Udm_cod Then
                                        txt.Text = riliev.Valore
                                    End If
                                Case Else
                                    Throw New NotImplementedException
                            End Select
                        End If
                    Next
                    '-------------------------------------------------------------------
                    '-------------------------------------------------------------------
                    '-------------------------------------------------------------------

                End If

                tcell.Controls.Add(txt)
                tcell.Attributes.Add("TipoGruppo", "Dato")
                tcell.Attributes.Add("TipoCella", "ValoreRilevato")

                tcell.Attributes.Add("IdAzienda", CStr(ListaImpianti(i).Piva))
                tcell.Attributes.Add("IdCentro", CStr(ListaImpianti(i).Sa_Cod))
                tcell.Attributes.Add("IdAppezzamento", CStr(ListaImpianti(i).Appezza))
                tcell.Attributes.Add("IdReg", CStr(ListaImpianti(i).ID_Reg))

                '  Vanni, 27/05/2014 18:00:11: al momento memorizziamo un punto per ogni operazione, che è uguale per tutti.
                If objParametriAgenda.Rilievi.Count > 0 Then
                    tcell.Attributes.Add("mov_destinazioni_graphickey", objParametriAgenda.Rilievi.FirstOrDefault.mov_destinazioni_graphickey)
                End If


                trow2.Cells.Add(tcell)
            Next

            TabellaTrappole.Rows.Add(trow2)
        Next


        'modifica righe finale
        For ir = 0 To TabellaTrappole.Rows.Count - 1

            'impostazioni per tipo operazione
            Select Case CInt(objParametriAgenda.Tipo_Operazione)
                Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                    'nascondo cella valore default in lettura
                    TabellaTrappole.Rows(ir).Cells(2).Visible = False
                Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                Case Else
                    Throw New NotImplementedException
            End Select

            'impostazioni in base alla lavorazione
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    TabellaTrappole.Rows(ir).Cells(1).Visible = False

                Case LAVCOD_FASI_FENOLOGICHE
                    TabellaTrappole.Rows(ir).Cells(1).Visible = False

                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                Case LAVCOD_RILIEVO_INDICI_MATURITA

                Case Else
                    Throw New NotImplementedException
            End Select

        Next



        Session("Tabella") = TabellaTrappole
        PlaceTabella.Controls.Add(TabellaTrappole)
        Return True

    End Function


    Private Sub ImpostaEventoFocusTextBox(ByVal Av_cod As Integer, ByVal Udm_cod As Integer, ByRef txt As TextBox)
        txt.Attributes.Add("idSpecie", Cmb_Specie.ClientID)
        txt.Attributes.Add("onfocus", "CaricaValoriComboMisuraXAvversitaInputData($(this)," & Av_cod & "," & Udm_cod & ")")
    End Sub

#End Region


    Private Function getDtImpiantiFromPianoConimazione(PC_Testata_Cod) As DataTable

        'Leggo la testata del piano di concimazione
        'Dim objPC_TestataR As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R
        'Dim Dt_Testata As DataTable = objPC_TestataR.Leggi(CodPianoConc, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        'Leggo gli impianti collegati del piano di concimazione
        Dim objPC_EntitaxTestata_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
        Dim Dt_EntitaxTestata As DataTable = objPC_EntitaxTestata_R.Leggi(PC_Testata_Cod, 0, "", 0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim leggiDati As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ragsocFrompiva As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim saNomeFromPivaSaCod As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dim dtImp As DataTable = GeneraStrutturaDTImpianti()

        'Popolo la lista degli impianti collegati con tanto di NPK
        For Each drElem As DataRow In Dt_EntitaxTestata.Rows

            Dim piva = drElem.Item("Piva")
            Dim Sa_Cod = drElem.Item("Sa_Cod")
            Dim Appezza = drElem.Item("Appezza")
            Dim Id_reg = drElem.Item("Id_Imp")

            Dim dtLeggiDescrizioni As DataTable =
                leggiDati.Leggi_DescrizioniImpianti2(
                piva, Sa_Cod, Appezza, Id_reg,
                AGRODATAINIZIO, AGRODATAFINE,
                "", "", objParametri_Server
            )

            Dim dtLeggiDati As DataTable =
                leggiDati.Leggi(
                piva, Sa_Cod, Appezza, Id_reg,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "", "", objParametri_Server
            )

            Dim dr As DataRow = dtImp.NewRow()

            dr.Item("piva") = piva
            dr.Item("sa_cod") = Sa_Cod
            dr.Item("campo_cod") = drElem.Item("Campo_Cod")
            dr.Item("appezza") = Appezza
            dr.Item("id_reg") = Id_reg
            dr.Item("veg_cod") = leggiDati.VegCod_from_PivaSaCodAppezzaIdimp(piva, Sa_Cod, Appezza, Id_reg, "", "", objParametri_Server)
            'dr.Item("cul_cod") = objImpianto.Cul_Cod
            dr.Item("grfi_cod") = dtLeggiDati.Rows(0)("grfi_cod")
            dr.Item("regolamento") = dtLeggiDati.Rows(0)("regolamento")
            dr.Item("finanziamento") = dtLeggiDati.Rows(0)("finanziamento")
            'dr.Item("rag_soc") = ragsocFrompiva.RagSoc_from_Piva(piva, objParametri_Server)
            dr.Item("rag_soc") = dtLeggiDescrizioni.Rows(0)("rag_soc")
            'dr.Item("sa_nome") = saNomeFromPivaSaCod.SaNome_from_SaCod(piva, Sa_Cod, objParametri_Server)
            dr.Item("sa_nome") = dtLeggiDescrizioni.Rows(0)("sa_nome")
            dr.Item("app_nome") = dtLeggiDescrizioni.Rows(0)("app_nome")
            'dr.Item("lotto") = objImpianto.Progetto_Nome
            dr.Item("descrizione") = dtLeggiDescrizioni.Rows(0)("Campo_des") & " - " & dtLeggiDescrizioni.Rows(0)("App_nome")
            dr.Item("sup_imp") = dtLeggiDescrizioni.Rows(0)("sup_imp")
            'dr.Item("validita_inizio") = AGRODATAINIZIO
            'dr.Item("validita_fine") = AGRODATAFINE
            dr.Item("validita_inizio") = dtLeggiDati.Rows(0)("validita_inizio")
            dr.Item("validita_fine") = dtLeggiDati.Rows(0)("validita_fine")

            'Aggiungo i valori massimi di NPK per lo specifico impianto
            dr.Item("N_Max") = drElem.Item("QtaMaxN")
            dr.Item("P_Max") = drElem.Item("QtaMaxP2O5")
            dr.Item("K_Max") = drElem.Item("QtaMaxK2O")

            dtImp.Rows.Add(dr)
        Next

        Return dtImp


    End Function


    Private Sub calcolaNPK(dtOperazioni As DataTable, ByRef N_Distribuito_Ha As Decimal, ByRef P_Distribuito_Ha As Decimal, ByRef K_Distribuito_Ha As Decimal)

        Dim XmlDoc As New XmlDocument()

        'azzero i contatori
        N_Distribuito_Ha = 0
        P_Distribuito_Ha = 0
        K_Distribuito_Ha = 0

        'Per ogni operazione
        For Each op As DataRow In dtOperazioni.Rows
            'estraggo e carico la stringa XML
            Dim Xml_OperazioneStr As String = op.Item("Xml_Operazione")
            XmlDoc.LoadXml(Xml_OperazioneStr)

            'estraggo l'elenco dei prodotti in miscela
            Dim XML_Dettagli As XmlNodeList = XmlDoc.SelectNodes("Ricetta_Operazione/DatiRicetta_Dettagli/Ricetta_Dettaglio")

            For Each det As XmlElement In XML_Dettagli
                Dim cau_mov As String = det.GetAttribute("cau_mov")
                Dim elem_cod As String = det.GetAttribute("elem_cod")
                Dim qta As Integer = CInt(det.GetAttribute("qta")) 'numero di dosi

                'Verifico che si tratti di un prodotto e non di un costo accessorio
                If cau_mov = TipiEnumerativi.enum_Agenda_Causali.LAVORAZIONE AndAlso elem_cod = CostantiPersonalizzate.FERTILIZZANTI Then

                    Dim detTec2 As XmlElement = det.SelectSingleNode("Ricetta_Dettaglio_Tecnico_2")
                    Dim efficienza As Decimal = CDec(detTec2.GetAttribute("efficienza"))

                    If efficienza < 1 Then
                        N_Distribuito_Ha += (qta * CDec(detTec2.GetAttribute("n")) / 100 * efficienza)
                    Else
                        N_Distribuito_Ha += (qta * CDec(detTec2.GetAttribute("n")) / 100)
                    End If

                    P_Distribuito_Ha += (qta * CDec(detTec2.GetAttribute("p")) / 100)
                    K_Distribuito_Ha += (qta * CDec(detTec2.GetAttribute("k")) / 100)

                End If
            Next

        Next

    End Sub

    Private Sub Cmb_Specie_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles Cmb_Specie.SelectedIndexChanged
        GestisciComboPianificazioni(True)
    End Sub
End Class
