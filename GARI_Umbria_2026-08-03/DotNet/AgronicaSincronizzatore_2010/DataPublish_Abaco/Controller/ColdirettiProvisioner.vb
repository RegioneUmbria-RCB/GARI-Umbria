Imports System.Net.Http
Imports Newtonsoft.Json

Public Class ColdirettiProvisioner
    Inherits WebApiCaller
    Implements IDisposable

    Private Const ENDPOINT_UTENTE_PERSONA_GIURIDICA As String = "/anagrafica/v1/anagrafica/persone-giuridiche/SGL_CODFIS/{0}"
    Private Const ENDPOINT_UTENTE_PERSONA_FISICA As String = "/anagrafica/v1/anagrafica/persone-fisiche/SGL_CODFIS/{0}"
    Private Const ENDPOINT_SERVICE_SUBSCRIPTION As String = "/services-provisioning/api/v2/partners/agronica/companies/{0}/service_subscriptions"

    Private Const SERVICE_ACTIVE As Integer = 2

    Private Enum PartnerServiceID
        QuadernoDiCampagnaDemetra = 6
        QuadernoDiCampagnaIV = 8
        QuadernoDiCampagna4Mani = 9
    End Enum

    Public Sub New(ByVal baseUrl As String,
                   ByVal token As Tuple(Of String, String))
        MyBase.New(baseUrl, token, NameOf(ColdirettiProvisioner))
    End Sub

    Public Function GetUserFromCUAA_PersonaGiuridica(ByVal cuaa As String) As UtenteColdiretti
        Return ChiamaWS(Of UtenteColdiretti)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_UTENTE_PERSONA_GIURIDICA, cuaa), False, True)
    End Function
    Public Function GetUserFromCUAA_PersonaFisica(ByVal cuaa As String) As UtenteColdiretti
        Return ChiamaWS(Of UtenteColdiretti)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_UTENTE_PERSONA_FISICA, cuaa), False, True)
    End Function

    Public Function NotificaQDC_OK(ByVal cuaa As String) As Boolean
        Return ChiamaWS(Of Boolean)(HttpMethod.Post, Nothing, Nothing, JsonConvert.SerializeObject(New ProvisioningSubscriptionService() With {.data = New ServiceSubscription() With {.service_id = PartnerServiceID.QuadernoDiCampagnaDemetra, .service_subscription_status_id = SERVICE_ACTIVE}}), "application/json", getUrlEndpoint(ENDPOINT_SERVICE_SUBSCRIPTION, cuaa), False, True)
    End Function

    Public Function NotificaQDC_IV(ByVal cuaa As String) As Boolean
        Return ChiamaWS(Of Boolean)(HttpMethod.Post, Nothing, Nothing, JsonConvert.SerializeObject(New ProvisioningSubscriptionService() With {.data = New ServiceSubscription() With {.service_id = PartnerServiceID.QuadernoDiCampagnaIV, .service_subscription_status_id = SERVICE_ACTIVE}}), "application/json", getUrlEndpoint(ENDPOINT_SERVICE_SUBSCRIPTION, cuaa), False, True)
    End Function

    Public Function NotificaQDC_4Mani(ByVal cuaa As String) As Boolean
        Return ChiamaWS(Of Boolean)(HttpMethod.Post, Nothing, Nothing, JsonConvert.SerializeObject(New ProvisioningSubscriptionService() With {.data = New ServiceSubscription() With {.service_id = PartnerServiceID.QuadernoDiCampagna4Mani, .service_subscription_status_id = SERVICE_ACTIVE}}), "application/json", getUrlEndpoint(ENDPOINT_SERVICE_SUBSCRIPTION, cuaa), False, True)
    End Function

    Private Function getUrlEndpoint(ByVal Endpoint As String, ByVal cuaa As String) As String
        Return String.Format(Endpoint, cuaa)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        MyBase.Dispose()
    End Sub



End Class
