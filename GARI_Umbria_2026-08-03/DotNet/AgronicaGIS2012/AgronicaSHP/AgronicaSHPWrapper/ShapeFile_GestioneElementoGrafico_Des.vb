Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class ShapeFile_GestioneElementoGrafico_Des
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function aggiornaElementoGrafico_Des( _
                                            ByVal CODICE_FISCALE_TECNICO As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean

        Dim rval As Boolean

        'Select Case CODICE_FISCALE_TECNICO

        '    Case "13171470159" 'KWS italia
        '        rval = aggiornaElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, objParametri)

        '    Case "02022830406" 'SEES Vanderhave
        '        rval = aggiornaElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, objParametri)

        '    Case "00127740405" 'Apofruit
        '        rval = aggiornaElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, objParametri)

        '        'modello comune... (kws per adesso..)
        '    Case Else
        '        rval = aggiornaElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, objParametri)

        'End Select

        rval = aggiornaElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, objParametri)

        Return rval

    End Function


    Public Function aggiornaElementoGrafico_Des_KWS( _
                                        ByVal CODICE_FISCALE_TECNICO As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaSHPWrapper.ShapeFile_GestioneElementoGrafico_Des.aggiornaElementoGrafico_Des()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            '---------------------------------------------- QUERY PER UPDATE -------------------------------------------------------------
            '--- NB: impostare la PIVA giusta nel WHERE
            '--kws Italia								13171470159
            '--SES Vanderhave Italia Spa				02022830406

            Stb.Append(" UPDATE geg " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" SET ElementoGrafico_Des = " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("       'CA_PIVA§ ' + ca.piva  " & vbCrLf)
            'Stb.Append("    + '|CA_RagSo§ ' + imp.rag_soc + ' # ' + ca.sa_nome + ' # ' + a.APP_NOME  " & vbCrLf)
            Stb.Append("    + '|CA_RagSo§ ' + coalesce(imp.rag_soc, '')  " & vbCrLf)
            Stb.Append("    + '|CA_Nome§ ' + coalesce(ca.sa_nome, '')  " & vbCrLf)
            Stb.Append("    + '|CA_Indir§ ' + coalesce(i.ind_des, '') " & vbCrLf)
            Stb.Append("    + '|CA_Com§ ' +  coalesce(ist.localita , '')  " & vbCrLf)
            Stb.Append("    + '|CA_CAP§ ' +  coalesce(i.CAP , '') " & vbCrLf)
            Stb.Append("    + '|CA_Prov§ ' +  coalesce(ist.comuni_prov, '')   " & vbCrLf)
            Stb.Append("    + '|CA_Stato§ ' +  coalesce(i.stato , '')  " & vbCrLf)
            Stb.Append("    + '|App_Nome§ ' +  coalesce(a.APP_NOME , '')  " & vbCrLf)
            Stb.Append("    + '|App_Sup§ ' +  cast(a.SUP_APP  as varchar(50))  " & vbCrLf)
            Stb.Append("    + '|Imp_Sup§ ' +  cast(ri.Sup_Imp as varchar(50))  " & vbCrLf)
            Stb.Append("    + '|Tip_Var§ ' +   " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    case when ri.GRVA_Cod_VEG < 0 " & vbCrLf)
            Stb.Append("        then gv.grva_des + ' - ibrido' " & vbCrLf)
            Stb.Append("        else gv.grva_des " & vbCrLf)
            Stb.Append("             End " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    + '|Spec_Veg§ ' +  coalesce(sv.Veg_Des, '')  " & vbCrLf)
            Stb.Append("    + '|Sup_Ggle§ ' +  cast(geg.Poligono_GeoEntity.STArea() as varchar(50)) " & vbCrLf)
            Stb.Append("    + '|N_Lotto§ ' +  coalesce(ic.val_Cod, '') + '-' + coalesce( ip.Progetto_Nome, '')  " & vbCrLf)
            Stb.Append("    + '|Note§ ' +  ' '  " & vbCrLf)
            Stb.Append("    + '|Coord§ ' + cast( round( Centro.Lat, 6) as varchar(100)) + ' ' + cast( round( Centro.Long, 6) as varchar(100)) " & vbCrLf)
            Stb.Append("    + '|Via_Imp§ ' + coalesce(a.Via_Stringa, ' ') " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" FROM ( SELECT *  " & vbCrLf)
            Stb.Append("    FROM Reg_Impianti ri " & vbCrLf)
            Stb.Append("    WHERE ri.CODICE_FISCALE_TECNICO = " & Agro_SQL_SaveText_NULL(CODICE_FISCALE_TECNICO) & " ) ri  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN Centri_Aziendali ca " & vbCrLf)
            Stb.Append("        ON ri.piva = ca.piva " & vbCrLf)
            Stb.Append("        AND ri.SA_COD = ca.sa_cod  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN Imprese imp " & vbCrLf)
            Stb.Append("        ON imp.PIVA = ca.PIVA  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    LEFT JOIN Imprese_Codici ic " & vbCrLf)
            Stb.Append("         ON ic.PIVA = ca.PIVA   " & vbCrLf)
            Stb.Append("        and ic.id_cod = 1033 " & vbCrLf)

            Stb.Append("    LEFT JOIN CentrixIndirizzi ci " & vbCrLf)
            Stb.Append("        ON ca.PIVA = ci.PIVA  " & vbCrLf)
            Stb.Append("        AND ca.sa_cod = ci.SA_COD " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    LEFT JOIN Indirizzi i " & vbCrLf)
            Stb.Append("        ON ci.cod_indirizzo = i.cod_indirizzo " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    LEFT JOIN ISTAT ist " & vbCrLf)
            Stb.Append("        ON i.pro_cod_istat = ist.PROV  " & vbCrLf)
            Stb.Append("        AND i.com_cod_istat = ist.COM " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN Appezzamento a " & vbCrLf)
            Stb.Append("        ON ri.piva = a.piva " & vbCrLf)
            Stb.Append("        AND ri.sa_cod = a.sa_cod " & vbCrLf)
            Stb.Append("        AND ri.appezza = a.appezza " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN ( " & vbCrLf)
            Stb.Append("        SELECT 0 as Cul_Cod, 0 as Veg_Cod " & vbCrLf)
            Stb.Append("        UNION " & vbCrLf)
            Stb.Append("        SELECT cu.Cul_Cod, cu.Veg_Cod " & vbCrLf)
            Stb.Append("        FROM Cultivar cu " & vbCrLf)
            Stb.Append("        ) cu " & vbCrLf)
            Stb.Append("        on cu.Cul_Cod = ri.CUL_COD " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN ( " & vbCrLf)
            Stb.Append("        SELECT 0 as Grva_Cod, 'non specificato' as Grva_des " & vbCrLf)
            Stb.Append("        UNION " & vbCrLf)
            Stb.Append("        SELECT gv.Grva_Cod, gv.Grva_Des " & vbCrLf)
            Stb.Append("        FROM GruppoVarietale gv " & vbCrLf)
            Stb.Append("        ) gv " & vbCrLf)
            Stb.Append("        ON gv.Grva_Cod = abs(ri.GRVA_Cod_VEG) " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN SpecieVegetali sv " & vbCrLf)
            Stb.Append("        ON sv.Veg_Cod = cu.Veg_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN gis_entita ge " & vbCrLf)
            Stb.Append("        ON ri.piva = ge.Piva " & vbCrLf)
            Stb.Append("        AND ri.sa_cod = ge.Sa_Cod " & vbCrLf)
            Stb.Append("        AND ri.appezza = ge.Appezza " & vbCrLf)
            Stb.Append("        AND ri.ID_REG = ge.Id_Imp " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN GIS_ElementiGrafici geg " & vbCrLf)
            Stb.Append("        ON ge.entita_cod = geg.Entita_Cod  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN ( " & vbCrLf)
            Stb.Append("        select entita_cod, Poligono_GeoEntity.EnvelopeCenter() as Centro " & vbCrLf)
            Stb.Append("        from gis_elementigrafici  " & vbCrLf)
            Stb.Append(" ) aEvelope" & vbCrLf)
            Stb.Append("        ON aEvelope.entita_cod = geg.Entita_Cod  " & vbCrLf)

            Stb.Append("    LEFT JOIN imprese_Progetti ip " & vbCrLf)
            Stb.Append("        ON ip.Piva = ri.PIVA " & vbCrLf)
            Stb.Append("        AND ip.Sa_Cod = ri.SA_COD " & vbCrLf)
            Stb.Append("        AND ip.Appezza = ri.APPEZZA " & vbCrLf)
            Stb.Append("        AND ip.Id_Reg = ri.id_reg " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" WHERE ri.CODICE_FISCALE_TECNICO = " & Agro_SQL_SaveText_NULL(CODICE_FISCALE_TECNICO) & vbCrLf)
            Stb.Append(" AND geg.LayerElementiGrafici_Cod = 19 " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function


    Public Function ElementoGrafico_Des(ByVal CODICE_FISCALE_TECNICO As String, ByVal entita_cod As List(Of Integer), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim rval As DataTable

        'Select Case CODICE_FISCALE_TECNICO

        '    Case "13171470159" 'KWS italia
        '        rval = ElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, entita_cod, objParametri)

        '    Case "02022830406" 'SEES Vanderhave
        '        rval = ElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, entita_cod, objParametri)

        '    Case "00127740405" 'Apofruit
        '        rval = ElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, entita_cod, objParametri)

        '        'modello comune... (kws per adesso..)
        '    Case Else
        '        rval = ElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, entita_cod, objParametri)

        'End Select

        rval = ElementoGrafico_Des_KWS(CODICE_FISCALE_TECNICO, entita_cod, objParametri)

        Return rval

    End Function

    Public Function ElementoGrafico_Des_KWS(ByVal CODICE_FISCALE_TECNICO As String, ByVal entita_cod As List(Of Integer), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaSHPWrapper.ShapeFile_GestioneElementoGrafico_Des.ElementoGrafico_Des()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim lista As String = String.Join(", ", entita_cod)

            Stb.Length = 0

            Stb.AppendLine("SELECT ")
            'Stb.AppendLine("    aEnvelope.Entita_cod AS EntitaCod ")
            'Stb.AppendLine("    , 'CA_PIVA§ ' + ca.piva ")
            ''Stb.AppendLine("   + '|CA_RagSo§ ' + imp.rag_soc + ' # ' + ca.sa_nome + ' # ' + a.APP_NOME  ")
            'Stb.AppendLine("    + '|CA_RagSo§ ' + coalesce(imp.rag_soc, '') ")
            'Stb.AppendLine("    + '|CA_Nome§ ' + coalesce(ca.sa_nome, '') ")
            'Stb.AppendLine("    + '|CA_Indir§ ' + coalesce(i.ind_des, '') ")
            'Stb.AppendLine("    + '|CA_Com§ ' +  coalesce(ist.localita , '') ")
            'Stb.AppendLine("    + '|CA_CAP§ ' +  coalesce(i.CAP , '') ")
            'Stb.AppendLine("    + '|CA_Prov§ ' +  coalesce(ist.comuni_prov, '') ")
            'Stb.AppendLine("    + '|CA_Stato§ ' +  coalesce(i.stato , '') ")
            'Stb.AppendLine("    + '|App_Nome§ ' +  coalesce(a.APP_NOME , '') ")
            'Stb.AppendLine("    + '|App_Sup§ ' +  cast(a.SUP_APP  as varchar(50)) ")
            'Stb.AppendLine("    + '|Imp_Sup§ ' +  cast(ri.Sup_Imp as varchar(50)) ")
            'Stb.AppendLine("    + '|Tip_Var§ ' +  ")
            'Stb.AppendLine("    case when ri.GRVA_Cod_VEG < 0 ")
            'Stb.AppendLine("        then gv.grva_des + ' - ibrido' ")
            'Stb.AppendLine("        else gv.grva_des ")
            'Stb.AppendLine("             End ")
            'Stb.AppendLine("    + '|Spec_Veg§ ' +  coalesce(sv.Veg_Des, '') ")
            'Stb.AppendLine("    + '|Sup_Ggle§ ' +  cast(geg.Poligono_GeoEntity.STArea() as varchar(50)) ")
            'Stb.AppendLine("    + '|N_Lotto§ ' +  coalesce(ic.val_Cod, '') + '-' + coalesce( ip.Progetto_Nome, '') ")
            'Stb.AppendLine("    + '|Note§ ' +  ' ' ")
            'Stb.AppendLine("    + '|Coord§ ' + cast( round( Centro.Lat, 6) as varchar(100)) + ' ' + cast( round( Centro.Long, 6) as varchar(100)) ")
            'Stb.AppendLine("    + '|Via_Imp§ ' + coalesce(a.Via_Stringa, ' ') ")
            'Stb.AppendLine("    AS Descrizione ")

            Stb.AppendLine("    ca.piva AS CA_PIVA ")
            Stb.AppendLine("    , coalesce(imp.rag_soc, '') AS CA_RagSo ")
            Stb.AppendLine("    , coalesce(ca.sa_nome, '') AS CA_Nome ")
            Stb.AppendLine("    , coalesce(i.ind_des, '') AS CA_Indir ")
            Stb.AppendLine("    , coalesce(ist.localita , '') AS CA_Com ")
            Stb.AppendLine("    , coalesce(i.CAP , '') AS CA_CAP ")
            Stb.AppendLine("    , coalesce(ist.comuni_prov, '') AS CA_Prov ")
            Stb.AppendLine("    , coalesce(i.stato , '') AS CA_Stato ")
            Stb.AppendLine("    , coalesce(a.APP_NOME , '') AS App_Nome ")
            Stb.AppendLine("    , cast(a.SUP_APP as varchar(50)) AS App_Sup ")
            Stb.AppendLine("    , cast(ri.Sup_Imp as varchar(50)) AS Imp_Sup ")
            Stb.AppendLine("    , CASE WHEN ri.GRVA_Cod_VEG < 0 THEN gv.grva_des + ' - ibrido' ELSE gv.grva_des END AS Tip_Var")
            Stb.AppendLine("    , coalesce(sv.Veg_Des, '') AS Spec_Veg ")
            Stb.AppendLine("    , cast(geg.Poligono_GeoEntity.STArea() as varchar(50)) AS Sup_Ggle ")
            Stb.AppendLine("    , coalesce(ic.val_Cod, '') + '-' + coalesce( ip.Progetto_Nome, '') AS N_Lotto ")
            Stb.AppendLine("    , ' ' AS Note ")
            Stb.AppendLine("    , cast( round( Centro.Lat, 6) as varchar(100)) + ' ' + cast( round( Centro.Long, 6) as varchar(100)) AS Coord ")
            Stb.AppendLine("    , coalesce(a.Via_Stringa, ' ') AS Via_Imp ")
            Stb.AppendLine("	, aEnvelope.Gml AS Gml")
            Stb.AppendLine("	, coalesce(accRif.val_cod, '') as Rif")
            Stb.AppendLine()
            Stb.AppendLine(" FROM Reg_Impianti ri ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN Centri_Aziendali ca ")
            Stb.AppendLine("        ON ri.piva = ca.piva ")
            Stb.AppendLine("        AND ri.SA_COD = ca.sa_cod  ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN Imprese imp ")
            Stb.AppendLine("        ON imp.PIVA = ca.PIVA  ")
            Stb.AppendLine()
            Stb.AppendLine("    LEFT JOIN Imprese_Codici ic ")
            Stb.AppendLine("         ON ic.PIVA = ca.PIVA   ")
            Stb.AppendLine("        and ic.id_cod = 1033 ")
            Stb.AppendLine("    LEFT JOIN ( ")
            Stb.AppendLine("          Select piva, sa_cod, min(cod_indirizzo) As cod_indirizzo ")
            Stb.AppendLine("          From centriXindirizzi ci ")
            Stb.AppendLine("          Group By piva, sa_cod")
            Stb.AppendLine("    ) ci ")
            Stb.AppendLine("        ON ca.PIVA = ci.PIVA  ")
            Stb.AppendLine("        AND ca.sa_cod = ci.SA_COD ")
            Stb.AppendLine()
            Stb.AppendLine("    LEFT JOIN Indirizzi i ")
            Stb.AppendLine("        ON ci.cod_indirizzo = i.cod_indirizzo ")
            Stb.AppendLine()
            Stb.AppendLine("    LEFT JOIN ISTAT ist ")
            Stb.AppendLine("        ON i.pro_cod_istat = ist.PROV  ")
            Stb.AppendLine("        AND i.com_cod_istat = ist.COM ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN Appezzamento a ")
            Stb.AppendLine("        ON ri.piva = a.piva ")
            Stb.AppendLine("        AND ri.sa_cod = a.sa_cod ")
            Stb.AppendLine("        AND ri.appezza = a.appezza ")

            Stb.AppendLine("    Left Join Appezzamento_Codici accRif ")
            Stb.AppendLine("     On accRif.piva = a.piva ")
            Stb.AppendLine("     And accRif.sa_Cod = a.sa_cod ")
            Stb.AppendLine("     And accRif.appezza = a.appezza ")
            Stb.AppendLine("     And accRif.id_cod = " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)

            Stb.AppendLine("    INNER JOIN ( ")
            Stb.AppendLine("        SELECT 0 as Cul_Cod, 0 as Veg_Cod ")
            Stb.AppendLine("        UNION ")
            Stb.AppendLine("        SELECT cu.Cul_Cod, cu.Veg_Cod ")
            Stb.AppendLine("        FROM Cultivar cu ")
            Stb.AppendLine("        ) cu ")
            Stb.AppendLine("        on cu.Cul_Cod = ri.CUL_COD ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN ( ")
            Stb.AppendLine("        SELECT 0 as Grva_Cod, 'non specificato' as Grva_des ")
            Stb.AppendLine("        UNION ")
            Stb.AppendLine("        SELECT gv.Grva_Cod, gv.Grva_Des ")
            Stb.AppendLine("        FROM GruppoVarietale gv ")
            Stb.AppendLine("        ) gv ")
            Stb.AppendLine("        ON gv.Grva_Cod = abs(ri.GRVA_Cod_VEG) ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN SpecieVegetali sv ")
            Stb.AppendLine("        ON sv.Veg_Cod = cu.Veg_Cod ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN gis_entita ge ")
            Stb.AppendLine("        ON ri.piva = ge.Piva ")
            Stb.AppendLine("        AND ri.sa_cod = ge.Sa_Cod ")
            Stb.AppendLine("        AND ri.appezza = ge.Appezza ")
            Stb.AppendLine("        AND ri.ID_REG = ge.Id_Imp ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN GIS_ElementiGrafici geg ")
            Stb.AppendLine("        ON ge.entita_cod = geg.Entita_Cod ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN ( ")
            Stb.AppendLine("        select entita_cod, Poligono_GeoEntity.EnvelopeCenter() as Centro, Poligono_GeoEntity.AsGml() as Gml ")
            Stb.AppendLine("        from gis_elementigrafici ")
            Stb.AppendLine(" ) aEnvelope ")
            Stb.AppendLine("        ON aEnvelope.entita_cod = geg.Entita_Cod  ")
            Stb.AppendLine()
            Stb.AppendLine("    INNER JOIN imprese_Progetti ip ")
            Stb.AppendLine("        ON ip.Piva = ri.PIVA ")
            Stb.AppendLine("        AND ip.Sa_Cod = ri.SA_COD ")
            Stb.AppendLine("        AND ip.Appezza = ri.APPEZZA ")
            Stb.AppendLine("        AND ip.Id_Reg = ri.id_reg ")
            Stb.AppendLine(" ")

            Stb.Append(" WHERE   ip.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " + vbCrLf)
            Stb.Append(" AND   ip.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " + vbCrLf)

            If CODICE_FISCALE_TECNICO <> "CF-TEC" Then
                Stb.AppendLine(" AND ri.CODICE_FISCALE_TECNICO = " & Agro_SQL_SaveText_NULL(CODICE_FISCALE_TECNICO) & vbCrLf)
            End If

            Stb.AppendLine(" AND geg.LayerElementiGrafici_Cod = 19 ")
            Stb.AppendLine(" AND geg.Entita_Cod IN (" & Agro_SQL_Save_Clausola_IN(lista) & ") ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

End Class
