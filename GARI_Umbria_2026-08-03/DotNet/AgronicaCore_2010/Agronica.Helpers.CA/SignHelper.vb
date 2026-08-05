Imports System.IO
Imports System.Security.Cryptography
Imports System.Security.Cryptography.Pkcs
Imports System.Security.Cryptography.X509Certificates
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports EU.Europa.EC.Markt.Dss
Imports EU.Europa.EC.Markt.Dss.Signature
Imports EU.Europa.EC.Markt.Dss.Signature.Cades
Imports EU.Europa.EC.Markt.Dss.Signature.Token
Imports Org.BouncyCastle.Cms
Imports Org.BouncyCastle.Utilities.Encoders
Imports Org.BouncyCastle.X509.Store

Public Class SignHelper : Implements ISignHelper

    'Private ReadOnly _logger As AcciseLogger
    Private ReadOnly _parametri As Parametri_SignHelper
    Private ReadOnly _chiamante As String
    Private ReadOnly _fileName As String
    Private ReadOnly _piva As String
    Private ReadOnly _objParametri As AgronicaCoreParametri
    Private ReadOnly _objLog As LogProvider
    Private ReadOnly _objCustomLogParams As CustomLOGParams

    Public Sub New(ByVal parametri As Parametri_SignHelper, Optional ByVal piva As String = "", Optional ByVal fileName As String = "", Optional ByVal chiamante As String = "", Optional ByRef objParametri As AgronicaCoreParametri = Nothing)

        '_logger = logger
        _parametri = parametri
        _fileName = fileName
        _chiamante = chiamante
        _piva = piva
        _objParametri = objParametri
        _objLog = New LogProvider
        _objCustomLogParams = New CustomLOGParams
    End Sub

    Public Function CertificateInfo(data As Byte()) As Object

        Dim retVal As Boolean = True

        Try
            Dim cms As New CmsSignedData(data)

            Dim certStore As IX509Store = cms.GetCertificates("Collection")
            Dim certs As ICollection = certStore.GetMatches(New X509CertStoreSelector())

            Dim signerStore As SignerInformationStore = cms.GetSignerInfos()
            Dim signers As ICollection = signerStore.GetSigners()

            For Each tempCertification As Object In certs
                Dim certification As Org.BouncyCastle.X509.X509Certificate = DirectCast(tempCertification, Org.BouncyCastle.X509.X509Certificate)
                For Each tempSigner As Object In signers
                    Dim signer As SignerInformation = DirectCast(tempSigner, SignerInformation)
                    If Not signer.Verify(certification.GetPublicKey) Then
                        retVal = False
                        Exit For
                    End If

                Next

            Next

        Catch ex As Exception
            Return False
        End Try

        Return retVal

    End Function

    Public Function Segna(ByVal dati As Byte()) As String Implements ISignHelper.Segna

        Dim percorsoFileDaSegnare As String = Crea_File_Temporaneo_Da_Segnare(dati)
        Return Segna(percorsoFileDaSegnare)

    End Function

    Function Segna(ByVal percorsoFileCompleto As String) As String Implements ISignHelper.Segna

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

        Dim toBeSigned = New FileDocument(percorsoFileCompleto)

        Dim iStream = service.ToBeSigned(toBeSigned, parameters)
        Dim signatureValue = token.Sign(iStream, parameters.DigestAlgorithm, token.GetKeys()(0))
        Dim signedDocument = service.SignDocument(toBeSigned, parameters, signatureValue)

        Dim percorsoFileSegnato = Salva_File_Segnato(signedDocument, Path.GetFileNameWithoutExtension(percorsoFileCompleto) & ".p7m")
        iStream.Close()
        iStream.Dispose()

        Return percorsoFileSegnato

    End Function

    Public Function LeggiContenutoSegnato(ByVal data As Byte()) As MemoryStream Implements ISignHelper.LeggiContenutoSegnato

        Dim nomeProcedura = "SignHelper.LeggiContenutoSegnato"
        Try
            Dim cms = New CmsSignedData(data)
            If cms.SignedContent IsNot Nothing Then
                Dim ms = New MemoryStream()
                cms.SignedContent.Write(ms)
                ms.Flush()
                Return ms
            Else
                '_logger.Logga(nomeProcedura, String.Format("Non è stato possibile leggere il contenuto segnato del file messaggio {0}", nomeFileRicevuta))
                Return Nothing
            End If
        Catch ex As Exception
            '_logger.Logga(nomeProcedura, ex.Message)
            Return Nothing
        End Try
        Return New MemoryStream()

    End Function

    Public Function LeggiContenutoSegnato(ByVal percorsoFileCompleto As String) As MemoryStream Implements ISignHelper.LeggiContenutoSegnato

        Dim nomeProcedura = "SignHelper.LeggiContenutoSegnato"

        If Not File.Exists(percorsoFileCompleto) Then
            Return Nothing
        End If


        Dim dati = File.ReadAllBytes(percorsoFileCompleto)
        Return LeggiContenutoSegnato(dati)

    End Function

    Public Function VerificaFirmaValida(dati As Byte()) As Boolean Implements ISignHelper.VerificaFirmaValida

        Dim nomeProcedura = "SignHelper.VerificaFirmaValida"
        Dim retVal As Boolean = True

        Try
            Dim cms As New CmsSignedData(dati)

            Dim certStore As IX509Store = cms.GetCertificates("Collection")
            Dim certs As ICollection = certStore.GetMatches(New X509CertStoreSelector())

            Dim signerStore As SignerInformationStore = cms.GetSignerInfos()
            Dim signers As ICollection = signerStore.GetSigners()

            For Each tempCertification As Object In certs
                Dim certification As Org.BouncyCastle.X509.X509Certificate = DirectCast(tempCertification, Org.BouncyCastle.X509.X509Certificate)
                For Each tempSigner As Object In signers
                    Dim signer As SignerInformation = DirectCast(tempSigner, SignerInformation)
                    If Not signer.Verify(certification.GetPublicKey) Then
                        retVal = False
                        Exit For
                    End If

                Next

            Next

        Catch ex As Exception
            Return False
        End Try

        Return retVal

    End Function

    Public Function VerificaFirmaValida(ByVal percorsoFileCompleto As String) As Boolean Implements ISignHelper.VerificaFirmaValida

        If Not File.Exists(percorsoFileCompleto) Then
            Return Nothing
        End If

        Dim dati = File.ReadAllBytes(percorsoFileCompleto)
        Return VerificaFirmaValida(dati)

    End Function

    Public Function Verifica_Se_Firmato(ByVal dati As Byte()) As Boolean Implements ISignHelper.Verifica_Se_Firmato

        Dim nomeProcedura = "SignHelper.Verifica_Se_Firmato"

        Try
            Dim cms As New CmsSignedData(dati)
            Return True
        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Public Function Verifica_Se_Firmato_Byte(ByVal dati As Byte(), Optional ByRef exceptionMsg As String = "") As Boolean
        Dim nomeProcedura = "SignHelper.Verifica_Se_Firmato_Byte"

        Try
            Dim cms As New CmsSignedData(dati)
            Return True
        Catch ex As Exception
            exceptionMsg = ex.Message
            If ex.InnerException IsNot Nothing Then
                exceptionMsg &= " - Inner exception:" & ex.InnerException.Message
            End If
            Return False
        End Try

        Return True

    End Function


    Function Verifica_Se_Firmato(ByVal percorsoFileCompleto As String) As Boolean Implements ISignHelper.Verifica_Se_Firmato

        If Not File.Exists(percorsoFileCompleto) Then
            Return False
        End If

        Dim dati = File.ReadAllBytes(percorsoFileCompleto)
        Return Verifica_Se_Firmato(dati)

    End Function

    Private Function Crea_File_Temporaneo_Da_Segnare(ByVal dati As Byte()) As String

        Dim nomeFileTemp As String = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) & ".tmp"
        Dim percorsoCompleto = Path.Combine(_parametri.Percorso_File_Temporanei, nomeFileTemp)
        File.WriteAllBytes(percorsoCompleto, dati)
        Return percorsoCompleto

    End Function

    Private Function Salva_File_Segnato(ByVal documentoSegnato As Document, ByVal nomeFile As String) As String

        Dim percorsoCompleto = Path.Combine(_parametri.Percorso_File_Segnati, nomeFile)
        If (File.Exists(percorsoCompleto)) Then
            File.Delete(percorsoCompleto)
        End If

        Dim fout = File.OpenWrite(percorsoCompleto)

        documentoSegnato.OpenStream().CopyTo(fout)
        fout.Close()
        fout.Dispose()

        Return percorsoCompleto

    End Function

    Private Function LeggiCertificatoDaFile() As X509Certificate2

        Try

            Dim fullFilePath = Path.Combine(_parametri.Percorso_Certificato, "keystore.p12")

            Dim clientCertificate As New X509Certificate2(
                    File.ReadAllBytes(fullFilePath), _parametri.Chiave_Certificato,
                    X509KeyStorageFlags.Exportable Or X509KeyStorageFlags.MachineKeySet Or X509KeyStorageFlags.PersistKeySet)
            Return clientCertificate
        Catch ex As Exception
            '_logger.Logga("SignHelper.LeggiCertificatoDaFile", ex.Message)
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

    Public Function Verifica_Firma_Valida_NEW(signedData As Byte()) As Boolean

        Dim messaggioErrore As String = ""
        _objCustomLogParams.LogDirectory = FileSystemHelper.AggiungiSlashSeNonEsiste(_objParametri.LogDirectory) & "VerificaFirmaDigitale"
        _objCustomLogParams.LogFileName = "VerificaFirmaDigitale_" & _piva & "_" & Format(Date.Now, "yyyy_MM_dd__hh_mm_ss") & "_" & _fileName.Replace(".", "_") & ".txt"

        Dim decodedData As Byte() = signedData

        Dim nomeRoutine As String = "SignHelper.Verifica_Firma_Valida_NEW" & If(_chiamante <> "", " (" & _chiamante & ")", "")

        Try
            _objLog.Scrivi_LOG(_objParametri, nomeRoutine,
                               String.Format("File '{0}': primo tentativo con classe SignedCms", _fileName),
                               CustomLOGParams:=_objCustomLogParams)

            ' Inizializza SignedCms con i dati firmati
            Dim signedCms As New SignedCms()

            ' Controlla se il contenuto è codificato in Base64
            If IsBase64Encoded(signedData) Then
                decodedData = Base64.Decode(signedData)
            End If

            signedCms.Decode(decodedData)

            ' Verifica la firma
            signedCms.CheckSignature(True)
            signedCms.CheckSignature(False)

            _objLog.Scrivi_LOG(_objParametri, nomeRoutine,
                               String.Format("File '{0}': verifica primo tentativo successo con SignedCms", _fileName),
                               CustomLOGParams:=_objCustomLogParams)

            ' Se la verifica è corretta
            Return True

        Catch ex As CryptographicException

            messaggioErrore = String.Format("File '{0}': verifica primo tentativo fallita con SignedCms", _fileName) & vbCrLf & ex.Message
            If ex.InnerException IsNot Nothing Then
                messaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If
            _objLog.Scrivi_LOG(_objParametri, nomeRoutine,
                               messaggioErrore,
                               CustomLOGParams:=_objCustomLogParams)

            Try
                _objLog.Scrivi_LOG(_objParametri, nomeRoutine,
                                   String.Format("File '{0}': secondo tentativo con classe CmsSignedData (BouncyCastle)", _fileName),
                                   CustomLOGParams:=_objCustomLogParams)

                ' Controlla se il contenuto è codificato in Base64
                If Verifica_Se_Firmato_Byte(signedData, messaggioErrore) Then
                    _objLog.Scrivi_LOG(_objParametri, nomeRoutine,
                                       String.Format("File '{0}': verifica secondo tentativo successo con CmsSignedData (BouncyCastle)", _fileName),
                                       CustomLOGParams:=_objCustomLogParams)
                Else
                    Throw New Exception(messaggioErrore)
                End If

                ' Se la verifica è corretta
                Return True

            Catch ex2 As Exception
                messaggioErrore = String.Format("File '{0}': verifica secondo tentativo fallita con CmsSignedData (BouncyCastle)", _fileName) & vbCrLf & ex2.Message
                If ex2.InnerException IsNot Nothing Then
                    messaggioErrore &= " - Inner exception:" & ex2.InnerException.Message
                End If
                _objLog.Scrivi_LOG(_objParametri,
                                   "Verifica_Firma_Valida_NEW" & If(_chiamante <> "", "(" & _chiamante & ")", ""),
                                   messaggioErrore,
                                   CustomLOGParams:=_objCustomLogParams)

                Return False

            End Try

        End Try
    End Function

    Private Function IsBase64Encoded(data As Byte()) As Boolean
        ' Controlla se i dati sono codificati in Base64 guardando se contengono solo caratteri validi per Base64
        Dim textData As String = System.Text.Encoding.ASCII.GetString(data)
        Return textData.Trim().Replace(Environment.NewLine, "").All(Function(c) Char.IsLetterOrDigit(c) OrElse "+/=".Contains(c))

    End Function

End Class
