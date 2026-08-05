Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class IFNextApi

    Public Function GetBaseUrl(ByVal gias As Boolean) As String

        '*** API IRRIFRAME ***
        '*** https://www.irriframe.it/irriframeapi/api ***
        '*** http://www2.irriframe.it/irriframeapi/api ***
        Dim IFUrl As String = "http://www3.irriframe.it/irriframeapi/api"
        Dim IFDebugUrl As String = "http://localhost:4900/IrriframeAPI/api"

        '*** API GIAS ***
        Dim GIASUrl As String = "http://localhost/WS_ImportaGias_2014/WS_Ext.svc"
        Dim GIASDebugUrl As String = "http://localhost:52563/WS_ImportaGias_2014/WS_Ext.svc"

        Dim baseUrl As String = If(gias, GIASUrl, IFUrl)

        If Debugger.IsAttached Then
            baseUrl = If(gias, GIASDebugUrl, IFDebugUrl)
        End If

        Return baseUrl

    End Function
    Public Function GetToken() As String

        Dim errmsg As String = "Errore WS..."

        Dim body_token As New JObject
        body_token("email") = "agronica@irriframe.it"
        body_token("password") = "av2608agro@^"

        Dim token As String = ""

        Try

            Dim resp_token = ApiResponse("token", "POST", body_token, False)

            If resp_token.StatusCode = HttpStatusCode.OK Then
                token = resp_token.Content("token")
            Else
                errmsg = resp_token.Content("Message").ToString.Trim()
            End If

        Catch ex As Exception
            token = ""
        End Try

        Return token

    End Function

    Private Function ApiResponse(ByVal service As String, ByVal method As String, ByVal body As JObject, Optional ByVal token As String = "", Optional ByVal gias As Boolean = True) As AgronicaCoreUtility.Http.JsonRestResponse

        Dim hdrs As WebHeaderCollection = Nothing
        If Not String.IsNullOrEmpty(token) Then
            hdrs = New WebHeaderCollection
            hdrs.Add("Authorization", "Bearer " & token)
        End If

        Dim strBody As String = ""
        If body IsNot Nothing Then
            strBody = body.ToString()
        End If

        Dim http As New AgronicaCoreUtility.Http
        Dim baseUrl As String = GetBaseUrl(gias)
        Dim resp = http.CallWS_RestSharp_JSON(baseUrl, service, method, strBody, hdrs)

        Return resp

    End Function

    Private Function MessageOutput(ByVal response As AgronicaCoreUtility.Http.JsonRestResponse, ByRef errmsg As String) As JObject

        Dim objRes As JObject = Nothing

        If ({HttpStatusCode.InternalServerError, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.NotAcceptable, HttpStatusCode.ExpectationFailed}).Contains(response.StatusCode) Then

            '401 UNAUTHORIZED Chiamante non abilitato o User non proprietario del Plot
            '400 BAD REQUEST Chiamata incompleta (Mancanza QueryString...)
            '404 NOTFOUND appezzamento non esistente
            '417 EXPECTATIONFAILED: “Coltura scaduta
            '406 NotAcceptable {"Message":"dati mancanti per il calcolo"}
            errmsg = response.Content("message").ToString().Trim()

        ElseIf response.Content IsNot Nothing Then

            objRes = New JObject
            objRes("Status") = 0
            objRes("Message") = JsonConvert.SerializeObject(response.Content)

        End If

        Return objRes

    End Function

    Public Function RegIrrigation(ByVal body As JObject) As String

        Dim errmsg As String = ""
        Dim token As String = GetToken()
        Dim objRes As JObject = Nothing

        If Not String.IsNullOrEmpty(token) Then
            If body IsNot Nothing Then
                Try
                    Dim resp = ApiResponse("regirrigation", "POST", body, token, False)
                    objRes = MessageOutput(resp, errmsg)
                Catch ex As Exception
                    objRes = Nothing
                End Try

            End If
        End If

        If objRes Is Nothing Then
            objRes = New JObject
            objRes("Status") = -1
            objRes("Message") = errmsg
        End If

        Return objRes.ToString

    End Function

    Public Function RegUser(ByVal token As String, ByVal body As JObject) As String

        Dim errmsg As String = ""
        Dim objRes As JObject = Nothing

        If Not String.IsNullOrEmpty(token) Then
            If body IsNot Nothing Then
                Try
                    Dim resp = ApiResponse("RegisterUser", "POST", body, token)
                    objRes = MessageOutput(resp, errmsg)
                Catch ex As Exception
                    objRes = Nothing
                End Try
            End If
        End If

        If objRes Is Nothing Then
            objRes = New JObject
            objRes("Status") = -1
            objRes("Message") = errmsg
        End If

        Return objRes.ToString

    End Function

    Public Function RegFarm(ByVal token As String, ByVal body As JObject) As String

        Dim errmsg As String = ""
        Dim objRes As JObject = Nothing

        If Not String.IsNullOrEmpty(token) Then
            If body IsNot Nothing Then
                Try
                    Dim resp = ApiResponse("RegisterFarm", "POST", body, token)
                    objRes = MessageOutput(resp, errmsg)
                Catch ex As Exception
                    objRes = Nothing
                End Try
            End If
        End If

        If objRes Is Nothing Then
            objRes = New JObject
            objRes("Status") = -1
            objRes("Message") = errmsg
        End If

        Return objRes.ToString

    End Function

    Public Function RegPlot(ByVal token As String, ByVal body As JObject) As String

        Dim errmsg As String = ""
        Dim objRes As JObject = Nothing

        If Not String.IsNullOrEmpty(token) Then
            If body IsNot Nothing Then
                Try
                    Dim resp = ApiResponse("RegisterPlot", "POST", body, token)
                    objRes = MessageOutput(resp, errmsg)
                Catch ex As Exception
                    objRes = Nothing
                End Try
            End If
        End If

        If objRes Is Nothing Then
            objRes = New JObject
            objRes("Status") = -1
            objRes("Message") = errmsg
        End If

        Return objRes.ToString

    End Function

    Public Function RegCrop(ByVal token As String, ByVal body As JObject) As String

        Dim errmsg As String = ""
        Dim objRes As JObject = Nothing

        If Not String.IsNullOrEmpty(token) Then
            If body IsNot Nothing Then
                Try
                    Dim resp = ApiResponse("RegisterCrop", "POST", body, token)
                    objRes = MessageOutput(resp, errmsg)
                Catch ex As Exception
                    objRes = Nothing
                End Try
            End If
        End If

        If objRes Is Nothing Then
            objRes = New JObject
            objRes("Status") = -1
            objRes("Message") = errmsg
        End If

        Return objRes.ToString

    End Function

End Class
