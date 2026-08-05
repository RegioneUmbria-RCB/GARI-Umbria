Imports System.Net
Imports System.ServiceModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.exceptions

Public MustInherit Class wsNogmo

    Protected objParametri_Server As AgronicaCoreParametri
    Protected objParametri_Utenti As AgronicaCoreParametri

    Protected _http_Request As AgronicaCoreUtility.Http = Nothing
    Protected binding As BasicHttpBinding
    Protected theEndpoint As EndpointAddress
    Protected _hdr As WebHeaderCollection = Nothing
    Protected configurazioneNogmo As AgronicaCoreVarieDAL.Configurazione_NOGMO
    Public exporterID As Integer
    Private objLog As LogProvider
    Private logDirectory As String
    Private logFileName As String

    Protected Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        Me.objParametri_Server = objParametri_Server
        Me.objParametri_Utenti = objParametri_Utenti

        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        configurazioneNogmo = confSiti.leggiConfigurazioneNogmo(objParametri_Server)

        If configurazioneNogmo Is Nothing Then
            Throw New GiasException("Collegamento con NOGMO non configurato correttamente, contattare l'assistenza")
        End If
        'End If
        'End If

        If configurazioneNogmo.link = "" Then
            Throw New GiasException("Endpoint servizio NOGMO non configurato correttamente")
        End If

        If configurazioneNogmo.token = "" Then
            Throw New GiasException("Credenziali servizio NOGMO non configurate correttamente")
        End If


        exporterID = configurazioneNogmo.exporterID

        _http_Request = New AgronicaCoreUtility.Http
        _hdr = New WebHeaderCollection
        _hdr.Add("Authorization", "Bearer " & configurazioneNogmo.token)
        initializeLog()
    End Sub


    Sub initializeLog()
        objLog = New LogProvider
        logDirectory = Me.objParametri_Server.LogDirectory & "\NOGMO\"
        logFileName = "chiamateNOGMO.txt"
    End Sub

    Public Function request(body As Object, endPoint As String, httpMethod As RestSharp.Method) As RestSharp.IRestResponse
        'Dim objReq As New JObject()
        'objReq("idDB") = idDB



        Dim currentProtocol As SecurityProtocolType = ServicePointManager.SecurityProtocol
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
#If DEBUG Then
        ServicePointManager.ServerCertificateValidationCallback = AddressOf AcceptAllCertifications
#End If


        Dim resp
        Try

            resp = _http_Request.chiamaWS_RestShapr(body,
                                            "",
                                            configurazioneNogmo.link & endPoint,
                                            "application/json",
            httpMethod, "application/json", "", _hdr, , , True)

        Catch ex As Exception
            'TODO gestire l'eccezione
            resp = Nothing
        Finally
            ServicePointManager.SecurityProtocol = currentProtocol
        End Try

        Return resp
    End Function


    Private Shared Function AcceptAllCertifications(ByVal sender As Object, ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean

        Return True

    End Function


    Protected Function getStrDataFromDate(data As Date) As String
        Dim strData = ""
        If data = AGRODATAINIZIO Then
            Return strData
        End If
        If data = AGRODATAFINE Then
            Return strData
        End If
        strData = data.Day.ToString("D2") & "/" & data.Month.ToString("D2") & "/" & data.Year
        Return strData
    End Function



End Class


Public Class NogmoException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal e As Exception)
        MyBase.New(message, e)
    End Sub

End Class

Public Class ExpiredTokenNogmoException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal e As Exception)
        MyBase.New(message, e)
    End Sub

End Class

Public Class TokenNogmoException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal e As Exception)
        MyBase.New(message, e)
    End Sub

End Class