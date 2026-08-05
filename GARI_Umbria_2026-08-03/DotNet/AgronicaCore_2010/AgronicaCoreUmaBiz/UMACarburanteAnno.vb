Public Class UMACarburanteAnno
    Private carburante_Richiesto As Double
    Public Property CarburanteRichiesto() As Double
        Get
            Return carburante_Richiesto
        End Get
        Set(ByVal value As Double)
            carburante_Richiesto = value
        End Set
    End Property

    Private _carburantePerTipo As New Dictionary(Of Integer, Double)
    Public Property CarburantePerTipo() As Dictionary(Of Integer, Double)
        Get
            Return _carburantePerTipo
        End Get
        Set(ByVal value As Dictionary(Of Integer, Double))
            _carburantePerTipo = value
        End Set
    End Property

End Class
