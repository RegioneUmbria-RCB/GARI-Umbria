Imports System.IO

Public Class SDI_CicloAttivo_InviaFattura

    Private ReadOnly _soapController As ISOAPControllerCicloAttivo
    Private ReadOnly _fileManager As IFIleManager
    Private ReadOnly _logger As EFatturaLogger
    Public Sub New(
                    ByVal soapController As ISOAPControllerCicloAttivo,
                    ByVal fileManager As IFIleManager,
                    ByVal logger As EFatturaLogger
        )
        _soapController = soapController
        _fileManager = fileManager
        _logger = logger
    End Sub

    Public Function InviaFattura(ByVal nomeFileXMl As String, ByRef response As SendInvoiceResponseWrapper) As Boolean

        response = New SendInvoiceResponseWrapper()

        Try
            Dim zipFileName = _soapController.GetNomeFileZipFatturaPA()
            response.NomeFileZip = zipFileName

            Dim fileXmlFullPath = Path.Combine(
                _fileManager.OttieniPercorso(FatturaElettronicaPath.XmlGenerati), nomeFileXMl)

            Dim zipFullPath = _fileManager.CreaFileZip(zipFileName, fileXmlFullPath)

            _soapController.UploadFileFatturaPA(zipFullPath)

            Dim siResponse = _soapController.SendElectronicInvoice(zipFullPath, Nothing).SendElectronicInvoiceResult
            response.SoapResponse = siResponse


        Catch ex As Exception
            response.Errore = ex.Message
            Return False
        End Try

        Return True


    End Function

    Public Function InviaFattura() As Boolean

        Dim zipFileName = _soapController.GetNomeFileZipFatturaPA()

        _soapController.UploadFileFatturaPA(zipFileName)

        _soapController.SendElectronicInvoice(zipFileName, Nothing)

        Return Nothing

    End Function

End Class
