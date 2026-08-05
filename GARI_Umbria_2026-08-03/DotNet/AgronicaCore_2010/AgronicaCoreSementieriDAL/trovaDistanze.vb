Imports System.Globalization
Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider
Imports Newtonsoft.Json.Linq

Public Class TrovaDistanze_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Distanza_Massima_Poligoni As Decimal,
                          ByVal Entita_Cod_IN_A As String,
                          ByVal Entita_Cod_IN_B As String,
                          ByVal flagAggiuntivo As Boolean,
                          ByVal SoloImpresa As Integer,
                          ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.TrovaDistanze_R.Leggi()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder
        Try
            Dim stb_ins_appoggio As New StringBuilder

            stb_ins_appoggio.AppendLine("-- Tabella Appoggio {0} ")
            stb_ins_appoggio.AppendLine("DECLARE @TabellaAppoggio_{0} TABLE(ElementoGrafico_Cod INT, Poligono GEOGRAPHY, Baricentro GEOGRAPHY, DataInizio DATETIME, DataFine DATETIME, CODICE_FISCALE_TECNICO NVARCHAR(400), ID_Specie INT, ID_SottoSpecie INT, ID_Gruppo INT, ID_Genotipo INT) ")
            stb_ins_appoggio.AppendLine()
            stb_ins_appoggio.AppendLine("INSERT INTO @TabellaAppoggio_{0} ")
            stb_ins_appoggio.AppendLine()
            stb_ins_appoggio.AppendLine("SELECT GIS_ElementiGrafici.ElementoGrafico_Cod, GIS_ElementiGrafici.Poligono_GeoEntity, GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter(), ")
            stb_ins_appoggio.AppendLine("       Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine, Reg_Impianti.CODICE_FISCALE_TECNICO, ")
            stb_ins_appoggio.AppendLine("       Mappatura_Specie.ID_Specie, Mappatura_Specie.ID_SottoSpecie, Mappatura_Specie.ID_Gruppo, Mappatura_Specie.ID_Genotipo ")
            stb_ins_appoggio.AppendLine("FROM       GIS_ElementiGrafici ")
            stb_ins_appoggio.AppendLine("INNER JOIN GIS_Entita          ON GIS_ElementiGrafici.PivaSuperUser = GIS_Entita.PivaSuperUser AND GIS_ElementiGrafici.Entita_Cod = GIS_Entita.Entita_Cod ")
            stb_ins_appoggio.AppendLine("LEFT JOIN  Reg_Impianti        ON Reg_Impianti.PIVA = GIS_Entita.Piva AND Reg_Impianti.SA_COD = GIS_Entita.Sa_Cod AND Reg_Impianti.APPEZZA = GIS_Entita.Appezza AND Reg_Impianti.ID_REG = GIS_Entita.Id_Imp ")
            stb_ins_appoggio.AppendLine("LEFT JOIN  Cultivar            ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
            stb_ins_appoggio.AppendLine("LEFT JOIN  SpecieVegetali      ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            stb_ins_appoggio.AppendLine("LEFT JOIN  Mappatura_Specie    ON Mappatura_Specie.Veg_Cod = SpecieVegetali.Veg_Cod AND ABS(Reg_Impianti.GRVA_Cod_VEG) = Mappatura_Specie.Grva_Cod AND Mappatura_Specie.Hybrid = (CASE WHEN Cultivar.veg_cod IN (6,5000021,70,69) THEN -1 ELSE (CASE WHEN reg_impianti.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END) ")
            stb_ins_appoggio.AppendLine("WHERE GIS_ElementiGrafici.Entita_Cod IN ({1}) ")
            Dim ins_appoggio As String = stb_ins_appoggio.ToString

            stb.Length = 0
            stb.AppendLine("SET NOCOUNT ON ")
            stb.AppendLine()
            stb.AppendLine()
            stb.AppendFormat(ins_appoggio, "A", Entita_Cod_IN_A)
            stb.AppendLine()
            stb.AppendLine()
            stb.AppendFormat(ins_appoggio, "B", Entita_Cod_IN_B)
            stb.AppendLine()
            stb.AppendLine()
            stb.AppendLine("-- Tabella Appoggio Distanze ")
            stb.AppendLine("DECLARE @Distanze TABLE(ElementoGrafico_Cod_1 INT, ElementoGrafico_Cod_2 INT, Distanza FLOAT, ID_Specie_A INT, ID_SottoSpecie_A INT, ID_Gruppo_A INT, ID_Genotipo_A INT, ID_Specie_B INT, ID_SottoSpecie_B INT, ID_Gruppo_B INT, ID_Genotipo_B INT) ")
            stb.AppendLine()
            stb.AppendLine("DECLARE @ElementoGrafico_Cod INT, @Poligono GEOGRAPHY, @Baricentro GEOGRAPHY, @DataInizio DATETIME, @DataFine DATETIME, @CODICE_FISCALE_TECNICO NVARCHAR (25), @ID_Specie INT, @ID_SottoSpecie INT, @ID_Gruppo INT, @ID_Genotipo INT ")
            stb.AppendLine()
            stb.AppendLine("--CURSORE ")
            stb.AppendLine("DECLARE @miocursore CURSOR ")
            stb.AppendLine("SET @miocursore = CURSOR FOR SELECT * FROM @TabellaAppoggio_A ORDER BY ElementoGrafico_Cod ")
            stb.AppendLine()
            stb.AppendLine("OPEN @miocursore ")

            Dim Fetch_cursor As String = "FETCH NEXT FROM @miocursore INTO @ElementoGrafico_Cod, @Poligono, @Baricentro, @DataInizio, @DataFine, @CODICE_FISCALE_TECNICO, @ID_Specie, @ID_SottoSpecie, @ID_Gruppo, @ID_Genotipo "

            stb.AppendLine(Fetch_cursor)
            stb.AppendLine("WHILE @@FETCH_STATUS = 0 ")
            stb.AppendLine("BEGIN ")
            stb.AppendLine()
            stb.AppendLine("INSERT INTO @Distanze ")
            stb.AppendLine()
            stb.AppendLine("SELECT @ElementoGrafico_Cod, ElementoGrafico_Cod, Poligono.STDistance(@Poligono), ")
            stb.AppendLine("    @ID_Specie, @ID_SottoSpecie, @ID_Gruppo, @ID_Genotipo, ID_Specie, ID_SottoSpecie, ID_Gruppo, ID_Genotipo ")
            stb.AppendLine("FROM @TabellaAppoggio_B B ")
            stb.AppendLine("WHERE ElementoGrafico_Cod <> @ElementoGrafico_Cod ")
            If flagAggiuntivo = True Then
                If SoloImpresa = True Then
                    stb.AppendLine("AND @CODICE_FISCALE_TECNICO = CODICE_FISCALE_TECNICO ")
                Else
                    stb.AppendLine("AND @CODICE_FISCALE_TECNICO <> CODICE_FISCALE_TECNICO ")
                End If
            End If
            stb.AppendLine("AND (DataInizio <= @DataFine AND @DataInizio <= DataFine) ")
            stb.AppendLine("AND Baricentro.STDistance(@Baricentro) < " & (Distanza_Massima_Poligoni * 2))
            stb.AppendLine("AND Poligono.STDistance(@Poligono) < " & Distanza_Massima_Poligoni)
            stb.AppendLine("AND NOT EXISTS (SELECT 1 FROM @Distanze D WHERE B.ElementoGrafico_Cod = D.ElementoGrafico_Cod_1 AND @ElementoGrafico_Cod = D.ElementoGrafico_Cod_2) ")
            stb.AppendLine()
            stb.AppendLine(Fetch_cursor)
            stb.AppendLine("END ")
            stb.AppendLine("CLOSE @miocursore ")
            stb.AppendLine("DEALLOCATE @miocursore ")
            stb.AppendLine()
            stb.AppendLine()
            stb.AppendLine("SELECT * FROM @Distanze ")

            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt

    End Function

    'Gabriele - NON USATO (VEDI SOPRA Leggi())
    Public Function Leggi_(
            ByVal Distanza_Massima_Poligoni As Decimal,
            ByVal Entita_Cod_IN_A As String,
            ByVal Entita_Cod_IN_B As String,
            ByVal flagAggiuntivo As Boolean,
            ByVal SoloImpresa As Integer,
            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.TrovaDistanze_R.Leggi()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder
        Dim risp As Boolean
        Try


            stb.Length = 0

            stb.Append("CREATE TABLE [dbo].[TabellaAppoggio_Distanze](   " & vbCrLf)
            stb.Append("    [ElementoGrafico_Cod_1] [int] Not NULL, [ElementoGrafico_Cod_2] [int] Not NULL, [Distanza] [float] NULL,   " & vbCrLf)
            stb.Append("    [VInizio_A] [datetime] null, [VFine_A] [datetime] null, [VInizio_B] [datetime] null, [VFine_B] [datetime] null,   " & vbCrLf)
            stb.Append(" CONSTRAINT [PK_TabellaAppoggio_Distanze] PRIMARY KEY CLUSTERED    " & vbCrLf)
            stb.Append(" (   " & vbCrLf)
            stb.Append("    [ElementoGrafico_Cod_1] ASC, [ElementoGrafico_Cod_2] ASC    " & vbCrLf)
            stb.Append(" )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]   " & vbCrLf)
            stb.Append(" ) ON [PRIMARY]   " & vbCrLf)
            stb.Append(" -- indici " & vbCrLf)
            stb.Append(" CREATE NONCLUSTERED INDEX [ElementoGrafico_cod_1] ON [dbo].[TabellaAppoggio_Distanze]   " & vbCrLf)
            stb.Append(" ( [ElementoGrafico_Cod_1] Asc )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]  " & vbCrLf)
            stb.Append(" CREATE NONCLUSTERED INDEX [ElementoGrafico_Cod_2] ON [dbo].[TabellaAppoggio_Distanze]   " & vbCrLf)
            stb.Append(" ( [ElementoGrafico_Cod_2] Asc )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)


            stb.Append("--TAB_A  " & vbCrLf)
            stb.Append(" CREATE TABLE [dbo].[TabellaAppoggio_ElementoGraficoCod_A](   " & vbCrLf)
            stb.Append(" [ElementoGrafico_Cod] [int] Not NULL, [Poligono_GeoEntity] [geography] NULL, [Baricentro] [geography] NULL,   " & vbCrLf)
            stb.Append(" [DataInizio] [datetime] null, [DataFine] [datetime] null, [CODICE_FISCALE_TECNICO] [nvarchar] (400), " & vbCrLf)
            stb.Append(" CONSTRAINT [PK_TabellaAppoggio_ElementoGraficoCod_A] PRIMARY KEY CLUSTERED    " & vbCrLf)
            stb.Append(" ( [ElementoGrafico_Cod] Asc )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]   " & vbCrLf)
            stb.Append(" ) ON [PRIMARY]   " & vbCrLf)
            stb.Append(" CREATE SPATIAL INDEX [indice_baricentro_A] ON [dbo].[TabellaAppoggio_ElementoGraficoCod_A]    " & vbCrLf)
            stb.Append(" ( [Baricentro] )USING  GEOGRAPHY_GRID    " & vbCrLf)
            stb.Append(" WITH ( GRIDS =(LEVEL_1 = HIGH,LEVEL_2 = HIGH,LEVEL_3 = HIGH,LEVEL_4 = HIGH),    " & vbCrLf)
            stb.Append(" CELLS_PER_OBJECT = 2, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" -- Inserisco A " & vbCrLf)
            stb.Append(" INSERT INTO [TabellaAppoggio_ElementoGraficoCod_A]   " & vbCrLf)
            stb.Append(" ( [ElementoGrafico_Cod], [Poligono_GeoEntity], [Baricentro],[DataInizio] , [DataFine],  [CODICE_FISCALE_TECNICO] )   " & vbCrLf)
            stb.Append(" SELECT     GIS_ElementiGrafici.ElementoGrafico_Cod, GIS_ElementiGrafici.Poligono_GeoEntity, GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter() AS Baricentro,  " & vbCrLf)
            stb.Append("                       ISNULL(Reg_Impianti.Validita_Inizio, Programmazione_Entita.Validita_Inizio) AS ValiditaInizio,   " & vbCrLf)
            stb.Append("                       ISNULL(Reg_Impianti.Validita_Fine, Programmazione_Entita.Validita_Fine) AS ValiditaFine, " & vbCrLf)
            stb.Append("                       ISNULL(Programmazione_Entita.Codice_Fiscale_Tecnico, Reg_Impianti.CODICE_FISCALE_TECNICO)  as CODICE_FISCALE_TECNICO " & vbCrLf)
            stb.Append(" FROM         GIS_ElementiGrafici INNER JOIN " & vbCrLf)
            stb.Append("                       GIS_Entita ON GIS_ElementiGrafici.PivaSuperUser = GIS_Entita.PivaSuperUser And GIS_ElementiGrafici.Entita_Cod = GIS_Entita.Entita_Cod LEFT OUTER JOIN " & vbCrLf)
            stb.Append("                       Programmazione_Entita ON GIS_Entita.PivaSuperUser = Programmazione_Entita.Piva_SuperUser And  " & vbCrLf)
            stb.Append("                       GIS_Entita.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod LEFT OUTER JOIN " & vbCrLf)
            stb.Append("                       Reg_Impianti ON GIS_Entita.Piva = Reg_Impianti.PIVA And GIS_Entita.Sa_Cod = Reg_Impianti.SA_COD And GIS_Entita.Appezza = Reg_Impianti.APPEZZA And  " & vbCrLf)
            stb.Append("                       GIS_Entita.Id_Imp = Reg_Impianti.ID_REG " & vbCrLf)
            stb.Append(" where GIS_ElementiGrafici.ENTITA_COD in (" & Agro_SQL_Save_Clausola_IN(Entita_Cod_IN_A) & ")  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)



            stb.Append("--TAB_B  " & vbCrLf)
            stb.Append(" CREATE TABLE [dbo].[TabellaAppoggio_ElementoGraficoCod_B](   " & vbCrLf)
            stb.Append(" [ElementoGrafico_Cod] [int] Not NULL, [Poligono_GeoEntity] [geography] NULL, [Baricentro] [geography] NULL,   " & vbCrLf)
            stb.Append(" [DataInizio] [datetime] null, [DataFine] [datetime] null,   [CODICE_FISCALE_TECNICO] [nvarchar] (400)," & vbCrLf)
            stb.Append(" CONSTRAINT [PK_TabellaAppoggio_ElementoGraficoCod_B] PRIMARY KEY CLUSTERED    " & vbCrLf)
            stb.Append(" (  [ElementoGrafico_Cod] Asc )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]   " & vbCrLf)
            stb.Append(" ) ON [PRIMARY]   " & vbCrLf)
            stb.Append(" CREATE SPATIAL INDEX [indice_baricentro] ON [dbo].[TabellaAppoggio_ElementoGraficoCod_B]    " & vbCrLf)
            stb.Append(" ( [Baricentro] )USING  GEOGRAPHY_GRID    " & vbCrLf)
            stb.Append(" WITH ( GRIDS =(LEVEL_1 = HIGH,LEVEL_2 = HIGH,LEVEL_3 = HIGH,LEVEL_4 = HIGH),  " & vbCrLf)
            stb.Append(" CELLS_PER_OBJECT = 2, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" -- Inserisco B " & vbCrLf)
            stb.Append(" INSERT INTO [TabellaAppoggio_ElementoGraficoCod_B]   " & vbCrLf)
            stb.Append(" ( [ElementoGrafico_Cod], [Poligono_GeoEntity], [Baricentro],[DataInizio] , [DataFine] , [CODICE_FISCALE_TECNICO])   " & vbCrLf)
            stb.Append(" SELECT     GIS_ElementiGrafici.ElementoGrafico_Cod, GIS_ElementiGrafici.Poligono_GeoEntity, GIS_ElementiGrafici.Poligono_GeoEntity.EnvelopeCenter() AS Baricentro,  " & vbCrLf)
            stb.Append("                       ISNULL(Reg_Impianti.Validita_Inizio, Programmazione_Entita.Validita_Inizio) AS ValiditaInizio,   " & vbCrLf)
            stb.Append("                       ISNULL(Reg_Impianti.Validita_Fine, Programmazione_Entita.Validita_Fine) AS ValiditaFine, " & vbCrLf)
            stb.Append("                       ISNULL(Programmazione_Entita.Codice_Fiscale_Tecnico, Reg_Impianti.CODICE_FISCALE_TECNICO)  as CODICE_FISCALE_TECNICO " & vbCrLf)
            stb.Append(" FROM         GIS_ElementiGrafici INNER JOIN " & vbCrLf)
            stb.Append("                       GIS_Entita ON GIS_ElementiGrafici.PivaSuperUser = GIS_Entita.PivaSuperUser And GIS_ElementiGrafici.Entita_Cod = GIS_Entita.Entita_Cod LEFT OUTER JOIN " & vbCrLf)
            stb.Append("                       Programmazione_Entita ON GIS_Entita.PivaSuperUser = Programmazione_Entita.Piva_SuperUser And  " & vbCrLf)
            stb.Append("                       GIS_Entita.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod LEFT OUTER JOIN " & vbCrLf)
            stb.Append("                       Reg_Impianti ON GIS_Entita.Piva = Reg_Impianti.PIVA And GIS_Entita.Sa_Cod = Reg_Impianti.SA_COD And GIS_Entita.Appezza = Reg_Impianti.APPEZZA And  " & vbCrLf)
            stb.Append("                       GIS_Entita.Id_Imp = Reg_Impianti.ID_REG " & vbCrLf)
            stb.Append(" where GIS_ElementiGrafici.ENTITA_COD in (" & Agro_SQL_Save_Clausola_IN(Entita_Cod_IN_B) & ")  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)


            stb.Append(" --VARIABILI " & vbCrLf)
            stb.Append(" DECLARE @distanza_massima_Baricentro INT   " & vbCrLf)
            stb.Append(" set @distanza_massima_Baricentro = " & (Distanza_Massima_Poligoni * 2) & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" DECLARE @ElementoGraficoCorrente INT   " & vbCrLf)
            stb.Append(" DECLARE @poligono GEOGRAPHY   " & vbCrLf)
            stb.Append(" declare @baricentro GEOGRAPHY   " & vbCrLf)
            stb.Append(" DECLARE @vInizio datetime " & vbCrLf)
            stb.Append(" DECLARE @vFine datetime " & vbCrLf)
            stb.Append(" DECLARE @CODICE_FISCALE_TECNICO nvarchar (25)" & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" --cursore   " & vbCrLf)
            stb.Append(" DECLARE @miocursore  CURSOR   " & vbCrLf)
            stb.Append(" SET @miocursore = CURSOR FOR   " & vbCrLf)
            stb.Append(" SELECT *  " & vbCrLf)
            stb.Append(" FROM [TabellaAppoggio_ElementoGraficoCod_A]   " & vbCrLf)
            stb.Append(" order by ElementoGrafico_Cod   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" OPEN @miocursore    " & vbCrLf)
            stb.Append(" FETCH NEXT   " & vbCrLf)
            stb.Append(" FROM @miocursore  INTO @ElementoGraficoCorrente , @poligono , @baricentro , @vInizio , @vFine , @CODICE_FISCALE_TECNICO   " & vbCrLf)
            stb.Append(" WHILE @@FETCH_STATUS = 0   " & vbCrLf)
            stb.Append(" BEGIN   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INSERT INTO [TabellaAppoggio_Distanze]   " & vbCrLf)
            stb.Append("    ( [ElementoGrafico_Cod_1], [ElementoGrafico_Cod_2], [Distanza], [VInizio_A] , [VFine_A] , [VInizio_B] , [VFine_B])   " & vbCrLf)
            stb.Append("        select  @ElementoGraficoCorrente as [ElementoGrafico_Cod_1], ElementoGrafico_Cod as [ElementoGrafico_Cod_2] , Poligono_GeoEntity.STDistance(@poligono) as [Distanza] ,    " & vbCrLf)
            stb.Append("                @vInizio as [VInizio_A], @vFine as [VFine_A] ,     " & vbCrLf)
            stb.Append("                DataInizio as [VInizio_B], DataFine as [VFine_B]     " & vbCrLf)
            stb.Append("        from [TabellaAppoggio_ElementoGraficoCod_B] as B  " & vbCrLf)
            stb.Append("        where  ElementoGrafico_Cod <> @ElementoGraficoCorrente   " & vbCrLf)
            If flagAggiuntivo = True Then
                If SoloImpresa = True Then
                    stb.Append("            And @CODICE_FISCALE_TECNICO = CODICE_FISCALE_TECNICO " & vbCrLf)
                Else
                    stb.Append("            And @CODICE_FISCALE_TECNICO <> CODICE_FISCALE_TECNICO " & vbCrLf)
                End If
            End If
            stb.Append("            And ((DataInizio >=@vInizio  And DataInizio <= @vFine ) Or (DataFine >=@vInizio  And DataFine <= @vFine ) Or (@vInizio >= DataInizio And @vInizio <= DataFine) Or (@vFine >= DataInizio And @vFine <= DataFine))   " & vbCrLf)
            stb.Append("            And  Not exists ( select 1  from [TabellaAppoggio_Distanze] as a where b.ElementoGrafico_Cod = a.ElementoGrafico_Cod_1 And @ElementoGraficoCorrente = a.ElementoGrafico_Cod_2 )  " & vbCrLf)
            stb.Append("        And Baricentro.STDistance(@baricentro) < @distanza_massima_Baricentro    " & vbCrLf)
            stb.Append("        And Poligono_GeoEntity.STDistance(@poligono) < " & Distanza_Massima_Poligoni & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" FETCH NEXT   " & vbCrLf)
            stb.Append(" FROM @miocursore  INTO @ElementoGraficoCorrente , @poligono , @baricentro , @vInizio , @vFine , @CODICE_FISCALE_TECNICO    " & vbCrLf)
            stb.Append(" End " & vbCrLf)
            stb.Append(" CLOSE @miocursore    " & vbCrLf)
            stb.Append(" DEALLOCATE @miocursore    " & vbCrLf)


            risp = EseguiQuery_Scrittura(objParametri_server, stb.ToString, NomeRoutine)

            stb.Length = 0


            stb.Append(" select Distanze.* ,  " & vbCrLf)
            stb.Append(" DATI_A.ID_Specie as ID_Specie_A, DATI_A.ID_SottoSpecie as ID_SottoSpecie_A,  " & vbCrLf)
            stb.Append(" DATI_A.ID_Gruppo as ID_Gruppo_A, DATI_A.ID_Genotipo as ID_Genotipo_A,  " & vbCrLf)
            stb.Append(" DATI_B.ID_Specie as ID_Specie_B, DATI_B.ID_SottoSpecie as ID_SottoSpecie_B,  " & vbCrLf)
            stb.Append(" DATI_B.ID_Gruppo as ID_Gruppo_B, DATI_B.ID_Genotipo as ID_Genotipo_B  " & vbCrLf)
            stb.Append(" from [TabellaAppoggio_Distanze]  Distanze " & vbCrLf)
            stb.Append(" INNER JOIN " & vbCrLf)
            stb.Append(" (  " & vbCrLf)


            stb.Append(" SELECT DISTINCT  GIS_Entita.Entita_Cod AS UNID_Impianto,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_3.id_specie ,Mappatura_Specie_2.id_specie ) as ID_Specie,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.ID_SottoSpecie,Mappatura_Specie.ID_SottoSpecie ) as ID_SottoSpecie,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.ID_Gruppo, Mappatura_Specie.ID_Gruppo ) as ID_Gruppo , " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.ID_Genotipo,Mappatura_Specie.ID_Genotipo)as ID_Genotipo,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.Raggruppamento,Mappatura_Specie.Raggruppamento) as Raggruppamento,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.Hybrid, Mappatura_Specie.Hybrid) AS Hybrid  " & vbCrLf)
            stb.Append("    FROM Cultivar  " & vbCrLf)
            stb.Append("        LEFT JOIN Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD  " & vbCrLf)
            stb.Append("        LEFT JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie as Mappatura_specie_2 on SpecieVegetali.Veg_Cod = Mappatura_specie_2.Veg_Cod " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie ON SpecieVegetali.Veg_Cod = Mappatura_Specie.Veg_Cod And ABS(Reg_Impianti.GRVA_Cod_VEG) = Mappatura_Specie.Grva_Cod And Mappatura_Specie.Hybrid  = (CASE WHEN Cultivar.veg_cod in (6,5000021,70,69) THEN -1 ELSE (CASE WHEN reg_impianti.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END) " & vbCrLf)
            stb.Append("        RIGHT OUTER JOIN GIS_Entita  " & vbCrLf)
            stb.Append("        LEFT OUTER JOIN GIS_ElementiGrafici ON GIS_Entita.PivaSuperUser = GIS_ElementiGrafici.PivaSuperUser And GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod  " & vbCrLf)
            stb.Append("        LEFT OUTER JOIN Programmazione_Entita  " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie as Mappatura_specie_3 on Programmazione_Entita.Veg_Cod = Mappatura_specie_3.Veg_Cod " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie AS Mappatura_Specie_1 ON Programmazione_Entita.Veg_Cod = Mappatura_Specie_1.Veg_Cod And ABS(Programmazione_Entita.Grva_Cod) = Mappatura_Specie_1.Grva_Cod And Mappatura_Specie_1.Hybrid = (CASE WHEN Programmazione_Entita.GRVA_Cod < 0 THEN '1' ELSE '0' END)  ON GIS_Entita.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod ON Reg_Impianti.PIVA = GIS_Entita.Piva AND Reg_Impianti.SA_COD = GIS_Entita.Sa_Cod AND Reg_Impianti.APPEZZA = GIS_Entita.Appezza AND Reg_Impianti.ID_REG = GIS_Entita.Id_Imp " & vbCrLf)


            stb.Append(" ) DATI_A on DATI_A.UNID_Impianto = Distanze.ElementoGrafico_Cod_1  " & vbCrLf)
            stb.Append(" INNER JOIN " & vbCrLf)
            stb.Append(" (  " & vbCrLf)


            stb.Append(" SELECT DISTINCT  GIS_Entita.Entita_Cod AS UNID_Impianto,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_3.id_specie ,Mappatura_Specie_2.id_specie ) as ID_Specie,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.ID_SottoSpecie,Mappatura_Specie.ID_SottoSpecie ) as ID_SottoSpecie,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.ID_Gruppo, Mappatura_Specie.ID_Gruppo ) as ID_Gruppo , " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.ID_Genotipo,Mappatura_Specie.ID_Genotipo)as ID_Genotipo,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.Raggruppamento,Mappatura_Specie.Raggruppamento) as Raggruppamento,  " & vbCrLf)
            stb.Append("    isnull(Mappatura_Specie_1.Hybrid, Mappatura_Specie.Hybrid) AS Hybrid  " & vbCrLf)
            stb.Append("    FROM Cultivar  " & vbCrLf)
            stb.Append("        LEFT JOIN Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD  " & vbCrLf)
            stb.Append("        LEFT JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie as Mappatura_specie_2 on SpecieVegetali.Veg_Cod = Mappatura_specie_2.Veg_Cod " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie ON SpecieVegetali.Veg_Cod = Mappatura_Specie.Veg_Cod AND ABS(Reg_Impianti.GRVA_Cod_VEG) = Mappatura_Specie.Grva_Cod AND Mappatura_Specie.Hybrid  = (CASE WHEN Cultivar.veg_cod in (6,5000021,70,69) THEN -1 ELSE (CASE WHEN reg_impianti.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END) " & vbCrLf)
            stb.Append("        RIGHT OUTER JOIN GIS_Entita  " & vbCrLf)
            stb.Append("        LEFT OUTER JOIN GIS_ElementiGrafici ON GIS_Entita.PivaSuperUser = GIS_ElementiGrafici.PivaSuperUser AND GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod  " & vbCrLf)
            stb.Append("        LEFT OUTER JOIN Programmazione_Entita  " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie as Mappatura_specie_3 on Programmazione_Entita.Veg_Cod = Mappatura_specie_3.Veg_Cod " & vbCrLf)
            stb.Append("        LEFT JOIN Mappatura_Specie AS Mappatura_Specie_1 ON Programmazione_Entita.Veg_Cod = Mappatura_Specie_1.Veg_Cod AND ABS(Programmazione_Entita.Grva_Cod) = Mappatura_Specie_1.Grva_Cod AND Mappatura_Specie_1.Hybrid = (CASE WHEN Programmazione_Entita.GRVA_Cod < 0 THEN '1' ELSE '0' END)  ON GIS_Entita.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod ON Reg_Impianti.PIVA = GIS_Entita.Piva AND Reg_Impianti.SA_COD = GIS_Entita.Sa_Cod AND Reg_Impianti.APPEZZA = GIS_Entita.Appezza AND Reg_Impianti.ID_REG = GIS_Entita.Id_Imp " & vbCrLf)


            stb.Append(" ) DATI_b on DATI_b.UNID_Impianto = Distanze.ElementoGrafico_Cod_2 " & vbCrLf)

            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)

            stb.Length = 0
            stb.Append(" drop table [TabellaAppoggio_Distanze] " & vbCrLf)
            stb.Append(" drop table [TabellaAppoggio_ElementoGraficoCod_A] " & vbCrLf)
            stb.Append(" drop table [TabellaAppoggio_ElementoGraficoCod_B] " & vbCrLf)

            risp = EseguiQuery_Scrittura(objParametri_server, stb.ToString, NomeRoutine)

            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing

            stb.Length = 0
            stb.Append(" drop table [TabellaAppoggio_Distanze] " & vbCrLf)
            stb.Append(" drop table [TabellaAppoggio_ElementoGraficoCod_A] " & vbCrLf)
            stb.Append(" drop table [TabellaAppoggio_ElementoGraficoCod_B] " & vbCrLf)

            risp = EseguiQuery_Scrittura(objParametri_server, stb.ToString, NomeRoutine)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

End Class





Public Class VerificaInterferenza

    Private _ID_Specie_A As Integer
    Private _ID_Sottospecie_A As Integer
    Private _ID_Gruppo_A As Integer
    Private _ID_Genotipo_A As Integer

    Private _ID_Specie_B As Integer
    Private _ID_Sottospecie_B As Integer
    Private _ID_Gruppo_B As Integer
    Private _ID_Genotipo_B As Integer

    Private _TipoInterferenza As Integer    '0 -> No Interferenza, 1 -> Possibile Interferenza, 2 -> Interferenza 
    Private _DistanzaDiLegge As Double
    Private _DistanzaDiLegge_BietolaBase As Double
    Private _DistanzaDiLegge_BietolaCert As Double
    Private _Motivazione_Des As String

    Private _DT_Mappatura_Specie_Distanze As DataTable
    Private _DT_Distanze_AUX As DataTable

    Private Function InformazioniSufficienti() As Boolean
        Return _ID_Specie_A <> 0 AndAlso _ID_Sottospecie_A <> 0 AndAlso _ID_Gruppo_A <> 0 AndAlso _ID_Specie_B <> 0 AndAlso _ID_Sottospecie_B <> 0 AndAlso _ID_Gruppo_B <> 0
    End Function
    Private Function Informazioni_from_DT_Distanze_AUX(ByVal ID_Specie As Integer, ByVal ID_Gruppo As Integer) As Integer
        Dim Risultato As DataRow()
        Dim StrSelect As String = " ID_Specie = " & ID_Specie & " AND ID_Gruppo = " & ID_Gruppo

        Risultato = _DT_Distanze_AUX.Select(StrSelect)

        If Risultato.GetUpperBound(0) >= 0 Then
            Return Risultato(0)("Div_Varieta_in_Gruppo")
        End If

        Return 999999

    End Function
    Public Sub Set_Specie_A(ByVal val As Object)
        _ID_Specie_A = If(IsDBNull(val), 0, val)
    End Sub
    Public Sub Set_Sottospecie_A(ByVal val As Object)
        _ID_Sottospecie_A = If(IsDBNull(val), 0, val)
    End Sub
    Public Sub Set_Gruppo_A(ByVal val As Object)
        _ID_Gruppo_A = If(IsDBNull(val), 0, val)
    End Sub
    Public Sub Set_Genotipo_A(ByVal val As Object)
        _ID_Genotipo_A = If(IsDBNull(val), 0, val)
    End Sub
    Public Sub Set_Specie_B(ByVal val As Object)
        _ID_Specie_B = If(IsDBNull(val), 0, val)
    End Sub
    Public Sub Set_Sottospecie_B(ByVal val As Object)
        _ID_Sottospecie_B = If(IsDBNull(val), 0, val)
    End Sub
    Public Sub Set_Gruppo_B(ByVal val As Object)
        _ID_Gruppo_B = If(IsDBNull(val), 0, val)
    End Sub
    Public Sub Set_Genotipo_B(ByVal val As Object)
        _ID_Genotipo_B = If(IsDBNull(val), 0, val)
    End Sub
    Public ReadOnly Property TipoInterferenza As Integer
        Get
            Return _TipoInterferenza
        End Get
    End Property
    Public ReadOnly Property Motivazione_Des As String
        Get
            Return _Motivazione_Des
        End Get
    End Property
    Public ReadOnly Property DistanzaDiLegge As Double
        Get
            Return _DistanzaDiLegge
        End Get
    End Property
    Public ReadOnly Property DistanzaDiLegge_BietolaBase As Double
        Get
            Return _DistanzaDiLegge_BietolaBase
        End Get
    End Property
    Public ReadOnly Property DistanzaDiLegge_BietolaCert As Double
        Get
            Return _DistanzaDiLegge_BietolaCert
        End Get
    End Property

    Public Sub New(ByVal IDs_Specie_In As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        _ID_Specie_A = 0
        _ID_Sottospecie_A = 0
        _ID_Gruppo_A = 0
        _ID_Genotipo_A = 0

        _ID_Specie_B = 0
        _ID_Sottospecie_B = 0
        _ID_Gruppo_B = 0
        _ID_Genotipo_B = 0

        _TipoInterferenza = 0
        _DistanzaDiLegge = 0
        _DistanzaDiLegge_BietolaBase = 0
        _DistanzaDiLegge_BietolaCert = 0
        _Motivazione_Des = ""

        Dim objVarie As New AgronicaCoreSementieriDAL.Varie_R
        _DT_Mappatura_Specie_Distanze = objVarie.Leggi_Mappatura_Specie_Distanze(IDs_Specie_In, objParametri)
        _DT_Distanze_AUX = objVarie.Leggi_Mappatura_Specie_Distanze_Bietola(IDs_Specie_In, objParametri)

    End Sub

    Public Function DistanzaMassima() As Double
        Dim result As Double = 0
        For Each dr In _DT_Mappatura_Specie_Distanze.Rows
            result = Math.Max(result, dr.Item("Distanza_MaxValue"))
        Next

        If _DT_Distanze_AUX.Rows.Count > 0 Then
            For Each dr In _DT_Distanze_AUX.Rows
                result = Math.Max(result, dr.Item("Div_Varieta_in_Gruppo"))
            Next
        End If

        Return result
    End Function

    Public Sub Verifica(ByVal DistanzaEffettiva As Double, ByVal Moltiplicatore As Double)

        _TipoInterferenza = 0
        _DistanzaDiLegge = 0
        _DistanzaDiLegge_BietolaBase = 0
        _DistanzaDiLegge_BietolaCert = 0
        '_Motivazione_Cod = 0
        _Motivazione_Des = ""

        If _ID_Specie_A <> _ID_Specie_B Then
            'se le specie sono diverse non c'è interferenza
            Return
        End If

        Dim DR_Dist() As DataRow
        DR_Dist = _DT_Mappatura_Specie_Distanze.Select("ID_Specie = " & _ID_Specie_A)

        'If If ViewState("bietola") = True Then
        If _ID_Specie_A = 1 Or _ID_Specie_A = 21 Then
            'BIETOLA
            'controllo se ho abbastanza info 
            If Not InformazioniSufficienti() Then

                'Mi procuro la distanza maggiore
                Dim DistanzaMinima = DR_Dist(0).Item("Distanza_MaxValue")
                For Each dr In _DT_Distanze_AUX.Rows
                    DistanzaMinima = Math.Max(DistanzaMinima, dr.Item("Div_Varieta_in_Gruppo"))
                Next

                'Bietola - Informazioni Incomplete - Possibile INTERFERENZA 
                _DistanzaDiLegge = DistanzaMinima
                _TipoInterferenza = 1 'Possibile INTERFERENZA 
                '_Motivazione_Cod = 1 'Bietola - Informazioni Incomplete
                _Motivazione_Des = "Poichè le informazioni fornite sono incomplete, " & "<br>" &
                        "si è preso in considerazione il caso peggiore... " & "<br>" &
                        "È necessario analizzare più approfonditamente " & "<br>" &
                        "questa possibile interferenza !!!"

                Return
            End If

            'Se ho informazioni complete ...
            If _ID_Gruppo_A = _ID_Gruppo_B Then

                'Elementi ausiliari ...
                _DistanzaDiLegge_BietolaBase = Informazioni_from_DT_Distanze_AUX(_ID_Specie_A, _ID_Gruppo_A)

                'Bietola - Informazioni Complete - Stesso GRUPPO - Plurigerme
                _DistanzaDiLegge = _DistanzaDiLegge_BietolaBase
                _TipoInterferenza = 1    'Possibile INTERFERENZA 
                '_Motivazione_Cod = 2     'Bietola - Informazioni Complete - Stesso GRUPPO - Plurigerme
                _Motivazione_Des = "<b>Cat. BASE e PRE-BASE :</b> <br>Stesso GRUPPO - PLURIGERME"

                'Se ottengo 999999 allora non e' PLURIGERME ...
                If (_DistanzaDiLegge_BietolaBase = 999999) Then
                    'Provo con -1
                    _DistanzaDiLegge_BietolaBase = Informazioni_from_DT_Distanze_AUX(_ID_Specie_A, -1)

                    'Bietola - Informazioni Complete - Stesso GRUPPO - Non Plurigerme
                    _DistanzaDiLegge = _DistanzaDiLegge_BietolaBase
                    _TipoInterferenza = 1    'Possibile INTERFERENZA 
                    '_Motivazione_Cod = 3     'Bietola - Informazioni Complete - Stesso GRUPPO - Non Plurigerme
                    _Motivazione_Des = "<b>Cat. BASE e PRE-BASE :</b> <br>Stesso GRUPPO - NON-PLURIGERME"
                End If

            Else
                'Se il gruppo e' diverso allora ...
                _DistanzaDiLegge_BietolaBase = Informazioni_from_DT_Distanze_AUX(_ID_Specie_A, -1)

                'Bietola - Informazioni Complete - GRUPPO Diverso 
                _DistanzaDiLegge = _DistanzaDiLegge_BietolaBase
                _TipoInterferenza = 1    'Possibile INTERFERENZA 
                '_Motivazione_Cod = 4     'Bietola - Informazioni Complete - GRUPPO Diverso 
                _Motivazione_Des = "<b>Cat. BASE e PRE-BASE :</b> <br>GRUPPI diversi"

            End If  '(ID_Gruppo_A = ID_Gruppo_B)

            '////////////////////////////////////////////
            '///  Semente di Categoria CERTIFICATA  /////
            '////////////////////////////////////////////
            If _ID_Gruppo_A = _ID_Gruppo_B Then
                _DistanzaDiLegge_BietolaCert = DR_Dist(0).Item("Div_Varieta_in_Gruppo_OP")
                _Motivazione_Des += "<br><br><b>Cat. CERTIFICATA :</b> <br>Stesso GRUPPO" & vbCrLf
            Else
                If _ID_Sottospecie_A = _ID_Sottospecie_B Then
                    _DistanzaDiLegge_BietolaCert = DR_Dist(0).Item("Div_Gruppi_in_SottoSpecie_OP")
                    _Motivazione_Des += "<br><br><b>Cat. CERTIFICATA :</b> <br>GRUPPI diversi - stessa SOTTOSPECIE" & vbCrLf
                Else
                    _DistanzaDiLegge_BietolaCert = DR_Dist(0).Item("Div_SottoSpecie_in_Specie_OP")
                    _Motivazione_Des += "<br><br><b>Cat. CERTIFICATA :</b> <br>SOTTOSPECIE diverse" & vbCrLf
                End If
            End If

            _DistanzaDiLegge = Math.Max(_DistanzaDiLegge_BietolaCert, _DistanzaDiLegge_BietolaBase)

            'Se la DistanzaEffettiva e' inferiore ad almeno una delle distanze minime (modificate) ...
            If (DistanzaEffettiva < Moltiplicatore * _DistanzaDiLegge_BietolaBase) Or
                   (DistanzaEffettiva < Moltiplicatore * _DistanzaDiLegge_BietolaCert) Then

                'Se la DistanzaEffettiva e' inferiore ad entrambe le distanze minime (modificate) ...
                If (DistanzaEffettiva < Moltiplicatore * _DistanzaDiLegge_BietolaBase) And
                   (DistanzaEffettiva < Moltiplicatore * _DistanzaDiLegge_BietolaCert) Then

                    '=====> INTERFERENZA Bietola
                    _TipoInterferenza = 2
                Else

                    '=====> POSSIBILE INTERFERENZA Bietola   (dipende dal tipo di produzione BASE, PREBASE o CERTIFICATA)
                    _TipoInterferenza = 1
                End If

            End If

            Return
        End If



        'Implementazione provvisoria per la SEGALE...
        'quando sarà implementata una struttura dati che permetta la gesstione dei diversi casi possibili (BIETOLA, BARBABIETOLA, SEGALE, altre...)
        'sarà da modificare l'intera funzione... (possibile anche una implementazione solo da query)
        'così come è implementata non si può vedere...
        'Vedere anche in GIS_OperazioniCartograficheDB -> Elabora_Datatable_Interferenze (Codice quasi duplicato!!!)
        If _ID_Specie_A = 23 Then
            'SEGALE
            Dim ddl_PreBase As Double = 2000
            Dim ddl_Base As Double = 1000
            Dim ddl_Certificata As Double = 500
            Dim strVarieta As String = "Varietà con valori OP/HY diversi"
            Dim strCategoria As String = If(_ID_Gruppo_A = _ID_Gruppo_B, "stessa Categoria", "CATEGORIA diversa")

            If _ID_Genotipo_A = _ID_Genotipo_B And _ID_Genotipo_A = 0 Then
                'Stessa Classe OP
                strVarieta = "Varietà OP"
            Else
                'Stessa Classe HY oppure Classi diverse OP/HY

                ddl_PreBase = 5000
                ddl_Base = 3000
                ddl_Certificata = 2000

                If _ID_Genotipo_A = _ID_Genotipo_B Then
                    'Stessa Classe HY
                    strVarieta = "Varietà HY"
                Else
                    'Classi diverse OP/HY
                End If
            End If

            If _ID_Gruppo_A = _ID_Gruppo_B Then
                If _ID_Gruppo_A = 223 Then
                    'Pre-Base
                    _DistanzaDiLegge = ddl_PreBase
                ElseIf _ID_Gruppo_A = 224 Then
                    'Base
                    _DistanzaDiLegge = ddl_Base
                Else
                    'Cerfificata
                    _DistanzaDiLegge = ddl_Certificata
                End If
            Else
                If _ID_Gruppo_A = 223 Or _ID_Gruppo_B = 223 Then
                    _DistanzaDiLegge = ddl_PreBase
                ElseIf _ID_Gruppo_A = 224 Or _ID_Gruppo_B = 224 Then
                    _DistanzaDiLegge = ddl_Base
                Else
                    _DistanzaDiLegge = ddl_Certificata
                End If
            End If

            If DistanzaEffettiva < (_DistanzaDiLegge * Moltiplicatore) Then
                _TipoInterferenza = 2
            End If

            _Motivazione_Des = strVarieta & " - " & strCategoria

            Return
        End If



        'Implementazione provvisoria per il PORRO EUROPEO...
        'Vedi commenti per SEGALE
        If _ID_Specie_A = 24 Then
            'PORRO EUROPEO

            _DistanzaDiLegge = 2000

            Dim strVarieta As String = "Varietà con valori OP/HY diversi"
            'Dim strCategoria As String = If(_ID_Gruppo_A = _ID_Gruppo_B, "stessa Categoria", "CATEGORIA diversa")

            If _ID_Genotipo_A = _ID_Genotipo_B And _ID_Genotipo_A = 0 Then
                'Stessa Classe OP
                strVarieta = "Varietà OP"

                _DistanzaDiLegge = 1000
            Else
                'Stessa Classe HY oppure Classi diverse OP/HY

                If _ID_Genotipo_A = _ID_Genotipo_B Then
                    'Stessa Classe HY
                    strVarieta = "Varietà HY"
                Else
                    'Classi diverse OP/HY
                End If

            End If

            If DistanzaEffettiva < (_DistanzaDiLegge * Moltiplicatore) Then

                _TipoInterferenza = 2
            End If

            _Motivazione_Des = strVarieta '& " - " & strCategoria

            Return
        End If



        'Implementazione provvisoria per il PORRO ORIENTALE...
        'Vedi commenti per SEGALE
        If _ID_Specie_A = 25 Then
            'PORRO ORIENTALE

            _DistanzaDiLegge = 1000

            Dim strVarieta As String = "Varietà con valori OP/HY diversi"
            'Dim strCategoria As String = If(_ID_Gruppo_A = _ID_Gruppo_B, "stessa Categoria", "CATEGORIA diversa")

            If _ID_Genotipo_A = _ID_Genotipo_B And _ID_Genotipo_A = 0 Then
                'Stessa Classe OP
                strVarieta = "Varietà OP"
            Else
                'Stessa Classe HY oppure Classi diverse OP/HY

                If _ID_Genotipo_A = _ID_Genotipo_B Then
                    'Stessa Classe HY
                    strVarieta = "Varietà HY"
                Else
                    'Classi diverse OP/HY
                End If
            End If

            If DistanzaEffettiva < (_DistanzaDiLegge * Moltiplicatore) Then

                _TipoInterferenza = 2
            End If

            _Motivazione_Des = strVarieta '& " - " & strCategoria

            Return
        End If



        'Implementazione provvisoria per il BUNCHING ONION...
        'Vedi commenti per SEGALE
        If _ID_Specie_A = 26 Then
            'BUNCHING ONION

            _DistanzaDiLegge = 1000

            Dim strVarieta As String = "Varietà con valori OP/HY diversi"
            Dim strCategoria As String = If(_ID_Gruppo_A = _ID_Gruppo_B, "stessa Categoria", "CATEGORIA diversa")

            If _ID_Genotipo_A = _ID_Genotipo_B And _ID_Genotipo_A = 0 Then
                'Stessa Classe OP
                strVarieta = "Varietà OP"
            Else
                'Stessa Classe HY oppure Classi diverse OP/HY

                _DistanzaDiLegge = 2000

                If _ID_Genotipo_A = _ID_Genotipo_B Then
                    'Stessa Classe HY
                    strVarieta = "Varietà HY"
                Else
                    'Classi diverse OP/HY
                End If
            End If

            If DistanzaEffettiva < (_DistanzaDiLegge * Moltiplicatore) Then

                _TipoInterferenza = 2
            End If

            _Motivazione_Des = strVarieta & " - " & strCategoria

            Return
        End If



        '###############################################################
        '###############################################################
        '#####  ALTRE SPECIE  ##########################################
        '###############################################################
        '###############################################################
        If Not InformazioniSufficienti() Then
            _DistanzaDiLegge = DR_Dist(0).Item("Distanza_MaxValue")
            _TipoInterferenza = 1    'Possibile INTERFERENZA 
            'DistanzaMinima = Distanza_MaxValue
            '_Motivazione_Cod = 4     'Non Bietola - Informazioni Incomplete
            _Motivazione_Des = "Poiche' le informazioni fornite sono incomplete, " & "<br>" &
                "si e' preso in considerazione il caso peggiore ... " & "<br>" &
                "E' necessario analizzare piu' approfonditamente " & "<br>" &
                "questa possibile interferenza !!!"

            Return
        End If



        'Implementazione provvisoria per il RAVANELLO...
        'Bugfix 10 marzo 2022
        If _ID_Specie_A = 5 Then

            If _ID_Genotipo_A <> _ID_Genotipo_B Then

                _DistanzaDiLegge = 2000
                _Motivazione_Des = "Varietà con valori OP/HY diversi"
            Else

                If _ID_Sottospecie_A <> _ID_Sottospecie_B Then

                    If _ID_Genotipo_A = 1 Then

                        _DistanzaDiLegge = 2000
                        _Motivazione_Des = "Varietà HY - SOTTOSPECIE diverse"
                    Else

                        _DistanzaDiLegge = 1500
                        _Motivazione_Des = "Varietà OP - SOTTOSPECIE diverse"
                    End If
                Else

                    If _ID_Gruppo_A <> _ID_Gruppo_B Then

                        If _ID_Genotipo_A = 1 Then

                            _DistanzaDiLegge = 1500
                            _Motivazione_Des = "Varietà HY - stessa SOTTOSPECIE - GRUPPO diverso"

                        Else
                            _DistanzaDiLegge = 1000
                            _Motivazione_Des = "Varietà OP - stessa SOTTOSPECIE - GRUPPO diverso"
                        End If
                    Else

                        If _ID_Genotipo_A = 1 Then

                            _DistanzaDiLegge = 1000
                            _Motivazione_Des = "Varietà HY - stessa SOTTOSPECIE - stesso GRUPPO"
                        Else

                            _DistanzaDiLegge = 600
                            _Motivazione_Des = "Varietà OP - stessa SOTTOSPECIE - stesso GRUPPO"
                        End If
                    End If
                End If
            End If


            If DistanzaEffettiva < (_DistanzaDiLegge * Moltiplicatore) Then

                _TipoInterferenza = 2
            End If

            Return
        End If



        If _ID_Genotipo_A <> _ID_Genotipo_B Then

            _DistanzaDiLegge = DR_Dist(0).Item("Diversi_Genotipi")
            '_Motivazione_Cod = 5     'Non Bietola - Informazioni Complete - OP/HY diversi
            _Motivazione_Des = "Varietà con valori OP/HY diversi"

        Else
            If _ID_Gruppo_A <> _ID_Gruppo_B Then

                If _ID_Genotipo_A = 1 Then

                    'GABRIELE
                    '_DistanzaDiLegge = DR_Dist(0).Item("Div_Varieta_in_Gruppo_HY")
                    _DistanzaDiLegge = DR_Dist(0).Item("Div_Gruppi_in_SottoSpecie_HY")
                    '_Motivazione_Cod = 6     'Non Bietola - Informazioni Complete - Varieta' HY - GRUPPO diverso
                    _Motivazione_Des = "Varietà HY - GRUPPO diverso"

                Else
                    ' GABRIELE
                    '_DistanzaDiLegge = DR_Dist(0).Item("Div_Varieta_in_Gruppo_OP")
                    _DistanzaDiLegge = DR_Dist(0).Item("Div_Gruppi_in_SottoSpecie_OP")
                    '_Motivazione_Cod = 7     'Non Bietola - Informazioni Complete - Varieta' OP - GRUPPO diverso
                    _Motivazione_Des = "Varietà OP - GRUPPO diverso"

                End If
            Else
                If _ID_Sottospecie_A <> _ID_Sottospecie_B Then

                    If _ID_Genotipo_A = 1 Then

                        _DistanzaDiLegge = DR_Dist(0).Item("Div_SottoSpecie_in_Specie_HY")
                        '_Motivazione_Cod = 10     'Non Bietola - Informazioni Complete - Varieta' HY - SOTTOSPECIE Diversa
                        _Motivazione_Des = "Varietà HY - SOTTOSPECIE diverse"

                    Else

                        _DistanzaDiLegge = DR_Dist(0).Item("Div_SottoSpecie_in_Specie_OP")
                        '_Motivazione_Cod = 11     'Non Bietola - Informazioni Complete - Varieta' OP - SOTTOSPECIE Diversa
                        _Motivazione_Des = "Varietà OP - SOTTOSPECIE diverse"

                    End If
                Else
                    If _ID_Genotipo_A = 1 Then

                        'GABRIELE
                        '_DistanzaDiLegge = DR_Dist(0).Item("Div_Gruppi_in_SottoSpecie_HY")
                        _DistanzaDiLegge = DR_Dist(0).Item("Div_Varieta_in_Gruppo_HY")
                        '_Motivazione_Cod = 8     'Non Bietola - Informazioni Complete - Varieta' HY - GRUPPI Diversi - Stessa SOTTOSPECIE
                        _Motivazione_Des = "Varietà HY - stesso GRUPPO - stessa SOTTOSPECIE"

                    Else

                        'GABRIELE
                        '_DistanzaDiLegge = DR_Dist(0).Item("Div_Gruppi_in_SottoSpecie_OP")
                        _DistanzaDiLegge = DR_Dist(0).Item("Div_Varieta_in_Gruppo_OP")
                        '_Motivazione_Cod = 9     'Non Bietola - Informazioni Complete - Varieta' OP - GRUPPI Diversi - Stessa SOTTOSPECIE
                        _Motivazione_Des = "Varietà OP - stesso GRUPPO - stessa SOTTOSPECIE"

                    End If
                End If
            End If
        End If

        If DistanzaEffettiva < (_DistanzaDiLegge * Moltiplicatore) Then
            _TipoInterferenza = 2
        End If

    End Sub


    Private Shared Function Riga_Distanze(specie As String,
                                          sottospecie_op As Double,
                                          sottospecie_hy As Double,
                                          gruppi_op As Double,
                                          gruppi_hy As Double,
                                          varieta_op As Double,
                                          varieta_hy As Double,
                                          op_hy As Double) As JObject

        Dim jrow As New JObject(
            New JProperty("specie", specie),
            New JProperty("sottospecie_diverse",
                          New JObject(New JProperty("op", sottospecie_op),
                                      New JProperty("hy", sottospecie_hy)
                                      )
                                      ),
            New JProperty("gruppi_diversi",
                          New JObject(New JProperty("op", gruppi_op),
                                      New JProperty("hy", gruppi_hy)
                                      )
                                      ),
            New JProperty("stesso_gruppo",
                          New JObject(New JProperty("op", varieta_op),
                                      New JProperty("hy", varieta_hy)
                                      )
                                      ),
            New JProperty("diversi_genotipi", op_hy)
            )

        Return jrow
    End Function


    Public Shared Function TabelleDistanze(objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As JArray

        Dim jTable As New JArray

        Dim objVarie As New AgronicaCoreSementieriDAL.Varie_R
        Dim DT_Distanze = objVarie.Leggi_Mappatura_Specie_Distanze("", objParametri_server)

        For Each row In DT_Distanze.Rows

            Dim id_specie As Integer = CInt(row("ID_Specie"))

            If id_specie <> 1 AndAlso id_specie <> 21 AndAlso id_specie <> 23 AndAlso id_specie <> 24 Then

                jTable.Add(Riga_Distanze(row("Specie").ToString,
                                         Convert.ToDouble(row("Div_Sottospecie_in_Specie_OP"), CultureInfo.InvariantCulture),
                                         Convert.ToDouble(row("Div_Sottospecie_in_Specie_HY"), CultureInfo.InvariantCulture),
                                         Convert.ToDouble(row("Div_Gruppi_in_Sottospecie_OP"), CultureInfo.InvariantCulture),
                                         Convert.ToDouble(row("Div_Gruppi_in_Sottospecie_HY"), CultureInfo.InvariantCulture),
                                         Convert.ToDouble(row("Div_Varieta_in_Gruppo_OP"), CultureInfo.InvariantCulture),
                                         Convert.ToDouble(row("Div_Varieta_in_Gruppo_HY"), CultureInfo.InvariantCulture),
                                         Convert.ToDouble(row("Diversi_Genotipi"), CultureInfo.InvariantCulture)))

            Else

                If id_specie = 23 Then

                    jTable.Add(Riga_Distanze(row("Specie").ToString & " - Categoria 'CERTIFICATA'",
                                             500, 2000,
                                             500, 2000,
                                             500, 2000,
                                             2000))

                    jTable.Add(Riga_Distanze(row("Specie").ToString & " - Categoria 'BASE'",
                                             1000, 3000,
                                             1000, 3000,
                                             1000, 3000,
                                             3000))

                    jTable.Add(Riga_Distanze(row("Specie").ToString & " - Categoria 'PRE-BASE'",
                                             2000, 5000,
                                             2000, 5000,
                                             2000, 5000,
                                             5000))

                Else

                    If id_specie = 24 Then

                        jTable.Add(Riga_Distanze(row("Specie").ToString & " - Tipologia Orientale",
                                                 1000, 1000,
                                                 1000, 1000,
                                                 1000, 1000,
                                                 1000))

                        jTable.Add(Riga_Distanze(row("Specie").ToString & " - Tipologia Europeo",
                                                 1000, 2000,
                                                 1000, 2000,
                                                 1000, 2000,
                                                 2000))
                    End If
                End If
            End If

        Next

        Dim DT_Distanze_AUX = objVarie.Leggi_Mappatura_Specie_Distanze_Bietola("", objParametri_server)

        jTable.Add(New JObject(
                   New JProperty("specie", "Bietola (altre)"),
                   New JProperty("sottospecie_diverse", 1500),
                   New JProperty("gruppi_diversi", 600),
                   New JProperty("stesso_gruppo", 600),
                   New JProperty("diversi_genotipi", Nothing)
                   ))

        jTable.Add(New JObject(
                   New JProperty("specie", "Bietola per semente 'CERTIFICATA'"),
                   New JProperty("sottospecie_diverse", 1500),
                   New JProperty("gruppi_diversi", 1200),
                   New JProperty("stesso_gruppo", 1200),
                   New JProperty("diversi_genotipi", Nothing)
                   ))

        jTable.Add(New JObject(
                   New JProperty("specie", "Bietola per semente 'BASE e PRE-BASE' Plurigerme"),
                   New JProperty("sottospecie_diverse", 1500),
                   New JProperty("gruppi_diversi", 1500),
                   New JProperty("stesso_gruppo", 1200),
                   New JProperty("diversi_genotipi", Nothing)
                   ))

        jTable.Add(New JObject(
                   New JProperty("specie", "Bietola per semente 'BASE e PRE-BASE' Altre combinazioni"),
                   New JProperty("sottospecie_diverse", 1500),
                   New JProperty("gruppi_diversi", 1500),
                   New JProperty("stesso_gruppo", 1500),
                   New JProperty("diversi_genotipi", Nothing)
                   ))

        Return jTable
    End Function
End Class
