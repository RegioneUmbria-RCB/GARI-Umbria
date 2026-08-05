
Imports System.Xml.XPath

Imports AgronicaGIS2012.Commons
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports <xmlns="http://www.opengis.net/kml/2.2">
Imports System.Xml
Imports System.IO

Public Class KML


    Public ListaOggetti As List(Of KmlShape)

    Public Sub Load(ByVal xmlToLoad As String)

        ListaOggetti = New List(Of KmlShape)

        Try



            Dim listaOggettiGrafici As IEnumerable

            Dim sXmlToLoad As String =
                My.Computer.FileSystem.ReadAllText(xmlToLoad)

            Dim reader As XmlReader = XmlReader.Create(New StringReader(sXmlToLoad))
            Dim tmpDoc As XDocument = XDocument.Load(reader)
            Dim nameTable As XmlNameTable = reader.NameTable
            Dim namespaceManager As XmlNamespaceManager = New XmlNamespaceManager(nameTable)
            namespaceManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2")

            listaOggettiGrafici = CType(tmpDoc.XPathEvaluate("//kml:Placemark", namespaceManager), IEnumerable)

            ''aggiungo anche l'oggetto diretto, fuori dalle "folders"
            'Dim singoloOggetto As XElement =
            '    xD.<kml>.<Document>.<Placemark>.FirstOrDefault

            'If Not singoloOggetto Is Nothing Then
            '    listaOggettiGrafici.Add(singoloOggetto)
            'End If

            For Each OggettoGrafico As XElement In listaOggettiGrafici

                Dim xShape As New KmlShape
                xShape.Name = OggettoGrafico.<name>.Value

                'Dim objGis As XElement = OggettoGrafico.<Polygon>
                Dim objGis = OggettoGrafico.<Polygon>

                If objGis IsNot Nothing Then

                    'bordi esterni
                    Dim xOb As String = objGis.<outerBoundaryIs>.<LinearRing>.<coordinates>.Value

                    'bordi interni
                    Dim xIb As String = objGis.<outerBoundaryIs>.<LinearRing>.<coordinates>.Value

                    xShape.OggettoGraficoOuterBoundary = xOb
                    xShape.OggettoGraficoInnerBoundary = xIb

                    xShape.Tipo = Enum_kmlShape_tipo.Poligono

                End If

                objGis = OggettoGrafico.<Point>
                If Not objGis Is Nothing Then

                End If

                objGis = OggettoGrafico.<Linestring>
                If Not objGis Is Nothing Then

                End If

                ListaOggetti.Add(xShape)
            Next

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Throw New Exception("[kml] : " & MessaggioErrore, ex)

        End Try



    End Sub

End Class


Public Class KmlShape

    Private _oggettoGraficoOuterBoundary As String
    Private _oggettoGraficoInnerBoundary As String
    Public Name As String
    Public Tipo As Enum_kmlShape_tipo

    Public Property OggettoGraficoOuterBoundary As String
        Get
            Return _oggettoGraficoOuterBoundary
        End Get
        Set(value As String)
            _oggettoGraficoOuterBoundary = value
        End Set
    End Property

    Public Property OggettoGraficoInnerBoundary As String
        Get
            Return _oggettoGraficoInnerBoundary
        End Get
        Set(value As String)
            _oggettoGraficoInnerBoundary = value
        End Set
    End Property

    Public ReadOnly Property OggettoGraficoOuterBoundaryXYZ As List(Of xyz)

        Get
            'Dim wktHelp As New GML
            'Dim poligono As String = wktHelp.CreaPoligonoDaCoordinate(_oggettoGraficoOuterBoundary);
            'Return wktHelp.EstraiCoordinateDaPoligono(poligono, False)
            Dim punti As New List(Of xyz)
            If Not String.IsNullOrEmpty(_oggettoGraficoOuterBoundary) Then
                Dim strCorr As String = _oggettoGraficoOuterBoundary.Trim().Replace(vbCrLf, "|"c).Replace(vbLf, "|"c).Replace(" ", "|"c)

                Dim list = strCorr.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)

                For Each coord1 In list
                    If coord1.Contains(",") Then
                        Dim coordinateLette As String() = coord1.Split(",")
                        punti.Add(New xyz With {
                            .X = xyz.myCDBL(coordinateLette(1).Trim()),
                            .Y = xyz.myCDBL(coordinateLette(0).Trim())
                        })
                    End If
                Next
                'separato da pipe

                'For idx = 0 To list.Count - 1 Step 3
                '    punti.Add(New xyz With {
                '        .X = xyz.myCDBL(list(idx + 1).Trim()),
                '        .Y = xyz.myCDBL(list(idx).Trim())
                '    })
                'Next


            End If
            Return punti
        End Get

    End Property

    Public ReadOnly Property OggettoGraficoInnerBoundaryXYZ As List(Of xyz)

        Get
            Dim wktHelp As New GML
            Return wktHelp.EstraiCoordinateDaPoligono(
                wktHelp.CreaPoligonoDaCoordinate(_oggettoGraficoInnerBoundary),
                False
            )

        End Get

    End Property

End Class

Public Enum Enum_kmlShape_tipo
    Punto = 0
    Linestring = 1
    Poligono = 2
End Enum

