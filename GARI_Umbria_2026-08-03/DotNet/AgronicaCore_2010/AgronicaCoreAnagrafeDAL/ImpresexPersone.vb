Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class ImpresexPersone_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Public Function Leggi( _
    '                        ByVal cod_fis As String, _
    '                        ByVal Piva As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal FlagVisibilita As Int32, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_R.Leggi()"

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
    '        StrSQL.Append(" SELECT * , ")
    '        StrSQL.Append("       ImpresexPersone.Validita_Inizio as xValidita_Inizio, ")
    '        StrSQL.Append("       ImpresexPersone.Validita_Fine as xValidita_Fine ")
    '        StrSQL.Append(" FROM  ImpresexPersone , Persone ")
    '        StrSQL.Append(" WHERE ImpresexPersone.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   ImpresexPersone.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   Persone.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Persone.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   ImpresexPersone.Cod_Fis = Persone.Cod_Fis ")

    '        If cod_fis <> "" Then
    '            StrSQL.Append(" AND ImpresexPersone.Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
    '        End If

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND ImpresexPersone.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        End If

    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND     ImpresexPersone.Inviato >= 0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND     ImpresexPersone.Inviato = -1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        StrSQL.Append(" ORDER BY Persone.Cognome ASC, Persone.Nome ASC ")
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


    Public Function Leggi( _
                           ByVal cod_fis As String, _
                           ByVal Piva As String, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_R.Leggi()"

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
                    'TODO
                    'modificare query select

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * , ")
                    StrSQL.Append("       ImpresexPersone.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.Append("       ImpresexPersone.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  ImpresexPersone , Persone ")
                    StrSQL.Append(" WHERE ImpresexPersone.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ImpresexPersone.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Persone.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Persone.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ImpresexPersone.Cod_Fis = Persone.Cod_Fis ")

                    If cod_fis <> "" Then
                        StrSQL.Append(" AND ImpresexPersone.Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND ImpresexPersone.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     ImpresexPersone.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     ImpresexPersone.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Persone.Cognome ASC, Persone.Nome ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * , ")
                    StrSQL.Append("       ImpresexPersone.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.Append("       ImpresexPersone.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  ImpresexPersone , Persone ")
                    StrSQL.Append(" WHERE ImpresexPersone.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ImpresexPersone.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Persone.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Persone.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ImpresexPersone.Cod_Fis = Persone.Cod_Fis ")

                    If cod_fis <> "" Then
                        StrSQL.Append(" AND ImpresexPersone.Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND ImpresexPersone.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     ImpresexPersone.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     ImpresexPersone.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Persone.Cognome ASC, Persone.Nome ASC ")
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

        Return DT

    End Function

End Class




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################







Public Class ImpresexPersone_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Public Function Scrivi( _
    '                        ByVal cod_fis As String, _
    '                        ByVal Piva As String, _
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

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_W.Scrivi()"

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
    '        StrSQL.Append("INSERT INTO ImpresexPersone(       ")
    '        StrSQL.Append("                    Cod_Fis,         ")
    '        StrSQL.Append("                    PIVA,           ")
    '        StrSQL.Append("                    Inviato,            DataInvio, ")
    '        StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("                    ) ")
    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Agro_SQL_SaveText(cod_fis)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
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



    Public Function Scrivi( _
                                ByVal cod_fis As String, _
                                ByVal Piva As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO ImpresexPersone(       ")
            StrSQL.Append("                    Cod_Fis,         ")
            StrSQL.Append("                    PIVA,           ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Agro_SQL_SaveText(cod_fis)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
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
    '                        ByVal cod_fis As String, _
    '                        ByVal Piva As String, _
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

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_W.Modifica()"

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
    '        StrSQL.Append("UPDATE ImpresexPersone SET ")
    '        StrSQL.Append("        Inviato           =  0 ")
    '        StrSQL.Append("       ,DataInvio         =  Null ")
    '        StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE    Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
    '        StrSQL.Append(" AND      PIVA = '" & Agro_SQL_SaveText(Piva) & "'  ")
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

    Public Function Modifica( _
                            ByVal cod_fis As String, _
                            ByVal Piva As String, _
                             ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_W.Modifica()"

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
            StrSQL.Append("UPDATE ImpresexPersone SET ")
            StrSQL.Append("        Inviato           =  0 ")
            StrSQL.Append("       ,DataInvio         =  Null ")
            StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE    Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
            StrSQL.Append(" AND      PIVA = '" & Agro_SQL_SaveText(Piva) & "'  ")
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
    '                        ByVal cod_fis As String, _
    '                        ByVal Piva As String, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FlagCancellazioneLogica As Int32, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_W.Cancella()"

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
    '            StrSQL.Append(" UPDATE ImpresexPersone ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" AND Inviato >= 0")

    '            If cod_fis <> "" Then
    '                StrSQL.Append(" AND   Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
    '            End If

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM     ImpresexPersone ")
    '            StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

    '            If cod_fis <> "" Then
    '                StrSQL.Append(" AND   Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
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



    Public Function Cancella( _
                               ByVal cod_fis As String, _
                               ByVal Piva As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexPersone_W.Cancella()"

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
                StrSQL.Append(" UPDATE ImpresexPersone ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

                If cod_fis <> "" Then
                    StrSQL.Append(" AND   Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     ImpresexPersone ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

                If cod_fis <> "" Then
                    StrSQL.Append(" AND   Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
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
    '                        ByVal cod_fis As String, _
    '                        ByVal Piva As String, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ImpresexPersone_W.AggiornaValiditaInizio()"

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
    '        StrSQL.Append("UPDATE ImpresexPersone SET ")
    '        StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(FinestraTemp_Inizio))

    '        If cod_fis <> "" Then
    '            StrSQL.Append(" AND   Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
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

    Public Function AggiornaValiditaInizio( _
                            ByVal cod_fis As String, _
                            ByVal Piva As String, _
                             ByVal Validita_Inizio As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ImpresexPersone_W.AggiornaValiditaInizio()"

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
            StrSQL.Append("UPDATE ImpresexPersone SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If cod_fis <> "" Then
                StrSQL.Append(" AND   Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
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
    'Public Function AggiornaValiditaFine( _
    '                        ByVal cod_fis As String, _
    '                        ByVal Piva As String, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ImpresexPersone_W.AggiornaValiditaFine()"

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
    '        StrSQL.Append("UPDATE ImpresexPersone SET ")
    '        StrSQL.Append(" UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append(" ,Validita_Fine   =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Fine))

    '        If cod_fis <> "" Then
    '            StrSQL.Append(" AND Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
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

    Public Function AggiornaValiditaFine( _
                            ByVal cod_fis As String, _
                            ByVal Piva As String, _
                               ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ImpresexPersone_W.AggiornaValiditaFine()"

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
            StrSQL.Append("UPDATE ImpresexPersone SET ")
            StrSQL.Append(" UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If cod_fis <> "" Then
                StrSQL.Append(" AND Cod_Fis = '" & Agro_SQL_SaveText(cod_fis) & "' ")
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
