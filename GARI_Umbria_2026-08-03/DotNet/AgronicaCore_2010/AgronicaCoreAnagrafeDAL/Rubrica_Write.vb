Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate



Public Class Rubrica_Write
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    'Public Function Scrivi(ByVal Cod_Rubrica As Int32, _
    '                       ByVal NUMERO As String, _
    '                       ByVal Descr As String, _
    '                       ByVal UserName_Creazione As String, _
    '                       ByVal FinestraTemp_Inizio As Date, _
    '                       ByVal FinestraTemp_Fine As Date, _
    '                       ByRef objConnessione As DbConnection, _
    '                       ByRef objTransazione As DbTransaction, _
    '                       ByVal StringaConnessione As String, _
    '                       ByVal DirectoryLOG As String, _
    '                       ByVal FileLOG As String, _
    '                       ByVal IdentificatoreUtente As String _
    '                       ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rubrica_Write.Scrivi()"

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
    '        StrSQL.Append("INSERT INTO Rubrica( ")
    '        StrSQL.Append("                     Cod_Rubrica,        Numero,             Descr,  ")
    '        StrSQL.Append("                     Inviato,            DataInvio, ")
    '        StrSQL.Append("                     Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("                     UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("                     Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("                     ) ")
    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("           " & Agro_SQL_SaveNum(Cod_Rubrica) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(NUMERO) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Descr) & "' ")
    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")
    '        StrSQL.Append(" )")


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



    Public Function Scrivi(ByVal Cod_Rubrica As Int32, _
                           ByVal NUMERO As String, _
                           ByVal Descr As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rubrica_Write.Scrivi()"

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
            StrSQL.Append("INSERT INTO Rubrica( ")
            StrSQL.Append("                     Cod_Rubrica,        Numero,             Descr,  ")
            StrSQL.Append("                     Inviato,            DataInvio, ")
            StrSQL.Append("                     Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                     UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                     Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                     ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(Cod_Rubrica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(NUMERO) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Descr) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(" )")

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






    '############################################################################
    '############################################################################
    '############################################################################



    '============================================================================
    'Public Function Modifica(ByVal Cod_Rubrica As Int32, _
    '                         ByVal NUMERO As String, _
    '                         ByVal Descr As String, _
    '                         ByVal FinestraTemp_Inizio As Date, _
    '                         ByVal FinestraTemp_Fine As Date, _
    '                         ByVal UserName_Modifica As String, _
    '                         ByRef objConnessione As DbConnection, _
    '                         ByRef objTransazione As DbTransaction, _
    '                         ByVal StringaConnessione As String, _
    '                         ByVal DirectoryLOG As String, _
    '                         ByVal FileLOG As String, _
    '                         ByVal IdentificatoreUtente As String _
    '                         ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Rubrica_Write.Modifica()"

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
    '        '---------------------------------------------
    '        If Cod_Rubrica = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Cod_Rubrica obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Rubrica SET ")
    '        StrSQL.Append("    Numero            = '" & Agro_SQL_SaveText(NUMERO) & "'")
    '        StrSQL.Append("   ,Descr             = '" & Agro_SQL_SaveText(Descr) & "'")
    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))

    '        StrSQL.Append(" WHERE Cod_Rubrica = " & Cod_Rubrica & " ")

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


    Public Function Modifica(ByVal Cod_Rubrica As Int32, _
                             ByVal NUMERO As String, _
                             ByVal Descr As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Rubrica_Write.Modifica()"

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
            '---------------------------------------------
            If Cod_Rubrica = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rubrica obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Rubrica SET ")
            StrSQL.Append("    Numero            = '" & Agro_SQL_SaveText(NUMERO) & "'")
            StrSQL.Append("   ,Descr             = '" & Agro_SQL_SaveText(Descr) & "'")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Cod_Rubrica = " & Cod_Rubrica & " ")

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
    'Public Function Cancella(ByVal Cod_Rubrica As String, _
    '                         ByVal UserName_Modifica As String, _
    '                         ByVal FlagCancellazioneLogica As Int32, _
    '                         ByRef objConnessione As DbConnection, _
    '                         ByRef objTransazione As DbTransaction, _
    '                         ByVal StringaConnessione As String, _
    '                         ByVal DirectoryLOG As String, _
    '                         ByVal FileLOG As String, _
    '                         ByVal IdentificatoreUtente As String _
    '                         ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rubrica_Write.Cancella()"

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
    '        If Cod_Rubrica = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Cod_Rubrica obbligatorio)")
    '        End If


    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE Rubrica ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Inviato > 0 ")

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM Rubrica ")
    '            StrSQL.Append(" WHERE Inviato = 0")

    '        End If

    '        StrSQL.Append(" AND Cod_Rubrica = " & Cod_Rubrica)

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


    Public Function Cancella(ByVal Cod_Rubrica As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rubrica_Write.Cancella()"

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
            If Cod_Rubrica = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rubrica obbligatorio)")
            End If


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Rubrica ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato > 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Rubrica ")
                StrSQL.Append(" WHERE Inviato = 0")

            End If

            StrSQL.Append(" AND Cod_Rubrica = " & Cod_Rubrica)

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




    Public Function Aggiungi_Aggiorna(ByRef Cod_Rubrica_Ritorno As Int32, _
                        ByVal NUMERO As String, _
                        ByVal Descr As String, _
                             ByVal Validita_Inizio As Date, _
                             ByVal Validita_Fine As Date, _
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
             , Optional ByVal Data_creazione As Date = #2/1/1900# _
             , Optional ByVal Data_modifica As Date = #2/1/1900# _
             , Optional ByVal username_creazione As String = "" _
             , Optional ByVal username_modifica As String = "" _
                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rubrica_Write.Aggiungi_Aggiorna()"

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


            If Cod_Rubrica_Ritorno <> 0 Then

                'aggiorno il numero
                'controllo se esiste codice altrimenti acczione
                'Dim Rubrica_Read As New AgronicaCoreAnagrafeDAL.Rubrica_Read()
                Dim dt As DataTable = leggi(Cod_Rubrica_Ritorno, objParametri)
                If dt.Rows.Count <> 1 Then
                    Throw New Exception("Il codice Cod_Rubrica non identifica un unico record ma " & dt.Rows.Count)
                End If
                Dim i As Integer = 0
                For i = 0 To dt.Rows.Count - 1
                    xRisp = xRisp Or Modifica(dt.Rows(i).Item("Cod_Rubrica"), NUMERO, Descr, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, "", objParametri)
                Next

            Else


                'scrivo il numero
                Dim sequenza_tabelle As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim basecode = 0
                Dim topcode = UpperBoundTabelle_Per_SequenzaTabelle_Topcode
                Dim cod_rubrica As Integer = sequenza_tabelle.NuovoId_Tabella("rubrica", basecode, topcode, objParametri)

                xRisp = Scrivi(cod_rubrica, NUMERO, Descr,
                                     AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                     AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                                     objParametri)

                Cod_Rubrica_Ritorno = cod_rubrica

            End If


            xRisp = True

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Leggi(ByVal Cod_Rubrica As Int32, _
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Rubrica_Write.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        '------------------------------

        Try
            '---------------------------------------------
            If Cod_Rubrica = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rubrica obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("Select * from Rubrica  ")
            StrSQL.Append(" WHERE Cod_Rubrica = " & Cod_Rubrica & " ")
            'StrSQL.Append("    Numero            = '" & Agro_SQL_SaveText(NUMERO) & "'")
            'StrSQL.Append("   ,Descr             = '" & Agro_SQL_SaveText(Descr) & "'")
            'StrSQL.Append("   ,Inviato           =  0 ")
            'StrSQL.Append("   ,DataInvio         =  Null ")
            'StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            'StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            'StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            'StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))



            '----------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return dt


    End Function




End Class
