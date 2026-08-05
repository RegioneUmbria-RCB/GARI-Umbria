Imports System.Collections.Generic

Namespace Italia.Spid.Authentication
    Module SpidUserInfoHelper
        Function FullName(ByVal spidUserInfo As Dictionary(Of String, String)) As String
            Dim fullname1 As String = String.Empty
            Dim name1 As String = Name(spidUserInfo)

            If name1 <> "N/A" Then
                fullname1 = name1
            End If

            Dim familyName1 As String = FamilyName(spidUserInfo)

            If familyName1 <> "N/A" Then
                fullname1 &= " " & familyName1
            End If

            If String.IsNullOrWhiteSpace(fullname1) Then
                fullname1 = "N/A"
            End If

            Return fullname1.Trim()
        End Function

        Function Name(ByVal spidUserInfo As Dictionary(Of String, String)) As String
            Try
                Return spidUserInfo("name")
            Catch
                Return "N/A"
            End Try
        End Function

        Function FamilyName(ByVal spidUserInfo As Dictionary(Of String, String)) As String
            Try
                Return spidUserInfo("familyName")
            Catch
                Return "N/A"
            End Try
        End Function

        Function FiscalNumber(ByVal spidUserInfo As Dictionary(Of String, String)) As String
            Try
                Return spidUserInfo("fiscalNumber")
            Catch
                Return "N/A"
            End Try
        End Function

        Function Email(ByVal spidUserInfo As Dictionary(Of String, String)) As String
            Try
                Return spidUserInfo("email")
            Catch
                Return "N/A"
            End Try
        End Function
    End Module
End Namespace
