Imports System.IO
Imports System.ServiceModel
Imports AgronicaCoreEFatturaBIZ.Integrazione2c.Attivo

Public Class SOAPControllerCicloAttivo : Implements ISOAPControllerCicloAttivo

    Private ReadOnly _2cClient As SolutionDOC_HubSoapClient
    Private ReadOnly _codiceCliente As String
    Private ReadOnly _passWordServizi As String
    Private ReadOnly _encodedPassword As Byte()
    Private ReadOnly _fileManager As IFIleManager
    Private ReadOnly _SDI_CicloAttivo_InviaFattura As SDI_CicloAttivo_InviaFattura
    Private ReadOnly _SDI_CicloAttivo_LetturaEsiti As SDI_CicloAttivo_LetturaEsiti
    Private ReadOnly _logger As EFatturaLogger
    Public Sub New(
            ByVal codiceCliente As String,
            ByVal passWordServizi As String,
            ByVal serviceEnpoint As String,
            ByVal serviceTimeout As TimeSpan,
            ByVal fileManager As IFIleManager,
            ByVal logger As EFatturaLogger
        )

        Dim binding = New BasicHttpBinding With {
            .Name = "SolutionDOC_HubSoap"
        }
        binding.Security.Mode = BasicHttpSecurityMode.Transport
        binding.ReceiveTimeout = serviceTimeout

        Dim theEndpoint = New EndpointAddress(New Uri(serviceEnpoint))

        _2cClient = New SolutionDOC_HubSoapClient(binding, theEndpoint)
        _codiceCliente = codiceCliente
        _passWordServizi = passWordServizi
        _encodedPassword = System.Text.Encoding.Default.GetBytes(_passWordServizi)
        _fileManager = fileManager
        _logger = logger

        _SDI_CicloAttivo_InviaFattura = New SDI_CicloAttivo_InviaFattura(Me, fileManager, _logger)
        _SDI_CicloAttivo_LetturaEsiti = New SDI_CicloAttivo_LetturaEsiti(Me, fileManager, _logger)

    End Sub

    Public Function InviaFattura() As Boolean Implements ICicloAttivo.InviaFattura
        Return _SDI_CicloAttivo_InviaFattura.InviaFattura()
    End Function

    Public Function LeggiEsitiFattura(ByVal idSDI As String, ByRef response As InvoiceOutcomeResponseWrapper) As Boolean Implements ICicloAttivo.LeggiEsitiFattura
        Return _SDI_CicloAttivo_LetturaEsiti.LeggiEsitiFattura(idSDI, response)
    End Function

    Public Function InviaFattura(nomeFileXml As String, ByRef response As SendInvoiceResponseWrapper) As Boolean Implements ICicloAttivo.InviaFattura
        Return _SDI_CicloAttivo_InviaFattura.InviaFattura(nomeFileXml, response)
    End Function

    Public Function GetNomeFileZipFatturaPA() As String Implements ISOAPControllerCicloAttivo.GetNomeFileZipFatturaPA

        Dim messaggioErrore As String = ""
        Dim errori As String = String.Empty
        Dim nomeRoutine As String = "SOAPControllerCicloAttivo.GetNomeFileZipFatturaPA"

        Dim response As String = String.Empty

        Try
            Dim request As New GetNomeFileZipFatturaPARequest With
            {
                .codiceCliente = _codiceCliente,
                .passwordServizi = _encodedPassword
            }
            response = _2cClient.GetNomeFileZipFatturaPA(request).GetNomeFileZipFatturaPAResult

            If String.IsNullOrEmpty(response) OrElse response.ToLowerInvariant().Contains("error") Then
                Throw New InvioFatturaException("Errore nel recupero nome file Zip")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            errori &= "[" & nomeRoutine & "] : " & messaggioErrore & "</br>"
        End Try

        Return response

    End Function

    Public Function ContattoHub() As Boolean Implements ISOAPControllerCicloAttivo.ContattoHub

        Dim nomeProcedura As String = "SOAPControllerCicloAttivo.ContattoHub"
        Dim response = False

        Try

            Dim request = New ContattoHubRequest()

            Dim isRunning = _2cClient.ContattoHub(request).ContattoHubResult
            If String.IsNullOrEmpty(isRunning) Then Return False
            response = isRunning.Equals("0")

        Catch ex As Exception
            _logger.Logga(nomeProcedura, ex)
        End Try

        Return response

    End Function

    Public Function UploadFileFatturaPA(zipFileFullPath As String) As Object Implements ISOAPControllerCicloAttivo.UploadFileFatturaPA

        Dim response = String.Empty

        Try
            response = UploadFileFatturaPAUniqueChunk(zipFileFullPath)

        Catch ex As Exception

        End Try

        Return response

    End Function

    Public Function SendElectronicInvoice(zipFileFullPath As String, emails As String) As SendElectronicInvoiceResponse _
        Implements ISOAPControllerCicloAttivo.SendElectronicInvoice

        Dim result As SendElectronicInvoiceResponse

        Try
            Dim fileName = Path.GetFileName(zipFileFullPath)

            Dim auth = New Auth With
            {
                .CustomerCode = _codiceCliente,
                .Password = _encodedPassword
            }

            Dim paramInvoice = New SendInvoice With {
                .Filename = fileName,
                .ToSign = True,
                .paramInfo = Nothing,
                .paramEmail = Nothing
            }

            If Not String.IsNullOrEmpty(emails) Then
                Dim paramEmail = New SendEmail With {
                    .Send = True,
                    .Address = (New List(Of String)(emails.Split(";"c))).ToArray()
                }
                paramInvoice.paramEmail = paramEmail
            End If

            Dim request = New SendElectronicInvoiceRequest With
            {
                .paramAuth = auth,
                .paramInvoice = paramInvoice
            }

            result = _2cClient.SendElectronicInvoice(request)

        Catch ex As Exception
            Throw ex
        End Try

        Return result

    End Function

    Public Function CheckClienteFatturaPA(ByRef errore As String) As Boolean Implements ISOAPControllerCicloAttivo.CheckClienteFatturaPA

        Dim nomeProcedura As String = "SOAPControllerCicloAttivo.CheckClienteFatturaPA"

        Dim response = False

        Try

            Dim request = New CheckClienteFatturaPARequest With
            {
                .codiceCliente = _codiceCliente,
                .passwordServizi = _encodedPassword
            }

            Dim _2cResponse = _2cClient.CheckClienteFatturaPA(request).CheckClienteFatturaPAResult

            If _2cResponse.Equals("0") Then
                response = True
            Else
                errore = String.Format("La chiamata CheckClienteFatturaPA ha risposto col seguente errore: {0}", _2cResponse)
                _logger.Logga(nomeProcedura, errore)
            End If

        Catch ex As Exception
            errore = String.Format("La chiamata CheckClienteFatturaPA ha risposto col seguente errore: {0}", ex.Message)
            _logger.Logga(nomeProcedura, ex)
        End Try

        Return response

    End Function

    Private Function UploadFileFatturaPAUniqueChunk(ByVal zipFileFullPath As String) As String

        Dim response = String.Empty

        If Not File.Exists(zipFileFullPath) Then
            Throw New Exception(String.Format("Impossibile trovare il file [{0}]", zipFileFullPath))
        End If

        Dim fileByteArray = File.ReadAllBytes(zipFileFullPath)

        Try

            Dim request = New UploadFileFatturaPARequest With
            {
                .codiceCliente = _codiceCliente,
                .passwordServizi = _encodedPassword,
                .nomeFile = Path.GetFileName(zipFileFullPath),
                .buffer = fileByteArray,
                .offset = 0
            }

            response = _2cClient.UploadFileFatturaPA(request).UploadFileFatturaPAResult

        Catch ex As Exception
            Throw ex
        End Try

        Return response

    End Function
    Public Function GetElectronicInvoiceOutcomes(idSDI As Long) As ElectronicInvoiceOutcomeResponse Implements ISOAPControllerCicloAttivo.GetElectronicInvoiceOutcomes

        Dim response As ElectronicInvoiceOutcomeResponse = Nothing

        Dim auth = New Auth With
            {
                .CustomerCode = _codiceCliente,
                .Password = _encodedPassword
            }

        Dim filter = New ElectronicInvoiceOutcomeFilter With
                    {
                        .IdSdi = idSDI
                    }

        Dim request = New GetElectronicInvoiceOutcomesRequest With
            {
                .paramAuth = auth,
                .paramFilter = filter
            }

        response = _2cClient.GetElectronicInvoiceOutcomes(request).GetElectronicInvoiceOutcomesResult

        Return response

    End Function

    Public Function GetFileElectronicInvoiceOutcome(idOutcome As String) As FileResponse Implements ISOAPControllerCicloAttivo.GetFileElectronicInvoiceOutcome

        Dim response As FileResponse = Nothing

        Dim auth = New Auth With
           {
               .CustomerCode = _codiceCliente,
               .Password = _encodedPassword
           }

        Dim outcome = New FileOutcome With
                {
                    .IdOutcome = idOutcome
                }

        Dim request = New GetFileElectronicInvoiceOutcomeRequest With
            {
                .paramAuth = auth,
                .paramFileOutcome = outcome
            }

        response = _2cClient.GetFileElectronicInvoiceOutcome(request).GetFileElectronicInvoiceOutcomeResult
        Return response

    End Function
End Class
