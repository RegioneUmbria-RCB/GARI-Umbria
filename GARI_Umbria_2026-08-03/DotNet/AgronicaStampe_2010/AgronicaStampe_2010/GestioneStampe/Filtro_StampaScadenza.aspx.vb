Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.ConnessioniTransazioni
Imports AgronicaCoreDataProvider.Sicurezza
Imports System.Text

Public Class Filtro_StampaScadenza
    Inherits System.Web.UI.Page

    '----- objParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '----- Variabili globali nella pagina
    Dim Qs_PaginaStampa As String

    Dim Qs_AttivitaPermessi As String
    Dim Qs_OperazionePermessi As String

    Dim Qs_Piva As String
    Dim QS_PUA_Cod As Integer

    Dim QS_Scadenziario As Integer
    Dim Qs_FilePDF As String


    '######################################################################################################
    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub

    '######################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        If Not IsNothing(Request.QueryString("p")) Then

            Qs_PaginaStampa = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                                 AgroKey_EncoderDecoder, _
                                                 Server)
        Else
            Qs_PaginaStampa = ""
        End If

        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        If Not IsNothing(Request.QueryString("a")) Then

            Qs_AttivitaPermessi = Stringa_Decodifica(Request.QueryString("a").ToString, _
                                                      AgroKey_EncoderDecoder, _
                                                      Server)
        Else
            Qs_AttivitaPermessi = ""
        End If

        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        If Not IsNothing(Request.QueryString("o")) Then

            Qs_OperazionePermessi = Stringa_Decodifica(Request.QueryString("o").ToString, _
                                                      AgroKey_EncoderDecoder, _
                                                      Server)
        Else
            Qs_OperazionePermessi = ""
        End If

        If Not IsNothing(Request.QueryString("piva")) Then

            Qs_Piva = Stringa_Decodifica(Request.QueryString("piva").ToString, _
                                                      AgroKey_EncoderDecoder, _
                                                      Server)
        Else
            Qs_Piva = ""
        End If


        If Not IsNothing(Request.QueryString("pua")) Then

            QS_PUA_Cod = Stringa_Decodifica(Request.QueryString("pua").ToString, _
                                                      AgroKey_EncoderDecoder, _
                                                      Server)
        Else
            QS_PUA_Cod = 0
        End If

        If Not IsNothing(Request.QueryString("scadenziario")) Then

            QS_Scadenziario = Stringa_Decodifica(Request.QueryString("scadenziario").ToString, _
                                                      AgroKey_EncoderDecoder, _
                                                      Server)
        Else
            QS_Scadenziario = "0"
        End If


        If Not IsNothing(Request.QueryString("pdf")) Then

            Qs_FilePDF = Stringa_Decodifica(Request.QueryString("pdf").ToString, _
                                                      AgroKey_EncoderDecoder, _
                                                      Server)
        Else
            Qs_FilePDF = ""
        End If



        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim strDummy As String      'controllo accesso negato.....
        Dim UtenteAbilitato As Boolean

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente( _
                            Session("ASG_Utente_Username"), _
                            Session("ASG_IdServizio"), _
                            Qs_AttivitaPermessi, _
                            Qs_OperazionePermessi, _
                            Date.Now, _
                            "", _
                            objParametri_Utenti)

        If UtenteAbilitato = False Then

            Response.Redirect("Messaggi/AccessoNegato.htm")

        End If

        '######################################################################################################################
        '######################################################################################################################

        Session.Timeout = 180

        Dim UtenteAbilitatoScadenziario As Boolean
        UtenteAbilitatoScadenziario = objPermessi.Controlla_Permessi_Utente( _
                            Session("ASG_Utente_Username"), _
                            Session("ASG_IdServizio"), _
                            AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Attivita.Scadenziario_Menu, _
                            AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Operazione.Modifica, _
                            Date.Now, _
                            "", _
                            objParametri_Utenti)

        If Not UtenteAbilitatoScadenziario Then

            Qs_PaginaStampa &= "&anteprima=" + Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server)

            Response.Redirect(Qs_PaginaStampa)

        End If

        If QS_Scadenziario = "1" Then
            ApriScadenziario()
        End If



    End Sub



    Private Sub InserisciScriptClient()


    End Sub


    '######################################################################################################################
    Private Sub ImgBtn_Stampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        If RblOpzioni.SelectedItem.Value = 1 Then

            Qs_PaginaStampa &= "&anteprima=" + Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server)

            Response.Redirect(Qs_PaginaStampa)

        Else

            Qs_PaginaStampa &= "&anteprima=" + Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server)

            Response.Redirect(Qs_PaginaStampa)

        End If


    End Sub

    Private Sub ApriScadenziario()

        'If IsNothing(Session("Analisi_Testata_Cod")) OrElse Session("Analisi_Testata_Cod") = 0 Then
        '    AgronicaCoreUtility.Messaggi.AgroMsgBox("Impossibile associare un allegato ad una nuova analisi ", Page)
        '    Exit Sub
        'End If

        ' Dim objXmlPassaggio As New AgronicaCoreGestioneRichieste.ScriviXml
        'ha già creato i parametri di sessione e i parametri del web config
        Dim ParametriScadenziario As New AgronicaCoreGestioneRichieste.ParametriScadenziario
        ParametriScadenziario.Pagina_Richiesta = AgronicaCoreDataProvider.TipiEnumerativi.enum_PagineGiasOnline_2010.Nuova_Scandenza
        ParametriScadenziario.Piva = Qs_Piva

        ParametriScadenziario.Id_Area = AgronicaCoreDataProvider.TipiEnumerativi.enum_ID_Area_Alert.Nitrati
        ParametriScadenziario.Id_Tipologia = AgronicaCoreDataProvider.TipiEnumerativi.enum_ID_Area_Tipologia.PUA
        ParametriScadenziario.PUA_Cod = QS_PUA_Cod
        ParametriScadenziario.PathFile = Qs_FilePDF

        'If IsDate(Txt_DataFine.Text) Then
        '    objXmlPassaggio.ParametriScadenziario.Data_Scadenza = Txt_DataFine.Text
        'Else
        'objXmlPassaggio.ParametriScadenziario.Data_Scadenza = Now.Date.ToShortDateString
        ' End If


        'Dim script As String

        'Dim objAgroWebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        'script = objXmlPassaggio.ApriIframeConSito(objAgroWebconfig.LinkGiasOnline_2010, AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaPUA, AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_GiasOnline_2010)


        Dim script As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriIFrame_SitoOnline2010_PassandoDirettamente_ParametriScadenziario(AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaStampe_2010, ParametriScadenziario)

        'ScriptManager.RegisterClientScriptBlock(Update_Principale, Update_Principale.GetType(), String.Format("jQuery_{0}", Update_Principale.ClientID), script, True)



        'aggiungo lo script 
        ClientScript.RegisterClientScriptBlock(Page.GetType, "frame", script)


    End Sub




    ' ##################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      window.close() ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                  String.Format("jQuery_{0}", Me.Page.ClientID), strJS1.ToString, True)

    End Sub

End Class