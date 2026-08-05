
Imports DotSpatial.Projections

Namespace Agronica

    Public Class CoordinateConverterProj4
        Inherits CoordinateConverterAbstract


        Public Overrides Function ProiettaPunto( _
                [In] As PointObject, _
                ByVal swapCoordinates As Boolean, _
                ByRef objParametri As ParametriCoordinateConverter) _
                As PointObject


            Dim pStart As New ProjectionInfo()
            If objParametri.CSFromText.StartsWith("PROJCS") Or _
                 objParametri.CSFromText.StartsWith("GEOGCS") Then
                pStart = ProjectionInfo.FromEsriString(objParametri.CSFromText)
            Else
                pStart = ProjectionInfo.FromProj4String(objParametri.CSFromText)
            End If


            Dim pEnd As New ProjectionInfo()
            If objParametri.CStoText.StartsWith("PROJCS") Or _
                 objParametri.CStoText.StartsWith("GEOGCS") Then
                pEnd = ProjectionInfo.FromEsriString(objParametri.CStoText)
            Else
                pEnd = ProjectionInfo.FromProj4String(objParametri.CStoText)
            End If


            'Declares the point to be project, starts out as 0,0
            Dim xy As Double() = New Double(1) {[In].XCoord, [In].YCoord}
            Dim z As Double() = New Double(0) {[In].zCoord}
            'calls the reproject function and reprojects the points


            Reproject.ReprojectPoints(xy, z, pStart, pEnd, 0, 1)


            Dim a As Int16 = 0
            Dim b As Int16 = 1

            If swapCoordinates Then
                a = 1
                b = 0
            End If

            Dim rVal As New PointObject()
            rVal.XCoord = xy(a)
            rVal.YCoord = xy(b)
            rVal.value = [In].value

            Return rVal

        End Function



        'Code that allows the user to input a Proj4 string and reproject a WGS 1984 GCS to a Proj4 PCS
        Private Sub btnProjection_Click(sender As Object, e As EventArgs)

            'MessageBox.Show("Reprojection is compelte.")
        End Sub



       
    End Class

End Namespace
