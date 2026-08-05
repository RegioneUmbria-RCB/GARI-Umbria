Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PianoConti_EcoPat_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    'query utilizzata per il caricamento del menù a tendina dei conti per tutti gli anni
    Public Function Leggi_ContiEcoPat_DistinctCodContoPiuAnni(ByVal Piva_Ricl As String, _
                                                            ByVal AnnoMin As Integer, _
                                                            ByVal Codifica_Conto As Integer, _
                                                            ByVal Ric_Cod As Integer, _
                                                            ByVal Flag_UE As Integer, _
                                                            ByVal Cod_Contatto As String, _
                                                            ByVal Dare_Avere As String, _
                                                            ByVal Imputabile As Integer, _
                                                            ByVal FiltroContoDescr As String, _
                                                            ByVal xFiltroAggiuntivo As String, _
                                                            ByVal xOrderBy As String, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_EcoPat_R.Leggi_ContiEcoPat_DistinctCodContoPiuAnni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            '--------------------------------------------------------------------------
            '------------------ CONTI ECONOMICI ---------------------
            '--------------------------------------------------------------------------

            Stb.Append(" SELECT DISTINCT 'CE' AS Tipo, RicXConti.cod_conto, id_riclassificazione, Conto_descr, codifica_conto, Dare_avere " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Conti" & vbCrLf)
            Stb.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "'  " & vbCrLf)
            End If

            If AnnoMin <> 0 Then
                Stb.Append(" AND RicXConti.Anno >= " & Agro_SQL_SaveNum(AnnoMin) & "  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                Stb.Append(" AND RicXConti.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'  " & vbCrLf)
            End If

            If Imputabile <> CE_CONTO_IMPUTABILE_NOFILTRO Then
                Stb.Append(" AND RicXConti.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & "  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & "  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb.Append(" AND Conti.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & "  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                Stb.Append(" AND Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'  " & vbCrLf)
            End If

            If FiltroContoDescr <> "" Then
                Stb.Append(" AND Conti.Conto_Descr LIKE '%" & Agro_SQL_SaveText(FiltroContoDescr) & "%'  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            '------------------ UNION ---------------------
            '--------------------------------------------------------------------------
            Stb.Append(" " & vbCrLf)
            Stb.Append(" UNION ALL " & vbCrLf)
            Stb.Append(" " & vbCrLf)

            '--------------------------------------------------------------------------
            '------------------ CONTI PATRIMONIALI ---------------------
            '--------------------------------------------------------------------------

            Stb.Append(" SELECT DISTINCT 'SP' AS Tipo, RicxConti_Patrimonio.cod_conto_pat AS cod_conto, id_riclassificazione, Conto_pat_descr AS Conto_descr, codifica_conto_pat AS codifica_conto, Dare_avere " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Conti_Patrimonio " & vbCrLf)
            Stb.Append(" INNER JOIN RicxConti_Patrimonio ON Conti_Patrimonio.Cod_Conto_pat = RicxConti_Patrimonio.Cod_Conto_pat " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni_Patrimonio ON Riclassificazioni_Patrimonio.Ric_Cod_pat = RicxConti_Patrimonio.Ric_Cod_pat AND Riclassificazioni_Patrimonio.Piva = RicxConti_Patrimonio.Piva " & vbCrLf)
            Stb.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND RicxConti_Patrimonio.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "'  " & vbCrLf)
            End If

            If AnnoMin <> 0 Then
                Stb.Append(" AND RicxConti_Patrimonio.Anno >= " & Agro_SQL_SaveNum(AnnoMin) & "  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                Stb.Append(" AND RicxConti_Patrimonio.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'  " & vbCrLf)
            End If

            If Imputabile <> CE_CONTO_IMPUTABILE_NOFILTRO Then
                Stb.Append(" AND RicxConti_Patrimonio.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & "  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND Riclassificazioni_Patrimonio.Ric_Cod_pat = " & Agro_SQL_SaveNum(Ric_Cod) & "  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb.Append(" AND Conti_Patrimonio.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & "  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                Stb.Append(" AND Conti_Patrimonio.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'  " & vbCrLf)
            End If

            If FiltroContoDescr <> "" Then
                Stb.Append(" AND Conti_Patrimonio.Conto_Pat_Descr LIKE '%" & Agro_SQL_SaveText(FiltroContoDescr) & "%'  " & vbCrLf)
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY Tipo, Id_Riclassificazione, Conto_descr ")
            End If

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



End Class

Public Class PianoConti_Economici_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '###############################################################################
    'filtro aggiuntivo senza and
    Public Function PianoConti_ContoEconomico(ByVal Piva_Ricl As String, _
                                            ByVal Anno As Integer, _
                                            ByVal Id_Riclassificazione As String, _
                                            ByVal Dare_Avere As String, _
                                            ByVal Imputabile As Integer, _
                                            ByVal Ric_Cod As Integer, _
                                            ByVal Piva_Conti As String, _
                                            ByVal Cod_Conto As Integer, _
                                            ByVal Flag_UE As Integer, _
                                            ByVal Cod_Contatto As String, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Economici_R.PianoConti_ContoEconomico()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT     Conti.Cod_Conto, Conti.Piva AS piva_conti, Conti.Conto_Descr, Conti.Flag_UE, ISNULL(Conti.Cod_Contatto, '') AS Cod_Contatto,  " & vbCrLf)
            Stb.Append("  Conti.validita_inizio AS inizio_conti, Conti.validita_fine AS fine_conti, Conti.username_creazione AS username_conti, RicXConti.Piva AS piva_ric, " & vbCrLf)
            Stb.Append("  Imprese_Ric.rag_soc AS rag_soc_ric, RicXConti.Anno, RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo, RicXConti.Imputabile, " & vbCrLf)
            Stb.Append("   Riclassificazioni.Ric_Cod, Riclassificazioni.Ric_Des, RicXConti.validita_inizio AS inizio_ric, RicXConti.validita_fine AS fine_ric,  RicXConti.username_creazione AS username_ric " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Conti" & vbCrLf)
            Stb.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
            Stb.Append(" INNER JOIN  Imprese Imprese_Ric ON RicXConti.Piva = Imprese_Ric.PIVA " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Id_Riclassificazione <> "" Then
                Stb.Append(" AND (RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "')  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                Stb.Append(" AND (RicXConti.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "')  " & vbCrLf)
            End If

            If Imputabile <> CE_CONTO_IMPUTABILE_NOFILTRO Then
                Stb.Append(" AND (RicXConti.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND (Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Piva_Conti <> "" Then
                Stb.Append(" AND (Conti.Piva = '" & Agro_SQL_SaveText(Piva_Conti) & "')  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND (Conti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb.Append(" AND (Conti.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & ")  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                Stb.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY RicXConti.Piva ASC, RicXConti.Anno DESC, RicXConti.Id_Riclassificazione ASC ")
            End If

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



    '###############################################################################
    Public Function PianoConti_ContoEconomico_conSaldoTotale(ByVal Piva_Ricl As String, _
                                                            ByVal Anno As Integer, _
                                                            ByVal Id_Riclassificazione As String, _
                                                            ByVal Dare_Avere As String, _
                                                            ByVal Imputabile As Integer, _
                                                            ByVal Ric_Cod As Integer, _
                                                            ByVal Piva_Conti As String, _
                                                            ByVal Cod_Conto As Integer, _
                                                            ByVal Flag_UE As Integer, _
                                                            ByVal Cod_Contatto As String, _
                                                            ByVal xFiltroAggiuntivo As String, _
                                                            ByVal xOrderBy As String, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Economici_R.PianoConti_ContoEconomico_conSaldoTotale()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT     Conti.Cod_Conto, Conti.Piva AS piva_conti, Conti.Conto_Descr, Conti.Flag_UE, ISNULL(Conti.Cod_Contatto, '') AS Cod_Contatto,  " & vbCrLf)
            Stb.Append(" Conti.validita_inizio AS inizio_conti, Conti.validita_fine AS fine_conti, Conti.username_creazione AS username_conti, RicXConti.Piva AS piva_ric, " & vbCrLf)
            Stb.Append(" Imprese_Ric.rag_soc AS rag_soc_ric, RicXConti.Anno, RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo, RicXConti.Imputabile, " & vbCrLf)
            Stb.Append(" Riclassificazioni.Ric_Cod, Riclassificazioni.Ric_Des, RicXConti.validita_inizio AS inizio_ric, RicXConti.validita_fine AS fine_ric,  RicXConti.username_creazione AS username_ric " & vbCrLf)
            Stb.Append(" , ISNULL( ( SELECT SUM(RC_Int.Saldo) AS SaldoTotale " & vbCrLf)
            Stb.Append("            FROM    RicXConti RC_Int  " & vbCrLf)
            Stb.Append("            WHERE   RC_Int.Piva = RicXConti.Piva   " & vbCrLf)
            Stb.Append("            AND     RC_Int.Anno = RicXConti.anno  " & vbCrLf)
            Stb.Append("            AND     RC_Int.Id_Riclassificazione LIKE RicXConti.Id_Riclassificazione + '%'  " & vbCrLf)
            Stb.Append("            ) , 0 ) AS Saldo_totale " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Conti" & vbCrLf)
            Stb.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
            Stb.Append(" INNER JOIN  Imprese Imprese_Ric ON RicXConti.Piva = Imprese_Ric.PIVA " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Id_Riclassificazione <> "" Then
                Stb.Append(" AND (RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "')  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                Stb.Append(" AND (RicXConti.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "')  " & vbCrLf)
            End If

            If Imputabile <> 0 Then
                Stb.Append(" AND (RicXConti.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND (Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Piva_Conti <> "" Then
                Stb.Append(" AND (Conti.Piva = '" & Agro_SQL_SaveText(Piva_Conti) & "')  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND (Conti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb.Append(" AND (Conti.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & ")  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                Stb.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY RicXConti.Piva ASC, RicXConti.Anno DESC, RicXConti.Id_Riclassificazione ASC ")
            End If

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


    '###############################################################################
    Public Function PianoContiEco_xMovimenti(ByVal Conti_0Movimentati_1Tutti As Integer, _
                                            ByVal Data_Inizio As Date, _
                                            ByVal Data_Fine As Date, _
                                            ByVal Piva_Ricl As String, _
                                                ByVal Anno As Integer, _
                                                ByVal Id_Riclassificazione As String, _
                                                ByVal Dare_Avere As String, _
                                                ByVal Saldo As Decimal, _
                                                ByVal Imputabile As Integer, _
                                                ByVal Ric_Cod As Integer, _
                                                ByVal Piva_Conti As String, _
                                                ByVal Cod_Conto As Integer, _
                                                ByVal Flag_UE As Integer, _
                                                ByVal Cod_Contatto As String, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xOrderBy As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Economici_R.PianoContiEco_xMovimenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * " & vbCrLf)
            StrSQL.Append(" FROM " & vbCrLf)

            StrSQL.Append(" ( " & vbCrLf)

            '/*************************************************************************************
            '/**************** PIANO DEI CONTI: CONTI MOVIMENTATI  *************************
            '/*************************************************************************************

            StrSQL.Append(" ( " & vbCrLf)

            StrSQL.Append(" SELECT     Conti.Cod_Conto, Conti.Piva AS piva_conti,   " & vbCrLf)
            StrSQL.Append(" '' AS rag_soc_conti,  --Imprese_Conti.rag_soc AS rag_soc_conti,  " & vbCrLf)
            StrSQL.Append(" Conti.Conto_Descr, Conti.Flag_UE, Conti.Cod_Contatto,  " & vbCrLf)
            StrSQL.Append(" Conti.validita_inizio AS inizio_conti, Conti.validita_fine AS fine_conti, Conti.username_creazione AS username_conti, RicXConti.Piva AS piva_ric, " & vbCrLf)
            StrSQL.Append(" Imprese_Ric.rag_soc AS rag_soc_ric, RicXConti.Anno, RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo AS SaldoConto, 0 AS Saldo, RicXConti.Imputabile, " & vbCrLf)
            StrSQL.Append(" Riclassificazioni.Ric_Cod, Riclassificazioni.Ric_Des, RicXConti.validita_inizio AS inizio_ric, RicXConti.validita_fine AS fine_ric,  RicXConti.username_creazione AS username_ric, " & vbCrLf)
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append(" Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Movimenti.Cau_Mov, Movimenti.Mov_Desc,  " & vbCrLf)
            StrSQL.Append(" Movimenti.Data_Movimento, Movimenti.Num_Protocollo, Movimenti.Extra_Str, Movimenti.Extra_Int, Movimenti.Extra_Date,  " & vbCrLf)
            StrSQL.Append(" Movimenti_dettagli.Imponibile, Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Iva, " & vbCrLf)
            StrSQL.Append(" Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Prezzo_Unitario_Netto, Movimenti_dettagli.Qta, Movimenti_dettagli.Sconto, 0 AS Sconto_2,  " & vbCrLf)
            StrSQL.Append(" ISNULL(Movimenti_dettagli.Imponibile_Netto, 0 ) AS Importo_Riga, ISNULL(Movimenti_dettagli.Imponibile_Netto, 0 ) AS Imponibile_Netto, ISNULL(Movimenti_dettagli.Imponibile_Netto, 0 ) AS Importo_Movimento " & vbCrLf)
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append(" FROM Conti" & vbCrLf)
            StrSQL.Append(" -- LEFT OUTER JOIN  Imprese Imprese_Conti ON Imprese_Conti.PIVA = Conti.Piva " & vbCrLf)
            StrSQL.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
            StrSQL.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
            StrSQL.Append(" INNER JOIN  Imprese Imprese_Ric ON RicXConti.Piva = Imprese_Ric.PIVA " & vbCrLf)

            'JOIN MOVIMENTI DETTAGLI - RICXCONTI - CONTI 
            StrSQL.Append(" INNER JOIN  Movimenti_dettagli ON Movimenti_dettagli.PIVA = RicXConti.Piva AND Movimenti_dettagli.Anno = RicXConti.Anno AND Movimenti_dettagli.Ric_Cod = RicXConti.Ric_Cod " & vbCrLf)
            StrSQL.Append(" AND Movimenti_dettagli.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            StrSQL.Append(" INNER JOIN Movimenti " & vbCrLf)
            StrSQL.Append(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)

            'JOIN AGENDA - MOVIMENTI
            StrSQL.Append(" INNER JOIN Agenda " & vbCrLf)
            StrSQL.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda" & vbCrLf)

            'WHERE
            StrSQL.Append(" WHERE    Agenda.Lav_Cod NOT IN ( " &
                                                            CStr(LAVCOD_BOLLA_RICEVUTA) & "," &
                                                            CStr(LAVCOD_BOLLA_EMESSA) & "," &
                                                            CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & "," &
                                                            CStr(LAVCOD_CARICO) & "," &
                                                            CStr(LAVCOD_SCARICO) & "," &
                                                            CStr(LAVCOD_TRASFERIMENTO) & "," &
                                                            CStr(LAVCOD_CONFERIMENTO) & "," &
                                                            CStr(LAVCOD_CONFERIMENTO_DIVERSI) & "," &
                                                            CStr(LAVCOD_DOCO_EMESSO) & "," &
                                                            CStr(LAVCOD_DOCO_RICEVUTO) & "," &
                                                             CStr(LAVCOD_DAA_EMESSO) & "," &
                                                            CStr(LAVCOD_FATTURA_PROFORMA) &
                                                            ")" & vbCrLf)

            StrSQL.Append(" AND     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StrSQL.Append(" AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            If Piva_Ricl <> "" Then
                StrSQL.Append(" AND (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                StrSQL.Append(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Id_Riclassificazione <> "" Then
                StrSQL.Append(" AND (RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "')  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                StrSQL.Append(" AND (RicXConti.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "')  " & vbCrLf)
            End If

            If Saldo <> 0 Then
                StrSQL.Append(" AND (RicXConti.Saldo = " & Agro_SQL_SaveNum(Saldo) & ")  " & vbCrLf)
            End If

            If Imputabile <> 0 Then
                StrSQL.Append(" AND (RicXConti.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                StrSQL.Append(" AND (Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Piva_Conti <> "" Then
                StrSQL.Append(" AND (Conti.Piva = '" & Agro_SQL_SaveText(Piva_Conti) & "')  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                StrSQL.Append(" AND (Conti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                StrSQL.Append(" AND (Conti.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & ")  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                StrSQL.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If

            StrSQL.Append(" ) " & vbCrLf)

            If Conti_0Movimentati_1Tutti = 1 Then

                StrSQL.Append(" UNION ALL " & vbCrLf)

                '/*************************************************************************************
                '/**************** PIANO DEI CONTI: CONTI NON MOVIMENTATI  *************************
                '/*************************************************************************************

                StrSQL.Append(" (" & vbCrLf)


                StrSQL.Append(" SELECT     Conti.Cod_Conto, Conti.Piva AS piva_conti,   " & vbCrLf)
                StrSQL.Append(" '' AS rag_soc_conti,  --Imprese_Conti.rag_soc AS rag_soc_conti,  " & vbCrLf)
                StrSQL.Append(" Conti.Conto_Descr, Conti.Flag_UE, Conti.Cod_Contatto,  " & vbCrLf)
                StrSQL.Append(" Conti.validita_inizio AS inizio_conti, Conti.validita_fine AS fine_conti, Conti.username_creazione AS username_conti, RicXConti.Piva AS piva_ric, " & vbCrLf)
                StrSQL.Append(" Imprese_Ric.rag_soc AS rag_soc_ric, RicXConti.Anno, RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo AS SaldoConto, 0 AS Saldo, RicXConti.Imputabile, " & vbCrLf)
                StrSQL.Append(" Riclassificazioni.Ric_Cod, Riclassificazioni.Ric_Des, RicXConti.validita_inizio AS inizio_ric, RicXConti.validita_fine AS fine_ric,  RicXConti.username_creazione AS username_ric, " & vbCrLf)
                StrSQL.Append(" " & vbCrLf)
                StrSQL.Append(" 0 AS Id_Agenda, 0 AS Lav_Cod, '' AS des_lib, '' AS Cau_Mov, '' AS Mov_Desc,  " & vbCrLf)
                StrSQL.Append(" '01/01/1900' AS Data_Movimento, 0 AS Num_Protocollo, '' AS Extra_Str, 0 AS Extra_Int, '01/01/1900' AS Extra_Date,  " & vbCrLf)
                StrSQL.Append(" 0 AS Imponibile, 0 AS Cod_Iva, 0 AS Iva, " & vbCrLf)
                StrSQL.Append(" 0 AS Prezzo_Unitario, 0 AS Prezzo_Unitario_Netto, 0 AS Qta, 0 AS Sconto, 0 AS Sconto_2,  " & vbCrLf)
                StrSQL.Append(" 0 AS Importo_Riga, 0 AS Imponibile_Netto, 0 AS Importo_Movimento " & vbCrLf)
                StrSQL.Append(" " & vbCrLf)
                StrSQL.Append(" FROM Conti" & vbCrLf)
                StrSQL.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
                StrSQL.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
                StrSQL.Append(" INNER JOIN  Imprese Imprese_Ric ON RicXConti.Piva = Imprese_Ric.PIVA " & vbCrLf)

                'WHERE
                StrSQL.Append(" WHERE 1 = 1  " & vbCrLf)

                StrSQL.Append(" AND NOT EXISTS ( " & vbCrLf)
                StrSQL.Append("                 SELECT * " & vbCrLf)
                StrSQL.Append("                 FROM Movimenti_dettagli " & vbCrLf)
                StrSQL.Append("                 WHERE Movimenti_dettagli.PIVA = RicXConti.Piva " & vbCrLf)
                StrSQL.Append("                 AND Movimenti_dettagli.Anno = RicXConti.Anno " & vbCrLf)
                StrSQL.Append("                 AND Movimenti_dettagli.Ric_Cod = RicXConti.Ric_Cod " & vbCrLf)
                StrSQL.Append("                 AND Movimenti_dettagli.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
                StrSQL.Append("                 )   " & vbCrLf)

                'StrSQL.Append(" WHERE    Agenda.Lav_Cod NOT IN ( " + _
                '                                                CStr(LAVCOD_BOLLA_RICEVUTA) + "," + _
                '                                                CStr(LAVCOD_BOLLA_EMESSA) + "," + _
                '                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) + "," + _
                '                                                CStr(LAVCOD_CARICO) + "," + _
                '                                                CStr(LAVCOD_SCARICO) + "," + _
                '                                                CStr(LAVCOD_TRASFERIMENTO) + "," + _
                '                                                CStr(LAVCOD_CONFERIMENTO) + "," + _
                '                                                CStr(LAVCOD_CONFERIMENTO_DIVERSI) + "," + _
                '                                                CStr(LAVCOD_DOCO_EMESSO) + "," + _
                '                                                CStr(LAVCOD_DOCO_RICEVUTO) + "," + _
                '                                                 CStr(LAVCOD_DAA_EMESSO) + "," + _
                '                                                CStr(LAVCOD_FATTURA_PROFORMA) + _
                '                                                ")" & vbCrLf)

                'StrSQL.Append(" AND     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
                'StrSQL.Append(" AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

                If Piva_Ricl <> "" Then
                    StrSQL.Append(" AND (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
                End If

                If Anno <> 0 Then
                    StrSQL.Append(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
                End If

                If Id_Riclassificazione <> "" Then
                    StrSQL.Append(" AND (RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "')  " & vbCrLf)
                End If

                If Dare_Avere <> "" Then
                    StrSQL.Append(" AND (RicXConti.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "')  " & vbCrLf)
                End If

                If Saldo <> 0 Then
                    StrSQL.Append(" AND (RicXConti.Saldo = " & Agro_SQL_SaveNum(Saldo) & ")  " & vbCrLf)
                End If

                If Imputabile <> 0 Then
                    StrSQL.Append(" AND (RicXConti.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & ")  " & vbCrLf)
                End If

                If Ric_Cod <> 0 Then
                    StrSQL.Append(" AND (Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
                End If

                If Piva_Conti <> "" Then
                    StrSQL.Append(" AND (Conti.Piva = '" & Agro_SQL_SaveText(Piva_Conti) & "')  " & vbCrLf)
                End If

                If Cod_Conto <> 0 Then
                    StrSQL.Append(" AND (Conti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
                End If

                If Flag_UE <> CONTO_UE_NOFILTRO Then
                    StrSQL.Append(" AND (Conti.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & ")  " & vbCrLf)
                End If

                If Cod_Contatto <> "" Then
                    StrSQL.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
                End If

                StrSQL.Append(" )" & vbCrLf)

            End If

            '/*************************************************************************************

            StrSQL.Append(" ) CONTO_ECONOMICO " & vbCrLf)

            'StrSQL.Append(" ORDER BY RicXConti.Anno DESC, RicXConti.Id_Riclassificazione, Data_Movimento  " & vbCrLf)
            StrSQL.Append(" ORDER BY Anno DESC, Id_Riclassificazione ASC, Data_Movimento ASC " & vbCrLf)


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
    Public Function PianoContiEco_Riclassificazioni_2(ByVal Piva_Ricl As String, _
                                                ByVal Anno As Integer, _
                                                ByVal Ric_Cod As Integer, _
                                                ByVal Cod_Conto As Integer, _
                                                ByVal Cod_Contatto As String, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xOrderBy As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Economici_R.PianoContiEco_Riclassificazioni_2()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append("  SELECT DISTINCT Riclassificazioni.Ric_Cod, Riclassificazioni.Ric_Des " & vbCrLf)
            Stb.Append(" FROM Conti " & vbCrLf)
            Stb.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
            Stb.Append(" INNER JOIN  UtentiXImprese ON Riclassificazioni.Piva = UtentiXImprese.PIVA " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE   UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND (Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND (Conti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                Stb.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY  Riclassificazioni.Ric_Des  ")
            End If

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


    '####################################################################################################################
    Public Function ContoDescr_from_CodConto(ByVal Piva As String, _
                                            ByVal Ric_Cod As Integer, _
                                            ByVal Anno As Integer, _
                                            ByVal Cod_Conto As Integer, _
                                            ByVal Flag_VisualizzaID As Boolean, _
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                             ) As String

        Dim Dt_Conti As DataTable

        Dt_Conti = PianoConti_ContoEconomico(Piva, Anno,
                                            "",
                                             "",
                                            0,
                                            Ric_Cod,
                                            "",
                                            Cod_Conto,
                                            0,
                                            "",
                                            "", "",
                                            objParametri)


        If Not IsNothing(Dt_Conti) Then

            If Dt_Conti.Rows.Count <> 0 Then

                If Flag_VisualizzaID Then
                    Return Dt_Conti.Rows(0).Item("Id_Riclassificazione") & " - " & Dt_Conti.Rows(0).Item("Conto_Descr")
                Else
                    Return Dt_Conti.Rows(0).Item("Conto_Descr")
                End If
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    '###############################################################################
    Public Function ContoEconomico_SaldoConto_Leggi(ByVal Piva_Ricl As String, _
                                                    ByVal Anno As Integer, _
                                                    ByVal Id_Riclassificazione As String, _
                                                    ByVal xFiltroAggiuntivo As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Economici_R.ContoEconomico_SaldoConto_Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable
        Dim SaldoTotale As Decimal = 0

        Try

            Stb.Length = 0

            Stb.Append(" SELECT  SUM(RicxConti.Saldo) AS SaldoTotale " & vbCrLf)
            Stb.Append(" FROM Conti " & vbCrLf)
            Stb.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)

            Stb.Append(" WHERE   (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   " & vbCrLf)
            Stb.Append(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)

            'Nota Id_Riclassificazione = "0" -> Piano dei Conti (Radice dell'Albero)
            If Id_Riclassificazione <> "0" Then
                Stb.Append(" AND (RicXConti.Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%')  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT.Rows(0).Item("SaldoTotale")) Then
                SaldoTotale = CDbl(DT.Rows(0).Item("SaldoTotale"))
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return SaldoTotale

    End Function


    '###############################################################################
    Public Function PianoContiEco_AnnoContabile(ByVal Piva_Ricl As String,
                                                ByVal Ric_Cod As Integer,
                                                ByVal Cod_Conto As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Economici_R.PianoContiEco_AnnoContabile()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT DISTINCT Anno " & vbCrLf)
            Stb.Append(" FROM RicXConti " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "'   " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND RicXConti.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & "  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND RicXConti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY Anno ")
            End If

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

    '###############################################################################
    Public Function PianoContiEco_ConContatti(ByVal Piva_Ricl As String, _
                                            ByVal Anno As Integer, _
                                            ByVal Id_Riclassificazione As String, _
                                            ByVal Dare_Avere As String, _
                                            ByVal Imputabile As Integer, _
                                            ByVal Ric_Cod As Integer, _
                                            ByVal Piva_Conti As String, _
                                            ByVal Cod_Conto As Integer, _
                                            ByVal Flag_UE As Integer, _
                                            ByVal Cod_Contatto As String, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Economici_R.PianoContiEco_ConContatti()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT     Conti.Cod_Conto, Conti.Piva AS piva_conti, Conti.Conto_Descr, Conti.Flag_UE, ISNULL(Conti.Cod_Contatto, '') AS Cod_Contatto,  " & vbCrLf)
            Stb.Append("  Conti.validita_inizio AS inizio_conti, Conti.validita_fine AS fine_conti, Conti.username_creazione AS username_conti, RicXConti.Piva AS piva_ric, " & vbCrLf)
            Stb.Append("  Imprese_Ric.rag_soc AS rag_soc_ric, RicXConti.Anno, RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo, RicXConti.Imputabile, " & vbCrLf)
            Stb.Append("   Riclassificazioni.Ric_Cod, Riclassificazioni.Ric_Des, RicXConti.validita_inizio AS inizio_ric, RicXConti.validita_fine AS fine_ric,  RicXConti.username_creazione AS username_ric, " & vbCrLf)
            Stb.Append("  ISNULL(Contatti.Sa_Cod, -999) AS SaCod_Contatto,   ISNULL(Contatti.Id_CF, -999) AS IdCF_Contatto, " & vbCrLf)
            Stb.Append("  ISNULL(Contatti.Rag_Soc, '') AS RagSoc_Contatto,   ISNULL(Contatti.Codice_Fiscale, '') AS CodiceFiscale_Contatto " & vbCrLf)

            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Conti" & vbCrLf)
            Stb.Append(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva " & vbCrLf)
            Stb.Append(" INNER JOIN  Imprese Imprese_Ric ON RicXConti.Piva = Imprese_Ric.PIVA " & vbCrLf)
            Stb.Append(" LEFT OUTER JOIN Contatti ON Conti.Piva = Contatti.Piva AND Conti.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)

            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Id_Riclassificazione <> "" Then
                Stb.Append(" AND (RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "')  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                Stb.Append(" AND (RicXConti.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "')  " & vbCrLf)
            End If

            If Imputabile <> 0 Then
                Stb.Append(" AND (RicXConti.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND (Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Piva_Conti <> "" Then
                Stb.Append(" AND (Conti.Piva = '" & Agro_SQL_SaveText(Piva_Conti) & "')  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND (Conti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb.Append(" AND (Conti.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & ")  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                Stb.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY RicXConti.Piva ASC, RicXConti.Anno DESC, RicXConti.Id_Riclassificazione ASC ")
            End If

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


End Class

'###############################################################################
'###############################################################################
'###############################################################################
'###############################################################################
'###############################################################################
'###############################################################################


Public Class PianoConti_Patrimoniali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '###############################################################################
    Public Function PianoContiPat_Riclassificazioni(ByVal Piva_Ricl As String, _
                                                    ByVal Anno As Integer, _
                                                    ByVal Ric_Cod As Integer, _
                                                    ByVal Cod_Conto As Integer, _
                                                    ByVal Cod_Contatto As String, _
                                                    ByVal xFiltroAggiuntivo As String, _
                                                    ByVal xOrderBy As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Patrimoniali_R.PianoContiPat_Riclassificazioni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append("  SELECT DISTINCT Riclassificazioni_Patrimonio.Ric_Cod_Pat, Riclassificazioni_Patrimonio.Ric_Des_Pat " & vbCrLf)
            Stb.Append(" FROM Conti_Patrimonio " & vbCrLf)
            Stb.Append(" INNER JOIN RicXConti_Patrimonio ON Conti_Patrimonio.Cod_Conto_Pat = RicXConti_Patrimonio.Cod_Conto_pat " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni_Patrimonio ON Riclassificazioni_Patrimonio.Ric_Cod_Pat = RicXConti_Patrimonio.Ric_Cod_Pat AND Riclassificazioni_Patrimonio.Piva = RicXConti_Patrimonio.Piva " & vbCrLf)
            Stb.Append(" INNER JOIN  UtentiXImprese ON Riclassificazioni_Patrimonio.Piva = UtentiXImprese.PIVA " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE   UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND (RicXConti_Patrimonio.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND (RicXConti_Patrimonio.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND (Riclassificazioni_Patrimonio.Ric_Cod_Pat = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND (Conti_Patrimonio.Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                Stb.Append(" AND (Conti_Patrimonio.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY  Riclassificazioni_Patrimonio.Ric_Des_Pat  ")
            End If

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


    '###############################################################################
    Public Function PianoConti_StatoPatrimoniale(ByVal Piva_Ricl As String, _
                                                ByVal Anno As Integer, _
                                                ByVal Id_Riclassificazione As String, _
                                                ByVal Dare_Avere As String, _
                                                ByVal Imputabile As Integer, _
                                                ByVal Ric_Cod As Integer, _
                                                ByVal Piva_Conti As String, _
                                                ByVal Cod_Conto As Integer, _
                                                ByVal Flag_UE As Integer, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xOrderBy As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Patrimoniali_R.Bilancio_StatoPatrimoniale()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT Conti_Patrimonio.Cod_Conto_Pat, Conti_Patrimonio.Piva AS piva_conti, Conti_Patrimonio.Conto_Pat_Descr, Conti_Patrimonio.Flag_UE,   " & vbCrLf)
            Stb.Append(" Conti_Patrimonio.validita_inizio AS inizio_conti, Conti_Patrimonio.validita_fine AS fine_conti, Conti_Patrimonio.username_creazione AS username_conti, " & vbCrLf)
            Stb.Append(" Imprese_Ric.rag_soc AS rag_soc_ric,  " & vbCrLf)
            Stb.Append(" RicxConti_Patrimonio.Piva AS piva_ric, RicxConti_Patrimonio.Anno, RicxConti_Patrimonio.Id_Riclassificazione, RicxConti_Patrimonio.Dare_Avere, RicxConti_Patrimonio.Saldo, RicxConti_Patrimonio.Imputabile, " & vbCrLf)
            Stb.Append(" RicxConti_Patrimonio.validita_inizio AS inizio_ric, RicxConti_Patrimonio.validita_fine AS fine_ric,  RicxConti_Patrimonio.username_creazione AS username_ric, " & vbCrLf)
            Stb.Append(" Riclassificazioni_Patrimonio.Ric_Cod_Pat " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Conti_Patrimonio " & vbCrLf)
            Stb.Append(" INNER JOIN RicxConti_Patrimonio ON Conti_Patrimonio.Cod_Conto_Pat = RicxConti_Patrimonio.Cod_Conto_Pat " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni_Patrimonio ON Riclassificazioni_Patrimonio.Ric_Cod_Pat = RicxConti_Patrimonio.Ric_Cod_Pat AND Riclassificazioni_Patrimonio.Piva = RicxConti_Patrimonio.Piva " & vbCrLf)
            Stb.Append(" INNER JOIN  Imprese Imprese_Ric ON RicxConti_Patrimonio.Piva = Imprese_Ric.PIVA " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND (RicxConti_Patrimonio.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND (RicxConti_Patrimonio.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Id_Riclassificazione <> "" Then
                Stb.Append(" AND (RicxConti_Patrimonio.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "')  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                Stb.Append(" AND (RicxConti_Patrimonio.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "')  " & vbCrLf)
            End If

            If Imputabile <> SP_CONTO_IMPUTABILE_NOFILTRO Then
                Stb.Append(" AND (RicxConti_Patrimonio.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND (Riclassificazioni_Patrimonio.Ric_Cod_Pat = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Piva_Conti <> "" Then
                Stb.Append(" AND (Conti_Patrimonio.Piva = '" & Agro_SQL_SaveText(Piva_Conti) & "')  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND (Conti_Patrimonio.Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb.Append(" AND (Conti_Patrimonio.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & ")  " & vbCrLf)
            End If

            'If Cod_Contatto <> "" Then
            '    Stb.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY RicxConti_Patrimonio.Piva ASC, RicxConti_Patrimonio.Anno DESC, RicxConti_Patrimonio.Dare_Avere ASC, RicxConti_Patrimonio.Id_Riclassificazione ASC ")
            End If

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

    '###############################################################################
    Public Function Bilancio_StatoPatrimoniale_conSaldoTotale(ByVal Piva_Ricl As String, _
                                                            ByVal Anno As Integer, _
                                                            ByVal Id_Riclassificazione As String, _
                                                            ByVal Dare_Avere As String, _
                                                            ByVal Imputabile As Integer, _
                                                            ByVal Ric_Cod As Integer, _
                                                            ByVal Piva_Conti As String, _
                                                            ByVal Cod_Conto As Integer, _
                                                            ByVal Flag_UE As Integer, _
                                                            ByVal xFiltroAggiuntivo As String, _
                                                            ByVal xOrderBy As String, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PianoConti_Patrimoniali_R.Bilancio_StatoPatrimoniale_conSaldoTotale()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT Conti_Patrimonio.Cod_Conto_Pat, Conti_Patrimonio.Piva AS piva_conti, Conti_Patrimonio.Conto_Pat_Descr, Conti_Patrimonio.Flag_UE,   " & vbCrLf)
            Stb.Append(" Conti_Patrimonio.validita_inizio AS inizio_conti, Conti_Patrimonio.validita_fine AS fine_conti, Conti_Patrimonio.username_creazione AS username_conti, " & vbCrLf)
            Stb.Append(" Imprese_Ric.rag_soc AS rag_soc_ric,  " & vbCrLf)
            Stb.Append(" RicxConti_Patrimonio.Piva AS piva_ric, RicxConti_Patrimonio.Anno, RicxConti_Patrimonio.Id_Riclassificazione, RicxConti_Patrimonio.Dare_Avere, RicxConti_Patrimonio.Saldo, RicxConti_Patrimonio.Imputabile, " & vbCrLf)
            Stb.Append(" RicxConti_Patrimonio.validita_inizio AS inizio_ric, RicxConti_Patrimonio.validita_fine AS fine_ric,  RicxConti_Patrimonio.username_creazione AS username_ric, " & vbCrLf)
            Stb.Append(" Riclassificazioni_Patrimonio.Ric_Cod_Pat, " & vbCrLf)
            Stb.Append(" LEFT(id_riclassificazione,1) AS Id_riclassificazione_Padre " & vbCrLf)
            Stb.Append(" , ISNULL( ( SELECT Conto_Pat_Descr " & vbCrLf)
            Stb.Append("            FROM    Conti_Patrimonio CP  " & vbCrLf)
            Stb.Append("            INNER JOIN    RicxConti_Patrimonio RCP  " & vbCrLf)
            Stb.Append("            ON RCP.Cod_Conto_Pat = CP.Cod_Conto_Pat " & vbCrLf)
            Stb.Append("            WHERE   RCP.Piva = RicxConti_Patrimonio.Piva   " & vbCrLf)
            Stb.Append("            AND     RCP.Anno = RicxConti_Patrimonio.anno  " & vbCrLf)
            Stb.Append("            AND     RCP.ric_cod_Pat = RicxConti_Patrimonio.ric_cod_Pat  " & vbCrLf)
            Stb.Append("            AND     RCP.dare_avere = RicxConti_Patrimonio.dare_avere  " & vbCrLf)
            Stb.Append("            AND     RCP.Id_Riclassificazione = LEFT(RicxConti_Patrimonio.id_riclassificazione,1)  " & vbCrLf)
            Stb.Append("            ) , '' ) AS conto_descr_Padre " & vbCrLf)
            Stb.Append(" , ISNULL( ( SELECT SUM(RC_Int.Saldo) AS SaldoTotale " & vbCrLf)
            Stb.Append("            FROM    RicxConti_Patrimonio RC_Int  " & vbCrLf)
            Stb.Append("            WHERE   RC_Int.Piva = RicxConti_Patrimonio.Piva   " & vbCrLf)
            Stb.Append("            AND     RC_Int.Anno = RicxConti_Patrimonio.anno  " & vbCrLf)
            Stb.Append("            AND     RC_Int.ric_cod_Pat = RicxConti_Patrimonio.ric_cod_Pat  " & vbCrLf)
            Stb.Append("            AND     RC_Int.dare_avere = RicxConti_Patrimonio.dare_avere  " & vbCrLf)
            Stb.Append("            AND     RC_Int.Id_Riclassificazione LIKE RicxConti_Patrimonio.Id_Riclassificazione + '%'  " & vbCrLf)
            Stb.Append("            ) , 0 ) AS Saldo_totale " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Conti_Patrimonio " & vbCrLf)
            Stb.Append(" INNER JOIN RicxConti_Patrimonio ON Conti_Patrimonio.Cod_Conto_Pat = RicxConti_Patrimonio.Cod_Conto_Pat " & vbCrLf)
            Stb.Append(" INNER JOIN Riclassificazioni_Patrimonio ON Riclassificazioni_Patrimonio.Ric_Cod_Pat = RicxConti_Patrimonio.Ric_Cod_Pat AND Riclassificazioni_Patrimonio.Piva = RicxConti_Patrimonio.Piva " & vbCrLf)
            Stb.Append(" INNER JOIN  Imprese Imprese_Ric ON RicxConti_Patrimonio.Piva = Imprese_Ric.PIVA " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" WHERE 1 = 1 " & vbCrLf)

            If Piva_Ricl <> "" Then
                Stb.Append(" AND (RicxConti_Patrimonio.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')  " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND (RicxConti_Patrimonio.Anno = " & Agro_SQL_SaveNum(Anno) & ")  " & vbCrLf)
            End If

            If Id_Riclassificazione <> "" Then
                Stb.Append(" AND (RicxConti_Patrimonio.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "')  " & vbCrLf)
            End If

            If Dare_Avere <> "" Then
                Stb.Append(" AND (RicxConti_Patrimonio.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "')  " & vbCrLf)
            End If

            If Imputabile <> 0 Then
                Stb.Append(" AND (RicxConti_Patrimonio.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & ")  " & vbCrLf)
            End If

            If Ric_Cod <> 0 Then
                Stb.Append(" AND (Riclassificazioni_Patrimonio.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & ")  " & vbCrLf)
            End If

            If Piva_Conti <> "" Then
                Stb.Append(" AND (Conti_Patrimonio.Piva = '" & Agro_SQL_SaveText(Piva_Conti) & "')  " & vbCrLf)
            End If

            If Cod_Conto <> 0 Then
                Stb.Append(" AND (Conti_Patrimonio.Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto) & ")  " & vbCrLf)
            End If

            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb.Append(" AND (Conti_Patrimonio.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & ")  " & vbCrLf)
            End If

            'If Cod_Contatto <> "" Then
            '    Stb.Append(" AND (Conti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  " & vbCrLf)
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY RicxConti_Patrimonio.Piva ASC, RicxConti_Patrimonio.Anno DESC, RicxConti_Patrimonio.Dare_Avere ASC , RicxConti_Patrimonio.Id_Riclassificazione ASC ")
            End If

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


    '####################################################################################################################
    Public Function ContoPatDescr_from_CodContoPat(ByVal Piva As String, _
                                                    ByVal Ric_Cod As Integer, _
                                                    ByVal Anno As Integer, _
                                                    ByVal Cod_Conto As Integer, _
                                                    ByVal Flag_VisualizzaID As Boolean, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim Dt_Conti As DataTable

        Dt_Conti = PianoConti_StatoPatrimoniale(Piva, _
                                            Anno, _
                                            "", _
                                             "", _
                                            0, _
                                            Ric_Cod, _
                                            "", _
                                            Cod_Conto, _
                                            0, _
                                            "", "", _
                                            objParametri)


        If Not IsNothing(Dt_Conti) Then

            If Dt_Conti.Rows.Count <> 0 Then

                If Flag_VisualizzaID Then
                    Return Dt_Conti.Rows(0).Item("Id_Riclassificazione") & " - " & Dt_Conti.Rows(0).Item("Conto_Pat_Descr")
                Else
                    Return Dt_Conti.Rows(0).Item("Conto_Pat_Descr")
                End If
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

End Class
