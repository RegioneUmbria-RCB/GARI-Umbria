
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Stalla_Caratteristiche_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'Public Function Leggi( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal STA_NUM As Int32, _
    '                        ByVal Cod_Fabb As String, _
    '                        ByVal Att_Cod As Int32, _
    '                            ByVal FinestraTemp_Inizio As Date, _
    '                            ByVal FinestraTemp_Fine As Date, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal FlagVisibilita As Int32, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        'If Cod_Indirizzo = 0 Then
    '        '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
    '        'End If

    '        '------------------------------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM  Stalla_Caratteristiche ")
    '        StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        End If

    '        If STA_NUM <> 0 Then
    '            StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
    '        End If

    '        If Cod_Fabb <> "" Then
    '            StrSQL.Append(" AND COD_FABB = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "'  ")
    '        End If

    '        If Att_Cod <> 0 Then
    '            StrSQL.Append(" AND ATT_COD = " & Agro_SQL_SaveNum(Att_Cod) & "  ")
    '        End If


    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato >=0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato =-1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select


    '        StrSQL.Append(" ORDER BY ATT_COD ASC ")

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
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal STA_NUM As Int32, _
                            ByVal Cod_Fabb As String, _
                            ByVal Att_Cod As Int32, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            'If Cod_Indirizzo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            'End If

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Stalla_Caratteristiche ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If STA_NUM <> 0 Then
                        StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                    End If

                    If Cod_Fabb <> "" Then
                        StrSQL.Append(" AND COD_FABB = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "'  ")
                    End If

                    If Att_Cod <> 0 Then
                        StrSQL.Append(" AND ATT_COD = " & Agro_SQL_SaveNum(Att_Cod) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ATT_COD ASC ")
                    End If




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Stalla_Caratteristiche ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If STA_NUM <> 0 Then
                        StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                    End If

                    If Cod_Fabb <> "" Then
                        StrSQL.Append(" AND COD_FABB = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "'  ")
                    End If

                    If Att_Cod <> 0 Then
                        StrSQL.Append(" AND ATT_COD = " & Agro_SQL_SaveNum(Att_Cod) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ATT_COD ASC ")
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


Public Class Stalla_Caratteristiche_W
    Inherits AgronicaCoreDataProvider.DataProvider




    '============================================================================
    'Public Function Scrivi( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal STA_NUM As Int32, _
    '                        ByVal Cod_Fabb As String, _
    '                        ByVal Att_Cod As Int32, _
    '                        ByVal Valore As Decimal, _
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

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W.Scrivi()"

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

    '        StrSQL.Append(" INSERT INTO Stalla_Caratteristiche(PIVA, Sa_Cod, STA_NUM, ")
    '        StrSQL.Append("          COD_FABB, ATT_COD, VALORE, ")
    '        StrSQL.Append("          Inviato,            DataInvio, ")
    '        StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("         ) ")

    '        StrSQL.Append(" VALUES ( ")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(STA_NUM) & " ")

    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Att_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore) & "  ")

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


    Public Function Scrivi(
                                ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal STA_NUM As Int32,
                                ByVal Cod_Fabb As String,
                                ByVal Att_Cod As Int32,
                                ByVal Valore As Decimal,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Stalla_Caratteristiche(PIVA, Sa_Cod, STA_NUM, ")
            StrSQL.Append("          COD_FABB, ATT_COD, VALORE, ")
            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(STA_NUM) & " ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Att_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore) & "  ")

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






    '============================================================================
    'Public Function Modifica( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Sta_Num As Int32, _
    '                        ByVal Cod_Fabb As String, _
    '                        ByVal Att_Cod As Int32, _
    '                        ByVal Valore As Decimal, _
    '                            ByVal UserName_Modifica As String, _
    '                            ByVal FinestraTemp_Inizio As Date, _
    '                            ByVal FinestraTemp_Fine As Date, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByRef objTransazione As DbTransaction, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W.Modifica()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    '------------------------------

    '    Try

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        If Sa_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
    '        End If

    '        If Sta_Num = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
    '        End If

    '        If Cod_Fabb = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Cod_Fabb obbligatorio)")
    '        End If

    '        If Att_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Att_Cod obbligatorio)")
    '        End If

    '        '---------------------------------------------

    '        StrSQL.Length = 0

    '        StrSQL.Append(" UPDATE Stalla_Caratteristiche SET ")
    '        StrSQL.Append("    VALORE    = " & Agro_SQL_SaveNum(Valore) & "  ")

    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))

    '        StrSQL.Append(" WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append(" AND   STA_NUM   =  " & Agro_SQL_SaveNum(Sta_Num) & "  ")
    '        StrSQL.Append(" AND   COD_FABB  = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
    '        StrSQL.Append(" AND   ATT_COD   =  " & Agro_SQL_SaveNum(Att_Cod) & "  ")

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
                                ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Sta_Num As Int32,
                                ByVal Cod_Fabb As String,
                                ByVal Att_Cod As Int32,
                                ByVal Valore As Decimal,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Sta_Num = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
            End If

            If Cod_Fabb = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Fabb obbligatorio)")
            End If

            If Att_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Att_Cod obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Stalla_Caratteristiche SET ")
            StrSQL.Append("    VALORE    = " & Agro_SQL_SaveNum(Valore) & "  ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND   STA_NUM   =  " & Agro_SQL_SaveNum(Sta_Num) & "  ")
            StrSQL.Append(" AND   COD_FABB  = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
            StrSQL.Append(" AND   ATT_COD   =  " & Agro_SQL_SaveNum(Att_Cod) & "  ")

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




    '============================================================================
    'Public Function Cancella( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Sta_Num As Int32, _
    '                        ByVal Cod_Fabb As String, _
    '                        ByVal Att_Cod As Int32, _
    '                            ByVal UserName_Modifica As String, _
    '                            ByVal FlagCancellazioneLogica As Int32, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByRef objTransazione As DbTransaction, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Cod_Fabb = 0
    '    '   Att_Cod = 0

    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        If Sa_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
    '        End If

    '        If Sta_Num = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE Stalla_Caratteristiche ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Inviato >= 0 ")

    '        Else
    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM Stalla_Caratteristiche ")
    '            StrSQL.Append(" WHERE  1=1 ")

    '        End If

    '        If Cod_Fabb <> "" Then
    '            StrSQL.Append(" AND Cod_Fabb = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
    '        End If

    '        If Att_Cod <> 0 Then
    '            StrSQL.Append(" AND Att_Cod = " & Agro_SQL_SaveNum(Att_Cod) & "   ")
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
                               ByVal Piva As String,
                               ByVal Sa_Cod As Int32,
                               ByVal Sta_Num As Int32,
                               ByVal Cod_Fabb As String,
                               ByVal Att_Cod As Int32,
                                   ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Fabb = 0
        '   Att_Cod = 0

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

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Sta_Num = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Stalla_Caratteristiche ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Stalla_Caratteristiche ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            If Cod_Fabb <> "" Then
                StrSQL.Append(" AND Cod_Fabb = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
            End If

            If Att_Cod <> 0 Then
                StrSQL.Append(" AND Att_Cod = " & Agro_SQL_SaveNum(Att_Cod) & "   ")
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
        End Try

        Return xRisp

    End Function






End Class
