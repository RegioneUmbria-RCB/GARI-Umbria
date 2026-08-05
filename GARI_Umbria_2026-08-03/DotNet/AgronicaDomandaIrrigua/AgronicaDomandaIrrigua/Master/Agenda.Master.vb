Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaControlli_2010
Imports Agronica.Helpers.GiasBase
Public Class Agenda
    Inherits System.Web.UI.MasterPage

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public debug_isattached As Boolean = False

    Private _LinkGiasBase As String = String.Empty
    Public ReadOnly Property PATH_GIASBASE As String
        Get
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Server", _LinkGiasBase)
            Return _LinkGiasBase
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Debugger.IsAttached Then
            debug_isattached = True
        End If

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Server", _LinkGiasBase)

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        ScriptxLoading()

        'PARTITA IVA
        Dim objParametriAgenda As New ParametriAgenda
        'objParametriAgenda.Leggi()

        If objParametriAgenda.Piva <> "" Then
            If objParametriAgenda.RagSoc = "" Then
                'lo leggo solamente una volta
                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                objParametriAgenda.RagSoc = objImprese.RagSoc_from_Piva(objParametriAgenda.Piva, objParametri_Server)
            End If
            Lbl_Rag_Soc.Text = objParametriAgenda.RagSoc
        End If

        '##############################################################
        'Indicazione Finestra temporale
        '##############################################################

        If objParametri_Server.FinestraTemporaleInizio = "01/01/1900" And objParametri_Server.FinestraTemporaleFine = "31/12/2100" Then

            lbl_FinestraTemporale.Text = Resources.AgronicaAgenda_2010.VisualizzazioneIllimitata

        Else
            lbl_FinestraTemporale.Text = Resources.AgronicaAgenda_2010.VisualizzazioneLimitataTraIl &
                IIf(objParametri_Server.FinestraTemporaleInizio = "01/01/1900", "...", objParametri_Server.FinestraTemporaleInizio) & _
                " e il " &
            IIf(objParametri_Server.FinestraTemporaleFine = "31/12/2100", "...", objParametri_Server.FinestraTemporaleFine)

        End If
    End Sub


    Private Sub ScriptxLoading()

        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")
        Str.AppendLine("    $('#WaitFrame').hide(); ")
        Str.AppendLine("    $('.btn_per_load').click( ")
        Str.AppendLine("        function() { ")
        Str.AppendLine("            $('#WaitFrame').show();")
        Str.AppendLine("        }); ")

        Str.AppendLine("    $('.change_per_load').change( ")
        Str.AppendLine("        function() { ")
        Str.AppendLine("            $('#WaitFrame').show();")
        Str.AppendLine("        }); ")
        Str.AppendLine("    }); ")

        ScriptManager.RegisterStartupScript(updateScript, updateScript.GetType(),
                                        String.Format("jQuery_{0}", updateScript.ClientID), Str.ToString, True)

    End Sub


    Public Property Property_Lbl_Rag_Soc() As Global.System.Web.UI.WebControls.Label
        Get
            Return Lbl_Rag_Soc
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            Lbl_Rag_Soc = value
        End Set
    End Property

    Public Property Property_lbl_FinestraTemporale() As Global.System.Web.UI.WebControls.Label
        Get
            Return lbl_FinestraTemporale
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            lbl_FinestraTemporale = value
        End Set
    End Property

    Public Property Property_Lbl_Titolo() As Global.System.Web.UI.WebControls.Label
        Get
            Return Lbl_Titolo
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            Lbl_Titolo = value
        End Set
    End Property

    Public Property Property_ImgBtn_AnnullaTutto() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return ImgBtn_AnnullaTutto
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            ImgBtn_AnnullaTutto = value
        End Set
    End Property



    Public Function TrovaRedirectCorretto(ByVal online As Boolean, ByVal paginaRichiesta As enum_PagineGiasOnline, ByVal objParametriAgenda As ParametriAgenda) As String

        Dim TargetMenuAgenda As String = "../Menu/Menu.aspx"

        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable
        'DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)
        'If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0)("valore")) = "true" Then
        '    TargetMenuAgenda = "../menu/menubs_2017.aspx"
        'Else
        DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
            TargetMenuAgenda = "../menu/menubs_agenda_nuovo.aspx"
        End If
        'End If

        If online = False Then
            ' Se la sessione di provenienza è AgroAgenda, torno cross-site al suo menu
            If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then
                Dim objAgenda2010 As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                objAgenda2010.Piva = objParametriAgenda.Piva
                objAgenda2010.PaginaRichiesta = enum_PagineAgenda_2010.Menu_BS
                Return AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                        Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua,
                                        objAgenda2010)
            End If
            Return TargetMenuAgenda
        End If
        If Not IsNothing(HttpContext.Current.Session("Sito_Origine")) Then
            Select Case HttpContext.Current.Session("Sito_Origine")
                Case Enum_SiteRedirector.Sito_GiasOnline_2010
                    If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
                        Return TargetMenuAgenda
                    End If

                    Dim objGiasOnline_2010 As New AgronicaCoreGestioneRichieste.ParametriGiasOnline_2010

                    objGiasOnline_2010.Pagina_Richiesta = paginaRichiesta

                    Dim PaginaLink As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri( _
                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                                                   paginaRichiesta, _
                                                   enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                    Response.Redirect(PaginaLink)
                    Return Nothing

                    'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    'Return objWebConfig.LinkGiasOnline_2010
                Case Enum_SiteRedirector.Sito_GiasOnline
                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                    objGiasOnline.Cul_Cod = objParametriAgenda.Cul_Cod
                    objGiasOnline.DataSelezionata = objParametriAgenda.Data
                    objGiasOnline.Id_Agenda = objParametriAgenda.Id_Agenda
                    objGiasOnline.Lavorazione = objParametriAgenda.Lav_Cod
                    objGiasOnline.PaginaRichiesta = paginaRichiesta
                    objGiasOnline.Piva = objParametriAgenda.Piva
                    objGiasOnline.Sa_Cod = objParametriAgenda.Sa_Cod
                    Dim specie As Integer = 0
                    If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
                        specie = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
                    End If
                    objGiasOnline.Veg_Cod = specie

                    Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline( _
                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                                   objGiasOnline)

                    Response.Redirect(str)
                    Return Nothing
                Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                    ' Quando provengo da AgroAgenda, torno cross-site al suo menu (i path locali non esistono su DomandaIrrigua)
                    Dim objAgenda2010 As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                    objAgenda2010.Piva = objParametriAgenda.Piva
                    objAgenda2010.PaginaRichiesta = enum_PagineAgenda_2010.Menu_BS
                    Return AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                            Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua,
                                            objAgenda2010)
                Case Else
                    If paginaRichiesta = enum_PagineGiasOnline.MenuPrincipale Then
                        HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline
                        Return TrovaRedirectCorretto(True, paginaRichiesta, objParametriAgenda)
                    End If
                    Return TargetMenuAgenda
            End Select
        Else
            Return TargetMenuAgenda
        End If


    End Function

End Class