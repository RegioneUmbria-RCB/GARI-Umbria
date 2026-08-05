Imports System.Globalization

Public Class CustomMapOverlayBaseInizializzaCalendario_In
    Public Property wkt As String
    Public Property sensor As String
    Public Property source As String
    Public Property startDate As DateTime
    Public Property endDate As DateTime
End Class

Public Class CustomMapOverlayBaseInizializzaCalendario_Out
    Public Property stringResponse As List(Of GisSatSentinelOverlayModel)
End Class

Public Class GisSatSentinelOverlayModel
    Public Property dataRiferimento As String
    Public Property passaggi As List(Of GisSatSentinelOverlayPassageModel)
End Class

Public Class GisSatSentinelOverlayPassageModel
    Public Property tile As GisSatSentinelOverlayTileModel
    Public Property url As String
    Public Property sensore As List(Of GisSatSentinelOverlaySensoreModel)
End Class

Public Class GisSatSentinelOverlayTileModel
    Public Property tile As String
    Public Property poligonoWktBoundingBox As String
    Public Property GEORiferimento_COD As String
End Class

Public Class GisSatSentinelOverlaySensoreModel
    Public Property codiceSensore As String
    Public Property descrizione As String
    Public Property DatiRilevati As List(Of GisSatSentinelOverlaySensoreDatoRilevato)
End Class

Public Class GisSatSentinelOverlaySensoreDatoRilevato
    Public Property FormattedReturnDate As String
    Public Property DataRiferimento As DateTime
        Get
            Return DateTime.ParseExact(FormattedReturnDate, "o", CultureInfo.InvariantCulture)
        End Get
        Set(value As DateTime)
            FormattedReturnDate = value.ToString("o")
        End Set
    End Property
End Class