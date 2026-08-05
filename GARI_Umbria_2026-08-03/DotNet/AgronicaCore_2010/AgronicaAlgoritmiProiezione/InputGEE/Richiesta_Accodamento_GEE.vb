Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModello
Imports Newtonsoft.Json.Linq

Public Class Richiesta_Accodamento_GEE
    Implements InputGEE

    Protected LayerAnalysisConfig_GUID As String
    Protected Layer_1 As ProiezioneLayer
    Protected Layer_2 As ProiezioneLayer
    Protected Layer_Risultato As ProiezioneLayer

    Public Function CostruisciRigaDaAccodare(ByVal Esecuzione_cod As Int32,
                                             ByVal coordObj As String,
                                             ByVal GuidEntita1 As String,
                                             ByVal GuidEntita2 As String,
                                             ByVal GuidEntitaRisultato As String,
                                             ByVal LayerAnalysisConfig_Algorithm_Cod As Int32,
                                             ByVal LayerAnalysisConfig_AlgorithmType_Cod As Int32,
                                             ByVal AttivoTuttoLayer As Boolean,
                                             ByVal Esecuzione_GUID As String,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri,
                                             ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                             ByVal Optional elementoGraficoPerDescrizione As Int32 = 0,
                                             ByVal Optional collectionName As String = "",
                                             ByVal Optional viewNames As List(Of String) = Nothing,
                                             ByVal Optional paletteVisualizzazione As String = "") As JObject

        Dim output As New JObject

        Dim geoJsonPolygon As New JObject

        geoJsonPolygon.Add("type", "FeatureCollection")

        Dim features As New JArray

        Dim feature As New JObject

        feature.Add("type", "Feature")

        Dim geometry As New JObject

        geometry.Add("coordinates", JArray.FromObject(getStringGeometryGeoJson(coordObj).coordinates))
        geometry.Add("type", "Polygon")

        feature.Add("geometry", geometry)

        Dim properties As New JObject

        Dim guidExec As String = Esecuzione_GUID

        If guidExec.Equals("") Then
            guidExec = CreaNuovoGUIDEsecuzione(Esecuzione_cod, objParametri_Server)
        End If

        properties.Add("PivaSuperUser", objParametri_Server.PivaSuperUser)
        properties.Add("idDB", RecuperaIDDB(objParametri_Server, objParametri_Utenti, objParametri_Super_Server))
        properties.Add("GIS_LayerAnalysisConfig_Exec_Log_GUID", guidExec)
        properties.Add("featureGUID", GuidEntita1)
        properties.Add("LayerAnalysisConfig_GUID", LayerAnalysisConfig_GUID)
        properties.Add("LayerAnalysisConfig_Algorithm_Cod", LayerAnalysisConfig_Algorithm_Cod)
        properties.Add("AttivaSuTuttiLayer", AttivoTuttoLayer)

        If elementoGraficoPerDescrizione = 0 Then
            properties.Add("ElementoGrafico_Des", "")
        Else
            properties.Add("ElementoGrafico_Des", LeggiDescrizioneElementoGrafico(elementoGraficoPerDescrizione, objParametri_Server))
        End If

        properties.Add("LayerAnalysisConfig_Params", CreaArrayParametri())

        Dim LeggiCFGCallback As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim Installation As String =
            LeggiCFGCallback.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametri_Server)

        If Installation.EndsWith("/") Then
            Installation = Installation.Substring(0, Installation.Length - 2)
        End If

        Dim CallBackBaseUrl As String =
            LeggiCFGCallback.Leggi_Valore(0, "GiasOnline_Core_API", "", "", objParametri_Server)

        If CallBackBaseUrl.StartsWith("/") Then
            CallBackBaseUrl = CallBackBaseUrl.Substring(1)
        End If

        If CallBackBaseUrl.StartsWith("http") Then
            properties.Add("CallbackURL", String.Format("{0}/{1}", CallBackBaseUrl, CostantiPersonalizzate.CallBack_Piattaforma_GEE))
        Else
            properties.Add("CallbackURL", String.Format("{0}/{1}/{2}", Installation, CallBackBaseUrl, CostantiPersonalizzate.CallBack_Piattaforma_GEE))
        End If

        If LayerAnalysisConfig_Algorithm_Cod = TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Sync Or
            LayerAnalysisConfig_Algorithm_Cod = TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Yearly Then

            Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

            Dim DT As DataTable = xRead.LeggiIntersezioneConLayerRasterDaGUID(GuidEntita1, Layer_2.LayerElementiGrafici_Cod, objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count < 1 Then
                Throw New Exception("Impossibile determinare la intersezione con il layer Raster.")
            End If

            If DT.Select("ParametriVisualizzazioneLayer = ''").Count > 0 Then
                Throw New Exception("Parametri di visualizzazione non impostati per il layer Raster.")
            End If

            Dim parametriVisualizzazioneArray As New List(Of JObject)

            For Each row In DT.Rows
                parametriVisualizzazioneArray.Add(JObject.Parse(System.Text.RegularExpressions.Regex.Unescape(row("ParametriVisualizzazioneLayer").ToString)))
            Next

            'Dim parametriVisualizzazioneLayer = JObject.Parse(System.Text.RegularExpressions.Regex.Unescape(DT.Rows(0)("ParametriVisualizzazioneLayer").ToString))

            DT = xRead.LeggiDescrizioneElementoDaCodiceEntita(CInt(DT.Rows(0)("Entita_Cod")), objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                Throw New Exception("Impossibile leggere la descrizione dell'entita intersecata.")
            End If

            Dim descrizioneElementoGrafico = DT.Rows(0)("ElementoGrafico_Des").ToString

            Dim scale = 30

            If Not descrizioneElementoGrafico.Equals("") Then

                Dim scaleParam = descrizioneElementoGrafico.Split("|").Where(Function(s) s.StartsWith("Pixel size"))

                If scaleParam.Count > 0 Then
                    scale = CInt(scaleParam.FirstOrDefault.Split("§")(1).ToString.Trim)
                End If
            End If

            Dim uriList As New JArray

            If parametriVisualizzazioneArray.Count > 0 Then
                uriList = JArray.FromObject(parametriVisualizzazioneArray.Select(Function(p) p("gsUriFile").ToString).ToList())
            End If

            properties.Add("uri", uriList)
            properties.Add("scale", scale)
        End If

        If Not collectionName.Equals("") Then
            properties.Add("Index", collectionName)
        End If

        If viewNames IsNot Nothing Then
            properties.Add("views", JArray.FromObject(viewNames))
        End If

        If LayerAnalysisConfig_Algorithm_Cod = TipiEnumerativi.enum_AlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE Then

            Dim DT_Validita As DataTable
            Dim xReadEntita As New AgronicaCoreGisBIZ.GIS_Entita_R

            If Layer_1.LayerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI Then
                DT_Validita = xReadEntita.LeggiValiditaImpiantoDaEntitaGUID(GuidEntita1, objParametri_Server)

                If DT_Validita Is Nothing OrElse DT_Validita.Rows.Count <> 1 Then
                    Throw New Exception("Errore nella lettura della validità dell'impianto.")
                End If

            Else
                DT_Validita = xReadEntita.LeggiValiditaEntitaDaEntitaGUID(GuidEntita1, objParametri_Server)

                If DT_Validita Is Nothing OrElse DT_Validita.Rows.Count <> 1 Then
                    Throw New Exception("Errore nella lettura della validità dell'impianto.")
                End If

            End If

            properties.Add("Validita_Inizio", CDate(DT_Validita.Rows(0)("Validita_Inizio")))
            properties.Add("Validita_Fine", CDate(DT_Validita.Rows(0)("Validita_Fine")))

        End If

        If Not paletteVisualizzazione.Equals("") Then
            Dim paletteObj = JObject.Parse(paletteVisualizzazione)

            If Not paletteObj("vizParams").ToString.Equals("") Then
                properties.Add("vizParams", paletteObj("vizParams"))
            End If

            If Not paletteObj("SLDPalette").ToString.Equals("") Then
                properties.Add("SLDPalette", System.Text.RegularExpressions.Regex.Unescape(paletteObj("SLDPalette").ToString))
            End If
        End If

        feature.Add("properties", properties)

        features.Add(feature)

        geoJsonPolygon.Add("features", features)
        output.Add("geoJsonPolygon", geoJsonPolygon)

        Return output

    End Function

    Public Overloads Sub SetupParametriOpzionali() Implements InputGEE.SetupParametriOpzionali

    End Sub
    Public Overloads Sub SetupParametriOpzionali(ByVal LayerAnalysisConfig_GUID As String,
                                                 ByVal Layer_1 As ProiezioneLayer,
                                                 ByVal Layer_2 As ProiezioneLayer,
                                                 ByVal Layer_Risultato As ProiezioneLayer)

        Me.LayerAnalysisConfig_GUID = LayerAnalysisConfig_GUID
        Me.Layer_1 = Layer_1
        Me.Layer_2 = Layer_2
        Me.Layer_Risultato = Layer_Risultato

    End Sub

    Private Function RecuperaIDDB(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim objBiz As New AgronicaCoreProvisioningBIZ.DatiServer_R

        Dim datiServerDT = objBiz.GetDatiServer(AgronicaCoreDTOStd.InData.Provisioning.DatiServerRequest.enum_Scelta_Server.Server,
                                                objParametri,
                                                objParametri_Utenti,
                                                objParametri_Super_Server)

        If Not IsDBNull(datiServerDT.Rows(0)("ID_DB")) Then
            Return datiServerDT.Rows(0)("ID_DB").ToString
        End If

        Throw New Exception("Impossibile recuperare l'ID del DB.")
    End Function

    Private Function CreaNuovoGUIDEsecuzione(ByVal Esecuzione_cod As Int32,
                                             ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim newGuidEsecuzione = Guid.NewGuid().ToString

        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim resp = xWrite.SalvaGUIDEsecuzione(Esecuzione_cod, newGuidEsecuzione, objParametri_Server)

        If Not resp Then
            Throw New Exception("Impossibile salvare il nuovo GUID per l'esecuzione.")
        End If

        Return newGuidEsecuzione

    End Function

    Private Function CreaArrayParametri() As JArray
        Dim output As New JArray

        For Each param In Layer_1.Params
            Dim paramObj = CreaObjParametroLayer(param.TipologiaLayer_struct_GUID, 1,
                                                 Layer_1.LayerElementiGrafici_GUID, Layer_1.TipologiaLayer_cod)

            output.Add(paramObj)
        Next

        For Each param In Layer_2.Params
            Dim paramObj = CreaObjParametroLayer(param.TipologiaLayer_struct_GUID, 2,
                                                 Layer_2.LayerElementiGrafici_GUID, Layer_2.TipologiaLayer_cod)

            output.Add(paramObj)
        Next

        For Each param In Layer_Risultato.Params
            Dim paramObj = CreaObjParametroLayer(param.TipologiaLayer_struct_GUID, 3,
                                                 Layer_Risultato.LayerElementiGrafici_GUID, Layer_Risultato.TipologiaLayer_cod)

            output.Add(paramObj)
        Next

        Return output
    End Function

    Private Function CreaObjParametroLayer(ByVal TipologiaLayer_struct_GUID As String,
                                           ByVal index As Int32,
                                           ByVal LayerElementiGrafici_GUID As String,
                                           ByVal TipologiaLayer_cod As Int32) As JObject

        Dim layerObj As New JObject

        layerObj.Add("LayerAnalysisConfig_GUID", LayerAnalysisConfig_GUID)
        layerObj.Add("LayerXConfig_Cod", index)
        layerObj.Add("LayerElementiGrafici_GUID", LayerElementiGrafici_GUID)
        layerObj.Add("TipologiaLayer_Cod", TipologiaLayer_cod)
        layerObj.Add("TipologiaLayer_struct_GUID", TipologiaLayer_struct_GUID)

        Return layerObj
    End Function

    Private Function getStringGeometryGeoJson(ByVal elemento As String) As AgronicaCoreModelsSTD.Gis.GeoJson_Geometry_New

        Dim s As New SqlSpatialConverter(Of WktParser)(elemento)
        Return s.ToGeoJson

    End Function

    Private Function LeggiDescrizioneElementoGrafico(ByVal entita_cod As Int32,
                                                     ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

        Dim DT = xRead.LeggiDescrizioneElementoDaCodiceEntita(entita_cod, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Impossibile leggere la descrizione dell'elemento grafico.")
        End If

        Return DT.Rows(0)("ElementoGrafico_Des").ToString
    End Function

End Class
