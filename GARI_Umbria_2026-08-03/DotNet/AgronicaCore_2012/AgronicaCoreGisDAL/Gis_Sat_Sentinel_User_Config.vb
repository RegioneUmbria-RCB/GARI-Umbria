
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework_POCO

Public Class Gis_Sat_Sentinel_User_Config_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function IntersezioneMappa(ByRef User_Config_Codes As List(Of Integer), ByVal Piva_SuperUser As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Gis_Sat_Sentinel_User_Config_R.IntersezioneMappa()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine("SET NOCOUNT ON")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("DECLARE @TabellaAppoggio TABLE(Tile VARCHAR(50), PoligonoIntersezione VARCHAR(MAX))")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("INSERT INTO @TabellaAppoggio")
            StrSQL.AppendLine("     SELECT tt.Tile, Poligono_GeoEntity.STAsText() AS PoligonoIntersezione")
            StrSQL.AppendLine("     FROM (")
            StrSQL.AppendLine("         SELECT *")
            StrSQL.AppendLine("         FROM Gis_Sat_Sentinel_User_Config")
            StrSQL.AppendLine("         WHERE Gis_Sat_Sentinel_User_Config_COD in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", User_Config_Codes)) & ")")
            StrSQL.AppendLine("     ) cfg")
            StrSQL.AppendLine("     INNER JOIN Gis_Sat_Sentinel_Overlay_Tile tt")
            StrSQL.AppendLine("         ON GEOMETRY::STGeomFromWKB(Poligono_GeoEntity.STAsBinary(), 4326).STIntersects( tt.TileBoundaryGeom ) = 1")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT t.Tile, t.PoligonoIntersezione")
            StrSQL.AppendLine("FROM (")
            StrSQL.AppendLine("     SELECT Tile, MAX(PoligonoIntersezione) AS PoligonoIntersezione FROM @TabellaAppoggio GROUP BY Tile")
            StrSQL.AppendLine(") t")
            StrSQL.AppendLine("INNER JOIN Gis_Sat_Sentinel_Search_CFG src ON src.Tile = t.Tile")
            StrSQL.AppendLine("INNER JOIN Gis_Sat_Sentinel_Overlay_Tile_Xpiva_superUser_OperazioniAutorizzate aut ON aut.Tile = 'T' + t.Tile")
            StrSQL.AppendLine("WHERE aut.piva_superUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' AND aut.Tipo_Visibilita = 1")

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

End Class

Public Class Gis_Sat_Sentinel_User_Config_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal User_Config_Des As String,
                           ByVal Gis_Sat_Sentinel_Search_CFG_COD As Integer,
                           ByVal Piva_SuperUser As String,
                           ByVal Gis_Entita_Cod As Integer,
                           ByVal Text_Poligono_GeoEntity As String,
                           ByVal GG_da As Integer,
                           ByVal GG_a As Integer,
                           ByVal ParametriAggiuntivi As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal inviato As Integer = 0,
                           Optional ByVal DataInvio As DateTime? = Nothing,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "") As Integer

        Dim NomeRoutine As String = "Gis_Sat_Sentinel_User_Config_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xCod As Integer = -1

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

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            'xCod = objSeq.Agronica_SequenzaTabelle_NuovoID("Gis_Sat_Sentinel_User_Config", objParametri)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            xCod = objSeq.NuovoId_Tabella("Gis_Sat_Sentinel_User_Config", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)

            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine("DECLARE @GEO GEOGRAPHY ")
            Stb.AppendLine("SET @GEO = GEOGRAPHY::STGeomFromText('" & Agro_SQL_SaveText(Text_Poligono_GeoEntity) & "', 4326);")
            Stb.AppendLine("")
            Stb.AppendLine("INSERT INTO Gis_Sat_Sentinel_User_Config (")
            Stb.AppendLine("    Gis_Sat_Sentinel_User_Config_COD")
            Stb.AppendLine("    , Gis_Sat_Sentinel_User_Config_Des")
            If Gis_Sat_Sentinel_Search_CFG_COD >= 0 Then
                Stb.AppendLine("    , Gis_Sat_Sentinel_Search_CFG_COD")
            End If
            Stb.AppendLine("    , Piva_SuperUser")
            Stb.AppendLine("    , Gis_Entita_Cod")
            Stb.AppendLine("    , Poligono_GeoEntity")
            Stb.AppendLine("    , GG_da")
            Stb.AppendLine("    , GG_a")
            Stb.AppendLine("    , ParametriAggiuntivi")
            Stb.AppendLine("    , inviato")
            If DataInvio IsNot Nothing Then
                Stb.AppendLine("    , datainvio")
            End If
            Stb.AppendLine("    , Data_Creazione")
            Stb.AppendLine("    , Data_Modifica")
            Stb.AppendLine("    , Username_Creazione")
            Stb.AppendLine("    , Username_Modifica")
            Stb.AppendLine("    , Validita_Inizio")
            Stb.AppendLine("    , Validita_Fine")
            Stb.AppendLine(")")
            Stb.AppendLine("VALUES (")
            Stb.AppendLine("    " & Agro_SQL_SaveNum(xCod))
            Stb.AppendLine("    , '" & Agro_SQL_SaveText(User_Config_Des) & "'")
            If Gis_Sat_Sentinel_Search_CFG_COD >= 0 Then
                Stb.AppendLine("    , " & Agro_SQL_SaveNum(Gis_Sat_Sentinel_Search_CFG_COD))
            End If
            Stb.AppendLine("    , '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
            Stb.AppendLine("    , " & Agro_SQL_SaveNum(Gis_Entita_Cod))
            Stb.AppendLine("    , @GEO")
            Stb.AppendLine("    , " & Agro_SQL_SaveNum(GG_da))
            Stb.AppendLine("    , " & Agro_SQL_SaveNum(GG_a))
            Stb.AppendLine("    , '" & Agro_SQL_SaveText(ParametriAggiuntivi) & "'")
            Stb.AppendLine("    , " & Agro_SQL_SaveNum(inviato))
            If DataInvio IsNot Nothing Then
                Stb.AppendLine("    , " & Agro_SQL_SaveDate(DataInvio))
            End If
            Stb.AppendLine("    , " & Agro_SQL_SaveDate(Data_creazione))
            Stb.AppendLine("    , " & Agro_SQL_SaveDate(Data_modifica))
            Stb.AppendLine("    ,'" & Agro_SQL_SaveText(username_creazione) & "'")
            Stb.AppendLine("    ,'" & Agro_SQL_SaveText(username_modifica) & "'")
            Stb.AppendLine("    , " & Agro_SQL_SaveDate(AGRODATAINIZIO))
            Stb.AppendLine("    , " & Agro_SQL_SaveDate(AGRODATAFINE))
            Stb.AppendLine(")")

            '--------------------------------------------------------------------------
            If Not EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine) Then
                xCod = -1
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xCod = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xCod

    End Function


End Class
