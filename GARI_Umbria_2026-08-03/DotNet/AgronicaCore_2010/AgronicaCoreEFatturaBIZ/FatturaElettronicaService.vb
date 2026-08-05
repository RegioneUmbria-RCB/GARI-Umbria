Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ.Persisters
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class FatturaElettronicaService

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private _fatturaAttivaController As FatturaAttivaController
    Private _fatturaPassivaController As FatturaPassivaController
    Private ReadOnly _configurazioneServizio As Configurazione_Servizio
    Private ReadOnly _configMin As FatturaElettronicaServiceConfigMin
    Public Sub New(
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtente As AgronicaCoreParametri,
            ByVal objParametriSuperServer As AgronicaCoreParametri,
            ByVal configurazioneServizio As Configurazione_Servizio
        )

        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer
        _configurazioneServizio = configurazioneServizio

        _configMin = JsonConvert.DeserializeObject(Of FatturaElettronicaServiceConfigMin)(_configurazioneServizio.Parametri_Extra)

    End Sub

    Public Sub New(ByVal configurazioneServizio As Configurazione_Servizio)
        _configurazioneServizio = configurazioneServizio
        _configMin = JsonConvert.DeserializeObject(Of FatturaElettronicaServiceConfigMin)(_configurazioneServizio.Parametri_Extra)
    End Sub

    Public Function ParametriExtra() As FatturaElettronicaServiceConfigMin
        Return _configMin
    End Function

    Public Function Esegui() As List(Of String)

        Dim errori = New List(Of String)

        Select Case _configurazioneServizio.Tipo_Sincro
            Case enum_Tipi_Servizi_Background.EFattura_Generazione_XML
                errori = GeneraFatture()
            Case enum_Tipi_Servizi_Background.EFattura_Invio_XML_Attivi
                errori = InviaFatture()
            Case enum_Tipi_Servizi_Background.EFattura_Verifica_Esiti
                errori = LeggiEsitiFatture()
            Case enum_Tipi_Servizi_Background.EFattura_Ricevi_XML_Passivi
                ScaricaFatturePassive()
        End Select

        Return errori

    End Function
    Public Function LeggiEsitiFatture() As List(Of String)
        Return LeggiEsitiFatture(New AgendaFiltroLettura With {.PIVA = _configMin.PIVA})
    End Function
    Public Function LeggiEsitiFatture(ByVal filtro As AgendaFiltroLettura) As List(Of String)

        Dim result = New List(Of String)

        If _fatturaAttivaController Is Nothing Then
            _fatturaAttivaController = CreaControllerFatturaAttiva()
        End If

        Dim erroriCheck = String.Empty
        If Not _fatturaAttivaController.CheckPreliminari(erroriCheck) Then
            result.Add(erroriCheck)
            Return result
        End If

        Return _fatturaAttivaController.LeggiEsitiFatture(filtro)

    End Function

    Public Function InviaFatture() As List(Of String)
        Return InviaFatture(New AgendaFiltroLettura With {.PIVA = _configMin.PIVA})
    End Function

    Public Function InviaFatture(ByVal filtro As AgendaFiltroLettura) As List(Of String)

        Dim result = New List(Of String)

        If _fatturaAttivaController Is Nothing Then
            _fatturaAttivaController = CreaControllerFatturaAttiva()
        End If

        Dim erroriCheck = String.Empty
        If Not _fatturaAttivaController.CheckPreliminari(erroriCheck) Then
            result.Add(erroriCheck)
            Return result
        End If

        Return _fatturaAttivaController.InviaFatture2C(_configurazioneServizio.PivaSuperuser, filtro, _configMin.Debug)

    End Function

    Public Function GeneraFatture() As List(Of String)
        Return GeneraFatture(New AgendaFiltroLettura With {.PIVA = _configMin.PIVA})
    End Function

    Public Function GeneraFatture(ByVal filtro As AgendaFiltroLettura) As List(Of String)

        If _fatturaAttivaController Is Nothing Then
            _fatturaAttivaController = CreaControllerFatturaAttiva()
        End If

        If filtro Is Nothing Then
            filtro = New AgendaFiltroLettura With {.PIVA = _configMin.PIVA}
        End If

        Return _fatturaAttivaController.GeneraFatture(filtro, _configurazioneServizio.Id_Cod_Cliente, _configurazioneServizio.PivaSuperuser,
                                               _configMin.PostValidazioneXML, _configMin.EscludiFattureEstere, _configMin.PeriodoControlloGG)

    End Function

    Public Sub ScaricaFatturePassive()
        ScaricaFatturePassive(_configMin.PIVA)
    End Sub

    Public Sub ScaricaFatturePassive(ByVal PIVA As String)

        If _fatturaPassivaController Is Nothing Then
            _fatturaPassivaController = CreaControllerFatturaPassiva()
        End If

        If Not _fatturaPassivaController.CheckPreliminari() Then
            Return
        End If

        _fatturaPassivaController.ScaricaFatturePassive(PIVA)

    End Sub

    Private Function CreaControllerFatturaPassiva() As FatturaPassivaController

        Dim result As FatturaPassivaController

        Dim fileManager As New FileManager(_configurazioneServizio.DirectoryFileEsportazioni, _configurazioneServizio.DirectoryLOG)
        fileManager.Initialize()
        Dim logger As New EFatturaLogger(_objParametriServer, New LogProvider(), fileManager)
        Dim fsPersister As New FileSystemPersister(fileManager, logger)

        Dim soapControllerCicloPassivo As SOAPControllerCicloPassivo = Nothing
        If Not String.IsNullOrEmpty(_configurazioneServizio.Username) AndAlso Not String.IsNullOrEmpty(_configurazioneServizio.Password) Then
            If Not _configMin Is Nothing AndAlso Not String.IsNullOrEmpty(_configMin.EnpointServiceCicloAttivo) Then
                soapControllerCicloPassivo = New SOAPControllerCicloPassivo(
            _configurazioneServizio.Username,
            _configurazioneServizio.Password,
            _configMin.EnpointServiceCicloPassivo,
            OttieniTimeoutServizi(),
            fileManager, fsPersister)
            End If

        End If

        result = New FatturaPassivaController(
            Nothing,
            _objParametriServer,
            Nothing,
            fsPersister,
            soapControllerCicloPassivo,
            fileManager,
            logger
            )

        Return result
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

    Private Function CreaControllerFatturaAttiva() As FatturaAttivaController

        Dim result As FatturaAttivaController

        Dim fileManager As New FileManager(_configurazioneServizio.DirectoryFileEsportazioni, _configurazioneServizio.DirectoryLOG)
        fileManager.Initialize()

        Dim logger As New EFatturaLogger(_objParametriServer, New LogProvider(), fileManager)

        Dim fatturaReader As New DataReader(_objParametriServer, _objParametriUtente, _objParametriSuperServer, _configMin.Debug)

        Dim decodificheMapper As New DecodificheMapper(fatturaReader)
        Dim mapper As New FatturaMapper(decodificheMapper, _objParametriSuperServer, _objParametriServer, _objParametriUtente, logger, _configMin.Debug)
        Dim fsPersister As New FileSystemPersister(fileManager, logger)

        Dim logHelper As New SDI_Log_Helper(_objParametriServer)

        Dim soapControllerCicloAttivo As SOAPControllerCicloAttivo = Nothing

        If Not String.IsNullOrEmpty(_configurazioneServizio.Username) AndAlso Not String.IsNullOrEmpty(_configurazioneServizio.Password) Then
            If Not _configMin Is Nothing AndAlso Not String.IsNullOrEmpty(_configMin.EnpointServiceCicloAttivo) Then
                soapControllerCicloAttivo = New SOAPControllerCicloAttivo(
            _configurazioneServizio.Username,
            _configurazioneServizio.Password,
            _configMin.EnpointServiceCicloAttivo,
            OttieniTimeoutServizi(),
            fileManager,
            logger)
            End If

        End If

            result = New FatturaAttivaController(
            Nothing,
            _objParametriServer,
            Nothing,
            fatturaReader,
            mapper,
            fsPersister,
            logHelper,
            soapControllerCicloAttivo,
            fileManager,
            logger
            )

        Return result

    End Function

End Class
