

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiImpiantiConDatoGisSenzaOrigineAssociata(ByVal elencoSpecie As List(Of Integer), ByVal ValiditaInizioImp As Date, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT ")
            Stb.AppendLine("	TC1.piva ")
            Stb.AppendLine("	, TC1.SA_COD ")
            Stb.AppendLine("	, TC1.APPEZZA ")
            Stb.AppendLine("    , TC1.ID_REG ")
            Stb.AppendLine("    , 0 AS Tipo_sorgente ")
            Stb.AppendLine("    , 0 AS Stazione_Cod ")
            Stb.AppendLine("    , TC1.Coord.Lat AS Latitudine ")
            Stb.AppendLine("    , TC1.Coord.Long AS Longitudine ")
            Stb.AppendLine("FROM ( ")
            Stb.AppendLine()
            Stb.AppendLine("	SELECT ")
            Stb.AppendLine("		TC.piva ")
            Stb.AppendLine("		, TC.sa_cod ")
            Stb.AppendLine("		, TC.appezza ")
            Stb.AppendLine("		, TC.id_reg ")
            Stb.AppendLine("		, COALESCE(Coord1, Coord2, Coord3, geography::Point(0, 0, 4326)) as Coord ")
            Stb.AppendLine("	FROM ( ")
            Stb.AppendLine("		SELECT ")
            Stb.AppendLine("			i.piva ")
            Stb.AppendLine("			, i.SA_COD ")
            Stb.AppendLine("			, i.APPEZZA ")
            Stb.AppendLine("			, i.ID_REG ")
            Stb.AppendLine("			, e1.Coord AS Coord1 ")
            Stb.AppendLine("			, e2.Coord AS Coord2 ")
            Stb.AppendLine("			, e3.Coord AS Coord3 ")
            Stb.AppendLine("		FROM Reg_Impianti i ")
            Stb.AppendLine()
            Stb.AppendLine("		LEFT JOIN  ( --Impianti ")
            Stb.AppendLine("			SELECT e.Piva, e.Sa_Cod, e.Appezza, e.Id_Imp, g.Poligono_GeoEntity.EnvelopeCenter() AS Coord ")
            Stb.AppendLine("			FROM GIS_Entita e ")
            Stb.AppendLine("			INNER JOIN GIS_ElementiGrafici g ON g.Entita_Cod = e.Entita_Cod AND g.LayerElementiGrafici_cod = 19 ")
            Stb.AppendLine("		) e1 ON e1.Piva = i.PIVA AND e1.Sa_Cod = i.SA_COD AND e1.Appezza = i.APPEZZA AND e1.Id_Imp = i.ID_REG ")
            Stb.AppendLine()
            Stb.AppendLine("		LEFT JOIN  ( --Appezzamenti ")
            Stb.AppendLine("			SELECT e.Piva, e.Sa_Cod, e.Appezza, g.Poligono_GeoEntity.EnvelopeCenter() AS Coord ")
            Stb.AppendLine("			FROM GIS_Entita e ")
            Stb.AppendLine("			INNER JOIN GIS_ElementiGrafici g ON g.Entita_Cod = e.Entita_Cod AND g.LayerElementiGrafici_cod = 1 ")
            Stb.AppendLine("		) e2 ON e2.Piva = i.PIVA AND e2.Sa_Cod = i.SA_COD AND i.APPEZZA = e2.appezza ")
            Stb.AppendLine()
            Stb.AppendLine("		LEFT JOIN ( --Centri ")
            Stb.AppendLine("			SELECT PIVA, sa_cod, CASE WHEN (lat IS NULL OR long IS NULL) THEN NULL ELSE geography::Point(lat, long, 4326) END AS Coord ")
            Stb.AppendLine("			FROM Centri_Aziendali ")
            Stb.AppendLine("		) e3 ON e3.PIVA = i.PIVA and e3.sa_cod = i.SA_COD ")
            Stb.AppendLine()
            Stb.AppendLine("		INNER JOIN Cultivar cul ON cul.Cul_Cod = i.CUL_COD ")
            Stb.AppendLine("		INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = cul.Veg_Cod ")
            Stb.AppendLine()
            Stb.AppendLine("		WHERE 1 = 1 ")

            If elencoSpecie.Any Then

                Dim strLista As String = String.Join(", ", elencoSpecie)
                Stb.AppendLine("        AND sv.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(strLista) & ")")

            End If

            If ValiditaInizioImp > New Date(1900, 1, 1) Then

                Stb.AppendLine("        AND i.Validita_inizio <= " & UtilityProvider.Agro_SQL_SaveDate(ValiditaInizioImp) & " AND " & UtilityProvider.Agro_SQL_SaveDate(ValiditaInizioImp) & " <= i.Validita_fine ")

            End If

            Stb.AppendLine("		AND  i.Validita_inizio <=  CONVERT(DateTime,'2019/11/29',120)  AND  CONVERT(DateTime,'2019/11/29',120)  <= i.Validita_fine ")
            Stb.AppendLine()
            Stb.AppendLine("	) TC ")
            Stb.AppendLine()
            Stb.AppendLine("	LEFT JOIN DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo dm ON TC.PIVA = dm.piva AND TC.SA_COD = dm.sa_cod AND TC.APPEZZA = dm.appezza AND TC.ID_REG = dm.id_reg ")
            Stb.AppendLine()
            Stb.AppendLine("	WHERE dm.piva IS NULL ")
            Stb.AppendLine()
            Stb.AppendLine(") TC1 ")
            Stb.AppendLine()
            Stb.AppendLine("WHERE TC1.Coord.Lat <> 0 AND TC1.Coord.Long <> 0 ")


#If False Then

            Stb.AppendLine("SELECT ")
            Stb.AppendLine("    i.piva ")
            Stb.AppendLine("    , i.SA_COD  ")
            Stb.AppendLine("    , i.APPEZZA  ")
            Stb.AppendLine("    , i.ID_REG ")
            Stb.AppendLine("    , 0 as Tipo_sorgente ")
            Stb.AppendLine("    , 0 as Stazione_Cod ")
            Stb.AppendLine("    , Poligono_Centro.Lat as Latitudine ")
            Stb.AppendLine("    , Poligono_Centro.Long as Longitudine ")
            Stb.AppendLine("FROM( ")
            Stb.AppendLine()
            Stb.AppendLine("	SELECT")
            Stb.AppendLine("		i.piva ")
            Stb.AppendLine("		, i.SA_COD ")
            Stb.AppendLine("		, i.APPEZZA ")
            Stb.AppendLine("		, i.ID_REG ")
            Stb.AppendLine("		, g.Poligono_GeoEntity.EnvelopeCenter() AS Poligono_Centro ")
            Stb.AppendLine("	FROM Reg_Impianti i ")
            Stb.AppendLine("	INNER JOIN GIS_Entita e ON i.PIVA = e.piva AND i.SA_COD = e.sa_cod AND i.APPEZZA = e.appezza AND i.ID_REG = e.Id_Imp ")
            Stb.AppendLine("	INNER JOIN GIS_ElementiGrafici g ON g.Entita_Cod = e.Entita_Cod AND g.LayerElementiGrafici_cod = 19 ")
            Stb.AppendLine("	INNER JOIN Cultivar cul ON cul.Cul_Cod = i.CUL_COD ")
            Stb.AppendLine("	INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = cul.Veg_Cod ")
            Stb.AppendLine()
            Stb.AppendLine("	LEFT JOIN DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo dm ON i.PIVA = dm.piva AND i.SA_COD = dm.sa_cod AND i.APPEZZA = dm.appezza AND i.ID_REG = dm.id_reg ")
            Stb.AppendLine()
            Stb.AppendLine("    WHERE dm.piva IS NULL ")

            If ValiditaInizioImp > New Date(1900, 1, 1) Then

                Dim filtroAgg As String = " i.Validita_inizio <= " & UtilityProvider.Agro_SQL_SaveDate(ValiditaInizioImp)
                filtroAgg &= " AND " & UtilityProvider.Agro_SQL_SaveDate(ValiditaInizioImp) & " <= i.Validita_fine "
                Stb.AppendLine("    AND " & filtroAgg)

            End If

            If elencoSpecie.Any Then

                Dim strLista As String = String.Join(", ", elencoSpecie)
                Stb.AppendLine("    AND sv.Veg_Cod IN (" & strLista & ")")

            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   i.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   i.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Stb.AppendLine()
            Stb.AppendLine(" ) i ")

#End If

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



    '##############################################################################################
    Public Function LeggiElencoPerStazioneSpecie(ByVal elencoModelli As Integer(), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo_R.LeggiElenco()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            If elencoModelli.Length > 0 Then

                Stb.AppendLine("SELECT ")
                Stb.AppendLine("	TBL.tipo_sorgente ")
                Stb.AppendLine("	, TBL.Stazione_Cod ")
                Stb.AppendLine("	, TBL.Veg_Cod ")
                Stb.AppendLine("	, msa.Mod_Cod ")
                Stb.AppendLine("FROM ( ")
                Stb.AppendLine()

            End If

            Stb.AppendLine("SELECT DISTINCT ")
            Stb.AppendLine("	dss.tipo_sorgente ")
            Stb.AppendLine("	, dss.Stazione_Cod ")
            Stb.AppendLine("	, veg.Veg_Cod ")
            Stb.AppendLine("FROM DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo dss ")
            Stb.AppendLine("INNER JOIN Reg_Impianti imp ON imp.PIVA = dss.piva AND imp.SA_COD = dss.sa_cod AND imp.APPEZZA = dss.appezza AND imp.ID_REG = dss.id_reg ")
            Stb.AppendLine("INNER JOIN Cultivar cul ON cul.Cul_Cod = imp.CUL_COD ")
            Stb.AppendLine("INNER JOIN SpecieVegetali veg ON veg.Veg_Cod = cul.Veg_Cod ")

            If elencoModelli.Length > 0 Then

                Dim strLista As String = String.Join(", ", elencoModelli)

                Stb.AppendLine()
                Stb.AppendLine(") TBL ")
                Stb.AppendLine("INNER JOIN ModelliXSpeciexAvversita msa ON msa.Veg_Cod = TBL.Veg_Cod ")
                Stb.AppendLine("WHERE msa.Mod_Cod IN (" & Agro_SQL_Save_Clausola_IN(strLista) & ") ")
                Stb.AppendLine("ORDER BY tipo_sorgente, Stazione_Cod, Mod_Cod ")

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



    Public Function LeggiElencoPerStazioneSpecieDataSemina(ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo_R.LeggiElenco()"

        Dim DT As DataTable

        Try

            Dim Stb As New Text.StringBuilder

            Stb.Clear()

            Stb.AppendLine("SELECT DISTINCT ")
            Stb.AppendLine("	dss.tipo_sorgente ")
            Stb.AppendLine("	, dss.Stazione_Cod ")
            Stb.AppendLine("	, veg.Veg_Cod ")
            Stb.AppendLine("	, DataSeminaPrevista = iprog.Data_Inizio_Prevista ")
            Stb.AppendLine("FROM DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo dss ")
            Stb.AppendLine("INNER JOIN Reg_Impianti imp ON imp.PIVA = dss.piva AND imp.SA_COD = dss.sa_cod AND imp.APPEZZA = dss.appezza AND imp.ID_REG = dss.id_reg ")
            Stb.AppendLine("INNER JOIN Cultivar cul ON cul.Cul_Cod = imp.CUL_COD ")
            Stb.AppendLine("INNER JOIN SpecieVegetali veg ON veg.Veg_Cod = cul.Veg_Cod ")
            Stb.AppendLine("LEFT JOIN Imprese_Progetti iprog ON iprog.piva = imp.piva AND iprog.sa_cod = imp.sa_cod AND iprog.appezza = imp.appezza AND iprog.id_reg = imp.id_reg ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
              ByVal piva As String _
            , ByVal sa_cod As Integer _
            , ByVal appezza As Integer _
            , ByVal id_reg As Integer _
            , ByVal tipo_sorgente As Integer _
            , ByVal Stazione_Cod As Integer _
            , ByVal Stazione_Des As String _
            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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
        Dim strSQL As New System.Text.StringBuilder
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
            strSQL.Length = 0
            strSQL.Append(" INSERT DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo ( " + vbCrLf)

            strSQL.Append("   [Piva_SuperUser] " & vbCrLf)
            strSQL.Append("  ,[piva] " & vbCrLf)
            strSQL.Append("  ,[sa_cod] " & vbCrLf)
            strSQL.Append("  ,[appezza] " & vbCrLf)
            strSQL.Append("  ,[id_reg] " & vbCrLf)
            strSQL.Append("  ,[tipo_sorgente] " & vbCrLf)
            strSQL.Append("  ,[Stazione_Cod] " & vbCrLf)
            strSQL.Append("  ,[Stazione_Des] " & vbCrLf)


            strSQL.Append("              ")
            strSQL.Append("              , Inviato,            datainvio, ")
            strSQL.Append("              Data_Creazione,     Data_Modifica, ")
            strSQL.Append("              UserName_Creazione, UserName_Modifica ")
            strSQL.Append("              ) ")

            strSQL.Append(" VALUES ( ")

            strSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'" & vbCrLf)
            strSQL.Append(",'" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(sa_cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(appezza) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(id_reg) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(tipo_sorgente) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Stazione_Cod) & " " & vbCrLf)
            strSQL.Append(", '" & Agro_SQL_SaveText(Stazione_Des) & "' " & vbCrLf)


            strSQL.Append("         , 0  " + vbCrLf)
            strSQL.Append("         , Null  " + vbCrLf)

            strSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            strSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            strSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
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

