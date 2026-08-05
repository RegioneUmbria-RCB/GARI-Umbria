Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports System.Net.Http
Imports AgronicaControlliGIS
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.Gis

Public Class IndiciDiRischio_R

End Class
Public Class IndiciDiRischio_W
    Public Function ElaboraRischioConWSMappe(ByVal objPfIndiciRischio As pfIndiciRischio_In,
                                             ByVal codice_Fiscale_Tecnico As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                       ByRef objParametri_Super_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim xEnt_R As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        If Debugger.IsAttached Then
            ServicePointManager.ServerCertificateValidationCallback = Function(s, c, h, e) True
        End If

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            'richiamo ws_mappe_2024
            '0- richiamo calendario alla data per recuperare id del geotiff da utilizzare per la mappa

            Dim DT As DataTable

            DT = xEnt_R.leggiABDaDatiImpianto(objPfIndiciRischio.ChiaveAlbero.Piva,
                                                   objPfIndiciRischio.ChiaveAlbero.Sa_Cod,
                                                   objPfIndiciRischio.ChiaveAlbero.Appezza,
                                                   objPfIndiciRischio.ChiaveAlbero.Id_Imp,
                                                   objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count = 0 Then
                Throw New Exception("Errore nel recupero delle informazioni del poligono selezionato")
            End If

            Dim baseRow = DT.Select(String.Format("LayerElementiGrafici_Cod = {0}", CInt(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI))).FirstOrDefault
            Dim ARow = DT.Select("ElementoGrafico_Des = 'A'").FirstOrDefault
            Dim BRow = DT.Select("ElementoGrafico_Des = 'B'").FirstOrDefault

            If baseRow Is Nothing Then
                Throw New Exception("Nessun impianto trovato.")
            End If

            Dim DTEntita = xEnt_R.Leggi(objParametri_Server.PivaSuperUser,
                                             0,
                                             0,
                                             objPfIndiciRischio.ChiaveAlbero.Piva,
                                             objPfIndiciRischio.ChiaveAlbero.Sa_Cod,
                                             objPfIndiciRischio.ChiaveAlbero.Appezza,
                                             0,
                                             objPfIndiciRischio.ChiaveAlbero.Id_Imp,
                                             "",
                                             "",
                                             "-1",
                                             -1,
                                             -1,
                                             "-1",
                                             0,
                                             0,
                                             0,
                                             0,
                                             codice_Fiscale_Tecnico,
                                             "",
                                             Nothing,
                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             String.Format(" Entita.TipoEntita_Cod in (select tipoEntita_cod from gis_tipoentita where layerElementiGrafici_cod = {0} )", baseRow("LayerElementiGrafici_Cod").ToString),
                                             "",
                                             objParametri_Server,
                                             objParametri_Utenti)

            If DTEntita Is Nothing OrElse DTEntita.Rows.Count = 0 Then
                Throw New Exception("IMPOSSBILE PROCEDERE: Non esiste la cartografia per l'impianto associato alla ricetta.")
            End If



            Dim req As New JObject

            req = createRequestGetImageTiffPerWSMappe2024(baseRow,
                                                          "",
                                                          "5",
                                                          objPfIndiciRischio.dataRiferimento)

            Dim resImg = GetResultImageTiffPerWSMappe2024(req,
                                                          "/Maps/CustomMapOverlayBaseInizializzaCalendario",
                                                          objParametri_Server)

            '1- richiamo generazione griglia che ritorna il geojson da salvare negli allegati

            req = New JObject

            req = createRequestGetMapWSMappe2024(baseRow,
                                                 resImg.Last().Passaggi.Last().Url,
                                                 objPfIndiciRischio.cellSize,
                                                 4326)

            getResultGridMap(req,
                            "/GdalData/ProduceGrid",
                            objPfIndiciRischio,
                            objParametri_Server,
                            objParametri_Utenti)


            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

            resp = True

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp
    End Function

    Private Function createRequestGetImageTiffPerWSMappe2024(ByVal baseRow As DataRow,
                                                            ByVal sensor As String,
                                                            ByVal source As String,
                                                            ByVal [date] As DateTime
                                                           ) As JObject
        Dim output As New JObject

        output.Add("wkt", baseRow("geoData").ToString)
        output.Add("sensor", sensor)
        output.Add("source", source)
        output.Add("startDate", ([date].AddMonths(-6)).ToString("yyyy-MM-ddTHH:mm:ssZ"))
        output.Add("endDate", [date].ToString("yyyy-MM-ddTHH:mm:ssZ"))

        Return output

    End Function

    Private Function GetResultImageTiffPerWSMappe2024(ByVal input As JObject,
                                                      ByVal endpoint As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri) As List(Of GisSatSentinelOverlayModel)

        Dim ret As List(Of GisSatSentinelOverlayModel) = Nothing

        Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim confObj = JsonConvert.DeserializeObject(Of WS_Mappe_Config)(LeggiConfString.Leggi_Valore(0, CostantiPersonalizzate.WS_Mappe_ConfKey, "", "", objParametri_Server))

        Dim cf_client As New HttpClient()
        cf_client.DefaultRequestHeaders.Add(confObj.auth_Key.type, confObj.auth_Key.value)

        Dim content As New StringContent(input.ToString, Text.Encoding.UTF8, "application/json")

        Dim hr As HttpResponseMessage = cf_client.PostAsync(confObj.baseUrl + endpoint, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception(String.Format("Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ", hr.StatusCode.ToString, hr.ReasonPhrase))
        End If
        cf_client.Dispose()

        Return JsonConvert.DeserializeObject(Of rispostaStandard(Of List(Of GisSatSentinelOverlayModel)))(hr.Content.ReadAsStringAsync().GetAwaiter().GetResult()).RispostaStringa
    End Function

    Private Function createRequestGetMapWSMappe2024(ByVal baseRow As DataRow,
                                                             ByVal pathImg As String,
                                                             ByVal squareSide As Integer,
                                                             ByVal srid As Integer
                                                            ) As JObject
        Dim output As New JObject

        output.Add("wkt", baseRow("geoData").ToString)
        output.Add("path", pathImg)
        output.Add("squareSide", squareSide.ToString())
        output.Add("srid", srid)

        Return output

    End Function

    Private Function getResultGridMap(ByVal input As JObject,
                                              ByVal endpoint As String,
                                              ByVal objPfIndiciRischio As pfIndiciRischio_In,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = True
        Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim confObj = JsonConvert.DeserializeObject(Of WS_Mappe_Config)(LeggiConfString.Leggi_Valore(0, CostantiPersonalizzate.WS_Mappe_ConfKey, "", "", objParametri_Server))

        Dim cf_client As New HttpClient()
        cf_client.DefaultRequestHeaders.Add(confObj.auth_Key.type, confObj.auth_Key.value)

        Dim content As New StringContent(input.ToString, Text.Encoding.UTF8, "application/json")

        Dim hr As HttpResponseMessage = cf_client.PostAsync(confObj.baseUrl + endpoint, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception(String.Format("Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ", hr.StatusCode.ToString, hr.ReasonPhrase))
        End If
        Dim resp As New RispostaStandard
        Dim res = JsonConvert.DeserializeObject(Of GisDataReadRvalModel(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))(hr.Content.ReadAsStringAsync().GetAwaiter().GetResult())
        cf_client.Dispose()

        If res.Messages.Equals("") Then

            Dim layerDesc = CType(objPfIndiciRischio.tipoIndice, enum_TipoMappeIndiciRischio).ToString() + " - " + CDate(objPfIndiciRischio.dataRiferimento).ToString()

            '0 - creare nuovo layer custom con descrizione tipo e data (assegnare anche i permessi)
            Dim xLayer As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

            Dim chk = xLayer.LeggiLayer(0, layerDesc, "", objParametri_Server, objParametri_Utenti)
            If chk.LayerCod <= 0 Then
                Dim newLayer = xLayer.ScriviNuovoLayerPersonalizzato(New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.ScriviNuovoLayerPersonalizzatoInData() With {
                                                                        .NomeLayer = layerDesc,
                                                                        .MostraDescrizioneAssociata = "1",
                                                                        .FeatureTypeId = FeatureType.Polygon
                                                                    }, objParametri_Server)
                chk.LayerCod = newLayer.RispostaStringa.LayerElementiGrafici_Cod
                chk.LayerDescr = layerDesc
            End If

            Dim xEnt_W As New AgronicaCoreGisDAL.GIS_Entita_W
            Dim xEleGraf_W As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

            For Each feat In res.MyGeoJson.geoJsonCaricato.features

                Dim ent = V_M.SalvaEntitaConAttributi(New SalvaEntitaConAttributi_In() With {
                                                .EntitaCod = 0,
                                                .ElementoGraficoDes = feat.properties.Testo,
                                                .FlagGps = 0,
                                                .LayerElementiGraficiCod = chk.LayerCod,
                                                .Piva = objPfIndiciRischio.ChiaveAlbero.Piva,
                                                .SaCod = objPfIndiciRischio.ChiaveAlbero.Sa_Cod,
                                                .AnalisiCampioneCod = Nothing,
                                                .Cartografia = New SqlSpatialConverter(Of GeoJsonParser)(feat.geometry).ToWKTString()
                                            },
                                            objParametri_Server)
                If ent.RispostaOK = False Then
                    Throw New Exception(ent.Errore)
                End If
            Next

        End If


        Return ret

    End Function
End Class
