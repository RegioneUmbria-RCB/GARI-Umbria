Imports System
Imports System.Collections.Generic

Namespace Italia.Spid.Authentication.Schema
    Public Class IdpAuthnResponse
        Public Property Destination As String
        Public Property Id As String
        Public Property InResponseTo As String
        Public Property IssueInstant As DateTimeOffset
        Public Property Version As String
        Public Property Issuer As String
        Public Property StatusCodeValue As String
        Public Property StatusCodeInnerValue As String
        Public Property StatusMessage As String
        Public Property StatusDetail As String
        Public Property AssertionId As String
        Public Property AssertionIssueInstant As DateTimeOffset
        Public Property AssertionVersion As String
        Public Property AssertionIssuer As String
        Public Property SubjectNameId As String
        Public Property SubjectConfirmationMethod As String
        Public Property SubjectConfirmationDataInResponseTo As String
        Public Property SubjectConfirmationDataNotOnOrAfter As DateTimeOffset
        Public Property SubjectConfirmationDataRecipient As String
        Public Property ConditionsNotBefore As DateTimeOffset
        Public Property ConditionsNotOnOrAfter As DateTimeOffset
        Public Property Audience As String
        Public Property AuthnStatementAuthnInstant As DateTimeOffset
        Public Property AuthnStatementSessionIndex As String
        Public Property SpidUserInfo As Dictionary(Of String, String)

        Public ReadOnly Property IsSuccessful As Boolean
            Get
                Return StatusCodeValue = "Success"
            End Get
        End Property

        Public Sub New(ByVal destination As String, ByVal id As String, ByVal inResponseTo As String, ByVal issueInstant As DateTimeOffset, ByVal version As String, ByVal issuer As String, ByVal statusCodeValue As String, ByVal statusCodeInnerValue As String, ByVal statusMessage As String, ByVal statusDetail As String, ByVal assertionId As String, ByVal assertionIssueInstant As DateTimeOffset, ByVal assertionVersion As String, ByVal assertionIssuer As String, ByVal subjectNameId As String, ByVal subjectConfirmationMethod As String, ByVal subjectConfirmationDataInResponseTo As String, ByVal subjectConfirmationDataNotOnOrAfter As DateTimeOffset, ByVal subjectConfirmationDataRecipient As String, ByVal conditionsNotBefore As DateTimeOffset, ByVal conditionsNotOnOrAfter As DateTimeOffset, ByVal audience As String, ByVal authnStatementAuthnInstant As DateTimeOffset, ByVal authnStatementSessionIndex As String, ByVal spidUserInfo As Dictionary(Of String, String))
            Me.Destination = destination
            Me.Id = id
            Me.InResponseTo = inResponseTo
            Me.IssueInstant = issueInstant
            Me.Version = version
            Me.Issuer = issuer
            Me.StatusCodeValue = statusCodeValue
            Me.StatusCodeInnerValue = statusCodeInnerValue
            Me.StatusMessage = statusMessage
            Me.StatusDetail = statusDetail
            Me.AssertionId = assertionId
            Me.AssertionIssueInstant = assertionIssueInstant
            Me.AssertionVersion = assertionVersion
            Me.AssertionIssuer = assertionIssuer
            Me.SubjectNameId = subjectNameId
            Me.SubjectConfirmationMethod = subjectConfirmationMethod
            Me.SubjectConfirmationDataInResponseTo = subjectConfirmationDataInResponseTo
            Me.SubjectConfirmationDataNotOnOrAfter = subjectConfirmationDataNotOnOrAfter
            Me.SubjectConfirmationDataRecipient = subjectConfirmationDataRecipient
            Me.ConditionsNotBefore = conditionsNotBefore
            Me.ConditionsNotOnOrAfter = conditionsNotOnOrAfter
            Me.Audience = audience
            Me.AuthnStatementAuthnInstant = authnStatementAuthnInstant
            Me.AuthnStatementSessionIndex = authnStatementSessionIndex
            Me.SpidUserInfo = spidUserInfo
        End Sub
    End Class
End Namespace
