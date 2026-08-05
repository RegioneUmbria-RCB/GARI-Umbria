
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Fabbricati_Impostaz_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    'Public Function Leggi( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Fabbricato_Cod As Int32, _
    '                        ByVal Elem_Cod As Int32, _
    '                        ByVal Gru_Cod As Int32, _
    '                        ByVal Lav_Cod As Int32, _
    '                            ByVal FinestraTemp_Inizio As Date, _
    '                            ByVal FinestraTemp_Fine As Date, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal FlagVisibilita As Int32, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim strSort_Gru_Cod As String
    '    Dim strSort_Lav_Cod As String

    '    Try

    '        'If Cod_Indirizzo = 0 Then
    '        '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
    '        'End If

    '        '------------------------------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" SELECT  Fabbricati_Impostazioni.* ")
    '        StrSQL.Append(" FROM    Fabbricati_Impostazioni ")
    '        StrSQL.Append(" WHERE   Fabbricati_Impostazioni.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND     Fabbricati_Impostazioni.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

    '        'Nota Marco: la Piva_SuperUser al momento è opzionale poichè prensente solo in questa tabella della famiglia 'Fabbricati'

    '        If PivaSuperUser <> "" Then
    '            StrSQL.Append(" AND Fabbricati_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
    '        End If

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND Fabbricati_Impostazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Fabbricati_Impostazioni.Sa_Cod = " & Sa_Cod & " ")
    '        End If

    '        If Fabbricato_Cod <> 0 Then
    '            StrSQL.Append(" AND Fabbricati_Impostazioni.Fabbricato_Cod = " & Fabbricato_Cod & " ")
    '        End If

    '        If Elem_Cod <> 0 Then
    '            StrSQL.Append(" AND Fabbricati_Impostazioni.Elem_Cod = " & Elem_Cod & " ")
    '        End If

    '        If Gru_Cod <> 0 Then
    '            StrSQL.Append(" AND Fabbricati_Impostazioni.Gru_Cod In (" & Agro_SQL_SaveNum(Gru_Cod) & ",0)   ")

    '        End If

    '        If Lav_Cod <> 0 Then
    '            StrSQL.Append(" AND Fabbricati_Impostazioni.Lav_Cod In (" & Agro_SQL_SaveNum(Lav_Cod) & ",0)   ")
    '        End If


    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND   Fabbricati_Impostazioni.Inviato >=0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND   Fabbricati_Impostazioni.Inviato =-1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select


    '        strSort_Gru_Cod = IIf(Gru_Cod <> 0, "Desc", "Asc")
    '        strSort_Lav_Cod = IIf(Lav_Cod <> 0, "Desc", "Asc")


    '        StrSQL.Append(" ORDER BY    Fabbricati_Impostazioni.Piva_SuperUser Asc, ")
    '        StrSQL.Append("             Fabbricati_Impostazioni.Piva Asc,           ")
    '        StrSQL.Append("             Fabbricati_Impostazioni.Elem_Cod ,          ")
    '        StrSQL.Append("             Fabbricati_Impostazioni.Gru_Cod " & strSort_Gru_Cod & ", ")
    '        StrSQL.Append("             Fabbricati_Impostazioni.Lav_Cod " & strSort_Lav_Cod & "  ")

    '        '------------------------------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function



    Public Function Leggi( _
                                ByVal PivaSuperUser As String, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Fabbricato_Cod As Int32, _
                                ByVal Elem_Cod As Int32, _
                                ByVal Gru_Cod As Int32, _
                                ByVal Lav_Cod As Int32, _
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim strSort_Gru_Cod As String
        Dim strSort_Lav_Cod As String

        Try

            'If Cod_Indirizzo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            'End If

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    'TODO
                    'modificare la query di select
                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Fabbricati_Impostazioni.* ")
                    StrSQL.Append(" FROM    Fabbricati_Impostazioni ")
                    StrSQL.Append(" WHERE   Fabbricati_Impostazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Fabbricati_Impostazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    'Nota Marco: la Piva_SuperUser al momento è opzionale poichè prensente solo in questa tabella della famiglia 'Fabbricati'

                    If PivaSuperUser <> "" Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Fabbricato_Cod = " & Fabbricato_Cod & " ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Elem_Cod = " & Elem_Cod & " ")
                    End If

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Gru_Cod In (" & Agro_SQL_Save_Clausola_IN(Gru_Cod) & ",0)   ")

                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Lav_Cod In (" & Agro_SQL_Save_Clausola_IN(Lav_Cod) & ",0)   ")
                    End If





                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Fabbricati_Impostazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Fabbricati_Impostazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSort_Gru_Cod = IIf(Gru_Cod <> 0, "Desc", "Asc")
                        strSort_Lav_Cod = IIf(Lav_Cod <> 0, "Desc", "Asc")


                        StrSQL.Append(" ORDER BY    Fabbricati_Impostazioni.Piva_SuperUser Asc, ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Piva Asc,           ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Elem_Cod ,          ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Gru_Cod " & strSort_Gru_Cod & ", ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Lav_Cod " & strSort_Lav_Cod & "  ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    'TODO
                    'modificare la query di select
                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Fabbricati_Impostazioni.* ")
                    StrSQL.Append(" FROM    Fabbricati_Impostazioni ")
                    StrSQL.Append(" WHERE   Fabbricati_Impostazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Fabbricati_Impostazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    'Nota Marco: la Piva_SuperUser al momento è opzionale poichè prensente solo in questa tabella della famiglia 'Fabbricati'

                    If PivaSuperUser <> "" Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Fabbricato_Cod = " & Fabbricato_Cod & " ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Elem_Cod = " & Elem_Cod & " ")
                    End If

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Gru_Cod In (" & Agro_SQL_Save_Clausola_IN(Gru_Cod) & ",0)   ")

                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricati_Impostazioni.Lav_Cod In (" & Agro_SQL_Save_Clausola_IN(Lav_Cod) & ",0)   ")
                    End If





                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Fabbricati_Impostazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Fabbricati_Impostazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSort_Gru_Cod = IIf(Gru_Cod <> 0, "Desc", "Asc")
                        strSort_Lav_Cod = IIf(Lav_Cod <> 0, "Desc", "Asc")


                        StrSQL.Append(" ORDER BY    Fabbricati_Impostazioni.Piva_SuperUser Asc, ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Piva Asc,           ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Elem_Cod ,          ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Gru_Cod " & strSort_Gru_Cod & ", ")
                        StrSQL.Append("             Fabbricati_Impostazioni.Lav_Cod " & strSort_Lav_Cod & "  ")
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



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§




Public Class Fabbricati_Impostaz_W
    Inherits AgronicaCoreDataProvider.DataProvider




    '============================================================================
    'Public Function Scrivi( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Fabbricato_Cod As Int32, _
    '                        ByVal Elem_Cod As Int32, _
    '                        ByVal Gru_Cod As Int32, _
    '                        ByVal Lav_Cod As Int32, _
    '                            ByVal UserName_Creazione As String, _
    '                            ByVal FinestraTemp_Inizio As Date, _
    '                            ByVal FinestraTemp_Fine As Date, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByRef objTransazione As DbTransaction, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_W.Scrivi()"

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

    '        StrSQL.Append(" INSERT INTO Fabbricati_Impostazioni ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva_SuperUser,     Piva,         Sa_Cod,      ")
    '        StrSQL.Append("          Fabbricato_Cod,     Elem_Cod,     Gru_Cod,     ")
    '        StrSQL.Append("          Lav_Cod,                                       ")

    '        StrSQL.Append("          Inviato,            DataInvio, ")
    '        StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("         ) ")

    '        StrSQL.Append(" VALUES ( ")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(PivaSuperUser)) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")

    '        StrSQL.Append(") ")

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



    Public Function Scrivi( _
                                ByVal PivaSuperUser As String, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Fabbricato_Cod As Int32, _
                                ByVal Elem_Cod As Int32, _
                                ByVal Gru_Cod As Int32, _
                                ByVal Lav_Cod As Int32, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Fabbricati_Impostazioni ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva_SuperUser,     Piva,         Sa_Cod,      ")
            StrSQL.Append("          Fabbricato_Cod,     Elem_Cod,     Gru_Cod,     ")
            StrSQL.Append("          Lav_Cod,                                       ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(PivaSuperUser)) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")

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






    ''============================================================================
    'Public Function Cancella( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Fabbricato_Cod As Int32, _
    '                        ByVal Elem_Cod As Int32, _
    '                        ByVal Gru_Cod As Int32, _
    '                        ByVal Lav_Cod As Int32, _
    '                            ByVal UserName_Modifica As String, _
    '                            ByVal FlagCancellazioneLogica As Int32, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByRef objTransazione As DbTransaction, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""
    '    '   Sa_Cod = 0
    '    '   Fabbricato_Cod = 0
    '    '   Elem_Cod = 0
    '    '   Gru_Cod = 0
    '    '   Lav_Cod = 0
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        If PivaSuperUser = "" Then
    '            Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE Fabbricati_Impostazioni ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Inviato >= 0 ")

    '        Else
    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM Fabbricati_Impostazioni ")
    '            StrSQL.Append(" WHERE  1=1 ")

    '        End If

    '        'Obbligatorio
    '        StrSQL.Append(" AND Piva_SuperUser = '" & Trim(PivaSuperUser) & "' ")


    '        If Piva <> "" Then
    '            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
    '        End If

    '        If Fabbricato_Cod <> 0 Then
    '            StrSQL.Append(" AND Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   ")
    '        End If

    '        If Elem_Cod <> 0 Then
    '            StrSQL.Append(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
    '        End If

    '        If Gru_Cod <> 0 Then
    '            StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & "   ")
    '        End If

    '        If Lav_Cod <> 0 Then
    '            StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
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



    Public Function Cancella( _
                                ByVal PivaSuperUser As String, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Fabbricato_Cod As Int32, _
                                ByVal Elem_Cod As Int32, _
                                ByVal Gru_Cod As Int32, _
                                ByVal Lav_Cod As Int32, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Fabbricato_Cod = 0
        '   Elem_Cod = 0
        '   Gru_Cod = 0
        '   Lav_Cod = 0
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Fabbricati_Impostazioni ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Fabbricati_Impostazioni ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            'Obbligatorio
            StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Trim(PivaSuperUser)) & "' ")


            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   ")
            End If

            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Gru_Cod <> 0 Then
                StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If


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
