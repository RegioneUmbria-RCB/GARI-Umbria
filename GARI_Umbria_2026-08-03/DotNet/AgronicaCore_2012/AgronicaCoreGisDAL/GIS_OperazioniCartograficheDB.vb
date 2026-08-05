Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class GIS_OperazioniCartograficheDB
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function AggregaAppezzamentiContiguiComeParticelle( _
                          ByVal Tipo_entita_cod As Integer, _
                          ByVal ID_LayerParticelle As Integer, _
                            ByVal FattoreConversione As Double, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As Boolean


        Const NomeRoutine As String = "GIS_OperazioniCartograficheDB.AggregaAppezzamentiContiguiComeParticelle()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" create table #tmpEntita(pivasuperuser nvarchar(1000), piva nvarchar(1000), sa_cod int, appezza int, campo_cod int, id_imp int ,entita_cod int ,tipoEntita_cod int, prov nvarchar(1000), com nvarchar(1000), sezione nvarchar(1000), foglio int, numero int, subalterno nvarchar(1000), Programmazione_Entita_Cod int, ID_Agenda int, Geo geography ); " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" insert #tmpEntita(pivasuperuser, piva,sa_cod, Appezza, campo_cod, id_imp, entita_cod,TipoEntita_Cod, prov,com,sezione,foglio,numero,subalterno, Programmazione_entita_cod, id_Agenda, geo) " & vbCrLf)
            Stb.Append(" select  " & vbCrLf)
            Stb.Append("    e.pivasuperuser " & vbCrLf)
            Stb.Append("    , e.piva " & vbCrLf)
            Stb.Append("    , 0 as sa_cod   " & vbCrLf)
            Stb.Append("    , 0 as appezza " & vbCrLf)
            Stb.Append("    , 0 as campo_cod " & vbCrLf)
            Stb.Append("    , 0 as id_imp " & vbCrLf)
            'Lavez - 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                Stb.Append("    , dense_rank() over (order by prov,com,sezione,foglio,numero,subalterno) + (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_Gis_entita') as entita_cod  " & vbCrLf)
            Else
                Stb.Append("    , (select max(entita_cod) from gis_entita) + dense_rank() over (order by prov,com,sezione,foglio,numero,subalterno) as entita_cod " & vbCrLf)
            End If

            Stb.Append("    , " & Tipo_entita_cod & " as TipoEntita_Cod " & vbCrLf)
            Stb.Append("    ,prov " & vbCrLf)
            Stb.Append("    ,com " & vbCrLf)
            Stb.Append("    ,sezione " & vbCrLf)
            Stb.Append("    ,foglio " & vbCrLf)
            Stb.Append("    ,numero " & vbCrLf)
            Stb.Append("    ,subalterno " & vbCrLf)
            Stb.Append("    ,0 " & vbCrLf)
            Stb.Append("    ,0 " & vbCrLf)
            Stb.Append("    ,dbo.GeographyUnionAggregate(ef.Poligono_GeoEntity).STBuffer(" & Agro_SQL_SaveNum(FattoreConversione) & ") " & vbCrLf)
            Stb.Append(" from gis_entita e  " & vbCrLf)
            Stb.Append("    inner join  gis_elementigrafici ef on e.Entita_Cod = ef.Entita_Cod   " & vbCrLf)
            Stb.Append(" group by e.pivasuperuser, e.Piva, prov,com,sezione,foglio,numero,subalterno " & vbCrLf)
            Stb.Append(" having dbo.GeographyUnionAggregate(ef.Poligono_GeoEntity).STBuffer(" & Agro_SQL_SaveNum(FattoreConversione) & ") is not null " & vbCrLf)
            Stb.Append(" and not dbo.GeographyUnionAggregate(ef.Poligono_GeoEntity).STBuffer(" & Agro_SQL_SaveNum(FattoreConversione) & ").STAsText() like '%MULTIPOLYGON%' " & vbCrLf)
            Stb.Append(" and not dbo.GeographyUnionAggregate(ef.Poligono_GeoEntity).STBuffer(" & Agro_SQL_SaveNum(FattoreConversione) & ").STAsText() like '%GEOMETRYCOLLECTION%'; " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append(" insert gis_entita (pivasuperuser, piva,sa_cod, Appezza, campo_cod, id_imp, entita_cod,TipoEntita_Cod, prov,com,sezione,foglio,numero,subalterno, Programmazione_entita_cod, id_agenda) " & vbCrLf)
            Stb.Append(" select pivasuperuser, piva,sa_cod, Appezza, campo_cod, id_imp, entita_cod,TipoEntita_Cod, prov,com,sezione,foglio,numero,subalterno, Programmazione_entita_cod, ID_Agenda" & vbCrLf)
            Stb.Append(" from #tmpEntita; " & vbCrLf)

            Stb.Append("  " & vbCrLf)



            Stb.Append(" --seconda query, inserisce il dato cartografico " & vbCrLf)
            Stb.Append(" insert gis_ElementiGrafici(pivasuperuser,ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, Poligono_GeoEntity, LayerElementiGrafici_Cod) " & vbCrLf)
            Stb.Append(" select e.pivasuperuser " & vbCrLf)
            'Lavez 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                Stb.Append("    , dense_rank() over (order by prov,com,sezione,foglio,numero,subalterno) + (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_GIS_ElementiGrafici') as elemento_grafico_cod  " & vbCrLf)
            Else
                Stb.Append("    , (select max(ElementoGrafico_Cod) from GIS_ElementiGrafici ) + dense_rank() over (order by prov,com,sezione,foglio,numero,subalterno) as elemento_grafico_cod " & vbCrLf)
            End If

            Stb.Append("    , '' " & vbCrLf)
            Stb.Append("    , e.entita_cod " & vbCrLf)
            Stb.Append("    , geo " & vbCrLf)
            Stb.Append("    , " & ID_LayerParticelle & " as LayerElementiGrafici_Cod " & vbCrLf)
            Stb.Append(" from #tmpEntita e; " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" --ultime query, elimina le particelle catastali dagli oggetti di tipo impianto " & vbCrLf)
            Stb.Append(" update e " & vbCrLf)
            Stb.Append(" set prov='',com='',sezione='',foglio=-1,numero=-1,subalterno='',inviato=0 " & vbCrLf)
            Stb.Append(" from gis_entita e  " & vbCrLf)
            Stb.Append("    inner join gis_elementigrafici g on e.entita_cod = g.entita_cod " & vbCrLf)
            Stb.Append(" where e.inviato = -1; " & vbCrLf)

            'Stb.Append("insert ParticelleCatastali(PART_COD, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, PARTITA_CATASTALE, ETTARI, ARE, CENTIARE, TitoloPossesso, QUALITA_COD, CLASSE, REDDITO_DOMINICALE, REDDITO_AGRARIO, inviato, datainvio, username_creazione, username_modifica) " & vbCrLf)
            'Stb.Append(" select  " & vbCrLf)
            'Stb.Append("    (select MAX(PART_COD) from ParticelleCatastali ) + DENSE_RANK() over (order by prov, com, sezione, foglio, numero, subalterno) as part_cod " & vbCrLf)
            'Stb.Append("    , prov " & vbCrLf)
            'Stb.Append("    , COM  " & vbCrLf)
            'Stb.Append("    , SEZIONE " & vbCrLf)
            'Stb.Append("    , FOGLIO  " & vbCrLf)
            'Stb.Append("    , NUMERO  " & vbCrLf)
            'Stb.Append("    , case when SUBALTERNO = '' then '0' else SUBALTERNO end as SUBALTERNO " & vbCrLf)
            'Stb.Append("    , '' " & vbCrLf)
            'Stb.Append("    , 0 as Ettari " & vbCrLf)
            'Stb.Append("    , 0 as Are " & vbCrLf)
            'Stb.Append("    , 0 as centiare " & vbCrLf)
            'Stb.Append("    , 1 as titoloPossesso " & vbCrLf)
            'Stb.Append("    , 0 as Qualita_cod " & vbCrLf)
            'Stb.Append("    , '' as classe " & vbCrLf)
            'Stb.Append("    , 0 as reddito_dominicale " & vbCrLf)
            'Stb.Append("    , 0 as reddito_agrario " & vbCrLf)
            'Stb.Append("    , 0 as inviato " & vbCrLf)
            'Stb.Append("    , null as datainvio " & vbCrLf)
            'Stb.Append("    , '" & objParametri.PivaSuperUser & "' as username_creazione " & vbCrLf)
            'Stb.Append("    , '" & objParametri.PivaSuperUser & "' as username_modifica " & vbCrLf)
            'Stb.Append("  " & vbCrLf)
            'Stb.Append(" from #tmpEntita e " & vbCrLf)
            'Stb.Append(" where not exists (  " & vbCrLf)
            'Stb.Append("    select * " & vbCrLf)
            'Stb.Append("    from ParticelleCatastali pp " & vbCrLf)
            'Stb.Append("    where " & vbCrLf)
            'Stb.Append("    pp.PROV = e.prov collate latin1_general_ci_as  " & vbCrLf)
            'Stb.Append("    and pp.COM = e.com collate latin1_general_ci_as " & vbCrLf)
            'Stb.Append("    and pp.SEZIONE= e.SEZIONE collate latin1_general_ci_as " & vbCrLf)
            'Stb.Append("    and pp.foglio = e.foglio  " & vbCrLf)
            'Stb.Append("    and pp.numero = e.numero " & vbCrLf)
            'Stb.Append("    and pp.SUBALTERNO = case when e.subalterno = '' then '0' else e.subalterno end collate latin1_general_ci_as " & vbCrLf)
            'Stb.Append(" )" & vbCrLf)
            'Stb.Append(" group by  " & vbCrLf)
            'Stb.Append("   prov " & vbCrLf)
            'Stb.Append(" , COM  " & vbCrLf)
            'Stb.Append(" , SEZIONE " & vbCrLf)
            'Stb.Append(" , FOGLIO  " & vbCrLf)
            'Stb.Append(" , NUMERO  " & vbCrLf)
            'Stb.Append(" , SUBALTERNO; " & vbCrLf)
            'Stb.Append("update Sequenza_Tabelle set Ultimo_Valore = (select MAX(part_cod) from particellecatastali) where nome_tabella = 'particellecatastali'; " & vbCrLf)

            'Lavez - 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                Stb.AppendLine(" Declare @newGisEnt as int = coalesce( (select MAX(entita_cod) from gis_entita ), (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_Gis_entita')) ")
                Stb.AppendLine(" Alter sequence Sequence_Gis_entita ")
                Stb.AppendLine(" restart with @newGisEnt ")
                Stb.AppendLine(" Declare @newGisEle as int = coalesce( (select MAX(elementoGrafico_cod) from gis_elementigrafici ), (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_Gis_elementiGrafici')) ")
                Stb.AppendLine(" Alter sequence Sequence_Gis_elementiGrafici ")
                Stb.AppendLine(" restart with @newGisEle ")
            Else
                Stb.Append("update Sequenza_Tabelle set Ultimo_Valore = (select MAX(entita_cod) from gis_entita)   where nome_tabella = 'gis_entita;' " & vbCrLf)
                Stb.Append("update Sequenza_Tabelle set Ultimo_Valore = (select MAX(elementoGrafico_cod) from gis_elementigrafici) where nome_tabella = 'gis_elementiGrafici';" & vbCrLf)
            End If



            Stb.Append(" drop table #tmpEntita; " & vbCrLf)


            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        Return xRisp





    End Function





    ''' <summary>
    ''' Calcola le interferenze dato un poligono in formato GML
    ''' </summary>
    ''' <param name="NewImp_PivaReferente">P.iva della ditta sementiera a cui appartiene l utente</param>
    ''' <param name="NewImp_PoligonoGML">poligono in formato GML</param>
    ''' <param name="NewImp_Veg_Cod">Specie</param>
    ''' <param name="NewImp_Grva_Cod">gruppo varietale</param>
    ''' <param name="NewImp_FlagHybrid"></param>
    ''' <param name="NewImp_Validita_Inizio"></param>
    ''' <param name="NewImp_Validita_Fine"></param>
    ''' <param name="Flag_InterferenzeStessoReferente">Se ture, considera anche le distanze all'interno della stessa impresa</param>
    ''' <param name="MoltiplicatoreDistanze">fattore di moltiplicazione delle distanze (default=1, nessun adattamento)</param>
    ''' <param name="objParametri"></param>
    ''' <returns>Nothing se l'istanza geografica è sbagliata; datatable vouto se non ci sono interferenze</returns>
    ''' <remarks></remarks>
    Public Function ElencoImpiantiInterferenzaSementi(
                         ByVal NewImp_PivaReferente As String,
                         ByVal NewImp_PoligonoGML As String,
                         ByVal NewImp_Veg_Cod As Integer,
                         ByVal NewImp_Grva_Cod As Integer,
                         ByVal NewImp_FlagHybrid As Integer,
                         ByVal NewImp_Validita_Inizio As Date,
                         ByVal NewImp_Validita_Fine As Date,
                         ByVal Flag_InterferenzeStessoReferente As Boolean,
                         ByVal MoltiplicatoreDistanze As Double,
                         ByVal NumeroRecordDaSelezionare As Integer,
                         ByVal Flag_FinalitaCod As Boolean,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "GIS_OperazioniCartograficheDB.ElencoImpiantiInterferenzaSementi()"
        Dim MessaggioErrore As String = ""
        Dim DT_Impianti As DataTable = Nothing

        Dim TestPoligono As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim tResult As Boolean = TestPoligono.TestaPoligono(NewImp_PoligonoGML)
        If Not tResult Then
            '2 tentativo con il poligono invertito
            NewImp_PoligonoGML = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono(NewImp_PoligonoGML, True)
            tResult = TestPoligono.TestaPoligono(NewImp_PoligonoGML, objParametri)

        End If

        If tResult Then

            Dim stb As New System.Text.StringBuilder

            If MoltiplicatoreDistanze = 0 Then
                MoltiplicatoreDistanze = 1
            End If

            Try

                Dim nomeDB_Utenti = objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

                stb.Length = 0

                stb.AppendLine("SET NOCOUNT ON ")
                stb.AppendLine()
                stb.AppendLine("DECLARE @NEW_ID_SPECIE AS INTEGER = 0, @NEW_ID_SOTTOSPECIE AS INTEGER = 0, @NEW_ID_GRUPPO AS INTEGER = 0, @NEW_ID_GENOTIPO AS INTEGER = 0, @GRFI_COD AS INTEGER = 0 ")
                stb.AppendLine("SELECT TOP 1 ")
                stb.AppendLine("@NEW_ID_SPECIE      =   COALESCE(ID_SPECIE, 0), ")
                stb.AppendLine("@NEW_ID_SOTTOSPECIE =   COALESCE(ID_SOTTOSPECIE, 0), ")
                stb.AppendLine("@NEW_ID_GRUPPO		=	COALESCE(ID_GRUPPO, 0), ")
                stb.AppendLine("@NEW_ID_GENOTIPO	=	COALESCE(ID_GENOTIPO, 0), ")
                stb.AppendLine("@GRFI_COD           =	COALESCE(GRFI_COD, 0) ")
                stb.AppendLine("FROM Mappatura_Specie ")
                stb.AppendLine("WHERE Veg_Cod = " & NewImp_Veg_Cod)
                stb.AppendLine("AND Grva_Cod = " & Math.Abs(NewImp_Grva_Cod))
                stb.AppendLine("AND (" & NewImp_Veg_Cod & " IN (6, 5000021, 70, 69) OR Hybrid = " & NewImp_FlagHybrid & ") ")
                stb.AppendLine()
                stb.AppendLine()
                stb.AppendLine("DECLARE @ELENCO_SEMENTIERI AS TABLE(COD INTEGER) ")
                stb.AppendLine("INSERT INTO @ELENCO_SEMENTIERI ")
                stb.AppendLine("SELECT DISTINCT Veg_Cod FROM Mappatura_Specie ")
                stb.AppendLine("WHERE ID_Specie = (SELECT DISTINCT ID_Specie FROM Mappatura_Specie WHERE Veg_Cod = " & Agro_SQL_SaveNum(NewImp_Veg_Cod) & ")")
                stb.AppendLine()
                stb.AppendLine()
                stb.AppendLine("DECLARE @DISTANZA_CONFRONTO AS FLOAT ")
                stb.AppendLine("SET @DISTANZA_CONFRONTO = (SELECT MAX(LimiteSuperiore) FROM ")
                stb.AppendLine("    (SELECT COALESCE(MAX(Distanza_MaxValue), 0) AS LimiteSuperiore FROM Mappatura_Specie_Distanze ")
                stb.AppendLine("    WHERE ID_Specie IN (SELECT DISTINCT ID_Specie FROM Mappatura_Specie WHERE Veg_Cod IN (SELECT * FROM @ELENCO_SEMENTIERI)) ")
                stb.AppendLine("    UNION ")
                stb.AppendLine("    SELECT COALESCE(MAX(Div_Varieta_in_Gruppo), 0) AS LimiteSuperiore FROM Mappatura_Specie_Distanze_Bietola ")
                stb.AppendLine("    WHERE ID_Specie IN (SELECT DISTINCT ID_Specie FROM Mappatura_Specie WHERE Veg_Cod IN (SELECT * FROM @ELENCO_SEMENTIERI)) ")
                stb.AppendLine("    ) B ")
                stb.AppendLine(") * " & Agro_SQL_SaveNum(Math.Abs(MoltiplicatoreDistanze)))
                stb.AppendLine()
                stb.AppendLine()
                stb.AppendLine("DECLARE @x xml; ")
                stb.AppendLine("SET @x = '" & Agro_SQL_SaveText(NewImp_PoligonoGML) & "' ")
                stb.AppendLine("DECLARE @g geography; ")
                stb.AppendLine("SET @g = geography::GeomFromGml(@x, 4326) ")
                stb.AppendLine()
                stb.AppendLine()
                Dim sselect As String = "SELECT "
                If NumeroRecordDaSelezionare > 0 Then
                    sselect &= "TOP " & NumeroRecordDaSelezionare.ToString
                End If
                stb.AppendLine(sselect)
                stb.AppendLine("    @NEW_ID_SPECIE AS New_ID_Specie, ")
                stb.AppendLine("    @NEW_ID_SOTTOSPECIE AS New_ID_Sottospecie, ")
                stb.AppendLine("    @NEW_ID_GRUPPO AS New_ID_Gruppo, ")
                stb.AppendLine("    @NEW_ID_GENOTIPO AS New_ID_Genotipo, ")
                stb.AppendLine("    -1 AS TipoInterferenza, ")
                stb.AppendLine("    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx' AS Giustificazione, ")
                stb.AppendLine("    CASE WHEN @NEW_ID_SPECIE IN (1, 21) THEN 1 ELSE 0 END AS Flag_Bietola, ")
                stb.AppendLine("    @g.STDistance(Poligono_GeoEntity) AS DistanzaEffettiva, ")
                stb.AppendLine("    " & Agro_SQL_SaveNum(MoltiplicatoreDistanze) & " AS MoltiplicatoreDistanze, ")
                stb.AppendLine("    0.0 AS DistanzaMinima, ")
                stb.AppendLine("    0.0 AS DistanzaLeggeBase, ")
                stb.AppendLine("    0.0 AS DistanzaLeggeCertificata, ")
                stb.AppendLine("    GIS_Entita.Entita_Cod, ")
                'stb.AppendLine("    COALESCE(Appezzamento.APP_NOME, programmazione_testata.programmazione_des) AS Descrizione, ")
                'stb.AppendLine("    COALESCE(Reg_Impianti.CODICE_FISCALE_TECNICO, programmazione_entita.CODICE_FISCALE_TECNICO) AS ReferentePiva, ")
                stb.AppendLine("    Appezzamento.APP_NOME AS Descrizione, ")
                stb.AppendLine("    Reg_Impianti.CODICE_FISCALE_TECNICO AS ReferentePiva, ")
                stb.AppendLine("    lconf.Gruppi_Utente_des AS Referente, ")
                stb.AppendLine("    imp.PIVA AS AziendaPiva, ")
                stb.AppendLine("    imp.rag_soc AS Azienda, ")
                stb.AppendLine("    Reg_Impianti.SA_COD, ")
                stb.AppendLine("    Reg_Impianti.APPEZZA, ")
                stb.AppendLine("    Reg_Impianti.ID_REG, ")
                stb.AppendLine("    Cultivar.Veg_Cod, ")
                stb.AppendLine("    SpecieVegetali.Veg_Des, ")
                stb.AppendLine("    Reg_Impianti.CUL_COD, ")
                stb.AppendLine("    Cultivar.Cul_Des, ")
                'stb.AppendLine("    COALESCE(Reg_Impianti.Validita_Inizio, programmazione_entita.validita_inizio) AS Validita_Inizio, ")
                'stb.AppendLine("    COALESCE(Reg_Impianti.Validita_Fine, programmazione_entita.validita_fine) AS Validita_Fine, ")
                'stb.AppendLine("    COALESCE(Reg_Impianti.GRVA_Cod_VEG,  programmazione_entita.grva_cod) AS GRVA_Cod_VEG, ")
                stb.AppendLine("    Reg_Impianti.Validita_Inizio AS Validita_Inizio, ")
                stb.AppendLine("    Reg_Impianti.Validita_Fine AS Validita_Fine, ")
                stb.AppendLine("    Reg_Impianti.GRVA_Cod_VEG AS GRVA_Cod_VEG, ")
                'stb.AppendLine("    CASE WHEN COALESCE(Reg_Impianti.GRVA_Cod_VEG, programmazione_entita.grva_cod) < 0 THEN 1 ELSE 0 END AS FlagHybrid, ")
                stb.AppendLine("    CASE WHEN Reg_Impianti.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END AS FlagHybrid, ")
                stb.AppendLine("    GruppoVarietale.Grva_Des AS Tipologia, ")
                'stb.AppendLine("    CASE WHEN COALESCE(Reg_Impianti.GRVA_Cod_VEG, programmazione_entita.grva_cod) < 0 THEN 'Ibrido' ELSE 'Puro' END AS Genetica, ")
                stb.AppendLine("    CASE WHEN Reg_Impianti.GRVA_Cod_VEG < 0 THEN 'Ibrido' ELSE 'Puro' END AS Genetica, ")
                stb.AppendLine("    Reg_Impianti.Sup_Imp, ")
                stb.AppendLine("    GIS_ElementiGrafici.Poligono_GeoEntity.AsGml() AS PoligonoGML, ")
                stb.AppendLine("    ii.ind_des, ")
                stb.AppendLine("    ii.frz_des, ")
                stb.AppendLine("    i.cap, ")
                stb.AppendLine("    i.LOCALITA AS citta, ")
                stb.AppendLine("    i.COMUNI_PROV AS provincia, ")
                stb.AppendLine("    Mappatura_Specie.ID_Specie, ")
                stb.AppendLine("    Mappatura_Specie.ID_Sottospecie, ")
                stb.AppendLine("    Mappatura_Specie.ID_Gruppo, ")
                stb.AppendLine("    Mappatura_Specie.ID_Genotipo ")
                stb.AppendLine()
                stb.AppendLine("FROM            Imprese             Imp ")
                stb.AppendLine("INNER JOIN      Centri_Aziendali    sa      ON SA.Piva = Imp.piva ")
                stb.AppendLine("INNER JOIN      CentriXIndirizzi    ci      ON ci.piva = sa.piva ")
                stb.AppendLine("                                            AND ci.sa_cod = sa.sa_cod ")
                stb.AppendLine("INNER JOIN      Indirizzi           ii      ON ii.cod_indirizzo = ci.cod_indirizzo ")
                stb.AppendLine("INNER JOIN      ISTAT               i       ON ii.pro_cod_istat = i.prov ")
                stb.AppendLine("                                            AND ii.com_cod_istat = i.com ")
                stb.AppendLine("LEFT JOIN       Reg_Impianti                ON Reg_Impianti.PIVA = Imp.Piva ")
                stb.AppendLine("                                            AND Reg_Impianti.SA_COD = SA.SA_COD ")
                stb.AppendLine("LEFT JOIN       Appezzamento                ON Reg_Impianti.PIVA = Appezzamento.PIVA ")
                stb.AppendLine("                                            AND Reg_Impianti.SA_COD = Appezzamento.SA_COD ")
                stb.AppendLine("                                            AND Reg_Impianti.APPEZZA = Appezzamento.APPEZZA ")
                'stb.AppendLine("LEFT JOIN       Programmazione_Entita       ON Programmazione_Entita.piva = imp.piva ")
                'stb.AppendLine("                                            AND Programmazione_Entita.sa_cod = sa.sa_cod ")
                'stb.AppendLine("LEFT JOIN       Programmazione_Testata      ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_cod ")
                stb.AppendLine("INNER JOIN      GIS_Entita                  ON ( Reg_Impianti.PIVA = GIS_Entita.Piva ")
                stb.AppendLine("                                                AND Reg_Impianti.SA_COD = GIS_Entita.Sa_Cod ")
                stb.AppendLine("                                                AND Reg_Impianti.APPEZZA = GIS_Entita.Appezza ")
                stb.AppendLine("                                                AND Reg_Impianti.ID_REG = GIS_Entita.Id_Imp ) ")
                'stb.AppendLine("                                            OR Programmazione_Entita.Programmazione_entita_cod = gis_entita.Programmazione_entita_cod ")
                stb.AppendLine("INNER JOIN   " & nomeDB_Utenti & ".dbo.Gruppi_Utente lconf ON Reg_Impianti.CODICE_FISCALE_TECNICO = lconf.Gruppi_Utente_Identificativo ")
                'stb.AppendLine("                                            OR Programmazione_Entita.Codice_Fiscale_Tecnico = Layers_Configurazione.CodiceCollegato ")
                stb.AppendLine("INNER JOIN      Cultivar                    ON reg_impianti.cul_cod = cultivar.cul_cod ")
                'stb.AppendLine("                                            OR Programmazione_Entita.cul_Cod = cultivar.cul_cod ")
                stb.AppendLine("INNER JOIN      SpecieVegetali              ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
                stb.AppendLine("INNER JOIN      GIS_ElementiGrafici         ON GIS_Entita.PivaSuperUser = GIS_ElementiGrafici.PivaSuperUser ")
                stb.AppendLine("                                            AND GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod ")
                'stb.AppendLine("LEFT OUTER JOIN GruppoVarietale             ON ABS(coalesce(Reg_Impianti.GRVA_Cod_VEG,  programmazione_entita.grva_cod)) = GruppoVarietale.Grva_Cod ")
                stb.AppendLine("LEFT OUTER JOIN GruppoVarietale             ON ABS(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.Grva_Cod ")
                stb.AppendLine("LEFT JOIN       Mappatura_Specie            ON Mappatura_Specie.Veg_Cod = SpecieVegetali.Veg_Cod ")
                stb.AppendLine("                                            AND ABS(Reg_Impianti.GRVA_Cod_VEG) = Mappatura_Specie.Grva_Cod ")
                stb.AppendLine("                                            AND Mappatura_Specie.Hybrid = (CASE WHEN Cultivar.veg_cod IN (6, 5000021, 70, 69) THEN -1 ELSE (CASE WHEN reg_impianti.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END)")
                stb.AppendLine("                                            ")
                stb.AppendLine()
                stb.AppendLine("WHERE 1 = 1 ")
                stb.AppendLine("AND (Reg_Impianti.Validita_Inizio is not null ")
                stb.AppendLine("    AND (Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(NewImp_Validita_Fine) & " ) ")
                stb.AppendLine("    AND (Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(NewImp_Validita_Inizio) & " ) ")
                'stb.AppendLine("    OR (Reg_Impianti.Validita_Inizio is null ")
                'stb.AppendLine("    AND (Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(NewImp_Validita_Fine) & " ) ")
                'stb.AppendLine("    AND (Programmazione_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(NewImp_Validita_Inizio) & " ) ")
                'stb.AppendLine("    ) ")
                stb.AppendLine(") ")
                stb.AppendLine("AND (SpecieVegetali.Veg_Cod IN (SELECT * FROM @ELENCO_SEMENTIERI)) ")
                stb.AppendLine("AND (@g.STDistance(Poligono_GeoEntity) <= @DISTANZA_CONFRONTO) ")
                If Flag_InterferenzeStessoReferente = False Then
                    'stb.AppendLine("AND (COALESCE(Reg_Impianti.CODICE_FISCALE_TECNICO, programmazione_entita.CODICE_FISCALE_TECNICO)  <> '" & Agro_SQL_SaveText(NewImp_PivaReferente) & "') ")
                    stb.AppendLine("AND Reg_Impianti.CODICE_FISCALE_TECNICO <> '" & Agro_SQL_SaveText(NewImp_PivaReferente) & "' ")
                End If
                If Flag_FinalitaCod Then
                    stb.AppendLine("AND Reg_Impianti.GRFI_COD = @GRFI_COD ")
                End If
                stb.AppendLine()
                stb.AppendLine("ORDER BY @g.STDistance(Poligono_GeoEntity) ")

                '--------------------------------------------------------------------------
                DT_Impianti = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                'Elaboro il datatable
                If MoltiplicatoreDistanze > 0 Then
                    'Rendo modificabile il datatable degli impianti
                    For Each col In DT_Impianti.Columns
                        col.ReadOnly = False
                    Next

                    Dim objInterferenze As New AgronicaCoreSementieriDAL.VerificaInterferenza("", objParametri)

                    'Ciclo sugli impianti presenti nel datatable
                    Dim i As Integer
                    For i = 0 To DT_Impianti.Rows.Count - 1

                        objInterferenze.Set_Specie_A(DT_Impianti.Rows(i).Item("New_ID_Specie"))
                        objInterferenze.Set_Sottospecie_A(DT_Impianti.Rows(i).Item("New_ID_Sottospecie"))
                        objInterferenze.Set_Gruppo_A(DT_Impianti.Rows(i).Item("New_ID_Gruppo"))
                        objInterferenze.Set_Genotipo_A(DT_Impianti.Rows(i).Item("New_ID_Genotipo"))

                        objInterferenze.Set_Specie_B(DT_Impianti.Rows(i).Item("ID_Specie"))
                        objInterferenze.Set_Sottospecie_B(DT_Impianti.Rows(i).Item("ID_Sottospecie"))
                        objInterferenze.Set_Gruppo_B(DT_Impianti.Rows(i).Item("ID_Gruppo"))
                        objInterferenze.Set_Genotipo_B(DT_Impianti.Rows(i).Item("ID_Genotipo"))

                        objInterferenze.Verifica(DT_Impianti.Rows(i).Item("DistanzaEffettiva"), MoltiplicatoreDistanze)

                        If objInterferenze.TipoInterferenza = 0 Then
                            'Se non sussiste interferenza, cancello la riga del DT_Impianti  
                            DT_Impianti.Rows(i).Delete()

                        Else

                            DT_Impianti.Rows(i).Item("TipoInterferenza") = objInterferenze.TipoInterferenza
                            DT_Impianti.Rows(i).Item("Giustificazione") = "INTERFERENZA " & objInterferenze.Motivazione_Des
                            DT_Impianti.Rows(i).Item("DistanzaMinima") = objInterferenze.DistanzaDiLegge
                            DT_Impianti.Rows(i).Item("DistanzaLeggeBase") = objInterferenze.DistanzaDiLegge_BietolaBase
                            DT_Impianti.Rows(i).Item("DistanzaLeggeCertificata") = objInterferenze.DistanzaDiLegge_BietolaCert

                        End If

                    Next

                    'Conferma modifiche nel DT  (acceptchanges)
                    DT_Impianti.AcceptChanges()

                End If

            Catch ex As Exception

                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                DT_Impianti = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            End Try

        End If

        Return DT_Impianti
    End Function



    Public Function OttieniAreaDaGml(gml As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Double

        Const NomeRoutine As String = "GIS_OperazioniCartograficheDB.OttieniAreaDaGml()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim val As Double = 0
        Try


            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" DECLARE @wkt nvarchar(max) ")
            Stb.AppendLine(" SET @wkt = " + Agro_SQL_SaveText_NULL(gml) + " ")
            Stb.AppendLine(" DECLARE @geog geography ")
            Stb.AppendLine(" SET @geog = geography::GeomFromGml(@wkt , 4326) ")
            Stb.AppendLine(" SELECT @geog.STArea() ")


            Dim dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

            If dt.Rows.Count > 0 Then
                val = CDbl(dt.Rows(0)(0))
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        Return val

    End Function

    Public Function OttieniAreaDaWKT(wkt As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Double

        Const NomeRoutine As String = "GIS_OperazioniCartograficheDB.OttieniAreaDaGml()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim val As Double = 0
        Try


            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" DECLARE @wkt nvarchar(max) ")
            Stb.AppendLine(" SET @wkt = " + Agro_SQL_SaveText_NULL(wkt) + " ")
            Stb.AppendLine(" DECLARE @geog geography ")
            Stb.AppendLine(" SET @geog = geography::STGeomFromText(@wkt , 4326) ")
            Stb.AppendLine(" SELECT @geog.STArea() ")


            Dim dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

            If dt.Rows.Count > 0 Then
                val = CDbl(dt.Rows(0)(0))
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        Return val

    End Function

    Public Function STBufferFromWKTString(wkt As String, metri As Integer, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Const NomeRoutine As String = "GIS_OperazioniCartograficheDB.STBufferFromWKTString()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim val As String = ""
        Try


            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine("Declare @Metri int ")
            Stb.AppendLine(" Set @Metri = " & Agro_SQL_SaveNum(metri))
            Stb.AppendLine(" Declare @pText varchar(max) ")
            Stb.AppendLine(" Set @pText = '" & Agro_SQL_SaveText(wkt) & "' ")
            Stb.AppendLine(" Declare @g geography ")
            Stb.AppendLine(" Set @g = geography::STGeomFromText(@pText, 4326) ")
            Stb.AppendLine(" Select @g.STBuffer(@metri).STAsText() as STBuffer")



            Dim dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

            If dt.Rows.Count > 0 Then
                val = dt.Rows(0)(0).ToString
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        Return val

    End Function
End Class
