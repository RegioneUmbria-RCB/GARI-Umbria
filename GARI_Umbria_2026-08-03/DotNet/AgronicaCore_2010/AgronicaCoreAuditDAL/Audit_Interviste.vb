Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Audit_Interviste_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                            ByVal Audit_Tipo As Int32,
                            ByVal Regolamento_Cod As Int32,
                            ByVal Intervista_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Interviste_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   PUA_Cod = 0    
        '
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

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Audit_Interviste ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Intervista_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Intervista_Cod <> 0 Then
                StrSQL.Append(" AND Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & "   ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

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
                StrSQL.Append(" ORDER BY Intervista_SuperUser, Piva, Audit_Tipo, Intervista_Cod Asc ")
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

    '##############################################################################################
    Public Function LeggiInterviste(
                                ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Intervista_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Interviste_R.LeggiInterviste()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.AppendLine(" SELECT Audit_Interviste.*, Imprese.Rag_Soc,")
            StrSQL.AppendLine("     CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Audit_Interviste.PIVA ELSE Imprese.partitaIvaReale END PivaReale")

            If Audit_Tipo > 10 Then
                StrSQL.AppendLine(", i.piva AS Piva_OP, i.Rag_Soc AS Rag_Soc_OP, ic1010.val_cod AS CUAA")
            End If

            StrSQL.AppendLine(" FROM  Audit_Interviste ")
            StrSQL.AppendLine(" INNER Join Imprese ON Audit_Interviste.Piva = Imprese.PIVA ")

            If Audit_Tipo > 10 Then
                StrSQL.AppendLine(" LEFT JOIN GerarchiaImprese gi on gi.Figlio=Audit_Interviste.Piva ")
                StrSQL.AppendLine(" LEFT JOIN Imprese i on i.piva=gi.Padre ")
                StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic on ic.PIVA=Audit_Interviste.Piva And ic.id_cod=1088 ")
                StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic1010 on ic1010.PIVA=Audit_Interviste.Piva And ic1010.id_cod=1010 ")
            End If

            StrSQL.AppendLine(" WHERE Audit_Interviste.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" And   Audit_Interviste.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" And Audit_Interviste.Intervista_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Intervista_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Interviste.Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & "   ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.AppendLine(" AND Audit_Interviste.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Interviste.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            ' filtro su visibilita imprese
            If Piva <> "" Then
                StrSQL.AppendLine(" AND Audit_Interviste.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            Else
                Dim FiltroImprese As String = ""
                Dim UtentiVisibilitaLeggi As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim ImpreseVisibili As DataTable = UtentiVisibilitaLeggi.Leggi(1, "", "", objParametri)
                If Not ImpreseVisibili Is Nothing AndAlso ImpreseVisibili.Rows.Count > 0 Then
                    For i = 0 To ImpreseVisibili.Rows.Count - 1
                        FiltroImprese &= "'" & Agro_SQL_SaveText(ImpreseVisibili.Rows(i).Item("Piva")) & "',"
                    Next
                    If FiltroImprese <> "" Then
                        StrSQL.AppendLine(" AND Audit_Interviste.Piva IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroImprese, FiltroImprese.Length - 1), True) & ") ")
                    End If
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Audit_Interviste.Intervista_SuperUser, Audit_Interviste.Piva, Audit_Interviste.Audit_Tipo, Audit_Interviste.Intervista_Cod Asc ")
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

Public Class Audit_Interviste_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
                        ByVal Audit_Tipo As Integer _
                        , ByVal Regolamento_Cod As Integer _
                        , ByVal Intervista_Cod As Integer _
                        , ByVal Piva As String _
                        , ByVal Validita_Inizio As Date _
                        , ByVal Validita_Fine As Date _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_creazione As Date = #2/1/1900# _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_creazione As String = "" _
                        , Optional ByVal username_modifica As String = "" _
                        , Optional ByVal Intervista_Nome As String = ""
                                ) As Boolean




        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Interviste_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Audit_Interviste( ")
            StrSQL.Append("       Audit_Tipo, Regolamento_Cod, Intervista_Cod, Intervista_SuperUser, Piva,  ")
            StrSQL.Append("       Inviato, DataInvio, ")
            StrSQL.Append("       Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("       UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("       Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("       Intervista_Nome ")
            StrSQL.Append("       ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(Audit_Tipo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Intervista_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Intervista_Nome) & "' ")
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

    Public Function Modifica(
                        ByVal Audit_Tipo As Integer _
                        , ByVal Regolamento_Cod As Integer _
                        , ByVal Intervista_Cod As Integer _
                        , ByVal Piva As String _
                        , ByVal Validita_Inizio As Date _
                        , ByVal Validita_Fine As Date _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_creazione As Date = #2/1/1900# _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_creazione As String = "" _
                        , Optional ByVal username_modifica As String = "" _
                        , Optional ByVal Intervista_Nome As String = ""
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Interviste_W.Modifica()"

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
            StrSQL.Append("UPDATE Audit_Interviste SET ")
            StrSQL.Append("    Piva ='" & Agro_SQL_SaveText(Piva) & "' ")
            'StrSQL.Append("   ,Inviato           =  0 ")
            'StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("    ,Data_Modifica =" & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.Append("    ,UserName_Modifica ='" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("    ,Validita_Inizio =" & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("    ,Validita_Fine =" & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("    ,Intervista_Nome ='" & Agro_SQL_SaveText(Intervista_Nome) & "' ")
            StrSQL.Append(" WHERE Intervista_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & " ")
            StrSQL.Append(" AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.Append(" AND   Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & " ")
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

    Public Function Cancella(
                        ByVal Audit_Tipo As Integer _
                        , ByVal Regolamento_Cod As Integer _
                        , ByVal Intervista_Cod As Integer _
                        , ByVal Piva As String _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_creazione As Date = #2/1/1900# _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_creazione As String = "" _
                        , Optional ByVal username_modifica As String = ""
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Interviste_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("DELETE FROM Audit_Interviste WHERE 1=1 ")
            StrSQL.Append(" AND Intervista_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & " ")
            StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.Append(" AND Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod) & " ")
            '--------------------------------------------
            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
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
