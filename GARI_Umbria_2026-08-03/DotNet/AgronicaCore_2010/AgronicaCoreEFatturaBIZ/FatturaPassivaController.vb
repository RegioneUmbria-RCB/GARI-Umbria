Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ.Persisters
Imports AgronicaCoreEFatturaDAL

Public Class FatturaPassivaController

    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtenti As AgronicaCoreParametri

    Private ReadOnly _fileSystemPersister As IPersister
    Private ReadOnly _fileManager As IFIleManager

    Private ReadOnly _soapControllerCicloPassivo As ISOAPControllerCicloPassivo
    Private ReadOnly _logger As EFatturaLogger

    Private ReadOnly _connectionStringServer As String

    Public Sub New(ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri,
                   ByVal fileSystemPersister As IPersister,
                   ByVal soapControllerCicloPassivo As ISOAPControllerCicloPassivo,
                   ByVal fileManager As IFIleManager,
                   ByVal logger As EFatturaLogger
        )

        _objParametriSuperServer = objParametriSuperServer
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
        _fileSystemPersister = fileSystemPersister
        _soapControllerCicloPassivo = soapControllerCicloPassivo
        _logger = logger

    End Sub

    Public Sub ScaricaFatturePassive(ByVal PIVA As String)
        Dim dataScarico = Date.Now
        Dim dataUltimoScarico = LeggiDataUltimoScarico(PIVA)
        Dim numFatture = _soapControllerCicloPassivo.LeggiFatture(dataUltimoScarico)
        If numFatture > 0 Then
            ScriviDataUltimoScarico(PIVA, dataScarico, numFatture)
        End If
    End Sub

    Public Function LeggiDataUltimoScarico(ByVal PIVA As String) As Date
        Dim dalR = New Imprese_Codici_R(_objParametriServer)
        Dim valore = dalR.Leggi(PIVA, enum_CodiciAnagrafe.DataUltimaRicezioneEFattura)
        If Not String.IsNullOrEmpty(valore) Then
            Dim info = valore.Split("|")
            Return Date.ParseExact(info(0), "yyyyMMdd HHmmss", Globalization.CultureInfo.InvariantCulture)
        End If
        Return Date.MinValue 'AGRODATAINIZIO
    End Function

    Public Sub ScriviDataUltimoScarico(ByVal PIVA As String, ByVal dataScarico As Date, ByVal numFatture As Integer)
        Dim valore = Format(dataScarico, "yyyyMMdd HHmmss") & "|" & numFatture
        Dim dalW = New Imprese_Codici_W(_objParametriServer)
        dalW.Scrivi(PIVA, enum_CodiciAnagrafe.DataUltimaRicezioneEFattura, valore)
    End Sub

    Public Function CheckPreliminari() As Boolean

        Dim messaggioErrore As String = ""
        Dim nomeProcedura As String = "FatturaPassivaController.CheckPreliminari"
        Dim errori As String = String.Empty
        Dim result As Boolean = False

        Try
            If _soapControllerCicloPassivo Is Nothing Then
                _logger.Logga(nomeProcedura, "Cliente non abilitato")
                Return False
            End If

            'controlla se servizio 2c Running
            If Not _soapControllerCicloPassivo.ContattoHub() Then
                Throw New Exception(errori)
            End If

            result = True

        Catch ex As Exception
            messaggioErrore = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            errori &= "[" & nomeProcedura & "] : " & messaggioErrore & "</br>"
        End Try

        Return result

    End Function
End Class
