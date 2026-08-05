Imports System.IO
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider
Imports AgronicaGIS2012.Commons
Imports Newtonsoft.Json.Linq
Imports OSGeo.GDAL

Public Class Raster_ToAgronicaGIS2012
    Implements xxx_toAgronicaGIS2012

    Public Function convert(configurazioneImportazione As ConfigurazioneImportazione,
                            objParametri_Server As AgronicaCoreParametri,
                            objParametri_Utenti As AgronicaCoreParametri) As String Implements xxx_toAgronicaGIS2012.convert

        Dim agroSeq As New Agro_Sequenze
        Dim res As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Dim geoJson As String
            Dim geoDesc As String

            EstraiGeoInfoDaFileRaster(configurazioneImportazione, geoJson, geoDesc)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                FlagTransazioneLocale,
                                                                                objParametri_Server)

            Dim newAllegatoCod As Int32

            Dim ScriviAllegato As New AgronicaCoreGisDAL.GIS_Allegati_W

            res = ScriviAllegato.Scrivi(configurazioneImportazione.Piva,
                                        configurazioneImportazione.DescrizioneFile,
                                        Path.GetFileName(configurazioneImportazione.ShapeFileFullFileName),
                                        Path.GetExtension(configurazioneImportazione.ShapeFileFullFileName),
                                        configurazioneImportazione.DataInizioValidita,
                                        configurazioneImportazione.DataFineValidita,
                                        newAllegatoCod,
                                        objParametri_Server,
                                        objParametri_Utenti.UsernameOperazione,
                                        objParametri_Utenti.UsernameOperazione)

            If Not res Then
                Throw New Exception(String.Format("Errore nella scrittura dell'allegato {0}.", Path.GetFileName(configurazioneImportazione.ShapeFileFullFileName)))
            End If

            Dim ScriviEntita As New AgronicaCoreGisDAL.GIS_Entita_W

            Dim newEntitaCod As Int32 = agroSeq.NuovoId_Tabella("GIS_Entita", 0, Int32.MaxValue, objParametri_Server, True)

            res = ScriviEntita.Scrivi(
                                        objParametri_Utenti.PivaSuperUser,
                                        newEntitaCod,
                                        CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.RASTER),
                                        configurazioneImportazione.Piva,
                                        configurazioneImportazione.SaCod,
                                        configurazioneImportazione.Appezza,
                                        configurazioneImportazione.CampoCod,
                                        configurazioneImportazione.RegImpianto,
                                        "",
                                        "",
                                        "",
                                        0,
                                        0,
                                        "",
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        "",
                                        0,
                                        configurazioneImportazione.DataInizioValidita,
                                        configurazioneImportazione.DataFineValidita,
                                        objParametri_Server,
                                        Now,
                                        Now,
                                        objParametri_Utenti.UsernameOperazione,
                                        objParametri_Utenti.UsernameOperazione,
                                        newAllegatoCod,
                                        0,
                                        Path.GetFileNameWithoutExtension(configurazioneImportazione.ShapeFileFullFileName)
                                    )

            If Not res Then
                Throw New Exception("Errore nel salvataggio dell'Entità GIS")
            End If

            Dim ScriviElementoGrafico As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

            Dim newElementoGraficoCod As Int32 = agroSeq.NuovoId_Tabella("GIS_ElementiGrafici", 0, Int32.MaxValue, objParametri_Server, True)

            Dim gmlConverter As New wkt_gml

            Dim xDoc = XDocument.Parse(gmlConverter.Trasforma(geoJson, True, False, False, newElementoGraficoCod))

            res = ScriviElementoGrafico.Scrivi(objParametri_Utenti.PivaSuperUser,
                                               newElementoGraficoCod,
                                               geoDesc,
                                               newEntitaCod,
                                               configurazioneImportazione.LayerCod,
                                               xDoc.Descendants.Skip(1).FirstOrDefault.ToString,
                                               0,
                                               "",
                                               configurazioneImportazione.Piva,
                                               configurazioneImportazione.SaCod,
                                               configurazioneImportazione.DataInizioValidita,
                                               configurazioneImportazione.DataFineValidita,
                                               objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nel salvataggio dell'Elemento grafico.")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return "OK"

    End Function

    Private Sub EstraiGeoInfoDaFileRaster(ByVal conf As ConfigurazioneImportazione, ByRef geoJson As String, ByRef geoDesc As String)

        AgronicaSHPWrapper.GdalConfiguration.ConfigureGdal()
        Dim DS = Gdal.Open(conf.ShapeFileFullFileName, Access.GA_ReadOnly)

        Dim GdalInfo = Gdal.GDALInfo(DS, New GDALInfoOptions({"-json"}))

        DS.Dispose()

        Dim objInfo = JObject.Parse(GdalInfo)

        Dim coordArray = objInfo("wgs84Extent")("coordinates")

        Dim wktCoord As New List(Of xyz)

        For Each objCoord In coordArray(0)
            Dim coord = New xyz With {.X = objCoord(0), .Y = objCoord(1)}
            wktCoord.Add(coord)
        Next

        geoJson = (New WKT).CreaWKT(wktCoord, "POLYGON")

        Try
            Dim wktString = objInfo("coordinateSystem")("wkt").ToString

            Dim coordinateSystem As String = ""

            If objInfo.SelectToken("stac.proj:projjson.name") IsNot Nothing Then
                coordinateSystem = objInfo.SelectToken("stac.proj:projjson.name").ToString
            Else
                Dim coordinateSystemToken As String = wktString.Substring(0, wktString.IndexOf("[") + 1)

                coordinateSystem = wktString.Split(",")(0).Replace("""", "").Replace(coordinateSystemToken, "")
            End If

            Dim EPSG As String = ""

            If objInfo.SelectToken("stac.proj:epsg") IsNot Nothing Then
                EPSG = String.Format("EPSG: {0}", objInfo.SelectToken("stac.proj:epsg").ToString)
            Else
                EPSG = wktString.Substring(wktString.LastIndexOf("AUTHORITY[") + 10).Replace("""", "").Replace("]", "").Replace(",", ": ")
            End If

            coordinateSystem = String.Format("{0} - {1}", coordinateSystem, EPSG)

            Dim pixelSize As String

            If conf.PixelSize = 0 Then
                pixelSize = objInfo("geoTransform")(1).ToString
            Else
                pixelSize = conf.PixelSize
            End If

            Dim bands = objInfo("bands").Count.ToString

            Dim bandDes As String = ""

            For Each band In objInfo("bands")
                If band("description") Is Nothing Then
                    bandDes = String.Format("{0}, unnamed", bandDes)
                Else
                    bandDes = String.Format("{0}, {1}", bandDes, band("description").ToString.Replace("-", " "))
                End If

                If band("noDataValue") Is Nothing Then
                    bandDes = String.Format("{0} - noDataValue: ", bandDes)
                Else
                    bandDes = String.Format("{0} - noDataValue: {1}", bandDes, band("noDataValue").ToString)
                End If
            Next

            geoDesc = String.Format("Description§ {0}|Coordinate system§ {1}|Pixel size (m)§ {2}|N_Bands§ {3}|Bands Descriptions§ {4}|Validita Inizio§ {5}|Validita Fine§ {6}",
                                     conf.DescrizioneFile,
                                     coordinateSystem,
                                     pixelSize,
                                     bands,
                                     bandDes.Substring(2),
                                     conf.DataInizioValidita.ToString("dd/MM/yyyy"),
                                     conf.DataFineValidita.ToString("dd/MM/yyyy"))
        Catch ex As Exception
            geoDesc = ""
        End Try


    End Sub
End Class
