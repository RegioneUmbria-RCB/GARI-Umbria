

Public Class RisultatoMeteo

    Public Property Meteo_Table() As String
    Public Property Meteo_Charts() As String
    Public Property Meteo_RiepilogoPeriodo() As String
    Public Property Meteo_RiepilogoSensori() As String

End Class



Public Class RisultatoMeteoRiepilogo
    Public Class Stazione
        Public Property Descrizione As String
        Public Property UltimoAggiornamento As String
        Public Property Meteo As RisultatoMeteo
    End Class

    Public Property Stazioni As List(Of Stazione)

    Public Sub New()
        Stazioni = New List(Of Stazione)
    End Sub
End Class



Public Class RisultatoMeteoMonitoraggioSuolo
    Public Class Stazione
        Public Property Descrizione As String
        Public Property SogliaInf As Decimal
        Public Property SogliaSup As Decimal
        Public Property AlertSerie As String
        Public Property Meteo As RisultatoMeteo
    End Class

    Public Property Stazioni As List(Of Stazione)

    Public Sub New()
        Stazioni = New List(Of Stazione)
    End Sub
End Class
