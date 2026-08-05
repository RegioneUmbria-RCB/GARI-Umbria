Imports System.ServiceModel
Imports System.Text
Imports AgronicaCoreMVVBIZ.Integrazione.SIAN.MVV
Imports Newtonsoft.Json

Public Class SoapController_MVV

    Private ReadOnly _client As wsRegVinoInterServiceClient
    Private ReadOnly _userName As String
    Private ReadOnly _password As String

    Private _codOper As Integrazione.SIAN.MVV.CUAA
    Private _codIcqrf As String

    Public ReadOnly Property CodOper As CUAA
        Get
            Return _codOper
        End Get
    End Property

    Public Sub New(ByVal userName As String,
            ByVal password As String,
            ByVal serviceEnpoint As String,
            ByVal serviceTimeout As TimeSpan
        )

        Dim binding = New BasicHttpBinding With {
           .Name = "wsRegVino"
        }
        binding.Security.Mode = SecurityMode.Transport
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic
        binding.MessageEncoding = WSMessageEncoding.Text
        binding.TextEncoding = Encoding.UTF8
        binding.MaxReceivedMessageSize = 2147000000
        Dim address As New EndpointAddress(New Uri(serviceEnpoint))

        Dim theEndpoint = New EndpointAddress(New Uri(serviceEnpoint))
        _client = New wsRegVinoInterServiceClient(binding, theEndpoint)

        _userName = userName
        _password = password

        _client.ClientCredentials.UserName.UserName = userName ' "ByMn98V"
        _client.ClientCredentials.UserName.Password = password ' "E38f4qfE"

    End Sub

    Public Sub Init(ByVal codOper As String, ByVal personaFisica As Boolean, ByVal codIcqrf As String)

        ' imposto il codice operatore per l'azienda
        _codOper = New CUAA With {
            .Item = codOper,
            .ItemElementName = If(personaFisica, ItemChoiceType.PersonaFisica, ItemChoiceType.PersonaGiuridica)
        }

        _codIcqrf = codIcqrf

    End Sub

    Public Function InviaMvv(ByRef mvv As MVVSiRPVInput, ByRef errori As List(Of String)) As MVVSiRPVResponse

        Dim response As MVVSiRPVResponse = Nothing

        Try

            mvv.CodOper = _codOper

            Dim request As New MVVSiRPVRequest With
            {
                .SOAPAutenticazione = New SOAPAutenticazione With
                {
                    .nomeServizio = "MVVSiRPV",
                    .username = _userName,
                    .password = _password
                },
                .MVVSiRPVInput = mvv
            }

            response = _client.MVVSiRPV(request)

        Catch ex As Exception

            Console.WriteLine(ex.Message)
            errori.Add(ex.Message)

        End Try

        Return response

    End Function

    Public Function ConsultaMvv(ByRef errori As List(Of String)) As String

        Dim messaggio As String = ""

        Try

            Dim request As New VisMVVSiRPVRequest With
            {
                .SOAPAutenticazione = New SOAPAutenticazione With
                {
                    .nomeServizio = "VisMVVSiRPV",
                    .username = _userName,
                    .password = _password
                },
                .VisMVVSiRPVInput = New Integrazione.SIAN.MVV.VisMVVSiRPVInput With {
                    .CodOper = _codOper,
                    .CodIcqrfSped = _codIcqrf
                }
            }

            Dim response = _client.VisMVVSiRPV(request)
            If response.VisMVVSiRPVOutput.Esito IsNot Nothing Then
                errori.Add(response.VisMVVSiRPVOutput.Esito.messaggio)
            Else
                'Dim jsonMMV = JsonConvert.SerializeObject(response.VisMVVSiRPVOutput.ListaMVV).Replace("},{", "},<br>{")
                'messaggio = "Totale MMV-E: <b>" & response.VisMVVSiRPVOutput.ListaMVV.Count & "</b><br><br>" & jsonMMV
                messaggio = JsonConvert.SerializeObject(response.VisMVVSiRPVOutput.ListaMVV)
            End If

        Catch ex As Exception

            Console.WriteLine(ex.Message)
            errori.Add(ex.Message)

        End Try

        Return messaggio

    End Function

    Public Function ScaricaMvv(ByVal numMVV As String, ByRef fileMVV As Byte()) As String

        Dim messaggio As String = ""

        Try

            Dim request As New PrnMVVSiRPVRequest With
            {
                .SOAPAutenticazione = New SOAPAutenticazione With
                {
                    .nomeServizio = "PrnMVVSiRPV",
                    .username = _userName,
                    .password = _password
                },
                .PrnMVVSiRPVInput = New Integrazione.SIAN.MVV.PrnMVVSiRPVInput With {
                    .CodOper = _codOper,
                    .CodIcqrf = _codIcqrf,
                    .NumMVV = numMVV
                }
            }

            Dim response = _client.PrnMVVSiRPV(request)
            If response.PrnMVVSiRPVOutput.filePdf IsNot Nothing Then
                fileMVV = response.PrnMVVSiRPVOutput.filePdf
            End If
            If response.PrnMVVSiRPVOutput.Esito IsNot Nothing Then
                messaggio = response.PrnMVVSiRPVOutput.Esito.messaggio
            End If

        Catch ex As Exception

            Console.WriteLine(ex.Message)
            messaggio = ex.Message

        End Try

        Return messaggio

    End Function

    Public Function AnnullaMvv(ByVal numMVV As String, ByRef errori As List(Of String)) As AnnMVVSiRPVResponse

        Dim response As AnnMVVSiRPVResponse = Nothing

        Try

            Dim request As New AnnMVVSiRPVRequest With
            {
                .SOAPAutenticazione = New SOAPAutenticazione With
                {
                    .nomeServizio = "AnnMVVSiRPV",
                    .username = _userName,
                    .password = _password
                },
                .AnnMVVSiRPVInput = New Integrazione.SIAN.MVV.AnnMVVSiRPVInput With {
                    .CodOper = _codOper,
                    .CodIcqrf = _codIcqrf,
                    .NumMVV = numMVV
                }
            }

            response = _client.AnnMVVSiRPV(request)

        Catch ex As Exception

            Console.WriteLine(ex.Message)
            errori.Add(ex.Message)

        End Try

        Return response

    End Function

End Class
