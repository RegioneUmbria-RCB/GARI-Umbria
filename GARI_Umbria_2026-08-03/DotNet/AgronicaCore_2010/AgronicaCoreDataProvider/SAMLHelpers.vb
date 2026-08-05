Public Class UserLinkedAccount
    Public Username As String
End Class

Public Class UserLinkedAccountRisposta
    Public Accounts As DataTable
    Public MessaggioDialog As String

    Public Sub New(_accounts As DataTable, messaggio As String)
        Accounts = _accounts
        MessaggioDialog = messaggio
    End Sub
End Class
