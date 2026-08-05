Imports AgroAgenda_2010.Resources
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiBIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports System.Web.Services
Imports System.Configuration.ConfigurationManager
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreGestioneRichieste
Imports System.Security.Cryptography
Imports AgronicaCoreVarieDAL
Imports DocumentFormat.OpenXml.Wordprocessing

Public Class ReimpostaPassword
    Inherits System.Web.UI.Page

    Public debug_isattached As Boolean = False
    Private _loginVersione As String = VERSIONE_LOGIN_DEFAULT
    Private _loginVersioniDisponibili As String() = {VERSIONE_LOGIN_DEFAULT, "2022"}

    Public Property Login_Versione As String
        Get
            Return _loginVersione
        End Get
        Set(value As String)
            SetLoginVersione(value)
        End Set
    End Property

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return AgroMasterPage.PATH_GIASBASE
        End Get
    End Property

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        RecuperaCredenziali.InizializzaCulturaPreLogin()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Debugger.IsAttached Then
            debug_isattached = True
        End If

        If Not IsPostBack Then
            If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                RecuperaCredenziali.InizializzaSitoGiasOnline()
            End If

            Dim valSuperServer = Request.QueryString("d_v")
            Dim decoValSuperServer = Stringa_Decodifica(valSuperServer, AgroKey_EncoderDecoder)
            Dim idSuperServer = CInt(decoValSuperServer.Split("|")(0))

            Dim objParametri_Server As New AgronicaCoreParametri
            Dim objParametri_Utenti As New AgronicaCoreParametri
            RecuperaCredenziali.ValorizzaObjParametri(idSuperServer, objParametri_Server, objParametri_Utenti)

            hfValSuperServer.Value = valSuperServer
            descDominioScelto.InnerText = Stringa_Decodifica(Request.QueryString("d_d"), AgroKey_EncoderDecoder)

            mexInfo.InnerText = Utenti.MessaggioRequisitiPassword(objParametri_Server:=objParametri_Server)

            Dim username As String = Request.QueryString("u")
            Dim decoUsername = AgroZip.DeCompressioneBase64(0, Stringa_Decodifica(username, AgroKey_EncoderDecoder))

            hfUsername.Value = username
            txtUsername.Value = decoUsername

            Dim token As String = Request.QueryString("t")
            Dim decodeToken As String = Stringa_Decodifica(token, AgroKey_EncoderDecoder)

            hfToken.Value = token

            hfIsValidSession.Value = CheckIsValidToken(decodeToken, decoUsername, objParametri_Utenti)

        End If

        AgroMasterPage.CssPlaceHolder = siteCssPlaceHolder
        AgroMasterPage.FooterPlaceHolder = footerPlaceHolder
        AgroMasterPage.HeaderPlaceHolder = headerPlaceHolder
        AgroMasterPage.PaginaOspite = 0
        bootstrap.Versione = "5.3.3"
        bootstrap.BootstrapPlaceHeader = bootstrapPlaceHeader

        Dim objParametri_Super_Server As New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        ImpostaVersioneLogin(objParametri_Super_Server)

        Dim xAssistenza As New Configurazione_Siti_R
        AgroMasterPage.sAssistenzaInfo = xAssistenza.Assistenza(objParametri_Super_Server, Nothing)

    End Sub

    Public Shared Function CheckIsValidToken(token As String, username As String, objParametri_Utenti As AgronicaCoreParametri) As String
        Dim result As String = String.Empty

        Dim handleUtentiDett As New Utenti_Dettagli_R
        Dim sessionResetPassword As DataTable = handleUtentiDett.LeggiTokenResetCredenziali(token, username, objParametri_Utenti)

        Dim isValidSession As Boolean = Not IsNothing(sessionResetPassword)
        isValidSession = isValidSession AndAlso (sessionResetPassword.Rows.Count = 1)
        isValidSession = isValidSession AndAlso (sessionResetPassword.Rows(0)("Timestamp_Scadenza_Token") > DateTime.Now)

        If Not isValidSession Then
            result = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "TokenScadutoFareNuovaRichiesta"), String)
        End If

        Return result
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function wsReimpostaPassword(ByVal token As String, ByVal username As String, ByVal pwdNuova As String, ByVal pwdConferma As String, ByVal valSuperServer As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim decoValSuperServer = Stringa_Decodifica(valSuperServer, AgroKey_EncoderDecoder)
        Dim idSuperServer = CInt(decoValSuperServer.Split("|")(0))
        Dim decodeToken As String = Stringa_Decodifica(token, AgroKey_EncoderDecoder)
        Dim decoUsername = AgroZip.DeCompressioneBase64(0, Stringa_Decodifica(username, AgroKey_EncoderDecoder))

        Dim objParametri_Server As New AgronicaCoreParametri
        Dim objParametri_Utenti As New AgronicaCoreParametri
        RecuperaCredenziali.ValorizzaObjParametri(idSuperServer, objParametri_Server, objParametri_Utenti)

        r.Errore = CheckIsValidToken(decodeToken, decoUsername, objParametri_Utenti)
        If r.Errore = String.Empty Then

            If pwdNuova = pwdConferma Then
                Try

                    Dim handleConfigSiti As New Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", objParametri_Server)
                    Dim gestioneHashAbilitata As Boolean = False
                    If dtConfigSiti.Rows.Count > 0 Then
                        gestioneHashAbilitata = dtConfigSiti.Rows(0)("Valore")
                    End If

                    Dim handleAutentica As New AutenticaUtente
                    Dim passwordUguali = handleAutentica.VerificaCorrettezzaPassword(decoUsername, pwdNuova, gestioneHashAbilitata, objParametri_Utenti)

                    If passwordUguali = False Then
                        Dim isPasswordComplessa = Utenti.ValidaComplessitaPassword(pwdNuova, gestioneHashAbilitata:=gestioneHashAbilitata)

                        If isPasswordComplessa = True Then
                            Dim handleUtentiWrite As New Utenti_Write
                            handleUtentiWrite.Modifica(decoUsername, pwdNuova, gestioneHashAbilitata, False, objParametri_Utenti)
                        Else
                            r.Errore = AgronicaCoreDataProvider.My.Resources.Gias.RequisitiPasswordNonSoddisfatti
                        End If

                    Else
                        r.Errore = AgronicaAgenda_2010.NuovaPasswordDiversaDaPrecedente
                    End If

                Catch ex As Exception
                    r.Errore = ex.Message
                End Try
            Else
                r.Errore = AgronicaAgenda_2010.PasswordDiConfermaNonCorrisponde
            End If
        Else

            r.Tipo = "Token Scaduto"

        End If

        If r.Errore.Length = 0 Then
            r.RispostaStringa = AgronicaAgenda_2010.CambioPasswordEffettuatoFareLoginConNuovaPassword
            r.RispostaOK = True
        Else
            r.RispostaOK = False
        End If

        Return r
    End Function

    Private Sub btnTornaAllaLogin_Click(sender As Object, e As EventArgs) Handles btnTornaAllaLogin.Click
        Dim valSuperServer As String = hfValSuperServer.Value
        Dim rispLinkLogin = RecuperaCredenziali.LinkPaginaLogin(valSuperServer)
        Response.Redirect(rispLinkLogin.RispostaStringa)
    End Sub

    Private Sub ImpostaVersioneLogin(objParametri_Super_Server As AgronicaCoreParametri)
        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        _loginVersione = confSiti.Leggi_Valore(0, "VersioneLogin", "", "", objParametri_Super_Server)
        If String.IsNullOrEmpty(_loginVersione) OrElse String.IsNullOrWhiteSpace(_loginVersione) Then
            _loginVersione = VERSIONE_LOGIN_DEFAULT
        End If
    End Sub

    Private Sub SetLoginVersione(versione As String)
        If Not _loginVersioniDisponibili.Contains(versione) Then
            Throw New Exception("Versione Login " & versione & " sconosciuta, valori ammessi: " & String.Join(",", _loginVersioniDisponibili))
        End If
        _loginVersione = versione
    End Sub

End Class