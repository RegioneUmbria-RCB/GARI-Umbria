Imports System.Security.Cryptography

Public Class Login

    Public Shared Function GeneraTokenCasuale() As String

        Dim g1() As Byte = New Byte(25) {}
        Dim gen As RandomNumberGenerator = RandomNumberGenerator.Create()
        gen.GetBytes(g1)


        Dim s As String =
        Convert.ToBase64String(g1).Replace("+", "A").Replace("/", "B").Replace("=", "C")


        Return s

    End Function

End Class
