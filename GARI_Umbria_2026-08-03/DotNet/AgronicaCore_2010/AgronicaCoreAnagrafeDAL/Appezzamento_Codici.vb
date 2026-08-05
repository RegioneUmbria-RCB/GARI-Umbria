Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Imports AgronicaCoreDataProvider


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Appezzamento_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################
    Public Function RiferimentoAlfanumerico_from_Appezza(
                                                        ByVal Piva As String,
                                                        ByVal Sa_Cod As Int32,
                                                            ByVal Appezza As Int32,
                                                           ByVal xFiltroAggiuntivo As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R.RiferimentoAlfanumerico_from_Appezza()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim RiferimentoAlfanumerico As String = ""


        Try

            DT = Leggi(Piva,
                        Sa_Cod,
                        Appezza,
                        enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                         "",
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                        xFiltroAggiuntivo,
                        "",
                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                RiferimentoAlfanumerico = DT.Rows(0).Item("Val_Cod")
            End If

            DT = Nothing

        Catch ex As Exception
            RiferimentoAlfanumerico = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return RiferimentoAlfanumerico

    End Function

    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
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

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R.Leggi()"

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
                    stb.Append(" SELECT Distinct *,  Appezzamento_Codici.Validita_Inizio as xValidita_Inizio, Appezzamento_Codici.Validita_Fine as xValidita_Fine ")
                    stb.Append(" FROM  Appezzamento_Codici, Codici_Anagrafe ")
                    stb.Append(" WHERE Appezzamento_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Appezzamento_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Appezzamento_Codici.Id_Cod = Codici_Anagrafe.Codice ")

                    If Piva <> "" Then
                        stb.Append(" AND Appezzamento_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.Appezza = " & Appezza & " ")
                    End If

                    If Id_Cod <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        If leggiValCodPerLike Then
                            stb.Append(" AND Appezzamento_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                        Else
                            stb.Append(" AND Appezzamento_Codici.Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
                        End If
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   Appezzamento_Codici.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   Appezzamento_Codici.Inviato =-1 ")
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
                    stb.Append(" SELECT Distinct *,  Appezzamento_Codici.Validita_Inizio as xValidita_Inizio, Appezzamento_Codici.Validita_Fine as xValidita_Fine ")
                    stb.Append(" ,Appezzamento_Codici.username_creazione as Appezzamento_Codici_username_creazione,Appezzamento_Codici.Username_modifica as Appezzamento_Codici_Username_modifica,Appezzamento_Codici.data_creazione as Appezzamento_Codici_data_creazione,Appezzamento_Codici.data_modifica as Appezzamento_Codici_data_modifica ")
                    stb.Append(" FROM  Appezzamento_Codici, Codici_Anagrafe ")
                    stb.Append(" WHERE Appezzamento_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Appezzamento_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Appezzamento_Codici.Id_Cod = Codici_Anagrafe.Codice ")

                    If Piva <> "" Then
                        stb.Append(" AND Appezzamento_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.Appezza = " & Appezza & " ")
                    End If

                    If Id_Cod <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        stb.Append(" AND Appezzamento_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   Appezzamento_Codici.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   Appezzamento_Codici.Inviato =-1 ")
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
                    stb.AppendLine("       Appezzamento_Codici.id_cod ")
                    stb.AppendLine("  , Appezzamento_Codici.val_cod ")
                    stb.AppendLine("  , Codici_Anagrafe.Descrizione ")
                    stb.AppendLine("  , case when  des1.val_cod_2 Is null then '' else des1.val_cod_2 end as val_cod_2 ")
                    stb.AppendLine("  , Appezzamento_Codici.Validita_Inizio ")
                    stb.AppendLine("  , Appezzamento_Codici.Validita_Fine  ")
                    stb.AppendLine("  , Appezzamento_Codici.username_creazione as Appezzamento_Codici_username_creazione ")
                    stb.AppendLine("  , Appezzamento_Codici.Username_modifica as Appezzamento_Codici_Username_modifica ")
                    stb.AppendLine("  , Appezzamento_Codici.data_creazione as Appezzamento_Codici_data_creazione ")
                    stb.AppendLine("  , Appezzamento_Codici.data_modifica as Appezzamento_Codici_data_modifica   ")
                    stb.AppendLine("  ")
                    stb.AppendLine("  From Appezzamento_Codici ")
                    stb.AppendLine("     inner Join Codici_Anagrafe  ")
                    stb.AppendLine("         On   Appezzamento_Codici.Id_Cod = Codici_Anagrafe.Codice   ")
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
                    stb.AppendLine("  On des1.val_cod = Appezzamento_Codici.val_cod ")
                    stb.AppendLine("  And des1.id_cod = Appezzamento_Codici.id_cod")

                    stb.Append(" WHERE Appezzamento_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Appezzamento_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    stb.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        stb.Append(" AND Appezzamento_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.Appezza = " & Appezza & " ")
                    End If

                    If Id_Cod <> 0 Then
                        stb.Append(" AND Appezzamento_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        stb.Append(" AND Appezzamento_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND   Appezzamento_Codici.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND   Appezzamento_Codici.Inviato =-1 ")
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


    '##############################################################################################
    Public Function Leggi_Utilizzo_Terreno(ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal Id_Cod As Int32, _
                            ByVal Val_Cod As String, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R.Leggi()"

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
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Distinct Appezzamento_Codici.*,  Appezzamento_Codici.Validita_Inizio as xValidita_Inizio, Appezzamento_Codici.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  Appezzamento_Codici, Codici_Anagrafe ")
                    StrSQL.Append(" WHERE Appezzamento_Codici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Appezzamento_Codici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Codici_Anagrafe.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    'StrSQL.Append(" AND   Appezzamento_Codici.Id_Cod = Codici_Anagrafe.Codice ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND Appezzamento_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Appezzamento_Codici.sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Appezzamento_Codici.Appezza = " & Appezza & " ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Appezzamento_Codici.Id_Cod = " & Id_Cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND Appezzamento_Codici.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Appezzamento_Codici.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Appezzamento_Codici.Inviato =-1 ")
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
                    '
                    '
                    '
                    '

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



    Function EsisteCodice_RecuperaDatiAppezzamento(ByVal Piva As String, _
                                                        ByVal Id_Cod As Integer, _
                                                        ByVal Val_Cod As String, _
                                                        ByRef Sa_Cod As Integer, _
                                                        ByRef Appezza As String, _
                                                        ByVal xFiltroAggiuntivo As String, _
                                                        ByVal xOrderBy As String, _
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R.EsisteCodice_RecuperaDatiAppezzamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Flag_Esiste As Boolean = False

        Try

            DT = Leggi(Piva, _
                              0, _
                             0, _
                             Id_Cod, _
                             Val_Cod, _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", _
                              objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Flag_Esiste = True
                Sa_Cod = DT.Rows(0).Item("Sa_Cod")
                Appezza = DT.Rows(0).Item("Appezza")
                'Id_Reg = DT.Rows(0).Item("Id_Reg")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag_Esiste

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Data le chiavi di un appezzamento e una lista di identificativi di codici, restituisce
    ''' i valori dei codici corrispondenti dalla tabella Appezzamento_Codici.
    ''' </summary>
    ''' <param name="Piva">Partita IVA dell'impresa</param>
    ''' <param name="Sa_Cod">Codice del centro aziendale</param>
    ''' <param name="Appezza">Codice dell'appezzamento</param>
    ''' <param name="IdCods">Lista degli identificativi dei codici da leggere</param>
    ''' <param name="objParametri">Parametri di connessione e contesto</param>
    Public Function LeggiPerIdCods(ByVal piva As String,
                                    ByVal saCod As Int32,
                                    ByVal appezza As Int32,
                                    ByVal idCods As List(Of Int32),
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const routine = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R.LeggiPerIdCods()"
        Dim dataTable As DataTable

        Try
            Dim strSql As New System.Text.StringBuilder
            strSql.Length = 0
            
            strSql.Append(" SELECT PIVA, sa_cod, appezza, id_cod, val_cod ")
            strSql.Append(" FROM   Appezzamento_Codici ")
            strSql.Append(" WHERE  PIVA    = '" & Agro_SQL_SaveText(Trim(piva)) & "' ")
            strSql.Append(" AND    sa_cod  = " & Agro_SQL_SaveNum(saCod) & " ")
            strSql.Append(" AND    appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            strSql.Append(" AND    id_cod  IN (" & String.Join(", ", idCods) & ") ")

            dataTable = EseguiQuery_Lettura(objParametri, strSql.ToString, routine)

        Catch ex As Exception
            Dim errorMessage = ex.Message
            Scrivi_LOG(objParametri, routine, errorMessage)
            dataTable = Nothing
            Throw New Exception("[" & routine & "] : " & errorMessage)
        End Try

        Return dataTable

    End Function

End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Appezzamento_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    'Public Function Scrivi( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Appezza As Int32, _
    '                        ByVal Id_Cod As Int32, _
    '                        ByVal Val_Cod As String, _
    '                        ByVal UserName_Creazione As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append("INSERT INTO Appezzamento_Codici( ")
    '        StrSQL.Append("                    Piva,        ")
    '        StrSQL.Append("                    Sa_Cod,      ")
    '        StrSQL.Append("                    Appezza,   ")
    '        StrSQL.Append("                    Id_Cod,      ")
    '        StrSQL.Append("                    Val_Cod,     ")
    '        StrSQL.Append("                    Inviato, DataInvio, ")
    '        StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("                    ) ")
    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")
    '        StrSQL.Append(")")
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Scrivi( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal Id_Cod As Int32, _
                            ByVal Val_Cod As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.Scrivi()"

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

            StrSQL.Append("INSERT INTO Appezzamento_Codici( ")
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
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
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



    '##############################################################################################
    'Public Function Modifica( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Appezza As Int32, _
    '                        ByVal Id_Cod As Int32, _
    '                        ByVal Val_Cod As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.Modifica()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Appezzamento_Codici SET ")
    '        StrSQL.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE Piva = '" & Trim(Piva) & "'")
    '        StrSQL.Append(" AND   Sa_Cod    = " & Sa_Cod & " ")
    '        StrSQL.Append(" AND   Appezza   = " & Appezza & " ")
    '        StrSQL.Append(" AND   Id_Cod    = " & Id_Cod & " ")
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Modifica( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal Id_Cod As Int32, _
                            ByVal Val_Cod As String, _
                            ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.Modifica()"

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
            StrSQL.Append("UPDATE Appezzamento_Codici SET ")
            StrSQL.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod    = " & Sa_Cod & " ")
            StrSQL.Append(" AND   Appezza   = " & Appezza & " ")
            StrSQL.Append(" AND   Id_Cod    = " & Id_Cod & " ")
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

        End Try

        Return xRisp

    End Function


    Public Function Modifica_Parametrizzata(
                                          ByVal Piva As String,
                                          ByVal Sa_Cod As Int32,
                                          ByVal Appezza As Int32,
                                          ByVal Campo As String,
                                          ByVal Valore As Object,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.Modifica_Parametrizzata()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        'Dim Intero32 As Type = GetType(System.Int32)
        'Dim Doubl As Type = GetType(System.Decimal)


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
            StrSQL.Append("UPDATE Appezzamento_Codici SET ")

            StrSQL.Append(strAssegnamento)

            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '---------------------------------------------
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
        End Try

        Return xRisp

    End Function

    Public Function Modifica_Chiave(ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Appezza As Int32,
                                    ByVal Piva_OLD As String,
                                    ByVal Sa_Cod_OLD As Int32,
                                    ByVal Appezza_OLD As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Modifica_Parametrizzata()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        'Dim Intero32 As Type = GetType(System.Int32)
        'Dim Doubl As Type = GetType(System.Decimal)


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

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Appezzamento_Codici SET ")

            StrSQL.Append(" Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" ,Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" ,Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            StrSQL.Append(" ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva_OLD)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_OLD) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza_OLD) & " ")

            '---------------------------------------------
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
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    'Public Function Cancella( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Appezza As Int32, _
    '                        ByVal Id_Cod As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FlagCancellazioneLogica As Int32, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try


    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE Appezzamento_Codici ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" AND Inviato >= 0")

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM     Appezzamento_Codici ")
    '            StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" AND Inviato = 0")

    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Sa_Cod & " ")
    '        End If

    '        If Appezza <> 0 Then
    '            StrSQL.Append(" AND Appezza = " & Appezza & " ")
    '        End If

    '        If Id_Cod <> 0 Then
    '            StrSQL.Append(" AND Id_Cod = " & Id_Cod & " ")
    '        End If

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Cancella( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal Id_Cod As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.Cancella()"

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
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Appezzamento_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Appezzamento_Codici ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato = 0")

            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Sa_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Appezza = " & Appezza & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Id_Cod = " & Id_Cod & " ")
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

        End Try

        Return xRisp

    End Function

    Public Function aggiorna(PIVA As String, sa_cod As Integer, appezza As Integer, id_cod As enum_CodiciAnagrafe, val_cod As String, datainizio As Date, datafine As Date, objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        If PIVA = "" Then
            Throw New Exception("PIVA parametro obbligatorio")
        End If
        If id_cod = 0 Then
            Throw New Exception("id_cod parametro obbligatorio")
        End If
        If sa_cod = 0 Then
            Throw New Exception("sa_cod parametro obbligatorio")
        End If

        Cancella(PIVA, sa_cod, appezza, id_cod, "", objParametriServer)

        Return Scrivi(PIVA, sa_cod, appezza, id_cod, val_cod, datainizio, datafine, objParametriServer)


    End Function


#Region "Entity Framework"
    Public Sub ScriviModificaEliminaxAppezzamento(ByVal piva As String,
                                                    ByVal saCod As Integer,
                                                    ByVal appezza As Integer,
                                                    ByVal idCod As Integer,
                                                    ByVal valCod As String,
                                                    ByVal delete As Boolean,
                                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef GiasContext As Gias_DeveloperServer_Entities)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.ScriviModificaEliminaxAppezzamento()"
        Dim messaggioErrore As String = ""

        Try

            If idCod <> 0 Then

                Dim app_codl = From ic In GiasContext.Appezzamento_Codici
                               Where ic.PIVA = piva AndAlso
                                     ic.sa_cod = saCod AndAlso
                                     ic.appezza = appezza AndAlso
                                     ic.id_cod = idCod
                               Select ic

                Dim operazione As enum_TipoOperazioneDB

                If app_codl.Count > 0 AndAlso (valCod <> "" AndAlso valCod IsNot Nothing) Then
                    operazione = enum_TipoOperazioneDB.Modifica
                ElseIf app_codl.Count > 0 AndAlso (valCod = "" OrElse valCod Is Nothing) Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf app_codl.Count = 0 AndAlso (valCod = "" OrElse valCod Is Nothing) Then
                    operazione = enum_TipoOperazioneDB.Lettura
                ElseIf app_codl.Count = 0 AndAlso (valCod <> "" AndAlso valCod IsNot Nothing) Then
                    operazione = enum_TipoOperazioneDB.Scrittura
                End If

                Select Case operazione
                    Case enum_TipoOperazioneDB.Scrittura

                        Dim app_cod As New AgronicaCoreEntityFramework_POCO.Appezzamento_Codici With {
                            .PIVA = piva,
                            .sa_cod = saCod,
                            .appezza = appezza,
                            .id_cod = idCod,
                            .val_cod = valCod,
                            .inviato = 0,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .Username_Creazione = objParametriServer.UsernameOperazione,
                            .Username_Modifica = objParametriServer.UsernameOperazione
                        }

                        GiasContext.Appezzamento_Codici.Add(app_cod)

                    Case enum_TipoOperazioneDB.Modifica
                        Dim app_cod = app_codl.FirstOrDefault
                        app_cod.val_cod = valCod
                        app_cod.Data_Modifica = DateTime.Now
                        app_cod.Username_Modifica = objParametriServer.UsernameOperazione

                        GiasContext.Appezzamento_Codici.Attach(app_cod)
                        GiasContext.Entry(app_cod).State = EntityState.Modified

                    Case enum_TipoOperazioneDB.Cancellazione

                        Dim app_cod = app_codl.FirstOrDefault
                        GiasContext.Appezzamento_Codici.Attach(app_cod)
                        GiasContext.Appezzamento_Codici.Remove(app_cod)

                End Select

                GiasContext.SaveChanges()

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub
#End Region

    Public Function InsertUpdateDeleteCodici_Massivo(listChiavi As List(Of (String, Integer, Integer)),
                                                     tipoOperazione As enum_TipoOperazioneDB,
                                                     id_cod As Integer,
                                                     val_cod As String,
                                                     timeStamp As Date,
                                                     ByVal objParametri As AgronicaCoreParametri
                                                     ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W.InsertUpdateDeleteCodici_Massivo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim flagConnessione, flagTransazione As Boolean

        Try

            If listChiavi.Count = 0 Then
                Throw New Exception("Parametro non corretto nella query (listChiavi obbligatorio)")
            End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroAppezzamenti(listChiavi, nomeRoutine, objParametri)

            StrSQL.Length = 0
            Select Case tipoOperazione
                Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica
                    StrSQL.AppendLine(" MERGE INTO Appezzamento_Codici AS target ")
                    StrSQL.AppendLine(" USING #TempAppezzamento AS source ON  ")
                    StrSQL.AppendLine("     target.Piva = source.Piva COLLATE DATABASE_DEFAULT")
                    StrSQL.AppendLine(" AND target.Sa_Cod = source.Sa_Cod ")
                    StrSQL.AppendLine(" AND target.Appezza = source.Appezza ")
                    StrSQL.AppendLine($" AND target.Id_Cod = {Agro_SQL_SaveNum(id_cod)}")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" WHEN MATCHED THEN")
                    StrSQL.AppendLine(" UPDATE SET ")
                    StrSQL.AppendLine($"     target.Val_Cod = '{Agro_SQL_SaveText(val_cod)}' ")
                    StrSQL.AppendLine($"   , target.Data_Modifica = {Agro_SQL_SaveDateTime(Date.Now())} ")
                    StrSQL.AppendLine($"   , target.UserName_Modifica = '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" WHEN NOT MATCHED THEN")
                    StrSQL.AppendLine("")
                    StrSQL.AppendLine(" INSERT ( ")
                    StrSQL.AppendLine("           Piva ")
                    StrSQL.AppendLine("         , Sa_Cod ")
                    StrSQL.AppendLine("         , Appezza   ")
                    StrSQL.AppendLine("         , Id_Cod ")
                    StrSQL.AppendLine("         , Val_Cod ")
                    StrSQL.AppendLine("         , Inviato ")
                    StrSQL.AppendLine("         , DataInvio ")
                    StrSQL.AppendLine("         , Data_Creazione ")
                    StrSQL.AppendLine("         , Data_Modifica ")
                    StrSQL.AppendLine("         , UserName_Creazione ")
                    StrSQL.AppendLine("         , UserName_Modifica ")
                    StrSQL.AppendLine("         , Validita_Inizio ")
                    StrSQL.AppendLine("         , Validita_Fine ")
                    StrSQL.AppendLine(" ) ")
                    StrSQL.AppendLine("VALUES (")
                    StrSQL.AppendLine("           source.Piva ")
                    StrSQL.AppendLine("         , source.Sa_Cod ")
                    StrSQL.AppendLine("         , source.Appezza ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveNum(id_cod)} ")
                    StrSQL.AppendLine($"         ,'{Agro_SQL_SaveText(val_cod)}' ")
                    StrSQL.AppendLine("         , 0  ")
                    StrSQL.AppendLine("         , NULL  ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDateTime(Date.Now())}")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDateTime(Date.Now())}")
                    StrSQL.AppendLine($"         , '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine($"         , '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}' ")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDate(AGRODATAINIZIO)}")
                    StrSQL.AppendLine($"         , {Agro_SQL_SaveDate(AGRODATAFINE)}")
                    StrSQL.AppendLine(" );")

                Case enum_TipoOperazioneDB.Cancellazione
                    StrSQL.AppendLine(" DELETE Appezzamento_Codici ")
                    StrSQL.AppendLine(" FROM Appezzamento_Codici ")
                    StrSQL.AppendLine(" JOIN #TempAppezzamento temp ON  ")
                    StrSQL.AppendLine("     Appezzamento_Codici.Piva = temp.Piva COLLATE DATABASE_DEFAULT ")
                    StrSQL.AppendLine(" AND Appezzamento_Codici.Sa_Cod = temp.Sa_Cod ")
                    StrSQL.AppendLine(" AND Appezzamento_Codici.Appezza = temp.Appezza ")
                    StrSQL.AppendLine($" WHERE Id_Cod = {Agro_SQL_SaveNum(id_cod)} ")
            End Select
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(nomeRoutine, objParametri)

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            ' Rollback
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

End Class
