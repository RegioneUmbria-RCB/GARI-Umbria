Imports GeoAPI.CoordinateSystems.Transformations
Imports GeoAPI.CoordinateSystems
Imports ProjNet.CoordinateSystems.Transformations


Namespace Agronica

    Public Class CoordinateConverterPROJNET
        Inherits CoordinateConverterAbstract


        Private Shared _objParametri As ParametriCoordinateConverter

        Public Shared ReadOnly Property ObjParametri() As ParametriCoordinateConverter
            Get
                Return _objParametri
            End Get
        End Property

        'Private _fromCS As ICoordinateSystem
        'Private _toCS As ICoordinateSystem
        'Private _toCSGEO As ICoordinateSystem



        Public Overrides Function ProiettaPunto( _
                                        [In] As PointObject, _
                                        ByVal swapCoordinates As Boolean, _
                                        ByRef objParametri As ParametriCoordinateConverter) _
                                        As PointObject

            Dim tbCSFrom_Text As String = ""
            Dim tbCSto_Text As String = ""
            Dim tbGeo_Text As String = ""

            'If _fromCS Is Nothing Then
            tbCSFrom_Text = objParametri.CSFromText
            tbCSto_Text = objParametri.CStoText
            tbGeo_Text = objParametri.CStoGeoText
            'End If

            Dim fact = New ProjNet.CoordinateSystems.CoordinateSystemFactory

            ''non Projected
            'If _fromCS Is Nothing Then
            '    If tbCSFrom_Text.StartsWith("PROJCS") Then
            '        _fromCS = DirectCast(fact.CreateFromWkt(tbCSFrom_Text), IProjectedCoordinateSystem)
            '    Else
            '        _fromCS = DirectCast(fact.CreateFromWkt(tbCSFrom_Text), ICoordinateSystem)
            '    End If
            'End If


            ''ICoordinateSystem fromCS = SharpMap.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom.Text);
            'If _toCS Is Nothing Then
            '    If tbCSto_Text.StartsWith("PROJCS") Then
            '        _toCS = DirectCast(ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSto_Text), IProjectedCoordinateSystem)
            '    Else
            '        _toCS = DirectCast(ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSto_Text), ICoordinateSystem)
            '    End If
            'End If




            ''TO_GEO
            ''ICoordinateSystem fromCS = SharpMap.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom.Text);
            'If _toCSGEO Is Nothing Then
            '    If tbGeo_Text.StartsWith("PROJCS") Then
            '        _toCSGEO = DirectCast(ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbGeo_Text), IProjectedCoordinateSystem)
            '    Else
            '        _toCSGEO = DirectCast(ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbGeo_Text), ICoordinateSystem)
            '    End If
            'End If



            'First create a CoordinateTransformationFactory:
            Dim ctfac As New CoordinateTransformationFactory()

            'Then create the transformation instance:
            Dim trans As ICoordinateTransformation = ctfac.CreateFromCoordinateSystems(fact.CreateFromWkt(tbCSFrom_Text), fact.CreateFromWkt(tbCSto_Text))
            Dim planGeo As ICoordinateTransformation = ctfac.CreateFromCoordinateSystems(fact.CreateFromWkt(tbCSto_Text), fact.CreateFromWkt(tbGeo_Text))

            Dim fromPoint As Double() = New Double() {[In].XCoord, [In].YCoord}
            Dim toPoint As Double() = trans.MathTransform.Transform(fromPoint)


            Dim fromPlanPoint As Double() = New Double() {toPoint(0), toPoint(1)}
            Dim toGeoPoint As Double() = planGeo.MathTransform.Transform(fromPlanPoint)

            'Agronica Offset
            Dim AgroLatOffset As Double, AgroLonOffset As Double

            AgroLatOffset = objParametri.AgronicaLatOffset
            AgroLonOffset = objParametri.AgronicaLonOffset

            Dim a As Int16 = 0
            Dim b As Int16 = 1

            If swapCoordinates Then
                a = 1
                b = 0
            End If

            Dim rVal As New PointObject()
            rVal.XCoord = toGeoPoint(a) + AgroLonOffset
            rVal.YCoord = toGeoPoint(b) + AgroLatOffset
            rVal.value = [In].value

            Return rVal

        End Function



    End Class

End Namespace
