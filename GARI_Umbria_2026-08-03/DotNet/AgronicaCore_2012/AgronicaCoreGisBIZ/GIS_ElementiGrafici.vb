Imports <xmlns="http://www.agronica.it/grafica/">
Imports AgronicaGIS2012.Commons
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports Agronica.Helpers.GDALHelper.Agronica.Helpers.GDALHelper

Public Class GIS_ElementiGrafici_R

    Public Function LeggiDatiGML(
                ByVal PivaSuperUser As String,
                ByVal ElementoGrafico_Cod As Int32,
                ByVal Entita_Cod As Int32,
                ByVal LayerElementiGrafici_Cod As Int32,
                ByVal FormatoGraficoConvertito As AgronicaCoreDataProvider.AgronicaCoreParametri.enumFromatoCartograficoConvertito,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As String


        Dim rval As String = ""

        Dim letturaPoligoni As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        rval = letturaPoligoni.LeggiGML(
                PivaSuperUser,
                ElementoGrafico_Cod,
                Entita_Cod,
                LayerElementiGrafici_Cod,
                FormatoGraficoConvertito,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                xFiltroAggiuntivo,
                xOrderBy,
                objParametri
            )

        Return rval

    End Function

    Public Function LeggiDatiMinimiDaCodiceEntita(
            ByVal Entita_Cod As Int32,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable


        Dim rval As String = ""

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim DT As DataTable = xRead.LeggiElementoGraficoDaCodiceEntita(Entita_Cod, objParametri)

        Return DT

    End Function

    Public Function LeggiDescrizioneElementoDaCodiceEntita(
        ByVal Entita_Cod As Int32,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim rval As String = ""

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim DT As DataTable = xRead.LeggiDescrizioneElementoDaCodiceEntita(Entita_Cod, objParametri)

        Return DT

    End Function

    Public Function calcolaIntersezioniTraLayer(ByVal layerElementiGrafici_Cod As Int32,
                                                ByVal TipologiaLayer_struct_cod As Int32,
                                                ByVal geoData As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable
        Dim rval As String = ""

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim DT As DataTable = xRead.calcolaIntersezioniTraLayer(layerElementiGrafici_Cod, TipologiaLayer_struct_cod, geoData, objParametri)

        Return DT
    End Function

    ''' <summary>
    ''' This method calculates the projection of <paramref name="layerElementiGraficiCod1"/> over <paramref name="layerElementiGraficiCod2"/>.
    ''' Layers could be filtered by Company by passing <paramref name="pivaLayer1"/> for <paramref name="layerElementiGraficiCod1"/>
    ''' and <paramref name="pivaLayer2"/> for <paramref name="layerElementiGraficiCod2"/>.
    ''' <paramref name="numPolyLabel"/> Represents the name to assign to the number of polygons columns,
    ''' <paramref name="intersectionAreaLabel"/> Represents the name to assigna to the intersection surface column, expressed in [Ha]
    ''' </summary>
    ''' <param name="layerElementiGraficiCod1"></param>
    ''' <param name="layerElementiGraficiCod2"></param>
    ''' <param name="pivaLayer1"></param>
    ''' <param name="pivaLayer2"></param>
    ''' <param name="numPolyLabel"></param>
    ''' <param name="intersectionAreaLabel"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>
    ''' A <c>DataTable</c> containing the surface of <paramref name="layerElementiGraficiCod1"/> overlapping <paramref name="layerElementiGraficiCod2"/>,
    ''' the number of <paramref name="layerElementiGraficiCod1"/> instances intersecting each <paramref name="layerElementiGraficiCod2"/> instance,
    ''' and the number of <paramref name="layerElementiGraficiCod1"/> distinct Companies wich instances intersect each <paramref name="layerElementiGraficiCod2"/> instance
    ''' </returns>
    Public Function CalculateIntersectionsOnLayers(ByVal layerElementiGraficiCod1 As Int32,
                                                   ByVal layerElementiGraficiCod2 As Int32,
                                                   ByVal numPolyLabel As String,
                                                   ByVal intersectionAreaLabel As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   Optional ByVal pivaLayer1 As String = "",
                                                   Optional ByVal pivaLayer2 As String = "",
                                                   Optional ByVal validityYearLayer1 As Integer = Nothing,
                                                   Optional ByVal validityYearLayer2 As Integer = Nothing) As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim DT As DataTable = xRead.CalculateIntersectionsOnLayers(
            layerElementiGraficiCod1,
            layerElementiGraficiCod2,
            pivaLayer1,
            pivaLayer2,
            numPolyLabel,
            intersectionAreaLabel,
            validityYearLayer1,
            validityYearLayer2,
            objParametri
            )

        Return DT

    End Function

    Public Function LeggiIntersezioneConLayerRasterDaGUID(ByVal guidEntita1 As String,
                                                          ByVal layerElementiGrafici_Cod As Integer,
                                                          ByRef objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim DT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        DT = xRead.LeggiIntersezioneConLayerRasterDaGUID(guidEntita1, layerElementiGrafici_Cod, objParametri_Server)

        Return DT
    End Function

    Public Function LeggiInfoRaster(ByVal layerElementiGrafici_Cod As Int32,
                                    ByVal poligonoWKT As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri) As List(Of RasterInfoClick_Out)

        Dim resp As New List(Of RasterInfoClick_Out)

        Dim LeggiCFGDatiIniziali As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim conf = LeggiCFGDatiIniziali.Leggi_Valore(0, "pathFileRaster", "", "", objParametri_Server)
        Dim basePath = LeggiCFGDatiIniziali.Leggi_Valore(0, "GestioneAllegati_Repository", "", "", objParametri_Server)

        Dim configurazioniRaster = JObject.Parse(conf)

        Dim pathCompleto = Path.Combine(basePath, configurazioniRaster("BasePathAllegatiRaster").ToString)

        Dim DT As DataTable = xRead.LeggiInfoRaster(layerElementiGrafici_Cod, poligonoWKT, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return resp
        End If

        For Each row In DT.Rows
            Dim pathFolderRaster = Path.Combine(pathCompleto, row("Entita_GUID").ToString)
            Dim pathFileRaster = Path.Combine(pathFolderRaster, row("Allegati_Documenti_NomeFile").ToString)

            Dim ElementoGrafico_DesParts = row("ElementoGrafico_Des").ToString.Split("|")

            Dim GDALHelper As New InfoRaster()
            Dim values As List(Of Double) = GDALHelper.GetRasterInfo(pathFileRaster, poligonoWKT)

            If values.Count = 0 Then
                Throw New Exception("Impossibile determinare le info del punto desiderato sul raster selezionato.")
            End If

            Dim descrizioniBande() As String = New String() {""}

            If ElementoGrafico_DesParts.Where(Function(s) s.StartsWith("Bands Descriptions§")).Count > 0 Then
                descrizioniBande = ElementoGrafico_DesParts.Where(Function(s) s.StartsWith("Bands Descriptions§")).First.Split("§")(1).Split(",")
            End If

            While descrizioniBande.Count < values.Count
                descrizioniBande.Append("")
            End While

            Dim bandIndex = 0

            For Each value In values

                Dim ValoreStringa = ""

                If Double.IsNaN(value) Then
                    ValoreStringa = "Nessun Valore."
                End If

                Dim rasterInfo As New RasterInfoClick_Out With {
                    .BandN = bandIndex + 1,
                    .Descrizione = descrizioniBande(bandIndex),
                    .Valore = value,
                    .ValoreStringa = ValoreStringa
                }

                resp.Add(rasterInfo)

                bandIndex += 1
            Next

        Next

        Return resp
    End Function

    Public Function getEnvelopeWKT(layerId As Integer,
                                   objParametri As AgronicaCoreParametri) As String

        Dim wkt As String
        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim dt = xRead.getEnvelopeWKT(layerId, objParametri)
        wkt = dt.Rows(0)("EnvelopeWKT")

        Return wkt
    End Function

    'Public Function LeggiCoordinateOperazioneAgendaXYZ(
    '                        ByVal Piva As Int32,
    '                        ByVal id_Agenda As Int32,
    '                        ByVal xFiltroAggiuntivo As String,
    '                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of xyz)

    '    Dim letturaPoligoni As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
    '    Dim DTGeo As DataTable = letturaPoligoni.LeggiCoordinateOperazioneAgenda(Piva, id_Agenda, xFiltroAggiuntivo, objParametri)
    '    Dim strGeo As String = ""

    '    If Not IsNothing(DTGeo) AndAlso DTGeo.Rows.Count > 0 AndAlso Not String.IsNullOrEmpty(DTGeo.Rows(0).Item("geo")) Then
    '        strGeo = DTGeo.Rows(0).Item("geo")
    '    End If

    '    Dim wktHelp As New WKT
    '    Dim DatiCartograficiOriginali_WGS84 As List(Of xyz) = wktHelp.CreaCoordinateDaWkt(strGeo)

    '    Return DatiCartograficiOriginali_WGS84

    'End Function

    'Public Function LeggiCoordinateOperazioneAgendaText(
    '                    ByVal Piva As Int32,
    '                    ByVal id_Agenda As Int32,
    '                    ByVal xFiltroAggiuntivo As String,
    '                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

    '    Dim DatiCartograficiOriginali_WGS84 As List(Of xyz) = LeggiCoordinateOperazioneAgendaXYZ(Piva, id_Agenda, xFiltroAggiuntivo, objParametri)
    '    Dim strRes As String = DatiCartograficiOriginali_WGS84(0).Y.ToString().Replace(".", ",") & "," & DatiCartograficiOriginali_WGS84(0).X.ToString().Replace(".", ",")

    '    Return strRes

    'End Function

    Public Function LeggiWKTConGUID(ByVal Entita_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim DT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        DT = xRead.LeggiWKTConGUID(Entita_Cod, xFiltroAggiuntivo, objParametri_Server)

        Return DT

    End Function


End Class



Public Class GIS_ElementiGrafici_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function VerificaPoligono(ByVal PoligonoGML As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim TestPoligono As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim tResult As Boolean

        tResult = TestPoligono.TestaPoligono(PoligonoGML)
        If Not tResult Then

            '2 tentativo con il poligono invertito
            Dim oldPolygon As String = PoligonoGML
            PoligonoGML = PolygonOrder.InvertiPoligono(PoligonoGML, True)
            tResult = TestPoligono.TestaPoligono(PoligonoGML, objParametri)

        End If

        Return tResult
    End Function


    Public Function VerificaPoligonoRestituisciGmlRuotato(ByRef PoligonoGML As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim TestPoligono As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim tResult As Boolean

        tResult = TestPoligono.TestaPoligono(PoligonoGML)
        If Not tResult Then

            '2 tentativo con il poligono invertito
            Dim oldPolygon As String = PoligonoGML
            PoligonoGML = PolygonOrder.InvertiPoligono(PoligonoGML, True)
            tResult = TestPoligono.TestaPoligono(PoligonoGML, objParametri)

        End If

        Return tResult
    End Function

    Public Function VerificaPoligonoRestituisciWKTRuotato(ByRef PoligonoWKT As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim TestPoligono As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim tResult As Boolean

        tResult = TestPoligono.TestaPoligonoWKT(PoligonoWKT)
        If Not tResult Then

            '2 tentativo con il poligono invertito
            PoligonoWKT = PolygonOrder.InvertiPoligono_wkt(PoligonoWKT, True)
            tResult = TestPoligono.TestaPoligonoWKT(PoligonoWKT, objParametri)

        End If

        Return tResult
    End Function

    ''' <summary>
    ''' scrive UN SINGOLO elemento grafico restituendone il codice
    ''' </summary>
    ''' <param name="DatiGrafici"></param>
    ''' <param name="OUTPUT_ElementoGraficoCod"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks></remarks>
    Public Function Scrivi(
            ByRef DatiGrafici As String,
            ByRef OUTPUT_ElementoGraficoCod As Integer,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As Boolean

        Const NomeRoutine As String = "GIS_ElementiGrafici_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Dim ScriviElementiGrafici As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W


        Dim tmpDoc As XDocument = XDocument.Parse(DatiGrafici)
        Dim gml As XNamespace = "http://www.opengis.net/gml"


        Dim elemento =
            (From a In tmpDoc.<Entita>
             Select a).FirstOrDefault

        Dim pivaSuperUser As String
        Dim Validita_Inizo As DateTime = #1/1/1900#
        Dim validita_Fine As DateTime = #12/31/2100#


        'per test formato database ...
        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            pivaSuperUser = elemento.<EntitaGIAS>.<DatoGias>.<PivaSuperUser>.FirstOrDefault.Value

            Dim TipoOperazioneDB As Integer = elemento.@TipoOperazioneDB


            Dim layer1 As Integer
            If elemento.<layers>.ToList.Count > 0 Then
                Dim xe = (From elem In elemento.<layers>
                          Where elem.<layer>.@tipologia_layer = 1
                          Select elem.<layer>).FirstOrDefault
                If xe Is Nothing OrElse xe.Value = "" Then
                    layer1 = 1
                Else
                    layer1 = xe.Value
                End If
            End If
            'If layer1 Is Nothing Then
            '    layer1 = 1
            'End If
            Dim polygonOriginal As XElement = TryCast(elemento.LastNode, XElement)
            Dim polygon As XElement = Nothing

            If polygonOriginal IsNot Nothing Then
                polygon = New XElement(polygonOriginal)
                Dim attrsDaRimuovere = polygon.Attributes().
                           Where(Function(a) Not a.IsNamespaceDeclaration).
                           ToList()

                For Each attr In attrsDaRimuovere
                    attr.Remove()
                Next

            End If

            Select Case TipoOperazioneDB
                Case 1

                    Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    OUTPUT_ElementoGraficoCod = AgroSequenze.NuovoId_Tabella("GIS_ElementiGrafici", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
                    'OUTPUT_ElementoGraficoCod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
                    '            "GIS_ElementiGrafici",
                    '            objParametri
                    '        )

                    Dim EntitaCod As Integer = elemento.<EntitaGIAS>.<DatoGias>.<Entita_Cod>.FirstOrDefault.Value

                    xRisp = ScriviElementiGrafici.Scrivi(
                        pivaSuperUser,
                        OUTPUT_ElementoGraficoCod,
                        elemento.@text,
                        EntitaCod,
                        layer1,
                        polygon.ToString,' This is already the gml data
                        elemento.Descendants.FirstOrDefault(Function(x) x.Attributes.Any(Function(a) a.Name = "Flag_GPS")).@Flag_GPS,
                        elemento.<EntitaGIAS>.<DatoGias>.<OLDGrafica_ID>.FirstOrDefault.Value,
                        elemento.<EntitaGIAS>.<DatoGias>.<Piva>.FirstOrDefault.Value,
                        elemento.<EntitaGIAS>.<DatoGias>.<Sa_Cod>.FirstOrDefault.Value,
                        Validita_Inizo,
                        validita_Fine,
                        objParametri
                    )

                Case 2
                    Dim EntitaCod As Integer = elemento.<EntitaGIAS>.<DatoGias>.<Entita_Cod>.FirstOrDefault.Value
                    xRisp = ScriviElementiGrafici.Modifica(
                        pivaSuperUser,
                        elemento.Descendants.FirstOrDefault(Function(x) x.Attributes.Any(Function(a) a.Name = "ElementoGrafico_Cod")).@ElementoGrafico_Cod,
                        elemento.@text,
                        EntitaCod,
                        layer1,
                        polygon.ToString,' This is already the gml data
                        elemento.Descendants.FirstOrDefault(Function(x) x.Attributes.Any(Function(a) a.Name = "Flag_GPS")).@Flag_GPS,
                        elemento.<EntitaGIAS>.<DatoGias>.<OLDGrafica_ID>.FirstOrDefault.Value,
                        elemento.<EntitaGIAS>.<DatoGias>.<Piva>.FirstOrDefault.Value,
                        elemento.<EntitaGIAS>.<DatoGias>.<Sa_Cod>.FirstOrDefault.Value,
                        Validita_Inizo,
                        validita_Fine,
                        "",
                        objParametri
                    )

                Case 3
                    xRisp = ScriviElementiGrafici.Cancella(
                    pivaSuperUser,
                    elemento.Descendants.FirstOrDefault(Function(x) x.Attributes.Any(Function(a) a.Name = "ElementoGrafico_Cod")).@ElementoGrafico_Cod,
                    "",
                    objParametri
                    )

            End Select


            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(ElementoGrafico_cod=" & OUTPUT_ElementoGraficoCod & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

    Public Function scriviElementoGraficoBaseDaWKT(ByVal newIDElementoGrafico As Integer,
                                                   ByVal elementoGrafico_Des As String,
                                                   ByVal newIDEntita As Integer,
                                                   ByVal layerElementiGrafici_Cod As Integer,
                                                   ByVal geoData As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   ByVal Optional Inizio_Validita As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                                   ByVal Optional Fine_Validita As Date = CostantiPersonalizzate.AGRODATAFINE
                                                   ) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

        resp = xWrite.ScriviElementoGraficoBaseDaWKT(newIDElementoGrafico,
                                                     elementoGrafico_Des,
                                                     newIDEntita,
                                                     layerElementiGrafici_Cod,
                                                     geoData,
                                                     Inizio_Validita,
                                                     Fine_Validita,
                                                     objParametri)

        If Not resp Then
            Throw New Exception("Errore nel salvataggio dell'entità grafica.")
        End If

        Return resp
    End Function

    ''' <summary>
    ''' Updates the description of the graphic element identified by <paramref name="elementoGraficoCod"/> and <paramref name="entitaCod"/> into the database
    ''' </summary>
    ''' <param name="entitaCod"></param>
    ''' <param name="elementoGraficoCod"></param>
    ''' <param name="newDescription"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function UpdateGraphicElementDescription(entitaCod As Integer,
                                                    elementoGraficoCod As Integer,
                                                    newDescription As String,
                                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean
        Dim xWrite As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

        resp = xWrite.UpdateGraphicElementDescription(entitaCod, elementoGraficoCod, newDescription, objParametri)

        If Not resp Then
            Throw New Exception("Errore nel salvataggio dell'entità grafica.")
        End If

        Return resp
    End Function
End Class