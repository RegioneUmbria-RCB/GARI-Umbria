Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class Appezzamento_Storico_R
    Inherits AgronicaCoreDataProvider.DataProvider





    '============================================================================
    'Public Function Leggi_Da_Padre(ByVal Padre_Piva As String, _
    '                               ByVal Padre_Sa_Cod As Int32, _
    '                               ByVal Padre_Appezza As Int32, _
    '                               ByVal Figlio_Piva As String, _
    '                               ByVal Figlio_Sa_Cod As Int32, _
    '                               ByVal Figlio_Appezza As Int32, _
    '                               ByVal FinestraTemp_Inizio As Date, _
    '                               ByVal FinestraTemp_Fine As Date, _
    '                               ByRef objConnessione As DbConnection, _
    '                               ByVal StringaConnessione As String, _
    '                               ByVal FlagVisibilita As Int32, _
    '                               ByVal DirectoryLOG As String, _
    '                               ByVal FileLOG As String, _
    '                               ByVal IdentificatoreUtente As String _
    '                               ) As DataTable
    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_R.Leggi_Da_Padre()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Padre_Piva 
    '    '   Padre_Sa_Cod 
    '    '   Padre_Appezza
    '    '   Figlio_Piva 
    '    '   Figlio_Sa_Cod
    '    '   Figlio_Appezza 
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
    '        StrSQL.Append(" FROM  Appezzamento_Storico")
    '        StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   Inviato >= 0 ")

    '        If Padre_Piva <> "" Then
    '            StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
    '        End If

    '        If Padre_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
    '        End If

    '        If Padre_Appezza <> 0 Then
    '            StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
    '        End If

    '        If Figlio_Piva <> "" Then
    '            StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
    '        End If

    '        If Figlio_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
    '        End If

    '        If Figlio_Appezza <> 0 Then
    '            StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
    '        End If


    '        StrSQL.Append(" ORDER BY Padre_Piva, Padre_Sa_Cod, Padre_Appezza ASC")

    '        '---------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function


    'Versione che utilizza AgronicaCoreParametri
    '============================================================================
    Public Function Leggi_Da_Padre(ByVal Padre_Piva As String, _
                                    ByVal Padre_Sa_Cod As Int32, _
                                    ByVal Padre_Appezza As Int32, _
                                    ByVal Figlio_Piva As String, _
                                    ByVal Figlio_Sa_Cod As Int32, _
                                    ByVal Figlio_Appezza As Int32, _
                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_R.Leggi_Da_Padre()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    ''''''''''''''''''''DA MODIFICARE QUERY MINI
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Appezzamento_Storico")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Inviato >= 0 ")

                    If Padre_Piva <> "" Then
                        StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
                    End If

                    If Padre_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
                    End If

                    If Padre_Appezza <> 0 Then
                        StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
                    End If

                    If Figlio_Piva <> "" Then
                        StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
                    End If

                    If Figlio_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
                    End If

                    If Figlio_Appezza <> 0 Then
                        StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
                    End If

                    '--------------------------------------------------------------------------
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
                        StrSQL.Append(" ORDER BY Padre_Piva, Padre_Sa_Cod, Padre_Appezza ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Appezzamento_Storico")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Inviato >= 0 ")

                    If Padre_Piva <> "" Then
                        StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
                    End If

                    If Padre_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
                    End If

                    If Padre_Appezza <> 0 Then
                        StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
                    End If

                    If Figlio_Piva <> "" Then
                        StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
                    End If

                    If Figlio_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
                    End If

                    If Figlio_Appezza <> 0 Then
                        StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
                    End If



                    '--------------------------------------------------------------------------
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
                        StrSQL.Append(" ORDER BY Padre_Piva, Padre_Sa_Cod, Padre_Appezza ASC")
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



    ''============================================================================
    'Public Function Leggi_Da_Figlio(ByVal Figlio_Piva As String, _
    '                                ByVal Figlio_Sa_Cod As Int32, _
    '                                ByVal Figlio_Appezza As Int32, _
    '                                ByVal Padre_Piva As String, _
    '                                ByVal Padre_Sa_Cod As Int32, _
    '                                ByVal Padre_Appezza As Int32, _
    '                                ByVal FinestraTemp_Inizio As Date, _
    '                                ByVal FinestraTemp_Fine As Date, _
    '                                ByRef objConnessione As DbConnection, _
    '                                ByVal StringaConnessione As String, _
    '                                ByVal FlagVisibilita As Int32, _
    '                                ByVal DirectoryLOG As String, _
    '                                ByVal FileLOG As String, _
    '                                ByVal IdentificatoreUtente As String _
    '                                ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_R.Leggi_Da_Figlio()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Padre_Piva 
    '    '   Padre_Sa_Cod 
    '    '   Padre_Appezza
    '    '   Figlio_Piva 
    '    '   Figlio_Sa_Cod
    '    '   Figlio_Appezza 
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
    '        StrSQL.Append(" FROM  Appezzamento_Storico")
    '        StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   Inviato >= 0 ")

    '        If Padre_Piva <> "" Then
    '            StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
    '        End If

    '        If Padre_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
    '        End If

    '        If Padre_Appezza <> 0 Then
    '            StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
    '        End If

    '        If Figlio_Piva <> "" Then
    '            StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
    '        End If

    '        If Figlio_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
    '        End If

    '        If Figlio_Appezza <> 0 Then
    '            StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
    '        End If


    '        StrSQL.Append(" ORDER BY Padre_Piva, Padre_Sa_Cod, Padre_Appezza ASC")

    '        '---------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

    'Versione che utilizza AgronicaCoreParametri
    '============================================================================
    Public Function Leggi_Da_Figlio(ByVal Figlio_Piva As String, _
                                    ByVal Figlio_Sa_Cod As Int32, _
                                    ByVal Figlio_Appezza As Int32, _
                                    ByVal Padre_Piva As String, _
                                    ByVal Padre_Sa_Cod As Int32, _
                                    ByVal Padre_Appezza As Int32, _
                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_R.Leggi_Da_Figlio()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    ''''''''''''''''''''DA MODIFICARE QUERY MINI
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Appezzamento_Storico")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Inviato >= 0 ")

                    If Padre_Piva <> "" Then
                        StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
                    End If

                    If Padre_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
                    End If

                    If Padre_Appezza <> 0 Then
                        StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
                    End If

                    If Figlio_Piva <> "" Then
                        StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
                    End If

                    If Figlio_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
                    End If

                    If Figlio_Appezza <> 0 Then
                        StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
                    End If


                    '--------------------------------------------------------------------------
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
                        StrSQL.Append(" ORDER BY Padre_Piva, Padre_Sa_Cod, Padre_Appezza ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    ''''''''''''''''''''DA MODIFICARE QUERY MINI
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Appezzamento_Storico")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Inviato >= 0 ")

                    If Padre_Piva <> "" Then
                        StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
                    End If

                    If Padre_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
                    End If

                    If Padre_Appezza <> 0 Then
                        StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
                    End If

                    If Figlio_Piva <> "" Then
                        StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
                    End If

                    If Figlio_Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
                    End If

                    If Figlio_Appezza <> 0 Then
                        StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
                    End If


                    '--------------------------------------------------------------------------
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
                        StrSQL.Append(" ORDER BY Padre_Piva, Padre_Sa_Cod, Padre_Appezza ASC")
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


