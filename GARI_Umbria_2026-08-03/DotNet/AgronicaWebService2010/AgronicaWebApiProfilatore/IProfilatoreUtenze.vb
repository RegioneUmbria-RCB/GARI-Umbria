Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports AgronicaCoreProfilazioneBIZ




' NOTA: è possibile utilizzare il comando "Rinomina" del menu di scelta rapida per modificare il nome di interfaccia "IProfilatoreUtenze" nel codice e nel file di configurazione contemporaneamente.
<ServiceContract()>
Public Interface IProfilatoreUtenze

    'BodyStyle := WebMessageBodyStyle.Wrapped, ---> serve per usare più di un parametro nelle RestFul API in WCF..


    <OperationContract()>
    <WebInvoke(
        UriTemplate:="widgetManager",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function widgetManager(o As widgetManagerRequest) As widgetManagerResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="accountManagerEsterni",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function accountManagerEsterni(o As AccountManagerRequest) As AccountManagerResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="provisioningDSS",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function provisioningDSS(o As ProvisioningDSSRequest) As ProvisioningDSSResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="accountManagerEstesa",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function accountManagerEstesa(o As AccountManagerEstesaRequest) As AccountManagerResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="accountManager",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function accountManager(o As AccountManagerRequest) As AccountManagerResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="accountEdit",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function accountEdit(o As AccountManagerRequest) As AccountManagerResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="profileManagerEsterni",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function profileManagerEsterni(o As ProfileManagerRequest) As ProfileManagerResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="profileManager",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function profileManager(o As ProfileManagerRequest) As ProfileManagerResponse


    <OperationContract()>
    <WebInvoke(
        UriTemplate:="autenticazioneDemetra",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function autenticazioneDemetra(o As AuthenticationRequestDemetra) As AuthenticationResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="autenticazioneNewAgri",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function autenticazioneNewAgri(o As AuthenticationRequestDemetra) As AuthenticationResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="authentication",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function authentication(o As AuthenticationRequest) As AuthenticationResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="autenticazione",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function autenticazione(o As AuthenticationRequest) As AuthenticationResponse

    <OperationContract()>
    <WebInvoke(
        UriTemplate:="accounting",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function accounting(o As AccountingRequest) As AccountingResponse


    <OperationContract()>
    <WebInvoke(
        UriTemplate:="accountingUser",
        RequestFormat:=WebMessageFormat.Json,
        ResponseFormat:=WebMessageFormat.Json,
        Method:="POST")>
    Function accountingUser(o As AccountingUserRequest) As AccountingUserResponse



    '<OperationContract()>
    '<WebInvoke(
    '      UriTemplate:="passwordReset",
    '      RequestFormat:=WebMessageFormat.Json,
    '      ResponseFormat:=WebMessageFormat.Json,
    '      Method:="POST")>
    'Function passwordReset(o As GenericRequest) As AccountManagerResponse

End Interface
