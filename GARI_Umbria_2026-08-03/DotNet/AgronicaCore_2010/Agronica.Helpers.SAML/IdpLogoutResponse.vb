Imports System

Namespace Italia.Spid.Authentication.IdP
    Public Class IdpLogoutResponse
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

        Public ReadOnly Property IsSuccessful As Boolean
            Get
                Return StatusCodeValue = "Success"
            End Get
        End Property

        Public Sub New(ByVal destination As String, ByVal id As String, ByVal inResponseTo As String, ByVal issueInstant As DateTimeOffset, ByVal version As String, ByVal issuer As String, ByVal statusCodeValue As String, ByVal statusCodeInnerValue As String, ByVal statusMessage As String, ByVal statusDetail As String)
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
        End Sub
    End Class
End Namespace
