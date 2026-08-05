Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Varie_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Filtrone_New(
                                      Lista_Referenti As String,
                                      Lista_SpecieVegetali As String,
                                      Flag_Inizio_Precedente1_Successivo2 As Integer,
                                      Data_Inizio As Date,
                                      Flag_Fine_Precedente1_Successivo2 As Integer,
                                      Data_Fine As Date,
                                      ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Dim NomeRoutine As String = "Varie_R.Leggi_Filtrone()"

        Dim DT As DataTable

        Try

            Dim nomeDB_Utenti = objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            Dim sql As New StringBuilder

            sql.Clear()


#If False Then
            stb.AppendLine("Select a.*  ")
            stb.AppendLine(", geography::STGeomFromText(GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter().ToString(), 4326).Lat AS Lat_Baricentro  ")
            stb.AppendLine(", geography::STGeomFromText(GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter().ToString(), 4326).Long AS Lng_Baricentro  ")
            stb.AppendLine("  ")
            stb.AppendLine("from  ")
            stb.AppendLine("(  ")


            stb.AppendLine("   SELECT DISTINCT GIS_Entita.Entita_Cod AS UNID_Impianto, -9999 as Selezionato, Reg_Impianti.Sup_Imp as Superficie,  ")
            stb.AppendLine("      Reg_Impianti.Codice_Fiscale_Tecnico AS Referente_Piva,   ")
            stb.AppendLine("      lconf.Gruppi_Utente_des AS Referente_RagSoc, ")
            stb.AppendLine("      Imprese.rag_soc AS RagioneSociale, Cultivar.Cul_Cod, Cultivar.Cul_Des, SpecieVegetali.Veg_Cod,   ")
            stb.AppendLine("      SpecieVegetali.Veg_Des, CASE WHEN Reg_Impianti.Grva_Cod_veg < 0 THEN '1' ELSE '0' END AS Flag_Hybrid, Reg_Impianti.Grva_Cod_veg as Grva_Cod,   ")
            stb.AppendLine("      Indirizzi.ind_des AS Indirizzo, Indirizzi.frz_des AS Frazione, ISTAT.COM AS Comune_Cod, ISTAT.LOCALITA AS Comune_Des, ISTAT.PROV AS Provincia_Cod,   ")
            stb.AppendLine("      Lista_Province.PROVINCIA AS Provincia_Des, Lista_Province.SIGLA AS Provincia_Sigla, Lista_Province.REG AS Regione_Cod, Lista_Regioni.Regione_Des AS Regione_Des,  ")
            stb.AppendLine("      '' AS Layer, GIS_Entita.Entita_Cod, GruppoVarietale.Grva_Des  ")
            stb.AppendLine("      , 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Tipologia_Des ")
            stb.AppendLine("      , 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Descrizione_Poligono ")
            stb.AppendLine("      , Mappatura_Specie.ID_Specie AS ID_Specie_sementieri, Mappatura_Specie.ID_SottoSpecie, Mappatura_Specie.ID_Gruppo, Mappatura_Specie.ID_Genotipo  ")

            stb.AppendLine("      ,appezzamento.Via_Stringa ")

            stb.AppendLine("      ,Reg_Impianti.Validita_Inizio ")
            stb.AppendLine("      ,Reg_Impianti.Validita_Fine ")

            stb.AppendLine("    FROM GIS_Entita  ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        appezzamento on  GIS_Entita.Piva = appezzamento.PIVA AND GIS_Entita.Sa_Cod = appezzamento.SA_COD AND GIS_Entita.Appezza = appezzamento.APPEZZA  ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        Reg_Impianti ON GIS_Entita.Piva = Reg_Impianti.PIVA AND GIS_Entita.Sa_Cod = Reg_Impianti.SA_COD AND GIS_Entita.Appezza = Reg_Impianti.APPEZZA AND GIS_Entita.Id_Imp = Reg_Impianti.ID_REG ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine(nomeDB_Utenti & ".dbo.Gruppi_Utente lconf ON Reg_Impianti.Codice_Fiscale_Tecnico = lconf.Gruppi_Utente_Identificativo ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        Imprese ON Imprese.PIVA = Reg_Impianti.Piva ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        Cultivar on Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        SpecieVegetali on SpecieVegetali.veg_cod = Cultivar.veg_cod ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        CentrixIndirizzi on CentrixIndirizzi.piva = Reg_Impianti.Piva AND CentrixIndirizzi.Sa_Cod = Reg_Impianti.Sa_Cod ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        Indirizzi ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo  ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        ISTAT ON ISTAT.PROV = Indirizzi.pro_cod_istat AND ISTAT.COM = Indirizzi.com_cod_istat   ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        Lista_Province on Lista_Province.SIGLA = ISTAT.COMUNI_PROV ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        Lista_Regioni on Lista_Regioni.REG = Lista_Province.REG ")
            stb.AppendLine("    LEFT OUTER JOIN ")
            stb.AppendLine("        GruppoVarietale ON ABS(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.Grva_Cod  ")
            stb.AppendLine("    LEFT Join ")
            stb.AppendLine("        Mappatura_Specie ON  Mappatura_Specie.Hybrid =  (CASE WHEN SpecieVegetali.Veg_Cod in (6,5000021,70,69) THEN -1 ELSE (CASE WHEN Reg_Impianti.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END)")
            stb.AppendLine("            AND Mappatura_Specie.Veg_Cod = SpecieVegetali.Veg_Cod AND  Mappatura_Specie.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) ")




            stb.AppendLine(" WHERE   (CentrixIndirizzi.Tipo_Indirizzo = 1)  ")
            stb.AppendLine(" AND     (Cultivar.Cul_Cod <> 0)  ")



            stb.AppendLine(" AND     GIS_Entita.Entita_Cod IS NOT NULL  ")



            '/////      '00144040409-00144040409', '02022830406-02022830406'
            If Lista_Referenti <> "" Then
                stb.AppendLine(" AND     (reg_impianti.codice_fiscale_tecnico IN (" & Lista_Referenti & "))  ")
            End If

            '/////      38,42,67
            If Lista_SpecieVegetali <> "" Then
                stb.AppendLine(" AND     (SpecieVegetali.Veg_Cod IN (" & Lista_SpecieVegetali & "))  ")
            End If

            If Flag_Inizio_Precedente1_Successivo2 = 1 Then
                stb.AppendLine(" AND     (Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Inizio.ToShortDateString) & " )  ")
            Else
                stb.AppendLine(" AND     (Reg_Impianti.Validita_Inizio >= " & Agro_SQL_SaveDate(Data_Inizio.ToShortDateString) & " )  ")
            End If

            If Flag_Fine_Precedente1_Successivo2 = 1 Then
                stb.AppendLine(" AND     (Reg_Impianti.Validita_Fine <= " & Agro_SQL_SaveDate(Data_Fine.ToShortDateString) & " )  ")
            Else
                stb.AppendLine(" AND     (Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Fine.ToShortDateString) & " )  ")
            End If


            stb.AppendLine(") a ")
            stb.AppendLine(" ")
            stb.AppendLine("inner Join GIS_ElementiGrafici on GIS_ElementiGrafici.Entita_Cod = a.Entita_Cod ")
#End If

            sql.AppendLine("SELECT ")
            sql.AppendLine("	UNID_Impianto = ent.Entita_Cod ")
            sql.AppendLine("	, Selezionato = 0 ")
            sql.AppendLine("	, Referente_Piva = rimp.Codice_Fiscale_Tecnico ")
            sql.AppendLine("	, Referente_RagSoc = lconf.Gruppi_Utente_des ")
            sql.AppendLine("	, RagioneSociale = imp.rag_soc ")
            sql.AppendLine("	, app.Via_Stringa ")
            sql.AppendLine("	, Indirizzo = indi.ind_des ")
            sql.AppendLine("	, Frazione = indi.frz_des ")
            sql.AppendLine("	, Comune_Cod = ISTAT.COM ")
            sql.AppendLine("	, Comune_Des = ISTAT.LOCALITA ")
            sql.AppendLine("	, Provincia_Cod = ISTAT.PROV ")
            sql.AppendLine("	, Provincia_Des = lprov.PROVINCIA ")
            sql.AppendLine("	, Provincia_Sigla = lprov.SIGLA ")
            sql.AppendLine("	, Regione_Cod = lreg.REG ")
            sql.AppendLine("	, Regione_Des = lreg.Regione_Des ")
            sql.AppendLine("	, veg.Veg_Cod ")
            sql.AppendLine("	, veg.Veg_Des ")
            sql.AppendLine("	, ISNULL(CSV.Sementieri_ClassiDiSpecieVegetaliNomeScientifico_des, '') as NomeScientifico ")
            sql.AppendLine("	, cult.Cul_Cod ")
            sql.AppendLine("	, cult.Cul_Des ")
            sql.AppendLine("	, Grva_Cod = rimp.Grva_Cod_veg ")
            sql.AppendLine("	, Flag_Hybrid = CASE WHEN rimp.Grva_Cod_veg < 0 THEN '1' ELSE '0' END ")
            sql.AppendLine("	, Grva_Des = gvar.Grva_Des ")
            sql.AppendLine("	, ID_Specie_sementieri = mapp.ID_Specie ")
            sql.AppendLine("	, mapp.ID_SottoSpecie ")
            sql.AppendLine("	, mapp.ID_Gruppo ")
            sql.AppendLine("	, mapp.ID_Genotipo ")
            sql.AppendLine("	, Layer = '' ")
            sql.AppendLine("	, Tipologia_Des = '' ")
            sql.AppendLine("	, Descrizione_Poligono = '' ")
            sql.AppendLine("	, Superficie = rimp.Sup_Imp ")
            sql.AppendLine("	, Lat_Baricentro = geography::STGeomFromText(GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter().ToString(), 4326).Lat ")
            sql.AppendLine("	, Lng_Baricentro = geography::STGeomFromText(GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter().ToString(), 4326).Long ")
            sql.AppendLine("	, rimp.Validita_Inizio ")
            sql.AppendLine("	, rimp.Validita_Fine ")
            sql.AppendLine()
            sql.AppendLine("FROM Reg_Impianti rimp ")
            sql.AppendLine("INNER JOIN Imprese imp ON imp.PIVA = rimp.Piva ")
            sql.AppendLine("INNER JOIN " & nomeDB_Utenti & ".dbo.Gruppi_Utente lconf ON rimp.Codice_Fiscale_Tecnico = lconf.Gruppi_Utente_Identificativo ")
            sql.AppendLine("INNER JOIN Appezzamento app ON app.PIVA = rimp.PIVA AND app.SA_COD = rimp.SA_COD AND app.APPEZZA = rimp.APPEZZA ")
            sql.AppendLine("INNER JOIN Cultivar cult ON cult.Cul_Cod = rimp.CUL_COD ")
            sql.AppendLine("INNER JOIN SpecieVegetali veg ON veg.veg_cod = cult.veg_cod ")
            sql.AppendLine("LEFT OUTER JOIN GruppoVarietale gvar ON ABS(rimp.GRVA_Cod_VEG) = gvar.Grva_Cod ")
            sql.AppendLine("INNER JOIN GIS_Entita ent ON ent.Piva = rimp.PIVA AND ent.Sa_Cod = rimp.SA_COD AND ent.Appezza = rimp.APPEZZA AND ent.Id_Imp = rimp.ID_REG ")
            sql.AppendLine("INNER JOIN GIS_ElementiGrafici ON GIS_ElementiGrafici.Entita_Cod = ent.Entita_Cod ")
            sql.AppendLine("LEFT JOIN Mappatura_Specie mapp ON  mapp.Hybrid = (CASE WHEN veg.Veg_Cod in (6,5000021,70,69) THEN -1 ELSE (CASE WHEN rimp.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END) ")
            sql.AppendLine("			AND mapp.Veg_Cod = veg.Veg_Cod AND mapp.Grva_Cod = ABS(rimp.GRVA_Cod_VEG) ")
            sql.AppendLine("LEFT JOIN Sementieri_ClassiDiSpecieVegetali CSV ON CSV.ID_Specie = mapp.ID_Specie
                                                        and CSV.ID_SottoSpecie = mapp.ID_SottoSpecie
                                                        and CSV.ID_Gruppo = mapp.ID_Gruppo
                                                        and CSV.ID_Genotipo = mapp.ID_Genotipo ")
            sql.AppendLine("LEFT JOIN CentrixIndirizzi cxi ON cxi.PIVA = rimp.PIVA AND cxi.sa_cod = rimp.SA_COD ")
            sql.AppendLine("LEFT JOIN Indirizzi indi ON indi.cod_indirizzo = cxi.cod_indirizzo ")
            sql.AppendLine("LEFT JOIN ISTAT ON ISTAT.PROV = indi.pro_cod_istat AND ISTAT.COM = indi.com_cod_istat ")
            sql.AppendLine("LEFT JOIN Lista_Province lprov ON lprov.SIGLA = ISTAT.COMUNI_PROV ")
            sql.AppendLine("LEFT JOIN Lista_Regioni lreg ON lreg.REG = lprov.REG ")
            sql.AppendLine()

            Dim opDataInizio As String = If(Flag_Inizio_Precedente1_Successivo2 = 1, " <= ", " >= ")
            Dim opDataFine As String = If(Flag_Fine_Precedente1_Successivo2 = 1, " <= ", " >= ")

            sql.AppendLine("WHERE rimp.Validita_Inizio " & opDataInizio & Agro_SQL_SaveDate(Data_Inizio.ToShortDateString))
            sql.AppendLine("	AND rimp.Validita_Fine " & opDataFine & Agro_SQL_SaveDate(Data_Fine.ToShortDateString))

            If Lista_Referenti <> "" Then
                sql.AppendLine("	AND rimp.codice_fiscale_tecnico IN (" & Agro_SQL_Save_Clausola_IN(Lista_Referenti, True) & ") ")
            End If

            If Lista_SpecieVegetali <> "" Then
                sql.AppendLine("	AND veg.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Lista_SpecieVegetali) & ") ")
            End If

            sql.AppendLine("	AND cult.Cul_Cod <> 0 ")
            sql.AppendLine("	AND ent.Entita_Cod IS NOT NULL ")
            sql.AppendLine("	AND cxi.Tipo_Indirizzo = 1 ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, sql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message

            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function Leggi_Filtrone(
                            ByVal preventivo_1_Effettivo_2 As Integer,
                            ByVal Lista_Referenti As String,
                            ByVal Lista_Regioni As String,
                            ByVal Lista_Provincie As String,
                            ByVal Lista_Comuni As String,
                            ByVal Lista_SpecieVegetali As String,
                            ByVal Lista_Tipologie As String,
                            ByVal Flag_Inizio_Precedente1_Successivo2 As Integer,
                            ByVal Data_Inizio As Date,
                            ByVal Flag_Fine_Precedente1_Successivo2 As Integer,
                            ByVal Data_Fine As Date,
            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "Varie_R.Leggi_Filtrone()"
        Dim MessaggioErrore As String = ""


        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            Dim nomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            stb.AppendLine("Select a.*  ")
            stb.AppendLine(", geography::STGeomFromText(GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter().ToString(), 4326).Lat AS Lat_Baricentro  ")
            stb.AppendLine(", geography::STGeomFromText(GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter().ToString(), 4326).Long AS Lng_Baricentro  ")
            stb.AppendLine("  ")
            stb.AppendLine("from  ")
            stb.AppendLine("(  ")



            If preventivo_1_Effettivo_2 = 2 Then
                'effettivo

                stb.AppendLine("   SELECT DISTINCT GIS_Entita.Entita_Cod AS UNID_Impianto, -9999 as Selezionato, Reg_Impianti.Sup_Imp as Superficie,  ")
                stb.AppendLine("      Reg_Impianti.Codice_Fiscale_Tecnico AS Referente_Piva,   ")
                stb.AppendLine("      lconf.Gruppi_Utente_des AS Referente_RagSoc, ")
                stb.AppendLine("      Imprese.rag_soc AS RagioneSociale, Cultivar.Cul_Cod, Cultivar.Cul_Des, SpecieVegetali.Veg_Cod,   ")
                stb.AppendLine("      SpecieVegetali.Veg_Des, CASE WHEN Reg_Impianti.Grva_Cod_veg < 0 THEN '1' ELSE '0' END AS Flag_Hybrid, Reg_Impianti.Grva_Cod_veg as Grva_Cod,   ")
                stb.AppendLine("      Indirizzi.ind_des AS Indirizzo, Indirizzi.frz_des AS Frazione, ISTAT.COM AS Comune_Cod, ISTAT.LOCALITA AS Comune_Des, ISTAT.PROV AS Provincia_Cod,   ")
                stb.AppendLine("      Lista_Province.PROVINCIA AS Provincia_Des, Lista_Province.SIGLA AS Provincia_Sigla, Lista_Province.REG AS Regione_Cod, Lista_Regioni.Regione_Des AS Regione_Des,  ")
                stb.AppendLine("      '' AS Layer, GIS_Entita.Entita_Cod, GruppoVarietale.Grva_Des  ")
                stb.AppendLine("      , 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Tipologia_Des ")
                stb.AppendLine("      , 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Descrizione_Poligono ")
                stb.AppendLine("      , Mappatura_Specie.ID_Specie AS ID_Specie_sementieri, Mappatura_Specie.ID_SottoSpecie, Mappatura_Specie.ID_Gruppo, Mappatura_Specie.ID_Genotipo  ")

                stb.AppendLine("      ,appezzamento.Via_Stringa ")

                stb.AppendLine("      ,Reg_Impianti.Validita_Inizio ")
                stb.AppendLine("      ,Reg_Impianti.Validita_Fine ")

                stb.AppendLine("    FROM GIS_Entita  ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        appezzamento on  GIS_Entita.Piva = appezzamento.PIVA AND GIS_Entita.Sa_Cod = appezzamento.SA_COD AND GIS_Entita.Appezza = appezzamento.APPEZZA  ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        Reg_Impianti ON GIS_Entita.Piva = Reg_Impianti.PIVA AND GIS_Entita.Sa_Cod = Reg_Impianti.SA_COD AND GIS_Entita.Appezza = Reg_Impianti.APPEZZA AND GIS_Entita.Id_Imp = Reg_Impianti.ID_REG ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        " & nomeDB_Utenti & ".dbo.Gruppi_Utente layer ON Reg_Impianti.Codice_Fiscale_Tecnico = lconf.Gruppi_Utente_Identificativo ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        Imprese ON Imprese.PIVA = Reg_Impianti.Piva ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        Cultivar on Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        SpecieVegetali on SpecieVegetali.veg_cod = Cultivar.veg_cod ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        CentrixIndirizzi on CentrixIndirizzi.piva = Reg_Impianti.Piva AND CentrixIndirizzi.Sa_Cod = Reg_Impianti.Sa_Cod ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        Indirizzi ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo  ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        ISTAT ON ISTAT.PROV = Indirizzi.pro_cod_istat AND ISTAT.COM = Indirizzi.com_cod_istat   ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        Lista_Province on Lista_Province.SIGLA = ISTAT.COMUNI_PROV ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        Lista_Regioni on Lista_Regioni.REG = Lista_Province.REG ")
                stb.AppendLine("    LEFT OUTER JOIN ")
                stb.AppendLine("        GruppoVarietale ON ABS(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.Grva_Cod  ")
                stb.AppendLine("    LEFT Join ")
                stb.AppendLine("        Mappatura_Specie ON  Mappatura_Specie.Hybrid =  (CASE WHEN SpecieVegetali.Veg_Cod in (6,5000021,70,69) THEN -1 ELSE (CASE WHEN Reg_Impianti.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END)")
                stb.AppendLine("            AND Mappatura_Specie.Veg_Cod = SpecieVegetali.Veg_Cod AND  Mappatura_Specie.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) ")




                stb.AppendLine(" WHERE   (CentrixIndirizzi.Tipo_Indirizzo = 1)  ")
                stb.AppendLine(" AND     (Cultivar.Cul_Cod <> 0)  ")



                stb.AppendLine(" AND     GIS_Entita.Entita_Cod IS NOT NULL  ")



                '/////      '00144040409-00144040409', '02022830406-02022830406'
                If Lista_Referenti <> "" Then
                    stb.AppendLine(" AND     (reg_impianti.codice_fiscale_tecnico IN (" & Agro_SQL_Save_Clausola_IN(Lista_Referenti, True) & "))  ")
                End If

                '/////      38,42,67
                If Lista_SpecieVegetali <> "" Then
                    stb.AppendLine(" AND     (SpecieVegetali.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Lista_SpecieVegetali) & "))  ")
                End If

                '/////      -103, 103, -128
                If Lista_Tipologie <> "" Then
                    stb.AppendLine(" AND     (Reg_Impianti.GRVA_Cod_VEG IN (" & Agro_SQL_Save_Clausola_IN(Lista_Tipologie) & "))  ")
                End If

                If Flag_Inizio_Precedente1_Successivo2 = 1 Then
                    stb.AppendLine(" AND     (Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Inizio.ToShortDateString) & " )  ")
                Else
                    stb.AppendLine(" AND     (Reg_Impianti.Validita_Inizio >= " & Agro_SQL_SaveDate(Data_Inizio.ToShortDateString) & " )  ")
                End If

                If Flag_Fine_Precedente1_Successivo2 = 1 Then
                    stb.AppendLine(" AND     (Reg_Impianti.Validita_Fine <= " & Agro_SQL_SaveDate(Data_Fine.ToShortDateString) & " )  ")
                Else
                    stb.AppendLine(" AND     (Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Fine.ToShortDateString) & " )  ")
                End If

                '==========

                If Lista_Comuni <> "" Then

                    Dim vComuni() As String
                    Dim vComuni2() As String
                    Dim i As Integer
                    Dim Testo As String
                    Dim cProvincia As String
                    Dim cComune As String


                    '------------------------------
                    '---  '023:123','012:128'
                    '------------------------------

                    vComuni = Split(Lista_Comuni, ",")

                    stb.AppendLine(" AND     ( ")

                    For i = 0 To vComuni.GetLength(0) - 1

                        If i <> 0 Then
                            stb.AppendLine(" OR ")
                        End If

                        '------------------------------
                        '---  '023:123'
                        '------------------------------

                        Testo = vComuni(i)
                        Testo = Testo.Replace("'", "")

                        '------------------------------
                        '---  023:123
                        '------------------------------

                        vComuni2 = Split(Testo, ":")
                        cProvincia = vComuni2(0)
                        cComune = vComuni2(1)

                        stb.AppendLine(" ((ISTAT.PROV = '" & Agro_SQL_SaveText(cProvincia) & "') AND (ISTAT.COM = '" & Agro_SQL_SaveText(cComune) & "'))  ")

                    Next

                    stb.AppendLine(" ) ")

                Else

                    '///// PROVINCIA     '039', '040', '038'
                    If Lista_Provincie <> "" Then
                        stb.AppendLine(" AND     (ISTAT.PROV IN (" & Agro_SQL_Save_Clausola_IN(Lista_Provincie, True) & "))  ")

                    Else

                        '///// REGIONE     '008', '009'
                        If Lista_Regioni <> "" Then
                            stb.AppendLine(" AND     (Lista_Province.REG IN (" & Agro_SQL_Save_Clausola_IN(Lista_Regioni, True) & "))  ")
                        End If
                    End If
                End If

            Else
                'Presuntivo
                stb.Append("SELECT DISTINCT GIS_Entita.Entita_Cod AS UNID_Impianto, -9999 as Selezionato, Programmazione_Entita.Superficie, Programmazione_Entita.Codice_Fiscale_Tecnico AS Referente_Piva,  " & vbCrLf)
                stb.Append("                       lconf.Gruppi_Utente_des AS Referente_RagSoc, ImpreseF.rag_soc AS RagioneSociale, Cultivar.Cul_Cod, Cultivar.Cul_Des, SpecieVegetali.Veg_Cod,  " & vbCrLf)
                stb.Append("                       SpecieVegetali.Veg_Des, CASE WHEN Programmazione_Entita.Grva_Cod < 0 THEN '1' ELSE '0' END AS Flag_Hybrid, Programmazione_Entita.Grva_Cod,  " & vbCrLf)
                stb.Append("                       Indirizzi.ind_des AS Indirizzo, Indirizzi.frz_des AS Frazione, ISTAT.COM AS Comune_Cod, ISTAT.LOCALITA AS Comune_Des, ISTAT.PROV AS Provincia_Cod,  " & vbCrLf)
                stb.Append("                       Lista_Province.PROVINCIA AS Provincia_Des, Lista_Province.SIGLA AS Provincia_Sigla, Lista_Province.REG AS Regione_Cod, Lista_Regioni.Regione_Des AS Regione_Des,  " & vbCrLf)
                stb.Append("                       '' AS Layer, GIS_Entita.Entita_Cod, GruppoVarietale.Grva_Des " & vbCrLf)

                stb.Append("         , 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Tipologia_Des  ")
                stb.Append("         , 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Descrizione_Poligono  ")

                stb.Append("         , Mappatura_Specie.ID_Specie AS ID_Specie_sementieri, Mappatura_Specie.ID_SottoSpecie, Mappatura_Specie.ID_Gruppo, Mappatura_Specie.ID_Genotipo  ")
                stb.Append("         , Programmazione_Entita.via_stringa ")

                stb.Append("FROM         Mappatura_Specie LEFT JOIN " & vbCrLf)
                stb.Append("                       Cultivar LEFT JOIN " & vbCrLf)
                stb.Append("                       Lista_Province LEFT JOIN " & vbCrLf)
                stb.Append("                       ISTAT LEFT JOIN " & vbCrLf)
                stb.Append("                       CentrixIndirizzi LEFT JOIN " & vbCrLf)
                stb.Append("                       GerarchiaImprese LEFT JOIN " & vbCrLf)
                stb.Append("                       Imprese AS ImpreseR ON GerarchiaImprese.Padre = ImpreseR.PIVA LEFT JOIN " & vbCrLf)
                stb.Append("                       Imprese AS ImpreseF ON GerarchiaImprese.Figlio = ImpreseF.PIVA LEFT JOIN " & vbCrLf)
                stb.Append("                       Programmazione_Entita ON ImpreseF.PIVA = Programmazione_Entita.Piva ON CentrixIndirizzi.PIVA = Programmazione_Entita.Piva AND  " & vbCrLf)
                stb.Append("                       CentrixIndirizzi.sa_cod = Programmazione_Entita.Sa_Cod LEFT JOIN " & vbCrLf)
                stb.Append("                       Indirizzi ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ON ISTAT.PROV = Indirizzi.pro_cod_istat AND ISTAT.COM = Indirizzi.com_cod_istat ON  " & vbCrLf)
                stb.Append("                       Lista_Province.SIGLA = ISTAT.COMUNI_PROV LEFT JOIN " & vbCrLf)
                stb.Append("                       Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG LEFT JOIN " & vbCrLf)
                stb.Append("                       " & nomeDB_Utenti & ".dbo.Gruppi_Utente lconf ON Programmazione_Entita.Codice_Fiscale_Tecnico = lconf.Gruppi_Utente_Identificativo LEFT JOIN " & vbCrLf)
                stb.Append("                       GIS_Entita ON Programmazione_Entita.Piva_SuperUser = GIS_Entita.PivaSuperUser AND  " & vbCrLf)
                stb.Append("                       Programmazione_Entita.Programmazione_Entita_Cod = GIS_Entita.Programmazione_Entita_Cod ON Cultivar.Cul_Cod = Programmazione_Entita.Cul_Cod LEFT JOIN " & vbCrLf)
                stb.Append("                       SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ON Mappatura_Specie.Veg_Cod = SpecieVegetali.Veg_Cod AND  " & vbCrLf)
                stb.Append("                       Mappatura_Specie.Hybrid = (CASE WHEN Programmazione_Entita.Grva_Cod < 0 THEN '1' ELSE '0' END) AND  " & vbCrLf)
                stb.Append("                       Mappatura_Specie.Grva_Cod = ABS(Programmazione_Entita.Grva_Cod) LEFT OUTER JOIN " & vbCrLf)
                stb.Append("                       GruppoVarietale ON ABS(Programmazione_Entita.Grva_Cod) = GruppoVarietale.Grva_Cod " & vbCrLf)

                stb.AppendLine(" WHERE   (CentrixIndirizzi.Tipo_Indirizzo = 1)  ")
                stb.AppendLine(" AND     (Cultivar.Cul_Cod <> 0)  ")



                stb.AppendLine(" AND     GIS_Entita.Entita_Cod IS NOT NULL  ")




                    '/////      '00144040409-00144040409', '02022830406-02022830406'
                If Lista_Referenti <> "" Then
                    stb.AppendLine(" AND     (Programmazione_Entita.codice_fiscale_tecnico IN (" & Agro_SQL_Save_Clausola_IN(Lista_Referenti, True) & "))  ")
                End If

                '/////      38,42,67
                If Lista_SpecieVegetali <> "" Then
                    stb.AppendLine(" AND     (SpecieVegetali.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Lista_SpecieVegetali) & "))  ")
                End If

                '/////      -103, 103, -128
                If Lista_Tipologie <> "" Then
                    stb.AppendLine(" AND     (Programmazione_Entita.GRVA_Cod IN (" & Agro_SQL_Save_Clausola_IN(Lista_Tipologie) & "))  ")
                End If

                If Flag_Inizio_Precedente1_Successivo2 = 1 Then
                    stb.AppendLine(" AND     (Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Inizio.ToShortDateString) & " )  ")
                Else
                    stb.AppendLine(" AND     (Programmazione_Entita.Validita_Inizio >= " & Agro_SQL_SaveDate(Data_Inizio.ToShortDateString) & " )  ")
                End If

                If Flag_Fine_Precedente1_Successivo2 = 1 Then
                    stb.AppendLine(" AND     (Programmazione_Entita.Validita_Fine <= " & Agro_SQL_SaveDate(Data_Fine.ToShortDateString) & " )  ")
                Else
                    stb.AppendLine(" AND     (Programmazione_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Fine.ToShortDateString) & " )  ")
                End If

                '==========

                If Lista_Comuni <> "" Then

                    Dim vComuni() As String
                    Dim vComuni2() As String
                    Dim i As Integer
                    Dim Testo As String
                    Dim cProvincia As String
                    Dim cComune As String


                        '------------------------------
                        '---  '023:123','012:128'
                        '------------------------------

                    vComuni = Split(Lista_Comuni, ",")

                    stb.AppendLine(" AND     ( ")

                    For i = 0 To vComuni.GetLength(0) - 1

                        If i <> 0 Then
                            stb.AppendLine(" OR ")
                            End If

                            '------------------------------
                            '---  '023:123'
                            '------------------------------

                        Testo = vComuni(i)
                        Testo = Testo.Replace("'", "")

                            '------------------------------
                            '---  023:123
                            '------------------------------
                        vComuni2 = Split(Testo, ":")
                        cProvincia = vComuni2(0)
                        cComune = vComuni2(1)
                        stb.AppendLine(" ((ISTAT.PROV = '" & Agro_SQL_SaveText(cProvincia) & "') AND (ISTAT.COM = '" & Agro_SQL_SaveText(cComune) & "'))  ")

                    Next
                    stb.AppendLine(" ) ")
                Else
                    '///// PROVINCIA     '039', '040', '038'
                    If Lista_Provincie <> "" Then
                        stb.AppendLine(" AND     (ISTAT.PROV IN (" & Agro_SQL_Save_Clausola_IN(Lista_Provincie, True) & "))  ")

                    Else

                        '///// REGIONE     '008', '009'
                        If Lista_Regioni <> "" Then
                            stb.AppendLine(" AND     (Lista_Province.REG IN (" & Agro_SQL_Save_Clausola_IN(Lista_Regioni, True) & "))  ")
                        End If
                    End If
                    End If
            End If



            stb.AppendLine(") a ")
            stb.AppendLine(" ")
            stb.AppendLine("inner Join GIS_ElementiGrafici on GIS_ElementiGrafici.Entita_Cod = a.Entita_Cod ")



            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Leggi_VegCod_Da_MappaturaSpecie(ByVal id_specie As Integer, ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Varie_R.Leggi_VegCod_Da_MappaturaSpecie()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            stb.AppendLine(" SELECT DISTINCT Veg_Cod FROM Mappatura_Specie ")
            stb.AppendLine(" WHERE ID_Specie = " & id_specie & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt

    End Function

    Public Function Leggi_Mappatura_Specie_Distanze(ByVal id_specie_in As String, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Varie_R.Leggi_Mappatura_Specie_Distanze()"
        Dim MessaggioErrore As String = ""


        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            stb.AppendLine(" Select * from Mappatura_Specie_Distanze ")

            If id_specie_in <> "" Then
                stb.AppendLine(" where ID_Specie in (" & Agro_SQL_Save_Clausola_IN(id_specie_in) & ")  ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function


    Public Function Leggi_Mappatura_Specie_Distanze_Bietola(ByVal id_specie_in As String, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Varie_R.Leggi_Mappatura_Specie_Distanze_Bietola()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            stb.AppendLine(" Select * from Mappatura_Specie_Distanze_Bietola ")

            If id_specie_in <> "" Then
                stb.AppendLine(" where ID_Specie in (" & Agro_SQL_Save_Clausola_IN(id_specie_in) & ")  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Leggi_SeSpecieMappata(ByVal veg_cod As Integer, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "Varie_R.Leggi_SeSpecieMappata()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            stb.AppendLine(" SELECT DISTINCT Veg_Cod FROM Mappatura_Specie WHERE Veg_Cod = " & veg_cod)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt.Rows.Count > 0

    End Function

    Public Function SpecieMappata_Finalita(ByVal veg_cod As Integer, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "Varie_R.SpecieMappata_Finalita()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            'GABRIELE 26 03 2019 -> hardcoded, ma da gestire come colonna in tabella
            stb.AppendLine(" SELECT 6 AS Finalita FROM Mappatura_Specie WHERE Veg_Cod = " & veg_cod)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim finalita As Integer = -1

        If dt.Rows.Count > 0 Then

            finalita = CInt(dt.Rows(0)("Finalita"))

        End If

        Return finalita

    End Function

    Public Function SpeciePermessaMappaturaLibera(ByVal veg_cod As Integer,
                                                  ByVal finalita As Integer,
                                                  ByVal data_inizio As String,
                                                  ByVal data_fine As String,
                                                  ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "Varie_R.SpeciePermessaMappaturaLibera()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            'GABRIELE 26 03 2019 -> hardcoded, ma da gestire come colonna in tabella
            stb.AppendLine(" SELECT DISTINCT Veg_Cod, 6 AS Finalita FROM Mappatura_Specie WHERE Veg_Cod = " & veg_cod)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim result As Boolean = True

        If dt.Rows.Count > 0 Then

            For Each dr In dt.Rows

                If CInt(dr("Finalita")) = finalita Then
                    result = False
                End If

            Next

        End If

        Return result

    End Function

End Class

 