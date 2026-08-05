Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class UtentixStrutture_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Public Function Leggi( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal FlagVisibilita As Int32, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UtentixStrutture_Read.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""                   =>  si leggono tutte le imprese
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
    '        StrSQL.Append(" FROM  UtentixStrutture , Centri_Aziendali ")
    '        StrSQL.Append(" WHERE Centri_Aziendali.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   UtentixStrutture.PIVA = Centri_Aziendali.PIVA ")
    '        StrSQL.Append(" AND   UtentixStrutture.Sa_Cod = Centri_Aziendali.Sa_Cod ")

    '        If PivaSuperUser <> "" Then
    '            StrSQL.Append(" AND UtentixStrutture.[User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
    '        End If

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND UtentixStrutture.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND UtentixStrutture.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If

    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND     UtentixStrutture.Inviato >= 0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND     UtentixStrutture.Inviato = -1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        StrSQL.Append(" ORDER BY Centri_Aziendali.Sa_Nome ASC")
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


    Public Function Leggi(
                           ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                               ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal TipoG2G As Integer = 0
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UtentixStrutture_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
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

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  UtentixStrutture , Centri_Aziendali ")
                    StrSQL.Append(" WHERE Centri_Aziendali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UtentixStrutture.PIVA = Centri_Aziendali.PIVA ")
                    StrSQL.Append(" AND   UtentixStrutture.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND UtentixStrutture.[User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND UtentixStrutture.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND UtentixStrutture.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_imprese rr where rr.From_Piva = Centri_Aziendali.piva and rr.From_SaCod = Centri_Aziendali.Sa_cod ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.Append("and exists ( " & vbCrLf)
                            StrSQL.Append("    select 1 " & vbCrLf)
                            StrSQL.Append("    from g2g_recode_imprese rr " & vbCrLf)
                            StrSQL.Append("    where rr.DataInvio < Centri_Aziendali.Data_Modifica " & vbCrLf)
                            StrSQL.Append("    and rr.From_Piva = Centri_Aziendali.piva  " & vbCrLf)
                            StrSQL.Append("    and rr.From_SaCod = Centri_Aziendali.Sa_cod  " & vbCrLf)
                            StrSQL.Append(" ) " & vbCrLf)

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     UtentixStrutture.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     UtentixStrutture.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Centri_Aziendali.Sa_Nome ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" , UtentixStrutture.validita_Inizio as UtentixStrutture_validita_inizio")
                    StrSQL.Append(" , UtentixStrutture.validita_Fine as UtentixStrutture_validita_fine")
                    StrSQL.Append(" , UtentixStrutture.data_creazione as UtentixStrutture_data_creazione")
                    StrSQL.Append(" , UtentixStrutture.data_modifica as UtentixStrutture_data_modifica")
                    StrSQL.Append(" , UtentixStrutture.username_creazione as UtentixStrutture_username_creazione")
                    StrSQL.Append(" , UtentixStrutture.username_modifica as UtentixStrutture_username_modifica")

                    StrSQL.Append(" FROM  UtentixStrutture , Centri_Aziendali ")
                    StrSQL.Append(" WHERE Centri_Aziendali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UtentixStrutture.PIVA = Centri_Aziendali.PIVA ")
                    StrSQL.Append(" AND   UtentixStrutture.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND UtentixStrutture.[User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND UtentixStrutture.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND UtentixStrutture.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_imprese rr where rr.From_Piva = Centri_Aziendali.piva and rr.From_SaCod = Centri_Aziendali.Sa_cod ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.Append("and exists ( " & vbCrLf)
                            StrSQL.Append("    select 1 " & vbCrLf)
                            StrSQL.Append("    from g2g_recode_imprese rr " & vbCrLf)
                            StrSQL.Append("    where rr.DataInvio < Centri_Aziendali.Data_Modifica " & vbCrLf)
                            StrSQL.Append("    and rr.From_Piva = Centri_Aziendali.piva  " & vbCrLf)
                            StrSQL.Append("    and rr.From_SaCod = Centri_Aziendali.Sa_cod  " & vbCrLf)
                            StrSQL.Append(" ) " & vbCrLf)

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     UtentixStrutture.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     UtentixStrutture.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Centri_Aziendali.Sa_Nome ASC")
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




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class UtentixStrutture_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Public Function Scrivi( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
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

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.Scrivi()"

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
    '        StrSQL.Append("INSERT INTO UtentixStrutture(       ")
    '        StrSQL.Append("                    [User],         ")
    '        StrSQL.Append("                    PIVA,                Sa_Cod,      ")
    '        StrSQL.Append("                    Inviato,             DataInvio, ")
    '        StrSQL.Append("                    Data_Creazione,      Data_Modifica, ")
    '        StrSQL.Append("                    UserName_Creazione,  UserName_Modifica, ")
    '        StrSQL.Append("                    Validita_Inizio,     Validita_Fine ")
    '        StrSQL.Append("                    ) ")
    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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


    Public Function Scrivi(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.Scrivi()"

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
            StrSQL.Append("INSERT INTO UtentixStrutture(       ")
            StrSQL.Append("                    [User],         ")
            StrSQL.Append("                    PIVA,                Sa_Cod,      ")
            StrSQL.Append("                    Inviato,             DataInvio, ")
            StrSQL.Append("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,     Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append(", " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append(", " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append(",'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append(",'" & Agro_SQL_SaveText(username_modifica) & "' ")
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
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.Modifica()"

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
    '        StrSQL.Append("UPDATE UtentixStrutture SET ")
    '        StrSQL.Append("        Inviato           =  0 ")
    '        StrSQL.Append("       ,DataInvio         =  Null ")
    '        StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE Piva='" & Trim(Piva) & "'")
    '        StrSQL.Append(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
    '        StrSQL.Append(" AND   Sa_Cod= " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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


    Public Function Modifica(
                            ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.Modifica()"

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
            StrSQL.Append("UPDATE UtentixStrutture SET ")
            StrSQL.Append("        Inviato           =  0 ")
            StrSQL.Append("       ,DataInvio         =  Null ")
            StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva='" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
            StrSQL.Append(" AND   Sa_Cod= " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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



    '##############################################################################################
    'Public Function Cancella( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FlagCancellazioneLogica As Int32, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.Cancella()"

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
    '            StrSQL.Append(" UPDATE UtentixStrutture ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" AND Inviato >= 0")

    '            If Sa_Cod <> 0 Then
    '                StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '            End If

    '            If PivaSuperUser <> "" Then
    '                StrSQL.Append(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
    '            End If

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM     UtentixStrutture ")
    '            StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

    '            If Sa_Cod <> 0 Then
    '                StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '            End If

    '            If PivaSuperUser <> "" Then
    '                StrSQL.Append(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
    '            End If

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

    Public Function Cancella(
                            ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.Cancella()"

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
                StrSQL.Append(" UPDATE UtentixStrutture ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If PivaSuperUser <> "" Then
                    StrSQL.Append(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     UtentixStrutture ")
                StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If PivaSuperUser <> "" Then
                    StrSQL.Append(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
                End If

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

    '##############################################################################################
    'Public Function AggiornaValiditaInizio( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.AggiornaValiditaInizio()"

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
    '        StrSQL.Append("UPDATE UtentixStrutture SET ")
    '        StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        StrSQL.Append(" AND  [User]= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
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


    Public Function AggiornaValiditaInizio(
                            ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.AggiornaValiditaInizio()"

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
            StrSQL.Append("UPDATE UtentixStrutture SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND  [User]= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))
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
    'Public Function AggiornaValiditaFine( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.AggiornaValiditaFine()"

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
    '        StrSQL.Append("UPDATE UtentixStrutture SET ")
    '        StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        StrSQL.Append(" AND  [User]= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
    '        StrSQL.Append(" AND  Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Fine))
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


    Public Function AggiornaValiditaFine(
                            ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixStrutture_Write.AggiornaValiditaFine()"

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
            StrSQL.Append("UPDATE UtentixStrutture SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND  [User]= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            StrSQL.Append(" AND  Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))
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



End Class
