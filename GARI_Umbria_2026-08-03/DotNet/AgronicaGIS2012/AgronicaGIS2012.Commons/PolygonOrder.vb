Public Class PolygonOrder
    Public Shared Function InvertiLatLong(ByVal poligono As String, ByVal EscludiPuntiRipetuti As Boolean) As String
        Return InvertiPoligono(poligono, EscludiPuntiRipetuti, True)
    End Function

    Public Shared Function InvertiPoligono(ByVal poligono As String, ByVal EscludiPuntiRipetuti As Boolean, Optional ByVal invertiLatLong As Boolean = False) As String

        poligono = poligono.TrimStart(" ")
        Dim isGml As Boolean = poligono.StartsWith("<")

        Dim coordExterior As New List(Of String)
        Dim coordInterior As New List(Of String)
        If isGml Then
            If poligono.Contains("<gml:Polygon") Then
                coordExterior = coordinatesFromGML(poligono, "exterior")
                coordInterior = coordinatesFromGML(poligono, "interior")
            Else
                coordExterior = coordinatesFromGML(poligono, "")
            End If

        Else
            coordExterior.Add(poligono)
        End If

        Dim rval As String
        If invertiLatLong Then
            rval = invertiLatLongp(coordExterior.FirstOrDefault)
        Else
            rval = reorder(EscludiPuntiRipetuti, coordExterior.FirstOrDefault)
        End If

        If isGml Then

            Dim outputPattern As XElement
            If poligono.StartsWith("<gml:LineString") Then
                outputPattern = <gml:LineString xmlns:gml="http://www.opengis.net/gml">
                                    <gml:posList><%= rval.TrimEnd(" ") %></gml:posList>
                                </gml:LineString>
            Else


                outputPattern = <gml:Polygon xmlns:gml="http://www.opengis.net/gml">
                                <gml:exterior>
                                    <gml:LinearRing>
                                        <gml:posList><%= rval.TrimEnd(" ") %></gml:posList>
                                    </gml:LinearRing>
                                </gml:exterior>
                            </gml:Polygon>

                For Each sCoordInterior As String In coordInterior
                    Dim rInterior As XElement = <gml:interior xmlns:gml="http://www.opengis.net/gml">
                                                    <gml:LinearRing>
                                                        <gml:posList><%= sCoordInterior.TrimEnd(" ") %></gml:posList>
                                                    </gml:LinearRing>
                                                </gml:interior>

                    Dim ns As XNamespace = "http://www.opengis.net/gml"
                    outputPattern.Add(rInterior)

                Next
            End If
            rval = outputPattern.ToString
        End If

        Return rval.TrimEnd(" ")

    End Function

    Public Shared Function InvertiPoligono_wkt(ByVal poligono As String, ByVal EscludiPuntiRipetuti As Boolean, Optional ByVal invertiLatLong As Boolean = False) As String

        poligono = poligono.TrimStart(" ")

        Dim rval As String

        rval = poligono.Replace("MULTIPOLYGON", "")
        rval = rval.Replace("POLYGON", "")
        rval = rval.Replace("(", "")
        rval = rval.Replace(")", "")
        rval = rval.Replace(")", "")
        If invertiLatLong Then
            rval = invertiLatLongp(rval)
        Else
            rval = reorderWKT(rval)
        End If
        rval = rval.TrimEnd(" ")

        rval = "POLYGON ((" & rval & "))"

        Return rval.TrimEnd(" ")

    End Function

    Private Shared Function invertiLatLongp(ByVal coordPl As String) As String
        Dim switch() As String = coordPl.TrimStart(" ").TrimEnd(" ").Split(" ")

        Dim rval As String = ""

        For i As Integer = 0 To switch.Length - 2 Step 2
            rval &= switch(i + 1) & " " & switch(i) & " "
        Next

        Return rval

    End Function

    Private Shared Function reorder(ByVal EscludiPuntiRipetuti As Boolean, ByVal coordPl As String) As String
        Dim rval As String = ""
        Dim switch() As String = coordPl.TrimStart(" ").TrimEnd(" ").Split(" ")

        For i As Integer = switch.Length - 2 To 0 Step -2
            If Not EscludiPuntiRipetuti OrElse i = 0 OrElse (EscludiPuntiRipetuti AndAlso Not (rval.Contains(switch(i) & " " & switch(i + 1) & " "))) Then
                rval &= switch(i) & " " & switch(i + 1) & " "
            End If

        Next
        Return rval
    End Function

    Private Shared Function reorderWKT(ByVal coordPl As String) As String
        '12.266827113926411 44.168096153894091,12.267282083630562 44.168132227852411,12.267483919858932 44.165802285828555,12.266999781131744 44.16577005848724,12.266827113926411 44.168096153894091

        Dim rval As String = ""
        Dim switch() As String = coordPl.TrimStart(" ").TrimEnd(" ").Split(",")

        Dim FinalList As New List(Of String)

        For i As Integer = switch.Length - 1 To 0 Step -1
            If switch(i) <> "" Then
                FinalList.Add(switch(i))
            End If
        Next

        rval = String.Join(",", FinalList)

        Return rval
    End Function

    Private Shared Function coordinatesFromGML(ByVal gml As String, ByVal elemento As String) As List(Of String)

        Dim rval As New List(Of String)

        Dim ElementoGml As String = "Polygon"
        If elemento = "" Then
            ElementoGml = "LineString"
        End If

        ' Mengarda 2026-03-02 #207311
        ' Change namespace because the former is considered invalid
        Dim xdoc As XDocument = XDocument.Parse(gml)
        Dim ns As XNamespace = "http://www.opengis.net/gml"
        Dim lPolygonPart As List(Of XElement)

        ' Mengarda 2026-03-02 #207311
        ' use .Descendants instead of .Elements for flexibility 
        If elemento = "" Then
            lPolygonPart = xdoc.Descendants(ns + ElementoGml).ToList
        Else
            lPolygonPart = xdoc.Descendants(ns + ElementoGml).Descendants(ns + elemento).ToList
        End If

        If lPolygonPart Is Nothing OrElse lPolygonPart.Count = 0 Then
            Return New List(Of String)
        End If

        For Each n In lPolygonPart
            If elemento = "" Then
                rval.Add(n.Elements(ns + "posList").Value)
            Else
                rval.Add(n.Elements(ns + "LinearRing").Elements(ns + "posList").Value)
            End If

        Next

        Return rval

    End Function

End Class
