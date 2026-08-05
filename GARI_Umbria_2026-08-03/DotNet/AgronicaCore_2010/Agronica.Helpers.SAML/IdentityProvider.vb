Imports System

Namespace Italia.Spid.Authentication.IdP
    Public Class IdentityProvider
        Private subjectNameIdRemoveText As String
        Private dateTimeFormat As String
        Private nowDelta As Double
        Public Property EntityID As String
        Public Property OrganizationName As String
        Public Property OrganizationDisplayName As String
        Public Property OrganizationUrl As String
        Public Property SingleSignOnServiceUrl As String
        Public Property SingleLogoutServiceUrl As String

        Public Sub New(ByVal entityId As String, ByVal organizationName As String, ByVal organizationDisplayName As String, ByVal organizationUrl As String, ByVal singleSignOnServiceUrl As String, ByVal singleLogoutServiceUrl As String, ByVal subjectNameIdRemoveText As String, ByVal dateTimeFormat As String, ByVal nowDelta As Double)
            Me.EntityID = entityId
            Me.OrganizationName = organizationName
            Me.OrganizationDisplayName = organizationDisplayName
            Me.OrganizationUrl = organizationUrl
            Me.SingleSignOnServiceUrl = singleSignOnServiceUrl
            Me.SingleLogoutServiceUrl = singleLogoutServiceUrl
            Me.subjectNameIdRemoveText = subjectNameIdRemoveText
            Me.dateTimeFormat = dateTimeFormat
            Me.nowDelta = nowDelta
        End Sub

        Public Function SubjectNameIdFormatter(ByVal s As String) As String
            Return If((String.IsNullOrWhiteSpace(subjectNameIdRemoveText)), s, s.Replace(subjectNameIdRemoveText, ""))
        End Function

        Public Function Now(ByVal now1 As DateTime) As String
            Return now1.AddMinutes(nowDelta).ToString(dateTimeFormat)
        End Function

        Public Function After(ByVal after1 As DateTime) As String
            Return after1.ToString(dateTimeFormat)
        End Function

        Public Function NotBefore(ByVal now As DateTime) As String
            Return now.AddMinutes(-2).ToString(dateTimeFormat)
        End Function

        Public Sub ConfigOverrideSpidServiceUrl(ByVal singleSignOnServiceUrl As String)
            Me.SingleSignOnServiceUrl = singleSignOnServiceUrl
        End Sub

        Public Sub ConfigOverrideLogoutServiceUrl(ByVal singleLogoutServiceUrl As String)
            Me.SingleLogoutServiceUrl = singleLogoutServiceUrl
        End Sub

        Public Sub ConfigOverrideDateTimeFormat(ByVal dateTimeFormat As String)
            Me.dateTimeFormat = dateTimeFormat
        End Sub

        Public Sub ConfigOverrideNowDelta(ByVal nowDelta As Double)
            Me.nowDelta = nowDelta
        End Sub
    End Class
End Namespace
