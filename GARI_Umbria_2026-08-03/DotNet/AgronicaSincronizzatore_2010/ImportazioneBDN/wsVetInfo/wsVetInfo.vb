Imports System.Net
Imports System.Reflection
Imports System.Text
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.exceptions
Imports ImportazioneBDN.VetInfoResponseModel
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class wsVetInfo

    Protected objP_Server As AgronicaCoreParametri
    Protected objParametri_Utenti As AgronicaCoreParametri

    Protected serviceEnpoint As String
    Protected oAuthEndpoint As String
    Protected access_token As String
    Protected refresh_token As String

    Protected oAuthEndpointBDN As String

    Protected client_id As String
    Protected client_secret As String

#Region "Link Login/Logout"
    Protected Const Link_Account As String = "ws/1.1/trattamenti/account"
    Protected Const Link_Account_Test As String = "ws/1.1/trattamenti/account"
    Protected Const Link_Refresh As String = "j_auth/oauth/token"
    Protected Const Link_Refresh_Test As String = "j_test_auth/oauth/token"
    Protected Const Link_Logout As String = "j_auth/oauth/revoke"
    Protected Const Link_Logout_BDN As String = "j6_auth/oauth/revoke"
    Protected Const Link_Logout_Test As String = "j_test_auth/oauth/revoke"
#End Region

#Region "Link Lettura"
    Protected Const Link_Elenco_Prodotti_Prescrizione As String = "/ws/1.1/trattamenti/prescrizioni/medicinali/search"
    Protected Const Link_Registro_delle_Prescrizioni As String = "ws/1.1/trattamenti/prescrizioni/search"
    Protected Const Link_ElencoCapi_PerProdotto As String = "/ws/1.1/trattamenti/prescrizioni/medicinali/capi/search"
    Protected Const Link_Registro_dei_Trattamenti As String = "/ws/1.1/trattamenti/search"
    Protected Const Link_Registro_dei_Trattamenti_Da_Validare As String = "/ws/1.1/trattamenti/davalidare/search"
    Protected Const Link_Registro_delle_Somministrazioni As String = "/ws/1.1/trattamenti/somministrazioni/search"
    Protected Const Link_ElencoScarichi_PerSomministrazione As String = "/ws/1.1/trattamenti/somministrazioni/scarichi/search"
    Protected Const Link_ElencoCapi_PerSomministrazione As String = "/ws/1.1/trattamenti/somministrazioni/capi/search"
    Protected Const Link_Registro_delle_Disponibilita_Medicinali As String = "/ws/1.1/trattamenti/disponibilita/search"
    Protected Const Link_Registro_Scorta_Prodotto_perSomministrazione As String = "/ws/1.1/trattamenti/disponibilita/inserimento/search"
#End Region

#Region "Link Scrittura"
    Protected Const Link_Inserimento_Trattamento_Protocollo_Terapeutico As String = "/ws/1.1/trattamenti/protocolli/inizio"
    Protected Const Link_Inserimento_Trattamento_Semplificato As String = "/ws/1.1/trattamenti/semplificato/insert"
    Protected Const Link_Inserimento_Trattamento_Protocollo_Semplificato As String = "/ws/1.1/trattamenti/protocolli/semplificato/insert"
    Protected Const Link_Inserimento_Trattamento_Massivo As String = "/ws/1.1/trattamenti/massivo/insert"
    Protected Const Link_Inserimento_Somministrazione As String = "/ws/1.1/trattamenti/somministrazioni/insert"
    Protected Const Link_Riapertura_Trattamento_Prescrizione As String = "/ws/1.1/trattamenti/riapri"
    Protected Const Link_Riapertura_Trattamento_Protocollo As String = "/ws/1.1/trattamenti/protocolli/riapri"
    Protected Const Link_Chiusura_Trattamento_Protocollo_Terapeutico As String = "/ws/1.1/trattamenti/protocolli/chiudi"
    Protected Const Link_Chiusura_Trattamento_Protocollo_Terapeutico_Medicinale As String = "/ws/1.1/trattamenti/protocolli/trattamenti/chiudi"
    Protected Const Link_Chiusura_Trattamento As String = "/ws/1.1/trattamenti/chiudi"
    Protected Const Link_Annullamento_Trattamento As String = "/ws/1.1/trattamenti/annulla"
    Protected Const Link_Annullamento_Trattamento_Protocollo_Terapeutico As String = "/ws/1.1/trattamenti/protocolli/annulla"
