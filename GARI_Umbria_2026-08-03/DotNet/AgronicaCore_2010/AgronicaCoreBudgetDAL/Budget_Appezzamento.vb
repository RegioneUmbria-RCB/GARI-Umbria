Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.My.Resources
Public Class Budget_Appezzamento_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Recupera_Superfici_Campo(ByVal Id_Budget As Integer,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Campo_Cod As Integer,
                                        ByRef SAU_Totale As Decimal,
                                        ByRef SAU_Biologico As Decimal,
                                        ByRef SAU_Conversione As Decimal,
                                        ByRef SAU_Convenzionale As Decimal,
                                        ByRef SAU_Catastale As Decimal,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Appezzamento_R.Recupera_Superfici_Campo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim TipoAgricoltura As enum_TipoAgricoltura

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Budget_Appezzamento.Id_Budget, ")
            StrSQL.Append("         Budget_Appezzamento.PIVA, ")
            StrSQL.Append("         Budget_Appezzamento.SA_COD,  ")
            StrSQL.Append("         Budget_Appezzamento.Campo_Cod,  ")
            StrSQL.Append("         Budget_Appezzamento.APPEZZA,  ")
            StrSQL.Append("         Budget_Appezzamento.APP_NOME,  ")
            StrSQL.Append("         Budget_Appezzamento.SUP_APP,  ")
            StrSQL.Append("         Budget_Appezzamento_Codici.val_cod,  ")
            StrSQL.Append("         Budget_Appezzamento.Validita_Inizio,  ")
            StrSQL.Append("         Budget_Appezzamento.Validita_Fine, ")
            StrSQL.Append("         ISNULL(Budget_Appezzamento_Codici.id_cod, 1018) AS Id_Cod  ")

            StrSQL.Append(" FROM    Budget_Appezzamento LEFT OUTER JOIN ")
            StrSQL.Append("         Budget_Appezzamento_Codici ON Budget_Appezzamento.Id_Budget = Budget_Appezzamento_Codici.Id_Budget   ")
            StrSQL.Append("         AND Budget_Appezzamento.PIVA = Budget_Appezzamento_Codici.PIVA ")
            StrSQL.Append("         AND Budget_Appezzamento.SA_COD = Budget_Appezzamento_Codici.sa_cod ")
            StrSQL.Append("         AND Budget_Appezzamento.APPEZZA = Budget_Appezzamento_Codici.appezza ")
            StrSQL.Append("         AND Budget_Appezzamento_Codici.id_cod = 1018")

            StrSQL.Append(" WHERE   (Budget_Appezzamento.Id_Budget = " & Id_Budget & ")  ")
            StrSQL.Append(" AND     (Budget_Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
            StrSQL.Append(" AND     (Budget_Appezzamento.SA_COD = " & Sa_Cod & ")  ")
            StrSQL.Append(" AND     (Budget_Appezzamento.Campo_Cod = " & Campo_Cod & ") ")
            StrSQL.Append(" AND     (Budget_Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (Budget_Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")")


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Budget_Appezzamento.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Budget_Appezzamento.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        If DT.Rows.Count > 0 Then

            'Inizializzo
            SAU_Totale = 0
            SAU_Biologico = 0
            SAU_Conversione = 0
            SAU_Convenzionale = 0

            'Ciclo sugli elementi selezionati
            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                'Aggiorno il totale
                SAU_Totale += CDbl(DT.Rows(i).Item("sup_app"))

                'Verifico il tipo di agricoltura
                If IsDBNull(DT.Rows(i).Item("val_cod")) Then

                    'NOTA
                    'Se non e' impostato il tipo di agricoltura,
                    'considero come default quella Convenzionale
                    TipoAgricoltura = enum_TipoAgricoltura.Convenzionale
                Else
                    TipoAgricoltura = CInt(DT.Rows(i).Item("val_cod"))
                End If


                'Aggiorno il contatore giusto
                Select Case TipoAgricoltura

                    Case enum_TipoAgricoltura.Convenzionale
                        SAU_Convenzionale += CDbl(DT.Rows(i).Item("sup_app"))

                    Case enum_TipoAgricoltura.InConversione
                        SAU_Conversione += CDbl(DT.Rows(i).Item("sup_app"))

                    Case enum_TipoAgricoltura.Biologica
                        SAU_Biologico += CDbl(DT.Rows(i).Item("sup_app"))

                End Select

                'Prossimo record
            Next

        Else
            SAU_Totale = 0
            SAU_Biologico = 0
            SAU_Conversione = 0
            SAU_Convenzionale = 0
        End If


        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")

            StrSQL.Append(" FROM    Budget_CampiXParticelle ")
            StrSQL.Append(" WHERE   (Budget_CampiXParticelle.Id_Budget = " & Id_Budget & ")  ")
            StrSQL.Append(" AND     (Budget_CampiXParticelle.PIVA = '" & Piva & "')  ")
            StrSQL.Append(" AND     (Budget_CampiXParticelle.SA_COD = " & Sa_Cod & ")  ")
            StrSQL.Append(" AND     (Budget_CampiXParticelle.Campo_Cod = " & Campo_Cod & ") ")
            StrSQL.Append(" AND     (Budget_CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (Budget_CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")")


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Budget_CampiXParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Budget_CampiXParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        If DT.Rows.Count > 0 Then

            'Inizializzo
            SAU_Catastale = 0

            'Ciclo sugli elementi selezionati
            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                'Aggiorno il totale
                SAU_Catastale += CDbl(DT.Rows(i).Item("AREA"))

                'Prossimo record
            Next

        Else

            SAU_Catastale = 0

        End If

    End Function

    '##########################################################################################
    Public Function Recupera_Appezzamenti_Colture_del_Campo(ByVal Id_Budget As Integer,
                                        ByVal Piva As String,
                                        ByVal Sa_cod As Integer,
                                        ByVal Campo_Cod As Integer,
                                        ByVal DataValiditaInizio As Date,
                                        ByVal DataValiditaFine As Date,
                                        ByVal FlagIncludiAppezzamentiLiberi As Boolean,
                                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal IDTestataTemp As Integer = 0
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Appezzamento_R.Recupera_Appezzamenti_Colture_del_Campo()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    stb.Length = 0

                    stb.AppendLine(" SELECT  Budget_Appezzamento.Id_Budget,  ")
                    stb.AppendLine("         Budget_Appezzamento.PIVA,  ")
                    stb.AppendLine("         Budget_Appezzamento.SA_COD, Budget_Appezzamento.APPEZZA, Budget_Appezzamento.Campo_Cod, Budget_Appezzamento.SUP_APP,  ")
                    stb.AppendLine("         Budget_Appezzamento.APP_NOME, Budget_Appezzamento.Validita_Inizio, Budget_Appezzamento.Validita_Fine, ISNULL(Budget_Reg_Impianti.ID_REG,0) AS ID_REG,  ")
                    stb.AppendLine("         Budget_Reg_Impianti.Validita_Inizio AS Impianto_Validita_Inizio, Budget_Reg_Impianti.Validita_Fine AS Impianto_Validita_Fine, Budget_Reg_Impianti.CUL_COD,  ")
                    stb.AppendLine("         Cultivar.Cul_Des, SpecieVegetali.Veg_Des ")

                    stb.AppendLine(" FROM    Cultivar INNER JOIN ")
                    stb.AppendLine("         SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod RIGHT OUTER JOIN ")
                    stb.AppendLine("         Budget_Reg_Impianti ON Cultivar.Cul_Cod = Budget_Reg_Impianti.CUL_COD RIGHT OUTER JOIN ")
                    stb.AppendLine("         Budget_Appezzamento ON Budget_Reg_Impianti.Id_Budget = Budget_Appezzamento.Id_Budget AND Budget_Reg_Impianti.PIVA = Budget_Appezzamento.PIVA AND Budget_Reg_Impianti.SA_COD = Budget_Appezzamento.SA_COD AND  ")
                    stb.AppendLine("         Budget_Reg_Impianti.APPEZZA = Budget_Appezzamento.APPEZZA ")

                    If IDTestataTemp <> 0 Then

                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Budget_Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Budget_Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Budget_Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)

                    End If

                    stb.AppendLine(" WHERE   (Budget_Appezzamento.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & ")  ")
                    stb.AppendLine(" AND   (Budget_Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    stb.AppendLine(" AND     (Budget_Appezzamento.SA_COD = " & Agro_SQL_SaveNum(Sa_cod) & ")  ")

                    If FlagIncludiAppezzamentiLiberi = False Then
                        stb.AppendLine(" AND     (Budget_Appezzamento.campo_cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  ")
                    Else
                        stb.AppendLine(" AND     (Budget_Appezzamento.campo_cod = " & Agro_SQL_SaveNum(Campo_Cod) & " OR Budget_Appezzamento.campo_cod =0 )  ")
                    End If

                    stb.AppendLine(" AND     (Budget_Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(DataValiditaFine) & ")  ")
                    stb.AppendLine(" AND     (Budget_Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(DataValiditaInizio) & ")  ")


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY  Budget_Appezzamento.APP_NOME ")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select

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

    '##########################################################################################

    Function Numero_Appezzamenti(ByVal Id_Budget As Integer,
                                ByVal piva As String,
                                ByVal sa_cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_R.Numero_Appezzamenti()"

        Dim strQuery As New System.Text.StringBuilder
        Dim MessaggioErrore As String = ""
        Dim Dt As DataTable
        Dim Num_Appezza As Integer = 0

        Try

            strQuery.Length = 0
            strQuery.Append(" SELECT  COUNT(*) AS NUM ")
            strQuery.Append(" FROM    Budget_Appezzamento ")
            strQuery.Append(" WHERE  ")
            strQuery.Append(" Piva = '" + Agro_SQL_SaveText(piva) + "' ")

            If Id_Budget <> 0 Then
                strQuery.Append(" AND Id_Budget = " + Agro_SQL_SaveNum(Id_Budget) + " ")
            End If

            If sa_cod <> 0 Then
                strQuery.Append(" AND Sa_Cod = " + Agro_SQL_SaveNum(sa_cod) + " ")
            End If

            Dt = EseguiQuery_Lettura(objParametri, strQuery.ToString, NomeRoutine)

            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
                Num_Appezza = CInt(Dt.Rows(0).Item("NUM"))
            Else
                Num_Appezza = 0
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Num_Appezza

    End Function

    '##########################################################################################

    Function Numero_Appezzamenti_X_Specie(ByVal Id_Budget As Integer,
                                ByVal piva As String,
                                ByVal sa_cod As Integer,
                                ByVal Veg_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_R.Numero_Appezzamenti()"

        Dim strQuery As New System.Text.StringBuilder
        Dim MessaggioErrore As String = ""
        Dim Dt As DataTable
        Dim Num_Appezza As Integer = 0

        Try

            strQuery.Length = 0
            strQuery.Append(" SELECT  COUNT(*) AS NUM ")
            strQuery.Append(" FROM    Budget_Appezzamento ")
            strQuery.Append(" INNER JOIN Budget_Reg_Impianti ON ( Budget_Appezzamento.Id_Budget = Budget_Reg_Impianti.Id_Budget and Budget_Appezzamento.Piva = Budget_Reg_Impianti.Piva and Budget_Appezzamento.Sa_Cod = Budget_Reg_Impianti.Sa_Cod and Budget_Appezzamento.Appezza = Budget_Reg_Impianti.Appezza) ")
            strQuery.Append(" INNER JOIN Cultivar ON (Cultivar.Cul_Cod = Budget_Reg_Impianti.Cul_Cod) ")
            strQuery.Append(" INNER JOIN SpecieVegetali on (Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod)")
            strQuery.Append(" WHERE  ")
            strQuery.Append(" Budget_Appezzamento.Piva = '" + Agro_SQL_SaveText(piva) + "' ")

            If Id_Budget <> 0 Then
                strQuery.Append(" AND Budget_Appezzamento.Id_Budget = " + Agro_SQL_SaveNum(Id_Budget) + " ")
            End If

            If sa_cod <> 0 Then
                strQuery.Append(" AND Budget_Appezzamento.Sa_Cod = " + Agro_SQL_SaveNum(sa_cod) + " ")
            End If
            If Veg_Cod <> 0 Then
                strQuery.Append(" AND Cultivar.Veg_Cod = " + Agro_SQL_SaveNum(Veg_Cod) + " ")
            End If

            Dt = EseguiQuery_Lettura(objParametri, strQuery.ToString, NomeRoutine)

            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
                Num_Appezza = CInt(Dt.Rows(0).Item("NUM"))
            Else
                Num_Appezza = 0
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Num_Appezza

    End Function

    '##########################################################################################

    Public Function LeggiconCampo(ByVal Id_Budget As Int32,
                                  ByVal Piva As String,
                                  ByVal Sa_Cod As Int32,
                                  ByVal CampoCod As Int32,
                                  ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  Optional ByVal IDTestataTemp As Integer = 0
                                   ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Read.LeggiconCampo()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Budget_Appezzamento.ID_BUDGET, Budget_Appezzamento.PIVA,  Budget_Appezzamento.SA_COD,  Budget_Appezzamento.APPEZZA,  Budget_Appezzamento.Campo_Cod,   Budget_Appezzamento.APP_NOME,  Budget_Appezzamento.SUP_APP,  Budget_Appezzamento.Validita_Inizio,  Budget_Appezzamento.Validita_Fine,  Budget_Appezzamento.Validazione,   Budget_Appezzamento.Blk_Flag  ")

                    stb.AppendLine(" FROM  Budget_Appezzamento ")

                    If IDTestataTemp <> 0 Then

                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Budget_Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Budget_Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Budget_Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)

                    End If

                    stb.AppendLine(" WHERE Budget_Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Budget_Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        stb.AppendLine("AND Budget_Appezzamento.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        stb.AppendLine(" AND Budget_Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Budget_Appezzamento.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Budget_Appezzamento.Campo_Cod = " & CampoCod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Budget_Appezzamento.Id_Budget, Budget_Appezzamento.PIVA,  Budget_Appezzamento.SA_COD,  Budget_Appezzamento.APPEZZA,  Budget_Appezzamento.Campo_Cod,   Budget_Appezzamento.APP_NOME,  Budget_Appezzamento.SUP_APP,  Budget_Appezzamento.Validita_Inizio,  Budget_Appezzamento.Validita_Fine,  Budget_Appezzamento.Validazione,   Budget_Appezzamento.Blk_Flag , ISNULL( C.val_cod, '') as RiferimentoAlfanumerico ")

                    stb.AppendLine(" FROM  Budget_Appezzamento ")

                    If IDTestataTemp <> 0 Then

                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Budget_Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Budget_Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Budget_Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)

                    End If

                    stb.AppendLine("  left join Budget_Appezzamento_Codici C on C.Id_Budget = Budget_Appezzamento.Id_Budget AND C.PIVA = Budget_Appezzamento.piva AND C.SA_COD = Budget_Appezzamento.SA_COD AND C.APPEZZA = Budget_Appezzamento.APPEZZA AND C.id_cod= 1104 ")

                    stb.AppendLine(" WHERE Budget_Appezzamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Budget_Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        stb.AppendLine("AND Budget_Appezzamento.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        stb.AppendLine(" AND Budget_Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Budget_Appezzamento.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Budget_Appezzamento.Campo_Cod = " & CampoCod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY  Budget_Appezzamento.App_Nome ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    stb.Length = 0
                    stb.AppendLine(" SELECT  Budget_Appezzamento.Id_Budget, Budget_Appezzamento.PIVA, Budget_Appezzamento.SA_COD, Budget_Appezzamento.APPEZZA,  ")
                    stb.AppendLine(" Budget_Appezzamento.Campo_Cod,  Budget_Appezzamento.APP_NOME, Budget_Appezzamento.SUP_APP,  ")
                    stb.AppendLine("  Budget_Appezzamento.Validita_Inizio, Budget_Appezzamento.Validita_Fine,  Budget_Appezzamento.Validazione, ")
                    stb.AppendLine("  Budget_Appezzamento.Blk_Flag ,  T.InizioImpianto, ISNULL( C.val_cod, '') as RiferimentoAlfanumerico ")

                    stb.AppendLine(" FROM  Budget_Appezzamento ")


                    If IDTestataTemp <> 0 Then

                        stb.AppendLine(" inner Join __tmp_FiltroImpianti f")
                        stb.AppendLine("On f.piva = Budget_Appezzamento.PIVA  ")
                        stb.AppendLine(" And f.sa_cod = Budget_Appezzamento.SA_COD ")
                        stb.AppendLine(" And f.appezza = Budget_Appezzamento.APPEZZA ")
                        stb.AppendLine(" And f.idTestataTemp = " & IDTestataTemp)

                    End If

                    stb.AppendLine(" inner join  ")

                    stb.AppendLine(" (Select max(Validita_Inizio) as InizioImpianto , PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" from dbo.Budget_Reg_Impianti ")
                    stb.AppendLine(" group by PIVA,SA_COD,APPEZZA ")
                    stb.AppendLine(" ) T on T.PIVA = Budget_Appezzamento.piva AND T.SA_COD = Budget_Appezzamento.SA_COD AND T.APPEZZA = Budget_Appezzamento.APPEZZA  ")

                    stb.AppendLine("  left join Budget_Appezzamento_Codici C on C.Id_Budget = Budget_Appezzamento.Id_Budget AND C.PIVA = Budget_Appezzamento.piva AND C.SA_COD = Budget_Appezzamento.SA_COD AND C.APPEZZA = Budget_Appezzamento.APPEZZA AND C.id_cod= 1104 ")

                    stb.AppendLine(" WHERE Budget_Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.AppendLine(" AND   Budget_Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        stb.AppendLine("AND Budget_Appezzamento.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        stb.AppendLine(" AND Budget_Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.AppendLine(" AND Budget_Appezzamento.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If CampoCod <> 0 Then
                        stb.AppendLine(" AND Budget_Appezzamento.Campo_Cod = " & CampoCod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Budget_Appezzamento.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY App_Nome ASC ")
                    End If


            End Select

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

    '##########################################################################################

    Public Function Leggi(ByVal Id_Budget As Integer,
                          ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByVal Leggi_Cartografia As Boolean = False
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Appezzamento_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
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
                    StrSQL.Append(" SELECT Id_Budget, Piva, Sa_Cod, Appezza, Campo_Cod, App_Nome, Sup_App, Validita_Inizio, Validita_Fine ")
                    StrSQL.Append(" FROM  Budget_Appezzamento ")
                    StrSQL.Append(" WHERE Budget_Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Appezzamento.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Budget_Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Appezzamento.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_Appezzamento.Appezza = " & Appezza & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Appezzamento.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Appezzamento.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Budget_Appezzamento.* ")

                    If Leggi_Cartografia Then
                        StrSQL.AppendLine(", ISNULL (g.Poligono_GeoEntity.STAsText(), '') as cartografia")
                    End If

                    StrSQL.Append(" FROM  Budget_Appezzamento ")

                    If Leggi_Cartografia Then
                        StrSQL.AppendLine(" LEFT JOIN gis_entita e ")
                        StrSQL.AppendLine("     ON Budget_Appezzamento.piva = e.piva ")
                        StrSQL.AppendLine("     AND Budget_Appezzamento.sa_cod = e.sa_cod ")
                        StrSQL.AppendLine("     AND Budget_Appezzamento.appezza = e.appezza ")
                        StrSQL.AppendLine("     AND e.TipoEntita_Cod = 1 ")
                        StrSQL.AppendLine("     AND e.Id_Imp = 0 ")
                        StrSQL.AppendLine(" LEFT JOIN gis_elementigrafici g ")
                        StrSQL.AppendLine("     ON e.PivaSuperUser = g.PivaSuperUser ")
                        StrSQL.AppendLine("     AND e.entita_cod = g.Entita_Cod ")
                        StrSQL.AppendLine("     AND g.LayerElementiGrafici_Cod = 1")
                    End If



                    StrSQL.Append(" WHERE Budget_Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Appezzamento.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Budget_Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Appezzamento.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_Appezzamento.Appezza = " & Appezza & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Appezzamento.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Appezzamento.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome, Budget_Appezzamento.* ")

                    StrSQL.Append(" FROM  Imprese INNER JOIN ")
                    StrSQL.Append(" Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.Piva  ")
                    StrSQL.Append(" INNER JOIN Budget_Appezzamento ON Budget_Appezzamento.PIVA = Centri_Aziendali.PIVA ")
                    StrSQL.Append(" AND Budget_Appezzamento.sa_cod = Centri_Aziendali.sa_cod ")

                    StrSQL.Append(" WHERE Budget_Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Appezzamento.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND    Budget_Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     Budget_Appezzamento.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND     Budget_Appezzamento.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Appezzamento.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Appezzamento.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc, Sa_Nome, App_nome ")
                    End If




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" select app.* ")
                    StrSQL.AppendLine(" , ( ")
                    StrSQL.AppendLine("     Select * from(select StaticMap as '*') Tbl ")
                    StrSQL.AppendLine("     For Xml path('') ")
                    StrSQL.AppendLine(" ) StaticMapBase64String")
                    StrSQL.AppendLine(", ISNULL (g.Poligono_GeoEntity.STAsText(), '') as cartografia")

                    StrSQL.AppendLine(" From Budget_Appezzamento app ")
                    StrSQL.AppendLine(" Left Join gis_entita e ")
                    StrSQL.AppendLine("     On  app.piva = e.piva ")
                    StrSQL.AppendLine("     And app.sa_cod = e.sa_cod ")
                    StrSQL.AppendLine("     And app.appezza = e.appezza    ")
                    StrSQL.AppendLine("     AND e.TipoEntita_Cod = 1    ")
                    StrSQL.AppendLine("     AND e.Id_Imp = 0    ")
                    StrSQL.AppendLine(" Left Join gis_elementigrafici g ")
                    StrSQL.AppendLine("     On   e.PivaSuperUser = g.PivaSuperUser ")
                    StrSQL.AppendLine("     And  e.entita_cod = g.Entita_Cod ")
                    StrSQL.AppendLine("     And g.LayerElementiGrafici_Cod = 1")

                    StrSQL.AppendLine(" WHERE App.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   App.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND App.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND App.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND App.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND App.Appezza = " & Appezza & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   App.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   App.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##########################################################################################

    Public Function Leggi_Max_DataModifica(ByVal Id_Budget As Integer,
                             ByVal Piva As String,
                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DateTime

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_R.Leggi_Max_DataModifica()"

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
            StrSQL.AppendLine(" FROM Budget_Appezzamento ")
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

Public Class Budget_AppezzamentixIndirizzi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal idBudget As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_AppezzamentixIndirizzi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Budget_AppezzamentixIndirizzi ")
            stb.AppendLine(" WHERE Id_Budget = " & idBudget & " ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function LeggixAppezzamento(ByVal Id_Budget As Integer,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Appezza As Integer,
                                       ByVal cod_indirizzo As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_AppezzamentixIndirizzi_R.LeggixAppezzamento()"


        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" SELECT ")
            stb.AppendLine("    AppezzamentixIndirizzi.*, ")
            stb.AppendLine("    '' as Tipo_Indirizzo_Des, ")
            stb.AppendLine("    Indirizzi.ind_des, ")
            stb.AppendLine("    Indirizzi.frz_des, ")
            stb.AppendLine("    ISNULL(ISTAT.COMUNI_PROV, '') As pro_cod, ")
            stb.AppendLine("    ISNULL(ISTAT.LOCALITA, '') as com_des, ")
            stb.AppendLine("    Indirizzi.com_cod_istat, ")
            stb.AppendLine("    Indirizzi.pro_cod_istat, ")
            stb.AppendLine("    Indirizzi.note, ")
            stb.AppendLine("    Indirizzi.CAP, ")
            stb.AppendLine("    Indirizzi.stato, ")
            stb.AppendLine("    ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.descrizione as Stato_Des ")
            stb.AppendLine(" FROM Indirizzi ")
            stb.AppendLine(" JOIN Budget_AppezzamentixIndirizzi AppezzamentixIndirizzi On Indirizzi.cod_indirizzo = AppezzamentixIndirizzi.cod_indirizzo ")
            stb.AppendLine(" LEFT JOIN ISTAT On Indirizzi.pro_cod_istat = ISTAT.PROV And Indirizzi.com_cod_istat = ISTAT.COM ")
            stb.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 On Indirizzi.stato = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")

            stb.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                stb.AppendLine(" AND AppezzamentixIndirizzi.Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" And AppezzamentixIndirizzi.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                stb.AppendLine(" And AppezzamentixIndirizzi.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If cod_indirizzo <> 0 Then
                stb.AppendLine(" And AppezzamentixIndirizzi.cod_indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
            End If

            If Id_Budget <> 0 Then
                stb.AppendLine(" And AppezzamentixIndirizzi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
End Class

Public Class Budget_Appezzamento_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal idBudget As Integer,
                                               ByVal Piva As String,
                                               ByVal Sa_Cod As Int32,
                                               ByVal Appezza As Int32,
                                               ByVal Sup_App As Decimal,
                                               ByVal Data_App As Date,
                                               ByVal Ep_Camp As Date,
                                               ByVal X As Decimal,
                                               ByVal Y As Decimal,
                                               ByVal ZSLM As Decimal,
                                               ByVal Esposiz As String,
                                               ByVal Pende As Decimal,
                                               ByVal Ubicazione As String,
                                               ByVal Num_Del As Int32,
                                               ByVal Clas As String,
                                               ByVal Sabbia As Decimal,
                                               ByVal Limo As Decimal,
                                               ByVal Argilla As Decimal,
                                               ByVal pH As Decimal,
                                               ByVal CalTot As Decimal,
                                               ByVal CalAtt As Decimal,
                                               ByVal SostOrg As Decimal,
                                               ByVal K2OAss As Decimal,
                                               ByVal P2O5Ass As Decimal,
                                               ByVal Mg As Decimal,
                                               ByVal Ntot As Decimal,
                                               ByVal UM_S As Decimal,
                                               ByVal CL_Dren As String,
                                               ByVal Falda As Int32,
                                               ByVal CSC As Decimal,
                                               ByVal K2OAss_Data As Date,
                                               ByVal MatOrg As Decimal,
                                               ByVal MatOrg_Data As Date,
                                               ByVal NOTot As Decimal,
                                               ByVal NOTot_Data As Date,
                                               ByVal P2O5Ass_Data As Date,
                                               ByVal Suolo_CodAttri As String,
                                               ByVal PivaSuperuser As String,
                                               ByVal App_Nome As String,
                                               ByVal Campo_Spia As Int16,
                                               ByVal Campo_Spia_Area As Decimal,
                                               ByVal CS_Sipi As String,
                                               ByVal Campo_Cod As Int32,
                                               ByVal Data_Inizio As Date,
                                               ByVal Data_Fine As Date,
                                               ByVal Prossimo As Int32,
                                               ByVal Validita_Inizio As Date,
                                               ByVal Validita_Fine As Date,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                                                , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                                                , Optional ByVal username_creazione As String = "" _
                                                , Optional ByVal username_modifica As String = "" _
                                                , Optional ByVal blk_flag As Integer = 0 _
                                                , Optional ByVal blk_inizio_data As Date = #2/1/1900# _
                                                , Optional ByVal blk_inizio_username As String = "" _
                                                , Optional ByVal blk_inizio_note As String = "" _
                                                , Optional ByVal blk_fine_data As Date = #12/30/2100# _
                                                , Optional ByVal blk_fine_username As String = "" _
                                                , Optional ByVal blk_fine_note As String = "" _
                                                , Optional ByVal via_stringa As String = "" _
                                                , Optional ByVal BZ_CorpiIdrici As Decimal = 0 _
                                                , Optional ByVal BZ_AreeResPub As Decimal = 0 _
                                                , Optional ByRef BZ_Allevamenti As Decimal = 0 _
                                                , Optional ByRef BZ_VegNatNonColt As Decimal = 0 _
                                                , Optional ByRef BZ_SupRiduzione As Decimal = 0
                                             ) As Boolean

        If blk_inizio_data = #2/1/1900# Then
            blk_inizio_data = Now.Date
        End If

        If blk_fine_data = #12/30/2100# Then
            blk_fine_data = Now.Date
        End If

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_W.Scrivi_Budget_Appezzamenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

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

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Budget_Appezzamento ( Id_Budget ")
            StrSQL.Append("                    ,Piva         ,Sa_Cod        ,Appezza       ,Sup_App          ")
            StrSQL.Append("                    ,Data_App     ,Ep_Camp       ,X             ,Y                ")
            StrSQL.Append("                    ,Zslm         ,Esposiz       ,Pende         ,Ubicazione       ")
            StrSQL.Append("                    ,Num_Del      ,Clas          ,Sabbia        ,Limo             ")
            StrSQL.Append("                    ,Argilla      ,pH            ,CalTot        ,CalAtt           ")
            StrSQL.Append("                    ,SostOrg      ,K2OAss        ,P2O5Ass       ,Mg               ")
            StrSQL.Append("                    ,Ntot         ,UM_S          ,CL_Dren       ,Falda            ")
            StrSQL.Append("                    ,CSC          ,K2OAss_Data   ,MatOrg        ,MatOrg_Data      ")
            StrSQL.Append("                    ,NOTot_Data   ,NOTot         ,P2O5Ass_Data  ,Suolo_CodAttri   ")
            StrSQL.Append("                    ,[User]       ,App_Nome      ,Campo_Spia    ,Campo_Spia_Area  ")
            StrSQL.Append("                    ,Cs_Sipi      ,Campo_Cod     ,Data_Inizio   ,Data_Fine        ")
            StrSQL.Append("                    ,Prossimo     ")

            StrSQL.Append("                    ,blk_flag     ,blk_inizio_data,blk_inizio_username, blk_inizio_note ")
            StrSQL.Append("                    ,blk_fine_data     ,blk_fine_username,blk_fine_note   ")

            StrSQL.Append("                    ,via_stringa     ,DistBZ_CorpiIdrici , DistBZ_AreeResPub  ")
            StrSQL.Append("                    ,DistBZ_Allevamenti   ,DistBZ_VegNatNonColt , SupBZ_Riduzione  ")


            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(idBudget) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sup_App) & "  ")
            StrSQL.Append("         , " & IIf(Data_App = New Date, "Null", Agro_SQL_SaveDate(Data_App)) & "  ")
            StrSQL.Append("         , " & IIf(Ep_Camp = New Date, "Null", Agro_SQL_SaveDate(Ep_Camp)) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(X) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Y) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ZSLM) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Esposiz) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pende) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Ubicazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Num_Del) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Clas) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sabbia) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Limo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Argilla) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(pH) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(CalTot) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(CalAtt) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(SostOrg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(K2OAss) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(P2O5Ass) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ntot) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(UM_S) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(CL_Dren) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Falda) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(CSC) & "  ")
            StrSQL.Append("         , " & IIf(K2OAss_Data = New Date, "Null", Agro_SQL_SaveDate(K2OAss_Data)) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(MatOrg) & "  ")
            StrSQL.Append("         , " & IIf(MatOrg_Data = New Date, "Null", Agro_SQL_SaveDate(MatOrg_Data)) & "  ")
            StrSQL.Append("         , " & IIf(NOTot_Data = New Date, "Null", Agro_SQL_SaveDate(NOTot_Data)) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(NOTot) & "  ")
            StrSQL.Append("         , " & IIf(P2O5Ass_Data = New Date, "Null", Agro_SQL_SaveDate(P2O5Ass_Data)) & "  ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Suolo_CodAttri) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(App_Nome) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Spia) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Spia_Area) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(CS_Sipi) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         , " & IIf(CStr(Data_Inizio) = New Date, "Null", Agro_SQL_SaveDate(Data_Inizio)) & "  ")
            StrSQL.Append("         , " & IIf(CStr(Data_Fine) = New Date, "Null", Agro_SQL_SaveDate(Data_Fine)) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prossimo) & "  ")



            StrSQL.Append("         , " & Agro_SQL_SaveNum(blk_flag) & "  ")
            StrSQL.Append("         ,  " & IIf(CStr(blk_inizio_data) = New Date, "Null", Agro_SQL_SaveDate(blk_inizio_data)) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(blk_inizio_username) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(blk_inizio_note) & "'  ")

            StrSQL.Append("         ,  " & IIf(CStr(blk_fine_data) = New Date, "Null", Agro_SQL_SaveDate(blk_fine_data)) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(blk_fine_username) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(blk_fine_note) & "'  ")

            StrSQL.Append("         , '" & Agro_SQL_SaveText(via_stringa) & "'  ")


            StrSQL.Append("         , " & Agro_SQL_SaveNum(BZ_CorpiIdrici) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(BZ_AreeResPub) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(BZ_Allevamenti) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(BZ_VegNatNonColt) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(BZ_SupRiduzione) & "  ")


            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
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

    Public Function Cancella(ByVal IdBudget As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_W.EliminaDaBudget()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_Appezzamento ")
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
                                                ByVal Sa_Cod As Long,
                                                ByVal Id_Campo As Long,
                                                ByVal Appezza As Long,
                                                    ByVal Validita_Inizio As Date,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_W.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '   Id_Campo = 0         =>  aggiorna tutti gli appezzamenti del centro aziendale
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
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
            StrSQL.Append("UPDATE Budget_Appezzamento SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Id_Budget <> 0 Then
                StrSQL.Append("  AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND   Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '---------------------------------------------
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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function

    Public Function AggiornaValiditaFine(ByVal Id_Budget As Integer,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Long,
                                            ByVal Id_Campo As Long,
                                            ByVal Appezza As Long,
                                                ByVal Validita_Fine As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  aggiorna tutti gli appezzamenti del centro aziendale
        '   Id_Campo = 0         =>  aggiorna tutti gli appezzamenti del centro aziendale
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
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
            StrSQL.Append("UPDATE Budget_Appezzamento SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Id_Budget <> 0 Then
                StrSQL.Append("  AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND   Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            '---------------------------------------------
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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function


End Class

Public Class Budget_AppezzamentixIndirizzi_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(ByVal idBudget As Integer,
                                                         ByVal Piva As String,
                                                         ByVal Sa_Cod As Int32,
                                                         ByVal Appezza As Int32,
                                                         ByVal Cod_Indirizzo As Integer,
                                                         ByVal Tipo_Indirizzo As Integer,
                                                         ByVal Validita_Inizio As Date,
                                                         ByVal Validita_Fine As Date,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                         , Optional ByVal Data_creazione As Date = #2/1/1900# _
                                                         , Optional ByVal Data_modifica As Date = #2/1/1900# _
                                                         , Optional ByVal username_creazione As String = "" _
                                                         , Optional ByVal username_modifica As String = ""
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Write.Scrivi_Budget_AppezzamentixIndirizzi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Budget_AppezzamentixIndirizzi( id_Budget,      ")
            StrSQL.Append("                    PIVA, Sa_Cod, Appezza, Cod_Indirizzo, ")
            StrSQL.Append("                    Tipo_Indirizzo, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(idBudget) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Indirizzo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
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

    Public Function Cancella(ByVal IdBudget As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Write.EliminaDaBudget()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_AppezzamentixIndirizzi ")
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