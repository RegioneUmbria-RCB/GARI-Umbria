Public Class QueryHelper
    'Funzione copia del QueryHelper presente in ASP.Net core 3++
    Public Shared Function AddQueryString(ByVal BaseUrl As String, ByVal QueryParameters As Dictionary(Of String, String)) As String
        Try
            If QueryParameters Is Nothing Then
                Return BaseUrl
            Else
                Dim StringReturn As String = BaseUrl
                Dim AddStartQueryParamChar As Boolean = True
                For Each key In QueryParameters.Keys
                    If AddStartQueryParamChar Then
                        If BaseUrl <> "" Then
                            StringReturn &= "?"
                        End If
                        AddStartQueryParamChar = False
                    Else
                        StringReturn &= "&"
                    End If

                    StringReturn &= key & "=" & DecodeKeyValue(QueryParameters(key))
                Next
                Return StringReturn
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Shared Function DecodeKeyValue(ByVal Value As String, Optional FlagDecode As Boolean = True) As String
        Dim retValue As String = ""
        Try
            If Value.Length > 0 Then
                For i As Integer = 0 To Value.Length - 1
                    If FlagDecode Then
                        retValue &= DecodeSpecialChar(Value.Chars(i))
                    Else
                        retValue &= Value.Chars(i)
                    End If
                Next i
            Else
                If FlagDecode Then
                    retValue = DecodeSpecialChar(Value)
                Else
                    retValue = Value
                End If
            End If

        Catch ex As Exception

        End Try
        Return retValue
    End Function

    Private Shared Function DecodeSpecialChar(ByVal Character As Char) As String
        Dim retConv As String = ""
        Try
            If InStr(":|!£$%&/=?^ ", Character, CompareMethod.Binary) > 0 Then
                retConv = "%" & CStr(Hex(Int(IIf(Asc(Character) < 10, "0" & Asc(Character), Asc(Character)))))
            Else
                retConv = Character
            End If
            Return retConv
        Catch ex As Exception
            Return retConv
        End Try
    End Function

    Public Shared Function AddQueryStringNoDecode(ByVal BaseUrl As String, ByVal QueryParameters As Dictionary(Of String, String)) As String
        Try
            If QueryParameters Is Nothing Then
                Return BaseUrl
            Else
                Dim StringReturn As String = BaseUrl
                Dim AddStartQueryParamChar As Boolean = True
                For Each key In QueryParameters.Keys
                    If AddStartQueryParamChar Then
                        If BaseUrl <> "" Then
                            StringReturn &= "?"
                        End If
                        AddStartQueryParamChar = False
                    Else
                        StringReturn &= "&"
                    End If

                    StringReturn &= key & "=" & DecodeKeyValue(QueryParameters(key), False)
                Next
                Return StringReturn
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
