Imports System.Configuration
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions

Public Class ParticelleCatastali_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################
    Public Function LeggiConfrontoGIS(
                                    ByVal Entita_cod As Integer,
                                    ByVal TipoEntita_cod As enum_GIS2012_TipoEntita,
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.LeggiConfrontoGIS()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable



        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    Stb.Length = 0

                    Dim NomeCampoArea As String = "AREA"

                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        NomeCampoArea = "SUPERFICIE"
                    End If

                    Stb.AppendLine("declare @a geography")
                    Stb.AppendLine("declare @Conversione int")
                    Stb.AppendLine("set @Conversione = 10000")
                    Stb.AppendLine("")
                    Stb.AppendLine("declare @entita_Cod int")
                    Stb.AppendLine($"set @entita_Cod = {Entita_cod}")
                    Stb.AppendLine("")
                    Stb.AppendLine("select @a = Poligono_GeoEntity")
                    Stb.AppendLine("from GIS_ElementiGrafici")
                    Stb.AppendLine("where Entita_Cod = @Entita_Cod;")

                    Stb.AppendLine("")
                    Stb.AppendLine("WITH")
                    Stb.AppendLine("#ixpNotValid AS (")
                    Stb.AppendLine("	SELECT ")
                    Stb.AppendLine("		PROV")
                    Stb.AppendLine("		, COM")
                    Stb.AppendLine("		, SEZIONE")
                    Stb.AppendLine("		, Foglio")
                    Stb.AppendLine("		, NUMERO")
                    Stb.AppendLine("		, SUBALTERNO")
                    Stb.AppendLine("		, MAX(Validita_Fine) AS ValiditaFineParticella")
                    Stb.AppendLine("		, MIN(Validita_Inizio) as ValiditaInizioParticella")
                    Stb.AppendLine("	FROM ImpreseXParticelle ixp")
                    Stb.AppendLine("	GROUP BY PROV")
                    Stb.AppendLine("		, COM")
                    Stb.AppendLine("		, SEZIONE")
                    Stb.AppendLine("		, Foglio")
                    Stb.AppendLine("		, NUMERO")
                    Stb.AppendLine("		, SUBALTERNO")
                    Stb.AppendLine($"    HAVING MAX(Validita_Fine) < {Agro_SQL_SaveDate(Date.Today)}")
                    Stb.AppendLine("),")
                    Stb.AppendLine("#ixpValid AS (")
                    Stb.AppendLine("	SELECT ")
                    Stb.AppendLine("		PROV")
                    Stb.AppendLine("		, COM")
                    Stb.AppendLine("		, SEZIONE")
                    Stb.AppendLine("		, Foglio")
                    Stb.AppendLine("		, NUMERO")
                    Stb.AppendLine("		, SUBALTERNO")
                    Stb.AppendLine("		, MAX(Validita_Fine) AS ValiditaFineParticella")
                    Stb.AppendLine("		, MIN(Validita_Inizio) as ValiditaInizioParticella")
                    Stb.AppendLine("	FROM ImpreseXParticelle ixp")
                    Stb.AppendLine("	GROUP BY PROV")
                    Stb.AppendLine("		, COM")
                    Stb.AppendLine("		, SEZIONE")
                    Stb.AppendLine("		, Foglio")
                    Stb.AppendLine("		, NUMERO")
                    Stb.AppendLine("		, SUBALTERNO")
                    Stb.AppendLine($"    HAVING MIN(Validita_Inizio) <= {Agro_SQL_SaveDate(Date.Today)}")
                    Stb.AppendLine($"         AND MAX(Validita_Fine) >= {Agro_SQL_SaveDate(Date.Today)}")
                    Stb.AppendLine(")")

                    Stb.AppendLine("")
                    Stb.AppendLine("select")
                    Stb.AppendLine("    cast(ii.piva as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(sa.sa_cod as varchar(100)) + '|' +")

                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("    '-' + cast(appLetto.Programmazione_Entita_cod as varchar(100)) + '|' +")
                    Else

                        Stb.AppendLine("    cast(appLetto.appezza as varchar(100)) + '|' +")
                    End If

                    Stb.AppendLine("    cast(Entita.PROV as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(Entita.COM as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(Entita.SEZIONE as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(Entita.Foglio as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(Entita.NUMERO as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(Entita.SUBALTERNO as varchar(100))")
                    Stb.AppendLine("    as Chiave")

                    Stb.AppendLine("    , ii.rag_soc")
                    Stb.AppendLine("    , sa.sa_nome")

                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("    , '' as Campo_Des")
                    Else
                        Stb.AppendLine("    , coalesce(cp.Campo_Des,'') as Campo_Des")
                    End If

                    Stb.AppendLine("    , ist.COMUNI_PROV + ' (' + Entita.prov + ') - ' +")
                    Stb.AppendLine("        ist.LOCALITA + ' (' + Entita.COM + ') - ' +")
                    Stb.AppendLine("        cast(Entita.SEZIONE as varchar(100)) + ' - ' +")
                    Stb.AppendLine("        cast(Entita.Foglio as varchar(100)) + ' - ' +")
                    Stb.AppendLine("        cast(Entita.NUMERO as varchar(100)) + ' - ' +")
                    Stb.AppendLine("        cast(Entita.SUBALTERNO as varchar(100))")
                    Stb.AppendLine("    as catasto")
                    Stb.AppendLine("")
                    Stb.AppendLine("    , right('000' + cast (p.ETTARI as varchar(100)), 2) + '.' +")
                    Stb.AppendLine("        right('000' + cast (p.Are as varchar(100)), 2)  + '.' +")
                    Stb.AppendLine("        right('000' + cast (p.CentiAre as varchar(100)), 2)")
                    Stb.AppendLine("    as p_Ettari_Are_Centiare")


                    ' VAnni: 27/3/2017: calcolo dell'intersezione fra poligono e particelle
                    Stb.AppendLine("    , ")
                    qry_HA_Are_Centiare("@a.STIntersection(g.Poligono_GeoEntity).STArea()/@Conversione", "inter_Ettari_Are_Centiare", Stb)



                    ' VAnni: 27/3/2017: calcolo della percentuale di intersezione
                    Stb.AppendLine($"    , case when (appXp.{NomeCampoArea} is null OR appXp.{NomeCampoArea} = 0) then 0 else -1*(100 - round( ( (")

                    Stb.AppendLine("       @a.STIntersection(g.Poligono_GeoEntity).STArea()/@Conversione")

                    Stb.AppendLine("    )")
                    Stb.AppendLine($"    / appXp.{NomeCampoArea}) * 100 , 2)) end as percentualeScarto")



                    ' VAnni: 27/3/2017: ricavo l'intersezione precedente in termini di ha, are, centiare (partendo da intersezione in ettari)
                    Stb.AppendLine("    , ")
                    qry_HA_Are_Centiare("coalesce(appXp." & NomeCampoArea & ", 0)", "vecchioInter_Ettari_Are_Centiare", Stb)



                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("    , appLetto.Entita_DES + ' (' + coalesce(veg.veg_des + ' - ' + cul.cul_Des, '') + ')' as App_nome")
                        Stb.AppendLine("    , case when appXp.Programmazione_Entita_cod is null then 1 else 2 end  as operazione_DB")
                        Stb.AppendLine("    , 'Pianificato' as TipoDiImpianto")

                    Else
                        Stb.AppendLine("    , appLetto.APP_NOME + ' (' + coalesce(veg.veg_des + ' - ' + cul.cul_Des, '') + ')' as App_nome")
                        Stb.AppendLine("    , case when appXp.APPEZZA is null then 1 else 2 end  as operazione_DB")
                        Stb.AppendLine("    , 'Reale' as TipoDiImpianto")

                    End If

                    Stb.AppendLine($"    , {Entita_cod} as Entita_Cod")


                    ' VAnni: 27/3/2017: ricavo l'area del poligono (impianto oppure pianificato) in termini di ha, are, centiare
                    Stb.AppendLine("    , ")
                    qry_HA_Are_Centiare("@a.STArea()/@Conversione", "impianto_Ettari_Are_Centiare", Stb)

                    Stb.AppendLine("    , Entita.PROV")
                    Stb.AppendLine("    , Entita.COM")
                    Stb.AppendLine("    , Entita.SEZIONE")
                    Stb.AppendLine("    , Entita.Foglio")
                    Stb.AppendLine("    , Entita.NUMERO")
                    Stb.AppendLine("    , Entita.SUBALTERNO")

                    Stb.AppendLine("")
                    Stb.AppendLine("from gis_entita Entita")
                    Stb.AppendLine("    inner join GIS_ElementiGrafici g on Entita.Entita_Cod = g.Entita_cod")
                    Stb.AppendLine("")


                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then

                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join (")
                        Stb.AppendLine("        select piva, sa_cod, programmazione_cod, programmazione_entita_cod")
                        Stb.AppendLine("        from gis_entita")
                        Stb.AppendLine("        where Entita_Cod = @entita_Cod")
                        Stb.AppendLine("    ) gAppezza on 1 = 1")

                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join Programmazione_entita appLetto on appLetto.piva = gAppezza.piva")
                        Stb.AppendLine("        and appLetto.Programmazione_Entita_Cod = gAppezza.Programmazione_Entita_Cod")

                        Stb.AppendLine("")
                        Stb.AppendLine("    left join (")
                        Stb.AppendLine("        select impianti2.*")
                        Stb.AppendLine("        from (")
                        Stb.AppendLine("            select piva, sa_cod, programmazione_Entita_Cod, max(validita_fine) as data_fine")
                        Stb.AppendLine("            from Programmazione_Entita")
                        Stb.AppendLine("            group by piva, sa_cod, programmazione_Entita_Cod")
                        Stb.AppendLine("        ) impianti1")
                        Stb.AppendLine("")
                        Stb.AppendLine("        inner join Programmazione_Entita impianti2 on impianti2.piva = impianti1.piva")
                        Stb.AppendLine("            and impianti2.programmazione_Entita_cod = impianti1.programmazione_entita_cod")
                        Stb.AppendLine("            and impianti2.validita_Fine = impianti1.data_fine")
                        Stb.AppendLine("")
                        Stb.AppendLine("    ) imp on appLetto.piva = imp.piva")
                        Stb.AppendLine("        and appLetto.sa_cod = imp.sa_cod")
                        Stb.AppendLine("        and appLetto.Programmazione_Entita_Cod = Imp.Programmazione_Entita_Cod")

                    Else

                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join ( ")
                        Stb.AppendLine("        select piva, sa_cod, appezza")
                        Stb.AppendLine("        from gis_entita")
                        Stb.AppendLine("        where Entita_Cod = @entita_Cod")
                        Stb.AppendLine("    ) gAppezza on 1=1 ")

                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join Appezzamento appLetto on appLetto.piva = gAppezza.piva")
                        Stb.AppendLine("        and appLetto.SA_COD = gAppezza.Sa_Cod  ")
                        Stb.AppendLine("        and appLetto.APPEZZA = gAppezza.Appezza ")

                        Stb.AppendLine("")
                        Stb.AppendLine("    LEFT join campi cp on appLetto.piva = cp.piva")
                        Stb.AppendLine("        and appLetto.SA_COD = cp.Sa_Cod")
                        Stb.AppendLine("        and appLetto.Campo_Cod = cp.Campo_Cod")

                        Stb.AppendLine("")
                        Stb.AppendLine("    left join (")
                        Stb.AppendLine("        select impianti2.*")
                        Stb.AppendLine("        from (")
                        Stb.AppendLine("            select piva, sa_cod, appezza, max(validita_fine)  as data_fine ")
                        Stb.AppendLine("            from Reg_impianti   ")
                        Stb.AppendLine("            group by piva, sa_cod, appezza ")
                        Stb.AppendLine("        ) impianti1")
                        Stb.AppendLine("    ")
                        Stb.AppendLine("            inner join reg_impianti impianti2 on impianti2.piva = impianti1.piva")
                        Stb.AppendLine("                and impianti2.sa_cod = impianti1.sa_cod ")
                        Stb.AppendLine("                and impianti2.APPEZZA = impianti1.Appezza ")
                        Stb.AppendLine("                and impianti2.validita_Fine = impianti1.data_fine")
                        Stb.AppendLine("    ) imp on appLetto.piva = imp.piva")
                        Stb.AppendLine("        and appLetto.sa_cod = imp.sa_cod")
                        Stb.AppendLine("        and appLetto.APPEZZA = Imp.Appezza")

                    End If

                    Stb.AppendLine("    inner join Imprese ii on appLetto.piva = ii.piva")
                    Stb.AppendLine("")
                    Stb.AppendLine("    inner join Centri_Aziendali sa on sa.piva = appLetto.PIVA")
                    Stb.AppendLine("        and sa.Sa_Cod = appLetto.SA_COD")
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join Cultivar cul on imp.cul_cod = cul.cul_cod")
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join specievegetali veg on veg.veg_cod = cul.veg_cod")
                    Stb.AppendLine("")
                    Stb.AppendLine("    inner join istat ist on ist.prov = entita.prov")
                    Stb.AppendLine("        and ist.COM = Entita.COM  ")
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join ParticelleCatastali p on p.prov = Entita.prov")
                    Stb.AppendLine("        and p.com = Entita.COM")
                    Stb.AppendLine("        and p.SEZIONE = Entita.SEZIONE")
                    Stb.AppendLine("        and p.Foglio = Entita.Foglio")
                    Stb.AppendLine("        and p.NUMERO = Entita.NUMERO")
                    Stb.AppendLine("        and p.SUBALTERNO = Entita.SUBALTERNO")

                    'this two joins (ixp, ixpV) allow to filter on the validity of Particelle that have at least on record on the ImpreseXParticelle
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join #ixpValid ixpV on p.PROV = ixpV.PROV")
                    Stb.AppendLine("        and p.com = ixpV.COM")
                    Stb.AppendLine("        and p.SEZIONE = ixpV.SEZIONE")
                    Stb.AppendLine("        and p.Foglio = ixpV.Foglio")
                    Stb.AppendLine("        and p.NUMERO = ixpV.NUMERO")
                    Stb.AppendLine("        and p.SUBALTERNO = ixpV.SUBALTERNO")

                    Stb.AppendLine("")
                    Stb.AppendLine("    left join ImpreseXParticelle ixpAll on p.PROV = ixpAll.PROV")
                    Stb.AppendLine("        and p.com = ixpAll.COM")
                    Stb.AppendLine("        and p.SEZIONE = ixpAll.SEZIONE")
                    Stb.AppendLine("        and p.Foglio = ixpAll.Foglio")
                    Stb.AppendLine("        and p.NUMERO = ixpAll.NUMERO")
                    Stb.AppendLine("        and p.SUBALTERNO = ixpAll.SUBALTERNO")

                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then

                        Stb.AppendLine("    left join Programmazione_Particelle appXp on appXp.prov = Entita.prov")
                        Stb.AppendLine("        and appXp.com = Entita.COM    ")
                        Stb.AppendLine("        and appXp.SEZIONE = Entita.SEZIONE    ")
                        Stb.AppendLine("        and appXp.Foglio = Entita.Foglio   ")
                        Stb.AppendLine("        and appXp.NUMERO = Entita.NUMERO    ")
                        Stb.AppendLine("        and appXp.SUBALTERNO = Entita.SUBALTERNO  ")
                        Stb.AppendLine("        and appXp.Programmazione_Entita_cod = appLetto.Programmazione_entita_cod ")
                        Stb.AppendLine("")
                        Stb.AppendLine("    left join Programmazione_Entita App on appXp.Programmazione_Entita_cod = app.Programmazione_Entita_Cod")
                        Stb.AppendLine("")

                    Else

                        Stb.AppendLine("    left join AppezzamentiXParticelle appXp on appXp.prov = Entita.prov")
                        Stb.AppendLine("        and appXp.com = Entita.COM")
                        Stb.AppendLine("        and appXp.SEZIONE = Entita.SEZIONE")
                        Stb.AppendLine("        and appXp.Foglio = Entita.Foglio")
                        Stb.AppendLine("        and appXp.NUMERO = Entita.NUMERO")
                        Stb.AppendLine("        and appXp.SUBALTERNO = Entita.SUBALTERNO")
                        Stb.AppendLine("        and appXp.piva = appLetto.piva")
                        Stb.AppendLine("        and appXp.SA_COD = appLetto.Sa_Cod")
                        Stb.AppendLine("        and appXp.APPEZZA = appLetto.Appezza")

                        Stb.AppendLine("    left join Appezzamento App on appXp.PIVA = app.PIVA")
                        Stb.AppendLine("        and appXp.SA_COD = app.SA_COD  ")
                        Stb.AppendLine("        and appXp.APPEZZA = app.appezza ")
                        Stb.AppendLine("")

                    End If

                    Stb.AppendLine("where @a.STIntersects(Poligono_GeoEntity) = 1")
                    Stb.AppendLine("    and g.Entita_cod <> @entita_Cod")
                    Stb.AppendLine("    and g.LayerElementiGrafici_Cod = 3")
                    Stb.AppendLine("    and @a.STIntersection(g.Poligono_GeoEntity).STArea() > 0")

                    ' filter on Particella validity (if at least one Possesso is stored)
                    Stb.AppendLine("    and ((ixpV.PROV is null and ixpAll.prov is null) or ixpV.prov is not null)") ' takes only valid Particella (Possesso IS NOT expired)
                    Stb.AppendLine("")

                    Stb.AppendLine("union")

                    Stb.AppendLine("")
                    Stb.AppendLine("select")
                    Stb.AppendLine("    cast(gAppezza.piva  as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(gAppezza.sa_cod   as varchar(100)) + '|' +")

                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("    '-' + cast(gAppezza.Programmazione_entita_cod as varchar(100)) + '|' +")
                    Else
                        Stb.AppendLine("    cast(gAppezza.appezza  as varchar(100)) + '|' +")
                    End If


                    Stb.AppendLine("    cast(appXp.PROV as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(appXp.COM as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(appXp.SEZIONE as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(appXp.Foglio as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(appXp.NUMERO as varchar(100)) + '|' +")
                    Stb.AppendLine("    cast(appXp.SUBALTERNO as varchar(100))")
                    Stb.AppendLine("    as Chiave")
                    Stb.AppendLine("")
                    Stb.AppendLine("    , ii.rag_soc")
                    Stb.AppendLine("    , sa.sa_nome")


                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("    , '' as Campo_Des")
                    Else
                        Stb.AppendLine("    , coalesce(cp.Campo_Des,'') as Campo_Des")
                    End If

                    Stb.AppendLine("    , ist.COMUNI_PROV + ' (' + appXp.prov + ') - ' +")
                    Stb.AppendLine("        ist.LOCALITA + ' (' + appXp.COM + ') - ' +")
                    Stb.AppendLine("        cast(appXp.SEZIONE as varchar(100)) + ' - ' +")
                    Stb.AppendLine("        cast(appXp.FOGLIO as varchar(100)) + ' - ' +")
                    Stb.AppendLine("        cast(appXp.NUMERO as varchar(100)) + ' - ' +")
                    Stb.AppendLine("        cast(appXp.SUBALTERNO as varchar(100)) + ' - '")
                    Stb.AppendLine("    as catasto")
                    Stb.AppendLine("")
                    Stb.AppendLine("    , right('000' + cast (p.ETTARI as varchar(100)), 2) + '.' +")
                    Stb.AppendLine("       right('000' + cast (p.Are as varchar(100)), 2)  + '.' +")
                    Stb.AppendLine("       right('000' + cast (p.CentiAre as varchar(100)), 2)")
                    Stb.AppendLine("    as p_Ettari_Are_Centiare")
                    Stb.AppendLine("")
                    Stb.AppendLine("    , '0' as inter_Ettari_Are_Centiare")
                    Stb.AppendLine("    , '0' as percentualeScarto")
                    Stb.AppendLine($"    , cast(cast(coalesce(appXp.{NomeCampoArea}, 0) as int) as varchar(100)) + '.' +")
                    Stb.AppendLine("        substring(cast(abs(cast(coalesce(appXp." & NomeCampoArea & ", 0) as int) - coalesce(appXp." & NomeCampoArea & ", 0)) as varchar(100)), 3,2) + '.' +")
                    Stb.AppendLine("        substring(cast(abs(cast(coalesce(appXp." & NomeCampoArea & ", 0) as int) - coalesce(appXp." & NomeCampoArea & ", 0)) as varchar(100)), 5,2)")
                    Stb.AppendLine("    as vecchioInter_Ettari_Are_Centiare")
                    Stb.AppendLine("")

                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("    , appLetto.Entita_DES + ' (' + coalesce(veg.veg_des + ' - ' + cul.cul_Des, '') + ')' as App_nome")
                    Else
                        Stb.AppendLine("    , appLetto.APP_NOME + ' (' + coalesce(veg.veg_des + ' - ' + cul.cul_Des, '') + ')' as App_nome")
                    End If

                    Stb.AppendLine("    , 3 as operazione_DB")

                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("    , 'Pianificato' as TipoDiImpianto")
                    Else
                        Stb.AppendLine("    , 'Reale' as TipoDiImpianto")
                    End If

                    Stb.AppendLine($"    , {Entita_cod} as Entita_Cod")


                    ' VAnni: 27/3/2017: ricavo l'area del poligono (impianto oppure pianificato) in termini di ha, are, centiare
                    Stb.AppendLine("    , ")
                    qry_HA_Are_Centiare("@a.STArea()/@Conversione", "impianto_Ettari_Are_Centiare", Stb)

                    Stb.AppendLine("    , appXp.PROV")
                    Stb.AppendLine("    , appXp.COM")
                    Stb.AppendLine("    , appXp.SEZIONE")
                    Stb.AppendLine("    , appXp.Foglio")
                    Stb.AppendLine("    , appXp.NUMERO")
                    Stb.AppendLine("    , appXp.SUBALTERNO")


                    If TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI Then
                        Stb.AppendLine("from (")
                        Stb.AppendLine("   select piva, sa_cod, programmazione_cod, Programmazione_Entita_Cod")
                        Stb.AppendLine("   from gis_entita")
                        Stb.AppendLine("   where Entita_Cod = @entita_Cod")
                        Stb.AppendLine(") gAppezza")
                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join Programmazione_Particelle appXp on appXp.Programmazione_entita_cod = gAppezza.Programmazione_entita_cod")
                        Stb.AppendLine("")
                        Stb.AppendLine("    left join ParticelleCatastali p on p.prov = appXp.prov")
                        Stb.AppendLine("        and p.com = appXp.COM")
                        Stb.AppendLine("        and p.SEZIONE = appXp.SEZIONE")
                        Stb.AppendLine("        and p.Foglio = appXp.Foglio")
                        Stb.AppendLine("        and p.NUMERO = appXp.NUMERO")
                        Stb.AppendLine("        and p.SUBALTERNO = appXp.SUBALTERNO")

                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join Programmazione_Entita appLetto on appLetto.piva = gAppezza.piva")
                        Stb.AppendLine("        and appLetto.Programmazione_Entita_Cod = gAppezza.Programmazione_Entita_Cod")
                        Stb.AppendLine("")
                        Stb.AppendLine("    left join (")
                        Stb.AppendLine("        select impianti2.*")
                        Stb.AppendLine("        from (")
                        Stb.AppendLine("            select piva, sa_cod, programmazione_entita_cod, max(validita_fine) as data_fine")
                        Stb.AppendLine("            from Programmazione_Entita")
                        Stb.AppendLine("            group by piva, sa_cod, programmazione_entita_cod")
                        Stb.AppendLine("        ) impianti1")
                        Stb.AppendLine("")
                        Stb.AppendLine("            inner join Programmazione_Entita impianti2 on impianti2.piva = impianti1.piva")
                        Stb.AppendLine("                and impianti2.Programmazione_entita_cod = impianti1.Programmazione_entita_cod")
                        Stb.AppendLine("                and impianti2.validita_Fine = impianti1.data_fine")
                        Stb.AppendLine("")
                        Stb.AppendLine("    ) imp on appLetto.piva = imp.piva")
                        Stb.AppendLine("        and appLetto.Programmazione_Entita_Cod = Imp.Programmazione_Entita_Cod")
                    Else
                        Stb.AppendLine("from (")
                        Stb.AppendLine("    select piva, sa_cod, appezza")
                        Stb.AppendLine("    from gis_entita")
                        Stb.AppendLine("    where Entita_Cod = @entita_Cod")
                        Stb.AppendLine(") gAppezza")

                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join")
                        Stb.AppendLine("    AppezzamentiXParticelle appXp on appXp.piva = gAppezza.piva")
                        Stb.AppendLine("        and appXp.SA_COD = gAppezza.Sa_Cod")
                        Stb.AppendLine("        and appXp.APPEZZA = gAppezza.Appezza")
                        Stb.AppendLine("")
                        Stb.AppendLine("    left join ParticelleCatastali p on p.prov = appXp.prov")
                        Stb.AppendLine("        and p.com = appXp.COM")
                        Stb.AppendLine("        and p.SEZIONE = appXp.SEZIONE")
                        Stb.AppendLine("        and p.Foglio = appXp.Foglio")
                        Stb.AppendLine("        and p.NUMERO = appXp.NUMERO")
                        Stb.AppendLine("        and p.SUBALTERNO = appXp.SUBALTERNO")

                        Stb.AppendLine("")
                        Stb.AppendLine("    inner join Appezzamento appLetto on appLetto.piva = gAppezza.piva")
                        Stb.AppendLine("        and appLetto.SA_COD = gAppezza.Sa_Cod")
                        Stb.AppendLine("        and appLetto.APPEZZA = gAppezza.Appezza")
                        Stb.AppendLine("")
                        Stb.AppendLine("    LEFT join campi cp on appLetto.piva = cp.piva")
                        Stb.AppendLine("        and appLetto.SA_COD = cp.Sa_Cod")
                        Stb.AppendLine("        and appLetto.Campo_Cod = cp.Campo_Cod")

                        Stb.AppendLine("")
                        Stb.AppendLine("    left join (")
                        Stb.AppendLine("        select impianti2.*")
                        Stb.AppendLine("        from (")
                        Stb.AppendLine("            select piva, sa_cod, appezza, max(validita_fine) as data_fine")
                        Stb.AppendLine("            from Reg_impianti")
                        Stb.AppendLine("            group by piva, sa_cod, appezza")
                        Stb.AppendLine("        ) impianti1")
                        Stb.AppendLine("")
                        Stb.AppendLine("            inner join reg_impianti impianti2 on impianti2.piva = impianti1.piva")
                        Stb.AppendLine("                and impianti2.sa_cod = impianti1.sa_cod")
                        Stb.AppendLine("                and impianti2.APPEZZA = impianti1.Appezza")
                        Stb.AppendLine("                and impianti2.validita_Fine = impianti1.data_fine")
                        Stb.AppendLine("")
                        Stb.AppendLine("    ) imp on appLetto.piva = imp.piva")
                        Stb.AppendLine("        and appLetto.sa_cod = imp.sa_cod")
                        Stb.AppendLine("        and appLetto.APPEZZA = Imp.Appezza")
                        Stb.AppendLine("")

                    End If

                    'this two joins (ixpAll, ixpNV) allow to filter on the validity of Particelle that have at least on record on the ImpreseXParticelle
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join #ixpNotValid ixpNV on p.PROV = ixpNV.PROV")
                    Stb.AppendLine("        and p.com = ixpNV.COM")
                    Stb.AppendLine("        and p.SEZIONE = ixpNV.SEZIONE")
                    Stb.AppendLine("        and p.Foglio = ixpNV.Foglio")
                    Stb.AppendLine("        and p.NUMERO = ixpNV.NUMERO")
                    Stb.AppendLine("        and p.SUBALTERNO = ixpNV.SUBALTERNO")

                    Stb.AppendLine("")
                    Stb.AppendLine("    inner join Imprese ii on appLetto.piva = ii.piva")
                    Stb.AppendLine("")
                    Stb.AppendLine("    inner join Centri_Aziendali sa on sa.piva = appLetto.PIVA")
                    Stb.AppendLine("        and sa.Sa_Cod = appLetto.SA_COD")
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join Cultivar cul on imp.cul_cod = cul.cul_cod")
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join specievegetali veg on veg.veg_cod = cul.veg_cod")
                    Stb.AppendLine("              ")
                    Stb.AppendLine("    ")
                    Stb.AppendLine("    inner join istat ist on ist.prov = appXp.prov")
                    Stb.AppendLine("        and ist.COM = appXp.COM")
                    Stb.AppendLine("")
                    Stb.AppendLine("    left join (")
                    Stb.AppendLine("        select e.*")
                    Stb.AppendLine("        from gis_Entita e")
                    Stb.AppendLine("            inner join gis_elementiGrafici g on e.Entita_Cod = g.Entita_Cod")
                    Stb.AppendLine("")
                    Stb.AppendLine("        where @a.STIntersects(Poligono_GeoEntity) = 1")
                    Stb.AppendLine("            and g.Entita_cod <> @entita_Cod")
                    Stb.AppendLine("            and g.LayerElementiGrafici_Cod = 3")
                    Stb.AppendLine("            and @a.STIntersection(g.Poligono_GeoEntity).STArea() > 0")
                    Stb.AppendLine("    ) appXp1 on appXp1.prov = appXp.prov")
                    Stb.AppendLine("        and appXp1.com = appXp.COM")
                    Stb.AppendLine("        and appXp1.SEZIONE = appXp.SEZIONE")
                    Stb.AppendLine("        and appXp1.Foglio = appXp.Foglio")
                    Stb.AppendLine("        and appXp1.NUMERO = appXp.NUMERO")
                    Stb.AppendLine("        and appXp1.SUBALTERNO = appXp.SUBALTERNO")

                    Stb.AppendLine("")
                    Stb.AppendLine("where appXp1.PROV is null")

                    ' filter on Particella validity (if at least one Possesso is stored) and if it is linked to an Appezzamento
                    Stb.AppendLine("    OR (")
                    Stb.AppendLine("        ixpNV.prov is not null") ' takes no more valid Particella (Possesso is expired)
                    Stb.AppendLine("        AND EXISTS(")
                    Stb.AppendLine("    	    SELECT 1 FROM ParticelleCatastali pp") ' takes Particella linked to Appezzamento
                    Stb.AppendLine("    	    WHERE pp.prov = appXp.prov")
                    Stb.AppendLine("                and pp.com = appXp.COM")
                    Stb.AppendLine("                and pp.SEZIONE = appXp.SEZIONE")
                    Stb.AppendLine("                and pp.Foglio = appXp.Foglio")
                    Stb.AppendLine("                and pp.NUMERO = appXp.NUMERO")
                    Stb.AppendLine("                and pp.SUBALTERNO = appXp.SUBALTERNO")
                    Stb.AppendLine("    	)")
                    Stb.AppendLine("    )")

                    If xFiltroAggiuntivo <> "" Then
                        Stb.AppendLine("   AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        Stb.AppendLine("")
                        Stb.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

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

    Private Shared Sub qry_HA_Are_Centiare(ByVal CampoAreaTotaleDaDividere As String, ByVal AliasCampo As String, ByRef Stb As Text.StringBuilder)
        Stb.Append("    cast(cast( " & CampoAreaTotaleDaDividere & " as int) as varchar(100)) + '.' +")
        Stb.AppendLine("    substring(cast(format(abs(cast(" & CampoAreaTotaleDaDividere & " as int) - " & CampoAreaTotaleDaDividere & "), 'N8') as varchar(100)), 3,2)  + '.' +")
        Stb.AppendLine("    substring(cast(format(abs(cast(" & CampoAreaTotaleDaDividere & " as int) - " & CampoAreaTotaleDaDividere & "), 'N8') as varchar(100)), 5,2)")
        If AliasCampo <> "" Then
            Stb.Append($"    as {AliasCampo}")
        End If

        Stb.AppendLine("")
    End Sub

    '##############################################################################################
    Public Function LeggixChiave_conClassamento(
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Long,
                                ByVal NUMERO As Long,
                                ByVal SUBALTERNO As String,
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.LeggixChiave_conClassamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT ParticelleCatastali.PART_COD, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ParticelleCatastali.PARTITA_CATASTALE, ParticelleCatastali.TitoloPossesso, ")
                    StrSQL.AppendLine(" ParticelleCatastali.ETTARI, ParticelleCatastali.ARE, ParticelleCatastali.CENTIARE, ")
                    StrSQL.AppendLine(" ParticelleCatastali.Data_Creazione, ParticelleCatastali.Data_Modifica, ParticelleCatastali.Username_Creazione, ParticelleCatastali.Username_Modifica, ParticelleCatastali.Validita_Inizio, ParticelleCatastali.Validita_Fine, ")
                    StrSQL.AppendLine(" ISNULL(ParticelleCatastaliClassamento.QUALITA_COD,0) AS QUALITA_COD, ISNULL(ParticelleCatastaliClassamento.Porzione,'') AS Porzione, ISNULL(ParticelleCatastaliClassamento.CLASSE,'') AS CLASSE, ISNULL(ParticelleCatastaliClassamento.Sup_Classe,0) AS Sup_Classe, ISNULL(ParticelleCatastaliClassamento.REDDITO_DOMINICALE,0) AS REDDITO_DOMINICALE, ISNULL(ParticelleCatastaliClassamento.REDDITO_AGRARIO,0) AS REDDITO_AGRARIO, ISNULL(ParticelleCatastaliClassamento.Deduzione,'') AS Deduzione, ")
                    StrSQL.AppendLine(" ISNULL(QualitaCatasto.QUALITA_DES,'') AS QUALITA_DES ")
                    StrSQL.AppendLine(" FROM  QualitaCatasto INNER JOIN ")
                    StrSQL.AppendLine(" ParticelleCatastaliClassamento ON QualitaCatasto.QUALITA_COD = ParticelleCatastaliClassamento.QUALITA_COD INNER JOIN ")
                    StrSQL.AppendLine(" ParticelleCatastali ON ParticelleCatastaliClassamento.PROV = ParticelleCatastali.PROV AND ")
                    StrSQL.AppendLine(" ParticelleCatastaliClassamento.COM = ParticelleCatastali.COM AND ParticelleCatastaliClassamento.SEZIONE = ParticelleCatastali.SEZIONE AND  ")
                    StrSQL.AppendLine(" ParticelleCatastaliClassamento.FOGLIO = ParticelleCatastali.FOGLIO AND ParticelleCatastaliClassamento.NUMERO = ParticelleCatastali.NUMERO AND  ")
                    StrSQL.AppendLine(" ParticelleCatastaliClassamento.SUBALTERNO = ParticelleCatastali.SUBALTERNO  ")
                    StrSQL.AppendLine(" WHERE ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Sezione     " & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", "= '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "'", "IN ('0', '')"))
                    StrSQL.AppendLine(" AND   ParticelleCatastali.FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.SUBALTERNO     " & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", "= '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "'", "IN ('0', '')"))
                    'StrSQL.AppendLine(" AND   ParticelleCatastali.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ParticelleCatastali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ParticelleCatastali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
    Public Function CheckEsistenzaPossessi(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Long,
                                    ByVal NUMERO As Long,
                                    ByVal SUBALTERNO As String,
                                    ByVal flag_cancellazione As Boolean,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.CheckEsistenzaPossessi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim esito As Boolean = False
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  contrattiXimpreseXparticelle ")
            StrSQL.Append(" WHERE PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esito = False
            Throw New Exception(MessaggioErrore)
        End Try

        If DT.Rows.Count <> 0 Then
            esito = True
            If flag_cancellazione Then
                esito = False
                Throw New GiasException("Impossibile eliminare il dato in quanto sono presenti contratti di affitto")
            End If
        End If
        Return esito

    End Function

    '##############################################################################################
    Public Function LeggixChiave(ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Long,
                                ByVal NUMERO As Long,
                                ByVal SUBALTERNO As String,
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.LeggixChiave()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ParticelleCatastali ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
    Public Function LeggixAlberoAnagrafica(
        ByVal Piva As String,
        ByVal Sa_Cod As Integer,
        ByVal xSelezioneVariabile As enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri,
        Optional ByVal appezza As Integer = 0
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.LeggixAlberoAnagrafica()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0
                    stb.Append(" SELECT  ParticelleCatastali.PROV,  ")
                    stb.Append("         ParticelleCatastali.COM,  ")
                    stb.Append("         ParticelleCatastali.SEZIONE,  ")
                    stb.Append("         ParticelleCatastali.FOGLIO,  ")
                    stb.Append("         ParticelleCatastali.NUMERO,  ")
                    stb.Append("         ParticelleCatastali.SUBALTERNO,  ")
                    stb.Append("         ParticelleCatastali.Part_Cod,  ")
                    stb.Append("         ParticelleCatastali.ETTARI,  ")
                    stb.Append("         ParticelleCatastali.[ARE],  ")
                    stb.Append("         ParticelleCatastali.CENTIARE,  ")
                    stb.Append("         ParticelleCatastali.Validazione,  ")
                    stb.Append("         ImpreseXParticelle.Piva,  ")
                    stb.Append("         ImpreseXParticelle.Sa_Cod,  ")
                    stb.Append("         ImpreseXParticelle.TitoloPossesso,  ")
                    stb.Append("         ImpreseXParticelle.Validita_Inizio,  ")
                    stb.Append("         ImpreseXParticelle.Validita_Fine, ")
                    stb.Append("         ImpreseXParticelle.Validazione, ")
                    stb.Append("         ImpreseXParticelle.Sup_Condotta, ")

                    'GABRIELE
                    'stb.Append("         ISTAT_Comuni.Provincia AS ISTAT_PROVINCIA, ")
                    'stb.Append("         ISTAT_Comuni.Descrizione AS ISTAT_COMUNE, ")
                    'stb.Append("         ISTAT_Comuni.Cod_Belfiore AS ISTAT_COD_BELFIORE ")
                    stb.Append("         ISTAT.Comuni_Prov AS ISTAT_PROVINCIA, ")
                    stb.Append("         ISTAT.Localita AS ISTAT_COMUNE, ")
                    stb.Append("         ISTAT.CodiceCatastale AS ISTAT_COD_BELFIORE ")
                    stb.AppendLine()

                    stb.Append(" FROM    ParticelleCatastali INNER JOIN ")
                    stb.Append(" ImpreseXParticelle ON ParticelleCatastali.PROV = ImpreseXParticelle.PROV AND ParticelleCatastali.COM = ImpreseXParticelle.COM AND  ")
                    stb.Append(" ParticelleCatastali.SEZIONE = ImpreseXParticelle.SEZIONE AND ParticelleCatastali.FOGLIO = ImpreseXParticelle.FOGLIO AND  ")
                    stb.Append(" ParticelleCatastali.NUMERO = ImpreseXParticelle.NUMERO AND ParticelleCatastali.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ")

                    If appezza <> 0 Then
                        stb.AppendLine(" INNER JOIN  [dbo].[AppezzamentiXParticelle] app1")
                        stb.AppendLine(" on ")
                        stb.AppendLine("         app1.PROV = ParticelleCatastali.PROV")
                        stb.AppendLine("     and app1.COM =  ParticelleCatastali.COM")
                        stb.AppendLine("     and app1.SEZIONE =  ParticelleCatastali.SEZIONE")
                        stb.AppendLine("     and app1.FOGLIO =  ParticelleCatastali.FOGLIO")
                        stb.AppendLine("     and app1.NUMERO =  ParticelleCatastali.NUMERO")
                        stb.AppendLine("     and app1.SUBALTERNO =  ParticelleCatastali.SUBALTERNO")
                    End If

                    stb.AppendLine()
                    'stb.AppendLine("INNER JOIN ISTAT_Comuni on ISTAT_Comuni.Pro_Cod_Istat = ParticelleCatastali.PROV and ISTAT_Comuni.Com_Cod_Istat = ParticelleCatastali.COM")
                    stb.AppendLine("INNER JOIN ISTAT on ISTAT.PROV = ParticelleCatastali.PROV and ISTAT.COM = ParticelleCatastali.COM")
                    stb.AppendLine()

                    stb.Append(" WHERE     (ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    stb.Append(" AND (ImpreseXParticelle.Validita_Inizio <=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    stb.Append(" AND (ImpreseXParticelle.Validita_Fine   >=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")
                    stb.Append(" AND (ParticelleCatastali.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    stb.Append(" AND (ParticelleCatastali.Validita_Fine   >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                    If (Sa_Cod <> 0) Then
                        stb.Append(" AND (ImpreseXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If (appezza <> 0) Then
                        stb.Append(" AND (app1.piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                        stb.Append(" AND (app1.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                        stb.Append(" AND (app1.appezza = " & Agro_SQL_SaveNum(appezza) & ")  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   ParticelleCatastali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   ParticelleCatastali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.Append(" ORDER BY ParticelleCatastali.PROV, ParticelleCatastali.COM , ParticelleCatastali.SEZIONE , ParticelleCatastali.FOGLIO , ParticelleCatastali.NUMERO , ParticelleCatastali.SUBALTERNO  ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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
    Public Function Leggi(ByVal Part_Cod As Int32,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Int32,
                           ByVal NUMERO As Int32,
                           ByVal SUBALTERNO As String,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ParticelleCatastali ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Part_Cod <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali.PART_COD =  " & Agro_SQL_SaveNum(Part_Cod) & "")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Validita_inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ParticelleCatastali ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Part_Cod <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali.PART_COD =  " & Agro_SQL_SaveNum(Part_Cod) & "")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Validita_inizio ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
    Public Function Leggi_conClassamentoCodice(ByVal PIVA As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal PROV As String,
                                               ByVal COM As String,
                                               ByVal SEZIONE As String,
                                               ByVal FOGLIO As Integer,
                                               ByVal NUMERO As Integer,
                                               ByVal SUBALTERNO As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByVal dataInizioValidita As Date? = Nothing,
                                               Optional ByVal dataFineValidita As Date? = Nothing,
                                               Optional ByVal TitoloPossesso As Integer? = Nothing
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Leggi_conClassamentoCodice()"

        Dim messaggioErrore As String = ""
        Dim strColonneSql As New System.Text.StringBuilder
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim finestraTemporaleInizio As Date
        Dim finestraTemporaleFine As Date

        If Not IsNothing(dataInizioValidita) Then
            finestraTemporaleInizio = dataInizioValidita
        Else
            finestraTemporaleInizio = objParametri.FinestraTemporaleInizio
        End If

        If Not IsNothing(dataFineValidita) Then
            finestraTemporaleFine = dataFineValidita
        Else
            finestraTemporaleFine = objParametri.FinestraTemporaleFine
        End If

        Try

            'Elenco colonne estratte, usato anche in group by

            strColonneSql.Length = 0
            strColonneSql.AppendLine("   ParticelleCatastali.PROV ")
            strColonneSql.AppendLine(" , ParticelleCatastali.COM ")
            strColonneSql.AppendLine(" , ParticelleCatastali.SEZIONE ")
            strColonneSql.AppendLine(" , ParticelleCatastali.FOGLIO ")
            strColonneSql.AppendLine(" , ParticelleCatastali.NUMERO ")
            strColonneSql.AppendLine(" , ParticelleCatastali.SUBALTERNO ")
            strColonneSql.AppendLine(" , ParticelleCatastali.SUBALTERNO ")
            strColonneSql.AppendLine(" , ParticelleCatastali.ETTARI ")
            strColonneSql.AppendLine(" , ParticelleCatastali.ARE ")
            strColonneSql.AppendLine(" , ParticelleCatastali.CENTIARE ")
            strColonneSql.AppendLine(" , ISTAT.LOCALITA ")
            strColonneSql.AppendLine(" , ISTAT.COMUNI_PROV ")

            'Inizio comando SELECT

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.Append(strColonneSql.ToString)
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_DOMINICALE,0) as REDDITO_DOMINICALE_CLASS ")
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_AGRARIO,0) as REDDITO_AGRARIO_CLASS ")
            '---NB: MIN/MAX ignorano i valori NULLI
            strSql.AppendLine(" , MIN(ISNULL(ImpresexParticelle_Codici.Val_Cod,'')) Cod_Particella  ")
            strSql.AppendLine(" , MAX(ISNULL(ImpresexParticelle_Codici.Val_Cod,'')) Cod_Particella_Max  ")
            strSql.AppendLine(" , MIN(CONCAT(ImpresexParticelle_Codici.Val_Cod,'|',ImpresexParticelle.ID)) Cod_Particella_Id ")
            '---
            strSql.AppendLine(" FROM  ParticelleCatastali ")

            ' Join ISTAT

            strSql.AppendLine(" INNER JOIN ISTAT ON ISTAT.PROV = ParticelleCatastali.PROV AND ISTAT.COM = ParticelleCatastali.COM ")

            ' Join ParticelleCatastaliClassamento

            strSql.AppendLine(" LEFT JOIN ParticelleCatastaliClassamento ON ")
            strSql.AppendLine("           ParticelleCatastaliClassamento.PROV = ParticelleCatastali.PROV ")
            strSql.AppendLine("       AND ParticelleCatastaliClassamento.COM = ParticelleCatastali.COM ")
            strSql.AppendLine("       AND ParticelleCatastaliClassamento.SEZIONE = ParticelleCatastali.SEZIONE ")
            strSql.AppendLine("       AND ParticelleCatastaliClassamento.FOGLIO = ParticelleCatastali.FOGLIO ")
            strSql.AppendLine("       AND ParticelleCatastaliClassamento.NUMERO = ParticelleCatastali.NUMERO ")
            strSql.AppendLine("       AND ParticelleCatastaliClassamento.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
            strSql.AppendLine("       AND ParticelleCatastaliClassamento.QUALITA_COD = ParticelleCatastali.QUALITA_COD ")

            ' Join ImpresexParticelle

            strSql.AppendLine(" LEFT JOIN ImpresexParticelle ON ")
            strSql.AppendLine("           ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
            strSql.AppendLine("       AND ImpresexParticelle.COM = ParticelleCatastali.COM ")
            strSql.AppendLine("       AND ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
            strSql.AppendLine("       AND ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
            strSql.AppendLine("       AND ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
            strSql.AppendLine("       AND ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            strSql.AppendLine("       AND ImpresexParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(finestraTemporaleFine) & " ")
            strSql.AppendLine("       AND ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(finestraTemporaleInizio) & " ")

            If PIVA <> "" Then
                strSql.AppendLine("       AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine("       AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Not IsNothing(TitoloPossesso) Then
                strSql.AppendLine("       AND ImpresexParticelle.TitoloPossesso = " & Agro_SQL_SaveNum(TitoloPossesso))
            End If

            ' Join ImpresexParticelle_Codici

            strSql.AppendLine(" LEFT JOIN ImpresexParticelle_Codici ON ")
            strSql.AppendLine("           ImpresexParticelle_Codici.ID = ImpresexParticelle.ID ")
            strSql.AppendLine("       AND ImpresexParticelle_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.CodiceParticella.ToString("D")))

            ' Where

            strSql.AppendLine(" WHERE   (ParticelleCatastali.Validita_Inizio <= " & Agro_SQL_SaveDate(finestraTemporaleFine) & ") ")
            strSql.AppendLine(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(finestraTemporaleInizio) & ") ")

            If PROV <> "" Then
                strSql.AppendLine(" AND ParticelleCatastali.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                strSql.AppendLine(" AND ParticelleCatastali.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                strSql.AppendLine(" AND ParticelleCatastali.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
            End If

            If FOGLIO <> 0 Then
                strSql.AppendLine(" AND ParticelleCatastali.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                strSql.AppendLine(" AND ParticelleCatastali.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                strSql.AppendLine(" AND ParticelleCatastali.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND ParticelleCatastali.inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND ParticelleCatastali.inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSql.AppendLine(" GROUP BY ")
            strSql.Append(strColonneSql.ToString)
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_DOMINICALE,0) ")
            strSql.AppendLine(" , ISNULL(ParticelleCatastaliClassamento.REDDITO_AGRARIO,0) ")

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_x_anagrafica(
                        ByVal Tipo_1_normale_2_Full As String,
                           ByVal Piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT        i.PIVA + '_' + CAST(i.sa_cod AS nvarchar(15)) + '_' + CAST(i.PROV AS nvarchar(15)) + '_' + CAST(i.COM AS nvarchar(15)) + '_' + CAST(i.SEZIONE AS nvarchar(15)) + '_' + CAST(i.FOGLIO AS nvarchar(15)) + '_' + CAST(i.NUMERO AS nvarchar(15)) + '_' + CAST(i.SUBALTERNO AS nvarchar(15)) + '_' + CAST(p.Part_Cod AS nvarchar(15)) AS chiave,  ")
            StrSQL.Append("     i.PROV, i.COM, CASE i.SEZIONE WHEN '0' THEN '' ELSE i.SEZIONE END AS 'Sezione', i.FOGLIO, i.NUMERO, CASE i.SUBALTERNO WHEN '0' THEN '' ELSE i.SUBALTERNO END AS 'Subalterno',  ")
            StrSQL.Append("     CASE i.TitoloPossesso WHEN 1 THEN 'Proprietà' WHEN 2 THEN 'Comodato d''uso' WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' WHEN 5 THEN 'In conto terzi' WHEN 6 THEN 'In convenzione' ELSE 'Altro' END AS 'Titolo Possesso', i.Sup_Condotta as 'Sup. Condotta',  ")
            StrSQL.Append("     i.Validita_Inizio as 'Validità Inizio', i.Validita_Fine as 'Validità Fine',  ")
            StrSQL.Append("     CAST(CAST(p.ETTARI AS nvarchar(20)) + '.' + RIGHT('000' + CAST(p.ARE AS varchar(100)), 2) + '' + RIGHT('000' + CAST(p.CENTIARE AS varchar(100)), 2) AS float) AS 'Sup. Catastale' ")

            If Tipo_1_normale_2_Full = 1 Then
                StrSQL.Append(" FROM  ImpreseXParticelle AS i ")
                StrSQL.Append(" INNER JOIN ParticelleCatastali AS p ON i.PROV = p.PROV AND i.COM = p.COM AND i.SEZIONE = p.SEZIONE AND i.FOGLIO = p.FOGLIO AND i.NUMERO = p.NUMERO AND i.SUBALTERNO = p.SUBALTERNO ")
            End If

            If Tipo_1_normale_2_Full = 2 Then
                StrSQL.Append("         , a.APP_NOME ")
                StrSQL.Append(" FROM            Appezzamento AS a  ")
                StrSQL.Append("         INNER JOIN AppezzamentiXParticelle AS ap ON a.SA_COD = ap.SA_COD AND a.APPEZZA = ap.APPEZZA AND a.PIVA = ap.PIVA  ")
                StrSQL.Append("         RIGHT OUTER JOIN ImpreseXParticelle AS i  ")
                StrSQL.Append("         INNER JOIN ParticelleCatastali AS p ON i.PROV = p.PROV AND i.COM = p.COM AND i.SEZIONE = p.SEZIONE AND i.FOGLIO = p.FOGLIO AND i.NUMERO = p.NUMERO AND i.SUBALTERNO = p.SUBALTERNO ON ap.PIVA = i.PIVA AND ap.SA_COD = i.sa_cod AND ap.PROV = i.PROV AND ap.COM = i.COM AND ap.SEZIONE = i.SEZIONE AND ap.FOGLIO = i.FOGLIO AND ap.NUMERO = i.NUMERO AND ap.SUBALTERNO = i.SUBALTERNO ")

            End If


            StrSQL.Append(" where i.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" and  i.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND  i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY i.PROV, i.COM, i.SEZIONE, i.FOGLIO, i.NUMERO, i.SUBALTERNO ASC")
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
    Public Function Leggi_x_anagrafica_desc(
                           ByVal Piva As String,
                           ByVal Sa_cod As Integer,
                           ByVal Prov As String,
                           ByVal Com As String,
                           ByVal sezione As String,
                           ByVal Foglio As Integer,
                           ByVal Numero As Integer,
                           ByVal Subalterno As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("")
            StrSQL.AppendLine("Select ")
            StrSQL.AppendLine("i.PIVA + '_' + CAST(i.sa_cod AS nvarchar(15)) + '_' + CAST(i.PROV AS nvarchar(15)) + '_' + CAST(i.COM AS nvarchar(15)) + '_' + ")
            StrSQL.AppendLine("CAST(i.SEZIONE As nvarchar(15)) + '_' + CAST(i.FOGLIO AS nvarchar(15)) + '_' + CAST(i.NUMERO AS nvarchar(15)) + '_' + ")
            StrSQL.AppendLine("CAST(i.SUBALTERNO As nvarchar(15)) + '_' + CAST(p.Part_Cod AS nvarchar(15)) AS chiave, p.part_cod, ipc.Val_Cod as cod_particella, ")
            StrSQL.AppendLine("i.PIVA as Piva, ")
            StrSQL.AppendLine("i.Prov, ")
            StrSQL.AppendLine("i.Com, ")
            StrSQL.AppendLine("ist.LOCALITA as 'Comune', ")
            StrSQL.AppendLine("ist.COMUNI_PROV as 'Provincia', ")
            StrSQL.AppendLine("i.Sezione, --Case i.SEZIONE When '0' THEN '' ELSE i.SEZIONE END AS 'Sezione', ")
            StrSQL.AppendLine("i.FOGLIO,")
            StrSQL.AppendLine("i.NUMERO,")
            StrSQL.AppendLine("i.subalterno, --CASE i.SUBALTERNO WHEN '0' THEN '' ELSE i.SUBALTERNO END AS 'Subalterno',       ")
            StrSQL.AppendLine("Case i.TitoloPossesso When 1 Then 'Proprietà' WHEN 2 THEN 'Comodato d''uso' WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' WHEN 5 THEN 'In conto terzi' WHEN 6 THEN 'In convenzione' ELSE 'Altro' END AS 'Titolo_Possesso', ")
            StrSQL.AppendLine("i.Sup_Condotta as 'Sup_Condotta',       ")
            StrSQL.AppendLine("i.Validita_Inizio as 'Validita_Inizio', ")
            StrSQL.AppendLine("i.Validita_Fine as 'Validita_Fine',       ")
            StrSQL.AppendLine("CAST(CAST(p.ETTARI As nvarchar(20)) + '.' + RIGHT('000' + CAST(p.ARE AS varchar(100)), 2) + '' + RIGHT('000' + CAST(p.CENTIARE AS varchar(100)), 2) AS float) AS 'Sup_Catastale'     ,     ")
            StrSQL.AppendLine(" ca.sa_Cod, ca.sa_nome,   ")
            StrSQL.AppendLine(" (select [User] from utenti where CODICE_FISCALE = i.Username_Creazione ) as utente_creazione , i.Data_Creazione, ")
            StrSQL.AppendLine(" (select [User] from utenti where CODICE_FISCALE = i.Username_Modifica ) as utente_modifica , i.Data_Modifica ")
            StrSQL.AppendLine(" , i.ID AS Id_ImpresexParticelle ")

            StrSQL.AppendLine("From ImpreseXParticelle As i           ")
            StrSQL.AppendLine("INNER Join ParticelleCatastali AS p ON i.PROV = p.PROV And i.COM = p.COM And i.SEZIONE = p.SEZIONE And i.FOGLIO = p.FOGLIO And i.NUMERO = p.NUMERO And i.SUBALTERNO = p.SUBALTERNO ")
            StrSQL.AppendLine("LEFT JOIN Centri_Aziendali ca ON i.piva = ca.piva AND i.sa_cod = ca.sa_cod ")
            StrSQL.AppendLine("LEFT JOIN ImpreseXParticelle_Codici ipc ON i.ID = ipc.ID AND ipc.Id_Cod = " & enum_CodiciAnagrafe.CodiceParticella)
            StrSQL.AppendLine(" INNER JOIN ISTAT AS ist ON i.PROV = ist.PROV AND i.COM = ist.COM ")

            StrSQL.AppendLine(" where i.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If Sa_cod <> 0 Then
                StrSQL.AppendLine(" AND i.Sa_Cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
            End If

            If Prov <> "" Then
                StrSQL.AppendLine(" AND i.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.AppendLine(" AND i.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If sezione <> "" Then
                StrSQL.AppendLine(" AND i.sezione = '" & Agro_SQL_SaveText(sezione) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.AppendLine(" AND i.Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If

            If Numero <> 0 Then
                StrSQL.AppendLine(" AND i.Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If

            If Subalterno <> "" Then
                StrSQL.AppendLine(" AND i.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If

            StrSQL.AppendLine(" and  i.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND  i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")



            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY i.PROV, i.COM, i.SEZIONE, i.FOGLIO, i.NUMERO, i.SUBALTERNO ASC")
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_x_anagrafica_angular(
                           ByVal Piva As String,
                           ByVal Sa_cod As Integer,
                           ByVal Prov As String,
                           ByVal Com As String,
                           ByVal sezione As String,
                           ByVal Foglio As Integer,
                           ByVal Numero As Integer,
                           ByVal Subalterno As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("WITH #MetodiProduzione AS ( ")
            StrSQL.AppendLine("    SELECT i.ID, metodoProduzione.*, ")
            StrSQL.AppendLine("        CASE  ")
            StrSQL.AppendLine("        		WHEN MetodoProduzione_Cod = 1 THEN 'Integrato'  ")
            StrSQL.AppendLine("        		WHEN MetodoProduzione_Cod = 2 THEN 'In Conversione'  ")
            StrSQL.AppendLine("        		WHEN MetodoProduzione_Cod = 3 THEN 'Biologico'  ")
            StrSQL.AppendLine("        END AS MetodoProduzione_Des ")
            StrSQL.AppendLine("    FROM  ")
            StrSQL.AppendLine("    ImpreseXParticelle As i ")
            StrSQL.AppendLine("        INNER Join ParticelleCatastali AS p ON i.PROV = p.PROV And i.COM = p.COM And i.SEZIONE = p.SEZIONE And i.FOGLIO = p.FOGLIO And i.NUMERO = p.NUMERO And i.SUBALTERNO = p.SUBALTERNO  ")
            StrSQL.AppendLine("        CROSS APPLY (SELECT TOP 1 * FROM ParticelleCatastali_MetodoProduzione AS mp WHERE mp.PROV = p.PROV And mp.COM = p.COM And mp.SEZIONE = p.SEZIONE And mp.FOGLIO = p.FOGLIO And mp.NUMERO = p.NUMERO And mp.SUBALTERNO = p.SUBALTERNO ORDER BY Validita_Inizio DESC) metodoProduzione ")
            StrSQL.AppendLine("    where 1 = 1   ")
            If Piva <> "" Then
                StrSQL.Append("    AND i.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            StrSQL.AppendLine(") ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("Select ")
            StrSQL.AppendLine("    i.ID as Codice, ")
            StrSQL.AppendLine("    i.PIVA + '_' + CAST(i.sa_cod AS nvarchar(15)) + '_' + CAST(i.PROV AS nvarchar(15)) + '_' + CAST(i.COM AS nvarchar(15)) + '_' + ")
            StrSQL.AppendLine("    CAST(i.SEZIONE As nvarchar(15)) + '_' + CAST(i.FOGLIO AS nvarchar(15)) + '_' + CAST(i.NUMERO AS nvarchar(15)) + '_' + ")
            StrSQL.AppendLine("    CAST(i.SUBALTERNO As nvarchar(15)) + '_' + CAST(p.Part_Cod AS nvarchar(15)) AS chiave, p.part_cod, ipc.Val_Cod as cod_particella, ")
            StrSQL.AppendLine("    i.PIVA as Piva, ")
            StrSQL.AppendLine("    i.Prov, ")
            StrSQL.AppendLine("    i.Com, ")
            StrSQL.AppendLine("    ist.LOCALITA as 'Comune', ")
            StrSQL.AppendLine("    Lista_Province.Provincia as 'Provincia',  ")
            StrSQL.AppendLine("    Case i.SEZIONE When '0' THEN '' ELSE i.SEZIONE END AS 'Sezione', ")
            StrSQL.AppendLine("    i.FOGLIO,")
            StrSQL.AppendLine("    i.NUMERO,")
            StrSQL.AppendLine("    CASE i.SUBALTERNO WHEN '0' THEN '' ELSE i.SUBALTERNO END AS 'Subalterno',       ")
            StrSQL.AppendLine("    i.TitoloPossesso as Titolo_Possesso_Cod, ")
            StrSQL.AppendLine("    Case i.TitoloPossesso When 1 Then 'Proprietà' WHEN 2 THEN 'Comodato d''uso' WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' WHEN 5 THEN 'In conto terzi' WHEN 6 THEN 'In convenzione' ELSE 'Altro' END AS 'Titolo_Possesso', ")
            StrSQL.AppendLine("    i.Sup_Condotta as 'Sup_Condotta',       ")
            StrSQL.AppendLine("    i.Validita_Inizio as 'Validita_Inizio', ")
            StrSQL.AppendLine("    i.Validita_Fine as 'Validita_Fine',       ")
            StrSQL.AppendLine("    CAST(CAST(p.ETTARI As nvarchar(20)) + '.' + RIGHT('000' + CAST(p.ARE AS varchar(100)), 2) + '' + RIGHT('000' + CAST(p.CENTIARE AS varchar(100)), 2) AS float) AS 'Sup_Catastale'     ,     ")
            StrSQL.AppendLine("    ca.sa_Cod, ca.sa_nome,   ")
            StrSQL.AppendLine("    (select [User] from utenti where CODICE_FISCALE = i.Username_Creazione ) as utente_creazione , i.Data_Creazione, ")
            StrSQL.AppendLine("    (select [User] from utenti where CODICE_FISCALE = i.Username_Modifica ) as utente_modifica , i.Data_Modifica, ")
            StrSQL.AppendLine("    CASE WHEN i.Validita_Inizio < GETDATE() AND i.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo, ")
            StrSQL.AppendLine("    ISNULL(#MetodiProduzione.MetodoProduzione_Cod, 0) AS MetodoProduzione_Cod, ")
            StrSQL.AppendLine("    ISNULL(#MetodiProduzione.MetodoProduzione_Des, '') as MetodoProduzione_Des, ")
            StrSQL.AppendLine("    CASE WHEN zvn.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END As ZVN ")
            StrSQL.AppendLine("    , p.Proprietario")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("From ImpreseXParticelle As i           ")
            StrSQL.AppendLine("    INNER Join ParticelleCatastali AS p ON i.PROV = p.PROV And i.COM = p.COM And i.SEZIONE = p.SEZIONE And i.FOGLIO = p.FOGLIO And i.NUMERO = p.NUMERO And i.SUBALTERNO = p.SUBALTERNO ")
            StrSQL.AppendLine("    LEFT JOIN Centri_Aziendali ca ON i.piva = ca.piva AND i.sa_cod = ca.sa_cod ")
            StrSQL.AppendLine("    LEFT JOIN ImpreseXParticelle_Codici ipc ON i.ID = ipc.ID AND ipc.Id_Cod = " & enum_CodiciAnagrafe.CodiceParticella)
            StrSQL.AppendLine("    INNER JOIN ISTAT AS ist ON i.PROV = ist.PROV AND i.COM = ist.COM ")
            StrSQL.AppendLine("    INNER JOIN Lista_Province ON i.PROV = Lista_Province.PROV ")
            StrSQL.AppendLine("    LEFT JOIN #MetodiProduzione ON i.id = #MetodiProduzione.ID ")
            StrSQL.AppendLine("    LEFT JOIN ZonexParticelle zvn ON p.PROV = zvn.PROV AND p.COM = zvn.COM AND p.SEZIONE = zvn.SEZIONE AND p.FOGLIO = zvn.FOGLIO AND p.NUMERO = zvn.NUMERO AND p.SUBALTERNO = zvn.SUBALTERNO AND zvn.Zona_Cod = -17 ")

            StrSQL.Append(" where 1 = 1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND i.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_cod <> 0 Then
                StrSQL.Append(" AND i.Sa_Cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND i.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Prov <> "" Then
                StrSQL.Append(" AND i.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.Append(" AND i.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If sezione <> "" Then
                StrSQL.Append(" AND i.sezione = '" & Agro_SQL_SaveText(sezione) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND i.Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND i.Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If

            If Subalterno <> "" Then
                StrSQL.Append(" AND i.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If

            StrSQL.Append(" and  i.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND  i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY i.PROV, i.COM, i.SEZIONE, i.FOGLIO, i.NUMERO, i.SUBALTERNO ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '###############################################################################
    Public Function Recupera_Particelle_per_Centro(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal DataInizio As Date,
                                                  ByVal DataFine As Date,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal isBudget As Boolean = False,
                                                  Optional ByVal idBudget As Integer = 0
                                                  ) As DataTable

        '----------------------------------------------------------------
        '----- Dimensionamento variabili
        '----------------------------------------------------------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Recupera_Particelle_per_Centro()"

        Dim MessaggioErrore As String = ""

        Dim RSImpresexPart As DataTable

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim DtParticelle As New DataTable

        Dim Prov, Com, Sezione, Subalterno, ZVN As String
        Dim Foglio, Numero As Integer
        Dim Ettari As Integer
        Dim Are As Integer
        Dim Centiare As Integer
        Dim Superficie As Decimal
        Dim Sup_Condotta As Decimal
        Dim TitoloPossesso As Integer
        Dim inizioValidita, fineValidita As Date

        Dim stbQuery As New System.Text.StringBuilder

        Dim DrApp As DataRow()
        Dim ArrayDate As Date()

        Dim SupUtilizzataMax As Decimal = 0
        Dim SupSuCampiSquadri_Max As Decimal = 0
        Dim SupSuAppezzamentiLiberi_Max As Decimal = 0
        Dim SupSuAppezzamentiSuCampiNonSquadri_Max As Decimal = 0

        Dim Messaggio As String = Nothing

        '----------------------------------------------------------------
        '----- Elenco Appezzamenti Liberi e Utilizzati
        '----------------------------------------------------------------
        'La seguente query recupera tutte le particelle del centro aziendale indicato.
        'Per ogni particella compaiono anche la superficie di intersezione della particella con :
        '1. gli SQUADRI del centro;
        '2. gli appezzamenti liberi;
        '3. gli appezzamenti in campi NON SQUADRI;
        '----------------------------------------------------------------
        '----------------------------------------------------------------

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Prov", GetType(String)))
        Dt.Columns.Add(New DataColumn("Com", GetType(String)))
        Dt.Columns.Add(New DataColumn("COMUNI_PROV", GetType(String)))
        Dt.Columns.Add(New DataColumn("LOCALITA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_ettari", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_are", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_centiare", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieDisponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieUtilizzata", GetType(String)))
        Dt.Columns.Add(New DataColumn("AreaSuAppLiberi", GetType(String)))
        Dt.Columns.Add(New DataColumn("AreaSuCampiSquadri", GetType(String)))
        Dt.Columns.Add(New DataColumn("AreaSuAppSuCampiNonSquadri", GetType(String)))
        Dt.Columns.Add(New DataColumn("ZVN", GetType(String)))
        Dt.Columns.Add(New DataColumn("Titolo_Possesso", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))

        Dim objImprexPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

        'ricavo le particelle attive al momento dell'apertura dell'appezzamento
        Dim strFiltro As String = ""

        If DataInizio > Estremo_Validita_Inizio Then
            strFiltro = " AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                         " AND ImpreseXParticelle.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio)
        End If

        If strFiltro <> "" Then
            'elimino il primo AND
            strFiltro = Right(strFiltro, strFiltro.Length - 4)
        End If

        RSImpresexPart = objImprexPart.Leggi(
            0,
            CStr(Piva),
            CInt(Sa_Cod),
            0,
            "",
            "",
            "",
            0,
            0,
            "",
            enumSelezioneVariabile.Selezione_JoinDescrizioni,
            strFiltro,
            " ParticelleCatastali.prov,ParticelleCatastali.com,ParticelleCatastali.SEZIONE ,ParticelleCatastali.FOGLIO,ParticelleCatastali.numero",
            objParametri,
            0,
            True
            )

        If RSImpresexPart.Rows.Count > 0 Then
            Dim chiave_hash As String
            Dim table_hash As New Hashtable

            Dim ii As Integer

            Dim campiTableName As String = "#Campi"
            Dim campiXParticelleTableName As String = "#CampiXParticelle"
            Dim appezzamentoTableName As String = "#Appezzamento"
            Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

            Dim stbSelectionAdditionsQuadri As New System.Text.StringBuilder
            Dim stbSelectionAdditionsLiberi As New System.Text.StringBuilder
            Dim stbSelectionAdditionsNonQuadri As New System.Text.StringBuilder

            stbQuery.Length = 0

            stbSelectionAdditionsQuadri.Length = 0

            stbSelectionAdditionsQuadri.AppendLine("    --MI TIRO SU LE DATE DEL CAMPO PER LEGGERE CORRETTAMENTE LA SUP CONDOTTA DISPONIBILE")
            stbSelectionAdditionsQuadri.AppendLine("    , C.Validita_Inizio as ValiditaInizio_SupDisponibile")
            stbSelectionAdditionsQuadri.AppendLine("    , C.Validita_Fine as ValiditaFine_SupDisponibile")

            stbSelectionAdditionsLiberi.Length = 0

            stbSelectionAdditionsLiberi.AppendLine("    --MI TIRO SU LE DATE DEL CAMPO PER LEGGERE CORRETTAMENTE LA SUP CONDOTTA DISPONIBILE")
            stbSelectionAdditionsLiberi.AppendLine("    , A.Validita_Inizio as ValiditaInizio_SupDisponibile")
            stbSelectionAdditionsLiberi.AppendLine("    , A.Validita_Fine as ValiditaFine_SupDisponibile")

            stbSelectionAdditionsNonQuadri.Length = 0

            stbSelectionAdditionsNonQuadri.AppendLine("    --MI TIRO SU LE DATE DEL CAMPO PER LEGGERE CORRETTAMENTE LA SUP CONDOTTA DISPONIBILE")
            stbSelectionAdditionsNonQuadri.AppendLine("    , A.Validita_Inizio as ValiditaInizio_SupDisponibile")
            stbSelectionAdditionsNonQuadri.AppendLine("    , A.Validita_Fine as ValiditaFine_SupDisponibile")

            Me.GetQueryIntersezioneSuperficieParticelle(
                stbQuery,
                Piva,
                Sa_Cod,
                isBudget,
                idBudget,
                campiTableName,
                campiXParticelleTableName,
                appezzamentoTableName,
                appezzamentiXParticelleTableName,
                stbSelectionAdditionsQuadri:=stbSelectionAdditionsQuadri,
                stbSelectionAdditionsLiberi:=stbSelectionAdditionsLiberi,
                stbSelectionAdditionsNonQuadri:=stbSelectionAdditionsNonQuadri
                )

            Try
                '--------------------------------------------------------------------------
                DtParticelle = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Dt = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

            '----------------------------------------------------------------------
            '----------------------------------------------------------------------
            '----------------------------------------------------------------------

            For ii = 0 To RSImpresexPart.Rows.Count - 1

                'Per ogni particella ricavo la superficie di intersezione 
                '1. con eventuali SQUADRI
                '2. con altri appezzamenti liberi
                '3. con altri appezzamenti aggregati in campi nn squadri

                Prov = RSImpresexPart.Rows(ii).Item("prov")
                Com = RSImpresexPart.Rows(ii).Item("com")
                Sezione = RSImpresexPart.Rows(ii).Item("sezione")
                Foglio = CInt(RSImpresexPart.Rows(ii).Item("foglio"))
                Numero = CInt(RSImpresexPart.Rows(ii).Item("numero"))
                Subalterno = RSImpresexPart.Rows(ii).Item("subalterno")
                ZVN = RSImpresexPart.Rows(ii).Item("ZVN")
                TitoloPossesso = RSImpresexPart.Rows(ii).Item("TitoloPossesso")
                inizioValidita = RSImpresexPart.Rows(ii).Item("Validita_Inizio")
                fineValidita = RSImpresexPart.Rows(ii).Item("Validita_Fine")

                'creo la stringa CHIAVE
                chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno

                'verifico di non aver già inserito la particella...
                If Not table_hash.ContainsKey(chiave_hash) Then
                    table_hash.Add(chiave_hash, "")

                    'Creo una nuova riga e ne definisco i valori
                    Dr = Dt.NewRow

                    Dr.Item("prov") = Prov
                    Dr.Item("com") = Com
                    Dr.Item("Sezione") = If(Sezione = "0", "", Sezione)
                    Dr.Item("Foglio") = Foglio
                    Dr.Item("Numero") = Numero
                    Dr.Item("Subalterno") = If(Subalterno = "0", "", Subalterno)
                    Dr.Item("COMUNI_PROV") = RSImpresexPart.Rows(ii).Item("COMUNI_PROV")
                    Dr.Item("LOCALITA") = RSImpresexPart.Rows(ii).Item("LOCALITA")
                    Dr.Item("Part_Cod") = RSImpresexPart.Rows(ii).Item("part_cod")
                    Dr.Item("ZVN") = ZVN
                    Dr.Item("Titolo_Possesso") = TitoloPossesso
                    Dr.Item("Validita_Inizio") = inizioValidita
                    Dr.Item("Validita_Fine") = fineValidita

                    Dr.Item("Particella_ettari") = RSImpresexPart.Rows(ii).Item("ettari")
                    Dr.Item("Particella_are") = RSImpresexPart.Rows(ii).Item("are")
                    Dr.Item("Particella_centiare") = RSImpresexPart.Rows(ii).Item("centiare")

                    Ettari = RSImpresexPart.Rows(ii).Item("ettari")
                    Are = RSImpresexPart.Rows(ii).Item("are")
                    Centiare = RSImpresexPart.Rows(ii).Item("centiare")

                    Superficie = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)
                    Dr.Item("Superficie") = Format(Superficie, "0.0000")

                    Sup_Condotta = CDbl(RSImpresexPart.Rows(ii).Item("sup_condotta"))
                    Dr.Item("Sup_Condotta") = Format(Sup_Condotta, "0.0000")

                    DrApp = UtilityParticelle.FilterParcelRows(DtParticelle, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")

                    ArrayDate = UtilityParticelle.ExtractDateArray(DrApp, DataInizio, DataFine, "ValiditaInizio_SupDisponibile", "ValiditaFine_SupDisponibile")

                    SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "area", "ValiditaInizio_SupDisponibile", "ValiditaFine_SupDisponibile")
                    SupSuAppezzamentiLiberi_Max = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "AreaSuAppLiberi", "ValiditaInizio_SupDisponibile", "ValiditaFine_SupDisponibile")
                    SupSuCampiSquadri_Max = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "AreaSuCampiSquadri", "ValiditaInizio_SupDisponibile", "ValiditaFine_SupDisponibile")
                    SupSuAppezzamentiSuCampiNonSquadri_Max = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "AreaSuAppSuCampiNonSquadri", "ValiditaInizio_SupDisponibile", "ValiditaFine_SupDisponibile")

                    Dr.Item("SuperficieUtilizzata") = SupUtilizzataMax
                    Dr.Item("SuperficieDisponibile") = Sup_Condotta - SupUtilizzataMax

                    Dr.Item("AreaSuAppLiberi") = SupSuAppezzamentiLiberi_Max
                    Dr.Item("AreaSuCampiSquadri") = SupSuCampiSquadri_Max
                    Dr.Item("AreaSuAppSuCampiNonSquadri") = SupSuAppezzamentiSuCampiNonSquadri_Max

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                End If

            Next
        End If

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            'ERRORE
            MessaggioErrore = Messaggio
            Return Nothing
        Else
            MessaggioErrore = ""
            Return Dt
        End If

    End Function

    '###############################################################################
    Public Function Recupera_Particelle_per_Centro_con_Macrousi(ByVal Piva As String,
                                                               ByVal Sa_Cod As Integer,
                                                               ByVal DataInizio As Date,
                                                               ByVal DataFine As Date,
                                                               ByRef objParametri As AgronicaCoreParametri,
                                                               Optional ByVal isBudget As Boolean = False,
                                                               Optional ByVal idBudget As Integer = 0
                                                               ) As DataTable

        '----------------------------------------------------------------
        '----- Dimensionamento variabili
        '----------------------------------------------------------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi()"

        Dim MessaggioErrore As String = ""

        Dim DtImpresexPart As DataTable

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim DtParticelle As New DataTable

        Dim Prov, Com, Sezione, Subalterno, ZVN As String
        Dim Foglio, Numero As Integer
        Dim Ettari As Integer
        Dim Are As Integer
        Dim Centiare As Integer
        Dim Superficie As Decimal
        Dim Sup_Condotta As Decimal

        Dim stbQuery As New System.Text.StringBuilder

        Dim DrApp As DataRow()
        Dim ArrayDate As Date()

        Dim SupUtilizzataMax As Decimal = 0

        Dim Messaggio As String = Nothing

        '----------------------------------------------------------------
        '----- Elenco Appezzamenti Liberi e Utilizzati
        '----------------------------------------------------------------
        'La seguente query recupera tutte le particelle del centro aziendale indicato.
        'Per ogni particella compaiono anche la superficie di intersezione della particella con :
        '1. gli SQUADRI del centro;
        '2. gli appezzamenti liberi;
        '3. gli appezzamenti in campi NON SQUADRI;
        '----------------------------------------------------------------
        '----------------------------------------------------------------

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Prov", GetType(String)))
        Dt.Columns.Add(New DataColumn("Com", GetType(String)))
        Dt.Columns.Add(New DataColumn("COMUNI_PROV", GetType(String)))
        Dt.Columns.Add(New DataColumn("LOCALITA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_ettari", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_are", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_centiare", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieDisponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieUtilizzata", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Macrouso", GetType(String)))
        Dt.Columns.Add(New DataColumn("ZVN", GetType(String)))

        Dim objImprexPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

        'ricavo le particelle attive al momento dell'apertura dell'appezzamento
        Dim strFiltro As String = ""

        If DataInizio > Estremo_Validita_Inizio Then
            strFiltro = " AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ImpreseXParticelle.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio) &
                        " AND ParticelleCatastalixMacrousi.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ParticelleCatastalixMacrousi.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio)
        End If

        If strFiltro <> "" Then
            'elimino il primo AND
            strFiltro = Right(strFiltro, strFiltro.Length - 4)
        End If

        DtImpresexPart = objImprexPart.Leggi_conMacrousi(
            CStr(Piva),
            CInt(Sa_Cod),
            "",
            "",
            "",
            0,
            0,
            "",
            strFiltro,
            " ParticelleCatastali.prov,ParticelleCatastali.com,ParticelleCatastali.SEZIONE ,ParticelleCatastali.FOGLIO,ParticelleCatastali.numero",
            objParametri,
            True
            )

        If DtImpresexPart.Rows.Count > 0 Then
            Dim chiave_hash As String
            Dim table_hash As New Hashtable

            Dim ii As Integer

            '----------------------------------------------------------------------
            Dim campiTableName As String = "#Campi"
            Dim campiXParticelleTableName As String = "#CampiXParticelle"
            Dim appezzamentoTableName As String = "#Appezzamento"
            Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

            stbQuery.Length = 0

            Me.GetQueryIntersezioneSuperficieParticelle(
                stbQuery,
                Piva,
                Sa_Cod,
                isBudget,
                idBudget,
                campiTableName,
                campiXParticelleTableName,
                appezzamentoTableName,
                appezzamentiXParticelleTableName
                )

            Try
                '--------------------------------------------------------------------------
                DtParticelle = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Dt = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try


            For ii = 0 To DtImpresexPart.Rows.Count - 1

                'Per ogni particella ricavo la superficie di intersezione 
                '1. con eventuali SQUADRI
                '2. con altri appezzamenti liberi
                '3. con altri appezzamenti aggregati in campi nn squadri

                Prov = DtImpresexPart.Rows(ii).Item("prov")
                Com = DtImpresexPart.Rows(ii).Item("com")
                Sezione = DtImpresexPart.Rows(ii).Item("sezione")
                Foglio = CInt(DtImpresexPart.Rows(ii).Item("foglio"))
                Numero = CInt(DtImpresexPart.Rows(ii).Item("numero"))
                Subalterno = DtImpresexPart.Rows(ii).Item("subalterno")

                ZVN = DtImpresexPart.Rows(ii).Item("ZVN")

                'creo la stringa CHIAVE
                chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno & "|" & DtImpresexPart.Rows(ii).Item("Macrouso_Cod")

                'verifico di non aver già inserito la particella...
                If Not table_hash.ContainsKey(chiave_hash) Then
                    table_hash.Add(chiave_hash, "")

                    'Creo una nuova riga e ne definisco i valori
                    Dr = Dt.NewRow

                    Dr.Item("prov") = Prov
                    Dr.Item("com") = Com
                    Dr.Item("Sezione") = If(Sezione = "0", "", Sezione)
                    Dr.Item("Foglio") = Foglio
                    Dr.Item("Numero") = Numero
                    Dr.Item("Subalterno") = If(Subalterno = "0", "", Subalterno)
                    Dr.Item("COMUNI_PROV") = DtImpresexPart.Rows(ii).Item("COMUNI_PROV")
                    Dr.Item("LOCALITA") = DtImpresexPart.Rows(ii).Item("LOCALITA")
                    Dr.Item("Part_Cod") = DtImpresexPart.Rows(ii).Item("part_cod")
                    Dr.Item("ZVN") = ZVN

                    Dr.Item("Particella_ettari") = DtImpresexPart.Rows(ii).Item("ettari")
                    Dr.Item("Particella_are") = DtImpresexPart.Rows(ii).Item("are")
                    Dr.Item("Particella_centiare") = DtImpresexPart.Rows(ii).Item("centiare")

                    Ettari = DtImpresexPart.Rows(ii).Item("ettari")
                    Are = DtImpresexPart.Rows(ii).Item("are")
                    Centiare = DtImpresexPart.Rows(ii).Item("centiare")

                    Superficie = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)
                    Dr.Item("Superficie") = Format(Superficie, "0.0000")

                    Sup_Condotta = CDbl(DtImpresexPart.Rows(ii).Item("sup_condotta"))
                    Dr.Item("Sup_Condotta") = Format(Sup_Condotta, "0.0000")

                    DrApp = UtilityParticelle.FilterParcelRows(DtParticelle, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")

                    ArrayDate = UtilityParticelle.ExtractDateArray(DrApp, DataInizio, DataFine, "validita_inizio", "validita_fine")

                    SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "area", "validita_inizio", "validita_fine")

                    Dr.Item("SuperficieUtilizzata") = SupUtilizzataMax
                    Dr.Item("SuperficieDisponibile") = Sup_Condotta - SupUtilizzataMax

                    Dr.Item("Macrouso_Des") = DtImpresexPart.Rows(ii).Item("Macrouso_Des")
                    Dr.Item("Macrouso_Cod") = DtImpresexPart.Rows(ii).Item("Macrouso_Cod")

                    If Not IsDBNull(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")) Then
                        Dr.Item("Sup_Macrouso") = Format(CDbl(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")), "0.0000")
                    Else
                        Dr.Item("Sup_Macrouso") = ""
                    End If

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                End If

            Next

        End If


        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            'ERRORE
            MessaggioErrore = Messaggio
            Return Nothing
        Else
            MessaggioErrore = ""
            Return Dt
        End If

    End Function

    '### Paolo: Ho aggiunto la possibilità di filtrare la query
    '###############################################################################
    Public Function Recupera_Particelle_per_Centro_con_Macrousi2(
                                                                ByVal Piva As String,
                                                                ByVal Sa_Cod As Integer,
                                                                ByVal DataInizio As Date,
                                                                ByVal DataFine As Date,
                                                                ByVal filtro_macrousi As String,
                                                                ByRef objParametri As AgronicaCoreParametri,
                                                                Optional ByVal isBudget As Boolean = False,
                                                                Optional ByVal idBudget As Integer = 0
                                                                ) As DataTable

        '----------------------------------------------------------------
        '----- Dimensionamento variabili
        '----------------------------------------------------------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi()"

        Dim MessaggioErrore As String = ""

        Dim DtImpresexPart As DataTable

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim DtParticelle As New DataTable

        Dim Prov, Com, Sezione, Subalterno As String
        Dim Foglio, Numero As Integer
        Dim Ettari As Integer
        Dim Are As Integer
        Dim Centiare As Integer
        Dim Superficie As Decimal
        Dim Sup_Condotta As Decimal

        Dim stbQuery As New System.Text.StringBuilder

        Dim DrApp As DataRow()
        Dim ArrayDate As Date()

        Dim SupUtilizzataMax As Decimal = 0

        Dim Messaggio As String = Nothing

        '----------------------------------------------------------------
        '----- Elenco Appezzamenti Liberi e Utilizzati
        '----------------------------------------------------------------
        'La seguente query recupera tutte le particelle del centro aziendale indicato.
        'Per ogni particella compaiono anche la superficie di intersezione della particella con :
        '1. gli SQUADRI del centro;
        '2. gli appezzamenti liberi;
        '3. gli appezzamenti in campi NON SQUADRI;
        '----------------------------------------------------------------
        '----------------------------------------------------------------

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Prov", GetType(String)))
        Dt.Columns.Add(New DataColumn("Com", GetType(String)))
        Dt.Columns.Add(New DataColumn("COMUNI_PROV", GetType(String)))
        Dt.Columns.Add(New DataColumn("LOCALITA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_ettari", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_are", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_centiare", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieDisponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieUtilizzata", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Macrouso", GetType(String)))

        Dim objImprexPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

        'ricavo le particelle attive al momento dell'apertura dell'appezzamento
        Dim strFiltro As String = ""

        If DataInizio > Estremo_Validita_Inizio Then
            strFiltro = " AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ImpreseXParticelle.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio) &
                        " AND ParticelleCatastalixMacrousi.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ParticelleCatastalixMacrousi.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio)
        End If

        ''' Paolo: Aggiungo il nuovo filtro
        strFiltro &= filtro_macrousi

        If strFiltro <> "" Then
            'elimino il primo AND
            strFiltro = Right(strFiltro, strFiltro.Length - 4)
        End If

        DtImpresexPart = objImprexPart.Leggi_conMacrousi(
            CStr(Piva),
            CInt(Sa_Cod),
            "",
            "",
            "",
            0,
            0,
            "",
            strFiltro,
            " ParticelleCatastali.prov,ParticelleCatastali.com,ParticelleCatastali.SEZIONE ,ParticelleCatastali.FOGLIO,ParticelleCatastali.numero",
            objParametri
            )

        If DtImpresexPart.Rows.Count > 0 Then
            Dim chiave_hash As String
            Dim table_hash As New Hashtable

            Dim ii As Integer

            '----------------------------------------------------------------------
            Dim campiTableName As String = "#Campi"
            Dim campiXParticelleTableName As String = "#CampiXParticelle"
            Dim appezzamentoTableName As String = "#Appezzamento"
            Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

            stbQuery.Length = 0

            Me.GetQueryIntersezioneSuperficieParticelle(
                stbQuery,
                Piva,
                Sa_Cod,
                isBudget,
                idBudget,
                campiTableName,
                campiXParticelleTableName,
                appezzamentoTableName,
                appezzamentiXParticelleTableName
                )

            Try
                '--------------------------------------------------------------------------
                DtParticelle = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Dt = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

            For ii = 0 To DtImpresexPart.Rows.Count - 1

                'Per ogni particella ricavo la superficie di intersezione 
                '1. con eventuali SQUADRI
                '2. con altri appezzamenti liberi
                '3. con altri appezzamenti aggregati in campi nn squadri

                Prov = DtImpresexPart.Rows(ii).Item("prov")
                Com = DtImpresexPart.Rows(ii).Item("com")
                Sezione = DtImpresexPart.Rows(ii).Item("sezione")
                Foglio = CInt(DtImpresexPart.Rows(ii).Item("foglio"))
                Numero = CInt(DtImpresexPart.Rows(ii).Item("numero"))
                Subalterno = DtImpresexPart.Rows(ii).Item("subalterno")

                'creo la stringa CHIAVE
                chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno & "|" & DtImpresexPart.Rows(ii).Item("Macrouso_Cod")

                If Not table_hash.ContainsKey(chiave_hash) Then
                    table_hash.Add(chiave_hash, "")

                    'Creo una nuova riga e ne definisco i valori
                    Dr = Dt.NewRow

                    Dr.Item("prov") = Prov
                    Dr.Item("com") = Com
                    Dr.Item("Sezione") = If(Sezione = "0", "", Sezione)
                    Dr.Item("Foglio") = Foglio
                    Dr.Item("Numero") = Numero
                    Dr.Item("Subalterno") = If(Subalterno = "0", "", Subalterno)
                    Dr.Item("COMUNI_PROV") = DtImpresexPart.Rows(ii).Item("COMUNI_PROV")
                    Dr.Item("LOCALITA") = DtImpresexPart.Rows(ii).Item("LOCALITA")
                    Dr.Item("Part_Cod") = DtImpresexPart.Rows(ii).Item("part_cod")

                    Dr.Item("Particella_ettari") = DtImpresexPart.Rows(ii).Item("ettari")
                    Dr.Item("Particella_are") = DtImpresexPart.Rows(ii).Item("are")
                    Dr.Item("Particella_centiare") = DtImpresexPart.Rows(ii).Item("centiare")

                    Ettari = DtImpresexPart.Rows(ii).Item("ettari")
                    Are = DtImpresexPart.Rows(ii).Item("are")
                    Centiare = DtImpresexPart.Rows(ii).Item("centiare")

                    Superficie = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)
                    Dr.Item("Superficie") = Format(Superficie, "0.0000")

                    Sup_Condotta = CDbl(DtImpresexPart.Rows(ii).Item("sup_condotta"))
                    Dr.Item("Sup_Condotta") = Format(Sup_Condotta, "0.0000")

                    DrApp = UtilityParticelle.FilterParcelRows(DtParticelle, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")

                    ArrayDate = UtilityParticelle.ExtractDateArray(DrApp, DataInizio, DataFine, "validita_inizio", "validita_fine")

                    SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "area", "validita_inizio", "validita_fine")

                    Dr.Item("SuperficieUtilizzata") = SupUtilizzataMax
                    Dr.Item("SuperficieDisponibile") = Sup_Condotta - SupUtilizzataMax

                    Dr.Item("Macrouso_Des") = DtImpresexPart.Rows(ii).Item("Macrouso_Des")
                    Dr.Item("Macrouso_Cod") = DtImpresexPart.Rows(ii).Item("Macrouso_Cod")

                    If Not IsDBNull(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")) Then
                        Dr.Item("Sup_Macrouso") = Format(CDbl(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")), "0.0000")
                    Else
                        Dr.Item("Sup_Macrouso") = ""
                    End If

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                End If

            Next

        End If

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            'ERRORE
            MessaggioErrore = Messaggio
            Return Nothing
        Else
            MessaggioErrore = ""
            Return Dt
        End If

    End Function

    '###############################################################################
    Public Function Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(
                                                                       ByVal Piva As String,
                                                                       ByVal Sa_Cod As Integer,
                                                                       ByVal DataInizio As Date,
                                                                       ByVal DataFine As Date,
                                                                       ByRef objParametri As AgronicaCoreParametri,
                                                                       Optional ByVal isBudget As Boolean = False,
                                                                       Optional ByVal idBudget As Integer = 0
                                                                       ) As DataTable

        '----------------------------------------------------------------
        '----- Dimensionamento variabili
        '----------------------------------------------------------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi()"

        Dim MessaggioErrore As String = ""

        Dim DtImpresexPart As DataTable

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim DtParticelle As New DataTable

        Dim Prov, Com, Sezione, Subalterno, ZVN As String
        Dim Foglio, Numero As Integer
        Dim Ettari As Integer
        Dim Are As Integer
        Dim Centiare As Integer
        Dim Superficie As Decimal
        Dim Sup_Condotta As Decimal

        Dim stbQuery As New System.Text.StringBuilder

        Dim DrApp As DataRow()
        Dim ArrayDate As Date()

        Dim SupUtilizzataMax As Decimal = 0

        Dim Messaggio As String = Nothing

        '----------------------------------------------------------------
        '----- Elenco Appezzamenti Liberi e Utilizzati
        '----------------------------------------------------------------
        'La seguente query recupera tutte le particelle del centro aziendale indicato.
        'Per ogni particella compaiono anche la superficie di intersezione della particella con :
        '1. gli SQUADRI del centro;
        '2. gli appezzamenti liberi;
        '3. gli appezzamenti in campi NON SQUADRI;
        '----------------------------------------------------------------
        '----------------------------------------------------------------

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Prov", GetType(String)))
        Dt.Columns.Add(New DataColumn("Com", GetType(String)))
        Dt.Columns.Add(New DataColumn("COMUNI_PROV", GetType(String)))
        Dt.Columns.Add(New DataColumn("LOCALITA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_ettari", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_are", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_centiare", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieDisponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieUtilizzata", GetType(String)))

        Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Macrouso", GetType(String)))

        Dt.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Utilizzo", GetType(String)))

        Dt.Columns.Add(New DataColumn("ZVN", GetType(String)))

        Dim objImprexPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

        'ricavo le particelle attive al momento dell'apertura dell'appezzamento
        Dim strFiltro As String = ""

        If DataInizio > Estremo_Validita_Inizio Then
            strFiltro = " AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ImpreseXParticelle.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio) &
                        " AND ParticelleCatastalixMacrousi.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ParticelleCatastalixMacrousi.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio) &
                        " AND ParticelleCatastalixMacrousixUtilizzo.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ParticelleCatastalixMacrousixUtilizzo.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio)

        End If

        If strFiltro <> "" Then
            'elimino il primo AND
            strFiltro = Right(strFiltro, strFiltro.Length - 4)
        End If

        DtImpresexPart = objImprexPart.Leggi_conMacrousiUtilizzi(
            CStr(Piva),
            CInt(Sa_Cod),
            "",
            "",
            "",
            0,
            0,
            "",
            strFiltro,
            " ParticelleCatastali.prov,ParticelleCatastali.com,ParticelleCatastali.SEZIONE ,ParticelleCatastali.FOGLIO,ParticelleCatastali.numero",
            objParametri,
            True
            )

        If DtImpresexPart.Rows.Count > 0 Then
            Dim chiave_hash As String
            Dim table_hash As New Hashtable

            Dim ii As Integer

            '----------------------------------------------------------------------
            Dim campiTableName As String = "#Campi"
            Dim campiXParticelleTableName As String = "#CampiXParticelle"
            Dim appezzamentoTableName As String = "#Appezzamento"
            Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

            stbQuery.Length = 0

            Me.GetQueryIntersezioneSuperficieParticelle(
                stbQuery,
                Piva,
                Sa_Cod,
                isBudget,
                idBudget,
                campiTableName,
                campiXParticelleTableName,
                appezzamentoTableName,
                appezzamentiXParticelleTableName
                )

            Try
                '--------------------------------------------------------------------------
                DtParticelle = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Dt = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

            For ii = 0 To DtImpresexPart.Rows.Count - 1

                'Per ogni particella ricavo la superficie di intersezione 
                '1. con eventuali SQUADRI
                '2. con altri appezzamenti liberi
                '3. con altri appezzamenti aggregati in campi nn squadri

                Prov = DtImpresexPart.Rows(ii).Item("prov")
                Com = DtImpresexPart.Rows(ii).Item("com")
                Sezione = DtImpresexPart.Rows(ii).Item("sezione")
                Foglio = CInt(DtImpresexPart.Rows(ii).Item("foglio"))
                Numero = CInt(DtImpresexPart.Rows(ii).Item("numero"))
                Subalterno = DtImpresexPart.Rows(ii).Item("subalterno")
                ZVN = DtImpresexPart.Rows(ii).Item("ZVN")

                'creo la stringa CHIAVE
                chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno & "|" & DtImpresexPart.Rows(ii).Item("Macrouso_Cod") & "|" & DtImpresexPart.Rows(ii).Item("Veg_Cod_Agea").ToString & "|" & DtImpresexPart.Rows(ii).Item("Cul_Cod_Agea").ToString

                If Not table_hash.ContainsKey(chiave_hash) Then
                    table_hash.Add(chiave_hash, "")

                    'Creo una nuova riga e ne definisco i valori
                    Dr = Dt.NewRow

                    Dr.Item("prov") = Prov
                    Dr.Item("com") = Com
                    Dr.Item("Sezione") = If(Sezione = "0", "", Sezione)
                    Dr.Item("Foglio") = Foglio
                    Dr.Item("Numero") = Numero
                    Dr.Item("Subalterno") = If(Subalterno = "0", "", Subalterno)
                    Dr.Item("COMUNI_PROV") = DtImpresexPart.Rows(ii).Item("COMUNI_PROV")
                    Dr.Item("LOCALITA") = DtImpresexPart.Rows(ii).Item("LOCALITA")
                    Dr.Item("Part_Cod") = DtImpresexPart.Rows(ii).Item("part_cod")
                    Dr.Item("ZVN") = ZVN

                    Dr.Item("Particella_ettari") = DtImpresexPart.Rows(ii).Item("ettari")
                    Dr.Item("Particella_are") = DtImpresexPart.Rows(ii).Item("are")
                    Dr.Item("Particella_centiare") = DtImpresexPart.Rows(ii).Item("centiare")

                    Ettari = DtImpresexPart.Rows(ii).Item("ettari")
                    Are = DtImpresexPart.Rows(ii).Item("are")
                    Centiare = DtImpresexPart.Rows(ii).Item("centiare")

                    Superficie = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)
                    Dr.Item("Superficie") = Format(Superficie, "0.0000")

                    Sup_Condotta = CDbl(DtImpresexPart.Rows(ii).Item("sup_condotta"))
                    Dr.Item("Sup_Condotta") = Format(Sup_Condotta, "0.0000")

                    DrApp = UtilityParticelle.FilterParcelRows(DtParticelle, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")

                    ArrayDate = UtilityParticelle.ExtractDateArray(DrApp, DataInizio, DataFine, "validita_inizio", "validita_fine")

                    SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "area", "validita_inizio", "validita_fine")

                    Dr.Item("SuperficieUtilizzata") = SupUtilizzataMax
                    Dr.Item("SuperficieDisponibile") = Sup_Condotta - SupUtilizzataMax

                    Dr.Item("Macrouso_Des") = DtImpresexPart.Rows(ii).Item("Macrouso_Des")
                    Dr.Item("Macrouso_Cod") = DtImpresexPart.Rows(ii).Item("Macrouso_Cod")

                    If Not IsDBNull(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")) Then
                        Dr.Item("Sup_Macrouso") = Format(CDbl(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")), "0.0000")
                    Else
                        Dr.Item("Sup_Macrouso") = ""
                    End If

                    Dr.Item("Veg_Des_Agea") = DtImpresexPart.Rows(ii).Item("Veg_Des_Agea")
                    Dr.Item("Veg_Cod_Agea") = DtImpresexPart.Rows(ii).Item("Veg_Cod_Agea")
                    Dr.Item("Cul_Des_Agea") = DtImpresexPart.Rows(ii).Item("Cul_Des_Agea")
                    Dr.Item("Cul_Cod_Agea") = DtImpresexPart.Rows(ii).Item("Cul_Cod_Agea")

                    If Not IsDBNull(DtImpresexPart.Rows(ii).Item("Sup_Utilizzo")) Then
                        Dr.Item("Sup_Utilizzo") = Format(CDbl(DtImpresexPart.Rows(ii).Item("Sup_Utilizzo")), "0.0000")
                    Else
                        Dr.Item("Sup_Utilizzo") = ""
                    End If

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                End If

            Next

        End If

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            'ERRORE
            MessaggioErrore = Messaggio
            Return Nothing
        Else
            MessaggioErrore = ""
            Return Dt
        End If

    End Function

    '###############################################################################
    Public Function Recupera_Particelle_per_Centro_con_MacrousiUtilizzi2(
                                                                        ByVal Piva As String,
                                                                        ByVal Sa_Cod As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal filtro_macrousi As String,
                                                                        ByVal filtro_utilizzi As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                        Optional ByVal isBudget As Boolean = False,
                                                                        Optional ByVal idBudget As Integer = 0
                                                                        ) As DataTable

        '----------------------------------------------------------------
        '----- Dimensionamento variabili
        '----------------------------------------------------------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi()"

        Dim MessaggioErrore As String = ""

        Dim DtImpresexPart As DataTable

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim DtParticelle As New DataTable

        Dim Prov, Com, Sezione, Subalterno As String
        Dim Foglio, Numero As Integer
        Dim Ettari As Integer
        Dim Are As Integer
        Dim Centiare As Integer
        Dim Superficie As Decimal
        Dim Sup_Condotta As Decimal

        Dim stbQuery As New System.Text.StringBuilder

        Dim DrApp As DataRow()
        Dim ArrayDate As Date()

        Dim SupUtilizzataMax As Decimal = 0

        Dim Messaggio As String = Nothing

        '----------------------------------------------------------------
        '----- Elenco Appezzamenti Liberi e Utilizzati
        '----------------------------------------------------------------
        'La seguente query recupera tutte le particelle del centro aziendale indicato.
        'Per ogni particella compaiono anche la superficie di intersezione della particella con :
        '1. gli SQUADRI del centro;
        '2. gli appezzamenti liberi;
        '3. gli appezzamenti in campi NON SQUADRI;
        '----------------------------------------------------------------
        '----------------------------------------------------------------

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Prov", GetType(String)))
        Dt.Columns.Add(New DataColumn("Com", GetType(String)))
        Dt.Columns.Add(New DataColumn("COMUNI_PROV", GetType(String)))
        Dt.Columns.Add(New DataColumn("LOCALITA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_ettari", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_are", GetType(String)))
        Dt.Columns.Add(New DataColumn("Particella_centiare", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieDisponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieUtilizzata", GetType(String)))

        Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Macrouso", GetType(String)))

        Dt.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Utilizzo", GetType(String)))

        Dim objImprexPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

        'ricavo le particelle attive al momento dell'apertura dell'appezzamento
        Dim strFiltro As String = ""

        If DataInizio > Estremo_Validita_Inizio Then
            strFiltro = " AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ImpreseXParticelle.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio) &
                        " AND ParticelleCatastalixMacrousi.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ParticelleCatastalixMacrousi.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio) &
                        " AND ParticelleCatastalixMacrousixUtilizzo.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
                        " AND ParticelleCatastalixMacrousixUtilizzo.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio)

        End If

        ''' Paolo: Aggiungo il nuovo filtro
        strFiltro &= filtro_macrousi & filtro_utilizzi

        If strFiltro <> "" Then
            'elimino il primo AND
            strFiltro = Right(strFiltro, strFiltro.Length - 4)
        End If

        DtImpresexPart = objImprexPart.Leggi_conMacrousiUtilizzi(
            CStr(Piva),
            CInt(Sa_Cod),
            "",
            "",
            "",
            0,
            0,
            "",
            strFiltro,
            " ParticelleCatastali.prov,ParticelleCatastali.com,ParticelleCatastali.SEZIONE ,ParticelleCatastali.FOGLIO,ParticelleCatastali.numero",
            objParametri
            )

        If DtImpresexPart.Rows.Count > 0 Then
            Dim chiave_hash As String
            Dim table_hash As New Hashtable

            Dim ii As Integer

            '----------------------------------------------------------------------
            Dim campiTableName As String = "#Campi"
            Dim campiXParticelleTableName As String = "#CampiXParticelle"
            Dim appezzamentoTableName As String = "#Appezzamento"
            Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

            stbQuery.Length = 0

            Me.GetQueryIntersezioneSuperficieParticelle(
                stbQuery,
                Piva,
                Sa_Cod,
                isBudget,
                idBudget,
                campiTableName,
                campiXParticelleTableName,
                appezzamentoTableName,
                appezzamentiXParticelleTableName
                )

            Try
                '--------------------------------------------------------------------------
                DtParticelle = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Dt = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

            For ii = 0 To DtImpresexPart.Rows.Count - 1

                'Per ogni particella ricavo la superficie di intersezione 
                '1. con eventuali SQUADRI
                '2. con altri appezzamenti liberi
                '3. con altri appezzamenti aggregati in campi nn squadri

                Prov = DtImpresexPart.Rows(ii).Item("prov")
                Com = DtImpresexPart.Rows(ii).Item("com")
                Sezione = DtImpresexPart.Rows(ii).Item("sezione")
                Foglio = CInt(DtImpresexPart.Rows(ii).Item("foglio"))
                Numero = CInt(DtImpresexPart.Rows(ii).Item("numero"))
                Subalterno = DtImpresexPart.Rows(ii).Item("subalterno")

                'creo la stringa CHIAVE
                chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno & "|" & DtImpresexPart.Rows(ii).Item("Macrouso_Cod") & "|" & DtImpresexPart.Rows(ii).Item("Veg_Cod_Agea").ToString & "|" & DtImpresexPart.Rows(ii).Item("Cul_Cod_Agea").ToString

                If Not table_hash.ContainsKey(chiave_hash) Then
                    table_hash.Add(chiave_hash, "")

                    'Creo una nuova riga e ne definisco i valori
                    Dr = Dt.NewRow

                    Dr.Item("prov") = Prov
                    Dr.Item("com") = Com
                    Dr.Item("Sezione") = IIf(Sezione = "0", "", Sezione)
                    Dr.Item("Foglio") = Foglio
                    Dr.Item("Numero") = Numero
                    Dr.Item("Subalterno") = IIf(Subalterno = "0", "", Subalterno)
                    Dr.Item("COMUNI_PROV") = DtImpresexPart.Rows(ii).Item("COMUNI_PROV")
                    Dr.Item("LOCALITA") = DtImpresexPart.Rows(ii).Item("LOCALITA")
                    Dr.Item("Part_Cod") = DtImpresexPart.Rows(ii).Item("part_cod")

                    Dr.Item("Particella_ettari") = DtImpresexPart.Rows(ii).Item("ettari")
                    Dr.Item("Particella_are") = DtImpresexPart.Rows(ii).Item("are")
                    Dr.Item("Particella_centiare") = DtImpresexPart.Rows(ii).Item("centiare")

                    Ettari = DtImpresexPart.Rows(ii).Item("ettari")
                    Are = DtImpresexPart.Rows(ii).Item("are")
                    Centiare = DtImpresexPart.Rows(ii).Item("centiare")

                    Superficie = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)
                    Dr.Item("Superficie") = Format(Superficie, "0.0000")

                    Sup_Condotta = CDbl(DtImpresexPart.Rows(ii).Item("sup_condotta"))
                    Dr.Item("Sup_Condotta") = Format(Sup_Condotta, "0.0000")

                    DrApp = UtilityParticelle.FilterParcelRows(DtParticelle, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")

                    ArrayDate = UtilityParticelle.ExtractDateArray(DrApp, DataInizio, DataFine, "validita_inizio", "validita_fine")

                    SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrApp, "area", "validita_inizio", "validita_fine")

                    Dr.Item("SuperficieUtilizzata") = SupUtilizzataMax
                    Dr.Item("SuperficieDisponibile") = Sup_Condotta - SupUtilizzataMax

                    Dr.Item("Macrouso_Des") = DtImpresexPart.Rows(ii).Item("Macrouso_Des")
                    Dr.Item("Macrouso_Cod") = DtImpresexPart.Rows(ii).Item("Macrouso_Cod")

                    If Not IsDBNull(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")) Then
                        Dr.Item("Sup_Macrouso") = Format(CDbl(DtImpresexPart.Rows(ii).Item("Sup_Macrouso")), "0.0000")
                    Else
                        Dr.Item("Sup_Macrouso") = ""
                    End If

                    Dr.Item("Veg_Des_Agea") = DtImpresexPart.Rows(ii).Item("Veg_Des_Agea")
                    Dr.Item("Veg_Cod_Agea") = DtImpresexPart.Rows(ii).Item("Veg_Cod_Agea")
                    Dr.Item("Cul_Des_Agea") = DtImpresexPart.Rows(ii).Item("Cul_Des_Agea")
                    Dr.Item("Cul_Cod_Agea") = DtImpresexPart.Rows(ii).Item("Cul_Cod_Agea")

                    If Not IsDBNull(DtImpresexPart.Rows(ii).Item("Sup_Utilizzo")) Then
                        Dr.Item("Sup_Utilizzo") = Format(CDbl(DtImpresexPart.Rows(ii).Item("Sup_Utilizzo")), "0.0000")
                    Else
                        Dr.Item("Sup_Utilizzo") = ""
                    End If

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                End If

            Next

        End If

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            'ERRORE
            MessaggioErrore = Messaggio
            Return Nothing
        Else
            MessaggioErrore = ""
            Return Dt
        End If

    End Function

    '##############################################################################################
    Public Function Esiste_Particella(ByVal Prov As String,
                                     ByVal Com As String,
                                     ByVal Sezione As String,
                                     ByVal Foglio As Integer,
                                     ByVal Numero As Integer,
                                     ByVal Subalterno As String,
                                     ByRef Data_Modifica As Date,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Esiste_Particella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False

        If Sezione = "" Then
            Sezione = "0"
        End If
        If Subalterno = "" Then
            Subalterno = "0"
        End If

        Try
            '---------------------------------------------
            'Genero la query SQL
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM    ParticelleCatastali ")
            StrSQL.AppendLine(" WHERE   Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            StrSQL.AppendLine(" AND     Com = '" & Agro_SQL_SaveText(Com) & "' ")
            StrSQL.AppendLine(" AND     Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            StrSQL.AppendLine(" AND     Foglio = " & Foglio & " ")
            StrSQL.AppendLine(" AND     Numero = " & Numero & " ")
            StrSQL.AppendLine(" AND     Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing Then
                If DT.Rows.Count = 1 Then

                    Data_Modifica = CDate(DT.Rows(0).Item("data_modifica"))
                    bRet = True

                ElseIf DT.Rows.Count = 0 Then

                    Data_Modifica = Nothing
                    bRet = False

                Else
                    Throw New Exception("Sono presenti più particelle con quel codice, verificare sul db i record  ")
                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return bRet

    End Function

    '################################################################################
    Public Function Recupera_Superfici_Particella(ByVal Prov As String,
                                                  ByVal Com As String,
                                                  ByVal Sezione As String,
                                                  ByVal Foglio As Integer,
                                                  ByVal Numero As Integer,
                                                  ByVal Subalterno As String,
                                                  ByRef Ettari As Decimal,
                                                  ByRef Are As Integer,
                                                  ByRef Centiare As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Esiste_Particella()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim bRet As Boolean = False


        'modifico la validita di inizio e fine
        Dim appInizio As Date
        Dim appFine As Date
        appInizio = objParametri.FinestraTemporaleInizio
        appFine = objParametri.FinestraTemporaleFine
        objParametri.FinestraTemporaleInizio = Now.Date
        objParametri.FinestraTemporaleFine = Now.Date


        Dim objParticella As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
        DT = objParticella.Leggi(0, Prov, Com, Sezione, Foglio, Numero, Subalterno,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametri)

        'reimposto i parametri
        objParametri.FinestraTemporaleInizio = appInizio
        objParametri.FinestraTemporaleFine = appFine

        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
            Ettari = DT.Rows(0).Item("Ettari")
            Are = DT.Rows(0).Item("Are")
            Centiare = DT.Rows(0).Item("Centiare")
        End If

    End Function

    '################################################################################
    Public Function Anagrafica_Particelle_Leggi(
                                        ByVal Piva As String,
                                        ByVal Optional_SaCod As Integer,
                                        ByVal Data_Inizio As Date,
                                        ByVal Data_Fine As Date,
                                            ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal strFiltro As String = "") _
                                        As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Anagrafica_Particelle_Leggi()"

        Dim MessaggioErrore As String
        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  DISTINCT ")
            StrSQL.AppendLine("         0 as IndiceParticella, ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.COM + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE  + '_' + ")
            StrSQL.AppendLine("         CONVERT(varchar(10), ParticelleCatastali.FOGLIO) + '_' + ")
            StrSQL.AppendLine("         CONVERT(varchar(10), ParticelleCatastali.NUMERO) + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.SUBALTERNO AS PART_UNID, ")

            StrSQL.AppendLine("         ImpreseXParticelle.PIVA, ImpreseXParticelle.sa_cod, Centri_Aziendali.sa_nome,  ")
            StrSQL.AppendLine("         ISTAT.COMUNI_PROV, ISTAT.LOCALITA, ISNULL(ISTAT.CAP, '') CAP,  ")
            StrSQL.AppendLine("         ParticelleCatastali.Part_Cod,  ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV, ParticelleCatastali.COM, ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO,  ")
            StrSQL.AppendLine("         ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO,  ")
            StrSQL.AppendLine("         ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ParticelleCatastali.CENTIARE, ")
            StrSQL.AppendLine("         ImpreseXParticelle.Sup_Condotta, ImpreseXParticelle.Sup_Spandibile,  ImpreseXParticelle.ID, ")
            StrSQL.AppendLine("         ImpreseXParticelle.Validita_Inizio, ImpreseXParticelle.Validita_Fine, ")
            StrSQL.AppendLine("         ImpreseXParticelle.TitoloPossesso, ")
            StrSQL.AppendLine("         CASE ImpreseXParticelle.TitoloPossesso WHEN 1 THEN 'Proprietà' WHEN 2 THEN 'Comodato d''uso' ")
            StrSQL.AppendLine("                                                WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' ")
            StrSQL.AppendLine("                                                WHEN 5 THEN 'In conto terzi' WHEN 6 THEN 'In convenzione' ELSE 'Altro' END AS TitoloPossesso_Des, ")

            StrSQL.AppendLine("")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 1) ")
            StrSQL.AppendLine("    , 0) AS Sup_Seminabile,  ")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 3) ")
            StrSQL.AppendLine("    , 0) AS Sup_Unar,  ")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1  ImpresexParticelle_Codici.Val_Cod ")
            StrSQL.AppendLine("		    FROM ImpresexParticelle_Codici ")
            StrSQL.AppendLine("		    WHERE ImpresexParticelle_Codici.ID = ImpresexParticelle.ID ")
            StrSQL.AppendLine("	        AND ImpresexParticelle_Codici.Id_Cod = 5006 ), '0') AS Modifica ")

            StrSQL.AppendLine(" FROM    Centri_Aziendali INNER JOIN ")
            StrSQL.AppendLine("         ImpreseXParticelle ON Centri_Aziendali.PIVA = ImpreseXParticelle.PIVA AND Centri_Aziendali.sa_cod = ImpreseXParticelle.sa_cod INNER JOIN ")
            StrSQL.AppendLine("         ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("         ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN")
            StrSQL.AppendLine("         ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM ")

            StrSQL.AppendLine(" WHERE   (ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
            StrSQL.AppendLine(" AND   (ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.AppendLine(" AND     (ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")

            If Optional_SaCod <> 0 Then
                StrSQL.Append(" AND     (ImpreseXParticelle.sa_cod = " & Agro_SQL_SaveNum(Optional_SaCod.ToString) & ") ")
            End If

            If strFiltro <> "" Then
                StrSQL.Append(strFiltro)
            End If

            StrSQL.AppendLine(" ORDER BY ISTAT.COMUNI_PROV, ISTAT.LOCALITA, ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV, ParticelleCatastali.COM, ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO, ")
            StrSQL.AppendLine("         ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO,  ")

            StrSQL.AppendLine("         ImpreseXParticelle.Validita_Fine DESC  ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Anagrafica_Particelle_Leggi_Per_Centri(ByVal ListaCentri As List(Of (String, Integer)),
                                                       ByVal Data_Inizio As Date,
                                                       ByVal Data_Fine As Date,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Anagrafica_Particelle_Leggi_Per_Centro()"

        Dim MessaggioErrore As String
        Dim DT As New DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try
            If ListaCentri Is Nothing OrElse ListaCentri.Count = 0 Then
                Throw New Exception("La lista di PIVA e SA_COD è vuota.")
            End If

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroCentro(ListaCentri, NomeRoutine, objParametri)

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT CASE ")
            StrSQL.AppendLine("  WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN IXP.PIVA ")
            StrSQL.AppendLine("  ELSE Imprese.partitaIvaReale ")
            StrSQL.AppendLine(" END PivaReale, ")
            StrSQL.AppendLine(" IXP.PROV,    ")
            StrSQL.AppendLine(" IXP.COM,    ")
            StrSQL.AppendLine(" ISTAT.LOCALITA,    ")
            StrSQL.AppendLine(" ISTAT.COMUNI_PROV,    ")
            StrSQL.AppendLine(" ISTAT.CAP,    ")
            StrSQL.AppendLine(" IIF(IXP.SEZIONE LIKE 0, '', IXP.SEZIONE) AS SEZIONE,    ")
            StrSQL.AppendLine(" IXP.FOGLIO,     ")
            StrSQL.AppendLine(" IXP.NUMERO,    ")
            StrSQL.AppendLine(" IIf(IXP.SUBALTERNO LIKE 0, '', IXP.SUBALTERNO) AS SUBALTERNO,  ")
            StrSQL.AppendLine(" IXP.TitoloPossesso,  ")
            StrSQL.AppendLine(" '' AS TitoloPossesso_Des,  ")
            StrSQL.AppendLine(" IXP.PIVA,    ")
            StrSQL.AppendLine(" IXP.sa_cod,  ")
            StrSQL.AppendLine(" 0 AS Part_Cod,  ")
            StrSQL.AppendLine(" IXP.Validita_Inizio,  ")
            StrSQL.AppendLine(" IXP.Validita_Fine,  ")
            StrSQL.AppendLine(" IXP.Validita_Inizio AS Validita_Inizio_2,  ")
            StrSQL.AppendLine(" IXP.Validita_Fine AS Validita_Fine_2,  ")
            '            StrSQL.AppendLine(" CAST(CAST(PC.ETTARI As nvarchar(20)) + '.' + RIGHT('000' + CAST(PC.ARE AS varchar(100)), 2) + '' + RIGHT('000' + CAST(PC.CENTIARE AS varchar(100)), 2) AS float) AS Sup_Cat  ")
            StrSQL.AppendLine(" PC.ETTARI, ")
            StrSQL.AppendLine(" PC.ARE, ")
            StrSQL.AppendLine(" PC.CENTIARE ")
            StrSQL.AppendLine(" FROM ImpreseXParticelle as IXP ")
            StrSQL.AppendLine(" LEFT JOIN ImpreseXParticelle as i ON IXP.Piva = i.Piva COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" LEFT JOIN Imprese ON IXP.Piva = Imprese.Piva COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" INNER JOIN #TempCentro tmp ON tmp.Piva = IXP.Piva COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND tmp.Sa_Cod = IXP.Sa_Cod ")
            StrSQL.AppendLine(" INNER JOIN ParticelleCatastali PC ON IXP.PROV = PC.PROV AND IXP.COM = PC.COM ")
            StrSQL.AppendLine("                                AND IXP.SEZIONE = PC.SEZIONE AND IXP.FOGLIO = PC.FOGLIO ")
            StrSQL.AppendLine("                                AND IXP.NUMERO = PC.NUMERO AND IXP.SUBALTERNO = PC.SUBALTERNO ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON IXP.PROV = ISTAT.PROV AND IXP.COM = ISTAT.COM  ")
            StrSQL.AppendLine(" WHERE   (IXP.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.AppendLine(" AND     (IXP.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")
            StrSQL.AppendLine(" ORDER BY ISTAT.COMUNI_PROV, ISTAT.LOCALITA, ")
            StrSQL.AppendLine("         IXP.PROV, IXP.COM, ")
            StrSQL.AppendLine("         IXP.SEZIONE, IXP.FOGLIO, ")
            StrSQL.AppendLine("         IXP.NUMERO, IXP.SUBALTERNO,  ")
            StrSQL.AppendLine("         IXP.Validita_Fine DESC  ")

            '----------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '----------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroCentro(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

    Public Function Anagrafica_Particelle_Leggi_Per_Impianti(
                                                            ByVal Piva As String,
                                                            ByVal Optional_SaCod As Integer,
                                                            ByVal Data_Inizio As Date,
                                                            ByVal Data_Fine As Date,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Anagrafica_Particelle_Leggi_Per_Impianti()"

        Dim MessaggioErrore As String
        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  DISTINCT ")
            StrSQL.AppendLine("         0 as IndiceParticella, ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.COM + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE  + '_' + ")
            StrSQL.AppendLine("         CONVERT(varchar(10), ParticelleCatastali.FOGLIO) + '_' + ")
            StrSQL.AppendLine("         CONVERT(varchar(10), ParticelleCatastali.NUMERO) + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.SUBALTERNO AS PART_UNID, ")

            StrSQL.AppendLine("         ImpreseXParticelle.PIVA, ImpreseXParticelle.sa_cod, Centri_Aziendali.sa_nome,  ")
            StrSQL.AppendLine("         ISTAT.COMUNI_PROV, ISTAT.LOCALITA, ISNULL(ISTAT.CAP, '') CAP,  ")
            StrSQL.AppendLine("         ParticelleCatastali.Part_Cod,  ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV, ParticelleCatastali.COM, ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO,  ")
            StrSQL.AppendLine("         ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO,  ")
            StrSQL.AppendLine("         ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ParticelleCatastali.CENTIARE, ")
            StrSQL.AppendLine("         ImpreseXParticelle.Sup_Condotta, ImpreseXParticelle.Sup_Spandibile,  ImpreseXParticelle.ID, ")
            StrSQL.AppendLine("         ImpreseXParticelle.Validita_Inizio, ImpreseXParticelle.Validita_Fine, ")
            StrSQL.AppendLine("         ImpreseXParticelle.TitoloPossesso, ")
            StrSQL.AppendLine("         CASE ImpreseXParticelle.TitoloPossesso WHEN 1 THEN 'Proprietà' WHEN 2 THEN 'Comodato d''uso' ")
            StrSQL.AppendLine("                                                WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' ")
            StrSQL.AppendLine("                                                WHEN 5 THEN 'In conto terzi' WHEN 6 THEN 'In convenzione' ELSE 'Altro' END AS TitoloPossesso_Des, ")

            StrSQL.AppendLine("")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 1) ")
            StrSQL.AppendLine("    , 0) AS Sup_Seminabile,  ")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 3) ")
            StrSQL.AppendLine("    , 0) AS Sup_Unar,  ")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1  ImpresexParticelle_Codici.Val_Cod ")
            StrSQL.AppendLine("		    FROM ImpresexParticelle_Codici ")
            StrSQL.AppendLine("		    WHERE ImpresexParticelle_Codici.ID = ImpresexParticelle.ID ")
            StrSQL.AppendLine("	        AND ImpresexParticelle_Codici.Id_Cod = 5006 ), '0') AS Modifica ")

            StrSQL.AppendLine(" FROM    Centri_Aziendali INNER JOIN ")
            StrSQL.AppendLine("         ImpreseXParticelle ON Centri_Aziendali.PIVA = ImpreseXParticelle.PIVA AND Centri_Aziendali.sa_cod = ImpreseXParticelle.sa_cod INNER JOIN ")
            StrSQL.AppendLine("         ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("         ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN")
            StrSQL.AppendLine("         ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM ")

            StrSQL.AppendLine("         LEFT JOIN AppezzamentiXParticelle AxP ON  AxP.SEZIONE = ParticelleCatastali.SEZIONE And AxP.FOGLIO = ParticelleCatastali.FOGLIO And AxP.NUMERO = ParticelleCatastali.NUMERO And AxP.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            StrSQL.AppendLine(" WHERE   (ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
            StrSQL.AppendLine(" AND   (ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.AppendLine(" AND     (ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")

            If Optional_SaCod <> 0 Then
                StrSQL.Append(" AND     (ImpreseXParticelle.sa_cod = " & Agro_SQL_SaveNum(Optional_SaCod.ToString) & ") ")
            End If


            StrSQL.AppendLine(" ORDER BY ISTAT.COMUNI_PROV, ISTAT.LOCALITA, ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV, ParticelleCatastali.COM, ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO, ")
            StrSQL.AppendLine("         ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO,  ")

            StrSQL.AppendLine("         ImpreseXParticelle.Validita_Fine DESC  ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '################################################################################
    Public Function Anagrafica_Particelle_MacrousiUtilizzi(
                                                            ByVal Piva As String,
                                                            ByVal Optional_SaCod As Integer,
                                                            ByVal Data_Inizio As Date,
                                                            ByVal Data_Fine As Date,
                                                            ByVal VisualizzaMacrousi As Boolean,
                                                            ByVal VisualizzaUtilizzi As Boolean,
                                                            ByRef objParametri As AgronicaCoreParametri,
                                                            Optional ByVal strFiltro As String = "") _
                                                            As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Anagrafica_Particelle_MacrousiUtilizzi()"

        Dim MessaggioErrore As String
        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  DISTINCT ")
            StrSQL.AppendLine("         0 as IndiceParticella, ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.COM + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE  + '_' + ")
            StrSQL.AppendLine("         CONVERT(varchar(10), ParticelleCatastali.FOGLIO) + '_' + ")
            StrSQL.AppendLine("         CONVERT(varchar(10), ParticelleCatastali.NUMERO) + '_' + ")
            StrSQL.AppendLine("         ParticelleCatastali.SUBALTERNO AS PART_UNID, ")

            StrSQL.AppendLine("         ImpreseXParticelle.PIVA, ImpreseXParticelle.sa_cod, Centri_Aziendali.sa_nome,  ")
            StrSQL.AppendLine("         ISTAT.COMUNI_PROV, ISTAT.LOCALITA,  ")
            StrSQL.AppendLine("         ParticelleCatastali.Part_Cod,  ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV, ParticelleCatastali.COM, ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO,  ")
            StrSQL.AppendLine("         ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO,  ")
            StrSQL.AppendLine("         ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ParticelleCatastali.CENTIARE, ")
            StrSQL.AppendLine("         ImpreseXParticelle.Sup_Condotta, ImpreseXParticelle.Sup_Spandibile,  ImpreseXParticelle.ID, ")
            StrSQL.AppendLine("         ImpreseXParticelle.Validita_Inizio, ImpreseXParticelle.Validita_Fine, ")
            StrSQL.AppendLine("         ImpreseXParticelle.TitoloPossesso, ")
            StrSQL.AppendLine("         CASE ImpreseXParticelle.TitoloPossesso WHEN 1 THEN 'Proprietà' WHEN 2 THEN 'Comodato d''uso' ")
            StrSQL.AppendLine("                                                WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' ")
            StrSQL.AppendLine("                                                WHEN 5 THEN 'In conto terzi' WHEN 6 THEN 'In convenzione' ELSE 'Altro' END AS TitoloPossesso_Des, ")

            StrSQL.AppendLine("")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 1) ")
            StrSQL.AppendLine("    , 0) AS Sup_Seminabile,  ")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 3) ")
            StrSQL.AppendLine("    , 0) AS Sup_Unar,  ")

            StrSQL.AppendLine("ISNULL( (SELECT TOP 1  ImpresexParticelle_Codici.Val_Cod ")
            StrSQL.AppendLine("		    FROM ImpresexParticelle_Codici ")
            StrSQL.AppendLine("		    WHERE ImpresexParticelle_Codici.ID = ImpresexParticelle.ID ")
            StrSQL.AppendLine("	        AND ImpresexParticelle_Codici.Id_Cod = 5006 ), '0') AS Modifica ")


            If VisualizzaUtilizzi Then
                StrSQL.Append("  ,ISNULL(MU.Veg_Cod_Agea,'') As Cod_Utilizzo, ISNULL(MU.Cul_Cod_Agea,'') AS Cul_Cod_Agea, ")

                StrSQL.Append(" ISNULL ( ISNULL( (SELECT TOP 1  Veg_Des_Agea      ")
                StrSQL.Append(" FROM Codifica_SpecieVegetali_Agea  ")
                StrSQL.Append(" WHERE Codifica_SpecieVegetali_Agea.Veg_Cod_Agea = MU.Veg_Cod_Agea ")
                StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Cul_Cod_Agea = MU.Cul_Cod_Agea    ),  ")
                StrSQL.Append(" (SELECT TOP 1  Veg_Des_Agea        ")
                StrSQL.Append("  FROM Codifica_SpecieVegetali_Agea  ")
                StrSQL.Append("  WHERE Codifica_SpecieVegetali_Agea.Veg_Cod_Agea = MU.Veg_Cod_Agea ")
                StrSQL.Append("  AND Codifica_SpecieVegetali_Agea.Cul_Cod_Agea = '000'   )),'') AS  Veg_Des_Agea ")

                StrSQL.Append(" ,ISNULL(MU.superficie,0) AS SupUtilizzo ")

            Else
                StrSQL.Append("  ,'' As Cod_Utilizzo, '' AS Cul_Cod_Agea, '' AS Veg_Des_Agea, 0 AS SupUtilizzo ")
            End If

            If VisualizzaMacrousi Then
                StrSQL.Append("  ,ISNULL(M.macrouso_cod,'') AS Cod_Macrouso, ISNULL(Macrouso_Des,'') AS Macrouso_Des, ISNULL(M.superficie,0) AS SupMacrouso ")
            Else
                StrSQL.Append("  ,'' AS Cod_Macrouso, '' AS Macrouso_Des, 0 AS SupMacrouso ")
            End If

            StrSQL.AppendLine(" FROM    Centri_Aziendali INNER JOIN ")
            StrSQL.AppendLine("         ImpreseXParticelle ON Centri_Aziendali.PIVA = ImpreseXParticelle.PIVA AND Centri_Aziendali.sa_cod = ImpreseXParticelle.sa_cod INNER JOIN ")
            StrSQL.AppendLine("         ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND ")
            StrSQL.AppendLine("         ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ")
            StrSQL.AppendLine("         ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN")
            StrSQL.AppendLine("         ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM ")

            'If VisualizzaUtilizzi And Not VisualizzaMacrousi Then

            '    StrSQL.Append(" LEFT JOIN ParticelleCatastalixMacrousixutilizzo MU ON ImpreseXParticelle.PIVA= MU.Piva AND ")
            '    StrSQL.Append(" ImpreseXParticelle.PROV = MU.PROV And ImpreseXParticelle.COM = MU.COM And ")
            '    StrSQL.Append(" ImpreseXParticelle.SEZIONE = MU.SEZIONE And ImpreseXParticelle.FOGLIO = MU.FOGLIO And ")
            '    StrSQL.Append(" ImpreseXParticelle.NUMERO = MU.NUMERO And ImpreseXParticelle.SUBALTERNO = MU.SUBALTERNO ")
            '    StrSQL.Append(" LEFT JOIN Codifica_SpecieVegetali_Agea ON MU.Veg_Cod_Agea = Codifica_SpecieVegetali_Agea.Veg_Cod_Agea ")
            '    StrSQL.Append(" AND MU.Cul_Cod_Agea = Codifica_SpecieVegetali_Agea.Cul_Cod_Agea ")

            'Else

            If VisualizzaMacrousi Then
                StrSQL.AppendLine(" LEFT JOIN ParticelleCatastalixMacrousi M ON ImpreseXParticelle.PIVA= M.Piva AND  ")
                StrSQL.AppendLine(" ImpreseXParticelle.PROV = M.PROV And ImpreseXParticelle.COM = M.COM And ")
                StrSQL.AppendLine(" ImpreseXParticelle.SEZIONE = M.SEZIONE And ImpreseXParticelle.FOGLIO = M.FOGLIO And ")
                StrSQL.AppendLine(" ImpreseXParticelle.NUMERO = M.NUMERO And ImpreseXParticelle.SUBALTERNO = M.SUBALTERNO ")
                StrSQL.AppendLine(" LEFT JOIN Macrousi ON M.Macrouso_Cod = Macrousi.Macrouso_Cod ")
            End If

            If VisualizzaUtilizzi Then
                StrSQL.AppendLine(" LEFT JOIN ParticelleCatastalixMacrousixutilizzo MU ON M.PIVA= MU.Piva AND ")
                StrSQL.AppendLine(" m.PROV = MU.PROV And m.COM = MU.COM And ")
                StrSQL.AppendLine(" m.SEZIONE = MU.SEZIONE And m.FOGLIO = MU.FOGLIO And ")
                StrSQL.AppendLine(" m.NUMERO = MU.NUMERO And m.SUBALTERNO = MU.SUBALTERNO And m.macrouso_cod = MU.Macrouso_Cod ")
            End If

            ' End If

            StrSQL.AppendLine(" WHERE   (ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
            StrSQL.AppendLine(" AND     (ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.AppendLine(" AND     (ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")

            If VisualizzaMacrousi Then
                StrSQL.Append(" AND     (M.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " OR M.Validita_Inizio IS NULL)  ")
                StrSQL.Append(" AND     (M.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " OR M.Validita_Fine IS NULL)    ")
            End If

            If VisualizzaUtilizzi Then
                StrSQL.Append(" AND     (MU.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " OR MU.Validita_Inizio IS NULL)  ")
                StrSQL.Append(" AND     (MU.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " OR MU.Validita_Fine IS NULL)    ")
            End If

            If Optional_SaCod <> 0 Then
                StrSQL.Append(" AND     (ImpreseXParticelle.sa_cod = " & Agro_SQL_SaveNum(Optional_SaCod.ToString) & ") ")
            End If

            ' se la particella ha diversi possessi, prende il più recente nell'intervallo definito
            StrSQL.AppendLine(" AND NOT EXISTS (SELECT * FROM ImpreseXParticelle IP2 ")
            StrSQL.AppendLine(" WHERE IP2.PIVA = ImpreseXParticelle.PIVA And ")
            StrSQL.AppendLine(" IP2.sa_cod = ImpreseXParticelle.sa_cod And ")
            StrSQL.AppendLine(" IP2.Prov = ImpreseXParticelle.PROV And ")
            StrSQL.AppendLine(" IP2.Com = ImpreseXParticelle.COM And IP2.Sezione = ImpreseXParticelle.SEZIONE And ")
            StrSQL.AppendLine(" IP2.Foglio = ImpreseXParticelle.FOGLIO And IP2.Numero = ImpreseXParticelle.NUMERO And ")
            StrSQL.AppendLine(" IP2.Subalterno = ImpreseXParticelle.SUBALTERNO ")
            StrSQL.AppendLine(" AND IP2.Validita_Fine>ImpreseXParticelle.Validita_Fine ")
            StrSQL.AppendLine(" AND IP2.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.AppendLine(" AND IP2.Validita_fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " ) ")

            If strFiltro <> "" Then
                StrSQL.Append(strFiltro)
            End If

            StrSQL.AppendLine(" ORDER BY ISTAT.COMUNI_PROV, ISTAT.LOCALITA, ")
            StrSQL.AppendLine("         ParticelleCatastali.PROV, ParticelleCatastali.COM, ")
            StrSQL.AppendLine("         ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO, ")
            StrSQL.AppendLine("         ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO,  ")

            StrSQL.AppendLine("         ImpreseXParticelle.Validita_Fine DESC  ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Esiste_ClassamentoxParticella(
                                                 ByVal PROV As String,
                                                 ByVal COM As String,
                                                 ByVal SEZIONE As String,
                                                 ByVal FOGLIO As Int32,
                                                 ByVal NUMERO As Int32,
                                                 ByVal SUBALTERNO As String,
                                                 ByVal Qualita_Cod As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Esiste_ClassamentoxParticella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim bRet As Boolean = False

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If PROV = "" Then
                Throw New Exception("Parametro non corretto nella query (Provincia obbligatoria)")
            End If

            If COM = "" Then
                Throw New Exception("Parametro non corretto nella query (Comune obbligatorio)")
            End If

            If SEZIONE = "" Then
                Throw New Exception("Parametro non corretto nella query (Sezione obbligatoria)")
            End If

            If FOGLIO = 0 Then
                Throw New Exception("Parametro non corretto nella query (Foglio obbligatorio)")
            End If

            If NUMERO = 0 Then
                Throw New Exception("Parametro non corretto nella query (Numero obbligatorio)")
            End If

            If SUBALTERNO = "" Then
                Throw New Exception("Parametro non corretto nella query (Subalterno obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  ParticelleCatastaliClassamento ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.AppendLine(" AND Qualita_Cod = " & Agro_SQL_SaveNum(Qualita_Cod) & " ")
            StrSQL.AppendLine(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.AppendLine(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.AppendLine(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            StrSQL.AppendLine(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            StrSQL.AppendLine(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            StrSQL.AppendLine(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '---------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If DT IsNot Nothing Then
                If DT.Rows.Count > 0 Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing
        Return bRet

    End Function

    '##############################################################################################
    Public Function Leggi_ParticelleCatastali_Codici(
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Long,
                                ByVal NUMERO As Long,
                                ByVal SUBALTERNO As String,
                                ByVal Id_Cod As Integer,
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Leggi_ParticelleCatastali_Codici()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  ParticelleCatastali_Codici ")
                    StrSQL.AppendLine(" WHERE PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.AppendLine(" AND   COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    StrSQL.AppendLine(" AND   Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    StrSQL.AppendLine(" AND   FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    StrSQL.AppendLine(" AND   Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    StrSQL.AppendLine(" AND   SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

                    If Id_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   ID_Cod =  " & Agro_SQL_SaveNum(Id_Cod))
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
    Public Function LeggixWarning(ByVal Tabella As String,
                                  ByVal Campi As String,
                                  ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal PROV As String,
                                  ByVal COM As String,
                                  ByVal SEZIONE As String,
                                  ByVal FOGLIO As Long,
                                  ByVal NUMERO As Long,
                                  ByVal SUBALTERNO As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.LeggixWarning()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT " & Campi & " ")
            StrSQL.AppendLine(" FROM " & Tabella & " a ")

            If Campi <> "COUNT(*)" Then
                If Tabella = "CampiXParticelle" Then
                    StrSQL.Append(" LEFT JOIN Campi b ON a.Piva=b.Piva AND a.Sa_Cod=b.Sa_Cod AND a.Campo_Cod=b.Campo_Cod ")
                ElseIf Tabella = "AppezzamentiXParticelle" Then
                    StrSQL.Append(" LEFT JOIN Appezzamento b ON a.Piva=b.Piva AND a.Sa_Cod=b.Sa_Cod AND a.Appezza=b.Appezza ")
                End If
            End If

            StrSQL.AppendLine(" WHERE a.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   a.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND      a.PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.AppendLine(" AND      a.COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.AppendLine(" AND      a.Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
            StrSQL.AppendLine(" AND      a.FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.AppendLine(" AND      a.Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.AppendLine(" AND      a.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND      a.Piva        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND      a.Sa_Cod        = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function CheckParticellaPerCancellazione(ByVal piva As String,
                                                    ByVal saCod As Integer,
                                                    ByVal prov As String,
                                                    ByVal com As String,
                                                    ByVal sezione As String,
                                                    ByVal foglio As Integer,
                                                    ByVal numero As Integer,
                                                    ByVal subalterno As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As Boolean

        Return CheckParticellaPerModificaCancellazione(True,
                                                       prov,
                                                       com,
                                                       sezione,
                                                       foglio,
                                                       numero,
                                                       subalterno,
                                                       objParametri)

    End Function

    Public Function CheckParticellaPerModificaCancellazione(ByVal cancellazione As Boolean,
                                                            ByVal prov As String,
                                                            ByVal com As String,
                                                            ByVal sezione As String,
                                                            ByVal foglio As Integer,
                                                            ByVal numero As Integer,
                                                            ByVal subalterno As String,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As Boolean

        Dim listaTabelle As New List(Of String) From {
            "ws_SQNPI_Domanda_Soggetto_Catasto_Terreno",
            "ws_SQNPI_Domanda_Soggetto_Catasto_Terreno_err"
        }

        If cancellazione Then
            listaTabelle.Add("Analisi_EntitaxTestata")
            listaTabelle.Add("GIS_Entita")
        End If

        Return CheckParticella(listaTabelle,
                               prov,
                               com,
                               sezione,
                               foglio,
                               numero,
                               subalterno,
                               objParametri)

    End Function

    Public Function CheckParticella(ByVal listaTabelle As List(Of String),
                                    ByVal prov As String,
                                    ByVal com As String,
                                    ByVal sezione As String,
                                    ByVal foglio As Integer,
                                    ByVal numero As Integer,
                                    ByVal subalterno As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim esito As Boolean = True

        Dim objParticelle_R As New ParticelleCatastali_R

        For Each tabella In listaTabelle

            Dim dt = objParticelle_R.LeggixWarning(tabella,
                                                   "COUNT(*)",
                                                   "",
                                                   0,
                                                   prov,
                                                   com,
                                                   sezione,
                                                   foglio,
                                                   numero,
                                                   subalterno,
                                                   "",
                                                   "",
                                                   objParametri)

            If CInt(dt.Rows(0).Item(0)) > 0 Then
                esito = False
                Exit For
            End If

        Next

        Return esito

    End Function

    Private Sub GetQueryIntersezioneSuperficieConSquadri(
                                                        ByRef stbQuery As System.Text.StringBuilder,
                                                        ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal isBudget As Boolean,
                                                        ByVal idBudget As Integer,
                                                        ByVal campiTableName As String,
                                                        ByVal campiXParticelleTableName As String,
                                                        Optional ByVal stbSelectionAdditions As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbJoinAdditions As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbWhereAdditions As System.Text.StringBuilder = Nothing
                                                        )
        '------------------------------------------------------------
        'Ricavo la superficie di intersezione con gli SQUADRI
        '------------------------------------------------------------
        stbQuery.AppendLine("SELECT")
        stbQuery.AppendLine("    CP.PIVA")
        stbQuery.AppendLine("    , CP.SA_COD")
        stbQuery.AppendLine("    , CP.CAMPO_COD")
        stbQuery.AppendLine("    , 0 AS APPEZZA")
        stbQuery.AppendLine("    , CP.PROV")
        stbQuery.AppendLine("    , CP.COM")
        stbQuery.AppendLine("    , CP.SEZIONE")
        stbQuery.AppendLine("    , CP.FOGLIO")
        stbQuery.AppendLine("    , CP.NUMERO")
        stbQuery.AppendLine("    , CP.SUBALTERNO")
        stbQuery.AppendLine("    , CP.AREA AS AreaSuCampiSquadri")
        stbQuery.AppendLine("    , 0 AS AreaSuAppLiberi")
        stbQuery.AppendLine("    , 0 AS AreaSuAppSuCampiNonSquadri")
        stbQuery.AppendLine("    , CP.AREA")
        stbQuery.AppendLine("    , C.Validita_Inizio")
        stbQuery.AppendLine("    , C.Validita_Fine")

        If isBudget Then
            stbQuery.AppendLine("    , CP.Id_Budget")
        End If

        If stbSelectionAdditions IsNot Nothing Then
            stbQuery.Append(stbSelectionAdditions)
        End If

        stbQuery.AppendLine("")

        stbQuery.AppendLine(String.Format("FROM {0} C", campiTableName))
        stbQuery.AppendLine(String.Format("    INNER JOIN {0} CP ON C.PIVA = CP.PIVA", campiXParticelleTableName))
        stbQuery.AppendLine("        AND C.SA_COD = CP.SA_COD")
        stbQuery.AppendLine("        AND C.Campo_Cod = CP.Campo_Cod")

        If isBudget Then
            stbQuery.AppendLine("        AND C.Id_Budget = CP.Id_Budget")
        End If

        If stbJoinAdditions IsNot Nothing Then
            stbQuery.Append(stbJoinAdditions)
        End If

        stbQuery.AppendLine("")
        stbQuery.AppendLine(String.Format("WHERE C.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
        stbQuery.AppendLine(String.Format("    AND C.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))

        If isBudget Then
            stbQuery.AppendLine(String.Format("    AND C.Id_Budget = {0}", Agro_SQL_SaveNum(idBudget)))
        End If

        If stbWhereAdditions IsNot Nothing Then
            stbQuery.Append(stbWhereAdditions)
        End If

    End Sub

    Private Sub GetQueryIntersezioneSuperficieConAppezzamentiInCampiNonSquadri(
                                                                              ByRef stbQuery As System.Text.StringBuilder,
                                                                              ByVal Piva As String,
                                                                              ByVal Sa_Cod As Integer,
                                                                              ByVal isBudget As Boolean,
                                                                              ByVal idBudget As Integer,
                                                                              ByVal appezzamentoTableName As String,
                                                                              ByVal appezzamentiXParticelleTableName As String,
                                                                              ByVal campiXParticelleTableName As String,
                                                                              Optional ByVal stbSelectionAdditions As System.Text.StringBuilder = Nothing,
                                                                              Optional ByVal stbJoinAdditions As System.Text.StringBuilder = Nothing,
                                                                              Optional ByVal stbWhereAdditions As System.Text.StringBuilder = Nothing
                                                                              )

        '--------------------------------------------------------------------------------------------
        'Ricavo la superficie di intersezione con gli app. in campi NON SQUADRI (campi senza catasto)
        '--------------------------------------------------------------------------------------------
        stbQuery.AppendLine("SELECT")
        stbQuery.AppendLine("    AP.PIVA")
        stbQuery.AppendLine("    , AP.SA_COD")
        stbQuery.AppendLine("    , 0 AS CAMPO_COD")
        stbQuery.AppendLine("    , AP.APPEZZA")
        stbQuery.AppendLine("    , AP.PROV")
        stbQuery.AppendLine("    , AP.COM")
        stbQuery.AppendLine("    , AP.SEZIONE")
        stbQuery.AppendLine("    , AP.FOGLIO")
        stbQuery.AppendLine("    , AP.NUMERO")
        stbQuery.AppendLine("    , AP.SUBALTERNO")
        stbQuery.AppendLine("    , 0 AS AreaSuCampiSquadri")
        stbQuery.AppendLine("    , 0 AS AreaSuAppLiberi")
        stbQuery.AppendLine("    , AP.AREA AS AreaSuAppSuCampiNonSquadri")
        stbQuery.AppendLine("    , AP.AREA")
        stbQuery.AppendLine("    , A.Validita_Inizio")
        stbQuery.AppendLine("    , A.Validita_Fine")

        If isBudget Then
            stbQuery.AppendLine("    , AP.Id_Budget")
        End If

        If stbSelectionAdditions IsNot Nothing Then
            stbQuery.Append(stbSelectionAdditions)
        End If

        stbQuery.AppendLine("")

        stbQuery.AppendLine(String.Format("FROM {0} A", appezzamentoTableName))
        stbQuery.AppendLine(String.Format("    INNER JOIN {0} AP ON A.PIVA = AP.PIVA", appezzamentiXParticelleTableName))
        stbQuery.AppendLine("        AND A.SA_COD = AP.SA_COD")
        stbQuery.AppendLine("        AND A.APPEZZA = AP.APPEZZA")

        If isBudget Then
            stbQuery.AppendLine("        AND A.Id_Budget = AP.Id_Budget")
        End If

        If stbJoinAdditions IsNot Nothing Then
            stbQuery.Append(stbJoinAdditions)
        End If

        stbQuery.AppendLine("")
        stbQuery.AppendLine(String.Format("WHERE A.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
        stbQuery.AppendLine(String.Format("    AND A.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))
        stbQuery.AppendLine("    AND A.CAMPO_COD <> 0")
        stbQuery.AppendLine("    AND NOT EXISTS (")
        stbQuery.AppendLine("        SELECT CP.Piva, CP.Sa_Cod, CP.campo_cod")
        stbQuery.AppendLine(String.Format("        FROM {0} CP", campiXParticelleTableName))
        stbQuery.AppendLine("        WHERE CP.piva= A.piva")
        stbQuery.AppendLine("            AND CP.sa_cod = A.sa_cod")
        stbQuery.AppendLine("            AND CP.campo_cod = A.campo_cod")

        If isBudget Then
            stbQuery.AppendLine(String.Format("            AND CP.Id_Budget = {0}", Agro_SQL_SaveNum(idBudget)))
        End If

        stbQuery.AppendLine("    ) ")

        If isBudget Then
            stbQuery.AppendLine(String.Format("    AND A.Id_Budget = {0}", Agro_SQL_SaveNum(idBudget)))
        End If

        If stbWhereAdditions IsNot Nothing Then
            stbQuery.Append(stbWhereAdditions)
        End If

    End Sub

    Private Sub GetQueryIntersezioneSuperficieConAppezzamentiLiberi(
                                                                   ByRef stbQuery As System.Text.StringBuilder,
                                                                   ByVal Piva As String,
                                                                   ByVal Sa_Cod As Integer,
                                                                   ByVal isBudget As Boolean,
                                                                   ByVal idBudget As Integer,
                                                                   ByVal appezzamentoTableName As String,
                                                                   ByVal appezzamentiXParticelleTableName As String,
                                                                   Optional ByVal stbSelectionAdditions As System.Text.StringBuilder = Nothing,
                                                                   Optional ByVal stbJoinAdditions As System.Text.StringBuilder = Nothing,
                                                                   Optional ByVal stbWhereAdditions As System.Text.StringBuilder = Nothing
                                                                   )

        '------------------------------------------------------------
        'Ricavo la superficie di intersezione con gli app. LIBERI
        '------------------------------------------------------------
        stbQuery.AppendLine("SELECT")
        stbQuery.AppendLine("    AP.PIVA")
        stbQuery.AppendLine("    , AP.SA_COD")
        stbQuery.AppendLine("    , 0 AS CAMPO_COD")
        stbQuery.AppendLine("    , AP.APPEZZA")
        stbQuery.AppendLine("    , AP.PROV")
        stbQuery.AppendLine("    , AP.COM")
        stbQuery.AppendLine("    , AP.SEZIONE")
        stbQuery.AppendLine("    , AP.FOGLIO")
        stbQuery.AppendLine("    , AP.NUMERO")
        stbQuery.AppendLine("    , AP.SUBALTERNO")
        stbQuery.AppendLine("    , 0 AS AreaSuCampiSquadri")
        stbQuery.AppendLine("    , AP.AREA AS AreaSuAppLiberi")
        stbQuery.AppendLine("    , 0 AS AreaSuAppSuCampiNonSquadri")
        stbQuery.AppendLine("    , AP.AREA")
        stbQuery.AppendLine("    , A.Validita_Inizio")
        stbQuery.AppendLine("    , A.Validita_Fine")

        If isBudget Then
            stbQuery.AppendLine("    , AP.Id_Budget")
        End If

        If stbSelectionAdditions IsNot Nothing Then
            stbQuery.Append(stbSelectionAdditions)
        End If

        stbQuery.AppendLine("")
        stbQuery.AppendLine(String.Format("FROM {0} A", appezzamentoTableName))
        stbQuery.AppendLine(String.Format("    INNER JOIN {0} AP ON A.PIVA = AP.PIVA", appezzamentiXParticelleTableName))
        stbQuery.AppendLine("        AND A.SA_COD = AP.SA_COD")
        stbQuery.AppendLine("        AND A.APPEZZA = AP.APPEZZA")

        If isBudget Then
            stbQuery.AppendLine("        AND A.Id_Budget = AP.Id_Budget")
        End If

        If stbJoinAdditions IsNot Nothing Then
            stbQuery.Append(stbJoinAdditions)
        End If

        stbQuery.AppendLine("")
        stbQuery.AppendLine(String.Format("WHERE A.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
        stbQuery.AppendLine(String.Format("    AND A.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))
        stbQuery.AppendLine("    AND A.CAMPO_COD = 0")

        If isBudget Then
            stbQuery.Append(String.Format("        AND A.Id_Budget = {0}", Agro_SQL_SaveNum(idBudget)))
        End If

        If stbWhereAdditions IsNot Nothing Then
            stbQuery.Append(stbWhereAdditions)
        End If

    End Sub

    Private Sub GetQueryIntersezioneSuperficieParticelle(
                                                        ByRef stbQuery As System.Text.StringBuilder,
                                                        ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal isBudget As Boolean,
                                                        ByVal idBudget As Integer,
                                                        ByVal campiTableName As String,
                                                        ByVal campiXParticelleTableName As String,
                                                        ByVal appezzamentoTableName As String,
                                                        ByVal appezzamentiXParticelleTableName As String,
                                                        Optional ByVal stbSelectionAdditionsQuadri As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbJoinAdditionsQuadri As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbWhereAdditionsQuadri As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbSelectionAdditionsLiberi As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbJoinAdditionsLiberi As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbWhereAdditionsLiberi As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbSelectionAdditionsNonQuadri As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbJoinAdditionsNonQuadri As System.Text.StringBuilder = Nothing,
                                                        Optional ByVal stbWhereAdditionsNonQuadri As System.Text.StringBuilder = Nothing
                                                        )

        UtilityParticelle.GetQueryTablesAlias(stbQuery, isBudget, campiTableName, campiXParticelleTableName, appezzamentoTableName, appezzamentiXParticelleTableName)

        '------------------------------------------------------------
        'Ricavo la superficie di intersezione con gli SQUADRI
        '------------------------------------------------------------
        Me.GetQueryIntersezioneSuperficieConSquadri(
            stbQuery,
            Piva,
            Sa_Cod,
            isBudget,
            idBudget,
            campiTableName,
            campiXParticelleTableName,
            stbSelectionAdditions:=stbSelectionAdditionsQuadri,
            stbJoinAdditions:=stbJoinAdditionsQuadri,
            stbWhereAdditions:=stbWhereAdditionsQuadri
            )

        stbQuery.AppendLine("")
        stbQuery.AppendLine("UNION")
        stbQuery.AppendLine("")

        '------------------------------------------------------------
        'Ricavo la superficie di intersezione con gli app. LIBERI
        '------------------------------------------------------------
        Me.GetQueryIntersezioneSuperficieConAppezzamentiLiberi(
            stbQuery,
            Piva,
            Sa_Cod,
            isBudget,
            idBudget,
            appezzamentoTableName,
            appezzamentiXParticelleTableName,
            stbSelectionAdditions:=stbSelectionAdditionsLiberi,
            stbJoinAdditions:=stbJoinAdditionsLiberi,
            stbWhereAdditions:=stbWhereAdditionsLiberi
            )

        stbQuery.AppendLine("")
        stbQuery.AppendLine("UNION")
        stbQuery.AppendLine("")

        '-----------------------------------------------------------------------
        'Ricavo la superficie di intersezione con gli app. in campi NON SQUADRI
        '-----------------------------------------------------------------------
        Me.GetQueryIntersezioneSuperficieConAppezzamentiInCampiNonSquadri(
            stbQuery,
            Piva,
            Sa_Cod,
            isBudget,
            idBudget,
            appezzamentoTableName,
            appezzamentiXParticelleTableName,
            campiXParticelleTableName,
            stbSelectionAdditions:=stbSelectionAdditionsNonQuadri,
            stbJoinAdditions:=stbJoinAdditionsNonQuadri,
            stbWhereAdditions:=stbWhereAdditionsNonQuadri
            )

    End Sub

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class ParticelleCatastali_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function AggiornaParticelleDaGIS(
                                ByVal id_importazioneComeAgenda As Integer,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Long,
                                ByVal NUMERO As Long,
                                ByVal SUBALTERNO As String,
                                    ByRef objParametri As AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = ""
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.InserisciParticelleDaGIS()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim rval As Boolean

        Try
            '---------------------------------------------


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



            Stb.Length = 0

            Stb.Append(" declare @Conversione int " & vbCrLf)
            Stb.Append(" set @Conversione = 10000 " & vbCrLf)
            Stb.Append(" update p set ETTARI = cast(g.poligono_Geoentity.STArea()/@Conversione as int), ARE = substring(cast(abs(cast(g.poligono_Geoentity.STArea()/@Conversione as int) - g.poligono_Geoentity.STArea()/@Conversione) as varchar(100)), 3,2), CENTIARE=substring(cast(abs(cast(g.poligono_Geoentity.STArea()/@Conversione as int) - g.poligono_Geoentity.STArea()/@Conversione) as varchar(100)), 5,2), Data_Modifica = " & Agro_SQL_SaveDate(Data_modifica) & ",  Username_Modifica='" & Agro_SQL_SaveText(username_modifica) & "'  " & vbCrLf)

            Stb.Append(" from gis_entita Entita " & vbCrLf)
            Stb.Append("    inner join GIS_ElementiGrafici g  " & vbCrLf)
            Stb.Append("        on Entita.Entita_Cod = g.Entita_cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join  ParticelleCatastali  p " & vbCrLf)
            Stb.Append("        on p.prov = Entita.prov " & vbCrLf)
            Stb.Append("        and p.com = Entita.COM  " & vbCrLf)
            Stb.Append("        and p.SEZIONE = Entita.SEZIONE  " & vbCrLf)
            Stb.Append("        and p.Foglio = Entita.Foglio " & vbCrLf)
            Stb.Append("        and p.NUMERO = Entita.NUMERO  " & vbCrLf)
            Stb.Append("        and p.SUBALTERNO = Entita.SUBALTERNO  " & vbCrLf)
            Stb.Append("        and Entita.inviato = " & id_importazioneComeAgenda & vbCrLf)

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return rval

    End Function

    '##############################################################################################
    Public Function InserisciParticelleDaGIS(
                                ByVal id_importazioneComeAgenda As Integer,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Long,
                                ByVal NUMERO As Long,
                                ByVal SUBALTERNO As String,
                                    ByRef objParametri As AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = ""
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.InserisciParticelleDaGIS()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim rval As Boolean

        Try
            '---------------------------------------------


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



            Stb.Length = 0

            Stb.Append(" declare @Conversione int " & vbCrLf)
            Stb.Append(" set @Conversione = 10000 " & vbCrLf)
            Stb.Append(" insert ParticelleCatastali (PART_COD, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, PARTITA_CATASTALE, ETTARI, ARE, CENTIARE, TitoloPossesso, QUALITA_COD, CLASSE, REDDITO_DOMINICALE, REDDITO_AGRARIO, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, Validazione, Data_Validazione, UserName_Validazione) " & vbCrLf)
            Stb.Append(" select   " & vbCrLf)
            'Lavez - 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                Stb.Append("    dense_rank() over (order by Entita.entita_cod) + cast(isnull( (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_particelleCatastali'), 0) as int)  as PART_COD   " & vbCrLf)
            Else
                Stb.Append("    dense_rank() over (order by Entita.entita_cod) + isnull( (select max(PART_COD) from particelleCatastali), 0)  as PART_COD   " & vbCrLf)
            End If

            Stb.Append("    , Entita.prov as gis_prov " & vbCrLf)
            Stb.Append("    ,Entita.COM  as gis_COM " & vbCrLf)
            Stb.Append("    ,Entita.SEZIONE  as gis_SEZIONE " & vbCrLf)
            Stb.Append("    ,Entita.FOGLIO  as gis_FOGLIO " & vbCrLf)
            Stb.Append("    ,Entita.NUMERO  as gis_NUMERO " & vbCrLf)
            Stb.Append("    ,Entita.SUBALTERNO as gis_SUBALTERNO " & vbCrLf)
            Stb.Append("    ,'' as Partita_Catastale " & vbCrLf)
            Stb.Append("    ,cast(round(g.poligono_Geoentity.STArea()/@Conversione,4) as int) as Ettari " & vbCrLf)
            Stb.Append("    ,substring(cast(abs(cast(round(g.poligono_Geoentity.STArea()/@Conversione,4) as int) - round(g.poligono_Geoentity.STArea()/@Conversione,4)) as varchar(100)), 3,2) as Are " & vbCrLf)
            Stb.Append("    ,substring(cast(abs(cast(round(g.poligono_Geoentity.STArea()/@Conversione,4) as int) - round(g.poligono_Geoentity.STArea()/@Conversione,4)) as varchar(100)), 5,2) as Centiare " & vbCrLf)
            Stb.Append("    ,1 as TitoloPossesso " & vbCrLf)
            Stb.Append("    ,0 as Qualita_cod " & vbCrLf)
            Stb.Append("    ,'' as CLASSE " & vbCrLf)
            Stb.Append("    ,0 as Reddito_Dominicale " & vbCrLf)
            Stb.Append("    ,0 as Reddito_Agrario" & vbCrLf)

            Stb.Append("         , 0  ")
            Stb.Append("         , Null  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            Stb.Append("         , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            'validazione
            Stb.Append("         , 0  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")

            Stb.Append(" from gis_entita Entita " & vbCrLf)
            Stb.Append("    inner join GIS_ElementiGrafici g  " & vbCrLf)
            Stb.Append("        on Entita.Entita_Cod = g.Entita_cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    where Entita.inviato = " & id_importazioneComeAgenda & " and not exists ( select 1 from ParticelleCatastali  p " & vbCrLf)
            Stb.Append("        where p.prov = Entita.prov " & vbCrLf)
            Stb.Append("        and p.com = Entita.COM  " & vbCrLf)
            Stb.Append("        and p.SEZIONE = Entita.SEZIONE  " & vbCrLf)
            Stb.Append("        and p.Foglio = Entita.Foglio " & vbCrLf)
            Stb.Append("        and p.NUMERO = Entita.NUMERO  " & vbCrLf)
            Stb.Append("        and p.SUBALTERNO = Entita.SUBALTERNO ) " & vbCrLf)



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Entita.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Entita.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return rval

    End Function

    '==========================================================================================
    'Public Function Scrivi(ByVal Part_Cod As Int32, _
    '                       ByVal PROV As String, _
    '                       ByVal COM As String, _
    '                       ByVal SEZIONE As String, _
    '                       ByVal FOGLIO As Int32, _
    '                       ByVal NUMERO As Int32, _
    '                       ByVal SUBALTERNO As String, _
    '                       ByVal Partita_Catastale As String, _
    '                       ByVal ETTARI As Decimal, _
    '                       ByVal ARE As Int32, _
    '                       ByVal CENTIARE As Int32, _
    '                       ByVal QUALITA_COD As Int32, _
    '                       ByVal CLASSE As String, _
    '                       ByVal REDDITO_DOMINICALE As Decimal, _
    '                       ByVal REDDITO_AGRARIO As Decimal, _
    '                       ByVal TitoloPossesso As Int32, _
    '                       ByVal UserName_Creazione As String, _
    '                       ByVal FinestraTemp_Inizio As Date, _
    '                       ByVal FinestraTemp_Fine As Date, _
    '                       ByRef objConnessione As DbConnection, _
    '                       ByRef objTransazione As DbTransaction, _
    '                       ByVal StringaConnessione As String, _
    '                       ByVal DirectoryLOG As String, _
    '                       ByVal FileLOG As String, _
    '                       ByVal IdentificatoreUtente As String _
    '                       ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("INSERT INTO ParticelleCatastali(       ")
    '        StrSQL.Append("             Part_Cod,           PROV,           COM,        SEZIONE, ")
    '        StrSQL.Append("             FOGLIO,             NUMERO,         SUBALTERNO, Partita_Catastale,      ETTARI,         ARE, ")
    '        StrSQL.Append("             CENTIARE,           QUALITA_COD,    CLASSE,     REDDITO_DOMINICALE, ")
    '        StrSQL.Append("             REDDITO_AGRARIO,    TitoloPossesso, ")

    '        StrSQL.Append("             Inviato,            DataInvio, ")
    '        StrSQL.Append("             Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("             UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("             Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("             ) ")

    '        StrSQL.Append(" VALUES (")
    '        StrSQL.Append("           " & Agro_SQL_SaveNum(Part_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
    '        StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE,false) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
    '        StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO,false) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
    '        StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO,false) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
    '        StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO,false) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Partita_Catastale)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(ETTARI) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(ARE) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(CENTIARE) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(QUALITA_COD) & "  ")

    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(CLASSE)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(REDDITO_DOMINICALE) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(REDDITO_AGRARIO) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(TitoloPossesso))

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")

    '        StrSQL.Append(") ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Scrivi(ByVal Part_Cod As Int32,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Int32,
                           ByVal NUMERO As Int32,
                           ByVal SUBALTERNO As String,
                           ByVal Partita_Catastale As String,
                           ByVal ETTARI As Decimal,
                           ByVal ARE As Int32,
                           ByVal CENTIARE As Int32,
                           ByVal QUALITA_COD As Int32,
                           ByVal CLASSE As String,
                           ByVal REDDITO_DOMINICALE As Decimal,
                           ByVal REDDITO_AGRARIO As Decimal,
                           ByVal TitoloPossesso As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

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

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO ParticelleCatastali(       ")
            StrSQL.Append("             Part_Cod,           PROV,           COM,        SEZIONE, ")
            StrSQL.Append("             FOGLIO,             NUMERO,         SUBALTERNO, Partita_Catastale,      ETTARI,         ARE, ")
            StrSQL.Append("             CENTIARE,           QUALITA_COD,    CLASSE,     REDDITO_DOMINICALE, ")
            StrSQL.Append("             REDDITO_AGRARIO,    TitoloPossesso, ")

            StrSQL.Append("             Inviato,            DataInvio, ")
            StrSQL.Append("             Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("             UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("             Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("             ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Partita_Catastale)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ETTARI) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ARE) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(CENTIARE) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(QUALITA_COD) & "  ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(CLASSE)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(REDDITO_DOMINICALE) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(REDDITO_AGRARIO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(TitoloPossesso))

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    'Public Function Modifica_2(ByVal PROV As String, _
    '                           ByVal COM As String, _
    '                           ByVal SEZIONE As String, _
    '                           ByVal FOGLIO As Int32, _
    '                           ByVal NUMERO As Int32, _
    '                           ByVal SUBALTERNO As String, _
    '                           ByVal Partita_Catastale As String, _
    '                           ByVal ETTARI As Decimal, _
    '                           ByVal ARE As Int32, _
    '                           ByVal CENTIARE As Int32, _
    '                           ByVal QUALITA_COD As Int32, _
    '                           ByVal CLASSE As String, _
    '                           ByVal REDDITO_DOMINICALE As Decimal, _
    '                           ByVal REDDITO_AGRARIO As Decimal, _
    '                           ByVal TitoloPossesso As Int32, _
    '                           ByVal UserName_Modifica As String, _
    '                           ByVal FinestraTemp_Inizio As Date, _
    '                           ByVal FinestraTemp_Fine As Date, _
    '                           ByRef objConnessione As DbConnection, _
    '                           ByRef objTransazione As DbTransaction, _
    '                           ByVal StringaConnessione As String, _
    '                           ByVal DirectoryLOG As String, _
    '                           ByVal FileLOG As String, _
    '                           ByVal IdentificatoreUtente As String _
    '                           ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Modifica_2()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE ParticelleCatastali SET ")
    '        StrSQL.Append("        ETTARI             =  " & Agro_SQL_SaveNum(ETTARI) & "  ")
    '        StrSQL.Append("       ,ARE                =  " & Agro_SQL_SaveNum(ARE) & "  ")
    '        StrSQL.Append("       ,CENTIARE           =  " & Agro_SQL_SaveNum(CENTIARE) & "  ")
    '        StrSQL.Append("       ,QUALITA_COD        =  " & Agro_SQL_SaveNum(QUALITA_COD) & "  ")
    '        StrSQL.Append("       ,CLASSE             = '" & Agro_SQL_SaveText(CLASSE) & "' ")
    '        StrSQL.Append("       ,Partita_Catastale  = '" & Agro_SQL_SaveText(Partita_Catastale) & "' ")
    '        StrSQL.Append("       ,REDDITO_DOMINICALE =  " & Agro_SQL_SaveNum(REDDITO_DOMINICALE) & "  ")
    '        StrSQL.Append("       ,REDDITO_AGRARIO    =  " & Agro_SQL_SaveNum(REDDITO_AGRARIO) & "  ")
    '        StrSQL.Append("       ,TitoloPossesso     =  " & Agro_SQL_SaveNum(TitoloPossesso) & " ")
    '        StrSQL.Append("       ,Inviato            =  0 ")
    '        StrSQL.Append("       ,DataInvio          =  Null ")
    '        StrSQL.Append("       ,Data_Modifica      =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("       ,UserName_Modifica  = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("       ,Validita_Inizio    =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("       ,Validita_Fine      =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE    PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '        StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '        StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE,false) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
    '        StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO,false) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
    '        StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO,false) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
    '        StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO,false) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Modifica_2(ByVal PROV As String,
                               ByVal COM As String,
                               ByVal SEZIONE As String,
                               ByVal FOGLIO As Int32,
                               ByVal NUMERO As Int32,
                               ByVal SUBALTERNO As String,
                               ByVal Partita_Catastale As String,
                               ByVal ETTARI As Decimal,
                               ByVal ARE As Int32,
                               ByVal CENTIARE As Int32,
                               ByVal QUALITA_COD As Int32,
                               ByVal CLASSE As String,
                               ByVal REDDITO_DOMINICALE As Decimal,
                               ByVal REDDITO_AGRARIO As Decimal,
                               ByVal TitoloPossesso As Int32,
                               ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Modifica_2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE ParticelleCatastali SET ")
            StrSQL.Append("        ETTARI             =  " & Agro_SQL_SaveNum(ETTARI) & "  ")
            StrSQL.Append("       ,ARE                =  " & Agro_SQL_SaveNum(ARE) & "  ")
            StrSQL.Append("       ,CENTIARE           =  " & Agro_SQL_SaveNum(CENTIARE) & "  ")
            StrSQL.Append("       ,QUALITA_COD        =  " & Agro_SQL_SaveNum(QUALITA_COD) & "  ")
            StrSQL.Append("       ,CLASSE             = '" & Agro_SQL_SaveText(CLASSE) & "' ")
            StrSQL.Append("       ,Partita_Catastale  = '" & Agro_SQL_SaveText(Partita_Catastale) & "' ")
            StrSQL.Append("       ,REDDITO_DOMINICALE =  " & Agro_SQL_SaveNum(REDDITO_DOMINICALE) & "  ")
            StrSQL.Append("       ,REDDITO_AGRARIO    =  " & Agro_SQL_SaveNum(REDDITO_AGRARIO) & "  ")
            StrSQL.Append("       ,TitoloPossesso     =  " & Agro_SQL_SaveNum(TitoloPossesso) & " ")
            StrSQL.Append("       ,Inviato            =  0 ")
            StrSQL.Append("       ,DataInvio          =  Null ")
            StrSQL.Append("       ,Data_Modifica      =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio    =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("       ,Validita_Fine      =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE    PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica_SuperficieParticella(
                           ByVal Old_PROV As String,
                           ByVal Old_COM As String,
                           ByVal Old_SEZIONE As String,
                           ByVal Old_FOGLIO As Int32,
                           ByVal Old_NUMERO As Int32,
                           ByVal Old_SUBALTERNO As String,
                           ByVal New_ETTARI As Decimal,
                           ByVal New_ARE As Int32,
                           ByVal New_CENTIARE As Int32,
                           ByVal xFiltroAggiuntivo As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Modifica_SuperficieParticella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE ParticelleCatastali SET ")
            StrSQL.Append("        ETTARI             =  " & Agro_SQL_SaveNum(New_ETTARI) & "  ")
            StrSQL.Append("       ,ARE                =  " & Agro_SQL_SaveNum(New_ARE) & "  ")
            StrSQL.Append("       ,CENTIARE           =  " & Agro_SQL_SaveNum(New_CENTIARE) & "  ")
            StrSQL.Append("       ,Data_Modifica      =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE    PROV        = '" & Agro_SQL_SaveText(Trim(Old_PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(Old_COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(Old_SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(Old_SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(Old_FOGLIO, False) <> 0, Agro_SQL_SaveNum(Old_FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(Old_NUMERO, False) <> 0, Agro_SQL_SaveNum(Old_NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(Old_SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(Old_SUBALTERNO)), 0) & "' ")

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    'Cancellazione utilizzando il Part_Cod
    '==========================================================================================
    'Public Function Cancella_1( _
    '                        ByVal Part_Cod As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FlagCancellazioneLogica As Int32, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Cancella_1()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try


    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE ParticelleCatastali ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
    '            StrSQL.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("         ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE   PART_COD       =  " & Agro_SQL_SaveNum(Part_Cod) & "  ")
    '            StrSQL.Append(" AND     Inviato >= 0")

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM    ParticelleCatastali ")
    '            StrSQL.Append(" WHERE   PART_COD       =  " & Agro_SQL_SaveNum(Part_Cod) & "  ")

    '        End If

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Cancella_1(ByVal Part_Cod As Int32,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Cancella_1()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE ParticelleCatastali ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
                StrSQL.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   PART_COD       =  " & Agro_SQL_SaveNum(Part_Cod) & "  ")
                StrSQL.Append(" AND     Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    ParticelleCatastali ")
                StrSQL.Append(" WHERE   PART_COD       =  " & Agro_SQL_SaveNum(Part_Cod) & "  ")

            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

    'Cancellazione utilizzando la sestupla
    '==========================================================================================
    'Public Function Cancella_2( _
    '                        ByVal PROV As String, _
    '                        ByVal COM As String, _
    '                        ByVal SEZIONE As String, _
    '                        ByVal FOGLIO As Int32, _
    '                        ByVal NUMERO As Int32, _
    '                        ByVal SUBALTERNO As String, _
    '                            ByVal UserName_Modifica As String, _
    '                            ByVal FlagCancellazioneLogica As Int32, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByRef objTransazione As DbTransaction, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String) _
    '                                As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Cancella_2()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try


    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE ParticelleCatastali ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
    '            StrSQL.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("         ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE   Inviato >= 0")

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM    ParticelleCatastali ")
    '            StrSQL.Append(" WHERE   1=1")

    '        End If

    '        ' La clausula è la stessa per entrambe le query
    '        StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '        StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '        StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE,false) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
    '        StrSQL.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO,false) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
    '        StrSQL.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO,false) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
    '        StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO,false) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Cancella_2(ByVal PROV As String,
                               ByVal COM As String,
                               ByVal SEZIONE As String,
                               ByVal FOGLIO As Int32,
                               ByVal NUMERO As Int32,
                               ByVal SUBALTERNO As String,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Cancella_2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE ParticelleCatastali ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
                StrSQL.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    ParticelleCatastali ")
                StrSQL.Append(" WHERE   1=1")

            End If

            ' La clausula è la stessa per entrambe le query
            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

    '==========================================================================================
    Public Function Cancella_ParticelleCatastaliClassamento(
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Long,
                                    ByVal NUMERO As Long,
                                    ByVal SUBALTERNO As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Cancella_ParticelleCatastaliClassamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE   ParticelleCatastaliClassamento ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("31/12/1899")) & " ")
                StrSQL.Append("         ,Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1")

            Else

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     ParticelleCatastaliClassamento ")
                StrSQL.Append(" WHERE   1=1")

            End If

            StrSQL.Append(" And      PROV           = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM            = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione          = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(SEZIONE), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO           = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero           = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO       = '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(SUBALTERNO), 0) & "' ")

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

    '==========================================================================================
    Public Function Scrivi_ParticelleCatastaliClassamento(ByVal PROV As String,
                       ByVal COM As String,
                       ByVal SEZIONE As String,
                       ByVal FOGLIO As Long,
                       ByVal NUMERO As Long,
                       ByVal SUBALTERNO As String,
                       ByVal QUALITA_COD As Long,
                       ByVal Porzione As String,
                       ByVal CLASSE As String,
                       ByVal Sup_Classe As Decimal,
                       ByVal REDDITO_DOMINICALE As Decimal,
                       ByVal REDDITO_AGRARIO As Decimal,
                       ByVal Deduzione As String,
                       ByVal Validita_Inizio As Date,
                       ByVal Validita_Fine As Date,
                                      ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Scrivi_ParticelleCatastaliClassamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO ParticelleCatastaliClassamento(       " &
                            "                    PROV, COM, SEZIONE, " &
                            "                    FOGLIO, NUMERO, SUBALTERNO, QUALITA_COD, Porzione, CLASSE, Sup_Classe,  " &
                            "                    REDDITO_DOMINICALE, REDDITO_AGRARIO, Deduzione, Inviato, DataInvio, " &
                            "                    Data_Creazione,     Data_Modifica, " &
                            "                    UserName_Creazione, UserName_Modifica, " &
                            "                    Validita_Inizio,    Validita_Fine " &
                            "                    ) " &
                            "VALUES (" &
                            "         '" & Agro_SQL_SaveText(Trim(PROV)) & "'  " &
                            "         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  " &
                            "         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  " &
                            "         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  " &
                            "         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  " &
                            "         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' " &
                            "         , " & Agro_SQL_SaveNum(QUALITA_COD) & "  ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Porzione)) & "' " &
                  "         ,'" & Agro_SQL_SaveText(Trim(CLASSE)) & "' " &
                  "         , " & Agro_SQL_SaveNum(Sup_Classe) & "  " &
                  "         , " & Agro_SQL_SaveNum(REDDITO_DOMINICALE) & "  " &
                  "         , " & Agro_SQL_SaveNum(REDDITO_AGRARIO) & "  " &
                  "         ,'" & Agro_SQL_SaveText(Trim(Deduzione)) & "' " &
                  "         , 0  " &
                  "         , Null  " &
                  "         , " & Agro_SQL_SaveDate(Now) & "  " &
                  "         , " & Agro_SQL_SaveDate(Now) & "  " &
                  "         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " &
                  "         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " &
                  "         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " &
                  "         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " &
                  ")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '============================================================================
    Public Function Modifica_ParticelleCatastaliClassamento(
                            ByVal Qualita_Cod As Integer,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Integer,
                            ByVal NUMERO As Integer,
                            ByVal SUBALTERNO As String,
                            ByVal Porzione As String,
                            ByVal Classe As String,
                            ByVal Sup_Classe As Decimal,
                            ByVal REDDITO_DOMINICALE As Decimal,
                            ByVal REDDITO_AGRARIO As Decimal,
                            ByVal Deduzione As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ParticelleCatastali_W.Modifica_ParticelleCatastaliClassamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Qualita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Qualita_Cod obbligatorio)")
            End If

            If PROV = "" Then
                Throw New Exception("Parametro non corretto nella query (PROV obbligatorio)")
            End If

            If COM = "" Then
                Throw New Exception("Parametro non corretto nella query (COM obbligatorio)")
            End If

            If SEZIONE = "" Then
                Throw New Exception("Parametro non corretto nella query (SEZIONE obbligatorio)")
            End If

            If FOGLIO = 0 Then
                Throw New Exception("Parametro non corretto nella query (FOGLIO obbligatorio)")
            End If

            If NUMERO = 0 Then
                Throw New Exception("Parametro non corretto nella query (NUMERO obbligatorio)")
            End If

            If SUBALTERNO = "" Then
                Throw New Exception("Parametro non corretto nella query (SUBALTERNO obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE ParticelleCatastaliClassamento SET ")

            StrSQL.Append("   Porzione              =  '" & Agro_SQL_SaveText(Porzione) & "' ")
            StrSQL.Append("   ,Classe              =  '" & Agro_SQL_SaveText(Classe) & "' ")
            StrSQL.Append("   ,Sup_Classe              =  " & Agro_SQL_SaveNum(Sup_Classe) & " ")
            StrSQL.Append("   ,REDDITO_DOMINICALE              =  " & Agro_SQL_SaveNum(REDDITO_DOMINICALE) & " ")
            StrSQL.Append("   ,REDDITO_AGRARIO              =  " & Agro_SQL_SaveNum(REDDITO_AGRARIO) & " ")
            StrSQL.Append("   ,Deduzione              =  '" & Agro_SQL_SaveText(Deduzione) & "' ")
            StrSQL.Append("   ,Validita_Inizio  =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine    =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE    PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & Agro_SQL_SaveText(SEZIONE) & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            StrSQL.Append(" AND      Numero      = " & Agro_SQL_SaveNum(NUMERO) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & Agro_SQL_SaveText(SUBALTERNO) & "' ")
            StrSQL.Append(" AND      Qualita_Cod      = " & Agro_SQL_SaveNum(Qualita_Cod) & "  ")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Qualita_Cod: " & CStr(Qualita_Cod) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiungi_Aggiorna(ByRef Part_Cod_RITORNO As Int32,
                         ByVal PROV As String,
                         ByVal COM As String,
                         ByVal SEZIONE As String,
                         ByVal FOGLIO As Int32,
                         ByVal NUMERO As Int32,
                         ByVal SUBALTERNO As String,
                         ByVal Partita_Catastale As String,
                         ByVal ETTARI As Decimal,
                         ByVal ARE As Int32,
                         ByVal CENTIARE As Int32,
                         ByVal QUALITA_COD As Int32,
                         ByVal CLASSE As String,
                         ByVal REDDITO_DOMINICALE As Decimal,
                         ByVal REDDITO_AGRARIO As Decimal,
                         ByVal TitoloPossesso As Int32,
                         ByVal Validita_Inizio As Date,
                         ByVal Validita_Fine As Date,
                         ByVal xFiltroAggiuntivo As String,
                         ByRef objParametri As AgronicaCoreParametri _
              , Optional ByVal Data_creazione As Date = #2/1/1900# _
              , Optional ByVal Data_modifica As Date = #2/1/1900# _
              , Optional ByVal username_creazione As String = "" _
              , Optional ByVal username_modifica As String = ""
                              ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Aggiungi_Aggiorna()"

        Part_Cod_RITORNO = 0

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Try

            Dim ParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim dtPC As DataTable = ParticelleCatastali_R.Leggi(Part_Cod_RITORNO, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri)
            If dtPC.Rows.Count = 1 Then
                Part_Cod_RITORNO = dtPC.Rows(0).Item("Part_Cod")
                Modifica_2(PROV:=PROV,
                            COM:=COM,
                            SEZIONE:=SEZIONE,
                            FOGLIO:=FOGLIO,
                            NUMERO:=NUMERO,
                            SUBALTERNO:=SUBALTERNO,
                           Partita_Catastale:="",
                           ETTARI:=ETTARI,
                           ARE:=ARE,
                           CENTIARE:=CENTIARE,
                           QUALITA_COD:=0,
                           CLASSE:="",
                           REDDITO_DOMINICALE:=0,
                           REDDITO_AGRARIO:=0,
                           TitoloPossesso:=0,
                           Validita_Inizio:=AGRODATAINIZIO,
                           Validita_Fine:=AGRODATAFINE,
                            xFiltroAggiuntivo:=xFiltroAggiuntivo,
                           objParametri:=objParametri)

            ElseIf dtPC.Rows.Count = 0 Then
                Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim basecode = 0
                Dim topcode = UpperBoundTabelle_Per_SequenzaTabelle_Topcode
                Part_Cod_RITORNO = objSequenze.NuovoId_Tabella("ParticelleCAtastali", basecode, topcode, objParametri)
                Scrivi(Part_Cod:=Part_Cod_RITORNO,
                       PROV:=PROV,
                            COM:=COM,
                            SEZIONE:=SEZIONE,
                            FOGLIO:=FOGLIO,
                            NUMERO:=NUMERO,
                            SUBALTERNO:=SUBALTERNO,
                           Partita_Catastale:="",
                           ETTARI:=ETTARI,
                           ARE:=ARE,
                           CENTIARE:=CENTIARE,
                           QUALITA_COD:=0,
                           CLASSE:="",
                           REDDITO_DOMINICALE:=0,
                           REDDITO_AGRARIO:=0,
                           TitoloPossesso:=0,
                           Validita_Inizio:=AGRODATAINIZIO,
                           Validita_Fine:=AGRODATAFINE,
                           objParametri:=objParametri)

            Else
                Throw New Exception("La query deve selezionare al massimo un solo record")
            End If

            xRisp = True
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function impresexParticelle_MarcaComeInviato(
                                            ByVal PIVA As String,
                                            ByVal Sa_Cod As Int32,
                                            ByVal Part_Cod As Int32,
                                            ByVal prov As String,
                                            ByVal com As String,
                                            ByVal sezione As String,
                                            ByVal foglio As Int32,
                                            ByVal Numero As Int32,
                                            ByVal subalterno As String,
                                            ByVal Data_invio As DateTime,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) _
                                            As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Particelle_MarcaComeInviato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        ''------------------------------

        Try

            If PIVA = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE ImpreseXParticelle SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'  ")
            StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            If Part_Cod <> 0 Then
                StrSQL.Append(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            End If

            If prov <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(prov)) & "' ")
            End If

            If com <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(com)) & "' ")
            End If

            If sezione <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(sezione))) & "' ")
            End If

            If foglio <> 0 Then
                StrSQL.Append(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(foglio) & " ")
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(Numero) & " ")
            End If

            If subalterno <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(subalterno))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal PROV_Origine As String,
                             ByVal COM_Origine As String,
                             ByVal SEZIONE_Origine As String,
                             ByVal FOGLIO_Origine As Int32,
                             ByVal NUMERO_Origine As Int32,
                             ByVal SUBALTERNO_Origine As String,
                             ByVal Partita_Catastale As String,
                             ByVal ETTARI As Decimal,
                             ByVal ARE As Int32,
                             ByVal CENTIARE As Int32,
                             ByVal QUALITA_COD As Int32,
                             ByVal CLASSE As String,
                             ByVal REDDITO_DOMINICALE As Decimal,
                             ByVal REDDITO_AGRARIO As Decimal,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE ParticelleCatastali SET ")
            StrSQL.Append(" PROV = '" & Agro_SQL_SaveText(PROV) & "',  ")
            StrSQL.Append(" COM = '" & Agro_SQL_SaveText(COM) & "',  ")
            StrSQL.Append(" SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "',  ")
            StrSQL.Append(" FOGLIO =" & FOGLIO & ",  ")
            StrSQL.Append(" NUMERO =" & NUMERO & ",  ")
            StrSQL.Append(" Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "', ")
            StrSQL.Append(" PARTITA_CATASTALE = '" & Agro_SQL_SaveText(Partita_Catastale) & "',  ")
            StrSQL.Append(" ETTARI =" & ETTARI & ",  ")
            StrSQL.Append(" ARE =" & ARE & ",  ")
            StrSQL.Append(" CENTIARE = " & CENTIARE & ", ")
            StrSQL.Append(" QUALITA_COD = " & QUALITA_COD & ",  ")
            StrSQL.Append(" CLASSE ='" & Agro_SQL_SaveText(CLASSE) & "',  ")
            StrSQL.Append(" REDDITO_DOMINICALE =" & REDDITO_DOMINICALE & ",  ")
            StrSQL.Append(" REDDITO_AGRARIO = " & REDDITO_AGRARIO & ", ")
            StrSQL.Append(" Data_Modifica =" & Agro_SQL_SaveDate(Date.Today) & ",")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")

            StrSQL.Append(" WHERE PROV = '" & Agro_SQL_SaveText(PROV_Origine) & "'  ")
            StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(COM_Origine) & "'  ")
            StrSQL.Append(" AND SEZIONE ='" & Agro_SQL_SaveText(SEZIONE_Origine) & "'  ")
            StrSQL.Append(" AND FOGLIO =" & FOGLIO_Origine & "  ")
            StrSQL.Append(" AND NUMERO =" & NUMERO_Origine & "  ")
            StrSQL.Append(" AND Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO_Origine) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    'Public Function ScriviLayerGrafici(ByRef objParametri As AgronicaCoreParametri)

    'Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Modifica()"

    '====================================================================================
    'Parametri opzionali :
    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '   FileLOG = ""                =>  viene usato il valore di default
    '====================================================================================

    'Dim MessaggioErrore As String = ""
    'Dim stb As New System.Text.StringBuilder
    'Dim xRisp As Boolean = False

    'Try
    '    stb.Length = 0
    '    stb.AppendLine(" declare @last int  ")
    '    stb.AppendLine(" select @last = coalesce(max(Entita_Cod), 0)  ")
    '    stb.AppendLine(" from GIS_Entita  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine(" create table #tEnt (OldEntita_cod int, NewEntita_Cod int) ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine(" insert #tEnt(OldEntita_cod, NewEntita_Cod) ")
    '    stb.AppendLine(" select e.entita_cod, @last + dense_rank() over (order by entita_cod, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO )as newEntita_Cod ")
    '    stb.AppendLine(" from gis_entita e  ")
    '    stb.AppendLine(" where TipoEntita_Cod = 3  ")
    '    stb.AppendLine(" and Not exists (  ")
    '    stb.AppendLine("     select 1  ")
    '    stb.AppendLine("     from GIS_Entita ii  ")
    '    stb.AppendLine("     where TipoEntita_Cod = 5  ")
    '    stb.AppendLine("     and ii.PROV = e.prov  ")
    '    stb.AppendLine("     and ii.COM = e.com  ")
    '    stb.AppendLine("     and ii.SEZIONE = e.SEZIONE   ")
    '    stb.AppendLine("     and ii.FOGLIO = e.FOGLIO   ")
    '    stb.AppendLine("     and ii.NUMERO= e.NUMERO   ")
    '    stb.AppendLine("     and ii.SUBALTERNO =e.SUBALTERNO   ")
    '    stb.AppendLine(" )  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine(" insert gis_entita  ")
    '    stb.AppendLine(" select PivaSuperUser, NewEntita_Cod, 5 as TipoEntita_Cod, Piva, Sa_Cod, Appezza, Campo_Cod, Id_Imp, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, Programmazione_Entita_Cod, ID_Agenda, Ricetta_Operazione_cod, analisi_campione_cod, OLDGrafica_ID, 0 as inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, programmazione_cod, id_mov_det  ")
    '    stb.AppendLine(" from gis_entita e  ")
    '    stb.AppendLine("  inner join #tEnt old ")
    '    stb.AppendLine("      on e.entita_cod = old.oldEntita_cod ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine("                   ")
    '    stb.AppendLine(" insert GIS_ElementiGrafici  ")
    '    stb.AppendLine(" select  ")
    '    stb.AppendLine("        e.PivaSuperUser ")
    '    stb.AppendLine("  , ii.NewEntita_Cod as elementografico_cod ")
    '    stb.AppendLine("  , cast(E.foglio as varchar(100)) + ' - ' +  cast(e.numero as varchar(100))  as ElementoGrafico_Des ")
    '    stb.AppendLine("  , ii.NewEntita_Cod as Entita_Cod ")
    '    stb.AppendLine("  , 5 as LayerElementiGrafici_Cod  ")
    '    stb.AppendLine(" , geography::STGeomFromText( geometry::STGeomFromText(Poligono_GeoEntity.STAsText(), 0).MakeValid().STCentroid().STAsText(), 4326) as Poligono_GeoEntity  ")
    '    stb.AppendLine(" , Flag_GPS, g.inviato, g.datainvio, g.Data_Creazione, g.Data_Modifica, g.Username_Creazione, g.Username_Modifica, g.Validita_Inizio, g.Validita_Fine, NULL  ")
    '    stb.AppendLine(" from #tEnt ii  ")
    '    stb.AppendLine("     inner join gis_entita e  ")
    '    stb.AppendLine("      on ii.OldEntita_cod = e.entita_cod ")
    '    stb.AppendLine("     inner join GIS_ElementiGrafici g  ")
    '    stb.AppendLine("      on e.Entita_Cod = g.Entita_Cod   ")
    '    stb.AppendLine("                   ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine(" update sequenza_tabelle  ")
    '    stb.AppendLine(" set ultimo_valore = coalesce(  ")
    '    stb.AppendLine("     (select max(Entita_Cod)  ")
    '    stb.AppendLine("     from GIS_Entita  ")
    '    stb.AppendLine(" ), 0)    ")
    '    stb.AppendLine(" where nome_tabella = 'GIS_Entita'  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine("                   ")
    '    stb.AppendLine("                   ")
    '    stb.AppendLine(" update sequenza_tabelle  ")
    '    stb.AppendLine(" set ultimo_valore = coalesce(  ")
    '    stb.AppendLine("     (select max(ElementoGrafico_Cod )  ")
    '    stb.AppendLine("     from GIS_ElementiGrafici  ")
    '    stb.AppendLine(" ), 0)  ")
    '    stb.AppendLine(" where nome_tabella = 'GIS_ElementiGrafici'  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine("  ")
    '    stb.AppendLine(" drop table #tEnt")


    '    EseguiQuery_Scrittura(objParametri, stb.ToString, "")
    'Catch ex As Exception

    '    MessaggioErrore = ex.Message
    '    Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '    xRisp = False
    '    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    'End Try

    'Return xRisp

    'End Function

    '==========================================================================================
    Public Function Cancella_ParticelleCatastali_Codici(
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Long,
                                    ByVal NUMERO As Long,
                                    ByVal SUBALTERNO As String,
                                    ByVal ID_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Cancella_ParticelleCatastali_Codici()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE   ParticelleCatastali_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("31/12/1899")) & " ")
                StrSQL.Append("         ,Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1")

            Else

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     ParticelleCatastali_Codici ")
                StrSQL.Append(" WHERE   1=1")

            End If

            StrSQL.Append(" And      PROV           = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM            = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione          = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(SEZIONE), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO           = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero           = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO       = '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(SUBALTERNO), 0) & "' ")

            If ID_Cod <> 0 Then
                StrSQL.Append(" AND      Id_Cod       = " & Agro_SQL_SaveNum(ID_Cod))
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

    '==========================================================================================
    Public Function Scrivi_ParticelleCatastali_Codici(ByVal PROV As String,
                       ByVal COM As String,
                       ByVal SEZIONE As String,
                       ByVal FOGLIO As Long,
                       ByVal NUMERO As Long,
                       ByVal SUBALTERNO As String,
                       ByVal Id_Cod As Integer,
                       ByVal Val_Cod As String,
                       ByVal Validita_Inizio As Date,
                       ByVal Validita_Fine As Date,
                       ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Scrivi_ParticelleCatastali_Codici()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO ParticelleCatastali_Codici(       " &
                            "                    PROV, COM, SEZIONE, " &
                            "                    FOGLIO, NUMERO, SUBALTERNO, Id_Cod, Val_Cod,  " &
                            "                    Inviato, DataInvio, " &
                            "                    Data_Creazione,     Data_Modifica, " &
                            "                    UserName_Creazione, UserName_Modifica, " &
                            "                    Validita_Inizio,    Validita_Fine " &
                            "                    ) " &
                            "VALUES (" &
                            "         '" & Agro_SQL_SaveText(Trim(PROV)) & "'  " &
                            "         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  " &
                            "         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  " &
                            "         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  " &
                            "         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  " &
                            "         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' " &
                            "         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' " &
                  "         , 0  " &
                  "         , Null  " &
                  "         , " & Agro_SQL_SaveDate(Now) & "  " &
                  "         , " & Agro_SQL_SaveDate(Now) & "  " &
                  "         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " &
                  "         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " &
                  "         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " &
                  "         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " &
                  ")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
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
