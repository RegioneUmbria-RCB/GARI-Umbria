
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports OSGeo.GDAL

Namespace Agronica.Helpers.GDALHelper

    Public Class InfoRaster

        Shared Sub New()
            GdalConfiguration.ConfigureGdal()
            GdalConfiguration.ConfigureOgr()
        End Sub

        Public Function GetRasterInfo(ByVal pathFileRaster As String, ByVal point As String) As List(Of Double)
            Dim DS = Gdal.Open(pathFileRaster, Access.GA_ReadOnly)

            Dim gt(6) As Double

            DS.GetGeoTransform(gt) 'Read geo transform info into array
            Dim Rows As Int32 = DS.RasterYSize 'Get the number Of rows 
            Dim Cols As Int32 = DS.RasterXSize 'Get the number Of columns

            'Band Band = DS.GetRasterBand(1);    //Read band 1
            Dim startX = gt(0)      'Upper left lon
            Dim startY = gt(3)      'Upper left lat
            Dim interval = gt(1)    'Cell size

            Dim pointParts = point.Substring(6, point.Length - 7).Split(" ")

            'Current lon And lat
            Dim lat = CDbl(pointParts(0).Replace(".", ","))
            Dim lng = CDbl(pointParts(1).Replace(".", ","))

            Dim pixelLatT As Double
            Dim pixelLngT As Double

            Gdal.InvGeoTransform(gt, gt)

            Gdal.ApplyGeoTransform(gt, lat, lng, pixelLatT, pixelLngT)

            Dim col = CInt(pixelLatT)
            Dim row = CInt(pixelLngT)

            Dim valueArray = New List(Of Double)

            For BandIndex As Int32 = 1 To DS.RasterCount
                Dim band = DS.GetRasterBand(BandIndex)
                Dim buffer(0) As Double

                band.ReadRaster(col, row, 1, 1, buffer, 1, 1, 0, 0)

                valueArray.Add(buffer(0))
            Next

            Return valueArray

        End Function

        Public Function COGChecker(pathFile As String) As Boolean
            Dim DS = Gdal.Open(pathFile, Access.GA_ReadOnly)

            Dim infoJSON = JObject.Parse(Gdal.GDALInfo(DS, New GDALInfoOptions({"-json"})))

            DS.Dispose()

            Dim layout = infoJSON.SelectToken("metadata.IMAGE_STRUCTURE.LAYOUT")

            Return layout IsNot Nothing AndAlso layout.ToString.Equals("COG")
        End Function

        Public Function ConvertToCOG(pathFile As String) As String
            Dim fileName = Path.GetFileNameWithoutExtension(pathFile)

            Dim newPathFile = String.Format("{0}/{1}{2}{3}", Path.GetDirectoryName(pathFile), fileName, "_COG", Path.GetExtension(pathFile))

            Dim DS = Gdal.Open(pathFile, Access.GA_ReadOnly)

            Dim newDS = Gdal.wrapper_GDALTranslate(newPathFile, DS, New GDALTranslateOptions({"-of", "COG"}), Nothing, Nothing)

            newDS.FlushCache()
            newDS.Dispose()
            DS.Dispose()

            Return newPathFile
        End Function
    End Class
End Namespace
