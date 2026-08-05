Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class WidgetIndiciProduttivita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function readSpecificIndiciProduttivita(
                                                  ByVal piva As String,
                                                  ByVal year As Integer,
                                                  ByVal vegCod As Integer,
                                                  ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.WidgetIndiciProduttivita_R.readSpecificIndiciProduttivita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim dataInizio = New Date(year, 1, 1)
        Dim dataFine = New Date(year, 12, 31)

        Try
            'StrSQL.AppendLine("WITH Impianti AS (")
            'StrSQL.AppendLine("    SELECT TOP 1 ")
            'StrSQL.AppendLine("        ri.PIVA")
            'StrSQL.AppendLine("        ,ri.APPEZZA")
            'StrSQL.AppendLine("        ,ri.SA_COD")
            'StrSQL.AppendLine("        ,ri.ID_REG")
            'StrSQL.AppendLine("        ,ri.Sup_Imp")

            'If vegCod <> 0 Then
            '    StrSQL.AppendLine("        ,C.Cul_Cod")
            '    StrSQL.AppendLine("        ,C.Cul_Des")
            '    StrSQL.AppendLine("        ,sv.Veg_Cod")
            '    StrSQL.AppendLine("        ,sv.Veg_Des")
            'Else
            '    StrSQL.AppendLine("        ,0 AS Cul_Cod")
            '    StrSQL.AppendLine("        ,'' AS Cul_Des")
            '    StrSQL.AppendLine("        ,0 AS Veg_Cod")
            '    StrSQL.AppendLine("        ,'' AS Veg_Des")
            'End If

            'StrSQL.AppendLine("")
            'StrSQL.AppendLine("    FROM Reg_Impianti ri")

            'If vegCod <> 0 Then
            '    StrSQL.AppendLine("        INNER JOIN Cultivar c ON c.Cul_Cod = ri.CUL_COD")
            '    StrSQL.AppendLine("        INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = c.Veg_Cod")
            'End If

            'StrSQL.AppendLine("    WHERE 1 = 1")

            'If vegCod <> 0 Then
            '    StrSQL.AppendLine("        AND sv.Veg_Cod = " & Agro_SQL_SaveNum(vegCod))
            'End If

            'StrSQL.AppendLine(")")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine("SELECT TOP 1 ")
            StrSQL.AppendLine("      0 as Cul_Cod")
            StrSQL.AppendLine("    , '' as Cul_Des")
            StrSQL.AppendLine("    , 0 as Veg_Cod")
            StrSQL.AppendLine("    , '' as Veg_Des")
            StrSQL.AppendLine("    ,i.Sup_Imp")
            StrSQL.AppendLine("    ,ind.IndiciProduttivitaAi_Cod")
            StrSQL.AppendLine("    ,ind.Produttivita")
            StrSQL.AppendLine("    ,ind.PLV")
            StrSQL.AppendLine("    ,ind.IndiceErosione")
            StrSQL.AppendLine("    ,ind.IndiceCO2")
            StrSQL.AppendLine("    ,ind.IndiceRischioMeteoAggregato as IndiceRischioMeteoAggregato")
            StrSQL.AppendLine("    ,ind.IndiceRischioGelata as IndiceRischioGelata")
            StrSQL.AppendLine("    ,ind.IndiceRischioVentoForte as IndiceRischioVentoForte")
            StrSQL.AppendLine("    ,ind.IndiceRischioSiccita as IndiceRischioSiccita")
            StrSQL.AppendLine("    ,ind.IndiceRischioGrandine as IndiceRischioGrandine")
            StrSQL.AppendLine("    ,ind.IndiceRischioAllagamento as IndiceRischioAllagamento")
            StrSQL.AppendLine("FROM IndiciProduttivitaAi AS ind")
            StrSQL.AppendLine("    INNER JOIN Reg_Impianti_XIndiciProduttivitaAi ixi ON ixi.IndiciProduttivitaAi_COD = ind.IndiciProduttivitaAi_COD")
            StrSQL.AppendLine("    INNER JOIN Reg_Impianti i ON i.PIVA = ixi.piva")
            StrSQL.AppendLine("        AND i.APPEZZA = ixi.appezza")
            StrSQL.AppendLine("        AND i.SA_COD= ixi.sa_cod")
            StrSQL.AppendLine("        AND i.ID_REG = ixi.id_reg")
            StrSQL.AppendLine("WHERE ind.Validita_Inizio <= " & Agro_SQL_SaveDate(dataFine))
            StrSQL.AppendLine("    AND ind.Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("    AND i.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine("")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function readGeneralIndiciProduttivita(
                                                 ByVal regione As String,
                                                 ByVal year As Integer,
                                                 ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.WidgetIndiciProduttivita_R.readGeneralIndiciProduttivita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim dataInizio = New Date(year, 1, 1)
        Dim dataFine = New Date(year, 12, 31)

        Try

            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    cast(ixi.piva as integer) as IndiciProduttivitaAi_COD ")
            StrSQL.AppendLine("    , ai.Produttivita")
            StrSQL.AppendLine("    , ai.PLV")
            StrSQL.AppendLine("    , ai.IndiceErosione")
            StrSQL.AppendLine("    , ai.IndiceCO2")
            StrSQL.AppendLine("    , ai.IndiceRischioMeteoAggregato as IndiceRischioMeteoAggregato")
            StrSQL.AppendLine("    , ai.IndiceRischioGelata as IndiceRischioGelata")
            StrSQL.AppendLine("    , ai.IndiceRischioVentoForte as IndiceRischioVentoForte")
            StrSQL.AppendLine("    , ai.IndiceRischioSiccita as IndiceRischioSiccita")
            StrSQL.AppendLine("    , ai.IndiceRischioGrandine as IndiceRischioGrandine")
            StrSQL.AppendLine("    , ai.IndiceRischioAllagamento as IndiceRischioAllagamento")
            StrSQL.AppendLine("FROM Reg_Impianti_XIndiciProduttivitaAi ixi")
            StrSQL.AppendLine("    INNER JOIN  IndiciProduttivitaAi ai ON ixi.IndiciProduttivitaAi_COD = ai.IndiciProduttivitaAi_COD")
            StrSQL.AppendLine("        AND (ixi.piva = '" & Agro_SQL_SaveText((-CType(regione, Integer)).ToString()) & "' OR ixi.piva in ('-30'))")
            StrSQL.AppendLine("WHERE ai.Validita_Inizio <= " & Agro_SQL_SaveDate(dataFine))
            StrSQL.AppendLine("    AND ai.Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function readDatiImpresaIndiceProduttivita(
                                                     ByVal piva As String,
                                                     ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.WidgetIndiciProduttivita_R.readDatiImpresaIndiceProduttivita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine("SELECT i.PIVA,")
            StrSQL.AppendLine("    i.rag_soc,")
            StrSQL.AppendLine("    ic.val_cod AS cuaa,")
            StrSQL.AppendLine("    lp.REG AS regioneCod,")
            StrSQL.AppendLine("    lr.Regione_Des AS regioneDes")
            StrSQL.AppendLine("FROM Imprese i")
            StrSQL.AppendLine("    LEFT JOIN Imprese_Codici ic ON ic.PIVA = i.PIVA")
            StrSQL.AppendLine("        AND ic.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.CodiceCUAA))
            StrSQL.AppendLine("    INNER JOIN ImpresexIndirizzi ixi ON ixi.PIVA = i.PIVA")
            StrSQL.AppendLine("    INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ixi.cod_indirizzo")
            StrSQL.AppendLine("    INNER JOIN Lista_Province lp ON lp.PROV = ind.pro_cod_istat")
            StrSQL.AppendLine("    INNER JOIN Lista_Regioni lr ON lr.REG = lp.REG")
            StrSQL.AppendLine("WHERE i.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine("")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function readAvailableYears(
                                      ByVal piva As String,
                                      ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.WidgetIndiciProduttivita_R.readAvailableYears()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine("SELECT DISTINCT")
            StrSQL.AppendLine("    i.Validita_Inizio")
            StrSQL.AppendLine("    ,i.Validita_Fine")
            StrSQL.AppendLine("FROM IndiciProduttivitaAi i")
            StrSQL.AppendLine("    INNER JOIN Reg_Impianti_XIndiciProduttivitaAi ixi ON ixi.IndiciProduttivitaAi_COD = i.IndiciProduttivitaAi_COD")
            StrSQL.AppendLine("WHERE i.IndiciProduttivitaAi_COD > 0 ")
            StrSQL.AppendLine("    AND ixi.piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine("")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function readKPI(
                           ByVal piva As String,
                           ByVal year As Integer,
                           ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.WidgetIndiciProduttivita_R.readKPI()"

        Dim dataInizio = New Date(year, 1, 1)
        Dim dataFine = New Date(year, 12, 31)

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine("SELECT TOP 1")
            StrSQL.AppendLine("    rai.Validita_Inizio")
            StrSQL.AppendLine("    ,rai.Validita_Fine")
            StrSQL.AppendLine("    ,ai.IndiciProduttivitaAi_COD")

            StrSQL.AppendLine("    ,ai.Produttivita")
            StrSQL.AppendLine("    ,minVal.minProduttivita")
            StrSQL.AppendLine("    ,maxVal.maxProduttivita")
            StrSQL.AppendLine("    ,case when (maxVal.MaxProduttivita =0 and minVal.minProduttivita=0 ) then 0 else ROUND ( ( ( (ai.Produttivita - minVal.minProduttivita) / (maxVal.MaxProduttivita - minVal.minProduttivita) ) * 5) + 1, 0) end AS indiceProduttivitaNN")

            StrSQL.AppendLine("    ,ai.PLV")
            StrSQL.AppendLine("    ,minVal.minPlv")
            StrSQL.AppendLine("    ,maxVal.maxPlv")
            StrSQL.AppendLine("    ,case when (maxVal.maxPlv =0 and minVal.minPlv=0 ) then 0 else ROUND ( ( ( (ai.Plv - minVal.minPlv) / (maxVal.maxPlv - minVal.minPlv) ) * 5) + 1, 0) end AS indicePlvNN")

            StrSQL.AppendLine("    ,ai.IndiceCO2")
            StrSQL.AppendLine("    ,minVal.minIndiceCO2")
            StrSQL.AppendLine("    ,maxVal.maxIndiceCO2")
            StrSQL.AppendLine("    ,case when (maxVal.maxIndiceCO2 =0 and minVal.minIndiceCO2=0 ) then 0 else ROUND ( ( ( (ai.IndiceCO2 - minVal.minIndiceCO2) / (maxVal.maxIndiceCO2 - minVal.minIndiceCO2) ) * 5) + 1, 0) end AS indiceCO2NN")

            StrSQL.AppendLine("    ,ai.IndiceErosione")
            StrSQL.AppendLine("    ,minVal.minIndiceErosione")
            StrSQL.AppendLine("    ,maxVal.maxIndiceErosione")
            StrSQL.AppendLine("    ,case when (maxVal.maxIndiceErosione =0 and minVal.minIndiceErosione=0 ) then 0 else ROUND ( ( ( (ai.IndiceErosione - minVal.minIndiceErosione) / (maxVal.maxIndiceErosione- minVal.minIndiceErosione) ) * 5) + 1, 0) end AS indiceErosioneNN")

            StrSQL.AppendLine("    ,ai.IndiceRischioMeteoAggregato AS IndiceRischioMeteoAggregato")
            StrSQL.AppendLine("    ,minVal.minIndiceRischioMeteoAggregato")
            StrSQL.AppendLine("    ,maxVal.maxIndiceRischioMeteoAggregato")
            StrSQL.AppendLine("    ,case when (maxVal.maxIndiceRischioMeteoAggregato =0 and minVal.minIndiceRischioMeteoAggregato=0 ) then 0 else ROUND ( ai.IndiceRischioMeteoAggregato, 0) end AS indiceRischioMeteoAggregatoNN")

            StrSQL.AppendLine("FROM Reg_Impianti_XIndiciProduttivitaAi rai")
            StrSQL.AppendLine("    INNER JOIN IndiciProduttivitaAi ai")
            StrSQL.AppendLine("        ON ai.IndiciProduttivitaAi_COD = rai.IndiciProduttivitaAi_COD")
            StrSQL.AppendLine("    INNER JOIN (")
            StrSQL.AppendLine("        SELECT TOP 1")
            StrSQL.AppendLine("            Produttivita AS maxProduttivita")
            StrSQL.AppendLine("            ,Plv AS maxPlv")
            StrSQL.AppendLine("            ,IndiceCO2 AS maxIndiceCO2")
            StrSQL.AppendLine("            ,IndiceErosione AS maxIndiceErosione")
            StrSQL.AppendLine("            ,IndiceRischioMeteoAggregato AS maxIndiceRischioMeteoAggregato")

            StrSQL.AppendLine("        FROM Reg_Impianti_XIndiciProduttivitaAi rai")
            StrSQL.AppendLine("            INNER JOIN IndiciProduttivitaAi ai")
            StrSQL.AppendLine("                ON ai.IndiciProduttivitaAi_COD = rai.IndiciProduttivitaAi_COD")
            StrSQL.AppendLine("        WHERE ai.Validita_Inizio = " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("            AND rai.piva = '-32'")
            StrSQL.AppendLine("    ) maxVal")
            StrSQL.AppendLine("        ON 1 = 1")

            StrSQL.AppendLine("    INNER JOIN (")
            StrSQL.AppendLine("        SELECT TOP 1")
            StrSQL.AppendLine("            Produttivita AS minProduttivita")
            StrSQL.AppendLine("            ,Plv AS minPlv")
            StrSQL.AppendLine("            ,IndiceCO2 AS minIndiceCO2")
            StrSQL.AppendLine("            ,IndiceErosione AS minIndiceErosione")
            StrSQL.AppendLine("            ,IndiceRischioMeteoAggregato AS minIndiceRischioMeteoAggregato")

            StrSQL.AppendLine("        FROM Reg_Impianti_XIndiciProduttivitaAi rai")
            StrSQL.AppendLine("            INNER JOIN IndiciProduttivitaAi ai")
            StrSQL.AppendLine("                ON ai.IndiciProduttivitaAi_COD = rai.IndiciProduttivitaAi_COD")
            StrSQL.AppendLine("        WHERE ai.Validita_Inizio = " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("            AND rai.piva = '-31'")
            StrSQL.AppendLine("    ) minVal")
            StrSQL.AppendLine("        ON 1 = 1")

            StrSQL.AppendLine("WHERE rai.piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine("    AND ai.validita_Inizio = " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("    AND ai.IndiciProduttivitaAi_COD > 0")
            StrSQL.AppendLine("")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function
End Class
