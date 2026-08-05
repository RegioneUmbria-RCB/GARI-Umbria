Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text
Imports AgronicaCoreDataProvider.DataProviderExtensions
Public Class Budget_AppezzaxParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiParticelle_Da_Appezzamento(ByVal Id_Budget As Integer,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Long,
                                                    ByVal Appezza As Long,
                                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.AppezzaxParticelle_R.LeggiParticelle_Da_Appezzamento()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Budget_AppezzamentixParticelle.* ")
                    StrSQL.Append(" FROM   Budget_AppezzamentixParticelle ")
                    StrSQL.Append(" WHERE Budget_AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append("AND Budget_AppezzamentixParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Budget_AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_AppezzamentixParticelle.Inviato =-1 ")
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


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Budget_AppezzamentixParticelle.* , ParticelleCatastali.*  , Budget_Appezzamento.Campo_Cod , ISTAT.LOCALITA AS Comune , ISTAT.COMUNI_PROV As Provincia ")
                    StrSQL.AppendLine(" FROM   Budget_AppezzamentixParticelle , ParticelleCatastali , Budget_Appezzamento, ISTAT  ")

                    StrSQL.AppendLine(" WHERE Budget_AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Id_Budget = Budget_Appezzamento.Id_Budget ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Piva = Budget_Appezzamento.Piva ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Sa_Cod = Budget_Appezzamento.Sa_Cod ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Appezza = Budget_Appezzamento.Appezza ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Com = ISTAT.COM ")
                    StrSQL.AppendLine(" AND   Budget_AppezzamentiXParticelle.prov = ISTAT.PROV ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append("AND Budget_AppezzamentixParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Budget_AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Budget_AppezzamentixParticelle.Appezza ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '



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

    Public Function LeggiAppezzamenti_Da_Particella(ByVal Id_Budget As Int32,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Long,
                                                    ByVal Appezza As Long,
                                                    ByVal Id_Reg As Long,
                                                    ByVal PROV As String,
                                                    ByVal COM As String,
                                                    ByVal SEZIONE As String,
                                                    ByVal FOGLIO As Int32,
                                                    ByVal NUMERO As Int32,
                                                    ByVal SUBALTERNO As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_R.LeggiAppezzamenti_Da_Particella()"

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

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT   ")
            StrSQL.AppendLine(" Imprese.rag_soc, ")
            StrSQL.AppendLine(" Centri_Aziendali.sa_nome, ")
            StrSQL.AppendLine(" Budget_Appezzamento.Appezza, ")
            StrSQL.AppendLine(" Budget_Reg_Impianti.Id_Reg, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.Id_Budget, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.Piva, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.SA_COD, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.PROV, ")
            StrSQL.AppendLine(" ISTAT.COMUNI_PROV, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.COM, ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.SEZIONE, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.FOGLIO, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.NUMERO, ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.SUBALTERNO, ")
            StrSQL.AppendLine(" ISNULL(Budget_Campi.Campo_Des, '') As Campo_Des, ")
            StrSQL.AppendLine(" Budget_Appezzamento.APP_NOME,  ")
            StrSQL.AppendLine(" Budget_Appezzamento.SUP_APP,  ")
            StrSQL.AppendLine(" Budget_Reg_Impianti.Validita_Inizio,  ")
            StrSQL.AppendLine(" Budget_Reg_Impianti.Validita_Fine,  ")
            StrSQL.AppendLine(" Budget_AppezzamentixParticelle.AREA,  ")
            StrSQL.AppendLine(" Cultivar.Cul_Des,  ")
            StrSQL.AppendLine(" SpecieVegetali.Veg_Des, ")
            StrSQL.AppendLine(" dest.descrizione, ")
            StrSQL.AppendLine(" CASE WHEN Budget_Reg_Impianti.Cul_Cod = 0  ")
            StrSQL.AppendLine(" THEN CASE WHEN destinazione.id_cod > 0 THEN dest.descrizione ELSE 'Terreno Nudo' END ")
            StrSQL.AppendLine(" ELSE SpecieVegetali.Veg_Des + ' - ' + Cultivar.Cul_Des ")
            StrSQL.AppendLine(" END as Utilizzo, ")
            StrSQL.AppendLine(" CASE WHEN Budget_Appezzamento.Validita_Inizio < GETDATE() AND Budget_Appezzamento.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo, ")
            StrSQL.AppendLine(" CASE WHEN ZonexParticelle.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN ")
            StrSQL.AppendLine(" FROM Budget_AppezzamentixParticelle ")
            StrSQL.AppendLine(" JOIN Budget_Appezzamento ON Budget_AppezzamentixParticelle.Id_Budget = Budget_Appezzamento.Id_Budget ")
            StrSQL.AppendLine("     AND Budget_AppezzamentixParticelle.Piva = Budget_Appezzamento.Piva ")
            StrSQL.AppendLine(" 	AND Budget_AppezzamentixParticelle.Sa_Cod = Budget_Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Budget_AppezzamentixParticelle.Appezza = Budget_Appezzamento.Appezza ")
            StrSQL.AppendLine(" LEFT JOIN Budget_Campi ON Budget_Appezzamento.Id_Budget = Budget_Campi.Id_Budget ")
            StrSQL.AppendLine("                   AND Budget_Appezzamento.Piva = Budget_Campi.Piva ")
            StrSQL.AppendLine("                   AND Budget_Appezzamento.Sa_Cod = Budget_Campi.Sa_Cod ")
            StrSQL.AppendLine("                   AND Budget_Appezzamento.Campo_Cod = Budget_Campi.Campo_Cod ")
            StrSQL.AppendLine(" JOIN Budget_Reg_Impianti ON Budget_Reg_Impianti.Id_Budget = Budget_Appezzamento.Id_Budget ")
            StrSQL.AppendLine(" 				AND Budget_Reg_Impianti.Piva = Budget_Appezzamento.Piva ")
            StrSQL.AppendLine(" 				AND Budget_Reg_Impianti.Sa_Cod = Budget_Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" 				AND Budget_Reg_Impianti.Appezza = Budget_Appezzamento.Appezza ")
            StrSQL.AppendLine(" JOIN ISTAT ON Budget_AppezzamentixParticelle.Prov = ISTAT.Prov  ")
            StrSQL.AppendLine(" 		AND Budget_AppezzamentixParticelle.Com = ISTAT.Com ")
            StrSQL.AppendLine(" JOIN Imprese ON Budget_AppezzamentixParticelle.Piva = imprese.PIVA ")
            StrSQL.AppendLine(" JOIN Centri_Aziendali ON Budget_AppezzamentixParticelle.Piva = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" 					AND	Budget_AppezzamentixParticelle.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = Budget_Reg_Impianti.CUL_COD ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Budget_Reg_Impianti_Codici destinazione ON Budget_Reg_Impianti.Id_Budget = destinazione.Id_Budget ")
            StrSQL.AppendLine(" 				AND Budget_Reg_Impianti.Piva = destinazione.Piva ")
            StrSQL.AppendLine(" 				AND Budget_Reg_Impianti.Sa_Cod = destinazione.Sa_Cod ")
            StrSQL.AppendLine(" 				AND Budget_Reg_Impianti.Appezza = destinazione.Appezza ")
            StrSQL.AppendLine(" 				AND Budget_Reg_Impianti.ID_REG = destinazione.Id_Reg ")
            StrSQL.AppendLine(" 				AND destinazione.id_cod >= 3000 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe dest ON destinazione.id_cod = dest.codice ")
            StrSQL.AppendLine(" LEFT JOIN ZonexParticelle ON Budget_AppezzamentixParticelle.PROV = ZonexParticelle.PROV ")
            StrSQL.AppendLine("             AND Budget_AppezzamentixParticelle.COM = ZonexParticelle.COM ")
            StrSQL.AppendLine("             AND Budget_AppezzamentixParticelle.SEZIONE = ZonexParticelle.SEZIONE ")
            StrSQL.AppendLine("             AND Budget_AppezzamentixParticelle.FOGLIO = ZonexParticelle.FOGLIO ")
            StrSQL.AppendLine("             AND Budget_AppezzamentixParticelle.NUMERO = ZonexParticelle.NUMERO ")
            StrSQL.AppendLine("             AND Budget_AppezzamentixParticelle.SUBALTERNO = ZonexParticelle.SUBALTERNO ")
            StrSQL.AppendLine("             AND ZonexParticelle.Zona_Cod = " & enum_Zone.ZVN & " ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Budget_Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Prov = '" & Agro_SQL_SaveText(PROV) & "'  ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Com = '" & Agro_SQL_SaveText(COM) & "'  ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'  ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Foglio = " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Numero = " & Agro_SQL_SaveNum(NUMERO) & "  ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND Budget_AppezzamentixParticelle.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Budget_AppezzamentixParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Budget_AppezzamentixParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Budget_AppezzamentixParticelle.Appezza ASC")
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

    Public Function Leggi(ByVal idBudget As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_R.Leggi_Budget_AppezzamentixParticelle()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM  Budget_AppezzamentiXParticelle ")
            StrSQL.Append(" WHERE Id_Budget = " & idBudget & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

Public Class Budget_AppezzaxParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal idBudget As Integer,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Int32,
                                                ByVal Appezza As Int32,
                                                ByVal PROV As String,
                                                ByVal COM As String,
                                                ByVal SEZIONE As String,
                                                ByVal FOGLIO As Int32,
                                                ByVal NUMERO As Int32,
                                                ByVal SUBALTERNO As String,
                                                ByVal Area As Decimal,
                                                ByVal SAU_Convenz_Ettari As Decimal,
                                                ByVal SAU_Convenz_Are As Int32,
                                                ByVal SAU_Convenz_Centiare As Int32,
                                                ByVal SAU_Convers_Ettari As Decimal,
                                                ByVal SAU_Convers_Are As Int32,
                                                ByVal SAU_Convers_Centiare As Int32,
                                                ByVal SAU_Bio_Ettari As Decimal,
                                                ByVal SAU_Bio_Are As Int32,
                                                ByVal SAU_Bio_Centiare As Int32,
                                                ByVal Validita_Inizio As Date,
                                                ByVal Validita_Fine As Date,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                                                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                                                , Optional ByVal username_creazione As String = "" _
                                                , Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_W.Scrivi_Budget_AppezzaxPart()"

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
            StrSQL.Append("INSERT INTO Budget_AppezzamentixParticelle(  Id_Budget,     ")
            StrSQL.Append("                    PIVA, Sa_Cod, Appezza, PROV, COM, SEZIONE, ")
            StrSQL.Append("                    FOGLIO, NUMERO, SUBALTERNO,  AREA,     ")
            StrSQL.Append("                    SAU_Convenz_Ettari, SAU_Convenz_Are, SAU_Convenz_Centiare, ")
            StrSQL.Append("                    SAU_Convers_Ettari, SAU_Convers_Are, SAU_Convers_Centiare, ")
            StrSQL.Append("                    SAU_Bio_Ettari,     SAU_Bio_Are,     SAU_Bio_Centiare, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(idBudget) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Area) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Centiare) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Centiare) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Centiare) & "   ")
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

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_W.EliminaDaBudget()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_AppezzamentiXParticelle ")
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
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Campo As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal PROV As String,
                                        ByVal COM As String,
                                        ByVal SEZIONE As String,
                                        ByVal FOGLIO As Int32,
                                        ByVal NUMERO As Int32,
                                        ByVal SUBALTERNO As String,
                                            ByVal Validita_Inizio As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutte le particelle dell'impresa
        '   Appezza = 0          =>  aggiorna tutte le particelle del centro aziendale
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
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
            StrSQL.Append("UPDATE Budget_AppezzamentixParticelle SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Id_Budget <> 0 Then
                StrSQL.Append(" AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND  Appezza IN ( Select Budget_Appezzamento.Appezza From Budget_Appezzamento ")
                StrSQL.Append(" Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo))
                StrSQL.Append(" And   Id_Budget    = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                StrSQL.Append(" And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                StrSQL.Append(" And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
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
                                        ByVal Sa_Cod As Int32,
                                        ByVal Id_Campo As Integer,
                                        ByVal Appezza As Int32,
                                        ByVal PROV As String,
                                        ByVal COM As String,
                                        ByVal SEZIONE As String,
                                        ByVal FOGLIO As Int32,
                                        ByVal NUMERO As Int32,
                                        ByVal SUBALTERNO As String,
                                            ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutte le particelle dell'impresa
        '   Appezza = 0          =>  aggiorna tutte le particelle del centro aziendale
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
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
            StrSQL.Append("UPDATE Budget_AppezzamentixParticelle SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Id_Budget <> 0 Then
                StrSQL.Append(" AND   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND  Appezza IN ( Select Budget_Appezzamento.Appezza From Budget_Appezzamento ")
                StrSQL.Append(" Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo))
                StrSQL.Append(" And   Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                StrSQL.Append(" And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                StrSQL.Append(" And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            End If

            '----------------------------------------------------------------------
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