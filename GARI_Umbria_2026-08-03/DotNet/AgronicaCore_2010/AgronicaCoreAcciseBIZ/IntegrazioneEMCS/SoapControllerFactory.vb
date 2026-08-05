Imports AgronicaCoreVarieDAL

Public Class SoapControllerFactory

    Private _soapFtp As ISOAPControllerWsFtp
    Private _soapEmcs As ISOAPControllerEMCS
    Private ReadOnly _configMin As AcciseServiceConfigMin
    Private ReadOnly _configurazioneServizio As Configurazione_Servizio
    Private ReadOnly _logger As AcciseLogger
    Private ReadOnly _signHelper As SignHelper

    Public Sub New(ByVal configMin As AcciseServiceConfigMin,
                   ByVal configurazioneServizio As Configurazione_Servizio,
                   ByVal logger As AcciseLogger,
                   ByVal signHelper As SignHelper)
        _configMin = configMin
        _configurazioneServizio = configurazioneServizio
        _logger = logger
        _signHelper = signHelper
    End Sub

    Public Function Create(Of T)(Optional ByVal userName As String = "", Optional ByVal password As String = "") As ISOAPController

        If GetType(T) = GetType(SOAPControllerWsFtp) Then
            If _soapFtp Is Nothing Then _soapFtp = CreateWsdlFtpSoapController(userName, password)
            Return _soapFtp
        Else
            If _soapEmcs Is Nothing Then _soapEmcs = CreateEmcsSoapController()
            Return _soapEmcs
        End If

    End Function

    Private Function OttieniTimeoutServizi() As TimeSpan

        Dim timeout As Integer

        If String.IsNullOrEmpty(_configMin.ServiceTimeout) Then
            Return New TimeSpan(0, 2, 0)
        End If

        If Not Integer.TryParse(_configMin.ServiceTimeout, timeout) Then
            Return New TimeSpan(0, 2, 0)
        End If

        Return New TimeSpan(0, timeout, 0)

    End Function

    Private Function CreateEmcsSoapController() As SOAPControllerEMCS

        Return New SOAPControllerEMCS(
           _configurazioneServizio.Username,
           _configurazioneServizio.Password,
           _configMin.EMCSServiceEndpoint,
           OttieniTimeoutServizi(),
           _logger,
           _signHelper
           )

    End Function

    Private Function CreateWsdlFtpSoapController(Optional ByVal userName As String = "",
                                                 Optional ByVal password As String = "") As SOAPControllerWsFtp

        If Not String.IsNullOrEmpty(userName) AndAlso Not IsNothing(password) Then
            Return New SOAPControllerWsFtp(
               userName,
               password,
               _configMin.WsdlFtpServiceEndpoint,
               OttieniTimeoutServizi(),
               _logger
           )
        Else
            Return New SOAPControllerWsFtp(
                _configurazioneServizio.Username,
                _configurazioneServizio.Password,
                _configMin.WsdlFtpServiceEndpoint,
                OttieniTimeoutServizi(),
                _logger
            )
        End If

    End Function



End Class
