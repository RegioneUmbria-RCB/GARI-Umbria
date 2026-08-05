Imports System.Net
Imports AgronicaCoreDTOStd.InData.importazioni

Public Class Util

    Public Function CheckRequest(objRequest As CoreWS_Generic(Of ImportDemetra), ByRef errorMessage As String) As Boolean

        errorMessage = ""

        If objRequest Is Nothing OrElse objRequest.InData Is Nothing Then

            errorMessage = My.Resources.AgronicaCoreDemetraBIZ.JsonNonValorizzato
            Return False

        Else
            If objRequest.InData.CUAA = "" Then

                errorMessage = My.Resources.AgronicaCoreDemetraBIZ.CuaaNonValorizzato
                Return False

            ElseIf objRequest.InData.dati = "" Then

                errorMessage = My.Resources.AgronicaCoreDemetraBIZ.DatiNonValorizzati
                Return False

            End If
        End If

        Return True

    End Function

    Public Function CallEndpoint(apiKey As String, url As String, dataToSend As ImportDemetra, ByRef errorMessage As String, Optional timeoutCallEndPoint As Integer = -1) As Boolean

        Dim http_Request = New AgronicaCoreUtility.Http
        Dim hdr = New WebHeaderCollection()

        If apiKey.Contains("Bearer") Then
            hdr.Add(HttpRequestHeader.Authorization, apiKey)
        Else
            hdr.Add(HttpRequestHeader.Authorization, "APIKEY " & apiKey)
        End If

        Dim resp = http_Request.chiamaWS_RestShapr(dataToSend,
                                                       "",
                                                       url,
                                                       "application/json",
                                                       RestSharp.Method.POST,
                                                       "application/json",
                                                       "", hdr, , timeoutCallEndPoint, True)

        If resp.IsSuccessful = False OrElse resp.ResponseStatus <> RestSharp.ResponseStatus.Completed OrElse resp.StatusCode <> HttpStatusCode.OK Then

            If resp.StatusCode = HttpStatusCode.BadRequest Then

                errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ErroreHttpExport, "", resp.StatusCode, "", "", resp.ErrorMessage, resp.Content)

                'TODO_DT_send mail

            Else

                errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ErroreHttpExport, resp.ResponseStatus, resp.StatusCode, url, hdr(HttpRequestHeader.Authorization), resp.ErrorMessage, resp.Content)

            End If

            Return False

        End If

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
