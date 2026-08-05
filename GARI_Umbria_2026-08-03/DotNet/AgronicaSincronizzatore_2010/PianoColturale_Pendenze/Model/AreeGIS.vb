Public Class CalcoloAreeParticelleGIS
    Public Property areaIntersezione As Double
    Public Property areaParticella As Double

    Public Sub New()
        areaIntersezione = 0
        areaParticella = 0
    End Sub
    Public Sub New(_areaIntersezione As Double, _areaParticella As Double)
        areaIntersezione = _areaIntersezione
        areaParticella = _areaParticella
    End Sub
End Class
