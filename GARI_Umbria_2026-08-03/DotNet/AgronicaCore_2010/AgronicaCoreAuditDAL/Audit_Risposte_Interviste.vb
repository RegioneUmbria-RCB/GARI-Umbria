Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Audit_Risposte_Interviste_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                            ByVal Audit_Tipo As Int32, _
                            ByVal Regolamento_Cod As Int32, _
                            ByVal Intervista_Cod As Int32, _
                            ByVal Domanda_Cod As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_Interviste_R.Leggi()"

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

            '------------------------------------------------------------------

            'TODO
            'modificare la query di select
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Audit_Risposte_Interviste.*, Audit_Domande_Interviste.Tipo ")
            StrSQL.Append(" FROM  Audit_Risposte_Interviste ")
            StrSQL.Append(" INNER JOIN Audit_Domande_Interviste ON Audit_Risposte_Interviste.Domanda_Cod = Audit_Domande_Interviste.Domanda_Cod ")
            StrSQL.Append(" AND Audit_Risposte_Interviste.Audit_Tipo = Audit_Domande_Interviste.Audit_Tipo ")
            StrSQL.Append(" AND Audit_Risposte_Interviste.Regolamento_Cod = Audit_Domande_Interviste.Regolamento_Cod ")
            StrSQL.Append(" WHERE Audit_Risposte_Interviste.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Audit_Risposte_Interviste.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Intervista_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Domande_Interviste.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Domande_Interviste.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            If Intervista_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & " ")
            End If

            If Domanda_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Domanda_Cod = " & Agro_SQL_SaveNum(Domanda_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Audit_Risposte_Interviste.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Audit_Risposte_Interviste.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Intervista_SuperUser, Intervista_Cod, Audit_Risposte_Interviste.Domanda_Cod Asc ")
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

    Public Function LeggiRisposte(
                            ByVal Audit_Tipo As Int32,
                            ByVal Regolamento_Cod As Int32,
                            ByVal Intervista_Cod As Int32,
                            ByVal Domanda_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_Interviste_R.LeggiRisposte()"

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

            '------------------------------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Audit_Risposte_Interviste.*")
            StrSQL.Append(" FROM  Audit_Risposte_Interviste ")
            StrSQL.Append(" WHERE Audit_Risposte_Interviste.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Audit_Risposte_Interviste.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Intervista_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            If Intervista_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & " ")
            End If

            If Domanda_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte_Interviste.Domanda_Cod = " & Agro_SQL_SaveNum(Domanda_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Intervista_SuperUser, Intervista_Cod, Audit_Risposte_Interviste.Domanda_Cod Asc ")
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

Public Class Audit_Risposte_Interviste_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi( _
                        ByVal Audit_Tipo As Integer _
                        ,ByVal Regolamento_Cod As Integer _
                        ,ByVal Intervista_Cod As Integer _
                        ,ByVal Domanda_Cod As Integer _
                        ,ByVal Valore As String _
                        ,ByVal Validita_Inizio As Date _
                        ,ByVal Validita_Fine As Date _
                        ,ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_creazione As Date = #2/1/1900# _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_creazione As String = "" _
                        , Optional ByVal username_modifica As String = "" _
                                ) As Boolean




        If #2/1/1900# = (Data_creazione) Then
            Data_creazione = Date.Now
        End If

        If #2/1/1900# = Data_modifica Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_Interviste_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Audit_Risposte_Interviste( ")
            StrSQL.Append("       Audit_Tipo, Regolamento_Cod, Intervista_Cod, Intervista_SuperUser, Domanda_Cod, Valore,  ")
            StrSQL.Append("       Inviato, DataInvio, ")
            StrSQL.Append("       Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("       UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("       Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("       ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(Audit_Tipo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Intervista_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Domanda_Cod) & "  ")
            If IsNothing(Valore) Then
                StrSQL.Append(", NULL ")
            Else
                StrSQL.Append(",'" & Agro_SQL_SaveText(Valore) & "'" & vbCrLf)
            End If

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
            '--------------------------------------------

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

    Public Function ScriviRisposte(
                        ByVal Audit_Tipo As Integer _
                        , ByVal Regolamento_Cod As Integer _
                        , ByVal Intervista_Cod As Integer _
                        , ByVal Piva As String _
                        , ByVal Domanda_Cod As Long _
                        , ByVal Valore As String _
                        , ByVal Validita_Inizio As Date _
                        , ByVal Validita_Fine As Date _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_creazione As Date = #2/1/1900# _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_creazione As String = "" _
                        , Optional ByVal username_modifica As String = ""
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_Interviste_W.ScriviRisposte()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        If #2/1/1900# = Data_creazione Then
            Data_creazione = Date.Now
        End If

        If #2/1/1900# = Data_modifica Then
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
            StrSQL.Append("INSERT INTO Audit_Risposte_Interviste( ")
            StrSQL.Append("       Audit_Tipo, Regolamento_Cod, Intervista_Cod, Intervista_SuperUser, Domanda_Cod, Valore, ")
            StrSQL.Append("       Inviato, DataInvio, ")
            StrSQL.Append("       Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("       UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("       Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("       ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(Audit_Tipo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Intervista_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Domanda_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valore) & "' ")
            ' StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
            '--------------------------------------------

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

    Public Function ModificaRisposte(
                        ByVal Audit_Tipo As Integer _
                        , ByVal Regolamento_Cod As Integer _
                        , ByVal Intervista_Cod As Integer _
                        , ByVal Piva As String _
                        , ByVal Domanda_Cod As Long _
                        , ByVal Valore As String _
                        , ByVal Validita_Inizio As Date _
                        , ByVal Validita_Fine As Date _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_creazione As Date = #2/1/1900# _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_creazione As String = "" _
                        , Optional ByVal username_modifica As String = ""
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Domande_Risposte_W.ModificaRisposte()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        If #2/1/1900# = Data_creazione Then
            Data_creazione = Date.Now
        End If

        If #2/1/1900# = Data_modifica Then
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
            StrSQL.Append("UPDATE Audit_Risposte_Interviste SET ")
            StrSQL.Append("    Valore ='" & Agro_SQL_SaveText(Valore) & "' ")
            'StrSQL.Append("    Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
            'StrSQL.Append("   ,Inviato           =  0 ")
            'StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("    ,Data_Modifica =" & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.Append("    ,UserName_Modifica ='" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("    ,Validita_Inizio =" & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("    ,Validita_Fine =" & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Intervista_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & " ")
            StrSQL.Append(" AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.Append(" AND   Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & " ")
            StrSQL.Append(" AND   Domanda_Cod = " & Agro_SQL_SaveNum(Domanda_Cod) & " ")
            '--------------------------------------------

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

    Public Function CancellaRisposte(
                        ByVal Audit_Tipo As Integer,
                        ByVal Regolamento_Cod As Integer,
                        ByVal Intervista_Cod As Integer,
                        ByVal Domanda_Cod As Integer,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_Interviste_W.CancellaRisposte()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append("DELETE FROM Audit_Risposte_Interviste")
            StrSQL.Append(" WHERE Intervista_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & " ")
            StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.Append(" AND Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & " ")

            If (Domanda_Cod <> 0) Then
                StrSQL.Append(" AND Domanda_Cod = " & Agro_SQL_SaveNum(Domanda_Cod) & " ")
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
