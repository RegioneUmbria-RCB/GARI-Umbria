Imports AgronicaCoreModello.Geometry
Imports AgronicaCoreModelsSTD.Gis
Imports Newtonsoft.Json

Public Class WktParser
    Implements IGeoJsonConverter, IGeometry

    Private ReadOnly _decsep As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
    Private Class GeometryTypeTranslation
        Public Const Point As String = "POINT"
        Public Const MultiPoint As String = "MULTIPOINT"
        Public Const LineString As String = "LINESTRING"
        Public Const MultiLineString As String = "MULTILINESTRING"
        Public Const Polygon As String = "POLYGON"
        Public Const MultiPolygon As String = "MULTIPOLYGON"
        Public Const GeometryCollection As String = "GEOMETRYCOLLECTION"
    End Class


    Dim _geometry As GeometryDefinition = Nothing
    Dim _wktstr As String = ""

    Public Sub New(ByVal wkt As String)
        _wktstr = wkt
        Convert()
    End Sub

    'Public Sub New()

    'End Sub

    Private Sub Convert()
        If _wktstr = "" Then
            Throw New Exception("String WKT non valorizzata. Impossibile eseguire conversione")
        End If

        Dim shapetype As String = _getGeometryTypeFromString(_wktstr)
        Dim shapeobject As String = _wktstr.Replace(shapetype, "").Trim

        ValidaFormatoGeometria(shapetype, shapeobject)

        _geometry = _convert(shapetype, shapeobject)
    End Sub

    Private Function _convert(ByVal shapetype As String, ByVal shapeobject As String) As GeometryDefinition
        Dim r As New GeometryDefinition
        r.type = shapetype
        r.composition = _getGeometryComposition(shapetype, shapeobject)
        Return r
    End Function

    Private Function _getGeometryComposition(ByVal shapetype As String, ByVal shapeobject As String) As GeometryShapeComposition
        Dim r As New GeometryShapeComposition
        Dim i As Integer = 0
        Dim w As String = ""
        Dim newObj As New GeometryShapeDefinition()
        Select Case shapetype
            Case GeometryTypeTranslation.Point
                w = shapeobject.Replace("(", "").Replace(")", "").Trim
                newObj.shape = _getPointList(w.Split(""))
                r.shapeform.Add(newObj)

            Case GeometryTypeTranslation.MultiPoint
                w = shapeobject.Replace("(", "").Replace(")", "").Trim
                Dim p = w.Split(", ")
                'If p.Length < 2 Then
                '    Throw New Exception("Il tipo geospaziale MULTIPOINT deve avere almeno due punti ")
                'End If
                newObj.shape = _getPointList(p)
                r.shapeform.Add(newObj)

            Case GeometryTypeTranslation.LineString
                w = shapeobject.Replace("(", "").Replace(")", "").Trim
                Dim p = w.Split(", ")
                If p.Length < 2 Then
                    Throw New Exception("Il tipo geospaziale LINESTRING deve avere almeno due punti ")
                End If
                newObj.shape = _getPointList(p)
                r.shapeform.Add(newObj)

            Case GeometryTypeTranslation.MultiLineString
                Dim lines = shapeobject.Split("), (").Where(Function(x) x <> "").ToArray
                'If lines.Length < 2 Then
                '    Throw New Exception("Il tipo geospaziale MULTILINESTRING deve avere almeno due linee ")
                'End If
                For Each line In lines
                    line = line.Replace("(", "").Replace(")", "").Trim
                    newObj = New GeometryShapeDefinition()
                    newObj.shape = _getPointList(line.Split(", "))
                    r.shapeform.Add(newObj)
                Next

            Case GeometryTypeTranslation.Polygon
                Dim poly = shapeobject.Split({"), ("}, StringSplitOptions.RemoveEmptyEntries)
                newObj = New GeometryShapeDefinition()
                newObj.shape = _getPointList(poly(0).Replace("(", "").Replace(")", "").Split(", "))

                For i = 1 To poly.Length - 1
                    newObj.hole.Add(_getPointList(poly(i).Replace("(", "").Replace(")", "").Split(", ")))
                Next
                r.shapeform.Add(newObj)

            Case GeometryTypeTranslation.MultiPolygon
                w = shapeobject.Substring(1, shapeobject.Length - 2)
                Dim mpolys = w.Split({")), (("}, StringSplitOptions.RemoveEmptyEntries)
                For Each mpoly In mpolys
                    Dim poly = mpoly.Split({"), ("}, StringSplitOptions.RemoveEmptyEntries)
                    newObj = New GeometryShapeDefinition()
                    newObj.shape = _getPointList(poly(0).Replace("(", "").Replace(")", "").Split(", "))

                    For i = 1 To poly.Length - 1
                        newObj.hole.Add(_getPointList(poly(i).Replace("(", "").Replace(")", "").Split(", ")))
                    Next
                    r.shapeform.Add(newObj)
                Next

            Case GeometryTypeTranslation.GeometryCollection
                'per ora no


                'w = w.Substring(1, shapeobject.Length - 2)
                'Dim objlist = Decompose(w)
                'For Each obj In objlist
                '    Dim subshapetype = _getGeometryTypeFromString(obj)
                '    Dim subshapeobject = obj.Replace(subshapetype, "").Trim
                '    VerificaInsiemePunti(subshapeobject, subshapetype, True)
                'Next

        End Select
        Return r
    End Function

    Private Function _getPointList(ByVal strPoint As String()) As GeometryShape
        Dim r As New GeometryShape
        For Each point In strPoint.Where(Function(x) x <> "").ToList
            Dim p = point.Trim.Split(" ")
            If p.Length <> 2 Then
                Throw New Exception("Punto non definito da due (x,y) ")
            End If
            r.Add(New Geometry.Point() With {
                    .Latitude = Decimal.Parse(p(0).Replace(".", _decsep), Globalization.NumberStyles.Any),
                    .Longitude = Decimal.Parse(p(1).Replace(".", _decsep), Globalization.NumberStyles.Any)
                })
        Next
        Return r
    End Function

    Private Sub ValidaFormatoGeometria(ByVal shapetype As String, ByVal shapeobject As String)
        Select Case shapetype
            Case GeometryTypeTranslation.Point
                VerificaDefinizionePOINT(shapeobject)
            Case GeometryTypeTranslation.MultiPoint
                VerificaDefinizioneMULTIPOINT(shapeobject)
            Case GeometryTypeTranslation.LineString
                VerificaDefinizioneLINESTRING(shapeobject)
            Case GeometryTypeTranslation.MultiLineString
                VerificaDefinizioneMULTILINESTRING(shapeobject)
            Case GeometryTypeTranslation.Polygon
                VerificaDefinizionePOLYGON(shapeobject)
            Case GeometryTypeTranslation.MultiPolygon
                VerificaDefinizioneMULTIPOLYGON(shapeobject)
            Case GeometryTypeTranslation.GeometryCollection
                VerificaDefinizioneGEOMETRYCOLLECTION(shapeobject)
        End Select
    End Sub

    Private Sub VerificaDefinizionePOINT(ByVal shapeobject As String)
        If Not VerificaParentesiIniziale(shapeobject, 1) Then
            Throw New Exception("Formato WKT non valido per la geometria POINT. Impossibile proseguire")
        Else
            Dim r As Decimal = -1

            If Not VerificaPrimaCoordinata(shapeobject, 2) Then
                Throw New Exception("Formato WKT non valido per la geometria POINT. Impossibile proseguire")
            End If
        End If
        VerificaInsiemePunti(shapeobject, GeometryTypeTranslation.Point)
    End Sub

    Private Sub VerificaDefinizioneMULTIPOINT(ByVal shapeobject As String)
        Dim check1 As Boolean = VerificaParentesiIniziale(shapeobject, 0)
        Dim check2 As Boolean = VerificaParentesiIniziale(shapeobject, 1)
        If Not check1 AndAlso Not check2 Then
            Throw New Exception("Formato WKT non valido per la geometria MULTIPOINT. Impossibile proseguire")
        Else
            Dim r As Decimal = -1
            If check1 And Not check2 Then
                If Not VerificaPrimaCoordinata(shapeobject, 2) Then
                    Throw New Exception("Formato WKT non valido per la geometria MULTIPOINT. Impossibile proseguire")
                End If
            Else
                If Not VerificaPrimaCoordinata(shapeobject, 3) Then
                    Throw New Exception("Formato WKT non valido per la geometria MULTIPOINT. Impossibile proseguire")
                End If
            End If
        End If
        VerificaInsiemePunti(shapeobject, GeometryTypeTranslation.MultiPoint)
    End Sub

    Private Sub VerificaDefinizioneLINESTRING(ByVal shapeobject As String)
        If Not VerificaParentesiIniziale(shapeobject, 0) Then
            Throw New Exception("Formato WKT non valido per la LINESTRING. Impossibile proseguire")
        Else
            If Not VerificaPrimaCoordinata(shapeobject, 2) Then
                Throw New Exception("Formato WKT non valido per la geometria LINESTRING. Impossibile proseguire")
            End If
        End If
        VerificaInsiemePunti(shapeobject, GeometryTypeTranslation.LineString)
    End Sub

    Private Sub VerificaDefinizioneMULTILINESTRING(ByVal shapeobject As String)
        Dim check1 As Boolean = VerificaParentesiIniziale(shapeobject, 0)
        Dim check2 As Boolean = VerificaParentesiIniziale(shapeobject, 1)
        If Not check1 AndAlso Not check2 Then
            Throw New Exception("Formato WKT non valido per la geometria MULTILINESTRING. Impossibile proseguire")
        Else
            Dim r As Decimal = -1
            If check1 And Not check2 Then
                If Not VerificaPrimaCoordinata(shapeobject, 2) Then
                    Throw New Exception("Formato WKT non valido per la geometria MULTILINESTRING. Impossibile proseguire")
                End If
            Else
                If Not VerificaPrimaCoordinata(shapeobject, 3) Then
                    Throw New Exception("Formato WKT non valido per la geometria MULTILINESTRING. Impossibile proseguire")
                End If
            End If
        End If
        VerificaInsiemePunti(shapeobject, GeometryTypeTranslation.MultiLineString)
    End Sub

    Private Sub VerificaDefinizionePOLYGON(ByVal shapeobject As String)
        If Not VerificaParentesiIniziale(shapeobject, 1) Then
            Throw New Exception("Formato WKT non valido per la POLYGON. Impossibile proseguire")
        Else
            If Not VerificaPrimaCoordinata(shapeobject, 3) Then
                Throw New Exception("Formato WKT non valido per la geometria POLYGON. Impossibile proseguire")
            End If
        End If
        VerificaInsiemePunti(shapeobject, GeometryTypeTranslation.Polygon)
    End Sub

    Private Sub VerificaDefinizioneMULTIPOLYGON(ByVal shapeobject As String)
        If Not VerificaParentesiIniziale(shapeobject, 2) Then
            Throw New Exception("Formato WKT non valido per la MULTIPOLYGON. Impossibile proseguire")
        Else
            If Not VerificaPrimaCoordinata(shapeobject, 4) Then
                Throw New Exception("Formato WKT non valido per la geometria POLYGON. Impossibile proseguire")
            End If
        End If
        VerificaInsiemePunti(shapeobject, GeometryTypeTranslation.MultiPolygon)
    End Sub

    Private Sub VerificaDefinizioneGEOMETRYCOLLECTION(ByVal shapeobject As String)
        If Not VerificaParentesiIniziale(shapeobject, 1) Then
            Throw New Exception("Formato WKT non valido per la GEOMETRYCOLLECTION. Impossibile proseguire")
        End If

        If Not shapeobject.Substring(1).StartsWith(GeometryTypeTranslation.Point) AndAlso
           Not shapeobject.Substring(1).StartsWith(GeometryTypeTranslation.LineString) AndAlso
           Not shapeobject.Substring(1).StartsWith(GeometryTypeTranslation.Polygon) Then

            Throw New Exception("Formato WKT non valido per la geometria GEOMETRYCOLLECTION. Impossibile proseguire")
        End If
        VerificaInsiemePunti(shapeobject, GeometryTypeTranslation.GeometryCollection)
    End Sub


    Private Function VerificaParentesiIniziale(ByVal shapeobject As String, ByVal numeroParentesi As Integer) As Boolean
        Return shapeobject.StartsWith(New String("(", numeroParentesi))
    End Function

    Private Function VerificaPrimaCoordinata(ByVal shapeobject As String, ByVal pos As Integer) As Boolean
        Dim r As Decimal = -1
        If shapeobject.Substring(pos, 1) = "-" Then
            Return Decimal.TryParse(shapeobject.Substring(pos + 1, 1).Replace(".", _decsep), r)
        Else
            'Return Decimal.TryParse(shapeobject.Substring(pos, 1).Replace(".", _decsep), r)
            Return Decimal.TryParse(shapeobject.Substring(pos, 2).Replace(".", _decsep), r)
            Return Decimal.TryParse(shapeobject.Substring(pos, 1).Replace(".", _decsep), r)
        End If

    End Function

    Private Sub VerificaInsiemePunti(ByVal shapeobject As String, ByVal shapetype As String, Optional ByVal subobject As Boolean = False)
        Select Case shapetype
            Case GeometryTypeTranslation.Point
                'deve avere una sola coppia di coordinate Lat/Long
                Dim pointList = shapeobject.Replace("(", "").Replace(")", "").Split(",")
                If Not pointList.Length = 1 Then
                    Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " POINT con più punti. formato non valido.")
                End If
            Case GeometryTypeTranslation.MultiPoint
                Dim pointList = shapeobject.Replace("(", "").Replace(")", "").Split(",")
                'Lavez - 22/02/2024 - controllo rilassato perchè formalmente non corretto. Un multipoint può contenere anche un solo point
                'If Not pointList.Length > 1 Then
                '    Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " MULTIPOINT con un solo punto. formato non valido.")
                'End If
            Case GeometryTypeTranslation.LineString
                Dim lineList = shapeobject.Replace("(", "").Replace(")", "").Split(",")
                If Not lineList.Length > 1 Then
                    Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " LINESTRING con un solo punto. formato non valido.")
                End If
            Case GeometryTypeTranslation.MultiLineString
                Dim lineList = shapeobject.Split("),(")
                'Lavez - 22/02/2024 - controllo rilassato perchè formalmente non corretto. Una multilinestring può contenere anche una sola linestring
                'If Not lineList.Length > 1 Then
                '    Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " MULTILINESTRING con una sola Linea. formato non valido.")
                'End If
                For Each line In lineList.Where(Function(x) x <> "").ToList
                    Dim pointList = line.Replace("(", "").Replace(")", "").Split(",").Where(Function(x) x <> "").ToArray
                    If Not pointList.Length > 1 Then
                        Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " MULTILINESTRING rilevata LINESTRING con un solo punto. formato non valido.")
                    End If
                Next
            Case GeometryTypeTranslation.Polygon
                Dim polyList = shapeobject.Split("),")

                For Each poly In polyList.Where(Function(x) x <> "").ToList
                    Dim pointList = poly.Replace("(", "").Replace(")", "").Split(",")
                    If Not pointList.Length > 1 Then
                        Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " POLYGON elenco punti non corretto. impossibile proseguire.")
                    End If
                Next
            Case GeometryTypeTranslation.MultiPolygon
                Dim polyList = shapeobject.Split("),(").Where(Function(x) x <> "").ToList
                'Lavez - 22/02/2024 - controllo rilassato perchè formalmente non corretto. Un Multipolygon può contenere anche un solo polygon
                'If Not polyList.Count > 1 Then
                '    Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " MULTIPOLYGON con un solo Poligono. formato non valido.")
                'End If
                For Each poly In polyList
                    Dim pointList = poly.Replace("(", "").Replace(")", "").Split(",").Where(Function(x) x <> "").ToArray
                    If Not pointList.Length > 1 Then
                        Throw New Exception("Stringa WKT per la " + IIf(subobject, "sub geometria", "geometria") + " MULTIPOLYGON elenco punti non corretto. impossibile proseguire.")
                    End If
                Next
            Case GeometryTypeTranslation.GeometryCollection
                Dim w As String = shapeobject.Substring(1, shapeobject.Length - 2)
                Dim objlist = Decompose(w)
                For Each obj In objlist
                    Dim subshapetype = _getGeometryTypeFromString(obj)
                    Dim subshapeobject = obj.Replace(subshapetype, "").Trim
                    VerificaInsiemePunti(subshapeobject, subshapetype, True)
                Next
        End Select

    End Sub

    Private Function Decompose(ByVal str As String) As String()
        Dim r As New List(Of String)

        Dim i = _getNearestGeometryType(str, 1)
        Dim idx = 0
        While i < 9999999
            r.Add(str.Substring(idx, ((i - 1) - idx)))
            idx = i
            i = _getNearestGeometryType(str, idx)
        End While
        r.Add(str.Substring(idx))
        Return r.ToArray()
    End Function

    Private Function _getNearestGeometryType(ByVal str As String, ByVal start As Integer) As Integer
        Dim a As Integer, b As Integer, c As Integer, r As Integer = 9999999

        a = str.IndexOf(GeometryTypeTranslation.Point, start)
        b = str.IndexOf(GeometryTypeTranslation.LineString, start)
        c = str.IndexOf(GeometryTypeTranslation.Polygon, start)

        If a > start Then
            If a < r Then
                r = a
            End If
        End If

        If b > start Then
            If b < r Then
                r = b
            End If
        End If

        If c > start Then
            If c < r Then
                r = c
            End If
        End If

        Return r
    End Function

    Private Function _getGeometryTypeFromString(ByVal str As String) As String
        Return str.Substring(0, str.IndexOf("(") - 1).Trim
    End Function

    Private Function _getFeatureType(ByVal type As String) As FeatureType
        Select Case type
            Case GeometryTypeTranslation.Point
                Return FeatureType.Point
            Case GeometryTypeTranslation.MultiPoint
                Return FeatureType.MultiPoint
            Case GeometryTypeTranslation.LineString
                Return FeatureType.Linestring
            Case GeometryTypeTranslation.MultiLineString
                Return FeatureType.MultiLineString
            Case GeometryTypeTranslation.Polygon
                Return FeatureType.Polygon
            Case GeometryTypeTranslation.MultiPolygon
                Return FeatureType.MultiPolygon
        End Select
    End Function

    Private Function _getGeometryPointToString() As String
        Return JsonConvert.SerializeObject(_getPointList(_geometry.composition.shapeform(0).shape(0)))
    End Function

    Private Function _getGeometryMultiPointToString() As String
        Return JsonConvert.SerializeObject(_getShapePointList(_geometry.composition.shapeform(0).shape))
    End Function
    Private Function _getGeometryLineStringToString() As String
        Return JsonConvert.SerializeObject(_getShapePointList(_geometry.composition.shapeform(0).shape))
    End Function
    Private Function _getGeometryMultiLineStringToString() As String
        Dim wrk As New List(Of List(Of List(Of Double)))
        For Each form In _geometry.composition.shapeform
            wrk.Add(_getShapePointList(form.shape))
        Next

        Return JsonConvert.SerializeObject(wrk)
    End Function
    Private Function _getGeometryPolygonToString() As String

        Dim wrk As New List(Of List(Of List(Of Double)))
        For Each form In _geometry.composition.shapeform
            wrk.Add(_getShapePointList(form.shape))
            For Each hole In form.hole
                wrk.Add(_getHolePointList(hole))
            Next
        Next

        Return JsonConvert.SerializeObject(wrk)
    End Function

    Private Function _getGeometryMultiPolygonToString() As String
        Dim wrk As New List(Of List(Of List(Of List(Of Double))))

        Dim poly As List(Of List(Of List(Of Double)))

        For Each form In _geometry.composition.shapeform
            poly = New List(Of List(Of List(Of Double)))
            poly.Add(_getShapePointList(form.shape))
            For Each hole In form.hole
                poly.Add(_getHolePointList(hole))
            Next
            wrk.Add(poly)
        Next

        Return JsonConvert.SerializeObject(wrk)
    End Function

    Private Function _getShapePointList(shape As GeometryShape) As List(Of List(Of Double))
        Dim shapeRet As New List(Of List(Of Double))
        For Each point In shape
            shapeRet.Add(_getPointList(point))
        Next
        If shape.Count <= 0 Then
            Throw New Exception("Polygon senza forma intera")
        End If
        Return shapeRet
    End Function

    Private Function _getHolePointList(hole As GeometryShape) As List(Of List(Of Double))
        Dim shapeRet As New List(Of List(Of Double))
        For Each point In hole
            shapeRet.Add(_getPointList(point))
        Next
        Return shapeRet
    End Function

    Private Function _getPointList(point As Geometry.Point) As List(Of Double)
        Dim pt As New List(Of Double)
        pt.Add(point.Latitude)
        pt.Add(point.Longitude)
        Return pt
    End Function


    Public Function getGeometry() As GeometryDefinition Implements IGeometry.getGeometry
        Return _geometry
    End Function

    Public Function ToGeoJson() As GeoJson_Geometry_New Implements IGeoJsonConverter.ToGeoJson
        Try
            Dim coordinates As String = ""
            Select Case _geometry.type
                Case GeometryTypeTranslation.Point
                    coordinates = _getGeometryPointToString()
                Case GeometryTypeTranslation.MultiPoint
                    coordinates = _getGeometryMultiPointToString()
                Case GeometryTypeTranslation.LineString
                    coordinates = _getGeometryLineStringToString()
                Case GeometryTypeTranslation.MultiLineString
                    coordinates = _getGeometryMultiLineStringToString()
                Case GeometryTypeTranslation.Polygon
                    coordinates = _getGeometryPolygonToString()
                Case GeometryTypeTranslation.MultiPolygon
                    coordinates = _getGeometryMultiPolygonToString()
            End Select

            Return New GeoJson_Geometry_New(_getFeatureType(_geometry.type), coordinates)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function


End Class



