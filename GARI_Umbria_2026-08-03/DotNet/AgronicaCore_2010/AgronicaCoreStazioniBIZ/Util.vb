Imports System.Net
Imports AgronicaCoreDTOStd.InData.importazioni
Imports Newtonsoft.Json
Imports OutData.Infragri
Imports RestSharp

Public Class Util

    Public Function GetAccessToken(endPointToken As String, grantType As String, clientId As String, clientSecret As String, scope As String) As String
        Dim client = New RestClient(endPointToken)
        Dim request = New RestRequest(Method.POST)
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded")
        request.AddParameter("grant_type", grantType)
        request.AddParameter("client_id", clientId)       ' <-- criptare
        request.AddParameter("client_secret", clientSecret)   ' <-- criptare
        request.AddParameter("scope", scope)

        Dim response = client.Execute(request)
        If response.IsSuccessful Then
            Dim json = Newtonsoft.Json.Linq.JObject.Parse(response.Content)
            Return json("access_token").ToString()
        Else
            Throw New Exception("Error token: " & response.ErrorMessage)
        End If
    End Function

    Public Function CallEndpoint(token As String,
                                 url As String,
                                 dataToSend As Dispositivo,
                                 ByRef esito As String,
                                 ByRef errorMessage As String,
                                 Optional timeoutCallEndPoint As Integer = -1) As Boolean

        Dim client = New RestClient(url)
        client.Timeout = timeoutCallEndPoint

        Dim request = New RestRequest(Method.POST)
        request.AddHeader("Content-Type", "application/json")
        request.AddHeader("Authorization", "Bearer " & token)

        Dim settings = New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
        }

        Dim json = JsonConvert.SerializeObject(dataToSend, settings)

        request.AddParameter(
            "application/json",
            json,
            ParameterType.RequestBody
        )

        Dim resp = client.Execute(request)

        If resp.IsSuccessful = False OrElse resp.ResponseStatus <> RestSharp.ResponseStatus.Completed OrElse resp.StatusCode <> HttpStatusCode.OK Then

            Select Case resp.StatusCode
                Case 400 To 499
                    esito = Util_Costanti.ESITO_BLK
                    errorMessage = String.Format(My.Resources.AgronicaCoreStazioniBIZ.DetailsExportStazioneNotFound, dataToSend.serialNumberDispositivo, dataToSend.codiceModelloDispositivo, dataToSend.codiceContratto, JsonConvert.SerializeObject(dataToSend, Formatting.Indented), resp.Content)
                Case Else
                    esito = Util_Costanti.ESITO_KO
                    errorMessage = String.Format(My.Resources.AgronicaCoreStazioniBIZ.DetailsExportGenericError, dataToSend.serialNumberDispositivo, dataToSend.codiceModelloDispositivo, dataToSend.codiceContratto, JsonConvert.SerializeObject(dataToSend, Formatting.Indented), resp.Content)
            End Select

            Return False

        End If

        errorMessage = ""
        esito = Util_Costanti.ESITO_OK
        Return True

    End Function

    Public Function CallEndPointAttachedFiles(token As String,
                                              serialNumberDispositivo As String,
                                              codiceModelloDispositivo As String,
                                              codiceContratto As String,
                                              fileName As String,
                                              fileByteArray As Byte(),
                                              url As String,
                                              ByRef esito As String,
                                              ByRef errorMessage As String) As Boolean

        Dim client = New RestClient(url)

        Dim request = New RestRequest(Method.POST)
        request.AddHeader("Authorization", "Bearer " & token)

        request.AddFile("files", fileByteArray, fileName, "application/octet-stream")

        Dim resp = client.Execute(request)

        If resp.IsSuccessful = False OrElse resp.ResponseStatus <> RestSharp.ResponseStatus.Completed OrElse resp.StatusCode <> HttpStatusCode.OK Then

            Select Case resp.StatusCode
                Case 400 To 499
                    esito = Util_Costanti.ESITO_BLK
                    errorMessage = String.Format(My.Resources.AgronicaCoreStazioniBIZ.DetailsExportAllegatoBLK, serialNumberDispositivo, codiceModelloDispositivo, codiceContratto, fileName, resp.Content)
                Case Else
                    esito = Util_Costanti.ESITO_KO
                    errorMessage = String.Format(My.Resources.AgronicaCoreStazioniBIZ.DetailsExportAllegatoKO, serialNumberDispositivo, codiceModelloDispositivo, codiceContratto, fileName, resp.Content)
            End Select

            Return False

        End If

        errorMessage = ""
        esito = Util_Costanti.ESITO_OK
        Return True

    End Function

    Public Sub Chiama_ScriviLOG(nomeRoutine As String,
                                text As String,
                                LogDirectory As String,
                                LogFileName As String,
                                objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional verificaInviaElasticSearch As Boolean = True)

        'Salvo gli originali
        Dim _LogDirectory As String = objParametri.LogDirectory
        Dim _LogFileName As String = objParametri.LogFileName

        'Sovrascrivo con quelli del GSB
        objParametri.LogDirectory = LogDirectory
        objParametri.LogFileName = LogFileName

        Dim objLog As New AgronicaCoreDataProvider.LogProvider
        objLog.Scrivi_LOG(objParametri, nomeRoutine, text, verificaInviaElasticSearch:=verificaInviaElasticSearch)
        'objLog.Scrivi_LOG(objParametri, nomeRoutine, text, verificaInviaElasticSearch:=False) 'Per demetra c'è già un tool che invia ad ES

        'Ripristino i valori
        objParametri.LogDirectory = _LogDirectory
        objParametri.LogFileName = _LogFileName

    End Sub

End Class

Public Class Util_Costanti

    ''' <summary>
    ''' esito = OK       Dati_Ricevuti = ""
    ''' </summary>
    Public Const ESITO_OK As String = "OK"

    ''' <summary>
    ''' esito = KO       Dati_Ricevuti = errore
    ''' </summary>
    Public Const ESITO_KO As String = "KO"

    ''' <summary>
    ''' esito =  BLK    Dati_Ricevuti = BLK_MESSAGE_PREFIX + <motivazione>
    ''' </summary>
    Public Const ESITO_BLK As String = "BLK"
    Public Const BLK_MESSAGE_PREFIX As String = "Disattivata manualmente per "

End Class

Public Class AttachedFile

    Public FileAllegatiDocumentiCod As Integer
    Public FileName As String
    Public FileByteArray As Byte()
    Public Contratto As String
    Public SerialNumber As String

    Public Sub New(cod As Integer, name As String, fileByte As Byte(), contratto As String, serialNumber As String)
        Me.FileAllegatiDocumentiCod = cod
        Me.FileName = name
        Me.FileByteArray = fileByte
        Me.Contratto = contratto
        Me.SerialNumber = serialNumber
    End Sub

End Class
