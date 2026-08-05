Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class UtentixFabbricati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal username As String,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Fabbricato_Cod As Integer,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UtentixFabbricati_Read.Leggi()"

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


            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  UtentiXFabbricati , Fabbricati ")
            StrSQL.Append(" WHERE Fabbricati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   UtentiXFabbricati.PIVA = Fabbricati.PIVA ")
            StrSQL.Append(" AND   UtentiXFabbricati.Sa_Cod = Fabbricati.Sa_Cod ")
            StrSQL.Append(" AND   UtentiXFabbricati.Fabbricato_Cod = Fabbricati.Fabbricato_Cod ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND UtentiXFabbricati.[User] = '" & Agro_SQL_SaveText(username) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND UtentiXFabbricati.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND UtentiXFabbricati.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     UtentiXFabbricati.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     UtentiXFabbricati.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Fabbricati.Fabbricato_Des ASC")
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


Public Class UtentixFabbricati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal username As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Fabbricato_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            , Optional ByVal Data_creazione As Date = #2/1/1900# _
                            , Optional ByVal Data_modifica As Date = #2/1/1900# _
                            , Optional ByVal username_creazione As String = "" _
                            , Optional ByVal username_modifica As String = ""
                            ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentiXFabbricati_Write.Scrivi()"

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
            StrSQL.Append("INSERT INTO UtentiXFabbricati(       ")
            StrSQL.Append("                    [User],         ")
            StrSQL.Append("                    PIVA,                Sa_Cod,      ")
            StrSQL.Append("                    Fabbricato_Cod,      ")
            StrSQL.Append("                    Inviato,             DataInvio, ")
            StrSQL.Append("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,     Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(username) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
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

    Public Function Modifica(
                            ByVal Username As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Fabbricato_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentiXFabbricati_Write.Modifica()"

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
            StrSQL.Append("UPDATE UtentiXFabbricati SET ")
            StrSQL.Append("        Inviato           =  0 ")
            StrSQL.Append("       ,DataInvio         =  Null ")
            StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva='" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   [User] = '" & Replace(Username, "'", "''") & "' ")
            StrSQL.Append(" AND   Sa_Cod= " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Fabbricato_Cod= " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
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

    Public Function Cancella(
                            ByVal Username As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Fabbricato_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentiXFabbricati_Write.Cancella()"

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
                StrSQL.Append(" UPDATE UtentiXFabbricati ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If Fabbricato_Cod <> 0 Then
                    StrSQL.Append(" AND  Fabbricato_Cod =  " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                End If

                If Username <> "" Then
                    StrSQL.Append(" AND   [User] = '" & Replace(Username, "'", "''") & "' ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     UtentiXFabbricati ")
                StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If Fabbricato_Cod <> 0 Then
                    StrSQL.Append(" AND  Fabbricato_Cod =  " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                End If

                If Username <> "" Then
                    StrSQL.Append(" AND   [User] = '" & Replace(Username, "'", "''") & "' ")
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

End Class
