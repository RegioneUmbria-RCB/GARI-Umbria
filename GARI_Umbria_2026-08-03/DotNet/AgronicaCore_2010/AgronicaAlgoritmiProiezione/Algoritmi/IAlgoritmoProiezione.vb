Imports AgronicaCoreDataProvider

Public Interface IAlgoritmoProiezione

    Function Esegui(ByVal LayerAnalysisConfig_Cod As Int32,
                    ByVal Entita_cod_1 As Int32,
                    ByVal Entita_cod_2 As Int32,
                    ByVal Entita_cod_Risultato As Int32,
                    ByVal Esecuzione_cod As Int32,
                    ByVal Esecuzione_GUID As String,
                    ByVal ParametriEsecuzione As String,
                    ByRef objParametri_Server As AgronicaCoreParametri,
                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                    ByRef objParametri_Super_Server As AgronicaCoreParametri,
                    ByVal Optional override_transazione As Boolean = False
                    ) As Boolean

    Sub LeggiLayerDaConfigurazioneAlgoritmo(ByVal LayerAnalysisConfig_Cod As Int32, ByRef objParametri As AgronicaCoreParametri)

    Sub LeggiLayerDaParametriEsecuzione(ByVal ParametriEsecuzione As String)

End Interface
