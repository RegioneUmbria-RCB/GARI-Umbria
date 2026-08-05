Imports System.IO
Imports System.Security.Cryptography.X509Certificates
Imports EU.Europa.EC.Markt.Dss
Imports EU.Europa.EC.Markt.Dss.Signature
Imports EU.Europa.EC.Markt.Dss.Signature.Cades
Imports EU.Europa.EC.Markt.Dss.Signature.Token
Imports Org.BouncyCastle.Cms

Public Class SignHelper : Implements ISignHelper

    Private ReadOnly _logger As AcciseLogger
    Private ReadOnly _configMin As AcciseServiceConfigMin
    Private ReadOnly _fileSystemPersister As FileSystemPersister
    Public Sub New(ByVal logger As AcciseLogger,
                   ByVal configMin As AcciseServiceConfigMin,
                   ByVal fileSystemPersister As FileSystemPersister)
        _logger = logger
        _configMin = configMin
        _fileSystemPersister = fileSystemPersister
    End Sub
    Public Function Sign(ByVal plainData As Byte(), ByVal percorsoFile As String) As String Implements ISignHelper.Sign

        Dim cert = LeggiCertificatoDaFile()

        Dim service = New CAdESService()
        Dim token = New MSCAPISignatureToken With {
            .Cert = cert
        }
        Dim parameters = New SignatureParameters With {
            .SignatureAlgorithm = SignatureAlgorithm.RSA,
            .SignatureFormat = SignatureFormat.CAdES_BES,
            .DigestAlgorithm = DigestAlgorithm.SHA256,
            .SignaturePackaging = SignaturePackaging.ENVELOPING,
            .SigningCertificate = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(token.Cert),
            .SigningDate = DateTime.UtcNow
        }

        Dim toBeSigned = New FileDocument(percorsoFile)

        Dim iStream = service.ToBeSigned(toBeSigned, parameters)
        Dim signatureValue = token.Sign(iStream, parameters.DigestAlgorithm, token.GetKeys()(0))
        Dim signedDocument = service.SignDocument(toBeSigned, parameters, signatureValue)

        Dim percorsoFileSegnato = _fileSystemPersister.SaveDAASigned(signedDocument, Path.GetFileNameWithoutExtension(percorsoFile) + ".p7m")
        iStream.Close()
        iStream.Dispose()

        Return percorsoFileSegnato

    End Function

    Public Sub Verify() Implements ISignHelper.Verify
        Throw New NotImplementedException()
    End Sub

    Public Function LeggiContenutoSegnato(ByVal data As Byte(), ByVal nomeFileRicevuta As String) As MemoryStream Implements ISignHelper.LeggiContenutoSegnato

        Dim nomeProcedura = "SignHelper.LeggiContenutoSegnato"
        Try
            Dim cms = New CmsSignedData(data)
            If Not cms.SignedContent Is Nothing Then
                Dim ms = New MemoryStream()
                cms.SignedContent.Write(ms)
                ms.Flush()
                Return ms
            Else
                _logger.Logga(nomeProcedura, String.Format("Non è stato possibile leggere il contenuto segnato del file messaggio {0}", nomeFileRicevuta))
                Return Nothing
            End If
        Catch ex As Exception
            _logger.Logga(nomeProcedura, ex.Message)
            Return Nothing
        End Try
        Return New MemoryStream()

    End Function

    Public Function LeggiCertificatoDaFile() As X509Certificate2

        Try

            Dim fullFilePath = Path.Combine(_configMin.PercorsoCertificato, "keystore.p12")

            Dim clientCertificate As X509Certificate2 = New X509Certificate2(
                    File.ReadAllBytes(fullFilePath), _configMin.ChiaveCertificato,
                    X509KeyStorageFlags.Exportable Or X509KeyStorageFlags.MachineKeySet Or X509KeyStorageFlags.PersistKeySet)
            Return clientCertificate
        Catch ex As Exception
            _logger.Logga("SignHelper.LeggiCertificatoDaFile", ex.Message)
            Throw ex
        End Try

    End Function

    Private Function MsToByteArray(ByVal stream As MemoryStream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)
        Dim fileData As Byte() = New Byte(streamLength) {}

        stream.Read(fileData, 0, streamLength)
        stream.Flush()
        stream.Close()

        Return fileData

    End Function

End Class
