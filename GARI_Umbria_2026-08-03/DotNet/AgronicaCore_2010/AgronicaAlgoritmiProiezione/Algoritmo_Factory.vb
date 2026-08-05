Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq

Public Class Algoritmo_Factory

    Private cfg_siti As JObject

    Public Function GetAlgoritmoDaCodice(ByVal Algoritmo_Cod As Int32,
                                         ByVal Parametri_Addizionali As JObject,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As IAlgoritmoProiezione

        LeggiConfigurazioneAlgoritmi(Algoritmo_Cod, objParametri_Server)
        Dim isSatActiveConf = IsSatActive(objParametri_Server)

        Select Case Algoritmo_Cod
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Assessment_NoGo_Areas,
                 TipiEnumerativi.enum_AlgoritmoProiezione.Assessment_Risk_Biodiversity,
                 TipiEnumerativi.enum_AlgoritmoProiezione.Intact_Forest_Landscape
                Return New Algoritmo_Intersezione(Algoritmo_Cod,
                                                     TipiEnumerativi.enum_TipoAlgoritmoProiezione.Intersezione)
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Soil_Erosion_by_Water
                Return New Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                              TipiEnumerativi.enum_TipoAlgoritmoProiezione.Proiezione_Vettoriale_su_Raster, cfg_siti)

            Case TipiEnumerativi.enum_AlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE
                Return New Caricamento_Raster_GEE(Algoritmo_Cod,
                              TipiEnumerativi.enum_TipoAlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE, CLng(Parametri_Addizionali("Max_FileSize_In_Byte")), cfg_siti)

            Case TipiEnumerativi.enum_AlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE
                If isSatActiveConf Then
                    Return New CaricamentoPiattaformaSat(TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE)
                Else
                    Return New Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                                                         TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE, cfg_siti)
                End If

            Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Sync
                Return New Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                              TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync, cfg_siti)

            Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Yearly
                Return New Caricamento_Piattaforma_GEE(Algoritmo_Cod,
                              TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly, cfg_siti)

            Case TipiEnumerativi.enum_AlgoritmoProiezione.EsportazioneBulkLayer
                Return New EsportazioneBulkLayer(Algoritmo_Cod,
                                                 TipiEnumerativi.enum_TipoAlgoritmoProiezione.EsportazioneBulkLayer)

            Case TipiEnumerativi.enum_AlgoritmoProiezione.ProjectionLayerOverLayer
                Return New ProjectionLayerOverLayerAlgorithm(
                    Algoritmo_Cod,
                    TipiEnumerativi.enum_TipoAlgoritmoProiezione.ProjectionOnLayers
                    )
            Case TipiEnumerativi.enum_AlgoritmoProiezione.ProjectionLayerOverRasterGDAL
                Return New RasterOverlayGDAL_Algorithm(
                    Algoritmo_Cod,
                    TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly
                    )

            Case TipiEnumerativi.enum_AlgoritmoProiezione.Caricamento_Catasto
                Return New Caricamento_Catasto(
                    Algoritmo_Cod,
                    TipiEnumerativi.enum_TipoAlgoritmoProiezione.Caricamento_Catasto)

            Case TipiEnumerativi.enum_AlgoritmoProiezione.CalcoloPianoConcimazione
                Return New AlgoritmoBaseGEE()

            Case TipiEnumerativi.enum_AlgoritmoProiezione.Richiamo_WS_Mappe
                Return New Caricamento_WS_Mappe(Algoritmo_Cod,
                                                TipiEnumerativi.enum_TipoAlgoritmoProiezione.Richiamo_WS_Mappe,
                                                objParametri_Server)
            Case Else
                Throw New Exception(String.Format("Algoritmo non riconosciuto. Codice {0}", Algoritmo_Cod))
        End Select
    End Function

    Private Sub LeggiConfigurazioneAlgoritmi(ByVal algoritmo_Cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri)

        Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim confString As String =
            LeggiConfString.Leggi_Valore(0, "Configurazioni_GoogleEarthEngine", "", "", objParametri_Server)

        Dim confObj = JObject.Parse(confString)

        Dim jsonAuth = confObj("JSONAuth").ToString
        Dim audienceBaseUrl = confObj("BaseUrl_GEE").ToString
        Dim audience As String = ""

        Select Case algoritmo_Cod
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Intact_Forest_Landscape,
                 TipiEnumerativi.enum_AlgoritmoProiezione.Soil_Erosion_by_Water,
                 TipiEnumerativi.enum_AlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE
                audience = String.Format("{0}/{1}", audienceBaseUrl, confObj("SubmitGISAlgorithm").ToString)
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Sync
                audience = String.Format("{0}/{1}", audienceBaseUrl, confObj("GISAlgorithmOnGCP_GEESyncDispatcher").ToString)
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE
                audience = String.Format("{0}/{1}", audienceBaseUrl, confObj("GEOTIFFUploadOnGCP").ToString)
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Yearly
                audience = String.Format("{0}/{1}", audienceBaseUrl, confObj("GISAlgorithmOnGCP_GEESyncDispatcher").ToString)
            Case TipiEnumerativi.enum_AlgoritmoProiezione.EsportazioneBulkLayer
                Return
        End Select

        Me.cfg_siti = New JObject

        Me.cfg_siti.Add("jsonAuth", jsonAuth)
        Me.cfg_siti.Add("endpoint", audience)

    End Sub
    
    Private Function IsSatActive(ByRef objParametriServer As AgronicaCoreParametri) As Boolean
        Dim leggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim isActive As String = leggiConfString.Leggi_Valore(0, CostantiPersonalizzate.SAT_IsActive_ConfKey, "", "", objParametriServer)
        Return isActive.ToLower() = "true"
    End Function
End Class
