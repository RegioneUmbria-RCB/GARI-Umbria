Imports System.IO
Imports System.Net
Imports System.ServiceModel
Imports AgronicaCoreAcciseBIZ.IntegrazioneEMCS
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Xml
Imports AgronicaCoreAcciseCommon.DAA.Messaggi.Ie815
Imports System.Xml.Serialization
Imports AgronicaCoreAcciseCommon.DAA.Messaggi.Ie801

Public Class SOAPControllerEMCS : Implements ISOAPControllerEMCS

    Private ReadOnly _emcsClient As DispatcherBeanClient
    Private ReadOnly _logger As AcciseLogger
    Private _loggedIn As Boolean = False

    Public Sub New(
            ByVal user As String,
            ByVal password As String,
            ByVal serviceEnpoint As String,
            ByVal serviceTimeout As TimeSpan,
            ByVal logger As AcciseLogger,
            ByVal signHelper As SignHelper)

        Dim binding = New BasicHttpBinding With {
           .Name = "DispatcherBean"
       }
        binding.Security.Mode = SecurityMode.Transport
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate
        binding.MessageEncoding = WSMessageEncoding.Mtom
        binding.TextEncoding = Encoding.UTF8

        Dim theEndpoint = New EndpointAddress(New Uri(serviceEnpoint))
        _emcsClient = New DispatcherBeanClient(binding, theEndpoint)
        _emcsClient.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySerialNumber, "3619a76b6082489b")
        _emcsClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerTrust
        _emcsClient.ClientCredentials.UserName.UserName = user
        _emcsClient.ClientCredentials.UserName.Password = password

        _logger = logger

    End Sub

    Public Sub Inizializza() Implements ISOAPControllerEMCS.Inizializza
        _loggedIn = Me.LoginAndTest()
    End Sub

    Public Function PUT(ByVal fileContent As Byte(), ByVal fileName As String, ByRef errore As String) As Boolean Implements ISOAPController.PUT
        Throw New NotImplementedException()
    End Function

    Public Function DIR(patternRicerca As String, ByRef errore As String) As List(Of FileRisposta) Implements ISOAPController.DIR
        Throw New NotImplementedException()
    End Function

    Public Function [GET](nomeFileRisposta As String, ByRef errore As String) As Byte() Implements ISOAPController.GET

        CheckLoggedIn()

        Dim ie = New IE801Type()
        _2_LeggiXML(ie, "", New StringBuilder())

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
            Dim response = _emcsClient.dispatcher(New MessageDTO With
                                              {
                                                .serviceID = "D2",
                                                .inputObj = "19ITTVV00823F00002168"
                                              })

            Dim doc = New XmlDocument()
            Dim MS = New MemoryStream(DirectCast(response.outputObj, XmlDTO).[xml])
            doc.Load(MS)

            Dim oObj = response.outputObj

            Console.WriteLine(response)
        Catch ex As Exception
            _logger.Logga("", ex.Message)
        End Try

    End Function

    Private Function LoginAndTest() As Boolean

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
            Dim response = _emcsClient.dispatcher(New MessageDTO With
                                              {
                                                .serviceID = "L0"
                                              })
            Dim xmlDoc = XmlDaDTO(response)
            Return If(xmlDoc.DocumentElement.InnerText = "1", True, False)

        Catch ex As Exception
            _logger.Logga("", ex.Message)
            Return False
        End Try

    End Function

    Private Sub CheckLoggedIn()
        If Not _loggedIn Then
            Throw New Exception("Non loggato al servizio")
        End If
    End Sub

    Private Function XmlDaDTO(ByVal risposta As MessageDTO) As XmlDocument

        Dim doc = New XmlDocument()
        Dim MS = New MemoryStream(DirectCast(risposta.outputObj, XmlDTO).[xml])
        doc.Load(MS)
        Return doc

    End Function

    Private Sub _2_LeggiXML(ByRef cont As IE801Type,
                           ByRef errori As String,
                           ByRef mailMsg As StringBuilder)

        Dim xmlReader As XmlReader = Nothing

        Try

            xmlReader = XmlReader.Create(Path.Combine("C:\AgenziaDogane\Borgoluce\XML", "19ITTVV00823F00002168"))

            Dim serializerTrans As New XmlSerializer(GetType(IE801Type))
            If serializerTrans.CanDeserialize(xmlReader) Then
                cont = serializerTrans.Deserialize(xmlReader)
            Else
                cont = serializerTrans.Deserialize(xmlReader)
            End If

            xmlReader.Close()

        Catch ex As Exception
            xmlReader.Close()
            Dim innerMsg As String = ""
            If ex.InnerException IsNot Nothing Then
                innerMsg = ex.InnerException.Message
            End If

        End Try

    End Sub

    Public Function GETLOG() As Boolean Implements ISOAPController.GETLOG
        Throw New NotImplementedException()
    End Function
End Class
