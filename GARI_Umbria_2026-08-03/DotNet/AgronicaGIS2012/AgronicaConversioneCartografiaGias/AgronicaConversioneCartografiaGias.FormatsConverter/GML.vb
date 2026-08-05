Imports <xmlns="http://www.agronica.it/grafica/">

Imports AgronicaGIS2012.Commons
Imports System.Xml.XPath
Imports System.Xml

Public Class GML

    ''' <summary>
    ''' recupera le coordinate dal primo elemento grafico
    ''' </summary>
    ''' <param name="sElemento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DammiArrayCoordinateDatoXml(ByVal sElemento As String) As String()

        Dim ns As XNamespace = "http://www.opengis.net/gml"

        Dim xDataGML As XDocument = XDocument.Parse(sElemento)


        Return DammiArrayCoordinateDatoXml(ns, xDataGML.Elements("DatiEntita").Elements("Entita").Elements("geodata").FirstOrDefault)

    End Function

    Public Shared Function DammiArrayCoordinateDatoXml(ByVal ns As XNamespace, ByVal elemento As XElement) As String()

        Dim str As String() = GetStr(ns, elemento, elemento)

        Return str
    End Function

    Private Shared Function GetStr(ByVal ns As XNamespace, ByVal elemento As XElement, ByVal EstraiDati As XElement) As String()
        Dim str As String() = Nothing
        Select Case CType(elemento.DescendantNodes.FirstOrDefault, XElement).Name
            Case ns + "Polygon"
                str = EstraiDati.Elements(ns + "Polygon").Elements(ns + "exterior").Elements(ns + "LinearRing").Elements(ns + "posList").Value.Split(" ")
            Case ns + "Point"
                str = EstraiDati.Elements(ns + "Point").Elements(ns + "pos").Value.Split(" ")
            Case ns + "LineString"
                str = EstraiDati.Elements(ns + "LineString").Elements(ns + "posList").Value.Split(" ")
            Case ns + "MultiSurface"
                str = EstraiDati.Elements(ns + "MultiSurface").Elements(ns + "surfaceMembers").Elements(ns + "Polygon").Elements(ns + "exterior").Elements(ns + "LinearRing").Elements(ns + "posList").Value.Split(" ")

        End Select
        Return str
    End Function
    Public Function EstraiCoordinateDaPoligono(ByVal Entita As String, ByVal InvertiXY As Boolean) As List(Of xyz)

        Dim rval As New List(Of xyz)

        Dim oDoc As XDocument = XDocument.Parse(Entita)
        Dim ns As XNamespace = "http://www.opengis.net/gml"

        Dim namespaceManager As New XmlNamespaceManager(New NameTable())
        namespaceManager.AddNamespace("empty", "http://www.agronica.it/grafica/")

        Dim list As String = CType(oDoc.XPathSelectElement("//empty:geodata", namespaceManager).FirstNode, XElement).Value
        Dim punti As String() = list.Split(" ")

        Dim a, b As UInt16
        If InvertiXY Then
            a = 1
            b = 0
        Else
            a = 0
            b = 1
        End If

        For i As Integer = 0 To punti.Length - 1 Step 2
            rval.Add(New xyz With { _
                      .X = xyz.myCDBL(punti(i + a)), _
                      .Y = xyz.myCDBL(punti(i + b)) _
                  })

        Next

        Return rval
    End Function

    Public Function EstraiCoordinateDaGML(ByVal gml As String, ByVal InvertiXY As Boolean, Exterior As Boolean) As List(Of xyz)

        Dim rval As New List(Of xyz)

        Dim oDoc As XDocument = XDocument.Parse(gml)
        Dim ns As XNamespace = "http://www.opengis.net/gml"


        Dim LeggiDa As String = "exterior"
        If Not Exterior Then
            LeggiDa = "interior"
        End If
        Dim list As String 
        
        ' Mengarda 2026-03-02 #207311
        ' oDoc.Elements(ns + "Polygon").Elements(ns + LeggiDa).Value was not enough to get the coordinates 
        ' of the polygon. If this does not work we use the former approach
        list = oDoc.Descendants(ns + "Polygon").Descendants(ns + LeggiDa).Descendants(ns + "posList").Value
        If list Is Nothing Then
            list = oDoc.Elements(ns + "Polygon").Elements(ns + LeggiDa).Value
        End If

        Dim punti As String() = list.Split(" ")

        Dim a, b As UInt16
        If InvertiXY Then
            a = 1
            b = 0
        Else
            a = 0
            b = 1
        End If

        For i As Integer = 0 To punti.Length - 1 Step 2
            rval.Add(New xyz With {
                      .X = xyz.myCDBL(punti(i + a)),
                      .Y = xyz.myCDBL(punti(i + b))
                  })

        Next

        Return rval
    End Function

    Public Function CreaPoligonoDaCoordinate(ByVal Coordinate As String) As String
        Dim oDoc As XDocument

        'Coordinate = Coordinate.TrimEnd() & vbCrLf

        'Dim outputPattern As XElement = <gml:Polygon xmlns:gml="http://www.opengis.net/gml">
        '                                    <gml:exterior>
        '                                        <gml:LinearRing>
        '                                            <gml:posList>
        '                                                <%= Coordinate %>
        '                                            </gml:posList>
        '                                        </gml:LinearRing>
        '                                    </gml:exterior>
        '                                </gml:Polygon>

        'oDoc = ReadXmlFromString("<geodata></geodata>")

        'oDoc.Add(outputPattern)


        oDoc = ReadXmlFromString("<geodata>" & vbCrLf &
                                 "  <gml:Polygon xmlns:gml=""http://www.opengis.net/gml"">" & vbCrLf &
                                 "      <gml:exterior>" & vbCrLf &
                                 "          <gml:LinearRing>" & vbCrLf &
                                 "              <gml:posList>" & vbCrLf &
                                 "                  " & vbCrLf &
                                 "              </gml:posList>" & vbCrLf &
                                 "          </gml:LinearRing>" & vbCrLf &
                                 "      </gml:exterior>" & vbCrLf &
                                 "  </gml:Polygon>" & vbCrLf &
                                 "</geodata>")

        Return RemoveNamespace(oDoc).ToString

    End Function

    Public Sub AggiungiInterior(ByRef oDoc As XElement, ByVal Coordinate As String)

        Dim interior As XElement = _
            <gml:interior xmlns:gml="http://www.opengis.net/gml">
                <gml:LinearRing>
                    <gml:posList><%= Coordinate %></gml:posList>
                </gml:LinearRing>
            </gml:interior>


        Dim ns As XNamespace = "http://www.opengis.net/gml"
        oDoc.<geodata>.Elements(ns + "Polygon").FirstOrDefault.Add(interior)


    End Sub

    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function



#Region "Utility xml"
    Private Function RemoveNamespace(xdoc As XDocument) As XDocument


        For Each e As XElement In xdoc.Root.DescendantsAndSelf()
            If e.Name.[Namespace] <> XNamespace.None Then
                e.Name = XNamespace.None.GetName(e.Name.LocalName)
            End If
            If e.Attributes().Where(Function(a) a.IsNamespaceDeclaration OrElse a.Name.[Namespace] <> XNamespace.None).Any() Then
                e.ReplaceAttributes(e.Attributes().[Select](Function(a) If(a.IsNamespaceDeclaration, Nothing, If(a.Name.[Namespace] <> XNamespace.None, New XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value), a))))
            End If
        Next
        Return xdoc
    End Function
#End Region

End Class
