
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Gis_Sat_Sentinel_Search_CFG_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    Public Function LeggiConfigurazioniDaParametriUtente(
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine("  SET NOCOUNT ON ")
            Stb.AppendLine("  Declare @TabellaAppoggio TABLE(Tile VARCHAR(50), PoligonoIntersezione VARCHAR(MAX), AreaIntersezione float, gg_da Integer, gg_a Integer, piva_SuperUser varchar(50)) ")

            Stb.AppendLine(" INSERT INTO @TabellaAppoggio ")
            Stb.AppendLine(" Select ")
            Stb.AppendLine("          tt.Tile ")
            Stb.AppendLine("        , Poligono_GeoEntity.STAsText() AS PoligonoIntersezione ")
            Stb.AppendLine("     , GEOMETRY::STGeomFromWKB(Poligono_GeoEntity.STAsBinary(), 4326).STIntersection( tt.TileBoundaryGeom).STArea()         ")
            Stb.AppendLine("     , cfg.GG_da ")
            Stb.AppendLine("     , cfg.GG_a ")
            Stb.AppendLine("     , cfg.Piva_SuperUser ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" FROM Gis_Sat_Sentinel_User_Config cfg ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" INNER JOIN Gis_Sat_Sentinel_Overlay_Tile tt On GEOMETRY::STGeomFromWKB(Poligono_GeoEntity.STAsBinary(), 4326).STIntersects( tt.TileBoundaryGeom ) = 1 ")
            Stb.AppendLine(" where cfg.inviato = 0  ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Select tt.piva_SuperUser, t.Tile, tt.PoligonoIntersezione As PoligonoIntersezione, src.Gis_Sat_Sentinel_Search_CFG_COD, tt.gg_da, tt.gg_a ")
            Stb.AppendLine(" FROM ( ")
            Stb.AppendLine("  Select piva_SuperUser, tile, gg_da, gg_a, MAX(AreaIntersezione) As AreaIntersezione FROM @TabellaAppoggio GROUP BY Tile, gg_da, gg_a, piva_SuperUser ")
            Stb.AppendLine(" ) t ")
            Stb.AppendLine(" inner join @TabellaAppoggio tt ")
            Stb.AppendLine("  On t.tile = tt.tile ")
            Stb.AppendLine("  And t.AreaIntersezione = tt.AreaIntersezione ")
            Stb.AppendLine("  And t.gg_Da = tt.gg_da ")
            Stb.AppendLine("  And t.gg_a = tt.gg_a ")
            Stb.AppendLine("  And t.piva_SuperUser = tt.piva_SuperUser  ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" LEFT JOIN Gis_Sat_Sentinel_Search_CFG src On src.Tile = tt.Tile ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" order by PoligonoIntersezione, Tile, GG_da, gg_a")



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Public Function LeggiConfigurazioniAttive(
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select isNull(cfg.Gis_Sat_Sentinel_Search_CFG_Des, '') as Gis_Sat_Sentinel_Search_CFG_Des, cfg.Poligono_GeoEntity.STAsText() As Ritaglio, gg_da, gg_A, isNull(OrbitaRelativa, -1) as OrbitaRelativa, isnull(PercentualeCoperturaNuvole, '' ) as PercentualeCoperturaNuvole ")
            Stb.AppendLine(" From Gis_Sat_Sentinel_Search_CFG cfg ")
            Stb.AppendLine(" where 1=1 ")



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Gis_Sat_Sentinel_Search_CFG_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
        ByVal Gis_Sat_Sentinel_Search_CFG_COD As Integer,
        ByVal Gis_Sat_Sentinel_Search_CFG_DES As String,
        ByVal PoligonoWKT As String,
        ByVal Coord_AltoSinistra_BassoDestra As String,
        ByVal tile As String,
        ByVal StatoElaborazione As Integer,
        ByVal GG_DA As Integer,
        ByVal GG_A As Integer,
        ByVal OrbitaRelativa As Integer,
        ByVal PercentualeCoperturaNuvole As String,
        ByVal FlagAttivo As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = ""
        ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT Gis_Sat_Sentinel_Search_CFG " + vbCrLf)

            Stb.Append("              (")

            Stb.AppendLine("         Gis_Sat_Sentinel_Search_CFG_COD , ")
            Stb.AppendLine("         Gis_Sat_Sentinel_Search_CFG_DES, ")
            Stb.AppendLine("         Poligono_GeoEntity, ")
            Stb.AppendLine("         Coord_AltoSinistra_BassoDestra, ")
            Stb.AppendLine("         tile, ")
            Stb.AppendLine("         StatoElaborazione, ")
            Stb.AppendLine("         GG_DA, ")
            Stb.AppendLine("         GG_A, ")

            If OrbitaRelativa > 0 Then
                Stb.AppendLine("         OrbitaRelativa, ")
            End If

            If PercentualeCoperturaNuvole <> "" Then
                Stb.AppendLine("         PercentualeCoperturaNuvole,")
            End If


            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            Stb.Append("			 " & Agro_SQL_SaveNum(Gis_Sat_Sentinel_Search_CFG_COD) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(Gis_Sat_Sentinel_Search_CFG_DES) & "' ")
            Stb.Append("			, " & Agro_SQL_SaveGeograpyFromWKTString(PoligonoWKT) & " ")
            Stb.Append("			,'" & Agro_SQL_SaveText(Coord_AltoSinistra_BassoDestra) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(tile) & "' ")
            Stb.Append("			, " & Agro_SQL_SaveNum(StatoElaborazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveNum(GG_DA) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveNum(GG_A) & "  ")

            If OrbitaRelativa > 0 Then
                Stb.Append("			, " & Agro_SQL_SaveNum(OrbitaRelativa) & "  ")
            End If

            If PercentualeCoperturaNuvole <> "" Then
                Stb.Append("			,'" & Agro_SQL_SaveText(PercentualeCoperturaNuvole) & "' ")
            End If


            Stb.Append("         ,   " & FlagAttivo & vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            Stb.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")



            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class
