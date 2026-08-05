Imports AgronicaCoreDTOStd.InData.Gis

Public Interface IInput_WS_Mappe
    Sub SetupParametriOpzionali(ByVal LayerAnalysisConfig_GUID As String,
                                ByVal Layer_1 As ProiezioneLayer,
                                ByVal Layer_2 As ProiezioneLayer,
                                ByVal Layer_Risultato As ProiezioneLayer)
End Interface
