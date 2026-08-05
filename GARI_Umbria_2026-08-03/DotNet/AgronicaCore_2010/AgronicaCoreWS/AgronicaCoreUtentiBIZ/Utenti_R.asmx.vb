Imports System.Web.Services
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreModelsSTD.provisioning
Imports AgronicaCoreUtentiBIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Utenti_R
    Inherits WebService

    Private Function GetObjParams(Of T)(inData As CoreWS_Generic(Of T)) As ObjParams
        Return New ObjParams With {
            .ObjParametri_SuperServer = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_super_server),
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_server),
            .ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_utenti)
        }
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Autenticazione(Username As String, Password As String, DatiConnessioneSelezionata As String) As rispostaStandard(Of AgronicaCoreWebService.CoreWS_Utenti_R)
        Dim r As New rispostaStandard(Of AgronicaCoreWebService.CoreWS_Utenti_R)

        Dim Ritorno_objParametri_Server As AgronicaCoreParametri = Nothing

        Try

            'esempio di chiamata: {  "Username": "terrinnova4", "Password": "test4",  "DatiConnessioneSelezionata": "58|1|AGRODEV1\\SQL2008R2ENG|AGRONICA_NAZIONALE_SERVER_PROD|05644051004|1" }

            Dim Ritorno_objParametri_Utenti As AgronicaCoreParametri = Nothing

            Dim objParametri_Super_Server As AgronicaCoreParametri

            Dim SessioneHttp As HttpSessionState = HttpContext.Current.Session

            'crea connessioni e objparametrisuperserver o server a seconda della chiave nel webconfig
            Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
            inizializza.InizializzaSito_GiasOnLine(SessioneHttp)

            objParametri_Super_Server = SessioneHttp("ASG_objParametri_Super_Server")

            Dim Ritorno_Descrizione As String = ""
            Dim Ritorno_Note As String = ""

            Dim Ritorno_Descrizione_Utenti As String = ""
            Dim Ritorno_Note_Utenti As String = ""


            If DatiConnessioneSelezionata.Split("|").Length <> 6 Then
                Throw New Exception("Formato non corretto nel valore del db selezionato")
            End If

            Dim ID_DB_Sel As String = DatiConnessioneSelezionata.Split("|")(0)



            'li creo, ma tanto la funzione chiamata li salva in sessione assieme 
            'alle variabili utili per la retrocompatibilità
            inizializza.Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine(
                    Username,
                    Password,
                    CInt(ID_DB_Sel),
                    Ritorno_objParametri_Server,
                    Ritorno_objParametri_Utenti,
                    Ritorno_Descrizione,
                    Ritorno_Note,
                    Ritorno_Descrizione_Utenti,
                    Ritorno_Note_Utenti,
                    objParametri_Super_Server,
                    SessioneHttp)

            Dim leggiConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim minutiValiditaLoginMemorizzato As String =
                leggiConfigSiti.Leggi_Valore(0, "minutiValiditaLoginMemorizzato", "", "", Ritorno_objParametri_Server)

            If minutiValiditaLoginMemorizzato = "" Then
                minutiValiditaLoginMemorizzato = "240"
            End If

            Dim objP_server As String = Utility.convertOBJparametritoString(Ritorno_objParametri_Server)
            Dim objP_utenti As String = Utility.convertOBJparametritoString(Ritorno_objParametri_Utenti)
            Dim objP_superServer As String = Utility.convertOBJparametritoString(objParametri_Super_Server)

            Dim xApiCall As New AgronicaCoreWebService.CoreWS_Utenti_R(objP_superServer, objP_server, objP_utenti, Ritorno_objParametri_Utenti.UtenteCodFiscale, minutiValiditaLoginMemorizzato)

            Dim gestoreUtente As New AgronicaCoreUtentiBIZ.Utenti
            gestoreUtente.InizializzaTabellaUtentiVisibilitaAppoggio(Username, 5, Ritorno_objParametri_Server, Ritorno_objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = xApiCall

        Catch ex As Exception

            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            Dim MessaggioErrore As String =
                "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            'potrei non avere l'oggetto
            If Not Ritorno_objParametri_Server Is Nothing Then
                Dim objDP As New DataProvider
                objDP.Scrivi_LOG(Ritorno_objParametri_Server, "PercorsoCaricaDati", MessaggioErrore)

            End If

            r.RispostaOK = False
            r.Errore = MessaggioErrore

        End Try

        Return r
    End Function

    ''' <summary>
    ''' Ottiene lista delle connessioni
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="SiteRedirector">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <returns></returns>
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ListaConnessioni(PivaSuperUser As String, ByVal SiteRedirector As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri

        'esempio di chiamata..
        '{  "PivaSuperUser": "05644051004", "SiteRedirector": 6 }
        Try


            If PivaSuperUser = "" Then
                Throw New Exception("parametro piva super user non valido.")
            End If


            Dim SessioneHttp As HttpSessionState = HttpContext.Current.Session

            'crea connessioni e objparametrisuperserver o server a seconda della chiave nel webconfig
            Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
            inizializza.InizializzaSito_GiasOnLine(SessioneHttp)

            objParametri_Super_Server = SessioneHttp("ASG_objParametri_Super_Server")

            Dim objP_superServer As String = Utility.convertOBJparametritoString(objParametri_Super_Server)


            'leggo dalla sessione se ho i fitri impostati
            'utile ad esempio per utuenti esterni come su neodimio che si suole che 
            'non vedano altri server anche se fanno un logout
            Dim filtriSito As AgronicaCoreParametriFiltroIngressoSitoOnline
            If Not IsNothing(Session("Filtri_Siti_Per_PaginaDefault")) Then
                filtriSito = Session("Filtri_Siti_Per_PaginaDefault")
            Else
                filtriSito.ID_DB = 0 'usato in querystring/viewstate
                filtriSito.TipoDB = enum_Tipo_DB.GIAS_SERVER
                filtriSito.Server = "" 'usato in querystring/viewstate
                filtriSito.DB = "" 'usato in querystring/viewstate
                filtriSito.Provider = ""
                filtriSito.UserId = ""
                filtriSito.Password = ""
                filtriSito.PivaSuperUser = "" 'usato in querystring/viewstate
                filtriSito.Note = ""
                filtriSito.Progressivo = 0 'usato in querystring/viewstate
                filtriSito.Descrizione = ""
            End If

            'se apro la pagina la prima volta leggo parametri filtro da querystring e li metto nel filtriSito,
            'quindi i parametri viewstate hanno sempre la precedenza            

            filtriSito.PivaSuperUser = PivaSuperUser

            Dim idleCmbServer As New DropDownList

            Dim xFiltroAPP As String = ""
            If SiteRedirector = Enum_SiteRedirector.GiasAPP Then
                xFiltroAPP = " (DisponibilePerGiasAPP is not null and DisponibilePerGiasAPP = 1) "
            End If



            AgronicaCoreUtility.CaricaListControl.Connessioni(idleCmbServer, False, "", "",
                            filtriSito.ID_DB,
                            filtriSito.TipoDB,
                            filtriSito.Server,
                            filtriSito.DB,
                            filtriSito.Provider,
                            filtriSito.UserId,
                            filtriSito.Password,
                            filtriSito.PivaSuperUser,
                            filtriSito.Note,
                            filtriSito.Progressivo,
                            filtriSito.Descrizione,
                            Date.Today,
                            Date.Today,
                            xFiltroAPP, " Descrizione, PivaSuperUser, Server, Progressivo desc, DB ", objParametri_Super_Server)



            Dim lCon As New List(Of String)
            For Each cItem As ListItem In idleCmbServer.Items
                lCon.Add("{ ""chiave"": """ & jSon.Escape(cItem.Value) & """, ""valore"": """ & jSon.Escape(cItem.Text) & """ } ")
            Next

            r.RispostaOK = True
            r.RispostaStringa = "[" & String.Join(",", lCon.ToArray) & "]"


        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String =
                "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Dim objDP As New DataProvider
            objDP.Scrivi_LOG(objParametri_Super_Server, "PercorsoCaricaDati", MessaggioErrore)


            r.RispostaOK = False
            r.Errore = MessaggioErrore


        End Try

        Return r
    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDatiUtente(ByVal objP_server As String, ByVal objP_utenti As String, ByVal usernameUtente As String, ByVal escludiSuperUser As Boolean) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim objUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim xFiltroAggiuntivo As String = ""

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If escludiSuperUser = True Then
                xFiltroAggiuntivo = " (UserNameCommerciale <> 'superuser') "
            End If

            Dim lingua As Lingua = objUtenti.Leggi_Lingua(usernameUtente, "", "", objParametriUtenti)

            Dim dt As DataTable = objUtentiDettagli.Leggi(
                usernameUtente, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                xFiltroAggiuntivo, " Cognome, Nome, Rag_Soc ", objParametriUtenti)


            ' contatto utente
            Dim contatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti = contatti_R.Leggi_Contatti_Utenti_Gias(objParametriServer, usernameUtente, "", CStr(enum_Rapporti_Contabili_Standard.Tecnico))
            Dim contatto As String = ""
            If dtContatti.Rows.Count > 0 Then
                contatto = dtContatti.Rows(0).Item("Piva") &
                    "|" & dtContatti.Rows(0).Item("Cod_Contatto") &
                    "|" & dtContatti.Rows(0).Item("Cod_RisUm")
            End If

            ' gruppo utente
            Dim gruppo As Integer
            Dim gruppo_Utente_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Dim dt_Gruppo = gruppo_Utente_R.Leggi(usernameUtente, 0, "", "", objParametriUtenti)
            If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
                gruppo = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")
            End If

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dt.Rows
                Dim utente As String = Trim(dr.Item("Cognome") & " " & dr.Item("Nome"))

                If utente = "" Then
                    utente = Trim(dr.Item("Rag_Soc"))
                End If

                jArrayListaOp.Add(New JObject(
                    New JProperty("Utente", utente),
                    New JProperty("Contatto", contatto),
                    New JProperty("Nome", dr.Item("Nome")),
                    New JProperty("Cognome", dr.Item("Cognome")),
                    New JProperty("Ragione_Sociale", dr.Item("Rag_Soc")),
                    New JProperty("Codice_Fiscale", dr.Item("CodFisc")),
                    New JProperty("Telefono", dr.Item("Tel")),
                    New JProperty("Email", dr.Item("Email")),
                    New JProperty("Qualifica", dr.Item("UserNameCommerciale")),
                    New JProperty("Gruppo", gruppo),
                    New JProperty("Lingua", If(lingua Is Nothing, "", lingua.CodiceISO))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetOperatore(ByVal objP_utenti As String,
                                 ByVal usernameUtente As String,
                                 ByVal escludiSuperUser As Boolean
                                 ) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim xFiltroAggiuntivo As String = ""

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If escludiSuperUser = True Then
                xFiltroAggiuntivo = " (UserNameCommerciale <> 'superuser') "
            End If

            Dim dt As DataTable = objUtenti.Leggi(usernameUtente,
                                                  0,
                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                  xFiltroAggiuntivo,
                                                  " Cognome, Nome, Rag_Soc ",
                                                  objParametriUtenti)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dt.Rows
                Dim utente As String = Trim(dr.Item("Cognome") & " " & dr.Item("Nome"))

                If utente = "" Then
                    utente = Trim(dr.Item("Rag_Soc"))
                End If

                jArrayListaOp.Add(New JObject(New JProperty("Codice_Fiscale", dr.Item("CodFisc")),
                                              New JProperty("Utente", utente)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_DatiAccount_APP(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim xFiltroAggiuntivo As String = ""

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim DatiAccount As String = ""
            Dim leggiEntrateUscite As New AgronicaCoreContabDAL.Orari_Entrata_Uscita_R
            Dim dtEntrateUscite = leggiEntrateUscite.LeggiUltimaTimbratura(objParametriServer.UtenteCodFiscale, objParametriServer)
            If dtEntrateUscite IsNot Nothing AndAlso dtEntrateUscite.Rows.Count > 0 Then
                Dim row = dtEntrateUscite.Rows(0)
                DatiAccount = row.Item("NrBadge") & "|" & row.Item("Identif_Dispositivo") & "|" & row.Item("Nome") & "|" & row.Item("Cognome")
            End If

            r.RispostaStringa = DatiAccount
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListaUtenti(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            }
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            Dim JFilters = JsonConvert.DeserializeObject(Of JArray)(InData.InData)
            'Dim Dt_Utenti = objUtentiBIZ.CaricaUtentiEntryPoint(InData.InData, params)
            If JFilters Is Nothing Then
                JFilters = New JArray()
            End If
            Dim Dt_Utenti = objUtentiBIZ.CaricaUtentiEntryPoint(JFilters, params)
            r.RispostaStringa = JsonConvert.SerializeObject(Dt_Utenti, Formatting.None)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function CreateRandomCF(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            }
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            Dim cf = objUtentiBIZ.GetRandomCodFisc(params)
            r.RispostaStringa = JsonConvert.SerializeObject(cf, Formatting.None)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function GetUsersCount(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            }

            res.RispostaStringa = objUtentiBIZ.GetUsersCount(params)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListaUtentiDatiBase(InData As Object) As rispostaStandard(Of ListaUtenti)

        Dim r As New rispostaStandard(Of ListaUtenti)
        Dim objUtenti As New Utenti_Dettagli_R

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objStr = JsonConvert.SerializeObject(InData, a)

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objUtentiBIZ = New Utenti

            r.RispostaStringa = objUtentiBIZ.Carica_Utenti_Dati_Base(enum_Id_Servizio.GiasOnline,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListaGruppi(InData As Object) As rispostaStandard(Of List(Of GruppoUtente))

        Dim r As New rispostaStandard(Of List(Of GruppoUtente))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)


        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objUtentiBIZ As New AgronicaCoreUtentiBIZ.Utenti
            Dim gruppi = objUtentiBIZ.Carica_Gruppi(objParametri_Utenti)

            r.RispostaStringa = gruppi
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiGruppiUtenti(ByVal objParam_server As String, ByVal objParam_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objParam_server = "" OrElse objParam_utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim dal As New Gruppi_UtenteBiz(objP_Server, objP_Utenti)
            Dim risposta As DataTable = dal.GetGruppiUtenti()

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListaGruppiUtente(InData As Object) As rispostaStandard(Of ListaGruppiUtente)

        Dim r As New rispostaStandard(Of ListaGruppiUtente)

        r.RispostaStringa = New ListaGruppiUtente()

        r.RispostaStringa.ListaDatiGruppoUtente = New List(Of DatiGruppoUtente)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objUtentiBIZ As New Utenti
            Dim gruppi = objUtentiBIZ.Carica_Gruppi(objParametri_Utenti)

            For Each gruppo In gruppi

                Dim datiGruppoUtente = New DatiGruppoUtente()

                datiGruppoUtente.Codice = gruppo.codice
                datiGruppoUtente.Descrizione = gruppo.descrizione
                datiGruppoUtente.Identificativo = gruppo.Identificativo

                r.RispostaStringa.ListaDatiGruppoUtente.Add(datiGruppoUtente)

            Next

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiGruppiMerce_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim dal As New GruppiMerce(objP_Server, objP_Utenti)
            Dim risposta As DataTable = dal.GetGruppiMerce(piva)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiGruppiMerce(piva As String, ByVal objParam_server As String, ByVal objParam_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objParam_server = "" OrElse objParam_utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim dal As New GruppiMerce(objP_Server, objP_Utenti)
            Dim risposta As DataTable = dal.GetGruppiMerce(piva)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiGruppiMerceAnagrafica(piva As String, ByVal objParam_server As String, ByVal objParam_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objParam_server = "" OrElse objParam_utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim dal As New GruppiMerce(objP_Server, objP_Utenti)

            Dim risposta As DataTable = dal.GetGruppiMercePerAnagrafica(piva)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiGruppiMerceAnagrafica_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim dal As New GruppiMerce(objP_Server, objP_Utenti)

            Dim risposta As DataTable = dal.GetGruppiMercePerAnagrafica(piva)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function TentativoCancellazioneGruppoMerce(IdGruppoMerce As Integer, forzaCancellazione As Boolean, objParam_server As String, objParam_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objParam_server = "" OrElse objParam_utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim dal As New GruppiMerce(objP_Server, objP_Utenti)

            Dim risposta = dal.TentativoCancellazioneGruppoMerce(IdGruppoMerce, forzaCancellazione)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function TentativoCancellazioneGruppoMerce_NG(InData As CoreWS_Generic(Of TentativoCancellazioneGruppoMerce)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" OrElse InData.objP.objP_utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim dal As New GruppiMerce(objP_Server, objP_Utenti)

            Dim risposta = dal.TentativoCancellazioneGruppoMerce(InData.InData.IdGruppoMerce, InData.InData.forzaCancellazione)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviGruppiUtentiPerGruppiMerce(piva As String, ByVal righeSelezionate As RigheSelezionate, ByVal objP_Server As String, objP_Utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_Server = "" OrElse objP_Utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParam_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Server)
            Dim objParam_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Utenti)

            Dim biz As New GruppiUtentiPerGruppiMerce(objParam_Server, objParam_Utenti)
            Dim risposta As ScriviDatiResult = biz.ScriviGruppiUtentiPerGruppiMercePolicy(piva, righeSelezionate)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviGruppiUtentiPerGruppiMerce_NG(InData As CoreWS_Generic(Of ScriviGruppiUtentiPerGruppiMerce)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" OrElse InData.objP.objP_utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParam_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParam_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim biz As New GruppiUtentiPerGruppiMerce(objParam_Server, objParam_Utenti)
            Dim righe As New RigheSelezionate()
            righe.Gruppi_Utente_codici = InData.InData.righeSelezionate.Gruppi_Utente_codici
            righe.Ids_Gruppo_Merce = InData.InData.righeSelezionate.Ids_Gruppo_Merce

            Dim risposta As ScriviDatiResult = biz.ScriviGruppiUtentiPerGruppiMercePolicy(InData.InData.piva, righe)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaPermesso(permessi As ChiaveCancellaPermesso(), objP_Server As String, objP_Utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_Server = "" OrElse objP_Utenti = "" Then
            r.Errore = "objParam_server o objParam_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParam_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Server)
            Dim objParam_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Utenti)

            Dim biz As New GruppiUtentiPerGruppiMerce(objParam_Server, objParam_Utenti)
            Dim risposta = biz.CancellaPermessiPolicy(permessi)

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex, False)
        End Try
    End Function
    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function CancellaPermesso_NG(InData As CoreWS_Generic(Of ListaChiaveCancellaPermessoDto)) As RispostaStandard
    '    Dim r As New RispostaStandard

    '    If InData.objP.objP_server = "" OrElse InData.objP.objP_utenti = "" Then
    '        r.Errore = "objParam_server o objParam_utenti non valorizzato"
    '        Return r
    '    End If

    '    Try
    '        Dim objParam_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
    '        Dim objParam_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

    '        Dim biz As New GruppiUtentiPerGruppiMerce(objParam_Server, objParam_Utenti)

    '        Dim permessi As New ChiaveCancellaPermesso()
    '        Dim permesso As New ChiaveCancellaPermesso()

    '        For Each perm In InData.InData.permessi
    '            permesso = New ChiaveCancellaPermesso()
    '            permesso.Gruppi_Utente_cod = perm.Gruppi_Utente_cod
    '            permesso.Id_Gruppo_Merce = perm.Id_Gruppo_Merce
    '            permesso.Piva = perm.Piva
    '        Next

    '        Dim risposta = biz.CancellaPermessiPolicy(permessi)

    '        Return ProvideRispostaStandardFrom(risposta)
    '    Catch ex As Exception
    '        Return MessaggioErroreFrom(ex, False)
    '    End Try
    'End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiungiOModificaGruppoMerce(gruppoMerce As GruppoMerce, objParam_server As String, objParam_utenti As String) As RispostaStandard

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)


            Dim biz As New GruppiMerce(objP_Server, objP_Utenti)
            biz.AggiungiOModificaGruppoMerce(gruppoMerce)

            Return ProvideRispostaStandardFrom(SUCCESS)

        Catch ex As GruppoUtilizzatoDaUnProdottoExtraEccezione
            Return RispostaStandardOnError(ex.Message)
        Catch ex As CodiceDuplicatoEccezione
            Return RispostaStandardOnError(ex.Message)
        Catch ex As UtilizzatoDaUnAltraImpresaComeGruppoMerceDefaultEccezione
            Return RispostaStandardOnError(ex.Message)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex, False)
        End Try

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiungiOModificaGruppoMerce_NG(InData As CoreWS_Generic(Of GruppoMerce)) As RispostaStandard

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)


            Dim biz As New GruppiMerce(objP_Server, objP_Utenti)
            biz.AggiungiOModificaGruppoMerce(InData.InData)

            Return ProvideRispostaStandardFrom(SUCCESS)

        Catch ex As GruppoUtilizzatoDaUnProdottoExtraEccezione
            Return RispostaStandardOnError(ex.Message)
        Catch ex As CodiceDuplicatoEccezione
            Return RispostaStandardOnError(ex.Message)
        Catch ex As UtilizzatoDaUnAltraImpresaComeGruppoMerceDefaultEccezione
            Return RispostaStandardOnError(ex.Message)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex, False)
        End Try

    End Function


    ''' <summary>
    ''' Dato access_token recupera iddb e idutenti da utenti token e ne verifica la validità
    ''' </summary>
    ''' <param name="access_token"></param>
    ''' <returns></returns>
    ''' 
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiParametriConnessioneDaAccessToken(access_token As String) As rispostaStandard(Of AgronicaCoreModelsSTD.provisioning.BackgroundLoginModel)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.provisioning.BackgroundLoginModel)
        r.RispostaOK = False

        Dim objParametri_Super_Server As AgronicaCoreParametri

        Try

            Dim SessioneHttp As HttpSessionState = HttpContext.Current.Session

            'crea connessioni e objparametrisuperserver o server a seconda della chiave nel webconfig
            Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
            inizializza.InizializzaSito_GiasOnLine(SessioneHttp)

            objParametri_Super_Server = SessioneHttp("ASG_objParametri_Super_Server")

            'Dim objP_superServer As String = Utility.convertOBJparametritoString(objParametri_Super_Server)

            Dim auth = New AgronicaCoreUtentiDAL.AutenticaUtente
            Dim token_dal = New AgronicaCoreUtentiDAL.Utenti_Token_R
            Dim dttok = token_dal.Leggi_Token(access_token, "", objParametri_Super_Server)
            If dttok.Rows.Count <= 0 Then
                Throw New Exception("Invalid token")
            End If

            r.RispostaStringa = New BackgroundLoginModel

            r.RispostaStringa.PivaSuperUser = dttok.Rows(0)("PivaSuperUser")

            auth.ASG_Autenticazione_Utente_viaToken(access_token, r.RispostaStringa.Username, r.RispostaStringa.UserPwd, r.RispostaStringa.iddb_server, objParametri_Super_Server)

            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String =
                "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.RispostaOK = False
            r.Errore = MessaggioErrore


        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CheckUtente(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = False

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.profilazione.Utente) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.profilazione.Utente))(JsonConvert.SerializeObject(InData), a)


        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim rUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

            Dim dt = rUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, " UserName='" + iData.InData.UserName + "' ", "", objP_Utenti)
            If dt.Rows.Count <= 0 Then
                Throw New Exception("Utente (" + iData.InData.UserName + ") non trovato.")
            End If
            r.RispostaOK = True
            r.Errore = ""
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String =
                "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.RispostaOK = False
            r.Errore = MessaggioErrore

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getMinutiValiditaLoginMemorizzato(InData As Object) As rispostaStandard(Of Integer)
        Dim r As New rispostaStandard(Of Integer)
        r.RispostaOK = False

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)


        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim minutiValiditaLogin As Integer = 5

            Dim rConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim valServer = rConfigurazione_Siti.Leggi_Valore(0, "minutiValiditaLoginMemorizzato", "", "", objP_Server)
            If valServer = "" Then
                Dim valSuperServer = rConfigurazione_Siti.Leggi_Valore(0, "minutiValiditaLoginMemorizzato", "", "", objP_Server)
                If valSuperServer <> "" AndAlso IsNumeric(valSuperServer) Then
                    minutiValiditaLogin = CInt(valSuperServer)
                End If
            ElseIf valServer <> "" AndAlso IsNumeric(valServer) Then
                minutiValiditaLogin = CInt(valServer)
            End If


            r.RispostaOK = True
            r.RispostaStringa = minutiValiditaLogin
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String =
                "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.RispostaOK = False
            r.Errore = MessaggioErrore

        End Try
        Return r
    End Function

End Class