#End Region

    Private objLog As AgronicaCoreDataProvider.LogProvider
    Private logDirectory As String
    Private logFileName As String

    Private log As Boolean = False

    Public Sub New(objParametri_Server As AgronicaCoreParametri,
                   objParametri_Utenti As AgronicaCoreParametri,
                   Username As String)

        Me.objP_Server = objParametri_Server
        Me.objParametri_Utenti = objParametri_Utenti


        Dim objUtentiDal_R As New AgronicaCoreUtentiDAL.Utenti_Token_Spid_R

        If Username = "" Then
            Username = objParametri_Server.UtenteUsername
        End If
        Dim dtToken = objUtentiDal_R.Leggi(0, Username, 1, "", "", objParametri_Utenti)
        If dtToken.Rows.Count = 0 Then
            Throw New GiasException("Token non impostato per l'utente corrente")
        End If

        Me.access_token = dtToken.Rows(0)("Access_Token")
        Me.refresh_token = dtToken.Rows(0)("Refresh_Token")



        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim confSitiVetinfo = confSiti.leggiConfigurazioneBDNVetInfo(objParametri_Server)

        Me.serviceEnpoint = confSitiVetinfo.VetInfo.link
        Me.oAuthEndpoint = confSitiVetinfo.VetInfo.authorizeLink
        Me.oAuthEndpointBDN = confSitiVetinfo.BDN.authorizeLink
        Me.client_id = confSitiVetinfo.VetInfo.clientId
        Me.client_secret = confSitiVetinfo.VetInfo.clientSecret

        Me.log = confSitiVetinfo.logMsg
        'If Debugger.IsAttached Then
        '    Me.serviceEnpoint = "https://wstest.izs.it/demo_farmaco_test"
        'End If

        Me.objLog = New AgronicaCoreDataProvider.LogProvider
        Me.logDirectory = Me.objP_Server.LogDirectory & "\BDN\"

        Dim dataOggi As String = Date.Now.ToString("yyyyMMdd")
        Dim codFiscUsername As String = objParametri_Server.UtenteCodFiscale

        Me.logFileName = dataOggi & "_" & codFiscUsername & "_" & "chiamateVetInfo.txt"

    End Sub


    ''' <summary>
    ''' Log delle esportazioni verso VetInfo
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="saCod"></param>
    ''' <param name="staNum"></param>
    ''' <param name="msg"></param>
    ''' <param name="esito"></param>
    ''' <param name="datiInviati"></param>
    ''' <param name="datiRicevuti"></param>
    ''' <param name="objP_Server"></param>
    Private Sub LogVIExport(ByVal piva As String,
                            ByVal saCod As Integer,
                            ByVal staNum As Integer,
                            ByVal idAgenda As Integer,
                            ByVal msg As String,
                            ByVal esito As String,
                            ByVal datiInviati As String,
                            ByVal datiRicevuti As String,
                            ByRef objP_Server As AgronicaCoreParametri)
        Dim logInvioA_W As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_W
        Dim logInvioC_W As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Const CAUSALE_COD As Integer = 1 ' invio a vetinfo

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objP_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        'AGRONICA LOG INVIO CHIAMATE
        Dim logInvioC = logInvioC_W.Create_Agronica_Log_Invio_Chiamate(
            enum_Esportazioni_Sistema_Cod.BDN,
            datiInviati,
            DateTime.Now,
            esito,
            datiRicevuti,
            0,
            0,
            objP_Server,
            GiasContext
        )

        Dim chiaveStalla As String = $"{piva}_{saCod}_{staNum}"
        'AGRONICA LOG INVIO AGENDA
        Dim logInvioA = logInvioA_W.Create_Agronica_Log_Invio_Agenda(
            enum_Esportazioni_Sistema_Cod.BDN,
            idAgenda,
            0,
            logInvioC.ID,
            objP_Server,
            GiasContext,
            Chiave_Esterna:=chiaveStalla,
            causaleCod:=CAUSALE_COD
        )
    End Sub


    Protected Function callVetInfo(ByVal uri As String,
                                   ByVal Method As String,
                                   Optional ByVal strFilter As String = "",
                                   Optional number As Integer = 0,
                                   Optional ByVal errorOn401 As Boolean = False) As String
        Dim complete_uri = Me.serviceEnpoint & "/" & uri
        If strFilter <> "" Then
            complete_uri = complete_uri & "?" & strFilter
        End If

        Dim rsMethod As RestSharp.Method
        If Method = "GET" Then
            rsMethod = RestSharp.Method.GET
        ElseIf Method = "POST" Then
            rsMethod = RestSharp.Method.POST
        End If

        Dim customHeader = New System.Net.WebHeaderCollection
        customHeader.Add("Authorization", "Bearer " & access_token)

        Dim client = New AgronicaCoreUtility.Http()
        Dim responseStr1 As String
        'Dim responseStr1 = webHelper.chiamaWS("",
        '                                      "",
        '                                      confBDN.BDN.authorizeLink & "/" & userPath,
        '                                      "",
        '                                      "GET",
        '                                      "",
        '                                      "",
        '                                      customHeader)
        Dim bkSec = ServicePointManager.SecurityProtocol

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objP_Server.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = logFileName
        }
        If Me.log Then
            objLog.Scrivi_LOG(objP_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Chiamata:" & vbCrLf & complete_uri,
                          CustomLOGParams:=customLOGParams)
        End If


        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim responseStrRestShapr = client.chiamaWS_RestShapr("",
                                          "",
                                          complete_uri,
                                          "application/x-www-form-urlencoded",
                                          rsMethod,
                                          "application/json",
                                          "",
                                          customHeader,
                                          TimeOut:=900000)

        responseStr1 = responseStrRestShapr.Content

        If Me.log Then
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "Risposta (" & CStr(responseStrRestShapr.StatusCode) & ") :" & vbCrLf & responseStr1, CustomLOGParams:=customLOGParams)
        End If


        ServicePointManager.SecurityProtocol = bkSec

        If responseStrRestShapr.StatusCode = HttpStatusCode.InternalServerError Then
        ElseIf responseStrRestShapr.StatusCode = HttpStatusCode.Unauthorized Then
            'Refresh Token
            If errorOn401 Then
                Throw New BDNException("Sessione scaduta, loggarsi nuovamente a VetInfo")
            End If
            RefreshToken()
            If number = 0 Then responseStr1 = callVetInfo(uri, Method, strFilter, 1)

        ElseIf responseStrRestShapr.StatusCode = 0 Then
            ' Ritenta due volte
            If errorOn401 Then
                Throw New BDNException("Sessione scaduta, loggarsi nuovamente a VetInfo")
            End If
            If number < 2 Then responseStr1 = callVetInfo(uri, Method, strFilter, number + 1)

        End If

        Return responseStr1

    End Function

    Protected Function callVetInfoWrite(ByVal uri As String,
                                        ByVal content As JObject,
                                        ByVal method As String,
                                        Optional ByVal number As Integer = 0) As String
        Dim complete_uri = Me.serviceEnpoint & "/" & uri

        Dim rsMethod As Integer
        If method = "GET" Then
            rsMethod = RestSharp.Method.GET
        ElseIf method = "POST" Then
            rsMethod = RestSharp.Method.POST
        End If

        Dim customHeader = New System.Net.WebHeaderCollection
        customHeader.Add("Authorization", "Bearer " & access_token)

        Dim client = New AgronicaCoreUtility.Http()
        Dim responseStr As String
        Dim bkSec = ServicePointManager.SecurityProtocol

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objP_Server.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = logFileName
        }
        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "Chiamata:" & vbCrLf & complete_uri, CustomLOGParams:=customLOGParams)

        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "obj:" & vbCrLf & content.ToString, CustomLOGParams:=customLOGParams)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim responseStrRestShapr = client.chiamaWS_RestShapr(content, "", complete_uri,
                                                             "application/json", rsMethod, "application/json",
                                                             "", customHeader, UseNewtonsoftJson:=True, TimeOut:=900000)
        responseStr = responseStrRestShapr.Content

        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "Risposta (" & CStr(responseStrRestShapr.StatusCode) & ") :" & vbCrLf & responseStr, CustomLOGParams:=customLOGParams)

        ServicePointManager.SecurityProtocol = bkSec

        If responseStrRestShapr.StatusCode = HttpStatusCode.InternalServerError Then

        ElseIf responseStrRestShapr.StatusCode = HttpStatusCode.Unauthorized Then
            Throw New BDNException("Sessione scaduta, loggarsi nuovamente a VetInfo")
            'RefreshToken()
            'If number = 0 Then responseStr = callVetInfoWrite(uri, content, method, 1)
        ElseIf responseStr.Contains("invalid_token") Then
            Throw New BDNException("Sessione scaduta, loggarsi nuovamente a VetInfo")
            'RefreshToken()
            'If number = 0 Then responseStr = callVetInfoWrite(uri, content, method, 1)
        End If

        Return responseStr

    End Function

    Protected Function callVetInfoRefresh(ByVal uri As String,
                                   ByVal Method As String,
                                   Optional ByVal strFilter As String = "") As String
        Dim complete_uri = uri
        If strFilter <> "" Then
            complete_uri = complete_uri & "?" & strFilter
        End If

        Dim WebReq As HttpWebRequest = WebRequest.Create(complete_uri)
        Dim rsMethod As RestSharp.Method
        If Method = "GET" Then
            rsMethod = RestSharp.Method.GET
        ElseIf Method = "POST" Then
            rsMethod = RestSharp.Method.POST
        End If

        Dim client_auth_64 = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(Me.client_id & ":" & Me.client_secret))
        Dim customHeader = New System.Net.WebHeaderCollection
        customHeader.Add("Authorization", "Basic " & client_auth_64)

        Dim client = New AgronicaCoreUtility.Http()
        Dim responseStr1 As String
        Dim bkSec = ServicePointManager.SecurityProtocol

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objP_Server.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = logFileName
        }

        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "Chiamata:" & vbCrLf & complete_uri, CustomLOGParams:=customLOGParams)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim responseStrRestShapr = client.chiamaWS_RestShapr("",
                                          "",
                                          complete_uri,
                                          "application/x-www-form-urlencoded",
                                          rsMethod,
                                          "application/json",
                                          "",
                                          customHeader)

        responseStr1 = responseStrRestShapr.Content

        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "Risposta (" & CStr(responseStrRestShapr.StatusCode) & "):" & vbCrLf & responseStr1, CustomLOGParams:=customLOGParams)

        ServicePointManager.SecurityProtocol = bkSec

        If responseStrRestShapr.StatusCode = HttpStatusCode.Unauthorized Then
            ' Refresh Token
            Throw New GiasException("Token Scaduto")
        End If

        Return responseStr1

    End Function

    Protected Function callVetInfoLogout(ByVal uri As String,
                                   ByVal Method As String,
                                   Optional ByVal strFilter As String = "") As String
        Dim complete_uri = uri
        If strFilter <> "" Then
            complete_uri = complete_uri & "?" & strFilter
        End If

        Dim WebReq As HttpWebRequest = WebRequest.Create(complete_uri)
        Dim rsMethod As RestSharp.Method
        If Method = "GET" Then
            rsMethod = RestSharp.Method.GET
        ElseIf Method = "POST" Then
            rsMethod = RestSharp.Method.POST
        End If

        Dim client_auth_64 = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(Me.client_id & ":" & Me.client_secret))
        Dim customHeader = New System.Net.WebHeaderCollection
        customHeader.Add("Authorization", "Bearer " & access_token)

        Dim client = New AgronicaCoreUtility.Http()
        Dim responseStr1 As String
        Dim bkSec = ServicePointManager.SecurityProtocol

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim responseStrRestShapr = client.chiamaWS_RestShapr("",
                                          "",
                                          complete_uri,
                                          "application/x-www-form-urlencoded",
                                          rsMethod,
                                          "application/json",
                                          "",
                                          customHeader)

        responseStr1 = responseStrRestShapr.Content

        ServicePointManager.SecurityProtocol = bkSec

        If responseStrRestShapr.StatusCode = HttpStatusCode.Unauthorized Then
            ' Refresh Token
            Throw New GiasException("Token Scaduto")
        End If

        Return responseStr1

    End Function

    Private Function getStrFilters(filters As List(Of VetInfoModel.VetInfoFilter)) As String
        Dim strFilters As String = ""

        If Not IsNothing(filters) Then
            Dim i As Integer = 0

            For Each f In filters
                If strFilters <> "" Then
                    strFilters &= "&"
                End If

                strFilters &= getStrFilter(f, i)
                i += 1
            Next
        End If

        Return strFilters
    End Function

    Private Function getStrFilter(filter As VetInfoModel.VetInfoFilter, index As Integer) As String
        Dim strFilter As String = ""

        If filter IsNot Nothing Then

            strFilter &= "filter%5b" & index & "%5D.field=" & filter.field

            If filter.op IsNot Nothing AndAlso filter.op <> "" Then
                If strFilter <> "" Then
                    strFilter &= "&"
                End If
                strFilter &= "filter%5b" & index & "%5D.op=" & filter.op
            End If

            If filter.value1 IsNot Nothing AndAlso filter.value1 <> "" Then
                If strFilter <> "" Then
                    strFilter &= "&"
                End If
                strFilter &= "filter%5b" & index & "%5D.value1=" & filter.value1
            End If

            If filter.value2 IsNot Nothing AndAlso filter.value2 <> "" Then
                If strFilter <> "" Then
                    strFilter &= "&"
                End If
                strFilter &= "filter%5b" & index & "%5D.value2=" & filter.value2
            End If

        End If

        Return strFilter

    End Function

    Private Function getStrPagination(paginations As VetInfoModel.VetInfoPagination)
        Dim strPagination As String = ""

        If paginations IsNot Nothing Then

            If paginations.recordsPerPage > 0 Then
                strPagination &= "pagination.recordsPerPage=" & paginations.recordsPerPage
            End If

            If paginations.page > 0 Then
                If strPagination <> "" Then
                    strPagination &= "&"
                End If
                strPagination &= "pagination.page=" & paginations.page
            End If

            If paginations.sortBy <> "" Then
                If strPagination <> "" Then
                    strPagination &= "&"
                End If
                strPagination &= "pagination.sortBy=" & paginations.sortBy
            End If

            If paginations.sortType <> "" Then
                If strPagination <> "" Then
                    strPagination &= "&"
                End If
                strPagination &= "pagination.sortType=" & paginations.sortType
            End If

        End If

        Return strPagination
    End Function

    Public Function Account() As VetInfoResponse(Of Account_Response)
        Dim objResp_Account As VetInfoResponse(Of Account_Response)
        Dim strResp As String = ""

        Try

            strResp = callVetInfo(Link_Account, "GET")
            objResp_Account = JsonConvert.DeserializeObject(Of VetInfoResponse(Of Account_Response))(strResp)

        Catch ex As Exception
            Throw New Exception(strResp)
        End Try

        Return objResp_Account
    End Function

    Public Function Logout() As LogoutToken_Response
        Dim objResp_Account As LogoutToken_Response

        Dim complete_uri = Me.oAuthEndpoint
        If complete_uri.Contains("test") Then
            complete_uri = complete_uri & "/" & Link_Logout_Test
        Else
            complete_uri = complete_uri & "/" & Link_Logout
        End If

        Dim strResp = callVetInfoLogout(complete_uri, "GET")
        objResp_Account = JsonConvert.DeserializeObject(Of LogoutToken_Response)(strResp)
        Return objResp_Account
    End Function

    Public Function LogoutBDN() As LogoutToken_Response
        Dim objResp_Account As LogoutToken_Response

        Dim complete_uri = Me.oAuthEndpointBDN
        If complete_uri.Contains("test") Then
            complete_uri = complete_uri & "/" & Link_Logout_Test
        Else
            complete_uri = complete_uri & "/" & Link_Logout_BDN
        End If

        Dim strResp = callVetInfoLogout(complete_uri, "GET")
        objResp_Account = JsonConvert.DeserializeObject(Of LogoutToken_Response)(strResp)
        Return objResp_Account
    End Function

    Public Function RefreshToken() As RefreshToken_Response
        Dim objResp_Account As RefreshToken_Response

        Dim complete_uri = Me.oAuthEndpoint
        If complete_uri.Contains("test") Then
            complete_uri = complete_uri & "/" & Link_Refresh_Test
        Else
            complete_uri = complete_uri & "/" & Link_Refresh
        End If

        Dim link = complete_uri & "?grant_type=refresh_token&refresh_token=" & Me.refresh_token
        Dim strResp = callVetInfoRefresh(link, "POST")
        objResp_Account = JsonConvert.DeserializeObject(Of RefreshToken_Response)(strResp)
        Dim objUtentiDal_R As New AgronicaCoreUtentiDAL.Utenti_Token_Spid_R
        Dim objUtentiDal_W As New AgronicaCoreUtentiDAL.Utenti_Token_Spid_W
        Dim dtUtenti = objUtentiDal_R.Leggi(0, objParametri_Utenti.UtenteUsername, 1, "", "", objParametri_Utenti)
        If dtUtenti.Rows.Count > 0 Then
            objUtentiDal_W.Modifica_Parametrizzata(dtUtenti.Rows(0)("ID"), "Access_Token", objResp_Account.access_token, objParametri_Utenti)
            objUtentiDal_W.Modifica_Parametrizzata(dtUtenti.Rows(0)("ID"), "Refresh_Token", objResp_Account.refresh_token, objParametri_Utenti)

            Me.access_token = objResp_Account.access_token
            Me.refresh_token = objResp_Account.refresh_token
        End If


        Return objResp_Account
    End Function

