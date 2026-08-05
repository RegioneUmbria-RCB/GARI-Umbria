Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.Matomo
Imports Newtonsoft.Json

Public Class Stampe
    Inherits System.Web.UI.MasterPage

    Private _FrameworkClient As String = "jqueryUI"

    Public Property FrameworkClient As String
        Get
            Return _FrameworkClient
        End Get
        Set(ByVal value As String)
            _FrameworkClient = value
        End Set
    End Property

    Public Property LblTitolo() As Global.System.Web.UI.WebControls.Label
        Get
            Return Lbl_Titolo
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            Lbl_Titolo = value
        End Set
    End Property

    Public Property LblRag_Soc() As Global.System.Web.UI.WebControls.Label
        Get
            Return Lbl_Rag_Soc
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            Lbl_Rag_Soc = value
        End Set
    End Property

    'delegato per l'evento di uscita
    Public Delegate Sub ExitButton_Clicked_delegate(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ExitClicked As ExitButton_Clicked_delegate


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametriProfilazione As AgronicaCoreGestioneRichieste.ParametriProfilazione_2010

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ScriptxLoading()
        'errore autenticazione o sessione scaduta
        If (IsNothing(Session("ASG_objParametri_Server")) And Not Page.ToString().Equals("ASP.error_aspx")) Then
            Response.Redirect("~/Error.aspx?noSes=1")
        Else
            If (Not IsNothing(Session("ASG_objParametri_Server"))) Then

                '---
                objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
                '---

                If (Not IsPostBack) Then
                    objParametriProfilazione = New AgronicaCoreGestioneRichieste.ParametriProfilazione_2010
                    objParametriProfilazione.Leggi()

                    'objParametriStampe = New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                    'objParametriStampe.Leggi()

                    If objParametriProfilazione.Piva <> "" Then
                        Dim imp As New AgronicaCoreAnagrafeDAL.Imprese_Read()
                        'imposto la label coi dati dell'utente...
                        Lbl_Rag_Soc.Text = imp.RagSoc_from_Piva(objParametriProfilazione.Piva, objParametri_Server) + " - P.iva: " + objParametriProfilazione.Piva
                        'Lbl_Rag_Soc.Text = imp.RagSoc_from_Piva(objParametriStampe, objParametri_Server) + " - P.iva: " + objParametriProfilazione.Piva
                    End If

                    'integrazione Matomo
                    Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername = objParametri_Utenti.UtenteUsername)

                    If Not isSuperUser Then

                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                        Dim DTConfigSiti = objConfigSiti.Leggi(0, "ConfigMatomo", "", "", objParametri_Server)
                        Dim flag_Matomo_Enabled = Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0

                        Dim result As ConfigMatomo

                        'se esiste quantomeno la configurazione
                        If flag_Matomo_Enabled Then
                            'verifico che la configurazione registrata abbia i campi corretti
                            result = JsonConvert.DeserializeObject(Of ConfigMatomo)(DTConfigSiti.Rows(0)("valore").ToString)
                            If result IsNot Nothing AndAlso Not String.IsNullOrEmpty(result.Domain) AndAlso Not String.IsNullOrEmpty(result.Container) Then
                                Dim script As String = "<script>
                                            var _mtm = window._mtm = window._mtm || [];
                                            var _paq = window._paq = window._paq || [];
                                            _paq.push(['setUserId', '" & objParametri_Utenti.UtenteUsername & "']);
                                            _mtm.push({'mtm.startTime': (new Date().getTime()), 'event': 'mtm.Start'});
                                            var d = document, g = d.createElement('script'), s = d.getElementsByTagName('script')[0];
                                            g.async = true; g.src = 'https://" & result.Domain & "/js/container_" & result.Container & ".js';
                                            s.parentNode.insertBefore(g, s);
                                        </script>"

                                Page.ClientScript.RegisterStartupScript(Me.GetType(), "MatomoScript", script, False)
                            End If
                        End If

                    End If
                End If
            End If
        End If

    End Sub

    Private Sub ScriptxLoading()

        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")


        Str.AppendLine("    $('input:submit').button(); //bottoni formattati col tema ui ")
        Str.AppendLine("    $('.myCombo').combobox(); //inizializzo il combo UI ")


        Str.AppendLine("    $('#WaitFrame').hide(); ")
        Str.AppendLine("    $('.btn_per_load').click( ")
        Str.AppendLine("        function() { ")
        Str.AppendLine("            $('#WaitFrame').show();")
        Str.AppendLine("        }); ")
        Str.AppendLine("    }); ")

        'ScriptManager.RegisterStartupScript(UpdateLBLCLiente, UpdateLBLCLiente.GetType(),
        '                                String.Format("jQuery_{0}", UpdateLBLCLiente.ClientID), Str.ToString, True)

        ScriptManager.RegisterStartupScript(Lbl_Rag_Soc, Lbl_Rag_Soc.GetType(),
                                 String.Format("jQuery_{0}", Lbl_Rag_Soc.ClientID), Str.ToString, True)

    End Sub


    Public Sub AggiornaImpresa()
        objParametriProfilazione = New AgronicaCoreGestioneRichieste.ParametriProfilazione_2010
        objParametriProfilazione.Leggi()

        If objParametriProfilazione.Piva <> "" Then
            Dim imp As New AgronicaCoreAnagrafeDAL.Imprese_Read()
            'imposto la label coi dati dell'utente...
            Lbl_Rag_Soc.Text = imp.RagSoc_from_Piva(objParametriProfilazione.Piva, objParametri_Server) + " - P.iva: " + objParametriProfilazione.Piva
        End If
    End Sub
    Public Function GetVersion() As String
        If (Not IsNothing(ConfigurationManager.AppSettings("ver"))) Then
            Return ConfigurationManager.AppSettings("ver")
        Else
            Return ""
        End If

    End Function

    ''solleva l'evento e lo passa al delegato
    'Protected Sub btnExit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnExit.Click
    '    RaiseEvent ExitClicked(sender, e) 'faccio sorgere l'evento
    'End Sub

End Class