' NOTA: è possibile utilizzare il comando "Rinomina" del menu di scelta rapida per modificare il nome di classe "ProfilatoreUtenze" nel codice, nel file svc e nel file di configurazione contemporaneamente.
' NOTA: per avviare il client di prova WCF per testare il servizio, selezionare ProfilatoreUtenze.svc o ProfilatoreUtenze.svc.vb in Esplora soluzioni e avviare il debug.
Imports System.ServiceModel.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreProfilazioneBIZ
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreWebService
Imports Newtonsoft.Json

Public Class ProfilatoreUtenze
    Implements IProfilatoreUtenze

    ''' <summary>
    ''' verifica il token chiamante su database
    ''' </summary>
    ''' <param name="ObjParametri"></param>
    ''' <returns></returns>
    Private Function verificaTokenChiamante(ByVal applicazione_richiedente As String, ByVal ObjParametri As AgronicaCoreParametri) As GenericResponse

        Dim res As New GenericResponse

        Dim httpHelper As New AgronicaCoreUtility.Http
        Dim request As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest

        Dim tokenChiamante As String = httpHelper.GetAuthFromCurrentHeader(request.Headers)

        Dim Err_tokenChiamate As String = ""

        If tokenChiamante.ToLower.StartsWith("bearer") Then
            tokenChiamante = tokenChiamante.Split(" ")(1)
        Else
            Err_tokenChiamate = "Token non indicato nell'header"
        End If


        'verifica token, al momento da database
        Dim objTokenVerifica As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Dim dtTokenVerifica As DataTable =
            objTokenVerifica.Leggi_Token(tokenChiamante, " applicazione_Richiedente = " & applicazione_richiedente, ObjParametri)

        If dtTokenVerifica.Rows.Count = 0 Then
            Err_tokenChiamate = "Token inesistente per l'utente"
        Else

            Dim scadenzaToken As Date = dtTokenVerifica(0)("validita_fine")
            If scadenzaToken <= Now() Then
                Err_tokenChiamate = "Token scaduto"
            End If
        End If



        'fine verifica token chiamante, se ok procedo, altrimenti errore!
        If Err_tokenChiamate <> "" Then

            res.statusCode = 500
            res.message = "Non Autorizzato."
            res.token = ""
            Return res
        End If


        res.statusCode = 200
        res.message = "Ok"
        res.token = tokenChiamante

        Return res


    End Function

    ''' <summary>
    ''' verifica il token chiamante su database
    ''' </summary>
    ''' <param name="ObjParametri"></param>
    ''' <returns></returns>
    Private Function verificaTokenChiamante(ByVal ObjParametri As AgronicaCoreParametri) As GenericResponse

        Dim res As New GenericResponse

        Dim tokenChiamante As String = Nothing
        Dim Err_tokenChiamate As String = Nothing
        tokenOttieniDaHeaderHttp(tokenChiamante, Err_tokenChiamate)


        'verifica token, al momento da database
        Dim objTokenVerifica As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Dim dtTokenVerifica As DataTable =
            objTokenVerifica.LeggiSuperUser(" applicazione_Richiedente = 102 ", "", ObjParametri)

        If dtTokenVerifica.Rows.Count = 0 Then
            Err_tokenChiamate = "Token inesistente per l'utente"
        Else
            Dim tokenDb As String = dtTokenVerifica(0)("token_id")
            Dim scadenzaToken As Date = dtTokenVerifica(0)("validita_fine")
            If tokenChiamante <> tokenDb Then
                Err_tokenChiamate = "Token non valido"
            End If

            If scadenzaToken <= Now() Then
                Err_tokenChiamate = "Token scaduto"
            End If
        End If



        'fine verifica token chiamante, se ok procedo, altrimenti errore!
        If Err_tokenChiamate <> "" Then

            res.statusCode = 500
            res.message = "Non Autorizzato."
            res.token = ""
            Return res
        End If


        res.statusCode = 200
        res.message = "Ok"
        res.token = ""

        Return res


    End Function

    Private Shared Sub tokenOttieniDaHeaderHttp(ByRef tokenChiamante As String, ByRef Err_tokenChiamate As String)
        Dim httpHelper As New AgronicaCoreUtility.Http
        Dim request As IncomingWebRequestContext = WebOperationContext.Current.IncomingRequest

        tokenChiamante = httpHelper.GetAuthFromCurrentHeader(request.Headers)

        Err_tokenChiamate = ""

        If tokenChiamante.ToLower.StartsWith("bearer") Then
            tokenChiamante = tokenChiamante.Split(" ")(1)
        Else
            Err_tokenChiamate = "Token non indicato nell'header"
        End If
    End Sub

    Private Sub accountManagerVerificaDatiObbligatoriNoDatatable(o1 As GenericRequest, res As AccountManagerResponse, AP_TipoAccountManagerRequest As enum_AP_TipoAccountManagerRequest)

        Dim o
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
            o = CType(o1, AccountManagerEstesaRequest)
        Else
            o = CType(o1, AccountManagerRequest)
        End If


        'username riservato per token di autenticazione
        If o.username = SuperUserUsername Then
            res.statusCode = 500
            res.message = "Non può esiste un utente con username portalesoci. Scegliere uno username differente."
            Exit Sub
        End If

        'username riservato per token di autenticazione
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then

            If o.cognome = "" Or
                o.nome = "" Or
                o.email = "" Or
                o.via = "" Or
                o.cap = "" Or
                o.citta = "" Or
                o.numeroCivico = "" Or
                o.provincia = "" Or
                (o.codiceFiscale = "" And o.piva = "") Or
                (o.piva <> "" And o.ragsoc = "") Then

                res.statusCode = 500
                res.message = "Indicare tutti i dati obbligatori."
                Exit Sub

            End If

        Else
            If o.username = "" Or o.password = "" Or o.cognome = "" Or o.nome = "" Or o.email = "" Or o.codiceFiscale = "" Then
                res.statusCode = 500
                res.message = "Indicare tutti i dati obbligatori."
                Exit Sub
            End If
        End If


        'verifica validita codice fiscale..
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa AndAlso Not String.IsNullOrEmpty(o.codiceFiscale) AndAlso o.codiceFiscale.length = 16 Then

            Dim xCF As New AgronicaCoreUtility.ItaliaAnagrafeFisco
            Dim cfValido As Boolean =
                xCF.CodiceFiscaleValido_CarattereControllo(o.codiceFiscale)

            If Not cfValido Then
                res.statusCode = 500
                res.message = "codice fiscale non valido."
                Exit Sub
            End If

        End If

        'verifica validità della p.iva
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa AndAlso Not String.IsNullOrEmpty(o.piva) AndAlso Not o.piva.ToString.StartsWith("F") Then


            Dim pivaValida As Boolean =
                AgronicaCoreUtility.ItaliaAnagrafeFisco.IsPivaValida(o.piva)

            If Not pivaValida Then
                res.statusCode = 500
                res.message = "partita iva specificata non valida."
                Exit Sub
            End If
        End If

        'verifica validità della email
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
            Dim emailVAlida As Boolean =
                            AgronicaCoreDataProvider.UtilityProvider.VerificaEspressioneRegolare(o.email, "", enum_EspressioniRegolari.RegExp_Email)

            If Not emailVAlida Then
                res.statusCode = 500
                res.message = "email specificata non valida."
                Exit Sub
            End If


        End If

    End Sub

    Private Sub accountManagerVerificaDatiObbligatori(dtUtente As DataTable, dtUtenteVerificaCodiceFiscale As DataTable, o As AccountManagerRequest, res As AccountManagerResponse, AP_TipoAccountManagerRequest As enum_AP_TipoAccountManagerRequest)

        accountManagerVerificaDatiObbligatoriNoDatatable(o, res, AP_TipoAccountManagerRequest)

        If res.statusCode = 500 Then
            Exit Sub
        End If


        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.Interna AndAlso dtUtente.Rows.Count > 0 Then
            res.statusCode = 500
            res.message = "Esiste già in archivio un utente con lo stesso username. Non è quindi possibile creare questo utente. Per Effettuare modifiche dei dati usare il metodo ""AccountEdit"". "
            Exit Sub
        End If

        Dim codiceFiscalePrecedente As String

        If dtUtente.Rows.Count > 0 Then

            codiceFiscalePrecedente = dtUtente(0)("CodFisc")

            If codiceFiscalePrecedente <> o.codiceFiscale Then
                res.statusCode = 500
                res.message = "Il codice fiscale non può essere modificato."
                Exit Sub
            End If

        End If

        If dtUtenteVerificaCodiceFiscale.Rows.Count > 0 Then

            Dim UtenteLettoPerCodiceFiscale As String =
                dtUtenteVerificaCodiceFiscale(0)("Username")

            If UtenteLettoPerCodiceFiscale <> o.username Then
                res.statusCode = 500
                res.message = "Esiste già in archivio un utente con lo stesso codice fiscale. Non è quindi possibile creare questo utente."
                Exit Sub
            End If

        End If
    End Sub


    Public Sub accountManagerEstesaGeneraUserPass(o As AccountManagerEstesaRequest)

        o.username = o.email
        o.password = accountManagerEstesaGeneraPass()

    End Sub

    Public Function accountManagerEstesaGeneraPass() As String

        Dim vRnd As String() = {"albero", "casa", "formica"}

        Dim rnd1 As Integer =
            randomValue(0, vRnd.Length - 1)

        Dim rnd2 As Integer =
            randomValue(10, 99)

        Return vRnd(rnd1) & (rnd2.ToString())

    End Function

    Public Function randomValue(lowerbound As Integer, upperbound As Integer) As Integer
        Return CInt(Math.Floor((upperbound - lowerbound + 1) * Rnd())) + lowerbound
    End Function


    Public Function provisioningDSS(o As ProvisioningDSSRequest) As ProvisioningDSSResponse Implements IProfilatoreUtenze.provisioningDSS



        Dim nomeFunzione = "provisioningDSS"
        Dim res As New ProvisioningDSSResponse

        Dim FlagTransazioneLocaleServer As Boolean = False
        Dim FlagConnessioneLocaleServer As Boolean = False

        Dim FlagTransazioneLocaleUtenti As Boolean = False
        Dim FlagConnessioneLocaleUtenti As Boolean = False

        Dim FlagTransazioneLocaleSuperServer As Boolean = False
        Dim FlagConnessioneLocaleSuperServer As Boolean = False

        'verifico se sono autorizzato o meno alla chiamata del metodo.
        Dim serviziAutorizzatiWebConfigVerifica As String = verificaSeChiamataAutorizzata(enum_AP_TipoAccountManagerRequest.RichiestaEstesa)

        If serviziAutorizzatiWebConfigVerifica = "" Then
            res.statusCode = 500
            res.message = "Chiamata alla api non autorizzata"
            Return res
        End If

        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri(o.username, "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        Try


            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Return res1
            End If

            Dim gestioneProvisioning As New AgronicaCoreProfilazioneBIZ.ProvisioningDSSbiz
            res = gestioneProvisioning.GestioneProvisioning(o, ObjParametri_Server)


            'risposta
            res.statusCode = 200
            res.message = "OK"
            res.token = ""


        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message

            'Faccio il rollback della transazione
            If Not ObjParametri_Utenti.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Utenti)
            End If

            'Faccio il rollback della transazione
            If Not ObjParametri_Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If

            'Faccio il rollback della transazione
            If Not ObjParametri_SuperServer.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_SuperServer)
            End If

        Finally


            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocaleUtenti, ObjParametri_Utenti)
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocaleServer, ObjParametri_Server)
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocaleSuperServer, ObjParametri_SuperServer)

        End Try

        Return res
    End Function
    Public Function accountManagerEstesa(o As AccountManagerEstesaRequest) As AccountManagerResponse Implements IProfilatoreUtenze.accountManagerEstesa



        Dim nomeFunzione = "AccountManagerEstesa"
        Dim res As New AccountManagerResponse

        Dim FlagTransazioneLocaleServer As Boolean = False
        Dim FlagConnessioneLocaleServer As Boolean = False

        Dim FlagTransazioneLocaleUtenti As Boolean = False
        Dim FlagConnessioneLocaleUtenti As Boolean = False

        Dim FlagTransazioneLocaleSuperServer As Boolean = False
        Dim FlagConnessioneLocaleSuperServer As Boolean = False

        'verifico se sono autorizzato o meno alla chiamata del metodo.
        Dim serviziAutorizzatiWebConfigVerifica As String = verificaSeChiamataAutorizzata(enum_AP_TipoAccountManagerRequest.RichiestaEstesa)

        If serviziAutorizzatiWebConfigVerifica = "" Then
            res.statusCode = 500
            res.message = "Chiamata alla api non autorizzata"
            Return res
        End If

        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri(o.username, o.codiceFiscale, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        Try


            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Return res1
            End If

            accountManagerVerificaDatiObbligatoriNoDatatable(o, res, enum_AP_TipoAccountManagerRequest.RichiestaEstesa)
            If res.statusCode = 500 Then
                Return res
            End If


            'verifiche passate, apro la transazione.

            'Apro la connessione, transazione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleSuperServer,
                FlagTransazioneLocaleSuperServer,
                ObjParametri_SuperServer
            )

            'Apro la connessione, transazione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleServer,
                FlagTransazioneLocaleServer,
                ObjParametri_Server
            )

            'Apro la connessione, transazione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleUtenti,
                FlagTransazioneLocaleUtenti,
                ObjParametri_Utenti
            )


            Dim contattoPersonaFisicaGiuridicaRichiestaAPI As enum_Contatti_IdCf =
                enum_Contatti_IdCf.PersonaGiuridica

            If o.piva = "" Then
                contattoPersonaFisicaGiuridicaRichiestaAPI = enum_Contatti_IdCf.PersonaFisica
            End If


            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dtUtentiVerificaEsistenza As DataTable =
                objUtenti.Leggi2(o.email, "", "", ObjParametri_Utenti)


            'l'utente esiste già, quindi leggo i suoi dati.
            If dtUtentiVerificaEsistenza.Rows.Count > 0 Then

                Dim UtenteUsernameLetto As String = o.email
                Dim UtenteCodiceFiscaleLetto As String = dtUtentiVerificaEsistenza.Rows(0)("CodFisc")

                o.codiceFiscale = UtenteCodiceFiscaleLetto

                ProfilatoreUtenzeHelper.AccountManagerEstesaImpostaValoriObjParametri(
                    UtenteUsernameLetto, UtenteCodiceFiscaleLetto, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer
                )

                AccountManagerEstesaGestioneUtenteEsistente(o, ObjParametri_Utenti, dtUtentiVerificaEsistenza)

            End If

            If String.IsNullOrEmpty(o.username) Then
                'Genero Username e Password
                accountManagerEstesaGeneraUserPass(o)
            End If


            Dim FlagAziendaPersona As Integer = 1
            If contattoPersonaFisicaGiuridicaRichiestaAPI = enum_Contatti_IdCf.PersonaFisica Then
                FlagAziendaPersona = 2
            End If


            Dim pivaPerFatturazione As String = o.piva
            Dim ragioneSocialePerFatturazione As String = o.ragsoc
            Dim pivaAziendaFittizia As String = ""


            'vengono attivate due elementi in GIAS ..:
            '1. contatto ti tipo persona fisica o giuridica con la p.iva (o codice fiscale) specificati (utile per fatturazione)
            '2. impresa, con p.iva fittizia F1999999 nel caso di utenti privati (piva non inviata), questa associata a pratiche e workflow.


            'se esiste già una piva (sempre fittizia) associata al codice fiscale questa va letta da database (è la piva fittizia di cui al punto 2. sopra)
            Dim leggiPivaDaImprese_codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim pivaDaImprese_codici As String = leggiPivaDaImprese_codici.Piva_from_IdCodValCod(enum_CodiciAnagrafe.CodiceCUAA, o.codiceFiscale, ObjParametri_Server)

            If Not String.IsNullOrEmpty(pivaDaImprese_codici) Then
                pivaAziendaFittizia = pivaDaImprese_codici
            Else
                Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze
                pivaAziendaFittizia = objAgroSe.NuovoId_Tabella("impresa", 0, 0, ObjParametri_Server).ToString.Replace("-", "F")
            End If

            'nel caso di utenti aziendali in cui non viene specificato il codice fiscale, quindi viene assegnato uguale alla p.iva
            If String.IsNullOrEmpty(o.codiceFiscale) Then
                o.codiceFiscale = pivaAziendaFittizia
            End If

            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(o.username, "", "", 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)
            Dim dt_UtenteVerificaCodiceFiscale = objUtenti.Leggi_DatiUtente_e_DatiSuperUser("", "", "", 0, 0, Nothing, 0, False, 0, " Dettagli_Utenti.CodFisc = '" & UtilityProvider.Agro_SQL_SaveText(o.codiceFiscale) & "' ", "", ObjParametri_Utenti)


            accountManagerVerificaDatiObbligatori(dt_Utente, dt_UtenteVerificaCodiceFiscale, o, res, enum_AP_TipoAccountManagerRequest.RichiestaEstesa)
            If res.statusCode = 500 Then
                Return res
            End If

            ProfilatoreUtenzeHelper.AccountManagerEstesaImpostaValoriObjParametri(
                o.username, o.codiceFiscale, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer
            )

            'l'impostazione della ragione sociale deve essere fatta prima della chiamata ad accountmanager
            o.ragsoc = o.nome & " - " & o.cognome

            'ACCOUNTMANAGERCOMMON: Prima chiamata di generazione account utente, se non esiste.
            Dim resAccountManager1 As AccountManagerResponse =
                accountManagerCommon(
                    o,
                    enum_AP_TipoAccountManagerRequest.RichiestaEstesa,
                    ObjParametri_Utenti,
                    ObjParametri_Server,
                    ObjParametri_SuperServer,
                    FlagAziendaPersona:=2
                 )



            If resAccountManager1.statusCode = 500 Then
                Throw New Exception(resAccountManager1.message)
            End If




            Dim letturaCodiciIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim proCodIstat As String = ""
            Dim comCodIStat As String = ""
            Dim capIstat As String = ""

            IstatOttieniCodici(o, ObjParametri_Server, letturaCodiciIstat, proCodIstat, comCodIStat, capIstat)

            o.via &= ", " & o.numeroCivico


            'genero impresa con i dati di fatturazione. laddove non esiste. L'azienda "fittizia" legata all'utente 
            ' per la gestione pratiche diventa figlia di quella appena creata.

            If Not String.IsNullOrEmpty(pivaPerFatturazione) Then

                'se non esiste l'azienda allora va creata nel caso di chiamata da "esterni" (verifico esistenza record in tabella Imprese)
                Dim impreseLeggiXverifica As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim dtImpreseLeggiXVerifica As DataTable =
                    impreseLeggiXverifica.EsisteRecordInTabellaImprese(pivaPerFatturazione, ObjParametri_Server)

                If dtImpreseLeggiXVerifica.Rows.Count = 0 Then

                    profileManagerGeneraImpresa(
                        ObjParametri_Server.PivaSuperUser,
                        pivaPerFatturazione,
                        ragioneSocialePerFatturazione,
                        o.username,
                        ObjParametri_Server,
                        ObjParametri_Utenti,
                        o.via,
                        o.citta,
                        o.cap,
                        o.citta,
                        o.provincia,
                        "Italia",
                        proCodIstat,
                        comCodIStat
                   )

                End If

            End If

            Dim listaAnnotazioni As New List(Of String)
            Dim annotazionePassaggio As String = ""
            If o.numeroCoupon <> "" Then
                listaAnnotazioni.Add("Numero Coupon: " & o.numeroCoupon)
            End If

            If o.tipologia <> "" Then
                listaAnnotazioni.Add("Tipologia: " & o.tipologia)
            End If

            If o.opzioni_pagamento <> "" Then
                listaAnnotazioni.Add("Opzioni Pagamento: " & o.opzioni_pagamento)
            End If

            annotazionePassaggio = String.Join(",", listaAnnotazioni.ToArray)

            Dim profileManagerRequest As New ProfileManagerRequest With {
                .username = o.username,
                .impreseProfilateRequest = {New ImpreseProfilateRequest With {
                    .impresa = o.codiceFiscale,
                    .impresa_piva = pivaAziendaFittizia,
                    .serviziRequest = {New ServiziRequest With {
                        .servizio_id = TipiEnumerativi.GetEnumInt(Of enum_Servizi)(enum_Servizi.Profitosan_serverSide),
                        .stato_id = TipiEnumerativi.GetEnumInt(Of enum_WWorflow_WAnagraficaStati)(enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__in_demo_30_gg),
                        .annotazioniStato = annotazionePassaggio
                        }}
                    }}
                }



            'PROFILEMANAGERCOMMON: seconda chiamata di profilazione pratiche
            Dim profileManagerResponse As ProfileManagerResponse =
                profileManagerCommon(
                    profileManagerRequest,
                    enum_AP_TipoAccountManagerRequest.RichiestaEstesa,
                    ObjParametri_Utenti,
                    ObjParametri_Server,
                    ObjParametri_SuperServer,
                    o.via,
                    o.citta,
                    o.cap,
                    o.citta,
                    o.provincia,
                    "Italia",
                    proCodIstat,
                    comCodIStat,
                    pivaPerFatturazione
                 )


            If profileManagerResponse.statusCode = 500 Then
                Throw New Exception(profileManagerResponse.message)
            End If


            'genero contatto figlio dell'impesa super-user a scopo di fattuazione elettronica..
            profileManagerGeneraContatto(
            PivaSuperUser,
            pivaPerFatturazione,
            o.codiceFiscale,
            o.nome,
            o.cognome,
            ragioneSocialePerFatturazione,
            o.via,
            o.citta,
            o.cap,
            o.citta,
            o.provincia,
            "Italia",
            proCodIstat,
            comCodIStat,
            o.pec,
            o.sdi,
            o.tipoContattoFatturaInt,
            contattoPersonaFisicaGiuridicaRichiestaAPI,
            ObjParametri_Server
         )


            If dtUtentiVerificaEsistenza.Rows.Count = 0 Then
                Dim TipoMail_Chiave As String = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("") & "_" & o.email

                Dim fullFileNameWithPathTestoEmail As String =
                    Hosting.HostingEnvironment.ApplicationPhysicalPath & "\Testi\HTML_EMAIL_RISPOSTA.htm"

                Dim AccodaEmailUserPassword As New AgronicaCoreProfilazioneBIZ.ProfilazioneMailing
                Dim emailProfilazioneTestoBody As String = ProfilatoreUtenzeHelper.GetEmailProfilazioneTestoBody(o, resAccountManager1.token, fullFileNameWithPathTestoEmail, ObjParametri_Server)
                Dim EmailProfilazioneOggetto As String = ProfilatoreUtenzeHelper.GetEmailProfilazioneOggetto()
                AccodaEmailUserPassword.AccodaEmailNotifica(o.email, EmailProfilazioneOggetto, emailProfilazioneTestoBody, TipoMail_Chiave, ObjParametri_Server)

            End If

            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleUtenti, ObjParametri_Utenti)
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleServer, ObjParametri_Server)
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleSuperServer, ObjParametri_SuperServer)

            'risposta
            res.statusCode = 200
            res.message = "OK"
            res.token = resAccountManager1.token


        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message

            'Faccio il rollback della transazione
            If Not ObjParametri_Utenti.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Utenti)
            End If

            'Faccio il rollback della transazione
            If Not ObjParametri_Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If

            'Faccio il rollback della transazione
            If Not ObjParametri_SuperServer.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_SuperServer)
            End If

        Finally


            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocaleUtenti, ObjParametri_Utenti)
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocaleServer, ObjParametri_Server)
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocaleSuperServer, ObjParametri_SuperServer)

        End Try

        Return res
    End Function


    Private Sub AccountManagerEstesaGestioneUtenteEsistente(ByRef o As AccountManagerEstesaRequest, ByRef ObjParametri_Utenti As AgronicaCoreParametri, dtUtentiDettagli As DataTable)

        o.username = dtUtentiDettagli(0)("username")

        'se l'utete è stato creato con la procedura precedente occorre impostare i dati per le successive query, se ncessario.

        'impostare una visibilità su "Niente", altrimenti fino a chiamata di profileManager  se l'utente si logga vede tutto.
        impostaVisibilitaUtente(o.username, "abc", ObjParametri_Utenti)

        'impostazione Utenti_xGruppi_Utente su gruppo default, per cartografia et al.
        Dim objUtentiGruppi As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_W
        Dim objUtentiGruppi_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim dtVerificaGruppi As DataTable =
            objUtentiGruppi_R.Leggi(o.username, 99999999, "", "", ObjParametri_Utenti)

        If dtVerificaGruppi.Rows.Count = 0 Then
            objUtentiGruppi.Scrivi(99999999, o.username, AGRODATAINIZIO, AGRODATAFINE, ObjParametri_Utenti)
        End If

    End Sub


    Private Shared Sub IstatOttieniCodici(o As AccountManagerEstesaRequest, ByRef ObjParametri_Server As AgronicaCoreParametri, letturaCodiciIstat As AgronicaCoreMetaSchemaDAL.Istat_R, ByRef proCodIstat As String, ByRef comCodIStat As String, ByRef capIstat As String)
        letturaCodiciIstat.CodiciISTAT_from_DatiDaImportare_ImportFRG(o.provincia, "", o.citta, o.cap, proCodIstat, comCodIStat, capIstat, ObjParametri_Server)

        If String.IsNullOrEmpty(proCodIstat) Then
            'dato di default...
            proCodIstat = "015"
            comCodIStat = "146"
        End If

        If String.IsNullOrEmpty(comCodIStat) Then
            Dim leggiPrimoComInProvincia As DataTable =
                letturaCodiciIstat.ISTATXListaProvince(proCodIstat, "", "", "", o.cap, "", "", "", "", "", ObjParametri_Server)

            If leggiPrimoComInProvincia.Rows.Count > 0 Then
                comCodIStat = leggiPrimoComInProvincia(0)("COM")
            End If
        End If
    End Sub




    Public Function accountManagerEsterni(o As AccountManagerRequest) As AccountManagerResponse Implements IProfilatoreUtenze.accountManagerEsterni


        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri(o.username, o.codiceFiscale, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)


        Return accountManagerCommon(o, enum_AP_TipoAccountManagerRequest.Esterna, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer)

    End Function

    Public Function accountManager(o As AccountManagerRequest) As AccountManagerResponse Implements IProfilatoreUtenze.accountManager


        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri(o.username, o.codiceFiscale, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        Return accountManagerCommon(o, enum_AP_TipoAccountManagerRequest.Interna, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer)

    End Function

    Private Function accountManagerCommon(
            o1 As GenericRequest,
            ByVal AP_TipoAccountManagerRequest As enum_AP_TipoAccountManagerRequest,
            ByVal ObjParametri_Utenti As AgronicaCoreParametri,
            ByVal ObjParametri_Server As AgronicaCoreParametri,
            ByVal ObjParametri_SuperServer As AgronicaCoreParametri,
            Optional ByVal FlagAziendaPersona As Integer = 1
        ) As AccountManagerResponse


        Dim FlagTransazioneLocaleUtenti As Boolean = False
        Dim FlagConnessioneLocaleUtenti As Boolean = False

        Dim FlagTransazioneLocaleSuperServer As Boolean = False
        Dim FlagConnessioneLocaleSuperServer As Boolean = False


        Dim nomeFunzione = "AccountManager"
        Dim res As New AccountManagerResponse

        Dim o
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
            o = CType(o1, AccountManagerEstesaRequest)
        Else
            o = CType(o1, AccountManagerRequest)
        End If


        'verifico se sono autorizzato o meno alla chiamata del metodo.
        Dim serviziAutorizzatiWebConfigVerifica As String = verificaSeChiamataAutorizzata(AP_TipoAccountManagerRequest)

        If serviziAutorizzatiWebConfigVerifica = "" Then
            res.statusCode = 500
            res.message = "Chiamata alla api non autorizzata"
            Return res
        End If

        Try


            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Return res1
            End If

            Dim user = o.username

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(user, "", "", 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)
            Dim dt_UtenteVerificaCodiceFiscale = objUtenti.Leggi_DatiUtente_e_DatiSuperUser("", "", "", 0, 0, Nothing, 0, False, 0, " Dettagli_Utenti.CodFisc = '" & UtilityProvider.Agro_SQL_SaveText(o.codiceFiscale) & "' ", "", ObjParametri_Utenti)

            accountManagerVerificaDatiObbligatori(dt_Utente, dt_UtenteVerificaCodiceFiscale, o, res, AP_TipoAccountManagerRequest)
            If res.statusCode = 500 Then
                Return res
            End If


            'la verifica dati è passata, quindi procedo.



            'Apro la connessione, transazione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleSuperServer,
                FlagTransazioneLocaleSuperServer,
                ObjParametri_SuperServer
            )

            'Apro la connessione, transazione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleUtenti,
                FlagTransazioneLocaleUtenti,
                ObjParametri_Utenti
            )

            Dim ragSoc As String
            If Not AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
                ragSoc = o.nome & " - " & o.cognome
            Else
                ragSoc = o.ragsoc
            End If

            Dim objUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W


            If dt_Utente.Rows.Count = 0 Then

                'nuovo utente


                Dim objUtentiW As New AgronicaCoreUtentiDAL.UtentiGias_Write

                Dim objUtentiGruppi As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_W


                If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
                    objUtentiW.Scrivi_2(o.username, o.password, 1, ObjParametri_Utenti, "1")
                    objUtentiDettagli.Scrivi(o.username, o.cognome, o.nome, o.via, "", o.citta, o.provincia, o.cap, o.tel, o.fax, o.email, "", o.codiceFiscale, ragSoc, FlagAziendaPersona, True, "", ObjParametri_Utenti, cellulare:=o.cellulare)
                Else
                    objUtentiW.Scrivi_2(o.username, o.password, 1, ObjParametri_Utenti, "0")
                    objUtentiDettagli.Scrivi(o.username, "", "", o.via, o.numeroCivico, o.citta, o.provincia, o.cap, o.tel, o.fax, o.email, "", o.codiceFiscale, ragSoc, 1, True, "", ObjParametri_Utenti)
                End If


                'impostare una visibilità su "Niente", altrimenti fino a chiamata di profileManager  se l'utente si logga vede tutto.
                impostaVisibilitaUtente(o.username, "abc", ObjParametri_Utenti)

                'impostazione Utenti_xGruppi_Utente su gruppo default, per cartografia et al.
                objUtentiGruppi.Scrivi(99999999, o.username, AGRODATAINIZIO, AGRODATAFINE, ObjParametri_Utenti)

            Else


                'utente esistente
                If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
                    objUtentiDettagli.Modifica(o.username, o.cognome, o.nome, o.via, o.numeroCivico, o.citta, o.provincia, o.cap, o.telefono, o.fax, o.email, "", o.codiceFiscale, ragSoc, FlagAziendaPersona, True, "", ObjParametri_Utenti, Cellulare:=o.cellulare)
                End If

            End If



            Dim xToken As String = ""
            Dim Applicazione_Richiedente As Integer =
                enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Richieste

            If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
                Applicazione_Richiedente = enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Richieste_Estese
            End If

            xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()

            'TODO:
            'memorizzare il token per recupero successivo della procedura di login
            Dim objTokenScrivi As New AgronicaCoreUtentiDAL.Utenti_Token_W

            salvataggioToken(o, Applicazione_Richiedente, ObjParametri_SuperServer, xToken)




            'chiudi transazione

            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleUtenti, ObjParametri_Utenti)
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleSuperServer, ObjParametri_SuperServer)

            'risposta
            res.statusCode = 200
            res.message = "OK"
            res.token = xToken



        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message

            'Faccio il rollback della transazione
            If Not ObjParametri_Utenti.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Utenti)
            End If


            'Faccio il rollback della transazione
            If Not ObjParametri_SuperServer.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_SuperServer)
            End If


        End Try
        Return res
    End Function


    Public Function profileManagerVerificaSingoloCUAA(impresa As ImpreseProfilateRequest, serviziRes As List(Of ServiziResponse), servizio As ServiziRequest, servizioRes As ServiziResponse) As Boolean
        If impresa.impresa.Length <> 16 AndAlso impresa.impresa.Length <> 11 Then
            If Not serviziRes Is Nothing Then
                servizioRes = impostaEsitoNegativo("Dimensione del campo CUAA diverso da 11 o 16 caratteri", servizio, servizioRes)
                serviziRes.Add(servizioRes)
            End If

            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Verifica impresa
    ''' </summary>
    ''' <param name="impresa"></param>
    ''' <param name="serviziRes"></param>
    ''' <param name="servizio"></param>
    ''' <param name="servizioRes"></param>
    ''' <returns>true se ci sono errori</returns>
    Public Function profileManagerVerificaSingolaimpresa(impresa As ImpreseProfilateRequest, serviziRes As List(Of ServiziResponse), servizio As ServiziRequest, servizioRes As ServiziResponse, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim rvalCuaa As Boolean = profileManagerVerificaSingoloCUAA(impresa, serviziRes, servizio, servizioRes)
        If rvalCuaa Then
            Return True
        End If

        If impresa.impresa_piva = "" Then
            If Not serviziRes Is Nothing Then
                servizioRes = impostaEsitoNegativo("P.iva impresa Assente", servizio, servizioRes)
                serviziRes.Add(servizioRes)
            End If
            Return True
        End If

        If impresa.impresa = "" Then
            If Not serviziRes Is Nothing Then
                servizioRes = impostaEsitoNegativo("CUAA impresa Assente", servizio, servizioRes)
                serviziRes.Add(servizioRes)
            End If

            Return True
        End If

        If impresa.impresa.Length > 11 And impresa.impresa.Substring(0, 11) = impresa.impresa_piva Then
            If Not serviziRes Is Nothing Then
                servizioRes = impostaEsitoNegativo("Se si utilizza un CUAA che è un codice fiscale di 16 cifre, allora la p.iva non può coincidere con le prime 11 cifre del codice fiscale.", servizio, servizioRes)
                serviziRes.Add(servizioRes)
            End If

            Return True
        End If

        If impresa.impresa_piva.Length <> 11 Then
            If Not serviziRes Is Nothing Then
                servizioRes = impostaEsitoNegativo("La p.iva non puà superare le 11 cifre in lunghezza", servizio, servizioRes)
                serviziRes.Add(servizioRes)
            End If

            Return True
        End If

        If Not servizio Is Nothing AndAlso servizio.servizio_id = 1017 AndAlso servizio.stato_id = 1002 Then

            'richiesta attivazione demetra, test necessari                    
            Dim esito As Boolean =
                VerificaEsistenzaImpiantiPeriodo(impresa.impresa_piva, objParametri_Server, objParametri_Utenti)
            If Not esito Then
                servizioRes = impostaEsitoNegativo("esiste un piano colturale per l'annata agraria relativa al periodo indicato", servizio, servizioRes)
                serviziRes.Add(servizioRes)
            End If
            Return Not esito
        End If

        'nessun errore
        Return False
    End Function

    Private Shared Function DemetraRiportoWorkFlowAttivazioneAStatoPrec(impresa As ImpreseProfilateRequest, ByRef objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim statoAttuale As enum_WWorflow_WAnagraficaStati
        Dim leggiStatoAttuale As New Pratiche_R
        Dim esitoGestioneWorkflow As New RispostaStandard
        esitoGestioneWorkflow.RispostaOK = True

        'se si tratta di un azienda già profilata allora procedo riportando indietro lo stato nel workflow di attivazione.
        statoAttuale = leggiStatoAttuale.StatoAttualeDaPivaServizio(impresa.impresa_piva, "", enum_Servizi.Workflow_di_attivazione_aziende_GIAS, objParametri_Server)

        If statoAttuale = enum_WWorflow_WAnagraficaStati.Attivazione_Aziende_Agrarie_in_GIAS_Impresa_correttamente_profilata Then

            Dim gestioneWorkflow As New Pratiche_W
            Dim idStatoRichiesto As enum_WWorflow_WAnagraficaStati = enum_WWorflow_WAnagraficaStati.Attivazione_Aziende_Agrarie_in_GIAS_Anagrafica_Impresa_Memorizzata

            Dim iUltimoStatoDaTransizioneDiStatoDaPivaServizio As Integer =
                leggiStatoAttuale.UltimoStatoDaTransizioneDiStatoDaPivaServizio(impresa.impresa_piva, impresa.impresa, enum_Servizi.Workflow_di_attivazione_aziende_GIAS, objParametri_Server)

            If iUltimoStatoDaTransizioneDiStatoDaPivaServizio <> 0 Then
                idStatoRichiesto = iUltimoStatoDaTransizioneDiStatoDaPivaServizio
            End If

            esitoGestioneWorkflow =
                gestioneWorkflow.impostaPratica(enum_WWorflow.Attivazione_Aziende_Agrarie_in_GIAS,
                                                impresa.impresa_piva,
                                                impresa.impresa,
                                                objParametri_Server.UtenteUsername,
                                                enum_Servizi.Workflow_di_attivazione_aziende_GIAS,
                                                idStatoRichiesto,
                                                objParametri_Server,
                                                objParametri_Utenti, 0, "", 0, False, "",
                                                AGRODATAINIZIO, AGRODATAFINE, 0, 0)

        End If

        Return esitoGestioneWorkflow
    End Function

    Public Function profileManager(o As ProfileManagerRequest) As ProfileManagerResponse Implements IProfilatoreUtenze.profileManager

        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri(o.username, o.username, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        Return profileManagerCommon(o, enum_AP_TipoAccountManagerRequest.Interna, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer)
    End Function


    Public Function profileManagerEsterni(o As ProfileManagerRequest) As ProfileManagerResponse Implements IProfilatoreUtenze.profileManagerEsterni

        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri(o.username, o.username, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        Return profileManagerCommon(o, enum_AP_TipoAccountManagerRequest.Esterna, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer)
    End Function

    Public Function profileManagerCommon(
        o As ProfileManagerRequest,
        AP_TipoAccountManagerRequest As enum_AP_TipoAccountManagerRequest,
        ByVal ObjParametri_Utenti As AgronicaCoreParametri,
        ByVal ObjParametri_Server As AgronicaCoreParametri,
        ByVal ObjParametri_SuperServer As AgronicaCoreParametri,
        Optional ByVal Ind_Des As String = "Milano",
        Optional ByVal Frz_Des As String = "Milano",
        Optional ByVal CAP As String = "20151",
        Optional ByVal Com_Des As String = "Milano",
        Optional ByVal Pro_Cod As String = "MI",
        Optional ByVal Stato As String = "Italia",
        Optional ByVal Pro_Cod_Istat As String = "015",
        Optional ByVal Com_Cod_Istat As String = "146",
        Optional ByVal PivaPadreInGerarchia As String = ""
    ) As ProfileManagerResponse

        If PivaPadreInGerarchia = "" Then
            PivaPadreInGerarchia = ObjParametri_Server.PivaSuperUser
        End If

        Dim nomeFunzione = "profileManager"
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.Esterna Then
            nomeFunzione = "profileManagerEsterni"
        End If
        If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
            nomeFunzione = "profileManagerEstesa"
        End If

        Dim res As New ProfileManagerResponse
        'verifico se sono autorizzato o meno alla chiamata del metodo.
        Dim serviziAutorizzatiWebConfigVerifica As String = verificaSeChiamataAutorizzata(AP_TipoAccountManagerRequest)

        If serviziAutorizzatiWebConfigVerifica = "" Then
            res.statusCode = 500
            res.message = "Chiamata alla api non autorizzata"
            Return res
        End If




        'verifica del token del chiamante
        Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
        If res1.statusCode = 500 Then
            res.statusCode = 500
            res.message = res1.message
            Return res
        End If

        'username riservato per token di autenticazione
        If o.username = SuperUserUsername Then
            res.statusCode = 500
            res.message = "Non può esiste un utente con questo username. Scegliere uno username differente."
            Return res
        End If


        'piva coldiretti, riservata
        Dim xL As Integer = (From ll In o.impreseProfilateRequest Where ll.impresa = PivaSuperUser Or ll.impresa_piva = PivaSuperUser).Count
        If xL > 0 Then
            res.statusCode = 500
            res.message = "Non può essere profilata un'impresa con piva " & PivaSuperUser & "."
            Return res
        End If

        Try

            Dim user = o.username
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(user, "", "", 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)

            If dt_Utente.Rows.Count = 0 Then
                res.statusCode = 500
                res.message = "Utente non presente in archivio"
                Return res
            End If


            Dim determinaSeImpostareVisibilita As Boolean =
                DecidiSeImpostareVisibilitaUtente(user, ObjParametri_Utenti)


            Dim FlagTransazioneLocaleUtenti As Boolean = False
            Dim FlagTransazioneLocaleServer As Boolean = False

            Dim FlagConnessioneLocaleUtenti As Boolean = False
            Dim FlagConnessioneLocaleServer As Boolean = False

            'Apro la connessione, transazione al DB utenti
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleUtenti,
                FlagTransazioneLocaleUtenti,
                ObjParametri_Utenti
            )

            'Apro la connessione, transazione al DB server
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleServer,
                FlagTransazioneLocaleServer,
                ObjParametri_Server
            )



            'scorre le richiste di servizio+stato per impresa

            Dim impreseRes As New List(Of ImpreseProfilateResponse)
            Dim impreseReader As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

            'anche se i servizi+stati sono per impresa, esiste in gias un profilo di permessi non legato all'impresa.
            'determino quindi il profilo più "largo" rispetto alle diverse richieste.
            Dim id_ServizioXTipologiaPermessiGias As Integer = 0
            Dim id_StatoXDurataPermessiGias As Integer = 0

            Dim almenoUnAziendaPassaVerifica As Boolean = False
            Dim TuttiServiziProfilatiCorrettamente As Boolean = True

            Dim impresaCorrenteWorkflowProfilato As Boolean = False

            For Each impresa In o.impreseProfilateRequest


                Dim impresaProf As New ImpreseProfilateResponse
                Dim serviziRes As New List(Of ServiziResponse)


                ' VAnni: 16/5/2018: un'eventuale piva memorizzata ed associata in GIAS al cuaa passato come parametro è prioritaria rispetto al valore della p.iva passata al web service, che viene sostituita.
                ' laddove non viene trovata nessuna p.iva in GIAS e non è stata passata alcuna p.iva al web service allora viene generata con il meccanismo dei progressivi.

                Dim ErroriSuCUAA As Boolean =
                    profileManagerVerificaSingoloCUAA(impresa, Nothing, Nothing, Nothing)

                Dim ErroriSuImpresa As Boolean


                If Not ErroriSuCUAA Then

                    Dim impreseCodiciLeggi As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

                    Dim pivaLetta As String = impreseCodiciLeggi.Piva_from_IdCodValCod(1010, impresa.impresa, ObjParametri_Server)

                    If pivaLetta = "" Then

                        If impresa.impresa_piva = "" Then

                            Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze
                            impresa.impresa_piva = objAgroSe.NuovoId_Tabella("impresa", 0, 0, ObjParametri_Server).ToString.Replace("-", "F")
                        Else

                            ' VAnni: 26/6/2018: se viene passata la p.iva allora verifico se è già stato memorizzato in precedenza il dato del CUAA ed uso comunque questo dato.
                            Dim CuaaLetto As String =
                                impreseCodiciLeggi.Leggi_CUAA(impresa.impresa_piva, ObjParametri_Server)
                            If CuaaLetto <> "" Then
                                impresa.impresa = CuaaLetto
                            End If

                        End If

                    Else
                        impresa.impresa_piva = pivaLetta

                    End If


                    ErroriSuImpresa =
                        profileManagerVerificaSingolaimpresa(impresa, Nothing, Nothing, Nothing, ObjParametri_Server, ObjParametri_Utenti)

                Else
                    'ci sono errori su cuaa, quindi fallisce anche verifica impresa
                    ErroriSuImpresa = True

                End If

                If Not almenoUnAziendaPassaVerifica Then
                    almenoUnAziendaPassaVerifica = Not ErroriSuImpresa
                End If


                'gli utenti non hanno profili/permessi basati su p.iva, quindi determino in base a tutte le richieste qual'è il servizio "superiore".
                'lo stesso ragionamento si applica sullo stato, se viene richiesta la disattivazione di un servizio al momento questa avviene anche sugli altri.

                For Each servizio In impresa.serviziRequest

                    id_ServizioXTipologiaPermessiGias = determinaServizioSuperiore(servizio.servizio_id)
                    determinaStatoSuperiore(servizio.stato_id, id_StatoXDurataPermessiGias)

                Next


                Dim listaServiziPerImpostazione As List(Of ServiziRequest)

                If id_StatoXDurataPermessiGias = enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Non_Attivo Then

                    'tutti i servizi a prescindere da quanto passato vanno impostati su "non Attivo".
                    Dim xProfilatore As New AgronicaCoreProfilazioneDAL.Pratiche_R
                    Dim xDtServiziDisattivare As DataTable =
                        xProfilatore.LeggiXAccounting(impresa.impresa_piva, "", "", ObjParametri_Server)

                    listaServiziPerImpostazione = (
                        From ll In xDtServiziDisattivare.AsEnumerable
                        Select New ServiziRequest With {
                            .servizio_id = CInt(ll("Servizio_Id")),
                            .stato_id = id_StatoXDurataPermessiGias
                            }
                        ).ToList()
                Else
                    listaServiziPerImpostazione = impresa.serviziRequest.ToList

                End If


                impresaCorrenteWorkflowProfilato = False

                For Each servizio In listaServiziPerImpostazione


                    Dim servizioRes As New ServiziResponse

                    'verifiche su singola impresa.. (riporta lo stesso errore su tutti i servizi richiesti)

                    Dim bContinueFor As Boolean =
                        profileManagerVerificaSingolaimpresa(impresa, serviziRes, servizio, servizioRes, ObjParametri_Server, ObjParametri_Utenti)

                    'passo al servizio successivo se ci sono errori sulla singola impresa...
                    If bContinueFor Then
                        TuttiServiziProfilatiCorrettamente = False
                        Continue For
                    End If

                    '' VAnni: 14/5/2018: verifica dei servizi autorizzati in base a web.config
                    Dim kLettura As String = "serviziAutorizzati"
                    Dim serviziAutorizzatiWebConfig As String = "1001,1002,1003,1004,1005,1006"
                    If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.Esterna Then
                        kLettura = "serviziAutorizzatiEsterni"
                        serviziAutorizzatiWebConfig = "1007"
                    End If

                    If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
                        kLettura = "serviziAutorizzatiEstesi"
                        serviziAutorizzatiWebConfig = "1008"
                    End If
                    'se chiave assente vai con i default
                    Try
                        serviziAutorizzatiWebConfig = System.Configuration.ConfigurationManager.AppSettings(kLettura)
                    Catch ex As Exception
                    End Try


                    Dim vServiziAutorizzati As String() = serviziAutorizzatiWebConfig.Split(",")

                    'marco errore..
                    If Not vServiziAutorizzati.Contains(servizio.servizio_id.ToString) Then
                        servizioRes = impostaEsitoNegativo("Servizio non autorizzabile", servizio, servizioRes)
                        serviziRes.Add(servizioRes)

                        'passo al successivo...
                        TuttiServiziProfilatiCorrettamente = False
                        Continue For
                    End If


                    '' VAnni: 16/5/2018: imposto l'avanzamento del workflow di servizio
                    servizioRes = impostaPratica(
                        impresa.impresa_piva,
                        impresa.impresa,
                        user,
                        servizio.servizio_id,
                        servizio.stato_id,
                        servizio.annotazioniStato,
                        ObjParametri_Server,
                        ObjParametri_Utenti
                   )


                    'solo una volta per impresa corrente nel ciclo..
                    If Not impresaCorrenteWorkflowProfilato Then
                        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
                        Dim dtPratica As DataTable =
                            praticheR.Leggi_2(0, impresa.impresa_piva, impresa.impresa, 0, 0, 0, enum_Servizi.Workflow_di_attivazione_aziende_GIAS, AGRODATAINIZIO, AGRODATAFINE, "", "", ObjParametri_Server, False)

                        impresaCorrenteWorkflowProfilato = (dtPratica.Rows.Count > 0)
                    End If


                    '' VAnni: 16/5/2018: imposto l'avanzamento del workflow di attivazione (1050)
                    If AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.Esterna And servizioRes.esitoBool And Not impresaCorrenteWorkflowProfilato Then

                        Dim xImpostaWorkflow As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                        Dim rsImpostaWorklow As RispostaStandard =
                            xImpostaWorkflow.impostaPratica(
                                enum_WWorflow.Attivazione_Aziende_Agrarie_in_GIAS,
                                impresa.impresa_piva,
                                impresa.impresa,
                                o.username,
                                enum_Servizi.Workflow_di_attivazione_aziende_GIAS,
                                enum_WWorflow_WAnagraficaStati.Attivazione_Aziende_Agrarie_in_GIAS_Impresa_correttamente_profilata,
                                ObjParametri_Server,
                                ObjParametri_Utenti,
                                0, "", 0, False, "",
                                AGRODATAINIZIO, AGRODATAFINE, 0, 0
                           )



                        If Not rsImpostaWorklow.RispostaOK Then

                            impostaEsitoNegativo("Errore in fase di attivazione azienda (1050)", servizio, servizioRes)
                            serviziRes.Add(servizioRes)

                            TuttiServiziProfilatiCorrettamente = False
                            Continue For

                        End If

                    End If
                    'fine imposto avanzamento WorkFlow.

                    '' VAnni: 30/11/2020: Demetra, se la verifica del PC è passata (mi trovo comunque qui perchè altrimenti sarei passato al servizio successivo, vedi flag bContinueFor).
                    If servizio.servizio_id = enum_Servizi.QDemetra OrElse
                        servizio.servizio_id = enum_Servizi.QDemetraQdCBluarancio Then

                        Dim rvalRiportoWorkFlowDemetra As RispostaStandard =
                            DemetraRiportoWorkFlowAttivazioneAStatoPrec(impresa, ObjParametri_Server, ObjParametri_Utenti)

                        If Not rvalRiportoWorkFlowDemetra.RispostaOK Then
                            servizioRes = impostaEsitoNegativo(rvalRiportoWorkFlowDemetra.RispostaStringa, servizio, servizioRes)
                            serviziRes.Add(servizioRes)

                            'passo al successivo...' VAnni: 30/11/2020:' VAnni: 30/11/2020:
                            TuttiServiziProfilatiCorrettamente = False
                            Continue For

                        End If

                    End If


                    'finalmente tutto in ordine ... imposto esito (sia esso positivo o negativo) ...
                    serviziRes.Add(servizioRes)

                    If Not servizioRes.esitoBool Then
                        TuttiServiziProfilatiCorrettamente = False
                    End If


                Next 'Prossimo Servizio

                'solo se ho impostato tutti i servizi
                If Not ErroriSuImpresa And TuttiServiziProfilatiCorrettamente Then

                    'se non esiste l'azienda allora va creata nel caso di chiamata da "esterni" (verifico esistenza record in tabella Imprese)
                    Dim impreseLeggiXverifica As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim dtImpreseLeggiXVerifica As DataTable =
                        impreseLeggiXverifica.EsisteRecordInTabellaImprese(impresa.impresa_piva, ObjParametri_Server)


                    If dtImpreseLeggiXVerifica.Rows.Count = 0 AndAlso (
                        AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.Esterna Or
                        AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa
                    ) Then


                        profileManagerGeneraImpresa(
                            PivaPadreInGerarchia,
                            impresa.impresa_piva,
                            "",
                            o.username,
                            ObjParametri_Server,
                            ObjParametri_Utenti,
                            Ind_Des,
                            Frz_Des,
                            CAP,
                            Com_Des,
                            Pro_Cod,
                            "Italia",
                            Pro_Cod_Istat,
                            Com_Cod_Istat
                         )


                    End If

                    If (impresa.impresa_piva <> "" And impresa.impresa <> "") Then
                        impostaCuaaImpreseCodici(impresa.impresa, impresa.impresa_piva, ObjParametri_Server)
                    End If

                    If determinaSeImpostareVisibilita AndAlso impresa.impresa_piva <> "" Then
                        impostaVisibilitaUtente(o.username, impresa.impresa_piva, ObjParametri_Utenti)
                    End If

                End If

                impresaProf.impresa = impresa.impresa
                impresaProf.serviziResponse = serviziRes.ToArray
                impreseRes.Add(impresaProf)

            Next 'Prossima impresa




            'se almeno un'impresa passa la verifica e per non ci sono errori di profilazione significa che quell'impresa è stata profilata correttamente,
            'a questo punto posso passare alla gestione dell'utente.

            If almenoUnAziendaPassaVerifica And TuttiServiziProfilatiCorrettamente Then

                'imposta permessi / profili utente
                'il tutto in transazione, rispetto alle chiamate precedenti..? da capire
                Dim dettEsPermessiProfili As GenericResponse = impostaPermessiProfiloUtente(user, determinaSeImpostareVisibilita, id_ServizioXTipologiaPermessiGias, id_StatoXDurataPermessiGias, ObjParametri_Utenti)

                If dettEsPermessiProfili.statusCode = 500 Then

                    'Faccio il rollback della transazione
                    If Not ObjParametri_Utenti.objTransazione Is Nothing Then
                        ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Utenti)
                    End If

                    'Faccio il rollback della transazione
                    If Not ObjParametri_Server.objTransazione Is Nothing Then
                        ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
                    End If

                    res.statusCode = 500
                    res.message = dettEsPermessiProfili.message
                    Return res
                End If

            End If

            Dim dettEs As New DettaglioEsito
            dettEs.username = user
            dettEs.impreseProfilateResponse = impreseRes.ToArray

            'Chiudo la connessione al DB utenti 
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleUtenti, ObjParametri_Utenti)

            'Chiudo la connessione al DB server
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleServer, ObjParametri_Server)



            res.dettaglioEsito = dettEs
            res.statusCode = 200
            res.message = "OK"
            res.token = ""

        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message


            'Faccio il rollback della transazione
            If Not ObjParametri_Utenti.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Utenti)
            End If

            'Faccio il rollback della transazione
            If Not ObjParametri_Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If
        End Try


        Return res


    End Function


    Private Shared Function VerificaEsistenzaImpiantiPeriodo(ByVal piva As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As Boolean


        Dim finestraInizioPrec As Date = objParametri_Server.FinestraTemporaleInizio
        Dim finestraFinePRec As Date = objParametri_Server.FinestraTemporaleFine
        Dim userPrec As String = objParametri_Utenti.UtenteUsername

        Try

            'Dim annataAgraria_inizio As Date
            'Dim annataAgraria_fine As Date
            Dim adesso As Date = Now.Date

            objParametri_Utenti.UtenteUsername = objParametri_Server.SuperUserUsername

            'Dim u As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            'u.AnnataAgraria(adesso, annataAgraria_inizio, annataAgraria_fine, objParametri_Utenti)

            objParametri_Server.FinestraTemporaleInizio = adesso
            objParametri_Server.FinestraTemporaleFine = adesso

            Dim xFiltroDemetra As String = " NOT EXISTS (SELECT 1 FROM reg_impianti_codici cc WHERE cc.id_cod = " & enum_CodiceAnagrafe_Clienti.Demetra & " and cc.piva = r.piva and cc.sa_Cod = r.sa_cod and cc.appezza = r.appezza and cc.id_reg = r.id_reg) "

            Dim verifica As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim dtImp As DataTable =
                verifica.Leggi_Intersezioni(objParametri_Server.PivaSuperUser, piva, 0, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, xFiltroDemetra, "", objParametri_Server, True)



            If (dtImp.Rows.Count = 0) Then
                Return True
            End If

        Catch ex As Exception
            Return False

        Finally
            'sempre eseguito.
            objParametri_Server.FinestraTemporaleInizio = finestraInizioPrec
            objParametri_Server.FinestraTemporaleFine = finestraFinePRec
            objParametri_Utenti.UtenteUsername = userPrec
        End Try

        Return False
    End Function

    Private Shared Function verificaSeChiamataAutorizzata(ByVal enum_AP_TipoAccountManagerRequest As enum_AP_TipoAccountManagerRequest) As String

        Dim kLetturaVerifica As String = "serviziAutorizzati"
        Dim serviziAutorizzatiWebConfigVerifica As String = ""


        If enum_AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.RichiestaEstesa Then
            kLetturaVerifica = "serviziAutorizzatiEstesi"
        Else
            If enum_AP_TipoAccountManagerRequest = enum_AP_TipoAccountManagerRequest.Esterna Then
                kLetturaVerifica = "serviziAutorizzatiEsterni"
            End If
        End If


        'se chiave assente vai con i default
        Try
            serviziAutorizzatiWebConfigVerifica = System.Configuration.ConfigurationManager.AppSettings(kLetturaVerifica)
        Catch ex As Exception
        End Try

        Return serviziAutorizzatiWebConfigVerifica
    End Function


    Private Sub profileManagerGeneraContatto(
        ByVal PivaAppartenenzaContatto As String,
        ByVal piva As String,
        ByVal CodiceFiscale As String,
        ByVal nome As String,
        ByVal cognome As String,
        ByVal strRagSoc As String,
        ByVal Ind_Des As String,
        ByVal Frz_Des As String,
        ByVal CAP As String,
        ByVal Com_Des As String,
        ByVal Pro_Cod As String,
        ByVal Stato As String,
        ByVal Pro_Cod_Istat As String,
        ByVal Com_Cod_Istat As String,
        ByVal pec As String,
        ByVal sdi As String,
        ByVal TipoContattoFattura As enumTipoContattoFattura,
        ByVal contattoPersonaFisicaGiuridica As enum_Contatti_IdCf,
        objParametri_server As AgronicaCoreParametri
    )

        If TipoContattoFattura = 0 Then
            TipoContattoFattura = enumTipoContattoFattura.Privato
        End If

        Dim cod_Contatto As String


        If contattoPersonaFisicaGiuridica = enum_Contatti_IdCf.PersonaGiuridica Then
            nome = ""
            cognome = ""
            cod_Contatto = piva
        Else
            strRagSoc = ""
            cod_Contatto = CodiceFiscale
        End If


        Dim leggiContattoPerVerifica As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dtLeggiContattoPerVerifica As DataTable =
                leggiContattoPerVerifica.LeggiDatiMinimi(
                    PivaAppartenenzaContatto,
                    cod_Contatto,
                    "",
                    objParametri_server
            )



        If dtLeggiContattoPerVerifica.Rows.Count = 0 Then


            Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze

            'impresa
            Dim xImpresaW As New AgronicaCoreAnagrafeDAL.Contatti_W

            xImpresaW.Scrivi(
                Piva:=PivaAppartenenzaContatto,
                Sa_Cod:=0,
                Cod_Contatto:=cod_Contatto,
                Id_CF:=contattoPersonaFisicaGiuridica,
                Rag_Soc:=strRagSoc,
                Codice_Fiscale:=CodiceFiscale,
                Convenevoli:="",
                Tipo_Indirizzo_Default:=enum_IndirizzoTipo.SedeLegale,
                Nome:=nome,
                Cognome:=cognome,
                Data_Nascita:=AGRODATAINIZIO,
                Sesso:="",
                Cod_Contatto_Referente:="",
                Validita_Inizio:=AGRODATAINIZIO,
                Validita_Fine:=AGRODATAFINE,
                objParametri:=objParametri_server
             )


            Dim contattiCodScrivi As New AgronicaCoreAnagrafeDAL.Contatti_Codici_W

            'dati di fatturazione elettronica..
            contattiCodScrivi.Scrivi(PivaAppartenenzaContatto, 0, cod_Contatto, 1303, TipoContattoFattura, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)

            If pec <> "" Then
                contattiCodScrivi.Scrivi(PivaAppartenenzaContatto, 0, cod_Contatto, 4019, pec, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)
            End If

            If sdi <> "" Then
                contattiCodScrivi.Scrivi(PivaAppartenenzaContatto, 0, cod_Contatto, 4020, sdi, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)
            End If


            'indirizzi
            Dim indirizzoCod As Integer = objAgroSe.NuovoId_Tabella("indirizzi", 0, 0, objParametri_server)

            Dim xIndirizzoScrivi As New AgronicaCoreAnagrafeDAL.Indirizzi_Write



            xIndirizzoScrivi.Scrivi(indirizzoCod, Ind_Des, Frz_Des, CAP, Com_Des, Pro_Cod, Stato, "", Pro_Cod_Istat, Com_Cod_Istat, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)

            'impresaXindirizzi
            Dim impresaXindirizziScrivi As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_W
            impresaXindirizziScrivi.Scrivi(PivaAppartenenzaContatto, 0, cod_Contatto, indirizzoCod, enum_IndirizzoTipo.SedeLegale, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)


            'risorse umane
            Dim risumScrivi As New AgronicaCoreAnagrafeDAL.Risorse_Umane_W
            Dim Cod_RisUm As Integer =
                objAgroSe.NuovoId_Tabella("RISORSE_UMANE", 0, 0, objParametri_server)

            risumScrivi.Scrivi(
                Piva:=PivaAppartenenzaContatto,
                Sa_Cod:=0,
                Cod_RisUm:=Cod_RisUm,
                Cod_Contatto:=cod_Contatto,
                Cod_Rapporto:=enum_Rapporti_Contabili_Standard.Cliente,
                Settore_Des:="",
                Attivita_Des:="",
                Corrispettivo_Mensile:=0,
                Corrispettivo_Orario:=0,
                Ore_Settimanali:=0,
                Giorni_Ferie:=0,
                Ferie_Godute:=0,
                Giorni_Malattia:=0,
                Occasionale:=0,
                Patentino:="",
                Data_Rilascio_Patentino:=AGRODATAINIZIO,
                Data_Scadenza_Patentino:=AGRODATAFINE,
                Ente_di_rilascio:="",
                Cod_RisUm_Origine:=0,
                Piva_SuperUser_Origine:="",
                Saldo_Iniziale_Crediti:=0,
                Saldo_Iniziale_Debiti:=0,
                ChkSpesometro:=0,
                Validita_Inizio:=AGRODATAINIZIO,
                Validita_Fine:=AGRODATAFINE,
                objParametri:=objParametri_server
             )





        End If
        'contatto inesistente


    End Sub

    Private Sub profileManagerGeneraImpresa(
        ByVal PivaPadreInGerarchia As String,
        ByVal piva As String,
        ByVal RagSoc As String,
        ByVal Username As String,
        objParametri_server As AgronicaCoreParametri,
        objparametri_utenti As AgronicaCoreParametri,
        Optional ByVal Ind_Des As String = "Milano",
        Optional ByVal Frz_Des As String = "Milano",
        Optional ByVal CAP As String = "20151",
        Optional ByVal Com_Des As String = "Milano",
        Optional ByVal Pro_Cod As String = "MI",
        Optional ByVal Stato As String = "Italia",
        Optional ByVal Pro_Cod_Istat As String = "015",
        Optional ByVal Com_Cod_Istat As String = "146"
    )

        Dim strRagSoc As String

        If Not String.IsNullOrEmpty(RagSoc) Then
            strRagSoc = RagSoc
        Else

            Dim xRagSoc As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim dtRagSoc As DataTable =
                xRagSoc.Leggi(Username, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objparametri_utenti)

            If dtRagSoc.Rows.Count = 0 Then
                Throw New Exception("Dettagli Utente non trovati.")
            End If

            strRagSoc = dtRagSoc.Rows(0)("Rag_Soc")

        End If

        Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze


        'impresa
        Dim xImpresaW As New AgronicaCoreAnagrafeDAL.Imprese_Write
        xImpresaW.Scrivi(piva, strRagSoc, "", "", "", "", 0, enum_TipoImpresaGerarchia.Impresa, "", AGRODATAINIZIO, AGRODATAFINE, objParametri_server)

        'indirizzi
        Dim indirizzoCod As Integer = objAgroSe.NuovoId_Tabella("indirizzi", 0, 0, objParametri_server)

        Dim xIndirizzoScrivi As New AgronicaCoreAnagrafeDAL.Indirizzi_Write



        xIndirizzoScrivi.Scrivi(indirizzoCod, Ind_Des, Frz_Des, CAP, Com_Des, Pro_Cod, Stato, "", Pro_Cod_Istat, Com_Cod_Istat, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)

        'impresaXindirizzi
        Dim impresaXindirizziScrivi As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_W
        impresaXindirizziScrivi.Scrivi(piva, indirizzoCod, enum_IndirizzoTipo.SedeOperativa, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)

        'GerarchiaImprese
        Dim gerarchiaImpreseScrivi As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_W
        gerarchiaImpreseScrivi.Scrivi(PivaPadreInGerarchia, piva, AGRODATAINIZIO, AGRODATAFINE, objParametri_server, objparametri_utenti)

        'utentixImprese
        Dim utentixImpreseScrivi As New AgronicaCoreAnagrafeDAL.UtentixImprese_Write
        utentixImpreseScrivi.Scrivi(piva, AGRODATAINIZIO, AGRODATAFINE, objParametri_server)

    End Sub

    Private Sub determinaStatoSuperiore(StatoRichiesto As Integer, ByRef StatoDaAggiornare As Integer)

        'sono già in stato disattivato, esco!
        If StatoDaAggiornare = enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Non_Attivo Then
            'nessuna modifica sullo stato
            Exit Sub
        End If

        'viene richiesta disattivazione, imposto stato ed esco!
        If StatoRichiesto = enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Non_Attivo Then
            StatoDaAggiornare = StatoRichiesto
            Exit Sub
        End If


        'Passo da uno stato in demo ad uno stato pagante..
        If (({
            enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__in_demo_30_gg,
            enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__in_demo_60_gg
        }).Contains(StatoDaAggiornare) And
        ({
            enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__Pagante,
            enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__Gratuito
        }).Contains(StatoRichiesto)) Or StatoDaAggiornare = 0 Then

            StatoDaAggiornare = StatoRichiesto
            Exit Sub
        End If

    End Sub

    Private Shared Function impostaEsitoNegativo(messaggio As String, servizio As ServiziRequest, servizioRes As ServiziResponse) As ServiziResponse
        servizioRes.esito = messaggio
        servizioRes.esitoBool = False
        servizioRes.servizio_id = servizio.servizio_id
        servizioRes.stato_id = servizio.stato_id
        Return servizioRes
    End Function

    Private Sub impostaCuaaImpreseCodici(cuaa As String, piva As String, ByVal objParametri As AgronicaCoreParametri)


        Dim impreseCodiciLeggi As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim impreseCodiciScrivi As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

        Dim cuaaLetto As String = impreseCodiciLeggi.Leggi_CUAA(piva, objParametri)

        If String.IsNullOrEmpty(cuaaLetto) Then
            impreseCodiciScrivi.Scrivi(piva, 1010, cuaa, AGRODATAINIZIO, AGRODATAFINE, objParametri)
        End If

    End Sub

    Private Function DecidiSeImpostareVisibilitaUtente(user As String, objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim rval As Boolean = False

        Dim leggiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim dtDettagliLetti As DataTable =
            leggiDettagli.Leggi(user, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Utenti, True)

        If dtDettagliLetti.Rows.Count > 0 AndAlso (
                dtDettagliLetti(0)("UserNameCommerciale") = "" Or
                dtDettagliLetti(0)("UserNameCommerciale").ToString.ToLower.Contains("agronicawebapiprofilatore")
            ) Then

            rval = True

        End If

        Return rval

    End Function

    Private Function impostaVisibilitaUtente(user As String, piva As String, objParametri_Utenti As AgronicaCoreParametri) As GenericResponse

        Dim res As New GenericResponse

        Try

            Dim utenteProfiliLeggi As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim dtProfili As DataTable = utenteProfiliLeggi.Leggi(user, 5, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Utenti)
            Dim vecchioFiltroImpostato As String = ""


            Dim listaPivaLette As List(Of String)
            If dtProfili.Rows.Count > 0 Then
                vecchioFiltroImpostato = dtProfili.Rows(0)("Descrizione_1")
                listaPivaLette = impostaVisibilitaUtente_LeggiPiveDaXml(vecchioFiltroImpostato, piva)

            Else
                listaPivaLette = New List(Of String)
                listaPivaLette.Add(piva)
            End If


            Dim utente_profili As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
            Dim d1 As String = ""
            Dim d2 As String = ""
            impostaVisibilitaUtente_descr1descr2(listaPivaLette, d1, d2)

            If dtProfili.Rows.Count > 0 Then
                utente_profili.Modifica(user, 5, d1, d2, 0, 0, "", objParametri_Utenti)
            Else
                utente_profili.Scrivi(user, 5, d1, d2, 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
            End If


        Catch ex As Exception
            res.statusCode = 500
            res.message = "impostaVisibilitaUtente - " & ex.Message
        End Try


        Return res


    End Function

    Private Function impostaVisibilitaUtente_LeggiPiveDaXml(ByVal sXml As String, ByVal curPiva As String) As List(Of String)

        Dim xDocPivas As XDocument = XDocument.Parse(sXml)

        Dim rval As List(Of String) = (From ll In xDocPivas.Elements("DatiFiltri").Elements("Filtro").Elements("Impresa") Where CStr(ll.Attribute("piva")) <> "abc" Select CStr(ll.Attribute("piva"))).ToList

        If Not rval.Contains(curPiva) Then
            rval.Add(curPiva)

        End If

        Return rval

    End Function

    Private Sub impostaVisibilitaUtente_descr1descr2(ByVal listaPive As List(Of String), ByRef d1 As String, ByRef d2 As String)

        If listaPive.Count = 1 Then

            d1 = "<DatiFiltri><Filtro><Impresa piva='" & listaPive.First & "'><Struttura><Appezzamento><Impianto><Agenda><Contatto /></Agenda></Impianto></Appezzamento></Struttura></Impresa><DatiGerarchiaImprese /></Filtro></DatiFiltri>"
            d2 = "Imprese.piva='" & listaPive.First & "'"
        Else

            Dim xDocRval As XDocument = XDocument.Parse("<DatiFiltri><Filtro><DatiGerarchiaImprese/></Filtro></DatiFiltri>")
            Dim lRval As New List(Of String)

            For Each curPiva In listaPive

                Dim xN As XElement = <Impresa piva=<%= curPiva %>><Struttura><Appezzamento><Impianto><Agenda><Contatto/></Agenda></Impianto></Appezzamento></Struttura></Impresa>
                xDocRval.Element("DatiFiltri").Element("Filtro").Add(xN)

                lRval.Add("Imprese.piva = '" & curPiva & "'")

            Next

            d1 = xDocRval.ToString()
            d2 = " ( " & String.Join(" OR ", lRval.ToArray) & " )"

        End If


        'Descrizione_1	Descrizione_2
        '<DatiFiltri><Filtro><DatiGerarchiaImprese/><DatiPive><Piva piva="01511110221" /<> piva piva="01551420225" /><Piva piva="00894290220"/><Piva piva="0089429O220"/></DatiPive><Impresa><Struttura><Appezzamento><Impianto><Agenda><Contatto/></Agenda></Impianto></Appezzamento></Struttura></Impresa></Filtro></DatiFiltri> |||||	And (  Imprese.piva = '01511110221' OR  Imprese.piva = '01551420225' OR  Imprese.piva = '00894290220' OR  Imprese.piva = '0089429O220' )
    End Sub



    Private Function impostaPermessiProfiloUtente(user As String, UTENTE_Attiva_Configurazione_Pratica_AttivaFlag As Boolean, servizio_id As Integer, statoCod As Integer, objParametri_Utenti As AgronicaCoreParametri) As GenericResponse

        Dim validitaFineRichiesta As DateTime
        Dim rval As New GenericResponse

        'pre-requisiti: esiste l'utente (già verificato), 
        'esiste la tipologia, NON ci sono già permessi sull'utente

        Dim utentiPermessiLeggi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim utentiPermessiScrivi As New AgronicaCoreUtentiDAL.Utenti_Permessi_W

        ' VAnni: 11/4/2018: se viene richiesta la chiusura..
        If statoCod = enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Non_Attivo Then
            Dim rvalCancella As Boolean = utentiPermessiScrivi.Cancella(user, 5, 0, 0, 0, "", objParametri_Utenti)

            'se ok..
            If rvalCancella Then
                rval.statusCode = 200
                rval.message = "OK"
            Else
                rval.statusCode = 500
                rval.message = "Errore in fase di revoca dei permessi utente"
            End If

            rval.token = ""

            Return rval

        End If

        Dim tipologia_cod As Integer = determinaTipologiaUtenteDaServizi(servizio_id)
        validitaFineRichiesta = DeterminaValiditaFineDaStato(statoCod)

        Dim utentiTipologieScrivi As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
        Dim utentiTipologieLeggi As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R
        Dim impostazioneLeggi As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim impostazioneScrivi As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        Dim bDeterminaAggiornamentoDataNecessario As Boolean = False
        Dim dtDeterminaAggiornamentoDataNecessario As DataTable =
            utentiPermessiLeggi.Leggi(user, 5, 0, 0, 0, "", "", objParametri_Utenti)

        'se esiste un record di permessi allora è necessari impostare una nuova validità fine 
        'se la data richiesta è differente
        If dtDeterminaAggiornamentoDataNecessario.Rows.Count > 0 Then
            Dim validitaFinePrecedente As DateTime = dtDeterminaAggiornamentoDataNecessario(0)("Validita_Fine")

            ' VAnni: 31/8/2017: il fatto che le date si possano solo spostare in avanti garantisce che a fronte di chiamate su aziende diverse
            'in momenti differenti i permessi di maggiore validità impostati rimangono validi
            If validitaFinePrecedente < validitaFineRichiesta Then
                bDeterminaAggiornamentoDataNecessario = True
            End If

        End If

        utentiTipologieScrivi.GeneraPermessiUtenteDaTipologia(tipologia_cod, user, AGRODATAINIZIO, validitaFineRichiesta, objParametri_Utenti)

        If tipologia_cod = -4 Then
            Dim xUtentiImpostazioniDelete As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            xUtentiImpostazioniDelete.Cancella2(user, 0, "", objParametri_Utenti)
        End If

        ' VAnni: 8/11/2017: l'impostazione UTENTE_Attiva_Configurazione_Pratica  va copiata solo per gli utenti web, non per quelli caa.
        If UTENTE_Attiva_Configurazione_Pratica_AttivaFlag Then
            utentiTipologieScrivi.GeneraImpostazioniUtenteDaTipologia(tipologia_cod, user, AGRODATAINIZIO, validitaFineRichiesta, "", objParametri_Utenti)
        Else
            utentiTipologieScrivi.GeneraImpostazioniUtenteDaTipologia(tipologia_cod, user, AGRODATAINIZIO, validitaFineRichiesta, " i.Impostazione_Cod <> " & enum_Impostazioni_Utenti.UTENTE_Attiva_Configurazione_Pratica, objParametri_Utenti)
        End If

        ' VAnni: 8/5/2019: in caso di escalation le impostazioni vanno sovrascritte, se necessario (a meno del Q-Fert dove vengono sovrascritte).
        If tipologia_cod <> -4 Then

            'legge le impostazioni
            Dim dtImpostazioniPerUpdate As DataTable =
                impostazioneLeggi.Leggi2(1, tipologia_cod, 0, " Impostazione_Valore_4 = 'update' ", "", objParametri_Utenti)


            For Each drImposta As DataRow In dtImpostazioniPerUpdate.Rows
                'aggiorno tutto a meno del "valore 4"
                impostazioneScrivi.Modifica2(
                    user,
                    drImposta("Impostazione_cod"),
                    drImposta("Impostazione_Valore_1"),
                    drImposta("Impostazione_Valore_2"),
                    drImposta("Impostazione_Valore_3"),
                    "",
                    drImposta("Validita_Inizio"),
                    drImposta("Validita_Fine"),
                    objParametri_Utenti
                )
            Next


        End If

        If tipologia_cod = -4 Then
            Dim xUtentiImpostazioniFiltroMonoDelete As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W
            xUtentiImpostazioniFiltroMonoDelete.Cancella(user, 0, "", objParametri_Utenti)
        End If
        utentiTipologieScrivi.GeneraImpostazioniUtenteFiltroMonoDaTipologia(tipologia_cod, user, AGRODATAINIZIO, validitaFineRichiesta, objParametri_Utenti)


        If bDeterminaAggiornamentoDataNecessario Then
            utentiTipologieScrivi.AggiornaValiditaPermessiUtente(5, user, validitaFineRichiesta, objParametri_Utenti)
        End If


        'se ok..
        rval.statusCode = 200
        rval.message = "OK"
        rval.token = ""

        Return rval

    End Function

    Public Function DeterminaValiditaFineDaStato(ByVal stato_cod As enum_WWorflow_WAnagraficaStati) As DateTime

        Dim DaysToAdd As Integer = 0
        If stato_cod = enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__in_demo_30_gg Then
            DaysToAdd = 30
        End If

        If stato_cod = enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__in_demo_60_gg Then
            DaysToAdd = 60
        End If

        If DaysToAdd > 0 Then

            Dim d As Date = Now.Date
            d = d.AddDays(DaysToAdd)
            Return d
        Else
            Return AGRODATAFINE
        End If

    End Function

    Private Function determinaTipologiaUtenteDaServizi(ByVal id_servizio As Integer) As Integer

        Dim rval As Integer

        Select Case id_servizio
            Case enum_Servizi.Profitosan
                rval = enum_TipologieUtenti.Profitosan

            Case enum_Servizi.QStandard
                rval = enum_TipologieUtenti.QStandard

            Case enum_Servizi.QPlus
                rval = enum_TipologieUtenti.QPlus

            Case enum_Servizi.QBio
                rval = enum_TipologieUtenti.QBio

            Case enum_Servizi.QFert
                rval = enum_TipologieUtenti.QFert

            Case enum_Servizi.QMaps
                rval = enum_TipologieUtenti.QMaps

            Case enum_Servizi.Condizionalita2018
                rval = enum_TipologieUtenti.Condizionalita

            Case enum_Servizi.Profitosan_serverSide
                rval = enum_TipologieUtenti.ProfitosanServerSide

            Case enum_Servizi.QDemetra
                rval = enum_TipologieUtenti.QDemetra

            Case enum_Servizi.QDemetraQdCBluarancio
                rval = enum_TipologieUtenti.QDemetra

            Case Else

                Throw New Exception("Impossibile determinare una Tipologia Utente Dal Servizio (non valido) : " & id_servizio)

        End Select


        Return rval

    End Function

    Private Function determinaServizioSuperiore(ByVal id_servizio As Integer) As Integer

        Dim rval As Integer

        Select Case id_servizio
            Case 1001 'Profitosan
                rval = 1001

            Case 1002
                rval = 1002

            Case 1003
                rval = 1003

            Case 1004
                rval = 1004

            Case 1005
                rval = 1005

            Case 1006
                rval = 1006

            Case 1007
                rval = 1007

            Case 1008
                rval = 1008

            Case 1017
                rval = 1017

            Case Else


        End Select


        Return rval

    End Function

    Private Function impostaPratica(
        piva As String,
        cuaa As String,
        user As String,
        servizio_id As Integer,
        statoFinaleRichiesto As Integer,
        AnnotazioniStato As String,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    ) As ServiziResponse

        Dim res As New ServiziResponse
        res.servizio_id = servizio_id
        res.stato_id = statoFinaleRichiesto
        res.esito = "Operazione eseguita correttamente"
        res.esitoBool = True

        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim praticheW As New AgronicaCoreProfilazioneDAL.Pratiche_W


        Dim dtPratica = praticheR.Leggi_2(0, piva, cuaa, 0, 0, 0, servizio_id, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

        Dim esitoTransizioneStatoOk As Boolean = False
        Dim statoIniziale As Integer = 0

        Dim pratica_cod As Integer

        If dtPratica.Rows.Count = 0 Then

            'Pratica non ancora inserita

            esitoTransizioneStatoOk = ControllaTransizionediStatoIniziale(1001, statoFinaleRichiesto)

            If esitoTransizioneStatoOk Then
                agroProgressivo("Pratiche", objParametri_Server, pratica_cod)
                praticheW.Scrivi(pratica_cod,
                                 cuaa & " - " & servizio_id,
                                 piva,
                                 cuaa, 0, 0, 0, servizio_id,
                                 AGRODATAINIZIO,
                                 AGRODATAFINE,
                                 objParametri_Server,
                                 DateTime.Now,
                                 DateTime.Now,
                                 objParametri_Server.PivaSuperUser,
                                 objParametri_Server.PivaSuperUser, 0,
                                 "", "", AGRODATAINIZIO, "", 0, 0)



                'praticheStati.Scrivi()
            Else
                res.esito = "Stato non valido"
                res.esitoBool = False
            End If
        Else

            pratica_cod = dtPratica(0)("pratica_Cod")

            'recupero lo stato iniziale in cui si trova la pratica
            Dim praticheStatiLeggi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
            Dim xDtoPraticheStati As DataTable =
                praticheStatiLeggi.Leggi(pratica_cod, "", "", objParametri_Server)

            statoIniziale = xDtoPraticheStati(0)("Stato_Cod")

            esitoTransizioneStatoOk = ControllaTransizionediStato(servizio_id, statoIniziale, statoFinaleRichiesto, objParametri_Server)

        End If


        If esitoTransizioneStatoOk Then

            Dim dataStatoAttuale As Date = DeterminaValiditaFineDaStato(statoFinaleRichiesto)

            Dim praticheStatiAttualiScrivi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W
            Dim praticheStatiScrivi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W

            'scrivo stato attuale
            If dtPratica.Rows.Count = 0 Then
                praticheStatiAttualiScrivi.Scrivi(pratica_cod, statoFinaleRichiesto, AnnotazioniStato, Now(), dataStatoAttuale, objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")
            Else
                praticheStatiAttualiScrivi.Modifica(pratica_cod, statoFinaleRichiesto, statoIniziale, AnnotazioniStato, Now(), dataStatoAttuale, "", objParametri_Server)
            End If

            'scrivo transizione di stato.    
            Dim PassaggioDiStato_cod As Integer
            agroProgressivo("passaggiodistato_cod", objParametri_Server, PassaggioDiStato_cod)
            praticheStatiScrivi.Scrivi(pratica_cod, statoFinaleRichiesto, servizio_id, Now(), AGRODATAFINE, AnnotazioniStato, statoIniziale, PassaggioDiStato_cod, objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")
        Else

            res.esito = "Stato non valido"
            res.esitoBool = False

        End If


        Return res
    End Function

    Private Shared Sub agroProgressivo(ByVal tabella As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef pratica_cod As Integer)
        'Scrivo Pratica
        Dim TopCode As Integer
        Dim BaseCode As Integer
        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode,
                                                                      TopCode,
                                                                      1)
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        pratica_cod = objSequenze.NuovoId_Tabella(tabella, BaseCode, TopCode, objParametri_Server)
    End Sub

    Private Function ControllaTransizionediStatoIniziale(WorkFlow_Cod As Integer, StatoF As Integer) As Boolean

        Dim rval As Boolean = False



        If ({1006, 1002, 1003, 1004, 1005}).Contains(StatoF) Then

            rval = True

        End If

        Return rval

    End Function

    ''' <summary>
    ''' Dato il workflow, lo stato iniziale, lo stato finale ed il servizio richiesti, verifica se esiste una transizione di stato configurata
    ''' </summary>    
    ''' <param name="servizio_Cod"></param>
    ''' <param name="StatoI"></param>
    ''' <param name="StatoF"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Private Function ControllaTransizionediStato(servizio_Cod As Integer, StatoI As Integer, StatoF As Integer, objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        If praticheR.Leggi_TransizioniDisponibiliDatoStato(StatoI, servizio_Cod, "", "", objParametri_Server).Select(" WAnagraficaStati_Cod=" & StatoF & " ").Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function autenticazioneDemetra(o As AuthenticationRequestDemetra) As AuthenticationResponse Implements IProfilatoreUtenze.autenticazioneDemetra
        Dim nomeFunzione = "autenticazioneDemetra"
        Dim res As New AuthenticationResponse

        Try

            Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
            Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
            Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

            Dim su = ConfigurationManager.AppSettings("SuperUserUsername")

            ProfilatoreUtenzeHelper.GetObjParametri(su, "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante("105", ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Dim ctx As WebOperationContext = WebOperationContext.Current
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden
                Return New AuthenticationResponse With {.message = "Accesso Negato [Token non valido]"}
            End If

            'se il token è in ordine procedo
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

            Dim username = o.username
            Dim dt = objUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Utenti, username)

            If dt.Rows.Count <= 0 Then
                'verifico se il cuaa esiste prima di procedere alla creazione
                Dim Piva As String = ""
                Dim xImpR As New AgronicaCoreAnagrafeBIZ.Impresa_R
                If xImpR.VerificaEsistenzaImpresaByCUAA(o.CUAA, ObjParametri_Server) = False Then
                    Dim ctx As WebOperationContext = WebOperationContext.Current
                    ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest
                    Return New AuthenticationResponse With {.message = "Accesso Negato [Azienda agricola inesistente]"}
                Else
                    Dim xRead As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                    Piva = xRead.Piva_from_CUAA(o.CUAA, ObjParametri_Server)
                    If Piva = "" Then
                        Dim ctx As WebOperationContext = WebOperationContext.Current
                        ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest
                        Return New AuthenticationResponse With {.message = "Accesso Negato [Azienda agricola inesistente]"}
                    End If
                End If

                Dim confString = ConfigurationManager.AppSettings("BaseUrlColdirettiWebServicePortaleSocio")
                If confString = "" Then
                    Dim ctx As WebOperationContext = WebOperationContext.Current
                    ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError
                    Return New AuthenticationResponse With {.message = "Errore lettura url web service coldiretti."}
                End If

                'Lavez - 19/05/2025 - aggiunta chiamata ad endpoint persone-fisiche in caso quello per le persone giuridiche non restituisca nulla
                Dim ObjUser = GetCodiceUtenteColdirettiPersonaGiuridica(o.CUAA, confString)
                If ObjUser Is Nothing OrElse ObjUser.GetUserCode() = "" Then
                    ObjUser = GetCodiceUtenteColdirettiPersonaFisica(o.CUAA, confString)
                End If
                If ObjUser Is Nothing OrElse username <> ObjUser.GetUserCode() Then
                    Dim ctx As WebOperationContext = WebOperationContext.Current
                    ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest
                    Return New AuthenticationResponse With {.message = "Accesso Negato [Utente azienda agricola non trovato su servizi coldiretti]"}
                End If
                Dim defProfile = ConfigurationManager.AppSettings("ProfiloPermessiDefault")
                If defProfile = "" Then
                    Dim ctx As WebOperationContext = WebOperationContext.Current
                    ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError
                    Return New AuthenticationResponse With {.message = "Errore lettura parametri utente."}
                End If

                Dim ObjCreazioneUtente = MapUserToAgronicaUtente(ObjUser, username, Piva, CInt(defProfile), ObjParametri_Server)



                Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita
                Dim objGisBIZ As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W


                objUtentiBIZ.ImpostaVisibilitaAzienda(ObjCreazioneUtente.Utente, ObjCreazioneUtente.AziendeVisibili,
                                              True, True, ObjParametri_Server, ObjParametri_Utenti)
                objGisBIZ.ImpostaLayerGisSePermessiCartografia(ObjCreazioneUtente.Utente.UserName, ObjParametri_Server)

            End If
            'reinizializzo gli objparametri con l'utente chiamante per poter gestire poi il token
            ProfilatoreUtenzeHelper.GetObjParametri(o.username, "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

            Dim xGDPR As New AgronicaCoreUtentiBIZ.GDPR

            If xGDPR.CheckGDPRAcceptation(o.username, ObjParametri_Utenti) = False Then
                xGDPR.ForceGDPRAcceptation(o.username, ObjParametri_Utenti)
            End If

            Dim tmpPwd As String = objUtenti.Password_From_UserName(o.username, ObjParametri_Utenti)

            Dim oUsr = New GenericRequest With {.username = o.username, .password = tmpPwd}

            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(oUsr.username, "", oUsr.password, 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)

            If dt_Utente.Rows.Count = 0 Then
                res.statusCode = 500
                res.message = "Utente non presente in archivio"
                Return res
            End If

            'verifica se esiste un token valido al momento della chiamata altrimenti assegno l'ultimo valido letto da database
            Dim objTokenVerifica As New AgronicaCoreUtentiDAL.Utenti_Token_R

            Dim dtTokenVerifica105 As DataTable =
                objTokenVerifica.Leggi_Token(res1.token, " applicazione_Richiedente = " & enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_G2G_API, ObjParametri_SuperServer)

            Dim TokenVerifica As String =
                objTokenVerifica.Leggi_Token("", ObjParametri_SuperServer)

            ' VAnni: 2/12/2019: al momento prevedo di leggere la configurazione del super-user, non dell'utente "tunnel"
            Dim user105 As String = ""
            'dtTokenVerifica105.Rows(0)("Utente_Username")

            Dim validitaTokenLeggi As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_R
            Dim bkUser As String = ObjParametri_SuperServer.UtenteUsername
            ObjParametri_SuperServer.UtenteUsername = user105

            Dim intervalloValiditaToken As Integer =
                validitaTokenLeggi.Leggi_Validita("", ObjParametri_SuperServer)

            ObjParametri_SuperServer.UtenteUsername = bkUser

            Dim xToken As String = ""

            If TokenVerifica = "" Then
                xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                salvataggioToken(oUsr, enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Token, ObjParametri_SuperServer, xToken)
            Else
                If intervalloValiditaToken = -1 Then
                    'lavez - 07/02/2024 - ad ogni richiesta genero un nuovo token per invalidare il precedente
                    xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                    salvataggioToken(oUsr, enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Token, ObjParametri_SuperServer, xToken)
                Else
                    Dim scadenzaToken As Date = Now.AddMinutes(intervalloValiditaToken)
                    If scadenzaToken <= Now() Then
                        xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                        salvataggioToken(oUsr, enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Token, ObjParametri_SuperServer, xToken)
                    Else
                        xToken = TokenVerifica
                    End If
                End If
            End If


            res.statusCode = 200
            res.token = xToken
            res.message = "Ok"

        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message
        End Try
        Return res
    End Function

    Public Function autenticazioneNewAgri(o As AuthenticationRequestDemetra) As AuthenticationResponse Implements IProfilatoreUtenze.autenticazioneNewAgri
        Dim nomeFunzione = "autenticazioneNewAgri"
        Dim res As New AuthenticationResponse

        Try

            Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
            Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
            Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

            Dim su = ConfigurationManager.AppSettings("SuperUserUsername")

            ProfilatoreUtenzeHelper.GetObjParametri(su, "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante("105", ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Dim ctx As WebOperationContext = WebOperationContext.Current
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden
                Return New AuthenticationResponse With {.message = "Accesso Negato [Token non valido]"}
            End If

            'se il token è in ordine procedo
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

            Dim username = o.username
            Dim dt = objUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Utenti, username)

            If dt.Rows.Count <= 0 Then
                Dim ctx As WebOperationContext = WebOperationContext.Current
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest
                Return New AuthenticationResponse With {.message = "Accesso Negato [Utente inesistente]"}
            End If

            'reinizializzo gli objparametri con l'utente chiamante per poter gestire poi il token
            ProfilatoreUtenzeHelper.GetObjParametri(o.username, "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

            Dim xGDPR As New AgronicaCoreUtentiBIZ.GDPR

            If xGDPR.CheckGDPRAcceptation(o.username, ObjParametri_Utenti) = False Then
                xGDPR.ForceGDPRAcceptation(o.username, ObjParametri_Utenti)
            End If

            Dim tmpPwd As String = objUtenti.Password_From_UserName(o.username, ObjParametri_Utenti)

            Dim oUsr = New GenericRequest With {.username = o.username, .password = tmpPwd}

            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(oUsr.username, "", oUsr.password, 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)

            If dt_Utente.Rows.Count = 0 Then
                res.statusCode = 500
                res.message = "Utente non presente in archivio"
                Return res
            End If

            'verifica se esiste un token valido al momento della chiamata altrimenti assegno l'ultimo valido letto da database
            Dim objTokenVerifica As New AgronicaCoreUtentiDAL.Utenti_Token_R

            Dim dtTokenVerifica105 As DataTable =
                objTokenVerifica.Leggi_Token(res1.token, " applicazione_Richiedente = " & enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_G2G_API, ObjParametri_SuperServer)

            Dim TokenVerifica As String =
                objTokenVerifica.Leggi_Token("", ObjParametri_SuperServer)

            ' VAnni: 2/12/2019: al momento prevedo di leggere la configurazione del super-user, non dell'utente "tunnel"
            Dim user105 As String = ""
            'dtTokenVerifica105.Rows(0)("Utente_Username")

            Dim validitaTokenLeggi As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_R
            Dim bkUser As String = ObjParametri_SuperServer.UtenteUsername
            ObjParametri_SuperServer.UtenteUsername = user105

            Dim intervalloValiditaToken As Integer =
                validitaTokenLeggi.Leggi_Validita("", ObjParametri_SuperServer)

            ObjParametri_SuperServer.UtenteUsername = bkUser

            Dim xToken As String = ""

            If TokenVerifica = "" Then
                xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                salvataggioToken(oUsr, enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Token, ObjParametri_SuperServer, xToken)
            Else
                If intervalloValiditaToken = -1 Then
                    'lavez - 07/02/2024 - ad ogni richiesta genero un nuovo token per invalidare il precedente
                    xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                    salvataggioToken(oUsr, enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Token, ObjParametri_SuperServer, xToken)
                Else
                    Dim scadenzaToken As Date = Now.AddMinutes(intervalloValiditaToken)
                    If scadenzaToken <= Now() Then
                        xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                        salvataggioToken(oUsr, enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_Token, ObjParametri_SuperServer, xToken)
                    Else
                        xToken = TokenVerifica
                    End If
                End If
            End If


            res.statusCode = 200
            res.token = xToken
            res.message = "Ok"

        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message
        End Try
        Return res
    End Function


    Public Function autenticazione(o As AuthenticationRequest) As AuthenticationResponse Implements IProfilatoreUtenze.autenticazione
        Dim nomeFunzione = "authentication"
        Dim res As New AuthenticationResponse

        Try

            Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
            Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
            Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

            ProfilatoreUtenzeHelper.GetObjParametri(o.username, "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante("105", ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Dim ctx As WebOperationContext = WebOperationContext.Current
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden
                Return New AuthenticationResponse With {.message = "Accesso Negato"}
            End If


            'se il token è in ordine procedo

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(o.username, "", o.password, 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)

            If dt_Utente.Rows.Count = 0 Then
                res.statusCode = 500
                res.message = "Utente non presente in archivio"
                Return res
            End If

            'verifica se esiste un token valido al momento della chiamata altrimenti assegno l'ultimo valido letto da database
            Dim objTokenVerifica As New AgronicaCoreUtentiDAL.Utenti_Token_R

            Dim dtTokenVerifica105 As DataTable =
                objTokenVerifica.Leggi_Token(res1.token, " applicazione_Richiedente = " & enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_G2G_API, ObjParametri_SuperServer)

            Dim TokenVerifica As String =
                objTokenVerifica.Leggi_Token("", ObjParametri_SuperServer)

            ' VAnni: 2/12/2019: al momento prevedo di leggere la configurazione del super-user, non dell'utente "tunnel"
            Dim user105 As String = ""
            'dtTokenVerifica105.Rows(0)("Utente_Username")

            Dim validitaTokenLeggi As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_R
            Dim bkUser As String = ObjParametri_Server.UtenteUsername
            ObjParametri_SuperServer.UtenteUsername = user105

            Dim intervalloValiditaToken As Integer =
                validitaTokenLeggi.Leggi_Validita("", ObjParametri_SuperServer)

            ObjParametri_Server.UtenteUsername = bkUser

            Dim xToken As String = ""

            If TokenVerifica = "" Then
                xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                salvataggioToken(o, enum_AWS_ApplicazioneRichiedente.AgronicaWidgetManager, ObjParametri_SuperServer, xToken)
            Else
                If intervalloValiditaToken = -1 Then
                    xToken = TokenVerifica
                Else
                    Dim scadenzaToken As Date = Now.AddMinutes(intervalloValiditaToken)
                    If scadenzaToken <= Now() Then
                        xToken = AgronicaCoreUtility.Login.GeneraTokenCasuale()
                        salvataggioToken(o, enum_AWS_ApplicazioneRichiedente.AgronicaWidgetManager, ObjParametri_SuperServer, xToken)
                    Else
                        xToken = TokenVerifica
                    End If
                End If
            End If


            res.statusCode = 200
            res.token = xToken
            res.message = "Ok"

        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message
        End Try
        Return res
    End Function

    Public Function authentication(o As AuthenticationRequest) As AuthenticationResponse Implements IProfilatoreUtenze.authentication
        Dim nomeFunzione = "authentication"
        Dim res As New AuthenticationResponse

        Try

            Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
            Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
            Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

            ProfilatoreUtenzeHelper.GetObjParametri(o.username, "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Return res1
            End If


            'se il token è in ordine procedo

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(o.username, "", o.password, 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)

            If dt_Utente.Rows.Count = 0 Then
                res.statusCode = 500
                res.message = "Utente non presente in archivio"
                Return res
            End If

            Dim xToken As String = AgronicaCoreUtility.Login.GeneraTokenCasuale()
            salvataggioToken(o, 103, ObjParametri_SuperServer, xToken)

            res.statusCode = 200
            res.token = xToken
            res.message = "Ok"

        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message
        End Try
        Return res
    End Function

    Private Shared Sub salvataggioToken(o As GenericRequest, Applicazione_Richiedente As Integer, ObjParametri As AgronicaCoreParametri, xToken As String)
        Dim objTokenScrivi As New AgronicaCoreUtentiDAL.Utenti_Token_W
        Dim objTokenR As New AgronicaCoreUtentiDAL.Utenti_Token_R


        If objTokenR.Esiste_Utente_Formulato(0, ObjParametri) = True Then
            objTokenScrivi.Modifica_Token(xToken, ObjParametri.SuperUserUsername, o.username, 0, cfgTokenParametri, ObjParametri)
        Else
            objTokenScrivi.Scrivi(xToken, ObjParametri.PivaSuperUser, ObjParametri.SuperUserUsername, "", o.username, "", Applicazione_Richiedente, "", 0, cfgTokenParametri, ObjParametri)

        End If

    End Sub


    ''' <summary>
    ''' restituisce lo stato di accounting sui diversi utenti
    ''' </summary>
    ''' <returns></returns>
    Public Function accounting(ByVal o As AccountingRequest) As AccountingResponse Implements IProfilatoreUtenze.accounting
        Dim nomeFunzione = "accounting"
        Dim res As New AccountingResponse


        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri("", "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        Try


            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Return res1
            End If

            Dim xProfilatore As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim dtAccounting As DataTable = xProfilatore.LeggiXAccounting(o.piva, "", "", ObjParametri_Server)

            res.statusCode = 200
            res.accounting = accountingLeggiDt(dtAccounting)
            res.message = "Ok"

        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message
        End Try
        Return res


    End Function

    Private Function accountingLeggiDt(ByVal dtAccounting As DataTable) As AccoutingUtenti()


        Dim rval As New List(Of AccoutingUtenti)

        If dtAccounting.Rows.Count = 0 Then
            Return rval.ToArray
        End If

        Dim cUtente As String = ""
        Dim cImpresa As String = ""
        Dim cServizio As String = ""

        Dim nUtente As AccoutingUtenti = Nothing
        Dim nImpresa As AccountingImpreseProfilate = Nothing
        Dim nServizi As accountingServizioStato = Nothing


        For Each dRowAccounting In dtAccounting.Rows

            'al cambio del servizio (stessa impresa)
            If cServizio <> CStr(dRowAccounting("servizio_id")) Then

                If Not nServizi Is Nothing Then
                    AgronicaCoreUtility.arrayHlp.Add(nImpresa.servizi, nServizi)
                End If

                nServizi = New accountingServizioStato With {.servizio_id = dRowAccounting("servizio_id"), .stato_id = dRowAccounting("stato_id"), .dataScadenza1 = dRowAccounting("dataScadenza")}

                cServizio = dRowAccounting("servizio_id")

            End If

            If cImpresa <> dRowAccounting("impresa") Then

                If Not nImpresa Is Nothing Then
                    AgronicaCoreUtility.arrayHlp.Add(nUtente.impreseProfilate, nImpresa)
                End If

                nImpresa = New AccountingImpreseProfilate With {.impresa = dRowAccounting("impresa")}

                cImpresa = dRowAccounting("impresa")


            End If

            If cUtente <> dRowAccounting("username") Then

                If Not nUtente Is Nothing Then
                    rval.Add(nUtente)
                End If

                nUtente = New AccoutingUtenti With {.username = dRowAccounting("username")}

                cUtente = dRowAccounting("username")

            End If

        Next

        'gli ultimi letti ovviamente sono da inserire fuori dal ciclo for each
        AgronicaCoreUtility.arrayHlp.Add(nImpresa.servizi, nServizi)
        AgronicaCoreUtility.arrayHlp.Add(nUtente.impreseProfilate, nImpresa)
        rval.Add(nUtente)

        Return rval.ToArray
    End Function



    ''' <summary>
    ''' La funzione permette di ottenere, a fronte di una chiamata dove vengono passati username e CUAA: 
    ''' 1. Una verifica su esistenza utente 
    ''' 2. Verifica su esistenza di una corretta profilazione per l'utente (l'utente è stato correttamente profilato attraverso chiamata ad api profileManager) 
    ''' 3. Verifica dell'associazione dell'utente con CUAA 
    ''' 4. Lista servizi associati al CUAA, con relativo stato e scadenza
    ''' </summary>
    ''' <param name="o"></param>
    ''' <returns></returns>
    Public Function accountingUser(o As AccountingUserRequest) As AccountingUserResponse Implements IProfilatoreUtenze.accountingUser


        Dim nomeFunzione = "AccountingUser"
        Dim res As New AccountingUserResponse


        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri("", "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        ObjParametri_Utenti.UtenteUsername = o.username

        Try

            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Return res1
            End If

            accountingUtenteVerificaDatiObbligatori(o, res)
            If res.statusCode = 500 Then
                Return res
            End If

            Dim impreseCodiciLeggi As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

            Dim pivaLetta As String = impreseCodiciLeggi.Piva_from_IdCodValCod(1010, o.cuaa, ObjParametri_Server)

            ' VAnni: 30/5/2018: commento, i controlli avanzano comunque
            'If pivaLetta = "" Then
            '    res.statusCode = 500
            '    res.message = "Al cuaa indicato non corrisponde alcuna impresa memorizzata nel sistema"
            '    Return res
            'End If

            Dim xacc2 As New List(Of AccoutingUtentiCompleto)

            Dim ac1 As New AccoutingUtentiCompleto


            ' 1. Una verifica su esistenza utente
            Dim user = o.username
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(user, "", "", 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)


            ac1.usernameEsisteBool = True
            ac1.usernameEsisteMsg = "Utente presente"

            If dt_Utente.Rows.Count = 0 Then
                ac1.usernameEsisteBool = False
                ac1.usernameEsisteMsg = "Utente non presente in archivio"
            End If



            ' 2. Verifica su esistenza di una corretta profilazione per l'utente (l'utente è stato correttamente profilato attraverso chiamata ad api profileManager) 

            Dim xLetturaImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim xLetturaPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim dtImpostazioni As DataTable =
                xLetturaImpostazioni.Leggi(172, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Utenti)

            ac1.usernameProfilatoBool = True
            ac1.usernameProfilatoMsg = ""

            ac1.impresaProfilataBool = True
            ac1.impresaProfilataMsg = ""

            If dtImpostazioni.Rows.Count = 0 Then
                ac1.usernameProfilatoBool = False
                ac1.usernameProfilatoMsg &= "Mancano le impostazioni utente; "
            End If

            Dim dtPermessiUtente As DataTable =
                xLetturaPermessi.Leggi(o.username, 0, 0, enum_Security_Operazione.Lettura, 0, "", "", ObjParametri_Utenti)

            If dtPermessiUtente.Rows.Count = 0 Then


                ac1.usernameProfilatoMsg &= "Nessun Permesso impostato per l'utente; "
                If ac1.usernameProfilatoBool Then
                    ac1.usernameProfilatoMsg &= "utente revocato; "
                End If
                ac1.usernameProfilatoBool = False
            End If

            ' 3. Verifica dell'associazione dell'utente con piva/CUAA 
            Dim xLetturaProfili As New AgronicaCoreUtentiDAL.Utenti_Profili_Read

            Dim dtProfili As DataTable =
                xLetturaProfili.Leggi(o.username, 5, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Utenti)


            Dim xRiportaAssociazioneCuaa As Boolean = False
            Dim Descrizione_2 As String = ""
            If dtProfili.Rows.Count > 0 Then
                Descrizione_2 = dtProfili.Rows(0)("Descrizione_2")
                If (Not Descrizione_2.Contains(pivaLetta)) Or pivaLetta = "" Then
                    ac1.usernameProfilatoBool = False
                    ac1.usernameProfilatoMsg &= "Nessuna associazione tra utente e CUAA;"
                    xRiportaAssociazioneCuaa = True
                End If
            Else
                ac1.usernameProfilatoBool = False
                ac1.usernameProfilatoMsg &= "Nessuna dato di associazione tra utente e CUAA presente;"
                xRiportaAssociazioneCuaa = True
            End If




            If ac1.usernameProfilatoBool Then
                ac1.usernameProfilatoMsg = "Utente profilato correttamente"
            End If


            ' 4. Lista servizi associati al CUAA, con relativo stato e scadenza

            Dim xProfilatore As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim dtAccounting As DataTable = xProfilatore.LeggiXAccounting(pivaLetta, " at.username_creazione = '" & o.username & "'", "", ObjParametri_Server)

            Dim xacc1 As AccoutingUtenti() = accountingLeggiDt(dtAccounting)


            If Not xacc1.Count = 0 Then
                ac1.impresa = xacc1.FirstOrDefault.impreseProfilate
            Else

                Dim l1 As New List(Of AccountingImpreseProfilate)
                If pivaLetta <> "" Then

                    Dim xserv() As accountingServizioStato
                    Dim xlServ As New List(Of accountingServizioStato)
                    xserv = xlServ.ToArray()

                    Dim lPivaEsiste As New AccountingImpreseProfilate With {.impresa = o.cuaa, .servizi = xserv}
                    l1.Add(lPivaEsiste)

                End If

                ac1.impresa = l1.ToArray()

            End If

            If ac1.impresa.Count = 0 Then
                ac1.impresaProfilataBool = False
                If pivaLetta = "" Then
                    ac1.impresaProfilataMsg = "Al cuaa indicato non corrisponde alcuna impresa memorizzata nel sistema;"
                End If
                If xRiportaAssociazioneCuaa Then
                    ac1.impresaProfilataMsg = "Nessuna dato di associazione tra utente e CUAA presente;"
                End If
            End If


            If ac1.impresaProfilataBool Then
                ac1.impresaProfilataMsg = "Impresa profilata correttamente"
            End If

            xacc2.Add(ac1)

            res.statusCode = 200
            res.accounting = xacc2.ToArray()
            res.message = "Ok"

        Catch ex As Exception

            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message
        End Try


        Return res




    End Function

    Private Sub accountingUtenteVerificaDatiObbligatori(o As AccountingUserRequest, res As AccountingUserResponse)

        If o.username = "" Then
            res.statusCode = 500
            res.message = "Parametro username assente"
            Exit Sub
        End If

        If o.cuaa = "" Then
            res.statusCode = 500
            res.message = "Parametro cuaa assente"
            Exit Sub
        End If
    End Sub

    'Public Function passwordReset(o As GenericRequest) As AccountManagerResponse Implements IProfilatoreUtenze.passwordReset

    '    Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
    '    Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
    '    Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing


    '    Dim idleCodiceFiscale As String = ""
    '    ProfilatoreUtenzeHelper.GetObjParametri(o.username, idleCodiceFiscale, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)


    '    'verifico se sono autorizzato o meno alla chiamata del metodo.
    '    Dim serviziAutorizzatiWebConfigVerifica As String = verificaSeChiamataAutorizzata(enum_AP_TipoAccountManagerRequest.Interna)

    '    Dim res As New AccountManagerResponse

    '    If serviziAutorizzatiWebConfigVerifica = "" Then
    '        res.statusCode = 500
    '        res.message = "Chiamata alla api non autorizzata"
    '        Return res
    '    End If

    '    'verifica del token del chiamante
    '    Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
    '    If res1.statusCode = 500 Then
    '        Return res1
    '    End If


    '    Dim FlagTransazioneLocaleUtenti As Boolean = False
    '    Dim FlagConnessioneLocaleUtenti As Boolean = False

    '    Dim nomeFunzione = "passwordReset"

    '    Try

    '        'Apro la connessione, transazione al DB
    '        ConnessioniTransazioni.ApriConnessioneXCoreBiz(
    '            FlagConnessioneLocaleUtenti,
    '            FlagTransazioneLocaleUtenti,
    '            ObjParametri_Utenti
    '        )


    '        If String.IsNullOrEmpty(o.username) OrElse String.IsNullOrEmpty(o.password) Then
    '            res.statusCode = 419
    '            res.message = "nome utente non impostato o password non impostata nella richiesta"
    '            Return res
    '        End If



    '        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
    '        Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(o.username, "", "", 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)

    '        If dt_Utente.Rows.Count = 0 Then
    '            res.statusCode = 401
    '            res.message = "Utente non presente in archivio"
    '            Return res
    '        End If



    '        Dim objUtentiD As New AgronicaCoreUtentiDAL.Utenti_Write
    '        objUtentiD.Modifica(o.username, o.password, ObjParametri_Utenti)

    '        'chiudi transazione

    '        'Chiudo la connessione al DB
    '        ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleUtenti, ObjParametri_Utenti)

    '        'risposta
    '        res.statusCode = 200
    '        res.message = "OK"
    '        res.token = ""


    '    Catch ex As Exception
    '        res.statusCode = 500
    '        res.message = nomeFunzione & " - " & ex.Message

    '        'Faccio il rollback della transazione
    '        If Not ObjParametri_Utenti.objTransazione Is Nothing Then
    '            ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Utenti)
    '        End If

    '    End Try
    '    Return res


    'End Function



    Private Function accountEdit(
            o As AccountManagerRequest
        ) As AccountManagerResponse Implements IProfilatoreUtenze.accountEdit


        Dim FlagTransazioneLocaleUtenti As Boolean = False
        Dim FlagConnessioneLocaleUtenti As Boolean = False



        Dim nomeFunzione = "accountEdit"
        Dim res As New AccountManagerResponse

        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing


        ProfilatoreUtenzeHelper.GetObjParametri(o.username, o.codiceFiscale, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)


        'verifico se sono autorizzato o meno alla chiamata del metodo.
        Dim serviziAutorizzatiWebConfigVerifica As String = verificaSeChiamataAutorizzata(enum_AP_TipoAccountManagerRequest.Interna)

        If serviziAutorizzatiWebConfigVerifica = "" Then
            res.statusCode = 500
            res.message = "Chiamata alla api non autorizzata"
            Return res
        End If

        Try


            'verifica del token del chiamante
            Dim res1 As GenericResponse = verificaTokenChiamante(ObjParametri_SuperServer)
            If res1.statusCode = 500 Then
                Return res1
            End If


            If String.IsNullOrEmpty(o.username) Then
                res.statusCode = 419
                res.message = "nome utente non impostato nella richiesta"
                Return res
            End If

            Dim user = o.username

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(user, "", "", 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)
            'Dim dt_UtenteVerificaCodiceFiscale = objUtenti.Leggi_DatiUtente_e_DatiSuperUser("", "", "", 0, 0, Nothing, 0, False, 0, " Dettagli_Utenti.CodFisc = '" & UtilityProvider.Agro_SQL_SaveText(o.codiceFiscale) & "' ", "", ObjParametri_Utenti)

            'la verifica dati è passata, quindi procedo.


            'Apro la connessione, transazione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocaleUtenti,
                FlagTransazioneLocaleUtenti,
                ObjParametri_Utenti
            )


            Dim objUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W


            If dt_Utente.Rows.Count = 0 Then

                res.statusCode = 401
                res.message = "Utente non trovato"
                Return res

            Else

                If Not String.IsNullOrEmpty(o.codiceFiscale) Then
                    res.statusCode = 500
                    res.message = "Il codice fiscale non può essere modificato."
                    Return res
                End If

                Dim ragSoc As String = o.nome & " - " & o.cognome
                If String.IsNullOrEmpty(o.nome) And String.IsNullOrEmpty(o.cognome) Then
                    ragSoc = ""
                End If

                'modifica sui singoli non vuoti..:

                accountEditModificaParametro(o.username, "via", o.via, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "numeroCivico", o.numeroCivico, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "citta", o.citta, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "provincia", o.provincia, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "cap", o.cap, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "tel", o.tel, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "fax", o.fax, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "email", o.email, ObjParametri_Utenti, objUtentiDettagli)
                accountEditModificaParametro(o.username, "rag_Soc", ragSoc, ObjParametri_Utenti, objUtentiDettagli)

            End If


            If Not String.IsNullOrEmpty(o.password) Then

                Dim handleConfigSiti As New Configurazione_Siti_R
                Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", ObjParametri_Server)
                Dim hashPasswordAbilitato As Boolean = False
                If dtConfigSiti.Rows.Count > 0 Then
                    hashPasswordAbilitato = dtConfigSiti.Rows(0)("Valore")
                End If

                Dim objUtentiD As New AgronicaCoreUtentiDAL.Utenti_Write
                objUtentiD.Modifica(o.username, o.password, hashPasswordAbilitato, False, ObjParametri_Utenti)
            End If


            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleUtenti, ObjParametri_Utenti)

            'risposta
            res.statusCode = 200
            res.message = "OK"
            res.token = ""



        Catch ex As Exception
            res.statusCode = 500
            res.message = nomeFunzione & " - " & ex.Message

            'Faccio il rollback della transazione
            If Not ObjParametri_Utenti.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Utenti)
            End If

        End Try
        Return res
    End Function

    Private Shared Sub accountEditModificaParametro(username As String, chiave As String, valore As String, ByRef ObjParametri_Utenti As AgronicaCoreParametri, objUtentiDettagli As AgronicaCoreUtentiDAL.Utenti_Dettagli_W)
        If Not String.IsNullOrEmpty(valore) Then
            objUtentiDettagli.Modifica_Parametrizzata(username, chiave, valore, "", ObjParametri_Utenti)
        End If
    End Sub

    Public Function widgetManager(o As widgetManagerRequest) As widgetManagerResponse Implements IProfilatoreUtenze.widgetManager

        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri("", "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        'verifica del token del chiamante
        Dim res1 As GenericResponse = verificaTokenChiamante(enum_AWS_ApplicazioneRichiedente.AgronicaWidgetManager, ObjParametri_SuperServer)
        If res1.statusCode = 500 Then
            Dim ctx As WebOperationContext = WebOperationContext.Current
            ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden
            Return New widgetManagerResponse With {.errori = "Accesso Negato"}
        End If


        'se il token è in ordine procedo.

        Try
            Dim rval As New widgetManagerResponse

            Dim leggicfg As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim basePath As String = leggicfg.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", ObjParametri_Server)
            Dim agnd As String = leggicfg.Leggi_Valore(0, "LinkAgronicaAgenda2010", "", "", ObjParametri_Server)

            If Not agnd.Contains("http") Then
                agnd = basePath & agnd
            End If

            Dim tokenChiamante As String = Nothing
            Dim Err_tokenChiamate As String = Nothing
            tokenOttieniDaHeaderHttp(tokenChiamante, Err_tokenChiamate)

            rval.message = "OK"
            rval.errori = ""
            rval.dettaglioEsito = New widgetManagerResponse_DettaglioEsito
            rval.dettaglioEsito.titoloWidget = "widget di prova"
            rval.dettaglioEsito.dettaglioRisposta = New List(Of widgetManagerResponse_DettaglioRisposta)

            'login con tunnel
            rval.dettaglioEsito.dettaglioRisposta.Add(
                New widgetManagerResponse_DettaglioRisposta With {
                    .idWidget = "SSOByPassGias",
                    .urlServizio = agnd.ToLower.Replace("/gestionerichieste.aspx", "/index.aspx") & "?token=" & tokenChiamante,
                    .titolo = "ACCEDI A GIAS",
                    .descrizione = "apri direttamente GIAS in una nuova scheda",
                    .descrizioneAggiuntiva = "senza ripetere il login accedi all’applicazione completa",
                    .urlImmagine = agnd.ToLower.Replace("/gestionerichieste.aspx", "/logoWidget.png"),
                    .tipoRender = "nuovaScheda",
                    .nascosto = False
                    })

            'DSS
            Dim rDir As String = CInt(TipiEnumerativi.enum_PagineAgenda_2010.DSSWidget)
            rDir = "pageRedir_" & rDir
            rDir = Sicurezza.Stringa_Codifica_LANCompatibile(rDir, AgroKey_EncoderDecoder)
            rval.dettaglioEsito.dettaglioRisposta.Add(
                New widgetManagerResponse_DettaglioRisposta With {
                    .idWidget = "DSSAgronica1",
                    .urlServizio = agnd.ToLower.Replace("/gestionerichieste.aspx", "/index.aspx") & "?token=" & tokenChiamante & "&rDir=" & rDir,
                    .titolo = "INDICATORI DSS DIFESA",
                    .descrizione = "riepilogo delle previsioni dei modelli di difesa",
                    .descrizioneAggiuntiva = "",
                    .urlImmagine = agnd.ToLower.Replace("/gestionerichieste.aspx", "/logoWidget.png"),
                    .tipoRender = "iframe",
                    .nascosto = False
                    })


            Return rval

        Catch ex As Exception
            Dim rval As New widgetManagerResponse
            rval.errori = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return rval
        End Try


    End Function

    Private Function GetCodiceUtenteColdirettiPersonaGiuridica(ByVal CUAA As String, ByVal coldirettiws_baseurl As String) As UtenteColdiretti
        Dim userCode As UtenteColdiretti = Nothing
        Try
            Dim urlWS As String = coldirettiws_baseurl & "/anagrafica/v1/anagrafica/persone-giuridiche/SGL_CODFIS/" & CUAA

            Dim hlpHttp As New Http

            Dim resp As String = hlpHttp.chiamaWS(Nothing, Nothing, urlWS, "application/json", "GET", "application/json", "", TimeOutRichiesta:=600)

            userCode = JsonConvert.DeserializeObject(Of UtenteColdiretti)(resp)
        Catch ex As Exception
            userCode = Nothing
            'do not throw exception for handling 404 return on not found CUAA
            'Throw New Exception(ex.Message, ex)
        End Try
        Return userCode

    End Function

    Private Function GetCodiceUtenteColdirettiPersonaFisica(ByVal CUAA As String, ByVal coldirettiws_baseurl As String) As UtenteColdiretti
        Dim userCode As UtenteColdiretti = Nothing
        Try
            Dim urlWS As String = coldirettiws_baseurl & "/anagrafica/v1/anagrafica/persone-fisiche/SGL_CODFIS/" & CUAA

            Dim hlpHttp As New Http

            Dim resp As String = hlpHttp.chiamaWS(Nothing, Nothing, urlWS, "application/json", "GET", "application/json", "", TimeOutRichiesta:=600)

            userCode = JsonConvert.DeserializeObject(Of UtenteColdiretti)(resp)
        Catch ex As Exception
            userCode = Nothing
            'do not throw exception for handling 404 return on not found CUAA
            'Throw New Exception(ex.Message, ex)
        End Try
        Return userCode

    End Function

    Private Function MapUserToAgronicaUtente(ByVal datiColdiretti As UtenteColdiretti,
                                                   ByVal username As String,
                                                   ByVal Piva As String,
                                                   ByVal profilo_default As String,
                                                   ByRef ObjParametriServer As AgronicaCoreParametri
                                                   ) As AgronicaCoreModelsSTD.profilazione.LeggiScriviVisibilitaUtente
        Dim result As AgronicaCoreModelsSTD.profilazione.LeggiScriviVisibilitaUtente = Nothing
        Dim ret As AgronicaCoreModelsSTD.profilazione.Utente = Nothing
        Try
            result = New AgronicaCoreModelsSTD.profilazione.LeggiScriviVisibilitaUtente
            result.Utente = New AgronicaCoreModelsSTD.profilazione.UtenteDTO()
            result.Utente.Tipologia = New AgronicaCoreModelsSTD.profilazione.TipologiaUtente() With {
                        .codice = profilo_default
                    }

            result.Utente.Piva_SuperUser = ObjParametriServer.PivaSuperUser
            If datiColdiretti.COD_TIPO_ANAG = "G" And datiColdiretti.data.SGL_CODFIS.Length = 16 Then
                result.Utente.UserName = username
                result.Utente.Nome = datiColdiretti.data.titolare.DES_NOME
                result.Utente.Cognome = datiColdiretti.data.titolare.DES_COGNOME
                result.Utente.Rag_Soc = ""
                result.Utente.codice_fiscale = datiColdiretti.data.SGL_CODFIS
                result.Utente.piva = ""
                result.Utente.flag_azienda_persona = 2
            Else
                result.Utente.UserName = username
                result.Utente.Nome = ""
                result.Utente.Cognome = ""
                result.Utente.Rag_Soc = datiColdiretti.data.DES_RAGIONE_SOCIALE
                result.Utente.codice_fiscale = datiColdiretti.data.SGL_CODFIS
                result.Utente.piva = ""
                result.Utente.flag_azienda_persona = 1
            End If

            result.Utente.username_commerciale = "UT.PDS.DEMETRA"
            result.Utente.Email = ""
            result.Utente.Password = ""

            result.AziendeVisibili = New List(Of ImpresaDto)
            result.AziendeVisibili.Add(New ImpresaDto() With {
                                        .piva = Piva,
                                        .Sa_Cod = 0,
                                        .Sa_Nome = ""
                                       })


        Catch ex As Exception
            result = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return result
    End Function
End Class
