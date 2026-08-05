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

Imports Importazioni_OPTA

Imports AgronicaCoreWebService

Public Class AnalisiRitiriTabacco
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public Master_Operazione As Agenda

    Private Sub Menu_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, Agenda)
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

        'AlberoAnagrafica.Flag_Singola_Selezione = False
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Expires = 0

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("../Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        CType(Master.FindControl("Lbl_Titolo"), Label).Text = "Analisi Dati Ritiri Tabacco"



        InizializzaVarie()

        Dim ispostbackscript As String = "var isPostBack = false;"

        If Not IsPostBack Then

            DistruggiSessionVecchie()



            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================

            Dim UtenteAbilitato_Lettura As Boolean = False

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                        Session("ASG_Utente_Username"), _
                                        Session("ASG_IdServizio"), _
                                        enum_Security_Attivita.Gest_Magazzino, _
                                        enum_Security_Operazione.Lettura, _
                                        Date.Now, _
                                        "", _
                                        objParametri_Utenti)

            ViewState("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

            Dim UtenteAbilitato_Modifica As Boolean = False
            UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Gest_Magazzino, _
                                       enum_Security_Operazione.Modifica, _
                                       Date.Now, _
                                       "", _
                                       objParametri_Utenti)

            ViewState("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica


            '##############################################################
            '#####  Inizializzo i controlli  ##############################
            '##############################################################
            ImpostaPermessi()

            CaricaImprese()

        End If




        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "IsPostBack", ispostbackscript, True)

    End Sub


    Private Sub DistruggiSessionVecchie()
        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")
    End Sub


    Public Sub ImpostaPermessi()
        'Controllo se ha il permesso di lettura
        If ViewState("UtenteAbilitato_Lettura") = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If objParametri_Server.UtenteCodFiscale = objParametri_Server.PivaSuperUser Then
           
            DivReport.Visible = True
            DivImportaRicevimenti.Visible = True
            DivRintraccia.Visible = True

        Else
            DivReport.Visible = False
            DivImportaRicevimenti.Visible = False
            DivRintraccia.Visible = False
        End If


    End Sub


    Private Sub InizializzaVarie()

        ScriptManager.RegisterStartupScript(UpdatePanelscript, UpdatePanelscript.GetType(),
                                         String.Format("jQuery_{0}", ComboImprese.ClientID), ComboImprese.GetJS(), True)


        Dim StrSelect As New StringBuilder
        StrSelect.AppendLine("$(document).ready(function () { ")
         StrSelect.AppendLine("       ('#WaitFrame').hide();   ")
        StrSelect.AppendLine("});")
        ScriptManager.RegisterStartupScript(UpdatePanelscript, UpdatePanelscript.GetType(),
                                     String.Format("jQuery_{0}", ComboImprese.ClientID), StrSelect.ToString, True)


    End Sub


    Private Sub CaricaImprese()
        ComboImprese.Piva = ""
        ComboImprese.CaricaComboImprese()
        ComboImprese.Valore_Combo = 0
    End Sub


    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        'Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))

        Dim strJS As New StringBuilder
        strJS.AppendLine("$(document).ready(function () { ")
        strJS.AppendLine("      window.close(); ")
        strJS.AppendLine(" });")
        ScriptManager.RegisterStartupScript( _
           UpdatePanelscript, _
           UpdatePanelscript.GetType(), _
               String.Format("jQuery_{0}", UpdatePanelscript.ClientID), strJS.ToString, True)
        Exit Sub

    End Sub


  
    Protected Sub ImageButtonAnalizza_Impresa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonAnalizza_Impresa.Click

    End Sub

    Protected Sub ImageButtonImporta_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonImporta.Click
        Dim msg As String = ""
        Dim res As Boolean = False
        res = ImportaRicevimenti(msg)

        If res Then
            LabelRes.BackColor = Drawing.Color.GreenYellow
            LabelRes.Text = "Fatto" & msg
        Else
            LabelRes.BackColor = Drawing.Color.Pink
            LabelRes.Text = msg
        End If
    End Sub

    Protected Sub ImageButtonAnalizza_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonAnalizza.Click
        Dim msg As String = ""
        Dim res As Boolean = False
        res = rintraccia(msg)

        If res Then
            LabelRes.BackColor = Drawing.Color.GreenYellow
            LabelRes.Text = "Fatto" & msg
        Else
            LabelRes.BackColor = Drawing.Color.Pink
            LabelRes.Text = msg
        End If
    End Sub

    Private Function ImportaRicevimenti(ByRef msg As String) As Boolean

        msg = ""
        'Dim Opta_Ws As AgronicaCoreWebService.Opta_Ws
        Dim res As Boolean = False
        Select Case RadioButtonListImporta.SelectedValue
            Case "1"
                If chkCosaImportareRitiro.Checked Then
                    res = Opta_Ws.Importa_Tutti_Carichi(msg, objParametri_Server)
                End If
                If chkCosaImportareLavorazione.Checked Then
                    res = Opta_Ws.Importa_Tutti_Lavorazione(msg, objParametri_Server)
                End If
                If chkCosaImportareProdotto.Checked Then
                    res = Opta_Ws.Importa_Tutti_ProdottoFinito(msg, objParametri_Server)
                End If


            Case "2"
                If chkCosaImportareRitiro.Checked Then
                    res = Opta_Ws.Importa_Ultimi_Carichi(msg, objParametri_Server)
                End If
                If chkCosaImportareLavorazione.Checked Then
                    res = Opta_Ws.Importa_Ultimi_Lavorazione(msg, objParametri_Server)
                End If
                If chkCosaImportareProdotto.Checked Then
                    res = Opta_Ws.Importa_Ultimi_ProdottoFinito(msg, objParametri_Server)
                End If


            Case "3"
                If chkCosaImportareRitiro.Checked Then
                    res = Opta_Ws.Importa_Carichi_Mancanti(msg, objParametri_Server)
                End If

                'TODO: realizzare cosa analoga su lavorazione, prodotto finito

            Case "4"

                'TODO: realizzare test su ritiro

                If chkCosaImportareLavorazione.Checked Then
                    res = Opta_Ws.Importa_Test_Lavorazione(msg, objParametri_Server)
                End If
                If chkCosaImportareProdotto.Checked Then
                    res = Opta_Ws.Importa_Test_ProdottoFinito(msg, objParametri_Server)
                End If


            Case Else
                msg = "Non Previsto"
        End Select

        Return res

    End Function

    Private Function rintraccia(msg As String) As Boolean
        msg = ""
        Dim Opta_Ws As AgronicaCoreWebService.Opta_Ws
        Dim res As Boolean = False
        Select Case RadioButtonListImporta.SelectedValue
            Case "1"
                Dim Solonuovi As Boolean = False
                If chkCosaImportareRitiro.Checked Then
                    res = Importazioni_OPTA.Rintraccio.RintracciaTuttiLotti_GeneraTabellaRitiroXImpiantiRaccolti(Solonuovi, 0, 0, "", "", "", msg, objParametri_Server)
                End If
                If chkCosaImportareLavorazione.Checked Then
                    res = Importazioni_OPTA.Rintraccio.Rintraccia_AssociaImmesso_ProdFinito(True, msg, objParametri_Server)
                End If
            Case "2"
                Dim Solonuovi As Boolean = True
                If chkCosaImportareRitiro.Checked Then
                    res = Importazioni_OPTA.Rintraccio.RintracciaTuttiLotti_GeneraTabellaRitiroXImpiantiRaccolti(Solonuovi, 0, 0, "", "", "", msg, objParametri_Server)
                End If
                If chkCosaImportareLavorazione.Checked Then
                    res = Importazioni_OPTA.Rintraccio.Rintraccia_AssociaImmesso_ProdFinito(True, msg, objParametri_Server)
                End If
            Case Else
                msg = "Non Previsto"
        End Select

        Return res
    End Function

End Class