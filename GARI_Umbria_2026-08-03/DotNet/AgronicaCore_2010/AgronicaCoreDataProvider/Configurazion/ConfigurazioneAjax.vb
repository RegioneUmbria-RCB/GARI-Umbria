Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ConfigurazioneAjax

    Public AbilitaCompressioneRisposte As Boolean
    Public DimensioneMinimaPerCompressioneInMB As Decimal

    Public Sub New()
        AbilitaCompressioneRisposte = True
        DimensioneMinimaPerCompressioneInMB = 0
    End Sub

End Class
