Imports System.IO
Imports Newtonsoft.Json.Linq
Imports OSGeo.GDAL

Namespace Agronica.Helpers.GDALHelper

    ''' <summary>
    ''' Coordinate di bounding box WGS84 (EPSG:4326) di un raster.
    ''' Utilizzabile direttamente come LatLngBounds per un GroundOverlay su Google Maps.
    ''' </summary>
    Public Class RasterBounds
        ''' <summary>Latitudine massima (bordo nord).</summary>
        Public Property North As Double
        ''' <summary>Latitudine minima (bordo sud).</summary>
        Public Property South As Double
        ''' <summary>Longitudine massima (bordo est).</summary>
        Public Property East As Double
        ''' <summary>Longitudine minima (bordo ovest).</summary>
        Public Property West As Double
    End Class

    ''' <summary>
    ''' Dati per il rendering di un GroundOverlay su Google Maps SDK.
    ''' Contiene l'immagine PNG codificata in Base64 e il bounding box WGS84.
    ''' </summary>
    Public Class MappaPrescrizioneGroundOverlay_Out
        ''' <summary>Immagine PNG della mappa di prescrizione codificata in Base64.</summary>
        Public Property PngBase64 As String
        ''' <summary>Coordinate del bounding box WGS84 per il GroundOverlay (north/south/east/west).</summary>
        Public Property Bounds As RasterBounds
    End Class

    ''' <summary>
    ''' Singola fermata cromatica in una scala di colori.
    ''' Position deve essere compreso tra 0.0 (valore minimo del raster) e 1.0 (valore massimo).
    ''' </summary>
    Public Class ColorStop
        Public Property Position As Double
        Public Property R As Byte
        Public Property G As Byte
        Public Property B As Byte
        Public Property A As Byte = 255
    End Class

    ''' <summary>
    ''' Scala di colori per la visualizzazione di un raster.
    ''' Supporta due modalità:
    ''' - <see cref="CustomMapper"/>: funzione algoritmica continua (colore unico per ogni valore di t).
    ''' - <see cref="Stops"/>: interpolazione lineare tra fermate discrete (fallback).
    ''' </summary>
    Public Class ColorRamp

        ''' <summary>
        ''' Funzione algoritmica opzionale. Se impostata, viene usata al posto delle fermate.
        ''' Riceve t ∈ [0,1] e restituisce il colore RGBA corrispondente.
        ''' </summary>
        Public Property CustomMapper As Func(Of Double, (R As Byte, G As Byte, B As Byte, A As Byte))

        Public Property Stops As List(Of ColorStop) = New List(Of ColorStop)()

        ''' <summary>
        ''' Restituisce il colore RGBA per un valore normalizzato t ∈ [0, 1].
        ''' Se è definito un <see cref="CustomMapper"/>, viene usato quello (gradiente continuo);
        ''' altrimenti si interpola linearmente tra le <see cref="Stops"/> definite.
        ''' </summary>
        Public Function Interpolate(ByVal t As Double) As (R As Byte, G As Byte, B As Byte, A As Byte)
            If Double.IsNaN(t) OrElse Double.IsInfinity(t) Then Return (0, 0, 0, 0)
            t = Math.Max(0.0, Math.Min(1.0, t))

            If CustomMapper IsNot Nothing Then
                Return CustomMapper(t)
            End If

            If Stops.Count = 0 Then Return (0, 0, 0, 255)

            Dim sorted = Stops.OrderBy(Function(s) s.Position).ToList()

            If t <= sorted(0).Position Then Return (sorted(0).R, sorted(0).G, sorted(0).B, sorted(0).A)
            Dim last = sorted(sorted.Count - 1)
            If t >= last.Position Then Return (last.R, last.G, last.B, last.A)

            For i As Integer = 0 To sorted.Count - 2
                Dim s0 = sorted(i)
                Dim s1 = sorted(i + 1)
                If t >= s0.Position AndAlso t <= s1.Position Then
                    Dim span As Double = s1.Position - s0.Position
                    Dim f As Double = If(span > 0, (t - s0.Position) / span, 0.0)
                    Return (
                        CByte(Math.Round(s0.R + f * (s1.R - s0.R))),
                        CByte(Math.Round(s0.G + f * (s1.G - s0.G))),
                        CByte(Math.Round(s0.B + f * (s1.B - s0.B))),
                        CByte(Math.Round(s0.A + f * (s1.A - s0.A)))
                    )
                End If
            Next

            Return (last.R, last.G, last.B, last.A)
        End Function

        ''' <summary>
        ''' Converte un colore HSV in RGBA (alpha = 255).
        ''' h: 0–360 gradi, s: 0–1, v: 0–1.
        ''' </summary>
        Private Shared Function HsvToRgba(ByVal h As Double, ByVal s As Double, ByVal v As Double) As (R As Byte, G As Byte, B As Byte, A As Byte)
            Dim c As Double = v * s
            Dim x As Double = c * (1.0 - Math.Abs(h / 60.0 Mod 2.0 - 1.0))
            Dim m As Double = v - c
            Dim r1, g1, b1 As Double
            If h < 60 Then
                r1 = c : g1 = x : b1 = 0
            ElseIf h < 120 Then
                r1 = x : g1 = c : b1 = 0
            ElseIf h < 180 Then
                r1 = 0 : g1 = c : b1 = x
            ElseIf h < 240 Then
                r1 = 0 : g1 = x : b1 = c
            ElseIf h < 300 Then
                r1 = x : g1 = 0 : b1 = c
            Else
                r1 = c : g1 = 0 : b1 = x
            End If
            Return (CByte(Math.Min(255, Math.Max(0, Math.Round((r1 + m) * 255)))),
                    CByte(Math.Min(255, Math.Max(0, Math.Round((g1 + m) * 255)))),
                    CByte(Math.Min(255, Math.Max(0, Math.Round((b1 + m) * 255)))),
                    255)
        End Function

        ''' <summary>
        ''' Gradiente continuo: Verde (valore basso) → Giallo → Rosso (valore alto).
        ''' Usa la conversione HSV→RGB (hue 120°→0°): ogni valore di t produce un colore univoco.
        ''' </summary>
        Public Shared ReadOnly Property GreenToRed As ColorRamp
            Get
                Return New ColorRamp() With {
                    .CustomMapper = Function(t) HsvToRgba((1.0 - t) * 120.0, 1.0, 0.85)
                }
            End Get
        End Property

        ''' <summary>
        ''' Gradiente continuo: Rosso (valore basso) → Giallo → Verde (valore alto).
        ''' Usa la conversione HSV→RGB (hue 0°→120°): ogni valore di t produce un colore univoco.
        ''' </summary>
        Public Shared ReadOnly Property RedToGreen As ColorRamp
            Get
                Return New ColorRamp() With {
                    .CustomMapper = Function(t) HsvToRgba(t * 120.0, 1.0, 0.85)
                }
            End Get
        End Property

        ''' <summary>Blu → Ciano → Verde → Giallo → Rosso (stile Jet colormap), gradiente continuo.</summary>
        Public Shared ReadOnly Property Jet As ColorRamp
            Get
                Return New ColorRamp() With {
                    .CustomMapper = Function(t) HsvToRgba((1.0 - t) * 240.0, 1.0, 0.9)
                }
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Utility per la conversione e interrogazione di file raster GIS tramite GDAL NuGet.
    ''' </summary>
    Public Class GisRasterUtility

        Shared Sub New()
            GdalConfiguration.ConfigureGdal()
            GdalConfiguration.ConfigureOgr()
        End Sub

        ''' <summary>
        ''' Converte un file raster (.tif) in PNG e restituisce il contenuto in Base64.
        ''' Quando viene fornita una <paramref name="colorRamp"/>, i valori del raster vengono
        ''' normalizzati sul min/max reale e mappati sulla scala cromatica indicata; il PNG
        ''' risultante viene scritto su disco come "<i>nomefile</i>_colored.png".
        ''' Senza color ramp il raster viene semplicemente riproiettato in EPSG:4326 con canale
        ''' alpha e scritto come "<i>nomefile</i>.png".
        ''' Il file PNG su disco funge da cache: se già presente viene restituito senza rielaborare.
        ''' </summary>
        ''' <param name="percorsoFile">Percorso assoluto del file raster di input.</param>
        ''' <param name="colorRamp">Scala cromatica opzionale. Se Nothing viene prodotto un PNG in scala di grigi con alpha.</param>
        ''' <returns>Stringa Base64 dell'immagine PNG.</returns>
        Public Shared Function RasterToPngBase64(ByVal percorsoFile As String, Optional ByVal colorRamp As ColorRamp = Nothing) As String
            If Not File.Exists(percorsoFile) Then
                Throw New FileNotFoundException("File raster non trovato: " & percorsoFile)
            End If

            Dim percorsoPng As String
            If colorRamp IsNot Nothing Then
                percorsoPng = Path.Combine(
                    Path.GetDirectoryName(percorsoFile),
                    Path.GetFileNameWithoutExtension(percorsoFile) & "_colored.png")
            Else
                percorsoPng = Path.ChangeExtension(percorsoFile, ".png")
            End If

            If Not File.Exists(percorsoPng) Then
                Dim inputDs As Dataset = Gdal.Open(percorsoFile, Access.GA_ReadOnly)
                If inputDs Is Nothing Then
                    Throw New Exception("Impossibile aprire il file raster: " & percorsoFile)
                End If

                Try
                    If colorRamp IsNot Nothing Then
                        GenerateColoredPng(inputDs, percorsoPng, colorRamp)
                    Else
                        Dim warpOptions As New GDALWarpAppOptions({"-t_srs", "EPSG:4326", "-of", "PNG", "-dstalpha"})
                        Dim outputDs As Dataset = Gdal.Warp(percorsoPng, {inputDs}, warpOptions, Nothing, Nothing)
                        If outputDs Is Nothing Then
                            Throw New Exception("La conversione gdalwarp ha restituito un dataset nullo.")
                        End If
                        outputDs.FlushCache()
                        outputDs.Dispose()
                    End If
                Finally
                    inputDs.Dispose()
                End Try

                If Not File.Exists(percorsoPng) Then
                    Throw New FileNotFoundException("File PNG non generato: " & percorsoPng)
                End If
            End If

            Return Convert.ToBase64String(File.ReadAllBytes(percorsoPng))
        End Function

        ''' <summary>
        ''' Riproietta il raster in EPSG:4326, applica la scala cromatica normalizzando i valori
        ''' sul min/max reale dei pixel validi e scrive il risultato come PNG RGBA.
        ''' </summary>
        Private Shared Sub GenerateColoredPng(ByVal sourceDs As Dataset, ByVal percorsoPng As String, ByVal colorRamp As ColorRamp)
            ' 1. Warp to EPSG:4326 preserving float values (into vsimem)
            Dim vsimemPath As String = "/vsimem/colored_warp_" & Guid.NewGuid().ToString("N") & ".tif"
            Dim warpedDs As Dataset = Nothing
            Try
                Dim warpOptions As New GDALWarpAppOptions({"-t_srs", "EPSG:4326"})
                warpedDs = Gdal.Warp(vsimemPath, {sourceDs}, warpOptions, Nothing, Nothing)
                If warpedDs Is Nothing Then
                    Throw New Exception("Warp EPSG:4326 in memoria fallito.")
                End If

                Dim xSize As Integer = warpedDs.RasterXSize
                Dim ySize As Integer = warpedDs.RasterYSize
                Dim band As Band = warpedDs.GetRasterBand(1)

                ' 2. Read float pixel buffer
                Dim pixelData As Single() = New Single(xSize * ySize - 1) {}
                band.ReadRaster(0, 0, xSize, ySize, pixelData, xSize, ySize, 0, 0)

                ' 3. Get nodata
                Dim noDataVal As Double = Double.NaN
                Dim hasNoData As Integer = 0
                band.GetNoDataValue(noDataVal, hasNoData)

                ' 4. Compute min/max from valid pixels
                Dim minVal As Single = Single.MaxValue
                Dim maxVal As Single = Single.MinValue
                For Each v As Single In pixelData
                    If hasNoData = 0 OrElse Math.Abs(CDbl(v) - noDataVal) > 0.0001 Then
                        If v < minVal Then minVal = v
                        If v > maxVal Then maxVal = v
                    End If
                Next
                Dim range As Double = If(maxVal > minVal, CDbl(maxVal - minVal), 1.0)

                ' 5. Map each pixel to RGBA using the color ramp
                Dim rBuf As Byte() = New Byte(xSize * ySize - 1) {}
                Dim gBuf As Byte() = New Byte(xSize * ySize - 1) {}
                Dim bBuf As Byte() = New Byte(xSize * ySize - 1) {}
                Dim aBuf As Byte() = New Byte(xSize * ySize - 1) {}

                For i As Integer = 0 To pixelData.Length - 1
                    If Single.IsNaN(pixelData(i)) OrElse Single.IsInfinity(pixelData(i)) OrElse
                       (hasNoData = 1 AndAlso Math.Abs(CDbl(pixelData(i)) - noDataVal) < 0.0001) Then
                        ' nodata / invalid → fully transparent
                        rBuf(i) = 0 : gBuf(i) = 0 : bBuf(i) = 0 : aBuf(i) = 0
                    Else
                        Dim t As Double = (CDbl(pixelData(i)) - CDbl(minVal)) / range
                        Dim color = colorRamp.Interpolate(t)
                        rBuf(i) = color.R : gBuf(i) = color.G : bBuf(i) = color.B : aBuf(i) = color.A
                    End If
                Next

                ' 6. Create 4-band RGBA MEM dataset
                Dim memDs As Dataset = Gdal.GetDriverByName("MEM").Create("", xSize, ySize, 4, DataType.GDT_Byte, Nothing)
                Dim gt As Double() = New Double(5) {}
                warpedDs.GetGeoTransform(gt)
                memDs.SetGeoTransform(gt)
                memDs.SetProjection(warpedDs.GetProjection())

                memDs.GetRasterBand(1).WriteRaster(0, 0, xSize, ySize, rBuf, xSize, ySize, 0, 0)
                memDs.GetRasterBand(2).WriteRaster(0, 0, xSize, ySize, gBuf, xSize, ySize, 0, 0)
                memDs.GetRasterBand(3).WriteRaster(0, 0, xSize, ySize, bBuf, xSize, ySize, 0, 0)
                memDs.GetRasterBand(4).WriteRaster(0, 0, xSize, ySize, aBuf, xSize, ySize, 0, 0)

                memDs.GetRasterBand(1).SetColorInterpretation(ColorInterp.GCI_RedBand)
                memDs.GetRasterBand(2).SetColorInterpretation(ColorInterp.GCI_GreenBand)
                memDs.GetRasterBand(3).SetColorInterpretation(ColorInterp.GCI_BlueBand)
                memDs.GetRasterBand(4).SetColorInterpretation(ColorInterp.GCI_AlphaBand)

                ' 7. Write PNG
                Dim pngDs As Dataset = Gdal.GetDriverByName("PNG").CreateCopy(percorsoPng, memDs, 0, Nothing, Nothing, Nothing)
                If pngDs Is Nothing Then
                    Throw New Exception("CreateCopy PNG ha restituito un dataset nullo.")
                End If
                pngDs.FlushCache()
                pngDs.Dispose()
                memDs.FlushCache()
                memDs.Dispose()
            Finally
                If warpedDs IsNot Nothing Then warpedDs.Dispose()
                Gdal.Unlink(vsimemPath)
            End Try
        End Sub

        ''' <summary>
        ''' Estrae le coordinate di bounding box WGS84 (EPSG:4326) da un file raster.
        ''' Il risultato è direttamente utilizzabile per un GroundOverlay su Google Maps
        ''' (LatLngBounds con north/south/east/west).
        ''' </summary>
        ''' <param name="percorsoFile">Percorso assoluto del file raster (es. .tif o .png).</param>
        ''' <returns>Oggetto <see cref="RasterBounds"/> con i quattro bordi in gradi decimali WGS84.</returns>
        ''' <exception cref="FileNotFoundException">Se il file non esiste.</exception>
        ''' <exception cref="Exception">Se il dataset non ha un CRS definito o il JSON non è valido.</exception>
        Public Shared Function GetRasterBounds(ByVal percorsoFile As String) As RasterBounds
            If Not File.Exists(percorsoFile) Then
                Throw New FileNotFoundException("File raster non trovato: " & percorsoFile)
            End If

            Dim ds As Dataset = Gdal.Open(percorsoFile, Access.GA_ReadOnly)
            If ds Is Nothing Then
                Throw New Exception("Impossibile aprire il file raster: " & percorsoFile)
            End If

            Try
                Dim infoJson As String = Gdal.GDALInfo(ds, New GDALInfoOptions({"-json"}))

                Dim json As JObject
                Try
                    json = JObject.Parse(infoJson)
                Catch ex As Exception
                    Throw New Exception("Impossibile analizzare l'output JSON di gdalinfo: " & ex.Message)
                End Try

                ' wgs84Extent is always in WGS84 (lon/lat) regardless of the raster CRS.
                ' GeoJSON polygon ring order: [west,south], [east,south], [east,north], [west,north], [west,south]
                Dim wgs84Extent = TryCast(json("wgs84Extent"), JObject)
                If wgs84Extent Is Nothing Then
                    Throw New Exception("Il file raster non ha un sistema di riferimento definito (wgs84Extent mancante).")
                End If

                Dim coords = TryCast(wgs84Extent("coordinates")(0), JArray)
                If coords Is Nothing OrElse coords.Count < 4 Then
                    Throw New Exception("Formato inatteso di 'wgs84Extent.coordinates'.")
                End If

                Dim lons = coords.Select(Function(pt) CDbl(pt(0))).ToArray()
                Dim lats = coords.Select(Function(pt) CDbl(pt(1))).ToArray()

                Return New RasterBounds() With {
                    .West = lons.Min(),
                    .East = lons.Max(),
                    .South = lats.Min(),
                    .North = lats.Max()
                }
            Finally
                ds.Dispose()
            End Try
        End Function
    End Class

End Namespace
