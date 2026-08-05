Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Public Class Budget_Appezzamento_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Budget As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Id_Cod As Int32,
                            ByVal Val_Cod As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal leggiValCodPerLike As Boolean = True
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Appezzamento_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
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
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    stb.Length = 0
                    stb.Append(" SELECT Distinct *,  Budget_Appezzamento_Codici.Validita_Inizio as xValidita_Inizio, Budget_Appezzamento_Codici.Validita_Fine as xValidita_Fine ")
                    stb.Append(" FROM  Budget_Appezzamento_Codici, Codici_Anagrafe ")
                    stb.Append(" WHERE Budget_Appezzamento_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Budget_Appezzamento_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Budget_Appezzamento_Codici.Id_Cod = Codici_Anagrafe.Codice ")

                    If Id_Budget <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Appezza = " & Appezza & " ")
                    End If

                    If Id_Cod <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        If leggiValCodPerLike Then
                            stb.Append(" AND Budget_Appezzamento_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                        Else
                            stb.Append(" AND Budget_Appezzamento_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
                        End If
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   Budget_Appezzamento_Codici.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   Budget_Appezzamento_Codici.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0
                    stb.Append(" SELECT Distinct *,  Budget_Appezzamento_Codici.Validita_Inizio as xValidita_Inizio, Budget_Appezzamento_Codici.Validita_Fine as xValidita_Fine ")
                    stb.Append(" ,Budget_Appezzamento_Codici.username_creazione as Budget_Appezzamento_Codici_username_creazione,Budget_Appezzamento_Codici.Username_modifica as Budget_Appezzamento_Codici_Username_modifica,Budget_Appezzamento_Codici.data_creazione as Budget_Appezzamento_Codici_data_creazione,Budget_Appezzamento_Codici.data_modifica as Budget_Appezzamento_Codici_data_modifica ")
                    stb.Append(" FROM  Budget_Appezzamento_Codici, Codici_Anagrafe ")
                    stb.Append(" WHERE Budget_Appezzamento_Codici.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Budget_Appezzamento_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Budget_Appezzamento_Codici.Id_Cod = Codici_Anagrafe.Codice ")

                    If Id_Budget <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Appezza = " & Appezza & " ")
                    End If

                    If Id_Cod <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   Budget_Appezzamento_Codici.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   Budget_Appezzamento_Codici.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    stb.Length = 0
                    stb.AppendLine(" Select distinct ")
                    stb.AppendLine("       Budget_Appezzamento_Codici.id_cod ")
                    stb.AppendLine("  , Budget_Appezzamento_Codici.val_cod ")
                    stb.AppendLine("  , Codici_Anagrafe.Descrizione ")
                    stb.AppendLine("  , case when  des1.val_cod_2 Is null then '' else des1.val_cod_2 end as val_cod_2 ")
                    stb.AppendLine("  , Budget_Appezzamento_Codici.Validita_Inizio ")
                    stb.AppendLine("  , Budget_Appezzamento_Codici.Validita_Fine  ")
                    stb.AppendLine("  , Budget_Appezzamento_Codici.username_creazione as Budget_Appezzamento_Codici_username_creazione ")
                    stb.AppendLine("  , Budget_Appezzamento_Codici.Username_modifica as Budget_Appezzamento_Codici_Username_modifica ")
                    stb.AppendLine("  , Budget_Appezzamento_Codici.data_creazione as Budget_Appezzamento_Codici_data_creazione ")
                    stb.AppendLine("  , Budget_Appezzamento_Codici.data_modifica as Budget_Appezzamento_Codici_data_modifica   ")
                    stb.AppendLine("  ")
                    stb.AppendLine("  From Budget_Appezzamento_Codici ")
                    stb.AppendLine("     inner Join Codici_Anagrafe  ")
                    stb.AppendLine("         On   Budget_Appezzamento_Codici.Id_Cod = Codici_Anagrafe.Codice   ")
                    stb.AppendLine("  Left Join( ")
                    stb.AppendLine("     select c.codice as id_cod, cast(veg_cod As varchar(50)) + '|' + cast(v.gru_cod as varchar(50)) as val_Cod , veg_des + '(' + gru_des + ')' as val_cod_2 ")
                    stb.AppendLine("        From SpecieVegetali v ")
                    stb.AppendLine("     inner Join GruppoVegetale vv ")
                    stb.AppendLine("          On vv.Gru_Cod = v.Gru_Cod ")
                    stb.AppendLine("     inner Join( ")
                    stb.AppendLine("            select codice ")
                    stb.AppendLine("         From Codici_Anagrafe ")
                    stb.AppendLine("            Where descrizione Like '%coltura precedente%' ")
                    stb.AppendLine("            And gruppo = 'APPEZZA' ")
                    stb.AppendLine("     ) c on 1=1 ")
                    stb.AppendLine("  ) des1 ")
                    stb.AppendLine("  On des1.val_cod = Budget_Appezzamento_Codici.val_cod ")
                    stb.AppendLine("  And des1.id_cod = Budget_Appezzamento_Codici.id_cod")

                    stb.Append(" WHERE Budget_Appezzamento_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Budget_Appezzamento_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Id_Budget = " & Id_Budget & " ")
                    End If

                    If Piva <> "" Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Appezza = " & Appezza & " ")
                    End If

                    If Id_Cod <> 0 Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        stb.Append(" AND Budget_Appezzamento_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   Budget_Appezzamento_Codici.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   Budget_Appezzamento_Codici.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class

Public Class Budget_Appezzamento_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal idBudget As Integer,
                                                      ByVal Piva As String,
                                                      ByVal Sa_Cod As Int32,
                                                      ByVal Appezza As Int32,
                                                      ByVal Id_Cod As Int32,
                                                      ByVal Val_Cod As String,
                                                      ByVal Validita_Inizio As Date,
                                                      ByVal Validita_Fine As Date,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                      , Optional ByVal Data_creazione As Date = #2/1/1900# _
                                                      , Optional ByVal Data_modifica As Date = #2/1/1900# _
                                                      , Optional ByVal username_creazione As String = "" _
                                                      , Optional ByVal username_modifica As String = ""
                                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_Codici_W.Scrivi_Budget_Appezzamento_Codici()"

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

            StrSQL.Append("INSERT INTO Budget_Appezzamento_Codici( Id_Budget,")
            StrSQL.Append("                    Piva,        ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Appezza,   ")
            StrSQL.Append("                    Id_Cod,      ")
            StrSQL.Append("                    Val_Cod,     ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(idBudget) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
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

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Appezzamento_Codici_W.EliminaDaBudget()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_Appezzamento_Codici ")
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
