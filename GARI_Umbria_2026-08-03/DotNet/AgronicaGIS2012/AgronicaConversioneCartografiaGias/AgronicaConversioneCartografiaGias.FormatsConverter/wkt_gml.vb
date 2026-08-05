Imports <xmlns="http://www.agronica.it/grafica/">

Imports AgronicaGIS2012.Commons
Imports AgronicaConversioneCartografiaGias.Agronica

Public Class wkt_gml

    Public Sub New()

    End Sub


    Public Function Trasforma(ByVal wkt As String, ByVal SwapLatLong As Boolean, ByVal InvertOrder As Boolean, ByVal EscludiPuntiRipetuti As Boolean, ByVal ElementoGrafico_Cod As Integer) As String

        Dim rval As String = ""
        If wkt.ToLower.StartsWith("polygon") Then
            rval = TrasformaPoligono(wkt, SwapLatLong, InvertOrder, EscludiPuntiRipetuti, ElementoGrafico_Cod)
        End If

        If wkt.ToLower.StartsWith("point") Then
            rval = TrasformaPunto(wkt)
        End If

        If wkt.ToLower.StartsWith("linestring") Then
            rval = TrasformaPolyline(wkt)
        End If
        Return rval

    End Function

    Private Function TrasformaPolyline(ByVal wkt As String) As String

        Dim oDoc As New XDocument
        Dim coordinates As String
        coordinates = get_coordinates(wkt, False, False, False)

        ' Mengarda 2026-14-16 #213041
        ' removed xml root element <geodata>
        ' because it's not considered valid by SQL Server
        Dim outputPattern As XElement = <gml:LineString xmlns:gml="http://www.opengis.net/gml">
                                            <gml:posList><%= coordinates %></gml:posList>
                                        </gml:LineString>

        oDoc.Add(outputPattern)

        Return oDoc.ToString

    End Function
    Private Function TrasformaPunto(ByVal wkt As String) As String

        Dim oDoc As New XDocument
        Dim coordinates As String
        coordinates = get_coordinates(wkt, False, False, False)

        ' Mengarda 2026-14-16 #213041
        ' removed xml root element <geodata>
        ' because it's not considered valid by SQL Server
        Dim outputPattern As XElement = <gml:Point xmlns:gml="http://www.opengis.net/gml">
                                            <gml:pos><%= coordinates %></gml:pos>
                                        </gml:Point>

        oDoc.Add(outputPattern)

        Return oDoc.ToString

    End Function


    Private Function TrasformaPoligono(ByVal wkt As String, ByVal SwapLatLong As Boolean, ByVal InvertOrder As Boolean, ByVal EscludiPuntiRipetuti As Boolean, ByVal ElementoGrafico_Cod As Integer) As String

        Dim oDoc As New XDocument
        Dim coordOuterBound As String = ""
        Dim coordInnerBound As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)

        get_coordinates(wkt, SwapLatLong, InvertOrder, EscludiPuntiRipetuti, coordOuterBound, coordInnerBound)

        If coordInnerBound Is Nothing Then
            ' Mengarda 2026-03-02 #207311
            ' removed xml root element <geodata ElementoGrafico_Cod=<%= ElementoGrafico_Cod %>>
            ' because it's not considered valid by SQL Server
            Dim outputPattern As XElement = 
                                                <gml:Polygon xmlns:gml="http://www.opengis.net/gml">
                                                    <gml:exterior>
                                                        <gml:LinearRing>
                                                            <gml:posList><%= coordOuterBound %></gml:posList>
                                                        </gml:LinearRing>
                                                    </gml:exterior>
                                                </gml:Polygon>

            oDoc.Add(outputPattern)
        Else
            ' Mengarda 2026-03-02 #207311
            ' removed xml root element <geodata ElementoGrafico_Cod=<%= ElementoGrafico_Cod %>>
            ' because it's not considered valid by SQL Server
            Dim outputPattern As XElement = <gml:Polygon xmlns:gml="http://www.opengis.net/gml">
                                                    <gml:exterior>
                                                        <gml:LinearRing>
                                                            <gml:posList><%= coordOuterBound %></gml:posList>
                                                        </gml:LinearRing>
                                                    </gml:exterior>
                                                </gml:Polygon>

            Dim polygonElement As XElement = outputPattern.Descendants.First()

            For Each key In coordInnerBound.Keys
                If coordInnerBound(key) <> "" Then
                    Dim interiorElement As XElement = <gml:interior xmlns:gml="http://www.opengis.net/gml">
                                                      <gml:LinearRing>
                                                          <gml:posList><%= coordInnerBound(key) %></gml:posList>
                                                      </gml:LinearRing>
                                                  </gml:interior>
                    polygonElement.Add(interiorElement)
                End If
            Next

            oDoc.Add(outputPattern)
        End If


        Return oDoc.ToString

    End Function



    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function

    Private _Soglia_ConsideraPuntiUguali As Double = 0.00000000005
    Public Property Soglia_ConsideraPuntiUguali() As Double
        Get
            Return _Soglia_ConsideraPuntiUguali
        End Get
        Set(value As Double)
            _Soglia_ConsideraPuntiUguali = value
        End Set
    End Property

    Private Function confrontaCoordinateUguali(ByVal p1 As xyz, ByVal p2 As xyz) As Boolean
        'Dim rval As Boolean = False
        'rval = Math.Abs(p1.X - p2.X) < _Soglia_ConsideraPuntiUguali
        'rval = Math.Abs(p1.Y - p2.Y) < _Soglia_ConsideraPuntiUguali
        'Return rval
        Dim sogliaX As Boolean = Math.Abs(p1.X - p2.X) < _Soglia_ConsideraPuntiUguali
        Dim sogliaY As Boolean = Math.Abs(p1.Y - p2.Y) < _Soglia_ConsideraPuntiUguali
        Return sogliaX And sogliaY
    End Function

    Private Function ConfrontaVettoreCoordintate(ByVal coord As String(), ByVal p As xyz) As Boolean
        Dim trovato As Boolean = False
        If coord.Length <> 1 Then
            For Each c In coord
                Dim cc As String() = c.Split(" ")
                If confrontaCoordinateUguali(p, New xyz With {.X = xyz.myCDBL(cc(0)), .Y = xyz.myCDBL(cc(1).Replace(",", "."))}) Then
                    trovato = True
                    Exit For
                End If

            Next
        End If

        Return trovato
    End Function


    Public Sub get_coordinates(ByVal wkt As String, ByVal SwapLatLong As Boolean, ByVal InvertOrder As Boolean, ByVal EscludiPuntiRipetuti As Boolean, ByRef outerBound As String, ByRef innerBound As Dictionary(Of Integer, String))

        Dim vetOuterBound As String() = Nothing
        Dim vetInnerBound As Dictionary(Of Integer, String()) = New Dictionary(Of Integer, String())

        CoordinateConverter.wktGetOuterAndInnerBoundArray(wkt, vetOuterBound, vetInnerBound)

        outerBound = get_coordinates(wktPolyFromCoord(vetOuterBound), SwapLatLong, InvertOrder, EscludiPuntiRipetuti)

        Dim c As Integer = 0
        For Each innerBoundKey In vetInnerBound.Keys
            Dim tmpInnerBoundStr = get_coordinates(wktPolyFromCoord(vetInnerBound(innerBoundKey)), SwapLatLong, InvertOrder, EscludiPuntiRipetuti)
            innerBound.Add(c, tmpInnerBoundStr)
            c = c + 1
        Next
        If vetInnerBound Is Nothing OrElse vetInnerBound.Keys.Count = 0 Then
            innerBound.Add(c, "")
        End If

        'If vetInnerBound.Length > 0 Then
        '    innerBound = get_coordinates(wktPolyFromCoord(vetInnerBound), SwapLatLong, InvertOrder, EscludiPuntiRipetuti)
        'Else
        '    innerBound = ""
        'End If


    End Sub

    Public Function wktPolyFromCoord(ByVal coordPoly As String()) As String

        Dim rval As String = ""
        Dim cCoord As String
        Dim lRval As New List(Of String)

        For i As Integer = 0 To coordPoly.Length - 2 Step 2

            cCoord = coordPoly(i) & " " & coordPoly(i + 1)
            lRval.Add(cCoord)
        Next

        Return "POLYGON ((" & String.Join(", ", lRval.ToArray) & "))"

    End Function


    Public Function get_coordinates(ByVal wkt As String, ByVal SwapLatLong As Boolean, ByVal InvertOrder As Boolean, ByVal EscludiPuntiRipetuti As Boolean) As String

        Dim isPoly As Boolean = (wkt.ToLower.StartsWith("polygon"))

        Dim splitIdx As Integer = 1
        If wkt.Contains("((") Then
            splitIdx = 2
        End If


        'esempi: POINT (10 40)
        'esempi: POLYGON ((0 0, 0 10, 10 0, 10 10))

        Dim ArrayOfCoordinates() As String = wkt.Split("(")(splitIdx).Split(")")(0).Split(",")


        Dim rval As String = ""

        Dim firstCoord As String = ""
        Dim lastCoord As String = ""

        Dim a, b As Int16
        If SwapLatLong Then
            a = 1
            b = 0
        Else
            a = 0
            b = 1
        End If

        For Each coordinata As String In ArrayOfCoordinates


            coordinata = coordinata.TrimEnd(" ")
            coordinata = coordinata.TrimStart(" ")

            If firstCoord = "" Then
                firstCoord = coordinata
            End If


            Dim switchcoord() As String = coordinata.Split(" ")
            If Not EscludiPuntiRipetuti OrElse (EscludiPuntiRipetuti AndAlso Not (rval.Contains(switchcoord(a) & " " & switchcoord(b) & "|"))) Then
                If Not ConfrontaVettoreCoordintate(rval.TrimEnd("|").Split("|"), New xyz With {.X = xyz.myCDBL(switchcoord(0)), .Y = xyz.myCDBL(switchcoord(1))}) Then
                    rval &= switchcoord(a) & " " & switchcoord(b) & "|"
                    lastCoord = coordinata
                End If

            End If


        Next

        If isPoly AndAlso firstCoord <> lastCoord Then
            Dim switchcoord() As String = firstCoord.Split(" ")
            rval &= switchcoord(a) & " " & switchcoord(b) & " "

        End If

        rval = rval.Replace("|", " ").TrimEnd(" ")

        If InvertOrder Then
            rval = PolygonOrder.InvertiPoligono(rval, False)
        End If

        Return rval
    End Function



End Class
