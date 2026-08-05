Public Class xyz

    Private _x As Double
    Public Property X() As Double
        Get
            Return _x
        End Get
        Set(value As Double)
            _x = Value
        End Set
    End Property
    Private _y As Double
    Public Property Y() As Double
        Get

            Return _y
        End Get
        Set(value As Double)
            _y = Value
        End Set
    End Property
    Private _z As Double
    Public Property Z() As Double
        Get
            Return _z
        End Get
        Set(value As Double)
            _z = Value
        End Set
    End Property

    Public Shared Function myCDBL(ByVal sDBL As String) As Double
        Dim i As Integer = 0
        Dim toReplace As String

        If sDBL.Contains("E") Then

            Dim s1 As String() = sDBL.Split("E")
            Dim s2 As String() = s1(0).Split(".")
            Dim iDecToAdd As Integer = CInt(s1(1).Replace("-", ""))
            If s2.Length > 1 Then
                iDecToAdd += s2(1).Length
            End If
            sDBL = CDbl(sDBL.Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)).ToString("F" & iDecToAdd).Replace(",", ".")
        End If

        While IsNumeric(sDBL(i)) OrElse sDBL(i) = "-"
            If i = sDBL.Length - 1 Then
                Exit While
            End If
            i += 1
        End While
        toReplace = sDBL(i)
        If IsNumeric(toReplace) Then
            Return Math.Round(CDbl(sDBL), 7)       'lavez - 18/04/2023 - limito le coordinate a 10 decimali
        Else
            Return Math.Round(CDbl(sDBL.Replace(toReplace, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator)), 7)    'lavez - 18/04/2023 - limito le coordinate a 10 decimali

        End If
    End Function
End Class
