Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_LayerElementiGrafici_R
    Inherits AgronicaCoreDataProvider.DataProvider2010

    Private Sub LeggiXGetQuery_GenericLayers(
            ByVal PivaSuperUser As String,
            ByVal TipologiaLayer_Cod As Integer,
            ByVal LayerElementiGrafici_Cod As String,
            ByVal utente As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByVal QryTiles As String,
            ByVal objparametri As AgronicaCoreParametri,
            ByRef stb As StringBuilder,
            ByVal leggiLayerNonVisibili As Boolean,
            Optional ByVal permessiWMS As Boolean = False)

        stb.Append("select  " & vbCrLf)
        stb.Append("      gl.TipologiaLayer_Cod as '@tipologia_layer' " & vbCrLf)
        stb.Append("    , gl.TipologiaLayer_des as '@nome_layer' " & vbCrLf)
        stb.Append("    , ( " & vbCrLf)
        stb.Append("        select tl.PivaSuperUser as '@pivasuperuser' " & vbCrLf)
        stb.Append("        , tl.Utente as '@utente' " & vbCrLf)
        stb.Append("        , tl.LayerElementiGrafici_Cod as '@codice'       " & vbCrLf)
        stb.Append("        , tl.Colore_Selezionato as '@colore_selezionato' " & vbCrLf)
        stb.Append("        , tl.Colore_Primario as '@colore_primario' " & vbCrLf)
        stb.Append("        , tl.Colore_Secondario as '@colore_secondario' " & vbCrLf)
        stb.Append("        , coalesce(tl.icona16, 'x04_GenericoLayer_16.png') as '@icona16' " & vbCrLf)
        stb.Append("        , coalesce(tl.icona32, 'x04_GenericoLayer_32.png') as '@icona32' " & vbCrLf)
        stb.Append("        , tl.Varianza as '@varianza' " & vbCrLf)
        stb.Append("        , tl.Trasparenza as '@trasparenza' " & vbCrLf)
        stb.Append("        , tl.MostraDescrizioneAssociata as '@MostraDescrizioneAssociata' " & vbCrLf)
        stb.Append("        , tl.ZIndex as '@ZIndex' " & vbCrLf)
        stb.Append("        , tl.flag_visibile as '@flagvisibile' " & vbCrLf)
        stb.Append("        , tl.flag_attivo as '@flagattivo' " & vbCrLf)
        'Colonne GIS_LayerElementiGrafici_Anagrafica
        stb.Append("        , tla.Flag_Inserimento as '@FlagInserimento' " & vbCrLf)
        stb.Append("        , tla.Flag_Modifica as '@FlagModifica' " & vbCrLf)
        stb.Append("        , tla.Flag_Cancellazione as '@FlagCancellazione' " & vbCrLf)
        stb.Append("        , tla.Flag_Informazioni as '@FlagInformazioni' " & vbCrLf)
        'Colonne GIS_TipoOggetto
        stb.Append("        , tog.FeatureTypeId as '@FeatureTypeId' " & vbCrLf)
        '---
        stb.Append("        , TipoNodoAlberoAnagrafe.TipoNodoAlberoAnagrafe as '@TipoNodoAlberoAnagrafe' " & vbCrLf)
        stb.Append("        , tl.LayerElementiGrafici_Des as '*' " & vbCrLf)
        stb.Append("        from GIS_LayerElementiGrafici tl  " & vbCrLf)
        'Join GIS_LayerElementiGrafici_Anagrafica => tla
        stb.Append("        left Join GIS_LayerElementiGrafici_Anagrafica tla " & vbCrLf)
        stb.Append("                  on  tla.PivaSuperUser = tl.PivaSuperUser " & vbCrLf)
        If IsLayerAppartenenzaImpianti(TipologiaLayer_Cod) Then
            stb.Append("                  and tla.LayerElementiGrafici_Cod = '" & Agro_SQL_SaveText(enum_Gis_LayerElementiGrafici_std.IMPIANTI) & "' " & vbCrLf)
            stb.Append("                  and tla.TipologiaLayer_cod = '" & Agro_SQL_SaveText(enum_TipologiaLayer.Std) & "' " & vbCrLf)
        Else
            '-------------------------------> CAST necessario per AgronicaSementi2013 dove GIS_LayerElementiGrafici.LayerElementiGrafici_Cod = varchar
            'stb.Append("                  and cast(tla.LayerElementiGrafici_Cod as varchar) = cast(tl.LayerElementiGrafici_Cod as varchar) " & vbCrLf)
            stb.Append("                  and tla.LayerElementiGrafici_Cod = tl.LayerElementiGrafici_Cod " & vbCrLf)
            '-------------------------------<
            stb.Append("                  and tla.TipologiaLayer_cod = tl.TipologiaLayer_cod " & vbCrLf)
        End If
        'Join GIS_LayerElementiGraficiXTipoOggetto => legtog
        stb.Append("        left Join GIS_LayerElementiGraficiXTipoOggetto legtog " & vbCrLf)
        stb.Append("                  on  legtog.PivaSuperUser = tla.PivaSuperUser " & vbCrLf)
        stb.Append("                  and legtog.LayerElementiGrafici_Cod = tla.LayerElementiGrafici_Cod " & vbCrLf)
        'Join GIS_TipoOggetto => tog
        stb.Append("        left Join GIS_TipoOggetto tog " & vbCrLf)
        stb.Append("                  on  tog.GIS_TipoOggetto_Cod = legtog.GIS_TipoOggetto_Cod " & vbCrLf)
        '---
        stb.Append("                    inner Join " & vbCrLf)
        stb.Append(" ( " & vbCrLf)
        stb.Append("    select tipi.LayerElementiGrafici_Cod,  " & vbCrLf)
        stb.Append("            tipi.Classi AS TipoNodoAlberoAnagrafe " & vbCrLf)
        stb.Append("             from ( " & vbCrLf)
        stb.Append("                select distinct t1.LayerElementiGrafici_Cod,(  " & vbCrLf)
        stb.Append("                    select cast(nodi.TipoNodoAlberoAnagrafe as varchar(100)) + ',' as [text()] " & vbCrLf)
        stb.Append("                    from (select distinct t.TipoNodoAlberoAnagrafe " & vbCrLf)
        stb.Append("                          from gis_layerelementigrafici g " & vbCrLf)
        stb.Append("                          inner join gis_tipoEntita t on g.LayerElementiGrafici_Cod = t.LayerElementiGrafici_Cod " & vbCrLf)
        stb.Append("                          where g.LayerElementiGrafici_Cod = t1.LayerElementiGrafici_Cod) nodi " & vbCrLf)
        stb.Append("                    for XML path('') " & vbCrLf)
        stb.Append("                ) [Classi]           " & vbCrLf)
        stb.Append("        from gis_layerelementigrafici t1 ) tipi " & vbCrLf)
        stb.Append(" ) TipoNodoAlberoAnagrafe  on TipoNodoAlberoAnagrafe.LayerElementiGrafici_Cod = tl.LayerElementiGrafici_Cod " & vbCrLf)
        stb.Append("        where gl.TipologiaLayer_Cod = tl.TipologiaLayer_Cod " & vbCrLf)
        If Not leggiLayerNonVisibili Then
            stb.Append("        and tl.flag_visibile <> 0 " & vbCrLf)
        End If
        stb.Append("         AND tl.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
        stb.Append("         AND gl.TipologiaLayer_Cod = " & Agro_SQL_SaveNum(TipologiaLayer_Cod) & " ")

        If LayerElementiGrafici_Cod <> 0 Then
            stb.Append("         AND tl.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
        Else
            If permessiWMS = False And TipologiaLayer_Cod = 1 Then
                stb.Append("         AND tl.LayerElementiGrafici_Cod >= 0 ")
            End If
        End If

        If utente = "" Then
            utente = objparametri.UtenteUsername
        End If

        stb.Append("         AND tl.Utente = '" & Agro_SQL_SaveText(utente) & "' ")

        'If permessiWMS = False Then
        '    stb.Append("         AND tl.Utente = '" & Agro_SQL_SaveText(utente) & "' ")
        'End If

        If xFiltroAggiuntivo <> "" Then
            stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objparametri))
        End If
        stb.Append("    order by " & xOrderBy & "  tl.LayerElementiGrafici_Cod  for xml path('valori'), type, elements       " & vbCrLf)
        stb.Append(" ) " & vbCrLf)

        'tiles in standard entità

        If QryTiles <> "-1" Then
            If Not String.IsNullOrEmpty(QryTiles) Then
                stb.Append(QryTiles)
            Else
                stb.Append(", (  " & vbCrLf)
                stb.Append("Select " & vbCrLf)
                stb.Append("           tt.LayerElementiGrafici_Cod as '@codice'  " & vbCrLf)
                stb.Append("         , tt.Colore_Primario as '@colore_primario'  " & vbCrLf)
                stb.Append("         , tt.Colore_Secondario as '@colore_secondario'  " & vbCrLf)
                stb.Append("         , tt.Varianza as '@varianza'  " & vbCrLf)
                stb.Append("         , tt.LayerTiles_Cod as '@id'  " & vbCrLf)
                stb.Append("         , tt.LayerTiles_Des as tiletesto  " & vbCrLf)
                stb.Append("         from GIS_LayerElementiGrafici tl  " & vbCrLf)
                stb.Append("         inner join GIS_LayerTiles tt " & vbCrLf)
                stb.Append("               on  tl.PivaSuperUser = tt.PivaSuperUser  " & vbCrLf)
                stb.Append("               and tl.utente = tt.Utente  " & vbCrLf)
                stb.Append("               and tl.LayerElementiGrafici_Cod = tt.LayerElementiGrafici_Cod " & vbCrLf)
                stb.Append("               and tl.TipologiaLayer_cod = tt.TipologiaLayer_cod " & vbCrLf)
                stb.Append("         where gl.TipologiaLayer_Cod = tl.TipologiaLayer_Cod  " & vbCrLf)
                stb.Append("         AND tl.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
                stb.Append("         AND tl.Utente = '" & Agro_SQL_SaveText(utente) & "' ")
                stb.Append("         and tl.flag_visibile <> 0 " & vbCrLf)
                stb.Append("         order by   tl.LayerElementiGrafici_Cod   " & vbCrLf)
                stb.Append("         for xml path('tile'), type, elements  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("  ) tiles " & vbCrLf)
            End If
        Else
            stb.Append("  , NULL as  tiles " & vbCrLf)
        End If

        stb.Append(" from  GIS_TipologiaLayer  gl " & vbCrLf)

        stb.Append(" where gl.TipologiaLayer_cod =  " & TipologiaLayer_Cod & vbCrLf)

        stb.Append(" group by gl.tipologiaLayer_Cod, gl.TipologiaLayer_des " & vbCrLf)

    End Sub

    Private Shared Function IsLayerAppartenenzaImpianti(TipologiaLayer_Cod As Integer) As Boolean
        Return TipologiaLayer_Cod = enum_TipologiaLayer.SpecieVegetale OrElse
               TipologiaLayer_Cod = enum_TipologiaLayer.Cultivar
    End Function

    Private Shared Sub LeggiXGetQuery_tilesRilieviVegetoProduttivi(ByVal objparametri As AgronicaCoreParametri, ByVal utente As String, ByVal stb1 As StringBuilder)

        If utente = "" Then
            utente = objparametri.UtenteUsername
        End If

        stb1.Append(" , (   " & vbCrLf)
        stb1.Append(" Select  " & vbCrLf)
        stb1.Append("           tt.LayerElementiGrafici_Cod as '@codice' " & vbCrLf)
        stb1.Append("         , tt.Varianza as '@varianza' " & vbCrLf)
        stb1.Append("         , tt.colore_primario as '@colore_primario' " & vbCrLf)
        stb1.Append("         , tt.colore_secondario as '@colore_secondario' " & vbCrLf)
        stb1.Append("         , tt.MxAvCod as '@id' " & vbCrLf)
        stb1.Append("         , tt.LayerTiles_Des as tiletesto " & vbCrLf)
        stb1.Append("         , tt.av_des as tilelayerpadre " & vbCrLf)
        stb1.Append("         , tileLabels.c1 as tilelabels " & vbCrLf)

        stb1.Append(" from (  " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("             select  " & vbCrLf)
        stb1.Append("                  id1.IND_MAT_DES as  av_des " & vbCrLf)
        stb1.Append("                 , mav.IND_MAT_COD as LayerElementiGrafici_Cod  " & vbCrLf)
        stb1.Append("                 , mav.IND_MAT_COD  " & vbCrLf)
        stb1.Append("                 , mav.cod as MxAvCod  " & vbCrLf)
        stb1.Append("                 , UDM.udm_des as LayerTiles_Des  " & vbCrLf)
        stb1.Append("                 , t2.varianza " & vbCrLf)
        stb1.Append("                 , t2.Colore_Primario " & vbCrLf)
        stb1.Append("                 , t2.Colore_secondario   " & vbCrLf)
        stb1.Append(" ")

        stb1.Append("             from indiciMaturita id1  " & vbCrLf)
        stb1.Append("                 inner join MisuraxIndiciMaturita mXid  " & vbCrLf)
        stb1.Append("                     ON mXid.IND_MAT_COD = id1.IND_MAT_COD          " & vbCrLf)
        stb1.Append("                inner join UnitaMisura udm  " & vbCrLf)
        stb1.Append("                    on mXid.UDM_COD = udm.UDM_COD          " & vbCrLf)
        stb1.Append("                inner join ( select LayerElementiGrafici_Cod, utente, LayerTiles_Cod as MxAv_cod, varianza, colore_primario, colore_secondario from gis_layertiles where utente = '" & utente & "' and TipologiaLayer_Cod = " & TipiEnumerativi.enum_TipologiaLayer.RilieviVegetoProduttivi & " ) t2  " & vbCrLf)
        stb1.Append("                     on t2.MxAv_cod = mav.Cod  " & vbCrLf)

        stb1.Append("  " & vbCrLf)
        stb1.Append("          ) tt  " & vbCrLf)
        stb1.Append("  " & vbCrLf)

        stb1.Append("         left join (select MxAv_Cod, count(*) as varianza from  misuraXAvversita_anagrafiche group by MxAv_Cod) tt1 " & vbCrLf)
        stb1.Append("            on tt.MxAvCod = tt1.MxAV_Cod " & vbCrLf)
        stb1.Append("         left join ( " & vbCrLf)
        stb1.Append("            select t1.MxAV_Cod, (  " & vbCrLf)

        stb1.Append("                select  " & vbCrLf)
        stb1.Append("                      c1.Anag_valore as '@valore_associato' " & vbCrLf)
        stb1.Append("                    , '0' as '@valore_min' " & vbCrLf)
        stb1.Append("                    , '0' as '@valore_max' " & vbCrLf)
        stb1.Append("                    , Anag_des as [text()]  " & vbCrLf)
        stb1.Append("                from MisuraXAvversita_Anagrafiche c1 " & vbCrLf)
        stb1.Append("                where c1.MxAV_Cod = t1.MxAV_Cod " & vbCrLf)
        stb1.Append("                for xml path('tilelabel'), type, elements  ) c1  " & vbCrLf)

        stb1.Append("            from ( select distinct MxAV_Cod from MisuraXAvversita_Anagrafiche ) t1  " & vbCrLf)
        stb1.Append("                inner join ( select utente, LayerTiles_Cod as MxAv_cod from gis_layertiles ) t2 " & vbCrLf)
        stb1.Append("                    on t2.MxAv_cod = t1.MxAV_Cod " & vbCrLf)

        stb1.Append("  " & vbCrLf)
        stb1.Append("        ) tileLabels " & vbCrLf)
        stb1.Append("            on tt.MxAvCod = tileLabels.MxAV_Cod " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("           order by   tt.LayerElementiGrafici_Cod    " & vbCrLf)
        stb1.Append("           for xml path('tile'), type, elements   " & vbCrLf)
        stb1.Append(" ")

        stb1.Append("  " & vbCrLf)
        stb1.Append(" ) as tiles ")
    End Sub

    Private Shared Sub LeggiXGetQuery_tilesAvv(
        ByVal objparametri As AgronicaCoreParametri,
        ByVal utente As String,
        ByVal stb1 As StringBuilder)

        If utente = "" Then
            utente = objparametri.UtenteUsername
        End If

        stb1.Append(" , (   " & vbCrLf)
        stb1.Append(" Select  " & vbCrLf)
        stb1.Append("           tt.LayerElementiGrafici_Cod as '@codice'                 " & vbCrLf)
        stb1.Append("         , tt.Varianza as '@varianza'   " & vbCrLf)
        stb1.Append("         , tt.colore_primario as '@colore_primario' " & vbCrLf)
        stb1.Append("         , tt.colore_secondario as '@colore_secondario' " & vbCrLf)
        stb1.Append("         , tt.MxAvCod as '@id'  " & vbCrLf)
        stb1.Append("         , tt.LayerTiles_Des as tiletesto   " & vbCrLf)
        stb1.Append("         , tt.av_des as tilelayerpadre   " & vbCrLf)
        stb1.Append("         , tileLabels.c1 as tilelabels " & vbCrLf)

        stb1.Append(" from (  " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("             select  " & vbCrLf)
        stb1.Append("                   a.Av_Des_Vol as  av_des " & vbCrLf)
        stb1.Append("                 , mav.av_cod as LayerElementiGrafici_Cod  " & vbCrLf)
        stb1.Append("                 , mav.av_cod  " & vbCrLf)
        stb1.Append("                 , mav.cod as MxAvCod  " & vbCrLf)
        stb1.Append("                 , UDM.udm_des collate Latin1_General_CI_AS + ' [' + sv.veg_des + ']' as LayerTiles_Des  " & vbCrLf)
        stb1.Append("                 , t2.varianza " & vbCrLf)
        stb1.Append("                 , t2.Colore_Primario " & vbCrLf)
        stb1.Append("                 , t2.Colore_secondario   " & vbCrLf)
        stb1.Append(" ")

        stb1.Append("             from Avversita a  " & vbCrLf)
        stb1.Append("             inner join MisuraXAvversita mav  " & vbCrLf)
        stb1.Append("                   on a.av_cod = mav.av_cod            " & vbCrLf)
        stb1.Append("             inner join specievegetali sv " & vbCrLf)
        stb1.Append("                   on sv.veg_cod = mav.veg_cod " & vbCrLf)
        stb1.Append("             inner join UnitaMisura udm  " & vbCrLf)
        stb1.Append("                   on mav.UDM_COD = udm.UDM_COD          " & vbCrLf)
        stb1.Append("             inner join ( select LayerElementiGrafici_Cod, utente, LayerTiles_Cod as MxAv_cod, varianza, colore_primario, colore_secondario " & vbCrLf)
        stb1.Append("                          from   gis_layertiles " & vbCrLf)
        stb1.Append("                          where  utente = '" & utente & "' " & vbCrLf)
        stb1.Append("                          and    TipologiaLayer_Cod = " & TipiEnumerativi.enum_TipologiaLayer.Avversita & " " & vbCrLf)
        stb1.Append("                        ) t2  " & vbCrLf)
        stb1.Append("                   on t2.MxAv_cod = mav.Cod  " & vbCrLf)

        stb1.Append("  " & vbCrLf)
        stb1.Append("          ) tt  " & vbCrLf)
        stb1.Append("  " & vbCrLf)

        stb1.Append("         left join (select MxAv_Cod, count(*) as varianza  " & vbCrLf)
        MisuraXAvversitaAnagrafiche(stb1, "a1")
        stb1.Append("         group by MxAv_Cod) tt1 " & vbCrLf)
        stb1.Append("            on tt.MxAvCod = tt1.MxAV_Cod " & vbCrLf)
        stb1.Append("          left join ( " & vbCrLf)
        stb1.Append("            select t1.MxAV_Cod, (  " & vbCrLf)

        stb1.Append("                select  " & vbCrLf)
        stb1.Append("                      c1.Anag_valore as '@valore_associato' " & vbCrLf)
        stb1.Append("                    , '0' as '@valore_min' " & vbCrLf)
        stb1.Append("                    , '0' as '@valore_max' " & vbCrLf)
        stb1.Append("                    , Anag_des as [text()]  " & vbCrLf)
        MisuraXAvversitaAnagrafiche(stb1, "c1")
        stb1.Append("                where c1.MxAV_Cod = t1.MxAV_Cod " & vbCrLf)
        stb1.Append("                for xml path('tilelabel'), type, elements  ) c1  " & vbCrLf)

        stb1.Append("            from ( select distinct MxAV_Cod   " & vbCrLf)
        MisuraXAvversitaAnagrafiche(stb1, "a1")
        stb1.Append("           ) t1  " & vbCrLf)
        stb1.Append("             inner join ( select utente, LayerTiles_Cod as MxAv_cod from gis_layertiles " & vbCrLf)
        stb1.Append("                          where  utente = '" & utente & "' " & vbCrLf)
        stb1.Append("                          and    TipologiaLayer_Cod = " & TipiEnumerativi.enum_TipologiaLayer.Avversita & "  " & vbCrLf)
        stb1.Append("                        ) t2 " & vbCrLf)
        stb1.Append("             on t2.MxAv_cod = t1.MxAV_Cod " & vbCrLf)

        stb1.Append("  " & vbCrLf)
        stb1.Append("        ) tileLabels " & vbCrLf)
        stb1.Append("            on tt.MxAvCod = tileLabels.MxAV_Cod " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("           order by   tt.LayerElementiGrafici_Cod    " & vbCrLf)
        stb1.Append("           for xml path('tile'), type, elements   " & vbCrLf)
        stb1.Append(" ")

        stb1.Append("  " & vbCrLf)
        stb1.Append(" ) as tiles ")

    End Sub

    Public Shared Sub MisuraXAvversitaAnagrafiche(stb1 As StringBuilder, aliasTb As String)

        stb1.AppendLine("     from ( ")
        stb1.AppendLine("        select MxAV_Cod, anag_valore, anag_Des ")
        stb1.AppendLine("         From MisuraXAvversita_Anagrafiche ")
        stb1.AppendLine("         union ")
        stb1.AppendLine("         Select ma.cod As MxAV_Cod, maa.* ")
        stb1.AppendLine("         from(select * from  MisuraxAvversita where UDM_COD = 5001049 )  ma ")
        stb1.AppendLine("         inner Join( ")
        stb1.AppendLine("             select 0 as anag_valore, 'Nessun Rischio'  as anag_Des ")
        stb1.AppendLine("             union  ")
        stb1.AppendLine("             Select 5 As anag_valore, 'Rischio Medio'  ")
        stb1.AppendLine("             union ")
        stb1.AppendLine("             Select 10 As anag_valore, 'Rischio Alto' ")
        stb1.AppendLine("         ) maa ")
        stb1.AppendLine("     On 1=1 ")
        stb1.AppendLine("    ) " & aliasTb)

    End Sub

    Private Sub LeggiXGetQuery(ByVal PivaSuperUser As String,
                               ByRef Utente As String,
                               ByVal LayerElementiGrafici_Cod As Int32,
                               ByVal AggiungiLayer_Imprese As Boolean,
                               ByVal AggiungiLayer_SpecieVegetali As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByRef stb As System.Text.StringBuilder,
                               ByVal leggiLayerNonVisibili As Boolean,
                               Optional ByVal permessiWMS As Boolean = False)

        '------ standard entità (1)
        'GABRIELE ---> se il parametro QryTiles <> "-1" (era "") non si disegnano i poligoni con il colore giusto in ImpostaShape() (Mappa.js)
        '' VAnni: 13/3/2019: non chiaro il commento di Gabriele, in ogni caso anche sulla tipologia layer standard ci sono le tematizzazioni, re-imposto la chiamata con QryTiles = ""
        LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.Std, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, " tl.ZIndex, ", "", objParametri, stb, leggiLayerNonVisibili, permessiWMS)

        '--------imprese (100)

        If AggiungiLayer_Imprese Then
            stb.Append(" union all  " & vbCrLf)
            LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.Organizzazione, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "tl.LayerElementiGrafici_Des, ", "-1", objParametri, stb, leggiLayerNonVisibili)
        End If

        stb.Append("  " & vbCrLf)

        'i layer seguenti (specie, avversità fenologia, ecc.. si basano su un assegnazione automatica dei dati nelle tabelle gis_layerElementiGrafici e gis_layerTiles)
        'vedi le funzioni "LeggiXGetQuery_GeneraColoriAutomatici_*" in GIS_LayerElementiGrafici_W

        '--------specie vegetali (5)
        stb.Append(" union all " & vbCrLf)

        If AggiungiLayer_SpecieVegetali Then
            LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.SpecieVegetale, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "tl.LayerElementiGrafici_Des, ", "-1", objParametri, stb, leggiLayerNonVisibili)

            '--------cultivar (15)
            stb.Append(" union all " & vbCrLf)

            LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.Cultivar, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "", "-1", objParametri, stb, leggiLayerNonVisibili)
        End If

        '------- tiles legati ad avversità.
        Dim stb1 As New StringBuilder
        LeggiXGetQuery_tilesAvv(objParametri, Utente, stb1)

        '-------- avversità (7)
        stb.Append("        union all  " & vbCrLf)
        LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.Avversita, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "", stb1.ToString, objParametri, stb, leggiLayerNonVisibili)

        '------- fenologia (8)
        stb.Append("        union all  " & vbCrLf)
        LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.Fenologia, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "", "-1", objParametri, stb, leggiLayerNonVisibili)

        '------- rilievi vegeto produttivi (9)
        stb.Append("        union all  " & vbCrLf)
        LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.RilieviVegetoProduttivi, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "", "", objParametri, stb, leggiLayerNonVisibili)

        '------- Percorsi (10)
        stb.Append("        union all  " & vbCrLf)
        LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.Percorsi, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "", "", objParametri, stb, leggiLayerNonVisibili)

        '------- Percorsi (11)
        stb.Append("        union all  " & vbCrLf)
        LeggiXGetQuery_GenericLayers(PivaSuperUser, enum_TipologiaLayer.AnalisiPeriodicitaAgenda, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "", "", objParametri, stb, leggiLayerNonVisibili)

        stb.Append(" for xml path('layer'), root('layersdescrizioni'), elements " & vbCrLf)

    End Sub

    Private Shared Sub LeggiXGetQueryGeneraVarC(ByVal indice As String, ByVal stb As System.Text.StringBuilder)
        stb.Append(" declare @c" & indice & " int " & vbCrLf)
        stb.Append(" select @c" & indice & " = COUNT (*) " & vbCrLf)
        stb.Append(" from #t" & indice & " " & vbCrLf)
        stb.Append("" & vbCrLf)
    End Sub

    Public Function LeggiXML(
        ByVal PivaSuperUser As String,
        ByVal Utente As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal AggiungiLayer_Imprese As Boolean,
        ByVal AggiungiLayer_SpecieVegetali As Boolean,
                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   LayerElementiGrafici_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As String

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    LeggiXGetQuery(PivaSuperUser, Utente, LayerElementiGrafici_Cod, AggiungiLayer_Imprese, AggiungiLayer_SpecieVegetali, xFiltroAggiuntivo, objParametri, stb, False)

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XML(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT


    End Function

    '//////////////////////

    Public Function LeggiXDocument(ByVal PivaSuperUser As String,
                                   ByVal Utente As String,
                                   ByVal LayerElementiGrafici_Cod As Int32,
                                   ByVal AggiungiLayer_Imprese As Boolean,
                                   ByVal AggiungiLayer_SpecieVegetali As Boolean,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal leggiLayerNonVisibili As Boolean = False,
                                   Optional ByVal permessiWMS As Boolean = False
                                   ) As XDocument

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   LayerElementiGrafici_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As XDocument

        Try

            LeggiXGetQuery(PivaSuperUser,
                           Utente,
                           LayerElementiGrafici_Cod,
                           AggiungiLayer_Imprese,
                           AggiungiLayer_SpecieVegetali,
                           xFiltroAggiuntivo,
                           objParametri,
                           stb,
                           leggiLayerNonVisibili,
                           permessiWMS)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XDoc(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Sub Leggi_CondizioneWhere(
        ByVal PivaSuperUser As String,
        ByVal Utente As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal TipologiaLayer_cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef StrSQL As System.Text.StringBuilder
    )

        StrSQL.AppendLine(" WHERE   GIS_LayerElementiGrafici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
        StrSQL.AppendLine(" AND     GIS_LayerElementiGrafici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

        StrSQL.AppendLine(" AND GIS_LayerElementiGrafici.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

        If TipologiaLayer_cod <> 0 Then
            StrSQL.AppendLine(" AND GIS_LayerElementiGrafici.TipologiaLayer_Cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")
        End If

        If LayerElementiGrafici_Cod <> 0 Then
            StrSQL.AppendLine(" AND GIS_LayerElementiGrafici.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
        End If

        If Utente <> "" Then
            StrSQL.AppendLine(" AND GIS_LayerElementiGrafici.Utente = '" & Agro_SQL_SaveText(Utente) & "' ")
        End If

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If
        '--------------------------------------------------------------------------
        Select Case objParametri.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                StrSQL.AppendLine(" AND   GIS_LayerElementiGrafici.Inviato >=0 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                StrSQL.AppendLine(" AND   GIS_LayerElementiGrafici.Inviato =-1 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select
        '--------------------------------------------------------------------------
        If xOrderBy <> "" Then
            StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        End If
    End Sub

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Leggi(
            ByVal PivaSuperUser As String,
            ByVal Utente As String,
            ByVal LayerElementiGrafici_Cod As Int32,
            ByVal TipologiaLayer_cod As Integer,
            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   LayerElementiGrafici_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT  LayerElementiGrafici_Cod, LayerElementiGrafici_Des ")
                    StrSQL.AppendLine(" FROM    GIS_LayerElementiGrafici ")
                    Leggi_CondizioneWhere(PivaSuperUser, Utente, LayerElementiGrafici_Cod, TipologiaLayer_cod, xFiltroAggiuntivo, xOrderBy, objParametri, StrSQL)


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT  * ")
                    StrSQL.AppendLine(" FROM    GIS_LayerElementiGrafici ")
                    Leggi_CondizioneWhere(PivaSuperUser, Utente, LayerElementiGrafici_Cod, TipologiaLayer_cod, xFiltroAggiuntivo, xOrderBy, objParametri, StrSQL)



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '

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

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////

    Public Function Leggi_quelli_che_hanno_dettagli(
            ByVal utente As String,
            ByVal tipologia_layer As String,
            ByVal PivaSuperUser As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT distinct gl.LayerElementiGrafici_Cod, gl.LayerElementiGrafici_Des " & vbCrLf)
            stb.AppendLine(" FROM GIS_LayerElementiGrafici gl " & vbCrLf)
            stb.AppendLine(" INNER JOIN GIS_LayerTiles t  " & vbCrLf)
            stb.AppendLine("         ON gl.PivaSuperUser = t.PivaSuperUser " & vbCrLf)
            stb.AppendLine("        AND gl.LayerElementiGrafici_Cod = t.LayerElementiGrafici_Cod " & vbCrLf)
            stb.AppendLine("        AND gl.TipologiaLayer_cod = t.TipologiaLayer_cod " & vbCrLf)
            stb.AppendLine("        AND gl.Utente = t.Utente " & vbCrLf)
            stb.AppendLine(" WHERE gl.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            stb.AppendLine(" AND gl.tipologiaLayer_cod = " & Agro_SQL_SaveNum(tipologia_layer) & " ")
            stb.AppendLine(" AND gl.utente = '" & Agro_SQL_SaveText(utente) & "' ")

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

#Region "Nuove chiamate GIS"

    Public Function LeggiElencoRecordLayerElementiGraficiSenzaPermessi(ByVal PivaSuperUser As String,
                                                                       ByVal LayerElementiGrafici_Cod As Integer,
                                                                       ByRef objParametri_server As AgronicaCoreParametri,
                                                                       ByRef objParametri_utenti As AgronicaCoreParametri
                                                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.LeggiElencoRecordLayerElementiGraficiSenzaPermessi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Clear()
            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine(" 	T.PivaSuperUser, ")
            StrSQL.AppendLine(" 	T.Utente, ")
            StrSQL.AppendLine(" 	T.LayerElementiGrafici_Cod, ")
            StrSQL.AppendLine(" 	T.TipologiaLayer_cod, ")
            StrSQL.AppendLine(" 	P.Utente as UtenteLink ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine(" 	gis_layerelementigrafici T left join ")
            StrSQL.AppendLine(" 	( ")
            StrSQL.AppendLine(" 		select ")
            StrSQL.AppendLine(" 			LayerElementiGrafici_Cod, ")
            StrSQL.AppendLine(" 			Utente, ")
            StrSQL.AppendLine(" 			1 as TipologiaLayer_Cod ")
            StrSQL.AppendLine(" 		from ")
            StrSQL.AppendLine(" 			GIS_LayerElementiGraficiXUtente ")
            StrSQL.AppendLine(" 		where ")
            StrSQL.AppendLine(" 			LayerElementiGrafici_Cod=" & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            StrSQL.AppendLine(" 		union all ")
            StrSQL.AppendLine(" 		select ")
            StrSQL.AppendLine(" 			LayerElementiGrafici_Cod, ")
            StrSQL.AppendLine(" 			b.UserName, ")
            StrSQL.AppendLine(" 			1 as TipologiaLayer_Cod ")
            StrSQL.AppendLine(" 		from ")
            StrSQL.AppendLine(" 			GIS_LayerElementiGraficiXGruppiUtente a inner join ")
            StrSQL.AppendLine(" 			" & objParametri_utenti.Recupera_NomeDB() & ".dbo.Utenti_xGruppi_Utente b ")
            StrSQL.AppendLine(" 			on (a.Gruppi_Utente_cod = b.Gruppi_Utente_cod) ")
            StrSQL.AppendLine(" 		where ")
            StrSQL.AppendLine(" 			LayerElementiGrafici_Cod=" & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            StrSQL.AppendLine(" 	) as P ")
            StrSQL.AppendLine(" on (T.LayerElementiGrafici_Cod=P.LayerElementiGrafici_Cod and T.Utente=P.Utente and T.TipologiaLayer_cod=P.TipologiaLayer_Cod) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine(" 	T.PivaSuperUser='" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.AppendLine(" 	and T.LayerElementiGrafici_Cod=" & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            StrSQL.AppendLine(" 	and T.TipologiaLayer_cod=1 ")
            StrSQL.AppendLine(" 	and P.Utente is null ")

            DT = EseguiQuery_Lettura(objParametri_server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function


    Public Function LeggiElencoDettagliLayers(ByVal PivaSuperUser As String,
                                              ByRef Utente As String,
                                              ByVal Tipologia_Layer_Richiesta As Integer,
                                              ByVal LayerElementiGrafici_Cod As Int32,
                                              ByVal AggiungiLayer_Imprese As Boolean,
                                              ByVal AggiungiLayer_SpecieVegetali As Boolean,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              Optional ByVal LayerList As List(Of Integer) = Nothing
                                               ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.LeggiElencoDettagliLayers()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim DTFinal As New DataTable

        Try
            'lavez - 22/07/2024 - aggiunti due parametri tipologia_layer e layerList in caso si provenga al di fuori del GIS
            If LayerList IsNot Nothing AndAlso LayerList.Count > 0 Then
                For Each layer In LayerList
                    StrSQL.Clear()
                    LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, layer, Tipologia_Layer_Richiesta, xFiltroAggiuntivo, objParametri, StrSQL)
                    DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                    If DT IsNot Nothing Then
                        DTFinal.Merge(DT)
                    End If
                Next
            Else
                StrSQL.Clear()
                LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.Std, xFiltroAggiuntivo, objParametri, StrSQL)
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                If DT IsNot Nothing Then
                    DTFinal.Merge(DT)
                End If

                '--------imprese (100)
                If AggiungiLayer_Imprese Then
                    StrSQL.Clear()
                    LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.Organizzazione, xFiltroAggiuntivo, objParametri, StrSQL)
                    DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                    If DT IsNot Nothing Then
                        DTFinal.Merge(DT)
                    End If

                End If

                If AggiungiLayer_SpecieVegetali Then
                    '--------specie vegetali (5)
                    StrSQL.Clear()
                    LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.SpecieVegetale, xFiltroAggiuntivo, objParametri, StrSQL)
                    DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                    If DT IsNot Nothing Then
                        DTFinal.Merge(DT)
                    End If

                    '--------cultivar (15)
                    StrSQL.Clear()
                    LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.Cultivar, xFiltroAggiuntivo, objParametri, StrSQL)
                    DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                    If DT IsNot Nothing Then
                        DTFinal.Merge(DT)
                    End If
                End If

                '-------- avversità (7)
                StrSQL.Clear()
                LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.Avversita, xFiltroAggiuntivo, objParametri, StrSQL)
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                If DT IsNot Nothing Then
                    DTFinal.Merge(DT)
                End If

                '------- fenologia (8)
                StrSQL.Clear()
                LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.Fenologia, xFiltroAggiuntivo, objParametri, StrSQL)
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                If DT IsNot Nothing Then
                    DTFinal.Merge(DT)
                End If

                '------- rilievi vegeto produttivi (9)
                StrSQL.Clear()
                LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.RilieviVegetoProduttivi, xFiltroAggiuntivo, objParametri, StrSQL)
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                If DT IsNot Nothing Then
                    DTFinal.Merge(DT)
                End If

                '------- Percorsi (10)
                StrSQL.Clear()
                LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.Percorsi, xFiltroAggiuntivo, objParametri, StrSQL)
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                If DT IsNot Nothing Then
                    DTFinal.Merge(DT)
                End If

                '------- Analisi Periodicità Agenda (11)
                StrSQL.Clear()
                LeggiElencoDettagliPerTipologiaLayer(PivaSuperUser, Utente, LayerElementiGrafici_Cod, enum_TipologiaLayer.AnalisiPeriodicitaAgenda, xFiltroAggiuntivo, objParametri, StrSQL)
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                If DT IsNot Nothing Then
                    DTFinal.Merge(DT)
                End If
            End If
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DTFinal = Nothing
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DTFinal
    End Function

    Public Function VerificaEsistenzaPalette(ByVal layerElementiGrafici_Cod As Int32,
                                             ByVal tipologiaLayer_Cod As Int32,
                                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.VerificaEsistenzaPalette()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim resp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     ISNULL(PaletteVisualizzazione, '') PaletteVisualizzazione ")
            StrSQL.AppendLine(" FROM GIS_LayerElementiGrafici_Anagrafica ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" AND TipologiaLayer_cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_Cod)))

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT Is Nothing OrElse DT.Rows.Count <= 0 Then
                Throw New Exception("Layer non trovato.")
            End If

            resp = Not DT.Rows(0)("PaletteVisualizzazione").ToString.Equals("")

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Private Sub LeggiElencoDettagliPerTipologiaLayer(ByVal PivaSuperUser As String,
                                                      ByRef Utente As String,
                                                      ByVal LayerElementiGrafici_Cod As Int32,
                                                      ByVal TipologiaLayer_cod As Integer,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal objParametri As AgronicaCoreParametri,
                                                      ByRef StrSQL As StringBuilder)

        Select Case TipologiaLayer_cod
            Case enum_TipologiaLayer.Std
                LeggiGetQuery_GenericLayersSenzaTiles(PivaSuperUser, TipologiaLayer_cod, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, " tl.ZIndex, ", objParametri, StrSQL)
            Case enum_TipologiaLayer.Organizzazione
                LeggiGetQuery_GenericLayersSenzaTiles(PivaSuperUser, TipologiaLayer_cod, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "tl.LayerElementiGrafici_Des, ", objParametri, StrSQL)
            Case enum_TipologiaLayer.SpecieVegetale
                LeggiGetQuery_GenericLayersSenzaTiles(PivaSuperUser, TipologiaLayer_cod, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "tl.LayerElementiGrafici_Des, ", objParametri, StrSQL)
            Case enum_TipologiaLayer.Cultivar,
                 enum_TipologiaLayer.Avversita,
                 enum_TipologiaLayer.Fenologia,
                 enum_TipologiaLayer.RilieviVegetoProduttivi,
                 enum_TipologiaLayer.Percorsi,
                 enum_TipologiaLayer.AnalisiPeriodicitaAgenda
                LeggiGetQuery_GenericLayersSenzaTiles(PivaSuperUser, TipologiaLayer_cod, LayerElementiGrafici_Cod, Utente, xFiltroAggiuntivo, "", objParametri, StrSQL)
        End Select

    End Sub

    Private Sub LeggiGetQuery_GenericLayersSenzaTiles(
            ByVal PivaSuperUser As String,
            ByVal TipologiaLayer_Cod As Integer,
            ByVal LayerElementiGrafici_Cod As String,
            ByVal utente As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByVal objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef StrSQL As System.Text.StringBuilder)

        'NB: utilizzato in lettura GeoJson

        StrSQL.AppendLine(" Select ")
        StrSQL.AppendLine("     gl.TipologiaLayer_Cod as 'TipologiaLayer_Cod' ")
        StrSQL.AppendLine("     , gl.TipologiaLayer_des as 'TipologiaLayer_des' ")
        StrSQL.AppendLine("     , tl.PivaSuperUser as 'PivaSuperUser' ")
        StrSQL.AppendLine("     , tl.Utente as 'Utente' ")
        StrSQL.AppendLine("     , tl.LayerElementiGrafici_Cod as 'LayerElementiGrafici_Cod' ")
        StrSQL.AppendLine("     , tl.Colore_Selezionato as 'Colore_Selezionato' ")
        StrSQL.AppendLine("     , tl.Colore_Primario as 'Colore_Primario' ")
        StrSQL.AppendLine("     , tl.Colore_Secondario as 'Colore_Secondario' ")
        StrSQL.AppendLine("     , coalesce(tl.icona16, 'x04_GenericoLayer_16.png') as 'Icona16' ")
        StrSQL.AppendLine("     , coalesce(tl.icona32, 'x04_GenericoLayer_32.png') as 'Icona32' ")
        StrSQL.AppendLine("     , tl.Varianza as 'Varianza' ")
        StrSQL.AppendLine("     , tl.Trasparenza as 'Trasparenza' ")
        StrSQL.AppendLine("     , tl.MostraDescrizioneAssociata as 'MostraDescrizioneAssociata' ")
        StrSQL.AppendLine("     , tl.ZIndex as 'ZIndex' ")
        StrSQL.AppendLine("     , tl.flag_visibile as 'flag_visibile' ")
        StrSQL.AppendLine("     , tl.flag_attivo as 'flag_attivo' ")
        '---
        'Colonne GIS_LayerElementiGrafici_Anagrafica
        '---
        'StrSQL.AppendLine("     , tla.Flag_Inserimento as 'Flag_Inserimento' ")
        'StrSQL.AppendLine("     , tla.Flag_Modifica as 'Flag_Modifica' ")
        'StrSQL.AppendLine("     , tla.Flag_Cancellazione as 'Flag_Cancellazione' ")
        'StrSQL.AppendLine("     , tla.Flag_Informazioni as 'Flag_Informazioni' ")
        '---
        StrSQL.AppendLine("     , TipoNodoAlberoAnagrafe.TipoNodoAlberoAnagrafe as 'TipoNodoAlberoAnagrafe' ")
        StrSQL.AppendLine("     , tl.LayerElementiGrafici_Des as 'LayerElementiGrafici_Des' ")
        StrSQL.AppendLine(" From GIS_TipologiaLayer gl ")
        'Join GIS_LayerElementiGrafici => tl
        StrSQL.AppendLine(" Left Join ")
        StrSQL.AppendLine("  ( ")
        StrSQL.AppendLine("     Select * ")
        StrSQL.AppendLine("     From GIS_LayerElementiGrafici tl ")
        StrSQL.AppendLine("     Where tl.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
        StrSQL.AppendLine("     and tl.Utente = '" & Agro_SQL_SaveText(IIf(utente = "", objparametri.UtenteUsername, utente)) & "'")
        StrSQL.AppendLine("  ) tl ")
        StrSQL.AppendLine("     On  gl.TipologiaLayer_Cod = tl.TipologiaLayer_Cod ")
        '---
        'Join GIS_LayerElementiGrafici_Anagrafica => tla
        '---
        'StrSQL.AppendLine(" Left Join GIS_LayerElementiGrafici_Anagrafica tla ")
        'StrSQL.AppendLine("           on  tla.PivaSuperUser = tl.PivaSuperUser ")
        'If IsLayerAppartenenzaImpianti(TipologiaLayer_Cod) Then
        '    StrSQL.AppendLine("           and tla.LayerElementiGrafici_Cod = '" & Agro_SQL_SaveText(enum_Gis_LayerElementiGrafici_std.IMPIANTI) & "' ")
        '    StrSQL.AppendLine("           and tla.TipologiaLayer_cod = '" & Agro_SQL_SaveText(enum_TipologiaLayer.Std) & "' ")
        'Else
        '    '-------------------------------------> CAST necessario per AgronicaSementi2013 dove GIS_LayerElementiGrafici.LayerElementiGrafici_Cod = varchar
        '    StrSQL.AppendLine("           and cast(tla.LayerElementiGrafici_Cod as varchar) = cast(tl.LayerElementiGrafici_Cod as varchar) ")
        '    '-------------------------------------<
        '    StrSQL.AppendLine("           and tla.TipologiaLayer_cod = tl.TipologiaLayer_cod ")
        'End If
        '---
        'Join GIS_LayerElementigrafici+GIS_TipoEntita => TipoNodoAlberoAnagrafe
        StrSQL.AppendLine("  Left Join ")
        StrSQL.AppendLine("  ( ")
        StrSQL.AppendLine("   Select tipi.LayerElementiGrafici_Cod, ")
        StrSQL.AppendLine("          tipi.Classi AS TipoNodoAlberoAnagrafe ")
        StrSQL.AppendLine("   from (select nodi.LayerElementiGrafici_Cod, ")
        StrSQL.AppendLine("                string_agg(cast(nodi.TipoNodoAlberoAnagrafe as varchar(100)) , ',') as Classi ")
        StrSQL.AppendLine("         from (select distinct g.LayerElementiGrafici_Cod, ")
        StrSQL.AppendLine("                               t.TipoNodoAlberoAnagrafe ")
        StrSQL.AppendLine("               from gis_layerelementigrafici g ")
        StrSQL.AppendLine("               inner join gis_tipoEntita t on g.LayerElementiGrafici_Cod = t.LayerElementiGrafici_Cod) nodi ")
        StrSQL.AppendLine("         group by nodi.LayerElementiGrafici_Cod ")
        StrSQL.AppendLine("     ) tipi ")
        StrSQL.AppendLine("  ) TipoNodoAlberoAnagrafe ")
        StrSQL.AppendLine("     On TipoNodoAlberoAnagrafe.LayerElementiGrafici_Cod = tl.LayerElementiGrafici_Cod ")
        'Where
        StrSQL.AppendLine("     where gl.TipologiaLayer_Cod = " & Agro_SQL_SaveNum(TipologiaLayer_Cod) & " ")
        If LayerElementiGrafici_Cod <> 0 Then
            StrSQL.AppendLine("     AND tl.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
        End If
        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
        End If
        StrSQL.AppendLine("     order by " & xOrderBy & " gl.tipologiaLayer_Cod, tl.LayerElementiGrafici_Cod")

    End Sub

    Public Function GetElencoAttributiLayer(ByVal PivaSuperUser As String,
                                             ByVal Utente As String,
                                             ByVal idLayer As Int32,
                                             ByVal TipologiaLayer_Cod As Int32,
                                             ByVal permessiWMS As Boolean,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.GetElencoAttributiLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Clear()
            StrSQL.AppendLine(" select distinct ")
            StrSQL.AppendLine(" LEG.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" , LEG.LayerElementiGrafici_Des ")
            StrSQL.AppendLine(" , LEGTO.GIS_TipoOggetto_Cod ")
            StrSQL.AppendLine(" , DS.LayerElementiGrafici_Etichetta ")
            StrSQL.AppendLine(" , DS.LayerElementiGrafici_TipoDato ")
            StrSQL.AppendLine(" , DS.TipologiaLayer_struct_cod  ")
            StrSQL.AppendLine(" , DS.CampoChiave  ")
            StrSQL.AppendLine(" , DS.EtichettaVisibile  ")
            StrSQL.AppendLine(" , ISNULL(LT.Attivo,0) Attivo ")
            StrSQL.AppendLine(" , ISNULL(EG.Bloccato, 0) Bloccato ")
            StrSQL.AppendLine(" from gis_layerelementigrafici LEG ")
            StrSQL.AppendLine(" left join GIS_LayerElementiGrafici_Anagrafica_DataStruct DS  ")
            StrSQL.AppendLine(" 	on DS.PivaSuperUser = LEG.PivaSuperUser ")
            StrSQL.AppendLine(" 	and DS.LayerElementiGrafici_Cod = LEG.LayerElementiGrafici_Cod  ")
            StrSQL.AppendLine(" 	and DS.TipologiaLayer_cod = LEG.TipologiaLayer_cod ")
            StrSQL.AppendLine(" left join GIS_LayerElementiGraficiXTipoOggetto LEGTO ")
            StrSQL.AppendLine(" 	on LEGTO.PivaSuperUser = LEG.PivaSuperUser ")
            StrSQL.AppendLine(" 	and LEGTO.LayerElementiGrafici_Cod = LEG.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" left join (select count(*) Attivo, LayerElementiGrafici_Cod, Utente, LayerTiles_Cod ")
            StrSQL.AppendLine(" 		from GIS_LayerTiles  ")
            StrSQL.AppendLine(" 		group by LayerElementiGrafici_Cod, Utente, LayerTiles_Cod) LT  ")
            StrSQL.AppendLine(" 	on LT.LayerElementiGrafici_Cod = LEG.LayerElementiGrafici_Cod  ")
            StrSQL.AppendLine(" 	and LT.Utente = LEG.Utente ")
            StrSQL.AppendLine(" 	and LT.LayerTiles_Cod = DS.TipologiaLayer_struct_Cod ")
            StrSQL.AppendLine(" left join (select count(*) Bloccato, LayerElementiGrafici_Cod, PivaSuperUser  ")
            StrSQL.AppendLine(" 		from GIS_ElementiGrafici  ")
            StrSQL.AppendLine(" 		group by LayerElementiGrafici_Cod, PivaSuperUser) EG ")
            StrSQL.AppendLine(" 	on EG.PivaSuperUser = LEG.PivaSuperUser ")
            StrSQL.AppendLine(" 	and EG.LayerElementiGrafici_Cod = LEG.LayerElementiGrafici_Cod  ")
            StrSQL.AppendLine(" where 1=1 ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" and LEG.PivaSuperUser='" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If
            If Utente <> "" Then
                StrSQL.AppendLine(" and LEG.Utente='" & Agro_SQL_SaveText(Utente) & "' ")
            End If

            If TipologiaLayer_Cod <> 0 Then
                StrSQL.AppendLine(" and LEG.tipologialayer_cod=" & Agro_SQL_SaveNum(TipologiaLayer_Cod) & " ")
                If TipologiaLayer_Cod = 1 And permessiWMS = False Then
                    If idLayer < 0 Then
                        Throw New Exception("L'utente non ha i permessi per gestire le impostazioni avanzate del layer desiderato")
                    End If
                End If

                StrSQL.AppendLine(" and LEG.LayerElementiGrafici_Cod=" & Agro_SQL_SaveNum(idLayer) & " ")

                If GIS_LayerElementiGrafici_Utility.LayerZeroNonAmmesso(TipologiaLayer_Cod) Then
                    StrSQL.AppendLine(" and LEG.LayerElementiGrafici_Cod<>0 ")
                End If
            End If

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function GetElencoTipologiaLayers(ByVal PivaSuperUser As String,
                                             ByVal Utente As String,
                                             ByVal TipologiaLayer_Cod As Int32,
                                             ByVal permessiWMS As Boolean,
                                             ByVal permessiDatiSatellitari As Boolean,
                                             ByVal permessiTecniciInCampo As Boolean,
                                             ByVal analisiProduttivita As Boolean,
                                             ByVal AggiungiLayer_Imprese As Boolean,
                                             ByVal AggiungiLayer_SpecieVegetali As Boolean,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri,
                                             Optional ByVal LeggiLayerNonVisibili As Boolean = False,
                                             Optional ByVal LeggiLayerNonAttivi As Boolean = False) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.GetElencoTipologiaLayers()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Clear()
            StrSQL.AppendLine("  select ")
            StrSQL.AppendLine("  	b.TipologiaLayer_cod, ")
            StrSQL.AppendLine("  	case when trans.LayerElementiGrafici_des is null OR trans.LayerElementiGrafici_des = '' then a.LayerElementiGrafici_des else trans.LayerElementiGrafici_des end as LayerElementiGrafici_des, ")
            StrSQL.AppendLine("  	a.LayerElementiGrafici_Cod, ")
            StrSQL.AppendLine("  	a.Colore_Primario as Colore_Primario_Layer, ")
            StrSQL.AppendLine("  	a.Colore_Secondario as Colore_Secondario_Layer, ")
            StrSQL.AppendLine("  	a.Varianza as Varianza_Layer, ")
            StrSQL.AppendLine("  	a.trasparenza, ")
            StrSQL.AppendLine("  	a.MostraDescrizioneAssociata, ")
            StrSQL.AppendLine("  	a.ZIndex, ")
            StrSQL.AppendLine("  	a.Icona16, ")
            StrSQL.AppendLine("  	a.Icona32, ")
            StrSQL.AppendLine("  	0 as TipoNodoAlberoAnagrafe, ")
            StrSQL.AppendLine("  	a.Flag_Visibile, ")
            StrSQL.AppendLine("  	a.Flag_Attivo, ")
            StrSQL.AppendLine("  	'1' as RaggruppaDescrizioneAssociata, ")
            StrSQL.AppendLine("	    case when permessi.Flag_Inserimento is null then c.Flag_Inserimento else permessi.Flag_Inserimento end as Flag_Inserimento,  ")
            StrSQL.AppendLine("	    case when permessi.Flag_Modifica is null then c.Flag_Modifica else permessi.Flag_Modifica end as Flag_Modifica,  ")
            StrSQL.AppendLine("	    case when permessi.Flag_Cancellazione is null then c.Flag_Cancellazione else permessi.Flag_Cancellazione end as Flag_Cancellazione,  ")
            StrSQL.AppendLine("	    case when permessi.Flag_Informazioni is null then c.Flag_Informazioni else permessi.Flag_Informazioni end as Flag_Informazioni, ")
            StrSQL.AppendLine("	    case when permessi.Flag_Amministrazione is null then 0 else permessi.Flag_Amministrazione end as Flag_Amministrazione, ")
            StrSQL.AppendLine("  	e.FeatureTypeId, ")
            StrSQL.AppendLine("  	t.LayerTiles_Des, ")
            StrSQL.AppendLine("  	t.LayerTiles_Cod, ")
            StrSQL.AppendLine("  	t.Colore_Primario as Colore_Primario_Tile, ")
            StrSQL.AppendLine("  	t.Colore_Secondario as Colore_Secondario_Tile, ")
            StrSQL.AppendLine("  	t.varianza as Varianza_Tile, ")
            StrSQL.AppendLine("  	'' as v_min, ")
            StrSQL.AppendLine("  	'' as v_max, ")
            StrSQL.AppendLine("  	'' as tilelayerpadre ")
            StrSQL.AppendLine("  from  ")
            StrSQL.AppendLine("  	gis_layerelementigrafici a inner join ")
            StrSQL.AppendLine("  	GIS_TipologiaLayer b ")
            StrSQL.AppendLine("  		on (a.TipologiaLayer_cod=b.TipologiaLayer_cod) ")
            StrSQL.AppendLine("  	left join gis_layerelementigrafici_anagrafica c ")
            StrSQL.AppendLine("  		on (a.PivaSuperUser=c.PivaSuperUser and ")
            '            StrSQL.AppendLine("  		cast(a.LayerElementiGrafici_Cod as varchar)=cast(c.LayerElementiGrafici_Cod as varchar) and ")
            StrSQL.AppendLine("  		a.LayerElementiGrafici_Cod = c.LayerElementiGrafici_Cod and ")
            StrSQL.AppendLine("  			 a.TipologiaLayer_cod = c.TipologiaLayer_cod) ")
            StrSQL.AppendLine("  	left join GIS_LayerElementiGrafici_Anagrafica_XLingue trans ")
            StrSQL.AppendLine("  		on (c.LayerElementiGrafici_Cod = trans.LayerElementiGrafici_Cod and ")
            StrSQL.AppendLine("  		     c.TipologiaLayer_cod = trans.TipologiaLayer_cod and ")
            StrSQL.AppendLine("  			 trans.Lingua_Cod = " & Agro_SQL_SaveNum(objParametri_Utenti.Lingua_Cod) & ") ")
            StrSQL.AppendLine("  	left join ( GIS_LayerElementiGraficiXTipoOggetto d ")
            StrSQL.AppendLine("  				inner join GIS_TipoOggetto e ")
            StrSQL.AppendLine("  				on (d.GIS_TipoOggetto_Cod=e.GIS_TipoOggetto_Cod)) ")
            StrSQL.AppendLine("  		on (a.PivaSuperUser=d.PivaSuperUser and ")
            StrSQL.AppendLine("  		a.LayerElementiGrafici_Cod=d.LayerElementiGrafici_Cod) ")
            StrSQL.AppendLine("  	left join GIS_LayerTiles t ")
            StrSQL.AppendLine("  		on (a.PivaSuperUser=t.PivaSuperUser and ")
            StrSQL.AppendLine("  		a.LayerElementiGrafici_Cod=t.LayerElementiGrafici_Cod and ")
            StrSQL.AppendLine("  		a.Utente=t.Utente and ")
            StrSQL.AppendLine("  		a.TipologiaLayer_cod=t.TipologiaLayer_cod) ")
            StrSQL.AppendLine("		left join ")
            StrSQL.AppendLine("			(select LayerElementiGrafici_Cod,utente,Max(Flag_Inserimento) as Flag_Inserimento,Max(Flag_Modifica) as Flag_Modifica,Max(Flag_Cancellazione) as Flag_Cancellazione,Max(Flag_Informazioni) as Flag_Informazioni,Max(Flag_Amministrazione) as Flag_Amministrazione  ")
            StrSQL.AppendLine("				from ( ")
            StrSQL.AppendLine("				select LayerElementiGrafici_Cod,utente,Flag_Inserimento,Flag_Modifica,Flag_Cancellazione,Flag_Informazioni,Flag_Amministrazione ")
            StrSQL.AppendLine("				from GIS_LayerElementiGraficiXUtente ")
            StrSQL.AppendLine("				union all ")
            StrSQL.AppendLine("				select LayerElementiGrafici_Cod,b.UserName,Flag_Inserimento,Flag_Modifica,Flag_Cancellazione,Flag_Informazioni,Flag_Amministrazione ")
            StrSQL.AppendLine("				from GIS_LayerElementiGraficiXGruppiUtente a inner join " & objParametri_Utenti.Recupera_NomeDB() & ".[dbo].[Utenti_xGruppi_Utente] b on (a.Gruppi_Utente_cod=b.Gruppi_Utente_cod) ")
            StrSQL.AppendLine("				) as list ")
            StrSQL.AppendLine("				group by LayerElementiGrafici_Cod,utente ")
            StrSQL.AppendLine("				) as permessi ")
            StrSQL.AppendLine("			on (a.LayerElementiGrafici_Cod=permessi.LayerElementiGrafici_Cod and a.Utente=permessi.Utente) ")
            StrSQL.AppendLine("  where 1=1 ")
            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" and a.PivaSuperUser='" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If
            If Utente <> "" Then
                StrSQL.AppendLine(" and a.Utente='" & Agro_SQL_SaveText(Utente) & "' ")
            End If
            If TipologiaLayer_Cod <> 0 Then
                StrSQL.AppendLine(" and a.tipologialayer_cod=" & Agro_SQL_SaveNum(TipologiaLayer_Cod) & " ")
                If TipologiaLayer_Cod = 1 Then 'And permessiWMS = False Then
                    StrSQL.AppendLine(" and ( a.LayerElementiGrafici_Cod >= 0 ")
                    If Not permessiTecniciInCampo Then
                        StrSQL.AppendLine(" and a.LayerElementiGrafici_Cod <> " & Agro_SQL_SaveNum(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Tecnici_in_campo) & " ")
                    End If
                    If Not analisiProduttivita Then
                        Dim elencoLayerProduttivita As List(Of Int32) = {CInt(enum_Gis_LayerElementiGrafici_std.Indice_CO2),
                                                                         CInt(enum_Gis_LayerElementiGrafici_std.Indice_Erosione),
                                                                         CInt(enum_Gis_LayerElementiGrafici_std.Indice_Rischio_Meteo),
                                                                         CInt(enum_Gis_LayerElementiGrafici_std.Indici_Rischio_Produttivita)}.ToList

                        StrSQL.AppendLine(" and a.LayerElementiGrafici_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", elencoLayerProduttivita)) & ") ")
                    End If
                    If permessiWMS Then
                        StrSQL.AppendLine(" or a.LayerElementiGrafici_Cod = -1 ")
                    End If
                    If permessiDatiSatellitari Then
                        StrSQL.AppendLine(" or a.LayerElementiGrafici_Cod = -2 ")
                    End If
                    StrSQL.AppendLine(" ) ")
                End If
                If GIS_LayerElementiGrafici_Utility.LayerZeroNonAmmesso(TipologiaLayer_Cod) Then
                    StrSQL.AppendLine(" and a.LayerElementiGrafici_Cod<>0 ")
                End If
            End If
            If LeggiLayerNonVisibili = False Then
                StrSQL.AppendLine(" and Flag_Visibile=1 ")
            End If
            If LeggiLayerNonAttivi = False Then
                StrSQL.AppendLine(" and Flag_Attivo=1 ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            StrSQL.AppendLine(" order by a.PivaSuperUser, a.TipologiaLayer_cod, a.LayerElementiGrafici_Cod, a.ZIndex, t.LayerTiles_Cod ")

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function

    
    Public Function LeggiTraduzioni(ByVal TipologiaLayer_Cod As Int32,
                                    ByRef LayerElementiGrafici_Cods As String(),
                                    ByRef objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.LeggiTraduzioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            ' Query per pprendere lingua e traduzione, anche se le traduzioni ancora non esistono
            Dim values = String.Join(",", LayerElementiGrafici_Cods.Select(Function(c) "(" & c & ")"))
            StrSQL.Clear()
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     l.Lingua_Cod, ")
            StrSQL.AppendLine("     l.Descrizione, ")
            StrSQL.AppendLine("     lv.LayerElementiGrafici_Cod, ")
            StrSQL.AppendLine("     a.LayerElementiGrafici_Des ")
            StrSQL.AppendLine(" FROM ")
            StrSQL.AppendLine("     Lingue l ")
            StrSQL.AppendLine(" JOIN ")
            StrSQL.AppendLine("     (VALUES " & values & ") AS lv(LayerElementiGrafici_Cod) ")
            StrSQL.AppendLine("     ON 1=1 ")
            StrSQL.AppendLine(" LEFT JOIN ")
            StrSQL.AppendLine("     GIS_LayerElementiGrafici_Anagrafica_XLingue a ")
            StrSQL.AppendLine("     ON a.Lingua_Cod = l.Lingua_Cod ")
            StrSQL.AppendLine("     AND a.LayerElementiGrafici_Cod = lv.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine("     AND a.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_Cod) & " ")
            StrSQL.AppendLine(" ORDER BY ")
            StrSQL.AppendLine("     l.Lingua_Cod, lv.LayerElementiGrafici_Cod ")

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function
    
    Public Function LeggiTraduzioniAttributoByLayerId(ByVal TipologiaLayer_Cod As Int32, 
                                    ByVal LayerElementiGrafici_Cod As String,
                                    ByVal linguaCod As Nullable(Of Int32),
                                    ByRef objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.LeggiTraduzioniEtichetta()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            ' Query per prendere lingua e traduzione, anche se le traduzioni ancora non esistono
            StrSQL.Clear()
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     l.Lingua_Cod, ")
            StrSQL.AppendLine("     l.Descrizione, ")
            StrSQL.AppendLine("     a.TipologiaLayer_struct_cod, ") 
            StrSQL.AppendLine("     a.LayerElementiGrafici_Etichetta as attributeName, ")
            StrSQL.AppendLine("     b.LayerElementiGrafici_Etichetta ")
            StrSQL.AppendLine(" FROM GIS_LayerElementiGrafici_Anagrafica_DataStruct a")
            StrSQL.AppendLine(" JOIN Lingue l ON 1 = 1 ")
            StrSQL.AppendLine(" LEFT JOIN ")
            StrSQL.AppendLine("     GIS_LayerElementiGrafici_Anagrafica_DataStruct_XLingue b ")
            StrSQL.AppendLine("     ON b.Lingua_Cod = l.Lingua_Cod ")
            StrSQL.AppendLine("     AND b.TipologiaLayer_struct_cod = a.TipologiaLayer_struct_cod ")
            StrSQL.AppendLine("     AND b.TipologiaLayer_cod = a.TipologiaLayer_cod ")
            StrSQL.AppendLine("     AND b.LayerElementiGrafici_Cod = a.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" WHERE a.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_Cod))
            StrSQL.AppendLine("     AND a.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            
            If Not linguaCod Is Nothing Then 
                StrSQL.AppendLine("     AND l.Lingua_Cod = " & Agro_SQL_SaveNum(linguaCod))
            End If
            
            StrSQL.AppendLine(" ORDER BY ")
            StrSQL.AppendLine("     l.Lingua_Cod, b.TipologiaLayer_struct_cod ")

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function
    
    
    Public Function LeggiTraduzioniAttributoByStructCode(
                                    ByVal TipologiaLayer_Cod As Int32, 
                                    ByVal LayerElementiGrafici_Cod As Int32, 
                                    ByVal linguaCod As Nullable(Of Int32),
                                    ByRef TipologiaLayer_struct_cods As String(),
                                    ByRef objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.LeggiTraduzioniEtichetta()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            ' Query per prendere lingua e traduzione, anche se le traduzioni ancora non esistono
            Dim values = String.Join(",", TipologiaLayer_struct_cods.Select(Function(c) "(" & c & ")"))
            StrSQL.Clear()
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     l.Lingua_Cod, ")
            StrSQL.AppendLine("     l.Descrizione, ")
            StrSQL.AppendLine("     lv.TipologiaLayer_struct_cod, ")
            StrSQL.AppendLine("     a.LayerElementiGrafici_Etichetta as attributeName, ")
            StrSQL.AppendLine("     b.LayerElementiGrafici_Etichetta")
            StrSQL.AppendLine(" FROM ")
            StrSQL.AppendLine("     Lingue l ")
            StrSQL.AppendLine(" JOIN ")
            StrSQL.AppendLine("     (VALUES " & values & ") AS lv(TipologiaLayer_struct_cod) ")
            StrSQL.AppendLine("     ON 1=1 ")
            StrSQL.AppendLine(" INNER JOIN ")
            StrSQL.AppendLine("     GIS_LayerElementiGrafici_Anagrafica_DataStruct a ")
            StrSQL.AppendLine("     ON a.TipologiaLayer_struct_cod = lv.TipologiaLayer_struct_cod ")
            StrSQL.AppendLine("     AND a.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            StrSQL.AppendLine("     AND a.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_Cod) & " ")
            StrSQL.AppendLine(" LEFT JOIN ")
            StrSQL.AppendLine("     GIS_LayerElementiGrafici_Anagrafica_DataStruct_XLingue b ")
            StrSQL.AppendLine("     ON b.Lingua_Cod = l.Lingua_Cod ")
            StrSQL.AppendLine("     AND b.TipologiaLayer_struct_cod = lv.TipologiaLayer_struct_cod ")
            StrSQL.AppendLine("     AND b.LayerElementiGrafici_Cod = a.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine("     AND b.TipologiaLayer_cod = a.TipologiaLayer_cod ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")
            
            If Not linguaCod Is Nothing Then 
                StrSQL.AppendLine("     AND l.Lingua_Cod = " & Agro_SQL_SaveNum(linguaCod))
            End If
            
            StrSQL.AppendLine(" ORDER BY ")
            StrSQL.AppendLine("     l.Lingua_Cod, b.TipologiaLayer_struct_cod ")

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function
#End Region

    'Public Function Leggi_x_utente( _
    '                        ByVal Utente As String, _
    '                        ByVal LayerElementiGrafici_Cod As Int32, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   LayerElementiGrafici_Cod = 0
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT  * ")
    '        StrSQL.Append(" FROM    GIS_LayerElementiGrafici ")
    '        StrSQL.Append(" WHERE   GIS_LayerElementiGrafici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '        StrSQL.Append(" AND     GIS_LayerElementiGrafici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


    '        StrSQL.Append(" AND GIS_LayerElementiGrafici.Utente = '" & Agro_SQL_SaveText(Utente) & "' ")

    '        If LayerElementiGrafici_Cod <> 0 Then
    '            StrSQL.Append(" AND GIS_LayerElementiGrafici.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
    '        End If

    '        '--------------------------------------------------------------------------
    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If
    '        '--------------------------------------------------------------------------
    '        Select Case objParametri.FlagVisibilita
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                StrSQL.Append(" AND   GIS_LayerElementiGrafici.Inviato >=0 ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                StrSQL.Append(" AND   GIS_LayerElementiGrafici.Inviato =-1 ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                '...................................
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '--------------------------------------------------------------------------
    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        End If



    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_LayerElementiGrafici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'Private Const base_color As Integer = 1111111 '16777216

    Private Const coloreBaseAgronica As String = "76BC21"

    Shared _layerSpecialeCatasto As String = CInt(enum_Gis_LayerElementiGrafici_speciali.Catasto).ToString()
    Shared _layerSpecialeImpianti As String = CInt(enum_Gis_LayerElementiGrafici_speciali.Impianti).ToString()

#Region "Gestione della colorazione automatica"
    Shared upperbound As Integer = 16777216
    Shared lowerbound As Integer = 1111111

    Private Shared Function SetBaseColorRND() As Integer
        Return CInt(Math.Floor((upperbound - lowerbound + 1) * Rnd())) + lowerbound
    End Function

    Private Shared Sub LeggiXGetQuery_Colore(
        ByVal utente As String,
        ByVal TipologiaLayer As Integer,
        ByVal startingColor As String,
        ByVal dense_rank_orderBY As String,
        ByRef stb As System.Text.StringBuilder,
        ByVal bPrimario As Boolean)

        Dim ascDesc As String = ""
        If Not bPrimario Then
            ascDesc = "Desc"
        End If

        Dim colore As String = "colore_primario"
        If Not bPrimario Then
            colore = "colore_secondario"
        End If

        Select Case TipologiaLayer

            Case enum_TipologiaLayer.SpecieVegetale

                stb.AppendLine(String.Format(
                               " , case when veg.veg_cod = {0} then isnull (({1}), '8a238a') else ",
                               _layerSpecialeCatasto,
                               QueryCalcolaColoreLayerCatasto(colore, utente)))

                AggiungiCalcoloColoreLayer(stb, "veg.veg_cod", "", startingColor, colore)

            Case enum_TipologiaLayer.Cultivar

                stb.AppendLine(String.Format(
                               " , case when cul.cul_cod = {0} then isnull (({1}), '8a238a') else ",
                               _layerSpecialeCatasto,
                               QueryCalcolaColoreLayerCatasto(colore, utente)))

                AggiungiCalcoloColoreLayer(stb, "cul.cul_cod", "", startingColor, colore)

            Case enum_TipologiaLayer.RilieviVegetoProduttivi

                If bPrimario Then
                    stb.AppendLine(String.Format(
                                   " , case when a.ind_mat_cod = {0} then isnull (({1}), '0000FF') else ",
                                   _layerSpecialeImpianti,
                                   QueryCalcolaColoreLayerImpianto(colore, utente)))
                Else
                    stb.AppendLine(String.Format(
                                   " , case when a.ind_mat_cod = {0} then null else ",
                                   _layerSpecialeImpianti))
                End If

                AggiungiCalcoloColoreLayer(stb, dense_rank_orderBY, ascDesc, startingColor, colore)

            Case enum_TipologiaLayer.Avversita

                If bPrimario Then
                    stb.AppendLine(String.Format(
                                   " , case when a.av_cod = {0} then isnull (({1}), '0000FF') else ",
                                   _layerSpecialeImpianti,
                                   QueryCalcolaColoreLayerImpianto(colore, utente)))
                Else
                    stb.AppendLine(String.Format(
                                   " , case when a.av_cod = {0} then null else ",
                                   _layerSpecialeImpianti))
                End If

                AggiungiCalcoloColoreLayer(stb, dense_rank_orderBY, ascDesc, startingColor, colore)

            Case Else

                stb.Append(" , ")
                AggiungiCalcoloColoreLayer(stb, dense_rank_orderBY, ascDesc, startingColor, colore, True)

        End Select

    End Sub

    Private Shared Function QueryCalcolaColoreLayerCatasto(ByVal colore As String, ByVal utente As String) As String

        Return String.Format("select top 1 {0} from GIS_LayerElementiGrafici gg " &
                             "where gg.Utente = '{1}' and gg.TipologiaLayer_cod = 1 and gg.LayerElementiGrafici_Cod = 3",
                             colore,
                             utente)

    End Function

    Private Shared Function QueryCalcolaColoreLayerImpianto(ByVal colore As String, ByVal utente As String) As String

        Return String.Format("select top 1 {0} from GIS_LayerElementiGrafici gg " &
                             "where gg.Utente = '{1}' and gg.TipologiaLayer_cod = 1 and gg.LayerElementiGrafici_Cod = 19",
                             colore,
                             utente)

    End Function

    Private Shared Sub AggiungiCalcoloColoreLayer(
        ByRef stb As StringBuilder,
        ByVal dense_rank_orderBY As String,
        ByVal ascDesc As String,
        ByVal startingColor As String,
        ByVal colore As String,
        Optional ByVal noEndCaseBeforeAs As Boolean = False)

        Dim endCaseBeforeAs As String = "end"
        If noEndCaseBeforeAs Then
            endCaseBeforeAs = ""
        End If

        stb.Append(" substring(convert(varchar(64), (convert(varbinary(3), " & vbCrLf)
        stb.Append(" (dense_rank() over (order by " & dense_rank_orderBY & " " & ascDesc & ")) " & vbCrLf)
        stb.Append(" * (cast('1' + right('0000', (5 - len( cast( (count(*) * 20) as varchar(5))))) as int)) " & vbCrLf)
        stb.Append(" * (select (" & startingColor & " / case when count(*) = 0 then 1 else count(*) end )) " & vbCrLf)
        stb.Append(" )) , 1), 3, 6) " & endCaseBeforeAs & " as " & colore & vbCrLf)

    End Sub

    Private Shared Sub LeggiXGetQuery_Colore(ByVal startingColor As String, ByVal tabella As String, ByVal clausolaWhere As String, ByVal nomeColonna As String, ByRef stb As System.Text.StringBuilder, Optional ByVal reverse As Boolean = False)

        Dim ascDesc As String = ""
        If reverse Then
            ascDesc = "Desc"
        End If

        stb.Append("                     , (Select colore" & ascDesc & vbCrLf)
        stb.Append("                        from  " & tabella & " k3" & vbCrLf)
        stb.Append("                        where  " & clausolaWhere & " " & vbCrLf)
        stb.Append("                    ) as '@" & nomeColonna & "'  " & vbCrLf)

    End Sub

    Private Shared Sub LeggiXGetQuery_Colore(ByVal startingColor As String, ByVal VariabileC As String, ByVal tabella As String, ByVal colonnaIntabella As String, ByVal nomeColonna As String, ByVal clausolaWhere As String, ByRef stb As System.Text.StringBuilder, Optional ByVal Reverse As Boolean = False)

        Dim ascDesc As String = ""
        If Reverse Then
            ascDesc = "Desc"
        End If

        stb.Append("                     , (Select substring(Convert(varchar(64), colore, 1), 3, 6)  " & vbCrLf)
        stb.Append("                     from (  " & vbCrLf)
        stb.Append("                     select (CONVERT(VARBINARY(3),(dense_rank() over (order by k3." & colonnaIntabella & " " & ascDesc & " )) * (cast( '1' +  right(  '0000', (5 - len( cast( (" & VariabileC & " * 20) as varchar(5)))))  as int)) *  " & vbCrLf)
        stb.Append("                     (select  (" & startingColor & " / " & VariabileC & "))  " & vbCrLf)
        stb.Append("                     )) as colore , " & colonnaIntabella & "  " & vbCrLf)
        stb.Append("                     from " & tabella & " k3  " & vbCrLf)
        stb.Append("                     ) s  " & vbCrLf)
        stb.Append("                     where(" & clausolaWhere & ")) as '@" & nomeColonna & "'  " & vbCrLf)

    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM_tiles(
            ByVal PivaSuperUser As String,
            ByRef Utente As String,
            ByVal TipologiaLayer_cod As Integer,
            ByVal xLayerElementiGrafici_Cod As String,
            ByVal xLayerTiles_Cod As String,
            ByVal xLayerTiles_Des As String,
            ByVal xVarianza As String,
            ByVal QueryPart As String,
            ByVal xLeggiColore_primario As String,
            ByVal xLeggiColore_secondario As String,
            ByRef stb As System.Text.StringBuilder
    )

        Dim dataCreazione As DateTime
        dataCreazione = Now

        stb.Append(" insert GIS_LayerTiles ( " & vbCrLf)

        stb.Append(" PivaSuperUser, " & vbCrLf)
        stb.Append(" LayerElementiGrafici_Cod, " & vbCrLf)
        stb.Append(" Utente, " & vbCrLf)
        stb.Append(" LayerTiles_Cod, " & vbCrLf)
        stb.Append(" LayerTiles_Des, " & vbCrLf)
        stb.Append(" TipologiaLayer_cod, " & vbCrLf)
        stb.Append(" Colore_Primario, " & vbCrLf)
        stb.Append(" Colore_Secondario, " & vbCrLf)
        stb.Append(" Colore_Base, " & vbCrLf)
        stb.Append(" Varianza, " & vbCrLf)
        stb.Append(" inviato, " & vbCrLf)
        stb.Append(" datainvio, " & vbCrLf)
        stb.Append(" Data_Creazione, " & vbCrLf)
        stb.Append(" Data_Modifica, " & vbCrLf)
        stb.Append(" Username_Creazione, " & vbCrLf)
        stb.Append(" Username_Modifica, " & vbCrLf)
        stb.Append(" Validita_Inizio, " & vbCrLf)
        stb.Append(" Validita_Fine " & vbCrLf)
        stb.Append("  ) " & vbCrLf)

        stb.Append(" Select distinct " & vbCrLf)
        stb.Append("      '" & Agro_SQL_SaveText(PivaSuperUser) & "' as pivasuperuser  " & vbCrLf)
        stb.Append("    , " & xLayerElementiGrafici_Cod & " as LayerElementiGrafici_Cod  " & vbCrLf)
        stb.Append("    , '" & Agro_SQL_SaveText(Utente) & "' as utente  " & vbCrLf)
        stb.Append("    , " & xLayerTiles_Cod & " as LayerTiles_Cod " & vbCrLf)
        stb.Append("    , " & xLayerTiles_Des & " as LayerTiles_Des " & vbCrLf)
        stb.Append("    , " & TipologiaLayer_cod & " as TipologiaLayer_cod " & vbCrLf)

        Dim base_color As String
        base_color = SetBaseColorRND().ToString

        If Not String.IsNullOrEmpty(xLeggiColore_primario) Then
            stb.Append(xLeggiColore_primario)
        Else

            LeggiXGetQuery_Colore(Utente, TipologiaLayer_cod, base_color, xLayerTiles_Cod, stb, True)
        End If

        If Not String.IsNullOrEmpty(xLeggiColore_secondario) Then
            stb.Append(xLeggiColore_secondario)
        Else
            LeggiXGetQuery_Colore(Utente, TipologiaLayer_cod, base_color, xLayerTiles_Cod, stb, False)
        End If

        stb.Append("  , '000000' as colore_base " & vbCrLf)
        stb.Append("  , " & xVarianza & " as varianza " & vbCrLf)

        stb.Append("  , 0 as inviato " & vbCrLf)
        stb.Append("  , null as DataInvio " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(dataCreazione) & " as DataCreazione " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(dataCreazione) & " as Datamodifica " & vbCrLf)
        stb.Append("  , '" & Agro_SQL_SaveText(Utente) & "' as username_creazione " & vbCrLf)
        stb.Append("  , '" & Agro_SQL_SaveText(Utente) & "' as username_modifica " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO) & " as validita_inizio " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE) & " as validita_fine " & vbCrLf)
        stb.Append(" ")

        stb.Append(QueryPart)

    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM_tilesDes(
    ByVal PivaSuperUser As String,
    ByRef Utente As String,
    ByVal TipologiaLayer_cod As Integer,
    ByVal xLayerElementiGrafici_Cod As String,
    ByVal xLayerTiles_Cod As String,
    ByVal xLayerTilesDescrizione_Cod As String,
    ByVal xLayerTilesDescrizione_Des As String,
    ByVal xValore_associato As String,
    ByVal xColore_Base As String,
    ByVal fromQueryPart As String,
    ByRef stbInsert As System.Text.StringBuilder)

        Dim dataCreazione As DateTime
        dataCreazione = Now

        stbInsert.Append(" insert GIS_LayerTilesDescrizione ( " & vbCrLf)

        stbInsert.Append(" PivaSuperUser, " & vbCrLf)
        stbInsert.Append(" LayerElementiGrafici_Cod, " & vbCrLf)
        stbInsert.Append(" Utente, " & vbCrLf)
        stbInsert.Append(" LayerTiles_Cod, " & vbCrLf)
        stbInsert.Append(" LayerTilesDescrizione_Cod, " & vbCrLf)
        stbInsert.Append(" LayerTilesDescrizione_Des, " & vbCrLf)
        stbInsert.Append(" Colore_Base, " & vbCrLf)
        stbInsert.Append(" inviato, " & vbCrLf)
        stbInsert.Append(" datainvio, " & vbCrLf)
        stbInsert.Append(" Data_Creazione, " & vbCrLf)
        stbInsert.Append(" Data_Modifica, " & vbCrLf)
        stbInsert.Append(" Username_Creazione, " & vbCrLf)
        stbInsert.Append(" Username_Modifica, " & vbCrLf)
        stbInsert.Append(" Validita_Inizio, " & vbCrLf)
        stbInsert.Append(" Validita_Fine, " & vbCrLf)
        stbInsert.Append(" TipologiaLayer_cod, " & vbCrLf)
        stbInsert.Append(" Valore_a, " & vbCrLf)
        stbInsert.Append(" Valore_da, " & vbCrLf)
        stbInsert.Append(" Valore_associato " & vbCrLf)
        stbInsert.Append("  ) " & vbCrLf)

        stbInsert.Append(" Select distinct " & vbCrLf)
        stbInsert.Append("      '" & Agro_SQL_SaveText(PivaSuperUser) & "' as pivasuperuser  " & vbCrLf)
        stbInsert.Append("    , " & xLayerElementiGrafici_Cod & " as LayerElementiGrafici_Cod  " & vbCrLf)
        stbInsert.Append("    , '" & Agro_SQL_SaveText(Utente) & "' as utente  " & vbCrLf)
        stbInsert.Append("    , " & xLayerTiles_Cod & " as LayerTiles_Cod " & vbCrLf)
        stbInsert.Append("    , " & xLayerTilesDescrizione_Cod & " as LayerTilesDescrizione_Cod " & vbCrLf)
        stbInsert.Append("    , " & xLayerTilesDescrizione_Des & " as LayerTilesDescrizione_Des " & vbCrLf)

        If String.IsNullOrEmpty(xColore_Base) Then
            Const formula_Colore_Base =
                "Format(ABS(CHECKSUM(NewId())) % 256,'x2') + " &
                "Format(ABS(CHECKSUM(NewId())) % 256,'x2') + " &
                "Format(ABS(CHECKSUM(NewId())) % 256,'x2')"
            stbInsert.Append("  , " + formula_Colore_Base + " as colore_base " & vbCrLf)
        Else
            stbInsert.Append("  , '" + Agro_SQL_SaveText(xColore_Base) + "' as colore_base " & vbCrLf)
        End If

        stbInsert.Append("  , 0 as inviato " & vbCrLf)
        stbInsert.Append("  , null as DataInvio " & vbCrLf)
        stbInsert.Append("  , " & Agro_SQL_SaveDate(dataCreazione) & " as DataCreazione " & vbCrLf)
        stbInsert.Append("  , " & Agro_SQL_SaveDate(dataCreazione) & " as Datamodifica " & vbCrLf)
        stbInsert.Append("  , '" & Agro_SQL_SaveText(Utente) & "' as username_creazione " & vbCrLf)
        stbInsert.Append("  , '" & Agro_SQL_SaveText(Utente) & "' as username_modifica " & vbCrLf)
        stbInsert.Append("  , " & Agro_SQL_SaveDate(AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO) & " as validita_inizio " & vbCrLf)
        stbInsert.Append("  , " & Agro_SQL_SaveDate(AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE) & " as validita_fine " & vbCrLf)
        stbInsert.Append("  , " & TipologiaLayer_cod & " as TipologiaLayer_cod " & vbCrLf)
        stbInsert.Append("  , 0 as Valore_a " & vbCrLf)
        stbInsert.Append("  , 0 as Valore_da " & vbCrLf)
        stbInsert.Append("  , " & xValore_associato & " as Valore_associato " & vbCrLf)

        stbInsert.Append(fromQueryPart & vbCrLf)

        stbInsert.Append(";" & vbCrLf)

        'Aggiornamento sequenza tabelle

        stbInsert.Append(vbCrLf)
        stbInsert.Append("update Sequenza_Tabelle set ultimo_valore = " & vbCrLf)
        stbInsert.Append("ISNULL((select max(LayerTilesDescrizione_Cod) from GIS_LayerTilesDescrizione), 0) " & vbCrLf)
        stbInsert.Append("where Nome_Tabella = 'GIS_LayerTilesDescrizione' " & vbCrLf)

    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            ByVal PivaSuperUser As String,
            ByRef Utente As String,
            ByVal TipologiaLayer_cod As Integer,
            ByVal xLayerElementiGrafici_Cod As String,
            ByVal xLayerElementiGrafici_Des As String,
            ByVal Varianza As Integer,
            ByVal icona16 As String,
            ByVal icona32 As String,
            ByVal QueryPart As String,
            ByVal QueryTempATable As String,
            ByVal proponi_colore_Secondario As Boolean,
            ByRef stb As System.Text.StringBuilder
    )

        Dim dataCreazione As DateTime
        dataCreazione = Now

        If Not QueryTempATable.Trim.Equals("") Then
            stb.Append(QueryTempATable)
            stb.Append("")
        End If

        stb.Append(" insert GIS_LayerElementiGrafici ( " & vbCrLf)

        stb.Append(" PivaSuperUser, " & vbCrLf)
        stb.Append(" Utente, " & vbCrLf)
        stb.Append(" LayerElementiGrafici_Cod, " & vbCrLf)
        stb.Append(" LayerElementiGrafici_Des, " & vbCrLf)
        stb.Append(" Flag_Attivo, " & vbCrLf)
        stb.Append(" Flag_Visibile, " & vbCrLf)
        stb.Append(" Colore_Base, " & vbCrLf)
        stb.Append(" Colore_Selezionato, " & vbCrLf)
        stb.Append(" Colore_Primario, " & vbCrLf)
        stb.Append(" Colore_Secondario, " & vbCrLf)
        stb.Append(" Varianza, " & vbCrLf)
        stb.Append(" TipologiaLayer_cod, " & vbCrLf)
        stb.Append(" ZIndex, " & vbCrLf)
        stb.Append(" icona16, " & vbCrLf)
        stb.Append(" icona32, " & vbCrLf)
        stb.Append(" inviato, " & vbCrLf)
        stb.Append(" datainvio, " & vbCrLf)
        stb.Append(" Data_Creazione, " & vbCrLf)
        stb.Append(" Data_Modifica, " & vbCrLf)
        stb.Append(" Username_Creazione, " & vbCrLf)
        stb.Append(" Username_Modifica, " & vbCrLf)
        stb.Append(" Validita_Inizio, " & vbCrLf)
        stb.Append(" Validita_Fine, " & vbCrLf)
        stb.Append(" Trasparenza, " & vbCrLf)
        stb.Append(" MostraDescrizioneAssociata " & vbCrLf)
        stb.Append("  ) " & vbCrLf)

        stb.Append(" Select distinct " & vbCrLf)
        stb.Append("      '" & Agro_SQL_SaveText(PivaSuperUser) & "' as pivasuperuser  " & vbCrLf)
        stb.Append("    , '" & Agro_SQL_SaveText(Utente) & "' as utente  " & vbCrLf)
        stb.Append("    , " & xLayerElementiGrafici_Cod & " as LayerElementiGrafici_Cod " & vbCrLf)
        stb.Append("    , " & xLayerElementiGrafici_Des & " as LayerElementiGrafici_Des " & vbCrLf)
        stb.Append("    , 1 as Flag_Attivo " & vbCrLf)
        stb.Append("    , 1 as Flag_Visibile " & vbCrLf)
        stb.Append("    , null as colore_base " & vbCrLf)
        stb.Append("    , null as colore_selezionato     " & vbCrLf)

        Dim base_color As String
        base_color = SetBaseColorRND().ToString

        LeggiXGetQuery_Colore(Utente, TipologiaLayer_cod, base_color, xLayerElementiGrafici_Cod, stb, True)

        If proponi_colore_Secondario Then
            LeggiXGetQuery_Colore(Utente, TipologiaLayer_cod, base_color, xLayerElementiGrafici_Cod, stb, False)
        Else
            stb.Append("  , null as colore_secondario " & vbCrLf)
        End If

        Select Case TipologiaLayer_cod
            Case enum_TipologiaLayer.SpecieVegetale

                stb.Append("  , " & Varianza & " as varianza " & vbCrLf)
                stb.Append("  , " & TipologiaLayer_cod & " as tipologia_layer_Cod " & vbCrLf)


                stb.Append("  , case when veg.veg_Cod = " & _layerSpecialeCatasto & " then -999 else 999 end as Zindex  " & vbCrLf)
                stb.Append("  , case when veg.veg_cod = " & _layerSpecialeCatasto & " then 'Catasto16.png' else " & icona16.TrimStart(" ").TrimStart(",") & " end as icona16  " & vbCrLf)
                stb.Append("  , case when veg.veg_cod = " & _layerSpecialeCatasto & " then 'Catasto32.png' else " & icona32.TrimStart(" ").TrimStart(",") & " end as icona32  " & vbCrLf)

            Case enum_TipologiaLayer.Cultivar

                stb.Append("  , " & Varianza & " as varianza " & vbCrLf)
                stb.Append("  , " & TipologiaLayer_cod & " as tipologia_layer_Cod " & vbCrLf)


                stb.Append("  , case when cul.cul_Cod = " & _layerSpecialeCatasto & " then -999 else 999 end as Zindex  " & vbCrLf)
                stb.Append("  , case when cul.cul_cod = " & _layerSpecialeCatasto & " then 'Catasto16.png' else " & icona16.TrimStart(" ").TrimStart(",") & " end as icona16  " & vbCrLf)
                stb.Append("  , case when cul.cul_cod = " & _layerSpecialeCatasto & " then 'Catasto32.png' else " & icona32.TrimStart(" ").TrimStart(",") & " end as icona32  " & vbCrLf)

            Case enum_TipologiaLayer.RilieviVegetoProduttivi

                stb.Append("  , case when a.Ind_Mat_Cod = " & _layerSpecialeImpianti & " then 1 else " & Varianza & " end  as varianza " & vbCrLf)
                stb.Append("  , " & TipologiaLayer_cod & " as tipologia_layer_Cod " & vbCrLf)

                stb.Append("  , case when a.IND_MAT_COD = " & _layerSpecialeImpianti & " then -999 else 999 end as Zindex  " & vbCrLf)
                stb.Append("  , case when a.IND_MAT_COD = " & _layerSpecialeImpianti & " then 'Impianto16.png' else " & icona16.TrimStart(" ").TrimStart(",") & " end as icona16  " & vbCrLf)
                stb.Append("  , case when a.IND_MAT_COD = " & _layerSpecialeImpianti & " then 'Impianto32.png' else " & icona32.TrimStart(" ").TrimStart(",") & " end as icona32  " & vbCrLf)

            Case enum_TipologiaLayer.Avversita

                stb.Append("  , case when a.av_cod = " & _layerSpecialeImpianti & " then 1 else " & Varianza & " end  as varianza " & vbCrLf)
                stb.Append("  , " & TipologiaLayer_cod & " as tipologia_layer_Cod " & vbCrLf)

                stb.Append("  , case when a.av_cod = " & _layerSpecialeImpianti & " then -999 else 999 end as Zindex  " & vbCrLf)
                stb.Append("  , case when a.av_cod = " & _layerSpecialeImpianti & " then 'Impianto16.png' else " & icona16.TrimStart(" ").TrimStart(",") & " end as icona16  " & vbCrLf)
                stb.Append("  , case when a.av_cod = " & _layerSpecialeImpianti & " then 'Impianto32.png' else " & icona32.TrimStart(" ").TrimStart(",") & " end as icona32  " & vbCrLf)

            Case Else

                stb.Append("  , " & Varianza & " as varianza " & vbCrLf)
                stb.Append("  , " & TipologiaLayer_cod & " as tipologia_layer_Cod " & vbCrLf)

                stb.Append("  , 999 as Zindex " & vbCrLf)
                stb.Append(icona16 & " as icona16 " & vbCrLf)
                stb.Append(icona32 & " as icona32 " & vbCrLf)

        End Select

        stb.Append("  , 0 as inviato " & vbCrLf)
        stb.Append("  , null as DataInvio " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(dataCreazione) & " as DataCreazione " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(dataCreazione) & " as Datamodifica " & vbCrLf)
        stb.Append("  , '" & Agro_SQL_SaveText(Utente) & "' as username_creazione " & vbCrLf)
        stb.Append("  , '" & Agro_SQL_SaveText(Utente) & "' as username_modifica " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO) & " as validita_inizio " & vbCrLf)
        stb.Append("  , " & Agro_SQL_SaveDate(AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE) & " as validita_fine " & vbCrLf)

        Select Case TipologiaLayer_cod

            Case enum_TipologiaLayer.SpecieVegetale
                stb.Append(", case when veg.veg_cod = " & _layerSpecialeCatasto & " then isnull( (select top 1 trasparenza from GIS_LayerElementiGrafici gg where gg.Utente = '" & Agro_SQL_SaveText(Utente) & "' and gg.TipologiaLayer_cod = 1 and gg.LayerElementiGrafici_Cod = 3), 0.6) else 0.6 end as Varianza " & vbCrLf)

            Case enum_TipologiaLayer.Cultivar
                stb.Append(", case when cul.cul_cod = " & _layerSpecialeCatasto & " then isnull( (select top 1 trasparenza from GIS_LayerElementiGrafici gg where gg.Utente = '" & Agro_SQL_SaveText(Utente) & "' and gg.TipologiaLayer_cod = 1 and gg.LayerElementiGrafici_Cod = 3), 0.6) else 0.6 end as Varianza " & vbCrLf)

            Case enum_TipologiaLayer.RilieviVegetoProduttivi
                stb.Append(", case when a.IND_MAT_COD = " & _layerSpecialeImpianti & " then 0 else 0.6 end as trasparenza " & vbCrLf)

            Case enum_TipologiaLayer.Avversita
                stb.Append(", case when a.av_cod = " & _layerSpecialeImpianti & " then 0 else 0.6 end as trasparenza " & vbCrLf)

            Case Else
                stb.Append(", 0.6 as trasparenza " & vbCrLf)

        End Select

        'vanni, 01/03/2018: al momento la descrizione associata va in default su tutti i layer del tipo specie vegetali e sul layer impianti 
        'vanni, 30/03/2018: baco su xLayerElementiGrafici_Cod, è una stringa, non un identificativo di codice layer, imposto solo 5 poichè la routine non viene mai chiamata su tipologia 1!
        If TipologiaLayer_cod = 5 Then
            stb.Append(" , 1 as MostraDescrizioneAssociata ")
        Else
            stb.Append(" , 0 as MostraDescrizioneAssociata ")
        End If
        stb.Append(" ")

        stb.Append(QueryPart)

        If Not QueryTempATable.Trim.Equals("") Then
            stb.Append("If OBJECT_ID('tempdb..#ATABLE') IS NOT NULL DROP TABLE #ATABLE")
        End If

    End Sub
#End Region

#Region "Funzioni che colorano in automatico"

    Private Shared Sub LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles_DEL(ByVal Utente As String, ByVal stb As System.Text.StringBuilder)
        stb.Append("    delete  tt  " & vbCrLf)
        stb.Append(" from GIS_LayerTiles tt " & vbCrLf)
        stb.Append("    inner join gis_layerElementiGrafici g " & vbCrLf)
        stb.Append("        on tt.layerElementiGrafici_cod = g.layerElementiGrafici_cod " & vbCrLf)
        stb.Append("        and g.Utente = tt.Utente  " & vbCrLf)
        stb.Append(" where tt.utente = '" & Utente & "' " & vbCrLf)
        stb.Append(" and g.TipologiaLayer_cod = 7 " & vbCrLf)
        stb.Append(" ")
    End Sub

    Public Function GeneraColoriAutomatici(ByVal PivaSuperUser As String, ByVal Utente As String, ByVal TipologiaLayer As enum_TipologiaLayer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.GeneraColoriAutomatici()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

            Select Case TipologiaLayer

                Case enum_TipologiaLayer.Std

                Case enum_TipologiaLayer.SpecieVegetale

                    LeggiXGetQuery_GeneraColoriAutomatici_SpecieVegetali(PivaSuperUser, Utente, stb)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                Case enum_TipologiaLayer.Cultivar

                    LeggiXGetQuery_GeneraColoriAutomatici_Cultivar(PivaSuperUser, Utente, stb)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                Case enum_TipologiaLayer.Organizzazione

                Case enum_TipologiaLayer.Avversita

                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean

                    LeggiXGetQuery_GeneraColoriAutomatici_Avversita(PivaSuperUser, "ricette_dettaglio_Tecnico", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_Avversita(PivaSuperUser, "Mov_Dettaglio_Tecnico", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_Avversita(PivaSuperUser, "DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    'stb.Length = 0
                    'LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles_DEL(Utente, stb)
                    ''--------------------------------------------------------------------------
                    'xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    'xRispFinal = (xRispFinal And xRispQry)
                    ''--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles(PivaSuperUser, "ricette_dettaglio_Tecnico", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles(PivaSuperUser, "Mov_Dettaglio_Tecnico", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles(PivaSuperUser, "DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    xRisp = xRispFinal

                Case enum_TipologiaLayer.Fenologia

                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean

                    LeggiXGetQuery_GeneraColoriAutomatici_Fenologia(PivaSuperUser, "Ricette_Dettaglio_Tecnico", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_Fenologia(PivaSuperUser, "Mov_Dettaglio_Tecnico", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    xRisp = xRispFinal

                Case enum_TipologiaLayer.RilieviVegetoProduttivi

                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean

                    LeggiXGetQuery_GeneraColoriAutomatici_RilieviVegetoProduttivi(PivaSuperUser, "Mov_Dettaglio_Tecnico", Utente, stb)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_IndiciMaturitaTiles(PivaSuperUser, Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)

                    xRisp = xRispFinal

                Case enum_TipologiaLayer.AnalisiPeriodicitaAgenda

                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean

                    LeggiXGetQuery_GeneraColoriAutomatici_AnalisiPeriodicitaAgenda(PivaSuperUser, Utente, stb)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------

                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AnalisiPeriodicitaAgendaTiles(PivaSuperUser, Utente, stb)
                    '--------------------------------------------------------------------------
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)

                    xRisp = xRispFinal

            End Select

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Commit & Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

    Public Function GeneraColoriAutomatici2(
        ByVal PivaSuperUser As String,
        ByVal Utente As String,
        ByVal TipologiaLayer As enum_TipologiaLayer,
        ByRef objParametri As AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.GeneraColoriAutomatici2()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Const tabellaRicette = "Ricette_Dettaglio_Tecnico"
        Const tabellaMovimenti = "Mov_Dettaglio_Tecnico"
        Const tabellaDssMeteo = "DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV"

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Utility.VerificaApriTransazione(objParametri, FlagConnessioneLocale, FlagTransazioneLocale)

            Select Case TipologiaLayer

                Case enum_TipologiaLayer.Std

                Case enum_TipologiaLayer.SpecieVegetale
                    LeggiXGetQuery_GeneraColoriAutomatici_SpecieVegetali(PivaSuperUser, Utente, stb)
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)

                Case enum_TipologiaLayer.Cultivar
                    LeggiXGetQuery_GeneraColoriAutomatici_Cultivar(PivaSuperUser, Utente, stb)
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)

                Case enum_TipologiaLayer.Organizzazione

                Case enum_TipologiaLayer.Avversita
                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean
                    '--------------------------------------------------------------------------------
                    'Colori Layers
                    '--------------------------------------------------------------------------------
                    'Ricette
                    LeggiXGetQuery_GeneraColoriAutomatici_Avversita(PivaSuperUser, tabellaRicette, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'Dettaglio Tecnico
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_Avversita(PivaSuperUser, tabellaMovimenti, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'DSS Meteo
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_Avversita(PivaSuperUser, tabellaDssMeteo, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------------
                    'Tiles: Colori e Descrizioni
                    '--------------------------------------------------------------------------------
                    'Apertura sequenza tabelle per descrizioni
                    Dim objSeqTab As New AgronicaCoreDataProvider.Agro_Sequenze
                    objSeqTab.Apertura_Sequenza_Tabella("GIS_LayerTilesDescrizione",
                                                        0,
                                                        2000000000,
                                                        objParametri)
                    'Ricette - Colori
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles(PivaSuperUser, tabellaRicette, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'Ricette - Descrizioni
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTilesDes(PivaSuperUser, tabellaRicette, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'DettaglioTecnico - Colori
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles(PivaSuperUser, tabellaMovimenti, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'DettaglioTecnico - Descrizioni
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTilesDes(PivaSuperUser, tabellaMovimenti, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'DssMeteo - Colori
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles(PivaSuperUser, tabellaDssMeteo, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'DssMeteo - Descrizioni
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AvvTilesDes(PivaSuperUser, tabellaDssMeteo, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '---
                    xRisp = xRispFinal

                Case enum_TipologiaLayer.Fenologia
                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean
                    'Ricette
                    LeggiXGetQuery_GeneraColoriAutomatici_StadiCrescita(PivaSuperUser, tabellaRicette, Utente, stb)
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    'Dettaglio Tecnico
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_StadiCrescita(PivaSuperUser, tabellaMovimenti, Utente, stb)
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '---
                    xRisp = xRispFinal

                Case enum_TipologiaLayer.RilieviVegetoProduttivi
                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean
                    '--------------------------------------------------------------------------------
                    'Colori Layers
                    '--------------------------------------------------------------------------------
                    LeggiXGetQuery_GeneraColoriAutomatici_RilieviVegetoProduttivi(PivaSuperUser, tabellaMovimenti, Utente, stb)
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------------
                    'Colori Tiles
                    '--------------------------------------------------------------------------------
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_IndiciMaturitaTiles(PivaSuperUser, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '---
                    xRisp = xRispFinal

                Case enum_TipologiaLayer.AnalisiPeriodicitaAgenda
                    Dim xRispFinal As Boolean = True
                    Dim xRispQry As Boolean
                    '--------------------------------------------------------------------------------
                    'Colori Layers
                    '--------------------------------------------------------------------------------
                    LeggiXGetQuery_GeneraColoriAutomatici_AnalisiPeriodicitaAgenda(PivaSuperUser, Utente, stb)
                    xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '--------------------------------------------------------------------------------
                    'Colori Tiles
                    '--------------------------------------------------------------------------------
                    stb.Length = 0
                    LeggiXGetQuery_GeneraColoriAutomatici_AnalisiPeriodicitaAgendaTiles(PivaSuperUser, Utente, stb)
                    xRispQry = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
                    xRispFinal = (xRispFinal And xRispQry)
                    '---
                    xRisp = xRispFinal

            End Select

            Utility.VerificaChiudiTransazione(objParametri, FlagTransazioneLocale)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, FlagTransazioneLocale)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, FlagConnessioneLocale)

        End Try

        Return xRisp

    End Function

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_SpecieVegetali(ByVal PivaSuperUser As String, ByRef Utente As String, ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.Append(" " & vbCrLf)
        stb1.Append(" from ( " & vbCrLf)
        stb1.Append("    SELECT DISTINCT Cultivar.Veg_Cod " & vbCrLf)
        stb1.Append("    FROM         Reg_Impianti r1 INNER JOIN " & vbCrLf)
        stb1.Append("        Cultivar ON r1.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
        stb1.Append("    UNION " & vbCrLf)
        stb1.Append("    SELECT DISTINCT Cultivar.Veg_Cod " & vbCrLf)
        stb1.Append("    FROM         Programmazione_Entita INNER JOIN " & vbCrLf)
        stb1.Append("    Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
        stb1.Append("    UNION  " & vbCrLf)
        stb1.Append("    SELECT " & _layerSpecialeCatasto & " AS Veg_Cod  " & vbCrLf)
        stb1.Append("    UNION  " & vbCrLf)
        stb1.Append("    SELECT 0 AS Veg_Cod  " & vbCrLf)

        stb1.Append(" ) a " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("        inner join (select veg.veg_cod, veg.veg_des from SpecieVegetali veg union " & vbCrLf)
        stb1.Append("                    select 0 as veg_cod, 'Nessuna coltura' as veg_des union " & vbCrLf)
        stb1.Append("                    select " & _layerSpecialeCatasto & " as veg_cod, 'Catasto' as veg_des) veg " & vbCrLf)
        stb1.Append("    on veg.veg_cod = a.veg_cod " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("    Left Join  " & vbCrLf)
        stb1.Append("         GIS_LayerElementiGrafici gl " & vbCrLf)
        stb1.Append("        on gl.TipologiaLayer_cod =  5 " & vbCrLf)
        stb1.Append("        and gl.LayerElementiGrafici_Cod = a.veg_cod " & vbCrLf)
        stb1.Append("        and gl.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("  where gl.layerElementiGrafici_Cod is null " & vbCrLf)

        stb1.Append("  group by a.veg_cod, veg.Veg_Cod, veg.Veg_Des " & vbCrLf)
        stb1.Append(" ")

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.SpecieVegetale,
            "veg.veg_cod ",
            "veg.veg_des ",
            1,
            "  ,  right( '0000000' + cast(veg.veg_cod as varchar(1000)), 7) + '.ico'",
            "  ,  right( '0000000' + cast(veg.veg_cod as varchar(1000)), 7) + '.ico'",
            stb1.ToString,
            "",
            False,
            stb
        )
        '----- fine  specie vegetali

    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_Cultivar(ByVal PivaSuperUser As String, ByRef Utente As String, ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.Append(" " & vbCrLf)
        stb1.Append("from( " & vbCrLf)
        stb1.Append("   SELECT DISTINCT Cultivar.cul_Cod FROM Reg_Impianti r1 " & vbCrLf)
        stb1.Append("   INNER JOIN Cultivar ON r1.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
        stb1.Append("   UNION " & vbCrLf)
        stb1.Append("   SELECT DISTINCT Cultivar.cul_Cod FROM Programmazione_Entita  " & vbCrLf)
        stb1.Append("   INNER JOIN Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
        stb1.Append("   UNION " & vbCrLf)
        stb1.Append("   SELECT " & _layerSpecialeCatasto & " AS cul_Cod  " & vbCrLf)
        stb1.Append("   UNION" & vbCrLf)
        stb1.Append("   SELECT 0 AS cul_Cod  " & vbCrLf)
        stb1.Append(") a " & vbCrLf)

        stb1.Append("INNER JOIN (" & vbCrLf)
        stb1.Append("   SELECT cul.cul_cod, cul.veg_cod, sv.veg_des + ' -  ' + cul.cul_des as veg_cul_des" & vbCrLf)
        stb1.Append("   FROM cultivar cul " & vbCrLf)
        stb1.Append("   INNER JOIN SpecieVegetali sv " & vbCrLf)
        stb1.Append("   ON sv.veg_cod = cul.veg_cod " & vbCrLf)
        stb1.Append("   UNION " & vbCrLf)
        stb1.Append("   SELECT 0 As cul_cod, 0 as veg_cod, 'Nessuna coltura' as veg_cul_des " & vbCrLf)
        stb1.Append("   UNION" & vbCrLf)
        stb1.Append("   SELECT " & _layerSpecialeCatasto & " As cul_cod, " & _layerSpecialeCatasto & " as veg_cod, 'Catasto' as veg_cul_des " & vbCrLf)
        stb1.Append(") cul " & vbCrLf)
        stb1.Append("ON cul.cul_cod = a.cul_cod " & vbCrLf)

        stb1.Append("LEFT JOIN GIS_LayerElementiGrafici gl " & vbCrLf)
        stb1.Append("ON gl.TipologiaLayer_cod =  15 " & vbCrLf)
        stb1.Append("   AND gl.LayerElementiGrafici_Cod = a.cul_cod " & vbCrLf)
        stb1.Append("   AND gl.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("WHERE gl.layerElementiGrafici_Cod IS NULL " & vbCrLf)

        stb1.Append("GROUP BY a.cul_cod, cul.cul_Cod, cul.veg_cod, cul.veg_cul_Des " & vbCrLf)
        stb1.Append(" ")

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.Cultivar,
            "cul.cul_cod ",
            "cul.veg_cul_des ",
            1,
            "  ,  right( '0000000' + cast(cul.veg_cod as varchar(1000)), 7) + '.ico'",
            "  ,  right( '0000000' + cast(cul.veg_cod as varchar(1000)), 7) + '.ico'",
            stb1.ToString,
            "",
            False,
            stb
        )
        '----- fine  cultivar

    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_Avversita(ByVal PivaSuperUser As String, ByVal tabellaLetturaAV As String, ByRef Utente As String, ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.Append("  from #ATABLE a " & vbCrLf)
        stb1.Append("  left  join GIS_LayerElementiGrafici gl on gl.TipologiaLayer_cod = 7 " & vbCrLf)
        stb1.Append("                                        and gl.LayerElementiGrafici_Cod = a.av_cod " & vbCrLf)
        stb1.Append("                                        and gl.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("  where gl.layerElementiGrafici_Cod is null " & vbCrLf)
        stb1.Append("  group by a.av_cod, a.av_des_vol " & vbCrLf)

        Dim tempTable As New StringBuilder
        tempTable.Append("  select * into #ATABLE from ( " & vbCrLf)
        tempTable.Append("  select " & _layerSpecialeImpianti & " as av_cod, 'Impianti' as av_des_vol " & vbCrLf)
        tempTable.Append("  union " & vbCrLf)
        tempTable.Append("  select avv.av_cod, avv.av_des_vol " & vbCrLf)
        tempTable.Append("  from  Avversita avv " & vbCrLf)
        tempTable.Append("  inner join MisuraXAvversita ma on ma.av_cod = avv.av_cod " & vbCrLf)
        tempTable.Append("  inner join " & tabellaLetturaAV & " t on t.av_cod = ma.av_cod  " & vbCrLf)
        tempTable.Append("  ) XYZ " & vbCrLf)

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.Avversita,
            "a.av_cod ",
            "a.av_des_vol",
            2,
            "  , 'insetto16.png'",
            "  , 'insetto32.png'",
            stb1.ToString,
            tempTable.ToString,
            True,
            stb)

    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_RilieviVegetoProduttivi(ByVal PivaSuperUser As String, ByVal tabellaLetturaAV As String, ByRef Utente As String, ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.Append("    from ( " & vbCrLf)
        stb1.Append("    select " & _layerSpecialeImpianti & " as ind_mat_cod, 'Impianti' as Ind_Mat_Des " & vbCrLf)
        stb1.Append("    union " & vbCrLf)
        stb1.Append("    Select a.ind_mat_cod, a.IND_MAT_DES " & vbCrLf)
        stb1.Append("    From IndiciMaturita a  " & vbCrLf)
        stb1.Append("    inner Join MisuraXIndiciMaturita ma  " & vbCrLf)
        stb1.Append("    On a.IND_MAT_COD = ma.IND_MAT_COD " & vbCrLf)
        stb1.Append("    inner Join " & tabellaLetturaAV & " t " & vbCrLf)
        stb1.Append("    On t.ff_classe = ma.IND_MAT_COD " & vbCrLf)
        stb1.Append("    ) a " & vbCrLf)
        stb1.Append("    Left Join  " & vbCrLf)
        stb1.Append("         GIS_LayerElementiGrafici gl " & vbCrLf)
        stb1.Append("        on gl.TipologiaLayer_cod =  " & enum_TipologiaLayer.RilieviVegetoProduttivi & vbCrLf)
        stb1.Append("        and gl.LayerElementiGrafici_Cod = a.IND_MAT_COD " & vbCrLf)
        stb1.Append("        and gl.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("  where gl.layerElementiGrafici_Cod is null " & vbCrLf)

        stb1.Append(" group by a.IND_MAT_COD, a.IND_MAT_DES " & vbCrLf)

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.RilieviVegetoProduttivi,
            "a.IND_MAT_COD ",
            "a.IND_MAT_DES ",
            2,
            "  , 'AreeOmogenee16.png'",
            "  , 'AreeOmogenee32.png'",
            stb1.ToString,
            "",
            True,
            stb
        )
        '---- fine avversità -----
    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_AnalisiPeriodicitaAgenda(ByVal PivaSuperUser As String, ByRef Utente As String, ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder

        stb1.AppendLine(" from ( ")

        stb1.AppendLine(" Select 0 as Lav_Cod ")
        stb1.AppendLine(" , 'Nessuna Operazione' as Lav_Des ")
        stb1.AppendLine(" Union ")

        stb1.AppendLine(" Select distinct ")
        stb1.AppendLine("    o.LAV_COD ")
        stb1.AppendLine("  , o.LAV_DES ")
        stb1.AppendLine(" From Agenda a1 ")
        stb1.AppendLine(" inner Join Operazioni o ")
        stb1.AppendLine("     On a1.Lav_Cod = o.LAV_COD")
        stb1.AppendLine(" union ")
        stb1.AppendLine("   Select top 1 113 As lav_cod, 'Rilievo Avversità in Campo' as Lav_Des ")
        stb1.AppendLine("   From DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV ")
        stb1.AppendLine(" ) a ")

        stb1.Append("    Left Join  " & vbCrLf)
        stb1.Append("         GIS_LayerElementiGrafici gl " & vbCrLf)
        stb1.Append("        on gl.TipologiaLayer_cod =  " & enum_TipologiaLayer.AnalisiPeriodicitaAgenda & vbCrLf)
        stb1.Append("        and gl.LayerElementiGrafici_Cod = a.LAV_COD " & vbCrLf)
        stb1.Append("        and gl.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("  where gl.layerElementiGrafici_Cod is null " & vbCrLf)

        stb1.Append(" group by a.LAV_COD, a.LAV_DES " & vbCrLf)

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.AnalisiPeriodicitaAgenda,
            "a.LAV_COD ",
            "a.LAV_DES ",
            2,
            "  , 'Agenda16.png'",
            "  , 'Agenda32.png'",
            stb1.ToString,
            "",
            True,
            stb
        )
        '---- fine avversità -----
    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_IndiciMaturitaTiles(
        ByVal PivaSuperUser As String,
        ByRef Utente As String,
        ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.Append("   from  IndiciMaturita a " & vbCrLf)
        stb1.Append("   inner join MisuraXIndiciMaturita ma " & vbCrLf)
        stb1.Append("           on a.ind_Mat_Cod = ma.Ind_Mat_cod " & vbCrLf)
        stb1.Append("   inner join (select distinct FF_classe from Mov_Dettaglio_tecnico ) t " & vbCrLf)
        stb1.Append("           on t.ff_classe = ma.ind_mat_Cod " & vbCrLf)
        stb1.Append("   inner join unitamisura udm " & vbCrLf)
        stb1.Append("           on udm.udm_cod = ma.udm_cod " & vbCrLf)
        stb1.Append("   inner join GIS_LayerElementiGrafici gl " & vbCrLf)
        stb1.Append("           on gl.LayerElementiGrafici_Cod = ma.ind_Mat_Cod " & vbCrLf)
        stb1.Append("          and gl.TipologiaLayer_cod = " & enum_TipologiaLayer.RilieviVegetoProduttivi & vbCrLf)
        stb1.Append("          and gl.utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)

        'stb1.Append("        left join (select MxAv_cod , count(*) as varianza from MisuraXAvversita_Anagrafiche group by MxAv_cod) anag " & vbCrLf)
        'stb1.Append("        on anag.MxAv_cod = ma.cod  " & vbCrLf)

        stb1.Append("   left join GIS_layerTiles glt " & vbCrLf)
        stb1.Append("          on glt.LayerTiles_Cod =  ma.ind_mat_Cod " & vbCrLf)
        stb1.Append("         and glt.LayerElementiGrafici_Cod =  a.ind_Mat_Cod " & vbCrLf)
        stb1.Append("         and glt.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("         and glt.TipologiaLayer_cod = " & enum_TipologiaLayer.RilieviVegetoProduttivi & " " & vbCrLf)
        stb1.Append("   where glt.layerElementiGrafici_Cod is null " & vbCrLf)

        '" isnull( anag.varianza , 7) ",

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM_tiles(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.RilieviVegetoProduttivi,
            "a.ind_Mat_cod",
            "ma.ind_mat_Cod",
            "udm.udm_des",
            " 7 ",
            stb1.ToString,
            ", gl.colore_primario ",
            ", gl.colore_secondario ",
            stb
        )


        '---- fine misuraXAvversita, TILES
    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_AnalisiPeriodicitaAgendaTiles(
        ByVal PivaSuperUser As String,
        ByRef Utente As String,
        ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder

        stb1.Append("    from GIS_LayerElementiGrafici gl " & vbCrLf)

        stb1.Append("    left join GIS_layerTiles glt " & vbCrLf)
        stb1.Append("           on glt.LayerTiles_Cod = gl.LayerElementiGrafici_Cod " & vbCrLf)
        stb1.Append("          and glt.LayerElementiGrafici_Cod = gl.LayerElementiGrafici_Cod " & vbCrLf)
        stb1.Append("          and glt.TipologiaLayer_cod = gl.TipologiaLayer_cod " & vbCrLf)
        stb1.Append("          and glt.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("    where glt.layerElementiGrafici_Cod is null " & vbCrLf)
        stb1.Append("    and   gl.TipologiaLayer_cod = " & enum_TipologiaLayer.AnalisiPeriodicitaAgenda & vbCrLf)
        stb1.Append("    and   gl.utente = '" & Agro_SQL_SaveText(Utente) & "'  " & vbCrLf)

        '" isnull( anag.varianza , 7) ",

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM_tiles(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.AnalisiPeriodicitaAgenda,
            "gl.LayerElementiGrafici_Cod",
            "gl.LayerElementiGrafici_Cod",
            "'Giorni trascorsi'",
            " 6 ",
            stb1.ToString,
            ", gl.colore_primario ",
            ", gl.colore_secondario ",
            stb
        )


        '---- fine misuraXAvversita, TILES
    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_AvvTiles(ByVal PivaSuperUser As String, ByVal tabellaAvversita As String, ByRef Utente As String, ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.Append("   from  Avversita a " & vbCrLf)
        stb1.Append("   inner join MisuraXAvversita ma on a.av_cod = ma.av_cod " & vbCrLf)
        stb1.Append("   inner join SpecieVegetali sv on sv.veg_cod = ma.veg_cod " & vbCrLf)
        stb1.Append("   inner join (select distinct av_cod from " & tabellaAvversita & " ) t on t.av_cod = ma.AV_COD " & vbCrLf)
        stb1.Append("   inner join unitamisura udm on udm.udm_cod = ma.udm_COD " & vbCrLf)
        stb1.Append("   inner join GIS_LayerElementiGrafici gl on gl.LayerElementiGrafici_Cod = ma.AV_COD " & vbCrLf)
        stb1.Append("                                         and gl.TipologiaLayer_cod = " & enum_TipologiaLayer.Avversita & " " & vbCrLf)
        stb1.Append("                                         and gl.utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("   left join (select MxAv_cod, count(*) as varianza " & vbCrLf)
        GIS_LayerElementiGrafici_R.MisuraXAvversitaAnagrafiche(stb1, "a1")
        stb1.Append("              group by MxAv_cod) anag on anag.MxAv_cod = ma.cod " & vbCrLf)

        stb1.Append("   where not exists ( select 1 from GIS_layerTiles glt " & vbCrLf)
        stb1.Append("                      where glt.LayerTiles_Cod = ma.cod " & vbCrLf)
        stb1.Append("                      and glt.LayerElementiGrafici_Cod = a.av_Cod " & vbCrLf)
        stb1.Append("                      and glt.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("                      and glt.TipologiaLayer_cod = " & enum_TipologiaLayer.Avversita & " ) " & vbCrLf)

        'stb1.Append("   Where not exists(Select 1 from Avversita a " & vbCrLf)
        'stb1.Append("                    inner join MisuraXAvversita ma on a.av_cod = ma.av_cod " & vbCrLf)
        'stb1.Append("                    inner join GIS_layerTiles glt on glt.LayerTiles_Cod = ma.cod " & vbCrLf)
        'stb1.Append("                               And glt.LayerElementiGrafici_Cod = a.av_Cod " & vbCrLf)
        'stb1.Append("                               And glt.Utente = '" & Agro_SQL_SaveText(utente) & "' " & vbCrLf)
        'stb1.Append("                               and glt.TipologiaLayer_cod = " & enum_TipologiaLayer.Avversita & " ) " & vbCrLf)

        'stb1.Append("   left join GIS_layerTiles glt on glt.LayerTiles_Cod = ma.cod " & vbCrLf)
        'stb1.Append("                               and glt.LayerElementiGrafici_Cod = a.av_Cod " & vbCrLf)
        'stb1.Append("                               and glt.Utente = '" & Agro_SQL_SaveText(utente) & "' " & vbCrLf)
        'stb1.Append("                               and glt.TipologiaLayer_cod = " & enum_TipologiaLayer.Avversita & " " & vbCrLf)
        'stb1.Append("   where glt.layerElementiGrafici_Cod is null " & vbCrLf)

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM_tiles(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.Avversita,
            "a.av_cod",
            "ma.cod",
            "udm.udm_des collate Latin1_General_CI_AS + ' [' + sv.veg_des + ']'",
            " isnull(anag.varianza, 7) ",
            stb1.ToString,
            ", gl.colore_primario ",
            ", gl.colore_secondario ",
            stb
        )


        '---- fine misuraXAvversita, TILES
    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_AvvTilesDes(
        ByVal PivaSuperUser As String,
        ByVal tabellaAvversita As String,
        ByRef Utente As String,
        ByRef stbInsert As System.Text.StringBuilder)

        Dim stbFrom As New StringBuilder
        stbFrom.Append("   from  Avversita a " & vbCrLf)
        stbFrom.Append("   inner join MisuraXAvversita ma on a.av_cod = ma.av_cod " & vbCrLf)
        stbFrom.Append("   inner join MisuraXAvversita_Anagrafiche ma_anag on ma_anag.mxav_cod = ma.cod " & vbCrLf)
        stbFrom.Append("   inner join SpecieVegetali sv on sv.veg_cod = ma.veg_cod " & vbCrLf)
        stbFrom.Append("   inner join (select distinct av_cod from " & tabellaAvversita & " ) t on t.av_cod = ma.AV_COD " & vbCrLf)
        stbFrom.Append("   inner join unitamisura udm on udm.udm_cod = ma.udm_COD " & vbCrLf)
        stbFrom.Append("   inner join GIS_LayerElementiGrafici gl on gl.LayerElementiGrafici_Cod = ma.AV_COD " & vbCrLf)
        stbFrom.Append("                                         and gl.TipologiaLayer_cod = " & enum_TipologiaLayer.Avversita & " " & vbCrLf)
        stbFrom.Append("                                         and gl.utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)

        stbFrom.Append("   where not exists ( select 1 from GIS_LayerTilesDescrizione gltd " & vbCrLf)
        stbFrom.Append("                      where gltd.LayerTiles_Cod = ma.cod " & vbCrLf)
        stbFrom.Append("                      and gltd.LayerElementiGrafici_Cod = a.av_Cod " & vbCrLf)
        stbFrom.Append("                      and gltd.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stbFrom.Append("                      and gltd.TipologiaLayer_cod = " & enum_TipologiaLayer.Avversita & " ) " & vbCrLf)

        Const xLayerTilesDescrizione_Cod =
            "row_number() over (order by a.av_cod, ma.cod, ma_anag.Anag_valore)" &
            " + (select ultimo_valore from Sequenza_Tabelle where Nome_Tabella = 'GIS_LayerTilesDescrizione')"

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM_tilesDes(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.Avversita,
            "a.av_cod",
            "ma.cod",
            xLayerTilesDescrizione_Cod,
            "ma_anag.Anag_des",
            "ma_anag.Anag_valore",
            "",
            stbFrom.ToString,
            stbInsert)

    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_Fenologia(ByVal PivaSuperUser As String, ByVal tabellaFF As String, ByRef Utente As String, ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.Append(" from  fasifenologiche ff " & vbCrLf)
        stb1.Append(" inner join ( " & vbCrLf)
        stb1.Append("    select FF_Classe as ff_cod " & vbCrLf)
        stb1.Append("    from " & tabellaFF & vbCrLf)
        stb1.Append("    union  " & vbCrLf)
        stb1.Append("    select piezo2 as ff_cod " & vbCrLf)
        stb1.Append("    from " & tabellaFF & vbCrLf)
        stb1.Append(" ) Ril on ril.ff_cod = ff.FF_COD " & vbCrLf)
        stb1.Append(" ")

        stb1.Append("    Left Join  " & vbCrLf)
        stb1.Append("         GIS_LayerElementiGrafici gl " & vbCrLf)
        stb1.Append("        on gl.TipologiaLayer_cod =  8 " & vbCrLf)
        stb1.Append("        and gl.LayerElementiGrafici_Cod = ff.ff_cod " & vbCrLf)
        stb1.Append("        and gl.Utente = '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
        stb1.Append("  " & vbCrLf)
        stb1.Append("  where gl.layerElementiGrafici_Cod is null " & vbCrLf)

        stb1.Append(" group by ff.ff_COD, ff.ff_des " & vbCrLf)

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.Fenologia,
            "ff.ff_cod ",
            "ff.ff_des ",
            1,
            "  , 'foglia16.png'",
            "  , 'foglia32.png'",
            stb1.ToString,
            "",
            False,
            stb
        )
        '---- fine fenologia
    End Sub

    Private Sub LeggiXGetQuery_GeneraColoriAutomatici_StadiCrescita(
        ByVal PivaSuperUser As String,
        ByVal tabellaFF As String,
        ByRef Utente As String,
        ByRef stb As System.Text.StringBuilder)

        Dim stb1 As New StringBuilder
        stb1.AppendLine(" from SpecieVegetaliXStadiCrescita ss ")
        stb1.AppendLine(" left join Stadi_Crescita_BBCH bbch on bbch.id_bbch = ss.id_bbch ")
        stb1.AppendLine(" inner join ( ")
        stb1.AppendLine("       select FF_Classe as ff_cod from " & tabellaFF & " ")
        stb1.AppendLine("       union  ")
        stb1.AppendLine("       select piezo2 as ff_cod from " & tabellaFF & " ")
        stb1.AppendLine(" ) ril on ril.ff_cod = ss.Cod_SS ")
        stb1.AppendLine(" left join GIS_LayerElementiGrafici gl ")
        stb1.AppendLine("        on gl.TipologiaLayer_cod =  " & enum_TipologiaLayer.Fenologia & " ")
        stb1.AppendLine("       and gl.LayerElementiGrafici_Cod = ss.Cod_SS ")
        stb1.AppendLine("       and gl.Utente = '" & Agro_SQL_SaveText(Utente) & "' ")
        stb1.AppendLine(" where gl.layerElementiGrafici_Cod is null ")
        stb1.AppendLine(" group by ss.Cod_SS, ss.descrizione, bbch.Stadio_Principale, bbch.Seconda_Cifra")

        Dim campoCod = "ss.Cod_SS"

        Dim campoDes = String.Format("{0} + {1} + {2} + {3}",
                                     "coalesce(ss.descrizione,'')",
                                     "case when bbch.stadio_principale is not null then ' - " + CostantiPersonalizzate.ScalaBBCH + " ' else '' end",
                                     "coalesce(cast(bbch.stadio_principale as varchar),'')",
                                     "coalesce(cast(bbch.seconda_cifra as varchar),'')")

        LeggiXGetQuery_GeneraColoriAutomatici_InsertSTM(
            PivaSuperUser,
            Utente,
            enum_TipologiaLayer.Fenologia,
            campoCod,
            campoDes,
            1,
            ", 'foglia16.png'",
            ", 'foglia32.png'",
            stb1.ToString,
            "",
            False,
            stb)

    End Sub

#End Region

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////

    Public Function UpdateSuperUserOneShot(
        ByVal PivaSuperUser As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Append("  UPDATE gl " & vbCrLf)
            stb.Append("  SET    PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "', Utente = '" & objParametri.SuperUserUsername & "' " & vbCrLf)
            stb.Append("  from   GIS_LayerElementiGrafici gl " & vbCrLf)
            stb.Append("  WHERE  PivaSuperUser = '123' " & vbCrLf)
            stb.Append("  and    not exists  " & vbCrLf)
            stb.Append("  ( " & vbCrLf)
            stb.Append("    Select 1 " & vbCrLf)
            stb.Append("    from   GIS_LayerElementiGrafici ii " & vbCrLf)
            stb.Append("    where  gl.LayerElementiGrafici_cod=ii.LayerElementiGrafici_cod " & vbCrLf)
            stb.Append("    and    ii.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' " & vbCrLf)
            stb.Append("    and    ii.Utente = '" & objParametri.SuperUserUsername & "' " & vbCrLf)
            stb.Append("  )  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            stb.Length = 0

            stb.Append(" update tt " & vbCrLf)
            stb.Append(" set    pivaSuperUser = gl.pivaSuperUser,  " & vbCrLf)
            stb.Append("        utente = gl.utente " & vbCrLf)
            stb.Append(" from   gis_layerTiles tt " & vbCrLf)
            stb.Append(" inner  join gis_layerElementiGrafici gl " & vbCrLf)
            stb.Append("          on gl.LayerElementiGrafici_Cod = tt.LayerElementiGrafici_Cod " & vbCrLf)
            stb.Append("         and gl.TipologiaLayer_cod = tt.TipologiaLayer_cod " & vbCrLf)
            stb.Append(" where  tt.pivasuperuser = '123' " & vbCrLf)
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function RicopiaLayerDaAltroUtenteTipologia(
            ByVal UtenteOrigine As String,
            ByVal UtenteDestinazione As String,
            ByVal TipologiaLayer_cod As Integer,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.RicopiaLayerDaAltroUtenteTipologia()"
        Dim stb As New System.Text.StringBuilder With {.Length = 0}
        Try
            stb.AppendLine("INSERT INTO GIS_LayerElementiGrafici (")
            stb.AppendLine("  [PivaSuperUser], [Utente],")
            stb.AppendLine("  [LayerElementiGrafici_Cod], [LayerElementiGrafici_Des], [Flag_Attivo], [Flag_Visibile],")
            stb.AppendLine("  [Colore_Base], [Colore_Selezionato], [Colore_Primario], [Colore_Secondario], [Varianza],")
            stb.AppendLine("  [TipologiaLayer_cod], [ZIndex], [Icona16], [Icona32], [inviato], [datainvio],")
            stb.AppendLine("  [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica],")
            stb.AppendLine("  [Validita_Inizio], [Validita_Fine], [trasparenza], [MostraDescrizioneAssociata] ")
            stb.AppendLine(") ")
            stb.AppendLine("SELECT")
            stb.AppendLine("  [PivaSuperUser], '" & Agro_SQL_SaveText(UtenteDestinazione) & "' As [Utente],")
            stb.AppendLine("  [LayerElementiGrafici_Cod], [LayerElementiGrafici_Des], [Flag_Attivo], [Flag_Visibile],")
            stb.AppendLine("  [Colore_Base], [Colore_Selezionato], [Colore_Primario], [Colore_Secondario], [Varianza],")
            stb.AppendLine("  [TipologiaLayer_cod], [ZIndex], [Icona16], [Icona32], [inviato], [datainvio],")
            stb.AppendLine("  [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica],")
            stb.AppendLine("  [Validita_Inizio], [Validita_Fine], [trasparenza], COALESCE([MostraDescrizioneAssociata], 0) AS [MostraDescrizioneAssociata] ")
            stb.AppendLine("FROM GIS_LayerElementiGrafici oo ")
            stb.AppendLine("WHERE TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod))
            stb.AppendLine("AND Utente = '" & Agro_SQL_SaveText(UtenteOrigine) & "' ")
            stb.AppendLine("AND NOT EXISTS ( ")
            stb.AppendLine("  SELECT 1 ")
            stb.AppendLine("  FROM GIS_LayerElementiGrafici ii ")
            stb.AppendLine("  WHERE TipologiaLayer_cod = 1 ")
            stb.AppendLine("  AND oo.LayerElementiGrafici_Cod = ii.LayerElementiGrafici_Cod ")
            stb.AppendLine("  AND ii.Utente = '" & Agro_SQL_SaveText(UtenteDestinazione) & "' ")
            stb.AppendLine(")")
            '--------------------------------------------------------------------------
           Return EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    'Lavez - 20/03/2025 - pezza temporanea per inizializzazione layer gis su creazione utente (da normalizzare come i metodi di copia layer)
    Public Function RicopiaLayerTilesDaAltroUtenteTipologia(
            ByVal UtenteOrigine As String,
            ByVal UtenteDestinazione As String,
            ByVal TipologiaLayer_cod As Integer,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.RicopiaLayerTilesDaAltroUtenteTipologia()"
        Dim stb As New System.Text.StringBuilder With {.Length = 0}
        Try
            stb.AppendLine("INSERT INTO GIS_LayerTiles (")
            stb.AppendLine("  [PivaSuperUser], [LayerElementiGrafici_Cod], [Utente],")
            stb.AppendLine("  [LayerTiles_Cod], [LayerTiles_Des],  [Colore_Primario], [Colore_Secondario], [Colore_Base], [Varianza],")
            stb.AppendLine("  [inviato], [datainvio], [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica],")
            stb.AppendLine("  [Validita_Inizio], [Validita_Fine],  [TipologiaLayer_Cod] ")
            stb.AppendLine(")")
            stb.AppendLine("SELECT ")
            stb.AppendLine("  [PivaSuperUser], [LayerElementiGrafici_Cod], '" & Agro_SQL_SaveText(UtenteDestinazione) & "' As [Utente],")
            stb.AppendLine("  [LayerTiles_Cod], [LayerTiles_Des],  [Colore_Primario], [Colore_Secondario], [Colore_Base], [Varianza],")
            stb.AppendLine("  [inviato], [datainvio], [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica],")
            stb.AppendLine("  [Validita_Inizio], [Validita_Fine],  [TipologiaLayer_Cod] ")
            stb.AppendLine("FROM GIS_LayerTiles oo ")
            stb.AppendLine("WHERE TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod))
            stb.AppendLine("AND Utente = '" & Agro_SQL_SaveText(UtenteOrigine) & "' ")
            stb.AppendLine("AND NOT EXISTS ( ")
            stb.AppendLine("  SELECT 1 ")
            stb.AppendLine("  FROM GIS_LayerTiles ii ")
            stb.AppendLine("  WHERE TipologiaLayer_cod = 1 ")
            stb.AppendLine("  AND oo.LayerElementiGrafici_Cod = ii.LayerElementiGrafici_Cod ")
            stb.AppendLine("  AND ii.Utente = '" & Agro_SQL_SaveText(UtenteDestinazione) & "' ")
            stb.AppendLine(")")
            '--------------------------------------------------------------------------
            Return EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    ''' <param name="UtenteDestinazione">Username dell'utente (o utenti) per cui inizializzare i layers. Se sono previsti più utenti, gli username devono essere contenuti tra apici e concatenati da virgola i.e. "'user1', 'user2', ..."</param>
    ''' <param name="TipologiaLayer_cod">Default : 1</param>
    ''' <param name="objServer">objParametri_server</param>
    Public Function RicopiaLayerDaAltroUtenteTipologiaMultiplo(
            ByVal UtenteOrigine As String,
            ByVal UtenteDestinazione As String,
            ByVal TipologiaLayer_cod As Integer,
            ByRef objUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.RicopiaLayerDaAltroUtenteTipologia()"
        Dim stb As New StringBuilder With {.Length = 0}
        Dim xrisp As Boolean
        Try
            stb.AppendLine(" ; WITH users AS ( ")
            stb.AppendLine("   SELECT username FROM [" & objUtenti.Recupera_NomeDB() & "].[dbo].[Utenti] WHERE username IN ( ")
            stb.AppendLine(Agro_SQL_Save_Clausola_IN(UtenteDestinazione, True))
            stb.AppendLine(" )) ")

            stb.AppendLine("INSERT INTO GIS_LayerElementiGrafici (")
            stb.AppendLine("  PivaSuperUser, Utente, LayerElementiGrafici_Cod, LayerElementiGrafici_Des, Flag_Attivo, Flag_Visibile, ")
            stb.AppendLine("  Colore_Base, Colore_Selezionato, Colore_Primario, Colore_Secondario, Varianza, TipologiaLayer_cod, ")
            stb.AppendLine("  ZIndex, Icona16, Icona32, inviato, datainvio, Data_Creazione, Data_Modifica,  ")
            stb.AppendLine("  Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, ")
            stb.AppendLine("  trasparenza, MostraDescrizioneAssociata ")
            stb.AppendLine(") ")

            stb.AppendLine("SELECT")
            stb.AppendLine("  [PivaSuperUser], u.[UserName] As [Utente], ")
            stb.AppendLine("  [LayerElementiGrafici_Cod], [LayerElementiGrafici_Des], [Flag_Attivo], [Flag_Visibile], [Colore_Base], [Colore_Selezionato], ")
            stb.AppendLine("  [Colore_Primario], [Colore_Secondario], [Varianza], [TipologiaLayer_cod], [ZIndex], [Icona16], [Icona32], ")
            stb.AppendLine("  oo.[inviato], oo.[datainvio], oo.[Data_Creazione], oo.[Data_Modifica], oo.[Username_Creazione], oo.[Username_Modifica], oo.[Validita_Inizio], oo.[Validita_Fine], ")
            stb.AppendLine("  [trasparenza], COALESCE([MostraDescrizioneAssociata], 0) AS [MostraDescrizioneAssociata] ")
            stb.AppendLine("FROM GIS_LayerElementiGrafici oo ")
            stb.AppendLine("CROSS JOIN users u ")
            stb.AppendLine("WHERE TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod))
            stb.AppendLine("AND Utente = '" & Agro_SQL_SaveText(UtenteOrigine) & "' ")
            stb.AppendLine("AND NOT EXISTS ( ")
            stb.AppendLine("  SELECT 1 ")
            stb.AppendLine("  FROM GIS_LayerElementiGrafici ii ")
            stb.AppendLine("  WHERE TipologiaLayer_cod = 1 ")
            stb.AppendLine("  AND oo.LayerElementiGrafici_Cod = ii.LayerElementiGrafici_Cod ")
            stb.AppendLine("  AND ii.Utente = u.UserName ")
            stb.AppendLine(")")

            xrisp = EseguiQuery_Scrittura(objServer, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objServer, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xrisp
    End Function

    ''Lavez - 20/03/2025 - pezza temporanea per inizializzazione layer gis su creazione utente (da normalizzare come i metodi di copia layer)
    ''' <param name="UtenteDestinazione">Username dell'utente (o utenti) per cui inizializzare i layers. Se sono previsti più utenti, gli username devono essere contenuti tra apici e concatenati da virgola i.e. "'user1', 'user2', ..."</param>
    ''' <param name="TipologiaLayer_cod">Default : 1</param>
    ''' <param name="objServer">objParametri_server</param>
    Public Function RicopiaLayerTilesDaAltroUtenteTipologiaMultiplo(
            ByVal UtenteOrigine As String,
            ByVal UtenteDestinazione As String,
            ByVal TipologiaLayer_cod As Integer,
            ByRef objUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.RicopiaLayerTilesDaAltroUtenteTipologiaMultiplo()"
        Dim stb As New StringBuilder With {.Length = 0}
        Dim xrisp As Boolean
        Try
            stb.AppendLine("; WITH users AS ( ")
            stb.AppendLine("  SELECT username FROM [" & objUtenti.Recupera_NomeDB() & "].[dbo].[Utenti] ")
            stb.AppendLine("  WHERE username IN ( ")
            stb.AppendLine(Agro_SQL_Save_Clausola_IN(UtenteDestinazione, True))
            stb.AppendLine("  )) ")

            stb.AppendLine("INSERT INTO GIS_LayerTiles (")
            stb.AppendLine("  PivaSuperUser, LayerElementiGrafici_Cod, Utente, LayerTiles_Cod, LayerTiles_Des,")
            stb.AppendLine("  Colore_Primario, Colore_Secondario, Colore_Base, varianza,")
            stb.AppendLine("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica,")
            stb.AppendLine("  Validita_Inizio, Validita_Fine, TipologiaLayer_cod")
            stb.AppendLine(")")
            stb.AppendLine("SELECT")
            stb.AppendLine("  [PivaSuperUser], [LayerElementiGrafici_Cod], u.[UserName] As [Utente], [LayerTiles_Cod], [LayerTiles_Des],")
            stb.AppendLine("  [Colore_Primario], [Colore_Secondario], [Colore_Base], [Varianza], ")
            stb.AppendLine("  oo.[inviato], oo.[datainvio], oo.[Data_Creazione], oo.[Data_Modifica], oo.[Username_Creazione], oo.[Username_Modifica],")
            stb.AppendLine("  oo.[Validita_Inizio], oo.[Validita_Fine], [TipologiaLayer_Cod] ")
            stb.AppendLine("FROM GIS_LayerTiles oo ")
            stb.AppendLine("CROSS JOIN users u")
            stb.AppendLine("WHERE TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod))
            stb.AppendLine("AND Utente = '" & Agro_SQL_SaveText(UtenteOrigine) & "' ")
            stb.AppendLine("AND NOT EXISTS ( ")
            stb.AppendLine("    SELECT 1 FROM GIS_LayerTiles ii ")
            stb.AppendLine("    WHERE TipologiaLayer_cod = 1 ")
            stb.AppendLine("    AND oo.LayerElementiGrafici_Cod = ii.LayerElementiGrafici_Cod ")
            stb.AppendLine("    AND ii.Utente = u.UserName ")
            stb.AppendLine(")")

            xrisp = EseguiQuery_Scrittura(objServer, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objServer, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xrisp
    End Function

    ''' <param name="TipologiaLayer_cod">Default : 1</param>
    ''' <param name="objServer">objParametri_server</param>
    Public Function RicopiaLayerDaAltroUtenteTipologiaMultiplo(
            ByVal UtenteOrigine As String,
            ByVal UtentiDestinazione As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.IUtente),
            ByVal TipologiaLayer_cod As Integer,
            ByRef objUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.RicopiaLayerDaAltroUtenteTipologiaMultiplo()"
        Dim stb As New StringBuilder With {.Length = 0}
        Dim xrisp As Boolean

        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 100 = 0 Then
                                    acc.name &= ", --" & u.index & vbNewLine & u.name
                                ElseIf u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim usersFilter = UtentiDestinazione.AsParallel.
            Select(Function(u, i) New With {.name = "'" & u.UserName & "'", .index = i}).
            Aggregate(prepareFilter).name

        Try
            stb.AppendLine(" ; WITH users AS ( ")
            stb.AppendLine("   SELECT username FROM [" & objUtenti.Recupera_NomeDB() & "].[dbo].[Utenti] WITH(NOLOCK) ")
            stb.AppendLine("   WHERE username IN ( " & usersFilter & " ) ")
            stb.AppendLine("   ) ")

            stb.AppendLine("INSERT INTO GIS_LayerElementiGrafici (")
            stb.AppendLine("  PivaSuperUser, Utente, LayerElementiGrafici_Cod, LayerElementiGrafici_Des, Flag_Attivo, Flag_Visibile, ")
            stb.AppendLine("  Colore_Base, Colore_Selezionato, Colore_Primario, Colore_Secondario, Varianza, TipologiaLayer_cod, ")
            stb.AppendLine("  ZIndex, Icona16, Icona32, inviato, datainvio, Data_Creazione, Data_Modifica,  ")
            stb.AppendLine("  Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, ")
            stb.AppendLine("  trasparenza, MostraDescrizioneAssociata ")
            stb.AppendLine(") ")

            stb.AppendLine("SELECT")
            stb.AppendLine("  [PivaSuperUser], u.[UserName] As [Utente], ")
            stb.AppendLine("  [LayerElementiGrafici_Cod], [LayerElementiGrafici_Des], [Flag_Attivo], [Flag_Visibile], [Colore_Base], [Colore_Selezionato], ")
            stb.AppendLine("  [Colore_Primario], [Colore_Secondario], [Varianza], [TipologiaLayer_cod], [ZIndex], [Icona16], [Icona32], ")
            stb.AppendLine("  oo.[inviato], oo.[datainvio], oo.[Data_Creazione], oo.[Data_Modifica], oo.[Username_Creazione], oo.[Username_Modifica], oo.[Validita_Inizio], oo.[Validita_Fine], ")
            stb.AppendLine("  [trasparenza], COALESCE([MostraDescrizioneAssociata], 0) AS [MostraDescrizioneAssociata] ")
            stb.AppendLine("FROM GIS_LayerElementiGrafici oo ")
            stb.AppendLine("CROSS JOIN users u ")
            stb.AppendLine("WHERE TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod))
            stb.AppendLine("AND Utente = '" & Agro_SQL_SaveText(UtenteOrigine) & "' ")
            stb.AppendLine("AND NOT EXISTS ( ")
            stb.AppendLine("  SELECT 1 ")
            stb.AppendLine("  FROM GIS_LayerElementiGrafici ii ")
            stb.AppendLine("  WHERE TipologiaLayer_cod = 1 ")
            stb.AppendLine("  AND oo.LayerElementiGrafici_Cod = ii.LayerElementiGrafici_Cod ")
            stb.AppendLine("  AND ii.Utente = u.UserName ")
            stb.AppendLine(")")

            xrisp = EseguiQuery_Scrittura(objServer, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objServer, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xrisp
    End Function

    ''Lavez - 20/03/2025 - pezza temporanea per inizializzazione layer gis su creazione utente (da normalizzare come i metodi di copia layer)
    ''' <param name="TipologiaLayer_cod">Default : 1</param>
    ''' <param name="objServer">objParametri_server</param>
    Public Function RicopiaLayerTilesDaAltroUtenteTipologiaMultiplo(
            ByVal UtenteOrigine As String,
            ByVal UtentiDestinazione As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.IUtente),
            ByVal TipologiaLayer_cod As Integer,
            ByRef objUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.RicopiaLayerTilesDaAltroUtenteTipologiaMultiplo()"
        Dim stb As New StringBuilder With {.Length = 0}
        Dim xrisp As Boolean

        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 100 = 0 Then
                                    acc.name &= ", --" & u.index & vbNewLine & u.name
                                ElseIf u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim usersFilter = UtentiDestinazione.AsParallel.
            Select(Function(u, i) New With {.name = "'" & u.UserName & "'", .index = i}).
            Aggregate(prepareFilter).name

        Try
            stb.AppendLine("; WITH users AS ( ")
            stb.AppendLine("  SELECT username FROM [" & objUtenti.Recupera_NomeDB() & "].[dbo].[Utenti] WITH(NOLOCK) ")
            stb.AppendLine("   WHERE username IN ( " & usersFilter & " ) ")
            stb.AppendLine("  ) ")

            stb.AppendLine("INSERT INTO GIS_LayerTiles (")
            stb.AppendLine("  PivaSuperUser, LayerElementiGrafici_Cod, Utente, LayerTiles_Cod, LayerTiles_Des,")
            stb.AppendLine("  Colore_Primario, Colore_Secondario, Colore_Base, varianza,")
            stb.AppendLine("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica,")
            stb.AppendLine("  Validita_Inizio, Validita_Fine, TipologiaLayer_cod")
            stb.AppendLine(")")
            stb.AppendLine("SELECT")
            stb.AppendLine("  [PivaSuperUser], [LayerElementiGrafici_Cod], u.[UserName] As [Utente], [LayerTiles_Cod], [LayerTiles_Des],")
            stb.AppendLine("  [Colore_Primario], [Colore_Secondario], [Colore_Base], [Varianza], ")
            stb.AppendLine("  oo.[inviato], oo.[datainvio], oo.[Data_Creazione], oo.[Data_Modifica], oo.[Username_Creazione], oo.[Username_Modifica],")
            stb.AppendLine("  oo.[Validita_Inizio], oo.[Validita_Fine], [TipologiaLayer_Cod] ")
            stb.AppendLine("FROM GIS_LayerTiles oo ")
            stb.AppendLine("CROSS JOIN users u")
            stb.AppendLine("WHERE TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod))
            stb.AppendLine("AND Utente = '" & Agro_SQL_SaveText(UtenteOrigine) & "' ")
            stb.AppendLine("AND NOT EXISTS ( ")
            stb.AppendLine("    SELECT 1 FROM GIS_LayerTiles ii ")
            stb.AppendLine("    WHERE TipologiaLayer_cod = 1 ")
            stb.AppendLine("    AND oo.LayerElementiGrafici_Cod = ii.LayerElementiGrafici_Cod ")
            stb.AppendLine("    AND ii.Utente = u.UserName ")
            stb.AppendLine(")")

            xrisp = EseguiQuery_Scrittura(objServer, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objServer, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xrisp
    End Function

    Public Function Scrivi(
                            ByVal PivaSuperUser As String,
                            ByVal Utente As String,
                            ByVal LayerElementiGrafici_Cod As String,
                            ByVal LayerElementiGrafici_Des As String,
                            ByVal Flag_Attivo As Int32,
                            ByVal Flag_Visibile As Int32,
                            ByVal Colore_Base As String,
                            ByVal Colore_Selezionato As String,
                            ByVal Colore_Primario As String,
                            ByVal Colore_Secondario As String,
                            ByVal Varianza As String,
                            ByVal Trasparenza As Double,
                            ByVal MostraDescrizioneAssociata As Integer,
                            ByVal TipologiaLayer_cod As Integer,
                            ByVal zindex As Integer,
                            ByVal icona16 As String,
                            ByVal icona32 As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If


            If (Not IsNumeric(LayerElementiGrafici_Cod) AndAlso LayerElementiGrafici_Cod = "") OrElse (IsNumeric(LayerElementiGrafici_Cod) AndAlso LayerElementiGrafici_Cod = 0) Then
                Throw New Exception("Parametro non corretto nella query (LayerElementiGrafici_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO GIS_LayerElementiGrafici ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    PivaSuperUser,               Utente,    ")
            StrSQL.Append("                    LayerElementiGrafici_Cod,    ")
            StrSQL.Append("                    LayerElementiGrafici_Des,    Flag_Attivo,    ")
            StrSQL.Append("                    Flag_Visibile,    ")
            If Colore_Base <> "" Then
                StrSQL.Append("                    Colore_Base,    ")
            End If
            If Colore_Selezionato <> "" Then
                StrSQL.Append("                    Colore_Selezionato,  ")
            End If
            If Colore_Primario <> "" Then
                StrSQL.Append("                    Colore_Primario,  ")
            End If
            If Colore_Secondario <> "" Then
                StrSQL.Append("                    Colore_Secondario,  ")
            End If
            If Varianza <> "" Then
                StrSQL.Append("                    Varianza,  ")
            End If

            StrSQL.Append("                    Trasparenza,  ")

            StrSQL.Append(" MostraDescrizioneAssociata, ")
            StrSQL.Append(" TipologiaLayer_cod, ")

            StrSQL.Append(" zindex, ")

            If icona16 <> "" Then
                StrSQL.Append("                    Icona16,  ")
            End If

            If icona32 <> "" Then
                StrSQL.Append("                    Icona32,  ")
            End If

            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Utente) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(LayerElementiGrafici_Cod) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(LayerElementiGrafici_Des) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_Attivo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_Visibile) & "  ")
            If Colore_Base <> "" Then
                StrSQL.Append("         , '" & Agro_SQL_SaveText(Colore_Base) & "'  ")
            End If
            If Colore_Selezionato <> "" Then
                StrSQL.Append("         , '" & Agro_SQL_SaveText(Colore_Selezionato) & "'  ")
            End If
            If Colore_Primario <> "" Then
                StrSQL.Append("         , '" & Agro_SQL_SaveText(Colore_Primario) & "'  ")
            End If
            If Colore_Secondario <> "" Then
                StrSQL.Append("         , '" & Agro_SQL_SaveText(Colore_Secondario) & "'  ")
            End If
            If Varianza <> "" Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Varianza) & "  ")
            End If

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Trasparenza) & "  ")


            StrSQL.Append("         ,   " & Agro_SQL_SaveNum(MostraDescrizioneAssociata) & " ")

            StrSQL.Append("         ,   " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")

            StrSQL.Append("         ,   " & Agro_SQL_SaveNum(zindex) & " ")

            If icona16 <> "" Then
                StrSQL.Append("                    , '" & Agro_SQL_SaveText(icona16) & "'  ")
            End If

            If icona32 <> "" Then
                StrSQL.Append("                    ,'" & Agro_SQL_SaveText(icona32) & "'  ")
            End If

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(" )")
            '---------------------------------------------

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

    Public Function Attiva_o_Disattiva_xUtente(
            ByVal Utente As String,
            ByVal Flag_Attivo As Integer,
            ByVal Flag_visibile As Integer,
            ByVal LayerElementiGrafici_Cod As Int32,
            ByVal TipologiaLayer_cod As Int32,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If LayerElementiGrafici_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerElementiGrafici_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE GIS_LayerElementiGrafici SET ")

            StrSQL.Append("    [Flag_Attivo]                =  " & Agro_SQL_SaveNum(Flag_Attivo) & " ")
            StrSQL.Append("   ,[Flag_Visibile]              =  " & Agro_SQL_SaveNum(Flag_visibile) & " ")

            StrSQL.Append(" WHERE LayerElementiGrafici_Cod  =  " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            StrSQL.Append(" AND   Utente  =  '" & Agro_SQL_SaveText(Utente) & "' ")
            StrSQL.Append(" AND TipologiaLayer_cod = " & Agro_SQL_SaveText(TipologiaLayer_cod) & "  ")
            '---------------------------------------------

            '----------------------------------------------------------------------
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

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////

    Public Function Modifica(
                            ByVal PivaSuperUser As String,
                            ByVal Utente As String,
                            ByVal LayerElementiGrafici_Cod As Int32,
                            ByVal LayerElementiGrafici_Des As String,
                            ByVal Flag_Attivo As Int32,
                            ByVal Flag_Visibile As Int32,
                            ByVal Colore_Base As String,
                            ByVal Colore_Selezionato As String,
                            ByVal Colore_Primario As String,
                            ByVal Colore_Secondario As String,
                            ByVal Varianza As Integer,
                            ByVal Zindex As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If LayerElementiGrafici_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerElementiGrafici_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE GIS_LayerElementiGrafici SET ")

            StrSQL.Append("   [LayerElementiGrafici_Des]   = '" & Agro_SQL_SaveText(LayerElementiGrafici_Des) & "'")
            StrSQL.Append("   ,[Flag_Attivo]                =  " & Agro_SQL_SaveNum(Flag_Attivo) & " ")
            StrSQL.Append("   ,[Flag_Visibile]              =  " & Agro_SQL_SaveNum(Flag_Visibile) & " ")
            StrSQL.Append("   ,[Colore_Base]                =  '" & Agro_SQL_SaveText(Colore_Base) & "' ")
            StrSQL.Append("   ,[Colore_Selezionato]                =  '" & Agro_SQL_SaveText(Colore_Selezionato) & "' ")
            StrSQL.Append("   ,[Colore_Primario]                =  '" & Agro_SQL_SaveText(Colore_Primario) & "' ")
            StrSQL.Append("   ,[Colore_Secondario]                =  '" & Agro_SQL_SaveText(Colore_Secondario) & "' ")
            StrSQL.Append("   ,[Varianza]         =  " & Agro_SQL_SaveNum(Varianza) & " ")

            StrSQL.Append("   ,[Zindex]         =  " & Agro_SQL_SaveNum(Zindex) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            StrSQL.Append(" WHERE PivaSuperUser             = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append(" AND   LayerElementiGrafici_Cod  =  " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            StrSQL.Append(" AND   Utente  =  '" & Agro_SQL_SaveText(Utente) & "' ")
            '---------------------------------------------

            '----------------------------------------------------------------------
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

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////

    Public Function Modifica_Colori(ByVal PivaSuperUser As String,
                                    ByVal Utente As String,
                                    ByVal LayerElementiGrafici_Cod As String,
                                    ByVal tipologiaLayer_cod As String,
                                    ByVal Colore_Primario As String,
                                    ByVal Colore_Secondario As String,
                                    ByVal Varianza As String,
                                    ByVal Trasparenza As String,
                                    ByVal MostraDescrizioneAssociata As String,
                                    ByVal zindex As String,
                                    ByVal flag_visibile As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            ' VAnni: 21/5/2018: il layer 0 è ammesso solo nella tipologia delle colture e analisi cronologia agenda
            If LayerElementiGrafici_Cod = 0 AndAlso GIS_LayerElementiGrafici_Utility.LayerZeroNonAmmesso(tipologiaLayer_cod) Then
                Throw New Exception("Parametro non corretto nella query (LayerElementiGrafici_Cod obbligatorio)")
            End If

            StrSQL.Length = 0

            StrSQL.Append("UPDATE GIS_LayerElementiGrafici SET ")

            StrSQL.Append("    Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            If Colore_Primario <> "" Then
                StrSQL.Append("   ,[Colore_Primario]                =  '" & Agro_SQL_SaveText(Colore_Primario) & "' ")
            End If
            If Colore_Secondario <> "" Then
                StrSQL.Append("   ,[Colore_Secondario]                =  '" & Agro_SQL_SaveText(Colore_Secondario) & "' ")
            End If
            If Varianza <> "" Then
                StrSQL.Append("   ,[Varianza]         =  " & Agro_SQL_SaveNum(Varianza) & " ")
            End If

            If Trasparenza <> "" Then
                StrSQL.Append("   ,[Trasparenza]         =  " & Agro_SQL_SaveNum(Trasparenza) & " ")
            End If

            If MostraDescrizioneAssociata <> "" Then
                StrSQL.Append("   ,[MostraDescrizioneAssociata]         =  " & Agro_SQL_SaveNum(MostraDescrizioneAssociata) & " ")
            End If

            If zindex <> "" Then
                StrSQL.Append("   ,zindex         = " & Agro_SQL_SaveNum(zindex) & " ")
            End If

            StrSQL.Append("   ,flag_visibile         = " & Agro_SQL_SaveNum(flag_visibile) & " ")

            StrSQL.Append(" WHERE PivaSuperUser            = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append(" AND   LayerElementiGrafici_Cod = '" & Agro_SQL_SaveText(LayerElementiGrafici_Cod) & "' ")
            StrSQL.Append(" AND   Utente             = '" & Agro_SQL_SaveText(Utente) & "' ")
            StrSQL.Append(" AND   tipologiaLayer_cod =  " & Agro_SQL_SaveNum(tipologiaLayer_cod) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Modifica_Flag_Vis(ByVal PivaSuperUser As String,
                                    ByVal Utente As String,
                                    ByVal LayerElementiGrafici_Cod As String,
                                    ByVal tipologiaLayer_cod As String,
                                    ByVal flag_visibile As Integer,
                                    ByVal flag_attivo As Integer,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            ' VAnni: 21/5/2018: il layer 0 è ammesso solo nella tipologia delle colture e analisi cronologia agenda
            If LayerElementiGrafici_Cod = 0 AndAlso GIS_LayerElementiGrafici_Utility.LayerZeroNonAmmesso(tipologiaLayer_cod) Then
                Throw New Exception("Parametro non corretto nella query (LayerElementiGrafici_Cod obbligatorio)")
            End If

            StrSQL.Length = 0

            StrSQL.Append("UPDATE GIS_LayerElementiGrafici SET ")

            StrSQL.Append("    Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append("   ,flag_visibile         = " & Agro_SQL_SaveNum(flag_visibile) & " ")
            StrSQL.Append("   ,flag_attivo         = " & Agro_SQL_SaveNum(flag_attivo) & " ")

            StrSQL.Append(" WHERE PivaSuperUser            = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append(" AND   LayerElementiGrafici_Cod = '" & Agro_SQL_SaveText(LayerElementiGrafici_Cod) & "' ")
            StrSQL.Append(" AND   Utente             = '" & Agro_SQL_SaveText(Utente) & "' ")
            StrSQL.Append(" AND   tipologiaLayer_cod =  " & Agro_SQL_SaveNum(tipologiaLayer_cod) & " ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Modifica_Visibilita(ByVal PivaSuperUser As String,
                                        ByVal Utente As String,
                                        ByVal layerElementiGraficiCod As String,
                                        ByVal tipologiaLayerCod As String,
                                        ByVal flagVisibile As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Return Modifica_Colori(PivaSuperUser,
                               Utente,
                               layerElementiGraficiCod,
                               tipologiaLayerCod,
                               "",
                               "",
                               "",
                               "",
                               "",
                               "",
                               flagVisibile,
                               "",
                               objParametri)

    End Function
    Public Function Modifica_Flag_Visibilita(ByVal PivaSuperUser As String,
                                        ByVal Utente As String,
                                        ByVal layerElementiGraficiCod As String,
                                        ByVal tipologiaLayerCod As String,
                                        ByVal flagVisibile As Integer,
                                        ByVal flagAttivo As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Return Modifica_Flag_Vis(PivaSuperUser,
                               Utente,
                               layerElementiGraficiCod,
                               tipologiaLayerCod,
                               flagVisibile,
                               flagAttivo,
                               objParametri)

    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Cancella(
                            ByVal PivaSuperUser As String,
                            ByVal Utente As String,
                            ByVal LayerElementiGrafici_Cod As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If LayerElementiGrafici_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerElementiGrafici_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE GIS_LayerElementiGrafici ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
                StrSQL.AppendLine(" AND LayerElementiGrafici_Cod      =   " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & "  ")

                StrSQL.AppendLine(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.AppendLine("DELETE")
                StrSQL.AppendLine("FROM GIS_LayerElementiGrafici")
                StrSQL.AppendLine($"WHERE PivaSuperUser = '{Agro_SQL_SaveText(PivaSuperUser)}'")
                StrSQL.AppendLine($"    AND LayerElementiGrafici_Cod = {Agro_SQL_SaveNum(LayerElementiGrafici_Cod)}")

            End If

            If Utente <> "" Then
                StrSQL.AppendLine(" AND Utente      =   '" & Agro_SQL_SaveText(Utente) & "'  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function SalvaPaletteSLD(ByVal layerElementiGrafici_Cod As Int32,
                                    ByVal tipologiaLayer_Cod As Int32,
                                    ByVal paletteObj As JObject,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W.SalvaPaletteSLD()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerElementiGrafici_Anagrafica ")

            StrSQL.AppendLine(String.Format(" SET palettevisualizzazione = '{0}' ", Agro_SQL_SaveText(paletteObj.ToString)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" AND TipologiaLayer_Cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_Cod)))

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function
End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_LayerElementiGrafici_Utility

    Public Shared Function LayerZeroNonAmmesso(ByVal tipologiaLayerCod As String) As Boolean

        Dim tipologieLayerZeroAmmesso As String() = {
            enum_TipologiaLayer.SpecieVegetale.ToString("D"),
            enum_TipologiaLayer.Cultivar.ToString("D"),
            enum_TipologiaLayer.AnalisiPeriodicitaAgenda.ToString("D")
            }

        Return Not tipologieLayerZeroAmmesso.Contains(tipologiaLayerCod)

    End Function

End Class
