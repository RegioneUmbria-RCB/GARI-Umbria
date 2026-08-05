Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaAlgoritmiProiezione.V2.DTOs

Namespace V2
    Public Class Algoritmo_Factory
        Private config As DTOs.CF_Config

        Public Function GetAlgoritmoDaCodice(ByVal Algoritmo_Cod As Int32,
                                         ByVal Parametri_Addizionali As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As V2.Algoritmi.IAlgoritmoProiezione

            LeggiConfigurazioneAlgoritmi(Algoritmo_Cod, objParametri_Server)

            Select Case Algoritmo_Cod
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Assessment_NoGo_Areas,
                     TipiEnumerativi.enum_AlgoritmoProiezione.Assessment_Risk_Biodiversity,
                     TipiEnumerativi.enum_AlgoritmoProiezione.Intact_Forest_Landscape
                    Return New Algoritmo_Intersezione(Algoritmo_Cod,
                                                         TipiEnumerativi.enum_TipoAlgoritmoProiezione.Intersezione)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Soil_Erosion_by_Water
                    Return New V2.Algoritmi.Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                                  TipiEnumerativi.enum_TipoAlgoritmoProiezione.Proiezione_Vettoriale_su_Raster, config)
                'Case TipiEnumerativi.enum_AlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE
                '    Return New Caricamento_Raster_GEE(Algoritmo_Cod,
                '                  TipiEnumerativi.enum_TipoAlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE, CLng(Parametri_Addizionali("Max_FileSize_In_Byte")), cfg_siti)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE
                    Return New V2.Algoritmi.Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                                  TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE, config)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Sync
                    Return New V2.Algoritmi.Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                                  TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync, config)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Yearly
                    Return New V2.Algoritmi.Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                                  TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly, config)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.EsportazioneBulkLayer
                    Return New EsportazioneBulkLayer(Algoritmo_Cod,
                                                     TipiEnumerativi.enum_TipoAlgoritmoProiezione.EsportazioneBulkLayer)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.CalcoloPianoConcimazione
                    Return New AlgoritmoBaseGEE()

                Case Else
                    Throw New Exception("Algoritmo non riconosciuto.")
            End Select
        End Function

        Private Sub LeggiConfigurazioneAlgoritmi(ByVal algoritmo_Cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri)

            Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim confString As String =
                LeggiConfString.Leggi_Valore(0, "Configurazioni_GoogleEarthEngine", "", "", objParametri_Server)

            Dim confObj = JsonConvert.DeserializeObject(Of V2.DTOs.Config_GEE)(confString)

            Dim jsonAuth = confObj.JSONAuth
            Dim audience As String = ""

            Select Case algoritmo_Cod
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Intact_Forest_Landscape,
                     TipiEnumerativi.enum_AlgoritmoProiezione.Soil_Erosion_by_Water,
                     TipiEnumerativi.enum_AlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE
                    audience = String.Format("{0}/{1}", confObj.BaseUrl_GEE, confObj.SubmitGISAlgorithm)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Sync
                    audience = String.Format("{0}/{1}", confObj.BaseUrl_GEE, confObj.GISAlgorithmOnGCP_GEESyncDispatcher)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE
                    audience = String.Format("{0}/{1}", confObj.BaseUrl_GEE, confObj.GEOTIFFUploadOnGCP)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Yearly
                    audience = String.Format("{0}/{1}", confObj.BaseUrl_GEE, confObj.GISAlgorithmOnGCP_GEESyncDispatcher)
                Case TipiEnumerativi.enum_AlgoritmoProiezione.EsportazioneBulkLayer
                    Return
            End Select

            Me.config = New DTOs.CF_Config With {
                .endpoint = audience,
                .jsonAuth = jsonAuth
                }

        End Sub
    End Class
End Namespace

