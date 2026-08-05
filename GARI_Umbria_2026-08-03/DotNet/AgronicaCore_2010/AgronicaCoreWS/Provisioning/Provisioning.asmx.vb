Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Agronica.Helper.Retail
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Provisioning
Imports AgronicaCoreDTOStd.InData.Provisioning.Retail
Imports AgronicaCoreModelsSTD.provisioning
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
<ToolboxItem(False)>
Public Class Provisioning
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiClientValidation(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal xAppName As String,
                                        ByVal xAppVersion As String,
                                        ByVal xPlatform As String,
                                        ByVal xEnvironment As String
                                        ) As RispostaStandard

        Dim r As New RispostaStandard

        'Dim ClientValidation As New AgronicaCoreDTOStd.InData.Provisioning.ClientValidationResponse
        Dim msg As String = ""


        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objClientValidation As New AgronicaCoreVarieDAL.ClientValidation_R
            Dim dt As DataTable = objClientValidation.Leggi(xAppName, xAppVersion, xPlatform, xEnvironment, objParametri_Server)

            If dt.Rows.Count > 0 Then
                msg = dt.Rows(0).Item("Message")
            End If

            r.RispostaOK = True
            r.RispostaStringa = msg

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: "
        End Try
        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function DatiServer(ByVal InData As Object) As rispostaStandard(Of DatiServer)
        Dim objParametri = DeserializzaInData(Of DatiServerRequest)(InData)

        Dim resp As New rispostaStandard(Of DatiServer)

        Dim objBiz As New AgronicaCoreProvisioningBIZ.DatiServer_R

        Try

            Dim datiServerDT = objBiz.GetDatiServer(objParametri.InData.SceltaServer,
                                                      objParametri.Server,
                                                      objParametri.Utenti,
                                                      objParametri.Super_Server)
            If datiServerDT Is Nothing Then
                Throw New Exception("Errore nell'esecuzione della richiesta")
            End If

            Dim datiServerResp As New DatiServer
            datiServerResp.datiServer = New List(Of ServerData)

            For Each row In datiServerDT.Rows
                If Not IsDBNull(row("ID_DB")) And Not IsDBNull(row("Descrizione")) Then
                    Dim serv As New ServerData

                    serv.IDDb = row("ID_DB").ToString
                    serv.Descrizione = row("Descrizione").ToString

                    datiServerResp.datiServer.Add(serv)
                End If
            Next

            resp.RispostaOK = True
            resp.RispostaStringa = datiServerResp

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message
        End Try

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CreaNuovoToken(ByVal InData As Object) As RispostaStandard
        Dim objParametri = DeserializzaInData(Of CreaToken_In)(InData)

        Dim resp As New RispostaStandard
        Dim tokenPresente As String = ""

        Dim objUtentiWriteBiz As New AgronicaCoreUtentiBIZ.Utenti_Token_W
        Dim objUtentiReadBiz As New AgronicaCoreUtentiBIZ.Utenti_Token_R
        Dim objImpostazioniUtenteBiz As New AgronicaCoreImpostazioniUtenteBIZ.Impostazioni_Utente_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        If Not objParametri.Server.SuperUserUsername.Equals(objParametri.Server.UtenteUsername) Then
            resp.RispostaOK = True
            resp.RispostaStringa = "Occorre fare il login come super user."

            Return resp
        End If

        Try

            Dim verificaToken As New RispostaStandard

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri.Super_Server,
                                                                                    System.Data.IsolationLevel.ReadUncommitted
                                                                                    )

            verificaToken = objUtentiReadBiz.VerificaEsistenzaToken(objParametri.Server, objParametri.Super_Server)

            If Not verificaToken.Errore.Equals("") Then
                Return verificaToken
            End If

            tokenPresente = verificaToken.RispostaStringa

            If tokenPresente.Equals("") Then
                resp = objUtentiWriteBiz.SalvaNuovoToken(objParametri.InData.DB_Utenti,
                                                         objParametri.InData.DB_Server,
                                                         objParametri.Server,
                                                         objParametri.Utenti,
                                                         objParametri.Super_Server)

            Else
                resp.RispostaOK = True
                resp.RispostaStringa = String.Format("OK^{0}", tokenPresente)
            End If

            If resp.RispostaOK = True And resp.RispostaStringa.StartsWith("OK") Then

                resp = objImpostazioniUtenteBiz.SalvaTokenImpostazioniUtenti(resp.RispostaStringa.Split("^")(1),
                                                                             objParametri.Server,
                                                                             objParametri.Utenti,
                                                                             objParametri.Super_Server)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagConnessioneLocale, objParametri.Super_Server)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.Super_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Super_Server)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Super_Server)
        End Try

        If Not tokenPresente.Equals("") Then
            resp.RispostaStringa = String.Format("{0} {1}", resp.RispostaStringa, "Il token era già presente per l'utente desiderato ed è stato aggiornato.")
        End If

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaEsistenzaToken(ByVal InData As Object) As RispostaStandard
        Dim objParametri = DeserializzaInData(Of String)(InData)

        Dim resp As New RispostaStandard

        Dim objBiz As New AgronicaCoreUtentiBIZ.Utenti_Token_R

        Try

            resp = objBiz.VerificaEsistenzaToken(objParametri.Server,
                                                 objParametri.Super_Server)

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message
        End Try

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NuovoUtenteRetail(ByVal InData As Object) As RispostaStandard
        Dim objParametri = DeserializzaInData(Of Nuovo_Utente_Retail_In)(InData)

        Dim resp As New RispostaStandard
        Dim objUtentiTokenBiz As New AgronicaCoreUtentiBIZ.Utenti_Token_R
        Dim objUtenti As New AgronicaCoreUtentiBIZ.Utenti
        Dim objSequenze As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Dim inDataJObject As JObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(InData))
            Dim token As String = inDataJObject.SelectToken("InData.Token").ToString

            Dim objSicurezza As New AgronicaCoreDataProvider.Sicurezza

            PopolaObjParametri(Of Nuovo_Utente_Retail_In)(token, objParametri.InData.CodiceISOLingua, objParametri)
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(objParametri.InData.CodiceISOLingua)

            If objParametri.InData.Utente.PIVA_Azienda.Equals("") Then
                objParametri.InData.Utente.PIVA_Azienda =
                    String.Format("F{0}", Math.Abs(objSequenze.NuovoId_Tabella("impresa", -2000000000, 0, objParametri.Server, True)).ToString)
            End If

            If objParametri.InData.Utente.CF_Azienda.Equals("") And Not objParametri.InData.Utente.CUAA.Equals("") Then
                objParametri.InData.Utente.CF_Azienda = objParametri.InData.Utente.CUAA
            End If

            Dim utentiDT = objUtenti.LeggiUtentiDaUsername(objParametri.InData.Utente.Email, objParametri.Utenti)

            If utentiDT IsNot Nothing Then
                If utentiDT.Rows.Count > 1 Then
                    Throw New Exception("INVALID_USER")
                End If

                If utentiDT.Rows.Count = 0 Then

                    Dim duplicato = objUtenti.VerificaEsistenzaUtente(objParametri.InData.Utente.Email,
                                                                      objParametri.InData.Utente.CF_Azienda,
                                                                      objParametri.InData.Utente.PIVA_Azienda,
                                                                      objParametri.Utenti)

                    If Not duplicato.Equals("OK") Then

                        Dim causa As String = ""

                        Select Case duplicato
                            Case "EMAIL"
                                causa = Gias.EmailInUso
                            Case "CF"
                                causa = Gias.CodiceFiscaleInUso
                            Case "PIVA"
                                causa = Gias.PivaInUso
                        End Select

                        Throw New Exception(causa)
                    End If
                End If

                If utentiDT.Rows.Count = 1 Then

                    objParametri.Server.UtenteUsername = utentiDT.Rows(0)("UserName")
                    objParametri.Utenti.UtenteUsername = utentiDT.Rows(0)("UserName")

                    Dim password = utentiDT.Rows(0)("Password")

                    If Not Convert.ToBoolean(utentiDT.Rows(0)("Flag_Encrypted")) Then
                        password = objSicurezza.GeneraNuovoHash(password)
                    End If

                    If Not password.Equals(objSicurezza.GeneraHashConfronto(password, objParametri.InData.Utente.Password)) Then
                        Throw New Exception("INVALID_USER")
                    End If
                End If
            End If

            objParametri.InData.Utente.Password = objSicurezza.GeneraNuovoHash(objParametri.InData.Utente.Password)

            Dim token_valido = objUtentiTokenBiz.VerificaValiditaToken(objParametri.Super_Server)

            If Not token_valido Then
                Throw New Exception("INVALID_TOKEN")
            End If

            Dim impresaEsistente As Boolean = False

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri.Server)

            impresaEsistente = VerificaEsistenzaImpresa(objParametri.InData.Utente, objParametri.Server)

            If impresaEsistente And Not objParametri.InData.ForzaRegistrazione Then

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Server)

                resp.RispostaOK = True
                resp.RispostaStringa = Gias.ImpresaEsistente

                Return resp
            End If

            If impresaEsistente And objParametri.InData.ForzaRegistrazione Then

                objParametri.InData.Utente.PIVA_Azienda =
                    String.Format("F{0}", Math.Abs(objSequenze.NuovoId_Tabella("impresa", -2000000000, 0, objParametri.Server, True)).ToString)

                objParametri.InData.Utente.CUAA = ""
            End If

            Dim codiceAttivazione = generaCodiceAttivazione(6)

            Dim xWrite As New AgronicaCoreUtentiRetailBIZ.Utenti_Retail_W

            resp = xWrite.Scrivi(codiceAttivazione, objParametri.InData, objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreTabellaTamponeUtentiRetail)
            End If

            resp = AccodaNotifica(objParametri.InData.Utente, codiceAttivazione, objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreAccodamentoNotifica)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Server)

            resp.RispostaStringa = Gias.OperazioneSuccesso

        Catch ex As Exception

            If objParametri.Server IsNot Nothing AndAlso objParametri.Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Server)
        End Try

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RinnovaUtente(ByVal InData As Object) As RispostaStandard
        Dim objParametri = DeserializzaInData(Of Rinnova_Utente_Retail_In)(InData)

        Dim resp As New RispostaStandard

        Dim FlagTransazioneLocale_S As Boolean = False
        Dim FlagConnessioneLocale_S As Boolean = False
        Dim FlagTransazioneLocale_U As Boolean = False
        Dim FlagConnessioneLocale_U As Boolean = False

        Dim objUtentiTokenBiz As New AgronicaCoreUtentiBIZ.Utenti_Token_R
        Dim objUtenti As New AgronicaCoreUtentiBIZ.Utenti
        Dim objSicurezza As New AgronicaCoreDataProvider.Sicurezza

        Try

            Dim inDataJObject As JObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(InData))
            Dim token As String = inDataJObject.SelectToken("InData.Token").ToString

            PopolaObjParametri(Of Rinnova_Utente_Retail_In)(token, objParametri.InData.CodiceISOLingua, objParametri)
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(objParametri.InData.CodiceISOLingua)

            Dim token_valido = objUtentiTokenBiz.VerificaValiditaToken(objParametri.Super_Server)

            If Not token_valido Then
                Throw New Exception("INVALID_TOKEN")
            End If

            Dim utentiDT = objUtenti.LeggiUtenteDaEmail(objParametri.InData.EmailUtente, objParametri.Utenti)

            If utentiDT IsNot Nothing Then
                If utentiDT.Rows.Count <> 1 Then
                    Throw New Exception("INVALID_USER")
                End If

                If utentiDT.Rows.Count = 1 Then

                    objParametri.Server.UtenteUsername = utentiDT.Rows(0)("UserName")
                    objParametri.Utenti.UtenteUsername = utentiDT.Rows(0)("UserName")

                    Dim password = utentiDT.Rows(0)("Password")

                    If Not Convert.ToBoolean(utentiDT.Rows(0)("Flag_Encrypted")) Then
                        password = objSicurezza.GeneraNuovoHash(password)
                    End If

                    If Not password.Equals(objSicurezza.GeneraHashConfronto(password, objParametri.InData.PasswordUtente)) Then
                        Throw New Exception("INVALID_USER")
                    End If
                End If
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_S,
                                                                                    FlagTransazioneLocale_S,
                                                                                    objParametri.Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_U,
                                                                                    FlagTransazioneLocale_U,
                                                                                    objParametri.Utenti)

            Dim profilatore As New ProfilatoreUtenze

            Dim ServizioNuovaRegistrazione = profilatore.ProfilaUtenteDaServizio(Convert.ToInt32(objParametri.InData.transazioneCommerciale.Codice_Servizio))
            Dim ServizioVecchiaRegistrazione = Convert.ToInt32(utentiDT.Rows(0)("Tipologia_Cod"))

            Dim objUtente As New JObject
            objUtente.Add("UserName", objParametri.InData.EmailUtente)
            objUtente.Add("Data_Fine", objParametri.InData.dataFineValiditaUtente.ToString("dd/MM/yyyy"))
            objUtente.Add("Tipologia_Cod", ServizioNuovaRegistrazione)

            resp.RispostaOK = objUtenti.ImpostaPermessi_Retail(objUtente,
                                                               CostantiPersonalizzate.Id_Servizio_GiasOnline,
                                                               objParametri.Server,
                                                               objParametri.Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreAggiornamentoPermessiUtenteRetail)
            End If

            'Check escalation del servizio
            If ServizioNuovaRegistrazione <> ServizioVecchiaRegistrazione Then

                Dim xReadImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
                Dim xWriteImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_W

                Dim dtImpostazioniPerUpdate As DataTable =
                    xReadImpostazioni.leggiImpostazioniDaCodiceTipologia(1,
                                                                         ServizioNuovaRegistrazione,
                                                                         0,
                                                                         " Impostazione_Valore_4 = 'update' ",
                                                                         "",
                                                                         objParametri.Utenti)


                For Each drImposta As DataRow In dtImpostazioniPerUpdate.Rows
                    xWriteImpostazioni.ScriviModifica_Impostazione(
                        CInt(drImposta("Impostazione_cod")),
                        drImposta("Impostazione_Valore_1").ToString,
                        drImposta("Impostazione_Valore_2").ToString,
                        drImposta("Impostazione_Valore_3").ToString,
                        "",
                        CDate(drImposta("Validita_Inizio")),
                        CDate(drImposta("Validita_Fine")),
                        objParametri.Utenti
                    )
                Next
            End If

            resp.RispostaOK = RegistraTransazioneCommerciale(objParametri.InData.transazioneCommerciale,
                                                             objParametri.Server,
                                                             objParametri.Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreSalvataggioTransazioneCommerciale)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_S, objParametri.Server)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_U, objParametri.Utenti)

            resp.RispostaStringa = Gias.OperazioneSuccesso

        Catch ex As Exception

            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            If Not objParametri.Utenti.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Utenti)
            End If

            resp.RispostaOK = False
            resp.RispostaStringa = ""
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_S, objParametri.Server)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_U, objParametri.Utenti)
        End Try

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RiportaNuovoUtente(ByVal InData As Object) As RispostaStandard
        Dim objParametri = DeserializzaInData(Of Riporta_Utente_Retail_In)(InData)

        Dim resp As New RispostaStandard

        Dim objBiz As New AgronicaCoreUtentiRetailBIZ.Utenti_Retail_R
        Dim objUtentiTokenBiz As New AgronicaCoreUtentiBIZ.Utenti_Token_R

        Dim FlagTransazioneLocale_S As Boolean = False
        Dim FlagConnessioneLocale_S As Boolean = False
        Dim FlagTransazioneLocale_U As Boolean = False
        Dim FlagConnessioneLocale_U As Boolean = False

        Try

            Dim inDataJObject As JObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(InData))
            Dim token As String = inDataJObject.SelectToken("InData.Token").ToString

            PopolaObjParametri(Of Riporta_Utente_Retail_In)(token, objParametri.InData.CodiceISOLingua, objParametri)
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(objParametri.InData.CodiceISOLingua)

            Dim token_valido = objUtentiTokenBiz.VerificaValiditaToken(objParametri.Super_Server)

            If Not token_valido Then
                Throw New Exception("INVALID_TOKEN")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_S,
                                                                                    FlagTransazioneLocale_S,
                                                                                    objParametri.Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_U,
                                                                                    FlagTransazioneLocale_U,
                                                                                    objParametri.Utenti)

            Dim objDaRiportare = objBiz.LeggiDaRiportare(objParametri.InData.codiceAttivazione, objParametri.Server)

            Dim objUtenteBIZ As New AgronicaCoreUtentiBIZ.Utenti

            Dim utentiDT = objUtenteBIZ.LeggiUtentiDaUsername(objDaRiportare.Utente.Email, objParametri.Utenti)

            If utentiDT IsNot Nothing AndAlso utentiDT.Rows.Count > 1 Then
                Throw New Exception(Gias.ErroreInformazioniUtente)
            End If

            Dim profilatore As New ProfilatoreUtenze

            Dim objUtente As New JObject
            objUtente.Add("UserName", objDaRiportare.Utente.Email)
            objUtente.Add("Cognome", objDaRiportare.Utente.Cognome)
            objUtente.Add("Nome", objDaRiportare.Utente.Nome)
            objUtente.Add("Password", objDaRiportare.Utente.Password)
            objUtente.Add("Tel", objDaRiportare.Utente.Telefono)
            objUtente.Add("Email", objDaRiportare.Utente.Email)
            objUtente.Add("CodFisc", objDaRiportare.Utente.CF_Azienda)
            objUtente.Add("Rag_Soc", objDaRiportare.Utente.Ragione_Sociale_Azienda)
            objUtente.Add("PIVA", objDaRiportare.Utente.PIVA_Azienda)
            objUtente.Add("Data_Fine", objDaRiportare.dataFineValiditaUtente.ToString("dd/MM/yyyy"))
            objUtente.Add("UserNameCommerciale", "")
            objUtente.Add("Tipologia_Cod", profilatore.ProfilaUtenteDaServizio(Convert.ToInt32(objDaRiportare.transazioneCommerciale.Codice_Servizio)))

            If utentiDT Is Nothing OrElse utentiDT.Rows.Count = 0 Then

                Dim emailOK = objUtenteBIZ.VerificaEsistenzaUtente(objDaRiportare.Utente.Email, "", "", objParametri.Utenti)

                If Not emailOK.Equals("OK") Then
                    Throw New Exception(Gias.EmailInUso)
                End If

                resp = objUtenteBIZ.Scrivi_Utente_Retail(objUtente, objParametri.Utenti)

                If Not resp.RispostaOK Then
                    Throw New Exception(Gias.ErroreSalvataggioUtenteRetail)
                End If
            End If

            resp.RispostaOK = objUtenteBIZ.ImpostaPermessi_Retail(objUtente,
                                                                  CostantiPersonalizzate.Id_Servizio_GiasOnline,
                                                                  objParametri.Server,
                                                                  objParametri.Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreAggiornamentoPermessiUtenteRetail)
            End If

            Dim objImpresa = PopolaImpresaDaTampone(objDaRiportare)

            Dim objImpresaBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_W

            objImpresaBIZ.Scrivi_Impresa_APP(objImpresa,
                                             TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                             objParametri.Server,
                                             objParametri.Utenti,
                                             "",
                                             False,
                                             "Impresa registrata da Retail",
                                             False)

            resp.RispostaOK = RegistraTransazioneCommerciale(objDaRiportare.transazioneCommerciale,
                                                             objParametri.Server,
                                                             objParametri.Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreSalvataggioTransazioneCommerciale)
            End If

            Dim objUtenteRetailBIZ As New AgronicaCoreUtentiRetailBIZ.Utenti_Retail_W

            resp.RispostaOK = objUtenteRetailBIZ.CancellaLogicamenteUtenteCreato(objParametri.InData.codiceAttivazione, objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreCancellazioneLogicaUtenteRetail)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_S, objParametri.Server)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_U, objParametri.Utenti)

            resp.RispostaStringa = Gias.OperazioneSuccesso

        Catch ex As Exception

            If Not objParametri.Server Is Nothing AndAlso Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            If Not objParametri.Utenti Is Nothing AndAlso Not objParametri.Utenti.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Utenti)
            End If

            resp.RispostaOK = False
            resp.RispostaStringa = ""
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_S, objParametri.Server)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_U, objParametri.Utenti)
        End Try


        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CambiaEmailUtenteRetail(ByVal InData As Object) As RispostaStandard

        Dim resp As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of NuovaEmail_In)(InData)

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim objUtentiBIZ As New AgronicaCoreUtentiBIZ.Utenti
        Dim objUtentiRetailBIZ As New AgronicaCoreUtentiRetailBIZ.Utenti_Retail_W

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri.Utenti)

            If objParametri.InData.nuovaEmail.Equals("") Then
                Throw New Exception(Gias.EmailNonValida)
            End If

            Dim DT = objUtentiBIZ.LeggiUtentiDettagliDaUserName(objParametri.Utenti.UtenteUsername, objParametri.Utenti)

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                Throw New Exception(Gias.UtenteNonTrovato)
            End If

            If DT.Rows(0)("Email").ToString.ToLower.Equals(objParametri.InData.nuovaEmail.Trim.ToLower) Then
                Throw New Exception(Gias.EmailInUso)
            End If

            If objUtentiBIZ.VerificaEsistenzaUtente(objParametri.InData.nuovaEmail.Trim, "", "", objParametri.Utenti) Then
                Throw New Exception(Gias.EmailInUso)
            End If

            Dim azienda_persona As String = ""
            If CInt(DT.Rows(0)("Flag_Azienda_Persona")) = 1 Then
                azienda_persona = "I"
            Else
                azienda_persona = "P"
            End If

            Dim utente As New AgronicaCoreModelsSTD.profilazione.Utente With {
                .UserName = objParametri.Utenti.UtenteUsername,
                .Cognome = DT.Rows(0)("Cognome").ToString,
                .Nome = DT.Rows(0)("Nome").ToString,
                .Tel = DT.Rows(0)("Tel").ToString,
                .Email = objParametri.InData.nuovaEmail,
                .PIVA = DT.Rows(0)("PIVA").ToString,
                .CodFisc = DT.Rows(0)("CodFisc").ToString,
                .UserNameCommerciale = DT.Rows(0)("UserNameCommerciale").ToString,
                .Azienda_Persona = azienda_persona
            }

            resp.RispostaOK = objUtentiBIZ.Modifica_Email_Utente_Retail(utente, objParametri.Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreCambioEmail)
            End If

            resp.RispostaOK = objUtentiRetailBIZ.LoggaCambioMail(DT.Rows(0)("Email").ToString,
                                                                 objParametri.InData.nuovaEmail,
                                                                 objParametri.Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception(Gias.ErroreLOGCambioEmail)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Utenti)

            resp.RispostaStringa = Gias.OperazioneSuccesso

        Catch ex As Exception
            If Not objParametri.Utenti.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Utenti)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Utenti)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CreaTokenJWT(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of CreaTokenJWT_In)(InData)
        Dim objTokenJWT = objParametri.InData

        Dim resp As New RispostaStandard

        Dim objUtentiTokenJWT_R As New AgronicaCoreUtentiBIZ.Utenti_TokenJWT_R
        Dim objUtentiTokenJWT_W As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'elimino tutti i token scaduti
            objUtentiTokenJWT_W.EliminaTokenScaduti(objParametri.Super_Server)

            'creazione nuovo token
            objUtentiTokenJWT_W.Scrivi(objTokenJWT.idToken,
                                         objTokenJWT.objP_super_server,
                                         objTokenJWT.objP_server,
                                         objTokenJWT.objP_utenti,
                                         objTokenJWT.codiceFiscale,
                                         objTokenJWT.coreWSBaseURL,
                                         objTokenJWT.username,
                                         objTokenJWT.pivaSuperUser,
                                         objTokenJWT.versioneApp,
                                         objTokenJWT.refreshToken,
                                         objTokenJWT.dataCreazione.ToUniversalTime(),
                                         objTokenJWT.dataFineValidita.ToUniversalTime(),
                                         objTokenJWT.IdDB,
                                         objParametri.Super_Server)

            resp.RispostaStringa = objTokenJWT.refreshToken 'String.Format("{0} {1}", objTokenJWT.refreshToken, " - Il token è stato creato correttamente.")
            resp.RispostaOK = True

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message
        End Try

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiTokenJWT(ByVal InData As Object) As rispostaStandard(Of CreaTokenJWT_In)

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim refreshToken As String = objParametri.InData

        Dim resp As New rispostaStandard(Of CreaTokenJWT_In)

        Dim objUtentiTokenJWT_R As New AgronicaCoreUtentiBIZ.Utenti_TokenJWT_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri.Super_Server,
                                                                                    System.Data.IsolationLevel.ReadUncommitted)

            Dim objTokenJWT As New CreaTokenJWT_In
            Dim dtTokenJWT = objUtentiTokenJWT_R.LeggiConRefreshToken(refreshToken, objParametri.Super_Server.PivaSuperUser, objParametri.Super_Server.SuperUserUsername, objParametri.Super_Server)

            If Not IsNothing(dtTokenJWT) AndAlso dtTokenJWT.Rows.Count = 1 Then
                objTokenJWT.idToken = dtTokenJWT(0)("idToken")
                objTokenJWT.objP_super_server = dtTokenJWT(0)("objP_SuperServer")
                objTokenJWT.objP_server = dtTokenJWT(0)("objP_Server")
                objTokenJWT.objP_utenti = dtTokenJWT(0)("objP_Utenti")
                objTokenJWT.codiceFiscale = dtTokenJWT(0)("Codice_Fiscale")
                objTokenJWT.coreWSBaseURL = dtTokenJWT(0)("CoreWSBaseURL")
                objTokenJWT.username = dtTokenJWT(0)("Username")
                objTokenJWT.pivaSuperUser = dtTokenJWT(0)("PivaSuperUser")
                objTokenJWT.versioneApp = dtTokenJWT(0)("VersioneApp")
                objTokenJWT.refreshToken = dtTokenJWT(0)("refreshToken")
                objTokenJWT.dataCreazione = dtTokenJWT(0)("Data_Creazione")
                objTokenJWT.dataFineValidita = dtTokenJWT(0)("Validita_Fine")
                objTokenJWT.IdDB = dtTokenJWT(0)("IdDB")

                resp.RispostaStringa = objTokenJWT
                resp.RispostaOK = True

            ElseIf Not IsNothing(dtTokenJWT) AndAlso dtTokenJWT.Rows.Count > 1 Then
                resp.RispostaStringa = Nothing
                resp.RispostaOK = False
                resp.Errore = String.Format("{0} {1}", objTokenJWT.idToken, " - Token presente più volte.")
            Else
                resp.RispostaStringa = Nothing
                resp.RispostaOK = False
                resp.Errore = String.Format("{0} {1}", objTokenJWT.idToken, " - Token non presente.")
            End If

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.Super_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Super_Server)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Super_Server)
        End Try

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RefreshTokenJWT(ByVal InData As Object) As rispostaStandard(Of CreaTokenJWT_In)

        Dim objParametri = DeserializzaInData(Of RefreshTokenJWT_In)(InData)
        Dim objTokenJWT = objParametri.InData

        Dim resp As New rispostaStandard(Of CreaTokenJWT_In)

        Dim objUtentiTokenJWT_R As New AgronicaCoreUtentiBIZ.Utenti_TokenJWT_R
        Dim objUtentiTokenJWT_W As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale,
                                                                                    objParametri.Super_Server, System.Data.IsolationLevel.ReadUncommitted)

            'controllo esistenza vecchio token
            Dim dtTokenJWT = objUtentiTokenJWT_R.LeggiConRefreshToken(objTokenJWT.oldRefreshToken, objParametri.Super_Server.PivaSuperUser, objTokenJWT.username, objParametri.Super_Server)

            If Not IsNothing(dtTokenJWT) AndAlso dtTokenJWT.Rows.Count = 1 Then
                Dim objNewTokenJWT As New CreaTokenJWT_In
                Dim rowTokenOld = dtTokenJWT(0)

                objNewTokenJWT.objP_super_server = objTokenJWT.objP_super_server
                objNewTokenJWT.objP_server = rowTokenOld("objP_Server")
                objNewTokenJWT.objP_utenti = rowTokenOld("objP_Utenti")
                objNewTokenJWT.codiceFiscale = rowTokenOld("Codice_Fiscale")
                objNewTokenJWT.coreWSBaseURL = objTokenJWT.coreWsBaseUrl
                objNewTokenJWT.username = rowTokenOld("Username")
                objNewTokenJWT.pivaSuperUser = rowTokenOld("PivaSuperUser")
                objNewTokenJWT.versioneApp = rowTokenOld("VersioneApp")
                objNewTokenJWT.IdDB = CInt(rowTokenOld("IdDB"))

                'creazione nuovo token
                Dim respW = objUtentiTokenJWT_W.Scrivi(objTokenJWT.newTokenID,
                                                 objNewTokenJWT.objP_super_server,
                                                 objNewTokenJWT.objP_server,
                                                 objNewTokenJWT.objP_utenti,
                                                 objNewTokenJWT.codiceFiscale,
                                                 objTokenJWT.coreWsBaseUrl,
                                                 objNewTokenJWT.username,
                                                 objNewTokenJWT.pivaSuperUser,
                                                 objNewTokenJWT.versioneApp,
                                                 objTokenJWT.newRefreshToken,
                                                 objTokenJWT.dataCreazione,
                                                 objTokenJWT.dataFineValidita,
                                                 objNewTokenJWT.IdDB,
                                                 objParametri.Super_Server)
                If respW Then
                    objNewTokenJWT.idToken = objTokenJWT.newTokenID
                    objNewTokenJWT.refreshToken = objTokenJWT.newRefreshToken
                    objNewTokenJWT.dataCreazione = objTokenJWT.dataCreazione
                    objNewTokenJWT.dataFineValidita = objTokenJWT.dataFineValidita

                    resp.RispostaStringa = objNewTokenJWT
                    resp.RispostaOK = True
                End If

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Super_Server)

            Else
                Throw New Exception("Vecchio token non presente.")
            End If

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.Super_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Super_Server)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Super_Server)
        End Try

        Return resp

    End Function

    Private Function CreaTokenJWT_InFromDTRow(row As DataRow) As CreaTokenJWT_In
        Dim resp As New CreaTokenJWT_In
        resp.codiceFiscale = row("Codice_Fiscale")
        resp.coreWSBaseURL = row("CoreWSBaseURL")
        resp.dataCreazione = row("Data_Creazione")
        resp.dataFineValidita = row("Validita_Fine")
        resp.idToken = row("idToken")
        resp.objP_server = row("objP_Server")
        resp.objP_super_server = row("objP_SuperServer")
        resp.objP_utenti = row("objP_Utenti")
        resp.pivaSuperUser = row("PivaSuperUser")
        resp.refreshToken = row("refreshToken")
        resp.username = row("Username")
        resp.versioneApp = row("VersioneApp")
        resp.IdDB = row("IdDB")

        Return resp
    End Function

    Private Function RegistraTransazioneCommerciale(ByVal transazioneCommerciale As Dati_Transazione_Commerciale,
                                                    ByRef objParametri_server As AgronicaCoreParametri,
                                                    ByRef objParametri_utenti As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim objTransazioneCommercialeBIZ As New AgronicaCoreUtentiBIZ.Utenti_InfoTransazioneComm_W

        If transazioneCommerciale.Codice_Transazione.Equals("") Then
            Dim objSequenze As New Agro_Sequenze

            Dim nuovoIdTransazione = objSequenze.NuovoId_Tabella("TransazioneCommerciale", 0, Int32.MaxValue, objParametri_server, True)

            transazioneCommerciale.Codice_Transazione =
                    String.Format("{0}_{1}", CostantiPersonalizzate.Retail_NuovoIdDaAppPrefisso, nuovoIdTransazione.ToString)
        End If

        resp = objTransazioneCommercialeBIZ.ScriviDaObj(transazioneCommerciale,
                                                        objParametri_server,
                                                        objParametri_utenti)

        Return resp
    End Function

    Private Function PopolaImpresaDaTampone(objDaRiportare As Nuovo_Utente_Retail_In) As AgronicaCoreModelsSTD.anagrafiche.Impresa
        Dim objImpresa As New AgronicaCoreModelsSTD.anagrafiche.Impresa

        objImpresa.partitaIva = objDaRiportare.Utente.PIVA_Azienda
        objImpresa.ragioneSociale = objDaRiportare.Utente.Ragione_Sociale_Azienda
        objImpresa.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE)
        objImpresa.tipo_Impresa = TipiEnumerativi.enum_TipoImpresaGerarchia.Impresa
        objImpresa.CUAA = objDaRiportare.Utente.CUAA

        objImpresa.codici = New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

        Dim datiImpresa As New JObject
        datiImpresa.Add("telephone", objDaRiportare.Utente.Telefono)

        objImpresa.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori With {.codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe With {.codice = TipiEnumerativi.enum_CodiciAnagrafe.CodiceCUAA}, .valore = objDaRiportare.Utente.CUAA})
        objImpresa.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori With {.codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe With {.codice = TipiEnumerativi.enum_CodiciAnagrafe.GiasAPP_Dati_Impresa}, .valore = datiImpresa.ToString})
        objImpresa.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori With {.codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe With {.codice = TipiEnumerativi.enum_CodiciAnagrafe.SDI}, .valore = objDaRiportare.transazioneCommerciale.Codice_SDI})
        objImpresa.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori With {.codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe With {.codice = TipiEnumerativi.enum_CodiciAnagrafe.PEC}, .valore = objDaRiportare.transazioneCommerciale.PEC})

        Return objImpresa
    End Function

    Private Function AccodaNotifica(ByVal utente As Dati_Utente_Retail,
                                    ByVal codiceAttivazione As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri
                                    ) As RispostaStandard

        Dim resp As New RispostaStandard
        resp.RispostaOK = False

        If Not utente.Telefono.Equals("") Then
            Dim xAccoda As New AgronicaCoreMailBIZ.SMS_Programmazione_W

            resp = xAccoda.AccodaPerInvio(objParametri_Server,
                                              1,
                                              String.Format("{0}_{1}", codiceAttivazione, utente.Telefono),
                                              CostantiPersonalizzate.Retail_MittenteSMS,
                                              utente.Telefono,
                                              CostantiPersonalizzate.Retail_testoSMS,
                                              Date.Now,
                                              Date.Now,
                                              Date.Now,
                                              objParametri_Server.UsernameOperazione,
                                              objParametri_Server.UsernameOperazione
                                              )

            Return resp

        ElseIf Not utente.Email.Equals("") Then

            Dim emailBody = System.IO.File.ReadAllText(CostantiPersonalizzate.Retail_testoEMAILFileLocation)

            Dim xRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim DT = xRead.Leggi(0, CostantiPersonalizzate.Retail_ChiaveMittenteEMAIL, "", "", objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                resp.Errore = "Errore nel recupero delle informazioni del mittente Email"
                Return resp
            End If

            Dim mittenteEmail As String = DT.Rows(0)("Valore").ToString

            Dim xAccoda As New AgronicaCoreMailBIZ.Mail_Programmazione_W

            resp.RispostaOK = xAccoda.ScriviNuovaMail(objParametri_Server,
                                                          6,
                                                          String.Format("{0}_{1}", codiceAttivazione, utente.Email),
                                                          mittenteEmail,
                                                          utente.Email,
                                                          "",
                                                          "",
                                                          CostantiPersonalizzate.Retail_OggettoEMAIL,
                                                          emailBody,
                                                          True,
                                                          "",
                                                          Date.Now,
                                                          Date.Now,
                                                          Date.Now,
                                                          objParametri_Server.UsernameOperazione,
                                                          objParametri_Server.UsernameOperazione)

            Return resp
        Else
            resp.Errore = "Almeno uno dei due valori tra indirizzo email e numero di telefono deve essere valorizzato."
            Return resp
        End If

    End Function

    Private Function generaCodiceAttivazione(ByVal lunghezza As Int32) As String

        Dim nuovoCodice As New System.Text.StringBuilder
        Dim tmpId = Guid.NewGuid
        'Dim b64 = Convert.ToBase64String(tmpId.ToByteArray)

        While nuovoCodice.Length < lunghezza

            For Each c In tmpId.ToString
                If Char.IsDigit(c) Then
                    nuovoCodice.Append(c)
                    If nuovoCodice.Length = lunghezza Then Exit For
                End If
            Next

        End While

        Return nuovoCodice.ToString

    End Function

    Private Function VerificaEsistenzaImpresa(ByVal utente As Dati_Utente_Retail,
                                              ByRef objParametri_server As AgronicaCoreParametri
                                              ) As Boolean

        Dim xReadImpresa As New AgronicaCoreAnagrafeBIZ.Impresa_R

        If utente.PIVA_Azienda.Equals("") And utente.CUAA.Equals("") Then
            Throw New Exception("Va specificata una Partita iva oppure un codice CUAA per l'impresa che si desidera registrare.")
        End If

        If Not utente.PIVA_Azienda.Equals("") Then

            Return xReadImpresa.VerificaEsistenzaImpresaByPiva(utente.PIVA_Azienda, objParametri_server)
        End If

        '
        'If Not utente.CF_Azienda.Equals("") Then
        '
        'Return xReadImpresa.VerificaEsistenzaImpresaByPivaCF(utente.CF_Azienda, objParametri_server)
        'End If

        If Not utente.CUAA.Equals("") Then

            Return xReadImpresa.VerificaEsistenzaImpresaByCUAA(utente.CUAA, objParametri_server)
        End If

        Return False

    End Function

    Private Sub PopolaObjParametri(Of T)(ByVal token As String, ByVal CodiceISOLingua As String, ByRef objParametri As ObjParametri(Of T))

        Dim datiUtentiDaToken As DataTable

        Dim objUtentiBiz As New AgronicaCoreUtentiBIZ.Utenti_Token_R
        Dim objServerBiz As New AgronicaCoreProvisioningBIZ.DatiServer_R
        Dim objLinguaBiz As New AgronicaCoreUtentiBIZ.Utenti

        Dim init As New AgronicaCoreGestioneRichieste.Inizializzatore

        Dim connessioneSuperServer As String = init.Crea_Stringa_Connessione_Super_Server_Da_Config_GiasOnLine("")

        objParametri.Super_Server = New AgronicaCoreParametri With {.StringaConnessione = connessioneSuperServer}

        datiUtentiDaToken = objUtentiBiz.LeggiDatiUtenteDaToken(token, objParametri.Super_Server)

        If datiUtentiDaToken Is Nothing OrElse datiUtentiDaToken.Rows.Count = 0 Then
            Throw New Exception("INVALID_TOKEN")
        End If

        objParametri.Super_Server.PivaSuperUser = datiUtentiDaToken.Rows(0)("PivaSuperUser").ToString
        objParametri.Super_Server.UtenteUsername = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString
        objParametri.Super_Server.SuperUserUsername = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString
        objParametri.Super_Server.UsernameOperazione = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString

        Dim datiServer As String = datiUtentiDaToken.Rows(0)("Parametri").ToString

        Dim datiServerObj = JObject.Parse(datiServer)

        Dim dbServerData = objServerBiz.GetConnessioneDB(Convert.ToInt32(datiServerObj("idDB_Server")), objParametri.Super_Server)

        objParametri.Server = New AgronicaCoreParametri With {
                .StringaConnessione = CreaStringaConnessione(dbServerData.Rows(0)),
                .PivaSuperUser = datiUtentiDaToken.Rows(0)("PivaSuperUser").ToString,
                .UtenteUsername = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString,
                .SuperUserUsername = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString,
                .UsernameOperazione = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString
            }

        dbServerData = objServerBiz.GetConnessioneDB(Convert.ToInt32(datiServerObj("idDB_Utenti")), objParametri.Super_Server)

        objParametri.Utenti = New AgronicaCoreParametri With {
                .StringaConnessione = CreaStringaConnessione(dbServerData.Rows(0)),
                .PivaSuperUser = datiUtentiDaToken.Rows(0)("PivaSuperUser").ToString,
                .UtenteUsername = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString,
                .SuperUserUsername = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString,
                .UsernameOperazione = datiUtentiDaToken.Rows(0)("SuperUser_Username").ToString
            }

        Dim DTLingue = objLinguaBiz.LeggiLingua(CodiceISOLingua, objParametri.Utenti)

        Dim codiceLingua As Int32 = 1

        If DTLingue IsNot Nothing AndAlso DTLingue.Rows.Count > 0 Then
            codiceLingua = CInt(DTLingue.Rows(0)("Lingua_Cod"))
        End If

        objParametri.Super_Server.Lingua_Cod = codiceLingua
        objParametri.Server.Lingua_Cod = codiceLingua
        objParametri.Utenti.Lingua_Cod = codiceLingua
    End Sub

    Private Function CreaStringaConnessione(ByVal dataRow As DataRow) As Object

        Return String.Format("Provider={0};Server={1};Initial Catalog={2};User Id={3};Password={4};",
                             dataRow("Provider").ToString,
                             dataRow("Server").ToString,
                             dataRow("DB").ToString,
                             dataRow("UserId").ToString,
                             dataRow("Password").ToString)

    End Function

    Private Class ObjParametri(Of T)

        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T

    End Class

    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        If objInData.objP.objP_super_server IsNot Nothing AndAlso Not objInData.objP.objP_super_server.Equals("") Then
            objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        End If
        If objInData.objP.objP_server IsNot Nothing AndAlso Not objInData.objP.objP_server.Equals("") Then
            objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        End If
        If objInData.objP.objP_utenti IsNot Nothing AndAlso Not objInData.objP.objP_utenti.Equals("") Then
            objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)
        End If

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

End Class