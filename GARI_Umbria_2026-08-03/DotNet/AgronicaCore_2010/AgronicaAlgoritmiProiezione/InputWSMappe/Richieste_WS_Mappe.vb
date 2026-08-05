Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports Newtonsoft.Json.Linq

Public Class Richieste_WS_Mappe
    Implements IInput_WS_Mappe

    Protected LayerAnalysisConfig_GUID As String
    Protected Layer_1 As ProiezioneLayer
    Protected Layer_2 As ProiezioneLayer
    Protected Layer_Risultato As ProiezioneLayer

    Public Sub SetupParametriOpzionali(LayerAnalysisConfig_GUID As String, Layer_1 As ProiezioneLayer, Layer_2 As ProiezioneLayer, Layer_Risultato As ProiezioneLayer) Implements IInput_WS_Mappe.SetupParametriOpzionali
        Me.LayerAnalysisConfig_GUID = LayerAnalysisConfig_GUID
        Me.Layer_1 = Layer_1
        Me.Layer_2 = Layer_2
        Me.Layer_Risultato = Layer_Risultato

    End Sub

    Public Function GetRequestInizializzaCalendario(ByVal wkt As String,
                                                    ByVal sensor As String,
                                                    ByVal source As String,
                                                    ByVal dateStart As DateTime,
                                                    ByVal dateEnd As DateTime
                                                    ) As CustomMapOverlayBaseInizializzaCalendario_In
        Return New CustomMapOverlayBaseInizializzaCalendario_In With {
                .wkt = wkt,
                .sensor = sensor,
                .source = source,
                .startDate = dateStart,
                .endDate = dateEnd
            }
    End Function

    Public Function GetRequestRasterCrop(ByVal wkt As String,
                                         ByVal url As String,
                                         ByVal srid As Integer) As RasterCrop_In
        Return New RasterCrop_In With {
                .wkt = wkt,
                .path = url,
                .srid = srid
            }
    End Function

    Public Function GetRequestRasterized(ByVal imageId As Guid,
                                         ByVal streamData As Boolean) As Rasterized_In
        Return New Rasterized_In With {
                .ImageId = imageId,
                .StreamData = streamData
            }
    End Function

End Class
