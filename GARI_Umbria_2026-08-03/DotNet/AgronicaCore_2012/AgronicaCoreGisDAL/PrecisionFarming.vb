
Imports System.Data.OleDb
Imports System.Text

Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Imports AgronicaGIS2012.Commons.ElementoGrafico_DES_Helper
Imports AgronicaCoreDataProvider



Public Class PrecisionFarming
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function EliminaGIS_NonAssociato(ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.PrecisionFarming.EliminaRaccolteNonBloccate()"

        Dim xRisp As Boolean
        Dim MessaggioErrore As String = ""

        Try

            Dim stb As New StringBuilder

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete g " & vbCrLf)
            stb.Append(" from gis_entita e   " & vbCrLf)
            stb.Append("    inner join gis_elementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where [LayerElementiGrafici_Cod] = 66 " & vbCrLf)
            stb.Append(" and e.piva = '' " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete e " & vbCrLf)
            stb.Append(" from gis_entita e   " & vbCrLf)
            stb.Append(" where tipoEntita_cod = 0  " & vbCrLf)
            stb.Append(" and piva = '' " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete g " & vbCrLf)
            stb.Append(" from gis_entita e   " & vbCrLf)
            stb.Append("    inner join gis_elementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     inner join Mov_Destinazioni M  " & vbCrLf)
            stb.Append("         on e.piva = m.piva  " & vbCrLf)
            stb.Append("         and e.sa_cod = m.sa_cod  " & vbCrLf)
            stb.Append("         and e.appezza = m.appezza  " & vbCrLf)
            stb.Append("         and e.Id_Imp = m.id_destinazione " & vbCrLf)
            stb.Append("         and m.Tipo_Destinazione = 0  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join agenda A  " & vbCrLf)
            stb.Append("         on A.id_Agenda = M.id_agenda  " & vbCrLf)
            stb.Append("         and a.des_lib = 'Raccolta da importazione iMotion (  [])'   " & vbCrLf)
            stb.Append("         and a.blocco_flag = 0    " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where [LayerElementiGrafici_Cod] = 66 " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete e " & vbCrLf)
            stb.Append(" from gis_entita e       " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     inner join Mov_Destinazioni M  " & vbCrLf)
            stb.Append("         on e.piva = m.piva  " & vbCrLf)
            stb.Append("         and e.sa_cod = m.sa_cod  " & vbCrLf)
            stb.Append("         and e.appezza = m.appezza  " & vbCrLf)
            stb.Append("         and e.Id_Imp = m.id_destinazione " & vbCrLf)
            stb.Append("         and m.Tipo_Destinazione = 0  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join agenda A  " & vbCrLf)
            stb.Append("         on A.id_Agenda = M.id_agenda  " & vbCrLf)
            stb.Append("         and a.des_lib = 'Raccolta da importazione iMotion (  [])'   " & vbCrLf)
            stb.Append("         and a.blocco_flag = 0  " & vbCrLf)
            stb.Append(" where e.TipoEntita_cod = 0 " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return xRisp

    End Function


    Public Function EliminaRaccolteNonBloccate(ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.PrecisionFarming.EliminaRaccolteNonBloccate()"

        Dim xRisp As Boolean
        Dim MessaggioErrore As String = ""

        Try

            Dim stb As New StringBuilder

            stb.Clear()
            stb.Length = 0
            stb.Append(" update e " & vbCrLf)
            stb.Append("    set piva = '' " & vbCrLf)
            stb.Append("    , sa_cod = 0  " & vbCrLf)
            stb.Append("    , appezza = 0  " & vbCrLf)
            stb.Append("    , id_imp = 0  " & vbCrLf)
            stb.Append(" from gis_entita e  " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append("    inner join Mov_Destinazioni M " & vbCrLf)
            stb.Append("        on e.piva = m.piva " & vbCrLf)
            stb.Append("        and e.sa_cod = m.sa_cod " & vbCrLf)
            stb.Append("        and e.appezza = m.appezza " & vbCrLf)
            stb.Append("        and e.Id_Imp = m.id_destinazione " & vbCrLf)
            stb.Append("        and m.Tipo_Destinazione = 0 " & vbCrLf)
            stb.Append("    inner join agenda A " & vbCrLf)
            stb.Append("        on e.id_Agenda = A.id_agenda " & vbCrLf)
            stb.Append("        and a.des_lib = 'Raccolta da importazione iMotion (  [])'  " & vbCrLf)
            stb.Append("        and a.blocco_flag = 0   " & vbCrLf)
            stb.Append("             where g.LayerElementiGrafici_Cod = 66 " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete g  " & vbCrLf)
            stb.Append(" from gis_entita e " & vbCrLf)
            stb.Append("    inner join agenda A " & vbCrLf)
            stb.Append("        on e.id_Agenda = A.id_agenda " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append(" where a.des_lib = 'Raccolta da importazione iMotion (  [])'  " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0 " & vbCrLf)
            stb.Append(" and g.LayerElementiGrafici_Cod = 63")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete e  " & vbCrLf)
            stb.Append(" from gis_entita e " & vbCrLf)
            stb.Append("    inner join agenda A " & vbCrLf)
            stb.Append("        on e.id_Agenda = A.id_agenda     " & vbCrLf)
            stb.Append(" where a.des_lib = 'Raccolta da importazione iMotion (  [])'  " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0 " & vbCrLf)
            stb.Append(" and e.TipoEntita_Cod = 63")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete M  " & vbCrLf)
            stb.Append(" from Agenda A " & vbCrLf)
            stb.Append("    inner join  mov_dettaglio_tecnico_extra M " & vbCrLf)
            stb.Append("        on A.id_Agenda = M.ID_Agenda  " & vbCrLf)
            stb.Append(" where A.des_lib = 'Raccolta da importazione iMotion (  [])' " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            stb.Clear()
            stb.Length = 0
            stb.Append(" update sequenza_tabelle " & vbCrLf)
            stb.Append(" set ultimo_Valore = coalesce( (select max(entita_cod) from GIS_Entita  ), 0 ) " & vbCrLf)
            stb.Append(" where nome_tabella = 'GIS_Entita' " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" update sequenza_tabelle " & vbCrLf)
            stb.Append(" set ultimo_Valore = coalesce( (select max(ElementoGrafico_cod) from gis_elementigrafici  ), 0 ) " & vbCrLf)
            stb.Append(" where nome_tabella = 'gis_elementigrafici' " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete M " & vbCrLf)
            stb.Append(" from Agenda A " & vbCrLf)
            stb.Append("    inner join  Mov_Dettaglio_Tecnico M " & vbCrLf)
            stb.Append("        on A.id_Agenda = M.ID_Agenda " & vbCrLf)
            stb.Append(" where A.des_lib = 'Raccolta da importazione iMotion (  [])' " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete M " & vbCrLf)
            stb.Append(" from Agenda A " & vbCrLf)
            stb.Append("    inner join  Mov_Destinazioni M " & vbCrLf)
            stb.Append("        on A.id_Agenda = M.ID_Agenda " & vbCrLf)
            stb.Append(" where A.des_lib = 'Raccolta da importazione iMotion (  [])' " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete M " & vbCrLf)
            stb.Append(" from  Agenda A " & vbCrLf)
            stb.Append("    inner join  Movimenti_dettagli M " & vbCrLf)
            stb.Append("        on A.id_Agenda = M.ID_Agenda " & vbCrLf)
            stb.Append(" where A.des_lib = 'Raccolta da importazione iMotion (  [])' " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete M " & vbCrLf)
            stb.Append(" from Agenda A " & vbCrLf)
            stb.Append("    inner join  Movimenti M " & vbCrLf)
            stb.Append("        on A.id_Agenda = M.ID_Agenda " & vbCrLf)
            stb.Append(" where A.des_lib = 'Raccolta da importazione iMotion (  [])' " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb.Clear()
            stb.Length = 0
            stb.Append(" delete A  " & vbCrLf)
            stb.Append(" from agenda A " & vbCrLf)
            stb.Append(" where A.des_lib = 'Raccolta da importazione iMotion (  [])' " & vbCrLf)
            stb.Append(" and A.BLocco_Flag = 0  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '''
            ''' DRUDI 19/06/2024 Commentato perché abbiamo iniziato a utilizzare le Sequence di SQL Server
            '''
            'stb.Clear()
            'stb.Length = 0
            'stb.Append(" update sequenza_tabelle " & vbCrLf)
            'stb.Append(" set ultimo_Valore = coalesce( (select max(Id_Agenda) from Agenda  ), 0 ) " & vbCrLf)
            'stb.Append(" where nome_tabella = 'AGENDA' " & vbCrLf)

            ''--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            ''--------------------------------------------------------------------------

            'stb.Clear()
            'stb.Length = 0
            'stb.Append(" update sequenza_tabelle " & vbCrLf)
            'stb.Append(" set ultimo_Valore = coalesce( (select max(id_mov) from MOVIMENTI  ), 0 ) " & vbCrLf)
            'stb.Append(" where nome_tabella = 'MOVIMENTI' " & vbCrLf)

            ''--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            ''--------------------------------------------------------------------------

            'stb.Clear()
            'stb.Length = 0
            'stb.Append(" update sequenza_tabelle " & vbCrLf)
            'stb.Append(" set ultimo_Valore = coalesce( (select max(id_mov_det) from MOVIMENTI_DETTAGLI  ), 0 ) " & vbCrLf)
            'stb.Append(" where nome_tabella = 'MOVIMENTI_DETTAGLI' " & vbCrLf)

            ''--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            ''--------------------------------------------------------------------------

            'stb.Clear()
            'stb.Length = 0
            'stb.Append(" update sequenza_tabelle " & vbCrLf)
            'stb.Append(" set ultimo_Valore = coalesce( (select max(id_reg_dettaglio) from mov_dettaglio_tecnico  ), 0 ) " & vbCrLf)
            'stb.Append(" where nome_tabella = 'MOVIMENTI_DETTAGLI_TECNICI' " & vbCrLf)

            ''--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            ''--------------------------------------------------------------------------

            'stb.Clear()
            'stb.Length = 0
            'stb.Append(" update sequenza_tabelle " & vbCrLf)
            'stb.Append(" set ultimo_Valore = coalesce( (select max(Id_Agenda) from Agenda  ), 0 ) " & vbCrLf)
            'stb.Append(" where nome_tabella = 'AGENDA' " & vbCrLf)

            ''--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            ''--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return xRisp

    End Function



    Public Sub ReportPercorsi_Raccolte_InfoByReplaces(ByVal NomeCampo As String, ByVal aliasInfo_Aggiuntive As String, ByVal Replaces As String, ByRef stb As StringBuilder)

        Dim rP As String() = Replaces.Split("*")

        Dim iContaRP As Integer = 0
        For Each rPCur In rP

            If iContaRP = 0 Then
                stb.Append(", ")
            End If
            For i = 0 To iContaRP
                stb.Append(vbTab)
            Next
            stb.Append("    replace ( " & vbCrLf)
            iContaRP += 1
        Next

        '----------

        iContaRP = rP.Length
        tabMe(iContaRP, stb)

        stb.Append(NomeCampo & vbCrLf)
        For Each rPCur In rP




            Dim sp1 As String() = rPCur.Split("#")

            tabMe(iContaRP, stb)
            stb.Append(", '" & Agro_SQL_SaveText(sp1(0)) & "'" & vbCrLf)

            tabMe(iContaRP, stb)
            stb.Append(", '" & Agro_SQL_SaveText(sp1(1)) & "'" & vbCrLf)

            tabMe(iContaRP, stb)
            stb.Append(")" & vbCrLf)

            iContaRP -= 1

        Next

        stb.Append(" as " & aliasInfo_Aggiuntive & vbCrLf)

    End Sub

    Public Sub tabMe(ByVal iContaRp As Integer, ByRef stb As StringBuilder)
        For i = iContaRp To 0 Step -1
            stb.Append(vbTab)
        Next

    End Sub

    Public Sub ReportPercorsi_Raccolte_Info(ByRef stb As StringBuilder)

        stb.Append(", replace ( " & vbCrLf)
        stb.Append("    replace ( " & vbCrLf)
        stb.Append("        replace ( " & vbCrLf)
        stb.Append("            replace (    " & vbCrLf)
        stb.Append("                  ElementoGrafico_Des " & vbCrLf)
        stb.Append("                , '|Plate§' " & vbCrLf)
        stb.Append("                , 'ID GPS: ' " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("            ) " & vbCrLf)
        stb.Append("            , '|from_time§ ' " & vbCrLf)
        stb.Append("            , ', Data Ora Inizio: ' " & vbCrLf)
        stb.Append("        ) " & vbCrLf)
        stb.Append("        , '|to_time§ ' " & vbCrLf)
        stb.Append("        , ', Data Ora Fine: ' " & vbCrLf)
        stb.Append("    ) " & vbCrLf)
        stb.Append("    , '|isStop§ 1' " & vbCrLf)
        stb.Append("    , '' " & vbCrLf)
        stb.Append(" ) as Info_Aggiuntive " & vbCrLf)


    End Sub


    Public Sub ReportPercorsi_Raccolte_GPS(ByRef stb As StringBuilder)


        stb.Append(" ,SUBSTRING ( " & vbCrLf)
        stb.Append("        RIGHT ( " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("            replace(replace(coalesce(a.Poligono_GeoEntity.STAsText(), ''), 'POINT (', ''), ')', '') " & vbCrLf)
        stb.Append("            , len(replace(replace(coalesce(a.Poligono_GeoEntity.STAsText(), ''), 'POINT (', ''), ')', '')) " & vbCrLf)
        stb.Append("            - charindex(         " & vbCrLf)
        stb.Append("         ' ',  " & vbCrLf)
        stb.Append("                replace(replace(coalesce(a.Poligono_GeoEntity.STAsText(), ''), 'POINT (', ''), ')', '') " & vbCrLf)
        stb.Append("                , 0 " & vbCrLf)
        stb.Append("            ) " & vbCrLf)
        stb.Append("        ) " & vbCrLf)
        stb.Append("    , 0, 12) " & vbCrLf)
        stb.Append("    + ' ' +  " & vbCrLf)
        stb.Append("    SUBSTRING ( " & vbCrLf)
        stb.Append("        SUBSTRING ( " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("            replace(replace(coalesce(a.Poligono_GeoEntity.STAsText(), ''), 'POINT (', ''), ')', '') " & vbCrLf)
        stb.Append("            , 0 " & vbCrLf)
        stb.Append("            , charindex(         " & vbCrLf)
        stb.Append("         ' ',  " & vbCrLf)
        stb.Append("                replace(replace(coalesce(a.Poligono_GeoEntity.STAsText(), ''), 'POINT (', ''), ')', '') " & vbCrLf)
        stb.Append("                , 0 " & vbCrLf)
        stb.Append("            ) " & vbCrLf)
        stb.Append("        )  " & vbCrLf)
        stb.Append("    , 0, 12) " & vbCrLf)
        stb.Append("    as GPS " & vbCrLf)

    End Sub

    Public Function ReportGiasPathFinderDettaglio(
        ByVal DataDa As Date,
        ByVal DataA As Date,
        ByVal FiltroTestata As String,
        ByVal FiltroUtenti As String,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
    ) As DataTable


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.precisionFarming.ReportGiasPathFinder("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

              Dim NomeDB_Utenti As String =
                objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


            stb.AppendLine("   Select ")
            stb.AppendLine("     cast(vehicle_id As varchar(100)) + '_' + cast( [date] as varchar(50)) + '_' + cast(  From_time as varchar(1000)) as id  ")
            stb.AppendLine("   , vehicle_id  ")
            stb.AppendLine("   , v.Plate as N_telefono  ")
            stb.AppendLine("   , v.Name as Descrizione  ")
            stb.AppendLine("   , cast([date] as datetime) as Data  ")
            stb.AppendLine("   , dateadd(s,  p.[timestamp], cast( '1970-01-01' as datetime) )  as DataOra ")
            stb.AppendLine("   , p.speed * 3.6 as Velocita  ")
            stb.AppendLine("   , p.Altitude as Quota ")
            stb.AppendLine("   , p.heading as direzione ")
            stb.AppendLine("    ")
            stb.AppendLine("  From iMotion_percorsi_dettagli_pos p  ")
            stb.AppendLine("   inner Join iMotion_Veicoli v  ")
            stb.AppendLine("          On p.vehicle_id = v.id  ")
            If Not String.IsNullOrEmpty(FiltroUtenti) Then
                stb.AppendLine("   inner join " & NomeDB_Utenti & ".dbo.utenti_dettagli u ")
                stb.AppendLine("      on u.tel = v.plate")
            End If
            stb.AppendLine("  where cast([Date] As datetime) between  CONVERT(DateTime,'1900/01/01',120)  AND  CONVERT(DateTime,'2100/12/31',120)      ")

            ' esempio: "  '2_2018-03-07_1520423008' "
            If Not String.IsNullOrEmpty(FiltroTestata) Then
                stb.AppendLine("  And cast(vehicle_id As varchar(100)) + '_' + cast( [date] as varchar(50)) + '_' + cast(  From_time as varchar(1000)) in ( ")
                stb.AppendLine(Agro_SQL_Save_Clausola_IN(FiltroTestata, True))
                stb.AppendLine("  )")

            End If

            
            If Not String.IsNullOrEmpty(FiltroUtenti) Then
                stb.AppendLine(" and u.username in (" & Agro_SQL_Save_Clausola_IN(FiltroUtenti, True) & ") ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function ReportGiasPathFinder(
        ByVal DataDa As Date,
        ByVal DataA As Date,
        ByVal FiltroUtenti As String,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
    ) As DataTable


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.precisionFarming.ReportGiasPathFinder("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

              Dim NomeDB_Utenti As String =
                objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


            stb.AppendLine("   Select ")
            stb.AppendLine("    cast(vehicle_id as varchar(100)) + '_' + cast( [date] as varchar(50)) + '_' + cast(  min([timestamp]) as varchar(1000)) as id ")
            stb.AppendLine("  , vehicle_id ")
            stb.AppendLine("  , v.Plate as N_telefono ")
            stb.AppendLine("  , v.Name as Descrizione ")
            stb.AppendLine("  , cast([date] as datetime) as Data ")
            stb.AppendLine("  , dateadd(s,  min([timestamp]), cast( '1970-01-01' as datetime) )  as DataOraPartenza ")
            stb.AppendLine("  , dateadd(s,  max([timestamp]), cast( '1970-01-01' as datetime) )     as DataOraArrivo ")
            stb.AppendLine("  , cast(( max([timestamp]) - min([timestamp]) ) / 60 as int) as Durata ")
            stb.AppendLine(" From iMotion_percorsi_dettagli_pos p ")
            stb.AppendLine("  inner Join iMotion_Veicoli v ")
            stb.AppendLine("         On p.vehicle_id = v.id ")
            stb.AppendLine("  ")
            
            If Not String.IsNullOrEmpty(FiltroUtenti) Then
                stb.AppendLine("   inner join " & NomeDB_Utenti & ".dbo.utenti_dettagli u ")
                stb.AppendLine("      on u.tel = v.plate")
            End If

            stb.AppendLine(" where cast([Date] As datetime) between " & Agro_SQL_SaveDate(DataDa) & " AND " & Agro_SQL_SaveDate(DataA) & "    ")

            
            If Not String.IsNullOrEmpty(FiltroUtenti) Then
                stb.AppendLine(" and u.username in (" & Agro_SQL_Save_Clausola_IN(FiltroUtenti, True) & ") ")
            End If

            stb.AppendLine(" group by vehicle_id, [date], from_time, to_time, v.Plate, v.name")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function


    Public Function ReportUltimaPosizioneGiasPathFinder(
        ByVal DataDa As Date,
        ByVal DataA As Date,
        ByVal FiltroUtenti As String,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
    ) As DataTable


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.precisionFarming.ReportGiasPathFinder("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            Dim NomeDB_Utenti As String =
                objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


            stb.AppendLine(" Select ")
            stb.AppendLine("    id ")
            stb.AppendLine("  , Plate as N_telefono ")
            stb.AppendLine("  , Name as Descrizione ")
            stb.AppendLine("  , Latitudine")
            stb.AppendLine("  , Longitudine")
            stb.AppendLine("  , speed * 3.6 as Velocita")
            stb.AppendLine("  , Heading as Direzione")
            stb.AppendLine("  , Altitudine as Quota")
            stb.AppendLine(" , dateadd(s,  [UltimaPosizioneTimestamp], cast( '1970-01-01' as datetime) )  as DataOra ")

            stb.AppendLine(" From [dbo].[iMotion_Veicoli] v ")

            If Not String.IsNullOrEmpty(FiltroUtenti) Then

                stb.AppendLine("  inner join Rubrica r ") 
                stb.AppendLine("      on r.numero = v.plate ") 
                stb.AppendLine("      and r.descr = 'Cellulare:' ") 
                stb.AppendLine("  inner join ContattiXRubrica cr ") 
                stb.AppendLine("      on cr.Cod_Rubrica = r.cod_rubrica ") 
                stb.AppendLine("  inner join contatti cc ") 
                stb.AppendLine("      on cc.piva = cr.piva ") 
                stb.AppendLine("      and cc.cod_contatto = cr.cod_contatto        ") 
                stb.AppendLine("  inner join ContattiXUtentiGias cug ") 
                stb.AppendLine("      on cc.piva = cug.piva ") 
                stb.AppendLine("      and cc.cod_contatto = cug.cod_contatto  "  )

                
            End If

            stb.AppendLine(" where dateadd(s,  [UltimaPosizioneTimestamp], cast( '1970-01-01' as datetime) ) between " & Agro_SQL_SaveDate(DataDa) & " AND " & Agro_SQL_SaveDate(DataA) & "    ")

            If Not String.IsNullOrEmpty(FiltroUtenti) Then
                stb.AppendLine(" and cug.username in (" & Agro_SQL_Save_Clausola_IN(FiltroUtenti, True) & ") ")
                stb.AppendLine(" and cc.Piva = '" & objParametri_Server.PivaSuperUser & "'"  )
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function


    Public Function ReportPercorsi_Raccolte(
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.precisionFarming.ReportPercorsi_Raccolte("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            Dim stringRlp As String =
                "|vehicle_id§ #GPS ID: *vehicle_plate#Targa*vehicle_name#Nome*from_time#Data ora inizio*to_time#Data Ora Fine*isStop§ 1#*|# - *§#:"

            stb.Append("            select  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("      ic.val_cod as CUAA " & vbCrLf)
            stb.Append("    , i.rag_soc as RagioneSociale " & vbCrLf)
            stb.Append("    , ind.ind_des as Indirizzo " & vbCrLf)
            stb.Append("    , ind.frz_des as Località " & vbCrLf)
            stb.Append("    , ind.CAP as CAP " & vbCrLf)
            stb.Append("    , ind.pro_cod as Provincia " & vbCrLf)
            stb.Append("    , sa.sa_nome as Centro " & vbCrLf)
            stb.Append("    , app.APP_NOME as [App.to] " & vbCrLf)
            stb.Append("    , app.via_stringa as [Indirizzo App.to]" & vbCrLf)
            stb.Append("    , veg.veg_Des as Specie " & vbCrLf)
            stb.Append("    , cc.cul_Des as Varietà " & vbCrLf)

            ReportPercorsi_Raccolte_GPS(stb)

            stb.Append("    , case when Racc.DataRaccolta is null then 'Impianto non Associato a Raccolte' else 'Raccolta OK' end as Stato_Raccolta " & vbCrLf)

            'ReportPercorsi_Raccolte_Info(stb)
            ReportPercorsi_Raccolte_InfoByReplaces(
                "a.ElementoGrafico_Des",
                "Info_Aggiuntive",
                stringRlp,
                stb
            )

            stb.Append("  " & vbCrLf)
            stb.Append(" from  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" reg_impianti reg " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" inner join imprese i " & vbCrLf)
            stb.Append("    on reg.piva = i.piva  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" inner join centri_aziendali sa " & vbCrLf)
            stb.Append("    on reg.piva = sa.piva " & vbCrLf)
            stb.Append("    and reg.sa_cod = sa.sa_cod " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" inner join appezzamento app " & vbCrLf)
            stb.Append("    on reg.piva = app.piva  " & vbCrLf)
            stb.Append("    and reg.Sa_Cod = app.SA_COD " & vbCrLf)
            stb.Append("    and reg.Appezza = app.appezza " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" inner join Cultivar cc " & vbCrLf)
            stb.Append("    on cc.Cul_Cod = reg.CUL_COD " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" inner join SpecieVegetali veg " & vbCrLf)
            stb.Append("    on veg.Veg_Cod = cc.veg_cod " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" inner join imprese_codici ic " & vbCrLf)
            stb.Append("    on ic.piva = i.piva " & vbCrLf)
            stb.Append("    and ic.id_cod = 1010 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" left join impresexindirizzi ii " & vbCrLf)
            stb.Append("    on i.piva =  ii.PIVA " & vbCrLf)
            stb.Append("    and ii.Tipo_Indirizzo = 1 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" left join Indirizzi ind " & vbCrLf)
            stb.Append("    on ind.cod_indirizzo = ii.cod_indirizzo " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" left join  " & vbCrLf)
            stb.Append(" ( " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    select  " & vbCrLf)
            stb.Append("      G.ElementoGrafico_Des " & vbCrLf)
            stb.Append("    , G.Poligono_GEoEntity " & vbCrLf)
            stb.Append("    , E.Piva " & vbCrLf)
            stb.Append("    , E.sa_cod " & vbCrLf)
            stb.Append("    , E.Appezza " & vbCrLf)
            stb.Append("    , E.id_imp " & vbCrLf)
            stb.Append("    , E.Entita_Cod " & vbCrLf)
            stb.Append("    , E.ID_Agenda " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    from GIS_Entita E " & vbCrLf)
            stb.Append("        inner join Gis_ElementiGrafici G " & vbCrLf)
            stb.Append("            on E.Entita_cod = G.Entita_cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    where G.LayerElementiGrafici_Cod = 66 " & vbCrLf)
            stb.Append("    and substring( g.Poligono_GeoEntity.STAsText(), 1, 5) = 'POINT' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" ) a " & vbCrLf)
            stb.Append(" on reg.piva = a.piva " & vbCrLf)
            stb.Append("    and reg.SA_cod = a.sa_Cod " & vbCrLf)
            stb.Append("    and reg.Appezza = a.Appezza " & vbCrLf)
            stb.Append("    and reg.ID_REG = a.id_imp " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" --Per join raccolte.. " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" left join (  " & vbCrLf)
            stb.Append("    select  " & vbCrLf)
            stb.Append("          E1.piva " & vbCrLf)
            stb.Append("        , E1.Sa_Cod  " & vbCrLf)
            stb.Append("        , E1.Appezza " & vbCrLf)
            stb.Append("        , E1.id_imp " & vbCrLf)
            stb.Append("        , min(M.Data_Movimento) as DataRaccolta " & vbCrLf)
            stb.Append("        from GIS_Entita E1 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("        inner join Movimenti M " & vbCrLf)
            stb.Append("            on E1.id_agenda =  M.id_agenda " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("        group by  " & vbCrLf)
            stb.Append("          E1.piva " & vbCrLf)
            stb.Append("        , E1.Sa_Cod " & vbCrLf)
            stb.Append("        , E1.Appezza " & vbCrLf)
            stb.Append("        , E1.id_imp " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" ) Racc " & vbCrLf)
            stb.Append("    on Racc.piva = a.piva " & vbCrLf)
            stb.Append("    and Racc.SA_cod = a.sa_Cod " & vbCrLf)
            stb.Append("    and Racc.Appezza = a.Appezza " & vbCrLf)
            stb.Append("    and Racc.id_imp = a.id_imp " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" union all " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("      null as CUAA " & vbCrLf)
            stb.Append("    , null as RagioneSociale " & vbCrLf)
            stb.Append("    , null as Indirizzo " & vbCrLf)
            stb.Append("    , null as Località " & vbCrLf)
            stb.Append("    , null as CAP " & vbCrLf)
            stb.Append("    , null as Provincia " & vbCrLf)
            stb.Append("    , null as Centro " & vbCrLf)
            stb.Append("    , null as [App.to] " & vbCrLf)
            stb.Append("    , null as [Indirizzo App.to] " & vbCrLf)
            stb.Append("    , null as Specie " & vbCrLf)
            stb.Append("    , null as Varietà " & vbCrLf)

            ReportPercorsi_Raccolte_GPS(stb)

            stb.Append("    , 'Raccolta non associata ad impianto' as Stato_Raccolta " & vbCrLf)

            'ReportPercorsi_Raccolte_Info(stb)
            ReportPercorsi_Raccolte_InfoByReplaces(
                "a.ElementoGrafico_Des",
                "Info_Aggiuntive",
                stringRlp,
                stb
            )

            stb.Append("  " & vbCrLf)
            stb.Append(" from GIS_ElementiGrafici a " & vbCrLf)
            stb.Append("    inner join gis_entita e " & vbCrLf)
            stb.Append("        on e.entita_cod = a.entita_cod " & vbCrLf)
            stb.Append(" where a.LayerElementiGrafici_Cod = 66 " & vbCrLf)
            stb.Append("    and substring( a.Poligono_GeoEntity.STAsText(), 1, 5) = 'POINT' " & vbCrLf)
            stb.Append("    and E.id_imp = 0 " & vbCrLf)



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


    Public Function verificaCancellaDatoPlanning(
                                        ByVal programmazione_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.precisionFarming.ProponiDescrizioneImpiantiAggregati("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder


            stb.Append(" Select 1 " & vbCrLf)
            stb.Append(" from programmazione_Entita e " & vbCrLf)
            stb.Append("    inner join Programmazione_Testata t " & vbCrLf)
            stb.Append("        on t.Programmazione_Cod = e.Programmazione_Cod  " & vbCrLf)
            'stb.Append("        and t.Tipo_Pianificazione = 10 " & vbCrLf)
            stb.Append("    inner join Ricette_Destinazioni d " & vbCrLf)
            stb.Append("    on e.Piva = d.Piva  " & vbCrLf)
            stb.Append("    and e.Sa_Cod = d.Sa_Cod  " & vbCrLf)
            stb.Append("    and e.appezza = d.appezza " & vbCrLf)
            stb.Append("    and e.Id_Reg = d.Id_Reg  " & vbCrLf)
            stb.Append(" where e.Programmazione_Cod = " & programmazione_Cod & vbCrLf)



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
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="sa_cod"></param>
    ''' <param name="FiltroSuImpianti"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProponiDescrizioneImpiantiAggregati(
                                        ByVal piva As String,
                                        ByVal sa_cod As Integer,
                                        ByVal FiltroSuImpianti As String,
                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.precisionFarming.ProponiDescrizioneImpiantiAggregati("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder


            stb.Append("declare @piva varchar(100) " & vbCrLf)
            stb.Append(" declare @Sa_cod int " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" set @piva = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
            stb.Append(" set @Sa_cod = " & Agro_SQL_SaveNum(sa_cod) & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" select distinct sa.sa_nome, coalesce(c.Campo_Des, '') as campo_des, a.app_nome, coalesce (veg.veg_des, '') as veg_des" & vbCrLf)
            stb.Append(" from Reg_Impianti rr " & vbCrLf)
            stb.Append("    inner join Appezzamento a " & vbCrLf)
            stb.Append("        on rr.PIVA = a.piva  " & vbCrLf)
            stb.Append("        and rr.SA_COD = a.SA_COD  " & vbCrLf)
            stb.Append("        and rr.APPEZZA = a.APPEZZA  " & vbCrLf)
            stb.Append("    inner join Centri_Aziendali sa " & vbCrLf)
            stb.Append("        on sa.PIVA =a.PIVA  " & vbCrLf)
            stb.Append("        and sa.sa_cod = a.SA_COD  " & vbCrLf)
            stb.Append("    left join Campi c " & vbCrLf)
            stb.Append("        on c.Piva = a.PIVA  " & vbCrLf)
            stb.Append("        and c.Sa_Cod = a.SA_COD  " & vbCrLf)
            stb.Append("        and c.Campo_Cod = a.Campo_Cod  " & vbCrLf)
            stb.Append("    left join cultivar cul  " & vbCrLf)
            stb.Append("        on rr.cul_cod = cul.cul_cod " & vbCrLf)
            stb.Append("    left join SpecieVegetali veg " & vbCrLf)
            stb.Append("        on veg.Veg_Cod = cul.Veg_Cod  " & vbCrLf)

            stb.Append(" where rr.PIVA = @Piva  " & vbCrLf)
            stb.Append(" and rr.SA_COD = @sa_cod " & vbCrLf)
            stb.Append(" and (  " & vbCrLf)
            stb.Append(FiltroSuImpianti & vbCrLf)
            stb.Append(" ) " & vbCrLf)


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

    Public Function LeggiPlanningAssociatoImpiantoDataRicetta(
                                    ByVal Ricetta_Operazione_cod As Integer,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder
            stb.Append("select distinct Entita.pivaSuperUser, Entita.programmazione_cod " & vbCrLf)
            stb.Append(" from Ricette_Destinazioni rd " & vbCrLf)
            stb.Append("    inner join Ricette_Destinazioni D " & vbCrLf)
            stb.Append("        on rd.Ricetta_Operazione_Cod = D.Ricetta_Operazione_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("      inner join Programmazione_Entita ee " & vbCrLf)
            stb.Append("        on ee.piva = rd.Piva   " & vbCrLf)
            stb.Append("          and ee.sa_cod = rd.sa_cod   " & vbCrLf)
            stb.Append("          and ee.Appezza = rd.Appezza    " & vbCrLf)
            stb.Append("          and ee.Id_Reg = rd.Id_Reg   " & vbCrLf)
            stb.Append("       inner join GIS_Entita Entita    " & vbCrLf)
            stb.Append("          on Entita.Programmazione_cod = ee.programmazione_cod " & vbCrLf)
            stb.Append("          and Entita.Programmazione_entita_cod = 0 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where rd.Ricetta_Operazione_Cod = " & Ricetta_Operazione_cod & vbCrLf)



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


    Public Function RimuoviSegmentiInutilizzati(
                                       ByVal Planning1Impianti2 As Integer,
                                       ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As Boolean
        Dim rval As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder


            stb.Append(" create table #toDelete(Entita_Cod int) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" insert #toDelete " & vbCrLf)
            stb.Append(" select distinct ABL.Entita_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from ( " & vbCrLf)
            stb.Append(" select ee.entita_Cod, ee.Piva, ee.Sa_Cod, ee.Appezza, ee.Id_Imp, ee.programmazione_cod, veg.Veg_Des, c.Cul_Des, aa.app_nome, g.elementoGrafico_des, g.Poligono_GeoEntity   " & vbCrLf)
            stb.Append(" from GIS_Entita ee " & vbCrLf)
            stb.Append("    inner join gis_elementigrafici g " & vbCrLf)
            stb.Append("    on ee.Entita_Cod = g.Entita_Cod  " & vbCrLf)

            If Planning1Impianti2 = 1 Then
                stb.Append(" inner join Programmazione_Entita i " & vbCrLf)
                stb.Append("    on ee.programmazione_cod = i.Programmazione_Cod  " & vbCrLf)

            Else


                stb.Append("    inner join Reg_Impianti i " & vbCrLf)
                stb.Append("        on i.PIVA = ee.Piva  " & vbCrLf)
                stb.Append("        and i.SA_COD = ee.Sa_Cod  " & vbCrLf)
                stb.Append("        and i.APPEZZA = ee.appezza " & vbCrLf)
                stb.Append("        and i.id_Reg = ee.id_imp " & vbCrLf)
            End If

            stb.Append("    inner join Cultivar c " & vbCrLf)
            stb.Append("        on i.CUL_COD = c.Cul_Cod  " & vbCrLf)
            stb.Append("    inner join SpecieVegetali veg " & vbCrLf)
            stb.Append("        on veg.Veg_Cod = c.Veg_Cod  " & vbCrLf)
            stb.Append("    inner join Appezzamento aa " & vbCrLf)
            stb.Append("    on aa.PIVA = ee.Piva  " & vbCrLf)
            stb.Append("    and aa.SA_COD = ee.Sa_Cod  " & vbCrLf)
            stb.Append("    and aa.APPEZZA = ee.Appezza  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where 1=1 " & vbCrLf)
            stb.Append(" and g.layerElementiGrafici_Cod in (55) " & vbCrLf)
            stb.Append(" and g.elementoGrafico_Des not in ('a','b') " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" ) ABL " & vbCrLf)
            stb.Append(" left join (  " & vbCrLf)
            stb.Append("    select ee.Entita_Cod , ee.Piva, ee.Sa_Cod, ee.Appezza, ee.Id_Imp, ee.programmazione_cod, g1.elementoGrafico_des, g1.Poligono_GeoEntity  " & vbCrLf)
            stb.Append("    from gis_entita ee " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici g1 " & vbCrLf)
            stb.Append("        on ee.Entita_Cod = g1.Entita_Cod ) e1 " & vbCrLf)
            stb.Append("    on ABL.Piva = e1.Piva  " & vbCrLf)
            stb.Append("    and ABL.Sa_Cod = e1.Sa_Cod  " & vbCrLf)
            stb.Append("    and ABL.Appezza = e1.Appezza  " & vbCrLf)
            stb.Append("    and ABL.Id_Imp = e1.id_imp " & vbCrLf)
            stb.Append("and ABL.programmazione_cod = e1.programmazione_cod  " & vbCrLf)

            stb.Append("    and e1.elementoGrafico_Des = 'a' " & vbCrLf)
            stb.Append("    and e1.Poligono_GeoEntity.STIntersects(abl.Poligono_GeoEntity) <> 1 " & vbCrLf)
            stb.Append(" where e1.piva is not null " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" delete from GIS_ElementiGrafici where Entita_Cod in (select Entita_Cod from #toDelete ) " & vbCrLf)
            stb.Append(" delete from GIS_Entita where Entita_Cod in (select Entita_Cod from #toDelete ) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" drop table #toDelete " & vbCrLf)


            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return rval
    End Function

    Public Function LeggiDatiAB_DaEntitaCod(
                                    ByVal Entita_cod As Integer,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim StrSQL As New System.Text.StringBuilder
            LeggiDatiAB_DaEntitaCod_getQuery(Entita_cod, StrSQL)


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
    Public Function LeggiDatiAB_DaEntitaCod_getQuery(ByVal Entita_cod As Integer, ByRef stb As StringBuilder) As DataTable
        stb.Append(" select distinct e1.Entita_Cod, g1.ElementoGrafico_Des  " & vbCrLf)
        stb.Append(" from  " & vbCrLf)
        stb.Append(" (SELECT      " & vbCrLf)
        stb.Append("          ee.PivaSuperUser " & vbCrLf)
        stb.Append("        , ee.Entita_Cod " & vbCrLf)
        stb.Append("        , ee.TipoEntita_Cod " & vbCrLf)
        stb.Append("        , ee.Piva " & vbCrLf)
        stb.Append("        , ee.Sa_Cod " & vbCrLf)
        stb.Append("        , ee.Appezza " & vbCrLf)
        stb.Append("        , ee.Campo_Cod " & vbCrLf)
        stb.Append("        , ee.Id_Imp  " & vbCrLf)
        stb.Append("        , gg.ElementoGrafico_Des     " & vbCrLf)
        stb.Append(" FROM         GIS_Entita AS ee INNER JOIN " & vbCrLf)
        stb.Append("                       GIS_ElementiGrafici AS gg ON ee.Entita_Cod = gg.Entita_Cod " & vbCrLf)
        stb.Append(" WHERE     (gg.ElementoGrafico_Cod = " & Entita_cod & ") " & vbCrLf)
        stb.Append(" ) ee " & vbCrLf)
        stb.Append(" inner join gis_entita e1 " & vbCrLf)
        stb.Append(" on ee.PivaSuperUser = e1.PivaSuperUser " & vbCrLf)
        stb.Append(" and ee.Piva= e1.Piva " & vbCrLf)
        stb.Append(" and ee.Sa_Cod= e1.Sa_Cod " & vbCrLf)
        stb.Append(" and ee.Appezza= e1.Appezza " & vbCrLf)
        stb.Append(" and ee.Campo_Cod= e1.Campo_Cod " & vbCrLf)
        stb.Append(" and ee.Id_Imp= e1.Id_Imp " & vbCrLf)
        stb.Append("inner join gis_elementiGrafici g1 " & vbCrLf)
        stb.Append(" on g1.Entita_Cod = e1.Entita_Cod  " & vbCrLf)

    End Function
    Public Function LeggiLineeGuidaAB(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal TipoEntita_Cod As Int32,
                            ByVal PivaPadre As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal ID_Imp As Int32,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32,
                            ByVal Numero As Int32,
                            ByVal Subalterno As String,
                            ByVal ID_Agenda As Int32,
                            ByVal Programmazione_cod As Integer,
                            ByVal Programmazione_Entita_cod As Integer,
                            ByVal Ricetta_Operazione_Cod As Integer,
                            ByVal ListaContatti As String,
                            ByVal LeggiContattiMacchineOppureDatoFinale As Integer,
                            ByVal dataDa As DateTime,
                            ByVal dataa As DateTime,
                            ByVal layerElementiPF As Integer,
                            ByVal Codice_Fiscale_Tecnico As String,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim StrSQL As New System.Text.StringBuilder
            LeggiGetQueryAB(PivaSuperUser, Entita_Cod, TipoEntita_Cod, PivaPadre, Piva, Sa_Cod, Appezza, Campo_Cod, ID_Imp, Prov, Com, Sezione, Foglio, Numero, Subalterno, ID_Agenda, Programmazione_cod, Programmazione_Entita_cod, Ricetta_Operazione_Cod, ListaContatti, xFiltroAggiuntivo, xOrderBy, objParametri, StrSQL, False, LeggiContattiMacchineOppureDatoFinale, dataDa, dataa, layerElementiPF, Codice_Fiscale_Tecnico)

            SettaParametriPrecedenti(DammiParametriCollezionati)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return DT

    End Function


    Public Function LeggiImpiantiPerVicinanzaPuntiStop(
            ByVal PivaSuperUser As String,
            ByVal Punto_stop_layerElementiGrafici_Cod As Int32,
            ByVal Impianti_layerElementiGrafici_Cod As String,
            ByVal DataRaccolta_ElementoGrafico_DaEstrarre As String,
            ByVal DataRaccolta_ElementoGraficoSuccessivoAQuelloDaEstrarre As String,
            ByVal DistanzaAttiva As Double,
            ByVal TipoEntita_Cod As Int32,
            ByVal Leggi_Solo_Se_Non_Assegnati_Raccolta As Boolean,
            ByVal xFiltroAggiuntivo_Punto_Stop As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try

            Dim stb As New StringBuilder

            stb.Length = 0
            stb.Append(" select * " & vbCrLf)
            stb.Append(" from ( " & vbCrLf)
            stb.Append("    select  " & vbCrLf)
            stb.Append("          a.Poligono_GeoEntity.STDistance(b.Poligono_GeoEntity) as Distanza " & vbCrLf)
            stb.Append("        , a.Entita_Cod as Punto_Stop_Entita_Cod      " & vbCrLf)

            If DataRaccolta_ElementoGrafico_DaEstrarre <> "" Then
                stb.Append("        , " & vbCrLf)

                QRY_GetFrom_ElementoGraficoDES_Piped(
                    ElementoGrafico_DaEstrarre:=DataRaccolta_ElementoGrafico_DaEstrarre,
                    ElementoGraficoSuccessivoAQuelloDaEstrarre:=DataRaccolta_ElementoGraficoSuccessivoAQuelloDaEstrarre,
                    stb:=stb,
                    NomeColonnaAlias:="DataORA_Inizio_Raccolta"
                )
            End If

            stb.Append("        , b.* " & vbCrLf)
            stb.Append("    from  " & vbCrLf)
            stb.Append("    ( select  " & vbCrLf)
            stb.Append("          PivaSuperUser " & vbCrLf)
            stb.Append("        , Entita_Cod " & vbCrLf)
            stb.Append("        , ElementoGrafico_Cod " & vbCrLf)
            stb.Append("        , ElementoGrafico_Des " & vbCrLf)
            stb.Append("        , poligono_GeoEntity " & vbCrLf)
            stb.Append("    from gis_elementiGrafici " & vbCrLf)
            stb.Append("    where layerElementiGrafici_Cod = " & Agro_SQL_SaveNum(Punto_stop_layerElementiGrafici_Cod) & vbCrLf)

            'stb.Append("    and charindex('isStop§ 1', ElementoGrafico_DES , 0)>1 " & vbCrLf)
            stb.Append(xFiltroAggiuntivo_Punto_Stop & vbCrLf)



            stb.Append("    ) a inner join ( " & vbCrLf)
            stb.Append("     select  " & vbCrLf)
            stb.Append("          e.PivaSuperUser " & vbCrLf)
            stb.Append("        , e.Entita_cod " & vbCrLf)
            stb.Append("        , ElementoGrafico_Cod " & vbCrLf)
            stb.Append("        , poligono_GeoEntity " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("        , e.piva " & vbCrLf)
            stb.Append("        , e.sa_cod " & vbCrLf)
            stb.Append("        , e.Appezza " & vbCrLf)
            stb.Append("        , e.Id_Imp " & vbCrLf)
            stb.Append("        , c.cul_cod " & vbCrLf)
            stb.Append("        , c.veg_cod " & vbCrLf)

            stb.Append("    from  gis_entita e " & vbCrLf)
            stb.Append("        inner join gis_elementiGrafici g " & vbCrLf)
            stb.Append("            on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append("        inner join reg_impianti reg " & vbCrLf)
            stb.Append("            on reg.piva = e.piva  " & vbCrLf)
            stb.Append("            and reg.sa_cod = e.Sa_Cod " & vbCrLf)
            stb.Append("            and reg.APPEZZA = e.Appezza " & vbCrLf)
            stb.Append("            and reg.ID_REG = e.Id_Imp " & vbCrLf)
            stb.Append("        inner join Cultivar c " & vbCrLf)
            stb.Append("            on c.Cul_Cod = reg.CUL_COD " & vbCrLf)

            stb.Append("    where layerElementiGrafici_Cod in  (" & Impianti_layerElementiGrafici_Cod & ") " & vbCrLf)
            stb.Append("    ) b on a.Poligono_GeoEntity.STDistance(b.Poligono_GeoEntity) < " & Agro_SQL_SaveNum(DistanzaAttiva) & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" ) rProposte " & vbCrLf)
            stb.Append("  " & vbCrLf)

            If Leggi_Solo_Se_Non_Assegnati_Raccolta Then

                stb.Append(" where not exists ( " & vbCrLf)
                stb.Append("    select top 1 1  " & vbCrLf)
                stb.Append("    from Agenda A " & vbCrLf)
                stb.Append("        inner join  Mov_Destinazioni dEff " & vbCrLf)
                stb.Append("            on A.id_agenda = dEff.id_agenda " & vbCrLf)
                stb.Append("    where dEff.piva = rProposte.piva " & vbCrLf)
                stb.Append("    and dEff.sa_cod = rProposte.sa_cod " & vbCrLf)
                stb.Append("    and dEff.appezza = rProposte.appezza " & vbCrLf)
                stb.Append("    and dEff.id_destinazione = rProposte.id_imp " & vbCrLf)
                stb.Append("    and dEff.Tipo_Destinazione = 0  " & vbCrLf)
                stb.Append("    and a.lav_cod = 125 " & vbCrLf)
                stb.Append(" ) " & vbCrLf)
                stb.Append(" ")


            End If


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



    Public Function AssegnaImpiantoSuEntitaEsistente(
        ByVal Entita_cod_Destinazione As Integer,
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal Appezza As Integer,
        ByVal id_reg As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean


        Dim rval As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.PrecisionFarming.AssegnaImpiantoSuEntitaEsistente("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder


            stb.Append(" update gis_entita  " & vbCrLf)
            stb.Append(" set piva = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
            stb.Append(" , sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & vbCrLf)
            stb.Append(" , appezza = " & Agro_SQL_SaveNum(Appezza) & vbCrLf)
            stb.Append(" , Id_Imp =  " & id_reg & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where Entita_Cod = " & Agro_SQL_SaveNum(Entita_cod_Destinazione) & vbCrLf)
            stb.Append(" ")


            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return rval
    End Function


    Public Function CopiaIncollaOggettoGrafico(
        ByVal Entita_cod_Origine As Integer,
        ByVal id_agenda As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean


        Dim rval As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.PrecisionFarming.AssegnaImpiantoSuEntitaEsistente("
        Dim MessaggioErrore As String = ""

        Try

            '1. Gis_Entita 

            Dim NuovoEntita_cod As Integer


            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            NuovoEntita_cod = AgroSequenze.NuovoId_Tabella("gis_entita", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            'NuovoEntita_cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
            '    "GIS_Entita",
            '    objParametri
            ')

            Dim stb As New System.Text.StringBuilder

            stb.Append("            insert GIS_Entita(PivaSuperUser, Entita_Cod, TipoEntita_Cod, Piva, Sa_Cod, Appezza, Campo_Cod, Id_Imp, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, Programmazione_Entita_Cod, id_agenda, Ricetta_Operazione_cod, analisi_campione_cod, OLDGrafica_ID, programmazione_cod) " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("                 '" & objParametri.PivaSuperUser & "' as PivaSuperUser " & vbCrLf)
            stb.Append("    , " & Agro_SQL_SaveNum(NuovoEntita_cod) & " as Entita_Cod " & vbCrLf)
            stb.Append("    , 63 as TipoEntita_Cod " & vbCrLf) 'destinazione Agenda
            stb.Append("    , Piva " & vbCrLf)
            stb.Append("    , Sa_Cod " & vbCrLf)
            stb.Append("    , Appezza " & vbCrLf)
            stb.Append("    , Campo_Cod " & vbCrLf)
            stb.Append("    , Id_Imp " & vbCrLf)
            stb.Append("    , PROV " & vbCrLf)
            stb.Append("    , COM " & vbCrLf)
            stb.Append("    , SEZIONE " & vbCrLf)
            stb.Append("    , FOGLIO " & vbCrLf)
            stb.Append("    , NUMERO " & vbCrLf)
            stb.Append("    , SUBALTERNO " & vbCrLf)
            stb.Append("    , Programmazione_Entita_Cod " & vbCrLf)
            stb.Append("    , " & Agro_SQL_SaveNum(id_agenda) & " as ID_Agenda " & vbCrLf)
            stb.Append("    , Ricetta_Operazione_cod " & vbCrLf)
            stb.Append("    , analisi_campione_cod " & vbCrLf)
            stb.Append("    , OLDGrafica_ID    " & vbCrLf)
            stb.Append("    , programmazione_cod" & vbCrLf)
            stb.Append("    From GIS_Entita " & vbCrLf)
            stb.Append("    Where Entita_cod =  " & Agro_SQL_SaveNum(Entita_cod_Origine) & vbCrLf)

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Dim NuovoElementoGrafico As Integer
            NuovoElementoGrafico = AgroSequenze.NuovoId_Tabella("gis_elementigrafici", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            'NuovoElementoGrafico = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
            '    "GIS_ElementiGrafici",
            '    objParametri
            ')

            '2. Gis_ElementiGrafici
            stb.Length = 0
            stb.Clear()

            stb.Append(" insert gis_elementiGrafici(PivaSuperUser, ElementoGrafico_Cod, ElementoGrafico_Des, Entita_Cod, LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS) " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("                 '" & objParametri.PivaSuperUser & "' as PivaSuperUser  " & vbCrLf)
            stb.Append("    , " & Agro_SQL_SaveNum(NuovoElementoGrafico) & " as ElementoGrafico_Cod    " & vbCrLf)
            stb.Append("    , ElementoGrafico_Des " & vbCrLf)
            stb.Append("    , " & Agro_SQL_SaveNum(NuovoEntita_cod) & " as Entita_Cod " & vbCrLf)
            stb.Append("    , 63 as LayerElementiGrafici_Cod " & vbCrLf)
            stb.Append("    , Poligono_GeoEntity " & vbCrLf)
            stb.Append("    , Flag_GPS " & vbCrLf)
            stb.Append(" FROM GIS_elementiGrafici " & vbCrLf)
            stb.Append(" WHERE Entita_Cod =  " & Agro_SQL_SaveNum(Entita_cod_Origine) & vbCrLf)

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return rval
    End Function

    Private Sub LeggiGetQueryAB(
           ByVal PivaSuperUser As String,
           ByVal Entita_Cod As Int32,
           ByVal TipoEntita_Cod As Int32,
           ByVal PivaPadre As String,
           ByVal Piva As String,
           ByVal Sa_Cod As Int32,
           ByVal Appezza As Int32,
           ByVal Campo_Cod As Int32,
           ByVal ID_Imp As Int32,
           ByVal Prov As String,
           ByVal Com As String,
           ByVal Sezione As String,
           ByVal Foglio As Int32,
           ByVal Numero As Int32,
           ByVal Subalterno As String,
           ByVal ID_Agenda As Integer,
           ByVal programmazione_cod As Integer,
           ByVal Programmazione_Entita_cod As Integer,
           ByVal Ricetta_Operazione_Cod As Integer,
           ByVal ListaContattiMacchine As String,
           ByVal xFiltroAggiuntivo As String,
           ByVal xOrderBy As String,
           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
           ByVal Stb As System.Text.StringBuilder,
           ByVal isXml As Boolean,
        ByVal LeggiContattiMacchineOppureEntita As Integer,
        ByVal dataDA As DateTime,
        ByVal dataA As DateTime,
        ByVal layerElementiPF As Integer,
        ByVal Codice_Fiscale_Tecnico As String
       )




        Stb.Append(" select  distinct " & vbCrLf)


        If ListaContattiMacchine <> "" Then

            If ListaContattiMacchine = "-1" Then
                Stb.Append("      Gg.Entita_Cod " & vbCrLf)

            Else

                Stb.Append("      Gg.Entita_Cod " & vbCrLf)
                Stb.Append("    , Gg.elementografico_des " & vbCrLf)
                Stb.Append("    , ii.rag_soc " & vbCrLf)

                If layerElementiPF = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Or layerElementiPF = enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then

                    Stb.Append(" , '' as app_nome " & vbCrLf)
                    Stb.Append(" , ee.Entita_Des as Campo_Des " & vbCrLf)


                Else
                    Stb.Append("    , app.app_nome " & vbCrLf)
                    Stb.Append("    , cc.Campo_Des  " & vbCrLf)

                End If

                Stb.Append("    , sa.sa_nome " & vbCrLf)
                Stb.Append("    , cast(lav.LAV_DES  as nvarchar(1000))  + '_AG' + cast(oo.Ricetta_Operazione_Cod as nvarchar(1000))  as descrizioneCartellaOperazione " & vbCrLf)
                Stb.Append("    , oo.Ricetta_Operazione_Cod  " & vbCrLf)

                Stb.Append("  " & vbCrLf)
            End If

        End If

        Select Case LeggiContattiMacchineOppureEntita

            Case enum_pf_Export_tipo.Macchine
                Stb.Append("      mm.mac_Cod as codice " & vbCrLf)
                Stb.Append("    , mm.Mac_Des + ' ' + mm.modello + ' (' + Targa + ')' as Mac_DES " & vbCrLf)

            Case enum_pf_Export_tipo.Operatori
                Stb.Append("      risum.Cod_RisUm as codice " & vbCrLf)
                Stb.Append("    , cont.Nome " & vbCrLf)
                Stb.Append("    , cont.cognome " & vbCrLf)

            Case enum_pf_Export_tipo.Operazioni
                Stb.Append("      r.Ricetta_Cod as codice " & vbCrLf)
                Stb.Append("    , r.Ricetta_Des as Ricetta_Des " & vbCrLf)

        End Select

        Stb.Append(" , rd.piva " & vbCrLf)
        Stb.Append(" , rd.sa_cod " & vbCrLf)

        If layerElementiPF = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Or layerElementiPF = enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then
            Stb.Append(" , T.programmazione_cod " & vbCrLf)

            If ListaContattiMacchine <> "" Then
                Stb.Append(" , ee.programmazione_entita_cod " & vbCrLf)
                Stb.Append(" , ee.Entita_Des " & vbCrLf)
            End If

        Else

            Stb.Append(" , rd.appezza " & vbCrLf)
            Stb.Append(" , rd.id_reg " & vbCrLf)
        End If

        If ListaContattiMacchine <> "" Then
            Stb.AppendLine("   , gg.LayerElementiGrafici_Cod ")
            Stb.AppendLine("   , gg.ElementoGrafico_Des ")

            If layerElementiPF = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Or layerElementiPF = enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then
                Stb.AppendLine("  , ee.Programmazione_Entita_Cod ")
            End If

            Stb.AppendLine("  , R.Ricetta_Cod")
        End If


        Stb.Append(" from ricette r " & vbCrLf)
        Stb.Append("    inner join Ricette_Destinazioni rd  " & vbCrLf)
        Stb.Append("        on r.ricetta_cod = rd.ricetta_Cod " & vbCrLf)
        Stb.Append("        inner join Ricette_Dettagli rdd " & vbCrLf)
        Stb.Append("        on rdd.Ricetta_Cod = r.Ricetta_Cod " & vbCrLf)

        Stb.AppendLine("    inner Join Ricette_Operazioni o ")
        Stb.AppendLine("         On rdd.ricetta_cod = o.ricetta_cod  ")
        Stb.AppendLine("         And rdd.ricetta_operazione_Cod = o.ricetta_operazione_cod")


        Select Case Math.Abs(LeggiContattiMacchineOppureEntita)
            Case enum_pf_Export_tipo.Macchine
                Stb.Append("        and rdd.cau_mov in (8100) " & vbCrLf)
                Stb.Append("        and rdd.Elem_Cod = 1 " & vbCrLf)
                Stb.Append("    inner join Parco_Macchine mm " & vbCrLf)
                Stb.Append("        on mm.mac_cod = rdd.mat_cod " & vbCrLf)

            Case enum_pf_Export_tipo.Operatori
                Stb.Append("        and rdd.cau_mov in (6800, 6851) " & vbCrLf)
                Stb.Append("        and rdd.Elem_Cod = 0 " & vbCrLf)
                Stb.Append("    inner join Risorse_Umane risum " & vbCrLf)
                Stb.Append("        on risum.cod_risum = rdd.mat_cod " & vbCrLf)
                Stb.Append("    inner join contatti cont " & vbCrLf)
                Stb.Append("        on cont.cod_contatto = risum.cod_contatto " & vbCrLf)

        End Select

        If ListaContattiMacchine <> "" And ListaContattiMacchine <> "-1" Then
            Stb.Append("    inner join ricette_operazioni oo " & vbCrLf)
            Stb.Append("        on oo.Ricetta_Cod = r.Ricetta_Cod  " & vbCrLf)
            Stb.Append("                inner join operazioni lav " & vbCrLf)
            Stb.Append("        on oo.lav_cod = lav.lav_cod " & vbCrLf)


        End If


        If layerElementiPF = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Or layerElementiPF = enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then

            Stb.Append("inner join Programmazione_Entita ee " & vbCrLf)
            Stb.Append("   on ee.Programmazione_Entita_Cod = rd.Programmazione_Entita_Cod   " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("      inner join Programmazione_Testata T " & vbCrLf)
            Stb.Append("        on T.programmazione_cod = ee.Programmazione_Cod  " & vbCrLf)

        End If

        Stb.Append("     inner join GIS_Entita Entita   " & vbCrLf)
        If layerElementiPF = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Or layerElementiPF = enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then
            Stb.Append("    on Entita.Programmazione_Entita_Cod = ee.Programmazione_Entita_Cod " & vbCrLf)


        Else

            Stb.Append("    on Entita.piva = rd.Piva  " & vbCrLf)
            Stb.Append("    and Entita.sa_cod = rd.sa_cod  " & vbCrLf)
            Stb.Append("    and Entita.Appezza = rd.Appezza   " & vbCrLf)
            Stb.Append("    and Entita.Id_Imp  = rd.Id_Reg  " & vbCrLf)
        End If


        Stb.Append("    inner join GIS_ElementiGrafici Gg    " & vbCrLf)
        Stb.Append("        on Entita.Entita_Cod = Gg.Entita_Cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join Imprese ii " & vbCrLf)
        Stb.Append("        on ii.PIVA = r.Piva  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join centri_Aziendali sa  " & vbCrLf)
        Stb.Append("        on sa.piva = rd.Piva " & vbCrLf)
        Stb.Append("        and sa.sa_cod = rd.sa_cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    left join Appezzamento app " & vbCrLf)
        Stb.Append("        on app.PIVA = rd.Piva  " & vbCrLf)
        Stb.Append("        and app.SA_COD = rd.Sa_Cod  " & vbCrLf)
        Stb.Append("        and app.appezza = rd.appezza  " & vbCrLf)

        Stb.Append("  " & vbCrLf)
        Stb.Append("    left join Campi cc  " & vbCrLf)
        Stb.Append("        on cc.piva = rd.Piva " & vbCrLf)
        Stb.Append("        and cc.sa_cod = rd.sa_cod " & vbCrLf)
        Stb.Append("        and cc.Campo_Cod  = app.Campo_Cod  " & vbCrLf)


        Dim ge = New GIS_Entita_R
        ge.SettaParametriPrecedenti(DammiParametriCollezionati())
        ge.LeggiGetQueryCondizioniWhere(PivaSuperUser, Entita_Cod, TipoEntita_Cod, PivaPadre, Piva, Sa_Cod, Appezza, Campo_Cod, ID_Imp, Prov, Com, Sezione, Foglio, Numero, Subalterno, ID_Agenda, programmazione_cod, Programmazione_Entita_cod, Ricetta_Operazione_Cod, xFiltroAggiuntivo, objParametri, Stb, isXml, -1, "CF TEC", "", False)

        Me.SettaParametriPrecedenti(ge.DammiParametriCollezionati)
        If ListaContattiMacchine <> "" Then
            Select Case Math.Abs(LeggiContattiMacchineOppureEntita)
                Case 1
                    Stb.Append(" and mm.mac_cod in (" & Agro_SQL_Save_Clausola_IN(ListaContattiMacchine) & ") ")
                Case 2
                    Stb.Append(" and risum.cod_risum in (" & Agro_SQL_Save_Clausola_IN(ListaContattiMacchine) & ") ")
                Case 3
                    Stb.Append(" and oo.ricetta_cod in (" & Agro_SQL_Save_Clausola_IN(ListaContattiMacchine) & ") ")

            End Select

        End If

        'If ListaContattiMacchine <> "-1" Then
        If dataDA <> #1/1/1900# Then
            Stb.Append("  and o.Validita_Inizio <= " & Agro_SQL_SaveDate(dataA) & " " & vbCrLf)
            Stb.Append("  and o.Validita_Inizio >= " & Agro_SQL_SaveDate(dataDA) & " " & vbCrLf)
        End If
        'End If

        If ListaContattiMacchine <> "" Then
            If layerElementiPF = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Or layerElementiPF = enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then
                Stb.Append(" order by Piva, sa_cod, R.Ricetta_cod, ee.Programmazione_Entita_Cod, gg.LayerElementiGrafici_Cod desc, gg.ElementoGrafico_Des   ")
            Else
                Stb.Append(" order by Piva, sa_cod, R.Ricetta_cod, appezza, id_Reg, gg.LayerElementiGrafici_Cod desc, gg.ElementoGrafico_Des  ")
            End If

        End If


    End Sub




End Class
