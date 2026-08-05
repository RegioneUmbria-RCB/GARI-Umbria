Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports System.Management
Imports System.Diagnostics
Imports System.Drawing.Printing
Imports System.Drawing
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class EstrazioneCatastoAffitti
    Inherits System.Web.UI.Page

    Dim Qs_Piva As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Try
            CType(MyBase.Master, StampeBootstrap).SetTitoloPagina(286)

            '##############################################################
            '###################### QUERYSTRING ###########################
            '##############################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                   AgroKey_EncoderDecoder,
                                   Server)

            'Valorizza il campo input hidden con la partita iva, utilizzabile da js
            hdPiva.Value = Qs_Piva

            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            'Verifica tramite query i permessi utente
            Dim UtenteAbilitatoEstrazioneCA As Boolean = objPermessi.Controlla_Permessi_Utente(
                                        HttpContext.Current.Session("ASG_Utente_Username"),
                                        HttpContext.Current.Session("ASG_IdServizio"),
                                        enum_Security_Attivita.EstrazioneCatastoAffitti,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        HttpContext.Current.Session("ASG_objParametri_Utenti"))
            hdUtenteAbilitato.Value = IIf(UtenteAbilitatoEstrazioneCA, 1, 0)

            'Se l'utente non è abilitato esegue un redirect
            If hdUtenteAbilitato.Value = False Then
                'TODO Verificare il redirect per le stampe
                Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
                Exit Sub
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Problemi durante il caricamento della pagina: " + vbCrLf + ex.Message, Page, "MainContent", True)
        End Try

    End Sub

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function caricaCatastoAffitti(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String, ByVal centriAziendali As String) As RispostaStandard

        Dim r As New RispostaStandard()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim handleCatastoAffitti As New AgronicaCoreContabBIZ.ContrattiXImpreseXParticelle(objParametri_Server)

        Try

            Dim dtCatastoAffitti = handleCatastoAffitti.Estrazione_CatastoAffitti(piva, dataDal, dataAl, centriAziendali)
            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

            'Serializza in JSON l'obj DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(dtCatastoAffitti, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


End Class