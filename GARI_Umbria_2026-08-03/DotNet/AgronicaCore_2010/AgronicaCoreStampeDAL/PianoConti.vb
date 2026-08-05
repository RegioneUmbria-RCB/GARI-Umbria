Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PianoConti
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    'elenco dei movimenti dei conti patrimoniali nell'intervallo temporale
    'in ordine di data di registrazione
    Public Function EstrattoConto_Contatti(ByVal Data_Inizio As Date,
                                           ByVal Data_Fine As Date,
                                           ByVal Piva As String,
                                           ByVal Anno As Integer,
                                           ByVal Ric_Cod As Integer,
                                           ByVal Id_Riclassificazione As String,
                                           ByVal Dare_Avere As String,
                                           ByVal Cod_Conto As Integer,
                                           ByVal Cod_RisUm As Integer,
                                           ByVal Lista_CodRisUm As String,
                                           ByVal xFiltroAggiuntivo1 As String,
                                           ByVal xFiltroAggiuntivo2 As String,
                                           ByVal xFiltroAggiuntivo3 As String,
                                           ByVal xFiltroAggiuntivo4 As String,
                                           ByVal xOrderBy As String,
                                           ByVal DT_Codifiche_Pat As DataTable,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                           ) As DataTable

        '     ByVal Sezionale_Cod As Integer,
        '    ByVal CE_Imputabile As Integer,
        'ByVal SP_Imputabile As Integer,
        'ByVal DT_Codifiche_Eco As DataTable,

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.EstrattoConto_Contatti()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_QueryContiEco As New StringBuilder
        Dim Stb_QueryContiPat As New StringBuilder
        Dim dt As DataTable

        Try
            Stb_Globale.Length = 0
            Stb_QueryContiEco.Length = 0
            Stb_QueryContiPat.Length = 0

            Dim Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean = False

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiPat = Query_Conti_Patrimoniali_Movimentati(False,
                                                                     False,
                                                                     AGRODATAINIZIO,
                                                                     Data_Inizio,
                                                                     Data_Fine,
                                                                     Piva,
                                                                     Anno,
                                                                     Ric_Cod,
                                                                     Id_Riclassificazione,
                                                                     Dare_Avere,
                                                                     SP_CONTO_IMPUTABILE_NOFILTRO,
                                                                     Cod_Conto,
                                                                     CONTO_UE_NOFILTRO,
                                                                     "",
                                                                     SEZIONALE_NOFILTRO,
                                                                     Cod_RisUm,
                                                                     Lista_CodRisUm,
                                                                     CODLIQUIDITA_NOFILTRO,
                                                                     False,
                                                                     Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                                     xFiltroAggiuntivo1,
                                                                     xFiltroAggiuntivo2,
                                                                     xFiltroAggiuntivo3,
                                                                     xFiltroAggiuntivo4,
                                                                     DT_Codifiche_Pat,
                                                                     objParametri,
                                                                     flagNuoviArrotondamenti:=flagNuoviArrotondamenti)

            If Stb_QueryContiPat.ToString <> "" Then

                '///////////////////////////////////////////////
                Stb_Globale.AppendLine(" SELECT * ")
                Stb_Globale.AppendLine(" FROM ")

                Stb_Globale.AppendLine(" ( ")

                Stb_Globale.AppendLine(Stb_QueryContiPat.ToString)

                'Stb_Globale.AppendLine(" ")
                'Stb_Globale.AppendLine(" UNION ALL  ")
                'Stb_Globale.AppendLine(" ")
                'Stb_Globale.AppendLine(" ")

                'Stb_Globale.AppendLine(Stb_QueryContiEco.ToString)


                '/************************************************************************************
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ) EC_CONTATTI ")

                '14/08/2015: aggiunto id_agenda, altrimenti quando c'era fattura+pagamento (registrazione automatica) l'ordinamento veniva sballato
                Stb_Globale.AppendLine(" ORDER BY Data_Registrazione, Progr_Registrazione, id_agenda ")

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

            Else
                dt = Nothing
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    'elenco dei movimenti dei conti patrimoniali ed economici nell'intervallo temporale
    'in ordine di data di registrazione
    Public Function Libro_Giornale_Contabile(ByVal Data_Inizio As Date,
                                             ByVal Data_Fine As Date,
                                             ByVal Piva As String,
                                             ByVal Anno As Integer,
                                             ByVal Ric_Cod As Integer,
                                             ByVal Id_Riclassificazione As String,
                                             ByVal Dare_Avere As String,
                                             ByVal CE_Imputabile As Integer,
                                             ByVal SP_Imputabile As Integer,
                                             ByVal Cod_Conto As Integer,
                                             ByVal Flag_UE As Integer,
                                             ByVal Cod_Contatto As String,
                                             ByVal Sezionale_Cod As Integer,
                                             ByVal Cod_RisUm As Integer,
                                             ByVal Cod_Liquidita As Integer,
                                             ByVal xFiltroAggiuntivo1 As String,
                                             ByVal xFiltroAggiuntivo2 As String,
                                             ByVal xFiltroAggiuntivo3 As String,
                                             ByVal xFiltroAggiuntivo4 As String,
                                             ByVal xOrderBy As String,
                                             ByVal DT_Codifiche_Pat As DataTable,
                                             ByVal DT_Codifiche_Eco As DataTable,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                             ) As DataTable


        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Libro_Giornale_Contabile()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_QueryContiEco As New StringBuilder
        Dim Stb_QueryContiPat As New StringBuilder
        Dim dt As DataTable

        Try
            Stb_Globale.Length = 0
            Stb_QueryContiEco.Length = 0
            Stb_QueryContiPat.Length = 0

            Dim Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean = False

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiPat = Query_Conti_Patrimoniali_Movimentati(False,
                                                                     False,
                                                                     AGRODATAINIZIO,
                                                                     Data_Inizio,
                                                                     Data_Fine,
                                                                     Piva,
                                                                     Anno,
                                                                     Ric_Cod,
                                                                     Id_Riclassificazione,
                                                                     Dare_Avere,
                                                                     SP_Imputabile,
                                                                     Cod_Conto,
                                                                     Flag_UE,
                                                                     Cod_Contatto,
                                                                     Sezionale_Cod,
                                                                     Cod_RisUm,
                                                                     "",
                                                                     Cod_Liquidita,
                                                                     False,
                                                                     Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                                     xFiltroAggiuntivo1,
                                                                     xFiltroAggiuntivo2,
                                                                     xFiltroAggiuntivo3,
                                                                     xFiltroAggiuntivo4,
                                                                     DT_Codifiche_Pat,
                                                                     objParametri,
                                                                     flagNuoviArrotondamenti:=flagNuoviArrotondamenti)

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiEco = Query_Conti_Economici_Movimentati(False,
                                                                  False,
                                                                  AGRODATAINIZIO,
                                                                  Data_Inizio,
                                                                  Data_Fine,
                                                                  Piva,
                                                                  Anno,
                                                                  Ric_Cod,
                                                                  Id_Riclassificazione,
                                                                  Dare_Avere,
                                                                  CE_Imputabile,
                                                                  Cod_Conto,
                                                                  Flag_UE,
                                                                  Cod_Contatto,
                                                                  Sezionale_Cod,
                                                                  Cod_RisUm,
                                                                  False,
                                                                  False,
                                                                  xFiltroAggiuntivo1,
                                                                  xFiltroAggiuntivo2,
                                                                  xFiltroAggiuntivo3,
                                                                  xFiltroAggiuntivo4,
                                                                  DT_Codifiche_Eco,
                                                                  objParametri,
                                                                  flagNuoviArrotondamenti)

            '///////////////////////////////////////////////
            Stb_Globale.AppendLine(" SELECT * ")
            Stb_Globale.AppendLine(" FROM ")

            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_QueryContiPat.ToString)

            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" UNION ALL  ")
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ")

            Stb_Globale.AppendLine(Stb_QueryContiEco.ToString)


            '/************************************************************************************
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ) LIBRO_GIORNALE ")

            'ordinamento modificato l'11/08/2015: aggiunti Dare e Avere per vedere la registrazione in ordine
            '14/08/2015: aggiunto id_agenda, altrimenti quando c'era fattura+pagamento (registrazione automatica) l'ordinamento veniva sballato
            Stb_Globale.AppendLine(" ORDER BY Data_Registrazione, Progr_Registrazione, id_agenda, Dare DESC, Avere ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    'recupera il saldo, da agrodatainizio alla data si saldo, del conto selezionato
    'al momento non usata
    Public Function Recupera_Saldo_Conti_Economici(ByVal Data_Saldo As Date,
                                                   ByVal Flag_SOLOSaldiIniziali As Boolean,
                                                   ByVal GestCont_Flag_ConsideraSaldiIniziali As Boolean,
                                                   ByVal GestCont_DataInizio As Date,
                                                   ByVal Piva As String,
                                                   ByVal Anno As Integer,
                                                   ByVal Ric_Cod As Integer,
                                                   ByVal Id_Riclassificazione As String,
                                                   ByVal Dare_Avere As String,
                                                   ByVal Imputabile As Integer,
                                                   ByVal Cod_Conto As Integer,
                                                   ByVal Flag_UE As Integer,
                                                   ByVal Cod_Contatto As String,
                                                   ByVal Sezionale_Cod As Integer,
                                                   ByVal Sezionale_ChkDefault As Integer,
                                                   ByVal Cod_RisUm As Integer,
                                                   ByVal xFiltroAggiuntivo1 As String,
                                                   ByVal xFiltroAggiuntivo2 As String,
                                                   ByVal xFiltroAggiuntivo3 As String,
                                                   ByVal xFiltroAggiuntivo4 As String,
                                                   ByVal xOrderBy As String,
                                                   ByVal DT_Codifiche As DataTable,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Decimal

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Recupera_Saldo_Conti_Economici()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim saldo As Decimal = 0
        Dim i As Integer

        Try

            dt = Saldo_Conti_Economici(AGRODATAINIZIO,
                                       Data_Saldo,
                                       Flag_SOLOSaldiIniziali,
                                       GestCont_Flag_ConsideraSaldiIniziali,
                                       GestCont_DataInizio,
                                       Piva,
                                       Anno,
                                       Ric_Cod,
                                       Id_Riclassificazione,
                                       Dare_Avere,
                                       Imputabile,
                                       Cod_Conto,
                                       Flag_UE,
                                       Cod_Contatto,
                                       Sezionale_Cod,
                                       Sezionale_ChkDefault,
                                       Cod_RisUm,
                                       False,
                                       xFiltroAggiuntivo1,
                                       xFiltroAggiuntivo2,
                                       xFiltroAggiuntivo3,
                                       xFiltroAggiuntivo4,
                                       xOrderBy,
                                       DT_Codifiche,
                                       False,
                                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                For i = 0 To dt.Rows.Count - 1

                    saldo += dt.Rows(i).Item("Saldo")

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            saldo = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return saldo

    End Function


    '###############################################################################
    'calcola il saldo, da Data_Inizio_Saldo alla data di saldo, di ogni conto
    'Flag_LayoutCostiRicavi -> default = false
    Public Function Saldo_Conti_Economici(ByVal Data_Inizio_Saldo As Date,
                                          ByVal Data_Saldo As Date,
                                          ByVal Flag_SOLOSaldiIniziali As Boolean,
                                          ByVal GestCont_Flag_ConsideraSaldiIniziali As Boolean,
                                          ByVal GestCont_DataInizio As Date,
                                          ByVal Piva As String,
                                          ByVal Anno As Integer,
                                          ByVal Ric_Cod As Integer,
                                          ByVal Id_Riclassificazione As String,
                                          ByVal Dare_Avere As String,
                                          ByVal Imputabile As Integer,
                                          ByVal Cod_Conto As Integer,
                                          ByVal Flag_UE As Integer,
                                          ByVal Cod_Contatto As String,
                                          ByVal Sezionale_Cod As Integer,
                                          ByVal Sezionale_ChkDefault As Integer,
                                          ByVal Cod_RisUm As Integer,
                                          ByVal Flag_ContiSaldo0 As Boolean,
                                          ByVal FlagEscludiIvaIndetraibile As Boolean,
                                          ByVal xFiltroAggiuntivo1 As String,
                                          ByVal xFiltroAggiuntivo2 As String,
                                          ByVal xFiltroAggiuntivo3 As String,
                                          ByVal xFiltroAggiuntivo4 As String,
                                          ByVal DT_Codifiche As DataTable,
                                          ByVal Flag_LayoutCostiRicavi As Boolean,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByRef strOutput As String = "",
                                          Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Saldo_Conti_Economici()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_QueryContiXMov As New StringBuilder
        Dim dt As DataTable

        Try
            Dim Flag_LeggiSaldiIniziali As Boolean = True

            'se sull'impresa ho detto di non considerare i saldi
            If Not GestCont_Flag_ConsideraSaldiIniziali Then
                Flag_LeggiSaldiIniziali = False
            Else
                If Sezionale_Cod = SEZIONALE_NOFILTRO Then
                    'non c'è filtro sul sezionale, ok leggo i saldi
                Else
                    'altrimenti sto facendo un filtro sui sezionali
                    'e i saldi iniziali vanno stampati solo sul sezionale principale
                    If Sezionale_ChkDefault <> 1 Then
                        'se il sezionale non è quello di default
                        Flag_LeggiSaldiIniziali = False
                    End If
                End If
            End If


            Stb_Globale.Length = 0
            Stb_QueryContiXMov.Length = 0

            'il primo valore prima era fisso a true, ora usa Flag_LeggiSaldiIniziali

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiXMov = Query_Conti_Economici_Movimentati(Flag_LeggiSaldiIniziali,
                                                                   Flag_SOLOSaldiIniziali,
                                                                   GestCont_DataInizio,
                                                                   Data_Inizio_Saldo,
                                                                   Data_Saldo,
                                                                   Piva,
                                                                   Anno,
                                                                   Ric_Cod,
                                                                   Id_Riclassificazione,
                                                                   Dare_Avere,
                                                                   Imputabile,
                                                                   Cod_Conto,
                                                                   Flag_UE,
                                                                   Cod_Contatto,
                                                                   Sezionale_Cod,
                                                                   Cod_RisUm,
                                                                   Flag_ContiSaldo0,
                                                                   FlagEscludiIvaIndetraibile,
                                                                   xFiltroAggiuntivo1,
                                                                   xFiltroAggiuntivo2,
                                                                   xFiltroAggiuntivo3,
                                                                   xFiltroAggiuntivo4,
                                                                   DT_Codifiche,
                                                                   objParametri,
                                                                   flagNuoviArrotondamenti)

            '///////////////////////////////////////////////
            Stb_Globale.AppendLine(" SELECT CodiceSplitGruppo, Cod_Conto_Eco, Conto_Eco_Descr, Id_Riclassificazione, SUM(Dare) AS Saldo_Dare, SUM(Avere) AS Saldo_Avere, SUM(Avere) - SUM(Dare) AS Saldo, Dare_Avere, Flag_UE ")
            Stb_Globale.AppendLine(" FROM ")

            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_QueryContiXMov.ToString)


            '/************************************************************************************
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ) SALDO_CONTI ")

            Stb_Globale.AppendLine(" GROUP BY CodiceSplitGruppo, Cod_Conto_Eco, Conto_Eco_Descr, Id_Riclassificazione, Dare_Avere, Flag_UE  ")

            If Not Flag_ContiSaldo0 Then
                Stb_Globale.AppendLine(" HAVING SUM(Dare) - SUM(Avere) <> 0 ")
            End If

            If Flag_LayoutCostiRicavi Then
                Stb_Globale.AppendLine(" ORDER BY Dare_Avere DESC, CodiceSplitGruppo ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            strOutput = Stb_Globale.ToString

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    'calcola il saldo, da Data_Inizio_Saldo alla data si saldo, di ogni conto
    Public Function Saldo_Conti_Patrimoniali(ByVal Data_Inizio_Saldo As Date,
                                             ByVal Data_Saldo As Date,
                                             ByVal Flag_SOLOSaldiIniziali As Boolean,
                                             ByVal GestCont_Flag_ConsideraSaldiIniziali As Boolean,
                                             ByVal GestCont_DataInizio As Date,
                                             ByVal Piva As String,
                                             ByVal Anno As Integer,
                                             ByVal Ric_Cod As Integer,
                                             ByVal Id_Riclassificazione As String,
                                             ByVal Dare_Avere As String,
                                             ByVal Imputabile As Integer,
                                             ByVal Cod_Conto As Integer,
                                             ByVal Flag_UE As Integer,
                                             ByVal Cod_Contatto As String,
                                             ByVal Sezionale_Cod As Integer,
                                             ByVal Sezionale_ChkDefault As Integer,
                                             ByVal Cod_RisUm As Integer,
                                             ByVal Lista_CodRisUm As String,
                                             ByVal Cod_Liquidita As Integer,
                                             ByVal Flag_ContiSaldo0 As Boolean,
                                             ByVal Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean,
                                             ByVal xFiltroAggiuntivo1 As String,
                                             ByVal xFiltroAggiuntivo2 As String,
                                             ByVal xFiltroAggiuntivo3 As String,
                                             ByVal xFiltroAggiuntivo4 As String,
                                             ByVal DT_Codifiche As DataTable,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByRef strOutput As String = "",
                                             Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                             ) As DataTable


        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Saldo_Conti_Patrimoniali()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_QueryContiXMov As New StringBuilder
        Dim dt As DataTable

        Try

            Dim Flag_LeggiSaldiIniziali As Boolean = True

            'se sull'impresa ho detto di non considerare i saldi
            If Not GestCont_Flag_ConsideraSaldiIniziali Then
                Flag_LeggiSaldiIniziali = False
            Else
                If Sezionale_Cod = SEZIONALE_NOFILTRO Then
                    'non c'è filtro sul sezionale, ok leggo i saldi
                Else
                    'altrimenti sto facendo un filtro sui sezionali
                    'e i saldi iniziali vanno stampati solo sul sezionale principale
                    If Sezionale_ChkDefault <> 1 Then
                        'se il sezionale non è quello di default
                        Flag_LeggiSaldiIniziali = False
                    End If
                End If
            End If

            '20/03/2017: non bisogna leggere dall'01/01/1900,
            'ma dalla data di inizio gestione contabile
            Data_Inizio_Saldo = GestCont_DataInizio

            Stb_Globale.Length = 0
            Stb_QueryContiXMov.Length = 0

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiXMov = Query_Conti_Patrimoniali_Movimentati(Flag_LeggiSaldiIniziali,
                                                                      Flag_SOLOSaldiIniziali,
                                                                      GestCont_DataInizio,
                                                                      Data_Inizio_Saldo,
                                                                      Data_Saldo,
                                                                      Piva,
                                                                      Anno,
                                                                      Ric_Cod,
                                                                      Id_Riclassificazione,
                                                                      Dare_Avere,
                                                                      Imputabile,
                                                                      Cod_Conto,
                                                                      Flag_UE,
                                                                      Cod_Contatto,
                                                                      Sezionale_Cod,
                                                                      Cod_RisUm,
                                                                      Lista_CodRisUm,
                                                                      Cod_Liquidita,
                                                                      Flag_ContiSaldo0,
                                                                      Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                                      xFiltroAggiuntivo1,
                                                                      xFiltroAggiuntivo2,
                                                                      xFiltroAggiuntivo3,
                                                                      xFiltroAggiuntivo4,
                                                                      DT_Codifiche,
                                                                      objParametri,
                                                                      flagNuoviArrotondamenti:=flagNuoviArrotondamenti)



            '///////////////////////////////////////////////
            Stb_Globale.AppendLine(" SELECT CodiceSplitGruppo, Cod_Conto_Pat, Conto_Pat_Descr, Id_Riclassificazione, SUM(Dare) AS Saldo_Dare, SUM(Avere) AS Saldo_Avere, SUM(Dare) - SUM(Avere) AS Saldo, Dare_Avere, Flag_UE ")
            Stb_Globale.AppendLine(" FROM ")

            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_QueryContiXMov.ToString)


            '/************************************************************************************
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ) SALDO_CONTI ")

            Stb_Globale.AppendLine(" GROUP BY CodiceSplitGruppo, Cod_Conto_Pat, Conto_Pat_Descr, Id_Riclassificazione, Dare_Avere, Flag_UE  ")

            If Not Flag_ContiSaldo0 Then
                Stb_Globale.AppendLine(" HAVING SUM(Dare) - SUM(Avere) <> 0 ")
            End If

            'dare_avere desc perché prima le attività (D), poi le passività (A)
            Stb_Globale.AppendLine(" ORDER BY Dare_Avere DESC, Id_Riclassificazione, Conto_Pat_Descr  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            strOutput = Stb_Globale.ToString

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    'calcola il saldo, da Data_Inizio_Saldo alla data si saldo, di ogni conto
    Public Function Saldo_Conti_Patrimoniali_x_Bilancio(ByVal Data_Inizio_Saldo As Date,
                                                        ByVal Data_Saldo As Date,
                                                        ByVal Flag_SOLOSaldiIniziali As Boolean,
                                                        ByVal GestCont_Flag_ConsideraSaldiIniziali As Boolean,
                                                        ByVal GestCont_DataInizio As Date,
                                                        ByVal Piva As String,
                                                        ByVal Anno As Integer,
                                                        ByVal Ric_Cod As Integer,
                                                        ByVal Id_Riclassificazione As String,
                                                        ByVal Dare_Avere As String,
                                                        ByVal Imputabile As Integer,
                                                        ByVal Cod_Conto As Integer,
                                                        ByVal Flag_UE As Integer,
                                                        ByVal Cod_Contatto As String,
                                                        ByVal Sezionale_Cod As Integer,
                                                        ByVal Sezionale_ChkDefault As Integer,
                                                        ByVal Cod_RisUm As Integer,
                                                        ByVal Lista_CodRisUm As String,
                                                        ByVal Cod_Liquidita As Integer,
                                                        ByVal Flag_ContiSaldo0 As Boolean,
                                                        ByVal Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean,
                                                        ByVal xFiltroAggiuntivo1 As String,
                                                        ByVal xFiltroAggiuntivo2 As String,
                                                        ByVal xFiltroAggiuntivo3 As String,
                                                        ByVal xFiltroAggiuntivo4 As String,
                                                        ByVal DT_Codifiche As DataTable,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        Optional ByRef strOutput As String = "",
                                                        Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Saldo_Conti_Patrimoniali_x_Bilancio()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_QueryContiXMov As New StringBuilder
        Dim dt As DataTable

        Try

            Dim Flag_LeggiSaldiIniziali As Boolean = True

            'se sull'impresa ho detto di non considerare i saldi
            If Not GestCont_Flag_ConsideraSaldiIniziali Then
                Flag_LeggiSaldiIniziali = False
            Else
                If Sezionale_Cod = SEZIONALE_NOFILTRO Then
                    'non c'è filtro sul sezionale, ok leggo i saldi
                Else
                    'altrimenti sto facendo un filtro sui sezionali
                    'e i saldi iniziali vanno stampati solo sul sezionale principale
                    If Sezionale_ChkDefault <> 1 Then
                        'se il sezionale non è quello di default
                        Flag_LeggiSaldiIniziali = False
                    End If
                End If
            End If


            Stb_Globale.Length = 0
            Stb_QueryContiXMov.Length = 0

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiXMov = Query_Conti_Patrimoniali_Movimentati(Flag_LeggiSaldiIniziali,
                                                                      Flag_SOLOSaldiIniziali,
                                                                      GestCont_DataInizio,
                                                                      Data_Inizio_Saldo,
                                                                      Data_Saldo,
                                                                      Piva,
                                                                      Anno,
                                                                      Ric_Cod,
                                                                      Id_Riclassificazione,
                                                                      Dare_Avere,
                                                                      Imputabile,
                                                                      Cod_Conto,
                                                                      Flag_UE,
                                                                      Cod_Contatto,
                                                                      Sezionale_Cod,
                                                                      Cod_RisUm,
                                                                      Lista_CodRisUm,
                                                                      Cod_Liquidita,
                                                                      Flag_ContiSaldo0,
                                                                      Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                                      xFiltroAggiuntivo1,
                                                                      xFiltroAggiuntivo2,
                                                                      xFiltroAggiuntivo3,
                                                                      xFiltroAggiuntivo4,
                                                                      DT_Codifiche,
                                                                      objParametri,
                                                                      flagBilancio:=True,
                                                                      flagNuoviArrotondamenti:=flagNuoviArrotondamenti)


            '///////////////////////////////////////////////
            Stb_Globale.AppendLine(" SELECT CodiceSplitGruppo, Cod_Conto_Pat, Conto_Pat_Descr, Id_Riclassificazione, SUM(Dare) AS Saldo_Dare, SUM(Avere) AS Saldo_Avere, SUM(Dare) - SUM(Avere) AS Saldo, Dare_Avere, Flag_UE ")
            Stb_Globale.AppendLine(" FROM ")

            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_QueryContiXMov.ToString)


            '/************************************************************************************
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ) SALDO_CONTI ")

            Stb_Globale.AppendLine(" GROUP BY CodiceSplitGruppo, Cod_Conto_Pat, Conto_Pat_Descr, Id_Riclassificazione, Dare_Avere, Flag_UE  ")

            If Not Flag_ContiSaldo0 Then
                Stb_Globale.AppendLine(" HAVING SUM(Dare) - SUM(Avere) <> 0 ")
            End If

            'dare_avere desc perché prima le attività (D), poi le passività (A)
            Stb_Globale.AppendLine(" ORDER BY Dare_Avere DESC, Id_Riclassificazione, Conto_Pat_Descr  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            strOutput = Stb_Globale.ToString

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    'elenco dei movimenti dei conti patrimoniali nell'intervallo temporale
    Public Function MastrinoConti_Patrimoniali(ByVal Data_Inizio As Date,
                                               ByVal Data_Fine As Date,
                                               ByVal Piva As String,
                                               ByVal Anno As Integer,
                                               ByVal Ric_Cod As Integer,
                                               ByVal Id_Riclassificazione As String,
                                               ByVal Dare_Avere As String,
                                               ByVal Imputabile As Integer,
                                               ByVal Cod_Conto As Integer,
                                               ByVal Flag_UE As Integer,
                                               ByVal Cod_Contatto As String,
                                               ByVal Sezionale_Cod As Integer,
                                               ByVal Cod_RisUm As Integer,
                                               ByVal Cod_Liquidita As Integer,
                                               ByVal xFiltroAggiuntivo1 As String,
                                               ByVal xFiltroAggiuntivo2 As String,
                                               ByVal xFiltroAggiuntivo3 As String,
                                               ByVal xFiltroAggiuntivo4 As String,
                                               ByVal xOrderBy As String,
                                               ByVal DT_Codifiche As DataTable,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                               ) As DataTable


        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.MastrinoConti_Patrimoniali()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_QueryContiXMov As New StringBuilder
        Dim dt As DataTable

        Try
            Stb_Globale.Length = 0
            Stb_QueryContiXMov.Length = 0

            Dim Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean = False

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiXMov = Query_Conti_Patrimoniali_Movimentati(False,
                                                                      False,
                                                                      AGRODATAINIZIO,
                                                                      Data_Inizio,
                                                                      Data_Fine,
                                                                      Piva,
                                                                      Anno,
                                                                      Ric_Cod,
                                                                      Id_Riclassificazione,
                                                                      Dare_Avere,
                                                                      Imputabile,
                                                                      Cod_Conto,
                                                                      Flag_UE,
                                                                      Cod_Contatto,
                                                                      Sezionale_Cod,
                                                                      Cod_RisUm,
                                                                      "",
                                                                      Cod_Liquidita,
                                                                      False,
                                                                      Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                                      xFiltroAggiuntivo1,
                                                                      xFiltroAggiuntivo2,
                                                                      xFiltroAggiuntivo3,
                                                                      xFiltroAggiuntivo4,
                                                                      DT_Codifiche,
                                                                      objParametri,
                                                                      flagNuoviArrotondamenti:=flagNuoviArrotondamenti)

            If Stb_QueryContiXMov.ToString <> "" Then



                '///////////////////////////////////////////////
                Stb_Globale.AppendLine(" SELECT * ")

                'select CodiceSplitGruppo,Anno,Data_Order,dare_avere,Conto_Pat_Descr,Id_Riclassificazione,Des_Lib,Data_Movimento,Data_Registrazione,Progr_Protocollo,
                'sum(Dare) as Dare,
                'sum(Avere) as Avere

                Stb_Globale.AppendLine(" FROM ")

                Stb_Globale.AppendLine(" ( ")

                Stb_Globale.AppendLine(Stb_QueryContiXMov.ToString)


                '/************************************************************************************
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ) MASTRINO ")


                'GROUP BY CodiceSplitGruppo,Anno,Data_Order,dare_avere,Conto_Pat_Descr,Id_Riclassificazione,Des_Lib,Data_Movimento,Data_Registrazione,Progr_Protocollo

                Stb_Globale.AppendLine(" ORDER BY CodiceSplitGruppo, Anno, Data_Order, Progr_Protocollo, Progr_Registrazione  ")

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

            Else
                dt = Nothing
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti patrimoniali
    'default:
    'Cod_Liquidita = CODLIQUIDITA_NOFILTRO
    'Imputabile = SP_CONTO_IMPUTABILE_NOFILTRO
    'Flag_UE = CONTO_UE_NOFILTRO
    'Sezionale_Cod = SEZIONALE_NOFILTRO
    Private Function Query_Conti_Patrimoniali_Movimentati(ByVal Flag_LeggiSaldiIniziali As Boolean,
                                                          ByVal Flag_SOLOSaldiIniziali As Boolean,
                                                          ByVal GestCont_DataInizio As Date,
                                                          ByVal Data_Inizio As Date,
                                                          ByVal Data_Fine As Date,
                                                          ByVal Piva As String,
                                                          ByVal Anno As Integer,
                                                          ByVal Ric_Cod As Integer,
                                                          ByVal Id_Riclassificazione As String,
                                                          ByVal Dare_Avere As String,
                                                          ByVal Imputabile As Integer,
                                                          ByVal Cod_Conto As Integer,
                                                          ByVal Flag_UE As Integer,
                                                          ByVal Cod_Contatto As String,
                                                          ByVal Sezionale_Cod As Integer,
                                                          ByVal Cod_RisUm As Integer,
                                                          ByVal Lista_CodRisum As String,
                                                          ByVal Cod_Liquidita As Integer,
                                                          ByVal Flag_ContiSaldo0 As Boolean,
                                                          ByVal Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean,
                                                          ByVal xFiltroAggiuntivo1 As String,
                                                          ByVal xFiltroAggiuntivo2 As String,
                                                          ByVal xFiltroAggiuntivo3 As String,
                                                          ByVal xFiltroAggiuntivo4 As String,
                                                          ByVal DT_Codifiche As DataTable,
                                                          ByRef objParametri As AgronicaCoreParametri,
                                                          Optional ByVal flagBilancio As Boolean = False,
                                                          Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                                          ) As StringBuilder


        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Query_Conti_Patrimoniali_Movimentati()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_SelectSaldiIniz1 As New StringBuilder
        Dim Stb_SelectSaldiIniz1RC As New StringBuilder
        Dim Stb_SelectSaldiIniz2RC As New StringBuilder
        Dim Stb_SelectSaldiIniz2a As New StringBuilder
        'Dim Stb_SelectSaldiIniz2a_ContoPadre As New StringBuilder
        'Dim Stb_SelectSaldiIniz2a_ContoFiglio As New StringBuilder
        Dim Stb_SelectSaldiIniz2b As New StringBuilder
        Dim Stb_SelectSaldiIniz3 As New StringBuilder
        Dim Stb_SaldiInizialiRC As New StringBuilder
        Dim Stb_SaldiInizialiLIQ As New StringBuilder
        Dim Stb_JoinSaldiIniz As New StringBuilder
        Dim Stb_SelectConti1 As New StringBuilder
        Dim Stb_SelectConti2 As New StringBuilder
        Dim Stb_SelectConti3 As New StringBuilder
        Dim Stb_JoinConti1 As New StringBuilder
        Dim Stb_JoinConti2 As New StringBuilder
        Dim Stb_JoinConti3 As New StringBuilder
        Dim Stb_WhereConti As New StringBuilder
        Dim Stb_WhereContiSezionaliContab As New StringBuilder
        Dim Stb_WhereContiSezionaliMag As New StringBuilder
        Dim Stb_SelectContab As New StringBuilder
        Dim Stb_SelectNoContab As New StringBuilder
        Dim Stb_JoinContab As New StringBuilder
        'Dim Stb_SelectIVAvendite As New StringBuilder
        'Dim Stb_SelectIVAacquisti As New StringBuilder
        Dim Stb_SelectIVA As New StringBuilder
        Dim Stb_SelectOmaggiDareAvere As New StringBuilder
        Dim Stb_JoinIVA As New StringBuilder
        Dim Stb_JoinIVAcredito As New StringBuilder
        Dim Stb_JoinIVAdebito As New StringBuilder

        Dim Stb_SelectPagamRiscossi1a As New StringBuilder
        Dim Stb_SelectPagam1a As New StringBuilder
        Dim Stb_SelectPagamRiscossi2 As New StringBuilder
        Dim Stb_SelectPartDoppia1a As New StringBuilder
        Dim Stb_SelectPartDoppia1b As New StringBuilder
        Dim Stb_SelectPartDoppiaNoRif As New StringBuilder
        Dim Stb_SelectPartDoppiaRifContatto As New StringBuilder

        'leggi commento al momento della definizione del select
        'Dim Stb_SelectPartDoppiaRifBanche As New StringBuilder
        Dim Stb_SelectPartDoppiaRifBanche_Avere As New StringBuilder
        Dim Stb_SelectPartDoppiaRifBanche_Dare As New StringBuilder

        Dim Stb_SelectPartDoppiaDARE As New StringBuilder
        Dim Stb_SelectPartDoppiaAVERE As New StringBuilder
        Dim Stb_SelectPartDoppia2 As New StringBuilder
        Dim Stb_SelectPD2_CreDeb As New StringBuilder
        Dim Stb_SaldiIniziRisUmCred As New StringBuilder
        Dim Stb_SaldiIniziRisUmDeb As New StringBuilder

        Dim Stb_joinPartDoppia1 As New StringBuilder
        Dim Stb_joinPartDoppiaDARE As New StringBuilder
        Dim Stb_joinPartDoppiaAVERE As New StringBuilder
        Dim Stb_joinPartDoppia2 As New StringBuilder

        Dim Stb_GroupByPartDoppia As New StringBuilder
        Dim Stb_GroupByPartDoppiaRifContattoDare As New StringBuilder
        Dim Stb_GroupByPartDoppiaRifContattoAvere As New StringBuilder
        Dim Stb_GroupByPartDoppiaRifBancheDare As New StringBuilder
        Dim Stb_GroupByPartDoppiaRifBancheAvere As New StringBuilder

        Dim FlagServeUnion As Boolean = False
        ' Dim DT As DataTable
        ' Dim xFiltroAggiuntivo_xDataRegistrazione, xFiltroAggiuntivo_xDataMovimento As String

        Dim Id_Riclassificazione_CreditiVersoClienti As String = ""
        Dim Id_Riclassificazione_DebitiVersoFornitori As String = ""
        Dim Id_Riclassificazione_DepositiBancariPostali As String = ""
        Dim Id_Riclassificazione_DenaroValoriInCassa As String = ""
        Dim Id_Riclassificazione_IvaACredito As String = ""
        Dim Id_Riclassificazione_IvaACredito_AcqIntra As String = ""
        Dim Id_Riclassificazione_IvaADebito As String = ""
        Dim Id_Riclassificazione_IvaADebito_AcqIntra As String = ""
        Dim Id_Riclassificazione_ErarioRitenuteLavoroAutonomo As String = ""
        Dim Id_Riclassificazione_DebitiVsEnasarco As String = ""

        Dim Conto_Pat_Descr_CreditiVersoClienti As String = ""
        Dim Conto_Pat_Descr_DebitiVersoFornitori As String = ""
        Dim Conto_Pat_Descr_DepositiBancariPostali As String = ""
        Dim Conto_Pat_Descr_DenaroValoriInCassa As String = ""
        Dim Conto_Pat_Descr_IvaACredito As String = ""
        Dim Conto_Pat_Descr_IvaACredito_AcqIntra As String = ""
        Dim Conto_Pat_Descr_IvaADebito As String = ""
        Dim Conto_Pat_Descr_IvaADebito_AcqIntra As String = ""
        Dim Conto_Pat_Descr_ErarioRitenuteLavoroAutonomo As String = ""
        Dim Conto_Pat_Descr_DebitiVsEnasarco As String = ""

        Dim Cod_Conto_Pat_CreditiVersoClienti As Integer = 0
        Dim Cod_Conto_Pat_DebitiVersoFornitori As Integer = 0
        Dim Cod_Conto_Pat_DepositiBancariPostali As Integer = 0
        Dim Cod_Conto_Pat_DenaroValoriInCassa As Integer = 0
        Dim Cod_Conto_Pat_IvaACredito As Integer = 0
        Dim Cod_Conto_Pat_IvaACredito_AcqIntra As Integer = 0
        Dim Cod_Conto_Pat_IvaADebito As Integer = 0
        Dim Cod_Conto_Pat_IvaADebito_AcqIntra As Integer = 0
        Dim Cod_Conto_Pat_ErarioRitenuteLavoroAutonomo As Integer = 0
        Dim Cod_Conto_Pat_DebitiVsEnasarco As Integer = 0


        Dim Anno_GestCont_DataInizio As Integer = GestCont_DataInizio.Year
        Dim Anno_Fine As Integer = Data_Fine.Year

        Try

            'gestione dei conti patrimoniali "automatizzati" (con riferimenti a contatti, liquidita, calcolo automatico iva, ecc...)
            Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita

            objContabHLP.Recupera_Cod_Des_ContiPatrimoniali(DT_Codifiche,
                                                            Id_Riclassificazione_CreditiVersoClienti,
                                                            Id_Riclassificazione_DebitiVersoFornitori,
                                                            Id_Riclassificazione_DepositiBancariPostali,
                                                            Id_Riclassificazione_DenaroValoriInCassa,
                                                            Id_Riclassificazione_IvaACredito,
                                                            Id_Riclassificazione_IvaACredito_AcqIntra,
                                                            Id_Riclassificazione_IvaADebito,
                                                            Id_Riclassificazione_IvaADebito_AcqIntra,
                                                            Conto_Pat_Descr_CreditiVersoClienti,
                                                            Conto_Pat_Descr_DebitiVersoFornitori,
                                                            Conto_Pat_Descr_DepositiBancariPostali,
                                                            Conto_Pat_Descr_DenaroValoriInCassa,
                                                            Conto_Pat_Descr_IvaACredito,
                                                            Conto_Pat_Descr_IvaACredito_AcqIntra,
                                                            Conto_Pat_Descr_IvaADebito,
                                                            Conto_Pat_Descr_IvaADebito_AcqIntra,
                                                            Cod_Conto_Pat_CreditiVersoClienti,
                                                            Cod_Conto_Pat_DebitiVersoFornitori,
                                                            Cod_Conto_Pat_DepositiBancariPostali,
                                                            Cod_Conto_Pat_DenaroValoriInCassa,
                                                            Cod_Conto_Pat_IvaACredito,
                                                            Cod_Conto_Pat_IvaACredito_AcqIntra,
                                                            Cod_Conto_Pat_IvaADebito,
                                                            Cod_Conto_Pat_IvaADebito_AcqIntra)


        Catch ex As Exception

        End Try

        Try

            Stb_Globale.Length = 0
            Stb_SelectSaldiIniz1.Length = 0
            Stb_SelectSaldiIniz1RC.Length = 0
            Stb_SelectSaldiIniz2RC.Length = 0
            'Stb_SelectSaldiIniz2a_ContoPadre.Length = 0
            'Stb_SelectSaldiIniz2a_ContoFiglio.Length = 0
            Stb_SelectSaldiIniz2a.Length = 0
            Stb_SelectSaldiIniz2b.Length = 0
            Stb_SelectSaldiIniz3.Length = 0
            Stb_JoinSaldiIniz.Length = 0
            Stb_SelectConti1.Length = 0
            Stb_SelectConti2.Length = 0
            Stb_SelectConti3.Length = 0
            Stb_JoinConti1.Length = 0
            Stb_JoinConti2.Length = 0
            Stb_JoinConti3.Length = 0
            Stb_WhereConti.Length = 0
            Stb_WhereContiSezionaliContab.Length = 0
            Stb_WhereContiSezionaliMag.Length = 0
            Stb_SelectContab.Length = 0
            Stb_SelectNoContab.Length = 0
            Stb_JoinContab.Length = 0
            ' Stb_JoinPagamenti = 0
            'Stb_SelectIVAvendite.Length = 0
            'Stb_SelectIVAacquisti.Length = 0
            Stb_SelectIVA.Length = 0
            Stb_JoinIVA.Length = 0
            Stb_JoinIVAcredito.Length = 0
            Stb_JoinIVAdebito.Length = 0

            Stb_SelectPartDoppia1a.Length = 0
            Stb_SelectPartDoppia1b.Length = 0
            Stb_SelectPartDoppiaNoRif.Length = 0
            Stb_SelectPartDoppiaRifContatto.Length = 0
            Stb_SelectPartDoppiaRifBanche_Dare.Length = 0
            Stb_SelectPartDoppiaRifBanche_Avere.Length = 0
            Stb_SelectPartDoppiaAVERE.Length = 0
            Stb_SelectPartDoppiaDARE.Length = 0
            Stb_SelectPartDoppia2.Length = 0
            Stb_SelectPD2_CreDeb.Length = 0

            Stb_joinPartDoppia1.Length = 0
            Stb_joinPartDoppiaAVERE.Length = 0
            Stb_joinPartDoppiaDARE.Length = 0
            Stb_joinPartDoppia2.Length = 0

            Stb_GroupByPartDoppia.Length = 0
            Stb_GroupByPartDoppiaRifContattoDare.Length = 0
            Stb_GroupByPartDoppiaRifBancheDare.Length = 0
            Stb_GroupByPartDoppiaRifContattoAvere.Length = 0
            Stb_GroupByPartDoppiaRifBancheDare.Length = 0
            Stb_GroupByPartDoppiaRifBancheAvere.Length = 0


            'poiché il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))


            '/************************************************************************
            '/*************** query conti  *******************
            Stb_SelectConti1.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib,   ")
            Stb_SelectConti1.AppendLine(" Conti_Pat.Cod_Conto_Pat, Conti_Pat.Piva AS piva_conti,   ")
            ''Stb_SelectConti.AppendLine(" Conti_Pat.Conto_Pat_Descr, ")

            'Stb_SelectConti2.AppendLine(" Conti_Pat.Flag_UE, '' AS Cod_Contatto,  ")
            Stb_SelectConti2.AppendLine(" '' AS Cod_Contatto,  ")
            Stb_SelectConti2.AppendLine(" RicXConti_Pat.Piva, Imprese_Ric.rag_soc AS rag_soc,  ")
            Stb_SelectConti2.AppendLine(" RicXConti_Pat.Anno, RicXConti_Pat.Id_Riclassificazione, RicXConti_Pat.Dare_Avere, RicXConti_Pat.Saldo,  ") '--RicXConti_Pat.Imputabile,
            Stb_SelectConti2.AppendLine(" Riclassificazioni_Pat.Ric_Cod_Pat, Riclassificazioni_Pat.Ric_Des_Pat,  ")
            If flagNuoviArrotondamenti Then
                Stb_SelectConti2.AppendLine(" ROUND(ISNULL(Movimenti_dettagli.Imponibile_Netto, 0), 2) AS Imponibile_Netto,  ")
            Else
                Stb_SelectConti2.AppendLine(" ISNULL(Movimenti_dettagli.Imponibile_Netto, 0) AS Imponibile_Netto,  ")
            End If
            Stb_SelectConti2.AppendLine(" Movimenti_dettagli.elem_cod, Movimenti_dettagli.pro_cod, Movimenti_dettagli.mat_cod, ")

            '////////  DARE - AVERE - GESTIONE PATRIMONIALE /////////////////////////////
            Stb_SelectConti3.AppendLine(" CASE WHEN Movimenti_dettagli.Imponibile_Netto >= 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectConti3.AppendLine(" THEN ( -1 * ROUND(Movimenti_dettagli.Iva, 2)) + ROUND(Movimenti_dettagli.Imponibile_Netto, 2) ")
            Else
                Stb_SelectConti3.AppendLine(" THEN ( -1 * Movimenti_dettagli.Iva) + Movimenti_dettagli.Imponibile_Netto ")
            End If
            Stb_SelectConti3.AppendLine(" ELSE 0 ")
            Stb_SelectConti3.AppendLine(" END AS Dare,    ")

            'Stb_SelectConti3.AppendLine(" CASE WHEN Movimenti_dettagli.Imponibile_Netto >= 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc = 0 ")
            'If flagNuoviArrotondamenti = True Then
            '    Stb_SelectConti3.AppendLine(" THEN ( -1 * ROUND(Movimenti_dettagli.Iva, 2)) + ROUND(Movimenti_dettagli.Imponibile_Netto, 2) ")
            'Else
            '    Stb_SelectConti3.AppendLine(" THEN ( -1 * Movimenti_dettagli.Iva) + Movimenti_dettagli.Imponibile_Netto ")
            'End If
            'Stb_SelectConti3.AppendLine("       WHEN Movimenti_dettagli.Imponibile_Netto >= 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc <> 0 ")
            'If flagNuoviArrotondamenti = True Then
            '    Stb_SelectConti3.AppendLine(" THEN ( -1 * ROUND(Movimenti_dettagli.Iva, 2)) + ROUND(Movimenti_dettagli.Imponibile_Netto, 2) + ROUND(Movimenti_dettagli.Iva_Indetraibile, 2) ")
            'Else
            '    Stb_SelectConti3.AppendLine(" THEN ( -1 * Movimenti_dettagli.Iva) + Movimenti_dettagli.Imponibile_Netto + Movimenti_dettagli.Iva_Indetraibile ")
            'End If
            'Stb_SelectConti3.AppendLine(" ELSE 0 ")
            'Stb_SelectConti3.AppendLine(" END AS Dare,    ")

            Stb_SelectConti3.AppendLine(" CASE WHEN Movimenti_dettagli.Imponibile_Netto < 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectConti3.AppendLine(" THEN (-1 * ROUND(Movimenti_dettagli.Imponibile_Netto, 2)) + ROUND(Movimenti_dettagli.Iva, 2) ")
            Else
                Stb_SelectConti3.AppendLine(" THEN (-1 * Movimenti_dettagli.Imponibile_Netto) + Movimenti_dettagli.Iva ")
            End If
            Stb_SelectConti3.AppendLine(" ELSE 0 ")
            Stb_SelectConti3.AppendLine(" END AS Avere,    ")

            'Stb_SelectConti3.AppendLine(" CASE WHEN Movimenti_dettagli.Imponibile_Netto < 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc = 0 ")
            'If flagNuoviArrotondamenti = True Then
            '    Stb_SelectConti3.AppendLine(" THEN (-1 * ROUND(Movimenti_dettagli.Imponibile_Netto, 2)) + ROUND(Movimenti_dettagli.Iva, 2) ")
            'Else
            '    Stb_SelectConti3.AppendLine(" THEN (-1 * Movimenti_dettagli.Imponibile_Netto) + Movimenti_dettagli.Iva ")
            'End If
            'Stb_SelectConti3.AppendLine("       WHEN Movimenti_dettagli.Imponibile_Netto < 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc <> 0 ")
            'If flagNuoviArrotondamenti = True Then
            '    Stb_SelectConti3.AppendLine(" THEN (-1 * ROUND(Movimenti_dettagli.Imponibile_Netto, 2)) + ROUND(Movimenti_dettagli.Iva, 2) + (-1 * ROUND(Movimenti_dettagli.Iva_Indetraibile, 2)) ")
            'Else
            '    Stb_SelectConti3.AppendLine(" THEN (-1 * Movimenti_dettagli.Imponibile_Netto) + Movimenti_dettagli.Iva + (-1 * Movimenti_dettagli.Iva_Indetraibile) ")
            'End If
            'Stb_SelectConti3.AppendLine(" ELSE 0 ")
            'Stb_SelectConti3.AppendLine(" END AS Avere,    ")

            '////////  DARE - AVERE - GESTIONE OMAGGI /////////////////////////////
            Stb_SelectOmaggiDareAvere.AppendLine(" CASE WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_ConRivalsaIva & " AND Movimenti_dettagli.Iva < 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN -1 * ROUND(Movimenti_dettagli.Iva, 2) ")
            Else
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN -1 * Movimenti_dettagli.Iva ")
            End If
            Stb_SelectOmaggiDareAvere.AppendLine("      WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_SenzaRivalsaIva & " AND Movimenti_dettagli.Iva < 0 ")
            Stb_SelectOmaggiDareAvere.AppendLine(" THEN 0  ")
            Stb_SelectOmaggiDareAvere.AppendLine("      ELSE 0 ")
            Stb_SelectOmaggiDareAvere.AppendLine(" END AS Dare,    ")
            Stb_SelectOmaggiDareAvere.AppendLine(" CASE WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_ConRivalsaIva & " AND Movimenti_dettagli.Iva >= 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN ROUND(Movimenti_dettagli.Iva, 2) ")
            Else
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN Movimenti_dettagli.Iva ")
            End If
            Stb_SelectOmaggiDareAvere.AppendLine("      WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_SenzaRivalsaIva & " AND Movimenti_dettagli.Iva >= 0 ")
            Stb_SelectOmaggiDareAvere.AppendLine(" THEN 0 ")
            Stb_SelectOmaggiDareAvere.AppendLine("      ELSE 0 ")
            Stb_SelectOmaggiDareAvere.AppendLine(" END AS Avere,    ")

            '////////  DARE - AVERE - GESTIONE IVA /////////////////////////////

            'modifica del 23/07/2015: il campo iva ora contiene l'iva totale, quindi devo togliere l'iva in compensazione (vendite)
            'o l'iva indetraibile (acquisti)

            'CASO DARE -> VENDITE: iva_indetraibile è negativa, quindi la sommo
            Stb_SelectIVA.AppendLine(" CASE WHEN Movimenti_dettagli.Iva > 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc = 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVA.AppendLine("          THEN ROUND(Movimenti_dettagli.Iva, 2) ")
            Else
                Stb_SelectIVA.AppendLine("          THEN Movimenti_dettagli.Iva ")
            End If
            Stb_SelectIVA.AppendLine("      WHEN Movimenti_dettagli.Iva > 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc <> 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVA.AppendLine("          THEN ROUND(Movimenti_dettagli.Iva, 2) + ROUND(Movimenti_dettagli.Iva_Indetraibile, 2) ")
            Else
                Stb_SelectIVA.AppendLine("          THEN Movimenti_dettagli.Iva + Movimenti_dettagli.Iva_Indetraibile ")
            End If
            Stb_SelectIVA.AppendLine(" ELSE 0 ")
            Stb_SelectIVA.AppendLine(" END AS Dare,    ")

            'CASO DARE -> VENDITE: iva_indetraibile è positiva, quindi la sottraggo
            Stb_SelectIVA.AppendLine(" CASE WHEN Movimenti_dettagli.Iva < 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc = 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVA.AppendLine("          THEN (-1 * ROUND(Movimenti_dettagli.Iva, 2))  ")
            Else
                Stb_SelectIVA.AppendLine("          THEN (-1 * Movimenti_dettagli.Iva)  ")
            End If
            Stb_SelectIVA.AppendLine("      WHEN Movimenti_dettagli.Iva < 0 AND Movimenti_dettagli.Iva_Indetraibile_Perc <> 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVA.AppendLine("          THEN (-1 * ROUND(Movimenti_dettagli.Iva, 2)) - ROUND(Movimenti_dettagli.Iva_Indetraibile, 2) ")
            Else
                Stb_SelectIVA.AppendLine("          THEN (-1 * Movimenti_dettagli.Iva) - Movimenti_dettagli.Iva_Indetraibile ")
            End If
            Stb_SelectIVA.AppendLine(" ELSE 0 ")
            Stb_SelectIVA.AppendLine(" END AS Avere,    ")

            'JOIN CONTI
            Stb_JoinConti1.AppendLine(" FROM Conti_Patrimonio Conti_Pat ")
            Stb_JoinConti1.AppendLine(" INNER JOIN RicxConti_Patrimonio RicXConti_Pat ON Conti_Pat.Cod_Conto_Pat = RicXConti_Pat.Cod_Conto_Pat ")
            Stb_JoinConti1.AppendLine(" INNER JOIN Riclassificazioni_Patrimonio Riclassificazioni_Pat ON Riclassificazioni_Pat.Piva_SuperUser = RicXConti_Pat.Piva_SuperUser AND Riclassificazioni_Pat.Piva = RicXConti_Pat.Piva AND Riclassificazioni_Pat.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat  ")
            Stb_JoinConti1.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Pat.Piva = Imprese_Ric.PIVA ")
            'JOIN MOVIMENTI DETTAGLI - RICXCONTI - CONTI 
            Stb_JoinConti2.AppendLine(" INNER JOIN  Movimenti_dettagli ON Movimenti_dettagli.PIVA = RicXConti_Pat.Piva AND Movimenti_dettagli.Anno = RicXConti_Pat.Anno AND Movimenti_dettagli.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat ")
            Stb_JoinConti2.AppendLine(" AND Movimenti_dettagli.Cod_Conto_Pat = RicXConti_Pat.Cod_Conto_Pat ")
            'JOIN MOVIMENTI DETTAGLI - RICXCONTI - CONTI - iva
            Stb_JoinIVA.AppendLine(" INNER JOIN  Movimenti_dettagli ON Movimenti_dettagli.PIVA = RicXConti_Pat.Piva AND Movimenti_dettagli.Anno = RicXConti_Pat.Anno AND Movimenti_dettagli.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat ")

            '§§§ gestione codifica piano dei conti
            'Stb_JoinIVAcredito.AppendLine(" AND RicXConti_Pat.Cod_Conto_Pat = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.IvaACredito) & "")
            Stb_JoinIVAcredito.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.IvaACredito) & "")
            'Stb_JoinIVAdebito.AppendLine(" AND RicXConti_Pat.Cod_Conto_Pat = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.IvaADebito) & "")
            Stb_JoinIVAdebito.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat = " & Agro_SQL_SaveNum(enum_Conti_Patrimoniali.IvaADebito) & "")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            Stb_JoinConti3.AppendLine(" INNER JOIN Movimenti Movimenti_Mag ")
            Stb_JoinConti3.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")
            'JOIN AGENDA - MOVIMENTI
            Stb_JoinConti3.AppendLine(" INNER JOIN Agenda ")
            Stb_JoinConti3.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")
            'JOIN AGENDA - OPERAZIONI
            Stb_JoinConti3.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'WHERE CONTI
            Stb_WhereConti.AppendLine(" WHERE RicXConti_Pat.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            Stb_WhereConti.AppendLine(" AND RicXConti_Pat.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")

            'non serve il filtro, tanto c'è il join tra Movimenti_Mag e movimenti_dettagli
            'Stb_WhereConti.AppendLine(" AND Movimenti_Mag.CAU_MOV IN ('" & CAU_CARICO & "', " &
            '                                                    "'" & CAU_SCARICO & "', " &
            '                                                    "'" & CAU_ABBUONI & "', " &
            '                                                    "'" & CAU_CONFERIMENTO & "', " &
            '                                                    "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
            '                                                    ") ")

            If Ric_Cod <> 0 Then
                Stb_WhereConti.AppendLine(" AND Riclassificazioni_Pat.Ric_Cod_Pat = " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
            End If
            If Anno <> 0 Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Pat.Anno = " & Agro_SQL_SaveNum(Anno) & "  ")
            End If
            If Id_Riclassificazione <> "" Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Pat.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'  ")
            End If
            If Dare_Avere <> "" Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Pat.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'  ")
            End If
            If Imputabile <> SP_CONTO_IMPUTABILE_NOFILTRO Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Pat.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & "  ")
            End If
            If Cod_Conto <> 0 Then
                Stb_WhereConti.AppendLine(" AND Conti_Pat.Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            End If
            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb_WhereConti.AppendLine(" AND Conti_Pat.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & "  ")
            End If
            'If Cod_Contatto <> "" Then
            '    Stb_WhereConti.AppendLine(" AND Conti_Pat.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'  ")
            'End If
            '/************************************************************************


            '  Giulia, 28/10/2016 11.04.30: Se un conto è stato mappato su di un solo sezionale deve tirare su solo i movimenti su quel sezionale
            '       per le operazioni che hanno un intestazione
            Stb_WhereContiSezionaliContab.AppendLine(vbCrLf & " -- Se il conto è stato mappato su di un sezionale preciso (CONTAB) ")
            Stb_WhereContiSezionaliContab.AppendLine(" AND ( ")
            Stb_WhereContiSezionaliContab.AppendLine("  CASE WHEN RicXConti_Pat.Sezionale_Cod_Conto_Pat <> -1 ")
            Stb_WhereContiSezionaliContab.AppendLine("  THEN RicXConti_Pat.Sezionale_Cod_Conto_Pat ")
            Stb_WhereContiSezionaliContab.AppendLine("  ELSE Movimenti_Contab.Sezionale_Cod ")
            Stb_WhereContiSezionaliContab.AppendLine("  END = Movimenti_Contab.Sezionale_Cod ")
            Stb_WhereContiSezionaliContab.AppendLine("  ) ")

            '/************************************************************************

            '  Giulia, 27/12/2016 16.32.55: Se un conto è stato mappato su di un solo sezionale deve tirare su solo i movimenti su quel sezionale
            '       per le operazioni che hanno direttamente il movimento di magazzino
            Stb_WhereContiSezionaliMag.AppendLine(vbCrLf & " -- Se il conto è stato mappato su di un sezionale preciso (MAG)")
            Stb_WhereContiSezionaliMag.AppendLine(" AND ( ")
            Stb_WhereContiSezionaliMag.AppendLine("  CASE WHEN RicXConti_Pat.Sezionale_Cod_Conto_Pat <> -1 ")
            Stb_WhereContiSezionaliMag.AppendLine("  THEN RicXConti_Pat.Sezionale_Cod_Conto_Pat ")
            Stb_WhereContiSezionaliMag.AppendLine("  ELSE Movimenti_Mag.Sezionale_Cod ")
            Stb_WhereContiSezionaliMag.AppendLine("  END = Movimenti_Mag.Sezionale_Cod ")
            Stb_WhereContiSezionaliMag.AppendLine("  ) ")

            '/************************************************************************
            '/*************** SELECT query movimenti NO contabili  *******************
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Movimenti_Mag.Doc_Numero)) + Movimenti_Mag.Doc_Numero_Des AS Numero_Doc,   ")
            Stb_SelectNoContab.AppendLine(" CONVERT(varchar, Movimenti_Mag.Data_Movimento, 103) AS Data_Movimento, ")
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Progr_Protocollo, Movimenti_Mag.Progr_Registrazione, CONVERT(date, Movimenti_Mag.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Cod_RisUm,  Movimenti_Mag.Mov_Desc, ")
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Num_Protocollo  ")
            '/************************************************************************

            '/************************************************************************
            '/*************** SELECT / JOIN query movimenti CONTABILI  *******************
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Movimenti_Contab.Doc_Numero)) + Movimenti_Contab.Doc_Numero_Des AS Numero_Doc,   ")
            Stb_SelectContab.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento, Movimenti_Contab.Progr_Protocollo, ")
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Progr_Registrazione,  ")
            Stb_SelectContab.AppendLine(" CONVERT(date, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Num_Protocollo  ")
            'JOIN AGENDA - MOVIMENTI CONTAB
            Stb_JoinContab.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            Stb_JoinContab.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")
            '/************************************************************************


            '/************************************************************************
            '/*************** query SALDI INIZIALI *******************
            Stb_SelectSaldiIniz1RC.AppendLine(" 0 AS Id_Agenda, 0 AS Lav_Cod, '' AS LAV_DES, 'Saldo Iniziale' AS des_lib,   ")
            Stb_SelectSaldiIniz1RC.AppendLine(" Conti_Pat.Cod_Conto_Pat, Conti_Pat.Piva AS piva_conti,   ")
            'Stb_SelectSaldiIniz1RC.AppendLine(" Conti_Pat.Conto_Pat_Descr,   ")

            Stb_SelectSaldiIniz1.AppendLine(" 0 AS Id_Agenda, 0 AS Lav_Cod, '' AS LAV_DES, 'Saldo Iniziale' AS des_lib,   ")

            'Stb_SelectSaldiIniz2RC.AppendLine(" Conti_Pat.Flag_UE, '' AS Cod_Contatto,  ")
            Stb_SelectSaldiIniz2RC.AppendLine(" '' AS Cod_Contatto,  ")
            Stb_SelectSaldiIniz2RC.AppendLine(" RicXConti_Pat.Piva, Imprese_Ric.rag_soc AS rag_soc,  ")
            Stb_SelectSaldiIniz2RC.AppendLine(" RicXConti_Pat.Anno, RicXConti_Pat.Id_Riclassificazione, RicXConti_Pat.Dare_Avere,   ") '--RicXConti_Pat.Imputabile,
            If Flag_LeggiSaldiIniziali = 0 Then
                Stb_SelectSaldiIniz2RC.AppendLine(" 0 as Saldo, ")
            Else
                Stb_SelectSaldiIniz2RC.AppendLine(" RicXConti_Pat.Saldo, ")
            End If
            Stb_SelectSaldiIniz2RC.AppendLine(" Riclassificazioni_Pat.Ric_Cod_Pat, Riclassificazioni_Pat.Ric_Des_Pat,  ")
            Stb_SelectSaldiIniz2RC.AppendLine(" 0 AS Imponibile_Netto,  ")
            Stb_SelectSaldiIniz2RC.AppendLine(" 0 AS elem_cod, 0 AS pro_cod, 0 AS mat_cod, ")

            Stb_SelectSaldiIniz2a.AppendLine("  '' AS Cod_Contatto,  ")
            Stb_SelectSaldiIniz2a.AppendLine(" '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS Piva, '' AS rag_soc, " & CStr(Anno) & " AS Anno,  ")


            'personalizzato
            '.AppendLine(" RicXConti_Pat.Id_Riclassificazione, RicXConti_Pat.Dare_Avere,  ") '--RicXConti_Pat.Imputabile,
            Stb_SelectSaldiIniz2b.AppendLine(" 0 AS Saldo,  " & CStr(Ric_Cod) & " AS Ric_Cod_Pat, 'Stato Patrimoniale Personalizzato' AS Ric_Des_Pat,  ")
            Stb_SelectSaldiIniz2b.AppendLine(" 0 AS Imponibile_Netto,  ")
            Stb_SelectSaldiIniz2b.AppendLine(" 0 AS elem_cod, 0 AS pro_cod, 0 AS mat_cod, ")

            '////////  DARE - AVERE - GESTIONE PATRIMONIALE ///////////////////////////// 
            If Flag_LeggiSaldiIniziali = 0 Then
                Stb_SaldiInizialiRC.AppendLine(" 0 AS Dare, 0 as Avere,  ")
            Else
                Stb_SaldiInizialiRC.AppendLine(" CASE WHEN RicXConti_Pat.Saldo_Iniziale > 0 THEN RicXConti_Pat.Saldo_Iniziale ELSE 0 ")
                Stb_SaldiInizialiRC.AppendLine(" END AS Dare,    ")
                Stb_SaldiInizialiRC.AppendLine(" CASE WHEN RicXConti_Pat.Saldo_Iniziale < 0 THEN (-1 * RicXConti_Pat.Saldo_Iniziale) ELSE 0 ")
                Stb_SaldiInizialiRC.AppendLine(" END AS Avere,    ")
            End If
            '////////  DARE - AVERE - GESTIONE PATRIMONIALE - RISORSE FINANZIARIE /////////////////////////////          
            If Flag_LeggiSaldiIniziali = 0 Then
                Stb_SaldiInizialiLIQ.AppendLine(" 0 AS Dare, 0 as Avere,  ")
            Else
                Stb_SaldiInizialiLIQ.AppendLine(" CASE WHEN Liquidita.Saldo_Iniziale > 0 THEN Liquidita.Saldo_Iniziale ELSE 0 ")
                Stb_SaldiInizialiLIQ.AppendLine(" END AS Dare,    ")
                Stb_SaldiInizialiLIQ.AppendLine(" CASE WHEN Liquidita.Saldo_Iniziale < 0 THEN (-1 * Liquidita.Saldo_Iniziale) ELSE 0 ")
                Stb_SaldiInizialiLIQ.AppendLine(" END AS Avere,    ")
            End If
            '////////  DARE - AVERE - GESTIONE PATRIMONIALE - CONTATTI CREDITI /////////////////////////////
            If Flag_LeggiSaldiIniziali = 0 Then
                Stb_SaldiIniziRisUmCred.AppendLine(" 0 AS Dare, 0 as Avere,  ")
            Else
                Stb_SaldiIniziRisUmCred.AppendLine(" CASE WHEN Risorse_Umane.Saldo_Iniziale_Crediti > 0 THEN Risorse_Umane.Saldo_Iniziale_Crediti ELSE 0 ")
                Stb_SaldiIniziRisUmCred.AppendLine(" END AS Dare,    ")
                Stb_SaldiIniziRisUmCred.AppendLine(" CASE WHEN Risorse_Umane.Saldo_Iniziale_Crediti < 0 THEN (-1 * Risorse_Umane.Saldo_Iniziale_Crediti) ELSE 0 ")
                Stb_SaldiIniziRisUmCred.AppendLine(" END AS Avere,    ")
            End If
            '////////  DARE - AVERE - GESTIONE PATRIMONIALE - CONTATTI DEBITI /////////////////////////////
            If Flag_LeggiSaldiIniziali = 0 Then
                Stb_SaldiIniziRisUmDeb.AppendLine(" 0 AS Dare, 0 as Avere,  ")
            Else
                Stb_SaldiIniziRisUmDeb.AppendLine(" CASE WHEN Risorse_Umane.Saldo_Iniziale_Debiti > 0 THEN Risorse_Umane.Saldo_Iniziale_Debiti ELSE 0 ")
                Stb_SaldiIniziRisUmDeb.AppendLine(" END AS Dare,    ")
                Stb_SaldiIniziRisUmDeb.AppendLine(" CASE WHEN Risorse_Umane.Saldo_Iniziale_Debiti < 0 THEN (-1 * Risorse_Umane.Saldo_Iniziale_Debiti) ELSE 0 ")
                Stb_SaldiIniziRisUmDeb.AppendLine(" END AS Avere,    ")
            End If

            Stb_SelectSaldiIniz3.AppendLine(" '' AS Numero_Doc, '01/01/1900' AS Data_Movimento, ")
            Stb_SelectSaldiIniz3.AppendLine(" 0 AS Progr_Protocollo, 0 AS Progr_Registrazione, CONVERT(date, '01/01/1900') AS Data_Registrazione, ")
            Stb_SelectSaldiIniz3.AppendLine(" 0 AS Cod_RisUm,  '' AS Mov_Desc, ")
            Stb_SelectSaldiIniz3.AppendLine(" 0 AS Num_Protocollo  ")
            Stb_SelectSaldiIniz3.AppendLine(" , '01/01/1900' AS Data_Order ")
            Stb_SelectSaldiIniz3.AppendLine("")



            Stb_JoinSaldiIniz.AppendLine(" FROM Conti_Patrimonio Conti_Pat ")
            Stb_JoinSaldiIniz.AppendLine(" INNER JOIN RicxConti_Patrimonio RicXConti_Pat ON Conti_Pat.Cod_Conto_Pat = RicXConti_Pat.Cod_Conto_Pat ")
            Stb_JoinSaldiIniz.AppendLine(" INNER JOIN Riclassificazioni_Patrimonio Riclassificazioni_Pat ON Riclassificazioni_Pat.Piva_SuperUser = RicXConti_Pat.Piva_SuperUser AND Riclassificazioni_Pat.Piva = RicXConti_Pat.Piva AND Riclassificazioni_Pat.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat  ")
            Stb_JoinSaldiIniz.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Pat.Piva = Imprese_Ric.PIVA ")
            '/************************************************************************

            '/************************************************************************
            '/*************** query PAGAMENTI *******************
            Stb_SelectPagamRiscossi1a.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, ('Incasso ' + Agenda.des_lib) AS des_lib,   ")
            Stb_SelectPagamRiscossi1a.AppendLine(" Conti_Pat.Cod_Conto_Pat, Conti_Pat.Piva AS piva_conti,   ")

            Stb_SelectPagam1a.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, ('Pagamento ' + Agenda.des_lib) AS des_lib,   ")
            Stb_SelectPagam1a.AppendLine(" Conti_Pat.Cod_Conto_Pat, Conti_Pat.Piva AS piva_conti,   ")


            Stb_SelectPagamRiscossi2.AppendLine(" Movimenti_Contab.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Movimenti_Contab.Doc_Numero)) + Movimenti_Contab.Doc_Numero_Des AS Numero_Doc,   ")
            Stb_SelectPagamRiscossi2.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento, ")
            'modifica dell'11/08/2015: aggiunto +1 al progr_registrazione per fare in modo che il pagamento venga visualizzato dopo il documento
            '(visto che hanno gli stessi id_agenda)
            Stb_SelectPagamRiscossi2.AppendLine(" Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione + 1,  ")
            '14/08/2015: la form del pagamento non ha la data di registrazione, Movimenti_Contab.Data_Registrazione è la data di registrazione della fattura
            'quindi uso Pagamenti.Data_Pagamento che è effettivamente la data del pagamento
            'Stb_SelectPagamRiscossi2.AppendLine(" CONVERT(date, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectPagamRiscossi2.AppendLine(" Pagamenti.Data_Pagamento AS Data_Registrazione, ")
            Stb_SelectPagamRiscossi2.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            Stb_SelectPagamRiscossi2.AppendLine(" Movimenti_Contab.Num_Protocollo  ")

            'Stb_SelectPagamRiscossi2.AppendLine(" '' AS Numero_Doc,   ")
            'Stb_SelectPagamRiscossi2.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento,  ")
            'Stb_SelectPagamRiscossi2.AppendLine(" Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, CONVERT(varchar, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            'Stb_SelectPagamRiscossi2.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            'Stb_SelectPagamRiscossi2.AppendLine(" Movimenti_Contab.Num_Protocollo  ")

            'Stb_JoinPagamenti.AppendLine(" INNER JOIN Movimenti movimenti_contab ON ")
            'Stb_JoinPagamenti.AppendLine(" pagamenti.Piva = movimenti_contab.piva ")
            'Stb_JoinPagamenti.AppendLine(" AND pagamenti.sa_cod = movimenti_contab.sa_cod ")
            'Stb_JoinPagamenti.AppendLine(" AND pagamenti.id_agenda = movimenti_contab.id_agenda ")

            '/************************************************************************
            '/*************** query PARTITA DOPPIA *******************

            Stb_SelectPartDoppia1a.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib,   ")
            Stb_SelectPartDoppia1a.AppendLine(" Conti_Pat.Cod_Conto_Pat, Conti_Pat.Piva AS piva_conti,   ")

            'conto senza riferimento
            Stb_SelectPartDoppiaNoRif.AppendLine(" Conti_Pat.Conto_Pat_Descr, Conti_Pat.Flag_UE,  ")
            'conto con riferimento contatto
            Stb_SelectPartDoppiaRifContatto.AppendLine(" Conti_Pat.Conto_Pat_Descr + ' - ' + Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome AS Conto_Pat_Descr, 2 AS Flag_UE,   ")

            'la Stb_SelectPartDoppiaRifBanche è stato suddiviso per gestire banche e cassa
            'la cassa deve avere flag_ue = 1, le banche 2
            'per distinguere cassa da banca si usa il cod_liquidita (non più, si usa il campo cau_risorsa)
            'che in un caso è dare, nell'altro avere
            'conto con riferimento banche
            'Stb_SelectPartDoppiaRifBanche.AppendLine("  Conti_Pat.Conto_Pat_Descr + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr, 1 AS Flag_UE,  ")
            'nota del 19/01/2015: quando si attiveranno più casse, anziché usare Cod_Liquidita_Avere = 0, si può discriminare su cau_risorsa = 2 della tabella liquidita
            Stb_SelectPartDoppiaRifBanche_Dare.AppendLine("  Conti_Pat.Conto_Pat_Descr + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr,   ")
            'Stb_SelectPartDoppiaRifBanche_Dare.AppendLine(" case when Cod_Liquidita_Dare = 0 THEN " & CStr(CONTO_UE) & " else " & CStr(CONTO_NONUE) & " end as flag_ue,   ")
            Stb_SelectPartDoppiaRifBanche_Dare.AppendLine(" case when Liquidita.Cau_risorsa = " & Agro_vb_SaveNum(enum_Liquidita_CauRisorsa.LiquiditaImmediata) & " THEN " & CStr(CONTO_UE) & " else " & CStr(CONTO_NONUE) & " end as flag_ue,   ")
            Stb_SelectPartDoppiaRifBanche_Avere.AppendLine("  Conti_Pat.Conto_Pat_Descr + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr,   ")
            'Stb_SelectPartDoppiaRifBanche_Avere.AppendLine(" case when Cod_Liquidita_Avere = 0 THEN " & CStr(CONTO_UE) & " else " & CStr(CONTO_NONUE) & " end as flag_ue,   ")
            Stb_SelectPartDoppiaRifBanche_Avere.AppendLine(" case when Liquidita.Cau_risorsa = " & Agro_vb_SaveNum(enum_Liquidita_CauRisorsa.LiquiditaImmediata) & " THEN " & CStr(CONTO_UE) & " else " & CStr(CONTO_NONUE) & " end as flag_ue,   ")

            'Stb_SelectPartDoppiaRifBanche.AppendLine("  Conti_Pat.Conto_Pat_Descr + ' - ' +  ")
            'Stb_SelectPartDoppiaRifBanche.AppendLine(" ISNULL(Istituto_Des, 'CASSA') ")
            ' Stb_SelectPartDoppiaRifBanche.AppendLine("  + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr,   ")

            'Stb_SelectPartDoppia1b.AppendLine(" Conti_Pat.Flag_UE, '' AS Cod_Contatto,  ")
            Stb_SelectPartDoppia1b.AppendLine(" '' AS Cod_Contatto,  ")
            Stb_SelectPartDoppia1b.AppendLine(" RicXConti_Pat.Piva, Imprese_Ric.rag_soc AS rag_soc,  ")
            Stb_SelectPartDoppia1b.AppendLine(" RicXConti_Pat.Anno, RicXConti_Pat.Id_Riclassificazione, RicXConti_Pat.Dare_Avere, RicXConti_Pat.Saldo,  ") '--RicXConti_Pat.Imputabile,
            Stb_SelectPartDoppia1b.AppendLine(" Riclassificazioni_Pat.Ric_Cod_Pat, Riclassificazioni_Pat.Ric_Des_Pat,  ")
            Stb_SelectPartDoppia1b.AppendLine(" 0 AS Imponibile_Netto,  ")
            Stb_SelectPartDoppia1b.AppendLine(" 0 AS elem_cod, 0 AS pro_cod, 0 AS mat_cod, ")

            '////////  DARE - AVERE - GESTIONE PATRIMONIALE /////////////////////////////        
            Stb_SelectPartDoppiaDARE.AppendLine(" SUM(Pagamenti.Importo) AS Dare, 0 AS Avere,    ")
            Stb_SelectPartDoppiaAVERE.AppendLine(" 0 AS Dare, SUM(Pagamenti.Importo) AS Avere,    ")

            '////////  PARTITA DOPPIA - parte2 della select per i movimenti senza aggancio a crediti/debiti ///////////////////////////// 
            Stb_SelectPartDoppia2.AppendLine(" '' AS Numero_Doc,   ")
            Stb_SelectPartDoppia2.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento,  ")
            Stb_SelectPartDoppia2.AppendLine(" Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, CONVERT(date, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectPartDoppia2.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            Stb_SelectPartDoppia2.AppendLine(" Movimenti_Contab.Num_Protocollo  ")

            '////////  PARTITA DOPPIA - parte2 della select per i movimenti senza aggancio a crediti/debiti ///////////////////////////// 
            Stb_SelectPD2_CreDeb.AppendLine(" ISNULL( ( SELECT TOP 1 Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Doc_Numero)) + Doc_Numero_Des ")
            Stb_SelectPD2_CreDeb.AppendLine("           FROM Movimenti MovFat ")
            Stb_SelectPD2_CreDeb.AppendLine("           INNER JOIN Mov_Dettagli_Riferimenti MDRif ON  MDRif.piva= MovFat.piva ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Id_Agenda_Rif = MovFat.Id_Agenda ")
            Stb_SelectPD2_CreDeb.AppendLine("           WHERE MovFat.cau_mov='" & CAU_REGISTRAZIONI & "' ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.cau_mov='" & CAU_REGISTRAZIONI & "' ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Lav_Cod=" & LAVCOD_MOV_FINANZIARIO & " ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Lav_Cod_Rif IN (" & LAVCOD_FATTURA_EMESSA & ", " & LAVCOD_FATTURA_RICEVUTA & ") ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Id_Agenda=	  Agenda.Id_Agenda ")
            Stb_SelectPD2_CreDeb.AppendLine("           ) , '') AS Numero_Doc, ")
            Stb_SelectPD2_CreDeb.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento,  ")
            Stb_SelectPD2_CreDeb.AppendLine(" ISNULL( ( SELECT TOP 1 Progr_Protocollo ")
            Stb_SelectPD2_CreDeb.AppendLine("           FROM Movimenti MovFat ")
            Stb_SelectPD2_CreDeb.AppendLine("           INNER JOIN Mov_Dettagli_Riferimenti MDRif ON  MDRif.piva= MovFat.piva ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Id_Agenda_Rif = MovFat.Id_Agenda ")
            Stb_SelectPD2_CreDeb.AppendLine("           WHERE MovFat.cau_mov='" & CAU_REGISTRAZIONI & "' ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.cau_mov='" & CAU_REGISTRAZIONI & "' ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Lav_Cod=" & LAVCOD_MOV_FINANZIARIO & " ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Lav_Cod_Rif IN (" & LAVCOD_FATTURA_EMESSA & ", " & LAVCOD_FATTURA_RICEVUTA & ") ")
            Stb_SelectPD2_CreDeb.AppendLine("           AND MDRif.Id_Agenda=	  Agenda.Id_Agenda ")
            Stb_SelectPD2_CreDeb.AppendLine("           ) , '') AS Progr_Protocollo, ")
            Stb_SelectPD2_CreDeb.AppendLine(" Movimenti_Contab.Progr_Registrazione, CONVERT(date, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectPD2_CreDeb.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            Stb_SelectPD2_CreDeb.AppendLine(" Movimenti_Contab.Num_Protocollo  ")

            Stb_joinPartDoppia1.AppendLine(" FROM Agenda ")
            Stb_joinPartDoppia1.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            Stb_joinPartDoppia1.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            Stb_joinPartDoppia1.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")
            Stb_joinPartDoppia1.AppendLine(" INNER JOIN Pagamenti ")
            Stb_joinPartDoppia1.AppendLine(" ON Pagamenti.PIVA = Movimenti_Contab.PIVA AND Pagamenti.Sa_Cod = Movimenti_Contab.Sa_Cod AND Pagamenti.Id_Agenda = Movimenti_Contab.Id_Agenda  AND Pagamenti.Id_MOV = Movimenti_Contab.Id_MOV  ")

            '////////  DARE - AVERE - GESTIONE PATRIMONIALE /////////////////////////////        
            Stb_joinPartDoppiaDARE.AppendLine(" INNER JOIN RicxConti_Patrimonio RicXConti_Pat ON Pagamenti.PIVA = RicXConti_Pat.Piva AND Pagamenti.Anno = RicXConti_Pat.Anno AND Pagamenti.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat  AND Pagamenti.Cod_Conto_Pat_Dare = RicXConti_Pat.Cod_Conto_Pat ")
            Stb_joinPartDoppiaAVERE.AppendLine(" INNER JOIN RicxConti_Patrimonio RicXConti_Pat ON Pagamenti.PIVA = RicXConti_Pat.Piva AND Pagamenti.Anno = RicXConti_Pat.Anno AND Pagamenti.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat  AND Pagamenti.Cod_Conto_Pat_Avere = RicXConti_Pat.Cod_Conto_Pat ")

            Stb_joinPartDoppia2.AppendLine(" INNER JOIN Conti_Patrimonio Conti_Pat ON Conti_Pat.Cod_Conto_Pat = RicXConti_Pat.Cod_Conto_Pat ")
            Stb_joinPartDoppia2.AppendLine(" INNER JOIN Riclassificazioni_Patrimonio Riclassificazioni_Pat ON Riclassificazioni_Pat.Piva_SuperUser = RicXConti_Pat.Piva_SuperUser AND Riclassificazioni_Pat.Piva = RicXConti_Pat.Piva AND Riclassificazioni_Pat.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat  ")
            Stb_joinPartDoppia2.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Pat.Piva = Imprese_Ric.PIVA ")

            Stb_GroupByPartDoppia.AppendLine("  GROUP BY Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib,  ")
            Stb_GroupByPartDoppia.AppendLine("  Conti_Pat.Cod_Conto_Pat, Conti_Pat.Piva,   ") 'Conti_Pat.Cod_Contatto,
            Stb_GroupByPartDoppia.AppendLine("  Conti_Pat.Conto_Pat_Descr,   ")
            Stb_GroupByPartDoppia.AppendLine("  Conti_Pat.Flag_UE, RicXConti_Pat.Piva, Imprese_Ric.rag_soc,  ")
            Stb_GroupByPartDoppia.AppendLine("  RicXConti_Pat.Anno, RicXConti_Pat.Id_Riclassificazione, RicXConti_Pat.Dare_Avere, RicXConti_Pat.Saldo,   ")
            Stb_GroupByPartDoppia.AppendLine("  Riclassificazioni_Pat.Ric_Cod_Pat, Riclassificazioni_Pat.Ric_Des_Pat, ")
            Stb_GroupByPartDoppia.AppendLine("  Movimenti_Contab.Data_Movimento, ")
            Stb_GroupByPartDoppia.AppendLine("  Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione,  ")
            Stb_GroupByPartDoppia.AppendLine("  Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Num_Protocollo  ")

            Stb_GroupByPartDoppiaRifContattoDare.AppendLine("  , Pagamenti.Tipo_Cod_Dare, contatti.Rag_Soc, contatti.Nome, contatti.cognome  ")
            Stb_GroupByPartDoppiaRifContattoAvere.AppendLine("  , Pagamenti.Tipo_Cod_Avere, contatti.Rag_Soc, contatti.Nome, contatti.cognome  ")

            Stb_GroupByPartDoppiaRifBancheDare.AppendLine(" , pagamenti.cod_liquidita_dare, Istituto_Des, Nazione, Cifre_Controllo,  Cin, Abi, Cab, Numero, Liquidita.cau_risorsa ")
            Stb_GroupByPartDoppiaRifBancheAvere.AppendLine("  , pagamenti.cod_liquidita_avere, Istituto_Des, Nazione, Cifre_Controllo,  Cin, Abi, Cab, Numero, Liquidita.cau_risorsa ")

            '/************************************************************************

            'If Flag_LeggiSaldiIniziali = True Then

            If Cod_RisUm <> 0 OrElse Lista_CodRisum <> "" OrElse Cod_Liquidita <> -1 Then
                'se ho fatto un tipo di filtro su contatti o banche, non leggo gli altri conti
            Else

                '/************************************************************************************
                '/***** 0 PARTE: SALDI INIZIALI CONTI PATRIMONIALI ***************
                '/************************************************************************************
                Stb_Globale.AppendLine(" -- 0a PARTE: SALDI INIZIALI ")
                Stb_Globale.AppendLine(" ( ")

                'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz1RC.ToString)
                Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2RC.ToString)
                Stb_Globale.AppendLine(Stb_SaldiInizialiRC.ToString)
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)
                Stb_Globale.AppendLine(Stb_JoinSaldiIniz.ToString)
                'CORREZIONE DEL 11/06/2015
                'mancava il filtro sui conti
                'quindi leggeva i saldi di tutto, sempre
                'Stb_Globale.AppendLine(" WHERE RicXConti_Pat.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                '18/03/2016: i saldi iniziali dei conti salvati in rixconti li leggo solo nell'anno di inizio gestione contabile
                'Stb_Globale.AppendLine(" AND RicXConti_Pat.Anno = " & Agro_SQL_SaveNum(Anno_Fine))
                Stb_Globale.AppendLine(" AND RicXConti_Pat.Anno = " & Agro_SQL_SaveNum(Anno_GestCont_DataInizio))

                'se non voglio i conti con saldo = 0
                If Not Flag_ContiSaldo0 Then
                    Stb_Globale.AppendLine(" AND RicXConti_Pat.Saldo_Iniziale <> 0   ")
                End If

                '§§§ gestione codifica piano dei conti
                'Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat NOT IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ",    ")
                'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DepositiBancariPostali) & ")    ")
                Stb_Globale.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat NOT IN ( " & enum_Conti_Patrimoniali.CreditiVersoClienti & ",    ")
                Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DebitiVersoFornitori & ",    ")
                Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DenaroValoriInCassa & ",    ")
                Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DepositiBancariPostali & ")    ")

                Stb_Globale.AppendLine(" ) ")

                FlagServeUnion = True

            End If

            '/************************************************************************************
            '/***** 0b PARTE: SALDI INIZIALI BANCHE ***************
            '/************************************************************************************
            'se non c'è il filtro o se ho scelto proprio il conto delle banche e se non sto facendo il report estrattoconto contatti
            If (Cod_Conto = 0 OrElse Cod_Conto = Cod_Conto_Pat_DepositiBancariPostali) AndAlso Lista_CodRisum = "" Then

                If Flag_AggiungiContoPadre_CreditiDebitiBanche Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 0b PARTE: CONTO PADRE BANCHE ")
                    Stb_Globale.AppendLine(" ( ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_DepositiBancariPostali) & "' AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                    Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_DepositiBancariPostali) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ")
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_DepositiBancariPostali) & "' AS Conto_Pat_Descr, 1 AS Flag_UE,   ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_DepositiBancariPostali) & "' AS Id_Riclassificazione, 'D' AS Dare_Avere,  ") '--RicXConti_Pat.Imputabile,
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                    Stb_Globale.AppendLine(" 0 AS Dare, 0 AS Avere, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)
                    Stb_Globale.AppendLine(" ) ")

                    FlagServeUnion = True
                End If

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 0b PARTE: SALDI INIZIALI BANCHE ")
                Stb_Globale.AppendLine(" ( ")
                'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Liquidita.Cod_Liquidita) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_DepositiBancariPostali) & "' + '_B' + CONVERT(varchar(500), Liquidita.Cod_Liquidita) AS CodiceSplitGruppo, ")

                Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                '§§§ gestione codifica piano dei conti
                'Stb_Globale.AppendLine(" " & CStr(enum_Conti_Patrimoniali.DepositiBancariPostali) & " AS Cod_Conto_Pat, '" & CStr(Piva) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_DepositiBancariPostali & "' AS Conto_Pat_Descr,  
                Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_DepositiBancariPostali) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_DepositiBancariPostali & "' AS Conto_Pat_Descr,  
                'Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_DepositiBancariPostali) & "' + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr, 2 AS Flag_UE,  ")
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_DepositiBancariPostali) & "' AS Id_Riclassificazione, 'D' AS Dare_Avere,  ") '--RicXConti_Pat.Imputabile,
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                Stb_Globale.AppendLine(Stb_SaldiInizialiLIQ.ToString)
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)

                'Stb_Globale.AppendLine(Stb_JoinSaldiIniz.ToString)
                'Stb_Globale.AppendLine(" INNER JOIN Liquidita ON Liquidita.Piva = RicXConti_Pat.Piva ")

                Stb_Globale.AppendLine(" FROM Liquidita  ")
                Stb_Globale.AppendLine("  INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                'Stb_Globale.AppendLine(" WHERE RicXConti_Pat.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                'Stb_Globale.AppendLine(" AND RicXConti_Pat.Cod_Conto_Pat = " & CStr(enum_Conti_Patrimoniali.DepositiBancariPostali) & " ")
                'Stb_Globale.AppendLine(" AND Liquidita.Saldo_Iniziale <> 0   ")
                Stb_Globale.AppendLine(" WHERE Liquidita.Saldo_Iniziale <> 0   ")
                Stb_Globale.AppendLine(" AND Liquidita.Cau_Risorsa = " & Agro_SQL_SaveNum(enum_Liquidita_CauRisorsa.RisorsaFinanziaria))

                If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                    Stb_Globale.AppendLine(" AND Liquidita.Cod_Liquidita = " & Agro_SQL_SaveNum(Cod_Liquidita))
                End If
                Stb_Globale.AppendLine(" AND Liquidita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                Stb_Globale.AppendLine(" ) ")

                FlagServeUnion = True

            End If 'filtro Cod_Conto



            '/************************************************************************************
            '/***** 0c PARTE: SALDI INIZIALI CONTATTI - CREDITI ***************
            '/************************************************************************************
            'se non c'è il filtro o se ho scelto proprio il conto crediti o se sto facendo il report estrattoconto contatti
            If Cod_Conto = 0 OrElse Cod_Conto = Cod_Conto_Pat_CreditiVersoClienti OrElse Lista_CodRisum <> "" Then

                If Flag_AggiungiContoPadre_CreditiDebitiBanche Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 0c PARTE: SALDI PADRE CONTATTI - CREDITI ")
                    Stb_Globale.AppendLine(" ( ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_CreditiVersoClienti) & "'  AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                    '§§§ gestione codifica piano dei conti
                    Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_CreditiVersoClienti) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ")
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_CreditiVersoClienti) & "' AS Conto_Pat_Descr, 1 AS Flag_UE,  ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_CreditiVersoClienti) & "' AS Id_Riclassificazione, 'D' AS Dare_Avere,  ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                    Stb_Globale.AppendLine(" 0 AS Dare, 0 AS Avere, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)
                    Stb_Globale.AppendLine(" ) ")

                    FlagServeUnion = True

                End If

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 0c PARTE: SALDI INIZIALI CONTATTI - CREDITI ")
                Stb_Globale.AppendLine(" ( ")
                'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Risorse_Umane.Cod_RisUm) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_CreditiVersoClienti) & "' + '_C' + CONVERT(varchar(500), Risorse_Umane.Cod_RisUm) AS CodiceSplitGruppo, ")

                'Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                'Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto)
                'Stb_Globale.AppendLine(Stb_SelectSaldiIniz2.ToString)
                'Stb_Globale.AppendLine(Stb_SaldiIniziRisUmCred.ToString)
                'Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)
                'Stb_Globale.AppendLine(Stb_JoinSaldiIniz.ToString)
                'Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Piva = RicXConti_Pat.Piva ")

                Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                '§§§ gestione codifica piano dei conti
                'Stb_Globale.AppendLine(" " & CStr(enum_Conti_Patrimoniali.CreditiVersoClienti) & " AS Cod_Conto_Pat, '" & CStr(Piva) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_CreditiVersoClienti & "' AS Conto_Pat_Descr,  
                Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_CreditiVersoClienti) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_CreditiVersoClienti & "' AS Conto_Pat_Descr,  
                'Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_CreditiVersoClienti) & "' + ' - ' + Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome AS Conto_Pat_Descr, 2 AS Flag_UE,   ")
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_CreditiVersoClienti) & "' AS Id_Riclassificazione, 'D' AS Dare_Avere,  ") '--RicXConti_Pat.Imputabile,
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                Stb_Globale.AppendLine(Stb_SaldiIniziRisUmCred.ToString)
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)

                Stb_Globale.AppendLine(" FROM Risorse_Umane  ")
                Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")
                Stb_Globale.AppendLine(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.cod_rapporto = Rapporti_Contabili.cod_rapporto  ")

                'Stb_Globale.AppendLine(" WHERE RicXConti_Pat.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                'Stb_Globale.AppendLine(" AND RicXConti_Pat.Cod_Conto_Pat = " & CStr(enum_Conti_Patrimoniali.CreditiVersoClienti) & " ")
                'Stb_Globale.AppendLine(" AND Risorse_Umane.Saldo_Iniziale_Crediti  <> 0   ")
                Stb_Globale.AppendLine(" WHERE Risorse_Umane.Saldo_Iniziale_Crediti  <> 0   ")
                If Lista_CodRisum <> "" Then
                    Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                End If
                If Cod_RisUm <> 0 Then
                    Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum = " & Agro_SQL_SaveNum(Cod_RisUm))
                End If
                Stb_Globale.AppendLine(" AND Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                '23/11/2015: commento perché voglio cercare per tutti i contatti esistenti (se uno ha valorizzato sui fornitori si è sbagliato, ma almeno si vede)
                'Stb_Globale.AppendLine(" AND (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Terzista= 1) ")
                Stb_Globale.AppendLine(" AND (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  OR Contatti.sa_cod = -1) ")

                Stb_Globale.AppendLine(" ) ")

                FlagServeUnion = True

            End If 'filtro cod_conto

            '/************************************************************************************
            '/***** 0d PARTE: SALDI INIZIALI CONTATTI - DEBITI ***************
            '/************************************************************************************
            'se non c'è il filtro o se ho scelto proprio il conto debiti o se sto facendo il report estrattoconto contatti
            If Cod_Conto = 0 OrElse Cod_Conto = Cod_Conto_Pat_DebitiVersoFornitori OrElse Lista_CodRisum <> "" Then

                If Flag_AggiungiContoPadre_CreditiDebitiBanche Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 0c PARTE: SALDI PADRE CONTATTI - DEBITI ")
                    Stb_Globale.AppendLine(" ( ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_DebitiVersoFornitori) & "' AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                    '§§§ gestione codifica piano dei conti
                    Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_DebitiVersoFornitori) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ")
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_DebitiVersoFornitori) & "'  AS Conto_Pat_Descr, 1 AS Flag_UE,  ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_DebitiVersoFornitori) & "' AS Id_Riclassificazione, 'A' AS Dare_Avere,  ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                    Stb_Globale.AppendLine(" 0 AS Dare, 0 AS Avere, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)
                    Stb_Globale.AppendLine(" ) ")

                    FlagServeUnion = True

                End If

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 0d PARTE: SALDI INIZIALI CONTATTI - DEBITI ")
                Stb_Globale.AppendLine(" ( ")
                'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Risorse_Umane.Cod_RisUm) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_DebitiVersoFornitori) & "' + '_C' + CONVERT(varchar(500), Risorse_Umane.Cod_RisUm) AS CodiceSplitGruppo, ")

                'Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                'Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto)
                'Stb_Globale.AppendLine(Stb_SelectSaldiIniz2.ToString)
                'Stb_Globale.AppendLine(Stb_SaldiIniziRisUmDeb.ToString)
                'Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)
                'Stb_Globale.AppendLine(Stb_JoinSaldiIniz.ToString)
                'Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Piva = RicXConti_Pat.Piva ")

                Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                '§§§ gestione codifica piano dei conti
                'Stb_Globale.AppendLine(" " & CStr(enum_Conti_Patrimoniali.DebitiVersoFornitori) & " AS Cod_Conto_Pat, '" & CStr(Piva) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_DebitiVersoFornitori & "' AS Conto_Pat_Descr,  
                Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_DebitiVersoFornitori) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_DebitiVersoFornitori & "' AS Conto_Pat_Descr,  
                'Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_DebitiVersoFornitori) & "' + ' - ' + Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome AS Conto_Pat_Descr, 2 AS Flag_UE,   ")
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_DebitiVersoFornitori) & "' AS Id_Riclassificazione, 'A' AS Dare_Avere,  ") '--RicXConti_Pat.Imputabile,
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                Stb_Globale.AppendLine(Stb_SaldiIniziRisUmDeb.ToString)
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)

                Stb_Globale.AppendLine(" FROM Risorse_Umane  ")
                Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")
                Stb_Globale.AppendLine(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.cod_rapporto = Rapporti_Contabili.cod_rapporto  ")

                'Stb_Globale.AppendLine(" WHERE RicXConti_Pat.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                'Stb_Globale.AppendLine(" AND RicXConti_Pat.Cod_Conto_Pat = " & CStr(enum_Conti_Patrimoniali.DebitiVersoFornitori) & " ")
                'Stb_Globale.AppendLine(" AND Risorse_Umane.Saldo_Iniziale_Debiti  <> 0   ")

                Stb_Globale.AppendLine(" WHERE Risorse_Umane.Saldo_Iniziale_Debiti  <> 0   ")
                If Cod_RisUm <> 0 Then
                    Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum = " & Agro_SQL_SaveNum(Cod_RisUm))
                End If
                If Lista_CodRisum <> "" Then
                    Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                End If
                Stb_Globale.AppendLine(" AND Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                '23/11/2015: commento perché voglio cercare per tutti i contatti esistenti (se uno ha valorizzato sui clienti si è sbagliato, ma almeno si vede)
                'Stb_Globale.AppendLine(" AND (Rapporti_Contabili.fornitore = 1 OR Rapporti_Contabili.Terzista= 1) ")
                Stb_Globale.AppendLine(" AND (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  OR Contatti.sa_cod = -1) ")

                Stb_Globale.AppendLine(" ) ")

                FlagServeUnion = True

            End If 'filtro cod_conto


            '/************************************************************************************
            '/***** 0e - f PARTE: SALDI INIZIALI CASSA  ***************
            '/************************************************************************************
            'se non c'è il filtro o se ho scelto proprio il conto delle banche e se non sto facendo il report estrattoconto contatti
            If (Cod_Conto = 0 OrElse Cod_Conto = Cod_Conto_Pat_DenaroValoriInCassa) AndAlso Lista_CodRisum = "" Then

                If Flag_AggiungiContoPadre_CreditiDebitiBanche Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 0e PARTE: CONTO PADRE CASSE ")
                    Stb_Globale.AppendLine(" ( ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_DenaroValoriInCassa) & "' AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                    Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_DenaroValoriInCassa) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ")
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_DenaroValoriInCassa) & "' AS Conto_Pat_Descr, 1 AS Flag_UE,   ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                    Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_DenaroValoriInCassa) & "' AS Id_Riclassificazione, 'D' AS Dare_Avere,  ") '--RicXConti_Pat.Imputabile,
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                    Stb_Globale.AppendLine(" 0 AS Dare, 0 AS Avere, ")
                    Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)
                    Stb_Globale.AppendLine(" ) ")

                    FlagServeUnion = True
                End If

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 0f PARTE: SALDI INIZIALI CASSA ")
                Stb_Globale.AppendLine(" ( ")
                Stb_Globale.AppendLine(" SELECT 'SP_' + '" & Agro_SQL_SaveText(Id_Riclassificazione_DenaroValoriInCassa) & "' + '_B' + CONVERT(varchar(500), Liquidita.Cod_Liquidita) AS CodiceSplitGruppo, ")

                Stb_Globale.AppendLine(Stb_SelectSaldiIniz1.ToString)
                '§§§ gestione codifica piano dei conti
                'Stb_Globale.AppendLine(" " & CStr(enum_Conti_Patrimoniali.DepositiBancariPostali) & " AS Cod_Conto_Pat, '" & CStr(Piva) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_DepositiBancariPostali & "' AS Conto_Pat_Descr,  
                Stb_Globale.AppendLine(" " & CStr(Cod_Conto_Pat_DenaroValoriInCassa) & " AS Cod_Conto_Pat, '" & Agro_SQL_SaveText(CStr(Piva)) & "' AS piva_conti,  ") ''" & Conto_Pat_Descr_DepositiBancariPostali & "' AS Conto_Pat_Descr,  
                'Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Conto_Pat_Descr_DenaroValoriInCassa) & "' + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr, 1 AS Flag_UE,   ")
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2a.ToString)
                Stb_Globale.AppendLine(" '" & Agro_SQL_SaveText(Id_Riclassificazione_DenaroValoriInCassa) & "' AS Id_Riclassificazione, 'D' AS Dare_Avere,  ") '--RicXConti_Pat.Imputabile,
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz2b.ToString)
                Stb_Globale.AppendLine(Stb_SaldiInizialiLIQ.ToString)
                Stb_Globale.AppendLine(Stb_SelectSaldiIniz3.ToString)

                Stb_Globale.AppendLine(" FROM Liquidita  ")
                Stb_Globale.AppendLine("  INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                Stb_Globale.AppendLine(" WHERE Liquidita.Saldo_Iniziale <> 0   ")
                'modifica del 08/02/2016: conn l'introduzione della multi-cassa cod_liquidita non sarà più solo 0
                ''devo filtrare cod_liquidita = 0 che è la cassa (altrimenti mette in join con tutti gli iban delle banche)
                'Stb_Globale.AppendLine(" AND Liquidita.Cod_Liquidita = 0   ")
                Stb_Globale.AppendLine(" AND Liquidita.Cau_Risorsa = " & Agro_SQL_SaveNum(enum_Liquidita_CauRisorsa.LiquiditaImmediata))

                If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                    Stb_Globale.AppendLine(" AND Liquidita.Cod_Liquidita = " & Agro_SQL_SaveNum(Cod_Liquidita))
                End If

                Stb_Globale.AppendLine(" AND Liquidita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                Stb_Globale.AppendLine(" ) ")

            End If 'filtro Cod_Conto

            Stb_Globale.AppendLine("  ")
            Stb_Globale.AppendLine(" -- FINE SALDI INIZIALI ")

            FlagServeUnion = True

            'End If 'saldi iniziali If Flag_LeggiSaldiIniziali Then

            If Not Flag_SOLOSaldiIniziali Then

                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 1a PARTE: DOC CONTABILI (ACQUISTI) FILTRATI X DATA REGISTRAZIONE  ************
                    '/****************  no crediti/debiti (senza link ai contatti ) *********************
                    '/************************************************************************************

                    Stb_Globale.AppendLine(" -- 1a PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")
                    Stb_Globale.AppendLine(" -- senza link ai contatti / risorse finanziarie ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti3.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    '§§§ gestione codifica piano dei conti
                    'Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat NOT IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                    'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ")    ")
                    Stb_Globale.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat NOT IN ( " & enum_Conti_Patrimoniali.CreditiVersoClienti & ",    ")
                    Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DebitiVersoFornitori & ")    ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                             CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                             CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                             CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                             CStr(LAVCOD_ALTRI_COSTI) &
                                                             " ) ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 1a PARTE: DOC CONTABILI (ACQUISTI) FILTRATI X DATA REGISTRAZIONE no crediti/debiti ")

                    FlagServeUnion = True

                End If 'filtro Cod_RisUm = 0 And Cod_Liquidita = -1

                '/************************************************************************************
                '/***** 1b PARTE: DOC CONTABILI (ACQUISTI) FILTRATI X DATA REGISTRAZIONE  ************
                '/******************   conti crediti / debiti sul contatto *****************************
                '/************************************************************************************
                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 1b PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")
                    Stb_Globale.AppendLine(" -- conti crediti / debiti sul contatto  ")
                    Stb_Globale.AppendLine(" ( ")

                    ''Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Risorse_Umane.Cod_RisUm) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti3.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Movimenti_Contab.Cod_RisUm  ")
                    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    '§§§ gestione codifica piano dei conti
                    'Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                    'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ")    ")
                    Stb_Globale.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat IN ( " & enum_Conti_Patrimoniali.CreditiVersoClienti & ",    ")
                    Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DebitiVersoFornitori & ")    ")


                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                 CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                                 CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                                 CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                                 CStr(LAVCOD_ALTRI_COSTI) &
                                                                 " ) ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 1b PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")

                    FlagServeUnion = True

                End If ' Cod_Liquidita = -1

                'questa parte la commento perché al momento non so se nella fattura/bolla viene considerato il legame del conto con la banca
                ''/************************************************************************************
                ''/***** 1c PARTE: DOC CONTABILI (ACQUISTI) FILTRATI X DATA REGISTRAZIONE  ************
                ''/******************   conti crediti / debiti sulla banca *****************************
                ''/************************************************************************************

                'Stb_Globale.AppendLine(" -- 1c PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")
                'Stb_Globale.AppendLine(" ( ")

                ' ''Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                ''Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Liquidita.Cod_Liquidita) AS CodiceSplitGruppo, ")
                'Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                'Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche.ToString)
                'Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                'Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                'Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Registrazione AS Data_Order ")

                'Stb_Globale.AppendLine(Stb_JoinConti.ToString)
                'Stb_Globale.AppendLine(Stb_JoinContab.ToString)
                'Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_avere = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                'Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                'Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                'Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DepositiBancariPostali) & " )    ")


                'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                ''leggi nota sopra, sulla data fine
                'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                ''Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                'Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '                                         CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                '                                         CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                '                                         CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                '                                         CStr(LAVCOD_ALTRI_COSTI) &
                '                                         " ) ")

                'Stb_Globale.AppendLine(" ) ")
                'Stb_Globale.AppendLine(" -- FINE 1 PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")

                'Stb_Globale.AppendLine(" ")
                'Stb_Globale.AppendLine(" ")
                'Stb_Globale.AppendLine(" UNION ALL ")


                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 2a° PARTE: DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO  ***************
                    '/**************** no crediti/debiti (senza link ai contatti)  *********************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 2a PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO ")
                    Stb_Globale.AppendLine(" -- no crediti/debiti (senza link ai contatti) ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti3.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_dettagli.sconto_modalita NOT IN (" & enModalitaSconto.Omaggio_SenzaRivalsaIva & " , " & enModalitaSconto.Omaggio_ConRivalsaIva & " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    '§§§ gestione codifica piano dei conti
                    'Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat NOT IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                    'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ")    ")
                    Stb_Globale.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat NOT IN ( " & enum_Conti_Patrimoniali.CreditiVersoClienti & ",    ")
                    Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DebitiVersoFornitori & ")    ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                                " ) ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 2a PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO ")

                    FlagServeUnion = True

                End If 'Cod_RisUm = 0 And Cod_Liquidita = -1 

                '/************************************************************************************
                '/***** 2b° PARTE: DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO  ***************
                '/****************  conti crediti / debiti sul contatto  *********************
                '/************************************************************************************
                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 2b PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO ")
                    Stb_Globale.AppendLine(" -- conti crediti / debiti sul contatto  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Risorse_Umane.Cod_RisUm) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti3.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Movimenti_Contab.Cod_RisUm  ")
                    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.sconto_modalita NOT IN (" & enModalitaSconto.Omaggio_SenzaRivalsaIva & " , " & enModalitaSconto.Omaggio_ConRivalsaIva & " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    '§§§ gestione codifica piano dei conti
                    'Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                    'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ")    ")
                    Stb_Globale.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat IN ( " & enum_Conti_Patrimoniali.CreditiVersoClienti & ",    ")
                    Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DebitiVersoFornitori & ")    ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                                " ) ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 2b PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO ")

                    FlagServeUnion = True

                End If '  If Cod_Liquidita = -1


                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 2C° PARTE: CORRISPETTIVI DI VENDITA  ***************
                    '/**************** senza link ai contatti  *********************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 2C PARTE: CORRISPETTIVI DI VENDITA ")
                    Stb_Globale.AppendLine(" -- senza link ai contatti ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Dare) AS CodiceSplitGruppo, ")

                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)

                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    'Stb_Globale.AppendLine(" Conti_Pat.Conto_Pat_Descr + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr, Conti_Pat.Flag_UE, ")

                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti3.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                    'Stb_Globale.AppendLine(" INNER JOIN Pagamenti ON Pagamenti.PIVA = Movimenti_Contab.PIVA AND Pagamenti.Sa_Cod = Movimenti_Contab.Sa_Cod AND Pagamenti.Id_Agenda = Movimenti_Contab.Id_Agenda  AND Pagamenti.Id_MOV = Movimenti_Contab.Id_MOV  ")

                    'Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_dare = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                    'Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")


                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    ''§§§ gestione codifica piano dei conti
                    ''Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat NOT IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                    ''Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ")    ")
                    'Stb_Globale.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat NOT IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                    'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ")    ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod = " & CStr(LAVCOD_VENDITA))

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 2C PARTE: CORRISPETTIVI DI VENDITA ")

                    FlagServeUnion = True

                End If

                '/************************************************************************************
                '/***** 2d° PARTE: DOC CONTABILI (VENDITE) CON OMAGGI FILTRATI X DATA MOVIMENTO  ***************
                '/****************  conti crediti / debiti sul contatto  *********************
                '/************************************************************************************
                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 2d° PARTE: DOC CONTABILI (VENDITE) CON OMAGGI FILTRATI X DATA MOVIMENTO ")
                    Stb_Globale.AppendLine(" -- conti crediti / debiti sul contatto  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Risorse_Umane.Cod_RisUm) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)

                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)

                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectOmaggiDareAvere.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Movimenti_Contab.Cod_RisUm  ")
                    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    'leggo solo gli omaggi con rivalsa iva (perché il cliente deve pagare solo l'iva), senza rivalsa iva non deve pagare niente e quindi non li leggo
                    'Stb_Globale.AppendLine(" AND    Movimenti_dettagli.sconto_modalita IN (" & enModalitaSconto.Omaggio_SenzaRivalsaIva & " , " & enModalitaSconto.Omaggio_ConRivalsaIva & " ) ")
                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_ConRivalsaIva & "  ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    '§§§ gestione codifica piano dei conti
                    'Stb_Globale.AppendLine(" AND RicXConti_Pat.cod_conto_pat IN ( " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.CreditiVersoClienti) & ",    ")
                    'Stb_Globale.AppendLine("                                            " & Agro_SQL_SaveText(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ")    ")
                    Stb_Globale.AppendLine(" AND RicXConti_Pat.Codifica_Conto_Pat IN ( " & enum_Conti_Patrimoniali.CreditiVersoClienti & ",    ")
                    Stb_Globale.AppendLine("                                            " & enum_Conti_Patrimoniali.DebitiVersoFornitori & ")    ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                                " ) ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 2d° PARTE: DOC CONTABILI (VENDITE) CON OMAGGI FILTRATI X DATA MOVIMENTO ")

                    FlagServeUnion = True

                End If '  If Cod_Liquidita = -1


                '/************************************************************************************
                '/***** 3° PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE  ***************
                '/************************************************************************************
                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 3 PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE ")
                    Stb_Globale.AppendLine(" ( ")

                    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti3.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Agenda.lav_cod = " & CStr(LAVCOD_ACQUISTO))

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 3 PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE ")

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")


                    '/************************************************************************************
                    '/***** 4° PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 4 PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti3.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND   Movimenti_Mag.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod IN ( " & CStr(LAVCOD_AUTOCONSUMO) & "," & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ")")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 4 PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ")

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")

                    '/************************************************************************************
                    '/***** 5° PARTE: PARTITA DOPPIA - CONTI AVERE  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 5° PARTE: PARTITA DOPPIA - CONTI AVERE  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))

                    'riferimento non impostato
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 0 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere = -1 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 5° PARTE: PARTITA DOPPIA - CONTI AVERE  ")

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")

                    Stb_Globale.AppendLine(" UNION ALL ")

                    '/************************************************************************************
                    '/***** 6° PARTE: PARTITA DOPPIA - CONTI DARE  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 6° PARTE: PARTITA DOPPIA - CONTI DARE  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    'riferimento non impostato
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Dare = 0 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Dare = -1 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 6° PARTE: PARTITA DOPPIA - CONTI DARE  ")

                    FlagServeUnion = True

                    '/**********************************************************************************

                End If ' filtro cod_risum e Cod_Liquidita

                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 7° PARTE: PARTITA DOPPIA - CONTI AVERE - CON RIFERIMENTO CONTATTI  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 7° PARTE: PARTITA DOPPIA - CONTI AVERE - CON RIFERIMENTO CONTATTI ")
                    Stb_Globale.AppendLine(" ( ")

                    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPD2_CreDeb.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = pagamenti.tipo_cod_avere  ")
                    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    'riferimento impostato
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Avere <> 0 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifContattoAvere.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 7° PARTE: PARTITA DOPPIA - CONTI AVERE - CON RIFERIMENTO CONTATTI ")

                    '/**********************************************************************************


                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    '/************************************************************************************
                    '/***** 8° PARTE: PARTITA DOPPIA - CONTI DARE - CON RIFERIMENTO CONTATTI  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 8° PARTE: PARTITA DOPPIA - CONTI DARE - CON RIFERIMENTO CONTATTI ")
                    Stb_Globale.AppendLine(" ( ")

                    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Dare) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Dare) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPD2_CreDeb.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = pagamenti.tipo_cod_dare  ")
                    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva AND Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    'riferimento impostato
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Dare = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Dare <> 0 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifContattoDare.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 8° PARTE: PARTITA DOPPIA - CONTI DARE - CON RIFERIMENTO CONTATTI ")

                    FlagServeUnion = True

                End If 'If Cod_Liquidita = -1 


                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 9° PARTE: PARTITA DOPPIA - CONTI AVERE - CON RIFERIMENTO BANCHE E CASSA ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 9° PARTE: PARTITA DOPPIA - CONTI AVERE - CON RIFERIMENTO BANCHE E CASSA")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Avere.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_avere = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                    Stb_Globale.AppendLine("  INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    ''riferimento impostato -> non c'è nelle banche, è solo per i contatti
                    'Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 1 ")
                    '0 è la cassa e il -1 è il non definito
                    '10/08/2015: inserito >= altrimenti la cassa non veniva letta
                    Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere >= 0 ")

                    If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere = " & Agro_SQL_SaveNum(Cod_Liquidita))
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheAvere.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 9° PARTE: PARTITA DOPPIA - CONTI AVERE - CON RIFERIMENTO BANCHE E CASSA")

                    '/**********************************************************************************

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    '/************************************************************************************
                    '/***** 10° PARTE: PARTITA DOPPIA - CONTI DARE - CON RIFERIMENTO BANCHE E CASSA ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 10° PARTE: PARTITA DOPPIA - CONTI DARE - CON RIFERIMENTO BANCHE E CASSA")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Dare) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Dare) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Dare.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_dare = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                    Stb_Globale.AppendLine("  INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    ''riferimento impostato -> non c'è nelle banche, è solo per i contatti
                    'Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Dare = 1 ")
                    '0 è la cassa e il -1 è il non definito
                    '10/08/2015: inserito >= altrimenti la prima cassa non veniva letta
                    Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Dare >= 0 ")

                    If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Dare = " & Agro_SQL_SaveNum(Cod_Liquidita))
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheDare.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 10° PARTE: PARTITA DOPPIA - CONTI DARE - CON RIFERIMENTO BANCHE E CASSA")

                    '/**********************************************************************************

                    FlagServeUnion = True

                End If '  If Cod_RisUm = 0


                '/************************************************************************************
                '/***** 11a° PARTE: RISCOSSIONE PAGAMENTI - CONTO CREDITI IN AVERE - CON RIFERIMENTO CONTATTI (CoGe Auto)  ***************
                '/************************************************************************************
                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 11a° PARTE: RISCOSSIONE PAGAMENTI - CONTO CREDITI IN AVERE - CON RIFERIMENTO CONTATTI ")
                    Stb_Globale.AppendLine(" ( ")

                    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    'Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = pagamenti.tipo_cod_avere  ")
                    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    'pagamenti di questa tipologia di documenti
                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                               CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                               CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                               CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                               CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                               CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                               CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                               CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                               CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                               " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    'filtro per pagamenti
                    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                    'riferimento impostato
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Avere <> 0 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifContattoAvere.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 11a° PARTE: RISCOSSIONE PAGAMENTI - CONTO CREDITI IN AVERE - CON RIFERIMENTO CONTATTI ")

                    '/**********************************************************************************

                    FlagServeUnion = True

                End If 'If Cod_Liquidita = -1

                ''/************************************************************************************
                ''/***** 11a2° PARTE: RISCOSSIONE PAGAMENTI - CONTO CREDITI IN AVERE - CON RIFERIMENTO CONTATTI (CoGe Manuale)  ***************
                ''/************************************************************************************
                'If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                '    If FlagServeUnion = True Then
                '        Stb_Globale.AppendLine("  ")
                '        Stb_Globale.AppendLine(" UNION ALL ")
                '        Stb_Globale.AppendLine("  ")
                '    End If

                '    Stb_Globale.AppendLine(" -- 11a2° PARTE: RISCOSSIONE PAGAMENTI - CONTO CREDITI IN AVERE - CON RIFERIMENTO CONTATTI (CoGe Manuale)")
                '    Stb_Globale.AppendLine(" ( ")

                '    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi1a.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                '    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                '    'Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                '    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = pagamenti.tipo_cod_avere  ")
                '    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                '    Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                '    'leggi nota sopra, sulla data fine
                '    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                '    If flagBilancio = True Then
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                '    Else
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                '    End If

                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                '    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_MANUALE_NO_CONTABILIZZAZIONE) & "   ")

                '    'pagamenti di questa tipologia di documenti
                '    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '                                               CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                '                                               CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                '                                               CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                '                                               CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                '                                               CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                '                                               CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                '                                               CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                '                                               CStr(LAVCOD_ALTRI_RICAVI) & " " &
                '                                               " ) ")

                '    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                '    End If

                '    If Cod_RisUm <> 0 Then
                '        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                '    End If
                '    If Lista_CodRisum <> "" Then
                '        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_SaveText(Lista_CodRisum) & ") ")
                '    End If

                '    'filtro per pagamenti
                '    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                '    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                '    'riferimento impostato
                '    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 1 ")
                '    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Avere <> 0 ")

                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifContattoAvere.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                '    Stb_Globale.AppendLine(" ) ")
                '    Stb_Globale.AppendLine(" -- 11a2° PARTE: RISCOSSIONE PAGAMENTI - CONTO CREDITI IN AVERE - CON RIFERIMENTO CONTATTI (CoGe Manuale) ")

                '    '/**********************************************************************************

                '    FlagServeUnion = True

                'End If 'If Cod_Liquidita = -1

                '/************************************************************************************
                '/***** 11b° PARTE: RISCOSSIONE CORRISPETTIVI VENDITA - CONTO IN AVERE - senza RIFERIMENTO CONTATTI   ***************
                '/************************************************************************************
                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 11b° PARTE: RISCOSSIONE CORRISPETTIVI VENDITA - CONTO IN AVERE - senza RIFERIMENTO CONTATTI  ")
                    Stb_Globale.AppendLine(" ( ")

                    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")

                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")

                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi1a.ToString)

                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)


                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    'pagamenti di questa tipologia di documenti
                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod = " & CStr(LAVCOD_VENDITA))

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    'filtro per pagamenti
                    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                    'riferimento impostato
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 1 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 11b° PARTE: RISCOSSIONE CORRISPETTIVI VENDITA - CONTO IN AVERE - senza RIFERIMENTO CONTATTI  ")

                    '/**********************************************************************************

                    FlagServeUnion = True

                End If 'If Cod_Liquidita = -1


                '/************************************************************************************
                '/***** 12° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN DARE - CON RIFERIMENTO BANCHE E CASSA  (CoGe Auto) ***************
                '/************************************************************************************
                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 12° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN DARE - CON RIFERIMENTO BANCHE E CASSA ")
                    Stb_Globale.AppendLine(" ( ")

                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Dare) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Dare.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    '   Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_dare = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                    Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    'pagamenti di questa tipologia di documenti
                    'Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                    '                                           CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                    '                                           CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                    '                                           CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                    '                                           CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                    '                                           CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                    '                                           CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                    '                                           CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                    '                                           CStr(LAVCOD_ALTRI_RICAVI) & " " &
                    '                                           " ) ")


                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                               CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                               CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                               CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                               CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                               CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                               CStr(LAVCOD_VENDITA) & ", " &
                                                               CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                               CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                               CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                               " ) ")




                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    'filtro per pagamenti
                    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                    'per le ris finanz è 0
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Dare = 0 ")
                    ' Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Dare <> 0 ")

                    If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Dare = " & Agro_SQL_SaveNum(Cod_Liquidita))
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheDare.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 12° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN DARE - CON RIFERIMENTO BANCHE E CASSA")

                    FlagServeUnion = True

                    '/**********************************************************************************

                End If ' If Cod_RisUm = 0

                ''/************************************************************************************
                ''/***** 12b° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN DARE - CON RIFERIMENTO BANCHE E CASSA  (CoGe Manuale) ***************
                ''/************************************************************************************
                'If Cod_RisUm = 0 And Lista_CodRisum = "" Then

                '    If FlagServeUnion = True Then
                '        Stb_Globale.AppendLine("  ")
                '        Stb_Globale.AppendLine(" UNION ALL ")
                '        Stb_Globale.AppendLine("  ")
                '    End If

                '    Stb_Globale.AppendLine(" -- 12b° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN DARE - CON RIFERIMENTO BANCHE E CASSA (CoGe Manuale)")
                '    Stb_Globale.AppendLine(" ( ")

                '    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Dare) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi1a.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Dare.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                '    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                '    '   Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                '    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_dare = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                '    Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                '    Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                '    'leggi nota sopra, sulla data fine
                '    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                '    If flagBilancio = True Then
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                '    Else
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                '    End If

                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                '    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                '    'pagamenti di questa tipologia di documenti
                '    'Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '    '                                           CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                '    '                                           CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                '    '                                           CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                '    '                                           CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                '    '                                           CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                '    '                                           CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                '    '                                           CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                '    '                                           CStr(LAVCOD_ALTRI_RICAVI) & " " &
                '    '                                           " ) ")


                '    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '                                               CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                '                                               CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                '                                               CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                '                                               CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                '                                               CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                '                                               CStr(LAVCOD_VENDITA) & ", " &
                '                                               CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                '                                               CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                '                                               CStr(LAVCOD_ALTRI_RICAVI) & " " &
                '                                               " ) ")




                '    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                '    End If

                '    'filtro per pagamenti
                '    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                '    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                '    'per le ris finanz è 0
                '    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Dare = 0 ")
                '    ' Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Dare <> 0 ")

                '    If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Dare = " & Agro_SQL_SaveNum(Cod_Liquidita))
                '    End If

                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheDare.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                '    Stb_Globale.AppendLine(" ) ")
                '    Stb_Globale.AppendLine(" -- 12b° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN DARE - CON RIFERIMENTO BANCHE E CASSA (CoGe Manuale)")

                '    FlagServeUnion = True

                '    '/**********************************************************************************

                'End If ' If Cod_RisUm = 0

                '/************************************************************************************
                '/***** 13° PARTE: PAGAMENTO DEBITI - CONTO DEBITI IN DARE - CON RIFERIMENTO CONTATTI (CoGe Auto) ***************
                '/************************************************************************************
                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 13° PARTE: PAGAMENTO DEBITI - CONTO DEBITI IN DARE - CON RIFERIMENTO CONTATTI ")
                    Stb_Globale.AppendLine(" ( ")

                    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Dare) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagam1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    'Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = pagamenti.tipo_cod_Dare  ")
                    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    'pagamenti di questa tipologia di documenti
                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                 CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                                 CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                                 CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                                 CStr(LAVCOD_ALTRI_COSTI) &
                                                                 " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    'filtro per pagamenti
                    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                    'riferimento impostato
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Dare = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Dare <> 0 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifContattoDare.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 13° PARTE: PAGAMENTO DEBITI - CONTO DEBITI IN DARE - CON RIFERIMENTO CONTATTI ")

                    '/**********************************************************************************

                    FlagServeUnion = True

                End If 'If Cod_Liquidita = -1


                ''/************************************************************************************
                ''/***** 13b° PARTE: PAGAMENTO DEBITI - CONTO DEBITI IN DARE - CON RIFERIMENTO CONTATTI (CoGe Manuale) ***************
                ''/************************************************************************************
                'If Cod_Liquidita = CODLIQUIDITA_NOFILTRO Then

                '    If FlagServeUnion = True Then
                '        Stb_Globale.AppendLine("  ")
                '        Stb_Globale.AppendLine(" UNION ALL ")
                '        Stb_Globale.AppendLine("  ")
                '    End If

                '    Stb_Globale.AppendLine(" -- 13b° PARTE: PAGAMENTO DEBITI - CONTO DEBITI IN DARE - CON RIFERIMENTO CONTATTI (CoGe Manuale)")
                '    Stb_Globale.AppendLine(" ( ")

                '    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Avere) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_C' + CONVERT(varchar(500), Pagamenti.Tipo_Cod_Dare) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(Stb_SelectPagam1a.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifContatto.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                '    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                '    'Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                '    Stb_Globale.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = pagamenti.tipo_cod_Dare  ")
                '    Stb_Globale.AppendLine(" INNER JOIN contatti ON Risorse_Umane.piva = contatti.piva and Risorse_Umane.cod_contatto = contatti.cod_contatto  ")

                '    Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                '    'leggi nota sopra, sulla data fine
                '    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                '    If flagBilancio = True Then
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                '    Else
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                '    End If

                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                '    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_MANUALE_NO_CONTABILIZZAZIONE) & "   ")

                '    'pagamenti di questa tipologia di documenti
                '    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '                                                 CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                '                                                 CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                '                                                 CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                '                                                 CStr(LAVCOD_ALTRI_COSTI) &
                '                                                 " ) ")

                '    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                '    End If

                '    If Cod_RisUm <> 0 Then
                '        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                '    End If
                '    If Lista_CodRisum <> "" Then
                '        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_SaveText(Lista_CodRisum) & ") ")
                '    End If

                '    'filtro per pagamenti
                '    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                '    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                '    'riferimento impostato
                '    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Dare = 1 ")
                '    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Dare <> 0 ")

                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifContattoDare.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                '    Stb_Globale.AppendLine(" ) ")
                '    Stb_Globale.AppendLine(" -- 13b° PARTE: PAGAMENTO DEBITI - CONTO DEBITI IN DARE - CON RIFERIMENTO CONTATTI (CoGe Manuale)")

                '    '/**********************************************************************************

                '    FlagServeUnion = True

                'End If 'If Cod_Liquidita = -1


                '/************************************************************************************
                '/***** 14° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN AVERE - CON RIFERIMENTO BANCHE E CASSA (CoGe Auto) ***************
                '/************************************************************************************
                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 14° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN AVERE - CON RIFERIMENTO BANCHE E CASSA")
                    Stb_Globale.AppendLine(" ( ")

                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagam1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Avere.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                    '   Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_Avere = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                    Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    'LAVCOD_FATTURA_PROFESSIONISTI non va messo qui perché è gestita nella 19° parte
                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                         CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                                         CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                                         CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                                         CStr(LAVCOD_ALTRI_COSTI) &
                                                                         " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    'filtro per pagamenti
                    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                    'per le ris finanz è 0
                    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 0 ")
                    ' Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Dare <> 0 ")

                    If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere = " & Agro_SQL_SaveNum(Cod_Liquidita))
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheAvere.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 14° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN AVERE - CON RIFERIMENTO BANCHE E CASSA")

                    FlagServeUnion = True

                    '/**********************************************************************************

                End If 'If Cod_RisUm = 0

                ''/************************************************************************************
                ''/***** 14b° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN AVERE - CON RIFERIMENTO BANCHE E CASSA (CoGe Manuale)***************
                ''/************************************************************************************
                'If Cod_RisUm = 0 And Lista_CodRisum = "" Then

                '    If FlagServeUnion = True Then
                '        Stb_Globale.AppendLine("  ")
                '        Stb_Globale.AppendLine(" UNION ALL ")
                '        Stb_Globale.AppendLine("  ")
                '    End If

                '    Stb_Globale.AppendLine(" -- 14b° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN AVERE - CON RIFERIMENTO BANCHE E CASSA (Coge Manuale)")
                '    Stb_Globale.AppendLine(" ( ")

                '    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Avere) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(Stb_SelectPagam1a.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Avere.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                '    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)
                '    '   Stb_Globale.AppendLine(Stb_JoinPagamenti.ToString)
                '    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_Avere = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                '    Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")

                '    Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                '    'leggi nota sopra, sulla data fine
                '    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                '    If flagBilancio = True Then
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                '    Else
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                '    End If

                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                '    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_MANUALE_NO_CONTABILIZZAZIONE) & "   ")

                '    'pagamenti di questa tipologia di documenti
                '    'Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '    '                                                     CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                '    '                                                     CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                '    '                                                     CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                '    '                                                     CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                '    '                                                     CStr(LAVCOD_ALTRI_COSTI) &
                '    '                                                     " ) ")
                '    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '                                                         CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                '                                                         CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                '                                                         CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                '                                                         CStr(LAVCOD_ALTRI_COSTI) &
                '                                                         " ) ")

                '    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                '    End If

                '    'filtro per pagamenti
                '    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                '    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                '    'per le ris finanz è 0
                '    Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Avere = 0 ")
                '    ' Stb_Globale.AppendLine(" AND Pagamenti.Tipo_Cod_Dare <> 0 ")

                '    If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere = " & Agro_SQL_SaveNum(Cod_Liquidita))
                '    End If

                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheAvere.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                '    Stb_Globale.AppendLine(" ) ")
                '    Stb_Globale.AppendLine(" -- 14b° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN AVERE - CON RIFERIMENTO BANCHE E CASSA (CoGe Manuale)")

                '    FlagServeUnion = True

                '    '/**********************************************************************************

                'End If 'If Cod_RisUm = 0


                '/************************************************************************************
                '/***** 15 PARTE: IVA NS CREDITO - DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ************
                '/************************************************************************************

                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO AndAlso Lista_CodRisum = "" Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 15 PARTE: IVA NS CREDITO - DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")
                    Stb_Globale.AppendLine(" --  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectIVA.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVA.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVAcredito.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.Iva <> 0  ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    ' leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                             CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                             CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                             CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                             CStr(LAVCOD_ALTRI_COSTI) &
                                                             " ) ")

                    '  Giulia, 20/09/2016 15.35.53: note sbagliate:
                    'Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                    '                                         CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                    '                                         CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                    '                                         CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                    '                                         CStr(LAVCOD_ALTRI_COSTI) &
                    '                                         " ) ")
                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 15 PARTE: IVA NS CREDITO  - DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")

                    FlagServeUnion = True

                End If 'Cod_Liquidita = -1


                '/************************************************************************************
                '/***** 16° PARTE: IVA NS CREDITO - ACQUISTO FILTRATO X DATA REGISTRAZIONE  ***************
                '/************************************************************************************
                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 16° PARTE: IVA NS CREDITO - ACQUISTO FILTRATO X DATA REGISTRAZIONE  ")
                    Stb_Globale.AppendLine(" ( ")

                    ' Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectIVA.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVA.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVAcredito.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.Iva <> 0  ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Agenda.lav_cod = " & CStr(LAVCOD_ACQUISTO))

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 16° PARTE: IVA NS CREDITO - ACQUISTO FILTRATO X DATA REGISTRAZIONE  ")

                    FlagServeUnion = True

                End If


                '/************************************************************************************
                '/*** 17° PARTE: IVA NS DEBITO - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO  ****
                '/**************** no crediti/debiti (senza link ai contatti)  *********************
                '/************************************************************************************

                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO AndAlso Lista_CodRisum = "" Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 17° PARTE: IVA NS DEBITO - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO ")
                    Stb_Globale.AppendLine(" -- no crediti/debiti (senza link ai contatti) ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectIVA.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVA.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVAdebito.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.Iva <> 0  ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                                CStr(LAVCOD_VENDITA) & ", " &
                                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                                " ) ")

                    '  Giulia, 20/09/2016 15.36.27: note sbagliate?!?:
                    'Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                    '                        CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                    '                        CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                    '                        CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                    '                        CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                    '                        CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                    '                        CStr(LAVCOD_VENDITA) & ", " &
                    '                        CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                    '                        CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                    '                        CStr(LAVCOD_ALTRI_RICAVI) & " " &
                    '                        " ) ")


                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 17° PARTE: IVA NS DEBITO - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO ")

                    FlagServeUnion = True

                End If 'If Cod_Liquidita = -1 


                '/************************************************************************************
                '/*** 17bis° PARTE: IVA NS DEBITO (COMP ESTERO) - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO  ****
                '/**************** no crediti/debiti (senza link ai contatti)  *********************
                '/************************************************************************************

                If Cod_Liquidita = CODLIQUIDITA_NOFILTRO AndAlso Lista_CodRisum = "" Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 17bis° PARTE: IVA NS DEBITO (COMP ESTERO) - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO ")
                    Stb_Globale.AppendLine(" -- x Compensazione IVA Estero ")
                    Stb_Globale.AppendLine(" -- no crediti/debiti (senza link ai contatti) ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)

                    'Stb_Globale.AppendLine(Stb_SelectIVA.ToString)
                    Stb_Globale.AppendLine(" -- Fatture, DDT, ... emessi")
                    Stb_Globale.AppendLine("  CASE WHEN Movimenti_dettagli.Imponibile_Netto > 0  ")
                    Stb_Globale.AppendLine("           THEN Movimenti_dettagli.iva_indetraibile ")
                    Stb_Globale.AppendLine("  ELSE 0  ")
                    Stb_Globale.AppendLine("  END AS Dare, ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  -- Nota di accredito al cliente emessa ")
                    Stb_Globale.AppendLine("  CASE WHEN Movimenti_dettagli.Imponibile_Netto < 0  ")
                    Stb_Globale.AppendLine("           THEN (-1 * Movimenti_dettagli.iva_indetraibile) ")
                    Stb_Globale.AppendLine("  ELSE 0  ")
                    Stb_Globale.AppendLine("  END AS Avere,   ")
                    Stb_Globale.AppendLine(" -- ")


                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVA.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVAdebito.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(" AND Movimenti_dettagli.Iva = 0  ")
                    Stb_Globale.AppendLine(" AND Movimenti_dettagli.Iva_Indetraibile_Perc <> 0  ")
                    Stb_Globale.AppendLine(" AND Movimenti_dettagli.Cod_Iva IN ( " &
                                                                            CStr(Art4DL331) & ", " &
                                                                            CStr(NonImpArt8DPR633_72) & ", " &
                                                                            CStr(NonImpArt8) & ", " &
                                                                            CStr(NonImpArt8C1LetB) & ", " &
                                                                            CStr(NonImpArt8C1LetC) &
                                                                            ")  ")

                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If
                    If Lista_CodRisum <> "" Then
                        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum IN ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisum) & ") ")
                    End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                                CStr(LAVCOD_VENDITA) & ", " &
                                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                                " ) ")

                    '  Giulia, 20/09/2016 15.36.27: note sbagliate?!?:
                    'Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                    '                        CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                    '                        CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                    '                        CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                    '                        CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                    '                        CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                    '                        CStr(LAVCOD_VENDITA) & ", " &
                    '                        CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                    '                        CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                    '                        CStr(LAVCOD_ALTRI_RICAVI) & " " &
                    '                        " ) ")


                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 17bis° PARTE: IVA NS DEBITO (COMP ESTERO) - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO ")

                    FlagServeUnion = True

                End If 'If Cod_Liquidita = -1 






                ''/************************************************************************************
                ''/*** 17°b PARTE: IVA NS DEBITO - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO  ****
                ''/**************** no crediti/debiti (senza link ai contatti)  *********************
                ''/**************** test per tirare su quelli con gestione manuale  *********************
                ''/************************************************************************************

                'If Cod_Liquidita = CODLIQUIDITA_NOFILTRO And Lista_CodRisum = "" Then

                '    If FlagServeUnion = True Then
                '        Stb_Globale.AppendLine("  ")
                '        Stb_Globale.AppendLine(" UNION ALL ")
                '        Stb_Globale.AppendLine("  ")
                '    End If

                '    Stb_Globale.AppendLine(" -- 17°b PARTE: IVA NS DEBITO - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO ")
                '    Stb_Globale.AppendLine(" -- no crediti/debiti (senza link ai contatti) ")
                '    Stb_Globale.AppendLine(" -- test per tirare su quelli con gestione manuale ")
                '    Stb_Globale.AppendLine(" ( ")

                '    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectIVA.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                '    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                '    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                '    Stb_Globale.AppendLine(Stb_JoinIVA.ToString)
                '    Stb_Globale.AppendLine(Stb_JoinIVAdebito.ToString)
                '    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                '    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                '    Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                '    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_MANUALE_NO_CONTABILIZZAZIONE) & "   ")

                '    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                '    End If

                '    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.Iva <> 0  ")

                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                '    If Cod_RisUm <> 0 Then
                '        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                '    End If
                '    If Lista_CodRisum <> "" Then
                '        Stb_Globale.AppendLine(" AND Risorse_Umane.Cod_Risum  IN ( " & Agro_SQL_SaveText(Lista_CodRisum) & ") ")
                '    End If

                '    'leggi nota sopra, sulla data fine
                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                '    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                '                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                '                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                '                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                '                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                '                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                '                                                CStr(LAVCOD_VENDITA) & ", " &
                '                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                '                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                '                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                '                                                " ) ")

                '    Stb_Globale.AppendLine(" ) ")
                '    Stb_Globale.AppendLine(" -- FINE 17° PARTE: IVA NS DEBITO - DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO ")

                '    FlagServeUnion = True

                'End If 'If Cod_Liquidita = -1 


                '/************************************************************************************
                '/***** 18° PARTE: IVA NS DEBITO - AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ***************
                '/************************************************************************************

                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 18° PARTE: IVA NS DEBITO - AUTOCONSUMO FILTRATO X DATA MOVIMENTO ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Pat.Cod_Conto_Pat) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectConti2.ToString)
                    Stb_Globale.AppendLine(Stb_SelectIVA.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVA.ToString)
                    Stb_Globale.AppendLine(Stb_JoinIVAdebito.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.Iva <> 0  ")

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND   Movimenti_Mag.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod IN ( " & CStr(LAVCOD_AUTOCONSUMO) & "," & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ")")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 18° PARTE: IVA NS DEBITO - AUTOCONSUMO FILTRATO X DATA MOVIMENTO ")

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")

                    FlagServeUnion = True

                End If


                '/************************************************************************************
                '/***** 19° PARTE: PAGAMENTO FATTURE - GESTIONE RITENUTA ACCONTO ED ENASARCO (CoGe Auto) ***************
                '/************************************************************************************
                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" Then
                    'If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 19° PARTE: RISCOSSIONE PAGAMENTI - GESTIONE RITENUTA ACCONTO ED ENASARCO ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Avere) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagam1a.ToString)

                    'Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    'Stb_Globale.AppendLine(" Conti_Pat.Conto_Pat_Descr + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr, Conti_Pat.Flag_UE, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Avere.ToString)

                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_Avere = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                    Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")


                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    '§§§16/01/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    '(questo pezzo era rimasto indietro)
                    '  Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")


                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " & CStr(LAVCOD_FATTURA_PROFESSIONISTI) & " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    'filtro per pagamenti
                    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                    '  Giulia, 27/10/2016 16.58.37: prendeva su tutti i pagamenti (anche quelli delle altre banche)
                    If Cod_Liquidita <> CODLIQUIDITA_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere = " & Agro_SQL_SaveNum(Cod_Liquidita))
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                    'Stb_Globale.AppendLine(" , pagamenti.cod_liquidita_avere, Istituto_Des, Nazione, Cifre_Controllo,  Cin, Abi, Cab, Numero ")
                    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheAvere.ToString)

                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 19° PARTE: RISCOSSIONE PAGAMENTI - GESTIONE RITENUTA ACCONTO ED ENASARCO ")

                    FlagServeUnion = True

                    '/**********************************************************************************

                End If 'If Cod_RisUm = 0


                '/************************************************************************************
                '/***** 19bis° PARTE: PAGAMENTO FATTURE - GESTIONE RITENUTA ACCONTO ED ENASARCO (solo ritenuta ed enasarco)  ***************
                '/************************************************************************************
                If Cod_RisUm = 0 AndAlso Lista_CodRisum = "" AndAlso Cod_Liquidita = -1 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    Stb_Globale.AppendLine(" -- 19bis° PARTE: RISCOSSIONE PAGAMENTI - GESTIONE RITENUTA ACCONTO ED ENASARCO (solo ritenuta ed enasarco) ")
                    Stb_Globale.AppendLine(" ( ")

                    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagam1a.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                    If flagBilancio Then
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                    Else
                        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    End If

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " & CStr(LAVCOD_FATTURA_PROFESSIONISTI) & " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    'filtro per pagamenti
                    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")
                    Stb_Globale.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere = -1 ")

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)
                    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 19bis° PARTE: RISCOSSIONE PAGAMENTI - GESTIONE RITENUTA ACCONTO ED ENASARCO (solo ritenuta ed enasarco) ")

                    FlagServeUnion = True

                    '/**********************************************************************************

                End If 'If Cod_RisUm = 0



                ''/************************************************************************************
                ''/***** 19b° PARTE: PAGAMENTO FATTURE - GESTIONE RITENUTA ACCONTO ED ENASARCO (CoGe Manuale) ***************
                ''/************************************************************************************
                'If Cod_RisUm = 0 And Lista_CodRisum = "" Then
                '    'If Cod_RisUm = 0 And Lista_CodRisum = "" And Cod_Liquidita = -1 Then

                '    If FlagServeUnion = True Then
                '        Stb_Globale.AppendLine("  ")
                '        Stb_Globale.AppendLine(" UNION ALL ")
                '        Stb_Globale.AppendLine("  ")
                '    End If

                '    Stb_Globale.AppendLine(" -- 19b° PARTE: RISCOSSIONE PAGAMENTI - GESTIONE RITENUTA ACCONTO ED ENASARCO (CoGe Manuale) ")
                '    Stb_Globale.AppendLine(" ( ")

                '    'Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(" SELECT 'SP_' + Id_Riclassificazione + '_B' + CONVERT(varchar(500), Pagamenti.Cod_Liquidita_Avere) AS CodiceSplitGruppo, ")
                '    Stb_Globale.AppendLine(Stb_SelectPagam1a.ToString)

                '    'Stb_Globale.AppendLine(Stb_SelectPartDoppiaNoRif.ToString)
                '    'Stb_Globale.AppendLine(" Conti_Pat.Conto_Pat_Descr + ' - ' + Istituto_Des + ' ' +  Nazione + Cifre_Controllo +  Cin +  Abi + Cab + Numero AS Conto_Pat_Descr, Conti_Pat.Flag_UE, ")
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaRifBanche_Avere.ToString)

                '    Stb_Globale.AppendLine(Stb_SelectPartDoppia1b.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                '    Stb_Globale.AppendLine(Stb_SelectPagamRiscossi2.ToString)
                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento AS Data_Order ")

                '    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                '    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                '    Stb_Globale.AppendLine(" INNER JOIN Liquidita ON pagamenti.cod_liquidita_Avere = Liquidita.cod_liquidita AND pagamenti.piva = Liquidita.piva  ")
                '    Stb_Globale.AppendLine(" INNER JOIN Ist_Credito ON Liquidita.cod_istituto = Ist_Credito.cod_istituto  ")


                '    Stb_Globale.AppendLine(Stb_WhereConti.ToString)

                '    'leggi nota sopra, sulla data fine
                '    Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                '    If flagBilancio = True Then
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(GestCont_DataInizio) & " ")
                '    Else
                '        Stb_Globale.AppendLine(" AND    Pagamenti.Data_Pagamento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                '    End If

                '    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                '    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_MANUALE_NO_CONTABILIZZAZIONE) & "   ")

                '    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " & CStr(LAVCOD_FATTURA_PROFESSIONISTI) & " ) ")

                '    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                '        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                '    End If

                '    'filtro per pagamenti
                '    Stb_Globale.AppendLine(" AND Pagamenti.previsto_avvenuto = 1 ")
                '    Stb_Globale.AppendLine(" AND Pagamenti.importo <> 0 ")

                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                '    'Stb_Globale.AppendLine(" , pagamenti.cod_liquidita_avere, Istituto_Des, Nazione, Cifre_Controllo,  Cin, Abi, Cab, Numero ")
                '    Stb_Globale.AppendLine(Stb_GroupByPartDoppiaRifBancheAvere.ToString)

                '    Stb_Globale.AppendLine(" , Pagamenti.Data_Pagamento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_des ")

                '    Stb_Globale.AppendLine(" ) ")
                '    Stb_Globale.AppendLine(" -- 19b° PARTE: RISCOSSIONE PAGAMENTI - GESTIONE RITENUTA ACCONTO ED ENASARCO (CoGe Manuale)")

                '    FlagServeUnion = True

                '    '/**********************************************************************************

                'End If 'If Cod_RisUm = 0


            End If 'Flag_SOLOSaldiIniziali


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Stb_Globale.Length = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return Stb_Globale

    End Function

    '###############################################################################
    'elenco dei movimenti dei conti economici nell'intervallo temporale
    Public Function MastrinoConti_Economici(ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByVal Piva As String,
                                            ByVal Anno As Integer,
                                            ByVal Ric_Cod As Integer,
                                            ByVal Id_Riclassificazione As String,
                                            ByVal Dare_Avere As String,
                                            ByVal Imputabile As Integer,
                                            ByVal Cod_Conto As Integer,
                                            ByVal Flag_UE As Integer,
                                            ByVal Cod_Contatto As String,
                                            ByVal Sezionale_Cod As Integer,
                                            ByVal Cod_RisUm As Integer,
                                            ByVal FlagEscludiIvaIndetraibile As Boolean,
                                            ByVal xFiltroAggiuntivo1 As String,
                                            ByVal xFiltroAggiuntivo2 As String,
                                            ByVal xFiltroAggiuntivo3 As String,
                                            ByVal xFiltroAggiuntivo4 As String,
                                            ByVal xOrderBy As String,
                                            ByVal DT_Codifiche As DataTable,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.MastrinoConti_Economici()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_QueryContiXMov As New StringBuilder

        Dim dt As DataTable
        ' Dim xFiltroAggiuntivo_xDataRegistrazione, xFiltroAggiuntivo_xDataMovimento As String

        Try
            Stb_Globale.Length = 0
            Stb_QueryContiXMov.Length = 0

            'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti
            Stb_QueryContiXMov = Query_Conti_Economici_Movimentati(False,
                                                                   False,
                                                                   AGRODATAINIZIO,
                                                                   Data_Inizio,
                                                                   Data_Fine,
                                                                   Piva,
                                                                   Anno,
                                                                   Ric_Cod,
                                                                   Id_Riclassificazione,
                                                                   Dare_Avere,
                                                                   Imputabile,
                                                                   Cod_Conto,
                                                                   Flag_UE,
                                                                   Cod_Contatto,
                                                                   Sezionale_Cod,
                                                                   Cod_RisUm,
                                                                   False,
                                                                   FlagEscludiIvaIndetraibile,
                                                                   xFiltroAggiuntivo1,
                                                                   xFiltroAggiuntivo2,
                                                                   xFiltroAggiuntivo3,
                                                                   xFiltroAggiuntivo4,
                                                                   DT_Codifiche,
                                                                   objParametri,
                                                                   flagNuoviArrotondamenti)

            If Stb_QueryContiXMov.ToString <> "" Then

                '///////////////////////////////////////////////
                Stb_Globale.AppendLine(" SELECT * ")

                'select CodiceSplitGruppo,Anno,Data_Order,dare_avere,Conto_Eco_Descr,Id_Riclassificazione,Des_Lib,Data_Movimento,Data_Registrazione,Progr_Protocollo,
                'sum(Dare) as Dare,
                'sum(Avere) as Avere


                Stb_Globale.AppendLine(" FROM ")

                Stb_Globale.AppendLine(" ( ")

                Stb_Globale.AppendLine(Stb_QueryContiXMov.ToString)


                '/************************************************************************************
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ) MASTRINO ")


                'GROUP BY CodiceSplitGruppo,Anno,Data_Order,dare_avere,Conto_Eco_Descr,Id_Riclassificazione,Des_Lib,Data_Movimento,Data_Registrazione,Progr_Protocollo
                Stb_Globale.AppendLine(" ORDER BY CodiceSplitGruppo, Anno, Id_Riclassificazione, Data_Order, Progr_Protocollo, Progr_Registrazione  ")

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

            Else
                dt = Nothing
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    'ritorna lo stringbuilder con la query che legge tutte le movimentazioni dei conti economici
    'DEFAULT
    'FLAG_UE -> CONTO_UE_NOFILTRO
    Private Function Query_Conti_Economici_Movimentati(ByVal Flag_LeggiSaldiIniziali As Boolean,
                                                       ByVal Flag_SOLOSaldiIniziali As Boolean,
                                                       ByVal GestCont_DataInizio As Date,
                                                       ByVal Data_Inizio As Date,
                                                       ByVal Data_Fine As Date,
                                                       ByVal Piva As String,
                                                       ByVal Anno As Integer,
                                                       ByVal Ric_Cod As Integer,
                                                       ByVal Id_Riclassificazione As String,
                                                       ByVal Dare_Avere As String,
                                                       ByVal Imputabile As Integer,
                                                       ByVal Cod_Conto As Integer,
                                                       ByVal Flag_UE As Integer,
                                                       ByVal Cod_Contatto As String,
                                                       ByVal Sezionale_Cod As Integer,
                                                       ByVal Cod_RisUm As Integer,
                                                       ByVal Flag_ContiSaldo0 As Boolean,
                                                       ByVal FlagEscludiIvaIndetraibile As Boolean,
                                                       ByVal xFiltroAggiuntivo1 As String,
                                                       ByVal xFiltroAggiuntivo2 As String,
                                                       ByVal xFiltroAggiuntivo3 As String,
                                                       ByVal xFiltroAggiuntivo4 As String,
                                                       ByVal DT_Codifiche As DataTable,
                                                       ByRef objParametri As AgronicaCoreParametri,
                                                       Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                                       ) As StringBuilder

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Query_Conti_Economici_Movimentati()"

        Dim messaggioErrore As String = ""
        Dim Stb_Globale As New StringBuilder
        Dim Stb_SaldiIniziali As New StringBuilder
        Dim Stb_SelectConti As New StringBuilder
        Dim Stb_SelectContiDareAvere As New StringBuilder
        Dim Stb_SelectOmaggiDareAvere As New StringBuilder
        Dim Stb_SelectIVAindetr As New StringBuilder
        Dim Stb_SelectIVAcompens As New StringBuilder
        Dim Stb_JoinConti1 As New StringBuilder
        Dim Stb_JoinConti2 As New StringBuilder
        Dim Stb_JoinConti2IVA As New StringBuilder
        Dim Stb_JoinConti3 As New StringBuilder
        Dim Stb_WhereConti As New StringBuilder
        Dim Stb_WhereContiSezionaliContab As New StringBuilder
        Dim Stb_WhereContiSezionaliMag As New StringBuilder
        Dim Stb_SelectContab As New StringBuilder
        Dim Stb_SelectNoContab As New StringBuilder
        Dim Stb_JoinContab As New StringBuilder

        Dim Stb_SelectPagamentiCEDare1 As New StringBuilder
        Dim Stb_SelectPagamentiCEAvere1 As New StringBuilder
        Dim Stb_SelectPagamentiCE1 As New StringBuilder
        Dim Stb_SelectPagamentiCE2 As New StringBuilder
        Dim Stb_SelectPartDoppia1 As New StringBuilder
        Dim Stb_SelectPartDoppiaDARE As New StringBuilder
        Dim Stb_SelectPartDoppiaAVERE As New StringBuilder
        Dim Stb_SelectPartDoppia2 As New StringBuilder
        Dim Stb_joinPartDoppia1 As New StringBuilder
        Dim Stb_joinPartDoppiaDARE As New StringBuilder
        Dim Stb_joinPartDoppiaAVERE As New StringBuilder
        Dim Stb_joinPartDoppia2 As New StringBuilder
        Dim Stb_GroupByPartDoppia As New StringBuilder

        Dim FlagServeUnion As Boolean = False

        Dim Id_Riclassificazione_RicavixIVAincompensazione As String = ""
        Dim Conto_Descr_RicavixIVAincompensazione As String = ""
        Dim Cod_Conto_RicavixIVAincompensazione As Integer = 0
        Dim Id_Riclassificazione_OmaggiAllaClientela As String = ""
        Dim Cod_Conto_OmaggiAllaClientela As Integer = 0
        Dim Conto_Descr_OmaggiAllaClientela As String = ""
        Dim Id_Riclassificazione_RicavixIVAincompensazioneEstero As String = ""
        Dim Cod_Conto_RicavixIVAincompensazioneEstero As Integer = 0
        Dim Conto_Descr_RicavixIVAincompensazioneEstero As String = ""

        Dim Anno_GestCont_DataInizio As Integer = GestCont_DataInizio.Year
        Dim Anno_Fine As Integer = Data_Fine.Year

        ' Dim DT As DataTable
        ' Dim xFiltroAggiuntivo_xDataRegistrazione, xFiltroAggiuntivo_xDataMovimento As String

        Try

            'gestione dei conti economici "automatizzati" (x iva in compensazione e iva indetraibile)
            Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
            objContabHLP.Recupera_Cod_Des_ContiEconomici(DT_Codifiche,
                                                         Id_Riclassificazione_RicavixIVAincompensazione,
                                                         Cod_Conto_RicavixIVAincompensazione,
                                                         Conto_Descr_RicavixIVAincompensazione,
                                                         Id_Riclassificazione_OmaggiAllaClientela,
                                                         Cod_Conto_OmaggiAllaClientela,
                                                         Conto_Descr_OmaggiAllaClientela,
                                                         Id_Riclassificazione_RicavixIVAincompensazioneEstero,
                                                         Cod_Conto_RicavixIVAincompensazioneEstero,
                                                         Conto_Descr_RicavixIVAincompensazioneEstero)


        Catch ex As Exception

        End Try

        Try

            Stb_Globale.Length = 0
            Stb_SaldiIniziali.Length = 0
            Stb_SelectConti.Length = 0
            Stb_SelectContiDareAvere.Length = 0
            Stb_SelectIVAindetr.Length = 0
            Stb_SelectIVAcompens.Length = 0
            Stb_JoinConti1.Length = 0
            Stb_JoinConti2.Length = 0
            Stb_JoinConti2IVA.Length = 0
            Stb_JoinConti3.Length = 0
            Stb_WhereConti.Length = 0
            Stb_WhereContiSezionaliContab.Length = 0
            Stb_WhereContiSezionaliMag.Length = 0
            Stb_SelectContab.Length = 0
            Stb_SelectNoContab.Length = 0
            Stb_JoinContab.Length = 0

            Stb_SelectPartDoppia1.Length = 0
            Stb_SelectPartDoppiaAVERE.Length = 0
            Stb_SelectPartDoppiaDARE.Length = 0
            Stb_SelectPartDoppia2.Length = 0
            Stb_joinPartDoppia1.Length = 0
            Stb_joinPartDoppiaAVERE.Length = 0
            Stb_joinPartDoppiaDARE.Length = 0
            Stb_joinPartDoppia2.Length = 0

            'poiché il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))

            '/************************************************************************
            '/*************** query conti  *******************
            Stb_SelectConti.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib,   ")
            Stb_SelectConti.AppendLine(" Conti_Eco.Cod_Conto AS Cod_Conto_Eco, Conti_Eco.Piva AS piva_conti,   ")
            Stb_SelectConti.AppendLine(" Conti_Eco.Conto_Descr AS Conto_Eco_Descr, Conti_Eco.Flag_UE, Conti_Eco.Cod_Contatto,  ")
            Stb_SelectConti.AppendLine(" RicXConti_Eco.Piva, Imprese_Ric.rag_soc AS rag_soc,  ")
            Stb_SelectConti.AppendLine(" RicXConti_Eco.Anno, RicXConti_Eco.Id_Riclassificazione, RicXConti_Eco.Dare_Avere, RicXConti_Eco.Saldo,  ") '--RicXConti_Eco.Imputabile,
            Stb_SelectConti.AppendLine(" Riclassificazioni_Eco.Ric_Cod AS Ric_Cod_Eco, Riclassificazioni_Eco.Ric_Des as Ric_Des_Eco,  ")
            If flagNuoviArrotondamenti Then
                Stb_SelectConti.AppendLine(" ROUND(ISNULL(Movimenti_dettagli.Imponibile_Netto, 0), 2) AS Imponibile_Netto,  ")
            Else
                Stb_SelectConti.AppendLine(" ISNULL(Movimenti_dettagli.Imponibile_Netto, 0) AS Imponibile_Netto,  ")
            End If
            Stb_SelectConti.AppendLine(" Movimenti_dettagli.elem_cod, Movimenti_dettagli.pro_cod, Movimenti_dettagli.mat_cod, ")

            'Stb_SelectConti.AppendLine(" CASE WHEN RicXConti_Eco.Dare_Avere = 'D' THEN Movimenti_dettagli.Imponibile_Netto ELSE 0 ")
            'Stb_SelectConti.AppendLine(" END AS Dare,    ")
            'Stb_SelectConti.AppendLine(" CASE WHEN RicXConti_Eco.Dare_Avere = 'A' THEN Movimenti_dettagli.Imponibile_Netto ELSE 0 ")
            'Stb_SelectConti.AppendLine(" END AS Avere,    ")

            Stb_SelectContiDareAvere.AppendLine(" CASE WHEN Movimenti_dettagli.Imponibile_Netto < 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectContiDareAvere.AppendLine(" THEN -1 * ROUND(Movimenti_dettagli.Imponibile_Netto, 2) ")
            Else
                Stb_SelectContiDareAvere.AppendLine(" THEN -1 * Movimenti_dettagli.Imponibile_Netto ")
            End If
            Stb_SelectContiDareAvere.AppendLine(" ELSE 0 ")
            Stb_SelectContiDareAvere.AppendLine(" END AS Dare,    ")

            Stb_SelectContiDareAvere.AppendLine(" CASE WHEN Movimenti_dettagli.Imponibile_Netto >= 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectContiDareAvere.AppendLine(" THEN ROUND(Movimenti_dettagli.Imponibile_Netto, 2) ")
            Else
                Stb_SelectContiDareAvere.AppendLine(" THEN Movimenti_dettagli.Imponibile_Netto ")
            End If
            Stb_SelectContiDareAvere.AppendLine(" ELSE 0 ")
            Stb_SelectContiDareAvere.AppendLine(" END AS Avere,    ")

            'Movimenti_dettagli.sconto_modalita IN (" & enModalitaSconto.Omaggio_SenzaRivalsaIva & " , " & enModalitaSconto.Omaggio_ConRivalsaIva & " ) ")

            Stb_SelectOmaggiDareAvere.AppendLine(" CASE WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_ConRivalsaIva & " AND Movimenti_dettagli.Imponibile_Netto >= 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN ROUND(Movimenti_dettagli.Imponibile_Netto, 2) ")
            Else
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN Movimenti_dettagli.Imponibile_Netto ")
            End If
            Stb_SelectOmaggiDareAvere.AppendLine("      WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_SenzaRivalsaIva & " AND Movimenti_dettagli.Imponibile_Netto >= 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN ROUND(Movimenti_dettagli.Imponibile_Netto, 2) + (ROUND(Movimenti_dettagli.Iva, 2) * -1)  ")
            Else
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN Movimenti_dettagli.Imponibile_Netto + (Movimenti_dettagli.Iva * -1)  ")
            End If
            Stb_SelectOmaggiDareAvere.AppendLine("      ELSE 0 ")
            Stb_SelectOmaggiDareAvere.AppendLine(" END AS Dare,    ")
            Stb_SelectOmaggiDareAvere.AppendLine(" CASE WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_ConRivalsaIva & " AND Movimenti_dettagli.Imponibile_Netto < 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN -1 * ROUND(Movimenti_dettagli.Imponibile_Netto, 2) ")
            Else
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN -1 * Movimenti_dettagli.Imponibile_Netto ")
            End If
            Stb_SelectOmaggiDareAvere.AppendLine("      WHEN Movimenti_dettagli.sconto_modalita = " & enModalitaSconto.Omaggio_SenzaRivalsaIva & " AND Movimenti_dettagli.Imponibile_Netto < 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN (-1 * ROUND(Movimenti_dettagli.Imponibile_Netto, 2)) + ROUND(Movimenti_dettagli.Iva, 2) ")
            Else
                Stb_SelectOmaggiDareAvere.AppendLine(" THEN (-1 * Movimenti_dettagli.Imponibile_Netto) + Movimenti_dettagli.Iva ")
            End If
            Stb_SelectOmaggiDareAvere.AppendLine("      ELSE 0 ")
            Stb_SelectOmaggiDareAvere.AppendLine(" END AS Avere,    ")


            'modificato il 23/07/2015
            'Stb_SelectIVAindetr.AppendLine("  ( -1 * Movimenti_dettagli.Iva_Indetraibile) AS Dare, 0 AS Avere, ")
            Stb_SelectIVAindetr.AppendLine(" CASE WHEN Movimenti_dettagli.Iva_Indetraibile < 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVAindetr.AppendLine(" THEN -1 * ROUND(Movimenti_dettagli.Iva_Indetraibile, 2) ")
            Else
                Stb_SelectIVAindetr.AppendLine(" THEN -1 * Movimenti_dettagli.Iva_Indetraibile ")
            End If
            Stb_SelectIVAindetr.AppendLine(" ELSE 0 ")
            Stb_SelectIVAindetr.AppendLine(" END AS Dare,    ")
            Stb_SelectIVAindetr.AppendLine(" CASE WHEN Movimenti_dettagli.Iva_Indetraibile >= 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVAindetr.AppendLine(" THEN ROUND(Movimenti_dettagli.Iva_Indetraibile, 2) ")
            Else
                Stb_SelectIVAindetr.AppendLine(" THEN Movimenti_dettagli.Iva_Indetraibile ")
            End If
            Stb_SelectIVAindetr.AppendLine(" ELSE 0 ")
            Stb_SelectIVAindetr.AppendLine(" END AS Avere,    ")

            'modificato il 23/07/2015
            'Stb_SelectIVAcompens.AppendLine(" 0 AS Dare, (Movimenti_dettagli.Iva_Indetraibile) AS Avere, ")
            Stb_SelectIVAcompens.AppendLine(" CASE WHEN Movimenti_dettagli.Iva_Indetraibile < 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVAcompens.AppendLine(" THEN -1 * ROUND(Movimenti_dettagli.Iva_Indetraibile, 2) ")
            Else
                Stb_SelectIVAcompens.AppendLine(" THEN -1 * Movimenti_dettagli.Iva_Indetraibile ")
            End If
            Stb_SelectIVAcompens.AppendLine(" ELSE 0 ")
            Stb_SelectIVAcompens.AppendLine(" END AS Dare,    ")
            Stb_SelectIVAcompens.AppendLine(" CASE WHEN Movimenti_dettagli.Iva_Indetraibile >= 0 ")
            If flagNuoviArrotondamenti Then
                Stb_SelectIVAcompens.AppendLine(" THEN ROUND(Movimenti_dettagli.Iva_Indetraibile, 2) ")
            Else
                Stb_SelectIVAcompens.AppendLine(" THEN Movimenti_dettagli.Iva_Indetraibile ")
            End If
            Stb_SelectIVAcompens.AppendLine(" ELSE 0 ")
            Stb_SelectIVAcompens.AppendLine(" END AS Avere,    ")

            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" CASE WHEN Movimenti_dettagli.Iva_Indetraibile < 0 ")
            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" THEN -1 * Movimenti_dettagli.Iva_Indetraibile ")
            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" ELSE 0 ")
            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" END AS Dare,    ")
            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" CASE WHEN Movimenti_dettagli.imponibile_netto >= 0 ")
            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" THEN Movimenti_dettagli.Iva_Indetraibile ")
            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" ELSE 0 ")
            'Stb_SelectOmaggioConRivalsaIva.AppendLine(" END AS Avere,    ")


            'JOIN CONTI
            Stb_JoinConti1.AppendLine(" FROM Conti Conti_Eco ")
            Stb_JoinConti1.AppendLine(" INNER JOIN RicXConti RicXConti_Eco ON Conti_Eco.Cod_Conto = RicXConti_Eco.Cod_Conto ")
            Stb_JoinConti1.AppendLine(" INNER JOIN Riclassificazioni Riclassificazioni_Eco ON Riclassificazioni_Eco.Ric_Cod = RicXConti_Eco.Ric_Cod AND Riclassificazioni_Eco.Piva = RicXConti_Eco.Piva ")
            Stb_JoinConti1.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Eco.Piva = Imprese_Ric.PIVA ")
            'JOIN MOVIMENTI DETTAGLI - RICXCONTI - CONTI 
            Stb_JoinConti2.AppendLine(" INNER JOIN  Movimenti_dettagli ON Movimenti_dettagli.PIVA = RicXConti_Eco.Piva AND Movimenti_dettagli.Anno = RicXConti_Eco.Anno AND Movimenti_dettagli.Ric_Cod = RicXConti_Eco.Ric_Cod ")
            Stb_JoinConti2.AppendLine(" AND Movimenti_dettagli.Cod_Conto = RicXConti_Eco.Cod_Conto ")
            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            Stb_JoinConti3.AppendLine(" INNER JOIN Movimenti Movimenti_Mag ")
            Stb_JoinConti3.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")
            'JOIN AGENDA - MOVIMENTI
            Stb_JoinConti3.AppendLine(" INNER JOIN Agenda ")
            Stb_JoinConti3.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")
            'JOIN AGENDA - OPERAZIONI
            Stb_JoinConti3.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN CONTI X IVA IN COMPENSAZIONE
            'sul join movimenti_dettagli toglie AND Movimenti_dettagli.Cod_Conto = RicXConti_Eco.Cod_Conto 
            'perché il conto impostato sul dettaglio non è quello che va visualizzato in questa movimentazione
            'JOIN MOVIMENTI DETTAGLI - RICXCONTI - CONTI 
            Stb_JoinConti2IVA.AppendLine(" INNER JOIN  Movimenti_dettagli ON Movimenti_dettagli.PIVA = RicXConti_Eco.Piva AND Movimenti_dettagli.Anno = RicXConti_Eco.Anno AND Movimenti_dettagli.Ric_Cod = RicXConti_Eco.Ric_Cod ")
            Stb_JoinConti2IVA.AppendLine(" -- CANCELLATO AND Movimenti_dettagli.Cod_Conto = RicXConti_Eco.Cod_Conto ")

            'WHERE CONTI
            Stb_WhereConti.AppendLine(" WHERE RicXConti_Eco.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")

            'non serve il filtro, tanto c'è il join tra Movimenti_Mag e movimenti_dettagli
            'Stb_WhereConti.AppendLine(" AND Movimenti_Mag.CAU_MOV IN ('" & CAU_CARICO & "', " &
            '                                                    "'" & CAU_SCARICO & "', " &
            '                                                    "'" & CAU_ABBUONI & "', " &
            '                                                    "'" & CAU_CONFERIMENTO & "', " &
            '                                                    "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
            '                                                    ") ")

            If Ric_Cod <> 0 Then
                Stb_WhereConti.AppendLine(" AND Riclassificazioni_Eco.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
            End If
            '24/03/2016
            If Anno <> 0 Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Eco.Anno = " & Agro_SQL_SaveNum(Anno) & "  ")
            End If
            If Id_Riclassificazione <> "" Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Eco.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'  ")
            End If
            If Dare_Avere <> "" Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Eco.Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'  ")
            End If
            If Imputabile <> 0 Then
                Stb_WhereConti.AppendLine(" AND RicXConti_Eco.Imputabile = " & Agro_SQL_SaveNum(Imputabile) & "  ")
            End If
            If Cod_Conto <> 0 Then
                Stb_WhereConti.AppendLine(" AND Conti_Eco.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            End If
            If Flag_UE <> CONTO_UE_NOFILTRO Then
                Stb_WhereConti.AppendLine(" AND Conti_Eco.Flag_UE = " & Agro_SQL_SaveNum(Flag_UE) & "  ")
            End If
            If Cod_Contatto <> "" Then
                Stb_WhereConti.AppendLine(" AND Conti_Eco.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'  ")
            End If
            '/************************************************************************

            '  Giulia, 28/10/2016 11.04.30: Se un conto è stato mappato su di un solo sezionale deve tirare su solo i movimenti su quel sezionale
            '       per le operazioni che hanno un intestazione
            Stb_WhereContiSezionaliContab.AppendLine(vbCrLf & " -- Se il conto è stato mappato su di un sezionale preciso (CONTAB) ")
            Stb_WhereContiSezionaliContab.AppendLine(" AND ( ")
            Stb_WhereContiSezionaliContab.AppendLine("  CASE WHEN RicXConti_Eco.Sezionale_Cod_Conto <> -1 ")
            Stb_WhereContiSezionaliContab.AppendLine("  THEN RicXConti_Eco.Sezionale_Cod_Conto ")
            Stb_WhereContiSezionaliContab.AppendLine("  ELSE Movimenti_Contab.Sezionale_Cod ")
            Stb_WhereContiSezionaliContab.AppendLine("  END = Movimenti_Contab.Sezionale_Cod ")
            Stb_WhereContiSezionaliContab.AppendLine("  ) ")

            '/************************************************************************

            '  Giulia, 27/12/2016 16.32.55: Se un conto è stato mappato su di un solo sezionale deve tirare su solo i movimenti su quel sezionale
            '       per le operazioni che hanno direttamente il movimento di magazzino
            Stb_WhereContiSezionaliMag.AppendLine(vbCrLf & " -- Se il conto è stato mappato su di un sezionale preciso (MAG)")
            Stb_WhereContiSezionaliMag.AppendLine(" AND ( ")
            Stb_WhereContiSezionaliMag.AppendLine("  CASE WHEN RicXConti_Eco.Sezionale_Cod_Conto <> -1 ")
            Stb_WhereContiSezionaliMag.AppendLine("  THEN RicXConti_Eco.Sezionale_Cod_Conto ")
            Stb_WhereContiSezionaliMag.AppendLine("  ELSE Movimenti_Mag.Sezionale_Cod ")
            Stb_WhereContiSezionaliMag.AppendLine("  END = Movimenti_Mag.Sezionale_Cod ")
            Stb_WhereContiSezionaliMag.AppendLine("  ) ")


            '/************************************************************************
            '/*************** SELECT query movimenti NO contabili  *******************
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Movimenti_Mag.Doc_Numero)) + Movimenti_Mag.Doc_Numero_Des AS Numero_Doc,   ")
            Stb_SelectNoContab.AppendLine(" CONVERT(varchar, Movimenti_Mag.Data_Movimento, 103) AS Data_Movimento, ")
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Progr_Protocollo, Movimenti_Mag.Progr_Registrazione, CONVERT(date, Movimenti_Mag.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Cod_RisUm,  Movimenti_Mag.Mov_Desc, ")
            Stb_SelectNoContab.AppendLine(" Movimenti_Mag.Num_Protocollo  ")
            '/************************************************************************

            '/************************************************************************
            '/*************** SELECT / JOIN query movimenti CONTABILI  *******************
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Movimenti_Contab.Doc_Numero)) + Movimenti_Contab.Doc_Numero_Des AS Numero_Doc,   ")
            Stb_SelectContab.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento, ")
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, CONVERT(date, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            Stb_SelectContab.AppendLine(" Movimenti_Contab.Num_Protocollo  ")
            'JOIN AGENDA - MOVIMENTI CONTAB
            Stb_JoinContab.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            Stb_JoinContab.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")
            '/************************************************************************


            '/************************************************************************
            '/*************** query SALDI INIZIALI *******************
            Stb_SaldiIniziali.AppendLine(" 0 AS Id_Agenda, 0 AS Lav_Cod, '' AS LAV_DES, 'Saldo Iniziale' AS des_lib,   ")
            Stb_SaldiIniziali.AppendLine(" Conti_Eco.Cod_Conto AS Cod_Conto_Eco, Conti_Eco.Piva AS piva_conti,   ")
            Stb_SaldiIniziali.AppendLine(" Conti_Eco.Conto_Descr AS Conto_Eco_Descr, Conti_Eco.Flag_UE, Conti_Eco.Cod_Contatto,  ")
            Stb_SaldiIniziali.AppendLine(" RicXConti_Eco.Piva, Imprese_Ric.rag_soc AS rag_soc,  ")
            Stb_SaldiIniziali.AppendLine(" RicXConti_Eco.Anno, RicXConti_Eco.Id_Riclassificazione, RicXConti_Eco.Dare_Avere,   ") '--RicXConti_Eco.Imputabile,
            If Flag_LeggiSaldiIniziali = 0 Then
                Stb_SaldiIniziali.AppendLine(" 0 as Saldo, ")
            Else
                Stb_SaldiIniziali.AppendLine(" RicXConti_Eco.Saldo, ")
            End If
            Stb_SaldiIniziali.AppendLine(" Riclassificazioni_Eco.Ric_Cod AS Ric_Cod_Eco, Riclassificazioni_Eco.Ric_Des Ric_Des_Eco,  ")
            Stb_SaldiIniziali.AppendLine(" 0 AS Imponibile_Netto,  ")
            Stb_SaldiIniziali.AppendLine(" 0 AS elem_cod, 0 AS pro_cod, 0 AS mat_cod, ")

            'SONO CONTI ECONOMICI: POSITIVO IN AVERE, NEGATIVO IN DARE
            If Flag_LeggiSaldiIniziali = 0 Then
                Stb_SaldiIniziali.AppendLine(" 0 as Dare , 0 as Avere, ")
            Else
                Stb_SaldiIniziali.AppendLine(" CASE WHEN RicXConti_Eco.Saldo_Iniziale < 0 THEN (-1 * RicXConti_Eco.Saldo_Iniziale) ELSE 0 ")
                Stb_SaldiIniziali.AppendLine(" END AS Dare,    ")
                Stb_SaldiIniziali.AppendLine(" CASE WHEN RicXConti_Eco.Saldo_Iniziale > 0 THEN RicXConti_Eco.Saldo_Iniziale ELSE 0 ")
                Stb_SaldiIniziali.AppendLine(" END AS Avere,    ")
            End If
            Stb_SaldiIniziali.AppendLine(" '' AS Numero_Doc, '01/01/1900' AS Data_Movimento, ")
            Stb_SaldiIniziali.AppendLine(" 0 AS Progr_Protocollo, 0 AS Progr_Registrazione, CONVERT(date, '01/01/1900') AS Data_Registrazione, ")
            Stb_SaldiIniziali.AppendLine(" 0 AS Cod_RisUm,  '' AS Mov_Desc, ")
            Stb_SaldiIniziali.AppendLine(" 0 AS Num_Protocollo  ")
            Stb_SaldiIniziali.AppendLine(" , '01/01/1900' AS Data_Order ")
            Stb_SaldiIniziali.AppendLine("")
            Stb_SaldiIniziali.AppendLine(" FROM Conti Conti_Eco ")
            Stb_SaldiIniziali.AppendLine(" INNER JOIN RicXConti RicXConti_Eco ON Conti_Eco.Cod_Conto = RicXConti_Eco.Cod_Conto ")
            Stb_SaldiIniziali.AppendLine(" INNER JOIN Riclassificazioni Riclassificazioni_Eco ON Riclassificazioni_Eco.Ric_Cod = RicXConti_Eco.Ric_Cod AND Riclassificazioni_Eco.Piva = RicXConti_Eco.Piva ")
            Stb_SaldiIniziali.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Eco.Piva = Imprese_Ric.PIVA ")
            '/************************************************************************



            Stb_SelectPagamentiCEDare1.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, 'Incasso ' + Agenda.des_lib AS des_lib,   ")
            Stb_SelectPagamentiCEAvere1.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, 'Pagamento ' + Agenda.des_lib AS des_lib,   ")
            Stb_SelectPagamentiCE1.AppendLine(" Conti_Eco.Cod_Conto AS Cod_Conto_Eco, Conti_Eco.Piva AS piva_conti,   ")
            Stb_SelectPagamentiCE1.AppendLine(" Conti_Eco.Conto_Descr AS Conto_Eco_Descr, Conti_Eco.Flag_UE, Conti_Eco.Cod_Contatto,  ")
            Stb_SelectPagamentiCE1.AppendLine(" RicXConti_Eco.Piva, Imprese_Ric.rag_soc AS rag_soc,  ")
            Stb_SelectPagamentiCE1.AppendLine(" RicXConti_Eco.Anno, RicXConti_Eco.Id_Riclassificazione, RicXConti_Eco.Dare_Avere, RicXConti_Eco.Saldo,  ") '--RicXConti_Eco.Imputabile,
            Stb_SelectPagamentiCE1.AppendLine(" Riclassificazioni_Eco.Ric_Cod AS Ric_Cod_Eco, Riclassificazioni_Eco.Ric_Des AS Ric_Des_Eco,  ")
            Stb_SelectPagamentiCE1.AppendLine(" 0 AS Imponibile_Netto,  ")
            Stb_SelectPagamentiCE1.AppendLine(" 0 AS elem_cod, 0 AS pro_cod, 0 AS mat_cod, ")

            Stb_SelectPagamentiCE2.AppendLine(" '' AS Numero_Doc,   ")
            Stb_SelectPagamentiCE2.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento,  ")
            Stb_SelectPagamentiCE2.AppendLine(" Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione + 1 AS Progr_Registrazione, CONVERT(date, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectPagamentiCE2.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            Stb_SelectPagamentiCE2.AppendLine(" Movimenti_Contab.Num_Protocollo  ")

            '/************************************************************************
            '/*************** query PARTITA DOPPIA *******************

            Stb_SelectPartDoppia1.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib,   ")
            Stb_SelectPartDoppia1.AppendLine(" Conti_Eco.Cod_Conto AS Cod_Conto_Eco, Conti_Eco.Piva AS piva_conti,   ")
            Stb_SelectPartDoppia1.AppendLine(" Conti_Eco.Conto_Descr AS Conto_Eco_Descr, Conti_Eco.Flag_UE, Conti_Eco.Cod_Contatto,  ")
            Stb_SelectPartDoppia1.AppendLine(" RicXConti_Eco.Piva, Imprese_Ric.rag_soc AS rag_soc,  ")
            Stb_SelectPartDoppia1.AppendLine(" RicXConti_Eco.Anno, RicXConti_Eco.Id_Riclassificazione, RicXConti_Eco.Dare_Avere, RicXConti_Eco.Saldo,  ") '--RicXConti_Eco.Imputabile,
            Stb_SelectPartDoppia1.AppendLine(" Riclassificazioni_Eco.Ric_Cod AS Ric_Cod_Eco, Riclassificazioni_Eco.Ric_Des AS Ric_Des_Eco,  ")
            Stb_SelectPartDoppia1.AppendLine(" 0 AS Imponibile_Netto,  ")
            Stb_SelectPartDoppia1.AppendLine(" 0 AS elem_cod, 0 AS pro_cod, 0 AS mat_cod, ")


            Stb_SelectPartDoppiaDARE.AppendLine(" SUM(Pagamenti.Importo) AS Dare, 0 AS Avere,    ")
            Stb_SelectPartDoppiaAVERE.AppendLine(" 0 AS Dare, SUM(Pagamenti.Importo) AS Avere,    ")

            Stb_SelectPartDoppia2.AppendLine(" '' AS Numero_Doc,   ")
            Stb_SelectPartDoppia2.AppendLine(" CONVERT(varchar, Movimenti_Contab.Data_Movimento, 103) AS Data_Movimento,  ")
            Stb_SelectPartDoppia2.AppendLine(" Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, CONVERT(date, Movimenti_Contab.Data_Registrazione, 103) AS Data_Registrazione, ")
            Stb_SelectPartDoppia2.AppendLine(" Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, ")
            Stb_SelectPartDoppia2.AppendLine(" Movimenti_Contab.Num_Protocollo  ")

            Stb_joinPartDoppia1.AppendLine(" FROM Agenda ")
            Stb_joinPartDoppia1.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            Stb_joinPartDoppia1.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            Stb_joinPartDoppia1.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")
            Stb_joinPartDoppia1.AppendLine(" INNER JOIN Pagamenti ")
            Stb_joinPartDoppia1.AppendLine(" ON Pagamenti.PIVA = Movimenti_Contab.PIVA AND Pagamenti.Sa_Cod = Movimenti_Contab.Sa_Cod AND Pagamenti.Id_Agenda = Movimenti_Contab.Id_Agenda  AND Pagamenti.Id_MOV = Movimenti_Contab.Id_MOV  ")

            Stb_joinPartDoppiaDARE.AppendLine(" INNER JOIN RicXConti RicXConti_Eco ON Pagamenti.PIVA = RicXConti_Eco.Piva AND Pagamenti.Anno = RicXConti_Eco.Anno AND Pagamenti.Ric_Cod = RicXConti_Eco.Ric_Cod  AND Pagamenti.Cod_Conto_Dare = RicXConti_Eco.Cod_Conto ")
            Stb_joinPartDoppiaAVERE.AppendLine(" INNER JOIN RicXConti RicXConti_Eco ON Pagamenti.PIVA = RicXConti_Eco.Piva AND Pagamenti.Anno = RicXConti_Eco.Anno AND Pagamenti.Ric_Cod = RicXConti_Eco.Ric_Cod  AND Pagamenti.Cod_Conto_avere = RicXConti_Eco.Cod_Conto ")

            Stb_joinPartDoppia2.AppendLine(" INNER JOIN Conti Conti_Eco ON Conti_Eco.Cod_Conto = RicXConti_Eco.Cod_Conto ")
            Stb_joinPartDoppia2.AppendLine(" INNER JOIN Riclassificazioni Riclassificazioni_Eco ON Riclassificazioni_Eco.Ric_Cod = RicXConti_Eco.Ric_Cod AND Riclassificazioni_Eco.Piva = RicXConti_Eco.Piva ")
            Stb_joinPartDoppia2.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Eco.Piva = Imprese_Ric.PIVA ")

            Stb_GroupByPartDoppia.AppendLine("  GROUP BY Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib,  ")
            Stb_GroupByPartDoppia.AppendLine("  Conti_Eco.Cod_Conto, Conti_Eco.Piva, Conti_Eco.Conto_Descr, Conti_Eco.Flag_UE, Conti_Eco.Cod_Contatto,   ")
            Stb_GroupByPartDoppia.AppendLine("  RicXConti_Eco.Piva, Imprese_Ric.rag_soc,  ")
            Stb_GroupByPartDoppia.AppendLine("  RicXConti_Eco.Anno, RicXConti_Eco.Id_Riclassificazione, RicXConti_Eco.Dare_Avere, RicXConti_Eco.Saldo,   ")
            Stb_GroupByPartDoppia.AppendLine("  Riclassificazioni_Eco.Ric_Cod, Riclassificazioni_Eco.Ric_Des, ")
            Stb_GroupByPartDoppia.AppendLine("  Movimenti_Contab.Data_Movimento, ")
            Stb_GroupByPartDoppia.AppendLine("  Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione,  ")
            Stb_GroupByPartDoppia.AppendLine("  Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Num_Protocollo  ")


            '/************************************************************************


            ''///////////////////////////////////////////////
            'Stb_Globale.AppendLine(" SELECT * ")
            'Stb_Globale.AppendLine(" FROM ")

            'Stb_Globale.AppendLine(" ( ")


            'If Flag_LeggiSaldiIniziali = True Then
            '/************************************************************************************
            '/***** 0 PARTE: SALDI INIZIALI  ***************
            '/************************************************************************************
            Stb_Globale.AppendLine(" -- 0 PARTE: SALDI INIZIALI ")
            Stb_Globale.AppendLine(" ( ")

            'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
            Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
            Stb_Globale.AppendLine(Stb_SaldiIniziali.ToString)

            Stb_Globale.AppendLine(Stb_WhereConti.ToString)
            '18/03/2016: i saldi iniziali dei conti salvati in rixconti li leggo solo nell'anno di inizio gestione contabile
            'Stb_Globale.AppendLine(" AND RicXConti_Eco.Anno = " & Agro_SQL_SaveNum(Anno_GestCont_DataInizio))
            Stb_Globale.AppendLine(" AND RicXConti_Eco.Anno = " & Agro_SQL_SaveNum(Anno_Fine))

            'se non voglio i conti con saldo = 0
            If Not Flag_ContiSaldo0 Then
                Stb_Globale.AppendLine(" AND RicXConti_Eco.Saldo_Iniziale <> 0   ")
            End If

            Stb_Globale.AppendLine(" ) ")

            FlagServeUnion = True

            'End If

            If Not Flag_SOLOSaldiIniziali Then

                '/************************************************************************************
                '/***** 1° PARTE: DOC CONTABILI (ACQUISTI) FILTRATI X DATA REGISTRAZIONE  ***************
                '/************************************************************************************

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 1 PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")
                Stb_Globale.AppendLine(" ( ")

                'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                Stb_Globale.AppendLine(Stb_SelectContiDareAvere.ToString)
                Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Registrazione AS Data_Order ")

                Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")
                If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                End If
                If Cod_RisUm <> 0 Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                End If

                'leggi nota sopra, sulla data fine
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                         CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                         CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                         CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                         CStr(LAVCOD_ALTRI_COSTI) &
                                                         " ) ")

                Stb_Globale.AppendLine(" ) ")
                Stb_Globale.AppendLine(" -- FINE 1 PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")

                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" UNION ALL ")


                '/************************************************************************************
                '/***** 2° PARTE: DOC CONTABILI (VENDITE) FILTRATI X DATA MOVIMENTO  ***************
                '/************************************************************************************
                Stb_Globale.AppendLine(" -- 2 PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO ")
                Stb_Globale.AppendLine(" ( ")

                'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                Stb_Globale.AppendLine(Stb_SelectContiDareAvere.ToString)
                Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti3.ToString)
                Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")
                If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                End If
                If Cod_RisUm <> 0 Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                End If

                'leggi nota sopra, sulla data fine
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                            CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                            CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                            CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                            CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                            CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                            CStr(LAVCOD_VENDITA) & ", " &
                                                            CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                            CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                            CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                            " ) ")

                Stb_Globale.AppendLine(" ) ")
                Stb_Globale.AppendLine(" -- FINE 2 PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO ")

                FlagServeUnion = True

                'in caso di filtro contatto (cod_risum <>0)
                'scarto gli acquisti e l'autoconsumo perché non hanno il contatto
                'scarto op. di partita doppia 
                'perché sui conti economici non c'è il legame col contatto (come invece crediti/debiti patrimoniali)
                If Cod_RisUm = 0 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 3° PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 3 PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContiDareAvere.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    ''nell'acquisto non c'è il contatto, quindi gli acquisti vengono esclusi
                    'If Cod_RisUm <> 0 Then
                    '    Stb_Globale.AppendLine(" AND Movimenti_Mag.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    'End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    Stb_Globale.AppendLine(" AND    Agenda.lav_cod = " & CStr(LAVCOD_ACQUISTO))

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 3 PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE ")

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")

                    '/************************************************************************************
                    '/***** 4° PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 4 PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContiDareAvere.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    ''nell'autoconsumo non c'è il contatto, quindi gli acquisti vengono esclusi
                    'If Cod_RisUm <> 0 Then
                    '    Stb_Globale.AppendLine(" AND Movimenti_Mag.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    'End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND   Movimenti_Mag.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod IN ( " & CStr(LAVCOD_AUTOCONSUMO) & "," & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ")")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 4 PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ")


                    'leggo op. di partita doppia solo se non ho impostato il filtro contatto
                    'perché sui conti economici non c'è il legame col contatto (come invece crediti/debiti patrimoniali)
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")

                    '/************************************************************************************
                    '/***** 5° PARTE: PARTITA DOPPIA - CONTI AVERE  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 5° PARTE: PARTITA DOPPIA - CONTI AVERE  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    'MODIFICA DEL 21/03/2016: riattivato il filtro iniziale sul solo lav_cod = 1032
                    'modifica dell'11/02/2016: correzione bug nato con la modifica del 07/12/2015:
                    'le movimentazioni in p.d. collegate a fatture senza contabilizzazione automatica (ovvero con chkcoge_manuale = 1), hanno chkcoge_manuale =3 e non era stato gestito
                    'modifica del 07/12/2015:
                    'nei pagamenti gestita anche la possibilità di movimentare conti economici attivi e passivi
                    'sostituisco il filtro fisso su lav_cod 1032
                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))
                    'Stb_Globale.AppendLine("   AND (  ")
                    'Stb_Globale.AppendLine("        ( Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Agenda.chkcoge_manuale IN ( " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & ", " & CStr(enum_ChkCoGe.CoGe_MANUALE_CONTABILIZZAZIONE) & ") ) ")
                    'Stb_Globale.AppendLine("        OR ")
                    'Stb_Globale.AppendLine("        (	Agenda.lav_cod <> " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Pagamenti.Previsto_Avvenuto=1 AND Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")
                    'Stb_Globale.AppendLine("        ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 5° PARTE: PARTITA DOPPIA - CONTI AVERE  ")

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")

                    '/************************************************************************************
                    '/***** 5B° PARTE: PAGAMENTO FATTURA - MOVIMENTAZIONE C. ECONOMICI IN AVERE ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 5B° PARTE: PAGAMENTO FATTURA - MOVIMENTAZIONE C. ECONOMICI IN AVERE ")
                    Stb_Globale.AppendLine(" ( ")

                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagamentiCEAvere1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamentiCE1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamentiCE2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaAVERE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine(" AND (	Agenda.lav_cod <> " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Pagamenti.Previsto_Avvenuto=1 AND Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")
                    Stb_Globale.AppendLine(" AND (	Agenda.lav_cod <> " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Pagamenti.Previsto_Avvenuto=1 AND Pagamenti.ChkCoge_Manuale_Pagamenti = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 5B° PARTE: PAGAMENTO FATTURA - MOVIMENTAZIONE C. ECONOMICI IN AVERE ")

                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")

                    '/************************************************************************************
                    '/***** 6° PARTE: PARTITA DOPPIA - CONTI DARE  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 6° PARTE: PARTITA DOPPIA - CONTI DARE  ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppia2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    'modifica del 20/08/2015: 
                    'leggi nota sopra, sulla data fine
                    'Stb_Globale.AppendLine(" AND   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    'MODIFICA DEL 21/03/2016: riattivato il filtro iniziale sul solo lav_cod = 1032
                    'modifica dell'11/02/2016: correzione bug nato con la modifica del 07/12/2015:
                    'le movimentazioni in p.d. collegate a fatture senza contabilizzazione automatica (ovvero con chkcoge_manuale = 1), hanno chkcoge_manuale =3 e non era stato gestito
                    'modifica del 07/12/2015:
                    'nei pagamenti gestita anche la possibilità di movimentare conti economici attivi e passivi
                    'sostituisco il filtro fisso su lav_cod 1032
                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO))
                    'Stb_Globale.AppendLine("   AND (  ")
                    'Stb_Globale.AppendLine("        ( Agenda.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Agenda.chkcoge_manuale IN ( " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & ", " & CStr(enum_ChkCoGe.CoGe_MANUALE_CONTABILIZZAZIONE) & ") ) ")
                    'Stb_Globale.AppendLine("        OR ")
                    'Stb_Globale.AppendLine("        (	Agenda.lav_cod <> " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Pagamenti.Previsto_Avvenuto=1 AND Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")
                    'Stb_Globale.AppendLine("        ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 6° PARTE: PARTITA DOPPIA - CONTI DARE  ")


                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" UNION ALL ")

                    '/************************************************************************************
                    '/***** 6b° PARTE: INCASSO FATTURA - MOVIMENTAZIONE C. ECONOMICI IN DARE  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 6b° PARTE: INCASSO FATTURA - MOVIMENTAZIONE C. ECONOMICI IN DARE  ")
                    Stb_Globale.AppendLine(" ( ")

                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectPagamentiCEDare1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamentiCE1.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_SelectPagamentiCE2.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_joinPartDoppia1.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppiaDARE.ToString)
                    Stb_Globale.AppendLine(Stb_joinPartDoppia2.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    '§§§12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
                    'Stb_Globale.AppendLine("   AND (	Agenda.lav_cod <> " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Pagamenti.Previsto_Avvenuto=1 AND Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")
                    Stb_Globale.AppendLine("   AND (	Agenda.lav_cod <> " & CStr(LAVCOD_MOV_FINANZIARIO) & " AND Pagamenti.Previsto_Avvenuto=1 AND Pagamenti.ChkCoge_Manuale_Pagamenti  = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If

                    Stb_Globale.AppendLine(Stb_GroupByPartDoppia.ToString)

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 6b° PARTE: INCASSO FATTURA - MOVIMENTAZIONE C. ECONOMICI IN DARE ")


                    FlagServeUnion = True

                    '/**********************************************************************************
                End If 'filtro sul cod_risum

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                End If

                '/************************************************************************************
                '/***** 7° PARTE: SCHEDA COMPENSI FILTRATI X DATA MOVIMENTO  ***************
                '/************************************************************************************
                Stb_Globale.AppendLine(" -- 7 PARTE: SCHEDA COMPENSI FILTRATI X DATA MOVIMENTO ")
                Stb_Globale.AppendLine(" ( ")

                'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                Stb_Globale.AppendLine(Stb_SelectContiDareAvere.ToString)
                Stb_Globale.AppendLine(Stb_SelectContab.ToString)

                Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_COMPENSI) & "'   ")

                'leggi nota sopra, sulla data fine
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                Stb_Globale.AppendLine(" AND Agenda.Lav_Cod = " & CStr(LAVCOD_REG_COMPENSI) & "  ")

                If Cod_RisUm <> 0 Then
                    'i contatti della scheda compensi sono salvati in movimenti_dettagli
                    Stb_Globale.AppendLine(" AND EXISTS (   ")
                    Stb_Globale.AppendLine("   SELECT 1 ")
                    Stb_Globale.AppendLine("   FROM Movimenti_dettagli ")
                    Stb_Globale.AppendLine("   WHERE Movimenti_dettagli.Piva = Agenda.Piva ")
                    Stb_Globale.AppendLine("   AND Movimenti_dettagli.Id_Agenda = Agenda.Id_Agenda ")
                    Stb_Globale.AppendLine("   AND Movimenti_dettagli.elem_Cod = 0 ")
                    Stb_Globale.AppendLine(" AND Movimenti_dettagli.Mat_cod = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    Stb_Globale.AppendLine("  )  ")
                End If

                Stb_Globale.AppendLine(" ) ")
                Stb_Globale.AppendLine(" -- FINE 7° PARTE: SCHEDA COMPENSI FILTRATI X DATA MOVIMENTO ")

                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ")


                '/************************************************************************************
                '/***** 8° PARTE: IVA INDETRAIBILE (COSTO) - DOC CONTABILI FILTRATI X DATA REGISTRAZIONE  ***************
                '/************************************************************************************

                'l'iva indetraibile diventa un costo
                'in tutti i report di contabilità viene dirottata sul conto economico selezionato nel dettaglio
                'in data 28/02/2017 martedì grasso è stato messo questo flag per richiesta delle Zanasi:
                'poter escludere l'iva indetraibile dal conteggio dei movimenti/saldi del conto economico 
                '(per ottenere un'informazione da mandare al commercialista)
                If Not FlagEscludiIvaIndetraibile Then

                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")

                    Stb_Globale.AppendLine(" -- 8° PARTE: IVA INDETRAIBILE (COSTO) - DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                    Stb_Globale.AppendLine(Stb_SelectIVAindetr.ToString)
                    Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.iva_indetraibile_perc <> 0  ")

                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    If Cod_RisUm <> 0 Then
                        Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                             CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                             CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                             CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                             CStr(LAVCOD_ALTRI_COSTI) &
                                                             " ) ")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- FINE 8° PARTE: IVA INDETRAIBILE (COSTO) - DOC CONTABILI FILTRATI X DATA REGISTRAZIONE ")

                    FlagServeUnion = True

                End If 'FlagEscludiIvaIndetraibile

                'in caso di filtro contatto (cod_risum <>0)
                'scarto gli acquisti perché non hanno il contatto
                If Cod_RisUm = 0 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 9° PARTE: IVA INDETRAIBILE (COSTO) - ACQUISTO FILTRATO X DATA REGISTRAZIONE  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 9 PARTE: IVA INDETRAIBILE (COSTO) - ACQUISTO FILTRATO X DATA REGISTRAZIONE ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                    Stb_Globale.AppendLine(Stb_SelectIVAindetr.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Registrazione AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.iva_indetraibile_perc <> 0  ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    ''nell'acquisto non c'è il contatto, quindi gli acquisti vengono esclusi
                    'If Cod_RisUm <> 0 Then
                    '    Stb_Globale.AppendLine(" AND Movimenti_Mag.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    'End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND    Movimenti_Mag.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    Stb_Globale.AppendLine(" AND    Agenda.lav_cod = " & CStr(LAVCOD_ACQUISTO))

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 9° PARTE:  IVA INDETRAIBILE (COSTO) - ACQUISTO FILTRATO X DATA REGISTRAZIONE ")

                    FlagServeUnion = True

                End If

                '/************************************************************************************
                '/***** 10° PARTE: IVA COMPENSAZIONE (RICAVO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO  ***************
                '/************************************************************************************

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 10° PARTE: IVA COMPENSAZIONE (RICAVO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO ")
                Stb_Globale.AppendLine(" ( ")

                'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                Stb_Globale.AppendLine(Stb_SelectIVAcompens.ToString)
                Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti2IVA.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                Stb_Globale.AppendLine(" AND Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                Stb_Globale.AppendLine(" AND Movimenti_dettagli.iva_indetraibile_perc <> 0  ")
                Stb_Globale.AppendLine(" AND Movimenti_dettagli.Cod_Iva NOT IN ( " &
                                                                            CStr(Art4DL331) & ", " &
                                                                            CStr(NonImpArt8DPR633_72) & ", " &
                                                                            CStr(NonImpArt8) & ", " &
                                                                            CStr(NonImpArt8C1LetB) & ", " &
                                                                            CStr(NonImpArt8C1LetC) &
                                                                            ")  ")

                Stb_Globale.AppendLine(" AND    RicXConti_Eco.Codifica_conto = " & Agro_SQL_SaveNum(enum_Conti_Economici.RicavixIVAincompensazione) & "   ")

                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                End If
                If Cod_RisUm <> 0 Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                End If

                'leggi nota sopra, sulla data fine
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                        CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                        CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                        CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                        CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                        CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                        CStr(LAVCOD_VENDITA) & ", " &
                                                        CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                        CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                        CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                        " ) ")

                Stb_Globale.AppendLine(" ) ")
                Stb_Globale.AppendLine(" -- FINE 10° PARTE: IVA COMPENSAZIONE (RICAVO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO ")

                FlagServeUnion = True


                'in caso di filtro contatto (cod_risum <>0)
                'scarto gli autoconsumi perché non hanno il contatto
                If Cod_RisUm = 0 Then

                    If FlagServeUnion Then
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine(" UNION ALL ")
                        Stb_Globale.AppendLine("  ")
                        Stb_Globale.AppendLine("  ")
                    End If

                    '/************************************************************************************
                    '/***** 11° PARTE: IVA COMPENSAZIONE (RICAVO) - AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ***************
                    '/************************************************************************************
                    Stb_Globale.AppendLine(" -- 11° PARTE: IVA COMPENSAZIONE (RICAVO) - AUTOCONSUMO FILTRATO X DATA MOVIMENTO ")
                    Stb_Globale.AppendLine(" ( ")

                    'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                    Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                    Stb_Globale.AppendLine(Stb_SelectIVAcompens.ToString)
                    Stb_Globale.AppendLine(Stb_SelectNoContab.ToString)
                    Stb_Globale.AppendLine(" , Movimenti_Mag.Data_Movimento AS Data_Order ")

                    Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti2IVA.ToString)
                    Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                    Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                    Stb_Globale.AppendLine(Stb_WhereContiSezionaliMag.ToString)

                    Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")
                    Stb_Globale.AppendLine(" AND    Movimenti_dettagli.iva_indetraibile_perc <> 0  ")

                    Stb_Globale.AppendLine(" AND    RicXConti_Eco.Codifica_conto = " & Agro_SQL_SaveNum(enum_Conti_Economici.RicavixIVAincompensazione) & "   ")

                    If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                        Stb_Globale.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                    End If
                    ''nell'autoconsumo non c'è il contatto
                    'If Cod_RisUm <> 0 Then
                    '    Stb_Globale.AppendLine(" AND Movimenti_Mag.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    'End If

                    'leggi nota sopra, sulla data fine
                    Stb_Globale.AppendLine(" AND   Movimenti_Mag.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                    Stb_Globale.AppendLine(" AND   Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                    Stb_Globale.AppendLine(" AND Agenda.lav_cod IN ( " & CStr(LAVCOD_AUTOCONSUMO) & "," & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ")")

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" -- 11° PARTE: IVA COMPENSAZIONE (RICAVO) - AUTOCONSUMO FILTRATO X DATA MOVIMENTO ")

                    FlagServeUnion = True

                End If

                '/************************************************************************************
                '/***** 12° PARTE: OMAGGI ALLA CLIENTELA (COSTO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO  ***************
                '/************************************************************************************

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 12° PARTE: OMAGGI ALLA CLIENTELA (COSTO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO ")
                Stb_Globale.AppendLine(" ( ")

                'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                Stb_Globale.AppendLine(Stb_SelectOmaggiDareAvere.ToString)
                Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti2IVA.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                Stb_Globale.AppendLine(" AND    Movimenti_dettagli.sconto_modalita IN (" & enModalitaSconto.Omaggio_SenzaRivalsaIva & " , " & enModalitaSconto.Omaggio_ConRivalsaIva & " ) ")
                Stb_Globale.AppendLine(" AND    RicXConti_Eco.Codifica_conto = " & Agro_SQL_SaveNum(enum_Conti_Economici.OmaggiAllaClientela) & "   ")

                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                End If
                If Cod_RisUm <> 0 Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                End If

                'leggi nota sopra, sulla data fine
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                        CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                        CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                        CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                        CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                        CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                        CStr(LAVCOD_VENDITA) & ", " &
                                                        CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                        CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                        CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                        " ) ")

                Stb_Globale.AppendLine(" ) ")
                Stb_Globale.AppendLine(" -- 12° PARTE: OMAGGI ALLA CLIENTELA (COSTO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO ")

                FlagServeUnion = True


                '/************************************************************************************
                '/***** 13° PARTE: IVA COMPENSAZIONE ESTERO (RICAVO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO  ***************
                '/************************************************************************************

                If FlagServeUnion Then
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine(" UNION ALL ")
                    Stb_Globale.AppendLine("  ")
                    Stb_Globale.AppendLine("  ")
                End If

                Stb_Globale.AppendLine(" -- 13° PARTE: IVA COMPENSAZIONE ESTERO (RICAVO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO ")
                Stb_Globale.AppendLine(" ( ")

                'Stb_Globale.AppendLine(" SELECT CONVERT(varchar(1000), Conti_Eco.Cod_Conto) AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(" SELECT 'CE_' + Id_Riclassificazione AS CodiceSplitGruppo, ")
                Stb_Globale.AppendLine(Stb_SelectConti.ToString)
                Stb_Globale.AppendLine(Stb_SelectIVAcompens.ToString)
                Stb_Globale.AppendLine(Stb_SelectContab.ToString)
                Stb_Globale.AppendLine(" , Movimenti_Contab.Data_Movimento AS Data_Order ")

                Stb_Globale.AppendLine(Stb_JoinConti1.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti2IVA.ToString)
                Stb_Globale.AppendLine(Stb_JoinConti3.ToString)

                Stb_Globale.AppendLine(Stb_JoinContab.ToString)

                Stb_Globale.AppendLine(Stb_WhereConti.ToString)
                Stb_Globale.AppendLine(Stb_WhereContiSezionaliContab.ToString)

                Stb_Globale.AppendLine(" AND    Agenda.chkcoge_manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "   ")

                Stb_Globale.AppendLine(" AND    Movimenti_dettagli.iva_indetraibile_perc <> 0  ")
                Stb_Globale.AppendLine(" AND    Movimenti_dettagli.Cod_Iva IN ( " &
                                                                            CStr(Art4DL331) & ", " &
                                                                            CStr(NonImpArt8DPR633_72) & ", " &
                                                                            CStr(NonImpArt8) & ", " &
                                                                            CStr(NonImpArt8C1LetB) & ", " &
                                                                            CStr(NonImpArt8C1LetC) &
                                                                            ")  ")

                Stb_Globale.AppendLine(" AND    RicXConti_Eco.Codifica_conto = " & Agro_SQL_SaveNum(enum_Conti_Economici.RicavixIVAincompensazioneEstero) & "   ")

                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

                If Sezionale_Cod <> SEZIONALE_NOFILTRO Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
                End If
                If Cod_RisUm <> 0 Then
                    Stb_Globale.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                End If

                'leggi nota sopra, sulla data fine
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
                Stb_Globale.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                Stb_Globale.AppendLine(" AND Agenda.Lav_Cod IN ( " &
                                                        CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                        CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                        CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                        CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                        CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                        CStr(LAVCOD_VENDITA) & ", " &
                                                        CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                        CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                        CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                        " ) ")

                Stb_Globale.AppendLine(" ) ")
                Stb_Globale.AppendLine(" -- FINE 13° PARTE: IVA COMPENSAZIONE ESTERO (RICAVO) - DOC CONTABILI FILTRATI X DATA MOVIMENTO ")

                FlagServeUnion = True

            End If 'Flag_SOLOSaldiIniziali


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Stb_Globale.Length = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        '   Return DT

        Return Stb_Globale

    End Function


    Public Function PianoContiEconomici_LeggiDS(ByRef dataSet2Fill As DataSet,
                                                ByVal strNomeDtNelDs As String,
                                                ByVal pivaRicl As String,
                                                ByVal anno As Integer,
                                                ByVal idRiclassificazione As String,
                                                ByVal dareAvere As String,
                                                ByVal saldo As Decimal,
                                                ByVal imputabile As Integer,
                                                ByVal ricCod As Integer,
                                                ByVal pivaConti As String,
                                                ByVal codConto As Integer,
                                                ByVal flagUe As Integer,
                                                ByVal codContatto As String,
                                                ByVal filtroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.PianoContiEconomici_LeggiDS"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim risp As Boolean = False

        Try

            stbQ.AppendLine(" SELECT Conti.Cod_Conto, Conti.Piva AS piva_conti, Imprese_Conti.rag_soc AS rag_soc_conti, Conti.Conto_Descr, Conti.Flag_UE, Conti.Cod_Contatto,  ")
            stbQ.AppendLine(" Conti.validita_inizio AS inizio_conti, Conti.validita_fine AS fine_conti, Conti.username_creazione AS username_conti, RicXConti.Piva AS piva_ric, ")
            stbQ.AppendLine(" Imprese_Ric.rag_soc AS rag_soc_ric, RicXConti.Anno, RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo, RicXConti.Imputabile, ")
            stbQ.AppendLine(" Riclassificazioni.Ric_Cod, Riclassificazioni.Ric_Des, RicXConti.validita_inizio AS inizio_ric, RicXConti.validita_fine AS fine_ric,  RicXConti.username_creazione AS username_ric ")

            stbQ.AppendLine(" FROM Conti ")
            stbQ.AppendLine(" LEFT OUTER JOIN  Imprese Imprese_Conti ON Imprese_Conti.PIVA = Conti.Piva ")
            stbQ.AppendLine(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva ")
            stbQ.AppendLine(" LEFT OUTER JOIN  Imprese Imprese_Ric ON RicXConti.Piva = Imprese_Ric.PIVA ")
            stbQ.AppendLine(" WHERE 1 = 1 ")

            If pivaRicl <> "" Then
                stbQ.AppendLine(" AND RicXConti.Piva = '" & Agro_SQL_SaveText(pivaRicl) & "'  ")
            End If

            If anno <> 0 Then
                stbQ.AppendLine(" AND RicXConti.Anno = " & Agro_SQL_SaveNum(anno) & "  ")
            End If

            If idRiclassificazione <> "" Then
                stbQ.AppendLine(" AND RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(idRiclassificazione) & "'  ")
            End If

            If dareAvere <> "" Then
                stbQ.AppendLine(" AND RicXConti.Dare_Avere = '" & Agro_SQL_SaveText(dareAvere) & "' ")
            End If

            If saldo <> 0 Then
                stbQ.AppendLine(" AND RicXConti.Saldo = " & Agro_SQL_SaveNum(saldo) & "  ")
            End If

            If imputabile <> 0 Then
                stbQ.AppendLine(" AND RicXConti.Imputabile = " & Agro_SQL_SaveNum(imputabile) & "  ")
            End If

            If ricCod <> 0 Then
                stbQ.AppendLine(" AND Riclassificazioni.Ric_Cod = " & Agro_SQL_SaveNum(ricCod) & "  ")
            End If

            If pivaConti <> "" Then
                stbQ.AppendLine(" AND Conti.Piva = '" & Agro_SQL_SaveText(pivaConti) & "'  ")
            End If

            If codConto <> 0 Then
                stbQ.AppendLine(" AND Conti.Cod_Conto = " & Agro_SQL_SaveNum(codConto) & "  ")
            End If

            If flagUe <> CONTO_UE_NOFILTRO Then
                stbQ.AppendLine(" AND Conti.Flag_UE = " & Agro_SQL_SaveNum(flagUe) & "  ")
            End If

            If codContatto <> "" Then
                stbQ.AppendLine(" AND Conti.Cod_Contatto = '" & Agro_SQL_SaveText(codContatto) & "'  ")
            End If

            If FiltroAggiuntivo <> "" Then
                stbQ.AppendLine(FiltroAggiuntivo)
            End If

            stbQ.AppendLine(" ORDER BY RicXConti.Anno DESC, RicXConti.Id_Riclassificazione  ")


            '--------------------------------------------------------------------------
            risp = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine, dataSet2Fill, strNomeDtNelDs)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return risp

    End Function


    Public Function PianoContiEconomici_Confronto_LeggiDS(ByRef DataSet2Fill As DataSet,
                                                          ByVal strNomeDtNelDS As String,
                                                          ByVal Piva_Ricl As String,
                                                          ByVal Anno As Integer,
                                                          ByVal AnnoConfronto As Integer,
                                                          ByVal FiltroAggiuntivo1 As String,
                                                          ByVal FiltroAggiuntivo2 As String,
                                                          ByVal FiltroAggiuntivo3 As String,
                                                          ByVal Ordinamento As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.PianoContiEconomici_Confronto_LeggiDS"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim risp As Boolean = False

        Try

            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  RicXConti_Anno1.Piva, RicXConti_Anno1.Ric_Cod, RicXConti_Anno1.Cod_Conto,  ")
            stbQ.AppendLine(" (CASE  ")
            stbQ.AppendLine("     WHEN RicXConti_Anno1.Id_Riclassificazione = '' THEN RicXConti_Anno2.Id_Riclassificazione ")
            stbQ.AppendLine("     ELSE RicXConti_Anno1.Id_Riclassificazione ")
            stbQ.AppendLine(" END ")
            stbQ.AppendLine(" ) AS Id_Ricl_Conto, ")
            stbQ.AppendLine(" Conti.Conto_Descr, Conti.Cod_Contatto, Conti.Flag_UE,  ")
            stbQ.AppendLine(" RicXConti_Anno1.Anno AS Anno_1, RicXConti_Anno1.Id_Riclassificazione AS Id_Riclassificazione_1, RicXConti_Anno1.Dare_Avere AS Dare_Avere_1,  ")
            stbQ.AppendLine(" RicXConti_Anno1.Saldo AS Saldo_1, RicXConti_Anno1.Imputabile AS Imputabile_1, RicXConti_Anno2.Anno AS Anno_2, ")
            stbQ.AppendLine(" RicXConti_Anno2.Id_Riclassificazione AS Id_Riclassificazione_2, RicXConti_Anno2.Dare_Avere AS Dare_Avere_2, RicXConti_Anno2.Saldo AS Saldo_2, RicXConti_Anno2.Imputabile AS Imputabile_2, ")

            stbQ.AppendLine(" '' AS Differenza_Perc_Analitico, '' AS Differenza_Analitico,  ")
            stbQ.AppendLine(" '' AS Differenza_Perc_SaldoTotale, '' AS Differenza_SaldoTotale,  ")
            stbQ.AppendLine(" '' AS TotaleGruppo,   ")
            stbQ.AppendLine(" '' AS PesoAnalitico_TotaleGruppo, '' AS PesoAnalitico_Perc_TotaleGruppo,  ")
            stbQ.AppendLine(" '' AS PesoTotale_TotaleGruppo, '' AS PesoTotale_Perc_TotaleGruppo,  ")
            stbQ.AppendLine(" '' AS SaldoTotale_1, '' AS SaldoTotale_2, ")
            stbQ.AppendLine(" '' AS Extra_Str1, '' AS Extra_Str2, '' AS Extra_Str3, '' AS Extra_Str4 ")

            stbQ.AppendLine(" FROM    RicXConti RicXConti_Anno2  ")
            stbQ.AppendLine(" INNER JOIN RicXConti RicXConti_Anno1 ON RicXConti_Anno2.Piva = RicXConti_Anno1.Piva AND RicXConti_Anno2.Ric_Cod = RicXConti_Anno1.Ric_Cod ")
            stbQ.AppendLine(" AND RicXConti_Anno2.Cod_Conto = RicXConti_Anno1.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Conti ON RicXConti_Anno1.Cod_Conto = Conti.Cod_Conto ")

            stbQ.AppendLine(" WHERE     (RicXConti_Anno1.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   ")

            stbQ.AppendLine(" AND (RicXConti_Anno1.Anno = " & Agro_SQL_SaveNum(Anno) & ")  ")
            stbQ.AppendLine(" AND (RicXConti_Anno2.Anno = " & Agro_SQL_SaveNum(AnnoConfronto) & ")  ")

            'stbQ.AppendLine( " AND (RicXConti_Anno1.Saldo <> 0 )  ")
            'stbQ.AppendLine( " AND (RicXConti_Anno2.Saldo <> 0 )  ")

            If FiltroAggiuntivo1 <> "" Then
                stbQ.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo1))
            End If

            'stbQ.AppendLine( " ORDER BY RicXConti_Anno1.Id_Riclassificazione  "
            stbQ.AppendLine(" ) ")

            stbQ.AppendLine(" UNION ")

            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  RicXConti.Piva, RicXConti.Ric_Cod, RicXConti.Cod_Conto, RicXConti.Id_Riclassificazione AS Id_Ricl_Conto, Conti.Conto_Descr, Conti.Cod_Contatto, Conti.Flag_UE, RicXConti.Anno AS Anno_1,  ")
            stbQ.AppendLine("         RicXConti.Id_Riclassificazione AS Id_Riclassificazione_1, RicXConti.Dare_Avere AS Dare_Avere_1, RicXConti.Saldo AS Saldo_1,  ")
            stbQ.AppendLine("         RicXConti.Imputabile AS Imputabile_1, 0 AS Anno_2, '' AS Id_Riclassificazione_2, '' AS Dare_Avere_2, 0 AS Saldo_2, 0 AS Imputabile_2,  ")

            stbQ.AppendLine(" '' AS Differenza_Perc_Analitico, '' AS Differenza_Analitico,  ")
            stbQ.AppendLine(" '' AS Differenza_Perc_SaldoTotale, '' AS Differenza_SaldoTotale,  ")
            stbQ.AppendLine(" '' AS TotaleGruppo,   ")
            stbQ.AppendLine(" '' AS PesoAnalitico_TotaleGruppo, '' AS PesoAnalitico_Perc_TotaleGruppo,  ")
            stbQ.AppendLine(" '' AS PesoTotale_TotaleGruppo, '' AS PesoTotale_Perc_TotaleGruppo,  ")
            stbQ.AppendLine(" '' AS SaldoTotale_1, '' AS SaldoTotale_2, ")
            stbQ.AppendLine(" '' AS Extra_Str1, '' AS Extra_Str2, '' AS Extra_Str3, '' AS Extra_Str4 ")

            stbQ.AppendLine(" FROM    RicXConti INNER JOIN ")
            stbQ.AppendLine("         Conti ON RicXConti.Cod_Conto = Conti.Cod_Conto ")
            stbQ.AppendLine(" WHERE   (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  ")
            stbQ.AppendLine(" AND     (RicXConti.Cod_Conto NOT IN ")
            stbQ.AppendLine("                             (SELECT cod_conto ")
            stbQ.AppendLine("                             FROM    ricxconti ")
            stbQ.AppendLine("                             WHERE   Anno = " & Agro_SQL_SaveNum(AnnoConfronto) & ")  ")
            stbQ.AppendLine("         ) ")

            'stbQ.AppendLine( " AND (Saldo <> 0 )  " & vbCrLf

            If FiltroAggiuntivo2 <> "" Then
                stbQ.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo2))
            End If

            'stbQ.AppendLine( " ORDER BY RicXConti.Id_Riclassificazione "
            stbQ.AppendLine(" ) ")

            stbQ.AppendLine(" UNION ")

            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT     RicXConti.Piva, RicXConti.Ric_Cod, RicXConti.Cod_Conto, RicXConti.Id_Riclassificazione AS Id_Ricl_Conto, Conti.Conto_Descr, Conti.Cod_Contatto, Conti.Flag_UE, 0 AS Anno_1,  ")
            stbQ.AppendLine("  '' AS Id_Riclassificazione_1, '' AS Dare_Avere_1, 0 AS Saldo_1, 0 AS Imputabile_1, RicXConti.Anno AS Anno_2,  ")
            stbQ.AppendLine(" RicXConti.Id_Riclassificazione AS Id_Riclassificazione_2, RicXConti.Dare_Avere AS Dare_Avere_2, RicXConti.Saldo AS Saldo_2, RicXConti.Imputabile AS Imputabile_2, ")

            stbQ.AppendLine(" '' AS Differenza_Perc_Analitico, '' AS Differenza_Analitico,  ")
            stbQ.AppendLine(" '' AS Differenza_Perc_SaldoTotale, '' AS Differenza_SaldoTotale,  ")
            stbQ.AppendLine(" '' AS TotaleGruppo,   ")
            stbQ.AppendLine(" '' AS PesoAnalitico_TotaleGruppo, '' AS PesoAnalitico_Perc_TotaleGruppo,  ")
            stbQ.AppendLine(" '' AS PesoTotale_TotaleGruppo, '' AS PesoTotale_Perc_TotaleGruppo,  ")
            stbQ.AppendLine(" '' AS SaldoTotale_1, '' AS SaldoTotale_2, ")
            stbQ.AppendLine(" '' AS Extra_Str1, '' AS Extra_Str2, '' AS Extra_Str3, '' AS Extra_Str4 ")

            stbQ.AppendLine(" FROM         RicXConti INNER JOIN ")
            stbQ.AppendLine(" Conti ON RicXConti.Cod_Conto = Conti.Cod_Conto ")
            stbQ.AppendLine(" WHERE   (RicXConti.Anno = " & Agro_SQL_SaveNum(AnnoConfronto) & ")  ")
            stbQ.AppendLine(" AND     (RicXConti.Cod_Conto NOT IN ")
            stbQ.AppendLine("                             (SELECT cod_conto ")
            stbQ.AppendLine("                             FROM    ricxconti ")
            stbQ.AppendLine("                             WHERE   Anno = " & Agro_SQL_SaveNum(Anno) & ")  ")
            stbQ.AppendLine("         ) ")

            'stbQ.AppendLine( " AND (Saldo <> 0 )  ")

            If FiltroAggiuntivo3 <> "" Then
                stbQ.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo3))
            End If

            'stbQ.AppendLine( " ORDER BY RicXConti.Id_Riclassificazione ")
            stbQ.AppendLine(" ) ")

            If Ordinamento <> "" Then
                stbQ.AppendLine(Ordinamento)
            Else
                'stbQ.AppendLine( " ORDER BY RicXConti_Anno1.Id_Riclassificazione, RicXConti_Anno2.Id_Riclassificazione   ")
                stbQ.AppendLine(" ORDER BY Id_Ricl_Conto   ")
            End If

            '--------------------------------------------------------------------------
            risp = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine, DataSet2Fill, strNomeDtNelDS)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Risp

    End Function


    Public Function PianoContiEconomici_SaldoConto_A_B_Confronto(ByRef SaldoTotale_A_1 As Decimal,
                                                                 ByRef SaldoTotale_B_1 As Decimal,
                                                                 ByRef SaldoTotale_A_2 As Decimal,
                                                                 ByRef SaldoTotale_B_2 As Decimal,
                                                                 ByVal Piva_Ricl As String,
                                                                 ByVal Anno As Integer,
                                                                 ByVal AnnoConfronto As Integer,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.PianoContiEconomici_SaldoConto_A_B_Confronto"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim risp As Boolean = False
        Dim dt As DataTable

        Try

            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  SUM(RicxConti.Saldo) AS SaldoTotale ")
            stbQ.AppendLine(" FROM Conti ")
            stbQ.AppendLine(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva ")
            stbQ.AppendLine(" WHERE   (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   ")
            stbQ.AppendLine(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  ")
            stbQ.AppendLine(" AND (RicXConti.Id_Riclassificazione LIKE 'A%')  ")
            stbQ.AppendLine(" ) ")
            stbQ.AppendLine(" UNION ALL ")
            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  SUM(RicxConti.Saldo) AS SaldoTotale ")
            stbQ.AppendLine(" FROM Conti ")
            stbQ.AppendLine(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva ")
            stbQ.AppendLine(" WHERE   (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   ")
            stbQ.AppendLine(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  ")
            stbQ.AppendLine(" AND (RicXConti.Id_Riclassificazione LIKE 'B%')  ")
            stbQ.AppendLine(" ) ")
            stbQ.AppendLine(" UNION ALL ")
            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  SUM(RicxConti.Saldo) AS SaldoTotale ")
            stbQ.AppendLine(" FROM Conti ")
            stbQ.AppendLine(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva ")
            stbQ.AppendLine(" WHERE   (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   ")
            stbQ.AppendLine(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(AnnoConfronto) & ")  ")
            stbQ.AppendLine(" AND (RicXConti.Id_Riclassificazione LIKE 'A%')  ")
            stbQ.AppendLine(" ) ")
            stbQ.AppendLine(" UNION ALL ")
            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  SUM(RicxConti.Saldo) AS SaldoTotale ")
            stbQ.AppendLine(" FROM Conti ")
            stbQ.AppendLine(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva ")
            stbQ.AppendLine(" WHERE   (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   ")
            stbQ.AppendLine(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(AnnoConfronto) & ")  ")
            stbQ.AppendLine(" AND (RicXConti.Id_Riclassificazione LIKE 'B%')  ")
            stbQ.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso DT.Rows.Count <> 0 Then
                If Not IsNothing(dt.Rows(0).Item("SaldoTotale")) Then
                    SaldoTotale_A_1 = CDec(dt.Rows(0).Item("SaldoTotale"))
                Else
                    SaldoTotale_A_1 = 0
                End If

                If Not IsNothing(dt.Rows(1).Item("SaldoTotale")) Then
                    SaldoTotale_B_1 = CDec(dt.Rows(1).Item("SaldoTotale"))
                Else
                    SaldoTotale_B_1 = 0
                End If
                If Not IsNothing(dt.Rows(2).Item("SaldoTotale")) Then
                    SaldoTotale_A_2 = CDec(dt.Rows(2).Item("SaldoTotale"))
                Else
                    SaldoTotale_A_2 = 0
                End If

                If Not IsNothing(dt.Rows(3).Item("SaldoTotale")) Then
                    SaldoTotale_B_2 = CDec(dt.Rows(3).Item("SaldoTotale"))
                Else
                    SaldoTotale_B_2 = 0
                End If
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return risp

    End Function


    Public Function PianoContiEconomici_SaldoConto_Confronto(ByRef SaldoTotale_1 As Decimal,
                                                             ByRef SaldoTotale_2 As Decimal,
                                                             ByVal Id_Riclassificazione As String,
                                                             ByVal Piva_Ricl As String,
                                                             ByVal Anno As Integer,
                                                             ByVal AnnoConfronto As Integer,
                                                             ByRef objParametri As AgronicaCoreParametri
                                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.PianoContiEconomici_SaldoConto_Confronto"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim risp As Boolean = False
        Dim dt As DataTable

        Try

            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  SUM(RicxConti.Saldo) AS SaldoTotale ")
            stbQ.AppendLine(" FROM Conti ")
            stbQ.AppendLine(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva ")
            stbQ.AppendLine(" WHERE   (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   ")
            stbQ.AppendLine(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) & ")  ")
            stbQ.AppendLine(" AND (RicXConti.Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%')   ")
            stbQ.AppendLine(" ) ")
            stbQ.AppendLine(" UNION ALL ")
            stbQ.AppendLine(" ( ")
            stbQ.AppendLine(" SELECT  SUM(RicxConti.Saldo) AS SaldoTotale ")
            stbQ.AppendLine(" FROM Conti ")
            stbQ.AppendLine(" INNER JOIN RicXConti ON Conti.Cod_Conto = RicXConti.Cod_Conto ")
            stbQ.AppendLine(" INNER JOIN Riclassificazioni ON Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod AND Riclassificazioni.Piva = RicXConti.Piva ")
            stbQ.AppendLine(" WHERE   (RicXConti.Piva = '" & Agro_SQL_SaveText(Piva_Ricl) & "')   ")
            stbQ.AppendLine(" AND (RicXConti.Anno = " & Agro_SQL_SaveNum(AnnoConfronto) & ")  ")
            stbQ.AppendLine(" AND (RicXConti.Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%')   ")
            stbQ.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
                If Not IsNothing(dt.Rows(0).Item("SaldoTotale")) Then
                    SaldoTotale_1 = CDec(dt.Rows(0).Item("SaldoTotale"))
                Else
                    SaldoTotale_1 = 0
                End If

                If Not IsNothing(dt.Rows(1).Item("SaldoTotale")) Then
                    SaldoTotale_2 = CDec(dt.Rows(1).Item("SaldoTotale"))
                Else
                    SaldoTotale_2 = 0
                End If
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risp

    End Function


    Public Function Update_SaldiIniziali(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal tipoConto As String,
                                         ByVal codConto As Integer,
                                         ByVal codContatto As Integer,
                                         ByVal tipoContatto As Integer,
                                         ByVal codBanca As Integer,
                                         ByVal valSaldo As Decimal,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Update_SaldiIniziali()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If tipoConto = "P" Then
                strSql.Length = 0


                If codContatto <> 0 AndAlso tipoContatto = COD_CLIENTE Then
                    strSql.AppendLine(" UPDATE Risorse_Umane ")
                    strSql.AppendLine(" SET Saldo_Iniziale_crediti = " & Agro_SQL_SaveNum(valSaldo))
                    strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    strSql.AppendLine(" AND cod_risum = " & codContatto & "   ")
                    strSql.AppendLine(" AND cod_rapporto = " & COD_CLIENTE & "   ")
                End If

                If codContatto <> 0 AndAlso tipoContatto = COD_FORNITORE Then
                    strSql.AppendLine(" UPDATE Risorse_Umane ")
                    strSql.AppendLine(" SET Saldo_Iniziale_debiti = " & Agro_SQL_SaveNum(valSaldo))
                    strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    strSql.AppendLine(" AND cod_risum = " & codContatto & "   ")
                    strSql.AppendLine(" AND cod_rapporto = " & COD_FORNITORE & "   ")
                End If

                If codBanca <> 0 Then
                    strSql.AppendLine(" UPDATE Liquidita ")
                    strSql.AppendLine(" SET saldo_iniziale = " & Agro_SQL_SaveNum(valSaldo))
                    strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    strSql.AppendLine(" AND Cod_Liquidita = " & codBanca & "   ")
                End If


                If codConto <> 0 Then
                    strSql.AppendLine(" UPDATE RicXConti_Patrimonio ")
                    strSql.AppendLine(" SET saldo_iniziale = " & Agro_SQL_SaveNum(valSaldo))
                    strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    strSql.AppendLine(" AND Ric_cod_pat = " & Agro_SQL_SaveNum(2) & "   ")
                    strSql.AppendLine(" AND Cod_conto_pat = " & Agro_SQL_SaveNum(codConto) & "   ")
                End If

            End If

            If tipoConto = "E" Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE RicXConti ")
                strSql.AppendLine(" SET saldo_iniziale = " & Agro_SQL_SaveNum(valSaldo))
                strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                strSql.AppendLine(" AND Ric_cod = " & Agro_SQL_SaveNum(2) & "   ")
                strSql.AppendLine(" AND Cod_conto = " & Agro_SQL_SaveNum(codConto) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    Public Function Azzera_SaldiIniziali(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreStampeDAL.PianoConti.Azzera_SaldiIniziali()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine(" UPDATE Risorse_Umane ")
            strSql.AppendLine(" SET Saldo_Iniziale_crediti = 0 , Saldo_Iniziale_debiti = 0 ")
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            strSql.Length = 0
            strSql.AppendLine(" UPDATE Liquidita ")
            strSql.AppendLine(" SET saldo_iniziale = 0 ")
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            strSql.Length = 0
            strSql.AppendLine(" UPDATE RicXConti_Patrimonio ")
            strSql.AppendLine(" SET saldo_iniziale = 0 ")
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" AND Ric_cod_pat = " & Agro_SQL_SaveNum(2) & "   ")
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            strSql.Length = 0
            strSql.AppendLine(" UPDATE RicXConti ")
            strSql.AppendLine(" SET saldo_iniziale = 0 ")
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" AND Ric_cod = " & Agro_SQL_SaveNum(2) & "   ")
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

End Class

