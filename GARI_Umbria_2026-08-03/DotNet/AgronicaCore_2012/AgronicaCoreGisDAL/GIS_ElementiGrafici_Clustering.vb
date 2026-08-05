Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text
Imports System.Data
Public Class GIS_ElementiGrafici_Clustering_R
    Inherits AgronicaCoreDataProvider.DataProvider2010

    ' Recupera un elenco di centroidi pre-calcolati che si trovano 
    ' all'interno di un dato bounding box e appartengono a specifici layer.


    Public Function LeggiCentroidiInBoundingBox(
        ByVal wkt_bbox As String, 'la stringa wkt del poligono che rappresenta la bbox
        ByVal layer_cods As List(Of Integer), 'lista di ID dei layer da includere nella ricerca
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_Clustering_R.LeggiCentroidiInBoundingBox()"
        Dim strSQL As New StringBuilder
        Dim dt As DataTable

        Try

            Dim layers As String = String.Join(",", layer_cods)

            strSQL.AppendLine(" DECLARE @g as geography;")
            strSQL.AppendLine(" SET @g = geography::STGeomFromText('" & Agro_SQL_SaveText(wkt_bbox) & "', 4326);")

            strSQL.AppendLine(" SELECT")
            strSQL.AppendLine("   A.Centroide_GeoEntity_WKT,")
            strSQL.AppendLine("   A.LayerElementoGrafico_Cod,")
            strSQL.AppendLine("   B.Entita_Cod")
            strSQL.AppendLine(" FROM dbo.GIS_ElementiGrafici_Clustering AS A")
            strSQL.AppendLine(" INNER JOIN dbo.GIS_ElementiGrafici AS B ON A.PivaSuperUser = B.PivaSuperUser AND A.ElementoGrafico_Cod = B.ElementoGrafico_Cod")
            strSQL.AppendLine(" WHERE A.Centroide_GeoEntity IS NOT NULL AND A.Centroide_GeoEntity.STIntersects(@g) > 0")
            strSQL.AppendLine("   AND A.LayerElementoGrafico_Cod IN (" & layers & ")")
            strSQL.AppendLine("   AND A.Stato = 1")
            strSQL.AppendLine(" ORDER BY a.LayerElementoGrafico_Cod")

            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Recupera i centroidi per il clustering basati sui filtri di un'entità (azienda/appezzamento, etc.).
    ''' </summary>
    Public Function LeggiCentroidiEntitaAnagrafica(
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal appezza As Integer,
        ByVal id_imp As Integer,
        ByVal layer_cods As List(Of Integer),
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_Clustering_R.LeggiCentroidiEntitaAnagrafica()"
        Dim strSQL As New StringBuilder
        Dim dt As DataTable

        Try

            Dim layers As String = String.Join(",", layer_cods)

            strSQL.AppendLine(" SELECT")
            strSQL.AppendLine("   A.Centroide_GeoEntity_WKT,")
            strSQL.AppendLine("   A.LayerElementoGrafico_Cod,")
            strSQL.AppendLine("   B.Entita_Cod")
            strSQL.AppendLine(" FROM dbo.GIS_ElementiGrafici_Clustering AS A")
            strSQL.AppendLine(" INNER JOIN dbo.GIS_ElementiGrafici AS B ON A.PivaSuperUser = B.PivaSuperUser AND A.ElementoGrafico_Cod = B.ElementoGrafico_Cod")
            strSQL.AppendLine(" INNER JOIN dbo.GIS_Entita AS C ON B.PivaSuperUser = C.PivaSuperUser AND B.Entita_Cod = C.Entita_Cod")
            strSQL.AppendLine(" WHERE 1=1")
            If Not String.IsNullOrEmpty(piva) Then
                strSQL.AppendLine(" AND C.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            If sa_cod > 0 Then
                strSQL.AppendLine(" AND C.SA_Cod = " & Agro_SQL_SaveNum(sa_cod))
            End If
            If appezza > 0 Then
                strSQL.AppendLine(" AND C.Appezzamento = " & Agro_SQL_SaveNum(appezza))
            End If
            If id_imp > 0 Then
                strSQL.AppendLine(" AND C.ID_Reg = " & Agro_SQL_SaveNum(id_imp))
            End If
            strSQL.AppendLine("   AND A.LayerElementoGrafico_Cod IN (" & layers & ")")
            strSQL.AppendLine("   AND A.Stato = 1")
            strSQL.AppendLine(" ORDER BY a.LayerElementoGrafico_Cod")

            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function
End Class
Public Class GIS_ElementiGrafici_Clustering_W
    Inherits AgronicaCoreDataProvider.DataProvider2010
End Class
