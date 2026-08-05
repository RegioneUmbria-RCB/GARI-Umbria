Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class DatiSensori_R
    Public Function LetturaVegIndexes_GEE(ByVal layerElementiGrafici_Cod As Int32,
                                          ByVal poligonoWKT As String,
                                          ByVal zoom As Int32,
                                          ByVal sensore As String,
                                          ByVal dataInizio As String,
                                          ByVal dataFine As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim resp As New RispostaStandard

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim DT As DataTable = xRead.LeggiElementiIntersecanti(layerElementiGrafici_Cod, poligonoWKT, objParametri_Server)

        If DT Is Nothing OrElse DT.Select("Entita_GUID <> ''").FirstOrDefault Is Nothing Then

            resp.RispostaOK = False
            resp.RispostaStringa = "BAD_REQUEST - Nessun area con indici vegetativi per il punto selezionato"
            Return resp

        End If

        Dim row = DT.Select("Entita_GUID <> ''").FirstOrDefault

        Dim GEEInput = creaInputPerGEE(row("Entita_GUID").ToString, poligonoWKT, zoom, sensore, dataInizio, dataFine)

        Dim xWrite As New AgronicaCoreDatiSensoriDAL.DatiSensori_W

        Dim risultatoElaborazioneGEE = xWrite.LetturaVegIndexes(GEEInput.ToString, objParametri_Server)

        resp.RispostaOK = True
        resp.RispostaStringa = risultatoElaborazioneGEE.Replace(sensore, "Valore")

        Return resp
    End Function

    Private Function creaInputPerGEE(ByVal Entita_GUID As String,
                                     ByVal poligonoWKT As String,
                                     ByVal zoom As Int32,
                                     ByVal sensore As String,
                                     ByVal dataInizio As String,
                                     ByVal dataFine As String) As JObject

        Dim GEEInput As New JObject

        Dim geoJsonPolygon As New JObject

        geoJsonPolygon.Add("type", "FeatureCollection")

        Dim features As New JArray

        Dim feature As New JObject

        feature.Add("type", "Feature")

        Dim geometry As New JObject
        geometry.Add("type", "Point")

        Dim coordinates As New JArray
        coordinates.Add(Double.Parse(poligonoWKT.Replace("POINT(", "").Split(" ")(0).Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)))
        coordinates.Add(Double.Parse(poligonoWKT.Replace("POINT(", "").Split(" ")(1).Replace(")", "").Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)))

        geometry.Add("coordinates", coordinates)

        feature.Add("geometry", geometry)

        Dim properties As New JObject

        properties.Add("name", "")
        properties.Add("featureGUID", Entita_GUID)
        properties.Add("platform", CostantiPersonalizzate.Dati_Sensori_Platform)
        properties.Add("StartDate", Date.Parse(dataInizio.Substring(0, 10)).ToString("yyyy-MM-dd"))
        properties.Add("EndDate", Date.Parse(dataFine.Substring(0, 10)).ToString("yyyy-MM-dd"))
        properties.Add("Index", sensore)
        properties.Add("zoom", zoom)

        feature.Add("properties", properties)

        features.Add(feature)

        geoJsonPolygon.Add("features", features)
        GEEInput.Add("geoJsonPolygon", geoJsonPolygon)

        Return GEEInput
    End Function

    ''' <summary>
    ''' Wrapper che recupera dati elaborati da SAT usando gli stessi parametri di LetturaVegIndexes_GEE.
    ''' Calcola l'id del poligono basandosi sul layerElementiGrafici_Cod e legge il tenant dalla configurazione.
    ''' </summary>
    Public Function LetturaVegIndexes_SAT(ByVal layerElementiGrafici_Cod As Int32,
                                         ByVal poligonoWKT As String,
                                         ByVal zoom As Int32,
                                         ByVal sensore As String,
                                         ByVal dataInizio As String,
                                         ByVal dataFine As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_SuperServer As AgronicaCoreParametri) As RispostaStandard

        Dim resp As New RispostaStandard

        ' individua il poligono associato al punto WKT
        Dim xReadGeo As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
        Dim DT As DataTable = xReadGeo.LeggiElementiIntersecanti(layerElementiGrafici_Cod, poligonoWKT, objParametri_Server)

        If DT Is Nothing OrElse DT.Select("Entita_GUID <> ''").FirstOrDefault Is Nothing Then
            resp.RispostaOK = False
            resp.RispostaStringa = "BAD_REQUEST - Nessun area con indici vegetativi per il punto selezionato"
            Return resp
        End If

        Dim row = DT.Select("Entita_GUID <> ''").FirstOrDefault
        Dim poligonoId = row("Entita_GUID").ToString()

        ' recupera tenant dalla configurazione
        Dim cfgRead As New Configurazione_Siti_BIZ_R
        Dim tenantId = cfgRead.leggiSatTenantName(objParametri_Server)
        If tenantId Is Nothing Then
            resp.RispostaOK = False
            resp.RispostaStringa = "Errore nel recupero del tenant SAT dalla configurazione"
            Return resp
        End If

        ' chiama la logica esistente per SAT
        Return RecuperaDatiElaboratiSAT(poligonoId, tenantId, sensore, dataInizio, dataFine, objParametri_Server, objParametri_SuperServer)
    End Function

    ''' <summary>
    ''' Recupera dati elaborati (indici spettrali) da SAT per un poligono.
    ''' Riferimento specifiche: RecuperoDatiElaboratiSAT - Scopo e Descrizione.
    ''' </summary>
    Public Function RecuperaDatiElaboratiSAT(ByVal poligonoId As String,
                                             ByVal tenantId As String,
                                             ByVal indiceRichiesto As String,
                                             ByVal dataInizio As String,
                                             ByVal dataFine As String,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_SuperServer As AgronicaCoreParametri) As RispostaStandard

        Dim resp As New RispostaStandard

        Try
            Dim xWrite As New AgronicaCoreDatiSensoriDAL.DatiSensori_W

            Dim risultatoSAT = xWrite.RecuperaDatiElaboratiSAT(
                poligonoId,
                tenantId,
                indiceRichiesto,
                dataInizio,
                dataFine,
                objParametri_Server,
                objParametri_SuperServer)

            resp.RispostaOK = True
            resp.RispostaStringa = risultatoSAT

            Return resp

        Catch ex As Exception
            resp.RispostaOK = False
            resp.RispostaStringa = String.Format("Errore nel recupero dati elaborati SAT: {0}", ex.Message)
            Return resp
        End Try
    End Function
End Class
Public Class DatiSensori_W

End Class
