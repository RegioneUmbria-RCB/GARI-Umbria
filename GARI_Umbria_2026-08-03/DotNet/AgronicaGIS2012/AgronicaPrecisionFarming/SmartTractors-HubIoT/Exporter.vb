Imports AgronicaCoreDTOStd.SmartTractors_HubIoT
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports HubIoT_Helper = AgronicaCoreHubIoTBIZ.HubIoT_Helper
Imports System.IO

Public Class HubIot_Exporter
    Inherits LogProvider
    Implements IDisposable, IHubIotExporter

    Private ReadOnly _vin As String
    Private ReadOnly _piva As String
    Private ReadOnly _iddocumento As Integer
    Private ReadOnly _iddocumentooperazione As Integer
    Private _objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _helper As HubIoT_Helper
    Private _connectionConfig As ConnectionConfig
    Private _settings As Settings
    Private _pathLog As String
    Private _fileLog As String

    Public Sub New(ByVal Piva As String,
                   ByVal VIN As String,
                   ByVal IdDocumento As Integer,
                   ByVal IdDocumentoOperazione As Integer,
                   ByVal PathLog As String,
                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        _vin = VIN
        _piva = Piva
        _iddocumento = IdDocumento
        _iddocumentooperazione = IdDocumentoOperazione
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        _pathLog = Path.Combine(PathLog, "SmartTractors", IIf(Piva = "-1", "general", Piva))
        If Not Directory.Exists(_pathLog) Then
            Directory.CreateDirectory(_pathLog)
        End If
        _fileLog = "export_" + DateTime.Now().ToString().Replace("/", "-").Replace("\", "-").Replace(" ", "_").Replace(":", "-") + ".log"
        _helper = New HubIoT_Helper(_objParametri_Server, _objParametri_Utenti, _pathLog, _fileLog)

        '0 - inizializzazione
        Try
            _connectionConfig = _helper.GetParametriConnessione(_piva)
            _settings = _helper.GetConfigurazione(_piva)
        Catch ex As Exception
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       ex.TargetSite.Name.ToString(),
                       $"[Error] Inizializzazione exporter {ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Public Sub Execute() Implements IHubIotExporter.Execute
        Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
        Scrivi_LOG(_objParametri_Server,
                   "HubIot_Exporter.Execute",
                   $"[INFO] Inizio esportazione dati verso HubIoT....",
                   CustomLOGParams:=customLOGParams)
        Try

            Dim objList = _helper.GetWorkOrderDetail(IIf(_piva = "-1", "", _piva), _vin, _iddocumento, _iddocumentooperazione, _connectionConfig.pars.orgIDs)
            If objList.Count > 0 Then
                Dim uriToken = _helper.getOAuth2Metadata(_settings.WellKnownUrl).token_endpoint

                Dim token = _helper.getAccessToken(uriToken, _connectionConfig.pars.conn)
                If token Is Nothing Then
                    Throw New Exception("Errore in recupero access token. elaborazione interrotta")
                End If

                Dim userTokeList = _helper.getUserToken(_settings.ApiUrl, token)

                Dim platform_token As UserToken

                For Each obj In objList
                    Select Case obj.Platform
                        Case enum_HubIoTPlatformDestination.JohnDeere
                            platform_token = userTokeList.Where(Function(x) x.provider = "JD").FirstOrDefault()

                        Case enum_HubIoTPlatformDestination.Agrirouter
                            platform_token = userTokeList.Where(Function(x) x.provider = "AG").FirstOrDefault()

                        Case enum_HubIoTPlatformDestination.AGCO_Trimble
                            platform_token = userTokeList.Where(Function(x) x.provider = "TB").FirstOrDefault()

                        Case enum_HubIoTPlatformDestination.CNH1
                            platform_token = userTokeList.Where(Function(x) x.provider = "CNH").FirstOrDefault()

                    End Select

                    If platform_token IsNot Nothing Then
                        If _helper.SendPrescriptionFile(_settings.ApiUrl, token, platform_token, obj, _piva) = True Then
                            _helper.AggiornaStatoWorkOrderKey(New List(Of String)({obj.workOrderId}), TipiEnumerativi.enum_HubIoT_StatoElaborazione.Inviato)
                        End If

                        Scrivi_LOG(_objParametri_Server,
                                   "HubIot_Exporter.Execute",
                                   $"[INFO] Ricetta Operazione con ID {obj.workOrderId} inviata.",
                                   CustomLOGParams:=customLOGParams)

                    Else
                        Scrivi_LOG(_objParametri_Server,
                                   "HubIot_Exporter.Execute",
                                   $"[Error] Platform token per la destinazione {obj.Platform.ToString()} non trovato. impossibile inviare la mappa di prescrizione",
                                   CustomLOGParams:=customLOGParams)

                    End If
                Next
            Else
                Scrivi_LOG(_objParametri_Server,
                           "HubIot_Exporter.Execute",
                           $"[INFO] Nessuna documento da elaborare per la partita iva {_piva}",
                           CustomLOGParams:=customLOGParams)

            End If
        Catch ex As Exception
            Scrivi_LOG(_objParametri_Server,
                       "HubIot_Exporter.Execute",
                       $"[Error] {ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)

        End Try
        Scrivi_LOG(_objParametri_Server,
                   "HubIot_Exporter.Execute",
                   $"[INFO] FINE esportazione dati verso HubIoT....",
                   CustomLOGParams:=customLOGParams)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _helper.Dispose()
        _objParametri_Server = Nothing
        _objParametri_Utenti = Nothing
        _connectionConfig = Nothing
        _settings = Nothing
    End Sub

#Region "Internal"
    Private Sub CheckRequirements(ByRef MessageError As String) Implements IHubIotExporter.CheckRequirements
        Throw New NotImplementedException()

        'If _tipologia = 0 OrElse _idRicetta = 0 Then
        '    Throw New Exception("Specificare tipologia e ricetta da esportare")
        'End If

        'Dim objList = _helper.GetElencoRicetteInviabili(_tipologia, _idRicetta)
        'If objList Is Nothing OrElse objList.Count <= 0 Then
        '    Throw New Exception("Nessuna ricetta trovata per i parametri inseriti - Tipologia (" + _tipologia.ToString() + ") / IdRicetta (" + _idRicetta.ToString() + ")")
        'End If

        'For Each obj In objList

        'Next


    End Sub

    Private Sub GeneraFileZip(IdDocument As String, FileName As String, WithPrescription As Boolean, ByRef MessageError As String) Implements IHubIotExporter.GeneraFileZip
        Throw New NotImplementedException()
    End Sub

    Private Sub SendFile(IdDocument As String, ByRef MessageError As String) Implements IHubIotExporter.SendFile
        Throw New NotImplementedException()
    End Sub

    Private Sub MarkDocumentAsSended(IdDocument As String, ByRef MessageError As String) Implements IHubIotExporter.MarkDocumentAsSended
        Throw New NotImplementedException()
    End Sub


    Private Shared Sub CheckMacchinaRicetta()


    End Sub

#End Region

End Class
