Imports System.Globalization
Imports System.Runtime.Serialization


Public Class ElaborazioniConfigurazioneUtente_List

    Public Property ListaElaborazioni As List(Of ElaborazioniConfigurazioneUtente_Elemento)

End Class

Public Class ElaborazioniConfigurazioneUtente_Elemento

    Public Gis_Sat_Sentinel_User_Config_COD As Integer
    Public StatoElaborazione As Integer
    Public DataRiferimentoElaborazione As String
    Public NumeroElaborazioni As Integer

End Class

Public Class Gis_Sat_Sentinel_Overlay_Flat


    'customMapOverlayBase.push({ Tile: "T32TQQ", SensoreElaborazione: "NDVI", DataRiferimento: "21/09/2018", Descrizione: "Sentinel 2 - 21/09/2018 - NDVI  ", url:  urlBase + "S2B_MSIL1C_20180921T101019_N0206_R022_T32TQQ_20180921T173236/NDVI/"});


    Public Property Tile As String

    Public Property SensoreElaborazione As String

    Public Property DataRiferimento As String

    Public ReadOnly Property Descrizione As String
        Get
            Return "Sentinel 2 - " & DataRiferimento & " - " & SensoreElaborazione & " "
        End Get
    End Property

    Public Property url As String

End Class

Public Class Gis_Sat_Sentinel_Overlay_list

    Public Property ListaOverlayer As List(Of Gis_Sat_Sentinel_Overlay)

End Class

Public Class Gis_Sat_Sentinel_Overlay

    Public Property DataRiferimento As String

    Public Property Passaggi As List(Of Gis_Sat_Sentinel_Overlay_Passaggio)


End Class

Public Class Gis_Sat_Sentinel_Overlay_Passaggio

    Public Property Tile As Gis_Sat_Sentinel_Overlay_Tile

    Public Property url As String

    Public Property Sensore As List(Of Gis_Sat_Sentinel_Overlay_Sensore)

End Class

Public Class Gis_Sat_Sentinel_Overlay_Sensore

    Public Property CodiceSensore As String

    Public Property Descrizione As String

    Public Property DatiRilevati As List(Of Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato)

End Class

<DataContract>
Public Class Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato

    <DataMember(Name:="DataRiferimento")>
    Private Property FormattedReturnDate As String

    <IgnoreDataMember>
    Public Property DataRiferimento As DateTime
        Get
            Return DateTime.ParseExact(FormattedReturnDate, "o", CultureInfo.InvariantCulture)
        End Get
        Set(ByVal value As DateTime)
            FormattedReturnDate = value.ToString("o")
        End Set
    End Property

    <DataMember>
    Public Property Valore As Double

End Class


Public Class LetturaProiezioniDatoPoligono_Response

    Public Property GEORiferimento_COD As Integer?
    Public Property GEORiferimento_COD_2 As Integer?

End Class

Public Class RateoPianoVariabileSuPoligono_Response
    Public Property EsriiAscii As String
    Public Property GEORiferimento_COD As String
End Class

Public Class Gis_Sat_Sentinel_Overlay_Tile

    Public Property Tile As String

    Public Property PoligonoWktBoundingBox As String

    Public Property GEORiferimento_COD As String

End Class

