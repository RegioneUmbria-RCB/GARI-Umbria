
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Gis_Sat_Sentinel_Overlay_R
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' <summary>
    ''' Legge i dati circa i passaggi satellitari memorizzati sul tile che interseca il Poligono passato come parametro
    ''' </summary>
    ''' <param name="PoligonoWkt">esempio: POINT(12.197024233234742 44.197172391184296)</param>
    ''' <returns></returns>
    Public Function Leggi(
        ByVal PoligonoWkt As String,
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

            Stb.AppendLine(" SELECT  ")
            Stb.AppendLine("    convert(varchar(50), p.DataRiferimento, 103) as DataRiferimento ")
            Stb.AppendLine("  , p.Tile ")
            Stb.AppendLine("  , p.[url] ")
            Stb.AppendLine("  , s.Sensore ")
            Stb.AppendLine("  , p.DataRiferimento as p1 ")

            If Not String.IsNullOrEmpty(PoligonoWkt) Then
                Stb.AppendLine("  , tt.GEORiferimento_COD ")
            Else
                Stb.AppendLine("  , '' as GEORiferimento_COD ")
            End If

            Stb.AppendLine(" FROM [dbo].[Gis_Sat_Sentinel_Overlay_Passaggio] p ")

            Stb.AppendLine("  INNER JOIN Gis_Sat_Sentinel_Overlay_Passaggio_Sensori s ")
            Stb.AppendLine("         On s.Gis_Sat_Sentinel_Overlay_Passaggio_COD = p.Gis_Sat_Sentinel_Overlay_Passaggio_COD ")

            Stb.AppendLine("  INNER JOIN Gis_Sat_Sentinel_Overlay_Tile_Xpiva_superUser_OperazioniAutorizzate aut ")
            Stb.AppendLine("         On  aut.Tile = p.Tile  ")

            If Not String.IsNullOrEmpty(PoligonoWkt) Then
                Stb.AppendLine("  INNER Join Gis_Sat_Sentinel_Overlay_Tile tt ")
                Stb.AppendLine("         On 'T' + tt.Tile = p.Tile ")
                Stb.AppendLine("  WHERE geometry::STGeomFromText('" & Agro_SQL_SaveText(PoligonoWkt) & "', 4326).STIntersects(  tt.TileBoundaryGeom ) = 1")
                Stb.AppendLine("  AND aut.piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
                Stb.AppendLine("  AND aut.Tipo_Visibilita = " & enum_Sentinel2_cfgFlagTipoVisibilita.Attivo & " ")
            Else
                Stb.AppendLine("  WHERE aut.piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
                Stb.AppendLine("  AND aut.Tipo_Visibilita = " & enum_Sentinel2_cfgFlagTipoVisibilita.Attivo & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
            Else
                Stb.AppendLine(" ORDER BY p1 DESC, p.tile, s.sensore ")
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

    ''' <summary>
    ''' Legge i dati circa i file zip da elaborare
    ''' </summary>    
    ''' <returns></returns>
    Public Function leggiZipDaElaborare(
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

            'tutte le configurazioni non elaborate
            'Stb.AppendLine(" Select ")
            'Stb.AppendLine("       STRING_AGG(ss.sensore, ',') as sensore  ")
            'Stb.AppendLine("     , STRING_AGG(ss.Sensore_Configurazione, ',') as Sensore_Configurazione  ")
            'Stb.AppendLine("     , p.Gis_Sat_Sentinel_Overlay_Passaggio_COD ")
            'Stb.AppendLine("     , p.url as nomeCartella")
            'Stb.AppendLine(" From Gis_Sat_Sentinel_Sensors ss ")
            'Stb.AppendLine("  inner Join [dbo].[Gis_Sat_Sentinel_Overlay_Passaggio] p ")
            'Stb.AppendLine("          On 1=1 ")
            'Stb.AppendLine("  Left Join [dbo].[Gis_Sat_Sentinel_Overlay_Passaggio_Sensori] s ")
            'Stb.AppendLine("         On p.Gis_Sat_Sentinel_Overlay_Passaggio_COD = s.Gis_Sat_Sentinel_Overlay_Passaggio_COD ")
            'Stb.AppendLine("      And s.Sensore = ss.Sensore ")
            'Stb.AppendLine(" where s.Gis_Sat_Sentinel_Overlay_Passaggio_COD Is null ")

            Stb.AppendLine("SELECT ")
            Stb.AppendLine("	sensore = STRING_AGG(ss.sensore, ',')")
            Stb.AppendLine("	, Sensore_Configurazione = STRING_AGG(ss.Sensore_Configurazione, ',') ")
            Stb.AppendLine("	, p.Gis_Sat_Sentinel_Overlay_Passaggio_COD ")
            Stb.AppendLine("	, nomeCartella = p.[url] ")
            Stb.AppendLine("	, Sorgente_LivelliZoom = scfg.[Sorgente_LivelliZoom] ")
            Stb.AppendLine("FROM Gis_Sat_Sentinel_Sensors ss ")
            Stb.AppendLine("INNER JOIN [dbo].[Gis_Sat_Sentinel_Sensors_XSorgente] sxs on sxs.Sensore = ss.Sensore ")
            Stb.AppendLine("INNER JOIN [dbo].[Gis_Sat_Sentinel_Overlay_Passaggio] p On 1=1 ")
            Stb.AppendLine("INNER JOIN [dbo].[Gis_Sat_Sentinel_Search_CFG] scfg on 'T' + scfg.Tile = p.Tile and sxs.Sorgente_cod = scfg.Sorgente_Cod ")
            Stb.AppendLine("LEFT JOIN [dbo].[Gis_Sat_Sentinel_Overlay_Passaggio_Sensori] s ")
            Stb.AppendLine("	On p.Gis_Sat_Sentinel_Overlay_Passaggio_COD = s.Gis_Sat_Sentinel_Overlay_Passaggio_COD  And s.Sensore = ss.Sensore ")
            Stb.AppendLine("WHERE s.Gis_Sat_Sentinel_Overlay_Passaggio_COD IS NULL ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Stb.AppendLine(" GROUP BY p.[url], p.Gis_Sat_Sentinel_Overlay_Passaggio_COD, Sorgente_LivelliZoom  ")
            Stb.AppendLine(" ")



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


    ''' <summary>
    ''' Legge i dati circa i file zip da scaricare
    ''' </summary>    
    ''' <returns></returns>
    Public Function leggiZipDaScaricare(
        ByVal urlDaRicercare As String,
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

            'tutte le configurazioni non elaborate
            Stb.AppendLine(" Select 1 ")
            Stb.AppendLine(" From Gis_Sat_Sentinel_Overlay_Passaggio p  ")
            Stb.AppendLine(" where 1=1 ")
            If urlDaRicercare <> "" Then
                Stb.AppendLine(" and  p.url = '" & Agro_SQL_SaveText(urlDaRicercare) & "'  ")
            End If


            Stb.AppendLine(" ")



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


    ''' <summary>
    ''' Legge i dati circa i ritagli elaborati (e non) per i passaggi satellitari memorizzati sul tile
    ''' </summary>    
    ''' <returns></returns>
    Public Function leggiRitagliDaElaborare(
        ByVal RaggruppaRisultato As Boolean,
        ByVal DataRiferimento As Date,
        ByVal FiltroElaborazioni As enum_Sentinel2_cfgFlagStatoElaborazioni,
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

            If RaggruppaRisultato Then
                Stb.AppendLine("    Select ")
                Stb.AppendLine("    Gis_Sat_Sentinel_User_Config_COD ")
                Stb.AppendLine("  , DataRiferimentoElaborazioni ")
                Stb.AppendLine("  , Stato ")
                Stb.AppendLine("  , COUNT(*) as conteggio ")
                Stb.AppendLine(" from ( ")
            End If

            Stb.AppendLine("  Select ")
            Stb.AppendLine("     El.* ")
            Stb.AppendLine("  , cfg.Gis_Sat_Sentinel_User_Config_COD ")
            Stb.AppendLine("  , cfg.GG_da ")
            Stb.AppendLine("  , cfg.GG_a ")
            Stb.AppendLine("  , geometry::STGeomFromText ( cfg.Poligono_GeoEntity.STAsText() , 4326 ).STEnvelope().STAsText() as PoligonoWKT ")
            Stb.AppendLine("  , cfg.ParametriAggiuntivi ")
            Stb.AppendLine("  , cfg.Piva_SuperUser ")

            Stb.AppendLine("  , isnull(cElaborate.DataRiferimento, '01/01/1900') as DataRiferimentoElaborazioni ")
            Stb.AppendLine("  , case when cElaborate.DataRiferimento Is null then " & enum_Sentinel2_cfgFlagStatoElaborazioni.NonElaborato & " else case when cElaborate.DataRiferimento > " & Agro_SQL_SaveDate(DataRiferimento) & " then " & enum_Sentinel2_cfgFlagStatoElaborazioni.ElaboratoInDataSuccessiva & " else " & enum_Sentinel2_cfgFlagStatoElaborazioni.Elaborato & " end end as Stato")

            Stb.AppendLine("  ")
            Stb.AppendLine(" from( ")
            Stb.AppendLine("     select  ")
            Stb.AppendLine("        p.DataRiferimento ")
            Stb.AppendLine("      , url as NomeCartella ")
            Stb.AppendLine("      , s.Sensore ")
            Stb.AppendLine("      , p.Gis_Sat_Sentinel_Overlay_Passaggio_COD       ")
            Stb.AppendLine("      , P.tile ")
            Stb.AppendLine("      , scfg.Sorgente_Cod ")
            Stb.AppendLine("  From Gis_Sat_Sentinel_Overlay_Passaggio_Sensori s ")
            Stb.AppendLine("      inner Join Gis_Sat_Sentinel_Overlay_Passaggio p ")
            Stb.AppendLine("             On s.Gis_Sat_Sentinel_Overlay_Passaggio_COD = p.Gis_Sat_Sentinel_Overlay_Passaggio_COD ")
            Stb.AppendLine("      INNER JOIN [dbo].[Gis_Sat_Sentinel_Sensors_XSorgente] sxs on sxs.Sensore = s.Sensore ")
            Stb.AppendLine("      INNER JOIN [dbo].[Gis_Sat_Sentinel_Search_CFG] scfg on 'T' + scfg.Tile = p.Tile and sxs.Sorgente_cod = scfg.Sorgente_Cod ")
            Stb.AppendLine(" ) El ")
            Stb.AppendLine(" inner Join Gis_Sat_Sentinel_User_Config cfg ")
            Stb.AppendLine("     On 1=1 ")

            ' VAnni: 5/2/2019: l'incrocio con i tile è comunque indispensabile
            ' VAnni: 4/11/2019: uso una sotto-query per velocizzare l'operazione STIntersects 
            'If FiltroElaborazioni = enum_Sentinel2_cfgFlagStatoElaborazioni.NonElaborato Then
            Stb.AppendLine(" INNER Join ( ")
            Stb.AppendLine("      select t.tile, tileBoundaryGeom  ")
            Stb.AppendLine("         From Gis_Sat_Sentinel_Overlay_Tile t ")
            Stb.AppendLine("          inner Join Gis_Sat_Sentinel_Search_CFG p ")
            Stb.AppendLine("                 On  t.Tile = p.Tile       ")
            Stb.AppendLine("      ) tt ")
            Stb.AppendLine("         On 'T' + tt.Tile = El.Tile  ")
            Stb.AppendLine("     And GEOMETRY::STGeomFromWKB(cfg.Poligono_GeoEntity.STAsBinary(), 4326).STIntersects(tt.TileBoundaryGeom) = 1")

            Stb.AppendLine(" ")
            'End If

            Stb.AppendLine("  ")
            Stb.AppendLine(" Left Join Gis_Sat_Sentinel_Overlay_Passaggio_Sensori_User_Config cElaborate ")
            Stb.AppendLine("     On cElaborate.Gis_Sat_Sentinel_Overlay_Passaggio_COD = El.Gis_Sat_Sentinel_Overlay_Passaggio_COD ")
            Stb.AppendLine("  And cElaborate.Sensore = El.Sensore ")
            Stb.AppendLine("  And cElaborate.Gis_Sat_Sentinel_User_Config_COD = cfg.Gis_Sat_Sentinel_User_Config_COD ")
            Stb.AppendLine("  ")
            Dim sWhere As String = " where "

            If FiltroElaborazioni = enum_Sentinel2_cfgFlagStatoElaborazioni.NonElaborato Then
                Stb.AppendLine(sWhere & " cElaborate.Gis_Sat_Sentinel_Overlay_Passaggio_COD Is null ")
                sWhere = " and "
            End If

            Stb.AppendLine(sWhere & "  El.Sensore <> 'RGB'")


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   cfg.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   cfg.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            If RaggruppaRisultato Then
                Stb.Append(" ) elaborazioni ")
                Stb.AppendLine(" group by Gis_Sat_Sentinel_User_Config_COD, DataRiferimentoElaborazioni, Stato")
                Stb.AppendLine(" order by Gis_Sat_Sentinel_User_Config_COD, DataRiferimentoElaborazioni, conteggio desc")
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

Public Class Gis_Sat_Sentinel_Overlay_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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
            Stb.Append(" INSERT ... " + vbCrLf)

            Stb.Append("              (")
            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine, ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")



            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



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

