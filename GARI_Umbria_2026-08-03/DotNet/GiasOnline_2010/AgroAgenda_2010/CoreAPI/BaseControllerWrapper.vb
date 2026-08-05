Imports System.Net
Imports AgronicaControlli_2010

Public Class BaseControllerWrapper

    Protected _http_Request As AgronicaCoreUtility.Http = Nothing
    Protected _hdr As WebHeaderCollection = Nothing
    Protected _linkCoreAPI As String = String.Empty
    Protected _linkCoreWS As String = String.Empty

    Private _coreApiCOntrollerFActory As CoreApiControllerFactory = Nothing
    Private _currentProtol As SecurityProtocolType

    Public Sub New()

        _coreApiCOntrollerFActory = New CoreApiControllerFactory

        _http_Request = _coreApiCOntrollerFActory.Http_Request
        _hdr = _coreApiCOntrollerFActory.Headers
        _linkCoreAPI = _coreApiCOntrollerFActory.LinkCoreApi
        _linkCoreWS = _coreApiCOntrollerFActory.LinkCoreWS

    End Sub

    Protected Function MakeRestCall(endpointUrl As String, contentType As String, restMethod As RestSharp.Method, Optional requestBody As Object = Nothing) As RestSharp.IRestResponse

        Dim resp = _http_Request.chiamaWS_RestShapr(requestBody,
                                        "",
                                        endpointUrl,
                                        contentType,
                                        restMethod, "application/json", "", _hdr, , , True)

        Return resp

    End Function

End Class