#Region "Chiamate Lettura"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function RegistroPrescrizioni(ByVal Azienda_Codice As String,
                                         ByVal Prop_IdFiscale As String,
                                         ByVal Data_Inizio As Date,
                                         ByVal Data_Fine As Date,
                                         Optional filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                         Optional paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of RegistroPrescrizioni_Response)
        Dim objResp_RegistroPresc As VetInfoResponseModel.VetInfoResponse(Of RegistroPrescrizioni_Response)

        If filters Is Nothing Then
            filters = New List(Of VetInfoModel.VetInfoFilter)
        End If

        Dim filterRecord_propIdFiscale As New VetInfoModel.VetInfoFilter
        filterRecord_propIdFiscale.field = "presProprietarioIdFiscale"
        filterRecord_propIdFiscale.op = "EQUALS"
        filterRecord_propIdFiscale.value1 = Prop_IdFiscale
        filters.Add(filterRecord_propIdFiscale)

        Dim filterRecord_aziendaCodice As New VetInfoModel.VetInfoFilter
        filterRecord_aziendaCodice.field = "presAziendaCodice"
        filterRecord_aziendaCodice.op = "EQUALS"
        filterRecord_aziendaCodice.value1 = Azienda_Codice
        filters.Add(filterRecord_aziendaCodice)

        'filtro per ricavare le prescrizioni degli ultimi 6 mesi (nel caso non sia presente il filtro su periodo)
        Dim filterRecord_LastMonths As New VetInfoModel.VetInfoFilter
        If Data_Inizio <> AGRODATAINIZIO OrElse Data_Fine <> AGRODATAFINE Then
            filterRecord_LastMonths.field = "presDtEmissione"
            filterRecord_LastMonths.op = "BETWEEN"
            filterRecord_LastMonths.value1 =
                If(Data_Inizio <> AGRODATAINIZIO, (Data_Inizio).ToString("dd-MM-yyyy"), ((Date.Now).AddDays(-180)).ToString("dd-MM-yyyy"))
            filterRecord_LastMonths.value2 =
                If(Data_Fine <> AGRODATAFINE, (Data_Fine).ToString("dd-MM-yyyy"), (Date.Now).ToString("dd-MM-yyyy"))
            filters.Add(filterRecord_LastMonths)
        End If

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination As String = getStrPagination(paginations)
            Dim strFilter As String = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_Registro_delle_Prescrizioni, "GET",
                                  strFilter & strPagination)

        Else
            strResp = callVetInfo(Link_Registro_delle_Prescrizioni, "GET")
        End If

        objResp_RegistroPresc = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroPrescrizioni_Response))(strResp)

        If IsNothing(objResp_RegistroPresc.errors) OrElse objResp_RegistroPresc.errors.Count = 0 Then
            Dim totalRecords = CInt(objResp_RegistroPresc.pagination.totalRecords)

            If totalRecords > 10 Then
                Dim totalCallD As Double = (totalRecords / 50)
                Dim totalCall As Integer = 0

                If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                    totalCall = CInt(totalCallD) + 1
                Else
                    totalCall = CInt(totalCallD)
                End If

                If IsNothing(paginations) Then
                    paginations = New VetInfoModel.VetInfoPagination
                End If

                For i As Integer = 1 To totalCall
                    paginations.page = i
                    paginations.recordsPerPage = 50
                    paginations.sortBy = ""
                    paginations.sortType = "ASC"

                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)

                    If strPagination <> "" Then
                        strFilter &= "&"
                    End If

                    strResp = callVetInfo(Link_Registro_delle_Prescrizioni, "GET",
                                          strFilter & strPagination)

                    If i = 1 Then
                        objResp_RegistroPresc = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroPrescrizioni_Response))(strResp)

                    Else
                        Dim objTempR_RegistroPresc = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroPrescrizioni_Response))(strResp)
                        objResp_RegistroPresc.data.AddRange(objTempR_RegistroPresc.data)
                    End If

                Next

            End If

            'Dim objResp_RegistroPrescFiltrato = objResp_RegistroPresc.data.GroupBy((Function(x) x.presTipoCodice))

            If Not IsNothing(objResp_RegistroPresc.data) AndAlso objResp_RegistroPresc.data.Count > 0 Then
                Return objResp_RegistroPresc.data
            Else
                Return Nothing
            End If

        Else
            Return Nothing

        End If


    End Function

    Public Function Read_Prescrizione(ByVal AziendaCodice As String,
                                      ByVal ProprietarioIdFiscale As String,
                                      ByVal Pres_Numero As String,
                                      Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                      Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As RegistroPrescrizioni_Response
        Dim objResp_RegistroPres As VetInfoResponseModel.VetInfoResponse(Of RegistroPrescrizioni_Response)

        Dim filterTrattNumero As New VetInfoModel.VetInfoFilter
        filterTrattNumero.field = "presNumero"
        filterTrattNumero.op = "EQUALS"
        filterTrattNumero.value1 = Pres_Numero

        If IsNothing(filters) Then
            filters = New List(Of VetInfoModel.VetInfoFilter)
            filters.Add(filterTrattNumero)
        Else
            filters.Add(filterTrattNumero)
        End If

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_Registro_delle_Prescrizioni, "GET",
                                  "presAziendaCodice=" & AziendaCodice & "&presProprietarioIdFiscale=" & ProprietarioIdFiscale &
                                  "&" & strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_Registro_delle_Prescrizioni, "GET",
                                  "presAziendaCodice=" & AziendaCodice & "&presProprietarioIdFiscale=" & ProprietarioIdFiscale)
        End If

        objResp_RegistroPres = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroPrescrizioni_Response))(strResp)

        If IsNothing(objResp_RegistroPres.errors) OrElse objResp_RegistroPres.errors.Count = 0 Then

            If Not IsNothing(objResp_RegistroPres.data) AndAlso objResp_RegistroPres.data.Count = 1 Then Return objResp_RegistroPres.data.First

            Return Nothing
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="presNumero">Chiave Prescrizione</param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function ElencoProdotti_Prescrizione(ByVal presNumero As String,
                                                Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                                Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of ProdottoPerPrescrizione_Response)
        Dim objResp_ElencoProdPres As VetInfoResponseModel.VetInfoResponse(Of ProdottoPerPrescrizione_Response)

        Try
            Dim getAllData As Boolean = False
            If paginations Is Nothing AndAlso filters Is Nothing Then
                getAllData = True
            End If

            Dim strResp As String
            If Not getAllData Then
                Dim strPagination = getStrPagination(paginations)
                Dim strFilter = getStrFilters(filters)

                If strPagination <> "" Then
                    strFilter &= "&"
                End If

                strResp = callVetInfo(Link_Elenco_Prodotti_Prescrizione, "GET",
                                  "presNumero=" & presNumero & "&" &
                                  strFilter & strPagination)
            Else
                strResp = callVetInfo(Link_Elenco_Prodotti_Prescrizione, "GET",
                                  "presNumero=" & presNumero)
            End If

            objResp_ElencoProdPres = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of ProdottoPerPrescrizione_Response))(strResp)

            If IsNothing(objResp_ElencoProdPres.errors) OrElse objResp_ElencoProdPres.errors.Count = 0 Then
                Dim totalRecords = CInt(objResp_ElencoProdPres.pagination.totalRecords)

                If totalRecords > 10 Then
                    Dim totalCallD As Double = (totalRecords / 20)
                    Dim totalCall As Integer = 0

                    If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                        totalCall = CInt(totalCallD) + 1
                    Else
                        totalCall = CInt(totalCallD)
                    End If

                    If IsNothing(paginations) Then
                        paginations = New VetInfoModel.VetInfoPagination
                    End If

                    For i As Integer = 1 To totalCall
                        paginations.page = i
                        paginations.recordsPerPage = 20
                        paginations.sortBy = ""
                        paginations.sortType = "ASC"

                        Dim strPagination = getStrPagination(paginations)
                        Dim strFilter = getStrFilters(filters)

                        If strPagination <> "" Then
                            strFilter &= "&"
                        End If

                        strResp = callVetInfo(Link_Elenco_Prodotti_Prescrizione, "GET",
                                          "presNumero=" & presNumero & "&" &
                                          strFilter & strPagination)

                        If i = 1 Then
                            objResp_ElencoProdPres = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of ProdottoPerPrescrizione_Response))(strResp)

                        Else
                            Dim objTempR_ElencoProdPres = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of ProdottoPerPrescrizione_Response))(strResp)
                            objResp_ElencoProdPres.data.AddRange(objTempR_ElencoProdPres.data)

                        End If
                    Next
                End If

                If Not IsNothing(objResp_ElencoProdPres.data) AndAlso objResp_ElencoProdPres.data.Count > 0 Then
                    Return objResp_ElencoProdPres.data
                Else
                    Return Nothing
                End If

            Else
                Return Nothing
            End If

        Catch ex As Exception
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                      "Errore in ElencoProdotti_Prescrizione: presNumero " & presNumero & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True))
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="presrigaNumero">Chiave riga Medicinale della Prescrizione</param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function ElencoAnimali_ProdottoPres(ByVal presrigaNumero As String,
                                               Optional ByVal errorOn401 As Boolean = False,
                                               Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                               Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of CapoPerProdotto_Response)
        Dim objResp_ElencoCapiProd As VetInfoResponseModel.VetInfoResponse(Of CapoPerProdotto_Response)

        Try
            Dim getAllData As Boolean = False
            If paginations Is Nothing Then
                getAllData = True
            End If

            Dim strResp As String
            If Not getAllData Then
                Dim strPagination = getStrPagination(paginations)
                Dim strFilter = getStrFilters(filters)

                strResp = callVetInfo(Link_ElencoCapi_PerProdotto, "GET",
                                      "presrigaNumero=" & presrigaNumero & "&" & strFilter & "&" & strPagination, 0, errorOn401)
            Else
                strResp = callVetInfo(Link_ElencoCapi_PerProdotto, "GET",
                                      "presrigaNumero=" & presrigaNumero, 0, errorOn401)
            End If

            objResp_ElencoCapiProd = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of CapoPerProdotto_Response))(strResp)

            If IsNothing(objResp_ElencoCapiProd.errors) OrElse objResp_ElencoCapiProd.errors.Count = 0 Then
                Dim totalRecords = CInt(objResp_ElencoCapiProd.pagination.totalRecords)

                If totalRecords > 10 Then
                    Dim totalCallD As Double = (totalRecords / 100)
                    Dim totalCall As Integer = 0

                    If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                        totalCall = CInt(totalCallD) + 1
                    Else
                        totalCall = CInt(totalCallD)
                    End If

                    If IsNothing(paginations) Then
                        paginations = New VetInfoModel.VetInfoPagination
                    End If

                    For i As Integer = 1 To totalCall
                        paginations.page = i
                        paginations.recordsPerPage = 100
                        paginations.sortBy = ""
                        paginations.sortType = "ASC"

                        Dim strPagination = getStrPagination(paginations)
                        Dim strFilter = getStrFilters(filters)

                        strResp = callVetInfo(Link_ElencoCapi_PerProdotto, "GET",
                                      "presrigaNumero=" & presrigaNumero & "&" & strFilter & "&" & strPagination)

                        If i = 1 Then
                            objResp_ElencoCapiProd = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of CapoPerProdotto_Response))(strResp)

                        Else
                            Dim objTempR_ElencoCapiProd = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of CapoPerProdotto_Response))(strResp)
                            objResp_ElencoCapiProd.data.AddRange(objTempR_ElencoCapiProd.data)

                        End If
                    Next
                End If

            End If

            If Not IsNothing(objResp_ElencoCapiProd.data) AndAlso objResp_ElencoCapiProd.data.Count > 0 Then
                Return objResp_ElencoCapiProd.data
            Else
                Return Nothing
            End If

        Catch ex As BDNException
            Throw ex
        Catch ex As Exception
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                      "Errore in ElencoAnimali_ProdottoPres: presrigaNumero " & presrigaNumero & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True))
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="AziendaCodice"></param>
    ''' <param name="ProprietarioIdFiscale"></param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function RegistroTrattamenti(ByVal AziendaCodice As String,
                                        ByVal ProprietarioIdFiscale As String,
                                        ByVal Data_Inizio As Date,
                                        ByVal Data_Fine As Date,
                                        Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                        Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of RegistroTrattamenti_Response)
        Dim objResp_RegistroTratt As VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamenti_Response)

        'filtro per ricavare i trattamenti degli ultimi 6 mesi 
        Dim filterRecord_LastMonths As New VetInfoModel.VetInfoFilter
        filterRecord_LastMonths.field = "tratDtInizio"
        filterRecord_LastMonths.op = "BETWEEN"
        filterRecord_LastMonths.value1 = ((Date.Now).AddDays(-365)).ToString("dd-MM-yyyy")
        filterRecord_LastMonths.value2 = (Date.Now).ToString("dd-MM-yyyy")

        If Data_Inizio <> AGRODATAINIZIO Then
            filterRecord_LastMonths.value1 = (Data_Inizio).ToString("dd-MM-yyyy")
        End If

        If Data_Fine <> AGRODATAFINE Then
            filterRecord_LastMonths.value2 = (Data_Fine).ToString("dd-MM-yyyy")
        End If

        If IsNothing(filters) Then
            filters = New List(Of VetInfoModel.VetInfoFilter)
            filters.Add(filterRecord_LastMonths)
        Else
            filters.Add(filterRecord_LastMonths)
        End If

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_Registro_dei_Trattamenti, "GET",
                                  "presAziendaCodice=" & AziendaCodice & "&presProprietarioIdFiscale=" & ProprietarioIdFiscale &
                                  "&" & strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_Registro_dei_Trattamenti, "GET",
                                  "presAziendaCodice=" & AziendaCodice & "&presProprietarioIdFiscale=" & ProprietarioIdFiscale)
        End If

        objResp_RegistroTratt = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamenti_Response))(strResp)

        If IsNothing(objResp_RegistroTratt.errors) OrElse objResp_RegistroTratt.errors.Count = 0 Then
            Dim totalRecords = CInt(objResp_RegistroTratt.pagination.totalRecords)

            If totalRecords > 10 Then
                Dim totalCallD As Double = (totalRecords / 50)
                Dim totalCall As Integer = 0

                If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                    totalCall = CInt(totalCallD) + 1
                Else
                    totalCall = CInt(totalCallD)
                End If

                If IsNothing(paginations) Then
                    paginations = New VetInfoModel.VetInfoPagination
                End If

                For i As Integer = 1 To totalCall
                    paginations.page = i
                    paginations.recordsPerPage = 50
                    paginations.sortBy = "tratDtInizio"
                    paginations.sortType = "ASC"

                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)

                    If strPagination <> "" Then
                        strFilter &= "&"
                    End If

                    strResp = callVetInfo(Link_Registro_dei_Trattamenti, "GET",
                                          "presAziendaCodice=" & AziendaCodice & "&presProprietarioIdFiscale=" & ProprietarioIdFiscale &
                                          "&" & strFilter & strPagination)

                    If i = 1 Then
                        objResp_RegistroTratt = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamenti_Response))(strResp)

                    Else
                        Dim objTempR_RegistroTratt = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamenti_Response))(strResp)
                        objResp_RegistroTratt.data.AddRange(objTempR_RegistroTratt.data)

                    End If

                Next

            End If

            If Not IsNothing(objResp_RegistroTratt.data) AndAlso objResp_RegistroTratt.data.Count > 0 Then
                objResp_RegistroTratt.data.Sort(Function(tratt1, tratt2)
                                                    Return CDate(tratt1.tratDtInizio) < CDate(tratt2.tratDtInizio)
                                                End Function)

                Return objResp_RegistroTratt.data
            Else
                Return Nothing
            End If

        Else
            Return Nothing
        End If

    End Function


    ''' <summary>
    ''' Legge un singolo trattamento
    ''' </summary>
    ''' <param name="AziendaCodice"></param>
    ''' <param name="ProprietarioIdFiscale"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <param name="Tratt_Numero"></param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function Read_Trattamento(ByVal AziendaCodice As String,
                                     ByVal ProprietarioIdFiscale As String,
                                     ByVal Tratt_Numero As String,
                                     Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                     Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As RegistroTrattamenti_Response
        Dim objResp_RegistroTratt As VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamenti_Response)

        Dim filterTrattNumero As New VetInfoModel.VetInfoFilter
        filterTrattNumero.field = "tratNumero"
        filterTrattNumero.op = "EQUALS"
        filterTrattNumero.value1 = Tratt_Numero

        If IsNothing(filters) Then
            filters = New List(Of VetInfoModel.VetInfoFilter)
            filters.Add(filterTrattNumero)
        Else
            filters.Add(filterTrattNumero)
        End If

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_Registro_dei_Trattamenti, "GET",
                                  "presAziendaCodice=" & AziendaCodice & "&presProprietarioIdFiscale=" & ProprietarioIdFiscale &
                                  "&" & strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_Registro_dei_Trattamenti, "GET",
                                  "presAziendaCodice=" & AziendaCodice & "&presProprietarioIdFiscale=" & ProprietarioIdFiscale)
        End If

        objResp_RegistroTratt = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamenti_Response))(strResp)

        If IsNothing(objResp_RegistroTratt.errors) OrElse objResp_RegistroTratt.errors.Count = 0 Then

            If Not IsNothing(objResp_RegistroTratt.data) AndAlso objResp_RegistroTratt.data.Count = 1 Then Return objResp_RegistroTratt.data.First

            Return Nothing
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function RegistroTrattamentiDaValidare(ByVal Data_Inizio As Date,
                                                  ByVal Data_Fine As Date,
                                                  Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                                  Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of RegistroTrattamentiDaValidare_Response)
        Dim objResp_RegistroTratt As VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamentiDaValidare_Response)

        'filtro per ricavare i trattamenti degli ultimi 6 mesi 
        Dim filterRecord_LastMonths As New VetInfoModel.VetInfoFilter
        filterRecord_LastMonths.field = "tratDtInizio"
        filterRecord_LastMonths.op = "BETWEEN"
        filterRecord_LastMonths.value1 = ((Date.Now).AddDays(-365)).ToString("dd-MM-yyyy")
        filterRecord_LastMonths.value2 = (Date.Now).ToString("dd-MM-yyyy")

        If Data_Inizio <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
            filterRecord_LastMonths.value1 = (Data_Inizio).ToString("dd-MM-yyyy")
        End If

        If Data_Fine <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE Then
            filterRecord_LastMonths.value2 = (Data_Fine).ToString("dd-MM-yyyy")
        End If

        If IsNothing(filters) Then
            filters = New List(Of VetInfoModel.VetInfoFilter)
            filters.Add(filterRecord_LastMonths)
        Else
            filters.Add(filterRecord_LastMonths)
        End If

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_Registro_dei_Trattamenti_Da_Validare, "GET", strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_Registro_dei_Trattamenti_Da_Validare, "GET")
        End If

        objResp_RegistroTratt = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamentiDaValidare_Response))(strResp)

        If IsNothing(objResp_RegistroTratt.errors) OrElse objResp_RegistroTratt.errors.Count = 0 Then
            Dim totalRecords = CInt(objResp_RegistroTratt.pagination.totalRecords)

            If totalRecords > 10 Then
                Dim totalCallD As Double = (totalRecords / 50)
                Dim totalCall As Integer = 0

                If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                    totalCall = CInt(totalCallD) + 1
                Else
                    totalCall = CInt(totalCallD)
                End If

                If IsNothing(paginations) Then
                    paginations = New VetInfoModel.VetInfoPagination
                End If

                For i As Integer = 1 To totalCall
                    paginations.page = i
                    paginations.recordsPerPage = 50
                    paginations.sortBy = "tratDtInizio"
                    paginations.sortType = "ASC"

                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)

                    If strPagination <> "" Then
                        strFilter &= "&"
                    End If

                    strResp = callVetInfo(Link_Registro_dei_Trattamenti_Da_Validare, "GET", strFilter & strPagination)

                    If i = 1 Then
                        objResp_RegistroTratt = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamentiDaValidare_Response))(strResp)

                    Else
                        Dim objTempR_RegistroTratt = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroTrattamentiDaValidare_Response))(strResp)
                        objResp_RegistroTratt.data.AddRange(objTempR_RegistroTratt.data)

                    End If

                Next

            End If

            If Not IsNothing(objResp_RegistroTratt.data) AndAlso objResp_RegistroTratt.data.Count > 0 Then
                objResp_RegistroTratt.data.Sort(Function(tratt1, tratt2)
                                                    Return tratt1.presrigaDtFineTrattamento < tratt2.presrigaDtFineTrattamento
                                                End Function)

                Return objResp_RegistroTratt.data
            Else
                Return Nothing
            End If

        Else
            Return Nothing
        End If

    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="presrigaNumero">Chiave riga Medicinale della Prescrizione</param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function RegistroSomministrazioni(ByVal presrigaNumero As String,
                                             Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                             Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of RegistroSomministrazioni_Response)
        Dim objResp_RegistroSomm As VetInfoResponseModel.VetInfoResponse(Of RegistroSomministrazioni_Response)

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_Registro_delle_Somministrazioni, "GET",
                                  "presrigaNumero=" & presrigaNumero & "&" &
                                  strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_Registro_delle_Somministrazioni, "GET",
                                  "presrigaNumero=" & presrigaNumero)
        End If

        objResp_RegistroSomm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroSomministrazioni_Response))(strResp)

        If IsNothing(objResp_RegistroSomm.errors) OrElse objResp_RegistroSomm.errors.Count = 0 Then
            Dim totalRecords = CInt(objResp_RegistroSomm.pagination.totalRecords)

            If totalRecords > 10 Then
                Dim totalCallD As Double = (totalRecords / 50)
                Dim totalCall As Integer = 0

                If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                    totalCall = CInt(totalCallD) + 1
                Else
                    totalCall = CInt(totalCallD)
                End If

                If IsNothing(paginations) Then
                    paginations = New VetInfoModel.VetInfoPagination
                End If

                For i As Integer = 1 To totalCall
                    paginations.page = i
                    paginations.recordsPerPage = 50
                    paginations.sortBy = "somDtSomministrazione"
                    paginations.sortType = "ASC"

                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)

                    If strPagination <> "" Then
                        strFilter &= "&"
                    End If

                    strResp = callVetInfo(Link_Registro_delle_Somministrazioni, "GET",
                                          "presrigaNumero=" & presrigaNumero & "&" &
                                          strFilter & strPagination)

                    If i = 1 Then
                        objResp_RegistroSomm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroSomministrazioni_Response))(strResp)

                    Else
                        Dim objTempR_RegistroSomm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of RegistroSomministrazioni_Response))(strResp)
                        objResp_RegistroSomm.data.AddRange(objTempR_RegistroSomm.data)

                    End If

                Next

            End If

            If Not IsNothing(objResp_RegistroSomm.data) AndAlso objResp_RegistroSomm.data.Count > 0 Then
                Return objResp_RegistroSomm.data
            Else
                Return Nothing
            End If

        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="somNumero">Chiave Somministrazione</param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function ElencoScarichi_Somministrazione(ByVal somNumero As String,
                                                    Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                                    Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of ScarichiPerSomministrazione_Response)
        Dim objResp_ElencoScarichi_Somm As VetInfoResponseModel.VetInfoResponse(Of ScarichiPerSomministrazione_Response)

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_ElencoScarichi_PerSomministrazione, "GET",
                                  "somNumero=" & somNumero & "&" & strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_ElencoScarichi_PerSomministrazione, "GET",
                                  "somNumero=" & somNumero)
        End If

        objResp_ElencoScarichi_Somm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of ScarichiPerSomministrazione_Response))(strResp)

        If IsNothing(objResp_ElencoScarichi_Somm.errors) OrElse objResp_ElencoScarichi_Somm.errors.Count = 0 Then
            Dim totalRecords = CInt(objResp_ElencoScarichi_Somm.pagination.totalRecords)

            If totalRecords > 10 Then

                Dim totalCallD As Double = (totalRecords / 20)
                Dim totalCall As Integer = 0

                If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                    totalCall = CInt(totalCallD) + 1
                Else
                    totalCall = CInt(totalCallD)
                End If

                If IsNothing(paginations) Then
                    paginations = New VetInfoModel.VetInfoPagination
                End If

                For i As Integer = 1 To totalCall
                    paginations.page = i
                    paginations.recordsPerPage = 20
                    paginations.sortBy = ""
                    paginations.sortType = ""

                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)

                    If strPagination <> "" Then
                        strFilter &= "&"
                    End If

                    strResp = callVetInfo(Link_ElencoScarichi_PerSomministrazione, "GET",
                                          "somNumero=" & somNumero & "&" &
                                          strFilter & strPagination)

                    If i = 1 Then
                        objResp_ElencoScarichi_Somm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of ScarichiPerSomministrazione_Response))(strResp)

                    Else
                        Dim objTempR_ElencoScarichi_Somm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of ScarichiPerSomministrazione_Response))(strResp)
                        objResp_ElencoScarichi_Somm.data.AddRange(objTempR_ElencoScarichi_Somm.data)

                    End If

                Next

            End If

            If Not IsNothing(objResp_ElencoScarichi_Somm.data) AndAlso objResp_ElencoScarichi_Somm.data.Count > 0 Then
                Return objResp_ElencoScarichi_Somm.data
            Else
                Return Nothing
            End If

        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="somNumero">Chiave Somministrazione</param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function ElencoAnimali_Somministrazione(ByVal somNumero As String,
                                                   Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                                   Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of CapiPerSomministrazione_Response)
        Dim objResp_ElencoCapi_Somm As VetInfoResponseModel.VetInfoResponse(Of CapiPerSomministrazione_Response)

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_ElencoCapi_PerSomministrazione, "GET",
                                  "somNumero=" & somNumero & "&" &
                                  strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_ElencoCapi_PerSomministrazione, "GET",
                                  "somNumero=" & somNumero)
        End If

        objResp_ElencoCapi_Somm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of CapiPerSomministrazione_Response))(strResp)

        If IsNothing(objResp_ElencoCapi_Somm.errors) OrElse objResp_ElencoCapi_Somm.errors.Count = 0 Then
            Dim totalRecords = CInt(objResp_ElencoCapi_Somm.pagination.totalRecords)

            If totalRecords > 10 Then
                Dim totalCallD As Double = (totalRecords / 100)
                Dim totalCall As Integer = 0

                If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                    totalCall = CInt(totalCallD) + 1
                Else
                    totalCall = CInt(totalCallD)
                End If

                If IsNothing(paginations) Then
                    paginations = New VetInfoModel.VetInfoPagination
                End If

                For i As Integer = 1 To totalCall
                    paginations.page = i
                    paginations.recordsPerPage = 100
                    paginations.sortBy = ""
                    paginations.sortType = ""

                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)

                    If strPagination <> "" Then
                        strFilter &= "&"
                    End If

                    strResp = callVetInfo(Link_ElencoCapi_PerSomministrazione, "GET",
                                          "somNumero=" & somNumero & "&" &
                                          strFilter & strPagination)

                    If i = 1 Then
                        objResp_ElencoCapi_Somm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of CapiPerSomministrazione_Response))(strResp)

                    Else
                        Dim objTempR_ElencoCapi_Somm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of CapiPerSomministrazione_Response))(strResp)
                        objResp_ElencoCapi_Somm.data.AddRange(objTempR_ElencoCapi_Somm.data)

                    End If

                Next

            End If

            If Not IsNothing(objResp_ElencoCapi_Somm.data) OrElse objResp_ElencoCapi_Somm.data.Count > 0 Then
                Return objResp_ElencoCapi_Somm.data
            Else
                Return Nothing
            End If

        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="listPresrigaNumero">possibile inserire piu' presrigaNumero</param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function Leggi_ScortaProdotto_perSomministrazione(ByVal listPresrigaNumero As List(Of String),
                                                             Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                                             Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of ScortaProdotto_Somministrazione)
        Dim listRegistroScortaProdotto As New List(Of ScortaProdotto_Somministrazione)

        Try
            Dim batchSize As Integer = 10
            Dim currentBatch As New List(Of String)
            Dim groupListPresrigaNumero As New List(Of String)
            If listPresrigaNumero.Count <= batchSize Then
                groupListPresrigaNumero.Add(String.Join(",", listPresrigaNumero))
            Else
                listPresrigaNumero.ForEach(Sub(numero)
                                               currentBatch.Add(numero)
                                               If currentBatch.Count = batchSize Then
                                                   groupListPresrigaNumero.Add(String.Join(",", currentBatch))
                                                   currentBatch.Clear()
                                               End If
                                           End Sub)

            End If

            Dim getAllData As Boolean = False
            If paginations Is Nothing AndAlso filters Is Nothing Then
                getAllData = True
            End If

            For Each groupNumero In groupListPresrigaNumero
                Dim strResp As String = ""

                If getAllData Then
                    strResp = callVetInfo(Link_Registro_Scorta_Prodotto_perSomministrazione, "GET", "presrigaNumero=" & groupNumero)
                Else
                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)
                    If strPagination <> "" Then strFilter &= "&"

                    strResp = callVetInfo(Link_Registro_Scorta_Prodotto_perSomministrazione, "GET",
                                          "presrigaNumero=" & groupNumero & "&" & strFilter & strPagination)
                End If

                Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of ScortaProdotto_Somministrazione))(strResp)
                If IsNothing(objResp.errors) OrElse objResp.errors.Count = 0 Then
                    Dim totalRecords = CInt(objResp.pagination.totalRecords)

                    If totalRecords > 10 Then
                        Dim totalCallD As Double = (totalRecords / 20)
                        Dim totalCall As Integer = 0

                        If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                            totalCall = CInt(totalCallD) + 1
                        Else
                            totalCall = CInt(totalCallD)
                        End If

                        If IsNothing(paginations) Then paginations = New VetInfoModel.VetInfoPagination

                        For i As Integer = 1 To totalCall
                            paginations.page = i
                            paginations.recordsPerPage = 20
                            paginations.sortBy = ""
                            paginations.sortType = "ASC"

                            Dim strPagination = getStrPagination(paginations)
                            Dim strFilter = getStrFilters(filters)
                            If strPagination <> "" Then strFilter &= "&"

                            strResp = callVetInfo(Link_Registro_Scorta_Prodotto_perSomministrazione, "GET",
                                                  "presrigaNumero=" & groupNumero & "&" & strFilter & strPagination)

                            If i = 1 Then
                                objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of ScortaProdotto_Somministrazione))(strResp)
                            Else
                                Dim tempObjResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of ScortaProdotto_Somministrazione))(strResp)
                                objResp.data.AddRange(tempObjResp.data)

                            End If
                        Next
                    End If

                End If

                If Not IsNothing(objResp.data) AndAlso objResp.data.Count > 0 Then listRegistroScortaProdotto.AddRange(objResp.data)
            Next

        Catch ex As Exception
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                      "Errore in Leggi_ScortaProdotto_perSomministrazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True))
            Return Nothing
        End Try

        Return listRegistroScortaProdotto

    End Function


    ''' <summary>
    ''' Prova per tirare fuori le giacenze dei farmaci nel magazzino dell'azienda
    ''' </summary>
    ''' <param name="presrigaNumero">Chiave riga Medicinale della Prescizione</param>
    ''' <param name="regscoAziendaCodice"></param>
    ''' <param name="regscoProprietarioIdFiscale"></param>
    ''' <returns></returns>
    Public Sub Ricava_FarmaciInMagazzino(ByVal presrigaNumero As String,
                                         ByVal regscoAziendaCodice As String,
                                         ByVal regscoProprietarioIdFiscale As String)
        Dim strresp1 As String
        Dim strresp2 As String
        strresp1 = callVetInfo("/ws/1.1/trattamenti/disponibilita/inserimento/search", "GET",
                                  "presrigaNumero=" & presrigaNumero)

        strresp2 = callVetInfo("/ws/1.1/trattamenti/disponibilita/search", "GET",
                               "regscoAziendaCodice=" & regscoAziendaCodice &
                               "&regscoProprietarioIdFiscale=" & regscoProprietarioIdFiscale)

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Azienda_Codice"></param>
    ''' <param name="Proprietario_IdFiscale"></param>
    ''' <param name="filters"></param>
    ''' <param name="paginations"></param>
    ''' <returns></returns>
    Public Function RegistroDisponibilitaMedicinali(ByVal Azienda_Codice As String,
                                                    ByVal Proprietario_IdFiscale As String,
                                                    Optional ByVal filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                                    Optional ByVal paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of DisponibilitaMedicinale)
        Dim objResp_RegistroMedicinali As VetInfoResponseModel.VetInfoResponse(Of DisponibilitaMedicinale)

        Dim getAllData As Boolean = False
        If paginations Is Nothing AndAlso filters Is Nothing Then
            getAllData = True
        End If

        Dim strResp As String
        If Not getAllData Then
            Dim strPagination = getStrPagination(paginations)
            Dim strFilter = getStrFilters(filters)

            If strPagination <> "" Then
                strFilter &= "&"
            End If

            strResp = callVetInfo(Link_Registro_delle_Disponibilita_Medicinali, "GET",
                                  "regscoAziendaCodice=" & Azienda_Codice & "&" &
                                    "regscoProprietarioIdFiscale=" & Proprietario_IdFiscale & "&" &
                                    strFilter & strPagination)
        Else
            strResp = callVetInfo(Link_Registro_delle_Disponibilita_Medicinali, "GET",
                                  "regscoAziendaCodice=" & Azienda_Codice & "&" &
                                    "regscoProprietarioIdFiscale=" & Proprietario_IdFiscale)
        End If

        objResp_RegistroMedicinali = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of DisponibilitaMedicinale))(strResp)

        If IsNothing(objResp_RegistroMedicinali.errors) OrElse objResp_RegistroMedicinali.errors.Count = 0 Then
            Dim totalRecords = CInt(objResp_RegistroMedicinali.pagination.totalRecords)

            If totalRecords > 10 Then
                Dim totalCallD As Double = (totalRecords / 50)
                Dim totalCall As Integer = 0

                If CDbl(totalCallD - CInt(totalCallD)) > 0 Then
                    totalCall = CInt(totalCallD) + 1
                Else
                    totalCall = CInt(totalCallD)
                End If

                If IsNothing(paginations) Then
                    paginations = New VetInfoModel.VetInfoPagination
                End If

                For i As Integer = 1 To totalCall
                    paginations.page = i
                    paginations.recordsPerPage = 50
                    paginations.sortBy = "regscoNumero"
                    paginations.sortType = "ASC"

                    Dim strPagination = getStrPagination(paginations)
                    Dim strFilter = getStrFilters(filters)

                    If strPagination <> "" Then
                        strFilter &= "&"
                    End If

                    strResp = callVetInfo(Link_Registro_delle_Disponibilita_Medicinali, "GET",
                                          "regscoAziendaCodice=" & Azienda_Codice & "&" &
                                            "regscoProprietarioIdFiscale=" & Proprietario_IdFiscale & "&" &
                                            strFilter & strPagination)

                    If i = 1 Then
                        objResp_RegistroMedicinali = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of DisponibilitaMedicinale))(strResp)
                    Else
                        Dim objTempR_RegistroSomm = JsonConvert.DeserializeObject(Of VetInfoResponseModel.VetInfoResponse(Of DisponibilitaMedicinale))(strResp)
                        objResp_RegistroMedicinali.data.AddRange(objTempR_RegistroSomm.data)
                    End If
                Next
            End If

            If Not IsNothing(objResp_RegistroMedicinali.data) AndAlso objResp_RegistroMedicinali.data.Count > 0 Then
                Return objResp_RegistroMedicinali.data
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If

    End Function

#End Region

#Region "Chiamate Scrittura"

    ''' <summary>
    ''' Creazione di un'Indicazione Terapeutica (nuova prescrizione) partendo da un Protocollo Terapeutico (prescrizione esistente).
    ''' </summary>
    ''' <param name="Pres_Numero">Numero della prescrizione da cui creare il trattamento</param>
    ''' <param name="Pres_Capi">Lista di Identificativi (Matricole) dei capi </param>
    ''' <returns></returns>
    Public Function Insert_Trattamento_Protocollo_Terapeutico(ByVal piva As String,
                                                              ByVal saCod As Integer,
                                                              ByVal staNum As Integer,
                                                              ByVal idagenda As Integer,
                                                              ByVal Pres_Numero As String,
                                                              ByVal Pres_Capi As List(Of String)) As CreateTrattamentoFromProtocollo_Response
        Dim resp As CreateTrattamentoFromProtocollo_Response

        Dim logMsg As String = ""
        Dim logEsito As String = "KO"

        Dim dataToSend As New InsertTrattamento_ProtTera(Pres_Numero, Pres_Capi)
        Dim strToSend As String = JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None)
        Dim jobjToSend = JsonConvert.DeserializeObject(strToSend)

        Dim strResp As String = callVetInfoWrite(Link_Inserimento_Trattamento_Protocollo_Terapeutico, jobjToSend, "POST")

        Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of InsertTrattamento_ProtTera_Response))(strResp)

        If Not IsNothing(objResp.data) AndAlso objResp.data.Count = 1 Then
            logEsito = "OK"
            resp = New CreateTrattamentoFromProtocollo_Response(True, objResp.data(0).presNumero, objResp.data(0).presrigaNumeri, objResp.data(0).presrigaProdottiFamiglieAic)
        ElseIf Not IsNothing(objResp) Then
            logMsg = objResp.GetWarningOrErrorMessage
            resp = New CreateTrattamentoFromProtocollo_Response(False, logMsg)
        Else
            logMsg = "Errore non specificato durante la creazione del trattamento da protocollo."
            resp = New CreateTrattamentoFromProtocollo_Response(False, logMsg)
        End If

        LogVIExport(piva, saCod, staNum, idagenda, logMsg, logEsito, strToSend, strResp, objP_Server)

        Return resp

    End Function

    ''' <summary>
    ''' Creazione di un'Indicazione Terapeutica (nuova prescrizione) partendo da un Protocollo Terapeutico (prescrizione esistente).
    ''' </summary>
    ''' <param name="Pres_Numero">Numero della prescrizione da cui creare il trattamento</param>
    ''' <param name="Pres_Capi">Lista di Identificativi (Matricole) dei capi </param>
    ''' <returns></returns>
    Public Function Insert_Trattamento_Protocollo_Terapeutico(ByVal piva As String,
                                                              ByVal saCod As Integer,
                                                              ByVal staNum As Integer,
                                                              ByVal idagenda As Integer,
                                                              ByVal Pres_Numero As String,
                                                              ByVal Pres_Capi As List(Of Tuple(Of String, String))) As CreateTrattamentoFromProtocollo_Response
        Dim resp As CreateTrattamentoFromProtocollo_Response

        Dim logMsg As String = ""
        Dim logEsito As String = "KO"

        Dim listaCapiToSend = Pres_Capi.
            GroupBy(Function(c) c.Item1).
            SelectMany(Function(r) r.GroupBy(Function(c) c.Item2).
                    Select(Function(c) New presCapo(r.Key, c.Key, c.Count))
                ).ToList

        Dim dataToSend As New InsertTrattamento_ProtTera(Pres_Numero, listaCapiToSend)
        Dim strToSend As String = JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None)
        Dim jobjToSend = JsonConvert.DeserializeObject(strToSend)

        Dim strResp As String = callVetInfoWrite(Link_Inserimento_Trattamento_Protocollo_Terapeutico, jobjToSend, "POST")

        Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of InsertTrattamento_ProtTera_Response))(strResp)

        If Not IsNothing(objResp.data) AndAlso objResp.data.Count = 1 Then
            logEsito = "OK"
            resp = New CreateTrattamentoFromProtocollo_Response(True, objResp.data(0).presNumero, objResp.data(0).presrigaNumeri, objResp.data(0).presrigaProdottiFamiglieAic)
        ElseIf Not IsNothing(objResp) Then
            logMsg = objResp.GetWarningOrErrorMessage
            resp = New CreateTrattamentoFromProtocollo_Response(False, logMsg)
        Else
            logMsg = "Errore non specificato durante la creazione del trattamento da protocollo."
            resp = New CreateTrattamentoFromProtocollo_Response(False, logMsg)
        End If

        LogVIExport(piva, saCod, staNum, idagenda, logMsg, logEsito, strToSend, strResp, objP_Server)

        Return resp

    End Function

    ''' <summary>
    ''' Inserimento di una Somministrazione in un Trattamento in corso.
    ''' </summary>
    ''' <param name="RegSco_Numero">Numero registro di scarico da cui creare la somministrazione</param>
    ''' <param name="Quantitativo"></param>
    ''' <param name="Specie_Codice"></param>
    ''' <param name="Cat_ClassyFarm"></param>
    ''' <param name="Note"></param>
    ''' <returns></returns>
    Public Function Insert_Trattamento_Semplificato(ByVal RegSco_Numero As String,
                                                    ByVal Quantitativo As Double,
                                                    ByVal Specie_Codice As String,
                                                    ByVal Cat_ClassyFarm As String,
                                                    ByVal Note As String) As Boolean
        Dim resp As Boolean = False

        Dim dataToSend As New InsertTrattamento_Semplificato(RegSco_Numero, Quantitativo, Specie_Codice, Cat_ClassyFarm, Note)
        Dim strToSend As String = JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None)
        Dim jobjToSend = JsonConvert.DeserializeObject(strToSend)

        Dim strResp As String = callVetInfoWrite(Link_Inserimento_Trattamento_Semplificato, jobjToSend, "POST")

        Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of String))(strResp)

        If IsNothing(strResp) Then
            resp = True
        End If

        Return resp

    End Function

    ''' <summary>
    ''' Inserimento di un Trattamento da Protocollo Terapeutico monofarmaco.
    ''' </summary>
    ''' <param name="Pres_Numero"></param>
    ''' <param name="Tratt_Inizio"></param>
    ''' <param name="Tratt_Fine"></param>
    ''' <param name="Pres_Capi"></param>
    ''' <param name="RegSco_Scarichi"></param>
    ''' <param name="Note"></param>
    ''' <returns></returns>
    Public Function Insert_Trattamento_Protocollo_Semplificato(dataToSend As InsertTrattamento_Protocollo_Semplificato)
        Dim resp As Boolean = False

        'Dim dataToSend As New InsertTrattamento_Protocollo_Semplificato(Pres_Numero, Tratt_Inizio, Tratt_Fine, Pres_Capi, RegSco_Scarichi, Note)
        Dim strToSend As String = JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None)
        Dim jobjToSend = JsonConvert.DeserializeObject(strToSend)

        Dim strResp As String = callVetInfoWrite(Link_Inserimento_Trattamento_Protocollo_Semplificato, jobjToSend, "POST")

        Dim objResp = JsonConvert.DeserializeObject(Of JObject)(strResp)

        If IsNothing(strResp) Then
            resp = True
        End If

        Return resp

    End Function

    ''' <summary>
    ''' Inserimento o chiusura di Trattamenti senza esclusione di capi e con una sola Somministrazione.
    ''' </summary>
    ''' <param name="PresRiga_Numero">Numero riga della Prescrizione da cui creare la Somministrazione</param>
    ''' <param name="Tratt_Inizio"></param>
    ''' <param name="Tratt_Fine"></param>
    ''' <param name="RegSco_Scarichi">Lista Registri di Scarichi.</param>
    ''' <param name="Tratt_Note"></param>
    ''' <returns></returns>
    Public Function Insert_Trattamenti_Massivo(ByVal PresRiga_Numero As String,
                                               ByVal Tratt_Inizio As Date,
                                               ByVal Tratt_Fine As Date,
                                               ByVal RegSco_Scarichi As List(Of Tuple(Of String, Double)),
                                               Optional ByVal Tratt_Note As String = "") As Boolean
        Dim resp As Boolean = False

        Dim dataToSend As New InsertTrattamento_Massivo(PresRiga_Numero, Tratt_Inizio, Tratt_Fine, RegSco_Scarichi, Tratt_Note)
        Dim strToSend As String = JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None)
        Dim jobjToSend = JsonConvert.DeserializeObject(strToSend)

        Dim strResp As String = callVetInfoWrite(Link_Inserimento_Trattamento_Massivo, jobjToSend, "POST")

        Dim objResp = JsonConvert.DeserializeObject(Of JObject)(strResp)

        If IsNothing(strResp) Then
            resp = True
        End If

        Return resp

    End Function

    ''' <summary>
    ''' Permette di inserire una Somministrazione in un Trattamento nuovo (e crearlo) o in corso.
    ''' </summary>
    ''' <param name="PresRiga_Numero"></param>
    ''' <param name="Somm_Data"></param>
    ''' <param name="RegSco_Scarichi"></param>
    ''' <param name="Somm_Note"></param>
    ''' <returns></returns>
    Public Function Insert_Somministrazione_In_Trattamento(ByVal piva As String,
                                                           ByVal saCod As Integer,
                                                           ByVal staNum As Integer,
                                                           ByVal idAgenda As Integer,
                                                           ByVal PresRiga_Numero As String,
                                                           ByVal Somm_Data As Date,
                                                           ByVal RegSco_Scarichi As List(Of Tuple(Of String, Double)),
                                                           Optional ByVal Somm_Note As String = "",
                                                           Optional ByVal Capi_ToDelete As List(Of presCapoRiduzione) = Nothing) As InsertTrattamento_Response
        Dim resp As InsertTrattamento_Response

        Dim logMsg As String = ""
        Dim logEsito As String = "KO"

        Dim dataToSend As New Insert_Somministrazione(PresRiga_Numero, Somm_Data, RegSco_Scarichi, Note:=Somm_Note, Riduz_Capi:=Capi_ToDelete)
        Dim strToSend As String = JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None,
                                                              settings:=New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})
        Dim jobjToSend = JsonConvert.DeserializeObject(strToSend)

        Dim strResp As String = callVetInfoWrite(Link_Inserimento_Somministrazione, jobjToSend, "POST")

        Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of Insert_Somministrazione_Response))(strResp)

        If Not IsNothing(objResp.data) AndAlso objResp.data.Count = 1 Then
            logEsito = "OK"
            resp = New InsertTrattamento_Response(True, objResp.data(0).tratNumero, objResp.data(0).somNumero)
        ElseIf Not IsNothing(objResp) Then
            logMsg = objResp.GetWarningOrErrorMessage
            resp = New InsertTrattamento_Response(False, logMsg)
        Else
            logMsg = "Errore non specificato durante l'inserimento della somministrazione nel trattamento."
            resp = New InsertTrattamento_Response(False, logMsg)
        End If

        LogVIExport(piva, saCod, staNum, idAgenda, logMsg, logEsito, strToSend, strResp, objP_Server)

        Return resp

    End Function

    Public Function Reopen_Trattamento_Prescrizione(ByVal Tratt_Numero As String) As Boolean
        Dim resp As Boolean = False

        Dim dataToSend As New JObject From {{"tratNumero", Tratt_Numero}}
        Dim strToSend As String = dataToSend.ToString

        Dim strResp As String = callVetInfoWrite(Link_Riapertura_Trattamento_Prescrizione, strToSend, "POST")

        If IsNothing(strResp) Then resp = True

        Return resp

    End Function

    Public Function Reopen_Trattamento_Protocollo(ByVal Pres_Numero As String) As Boolean
        Dim resp As Boolean = False

        Dim dataToSend As New JObject From {{"presNumero", Pres_Numero}}
        Dim strToSend As String = dataToSend.ToString

        Dim strResp As String = callVetInfoWrite(Link_Riapertura_Trattamento_Protocollo, strToSend, "POST")
        Dim objResp As JObject = JsonConvert.DeserializeObject(Of JObject)(strResp)

        If IsNothing(objResp) Then resp = True

        Return resp

    End Function

    Public Function Close_Trattamento_Protocollo_Terapeutico(ByVal piva As String,
                                                             ByVal saCod As Integer,
                                                             ByVal staNum As Integer,
                                                             ByVal idagenda As Integer,
                                                             ByVal Pres_Numero As String,
                                                             ByVal Tratt_Fine As String,
                                                             ByVal Tratt_Stato As String,
                                                             ByVal Note As String) As CloseTrattamento_Response
        Dim resp As CloseTrattamento_Response

        Dim logMsg As String = ""
        Dim logEsito As String = "KO"

        Dim dataToSend As New CloseTrattamento_ProtTera(Pres_Numero, Tratt_Fine, Tratt_Stato, Note)

        Dim jobjToSend = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None))

        Dim strResp As String = callVetInfoWrite(Link_Chiusura_Trattamento_Protocollo_Terapeutico, jobjToSend, "POST")

        Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of CloseTrattamento_ProtTera_Response))(strResp)

        If Not IsNothing(objResp.data) AndAlso objResp.data.Count = 1 Then
            logEsito = "OK"
            resp = New CloseTrattamento_Response(True)
        ElseIf Not IsNothing(objResp) Then
            logMsg = objResp.GetWarningOrErrorMessage
            resp = New CloseTrattamento_Response(False, logMsg)
        Else
            logMsg = "Errore non specificato durante la chiusura del trattamento."
            resp = New CloseTrattamento_Response(False, logMsg)
        End If

        LogVIExport(piva, saCod, staNum, idagenda, logMsg, logEsito, jobjToSend.ToString, strResp, objP_Server)

        Return resp

    End Function

    Public Function Close_Trattamento_Prescrizione(ByVal piva As String,
                                                   ByVal saCod As Integer,
                                                   ByVal staNum As Integer,
                                                   ByVal idagenda As Integer,
                                                   ByVal Tratt_Numero As String,
                                                   ByVal Tratt_Fine As String,
                                                   ByVal Tratt_Stato As String,
                                                   ByVal Note As String) As CloseTrattamento_Response
        Dim resp As CloseTrattamento_Response

        Dim logMsg As String = ""
        Dim logEsito As String = "KO"

        Dim dataToSend As New CloseTrattamento_Pres(Tratt_Numero, Tratt_Fine, Tratt_Stato, Note)
        Dim jobjToSend = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None))


        Dim strResp As String = callVetInfoWrite(Link_Chiusura_Trattamento, jobjToSend, "POST")
        Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of CloseTrattamento_Pres_Response))(strResp)

        If Not IsNothing(objResp.data) AndAlso objResp.data.Count = 1 Then
            logEsito = "OK"
            resp = New CloseTrattamento_Response(True)
        ElseIf Not IsNothing(objResp) Then
            logMsg = objResp.GetWarningOrErrorMessage
            resp = New CloseTrattamento_Response(False, logMsg)
        Else
            logMsg = "Errore non specificato durante la chiusura del trattamento."
            resp = New CloseTrattamento_Response(False, logMsg)
        End If

        LogVIExport(piva, saCod, staNum, idagenda, logMsg, logEsito, jobjToSend.ToString, strResp, objP_Server)

        Return resp

    End Function

    ''' <summary>
    ''' Chiude un singolo Trattamento
    ''' </summary>
    ''' <param name="Pres_Numero"></param>
    ''' <param name="Tratt_Numero"></param>
    ''' <param name="Tratt_Fine"></param>
    ''' <param name="Tratt_Stato"></param>
    ''' <param name="Note"></param>
    ''' <returns></returns>
    Public Function Close_Trattamento_Protocollo_Terapeutico_Medicinale(ByVal Pres_Numero As String,
                                                                        ByVal Tratt_Numero As String,
                                                                        ByVal Tratt_Fine As String,
                                                                        ByVal Tratt_Stato As String,
                                                                        ByVal Note As String) As CloseTrattamento_Response
        Dim listTrattamenti As New List(Of CloseTrattamento_ProtTera_Medicinale.Trattamento) From
            {New CloseTrattamento_ProtTera_Medicinale.Trattamento(Tratt_Numero, Tratt_Fine)}

        Dim dataToSend As New CloseTrattamento_ProtTera_Medicinale(Pres_Numero, listTrattamenti, Tratt_Stato, Note)
        Dim jobjToSend = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dataToSend, formatting:=Newtonsoft.Json.Formatting.None))

        Dim strResp As String = callVetInfoWrite(Link_Chiusura_Trattamento_Protocollo_Terapeutico_Medicinale, jobjToSend, "POST")
        Dim objResp = JsonConvert.DeserializeObject(Of VetInfoResponse(Of CloseTrattamento_ProtTera_Medicinale_Response))(strResp)

        If Not IsNothing(objResp.data) AndAlso objResp.data.Count = 1 Then
            Return New CloseTrattamento_Response(True)
        ElseIf Not IsNothing(objResp) Then
            Return New CloseTrattamento_Response(False, objResp.GetWarningOrErrorMessage)
        Else
            Return New CloseTrattamento_Response(False, "Errore non specificato durante la chiusura del trattamento.")
        End If
    End Function

    Public Function Delete_Trattamento_Prescrizione(ByVal Tratt_Numero As String,
                                                    ByVal Note As String) As Boolean
        Dim resp As Boolean = False

        Dim dataToSend As New JObject From {{"tratNumero", Tratt_Numero}, {"tratNote", Note}}
        Dim strToSend As String = dataToSend.ToString

        Dim strResp As String = callVetInfoWrite(Link_Annullamento_Trattamento, strToSend, "POST")

        If IsNothing(strResp) Then resp = True

        Return resp

    End Function

    Public Function Delete_Trattamento_Protocollo_Terapeutico(ByVal Pres_Numero As String,
                                                              ByVal Note As String) As Boolean
        Dim resp As Boolean = False

        Dim dataToSend As New JObject From {{"presNumero", Pres_Numero}, {"tratNote", Note}}
        Dim strToSend As String = dataToSend.ToString

        Dim strResp As String = callVetInfoWrite(Link_Annullamento_Trattamento_Protocollo_Terapeutico, strToSend, "POST")
        Dim objResp As JObject = JsonConvert.DeserializeObject(Of JObject)(strResp)

        If IsNothing(objResp) Then resp = True

        Return resp

    End Function

