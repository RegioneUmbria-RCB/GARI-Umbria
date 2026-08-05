
Public Class ParametriMeteoDSS : Implements IEquatable(Of ParametriMeteoDSS)

    Public Sorgente As Integer
    Public Stazione As Integer
    Public DataInizio As DateTime
    Public DataFine As DateTime
    Public Forecast As Boolean
    Public SogliaBagnaturaPerc As Decimal
    Public SogliaBagnaturaMin As Decimal
    Public SogliaDistanza_mt As Integer

    Public Sub New()
        SogliaBagnaturaPerc = 15
        SogliaBagnaturaMin = 0
        SogliaDistanza_mt = 7500
    End Sub

    Public Overrides Function Equals(other As Object) As Boolean
        If other.GetType Is GetType(ParametriMeteoDSS) Then
            Return _equals(CType(other, ParametriMeteoDSS))
        End If
        Return False
    End Function

    Public Function _equals(other As ParametriMeteoDSS) As Boolean Implements IEquatable(Of ParametriMeteoDSS).Equals
        If Sorgente <> other.Sorgente Then
            Return False
        End If
        If Stazione <> other.Stazione Then
            Return False
        End If
        If Math.Abs(DateDiff(DateInterval.Hour, DataInizio, other.DataInizio)) > 0 Then
            Return False
        End If
        If Math.Abs(DateDiff(DateInterval.Hour, DataFine, other.DataFine)) > 0 Then
            Return False
        End If
        If Forecast <> other.Forecast Then
            Return False
        End If
        If Math.Abs(SogliaBagnaturaPerc - other.SogliaBagnaturaPerc) > 0.1 Then
            Return False
        End If
        If Math.Abs(SogliaBagnaturaMin - other.SogliaBagnaturaMin) > 0.1 Then
            Return False
        End If
        If SogliaDistanza_mt <> other.SogliaDistanza_mt Then
            Return False
        End If
        Return True
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return 0
    End Function
End Class



