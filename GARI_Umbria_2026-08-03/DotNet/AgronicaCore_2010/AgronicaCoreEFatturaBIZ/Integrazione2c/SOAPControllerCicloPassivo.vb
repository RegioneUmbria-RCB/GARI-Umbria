Imports System.ServiceModel
Imports AgronicaCoreEFatturaBIZ.Integrazione2c.Passivo
Imports AgronicaCoreEFatturaBIZ.Persisters

Public Class SOAPControllerCicloPassivo : Implements ISOAPControllerCicloPassivo

    Private ReadOnly _2cClient As FatturaPassivaPAClient
    Private ReadOnly _codiceCliente As String
    Private ReadOnly _passWordServizi As String
    Private ReadOnly _encodedPassword As Byte()
    Private ReadOnly _fileManager As IFIleManager
    Private ReadOnly _fileSystemPersister As IPersister
    Private ReadOnly _SDI_CicloPassivo_LeggiFatture As SDI_CicloPassivo_LeggiFatture
    Public Sub New(
            ByVal codiceCliente As String,
            ByVal passWordServizi As String,
            ByVal serviceEnpoint As String,
            ByVal serviceTimeout As TimeSpan,
            ByVal fileManager As IFIleManager,
            ByVal fileSystemPersister As IPersister
        )

        Dim binding = New BasicHttpBinding With {
            .Name = "BasicHttpsBinding_IFatturaPassivaPA"
        }
        binding.Security.Mode = BasicHttpSecurityMode.Transport
        binding.MaxBufferPoolSize = 2147000000
        binding.MaxBufferSize = 2147000000
        binding.MaxReceivedMessageSize = 2147000000
        binding.ReaderQuotas = New Xml.XmlDictionaryReaderQuotas With
        {
            .MaxDepth = 32,
            .MaxStringContentLength = 2147000000,
            .MaxArrayLength = 2147000000,
            .MaxBytesPerRead = 4096,
            .MaxNameTableCharCount = 16384
        }
        binding.ReceiveTimeout = serviceTimeout
        Dim theEndpoint = New EndpointAddress(serviceEnpoint)

        _2cClient = New FatturaPassivaPAClient(binding, theEndpoint)

        _codiceCliente = codiceCliente
        _passWordServizi = passWordServizi
        _encodedPassword = System.Text.Encoding.Default.GetBytes(_passWordServizi)
        _fileManager = fileManager
        _fileSystemPersister = fileSystemPersister

        _SDI_CicloPassivo_LeggiFatture = New SDI_CicloPassivo_LeggiFatture(Me, fileManager, fileSystemPersister)

    End Sub

    Public Function LeggiFatture(ByVal dataInizio As Date) As Integer Implements ISOAPControllerCicloPassivo.LeggiFatture

        Return _SDI_CicloPassivo_LeggiFatture.LeggiFatture(dataInizio)

    End Function

    Public Function ContattoHub() As Boolean Implements ISOAPControllerCicloPassivo.ContattoHub

        Dim messaggioErrore As String = ""
        Dim errori As String = String.Empty
        Dim nomeRoutine As String = "SOAPControllerCicloPassivo.ContattoHub"

        Dim response = False

        Try

            Dim request = New ContattoRequest With
            {
                .codiceCliente = _codiceCliente,
                .passwordServizi = _encodedPassword
            }

            response = _2cClient.Contatto(request).ContattoResult

        Catch ex As Exception
            messaggioErrore = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            errori &= "[" & nomeRoutine & "] : " & messaggioErrore & "</br>"
        End Try

        Return response


    End Function

    Public Function CheckClienteFatturaPA() As Boolean Implements ISOAPControllerCicloPassivo.CheckClienteFatturaPA

        Dim messaggioErrore As String = ""
        Dim errori As String = String.Empty
        Dim nomeRoutine As String = "SOAPControllerCicloPassivo.CheckClienteFatturaPA"

        Dim response = False

        Try

            Dim request = New GetAutorizzazioniRequest With
            {
                .codiceCliente = _codiceCliente,
                .passwordServizi = _encodedPassword
            }

            Dim result = _2cClient.GetAutorizzazioni(request).GetAutorizzazioniResult

            Return True

        Catch ex As Exception
            messaggioErrore = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            errori &= "[" & nomeRoutine & "] : " & messaggioErrore & "</br>"
        End Try

        Return response

    End Function

    Public Function GetFatture(ByVal dataInizio As Date) As List(Of DatiFattura) Implements ISOAPControllerCicloPassivo.GetFatture

        Dim fattureRequest = New GetFattureRequest()

        ' filtro per data inizio
        If dataInizio > Date.MinValue Then
            fattureRequest.DataInizio = dataInizio
        End If

        Dim request = New GetFattureParamRequest With
            {
                .codiceCliente = _codiceCliente,
                .passwordServizi = _encodedPassword,
                .request = fattureRequest
            }

        Dim result = _2cClient.GetFattureParam(request).GetFattureParamResult

        Return result

    End Function

    Public Function GetFattura(ByVal idSDI As Long, ByVal codiceUfficio As String) As Fattura Implements ISOAPControllerCicloPassivo.GetFattura


        Dim request = New GetFatturaRequest With
        {
            .codiceCliente = _codiceCliente,
            .passwordServizi = _encodedPassword,
            .idSdi = idSDI,
            .codiceUfficio = codiceUfficio
        }

        Dim result = _2cClient.GetFattura(request).GetFatturaResult

        Return result


    End Function
End Class
