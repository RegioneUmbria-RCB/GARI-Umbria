Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class Risorse_Manager
    Inherits System.Web.UI.Page

    Public Master_Operazione As Agenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Private Sub Menu_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Master_Operazione = CType(Page.Master, Agenda)
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
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
       
        'Titolo
        CType(Master.FindControl("Lbl_Titolo"), Label).Text = "Gestione Magazzini"

        InizializzaVarie()

        If Not IsPostBack Then

            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================

            Dim UtenteAbilitato_Lettura As Boolean = False

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                        Session("ASG_Utente_Username"), _
                                        Session("ASG_IdServizio"), _
                                        enum_Security_Attivita.Gest_Prodotti, _
                                        enum_Security_Operazione.Lettura, _
                                        Date.Now, _
                                        "", _
                                        objParametri_Utenti)

            ViewState("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

            Dim UtenteAbilitato_Modifica As Boolean = False
            UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Gest_Prodotti, _
                                       enum_Security_Operazione.Modifica, _
                                       Date.Now, _
                                       "", _
                                       objParametri_Utenti)

            ViewState("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica

            If ViewState("UtenteAbilitato_Lettura") = False Then
                Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
            End If

        Else


        End If

    End Sub

    Private Sub InizializzaVarie()


        'script categoria
        Dim StrSelect As New StringBuilder
        StrSelect.AppendLine("$(document).ready(function () { ")
        StrSelect.AppendLine("   $('#" & cmb_Categoria.ClientID & "').combobox();")

        StrSelect.AppendLine("$('#" + txt_DataOperazioneDa.ClientID + "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        'StrSelect.AppendLine("  $('#" & txt_DataOperazioneDa.ClientID & "').change(function () {  $('#" & BTN_ChangeData_Da.ClientID & "').click(); });")

        StrSelect.AppendLine("$('#" + txt_DataOperazioneA.ClientID + "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        'StrSelect.AppendLine("  $('#" & txt_DataOperazioneA.ClientID & "').change(function () {  $('#" & BTN_ChangeData_A.ClientID & "').click(); });")

        StrSelect.AppendLine("});")
        ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType(),
                                     String.Format("jQuery_{0}", cmb_Categoria.ClientID), StrSelect.ToString, True)


    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        'objParametriAgenda.Svuota_DatiOperazione()
        'Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

        'If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline And
        '    paginaOnLineRitorno = enum_PagineGiasOnline.MenuMagazzini Then

        '    Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(True, paginaOnLineRitorno, objParametriAgenda))

        'End If

        'If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And
        '        paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

        '    Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))

        'End If

        'If paginaOnLineRitorno = 0 Then
        '    If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
        '        paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu
        '    Else
        '        paginaOnLineRitorno = enum_PagineAgenda_2010.Menu
        '    End If
        '    Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))
        'End If

        'Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))

    End Sub


End Class