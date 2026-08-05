Imports System.IO
Imports Agronica.Helpers.GDALHelper.Agronica.Helpers.GDALHelper
Imports OSGeo.GDAL
Imports OSGeo.OGR
Imports OSGeo.OSR

Public Class RasterOverlay
    Public Sub New()
        GdalConfiguration.ConfigureGdal()
        GdalConfiguration.ConfigureOgr()
    End Sub

    Public Function getFrequencyHistogram(ByVal geoTiffPath As String,
                                          ByVal wktPolygon As String,
                                          ByVal srid As Integer) As Dictionary(Of Single, Integer)
        Dim frequencyHistogram As Dictionary(Of Single, Integer) = Nothing
        If geoTiffPath = "" Then
            Throw New Exception("Path GeoTiff mandatory")
        End If

        If Not File.Exists(geoTiffPath) Then
            Throw New Exception("GeoTiff file not found")
        End If

        If wktPolygon = "" Then
            Throw New Exception("WKT Polygon mandatory")
        End If

        If srid <= 0 Then
            Throw New Exception("SRID mandatory")
        End If

        Dim rasterDS As Dataset = Gdal.Open(geoTiffPath, Access.GA_ReadOnly)
        If rasterDS Is Nothing Then
            Throw New Exception("Fail to open GeoTiff File.")
        End If

        If rasterDS.RasterCount() <= 0 Then
            Throw New Exception("Fail to read GeoTiff File.")
        End If

        Dim spatialRef = New SpatialReference("")
        spatialRef.ImportFromEPSG(srid)

        'If rasterDS.GetSpatialRef().Equals(spatialRef) = False Then
        '    Throw New Exception("Projection of Raster it's not the same of srid")
        'End If

        frequencyHistogram = New Dictionary(Of Single, Integer)

        Dim band = rasterDS.GetRasterBand(1)
        Dim xSize = band.XSize
        Dim ySize = band.YSize

        Dim memDS As DataSource = Ogr.GetDriverByName("Memory").CreateDataSource("tempData", Nothing)
        Dim layer = memDS.CreateLayer("polygonLayer", spatialRef, wkbGeometryType.wkbPolygon, Nothing)
        Dim featureDefn = layer.GetLayerDefn
        Dim feature = New Feature(featureDefn)
        feature.SetGeometry(Geometry.CreateFromWkt(wktPolygon))
        layer.CreateFeature(feature)

        Dim maskDataset As Dataset = Gdal.GetDriverByName("MEM").Create("", xSize, ySize, 1, DataType.GDT_Byte, Nothing)

        Dim args(6) As Double
        rasterDS.GetGeoTransform(args)
        maskDataset.SetGeoTransform(args)
        maskDataset.SetProjection(rasterDS.GetProjection())

        Gdal.RasterizeLayer(maskDataset, 1, New Integer() {1}, layer, IntPtr.Zero, IntPtr.Zero, 1, New Double(0) {1}, New String() {"ALL_TOUCHED=TRUE"}, Nothing, "")

        Dim maskData As Byte() = New Byte(xSize * ySize) {}
        maskDataset.GetRasterBand(1).ReadRaster(0, 0, xSize, ySize, maskData, xSize, ySize, 0, 0)

        Dim rasterData As Single() = New Single(xSize * ySize) {}
        band.ReadRaster(0, 0, xSize, ySize, rasterData, xSize, ySize, 0, 0)

        For i As Integer = 0 To (xSize * ySize) - 1
            If (maskData(i) = 1) Then
                Dim pixelValue As Single = rasterData(i)
                'escludo la key = 0 
                If pixelValue <> 0 Then
                    If (frequencyHistogram.ContainsKey(pixelValue)) Then
                        frequencyHistogram(pixelValue) = frequencyHistogram(pixelValue) + 1
                    Else
                        frequencyHistogram(pixelValue) = 1
                    End If
                End If
            End If
        Next

        feature.Dispose()
        layer.Dispose()
        band.Dispose()
        rasterDS.Dispose()
        maskDataset.Dispose()
        memDS.Dispose()

        Return frequencyHistogram
    End Function

End Class
