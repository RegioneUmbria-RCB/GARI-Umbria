Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports System.IO
Imports System.Text

Public Class RegVino
    Dim certificato As X509Certificate
    Dim httpCaller As New AgronicaCoreUtility.Http

    'Public Function ChiamataWSAsync_Test(ByVal messaggio As String) As String
    '    Return httpCaller.chiamaWS(messaggio, Utility.CertificateFile_Test, Utility.WebServiceAsyncAddress_Test, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
    'End Function

    'Public Function ChiamataWSAsync(ByVal messaggio As String) As String
    '    Return httpCaller.chiamaWS(messaggio, Utility.CertificateFile, Utility.WebServiceAsyncAddress_Test, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
    'End Function

    'Public Function ChiamataWSSync_Test(ByVal messaggio As String) As String
    '    Return httpCaller.chiamaWS(messaggio, Utility.CertificateFile_Test, Utility.WebServiceSyncAddress_Test, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
    'End Function

    'Public Function ChiamataWSSync(ByVal messaggio As String) As String
    '    Return httpCaller.chiamaWS(messaggio, Utility.CertificateFile, Utility.WebServiceSyncAddress_Test, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
    'End Function

    Public Function ChiamataWSSync(ByVal messaggio As String, url As String, CertificateFile As String) As String
        If CertificateFile IsNot Nothing AndAlso CertificateFile <> "" Then
            Return httpCaller.chiamaWS(messaggio, CertificateFile, url, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
        Else
            Return httpCaller.chiamaWS(messaggio, Utility.CertificateFile_Test, url, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
        End If
    End Function

    Public Function ChiamataWSAsync(ByVal messaggio As String, url As String, CertificateFile As String) As String
        If CertificateFile IsNot Nothing AndAlso CertificateFile <> "" Then
            Return httpCaller.chiamaWS(messaggio, CertificateFile, url, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
        Else
            Return httpCaller.chiamaWS(messaggio, Utility.CertificateFile, url, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
        End If
    End Function

End Class
