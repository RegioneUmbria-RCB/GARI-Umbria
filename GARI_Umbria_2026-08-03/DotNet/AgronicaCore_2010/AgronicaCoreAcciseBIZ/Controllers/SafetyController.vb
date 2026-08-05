Imports AgronicaCoreAcciseDAL
Imports AgronicaCoreVarieDAL

Public Class SafetyController

    Private ReadOnly _logger As AcciseLogger
    Private ReadOnly _soapFactory As SoapControllerFactory
    Private ReadOnly _dataManager As IDataManager

    Public Sub New(
           ByVal logger As AcciseLogger,
           ByVal soapFactory As SoapControllerFactory,
           ByVal dataManager As IDataManager
       )
        _logger = logger
        _soapFactory = soapFactory
        _dataManager = dataManager

    End Sub

    Public Function ServizioAttivo(ByVal servizio As Configurazione_Servizio) As Boolean
        Return _dataManager.ServizioAttivo(servizio)
    End Function

    Public Function ServizioAttivo() As Boolean
        Return _dataManager.ServizioAttivo()
    End Function
    Public Function Verifica(Optional ByVal disabilia As Boolean = True) As Boolean

        Dim autorizzato = VerificaAutorizzazioniDogane()
        If Not autorizzato Then
            If disabilia Then
                DisabilitaServizio()
            End If
        End If

        Return autorizzato

    End Function

    Public Function Verifica(ByVal userName As String, ByVal password As String) As Boolean

        Dim autorizzato = VerificaAutorizzazioniDogane(userName, password)
        Return autorizzato

    End Function

    Private Sub DisabilitaServizio(ByVal servizio As Configurazione_Servizio)
        _dataManager.DisabilitaServizio(servizio)
    End Sub

    Private Sub DisabilitaServizio()
        _dataManager.DisabilitaServizio()
    End Sub
    Private Function VerificaAutorizzazioniDogane(Optional ByVal userName As String = "", Optional ByVal password As String = "") As Boolean

        Dim risultato As Boolean = False
        Dim sc = _soapFactory.Create(Of SOAPControllerWsFtp)(userName, password)

        Try
            risultato = sc.GETLOG()
        Catch ex As UnauthorizedAccessException
            risultato = False
        End Try

        Return risultato

    End Function

End Class
