Imports System.Data.Common

Public Class Web_ComunicazionePagine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    'Public Function Leggi(ByVal Unid As String,
    '                        ByVal Id_Riga As Int32,
    '                        ByVal FinestraTemp_Inizio As Date,
    '                        ByVal FinestraTemp_Fine As Date,
    '                        ByRef objConnessione As DbConnection,
    '                        ByVal StringaConnessione As String,
    '                        ByVal DirectoryLOG As String,
    '                        ByVal FileLOG As String,
    '                        ByVal IdentificatoreUtente As String) _
    '                            As DataTable


    '    Const nomeRoutine = "AgronicaCoreVarieDAL.Web_ComunicazionePagine_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim dt As DataTable

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM  Web_ComunicazionePagine ")
    '        StrSQL.Append(" WHERE Unid ='" & Agro_SQL_SaveText(Trim(Unid)) & "' ")
    '        StrSQL.Append(" AND Id_Riga =" & Agro_SQL_SaveNum(Trim(Id_Riga)) & " ")

    '        '---------------------------------------------

    '        dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    Return dt

    'End Function




    Public Function Leggi(ByVal Unid As String, _
                            ByVal Id_Riga As Int32, _
                            ByVal FinestraTemp_Inizio As Date, _
                            ByVal FinestraTemp_Fine As Date, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Const nomeRoutine = "AgronicaCoreVarieDAL.Web_ComunicazionePagine_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Web_ComunicazionePagine ")
                    StrSQL.Append(" WHERE Unid ='" & Agro_SQL_SaveText(Trim(Unid)) & "' ")
                    If Id_Riga <> 0 Then
                        StrSQL.Append(" AND Id_Riga =" & Agro_SQL_SaveNum(Trim(Id_Riga)) & " ")
                    End If


                    '---------------------------------------------



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Web_ComunicazionePagine ")
                    StrSQL.Append(" WHERE Unid ='" & Agro_SQL_SaveText(Trim(Unid)) & "' ")
                    If Id_Riga <> 0 Then
                        StrSQL.Append(" AND Id_Riga =" & Agro_SQL_SaveNum(Trim(Id_Riga)) & " ")
                    End If

                    '---------------------------------------------



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


End Class




'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§





Public Class Web_ComunicazionePagine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    'Public Function Scrivi(ByVal Unid As String,
    '                       ByVal Id_Riga As Int32,
    '                       ByVal Tipo_Operazione As Int32,
    '                       ByVal Flag_Errore As Int32,
    '                       ByVal Errore As String,
    '                       ByVal Chiave_Oggetto As String,
    '                       ByVal Stringa_Parametri_Base As String,
    '                       ByVal Stringa_Parametri_Rif As String,
    '                       ByRef objConnessione As DbConnection,
    '                       ByRef objTransazione As DbTransaction,
    '                       ByVal StringaConnessione As String,
    '                       ByVal DirectoryLOG As String,
    '                       ByVal FileLOG As String,
    '                       ByVal IdentificatoreUtente As String
    '                       ) As Boolean


    '    Const nomeRoutine = "AgronicaCoreVarieDAL.Web_ComunicazionePagine_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" INSERT INTO Web_ComunicazionePagine ")

    '        StrSQL.Append(" (Unid, Id_Riga, Tipo_Operazione, Flag_Errore, Errore, Chiave_Oggetto, Stringa_Parametri_Base, Stringa_Parametri_Rif, Data, Username) ")

    '        StrSQL.Append(" VALUES ('" & Agro_SQL_SaveText(Trim(Unid)) & "', ")
    '        StrSQL.Append(" " & Agro_SQL_SaveNum(Id_Riga) & ", ")
    '        StrSQL.Append(" " & Agro_SQL_SaveNum(Tipo_Operazione) & ", ")
    '        StrSQL.Append(" " & Agro_SQL_SaveNum(Flag_Errore) & ", ")
    '        StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Errore)) & "', ")
    '        StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Chiave_Oggetto)) & "', ")
    '        StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Stringa_Parametri_Base)) & "', ")
    '        StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Stringa_Parametri_Rif)) & "', ")
    '        StrSQL.Append(Agro_SQL_SaveDate(Now.Today) & ", ")
    '        StrSQL.Append("'" & Agro_SQL_SaveText(IdentificatoreUtente) & "' ) ")


    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    Return xRisp


    'End Function



    Public Function Scrivi(ByVal Unid As String, _
                           ByVal Id_Riga As Int32, _
                           ByVal Tipo_Operazione As Int32, _
                           ByVal Flag_Errore As Int32, _
                           ByVal Errore As String, _
                           ByVal Chiave_Oggetto As String, _
                           ByVal Stringa_Parametri_Base As String, _
                           ByVal Stringa_Parametri_Rif As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean


        Const nomeRoutine = "AgronicaCoreVarieDAL.Web_ComunicazionePagine_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Web_ComunicazionePagine ")

            StrSQL.Append(" (Unid, Id_Riga, Tipo_Operazione, Flag_Errore, Errore, Chiave_Oggetto, Stringa_Parametri_Base, Stringa_Parametri_Rif, Data, Username) ")

            StrSQL.Append(" VALUES ('" & Agro_SQL_SaveText(Trim(Unid)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Id_Riga) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Tipo_Operazione) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Flag_Errore) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Errore)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Chiave_Oggetto)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Stringa_Parametri_Base)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Stringa_Parametri_Rif)) & "', ")
            StrSQL.Append(Agro_SQL_SaveDate(Now.Today) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function



    '============================================================================
    'Public Function Cancella(ByVal Unid As String,
    '                        ByVal Id_Riga As Int32,
    '                        ByRef objConnessione As DbConnection,
    '                        ByRef objTransazione As DbTransaction,
    '                        ByVal StringaConnessione As String,
    '                        ByVal DirectoryLOG As String,
    '                        ByVal FileLOG As String,
    '                        ByVal IdentificatoreUtente As String
    '                        ) As Boolean


    '    Const nomeRoutine = "AgronicaCoreVarieDAL.Web_ComunicazionePagine_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" DELETE FROM Web_ComunicazionePagine ")
    '        StrSQL.Append(" WHERE Unid ='" & Agro_SQL_SaveText(Trim(Unid)) & "' ")
    '        StrSQL.Append(" AND Id_Riga =" & Agro_SQL_SaveNum(Trim(Id_Riga)) & " ")

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    Return xRisp


    'End Function





    Public Function Cancella(ByVal Unid As String, _
                            ByVal Id_Riga As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean


        Const nomeRoutine = "AgronicaCoreVarieDAL.Web_ComunicazionePagine_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" DELETE FROM Web_ComunicazionePagine ")
            StrSQL.Append(" WHERE Unid ='" & Agro_SQL_SaveText(Trim(Unid)) & "' ")
            StrSQL.Append(" AND Id_Riga =" & Agro_SQL_SaveNum(Trim(Id_Riga)) & " ")

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return xRisp


    End Function




    Public Function CancellaTutti(ByVal Unid As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean


        Const nomeRoutine = "AgronicaCoreVarieDAL.Web_ComunicazionePagine_W.CancellaTutti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" DELETE FROM Web_ComunicazionePagine ")
            StrSQL.Append(" WHERE Unid ='" & Agro_SQL_SaveText(Trim(Unid)) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return xRisp


    End Function




End Class
