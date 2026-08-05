
Imports System
Imports Microsoft.Web.Services3.Design
Imports Microsoft.Web.Services3
Imports System.Xml
Imports System.Xml.XPath
Imports System.Collections.Generic
Imports Microsoft.Web.Services3.Security.Tokens
Imports Microsoft.Web.Services3.Security



Namespace Agronica




    '##############################################################################################
    Public Class TokenNames
        Public Const NamespaceURI As String = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"
        Public Const UtilityURI As String = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"
    End Class





    '##############################################################################################
    Public Class AgronicaPolicyAssertion
        Inherits PolicyAssertion

        Private _userName As String
        Private _password As String
        Private _Flag_Agrea1_AgriRer2 As Int32

        Public Shared AgroUsername As String
        Public Shared AgroPassword As String
        Public Shared AgroFlag_Agrea1_AgriRer2 As Int32



        Public Overloads Overrides Function CreateServiceInputFilter(ByVal context As FilterCreationContext) As Microsoft.Web.Services3.SoapFilter
            Return New WSSEDraftServiceInputFilter()
        End Function

        Public Overloads Overrides Function CreateServiceOutputFilter(ByVal context As FilterCreationContext) As Microsoft.Web.Services3.SoapFilter
            Return New WSSEDraftServiceOutputFilter()
        End Function

        Public Overloads Overrides Function CreateClientInputFilter(ByVal context As FilterCreationContext) As Microsoft.Web.Services3.SoapFilter
            'Return New WSSEDraftClientInputFilter(_Flag_Agrea1_AgriRer2)
            Return New WSSEDraftClientInputFilter(AgroFlag_Agrea1_AgriRer2)
        End Function

        Public Overloads Overrides Function CreateClientOutputFilter(ByVal context As FilterCreationContext) As Microsoft.Web.Services3.SoapFilter
            'Return New WSSEDraftClientOutputFilter(_userName, _password, _Flag_Agrea1_AgriRer2)
            Return New WSSEDraftClientOutputFilter(AgroUsername, AgroPassword, AgroFlag_Agrea1_AgriRer2)
        End Function

        Public Overloads Overrides Sub ReadXml(ByVal reader As XmlReader, ByVal extensions As IDictionary(Of String, Type))
            If reader Is Nothing Then
                Throw New ArgumentNullException("reader")
            End If
            If extensions Is Nothing Then
                Throw New ArgumentNullException("extensions")
            End If

            _userName = reader.GetAttribute("userName")
            _password = reader.GetAttribute("password")

            'Sostituisco con i valori corretti
            _userName = AgroUsername
            _password = AgroPassword
            _Flag_Agrea1_AgriRer2 = AgroFlag_Agrea1_AgriRer2

            Dim isEmpty As Boolean = reader.IsEmptyElement

            reader.ReadStartElement("AgronicaAssertion")
            If Not isEmpty Then
                reader.Skip()
            End If
        End Sub

        Public Overloads Overrides Function GetExtensions() As IEnumerable(Of KeyValuePair(Of String, Type))
            Return New KeyValuePair(Of String, Type)() {New KeyValuePair(Of String, Type)("AgronicaAssertion", Me.[GetType]())}
        End Function

    End Class






    '##############################################################################################
    Class WSSEDraftServiceInputFilter
        Inherits SoapFilter

        Public Overloads Overrides Function ProcessMessage(ByVal envelope As SoapEnvelope) As SoapFilterResult
            'TODO: The following will not work with SOAP 1.2
            Dim nsmanager As New XmlNamespaceManager(envelope.NameTable)
            nsmanager.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")
            nsmanager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")
            Dim tokens As XmlNodeList = envelope.SelectNodes("/soap:Envelope/soap:Header/wsse:Security/wsse:UsernameToken", nsmanager)
            If tokens.Count > 0 Then

                'Dim tokenNode As XmlElement = tokens(0)
                Dim tokenNode As XmlElement = CType(tokens(0), XmlElement)

                Dim token As New UsernameToken(tokenNode)

            End If

            'TODO: This is bad. There's got to be some way to indicate
            ' did understand without tearing the mustUnderstand attribute off.

            'Dim wssedraft As XmlElement = envelope.SelectNodes("/soap:Envelope/soap:Header/wsse:Security", nsmanager)(0)
            Dim wssedraft As XmlElement = CType(envelope.SelectNodes("/soap:Envelope/soap:Header/wsse:Security", nsmanager)(0), XmlElement)

            'wssedraft.Attributes.Remove(wssedraft.Attributes.GetNamedItem("soap:mustUnderstand", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"))
            wssedraft.Attributes.Remove(CType(wssedraft.Attributes.GetNamedItem("soap:mustUnderstand", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"), XmlAttribute))

            'vanni 08/06/2009 >> rimuovo il timestamp
            'wssedraft.RemoveChild(wssedraft.SelectSingleNode("wsu:Timestamp"))

            Return SoapFilterResult.[Continue]
        End Function

    End Class






    '##############################################################################################
    Class WSSEDraftServiceOutputFilter
        Inherits SoapFilter

        Public Overloads Overrides Function ProcessMessage(ByVal envelope As SoapEnvelope) As SoapFilterResult
            Dim header As XmlElement = envelope.CreateHeader()
            Dim timestampElement As XmlElement = envelope.CreateElement("wsu", "Timestamp", TokenNames.UtilityURI)

            Dim createdElement As XmlElement = envelope.CreateElement("wsu", "Created", TokenNames.UtilityURI)
            createdElement.InnerText = XmlConvert.ToString(System.DateTime.Now, "yyyy-MM-ddTHH:mm:ssZ")
            timestampElement.AppendChild(createdElement)

            Dim expiresElement As XmlElement = envelope.CreateElement("wsu", "Expires", TokenNames.UtilityURI)
            expiresElement.InnerText = XmlConvert.ToString(System.DateTime.Now.AddMinutes(5), "yyyy-MM-ddTHH:mm:ssZ")
            timestampElement.AppendChild(expiresElement)

            header.AppendChild(timestampElement)

            Return SoapFilterResult.[Continue]
        End Function

    End Class






    '##############################################################################################
    Class WSSEDraftClientInputFilter
        Inherits SoapFilter

        Private _Flag_Agrea1_AgriRer2 As Int32


        Friend Sub New(ByVal Flag_Agrea1_AgriRer2 As Int32)
            _Flag_Agrea1_AgriRer2 = Flag_Agrea1_AgriRer2
        End Sub


        Public Overloads Overrides Function ProcessMessage(ByVal envelope As SoapEnvelope) As SoapFilterResult


            Select Case _Flag_Agrea1_AgriRer2

                Case 1 'AGREA

                    '======================================================================================
                    '===  WS_AGREA  =======================================================================
                    '======================================================================================
                    '
                    '
                    '
                    '
                    '======================================================================================


                Case 2 'AGRIRER

                    '======================================================================================
                    '===  WS_AGRIRER  =====================================================================
                    '======================================================================================

                    Dim nsmanager As New XmlNamespaceManager(envelope.NameTable)
                    nsmanager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/")
                    nsmanager.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")
                    nsmanager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")

                    Dim wssedraft As XmlElement = CType(envelope.SelectNodes("/soapenv:Envelope/soapenv:Header/wsse:Security", nsmanager)(0), XmlElement)

                    'Modifica del valore dell'attributo "mustUnderstand" 
                    wssedraft.SetAttribute("soapenv:mustUnderstand", "0")

                    '======================================================================================

                Case Else 'ALTRO

                    '======================================================================================
                    '
                    '
                    '
                    '======================================================================================

            End Select

            Return SoapFilterResult.[Continue]

        End Function

    End Class






    '##############################################################################################
    Class WSSEDraftClientOutputFilter
        Inherits SoapFilter

        Private _userName As String
        Private _password As String
        Private _Flag_Agrea1_AgriRer2 As Int32


        Friend Sub New(ByVal userName As String, ByVal password As String, ByVal Flag_Agrea1_AgriRer2 As Int32)
            _userName = userName
            _password = password
            _Flag_Agrea1_AgriRer2 = Flag_Agrea1_AgriRer2
        End Sub


        ''' <summary>
        ''' Inserire qui il codice per modificare l'XML di richiesta SOAP
        ''' </summary>
        ''' <param name="envelope"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Overrides Function ProcessMessage(ByVal envelope As SoapEnvelope) As SoapFilterResult


            Select Case _Flag_Agrea1_AgriRer2

                Case 1 'AGREA

                    '======================================================================================
                    '===  WS_AGREA  =======================================================================
                    '======================================================================================

                    Dim securityElement As XmlElement = envelope.CreateElement("wsse", "Security", TokenNames.NamespaceURI)

                    Dim mustUnderstandAttribute As XmlAttribute = envelope.CreateAttribute(envelope.DocumentElement.Prefix, "mustUnderstand", envelope.DocumentElement.NamespaceURI)
                    mustUnderstandAttribute.Value = "1"
                    securityElement.Attributes.Append(mustUnderstandAttribute)

                    Dim token As New UsernameToken(_userName, _password, Microsoft.Web.Services3.Security.Tokens.PasswordOption.SendHashed)
                    securityElement.AppendChild(token.GetXml(envelope))

                    envelope.CreateHeader().AppendChild(securityElement)

                    '======================================================================================


                Case 2 'AGRIRER

                    '======================================================================================
                    '===  WS_AGRIRER  =====================================================================
                    '======================================================================================

                    '--------------------------------------------
                    '--- Creo il nodo <soap:Header>
                    '--------------------------------------------

                    Dim Header2 As XmlElement = envelope.CreateHeader()

                    '--------------------------------------------
                    '--- Creo il nodo <wsse:Security>
                    '--------------------------------------------

                    Dim securityElement2 As XmlElement = envelope.CreateElement("wsse", "Security", TokenNames.NamespaceURI)
                    Dim mustUnderstandAttribute2 As XmlAttribute = envelope.CreateAttribute(envelope.DocumentElement.Prefix, "mustUnderstand", envelope.DocumentElement.NamespaceURI)
                    mustUnderstandAttribute2.Value = "1"
                    securityElement2.Attributes.Append(mustUnderstandAttribute2)

                    '--------------------------------------------
                    '--- Creo il nodo <wsse:UsernameToken>
                    '--------------------------------------------

                    Dim token2 As New UsernameToken(_userName, _password, Microsoft.Web.Services3.Security.Tokens.PasswordOption.SendPlainText)
                    Dim usernametokenElement2 As XmlElement
                    usernametokenElement2 = token2.GetXml(envelope)

                    '--------------------------------------------
                    '--- Creo il nodo <wsu:Timestamp>
                    '--------------------------------------------

                    Dim timestampElement As XmlElement
                    Dim Ts As New Microsoft.Web.Services3.Security.Utility.Timestamp()
                    timestampElement = Ts.GetXml(envelope)

                    '--------------------------------------------
                    '--- Unisco gli elementi
                    '--------------------------------------------

                    securityElement2.AppendChild(timestampElement)
                    securityElement2.AppendChild(usernametokenElement2)
                    Header2.AppendChild(securityElement2)

                    Dim pippo As String = String.Empty

                    '======================================================================================


                Case Else 'ALTRO

                    '======================================================================================
                    '
                    '
                    '
                    '======================================================================================

            End Select



            Return SoapFilterResult.[Continue]

        End Function

    End Class



End Namespace