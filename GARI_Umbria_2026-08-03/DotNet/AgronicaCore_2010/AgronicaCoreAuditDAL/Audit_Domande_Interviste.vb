Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Audit_Domande_Interviste_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                            ByVal Audit_Tipo As Int32,
                            ByVal Regolamento_Cod As Int32,
                            ByVal Intervista_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Domande_Interviste_R.Leggi()"

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
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Audit_Domande_Interviste ")

            ' se sto caricando una nuova profilazione, carico solo le domande valide
            If Intervista_Cod = 0 Then
                StrSQL.Append(" WHERE Validita_Fine > GETDATE() ")
            Else
                ' se sto caricando una profilazione in modifica, recupero le domande che copilai grazie all'inner join
                StrSQL.Append(" INNER JOIN Audit_Risposte_Interviste ON Audit_Domande_Interviste.Domanda_Cod = Audit_Risposte_Interviste.Domanda_Cod ")
                StrSQL.Append(" AND Audit_Domande_Interviste.Audit_Tipo = Audit_Risposte_Interviste.Audit_Tipo ")
                StrSQL.Append(" AND Audit_Domande_Interviste.Regolamento_Cod = Audit_Risposte_Interviste.Regolamento_Cod ")
                StrSQL.Append(" WHERE Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod))
                StrSQL.Append(" AND   Intervista_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Domande_Interviste.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Domande_Interviste.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Audit_Domande_Interviste.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Audit_Domande_Interviste.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ordine ")
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

    Public Function LeggiDomande(
                            ByVal Audit_Tipo As Int32,
                            ByVal Regolamento_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Domande_Interviste_R.LeggiDomande()"

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
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Audit_Domande_Interviste ")
            StrSQL.Append(" WHERE Validita_Fine > GETDATE() ")

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Domande_Interviste.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Domande_Interviste.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ordine ")
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
