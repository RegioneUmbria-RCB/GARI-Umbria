Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Imports System.Text
Imports System.Globalization
Imports Newtonsoft.Json
Public Class Budget_Impresa_Progetti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################################

    Public Function Leggi(ByVal Id_Budget As Integer,
                          ByVal Piva As String,
                          ByVal Progetto_Cod As Integer,
                          ByVal Cau_Progetto As String,
                          ByVal Cod_Contratto As Integer,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Reg As Integer,
                          ByVal Veg_Cod As Integer,
                          ByVal Grfi_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal isGias2Gias As Boolean = False,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Progetto_Cod = 0
        '   Piva = ""
        '   Sa_Cod = 0
        '   Cau_Progetto = ""
        '   Appezza = 0
        '   Id_Reg = 0
        '   Veg_Cod = 0
        '   Grfi_Cod = 0
        '   Progetto_Cod = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine("SELECT *")
                    StrSQL.AppendLine("FROM Budget_Imprese_Progetti")
                    StrSQL.AppendLine("WHERE Budget_Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("AND Budget_Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Id_Budget <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If


                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Distinta rr where rr.From_Piva = Budget_Imprese_Progetti.piva and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and ( ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Budget_Imprese_Progetti.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Budget_Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) or ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    inner join Budget_reg_impianti_codici ric ")
                            StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Progetto_Cod = ric.Progetto_Cod ")
                            StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Budget_Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")
                            StrSQL.AppendLine(" ) ")
                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Budget_Imprese_Progetti.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Budget_Imprese_Progetti.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Budget_Imprese_Progetti.*, gr.GruppoRaccolta_Des, YEAR(Budget_Imprese_Progetti.Validita_Inizio) AS 'anno_validita',  convert (varchar(10), Budget_imprese_progetti.Validita_Inizio  ,103 ) as v_i_date, convert (varchar(10), Budget_imprese_progetti.Validita_Fine  ,103 ) as v_f_date  ")
                    StrSQL.AppendLine(" , CASE WHEN Budget_Imprese_Progetti.Stato_Impianto = 102 THEN 'In Produzione' ELSE COALESCE(FasiCicloColturale_Anagrafiche.Fase_Des, '') END as Fase_Des ")
                    StrSQL.AppendLine(" , CAST(COALESCE(grfi_RER.Val_Cod, 0) as int) as Tipologia_Cod  ")
                    StrSQL.AppendLine(" , COALESCE(GruppoFinalita_Rer.GRFI_DES, '') as Tipologia_Des ")
                    StrSQL.AppendLine(" FROM Budget_Imprese_Progetti ")
                    StrSQL.AppendLine("LEFT JOIN Gruppi_Raccolta gr ON gr.GruppoRaccolta_Cod = Budget_Imprese_Progetti.GruppoRaccolta_Cod")
                    StrSQL.AppendLine("LEFT JOIN FasiCicloColturale_Anagrafiche (NOLOCK) ON Budget_Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN Budget_Reg_Impianti_Codici grfi_RER ON Budget_Imprese_Progetti.Progetto_Cod = grfi_RER.Progetto_Cod AND grfi_RER.id_Cod = " & enum_CodiciAnagrafe.Finalita_Concimazione_Impianto & " ")
                    StrSQL.AppendLine(" LEFT JOIN GruppoFinalita_Rer (NOLOCK) ON grfi_RER.Val_Cod = GruppoFinalita_Rer.GRFI_COD ")
                    StrSQL.AppendLine(" WHERE Budget_Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Id_Budget <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If


                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Distinta rr where rr.From_Piva = Budget_Imprese_Progetti.piva and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and ( ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Budget_Imprese_Progetti.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Budget_Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) or ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    inner join Budget_reg_impianti_codici ric ")
                            StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Progetto_Cod = ric.Progetto_Cod ")
                            StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Budget_Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")
                            StrSQL.AppendLine(" ) ")
                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Budget_Imprese_Progetti.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Budget_Imprese_Progetti.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY anno_validita DESC, Budget_Imprese_Progetti.Id_Budget, Budget_Imprese_Progetti.Piva, Budget_Imprese_Progetti.Sa_cod, Budget_Imprese_Progetti.Appezza, Budget_Imprese_Progetti.Id_Reg, Budget_Imprese_Progetti.Progetto_cod ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Budget_Imprese_Progetti.*, Budget_Reg_Impianti.Sup_Imp, Budget_Reg_Impianti.Cul_Cod, ISNULL(Budget_Reg_Impianti.Grfi_Cod,0) as Grfi_Cod_Impianto, Budget_Appezzamento.Appezza, Budget_Campi.Campo_Cod,  ")
                    StrSQL.AppendLine(" Budget_Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Budget_Reg_Impianti.Validita_Fine AS Fine_Impianto  ")
                    StrSQL.AppendLine(" FROM Budget_Imprese_Progetti, Budget_Reg_Impianti, Budget_Appezzamento ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Budget_Campi ON ")
                    StrSQL.AppendLine(" (Budget_Appezzamento.Id_Budget = Budget_Campi.Id_Budget and Budget_Appezzamento.Piva = Budget_Campi.Piva and Budget_Appezzamento.Sa_Cod = Budget_Campi.Sa_Cod ")
                    StrSQL.AppendLine(" And Budget_Appezzamento.Campo_Cod = Budget_Campi.Campo_Cod) ")

                    StrSQL.AppendLine(" WHERE Budget_Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti.Id_Budget")
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti.Piva")
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti.Sa_Cod ")
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti.Appezza ")
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti.Id_Reg ")
                    StrSQL.AppendLine(" AND   Budget_Appezzamento.Id_Budget = Budget_Reg_Impianti.Id_Budget ")
                    StrSQL.AppendLine(" AND   Budget_Appezzamento.Piva = Budget_Reg_Impianti.Piva ")
                    StrSQL.AppendLine(" AND   Budget_Appezzamento.Sa_Cod = Budget_Reg_Impianti.Sa_Cod ")
                    StrSQL.AppendLine(" AND   Budget_Appezzamento.Appezza = Budget_Reg_Impianti.Appezza ")

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Id_Budget <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Distinta rr where rr.From_Piva = Budget_Imprese_Progetti.piva and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and ( ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Budget_Imprese_Progetti.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Budget_Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) or ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    inner join Budget_reg_impianti_codici ric ")
                            StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Progetto_Cod = ric.Progetto_Cod ")
                            StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Budget_Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")
                            StrSQL.AppendLine(" ) ")
                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Budget_Imprese_Progetti.Inviato >= 0 ")
                            If Not isGias2Gias Then
                                StrSQL.AppendLine(" AND     Budget_Reg_Impianti.Inviato >= 0 ")
                                StrSQL.AppendLine(" AND     Budget_Appezzamento.Inviato >= 0 ")
                            End If

                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Budget_Imprese_Progetti.Inviato = -1 ")
                            If Not isGias2Gias Then
                                StrSQL.AppendLine(" AND     Budget_Reg_Impianti.Inviato = -1 ")
                                StrSQL.AppendLine(" AND     Budget_Appezzamento.Inviato = -1 ")
                            End If

                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Budget_Imprese_Progetti.Id_Budget ASC, Budget_Imprese_Progetti.Piva ASC, Budget_Imprese_Progetti.Progetto_Cod ASC, Budget_Imprese_Progetti.Progetto_Nome ASC ")
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

    '#############################################################################################################

    Public Function LeggiDistinta3(ByVal Id_Budget As Integer,
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Long,
                            ByVal Appezza As Long,
                            ByVal Id_Reg As Long,
                            ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R.LeggiDistinta3()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  Budget_Imprese_Progetti.Id_Budget, Budget_Imprese_Progetti.Piva, Budget_Imprese_Progetti.Progetto_Cod, Progetto_Nome, Progetto_Des, Cau_Progetto, Budget_Imprese_Progetti.Sa_Cod, Budget_Imprese_Progetti.Appezza, Budget_Imprese_Progetti.Id_Reg, Budget_Imprese_Progetti.Validita_Inizio, Budget_Imprese_Progetti.Validita_Fine,  ")
            StrSQL.AppendLine(" Budget_Imprese_Progetti.Regolamento_Cod, Budget_Imprese_Progetti.Disciplinare_Cod, Disciplinare_PubblicoPrivato, Regolamento_Concimazioni_Cod, Stato_Impianto, ")
            StrSQL.AppendLine(" Budget_Imprese_Progetti.P_HA, Produzione_Prevista, Data_Fine_Prevista, Data_Inizio_Prevista,  Data_Fioritura_Prevista, ")
            StrSQL.AppendLine(" Budget_Appezzamento.App_Nome, Budget_Appezzamento.Sup_app, Budget_Appezzamento.Validita_Inizio AS Inizio_Appezza, Budget_Appezzamento.Validita_Fine AS Fine_Appezza, ")
            StrSQL.AppendLine(" Budget_Reg_Impianti.Sup_Imp, Budget_Reg_Impianti.Cul_Cod, Budget_Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Budget_Reg_Impianti.Validita_Fine AS Fine_Impianto,  ")
            StrSQL.AppendLine(" Cultivar.Cul_Cod, Cultivar.Cul_Des, SpecieVegetali.veg_cod, SpecieVegetali.veg_des, ")
            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            StrSQL.AppendLine(" Budget_Imprese_Progetti.P_HA_Femmine, Budget_Imprese_Progetti.P_HA_Maschi")
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Budget_Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Budget_Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato) & " ")
            'Aggiunta Id_Budget
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti_Codici.Id_Budget  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Progetto_Cod = Budget_Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), '') AS capitolato_Privato ")

            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Budget_Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Budget_Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) & " ")
            'Aggiunta Id_Budget
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti_Codici.Id_Budget  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Progetto_Cod = Budget_Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), '') AS Magazzino_Conferimento ")

            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Budget_Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Budget_Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteN) & " ")
            'Aggiunta Id_Budget
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti_Codici.Id_Budget  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Progetto_Cod = Budget_Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_N ")
            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Budget_Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Budget_Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteP) & " ")
            'Aggiunta Id_Budget
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti_Codici.Id_Budget  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Progetto_Cod = Budget_Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_P ")
            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Budget_Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Budget_Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteK) & " ")
            'Aggiunta Id_Budget
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti_Codici.Id_Budget  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Progetto_Cod = Budget_Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_K ")
            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Budget_Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Budget_Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteMg) & " ")
            'Aggiunta Id_Budget
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti_Codici.Id_Budget  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Budget_Imprese_Progetti.Progetto_Cod = Budget_Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_MG ")

            StrSQL.AppendLine(" FROM  Budget_Imprese_Progetti ")
            StrSQL.AppendLine(" INNER JOIN Budget_Reg_Impianti ON Budget_Imprese_Progetti.Id_Budget = Budget_Reg_Impianti.Id_Budget AND Budget_Imprese_Progetti.Piva = Budget_Reg_Impianti.Piva AND Budget_Imprese_Progetti.Sa_Cod = Budget_Reg_Impianti.Sa_Cod AND Budget_Imprese_Progetti.Appezza = Budget_Reg_Impianti.Appezza AND Budget_Imprese_Progetti.Id_Reg = Budget_Reg_Impianti.Id_Reg ")
            StrSQL.AppendLine(" INNER JOIN Budget_Appezzamento ON Budget_Reg_Impianti.Id_Budget = Budget_Appezzamento.Id_Budget And Budget_Reg_Impianti.Piva = Budget_Appezzamento.Piva And Budget_Reg_Impianti.Sa_Cod = Budget_Appezzamento.Sa_Cod And Budget_Reg_Impianti.Appezza = Budget_Appezzamento.Appezza ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Cultivar.Cul_Cod = Budget_Reg_Impianti.Cul_Cod ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine(" WHERE Budget_Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(CAU_PROGETTO_PRODUZIONE) & "'   ")

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "   ")
            End If

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Budget_Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Budget_Imprese_Progetti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Budget_Imprese_Progetti.Validita_fine DESC ")
            End If
            '---------------------------------------------

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

    '#############################################################################################################

    Public Function Leggi_x_anagraficaNG(ByVal Id_Budget As Integer,
                       ByVal Piva As String,
                       ByVal Sa_Cod As Long,
                       ByVal Appezza As Long,
                       ByVal Id_Reg As Long,
                       ByVal Campo_Cod As Long,
                       ByVal xFiltroAggiuntivo As String,
                       ByVal xOrderBy As String,
                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                       Optional dataAtt As Date = Nothing,
                       Optional leggiStaticMap As Boolean = False,
                       Optional leggiDatiRibaltamento As Boolean = False,
                       Optional leggiDatiPrenotazionePiante As Boolean = False
               ) As DataTable

        Dim NomeRoutine As String = "BudgetDAL.Budget_Impresa_Progetti_R.Leggi_x_anagrafica()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si leggono tutti gli impianti dell'impresa
        '   Appezza = 0          =>  si leggono tutti gli impianti del centro aziendale
        '   Id_reg = 0           =>  si leggono tutti gli impianti dell'appezzamento
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer
        Dim sql2017 As Boolean = True
        Dim major As Integer = VersioneSqlServer_Major(objParametri)

        If major >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017 = True
        Else
            sql2017 = False
        End If

        If IsNothing(dataAtt) Then
            dataAtt = Date.Now
        End If

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If


            StrSQL.Length = 0
            StrSQL.AppendLine("WITH #AppezzamentiSingoli as (  ")
            StrSQL.AppendLine("    SELECT Id_Budget, Piva, Sa_Cod, Appezza ")
            StrSQL.AppendLine("    FROM Budget_AppezzamentiXParticelle ")
            StrSQL.AppendLine("    WHERE 1 = 1 ")

            If Id_Budget <> 0 Then
                StrSQL.AppendLine("        AND Budget_AppezzamentiXParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine("        AND Budget_AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("        AND Budget_AppezzamentiXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine("        AND Budget_AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                    End If
                End If
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine("        AND Budget_AppezzamentiXParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            StrSQL.AppendLine("     GROUP BY Id_Budget, Piva, Sa_Cod, Appezza")
            StrSQL.AppendLine("     HAVING COUNT(*) = 1")
            StrSQL.AppendLine(")")


            StrSQL.AppendLine(", #AppezzamentiSingoliCompleti AS (")
            StrSQL.AppendLine("    SELECT #AppezzamentiSingoli.Piva")
            StrSQL.AppendLine("        , #AppezzamentiSingoli.Id_Budget")
            StrSQL.AppendLine("        , #AppezzamentiSingoli.SA_COD")
            StrSQL.AppendLine("        , #AppezzamentiSingoli.APPEZZA")
            StrSQL.AppendLine("        , Budget_AppezzamentiXParticelle.PROV")
            StrSQL.AppendLine("        , Lista_Province.PROVINCIA")
            StrSQL.AppendLine("        , Budget_AppezzamentiXParticelle.Com")
            StrSQL.AppendLine("        , ISTAT.Localita")
            StrSQL.AppendLine("        , Case Budget_AppezzamentiXParticelle.SEZIONE When '0' THEN '' ELSE Budget_AppezzamentiXParticelle.SEZIONE END AS 'SEZIONE'")
            StrSQL.AppendLine("        , Budget_AppezzamentiXParticelle.FOGLIO")
            StrSQL.AppendLine("        , Budget_AppezzamentiXParticelle.NUMERO")
            StrSQL.AppendLine("        , CASE Budget_AppezzamentiXParticelle.SUBALTERNO WHEN '0' THEN '' ELSE Budget_AppezzamentiXParticelle.SUBALTERNO END AS 'SUBALTERNO'")
            StrSQL.AppendLine("    FROM #AppezzamentiSingoli ")
            StrSQL.AppendLine("        JOIN Budget_AppezzamentiXParticelle (nolock) ON #AppezzamentiSingoli.Id_Budget = Budget_AppezzamentiXParticelle.Id_Budget AND #AppezzamentiSingoli.Piva = Budget_AppezzamentiXParticelle.Piva AND #AppezzamentiSingoli.SA_COD = Budget_AppezzamentiXParticelle.SA_COD AND #AppezzamentiSingoli.APPEZZA = Budget_AppezzamentiXParticelle.APPEZZA")
            StrSQL.AppendLine("        JOIN Lista_Province (nolock) ON Budget_AppezzamentiXParticelle.PROV = Lista_Province.PROV")
            StrSQL.AppendLine("        JOIN ISTAT (nolock) ON Budget_AppezzamentiXParticelle.PROV = ISTAT.PROV AND Budget_AppezzamentiXParticelle.COM = ISTAT.COM")
            StrSQL.AppendLine(")")


            'VERIFICO CHE CI SIA ALMENO SQL 2017 PER UTILIZZARE STRING_AGG
            If sql2017 Then

                StrSQL.AppendLine(", #AppezzaZVN AS (")
                StrSQL.AppendLine("    SELECT Id_Budget, Piva, Sa_Cod, Appezza, STRING_AGG(ZVN, ',') as ZVN FROM (")
                StrSQL.AppendLine("        SELECT DISTINCT ap.Id_Budget, ap.Piva, ap.sa_Cod, ap.Appezza, CASE WHEN zp.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN")
                StrSQL.AppendLine("        FROM Budget_AppezzamentiXParticelle ap")
                StrSQL.AppendLine("            LEFT JOIN ZonexParticelle zp ON ap.PROV = zp.PROV")
                StrSQL.AppendLine("                AND ap.COM = zp.COM")
                StrSQL.AppendLine("                AND ap.SEZIONE = zp.SEZIONE")
                StrSQL.AppendLine("                AND ap.FOGLIO = zp.FOGLIO")
                StrSQL.AppendLine("                AND ap.NUMERO = zp.NUMERO")
                StrSQL.AppendLine("                AND ap.SUBALTERNO = zp.SUBALTERNO")
                StrSQL.AppendLine("                AND zp.Zona_Cod = -17")

                StrSQL.AppendLine("        WHERE 1 = 1")

                If Id_Budget <> 0 Then
                    StrSQL.AppendLine("        AND ap.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
                End If

                If Piva <> "" Then
                    StrSQL.AppendLine("        AND ap.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                End If
                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine("        AND ap.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                Else
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                        Next
                        If FiltroCentri <> "" Then
                            StrSQL.AppendLine("    AND ap.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                        End If
                    End If
                End If

                If Appezza <> 0 Then
                    StrSQL.AppendLine("    AND ap.Appezza = " & Agro_SQL_SaveNum(Appezza))
                End If

                StrSQL.AppendLine("    ) a")
                StrSQL.AppendLine("    GROUP BY a.Id_Budget, a.Piva, a.SA_COD, a.APPEZZA")
                StrSQL.AppendLine(")")

            End If

            StrSQL.AppendLine(", #Reg_Impianti_Codici as (")
            StrSQL.AppendLine("    SELECT *")
            StrSQL.AppendLine("    FROM Budget_Reg_Impianti_Codici (nolock)")
            StrSQL.AppendLine("    WHERE 1 = 1")
            If Id_Budget <> 0 Then
                StrSQL.AppendLine("    AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
            End If
            If Piva <> "" Then
                StrSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine("    AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                    End If
                End If
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine("    AND Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If
            StrSQL.AppendLine(")")
            StrSQL.AppendLine(", #Appezzamento_Codici as (")
            StrSQL.AppendLine("    SELECT *")
            StrSQL.AppendLine("    FROM Budget_Appezzamento_Codici (nolock)")
            StrSQL.AppendLine("    WHERE 1 = 1")
            If Id_Budget <> 0 Then
                StrSQL.AppendLine("    AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
            End If
            If Piva <> "" Then
                StrSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine("    AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                    End If
                End If
            End If
            If Appezza <> 0 Then
                StrSQL.AppendLine("    AND Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If
            StrSQL.AppendLine(")")
            StrSQL.AppendLine(", #Campi as (")
            StrSQL.AppendLine("    SELECT *")
            StrSQL.AppendLine("    FROM Budget_Campi (nolock)")
            StrSQL.AppendLine("    WHERE 1 = 1")
            If Id_Budget <> 0 Then
                StrSQL.AppendLine("    AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
            End If
            If Piva <> "" Then
                StrSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine("    AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                    End If
                End If
            End If
            StrSQL.AppendLine(")")


            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    CAST(r.Id_Budget AS nvarchar(10)) + '_' + r.PIVA + '_' + CAST(r.SA_COD AS nvarchar(10)) + '_' + CAST(r.APPEZZA AS nvarchar(10)) + '_' + CAST(r.id_reg AS nvarchar(10)) + '_' +  CAST(Budget_Imprese_Progetti.Progetto_Cod AS nvarchar(10)) AS chiave")
            StrSQL.AppendLine("    , r.Id_Budget, r.PIVA, r.SA_COD, Centri_Aziendali.sa_nome, r.APPEZZA, r.id_Reg")
            StrSQL.AppendLine("    , a.app_nome")
            StrSQL.AppendLine("    , ISNULL(c.cul_des, '') as cul_des, ISNULL(r.cul_cod, 0) as cul_cod, ISNULL(s.veg_des, '') as veg_des, ISNULL(s.veg_cod, 0) as veg_cod, ISNULL(gru_des, '') as gru_des, ISNULL(r.GRFI_COD, 0) as GRFI_COD, ISNULL(gf.Grfi_Des, '') as Grfi_Des, ISNULL(r.GRVA_Cod_VEG, '') as GRVA_Cod_VEG, ISNULL(grva_des, '') as grva_des, r.p_ha, r.sup_imp")
            StrSQL.AppendLine("    , COALESCE(dettSpecie.val_cod, '') AS dettSpeciePersonalizzatoCod")
            StrSQL.AppendLine("    , COALESCE(cac.InfoAgg_Des, '') AS dettSpeciePersonalizzatoDes")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Validita_Inizio, Budget_Imprese_Progetti.Validita_Fine, (select [User] from utenti where CODICE_FISCALE = r.Username_Modifica ) as utente_modifica , r.Data_Modifica")
            StrSQL.AppendLine("    , (select [User] from utenti where CODICE_FISCALE = r.Username_Creazione ) as utente_creazione , r.Data_Creazione")
            StrSQL.AppendLine("    , Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome")
            StrSQL.AppendLine("    , ISNULL( #Campi.Campo_Cod, 0) As Campo_Cod, ISNULL( #Campi.Campo_Des, '') As Campo_Des")
            StrSQL.AppendLine("    , codiceImpianto.val_cod as Codice_Impianto")
            StrSQL.AppendLine("    , CAST(REPLACE(CASE WHEN tra_fila_m.val_cod is null THEN '0' ELSE CASE WHEN tra_fila_m.val_cod = ''  THEN '0' ELSE tra_fila_m.val_cod END END, ',', '.') as float) as tra_fila_m")
            StrSQL.AppendLine("    , CAST(REPLACE(CASE WHEN  su_fila_m.val_cod is null THEN '0' ELSE CASE WHEN  su_fila_m.val_cod = ''  THEN '0' ELSE  su_fila_m.val_cod END END, ',', '.') as float) as  su_fila_m")
            StrSQL.AppendLine("    , ISNULL(r.port_cod, 0) as port_cod, ISNULL(Portinnesti.port_des, '') as port_des")
            StrSQL.AppendLine("    , ISNULL(r.foral_cod, 0) as foral_cod, ISNULL(FormeAllevamento.foral_des, '') as foral_des")
            StrSQL.AppendLine("    , r.Setup_Cod")
            StrSQL.AppendLine("    , ISNULL(r.cop_cod, 0) as cop_cod, ISNULL(Copertura.Cop_Des, '') as Cop_Des")
            StrSQL.AppendLine("    , r.COVER, r.MONITORATO")
            StrSQL.AppendLine("    , a.Blk_Flag")
            StrSQL.AppendLine("    , r.Data_Inizio_Portinnesto")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Progetto_Cod")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Progetto_Nome")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Progetto_Des")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Produzione_Prevista as [Resa]")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Data_Inizio_Prevista as [DataTrapiantoSemina]")
            StrSQL.AppendLine("    , datepart(WEEK, Budget_Imprese_Progetti.Data_Inizio_Prevista) as [SettimanaTrapiantoSemina]")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Data_Fine_Prevista as [DataRaccolta]")
            StrSQL.AppendLine("    , datepart(WEEK, Budget_Imprese_Progetti.Data_Fine_Prevista) as [SettimanaRaccolta]")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Data_Fioritura_Prevista as [DataFioritura]")
            StrSQL.AppendLine("    , SupBZ_Riduzione, DistBZ_CorpiIdrici, DistBZ_AreeResPub, DistBZ_Allevamenti, DistBZ_VegNatNonColt")
            StrSQL.AppendLine("    , ISNULL(kpin.val_cod, '') AS cod_kpin")
            StrSQL.AppendLine("    , ISNULL(blockName.val_cod, '') AS cod_block")
            StrSQL.AppendLine("    , ISNULL(grower.val_cod, '') AS cod_grower")
            StrSQL.AppendLine("    , CASE distintaChiusa.val_cod WHEN '1' THEN 'SI' ELSE 'NO' END AS [Distinta_Chiusa]")
            StrSQL.AppendLine("    , CASE r.cul_Cod WHEN 0 THEN  ISNULL(Codici_Anagrafe.descrizione, 'Terreno Nudo') ELSE s.Veg_Des + ' - ' + c.Cul_Des END AS [Utilizzo]")
            StrSQL.AppendLine("    , CASE r.cul_Cod WHEN 0 THEN  COALESCE(Codici_Anagrafe.descrizione, 'Terreno Nudo') ELSE '' END AS Destinazione_Uso_Des")
            StrSQL.AppendLine("    , CASE r.cul_Cod WHEN 0 THEN  COALESCE(Codici_Anagrafe.Codice, 0) ELSE 0 END AS Destinazione_Uso_Cod")
            StrSQL.AppendLine("    , CASE WHEN Budget_Imprese_Progetti.Validita_Inizio < GETDATE() AND Budget_Imprese_Progetti.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo")
            StrSQL.AppendLine("    , CASE WHEN a.Blk_Flag = -1 THEN 1 ELSE 0 END as Bloccato")

            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.PROV")
            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.PROVINCIA")
            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.Com")
            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.Localita")
            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.SEZIONE")
            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.FOGLIO")
            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.NUMERO")
            StrSQL.AppendLine("    , #AppezzamentiSingoliCompleti.SUBALTERNO")



            If leggiStaticMap Then
                StrSQL.AppendLine(", (")
                StrSQL.AppendLine("    Select * from(select StaticMap as '*') Tbl")
                StrSQL.AppendLine("    For Xml path('')")
                StrSQL.AppendLine(") StaticMapBase64String")
                StrSQL.AppendLine("")
            End If

            If sql2017 Then
                StrSQL.AppendLine("    , #AppezzaZVN.ZVN")
            Else
                StrSQL.AppendLine("     , '' as ZVN")
            End If

            'metodoProd
            StrSQL.AppendLine("     , CAST(ISNULL(MetodoProd.Val_Cod, 1) as int) as Metodo_Produzione_Cod")

            StrSQL.AppendLine("    , CASE WHEN MetodoProd.Val_Cod = '1' THEN 'Integrato'")
            StrSQL.AppendLine("        WHEN MetodoProd.Val_Cod = '2' THEN 'In Conversione'")
            StrSQL.AppendLine("        WHEN MetodoProd.Val_Cod = '3' THEN 'Biologico'")
            StrSQL.AppendLine("        WHEN MetodoProd.Val_Cod is null THEN 'Integrato'")
            StrSQL.AppendLine("    END as Metodo_Produzione_Des")

            'Budget_Reg_Impianti = r
            'Budget_Appezzamento = a
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.Regolamento_Cod")
            StrSQL.AppendLine("    , Regolamenti.Reg_Des")

            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(N.Val_Cod, 0), ',', '.') as float) AS N")
            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(P.Val_Cod, 0), ',', '.') as float) AS P")
            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(K.Val_Cod, 0), ',', '.') as float) AS K")
            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(Mg.Val_Cod, 0), ',', '.') as float) AS Mg")

            StrSQL.AppendLine("    , COALESCE(interbina.val_Cod, '') as interbina")
            StrSQL.AppendLine("    , COALESCE(germinabilita.val_Cod, '100') as germinabilita")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.P_HA as pianteHa")
            StrSQL.AppendLine("    , Budget_Imprese_Progetti.P_HA * r.Sup_Imp as pianteImpianto")

            StrSQL.AppendLine("    , CASE WHEN DPI_Regolamenti.COD_REGOLAMENTO IS NULL THEN CAST(Budget_Imprese_Progetti.Regolamento_Cod as varchar(100)) ELSE CONCAT(DPI_Regolamenti.Flag_Privato_Pubblico, '_', DPI_Regolamenti.COD_REGOLAMENTO) END as RegolamentoDisciplinare_Cod")
            StrSQL.AppendLine("    , CASE WHEN DPI_Regolamenti.NomeEsteso IS NULL THEN CASE WHEN Regolamenti.Reg_Cod = '4' THEN 'Bio' ELSE Regolamenti.Reg_Des END ELSE DPI_Regolamenti.NomeEsteso END as RegolamentoDisciplinare_Des")

            StrSQL.AppendLine("    , CAST(COALESCE(grfi_RER.Val_Cod, 0) as int) as Tipologia_Cod")
            StrSQL.AppendLine("    , COALESCE(GruppoFinalita_Rer.GRFI_DES, '') as Tipologia_Des")
            StrSQL.AppendLine("    , COALESCE(Budget_Imprese_Progetti.Stato_Impianto, 0) as Stato_Impianto")
            StrSQL.AppendLine("    , COALESCE(FasiCicloColturale_Anagrafiche.fase_des, CASE WHEN Budget_Imprese_Progetti.Stato_Impianto = 102 THEN 'In produzione' ELSE '' END) as fase_des")
            StrSQL.AppendLine("    , COALESCE(Budget_Imprese_Progetti.Mat_Cod, 0) AS Mat_Cod")
            StrSQL.AppendLine("    , COALESCE(Materie_Prime.Mat_Des, '') AS Mat_Des")

            StrSQL.AppendLine("    , r.UDM_COD_ALT AS Udm_Cod_Alt")
            StrSQL.AppendLine("    , r.SUP_ALT AS Sup_Int_Alt")
            StrSQL.AppendLine("    , COALESCE(udmAlternative.UDM_DES, '') AS Udm_Des_Alt")
            StrSQL.AppendLine("    , COALESCE(Conversione_UnitaMisura_Alternative.Tasso_Conv, 0) AS Tasso_Conv")
            StrSQL.AppendLine("    , gr.GruppoRaccolta_Cod")
            StrSQL.AppendLine("    , gr.GruppoRaccolta_Des")

            StrSQL.AppendLine("    , r.UDM_COD_ALT AS Udm_Cod_Alt")
            StrSQL.AppendLine("    , r.SUP_ALT AS Sup_Int_Alt")
            StrSQL.AppendLine("    , udmAlternative.UDM_DES AS Udm_Des_Alt")

            If leggiDatiRibaltamento Then
                StrSQL.AppendLine("    , CASE WHEN ISNULL(rib.ID, -1) > 0")
                StrSQL.AppendLine("        THEN 'true'")
                StrSQL.AppendLine("        ELSE 'false'")
                StrSQL.AppendLine("    END AS Ribaltato")
                StrSQL.AppendLine("")
                StrSQL.AppendLine("    , CASE WHEN ISNULL(rib.ID, -1) > 0")
                StrSQL.AppendLine("        THEN rib.Data_Ribaltamento")
                StrSQL.AppendLine("        ELSE ''")
                StrSQL.AppendLine("    END AS Data_Ribaltamento ")
            Else
                StrSQL.AppendLine("    , 0 AS Ribaltato")
                StrSQL.AppendLine("    , '' AS Data_Ribaltamento")
            End If

            If leggiDatiPrenotazionePiante Then
                StrSQL.AppendLine("    , (SELECT COUNT(*)")
                StrSQL.AppendLine("        FROM Programmazione_Entita pe ")
                StrSQL.AppendLine("            JOIN Programmazione_Testata pt ON pe.Programmazione_Cod = pt.Programmazione_Cod")
                StrSQL.AppendLine("        WHERE pt.Tipo_Pianificazione = 12")
                StrSQL.AppendLine("            AND pt.Stato <> 16")
                StrSQL.AppendLine("            AND pe.Budget_Piva = Budget_Imprese_Progetti.Piva")
                StrSQL.AppendLine("            AND pe.Budget_Sa_Cod = Budget_Imprese_Progetti.Sa_Cod")
                StrSQL.AppendLine("            AND pe.Budget_Appezza = Budget_Imprese_Progetti.Appezza")
                StrSQL.AppendLine("            AND pe.Budget_Id_Reg = Budget_Imprese_Progetti.Id_Reg")
                StrSQL.AppendLine("            AND pe.Id_Budget =  Budget_Imprese_Progetti.Id_Budget")
                StrSQL.AppendLine("    ) as OrdiniCollegati")
            End If

            StrSQL.AppendLine("FROM Budget_Imprese_Progetti")
            StrSQL.AppendLine("    LEFT JOIN Gruppi_Raccolta gr ON gr.GruppoRaccolta_Cod = Budget_Imprese_Progetti.GruppoRaccolta_Cod")
            StrSQL.AppendLine("    INNER JOIN Budget_Reg_Impianti r ON Budget_Imprese_Progetti.Id_Budget = r.Id_Budget AND Budget_Imprese_Progetti.Piva = r.PIVA AND Budget_Imprese_Progetti.Sa_Cod = r.SA_COD AND Budget_Imprese_Progetti.Appezza = r.APPEZZA AND  Budget_Imprese_Progetti.id_reg = r.id_reg")
            StrSQL.AppendLine("    INNER JOIN Budget_Appezzamento a on a.Id_Budget = r.Id_Budget and a.piva  = r.piva and a.sa_cod = r.SA_COD and a.APPEZZA = r.APPEZZA")
            StrSQL.AppendLine("    INNER JOIN Centri_Aziendali ON a.Piva = Centri_Aziendali.Piva AND a.Sa_Cod = Centri_Aziendali.Sa_Cod")
            StrSQL.AppendLine("    LEFT JOIN DPI_Regolamenti ON Budget_Imprese_Progetti.Disciplinare_Cod = DPI_Regolamenti.COD_REGOLAMENTO AND Budget_Imprese_Progetti.Disciplinare_PubblicoPrivato = DPI_Regolamenti.Flag_Privato_Pubblico")
            StrSQL.AppendLine("    LEFT JOIN cultivar c on c.Cul_Cod = r.CUL_COD")
            StrSQL.AppendLine("    LEFT JOIN SpecieVegetali s on s.Veg_Cod = c.veg_cod")
            StrSQL.AppendLine("    LEFT JOIN GruppoVegetale g on g.gru_cod = s.Gru_Cod")
            StrSQL.AppendLine("    LEFT JOIN GruppoFinalita gf on gf.Grfi_Cod = r.GRFI_COD")
            StrSQL.AppendLine("    LEFT JOIN GruppoVarietale gv on gv.Grva_Cod = r.GRVA_Cod_VEG")
            StrSQL.AppendLine("    LEFT JOIN Centri_Aziendali ca on r.sa_cod = ca.sa_cod AND r.piva = ca.piva")
            StrSQL.AppendLine("    LEFT JOIN #Campi on a.Id_Budget = #Campi.Id_Budget AND a.sa_cod = #Campi.sa_cod AND a.piva = #Campi.piva AND a.Campo_Cod = #Campi.Campo_Cod")
            StrSQL.AppendLine("    LEFT JOIN Portinnesti on r.port_cod = Portinnesti.port_cod")
            StrSQL.AppendLine("    LEFT JOIN FormeAllevamento on r.foral_cod = FormeAllevamento.foral_cod")
            StrSQL.AppendLine("    LEFT JOIN Copertura on r.Cop_Cod = Copertura.Cop_Cod")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici codiceImpianto ON r.Id_Budget = codiceImpianto.Id_Budget AND r.PIVA = codiceImpianto.PIVA AND r.Sa_Cod = codiceImpianto.Sa_Cod AND r.Appezza = codiceImpianto.Appezza AND r.ID_Reg = codiceImpianto.ID_Reg AND codiceImpianto.id_Cod = 1300")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici tra_fila_m ON r.Id_Budget = tra_fila_m.Id_Budget AND  r.PIVA = tra_fila_m.PIVA AND r.Sa_Cod = tra_fila_m.Sa_Cod AND r.Appezza = tra_fila_m.Appezza AND r.ID_Reg = tra_fila_m.ID_Reg AND tra_fila_m.id_Cod = 1061")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici su_fila_m ON  r.Id_Budget = su_fila_m.Id_Budget AND  r.PIVA = su_fila_m.PIVA AND r.Sa_Cod = su_fila_m.Sa_Cod AND r.Appezza = su_fila_m.Appezza AND r.ID_Reg = su_fila_m.ID_Reg AND su_fila_m.id_Cod = 1063")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici Data_Inizio_Portinnesto ON  r.Id_Budget = Data_Inizio_Portinnesto.Id_Budget AND  r.PIVA = Data_Inizio_Portinnesto.PIVA AND r.Sa_Cod = Data_Inizio_Portinnesto.Sa_Cod AND r.Appezza = Data_Inizio_Portinnesto.Appezza AND r.ID_Reg = Data_Inizio_Portinnesto.ID_Reg AND Data_Inizio_Portinnesto.id_Cod = 1328")

            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici kpin ON kpin.Id_Budget = Budget_Imprese_Progetti.Id_Budget AND Budget_Imprese_Progetti.Progetto_Cod = kpin.Progetto_Cod AND kpin.id_cod = 1287")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici blockName ON blockName.Id_Budget = Budget_Imprese_Progetti.Id_Budget AND Budget_Imprese_Progetti.Progetto_Cod = blockName.Progetto_Cod AND blockName.id_cod = 1288")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici grower ON grower.Id_Budget = Budget_Imprese_Progetti.Id_Budget AND  Budget_Imprese_Progetti.Progetto_Cod = grower.Progetto_Cod AND grower.id_cod = 1317")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici distintaChiusa ON distintaChiusa.Id_Budget = Budget_Imprese_Progetti.Id_Budget AND  Budget_Imprese_Progetti.Progetto_Cod = distintaChiusa.Progetto_Cod AND distintaChiusa.id_cod = 1301")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici Destinazione ON r.Id_Budget = Destinazione.Id_Budget AND r.PIVA = Destinazione.PIVA AND r.Sa_Cod = Destinazione.Sa_Cod AND r.Appezza = Destinazione.Appezza AND r.ID_Reg = Destinazione.ID_Reg AND Destinazione.id_Cod >= 3000 AND Destinazione.id_cod < 4000")

            StrSQL.AppendLine("    LEFT JOIN Codici_Anagrafe ON Destinazione.id_cod = Codici_Anagrafe.codice")
            StrSQL.AppendLine("    LEFT JOIN #AppezzamentiSingoliCompleti ON a.Id_Budget = #AppezzamentiSingoliCompleti.Id_Budget AND a.PIVA = #AppezzamentiSingoliCompleti.PIVA AND a.SA_COD = #AppezzamentiSingoliCompleti.SA_COD AND a.APPEZZA = #AppezzamentiSingoliCompleti.APPEZZA")

            If leggiStaticMap Then
                StrSQL.AppendLine("    LEFT JOIN gis_entita e ON r.piva = e.piva")
                StrSQL.AppendLine("        And r.sa_cod = e.sa_cod")
                StrSQL.AppendLine("        And r.appezza = e.appezza")
                StrSQL.AppendLine("        And r.ID_REG = e.Id_Imp")
                StrSQL.AppendLine("")
                StrSQL.AppendLine("    LEFT JOIN gis_elementigrafici gis ON e.PivaSuperUser = gis.PivaSuperUser")
                StrSQL.AppendLine("        And e.entita_cod = gis.Entita_Cod")
                StrSQL.AppendLine("        And gis.LayerElementiGrafici_Cod = 19")
            End If

            StrSQL.AppendLine("    LEFT JOIN UnitaMisura_Alternative udmAlternative ON r.UDM_COD_ALT = udmAlternative.UDM_COD_ALT")
            StrSQL.AppendLine("    LEFT JOIN Conversione_UnitaMisura_Alternative (NOLOCK) ON udmAlternative.UDM_COD_ALT = Conversione_UnitaMisura_Alternative.UDM_COD_ALT_TO AND Conversione_UnitaMisura_Alternative.UDM_COD_FROM = 2123")

            If sql2017 Then
                StrSQL.AppendLine("    LEFT JOIN #AppezzaZVN ON a.Id_Budget = #AppezzaZVN.Id_Budget AND a.Piva = #AppezzaZVN.PIVA AND a.sa_cod = #AppezzaZVN.sa_cod AND a.APPEZZA = #AppezzaZVN.APPEZZA")
            End If

            StrSQL.AppendLine("    LEFT JOIN #Appezzamento_Codici metodoProd ON a.Id_Budget = metodoProd.Id_Budget AND a.PIVA = metodoProd.PIVA AND a.SA_COD = metodoProd.sa_cod AND a.APPEZZA = metodoProd.appezza AND metodoProd.id_cod = " & enum_CodiciAnagrafe.MetodoDiProduzione)

            StrSQL.AppendLine("    LEFT JOIN Regolamenti ON Budget_Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod")

            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici N ON r.Id_Budget = N.Id_Budget AND r.Piva = N.Piva AND r.Sa_Cod = N.Sa_Cod AND r.Appezza = N.Appezza AND r.Id_Reg = N.Id_Reg AND N.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteN)
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici P ON r.Id_Budget = P.Id_Budget AND  r.Piva = P.Piva AND r.Sa_Cod = P.Sa_Cod AND r.Appezza = P.Appezza AND r.Id_Reg = P.Id_Reg AND P.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteP)
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici K ON r.Id_Budget = K.Id_Budget AND  r.Piva = K.Piva AND r.Sa_Cod = K.Sa_Cod AND r.Appezza = K.Appezza AND r.Id_Reg = K.Id_Reg AND K.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteK)
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici Mg ON r.Id_Budget = Mg.Id_Budget AND  r.Piva = Mg.Piva AND r.Sa_Cod = Mg.Sa_Cod AND r.Appezza = Mg.Appezza AND r.Id_Reg = Mg.Id_Reg AND Mg.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteMg)
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici interbina ON r.Id_Budget = interbina.Id_Budget AND r.Piva = interbina.Piva AND r.Sa_Cod = interbina.Sa_Cod AND r.Appezza = interbina.Appezza AND r.Id_Reg = interbina.Id_Reg AND interbina.id_Cod = " & enum_CodiciAnagrafe.Impianto_Interbina)
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici germinabilita ON r.Id_Budget = germinabilita.Id_Budget AND r.Piva = germinabilita.Piva AND r.Sa_Cod = germinabilita.Sa_Cod AND r.Appezza = germinabilita.Appezza AND r.Id_Reg = germinabilita.Id_Reg AND germinabilita.id_Cod = " & enum_CodiciAnagrafe.Impianto_Germinabilita)

            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici grfi_RER ON Budget_Imprese_Progetti.Progetto_Cod = grfi_RER.Progetto_Cod AND grfi_RER.id_Cod = " & enum_CodiciAnagrafe.Finalita_Concimazione_Impianto)
            StrSQL.AppendLine("    LEFT JOIN GruppoFinalita_Rer ON grfi_RER.Val_Cod = GruppoFinalita_Rer.GRFI_COD")
            StrSQL.AppendLine("    LEFT JOIN FasiCicloColturale_Anagrafiche ON Budget_Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod")

            StrSQL.AppendLine("    LEFT JOIN Materie_Prime ON Budget_Imprese_Progetti.Mat_Cod = Materie_Prime.Mat_Cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN #Reg_Impianti_Codici dettSpecie ON r.id_reg = dettSpecie.id_reg")
            StrSQL.AppendLine("        AND dettSpecie.id_cod = " & enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato)
            StrSQL.AppendLine("        AND dettSpecie.PIVA = r.PIVA")
            StrSQL.AppendLine("        AND dettSpecie.sa_cod = r.SA_COD")
            StrSQL.AppendLine("        AND dettSpecie.appezza = r.APPEZZA")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN CAC_Codifica_InfoAggiuntive cac ON dettSpecie.val_cod = cac.InfoAgg_Cod AND cac.Argomento_Cod = " & enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.DettaglioSpeciePersonalizzato)
            StrSQL.AppendLine("")

            If leggiDatiRibaltamento Then
                StrSQL.AppendLine("    LEFT JOIN Ribaltamento_Reg_Impianti rib ON r.Id_Budget = rib.Budget_Id_Testata AND r.Piva = rib.Budget_Piva AND r.Sa_Cod = rib.Budget_Sa_Cod AND r.Appezza = rib.Budget_Appezza AND r.Id_Reg = rib.Budget_Id_Reg")
            End If

            StrSQL.AppendLine("WHERE Budget_Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine("    AND Budget_Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine("    AND r.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine("    AND r.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            StrSQL.AppendLine("    AND r.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("    AND r.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine("    AND r.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                    End If
                End If
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine("    AND r.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine("    AND r.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine("    AND #Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("    AND R.Inviato >= 0")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("    AND R.Inviato = -1")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'Nota: Questo ordinamento è importante per la gestione del campo.
                'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo

                StrSQL.AppendLine("ORDER BY R.Validita_Inizio Desc")
            End If

            '


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

    '#############################################################################################################

    Public Function Leggi_Max_DataModifica(ByVal Id_Budget As Integer,
                             ByVal Piva As String,
                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DateTime

        Dim NomeRoutine As String = "BudgetDAL.Budget_Impresa_Progetti_R.Leggi_Max_DataModifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Data_Modifica = AGRODATAINIZIO
        Try

            If Piva = "" Then
                Throw New Exception("Piva obbligatoria")
            End If

            If Id_Budget = 0 Then
                Throw New Exception("Id_Budget obbligatorio")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT MAX(Data_Modifica) ")
            StrSQL.AppendLine(" FROM Budget_Imprese_Progetti ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso
                DT.Rows.Count > 0 AndAlso
                IsDate(DT.Rows(0)(0)) Then
                Data_Modifica = DT.Rows(0)(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Data_Modifica = AGRODATAINIZIO
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Data_Modifica

    End Function

End Class
Public Class Budget_Impresa_Progetti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal idBudget As Integer,
                           ByVal Piva As String,
                           ByVal Progetto_Cod As Integer,
                           ByVal Progetto_Nome As String,
                           ByVal Progetto_Des As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Reg As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal Grfi_Cod As Integer,
                           ByVal Cau_Progetto As String,
                           ByVal Cod_Conto As Integer,
                           ByVal Cod_Contratto As Integer,
                           ByVal CSProgetto_Cod As Integer,
                           ByVal Giudizio As String,
                           ByVal Data_Inizio_Prevista As Date,
                           ByVal Data_Fioritura_Prevista As Date,
                           ByVal Data_Fine_Prevista As Date,
                           ByVal Ricavi_Previsti As Decimal,
                           ByVal Produzione_Prevista As Decimal,
                           ByVal Stato_Impianto As Integer,
                           ByVal Regolamento_Cod As Integer,
                           ByVal Disciplinare_Cod As Integer,
                           ByVal Disciplinare_PubblicoPrivato As Integer,
                           ByVal Regolamento_Concimazioni_Cod As Integer,
                           ByVal P_Ha As Decimal,
                           ByVal P_HA_Femmine As Decimal,        '- - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - - - -
                           ByVal P_HA_Maschi As Decimal, '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Sup_Prog As Decimal = 0,
                           Optional ByVal FlagSecondoRaccolto As Integer = 0
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_creazione = #2/1/1900# Then
                Data_creazione = Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Budget_Imprese_Progetti ")
            StrSQL.AppendLine("                    ( Id_Budget, Piva,               Progetto_Cod,         Progetto_Des, ")
            StrSQL.AppendLine("                      Progetto_Nome,      Cau_Progetto,         Cod_Conto,  ")
            StrSQL.AppendLine("                      Giudizio,           Sa_Cod,               Appezza,   ")
            StrSQL.AppendLine("                      Id_Reg,             Data_Inizio_Prevista, Data_Fioritura_Prevista, Data_Fine_Prevista, ")
            StrSQL.AppendLine("                      Ricavi_Previsti,    Produzione_Prevista,  Veg_Cod,    ")
            StrSQL.AppendLine("                      Grfi_Cod,           Cod_Contratto,        CSProgetto_Cod,   ")
            StrSQL.AppendLine("                      Stato_Impianto,     Regolamento_Cod,      Disciplinare_Cod,        Disciplinare_PubblicoPrivato, Regolamento_Concimazioni_Cod, P_Ha, ")
            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            StrSQL.AppendLine("                      P_HA_Femmine,       P_HA_Maschi,          ")
            '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            StrSQL.AppendLine("                      Sup_Prog,           FlagSecondoRaccolto, ")

            StrSQL.AppendLine("                      Inviato,            DataInvio, ")
            StrSQL.AppendLine("                      Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                      UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                      Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("           " & Agro_SQL_SaveNum(idBudget) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Progetto_Des) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Progetto_Nome) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(UCase(Cau_Progetto)) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Giudizio) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Inizio_Prevista) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Fioritura_Prevista) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Fine_Prevista) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Ricavi_Previsti) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Produzione_Prevista) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Contratto) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CSProgetto_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Stato_Impianto) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_Concimazioni_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(P_Ha) & "  ")
            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(P_HA_Femmine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(P_HA_Maschi) & "  ")
            '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Prog) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(FlagSecondoRaccolto) & "  ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

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

    Public Function Cancella(ByVal IdBudget As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_Imprese_Progetti ")
            StrSQL.Append(" WHERE    id_Budget= " & Agro_SQL_SaveNum(IdBudget) & " ")

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
