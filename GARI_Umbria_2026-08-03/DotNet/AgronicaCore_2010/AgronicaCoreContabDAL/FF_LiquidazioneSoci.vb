Imports System.Data.Entity
Imports System.Data.SqlClient
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class FF_LiquidazioneSoci_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Operazione_Agenda(ByVal idAgenda As Integer,
                                            ByVal filtro As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim aggiungiTotali As Boolean = True
        If xFiltroAggiuntivo = "NO_TOTALI" Then
            aggiungiTotali = False
            xFiltroAggiuntivo = ""
        End If

        Try

            stb.Length = 0

            stb.Append("SELECT " & vbCrLf)
            stb.Append("    --Testata  " & vbCrLf)

            stb.Append("      a.des_lib  " & vbCrLf)
            stb.Append("    , mTes.Data_Movimento " & vbCrLf)
            stb.Append("    , mTes.Data_Registrazione " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero_Sin " & vbCrLf)
            stb.Append("    , mTes.Mov_Desc " & vbCrLf)

            stb.Append("  " & vbCrLf)

            stb.Append("    --Dettagli " & vbCrLf)
            stb.Append("    , cast(MTes.cod_risum as varchar(100))  + '-' + d.lotto as Codice " & vbCrLf)
            stb.Append("    , c.Rag_soc as Fornitore_Rag_Soc " & vbCrLf)
            stb.Append("    , D.Lotto as Fornitore_Lotto " & vbCrLf)

            stb.Append("    , '' as Cliente_Rag_Soc    " & vbCrLf)
            stb.Append("    , coalesce(ss.WAnagraficaStati_Des, '') as DescrizioneStato " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    , DA_Ricavi.Prezzo_Unitario as A_Ricavi " & vbCrLf)
            stb.Append("    , DB_Costi.Prezzo_Unitario  as B_Costi " & vbCrLf)
            stb.Append("    , DB_1_Confezionamento.Prezzo_Unitario  as B_1_Confezionamento " & vbCrLf)
            stb.Append("    , DB_2_Ore_Manodopera.Prezzo_Unitario  as B_2_Ore_Manodopera " & vbCrLf)
            stb.Append("    , DB_3_Sfrido.Prezzo_Unitario  as B_3_Sfrido " & vbCrLf)
            stb.Append("    , DB_4_Inefficienze.Prezzo_Unitario  as B_4_Inefficienze " & vbCrLf)
            stb.Append("    , DB_5_SpeseTrasporto.Prezzo_Unitario  as B_5_SpeseTrasporto " & vbCrLf)
            stb.Append("    , D.Prezzo_unitario as AB_Totale_Liquidare")
            stb.Append("    , 0 as idleCalcola")

            stb.Append(" from Agenda A " & vbCrLf)
            stb.Append("    inner join Movimenti MTes " & vbCrLf)
            stb.Append("        on MTes.Id_Agenda= A.Id_Agenda " & vbCrLf)
            stb.Append("        AND MTes.Piva= A.Piva " & vbCrLf)
            stb.Append("        AND MTes.sa_cod= A.sa_cod " & vbCrLf)
            stb.Append("        and MTes.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join Movimenti MDet " & vbCrLf)
            stb.Append("        on MDet.Id_Agenda= A.Id_Agenda " & vbCrLf)
            stb.Append("        AND MDet.Piva= A.Piva " & vbCrLf)
            stb.Append("        AND MDet.sa_cod= A.sa_cod " & vbCrLf)
            stb.Append("        and MDet.Cau_Mov = '" & CAU_CARICO & "' " & vbCrLf)

            stb.Append("    inner join Movimenti MDetS " & vbCrLf)
            stb.Append("        on MDetS.Id_Agenda= A.Id_Agenda " & vbCrLf)
            stb.Append("        AND MDetS.Piva= A.Piva " & vbCrLf)
            stb.Append("        AND MDetS.sa_cod= A.sa_cod " & vbCrLf)
            stb.Append("        and MDetS.Cau_Mov = '" & CAU_REGISTRAZIONI_TERZIARIA & "' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join Movimenti_dettagli D " & vbCrLf)
            stb.Append("        on MDet.Id_Agenda = D.id_agenda " & vbCrLf)
            stb.Append("        and MDet.Piva = D.Piva " & vbCrLf)
            stb.Append("        and MDet.sa_cod = D.sa_cod " & vbCrLf)
            stb.Append("        and MDet.id_mov = D.id_mov " & vbCrLf)
            stb.Append("        and D.Extra_Int = 0 " & vbCrLf)

            stb.Append("    --fornitore iniziale " & vbCrLf)
            stb.Append("    inner join Risorse_Umane risumForn " & vbCrLf)
            stb.Append("        on risumForn.cod_risum = MTes.cod_Risum  " & vbCrLf)

            stb.Append("    inner join contatti c " & vbCrLf)
            stb.Append("        on c.Cod_Contatto = risumForn.Cod_Contatto  " & vbCrLf)
            stb.Append("        and c.Piva = risumForn.Piva  " & vbCrLf)
            stb.Append("    --fine fornitore iniziale " & vbCrLf)

            Leggi_Operazione_AgendaDettaglioDAti("A_Ricavi", stb)
            Leggi_Operazione_AgendaDettaglioDAti("B_Costi", stb)
            Leggi_Operazione_AgendaDettaglioDAti("B_1_Confezionamento", stb)
            Leggi_Operazione_AgendaDettaglioDAti("B_2_Ore_Manodopera", stb)
            Leggi_Operazione_AgendaDettaglioDAti("B_3_Sfrido", stb)
            Leggi_Operazione_AgendaDettaglioDAti("B_4_Inefficienze", stb)
            Leggi_Operazione_AgendaDettaglioDAti("B_5_SpeseTrasporto", stb)

            Leggi_stati_From(stb)


            stb.Append("  " & vbCrLf)
            stb.Append(" where A.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & vbCrLf)



            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            If aggiungiTotali Then
                CalcoloSelezione_ParteFissaPerTotali(True, stb)
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function




    Public Function LeggiBlocchiLiquid_Mov_CampionamentoConferito(ByVal piva_superuser As String,
                                                                  ByVal piva As String,
                                                                  ByVal id_agenda As Integer,
                                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.Liquid_Mov_CampionamentoConferito"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT Distinct anag_liq.descrizione FROM Liquid_Mov_CampionamentoConferito liq with(nolock) ")
            stbSql.AppendLine(" join AnagAccontiLiquidazioni_CampionamentoConferito anag_liq with(nolock) on anag_liq.id_anagrafica = liq.Id_acconto_liquidazione ")
            stbSql.AppendLine(" WHERE anag_liq.definitivo = 1 ")
            stbSql.AppendLine(" And liq.Piva_SuperUser = '" & piva_superuser & "' ")
            stbSql.AppendLine(" And liq.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stbSql.AppendLine(" And liq.Id_Mov_Det In (Select Id_Mov_Det From Movimenti_Dettagli with(nolock)   ")
            stbSql.AppendLine("                    Where Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(piva) & "' AND ")
            stbSql.AppendLine("                    Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & ")")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    Public Function LeggiBlocchiLiquid_Mov_FattVariaz_CampionamentoConferito(ByVal piva_superuser As String,
                                                                             ByVal piva As String,
                                                                             ByVal id_agenda As Integer,
                                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                             Optional ByVal id_acconto_liquidazione As Integer? = 0,
                                                                             Optional ByVal xSelezioneVariabile As enumSelezioneVariabile? = enumSelezioneVariabile.Selezione_TabellaCompleta) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.LeggiBlocchiLiquid_Mov_FattVariaz_CampionamentoConferito"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------------------------------------------
                    stbSql.Length = 0

                    stbSql.AppendLine(" SELECT Distinct Liquid_Mov_FattVariaz_CampionamentoConferito.* FROM Liquid_Mov_FattVariaz_CampionamentoConferito ")
                    stbSql.AppendLine(" WHERE Piva_SuperUser = '" & piva_superuser & "' ")
                    stbSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    stbSql.AppendLine(" And Id_Mov_Det In (Select Id_Mov_Det From Agenda, Movimenti_Dettagli  ")
                    stbSql.AppendLine("                    Where Agenda.Piva = Movimenti_Dettagli.Piva AND  ")
                    stbSql.AppendLine("                    Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND  ")
                    stbSql.AppendLine("                    Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & ")")
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '------------------------------------------------------------------
                    stbSql.Length = 0

                    stbSql.AppendLine(" SELECT Distinct id_fattore_variazione, fatt_var_descr  ")
                    stbSql.AppendLine(" FROM Liquid_Mov_FattVariaz_CampionamentoConferito ")
                    stbSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(piva_superuser) & "' ")
                    stbSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")

                    If id_acconto_liquidazione <> 0 Then
                        stbSql.AppendLine(" And id_acconto_liquidazione = " & Agro_SQL_SaveNum(id_acconto_liquidazione) & " ")
                    End If
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Liquid_Mov_PerCalibro_CampionamentoConferito(ByVal piva_superuser As String,
                                                                 ByVal piva As String,
                                                                 ByVal id_agenda As Integer,
                                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.Liquid_Mov_PerCalibro_CampionamentoConferito"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT Distinct Liquid_Mov_PerCalibro_CampionamentoConferito.* FROM Liquid_Mov_PerCalibro_CampionamentoConferito ")
            stbSql.AppendLine(" WHERE Piva_SuperUser = '" & piva_superuser & "' ")
            stbSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stbSql.AppendLine(" And Id_Mov_Det In (Select Id_Mov_Det From Agenda, Movimenti_Dettagli  ")
            stbSql.AppendLine("                    Where Agenda.Piva = Movimenti_Dettagli.Piva AND  ")
            stbSql.AppendLine("                    Agenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND  ")
            stbSql.AppendLine("                    Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda) & ")")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Verifica_Righe_Liquidazione_No_MovDet(ByVal id_acconto_liquidazione As Integer,
                                                                  ByVal piva As String,
                                                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.Verifica_Righe_Liquidazione_No_MovDet"
        Dim pivaSuperUser = objParametri_Server.PivaSuperUser
        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT Distinct anag_liq.descrizione FROM AnagAccontiLiquidazioni_CampionamentoConferito anag_liq with(nolock) ")
            stbSql.AppendLine(" join Liquid_Mov_CampionamentoConferito liq with(nolock) on anag_liq.id_anagrafica = liq.Id_acconto_liquidazione ")
            stbSql.AppendLine(" WHERE anag_liq.id_anagrafica = " & Agro_SQL_SaveNum(id_acconto_liquidazione))
            stbSql.AppendLine(" And anag_liq.Piva_SuperUser = '" & Agro_SQL_SaveText(piva) & "' ")
            stbSql.AppendLine(" And anag_liq.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stbSql.AppendLine(" And liq.Id_Mov_Det Not In (Select Id_Mov_Det From Movimenti_Dettagli  ")
            stbSql.AppendLine("                    Where liq.Piva = Movimenti_Dettagli.Piva )  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function




    Private Sub Leggi_Operazione_AgendaDettaglioDAti(ByVal dato As String, ByVal stb As System.Text.StringBuilder)
        stb.Append("    inner join Movimenti_dettagli D" & dato & " " & vbCrLf)
        stb.Append("        on MDetS.Id_Agenda = D" & dato & ".id_agenda " & vbCrLf)
        stb.Append("        and MDetS.Piva = D" & dato & ".Piva " & vbCrLf)
        stb.Append("        and MDetS.sa_cod = D" & dato & ".sa_cod " & vbCrLf)
        stb.Append("        and MDetS.id_mov = D" & dato & ".id_mov " & vbCrLf)
        stb.Append("        and D" & dato & ".extra_str = '" & Agro_SQL_SaveText(dato) & "'" & vbCrLf)
    End Sub

    Private Sub CalcoloSelezione_ParteFissaPerTotali(ByVal aggiungiTestata As Boolean, ByVal stb As System.Text.StringBuilder)
        stb.Append(" UNION ALL ")
        stb.Append(" Select  " & vbCrLf)

        If aggiungiTestata Then
            stb.Append("        '' as des_lib " & vbCrLf)
            stb.Append("      , '01/01/1900' as Data_Movimento " & vbCrLf)
            stb.Append("      , '01/01/1900' as Data_Registrazione " & vbCrLf)
            stb.Append("      , 1 as Doc_Numero " & vbCrLf)
            stb.Append("      , 'abc' as Doc_Numero_Sin " & vbCrLf)
            stb.Append("      , '' as Mov_Desc, " & vbCrLf)
        End If

        stb.Append("      '' as Codice " & vbCrLf)
        stb.Append("    , '' as Fornitore_Rag_Soc " & vbCrLf)
        stb.Append("    , '' as Fornitore_Lotto " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    ,'' as Cliente_Rag_Soc    " & vbCrLf)
        stb.Append("    , 'Totali complessivi: ' as DescrizioneStato " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    , 0 as A_Ricavi " & vbCrLf)
        stb.Append("    , 0 as B_Costi " & vbCrLf)
        stb.Append("    , 0 as B_1_Confezionamento " & vbCrLf)
        stb.Append("    , 0 as B_2_Ore_Manodopera " & vbCrLf)
        stb.Append("    , 0 as B_3_Sfrido " & vbCrLf)
        stb.Append("    , 0 as B_4_Inefficienze " & vbCrLf)
        stb.Append("    , 0 as B_5_SpeseTrasporto " & vbCrLf)
        stb.Append("    , 0 as AB_Totale_Liquidare")
        stb.Append("    , 0 as idleCalcola")
        stb.Append("    , 0 as TotaleQuantita")
        stb.Append("    , 0 as TotalePeso")
        stb.Append("    , 0 as CodiceIva")
        stb.Append("    , '' as DescrizioneIva")
        stb.Append("    , '' as Prodotto")
        stb.Append("    , '' as udmQtacodice  " & vbCrLf)
        stb.Append("    , '' as udmQtadescrizione  " & vbCrLf)
        stb.Append("    , '' as udmPesocodice  " & vbCrLf)
        stb.Append("    , '' as udmPesodescrizione  " & vbCrLf)


        stb.Append("    , 0 as elem_cod " & vbCrLf)
        stb.Append("    , 0 as pro_cod " & vbCrLf)
        stb.Append("    , 0 as mat_cod " & vbCrLf)

    End Sub

    Private Shared Sub CalcoloSelezione_OttieniImporti_leggiParametri(ByVal siglaParametro As String, ByVal stb As System.Text.StringBuilder)
        stb.Append(" left join FF_Parametri_Liquidazione cfg" & siglaParametro & " " & vbCrLf)
        stb.Append(" on cfg" & siglaParametro & ".piva = a.piva " & vbCrLf)
        stb.Append(" and cfg" & siglaParametro & ".Sigla = '" & siglaParametro & "'" & vbCrLf)
    End Sub

    Public Function CalcoloSelezione_OttieniImporti(ByVal filtro As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.Append(" Select " & vbCrLf)
            stb.Append("      cast(risumForn.cod_risum as varchar(100)) + '-' + d.lotto as Codice " & vbCrLf)
            stb.Append("    , c.Rag_Soc as Fornitore_Rag_Soc " & vbCrLf)
            stb.Append("    , d.Lotto as Fornitore_Lotto " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    , cCli.rag_soc as Cliente_Rag_Soc    " & vbCrLf)
            stb.Append("    , coalesce(ss.WAnagraficaStati_Des, '') as DescrizioneStato " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    , sum(d.Qta) * sum(d.Prezzo_Unitario) as A_Ricavi " & vbCrLf)
            stb.Append("    , 2 as B_Costi " & vbCrLf)
            stb.Append("    , 3 as B_1_Confezionamento " & vbCrLf)
            stb.Append("    , 4 as B_2_Ore_Manodopera " & vbCrLf)
            stb.Append("    , (sum(d.Qta) * sum(d.Prezzo_Unitario)) * coalesce(max(cfgB_3_Sfrido.extra_float), 1) as B_3_Sfrido " & vbCrLf)
            stb.Append("    , (sum(d.Qta) * sum(d.Prezzo_Unitario)) * coalesce(max(cfgB_4_Inefficienze.extra_float), 1) as B_4_Inefficienze  " & vbCrLf)
            stb.Append("    , 7 as B_5_SpeseTrasporto " & vbCrLf)
            stb.Append("    , 8 as AB_Totale_Liquidare" & vbCrLf)
            stb.Append("    , 0 as idleCalcola" & vbCrLf)
            stb.Append("    , sum(d.Qta) TotaleQuantita" & vbCrLf)
            stb.Append("    , sum(d.qta_extra) TotalePeso" & vbCrLf)
            stb.Append("    , cod_iva.codice as CodiceIva" & vbCrLf)
            stb.Append("    , cod_iva.Descrizione as DescrizioneIva" & vbCrLf)
            stb.Append("    , d.mov_det_des as Prodotto" & vbCrLf)

            stb.Append("    , udmQta.udm_cod as udmQtacodice" & vbCrLf)
            stb.Append("    , udmQta.udm_sim as udmQtaDescrizione" & vbCrLf)

            stb.Append("    , coalesce(udmPeso.udm_cod , 0) as udmPesocodice" & vbCrLf)
            stb.Append("    , udmPeso.udm_sim as udmPesoDescrizione" & vbCrLf)

            stb.Append("    , d.elem_cod " & vbCrLf)
            stb.Append("    , d.pro_cod " & vbCrLf)
            stb.Append("    , d.mat_cod " & vbCrLf)

            LottoComposizione_From(stb)
            Leggi_stati_From(stb)

            CalcoloSelezione_OttieniImporti_leggiParametri("B_3_Sfrido", stb)
            CalcoloSelezione_OttieniImporti_leggiParametri("B_4_Inefficienze", stb)

            stb.Append(vbCrLf & " WHERE 1=1 " & vbCrLf)
            LottoComposizione_Where(filtro, stb)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            LottoComposizione_GroupBy(stb)

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            CalcoloSelezione_ParteFissaPerTotali(False, stb)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Shared Sub LottoComposizione_GroupBy(ByVal stb As System.Text.StringBuilder)

        stb.Append("    group by " & vbCrLf)
        stb.Append("        cast(risumForn.cod_risum as varchar(100)) + '-' + d.lotto  " & vbCrLf)
        stb.Append("     , c.Rag_Soc " & vbCrLf)
        stb.Append("     , d.Lotto   " & vbCrLf)
        stb.Append("     , cCli.rag_soc  " & vbCrLf)
        stb.Append("     , coalesce(ss.WAnagraficaStati_Des, '') " & vbCrLf)
        stb.Append("     , cod_iva.codice  " & vbCrLf)
        stb.Append("     , cod_iva.descrizione  " & vbCrLf)
        stb.Append("     , d.mov_det_des  " & vbCrLf)
        stb.Append("     , udmQta.udm_cod  " & vbCrLf)
        stb.Append("     , udmPeso.udm_cod  " & vbCrLf)
        stb.Append("     , udmQta.udm_sim  " & vbCrLf)
        stb.Append("     , udmPeso.udm_sim  " & vbCrLf)

        stb.Append("    , d.elem_cod " & vbCrLf)
        stb.Append("    , d.pro_cod " & vbCrLf)
        stb.Append("    , d.mat_cod " & vbCrLf)

    End Sub

    Private Sub LottoComposizione_Where(ByVal filtro As String, ByVal stb As System.Text.StringBuilder)

        stb.Append(" and a.lav_cod = 1001 and cast(risumForn.cod_risum as varchar(100)) + '-' + d.lotto in (" & Agro_SQL_Save_Clausola_IN(GetLottoFiltriSql(filtro)) & ")")

    End Sub

    Private Shared Function GetLottoFiltriSql(ByVal filtro As String) As String

        Return "'" & filtro.Replace(",", "','") & "'"

    End Function

    Private Shared Sub LottoComposizione_From(ByVal stb As System.Text.StringBuilder)
        stb.Append(" from Movimenti_dettagli d " & vbCrLf)
        stb.Append("    inner join Agenda a  " & vbCrLf)
        stb.Append("        on d.Id_Agenda = a.Id_Agenda  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    INNER JOIN Movimenti m   " & vbCrLf)
        stb.Append("         ON d.PIVA = m.PIVA   " & vbCrLf)
        stb.Append("         and d.Id_Mov = m.Id_Mov          " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    inner join Materie_Prime_Campionature cc " & vbCrLf)
        stb.Append("        on cc.Progressivo = d.Cal_Cod  " & vbCrLf)
        stb.Append("        and cc.Tipo = 'OFornitore' " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    --cliente " & vbCrLf)
        stb.Append("    inner join Risorse_Umane risumCli " & vbCrLf)
        stb.Append("        on m.Cod_RisUm = risumCli.Cod_RisUm  " & vbCrLf)
        stb.Append("        and m.PIVA = risumCli.Piva  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    inner join contatti cCli " & vbCrLf)
        stb.Append("        on cCli.Cod_Contatto = risumCli.Cod_Contatto  " & vbCrLf)
        stb.Append("        and cCli.Piva = risumCli.Piva  " & vbCrLf)
        stb.Append("    --fine cliente " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    --fornitore iniziale " & vbCrLf)
        stb.Append("    inner join Risorse_Umane risumForn " & vbCrLf)
        stb.Append("        on risumForn.cod_risum = cc.Tipo_Cod  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    inner join contatti c " & vbCrLf)
        stb.Append("        on c.Cod_Contatto = risumForn.Cod_Contatto  " & vbCrLf)
        stb.Append("        and c.Piva = risumForn.Piva  " & vbCrLf)
        stb.Append("    --fine fornitore iniziale " & vbCrLf)
        stb.Append("  " & vbCrLf)

        stb.Append("    inner join IVA_Aliquote cod_iva " & vbCrLf)
        stb.Append("        on d.cod_iva = cod_iva.codice  " & vbCrLf)

        stb.Append("    inner join UnitaMisura udmQta " & vbCrLf)
        stb.Append("        on UdmQta.udm_cod = D.udm_cod  " & vbCrLf)

        stb.Append("    left join UnitaMisura udmPeso " & vbCrLf)
        stb.Append("        on udmPeso.udm_cod = D.udm_cod_extra  " & vbCrLf)

    End Sub

    '##############################################################################################
    Public Function CalcoloXLotto(ByVal piva As String,
                                  ByVal lotto As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.Append("Select distinct " & vbCrLf)
            stb.Append("      cast(risumForn.cod_risum as varchar(100)) + '-' + d.lotto as Codice " & vbCrLf)
            stb.Append("    , c.Rag_Soc as Fornitore_Rag_Soc " & vbCrLf)
            stb.Append("    , d.Lotto as Fornitore_Lotto " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    , cCli.rag_soc as Cliente_Rag_Soc    " & vbCrLf)
            stb.Append("    , coalesce(ss.WAnagraficaStati_Des, '') as DescrizioneStato " & vbCrLf)
            stb.Append("  " & vbCrLf)

            LottoComposizione_From(stb)
            Leggi_stati_From(stb)

            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where a.lav_cod in ( " & vbCrLf)
            stb.Append("     1001, 1000   " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" ")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Shared Sub Leggi_stati_From(ByVal stb As System.Text.StringBuilder)
        stb.Append("    left join lotto_liquidazione ll " & vbCrLf)
        stb.Append("        on d.lotto = ll.lotto_cod " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    left join wanagraficaStati ss " & vbCrLf)
        stb.Append("        on ss.WAnagraficaStati_Cod  = ll.stato_cod   " & vbCrLf)
    End Sub

    '##############################################################################################
    Public Function Leggi_ListaLiquidazione(ByVal codContattoSocio As String,
                                            ByVal statoLiquidazioneCod As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.Append("Select distinct " & vbCrLf)
            stb.Append("      cast(risumForn.cod_risum as varchar(100)) + '-' + d.lotto as Codice " & vbCrLf)
            stb.Append("    , c.Rag_Soc as Fornitore_Rag_Soc " & vbCrLf)
            stb.Append("    , d.Lotto as Fornitore_Lotto " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    , cCli.rag_soc as Cliente_Rag_Soc    " & vbCrLf)
            stb.Append("    , coalesce(ss.WAnagraficaStati_Des, '') as DescrizioneStato " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from Movimenti_dettagli d " & vbCrLf)
            stb.Append("    inner join Agenda a  " & vbCrLf)
            stb.Append("        on d.Id_Agenda = a.Id_Agenda  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INNER JOIN Movimenti m   " & vbCrLf)
            stb.Append("         ON d.PIVA = m.PIVA   " & vbCrLf)
            stb.Append("         and d.Id_Mov = m.Id_Mov          " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join Materie_Prime_Campionature cc " & vbCrLf)
            stb.Append("        on cc.Progressivo = d.Cal_Cod  " & vbCrLf)
            stb.Append("        and cc.Tipo = 'OFornitore' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    --cliente " & vbCrLf)
            stb.Append("    inner join Risorse_Umane risumCli " & vbCrLf)
            stb.Append("        on m.Cod_RisUm = risumCli.Cod_RisUm  " & vbCrLf)
            stb.Append("        and m.PIVA = risumCli.Piva  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join contatti cCli " & vbCrLf)
            stb.Append("        on cCli.Cod_Contatto = risumCli.Cod_Contatto  " & vbCrLf)
            stb.Append("        and cCli.Piva = risumCli.Piva  " & vbCrLf)
            stb.Append("    --fine cliente " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    --fornitore iniziale " & vbCrLf)
            stb.Append("    inner join Risorse_Umane risumForn " & vbCrLf)
            stb.Append("        on risumForn.cod_risum = cc.Tipo_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join contatti c " & vbCrLf)
            stb.Append("        on c.Cod_Contatto = risumForn.Cod_Contatto  " & vbCrLf)
            stb.Append("        and c.Piva = risumForn.Piva  " & vbCrLf)
            stb.Append("    --fine fornitore iniziale " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join lotto_liquidazione ll " & vbCrLf)
            stb.Append("        on d.lotto = ll.lotto_cod " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join wanagraficaStati ss " & vbCrLf)
            stb.Append("        on ss.WAnagraficaStati_Cod  = ll.stato_cod   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where a.lav_cod in ( " & vbCrLf)
            stb.Append("     1001, 1000   " & vbCrLf)
            stb.Append(" ) " & vbCrLf)

            'stb.Append(" AND c.cod_contatto = '" & Agro_SQL_SaveText(cod_contatto_socio) & "'")

            If statoLiquidazioneCod <> 0 Then
                stb.Append(" AND coalesce(ss.WAnagraficaStati_Cod, 11) = '" & Agro_SQL_SaveText(statoLiquidazioneCod) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function ElencoTrattenute(ByVal piva As String,
                                     ByVal id_acconto_liquidazione As Integer,
                                     ByVal cod_RisUm As Integer,
                                     ByRef dtTrattenute As DataTable,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As String

        Dim risposta As String = ""

        Dim pivaSuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.ElencoTrattenute()"

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            Dim elenco = From trattenute In giasContext.Liquid_Mov_Trattenute_CampionamentoConferito
                         Join risum In giasContext.Risorse_Umane
                             On trattenute.Cod_RisUm Equals risum.Cod_RisUm
                         Join contatti In giasContext.Contatti
                             On risum.Cod_Contatto Equals contatti.Cod_Contatto
                         Join liq In giasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                             On trattenute.Id_acconto_liquidazione Equals liq.id_anagrafica
                         Join iva_aliq In giasContext.IVA_Aliquote
                             On trattenute.Cod_Iva Equals iva_aliq.Codice
                         Where trattenute.PIVA.Equals(piva) AndAlso
                             trattenute.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                             (id_acconto_liquidazione = 0 OrElse trattenute.Id_acconto_liquidazione = id_acconto_liquidazione) AndAlso
                             (cod_RisUm = 0 OrElse trattenute.Cod_RisUm = cod_RisUm)
                         Order By trattenute.Id_liquidazione_trattenute
                         Select New With {
                             .Id_liquidazione_trattenute = trattenute.Id_liquidazione_trattenute,
                             .id_liquidazione = trattenute.Id_acconto_liquidazione,
                             .des_liquidazione = liq.descrizione,
                             .Cod_RisUm = trattenute.Cod_RisUm,
                             .Rag_Soc = contatti.Rag_Soc,
                             .Doc_Numero_Sin = trattenute.Doc_Numero_Sin,
                             .Doc_Numero = trattenute.Doc_Numero,
                             .Doc_Numero_Des = trattenute.Doc_Numero_Des,
                             .Data_Movimento = trattenute.Data_Movimento,
                             .Riferimento = trattenute.Riferimento,
                             .Imponibile = trattenute.Imponibile,
                             .Cod_IVA = trattenute.Cod_Iva,
                             .Sigla_IVA = iva_aliq.Sigla,
                             .IVA = trattenute.Iva,
                             .Detrarre_Da_Fattura = trattenute.Detrarre_Da_Fattura
                             }

            Dim myList = elenco.ToList()

            Dim ut As New Gias_EF_Utility
            dtTrattenute = ut.ObjectQueryToDataTable(myList)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function ElencoPercAccGrpFatt(ByVal piva As String,
                                         ByVal id_acconto_liquidazione As Integer,
                                         ByVal id_gruppo_fatturazione As Integer,
                                         ByRef dtElencoPercAccGrpFatt As DataTable,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Dim risposta As String = ""

        Dim pivaSuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.ElencoPercAccGrpFatt()"

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            Dim elenco = From anag In giasContext.AnagPercentAccontiPerGrpFatt_CampionamentoConferito
                         Join liq In giasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                             On anag.Id_acconto_liquidazione Equals liq.id_anagrafica
                         Join otp In giasContext.OTabelle_Parametri
                             On anag.Id_gruppo_fatturazione Equals otp.Tabella_Par_Cod
                         Where anag.PIVA.Equals(piva) AndAlso
                             anag.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                             (id_acconto_liquidazione = 0 OrElse anag.Id_acconto_liquidazione = id_acconto_liquidazione) AndAlso
                             (id_gruppo_fatturazione = 0 OrElse anag.Id_gruppo_fatturazione = id_gruppo_fatturazione)
                         Select New With {
                             .key_acconto_liquidazione = anag.Id_acconto_liquidazione,
                             .key_gruppo_fatturazione = anag.Id_gruppo_fatturazione,
                             .Tabella_Par_Cod = otp.Tabella_Par_Cod,
                             .Descrizione = otp.Descrizione,
                             .id_liquidazione = anag.Id_acconto_liquidazione,
                             .des_liquidazione = liq.descrizione,
                             .perc_valore_acconto = anag.perc_valore_acconto
                             }

            Dim myList = elenco.ToList()

            Dim ut As New Gias_EF_Utility
            dtElencoPercAccGrpFatt = ut.ObjectQueryToDataTable(myList)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta
    End Function

    Public Function Leggi_DatiLiquidazione(ByVal piva As String,
                                           ByVal idAccontoLiquidazione As Integer,
                                           ByVal fattAutofatt As String,
                                           ByVal codRisUmFiltro As Integer(),
                                           ByRef dtElencoLiquidazioni As DataTable,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef listaMovDettagliCollegati As List(Of Movimenti_dettagli)
                                           ) As List(Of Liquid_Mov_Dati_Generali_CampionamentoConferito)

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.Leggi_DatiLiquidazione()"
        Dim messaggioErrore As String = ""

        Dim list As List(Of Liquid_Mov_Dati_Generali_CampionamentoConferito)
        Dim risposta As String = ""

        Try
            Dim pivaSuperUser = objParametri.PivaSuperUser

            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                giasContext.Configuration.LazyLoadingEnabled = False

                Dim elenco = From datGen In giasContext.Liquid_Mov_Dati_Generali_CampionamentoConferito _
                                                       .Include("Liquid_Mov_CampionamentoConferito")
                             Where datGen.Piva_SuperUser = pivaSuperUser AndAlso
                                  datGen.PIVA = piva AndAlso
                                  datGen.Id_acconto_liquidazione = CInt(idAccontoLiquidazione)
                             Select datGen

                If fattAutofatt <> "" Then
                    elenco = elenco.Where(Function(x) x.FattAutofatt.Equals(fattAutofatt))
                End If

                If Not IsNothing(codRisUmFiltro) AndAlso codRisUmFiltro.Length > 0 Then
                    elenco = elenco.Where(Function(x) codRisUmFiltro.Contains(x.Cod_RisUm))
                End If

                list = elenco.ToList()

                'Per capire se utilizzare o meno i dati di movimenti_dettagli, devo ricavare il tipoLiquidazione
                Dim tipoLiquidazione As String = (From a In giasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                                                  Where a.Piva_SuperUser = pivaSuperUser AndAlso
                                                     a.PIVA = piva AndAlso
                                                     a.id_anagrafica = CInt(idAccontoLiquidazione)
                                                  Select a.tipo_anagrafica).FirstOrDefault()

                If tipoLiquidazione = "L" Then

                    Dim listaFigliMovCampionamento = list.SelectMany(Function(x) x.Liquid_Mov_CampionamentoConferito).ToList()

                    Dim listaIdMovDet As List(Of Integer) = listaFigliMovCampionamento.Select(Function(x) x.Id_Mov_Det).ToList()

                    listaMovDettagliCollegati = (From m In giasContext.Movimenti_dettagli
                                                 Where listaIdMovDet.Contains(m.Id_Mov_Det)
                                                 Select m).ToList()

                End If

                'Dim ut As New Gias_EF_Utility
                'dtElencoLiquidazioni = ut.ObjectQueryToDataTable(list)

                'Dim serializerSettings As New JsonSerializerSettings()
                'serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                'risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return list   'risposta

    End Function

    Public Function Leggi_TrattenuteLiquidazione(ByVal piva As String,
                                                 ByVal idAccontoLiquidazione As Integer,
                                                 ByVal detrarreDaFattura As Integer?,
                                                 ByVal codRisUmFiltro As Integer(),
                                                 ByRef dtElencoTrattenute As DataTable,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As List(Of Liquid_Mov_Trattenute_CampionamentoConferito)

        Dim messaggioErrore As String = ""
        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.Leggi_TrattenuteLiquidazione()"

        Dim list As List(Of Liquid_Mov_Trattenute_CampionamentoConferito)
        Dim risposta As String = ""

        Try
            Dim pivaSuperUser = objParametri.PivaSuperUser

            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                Dim elenco = From tratt In giasContext.Liquid_Mov_Trattenute_CampionamentoConferito
                             Where tratt.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                                   tratt.PIVA.Equals(piva) AndAlso
                                   tratt.Id_acconto_liquidazione = CInt(idAccontoLiquidazione)
                             Select tratt

                If Not IsNothing(codRisUmFiltro) AndAlso codRisUmFiltro.Length > 0 Then
                    elenco = elenco.Where(Function(x) codRisUmFiltro.Contains(x.Cod_RisUm))
                End If

                If Not IsNothing(detrarreDaFattura) AndAlso detrarreDaFattura <> -1 Then
                    elenco = elenco.Where(Function(x) x.Detrarre_Da_Fattura = CInt(detrarreDaFattura))
                End If

                list = elenco.ToList()

                'Dim ut As New Gias_EF_Utility
                'dtElencoTrattenute = ut.ObjectQueryToDataTable(list)

                'Dim serializerSettings As New JsonSerializerSettings()
                'serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                'risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return list

    End Function

    Public Function IntestazioneDoc(ByVal cod_risum As Integer,
                                    ByRef int_obj As IntestazioneObj,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim result = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.IntestazioneDoc()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim imp_x_ind = From impxind In GiasContext.ImpresexIndirizzi
                            Select New With {
                                .cod_contatto = impxind.PIVA,
                                .cod_indirizzo = impxind.cod_indirizzo,
                                .tipo_indirizzo = impxind.Tipo_Indirizzo,
                                .orig = "0_IMPRESE"
                            }
            Dim cen_x_ind = From cenxind In GiasContext.CentrixIndirizzi
                            Select New With {
                                .cod_contatto = cenxind.PIVA,
                                .cod_indirizzo = cenxind.cod_indirizzo,
                                .tipo_indirizzo = cenxind.Tipo_Indirizzo,
                                .orig = "1_CENTRI"
                            }
            Dim tipi_indirizzo As Integer() = {3, 101}
            Dim cont_x_ind = From conxind In GiasContext.ContattiXIndirizzi
                             Where tipi_indirizzo.Contains(conxind.Tipo_Indirizzo)
                             Select New With {
                                 .cod_contatto = conxind.Cod_Contatto,
                                 .cod_indirizzo = conxind.Cod_Indirizzo,
                                 .tipo_indirizzo = conxind.Tipo_Indirizzo,
                                 .orig = "2_CONTATTI"
                             }

            Dim tutti_x_ind = imp_x_ind.Union(cen_x_ind).Union(cont_x_ind)

            Dim indirizzi = From risum In GiasContext.Risorse_Umane
                            Join cont In GiasContext.Contatti On risum.Cod_Contatto Equals cont.Cod_Contatto
                            Join tuttixind In tutti_x_ind On tuttixind.cod_contatto Equals cont.Cod_Contatto
                            Join ind In GiasContext.Indirizzi On tuttixind.cod_indirizzo Equals ind.cod_indirizzo
                            Where risum.Cod_RisUm = cod_risum
                            Order By tuttixind.orig
                            Select New IntestazioneObj With {
                                .Convenevoli = cont.Convenevoli,
                                .Rag_Soc = cont.Rag_Soc,
                                .Cognome = cont.Cognome,
                                .Nome = cont.Nome,
                                .Cod_Contatto = cont.Cod_Contatto,
                                .Codice_Fiscale = cont.Codice_Fiscale,
                                .Id_CF = cont.Id_CF,
                                .Progressivo = risum.Settore_Des,
                                .Ind_Des = ind.ind_des,
                                .CAP = ind.CAP,
                                .Com_Des = ind.com_des,
                                .Frz_Des = ind.frz_des,
                                .Pro_Cod = ind.pro_cod,
                                .Stato = ind.stato
                            }

            If (indirizzi.Any()) Then

                int_obj = indirizzi.First()

            Else

                result = False

            End If

        End Using

        Return result

    End Function

    Public Function ElencoPagatiSuConferito(ByVal piva As String,
                                            ByVal id_acconto_liquidazione As Integer,
                                            ByVal filtro_fornitori As Integer(),
                                            ByVal filtro_grpfatt As Integer(),
                                            ByVal filtro_specie As Integer,
                                            ByVal filtro_varieta As Integer(),
                                            ByRef objParametri As AgronicaCoreParametri,
                                            ByRef listaMovDettagliCollegati As List(Of Movimenti_dettagli)
                                            ) As List(Of Liquid_Mov_CampionamentoConferito)

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.ElencoPagatiSuConferito()"

        Dim gefutils As New Gias_EF_Utility

        Dim piva_superuser As String = objParametri.PivaSuperUser

        Dim list As New List(Of Liquid_Mov_CampionamentoConferito)

        Dim tutti_fornitori = True
        If filtro_fornitori.Length > 0 Then
            tutti_fornitori = False
        End If
        Dim tutti_grpfatt = True
        If filtro_grpfatt.Length > 0 Then
            tutti_grpfatt = False
        End If
        Dim tutte_varieta = True
        If (filtro_specie <> -1) Then
            If filtro_varieta.Length > 0 Then
                tutte_varieta = False
            End If
        End If

        'TODO Stefano - tmetti anche il nr di riga sulla tabella dove c'è il nr bolla
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim elenco = From e In GiasContext.Liquid_Mov_CampionamentoConferito
                         Where e.Piva_SuperUser.Equals(piva_superuser) AndAlso
                               e.PIVA.Equals(piva) AndAlso
                               e.Prezzo_Su_Conferito AndAlso
                               e.Id_acconto_liquidazione = id_acconto_liquidazione AndAlso
                               (tutti_fornitori OrElse filtro_fornitori.Contains(e.Cod_RisUm)) AndAlso
                               (tutti_grpfatt OrElse filtro_grpfatt.Contains(e.Grp_Fatt_Cod)) AndAlso
                               (filtro_specie = -1 OrElse e.Veg_Cod = filtro_specie) AndAlso
                               (tutte_varieta OrElse filtro_varieta.Contains(e.Cul_Cod))
                         Order By e.Cod_RisUm, e.Mat_Cod, e.Qual_Cod, e.Calibro_Entrata_Cod, e.Certif_Cod, e.Id_Mov_Det
                         Select e

            list = elenco.ToList()

            'Per capire se utilizzare o meno i dati di movimenti_dettagli, devo ricavare il tipoLiquidazione
            Dim tipoLiquidazione As String = (From a In GiasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                                              Where a.Piva_SuperUser = piva_superuser AndAlso
                                                      a.PIVA = piva AndAlso
                                                      a.id_anagrafica = CInt(id_acconto_liquidazione)
                                              Select a.tipo_anagrafica).FirstOrDefault()

            If tipoLiquidazione = "L" Then

                Dim listaIdMovDet As List(Of Integer) = list.Select(Function(x) x.Id_Mov_Det).ToList()

                listaMovDettagliCollegati = (From m In GiasContext.Movimenti_dettagli
                                             Where listaIdMovDet.Contains(m.Id_Mov_Det)
                                             Select m).ToList()

            End If


        End Using

        Return list

    End Function


    Public Function Prepara_Scheda_Liquidazione_Su_Campionato(ByVal piva As String,
                                                              ByVal id_acconto_liquidazione As Integer,
                                                              ByVal filtro_fornitori As Integer(),
                                                              ByVal filtro_grpfatt As Integer(),
                                                              ByVal filtro_specie As Integer,
                                                              ByVal filtro_varieta As Integer(),
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As List(Of Object)

        Dim messaggioErrore As String = ""

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.ElencoPagatiSuConferito()"

        Dim piva_superuser As String = objParametri.PivaSuperUser

        Dim list As New List(Of Object)

        Dim tutti_fornitori = True
        If filtro_fornitori.Length > 0 Then
            tutti_fornitori = False
        End If
        Dim tutti_grpfatt = True
        If filtro_grpfatt.Length > 0 Then
            tutti_grpfatt = False
        End If
        Dim tutte_varieta = True
        If (filtro_specie <> -1) Then
            If filtro_varieta.Length > 0 Then
                tutte_varieta = False
            End If
        End If

        Dim risposta As String = ""

        Try
            Dim pivaSuperUser = objParametri.PivaSuperUser

            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                'giasContext.ContextOptions.LazyLoadingEnabled = False
                'TODO Stefano - togli il Join e metti anche il nr di riga sulla tabella dove c'è il nr bolla
                Dim elenco = (From e In giasContext.Liquid_Mov_CampionamentoConferito.Include("Liquid_Mov_CampionamentoConferito")
                              Where e.Piva_SuperUser.Equals(piva_superuser) _
                         AndAlso e.PIVA.Equals(piva) _
                         AndAlso Not e.Prezzo_Su_Conferito _
                         AndAlso e.Id_acconto_liquidazione = id_acconto_liquidazione _
                         AndAlso (tutti_fornitori OrElse filtro_fornitori.Contains(e.Cod_RisUm)) _
                         AndAlso (tutti_grpfatt OrElse filtro_grpfatt.Contains(e.Grp_Fatt_Cod)) _
                         AndAlso (filtro_specie = -1 OrElse e.Veg_Cod = filtro_specie) _
                         AndAlso (tutte_varieta OrElse filtro_varieta.Contains(e.Cul_Cod))
                              Order By e.Cod_RisUm, e.Mat_Cod, e.Qual_Cod, e.Calibro_Entrata_Cod, e.Certif_Cod, e.Id_Mov_Det
                              Select New With {
                                  .Cod_RisUm = e.Cod_RisUm,
                                  .Grp_Fatt_Sigla = e.Grp_Fatt_Sigla,
                                  .Grp_Fatt_Descr = e.Grp_Fatt_Descr,
                                  .Mat_Cod = e.Mat_Cod,
                                  .Qual_Cod = e.Qual_Cod,
                                  .Calibro_Entrata_Cod = e.Calibro_Entrata_Cod,
                                  .Certif_Cod = e.Certif_Cod,
                                  .Mat_Des = e.Mat_Des,
                                  .Qual_Sigla = e.Qual_Sigla,
                                  .Calibro_Entrata_Sigla = e.Calibro_Entrata_Sigla,
                                  .Certif_Sigla = e.Certif_Sigla,
                                  .PercValoreAcconto = e.perc_valore_acconto,
                                  .Id_Mov_Det = e.Id_Mov_Det,
                                  .Doc_Numero_Des = e.Doc_Numero_Des,
                                  .Doc_Numero = e.Doc_Numero,
                                  .Doc_Numero_Sin = e.Doc_Numero_Sin,
                                  .NrRiga = e.NrRiga,
                                  .DtDoc = e.Data_Documento,
                                  .Calibro = (From c In e.Liquid_Mov_PerCalibro_CampionamentoConferito
                                              Select New With {
                                                  .Ordine = c.Ordinam_Calibro,
                                                  .Id_Calibro = c.Id_Calibro,
                                                  .Des_Calibro = c.Qual_Calibro_Descr,
                                                  .KgNetti = c.KgNetti,
                                                  .Prezzo = c.PrezzoBaseAlKg + c.PrezzoTotaleFattVarAlKg,
                                                  .Imponibile = c.Imponibile})
                              }).ToList

                For Each obj In elenco
                    '.Qual_Sigla = e.Qual_Sigla,
                    '              .Certif_Sigla = e.Certif_Sigla,
                    '              .Calibro_Sigla = e.Calibro_Entrata_Sigla,

                    list.Add(obj)
                Next

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return list   'risposta

    End Function

    Public Function TrovaRigheAccontoLiquidazione(ByVal piva As String,
                                                  ByVal Id_Acconto_Liquidazione As Integer,
                                                  ByRef Liquidazione_Mov_Dati_Generali As List(Of Liquid_Mov_Dati_Generali_CampionamentoConferito),
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Dim messaggioErrore As String = ""

        Dim risposta As Boolean = False

        Dim pivaSuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.TrovaRigheAccontoLiquidazione()"

        Try

            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            'Dim transactionOptions As New System.Transactions.TransactionOptions()
            'transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted

            'Using scope As New TransactionScope(System.Transactions.TransactionScopeOption.Required,
            '            transactionOptions)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                ' GiasContext.ContextOptions.LazyLoadingEnabled = False

                giasContext.Database.CommandTimeout = 3600

                Liquidazione_Mov_Dati_Generali = (From lm In giasContext.Liquid_Mov_Dati_Generali_CampionamentoConferito _
                                                        .Include("Liquid_Mov_CampionamentoConferito") _
                                                        .Include("Liquid_Mov_CampionamentoConferito.Liquid_Mov_PerCalibro_CampionamentoConferito") _
                                                        .Include("Liquid_Mov_CampionamentoConferito.Liquid_Mov_FattVariaz_CampionamentoConferito")
                                                  Where lm.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                                                        lm.PIVA.Equals(piva) AndAlso
                                                        lm.Id_acconto_liquidazione = Id_Acconto_Liquidazione).ToList()

                'scope.Complete()

            End Using

            'End Using


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function Leggi_Liquidazione_Banche(ByVal piva As String,
                                              ByVal idAccontoLiquidazione As Integer,
                                              ByVal codRisUmFiltro As Integer(),
                                              ByRef dtLiquidazioneBanche As DataTable,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Dim messaggioErrore As String = ""
        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.Leggi_Liquidazione_Banche()"

        Dim risposta As String = ""

        Try
            Dim pivaSuperUser = objParametri.PivaSuperUser

            Dim tuttiFornitori As Boolean = True
            If codRisUmFiltro.Length > 0 Then
                tuttiFornitori = False
            End If

            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                'Per capire se utilizzare o meno i dati di movimenti_dettagli, devo ricavare il tipoLiquidazione
                Dim tipoLiquidazione As String = (From a In giasContext.AnagAccontiLiquidazioni_CampionamentoConferito
                                                  Where a.Piva_SuperUser = pivaSuperUser AndAlso
                                                          a.PIVA = piva AndAlso
                                                          a.id_anagrafica = CInt(idAccontoLiquidazione)
                                                  Select a.tipo_anagrafica).FirstOrDefault()

                Dim elenco = From campioConf In giasContext.Liquid_Mov_CampionamentoConferito
                             Join risUm In giasContext.Risorse_Umane
                                 On campioConf.Cod_RisUm Equals risUm.Cod_RisUm
                             Join rapCont In giasContext.Rapporti_Contabili
                                 On risUm.Cod_Rapporto Equals rapCont.Cod_Rapporto
                             Join contatti In giasContext.Contatti
                                 On risUm.Cod_Contatto Equals contatti.Cod_Contatto
                             Join movDet In giasContext.Movimenti_dettagli
                                 On campioConf.PIVA Equals movDet.PIVA And campioConf.Id_Mov_Det Equals movDet.Id_Mov_Det
                             Where campioConf.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                                   campioConf.PIVA.Equals(piva) AndAlso
                                   campioConf.Id_acconto_liquidazione = CInt(idAccontoLiquidazione) AndAlso
                                   (tuttiFornitori OrElse codRisUmFiltro.Contains(campioConf.Cod_RisUm))
                             Group By x = New With {
                                 Key campioConf.Id_acconto_liquidazione,
                                 Key campioConf.Cod_RisUm,
                                 Key risUm.Settore_Des,
                                 Key risUm.Cod_Rapporto,
                                 Key rapCont.Rapporto_Des,
                                 Key contatti.Rag_Soc,
                                 Key campioConf.Cod_Iva
                             } Into g = Group
                             Order By x.Rag_Soc
                             Select New With {
                                .IdAccontoLiquidazione = x.Id_acconto_liquidazione,
                                .CodRisUm = x.Cod_RisUm,
                                .Progressivo = x.Settore_Des,
                                .RapportoContabile = x.Rapporto_Des,
                                .Nominativo = x.Rag_Soc,
                                .Imponibile = g.Sum(Function(r) If(tipoLiquidazione = "L" AndAlso Not r.movDet.Imponibile_Netto Is Nothing AndAlso r.movDet.Imponibile_Netto <> 0,
                                                                   (-1 * r.movDet.Imponibile_Netto),
                                                                   r.campioConf.Imponibile)),
                                .ImponibileAcconti = (From tratt1 In giasContext.Liquid_Mov_Trattenute_CampionamentoConferito
                                                      Where tratt1.Id_acconto_liquidazione = x.Id_acconto_liquidazione AndAlso
                                                            tratt1.Cod_RisUm = x.Cod_RisUm AndAlso
                                                            tratt1.Detrarre_Da_Fattura = 1
                                                      Select tratt1.Imponibile).DefaultIfEmpty(0).Sum(),
                                .ImponibileTot = CDec(0),
                                .CodIva = x.Cod_Iva,
                                .Iva = CDec(0),
                                .Importo = CDec(0),
                                .UlterioriDetrazioni = -1 * ((From tratt0 In giasContext.Liquid_Mov_Trattenute_CampionamentoConferito
                                                              Where tratt0.Id_acconto_liquidazione = x.Id_acconto_liquidazione AndAlso
                                                                    tratt0.Cod_RisUm = x.Cod_RisUm AndAlso
                                                                    tratt0.Detrarre_Da_Fattura = 0
                                                              Select tratt0.Imponibile + tratt0.Iva).DefaultIfEmpty(0).Sum()),
                                .Saldo = CDec(0)
                             }

                Dim lista = elenco.ToList()

                'questi conteggi mi tocca farmeli qui in un altro giro
                For Each item In lista
                    item.ImponibileTot = item.Imponibile - item.ImponibileAcconti
                    item.Iva = ArrotondaVal_2((item.ImponibileTot * 4) / 100)
                    item.Importo = item.ImponibileTot + item.Iva
                    item.Saldo = item.Importo + item.UlterioriDetrazioni
                Next


                Dim ut As New Gias_EF_Utility
                dtLiquidazioneBanche = ut.ObjectQueryToDataTable(lista)

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(lista, Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

    Public Class IntestazioneObj
        Public Convenevoli As String
        Public Rag_Soc As String
        Public Cognome As String
        Public Nome As String
        Public Cod_Contatto As String
        Public Codice_Fiscale As String
        Public Id_CF As Integer
        Public Progressivo As String
        Public Ind_Des As String
        Public CAP As String
        Public Com_Des As String
        Public Frz_Des As String
        Public Pro_Cod As String
        Public Stato As String

        Public Function Chi() As String
            Dim result As String = Rag_Soc & " " & Cognome & " " & Nome
            Return result.Trim()
        End Function

        Public Function Dove() As String
            Dim result As String = Com_Des
            If (Not String.IsNullOrEmpty(Frz_Des)) Then
                If (Not String.IsNullOrEmpty(Com_Des)) Then
                    result = Com_Des & " " & Frz_Des
                Else
                    result = Frz_Des
                End If
            End If
            Return result
        End Function

    End Class

    Public Function Leggi_Risultato_Liquidazione(ByVal Piva As String,
                                                 ByVal liquidazione_scelta As Integer,
                                                 ByVal fornitori As String,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_R.Leggi_Risultato_Liquidazione()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt_Ris_Liqu As DataTable
        Dim risposta As String = String.Empty

        'LETTURA PARAMETRI QUALITATIVI GESTITI
        Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim DTParamQual As New DataTable
        DTParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametri)

        'LETTURA FATTORI DI VARIAZIONE GESTITI
        Dim objliquid_fattVar As New AgronicaCoreContabDAL.FF_LiquidazioneSoci_R
        Dim DTFattVar As New DataTable
        DTFattVar = objliquid_fattVar.LeggiBlocchiLiquid_Mov_FattVariaz_CampionamentoConferito(objParametri.PivaSuperUser,
                                                                              Piva, 0, objParametri, liquidazione_scelta, enumSelezioneVariabile.Selezione_TabellaDatiMinimi)

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT co.Rag_Soc, ")
            StrSQL.AppendLine(" ISNULL(m.Doc_Numero_Visualizzato,'')AS Doc_Numero_Visualizzato,ISNULL(m_DDt.Mov_Desc ,'') AS DDT_Completo, ")

            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine("     WHEN ru.Cod_Rapporto=-18 ")
            StrSQL.AppendLine("     THEN 'Conferimento' ")
            StrSQL.AppendLine(" ELSE 'Acquisto'")
            StrSQL.AppendLine(" END AS ConferimentoOAcquisto,")

            StrSQL.AppendLine(" lmcc.NrRiga,lmcc.Data_Documento AS Data_Movimento, lmcc.Data_Riferimento_Prezzi, ")

            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine("     WHEN lmcc.Tipo_Data_Riferimento_Prezzi='E' ")
            StrSQL.AppendLine("     THEN 'Entrata' ")
            StrSQL.AppendLine(" ELSE 'Semina'")
            StrSQL.AppendLine(" END As Tipo_Data_Riferimento_Prezzi,")

            StrSQL.AppendLine(" lmcc.Grp_Fatt_Descr,lmcc.Mat_Des,lmcc.Calibro_Entrata_Sigla,lmcc.Qual_Sigla,lmcc.Certif_Sigla, ")

            StrSQL.AppendLine(" ISNULL(CASE ")
            StrSQL.AppendLine("         WHEN lmpc.Ordinam_Calibro = 0 ")
            StrSQL.AppendLine("         THEN '' ")
            StrSQL.AppendLine(" ELSE '0' + convert(varchar, lmpc.Ordinam_Calibro) ")
            StrSQL.AppendLine(" END + lmpc.Qual_calibro_descr,'') As Descr_OrdinQualCalibro_Camp,")

            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine("        WHEN lmcc.Prezzo_da_riga_conferim=1 ")
            StrSQL.AppendLine("        THEN 'Sì'")
            StrSQL.AppendLine(" ELSE 'No' ")
            StrSQL.AppendLine("  END AS Prezzo_da_riga_conferimento,")

            StrSQL.AppendLine(" ISNULL(lmpc.PrezzoBaseAlKg,  lmcc.PrezzoBaseAlKg) As Prezzo,")

            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine("     WHEN ISNULL(lmpc.PrezzoBaseAlKg, 0) +  ")
            StrSQL.AppendLine("     ISNULL(lmpc.PrezzoTotaleFattVarAlKg, 0)  = 0 ")
            StrSQL.AppendLine("     Then lmcc.PrezzoTotaleAlKg  ")
            StrSQL.AppendLine(" Else ISNULL(lmpc.PrezzoBaseAlKg, 0) + ")
            StrSQL.AppendLine("      ISNULL(lmpc.PrezzoTotaleFattVarAlKg, 0) ")
            StrSQL.AppendLine(" End As TotalePrezzo,")

            StrSQL.AppendLine(" lmcc.DegradoPerc,ISNULL(lmpc.Degrado,lmcc.Degrado)As Degrado,")
            StrSQL.AppendLine(" ISNULL(lmpc.KgNetti,lmcc.KgNetti) As KgPerCalibro,ISNULL(lmpc.Tara ,lmcc.Tara) As TaraKgPerCalibro, ")
            StrSQL.AppendLine(" ISNULL(lmpc.Imponibile,  lmcc.Imponibile) As Imponibile,ISNULL(ccmr.PercentualeCampionato,0) As Percentuale_Campionato, ")

            StrSQL.AppendLine(" ISNULL(ccm.Note,'')AS Note, ")

            StrSQL.AppendLine(" ISNULL(CASE ")
            StrSQL.AppendLine("     WHEN ISNULL(ccm.StatoCampionamento,'')='' ")
            StrSQL.AppendLine("     THEN 'Non Campionato'  ")
            StrSQL.AppendLine(" ELSE 'Definitivo'")
            StrSQL.AppendLine(" END, 'Non Campionato')AS StatoCampionamento,")

            StrSQL.AppendLine(" ISNULL(ca.Sa_Nome,'') AS Centri_Aziendali, ")

            StrSQL.AppendLine(" ISNULL(CASE ")
            StrSQL.AppendLine("     WHEN ISNULL(ccm.Automatico,0) = 0")
            StrSQL.AppendLine("     THEN 'Manuale'  ")
            StrSQL.AppendLine(" ELSE 'Automatico'")
            StrSQL.AppendLine(" END, 'Manuale')AS Automatico,")

            StrSQL.AppendLine(" sv.Veg_Des, cu.Cul_Des,")

            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine("     WHEN mt.Regolamento = " & enum_Cod_Regolamento.Regolamento_bio)
            StrSQL.AppendLine("     THEN 'Biologico (Reg.CE 834/07 (Ex.Reg.CE 2092/91))' ")
            StrSQL.AppendLine(" ELSE 'Convenzionale (Reg. Nessuno)'")
            StrSQL.AppendLine(" END As Reg_Des,")

            StrSQL.AppendLine(" mt.Cod_Articolo, mt.Codice_Esterno ")

            StrSQL.AppendLine(" ")

            'ELENCO CAMPI MATERIE_PRIME_CAMPIONATURE
            If DTParamQual IsNot Nothing Then
                For Each paramQual In DTParamQual.Rows
                    If paramQual("Tipo") = 3 Then
                        'Numero
                        StrSQL.AppendLine(" , COALESCE(Convert(float, Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod), 0) As FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                    Else
                        If paramQual("Tipo") = 4 Then
                            'Stringa
                            StrSQL.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') As FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                        Else
                            If paramQual("Tipo") = 5 Then
                                'Data
                                StrSQL.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') As FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                            Else
                                'DDL
                                StrSQL.AppendLine(" , COALESCE(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod, 0) AS FF_" & paramQual("Tabella_Key") & "_Tipo_Cod  ")

                                If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                    StrSQL.AppendLine(" , COALESCE(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Sigla, '') AS FF_" & paramQual("Tabella_Key") & "_Sigla  ")
                                    StrSQL.AppendLine(" , COALESCE(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Descrizione, '') AS FF_" & paramQual("Tabella_Key") & "_Descrizione ")
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'ELENCO CAMPI FATTORI DI VARIAZIONE GESTITI
            If DTFattVar IsNot Nothing Then
                For Each FattVar In DTFattVar.Rows
                    Dim id_fatt_var_alias As String = "fatt_var_" & CStr(FattVar("id_fattore_variazione"))
                    Dim id_fatt_var_alias_cal As String = "fatt_var_cal_" & CStr(FattVar("id_fattore_variazione"))
                    Dim id_fatt_var_nomecampo As String = "fatt_var_" & CStr(FattVar("id_fattore_variazione")) & "_PrezzoAlKg"
                    StrSQL.AppendLine(",ISNULL(" & id_fatt_var_alias & ".PrezzoAlKg, ISNULL(" & id_fatt_var_alias_cal & ".PrezzoAlKg, 0)) As " & id_fatt_var_nomecampo)
                Next
            End If

            StrSQL.AppendLine(" , ISNULL(ccm.QtaCampionata, 0) / 100 * ISNULL(ccmr.PercentualeCampionato, 0)  As QtaCampionata ")
            StrSQL.AppendLine(" , ISNULL(lmpc.Messaggio_Errore, lmcc.Messaggio_Errore) As Messaggio ")
            StrSQL.AppendLine(" ,ISNULL(ag.Id_Agenda,0) AS Id_Agenda ")
            StrSQL.AppendLine(" ,ISNULL(ag.Lav_Cod,0) AS Lav_Cod, ISNULL(anagacc.definitivo,0) AS Definitivo")
            StrSQL.AppendLine(" ,ISNULL(ag.Blocco_Flag,0) AS Blocco_Flag")
            StrSQL.AppendLine(" ,ISNULL(lmcc.Id_Mov_Det,0) AS Id_Mov_Det")

            StrSQL.AppendLine(" FROM Liquid_Mov_Dati_Generali_CampionamentoConferito lmdg ")



            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Liquid_Mov_CampionamentoConferito lmcc ")
            StrSQL.AppendLine(" On lmdg.Piva_SuperUser=lmcc.Piva_SuperUser ")
            StrSQL.AppendLine(" And lmdg.PIVA=lmcc.PIVA  ")
            StrSQL.AppendLine(" And lmdg.Id_acconto_liquidazione=lmcc.Id_acconto_liquidazione  ")
            StrSQL.AppendLine(" And lmdg.Cod_RisUm=lmcc.Cod_RisUm  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Movimenti_Dettagli md ")
            StrSQL.AppendLine(" On md.PIVA=lmcc.PIVA  ")
            StrSQL.AppendLine(" And md.Id_mov_det=lmcc.Id_mov_det  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Agenda ag  ")
            StrSQL.AppendLine(" On md.PIVA=ag.PIVA   ")
            StrSQL.AppendLine(" And md.Id_Agenda=ag.Id_Agenda  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Movimenti m ")
            StrSQL.AppendLine(" On ag.PIVA=m.PIVA  ")
            StrSQL.AppendLine(" And ag.Id_Agenda= m.Id_Agenda ")
            StrSQL.AppendLine(" And m.Cau_Mov='4000' ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Movimenti m_DDt ")
            StrSQL.AppendLine(" ON ag.PIVA=m_DDt.PIVA  ")
            StrSQL.AppendLine(" AND ag.Id_Agenda= m_DDt.Id_Agenda ")
            StrSQL.AppendLine(" AND m_DDt.Cau_Mov='4050' ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Risorse_Umane ru ")
            StrSQL.AppendLine(" ON lmdg.Cod_RisUm=ru.Cod_RisUm  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Contatti co ")
            StrSQL.AppendLine(" ON (lmdg.PIVA=co.PIVA or co.sa_cod = -1) ")
            StrSQL.AppendLine(" AND ru.Cod_Contatto=co.Cod_Contatto  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Centri_Aziendali ca ")
            StrSQL.AppendLine(" ON md.PIVA=ca.PIVA ")
            StrSQL.AppendLine(" AND md.Sa_Cod=ca.Sa_Cod  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN AnagAccontiLiquidazioni_CampionamentoConferito anagacc ")
            StrSQL.AppendLine(" ON anagacc.Piva_SuperUser = lmdg.Piva_SuperUser ")
            StrSQL.AppendLine(" AND anagacc.PIVA = lmdg.PIVA ")
            StrSQL.AppendLine(" AND anagacc.id_anagrafica = lmdg.Id_acconto_liquidazione ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" LEFT JOIN Liquid_Mov_PerCalibro_CampionamentoConferito lmpc ")
            StrSQL.AppendLine(" ON lmdg.Piva_SuperUser=lmpc.Piva_SuperUser ")
            StrSQL.AppendLine(" AND lmdg.PIVA=lmpc.PIVA  ")
            StrSQL.AppendLine(" AND lmdg.Id_acconto_liquidazione=lmpc.Id_acconto_liquidazione  ")
            StrSQL.AppendLine(" AND lmdg.Cod_RisUm=lmpc.Cod_RisUm  ")
            StrSQL.AppendLine(" AND lmcc.Id_Mov_Det=lmpc.Id_Mov_Det  ")

            If DTFattVar IsNot Nothing Then
                For Each FattVar In DTFattVar.Rows
                    Dim id_fatt_var_alias As String = "fatt_var_" & CStr(FattVar("id_fattore_variazione"))
                    Dim id_fatt_var_alias_cal As String = "fatt_var_cal_" & CStr(FattVar("id_fattore_variazione"))

                    StrSQL.AppendLine(" ")
                    StrSQL.AppendLine(" LEFT JOIN Liquid_Mov_FattVariaz_CampionamentoConferito " & id_fatt_var_alias)
                    StrSQL.AppendLine(" ON lmcc.Piva_SuperUser= " & id_fatt_var_alias & ".Piva_SuperUser ")
                    StrSQL.AppendLine(" AND lmcc.PIVA= " & id_fatt_var_alias & ".PIVA  ")
                    StrSQL.AppendLine(" AND lmcc.Id_acconto_liquidazione= " & id_fatt_var_alias & ".Id_acconto_liquidazione  ")
                    StrSQL.AppendLine(" AND lmcc.Cod_RisUm= " & id_fatt_var_alias & ".Cod_RisUm  ")
                    StrSQL.AppendLine(" AND lmcc.Id_Mov_Det= " & id_fatt_var_alias & ".Id_Mov_Det  ")
                    StrSQL.AppendLine(" AND  " & id_fatt_var_alias & ".Id_calibro = 0  ")
                    StrSQL.AppendLine(" AND  " & id_fatt_var_alias & ".Id_fattore_variazione = " & CInt(FattVar("id_fattore_variazione")))

                    StrSQL.AppendLine(" ")
                    StrSQL.AppendLine(" LEFT JOIN Liquid_Mov_FattVariaz_CampionamentoConferito " & id_fatt_var_alias_cal)
                    StrSQL.AppendLine(" On lmpc.Piva_SuperUser= " & id_fatt_var_alias_cal & ".Piva_SuperUser ")
                    StrSQL.AppendLine(" And lmpc.PIVA= " & id_fatt_var_alias_cal & ".PIVA  ")
                    StrSQL.AppendLine(" And lmpc.Id_acconto_liquidazione= " & id_fatt_var_alias_cal & ".Id_acconto_liquidazione  ")
                    StrSQL.AppendLine(" And lmpc.Cod_RisUm= " & id_fatt_var_alias_cal & ".Cod_RisUm  ")
                    StrSQL.AppendLine(" And lmpc.Id_Mov_Det= " & id_fatt_var_alias_cal & ".Id_Mov_Det  ")
                    StrSQL.AppendLine(" And  lmpc.Id_calibro= " & id_fatt_var_alias_cal & ".Id_calibro ")
                    StrSQL.AppendLine(" AND  " & id_fatt_var_alias & ".Id_fattore_variazione = " & CInt(FattVar("id_fattore_variazione")))

                Next
            End If



            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" LEFT JOIN CampionamentoConferito_Movimenti ccm")
            StrSQL.AppendLine(" On  lmcc.Piva_SuperUser = ccm.Piva_SuperUser ")
            StrSQL.AppendLine(" And lmcc.PIVA=ccm.PIVA   ")
            StrSQL.AppendLine(" And lmcc.Id_Mov_Det=ccm.Id_Mov_Det  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" LEFT JOIN CampionamentoConferito_Movimenti_Righe ccmr")
            StrSQL.AppendLine(" On  lmpc.Piva_SuperUser = ccmr.Piva_SuperUser ")
            StrSQL.AppendLine(" And lmpc.PIVA=ccmr.PIVA   ")
            StrSQL.AppendLine(" And lmpc.Id_Mov_Det=ccmr.Id_Mov_Det  ")
            StrSQL.AppendLine(" And lmpc.Id_Calibro=ccmr.Id_Calibro  ")

            If DTParamQual IsNot Nothing Then
                StrSQL.AppendLine(" ")
                For Each paramQual In DTParamQual.Rows
                    StrSQL.AppendLine(" LEFT JOIN Materie_Prime_Campionature As Materie_Prime_Campionature_" & paramQual("Tabella_Key"))
                    StrSQL.AppendLine("     On md.Cal_Cod = Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Progressivo ")
                    StrSQL.AppendLine("     And Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo = 'o" & paramQual("Tabella_Key") & "'")
                    If paramQual("Tipo") = 1 AndAlso Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                        StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" & paramQual("Tabella_Key"))
                        StrSQL.AppendLine("     ON Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod =  OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Par_Cod ")
                        StrSQL.AppendLine("     AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & paramQual("Tabella_ID") & "'")
                    End If
                Next
            End If


            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Materie_Prime mt ")
            StrSQL.AppendLine(" ON lmcc.Mat_Cod=mt.Mat_Cod  ")
            StrSQL.AppendLine(" AND lmcc.Veg_Cod=mt.Veg_Cod  ")
            StrSQL.AppendLine(" AND lmcc.Cul_Cod=mt.Cul_Cod  ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN SpecieVegetali sv ")
            StrSQL.AppendLine(" ON mt.Veg_Cod=sv.Veg_Cod ")

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" JOIN Cultivar cu ")
            StrSQL.AppendLine(" ON mt.Veg_Cod=cu.Veg_Cod ")
            StrSQL.AppendLine(" AND mt.Cul_Cod=cu.Cul_Cod ")

            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" WHERE lmdg.Piva_SuperUser = '" & CStr(objParametri.PivaSuperUser) & "'")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND lmdg.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If liquidazione_scelta <> 0 Then
                StrSQL.AppendLine(" AND lmdg.Id_acconto_liquidazione = " & Agro_SQL_SaveNum(liquidazione_scelta) & "   ")
            End If

            If Not String.IsNullOrEmpty(fornitori) Then
                StrSQL.AppendLine(" AND co.Cod_Contatto IN (" & Agro_SQL_Save_Clausola_IN(fornitori, True) & ")   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If



            '--------------------------------------------------------------------------
            dt_Ris_Liqu = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)
            Dim c As ColonneNome

            c = New ColonneNome("Id_Agenda", "Id_Agenda", "number") With {
                    ._hidden = True
               }
            l.Add(c)

            c = New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {
                    ._hidden = True
               }
            l.Add(c)

            c = New ColonneNome("Blocco_Flag", "Blocco_Flag", "number") With {
                    ._hidden = True
               }
            l.Add(c)

            c = New ColonneNome("Definitivo", "Definitivo", "number") With {
                    ._hidden = True
               }
            l.Add(c)

            c = New ColonneNome("Id_Mov_Det", "Id_Mov_Det", "number") With {
                    ._hidden = True
               }
            l.Add(c)

            c = New ColonneNome("Rag_Soc", "Ragione sociale", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
               }
            l.Add(c)

            c = New ColonneNome("ConferimentoOAcquisto", "Acq. / Confer.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Doc_Numero_Visualizzato", "Nr. Doc.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("DDT_Completo", "Nr. DDT", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("NrRiga", "Riga", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Data_Movimento", "Data movimento", "date") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "{0:dd/MM/yyyy}"
            }
            l.Add(c)

            c = New ColonneNome("Data_Riferimento_Prezzi", "Data rifer. prezzi", "date") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "{0:dd/MM/yyyy}"
            }
            l.Add(c)

            c = New ColonneNome("Tipo_Data_Riferimento_Prezzi", "Tipo Data rifer. prezzi", "string") With {
                ._Editabile = False,
                ._Filtrabile = True,
                ._FiltrabileConCheck = True,
                ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Grp_Fatt_Descr", "Gruppo Fatt.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Mat_Des", "Prodotto", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Cod_Articolo", "Codice", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Veg_Des", "Specie", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Cul_Des", "Varietà", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Reg_Des", "Regolamento", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Codice_Esterno", "Codice Esterno", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            'COLONNE DA MOSTRARE PARAMETRI QUALITATIVI
            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 3 OrElse paramQual("Tipo") = 4 OrElse paramQual("Tipo") = 5 Then
                    If paramQual("Tipo") = 3 Then
                        c = New ColonneNome("FF_" & paramQual("Tabella_Key") & "_Val_Cod", paramQual("Tabella_Des"), "number") With {
                    ._formatNr = "n2"
                    }
                    End If
                    If paramQual("Tipo") = 4 Then
                        c = New ColonneNome("FF_" & paramQual("Tabella_Key") & "_Val_Cod", paramQual("Tabella_Des"), "string")
                    End If
                    If paramQual("Tipo") = 5 Then
                        c = New ColonneNome("FF_" & paramQual("Tabella_Key") & "_Val_Cod", paramQual("Tabella_Des"), "date")
                    End If
                    c._Filtrabile = True
                    c._Display = True
                    c._Editabile = False
                    l.Add(c)
                Else
                    If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then

                        c = New ColonneNome("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod", paramQual("Tabella_Key") & "_Tipo_Cod", "number")
                        c._hidden = True
                        l.Add(c)

                        c = New ColonneNome("FF_" & paramQual("Tabella_Key") & "_Sigla", paramQual("Tabella_Des") & " " & "Sigla", "string")
                        c._Filtrabile = True
                        c._FiltrabileConCheck = True
                        c._Display = True
                        c._Editabile = False
                        l.Add(c)

                        c = New ColonneNome("FF_" & paramQual("Tabella_Key") & "_Descrizione", paramQual("Tabella_Des"), "string")
                        c._Filtrabile = True
                        c._FiltrabileConCheck = True
                        c._Display = False
                        c._Editabile = False
                        l.Add(c)

                    End If
                End If

            Next

            c = New ColonneNome("Descr_OrdinQualCalibro_Camp", "Campionatura", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Prezzo", "Prezzo", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "n6"
            }
            l.Add(c)

            'COLONNE DA MOSTRARE FATTORI DI VARIAZIONE 
            If DTFattVar IsNot Nothing Then
                For Each FattVar In DTFattVar.Rows
                    Dim id_fatt_var_descr As String = "Variazione " & CStr(FattVar("fatt_var_descr"))
                    Dim id_fatt_var_nomecampo As String = "fatt_var_" & CStr(FattVar("id_fattore_variazione")) & "_PrezzoAlKg"
                    c = New ColonneNome(id_fatt_var_nomecampo, id_fatt_var_descr, "number") With {
                                ._Editabile = False,
                                ._Filtrabile = True,
                                ._Display = True,
                                ._formatNr = "n8"
                        }

                    l.Add(c)

                Next
            End If

            c = New ColonneNome("TotalePrezzo", "Prezzo totale", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "n8"
            }
            l.Add(c)

            c = New ColonneNome("DegradoPerc", "% Degrado", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = False,
                    ._formatNr = "n2"
            }
            l.Add(c)

            c = New ColonneNome("Degrado", "Degrado", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n0"
            }
            l.Add(c)

            c = New ColonneNome("KgPerCalibro", "Kg totali", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n0"
            }
            l.Add(c)

            c = New ColonneNome("TaraKgPerCalibro", "Tara totale", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n5"
            }
            l.Add(c)

            c = New ColonneNome("Imponibile", "Imponibile", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n2"
            }
            l.Add(c)

            c = New ColonneNome("Percentuale_Campionato", "% campione", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._formatNr = "n5"
            }
            l.Add(c)

            c = New ColonneNome("QtaCampionata", "Kg campione", "number") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._Display = True,
                    ._sum = True,
                    ._formatNr = "n5"
            }
            l.Add(c)

            c = New ColonneNome("Note", "Note", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("StatoCampionamento", "Stato Campion.", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Automatico", "Tipo Campionamento", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Centri_Aziendali", "Centro Aziendale", "string") With {
                ._Editabile = False,
                ._Filtrabile = True,
                ._FiltrabileConCheck = True,
                ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Messaggio", "Messaggi", "string") With {
                    ._Editabile = False,
                    ._Filtrabile = True,
                    ._FiltrabileConCheck = True,
                    ._Display = True
            }
            l.Add(c)

            c = New ColonneNome("Prezzo_da_riga_conferimento", "Prezzo Forzato in Riga", "string") With {
                ._Editabile = False,
                ._Filtrabile = True,
                ._FiltrabileConCheck = True,
                ._Display = True
            }
            l.Add(c)

            Dim js As New JSON_DataTable With {.Editabile_Deafault = False}
            risposta = js.JSON_DataTable_Kendo(dt_Ris_Liqu, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt_Ris_Liqu = Nothing
            Throw New Exception("[" & NomeRoutine & "] :     " & MessaggioErrore)
        End Try

        Return risposta

    End Function
End Class

'#################################################################
'#################################################################
'#################################################################

Public Class FF_LiquidazioneSoci_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean


        Dim nomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
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
            strSql.Length = 0
            strSql.Append(" INSERT ... " & vbCrLf)

            strSql.Append("              (")
            strSql.Append("              Inviato,            datainvio, ")
            strSql.Append("              Data_Creazione,     Data_Modifica, ")
            strSql.Append("              UserName_Creazione, UserName_Modifica, ")
            strSql.Append("              Validita_Inizio,    Validita_Fine, ")
            strSql.Append("              Validazione, Data_Validazione, UserName_Validazione " & vbCrLf)
            strSql.Append("              ) ")

            strSql.Append(" VALUES ( ")



            strSql.Append("         , 0  " & vbCrLf)
            strSql.Append("         , Null  " & vbCrLf)

            strSql.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            strSql.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function



    '##############################################################################################
    Public Function Scrivi_Acconti_Liquidazioni(
                ByVal piva As String,
                ByVal Id_Acconto_Liquidazione As Integer,
                ByVal Liquidazione_Mov_Dati_Generali_ToInsert As List(Of Liquid_Mov_Dati_Generali_CampionamentoConferito),
                ByVal Liquidazione_Mov_ToInsert As List(Of Liquid_Mov_CampionamentoConferito),
                ByVal Liquidazione_Mov_Calibri_ToInsert As List(Of Liquid_Mov_PerCalibro_CampionamentoConferito),
                ByVal Liquidazione_Mov_FattVar_ToInsert As List(Of Liquid_Mov_FattVariaz_CampionamentoConferito),
                ByRef objParametri As AgronicaCoreParametri
        ) As String

        Dim messaggioErrore As String = ""

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Scrivi_Acconti_Liquidazioni()"

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Dim leggi_liquidazioneSoci As New FF_LiquidazioneSoci_R

            Dim Liquidazione_Mov_Dati_Generali_ToDelete As List(Of Liquid_Mov_Dati_Generali_CampionamentoConferito) = Nothing

            'Cerco le righe da cancellare di precedenti acconti
            messaggioErrore = leggi_liquidazioneSoci.TrovaRigheAccontoLiquidazione(
                                    piva, Id_Acconto_Liquidazione,
                                    Liquidazione_Mov_Dati_Generali_ToDelete,
                                    objParametri)

            If messaggioErrore = "" Then

                'Eseguo comunque sempre la cancellazione con una prima transazione a parte
                '   Se qualcosa andasse male nella fase di inserimento è comunque meglio non avere i dati vecchi

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    ' Cancellazione lancio precedente
                    For Each lmd As Liquid_Mov_Dati_Generali_CampionamentoConferito In Liquidazione_Mov_Dati_Generali_ToDelete
                        GiasContext.Liquid_Mov_Dati_Generali_CampionamentoConferito.Attach(lmd)
                        GiasContext.Liquid_Mov_Dati_Generali_CampionamentoConferito.Remove(lmd)
                    Next

                    GiasContext.SaveChanges()

                End Using

                'Procedo con l'inserimento di tutte le nuove righe
                Dim transactionOptions As New TransactionOptions With {
                    .IsolationLevel = IsolationLevel.ReadCommitted,
                    .Timeout = TransactionManager.MaximumTimeout
                }

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)


                    Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                        'Nuove righe
                        For Each lmd As Liquid_Mov_Dati_Generali_CampionamentoConferito In Liquidazione_Mov_Dati_Generali_ToInsert
                            GiasContext.Liquid_Mov_Dati_Generali_CampionamentoConferito.Add(lmd)
                        Next

                        For Each lm As Liquid_Mov_CampionamentoConferito In Liquidazione_Mov_ToInsert
                            GiasContext.Liquid_Mov_CampionamentoConferito.Add(lm)
                        Next

                        For Each lmc As Liquid_Mov_PerCalibro_CampionamentoConferito In Liquidazione_Mov_Calibri_ToInsert
                            GiasContext.Liquid_Mov_PerCalibro_CampionamentoConferito.Add(lmc)
                        Next

                        For Each lmfv As Liquid_Mov_FattVariaz_CampionamentoConferito In Liquidazione_Mov_FattVar_ToInsert
                            GiasContext.Liquid_Mov_FattVariaz_CampionamentoConferito.Add(lmfv)
                        Next


                        GiasContext.SaveChanges()

                        'COMMIT Effettivo
                        scope.Complete()

                    End Using
                End Using

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function


    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        '----- Descrizione
        Dim nomeRoutine As String = "Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Append(" UPDATE ... ")
                strSql.Append(" SET ")
                strSql.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                strSql.Append("         ,Inviato = -1 ")
                strSql.Append(" WHERE   1=1 ")
                strSql.Append(" AND     Inviato >= 0 ")
            Else
                strSql.Append(" DELETE FROM ... ")
                strSql.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function OperazioneDBTrattenute(ByVal piva As String,
                                           ByVal riga As String,
                                           ByVal op As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        Dim pivaSuperUser = objParametri.PivaSuperUser

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_W.OperazioneDBTrattenute()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim objTratt As JObject = JsonConvert.DeserializeObject(riga, settingLoc)

        Try
            Using scope As New TransactionScope()

                Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                    Dim Tratt As New Liquid_Mov_Trattenute_CampionamentoConferito

                    If (op.ToUpper() = "INS") Then

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella_EF(giasContext,
                                                   "Liquid_Mov_Trattenute_CampionamentoConferito", 0, 2000000000, objParametri)

                        Tratt.Piva_SuperUser = pivaSuperUser
                        Tratt.PIVA = piva
                        Tratt.Id_liquidazione_trattenute = idSeq
                        Tratt.Data_Creazione = Date.Now
                        Tratt.Username_Creazione = objParametri.UsernameOperazione
                        Tratt.Validita_Inizio = AGRODATAINIZIO
                        Tratt.Validita_Fine = AGRODATAFINE

                    Else
                        Dim id As Integer = objTratt("Id_liquidazione_trattenute")
                        Tratt = (From trattenute In giasContext.Liquid_Mov_Trattenute_CampionamentoConferito
                                 Where trattenute.PIVA.Equals(piva) _
                                         AndAlso trattenute.Piva_SuperUser.Equals(pivaSuperUser) _
                                         AndAlso trattenute.Id_liquidazione_trattenute = id
                                 Select trattenute).FirstOrDefault()
                    End If

                    If IsNothing(Tratt) Then
                        Throw New Exception("Non c'è nessuno elemento con la chiave indicata.")
                    End If

                    If op.ToUpper() <> "DEL" Then

                        Tratt.Id_acconto_liquidazione = objTratt("id_liquidazione")
                        Tratt.Cod_RisUm = objTratt("Cod_RisUm")
                        Tratt.Doc_Numero_Sin = objTratt("Doc_Numero_Sin")
                        Tratt.Doc_Numero = objTratt("Doc_Numero")
                        Tratt.Doc_Numero_Des = objTratt("Doc_Numero_Des")
                        Tratt.Data_Movimento = objTratt("Data_Movimento")
                        Tratt.Riferimento = objTratt("Riferimento")
                        Tratt.Imponibile = objTratt("Imponibile")
                        Tratt.Cod_Iva = objTratt("Cod_IVA")
                        Tratt.Iva = objTratt("IVA")
                        Tratt.Detrarre_Da_Fattura = objTratt("Detrarre_Da_Fattura")

                        Tratt.Data_Modifica = Date.Now
                        Tratt.Username_Modifica = objParametri.UsernameOperazione

                        If op.ToUpper() = "INS" Then
                            giasContext.Liquid_Mov_Trattenute_CampionamentoConferito.Add(Tratt)
                        Else
                            giasContext.Liquid_Mov_Trattenute_CampionamentoConferito.Attach(Tratt)
                            giasContext.Entry(Tratt).State = EntityState.Modified
                        End If
                    Else

                        giasContext.Liquid_Mov_Trattenute_CampionamentoConferito.Attach(Tratt)
                        giasContext.Liquid_Mov_Trattenute_CampionamentoConferito.Remove(Tratt)

                    End If

                    giasContext.SaveChanges()

                    ' COMMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

    Public Function OperazioneDBPercAccGrpFatt(ByVal piva As String,
                                               ByVal riga As String,
                                               ByVal op As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As String

        Dim pivaSuperUser = objParametri.PivaSuperUser

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_LiquidazioneSoci_W.OperazioneDBPercAccGrpFatt()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim objPAGF As JObject = JsonConvert.DeserializeObject(riga, settingLoc)

        Try
            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                Dim PAGF As New AnagPercentAccontiPerGrpFatt_CampionamentoConferito

                If (op.ToUpper() = "INS") Then

                    PAGF.Piva_SuperUser = pivaSuperUser
                    PAGF.PIVA = piva

                    PAGF.inviato = 0
                    PAGF.Data_Creazione = Date.Now
                    PAGF.Username_Creazione = objParametri.UsernameOperazione
                    PAGF.Validita_Inizio = AGRODATAINIZIO
                    PAGF.Validita_Fine = AGRODATAFINE

                Else
                    Dim Id_acconto_liquidazione As Integer = objPAGF("key_acconto_liquidazione")
                    Dim Id_gruppo_fatturazione As Integer = objPAGF("key_gruppo_fatturazione")
                    PAGF = (From lista In giasContext.AnagPercentAccontiPerGrpFatt_CampionamentoConferito
                            Where lista.PIVA.Equals(piva) _
                                AndAlso lista.Piva_SuperUser.Equals(pivaSuperUser) _
                                AndAlso lista.Id_acconto_liquidazione = Id_acconto_liquidazione _
                                AndAlso lista.Id_gruppo_fatturazione = Id_gruppo_fatturazione
                            Select lista).FirstOrDefault()
                End If

                If (IsNothing(PAGF)) Then
                    Throw New Exception("Non c'è nessuno elemento con la chiave indicata.")
                End If

                If op.ToUpper() <> "DEL" Then

                    PAGF.Id_acconto_liquidazione = objPAGF("id_liquidazione")
                    PAGF.Id_gruppo_fatturazione = objPAGF("Tabella_Par_Cod")
                    PAGF.perc_valore_acconto = objPAGF("perc_valore_acconto")

                    PAGF.Data_Modifica = Date.Now
                    PAGF.Username_Modifica = objParametri.UsernameOperazione

                    If op.ToUpper() = "INS" Then
                        giasContext.AnagPercentAccontiPerGrpFatt_CampionamentoConferito.Add(PAGF)
                    Else
                        giasContext.AnagPercentAccontiPerGrpFatt_CampionamentoConferito.Attach(PAGF)
                        giasContext.Entry(PAGF).State = EntityState.Modified
                    End If
                Else

                    giasContext.AnagPercentAccontiPerGrpFatt_CampionamentoConferito.Attach(PAGF)
                    giasContext.AnagPercentAccontiPerGrpFatt_CampionamentoConferito.Remove(PAGF)

                End If

                giasContext.SaveChanges()

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message

            If (TypeOf ex.GetBaseException() Is SqlException) Then
                'Violation of primary key/Unique constraint can be handled here. 

                Dim sqlex As SqlException = ex.GetBaseException()
                If (sqlex.Number = 2627) Then
                    'Inserimento chiave duplicata 

                    messaggioErrore = sqlex.Message

                End If

            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return messaggioErrore

    End Function

End Class

