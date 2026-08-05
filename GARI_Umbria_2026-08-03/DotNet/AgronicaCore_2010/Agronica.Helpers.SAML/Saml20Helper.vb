Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Xml
Imports System.Xml.Linq
Imports System.Xml.Serialization
Imports Agronica.Helpers.SAML.Italia.Spid.Authentication.IdP
Imports Agronica.Helpers.SAML.Italia.Spid.Authentication.Schema
Imports Developers.Italia.SPID.SAML.Schema
Imports Italia.Spid.Authentication.IdP
Imports Italia.Spid.Authentication.Schema

Module XElementExtensions
    ''' <summary>
    ''' Safe getter for an XElement's attribute.
    ''' </summary>
    ''' <param name="element">The element on which the attribute is researched. Usually the object on which the function is called</param>
    ''' <param name="attributeName">The name of the attribute to get</param>
    ''' <param name="defaultValue">Vaslue to return if the Attribute is not present</param>
    ''' <returns>The value of the specified attribute or <tt>defaultValue</tt> if the attribute is not present</returns>
    <Extension()>
    Public Function GetAttribute(element As XElement, attributeName As String, Optional defaultValue As String = "") As String
        Return If(element.Attribute(attributeName) Is Nothing, defaultValue, element.Attribute(attributeName).Value)
    End Function
End Module

Namespace Italia.Spid.Authentication.Saml

    Public Class AgroSamlConfig
        Public Property ComparisonType As AuthnContextComparisonType
        Public Property SignAssertion As Boolean
        Public Property VerifyResponse As Boolean
        Public Property SpidLevel As Integer
    End Class

    Public Class SamlHelper
        Private Const VALUE_NOT_AVAILABLE As String = "N/A"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uuid"></param>
        ''' <param name="destination"></param>
        ''' <param name="consumerServiceURL"></param>
        ''' <param name="securityLevel"></param>
        ''' <param name="certificate"></param>
        ''' <param name="identityProvider"></param>
        ''' <param name="enviroment"></param>
        ''' <param name="AssertionConsumerServiceURL"></param>
        ''' <param name="comparisonType">Minimum per regione umbria SPID, exact per Zespri</param>
        ''' <returns></returns>
        Public Shared Function BuildAuthnPostRequest(
            ByVal uuid As String,
            ByVal destination As String,
            ByVal consumerServiceURL As String,
            ByVal securityLevel As Integer,
            ByVal certificate As X509Certificate2,
            ByVal identityProvider As IdentityProvider,
            ByVal enviroment As Integer,
            AssertionConsumerServiceURL As String,
            ByVal AgroConfig As AgroSamlConfig
        ) As String

            If String.IsNullOrWhiteSpace(uuid) Then
                Throw New ArgumentNullException("The uuid parameter can't be null or empty.")
            End If

            If String.IsNullOrWhiteSpace(destination) Then
                Throw New ArgumentNullException("The destination parameter can't be null or empty.")
            End If

            If String.IsNullOrWhiteSpace(consumerServiceURL) Then
                Throw New ArgumentNullException("The consumerServiceURL parameter can't be null or empty.")
            End If

            If certificate Is Nothing AndAlso AgroConfig.SignAssertion Then
                Throw New ArgumentNullException("The certificate parameter can't be null.")
            End If

            If identityProvider Is Nothing Then
                Throw New ArgumentNullException("The identityProvider parameter can't be null.")
            End If

            If enviroment < 0 Then
                Throw New ArgumentNullException("The enviroment parameter can't be less than zero.")
            End If

            If AssertionConsumerServiceURL Is Nothing Then
                Throw New ArgumentNullException("The AssertionConsumerServiceURL parameter can't be null.")
            End If

            Dim RequestedAuthnContextItems As String() = Nothing
            Dim RequestedAuthnContextItemsName As ItemsChoiceType7()
            Select Case AgroConfig.SpidLevel
                Case 1
                    RequestedAuthnContextItems = New String() {"urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport"}
                    RequestedAuthnContextItemsName = New ItemsChoiceType7() {ItemsChoiceType7.AuthnContextClassRef}
                Case 2
                    RequestedAuthnContextItems = New String() {"urn:oasis:names:tc:SAML:2.0:ac:classes:SecureRemotePassword", "urn:oasis:names:tc:SAML:2.0:ac:classes:Smartcard"}
                    RequestedAuthnContextItemsName = New ItemsChoiceType7() {ItemsChoiceType7.AuthnContextClassRef, ItemsChoiceType7.AuthnContextClassRef}
                Case 3
                    'TODO
            End Select

            Dim now As DateTime = DateTime.UtcNow
            Dim authnRequest As AuthnRequestType = New AuthnRequestType With {
                .ID = "_" & uuid,
                .Version = "2.0",
                .IssueInstant = identityProvider.Now(now),
                .Destination = destination,
                .IsPassive = False,
                .IsPassiveSpecified = True,
                .ProtocolBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
                .AssertionConsumerServiceURL = AssertionConsumerServiceURL,
                .ForceAuthn = (securityLevel > 1),
                .ForceAuthnSpecified = (securityLevel > 1),
                .Issuer = New NameIDType With {
                    .Value = consumerServiceURL.Trim(),
                    .NameQualifier = consumerServiceURL
                },
                .NameIDPolicy = New NameIDPolicyType With {
                    .Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient"
                },
                .Conditions = New ConditionsType With {
                    .NotBefore = identityProvider.NotBefore(now),
                    .NotBeforeSpecified = True,
                    .NotOnOrAfter = identityProvider.After(now.AddMinutes(10)),
                    .NotOnOrAfterSpecified = True
                },
            .RequestedAuthnContext = New RequestedAuthnContextType With {
                    .Comparison = AgroConfig.ComparisonType,
                    .ComparisonSpecified = True,
                    .ItemsElementName = RequestedAuthnContextItemsName,
                    .Items = RequestedAuthnContextItems
                }
            }


            '.RequestedAuthnContext = New RequestedAuthnContextType With {
            '    .Comparison = AuthnContextComparisonType.minimum,
            '    .ComparisonSpecified = True,
            '    .ItemsElementName = New ItemsChoiceType7() {ItemsChoiceType7.AuthnContextClassRef},
            '    .Items = New String() {"https://www.spid.gov.it/SpidL" & securityLevel.ToString()}
            '}
            Dim ns As XmlSerializerNamespaces = New XmlSerializerNamespaces()
            ns.Add("saml2p", "urn:oasis:names:tc:SAML:2.0:protocol")
            ns.Add("saml2", "urn:oasis:names:tc:SAML:2.0:assertion")
            Dim stringWriter As StringWriter = New StringWriter()
            Dim settings As XmlWriterSettings = New XmlWriterSettings With {
                .OmitXmlDeclaration = True,
                .Indent = True,
                .Encoding = Encoding.UTF8
            }
            Dim responseWriter As XmlWriter = XmlTextWriter.Create(stringWriter, settings)
            Dim responseSerializer As XmlSerializer = New XmlSerializer(authnRequest.[GetType]())
            responseSerializer.Serialize(responseWriter, authnRequest, ns)
            responseWriter.Close()
            Dim samlString As String = stringWriter.ToString()
            stringWriter.Close()
            Dim doc As New XmlDocument()
            doc.LoadXml(samlString)
            If doc Is Nothing Then
                Throw New ArgumentNullException("The doc parameter can't be null")
            End If

            If certificate Is Nothing AndAlso AgroConfig.SignAssertion Then
                Throw New ArgumentNullException("The certificate parameter can't be null")
            End If

            If String.IsNullOrWhiteSpace("_" & uuid) Then
                Throw New ArgumentNullException("The uuid parameter can't be null or empty")
            End If

            If AgroConfig.SignAssertion Then
                Dim signature As XmlElement = XmlSigningHelper.SignXMLDoc(doc, certificate, "_" & uuid)
                doc.DocumentElement.InsertBefore(signature, doc.DocumentElement.ChildNodes(1))
            End If

            Return Convert.ToBase64String(Encoding.UTF8.GetBytes("<?xml version=""1.0"" encoding=""UTF-8""?>" & doc.OuterXml))
        End Function

        Public Shared Function GetAuthnResponse(
            ByVal base64Response As String,
            certificate As X509Certificate2,
            ByVal AgroConfig As AgroSamlConfig
        ) As IdpAuthnResponse
            Dim idpResponse As String

            If String.IsNullOrEmpty(base64Response) Then
                Throw New ArgumentNullException("The base64Response parameter can't be null or empty.")
            End If

            Try
                idpResponse = Encoding.UTF8.GetString(Convert.FromBase64String(base64Response))
            Catch ex As Exception
                Throw New ArgumentException("Unable to converto base64 response to ascii string.", ex)
            End Try

            Try
                Dim xml As XmlDocument = New XmlDocument With {
                    .PreserveWhitespace = True
                }
                xml.LoadXml(idpResponse)

                'verifico la firma se richiesto..
                If AgroConfig.VerifyResponse Then
                    Dim chiave As RSA = ottieniChiavePubblica(certificate)
                    If Not XmlSigningHelper.VerifySignature(xml, chiave) Then
                        Throw New Exception("Unable to verify the signature of the IdP response.")
                    End If
                End If

                Return GetIdpAuthnResponse(idpResponse)
            Catch ex As Exception
                Throw New ArgumentException("Unable to read AttributeStatement attributes from SAML2 document.", ex)
            End Try
        End Function

        Private Shared Function GetIdpAuthnResponse(idpResponse As String) As IdpAuthnResponse
            Dim xdoc As XDocument = New XDocument()
            xdoc = XDocument.Parse(idpResponse)

            Dim assertionId As String = VALUE_NOT_AVAILABLE
            Dim assertionIssueInstant As DateTimeOffset = DateTimeOffset.MinValue
            Dim assertionVersion As String = VALUE_NOT_AVAILABLE
            Dim assertionIssuer As String = VALUE_NOT_AVAILABLE
            Dim subjectNameId As String = VALUE_NOT_AVAILABLE
            Dim subjectConfirmationMethod As String = VALUE_NOT_AVAILABLE
            Dim subjectConfirmationDataInResponseTo As String = VALUE_NOT_AVAILABLE
            Dim subjectConfirmationDataNotOnOrAfter As DateTimeOffset = DateTimeOffset.MinValue
            Dim subjectConfirmationDataRecipient As String = VALUE_NOT_AVAILABLE
            Dim conditionsNotBefore As DateTimeOffset = DateTimeOffset.MinValue
            Dim conditionsNotOnOrAfter As DateTimeOffset = DateTimeOffset.MinValue
            Dim audience As String = VALUE_NOT_AVAILABLE
            Dim authnStatementAuthnInstant As DateTimeOffset = DateTimeOffset.MinValue
            Dim authnStatementSessionIndex As String = VALUE_NOT_AVAILABLE
            Dim spidUserInfo As Dictionary(Of String, String) = New Dictionary(Of String, String)()

            ' Values from responseElement
            Dim responseElement As XElement = xdoc.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}Response").Single()
            Dim destination = responseElement.GetAttribute("Destination", VALUE_NOT_AVAILABLE)
            Dim id = responseElement.GetAttribute("ID", VALUE_NOT_AVAILABLE)
            Dim inResponseTo = responseElement.GetAttribute("InResponseTo", VALUE_NOT_AVAILABLE)
            Dim issueInstant = DateTimeOffset.Parse(responseElement.GetAttribute(
                    "IssueInstant",
                    DateTimeOffset.MinValue.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                ))
            Dim version = responseElement.GetAttribute("Version", VALUE_NOT_AVAILABLE)
            Dim issuer = responseElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}Issuer").Single().Value.Trim()

            ' Values from statusElement
            Dim StatusElement As XElement = responseElement.Descendants("{urn:oasis:names:tc:SAML:2.0:protocol}Status").Single()
            Dim statusCodeValue = StatusElement.Descendants("{urn:oasis:names:tc:SAML:2.0:protocol}StatusCode").
                First().GetAttribute("Value", VALUE_NOT_AVAILABLE).
                Replace("urn:oasis:names:tc:SAML:2.0:status:", "")
            Dim statusCodeInnerValue = GetStatusCodeInnerValue(StatusElement)
            Dim statusMessage As String = GetStatusMessage(StatusElement)
            Dim statusDetail As String = GetStatusDetails(StatusElement)

            If statusCodeValue = "Success" Then
                Dim assertionElement As XElement = responseElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}Assertion").Single()
                ReadAssertions(assertionElement, assertionId, assertionIssueInstant, assertionVersion, assertionIssuer)
                ReadSubject(
                    assertionElement, subjectNameId, subjectConfirmationMethod, subjectConfirmationDataInResponseTo,
                    subjectConfirmationDataNotOnOrAfter, subjectConfirmationDataRecipient
                )
                ReadConditions(assertionElement, conditionsNotBefore, conditionsNotOnOrAfter, audience)

                'Dim authnStatementElement As XElement = assertionElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}AuthnStatement").Single()
                'authnStatementAuthnInstant = DateTimeOffset.Parse(authnStatementElement.Attribute("AuthnInstant").Value)
                'authnStatementSessionIndex = authnStatementElement.Attribute("SessionIndex").Value

                For Each attribute As XElement In xdoc.Descendants("{urn:oasis:names:tc:SAML:2.0:assertion}AttributeStatement").Elements()
                    spidUserInfo.Add(
                        attribute.GetAttribute("Name"),
                        attribute.Elements().
                            FirstOrDefault(Function(a) a.Name = "{urn:oasis:names:tc:SAML:2.0:assertion}AttributeValue").
                            Value.Trim()
                    )
                Next
            End If

            Return New IdpAuthnResponse(
                destination, id, inResponseTo, issueInstant, version, issuer, statusCodeValue, statusCodeInnerValue,
                statusMessage, statusDetail, assertionId, assertionIssueInstant, assertionVersion, assertionIssuer,
                subjectNameId, subjectConfirmationMethod, subjectConfirmationDataInResponseTo,
                subjectConfirmationDataNotOnOrAfter, subjectConfirmationDataRecipient, conditionsNotBefore,
                conditionsNotOnOrAfter, audience, authnStatementAuthnInstant, authnStatementSessionIndex, spidUserInfo
            )
        End Function

        Private Shared Function GetStatusCodeInnerValue(StatusElement As XElement) As String
            Dim statusCodeElements = StatusElement.Descendants("{urn:oasis:names:tc:SAML:2.0:protocol}StatusCode")
            If statusCodeElements.Count() > 1 Then
                Return statusCodeElements.Last().GetAttribute("Value", VALUE_NOT_AVAILABLE).
                        Replace("urn:oasis:names:tc:SAML:2.0:status:", "")
            Else
                Return VALUE_NOT_AVAILABLE
            End If
        End Function

        Private Shared Function GetStatusMessage(StatusElement As XElement) As String
            If StatusElement.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}StatusMessage").SingleOrDefault() IsNot Nothing Then
                Return If(StatusElement.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}StatusMessage").SingleOrDefault().Value, VALUE_NOT_AVAILABLE)
            Else
                Return VALUE_NOT_AVAILABLE
            End If
        End Function

        Private Shared Function GetStatusDetails(StatusElement As XElement) As String
            If StatusElement.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}StatusDetail").SingleOrDefault() IsNot Nothing Then
                Return If(StatusElement.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}StatusDetail").SingleOrDefault().Value, VALUE_NOT_AVAILABLE)
            Else
                Return VALUE_NOT_AVAILABLE
            End If
        End Function

        Private Shared Sub ReadAssertions(
            ByVal assertionElement As XElement, ByRef assertionId As String,
            ByRef assertionIssueInstant As DateTimeOffset, ByRef assertionVersion As String,
            ByRef assertionIssuer As String
        )
            assertionId = assertionElement.GetAttribute("ID", VALUE_NOT_AVAILABLE)
            assertionIssueInstant = DateTimeOffset.Parse(assertionElement.GetAttribute(
                    "IssueInstant",
                    DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                ))
            assertionVersion = assertionElement.GetAttribute("Version", VALUE_NOT_AVAILABLE)
            assertionIssuer = assertionElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}Issuer").Single().Value.Trim()
        End Sub

        Private Shared Sub ReadSubject(
            ByVal assertionElement As XElement, ByRef subjectNameId As String,
            ByRef subjectConfirmationMethod As String,
            ByRef subjectConfirmationDataInResponseTo As String,
            ByRef subjectConfirmationDataNotOnOrAfter As DateTimeOffset,
            ByRef subjectConfirmationDataRecipient As String
        )
            Dim subjectElement As XElement = assertionElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}Subject").Single()
            subjectNameId = subjectElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}NameID").Single().Value.Trim()
            subjectConfirmationMethod = subjectElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}SubjectConfirmation").Single().
                    GetAttribute("Method", VALUE_NOT_AVAILABLE)

            Dim confirmationDataElement As XElement = subjectElement.Descendants("{urn:oasis:names:tc:SAML:2.0:assertion}SubjectConfirmationData").Single()
            subjectConfirmationDataInResponseTo = confirmationDataElement.GetAttribute("InResponseTo", VALUE_NOT_AVAILABLE)
            subjectConfirmationDataNotOnOrAfter = DateTimeOffset.Parse(confirmationDataElement.GetAttribute(
                        "NotOnOrAfter",
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                    ))
            subjectConfirmationDataRecipient = confirmationDataElement.GetAttribute("Recipient", VALUE_NOT_AVAILABLE)
        End Sub

        Private Shared Sub ReadConditions(
            ByVal assertionElement As XElement, ByRef conditionsNotBefore As DateTimeOffset,
            ByRef conditionsNotOnOrAfter As DateTimeOffset, ByRef audience As String
        )
            Dim conditionsElement As XElement = assertionElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}Conditions").Single()
            conditionsNotBefore = DateTimeOffset.Parse(conditionsElement.GetAttribute(
                    "NotBefore",
                    DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                ))
            conditionsNotOnOrAfter = DateTimeOffset.Parse(conditionsElement.GetAttribute(
                    "NotOnOrAfter",
                    DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                ))
            audience = conditionsElement.Descendants("{urn:oasis:names:tc:SAML:2.0:assertion}Audience").Single().Value.Trim()
        End Sub

        Private Shared Function ottieniChiavePubblica(certificate As X509Certificate2) As RSA
            Return certificate.PublicKey.Key
        End Function

        Public Shared Function ValidAuthnResponse(ByVal idpAuthnResponse As IdpAuthnResponse, ByVal spidRequestId As String, ByVal route As String) As Boolean
            Return (idpAuthnResponse.InResponseTo = "_" & spidRequestId) AndAlso (idpAuthnResponse.SubjectConfirmationDataRecipient = route)
        End Function

        Public Shared Function BuildLogoutPostRequest(
            ByVal uuid As String,
            ByVal consumerServiceURL As String,
            ByVal certificate As X509Certificate2,
            ByVal identityProvider As IdentityProvider,
            ByVal subjectNameId As String,
            ByVal authnStatementSessionIndex As String,
            ByVal AgroConfig As AgroSamlConfig
        ) As String

            If String.IsNullOrWhiteSpace(uuid) Then
                Throw New ArgumentNullException("The uuid parameter can't be null or empty.")
            End If

            If String.IsNullOrWhiteSpace(consumerServiceURL) Then
                Throw New ArgumentNullException("The consumerServiceURL parameter can't be null or empty.")
            End If

            If certificate Is Nothing AndAlso AgroConfig.SignAssertion Then
                Throw New ArgumentNullException("The certificate parameter can't be null.")
            End If

            If identityProvider Is Nothing Then
                Throw New ArgumentNullException("The identityProvider parameter can't be null.")
            End If

            If String.IsNullOrWhiteSpace(subjectNameId) Then
                Throw New ArgumentNullException("The subjectNameId parameter can't be null or empty.")
            End If

            If String.IsNullOrWhiteSpace(identityProvider.SingleLogoutServiceUrl) Then
                Throw New ArgumentNullException("The LogoutServiceUrl of the identity provider is null or empty.")
            End If

            Dim now As DateTime = DateTime.UtcNow
            Dim logoutRequest As LogoutRequestType = New LogoutRequestType With {
                .ID = "_" & uuid,
                .Version = "2.0",
                .IssueInstant = identityProvider.Now(now),
                .Destination = identityProvider.EntityID,
                .Issuer = New NameIDType With {
                    .Value = consumerServiceURL.Trim(),
                    .Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity",
                    .NameQualifier = consumerServiceURL
                },
                .Item = New NameIDType With {
                    .NameQualifier = consumerServiceURL,
                    .Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient",
                    .Value = identityProvider.SubjectNameIdFormatter(subjectNameId)
                },
                .NotOnOrAfterSpecified = True,
                .NotOnOrAfter = now.AddMinutes(10),
                .Reason = "urn:oasis:names:tc:SAML:2.0:logout:user",
                .SessionIndex = New String() {authnStatementSessionIndex}
            }

            Try
                Dim ns As XmlSerializerNamespaces = New XmlSerializerNamespaces()
                ns.Add("saml2p", "urn:oasis:names:tc:SAML:2.0:protocol")
                ns.Add("saml2", "urn:oasis:names:tc:SAML:2.0:assertion")
                Dim stringWriter As StringWriter = New StringWriter()
                Dim settings As XmlWriterSettings = New XmlWriterSettings With {
                    .OmitXmlDeclaration = True,
                    .Indent = True,
                    .Encoding = Encoding.UTF8
                }
                Dim responseWriter As XmlWriter = XmlTextWriter.Create(stringWriter, settings)
                Dim responseSerializer As XmlSerializer = New XmlSerializer(logoutRequest.[GetType]())
                responseSerializer.Serialize(responseWriter, logoutRequest, ns)
                responseWriter.Close()
                Dim samlString As String = stringWriter.ToString()
                stringWriter.Close()
                Dim doc As XmlDocument = New XmlDocument()
                doc.LoadXml(samlString)

                If AgroConfig.SignAssertion Then
                    Dim signature As XmlElement = XmlSigningHelper.SignXMLDoc(doc, certificate, "_" & uuid)
                    doc.DocumentElement.InsertBefore(signature, doc.DocumentElement.ChildNodes(1))
                End If

                Return Convert.ToBase64String(Encoding.UTF8.GetBytes("<?xml version=""1.0"" encoding=""UTF-8""?>" & doc.OuterXml))
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        Public Shared Function GetLogoutResponse(ByVal base64Response As String) As IdpLogoutResponse
            Const VALUE_NOT_AVAILABLE As String = "N/A"
            Dim idpResponse As String

            If String.IsNullOrEmpty(base64Response) Then
                Throw New ArgumentNullException("The base64Response parameter can't be null or empty.")
            End If

            Try
                idpResponse = Encoding.UTF8.GetString(Convert.FromBase64String(base64Response))
            Catch ex As Exception
                Throw New ArgumentException("Unable to converto base64 response to ascii string.", ex)
            End Try

            Try
                Dim xml As XmlDocument = New XmlDocument With {
                    .PreserveWhitespace = True
                }
                xml.LoadXml(idpResponse)

                If Not XmlSigningHelper.VerifySignature(xml) Then
                    Throw New Exception("Unable to verify the signature of the IdP response.")
                End If

                Dim xdoc As XDocument = New XDocument()
                xdoc = XDocument.Parse(idpResponse)
                Dim destination As String = VALUE_NOT_AVAILABLE
                Dim id As String = VALUE_NOT_AVAILABLE
                Dim inResponseTo As String = VALUE_NOT_AVAILABLE
                Dim issueInstant As DateTimeOffset = DateTimeOffset.MinValue
                Dim version As String = VALUE_NOT_AVAILABLE
                Dim statusCodeValue As String = VALUE_NOT_AVAILABLE
                Dim statusCodeInnerValue As String = VALUE_NOT_AVAILABLE
                Dim statusMessage As String = VALUE_NOT_AVAILABLE
                Dim statusDetail As String = VALUE_NOT_AVAILABLE
                Dim responseElement As XElement = xdoc.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}LogoutResponse").Single()
                destination = responseElement.Attribute("Destination").Value
                id = responseElement.Attribute("ID").Value
                inResponseTo = responseElement.Attribute("InResponseTo").Value
                issueInstant = DateTimeOffset.Parse(responseElement.Attribute("IssueInstant").Value)
                version = responseElement.Attribute("Version").Value
                Dim issuer As String = responseElement.Elements("{urn:oasis:names:tc:SAML:2.0:assertion}Issuer").Single().Value.Trim()
                Dim StatusElement As XElement = responseElement.Descendants("{urn:oasis:names:tc:SAML:2.0:protocol}Status").Single()
                Dim statusCodeElements As IEnumerable(Of XElement) = StatusElement.Descendants("{urn:oasis:names:tc:SAML:2.0:protocol}StatusCode")
                statusCodeValue = statusCodeElements.First().Attribute("Value").Value.Replace("urn:oasis:names:tc:SAML:2.0:status:", "")
                statusCodeInnerValue = If(statusCodeElements.Count() > 1, statusCodeElements.Last().Attribute("Value").Value.Replace("urn:oasis:names:tc:SAML:2.0:status:", ""), VALUE_NOT_AVAILABLE)
                statusMessage = If(StatusElement.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}StatusMessage").SingleOrDefault().Value, VALUE_NOT_AVAILABLE)
                statusDetail = If(StatusElement.Elements("{urn:oasis:names:tc:SAML:2.0:protocol}StatusDetail").SingleOrDefault().Value, VALUE_NOT_AVAILABLE)
                Return New IdpLogoutResponse(destination, id, inResponseTo, issueInstant, version, issuer, statusCodeValue, statusCodeInnerValue, statusMessage, statusDetail)
            Catch ex As Exception
                Throw New ArgumentException("Unable to read AttributeStatement attributes from SAML2 document.", ex)
            End Try
        End Function

        Public Shared Function ValidLogoutResponse(ByVal idpLogoutResponse As IdpLogoutResponse, ByVal spidRequestId As String) As Boolean
            Return (idpLogoutResponse.InResponseTo = "_" & spidRequestId)
        End Function
    End Class
End Namespace
