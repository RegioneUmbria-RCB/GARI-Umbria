Imports System.Data.Common
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Operazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Public Function Leggi(
    '                        ByVal Lav_Cod As Int32,
    '                        ByVal FinestraTemp_Inizio As Date,
    '                        ByVal FinestraTemp_Fine As Date,
    '                        ByRef objConnessione As DbConnection,
    '                        ByRef objTransazione As DbTransaction,
    '                        ByVal StringaConnessione As String,
    '                        ByVal FlagVisibilita As Int32,
    '                        ByVal DirectoryLOG As String,
    '                        ByVal FileLOG As String,
    '                        ByVal IdentificatoreUtente As String
    '                        ) As DataTable




    '    Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0           =>  si leggono tutti gli impianti dell'impresa
    '    '   Appezza = 0          =>  si leggono tutti gli impianti del centro aziendale
    '    '   Id_reg = 0           =>  si leggono tutti gli impianti dell'appezzamento
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try


    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM  Operazioni O ")

    '        StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")


    '        If Lav_Cod <> 0 Then
    '            StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
    '        End If

    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND   Inviato >=0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND   Inviato =-1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '---------------------------------------------

    '        'Nota: Questo ordinamento è importante per la gestione del campo.
    '        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
    '        StrSQL.Append(" ORDER BY Lav_Des ASC ")

    '        '------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

    Public Function LeggiQueryParametrica(
                            ByVal Lav_Cod As Integer,
                            ByVal P As Integer,
                            ByVal Gru_Op As Integer,
                            ByVal Tipo As String,
                            ByVal Att_Cod As Integer,
                            ByVal Cerca_LavDes As String,
                            ByVal Cerca_GruDes As String,
                            ByVal Flag_OpColturali As Boolean,
                            ByVal Flag_OpZoo As Boolean,
                            ByVal Flag_OpMacchine As Boolean,
                            ByVal Flag_OpContabili As Boolean,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivoParametri As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoParametri, , objParametri))


            stb.AppendLine(" Declare @Lav_Cod int ")
            stb.AppendLine(" Set @Lav_Cod = " & Lav_Cod)

            stb.AppendLine(" Declare @P  int ")
            stb.AppendLine(" set @P = " & P)

            stb.AppendLine(" Declare @Gru_Op int ")
            stb.AppendLine(" Set @Gru_Op = " & Gru_Op)

            stb.AppendLine(" Declare @Tipo varchar(max) ")
            stb.AppendLine(" Set @Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")

            stb.AppendLine(" Declare @Att_Cod int ")
            stb.AppendLine(" Set @Att_Cod =  " & Att_Cod)

            stb.AppendLine(" Declare @Cerca_LavDes varchar(max) ")
            stb.AppendLine(" set @Cerca_LavDes  =  '" & Agro_SQL_SaveText(Cerca_LavDes) & "'")

            stb.AppendLine(" Declare @Cerca_GruDes varchar(max) ")
            stb.AppendLine(" Set @Cerca_GruDes = '" & Agro_SQL_SaveText(Cerca_GruDes) & "' ")


            stb.AppendLine(" Declare @FinestraTemporaleInizio date ")
            stb.AppendLine(" Declare @FinestraTemporaleFine date ")
            stb.AppendLine(" set @FinestraTemporaleInizio  =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            stb.AppendLine(" set @FinestraTemporaleFine  =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))


            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                    stb.AppendLine(" SELECT  Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.P, Operazioni.GRU_OP ")

                    stb.AppendLine(" FROM    Operazioni  ")
                    stb.AppendLine(" WHERE   Operazioni.Validita_Inizio <= @FinestraTemporaleFine ")
                    stb.AppendLine(" AND     Operazioni.Validita_Fine >=  @FinestraTemporaleInizio ")

                    If Lav_Cod <> 0 Then
                        stb.AppendLine(" AND Operazioni.Lav_Cod = @Lav_Cod")
                    End If

                    If P <> 0 Then
                        stb.AppendLine(" AND Operazioni.P = @P")
                    End If

                    If Gru_Op <> 0 Then
                        stb.AppendLine(" AND Operazioni.Gru_Op = @Gru_Op")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Operazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Operazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY Operazioni.LAV_DES ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.AppendLine(" SELECT O.LAV_COD, coalesce(LL.LAV_DES, O.LAV_DES) as LAV_DES, O.P, O.GRU_OP, O.DATA_AGG, O.inviato, O.datainvio, O.Data_Creazione, O.Data_Modifica, O.Username_Creazione, O.Username_Modifica, O.Validita_Inizio, O.Validita_Fine ")
                    stb.AppendLine(" FROM  Operazioni O ")
                    stb.AppendLine(" LEFT JOIN Operazioni_XLingue LL on O.Lav_Cod = LL.Lav_COD And LL.Lingua_COD = " & objParametri.Lingua_Cod)

                    stb.AppendLine(" WHERE   1 = 1 ")


                    If Lav_Cod <> 0 Then
                        stb.AppendLine(" And O.Lav_Cod = @Lav_Cod")
                    End If

                    If P <> 0 Then
                        stb.AppendLine(" And O.P = @P ")
                    End If

                    If Gru_Op <> 0 Then
                        stb.AppendLine(" And O.Gru_Op = @Gru_Op")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" And   O.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" And   O.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        stb.AppendLine(" ORDER BY coalesce(LL.lav_DES, O.Lav_DES) ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                    stb.AppendLine(" Select  Operazioni.LAV_COD, coalesce ( operazioni_XLingue.lav_des, Operazioni.LAV_DES) As lav_des, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, Operazioni.GRU_OP,GruppoOperazioni.GRU_COD,  ")
                    stb.AppendLine("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")

                    stb.AppendLine(" FROM    Operazioni  ")
                    stb.AppendLine(" INNER JOIN GruppoOperazioni On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
                    stb.AppendLine(" LEFT JOIN operazioni_XLingue On Operazioni.LAV_COD = operazioni_XLingue.LAV_COD And operazioni_XLingue.Lingua_COD = " & objParametri.Lingua_Cod & " ")


                    stb.AppendLine(" WHERE   Operazioni.Validita_Inizio <= @FinestraTemporaleFine ")
                    stb.AppendLine(" And     Operazioni.Validita_Fine >= @FinestraTemporaleInizio ")

                    If Lav_Cod <> 0 Then
                        stb.AppendLine(" And Operazioni.Lav_Cod = @Lav_Cod ")
                    End If

                    If P <> 0 Then
                        stb.AppendLine(" And Operazioni.P = @P ")
                    End If

                    If Gru_Op <> 0 Then
                        stb.AppendLine(" And Operazioni.Gru_Op = @Gru_Op ")
                    End If


                    If Tipo <> "" Then
                        stb.AppendLine(" And GruppoOperazioni.Tipo = @Tipo")
                    End If

                    If Att_Cod <> 0 Then
                        stb.AppendLine(" AND GruppoOperazioni.Att_Cod = @Att_Cod")
                    End If

                    If Cerca_LavDes <> "" Then
                        stb.AppendLine(" AND Operazioni.Lav_Des LIKE '%' + @Cerca_lavDes + '%' ")
                    End If

                    If Cerca_GruDes <> "" Then
                        stb.AppendLine(" AND GruppoOperazioni.Gru_Des LIKE '%' + @Cerca_GruDes + '%' ")
                    End If

                    If Flag_OpColturali = True Then
                        stb.AppendLine(" AND GruppoOperazioni.Tipo = 'C' ")
                    End If

                    If Flag_OpZoo = True Then
                        stb.AppendLine(" AND GruppoOperazioni.Tipo = 'Z' ")
                    End If

                    If Flag_OpMacchine = True Then
                        stb.AppendLine(" AND GruppoOperazioni.Tipo = 'P' ")
                    End If

                    If Flag_OpContabili = True Then
                        stb.AppendLine(" AND GruppoOperazioni.Tipo = 'E' ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.AppendLine(" AND   Operazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.AppendLine(" AND   Operazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        stb.AppendLine(" ORDER BY GruppoOperazioni.GRU_DES, coalesce(operazioni_XLingue.lav_des, Operazioni.LAV_DES) ")
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

    Public Function Leggi(
                            ByVal Lav_Cod As Integer,
                            ByVal P As Integer,
                            ByVal Gru_Op As Integer,
                            ByVal Tipo As String,
                            ByVal Att_Cod As Integer,
                            ByVal Cerca_LavDes As String,
                            ByVal Cerca_GruDes As String,
                            ByVal Flag_OpColturali As Boolean,
                            ByVal Flag_OpZoo As Boolean,
                            ByVal Flag_OpMacchine As Boolean,
                            ByVal Flag_OpContabili As Boolean,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.P, Operazioni.GRU_OP ")

                    StrSQL.Append(" FROM    Operazioni  ")
                    StrSQL.Append(" WHERE   Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.Append(" AND     Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If

                    If P <> 0 Then
                        StrSQL.Append(" AND Operazioni.P = " & Agro_SQL_SaveNum(P))
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.Append(" AND Operazioni.Gru_Op = " & Agro_SQL_SaveNum(Gru_Op))
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Operazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Operazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Operazioni.LAV_DES ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT O.LAV_COD, coalesce(LL.LAV_DES, O.LAV_DES) as LAV_DES, O.P, O.GRU_OP, O.DATA_AGG, O.inviato, O.datainvio, O.Data_Creazione, O.Data_Modifica, O.Username_Creazione, O.Username_Modifica, O.Validita_Inizio, O.Validita_Fine ")
                    StrSQL.Append(" FROM  Operazioni O ")
                    StrSQL.Append(" LEFT JOIN Operazioni_XLingue LL on O.Lav_Cod = LL.Lav_COD AND LL.Lingua_COD = " & objParametri.Lingua_Cod)

                    StrSQL.Append(" WHERE   1 = 1 ")


                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND O.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If

                    If P <> 0 Then
                        StrSQL.Append(" AND O.P = " & Agro_SQL_SaveNum(P))
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.Append(" AND O.Gru_Op = " & Agro_SQL_SaveNum(Gru_Op))
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   O.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   O.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        StrSQL.Append(" ORDER BY coalesce(LL.lav_DES, O.Lav_DES) ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Operazioni.LAV_COD, coalesce ( operazioni_XLingue.lav_des, Operazioni.LAV_DES) as lav_des, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, Operazioni.GRU_OP,GruppoOperazioni.GRU_COD,  ")
                    StrSQL.Append("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")

                    StrSQL.Append(" FROM    Operazioni  ")
                    StrSQL.Append(" INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
                    StrSQL.Append(" LEFT JOIN operazioni_XLingue ON Operazioni.LAV_COD = operazioni_XLingue.LAV_COD AND operazioni_XLingue.Lingua_COD = " & objParametri.Lingua_Cod & " ")


                    StrSQL.Append(" WHERE   Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.Append(" AND     Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If

                    If P <> 0 Then
                        StrSQL.Append(" AND Operazioni.P = " & Agro_SQL_SaveNum(P))
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.Append(" AND Operazioni.Gru_Op = " & Agro_SQL_SaveNum(Gru_Op))
                    End If


                    If Tipo <> "" Then
                        StrSQL.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
                    End If

                    If Att_Cod <> 0 Then
                        StrSQL.Append(" AND GruppoOperazioni.Att_Cod = " & Agro_SQL_SaveNum(Att_Cod))
                    End If

                    If Cerca_LavDes <> "" Then
                        StrSQL.Append(" AND Operazioni.Lav_Des LIKE '%" & Agro_SQL_SaveText(Cerca_LavDes) & "%' ")
                    End If

                    If Cerca_GruDes <> "" Then
                        StrSQL.Append(" AND GruppoOperazioni.Gru_Des LIKE '%" & Agro_SQL_SaveText(Cerca_GruDes) & "%' ")
                    End If

                    If Flag_OpColturali = True Then
                        StrSQL.Append(" AND GruppoOperazioni.Tipo = 'C' ")
                    End If

                    If Flag_OpZoo = True Then
                        StrSQL.Append(" AND GruppoOperazioni.Tipo = 'Z' ")
                    End If

                    If Flag_OpMacchine = True Then
                        StrSQL.Append(" AND GruppoOperazioni.Tipo = 'P' ")
                    End If

                    If Flag_OpContabili = True Then
                        StrSQL.Append(" AND GruppoOperazioni.Tipo = 'E' ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Operazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Operazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        StrSQL.Append(" ORDER BY GruppoOperazioni.GRU_DES, coalesce(operazioni_XLingue.lav_des, Operazioni.LAV_DES) ")
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




    Public Function LeggixCaricaAlbero( _
                               ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                               ByVal xFiltroAggiuntivo As String, _
                               ByVal xOrderBy As String, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.LeggixCaricaAlbero()"

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

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  distinct  GruppoOperazioni.GRU_COD,  ")
                    StrSQL.Append("         Operazioni.GRU_OP, coalesce(GOperXL.GRU_DES, GruppoOperazioni.GRU_DES) AS GRU_DES, GruppoOperazioni.Tipo  ")

                    StrSQL.Append(" FROM    Operazioni  ")
                    StrSQL.Append(" INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
                    StrSQL.Append(" LEFT JOIN GruppoOperazioni_XLingua GOperXL on GruppoOperazioni.Gru_COD = GOperXL.Gru_COD AND GOperXL.Lingua_Cod = " & objParametri.Lingua_Cod)

                    StrSQL.Append(" WHERE   Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.Append(" AND     Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Operazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Operazioni.Inviato =-1 ")
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

    Public Function Gru_Op_from_LavorazioneCod(ByVal Lav_Cod As Integer, _
                          ByVal agroParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.LavorazioneDes_from_LavorazioneCod"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder


        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT Gru_Op ")
            StrSQL.Append(" FROM  Operazioni ")
            StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(agroParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(agroParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod.ToString) & "")
            '---------------------------------------------

            Dim DT As DataTable = MyBase.EseguiQuery_Lettura(agroParametri, StrSQL.ToString(), NomeRoutine)
            If (DT.Rows.Count > 0) Then
                'ritorno la descrizione...
                Return DT.Rows(0)(0).ToString()
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            MyBase.Scrivi_LOG(agroParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return ""

        End Try


    End Function


    'FILIPPO: OTTENGO LA DESCRIZIONE dal codice ###########################################################################################
    Public Function LavorazioneDes_from_LavorazioneCod(ByVal Lav_Cod As Integer, _
                            ByVal agroParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.LavorazioneDes_from_LavorazioneCod"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder


        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT coalesce(LL.lav_des, O.lav_des) as lav_des ")
            StrSQL.Append(" FROM  Operazioni O")

            StrSQL.Append(" LEFT JOIN Operazioni_XLingue LL ")
            StrSQL.Append(" on O.Lav_Cod = LL.Lav_COD ")
            StrSQL.Append(" AND LL.Lingua_cod= " & agroParametri.Lingua_Cod)

            StrSQL.Append(" WHERE   O.Validita_inizio <= " & Agro_SQL_SaveDate(agroParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     O.Validita_Fine >= " & Agro_SQL_SaveDate(agroParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND O.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod.ToString) & "")
            '---------------------------------------------

            Dim DT As DataTable = MyBase.EseguiQuery_Lettura(agroParametri, StrSQL.ToString(), NomeRoutine)
            If (DT.Rows.Count > 0) Then
                'ritorno la descrizione...
                Return DT.Rows(0)(0).ToString()
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            MyBase.Scrivi_LOG(agroParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return ""

        End Try


    End Function


    '##########################################################################################################################################
    Public Function LavDes_from_LavCod(ByVal Lav_Cod As Integer, _
                                    ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.LavDes_from_LavCod"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Lav_Des As String = ""

        Try

            Dim Dt As DataTable

            Dt = Leggi(Lav_Cod, _
                        0, 0, "", 0, "", "", _
                        True, True, True, True, _
                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                         "", "", objParametri)

            'Se il datatable non è chiuso allora ...	
            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
                Lav_Des = Dt.Rows(0).Item("Lav_Des")
            End If

            Dt = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            MyBase.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return ""
        End Try

        Return Lav_Des

    End Function

    '##############################################################################################
    'Public Function Leggi_Intervallo(
    '                        ByVal sLav_Cod As String,
    '                        ByVal FinestraTemp_Inizio As Date,
    '                        ByVal FinestraTemp_Fine As Date,
    '                        ByRef objConnessione As DbConnection,
    '                        ByRef objTransazione As DbTransaction,
    '                        ByVal StringaConnessione As String,
    '                        ByVal FlagVisibilita As Int32,
    '                        ByVal DirectoryLOG As String,
    '                        ByVal FileLOG As String,
    '                        ByVal IdentificatoreUtente As String
    '                        ) As DataTable




    '    Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0           =>  si leggono tutti gli impianti dell'impresa
    '    '   Appezza = 0          =>  si leggono tutti gli impianti del centro aziendale
    '    '   Id_reg = 0           =>  si leggono tutti gli impianti dell'appezzamento
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try


    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM  Operazioni ")

    '        StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")


    '        If sLav_Cod <> "" Then
    '            StrSQL.Append(" AND Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(sLav_Cod.ToString) & ") ")
    '        End If

    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND   Inviato >=0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND   Inviato =-1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '---------------------------------------------

    '        'Nota: Questo ordinamento è importante per la gestione del campo.
    '        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
    '        StrSQL.Append(" ORDER BY Lav_Des ASC ")

    '        '------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

End Class
