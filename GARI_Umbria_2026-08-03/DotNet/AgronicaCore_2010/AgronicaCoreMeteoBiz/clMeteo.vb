
Public Class MeteoStazione
    Public Class MeteoConfigurazione
        Public Property Colonna As String
        Public Property Descrizione As String
        Public Property Simbolo As String
        Public Property AggFun As String
        Public Property Colore As String
        Public Property Grafico As String
        Public Property Gruppo As Integer
        Public Property Ordine As Integer
    End Class
    Public Class MeteoDato
        Public Property DataOra As String
        Public Property Values As List(Of String)
        Public Sub New()
        End Sub
        Public Sub New(ByVal dt As DateTime)
            DataOra = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dt, DateTimeKind.Local)
            Values = New List(Of String)
        End Sub
    End Class
    Public Property Stazione As String
    Public Property Configurazione As List(Of MeteoConfigurazione)
    Public Property Meteo As List(Of MeteoDato)
    Public Property BaseUnit As String
    Public Sub New()
        Stazione = ""
        Configurazione = New List(Of MeteoConfigurazione)
        Meteo = New List(Of MeteoDato)
        BaseUnit = "minutes"
    End Sub
End Class


Public Class MeteoRiepilogo

    Public Property Stazioni As List(Of MeteoStazione)

    Public Sub New()
        Stazioni = New List(Of MeteoStazione)
    End Sub
End Class

