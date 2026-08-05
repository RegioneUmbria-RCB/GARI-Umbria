Public Class VB6_Proxy


    Public shared Function Valore(Dato As object) As String
        Dim Stringa As String
        Dim Posiz As Long
        Dim Numero As String
    
            Stringa = CStr(Dato)
            Posiz = InStr(Stringa, ",")
            If Posiz > 0 Then
                If Posiz = 1 Then
                    Numero = "." & Right(Stringa, Len(Stringa) - 1)
                ElseIf Posiz = Len(Stringa) Then
                    Numero = Left(Stringa, Len(Stringa) - 1)
                Else
                    Numero = Left(Stringa, Posiz - 1) & "." & Right(Stringa, Len(Stringa) - Posiz)
                End If
            Else
                Numero = Stringa
            End If
            Valore = Numero
    
End Function

End Class