End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Appezzamento_Storico_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'Public Function Scrivi( _
    '                        ByVal Padre_Piva As String, _
    '                        ByVal Padre_Sa_Cod As Int32, _
    '                        ByVal Padre_Appezza As Int32, _
    '                        ByVal Figlio_Piva As String, _
    '                        ByVal Figlio_Sa_Cod As Int32, _
    '                        ByVal Figlio_Appezza As Int32, _
    '                        ByVal Trasferimento_Superficie As Decimal, _
    '                        ByVal Trasferimento_Data As Date, _
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

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W.Scrivi()"

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
    '        StrSQL.Append("INSERT INTO Appezzamento_Storico(Padre_Piva , Padre_Sa_Cod, Padre_Appezza, ")
    '        StrSQL.Append("                                 Figlio_Piva, Figlio_Sa_Cod, Figlio_Appezza, ")
    '        StrSQL.Append("                                 Trasferimento_Superficie, Trasferimento_Data, ")
    '        StrSQL.Append("                                 Inviato, DataInvio, ")
    '        StrSQL.Append("                                 Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("                                 UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("                                 Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("                                 ) ")

    '        StrSQL.Append("   VALUES (")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Padre_Piva) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Padre_Sa_Cod))
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Padre_Appezza))
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Figlio_Piva) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Figlio_Sa_Cod))
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Figlio_Appezza))
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Trasferimento_Superficie))
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Trasferimento_Data) & "  ")

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
                            ByVal Padre_Piva As String, _
                            ByVal Padre_Sa_Cod As Int32, _
                            ByVal Padre_Appezza As Int32, _
                            ByVal Figlio_Piva As String, _
                            ByVal Figlio_Sa_Cod As Int32, _
                            ByVal Figlio_Appezza As Int32, _
                            ByVal Trasferimento_Superficie As Decimal, _
                            ByVal Trasferimento_Data As Date, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

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


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Appezzamento_Storico(Padre_Piva , Padre_Sa_Cod, Padre_Appezza, ")
            StrSQL.Append("                                 Figlio_Piva, Figlio_Sa_Cod, Figlio_Appezza, ")
            StrSQL.Append("                                 Trasferimento_Superficie, Trasferimento_Data, ")
            StrSQL.Append("                                 Inviato, DataInvio, ")
            StrSQL.Append("                                 Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                                 UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                                 Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                                 ) ")

            StrSQL.Append("   VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Padre_Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Padre_Sa_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Padre_Appezza))
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Figlio_Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Figlio_Sa_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Figlio_Appezza))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Trasferimento_Superficie))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Trasferimento_Data) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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


    ''##############################################################################################
    'Public Function Modifica( _
    '                        ByVal Padre_Piva As String, _
    '                        ByVal Padre_Sa_Cod As Long, _
    '                        ByVal Padre_Appezza As Long, _
    '                        ByVal Figlio_Piva As String, _
    '                        ByVal Figlio_Sa_Cod As Long, _
    '                        ByVal Figlio_Appezza As Long, _
    '                        ByVal Trasferimento_Superficie As Decimal, _
    '                        ByVal Trasferimento_Data As Date, _
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

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W.Modifica()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Padre_Piva = ""         
    '    '   Padre_Sa_Cod = 0        
    '    '   Padre_Appezza = 0       
    '    '   Figlio_Piva = ""         
    '    '   Figlio_Sa_Cod = 0        
    '    '   Figlio_Appezza = 0       
    '    '
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
    '        StrSQL.Append("UPDATE Appezzamento_Storico SET ")
    '        StrSQL.Append("    Trasferimento_Superficie  =  " & Agro_SQL_SaveNum(Trasferimento_Superficie))
    '        StrSQL.Append("   ,Trasferimento_Data        =  " & Agro_SQL_SaveDate(Trasferimento_Data))
    '        StrSQL.Append("   ,Inviato                   =  0 ")
    '        StrSQL.Append("   ,DataInvio                 =  Null ")
    '        StrSQL.Append("   ,Data_Modifica             =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica         = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio           =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("   ,Validita_Fine             =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE Inviato >=0 ")

    '        If Padre_Piva <> "" Then
    '            StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
    '        End If

    '        If Padre_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
    '        End If

    '        If Padre_Appezza <> 0 Then
    '            StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
    '        End If

    '        If Figlio_Piva <> "" Then
    '            StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
    '        End If

    '        If Figlio_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
    '        End If

    '        If Figlio_Appezza <> 0 Then
    '            StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
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
    Public Function Modifica( _
                            ByVal Padre_Piva As String, _
                            ByVal Padre_Sa_Cod As Long, _
                            ByVal Padre_Appezza As Long, _
                            ByVal Figlio_Piva As String, _
                            ByVal Figlio_Sa_Cod As Long, _
                            ByVal Figlio_Appezza As Long, _
                            ByVal Trasferimento_Superficie As Decimal, _
                            ByVal Trasferimento_Data As Date, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva = ""         
        '   Padre_Sa_Cod = 0        
        '   Padre_Appezza = 0       
        '   Figlio_Piva = ""         
        '   Figlio_Sa_Cod = 0        
        '   Figlio_Appezza = 0       
        '
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
            StrSQL.Append("UPDATE Appezzamento_Storico SET ")
            StrSQL.Append("    Trasferimento_Superficie  =  " & Agro_SQL_SaveNum(Trasferimento_Superficie))
            StrSQL.Append("   ,Trasferimento_Data        =  " & Agro_SQL_SaveDate(Trasferimento_Data))
            StrSQL.Append("   ,Inviato                   =  0 ")
            StrSQL.Append("   ,DataInvio                 =  Null ")
            StrSQL.Append("   ,Data_Modifica             =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica         = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio           =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine             =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Inviato >=0 ")

            If Padre_Piva <> "" Then
                StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
            End If

            If Padre_Sa_Cod <> 0 Then
                StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
            End If

            If Padre_Appezza <> 0 Then
                StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
            End If

            If Figlio_Piva <> "" Then
                StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
            End If

            If Figlio_Sa_Cod <> 0 Then
                StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
            End If

            If Figlio_Appezza <> 0 Then
                StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
            End If
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
    '                        ByVal Padre_Piva As String, _
    '                        ByVal Padre_Sa_Cod As Long, _
    '                        ByVal Padre_Appezza As Long, _
    '                        ByVal Figlio_Piva As String, _
    '                        ByVal Figlio_Sa_Cod As Long, _
    '                        ByVal Figlio_Appezza As Long, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FlagCancellazioneLogica As Int32, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Padre_Piva = ""         
    '    '   Padre_Sa_Cod = 0        
    '    '   Padre_Appezza = 0       
    '    '   Figlio_Piva = ""         
    '    '   Figlio_Sa_Cod = 0        
    '    '   Figlio_Appezza = 0       
    '    '
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
    '            StrSQL.Append(" UPDATE Appezzamento_Storico ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE Inviato >= 0")

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM     Appezzamento_Storico ")
    '            StrSQL.Append(" WHERE    Inviato = 0 ")

    '        End If

    '        If Padre_Piva <> "" Then
    '            StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
    '        End If

    '        If Padre_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
    '        End If

    '        If Padre_Appezza <> 0 Then
    '            StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
    '        End If

    '        If Figlio_Piva <> "" Then
    '            StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
    '        End If

    '        If Figlio_Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
    '        End If

    '        If Figlio_Appezza <> 0 Then
    '            StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
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



    '##############################################################################################
    Public Function Cancella( _
                            ByVal Padre_Piva As String, _
                            ByVal Padre_Sa_Cod As Long, _
                            ByVal Padre_Appezza As Long, _
                            ByVal Figlio_Piva As String, _
                            ByVal Figlio_Sa_Cod As Long, _
                            ByVal Figlio_Appezza As Long, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva = ""         
        '   Padre_Sa_Cod = 0        
        '   Padre_Appezza = 0       
        '   Figlio_Piva = ""         
        '   Figlio_Sa_Cod = 0        
        '   Figlio_Appezza = 0       
        '
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
                StrSQL.Append(" UPDATE Appezzamento_Storico ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Appezzamento_Storico ")
                StrSQL.Append(" WHERE    Inviato = 0 ")

            End If

            If Padre_Piva <> "" Then
                StrSQL.Append(" AND Padre_Piva = '" & Agro_SQL_SaveText(Padre_Piva) & "'")
            End If

            If Padre_Sa_Cod <> 0 Then
                StrSQL.Append(" AND Padre_Sa_Cod = " & Agro_SQL_SaveNum(Padre_Sa_Cod) & " ")
            End If

            If Padre_Appezza <> 0 Then
                StrSQL.Append(" AND Padre_Appezza = " & Agro_SQL_SaveNum(Padre_Appezza) & " ")
            End If

            If Figlio_Piva <> "" Then
                StrSQL.Append(" AND Figlio_Piva = '" & Agro_SQL_SaveText(Figlio_Piva) & "'")
            End If

            If Figlio_Sa_Cod <> 0 Then
                StrSQL.Append(" AND Figlio_Sa_Cod = " & Agro_SQL_SaveNum(Figlio_Sa_Cod) & " ")
            End If

            If Figlio_Appezza <> 0 Then
                StrSQL.Append(" AND Figlio_Appezza = " & Agro_SQL_SaveNum(Figlio_Appezza) & " ")
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

End Class
