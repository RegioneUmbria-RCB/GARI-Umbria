
Imports <xmlns="http://www.agronica.it/grafica/">

Imports Newtonsoft.Json

Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaGIS2012.Commons
Imports AgronicaCoreVarieBIZ

Public Class AgronicaGis2012ToJohnDeere


    ''' <summary>
    ''' Partendo dal formato xml Agronica Genera una stringa JSON in formato John Deere "Boundary"
    ''' </summary>
    ''' <param name="xmlToConvert"></param>
    ''' <returns></returns>
    Public Function convert(ByVal xmlToConvert As String) As RispostaStandard

        Dim Xdoc As XDocument = XDocument.Parse(xmlToConvert)
        Dim estraiCoordinate As New GML

        Dim rval As New RispostaStandard
        Dim rval1 As New jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)
        rval1.values = New List(Of jDeereDataModel_Boundary)

        Try

            Dim tot As Integer = 0

            For Each CurrEnt As XElement In (
            From n In Xdoc.<DatiEntita>.<Entita>
            Select n)

                tot += 1

                Dim coord As New List(Of xyz)
                coord = estraiCoordinate.EstraiCoordinateDaPoligono(CurrEnt.ToString, True)
                Dim coordCount As Integer = coord.Count

                Dim bnd As New jDeereDataModel_Boundary
                bnd.name = AgronicaCoreUtility.AgroGISHelper.GetValueFromGeoDes("Name", CurrEnt.Attribute("text"))
                bnd.modifiedTime = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(Now, DateTimeKind.Local)
                bnd.irrigated = False
                bnd.multipolygons = New List(Of jDeereDataModel_Polygon)
                bnd.active = True
                bnd.sourceType = "External"
                Dim areaFromAttr As String =
                    CurrEnt.<geodata>.<DatiCalcolati>.<geodata_Area>.Value.Replace(".", AgronicaCoreUtility.CulturaHelper.SeparatoreDecimaleVB)
                bnd.area = New jDeereDataModel_MeasurementAsDouble With {.unit = "ha", .valueAsDouble = areaFromAttr}
                bnd.workableArea = New jDeereDataModel_MeasurementAsDouble With {.unit = "ha", .valueAsDouble = areaFromAttr}

                bnd.id = "AGR-" & CurrEnt.<EntitaGIAS>.<DatoGias>.<Entita_Cod>.Value()

                Dim p As New jDeereDataModel_Polygon
                p.rings = New List(Of jDeereDataModel_Ring)

                Dim ring As New jDeereDataModel_Ring
                ring.points = New List(Of jDeereDataModel_Point)

                For Each c In coord

                    Dim pp As New jDeereDataModel_Point
                    pp.lat = c.Y
                    pp.lon = c.X
                    ring.points.Add(pp)

                Next
                'coordinata

                p.rings.Add(ring)

                bnd.multipolygons.Add(p)
                rval1.values.Add(bnd)

            Next
            'entità gias/gis

            rval1.total = tot

            rval.RispostaOK = True
            rval.RispostaStringa = JsonConvert.SerializeObject(rval1)

        Catch ex As Exception

            rval.RispostaOK = False
            rval.RispostaStringa = Nothing
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function


End Class
