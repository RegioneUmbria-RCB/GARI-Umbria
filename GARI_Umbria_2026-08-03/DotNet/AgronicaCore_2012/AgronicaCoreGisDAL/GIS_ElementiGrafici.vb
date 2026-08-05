Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaGIS2012.Commons




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_ElementiGrafici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiGML(ByVal PivaSuperUser As String,
                             ByVal ElementoGrafico_Cod As Int32,
                             ByVal Entita_Cod As Int32,
                             ByVal LayerElementiGrafici_Cod As Int32,
                             ByVal FormatoGraficoConvertito As enumFromatoCartograficoConvertito,
                             ByVal xSelezioneVariabile As enumSelezioneVariabile,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As String

        Dim NomeRoutine As String = "AgronicaCoreGISDal.GIS_ElementiGrafici_R.LeggiGML()"

        Dim DT As String
        Dim MessaggioErrore As String = ""

        Try
            Dim StrSQL As New System.Text.StringBuilder

            Leggi_creaSQL(PivaSuperUser,
                          ElementoGrafico_Cod,
                          Entita_Cod,
                          LayerElementiGrafici_Cod,
                          FormatoGraficoConvertito,
                          xSelezioneVariabile,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri,
                          StrSQL,
                          True,
                          0,
                          "*")

            DT = EseguiQuery_Lettura_XML(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function calcolaIntersezioniTraLayer(ByVal layerElementiGrafici_Cod As Integer,
                                                ByVal struct_cod As Int32,
                                                ByVal geoData As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.calcolaIntersezioniTraLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	XYZ.ElementoGrafico_Des, LEGADS.LayerElementiGrafici_Etichetta ")
            StrSQL.AppendLine(" 	, ISNULL(XYZ.Intersezione.STAsText(),'') as Intersezione ")
            StrSQL.AppendLine(" 	, ROUND(((XYZ.AreaIntersezione_Ha / XYZ.AreaTotaleEntitaDelCiclo_Ha) * 100), 2) as PercentualeIntersezione ")
            StrSQL.AppendLine(" FROM (select ")

            StrSQL.AppendLine(String.Format("        g.ElementoGrafico_Des, {0} as Layer_cod ", layerElementiGrafici_Cod.ToString))
            StrSQL.AppendLine(String.Format("        , g.Poligono_GeoEntity.STIntersection({0}) as Intersezione ", Agro_SQL_SaveGeograpyFromWKTString(geoData)))
            StrSQL.AppendLine(String.Format("        , g.Poligono_GeoEntity.STIntersection({0}).STArea() / 100000 as AreaIntersezione_Ha ", Agro_SQL_SaveGeograpyFromWKTString(geoData)))
            StrSQL.AppendLine(String.Format("        , {0}.STArea() / 100000 as AreaTotaleEntitaDelCiclo_Ha ", Agro_SQL_SaveGeograpyFromWKTString(geoData)))

            StrSQL.AppendLine(" 		from GIS_ElementiGrafici g ")

            StrSQL.AppendLine(String.Format(" 		where g.LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" 			   and g.Poligono_GeoEntity.STIntersects({0}) = 1 ", Agro_SQL_SaveGeograpyFromWKTString(geoData)))

            StrSQL.AppendLine(" ) XYZ ")
            StrSQL.AppendLine(" INNER Join GIS_LayerElementiGrafici_Anagrafica_DataStruct LEGADS ")
            StrSQL.AppendLine(" ON XYZ.Layer_cod = LEGADS.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(String.Format(" AND LEGADS.TipologiaLayer_struct_cod = {0} ", Agro_SQL_SaveNum(struct_cod)))

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    ''' <summary>
    ''' This method calculates the projection of <paramref name="layerElementiGraficiCod1"/> over <paramref name="layerElementiGraficiCod2"/>.
    ''' Layers could be filtered by Company by passing not empty <paramref name="pivaLayer1"/> for <paramref name="layerElementiGraficiCod1"/>
    ''' and not empty <paramref name="pivaLayer2"/> for <paramref name="layerElementiGraficiCod2"/>.
    ''' <paramref name="numPolyLabel"/> Represents the name to assign to the number of polygons columns,
    ''' <paramref name="intersectionAreaLabel"/> Represents the name to assigna to the intersection surface column, expressed in [Ha]
    ''' </summary>
    ''' <param name="layerElementiGraficiCod1"></param>
    ''' <param name="layerElementiGraficiCod2"></param>
    ''' <param name="pivaLayer1"></param>
    ''' <param name="pivaLayer2"></param>
    ''' <param name="numPolyLabel"></param>
    ''' <param name="intersectionAreaLabel"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>
    ''' A <c>DataTable</c> containing the surface of <paramref name="layerElementiGraficiCod1"/> overlapping <paramref name="layerElementiGraficiCod2"/>,
    ''' the number of <paramref name="layerElementiGraficiCod1"/> instances intersecting each <paramref name="layerElementiGraficiCod2"/> instance,
    ''' and the number of <paramref name="layerElementiGraficiCod1"/> distinct Companies wich instances intersect each <paramref name="layerElementiGraficiCod2"/> instance
    ''' </returns>
    Public Function CalculateIntersectionsOnLayers(ByVal layerElementiGraficiCod1 As Int32,
                                                   ByVal layerElementiGraficiCod2 As Int32,
                                                   ByVal pivaLayer1 As String,
                                                   ByVal pivaLayer2 As String,
                                                   ByVal numPolyLabel As String,
                                                   ByVal intersectionAreaLabel As String,
                                                   ByVal validityYearLayer1 As Integer,
                                                   ByVal validityYearLayer2 As Integer,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.CalculateIntersectionsOnLayers()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try
            StrSQL.Length = 0
            ' selection of layer1 entities
            StrSQL.AppendLine("WITH layer1 AS (")
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        GIS_ElementiGrafici.ElementoGrafico_Cod")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.ElementoGrafico_Des")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.ElementoGrafico_GUID")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.Entita_Cod")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.LayerElementiGrafici_Cod")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.Poligono_GeoEntity")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.Poligono_GeoEntity_WKT")
            StrSQL.AppendLine("        , GIS_Entita.Piva")
            StrSQL.AppendLine("        , Imprese.rag_soc")
            StrSQL.AppendLine("    FROM GIS_ElementiGrafici (NOLOCK)")
            StrSQL.AppendLine("        INNER JOIN GIS_Entita (NOLOCK) ON GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod")

            If pivaLayer1 IsNot Nothing AndAlso pivaLayer1 <> "" Then
                StrSQL.AppendLine(String.Format("            AND GIS_Entita.Piva = '{0}'", pivaLayer1))
            End If

            If Not IsNothing(validityYearLayer1) AndAlso validityYearLayer1 > 1900 AndAlso validityYearLayer1 < 2100 Then
                Me.AppendEntityValidityFilter(layerElementiGraficiCod1, validityYearLayer1, StrSQL)
            End If

            StrSQL.AppendLine("        LEFT JOIN Imprese (NOLOCK) ON Imprese.PIVA = GIS_Entita.Piva")
            StrSQL.AppendLine("    WHERE 1 = 1")
            StrSQL.AppendLine(String.Format("        AND LayerElementiGrafici_Cod = {0}", Agro_SQL_SaveNum(layerElementiGraficiCod1)))
            StrSQL.AppendLine("),")

            ' selection of layer2 entities
            StrSQL.AppendLine("layer2 AS (")
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        GIS_ElementiGrafici.ElementoGrafico_Cod")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.ElementoGrafico_Des")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.ElementoGrafico_GUID")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.Entita_Cod")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.LayerElementiGrafici_Cod")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.Poligono_GeoEntity")
            StrSQL.AppendLine("        , GIS_ElementiGrafici.Poligono_GeoEntity_WKT")
            StrSQL.AppendLine("        , GIS_Entita.Piva")
            StrSQL.AppendLine("        , Imprese.rag_soc")
            StrSQL.AppendLine("    FROM GIS_ElementiGrafici (NOLOCK)")
            StrSQL.AppendLine("        INNER JOIN GIS_Entita (NOLOCK) ON GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod")

            If pivaLayer2 IsNot Nothing AndAlso pivaLayer2 <> "" Then
                StrSQL.AppendLine(String.Format("            AND GIS_Entita.Piva = '{0}'", pivaLayer2))
            End If

            If Not IsNothing(validityYearLayer2) AndAlso validityYearLayer2 > 1900 AndAlso validityYearLayer2 < 2100 Then
                Me.AppendEntityValidityFilter(layerElementiGraficiCod2, validityYearLayer2, StrSQL)
            End If

            StrSQL.AppendLine("        LEFT JOIN Imprese (NOLOCK) ON Imprese.PIVA = GIS_Entita.Piva")
            StrSQL.AppendLine("    WHERE 1 = 1")
            StrSQL.AppendLine(String.Format("        AND LayerElementiGrafici_Cod = {0}", Agro_SQL_SaveNum(layerElementiGraficiCod2)))
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("")

            ' overlapping of layer1 over layer2
            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    result.ElementoGrafico_Des")
            StrSQL.AppendLine("    , result.Entita_Cod")
            StrSQL.AppendLine("    , result. ElementoGrafico_Cod")
            StrSQL.AppendLine(String.Format("    , CEILING(SUM(COALESCE(result.intersection_area_ha, 0))) AS [{0}]", intersectionAreaLabel))
            StrSQL.AppendLine(String.Format("    , COUNT(DISTINCT result.piva_layer1) AS {0}", "num_imprese"))
            StrSQL.AppendLine(String.Format("    , COUNT(result.impianti_entita_cod) AS [{0}]", numPolyLabel))
            StrSQL.AppendLine("FROM (")
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        layer2.*")
            StrSQL.AppendLine("        , layer2.Poligono_GeoEntity.STIntersection(layer1.Poligono_GeoEntity) AS intersection")
            StrSQL.AppendLine("        , layer2.Poligono_GeoEntity.STIntersection(layer1.Poligono_GeoEntity).STArea() AS intersection_area")
            StrSQL.AppendLine("        , layer2.Poligono_GeoEntity.STIntersection(layer1.Poligono_GeoEntity).STArea() / 100000 AS intersection_area_ha")
            StrSQL.AppendLine("        , layer1.Piva AS piva_layer1")
            StrSQL.AppendLine("        , layer1.rag_soc AS rag_soc_layer1")
            StrSQL.AppendLine("        , layer1.Entita_Cod AS impianti_entita_cod")
            StrSQL.AppendLine("    FROM layer2")
            StrSQL.AppendLine("        LEFT JOIN layer1 ON layer2.Poligono_GeoEntity.STIntersects(layer1.Poligono_GeoEntity) = 1")
            StrSQL.AppendLine(") AS result")
            StrSQL.AppendLine("GROUP BY ")
            StrSQL.AppendLine("    result.ElementoGrafico_Des")
            StrSQL.AppendLine("    , result.Entita_Cod")
            StrSQL.AppendLine("    , result. ElementoGrafico_Cod")
            StrSQL.AppendLine("")

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    ''' <summary>
    ''' This sub appends entity validity filter, based on a given year, to the query, it must be used in JOIN statement.
    ''' </summary>
    ''' <param name="layerCod"></param>
    ''' <param name="validityYear"></param>
    ''' <param name="strSQL"></param>
    Private Sub AppendEntityValidityFilter(ByVal layerCod As Int32,
                                           ByVal validityYear As Integer,
                                           ByRef strSQL As System.Text.StringBuilder)
        ' pay attention: this sub requires that Gis_Entita is already in JOIN in the query

        Select Case layerCod
            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                strSQL.AppendLine("        INNER JOIN Reg_Impianti ON Reg_impianti.PIVA = Gis_Entita.Piva")
                strSQL.AppendLine("            AND Reg_impianti.APPEZZA = Gis_Entita.Appezza")
                strSQL.AppendLine("            AND Reg_impianti.SA_COD = Gis_Entita.sa_cod")
                strSQL.AppendLine("            AND Reg_impianti.ID_REG = Gis_Entita.Id_Imp")
                strSQL.AppendLine(String.Format("            AND YEAR(Reg_Impianti.Validita_Inizio) <= {0}", validityYear))
                strSQL.AppendLine(String.Format("            AND YEAR(Reg_Impianti.Validita_Fine) >= {0}", validityYear))
            Case Else
                strSQL.AppendLine(String.Format("            AND YEAR(GIS_Entita.Validita_Inizio) <= {0}", validityYear))
                strSQL.AppendLine(String.Format("            AND YEAR(GIS_Entita.Validita_Fine) >= {0}", validityYear))
        End Select
    End Sub

    Public Function LeggiInfoRaster(ByVal layerElementiGrafici_Cod As Int32,
                                    ByVal poligonoWKT As String,
                                    ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiInfoRaster()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" DECLARE @GEO as geography ")
            StrSQL.AppendLine(String.Format(" SET @GEO = {0} ", Agro_SQL_SaveGeograpyFromWKTString(poligonoWKT)))

            StrSQL.AppendLine(" SELECT distinct E.Entita_Cod ")
            StrSQL.AppendLine(" , E.Entita_GUID ")
            StrSQL.AppendLine(" , AD.Allegati_Documenti_NomeFile ")
            StrSQL.AppendLine(" , EG.ElementoGrafico_Des ")
            StrSQL.AppendLine(" FROM GIS_LayerElementiGrafici LEG ")
            StrSQL.AppendLine(" INNER JOIN GIS_ElementiGrafici EG ")
            StrSQL.AppendLine("     ON EG.LayerElementiGrafici_Cod = LEG.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" INNER JOIN GIS_Entita E ")
            StrSQL.AppendLine("     ON E.Entita_Cod = EG.Entita_Cod ")
            StrSQL.AppendLine(" INNER JOIN GIS_Allegati_Documenti AD ")
            StrSQL.AppendLine("     ON AD.Allegati_Documenti_Cod = E.GIS_Allegati_Documenti_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LEG.LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(" AND @GEO.STIntersects(EG.Poligono_GeoEntity) = 1 ")

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiIntersezioneConLayerRasterDaGUID(ByVal guidEntita1 As String,
                                                          ByVal layerElementiGrafici_Cod As Integer,
                                                          ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.calcolaIntersezioniTraLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" DECLARE @GEO as geography ")
            StrSQL.AppendLine(" SET @GEO = (SELECT EGI.Poligono_GeoEntity ")
            StrSQL.AppendLine("             FROM GIS_Entita EI ")
            StrSQL.AppendLine("             INNER JOIN GIS_ElementiGrafici EGI on EGI.Entita_Cod = EI.Entita_Cod ")
            StrSQL.AppendLine(String.Format(" WHERE EI.Entita_GUID = '{0}') ", Agro_SQL_SaveText(guidEntita1)))

            StrSQL.AppendLine(" SELECT E.Entita_Cod ")
            StrSQL.AppendLine(" , ISNULL(E.ParametriVisualizzazioneLayer, '') As ParametriVisualizzazioneLayer ")
            StrSQL.AppendLine(" FROM GIS_Entita E ")
            StrSQL.AppendLine(" INNER JOIN GIS_ElementiGrafici EG on EG.Entita_Cod = E.Entita_Cod ")

            StrSQL.AppendLine(String.Format(" WHERE EG.LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))

            StrSQL.AppendLine(" AND @GEO.STIntersects(EG.Poligono_GeoEntity) = 1 ")

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDescrizioneElementoDaCodiceEntita(entita_Cod As Integer, objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiElementoGraficoDaCodiceEntita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try
            StrSQL.Length = 0

            '20231116 - Michele Furno: USARE COALESCE, ISNULL effettua un cast implicito al formato del primo parametri troncando
            ' il risultato di Poligono_GeoEntity.STAsText() a 2000 caratteri, ovvero il limite della colonna Poligono_GeoEntity_WKT

            StrSQL.AppendLine(" Select ISNULL(EG.ElementoGrafico_Des, '') As ElementoGrafico_Des ")
            'StrSQL.AppendLine(" , EG.Poligono_GeoEntity.STAsText() as GeoData ")
            StrSQL.AppendLine(" , COALESCE(EG.Poligono_GeoEntity_WKT, EG.Poligono_GeoEntity.STAsText()) as GeoData ")
            StrSQL.AppendLine(" , ISNULL(LEGA.PaletteVisualizzazione, '') as PaletteVisualizzazione ")
            StrSQL.AppendLine(" From GIS_ElementiGrafici EG ")
            StrSQL.AppendLine(" INNER JOIN GIS_LayerElementiGrafici_Anagrafica LEGA ON LEGA.LayerElementiGrafici_Cod = EG.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(String.Format(" WHERE EG.Entita_Cod = {0} ", Agro_SQL_SaveNum(entita_Cod)))

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiElementoGraficoDaCodiceEntita(ByVal entita_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiElementoGraficoDaCodiceEntita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	g.Poligono_GeoEntity.STAsText() As GeoData ")
            StrSQL.AppendLine(" 	, a.PIVA + ' - ' + imp.rag_soc + ' - ' + ii.sa_nome + ' - ' + a.APP_NOME + ' - ' + ISNULL(veg.veg_des + ' - ' + c.cul_Des, anag.descrizione) + ' - ' + Convert(CHAR(8),i.Validita_Inizio,112) + ' - ' + Convert(CHAR(8),i.Validita_Fine,112) As ElementoGrafico_Des ")
            StrSQL.AppendLine(" FROM GIS_ElementiGrafici g ")
            StrSQL.AppendLine(" 	INNER JOIN GIS_Entita e on e.Entita_Cod = g.Entita_Cod ")
            StrSQL.AppendLine(" 	INNER JOIN Imprese imp ")
            StrSQL.AppendLine(" 		 on imp.piva = e.Piva ")
            StrSQL.AppendLine(" 	INNER JOIN Centri_Aziendali ii ")
            StrSQL.AppendLine(" 		 on ii.piva = e.Piva ")
            StrSQL.AppendLine(" 		 and ii.SA_COD = e.Sa_Cod ")
            StrSQL.AppendLine(" 	INNER JOIN Appezzamento a ")
            StrSQL.AppendLine(" 		 on a.piva = e.Piva ")
            StrSQL.AppendLine(" 		 and a.SA_COD = e.Sa_Cod ")
            StrSQL.AppendLine(" 		 and a.APPEZZA = e.Appezza ")
            StrSQL.AppendLine(" 	INNER JOIN Reg_Impianti i ")
            StrSQL.AppendLine(" 		 on  i.piva = e.Piva ")
            StrSQL.AppendLine(" 		 and i.SA_COD = e.Sa_Cod ")
            StrSQL.AppendLine(" 		 and i.APPEZZA = e.Appezza ")
            StrSQL.AppendLine(" 		 and i.ID_REG = e.Id_Imp ")
            StrSQL.AppendLine(" 	LEFT JOIN Cultivar c ")
            StrSQL.AppendLine(" 		 on c.Cul_Cod = i.CUL_COD ")
            StrSQL.AppendLine(" 	LEFT JOIN SpecieVegetali veg ")
            StrSQL.AppendLine(" 		 on veg.Veg_Cod = c.Veg_Cod ")
            StrSQL.AppendLine(" 	LEFT JOIN Reg_Impianti_Codici cc ")
            StrSQL.AppendLine(" 		 on  i.piva = cc.Piva ")
            StrSQL.AppendLine(" 		 and i.SA_COD = cc.Sa_Cod ")
            StrSQL.AppendLine(" 		 and i.APPEZZA = cc.Appezza ")
            StrSQL.AppendLine(" 		 and i.ID_REG = cc.Id_Reg ")
            StrSQL.AppendLine(" 		 and cc.Progetto_Cod = 0 ")
            StrSQL.AppendLine(" 		 and cc.id_cod between 3000 and 3999 ")
            StrSQL.AppendLine(" 	LEFT JOIN Codici_Anagrafe anag ")
            StrSQL.AppendLine(" 		 on cc.id_cod = anag.codice ")
            StrSQL.AppendLine(String.Format(" WHERE e.Entita_Cod = {0} ", Agro_SQL_SaveNum(entita_Cod)))

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function



    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function LeggiWKT(
                            ByVal entitaCod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiCoordinateOperazioneAgenda()"

        '====================================================================================
        'Parametri opzionali :
        '   ElementoGrafico_Cod = 0
        '   Entita_Cod = 0
        '   LayerElementiGrafici_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT g.poligono_geoEntity.STAsText() AS geo, g.flag_GPS")
            StrSQL.AppendLine(" FROM gis_entita e")
            StrSQL.AppendLine(" INNER JOIN gis_elementigrafici g")
            StrSQL.AppendLine(" ON e.PivaSuperUser = g.PivaSuperUser AND e.entita_Cod = g.entita_Cod")
            StrSQL.AppendLine(" WHERE e.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND e.Entita_cod = " & Agro_SQL_SaveNum(entitaCod))


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiWKTConGUID(
                            ByVal Entita_Cod As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiWKTConGUID()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT g.Entita_Cod, ")
            StrSQL.AppendLine("     coalesce(g.poligono_geoEntity_wkt, g.poligono_geoEntity.STAsText()) AS wkt, ")
            StrSQL.AppendLine("     e.Entita_GUID, ")
            StrSQL.AppendLine("     e.GIS_Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" FROM gis_entita e")
            StrSQL.AppendLine(" INNER JOIN gis_elementigrafici g")
            StrSQL.AppendLine(" ON e.PivaSuperUser = g.PivaSuperUser AND e.entita_Cod = g.entita_Cod")
            StrSQL.AppendLine(" WHERE e.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND e.Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod))

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiWKTDaChiaveImpianto(
                            ByVal piva As String,
                            ByVal sa_cod As Integer,
                            ByVal appezza As Integer,
                            ByVal id_reg As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiCoordinateOperazioneAgenda()"

        '====================================================================================
        'Parametri opzionali :
        '   ElementoGrafico_Cod = 0
        '   Entita_Cod = 0
        '   LayerElementiGrafici_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	g.PivaSuperUser, ")
            StrSQL.AppendLine(" 	g.Entita_Cod, ")
            StrSQL.AppendLine(" 	g.poligono_geoEntity.STAsText() AS wkt ")
            StrSQL.AppendLine(" FROM gis_entita e ")
            StrSQL.AppendLine(" INNER JOIN gis_elementigrafici g ")
            StrSQL.AppendLine(" ON e.PivaSuperUser = g.PivaSuperUser AND e.entita_Cod = g.entita_Cod ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(" 	e.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            If piva <> "" Then
                StrSQL.AppendLine(" and e.Piva='" & Agro_SQL_SaveText(piva) & "' ")
            End If
            If sa_cod <> 0 Then
                StrSQL.AppendLine(" and e.Sa_Cod=" & Agro_SQL_SaveNum(sa_cod) & " ")
            End If
            If appezza <> 0 Then
                StrSQL.AppendLine(" and e.Appezza=" & Agro_SQL_SaveNum(appezza) & " ")
            End If
            If id_reg <> 0 Then
                StrSQL.AppendLine(" and e.Id_Imp=" & Agro_SQL_SaveNum(id_reg) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" AND " & xOrderBy & " ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiWKTDaCampioneAnalisi(ByVal analisi_campione_cod As Integer,
                                              ByVal piva As String,
                                              ByVal sa_cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiWKTDaCampioneAnalisi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	g.PivaSuperUser, ")
            StrSQL.AppendLine(" 	g.Entita_Cod, g.ElementoGrafico_Cod, ")
            StrSQL.AppendLine(" 	g.poligono_geoEntity.STAsText() AS wkt ")
            StrSQL.AppendLine(" FROM gis_entita e ")
            StrSQL.AppendLine(" INNER JOIN gis_elementigrafici g ")
            StrSQL.AppendLine(" ON e.PivaSuperUser = g.PivaSuperUser AND e.entita_Cod = g.entita_Cod ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(" 	e.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If analisi_campione_cod <> 0 Then
                StrSQL.AppendLine(" and e.analisi_campione_cod = " & Agro_SQL_SaveNum(analisi_campione_cod) & " ")
            End If

            If piva <> "" Then
                StrSQL.AppendLine(" and e.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If sa_cod <> 0 Then
                StrSQL.AppendLine(" and e.sa_cod =" & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" AND " & xOrderBy & " ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function LeggiCoordinateOperazioneAgenda(
                            ByVal Piva As String,
                            ByVal id_Agenda As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiCoordinateOperazioneAgenda()"

        '====================================================================================
        'Parametri opzionali :
        '   ElementoGrafico_Cod = 0
        '   Entita_Cod = 0
        '   LayerElementiGrafici_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrRes As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT g.poligono_geoEntity.STAsText() AS geo, g.flag_GPS")
            StrSQL.AppendLine(" FROM gis_entita e WITH(NOLOCK)")
            StrSQL.AppendLine(" INNER JOIN gis_elementigrafici g WITH(NOLOCK)")
            StrSQL.AppendLine(" ON e.PivaSuperUser = g.PivaSuperUser AND e.entita_Cod = g.entita_Cod")
            StrSQL.AppendLine(" WHERE e.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND e.piva = " & Agro_SQL_SaveText_NULL(Piva))
            StrSQL.AppendLine(" AND e.id_agenda = " & Agro_SQL_SaveNum_NULL(id_Agenda))


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Leggi(ByVal PivaSuperUser As String,
                          ByVal ElementoGrafico_Cod As Int32,
                          ByVal Entita_Cod As Int32,
                          ByVal LayerElementiGrafici_Cod As Int32,
                          ByVal FormatoGraficoConvertito As enumFromatoCartograficoConvertito,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal numeroRigheDaLeggere As Integer = 0,
                          Optional ByVal colonneDaLeggere As String = "*"
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   ElementoGrafico_Cod = 0
        '   Entita_Cod = 0
        '   LayerElementiGrafici_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Leggi_creaSQL(PivaSuperUser,
                          ElementoGrafico_Cod,
                          Entita_Cod,
                          LayerElementiGrafici_Cod,
                          FormatoGraficoConvertito,
                          xSelezioneVariabile,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri,
                          StrSQL,
                          False,
                          numeroRigheDaLeggere,
                          colonneDaLeggere)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiElencoEntitaDaLayer(ByVal layer_cod As Int32,
                                             ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiElencoEntitaDaLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT distinct Entita_Cod ")
            StrSQL.AppendLine(" FROM GIS_ElementiGrafici ")

            StrSQL.AppendLine(String.Format(" WHERE LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layer_cod)))

            StrSQL.AppendLine(" AND Validita_Inizio <= GETDATE() ")
            StrSQL.AppendLine(" AND Validita_Fine >= GETDATE() ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Private Sub Leggi_creaSQL(ByVal PivaSuperUser As String,
                              ByVal ElementoGrafico_Cod As Int32,
                              ByVal Entita_Cod As Int32,
                              ByVal LayerElementiGrafici_Cod As Int32,
                              ByVal FormatoGraficoConvertito As enumFromatoCartograficoConvertito,
                              ByVal xSelezioneVariabile As enumSelezioneVariabile,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri,
                              ByRef StrSQL As Text.StringBuilder,
                              ByVal isXML As Boolean,
                              ByVal numeroRigheDaLeggere As Integer,
                              ByVal colonneDaLeggere As String)

        Select Case xSelezioneVariabile

            Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                StrSQL.Length = 0
                StrSQL.AppendLine("SELECT ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, Flag_GPS, LayerElementiGrafici_Cod")
                StrSQL.AppendLine("FROM GIS_ElementiGrafici")
                StrSQL.AppendLine("WHERE 1 = 1")

                If Entita_Cod <> 0 Then
                    StrSQL.AppendLine($"    AND GIS_ElementiGrafici.Entita_Cod = {Agro_SQL_SaveNum(Entita_Cod)}")
                End If

                If LayerElementiGrafici_Cod <> 0 Then
                    StrSQL.AppendLine($"    AND GIS_ElementiGrafici.LayerElementiGrafici_Cod = {Agro_SQL_SaveNum(LayerElementiGrafici_Cod)}")
                End If

                If xFiltroAggiuntivo <> "" Then
                    StrSQL.AppendLine($"    AND {Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)}")
                End If

            Case enumSelezioneVariabile.Selezione_TabellaCompleta

                StrSQL.Length = 0
                StrSQL.AppendLine(" SELECT  ")

                If Not isXML Then

                    If numeroRigheDaLeggere > 0 Then
                        StrSQL.AppendLine(String.Format(" TOP {0} ", numeroRigheDaLeggere))
                    End If

                    StrSQL.AppendLine(String.Format(" {0} ", colonneDaLeggere))

                    If IsDaConvertire(FormatoGraficoConvertito) Then
                        StrSQL.AppendLine(" , ")
                    End If

                End If

                If IsDaConvertireGml(FormatoGraficoConvertito) Then

                    StrSQL.AppendLine(" Poligono_GeoEntity.AsGml() as geodata " & vbCrLf)

                End If

                If IsDaConvertireWkt(FormatoGraficoConvertito) Then

                    StrSQL.AppendLine(" Poligono_GeoEntity.STAsText() as geodata " & vbCrLf)

                End If

                StrSQL.AppendLine(" FROM    GIS_ElementiGrafici Entita ")

                StrSQL.AppendLine(" WHERE   Entita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                StrSQL.AppendLine(" AND     Entita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                StrSQL.AppendLine(" AND Entita.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

                If ElementoGrafico_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Entita.ElementoGrafico_Cod = " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & " ")
                End If

                If Entita_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Entita.Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & " ")
                End If

                If LayerElementiGrafici_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Entita.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
                End If

                If xFiltroAggiuntivo <> "" Then
                    StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                End If

                '--------------------------------------------------------------------------
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        StrSQL.AppendLine(" AND   Entita.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        StrSQL.AppendLine(" AND   Entita.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select
                '--------------------------------------------------------------------------

                If xOrderBy <> "" Then
                    StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                End If

                If isXML Then
                    StrSQL.AppendLine(" for xml auto, root('DatiEntita'), elements")
                End If

            Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                '
                '
                '

            Case enumSelezioneVariabile.Selezione_JoinCompleta
                '
                '
                '

        End Select

    End Sub

    Private Function IsDaConvertire(ByVal FormatoGraficoConvertito As enumFromatoCartograficoConvertito) As Boolean
        Return FormatoGraficoConvertito <> enumFromatoCartograficoConvertito.Nessuna_Conversione
    End Function

    Private Function IsDaConvertireGml(ByVal FormatoGraficoConvertito As enumFromatoCartograficoConvertito) As Boolean
        Return FormatoGraficoConvertito = enumFromatoCartograficoConvertito.GML Or
               FormatoGraficoConvertito = enumFromatoCartograficoConvertito.GML_e_WKT
    End Function

    Private Function IsDaConvertireWkt(ByVal FormatoGraficoConvertito As enumFromatoCartograficoConvertito) As Boolean
        Return FormatoGraficoConvertito = enumFromatoCartograficoConvertito.WKT Or
               FormatoGraficoConvertito = enumFromatoCartograficoConvertito.GML_e_WKT
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Polygon1"></param>
    ''' <param name="Polygon2"></param>    
    ''' <param name="objparametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Poligono_CalcolaDistanza(ByVal Polygon1 As String, ByVal Polygon2 As String, ByVal wkt_1_OrGML_2 As String, ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Double

        Dim xRisp As String = ""
        Dim MessaggioErrore As String

        Try

            Dim stb As New System.Text.StringBuilder

            Dim lFunzOut As String = "STAsText()"




            stb.Append("declare @g1 geography " & vbCrLf)
            stb.Append("declare @g2 geography " & vbCrLf)
            stb.Append("  " & vbCrLf)
            If wkt_1_OrGML_2 = 1 Then
                stb.Append(" set @g1 = " & Agro_SQL_SaveGeograpyFromWKTString(Polygon1) & vbCrLf)
                stb.Append(" set @g2 = " & Agro_SQL_SaveGeograpyFromWKTString(Polygon2) & vbCrLf)
            Else
                stb.Append(" set @g1 = " & Agro_SQL_SaveGeograpyFromGMLString(Polygon1) & vbCrLf)
                stb.Append(" set @g2 = " & Agro_SQL_SaveGeograpyFromGMLString(Polygon2) & vbCrLf)
            End If

            stb.Append("  " & vbCrLf)
            stb.Append(" select @g1.STDistance(@g2) as Distanza " & vbCrLf)


            Dim dt As DataTable =
            EseguiQuery_Lettura(objparametri, stb.ToString, "")

            If dt.Rows.Count > 0 Then
                xRisp = dt.Rows(0)("Distanza")
            End If

            Return xRisp

        Catch ex As Exception

            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objparametri, "", MessaggioErrore)
            xRisp = -1


        End Try

        Return xRisp
    End Function


    Public Function Poligono_GetEnvelope(ByVal Entita_cod As String, ByVal wkt_1_OrGML_2 As String, ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim xRisp As String = ""
        Dim MessaggioErrore As String

        Try

            Dim stb As New System.Text.StringBuilder

            Dim lFunzOut As String = "STAsText()"

            If wkt_1_OrGML_2 = 2 Then
                lFunzOut = "AsGml()"
            End If

            stb.Append(" select GEOMETRY::GeomFromGml(Poligono_GeoEntity.AsGml(), 4326).STEnvelope()." & lFunzOut & " as BoundingBox " & vbCrLf)
            stb.Append(" from gis_entita e inner join gis_Elementigrafici g on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append(" where e.entita_cod = " & Entita_cod & vbCrLf)
            Dim dt As DataTable =
            EseguiQuery_Lettura(objparametri, stb.ToString, "")

            If dt.Rows.Count > 0 Then
                xRisp = dt.Rows(0)("BoundingBox")
            End If

            Return xRisp

        Catch ex As Exception

            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objparametri, "", MessaggioErrore)
            xRisp = False


        End Try

        Return xRisp
    End Function

    Public Function getEnvelopeWKT(ByVal layerId As Integer, ByVal objparametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.getEnvelopeWKT()"

        Dim MessaggioErrore As String
        Dim dt As DataTable

        Try

            Dim stb As New System.Text.StringBuilder

            stb.AppendLine("SELECT")
            stb.AppendLine("    geometry::STGeomFromWKB(geography::EnvelopeAggregate(poligono_GeoEntity).STAsBinary() , 4326).STEnvelope().STAsText() AS EnvelopeWKT")
            stb.AppendLine("FROM GIS_ElementiGrafici")
            stb.AppendLine(String.Format("WHERE LayerElementiGrafici_Cod = {0}", layerId))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objparametri, stb.ToString, "")
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objparametri, "", MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function



    Public Function Poligono_GetBaricentro(ByVal Entita_cod As Integer, ByVal wkt_1_OrGML_2 As String, ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal Funzione As String = "STCentroid") As String

        Dim xRisp As String = ""
        Dim MessaggioErrore As String

        Try

            Dim stb As New System.Text.StringBuilder

            Dim lFunzOut As String = "STAsText()"

            If wkt_1_OrGML_2 = 2 Then
                lFunzOut = "AsGml()"
            End If

            Dim fnz As String = "GEOMETRY::GeomFromGml(Poligono_GeoEntity.AsGml(), 4326).STCentroid()." & lFunzOut

            If Funzione <> "STCentroid" Then
                fnz = "Poligono_GeoEntity.EnvelopeCenter()." & lFunzOut
            End If

            stb.Append(" select " & fnz & " as BoundingBox " & vbCrLf)
            stb.Append(" from gis_entita e inner join gis_Elementigrafici g on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append(" where e.entita_cod = " & Entita_cod & vbCrLf)
            Dim dt As DataTable =
            EseguiQuery_Lettura(objparametri, stb.ToString, "")

            If dt.Rows.Count > 0 Then
                xRisp = dt.Rows(0)("BoundingBox")
            End If

            Return xRisp

        Catch ex As Exception

            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objparametri, "", MessaggioErrore)
            xRisp = False


        End Try

        Return xRisp
    End Function



    Public Class mykey
        Public piva As String
        Public sa_cod As Integer
        Public appezza As Integer
        Public extKey As Integer
    End Class

    Public Function Leggi_X_IF(listaChiavi As List(Of mykey), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New System.Text.StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @TKEY AS TABLE(PIVA VARCHAR(50), SA_COD INTEGER, APPEZZA INTEGER, ExtKey INTEGER) ")
            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TKEY VALUES ")

            Dim sep As String = ""

            For Each k In listaChiavi
                sql.AppendLine(sep & "('" & Agro_SQL_SaveText(k.piva) & "', " & k.sa_cod.ToString & ", " & k.appezza.ToString & ", " & k.extKey.ToString & ") ")
                sep = ","
            Next

            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Etichetta = a.APP_NOME ")
            sql.AppendLine("	--, VegDes = v.Veg_Des ")
            sql.AppendLine("	--, CulDes = u.Cul_Des ")
            sql.AppendLine("	--, g.Poligono_GeoEntity ")
            sql.AppendLine("	--, g.Poligono_GeoEntity.ToString() ")
            sql.AppendLine("    , json_geom = '[' + REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Poligono_GeoEntity.ToString(),'POLYGON ',''),'(','['),')',']'),'], ',']],['),', ','],['),' ',',') + ']' ")
            sql.AppendLine("	, k.ExtKey ")
            sql.AppendLine("FROM GIS_ElementiGrafici g ")
            sql.AppendLine("INNER JOIN gis_entita e ON e.Entita_Cod = g.Entita_Cod ")
            sql.AppendLine("INNER JOIN @TKEY k ON k.PIVA = e.PIVA AND k.SA_COD = e.SA_COD AND k.APPEZZA = e.APPEZZA ")
            sql.AppendLine("INNER JOIN Appezzamento a ON a.PIVA = k.PIVA AND a.SA_COD = k.SA_COD AND a.APPEZZA = k.APPEZZA ")
            sql.AppendLine("--INNER JOIN Reg_Impianti i ")
            sql.AppendLine("--INNER JOIN Cultivar u on u .Cul_Cod = i.CUL_COD ")
            sql.AppendLine("--INNER JOIN SpecieVegetali v on v.Veg_Cod = u.Veg_Cod ")
            sql.AppendLine("WHERE g.LayerElementiGrafici_Cod = 1 --APPEZZAMENTI ")

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, "")

        Catch ex As Exception

            Scrivi_LOG(objParametri, "", ex.Message)
            DT = Nothing
        End Try

        Return DT
    End Function

    Public Function Poligono_GetGeoJsonPointsWithArea(
                ByVal EntitaCod As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.Poligono_GetGeoJsonPointsWithArea()"


        '==============================================
        '   !!!!ATTENZIONE!!!!
        '   La query per funzionare richiede che esiste la funzione geometry2json
        '==============================================   

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Select ")
            StrSQL.Append("     Poligono_GeoEntity.STArea()/10000 as area ")
            StrSQL.Append("     ,Poligono_GeoEntity.STAsText() as geo ")
            StrSQL.Append(" from ")
            StrSQL.Append("     GIS_ElementiGrafici ")
            StrSQL.Append(" where ")
            StrSQL.Append("     Entita_Cod=" + Agro_SQL_SaveNum(EntitaCod))

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiElementiIntersecanti(ByVal layerElementiGrafici_Cod As Integer,
                                              ByVal poligonoWKT As String,
                                              ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiElementiIntersecanti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
	        StrSQL.AppendLine("     EG.Entita_Cod ")
            StrSQL.AppendLine("     , ISNULL(E.Entita_GUID, '') As Entita_GUID ")
            StrSQL.AppendLine(" FROM GIS_ElementiGrafici EG ")
            StrSQL.AppendLine(" INNER JOIN GIS_Entita E ")
            StrSQL.AppendLine("     ON E.Entita_Cod = EG.Entita_Cod ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format("     EG.Poligono_GeoEntity.STIntersects({0}) = 1 ", Agro_SQL_SaveGeograpyFromWKTString(poligonoWKT)))

            If layerElementiGrafici_Cod = 0 Then
                layerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI
            End If

            StrSQL.AppendLine(String.Format("     AND EG.LayerElementiGrafici_Cod = {0} ", layerElementiGrafici_Cod))

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDatiLayer(ByVal layerElementiGrafici_cod As Int32,
                                   ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiDatiLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" distinct EG.Entita_Cod ")
            StrSQL.AppendLine(" , E.Entita_GUID ")
            StrSQL.AppendLine(" FROM GIS_ElementiGrafici EG ")
            StrSQL.AppendLine(" INNER JOIN GIS_Entita E ON E.Entita_Cod = EG.Entita_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" EG.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format("     AND EG.LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_cod)))

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiIntersezioneCatastoPoligono(ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Appezza As Integer,
                                                     ByVal Id_Reg As Integer,
                                                     ByVal TipoEntitaAppezza_Cod As Integer,
                                                     ByVal TipoEntitaCatasto_Cod As Integer,
                                                     ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.LeggiIntersezioneCatastoPoligono()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        If Piva = "" Then
            Throw New NullReferenceException("Specificare la piva")
        End If
        If TipoEntitaCatasto_Cod = 0 Then
            Throw New NullReferenceException("Specificare il layer su cui eseguire la sovrapposizione")
        End If

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" declare @g as geography ")
            StrSQL.AppendLine(" select @g=Poligono_GeoEntity from GIS_ElementiGrafici a1 inner join ")
            StrSQL.AppendLine(" GIS_Entita b1 on (a1.PivaSuperUser=b1.PivaSuperUser and a1.Entita_Cod=b1.Entita_Cod) ")
            StrSQL.AppendLine(" where 1=1 ")
            If Piva <> "" Then
                StrSQL.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(Piva)))
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" and Sa_Cod= {0}", Agro_SQL_SaveNum(Sa_Cod)))
            End If
            If Appezza <> 0 Then
                StrSQL.AppendLine(String.Format(" and Appezza= {0}", Agro_SQL_SaveNum(Appezza)))
            End If
            If Id_Reg <> 0 Then
                StrSQL.AppendLine(String.Format(" and Id_Imp= {0}", Agro_SQL_SaveNum(Id_Reg)))
            End If
            If TipoEntitaAppezza_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" and TipoEntita_Cod= {0}", Agro_SQL_SaveNum(TipoEntitaAppezza_Cod)))
            End If

            'Lavez - 22/10/2025 - workaround per velocizzare la query di intersezione
            '!!!!! ATTENZIONE !!!!!
            ' E' stata usata la tabella GIS_ElementiGrafici_CatastoSimplyfied in quanto le geometrie sono semplificate ed il calcolo delle intersezioni risulta molto più veloce
            ' Suddetta tabella è popolata solo tramite import catasto da shp/dxf!!! calcolando l'Envelope della geometria generata dal poligono geography della particella!!!
            ' Quindi se tale tabella non è aggiornata, i risultati della query potrebbero essere errati!!!

            StrSQL.AppendLine(" select ")
            'StrSQL.AppendLine("     Poligono_GeoEntity.STIntersection(@g).STArea() as AreaIntersezione, ")
            'StrSQL.AppendLine("     Poligono_GeoEntity.STArea() as AreaParticella, ")
            'StrSQL.AppendLine("     isnull(Poligono_GeoEntity_WKT,Poligono_GeoEntity.STAsText()) as wktParticella, ")
            StrSQL.AppendLine("     b.Entita_Cod, ")
            StrSQL.AppendLine("     b.PROV, ")
            StrSQL.AppendLine("     b.COM, ")
            StrSQL.AppendLine("     b.SEZIONE, ")
            StrSQL.AppendLine("     b.FOGLIO, ")
            StrSQL.AppendLine("     b.NUMERO, ")
            StrSQL.AppendLine("     b.SUBALTERNO ")
            StrSQL.AppendLine(" FROM GIS_ElementiGrafici_CatastoSimplyfied a ")
            StrSQL.AppendLine(" INNER JOIN GIS_Entita b On (a.PivaSuperUser=b.PivasuperUser and a.Entita_Cod = b.Entita_Cod) ")
            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" b.TipoEntita_Cod = {0} ", Agro_SQL_SaveNum(TipoEntitaCatasto_Cod)))
            StrSQL.AppendLine(String.Format(" AND a.Poligono_GeoEntity.STIntersects(@g)=1 "))

            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function
End Class




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_ElementiGrafici_W
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function AggiornaStaticMap(
        ByVal EntitaCod As Integer,
        ByVal StaticMap As Byte(),
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False




        Try
            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" update g   ")
            stb.AppendLine("       set StaticMap = @P1 ")
            stb.AppendLine("     , data_modifica = getdate() ")
            stb.AppendLine(" From gis_entita e ")
            stb.AppendLine("     inner Join gis_elementigrafici g ")
            stb.AppendLine("         On  e.PivaSuperUser = g.PivaSuperUser ")
            stb.AppendLine("         And e.entita_cod = g.Entita_Cod ")
            stb.AppendLine(" where e.Entita_Cod = " & Agro_SQL_SaveNum(EntitaCod))

            Dim CmdParameters As New Dictionary(Of String, Byte())
            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                CmdParameters.Add("?", StaticMap)
            Else
                CmdParameters.Add("@P1", StaticMap)
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura_ParamVarBinary(objParametri, stb.ToString, NomeRoutine, CmdParameters)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Public Function PlanningRibaltaFast(
        ByVal EntitaCodNuovo As Integer,
        ByVal ElementoGraficoCodNuovo As Integer,
        ByVal programmazione_Entita_Cod As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False




        Try
            '---------------------------------------------
            stb.Length = 0


            stb.AppendLine("  insert [dbo].[GIS_ElementiGrafici]( ")

            stb.AppendLine("                 PivaSuperUser, ")
            stb.AppendLine("                 ElementoGrafico_Cod, ")
            stb.AppendLine("                 ElementoGrafico_Des, ")
            stb.AppendLine("                 Entita_Cod, ")
            stb.AppendLine("                 LayerElementiGrafici_Cod, ")
            stb.AppendLine("                 Poligono_GeoEntity, ")
            stb.AppendLine("                 Flag_GPS, ")
            stb.AppendLine("                 inviato, ")
            stb.AppendLine("                 datainvio, ")
            stb.AppendLine("                 Data_Creazione, ")
            stb.AppendLine("                 Data_Modifica, ")
            stb.AppendLine("                 Username_Creazione, ")
            stb.AppendLine("                 Username_Modifica, ")
            stb.AppendLine("                 Validita_Inizio, ")
            stb.AppendLine("                 Validita_Fine ")
            stb.AppendLine("             ) ")
            stb.AppendLine("  ")
            stb.AppendLine(" Select top 1 ")
            stb.AppendLine("     g.PivaSuperUser, ")
            stb.AppendLine("     " & ElementoGraficoCodNuovo & " as  ElementoGrafico_Cod, ")
            stb.AppendLine("     ElementoGrafico_Des, ")
            stb.AppendLine("     " & EntitaCodNuovo & " Entita_Cod, ")
            stb.AppendLine("     19 as LayerElementiGrafici_Cod, ")
            stb.AppendLine("     Poligono_GeoEntity, ")
            stb.AppendLine("     Flag_GPS, ")
            stb.AppendLine("     g.inviato, ")
            stb.AppendLine("     g.datainvio, ")
            stb.AppendLine("     " & Agro_SQL_SaveDate(Now) & " as Data_Creazione, ")
            stb.AppendLine("     " & Agro_SQL_SaveDate(Now) & " as Data_Modifica, ")
            stb.AppendLine("     g.Username_Creazione, ")
            stb.AppendLine("     g.Username_Modifica, ")
            stb.AppendLine("     g.Validita_Inizio, ")
            stb.AppendLine("     g.Validita_Fine ")
            stb.AppendLine(" From GIS_Entita e ")
            stb.AppendLine("  inner Join GIS_ElementiGrafici g ")
            stb.AppendLine("         On e.Entita_Cod = g.Entita_Cod ")

            stb.AppendLine(" Where e.programmazione_Entita_Cod = " & programmazione_Entita_Cod)



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function


    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Scrivi(
                            ByVal PivaSuperUser As String,
                            ByVal ElementoGrafico_Cod As Int32,
                            ByVal ElementoGrafico_Des As String,
                            ByVal Entita_Cod As Int32,
                            ByVal LayerElementiGrafici_Cod As Int32,
                            ByVal Poligono_GeoEntity As String,
                            ByVal Flag_GPS As Int32,
                            ByVal OLDGrafica_ID As String,
                            ByVal piva As String,
                            ByVal sa_cod As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim xTest As Boolean = True

        Try

            If Poligono_GeoEntity.StartsWith("<gml:Polygon") Then 'Or Poligono_GeoEntity.StartsWith("<gml:LineString") Then
                xTest = TestaPoligono(Poligono_GeoEntity)
            Else
                xTest = True
            End If


            '1 tentativo con i dati passati
            If xTest = True Then
                Scrivi_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, objParametri, StrSQL, 1, OLDGrafica_ID, piva, sa_cod)

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Else

                '2 tentativo con il poligono invertito
                Dim oldPolygon As String = Poligono_GeoEntity
                Poligono_GeoEntity = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono(Poligono_GeoEntity, True)

                xTest = TestaPoligono(Poligono_GeoEntity, objParametri)
                If xTest Then

                    Scrivi_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, objParametri, StrSQL, 1, OLDGrafica_ID, piva, sa_cod)

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------
                Else

                    Dim LeggiCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtGfgSiti As DataTable
                    dtGfgSiti = LeggiCfgSiti.Leggi(
                        AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_GiasOnline_2010,
                        "ForzaMakeValid",
                        "",
                        "",
                        objParametri
                    )
                    Dim cfgForzaMakeValid As Boolean = False
                    If dtGfgSiti.Rows.Count > 0 AndAlso
                        dtGfgSiti.Rows(0)("valore").ToString.ToLower = "true" Then

                        cfgForzaMakeValid = True
                    End If

                    If cfgForzaMakeValid Then
                        '3 tento la chiamata alla funzione makevalid() - per ora solo su importazione vecchio gis
                        If Not String.IsNullOrEmpty(OLDGrafica_ID) Then
                            Poligono_GeoEntity = AgronicaGIS2012.Commons.PolygonOrder.InvertiLatLong(Poligono_GeoEntity, True)
                            xTest = TestaPoligono_usaMakeValid(Poligono_GeoEntity, objParametri)
                        End If
                    End If



                    If xTest Then

                        Scrivi_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, objParametri, StrSQL, 1, OLDGrafica_ID, piva, sa_cod, usaMakeValid:=True)

                        '--------------------------------------------------------------------------
                        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                        '--------------------------------------------------------------------------
                    Else


                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)


                        '4 loggo il mancato inserimento!
                        Scrivi_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, objParametri, StrSQL, 2, OLDGrafica_ID, piva, sa_cod)

                        '--------------------------------------------------------------------------
                        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                        '--------------------------------------------------------------------------

                        xRisp = False
                    End If

                End If

            End If

        Catch ex As Exception

            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        Return xRisp

    End Function

    Public Function TestaPoligono_usaMakeValid(
                            ByVal Poligono_GeoEntity As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = True

        Try

            '1 tentativo con i dati passati
            TestaPoligono_GetQuery_usa_MakeValid(Poligono_GeoEntity, objParametri, StrSQL)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False


        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Testa l'orientamento del poligono (disegnato in senso orario oppure antiorario), SQL Server richiede che i poligoni siano disegnati in senso antiorario, altrimenti disegna "il resto del mondo". 
    ''' </summary>
    ''' <param name="Poligono_GeoEntity"></param>
    ''' <returns></returns>
    ''' <remarks>https://gis.stackexchange.com/questions/287903/when-does-wkt-orientation-matter-for-polygon-and-multipolygon-in-geography-spati</remarks>
    Public Function TestaPoligono(Poligono_GeoEntity As String) As Boolean

        Dim rval As Boolean

        If Poligono_GeoEntity.TrimStart.StartsWith("<") Then
            rval = TestaPoligonoGML(Poligono_GeoEntity)
        Else
            rval = TestaPoligonoWKT(Poligono_GeoEntity)
        End If

        Return rval

    End Function
    Public Function TestaPoligonoGML(Poligono_GeoEntity As String) As Boolean

        Dim cvt As New GML 
        Dim listaPuntiExterior As List(Of xyz) = cvt.EstraiCoordinateDaGML(Poligono_GeoEntity, True, True)

        Dim EsitoTest As Boolean
        EsitoTest = TestaPoligonoSensoAntiorario(listaPuntiExterior)

        If Poligono_GeoEntity.Contains("interior") Then
            Dim listaPuntiInterior As List(Of xyz) = cvt.EstraiCoordinateDaGML(Poligono_GeoEntity, True, False)
            EsitoTest = EsitoTest And Not TestaPoligonoSensoAntiorario(listaPuntiInterior)
        End If


        Return EsitoTest

    End Function

    Public Function TestaPoligonoWKT(Poligono_GeoEntity As String) As Boolean

        Dim cvt As New WKT
        Dim listaPunti As List(Of xyz) = cvt.CreaCoordinateDaPoligono(Poligono_GeoEntity)

        Return TestaPoligonoSensoAntiorario(listaPunti)

    End Function


    ''' <summary>
    ''' Restituisce True se il poligono è stato disegnato in senso antiorario
    ''' </summary>
    ''' <param name="listaPunti"></param>
    ''' <returns></returns>
    Private Shared Function TestaPoligonoSensoAntiorario(listaPunti As List(Of xyz)) As Boolean
        Dim area As Double = 0

        '-- Sum over the edges, (x2 − x1)(y2 + y1).

        For i = 0 To listaPunti.Count - 2

            Dim p1 As xyz = listaPunti(i)
            Dim p2 As xyz = listaPunti(i + 1)

            area += (p2.X - p1.X) * (p2.Y + p1.Y)

        Next

        Return area < 0
    End Function

    Public Function TestaPoligonoWKT(
                            ByVal Poligono_GeoEntity As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = True

        Try

            '1 tentativo con i dati passati
            TestaPoligono_GetQueryWKT(Poligono_GeoEntity, objParametri, StrSQL)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False


        End Try

        Return xRisp

    End Function

    Public Function TestaPoligonoWKTValid(
                            ByVal Poligono_GeoEntity As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal LogError As Boolean = True
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.TestaPoligonoWKTValid()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = True
        Dim DT As DataTable

        Try

            '1 tentativo con i dati passati
            TestaPoligono_GetQueryWKTValid(Poligono_GeoEntity, objParametri, StrSQL)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine, LogError)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore, LogError)
            xRisp = False
            DT = Nothing
        End Try

        If DT IsNot Nothing Then
            xRisp = DT.Rows(0)("Valid")
        End If

        Return xRisp

    End Function

    Public Function TestaPoligono(
                            ByVal Poligono_GeoEntity As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = True

        Try

            '1 tentativo con i dati passati
            TestaPoligono_GetQuery(Poligono_GeoEntity, objParametri, StrSQL)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False


        End Try

        Return xRisp

    End Function

    Private Sub TestaPoligono_GetQuery(ByVal Poligono_GeoEntity As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal StrSQL As System.Text.StringBuilder)
        StrSQL.Length = 0
        StrSQL.Append("declare @g geography;" & vbCrLf)
        StrSQL.Append("set @g = " & Agro_SQL_SaveGeograpyFromGMLString(Poligono_GeoEntity) & ";" & vbCrLf)


    End Sub

    Private Sub TestaPoligono_GetQueryWKTValid(ByVal Poligono_GeoEntity As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal StrSQL As System.Text.StringBuilder)
        StrSQL.Length = 0
        StrSQL.AppendLine("BEGIN TRY ")
        StrSQL.AppendLine("     declare @g geography; ")
        StrSQL.AppendLine("     set @g = " & Agro_SQL_SaveGeograpyFromWKTString(Poligono_GeoEntity) & "; ")
        StrSQL.AppendLine("     select @g.STIsValid() as Valid ")
        StrSQL.AppendLine("END TRY ")
        StrSQL.AppendLine("BEGIN CATCH ")
        StrSQL.AppendLine("     IF (ERROR_NUMBER()=6522) ")
        StrSQL.AppendLine("     BEGIN ")
        StrSQL.AppendLine("         select cast(0 as bit) as Valid ")
        StrSQL.AppendLine("     END ")
        StrSQL.AppendLine("     ELSE ")
        StrSQL.AppendLine("     THROW ")
        StrSQL.AppendLine("END CATCH")

    End Sub

    Private Sub TestaPoligono_GetQueryWKT(ByVal Poligono_GeoEntity As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal StrSQL As System.Text.StringBuilder)
        StrSQL.Length = 0
        StrSQL.Append("declare @g geography;" & vbCrLf)
        StrSQL.Append("set @g = " & Agro_SQL_SaveGeograpyFromWKTString(Poligono_GeoEntity) & ";" & vbCrLf)


    End Sub
    Private Sub TestaPoligono_GetQuery_usa_MakeValid(ByVal Poligono_GeoEntity As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal StrSQL As System.Text.StringBuilder)
        StrSQL.Length = 0
        StrSQL.Append("declare @g geography;" & vbCrLf)
        StrSQL.Append("set @g = " & Agro_SQL_SaveGeograpyFromGMLString_usa_MakeValid(Poligono_GeoEntity) & ";" & vbCrLf)


    End Sub


    Private Sub Scrivi_GetQuery(ByVal PivaSuperUser As String, ByVal ElementoGrafico_Cod As Int32, ByVal ElementoGrafico_Des As String, ByVal Entita_Cod As Int32, ByVal LayerElementiGrafici_Cod As Int32, ByVal Poligono_GeoEntity As String, ByVal Flag_GPS As Int32, ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef StrSQL As System.Text.StringBuilder, ByVal Grafica_1_Stringa_2 As Int16, ByVal OLDGrafica_ID As String, ByVal Piva As String, ByVal sa_cod As Integer, Optional ByVal usaMakeValid As Boolean = False)

        Dim tabella As String
        Dim sOLDGrafica_ID As String
        Dim campoChiaveLog As String
        Dim campoChiaveQuery As String

        If Grafica_1_Stringa_2 = 1 Then
            tabella = "GIS_ElementiGrafici"
            sOLDGrafica_ID = ""
            campoChiaveLog = ""
            campoChiaveQuery = ""
        Else
            tabella = "GIS_ElementiGrafici_LogErrori"
            sOLDGrafica_ID = "OLDGrafica_ID, piva, sa_cod,"
            campoChiaveLog = "GIS_ElementiGrafici_LogErrori_COD"
            campoChiaveQuery = "coalesce((select max(" & campoChiaveLog & ")+1 from GIS_ElementiGrafici_LogErrori), 1)"
        End If

        '---------------------------------------------
        StrSQL.Length = 0
        StrSQL.Append("INSERT INTO  " & tabella & "  ")
        StrSQL.Append("                   ( ")

        If Grafica_1_Stringa_2 = 2 Then
            StrSQL.Append(campoChiaveLog & ",")
        End If

        StrSQL.Append("                    PivaSuperUser,               ElementoGrafico_Cod,    ")
        StrSQL.Append("                    ElementoGrafico_Des,         Entita_Cod,    ")
        StrSQL.Append("                    LayerElementiGrafici_Cod,    Poligono_GeoEntity,    ")
        StrSQL.Append("                    Flag_GPS, " & sOLDGrafica_ID)

        StrSQL.Append("                    Inviato,            DataInvio, ")
        StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
        StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
        StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
        StrSQL.Append("                   ) ")

        StrSQL.Append("VALUES (")

        If Grafica_1_Stringa_2 = 2 Then
            StrSQL.Append(campoChiaveQuery & ",")
        End If

        StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
        StrSQL.Append("         , " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & "  ")
        StrSQL.Append("         ,'" & Agro_SQL_SaveText(ElementoGrafico_Des) & "' ")
        StrSQL.Append("         , " & Agro_SQL_SaveNum(Entita_Cod) & "  ")
        StrSQL.Append("         , " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & "  ")

        If Grafica_1_Stringa_2 = 1 Then
            If usaMakeValid Then
                StrSQL.Append("         , " & Agro_SQL_SaveGeograpyFromGMLString_usa_MakeValid(Poligono_GeoEntity) & " ")
            Else
                StrSQL.Append("         , " & Agro_SQL_SaveGeograpyFromGMLString(Poligono_GeoEntity) & " ")
            End If

        Else
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Poligono_GeoEntity) & "' ")
        End If


        StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_GPS) & "  ")
        If Grafica_1_Stringa_2 = 2 Then
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(OLDGrafica_ID) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(sa_cod) & " ")
        End If


        StrSQL.Append("         , 0  ")
        StrSQL.Append("         , Null  ")
        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
        StrSQL.Append(" )")
        '---------------------------------------------
    End Sub
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Modifica(
                            ByVal PivaSuperUser As String,
                            ByVal ElementoGrafico_Cod As Int32?,
                            ByVal ElementoGrafico_Des As String,
                            ByVal Entita_Cod As Int32?,
                            ByVal LayerElementiGrafici_Cod As Int32?,
                            ByVal Poligono_GeoEntity As String,
                            ByVal Flag_GPS As Int32?,
                            ByVal OLDGrafica_ID As String,
                            ByVal piva As String,
                            ByVal sa_cod As String,
                                    ByVal Validita_Inizio As Date?,
                                    ByVal Validita_Fine As Date?,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        If PivaSuperUser Is Nothing OrElse PivaSuperUser = "" Then
            Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
        End If

        If ElementoGrafico_Cod Is Nothing OrElse ElementoGrafico_Cod = 0 Then
            Throw New Exception("Parametro non corretto nella query (ElementoGrafico_Cod obbligatorio)")
        End If


        Dim xTest As Boolean = True

        Try

            xTest = TestaPoligono(Poligono_GeoEntity)

            '1 tentativo con i dati passati
            If xTest = True Then
                Modifica_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, "", objParametri, StrSQL, 1, OLDGrafica_ID, piva, sa_cod)

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Else

                '2 tentativo con il poligono invertito
                Dim oldPolygon As String = Poligono_GeoEntity
                Poligono_GeoEntity = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono(Poligono_GeoEntity, True)

                xTest = TestaPoligono(Poligono_GeoEntity, objParametri)
                If xTest Then

                    Modifica_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, "", objParametri, StrSQL, 1, OLDGrafica_ID, piva, sa_cod)

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------
                Else

                    '3 loggo il mancato inserimento!
                    Modifica_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, "", objParametri, StrSQL, 2, OLDGrafica_ID, piva, sa_cod)

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    xRisp = False

                End If

            End If

        Catch ex As Exception

            'Eccezione solo in caso di mancato salvataggio nel LOG.
            MessaggioErrore = ex.Message & vbCrLf & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        Return xRisp


    End Function



    Private Sub Modifica_GetQuery(ByVal PivaSuperUser As String, ByVal ElementoGrafico_Cod As Int32?, ByVal ElementoGrafico_Des As String, ByVal Entita_Cod As Int32?, ByVal LayerElementiGrafici_Cod As Int32?, ByVal Poligono_GeoEntity As String, ByVal Flag_GPS As Int32?, ByVal Validita_Inizio As Date?, ByVal Validita_Fine As Date?, ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal StrSQL As System.Text.StringBuilder, ByVal Grafica_1_Stringa_2 As Int16, ByVal OLDGrafica_ID As String, ByVal piva As String, ByVal sa_cod As Integer)

        If Grafica_1_Stringa_2 = 2 Then
            Scrivi_GetQuery(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, Validita_Inizio, Validita_Fine, objParametri, StrSQL, 2, OLDGrafica_ID, piva, sa_cod)
            Exit Sub
        End If

        StrSQL.Length = 0

        StrSQL.Append("UPDATE gis_elementigrafici SET ")

        StrSQL.Append("   Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
        If Not IsNothing(ElementoGrafico_Des) AndAlso ElementoGrafico_Des <> "" Then StrSQL.Append("    ,[ElementoGrafico_Des]      = '" & Agro_SQL_SaveText(ElementoGrafico_Des) & "'")
        If Not IsNothing(Entita_Cod) AndAlso Entita_Cod <> 0 Then StrSQL.Append("   ,[Entita_Cod]                 =  " & Agro_SQL_SaveNum(Entita_Cod) & " ")
        If Not IsNothing(LayerElementiGrafici_Cod) AndAlso LayerElementiGrafici_Cod <> 0 Then StrSQL.Append("   ,[LayerElementiGrafici_Cod]   =  " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
        If Not IsNothing(Poligono_GeoEntity) Then StrSQL.Append("   ,[Poligono_GeoEntity]         =  " & Agro_SQL_SaveGeograpyFromGMLString(Poligono_GeoEntity) & " ")
        If Not IsNothing(Flag_GPS) Then StrSQL.Append("   ,[Flag_GPS]                   =  " & Agro_SQL_SaveNum(Flag_GPS) & " ")

        StrSQL.Append("   ,Inviato           =  0 ")
        StrSQL.Append("   ,DataInvio         =  Null ")
        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
        If Not IsNothing(Validita_Inizio) Then StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
        If Not IsNothing(Validita_Fine) Then StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

        StrSQL.Append(" WHERE PivaSuperUser         = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
        StrSQL.Append(" AND   ElementoGrafico_Cod   =  " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & " ")
        '---------------------------------------------

        '----------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If
    End Sub
    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Cancella(
                            ByVal PivaSuperUser As String,
                            ByVal ElementoGrafico_Cod As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If ElementoGrafico_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (ElementoGrafico_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE GIS_ElementiGrafici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE   PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
                StrSQL.Append(" AND     ElementoGrafico_Cod      =   " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & "  ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    GIS_ElementiGrafici ")
                StrSQL.Append(" WHERE   PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
                StrSQL.Append(" AND     ElementoGrafico_Cod      =   " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & "  ")

            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function EliminaDatiPrecisionFarmingDatoPlanning(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------


            stb.Length = 0

            stb.Append("declare @Entita_cod int " & vbCrLf)
            stb.Append(" set @Entita_cod = " & Entita_Cod & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" delete gg  " & vbCrLf)
            stb.Append(" from gis_entita ee " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici gg " & vbCrLf)
            stb.Append("        on ee.Entita_Cod = gg.entita_cod " & vbCrLf)
            stb.Append(" where exists (  " & vbCrLf)
            stb.Append("   Select 1 " & vbCrLf)
            stb.Append("        from gis_entita ii " & vbCrLf)
            stb.Append("            inner join programmazione_entita ee1 " & vbCrLf)
            stb.Append("                on ii.programmazione_cod = ee1.programmazione_cod             " & vbCrLf)
            stb.Append("        where entita_cod = @Entita_cod " & vbCrLf)
            stb.Append("            and ee.PivaSuperUser = ii.PivaSuperUser  " & vbCrLf)
            stb.Append("            and ee.piva = ii.piva " & vbCrLf)
            stb.Append("            and ee.sa_cod = ii.sa_cod " & vbCrLf)
            stb.Append("            and ee.Appezza = ee1.Appezza             " & vbCrLf)
            stb.Append("            and ee.Id_Imp = ee1.Id_Reg       " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" and ee.tipoentita_Cod in (" & Agro_SQL_Save_Clausola_IN(ElencoLayer) & " ) " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function EliminaDatiPrecisionFarmingDatoImpianto(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------


            stb.Length = 0

            stb.Append("declare @Entita_cod int " & vbCrLf)
            stb.Append(" set @Entita_cod = " & Entita_Cod & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" delete gg  " & vbCrLf)
            stb.Append(" from gis_entita ee " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici gg " & vbCrLf)
            stb.Append("        on ee.Entita_Cod = gg.entita_cod " & vbCrLf)
            stb.Append(" where exists ( " & vbCrLf)
            stb.Append(" Select 1 " & vbCrLf)
            stb.Append("    from gis_entita ii " & vbCrLf)
            stb.Append("    where entita_cod = @Entita_cod " & vbCrLf)
            stb.Append("    and ee.PivaSuperUser = ii.PivaSuperUser  " & vbCrLf)
            stb.Append("    and ee.piva = ii.piva " & vbCrLf)
            stb.Append("    and ee.sa_cod = ii.sa_cod " & vbCrLf)
            stb.Append("    and ee.Appezza = ii.Appezza  " & vbCrLf)
            stb.Append("    and ee.Campo_Cod = ii.Campo_Cod  " & vbCrLf)
            stb.Append("    and ee.Id_Imp = ii.Id_Imp  " & vbCrLf)
            stb.Append("    and ee.Programmazione_Cod = ii.Programmazione_cod  " & vbCrLf)

            stb.Append(" ) " & vbCrLf)
            stb.Append(" and ee.tipoentita_Cod in (" & Agro_SQL_Save_Clausola_IN(ElencoLayer) & " ) " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviElementoGraficoBaseDaWKT(ByVal newIDElementoGrafico As Integer,
                                                   ByVal elementoGrafico_Des As String,
                                                   ByVal newIDEntita As Integer,
                                                   ByVal layerElementiGrafici_Cod As Integer,
                                                   ByVal geoData As String,
                                                   ByVal inizio_Validita As Date,
                                                   ByVal fine_Validita As Date,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.ScriviElementoGraficoBaseDaWKT()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine("INSERT INTO GIS_ElementiGrafici ( ")
            StrSql.AppendLine(" PivaSuperUser ")
            StrSql.AppendLine(" , ElementoGrafico_Cod ")
            StrSql.AppendLine(" , ElementoGrafico_Des ")
            StrSql.AppendLine(" , Entita_Cod ")
            StrSql.AppendLine(" , LayerElementiGrafici_Cod ")
            StrSql.AppendLine(" , Poligono_GeoEntity ")
            StrSql.AppendLine(" , Flag_GPS ")
            StrSql.AppendLine(" , UserName_Creazione ")
            StrSql.AppendLine(" , UserName_Modifica ")
            StrSql.AppendLine(" , Validita_Inizio ")
            StrSql.AppendLine(" , Validita_Fine ")

            StrSql.AppendLine(" ) VALUES ( ")

            StrSql.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newIDElementoGrafico)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(elementoGrafico_Des)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newIDEntita)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveGeograpyFromWKTString(geoData)))

            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(0)))

            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(inizio_Validita)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(fine_Validita)))

            StrSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function EliminaElementiGraficiLayer(ByVal layerElementiGrafici_cod As Int32,
                                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.EliminaElementiGraficiLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine(" DELETE FROM GIS_ElementiGrafici ")

            StrSql.AppendLine(" WHERE ")

            StrSql.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSql.AppendLine(String.Format("   AND LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_cod)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp

    End Function

    Public Function EliminaEntitaLayer(ByVal entita_Cod As Int32,
                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.EliminaEntitaLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine(" DELETE FROM GIS_Entita ")

            StrSql.AppendLine(" WHERE ")

            StrSql.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSql.AppendLine(String.Format("   AND Entita_Cod = {0} ", Agro_SQL_SaveNum(entita_Cod)))



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function ClonaElementoGrafico(ByVal ElementoGrafico_Cod As Int32,
                                         ByVal Entita_Cod As Int32,
                                         ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim sequenza As New Agro_Sequenze
        Dim Nuovo_ElementoGrafico_Cod = sequenza.NuovoId_Tabella("GIS_ElementiGrafici", 0, Int32.MaxValue, objParametri, True)

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.ClonaElementoGrafico()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSql.Length = 0

            StrSql.AppendLine(" INSERT INTO GIS_ElementiGrafici ( ")
            StrSql.AppendLine(" PivaSuperUser ")
            StrSql.AppendLine(" , ElementoGrafico_Cod ")
            StrSql.AppendLine(" , ElementoGrafico_Des ")
            StrSql.AppendLine(" , Entita_Cod ")
            StrSql.AppendLine(" , LayerElementiGrafici_Cod ")
            StrSql.AppendLine(" , Poligono_GeoEntity ")
            StrSql.AppendLine(" , Flag_GPS ")
            StrSql.AppendLine(" , Data_Creazione ")
            StrSql.AppendLine(" , Data_Modifica ")
            StrSql.AppendLine(" , Username_Creazione ")
            StrSql.AppendLine(" , Username_Modifica ")
            StrSql.AppendLine(" , Validita_Inizio ")
            StrSql.AppendLine(" , Validita_Fine ")
            StrSql.AppendLine(" , StaticMap ")
            StrSql.AppendLine(" , ElementoGrafico_GUID ) ")

            StrSql.AppendLine(" SELECT ")
            StrSql.AppendLine(" PivaSuperUser ")

            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Nuovo_ElementoGrafico_Cod)))

            StrSql.AppendLine(" , ElementoGrafico_Des ")

            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Entita_Cod)))

            StrSql.AppendLine(" , LayerElementiGrafici_Cod ")
            StrSql.AppendLine(" , Poligono_GeoEntity ")
            StrSql.AppendLine(" , Flag_GPS ")
            StrSql.AppendLine(" , GETDATE() ")
            StrSql.AppendLine(" , GETDATE() ")

            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSql.AppendLine(" , Validita_Inizio ")
            StrSql.AppendLine(" , Validita_Fine ")
            StrSql.AppendLine(" , StaticMap ")
            StrSql.AppendLine(" , null ")

            StrSql.AppendLine(" FROM GIS_ElementiGrafici ")

            StrSql.AppendLine(String.Format(" WHERE ElementoGrafico_Cod = {0} ", Agro_SQL_SaveNum(ElementoGrafico_Cod)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp

    End Function

    ''' <summary>
    ''' Updates the description of the graphic element identified by <paramref name="elementoGraficoCod"/> and <paramref name="entitaCod"/> into the database
    ''' </summary>
    ''' <param name="entitaCod"></param>
    ''' <param name="elementoGraficoCod"></param>
    ''' <param name="newDescription"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function UpdateGraphicElementDescription(entitaCod As Integer,
                                                    elementoGraficoCod As Integer,
                                                    newDescription As String,
                                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.UpdateGraphicElementDescription"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            StrSql.Length = 0

            StrSql.AppendLine("UPDATE GIS_ElementiGrafici")
            StrSql.AppendLine(String.Format("SET ElementoGrafico_Des = '{0}'", newDescription))
            StrSql.AppendLine("WHERE 1 = 1")
            StrSql.AppendLine(String.Format("    AND ElementoGrafico_Cod = {0}", elementoGraficoCod))
            StrSql.AppendLine(String.Format("    AND Entita_Cod = {0}", entitaCod))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function UpdateGraphicElementGeometry(entitaCod As Integer,
                                                elementoGraficoCod As Integer,
                                                geoData As String,
                                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.UpdateGraphicElementGeometry"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            StrSql.Length = 0

            StrSql.AppendLine("UPDATE GIS_ElementiGrafici")
            StrSql.AppendLine(String.Format(" SET Poligono_GeoEntity = {0} ", Agro_SQL_SaveGeograpyFromWKTString(geoData)))
            StrSql.AppendLine(String.Format("     , Data_Modifica = {0} ", Agro_SQL_SaveDate(DateTime.Now)))
            StrSql.AppendLine(String.Format("     , Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine("WHERE 1 = 1")
            StrSql.AppendLine(String.Format("    AND ElementoGrafico_Cod = {0}", elementoGraficoCod))
            StrSql.AppendLine(String.Format("    AND Entita_Cod = {0}", entitaCod))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function
End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
