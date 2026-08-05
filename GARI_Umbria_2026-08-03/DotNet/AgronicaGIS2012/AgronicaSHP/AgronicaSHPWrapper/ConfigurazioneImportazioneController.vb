Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaSHPWrapper.TestShapeFile

Public Class ConfigurazioneImportazioneController


    Public Shared Sub RiportoCatastoDaGis(AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge As Short, CreaLayerTestuale As Boolean, progressivoImportCatasto As Integer, objParametri_Server As AgronicaCoreParametri)
        Try
            'Lavez - 05/11/2024 - gestione sequence inizializzazione se seq mancante
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                If Not AgronicaCoreDataProvider.Agro_Sequenze.InizializzaSequence("particelleCatastali", 0, 2000000, objParametri_Server) Then
                    Throw New Exception("Errore in inizializzazione sequence per particelleCatastali")
                End If
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            Dim CatastoRiportaDatiGIS As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_W
            CatastoRiportaDatiGIS.InserisciParticelleDaGIS(
                progressivoImportCatasto,
                "",
                "",
                "",
                0,
                0,
                0,
                objParametri_Server
            )

            'Lavez - 22/10/2025 - aggiunta scrittura tabella poligoni semplificati per calcolo ripartizione catastale da gis (New Agri)
            Dim CatastoPoligoniSemplificati As New AgronicaCoreGisDAL.GIS_ElementiGrafici_CatastoSimplyfied_W
            CatastoPoligoniSemplificati.InserisciPoligonoSemplificatoCatastoDaGIS(
                                        TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.CATASTO,
                                        objParametri_Server)

            If AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge = 1 Then
                Dim qrval As Boolean =
                CatastoRiportaDatiGIS.AggiornaParticelleDaGIS(
                                progressivoImportCatasto,
                                "",
                                "",
                                "",
                                0,
                                0,
                                0,
                                objParametri_Server
                            )

            End If

            CatastoRiportaDatiGIS.EseguiQuery_Scrittura(objParametri_Server, "update gis_entita set inviato = 0 where inviato = " & progressivoImportCatasto, "")

            Dim stb As New StringBuilder

            'Lavez - 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                Dim dtVal = CatastoRiportaDatiGIS.EseguiQuery_Lettura(objParametri_Server, "select cast(coalesce(MAX(part_cod), 0) + 1 as int) as newVal from particelleCatastali ", "")
                If dtVal.Rows.Count > 0 Then
                    stb.AppendLine(" Alter sequence Sequence_particelleCatastali restart with " + dtVal.Rows(0)("newVal").ToString())
                End If

            Else
                stb.Append(" update sequenza_Tabelle " & vbCrLf)
                stb.Append(" set ultimo_Valore = (select coalesce(MAX(part_cod), 0) + 1 from particelleCatastali) " & vbCrLf)
                stb.Append(" where nome_Tabella = 'particelleCatastali' " & vbCrLf)
            End If


            CatastoRiportaDatiGIS.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "")

            If CreaLayerTestuale Then

                'Lavez - 05/11/2024 - gestione sequence inizializzazione se seq mancante
                If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                    If Not AgronicaCoreDataProvider.Agro_Sequenze.InizializzaSequence("GIS_Entita", 0, 2000000, objParametri_Server) Then
                        Throw New Exception("Errore in inizializzazione sequence per GIS_Entita")
                    End If
                    If Not AgronicaCoreDataProvider.Agro_Sequenze.InizializzaSequence("GIS_ElementiGrafici", 0, 2000000, objParametri_Server) Then
                        Throw New Exception("Errore in inizializzazione sequence per GIS_ElementiGrafici")
                    End If
                End If


                stb.Length = 0
                stb.AppendLine(" declare @last int  ")
                'Lavez - 05/11/2024 - gestione sequence
                If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                    stb.AppendLine(" select @last = coalesce(SELECT current_value FROM sys.sequences WHERE name = 'Sequence_GIS_Entita', 0)  ")
                Else
                    stb.AppendLine(" select @last = coalesce(max(Entita_Cod), 0)  ")
                    stb.AppendLine(" from GIS_Entita  ")
                End If

                stb.AppendLine("  ")
                stb.AppendLine(" create table #tEnt (OldEntita_cod int, NewEntita_Cod int) ")
                stb.AppendLine("  ")
                stb.AppendLine("  ")
                stb.AppendLine(" insert #tEnt(OldEntita_cod, NewEntita_Cod) ")
                stb.AppendLine(" select e.entita_cod, @last + dense_rank() over (order by entita_cod, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO )as newEntita_Cod ")
                stb.AppendLine(" from gis_entita e  ")
                stb.AppendLine(" where TipoEntita_Cod = 3  ")
                stb.AppendLine(" and Not exists (  ")
                stb.AppendLine("     select 1  ")
                stb.AppendLine("     from GIS_Entita ii  ")
                stb.AppendLine("     where TipoEntita_Cod = 5  ")
                stb.AppendLine("     and ii.PROV = e.prov  ")
                stb.AppendLine("     and ii.COM = e.com  ")
                stb.AppendLine("     and ii.SEZIONE = e.SEZIONE   ")
                stb.AppendLine("     and ii.FOGLIO = e.FOGLIO   ")
                stb.AppendLine("     and ii.NUMERO= e.NUMERO   ")
                stb.AppendLine("     and ii.SUBALTERNO =e.SUBALTERNO   ")
                stb.AppendLine(" )  ")
                stb.AppendLine("  ")
                stb.AppendLine("  ")
                stb.AppendLine(" insert gis_entita  ")
                stb.AppendLine(" select PivaSuperUser, NewEntita_Cod, 5 as TipoEntita_Cod, Piva, Sa_Cod, Appezza, Campo_Cod, Id_Imp, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, Programmazione_Entita_Cod, ID_Agenda, Ricetta_Operazione_cod, analisi_campione_cod, OLDGrafica_ID, 0 as inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, programmazione_cod, id_mov_det  ")
                stb.AppendLine(" from gis_entita e  ")
                stb.AppendLine("  inner join #tEnt old ")
                stb.AppendLine("      on e.entita_cod = old.oldEntita_cod ")
                stb.AppendLine("  ")
                stb.AppendLine("                   ")
                stb.AppendLine(" insert GIS_ElementiGrafici  ")
                stb.AppendLine(" select  ")
                stb.AppendLine("        e.PivaSuperUser ")
                stb.AppendLine("  , ii.NewEntita_Cod as elementografico_cod ")
                stb.AppendLine("  , cast(E.foglio as varchar(100)) + ' - ' +  cast(e.numero as varchar(100))  as ElementoGrafico_Des ")
                stb.AppendLine("  , ii.NewEntita_Cod as Entita_Cod ")
                stb.AppendLine("  , 5 as LayerElementiGrafici_Cod  ")
                stb.AppendLine(" , geography::STGeomFromText( geometry::STGeomFromText(Poligono_GeoEntity.STAsText(), 0).MakeValid().STCentroid().STAsText(), 4326) as Poligono_GeoEntity  ")
                stb.AppendLine(" , Flag_GPS, g.inviato, g.datainvio, g.Data_Creazione, g.Data_Modifica, g.Username_Creazione, g.Username_Modifica, g.Validita_Inizio, g.Validita_Fine  ")
                stb.AppendLine(" from #tEnt ii  ")
                stb.AppendLine("     inner join gis_entita e  ")
                stb.AppendLine("      on ii.OldEntita_cod = e.entita_cod ")
                stb.AppendLine("     inner join GIS_ElementiGrafici g  ")
                stb.AppendLine("      on e.Entita_Cod = g.Entita_Cod   ")
                stb.AppendLine("                   ")
                stb.AppendLine("  ")
                stb.AppendLine("  ")
                'Lavez - 05/11/2024 - gestione sequence
                If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                    stb.AppendLine(" Declare @newGisEnt as int = coalesce( (select MAX(entita_cod) from GIS_Entita ), (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_Gis_entita')) ")
                    stb.AppendLine(" Alter sequence Sequence_Gis_entita ")
                    stb.AppendLine(" restart with @newGisEnt ")
                    stb.AppendLine(" Declare @newGisEle as int = coalesce( (select MAX(ElementoGrafico_Cod) from GIS_ElementiGrafici ), (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_GIS_ElementiGrafici')) ")
                    stb.AppendLine(" Alter sequence Sequence_Gis_elementiGrafici ")
                    stb.AppendLine(" restart with @newGisEle ")
                Else
                    stb.AppendLine(" update sequenza_tabelle  ")
                    stb.AppendLine(" set ultimo_valore = coalesce(  ")
                    stb.AppendLine("     (select max(Entita_Cod)  ")
                    stb.AppendLine("     from GIS_Entita  ")
                    stb.AppendLine(" ), 0)    ")
                    stb.AppendLine(" where nome_tabella = 'GIS_Entita'  ")
                    stb.AppendLine("  ")
                    stb.AppendLine("                   ")
                    stb.AppendLine("                   ")
                    stb.AppendLine(" update sequenza_tabelle  ")
                    stb.AppendLine(" set ultimo_valore = coalesce(  ")
                    stb.AppendLine("     (select max(ElementoGrafico_Cod )  ")
                    stb.AppendLine("     from GIS_ElementiGrafici  ")
                    stb.AppendLine(" ), 0)  ")
                    stb.AppendLine(" where nome_tabella = 'GIS_ElementiGrafici'  ")
                End If


                stb.AppendLine("  ")
                stb.AppendLine("  ")
                stb.AppendLine("  ")
                stb.AppendLine(" drop table #tEnt")


                CatastoRiportaDatiGIS.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "")

            End If

            G2G_Chiusura_Transazione(1, objParametri_Server)

        Catch ex As Exception
            G2G_Chiusura_Transazione(2, objParametri_Server)
        End Try
    End Sub

    Private Shared Sub G2G_Chiusura_Transazione(
        ByVal Flag_Commit1_Rollback2 As Integer,
        objParametri_Server As AgronicaCoreParametri
    )

        Dim NomeRoutine As String = "G2G_Chiusura_Transazione"

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, objParametri_Server)

        Catch ex As Exception
        End Try

    End Sub

    Public Shared Sub RecuperaChiaviLetturaCatasto(configurazioneImportazione As ConfigurazioneImportazione, shape As ShapeFileRecord, ByRef CodBelfiore As String, ByRef lIstatP As String, ByRef listatc As String, ByRef lid_sezc As String, ByRef lFoglio As String, ByRef lParticella As String, ByRef lSub As String)

        Dim ColNameCodBelfiore As String = Nothing
        Dim ColNameIstatP As String = Nothing
        Dim ColNameIstatC As String = Nothing
        Dim ColNameIdSezC As String = Nothing
        Dim ColNameFoglio As String = Nothing
        Dim ColNameParticella As String = Nothing
        Dim ColNameSub As String = Nothing

        recuperaChiaviConfigCatasto(configurazioneImportazione, ColNameCodBelfiore, ColNameIstatP, ColNameIstatC, ColNameIdSezC, ColNameFoglio, ColNameParticella, ColNameSub)
        'esiete ConfigurazioneImportazione_Catasto



        CodBelfiore = getAttribute(ColNameCodBelfiore, shape)
        lIstatP = getAttribute(ColNameIstatP, shape)
        listatc = getAttribute(ColNameIstatC, shape)
        lid_sezc = getAttribute(ColNameIdSezC, shape)
        lFoglio = getAttribute(ColNameFoglio, shape)
        lParticella = getAttribute(ColNameParticella, shape)
        lSub = getAttribute(ColNameSub, shape)


        If lIstatP = "" Then
            lIstatP = "0"
        End If

        If listatc = "" Then
            listatc = "0"
        End If

        If lid_sezc = "" Then
            lid_sezc = "0"
        End If

        If lFoglio = "" Then
            lFoglio = "-1"
        End If

        If lParticella = "" Then
            lParticella = "-1"
        End If

        If lSub = "" Then
            lSub = "0"
        End If
    End Sub

    Public Shared Sub recuperaChiaviConfigCatasto(configurazioneImportazione As ConfigurazioneImportazione, ByRef ColNameCodBelfiore As String, ByRef ColNameIstatP As String, ByRef ColNameIstatC As String, ByRef ColNameIdSezC As String, ByRef ColNameFoglio As String, ByRef ColNameParticella As String, ByRef ColNameSub As String)
        ColNameCodBelfiore = "Cod_Belfiore"
        ColNameIstatP = "istatp"
        ColNameIstatC = "istatc"
        ColNameIdSezC = "id_sezc"
        ColNameFoglio = "foglio"
        ColNameParticella = "particella"
        ColNameSub = "sub"

        If Not configurazioneImportazione Is Nothing AndAlso
            Not configurazioneImportazione.ConfigurazioneImportazione_Catasto Is Nothing Then
            ColNameCodBelfiore = configurazioneImportazione.ConfigurazioneImportazione_Catasto.CodBelfiore
            ColNameIstatP = configurazioneImportazione.ConfigurazioneImportazione_Catasto.Prov
            ColNameIstatC = configurazioneImportazione.ConfigurazioneImportazione_Catasto.Com
            ColNameIdSezC = configurazioneImportazione.ConfigurazioneImportazione_Catasto.Sezione
            ColNameFoglio = configurazioneImportazione.ConfigurazioneImportazione_Catasto.Foglio
            ColNameParticella = configurazioneImportazione.ConfigurazioneImportazione_Catasto.Particella
            ColNameSub = configurazioneImportazione.ConfigurazioneImportazione_Catasto.Subalterno
        End If
    End Sub

    Private Shared Function getAttribute(ByVal colName As String, ByVal record As ShapeFileRecord) As String
        If String.IsNullOrEmpty(colName) Then
            Return ""
        End If
        Dim rval As String = ""
        If Not record.Attributes Is Nothing Then
            For i As Integer = 0 To record.Attributes.ItemArray.GetLength(0) - 1
                If record.Attributes.Table.Columns.Item(i).ColumnName.ToLower = colName.ToLower Then
                    rval = record.Attributes(i).ToString()
                    Exit For
                End If
            Next
        End If

        Return rval
    End Function

End Class
