Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider

Public Class Attivita_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Id_Attivita As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional loadLavCodAssociatiVisite As Boolean = False
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT A.* ")

            If loadLavCodAssociatiVisite Then
                StrSQL.AppendLine(" , ISNULL(ao2.Lav_Cod,0) as LAV_COD ")
            End If

            StrSQL.AppendLine(" FROM  Attivita A ")
            StrSQL.AppendLine(" LEFT  OUTER JOIN Tariffe T ON A.Piva = T.Piva AND A.Tariffa_Cod=T.Tariffa_Cod  ")

            If loadLavCodAssociatiVisite Then
                StrSQL.AppendLine(" INNER JOIN AttivitaXOperazioni ao ON A.ID_Attivita = ao.ID_Attivita AND ao.Lav_Cod = " & CostantiPersonalizzate.LAVCOD_VISITA)
                StrSQL.AppendLine(" LEFT OUTER JOIN AttivitaXOperazioni ao2 ON A.ID_Attivita = ao2.ID_Attivita AND ao2.Lav_Cod IN (" & CostantiPersonalizzate.STR_OP_COLLEGABILI_A_VISITE_NG & ") ")
            End If

            StrSQL.AppendLine(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Id_Attivita <> 0 Then
                StrSQL.AppendLine(" AND A.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY [Desc] ASC ")
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


    '#############################################################################################à
    ' Leggi le attivita per caricare la griglia in Controllo di Gestione
    Public Function LeggiAttivitaXGrigliaCDG(ByVal Id_Attivita As Integer,
                                             ByVal piva As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_R.LeggiAttivitaXGrigliaCDG()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Att.Piva , i.rag_soc, Att.ID_Attivita , Att.Sigla , [Desc] , Att.Sa_Cod ,")
            StrSQL.AppendLine(" CASE WHEN Att.Sa_Cod=-1 THEN 'Sì' ELSE 'No' END AS Sa_Cod_Des,")
            StrSQL.AppendLine(" Att.Utilizzo_GiasAPP AS Utilizzo_GiasAPP_Cod,")
            StrSQL.AppendLine(" CASE WHEN Att.Utilizzo_GiasAPP=1 THEN 'Sì' ELSE 'No' END AS Utilizzo_GiasAPP_Des,")
            StrSQL.AppendLine(" Att.Tipo_Utilizzo As Utilizzo_Budget_Consuntivo_Cod, ")
            StrSQL.AppendLine(" CASE WHEN Att.Tipo_Utilizzo = " & TipiEnumerativi.enum_Attvita_Consuntivo_Budget.SoloConsuntivo & " THEN 'Consuntivo' ")
            StrSQL.AppendLine(" WHEN Att.Tipo_Utilizzo = " & TipiEnumerativi.enum_Attvita_Consuntivo_Budget.SoloBudget & " THEN 'Budget' ")
            StrSQL.AppendLine(" WHEN Att.Tipo_Utilizzo = " & TipiEnumerativi.enum_Attvita_Consuntivo_Budget.Entrambi & " THEN 'Consuntivo / Budget' END AS Utilizzo_Budget_Consuntivo, ")
            StrSQL.AppendLine(" Att.Attivita_Poliannuale AS Attivita_Poliannuale_Cod,")
            StrSQL.AppendLine(" CASE WHEN Att.Attivita_Poliannuale = 1 THEN 'Sì' ELSE 'No' END AS Attivita_Poliannuale_Des,")
            StrSQL.AppendLine(" Att.Attivita_Extra_Campagna AS Attivita_Extra_Campagna_Cod,")
            StrSQL.AppendLine(" CASE WHEN Att.Attivita_Extra_Campagna = 1 THEN 'Sì' ELSE 'No' END AS Attivita_Extra_Campagna_Des,")
            StrSQL.AppendLine(" CASE WHEN Att.Filtro_Operazioni = ''  THEN 'Vuoto' ELSE Att.Filtro_Operazioni END AS Opi_Cul_Cod ,")
            StrSQL.AppendLine(" CASE WHEN Att.Filtro_Operazioni = 'C' THEN 'Sì' ELSE 'No' END AS Opi_Cul_Des,")
            StrSQL.AppendLine(" IsNull(Att.Frazionabile,0) AS Frazionabile,")
            StrSQL.AppendLine(" CASE WHEN Att.Frazionabile = 1 THEN 'Sì' ELSE 'No' END AS Frazionabile_Des,")
            StrSQL.AppendLine(" Att.Qualifica_Cod_Min AS Qualifica_Cod, ISNULL(Qual.Qualifica_Des,'Nessuna') AS Qualifica_Des,")
            StrSQL.AppendLine(" Att.Tariffa_Cod, ISNULL(Tar.Tariffa_Des,'Nessuna') AS Tariffa_Des,")
            StrSQL.AppendLine(" Att.Validita_Inizio, Att.Validita_Fine,")
            StrSQL.AppendLine(" Att.Filtro_Categorie_Costi AS Costi_Cod_String , Att.Filtro_Categorie_Ricavi AS Ricavi_Cod_String, ")
            StrSQL.AppendLine(" ISNULL(Att.Ordine, 0) AS Ordine_Att, ")
            StrSQL.AppendLine(" ISNULL(Att.ID_Attivita_Gruppo, 0) AS ID_GruppoAtt1, ")
            StrSQL.AppendLine(" ISNULL(AttGrup1.Desc_Gruppo, 'Nessuna') AS Desc_GruppoAtt1, ")
            StrSQL.AppendLine(" ISNULL(Att.ID_Attivita_Gruppo2, 0) AS ID_GruppoAtt2, ")
            StrSQL.AppendLine(" ISNULL(AttGrup2.Desc_Gruppo, 'Nessuna') AS Desc_GruppoAtt2, ")
            StrSQL.AppendLine(" ISNULL(Att.ID_Attivita_Gruppo3, 0) AS ID_GruppoAtt3, ")
            StrSQL.AppendLine(" ISNULL(AttGrup3.Desc_Gruppo, 'Nessuna') AS Desc_GruppoAtt3, ")
            StrSQL.AppendLine(" Att.Attivita_Interna AS Attivita_Interna,")
            StrSQL.AppendLine(" CASE WHEN Att.Attivita_Interna = 1 THEN 'Sì' ELSE 'No' END AS Attivita_Interna_Des,")
            StrSQL.AppendLine(" ISNULL(Att.Valutazione_Conto_Cod_Attivo, 0) AS Valutazione_Conto_Cod_Attivo, ")
            StrSQL.AppendLine(" ISNULL(VCA.Valutazione_Conto_Des, 'Nessuno') AS Valutazione_Conto_Cod_Attivo_Des, ")
            StrSQL.AppendLine(" ISNULL(Att.Valutazione_Conto_Cod_Passivo, 0) AS Valutazione_Conto_Cod_Passivo, ")
            StrSQL.AppendLine(" ISNULL(VCP.Valutazione_Conto_Des, 'Nessuno') AS Valutazione_Conto_Cod_Passivo_Des ")
            StrSQL.AppendLine(" FROM  Attivita Att")

            StrSQL.AppendLine(" LEFT JOIN  imprese i on i.piva = att.piva ")
            StrSQL.AppendLine(" LEFT JOIN  Qualifiche Qual ON Qual.Qualifica_Cod = Att.Qualifica_Cod_Min ")
            StrSQL.AppendLine(" LEFT JOIN  Tariffe Tar ON Att.Tariffa_Cod = Tar.Tariffa_Cod ")
            StrSQL.AppendLine(" LEFT JOIN  Attivita_Gruppi AttGrup1 ON Att.ID_Attivita_Gruppo = AttGrup1.ID_Attivita_Gruppo ")
            StrSQL.AppendLine(" LEFT JOIN  Attivita_Gruppi AttGrup2 ON Att.ID_Attivita_Gruppo2 = AttGrup2.ID_Attivita_Gruppo ")
            StrSQL.AppendLine(" LEFT JOIN  Attivita_Gruppi AttGrup3 ON Att.ID_Attivita_Gruppo3 = AttGrup3.ID_Attivita_Gruppo ")
            StrSQL.AppendLine(" LEFT JOIN  Valutazione_Conto VCA ON Att.Valutazione_Conto_Cod_Attivo = VCA.Valutazione_Conto_Cod ")
            StrSQL.AppendLine(" LEFT JOIN  Valutazione_Conto VCP ON Att.Valutazione_Conto_Cod_Passivo = VCP.Valutazione_Conto_Cod ")

            StrSQL.AppendLine(" WHERE Att.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   Att.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND   Att.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If Id_Attivita <> 0 Then
                StrSQL.AppendLine(" AND Att.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If piva <> "" Then
                StrSQL.AppendLine(" AND (Att.Piva = '" & Agro_SQL_SaveText(piva) & "' Or Att.Sa_Cod = -1 )")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Att.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Att.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Att.Data_Creazione DESC , [Desc] ASC ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function

    '#############################################################################################à
    ' recupera le attività che non sono associate a nessuna operazione

    Public Function AttivitaNonAssociate(ByVal Id_Attivita As Int32,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_R.AttivitaNonAssociate()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT A.* ")
            StrSQL.Append(" FROM  Attivita A ")
            StrSQL.Append(" WHERE NOT EXISTS (SELECT * FROM AttivitaxOperazioni AO WHERE AO.Id_Attivita= A.Id_Attivita ) ")
            StrSQL.Append(" AND   A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If Id_Attivita <> 0 Then
                StrSQL.Append(" AND A.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY [Desc] ASC ")
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


    Public Function CostoOrario(ByVal Id_Attivita As Int32,
                                ByVal Turno_Cod As Int32,
                                ByVal DataAttivita As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim dblCostoOrarioTot As Decimal

        Try

            ' leggo i dati del turno
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Tariffa_Base, Perc_Aumento_Tariffa, Perc_Aumento_Turno ")
            StrSQL.Append(" FROM  Turni T ")
            StrSQL.Append(" WHERE T.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   T.Turno_Cod = " & Agro_SQL_SaveNum(Turno_Cod) & " ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   T.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   T.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Dim Aumento_Tariffa As Decimal
            Dim Aumento_Turno As Decimal
            Dim TariffaBase As Integer

            If DT.Rows.Count = 1 Then
                Aumento_Tariffa = DT.Rows(0).Item("Perc_Aumento_Tariffa")
                Aumento_Turno = DT.Rows(0).Item("Perc_Aumento_Turno")
                TariffaBase = DT.Rows(0).Item("Tariffa_Base")
            End If

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  TE.Paga_Base, TE.Contributi, TE.Aumento_CCNL, TE.Aumento_CIPL, TE.Terzo_Elemento, TE.TFR ")

            If TariffaBase = 0 Then

                StrSQL.Append(" FROM  Tariffe T ")
                StrSQL.Append(" INNER JOIN Tariffe_Elementi TE ON TE.Piva = T.Piva AND TE.Tariffa_Cod=T.Tariffa_Cod  ")
                StrSQL.Append(" INNER JOIN Attivita A ON A.Piva = T.Piva AND A.Tariffa_Cod=T.Tariffa_Cod  ")
                StrSQL.Append(" WHERE A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND   A.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")


            Else
                StrSQL.Append(" FROM  Tariffe T ")
                StrSQL.Append(" INNER JOIN Tariffe_Elementi TE ON TE.Piva = T.Piva AND TE.Tariffa_Cod=T.Tariffa_Cod  ")
                StrSQL.Append(" WHERE T.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND   T.Tariffa_Cod = " & Agro_SQL_SaveNum(TariffaBase) & " ")

            End If

            StrSQL.Append(" AND     TE.Validita_Inizio <= " & Agro_SQL_SaveDate(DataAttivita) & " ")
            StrSQL.Append(" AND     TE.Validita_Fine >= " & Agro_SQL_SaveDate(DataAttivita) & " ")


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   T.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   T.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            Dim Paga_Base As Decimal
            Dim Contributi As Decimal
            Dim Aumento_CCNL As Decimal
            Dim Aumento_CIPL As Decimal
            Dim Terzo_Elemento As Decimal
            Dim TFR As Decimal

            'Dim dblCostoOrario As Decimal
            If DT.Rows.Count = 1 Then
                'dblCostoOrario = DT.Rows(0).Item("Costo_Orario")
                Paga_Base = DT.Rows(0).Item("Paga_Base")
                Contributi = DT.Rows(0).Item("Contributi")
                Aumento_CCNL = DT.Rows(0).Item("Aumento_CCNL")
                Aumento_CIPL = DT.Rows(0).Item("Aumento_CIPL")
                Terzo_Elemento = DT.Rows(0).Item("Terzo_Elemento")
                TFR = DT.Rows(0).Item("TFR")
            End If

            If Not IsNothing(DT) Then
                DT.Dispose()
            End If
            DT = Nothing

            Dim TariffaOrdinaria As Decimal = 0
            Dim Straordinario As Decimal = 0

            If TariffaBase <> 0 Then
                Paga_Base = Paga_Base + Paga_Base * Aumento_Tariffa / 100
                Contributi = Contributi + Contributi * Aumento_Tariffa / 100
                Aumento_CCNL = Aumento_CCNL + Aumento_CCNL * Aumento_Tariffa / 100
                Aumento_CIPL = Aumento_CIPL + Aumento_CIPL * Aumento_Tariffa / 100
                Terzo_Elemento = Terzo_Elemento + Terzo_Elemento * Aumento_Tariffa / 100
            End If

            TariffaOrdinaria = Paga_Base + Contributi + Aumento_CCNL + Aumento_CIPL + Terzo_Elemento
            Straordinario = (Paga_Base + Contributi + Aumento_CCNL + Aumento_CIPL) * Aumento_Turno / 100

            dblCostoOrarioTot = TariffaOrdinaria + Straordinario + TFR

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return dblCostoOrarioTot

    End Function



    Public Function Leggi(ByVal Id_Attivita As Int32,
                          ByVal Cod_Rapporto As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Attivita A ")
            StrSQL.Append(" INNER JOIN AttivitaXRapporti_Contabili ARC ON A.Piva = ARC.Piva AND A.Id_Attivita=ARC.Id_Attivita  ")
            StrSQL.Append(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If Id_Attivita <> 0 Then
                StrSQL.Append(" AND A.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If Cod_Rapporto <> 0 Then
                StrSQL.Append(" AND ARC.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   A.Inviato >=0 ")
                    StrSQL.Append(" AND   ARC.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   A.Inviato =-1 ")
                    StrSQL.Append(" AND   ARC.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY [Desc] ASC ")
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



    Public Function AttivitaDes_From_AttivitaCod(ByVal Id_Attivita As Int32,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_R.AttivitaDes_From_AttivitaCod()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim strAttivitaDes As String

        Try

            ' leggo i dati del turno
            StrSQL.Length = 0
            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT [Desc] ")
            StrSQL.Append(" FROM  Attivita A ")
            StrSQL.Append(" WHERE A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   A.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If DT.Rows.Count = 1 Then
                strAttivitaDes = DT.Rows(0).Item("Desc")
            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return strAttivitaDes

    End Function


    Public Function Leggi_APP(ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_R.Leggi_APP()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT A.* ")
            StrSQL.Append(" FROM  Attivita A ")
            StrSQL.Append(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   A.Utilizzo_GiasAPP = 1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY [Desc] ASC ")
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

End Class


Public Class Attivita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    <Obsolete("NON USARE - Usare ScriviCompleta")>
    Public Function Scrivi(ByVal Id_Attivita As Integer,
                           ByVal Desc As String,
                           ByVal Sigla As String,
                           ByVal Tariffa_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_W.Scrivi()"

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


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Attivita( ")
            StrSQL.Append("                     Piva,               ID_Attivita,        [Desc],  Sigla,   Tariffa_Cod, ")
            StrSQL.Append("                     Inviato,            DataInvio, ")
            StrSQL.Append("                     Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                     UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                     Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("                     Piva_SuperUser,     Sa_Cod ")
            StrSQL.Append("                     ) ")


            StrSQL.Append("VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Desc) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Sigla) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")


            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , -1  ")
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



    '#########################################################################
    <Obsolete("NON USARE - Usare ModificaCompleta")>
    Public Function Modifica(ByVal Id_Attivita As Integer,
                             ByVal Desc As String,
                             ByVal Tariffa_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_W.Modifica()"

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

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("UPDATE Attivita SET ")
            StrSQL.Append("       [Desc]         = '" & Agro_SQL_SaveText(Desc) & "'")
            StrSQL.Append("      ,Tariffa_Cod    =  " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")

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

    '#########################################################################
    Public Function ModificaDescrizione(
                             ByVal Id_Attivita As Int32,
                             ByVal Desc As String,
                             ByVal Sigla As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Attivita_W.ModificaDescrizione()"

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

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.AppendLine("UPDATE Attivita SET ")
            StrSQL.AppendLine("   [Desc]         = '" & Agro_SQL_SaveText(Desc) & "'")
            StrSQL.AppendLine("  ,Sigla          = '" & Agro_SQL_SaveText(Sigla) & "'")

            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   ID_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")

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


    Public Function Cancella(ByVal Id_Attivita As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Attivita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND ID_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Attivita ")
                StrSQL.Append(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND ID_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
                StrSQL.Append(" AND Inviato = 0")

            End If


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function ScriviCompleta(ByVal piva As String,
                                   ByVal Id_Attivita As Integer,
                                   ByVal Desc As String,
                                   ByVal Tariffa_Cod As Integer,
                                   ByVal Sigla As String,
                                   ByVal Qualifica_Cod_Min As Integer,
                                   ByVal Filtro_Operazioni As String,
                                   ByVal Filtro_Categorie_Costi As String,
                                   ByVal Filtro_Categorie_Ricavi As String,
                                   ByVal Utilizzo_GiasAPP As Integer,
                                   ByVal Attivita_Extra_Campagna As Integer,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Attivita_Poliannuale As Integer,
                                   ByVal Frazionabile As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   Optional ByVal Ordine As Integer = 0,
                                   Optional ByVal ID_Attivita_Gruppo1 As Integer = 0,
                                   Optional ByVal ID_Attivita_Gruppo2 As Integer = 0,
                                   Optional ByVal ID_Attivita_Gruppo3 As Integer = 0,
                                   Optional ByVal Uso_Budget_Cons As Integer = 0,
                                   Optional ByVal Attivita_Interna As Integer = 0,
                                   Optional ByVal Valutazione_Conto_Cod_Attivo As Integer = 0,
                                   Optional ByVal Valutazione_Conto_Cod_Passivo As Integer = 0
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_W.ScriviCompleta()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Attivita( ")
            StrSQL.AppendLine("                     Piva,                     ID_Attivita,         [Desc],                     Tariffa_Cod, ")
            StrSQL.AppendLine("                     Sigla,                    Qualifica_Cod_Min,   Filtro_Operazioni,          Filtro_Categorie_Costi, ")
            StrSQL.AppendLine("                     Filtro_Categorie_Ricavi,  Utilizzo_GiasAPP,    Attivita_Extra_Campagna,    Attivita_Poliannuale,     Frazionabile, ")
            StrSQL.AppendLine("                     Piva_SuperUser,           Sa_Cod, ")
            StrSQL.AppendLine("                     Ordine, ")
            StrSQL.AppendLine("                     ID_Attivita_Gruppo,       ID_Attivita_Gruppo2, ID_Attivita_Gruppo3,        Attivita_Interna, ")
            StrSQL.AppendLine("                     Valutazione_Conto_Cod_Attivo,       Valutazione_Conto_Cod_Passivo, ")

            StrSQL.AppendLine("                     Inviato,                  DataInvio, ")
            StrSQL.AppendLine("                     Data_Creazione,           Data_Modifica, ")
            StrSQL.AppendLine("                     UserName_Creazione,       UserName_Modifica, ")
            StrSQL.AppendLine("                     Validita_Inizio,          Validita_Fine, Tipo_Utilizzo ")
            StrSQL.AppendLine("                     ) ")

            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Desc) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Sigla) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Qualifica_Cod_Min) & " ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Filtro_Operazioni) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Filtro_Categorie_Costi) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Filtro_Categorie_Ricavi) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Utilizzo_GiasAPP) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Attivita_Extra_Campagna) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Attivita_Poliannuale) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Frazionabile) & " ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Ordine) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Attivita_Gruppo1) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Attivita_Gruppo2) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Attivita_Gruppo3) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Attivita_Interna) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Valutazione_Conto_Cod_Attivo) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Valutazione_Conto_Cod_Passivo) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Uso_Budget_Cons) & "  ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function ModificaCompleta(ByVal Id_Attivita As Integer,
                                     ByVal Desc As String,
                                     ByVal Tariffa_Cod As Integer,
                                     ByVal Sigla As String,
                                     ByVal Qualifica_Cod_Min As Integer,
                                     ByVal Filtro_Operazioni As String,
                                     ByVal Filtro_Categorie_Costi As String,
                                     ByVal Filtro_Categorie_Ricavi As String,
                                     ByVal Utilizzo_GiasAPP As Integer,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Attivita_Poliannuale As Integer,
                                     ByVal Frazionabile As Integer,
                                     ByVal Utilizzazione_Budget_Consuntivo As Integer,
                                     ByVal Ordine As Integer,
                                     ByVal ID_Attivita_Gruppo1 As Integer,
                                     ByVal ID_Attivita_Gruppo2 As Integer,
                                     ByVal ID_Attivita_Gruppo3 As Integer,
                                     ByVal Valutazione_Conto_Cod_Attivo As Integer,
                                     ByVal Valutazione_Conto_Cod_Passivo As Integer,
                                     ByVal Validita_Inizio As Date,
                                     ByVal Validita_Fine As Date,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal Attivita_Interna As Integer = 0
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_W.ModificaCompleta()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Attivita SET ")
            StrSQL.AppendLine("       [Desc]    = '" & Agro_SQL_SaveText(Desc) & "'")
            StrSQL.AppendLine("      , Tariffa_Cod    =  " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            StrSQL.AppendLine("      , Sigla    =  '" & Agro_SQL_SaveText(Sigla) & "' ")
            StrSQL.AppendLine("      , Qualifica_Cod_Min    =  " & Agro_SQL_SaveNum(Qualifica_Cod_Min) & " ")
            StrSQL.AppendLine("      , Filtro_Operazioni    =  '" & Agro_SQL_SaveText(Filtro_Operazioni) & "' ")
            StrSQL.AppendLine("      , Filtro_Categorie_Costi    =  '" & Agro_SQL_SaveText(Filtro_Categorie_Costi) & "' ")
            StrSQL.AppendLine("      , Filtro_Categorie_Ricavi    =  '" & Agro_SQL_SaveText(Filtro_Categorie_Ricavi) & "' ")
            StrSQL.AppendLine("      , Utilizzo_GiasAPP    =  " & Agro_SQL_SaveNum(Utilizzo_GiasAPP) & " ")
            StrSQL.AppendLine("      , Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("      , Attivita_Poliannuale    =  " & Agro_SQL_SaveNum(Attivita_Poliannuale) & " ")
            StrSQL.AppendLine("      , Frazionabile    =  " & Agro_SQL_SaveNum(Frazionabile) & " ")
            StrSQL.AppendLine("      , Ordine    =  " & Agro_SQL_SaveNum(Ordine) & " ")
            StrSQL.AppendLine("      , ID_Attivita_Gruppo    =  " & Agro_SQL_SaveNum(ID_Attivita_Gruppo1) & " ")
            StrSQL.AppendLine("      , ID_Attivita_Gruppo2    =  " & Agro_SQL_SaveNum(ID_Attivita_Gruppo2) & " ")
            StrSQL.AppendLine("      , ID_Attivita_Gruppo3    =  " & Agro_SQL_SaveNum(ID_Attivita_Gruppo3) & " ")
            StrSQL.AppendLine("      , Attivita_Interna    =  " & Agro_SQL_SaveNum(Attivita_Interna) & " ")
            StrSQL.AppendLine("      , Valutazione_Conto_Cod_Attivo    =  " & Agro_SQL_SaveNum(Valutazione_Conto_Cod_Attivo) & " ")
            StrSQL.AppendLine("      , Valutazione_Conto_Cod_Passivo    =  " & Agro_SQL_SaveNum(Valutazione_Conto_Cod_Passivo) & " ")

            StrSQL.AppendLine("      , Inviato  =  0 ")
            StrSQL.AppendLine("      , DataInvio    =  Null ")
            StrSQL.AppendLine("      , Data_Modifica    =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("      , UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("      , Validita_Inizio  =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("      , Validita_Fine    =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("      , Tipo_Utilizzo    =  " & Agro_SQL_SaveNum(Utilizzazione_Budget_Consuntivo))

            StrSQL.AppendLine(" WHERE Piva_SuperUser    = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   ID_Attivita   = " & Agro_SQL_SaveNum(Id_Attivita) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
