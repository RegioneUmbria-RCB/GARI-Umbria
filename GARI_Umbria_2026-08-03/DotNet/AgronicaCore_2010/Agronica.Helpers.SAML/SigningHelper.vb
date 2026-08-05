Imports System
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Xml
Imports System.Xml

Namespace Italia.Spid.Authentication.Saml
    Public Class XmlSigningHelper
        Shared Function SignXMLDoc(ByVal doc As XmlDocument, ByVal certificate As X509Certificate2, ByVal referenceUri As String) As XmlElement
            If doc Is Nothing Then
                Throw New ArgumentNullException("The doc parameter can't be null")
            End If

            If certificate Is Nothing Then
                Throw New ArgumentNullException("The cert2 parameter can't be null")
            End If

            If String.IsNullOrWhiteSpace(referenceUri) Then
                Throw New ArgumentNullException("The referenceUri parameter can't be null or empty")
            End If

            'Dim privateKey As AsymmetricAlgorithm


            Dim cspParams As CspParameters
            Try
                cspParams = New CspParameters(24) With {.KeyContainerName = "XML_DSIG_RSA_KEY"}
            Catch ex As Exception
                Throw New Exception("cspParams: " & ex.Message)
            End Try

            Dim privateKey As RSACryptoServiceProvider
            Try
                privateKey = New RSACryptoServiceProvider(cspParams)
            Catch ex As Exception
                Throw New Exception("privateKey: " & ex.Message)
            End Try
            Dim a As String
            Try
                a = certificate.PrivateKey.ToXmlString(True)
            Catch ex As Exception
                Throw New Exception("a: " & ex.Message)
            End Try

            Try
                privateKey.FromXmlString(a)
            Catch ex As Exception
                Throw New Exception("privateKey.FromXmlString(a): " & ex.Message)
            End Try

            Try
                Dim signedXml As SignedXml = New SignedXml(doc) With {
                .SigningKey = privateKey
            }


                signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"
                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl
                Dim reference As Reference = New Reference With {
                    .DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256"
                }
                reference.AddTransform(New XmlDsigEnvelopedSignatureTransform())
                reference.AddTransform(New XmlDsigExcC14NTransform())
                reference.Uri = "#" & referenceUri
                signedXml.AddReference(reference)
                Dim keyInfo As KeyInfo = New KeyInfo()
                keyInfo.AddClause(New KeyInfoX509Data(certificate))
                signedXml.KeyInfo = keyInfo
                signedXml.ComputeSignature()
                Dim signature As XmlElement = signedXml.GetXml()
                Return signature
            Catch ex As Exception
                Throw New Exception("SignedError: " & ex.Message)
            End Try

            'privateKey = certificate.PrivateKey


            Return Nothing

        End Function

        Shared Function VerifySignature(ByVal signedDocument As XmlDocument) As Boolean
            If True Then

                If signedDocument Is Nothing Then
                    Throw New ArgumentNullException("The signedDocument parameter can't be null")
                End If

                Try
                    Dim signedXml As SignedXml = New SignedXml(signedDocument)
                    Dim nodeList As XmlNodeList = If((signedDocument.GetElementsByTagName("ds:Signature").Count > 0), signedDocument.GetElementsByTagName("ds:Signature"), If((signedDocument.GetElementsByTagName("ns2:Signature").Count > 0), signedDocument.GetElementsByTagName("ns2:Signature"), signedDocument.GetElementsByTagName("Signature")))
                    signedXml.LoadXml(CType(nodeList(0), XmlElement))
                    Return signedXml.CheckSignature()
                Catch ex As Exception
                    Throw New Exception("Error on VerifySignature", ex)
                End Try
            End If
        End Function
        Shared Function VerifySignature(ByVal signedDocument As XmlDocument, chiave As RSA) As Boolean
            If True Then

                If signedDocument Is Nothing Then
                    Throw New ArgumentNullException("The signedDocument parameter can't be null")
                End If

                Try
                    Dim signedXml As SignedXml = New SignedXml(signedDocument)
                    Dim nodeList As XmlNodeList = If((signedDocument.GetElementsByTagName("ds:Signature").Count > 0), signedDocument.GetElementsByTagName("ds:Signature"), If((signedDocument.GetElementsByTagName("ns2:Signature").Count > 0), signedDocument.GetElementsByTagName("ns2:Signature"), signedDocument.GetElementsByTagName("Signature")))
                    signedXml.LoadXml(CType(nodeList(0), XmlElement))
                    Return signedXml.CheckSignature(chiave)
                Catch ex As Exception
                    Throw New Exception("Error on VerifySignature", ex)
                End Try
            End If
        End Function
    End Class
End Namespace
