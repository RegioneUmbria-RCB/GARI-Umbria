Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreXML.XML_Stampe
Imports System.Drawing
Imports AgronicaCoreGestioneRichieste
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq



Public Class MenuStampe
    Inherits System.Web.UI.Page


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public Master_Operazione As Agenda
    Dim objParametriAgenda As ParametriAgenda


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
        objParametriAgenda = New ParametriAgenda
        'objParametriAgenda.Leggi()

        'Titolo
        CType(Master.FindControl("Lbl_Titolo"), Label).Text = "Menu Stampe"



        InizializzaVarie()

        Dim ispostbackscript As String = "var isPostBack = false;"

        If Not IsPostBack Then

            DistruggiSessionVecchie()



            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================

            Dim UtenteAbilitato_Lettura As Boolean = False

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente(
                                        Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Gest_Stampe,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)

            ViewState("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

            Dim UtenteAbilitato_Modifica As Boolean = False
            UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                       Session("ASG_IdServizio"),
                                       enum_Security_Attivita.Gest_Stampe,
                                       enum_Security_Operazione.Modifica,
                                       Date.Now,
                                       "",
                                       objParametri_Utenti)

            ViewState("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica


            '##############################################################
            '#####  Inizializzo i controlli  ##############################
            '##############################################################
            ImpostaPermessi()



            CaricaStampe()

            AggiungoIPulsantiAlTabPreferiti()


        Else

            '========================================
            '===== E' avvenuto un Postback ==========
            '========================================
            'If objParametriAgenda.Piva = "" Then
            '    Throw New Exception("objParametriAgenda.Piva = '' ")
            'End If


            ispostbackscript = "var isPostBack = true;"
        End If

        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "IsPostBack", ispostbackscript, True)





    End Sub


    Private Sub CaricaStampe()

        Dim filtroValidita = " Validita_Fine >= " & Agro_SQL_SaveDate(Date.Now) & " "
        Dim filtroStampeNuove = " AND Id_StampeReport NOT IN (74, -74, 75, 76, 81, 77, 78, 79, 80, 171, 127, 130, 131, 132, 133, 134, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244) "

        AgronicaCoreUtility.CaricaListControl.StampeReport(
                                    Me.cmb_Stampe,
                                    True, "", "",
                                    filtroValidita & filtroStampeNuove,
                                     "", objParametri_Server)



        MenuStampe.Items.Clear()


        Dim StampeReport As New AgronicaCoreMetaSchemaDAL.StampeReport

        Dim DT As DataTable
        Dim i As Integer

        ' ^^^ Tolgo le stampe che vengono aperte tramite la pagina Schede OP

        DT = StampeReport.LeggiGruppi(0, filtroValidita, " Id_StampeReportGruppi ", objParametri_Server)


        If DT.Rows.Count > 0 Then

            Dim MenuItem As System.Web.UI.WebControls.MenuItem

            For i = 0 To DT.Rows.Count - 1

                MenuItem = New System.Web.UI.WebControls.MenuItem(DT.Rows(i).Item("Descrizione"),
                                     "G" & DT.Rows(i).Item("Id_StampeReportGruppi"),
                                     DT.Rows(i).Item("Immagine"))

                MenuItem.ToolTip = DT.Rows(i).Item("Descrizione_Lunga")

                Dim DT2 As DataTable
                Dim i2 As Integer

                DT2 = StampeReport.Leggi(0, DT.Rows(i).Item("Id_StampeReportGruppi"), filtroValidita & filtroStampeNuove, "", objParametri_Server)


                If DT2.Rows.Count > 0 Then

                    Dim MenuItem2 As System.Web.UI.WebControls.MenuItem

                    For i2 = 0 To DT2.Rows.Count - 1

                        MenuItem2 = New System.Web.UI.WebControls.MenuItem(DT2.Rows(i2).Item("Descrizione"),
                                              DT2.Rows(i2).Item("Id_StampeReport"))

                        MenuItem2.ToolTip = DT2.Rows(i2).Item("Descrizione_Lunga")

                        MenuItem.ChildItems.Add(MenuItem2)

                    Next

                End If

                MenuStampe.Items.Add(MenuItem)




            Next
        End If

    End Sub

    Private Sub AggiungoIPulsantiAlTabPreferiti()
        'aggiungo i pulsanti al tab preferiti

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable
        DT = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "", "", objParametri_Utenti)
        If DT.Rows.Count > 0 Then
            If Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then
                Dim lavs As String = DT.Rows(0).Item("Impostazione_Valore_1")
                Dim lavstr() As String = lavs.Split("|")
                For i = 0 To lavstr.Count - 1
                    If IsNumeric(lavstr(i)) Then
                        Dim lavcod As Integer = CInt(lavstr(i))
                        Dim lavdes As String = New AgronicaCoreMetaSchemaDAL.StampeReport().LeggiDescrizione(lavcod, "", objParametri_Server)
                        LabelOperazionePref.Visible = True
                        If i = 0 Then
                            BtnOperazionePref1.Visible = True
                            BtnOperazionePref1.CommandName = lavcod
                            BtnOperazionePref1.Text = "" & lavdes
                            ''NuovaOperazioneL1.Visible = True
                            ''NuovaOperazioneL1.Text = "Nuova op. " & lavdes
                        End If
                        If i = 1 Then
                            BtnOperazionePref2.Visible = True
                            BtnOperazionePref2.CommandName = lavcod
                            BtnOperazionePref2.Text = "" & lavdes
                            'NuovaOperazioneL2.Visible = True
                            'NuovaOperazioneL2.Text = "Nuova op. " & lavdes
                        End If
                        If i = 2 Then
                            BtnOperazionePref3.Visible = True
                            BtnOperazionePref3.CommandName = lavcod
                            BtnOperazionePref3.Text = "" & lavdes
                            'NuovaOperazioneL3.Visible = True
                            'NuovaOperazioneL3.Text = "Nuova op. " & lavdes
                        End If
                        If i = 3 Then
                            BtnOperazionePref4.Visible = True
                            BtnOperazionePref4.CommandName = lavcod
                            BtnOperazionePref4.Text = "" & lavdes
                            'NuovaOperazioneL4.Visible = True
                            'NuovaOperazioneL4.Text = "Nuova op. " & lavdes
                        End If
                        If i = 4 Then
                            BtnOperazionePref5.Visible = True
                            BtnOperazionePref5.CommandName = lavcod
                            BtnOperazionePref5.Text = "" & lavdes
                            'BtnOperazionePref5.Visible = True
                            'BtnOperazionePref5.Text = "Nuova op. " & lavdes
                        End If
                        If i = 5 Then
                            BtnOperazionePref6.Visible = True
                            BtnOperazionePref6.CommandName = lavcod
                            BtnOperazionePref6.Text = "" & lavdes
                            'BtnOperazionePref6.Visible = True
                            'BtnOperazionePref6.Text = "Nuova op. " & lavdes
                        End If
                        If i = 6 Then
                            BtnOperazionePref7.Visible = True
                            BtnOperazionePref7.CommandName = lavcod
                            BtnOperazionePref7.Text = "" & lavdes
                            'BtnOperazionePref7.Visible = True
                            'BtnOperazionePref7.Text = "Nuova op. " & lavdes
                        End If
                        If i = 7 Then
                            BtnOperazionePref8.Visible = True
                            BtnOperazionePref8.CommandName = lavcod
                            BtnOperazionePref8.Text = "" & lavdes
                            'BtnOperazionePref8.Visible = True
                            'BtnOperazionePref8.Text = "Nuova op. " & lavdes
                        End If
                        If i = 8 Then
                            BtnOperazionePref9.Visible = True
                            BtnOperazionePref9.CommandName = lavcod
                            BtnOperazionePref9.Text = "" & lavdes
                            'BtnOperazionePref9.Visible = True
                            'BtnOperazionePref9.Text = "Nuova op. " & lavdes
                        End If
                        If i = 9 Then
                            BtnOperazionePref10.Visible = True
                            BtnOperazionePref10.CommandName = lavcod
                            BtnOperazionePref10.Text = "" & lavdes
                            'BtnOperazionePref10.Visible = True
                            'BtnOperazionePref10.Text = "Nuova op. " & lavdes
                        End If

                    End If
                Next
            End If

        End If
    End Sub



    Private Sub InizializzaVarie()

        'script stampe
        Dim StrSelect As New StringBuilder
        StrSelect.AppendLine("$(document).ready(function () { ")
        StrSelect.AppendLine("   $('#" & cmb_Stampe.ClientID & "').combobox();")
        StrSelect.AppendLine("   $('input:submit').button(); ")

        StrSelect.AppendLine("});")
        ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType(),
                                     String.Format("jQuery_{0}", cmb_Stampe.ClientID), StrSelect.ToString, True)

    End Sub


    Private Sub DistruggiSessionVecchie()

        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")

    End Sub


    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        'objParametriAgenda.Svuota_DatiOperazione()

        'Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))

        'Dim Str As New StringBuilder
        'Str.AppendLine("$(document).ready(function () {")
        'Str.AppendLine("    window.close();")
        'Str.AppendLine("    });")

        'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType(),
        '                             String.Format("jQuery_{0}", cmb_Stampe.ClientID), Str.ToString, True)

        Dim s As String = "<script language=javascript>"

        s += "window.opener=top; "
        s += "window.close(); "
        s += "</script>"
        RegisterStartupScript("CloseScript", s)
    End Sub

    Public Sub ImpostaPermessi()
        'Controllo se ha il permesso di lettura
        If ViewState("UtenteAbilitato_Lettura") = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

    End Sub





    Protected Sub BTN_ComboStampe_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboStampe.Click

        If IsNumeric(cmb_Stampe.SelectedItem.Value) Then
            Dim StampeReport As New AgronicaCoreMetaSchemaDAL.StampeReport
            Dim deslunga As String = StampeReport.LeggiDescrizioneLunga(cmb_Stampe.SelectedItem.Value, "", objParametri_Server)

            If deslunga <> "" Then
                LabelStampa.Text = deslunga
            Else
                LabelStampa.Text = cmb_Stampe.SelectedItem.Text
            End If
        End If


    End Sub






    Protected Sub ImageButton_Inserisci_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton_Inserisci.Click
        If Not IsNumeric(cmb_Stampe.SelectedItem.Value) Then
            Exit Sub
        End If

        Dim reportselezionato As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe
        reportselezionato = cmb_Stampe.SelectedItem.Value

        Dim nuovivalori As String = CStr(CInt(reportselezionato))

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable
        DT = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "", "", objParametri_Utenti)
        If DT.Rows.Count > 0 Then
            If Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then
                Dim lavs As String = DT.Rows(0).Item("Impostazione_Valore_1")


                Dim lavstr() As String = lavs.Split("|")
                For i = 0 To lavstr.Count - 1
                    'massimo 8 preferiti
                    If i = 9 Then
                        Exit For
                    End If
                    If IsNumeric(lavstr(i)) And CInt(lavstr(i)) <> CInt(reportselezionato) Then
                        'salto se c'è il repoort selezionato, è in testa
                        nuovivalori &= "|" & CInt(lavstr(i))
                    End If
                Next
            End If

        End If

        Dim Utenti_Impostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Utenti_Impostazioni_W.Cancella(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, "", objParametri_Utenti)
        Utenti_Impostazioni_W.Scrivi(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, nuovivalori, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        AggiungoIPulsantiAlTabPreferiti()
    End Sub




    Protected Sub MenuAgenda_MenuItemClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles MenuStampe.MenuItemClick
        If Not IsNumeric(MenuStampe.SelectedValue) Then
            Exit Sub
        End If

        Dim reportselezionato As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe
        reportselezionato = MenuStampe.SelectedValue
        gestisciStampa(reportselezionato)

    End Sub

    Protected Sub ImgBtn_Stampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        If Not IsNumeric(cmb_Stampe.SelectedItem.Value) Then
            Exit Sub
        End If

        Dim reportselezionato As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe
        reportselezionato = cmb_Stampe.SelectedItem.Value
        gestisciStampa(reportselezionato)


    End Sub

    Protected Sub BtnOperazionePref1_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref1.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref1.CommandName)
        gestisciStampa(lavcod)
    End Sub

    Protected Sub BtnOperazionePref2_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref2.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref2.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref3_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref3.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref3.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref4_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref4.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref4.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref5_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref5.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref5.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref6_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref6.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref6.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref7_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref7.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref7.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref8_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref8.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref8.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref9_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref9.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref9.CommandName)
        gestisciStampa(lavcod)
    End Sub
    Protected Sub BtnOperazionePref10_Click(sender As Object, e As EventArgs) Handles BtnOperazionePref10.Click
        Dim lavcod As Integer = CInt(BtnOperazionePref10.CommandName)
        gestisciStampa(lavcod)
    End Sub





    Private Sub gestisciStampa(ByVal reportselezionato As enum_CodificaStampe)
        Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010

        Dim piva As String = objParametriAgenda.Piva


        Dim Origine As String
        Dim Destinazione As String
        Dim Funzione As String

        Origine = Stringa_Codifica(
                    "../Stampe/MenuStampe.aspx",
                    AgroKey_EncoderDecoder, Server)

        Destinazione = Stringa_Codifica(
                        "../GestioneStampe/ChiamaStampe.aspx",
                        AgroKey_EncoderDecoder, Server)

        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa),
                        AgroKey_EncoderDecoder, Server)

        Dim TargetRedirect = ""

        Select Case reportselezionato

            '---------------------- 
            'Scheda Campagna
            '---------------------- 

            Case enum_CodificaStampe.SchedaCampagna_Biologico
                AgroMsgBox("Report in fase di costruzione!", Page)
                Exit Sub
            Case enum_CodificaStampe.SchedaCampagna_Biologico_Semplificata
                AgroMsgBox("Report in fase di costruzione!", Page)
                Exit Sub



                '========================================================================

            Case enum_CodificaStampe.SchedaMateriePrime_Biologico,
                         enum_CodificaStampe.SchedaVendite_Biologico,
                             enum_CodificaStampe.SchedaPreparati_Biologico


                Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                  AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                              reportselezionato,
                              piva,
                              Session,
                              objParametri_Server)

                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                    Enum_SiteRedirector.Sito_GiasOnline,
                                    ParametriAgronicaStampe)
                Response.Redirect(link)



                Exit Sub

                '========================================================================

            Case enum_CodificaStampe.PAP_Vegetale


                Dim Qs_Piva As String = ""
                Dim UserName As String
                Dim Password As String
                Dim User_Profilo As String
                Dim UserName_CodFisc As String
                Dim UserProfilo_CodFisc As String

                '----- Utente

                UserName = Session("ASG_Utente_Username").ToString
                Password = Session("ASG_Utente_Password").ToString
                User_Profilo = Session("ASG_SuperUser_Username").ToString
                UserProfilo_CodFisc = Session("ASG_SuperUser_CodFiscale").ToString

                UserName_CodFisc = Session("ASG_Utente_CodFiscale")

                Dim XmlParametri As String

                If Not piva Is Nothing Then
                    piva = piva.ToString
                End If


                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(
                        AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_GiasOnline,
                        enum_CodificaPagBio.PAP_Vegetale,
                        Session("ASG_Utente_CodFiscale").ToString,
                        piva)
                Response.Redirect(link)

                Exit Sub

                '========================================================================

            Case enum_CodificaStampe.Notifica_Biologico


                Dim Qs_Piva As String = ""
                Dim UserName As String
                Dim Password As String
                Dim User_Profilo As String
                Dim UserName_CodFisc As String
                Dim UserProfilo_CodFisc As String

                UserName = Session("ASG_Utente_Username").ToString
                Password = Session("ASG_Utente_Password").ToString
                User_Profilo = Session("ASG_SuperUser_Username").ToString
                UserProfilo_CodFisc = Session("ASG_SuperUser_CodFiscale").ToString

                UserName_CodFisc = Session("ASG_Utente_CodFiscale")

                '---  Creazione della stringa XML dei parametri  

                Dim XmlParametri As String


                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(
                        AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_GiasOnline,
                        enum_CodificaPagBio.Notifica,
                        Session("ASG_Utente_CodFiscale").ToString,
                        piva)
                Response.Redirect(link)
                Exit Sub

                '========================================================================

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze,
                    enum_CodificaStampe.SchedaMagazzinoMovimenti,
                         enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                            enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari,
                            enum_CodificaStampe.RiepilogoProdottiUtilizzati



                Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                    AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                reportselezionato,
                                piva,
                                Session,
                                objParametri_Server,
                                "",
                                0,
                                0,
                                0,
                                0, 0)

                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                    Enum_SiteRedirector.Sito_GiasOnline,
                    ParametriAgronicaStampe)
                Response.Redirect(link)


                Exit Sub


                '========================================================================


            Case enum_CodificaStampe.RiepilogoImpiegoSuperfici

                Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                  AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                              reportselezionato,
                              piva,
                              Session,
                              objParametri_Server)

                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                    Enum_SiteRedirector.Sito_GiasOnline,
                                    ParametriAgronicaStampe)

                Response.Redirect(link)

                '========================================================================

            Case enum_CodificaStampe.Esporta_GiasToSap

                Dim i, j As Integer
                Dim XmlDoc As New System.Xml.XmlDocument

                Dim LinkPaginaStampa As String = "../GestioneStampe/ChiamaStampe.aspx"
                Dim LinkSitoStampe As String = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                         enum_CodificaStampe.Esporta_GiasToSap,
                                                                         CStr(Session("ASG_Utente_Username")),
                                                                         CStr(Session("ASG_ProgressivoGIAS")),
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "")

                Response.Redirect(link)
                Exit Sub


                '========================================================================



            Case enum_CodificaStampe.Costo_Manodopera_XLS


                Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_Manodopera
                objGiasOnline.Piva = objParametriAgenda.Piva
                objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & reportselezionato

                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                          Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                          objGiasOnline)
                Response.Redirect(link)
                '========================================================================

            Case enum_CodificaStampe.Costo_ParcoMacchine_XLS

                Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_ParcoMacchine
                objGiasOnline.Piva = objParametriAgenda.Piva
                objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & reportselezionato


                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                          Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                          objGiasOnline)
                Response.Redirect(link)
                '========================================================================



                'Case enum_CodificaStampe.Esportazione_OP_Inv


                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Inv), _
                '                                AgroKey_EncoderDecoder, Server)

                '    '========================================================================

                'Case enum_CodificaStampe.Esportazione_OP_Gest

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest), _
                '                                AgroKey_EncoderDecoder, Server)


                'Case enum_CodificaStampe.Esportazione_OP_Gest_Coop

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest_Coop), _
                '                                AgroKey_EncoderDecoder, Server)

                '    '========================================================================


            Case enum_CodificaStampe.Report_RiconversioneVarietale

                Dim LinkSitoStampe, LinkpaginaStampa As String
                Dim XmlDoc As New System.Xml.XmlDocument


                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                         enum_CodificaStampe.Report_RiconversioneVarietale,
                                                                         CStr(Session("ASG_Utente_Username")),
                                                                         CStr(Session("ASG_ProgressivoGIAS")),
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "")

                Response.Redirect(link)

                Exit Sub

                '========================================================================


            Case enum_CodificaStampe.Report_ImpegnoProduzioneSoci

                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = piva

                Dim objVS As New AgronicaCoreXML.XML_Stampe
                Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)

                Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                objAgronicaStampe.report = reportselezionato
                objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
                objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                objAgronicaStampe.Xml_Generico.Length = 0
                objAgronicaStampe.Xml_Generico.Append(StrNodo)
                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgronicaStampe)

                Response.Redirect(link)

                Exit Sub
                '========================================================================

                'Case enum_CodificaStampe.Esportazione_CellulariContatti


                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_CellulariTecnici), _
                '                                AgroKey_EncoderDecoder, Server)

                '    '========================================================================

                'Case enum_CodificaStampe.Esportatore_Universale_Imprese


                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Imprese), _
                '                                AgroKey_EncoderDecoder, Server)

                '    '========================================================================

                'Case enum_CodificaStampe.Esportatore_Universale_Centri

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Centri), _
                '                                AgroKey_EncoderDecoder, Server)

                '========================================================================


            Case enum_CodificaStampe.Esportatore_Universale_Appezza

                AgroMsgBox("Funzione non ancora attivata!", Page)
                Exit Sub

                '========================================================================


                'Case enum_CodificaStampe.Esportatore_Universale_Impianti

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Impianti), _
                '                                AgroKey_EncoderDecoder, Server)

                '    '========================================================================

                'Case enum_CodificaStampe.Esportatore_Universale_Agenda

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Agenda), _
                '                                AgroKey_EncoderDecoder, Server)

                '========================================================================

                'Case enum_CodificaStampe.Esportatore_Universale_Rintraccio

                '    Dim UtenteAbilitato As Boolean
                '    Dim strDummy As String

                '    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                '    UtenteAbilitato = objPermessi.Controlla_Permessi_Utente( _
                '                         Session("ASG_Utente_Username"), _
                '                            Session("ASG_IdServizio"), _
                '                                                         enum_Security_Attivita.Stampe_Esportazione_Rintraccio, _
                '                                                         enum_Security_Operazione.Modifica, _
                '                                                          Date.Now, "", objParametri_Utenti)

                '    If UtenteAbilitato = True Then

                '        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Rintraccio), _
                '                                    AgroKey_EncoderDecoder, Server)

                '    Else
                '        AgroMsgBox("Permesso negato!", Page)
                '        Exit Sub

                '    End If

                '========================================================================

                '--------------------------------
                'Schede Varie x le OP
                '--------------------------------
            Case enum_CodificaStampe.Impegnative_capitolati

                Dim LinkSitoStampe, LinkPaginaStampa As String

                LinkSitoStampe = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                         enum_CodificaStampe.Impegnative_capitolati,
                                                                         CStr(Session("ASG_Utente_Username")),
                                                                         CStr(Session("ASG_ProgressivoGIAS")),
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         "")

                Response.Redirect(link)

                Exit Sub

                '========================================================================

                'Case enum_CodificaStampe.Bilancio_Fertilizzazioni

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Bilancio_Fertilizzazioni), _
                '                                AgroKey_EncoderDecoder, Server)

                '========================================================================

            Case enum_CodificaStampe.Programmazione_Vegetale


                Dim Qs_Piva As String = ""
                Dim UserName As String
                Dim Password As String
                Dim User_Profilo As String
                Dim UserName_CodFisc As String
                Dim UserProfilo_CodFisc As String

                '----- Utente

                UserName = Session("ASG_Utente_Username").ToString
                Password = Session("ASG_Utente_Password").ToString
                User_Profilo = Session("ASG_SuperUser_Username").ToString
                UserProfilo_CodFisc = Session("ASG_SuperUser_CodFiscale").ToString

                UserName_CodFisc = Session("ASG_Utente_CodFiscale")

                '---  Creazione della stringa XML dei parametri  

                Dim XmlParametri As String

                Dim ParametriPlanning As New AgronicaCoreGestioneRichieste.ParametriPlanning
                ParametriPlanning.PaginaProvenienza = enum_PagineGiasOnline.MenuStampe
                ParametriPlanning.PaginaRichiesta = enum_CodificaPagPlanning.PianificazioneVegetale
                ParametriPlanning.Cuaa = CUAA_from_PIVA(objParametri_Server, piva)
                ParametriPlanning.Piva = piva
                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(
                                     Enum_SiteRedirector.Sito_GiasOnline, ParametriPlanning)

                Response.Redirect(link)

                Exit Sub

                '========================================================================

            Case enum_CodificaStampe.Esportazione_AnagraficaProdotti



                piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                If piva = "" Then
                    'mando al filtrino
                    Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                        "../Stampe/MenuStampe.aspx",
                                                        "../GestioneStampe/ChiamaStampe.aspx",
                                                        Enum_SiteRedirector.Sito_AgronicaStampe,
                                                        reportselezionato)
                    Page.Response.Redirect(Indirizzofiltrino)
                    Exit Sub

                Else
                    Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                    Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                  AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                              reportselezionato,
                              piva,
                              Session,
                              objParametri_Server, rag_soc)

                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)
                    Response.Redirect(link)

                    Exit Sub


                End If

                Exit Sub


                '=======================================================================

                'Case enum_CodificaStampe.Esportazione_AnagraficaContatti



                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Contatti), _
                '                                AgroKey_EncoderDecoder, Server)

                '=======================================================================


            Case enum_CodificaStampe.SchedaTracciabilita_Animale

                'imposto la versione ZOO dell'alberoimprese
                Session("VersioneAlbero") = enum_VersioneAlberoImprese.Albero_Stalle

                'come pagina di ritorno non metto il menùstampe, ma l'alberoimprese
                'perchè così se l'utente vuole cambiare centro, può farlo facendo exit
                'dalla apgian delle consistenze

                If piva <> "" Then
                    'c'è una sola azienda o l'utente vede una sola azienda

                    ''TargetURL = "../GestioneStalle/StalleConsistenze_Info.aspx" & _
                    ''            "?p=" & Stringa_Codifica(CStr(piva), AgroKey_EncoderDecoder, Server) & _
                    ''            "&orig=" & Stringa_Codifica(enum_PagineGiasOnline.AlberoImprese, AgroKey_EncoderDecoder, Server)


                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                    objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.StalleConsistenze_Info
                    objGiasOnline.Piva = objParametriAgenda.Piva

                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                              Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                              objGiasOnline)
                    Response.Redirect(link)
                Else


                    Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                    "../Stampe/MenuStampe.aspx",
                                    ".../GestioneStalle/StalleConsistenze_Info.aspx",
                                    Enum_SiteRedirector.Sito_AgronicaStampe,
                                    reportselezionato)
                    Page.Response.Redirect(Indirizzofiltrino)
                    Exit Sub

                End If

                '========================================================================

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori

                piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                If piva = "" Then
                    'mando al filtrino
                    Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                        "../Stampe/MenuStampe.aspx",
                                                        "../GestioneStampe/ChiamaStampe.aspx",
                                                        Enum_SiteRedirector.Sito_AgronicaStampe,
                                                        enum_CodificaStampe.PacchettoIgiene_RegistroFornitori)
                    Page.Response.Redirect(Indirizzofiltrino)

                    Exit Sub
                Else

                    Dim StrNodiVariabili As String = ""
                    Dim StrNodo As String = ""
                    Dim vVarStampe(0) As ElementoStampe
                    vVarStampe(0).Nome = "piva"
                    vVarStampe(0).Valore = piva
                    StrNodo = XML_VariabiliStampe(vVarStampe)
                    StrNodiVariabili = StrNodiVariabili & StrNodo


                    Dim username As String = Session("ASG_Utente_Username")
                    Dim user_profilo As String = Session("ASG_ProgressivoGIAS")

                    Dim user_profilo_codfiscale As String = Session("ASG_Utente_CodFiscale")
                    Dim Sql_Filtro As String = ""
                    Dim XML_Filtro As String = ""
                    Dim username_codfisc As String = ""

                    Dim utente_codfiscale As String = Session("ASG_Utente_CodFiscale")
                    Dim superuser_username As String = Session("ASG_SuperUser_Username")
                    Dim superuser_codfiscale As String = Session("ASG_SuperUser_CodFiscale")

                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                    reportselezionato,
                                                                    username,
                                                                    user_profilo,
                                                                    StrNodiVariabili,
                                                                    user_profilo_codfiscale,
                                                                    Sql_Filtro,
                                                                    XML_Filtro,
                                                                    username_codfisc,
                                                                    utente_codfiscale,
                                                                    superuser_username,
                                                                    superuser_codfiscale)

                    Response.Redirect(link)

                    Exit Sub

                End If


                Exit Sub

                '========================================================================

            Case enum_CodificaStampe.PacchettoIgiene_RegistroClienti



                piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                If piva = "" Then
                    'mando al filtrino
                    Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                        "../Stampe/MenuStampe.aspx",
                                                        "../GestioneStampe/ChiamaStampe.aspx",
                                                        Enum_SiteRedirector.Sito_AgronicaStampe,
                                                        reportselezionato)
                    Page.Response.Redirect(Indirizzofiltrino)
                    Exit Sub

                Else
                    Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                    Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                  AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                              reportselezionato,
                              piva,
                              Session,
                              objParametri_Server, rag_soc)

                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

                    Response.Redirect(link)

                    Exit Sub

                End If

                Exit Sub

                '========================================================================

            Case enum_CodificaStampe.PacchettoIgiene_SchedaUsoAlimentiOGM

                AgroMsgBox("Stampa in fase di manutenzione.", Page)
                Exit Sub

                '========================================================================


            Case enum_CodificaStampe.PacchettoIgiene_RegistroAlimentazioneStalla

                piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                If piva = "" Then
                    'mando al filtrino
                    Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                        "../Stampe/MenuStampe.aspx",
                                                        "../GestioneStampe/ChiamaStampe.aspx",
                                                        Enum_SiteRedirector.Sito_AgronicaStampe,
                                                        reportselezionato)
                    Page.Response.Redirect(Indirizzofiltrino)

                    Exit Sub


                Else
                    Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                    Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                  AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                              reportselezionato,
                              piva,
                              Session,
                              objParametri_Server, rag_soc)

                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

                    Response.Redirect(link)

                    Exit Sub

                End If

                Exit Sub


                '========================================================================

            Case enum_CodificaStampe.PacchettoIgiene_RegistroRazionamento


                piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                If piva = "" Then
                    'mando al filtrino
                    Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                        "../Stampe/MenuStampe.aspx",
                                                        "../GestioneStampe/ChiamaStampe.aspx",
                                                        Enum_SiteRedirector.Sito_AgronicaStampe,
                                                        reportselezionato)
                    Page.Response.Redirect(Indirizzofiltrino)

                    Exit Sub

                Else
                    Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                    Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                  AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                              reportselezionato,
                              piva,
                              Session,
                              objParametri_Server, rag_soc)

                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

                    Response.Redirect(link)

                    Exit Sub

                End If

                Exit Sub


                '========================================================================

            Case enum_CodificaStampe.PacchettoIgiene_RegistroAnalisiNonConformi

                AgroMsgBox("Stampa in fase di manutenzione.", Page)
                Exit Sub

                '========================================================================


                'Case enum_CodificaStampe.Registro_Fertilizzazioni

                '========================================================================


            Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi

                Dim UtenteAbilitato As Boolean
                Dim strDummy As String      'controllo accesso negato.....

                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                                         Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                                                     enum_Security_Attivita.Report_Accettazione_DaDiversi,
                                                                     enum_Security_Operazione.Lettura,
                                                                     Date.Now, "", objParametri_Utenti)


                If UtenteAbilitato = True Then

                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                    If piva = "" Then
                        'mando al filtrino
                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                            "../Stampe/MenuStampe.aspx",
                                                            "../GestioneStampe/ChiamaStampe.aspx",
                                                            Enum_SiteRedirector.Sito_AgronicaStampe,
                                                            reportselezionato)
                        Page.Response.Redirect(Indirizzofiltrino)

                        Exit Sub

                    Else
                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                        Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                      AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                  reportselezionato,
                                  piva,
                                  Session,
                                  objParametri_Server, rag_soc)

                        Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                            Enum_SiteRedirector.Sito_GiasOnline,
                                            ParametriAgronicaStampe)

                        Response.Redirect(link)

                        Exit Sub

                    End If

                Else
                    AgroMsgBox("Non si dispone dei permessi di stampa di questo report.", Page)
                    Exit Sub
                End If

                Exit Sub






                '========================================================================

            Case enum_CodificaStampe.Registri_Preparazioni

                Dim UtenteAbilitato As Boolean
                Dim strDummy As String      'controllo accesso negato.....
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                                         Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                                                     enum_Security_Attivita.Registri_Cantina,
                                                                     enum_Security_Operazione.Lettura,
                                                                     Date.Now, "", objParametri_Utenti)


                If UtenteAbilitato = True Then
                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                    If piva = "" Then
                        'mando al filtrino
                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                            "../Stampe/MenuStampe.aspx",
                                                            "../GestioneStampe/ChiamaStampe.aspx",
                                                            Enum_SiteRedirector.Sito_AgronicaStampe,
                                                            reportselezionato)
                        Page.Response.Redirect(Indirizzofiltrino)

                        Exit Sub

                    Else
                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                        Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                      AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                  reportselezionato,
                                  piva,
                                  Session,
                                  objParametri_Server, rag_soc)

                        Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                            Enum_SiteRedirector.Sito_GiasOnline,
                                            ParametriAgronicaStampe)

                        Response.Redirect(link)

                        Exit Sub

                    End If

                End If

                Exit Sub

                '========================================================================

            Case enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea

                Dim UtenteAbilitato As Boolean
                Dim strDummy As String      'controllo accesso negato.....
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                                         Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                                                     enum_Security_Attivita.Report_Incongruenze_CatastoVSAgrea,
                                                                     enum_Security_Operazione.Lettura,
                                                                     Date.Now, "", objParametri_Utenti)

                If UtenteAbilitato = True Then

                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                    If piva = "" Then
                        'mando al filtrino
                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                            "../Stampe/MenuStampe.aspx",
                                                            "../GestioneStampe/ChiamaStampe.aspx",
                                                            Enum_SiteRedirector.Sito_AgronicaStampe,
                                                            reportselezionato)
                        Page.Response.Redirect(Indirizzofiltrino)

                        Exit Sub

                    Else
                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                        Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                      AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                  reportselezionato,
                                  piva,
                                  Session,
                                  objParametri_Server, rag_soc)

                        Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                            Enum_SiteRedirector.Sito_GiasOnline,
                                            ParametriAgronicaStampe)
                        Response.Redirect(link)

                        Exit Sub

                    End If


                Else
                    AgroMsgBox("Non si dispone dei permessi di stampa di questo report.", Page)
                    Exit Sub
                End If

                Exit Sub

                'al momento non essendoci un filtro delle operazioni contabili vado al menù agenda
                'la stampa è chiamabile infatti nel menù agenda
            Case enum_CodificaStampe.Bolle, enum_CodificaStampe.Fatture, enum_CodificaStampe.Nota_Accredito

                Dim link As String = "../menu/menu.aspx"
                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

                If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                    link = "../Menu/MenuBS_Agenda_Nuovo.aspx"
                End If

                Response.Redirect(link)

                '========================================================================

                'Case enum_CodificaStampe.ReportRisultatoFilrone
                '   Response.Redirect("../filtrone/filtrone.aspx")


                'Case enum_CodificaStampe.Esportazione_OP_Catasto


                '    objp.Sito_Origine = Stringa_Codifica( _
                '                Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                '                AgroKey_EncoderDecoder, Server)
                '    objp.Pagina_Origine = Stringa_Codifica( _
                '                            "../Stampe/Menu_Stampe.aspx", _
                '                            AgroKey_EncoderDecoder, Server)


                '    objp.Sito_Destinazione = Stringa_Codifica( _
                '                Enum_SiteRedirector.Sito_AgronicaStampe_2010, _
                '                AgroKey_EncoderDecoder, Server)
                '    objp.Pagina_Destinazione = Stringa_Codifica( _
                '                "", _
                '                AgroKey_EncoderDecoder, Server)


                '    objp.TipoFiltrone = Stringa_Codifica( _
                '                           CStr(enum_TipoFiltrone.Stampa), _
                '                           AgroKey_EncoderDecoder, Server)

                '    objp.CodificaStampe = Stringa_Codifica( _
                '                          enum_CodificaStampe.Esportazione_OP_Catasto, _
                '                           AgroKey_EncoderDecoder, Server)


                '    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010( _
                '                                           Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                '                                           objp)

                '    Response.Redirect(link)

                'Case enum_CodificaStampe.Esportazione_OP_Produttori

                '    'nuovo

                '    objp.Sito_Origine = Stringa_Codifica( _
                '                Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                '                AgroKey_EncoderDecoder, Server)
                '    objp.Pagina_Origine = Stringa_Codifica( _
                '                            "../Stampe/Menu_Stampe.aspx", _
                '                            AgroKey_EncoderDecoder, Server)


                '    objp.Sito_Destinazione = Stringa_Codifica( _
                '                Enum_SiteRedirector.Sito_AgronicaStampe_2010, _
                '                AgroKey_EncoderDecoder, Server)
                '    objp.Pagina_Destinazione = Stringa_Codifica( _
                '                "", _
                '                AgroKey_EncoderDecoder, Server)


                '    objp.TipoFiltrone = Stringa_Codifica( _
                '                           CStr(enum_TipoFiltrone.Stampa), _
                '                           AgroKey_EncoderDecoder, Server)

                '    objp.CodificaStampe = Stringa_Codifica( _
                '                          enum_CodificaStampe.Esportazione_OP_Produttori, _
                '                           AgroKey_EncoderDecoder, Server)


                'Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010( _
                '                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                '                                       objp)

                'Response.Redirect(link)

                ' End If



            Case enum_CodificaStampe.Registro_Fertilizzazioni_Massivo, enum_CodificaStampe.Registro_Trattamenti_Massivo

                Dim UtenteAbilitato As Boolean
                Dim strDummy As String      'controllo accesso negato.....

                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                                         Session("ASG_Utente_Username"),
                                                         Session("ASG_IdServizio"),
                                                        enum_Security_Attivita.Stampa_SchedaCampagna_Massiva,
                                                        enum_Security_Operazione.Lettura,
                                                        Date.Now, "", objParametri_Utenti)


                If UtenteAbilitato = True Then

                    'verifico in configurazione_siti
                    'se devo leggere le aziende da tabella
                    'se devo passare dal filtrone

                    Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dt As DataTable = cf.Leggi(0, "Stampe_Massive_Filtro", " valore = 'false' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))

                    If dt.Rows.Count = 1 Then

                        'devo leggere le aziende da tabella
                        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                        objAgronicaStampe.report = reportselezionato

                        'Verifico le stampe che devono avere la specie selezionata
                        Dim strVegCod As String = ""
                        Dim filtroImp As String = ""

                        Dim XmlDoc As New System.Xml.XmlDocument
                        Dim Xml_FiltroStampa As System.Xml.XmlElement
                        Dim StrVariabiliStampe As String = ""
                        Dim StrNodiVariabili As String = ""
                        Dim StrNodo As String = ""
                        Dim objVS As New AgronicaCoreXML.XML_Stampe

                        objAgronicaStampe.username = CStr(Session("ASG_Utente_Username"))
                        objAgronicaStampe.user_profilo = CStr(Session("ASG_ProgressivoGIAS"))
                        objAgronicaStampe.Xml_Generico.Length = 0
                        objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                         objAgronicaStampe)

                        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel),
                                                            CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel).GetType(),
                                                            "jQuery_{0}", strJS, False)

                        Exit Sub


                    Else

                        objp.Sito_Origine = Stringa_Codifica(
                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                     AgroKey_EncoderDecoder, Server)
                        objp.Pagina_Origine = Stringa_Codifica(
                                                "../Stampe/Menu_Stampe.aspx",
                                                AgroKey_EncoderDecoder, Server)


                        objp.Sito_Destinazione = Stringa_Codifica(
                                    Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                                    AgroKey_EncoderDecoder, Server)
                        objp.Pagina_Destinazione = Stringa_Codifica(
                                    "../GestioneStampe/ChiamaStampe.aspx",
                                    AgroKey_EncoderDecoder, Server)


                        objp.TipoFiltrone = Stringa_Codifica(
                                               CStr(enum_TipoFiltrone.Stampa),
                                               AgroKey_EncoderDecoder, Server)

                        objp.CodificaStampe = Stringa_Codifica(
                                              reportselezionato,
                                               AgroKey_EncoderDecoder, Server)
                    End If


                Else
                    AgroMsgBox("Non si dispone dei permessi di stampa massiva.", Page)
                    Exit Sub
                End If

                '-------------------------------------------------

            Case enum_CodificaStampe.EsportazioneAgeaTxtCSV

                objp.Sito_Origine = Stringa_Codifica(
                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                            AgroKey_EncoderDecoder, Server)
                objp.Pagina_Origine = Stringa_Codifica(
                                        "../Stampe/Menu_Stampe.aspx",
                                        AgroKey_EncoderDecoder, Server)


                objp.Sito_Destinazione = Stringa_Codifica(
                            Enum_SiteRedirector.Sito_AgronicaSincronizzatore,
                            AgroKey_EncoderDecoder, Server)
                objp.Pagina_Destinazione = Stringa_Codifica(
                            enum_PagineAgronicaSincro.EsportazioneAGEATxtCsv,
                            AgroKey_EncoderDecoder, Server)


                objp.TipoFiltrone = Stringa_Codifica(
                                       CStr(enum_TipoFiltrone.Esportazione_AgeaTXTCSV),
                                       AgroKey_EncoderDecoder, Server)

                objp.CodificaStampe = Stringa_Codifica(
                                      reportselezionato,
                                       AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Bilancio_Fertilizzazioni, enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato

                objp.Sito_Origine = Stringa_Codifica(
                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                   AgroKey_EncoderDecoder, Server)
                objp.Pagina_Origine = Stringa_Codifica(
                                        "../Stampe/Menu_Stampe.aspx",
                                        AgroKey_EncoderDecoder, Server)


                objp.Sito_Destinazione = Stringa_Codifica(
                            Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                            AgroKey_EncoderDecoder, Server)
                objp.Pagina_Destinazione = Stringa_Codifica(
                            "../GestioneStampe/ChiamaStampe.aspx",
                            AgroKey_EncoderDecoder, Server)


                objp.TipoFiltrone = Stringa_Codifica(
                                       CStr(enum_TipoFiltrone.Stampa),
                                       AgroKey_EncoderDecoder, Server)

                objp.CodificaStampe = Stringa_Codifica(
                                      reportselezionato,
                                       AgroKey_EncoderDecoder, Server)

                TargetRedirect = "../Filtrone/Filtrone_Nuovo.aspx?p_o=" & objp.Pagina_Origine &
                           "&s_o=" & objp.Sito_Origine &
                           "&p_d=" & objp.Pagina_Destinazione &
                           "&s_d=" & objp.Sito_Destinazione &
                           "&t_f=" & objp.TipoFiltrone &
                           "&c_s=" & objp.CodificaStampe &
                           "&v_c=" & objp.Veg_Cod &
                           "&c_c=" & objp.Cul_Cod &
                           "&d_i=" & objp.Data_Inizio &
                           "&d_f=" & objp.Data_Fine

            Case enum_CodificaStampe.ReportConserveItalia

                'Costruisco il link
                Dim linkBuilder As New UriBuilder
                linkBuilder.Scheme = HttpContext.Current.Request.Url.Scheme
                linkBuilder.Host = HttpContext.Current.Request.Url.Host
                linkBuilder.Port = HttpContext.Current.Request.Url.Port

                Dim sitoOrigine As String = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder)
                Dim sitoDestinazione As String = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder)
                Dim tipoOpFiltrone As String = Stringa_Codifica(enum_TipoFiltrone.Stampa, AgroKey_EncoderDecoder) 'enum_TipoFiltrone.OperazioniMultiAziendali
                Dim codificaStampe As String = Stringa_Codifica(reportselezionato, AgroKey_EncoderDecoder)
                Dim categoriaOutput As String = Stringa_Codifica("impianto", AgroKey_EncoderDecoder)
                Dim tipoFiltrino As String = Stringa_Codifica("0", AgroKey_EncoderDecoder)

                'Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                'Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                'Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "FiltroneBootstrap", "", "", objParametri_Server)
                'If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                '    linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/Filtrone/Filtrone_Nuovo.aspx")
                'Else
                '    linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/Filtrone/Filtrone.aspx")
                'End If

                linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/Filtrone/Filtrone_Nuovo.aspx")
                linkBuilder.Query = "s_o=" & sitoOrigine & "&p_o=" & Origine & "&p_d=" & codificaStampe & "&s_d=" & sitoDestinazione &
                    "&t_f=" & tipoOpFiltrone & "&c_s=" & codificaStampe & "&cat=" & categoriaOutput & "&f=" & tipoFiltrino &
                    "&d_i=" & objp.Data_Inizio & "&d_f=" & objp.Data_Fine &
                    "&v_c=" & objp.Veg_Cod & "&c_c=" & objp.Cul_Cod

                Response.Redirect(linkBuilder.ToString())

            Case Else


                ' ''Case enum_CodificaStampe.SchedaCampagna_2078
                ' ''Case enum_CodificaStampe.RegistroTrattamenti
                ' ''Case enum_CodificaStampe.SchedaRegistrazione
                ' ''Case enum_CodificaStampe.RegistroTrattamenti_Veneto
                ' ''Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata
                ' ''Case enum_CodificaStampe.SchedaCampagna_Multicentro
                ' ''Case enum_CodificaStampe.RegistroTrattamenti_Semplificata
                ' ''Case enum_CodificaStampe.SchedaRegistrazione_Semplificata
                ' ''Case enum_CodificaStampe.Eurep_Gap
                ' ''Case enum_CodificaStampe.Eurep_Gap_Semplificata
                ' ''Case enum_CodificaStampe.Eurep_Gap_Multicentro
                ' ''Case enum_CodificaStampe.Quadro_P
                ' ''Case enum_CodificaStampe.PianoRaccolta
                ' ''Case enum_CodificaStampe.SchedaColturale_Biologico
                ' ''Case enum_CodificaStampe.RiepilogoImpiegoSuperfici_Multiazienda
                ' ''Case enum_CodificaStampe.SchedaTracciabilita
                ' ''Case enum_CodificaStampe.ReportConserveItalia
                ' ''Case enum_CodificaStampe.SchedaCampagna_ConserveItalia
                ' ''Case enum_CodificaStampe.DatiAnelloFilieraIngresso
                ' ''Case enum_CodificaStampe.DatiAnelloFilieraLegameLotti
                ' ''Case enum_CodificaStampe.EstrattoreDatiGrafici
                ' ''Case enum_CodificaStampe.PianoColturale
                ' ''Case enum_CodificaStampe.ReportRisultatoFilrone
                ' ''Case enum_CodificaStampe.SchedaCampagna_Pizzoli


                objp.Sito_Origine = Stringa_Codifica(
                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                            AgroKey_EncoderDecoder, Server)
                objp.Pagina_Origine = Stringa_Codifica(
                                        "../Stampe/Menu_Stampe.aspx",
                                        AgroKey_EncoderDecoder, Server)


                objp.Sito_Destinazione = Stringa_Codifica(
                            Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                            AgroKey_EncoderDecoder, Server)
                objp.Pagina_Destinazione = Stringa_Codifica(
                            "../GestioneStampe/ChiamaStampe.aspx",
                            AgroKey_EncoderDecoder, Server)


                objp.TipoFiltrone = Stringa_Codifica(
                                       CStr(enum_TipoFiltrone.Stampa),
                                       AgroKey_EncoderDecoder, Server)

                objp.CodificaStampe = Stringa_Codifica(
                                      reportselezionato,
                                       AgroKey_EncoderDecoder, Server)

        End Select

        '--------------------------------------------------
        If TargetRedirect = "" Then
            TargetRedirect = "../Filtrone/Filtrone.aspx?p_o=" & objp.Pagina_Origine &
                           "&s_o=" & objp.Sito_Origine &
                           "&p_d=" & objp.Pagina_Destinazione &
                           "&s_d=" & objp.Sito_Destinazione &
                           "&t_f=" & objp.TipoFiltrone &
                           "&c_s=" & objp.CodificaStampe &
                           "&v_c=" & objp.Veg_Cod &
                           "&c_c=" & objp.Cul_Cod &
                           "&d_i=" & objp.Data_Inizio &
                           "&d_f=" & objp.Data_Fine
        End If

        Response.Redirect(TargetRedirect)

    End Sub



    Private Sub MenuStampe_OLD_Click(sender As Object, e As System.EventArgs) Handles MenuStampe_OLD.Click

        AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline(enum_PagineGiasOnline.MenuStampe, objParametriAgenda, Page)

    End Sub

End Class