Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreEntityFramework_POCO
Imports System.Data.Entity
Public Class Budget_Reg_Impianti_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Budget As Int32,
                    ByVal Piva As String,
                    ByVal Sa_Cod As Int32,
                    ByVal Appezza As Int32,
                    ByVal Id_Reg As Int32,
                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                    ByVal xFiltroAggiuntivo As String,
                    ByVal xOrderBy As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                    Optional ByVal specie As String = "",
                    Optional ByVal TipoG2G As Integer = 0,
                    Optional ByVal LeggiStaticMap As Boolean = False
            ) As DataTable

        Dim NomeRoutine As String = "BudgetDAL.Budget_Reg_Impianti_Read.Leggi()"

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

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            If specie <> "" Then
                If Id_Budget = 0 Then
                    Throw New Exception("Id_Budget obbligatorio")
                End If

                StrSQL.Length = 0
                StrSQL.AppendLine(" SELECT DISTINCT ri.* ")
                StrSQL.AppendLine(" FROM Budget_Reg_Impianti ri ")
                StrSQL.AppendLine(" JOIN Cultivar c on c.Cul_Cod = ri.CUL_COD ")
                StrSQL.AppendLine(" JOIN SpecieVegetali s on s.Veg_Cod = c.Veg_Cod ")
                StrSQL.AppendLine(" WHERE Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " AND " & specie & " ")

                If xFiltroAggiuntivo <> String.Empty Then
                    StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If

                '--------------------------------------------------------------------------
                Select Case objParametri.FlagVisibilita
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                        StrSQL.AppendLine(" AND   ri.Inviato >=0 ")
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                        StrSQL.AppendLine(" AND   ri.Inviato =-1 ")
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select
            Else

                Select Case xSelezioneVariabile

                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                        '---------------------------------------------
                        StrSQL.Length = 0
                        StrSQL.AppendLine(" SELECT Id_Budget, PIVA, SA_COD, APPEZZA, ID_REG, CUL_COD, GRFI_COD, Validita_Inizio, Validita_Fine, Sup_Imp, GRVA_Cod_VEG, UDM_COD_ALT, SUP_ALT ")
                        StrSQL.AppendLine(" FROM  Budget_Reg_Impianti ")

                        StrSQL.AppendLine(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        StrSQL.AppendLine(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                        If Id_Budget <> 0 Then
                            StrSQL.AppendLine(" AND     Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                        End If

                        If Piva <> "" Then
                            StrSQL.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                        End If

                        StrSQL.AppendLine(" AND     [User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                        If Sa_Cod <> 0 Then
                            StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                        Else
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim FiltroCentri As String = ""
                            Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                                Next
                                If FiltroCentri <> "" Then
                                    StrSQL.AppendLine(" AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                                End If
                            End If
                        End If

                        If Appezza <> 0 Then
                            StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                        End If

                        If Id_Reg <> 0 Then
                            StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                        End If

                        If xFiltroAggiuntivo <> "" Then
                            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If

                        Select Case TipoG2G
                            Case 1 'seleziona i nuovi dati.
                                StrSQL.AppendLine(" AND not exists ( ")
                                StrSQL.AppendLine(" select 1 from g2g_recode_impianti rr where rr.From_Piva = Budget_Reg_Impianti.piva and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod and rr.From_appezza = Budget_Reg_Impianti.Appezza and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")

                            Case 2 'seleziona i dati modificati
                                StrSQL.AppendLine("and ( ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    where rr.DataInvio < Budget_Reg_Impianti.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) or ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    inner join Budget_reg_impianti_codici ric ")
                                StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Sa_cod = ric.sa_cod and rr.From_appezza = ric.appezza and rr.From_Id_Reg = ric.id_reg and ric.Progetto_Cod=0")
                                StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")
                                StrSQL.AppendLine(" ) ")
                        End Select


                        '--------------------------------------------------------------------------
                        Select Case objParametri.FlagVisibilita
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
                        If xOrderBy <> "" Then
                            StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        End If




                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                        '---------------------------------------------
                        StrSQL.Length = 0
                        StrSQL.AppendLine(" SELECT * ")
                        StrSQL.AppendLine(" FROM  Budget_Reg_Impianti ")

                        StrSQL.AppendLine(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        StrSQL.AppendLine(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                        If Id_Budget <> 0 Then
                            StrSQL.AppendLine(" AND     Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                        End If

                        If Piva <> "" Then
                            StrSQL.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                        End If

                        StrSQL.AppendLine(" AND     [User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                        If Sa_Cod <> 0 Then
                            StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                        Else
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim FiltroCentri As String = ""
                            Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                                Next
                                If FiltroCentri <> "" Then
                                    StrSQL.AppendLine(" AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                                End If
                            End If
                        End If

                        If Appezza <> 0 Then
                            StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                        End If

                        If Id_Reg <> 0 Then
                            StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                        End If

                        If xFiltroAggiuntivo <> "" Then
                            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If


                        Select Case TipoG2G
                            Case 1 'seleziona i nuovi dati.
                                StrSQL.AppendLine(" AND not exists ( ")
                                StrSQL.AppendLine(" select 1 from g2g_recode_impianti rr where rr.From_Piva = Budget_Reg_Impianti.piva and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod and rr.From_appezza = Budget_Reg_Impianti.Appezza and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")

                            Case 2 'seleziona i dati modificati
                                StrSQL.AppendLine("and ( ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    where rr.DataInvio < Budget_Reg_Impianti.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) or ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    inner join Budget_reg_impianti_codici ric ")
                                StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Sa_cod = ric.sa_cod and rr.From_appezza = ric.appezza and rr.From_Id_Reg = ric.id_reg and ric.Progetto_Cod=0")
                                StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")
                                StrSQL.AppendLine(" ) ")
                        End Select
                        '--------------------------------------------------------------------------
                        Select Case objParametri.FlagVisibilita
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
                        If xOrderBy <> "" Then
                            StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        Else
                            'Nota: Questo ordinamento è importante per la gestione del campo.
                            'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo

                            StrSQL.AppendLine(" ORDER BY Validita_Inizio Desc ")
                        End If

                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                        StrSQL.Length = 0
                        StrSQL.AppendLine(" SELECT  Budget_Reg_Impianti.*,  ")
                        StrSQL.AppendLine("         ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod,  ")
                        StrSQL.AppendLine("         ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,  ")
                        StrSQL.AppendLine("         ISNULL(SpecieVegetali.Gru_Cod,0) AS Gru_Cod,  ")
                        StrSQL.AppendLine("         ISNULL(GruppoVegetale.Gru_Des,'') AS Gru_Des,  ")
                        StrSQL.AppendLine("         ISNULL(Cultivar.Cul_Des,'') AS Cul_Des,  ")
                        StrSQL.AppendLine("         ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des,  ")
                        StrSQL.AppendLine("         ISNULL(GruppoFinalita.Grfi_Cod,0) AS Grfi_Cod, ")
                        StrSQL.AppendLine("         ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ")
                        StrSQL.AppendLine("         Budget_Appezzamento.APP_NOME, ")
                        StrSQL.AppendLine("         Budget_Appezzamento.Validita_inizio AS Validita_inizio_Appezzamento, Budget_Appezzamento.Validita_Fine AS Validita_fine_Appezzamento, ")
                        StrSQL.AppendLine("         ISNULL(ImpiantiIrrigazioni.Imp_Cod,0) AS Imp_Cod, ")
                        StrSQL.AppendLine("         ISNULL(ImpiantiIrrigazioni.Imp_Des,'') AS Imp_Des, ")
                        StrSQL.AppendLine("         ISNULL(Copertura.Cop_Cod,0) AS Cop_Cod, ")
                        StrSQL.AppendLine("         ISNULL(Copertura.Cop_Des,'') AS Cop_Des, ")

                        StrSQL.AppendLine("         ISNULL(UnitaMisura_Alternative.UDM_SIM,'') AS UDM_SIM_ALT, ")
                        StrSQL.AppendLine("         ISNULL(UnitaMisura_Alternative.UDM_DES,'') AS UDM_DES_ALT ")
                        StrSQL.AppendLine("         ,ISNULL(Conversione_UnitaMisura_Alternative.Tasso_Conv,0) AS Tasso_Conv ")

                        StrSQL.AppendLine("         ,ISNULL ((SELECT     TOP 1 Budget_Reg_Impianti_Codici.id_cod ")
                        StrSQL.AppendLine("                   FROM Budget_Reg_Impianti_Codici ")
                        StrSQL.AppendLine("                   WHERE (Budget_Reg_Impianti_Codici.Id_Budget = Budget_Reg_Impianti.Id_Budget) ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.PIVA = Budget_Reg_Impianti.PIVA)   ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.sa_cod = Budget_Reg_Impianti.sa_cod)   ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.appezza = Budget_Reg_Impianti.appezza)  ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.ID_REG = Budget_Reg_Impianti.Id_Reg) ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.id_cod >=3000 and Budget_Reg_Impianti_Codici.id_cod<4000) ")
                        StrSQL.AppendLine("                  ), 0) AS id_cod ")

                        StrSQL.AppendLine(" FROM    GruppoFinalita RIGHT OUTER JOIN ")
                        StrSQL.AppendLine("         Budget_Appezzamento INNER JOIN ")
                        StrSQL.AppendLine("         Budget_Reg_Impianti ON Budget_Appezzamento.Id_Budget = Budget_Reg_Impianti.Id_Budget AND Budget_Appezzamento.PIVA = Budget_Reg_Impianti.PIVA AND Budget_Appezzamento.SA_COD = Budget_Reg_Impianti.SA_COD AND ")
                        StrSQL.AppendLine("         Budget_Appezzamento.APPEZZA = Budget_Reg_Impianti.APPEZZA LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         GruppoVarietale ON Budget_Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         ImpiantiIrrigazioni ON Budget_Reg_Impianti.IMP_COD = ImpiantiIrrigazioni.Imp_Cod LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         Copertura ON Budget_Reg_Impianti.COP_COD = Copertura.Cop_Cod ON GruppoFinalita.Grfi_Cod = Budget_Reg_Impianti.GRFI_COD LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         GruppoVegetale INNER JOIN ")
                        StrSQL.AppendLine("         SpecieVegetali INNER JOIN ")
                        StrSQL.AppendLine("         Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON GruppoVegetale.Gru_Cod = SpecieVegetali.Gru_Cod ON  ")
                        StrSQL.AppendLine("         Budget_Reg_Impianti.CUL_COD = Cultivar.Cul_Cod LEFT JOIN")
                        StrSQL.AppendLine("         UnitaMisura_Alternative ON Budget_Reg_Impianti.UDM_COD_ALT = UnitaMisura_Alternative.UDM_COD_ALT ")
                        StrSQL.AppendLine("         LEFT JOIN Conversione_UnitaMisura_Alternative ON UnitaMisura_Alternative.UDM_COD_ALT = Conversione_UnitaMisura_Alternative.UDM_COD_ALT_TO AND Conversione_UnitaMisura_Alternative.UDM_COD_FROM = 2123 ")

                        StrSQL.AppendLine(" WHERE   (Budget_Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                        StrSQL.AppendLine(" AND     (Budget_Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                        StrSQL.AppendLine(" AND     Budget_Reg_Impianti.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                        If Id_Budget <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                        End If

                        If (Piva <> "") Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                        End If

                        If Sa_Cod <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                        Else
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim FiltroCentri As String = ""
                            Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                                Next
                                If FiltroCentri <> "" Then
                                    StrSQL.AppendLine(" AND Budget_Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                                End If
                            End If
                        End If


                        If Appezza <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                        End If

                        If Id_Reg <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "  ")
                        End If


                        If xFiltroAggiuntivo <> "" Then
                            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If

                        Select Case TipoG2G
                            Case 1 'seleziona i nuovi dati.
                                StrSQL.AppendLine(" AND not exists ( ")
                                StrSQL.AppendLine(" select 1 from g2g_recode_impianti rr where rr.From_Piva = Budget_Reg_Impianti.piva and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod and rr.From_appezza = Budget_Reg_Impianti.Appezza and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")

                            Case 2 'seleziona i dati modificati
                                StrSQL.AppendLine("and ( ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    where rr.DataInvio < Budget_Reg_Impianti.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) or ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    inner join Budget_reg_impianti_codici ric ")
                                StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Sa_cod = ric.sa_cod and rr.From_appezza = ric.appezza and rr.From_Id_Reg = ric.id_reg and ric.Progetto_Cod=0")
                                StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")
                                StrSQL.AppendLine(" ) ")
                        End Select

                        '--------------------------------------------------------------------------
                        Select Case objParametri.FlagVisibilita
                            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Inviato >=0 ")
                            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Inviato =-1 ")
                            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                                '...................................
                            Case Else
                                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                        End Select
                        '--------------------------------------------------------------------------
                        If xOrderBy <> "" Then
                            StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        End If

                    '


                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                        '
                        StrSQL.Length = 0
                        StrSQL.AppendLine(" SELECT  Budget_Reg_Impianti.*,  ")
                        StrSQL.AppendLine("         ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod,  ")
                        StrSQL.AppendLine("         ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,  ")
                        StrSQL.AppendLine("         ISNULL(SpecieVegetali.Gru_Cod,0) AS Gru_Cod,  ")
                        StrSQL.AppendLine("         ISNULL(GruppoVegetale.Gru_Des,'') AS Gru_Des,  ")
                        StrSQL.AppendLine("         ISNULL(Cultivar.Cul_Des,'') AS Cul_Des,  ")
                        StrSQL.AppendLine("         ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des,  ")
                        StrSQL.AppendLine("         ISNULL(GruppoFinalita.Grfi_Cod,0) AS Grfi_Cod, ")
                        StrSQL.AppendLine("         ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ")
                        StrSQL.AppendLine("         Budget_Appezzamento.APP_NOME, ")
                        StrSQL.AppendLine("         Budget_Appezzamento.Validita_inizio AS Validita_inizio_Appezzamento, Budget_Appezzamento.Validita_Fine AS Validita_fine_Appezzamento, ")
                        StrSQL.AppendLine("         ISNULL(ImpiantiIrrigazioni.Imp_Cod,0) AS Imp_Cod2, ")
                        StrSQL.AppendLine("         ISNULL(ImpiantiIrrigazioni.Imp_Des,'') AS Imp_Des, ")
                        StrSQL.AppendLine("         ISNULL(Copertura.Cop_Cod,0) AS Cop_Cod, ")
                        StrSQL.AppendLine("         ISNULL(Copertura.Cop_Des,'') AS Cop_Des, ")

                        StrSQL.AppendLine("         Imprese.rag_soc, Centri_Aziendali.sa_nome, ")

                        StrSQL.AppendLine("         Budget_Imprese_Progetti.Progetto_Cod, Budget_Imprese_Progetti.Progetto_Nome, ")
                        StrSQL.AppendLine("         Budget_Imprese_Progetti.Stato_Impianto, Budget_Imprese_Progetti.Disciplinare_Cod,  ")
                        StrSQL.AppendLine("         Budget_Imprese_Progetti.Regolamento_Cod, Regolamenti.Reg_Des, ")
                        StrSQL.AppendLine("         Budget_Imprese_Progetti.Validita_inizio AS Validita_inizio_Distinta, Budget_Imprese_Progetti.Validita_Fine AS Validita_fine_Distinta, ")

                        StrSQL.AppendLine("         ISNULL(UnitaMisura_Alternative.UDM_SIM,'') AS UDM_SIM_ALT, ")
                        StrSQL.AppendLine("         ISNULL(UnitaMisura_Alternative.UDM_DES,'') AS UDM_DES_ALT ")
                        StrSQL.AppendLine("         ,ISNULL(Conversione_UnitaMisura_Alternative.Tasso_Conv,0) AS Tasso_Conv ")

                        StrSQL.AppendLine("         ,ISNULL ((SELECT     TOP 1 Budget_Reg_Impianti_Codici.id_cod ")
                        StrSQL.AppendLine("                   FROM Budget_Reg_Impianti_Codici ")
                        StrSQL.AppendLine("                   WHERE (Budget_Reg_Impianti_Codici.Id_Budget = Budget_Reg_Impianti.Id_Budget) ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.PIVA = Budget_Reg_Impianti.PIVA)   ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.sa_cod = Budget_Reg_Impianti.sa_cod)   ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.appezza = Budget_Reg_Impianti.appezza)  ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.ID_REG = Budget_Reg_Impianti.Id_Reg) ")
                        StrSQL.AppendLine("                   AND (Budget_Reg_Impianti_Codici.id_cod >=3000 and Budget_Reg_Impianti_Codici.id_cod<4000) ")
                        StrSQL.AppendLine("                  ), 0) AS id_cod ")

                        If LeggiStaticMap Then

                            StrSQL.AppendLine(", ISNULL (g.Poligono_GeoEntity.STAsText(), '') as cartografia")
                            StrSQL.AppendLine(", ( ")
                            StrSQL.AppendLine("     Select * from(select StaticMap as '*') Tbl ")
                            StrSQL.AppendLine("     For Xml path('') ")
                            StrSQL.AppendLine(" ) StaticMapBase64String ")
                            StrSQL.AppendLine(" ")
                        End If

                        StrSQL.AppendLine(" FROM    Regolamenti INNER JOIN ")
                        StrSQL.AppendLine("         Budget_Appezzamento INNER JOIN ")
                        StrSQL.AppendLine("         Budget_Reg_Impianti ON Budget_Appezzamento.Id_Budget = Budget_Reg_Impianti.Id_Budget AND Budget_Appezzamento.PIVA = Budget_Reg_Impianti.PIVA AND Budget_Appezzamento.SA_COD = Budget_Reg_Impianti.SA_COD AND ")
                        StrSQL.AppendLine("         Budget_Appezzamento.APPEZZA = Budget_Reg_Impianti.APPEZZA INNER JOIN ")
                        StrSQL.AppendLine("         Centri_Aziendali ON Budget_Appezzamento.PIVA = Centri_Aziendali.PIVA AND Budget_Appezzamento.SA_COD = Centri_Aziendali.sa_cod INNER JOIN ")
                        StrSQL.AppendLine("         Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA INNER JOIN ")
                        StrSQL.AppendLine("         Budget_Imprese_Progetti ON Budget_Reg_Impianti.Id_Budget = Budget_Imprese_Progetti.Id_Budget AND Budget_Reg_Impianti.PIVA = Budget_Imprese_Progetti.Piva AND Budget_Reg_Impianti.SA_COD = Budget_Imprese_Progetti.Sa_Cod AND ")
                        StrSQL.AppendLine("         Budget_Reg_Impianti.APPEZZA = Budget_Imprese_Progetti.Appezza AND Budget_Reg_Impianti.ID_REG = Budget_Imprese_Progetti.Id_Reg ON ")
                        StrSQL.AppendLine("         Regolamenti.Reg_Cod = Budget_Imprese_Progetti.Regolamento_Cod LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         GruppoVarietale ON Budget_Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         ImpiantiIrrigazioni ON Budget_Reg_Impianti.IMP_COD = ImpiantiIrrigazioni.Imp_Cod LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         Copertura ON Budget_Reg_Impianti.COP_COD = Copertura.Cop_Cod LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         GruppoFinalita ON Budget_Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                        StrSQL.AppendLine("         GruppoVegetale INNER JOIN ")
                        StrSQL.AppendLine("         SpecieVegetali INNER JOIN ")
                        StrSQL.AppendLine("         Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON GruppoVegetale.Gru_Cod = SpecieVegetali.Gru_Cod ON ")
                        StrSQL.AppendLine("         Budget_Reg_Impianti.CUL_COD = Cultivar.Cul_Cod LEFT JOIN")
                        StrSQL.AppendLine("         UnitaMisura_Alternative ON Budget_Reg_Impianti.UDM_COD_ALT = UnitaMisura_Alternative.UDM_COD_ALT ")
                        StrSQL.AppendLine("         LEFT JOIN Conversione_UnitaMisura_Alternative ON UnitaMisura_Alternative.UDM_COD_ALT = Conversione_UnitaMisura_Alternative.UDM_COD_ALT_TO AND Conversione_UnitaMisura_Alternative.UDM_COD_FROM = 2123 ")


                        If LeggiStaticMap Then
                            StrSQL.AppendLine(" Left Join gis_entita e ")
                            StrSQL.AppendLine("     On  Budget_Reg_Impianti.piva = e.piva ")
                            StrSQL.AppendLine("     And Budget_Reg_Impianti.sa_cod = e.sa_cod ")
                            StrSQL.AppendLine("     And Budget_Reg_Impianti.appezza = e.appezza    ")
                            StrSQL.AppendLine("     And Budget_Reg_Impianti.ID_REG = e.Id_Imp ")
                            StrSQL.AppendLine(" Left Join gis_elementigrafici g ")
                            StrSQL.AppendLine("     On   e.PivaSuperUser = g.PivaSuperUser ")
                            StrSQL.AppendLine("     And  e.entita_cod = g.Entita_Cod ")
                            StrSQL.AppendLine("     And g.LayerElementiGrafici_Cod = 19")

                        End If

                        StrSQL.AppendLine(" WHERE   (Budget_Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                        StrSQL.AppendLine(" AND     (Budget_Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                        StrSQL.AppendLine(" AND     Budget_Reg_Impianti.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                        If Id_Budget <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                        End If

                        If (Piva <> "") Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                        End If

                        If Sa_Cod <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                        Else
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim FiltroCentri As String = ""
                            Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                                Next
                                If FiltroCentri <> "" Then
                                    StrSQL.AppendLine(" AND Budget_Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                                End If
                            End If
                        End If


                        If Appezza <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                        End If

                        If Id_Reg <> 0 Then
                            StrSQL.AppendLine(" AND Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "  ")
                        End If


                        If xFiltroAggiuntivo <> "" Then
                            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If

                        Select Case TipoG2G
                            Case 1 'seleziona i nuovi dati.
                                StrSQL.AppendLine(" AND not exists ( ")
                                StrSQL.AppendLine(" select 1 from g2g_recode_impianti rr where rr.From_Piva = Budget_Reg_Impianti.piva and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod and rr.From_appezza = Budget_Reg_Impianti.Appezza and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")

                            Case 2 'seleziona i dati modificati
                                StrSQL.AppendLine("and ( ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    where rr.DataInvio < Budget_Reg_Impianti.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) or ")
                                StrSQL.AppendLine(" exists ( ")
                                StrSQL.AppendLine("    select 1 ")
                                StrSQL.AppendLine("    from g2g_recode_impianti rr ")
                                StrSQL.AppendLine("    inner join Budget_reg_impianti_codici ric ")
                                StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Sa_cod = ric.sa_cod and rr.From_appezza = ric.appezza and rr.From_Id_Reg = ric.id_reg and ric.Progetto_Cod=0")
                                StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                                StrSQL.AppendLine("    and rr.From_Piva = Budget_Reg_Impianti.piva  ")
                                StrSQL.AppendLine("    and rr.From_Sa_cod = Budget_Reg_Impianti.Sa_cod  ")
                                StrSQL.AppendLine("    and rr.From_appezza = Budget_Reg_Impianti.Appezza ")
                                StrSQL.AppendLine("    and rr.From_Id_Reg = Budget_Reg_Impianti.id_reg ")
                                StrSQL.AppendLine(" ) ")
                                StrSQL.AppendLine(" ) ")
                        End Select

                        '--------------------------------------------------------------------------
                        Select Case objParametri.FlagVisibilita
                            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Inviato >=0 ")
                            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Inviato =-1 ")
                            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                                '...................................
                            Case Else
                                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                        End Select
                        '--------------------------------------------------------------------------
                        If xOrderBy <> "" Then
                            StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        End If


                End Select
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

    Public Function LeggiSuperficie(ByVal Id_Budget As Integer,
                                   ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Appezza As Integer,
                                   ByVal ID_Reg As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Decimal


        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti.LeggiSuperficie()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Sup_Imp  ")
            StrSQL.AppendLine(" FROM  Budget_Reg_Impianti ")
            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND Budget_Reg_Impianti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If ID_Reg <> 0 Then
                StrSQL.AppendLine(" AND Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(ID_Reg) & " ")
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

        If IsDBNull(DT.Rows(0).Item("Sup_Imp")) Then
            Return 0
        End If
        Return CDbl(DT.Rows(0).Item("Sup_Imp"))

    End Function

    Public Function Leggi_Max_DataModifica(ByVal Id_Budget As Integer,
                             ByVal Piva As String,
                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DateTime

        Dim NomeRoutine As String = "BudgetDAL.Budget_Reg_Impianti_Read.Leggi_Max_DataModifica()"

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
            StrSQL.AppendLine(" FROM Budget_Reg_Impianti ")
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

    Public Function Leggi_Specie_Da_Budget(ByVal Id_Budget As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "BudgetDAL.Budget_Reg_Impianti_Read.Leggi_Specie_Da_Budget()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Data_Modifica = AGRODATAINIZIO
        Try

            If Id_Budget = 0 Then
                Throw New Exception("Id_Budget obbligatorio")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT s.Veg_Cod As Codice, s.Veg_Des As Specie ")
            StrSQL.AppendLine(" FROM Budget_Reg_Impianti ri ")
            StrSQL.AppendLine(" JOIN Cultivar c on c.Cul_Cod = ri.CUL_COD ")
            StrSQL.AppendLine(" JOIN SpecieVegetali s on s.Veg_Cod = c.Veg_Cod ")
            StrSQL.AppendLine(" WHERE Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

            If xFiltroAggiuntivo <> String.Empty Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case ObjParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ri.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ri.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            Data_Modifica = AGRODATAINIZIO
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    Public Function Leggi_Specie_Utilizzo_Da_Budget(ByVal Id_Budget As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "BudgetDAL.Budget_Reg_Impianti_Read.Leggi_Specie_Da_Budget()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Data_Modifica = AGRODATAINIZIO
        Try

            If Id_Budget = 0 Then
                Throw New Exception("Id_Budget obbligatorio")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT  ")
            StrSQL.AppendLine("   CASE WHEN  s.Veg_Cod IS NULL")
            StrSQL.AppendLine("   THEN '0/'+ CONVERT(varchar, ic.id_cod)")
            StrSQL.AppendLine("   ELSE CONVERT(varchar, s.Veg_Cod) + '/0'")
            StrSQL.AppendLine("   END AS codice")
            StrSQL.AppendLine(" , CASE WHEN  s.Veg_Cod IS NULL")
            StrSQL.AppendLine("   THEN ca.descrizione")
            StrSQL.AppendLine("   ELSE s.Veg_Des END AS descrizione")

            StrSQL.AppendLine(" FROM Budget_Reg_Impianti ri ")

            StrSQL.AppendLine(" LEFT JOIN Cultivar c on c.Cul_Cod = ri.CUL_COD ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali s on s.Veg_Cod = c.Veg_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Budget_Reg_Impianti_Codici ic ON ic.Id_Budget = ri.Id_Budget AND ic.PIVA = ri.PIVA AND ic.SA_COD = ri.SA_COD AND ic.appezza = ri.APPEZZA AND ic.Id_Reg = ri.ID_REG AND ic.id_cod between 3000 AND 3999")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe ca ON ca.codice = ic.id_cod")

            StrSQL.AppendLine(" WHERE ri.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

            If xFiltroAggiuntivo <> String.Empty Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case ObjParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ri.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ri.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            Data_Modifica = AGRODATAINIZIO
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function
End Class
Public Class Budget_Reg_Impianti_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal IdBudget As Integer,
                                               ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Appezza As Integer,
                                               ByVal Id_Reg As Integer,
                                               ByVal Cod_Resp As Integer,
                                               ByVal Cod_Ente As Integer,
                                               ByVal Campo_Spia As Integer,
                                               ByVal Data As Date,
                                               ByVal Cul_Cod As Integer,
                                               ByVal Grfi_Cod As Integer,
                                               ByVal Data_Raccolta As Date,
                                               ByVal Resa_Prevista As Decimal,
                                               ByVal Resa_Effettiva As Decimal,
                                               ByVal Scarto As Integer,
                                               ByVal Ind_Mat_Cod As Integer,
                                               ByVal Ind_Mat_Ril As Integer,
                                               ByVal Sta_Ter As String,
                                               ByVal Cop_DI As Date,
                                               ByVal Cop_DF As Date,
                                               ByVal Tra_Fila As Decimal,
                                               ByVal Su_Fila As Decimal,
                                               ByVal Data_Inizio_Portinnesto As Date,' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                                               ByVal Data_Inizio_Innesto As Date,
                                               ByVal Data_Inizio_Produzione As Date,
                                               ByVal Piante_Maschi_InSesto As Int16,' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                                               ByVal FORAL_COD As Integer,
                                               ByVal Setup_Cod As String,
                                               ByVal PORT_COD As Integer,
                                               ByVal Imp_Cod As Integer,
                                               ByVal Stru_Prot As Integer,
                                               ByVal Pro_Pag As Integer,
                                               ByVal Seme_Q As Integer,
                                               ByVal Seme_T As Integer,
                                               ByVal Seme_P As Integer,
                                               ByVal Seme_D As Integer,
                                               ByVal Stato_Residui As String,
                                               ByVal TECN_COD As Integer,
                                               ByVal Denitrificazione As Integer,
                                               ByVal Volatilizzazione As Integer,
                                               ByVal GrVa_Cod_Veg As Integer,
                                               ByVal Profonditalav As Int16,
                                               ByVal Id_Campo As Integer,
                                               ByVal Su_Cod As Integer,
                                               ByVal Cop_Cod As Integer,
                                               ByVal Cover As Int16,
                                               ByVal Monitorato As Int16,
                                               ByVal Codice_Fiscale_Tecnico As String,
                                               ByVal Regolamento As Integer,
                                               ByVal Finanziamento As Integer,
                                               ByVal Data_Conversione As Date,
                                               ByVal ProvenienzaSeme As Integer,
                                               ByVal Sup_Imp As Decimal,
                                               ByVal Id_Consociazione As Integer,
                                               ByVal Validita_Inizio As Date,
                                               ByVal Validita_Fine As Date,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                               Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                               Optional ByVal username_creazione As String = "",
                                               Optional ByVal username_modifica As String = "",
                                               Optional ByVal Unita_Vitata As Integer = 0,
                                               Optional ByVal PRODUZIONE As Integer = 0,
                                               Optional ByVal Sovrainnesto_Cod As Integer = 0
                                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
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
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Budget_Reg_Impianti ( Id_Budget")
            strSql.AppendLine("                     ,Piva                     ,Sa_Cod               ,Appezza               ,Id_Reg              ")
            strSql.AppendLine("                     ,Data_Agg                 ,Cod_Resp             ,Cod_Ente              ,Campo_Spia          ")
            strSql.AppendLine("                     ,Data                     ,Cul_Cod              ,Grfi_Cod              ,Data_Raccolta       ")
            strSql.AppendLine("                     ,Resa_Prevista            ,Resa_Effettiva       ,Scarto                ,Ind_Mat_Cod         ,Ind_Mat_Ril ")
            strSql.AppendLine("                     ,Sta_Ter                  ,Cop_DI               ,Cop_DF                ,Tra_Fila            ")
            strSql.AppendLine("                     ,Su_Fila                  ,Foral_Cod            ,Setup_Cod             ")
            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            strSql.AppendLine("                     ,Data_Inizio_Portinnesto  ,Data_Inizio_Innesto  ,Data_Inizio_Produzione,Piante_Maschi_InSesto ")
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
            strSql.AppendLine("                     ,Port_Cod                 ,Imp_Cod              ,Stru_Prot             ,Pro_Pag             ")
            strSql.AppendLine("                     ,Seme_Q                   ,Seme_T               ,Seme_P                ,Seme_D              ")
            strSql.AppendLine("                     ,Stato_Residui            ,Tecn_Cod             ")
            strSql.AppendLine("                     ,Denitrificazione         ,Volatilizzazione,    GrVa_Cod_Veg  ")
            strSql.AppendLine("                     ,Profonditalav            ,Id_Campo             ")
            strSql.AppendLine("                     ,Su_Cod                   ,Cop_Cod             ,Cover           ,Monitorato           ")
            strSql.AppendLine("                     ,Codice_Fiscale_Tecnico   ,[User]              ,Regolamento     ")
            strSql.AppendLine("                     ,Finanziamento            ,Data_Conversione,    ProvenienzaSeme ")
            strSql.AppendLine("                     ,Sup_Imp,                  Id_Consociazione,    Unita_Vitata    ")
            strSql.AppendLine("                     ,PRODUZIONE,               Sovrainnesto_Cod     ")

            strSql.AppendLine("                     ,Inviato                  ,datainvio            ")
            strSql.AppendLine("                     ,Data_Creazione           ,Data_Modifica        ")
            strSql.AppendLine("                     ,UserName_Creazione       ,UserName_Modifica    ")
            strSql.AppendLine("                     ,Validita_Inizio          ,Validita_Fine        ")
            strSql.AppendLine("                     ) ")

            strSql.AppendLine("VALUES ( ")
            strSql.AppendLine("            " & Agro_SQL_SaveNum(IdBudget) & "  ")
            strSql.AppendLine("          ,'" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Cod_Resp) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Cod_Ente) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Campo_Spia) & "  ")
            strSql.AppendLine("          , " & IIf(Data = New Date, "Null", Agro_SQL_SaveDate(Data)) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(Grfi_Cod, False) = -1, "Null", Agro_SQL_SaveNum(Grfi_Cod)) & "  ")
            strSql.AppendLine("          , " & IIf(Data_Raccolta = New Date, "Null", Agro_SQL_SaveDate(Data_Raccolta)) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Resa_Prevista) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Resa_Effettiva) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Scarto) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Ind_Mat_Cod) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Ind_Mat_Ril) & "  ")
            strSql.AppendLine("          ,'" & Agro_SQL_SaveText(Sta_Ter) & "' ")
            strSql.AppendLine("          , " & IIf(Cop_DI = New Date, "Null", Agro_SQL_SaveDate(Cop_DI)) & "  ")
            strSql.AppendLine("          , " & IIf(Cop_DF = New Date, "Null", Agro_SQL_SaveDate(Cop_DF)) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Tra_Fila) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Su_Fila) & "  ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(FORAL_COD, False) = -1, "Null", Agro_SQL_SaveNum(FORAL_COD)) & "  ")
            strSql.AppendLine("          ,'" & Agro_SQL_SaveText(Setup_Cod) & "' ")

            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            strSql.AppendLine("          , " & Agro_SQL_SaveDate(Data_Inizio_Portinnesto) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveDate(Data_Inizio_Innesto) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveDate(Data_Inizio_Produzione) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Piante_Maschi_InSesto) & "  ")
            '  - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            strSql.AppendLine("          , " & Agro_SQL_SaveNum(PORT_COD) & "  ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(Imp_Cod, False) = -1, "Null", Agro_SQL_SaveNum(Imp_Cod)) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Stru_Prot) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Pro_Pag) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Seme_Q) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Seme_T) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Seme_P) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Seme_D) & "  ")
            strSql.AppendLine("          ,'" & Agro_SQL_SaveText(Stato_Residui) & "' ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(TECN_COD, False) = -1, "Null", Agro_SQL_SaveNum(TECN_COD)) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Denitrificazione) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Volatilizzazione) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(GrVa_Cod_Veg) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Profonditalav) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Id_Campo) & "  ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(Su_Cod, False) = -1, "Null", Agro_SQL_SaveNum(Su_Cod)) & "  ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(Cop_Cod, False) = -1, "Null", Agro_SQL_SaveNum(Cop_Cod)) & "  ")

            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Cover) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Monitorato) & "  ")
            strSql.AppendLine("          ,'" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ")
            strSql.AppendLine("          ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(Regolamento, False) = -1, "Null", Agro_SQL_SaveNum(Regolamento)) & "  ")
            strSql.AppendLine("          , " & IIf(Agro_SQL_SaveNum(Finanziamento, False) = -1, "Null", Agro_SQL_SaveNum(Finanziamento)) & "  ")
            strSql.AppendLine("          , " & IIf(Data_Conversione = New Date, "Null", Agro_SQL_SaveDate(Data_Conversione)) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(ProvenienzaSeme) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Sup_Imp) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Id_Consociazione) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Unita_Vitata) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(PRODUZIONE) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveNum(Sovrainnesto_Cod) & "  ")

            strSql.AppendLine("          , 0  ")
            strSql.AppendLine("          , Null  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("          , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("          , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine("            )")

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

    Public Function Cancella(ByVal IdBudget As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Write.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_Reg_Impianti ")
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


    Public Function AggiornaValiditaInizio(ByVal Id_Budget As Integer,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Appezza As Int32,
                                    ByVal Id_Campo As Int32,
                                    ByVal Id_Reg As Int32,
                                    ByVal Validita_Inizio As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Write.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si aggiornano tutti gli impianti dell'impresa
        '   Appezza = 0          =>  si aggiornano tutti gli impianti del centro aziendale
        '   Id_Campo = 0         =>  si aggiornano tutti gli impianti di tutti i campi del centro
        '   Id_reg = 0           =>  si aggiornano tutti gli impianti dell'appezzamento
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Budget_Reg_Impianti SET ")
            StrSQL.AppendLine("      Budget_Reg_Impianti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("     ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Data   =  " & Agro_SQL_SaveDate(Validita_Inizio))

            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza IN ( SELECT Budget_Appezzamento.Appezza From Budget_Appezzamento " &
                                                                 " WHERE Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) &
                                                                 " AND   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " AND   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            '---------------------------------------------
            'Query di aggiornamento Data Inizio Copertura     

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Budget_Reg_Impianti SET ")
            StrSQL.AppendLine("      Budget_Reg_Impianti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Cop_Di   =  " & Agro_SQL_SaveDate(Validita_Inizio))

            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Cop_Di < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.AppendLine(" AND  Budget_Reg_Impianti.Appezza IN ( Select Budget_Appezzamento.Appezza From Budget_Appezzamento " &
                                                                 " Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " And   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) &
                                                                 " And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            '---------------------------------------------
            'Query di aggiornamento Data Fine Copertura     

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Budget_Reg_Impianti SET ")
            StrSQL.AppendLine("      Budget_Reg_Impianti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("     ,budget_Reg_Impianti.Cop_Df   =  " & Agro_SQL_SaveDate(Validita_Inizio))

            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Cop_Df < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.AppendLine(" AND  Budget_Reg_Impianti.Appezza IN ( Select Budget_Appezzamento.Appezza From Budget_Appezzamento " &
                                                                 " Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " And   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) &
                                                                 " And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
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

    Public Function AggiornaValiditaFine(ByVal Id_Budget As Integer,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Appezza As Int32,
                                    ByVal Id_Campo As Int32,
                                    ByVal Id_Reg As Int32,
                                    ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Write.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si aggiornano tutti gli impianti dell'impresa
        '   Appezza = 0          =>  si aggiornano tutti gli impianti del centro aziendale
        '   Id_Campo = 0         =>  si aggiornano tutti gli impianti di tutti i campi del centro
        '   Id_reg = 0           =>  si aggiornano tutti gli impianti dell'appezzamento
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Budget_Reg_Impianti SET ")
            StrSQL.AppendLine("      Budget_Reg_Impianti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("     ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))

            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza IN ( SELECT Budget_Appezzamento.Appezza From Budget_Appezzamento " &
                                                                 " WHERE Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) &
                                                                 " AND   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " AND   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '---------------------------------------------
            'Aggiorno la data raccolta se non rientra più entro il periodo di validità dell'impianto

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Budget_Reg_Impianti SET ")
            StrSQL.AppendLine("      Budget_Reg_Impianti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Data_Raccolta   =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Data_Raccolta > " & Agro_SQL_SaveDate(Validita_Fine))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.AppendLine(" AND  Budget_Reg_Impianti.Appezza IN ( Select Budget_Appezzamento.Appezza From Budget_Appezzamento " &
                                                                 " Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) &
                                                                 " And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '---------------------------------------------
            'Query di aggiornamento Data Inizio Copertura     

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Budget_Reg_Impianti SET ")
            StrSQL.AppendLine("      Budget_Reg_Impianti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Cop_Di   =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Cop_Di > " & Agro_SQL_SaveDate(Validita_Fine))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.AppendLine(" AND  Budget_Reg_Impianti.Appezza IN ( Select Budget_Appezzamento.Appezza From Budget_Appezzamento " &
                                                                 " Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) &
                                                                 " And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '---------------------------------------------
            'Query di aggiornamento Data Fine Copertura     

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Budget_Reg_Impianti SET ")
            StrSQL.AppendLine("      Budget_Reg_Impianti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("     ,Budget_Reg_Impianti.Cop_Df   =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Budget_Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Cop_Df > " & Agro_SQL_SaveDate(Validita_Fine))

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.AppendLine(" AND  Budget_Reg_Impianti.Appezza IN ( Select Budget_Appezzamento.Appezza From Budget_Appezzamento " &
                                                                 " Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) &
                                                                 " And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND   Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
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

End Class
