Imports System.Data.Entity
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.DataProviderExtensions
Public Class Budget_Reg_Impianti_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Budget As Int32,
                               ByVal PIVA As String,
                               ByVal Sa_Cod As Int32,
                               ByVal Appezza As Int32,
                               ByVal Id_Reg As Int32,
                               ByVal Gruppo As String,
                               ByVal Id_Cod As Int32,
                               ByVal Val_Cod As String,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = "" 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT DISTINCT Id_Budget, PIVA, sa_cod, appezza, Id_Reg, id_cod, val_cod, Validita_Inizio, Validita_Fine ")
                    StrSQL.Append(" FROM  Budget_Reg_Impianti_Codici ")
                    StrSQL.Append(" WHERE Budget_Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Progetto_Cod = 0 ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Budget_Reg_Impianti_Codici.Id_Budget, Budget_Reg_Impianti_Codici.Piva, Budget_Reg_Impianti_Codici.Sa_Cod, Budget_Reg_Impianti_Codici.Appezza, Budget_Reg_Impianti_Codici.Id_reg, Budget_Reg_Impianti_Codici.Id_Cod ASC ")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Distinct *,  Budget_Reg_Impianti_Codici.Validita_Inizio as xValidita_Inizio, Budget_Reg_Impianti_Codici.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  Budget_Reg_Impianti_Codici, Codici_Anagrafe ")
                    StrSQL.Append(" WHERE Budget_Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Id_Cod = Codici_Anagrafe.Codice ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Progetto_Cod = 0 ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.Append(" AND Codici_Anagrafe.Gruppo = '" & Agro_SQL_SaveText(Trim(Gruppo)) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato >= 0 ")
                            StrSQL.Append(" AND     Codici_Anagrafe.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato = -1 ")
                            StrSQL.Append(" AND     Codici_Anagrafe.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Budget_Reg_Impianti_Codici.Id_Budget, Budget_Reg_Impianti_Codici.Piva, Budget_Reg_Impianti_Codici.Sa_Cod, Budget_Reg_Impianti_Codici.Appezza, Budget_Reg_Impianti_Codici.Id_reg, Budget_Reg_Impianti_Codici.Id_Cod ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    ' Estrapolo tutti i record Codici dell'impianto (non quelli del'azienda, Reg_Impianti_Codici.Progetto_Cod = 0)

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Distinct *,  Budget_Reg_Impianti_Codici.Validita_Inizio as xValidita_Inizio, Budget_Reg_Impianti_Codici.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  Budget_Reg_Impianti_Codici, Codici_Anagrafe ")
                    StrSQL.Append(" WHERE Budget_Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Id_Cod = Codici_Anagrafe.Codice ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Progetto_Cod <> 0 ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.Append(" AND Codici_Anagrafe.Gruppo = '" & Agro_SQL_SaveText(Trim(Gruppo)) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato >= 0 ")
                            StrSQL.Append(" AND     Codici_Anagrafe.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato = -1 ")
                            StrSQL.Append(" AND     Codici_Anagrafe.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Budget_Reg_Impianti_Codici.Id_Budget, Budget_Reg_Impianti_Codici.Piva, Budget_Reg_Impianti_Codici.Sa_Cod, Budget_Reg_Impianti_Codici.Appezza, Budget_Reg_Impianti_Codici.Id_reg, Budget_Reg_Impianti_Codici.Id_Cod ASC ")
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

    Public Function LeggixProgetto(ByVal Id_Budget As Int32,
                                       ByVal PIVA As String,
                                       ByVal Sa_Cod As Int32,
                                       ByVal Appezza As Int32,
                                       ByVal Id_Reg As Int32,
                                       ByVal Gruppo As String,
                                       ByVal Progetto_Cod As Int32,
                                       ByVal Id_Cod As Int32,
                                       ByVal Val_Cod As String,
                                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_R.LeggixProgetto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Id_Cod = 0 
        '   Progetto_Cod = 0 
        '   Val_Cod = "" 
        '   Gruppo = ""         '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT id_cod, val_cod, progetto_cod ")
                    StrSQL.Append(" FROM  Budget_Reg_Impianti_Codici ")
                    StrSQL.Append(" WHERE Budget_Reg_Impianti_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Progetto_Cod <> 0 ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Progetto_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Progetto_Cod = " & Progetto_Cod & " ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'StrSQL.Append(" ORDER BY Reg_Impianti_Codici.Piva, Reg_Impianti_Codici.Sa_Cod, Reg_Impianti_Codici.Appezza, Reg_Impianti_Codici.Id_reg, Reg_Impianti_Codici.Id_Cod ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Distinct *,  Budget_Imprese_Progetti.Validita_Inizio as xValidita_Inizio, Budget_Imprese_Progetti.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  Budget_Imprese_Progetti, Budget_Reg_Impianti_Codici, Codici_Anagrafe ")
                    StrSQL.Append(" WHERE Budget_Imprese_Progetti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Imprese_Progetti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Id_Budget = Budget_Imprese_Progetti.Id_Budget ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Piva = Budget_Imprese_Progetti.Piva ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Sa_Cod = Budget_Imprese_Progetti.Sa_Cod ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Appezza = Budget_Imprese_Progetti.Appezza ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Id_Reg = Budget_Imprese_Progetti.Id_Reg ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Progetto_Cod = Budget_Imprese_Progetti.Progetto_Cod ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Id_Cod = Codici_Anagrafe.Codice ")
                    StrSQL.Append(" AND   Budget_Reg_Impianti_Codici.Progetto_Cod <> 0 ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Progetto_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Progetto_Cod = " & Progetto_Cod & " ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Budget_Reg_Impianti_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.Append(" AND Codici_Anagrafe.Gruppo = '" & Agro_SQL_SaveText(Trim(Gruppo)) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato >= 0 ")
                            StrSQL.Append(" AND     Codici_Anagrafe.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Budget_Reg_Impianti_Codici.Inviato = -1 ")
                            StrSQL.Append(" AND     Codici_Anagrafe.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Budget_Reg_Impianti_Codici.Id_Budget, Budget_Reg_Impianti_Codici.Piva, Budget_Reg_Impianti_Codici.Sa_Cod, Budget_Reg_Impianti_Codici.Appezza, Budget_Reg_Impianti_Codici.Id_reg, Budget_Reg_Impianti_Codici.Id_Cod ASC ")

                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


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

End Class
Public Class Budget_Reg_Impianti_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal IdBudget As Integer,
                                                      ByVal Piva As String,
                                                      ByVal Sa_Cod As Int32,
                                                      ByVal Appezza As Int32,
                                                      ByVal Id_Reg As Int32,
                                                      ByVal Id_Cod As Int32,
                                                      ByVal Val_Cod As String,
                                                      ByVal Validita_Inizio As Date,
                                                      ByVal Validita_Fine As Date,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                      , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                                                      , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                                                      , Optional ByVal username_creazione As String = "" _
                                                      , Optional ByVal username_modifica As String = ""
                                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Budget_Reg_Impianti_Codici( Id_Budget, ")
            StrSQL.Append("                    Piva,        ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Appezza,      ")
            StrSQL.Append("                    Id_Reg,      ")
            StrSQL.Append("                    Id_Cod,      ")
            StrSQL.Append("                    Val_Cod,     ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(IdBudget) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
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

    Public Function ScrivixProgetto(ByVal Id_Budget As Integer,
                                              ByVal piva As String,
                                              ByVal saCod As Integer,
                                              ByVal appezza As Integer,
                                              ByVal idReg As Integer,
                                              ByVal progettoCod As Integer,
                                              ByVal idCod As Integer,
                                              ByVal valCod As String,
                                              ByVal validita_inizio As Date,
                                              ByVal validita_fine As Date,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                              Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                              Optional ByVal username_creazione As String = "",
                                              Optional ByVal username_modifica As String = "") As Boolean

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_W.ScriviModificaEliminaxProgetto()"
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
            StrSQL.Append("INSERT INTO Budget_Reg_Impianti_Codici( Id_Budget, ")
            StrSQL.Append("                    Piva,        ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Appezza,      ")
            StrSQL.Append("                    Id_Reg,      ")
            StrSQL.Append("                    Progetto_Cod,      ")
            StrSQL.Append("                    Id_Cod,      ")
            StrSQL.Append("                    Val_Cod,     ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(Id_Budget) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(saCod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(idReg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(progettoCod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(idCod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(valCod) & "' ")

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(validita_fine) & "  ")
            StrSQL.Append(")")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

    Public Function Cancella(ByVal IdBudget As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_Reg_Impianti_Codici ")
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
