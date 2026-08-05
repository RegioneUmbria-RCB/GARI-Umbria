Imports Newtonsoft.Json
Imports System.Net
Imports System.Text
Imports AgronicaCoreUtility.Http
Imports AgronicaCoreDataProvider

Public Class OAuth2Controller

    Private _requester As AgronicaCoreUtility.Http

    Public Sub New(ByRef _cfg As OAuth2DataModel_Config)
        _requester = New AgronicaCoreUtility.Http()
        _cfg.oAuthMetaData = getOAuthMetadata(_cfg)
        '_cfg.Token.access_token = GetAuthCode(_cfg)
    End Sub

'    Public Function OAuth2RequestURL() As String
'
'    End Function

    Private Function getOAuthMetadata(ByVal cfg As OAuth2DataModel_Config) As Dictionary(Of String, Object)

        Try

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim resp = _requester.CallWS_RestSharp_JSON(cfg.WellKnown, "", "GET", "")
            Dim tmp = JsonConvert.SerializeObject(resp.Content)

            Return JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(tmp)

        Catch ex As Exception

            Dim MessaggioErrore As String = ex.Message  '' per ora

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Agronica.Helpers.OAuth2] : " & MessaggioErrore, ex)

            Return Nothing
        End Try

    End Function

    Public Function GetAuthCodeUrl(ByVal BaseURI As String, ByVal cfg As OAuth2DataModel_Config) As String
        Dim authUrl As String = ""
        Try

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim HttpQueryParams = New Dictionary(Of String, String)

            HttpQueryParams.Add("response_type", "code")
            HttpQueryParams.Add("scope", cfg.Scopes)
            HttpQueryParams.Add("client_id", cfg.ClientId)
            HttpQueryParams.Add("state", cfg.State)
            HttpQueryParams.Add("redirect_uri", BaseURI & "/" & cfg.Callback)


            authUrl = QueryHelper.AddQueryString(cfg.oAuthMetaData("authorization_endpoint"), HttpQueryParams)
            Return authUrl
        Catch ex As Exception
            Dim MessaggioErrore As String = ex.Message  '' per ora

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Agronica.Helpers.OAuth2] : " & MessaggioErrore, ex)

            Return authUrl
        End Try
    End Function

    Public Function GetAccessTokenUrl(ByVal cfg As OAuth2DataModel_Config) As String
        Dim AccessTokenUrl As String = ""
        Try
            Return cfg.oAuthMetaData("token_endpoint")
        Catch ex As Exception
            Dim MessaggioErrore As String
            MessaggioErrore = ex.Message  '' per ora

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Agronica.Helpers.OAuth2] : " & MessaggioErrore, ex)

            Return AccessTokenUrl
        End Try
    End Function

    Public Function BuildContentDataFromUrlQueryParams(ByVal Params As Dictionary(Of String, String)) As Byte()
        Dim strParam2Encode As String = ""
        Dim bRet As Byte() = Nothing
        Try
            strParam2Encode = QueryHelper.AddQueryString("", Params)
            bRet = Encoding.UTF8.GetBytes(strParam2Encode)
            Return bRet
        Catch ex As Exception
            Dim MessaggioErrore As String
            MessaggioErrore = ex.Message  '' per ora

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Agronica.Helpers.OAuth2] : " & MessaggioErrore, ex)

            Return bRet
        End Try
    End Function

    Public Function GetBase64EncodedClientCredentials(ByVal cfg As OAuth2DataModel_Config) As String
        Dim retVal As String = ""
        Try
            Dim creBuffer = Encoding.UTF8.GetBytes(cfg.ClientId & ":" & cfg.ClientSecret)
            retVal = Convert.ToBase64String(creBuffer)
            Return retVal
        Catch ex As Exception
            Dim MessaggioErrore As String
            MessaggioErrore = ex.Message  '' per ora

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Agronica.Helpers.OAuth2] : " & MessaggioErrore, ex)

            Return retVal
        End Try
    End Function

    Public Shared Function GetConnParametersFromDB(ByVal ChiaveConfigSiti As String, ByVal objParametri_Server As AgronicaCoreParametri) As OAuth2DataModel_Config
        Dim cfg_db As New OAuth2DataModel_Config
        Try
            Dim oReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R()
            cfg_db = JsonConvert.DeserializeObject(Of OAuth2DataModel_Config)(oReader.Leggi_Valore(6, ChiaveConfigSiti, "", "", objParametri_Server))
            Return cfg_db
        Catch ex As Exception
            Dim MessaggioErrore As String
            MessaggioErrore = ex.Message  '' per ora

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Agronica.Helpers.OAuth2] : " & MessaggioErrore, ex)

            Return cfg_db
        End Try
    End Function

End Class
