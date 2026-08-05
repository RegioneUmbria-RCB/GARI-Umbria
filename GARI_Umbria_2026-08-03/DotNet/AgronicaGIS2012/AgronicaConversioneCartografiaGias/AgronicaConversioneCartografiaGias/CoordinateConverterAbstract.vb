

Namespace Agronica
    Public MustInherit Class CoordinateConverterAbstract

        Public MustOverride Function ProiettaPunto( _
                    [In] As PointObject, _
            ByVal swapCoordinates As Boolean, _
            ByRef objParametri As ParametriCoordinateConverter) _
            As PointObject


    End Class

End Namespace