#End Region

#Region "Response Classes"

    Public Class CreateTrattamentoFromProtocollo_Response
        Public result As Boolean
        Public presNumero As String
        Public presRigaNumeri As List(Of String)
        Public famiglieAic As List(Of String)
        Public errore As String

        Public Sub New(res As Boolean, err As String)
            result = res
            presNumero = ""
            errore = err
        End Sub

        Public Sub New(res As Boolean, presNumero As String,
                       Optional presRigaNumeri As List(Of String) = Nothing,
                       Optional famiglieAic As List(Of String) = Nothing)
            result = res
            Me.presNumero = presNumero
            Me.presRigaNumeri = presRigaNumeri
            Me.famiglieAic = famiglieAic
            errore = ""
        End Sub
    End Class

    Public Class InsertTrattamento_Response
        Public result As Boolean
        Public trattNumero As String
        Public sommNumero As String
        Public errore As String

        Public Sub New(res As Boolean, err As String)
            result = res
            trattNumero = ""
            sommNumero = ""
            errore = err
        End Sub

        Public Sub New(res As Boolean, trattNumero As String, sommNumero As String)
            result = res
            Me.trattNumero = trattNumero
            Me.sommNumero = sommNumero
            errore = ""
        End Sub
    End Class

    Public Class CloseTrattamento_Response
        Public result As Boolean
        Public errore As String

        Public Sub New(res As Boolean, err As String)
            result = res
            errore = err
        End Sub

        Public Sub New(res As Boolean)
            result = res
            errore = ""
        End Sub
    End Class

#End Region

End Class
