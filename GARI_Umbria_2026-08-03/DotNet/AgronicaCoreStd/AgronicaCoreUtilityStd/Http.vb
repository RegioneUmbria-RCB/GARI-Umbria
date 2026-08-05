

Imports System.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Web
Imports System.Net.Http
Imports System.Net.Security

Public Class Http
    Dim certificato As X509Certificate

    Public Shared Function UriUnescape(ByVal uriToUnescape As String) As String
        Return Uri.UnescapeDataString(uriToUnescape)
    End Function


    Public Function chiamaWS(ByVal request As String,
                              ByVal certificatoPath As String,
                              ByVal webService As String,
                              ByVal contentType As String,
                              ByVal method As String,
                              ByVal accept As String,
                              ByVal soapAction As String,
                              Optional ByVal CustomHeaders As WebHeaderCollection = Nothing,
                             Optional ByVal TimeOut As Integer = -1
                    ) As String

        'certificato = X509Certificate2.CreateFromCertFile("C:\cooperazione.sian.it.crt")



        Dim resp As String
        If String.IsNullOrEmpty(certificatoPath) Then
            resp = GetResponse(webService, request, soapAction, contentType, method, accept, False, Nothing, CustomHeaders, TimeOut)
        Else

            If certificatoPath.Contains("https://") Then
                certificato = GetServerCertificate(certificatoPath)
                'certificato = GetCertFromUrl(certificatoPath)
            Else
                certificato = X509Certificate.CreateFromCertFile(certificatoPath)
            End If

            System.Net.ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf customXertificateValidation)

            'Dim messaggio As String = xSoggSiRPV.sSoggSiRPV("", "")
            resp = GetResponse(webService, request, soapAction, contentType, method, accept, True, certificato, CustomHeaders, TimeOut)

        End If
        Return resp
    End Function

    Public Function GetServerCertificate(ByVal certificatoPath As String) As X509Certificate

        Dim certificate As X509Certificate = Nothing
        Dim handler = New HttpClientHandler With {
            .ServerCertificateCustomValidationCallback =
                Function(arg1, arg2, arg3, arg4)
                    certificate = New X509Certificate2(arg2.GetRawCertData())
                    Return True
                End Function
        }

        Dim client = New HttpClient(handler)
        Dim response As HttpResponseMessage = client.GetAsync(certificatoPath).GetAwaiter().GetResult()

        Return certificate

    End Function

    Public Function GetCertFromUrl(ByVal certificatoPath As String) As X509Certificate

        'Do webrequest to get info on secure site
        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(certificatoPath), HttpWebRequest)
        Try

            System.Net.ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf customXertificateValidation)
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            response.Close()

            'retrieve the ssl cert and assign it to an X509Certificate object
            Return request.ServicePoint.Certificate
        Catch ex As Exception
            Return request.ServicePoint.Certificate
        End Try

    End Function

    Private Function customXertificateValidation(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPoicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
        Select Case sslPoicyErrors
            Case System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors

            Case System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch

            Case System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable

        End Select
        Return True
    End Function

    Private Function GetResponse(ByVal sSoapUri As String,
                                 ByVal sSoapMessage As String,
                                 ByVal sSoapAction As String,
                                 ByVal contentType As String,
                                 ByVal method As String,
                                 ByVal accept As String,
                                 ByVal bAttachCert As Boolean,
                                 _cert As X509Certificate,
                                 ByVal CustomHeaders As WebHeaderCollection,
                                 Optional ByVal timeOut As Integer = -1) As String
        Try
            Dim oHttpReq As HttpWebRequest = DirectCast(WebRequest.CreateDefault(New Uri(sSoapUri)), HttpWebRequest)
            oHttpReq.ContentType = contentType
            oHttpReq.Method = method
            oHttpReq.Accept = accept
            If timeOut > 0 Then
                oHttpReq.Timeout = timeOut
            Else
                oHttpReq.Timeout = 300000
            End If



            If CustomHeaders IsNot Nothing Then

                For Each key As String In CustomHeaders.AllKeys
                    Dim value As String = CustomHeaders(key)
                    Select Case key.ToLower
                        Case "accept"
                            oHttpReq.Accept = value
                        Case Else
                            oHttpReq.Headers.Add(key, value)
                    End Select

                Next

            End If


            If sSoapAction <> "" Then
                oHttpReq.Headers.Add("soapaction", sSoapAction)
            End If


            oHttpReq.ServicePoint.Expect100Continue = False  ' <-- I've tried this both on and off to no avail

            If bAttachCert Then
                oHttpReq.ClientCertificates.Add(_cert)
            End If

            If method <> "GET" Then

                Dim mencoding As New System.Text.UTF8Encoding()
                Dim bytes As Byte() = mencoding.GetBytes(sSoapMessage)

                oHttpReq.ContentLength = bytes.Length

                Dim oReqStream As Stream = oHttpReq.GetRequestStream()
                oReqStream.Write(bytes, 0, bytes.Length)  '<-- This string is in just over 4K in length
                oReqStream.Flush()
                oReqStream.Close()
            End If

            Dim oHttpResp As HttpWebResponse = TryCast(oHttpReq.GetResponse(), HttpWebResponse)
            oHttpReq = Nothing

            Dim responseString As String
            Using stream As Stream = oHttpResp.GetResponseStream()
                Dim reader As New StreamReader(stream, Encoding.UTF8)
                responseString = reader.ReadToEnd()
            End Using

            Return responseString

        Catch ex As WebException

            Dim MessaggioErrore As String
            MessaggioErrore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[AgronicaCoreUtility.http] : " & MessaggioErrore, ex)

        End Try
    End Function

End Class
