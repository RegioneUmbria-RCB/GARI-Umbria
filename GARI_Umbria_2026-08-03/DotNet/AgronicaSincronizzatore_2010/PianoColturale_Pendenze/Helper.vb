Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Notifiche
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD.anagrafiche
Imports NetTopologySuite.Geometries
Imports ProjNet.CoordinateSystems
Imports ProjNet.CoordinateSystems.Transformations

Public Class Helper
    Implements IDisposable

    'Private Const UTWGS_84_UTM_zone_32N As String = "PROJCS[""WGS_1984_UTM_Zone_32N"",GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]],PROJECTION[""Transverse_Mercator""],PARAMETER[""False_Easting"",500000.0],PARAMETER[""False_Northing"",0.0],PARAMETER[""Central_Meridian"",9.0],PARAMETER[""Scale_Factor"",0.9996],PARAMETER[""Latitude_Of_Origin"",0.0],UNIT[""Meter"",1.0]]"
    'Private Const UTWGS_84_UTM_zone_33N As String = "PROJCS[""WGS_1984_UTM_Zone_33N"",GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]],PROJECTION[""Transverse_Mercator""],PARAMETER[""False_Easting"",500000.0],PARAMETER[""False_Northing"",0.0],PARAMETER[""Central_Meridian"",15.0],PARAMETER[""Scale_Factor"",0.9996],PARAMETER[""Latitude_Of_Origin"",0.0],UNIT[""Meter"",1.0]]"
    Private Const WGS_84 As String = "GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]"
    'Private Const Monte_Mario_Italy_zone_2 As String = "PROJCS[""Monte_Mario_Italy_2"",GEOGCS[""GCS_Monte_Mario"",DATUM[""D_Monte_Mario"",SPHEROID[""International_1924"",6378388.0,297.0]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]],PROJECTION[""Transverse_Mercator""],PARAMETER[""False_Easting"",2520000.0],PARAMETER[""False_Northing"",0.0],PARAMETER[""Central_Meridian"",15.0],PARAMETER[""Scale_Factor"",0.9996],PARAMETER[""Latitude_Of_Origin"",0.0],UNIT[""Meter"",1.0]]"
    Private Const MonteMario_Italy_Zone_1_3003 As String = "PROJCS[""Monte Mario / Italy zone 1"",GEOGCS[""Monte Mario"",DATUM[""Monte_Mario"",SPHEROID[""International 1924"", 6378388, 297],TOWGS84[-104.1, -49.1, -9.9, 0.971, -2.917, 0.714, -11.68]],PRIMEM[""Greenwich"", 0,AUTHORITY[""EPSG"", ""8901""]],UNIT[""degree"", 0.0174532925199433,AUTHORITY[""EPSG"", ""9122""]],AUTHORITY[""EPSG"", ""4265""]],PROJECTION[""Transverse_Mercator""],PARAMETER[""latitude_of_origin"", 0],PARAMETER[""central_meridian"", 9],PARAMETER[""scale_factor"", 0.9996],PARAMETER[""false_easting"", 1500000],PARAMETER[""false_northing"", 0],UNIT[""metre"", 1,AUTHORITY[""EPSG"", ""9001""]],AXIS[""Easting"", EAST],AXIS[""Northing"", NORTH],AUTHORITY[""EPSG"", ""3003""]]"
    Private Const MonteMario_Italy_Zone_2_3004 As String = "PROJCS[""Monte Mario / Italy zone 2"",GEOGCS[""Monte Mario"",DATUM[""Monte_Mario"",SPHEROID[""International 1924"", 6378388, 297],TOWGS84[-104.1, -49.1, -9.9, 0.971, -2.917, 0.714, -11.68]],PRIMEM[""Greenwich"", 0,AUTHORITY[""EPSG"", ""8901""]],UNIT[""degree"", 0.0174532925199433,AUTHORITY[""EPSG"", ""9122""]],AUTHORITY[""EPSG"", ""4265""]],PROJECTION[""Transverse_Mercator""],PARAMETER[""latitude_of_origin"", 0],PARAMETER[""central_meridian"", 15],PARAMETER[""scale_factor"", 0.9996],PARAMETER[""false_easting"", 2520000],PARAMETER[""false_northing"", 0],UNIT[""metre"", 1,AUTHORITY[""EPSG"", ""9001""]],AXIS[""Easting"", EAST],AXIS[""Northing"", NORTH],AUTHORITY[""EPSG"", ""3004""]]"

    Public Sub New()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub

    Public Shared Function MapPendenzaParticellaToCatasto(ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                          ByVal PROV As String,
                                                          ByVal COM As String,
                                                          ByVal numeroParticella As Integer,
                                                          ByRef pendenzaParticellaDTO As AgronicaCoreDTOStd.InData.DataExchange.PendenzaParticella,
                                                          ByRef _logger As LoggerManager) As AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto

        Dim ret As AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto = Nothing
        Try
            Dim areaParticella As Double = Math.Round(pendenzaParticellaDTO.areaParticella, 0) / 10000

            Dim zonizzazioni As New List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliZona)
            zonizzazioni.Add(New ParticelleCatastaliZona With {
                                .zona = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr With {
                                    .codice = If(pendenzaParticellaDTO.pendenzaPct > 10,
                                    TipiEnumerativi.enum_Zone.B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc,
                                    TipiEnumerativi.enum_Zone.A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE)
                                },
                                .Area = 0,
                                .validita = New IntervalloTemporale With {
                                    .inizio = AGRODATAINIZIO,
                                    .fine = AGRODATAFINE
                                }
                            })
            Dim possessi As New List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella)
            possessi.Add(New AgronicaCoreModelsSTD.anagrafiche.PossessoParticella() With {
                                            .titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(1),
                                            .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                            .Area = areaParticella
                                            })
            Dim newValue = New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale() With {
                                    .centro = CentroPK,
                                    .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali() With {
                                        .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(
                                            PROV,
                                            COM,
                                            "0",
                                            pendenzaParticellaDTO.foglio,
                                            numeroParticella,
                                            If(String.IsNullOrEmpty(pendenzaParticellaDTO.subalterno.Trim()), "0", pendenzaParticellaDTO.subalterno)),
                                        .Area = areaParticella,
                                        .zonizzazione = zonizzazioni
                                    },
                                    .possessiParticella = possessi
                                }
            ret = New AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto(Nothing, newValue)

        Catch ex As Exception
            If Not IsNothing(_logger) Then
                _logger.AppendLog(LogLevel.ERRORI, ex.Message, LogType.Errore, ex)
            End If
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try

        Return ret
    End Function

    Public Shared Function MapParticellaToCatasto(ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                  ByVal PROV As String,
                                                  ByVal COM As String,
                                                  ByVal Sezione As String,
                                                  ByVal Foglio As String,
                                                  ByVal numeroParticella As Integer,
                                                  ByVal Subalterno As String,
                                                  ByVal areaParticella As Double,
                                                  ByVal classePendenza As String,
                                                  ByRef _logger As LoggerManager) As AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto

        Dim ret As AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto = Nothing
        Try
            Dim zonizzazioni As New List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliZona)

            zonizzazioni.Add(New ParticelleCatastaliZona With {
                                .zona = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr With {
                                    .codice = IIf(classePendenza = "A", TipiEnumerativi.enum_Zone.A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE, TipiEnumerativi.enum_Zone.B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc)
                                },
                                .Area = 0,
                                .validita = New IntervalloTemporale With {
                                    .inizio = AGRODATAINIZIO,
                                    .fine = AGRODATAFINE
                                }
                            })
            Dim possessi As New List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella)
            possessi.Add(New AgronicaCoreModelsSTD.anagrafiche.PossessoParticella() With {
                                            .titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(1),
                                            .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                            .Area = areaParticella / 10000
                                            })
            Dim newValue = New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale() With {
                                    .centro = CentroPK,
                                    .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali() With {
                                        .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(
                                            PROV,
                                            COM,
                                            Sezione,
                                            Foglio,
                                            numeroParticella,
                                            Subalterno),
                                        .Area = areaParticella / 10000,
                                        .zonizzazione = zonizzazioni
                                    },
                                    .possessiParticella = possessi
                                }
            ret = New AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto(Nothing, newValue)

        Catch ex As Exception
            If Not IsNothing(_logger) Then
                _logger.AppendLog(LogLevel.ERRORI, ex.Message, LogType.Errore, ex)
            End If
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try

        Return ret
    End Function

    Public Shared Function GetCartography(ByVal oriWKT As String,
                                          ByVal srOri As Integer,
                                          ByVal srDest As Integer) As String


        Dim xTest As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim xPolygonHistoryW As New AgronicaCoreVarieBIZ.DataPublish_Poligoni_NonValidi_W
        Dim newWKTString As String = oriWKT
        Dim geoCheckStr = ""
        Try
            Dim poligonoCorretto As Boolean = False
            Dim esitoOrientamento As Boolean = False

            Dim ntsGeoInstance = New NetTopologySuite.NtsGeometryServices()

            Dim oriGeoFact = ntsGeoInstance.CreateGeometryFactory(srOri)
            Dim destGeoFact = ntsGeoInstance.CreateGeometryFactory(srDest)


            Dim r As New NetTopologySuite.IO.WKTReader(ntsGeoInstance)
            Dim geocheck As Geometry = r.Read(oriWKT)

            geoCheckStr = geocheck.GeometryType.ToLower

            Select Case geocheck.GeometryType.ToLower
                Case "polygon"
                    Dim geom As Polygon = geocheck

                    Dim poly = destGeoFact.CreatePolygon(GetTransformCoordinatesVectors(geom.ExteriorRing.Coordinates, srOri, srDest))

                    newWKTString = poly.ToText()

                Case Else
                    Throw New InvalidPendenzaGeometryException("Geometria [" + geocheck.GeometryType + "] non consentita per lettura pendenza")
            End Select

        Catch ex As InvalidPendenzaGeometryException
            newWKTString = ""
            Throw ex
        Catch ex As Exception
            newWKTString = ""
            Throw New Exception(ex.Message, ex)
        End Try
        Return newWKTString
    End Function

    Public Shared Function getAreeParticella(ByVal wktAppezzamento As String,
                                             ByVal wktParticella As String,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             ByRef _logger As LoggerManager) As CalcoloAreeParticelleGIS


        Dim xr As New AgronicaCoreGisDAL.GIS_OperazioniCartograficheDB
        Dim xt As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

        Dim ntsGeoInstance = New NetTopologySuite.NtsGeometryServices()
        Dim ret As CalcoloAreeParticelleGIS = Nothing

        Try
            Dim r As New NetTopologySuite.IO.WKTReader(ntsGeoInstance)

            Dim polyAppezza As Polygon = r.Read(wktAppezzamento)
            Dim polyParticella As Polygon = r.Read(wktParticella)


            'Dim polyAppezza = destGeoFact.CreatePolygon(GetTransformCoordinatesVectors(wrkAppezzaGeom.ExteriorRing.Coordinates, 4326, 3857))
            'Dim polyParticella = destGeoFact.CreatePolygon(GetTransformCoordinatesVectors(wrkParticellaGeom.ExteriorRing.Coordinates, 4326, 3857))

            'Lavez - 03/11/2025 - nel caso in cui la particella sia un multipolygon prendo il primo poligono
            'Dim intersezione As Geometry = polyAppezza.Intersection(polyParticella)

            Dim intersezione As Geometry = polyAppezza.Intersection(polyParticella.GetGeometryN(0))

            'Lavez - 03/11/2025 - nel caso in cui la particella abbia dei buchi li tolgo dall'intersezione
            If polyParticella.InteriorRings.Count() > 0 Then
                For Each hole In polyParticella.InteriorRings
                    intersezione = intersezione.Difference(hole)
                Next
            End If

            '_logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("wktIntersect: {0}", intersezione.ToText()), LogType.Informazione)

            If intersezione.GeometryType.ToLower() = "multipolygon" Then
                Dim multi As MultiPolygon = intersezione
                intersezione = multi.GetGeometryN(0)
            End If

            Dim wktInterset As String = intersezione.Buffer(0).ToText()

            Dim esitoOrientamento = xt.TestaPoligonoWKTValid(wktInterset, objParametriServer, False)

            If Not esitoOrientamento Then
                wktInterset = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono_wkt(wktInterset, False, False)
                esitoOrientamento = xt.TestaPoligonoWKTValid(wktInterset, objParametriServer, False)
            End If
            If Not esitoOrientamento Then
                _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("errore in calcolo Intersezione particella\appezzamento" & vbCrLf & "wktIntersect: {0}", wktInterset), LogType.Errore)
                Throw New Exception("errore in calcolo Intersezione particella\appezzamento ")
            End If

            Dim areaIntersezione As Double = xr.OttieniAreaDaWKT(wktInterset, objParametriServer)
            Dim areaParticella As Double = xr.OttieniAreaDaWKT(wktParticella, objParametriServer)

            ret = New CalcoloAreeParticelleGIS(areaIntersezione / 10000, areaParticella / 10000)


        Catch ex As Exception
            _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("errore in calcolo Aree particella\appezzamento: {0}", ex.Message), LogType.Errore)
            ret = New CalcoloAreeParticelleGIS(-1, -1)
        End Try

        Return ret

    End Function

    Private Shared Function GetTransformCoordinatesVectors(ByVal coordinates As NetTopologySuite.Geometries.Coordinate(),
                                                           ByVal srOri As Integer,
                                                           ByVal srDest As Integer
                                                           ) As NetTopologySuite.Geometries.Coordinate()
        Dim ret As New List(Of NetTopologySuite.Geometries.Coordinate)
        Dim trf = New CoordinateTransformationFactory()

        Try
            Dim tr = trf.CreateFromCoordinateSystems(GetCoordinateSystemFromEPSG(srOri),
                                        GetCoordinateSystemFromEPSG(srDest))

            Dim transformedPoint = tr.MathTransform.TransformList(ConvCoordinateToPointList(coordinates))

            For Each point In transformedPoint
                ret.Add(New NetTopologySuite.Geometries.Coordinate With {
                            .X = point.X,
                            .Y = point.Y
                        })
            Next

        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret.ToArray()

    End Function

    Private Shared Function GetCoordinateSystemFromEPSG(ByVal id_code As Integer) As GeoAPI.CoordinateSystems.ICoordinateSystem
        Dim csfactory = New CoordinateSystemFactory()

        Select Case id_code
            Case 4326
                Return csfactory.CreateFromWkt(WGS_84)
            'Case 32632
            '    Return csfactory.CreateFromWkt(UTWGS_84_UTM_zone_32N)
            'Case 32633
            '    Return csfactory.CreateFromWkt(UTWGS_84_UTM_zone_33N)
            Case 3003
                Return csfactory.CreateFromWkt(MonteMario_Italy_Zone_1_3003)
            Case 3004
                Return csfactory.CreateFromWkt(MonteMario_Italy_Zone_2_3004)
            Case Else
                Throw New Exception("EPSG Code non mappato")
        End Select
    End Function

    Private Shared Function ConvCoordinateToPointList(ByVal coordinates As NetTopologySuite.Geometries.Coordinate()) As List(Of GeoAPI.Geometries.Coordinate)
        Dim ret As New List(Of GeoAPI.Geometries.Coordinate)
        For Each point In coordinates
            ret.Add(New GeoAPI.Geometries.Coordinate(point.X, point.Y))
        Next
        Return ret
    End Function

    Public Shared Function RemoveWktDecimals(ByVal wktInput As String) As String

        Dim index1 = 0
        Dim index2 = 0
        Dim c1, c2 As Char
        Dim strLen = wktInput.Length

        While index1 < strLen
            c1 = wktInput(index1)
            If c1 = "."c Then
                index2 = index1
                c2 = wktInput(index2)

                While c1 <> "."c AndAlso index2 < strLen
                    If c2 = "," OrElse c2 = " "c OrElse c2 = ")"c Then
                        wktInput = wktInput.Remove(index1, index2 - index1)
                        strLen = wktInput.Length
                        Exit While
                    End If

                    index2 += 1
                    c2 = wktInput(index2)
                End While
            End If

            index1 += 1
        End While

        Return wktInput
    End Function

End Class
