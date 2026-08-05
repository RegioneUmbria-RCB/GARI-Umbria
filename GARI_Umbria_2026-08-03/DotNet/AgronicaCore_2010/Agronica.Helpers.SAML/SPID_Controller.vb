Imports System.Security.Cryptography.X509Certificates
Imports Agronica.Helpers.SAML.Italia.Spid.Authentication
Imports Agronica.Helpers.SAML.Italia.Spid.Authentication.IdP
Imports Agronica.Helpers.SAML.Italia.Spid.Authentication.Saml
Imports Agronica.Helpers.SAML.Italia.Spid.Authentication.Schema
Imports Newtonsoft.Json.Linq

Public Class SPID_Controller
    Public WindowsfindBy As X509FindType = X509FindType.FindBySubjectName
    Public SPID_CERTIFICATE_NAME As String
    Public SPID_DOMAIN_VALUE As String
    Public ENVIROMENT As String


    Public Sub New(_SPID_CERTIFICATE_NAME As String, _SPID_DOMAIN_VALUE As String, _ENVIROMENT As String, ByVal _WindowsfindBy As X509FindType)
        SPID_CERTIFICATE_NAME = _SPID_CERTIFICATE_NAME
        SPID_DOMAIN_VALUE = _SPID_DOMAIN_VALUE
        ENVIROMENT = _ENVIROMENT
        WindowsfindBy = _WindowsfindBy
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="idp_ID"></param>
    ''' <param name="AssertionConsumerServiceURL"></param>
    ''' <param name="comparisonType">Minimum per l'umbria e Exact per Zespri</param>
    ''' <returns></returns>
    Public Function SpidRequest(idp_ID As String, AssertionConsumerServiceURL As String, AgroConfig As AgroSamlConfig) As String

        'Create the SPID request id
        Dim spidAuthnRequestId = Guid.NewGuid().ToString()

        ''Select the Identity Provider
        'Dim idp = IdentityProvidersList.GetIdpFromIdPName(idpName)

        Dim ObjIdp = JObject.Parse(idp_ID)

        Dim nowFormatText = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"
        Dim idp = New IdentityProvider(CStr(ObjIdp("entityId")),
                                       CStr(ObjIdp("organizationName")),
                                       CStr(ObjIdp("organizationDisplayName")),
                                       CStr(ObjIdp("organizationUrl")),
                                       CStr(ObjIdp("singleSignOnServiceUrl")),
                                       CStr(ObjIdp("singleLogoutServiceUrl")),
                                       CStr(ObjIdp("subjectNameIdRemoveText")),
                                       nowFormatText,
                                       0)

        Dim certificate As X509Certificate2 = Nothing
        If AgroConfig.SignAssertion Then
            certificate = X509Helper.GetCertificateFromStore(
                    StoreLocation.LocalMachine, StoreName.My,
                    WindowsfindBy,
                    SPID_CERTIFICATE_NAME,
                    validOnly:=False)
        End If


        Dim env As Integer = 0
        If ENVIROMENT = "dev" Then
            env = 1
        End If

        If AgroConfig.SpidLevel = 0 Then
            AgroConfig.SpidLevel = 1
        End If

        Dim spidAuthnRequest = SamlHelper.BuildAuthnPostRequest(
                    uuid:=spidAuthnRequestId,
                    destination:=idp.EntityID,
                    consumerServiceURL:=SPID_DOMAIN_VALUE,
                    securityLevel:=1,
                    certificate:=certificate,
                    identityProvider:=idp,
                    enviroment:=env,
                    AssertionConsumerServiceURL:=AssertionConsumerServiceURL,
                    AgroConfig:=AgroConfig)

        Return spidAuthnRequest

    End Function

    Public Function LogoutRequest(idp_id As String, AgroConfig As AgroSamlConfig)

        Dim logoutRequestId = Guid.NewGuid().ToString()
        Dim nowFormatText = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"

        Dim idp = New IdentityProvider(idp_id,
                                       "FedGW",
                                       "Federa Gateway",
                                       "http://www.federa.it",
                                       idp_id,
                                       idp_id,
                                       "",
                                       nowFormatText,
                                       0)

        Dim certificate = X509Helper.GetCertificateFromStore(
                    StoreLocation.LocalMachine, StoreName.My,
                    X509FindType.FindBySubjectName,
                    SPID_CERTIFICATE_NAME,
                    validOnly:=False)

        Dim spidLogoutRequest = SamlHelper.BuildLogoutPostRequest(
                    uuid:=logoutRequestId,
                    consumerServiceURL:=SPID_DOMAIN_VALUE,
                    certificate:=certificate,
                    identityProvider:=idp,
                    subjectNameId:="http://www.federa.it",
                    authnStatementSessionIndex:="1",
                    AgroConfig:=AgroConfig)

        Return spidLogoutRequest

    End Function

    Public Function GetAuthnResponse(ByVal base64Response As String, AgroConfig As AgroSamlConfig) As IdpAuthnResponse

        Dim certificate As X509Certificate2 = Nothing
        If AgroConfig.VerifyResponse Then

            certificate = X509Helper.GetCertificateFromStore(
                        StoreLocation.LocalMachine, StoreName.My,
                        WindowsfindBy,
                        SPID_CERTIFICATE_NAME,
                        validOnly:=False)

        End If

        Return SamlHelper.GetAuthnResponse(base64Response, certificate, AgroConfig)

    End Function

End Class
