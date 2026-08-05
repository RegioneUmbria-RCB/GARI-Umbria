Public Class VecchioFormatoHelper

    ''' <summary>
    ''' Dati i layer in formato "I,A" restituisce "F,E,I,A"
    ''' </summary>
    ''' <param name="prefisso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AccodaPrefissoAccessorio(ByVal prefisso As String) As String
        Dim tmp As String() = prefisso.Split(",")
        Dim apici As String = ""
        If prefisso.StartsWith("'") Then
            apici = "'"
        End If
        Dim rval As String = ""
        For Each s As String In tmp
            rval &= apici & lDammiPrefissoAccessorio(s.Replace("'", "")) & apici & ","
        Next

        Return rval & prefisso

    End Function
    Public Shared Function lDammiPrefissoAccessorio(ByVal prefisso As String) As String
        Select Case prefisso
            Case "A"
                Return "E"
            Case "D"
                Return "Q"
            Case "L"
                Return "B"
            Case "T"
                Return "T"
            Case "O"
                Return "P"
            Case "W"
                Return "K"
            Case "X"
                Return "Y"
            Case "M"
                Return "N"
            Case "I"
                Return "F"
            Case Else
                Return "Z"

                'A(E)
                'D(Q)
                'T(T)
                'L(B)
                'O(P)
                'W(K)
                'X(Y)                
                'M(N)
                'I(F)
        End Select

    End Function
End Class
