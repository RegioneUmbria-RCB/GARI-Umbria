
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Utenti_GDPR_Accettazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        GDPR_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"
        '----- Variabili
        Dim Stb As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            Stb.AppendLine(" SELECT * " + vbCrLf)
            Stb.AppendLine(" From Utenti_GDPR_Accettazione ")
            Stb.AppendLine(" Where piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If GDPR_Cod <> 0 Then
                Stb.AppendLine(" And GDPR_Cod =  " & Agro_SQL_SaveNum(GDPR_Cod))
            End If

            Stb.AppendLine(" And username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            Stb.AppendLine(" ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    Public Function LeggiXUtente(
        ByVal GDPR_Cod As Integer,
        ByVal Utente As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiXUtente()"
        '----- Variabili
        Dim Stb As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        If Utente = "" Then
            Throw New Exception("Inserire utente")
        End If

        Try
            Stb.AppendLine(" SELECT * " + vbCrLf)
            Stb.AppendLine(" From Utenti_GDPR_Accettazione ")
            Stb.AppendLine(" Where piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If GDPR_Cod <> 0 Then
                Stb.AppendLine(" And GDPR_Cod =  " & Agro_SQL_SaveNum(GDPR_Cod))
            End If
            If Utente <> "" Then
                Stb.AppendLine(" And username = '" & Agro_SQL_SaveText(Utente) & "' ")
            End If

            Stb.AppendLine(" ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    Public Function LeggiTutti(
        xFiltroAggiuntivo As String, xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiTutti()"
        '----- Variabili
        Dim Stb As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            Stb.AppendLine(" SELECT ud.username, cognome, nome, accettazione_dataora ")
            Stb.AppendLine(" FROM Utenti_GDPR_Accettazione  gdpr ")
            Stb.AppendLine(" JOIN Utenti_dettagli ud ON ud.Username  = gdpr.username  ")
            Stb.AppendLine(" WHERE 1=1  ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    ''' <summary>
    ''' Legge lo stato di accerttazione del gdpr per più utenti.
    ''' </summary>
    ''' <remarks>
    ''' Conveniente in termini di tempistiche se si deve leggere i dati di un
    ''' numero ristretto di utenti su database molto grandi. Se è necessario
    ''' leggere molti utenti in una sola volta (più di 10_000) la funzione
    ''' richiamerà direttamente la funzione <tt>LeggiTutti</tt>. Su
    ''' database piccoli (meno di 10_000 utenti) il risultato sarà lo stesso
    ''' che chiamare la funzione <tt>LeggiTutti</tt>.
    ''' </remarks>
    ''' <param name="users">Lista di utenti per cui verificare lo stato di accettazione gdpr.</param>
    Public Function LeggiPer(
        users As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.IUtente),
        xFiltroAggiuntivo As String, xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiPer()"
        '----- Variabili
        Dim Stb As New System.Text.StringBuilder With {.Length = 0}
        Dim usersFilter = String.Empty
        Dim DT As DataTable

        If users.Count > 10_000 Then
            Return LeggiTutti(xFiltroAggiuntivo, xOrderBy, objParametri)
        Else
            Dim prepareFilter = Function(acc, u)
                                    If u.index Mod 100 = 0 Then
                                        acc.name &= ", --" & u.index & vbNewLine & u.name
                                    ElseIf u.index Mod 10 = 0 Then
                                        acc.name &= ", " & vbNewLine & u.name
                                    Else
                                        acc.name &= ", " & u.name
                                    End If
                                    Return acc
                                End Function
            usersFilter = users.AsParallel.
                Select(Function(u, i) New With {.name = "'" & u.UserName & "'", .index = i}).
                Aggregate(prepareFilter).name
        End If

        Try
            Stb.AppendLine(" SELECT ud.username, cognome, nome, accettazione_dataora ")
            Stb.AppendLine(" FROM Utenti_GDPR_Accettazione  gdpr ")
            Stb.AppendLine(" JOIN Utenti_dettagli ud ON ud.Username  = gdpr.username  ")
            Stb.AppendLine(" WHERE 1=1  ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & usersFilter)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Utenti_GDPR_Accettazione_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                    GDPR_Cod As Integer,
                    Accettazione_Stato As Integer,
                    Accettazione_DataOra As Date,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT Utenti_GDPR_Accettazione " + vbCrLf)

            Stb.Append("              (")
            Stb.AppendLine("         Piva_SuperUser , ")
            Stb.AppendLine("         GDPR_Cod, ")
            Stb.AppendLine("         Username, ")
            Stb.AppendLine("         Accettazione_Stato , ")
            Stb.AppendLine("         Accettazione_DataOra,")

            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")


            Stb.Append("           '" & objParametri.PivaSuperUser & "'  " & vbCrLf)
            Stb.Append("         , " & GDPR_Cod & "  " & vbCrLf)
            Stb.Append("         ,  '" & objParametri.UtenteUsername & "' " & vbCrLf)
            Stb.Append("         , " & Accettazione_Stato & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveDateTime(Accettazione_DataOra) & "  " & vbCrLf)


            Stb.Append("         , 0  " & vbCrLf)
            Stb.Append("         , Null  " & vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function ScriviXutente(ByVal Utente As String,
                                  ByVal GDPR_Cod As Integer,
                                  ByVal Accettazione_Stato As Integer,
                                  ByVal Accettazione_DataOra As Date,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                                    , Optional ByVal username_creazione As String = "" _
                                    , Optional ByVal username_modifica As String = ""
                                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT Utenti_GDPR_Accettazione " + vbCrLf)

            Stb.Append("              (")
            Stb.AppendLine("         Piva_SuperUser , ")
            Stb.AppendLine("         GDPR_Cod, ")
            Stb.AppendLine("         Username, ")
            Stb.AppendLine("         Accettazione_Stato , ")
            Stb.AppendLine("         Accettazione_DataOra,")

            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")


            Stb.Append("           '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(GDPR_Cod) & "  " & vbCrLf)
            Stb.Append("         ,  '" & Agro_SQL_SaveText(Utente) & "' " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(Accettazione_Stato) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveDateTime(Accettazione_DataOra) & "  " & vbCrLf)


            Stb.Append("         , 0  " & vbCrLf)
            Stb.Append("         , Null  " & vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function





    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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
