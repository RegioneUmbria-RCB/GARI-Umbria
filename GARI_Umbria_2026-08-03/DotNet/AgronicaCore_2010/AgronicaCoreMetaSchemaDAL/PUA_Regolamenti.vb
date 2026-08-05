Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PUA_Regolamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Long,
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            'If Cod_Indirizzo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            'End If
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Regolamento_Cod, Regolamento_DES, Descrizione, Tipo FROM PUA_Regolamenti ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Regolamento_Cod <> 0 Then
                        StrSQL.Append(" AND Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Regolamento_DES ASC ")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    PUA_Regolamenti ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Regolamento_Cod <> 0 Then
                        StrSQL.Append(" AND Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Regolamento_DES ASC ")
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

    'a differenza della precedente viene passato il tipo
    Public Function Leggi(ByVal Regolamento_Cod As Long,
                            ByVal Tipo As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            'If Cod_Indirizzo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            'End If


            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    PUA_Regolamenti ")
            StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Tipo <> 0 Then
                StrSQL.Append(" AND tipo =  " & Agro_SQL_SaveNum(Tipo) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Regolamento_cod desc ")
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

    'a differenza della precedente vengono passate le date
    Public Function Leggi(ByVal Regolamento_Cod As Long,
                            ByVal Tipo As Int32,
                            ByVal DataInizio As Date,
                            ByVal DataFine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    PUA_Regolamenti ")
            StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(DataInizio) & " ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Tipo <> 0 Then
                StrSQL.Append(" AND tipo =  " & Agro_SQL_SaveNum(Tipo) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Regolamento_cod desc ")
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

    'a differenza della precedente viene passato il tipo metodo
    Public Function Leggi(ByVal Regolamento_Cod As Long,
                            ByVal Tipo As Int32,
                            ByVal TipoMetodo As Int32,
                            ByVal DataInizio As Date,
                            ByVal DataFine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    PUA_Regolamenti ")

            Select Case TipoMetodo
                Case enum_PUA_Tipo.Completo, enum_PUA_Tipo.Semplificato
                    StrSQL.Append(" INNER JOIN PUA_ParametrixRegolamenti ON PUA_Regolamenti.regolamento_cod=PUA_ParametrixRegolamenti.regolamento_cod")
            End Select

            StrSQL.Append(" WHERE   PUA_Regolamenti.Validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & " ")
            StrSQL.Append(" AND     PUA_Regolamenti.Validita_Fine >= " & Agro_SQL_SaveDate(DataInizio) & " ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND PUA_Regolamenti.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Tipo <> 0 Then
                StrSQL.Append(" AND PUA_Regolamenti.tipo =  " & Agro_SQL_SaveNum(Tipo) & "  ")
            End If

            Select Case TipoMetodo
                Case enum_PUA_Tipo.Completo
                    StrSQL.Append(" AND parametro_cod =  " & Agro_SQL_SaveNum(enum_PUAParametri.Metodo_Completo) & "  ")
                    StrSQL.Append(" AND parametro_valore =  '1'  ")
                Case enum_PUA_Tipo.Semplificato
                    StrSQL.Append(" AND parametro_cod =  " & Agro_SQL_SaveNum(enum_PUAParametri.Metodo_Semplificato) & "  ")
                    StrSQL.Append(" AND parametro_valore =  '1'  ")
            End Select

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PUA_Regolamenti.Regolamento_cod desc ")
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

    'a differenza della precedente viene passato il parametro VisualizzaPrivati e Piva_Superuser
    Public Function Leggi(ByVal Regolamento_Cod As Long,
                            ByVal Tipo As Int32,
                            ByVal TipoMetodo As Int32,
                            ByVal VisualizzaPrivati As Boolean, ByVal Piva_Superuser As String,
                            ByVal DataInizio As Date, ByVal DataFine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    PUA_Regolamenti ")

            Select Case TipoMetodo
                Case enum_PUA_Tipo.Completo, enum_PUA_Tipo.Semplificato
                    StrSQL.Append(" INNER JOIN PUA_ParametrixRegolamenti ON PUA_Regolamenti.regolamento_cod=PUA_ParametrixRegolamenti.regolamento_cod")
            End Select

            StrSQL.Append(" WHERE   PUA_Regolamenti.Validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & " ")
            StrSQL.Append(" AND     PUA_Regolamenti.Validita_Fine >= " & Agro_SQL_SaveDate(DataInizio) & " ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND PUA_Regolamenti.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Tipo <> 0 Then
                StrSQL.Append(" AND PUA_Regolamenti.tipo =  " & Agro_SQL_SaveNum(Tipo) & "  ")
            End If

            If VisualizzaPrivati = False Then
                StrSQL.Append(" AND (flag_privato_pubblico is null or Flag_Privato_Pubblico = 0) ")
            Else
                StrSQL.Append(" AND ((flag_privato_pubblico is null or Flag_Privato_Pubblico = 0) ")
                StrSQL.Append(" OR (Flag_Privato_Pubblico = 1 and PUA_Regolamenti.Regolamento_Cod in ( select regolamento_cod from PUA_RegolamentiXpiva_superUser_OperazioniAutorizzate where Piva_superUser = '" & Agro_SQL_SaveText(Piva_Superuser) & "'))) ")
            End If

            Select Case TipoMetodo
                Case enum_PUA_Tipo.Completo
                    StrSQL.Append(" AND parametro_cod =  " & Agro_SQL_SaveNum(enum_PUAParametri.Metodo_Completo) & "  ")
                    StrSQL.Append(" AND parametro_valore =  '1'  ")
                Case enum_PUA_Tipo.Semplificato
                    StrSQL.Append(" AND parametro_cod =  " & Agro_SQL_SaveNum(enum_PUAParametri.Metodo_Semplificato) & "  ")
                    StrSQL.Append(" AND parametro_valore =  '1'  ")
            End Select

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PUA_Regolamenti.Regolamento_cod desc ")
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

    Public Function Leggi_DaRegione(ByVal Regione_Cod As String,
                            ByVal Tipo As Int32,
                            ByVal DataInizio As Date,
                            ByVal DataFine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R.Leggi_DaRegione()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM   PUA_Regolamenti ")
            StrSQL.Append(" inner join    Enti on PUA_Regolamenti.IDEnte = Enti.IDEnte ")
            StrSQL.Append(" WHERE   PUA_Regolamenti.Validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & " ")
            StrSQL.Append(" AND     PUA_Regolamenti.Validita_Fine >= " & Agro_SQL_SaveDate(DataInizio) & " ")

            If Regione_Cod <> "" AndAlso IsNumeric(Regione_Cod) Then
                StrSQL.Append(" AND Enti.reg_istat =  '" & Agro_SQL_SaveText(Right("000" & Regione_Cod, 3)) & "' ")
            End If

            If Tipo <> 0 Then
                StrSQL.Append(" AND PUA_Regolamenti.tipo =  " & Agro_SQL_SaveNum(Tipo) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PUA_Regolamenti.validita_inizio DESC, PUA_Regolamenti.Regolamento_Cod DESC ")
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

    Public Function Leggi_Dir_Nitrati_DaRegolamentoCod(ByVal Regolamento_Cod As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R.Leggi_DaRegione()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM   PUA_Regolamenti ")
            StrSQL.Append(" WHERE Tipo = " & enum_PUARegolamenti_Tipo.PUA)
            StrSQL.Append(" AND   idente = (select idente from PUA_Regolamenti where regolamento_cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")
            StrSQL.Append(" AND   validita_inizio<=(select validita_fine from PUA_Regolamenti where regolamento_cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")
            StrSQL.Append(" AND   validita_fine>=(select validita_inizio from PUA_Regolamenti where regolamento_cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PUA_Regolamenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PUA_Regolamenti.validita_inizio DESC, PUA_Regolamenti.Regolamento_Cod DESC ")
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
