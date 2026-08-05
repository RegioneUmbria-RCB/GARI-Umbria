Imports System.Net
Imports System.ServiceModel
Imports AgronicaCoreAcciseBIZ.IntegrazioneWSFTP

Public Class SOAPControllerWsFtp : Implements ISOAPControllerWsFtp

    Private ReadOnly _wsFtpClient As TelematicoFtpWsBindingImplClient
    Private ReadOnly _logger As AcciseLogger
    Private Const _nomeProcedura = "SOAPControllerWsFtp.{0}"
    Public Sub New(
            ByVal user As String,
            ByVal password As String,
            ByVal serviceEnpoint As String,
            ByVal serviceTimeout As TimeSpan,
            ByVal logger As AcciseLogger)

        Dim binding = New BasicHttpBinding With {
           .Name = "TelematicoFtpWsBindingImpl"
       }
        binding.Security.Mode = SecurityMode.Transport
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic
        binding.MessageEncoding = WSMessageEncoding.Mtom

        Dim theEndpoint = New EndpointAddress(New Uri(serviceEnpoint))

        _wsFtpClient = New TelematicoFtpWsBindingImplClient(binding, theEndpoint)

        _wsFtpClient.ClientCredentials.UserName.UserName = user
        _wsFtpClient.ClientCredentials.UserName.Password = password

        _logger = logger

    End Sub

    Public Function PUT(ByVal fileContent As Byte(), ByVal fileName As String, ByRef errore As String) As Boolean Implements ISOAPControllerWsFtp.PUT

        Dim activeProtocol = ServicePointManager.SecurityProtocol
        Dim retVal = False

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls12
            Dim responese = _wsFtpClient.put(fileContent, fileName)
            retVal = InvioACCDAA_Positivo(responese)
            If Not retVal Then errore = responese
        Catch ex As Exception
            If ex.InnerException IsNot Nothing AndAlso ex.InnerException.Message.Contains("401") Then
                errore = ex.InnerException.Message
            Else
                errore = ex.Message
            End If
            _logger.Logga(String.Format(_nomeProcedura, "PUT"), ex.Message)
        Finally
            ServicePointManager.SecurityProtocol = activeProtocol
        End Try

        Return retVal

    End Function

    Private Function InvioACCDAA_Positivo(ByVal response As String) As Boolean
        Return response.Trim().ToLowerInvariant().Equals("Caricato con successo.".Trim().ToLowerInvariant())
    End Function

    Public Function GETLOG() As Boolean Implements ISOAPController.GETLOG

        Dim activeProtocol = ServicePointManager.SecurityProtocol

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls12
            Dim response = _wsFtpClient.getlog()

            Return True

        Catch ex As Exception
            If ex.InnerException IsNot Nothing AndAlso ex.InnerException.Message.Contains("401") Then
                Throw New UnauthorizedAccessException(ex.Message)
            Else
                _logger.Logga(String.Format(_nomeProcedura, "GetLog"), ex.Message)
                Return True
            End If
        Finally
            ServicePointManager.SecurityProtocol = activeProtocol
        End Try

    End Function

    Public Function DIR(patternRicerca As String, ByRef errore As String) As List(Of FileRisposta) Implements ISOAPController.DIR

        Dim returnValue = New List(Of FileRisposta)

        Dim activeProtocol = ServicePointManager.SecurityProtocol

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls12
            Dim response = _wsFtpClient.dir(patternRicerca)

            If String.IsNullOrEmpty(response) Then Return returnValue

            Dim risposte = response.Split("|")

            returnValue.AddRange(risposte.Take(risposte.Length - 1).Select(Function(r)
                                                                               Dim dati = r.Split(",")
                                                                               Dim dataFileRisposta As DateTime = DateTime.MinValue
                                                                               Date.TryParse(dati(3), dataFileRisposta)

                                                                               Return New FileRisposta With
                                    {
                                        .CodiceFile = dati(0),
                                        .Data = dataFileRisposta,
                                        .NomeFileRicevuta = dati(1),
                                        .TipoRicevuta = dati(2)
                                    }
                                                                           End Function).ToList())

        Catch ex As Exception
            If ex.InnerException IsNot Nothing AndAlso ex.InnerException.Message.Contains("401") Then
                errore = ex.InnerException.Message
            Else
                errore = ex.Message
            End If
            _logger.Logga(String.Format(_nomeProcedura, "DIR"), ex.Message)
        Finally
            ServicePointManager.SecurityProtocol = activeProtocol
        End Try

        Return returnValue

    End Function

    Public Function [GET](nomeFileRisposta As String, ByRef errore As String) As Byte() Implements ISOAPController.GET

        Dim returnValue = New Byte() {}
        errore = String.Empty

        Dim activeProtocol = ServicePointManager.SecurityProtocol

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls12
            returnValue = _wsFtpClient.get(nomeFileRisposta)

        Catch ex As Exception
            If ex.InnerException IsNot Nothing AndAlso ex.InnerException.Message.Contains("401") Then
                errore = ex.InnerException.Message
            Else
                errore = ex.Message
            End If
            _logger.Logga(String.Format(_nomeProcedura, "GET"), ex.Message)
            Return Nothing
        Finally
            ServicePointManager.SecurityProtocol = activeProtocol
        End Try

        Return returnValue

    End Function
End Class
