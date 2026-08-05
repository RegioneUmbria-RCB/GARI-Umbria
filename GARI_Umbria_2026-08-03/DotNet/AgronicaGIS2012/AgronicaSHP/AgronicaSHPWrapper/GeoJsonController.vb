Imports System.Text
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports Newtonsoft.Json
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.Gis

Public Class GeoJsonController(Of T)

    Private ns As XNamespace = "http://www.opengis.net/gml"


    ''' <summary>
    ''' Deserializza da stringa json
    ''' </summary>
    ''' <param name="GeoJsonStr"></param>
    ''' <returns></returns>
    Public Function Load(GeoJsonStr As String) As GeoJson_Shape(Of T)

        Dim geoJsonCaricato As GeoJson_Shape(Of T)

        If GeoJsonStr.Substring(1, 50).Replace(vbCrLf, "").Replace(vbLf, "").Replace(vbCr, "").Replace(" ", "").StartsWith("""type"":""FeatureCollection"",""features"":[") Then
            geoJsonCaricato = JsonConvert.DeserializeObject(Of GeoJson_Shape(Of T))(GeoJsonStr)
        Else

            geoJsonCaricato = New GeoJson_Shape(Of T)()
            geoJsonCaricato.type = "FeatureCollection"
            geoJsonCaricato.features = New List(Of GeoJson_Feature(Of T))
            geoJsonCaricato.features.Add(JsonConvert.DeserializeObject(Of GeoJson_Feature(Of T))(GeoJsonStr))

        End If

        Return geoJsonCaricato

    End Function

    ''' <summary>
    ''' Restituisce il formato WKT corrispondente al GeoJson caricato
    ''' </summary>
    ''' <returns></returns>
    Public Function STAsText(ByVal GeoJson As GeoJson_Geometry) As String


        Dim wkt As New StringBuilder
        wkt.Append(GeoJson.type.ToUpper)

        Dim coordinatesStr As String = JsonConvert.SerializeObject(GeoJson.coordinates)
        Dim xCoord As String = coordinatesStr.Replace(vbCrLf, "")

        Select Case GeoJson.type
            Case GeoJson_GeometryType.Polygon


                Dim vCoord As String() = xCoord.Split("[")



                'poligono senza innerbound
                wkt.Append(" ((")
                For i = 3 To vCoord.Length - 1
                    Dim coord As String = vCoord(i)

                    'trovato break fra outer ed inner bound ...:
                    If coord = "" Then
                        wkt.Append("),(")
                    Else
                        If i = vCoord.Length - 1 OrElse coord.Contains("]]") Then
                            wkt.Append(coord.Replace("]", "").Replace(" ", "").Replace(",", " ").TrimEnd(" "))
                        Else
                            wkt.Append(coord.Replace("],", "|").Replace(",", " ").Replace("|", ","))
                        End If
                    End If



                Next

                wkt.Append("))")

            Case Else
                Throw New Exception("NON GESTITO")
        End Select

        Return wkt.ToString

    End Function

    Public Function ObjJsonFromWKT(wktString As String) As GeoJson_Geometry


        Dim wktHelper As New WKT
        Dim coords As List(Of AgronicaGIS2012.Commons.xyz) = wktHelper.CreaCoordinateDaWkt(wktString)

        Dim stb As New StringBuilder
        Dim l1 As New List(Of String)
        stb.Append("[[")
        For Each coord In coords
            Dim stb1 As New StringBuilder
            stb1.Append("[")
            stb1.Append(coord.X.ToString.Replace(",", "."))
            stb1.Append(", ")
            stb1.Append(coord.Y.ToString.Replace(",", "."))
            stb1.Append("]")
            l1.Add(stb1.ToString)
        Next
        stb.Append(String.Join(",", l1))
        stb.Append("]]")

        Dim geom As New GeoJson_Geometry

        If coords.Count = 2 Then
            geom.type = GeoJson_GeometryType.Point
        Else
            geom.type = GeoJson_GeometryType.Polygon
        End If

        geom.coordinates = JsonConvert.DeserializeObject(Of Double()()())(stb.ToString)
        Return geom


    End Function

    Public Function ObjJsonFromGML(elemento As XElement) As GeoJson_Geometry



        Dim str As String() =
            GML.DammiArrayCoordinateDatoXml(ns, elemento)

        Dim stb As New StringBuilder
        Dim l1 As New List(Of String)
        stb.Append("[[")
        For i As Integer = 0 To str.Length - 2 Step 2
            Dim stb1 As New StringBuilder
            stb1.Append("[")
            stb1.Append(str(i + 1))
            stb1.Append(", ")
            stb1.Append(str(i))
            stb1.Append("]")
            l1.Add(stb1.ToString)
        Next
        stb.Append(String.Join(",", l1))
        stb.Append("]]")

        Dim geom As New GeoJson_Geometry

        If str.Count = 2 Then
            geom.type = GeoJson_GeometryType.Point
        Else
            geom.type = GeoJson_GeometryType.Polygon
        End If

        geom.coordinates = JsonConvert.DeserializeObject(Of Double()()())(stb.ToString)
        Return geom

    End Function

    Public Function ObjJsonFromGML2(elemento As XElement) As GeoJson_Geometry_New



        Dim str As String() =
            GML.DammiArrayCoordinateDatoXml(ns, elemento)

        Dim stb As New StringBuilder
        Dim l1 As New List(Of String)
        stb.Append("[[")
        For i As Integer = 0 To str.Length - 2 Step 2
            Dim stb1 As New StringBuilder
            stb1.Append("[")
            stb1.Append(str(i + 1))
            stb1.Append(", ")
            stb1.Append(str(i))
            stb1.Append("]")
            l1.Add(stb1.ToString)
        Next
        stb.Append(String.Join(",", l1))
        stb.Append("]]")

        Dim geom As New GeoJson_Geometry_New

        If str.Count = 2 Then
            geom = New GeoJson_Geometry_New(FeatureType.Point, stb.ToString)
        Else
            geom = New GeoJson_Geometry_New(FeatureType.Polygon, stb.ToString)
        End If

        Return geom

    End Function

    Public Function StringJsonFromGML(elemento As XElement) As String

        Dim str As String() =
            GML.DammiArrayCoordinateDatoXml(ns, elemento.<geodata>.FirstOrDefault)

        Dim strVertici As New StringBuilder
        Dim i As Integer

        Dim lContaVertici As Integer = 4
        If str.Count <= 4 Then
            lContaVertici = 1
        End If

        For i = 0 To str.Count - lContaVertici
            If strVertici.Length > 0 Then
                strVertici.Append(",{")
            Else
                strVertici.Append("{")
            End If
            strVertici.Append("""lat"":""" & str(i) & """,")
            i = i + 1
            strVertici.Append("""long"":""" & str(i) & """")

            strVertici.Append("}")
        Next

        Return strVertici.ToString

    End Function

End Class
