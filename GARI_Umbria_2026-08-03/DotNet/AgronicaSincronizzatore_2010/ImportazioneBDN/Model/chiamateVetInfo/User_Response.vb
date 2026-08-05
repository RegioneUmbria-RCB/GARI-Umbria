Public Class User_Response
    Public userID As Integer
    Public username As String
    Public name As String
    Public surname As String
    Public cf As String
    Public email As String
    Public authenticationLevel As Integer
    Public federato As Boolean
    Public _hash_ As String
    Public profiles As Profile_Response()
    Public oauth2Request As oauth2Request_Response
End Class


Public Class Profile_Response
    Public app As String
    Public role As String
    Public roleDescription As String
    Public description As String
    Public value As Integer
    Public code As String
End Class

Public Class oauth2Request_Response
    Public scope As String()
    Public clientId As String
    Public authorities As String()
End Class