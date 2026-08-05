Imports System
Imports System.IO
Imports System.Security.Cryptography.X509Certificates

Namespace Italia.Spid.Authentication
    Module X509Helper
        Function GetCertificateFromFile(ByVal certFilePath As String, ByVal certPassword As String) As X509Certificate2
            If String.IsNullOrWhiteSpace(certFilePath) Then
                Throw New ArgumentNullException("The certFilePath parameter can't be null or empty.")
            End If

            If String.IsNullOrWhiteSpace(certPassword) Then
                Throw New ArgumentNullException("The certPassword parameter can't be null or empty.")
            End If

            If File.Exists(certFilePath) Then
                Return New X509Certificate2(certFilePath, certPassword)
            Else
                Throw New FileNotFoundException("Unable to locate certificate")
            End If
        End Function

        Function GetCertificateFromStore(ByVal storeLocation As StoreLocation, ByVal storeName As StoreName, ByVal findType As X509FindType, ByVal findValue As Object, ByVal validOnly As Boolean) As X509Certificate2
            Dim certificate As X509Certificate2 = Nothing

            If findValue Is Nothing Then
                Throw New ArgumentNullException("The findValue parameter can't be null.")
            End If

            Try
                Dim store As X509Store = New X509Store(storeName, storeLocation)
                store.Open(OpenFlags.[ReadOnly] Or OpenFlags.OpenExistingOnly)
                Dim coll As X509Certificate2Collection = store.Certificates.Find(findType, findValue.ToString(), validOnly)

                If coll.Count > 0 Then
                    certificate = coll(0)
                End If

                store.Close()

                If certificate IsNot Nothing Then
                    Return certificate
                Else
                    Throw New FileNotFoundException("Unable to locate certificate")
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Function
    End Module
End Namespace
