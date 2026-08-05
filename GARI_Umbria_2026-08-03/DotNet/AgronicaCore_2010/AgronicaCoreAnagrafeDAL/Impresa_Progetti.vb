Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Imports System.Text
Imports System.Globalization
Imports Newtonsoft.Json

Public Class Impresa_Progetti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <param name="Data_Inizio">Usato per filtrare gli esercizi con Validita_Fine maggiore o uguale a Data_Inizio</param>
    ''' <param name="Data_Fine">Usato per filtrare gli esercizi con Validita_Inizio minore o uguale a Data_Fine</param>
    ''' <returns></returns>
    Public Function LeggiMinimal(ByVal Piva As String,
                                 ByVal Sa_Cod As Int32,
                                 ByVal Appezza As Int32,
                                 ByVal Id_Reg As Int32,
                                 ByVal Progetto_Cod As Int32,
                                 ByVal Data_Inizio As Date,
                                 ByVal Data_Fine As Date,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiMinimal()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  * ")
            StrSQL.AppendLine(" FROM Imprese_Progetti ")

            StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" order by validita_inizio desc ")
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


    Public Function EsisteDistinta_SuRangeDate(ByVal Piva As String,
                                               ByVal Sa_Cod As Int32,
                                               ByVal Appezza As Int32,
                                               ByVal Id_Reg As Int32,
                                               ByVal Data_Inizio As Date,
                                               ByVal Data_Fine As Date,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Integer

        Dim DtProgetto As DataTable
        Dim EsisteDistinta As Boolean = False
        'Dim Progetto_Cod As Integer = 0

        DtProgetto = LeggiMinimal(Piva, Sa_Cod, Appezza, Id_Reg, 0,
                                  Data_Inizio, Data_Fine,
                                  "", "", objParametri)

        If DtProgetto IsNot Nothing AndAlso DtProgetto.Rows.Count > 0 Then
            'Progetto_Cod = DtProgetto.Rows(0).Item("Progetto_Cod")
            EsisteDistinta = True
        End If

        'Return Progetto_Cod
        Return EsisteDistinta

    End Function

    Public Function ProgettoNome_from_ProgettoCod(ByVal ProgettoCod As Integer,
                                                  ByRef Cau_Progetto As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As String

        If ProgettoCod <> 0 Then

            Dim DtProgetto As DataTable

            Dim ProgettoNome As String

            DtProgetto = Leggi("",
                               CInt(ProgettoCod),
                               CStr(9100),
                               0, 0, 0, 0, 0, 0,
                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                               "", "", objParametri)

            If DtProgetto IsNot Nothing AndAlso DtProgetto.Rows.Count > 0 Then
                ProgettoNome = DtProgetto.Rows(0).Item("progetto_nome")
            Else
                ProgettoNome = ""
            End If

            'Elimino il datatable (anche se non necessario in ASP.NET)
            DtProgetto = Nothing

            Return ProgettoNome

        Else

            Return ""

        End If

    End Function

    Public Function Leggi(ByVal Piva As String,
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

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.Leggi()"

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
                    StrSQL.AppendLine(" SELECT  Imprese_Progetti.* ")

                    StrSQL.AppendLine(" FROM Imprese_Progetti ")

                    StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If


                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Distinta rr where rr.From_Piva = Imprese_Progetti.piva and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and ( ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Imprese_Progetti.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) or ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    inner join reg_impianti_codici ric ")
                            StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Progetto_Cod = ric.Progetto_Cod ")
                            StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")
                            StrSQL.AppendLine(" ) ")
                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato = -1 ")
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
                    StrSQL.AppendLine("SELECT Imprese_Progetti.*,")
                    StrSQL.AppendLine("    gr.GruppoRaccolta_Des,")
                    StrSQL.AppendLine("    YEAR(Imprese_Progetti.Validita_Inizio) AS 'anno_validita',")
                    StrSQL.AppendLine("    convert (varchar(10), imprese_progetti.Validita_Inizio ,103 ) as v_i_date,")
                    StrSQL.AppendLine("    convert (varchar(10), imprese_progetti.Validita_Fine ,103 ) as v_f_date, ")
                    StrSQL.AppendLine("    CASE WHEN Imprese_Progetti.Stato_Impianto = 102 THEN 'In Produzione' ELSE COALESCE(FasiCicloColturale_Anagrafiche.Fase_Des, '') END as Fase_Des ")
                    StrSQL.AppendLine(" , CAST(COALESCE(grfi_RER.Val_Cod, 0) as int) as Tipologia_Cod  ")
                    StrSQL.AppendLine(" , COALESCE(GruppoFinalita_Rer.GRFI_DES, '') as Tipologia_Des ")
                    StrSQL.AppendLine("FROM Imprese_Progetti (NOLOCK) ")
                    StrSQL.AppendLine("LEFT JOIN Gruppi_Raccolta (NOLOCK) gr ON gr.GruppoRaccolta_Cod = Imprese_Progetti.GruppoRaccolta_Cod ")
                    StrSQL.AppendLine("LEFT JOIN FasiCicloColturale_Anagrafiche (NOLOCK) ON Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici (NOLOCK) grfi_RER ON Imprese_Progetti.Progetto_Cod = grfi_RER.Progetto_Cod AND grfi_RER.id_Cod = " & enum_CodiciAnagrafe.Finalita_Concimazione_Impianto & " ")
                    StrSQL.AppendLine(" LEFT JOIN GruppoFinalita_Rer (NOLOCK) ON grfi_RER.Val_Cod = GruppoFinalita_Rer.GRFI_COD ")
                    StrSQL.AppendLine("WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If


                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Distinta rr where rr.From_Piva = Imprese_Progetti.piva and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and ( ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Imprese_Progetti.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) or ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    inner join reg_impianti_codici ric ")
                            StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Progetto_Cod = ric.Progetto_Cod ")
                            StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")
                            StrSQL.AppendLine(" ) ")
                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY anno_validita DESC, Imprese_Progetti.Piva, Imprese_Progetti.Sa_cod, Imprese_Progetti.Appezza, Imprese_Progetti.Id_Reg, Imprese_Progetti.Progetto_cod ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Imprese_Progetti.*, Reg_Impianti.Sup_Imp, Reg_Impianti.Cul_Cod, ISNULL(Reg_Impianti.Grfi_Cod,0) as Grfi_Cod_Impianto, Appezzamento.Appezza, Campi.Campo_Cod,  ")
                    StrSQL.AppendLine(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto  ")
                    StrSQL.AppendLine(" FROM Imprese_Progetti, Reg_Impianti, Appezzamento ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Campi ON ")
                    StrSQL.AppendLine(" (Appezzamento.Piva = Campi.Piva and Appezzamento.Sa_Cod = Campi.Sa_Cod ")
                    StrSQL.AppendLine(" And Appezzamento.Campo_Cod = Campi.Campo_Cod) ")

                    StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Piva = Reg_Impianti.Piva")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Appezza = Reg_Impianti.Appezza ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
                    StrSQL.AppendLine(" AND   Appezzamento.Piva = Reg_Impianti.Piva ")
                    StrSQL.AppendLine(" AND   Appezzamento.Sa_Cod = Reg_Impianti.Sa_Cod ")
                    StrSQL.AppendLine(" AND   Appezzamento.Appezza = Reg_Impianti.Appezza ")

                    If Progetto_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Distinta rr where rr.From_Piva = Imprese_Progetti.piva and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and ( ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Imprese_Progetti.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) or ")
                            StrSQL.AppendLine(" exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Distinta rr ")
                            StrSQL.AppendLine("    inner join reg_impianti_codici ric ")
                            StrSQL.AppendLine("    on rr.From_Piva = ric.piva and rr.From_Progetto_Cod = ric.Progetto_Cod ")
                            StrSQL.AppendLine("    where rr.DataInvio < ric.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = Imprese_Progetti.piva  ")
                            StrSQL.AppendLine("    and rr.From_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                            StrSQL.AppendLine(" ) ")
                            StrSQL.AppendLine(" ) ")
                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato >= 0 ")
                            If Not isGias2Gias Then
                                StrSQL.AppendLine(" AND     Reg_Impianti.Inviato >= 0 ")
                                StrSQL.AppendLine(" AND     Appezzamento.Inviato >= 0 ")
                            End If

                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     Imprese_Progetti.Inviato = -1 ")
                            If Not isGias2Gias Then
                                StrSQL.AppendLine(" AND     Reg_Impianti.Inviato = -1 ")
                                StrSQL.AppendLine(" AND     Appezzamento.Inviato = -1 ")
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
                        StrSQL.AppendLine(" ORDER BY Imprese_Progetti.Piva ASC, Imprese_Progetti.Progetto_Cod ASC, Imprese_Progetti.Progetto_Nome ASC ")
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


    '#####################################################################
    Public Function LeggiImpiantoDistinta(ByVal PIVA As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal Appezza As Int32,
                                          ByVal Id_Reg As Int32,
                                          ByVal Progetto_Cod_meno1tutti As Int32,
                                          ByVal Cul_Cod As Int32,
                                          ByVal Data_Inizio As Date,
                                          ByVal Data_Fine As Date,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiImpiantoDistinta()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Reg_impianti.PIVA, Reg_impianti.sa_cod, Reg_impianti.appezza, Reg_impianti.Id_Reg, Reg_Impianti.Sup_Imp, Reg_Impianti.cul_cod, Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine, progetto_cod ")
            StrSQL.AppendLine(" FROM  Reg_impianti ")
            StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ON Imprese_Progetti.PIVA = Reg_impianti.PIVA and Imprese_Progetti.sa_cod = Reg_Impianti.sa_cod and Imprese_Progetti.appezza =  Reg_Impianti.appezza and Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg")
            StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Reg_impianti.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Reg_impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Reg_impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Reg_impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod_meno1tutti <> -1 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod_meno1tutti) & " ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.AppendLine(" AND Reg_impianti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Reg_Impianti.Piva ASC, Reg_Impianti.Sa_Cod ASC, Reg_Impianti.Appezza ASC, Reg_Impianti.Id_reg ASC, Imprese_Progetti.validita_fine DESC ")
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



    '============================================================================
    ''' default Cau_Progetto As String = "9100", _
    Public Function LeggiDistinta2(ByVal PIVA As String,
                                   ByVal Sa_Cod As Long,
                                   ByVal Appezza As Long,
                                   ByVal Id_Reg As Long,
                                   ByVal Cau_Progetto As String,
                                   ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT Imprese_Progetti.*, Reg_Impianti.Grfi_Cod AS Finalita ")
                    StrSQL.AppendLine(" FROM  Imprese_Progetti, Reg_Impianti ")
                    StrSQL.AppendLine(" WHERE Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Imprese_Progetti.Piva ASC, Imprese_Progetti.Progetto_Cod ASC, Imprese_Progetti.Progetto_Nome ASC ")
                    End If
                    '---------------------------------------------

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


    '============================================================================
    ''' default Cau_Progetto As String = "9100", _
    Public Function LeggiDistinta(ByVal PIVA As String,
                                  ByVal Sa_Cod As Long,
                                  ByVal Appezza As Long,
                                  ByVal Id_Reg As Long,
                                  ByVal Cau_Progetto As String,
                                  ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Distinct Imprese_Progetti.*, GruppoFinalita.Grfi_Des ")
                    StrSQL.AppendLine(" FROM  Imprese_Progetti Left Outer Join GruppoFinalita On (Imprese_Progetti.Grfi_Cod = GruppoFinalita.Grfi_Cod ) ")
                    StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Cau_Progetto <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(Cau_Progetto) & "'   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Imprese_Progetti.Piva ASC, Imprese_Progetti.Progetto_Cod ASC, Imprese_Progetti.Progetto_Nome ASC ")
                    End If
                    '---------------------------------------------
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



    Public Function LeggiDistinta_Attiva_inData(ByVal PIVA As String,
                                                ByVal Sa_Cod As Long,
                                                ByVal Appezza As Long,
                                                ByVal Id_Reg As Long,
                                                ByVal Data_Operazione As Date,
                                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiDistinta_Attiva_inData()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Imprese_Progetti.* ")
                    StrSQL.AppendLine(" FROM  Imprese_Progetti ")
                    StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Operazione) & " ")
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Operazione) & " ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If
                    '---------------------------------------------


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


    Public Function LeggiDistinta_Attiva_inDataxAnagrafica(ByVal PIVA As String,
                                                           ByVal Sa_Cod As Long,
                                                           ByVal Appezza As Long,
                                                           ByVal Id_Reg As Long,
                                                           ByVal Data_Operazione As Date,
                                                           ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiDistinta_Attiva_inData()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Imprese_Progetti.* ")
                    StrSQL.AppendLine(" , ISNULL(kpin.val_cod, '') as cod_kpin ")
                    StrSQL.AppendLine(" , ISNULL(blockName.val_cod, '') as cod_block ")
                    StrSQL.AppendLine(" , ISNULL(GruppoFinalita.Grfi_Des, '') as stato_impianto_des ")
                    StrSQL.AppendLine(" , ISNULL(Regolamenti.Reg_Des, '') as regolamento ")
                    StrSQL.AppendLine(" FROM  Imprese_Progetti ")
                    StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici kpin ON Imprese_Progetti.Progetto_Cod = kpin.Progetto_Cod AND kpin.id_cod = " & enum_CodiciAnagrafe.Zespri_Codice_kPIN & " ")
                    StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici blockName ON Imprese_Progetti.Progetto_Cod = blockName.Progetto_Cod AND blockName.id_cod = " & enum_CodiciAnagrafe.Zespri_Block_Name & " ")
                    StrSQL.AppendLine(" LEFT JOIN GruppoFinalita ON Imprese_Progetti.Stato_Impianto = GruppoFinalita.Grfi_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod ")
                    StrSQL.AppendLine(" WHERE 1 = 1 ")

                    If Data_Operazione <> AGRODATAINIZIO Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Operazione) & " ")
                        StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Operazione) & " ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If
                    '---------------------------------------------


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
    Public Sub Leggi_Macroelementi_Impostati(ByVal Piva As String,
                                             ByVal SaCod As Integer,
                                             ByVal Appezza As Integer,
                                             ByVal IdReg As Integer,
                                             ByVal Progetto_Cod As Integer,
                                             ByRef N As String,
                                             ByRef N_Org As String,
                                             ByRef P2O5 As String,
                                             ByRef K2O As String,
                                             ByRef MgO As String,
                                             ByRef Regolamento_Concimazioni_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             )

        Dim Dt As DataTable

        Dim objRegImpCod As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

        Regolamento_Concimazioni_Cod = 0

        'Recupero le informazioni
        Dt = objRegImpCod.LeggixProgetto(CStr(Piva),
                                    CInt(SaCod),
                                    CInt(Appezza),
                                    CInt(IdReg),
                                    "", CInt(Progetto_Cod),
                                    0, "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "", objParametri)


        'Elimino gli oggetti COM
        objRegImpCod = Nothing

        Dim Dr As DataRow()

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            'Azoto
            Dr = Dt.Select("id_cod = 1050")

            If Dr.Length > 0 Then
                If IsNumeric(Dr(0)("val_cod")) Then
                    N = Dr(0)("val_cod")
                End If
            End If


            'Azoto
            Dr = Dt.Select("id_cod = 1316")

            If Dr.Length > 0 Then
                If IsNumeric(Dr(0)("val_cod")) Then
                    N_Org = Dr(0)("val_cod")
                End If
            End If

            'Potassio
            Dr = Dt.Select("id_cod = 1051")

            If Dr.Length > 0 Then
                If IsNumeric(Dr(0)("val_cod")) Then
                    P2O5 = Dr(0)("val_cod")
                End If
            End If


            'Fosforo
            Dr = Dt.Select("id_cod = 1052")

            If Dr.Length > 0 Then
                If IsNumeric(Dr(0)("val_cod")) Then
                    K2O = Dr(0)("val_cod")
                End If
            End If

            'Magnesio
            Dr = Dt.Select("id_cod = 1053")

            If Dr.Length > 0 Then
                If IsNumeric(Dr(0)("val_cod")) Then
                    MgO = Dr(0)("val_cod")
                End If
            End If

            If Not IsDBNull(Dt.Rows(0).Item("Regolamento_Concimazioni_Cod")) Then
                Regolamento_Concimazioni_Cod = Dt.Rows(0).Item("Regolamento_Concimazioni_Cod")
            End If


        End If

        'Elimino il recordset
        Dt = Nothing

    End Sub


    '#############################################################################################################
    Public Function CodProgetto_from_DataValidita(ByVal Piva As String,
                                                  ByVal SaCod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal IdReg As Integer,
                                                  ByVal Data_Validita As Date,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As Integer

        Dim Dt As DataTable
        Dim Cod_Progetto As Integer = 0
        Dim Filtro_Agg As String

        Filtro_Agg = " ( Imprese_Progetti.validita_inizio <= " & Agro_SQL_SaveDate(Data_Validita) &
                        " AND Imprese_Progetti.validita_fine >= " & Agro_SQL_SaveDate(Data_Validita) & " )"

        Dt = Leggi(CStr(Piva),
                    0,
                    CAU_PROGETTO_PRODUZIONE,
                    0,
                    CInt(SaCod),
                    CInt(Appezza),
                    CInt(IdReg),
                    0,
                    0,
                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    Filtro_Agg, "", objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            Cod_Progetto = Dt.Rows(0).Item("Progetto_Cod")
        End If

        Return Cod_Progetto


    End Function



    '============================================================================
    'Legge la distinta, l'impianto, l'appezzamento, specie vegetali e cultivar
    'DEFAULT PER I LIMITI NPK: -99999999
    Public Function LeggiDistinta3(ByVal PIVA As String,
                                   ByVal Sa_Cod As Long,
                                   ByVal Appezza As Long,
                                   ByVal Id_Reg As Long,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   Optional joinMovDestinazioni As Boolean = False,
                                   Optional lista_Id_Agenda As String = ""
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiDistinta3()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim distinct_str As String = ""
        If joinMovDestinazioni Then
            distinct_str = "DISTINCT"
        End If
        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT " & distinct_str & " Imprese_Progetti.Piva, Imprese_Progetti.Progetto_Cod, Progetto_Nome, Progetto_Des, Cau_Progetto, Imprese_Progetti.Sa_Cod, Imprese_Progetti.Appezza, Imprese_Progetti.Id_Reg, Imprese_Progetti.Validita_Inizio, Imprese_Progetti.Validita_Fine,  ")
            StrSQL.AppendLine(" Imprese_Progetti.Regolamento_Cod,Imprese_Progetti.Disciplinare_Cod, Disciplinare_PubblicoPrivato, Regolamento_Concimazioni_Cod, Stato_Impianto, ")
            StrSQL.AppendLine(" Imprese_Progetti.P_HA, Produzione_Prevista, Data_Fine_Prevista, Data_Inizio_Prevista,  Data_Fioritura_Prevista, ")
            StrSQL.AppendLine(" Appezzamento.App_Nome, Appezzamento.Sup_app, Appezzamento.Validita_Inizio AS Inizio_Appezza, Appezzamento.Validita_Fine AS Fine_Appezza, ")
            StrSQL.AppendLine(" Reg_Impianti.Sup_Imp, Reg_Impianti.Cul_Cod, Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto,  ")

            StrSQL.AppendLine("    ISNULL(Cultivar.Cul_Cod, 0) As Cul_Cod,")
            StrSQL.AppendLine("    ISNULL(Cultivar.Cul_Des, '') AS Cul_Des,")
            StrSQL.AppendLine("    ISNULL(SpecieVegetali.Veg_Cod, 0) As veg_cod,")
            StrSQL.AppendLine("    ISNULL(SpecieVegetali.Veg_Des, '') AS veg_des,")

            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            StrSQL.AppendLine(" Imprese_Progetti.P_HA_Femmine, Imprese_Progetti.P_HA_Maschi,")
            StrSQL.AppendLine(" Imprese_Progetti.FlagSecondoRaccolto")
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato) & " ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), '') AS capitolato_Privato ")

            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) & " ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), '') AS Magazzino_Conferimento ")

            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteN) & " ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_N ")
            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteP) & " ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_P ")
            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteK) & " ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_K ")
            StrSQL.AppendLine(" , ISNULL( (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("         FROM Reg_Impianti_Codici ")
            StrSQL.AppendLine("         WHERE Reg_Impianti_Codici.Id_Cod = " & CStr(enum_CodiciAnagrafe.Impianto_LimiteMg) & " ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.Appezza  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  ")
            StrSQL.AppendLine("         AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod  ")
            StrSQL.AppendLine("         ), -99999999) AS Limite_MG ")

            StrSQL.AppendLine(" FROM  Imprese_Progetti ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
            StrSQL.AppendLine(" INNER JOIN Appezzamento ON Reg_Impianti.Piva = Appezzamento.Piva And Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod And Reg_Impianti.Appezza = Appezzamento.Appezza ")
            StrSQL.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            If joinMovDestinazioni Then
                StrSQL.AppendLine(" LEFT JOIN Mov_Destinazioni mv  ")
                StrSQL.AppendLine(" ON Imprese_Progetti.Piva = mv.Piva AND Imprese_Progetti.Sa_Cod = mv.Sa_Cod AND Imprese_Progetti.Appezza = mv.Appezza AND Imprese_Progetti.Id_Reg = mv.Id_Destinazione ")
            End If

            StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(CAU_PROGETTO_PRODUZIONE) & "'   ")

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If joinMovDestinazioni Then
                StrSQL.AppendLine(" AND mv.ID_Agenda IN ( " & Agro_SQL_Save_Clausola_IN(lista_Id_Agenda) & " )")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Imprese_Progetti.Validita_fine DESC ")
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

    Public Function LeggiDistinta_ControlliAnagrafica(ByVal PIVA As String,
                                                      ByVal Sa_Cod As Integer,
                                                      ByVal Appezza As Integer,
                                                      ByVal Id_Reg As Integer,
                                                      ByVal Progetto_Cod As Integer,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiDistinta_ControlliAnagrafica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT")
            StrSQL.AppendLine("       Imprese_Progetti.Piva")
            StrSQL.AppendLine("     , Imprese_Progetti.Progetto_Cod")
            StrSQL.AppendLine("     , Imprese_Progetti.Sa_Cod")
            StrSQL.AppendLine("     , Imprese_Progetti.Appezza")
            StrSQL.AppendLine("     , Imprese_Progetti.Id_Reg")

            StrSQL.AppendLine("     , Progetto_Nome AS Lotto")

            StrSQL.AppendLine("     , Imprese_Progetti.Validita_Inizio")
            StrSQL.AppendLine("     , Imprese_Progetti.Validita_Fine")

            StrSQL.AppendLine("     , Appezzamento.App_Nome")

            StrSQL.AppendLine("     , Appezzamento.Validita_Inizio AS Inizio_Appezza")
            StrSQL.AppendLine("     , Appezzamento.Validita_Fine AS Fine_Appezza")

            StrSQL.AppendLine("     , Reg_Impianti.Cul_Cod")

            StrSQL.AppendLine("     , Reg_Impianti.Validita_Inizio AS Inizio_Impianto")

            StrSQL.AppendLine("     , Reg_Impianti.Validita_Fine AS Fine_Impianto")

            StrSQL.AppendLine("     , ISNULL(Cultivar.Cul_Des, '') AS Cul_Des")

            StrSQL.AppendLine("     , ISNULL(SpecieVegetali.Veg_Des, '') AS veg_des")

            StrSQL.AppendLine("     , ISNULL(DestinazioneUsoCod.id_cod, 0) AS id_cod ")
            StrSQL.AppendLine("     , ISNULL(DestinazioneUsoDes.descrizione, '') AS destinazioneUso ")

            StrSQL.AppendLine(" FROM Imprese_Progetti")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = Reg_Impianti.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg")
            StrSQL.AppendLine(" INNER JOIN Appezzamento ON ")
            StrSQL.AppendLine("     Reg_Impianti.Piva = Appezzamento.Piva")
            StrSQL.AppendLine(" AND Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod")
            StrSQL.AppendLine(" AND Reg_Impianti.Appezza = Appezzamento.Appezza")
            StrSQL.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici DestinazioneUsoCod ON ")
            StrSQL.AppendLine("     DestinazioneUsoCod.PIVA = Reg_Impianti.PIVA ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.sa_cod = Reg_Impianti.sa_cod ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.appezza = Reg_Impianti.appezza ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.ID_REG = Reg_Impianti.Id_Reg ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.id_cod >= 3000 and DestinazioneUsoCod.id_cod < 4000 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe DestinazioneUsoDes ON DestinazioneUsoDes.codice = DestinazioneUsoCod.id_cod ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If PIVA <> "" Then
                StrSQL.AppendLine($" AND Imprese_Progetti.Piva = '{Agro_SQL_SaveText(Trim(PIVA))}'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine($" And Imprese_Progetti.Sa_Cod = {Agro_SQL_SaveNum(Sa_Cod)}")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine($" And Imprese_Progetti.Appezza = { Agro_SQL_SaveNum(Appezza)}")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine($" And Imprese_Progetti.Id_Reg = { Agro_SQL_SaveNum(Id_Reg)}")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine($" And Imprese_Progetti.Progetto_Cod = { Agro_SQL_SaveNum(Progetto_Cod)}")
            End If

            StrSQL.AppendLine("ORDER BY Imprese_Progetti.Validita_fine DESC")

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
    Public Sub DatiAppezzaImpiantoDistinta_from_ChiaveImpianto(ByVal Piva As String,
                                                               ByVal SaCod As Integer,
                                                               ByVal Appezza As Integer,
                                                               ByVal IdReg As Integer,
                                                               ByRef Regolamento_Cod As Integer,
                                                               ByRef DPI_Cod As Integer,
                                                               ByVal Capitolato_Privato As String,
                                                               ByRef Inizio_Distinta As Date,
                                                               ByRef Fine_Distinta As Date,
                                                               ByRef Cul_Cod As Integer,
                                                               ByRef Cul_Des As String,
                                                               ByRef Veg_Cod As Integer,
                                                               ByRef Veg_des As String,
                                                               ByRef Sup_Imp As Decimal,
                                                               ByRef Inizio_Impianto As Date,
                                                               ByRef Fine_Impianto As Date,
                                                               ByRef App_Nome As String,
                                                               ByRef Sup_App As Decimal,
                                                               ByRef Inizio_appezza As Date,
                                                               ByRef Fine_Appezza As Date,
                                                               ByRef Disciplinare_PubblicoPrivato As Integer,
                                                               ByRef Regolamento_concimazioni_cod As Integer,
                                                               ByRef Magazzino_Conferimento As String,
                                                               ByRef Limite_N As Decimal,
                                                               ByRef Limite_P As Decimal,
                                                               ByRef Limite_K As Decimal,
                                                               ByRef Limite_Mg As Decimal,
                                                               ByRef Data_Semina_Prevista As Date,
                                                               ByRef Data_Fioritura_Prevista As Date,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                               )


        Dim Dt As DataTable

        Dt = LeggiDistinta3(CStr(Piva),
                            CInt(SaCod),
                            CInt(Appezza),
                            CInt(IdReg),
                            "", "",
                            objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            Regolamento_Cod = Dt.Rows(0).Item("Regolamento_Cod")
            DPI_Cod = Dt.Rows(0).Item("Disciplinare_Cod")
            Capitolato_Privato = Dt.Rows(0).Item("Capitolato_Privato")
            Inizio_Distinta = Dt.Rows(0).Item("Validita_Inizio")
            Fine_Distinta = Dt.Rows(0).Item("Validita_Fine")
            Cul_Cod = Dt.Rows(0).Item("Cul_Cod")
            Cul_Des = Dt.Rows(0).Item("Cul_Des")
            Veg_Cod = Dt.Rows(0).Item("Veg_Cod")
            Veg_des = Dt.Rows(0).Item("Veg_Des")
            Sup_Imp = Dt.Rows(0).Item("Sup_Imp")
            Inizio_Impianto = Dt.Rows(0).Item("Inizio_Impianto")
            Fine_Impianto = Dt.Rows(0).Item("Fine_Impianto")
            App_Nome = Dt.Rows(0).Item("App_Nome")
            Sup_App = Dt.Rows(0).Item("Sup_App")
            Inizio_appezza = Dt.Rows(0).Item("Inizio_appezza")
            Fine_Appezza = Dt.Rows(0).Item("Fine_Appezza")
            Disciplinare_PubblicoPrivato = Dt.Rows(0).Item("Disciplinare_PubblicoPrivato")
            Regolamento_concimazioni_cod = Dt.Rows(0).Item("Regolamento_concimazioni_cod")
            Magazzino_Conferimento = Dt.Rows(0).Item("Magazzino_Conferimento")
            Limite_N = Dt.Rows(0).Item("Limite_N")
            Limite_P = Dt.Rows(0).Item("Limite_P")
            Limite_K = Dt.Rows(0).Item("Limite_K")
            Limite_Mg = Dt.Rows(0).Item("Limite_Mg")
            Data_Semina_Prevista = Dt.Rows(0).Item("Data_Inizio_Prevista")
            Data_Fioritura_Prevista = Dt.Rows(0).Item("Data_Fioritura_Prevista")
        End If

    End Sub






    '============================================================================
    ''' default Cau_Progetto As String = "9100", _
    Public Function Leggi_ConRegolamenti(ByVal PIVA As String,
                                         ByVal Sa_Cod As Long,
                                         ByVal Appezza As Long,
                                         ByVal Id_Reg As Long,
                                         ByVal Cau_Progetto As String,
                                         ByVal Data As Date,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.Leggi_ConRegolamenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Imprese_Progetti.*, ISNULL(Regolamenti.Reg_Des,'') AS Reg_Des, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des ")
            StrSQL.AppendLine(" FROM   Imprese_Progetti LEFT OUTER JOIN ")
            StrSQL.AppendLine("    Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod LEFT OUTER JOIN  ")
            StrSQL.AppendLine("    GruppoFinalita ON Imprese_Progetti.Stato_Impianto = GruppoFinalita.Grfi_Cod  ")

            StrSQL.AppendLine(" ")


            If Data <> Nothing Then
                StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            Else
                StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            End If
            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Cau_Progetto <> "" Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(9100) & "'   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If


            '--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Imprese_Progetti.Piva ASC, Imprese_Progetti.Progetto_Cod ASC, Imprese_Progetti.Progetto_Nome ASC ")
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








    '============================================================================
    ''' default Cau_Progetto As String = "9100", _
    Public Function Progetto_from_PivaSaCodAppezzaIdreg(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Appezza As Integer,
                                                        ByVal Id_Reg As Integer,
                                                        ByVal Data As Date,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.Progetto_from_PivaSaCodAppezzaIdreg()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" SELECT Imprese_Progetti.*, ISNULL(Regolamenti.Reg_Des,'') AS Reg_Des, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des ")
            StrSQL.AppendLine(" FROM   Imprese_Progetti LEFT OUTER JOIN ")
            StrSQL.AppendLine(" Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine(" GruppoFinalita ON Imprese_Progetti.Stato_Impianto = GruppoFinalita.Grfi_Cod ")


            If Data <> Nothing Then
                StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            Else
                StrSQL.AppendLine(" WHERE Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                StrSQL.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            End If
            If Piva <> "" Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If CAU_PROGETTO <> "" Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Cau_Progetto = '" & Agro_SQL_SaveText(9100) & "'   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If


            '--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Imprese_Progetti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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

    Function Progetto_Des_From_Progetto_Cod(ByVal Piva As String,
                                            ByVal Progetto_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String


        Dim dt As DataTable


        dt = Leggi(Piva, Progetto_Cod, "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)


        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Progetto_Des")
            Else
                Return ""
            End If
        Else
            Return ""
        End If


    End Function



    Function Progetto_Nome_From_Progetto_Cod(ByVal Piva As String,
                                             ByVal Progetto_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As String


        Dim dt As DataTable


        dt = Leggi(Piva, Progetto_Cod, "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)


        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Progetto_Nome")
            Else
                Return ""
            End If
        Else
            Return ""
        End If


    End Function


    Function P_Ha_From_Progetto_Cod(ByVal Piva As String,
                                    ByVal Progetto_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Decimal


        Dim dt As DataTable


        dt = Leggi(Piva, Progetto_Cod, "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)


        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("P_Ha")
            Else
                Return 0
            End If
        Else
            Return 0
        End If


    End Function

    '#########################################################
    'filtra anche il cau_progetto
    Function ProgettoNome_From_ProgettoCod_2(ByVal Piva As String,
                                            ByVal Progetto_Cod As Integer,
                                            ByVal Cau_Progetto As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String


        Dim dt As DataTable


        dt = Leggi(Piva,
                    Progetto_Cod,
                     Cau_Progetto,
                     0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)


        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Progetto_Nome")
            Else
                Return ""
            End If
        Else
            Return ""
        End If


    End Function


    '################################################################################
    Public Function Esistono_Distinte_Su_Impianto(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal Id_Reg As Integer,
                                                  ByVal Cod_Progetto As Integer,
                                                  ByVal Validita_Inizio As Date,
                                                  ByVal Validita_Fine As Date,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As Boolean

        Dim Dt As DataTable
        Dim Dr As DataRow()

        'Dim Connessione As DbConnection

        Dim ObjProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

        ' APRO LA CONNESSIONE AL DATABASE



        'Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        'objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))



        objParametri.ImpostaFinestre_con_SalvataggioTemporale(Validita_Inizio, Validita_Fine)

        Dt = ObjProgetto.Leggi(CStr(Piva),
                                    0,
                                    CInt(9100),
                                    0,
                                    CInt(Sa_Cod),
                                    CInt(Appezza),
                                    CInt(Id_Reg),
                                    0, 0,
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        "", "", objParametri)
        objParametri.ResettaFinestra()

        'Connessione.Close()

        ObjProgetto = Nothing

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            Dr = Dt.Select("Progetto_Cod <> " & Cod_Progetto)

            If Dr IsNot Nothing AndAlso Dr.Length > 0 Then
                Return True
            Else
                Return False
            End If

        Else
            Return False

        End If


    End Function



    '################################################################################
    Public Function Trova_Distinte_Attive(ByVal piva As String,
                                          ByVal sa_cod As Integer,
                                          ByVal appezza As Integer,
                                          ByVal id_reg As Integer,
                                          ByVal data_movimento As Date,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Dim gefutils As New Gias_EF_Utility
        Dim dtDistinteAttive As DataTable
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ContaDistinte =
               From Reg_Impianti In GiasContext.Reg_Impianti
               Join Imprese_Progetti In GiasContext.Imprese_Progetti
                 On Reg_Impianti.PIVA Equals Imprese_Progetti.Piva And
                     Reg_Impianti.SA_COD Equals Imprese_Progetti.Sa_Cod And
                     Reg_Impianti.APPEZZA Equals Imprese_Progetti.Appezza And
                     Reg_Impianti.ID_REG Equals Imprese_Progetti.Id_Reg
               Group Join Reg_Impianti_Distinta In GiasContext.Reg_Impianti_Codici.Where(Function(x) x.id_cod = CInt(enum_CodiciAnagrafe.Distinta_Chiusa))
                 On Reg_Impianti_Distinta.PIVA Equals Imprese_Progetti.Piva And
                    Reg_Impianti_Distinta.sa_cod Equals Imprese_Progetti.Sa_Cod And
                   Reg_Impianti_Distinta.appezza Equals Imprese_Progetti.Appezza And
                   Reg_Impianti_Distinta.Id_Reg Equals Imprese_Progetti.Id_Reg And
                   Reg_Impianti_Distinta.Progetto_Cod Equals Imprese_Progetti.Progetto_Cod
                    Into Reg_Impianti_Distinta_Group = Group
               From _Reg_Impianti_Distinta_Group In Reg_Impianti_Distinta_Group.DefaultIfEmpty()
               Where Reg_Impianti.PIVA.Equals(piva) AndAlso
                     Reg_Impianti.SA_COD.Equals(sa_cod) AndAlso
                     Reg_Impianti.APPEZZA.Equals(appezza) AndAlso
                     Reg_Impianti.ID_REG.Equals(id_reg) AndAlso
                     Imprese_Progetti.Validita_Fine >= data_movimento
               Select New With {
                  .Piva = Reg_Impianti.PIVA,
                  .Sa_Cod = Reg_Impianti.SA_COD,
                  .Appezza = Reg_Impianti.APPEZZA,
                  .Id_Reg = Reg_Impianti.ID_REG,
                  .Progetto_Cod = Imprese_Progetti.Progetto_Cod,
                  .Validita_Inizio_Distinta = Imprese_Progetti.Validita_Inizio,
                  .Validita_Fine_Distinta = Imprese_Progetti.Validita_Fine,
                  .Flag_Distinta_Chiusa = If(_Reg_Impianti_Distinta_Group Is Nothing OrElse _Reg_Impianti_Distinta_Group.val_cod = 0, False, True)
              }

            dtDistinteAttive = gefutils.ObjectQueryToDataTable(ContaDistinte.Distinct().Where(Function(x) Not x.Flag_Distinta_Chiusa).ToList())

        End Using

        Return dtDistinteAttive

    End Function


    Public Function Leggi_x_anagrafica(ByVal Piva As String,
                                       ByVal Sa_Cod As Int32,
                                       ByVal Appezza As Int32,
                                       ByVal Id_Reg As Int32,
                                       ByVal Campo_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional dataAtt As Date = Nothing
                                       ) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.Impresa_Progetti_R.Leggi_x_anagrafica()"

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

            StrSQL.AppendLine(" SELECT   ")
            StrSQL.AppendLine(" r.PIVA + '_' + CAST(r.SA_COD AS nvarchar(10)) + '_' + CAST(r.APPEZZA AS nvarchar(10)) + '_' + CAST(r.id_reg AS nvarchar(10)) + '_' +  CAST(Imprese_Progetti.Progetto_Cod AS nvarchar(10)) AS chiave,   ")
            StrSQL.AppendLine(" a.app_nome,  ")
            StrSQL.AppendLine(" r.PIVA, r.SA_COD, r.APPEZZA, r.id_Reg,  ")
            StrSQL.AppendLine(" c.cul_des, r.cul_cod, s.veg_des, s.veg_cod, gru_des, r.GRFI_COD, gf.Grfi_Des, r.GRVA_Cod_VEG, grva_des, r.p_ha, r.sup_imp,  ")
            StrSQL.AppendLine(" Imprese_Progetti.Validita_Inizio, Imprese_Progetti.Validita_Fine, (select [User] from utenti where CODICE_FISCALE = r.Username_Modifica ) as utente_modifica , r.Data_Modifica,  ")
            StrSQL.AppendLine(" (select [User] from utenti where CODICE_FISCALE = r.Username_Creazione ) as utente_creazione , r.Data_Creazione,  ")
            StrSQL.AppendLine(" ca.sa_cod, ca.sa_nome,  ")
            StrSQL.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,  ")
            StrSQL.AppendLine(" codiceImpianto.val_cod as Codice_Impianto,  ")
            StrSQL.AppendLine(" CASE WHEN tra_fila_m.val_cod is null THEN '0' ELSE CASE WHEN tra_fila_m.val_cod = ''  THEN '0' ELSE tra_fila_m.val_cod END END as tra_fila_m,  ")
            StrSQL.AppendLine(" CASE WHEN  su_fila_m.val_cod is null THEN '0' ELSE CASE WHEN  su_fila_m.val_cod = ''  THEN '0' ELSE  su_fila_m.val_cod END END as  su_fila_m,  ")
            StrSQL.AppendLine(" r.port_cod, Portinnesti.port_des,  ")
            StrSQL.AppendLine(" r.foral_cod, FormeAllevamento.foral_des,  ")
            StrSQL.AppendLine(" r.Setup_Cod,  ")
            StrSQL.AppendLine(" r.cop_cod, Copertura.Cop_Des,  ")
            StrSQL.AppendLine(" r.COVER, r.MONITORATO,  ")
            StrSQL.AppendLine(" a.Blk_Flag  ")
            StrSQL.AppendLine(" ,ISNULL(Data_Inizio_Portinnesto.val_Cod, '') as Data_Inizio_Portinnesto  ")
            StrSQL.AppendLine(" , Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" , Imprese_Progetti.Progetto_Nome ")
            StrSQL.AppendLine(" , Imprese_Progetti.Progetto_Des ")
            StrSQL.AppendLine(" , Imprese_Progetti.Produzione_Prevista as [Resa] ")
            StrSQL.AppendLine(" , ISNULL(kpin.val_cod, '') AS cod_kpin ")
            StrSQL.AppendLine(" , ISNULL(blockName.val_cod, '') AS cod_block ")
            StrSQL.AppendLine(" , ISNULL(grower.val_cod, '') AS cod_grower ")
            StrSQL.AppendLine(" , CASE distintaChiusa.val_cod WHEN '1' THEN 'SI' ELSE 'NO' END AS [Distinta_Chiusa] ")
            StrSQL.AppendLine(" , CASE Imprese_Progetti.FlagSecondoRaccolto WHEN '1' THEN 'SI' ELSE 'NO' END AS [FlagSecondoRaccolto] ")
            StrSQL.AppendLine(" , CASE r.cul_Cod WHEN 0 THEN  ISNULL(Codici_Anagrafe.descrizione, 'Terreno Nudo') ELSE s.Veg_Des END AS [Utilizzo] ")

            'StrSQL.AppendLine(", Imprese_Progetti.Data_Inizio_Prevista AS Data_Semina")
            'StrSQL.AppendLine(", Imprese_Progetti.Data_Fine_Prevista AS Data_Raccolta")
            'StrSQL.AppendLine(", Imprese_Progetti.Data_Fioritura_Prevista AS Data_Fioritura")
            StrSQL.AppendLine(", CASE WHEN Imprese_Progetti.Data_Fioritura_Prevista IS NULL THEN CAST('2100-12-31' AS date) ELSE Imprese_Progetti.Data_Fioritura_Prevista END AS Data_Fioritura_Prevista")
            StrSQL.AppendLine(", CASE WHEN Imprese_Progetti.Data_Inizio_Prevista IS NULL THEN CAST('1900-01-01' AS date) ELSE Imprese_Progetti.Data_Inizio_Prevista END AS Data_Semina_Prevista")
            StrSQL.AppendLine(", CASE WHEN Imprese_Progetti.Data_Fine_Prevista IS NULL THEN CAST('2100-12-31' AS date) ELSE Imprese_Progetti.Data_Fine_Prevista END AS Data_Raccolta_Prevista")

            StrSQL.AppendLine(" FROM Imprese_Progetti  ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti r ON Imprese_Progetti.Piva = r.PIVA AND Imprese_Progetti.Sa_Cod = r.SA_COD AND Imprese_Progetti.Appezza = r.APPEZZA AND  Imprese_Progetti.id_reg = r.id_reg ")
            StrSQL.AppendLine(" INNER JOIN Appezzamento a on a.piva  = r.piva and a.sa_cod = r.SA_COD and a.APPEZZA = r.APPEZZA   ")
            StrSQL.AppendLine(" LEFT JOIN cultivar c on c.Cul_Cod = r.CUL_COD   ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali s on s.Veg_Cod = c.veg_cod   ")
            StrSQL.AppendLine(" LEFT JOIN GruppoVegetale g on g.gru_cod = s.Gru_Cod   ")
            StrSQL.AppendLine(" LEFT JOIN GruppoFinalita gf on gf.Grfi_Cod = r.GRFI_COD  ")
            StrSQL.AppendLine(" LEFT JOIN GruppoVarietale gv on gv.Grva_Cod = r.GRVA_Cod_VEG   ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca on r.sa_cod = ca.sa_cod AND r.piva = ca.piva   ")
            StrSQL.AppendLine(" LEFT JOIN Campi on a.sa_cod = Campi.sa_cod AND a.piva = Campi.piva AND a.Campo_Cod = Campi.Campo_Cod  ")
            StrSQL.AppendLine(" LEFT JOIN Portinnesti on r.port_cod = Portinnesti.port_cod  ")
            StrSQL.AppendLine(" LEFT JOIN FormeAllevamento on r.foral_cod = FormeAllevamento.foral_cod  ")
            StrSQL.AppendLine(" LEFT JOIN Copertura on r.Cop_Cod = Copertura.Cop_Cod  ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici codiceImpianto ON r.PIVA = codiceImpianto.PIVA AND r.Sa_Cod = codiceImpianto.Sa_Cod AND r.Appezza = codiceImpianto.Appezza AND r.ID_Reg = codiceImpianto.ID_Reg AND codiceImpianto.id_Cod = 1300  ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici tra_fila_m ON r.PIVA = tra_fila_m.PIVA AND r.Sa_Cod = tra_fila_m.Sa_Cod AND r.Appezza = tra_fila_m.Appezza AND r.ID_Reg = tra_fila_m.ID_Reg AND tra_fila_m.id_Cod = 1061  ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici su_fila_m ON r.PIVA = su_fila_m.PIVA AND r.Sa_Cod = su_fila_m.Sa_Cod AND r.Appezza = su_fila_m.Appezza AND r.ID_Reg = su_fila_m.ID_Reg AND su_fila_m.id_Cod = 1063  ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici Data_Inizio_Portinnesto ON r.PIVA = Data_Inizio_Portinnesto.PIVA AND r.Sa_Cod = Data_Inizio_Portinnesto.Sa_Cod AND r.Appezza = Data_Inizio_Portinnesto.Appezza AND r.ID_Reg = Data_Inizio_Portinnesto.ID_Reg AND Data_Inizio_Portinnesto.id_Cod = 1328  ")
            StrSQL.AppendLine(" LEFT JOIN ImpiantiIrrigazioni on r.Imp_Cod = ImpiantiIrrigazioni.Imp_Cod  ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici kpin ON Imprese_Progetti.Progetto_Cod = kpin.Progetto_Cod AND kpin.id_cod = 1287 ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici blockName ON Imprese_Progetti.Progetto_Cod = blockName.Progetto_Cod AND blockName.id_cod = 1288 ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici grower ON Imprese_Progetti.Progetto_Cod = grower.Progetto_Cod AND grower.id_cod = 1317 ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici distintaChiusa ON Imprese_Progetti.Progetto_Cod = distintaChiusa.Progetto_Cod AND distintaChiusa.id_cod = 1301 ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici Destinazione ON r.PIVA = Destinazione.PIVA AND r.Sa_Cod = Destinazione.Sa_Cod AND r.Appezza = Destinazione.Appezza AND r.ID_Reg = Destinazione.ID_Reg AND Destinazione.id_Cod >= 3000 AND Destinazione.id_cod < 4000 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe ON Destinazione.id_cod = Codici_Anagrafe.codice ")




            StrSQL.AppendLine(" WHERE   Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND    Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND     r.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            StrSQL.AppendLine(" AND     r.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND r.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
                        StrSQL.AppendLine(" AND r.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND r.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND r.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   R.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   R.Inviato =-1 ")
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

                StrSQL.AppendLine(" ORDER BY R.Validita_Inizio Desc ")
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

    Public Function Leggi_x_anagraficaNG(ByVal Piva As String,
                                         ByVal Sa_Cod As Long,
                                         ByVal Appezza As Long,
                                         ByVal Id_Reg As Long,
                                         ByVal Campo_Cod As Long,
                                         ByVal Codice_Fiscale_Tecnico As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional dataAtt As Date = Nothing,
                                         Optional leggiStaticMap As Boolean = False,
                                         Optional leggiDatiRibaltamento As Boolean = False,
                                         Optional readLinkedMachines As Boolean = False
                                         ) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.Impresa_Progetti_R.Leggi_x_anagrafica()"

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

        If major >= 14 Then
            sql2017 = True 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
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
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        Piva")
            StrSQL.AppendLine("        , Sa_Cod")
            StrSQL.AppendLine("        , Appezza")
            StrSQL.AppendLine("    INTO #AppezzamentiSingoli ")
            StrSQL.AppendLine("    FROM AppezzamentiXParticelle (NOLOCK)")
            StrSQL.AppendLine("    WHERE 1 = 1 ")

            If Piva <> "" Then
                StrSQL.AppendLine("    AND AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("    AND AppezzamentiXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
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
                        StrSQL.AppendLine("    AND AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                    End If
                End If
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine("    AND AppezzamentiXParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            StrSQL.AppendLine("    GROUP BY Piva, Sa_Cod, Appezza")
            StrSQL.AppendLine("    HAVING COUNT(*) = 1")
            'StrSQL.AppendLine(")")

            'StrSQL.AppendLine(", #AppezzamentiSingoliCompleti AS (")
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        #AppezzamentiSingoli.Piva")
            StrSQL.AppendLine("        , #AppezzamentiSingoli.SA_COD")
            StrSQL.AppendLine("        , #AppezzamentiSingoli.APPEZZA")
            StrSQL.AppendLine("        , AppezzamentixParticelle.PROV")
            StrSQL.AppendLine("        , Lista_Province.PROVINCIA")
            StrSQL.AppendLine("        , AppezzamentixParticelle.Com")
            StrSQL.AppendLine("        , ISTAT.Localita")
            StrSQL.AppendLine("        , Case AppezzamentixParticelle.SEZIONE When '0' THEN '' ELSE AppezzamentixParticelle.SEZIONE END AS 'SEZIONE'")
            StrSQL.AppendLine("        , AppezzamentixParticelle.FOGLIO")
            StrSQL.AppendLine("        , AppezzamentixParticelle.NUMERO")
            StrSQL.AppendLine("        , CASE AppezzamentixParticelle.SUBALTERNO WHEN '0' THEN '' ELSE AppezzamentixParticelle.SUBALTERNO END AS 'SUBALTERNO'")
            StrSQL.AppendLine("    INTO #AppezzamentiSingoliCompleti ")
            StrSQL.AppendLine("    FROM #AppezzamentiSingoli")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN AppezzamentixParticelle (nolock) ON #AppezzamentiSingoli.Piva = AppezzamentixParticelle.Piva")
            StrSQL.AppendLine("        AND #AppezzamentiSingoli.SA_COD = AppezzamentixParticelle.SA_COD")
            StrSQL.AppendLine("        AND #AppezzamentiSingoli.APPEZZA = AppezzamentixParticelle.APPEZZA")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN Lista_Province (nolock) ON AppezzamentixParticelle.PROV = Lista_Province.PROV")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN ISTAT (nolock) ON AppezzamentixParticelle.PROV = ISTAT.PROV AND AppezzamentixParticelle.COM = ISTAT.COM")
            StrSQL.AppendLine("        AND AppezzamentixParticelle.COM = ISTAT.COM")
            'StrSQL.AppendLine(") ")

            'VERIFICO CHE CI SIA ALMENO SQL 2017 PER UTILIZZARE STRING_AGG
            If sql2017 Then

                'StrSQL.AppendLine(", #AppezzaZVN AS (")
                StrSQL.AppendLine("    SELECT Piva, Sa_Cod, Appezza, STRING_AGG(ZVN, ',') AS ZVN ")
                StrSQL.AppendLine("    INTO #AppezzaZVN ")
                StrSQL.AppendLine("    FROM (")
                StrSQL.AppendLine("        SELECT DISTINCT ap.Piva, ap.sa_Cod, ap.Appezza, CASE WHEN zp.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN")
                StrSQL.AppendLine("        FROM AppezzamentiXParticelle ap (NOLOCK)")
                StrSQL.AppendLine("            LEFT JOIN ZonexParticelle zp (NOLOCK) ON ap.PROV = zp.PROV")
                StrSQL.AppendLine("                AND ap.COM = zp.COM")
                StrSQL.AppendLine("                AND ap.SEZIONE = zp.SEZIONE")
                StrSQL.AppendLine("                AND ap.FOGLIO = zp.FOGLIO")
                StrSQL.AppendLine("                AND ap.NUMERO = zp.NUMERO")
                StrSQL.AppendLine("                AND ap.SUBALTERNO = zp.SUBALTERNO")
                StrSQL.AppendLine("                AND zp.Zona_Cod = -17")

                StrSQL.AppendLine("        WHERE 1 = 1 ")

                If Piva <> "" Then
                    StrSQL.AppendLine("                AND ap.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                End If
                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine("                AND ap.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
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
                            StrSQL.AppendLine("                AND ap.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                        End If
                    End If
                End If

                If Appezza <> 0 Then
                    StrSQL.AppendLine("                AND ap.Appezza = " & Agro_SQL_SaveNum(Appezza))
                End If

                StrSQL.AppendLine("    ) a")
                StrSQL.AppendLine("    GROUP BY a.Piva, a.SA_COD, a.APPEZZA")
                'StrSQL.AppendLine(")")

            End If

            'StrSQL.AppendLine(")")
            'StrSQL.AppendLine(", #Campi AS (")
            StrSQL.AppendLine("    SELECT *")
            StrSQL.AppendLine("    INTO #Campi ")
            StrSQL.AppendLine("    FROM Campi (nolock)")
            StrSQL.AppendLine("    WHERE 1 = 1")
            If Piva <> "" Then
                StrSQL.AppendLine("        AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("        AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
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
                        StrSQL.AppendLine("        AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ")")
                    End If
                End If
            End If
            'StrSQL.AppendLine(")")

            If readLinkedMachines Then

                StrSQL.AppendLine("    SELECT axp.Piva, axp.Sa_Cod, axp.Appezza, STRING_AGG( ")
                StrSQL.AppendLine("	           '  ' + pm.Mac_Des +")
                StrSQL.AppendLine("                (")
                StrSQL.AppendLine("                    CASE ")
                StrSQL.AppendLine("                        WHEN CAST(axp.Validita_Inizio AS DATE) = '1900-01-01' ")
                StrSQL.AppendLine("                        THEN ''")
                StrSQL.AppendLine("                        ELSE ' : ' + CONVERT(VARCHAR(10), axp.Validita_Inizio, 121)")
                StrSQL.AppendLine("                    END +")
                StrSQL.AppendLine("                    CASE ")
                StrSQL.AppendLine("                        WHEN CAST(axp.Validita_Fine AS DATE) = '2100-12-31' ")
                StrSQL.AppendLine("                        THEN ''")
                StrSQL.AppendLine("                        ELSE ' - '  + CONVERT(VARCHAR(10), axp.validita_fine, 121)")
                StrSQL.AppendLine("                    END")
                StrSQL.AppendLine("                ), ' ') as LinkedMachines ")
                StrSQL.AppendLine("    INTO #AxP ")
                StrSQL.AppendLine("    FROM AppezzamentiXParcoMacchine axp")
                StrSQL.AppendLine("        JOIN Parco_Macchine pm ON pm.Mac_Cod = axp.Mac_Cod")
                If Piva <> "" Then
                    StrSQL.AppendLine("    WHERE axp.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                End If

                StrSQL.AppendLine("   GROUP BY axp.Piva, axp.Sa_Cod, axp.Appezza ")

            End If

            StrSQL.AppendLine("    SELECT pxc.ProgettoCod, pxc.Piva, STRING_AGG(c.ContributoDes, '  ') AS ContributoDes")
            StrSQL.AppendLine("    INTO #PxACA ")
            StrSQL.AppendLine("    FROM Imprese_ProgettiXContributi pxc")
            StrSQL.AppendLine("        JOIN Contributi c ON c.ContributoCod = pxc.ContributoCod")
            StrSQL.AppendLine("            AND c.Tipo = pxc.ContributoTipo")
            StrSQL.AppendLine($"    WHERE pxc.ContributoTipo = {CInt(ContributeType.ACA)}")
            If Piva <> "" Then
                StrSQL.AppendLine($"        AND pxc.Piva = '{Agro_SQL_SaveText(Piva)}'")
            End If
            StrSQL.AppendLine("    GROUP BY ProgettoCod, Piva")
            StrSQL.AppendLine("")

            StrSQL.AppendLine("SELECT   ")
            StrSQL.AppendLine("    r.PIVA + '_' + CAST(r.SA_COD AS nvarchar(10)) + '_' + CAST(r.APPEZZA AS nvarchar(10)) + '_' + CAST(r.id_reg AS nvarchar(10)) + '_' +  CAST(Imprese_Progetti.Progetto_Cod AS nvarchar(10)) AS chiave")
            StrSQL.AppendLine("    , r.PIVA, r.SA_COD, Centri_Aziendali.sa_nome, r.APPEZZA, r.id_Reg")
            StrSQL.AppendLine("    , a.app_nome")
            StrSQL.AppendLine("    , ISNULL(c.cul_des, '') as cul_des, ISNULL(r.cul_cod, 0) as cul_cod, ISNULL(s.veg_des, '') as veg_des, ISNULL(s.veg_cod, 0) as veg_cod, ISNULL(gru_des, '') as gru_des, ISNULL(r.GRFI_COD, 0) as GRFI_COD, ISNULL(gf.Grfi_Des, '') as Grfi_Des, ISNULL(r.GRVA_Cod_VEG, '') as GRVA_Cod_VEG, ISNULL(grva_des, '') as grva_des, r.p_ha, r.sup_imp")
            StrSQL.AppendLine("    , COALESCE(dettSpecie.val_cod, '') AS dettSpeciePersonalizzatoCod")
            StrSQL.AppendLine("    , COALESCE(cac.InfoAgg_Des, '') AS dettSpeciePersonalizzatoDes")
            StrSQL.AppendLine("    , Imprese_Progetti.Validita_Inizio, Imprese_Progetti.Validita_Fine, (select [User] from utenti where CODICE_FISCALE = r.Username_Modifica ) as utente_modifica , r.Data_Modifica")
            StrSQL.AppendLine("    , r.Validita_Inizio as Validita_Inizio_Impianto")
            StrSQL.AppendLine("    , r.Validita_Fine as Validita_Fine_Impianto")
            StrSQL.AppendLine("    , r.Data_Inizio_Impianto as Data_Inizio_Impianto")
            StrSQL.AppendLine("    , r.Data_Inizio_Produzione as Data_Inizio_Produzione")
            StrSQL.AppendLine("    , (select [User] from utenti where CODICE_FISCALE = r.Username_Creazione ) as utente_creazione , r.Data_Creazione")
            StrSQL.AppendLine("    , Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome")
            StrSQL.AppendLine("    , ISNULL(#Campi.Campo_Cod, 0) As Campo_Cod, ISNULL(#Campi.Campo_Des, '') As Campo_Des")
            StrSQL.AppendLine("    , codiceImpianto.val_cod as Codice_Impianto")
            StrSQL.AppendLine("    , CAST(REPLACE(CASE WHEN tra_fila_m.val_cod is null THEN '0' ELSE CASE WHEN tra_fila_m.val_cod = ''  THEN '0' ELSE tra_fila_m.val_cod END END, ',', '.') as float) as tra_fila_m")
            StrSQL.AppendLine("    , CAST(REPLACE(CASE WHEN  su_fila_m.val_cod is null THEN '0' ELSE CASE WHEN  su_fila_m.val_cod = ''  THEN '0' ELSE su_fila_m.val_cod END END, ',', '.') as float) as su_fila_m")
            StrSQL.AppendLine("    , ISNULL(r.port_cod, 0) as port_cod, ISNULL(Portinnesti.port_des, '') as port_des")
            StrSQL.AppendLine("    , ISNULL(r.foral_cod, 0) as foral_cod, ISNULL(FormeAllevamento.foral_des, '') as foral_des")
            StrSQL.AppendLine("    , r.Setup_Cod")
            StrSQL.AppendLine("    , ISNULL(r.cop_cod, 0) as cop_cod, ISNULL(Copertura.Cop_Des, '') as Cop_Des")
            StrSQL.AppendLine("    , r.COVER")
            StrSQL.AppendLine("    , r.MONITORATO")
            StrSQL.AppendLine("    , a.Blk_Flag")
            StrSQL.AppendLine("    , r.Data_Inizio_Portinnesto  ")
            StrSQL.AppendLine("    , Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine("    , Imprese_Progetti.Progetto_Nome ")
            StrSQL.AppendLine("    , Imprese_Progetti.Progetto_Des ")
            StrSQL.AppendLine("    , Imprese_Progetti.Produzione_Prevista as [Resa] ")
            StrSQL.AppendLine("    , Imprese_Progetti.Produzione_Prevista *  r.sup_imp as [ResaTotalePrevista]")
            StrSQL.AppendLine("    , Imprese_Progetti.Data_Inizio_Prevista as [DataTrapiantoSemina] ")
            StrSQL.AppendLine("    , datepart(WEEK, Imprese_Progetti.Data_Inizio_Prevista) as [SettimanaTrapiantoSemina] ")
            StrSQL.AppendLine("    , Imprese_Progetti.Data_Fine_Prevista as [DataRaccolta] ")
            StrSQL.AppendLine("    , datepart(WEEK, Imprese_Progetti.Data_Fine_Prevista) as [SettimanaRaccolta] ")
            StrSQL.AppendLine("    , Imprese_Progetti.Data_Fioritura_Prevista as [DataFioritura] ")
            StrSQL.AppendLine("    , SupBZ_Riduzione, DistBZ_CorpiIdrici, DistBZ_AreeResPub, DistBZ_Allevamenti, DistBZ_VegNatNonColt")
            StrSQL.AppendLine("    , ISNULL(kpin.val_cod, '') AS cod_kpin")
            StrSQL.AppendLine("    , ISNULL(blockName.val_cod, '') AS cod_block")
            StrSQL.AppendLine("    , ISNULL(grower.val_cod, '') AS cod_grower")
            StrSQL.AppendLine("    , ISNULL(organismoReferente.val_cod, '') AS organismo_Referente")
            StrSQL.AppendLine("    , ISNULL(organismoReferenteDesc.rag_soc, '') AS organismo_Referente_des")
            StrSQL.AppendLine("    , CASE distintaChiusa.val_cod WHEN '1' THEN 'SI' ELSE 'NO' END AS [Distinta_Chiusa]")
            StrSQL.AppendLine("    , COALESCE(codiceImpiantoRibaltato.val_cod, '') AS codiceImpiantoRibaltato")
            StrSQL.AppendLine("    , '' as replicaGiasPiva")
            StrSQL.AppendLine("    , '' as replicaGiasRagioneSociale")
            StrSQL.AppendLine("    , CASE r.cul_Cod WHEN 0 THEN  ISNULL(Codici_Anagrafe.descrizione, 'Terreno Nudo') ELSE s.Veg_Des + ' - ' + c.Cul_Des END AS [Utilizzo]")
            StrSQL.AppendLine("    , CASE r.cul_Cod WHEN 0 THEN  COALESCE(Codici_Anagrafe.descrizione, 'Terreno Nudo') ELSE '' END AS Destinazione_Uso_Des")
            StrSQL.AppendLine("    , CASE r.cul_Cod WHEN 0 THEN  COALESCE(Codici_Anagrafe.Codice, 0) ELSE 0 END AS Destinazione_Uso_Cod")
            StrSQL.AppendLine("    , CASE WHEN Imprese_Progetti.Validita_Inizio <= CAST(GETDATE() as date) AND Imprese_Progetti.validita_fine >= CAST(GETDATE() as date) THEN 1 ELSE 0 END as Attivo")
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
                StrSQL.AppendLine("    , (")
                StrSQL.AppendLine("         Select * from(select StaticMap as '*') Tbl")
                StrSQL.AppendLine("         For Xml path('')")
                StrSQL.AppendLine("    ) StaticMapBase64String")
                StrSQL.AppendLine("")
            End If

            If sql2017 Then
                StrSQL.AppendLine("    , #AppezzaZVN.ZVN")
            Else
                StrSQL.AppendLine("    , '' as ZVN")
            End If

            'metodoProd
            StrSQL.AppendLine("    , CAST(ISNULL(MetodoProd.Val_Cod, 1) as int) as Metodo_Produzione_Cod   ")

            StrSQL.AppendLine("    , CASE WHEN MetodoProd.Val_Cod = '1' THEN 'Integrato'")
            StrSQL.AppendLine("        WHEN MetodoProd.Val_Cod = '2' THEN 'In Conversione'")
            StrSQL.AppendLine("        WHEN MetodoProd.Val_Cod = '3' THEN 'Biologico'")
            StrSQL.AppendLine("        WHEN MetodoProd.Val_Cod is null THEN 'Integrato'")
            StrSQL.AppendLine("    END as Metodo_Produzione_Des")

            StrSQL.AppendLine("    , Imprese_Progetti.Regolamento_Cod")
            StrSQL.AppendLine("    , Regolamenti.Reg_Des")

            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(N.Val_Cod, 0), ',', '.') AS float) AS N")
            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(P.Val_Cod, 0), ',', '.') AS float) AS P ")
            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(K.Val_Cod, 0), ',', '.') AS float) AS K")
            StrSQL.AppendLine("    , CAST(REPLACE(ISNULL(Mg.Val_Cod, 0), ',', '.') AS float) AS Mg")

            StrSQL.AppendLine("    , COALESCE(interbina.val_Cod, '') as interbina")
            StrSQL.AppendLine("    , COALESCE(germinabilita.val_Cod, '100') as germinabilita")
            StrSQL.AppendLine("    , Imprese_Progetti.P_HA as pianteHa")
            StrSQL.AppendLine("    , ROUND(Imprese_Progetti.P_HA * r.Sup_Imp, 0) as pianteImpianto")

            StrSQL.AppendLine("    , CASE WHEN DPI_Regolamenti.COD_REGOLAMENTO IS NULL THEN CAST(Imprese_Progetti.Regolamento_Cod as varchar(100)) ELSE CAST(DPI_Regolamenti.Flag_Privato_Pubblico as varchar(10)) + '_' + CAST(DPI_Regolamenti.COD_REGOLAMENTO as varchar(10)) END as RegolamentoDisciplinare_Cod")
            StrSQL.AppendLine("    , CASE WHEN DPI_Regolamenti.NomeEsteso IS NULL THEN CASE WHEN Regolamenti.Reg_Cod = '4' THEN 'Bio' ELSE Regolamenti.Reg_Des END ELSE DPI_Regolamenti.NomeEsteso END as RegolamentoDisciplinare_Des")

            StrSQL.AppendLine("    , CAST(COALESCE(grfi_RER.Val_Cod, 0) as int) as Tipologia_Cod ")
            StrSQL.AppendLine("    , COALESCE(GruppoFinalita_Rer.GRFI_DES, '') as Tipologia_Des")
            StrSQL.AppendLine("    , COALESCE(Imprese_Progetti.Stato_Impianto, 0) as Stato_Impianto")
            StrSQL.AppendLine("    , COALESCE(FasiCicloColturale_Anagrafiche.fase_des, CASE WHEN Imprese_Progetti.Stato_Impianto = 102 THEN 'In produzione' ELSE '' END) as fase_des")
            StrSQL.AppendLine("    , COALESCE(Imprese_Progetti.Mat_Cod, 0) AS Mat_Cod")
            StrSQL.AppendLine("    , COALESCE(Materie_Prime.Mat_Des, '') AS Mat_Des")

            StrSQL.AppendLine("    , r.UDM_COD_ALT AS Udm_Cod_Alt")
            StrSQL.AppendLine("    , r.SUP_ALT AS Sup_Int_Alt")
            StrSQL.AppendLine("    , COALESCE(udmAlternative.UDM_DES, '') AS Udm_Des_Alt")
            StrSQL.AppendLine("    , COALESCE(Conversione_UnitaMisura_Alternative.Tasso_Conv, 0) AS Tasso_Conv")
            StrSQL.AppendLine("    , gr.GruppoRaccolta_Cod")
            StrSQL.AppendLine("    , gr.GruppoRaccolta_Des")

            If leggiDatiRibaltamento Then
                StrSQL.AppendLine("    , ISNULL(''''+ ba.Nome_Budget + ''' - ' +  CONVERT(VARCHAR(30), Data_Ribaltamento, 103) ,'') AS InfoRibaltamento  ")
            Else
                StrSQL.AppendLine("    , '' AS InfoRibaltamento")
            End If

            If readLinkedMachines Then
                StrSQL.AppendLine("	   , COALESCE(#AxP.LinkedMachines, '') AS LinkedMachines ")
            End If

            StrSQL.AppendLine("	   , pxa.ContributoDes AS LinkedACAContributes")

            StrSQL.AppendLine("")
            StrSQL.AppendLine("    , a.Agea_codiBarrScheVali")
            StrSQL.AppendLine("    , a.Agea_idAppezzamentoOrig")
            StrSQL.AppendLine("    , a.Agea_identificativoAppezzamento")
            StrSQL.AppendLine("    , a.Agea_identificativoIsola")
            StrSQL.AppendLine("    , a.Agea_identificativoPianoColtivazione")
            StrSQL.AppendLine("    , a.Agea_idSchedaValidazione")
            StrSQL.AppendLine("    , r.Agea_idColt")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    , r.flagCessata")

            StrSQL.AppendLine("")
            StrSQL.AppendLine("    , COALESCE((SELECT TOP(1) descrizione FROM ClassiTessituraB WHERE id_classetessitura = a.CLAS), '') AS WeavingClass")
            StrSQL.AppendLine("    , COALESCE(a.ARGILLA, 0) AS Clay")
            StrSQL.AppendLine("    , COALESCE(a.SABBIA, 0) AS Sand")
            StrSQL.AppendLine("    , COALESCE(a.LIMO, 0) AS Silt")
            StrSQL.AppendLine("    , COALESCE(a.PENDE, 0) AS Slope")
            StrSQL.AppendLine("")

            StrSQL.AppendLine("FROM Imprese_Progetti (NOLOCK)")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Gruppi_Raccolta gr ON gr.GruppoRaccolta_Cod = Imprese_Progetti.GruppoRaccolta_Cod")
            StrSQL.AppendLine("    INNER JOIN Reg_Impianti r (NOLOCK) ON Imprese_Progetti.Piva = r.PIVA AND Imprese_Progetti.Sa_Cod = r.SA_COD AND Imprese_Progetti.Appezza = r.APPEZZA AND  Imprese_Progetti.id_reg = r.id_reg")
            StrSQL.AppendLine("    INNER JOIN Appezzamento a (NOLOCK) on a.piva  = r.piva and a.sa_cod = r.SA_COD and a.APPEZZA = r.APPEZZA")
            StrSQL.AppendLine("    INNER JOIN Centri_Aziendali (NOLOCK) ON a.Piva = Centri_Aziendali.Piva AND a.Sa_Cod = Centri_Aziendali.Sa_Cod")
            StrSQL.AppendLine("    LEFT JOIN DPI_Regolamenti (NOLOCK) ON Imprese_Progetti.Disciplinare_Cod = DPI_Regolamenti.COD_REGOLAMENTO AND Imprese_Progetti.Disciplinare_PubblicoPrivato = DPI_Regolamenti.Flag_Privato_Pubblico")
            StrSQL.AppendLine("    LEFT JOIN cultivar (NOLOCK) c on c.Cul_Cod = r.CUL_COD")
            StrSQL.AppendLine("    LEFT JOIN SpecieVegetali (NOLOCK) s on s.Veg_Cod = c.veg_cod")
            StrSQL.AppendLine("    LEFT JOIN GruppoVegetale (NOLOCK) g on g.gru_cod = s.Gru_Cod")
            StrSQL.AppendLine("    LEFT JOIN GruppoFinalita (NOLOCK) gf on gf.Grfi_Cod = r.GRFI_COD")
            StrSQL.AppendLine("    LEFT JOIN GruppoVarietale (NOLOCK) gv on gv.Grva_Cod = r.GRVA_Cod_VEG ")
            StrSQL.AppendLine("    LEFT JOIN #Campi on a.sa_cod = #Campi.sa_cod AND a.piva = #Campi.piva AND a.Campo_Cod = #Campi.Campo_Cod")
            StrSQL.AppendLine("    LEFT JOIN Portinnesti (NOLOCK) on r.port_cod = Portinnesti.port_cod")
            StrSQL.AppendLine("    LEFT JOIN FormeAllevamento (NOLOCK) on r.foral_cod = FormeAllevamento.foral_cod")
            StrSQL.AppendLine("    LEFT JOIN Copertura (NOLOCK) on r.Cop_Cod = Copertura.Cop_Cod")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici codiceImpianto ON r.PIVA = codiceImpianto.PIVA AND r.Sa_Cod = codiceImpianto.Sa_Cod AND r.Appezza = codiceImpianto.Appezza AND r.ID_Reg = codiceImpianto.ID_Reg AND codiceImpianto.id_Cod = 1300")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici tra_fila_m ON r.PIVA = tra_fila_m.PIVA AND r.Sa_Cod = tra_fila_m.Sa_Cod AND r.Appezza = tra_fila_m.Appezza AND r.ID_Reg = tra_fila_m.ID_Reg AND tra_fila_m.id_Cod = 1061")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici su_fila_m ON r.PIVA = su_fila_m.PIVA AND r.Sa_Cod = su_fila_m.Sa_Cod AND r.Appezza = su_fila_m.Appezza AND r.ID_Reg = su_fila_m.ID_Reg AND su_fila_m.id_Cod = 1063")
            StrSQL.AppendLine("    LEFT JOIN ImpiantiIrrigazioni (NOLOCK) on r.Imp_Cod = ImpiantiIrrigazioni.Imp_Cod")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici kpin ON Imprese_Progetti.Progetto_Cod = kpin.Progetto_Cod AND kpin.id_cod = 1287")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici blockName ON Imprese_Progetti.Progetto_Cod = blockName.Progetto_Cod AND blockName.id_cod = 1288")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici grower ON Imprese_Progetti.Progetto_Cod = grower.Progetto_Cod AND grower.id_cod = 1317")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici distintaChiusa ON Imprese_Progetti.Progetto_Cod = distintaChiusa.Progetto_Cod AND distintaChiusa.id_cod = 1301")
            StrSQL.AppendLine(String.Format("    LEFT JOIN Reg_Impianti_Codici codiceImpiantoRibaltato ON Imprese_Progetti.Progetto_Cod = codiceImpiantoRibaltato.Progetto_Cod AND codiceImpiantoRibaltato.id_cod = {0}", CInt(enum_CodiciAnagrafe.Codice_Impianto_Ribaltato)))
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici organismoReferente ON Imprese_Progetti.Progetto_Cod = organismoReferente.Progetto_Cod AND organismoReferente.id_cod = " & enum_CodiciAnagrafe.Organismo_Referente)
            StrSQL.AppendLine("    LEFT JOIN Imprese organismoReferenteDesc (NOLOCK) ON organismoReferente.val_cod = organismoReferenteDesc.PIVA")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici Destinazione ON r.PIVA = Destinazione.PIVA AND r.Sa_Cod = Destinazione.Sa_Cod AND r.Appezza = Destinazione.Appezza AND r.ID_Reg = Destinazione.ID_Reg AND Destinazione.id_Cod >= 3000 AND Destinazione.id_cod < 4000")
            StrSQL.AppendLine("    LEFT JOIN Codici_Anagrafe (NOLOCK) ON Destinazione.id_cod = Codici_Anagrafe.codice")
            StrSQL.AppendLine("    LEFT JOIN #AppezzamentiSingoliCompleti ON a.PIVA = #AppezzamentiSingoliCompleti.PIVA AND a.SA_COD = #AppezzamentiSingoliCompleti.SA_COD AND a.APPEZZA = #AppezzamentiSingoliCompleti.APPEZZA")
            StrSQL.AppendLine("    LEFT JOIN UnitaMisura_Alternative udmAlternative (NOLOCK) ON r.UDM_COD_ALT = udmAlternative.UDM_COD_ALT")
            StrSQL.AppendLine("    LEFT JOIN Conversione_UnitaMisura_Alternative (NOLOCK) ON udmAlternative.UDM_COD_ALT = Conversione_UnitaMisura_Alternative.UDM_COD_ALT_TO AND Conversione_UnitaMisura_Alternative.UDM_COD_FROM = 2123")

            If sql2017 Then
                StrSQL.AppendLine("    LEFT JOIN #AppezzaZVN ON a.Piva = #AppezzaZVN.PIVA AND a.sa_cod = #AppezzaZVN.sa_cod AND a.APPEZZA = #AppezzaZVN.APPEZZA")
            End If

            If leggiStaticMap Then
                StrSQL.AppendLine("    LEFT JOIN gis_entita e (NOLOCK) ON  r.piva = e.piva")
                StrSQL.AppendLine("        AND r.sa_cod = e.sa_cod")
                StrSQL.AppendLine("        AND r.appezza = e.appezza")
                StrSQL.AppendLine("        AND r.ID_REG = e.Id_Imp")
                StrSQL.AppendLine("        AND e.TipoEntita_Cod IN (SELECT TipoEntita_Cod FROM GIS_TipoEntita WHERE LayerElementiGrafici_Cod = 19)")
                StrSQL.AppendLine("    LEFT JOIN gis_elementigrafici gis (NOLOCK) on e.PivaSuperUser = gis.PivaSuperUser")
                StrSQL.AppendLine("        AND  e.entita_cod = gis.Entita_Cod")
                StrSQL.AppendLine("        AND gis.LayerElementiGrafici_Cod = 19")
            End If

            StrSQL.AppendLine("    LEFT JOIN Appezzamento_Codici metodoProd ON a.PIVA = metodoProd.PIVA AND a.SA_COD = metodoProd.sa_cod AND a.APPEZZA = metodoProd.appezza AND metodoProd.id_cod = " & enum_CodiciAnagrafe.MetodoDiProduzione)
            StrSQL.AppendLine("    LEFT JOIN Regolamenti (NOLOCK) ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod")

            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici N ON Imprese_Progetti.Progetto_Cod = N.Progetto_Cod AND N.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteN)
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici P ON Imprese_Progetti.Progetto_Cod = P.Progetto_Cod AND P.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteP)
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici K ON Imprese_Progetti.Progetto_Cod = K.Progetto_Cod AND K.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteK)
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici Mg ON Imprese_Progetti.Progetto_Cod = Mg.Progetto_Cod AND Mg.id_Cod = " & enum_CodiciAnagrafe.Impianto_LimiteMg)
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici interbina ON r.Piva = interbina.Piva AND r.Sa_Cod = interbina.Sa_Cod AND r.Appezza = interbina.Appezza AND r.Id_Reg = interbina.Id_Reg AND interbina.id_Cod = " & enum_CodiciAnagrafe.Impianto_Interbina)
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici germinabilita ON r.Piva = germinabilita.Piva AND r.Sa_Cod = germinabilita.Sa_Cod AND r.Appezza = germinabilita.Appezza AND r.Id_Reg = germinabilita.Id_Reg AND germinabilita.id_Cod = " & enum_CodiciAnagrafe.Impianto_Germinabilita & "  ")

            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici grfi_RER ON Imprese_Progetti.Progetto_Cod = grfi_RER.Progetto_Cod AND grfi_RER.id_Cod = " & enum_CodiciAnagrafe.Finalita_Concimazione_Impianto)
            StrSQL.AppendLine("    LEFT JOIN GruppoFinalita_Rer (NOLOCK) ON grfi_RER.Val_Cod = GruppoFinalita_Rer.GRFI_COD")
            StrSQL.AppendLine("    LEFT JOIN FasiCicloColturale_Anagrafiche (NOLOCK) ON Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod")

            StrSQL.AppendLine("    LEFT JOIN Materie_Prime (NOLOCK) ON Imprese_Progetti.Mat_Cod = Materie_Prime.Mat_Cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Reg_Impianti_Codici dettSpecie ON r.id_reg = dettSpecie.id_reg")
            StrSQL.AppendLine("        AND dettSpecie.id_cod = " & enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato)
            StrSQL.AppendLine("        AND dettSpecie.PIVA = r.PIVA")
            StrSQL.AppendLine("        AND dettSpecie.sa_cod = r.SA_COD")
            StrSQL.AppendLine("        AND dettSpecie.appezza = r.APPEZZA")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN CAC_Codifica_InfoAggiuntive cac ON dettSpecie.val_cod = cac.InfoAgg_Cod AND cac.Argomento_Cod = " & enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.DettaglioSpeciePersonalizzato)
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN #PxACA pxa ON Imprese_Progetti.Progetto_Cod = pxa.ProgettoCod")
            StrSQL.AppendLine("            AND Imprese_Progetti.Piva = pxa.Piva")
            StrSQL.AppendLine("")
            If readLinkedMachines Then
                StrSQL.AppendLine("    LEFT JOIN #AxP ON #AxP.Piva = a.Piva AND #AxP.Sa_Cod = a.SA_COD AND #AxP.Appezza = a.Appezza ")
            End If

            If leggiDatiRibaltamento Then
                StrSQL.AppendLine(" LEFT JOIN Ribaltamento_Imprese_Progetti rip ON rip.Reale_Piva = Imprese_Progetti.Piva AND rip.Reale_Sa_Cod = Imprese_Progetti.Sa_Cod AND rip.Reale_Appezza= Imprese_Progetti.Appezza AND rip.Reale_Id_Reg= Imprese_Progetti.Id_Reg AND rip.Reale_Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
                StrSQL.AppendLine(" LEFT JOIN Budget_Testata ba on ba.Id_Budget = rip.Budget_Id_Testata ")
            End If

            StrSQL.AppendLine("")

            StrSQL.AppendLine("WHERE Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine("    AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Codice_Fiscale_Tecnico <> "" Then
                StrSQL.AppendLine("    AND r.CODICE_FISCALE_TECNICO = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "'")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine("    AND     r.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            'StrSQL.AppendLine(" AND     r.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

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
                StrSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
            End If

            'StrSQL.AppendLine(" DROP TABLE #Reg_Impianti_Codici ")
            StrSQL.AppendLine(" DROP TABLE #AppezzamentiSingoli ")
            StrSQL.AppendLine(" DROP TABLE #AppezzamentiSingoliCompleti ")
            If sql2017 Then
                StrSQL.AppendLine(" DROP TABLE #AppezzaZVN ")
            End If
            StrSQL.AppendLine(" DROP TABLE #Campi ")
            If readLinkedMachines Then
                StrSQL.AppendLine(" DROP TABLE #AxP ")
            End If
            StrSQL.AppendLine(" DROP TABLE #PxACA ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If xOrderBy = "" Then
                If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                    Dim view = DT.DefaultView
                    view.Sort = " Validita_Inizio DESC "
                    DT = view.ToTable()
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Max_DataModifica(ByVal Piva As String,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DateTime

        Dim NomeRoutine As String = "AnagrafeDAL.Impresa_Progetti_R.Leggi_Max_DataModifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Data_Modifica = AGRODATAINIZIO
        Try

            If Piva = "" Then
                Throw New Exception("Piva obbligatoria")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT MAX(Data_Modifica) ")
            StrSQL.AppendLine(" FROM Imprese_Progetti ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
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

#Region "Modifica Multipla Piano Colturale"
    Public Function CreaTabellaTemp_FiltroProgetti() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempProgetti') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempProgetti ( ")
        stb.AppendLine("        Progetto_Cod int NULL")
        stb.AppendLine("    )")
        stb.AppendLine()
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Public Function EliminaTabellaTemp_FiltroProgetti() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempProgetti') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempProgetti ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Public Function Leggi_Report_Modifica_Multipla_PianoColturale(ByVal filtroProgetti As List(Of Integer),
                                                                  ByVal ordinamento As String,
                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                  ) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.Impresa_Progetti_R.Leggi_Report_Modifica_Multipla_PianoColturale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim flagConnessione, flagTransazione As Boolean


        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroProgetto(filtroProgetti, NomeRoutine, objParametri)

            ' Query Principale
            StrSQL.AppendLine(" SELECT DISTINCT")
            StrSQL.AppendLine("      r.PIVA + '_' + CAST(r.sa_cod AS varchar(20))+ '_' + CAST(r.Appezza AS varchar(20)) + '_' + CAST(r.id_reg AS varchar(20)) + '_' + CAST(isnull(s.veg_cod ,'0') AS varchar(20)) + '_' + CAST(isnull(Imprese_Progetti.Progetto_Cod,'0') AS varchar(20)) AS chiave ")
            StrSQL.AppendLine("    , r.PIVA, r.sa_cod, r.APPEZZA, r.id_Reg, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine("    , sa_nome, a.app_nome, imprese.rag_soc ")
            StrSQL.AppendLine("    , r.Sup_Imp ")
            StrSQL.AppendLine("    , c.cul_cod, c.cul_des, gf.Grfi_Des, grva_des ")
            StrSQL.AppendLine("    , s.veg_cod, gf.grfi_cod ")

            StrSQL.AppendLine($"    , CASE WHEN Imprese_Progetti.FlagSecondoRaccolto = '1' THEN '{Gias.Si}' ELSE '{Gias.No}' END AS FlagSecondoRaccolto ")

            StrSQL.AppendLine("    , CASE WHEN tra_fila_m.val_cod IS NULL THEN '0' ELSE CASE WHEN tra_fila_m.val_cod = '' THEN '0' ELSE tra_fila_m.val_cod END END AS tra_fila_m ")
            StrSQL.AppendLine("    , CASE WHEN su_fila_m.val_cod IS NULL THEN '0' ELSE CASE WHEN su_fila_m.val_cod = '' THEN '0' ELSE su_fila_m.val_cod END END AS su_fila_m ")
            StrSQL.AppendLine("    , Portinnesti.port_des, FormeAllevamento.foral_des, Copertura.Cop_Des, ImpiantiIrrigazioni.Imp_des ")
            StrSQL.AppendLine("    , Imprese_Progetti.Produzione_Prevista AS Resa_Prevista ")
            StrSQL.AppendLine("    , Imprese_Progetti.Regolamento_COD AS regolamento ")
            StrSQL.AppendLine("    , metodoProd.val_cod AS MetodoProduzione_Cod ")
            StrSQL.AppendLine("    , CASE WHEN metodoProd.val_cod IS NULL THEN '' ")
            StrSQL.AppendLine($"          WHEN metodoProd.val_cod = '1' THEN '{Gias.Convenzionale}' ")
            StrSQL.AppendLine($"          WHEN metodoProd.val_cod = '2' THEN '{Gias.InConversione}' ")
            StrSQL.AppendLine($"          WHEN metodoProd.val_cod = '3' THEN '{Gias.Biologico}' ")
            StrSQL.AppendLine("           ELSE '' ")
            StrSQL.AppendLine("      END AS MetodoProduzione_Des ")
            StrSQL.AppendLine("    , CASE WHEN r.Data_Inizio_Portinnesto = CAST('1900-01-01' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE r.Data_Inizio_Portinnesto ")
            StrSQL.AppendLine("      END AS Data_Inizio_Portinnesto ")
            StrSQL.AppendLine("    , CASE WHEN Imprese_Progetti.Data_Inizio_Prevista = CAST('1900-01-01' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE Imprese_Progetti.Data_Inizio_Prevista ")
            StrSQL.AppendLine("      END AS Data_Semina_Prevista ")
            StrSQL.AppendLine("    , CASE WHEN Imprese_Progetti.Data_Fine_Prevista = CAST('2100-12-31' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE Imprese_Progetti.Data_Fine_Prevista ")
            StrSQL.AppendLine("      END AS Data_Raccolta_Prevista ")
            StrSQL.AppendLine("    , CASE WHEN Imprese_Progetti.Data_Fioritura_Prevista = CAST('1900-01-01' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE Imprese_Progetti.Data_Fioritura_Prevista ")
            StrSQL.AppendLine("      END AS Data_Fioritura_Prevista ")
            StrSQL.AppendLine("    , CASE WHEN r.Validita_Fine = CAST('2100-12-31' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE r.Validita_Fine ")
            StrSQL.AppendLine("      END AS data_fine_impianto ")
            StrSQL.AppendLine("    , CASE WHEN a.Validita_Fine = CAST('2100-12-31' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE a.Validita_Fine ")
            StrSQL.AppendLine("      END AS data_fine_appezzamento ")
            StrSQL.AppendLine("    , ISNULL((nrAppBio.val_cod), '') AS nrAppBio ")
            StrSQL.AppendLine("    , ISNULL((capitolatoPrivato_des.InfoAgg_Des), '') AS capitolatoPrivato_des ")
            StrSQL.AppendLine("    , SUBSTRING((magazzinoConferimento.val_cod), 0, CHARINDEX('|', (magazzinoConferimento.val_cod))) AS magazzinoConferimento_cod ")
            StrSQL.AppendLine("    , ISNULL((magazzinoConferimento.val_cod), '') AS magazzinoConferimento, Fabbricati.Fabbricato_Des + '(' + ownerMagazzinoConferimento.Rag_Soc + ')' AS magazzinoConferimento_des ")
            StrSQL.AppendLine("    , ISNULL((organismoReferente.val_cod), '') AS organismoReferente_cod, impreseOrgRef.PIVA + ' - ' + impreseOrgRef.rag_soc AS organismoReferente_des ")
            StrSQL.AppendLine("    , ISNULL((certificazione.val_cod), '') AS certificazione ")
            StrSQL.AppendLine("    , ISNULL((LimiteN.val_cod), '') AS LimiteN ")
            StrSQL.AppendLine("    , ISNULL((LimiteP.val_cod), '') AS LimiteP ")
            StrSQL.AppendLine("    , ISNULL((LimiteK.val_cod), '') AS LimiteK ")
            StrSQL.AppendLine("    , Imprese_Progetti.Stato_Impianto AS Stato_Impianto_cod ")
            StrSQL.AppendLine("    , ISNULL(statoImpianto.Grfi_Des, '') AS stato_impianto ")
            StrSQL.AppendLine("    , Imprese_Progetti.Regolamento_Concimazioni_Cod ")
            StrSQL.AppendLine("    , Imprese_Progetti.disciplinare_cod AS Dpi_Cod ")
            StrSQL.AppendLine("    , DPI_Regolamenti.Des AS disciplinare, Regolamenti.Reg_Des AS regolamento_str ")

            StrSQL.AppendLine("    , CASE WHEN Imprese_Progetti.Validita_Inizio = CAST('1900-01-01' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("      END AS validita_inizio ")
            StrSQL.AppendLine("    , CASE WHEN Imprese_Progetti.Validita_Fine = CAST('2100-12-31' AS DATE) THEN NULL ")
            StrSQL.AppendLine("           ELSE Imprese_Progetti.Validita_Fine ")
            StrSQL.AppendLine("      END AS validita_fine ")
            StrSQL.AppendLine($"    , CASE r.cul_Cod WHEN 0 THEN ISNULL(Codici_Anagrafe.descrizione, '{Gias.TerrenoNudo}') ELSE s.Veg_Des + ' - ' + c.Cul_Des END AS utilizzo ")
            StrSQL.AppendLine("    , ISNULL(CertificazioneProdotto.InfoAgg_Des, '') AS CertificazioneProdotto ")
            StrSQL.AppendLine("    , ISNULL(Residui.InfoAgg_Des, '') AS Residuo ")
            StrSQL.AppendLine("    , ISNULL(LicenzaColtivazione.Descrizione, '') AS LicenzaColtivazione ")
            StrSQL.AppendLine("    , ISNULL(RiferimentoTrasferimentoDati.Rag_Soc, '') AS RiferimentoTrasferimentoDati ")
            StrSQL.AppendLine("    , ISNULL(PianiSemina.InfoAgg_Des, '') AS PianoSemina ")
            StrSQL.AppendLine("    , ISNULL(Prodotti.Mat_Des, '') AS Prodotto ")
            StrSQL.AppendLine("    , a.Blk_Flag ")
            StrSQL.AppendLine($"    , CASE WHEN a.Blk_Flag = -1 THEN '{Gias.Si}' ELSE '{Gias.No}' END AS AppBloccato ")
            StrSQL.AppendLine("    , r.Data_Inizio_Impianto ")
            StrSQL.AppendLine("    , ISNULL(Campi.campo_Des, '') AS campo_des ")
            StrSQL.AppendLine("    , ISNULL(EsercizioChiuso.val_cod, '0') AS EsercizioChiusoCod ")
            StrSQL.AppendLine($"    , CASE WHEN ISNULL(EsercizioChiuso.val_cod, '0') = '1' THEN '{Gias.Si}' ELSE '{Gias.No}' END AS EsercizioChiusoDes ")
            StrSQL.AppendLine($"    , CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN r.PIVA ELSE Imprese.partitaIvaReale END AS partitaIvaReale ")

            StrSQL.AppendLine(" FROM Imprese_Progetti ")
            StrSQL.AppendLine(" INNER JOIN #TempProgetto tempP ON ")
            StrSQL.AppendLine("    Imprese_Progetti.Progetto_Cod = tempP.Progetto_Cod ")
            StrSQL.AppendLine(" JOIN imprese ON Imprese_Progetti.piva = imprese.PIVA ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti r ON Imprese_Progetti.Piva = r.PIVA ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = r.SA_COD AND Imprese_Progetti.Appezza = r.APPEZZA AND Imprese_Progetti.id_reg = r.id_reg ")
            StrSQL.AppendLine(" INNER JOIN Appezzamento a ON a.piva = r.piva and a.sa_cod = r.SA_COD and a.APPEZZA = r.APPEZZA ")
            StrSQL.AppendLine(" LEFT JOIN cultivar c On c.Cul_Cod = r.CUL_COD ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali s On s.Veg_Cod = c.veg_cod ")
            StrSQL.AppendLine(" LEFT JOIN GruppoVegetale g On g.gru_cod = s.Gru_Cod ")
            StrSQL.AppendLine(" LEFT JOIN GruppoFinalita gf On gf.grfi_cod = r.grfi_cod ")
            StrSQL.AppendLine(" LEFT JOIN GruppoVarietale gv On gv.Grva_Cod = r.GRVA_Cod_VEG ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca On r.sa_cod = ca.sa_cod AND r.piva = ca.piva ")
            StrSQL.AppendLine(" LEFT JOIN Campi On a.sa_cod = Campi.sa_cod AND a.piva = Campi.piva AND a.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Portinnesti On r.port_cod = Portinnesti.port_cod ")
            StrSQL.AppendLine(" LEFT JOIN FormeAllevamento On r.foral_cod = FormeAllevamento.foral_cod ")
            StrSQL.AppendLine(" LEFT JOIN Copertura On r.Cop_Cod = Copertura.Cop_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici codiceImpianto On r.PIVA = codiceImpianto.PIVA ")
            StrSQL.AppendLine(" AND r.Sa_Cod = codiceImpianto.Sa_Cod AND r.Appezza = codiceImpianto.Appezza AND r.ID_Reg = codiceImpianto.ID_Reg ")
            StrSQL.AppendLine($" AND codiceImpianto.id_Cod = {CInt(enum_CodiciAnagrafe.Codice_Impianto)} ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici tra_fila_m On r.PIVA = tra_fila_m.PIVA ")
            StrSQL.AppendLine(" AND r.Sa_Cod = tra_fila_m.Sa_Cod AND r.Appezza = tra_fila_m.Appezza AND r.ID_Reg = tra_fila_m.ID_Reg ")
            StrSQL.AppendLine($" AND tra_fila_m.id_Cod = {CInt(enum_CodiciAnagrafe.Impianto_TraFila_Maschio)} ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici su_fila_m On r.PIVA = su_fila_m.PIVA ")
            StrSQL.AppendLine(" AND r.Sa_Cod = su_fila_m.Sa_Cod AND r.Appezza = su_fila_m.Appezza AND r.ID_Reg = su_fila_m.ID_Reg ")
            StrSQL.AppendLine($" AND su_fila_m.id_Cod = {CInt(enum_CodiciAnagrafe.Impianto_SuFila_Maschio)} ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici Data_Inizio_Portinnesto On r.PIVA = Data_Inizio_Portinnesto.PIVA ")
            StrSQL.AppendLine(" AND r.Sa_Cod = Data_Inizio_Portinnesto.Sa_Cod AND r.Appezza = Data_Inizio_Portinnesto.Appezza AND r.ID_Reg = Data_Inizio_Portinnesto.ID_Reg ")
            StrSQL.AppendLine($" AND Data_Inizio_Portinnesto.id_Cod = {CInt(enum_CodiciAnagrafe.Data_Inizio_Portinnesto)} ")
            StrSQL.AppendLine(" LEFT JOIN ImpiantiIrrigazioni On r.Imp_Cod = ImpiantiIrrigazioni.Imp_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici Destinazione On r.PIVA = Destinazione.PIVA ")
            StrSQL.AppendLine(" AND r.Sa_Cod = Destinazione.Sa_Cod AND r.Appezza = Destinazione.Appezza AND r.ID_Reg = Destinazione.ID_Reg ")
            StrSQL.AppendLine(" AND Destinazione.id_Cod > =  3000 AND Destinazione.id_cod < 4000 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe On Destinazione.val_cod = Codici_Anagrafe.codice ")

            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici metodoProd ON a.PIVA = metodoProd.PIVA AND a.SA_COD = metodoProd.sa_cod ")
            StrSQL.AppendLine($" AND a.APPEZZA = metodoProd.appezza AND metodoProd.id_cod = {CInt(enum_CodiciAnagrafe.MetodoDiProduzione)} ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_Codici nrAppBio ON a.PIVA = nrAppBio.PIVA ")
            StrSQL.AppendLine($" AND a.SA_COD = nrAppBio.sa_cod AND a.APPEZZA = nrAppBio.appezza AND nrAppBio.id_cod = {CInt(enum_CodiciAnagrafe.Codice_Appezza_Biologico)} ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici capitolatoPrivato ON Imprese_Progetti.Piva = capitolatoPrivato.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = capitolatoPrivato.Sa_Cod AND Imprese_Progetti.Appezza = capitolatoPrivato.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = capitolatoPrivato.Id_Reg AND Imprese_Progetti.Progetto_Cod = capitolatoPrivato.Progetto_Cod ")
            StrSQL.AppendLine($" AND capitolatoPrivato.id_cod = {CInt(enum_CodiciAnagrafe.Capitolato_Privato)} ")
            StrSQL.AppendLine(" LEFT JOIN CAC_Codifica_InfoAggiuntive capitolatoPrivato_des ON capitolatoPrivato.Val_cod = capitolatoPrivato_des.InfoAgg_Cod ")
            StrSQL.AppendLine($" AND capitolatoPrivato_des.Argomento_Cod = {CInt(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CapitolatoPrivato)} ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici magazzinoConferimento ON Imprese_Progetti.Piva = magazzinoConferimento.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = magazzinoConferimento.Sa_Cod AND Imprese_Progetti.Appezza = magazzinoConferimento.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = magazzinoConferimento.Id_Reg AND Imprese_Progetti.Progetto_Cod = magazzinoConferimento.Progetto_Cod ")
            StrSQL.AppendLine($" AND magazzinoConferimento.id_cod =  {CInt(enum_CodiciAnagrafe.Magazzino_Conferimento)} ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici certificazione ON Imprese_Progetti.Piva = certificazione.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = certificazione.Sa_Cod AND Imprese_Progetti.Appezza = certificazione.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = certificazione.Id_Reg AND Imprese_Progetti.Progetto_Cod = certificazione.Progetto_Cod ")
            StrSQL.AppendLine($" AND certificazione.id_cod =  {CInt(enum_CodiciAnagrafe.Certificazione)} ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici organismoReferente ON Imprese_Progetti.Piva = organismoReferente.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = organismoReferente.Sa_Cod AND Imprese_Progetti.Appezza = organismoReferente.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = organismoReferente.Id_Reg AND Imprese_Progetti.Progetto_Cod = organismoReferente.Progetto_Cod ")
            StrSQL.AppendLine($" AND organismoReferente.id_cod =  {CInt(enum_CodiciAnagrafe.Organismo_Referente)}")
            StrSQL.AppendLine(" LEFT JOIN imprese impreseOrgRef ON impreseOrgRef.piva = organismoReferente.val_cod ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici LimiteN ON Imprese_Progetti.Piva = LimiteN.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = LimiteN.Sa_Cod AND Imprese_Progetti.Appezza = LimiteN.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = LimiteN.Id_Reg AND Imprese_Progetti.Progetto_Cod = LimiteN.Progetto_Cod ")
            StrSQL.AppendLine($" AND LimiteN.id_cod = {CInt(enum_CodiciAnagrafe.Impianto_LimiteN)} ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici LimiteP ON Imprese_Progetti.Piva = LimiteP.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = LimiteP.Sa_Cod AND Imprese_Progetti.Appezza = LimiteP.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = LimiteP.Id_Reg AND Imprese_Progetti.Progetto_Cod = LimiteP.Progetto_Cod ")
            StrSQL.AppendLine($" AND LimiteP.id_cod = {CInt(enum_CodiciAnagrafe.Impianto_LimiteP)} ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici LimiteK ON Imprese_Progetti.Piva = LimiteK.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = LimiteK.Sa_Cod AND Imprese_Progetti.Appezza = LimiteK.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = LimiteK.Id_Reg AND Imprese_Progetti.Progetto_Cod = LimiteK.Progetto_Cod ")
            StrSQL.AppendLine($" AND LimiteK.id_cod = {CInt(enum_CodiciAnagrafe.Impianto_LimiteK)} ")
            StrSQL.AppendLine(" LEFT JOIN GruppoFinalita statoImpianto ON Imprese_Progetti.Stato_Impianto = statoImpianto.Grfi_Cod ")
            StrSQL.AppendLine(" LEFT OUTER JOIN DPI_Regolamenti ON Imprese_Progetti.Disciplinare_Cod = DPI_Regolamenti.COD_REGOLAMENTO ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Disciplinare_PubblicoPrivato = DPI_Regolamenti.Flag_Privato_Pubblico ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod ")

            'Substring per estrapolare la parte centrale di ex. 132775938|132775937|0159112040  (magazzinoConferimento)
            StrSQL.AppendLine(" LEFT JOIN Fabbricati ON Fabbricato_Cod = SUBSTRING((magazzinoConferimento.val_cod), 0, CHARINDEX('|', (magazzinoConferimento.val_cod))) ")
            StrSQL.AppendLine(" --Substring per estrapolare la parte centrale di 132775938|132775937|01591120405 (magazzinoConferimento) ")
            StrSQL.AppendLine(" AND Fabbricati.SA_cOD = SUBSTRING((SUBSTRING(magazzinoConferimento.val_cod, LEN(SUBSTRING((magazzinoConferimento.val_cod), 0, CHARINDEX('|',(magazzinoConferimento.val_cod)))) + 2, LEN(magazzinoConferimento.val_cod))), 0, CHARINDEX('|', (SUBSTRING(magazzinoConferimento.val_cod, LEN(SUBSTRING((magazzinoConferimento.val_cod), 0, CHARINDEX('|', (magazzinoConferimento.val_cod))))+2, LEN(magazzinoConferimento.val_cod))))) ")
            StrSQL.AppendLine(" AND Fabbricati.piva = SUBSTRING(magazzinoConferimento.val_cod, LEN(magazzinoConferimento.val_cod) - CHARINDEX('|', REVERSE(magazzinoConferimento.val_cod)) + 1 + 1, LEN(magazzinoConferimento.val_cod)) ")
            StrSQL.AppendLine(" LEFT JOIN Imprese ownerMagazzinoConferimento ON ownerMagazzinoConferimento.Piva = Fabbricati.Piva ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici CertProdotto ON CertProdotto.Piva = Imprese_Progetti.Piva AND CertProdotto.Sa_Cod = Imprese_Progetti.SA_Cod ")
            StrSQL.AppendLine(" AND CertProdotto.Appezza = Imprese_Progetti.Appezza AND CertProdotto.Id_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine($" AND CertProdotto.Progetto_Cod = Imprese_Progetti.Progetto_Cod AND CertProdotto.id_Cod = {CInt(enum_CodiciAnagrafe.Codice_Certificazione_Prodotto)} ")
            StrSQL.AppendLine($" LEFT JOIN CAC_Codifica_InfoAggiuntive CertificazioneProdotto ON CertificazioneProdotto.InfoAgg_Cod = CertProdotto.val_cod and CertificazioneProdotto.Argomento_Cod = " & enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Certificazione_Prodotto & " ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici Residui_1 ON Residui_1.Piva = Imprese_Progetti.Piva AND Residui_1.Sa_Cod = Imprese_Progetti.SA_Cod ")
            StrSQL.AppendLine(" AND Residui_1.Appezza = Imprese_Progetti.Appezza AND Residui_1.Id_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine($" AND Residui_1.Progetto_Cod = Imprese_Progetti.Progetto_Cod AND Residui_1.id_Cod = {CInt(enum_CodiciAnagrafe.Codice_Residuo)} ")
            StrSQL.AppendLine($" LEFT JOIN CAC_Codifica_InfoAggiuntive Residui ON Residui.InfoAgg_Cod = Residui_1.val_cod and Residui.Argomento_Cod = {CInt(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Residuo)} ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici LiceColtivazione ON LiceColtivazione.Piva = Imprese_Progetti.Piva AND LiceColtivazione.Sa_Cod = Imprese_Progetti.SA_Cod ")
            StrSQL.AppendLine(" AND LiceColtivazione.Appezza = Imprese_Progetti.Appezza AND LiceColtivazione.Id_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine($" AND LiceColtivazione.Progetto_Cod = Imprese_Progetti.Progetto_Cod AND LiceColtivazione.id_Cod = {CInt(enum_CodiciAnagrafe.Zespri_Fasi_Fase)} ")
            StrSQL.AppendLine($" LEFT JOIN OTabelle_Parametri LicenzaColtivazione ON LicenzaColtivazione.Tabella_Par_Cod = LiceColtivazione.val_cod and LicenzaColtivazione.Tabella_Cod = {CInt(enum_CodiciAnagrafe.Zespri_Fasi_Fase)} ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici RifTrasferimentoDati ON RifTrasferimentoDati.Piva = Imprese_Progetti.Piva AND RifTrasferimentoDati.Sa_Cod = Imprese_Progetti.SA_Cod ")
            StrSQL.AppendLine(" AND RifTrasferimentoDati.Appezza = Imprese_Progetti.Appezza AND RifTrasferimentoDati.Id_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine($" AND RifTrasferimentoDati.Progetto_Cod = Imprese_Progetti.Progetto_Cod AND RifTrasferimentoDati.id_Cod = {CInt(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati)} ")
            StrSQL.AppendLine(" LEFT JOIN Risorse_Umane RiferimentoTrasferimentoDati_1 ON RiferimentoTrasferimentoDati_1.Cod_Contatto = RifTrasferimentoDati.val_cod ")
            StrSQL.AppendLine($" AND RiferimentoTrasferimentoDati_1.Cod_Rapporto = {CInt(enum_Rapporti_Contabili_Standard.Riferimento_Trasferimento_Dati)} ")
            StrSQL.AppendLine(" AND (RiferimentoTrasferimentoDati_1.Piva = Imprese_Progetti.PIVA OR RiferimentoTrasferimentoDati_1.Sa_Cod = -1)  ")

            StrSQL.AppendLine(" LEFT JOIN Contatti RiferimentoTrasferimentoDati ON RiferimentoTrasferimentoDati.Cod_Contatto = RiferimentoTrasferimentoDati_1.Cod_Contatto ")
            StrSQL.AppendLine(" AND RiferimentoTrasferimentoDati.Piva = RiferimentoTrasferimentoDati_1.PIVA ")
            StrSQL.AppendLine(" AND RiferimentoTrasferimentoDati.Sa_Cod = RiferimentoTrasferimentoDati_1.Sa_Cod  ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici PianiSemina_1 ON PianiSemina_1.Piva = Imprese_Progetti.Piva AND PianiSemina_1.Sa_Cod = Imprese_Progetti.SA_Cod ")
            StrSQL.AppendLine(" AND PianiSemina_1.Appezza = Imprese_Progetti.Appezza AND PianiSemina_1.Id_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine($" AND PianiSemina_1.Progetto_Cod = Imprese_Progetti.Progetto_Cod AND PianiSemina_1.id_Cod = {CInt(enum_CodiciAnagrafe.Impianto_PianoSemina)} ")
            StrSQL.AppendLine($" LEFT JOIN CAC_Codifica_InfoAggiuntive PianiSemina ON PianiSemina.InfoAgg_Cod = PianiSemina_1.val_cod and PianiSemina.Argomento_Cod = {CInt(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Piano_Semina)} ")

            StrSQL.AppendLine($" LEFT JOIN Materie_Prime Prodotti ON Prodotti.Mat_Cod = Imprese_Progetti.Mat_Cod AND Prodotti.Elem_Cod = {CInt(TRASFORMATI_VEGETALI)} AND (Prodotti.Piva = Imprese_Progetti.Piva OR Prodotti.Sa_Cod = -1) ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici EsercizioChiuso ON Imprese_Progetti.Piva = EsercizioChiuso.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = EsercizioChiuso.Sa_Cod AND Imprese_Progetti.Appezza = EsercizioChiuso.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = EsercizioChiuso.Id_Reg AND Imprese_Progetti.Progetto_Cod = EsercizioChiuso.Progetto_Cod ")
            StrSQL.AppendLine($" AND EsercizioChiuso.id_cod = {CInt(enum_CodiciAnagrafe.Distinta_Chiusa)} ")

            If ordinamento <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(ordinamento))
            Else
                StrSQL.AppendLine(" ORDER BY r.PIVA, r.sa_cod, r.APPEZZA, r.id_Reg DESC ")
            End If

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroProgetto(NomeRoutine, objParametri)

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)



        Catch ex As Exception
            ' Rollback
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return DT

    End Function

    Public Function Leggi_per_ModificaMultipla(listChiavi As List(Of (String, Integer, Integer, Integer, Integer)),
                                               modalitaEstrazioneEsercizi As Enum_ModEserciziModificaMultiplaPianoColturale,
                                               Data_Validita As Date,
                                               almenoUnParametroEsercizio As Boolean,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiValiditaAnagrafica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(listChiavi, NomeRoutine, objParametri)


            StrSQL.Length = 0

            If almenoUnParametroEsercizio Then
                StrSQL.AppendLine(" SELECT")
                StrSQL.AppendLine("       Imprese_Progetti.Piva")
                StrSQL.AppendLine("     , Imprese_Progetti.Progetto_Cod")
                StrSQL.AppendLine("     , Imprese_Progetti.Sa_Cod")
                StrSQL.AppendLine("     , Imprese_Progetti.Appezza")
                StrSQL.AppendLine("     , Imprese_Progetti.Id_Reg")
                StrSQL.AppendLine(" INTO #EserciziPostFiltroValidita_TT ")
                StrSQL.AppendLine(" ")
                StrSQL.AppendLine(" FROM Imprese_Progetti")
                StrSQL.AppendLine(" --Vado in join fino alla chiave dell'impianto per tirare fuori tutti gli esercizi ")
                StrSQL.AppendLine(" JOIN #TempEsercizio temp (NOLOCK) ON ")
                StrSQL.AppendLine("     Imprese_Progetti.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = temp.Sa_Cod ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = temp.Appezza ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = temp.Id_Reg ")
                StrSQL.AppendLine(" WHERE 1 = 1 ")
                If {Enum_ModEserciziModificaMultiplaPianoColturale.AttiviAllaDataOdierna,
                    Enum_ModEserciziModificaMultiplaPianoColturale.EserciziAttiviAllaDataX}.Contains(modalitaEstrazioneEsercizi) Then
                    StrSQL.AppendLine($" AND Imprese_Progetti.Validita_Inizio <= {Agro_SQL_SaveDate(Data_Validita)} ")
                    StrSQL.AppendLine($" AND Imprese_Progetti.Validita_Fine >= {Agro_SQL_SaveDate(Data_Validita)} ")
                End If
            End If

            StrSQL.AppendLine(" SELECT")
            StrSQL.AppendLine("       Appezzamento_Codici.Piva")
            StrSQL.AppendLine("     , Appezzamento_Codici.Sa_Cod")
            StrSQL.AppendLine("     , Appezzamento_Codici.Appezza")
            StrSQL.AppendLine("     , Appezzamento_Codici.Val_Cod")
            StrSQL.AppendLine(" INTO #MetodoProduzione_TT ")
            StrSQL.AppendLine(" FROM Appezzamento_Codici ")
            StrSQL.AppendLine(" JOIN #TempEsercizio temp (NOLOCK) ON ")
            StrSQL.AppendLine("     Appezzamento_Codici.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
            StrSQL.AppendLine(" AND Appezzamento_Codici.Sa_Cod = temp.Sa_Cod ")
            StrSQL.AppendLine(" AND Appezzamento_Codici.Appezza = temp.Appezza ")
            StrSQL.AppendLine($" WHERE Id_Cod = {CInt(enum_CodiciAnagrafe.MetodoDiProduzione)}")

            StrSQL.AppendLine(" SELECT")
            StrSQL.AppendLine("       Imprese_Progetti.Piva")
            StrSQL.AppendLine("     , Imprese_Progetti.Sa_Cod")
            StrSQL.AppendLine("     , Imprese_Progetti.Appezza")
            StrSQL.AppendLine("     , MIN(Imprese_Progetti.Regolamento_Cod) AS Regolamento ")
            StrSQL.AppendLine(" INTO #RegolamentoEsercizixAppezzamento_TT ")
            StrSQL.AppendLine(" FROM Imprese_Progetti ")
            StrSQL.AppendLine(" JOIN #TempEsercizio temp (NOLOCK) ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = temp.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = temp.Appezza ")
            StrSQL.AppendLine(" GROUP BY Imprese_Progetti.Piva, Imprese_Progetti.Sa_Cod, Imprese_Progetti.Appezza ")


            StrSQL.AppendLine(" SELECT")
            StrSQL.AppendLine("       Imprese_Progetti.Piva")
            StrSQL.AppendLine("     , Imprese_Progetti.Progetto_Cod")
            StrSQL.AppendLine("     , Imprese_Progetti.Sa_Cod")
            StrSQL.AppendLine("     , Imprese_Progetti.Appezza")
            StrSQL.AppendLine("     , Imprese_Progetti.Id_Reg")

            StrSQL.AppendLine("     , Imprese.Rag_Soc ")
            StrSQL.AppendLine("     , Imprese.Validita_Inizio Validita_Inizio_Azienda ")
            StrSQL.AppendLine("     , Imprese.Validita_Fine Validita_Fine_Azienda ")

            StrSQL.AppendLine("     , Centri_Aziendali.Sa_Nome ")
            StrSQL.AppendLine("     , Centri_Aziendali.Validita_Inizio Validita_Inizio_Centro ")
            StrSQL.AppendLine("     , Centri_Aziendali.Validita_Fine Validita_Fine_Centro ")

            StrSQL.AppendLine("     , Appezzamento.App_Nome")
            StrSQL.AppendLine("     , Appezzamento.Validita_Inizio Validita_Inizio_Appezzamento ")
            StrSQL.AppendLine("     , Appezzamento.Validita_Fine Validita_Fine_Appezzamento ")
            StrSQL.AppendLine($"     , CASE WHEN RegolamentoEsercizixAppezzamento.Regolamento <> {CInt(enum_Cod_Regolamento.Regolamento_bio)} THEN 0 ELSE 1 END AS SoloEserciziBio ")
            StrSQL.AppendLine($"     , ISNULL(MetodoProduzione.Val_Cod, '{CInt(enum_MetodoProduzione.Integrato)}') AS MetodoProduzione ")

            StrSQL.AppendLine("     , CASE WHEN ISNULL(Appezzamento.campo_cod, 0) <> 0 THEN 1 ELSE 0 END AS hasCampo  ")
            StrSQL.AppendLine("     , ISNULL(Campi.Campo_Des, '') AS Campo_Des  ")
            StrSQL.AppendLine("     , Campi.Validita_Inizio Validita_Inizio_Campo ")
            StrSQL.AppendLine("     , Campi.Validita_Fine Validita_Fine_Campo")

            StrSQL.AppendLine("     , Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto")
            StrSQL.AppendLine("     , Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto")
            StrSQL.AppendLine("     , Reg_Impianti.Cul_Cod")
            StrSQL.AppendLine("     , ISNULL(Cultivar.Cul_Des, '') AS Cul_Des")
            StrSQL.AppendLine("     , ISNULL(SpecieVegetali.Veg_Des, '') AS veg_des")
            StrSQL.AppendLine("     , ISNULL(DestinazioneUsoCod.id_cod, 0) AS id_cod ")
            StrSQL.AppendLine("     , ISNULL(DestinazioneUsoDes.descrizione, '') AS destinazioneUso ")

            StrSQL.AppendLine("     , Imprese_Progetti.Validita_Inizio Validita_Inizio_Esercizio ")
            StrSQL.AppendLine("     , Imprese_Progetti.Validita_Fine Validita_Fine_Esercizio")
            StrSQL.AppendLine("     , ISNULL(Progetto_Nome, '') AS Lotto")
            StrSQL.AppendLine("     , ISNULL(EsercizioChiuso.val_cod, '0') AS EsercizioChiuso ")
            StrSQL.AppendLine("     , Imprese_Progetti.Regolamento_Cod AS Regolamento ")

            If almenoUnParametroEsercizio Then
                'Se devo modificare gli esercizi mi serve sapere quale di questi è da modificare e quale da saltare
                StrSQL.AppendLine("     , CASE WHEN EserciziPostFiltroValidita.Piva IS NULL THEN 0 ELSE 1 END AS ModificaEsercizioPostFiltroValidita ")
            Else
                StrSQL.AppendLine("     , 1 AS ModificaEsercizioPostFiltroValidita ")

            End If

            StrSQL.AppendLine(" FROM Imprese_Progetti")
            StrSQL.AppendLine(" --Se stiamo modificando almeno un parametro esercizio devo andare in left join con le chiavi selezionate dalla griglia, per poter verificare tutti gli esercizi degli impianti ")
            StrSQL.AppendLine($" {If(almenoUnParametroEsercizio, "LEFT", "")} JOIN #TempEsercizio temp (NOLOCK) ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = temp.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = temp.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = temp.Id_Reg ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = temp.Progetto_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici EsercizioChiuso ON ")
            StrSQL.AppendLine("     EsercizioChiuso.PIVA = Imprese_Progetti.PIVA ")
            StrSQL.AppendLine(" AND EsercizioChiuso.sa_cod = Imprese_Progetti.sa_cod ")
            StrSQL.AppendLine(" AND EsercizioChiuso.appezza = Imprese_Progetti.appezza ")
            StrSQL.AppendLine(" AND EsercizioChiuso.ID_REG = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine(" AND EsercizioChiuso.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine($" AND EsercizioChiuso.id_cod = {CInt(enum_CodiciAnagrafe.Distinta_Chiusa)} ")

            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = Reg_Impianti.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg")
            StrSQL.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici DestinazioneUsoCod ON ")
            StrSQL.AppendLine("     DestinazioneUsoCod.PIVA = Reg_Impianti.PIVA ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.sa_cod = Reg_Impianti.sa_cod ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.appezza = Reg_Impianti.appezza ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.ID_REG = Reg_Impianti.Id_Reg ")
            StrSQL.AppendLine(" AND DestinazioneUsoCod.id_cod >= 3000 and DestinazioneUsoCod.id_cod < 4000 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe DestinazioneUsoDes ON DestinazioneUsoDes.codice = DestinazioneUsoCod.id_cod ")

            StrSQL.AppendLine(" INNER JOIN Appezzamento ON ")
            StrSQL.AppendLine("     Reg_Impianti.Piva = Appezzamento.Piva")
            StrSQL.AppendLine(" AND Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod")
            StrSQL.AppendLine(" AND Reg_Impianti.Appezza = Appezzamento.Appezza")

            StrSQL.AppendLine(" LEFT JOIN Campi ON ")
            StrSQL.AppendLine("     Campi.Piva = Appezzamento.Piva")
            StrSQL.AppendLine(" AND Campi.Sa_Cod = Appezzamento.Sa_Cod")
            StrSQL.AppendLine(" AND Campi.Campo_Cod = Appezzamento.Campo_Cod")

            StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ON ")
            StrSQL.AppendLine("     Centri_Aziendali.Piva = Appezzamento.Piva")
            StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod")

            StrSQL.AppendLine(" INNER JOIN Imprese ON ")
            StrSQL.AppendLine("     Imprese.Piva = Appezzamento.Piva")

            StrSQL.AppendLine(" INNER JOIN #MetodoProduzione_TT MetodoProduzione ON ")
            StrSQL.AppendLine("     MetodoProduzione.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = Appezzamento.Piva ")
            StrSQL.AppendLine(" AND MetodoProduzione.Sa_Cod = Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" AND MetodoProduzione.Appezza = Appezzamento.Appezza ")

            StrSQL.AppendLine(" INNER JOIN #RegolamentoEsercizixAppezzamento_TT RegolamentoEsercizixAppezzamento ON ")
            StrSQL.AppendLine("     RegolamentoEsercizixAppezzamento.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = Appezzamento.Piva ")
            StrSQL.AppendLine(" AND RegolamentoEsercizixAppezzamento.Sa_Cod = Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" AND RegolamentoEsercizixAppezzamento.Appezza = Appezzamento.Appezza ")

            If almenoUnParametroEsercizio Then
                StrSQL.AppendLine(" LEFT JOIN #EserciziPostFiltroValidita_TT EserciziPostFiltroValidita ON ")
                StrSQL.AppendLine("     Imprese_Progetti.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = EserciziPostFiltroValidita.Piva ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = EserciziPostFiltroValidita.Sa_Cod ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = EserciziPostFiltroValidita.Appezza ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = EserciziPostFiltroValidita.Id_Reg ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = EserciziPostFiltroValidita.Progetto_Cod ")
            End If


            StrSQL.AppendLine("")

            StrSQL.AppendLine(" --Ordino per avere l'ultimo esercizio valido di ogni appezzamento come primo in lista")
            StrSQL.AppendLine(" ORDER BY Piva, Sa_Cod, Appezza, Validita_Fine_Esercizio DESC")

            If almenoUnParametroEsercizio Then
                StrSQL.AppendLine(" DROP TABLE #EserciziPostFiltroValidita_TT ")
            End If
            StrSQL.AppendLine(" DROP TABLE #MetodoProduzione_TT ")
            StrSQL.AppendLine(" DROP TABLE #RegolamentoEsercizixAppezzamento_TT ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(NomeRoutine, objParametri)

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

    Public Function Leggi_per_EditResaPrevista(listChiavi As List(Of (String, Integer, Integer, Integer, Integer)),
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.LeggiValiditaAnagrafica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(listChiavi, NomeRoutine, objParametri)


            StrSQL.Length = 0
            StrSQL.AppendLine("WITH axi AS (")
            StrSQL.AppendLine("    SELECT TOP 1 ")
            StrSQL.AppendLine("    AppezzamentixIndirizzi.cod_indirizzo")
            StrSQL.AppendLine("    , AppezzamentixIndirizzi.PIVA")
            StrSQL.AppendLine("    , AppezzamentixIndirizzi.sa_cod")
            StrSQL.AppendLine("    , AppezzamentixIndirizzi.appezza")
            StrSQL.AppendLine("    FROM AppezzamentixIndirizzi")
            StrSQL.AppendLine("JOIN #TempEsercizio temp (NOLOCK) ON")
            StrSQL.AppendLine("    AppezzamentixIndirizzi.PIVA COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva")
            StrSQL.AppendLine("    AND AppezzamentixIndirizzi.sa_cod = temp.Sa_Cod")
            StrSQL.AppendLine("    AND AppezzamentixIndirizzi.appezza = temp.Appezza")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine(", cxi AS (")
            StrSQL.AppendLine("    SELECT TOP 1 ")
            StrSQL.AppendLine("    CentrixIndirizzi.cod_indirizzo")
            StrSQL.AppendLine("    , CentrixIndirizzi.PIVA")
            StrSQL.AppendLine("    , CentrixIndirizzi.sa_cod")
            StrSQL.AppendLine("    FROM CentrixIndirizzi")
            StrSQL.AppendLine("JOIN #TempEsercizio temp (NOLOCK) ON")
            StrSQL.AppendLine("    CentrixIndirizzi.PIVA COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva")
            StrSQL.AppendLine("    AND CentrixIndirizzi.sa_cod = temp.Sa_Cod")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT MAX(ai.cod_indirizzo) cod_indirizzo, ai.PIVA, ai.sa_cod, ai.appezza, MAX(lp.REG) REG, MAX(i.pro_cod_istat) pro_cod_istat, MAX(i.stato) stato")
            StrSQL.AppendLine("INTO #AppezzamentixIndirizzi")
            StrSQL.AppendLine("FROM AppezzamentixIndirizzi ai")
            StrSQL.AppendLine("    INNER JOIN Indirizzi i ON ai.cod_indirizzo = i.cod_indirizzo")
            StrSQL.AppendLine("    LEFT JOIN Lista_Province lp ON lp.PROV = i.pro_cod_istat")
            StrSQL.AppendLine("    GROUP BY ai.PIVA, ai.sa_cod, ai.appezza")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT MAX(ai.cod_indirizzo) cod_indirizzo, ai.PIVA, ai.sa_cod, MAX(lp.REG) REG, MAX(i.pro_cod_istat) pro_cod_istat, MAX(i.stato) stato")
            StrSQL.AppendLine("INTO #CentrixIndirizzi")
            StrSQL.AppendLine("FROM CentrixIndirizzi ai")
            StrSQL.AppendLine("    INNER JOIN Indirizzi i ON ai.cod_indirizzo = i.cod_indirizzo")
            StrSQL.AppendLine("    LEFT JOIN Lista_Province lp ON lp.PROV = i.pro_cod_istat")
            StrSQL.AppendLine("    GROUP BY ai.PIVA, ai.sa_cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("       Imprese_Progetti.Piva")
            StrSQL.AppendLine("     , Imprese_Progetti.Progetto_Cod")
            StrSQL.AppendLine("     , Imprese_Progetti.Sa_Cod")
            StrSQL.AppendLine("     , Imprese_Progetti.Appezza")
            StrSQL.AppendLine("     , Imprese_Progetti.Id_Reg")
            StrSQL.AppendLine("     , Reg_Impianti.Cul_Cod")
            StrSQL.AppendLine("     , Reg_Impianti.GRFI_Cod")
            StrSQL.AppendLine("     , Reg_Impianti.GRVA_Cod_VEG")
            StrSQL.AppendLine("     , ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD")
            StrSQL.AppendLine("     , ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD")
            StrSQL.AppendLine("     , ISNULL(dettagliospeciepersonalizzato.val_cod, '0') AS dettSpeciePersonalizzatoCod")
            StrSQL.AppendLine("     , SpecieVegetali.Veg_Cod AS veg_cod")
            StrSQL.AppendLine("     , ISNULL(Cultivar.Cul_Des, '') AS Cul_Des")
            StrSQL.AppendLine("     , ISNULL(SpecieVegetali.Veg_Des, '') AS veg_des")
            StrSQL.AppendLine("     , Imprese_Progetti.Validita_Inizio Validita_Inizio_Esercizio")
            StrSQL.AppendLine("     , Imprese_Progetti.Validita_Fine Validita_Fine_Esercizio")
            StrSQL.AppendLine("     , ISNULL(Progetto_Nome, '') AS Lotto")
            StrSQL.AppendLine("     , Imprese_Progetti.Regolamento_Cod AS Regolamento")
            StrSQL.AppendLine("     , Imprese_Progetti.Stato_Impianto AS StatoCod")
            StrSQL.AppendLine("     , CASE WHEN axi.cod_indirizzo IS NOT NULL THEN axi.REG ELSE ISNULL(cxi.REG, '000') END AS REG")
            StrSQL.AppendLine("     , CASE WHEN axi.cod_indirizzo IS NOT NULL THEN axi.pro_cod_istat ELSE ISNULL(cxi.pro_cod_istat, '000') END AS pro_cod_istat")
            StrSQL.AppendLine("     , CASE WHEN axi.cod_indirizzo IS NOT NULL THEN axi.stato ELSE ISNULL(cxi.stato, '') END AS stato")
            StrSQL.AppendLine("FROM Imprese_Progetti")
            StrSQL.AppendLine("JOIN #TempEsercizio temp (NOLOCK) ON")
            StrSQL.AppendLine("    Imprese_Progetti.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva")
            StrSQL.AppendLine("    AND Imprese_Progetti.Sa_Cod = temp.Sa_Cod")
            StrSQL.AppendLine("    AND Imprese_Progetti.Appezza = temp.Appezza")
            StrSQL.AppendLine("    AND Imprese_Progetti.Id_Reg = temp.Id_Reg")
            StrSQL.AppendLine("    AND Imprese_Progetti.Progetto_Cod = temp.Progetto_Cod")
            StrSQL.AppendLine("INNER JOIN Reg_Impianti ON")
            StrSQL.AppendLine("    Imprese_Progetti.Piva = Reg_Impianti.Piva")
            StrSQL.AppendLine("    AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod")
            StrSQL.AppendLine("    AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza")
            StrSQL.AppendLine("    AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg")
            StrSQL.AppendLine("INNER JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod")
            StrSQL.AppendLine("INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod")
            StrSQL.AppendLine("LEFT JOIN Reg_Impianti_Codici dettagliospeciepersonalizzato ON")
            StrSQL.AppendLine("    dettagliospeciepersonalizzato.PIVA = Imprese_Progetti.PIVA")
            StrSQL.AppendLine("    AND dettagliospeciepersonalizzato.sa_cod = Imprese_Progetti.sa_cod")
            StrSQL.AppendLine("    AND dettagliospeciepersonalizzato.appezza = Imprese_Progetti.appezza")
            StrSQL.AppendLine("    AND dettagliospeciepersonalizzato.ID_REG = Imprese_Progetti.Id_Reg")
            StrSQL.AppendLine("    AND dettagliospeciepersonalizzato.Progetto_Cod = Imprese_Progetti.Progetto_Cod")
            StrSQL.AppendLine("    AND dettagliospeciepersonalizzato.id_cod = 1108")
            StrSQL.AppendLine("LEFT JOIN #AppezzamentixIndirizzi axi ON")
            StrSQL.AppendLine("    axi.PIVA = Imprese_Progetti.Piva")
            StrSQL.AppendLine("    AND axi.sa_cod = Imprese_Progetti.Sa_Cod")
            StrSQL.AppendLine("    AND axi.appezza = Imprese_Progetti.Appezza")
            StrSQL.AppendLine("LEFT JOIN #CentrixIndirizzi cxi ON")
            StrSQL.AppendLine("    cxi.PIVA = Imprese_Progetti.Piva")
            StrSQL.AppendLine("    AND cxi.sa_cod = Imprese_Progetti.Sa_Cod")
            StrSQL.AppendLine("ORDER BY Imprese_Progetti.Piva, Sa_Cod, Appezza, Validita_Fine_Esercizio DESC")
            StrSQL.AppendLine("DROP TABLE #AppezzamentixIndirizzi")
            StrSQL.AppendLine("DROP TABLE #CentrixIndirizzi")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(NomeRoutine, objParametri)

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

#End Region


    Public Function CalcolaResaHaPerImpianto(ByVal Piva As String,
                                             ByVal Veg_Cod As Integer,
                                             ByVal ID_Agenda As Integer,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Appezza As Integer,
                                             ByVal Id_Reg As Integer,
                                            ByVal Progetto_Cod As Integer,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        Const NomeRoutine = "AnagrafeDAL.Impresa_Progetti_R.CalcolaResaHaPerImpianto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xFiltroImpianti As New StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" Select ")

            strSql.AppendLine("  Cultivar.Veg_Cod ")
            'strSql.AppendLine(" , Reg_Impianti.Piva ")
            'strSql.AppendLine(" , Reg_Impianti.Sa_Cod ")
            'strSql.AppendLine(" , Reg_Impianti.Appezza ")
            'strSql.AppendLine(" , Reg_Impianti.Id_Reg ")
            'strSql.AppendLine(" , Imprese_Progetti.Progetto_Cod ")


            strSql.AppendLine(" , CASE  ")
            strSql.AppendLine("    WHEN SUM(ISNULL(Mov_Destinazioni.Qta,0)) > 0 Then SUM(ISNULL(Mov_Destinazioni.Qta,0)) ")
            strSql.AppendLine("    ELSE SUM(Imprese_Progetti.Produzione_Prevista * Reg_Impianti.sup_imp)   ")
            strSql.AppendLine("     END  ")
            strSql.AppendLine(" AS RESA_TOT    ")

            'Se ci sono raccolte viene considerata la sola superficie raccolta, non quella dell'impianto
            strSql.AppendLine(" , CASE  ")
            strSql.AppendLine("    WHEN SUM(ISNULL(Mov_Destinazioni.Qta2,0)) > 0 Then SUM(ISNULL(Mov_Destinazioni.Qta2,0)) ")
            strSql.AppendLine("    ELSE  SUM(Reg_Impianti.sup_imp)  " & vbCrLf)
            strSql.AppendLine("     END  ")
            strSql.AppendLine(" AS HA    ")

            'Se ci sono raccolte divido per la sola superficie raccolta, non per quella dell'impianto
            strSql.AppendLine(" , CASE  ")
            strSql.AppendLine("    WHEN SUM(ISNULL(Mov_Destinazioni.Qta,0)) > 0 AND SUM(ISNULL(Mov_Destinazioni.Qta2,0)) > 0 Then SUM(ISNULL(Mov_Destinazioni.Qta,0)) / SUM(ISNULL(Mov_Destinazioni.Qta2,0)) ")
            strSql.AppendLine("    ELSE  SUM(Imprese_Progetti.Produzione_Prevista)  ")
            strSql.AppendLine("     END  ")
            strSql.AppendLine(" AS RESA_HA    ")



            ' -------------
            ' ---- FROM
            ' -------------

            strSql.Append(" From Imprese_Progetti with(nolock) " & vbCrLf)

            strSql.AppendLine("  Join reg_impianti with(nolock) On ")
            strSql.AppendLine("  Imprese_Progetti.Piva = reg_impianti.Piva  ")
            strSql.AppendLine(" And Imprese_Progetti.Sa_Cod = reg_impianti.sa_cod  ")
            strSql.AppendLine(" And Imprese_Progetti.appezza = reg_impianti.appezza  ")
            strSql.AppendLine(" And Imprese_Progetti.id_reg  = reg_impianti.id_reg ")

            strSql.AppendLine(" JOIN Cultivar with(nolock) On reg_impianti.Cul_Cod = Cultivar.Cul_Cod  ")

            strSql.AppendLine(" Left Join Mov_Destinazioni with(nolock) On ")
            strSql.AppendLine(" Mov_Destinazioni.piva = Imprese_Progetti.piva ")
            strSql.AppendLine(" And Mov_Destinazioni.SA_COD = Imprese_Progetti.Sa_Cod ")
            strSql.AppendLine(" And Mov_Destinazioni.APPEZZA = Imprese_Progetti.Appezza ")
            strSql.AppendLine(" And Mov_Destinazioni.ID_Destinazione = Imprese_Progetti.Id_Reg  ")
            strSql.AppendLine(" AND Mov_Destinazioni.Qta != 0 ")

            strSql.AppendLine(" Left Join Agenda with(nolock) ")
            strSql.AppendLine(" On Agenda.id_agenda = Mov_Destinazioni.id_agenda ")
            strSql.AppendLine(" And Agenda.Lav_Cod = " & LAVCOD_RACCOLTA)
            strSql.AppendLine(" And   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            strSql.AppendLine(" Left Join Movimenti with(nolock) ")
            strSql.AppendLine(" On Agenda.piva  = Movimenti.piva  And ")
            strSql.AppendLine(" Agenda.id_Agenda = Movimenti.id_Agenda    ")
            strSql.AppendLine(" And Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio  ")
            strSql.AppendLine(" And Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine  ")
            strSql.AppendLine(" And Movimenti.Cau_Mov = '" & CAU_RILIEVO_RACCOLTA & "'")

            'WHERE
            strSql.AppendLine(" WHERE ")

            strSql.AppendLine(" (isnull(Agenda.id_agenda,0 ) = 0 Or Agenda.id_agenda in ")
            strSql.AppendLine(" ( select id_agenda from Movimenti Mov_Mag with(nolock)   ")
            strSql.AppendLine(" where  ")
            strSql.AppendLine(" Agenda.Id_Agenda = Mov_Mag.id_Agenda  ")
            strSql.AppendLine(" And Mov_Mag.Cau_Mov = '" & CAU_CARICO & "')) ")

            strSql.AppendLine(" And Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            Dim Inizio_Anno_Corrente = "01/01/" & Now.Year
            Dim Fine_Anno_Corrente = "31/12/" & Now.Year
            strSql.AppendLine(" AND ( ")
            strSql.AppendLine(" ( Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Date.Today))
            strSql.AppendLine(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Today) & " ) OR ")
            strSql.AppendLine(" ( Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Inizio_Anno_Corrente))
            strSql.AppendLine(" AND Imprese_Progetti.Validita_Fine <= " & Agro_SQL_SaveDate(Fine_Anno_Corrente) & " ) ")
            strSql.AppendLine(" ) ")

            xFiltroImpianti.Length = 0
            xFiltroImpianti.AppendLine(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))

            If Sa_Cod <> 0 AndAlso Appezza <> 0 AndAlso Id_Reg <> 0 AndAlso Progetto_Cod <> 0 Then
                'Filtro del solo esercizio passato come parametro per poter vedere il GHG di un esercizio
                xFiltroImpianti.AppendLine(" And   Imprese_Progetti.Sa_Cod = '" & Agro_SQL_SaveNum(Sa_Cod))
                xFiltroImpianti.AppendLine(" And   Imprese_Progetti.Appezza = '" & Agro_SQL_SaveNum(Appezza))
                xFiltroImpianti.AppendLine(" And   Imprese_Progetti.Id_Reg = '" & Agro_SQL_SaveNum(Id_Reg))
                xFiltroImpianti.AppendLine(" And   Imprese_Progetti.Progetto_Cod = '" & Agro_SQL_SaveNum(Progetto_Cod))

            Else
                If ID_Agenda <> 0 Then
                    'TODO occorre leggere tutti gli impianti dell'operazione di agenda passati
                    '       per poter vedere il GHG di un'operazione di agenda
                    'xFiltroImpianti = 
                End If
            End If

            strSql.AppendLine(xFiltroImpianti.ToString)

            'strSql.AppendLine(" Group by Imprese_Progetti.Piva, Imprese_Progetti.Sa_Cod, Imprese_Progetti.Appezza, Imprese_Progetti.Id_Reg, Imprese_Progetti.Progetto_Cod, Cultivar.Veg_Cod ")
            strSql.AppendLine(" Group by Cultivar.Veg_Cod ")

            '--------------------------------------------------------------------------

            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    Public Function Leggi_StimeProduzione_ColturaOrEsercizi(ByVal Piva As String,
                                            ByVal Veg_Cod As Integer,
                                            ByVal filtroProgetti As List(Of Integer),
                                                  ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_StimeProduzione_ColturaOrEsercizi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Dim Inizio_Anno_Corrente = "01/01/" & Now.Year
        Dim Fine_Anno_Corrente = "31/12/" & Now.Year

        Try

            ConnessioniTransazioni.ApriConnessione(True, objParametri)
            ''Utility.VerificaApriConnessione(objParametri, False)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroProgetti(), NomeRoutine)

            If Veg_Cod <> 0 Then
                StrSQL.Append(" INSERT INTO #TempProgetti (Progetto_Cod) ")
                StrSQL.Append(" SELECT progetto_cod FROM imprese_progetti ")
                StrSQL.Append(" JOIN Reg_impianti r_i ON ")
                StrSQL.Append(" r_i.piva = imprese_progetti.piva ")
                StrSQL.Append(" AND r_i.sa_cod = imprese_progetti.sa_cod ")
                StrSQL.Append(" AND r_i.appezza = imprese_progetti.appezza ")
                StrSQL.Append(" AND r_i.id_reg = imprese_progetti.id_reg ")
                StrSQL.Append(" JOIN cultivar var ON ")
                StrSQL.Append(" var.cul_cod = r_i.cul_cod ")
                StrSQL.Append(" WHERE ")
                StrSQL.Append(" var.veg_cod = " & Agro_SQL_SaveNum(Veg_Cod))
                StrSQL.AppendLine(" AND ( ")
                StrSQL.AppendLine(" ( Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Date.Today))
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Today) & " ) Or ")
                StrSQL.AppendLine(" ( Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Inizio_Anno_Corrente))
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine <= " & Agro_SQL_SaveDate(Fine_Anno_Corrente) & " ) ")
                StrSQL.AppendLine(" ) ")
            Else
                If Not IsNothing(filtroProgetti) AndAlso filtroProgetti.Any Then
                    Dim chunks = ChunkBy(Of Integer)(filtroProgetti, 1000)
                    For Each chunk In chunks
                        StrSQL.Append(" INSERT INTO #TempProgetti (Progetto_Cod) VALUES ")
                        For Each p As Integer In chunk
                            StrSQL.AppendLine(String.Format("({0}),", p))
                        Next
                        Dim strSqlInsert As String = StrSQL.ToString
                        strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                        StrSQL.Clear()
                        EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                    Next
                End If
            End If

            StrSQL.AppendLine(" ;WITH CTE_Sup_Abbattuta AS ( ")
            StrSQL.AppendLine(" 	SELECT SUM(Mov_Destinazioni.qta2) AS Sup_Abbattuta ")
            StrSQL.AppendLine(" 		 , Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione AS ID_Reg, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" 	FROM Agenda ")
            StrSQL.AppendLine(" 	JOIN Movimenti ON ")
            StrSQL.AppendLine(" 		Movimenti.PIVA = Agenda.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti.Sa_Cod = Agenda.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")

            StrSQL.AppendLine(" 	JOIN Movimenti_Dettagli ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Movimenti.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")

            StrSQL.AppendLine(" 	JOIN Mov_Destinazioni ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            StrSQL.AppendLine(" 	JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine(" 		Mov_Destinazioni.PIVA = Imprese_Progetti.PIVA ")
            StrSQL.AppendLine("   AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Mov_Destinazioni.APPEZZA = Imprese_Progetti.APPEZZA ")
            StrSQL.AppendLine("   AND Mov_Destinazioni.ID_destinazione = Imprese_Progetti.ID_REG ")
            StrSQL.AppendLine(" 	AND Movimenti.Data_Movimento >=  Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("   AND Movimenti.Data_Movimento <=  Imprese_Progetti.Validita_Fine ")

            StrSQL.AppendLine("    WHERE 1 = 1 ")
            StrSQL.AppendLine("    AND Agenda.Lav_Cod = " & LAVCOD_ABBATTIMENTOIMPIANTI & " ")
            StrSQL.AppendLine("    AND Movimenti.Cau_Mov = '" & CAU_LAVORAZIONE & "' ")
            StrSQL.AppendLine("    GROUP BY Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" CTE_Perc_Piante_Morte AS ( ")
            StrSQL.AppendLine(" 	SELECT SUM(Mov_Destinazioni.qta) AS Perc_Piante_Morte ")
            StrSQL.AppendLine(" 		 , Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione AS ID_Reg, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" 	FROM Agenda ")
            StrSQL.AppendLine(" 	JOIN Movimenti ON ")
            StrSQL.AppendLine(" 		Movimenti.PIVA = Agenda.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti.Sa_Cod = Agenda.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")

            StrSQL.AppendLine(" 	JOIN Movimenti_Dettagli ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Movimenti.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")

            StrSQL.AppendLine(" 	JOIN Mov_Destinazioni ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            StrSQL.AppendLine(" 	JOIN Mov_Dettaglio_Tecnico ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")

            StrSQL.AppendLine(" 	JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine(" 		Mov_Destinazioni.PIVA = Imprese_Progetti.PIVA ")
            StrSQL.AppendLine("     AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Mov_Destinazioni.APPEZZA = Imprese_Progetti.APPEZZA ")
            StrSQL.AppendLine("     AND Mov_Destinazioni.ID_destinazione = Imprese_Progetti.ID_REG ")
            StrSQL.AppendLine(" 	AND Movimenti.Data_Movimento >=  Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("     AND Movimenti.Data_Movimento <=  Imprese_Progetti.Validita_Fine ")

            StrSQL.AppendLine("    WHERE 1 = 1 ")
            StrSQL.AppendLine("    AND Agenda.Lav_Cod = " & LAVCOD_DANNI_RACCOLTA & " ")
            StrSQL.AppendLine("    AND Movimenti.Cau_Mov = '" & CAU_RILIEVO_RACCOLTA & "' ")
            'StrSQL.AppendLine("    AND Mov_Dettaglio_Tecnico.ff_clASse IN (28,29) ")
            StrSQL.AppendLine("    GROUP BY Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" ) ")

            ' Query Principale
            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" SELECT veg_cod, SUM(Sup_Imp) AS HA_TOT, SUM(Stime_Produzione) AS STIMA_PRODUZIONE_TOT FROM ( ")
            End If

            StrSQL.AppendLine(" SELECT DISTINCT ")

            StrSQL.AppendLine("   Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" , Cultivar. veg_cod ")
            StrSQL.AppendLine(" , Reg_Impianti.Sup_Imp ")

            StrSQL.AppendLine(" , CASE WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) < reg_impianti.sup_imp ") '1) ATTIVO
            StrSQL.AppendLine("     THEN ")

            StrSQL.AppendLine("        CASE WHEN ( Year(data_inizio_produzione) ) <= Year(Getdate()) ") '2) PRODUCING
            StrSQL.AppendLine("             THEN ")

            StrSQL.AppendLine("                 CASE  ")
            StrSQL.AppendLine("                 WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) > sup_imp OR ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) > 100 ")
            StrSQL.AppendLine("                 --DATI INCOERENTI: SUPERFICIE ABBATTUTA > SUP_IMP OPPURE % PIANTE MORTE > 100%  ")
            StrSQL.AppendLine("                     THEN 0 ")

            'Se ABBATTUTO PARZIALE + RILIEVO DANNI allora il calcolo diventa:
            'Stima prod = ((Ha impianto – Ha abbattimento) - (Ha impianto – Ha abbattimento) x %Danno) x Resa Impianto 
            StrSQL.AppendLine("                 WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) < sup_imp ") '3A) MORIA + ABBATTIMENTO PARZIALE
            StrSQL.AppendLine("                 AND (ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) > 0 AND ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) < 100) ")
            StrSQL.AppendLine("                     THEN ((sup_imp - ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0)) - ((sup_imp - ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0)) * ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0)/100))  * produzione_prevista ")
            'StrSQL.AppendLine("                 END ")

            'Se RILIEVO DANNI, che è espressa in percentuale, allora il calcolo diventa:
            'Stima prod = (Ha impianto – Ha Impianto x %Danno) x Resa Impianto 
            StrSQL.AppendLine("                 WHEN ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) > 0  AND ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) < 100 ") '3B) MORIA
            StrSQL.AppendLine("                     THEN (sup_imp - (sup_imp * ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0)) /100 * produzione_prevista) ")

            'Se ABBATTUTO PARZIALE allora il calcolo diventa:
            'Stima prod = (Ha impianto – Ha abbattimento) x Resa Impianto  
            StrSQL.AppendLine("                 WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) < sup_imp ") '3C) ABBATTIMENTO PARZIALE
            StrSQL.AppendLine("                     THEN (sup_imp - ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0)) * produzione_prevista ")

            StrSQL.AppendLine("                 ELSE ") '3D) NESSUN DANNO - TUTTO OK 
            StrSQL.AppendLine("                      imprese_progetti.produzione_prevista * reg_impianti.sup_imp ")
            StrSQL.AppendLine("                 END ")
            StrSQL.AppendLine("         ELSE ") '2) NON PRODUCING
            StrSQL.AppendLine("              0 ")
            StrSQL.AppendLine("         END ")
            StrSQL.AppendLine(" ELSE 0 ") '1) NON ATTIVO
            StrSQL.AppendLine(" END AS Stime_Produzione ")

            StrSQL.AppendLine(" FROM Imprese_Progetti with(nolock) ")

            StrSQL.AppendLine(" JOIN Reg_Impianti with(nolock) ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Piva = Reg_Impianti.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.id_reg = Reg_Impianti.id_reg ")

            StrSQL.AppendLine(" INNER JOIN #TempProgetti tempP ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Progetto_Cod = tempP.Progetto_Cod ")

            StrSQL.AppendLine(" INNER JOIN Cultivar with(nolock) On reg_impianti.Cul_Cod = Cultivar.Cul_Cod ")

            StrSQL.AppendLine(" LEFT JOIN CTE_Sup_Abbattuta ON ")
            StrSQL.AppendLine("	    CTE_Sup_Abbattuta.Piva = Imprese_Progetti.Piva ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.Appezza = Imprese_Progetti.Appezza ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.ID_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")

            StrSQL.AppendLine(" LEFT JOIN CTE_Perc_Piante_Morte ON ")
            StrSQL.AppendLine("	    CTE_Perc_Piante_Morte.Piva = Imprese_Progetti.Piva ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.Appezza = Imprese_Progetti.Appezza ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.ID_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")

            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" ) AS STIME_PRODUZIONE_ESERCIZIO ")

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" GROUP BY VEG_COD ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroProgetti, NomeRoutine)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception

            ' Rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)

        Finally

            ConnessioniTransazioni.ChiudiConnessione(objParametri)
            ''Utility.VerificaChiudiConnessione(objParametri, False)

        End Try

        Return DT

    End Function

    Public Function Leggi_filtro_TempEsercizio(ByVal listChiavi As List(Of (String, Integer, Integer, Integer, Integer)),
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                               Optional ByVal Validita_Fine As Date = AGRODATAFINE) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_R.Leggi_filtro_TempEsercizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            TempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(listChiavi, NomeRoutine, objParametri_Server)


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Imprese_Progetti ")

            StrSQL.AppendLine(" INNER JOIN #TempEsercizio temp ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Piva = temp.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = temp.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = temp.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = temp.Id_Reg ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = temp.Progetto_Cod ")

            StrSQL.AppendLine(" WHERE 1 = 1")

            If Validita_Inizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Inizio))
            End If

            If Validita_Fine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Fine))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(NomeRoutine, objParametri_Server)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

        Return DT
    End Function

End Class




'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Impresa_Progetti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'non usa il campo p_ha
    <Obsolete("Usare l'altra funzione Scrivi perché è quella più aggiornata")>
    Public Function Scrivi(ByVal Piva As String,
                        ByVal Progetto_Cod As Int32,
                        ByVal Progetto_Nome As String,
                        ByVal Progetto_Des As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Appezza As Int32,
                        ByVal Id_Reg As Int32,
                        ByVal Veg_Cod As Int32,
                        ByVal Grfi_Cod As Int32,
                        ByVal Cau_Progetto As String,
                        ByVal Cod_Conto As Int32,
                        ByVal Cod_Contratto As Int32,
                        ByVal CSProgetto_Cod As Int32,
                        ByVal Giudizio As String,
                        ByVal Data_Inizio_Prevista As Date,
                        ByVal Data_Fioritura_Prevista As Date,
                        ByVal Data_Fine_Prevista As Date,
                        ByVal Ricavi_Previsti As Decimal,
                        ByVal Produzione_Prevista As Decimal, ByVal Stato_Impianto As Int32,
                        ByVal Regolamento_Cod As Int32, ByVal Disciplinare_Cod As Int32,
                        ByVal Disciplinare_PubblicoPrivato As Int32,
                        ByVal Regolamento_Concimazioni_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Imprese_Progetti ")
            StrSQL.AppendLine("                    ( Piva,             Progetto_Cod,         Progetto_Des, ")
            StrSQL.AppendLine("                      Progetto_Nome,    Cau_Progetto,         Cod_Conto,  ")
            StrSQL.AppendLine("                      Giudizio,         Sa_Cod,               Appezza,   ")
            StrSQL.AppendLine("                      Id_Reg,           Data_Inizio_Prevista, Data_Fioritura_Prevista, Data_Fine_Prevista, ")
            StrSQL.AppendLine("                      Ricavi_Previsti,  Produzione_Prevista,  Veg_Cod,    ")
            StrSQL.AppendLine("                      Grfi_Cod,         Cod_Contratto,        CSProgetto_Cod,   ")
            StrSQL.AppendLine("                      Stato_Impianto,   Regolamento_Cod,      Disciplinare_Cod, Disciplinare_PubblicoPrivato, Regolamento_Concimazioni_Cod, ")
            StrSQL.AppendLine("                      Inviato,            DataInvio, ")
            StrSQL.AppendLine("                      Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                      UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                      Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
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
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")
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


    'non usa il campo p_ha
    <Obsolete("Usare l'altra funzione Modifica perché è quella aggiornata")>
    Public Function Modifica(ByVal Piva As String,
                            ByVal Progetto_Cod As Int32,
                            ByVal Progetto_Nome As String,
                            ByVal Progetto_Des As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Id_Reg As Int32,
                            ByVal Veg_Cod As Int32,
                            ByVal Grfi_Cod As Int32,
                            ByVal Cau_Progetto As String,
                            ByVal Cod_Conto As Int32,
                            ByVal Cod_Contratto As Int32,
                            ByVal CSProgetto_Cod As Int32,
                            ByVal Giudizio As String,
                            ByVal Data_Inizio_Prevista As Date,
                            ByVal Data_Fioritura_Prevista As Date,
                            ByVal Data_Fine_Prevista As Date,
                            ByVal Ricavi_Previsti As Decimal,
                            ByVal Produzione_Prevista As Decimal,
                            ByVal Stato_Impianto As Int32,
                            ByVal Regolamento_Cod As Int32,
                            ByVal Disciplinare_Cod As Int32,
                            ByVal Disciplinare_PubblicoPrivato As Int32,
                            ByVal Regolamento_Concimazioni_Cod As Int32,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Progetto_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Progetto_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    Progetto_Nome     = '" & Agro_SQL_SaveText(Progetto_Nome) & "'  ")
            StrSQL.AppendLine("   ,Progetto_Des      = '" & Agro_SQL_SaveText(Progetto_Des) & "'  ")
            StrSQL.AppendLine("   ,Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("   ,Appezza           =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.AppendLine("   ,Id_Reg            =  " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.AppendLine("   ,Cau_Progetto      = '" & Agro_SQL_SaveText(UCase(Cau_Progetto)) & "'  ")
            StrSQL.AppendLine("   ,Cod_Conto         =  " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            StrSQL.AppendLine("   ,Cod_Contratto     =  " & Agro_SQL_SaveNum(Cod_Contratto) & "  ")
            StrSQL.AppendLine("   ,CSProgetto_Cod    =  " & Agro_SQL_SaveNum(CSProgetto_Cod) & "  ")
            StrSQL.AppendLine("   ,Giudizio          = '" & Agro_SQL_SaveText(Giudizio) & "'  ")
            StrSQL.AppendLine("   ,Data_Inizio_Prevista   =  " & Agro_SQL_SaveDate(Data_Inizio_Prevista))
            StrSQL.AppendLine("   ,Data_Fioritura_Prevista     =  " & Agro_SQL_SaveDate(Data_Fioritura_Prevista))
            StrSQL.AppendLine("   ,Data_Fine_Prevista     =  " & Agro_SQL_SaveDate(Data_Fine_Prevista))
            StrSQL.AppendLine("   ,Ricavi_Previsti        =  " & Agro_SQL_SaveNum(Ricavi_Previsti) & "  ")
            StrSQL.AppendLine("   ,Produzione_Prevista    =  " & Agro_SQL_SaveNum(Produzione_Prevista) & "  ")
            StrSQL.AppendLine("   ,Stato_Impianto         =  " & Agro_SQL_SaveNum(Stato_Impianto) & "  ")
            StrSQL.AppendLine("   ,Regolamento_Cod        =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.AppendLine("   ,Disciplinare_Cod       =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            StrSQL.AppendLine("   ,Disciplinare_PubblicoPrivato       =  " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")
            StrSQL.AppendLine("   ,Regolamento_Concimazioni_Cod       =  " & Agro_SQL_SaveNum(Regolamento_Concimazioni_Cod) & "  ")
            StrSQL.AppendLine("   ,Inviato           = 0 ")
            StrSQL.AppendLine("   ,DataInvio         = Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE Piva         = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Progetto_Cod =  " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")

            '---------------------------------------------


            '----------------------------------------------------------------------
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


    '     usa il campo p_ha
    Public Function Scrivi(ByVal Piva As String,
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
                           Optional ByVal FlagSecondoRaccolto As Integer = 0,
                           Optional ByVal Mat_Cod As Integer = 0
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Scrivi()"

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

            StrSQL.AppendLine(" INSERT INTO Imprese_Progetti ")
            StrSQL.AppendLine("                    ( Piva,               Progetto_Cod,         Progetto_Des, ")
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
            StrSQL.AppendLine("                      Validita_Inizio,    Validita_Fine, Mat_Cod ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
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
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
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


    ' usa il campo p_ha
    Public Function Modifica(ByVal Piva As String,
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
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Sup_Prog As Decimal? = Nothing,
                             Optional ByVal FlagSecondoRaccolto As Integer? = Nothing
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Progetto_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Progetto_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    Progetto_Nome     = '" & Agro_SQL_SaveText(Progetto_Nome) & "'  ")
            StrSQL.AppendLine("   ,Progetto_Des      = '" & Agro_SQL_SaveText(Progetto_Des) & "'  ")
            StrSQL.AppendLine("   ,Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("   ,Appezza           =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.AppendLine("   ,Id_Reg            =  " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.AppendLine("   ,Cau_Progetto      = '" & Agro_SQL_SaveText(UCase(Cau_Progetto)) & "'  ")
            StrSQL.AppendLine("   ,Cod_Conto         =  " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            StrSQL.AppendLine("   ,Cod_Contratto     =  " & Agro_SQL_SaveNum(Cod_Contratto) & "  ")
            StrSQL.AppendLine("   ,CSProgetto_Cod    =  " & Agro_SQL_SaveNum(CSProgetto_Cod) & "  ")
            StrSQL.AppendLine("   ,Giudizio          = '" & Agro_SQL_SaveText(Giudizio) & "'  ")
            StrSQL.AppendLine("   ,Data_Inizio_Prevista   =  " & Agro_SQL_SaveDate(Data_Inizio_Prevista))
            StrSQL.AppendLine("   ,Data_Fioritura_Prevista     =  " & Agro_SQL_SaveDate(Data_Fioritura_Prevista))
            StrSQL.AppendLine("   ,Data_Fine_Prevista     =  " & Agro_SQL_SaveDate(Data_Fine_Prevista))
            StrSQL.AppendLine("   ,Ricavi_Previsti        =  " & Agro_SQL_SaveNum(Ricavi_Previsti) & "  ")
            StrSQL.AppendLine("   ,Produzione_Prevista    =  " & Agro_SQL_SaveNum(Produzione_Prevista) & "  ")
            StrSQL.AppendLine("   ,Stato_Impianto         =  " & Agro_SQL_SaveNum(Stato_Impianto) & "  ")
            StrSQL.AppendLine("   ,Regolamento_Cod        =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.AppendLine("   ,Disciplinare_Cod       =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            StrSQL.AppendLine("   ,Disciplinare_PubblicoPrivato       =  " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")
            StrSQL.AppendLine("   ,Regolamento_Concimazioni_Cod       =  " & Agro_SQL_SaveNum(Regolamento_Concimazioni_Cod) & "  ")
            StrSQL.AppendLine("   ,P_Ha                   =  " & Agro_SQL_SaveNum(P_Ha) & "  ")
            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            StrSQL.AppendLine("   ,P_HA_Femmine           =  " & Agro_SQL_SaveNum(P_HA_Femmine) & "  ")
            StrSQL.AppendLine("   ,P_HA_Maschi            =  " & Agro_SQL_SaveNum(P_HA_Maschi) & "  ")
            '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -


            If Not IsNothing(Sup_Prog) Then
                StrSQL.AppendLine("   ,Sup_Prog =  " & Agro_SQL_SaveNum(Sup_Prog) & " ")
            End If

            If Not IsNothing(FlagSecondoRaccolto) Then
                StrSQL.AppendLine("   ,FlagSecondoRaccolto =  " & Agro_SQL_SaveNum(FlagSecondoRaccolto) & " ")
            End If

            StrSQL.AppendLine("   ,Inviato           = 0 ")
            StrSQL.AppendLine("   ,DataInvio         = Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Piva         = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Progetto_Cod =  " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")

            '----------------------------------------------------------------------
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


    Public Function Cancella(ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Appezza As Int32,
                        ByVal Id_Reg As Int32,
                        ByVal Progetto_Cod As Int32,
                        ByVal Cod_Contratto As Int32,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                '---------------------------------------------
                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE  Imprese_Progetto_Fasi ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
                StrSQL.AppendLine("         ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE    Inviato >= 0 ")

                StrSQL.AppendLine(" AND EXISTS  ( SELECT * From Imprese_Progetti ")
                StrSQL.AppendLine("               WHERE Inviato >= 0 ")
                StrSQL.AppendLine("               AND Imprese_Progetti.Piva = Imprese_Progetto_Fasi.Piva ")
                StrSQL.AppendLine("               AND Imprese_Progetti.Progetto_Cod = Imprese_Progetto_Fasi.Progetto_Cod ")

                'StrSQL.AppendLine(" AND      Imprese_Progetto_Fasi.Progetto_Cod ")
                'StrSQL.AppendLine("           In  ( Select Progetto_Cod From Imprese_Progetti")
                'StrSQL.AppendLine("                 Where Inviato >= 0 ")

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                End If

                If Appezza <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                End If

                If Id_Reg <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                End If

                If Progetto_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                End If

                StrSQL.AppendLine(" ) ")    ' fine della Subquery


                '---------------------------------------------


                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------


                '---------------------------------------------
                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE  Imprese_Progetti ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("          Username_Modifica = '" & objParametri.UsernameOperazione & "'  ")
                StrSQL.AppendLine("         ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE    Inviato >= 0 ")

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                End If

                If Appezza <> 0 Then
                    StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                End If

                If Id_Reg <> 0 Then
                    StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                End If

                If Progetto_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                End If
                '---------------------------------------------


                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Else
                '---------------------------------------------
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE FROM Imprese_Progetto_Fasi ")
                StrSQL.AppendLine(" WHERE    1=1 ")

                StrSQL.AppendLine(" AND EXISTS  ( SELECT * From Imprese_Progetti ")
                StrSQL.AppendLine("               WHERE Inviato >= 0 ")
                StrSQL.AppendLine("               AND Imprese_Progetti.Piva = Imprese_Progetto_Fasi.Piva ")
                StrSQL.AppendLine("               AND Imprese_Progetti.Progetto_Cod = Imprese_Progetto_Fasi.Progetto_Cod ")

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                End If

                If Appezza <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                End If

                If Id_Reg <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                End If

                If Progetto_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                End If

                StrSQL.AppendLine(" ) ")
                '---------------------------------------------


                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------


                '---------------------------------------------
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE FROM  Imprese_Progetti ")
                StrSQL.AppendLine(" WHERE    1=1 ")

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                End If

                If Appezza <> 0 Then
                    StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                End If

                If Id_Reg <> 0 Then
                    StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                End If

                If Progetto_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                End If
                '---------------------------------------------


                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            End If
            '---------------------------------------------           

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella_Solo_Imprese_Progetti(ByVal Piva As String,
                                                    ByVal Sa_Cod As Int32,
                                                    ByVal Appezza As Int32,
                                                    ByVal Id_Reg As Int32,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Cancella_Solo_Imprese_Progetti()"

        '====================================================================================
        'Parametri opzionali :
        '   sa_cod = 0                 
        '   appezza = 0                
        '   Id_reg = 0                  
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE Imprese_Progetti ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND Inviato >= 0")
            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Imprese_Progetti ")
                StrSQL.AppendLine(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine("  AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine("  AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            '---------------------------------------------

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

    Public Function AggiornaValiditaFine(ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Appezza As Int32,
                                    ByVal Id_Reg As Int32,
                                    ByVal Progetto_Cod As Int32,
                                    ByVal Cod_Contratto As Int32,
                                    ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutti i progetti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti i progetti del centro aziendale
        '   Id_Reg = 0           =>  aggiorna tutti i progetti di tutti gli impianti
        '   Progetto_Cod = 0     =>  aggiorna tutti i progetti di un impianto
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
            StrSQL.AppendLine("UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            End If

            '---------------------------------------------

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Aggiorna_NomeProgetto(ByVal Progetto_Cod As Integer,
                                          ByVal Piva As String,
                                          ByVal NomeProgetto As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Aggiorna_NomeProgetto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("        UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("      , Progetto_Nome = '" & Agro_SQL_SaveText(NomeProgetto) & "' ")
            StrSQL.AppendLine(" WHERE     (Piva = '" & Agro_SQL_SaveText(Piva) & "' ) AND (Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " )")

            '---------------------------------------------

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_Lotto(ByVal Piva As String,
                          ByVal Sa_Cod As Long,
                          ByVal Appezza As Long,
                          ByVal Id_Reg As Long,
                          ByVal Data_Riferimento As String,
                          ByVal Progetto_Nome As String,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    Progetto_Nome     = '" & Agro_SQL_SaveText(Progetto_Nome) & "'  ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine(" WHERE Piva           = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Sa_Cod         =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.AppendLine(" AND   Appezza        =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            StrSQL.AppendLine(" AND   Id_Reg         =  " & Agro_SQL_SaveNum(Id_Reg) & "   ")

            If IsDate(Data_Riferimento) Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
            Else

                'Nessun vincolo temporale
            End If

            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function Modifica_Regolamento(ByVal Piva As String,
                          ByVal Sa_Cod As Long,
                          ByVal Appezza As Long,
                          ByVal Id_Reg As Long,
                          ByVal Data_Riferimento As String,
                          ByVal Regolamento_Cod As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_Regolamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    Regolamento_Cod     = '" & Agro_SQL_SaveText(Regolamento_Cod) & "'  ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine(" WHERE Piva           = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Sa_Cod         =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.AppendLine(" AND   Appezza        =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            StrSQL.AppendLine(" AND   Id_Reg         =  " & Agro_SQL_SaveNum(Id_Reg) & "   ")

            If IsDate(Data_Riferimento) Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
            Else

                'Nessun vincolo temporale
            End If



            If IsDate(Data_Riferimento) Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
            Else

                'Nessun vincolo temporale
            End If


            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function Modifica_Disciplinare(ByVal Piva As String,
                          ByVal Sa_Cod As Long,
                          ByVal Appezza As Long,
                          ByVal Id_Reg As Long,
                          ByVal Data_Riferimento As String,
                          ByVal Disciplinare_Cod As Integer,
                          ByVal Disciplinare_PubblicoPrivato As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_Disciplinare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    Disciplinare_Cod  =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            StrSQL.AppendLine("   ,Disciplinare_PubblicoPrivato  =  " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine(" WHERE Piva           = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Sa_Cod         =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.AppendLine(" AND   Appezza        =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            StrSQL.AppendLine(" AND   Id_Reg         =  " & Agro_SQL_SaveNum(Id_Reg) & "   ")


            If IsDate(Data_Riferimento) Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
            Else

                'Nessun vincolo temporale
            End If

            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function Modifica_Validita_Fine(ByVal Piva As String,
                                           ByVal Cod_Progetto As Long,
                                           ByVal Validita_Fine As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_Disciplinare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")

            StrSQL.AppendLine("   Validita_Fine         =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" , Data_Modifica         =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine(" , UserName_Modifica     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")

            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND  Progetto_Cod =  " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")

            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function Modifica_Validita_Inizio(ByVal Piva As String,
                                             ByVal Cod_Progetto As Long,
                                             ByVal Validita_Inizio As Date,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_Disciplinare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")

            StrSQL.AppendLine("   Validita_Inizio       =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine(" , Data_Modifica         =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine(" , UserName_Modifica     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")

            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Progetto_Cod = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")

            '----------------------------------------------------------------------
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

    '============================================================================
    Public Function Imprese_Progetti_MarcaComeInviato(ByVal Piva As String,
                                ByVal progetto_cod As Long,
                                ByVal Data_invio As DateTime,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_MarcaComeInviato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("     inviato         =  -2 ")
            StrSQL.AppendLine("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.AppendLine(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.AppendLine(" AND     progetto_cod   = " & Agro_SQL_SaveNum(progetto_cod) & "  ")


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


    '##############################################################################################
    Public Function Modifica_Validita_Fine2(ByVal Piva As String,
                            ByVal Sa_Cod As Long,
                            ByVal Appezza As Long,
                            ByVal Id_Reg As Long,
                            ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean


        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_Validita_Fine2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine(" Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" ,Data_Modifica       =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine(" ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine(" WHERE Progetto_Cod  = (SELECT TOP 1  Progetto_Cod " &
                        "                           FROM       Imprese_Progetti " &
                        "                           WHERE Piva           = '" & Agro_SQL_SaveText(Piva) & "'  " &
                        "                           AND   Sa_Cod   =  " & Agro_SQL_SaveNum(Sa_Cod) & "   " &
                        "                           AND   Appezza   =  " & Agro_SQL_SaveNum(Appezza) & "   " &
                        "                           AND   id_reg   =  " & Agro_SQL_SaveNum(Id_Reg) & "   " &
                        "                           ORDER BY Validita_Inizio DESC  ) ")


            '----------------------------------------------------------------------
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



    '##############################################################################################
    Public Function Modifica_StatoImpianto(ByVal Piva As String,
                                ByVal Sa_Cod As Long,
                                ByVal Appezza As Long,
                                ByVal Id_Reg As Long,
                                ByVal Data_Riferimento As String,
                                ByVal Stato_Impianto As Long,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_StatoImpianto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    Stato_Impianto    =  " & Agro_SQL_SaveNum(Stato_Impianto) & "  ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine(" WHERE Piva           = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Sa_Cod         =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.AppendLine(" AND   Appezza        =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            StrSQL.AppendLine(" AND   Id_Reg         =  " & Agro_SQL_SaveNum(Id_Reg) & "   ")


            If IsDate(Data_Riferimento) Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
            Else

                'Nessun vincolo temporale
            End If

            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function Modifica_x_PUA(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Appezza As Integer,
                                   ByVal Id_Reg As Integer,
                                   ByVal Progetto_Cod As Integer,
                                   ByVal Produzione_Prevista As Decimal,
                                   ByVal Ciclo As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_x_PUA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    Produzione_Prevista    =  " & Agro_SQL_SaveNum(Produzione_Prevista) & "  ")
            StrSQL.AppendLine("   , FlagSecondoRaccolto   =  " & Agro_SQL_SaveNum(Ciclo) & "  ")
            StrSQL.AppendLine("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine(" WHERE Piva           = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Sa_Cod         =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.AppendLine(" AND   Appezza        =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            StrSQL.AppendLine(" AND   Id_Reg         =  " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            StrSQL.AppendLine(" AND   Progetto_Cod   =  " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")

            '----------------------------------------------------------------------
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


    '##############################################################################################
    Public Function ModificaSingolo_CampoNumerico(ByVal Piva As String,
                                ByVal Sa_Cod As Long,
                                ByVal Appezza As Long,
                                ByVal Id_Reg As Long,
                                ByVal Progetto_Cod As Long,
                                ByVal NomeCampo As String,
                                ByVal CampoValoreNumerico As Long,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.ModificaSingolo_CampoNumerico()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("    " & NomeCampo & " =  " & IIf(Agro_SQL_SaveNum(CampoValoreNumerico, False) = -1, "Null", Agro_SQL_SaveNum(CampoValoreNumerico)) & " ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            End If

            '----------------------------------------------------------------------
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


    '##############################################################################################
    Public Function Modifica_Parametrizzata(ByVal Piva As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal Appezza As Int32,
                                          ByVal Id_Reg As Int32,
                                          ByVal Progetto_Cod As Int32,
                                          ByVal Campo As String,
                                          ByVal Valore As Object,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_Parametrizzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If




            '---------------------------------------------

            Dim TypeVal As Type = Valore.GetType()

            If TypeVal.Equals(Stringa) Then

                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "

            ElseIf TypeVal.Equals(Data) Then

                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "

            Else

                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "

            End If


            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Imprese_Progetti SET ")

            StrSQL.AppendLine(strAssegnamento)

            StrSQL.AppendLine("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.AppendLine(" AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If
            '---------------------------------------------
            '----------------------------------------------------------------------
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


    Public Function Modifica_Chiave(ByVal Piva As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal Appezza As Int32,
                                          ByVal Id_Reg As Int32,
                                          ByVal Progetto_Cod As Int32,
                                    ByVal Piva_OLD As String,
                                          ByVal Sa_Cod_OLD As Int32,
                                          ByVal Appezza_OLD As Int32,
                                          ByVal Id_Reg_OLD As Int32,
                                          ByVal Progetto_Cod_OLD As Int32,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Modifica_Parametrizzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Imprese_Progetti SET ")

            StrSQL.AppendLine(" Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.AppendLine(" ,Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" ,Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.AppendLine(" ,Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            StrSQL.AppendLine(" ,Progetto_Cod= " & Agro_SQL_SaveNum(Progetto_Cod) & " ")

            StrSQL.AppendLine("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva_OLD)) & "'")
            StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_OLD) & " ")
            StrSQL.AppendLine(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza_OLD) & " ")
            StrSQL.AppendLine(" AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg_OLD) & " ")
            StrSQL.AppendLine(" AND   Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod_OLD) & " ")
            '---------------------------------------------
            '----------------------------------------------------------------------
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

    '##############################################################################################
    'A T T E N Z I O N E !!!!!!!!!!!!!!!!!!!
    'NON USARE QUESTA FUNZIONE!
    'DEVE ESSERE UTILIZZATA SOLO DALLA PAGINA MultiModifica_Impianti.aspx
    'PERCHE' PUO' CAMBIARE SE VIENE GESTITA LA MODIFICA  DI NUOVI DATI DELLA DISTINTA
    Public Function GO_MultiModificaimpianti_ModificaDPIResaDataSeminaRaccolta(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Id_Reg As Integer,
                            ByVal Progetto_Cod As Integer,
                            ByVal Disciplinare_Cod As Integer,
                            ByVal Disciplinare_PubblicoPrivato As Integer,
                            ByVal Resa_Prevista As Decimal,
                            ByVal Data_Semina_Prevista As Date,
                            ByVal Data_Fioritura_Prevista As Date,
                            ByVal Data_Raccolta_Prevista As Date,
                            ByVal Regolamento_Concimazioni_Cod As Integer,
                            ByVal Regolamento_Cod As Integer,
                            ByVal Stato_Cod As Integer,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.GO_MultiModificaimpianti_ModificaDPIResaDataSeminaRaccolta()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Imprese_Progetti SET ")
            StrSQL.AppendLine("         Disciplinare_Cod  =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            StrSQL.AppendLine("         ,Disciplinare_PubblicoPrivato  =  " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")
            StrSQL.AppendLine("         ,Produzione_Prevista  =  " & Agro_SQL_SaveNum(Resa_Prevista) & "  ")
            StrSQL.AppendLine("         ,Data_Inizio_Prevista  =  " & Agro_SQL_SaveDate(Data_Semina_Prevista) & "  ")
            StrSQL.AppendLine("         ,Data_Fioritura_Prevista  =  " & Agro_SQL_SaveDate(Data_Fioritura_Prevista) & " ")
            StrSQL.AppendLine("         ,Data_Fine_Prevista  =  " & Agro_SQL_SaveDate(Data_Raccolta_Prevista) & " ")
            StrSQL.AppendLine("         ,Regolamento_Concimazioni_Cod  =  " & Agro_SQL_SaveNum(Regolamento_Concimazioni_Cod) & "  ")
            StrSQL.AppendLine("         ,Regolamento_Cod  =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.AppendLine("         ,Stato_Impianto  =  " & Agro_SQL_SaveNum(Stato_Cod) & "  ")
            StrSQL.AppendLine("         ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine(" WHERE Piva           = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Sa_Cod         =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.AppendLine(" AND   Appezza        =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            StrSQL.AppendLine(" AND   Id_Reg         =  " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            StrSQL.AppendLine(" AND   Progetto_Cod  =  " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")

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

    Public Function AggiornaStatoImpianto_Massiva(PIVA As String,
                                                  Data_Inizio As Date,
                                                  Data_Fine As Date,
                                                  objParametri_Server As AgronicaCoreParametri
                                                  ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Impresa_Progetti_W.AggiornaStatoImpianto_Massiva()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Imprese_Progetti  ")
            StrSQL.AppendLine(" SET Stato_Impianto = " & enum_Stato_Impianto.Impianto_Produzione)
            StrSQL.AppendLine(" WHERE CONVERT(VARCHAR, Piva) + '|' + CONVERT(VARCHAR, Sa_Cod) + '|' + CONVERT(VARCHAR, Appezza) + '|' + CONVERT(VARCHAR, Id_Reg)")
            StrSQL.AppendLine(" IN (")
            StrSQL.AppendLine("     SELECT  CONVERT(VARCHAR, ip.Piva) + '|' + CONVERT(VARCHAR, ip.Sa_Cod) + '|' + CONVERT(VARCHAR, ip.Appezza) + '|' + CONVERT(VARCHAR, ip.Id_Reg) AS CHIAVE")
            StrSQL.AppendLine("     FROM Reg_Impianti r")
            StrSQL.AppendLine("     INNER JOIN Imprese_Progetti ip")
            StrSQL.AppendLine("         	ON r.PIVA = ip.Piva AND r.SA_COD = ip.SA_COD AND r.APPEZZA = ip.APPEZZA AND r.ID_REG = ip.Id_Reg")
            StrSQL.AppendLine("     WHERE  1 = 1 ")
            StrSQL.AppendLine("     AND r.CUL_COD <> 0")
            StrSQL.AppendLine("     AND ip.Stato_Impianto = 0")
            StrSQL.AppendLine("     AND r.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            StrSQL.AppendLine("     AND r.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.AppendLine("     AND r.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function UpdateColonna_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer, Integer)),
                                          nomeColonna As String,
                                          valore As String,
                                          tipoDato As String,
                                          timeStamp As Date,
                                          ByVal objParametri_Server As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.UpdateColonna_Massivo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim flagConnessione, flagTransazione As Boolean

        Try

            If listChiavi.Count = 0 Then
                Throw New Exception("Parametro non corretto nella query (listChiavi obbligatorio)")
            End If

            Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(listChiavi, nomeRoutine, objParametri_Server)

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE A ")
            StrSQL.AppendLine("SET ")

            StrSQL.Append($" {nomeColonna} = ")
            Select Case tipoDato
                Case "string"
                    StrSQL.Append($"'{Agro_SQL_SaveText(valore)}'")
                Case "date"
                    StrSQL.Append(Agro_SQL_SaveDate(valore))
                Case "number"
                    StrSQL.Append(Agro_SQL_SaveNum(valore))
            End Select

            StrSQL.AppendLine($" , Data_Modifica = {Agro_SQL_SaveDateTime(timeStamp)} ")

            StrSQL.AppendLine(" FROM Imprese_Progetti A ")
            StrSQL.AppendLine(" JOIN #TempEsercizio temp (NOLOCK) ON  ")
            StrSQL.AppendLine("     A.Piva = temp.Piva ")
            StrSQL.AppendLine(" AND A.Sa_Cod = temp.Sa_Cod ")
            StrSQL.AppendLine(" AND A.Appezza = temp.Appezza ")
            StrSQL.AppendLine(" AND A.Id_Reg = temp.Id_Reg ")
            StrSQL.AppendLine(" AND A.Progetto_Cod = temp.Progetto_Cod ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(nomeRoutine, objParametri_Server)
            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)

        Catch ex As Exception
            ' Rollback
            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function UpdateModifyDateAndUsername(progettoCod As Int32,
                                                objServer As AgronicaCoreParametri) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.UpdateConstrain()"
        Dim StrSQL As New Text.StringBuilder

        Try
            StrSQL.AppendLine("    UPDATE Imprese_Progetti")
            StrSQL.AppendLine("    SET")
            StrSQL.AppendLine($"         Username_Modifica = '{Agro_SQL_SaveText(objServer.UtenteUsername)}'")
            StrSQL.AppendLine($"         , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine($"    WHERE Progetto_Cod = {Agro_SQL_SaveNum(progettoCod)}")

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function

#Region "ExerciseConstrain"
    Public Function UpdateConstrain(regolamentoCod As Integer,
                                    disciplinareCod As Integer,
                                    disciplinarePubblicoPrivato As Integer,
                                    regolamentoConcimazioniCod As Integer,
                                    piva As String,
                                    progettoCod As Int32,
                                    objServer As AgronicaCoreParametri) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.UpdateConstrain()"
        Dim StrSQL As New Text.StringBuilder

        Try
            StrSQL.AppendLine("    UPDATE Imprese_Progetti")
            StrSQL.AppendLine("    SET")
            StrSQL.AppendLine($"         Regolamento_Cod = {Agro_SQL_SaveNum(regolamentoCod)}")
            StrSQL.AppendLine($"         , Disciplinare_Cod = {Agro_SQL_SaveNum(disciplinareCod)}")
            StrSQL.AppendLine($"         , Disciplinare_PubblicoPrivato = {Agro_SQL_SaveNum(disciplinarePubblicoPrivato)}")
            StrSQL.AppendLine($"         , Regolamento_Concimazioni_Cod = {Agro_SQL_SaveNum(regolamentoConcimazioniCod)}")
            StrSQL.AppendLine($"         , Username_Modifica = '{Agro_SQL_SaveText(objServer.UtenteCodFiscale)}'")
            StrSQL.AppendLine($"         , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine($"    WHERE Piva = '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        AND Progetto_Cod = {Agro_SQL_SaveNum(progettoCod)}")

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function
#End Region

#Region "Entity Framework"

    Public Sub ScriviEFxAnagrafica(ByVal dati_distinta As String,
                                   ByVal piva As String,
                                   ByVal saCod As Integer,
                                   ByVal appezza As Integer,
                                   ByVal idReg As Integer,
                                   ByRef OUTPUT_Progetto_Cod As Integer,
                                   ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
                                   Optional NoteLog As String = "")

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.ScriviEFxAnagrafica()"
        Dim messaggioErrore As String = ""

        Try

            Dim imp_pro As New AgronicaCoreEntityFramework_POCO.Imprese_Progetti
            Dim distinta = Newtonsoft.Json.JsonConvert.DeserializeObject(dati_distinta)

            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim progetto_cod As Integer = agroDP.NuovoId_Tabella_EF(GiasContext, "IMPRESA_PROGETTO", 0, 200000000, objParametriServer)
            OUTPUT_Progetto_Cod = progetto_cod

            Dim progetto_nome = ""
            If Not IsNothing(distinta.getValue("Progetto_Nome")) Then
                progetto_nome = distinta.getValue("Progetto_Nome").ToString
            End If

            Dim progetto_des = ""
            If Not IsNothing(distinta.getValue("Progetto_Des")) Then
                progetto_des = distinta.getValue("Progetto_Des").ToString
            End If

            Dim validita_inizio_progetto = CDate(distinta.getValue("Validita_Inizio"))
            Dim validita_fine_progetto = CDate(distinta.getValue("Validita_Fine"))

            Dim regolamento_cod = 1
            If Not IsNothing(distinta.getValue("Regolamento_Cod")) AndAlso IsNumeric(distinta.getValue("Regolamento_Cod")) AndAlso CInt(distinta.getValue("Regolamento_Cod")) > 0 Then
                regolamento_cod = distinta.getValue("Regolamento_Cod")
            End If

            Dim Disciplinare_PubblicoPrivato = 0
            If Not IsNothing(distinta.getValue("Disciplinare_PubblicoPrivato")) AndAlso IsNumeric(distinta.getValue("Disciplinare_PubblicoPrivato")) Then
                Disciplinare_PubblicoPrivato = distinta.getValue("Disciplinare_PubblicoPrivato")
            End If

            Dim Regolamento_Concimazione_Cod = 0
            If Not IsNothing(distinta.getValue("Regolamento_Concimazione_Cod")) AndAlso IsNumeric(distinta.getValue("Regolamento_Concimazione_Cod")) Then
                Regolamento_Concimazione_Cod = distinta.getValue("Regolamento_Concimazione_Cod")
            End If

            Dim Disciplinare_Cod = 0
            If Not IsNothing(distinta.getValue("Disciplinare_Cod")) AndAlso IsNumeric(distinta.getValue("Disciplinare_Cod")) Then
                Disciplinare_Cod = distinta.getValue("Disciplinare_Cod")
            End If

            Dim stato_impianto = 0
            If Not IsNothing(distinta.getValue("stato_impianto")) AndAlso IsNumeric(distinta.getValue("stato_impianto")) Then
                stato_impianto = distinta.getValue("stato_impianto")
            End If

            Dim p_ha As Double = 0
            If Not IsNothing(distinta.getValue("p_ha")) AndAlso IsNumeric(distinta.getValue("p_ha")) Then
                p_ha = distinta.getValue("p_ha")
            End If

            Dim PianteHa = 0
            If Not IsNothing(distinta.getValue("PianteHa")) AndAlso IsNumeric(distinta.getValue("PianteHa")) Then
                PianteHa = distinta.getValue("PianteHa")
            End If

            Dim data_semina_prevista = AGRODATAINIZIO
            If Not IsNothing(distinta.getValue("data_semina_prevista")) AndAlso IsDate(distinta.getValue("data_semina_prevista")) Then
                data_semina_prevista = CDate(distinta.getValue("data_semina_prevista"))
                If data_semina_prevista < AGRODATAINIZIO Then
                    data_semina_prevista = AGRODATAINIZIO
                End If
            End If

            Dim data_raccolta_prevista = AGRODATAFINE
            If Not IsNothing(distinta.getValue("data_raccolta_prevista")) AndAlso IsDate(distinta.getValue("data_raccolta_prevista")) Then
                data_raccolta_prevista = distinta.getValue("data_raccolta_prevista")
                If data_raccolta_prevista < AGRODATAINIZIO Then
                    data_raccolta_prevista = AGRODATAFINE
                End If
            End If

            Dim data_fioritura_prevista = AGRODATAINIZIO
            If Not IsNothing(distinta.getValue("data_fioritura_prevista")) AndAlso IsDate(distinta.getValue("data_fioritura_prevista")) Then
                data_fioritura_prevista = distinta.getValue("data_fioritura_prevista")
                If data_fioritura_prevista < AGRODATAINIZIO Then
                    data_fioritura_prevista = AGRODATAINIZIO
                End If
            End If

            Dim Sup_Prog = 0
            If Not IsNothing(distinta.getValue("Sup_Prog")) AndAlso IsNumeric(distinta.getValue("Sup_Prog")) Then
                Sup_Prog = CDbl(distinta.getValue("Sup_Prog"))
            End If

            Dim FlagSecondoRaccolto = 0
            If Not IsNothing(distinta.getValue("FlagSecondoRaccolto")) AndAlso IsNumeric(distinta.getValue("FlagSecondoRaccolto")) Then
                FlagSecondoRaccolto = CInt(distinta.getValue("FlagSecondoRaccolto"))
            End If


            '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            Dim P_HA_Femmine As Double = 0
            If Not IsNothing(distinta.getValue("P_HA_Femmine")) AndAlso IsNumeric(distinta.getValue("P_HA_Femmine")) Then
                P_HA_Femmine = distinta.getValue("P_HA_Femmine")
            End If

            Dim P_HA_Maschi As Double = 0
            If Not IsNothing(distinta.getValue("P_HA_Maschi")) AndAlso IsNumeric(distinta.getValue("P_HA_Maschi")) Then
                P_HA_Maschi = distinta.getValue("P_HA_Maschi")
            End If
            '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -




            imp_pro.Piva = piva
            imp_pro.Sa_Cod = saCod
            imp_pro.Appezza = appezza
            imp_pro.Id_Reg = idReg
            imp_pro.Progetto_Cod = progetto_cod
            imp_pro.Progetto_Des = progetto_des
            imp_pro.Progetto_Nome = progetto_nome
            imp_pro.Cod_Contratto = 0
            imp_pro.Cod_Conto = 0
            imp_pro.Ricavi_Previsti = 0
            imp_pro.Produzione_Prevista = 0
            imp_pro.Cau_Progetto = 9100
            imp_pro.Giudizio = ""
            imp_pro.Data_Inizio_Prevista = data_semina_prevista
            imp_pro.Data_Fine_Prevista = data_raccolta_prevista
            imp_pro.Validita_Inizio = validita_inizio_progetto
            imp_pro.Validita_Fine = validita_fine_progetto
            imp_pro.inviato = 0
            imp_pro.Data_Creazione = DateTime.Now
            imp_pro.Data_Modifica = DateTime.Now
            imp_pro.Username_Creazione = objParametriServer.UsernameOperazione
            imp_pro.Username_Modifica = objParametriServer.UsernameOperazione
            imp_pro.Veg_Cod = 0
            imp_pro.Grfi_Cod = 0
            imp_pro.CSProgetto_Cod = 0
            imp_pro.Stato_Impianto = stato_impianto
            imp_pro.Regolamento_Cod = regolamento_cod
            imp_pro.Disciplinare_Cod = Disciplinare_Cod
            imp_pro.P_HA = p_ha
            imp_pro.Data_Fioritura_Prevista = data_fioritura_prevista
            imp_pro.Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato
            imp_pro.Regolamento_Concimazioni_Cod = Regolamento_Concimazione_Cod
            imp_pro.Sup_Prog = Sup_Prog
            imp_pro.FlagSecondoRaccolto = FlagSecondoRaccolto

            '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            imp_pro.P_HA_Femmine = P_HA_Femmine
            imp_pro.P_HA_Maschi = P_HA_Maschi
            '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -


            GiasContext.Imprese_Progetti.Add(imp_pro)
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                        imp_pro.Piva, CStr(imp_pro.Progetto_Cod),
                                                        CStr(imp_pro.Sa_Cod), CStr(imp_pro.Appezza),
                                                        CStr(imp_pro.Id_Reg), Nothing,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        objParametriServer, enum_Id_Servizio.GiasOnline,
                                                        note:=NoteLog)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub ModificaEFxAnagrafica(ByVal dati_distinta As String,
                                     ByVal piva As String,
                                     ByVal saCod As Integer,
                                     ByVal appezza As Integer,
                                     ByVal idReg As Integer,
                                     ByVal progettoCod As Integer,
                                     ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef GiasContext As Gias_DeveloperServer_Entities,
                                     Optional NoteLog As String = "")

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.ModificaEFxAnagrafica()"
        Dim messaggioErrore As String = ""

        Try

            Dim distinta = Newtonsoft.Json.JsonConvert.DeserializeObject(dati_distinta)

            Dim imp_prol = From imp In GiasContext.Imprese_Progetti
                           Where imp.Piva = piva And
                                 imp.Sa_Cod = saCod And
                                 imp.Appezza = appezza And
                                 imp.Id_Reg = idReg And
                                 imp.Progetto_Cod = progettoCod
                           Select imp

            If imp_prol.Count = 0 Then
                Throw New Exception("Non è stata trovata la distinta con codice:" & progettoCod)
            End If

            Dim imp_pro = imp_prol.FirstOrDefault

            Dim progetto_nome = distinta.getValue("Progetto_Nome").ToString
            Dim progetto_des = distinta.getValue("Progetto_Des").ToString
            Dim validita_inizio_progetto = DateTime.Parse(distinta.getValue("Validita_Inizio"))
            Dim validita_fine_progetto As Date = AGRODATAFINE
            If IsDate(distinta.GetValue("Validita_Fine").ToString) Then
                validita_fine_progetto = CDate(distinta.GetValue("Validita_Fine").ToString)
            End If
            Dim regolamento_cod = 0
            If IsNumeric(distinta.getValue("Regolamento_Cod")) Then
                regolamento_cod = CInt(distinta.getValue("Regolamento_Cod"))
            End If

            Dim Disciplinare_PubblicoPrivato = 0
            If IsNumeric(distinta.getValue("Disciplinare_PubblicoPrivato")) Then
                Disciplinare_PubblicoPrivato = CInt(distinta.getValue("Disciplinare_PubblicoPrivato"))
            End If

            Dim Regolamento_Concimazione_Cod = 0
            If IsNumeric(distinta.getValue("Regolamento_Concimazione_Cod")) Then
                Regolamento_Concimazione_Cod = distinta.getValue("Regolamento_Concimazione_Cod")
            End If

            Dim Produzione_Prevista = 0
            If IsNumeric(distinta.getValue("produzione_prevista")) Then
                Produzione_Prevista = distinta.getValue("produzione_prevista")
            End If

            Dim Disciplinare_Cod = 0
            If IsNumeric(distinta.getValue("Disciplinare_Cod")) Then
                Disciplinare_Cod = distinta.getValue("Disciplinare_Cod")
            End If

            Dim stato_impianto = 0
            If IsNumeric(distinta.getValue("stato_impianto")) Then
                stato_impianto = CInt(distinta.getValue("stato_impianto"))
            End If

            Dim p_ha As Double = 0
            If IsNumeric(distinta.getValue("p_ha")) Then
                p_ha = distinta.getValue("p_ha")
            End If

            Dim PianteHa = 0
            If IsNumeric(distinta.getValue("PianteHa")) Then
                PianteHa = distinta.getValue("PianteHa")
            End If

            '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            Dim P_HA_Femmine As Double = 0
            If Not IsDBNull(distinta.getValue("P_HA_Femmine")) AndAlso
               IsNumeric(distinta.getValue("P_HA_Femmine")) AndAlso
               Not IsNothing(distinta.getValue("P_HA_Femmine")) Then
                P_HA_Femmine = distinta.getValue("P_HA_Femmine")
            End If

            Dim P_HA_Maschi As Double = 0
            If Not IsDBNull(distinta.getValue("P_HA_Maschi")) AndAlso
               IsNumeric(distinta.getValue("P_HA_Maschi")) AndAlso
               Not IsNothing(distinta.getValue("P_HA_Maschi")) Then
                P_HA_Maschi = distinta.getValue("P_HA_Maschi")
            End If
            '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -



            Dim data_semina_prevista = AGRODATAINIZIO
            Dim data_raccolta_prevista = AGRODATAFINE
            Dim data_fioritura_prevista = AGRODATAINIZIO

            If distinta.getValue("data_semina_prevista").ToString <> "" Then
                data_semina_prevista = DateTime.Parse(distinta.getValue("data_semina_prevista"))
            End If

            If distinta.getValue("data_raccolta_prevista").ToString <> "" Then
                data_raccolta_prevista = DateTime.Parse(distinta.getValue("data_raccolta_prevista"))
            End If

            If distinta.getValue("data_fioritura_prevista").ToString <> "" Then
                data_fioritura_prevista = DateTime.Parse(distinta.getValue("data_fioritura_prevista"))
            End If

            Dim FlagSecondoRaccolto = 0
            If Not IsNothing(distinta.getValue("FlagSecondoRaccolto")) AndAlso IsNumeric(distinta.getValue("FlagSecondoRaccolto")) Then
                FlagSecondoRaccolto = CInt(distinta.getValue("FlagSecondoRaccolto"))
            End If

            imp_pro.Piva = piva
            imp_pro.Sa_Cod = saCod
            imp_pro.Appezza = appezza
            imp_pro.Id_Reg = idReg
            imp_pro.Progetto_Cod = progettoCod
            imp_pro.Progetto_Des = progetto_des
            imp_pro.Progetto_Nome = progetto_nome
            imp_pro.Cod_Contratto = 0
            imp_pro.Cod_Conto = 0
            imp_pro.Ricavi_Previsti = 0
            imp_pro.Produzione_Prevista = Produzione_Prevista
            imp_pro.Cau_Progetto = 9100
            imp_pro.Giudizio = ""
            imp_pro.Data_Inizio_Prevista = data_semina_prevista
            imp_pro.Data_Fine_Prevista = data_raccolta_prevista
            imp_pro.Validita_Inizio = validita_inizio_progetto
            imp_pro.Validita_Fine = validita_fine_progetto
            imp_pro.inviato = 0
            imp_pro.Data_Creazione = DateTime.Now
            imp_pro.Data_Modifica = DateTime.Now
            imp_pro.Username_Creazione = objParametriServer.UsernameOperazione
            imp_pro.Username_Modifica = objParametriServer.UsernameOperazione
            imp_pro.Veg_Cod = 0
            imp_pro.Grfi_Cod = 0
            imp_pro.CSProgetto_Cod = 0
            imp_pro.Stato_Impianto = stato_impianto
            imp_pro.Regolamento_Cod = regolamento_cod
            imp_pro.Disciplinare_Cod = Disciplinare_Cod
            imp_pro.P_HA = p_ha
            imp_pro.Data_Fioritura_Prevista = data_fioritura_prevista
            imp_pro.Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato
            imp_pro.Regolamento_Concimazioni_Cod = Regolamento_Concimazione_Cod

            '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            imp_pro.P_HA_Femmine = P_HA_Femmine
            imp_pro.P_HA_Maschi = P_HA_Maschi
            '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            imp_pro.FlagSecondoRaccolto = FlagSecondoRaccolto


            If Not IsNothing(distinta.GetValue("Sup_Prog")) Then
                imp_pro.Sup_Prog = CDbl(distinta.GetValue("Sup_Prog"))
            End If

            GiasContext.Entry(imp_pro).State = EntityState.Modified
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                        imp_pro.Piva, CStr(imp_pro.Progetto_Cod),
                                                        CStr(imp_pro.Sa_Cod), CStr(imp_pro.Appezza),
                                                        CStr(imp_pro.Id_Reg), Nothing,
                                                        enum_TipoOperazioneDB.Modifica,
                                                        objParametriServer, enum_Id_Servizio.GiasOnline,
                                                        note:=NoteLog)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub EliminaEFxAnagrafica(ByRef distinta As Imprese_Progetti,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    Optional NoteLog As String = "")

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.EliminaEFxAnagrafica()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Entry(distinta).State = EntityState.Deleted
            GiasContext.Imprese_Progetti.Remove(distinta)

            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                        distinta.Piva, CStr(distinta.Progetto_Cod),
                                                        CStr(distinta.Sa_Cod), CStr(distinta.Appezza),
                                                        CStr(distinta.Id_Reg), Nothing,
                                                        enum_TipoOperazioneDB.Cancellazione,
                                                        objParametriServer, enum_Id_Servizio.GiasOnline,
                                                        note:=NoteLog)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub
#End Region

End Class
