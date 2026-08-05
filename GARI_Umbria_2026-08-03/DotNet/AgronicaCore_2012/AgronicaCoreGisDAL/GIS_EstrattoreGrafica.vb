Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class GIS_EstrattoreGrafica_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
            ByVal piva As String, _
            ByVal sa_cod As Integer, _
            ByVal DataDa As DateTime, _
            ByVal DataA As DateTime, _
            ByVal ListaLayers As String, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append("    select  " & vbCrLf)
            stb.Append("      e.Entita_Cod " & vbCrLf)

            stb.Append("    , e.piva  " & vbCrLf)
            stb.Append("    , e.sa_cod  " & vbCrLf)
            stb.Append("    , e.appezza  " & vbCrLf)

            stb.Append("    , g.ElementoGrafico_Cod  " & vbCrLf)
            stb.Append("    , g.ElementoGrafico_Des  " & vbCrLf)
            stb.Append("    , g.LayerElementiGrafici_Cod  " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.STAsText() as Poligono_GeoEntityWKT " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.STArea() as Area  " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.STLength() as Perimetro " & vbCrLf)
            stb.Append("    , l.LayerElementiGrafici_Des  " & vbCrLf)
            stb.Append("    , coalesce(c.Analisi_Campione_Cod , 0) as Analisi_Campione_Cod  " & vbCrLf)
            stb.Append("    , coalesce(c.Analisi_Campione_Des , '') as Analisi_Campione_Des " & vbCrLf)
            stb.Append("    , coalesce(c.Validita_Inizio, e.validita_inizio) as validita_inizio " & vbCrLf)
            stb.Append("    , coalesce(c.Validita_Fine, e.validita_Fine) as validita_Fine " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from gis_entita e  " & vbCrLf)
            stb.Append("    inner join gis_elementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append("    left join Analisi_Campioni c " & vbCrLf)
            stb.Append("        on c.Analisi_SuperUser = e.PivaSuperUser  " & vbCrLf)
            stb.Append("        and c.Analisi_Campione_Cod = e.analisi_campione_cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)

            stb.Append(" inner join ( " & vbCrLf)
            stb.Append("        select distinct LayerElementiGrafici_Cod, LayerElementiGrafici_Des, TipologiaLayer_cod  " & vbCrLf)
            stb.Append("        from GIS_LayerElementiGrafici  " & vbCrLf)
            stb.Append("    ) l " & vbCrLf)
            stb.Append("        on l.LayerElementiGrafici_Cod = g.LayerElementiGrafici_Cod  " & vbCrLf)
            stb.Append(" ")

            stb.Append(" where e.Piva =  '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
            stb.Append(" and e.sa_Cod =  " & sa_cod & vbCrLf)



            'stb.Append(" and ( " & vbCrLf)
            'stb.Append("    coalesce(c.Validita_Inizio, e.validita_inizio) = '01/01/1900' or  " & vbCrLf)
            'stb.Append("     ( " & vbCrLf)
            'stb.Append("        coalesce(c.Validita_Inizio, e.validita_inizio)  >=  CONVERT(DateTime,'2013/11/01',120)  " & vbCrLf)
            'stb.Append("        AND coalesce(c.Validita_Inizio, e.validita_inizio)  <=  CONVERT(DateTime,'2014/10/31',120)  " & vbCrLf)
            'stb.Append("    ) " & vbCrLf)
            'stb.Append(" ) " & vbCrLf)
            'stb.Append("  AND g.LayerElementiGrafici_Cod in (" & ListaLayers & ")")

            stb.Append(" and ( " & vbCrLf)
            stb.Append("    coalesce(c.Validita_Inizio, e.validita_inizio) = '01/01/1900' or  " & vbCrLf)
            stb.Append("     ( " & vbCrLf)
            stb.Append("        coalesce(c.Validita_Inizio, e.validita_inizio)  >=  " & Agro_SQL_SaveDate(DataDa) & "  " & vbCrLf)
            stb.Append("        AND coalesce(c.Validita_Inizio, e.validita_inizio)  <=  " & Agro_SQL_SaveDate(DataA) & "  " & vbCrLf)
            stb.Append("    ) " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append("  AND g.LayerElementiGrafici_Cod in (" & Agro_SQL_Save_Clausola_IN(ListaLayers) & ")")
            stb.Append("  AND l.TipologiaLayer_cod = 1")


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
                '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                        stb.Append(" AND   e.Inviato >=0 ")
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                        stb.Append(" AND   e.Inviato =-1 ")
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select
                '--------------------------------------------------------------------------

                If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    '##############################################################################################
    Public Function PoligonoWKTDatoEntitaCod(
            ByVal Entita_Cod As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append("    select  " & vbCrLf)
            stb.Append("      e.Entita_Cod " & vbCrLf)

            stb.Append("    , e.piva  " & vbCrLf)
            stb.Append("    , e.sa_cod  " & vbCrLf)
            stb.Append("    , e.appezza  " & vbCrLf)

            stb.Append("    , g.ElementoGrafico_Cod  " & vbCrLf)
            stb.Append("    , g.ElementoGrafico_Des  " & vbCrLf)
            stb.Append("    , g.LayerElementiGrafici_Cod  " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.STAsText() as Poligono_GeoEntityWKT " & vbCrLf)
            stb.Append("    , geometry::STGeomFromWKB(g.Poligono_GeoEntity.STAsBinary(), 1426).STEnvelope().STAsText() as Poligono_GeoEntity_envelopeWKT " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.STArea() as Area  " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.STLength() as Perimetro " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from gis_entita e  " & vbCrLf)
            stb.Append("    inner join gis_elementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod " & vbCrLf)


            stb.Append(" where e.entita_Cod =  " & Entita_Cod & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   e.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   e.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Date le chiavi di un impianto, ritorna Entita_Cod e la geometria in formato WKT associata.
    ''' </summary>
    Public Function LetturaGeometriaImpianto(
                                             piva As String,
                                             saCod As Integer,
                                             appezza As Integer,
                                             idImp As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Const routing = "AgronicaCoreGisDAL.GIS_EstrattoreGrafica_R.LetturaGeometriaImpianto()"
        Dim dataTable As DataTable

        Try
            Dim stb As New Text.StringBuilder
            stb.Length = 0

            stb.AppendLine("SELECT")
            stb.AppendLine("    a.Entita_Cod")
            stb.AppendLine(
                "  , COALESCE(a.Poligono_GeoEntity_WKT, a.Poligono_GeoEntity.STAsText()) AS Poligono_GeoEntity_WKT")
            stb.AppendLine("FROM GIS_ElementiGrafici a")
            stb.AppendLine("    INNER JOIN GIS_Entita b ON a.Entita_Cod = b.Entita_Cod")
            stb.AppendLine("WHERE b.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            stb.AppendLine("  AND b.Sa_Cod = " & saCod)
            stb.AppendLine("  AND b.Appezza = " & appezza)
            stb.AppendLine("  AND b.Id_Imp = " & idImp)

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine("  AND b.Inviato >= 0")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine("  AND b.Inviato = -1")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dataTable = EseguiQuery_Lettura(objParametri, stb.ToString, routing)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim errorMessage = ex.Message
            Scrivi_LOG(objParametri, routing, errorMessage)
            Throw New Exception("[" & routing & "] : " & errorMessage)
        End Try

        Return dataTable
    End Function
End Class




