Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace Italia.Spid.Authentication.IdP
    'Module IdentityProvidersList
    '    Public Property IdpList As List(Of IdentityProvider)

    '    Sub IdentityProvidersListFactory(ByVal idpConfigDataList As List(Of IdentityProviderConfigData))
    '        IdentityProvidersListFactory(Nothing, idpConfigDataList)
    '    End Sub

    '    Sub IdentityProvidersListFactory(ByVal idpMetaDataList As List(Of IdentityProviderMetaData), ByVal Optional idpConfigDataList As List(Of IdentityProviderConfigData) = Nothing)
    '        IdpList = New List(Of IdentityProvider)()

    '        If idpMetaDataList?.Count > 0 Then

    '            For Each idpMetaData In idpMetaDataList
    '                IdpList.Add(New IdentityProvider(entityId:=idpMetaData.EntityId, organizationName:=idpMetaData.OrganizationName, organizationDisplayName:=idpMetaData.OrganizationDisplayName, organizationUrl:=idpMetaData.OrganizationUrl, singleSignOnServiceUrl:=idpMetaData.SingleSignOnServiceUrl, singleLogoutServiceUrl:=idpMetaData.SingleLogoutServiceUrl, subjectNameIdRemoveText:=String.Empty, dateTimeFormat:="yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'", nowDelta:=0))
    '            Next
    '        End If

    '        If idpConfigDataList.Count > 0 Then

    '            For Each idpConfigData In idpConfigDataList

    '                If String.IsNullOrWhiteSpace(idpConfigData.EntityId) Then
    '                    Throw New ArgumentNullException("The EntityId property of a idpConfigData (Identity Provider configuration data) item can't be null.")
    '                End If

    '                Dim foundElement = IdpList.FirstOrDefault(Function(x) x.EntityID = idpConfigData.EntityId)

    '                If foundElement IsNot Nothing Then

    '                    If Not String.IsNullOrWhiteSpace(idpConfigData.SingleSignOnServiceUrl) Then
    '                        foundElement.ConfigOverrideSpidServiceUrl(idpConfigData.SingleSignOnServiceUrl)
    '                    End If

    '                    If Not String.IsNullOrWhiteSpace(idpConfigData.SingleLogoutServiceUrl) Then
    '                        foundElement.ConfigOverrideLogoutServiceUrl(idpConfigData.SingleLogoutServiceUrl)
    '                    End If

    '                    If Not String.IsNullOrWhiteSpace(idpConfigData.DateTimeFormat) Then
    '                        foundElement.ConfigOverrideDateTimeFormat(idpConfigData.DateTimeFormat)
    '                    End If

    '                    foundElement.ConfigOverrideNowDelta(idpConfigData.NowDelta)
    '                Else

    '                    If String.IsNullOrWhiteSpace(idpConfigData.SingleSignOnServiceUrl) Then
    '                        Throw New ArgumentNullException("When adding a IdP from Config, the SpidServiceUrl property can't be null or empty.")
    '                    End If

    '                    If String.IsNullOrWhiteSpace(idpConfigData.SingleLogoutServiceUrl) Then
    '                        Throw New ArgumentNullException("When adding a IdP from Config, the LogoutServiceUrl property can't be null or empty.")
    '                    End If

    '                    Dim subjectNameIdRemoveText As String = If(String.IsNullOrWhiteSpace(idpConfigData.SubjectNameIdRemoveText), String.Empty, idpConfigData.SubjectNameIdRemoveText)
    '                    Dim nowFormatText As String = If(String.IsNullOrWhiteSpace(idpConfigData.DateTimeFormat), "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'", idpConfigData.DateTimeFormat)
    '                    IdpList.Add(New IdentityProvider(entityId:=idpConfigData.EntityId, organizationName:=idpConfigData.OrganizationName, organizationDisplayName:=idpConfigData.OrganizationDisplayName, organizationUrl:=idpConfigData.OrganizationUrl, singleSignOnServiceUrl:=idpConfigData.SingleSignOnServiceUrl, singleLogoutServiceUrl:=idpConfigData.SingleLogoutServiceUrl, subjectNameIdRemoveText:=subjectNameIdRemoveText, dateTimeFormat:=nowFormatText, nowDelta:=idpConfigData.NowDelta))
    '                End If
    '            Next
    '        End If
    '    End Sub

    '    'Async Function GetIdpMetaDataListAsync(ByVal idpMetadataListUrl As String) As Task(Of List(Of IdentityProviderMetaData))
    '    '    Dim ipdMetaDataList As List(Of IdentityProviderMetaData) = New List(Of IdentityProviderMetaData)()

    '    '    If Not String.IsNullOrWhiteSpace(idpMetadataListUrl) Then
    '    '    End If

    '    '    Return ipdMetaDataList
    '    'End Function

    '    Function GetIdpFromIdPName(ByVal idpName As String) As IdentityProvider
    '        If String.IsNullOrWhiteSpace(idpName) Then
    '            Throw New ArgumentNullException("The idpName parameter can't be null.")
    '        End If

    '        Dim idp As IdentityProvider = IdpList?.FirstOrDefault(Function(x) x.EntityID = idpName)

    '        If idp Is Nothing Then
    '            Throw New Exception($"Error on GetIdpFromUserChoice: Identity Provider {idpName} not found.")
    '        End If

    '        If String.IsNullOrWhiteSpace(idp.SingleSignOnServiceUrl) Then
    '            Throw New Exception($"Error on GetIdpFromUserChoice: Identity Provider {idpName} doesn't have a login endpoint.")
    '        End If

    '        If String.IsNullOrWhiteSpace(idp.SingleLogoutServiceUrl) Then
    '            Throw New Exception($"Error on GetIdpFromUserChoice: Identity Provider {idpName} doesn't have a logout endpoint.")
    '        End If

    '        Return idp
    '    End Function
    'End Module
End Namespace
