Imports AgroAgenda_2010.Resources
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports System.Configuration.ConfigurationManager
Imports System.Web.Services
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente

Public Class TroubleLogin
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

            hfValSuperServer.Value = Request.QueryString("d_v")
            Dim dominioDesc As String = Request.QueryString("d_d")

            hfDescSuperServer.Value = dominioDesc
            descDominioScelto.InnerText = Stringa_Decodifica(dominioDesc, AgroKey_EncoderDecoder)
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

    <WebMethod(EnableSession:=True)>
    Public Shared Function wsReimpostaPasswordDaUtente(ByVal username As String, ByVal valSuperServer As String, ByVal descSuperServer As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim decoValSuperServer = Stringa_Decodifica(valSuperServer, AgroKey_EncoderDecoder)
        Dim idSuperServer = CInt(decoValSuperServer.Split("|")(0))

        Dim objParametri_Server As New AgronicaCoreParametri
        Dim objParametri_Utenti As New AgronicaCoreParametri
        RecuperaCredenziali.ValorizzaObjParametri(idSuperServer, objParametri_Server, objParametri_Utenti)

        If String.IsNullOrWhiteSpace(username) Then
            r.Errore = AgronicaAgenda_2010.Username & " " & AgronicaAgenda_2010.NonImpostata
            r.RispostaOK = False
            Return r
        End If

        Dim handleUtentiDett As New Utenti_Dettagli_R
        Dim dtUtentiDett As DataTable = handleUtentiDett.Utenti_Dettagli_from_USERNAME(username, objParametri_Utenti)

        Select Case dtUtentiDett.Rows.Count
            Case 0
                r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "UtenteNonTrovatoPerUsername"), String)

            Case 1
                Dim emailDaUsername As String = dtUtentiDett(0)("Email")
                If Not emailDaUsername = "" Then
                    Dim dtUtenteDaEmail = handleUtentiDett.Leggi(
                        "",
                        0,
                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                        " email = '" & Agro_SQL_SaveText(emailDaUsername) & "'",
                        "",
                        objParametri_Utenti)
                    If dtUtenteDaEmail.Rows.Count = 1 Then
                        Dim objParametri_Super_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))
                        Dim erroreMail = wsReimpostaPasswordInviaMail(objParametri_Server, objParametri_Super_Server, objParametri_Utenti, valSuperServer, descSuperServer, dtUtentiDett)
                        If erroreMail = "" Then
                            'Procedura avvenuta con successo
                            r.RispostaStringa = String.Format(
                                DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "InviataMailAdIndirizzoPostaX"), String),
                                emailDaUsername)
                        Else
                            'Errore nell'invio
                            r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "ImpossibileContinuareProceduraRecuperoPassword"), String)
                            'Per debug:
                            'r.Errore = erroreMail
                            Dim objLog As New LogProvider
                            objLog.Scrivi_LOG(objParametri_Server, "TroubleLogin.aspx.vb.wsReimpostaPasswordDaUtente()", erroreMail)
                        End If
                    Else
                        'Errore, Più utenti con la stessa mail
                        r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "ImpossibileContinuareProceduraRecuperoPassword"), String)
                    End If
                Else
                    'Utente senza email
                    r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "NessunIndirizzoCollegatoAlloUsernameInserito"), String)
                End If

        End Select

        If r.Errore.Length = 0 Then
            r.RispostaOK = True
        Else
            r.RispostaOK = False
        End If

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function wsReimpostaPasswordDaEmail(ByVal email As String, ByVal valSuperServer As String, ByVal descSuperServer As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim decoValSuperServer = Stringa_Decodifica(valSuperServer, AgroKey_EncoderDecoder)
        Dim idSuperServer = CInt(decoValSuperServer.Split("|")(0))

        Dim objParametri_Server As New AgronicaCoreParametri
        Dim objParametri_Utenti As New AgronicaCoreParametri
        RecuperaCredenziali.ValorizzaObjParametri(idSuperServer, objParametri_Server, objParametri_Utenti)

        If String.IsNullOrWhiteSpace(email) Then
            r.Errore = AgronicaAgenda_2010.Email & " " & AgronicaAgenda_2010.NonImpostata
            r.RispostaOK = False
            Return r
        End If

        Dim handleUtentiDett As New Utenti_Dettagli_R
        Dim dtUtenteDaEmail = handleUtentiDett.Leggi(
            "",
            0,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            " email = '" & Agro_SQL_SaveText(email) & "'",
            "",
            objParametri_Utenti)
        Select Case dtUtenteDaEmail.Rows.Count
            Case 0
                r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "NessunUtenteCollegatoAllaMailIndicata"), String)

            Case 1

                Dim dtUtentiDett As DataTable = handleUtentiDett.Utenti_Dettagli_from_USERNAME(dtUtenteDaEmail(0)("UserName"), objParametri_Utenti)

                Dim objParametri_Super_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))
                Dim erroreMail = wsReimpostaPasswordInviaMail(objParametri_Server, objParametri_Super_Server, objParametri_Utenti, valSuperServer, descSuperServer, dtUtentiDett)
                If erroreMail = "" Then
                    'Procedura avvenuta con successo
                    r.RispostaStringa = String.Format(
                                DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "InviataMailAdIndirizzoPostaX"), String),
                                email)
                Else
                    'Errore nell'invio
                    r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "ImpossibileContinuareProceduraRecuperoPassword"), String)
                    'Per debug:
                    'r.Errore = erroreMail
                    Dim objLog As New LogProvider
                    objLog.Scrivi_LOG(objParametri_Server, "TroubleLogin.aspx.vb.wsReimpostaPasswordDaEmail()", erroreMail)
                End If

            Case > 1
                'Errore, Più utenti con la stessa mail
                r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "ImpossibileContinuareProceduraRecuperoPassword"), String)
        End Select


        If r.Errore.Length = 0 Then
            r.RispostaOK = True
        Else
            r.RispostaOK = False
        End If

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function wsRecuperaUsername(ByVal email As String, ByVal valSuperServer As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim decoValSuperServer = Stringa_Decodifica(valSuperServer, AgroKey_EncoderDecoder)
        Dim idSuperServer = CInt(decoValSuperServer.Split("|")(0))

        Dim objParametri_Server As New AgronicaCoreParametri
        Dim objParametri_Utenti As New AgronicaCoreParametri
        RecuperaCredenziali.ValorizzaObjParametri(idSuperServer, objParametri_Server, objParametri_Utenti)

        If String.IsNullOrWhiteSpace(email) Then
            r.Errore = AgronicaAgenda_2010.Email & " " & AgronicaAgenda_2010.NonImpostata
            r.RispostaOK = False
            Return r
        End If

        Dim objParametri_Super_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))

        Dim handleUtentiDett As New Utenti_Dettagli_R
        Dim dtUtenteDaEmail = handleUtentiDett.Leggi(
            "",
            0,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            " email = '" & Agro_SQL_SaveText(email) & "'",
            "",
            objParametri_Utenti)

        Select Case dtUtenteDaEmail.Rows.Count
            Case 0
                r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "NessunUtenteCollegatoAllaMailIndicata"), String)

            Case 1
                Dim username As String = dtUtenteDaEmail(0)("UserName")

                Dim dtUtentiDett As DataTable = handleUtentiDett.Utenti_Dettagli_from_USERNAME(username, objParametri_Utenti)

                Dim nome As String = dtUtentiDett(0)("Nome")
                Dim cognome As String = dtUtentiDett(0)("Cognome")
                Dim nominativo = AgronicaAgenda_2010.Utente
                If nome.Length > 0 Then
                    nominativo = nome & IIf(cognome.Length > 0, " " & cognome, "")
                End If

                Dim swOggetto, mailLogo As String
                PersonalizzazioniMail(objParametri_Server, objParametri_Super_Server, swOggetto, mailLogo)

                Dim mailOggetto = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "OggettoMailRecuperaUsername"), String) &
                    " " & swOggetto

                Dim mailCorpo = mailLogo & String.Format(
                    DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "CorpoMailRecuperaUsername"), String),
                    nominativo) & "<br/><b>" & username & "</b>"

                Dim xAssistenza As New Configurazione_Siti_R
                Dim mailMittente As String = xAssistenza.Leggi_Valore(0, "MailFrom_smtp", "", "", objParametri_Super_Server)

                Dim erroreMail As String
                Try
                    Dim handleMail As New Mail
                    erroreMail = handleMail.invia(
                        objParametri_Super_Server,
                        mailMittente,
                        email,
                        "",
                        "",
                        mailOggetto,
                        mailCorpo,
                        True,
                        Nothing)
                Catch ex As Exception
                    erroreMail = ex.Message
                End Try

                If erroreMail = "" Then
                    'Procedura avvenuta con successo
                    r.RispostaStringa = String.Format(
                                DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "InviataMailAdIndirizzoPostaX"), String),
                                email)
                Else
                    'Errore nell'invio
                    r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "ImpossibileContinuareProceduraRecuperoUsername"), String)
                    'Per debug:
                    'r.Errore = erroreMail
                    Dim objLog As New LogProvider
                    objLog.Scrivi_LOG(objParametri_Server, "TroubleLogin.aspx.vb.wsRecuperaUsername()", erroreMail)
                End If

            Case > 1
                'Errore, Più utenti con la stessa mail
                r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "ImpossibileContinuareProceduraRecuperoUsername"), String)
        End Select



        If r.Errore.Length = 0 Then
            r.RispostaOK = True
        Else
            r.RispostaOK = False
        End If

        Return r
    End Function

    Protected Shared Function wsReimpostaPasswordInviaMail(ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                           ByVal valSuperServer As String,
                                                           ByVal descSuperServer As String,
                                                           ByVal dtUtentiDett As DataTable) As String
        Dim errore As String

        Dim swOggetto, mailLogo As String

        Try

            PersonalizzazioniMail(objParametri_Server, objParametri_Super_Server, swOggetto, mailLogo)

            Dim mailOggetto = DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "OggettoMailReimpostaPassword"), String) &
                " " & swOggetto

            Dim token As String = Guid.NewGuid().ToString()
            Dim username As String = dtUtentiDett(0)("UserName")
            Dim nome As String = dtUtentiDett(0)("Nome")
            Dim cognome As String = dtUtentiDett(0)("Cognome")
            Dim nominativo = AgronicaAgenda_2010.Utente
            If nome.Length > 0 Then
                nominativo = nome & IIf(cognome.Length > 0, " " & cognome, "")
            End If

            Dim xAssistenza As New Configurazione_Siti_R
            Dim periodoValidita As Integer = 30

            Dim configPeriodoValidita As String = xAssistenza.Leggi_Valore(Enum_SiteRedirector.Sito_GiasOnline, "MinutiValiditaTokenRecuperoPassword", "", "", objParametri_Super_Server)
            If (IsNumeric(configPeriodoValidita)) Then
                Integer.TryParse(configPeriodoValidita, periodoValidita)
            End If

            Dim handleUtentiDett As New Utenti_Dettagli_W
            handleUtentiDett.ScriviTokenResetCredenziali(token, username, periodoValidita, objParametri_Utenti)

            Dim dicQueryString As New Dictionary(Of String, String)
            dicQueryString.Add("d_v", valSuperServer)
            dicQueryString.Add("d_d", descSuperServer)
            dicQueryString.Add("u", Stringa_Codifica(
                               AgroZip.CompressioneBase64(0, username),
                               AgroKey_EncoderDecoder))
            dicQueryString.Add("t", Stringa_Codifica(token, AgroKey_EncoderDecoder))

            Dim linkBuilder As New UriBuilder
            linkBuilder.Scheme = HttpContext.Current.Request.Url.Scheme
            linkBuilder.Host = HttpContext.Current.Request.Url.Host
            linkBuilder.Port = HttpContext.Current.Request.Url.Port
            linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/RecuperaCredenziali/ReimpostaPassword.aspx") 'In sostituzione di ResolveUrl
            linkBuilder.Query = RecuperaCredenziali.ComponiQueryStringDaDictionary(dicQueryString)

            Dim mailLinkRipristinoPwd As String = linkBuilder.ToString()
            Dim mailCorpo = mailLogo & String.Format(
                DirectCast(HttpContext.GetLocalResourceObject("~/RecuperaCredenziali/TroubleLogin.aspx", "CorpoMailReimpostaPassword"), String),
                nominativo,
                username,
                mailLinkRipristinoPwd)

            Dim mailMittente As String = xAssistenza.Leggi_Valore(0, "MailFrom_smtp", "", "", objParametri_Super_Server)
            Dim mailUtente As String = dtUtentiDett(0)("Email")


            Dim handleMail As New Mail
            errore = handleMail.invia(
                objParametri_Super_Server,
                mailMittente,
                mailUtente,
                "",
                "",
                mailOggetto,
                mailCorpo,
                True,
                Nothing)
        Catch ex As Exception
            errore = ex.Message
        End Try

        Return errore
    End Function

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

    Private Shared Sub PersonalizzazioniMail(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri, ByRef mailSWOggetto As String, ByRef mailLogo As String)

        'L'AgroWebConfig è salvato in sessione e contiene i valori di configurazione_siti del super_server e del server con priorità a quest'ultimo,
        'se non è possibile usarlo occorre aggiungere una lettura come la seguente:

        Dim CustomLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        mailSWOggetto = "Gias Agronica"
        Dim nomeLogo = "agronica/AB_Immagini/Logo/Logo_Gias_Login.png"

        If CustomLoghi IsNot Nothing Then
            If Not String.IsNullOrWhiteSpace(CustomLoghi.Title) Then
                mailSWOggetto = CustomLoghi.Title
            End If
            If Not String.IsNullOrWhiteSpace(CustomLoghi.Logo_Login_PATH) Then
                nomeLogo = CustomLoghi.Logo_Login_PATH
            End If
        End If

        Dim linkGiasBase = objParametri_Super_Server.LinkGiasBase

        If Not linkGiasBase.ToLowerInvariant.Contains("http") Then
            Dim linkBuilder As New UriBuilder
            linkBuilder.Scheme = HttpContext.Current.Request.Url.Scheme
            linkBuilder.Host = HttpContext.Current.Request.Url.Host
            linkBuilder.Path = linkGiasBase

            linkGiasBase = linkBuilder.ToString()
        End If

        mailLogo = String.Format("<img src='{0}{1}' /><br/><br/>", linkGiasBase, nomeLogo)

    End Sub


End Class