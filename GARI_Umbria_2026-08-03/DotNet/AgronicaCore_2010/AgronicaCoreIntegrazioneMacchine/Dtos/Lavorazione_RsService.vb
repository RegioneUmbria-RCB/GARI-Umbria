Public Class Lavorazione_RSService
    Public IdStore As Integer
    Public DeviceStore As String
    Public DataStore As Date
    Public OraStore As Tempo
    Public ScadenzaStore As Date
    Public CodePLUStore As String
    Public DescrizioneStore As String
    Public CodeTraceabilityStore As String
    Public IdClienteStore As String
    Public LottoStore As String
    Public CodProduzioneStore As String
    Public TaraStore As Double
    Public ScartoStore As Double
    Public PesoScStore As Double
    Public ImportoScStore As Double
    Public PesoInSoglia As Integer
    Public CheckStore As Integer
End Class

Public Class Tempo
    Public Ore As Integer
    Public Minuti As Integer
    Public Secondi As Integer

    Public Sub New(_date As String)
        Dim contents = _date.Split(":")
        Dim h = Integer.Parse(contents(0))
        Dim m = Integer.Parse(contents(1))
        Dim s = Integer.Parse(contents(2))

        If h > 23 OrElse m > 59 OrElse s > 59 Then
            Throw New Exception("Ora specificata è invalida")
        End If
        Ore = h
        Minuti = m
        Secondi = s
    End Sub

    Public Sub New(dt As DateTime)
        Ore = dt.Hour
        Minuti = dt.Minute
        Secondi = dt.Second
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format(
            "{0:00}:{1:00}:{2:00}",
            Me.Ore, Me.Minuti, Me.Secondi)
    End Function

    Public Function ToDatetime() As DateTime
        Return DateTime.Parse(Me.ToString())
    End Function
End Class
